// Decompiled with JetBrains decompiler
// Type: BlueStacks.Uninstaller.UninstallerStats
// Assembly: BlueStacksUninstaller, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: DBF002A0-6BF3-43CC-B5E7-0E90D1C19949
// Assembly location: C:\Program Files\BlueStacks\BlueStacksUninstaller.exe

using BlueStacks.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace BlueStacks.Uninstaller
{
  public class UninstallerStats
  {
    public static int UninstallerComment;

    public static string UninstallerStatsUrl
    {
      get
      {
        return string.Format("{0}{1}", (object) UninstallerProperties.CloudHost, (object) "/bs3/stats/unified_install_stats");
      }
    }

    public static Dictionary<string, string> GetCommonData(string eventName)
    {
      return new Dictionary<string, string>()
      {
        {
          "event",
          eventName
        },
        {
          "install_id",
          UninstallerProperties.UninstallId
        },
        {
          "engine_version",
          "4.250.0.1070"
        },
        {
          "client_version",
          "4.250.0.1070"
        },
        {
          "installer_arch",
          UninstallerStats.GetInstallerArch()
        },
        {
          "guid",
          UninstallerProperties.GUID
        },
        {
          "oem",
          "bgp"
        },
        {
          "campaign_hash",
          UninstallerProperties.CampaignMD5
        },
        {
          "campaign_name",
          UninstallerProperties.CampaignName
        },
        {
          "locale",
          UninstallerProperties.CurrentLocale
        },
        {
          "time_since_launch",
          (App.sClock.ElapsedMilliseconds / 1000L).ToString()
        },
        {
          "comment",
          UninstallerStats.UninstallerComment.ToString()
        }
      };
    }

    private static string GetInstallerArch()
    {
      return SystemUtils.IsOs64Bit() ? InstallerArchitectures.AMD64 : InstallerArchitectures.X86;
    }

    public static void SendUninstallCompletedStats(string userComment, string uninstallReason)
    {
      UninstallerStats.SendStats(UninstallerStatsEvent.UninstallCompleted, new Dictionary<string, string>()
      {
        {
          "user_comment",
          userComment
        },
        {
          "uninstall_reason",
          uninstallReason
        }
      });
    }

    public static void SendStatsAsync(string uninstallEvent)
    {
      new Thread((ThreadStart) (() => UninstallerStats.SendStats(uninstallEvent, (Dictionary<string, string>) null)))
      {
        IsBackground = true
      }.Start();
    }

    public static void SendStats(string uninstallEvent, Dictionary<string, string> extraData = null)
    {
      Dictionary<string, string> commonData = UninstallerStats.GetCommonData(uninstallEvent);
      if (extraData != null)
      {
        foreach (KeyValuePair<string, string> keyValuePair in extraData)
          commonData.Add(keyValuePair.Key, keyValuePair.Value);
      }
      try
      {
        string str = BstHttpClient.Post(UninstallerStats.UninstallerStatsUrl, commonData, (Dictionary<string, string>) null, false, (string) null, 0, 1, 0, false, "bgp");
        Logger.Debug(string.Format("Response for event {0}: {1}", (object) uninstallEvent, (object) str));
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to send stats for event: " + uninstallEvent);
        Logger.Error(ex.ToString());
      }
    }

    public static void ReportUninstallFailedStats(string uninstallEvent)
    {
      Dictionary<string, string> commonData = UninstallerStats.GetCommonData(uninstallEvent);
      commonData.Add("failure_reason", UninstallerStats.ParseEnumToString(UninstallerStats.UninstallerComment));
      try
      {
        Logger.Debug("Response for uninstall failed logs upload : " + BstHttpClient.HTTPGaeFileUploader(UninstallerStats.UninstallerStatsUrl, commonData, (Dictionary<string, string>) null, UninstallerProperties.LogFilePath, "text/plain", false, (string) null));
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to upload uninstall failed logs.");
        Logger.Error(ex.ToString());
      }
    }

    private static string ParseEnumToString(int enumVal)
    {
      string str = "";
      foreach (UninstallerComments uninstallerComments in Enum.GetValues(typeof (UninstallerComments)).Cast<UninstallerComments>().ToList<UninstallerComments>())
      {
        if (((UninstallerComments) enumVal & uninstallerComments) != (UninstallerComments) 0)
          str = str + UninstallerStats.ConvertIntToEnumString((int) uninstallerComments) + ",";
      }
      int length = str.LastIndexOf(',');
      return length > 0 ? str.Substring(0, length) : "";
    }

    private static string ConvertIntToEnumString(int enumCode)
    {
      return Enum.GetName(typeof (UninstallerComments), (object) (UninstallerComments) enumCode);
    }
  }
}
