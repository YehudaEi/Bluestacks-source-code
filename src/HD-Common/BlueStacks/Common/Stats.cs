// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.Stats
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;

namespace BlueStacks.Common
{
  public static class Stats
  {
    public const string AppInstall = "true";
    public const string AppUninstall = "false";
    private static string sSessionId;

    private static string SessionId
    {
      get
      {
        if (Stats.sSessionId == null)
          Stats.ResetSessionId();
        return Stats.sSessionId;
      }
      set
      {
        Stats.sSessionId = value;
      }
    }

    public static string GetSessionId()
    {
      return Stats.SessionId;
    }

    public static string ResetSessionId()
    {
      Stats.SessionId = Stats.Timestamp;
      return Stats.SessionId;
    }

    public static void SendAppStats(
      string appName,
      string packageName,
      string appVersion,
      string homeVersion,
      Stats.AppType appType,
      string vmName,
      string appVersionName = "")
    {
      Stats.SendAppStats(appName, packageName, appVersion, homeVersion, appType, (string) null, vmName, appVersionName);
    }

    public static void SendAppStats(
      string appName,
      string packageName,
      string appVersion,
      string homeVersion,
      Stats.AppType appType,
      string source,
      string vmName,
      string appVersionName)
    {
      new Thread((ThreadStart) (() =>
      {
        try
        {
          string url = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) RegistryManager.Instance.Host, (object) "stats/appclickstats");
          Dictionary<string, string> data = new Dictionary<string, string>()
          {
            {
              "email",
              Stats.GetURLSafeBase64String(RegistryManager.Instance.RegisteredEmail)
            },
            {
              "app_name",
              Stats.GetURLSafeBase64String(appName)
            },
            {
              "app_pkg",
              Stats.GetURLSafeBase64String(packageName)
            },
            {
              "app_ver",
              Stats.GetURLSafeBase64String(appVersion)
            },
            {
              "home_app_ver",
              Stats.GetURLSafeBase64String(homeVersion)
            },
            {
              "user_time",
              Stats.GetURLSafeBase64String(Stats.Timestamp)
            },
            {
              "app_type",
              Stats.GetURLSafeBase64String(appType.ToString())
            },
            {
              "app_ver_name",
              Stats.GetURLSafeBase64String(appVersionName)
            }
          };
          if (source != null)
            data.Add(nameof (source), Stats.GetURLSafeBase64String(source));
          if (!string.IsNullOrEmpty(RegistryManager.Instance.ClientLaunchParams))
          {
            JObject jobject = JObject.Parse(RegistryManager.Instance.ClientLaunchParams);
            if (jobject["campaign_id"] != null)
              data.Add("externalsource_campaignid", Stats.GetURLSafeBase64String(jobject["campaign_id"].ToString()));
            if (jobject["source_version"] != null)
              data.Add("externalsource_version", Stats.GetURLSafeBase64String(jobject["source_version"].ToString()));
          }
          Logger.Info("Sending App Stats for: {0}", (object) appName);
          BstHttpClient.Post(url, data, (Dictionary<string, string>) null, false, vmName, 0, 1, 0, false, "bgp");
        }
        catch (Exception ex)
        {
          Logger.Error("Failed to send app stats. error: " + ex.ToString());
        }
      }))
      {
        IsBackground = true
      }.Start();
    }

    public static void SendWebAppChannelStats(
      string appName,
      string packageName,
      string homeVersion,
      string source,
      string vmName)
    {
      new Thread((ThreadStart) (() =>
      {
        string url = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) RegistryManager.Instance.Host, (object) "stats/webappchannelclickstats");
        Dictionary<string, string> data = new Dictionary<string, string>()
        {
          {
            "app_name",
            Stats.GetURLSafeBase64String(appName)
          },
          {
            "app_pkg",
            Stats.GetURLSafeBase64String(packageName)
          },
          {
            "home_app_ver",
            Stats.GetURLSafeBase64String(homeVersion)
          },
          {
            "user_time",
            Stats.GetURLSafeBase64String(Stats.Timestamp)
          },
          {
            "email",
            Stats.GetURLSafeBase64String(RegistryManager.Instance.RegisteredEmail)
          },
          {
            nameof (source),
            Stats.GetURLSafeBase64String(source)
          }
        };
        try
        {
          Logger.Info("Sending Channel App Stats for: {0}", (object) appName);
          Logger.Info("Got Channel App Stat response: {0}", (object) BstHttpClient.Post(url, data, (Dictionary<string, string>) null, false, vmName, 0, 1, 0, false, "bgp"));
        }
        catch (Exception ex)
        {
          Logger.Error(ex.ToString());
        }
      }))
      {
        IsBackground = true
      }.Start();
    }

    public static void SendSearchAppStats(string keyword, string vmName)
    {
      Stats.SendSearchAppStats(keyword, (string) null, vmName);
    }

    public static void SendSearchAppStats(string keyword, string source, string vmName)
    {
      new Thread((ThreadStart) (() =>
      {
        string url = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) RegistryManager.Instance.Host, (object) "stats/searchappstats");
        Dictionary<string, string> data = new Dictionary<string, string>()
        {
          {
            nameof (keyword),
            keyword
          }
        };
        if (source != null)
          data.Add(nameof (source), source);
        try
        {
          Logger.Info("Sending Search App Stats for: {0}", (object) keyword);
          Logger.Info("Got Search App Stat response: {0}", (object) BstHttpClient.Post(url, data, (Dictionary<string, string>) null, false, vmName, 0, 1, 0, false, "bgp"));
        }
        catch (Exception ex)
        {
          Logger.Error(ex.ToString());
        }
      }))
      {
        IsBackground = true
      }.Start();
    }

    public static void SendAppInstallStats(
      string appName,
      string packageName,
      string appVersion,
      string appVersionName,
      string appInstall,
      string isUpdate,
      string source,
      string vmName,
      string campaignName,
      string clientVersion,
      string apkType = "")
    {
      new Thread((ThreadStart) (() =>
      {
        string url = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) RegistryManager.Instance.Host, (object) "stats/appinstallstats");
        Dictionary<string, string> data = new Dictionary<string, string>()
        {
          {
            "email",
            Stats.GetURLSafeBase64String(RegistryManager.Instance.RegisteredEmail)
          },
          {
            "app_name",
            Stats.GetURLSafeBase64String(appName)
          },
          {
            "app_pkg",
            Stats.GetURLSafeBase64String(packageName)
          },
          {
            "app_ver",
            Stats.GetURLSafeBase64String(appVersion)
          },
          {
            "is_install",
            Stats.GetURLSafeBase64String(appInstall)
          },
          {
            "is_update",
            Stats.GetURLSafeBase64String(isUpdate)
          },
          {
            "user_time",
            Stats.GetURLSafeBase64String(Stats.Timestamp)
          },
          {
            "install_source",
            Stats.GetURLSafeBase64String(source)
          },
          {
            "utm_campaign",
            Stats.GetURLSafeBase64String(campaignName)
          },
          {
            "client_ver",
            Stats.GetURLSafeBase64String(clientVersion)
          },
          {
            "apk_type",
            Stats.GetURLSafeBase64String(apkType)
          },
          {
            "app_ver_name",
            Stats.GetURLSafeBase64String(appVersionName)
          }
        };
        if (!string.IsNullOrEmpty(RegistryManager.Instance.ClientLaunchParams))
        {
          JObject jobject = JObject.Parse(RegistryManager.Instance.ClientLaunchParams);
          if (jobject["campaign_id"] != null)
          {
            if (jobject["isFarmingInstance"] != null)
              data.Add("feature_campaign_id", Stats.GetURLSafeBase64String(jobject["campaign_id"].ToString()));
            else
              data.Add("externalsource_campaignid", Stats.GetURLSafeBase64String(jobject["campaign_id"].ToString()));
          }
          if (jobject["source_version"] != null)
            data.Add("externalsource_version", Stats.GetURLSafeBase64String(jobject["source_version"].ToString()));
        }
        try
        {
          Logger.Info("Sending App Install Stats for: {0}", (object) appName);
          Logger.Debug("Got App Install Stat response: {0}", (object) BstHttpClient.Post(url, data, (Dictionary<string, string>) null, false, vmName, 0, 1, 0, false, "bgp"));
        }
        catch (Exception ex)
        {
          Logger.Error("Error in Sending AppInstallStats : " + ex.ToString());
        }
      }))
      {
        IsBackground = true
      }.Start();
    }

    public static void SendSystemInfoStats(string vmName)
    {
      Stats.SendSystemInfoStatsAsync((string) null, true, (Dictionary<string, string>) null, (string) null, (string) null, (string) null, vmName);
    }

    public static void SendSystemInfoStatsAsync(
      string host,
      bool createRegKey,
      Dictionary<string, string> dataInfo,
      string guid,
      string pfDir,
      string pdDir,
      string vmName)
    {
      new Thread((ThreadStart) (() => Stats.SendSystemInfoStatsSync(host, createRegKey, dataInfo, guid, pfDir, pdDir, vmName)))
      {
        IsBackground = true
      }.Start();
    }

    public static string SendSystemInfoStatsSync(
      string host,
      bool createRegKey,
      Dictionary<string, string> dataInfo,
      string guid,
      string programFilesDir,
      string programDataDir,
      string vmName)
    {
      string str = "not sent";
      try
      {
        Dictionary<string, string> dictionary = Profile.Info();
        Logger.Info("Got Device Profile Info:");
        foreach (KeyValuePair<string, string> keyValuePair in dictionary)
          Logger.Info(keyValuePair.Key + " " + keyValuePair.Value);
        if (host == null)
          host = RegistryManager.Instance.Host;
        string url = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) host, (object) "stats/systeminfostats");
        Dictionary<string, string> data = new Dictionary<string, string>()
        {
          {
            "p",
            Stats.GetURLSafeBase64String(dictionary["Processor"])
          },
          {
            "nop",
            Stats.GetURLSafeBase64String(dictionary["NumberOfProcessors"])
          },
          {
            "g",
            Stats.GetURLSafeBase64String(dictionary["GPU"])
          },
          {
            "gd",
            Stats.GetURLSafeBase64String(dictionary["GPUDriver"])
          },
          {
            "o",
            Stats.GetURLSafeBase64String(dictionary["OS"])
          },
          {
            "osv",
            Stats.GetURLSafeBase64String(dictionary["OSVersion"])
          },
          {
            "sr",
            Stats.GetURLSafeBase64String(dictionary["ScreenResolution"])
          },
          {
            "dnv",
            Stats.GetURLSafeBase64String(dictionary["DotNetVersion"])
          },
          {
            "osl",
            Stats.GetURLSafeBase64String(CultureInfo.CurrentCulture.Name.ToLower(CultureInfo.InvariantCulture))
          },
          {
            "oem_info",
            Stats.GetURLSafeBase64String(dictionary["OEMInfo"])
          },
          {
            "ram",
            Stats.GetURLSafeBase64String(dictionary["RAM"])
          },
          {
            "machine_type",
            Stats.GetURLSafeBase64String(dictionary["OSVERSIONTYPE"])
          }
        };
        if (dataInfo != null)
        {
          data.Add("glmode", Stats.GetURLSafeBase64String(dataInfo["GlMode"]));
          data.Add("glrendermode", Stats.GetURLSafeBase64String(dataInfo["GlRenderMode"]));
          data.Add("gl_vendor", Stats.GetURLSafeBase64String(dataInfo["GlVendor"]));
          data.Add("gl_renderer", Stats.GetURLSafeBase64String(dataInfo["GlRenderer"]));
          data.Add("gl_version", Stats.GetURLSafeBase64String(dataInfo["GlVersion"]));
          data.Add("bstr", Stats.GetURLSafeBase64String(dataInfo["BlueStacksResolution"]));
          if (dataInfo.ContainsKey("gl_check"))
            data.Add("gl_check", Stats.GetURLSafeBase64String(dataInfo["gl_check"]));
          if (dataInfo.ContainsKey("supported_glmodes"))
            data.Add("supported_glmodes", Stats.GetURLSafeBase64String(dataInfo["supported_glmodes"]));
          if (dataInfo.ContainsKey("IsVulkanSupported"))
            data.Add("is_vulkan_supported", dataInfo["IsVulkanSupported"]);
        }
        else
        {
          data.Add("bstr", Stats.GetURLSafeBase64String(dictionary["BlueStacksResolution"]));
          data.Add("glmode", Stats.GetURLSafeBase64String(dictionary["GlMode"]));
          data.Add("glrendermode", Stats.GetURLSafeBase64String(dictionary["GlRenderMode"]));
        }
        try
        {
          string glVendor;
          string glRenderer;
          string glVersion;
          int graphicsInfo1 = Utils.GetGraphicsInfo(programFilesDir + "\\HD-GLCheck.exe", "2", out glVendor, out glRenderer, out glVersion, false);
          int graphicsInfo2 = Utils.GetGraphicsInfo(programFilesDir + "\\HD-GLCheck.exe", "3", out glVendor, out glRenderer, out glVersion, false);
          int graphicsInfo3 = Utils.GetGraphicsInfo(programFilesDir + "\\HD-GLCheck.exe", "1", out glVendor, out glRenderer, out glVersion, false);
          string originalString1 = graphicsInfo1 != 0 ? "0" : "1";
          string originalString2 = graphicsInfo2 != 0 ? "0" : "1";
          string originalString3 = graphicsInfo3 != 0 ? "0" : "1";
          data.Add("dx9check", Stats.GetURLSafeBase64String(originalString1));
          data.Add("dx11check", Stats.GetURLSafeBase64String(originalString2));
          data.Add("gl_check", Stats.GetURLSafeBase64String(originalString3));
        }
        catch (Exception ex)
        {
          Logger.Error("got exception when checking dxcheck and glcheck for sending to systeminfostats ex:{0}", (object) ex.ToString());
        }
        bool bBothCamera = false;
        if (!Utils.CheckTwoCameraPresentOnDevice(ref bBothCamera))
          Logger.Error("Check for Two Camera Present on Device Failed");
        Logger.Info("Two Camera present on Device: " + bBothCamera.ToString());
        data.Add("two_camera", bBothCamera ? "1" : "0");
        Logger.Info("TwoCamera Value: " + data["two_camera"]);
        if (guid != null)
          data.Add(nameof (guid), Stats.GetURLSafeBase64String(guid));
        data.Add("install_id", RegistryManager.Instance.InstallID);
        if (string.IsNullOrEmpty(programDataDir))
          programDataDir = RegistryStrings.UserDefinedDir;
        data.Add("program_data_drive_type", SSDCheck.IsMediaTypeSSD(programDataDir) ? "ssd" : "hdd");
        Logger.Info("Sending System Info Stats");
        vmName = "";
        str = BstHttpClient.Post(url, data, (Dictionary<string, string>) null, false, vmName, 10000, 1, 0, false, "bgp");
        Logger.Info("Got System Info  response: {0}", (object) str);
        if (createRegKey)
          RegistryManager.Instance.SystemStats = 1;
      }
      catch (Exception ex)
      {
        Logger.Error(ex.ToString());
      }
      return str;
    }

    public static void SendFrontendStatusUpdate(string evt, string vmName)
    {
      Logger.Info("SendFrontendStatusUpdate: evt {0}", (object) evt);
      Thread thread = new Thread((ThreadStart) (() =>
      {
        try
        {
          string url = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "http://127.0.0.1:{0}", (object) RegistryManager.Instance.AgentServerPort), (object) "FrontendStatusUpdate");
          Dictionary<string, string> data = new Dictionary<string, string>()
          {
            {
              "event",
              evt
            }
          };
          Dictionary<string, string> headers = new Dictionary<string, string>();
          if (!vmName.Equals("Android", StringComparison.OrdinalIgnoreCase))
            headers.Add("vmid", vmName.Split('_')[1]);
          Logger.Info("Sending FrontendStatusUpdate to {0}", (object) url);
          Logger.Info("Got FrontendStatusUpdate response: {0}", (object) BstHttpClient.Post(url, data, headers, false, vmName, 0, 10, 1000, false, "bgp"));
        }
        catch (Exception ex)
        {
          Logger.Error(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Error Occured, Err : {0}", (object) ex.ToString()));
        }
      }))
      {
        IsBackground = true
      };
      thread.Start();
      if (string.Compare(evt, "frontend-closed", StringComparison.OrdinalIgnoreCase) != 0)
        return;
      thread.Join(200);
    }

    public static void SendTimelineStats(
      long agent_timestamp,
      long sequence,
      string evt,
      long duration,
      string s1,
      string s2,
      string s3,
      string s4,
      string s5,
      string s6,
      string s7,
      string s8,
      string timezone,
      string locale,
      long from_timestamp,
      long to_timestamp,
      long from_ticks,
      long to_ticks,
      string vmName)
    {
      try
      {
        BstHttpClient.Post(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) RegistryManager.Instance.Host, (object) "stats/timelinestats4"), new Dictionary<string, string>()
        {
          {
            nameof (agent_timestamp),
            Stats.GetURLSafeBase64String(agent_timestamp.ToString((IFormatProvider) CultureInfo.InvariantCulture))
          },
          {
            nameof (sequence),
            Stats.GetURLSafeBase64String(sequence.ToString((IFormatProvider) CultureInfo.InvariantCulture))
          },
          {
            "event",
            Stats.GetURLSafeBase64String(evt)
          },
          {
            nameof (duration),
            Stats.GetURLSafeBase64String(duration.ToString((IFormatProvider) CultureInfo.InvariantCulture))
          },
          {
            nameof (s1),
            Stats.GetURLSafeBase64String(s1)
          },
          {
            nameof (s2),
            Stats.GetURLSafeBase64String(s2)
          },
          {
            nameof (s3),
            Stats.GetURLSafeBase64String(s3)
          },
          {
            nameof (s4),
            Stats.GetURLSafeBase64String(s4)
          },
          {
            nameof (s5),
            Stats.GetURLSafeBase64String(s5)
          },
          {
            nameof (s6),
            Stats.GetURLSafeBase64String(s6)
          },
          {
            nameof (s7),
            Stats.GetURLSafeBase64String(s7)
          },
          {
            nameof (s8),
            Stats.GetURLSafeBase64String(s8)
          },
          {
            nameof (timezone),
            Stats.GetURLSafeBase64String(timezone)
          },
          {
            nameof (locale),
            Stats.GetURLSafeBase64String(locale)
          },
          {
            nameof (from_timestamp),
            Stats.GetURLSafeBase64String(from_timestamp.ToString((IFormatProvider) CultureInfo.InvariantCulture))
          },
          {
            nameof (to_timestamp),
            Stats.GetURLSafeBase64String(to_timestamp.ToString((IFormatProvider) CultureInfo.InvariantCulture))
          },
          {
            nameof (from_ticks),
            Stats.GetURLSafeBase64String(from_ticks.ToString((IFormatProvider) CultureInfo.InvariantCulture))
          },
          {
            nameof (to_ticks),
            Stats.GetURLSafeBase64String(to_ticks.ToString((IFormatProvider) CultureInfo.InvariantCulture))
          }
        }, (Dictionary<string, string>) null, false, vmName, 0, 1, 0, false, "bgp");
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to send timeline stats for : " + evt);
        Logger.Error(ex.ToString());
      }
    }

    public static void SendBootStats(string type, bool booted, bool wait, string vmName)
    {
      Thread thread = new Thread((ThreadStart) (() =>
      {
        string url = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) RegistryManager.Instance.Host, (object) "stats/bootstats");
        string empty = string.Empty;
        if (!string.IsNullOrEmpty(RegistryManager.Instance.ClientLaunchParams))
        {
          JObject jobject = JObject.Parse(RegistryManager.Instance.ClientLaunchParams);
          if (jobject["campaign_id"] != null)
            empty = jobject["campaign_id"].ToString();
        }
        Dictionary<string, string> headers = new Dictionary<string, string>();
        if (!string.IsNullOrEmpty(empty))
          headers.Add("x_campaign_id", empty);
        Dictionary<string, string> data = new Dictionary<string, string>()
        {
          {
            nameof (type),
            Stats.GetURLSafeBase64String(type)
          },
          {
            nameof (booted),
            Stats.GetURLSafeBase64String(booted.ToString((IFormatProvider) CultureInfo.InvariantCulture))
          }
        };
        try
        {
          Logger.Info("Sending Boot Stats to {0}", (object) url);
          Logger.Info("Got Boot Stats response: {0}", (object) BstHttpClient.Post(url, data, headers, false, vmName, 0, 1, 0, false, "bgp"));
        }
        catch (Exception ex)
        {
          Logger.Error(ex.ToString());
        }
      }))
      {
        IsBackground = true
      };
      thread.Start();
      if (!wait || thread.Join(5000))
        return;
      thread.Abort();
    }

    public static void SendHomeScreenDisplayedStats(string vmName)
    {
      new Thread((ThreadStart) (() =>
      {
        string url = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) RegistryManager.Instance.Host, (object) "stats/homescreenstats");
        try
        {
          Logger.Info("Sending Home Screen Displayed Stats to {0}", (object) url);
          Logger.Info("Got Home Screen Displayed Stats response: {0}", (object) BstHttpClient.Get(url, (Dictionary<string, string>) null, false, vmName, 0, 1, 0, false, "bgp"));
        }
        catch (Exception ex)
        {
          Logger.Error(ex.ToString());
        }
      }))
      {
        IsBackground = true
      }.Start();
    }

    public static void SendBtvFunnelStatsSync(
      string network,
      string statEvent,
      string statDataKey,
      string statDataValue,
      string vmName)
    {
      string url = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) RegistryManager.Instance.Host, (object) "stats/btvfunnelstats");
      Dictionary<string, string> data = new Dictionary<string, string>()
      {
        {
          "session_id",
          Stats.SessionId
        },
        {
          "streaming_platform",
          network
        },
        {
          "event_type",
          statEvent
        }
      };
      if (statDataKey != null)
        data.Add(statDataKey, statDataValue);
      try
      {
        Logger.Info("Sending Btv Funnel Stats to {0}", (object) url);
        vmName = "";
        BstHttpClient.Post(url, data, (Dictionary<string, string>) null, false, vmName, 0, 1, 0, false, "bgp");
        Logger.Info("Sent Btv Funnel Stats");
      }
      catch (Exception ex)
      {
        Logger.Error(ex.ToString());
      }
    }

    public static void SendStyleAndThemeInfoStats(
      string actionName,
      string styleName,
      string themeName,
      string optionalParam,
      string vmName)
    {
      Stats.SendStyleAndThemeInfoStatsAsync(actionName, styleName, themeName, optionalParam, vmName);
    }

    public static void SendStyleAndThemeInfoStatsAsync(
      string actionName,
      string styleName,
      string themeName,
      string optionalParam,
      string vmName)
    {
      new Thread((ThreadStart) (() => Stats.SendStyleAndThemeInfoStatsSync(actionName, styleName, themeName, optionalParam, vmName)))
      {
        IsBackground = true
      }.Start();
    }

    public static void SendStyleAndThemeInfoStatsSync(
      string actionName,
      string styleName,
      string themeName,
      string optionalParam,
      string vmName)
    {
      try
      {
        Logger.Info("Sending Style and Theme Stats");
        Dictionary<string, string> data = Stats.CollectStyleAndThemeData(actionName, styleName, themeName, optionalParam);
        foreach (KeyValuePair<string, string> keyValuePair in data)
          Logger.Info(keyValuePair.Key + " " + keyValuePair.Value);
        Stats.SendData(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) RegistryManager.Instance.Host, (object) "/stats/miscellaneousstats"), data, vmName, 0);
      }
      catch (Exception ex)
      {
        Logger.Error(ex.ToString());
      }
    }

    public static void SendMiscellaneousStatsSync(
      string tag,
      string arg1,
      string arg2,
      string arg3,
      string arg4,
      string arg5,
      string arg6 = null,
      string arg7 = null,
      string arg8 = null,
      string vmName = "Android",
      int timeOut = 0)
    {
      try
      {
        Logger.Info("Sending miscellaneous stats for tag: {0}", (object) tag);
        Dictionary<string, string> data = new Dictionary<string, string>()
        {
          {
            nameof (tag),
            tag
          },
          {
            nameof (arg1),
            arg1
          },
          {
            nameof (arg2),
            arg2
          },
          {
            nameof (arg3),
            arg3
          },
          {
            nameof (arg4),
            arg4
          },
          {
            nameof (arg5),
            arg5
          },
          {
            nameof (arg6),
            arg6
          },
          {
            nameof (arg7),
            arg7
          },
          {
            nameof (arg8),
            arg8
          }
        };
        foreach (KeyValuePair<string, string> keyValuePair in data)
          Logger.Debug(keyValuePair.Key + " " + keyValuePair.Value);
        Stats.SendData(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) RegistryManager.Instance.Host, (object) "/stats/miscellaneousstats"), data, vmName, timeOut);
      }
      catch (Exception ex)
      {
        Logger.Error(ex.ToString());
      }
    }

    public static void SendMiscellaneousStatsAsync(
      string tag,
      string arg1,
      string arg2,
      string arg3,
      string arg4,
      string arg5,
      string arg6 = null,
      string arg7 = null,
      string arg8 = null,
      string vmName = "Android",
      int timeOut = 0)
    {
      new Thread((ThreadStart) (() => Stats.SendMiscellaneousStatsSync(tag, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, vmName, timeOut)))
      {
        IsBackground = true
      }.Start();
    }

    public static void SendMiscellaneousStatsAsyncForDMM(
      string arg1,
      string arg2,
      string arg3 = null,
      string arg4 = null,
      string arg5 = null,
      string vmName = "Android",
      int timeOut = 0)
    {
      if (!"bgp".Equals("dmm", StringComparison.InvariantCultureIgnoreCase))
        return;
      new Thread((ThreadStart) (() => Stats.SendMiscellaneousStatsSync("dmm_event", RegistryManager.Instance.UserGuid, arg1, arg2, arg3, arg4, arg5, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.InstallID, vmName, timeOut)))
      {
        IsBackground = true
      }.Start();
    }

    public static void SendGamingMouseStats(string jsonData, string vmName)
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>()
      {
        {
          "dataset_id",
          "SystemInfoStatsDataset"
        },
        {
          "table_id",
          "GamingMouseStats"
        },
        {
          "body",
          jsonData
        }
      };
      foreach (KeyValuePair<string, string> keyValuePair in dictionary)
        Logger.Info(keyValuePair.Key + " " + keyValuePair.Value);
      string url = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) RegistryManager.Instance.Host, (object) "bigquery/uploadtobigquery");
      vmName = "";
      Dictionary<string, string> data = dictionary;
      string vmName1 = vmName;
      Stats.SendData(url, data, vmName1, 0);
    }

    public static void SendData(
      string url,
      Dictionary<string, string> data,
      string vmName,
      int timeOut = 0)
    {
      Logger.Info("Sending stats to " + url);
      try
      {
        BstHttpClient.Post(url, data, (Dictionary<string, string>) null, false, vmName, timeOut, 1, 0, false, "bgp");
      }
      catch (Exception ex)
      {
        Logger.Error(ex.ToString());
      }
      Logger.Info("Sent stats");
    }

    public static void SendCommonClientStatsAsync(
      string featureType,
      string eventType,
      string vmName,
      string packageName = "",
      string extraInfo = "",
      string arg2 = "",
      string arg4 = "")
    {
      ThreadPool.QueueUserWorkItem((WaitCallback) (obj =>
      {
        try
        {
          Logger.Info("Sending Client Stats");
          Dictionary<string, string> data = new Dictionary<string, string>()
          {
            {
              "guid",
              RegistryManager.Instance.UserGuid
            },
            {
              "prod_ver",
              RegistryManager.Instance.ClientVersion
            },
            {
              "utm_campaign",
              string.Empty
            },
            {
              "feature_type",
              featureType
            },
            {
              "event_type",
              eventType
            },
            {
              "app_pkg",
              packageName
            },
            {
              "arg1",
              extraInfo
            },
            {
              nameof (arg2),
              arg2
            },
            {
              "arg3",
              RegistryManager.Instance.UserSelectedLocale
            }
          };
          if (!string.IsNullOrEmpty(arg4))
            data.Add("game_popup_id", arg4);
          Stats.SendData(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) RegistryManager.Instance.Host, (object) "bs4/stats/clientstats"), data, vmName, 0);
        }
        catch (Exception ex)
        {
          Logger.Error("Exception in sending client stats async err : " + ex.ToString());
        }
      }));
    }

    private static Dictionary<string, string> CollectStyleAndThemeData(
      string actionName,
      string styleName,
      string themeName,
      string optionalParam)
    {
      return new Dictionary<string, string>()
      {
        {
          "tag",
          "StyleAndThemeData"
        },
        {
          "arg1",
          RegistryManager.Instance.UserGuid
        },
        {
          "arg2",
          actionName
        },
        {
          "arg3",
          styleName
        },
        {
          "arg4",
          themeName
        },
        {
          "arg5",
          optionalParam
        }
      };
    }

    private static string Timestamp
    {
      get
      {
        DateTime now = DateTime.Now;
        long ticks1 = now.Ticks;
        now = DateTime.Parse("01/01/1970 00:00:00", (IFormatProvider) CultureInfo.InvariantCulture);
        long ticks2 = now.Ticks;
        return ((ticks1 - ticks2) / 10000000L).ToString((IFormatProvider) CultureInfo.InvariantCulture);
      }
    }

    private static string GetURLSafeBase64String(string originalString)
    {
      return string.IsNullOrEmpty(originalString) ? "" : Convert.ToBase64String(Encoding.UTF8.GetBytes(originalString));
    }

    public static void SendMultiInstanceStatsAsync(
      string vmId,
      string oem,
      string cloneType,
      string eventType,
      string timeCompletion,
      string exitCode,
      bool wait)
    {
      Thread thread = new Thread((ThreadStart) (() =>
      {
        string url = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) RegistryManager.Instance.Host, (object) "stats/multiinstancestats");
        Dictionary<string, string> data = new Dictionary<string, string>()
        {
          {
            "vm_id",
            Stats.GetURLSafeBase64String(vmId)
          },
          {
            nameof (oem),
            Stats.GetURLSafeBase64String(oem)
          },
          {
            "return_code",
            Stats.GetURLSafeBase64String(exitCode)
          },
          {
            "clone_type",
            Stats.GetURLSafeBase64String(cloneType)
          },
          {
            "event_type",
            Stats.GetURLSafeBase64String(eventType)
          },
          {
            "time_completed",
            Stats.GetURLSafeBase64String(timeCompletion)
          }
        };
        try
        {
          Logger.Info("Sending MultiInstance Stats to {0}", (object) url);
          Logger.Info("Got MultiInstance Stats response: {0}", (object) BstHttpClient.Post(url, data, (Dictionary<string, string>) null, false, vmId, 0, 1, 0, false, "bgp"));
        }
        catch (Exception ex)
        {
          Logger.Error(ex.ToString());
        }
      }))
      {
        IsBackground = true
      };
      thread.Start();
      if (!wait || thread.Join(5000))
        return;
      thread.Abort();
    }

    public static void SendMultiInstanceStatsAsync(
      string eventName,
      string displayName,
      string performance,
      string resolution,
      int abiValue,
      string dpi,
      int instanceCount,
      string oemOption,
      string prodVerOption,
      string arg1,
      string arg2,
      string vmId,
      string utmCampaign,
      bool isMim,
      string arg3 = "")
    {
      new Thread((ThreadStart) (() =>
      {
        string url = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) RegistryManager.Instance.Host, (object) "stats/multiinstancestats");
        Dictionary<string, string> data = new Dictionary<string, string>()
        {
          {
            "event_name",
            eventName
          },
          {
            "display_name",
            displayName
          },
          {
            nameof (performance),
            performance
          },
          {
            nameof (resolution),
            resolution
          },
          {
            "abi_value",
            abiValue.ToString((IFormatProvider) CultureInfo.InvariantCulture)
          },
          {
            nameof (dpi),
            dpi
          },
          {
            "instance_count",
            instanceCount.ToString((IFormatProvider) CultureInfo.InvariantCulture)
          },
          {
            "oem_option",
            oemOption
          },
          {
            "prod_ver_option",
            prodVerOption
          },
          {
            nameof (arg1),
            arg1
          },
          {
            nameof (arg2),
            arg2
          },
          {
            "vm_id",
            vmId
          },
          {
            "utm_campaign",
            utmCampaign
          },
          {
            "is_mim",
            isMim.ToString((IFormatProvider) CultureInfo.InvariantCulture)
          }
        };
        if (!string.IsNullOrEmpty(arg3))
          data.Add(nameof (arg3), arg3);
        try
        {
          Logger.Info("Sending MultiInstance Stats to {0}", (object) url);
          Logger.Info("Got MultiInstance Stats response: {0}", (object) BstHttpClient.Post(url, data, (Dictionary<string, string>) null, false, vmId, 0, 1, 0, false, "bgp"));
        }
        catch (Exception ex)
        {
          Logger.Error(ex.ToString());
        }
      }))
      {
        IsBackground = true
      }.Start();
    }

    public static void SendTroubleshooterStatsASync(
      string eventType,
      string issueName,
      string ver,
      string vm)
    {
      new Thread((ThreadStart) (() =>
      {
        Logger.Info("Sending Troubleshooter stats");
        string url = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) RegistryManager.Instance.Host, (object) "stats/troubleshooterlogs");
        Dictionary<string, string> data = new Dictionary<string, string>()
        {
          {
            "guid",
            Utils.GetUserGUID()
          },
          {
            "prod_ver",
            RegistryManager.Instance.Version
          },
          {
            "issue_name",
            issueName
          },
          {
            "event_type",
            eventType
          },
          {
            "country",
            Utils.GetUserCountry(vm)
          },
          {
            "oem",
            RegistryManager.Instance.Oem
          },
          {
            "locale",
            CultureInfo.CurrentCulture.ToString()
          },
          {
            "troubleshooter_ver",
            ver
          }
        };
        try
        {
          BstHttpClient.Post(url, data, (Dictionary<string, string>) null, false, vm, 5000, 1, 0, false, "bgp");
        }
        catch (Exception ex)
        {
          Logger.Error(ex.ToString());
        }
      }))
      {
        IsBackground = true
      }.Start();
    }

    public static Dictionary<string, string> GetUnifiedInstallStatsCommonData()
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>()
      {
        {
          "os",
          Profile.OS
        },
        {
          "oem",
          "bgp"
        },
        {
          "guid",
          RegistryManager.Instance.UserGuid
        },
        {
          "email",
          RegistryManager.Instance.RegisteredEmail
        },
        {
          "locale",
          RegistryManager.Instance.UserSelectedLocale
        },
        {
          "install_id",
          RegistryManager.Instance.InstallID
        },
        {
          "campaign_hash",
          RegistryManager.Instance.CampaignMD5
        },
        {
          "campaign_name",
          RegistryManager.Instance.CampaignName
        },
        {
          "client_version",
          "4.250.0.1070"
        },
        {
          "engine_version",
          "4.250.0.1070"
        },
        {
          "product_version",
          "4.250.0.1070"
        }
      };
      if (RegistryManager.Instance.InstallationType != InstallationTypes.FullEdition)
      {
        dictionary.Add("installation_type", RegistryManager.Instance.InstallationType.ToString());
        dictionary.Add("gaming_pkg_name", RegistryManager.Instance.InstallerPkgName);
      }
      return dictionary;
    }

    public static void SendUnifiedInstallStatsAsync(
      string eventName,
      string email = "",
      string vmname = "Android",
      string campaignID = "")
    {
      ThreadPool.QueueUserWorkItem((WaitCallback) (obj =>
      {
        try
        {
          Stats.SendUnifiedInstallStats(eventName, email, vmname, campaignID);
        }
        catch (Exception ex)
        {
          Logger.Error("An exception in sending unified install stats. Ex: {0}", (object) ex);
        }
      }));
    }

    public static string SendUnifiedInstallStats(
      string eventName,
      string email = "",
      string vmname = "Android",
      string campaignID = "")
    {
      string str = "";
      Dictionary<string, string> headers = new Dictionary<string, string>();
      if (!string.IsNullOrEmpty(campaignID))
        headers.Add("x_campaign_id", campaignID);
      Dictionary<string, string> installStatsCommonData = Stats.GetUnifiedInstallStatsCommonData();
      installStatsCommonData.Add("event", eventName);
      if (!string.IsNullOrEmpty(email))
        installStatsCommonData[nameof (email)] = email;
      try
      {
        HTTPUtils.SendRequestToCloud("/bs3/stats/unified_install_stats", installStatsCommonData, vmname, 0, headers, false, 1, 0, false);
        Logger.Debug(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Response for event {0}: {1}", (object) eventName, (object) str));
      }
      catch (Exception ex)
      {
        Logger.Warning("Failed to send stats for event: {0}, Ex: {1}", (object) eventName, (object) ex.Message);
      }
      return str;
    }

    public enum AppType
    {
      app,
      market,
      suggestedapps,
      web,
    }

    public enum DMMEvent
    {
      download_start,
      download_failed,
      download_complete,
      app_install_started,
      app_install_success,
      app_install_failed,
      agent_launched,
      get_progress_success,
      install_app,
      runapp_started,
      runapp_completed,
      client_launched,
      boot_failed,
      boot_success,
      is_app_installed,
    }
  }
}
