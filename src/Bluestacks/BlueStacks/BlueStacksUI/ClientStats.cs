// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.ClientStats
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace BlueStacks.BlueStacksUI
{
  internal class ClientStats
  {
    private static string sDevUrl = RegistryManager.Instance.BGPDevUrl;

    internal static Dictionary<string, string> GetCommonData
    {
      get
      {
        Dictionary<string, string> dictionary = new Dictionary<string, string>()
        {
          {
            "guid",
            RegistryManager.Instance.UserGuid
          },
          {
            "engine_guid",
            RegistryManager.Instance.UserGuid
          },
          {
            "engine_ver",
            RegistryManager.Instance.Version
          },
          {
            "client_ver",
            RegistryManager.Instance.ClientVersion
          },
          {
            "oem",
            Oem.Instance.OEM
          },
          {
            "campaign_md5",
            RegistryManager.Instance.CampaignMD5
          },
          {
            "partner",
            RegistryManager.Instance.Partner
          },
          {
            "lang",
            RegistryManager.Instance.UserSelectedLocale
          },
          {
            "email",
            RegistryManager.Instance.RegisteredEmail
          },
          {
            "engine_mode",
            RegistryManager.Instance.DeviceCaps
          }
        };
        string campaignJson = RegistryManager.Instance.CampaignJson;
        if (!string.IsNullOrEmpty(campaignJson))
        {
          try
          {
            JObject jobject = JObject.Parse(campaignJson);
            dictionary.Add("campaign_name", jobject["campaign_name"].ToString());
          }
          catch
          {
            dictionary.Add("campaign_name", "");
          }
        }
        else
          dictionary.Add("campaign_name", "");
        if (!string.IsNullOrEmpty(RegistryManager.Instance.ClientLaunchParams))
        {
          JObject jobject = JObject.Parse(RegistryManager.Instance.ClientLaunchParams);
          if (jobject["campaign_id"] != null)
            dictionary.Add("externalsource_campaignid", jobject["campaign_id"].ToString());
          if (jobject["source_version"] != null)
            dictionary.Add("externalsource_version", jobject["source_version"].ToString());
        }
        return dictionary;
      }
    }

    internal static void SendClientStatsAsync(
      string op,
      string status,
      string uri,
      string package = "",
      string errorCode = "",
      string vmName = "")
    {
      ThreadPool.QueueUserWorkItem((WaitCallback) (obj =>
      {
        try
        {
          ClientStats.SendStatsSync(op, status, uri, package, errorCode, vmName);
        }
        catch (Exception ex)
        {
          Logger.Error("Failed to send stats for uri : " + uri + ". Reason : " + ex.ToString());
        }
      }));
    }

    internal static void SendFrontendClickStats(
      string eventType,
      string keyword,
      string app_loc,
      string app_pkg,
      string is_installed,
      string app_position,
      string app_rank,
      string apps_recommendation_obj)
    {
      Dictionary<string, string> getCommonData = ClientStats.GetCommonData;
      getCommonData.Add("event", eventType);
      getCommonData.Add(nameof (keyword), keyword);
      getCommonData.Add(nameof (app_loc), app_loc);
      getCommonData.Add(nameof (app_pkg), app_pkg);
      getCommonData.Add(nameof (is_installed), is_installed);
      getCommonData.Add(nameof (app_position), app_position);
      getCommonData.Add(nameof (app_rank), app_rank);
      getCommonData.Add(nameof (apps_recommendation_obj), apps_recommendation_obj);
      ClientStats.SendStatsAsync(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/bs3/stats/{1}", (object) RegistryManager.Instance.Host, (object) "frontend_click_stats"), getCommonData, (Dictionary<string, string>) null);
    }

    internal static void SendCalendarStats(
      string eventType,
      string calendarstartdate,
      string calendarenddate,
      string calendarlink,
      string success = "",
      string rowsaffected = "")
    {
      Dictionary<string, string> getCommonData = ClientStats.GetCommonData;
      getCommonData.Add("event_type", eventType);
      getCommonData.Add("calendar_start_date", calendarstartdate);
      getCommonData.Add("calendar_end_date", calendarenddate);
      getCommonData.Add("calendar_link", calendarlink);
      getCommonData.Add(nameof (success), success);
      getCommonData.Add("rows_affected", rowsaffected);
      ClientStats.SendStatsAsync(RegistryManager.Instance.Host + "/bs4/stats/calendar_stats", getCommonData, (Dictionary<string, string>) null);
    }

    internal static void SendStatsSync(
      string op,
      string status,
      string uri,
      string package,
      string errorCode = "",
      string vmname = "")
    {
      Dictionary<string, string> data = ClientStats.GetCommonData;
      data.Add(nameof (op), op);
      data.Add(nameof (status), status);
      string str = !(uri != "engine_activity") ? RegistryManager.Instance.Version : "4.250.0.1070";
      data.Add("version", str);
      if (uri == "emulator_activity")
      {
        Dictionary<string, string> resolutionData = BlueStacksUIUtils.GetResolutionData();
        try
        {
          resolutionData.ToList<KeyValuePair<string, string>>().ForEach((System.Action<KeyValuePair<string, string>>) (kvp => data[kvp.Key] = kvp.Value));
        }
        catch (Exception ex)
        {
          Logger.Error("Merge dictionary failed. Ex : " + ex.ToString());
        }
        try
        {
          BlueStacksUIUtils.GetEngineSettingsData(vmname).ToList<KeyValuePair<string, string>>().ForEach((System.Action<KeyValuePair<string, string>>) (kvp => data[kvp.Key] = kvp.Value));
        }
        catch (Exception ex)
        {
          Logger.Error("Merge dictionary failed. Ex : " + ex.ToString());
        }
      }
      if (!string.IsNullOrEmpty(errorCode))
        data.Add("error_code", errorCode);
      if (!string.IsNullOrEmpty(package))
        data.Add("app_pkg", package);
      ClientStats.SendStats(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/bs3/stats/{1}", string.IsNullOrEmpty(ClientStats.sDevUrl) ? (object) RegistryManager.Instance.Host : (object) ClientStats.sDevUrl, (object) uri), data, (Dictionary<string, string>) null, vmname);
    }

    internal static void SendGPlayClickStats(Dictionary<string, string> clientData)
    {
      ThreadPool.QueueUserWorkItem((WaitCallback) (obj =>
      {
        try
        {
          Dictionary<string, string> getCommonData = ClientStats.GetCommonData;
          if (clientData != null)
          {
            foreach (KeyValuePair<string, string> keyValuePair in clientData)
              getCommonData.Add(keyValuePair.Key, keyValuePair.Value);
          }
          ClientStats.SendStats(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/bs3/stats/gplay_click_stats", string.IsNullOrEmpty(ClientStats.sDevUrl) ? (object) RegistryManager.Instance.Host : (object) ClientStats.sDevUrl), getCommonData, (Dictionary<string, string>) null, "");
        }
        catch (Exception ex)
        {
          Logger.Error("Failed to send gplay stats... Err : " + ex.ToString());
        }
      }));
    }

    internal static void SendMiscellaneousStatsAsync(
      string tag,
      string arg1,
      string arg2,
      string arg3,
      string arg4,
      string arg5,
      string arg6 = null,
      string arg7 = null,
      string arg8 = null,
      string vmName = "Android")
    {
      ThreadPool.QueueUserWorkItem((WaitCallback) (obj =>
      {
        try
        {
          Logger.Info("Sending miscellaneous Stats for tag : " + tag);
          ClientStats.SendStats(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) RegistryManager.Instance.Host, (object) "/stats/miscellaneousstats"), new Dictionary<string, string>()
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
          }, (Dictionary<string, string>) null, vmName);
        }
        catch (Exception ex)
        {
          Logger.Error("Exception in sending miscellaneous stats async err : " + ex.ToString());
        }
      }));
    }

    internal static void SendKeyMappingUIStatsAsync(
      string eventtype,
      string packageName,
      string extraInfo = "")
    {
      ThreadPool.QueueUserWorkItem((WaitCallback) (obj =>
      {
        try
        {
          Logger.Info("Sending KeyMappingUI Stats");
          ClientStats.SendStats(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) RegistryManager.Instance.Host, (object) "/stats/keymappinguistats"), new Dictionary<string, string>()
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
              "oem",
              RegistryManager.Instance.Oem
            },
            {
              "app_pkg",
              packageName
            },
            {
              "event_type",
              eventtype
            },
            {
              "email",
              RegistryManager.Instance.RegisteredEmail
            },
            {
              "extra_info",
              extraInfo
            },
            {
              "locale",
              RegistryManager.Instance.UserSelectedLocale
            }
          }, (Dictionary<string, string>) null, "");
        }
        catch (Exception ex)
        {
          Logger.Error("Exception in sending miscellaneous stats async err : " + ex.ToString());
        }
      }));
    }

    internal static void SendLocalQuitPopupStatsAsync(string tag, string eventType)
    {
      Logger.Debug("Sending LocalQuitPopupStats for {0}", (object) eventType);
      string userGuid = RegistryManager.Instance.UserGuid;
      string clientVersion = RegistryManager.Instance.ClientVersion;
      string campaignMd5 = RegistryManager.Instance.CampaignMD5;
      ClientStats.SendMiscellaneousStatsAsync(tag, eventType, userGuid, clientVersion, campaignMd5, "", (string) null, (string) null, (string) null, "Android");
    }

    internal static void SendBluestacksUpdaterUIStatsAsync(string eventName, string comment = "")
    {
      ThreadPool.QueueUserWorkItem((WaitCallback) (obj =>
      {
        try
        {
          Logger.Info("Sending Bluestacks Updater UI Stats");
          Dictionary<string, string> data = new Dictionary<string, string>()
          {
            {
              "event",
              eventName
            },
            {
              "install_id",
              RegistryManager.Instance.InstallID
            },
            {
              "engine_version",
              RegistryManager.Instance.Version
            },
            {
              "client_version",
              RegistryManager.Instance.ClientVersion
            },
            {
              "os",
              Profile.OS
            }
          };
          string str = InstallerArchitectures.AMD64;
          if (!SystemUtils.IsOs64Bit())
            str = InstallerArchitectures.X86;
          data.Add("installer_arch", str);
          data.Add("guid", RegistryManager.Instance.UserGuid);
          data.Add("oem", Oem.Instance.OEM);
          data.Add("campaign_hash", RegistryManager.Instance.CampaignMD5);
          data.Add("campaign_name", RegistryManager.Instance.CampaignName);
          data.Add("locale", RegistryManager.Instance.UserSelectedLocale);
          data.Add(nameof (comment), comment);
          data.Add("installation_type", RegistryManager.Instance.InstallationType.ToString());
          data.Add("gaming_pkg_name", RegistryManager.Instance.InstallerPkgName);
          ClientStats.SendStats(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}", (object) RegistryManager.Instance.Host, (object) "/bs3/stats/unified_install_stats"), data, (Dictionary<string, string>) null, "");
        }
        catch (Exception ex)
        {
          Logger.Error("Exception in sending miscellaneous stats async err : " + ex.ToString());
        }
      }));
    }

    internal static void SendPopupBrowserStatsInMiscASync(string eventType, string url)
    {
      ClientStats.SendMiscellaneousStatsAsync("PopupBrowser", RegistryManager.Instance.UserGuid, eventType, url, RegistryManager.Instance.RegisteredEmail, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, (string) null, (string) null, "Android");
    }

    internal static void SendGeneralStats(string op, Dictionary<string, string> sourceData)
    {
      ThreadPool.QueueUserWorkItem((WaitCallback) (obj =>
      {
        try
        {
          Dictionary<string, string> getCommonData = ClientStats.GetCommonData;
          getCommonData.Add(nameof (op), op);
          if (sourceData != null)
          {
            foreach (KeyValuePair<string, string> keyValuePair in sourceData)
              getCommonData.Add(keyValuePair.Key, keyValuePair.Value);
          }
          getCommonData.Add("os_ver", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}", (object) Environment.OSVersion.Version.Major, (object) Environment.OSVersion.Version.Minor));
          ClientStats.SendStats(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/bs3/stats/general_json", string.IsNullOrEmpty(ClientStats.sDevUrl) ? (object) RegistryManager.Instance.Host : (object) ClientStats.sDevUrl), getCommonData, (Dictionary<string, string>) null, "");
        }
        catch (Exception ex)
        {
          Logger.Info("Failed to send general stat for op : " + op + "...Err : " + ex.ToString());
        }
      }));
    }

    internal static void SendStatsAsync(
      string url,
      Dictionary<string, string> data,
      Dictionary<string, string> headers = null)
    {
      ThreadPool.QueueUserWorkItem((WaitCallback) (obj =>
      {
        try
        {
          ClientStats.SendStats(url, data, headers, "");
        }
        catch (Exception ex)
        {
          Logger.Error("Failed to send stats for uri : " + url + ". Reason : " + ex.ToString());
        }
      }));
    }

    internal static void SendPromotionAppClickStatsAsync(
      Dictionary<string, string> appData,
      string uri)
    {
      ThreadPool.QueueUserWorkItem((WaitCallback) (obj =>
      {
        Dictionary<string, string> getCommonData = ClientStats.GetCommonData;
        foreach (KeyValuePair<string, string> keyValuePair in appData)
          getCommonData.Add(keyValuePair.Key, keyValuePair.Value);
        ClientStats.SendStats(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/bs3/stats/{1}", string.IsNullOrEmpty(ClientStats.sDevUrl) ? (object) RegistryManager.Instance.Host : (object) ClientStats.sDevUrl, (object) uri), getCommonData, (Dictionary<string, string>) null, "");
      }));
    }

    internal static void SendStats(
      string url,
      Dictionary<string, string> data,
      Dictionary<string, string> headers = null,
      string vmname = "")
    {
      try
      {
        BstHttpClient.Post(url, data, headers, false, vmname, 0, 1, 0, false, "bgp");
      }
      catch (Exception ex)
      {
        Logger.Info("Failed to send stats for : " + url + ". Reason : " + ex.ToString());
      }
    }
  }
}
