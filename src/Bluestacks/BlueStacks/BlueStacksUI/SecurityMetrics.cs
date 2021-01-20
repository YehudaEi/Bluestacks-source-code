// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.SecurityMetrics
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Timers;

namespace BlueStacks.BlueStacksUI
{
  internal class SecurityMetrics : IDisposable
  {
    private Dictionary<SecurityBreach, string> mSecurityBreachesList = new Dictionary<SecurityBreach, string>();
    private string mVmName;
    private System.Timers.Timer mTimer;
    private bool disposedValue;

    public static SerializableDictionary<string, SecurityMetrics> SecurityMetricsInstanceList { get; set; } = new SerializableDictionary<string, SecurityMetrics>();

    public SecurityMetrics(string vmName)
    {
      this.mVmName = vmName;
      this.mTimer = new System.Timers.Timer() { Interval = 86400000.0 };
      this.mTimer.Elapsed += new ElapsedEventHandler(this.OnTimedEvent);
      this.mTimer.AutoReset = true;
      this.mTimer.Enabled = true;
      new Thread((ThreadStart) (() =>
      {
        this.CheckMd5HashOfRootVdi();
        this.CheckAppPlayerRootInfoFromAndroidBstk();
      }))
      {
        IsBackground = true
      }.Start();
    }

    private void OnTimedEvent(object sender, ElapsedEventArgs e)
    {
      this.SendSecurityBreachesStatsToCloud(false);
    }

    internal static void Init(string vmName)
    {
      if (SecurityMetrics.SecurityMetricsInstanceList.ContainsKey(vmName))
        return;
      SecurityMetrics.SecurityMetricsInstanceList.Add(vmName, new SecurityMetrics(vmName));
    }

    internal void SendSecurityBreachesStatsToCloud(bool isOnClose = false)
    {
      new Thread((ThreadStart) (() =>
      {
        try
        {
          this.AddBlacklistedRunningApplicationsToSecurityBreaches();
          if (this.mSecurityBreachesList.Count > 0)
            BstHttpClient.Post(WebHelper.GetUrlWithParams(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) RegistryManager.Instance.Host, (object) "/bs4/security_metrics"), (string) null, (string) null, (string) null), new Dictionary<string, string>()
            {
              {
                "security_metric_data",
                this.GetSecurityMetricsData()
              }
            }, (Dictionary<string, string>) null, false, this.mVmName, 10000, 1, 0, false, "bgp");
        }
        catch (Exception ex)
        {
          Logger.Error("Exception while sending security stats to cloud : {0}", (object) ex.ToString());
        }
        if (!isOnClose)
          return;
        SecurityMetrics.SecurityMetricsInstanceList.Remove(this.mVmName);
      })).Start();
    }

    private string GetSecurityMetricsData()
    {
      string empty = string.Empty;
      StringBuilder sb = new StringBuilder();
      using (StringWriter stringWriter = new StringWriter(sb))
      {
        JsonTextWriter jsonTextWriter = new JsonTextWriter((TextWriter) stringWriter);
        jsonTextWriter.Formatting = Formatting.Indented;
        using (JsonWriter jsonWriter = (JsonWriter) jsonTextWriter)
        {
          jsonWriter.WriteStartObject();
          foreach (SecurityBreach key in this.mSecurityBreachesList.Keys)
          {
            switch (key)
            {
              case SecurityBreach.SCRIPT_TOOLS:
                jsonWriter.WritePropertyName(key.ToString().ToLower(CultureInfo.InvariantCulture));
                jsonWriter.WriteStartObject();
                jsonWriter.WritePropertyName("running_blacklist_programs");
                jsonWriter.WriteValue(this.mSecurityBreachesList[key]);
                jsonWriter.WriteEndObject();
                continue;
              case SecurityBreach.DEVICE_PROBED:
              case SecurityBreach.DEVICE_ROOTED:
              case SecurityBreach.DEVICE_PROFILE_CHANGED:
              case SecurityBreach.SYNTHETIC_INPUT:
                jsonWriter.WritePropertyName(key.ToString().ToLower(CultureInfo.InvariantCulture));
                jsonWriter.WriteValue(this.mSecurityBreachesList[key]);
                continue;
              default:
                continue;
            }
          }
          jsonWriter.WriteEndObject();
          empty = sb.ToString();
          Logger.Debug("security data " + empty);
        }
      }
      return empty;
    }

    private void AddBlacklistedRunningApplicationsToSecurityBreaches()
    {
      List<string> applicationsList = PromotionObject.Instance.BlackListedApplicationsList;
      List<string> stringList = new List<string>();
      foreach (string name in applicationsList)
      {
        if (ProcessUtils.FindProcessByName(name))
          stringList.Add(name);
      }
      if (stringList.Count <= 0)
        return;
      this.AddSecurityBreach(SecurityBreach.SCRIPT_TOOLS, JsonConvert.SerializeObject((object) stringList));
    }

    internal void AddSecurityBreach(SecurityBreach breach, string data)
    {
      try
      {
        if (this.mSecurityBreachesList.ContainsKey(breach))
          return;
        this.mSecurityBreachesList.Add(breach, data);
        Logger.Info("Security breach added for: {0}", (object) breach);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in adding security breach: {0}", (object) ex.ToString());
      }
    }

    internal void CheckMd5HashOfRootVdi()
    {
      try
      {
        string blockDevice0Path = RegistryManager.Instance.Guest["Android"].BlockDevice0Path;
        string rootVdiMd5Hash = RegistryManager.Instance.RootVdiMd5Hash;
        if (string.IsNullOrEmpty(rootVdiMd5Hash))
        {
          Utils.CreateMD5HashOfRootVdi();
        }
        else
        {
          string md5HashFromFile = Utils.GetMD5HashFromFile(blockDevice0Path);
          if (string.IsNullOrEmpty(md5HashFromFile) || string.Equals(md5HashFromFile, rootVdiMd5Hash, StringComparison.OrdinalIgnoreCase))
            return;
          this.AddSecurityBreach(SecurityBreach.DEVICE_ROOTED, string.Empty);
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in checking md5 hash of root vdi: {0}", (object) ex);
      }
    }

    private void CheckAppPlayerRootInfoFromAndroidBstk()
    {
      try
      {
        JArray jarray = JArray.Parse(HTTPUtils.SendRequestToEngine("isAppPlayerRooted", (Dictionary<string, string>) null, this.mVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "", "bgp"));
        if (!(bool) jarray[0][(object) "success"] || !(bool) jarray[0][(object) "isRooted"])
          return;
        this.AddSecurityBreach(SecurityBreach.DEVICE_ROOTED, string.Empty);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in checking root info from engine: {0}", (object) ex.ToString());
      }
    }

    protected virtual void Dispose(bool disposing)
    {
      if (this.disposedValue)
        return;
      int num = disposing ? 1 : 0;
      if (this.mTimer != null)
      {
        this.mTimer.Elapsed -= new ElapsedEventHandler(this.OnTimedEvent);
        this.mTimer.Dispose();
      }
      this.disposedValue = true;
    }

    ~SecurityMetrics()
    {
      this.Dispose(false);
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }
  }
}
