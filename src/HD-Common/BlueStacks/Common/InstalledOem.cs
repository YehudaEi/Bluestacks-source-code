// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.InstalledOem
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace BlueStacks.Common
{
  public static class InstalledOem
  {
    private static readonly object listLock = new object();
    public static readonly int[] BGP6432BitABIValues = new int[7]
    {
      1,
      2,
      3,
      4,
      5,
      6,
      7
    };
    private static BackgroundWorker mBgwGetOem = (BackgroundWorker) null;
    private static List<string> mAllInstalledOemList;
    private static List<string> mInstalledCoexistingOemList;
    private static ObservableCollection<AppPlayerModel> mCoexistingOemList;

    public static List<string> AllInstalledOemList
    {
      get
      {
        InstalledOem.SetAllInstalledOems();
        return InstalledOem.mAllInstalledOemList;
      }
      private set
      {
        if (InstalledOem.mAllInstalledOemList == value)
          return;
        InstalledOem.mAllInstalledOemList = value;
      }
    }

    public static void SetAllInstalledOems()
    {
      List<string> stringList = new List<string>()
      {
        "bgp"
      };
      try
      {
        RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("Software");
        foreach (string subKeyName in registryKey.GetSubKeyNames())
        {
          if (subKeyName.StartsWith("BlueStacks", StringComparison.OrdinalIgnoreCase) && !subKeyName.StartsWith("BlueStacksGP", StringComparison.OrdinalIgnoreCase) && (!subKeyName.StartsWith("BlueStacksInstaller", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty((string) Utils.GetRegistryHKLMValue("Software\\" + subKeyName, "Version", (object) ""))))
          {
            string registryHklmValue = (string) Utils.GetRegistryHKLMValue("Software\\" + subKeyName + "\\Config", "Oem", (object) "bgp");
            stringList.AddIfNotContain<string>(registryHklmValue);
          }
        }
        registryKey.Close();
        RegistryManager.SetRegistryManagers(stringList);
        InstalledOem.AllInstalledOemList = stringList;
      }
      catch (Exception ex)
      {
        Logger.Info("Error in finding installed oems " + ex.ToString());
      }
    }

    public static List<string> InstalledCoexistingOemList
    {
      get
      {
        InstalledOem.SetInstalledCoexistingOems();
        return InstalledOem.mInstalledCoexistingOemList;
      }
      private set
      {
        if (InstalledOem.mInstalledCoexistingOemList == value)
          return;
        InstalledOem.mInstalledCoexistingOemList = value;
        RegistryManager.SetRegistryManagers(InstalledOem.mInstalledCoexistingOemList);
      }
    }

    public static void SetInstalledCoexistingOems()
    {
      List<string> stringList = new List<string>()
      {
        "bgp"
      };
      if (Oem.Instance.IsShowMimOtherOEM)
      {
        foreach (AppPlayerModel appPlayerModel in InstalledOem.CoexistingOemList.ToList<AppPlayerModel>())
        {
          string str = appPlayerModel.AppPlayerOem.Equals("bgp", StringComparison.InvariantCultureIgnoreCase) ? "" : "_" + appPlayerModel.AppPlayerOem;
          RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("Software\\BlueStacks" + str + "\\Config");
          if (registryKey != null && !string.IsNullOrEmpty((string) Utils.GetRegistryHKLMValue("Software\\BlueStacks" + str, "Version", (object) "")))
          {
            registryKey.Close();
            stringList.AddIfNotContain<string>(appPlayerModel.AppPlayerOem);
          }
        }
      }
      RegistryManager.SetRegistryManagers(stringList);
      InstalledOem.InstalledCoexistingOemList = stringList;
      Logger.Info("InstalledCoexistingOemList: " + string.Join(",", stringList.ToArray()));
    }

    public static ObservableCollection<AppPlayerModel> CoexistingOemList
    {
      get
      {
        if (InstalledOem.mCoexistingOemList == null || InstalledOem.mCoexistingOemList.Count < 0)
        {
          InstalledOem.mCoexistingOemList = JsonConvert.DeserializeObject<ObservableCollection<AppPlayerModel>>(RegistryManager.Instance.AppPlayerEngineInfo, Utils.GetSerializerSettings());
          if (!InstalledOem.mCoexistingOemList.Where<AppPlayerModel>((Func<AppPlayerModel, bool>) (x => string.Equals(x.AppPlayerOem, "bgp", StringComparison.InvariantCultureIgnoreCase))).Any<AppPlayerModel>())
          {
            foreach (AppPlayerModel appPlayerModel in (Collection<AppPlayerModel>) JsonConvert.DeserializeObject<ObservableCollection<AppPlayerModel>>(Constants.DefaultAppPlayerEngineInfo, Utils.GetSerializerSettings()))
            {
              if (string.Equals(appPlayerModel.AppPlayerOem, "bgp", StringComparison.InvariantCultureIgnoreCase))
                InstalledOem.mCoexistingOemList.Add(appPlayerModel);
            }
          }
        }
        if (!InstalledOem.CloudResponseRecieved)
          InstalledOem.GetCoexistingOemsFromCloud();
        return InstalledOem.mCoexistingOemList;
      }
      private set
      {
        if (InstalledOem.mCoexistingOemList == value)
          return;
        InstalledOem.mCoexistingOemList = value;
      }
    }

    private static BackgroundWorker BGGetOem
    {
      get
      {
        if (InstalledOem.mBgwGetOem == null)
        {
          InstalledOem.mBgwGetOem = new BackgroundWorker();
          InstalledOem.mBgwGetOem.DoWork += new DoWorkEventHandler(InstalledOem.BgGetOem_DoWork);
          InstalledOem.mBgwGetOem.RunWorkerCompleted += new RunWorkerCompletedEventHandler(InstalledOem.BgGetOem_RunWorkerCompleted);
        }
        return InstalledOem.mBgwGetOem;
      }
    }

    public static bool CloudResponseRecieved { get; private set; } = false;

    public static void GetCoexistingOemsFromCloud()
    {
      InstalledOem.CloudResponseRecieved = false;
      if (InstalledOem.BGGetOem.IsBusy)
        return;
      InstalledOem.BGGetOem.RunWorkerAsync();
    }

    private static void BgGetOem_DoWork(object sender, DoWorkEventArgs e)
    {
      JToken jtoken = (JToken) null;
      try
      {
        jtoken = JToken.Parse(BstHttpClient.Get(InstalledOem.CreateRequestUrlAndDownloadJsonData(), (Dictionary<string, string>) null, false, "Android", 0, 1, 0, false, "bgp"));
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to get oem err: {0}", (object) ex.Message);
      }
      finally
      {
        e.Result = (object) jtoken;
      }
    }

    private static void BgGetOem_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      try
      {
        if (e.Result is JToken result)
          InstalledOem.ResetCoexistingOems(result);
        InstalledOem.CloudResponseRecieved = true;
        Logger.Debug("Oem List data Url: " + result?.ToString());
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to get oem err: {0}", (object) ex.Message);
      }
    }

    private static string CreateRequestUrlAndDownloadJsonData()
    {
      string urlOriginal = RegistryManager.Instance.Host + "/bs4/getmultiinstancebuild?";
      try
      {
        string osName = string.Empty;
        string str1 = "app_player";
        string str2 = "w" + SystemUtils.GetOSArchitecture().ToString();
        string userSelectedLocale = RegistryManager.Instance.UserSelectedLocale;
        SystemUtils.GetOSInfo(out osName, out string _, out string _);
        string urlOverideParams = "app_player_win_version=" + osName + "&source=" + str1 + "&app_player_os_arch=" + str2 + "&app_player_language=" + userSelectedLocale;
        urlOriginal = HTTPUtils.MergeQueryParams(urlOriginal, urlOverideParams, true);
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to create url err: {0}", (object) ex.Message);
      }
      return urlOriginal;
    }

    private static void ResetCoexistingOems(JToken jTokenResponse)
    {
      try
      {
        lock (InstalledOem.listLock)
        {
          ObservableCollection<AppPlayerModel> source1 = new ObservableCollection<AppPlayerModel>();
          JEnumerable<JToken> source2 = jTokenResponse.First<JToken>().Children();
          if (source2.Children<JToken>().Any<JToken>())
          {
            foreach (object child in (IEnumerable<JToken>) source2.Children<JToken>())
            {
              AppPlayerModel appPlayerModel = JsonConvert.DeserializeObject<AppPlayerModel>(child.ToString(), Utils.GetSerializerSettings());
              source1.Add(appPlayerModel);
            }
          }
          if (!source1.Where<AppPlayerModel>((Func<AppPlayerModel, bool>) (x => string.Equals(x.AppPlayerOem, "bgp", StringComparison.InvariantCultureIgnoreCase))).Any<AppPlayerModel>())
          {
            foreach (AppPlayerModel appPlayerModel in (Collection<AppPlayerModel>) JsonConvert.DeserializeObject<ObservableCollection<AppPlayerModel>>(Constants.DefaultAppPlayerEngineInfo, Utils.GetSerializerSettings()))
            {
              if (string.Equals(appPlayerModel.AppPlayerOem, "bgp", StringComparison.InvariantCultureIgnoreCase))
                source1.Add(appPlayerModel);
            }
          }
          string b = JsonConvert.SerializeObject((object) source1, Utils.GetSerializerSettings());
          if (!string.Equals(RegistryManager.Instance.AppPlayerEngineInfo, b, StringComparison.InvariantCultureIgnoreCase))
            RegistryManager.Instance.AppPlayerEngineInfo = b;
          InstalledOem.CoexistingOemList = source1;
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in parsing cloud response:" + ex?.ToString());
      }
    }

    public static bool CheckIfOemInstancePresent(string oem, string abi)
    {
      if (!string.IsNullOrEmpty(oem) && InstalledOem.InstalledCoexistingOemList.Contains(oem))
      {
        if (!oem.Contains("bgp64"))
          return true;
        int result;
        if (!int.TryParse(abi, out result))
          result = int.Parse(ABISetting.ARM64.GetDescription(), (IFormatProvider) CultureInfo.InvariantCulture);
        abi = ((IEnumerable<int>) InstalledOem.BGP6432BitABIValues).Contains<int>(result) ? ABISetting.Auto64.GetDescription() : ABISetting.ARM64.GetDescription();
        foreach (string vm in RegistryManager.RegistryManagers[oem].VmList)
        {
          if (string.Equals(abi, Utils.GetValueInBootParams("abivalue", vm, string.Empty, oem), StringComparison.InvariantCultureIgnoreCase))
            return true;
        }
      }
      return false;
    }

    public static void LaunchOemInstance(
      string oem,
      string abi,
      string vmname = "",
      string packageName = "",
      string actionWithRemainingInstances = "")
    {
      if (!InstalledOem.CheckIfOemInstancePresent(oem, abi))
        return;
      string partnerExePath = RegistryManager.RegistryManagers[oem].PartnerExePath;
      if (string.IsNullOrEmpty(vmname) || !((IEnumerable<string>) RegistryManager.RegistryManagers[oem].VmList).Contains<string>(vmname))
      {
        vmname = "Android";
        if (oem.Contains("bgp64"))
        {
          int result;
          if (!int.TryParse(abi, out result))
            result = int.Parse(ABISetting.ARM64.GetDescription(), (IFormatProvider) CultureInfo.InvariantCulture);
          abi = ((IEnumerable<int>) InstalledOem.BGP6432BitABIValues).Contains<int>(result) ? ABISetting.Auto64.GetDescription() : ABISetting.ARM64.GetDescription();
          foreach (string vm in RegistryManager.RegistryManagers[oem].VmList)
          {
            if (string.Equals(abi, Utils.GetValueInBootParams("abivalue", vm, string.Empty, oem), StringComparison.InvariantCultureIgnoreCase))
            {
              vmname = vm;
              break;
            }
          }
        }
      }
      string str = "-vmname " + vmname;
      if (!string.IsNullOrEmpty(packageName))
      {
        JObject jobject;
        if (new System.Version(RegistryManager.RegistryManagers[oem].Version) < new System.Version("4.210.0.0000"))
          jobject = new JObject()
          {
            {
              "app_pkg",
              (JToken) packageName
            }
          };
        else
          jobject = new JObject()
          {
            {
              "fle_pkg",
              (JToken) packageName
            },
            {
              "source",
              (JToken) "mim"
            }
          };
        if (jobject != null)
          str = str + " -json " + Uri.EscapeUriString(jobject.ToString(Formatting.None));
      }
      Process.Start(new ProcessStartInfo()
      {
        Arguments = str,
        UseShellExecute = false,
        CreateNoWindow = true,
        FileName = partnerExePath
      });
      if (string.Equals(actionWithRemainingInstances, "close", StringComparison.InvariantCultureIgnoreCase))
      {
        InstalledOem.ActionOnRemainingInstances("stopInstance", oem, vmname);
      }
      else
      {
        if (!string.Equals(actionWithRemainingInstances, "minimize", StringComparison.InvariantCultureIgnoreCase))
          return;
        InstalledOem.ActionOnRemainingInstances("minimizeInstance", oem, vmname);
      }
    }

    private static void ActionOnRemainingInstances(
      string route,
      string launchedOem,
      string launchedVmName)
    {
      foreach (string installedCoexistingOem in InstalledOem.InstalledCoexistingOemList)
      {
        if (ProcessUtils.IsAlreadyRunning("Global\\BlueStacks_BlueStacksUI_Lock" + installedCoexistingOem))
        {
          foreach (string vm in RegistryManager.RegistryManagers[installedCoexistingOem].VmList)
          {
            try
            {
              if (string.Equals(installedCoexistingOem, launchedOem, StringComparison.InvariantCultureIgnoreCase))
              {
                if (string.Equals(vm, launchedVmName, StringComparison.InvariantCultureIgnoreCase))
                  continue;
              }
              if (Utils.PingPartner(installedCoexistingOem, vm))
              {
                Logger.Info("Sending " + route + " call to oem:" + installedCoexistingOem + " vm:" + vm);
                HTTPUtils.SendRequestToClientAsync(route, (Dictionary<string, string>) null, vm, 0, (Dictionary<string, string>) null, false, 1, 0, installedCoexistingOem);
              }
            }
            catch (Exception ex)
            {
              Logger.Info(string.Format("Error Sending {0} call to oem: {1} vm: {2} with exception: {3}", (object) route, (object) installedCoexistingOem, (object) vm, (object) ex));
            }
          }
        }
      }
    }

    public static AppPlayerModel GetAppPlayerModel(string oem, string abi)
    {
      if (string.IsNullOrEmpty(oem))
        oem = "bgp";
      if (!oem.Contains("bgp64") || oem.Contains("bgp64_hyperv") || oem.Contains("msi64_hyperv"))
        return InstalledOem.CoexistingOemList.FirstOrDefault<AppPlayerModel>((Func<AppPlayerModel, bool>) (x => x != null && x.AppPlayerOem == oem));
      int result;
      if (!int.TryParse(abi, out result))
        result = int.Parse(ABISetting.ARM64.GetDescription(), (IFormatProvider) CultureInfo.InvariantCulture);
      abi = ((IEnumerable<int>) InstalledOem.BGP6432BitABIValues).Contains<int>(result) ? ABISetting.Auto64.GetDescription() : ABISetting.ARM64.GetDescription();
      return InstalledOem.CoexistingOemList.FirstOrDefault<AppPlayerModel>((Func<AppPlayerModel, bool>) (x => x != null && x.AppPlayerOem == oem && string.Equals(x.AbiValue.ToString((IFormatProvider) CultureInfo.InvariantCulture), abi, StringComparison.InvariantCultureIgnoreCase)));
    }

    public static string GetOemFromVmnameWithSuffix(string vmNameWithSuffix)
    {
      string str = "bgp";
      foreach (AppPlayerModel coexistingOem in (Collection<AppPlayerModel>) InstalledOem.CoexistingOemList)
      {
        string appPlayerOem = coexistingOem.AppPlayerOem;
        if (vmNameWithSuffix.EndsWith(appPlayerOem, StringComparison.InvariantCultureIgnoreCase))
        {
          str = appPlayerOem;
          break;
        }
      }
      return str;
    }
  }
}
