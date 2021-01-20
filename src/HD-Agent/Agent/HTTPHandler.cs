// Decompiled with JetBrains decompiler
// Type: BlueStacks.Agent.HTTPHandler
// Assembly: HD-Agent, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 06DAED18-1D79-40C2-83F8-3A28B5222574
// Assembly location: C:\Program Files\BlueStacks\HD-Agent.exe

using BlueStacks.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Input;

namespace BlueStacks.Agent
{
  public class HTTPHandler
  {
    private static LinkedList<AndroidNotification> s_PendingNotifications = new LinkedList<AndroidNotification>();
    private static System.Threading.Timer s_NotificationTimer = (System.Threading.Timer) null;
    private static int s_NotificationTimeout = 20000;
    private static bool[] s_NotificationLockHelper = new bool[2];
    private static int s_LockForTurn = 0;
    private static object s_NotificationLock = new object();
    public static Thread sApkInstallThread = (Thread) null;
    public static bool sAppUninstallationInProgress = false;
    public static string sApkInstallResult = "";
    private static string sCurrentAppPackageFromRunApp = "";
    internal static List<string> sXapkPackageToBeInstalledList = new List<string>();
    public static object s_sync = new object();
    private const int UAC_CANCEL_EVENT_CODE = 1223;
    private static Mutex sAppCrashInfoWriteLock;

    public static void ShowTileInterface(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        HTTPUtils.ParseRequest(req);
        KeyboardSend.KeyDown(Keys.LWin);
        KeyboardSend.KeyUp(Keys.LWin);
        HTTPUtils.WriteSuccessArrayJson(res, "");
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in ShowTileInterface: " + ex.Message);
        HTTPUtils.WriteErrorArrayJson(res, ex.Message);
      }
    }

    public static void LogAndroidClickEvent(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        Logger.Info("Data");
        foreach (string allKey in request.Data.AllKeys)
          Logger.Debug("Key: {0}, Value: {1}", (object) allKey, (object) request.Data[allKey]);
        if (!Oem.Instance.SendAppClickStatsFromClient)
        {
          string str = request.Data["package"];
          string homeVersion = request.Data["clickloc"];
          string appName = request.Data["appname"];
          string appVersion = request.Data["appver"];
          string appVersionName = request.Data["appVersionName"];
          string vmName = request.Data["source"];
          if (string.Compare(str, "com.bluestacks.home", true) != 0 && string.Compare(str, "com.bluestacks.setup", true) != 0 && (string.Compare(str, "mpi.v23", true) != 0 && string.Compare(str, "com.android.systemui", true) != 0) && (string.Compare(str, "com.bluestacks.s2p", true) != 0 && string.Compare(str, "com.bluestacks.gamepophome", true) != 0))
            Stats.SendAppStats(appName, str, appVersion, homeVersion, Stats.AppType.app, vmName, appVersionName);
        }
        HTTPUtils.WriteSuccessJson(res, "");
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in Server LogAndroidClickEvent" + ex?.ToString());
        HTTPUtils.WriteErrorArrayJson(res, ex.Message);
      }
    }

    public static void PostHttpUrl(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        bool flag = false;
        string str = string.Empty;
        RequestData request = HTTPUtils.ParseRequest(req);
        string requestVmName = request.RequestVmName;
        try
        {
          string empty = string.Empty;
          Dictionary<string, string> data = new Dictionary<string, string>();
          foreach (string allKey in request.Data.AllKeys)
          {
            Logger.Info(allKey + " = " + request.Data[allKey]);
            if (allKey.Equals("url", StringComparison.InvariantCultureIgnoreCase))
              empty = request.Data[allKey];
            else
              data.Add(allKey, request.Data[allKey]);
          }
          str = BstHttpClient.Post(empty, data, (Dictionary<string, string>) null, false, requestVmName, 0, 1, 0, false, "bgp");
          flag = true;
        }
        catch (Exception ex)
        {
          Logger.Error("An error occured while fetching info from cloud...Err : " + ex.ToString());
        }
        HTTPUtils.Write(new JArray()
        {
          (JToken) new JObject()
          {
            {
              "response",
              (JToken) str
            },
            {
              "success",
              (JToken) flag
            }
          }
        }.ToString(Formatting.None), res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in PostHtpUrl: " + ex.ToString());
        HTTPUtils.WriteErrorArrayJson(res, ex.Message);
      }
    }

    public static void GrmAppLaunch(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        HTTPUtils.SendRequestToClient("showGrmAndLaunchApp", new Dictionary<string, string>()
        {
          {
            "package",
            request.Data["result"]
          }
        }, request.RequestVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in GrmAppLaunch. Err : " + ex.ToString());
      }
    }

    public static void ReInitLocalization(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        LocaleStrings.InitLocalization((string) null, "Android", false);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in ReInitLocalization. Err : " + ex.ToString());
      }
    }

    public static void ClearAppDataHandler(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        bool flag = false;
        RequestData request = HTTPUtils.ParseRequest(req);
        string requestVmName = request.RequestVmName;
        foreach (string allKey in request.Data.AllKeys)
        {
          if (string.Compare(allKey, "package", true) == 0)
          {
            string package = request.Data[allKey];
            string failReason;
            if (!Utils.IsAppInstalled(package, requestVmName, out string _, out failReason, true))
            {
              HTTPUtils.WriteErrorArrayJson(res, failReason);
              return;
            }
            flag = true;
            if (string.Compare(VmCmdHandler.RunCommand(string.Format("clearappdata {0}", (object) request.Data[allKey]), requestVmName, "bgp"), "ok", true) != 0)
            {
              HTTPUtils.WriteErrorArrayJson(res, string.Format("Unable to clear the app data for {0}", (object) package));
              return;
            }
          }
        }
        if (flag)
          HTTPUtils.WriteSuccessArrayJson(res, "");
        else
          HTTPUtils.WriteErrorArrayJson(res, "");
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in ClearAppDataInterface: " + ex.Message);
        HTTPUtils.WriteErrorArrayJson(res, ex.Message);
      }
    }

    public static void SettingsImageMouseUp(object sender, EventArgs e)
    {
      if (sender is Popup)
        Stats.SendCommonClientStatsAsync("notification_mode", "alert_clicked", "Android", "", "", "", "");
      Stats.SendCommonClientStatsAsync("notification_mode", "alert_clicked", "Android", string.Empty, "gear_icon", "", "");
      HTTPHandler.OpenNotificationsSettingsInGM("");
    }

    public static void StopAppHandler(HttpListenerRequest req, HttpListenerResponse res)
    {
      RequestData request = HTTPUtils.ParseRequest(req);
      string requestVmName = request.RequestVmName;
      try
      {
        if (!Utils.IsUIProcessAlive(requestVmName, "bgp"))
        {
          Logger.Warning("Frontend not running");
          HTTPUtils.WriteErrorArrayJson(res, "Frontend not running");
        }
        else
        {
          bool flag = false;
          foreach (string allKey in request.Data.AllKeys)
          {
            if (string.Compare(allKey, "pkg", true) == 0)
            {
              string package = request.Data[allKey];
              string failReason;
              if (!Utils.IsAppInstalled(package, requestVmName, out string _, out failReason, true))
              {
                HTTPUtils.WriteErrorArrayJson(res, failReason);
                return;
              }
              flag = true;
              if (string.Compare(VmCmdHandler.RunCommand(string.Format("StopApp {0}", (object) request.Data[allKey]), requestVmName, "bgp"), "ok", true) == 0)
              {
                Dictionary<string, string> data = new Dictionary<string, string>()
                {
                  {
                    "appPackage",
                    package
                  }
                };
                if (Utils.WaitForFrontendPingResponse(requestVmName))
                {
                  HTTPUtils.SendRequestToEngine("stopAppInfo", data, requestVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "", "bgp");
                }
                else
                {
                  Logger.Error("Frontend not responding to ping response");
                  HTTPUtils.WriteErrorArrayJson(res, "Frontend Server not running");
                  return;
                }
              }
              else
              {
                HTTPUtils.WriteErrorArrayJson(res, string.Format("Unable to stop the app: {0}", (object) package));
                return;
              }
            }
          }
          HTTPHandler.sCurrentAppPackageFromRunApp = "";
          Logger.Info("Assigning value empty to sCurrentAppPackageFromRunApp");
          if (flag)
            HTTPUtils.WriteSuccessArrayJson(res, "");
          else
            HTTPUtils.WriteErrorArrayJson(res, "");
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in StopAppInterface: " + ex.ToString());
        HTTPUtils.WriteErrorArrayJson(res, ex.Message);
      }
    }

    internal static void IsEngineReady(HttpListenerRequest req, HttpListenerResponse res)
    {
      Logger.Info("HTTPHandler: Got IsEngineReady {0} request from {1}", (object) req.HttpMethod, (object) req.RemoteEndPoint.ToString());
      try
      {
        string guest = HTTPUtils.SendRequestToGuest("checkIfGuestReady", (Dictionary<string, string>) null, HTTPUtils.ParseRequest(req).RequestVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
        Logger.Info("response for isengineready : " + guest);
        if (JObject.Parse(guest)["result"].ToString().Trim().Equals("ok", StringComparison.InvariantCultureIgnoreCase))
          HTTPUtils.WriteSuccessJson(res, "");
        else
          HTTPUtils.WriteErrorJson(res, "engine not ready");
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in IsEngineReady: " + ex.ToString());
        HTTPUtils.WriteErrorJson(res, ex.Message);
      }
    }

    internal static void LaunchDefaultWebApp(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        string requestVmName = request.RequestVmName;
        Dictionary<string, string> data = new Dictionary<string, string>()
        {
          {
            "action",
            request.Data["action"]
          }
        };
        switch (request.Data["action"])
        {
          case "browser":
            data.Add("url", request.Data["url"]);
            break;
          case "email":
            try
            {
              data.Add("to", request.Data["to"]);
              data.Add("cc", request.Data["cc"]);
              data.Add("bcc", request.Data["bcc"]);
              data.Add("message", request.Data["message"]);
              data.Add("subject", request.Data["subject"]);
              data.Add("mailTo", request.Data["mailto"]);
              break;
            }
            catch
            {
              break;
            }
          default:
            HTTPUtils.WriteErrorArrayJson(res, "Wrong or empty action");
            break;
        }
        string client = HTTPUtils.SendRequestToClient("launchDefaultWebApp", data, requestVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
        if (client.Equals("success"))
          HTTPUtils.WriteSuccessArrayJson(res, "");
        else
          HTTPUtils.WriteErrorArrayJson(res, client);
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to LaunchDefaultWebApp app. Err: " + ex.Message);
        HTTPUtils.WriteErrorArrayJson(res, ex.Message);
      }
    }

    internal static void LaunchPlay(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        if (!((IEnumerable<string>) request.Data.AllKeys).Contains<string>("package"))
          HTTPUtils.WriteErrorArrayJson(res, "Package name cannot be empty");
        else if (!((IEnumerable<string>) request.Data.AllKeys).Contains<string>("vmname"))
        {
          HTTPUtils.WriteErrorArrayJson(res, "Invalid vmname");
        }
        else
        {
          string vmName = request.Data["vmname"];
          string str = request.Data["package"];
          if (string.IsNullOrEmpty(vmName) || !((IEnumerable<string>) RegistryManager.Instance.VmList).Contains<string>(vmName))
          {
            HTTPUtils.WriteErrorArrayJson(res, "Invalid vmname");
          }
          else
          {
            Dictionary<string, string> data = new Dictionary<string, string>()
            {
              {
                "vmname",
                vmName
              }
            };
            if (!ProcessUtils.IsAlreadyRunning("Global\\BlueStacks_BlueStacksUI_Lockbgp"))
            {
              ProcessUtils.GetProcessObject(RegistryManager.Instance.PartnerExePath, "-vmname " + vmName, false).Start();
              if (!Utils.WaitForBGPClientPing(40))
              {
                HTTPUtils.WriteErrorArrayJson(res, "Client not responding");
                return;
              }
            }
            else
              HTTPUtils.SendRequestToClient("showWindow", data, "Android", 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
            data.Add("package", str);
            HTTPUtils.SendRequestToClientAsync("launchPlay", data, vmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
            HTTPUtils.WriteSuccessArrayJson(res, "");
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Error in launchplay. Ex : " + ex.ToString());
        HTTPUtils.WriteErrorArrayJson(res, ex.Message);
      }
    }

    internal static void ConfigFileChanged(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        HTTPUtils.SendRequestToClientAsync("configFileChanged", new Dictionary<string, string>()
        {
          {
            "config",
            request.Data["result"]
          }
        }, request.RequestVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in ConfigFileChanged. Err : " + ex.ToString());
      }
    }

    internal static void NotificationStatsOnClosing(
      HttpListenerRequest req,
      HttpListenerResponse res)
    {
      try
      {
        string requestVmName = HTTPUtils.ParseRequest(req).RequestVmName;
        if (NotificationWindow.Instance.AppNotificationCountDictForEachVM.Keys.Contains<string>(requestVmName))
        {
          foreach (string key in NotificationWindow.Instance.AppNotificationCountDictForEachVM[requestVmName].Keys)
            Stats.SendCommonClientStatsAsync("minimize_bluestacks_notification", "Focus_notification_number", requestVmName, key, string.Empty, NotificationWindow.Instance.AppNotificationCountDictForEachVM[requestVmName][key].ToString(), "");
        }
        NotificationWindow.Instance.AppNotificationCountDictForEachVM.Clear();
      }
      catch (Exception ex)
      {
        Logger.Error("Error in Parsing value for notifications when player closed. err:" + ex.ToString());
      }
    }

    internal static void OverrideDesktopNotificationSettings(
      HttpListenerRequest req,
      HttpListenerResponse res)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        string requestVmName = request.RequestVmName;
        if (!((IEnumerable<string>) request.Data.AllKeys).Contains<string>("override"))
          return;
        bool result;
        if (!bool.TryParse(request.Data["override"].ToString(), out result))
        {
          HTTPUtils.WriteErrorArrayJson(res, "Error in overriding notification settings");
        }
        else
        {
          if (NotificationWindow.Instance.IsOverrideDesktopNotificationSettingsDict.ContainsKey(requestVmName))
            NotificationWindow.Instance.IsOverrideDesktopNotificationSettingsDict[requestVmName] = result;
          else
            NotificationWindow.Instance.IsOverrideDesktopNotificationSettingsDict.Add(requestVmName, result);
          Logger.Info("Override value for vm{0} changed to ={1}", (object) requestVmName, (object) NotificationWindow.Instance.IsOverrideDesktopNotificationSettingsDict[requestVmName]);
          if (NotificationWindow.Instance.IsOverrideDesktopNotificationSettingsDict[requestVmName])
          {
            if (NotificationWindow.Instance.AppNotificationCountDictForEachVM.Keys.Contains<string>(requestVmName))
            {
              foreach (string key in NotificationWindow.Instance.AppNotificationCountDictForEachVM[requestVmName].Keys)
                Stats.SendCommonClientStatsAsync("minimize_bluestacks_notification", "Focus_notification_number", requestVmName, key, string.Empty, NotificationWindow.Instance.AppNotificationCountDictForEachVM[requestVmName][key].ToString(), "");
            }
          }
          else if (NotificationWindow.Instance.AppNotificationCountDictForEachVM.Keys.Contains<string>(requestVmName))
          {
            foreach (string key in NotificationWindow.Instance.AppNotificationCountDictForEachVM[requestVmName].Keys)
              Stats.SendCommonClientStatsAsync("minimize_bluestacks_notification", "Minimize_popup_notification_number", requestVmName, key, string.Empty, NotificationWindow.Instance.AppNotificationCountDictForEachVM[requestVmName][key].ToString(), "");
          }
          if (result)
          {
            Stats.SendCommonClientStatsAsync("notification_mode", "alert_shown", requestVmName, "", "", "", "");
            NotificationWindow.Instance.AddAlert((string) null, "BlueStacks", LocaleStrings.GetLocalizedString("STRING_NOTIFICATION_MODE_TOAST", ""), true, 5000, new MouseButtonEventHandler(HTTPHandler.SettingsImageMouseUp), true, requestVmName, false, string.Empty, true, "0", true);
          }
          NotificationWindow.Instance.AppNotificationCountDictForEachVM.Clear();
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Error in Parsing AutoHide value for notifications. err:" + ex.ToString());
      }
    }

    internal static void SendEngineTimelineStats(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        TimelineStatsSender.HandleEngineBootEvent(HTTPUtils.ParseRequest(req).Data["event"]);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in SendEngineTimelineStats. Ex : " + ex.ToString());
      }
    }

    internal static void SetShortcut(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        if (!((IEnumerable<string>) request.Data.AllKeys).Contains<string>("json"))
        {
          HTTPUtils.WriteErrorArrayJson(res, "Shortcut json cannot be empty");
        }
        else
        {
          Logger.Info("Data:");
          foreach (string allKey in request.Data.AllKeys)
            Logger.Info("Key: " + allKey + ", Value: " + request.Data[allKey]);
          string str = request.Data["json"];
          try
          {
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
              Formatting = Formatting.None
            };
            HDAgent.sShortcutConfig = JsonConvert.DeserializeObject<ShortcutConfig>(str, settings);
          }
          catch (Exception ex)
          {
            Logger.Error("Failed to parse shortcut json. Err : " + ex.ToString());
            HTTPUtils.WriteErrorArrayJson(res, "Invalid shortcut json");
            return;
          }
          HDAgent.sShortcutConfig.SaveUserDefinedShortcuts();
          foreach (string vm in RegistryManager.Instance.VmList)
          {
            if (ProcessUtils.IsLockInUse(BlueStacks.Common.Strings.GetClientInstanceLockName(vm, "bgp")))
            {
              HTTPUtils.SendRequestToClientAsync("reloadShortcuts", (Dictionary<string, string>) null, vm, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
              break;
            }
          }
          Stats.SendMiscellaneousStatsAsync("KeyboardShortcuts", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "shortcut_save", (string) null, (string) null, (string) null, (string) null, (string) null, "Android", 0);
          HTTPUtils.WriteSuccessArrayJson(res, "");
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Error in SetShortcutKey. Ex : " + ex.ToString());
        HTTPUtils.WriteErrorArrayJson(res, ex.Message);
      }
    }

    internal static void GetShortcut(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        HTTPUtils.ParseRequest(req);
        string str = "";
        string json;
        if (!string.IsNullOrEmpty(RegistryManager.Instance.UserDefinedShortcuts))
          json = RegistryManager.Instance.UserDefinedShortcuts;
        else if (string.IsNullOrEmpty(str) && !string.IsNullOrEmpty(RegistryManager.Instance.DefaultShortcuts))
        {
          json = RegistryManager.Instance.DefaultShortcuts;
        }
        else
        {
          HTTPUtils.WriteErrorArrayJson(res, "Shortcut json does not exist.");
          return;
        }
        JObject jobject = JObject.Parse(json);
        Logger.Debug("shortcuts : " + jobject.ToString(Formatting.None));
        Dictionary<string, object> data = new Dictionary<string, object>()
        {
          {
            "success",
            (object) true
          },
          {
            "keys",
            (object) jobject.ToString(Formatting.None)
          }
        };
        HTTPUtils.WriteArrayJson(res, data);
      }
      catch (Exception ex)
      {
        Logger.Error("Error in GetShortcutKey. Ex : " + ex.ToString());
        HTTPUtils.WriteErrorArrayJson(res, ex.Message);
      }
    }

    internal static void MakeDir(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        string requestVmName = request.RequestVmName;
        if (string.IsNullOrEmpty(requestVmName) || !((IEnumerable<string>) RegistryManager.Instance.VmList).Contains<string>(requestVmName))
          HTTPUtils.WriteErrorArrayJson(res, "Invalid vmname");
        else if (!((IEnumerable<string>) request.Data.AllKeys).Contains<string>("package") || string.IsNullOrEmpty(request.Data["package"]))
          HTTPUtils.WriteErrorArrayJson(res, "Package name cannot be empty");
        else if (!((IEnumerable<string>) request.Data.AllKeys).Contains<string>("subdir") || string.IsNullOrEmpty(request.Data["subdir"]))
          HTTPUtils.WriteErrorArrayJson(res, "Subdir cannot be empty");
        else if (Utils.CheckIfGuestReady(requestVmName, 1))
        {
          string str1 = request.Data["package"];
          string str2 = request.Data["subdir"];
          string str3 = string.Format("{0} && busybox chmod -R 777 '/data/data/{1}/{2}'", (object) string.Format("mkdir -p '/data/data/{0}/{1}'", (object) str1, (object) str2), (object) str1, (object) str2).Replace("&", "%26");
          StringBuilder sb = new StringBuilder();
          using (JsonWriter jsonWriter = (JsonWriter) new JsonTextWriter((TextWriter) new StringWriter(sb)))
          {
            jsonWriter.WriteStartObject();
            jsonWriter.WritePropertyName("commands");
            jsonWriter.WriteValue(str3);
            jsonWriter.WriteEndObject();
          }
          string reason = BstHttpClient.Get(string.Format("{0}/exec?d={1}", (object) HTTPUtils.GuestServerUrl(requestVmName, "bgp"), (object) sb.ToString()), (Dictionary<string, string>) null, false, requestVmName, -1, 1, 0, false, "bgp");
          if (reason.Contains("ok"))
            HTTPUtils.WriteSuccessArrayJson(res, reason);
          else
            HTTPUtils.WriteErrorArrayJson(res, reason);
        }
        else
          HTTPUtils.WriteErrorArrayJson(res, "Guest not ready");
      }
      catch (Exception ex)
      {
        Logger.Error("Error in make dir. Ex : " + ex.ToString());
        HTTPUtils.WriteErrorArrayJson(res, ex.Message);
      }
    }

    internal static void GetHeightWidth(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        string requestVmName = HTTPUtils.ParseRequest(req).RequestVmName;
        if (string.IsNullOrEmpty(requestVmName) || !((IEnumerable<string>) RegistryManager.Instance.VmList).Contains<string>(requestVmName))
          HTTPUtils.WriteErrorArrayJson(res, "Invalid vmname");
        else if (ProcessUtils.IsLockInUse(BlueStacks.Common.Strings.GetClientInstanceLockName(requestVmName, "bgp")))
        {
          string client = HTTPUtils.SendRequestToClient("getHeightWidth", (Dictionary<string, string>) null, requestVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
          if (((JObject) JArray.Parse(client).First<JToken>())["success"].ToObject<bool>())
            HTTPUtils.Write(client, res);
          else
            HTTPUtils.WriteErrorArrayJson(res, "Client Instance not running");
        }
        else
          HTTPUtils.WriteErrorArrayJson(res, "Client Instance not running");
      }
      catch (Exception ex)
      {
        Logger.Error("Error in getting height and width err:" + ex.ToString());
        HTTPUtils.WriteErrorArrayJson(res, ex.Message);
      }
    }

    internal static void ScreenLock(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        string requestVmName = request.RequestVmName;
        if (string.IsNullOrEmpty(requestVmName) || !((IEnumerable<string>) RegistryManager.Instance.VmList).Contains<string>(requestVmName))
          HTTPUtils.WriteErrorArrayJson(res, "Invalid vmname");
        else if (ProcessUtils.IsLockInUse(BlueStacks.Common.Strings.GetClientInstanceLockName(requestVmName, "bgp")))
        {
          HTTPUtils.SendRequestToClient("screenLock", new Dictionary<string, string>()
          {
            {
              "lock",
              request.Data["lock"]
            }
          }, requestVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
          HTTPUtils.WriteSuccessArrayJson(res, "");
        }
        else
          HTTPUtils.WriteErrorArrayJson(res, "Client Instance not running");
      }
      catch (Exception ex)
      {
        Logger.Error("Error in screen lock. Ex : " + ex.ToString());
        HTTPUtils.WriteErrorArrayJson(res, ex.Message);
      }
    }

    internal static void SetStreamingStatus(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        string requestVmName = request.RequestVmName;
        if (string.IsNullOrEmpty(requestVmName) || !((IEnumerable<string>) RegistryManager.Instance.VmList).Contains<string>(requestVmName))
          HTTPUtils.WriteErrorArrayJson(res, "Invalid vmname");
        else if (ProcessUtils.IsLockInUse(BlueStacks.Common.Strings.GetClientInstanceLockName(requestVmName, "bgp")))
        {
          HTTPUtils.SendRequestToClient("setStreamingStatus", new Dictionary<string, string>()
          {
            {
              "status",
              request.Data["status"]
            }
          }, requestVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
          HTTPUtils.WriteSuccessArrayJson(res, "");
        }
        else
          HTTPUtils.WriteErrorArrayJson(res, "Client Instance not running");
      }
      catch (Exception ex)
      {
        Logger.Error("Error in SetStreamingStatus. Ex : " + ex.ToString());
        HTTPUtils.WriteErrorArrayJson(res, ex.Message);
      }
    }

    internal static void SetDeviceProfile(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        string requestVmName = request.RequestVmName;
        if (string.IsNullOrEmpty(requestVmName) || !((IEnumerable<string>) RegistryManager.Instance.VmList).Contains<string>(requestVmName))
          HTTPUtils.WriteErrorArrayJson(res, "Invalid vmname");
        else if (Utils.CheckIfGuestReady(requestVmName, 1))
        {
          if (!((IEnumerable<string>) request.Data.AllKeys).Contains<string>("model"))
            HTTPUtils.WriteErrorArrayJson(res, "Model cannot be empty");
          else if (!((IEnumerable<string>) request.Data.AllKeys).Contains<string>("brand"))
            HTTPUtils.WriteErrorArrayJson(res, "Brand cannot be empty");
          else if (!((IEnumerable<string>) request.Data.AllKeys).Contains<string>("manufacturer"))
          {
            HTTPUtils.WriteErrorArrayJson(res, "Manufacturer cannot be empty");
          }
          else
          {
            JObject response;
            if (VmCmdHandler.SendRequest("currentdeviceprofile", (Dictionary<string, string>) null, requestVmName, out response, "bgp").Equals("ok", StringComparison.InvariantCultureIgnoreCase))
              response.Remove("result");
            if (response == null || !response.ContainsKey("pcode"))
            {
              HTTPUtils.WriteErrorArrayJson(res, "Cannot get old device profile from Android");
            }
            else
            {
              JObject mChangedDeviceProfile = new JObject()
              {
                {
                  "pcode",
                  (JToken) "custom"
                },
                {
                  "model",
                  (JToken) request.Data["model"]
                },
                {
                  "brand",
                  (JToken) request.Data["brand"]
                },
                {
                  "manufacturer",
                  (JToken) request.Data["manufacturer"]
                },
                {
                  "caSelector",
                  (JToken) response["caSelector"].ToString()
                }
              };
              if (Utils.CheckIfDeviceProfileChanged(response, mChangedDeviceProfile))
              {
                mChangedDeviceProfile.Remove("pcode");
                mChangedDeviceProfile.Add("createcustomprofile", (JToken) "true");
                string guest = HTTPUtils.SendRequestToGuest("changeDeviceProfile?d=" + mChangedDeviceProfile.ToString(Formatting.None), (Dictionary<string, string>) null, requestVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
                Logger.Info("Result for device profile change command: " + guest);
                mChangedDeviceProfile.Remove("createcustomprofile");
                mChangedDeviceProfile.Add("pcode", (JToken) "custom");
                if (JObject.Parse(guest)["result"].ToString().Equals("ok", StringComparison.InvariantCultureIgnoreCase))
                {
                  Utils.UpdateValueInBootParams("pcode", "custom", requestVmName, false, "bgp");
                  Utils.UpdateValueInBootParams("caSelector", mChangedDeviceProfile["caSelector"].ToString(), requestVmName, false, "bgp");
                  Stats.SendMiscellaneousStatsAsync("DeviceProfileChangeStats", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "success", JsonConvert.SerializeObject((object) mChangedDeviceProfile), JsonConvert.SerializeObject((object) response), RegistryManager.Instance.Version, "DeviceProfileSetting", (string) null, "Android", 0);
                  HTTPUtils.WriteSuccessArrayJson(res, "");
                }
                else
                {
                  Stats.SendMiscellaneousStatsAsync("DeviceProfileChangeStats", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "failed", JsonConvert.SerializeObject((object) mChangedDeviceProfile), JsonConvert.SerializeObject((object) response), RegistryManager.Instance.Version, "DeviceProfileSetting", (string) null, "Android", 0);
                  HTTPUtils.WriteErrorArrayJson(res, "Invalid parameters in device profile");
                }
              }
              else
                HTTPUtils.WriteErrorArrayJson(res, "Device profile already in use");
            }
          }
        }
        else
          HTTPUtils.WriteErrorArrayJson(res, "Guest not ready");
      }
      catch (Exception ex)
      {
        Logger.Error("Error in settting device profile. Ex : " + ex.ToString());
        HTTPUtils.WriteErrorArrayJson(res, ex.Message);
      }
    }

    internal static void RemoveAccount(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        string requestVmName = HTTPUtils.ParseRequest(req).RequestVmName;
        if (string.IsNullOrEmpty(requestVmName) || !((IEnumerable<string>) RegistryManager.Instance.VmList).Contains<string>(requestVmName))
          HTTPUtils.WriteErrorArrayJson(res, "Invalid vmname");
        else if (Utils.CheckIfGuestReady(requestVmName, 1))
        {
          Logger.Info("Account removed response: " + HTTPUtils.SendRequestToGuest("removeAccountsInfo", (Dictionary<string, string>) null, requestVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp"));
          HTTPUtils.WriteSuccessArrayJson(res, "");
        }
        else
          HTTPUtils.WriteErrorArrayJson(res, "Guest not ready");
      }
      catch (Exception ex)
      {
        Logger.Error("Error in remove account. Ex : " + ex.ToString());
        HTTPUtils.WriteErrorArrayJson(res, ex.Message);
      }
    }

    internal static void SetNCPlayerCharacterName(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        string requestVmName = request.RequestVmName;
        Logger.Info("Data:");
        foreach (string allKey in request.Data.AllKeys)
          Logger.Info("Key: " + allKey + ", Value: " + request.Data[allKey]);
        if (!((IEnumerable<string>) RegistryManager.Instance.VmList).Contains<string>(requestVmName))
        {
          HTTPUtils.WriteErrorArrayJson(res, "Invalid vmname");
        }
        else
        {
          Dictionary<string, string> data = new Dictionary<string, string>()
          {
            {
              "game",
              request.Data["game"]
            },
            {
              "character",
              request.Data["character"]
            }
          };
          if (ProcessUtils.IsAlreadyRunning("Global\\BlueStacks_BlueStacksUI_Lockbgp"))
          {
            JToken jtoken = JToken.Parse(HTTPUtils.SendRequestToClient("ncSetGameInfoOnTopBar", data, requestVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp"));
            JObject jobject = !(jtoken is JArray) ? jtoken as JObject : JObject.Parse((jtoken as JArray)[0].ToString());
            if (jobject["success"].ToObject<bool>())
              HTTPUtils.WriteSuccessArrayJson(res, "");
            else
              HTTPUtils.WriteErrorArrayJson(res, jobject["reason"].ToString());
          }
          else
            HTTPUtils.WriteErrorArrayJson(res, "Client process is not running");
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to set character name. Ex : " + ex?.ToString());
        HTTPUtils.WriteErrorArrayJson(res, ex.Message);
      }
    }

    internal static void MaintenanceWarning(HttpListenerRequest req, HttpListenerResponse res)
    {
      Logger.Info("HTTPHandler: Got MaintenanceWarning {0} request from {1}", (object) req.HttpMethod, (object) req.RemoteEndPoint.ToString());
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        string requestVmName = request.RequestVmName;
        string str = request.Data["message"];
        HTTPHandler.WriteJSON(new Dictionary<string, string>()
        {
          {
            "result_code",
            "0"
          }
        }, res);
        HTTPUtils.SendRequestToClient("maintenanceWarning", new Dictionary<string, string>()
        {
          {
            "message",
            str
          }
        }, requestVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to MaintenanceWarning app... Err : " + ex.ToString());
      }
    }

    internal static void GuestBootFailedHandler(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        string requestVmName = HTTPUtils.ParseRequest(req).RequestVmName;
        if (HDAgent.sBootFailed.Contains(requestVmName))
          return;
        HDAgent.sBootFailed.Add(requestVmName);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception to GuestBootFailedHandler. Err: " + ex.Message);
      }
    }

    internal static void GetDownloadInstallProgress(
      HttpListenerRequest req,
      HttpListenerResponse res)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        string requestVmName = request.RequestVmName;
        string str = request.Data["app_package_name"];
        Logger.Info("package name: " + str);
        Dictionary<string, string> data = new Dictionary<string, string>()
        {
          {
            "app_package_name",
            str
          }
        };
        bool flag1 = Utils.IsAllUIProcessAlive(requestVmName);
        bool flag2 = true;
        if (flag1)
          flag2 = Utils.CheckIfGuestReady(requestVmName, 0);
        if (HDAgent.sBootFailed.Contains(requestVmName))
        {
          data.Add("status", Convert.ToString(1));
          uint num = ReturnCodesUInt.INSTALL_FAILED_HOST_BASE_VALUE + 10U;
          data.Add("result_code", num.ToString());
        }
        else if (!flag1 || !flag2)
        {
          data.Add("status", Convert.ToString(1));
          data.Add("result_code", Convert.ToString(0));
        }
        else if (HDAgent.sApkDownloadInstallStatusList.ContainsKey(requestVmName + str))
        {
          ApkDownloadInstallStatus downloadInstallStatus = HDAgent.sApkDownloadInstallStatusList[requestVmName + str];
          Logger.Info("ApkStatus received " + downloadInstallStatus.status.ToString() + " for package " + str);
          switch (downloadInstallStatus.status)
          {
            case DownloadInstallStatus.Downloading:
              data.Add("status", Convert.ToString(2));
              data.Add("downloaded_bytes", downloadInstallStatus.downloadedSize.ToString());
              data.Add("result_code", Convert.ToString(0));
              break;
            case DownloadInstallStatus.DownloadFailed:
              data.Add("status", Convert.ToString(2));
              data.Add("result_code", downloadInstallStatus.downloadFailedCode.ToString());
              break;
            case DownloadInstallStatus.InstallFailed:
              if (downloadInstallStatus.isUpgrade)
                data.Add("status", Convert.ToString(4));
              else
                data.Add("status", Convert.ToString(3));
              data.Add("result_code", downloadInstallStatus.installFailedCode.ToString());
              break;
            default:
              data.Add("status", Convert.ToString((int) downloadInstallStatus.status));
              data.Add("result_code", Convert.ToString(0));
              Stats.SendMiscellaneousStatsAsyncForDMM(Stats.DMMEvent.get_progress_success.ToString(), str, (string) null, (string) null, (string) null, "Android", 0);
              break;
          }
        }
        else if (HDAgent.sApkUpgradingList.ContainsKey(requestVmName + str) && !HDAgent.sApkUpgradingList[requestVmName + str])
        {
          data.Add("status", Convert.ToString(5));
          data.Add("result_code", Convert.ToString(0));
          Stats.SendMiscellaneousStatsAsyncForDMM(Stats.DMMEvent.get_progress_success.ToString(), str, (string) null, (string) null, (string) null, "Android", 0);
        }
        else
        {
          data.Add("status", Convert.ToString(1));
          data.Add("result_code", Convert.ToString(0));
        }
        HTTPHandler.WriteJSON(data, res);
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to GetDownloadInstallProgress app. Err: {0}", (object) ex);
        HTTPHandler.WriteJSON(new Dictionary<string, string>()
        {
          {
            "result_code",
            "-1"
          },
          {
            "reason",
            ex.Message
          }
        }, res);
      }
    }

    internal static void SortWindows(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        string requestVmName = request.RequestVmName;
        int int32 = Convert.ToInt32(request.Data["layout"]);
        switch (int32)
        {
          case 0:
          case 1:
            RegistryManager.Instance.ArrangeWindowMode = int32;
            string client;
            if (int32 == 0)
            {
              RegistryManager.Instance.TileWindowColumnCount = (long) Convert.ToInt32(request.Data["columns"]);
              client = HTTPUtils.SendRequestToClient("tileWindow", new Dictionary<string, string>()
              {
                {
                  "columns",
                  RegistryManager.Instance.TileWindowColumnCount.ToString()
                }
              }, "Android", 0, (Dictionary<string, string>) null, true, 1, 0, "bgp");
            }
            else
              client = HTTPUtils.SendRequestToClient("cascadeWindow", (Dictionary<string, string>) null, "Android", 0, (Dictionary<string, string>) null, true, 1, 0, "bgp");
            JArray jarray = JArray.Parse(client);
            if (Convert.ToBoolean((object) jarray[0][(object) "success"]))
            {
              HTTPUtils.WriteSuccessArrayJson(res, "");
              break;
            }
            HTTPUtils.WriteErrorArrayJson(res, jarray[0][(object) "reason"].ToString());
            break;
          default:
            HTTPUtils.WriteErrorArrayJson(res, "incorrect layout style");
            break;
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to sort windows. Err : " + ex.ToString());
        HTTPUtils.WriteErrorArrayJson(res, ex.Message);
      }
    }

    internal static void SetVMDisplayName(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        string requestVmName = request.RequestVmName;
        if (!((IEnumerable<string>) RegistryManager.Instance.VmList).Contains<string>(requestVmName))
        {
          HTTPUtils.WriteErrorArrayJson(res, "vmname does not exist");
        }
        else
        {
          string controlCharFreeString = StringUtils.GetControlCharFreeString(request.Data["displayname"]);
          if (string.IsNullOrEmpty(controlCharFreeString))
          {
            HTTPUtils.WriteErrorArrayJson(res, "invalid display name");
          }
          else
          {
            RegistryManager.Instance.Guest[requestVmName].DisplayName = controlCharFreeString;
            HTTPUtils.WriteSuccessArrayJson(res, "");
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to set display name. Err : " + ex.ToString());
        HTTPUtils.WriteErrorArrayJson(res, ex.Message);
      }
    }

    internal static void DownloadInstalledAppsCfg(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        string requestVmName = HTTPUtils.ParseRequest(req).RequestVmName;
        string[] vmList = RegistryManager.Instance.VmList;
        List<string> stringList = new List<string>();
        foreach (string vmName in vmList)
        {
          foreach (AppInfo app in new JsonParser(vmName).GetAppList())
          {
            if (!stringList.Contains(app.Package))
              stringList.Add(app.Package);
          }
        }
        foreach (string packageName in stringList)
        {
          Logger.Info("Sending req to download cfg for {0}", (object) packageName);
          Utils.SendKeymappingFiledownloadRequest(packageName, requestVmName);
        }
      }
      catch (Exception ex)
      {
        Logger.Error("An error occured while getting cfs. Ex: {0}", (object) ex);
      }
    }

    internal static void CopyToAndroid(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        string requestVmName = request.RequestVmName;
        string[] allKeys = request.Data.AllKeys;
        if (!((IEnumerable<string>) allKeys).Contains<string>("from") || !((IEnumerable<string>) allKeys).Contains<string>("to"))
        {
          HTTPUtils.WriteErrorArrayJson(res, "Source or desitnation path does not exist");
        }
        else
        {
          string path = request.Data["from"].ToString();
          string str1 = request.Data["to"].ToString();
          string str2 = ((IEnumerable<string>) allKeys).Contains<string>("permissions") ? request.Data["permissions"].ToString() : (string) null;
          string str3 = ((IEnumerable<string>) allKeys).Contains<string>("owner") ? request.Data["owner"].ToString() : (string) null;
          if (string.IsNullOrEmpty(path) || !IOUtils.IfPathExists(path))
            HTTPUtils.WriteErrorArrayJson(res, "Source path does not exist");
          else if (string.IsNullOrEmpty(str1))
            HTTPUtils.WriteErrorArrayJson(res, "Destination path cannot be empty");
          else if (Utils.IsSharedFolderMounted(requestVmName))
          {
            string oldValue = RegistryManager.Instance.Guest[requestVmName].SharedFolder0Path.Trim('\\');
            string str4 = path.Replace(oldValue, "").TrimStart('\\').Replace("\\", "/");
            if (str1.Contains(" "))
              str1 = "\"" + str1 + "\"";
            string str5 = string.Format("cp -rf \"/sdcard/windows/BstSharedFolder/{0}\" {1}", (object) str4, (object) str1);
            string str6 = string.IsNullOrEmpty(str2) ? string.Format("{0} && busybox chmod -R 777 {1}", (object) str5, (object) str1) : string.Format("{0} && busybox chmod -R {1} {2}", (object) str5, (object) str2, (object) str1);
            if (!string.IsNullOrEmpty(str3))
              str6 = string.Format("{0} && busybox chown -R {1} {2}", (object) str6, (object) str3, (object) str1);
            string str7 = str6.Replace("&", "%26");
            StringBuilder sb = new StringBuilder();
            using (JsonWriter jsonWriter = (JsonWriter) new JsonTextWriter((TextWriter) new StringWriter(sb)))
            {
              jsonWriter.WriteStartObject();
              jsonWriter.WritePropertyName("commands");
              jsonWriter.WriteValue(str7);
              jsonWriter.WriteEndObject();
            }
            string reason = BstHttpClient.Get(string.Format("{0}/exec?d={1}", (object) HTTPUtils.GuestServerUrl(requestVmName, "bgp"), (object) sb.ToString()), (Dictionary<string, string>) null, false, requestVmName, -1, 1, 0, false, "bgp");
            if (reason.Contains("ok"))
              HTTPUtils.WriteSuccessArrayJson(res, reason);
            else
              HTTPUtils.WriteErrorArrayJson(res, reason);
          }
          else
            HTTPUtils.WriteErrorArrayJson(res, "Shared folder not mounted");
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to copy file to android. Err : " + ex.ToString());
        HTTPUtils.WriteErrorArrayJson(res, ex.Message);
      }
    }

    internal static void SetCurrentVolume(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        HTTPUtils.SendRequestToClient("setCurrentVolumeFromAndroid", new Dictionary<string, string>()
        {
          {
            "volume",
            Convert.ToInt32(request.Data["currentvolume"]).ToString()
          }
        }, request.RequestVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
        HTTPUtils.WriteSuccessArrayJson(res, "");
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in set current volume. Err : " + ex.ToString());
        HTTPUtils.WriteErrorArrayJson(res, ex.Message);
      }
    }

    internal static void CopyToWindows(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        string requestVmName = request.RequestVmName;
        string[] allKeys = request.Data.AllKeys;
        if (!((IEnumerable<string>) allKeys).Contains<string>("from"))
        {
          HTTPUtils.WriteErrorArrayJson(res, "Source does not exist");
        }
        else
        {
          string str1 = request.Data["from"].ToString();
          string str2 = "/sdcard/windows/BstSharedFolder";
          if (((IEnumerable<string>) allKeys).Contains<string>("to"))
            str2 = str2 + "/" + request.Data["to"].ToString();
          if (string.IsNullOrEmpty(str1))
            HTTPUtils.WriteErrorArrayJson(res, "Source path does not exist");
          else if (Utils.CheckIfGuestReady(requestVmName, 1))
          {
            string str3 = string.Format("cp -rf {0} {1}", (object) str1, (object) str2);
            StringBuilder sb = new StringBuilder();
            using (JsonWriter jsonWriter = (JsonWriter) new JsonTextWriter((TextWriter) new StringWriter(sb)))
            {
              jsonWriter.WriteStartObject();
              jsonWriter.WritePropertyName("commands");
              jsonWriter.WriteValue(str3);
              jsonWriter.WriteEndObject();
            }
            string reason = BstHttpClient.Get(string.Format("{0}/exec?d={1}", (object) HTTPUtils.GuestServerUrl(requestVmName, "bgp"), (object) sb.ToString()), (Dictionary<string, string>) null, false, requestVmName, -1, 1, 0, false, "bgp");
            if (reason.Contains("ok"))
              HTTPUtils.WriteSuccessArrayJson(res, reason);
            else
              HTTPUtils.WriteErrorArrayJson(res, reason);
          }
          else
            HTTPUtils.WriteErrorArrayJson(res, "Guest not ready");
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to copy file to windows. Err : " + ex.ToString());
        HTTPUtils.WriteErrorArrayJson(res, ex.Message);
      }
    }

    internal static void InstallApp(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        string requestVmName = request.RequestVmName;
        string apkUrl = request.Data["app_download_url"];
        string firstVersion = request.Data["app_version_code"];
        string str = request.Data["app_package_name"];
        Logger.Info("App download url: " + apkUrl);
        Logger.Info("App version code: " + firstVersion);
        Logger.Info("Installing package name: " + str);
        Stats.SendMiscellaneousStatsAsyncForDMM(Stats.DMMEvent.install_app.ToString(), apkUrl, firstVersion, str, (string) null, "Android", 0);
        if (string.IsNullOrEmpty(str))
          throw new Exception("package name cannot be empty");
        string version = string.Empty;
        HTTPHandler.WriteJSON(new Dictionary<string, string>()
        {
          {
            "result_code",
            "0"
          }
        }, res);
        if (!Utils.IsAllUIProcessAlive(requestVmName))
          Utils.sIsGuestReady[requestVmName] = false;
        if (HDAgent.sBootFailed.Contains(requestVmName))
          HDAgent.sBootFailed.Remove(requestVmName);
        string failReason;
        bool flag = Utils.IsAppInstalled(str, requestVmName, out version, out failReason, true);
        if (failReason.Equals("Guest boot failed", StringComparison.OrdinalIgnoreCase))
        {
          if (HDAgent.sBootFailed.Contains(requestVmName))
            return;
          HDAgent.sBootFailed.Add(requestVmName);
        }
        else
        {
          bool isUpgrade = !string.IsNullOrEmpty(version) && Utils.IsFirstVersionHigher(firstVersion, version);
          Logger.Info("isAppInstalled = " + flag.ToString() + ", isUpgrade = " + isUpgrade.ToString());
          if (flag)
          {
            HDAgent.sApkUpgradingList[requestVmName + str] = isUpgrade;
            DownloadInstallApk.GetApkStatusObject(str, requestVmName).status = DownloadInstallStatus.Installed;
          }
          if (flag && !isUpgrade)
            return;
          DownloadInstallApk.DownloadApk(apkUrl, str, requestVmName, isUpgrade);
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to Install app. Err: " + ex.ToString());
        HTTPHandler.WriteJSON(new Dictionary<string, string>()
        {
          {
            "result_code",
            "-1"
          }
        }, res);
      }
    }

    internal static void WriteJSON(Dictionary<string, string> data, HttpListenerResponse res)
    {
      HTTPUtils.Write(JSONUtils.GetJSONArrayString(data), res);
    }

    public static void GetGuid(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        HTTPUtils.Write(new JArray()
        {
          (JToken) new JObject()
          {
            {
              "response",
              (JToken) RegistryManager.Instance.UserGuid
            },
            {
              "success",
              (JToken) true
            }
          }
        }.ToString(Formatting.None), res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in getting guid: {0}" + ex.Message);
        HTTPUtils.WriteErrorArrayJson(res, ex.Message);
      }
    }

    public static void SetVmConfig(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        string vmName = "";
        string vmData = "";
        if (!HTTPHandler.GetVmNameFromRequest(HTTPUtils.ParseRequest(req), ref vmName, ref vmData) || string.IsNullOrEmpty(vmData))
          HTTPUtils.WriteErrorJson(res, "");
        else if (!HTTPHandler.CheckForInstance(vmName))
        {
          string reason = MultiInstanceErrorCodesEnum.VmNotExist.ToString();
          HTTPHandler.WriteVmConfigJson(res, vmName, reason, false, true);
        }
        else
        {
          int requestAndProcess = HTTPHandler.ParseSetVmRequestAndProcess(vmName, vmData);
          if (requestAndProcess < 0)
          {
            string reason = MultiInstanceErrorCodesEnum.WrongValue.ToString();
            if (requestAndProcess == -1)
              reason = MultiInstanceErrorCodesEnum.NotSupportedInLegacyMode.ToString();
            HTTPHandler.WriteVmConfigJson(res, vmName, reason, false, true);
          }
          HTTPHandler.WriteVmConfigJson(res, vmName, "success", true, false);
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in SetVmConfig: " + ex.ToString());
        HTTPUtils.WriteErrorArrayJson(res, ex.Message);
      }
    }

    public static void IsMultiInstanceSupported(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        HTTPUtils.WriteSuccessJson(res, "");
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in IsMultiInstanceSupported, ex: {0}", (object) ex.Message);
        HTTPUtils.WriteErrorJson(res, "");
      }
    }

    private static int ParseSetVmRequestAndProcess(string vmName, string data)
    {
      Logger.Info(nameof (ParseSetVmRequestAndProcess));
      Dictionary<string, int> dictionary = ((IEnumerable<string>) data.Split(new char[1]
      {
        ';'
      }, StringSplitOptions.RemoveEmptyEntries)).Select<string, string[]>((Func<string, string[]>) (part => part.Split('='))).ToDictionary<string[], string, int>((Func<string[], string>) (split => split[0].Trim()), (Func<string[], int>) (split => Convert.ToInt32(split[1].Trim())));
      if (dictionary.ContainsKey("dpi"))
      {
        Logger.Info("Request DPI: " + dictionary["dpi"].ToString());
        HTTPHandler.SetVmConfigDPI(vmName, dictionary);
      }
      if (dictionary.ContainsKey("resolution"))
        Logger.Info("Request Resolution: " + dictionary["resolution"].ToString());
      if (dictionary.ContainsKey("ram"))
      {
        Logger.Info("Request RAM: " + dictionary["ram"].ToString());
        int num = HTTPHandler.SetRam(dictionary["ram"], vmName);
        if (num != 0)
          return num;
        RegistryManager.Instance.Guest[vmName].Memory = dictionary["ram"];
      }
      return 0;
    }

    private static void SetVmConfigDPI(string vmName, Dictionary<string, int> setParams)
    {
      string bootParameters = RegistryManager.Instance.Guest[vmName].BootParameters;
      List<string[]> list1 = ((IEnumerable<string>) bootParameters.Split(new char[1]
      {
        ' '
      }, StringSplitOptions.RemoveEmptyEntries)).Select<string, string[]>((Func<string, string[]>) (part => part.Split('='))).Where<string[]>((Func<string[], bool>) (x => x.Length == 1)).ToList<string[]>();
      Dictionary<string, string> dictionary = ((IEnumerable<string>) bootParameters.Split(new char[1]
      {
        ' '
      }, StringSplitOptions.RemoveEmptyEntries)).Select<string, string[]>((Func<string, string[]>) (part => part.Split('='))).Where<string[]>((Func<string[], bool>) (x => x.Length == 2)).ToDictionary<string[], string, string>((Func<string[], string>) (split => split[0]), (Func<string[], string>) (split => split[1]));
      if (dictionary.ContainsKey("DPI"))
        dictionary["DPI"] = Convert.ToString(setParams["dpi"]);
      else
        dictionary.Add("DPI", Convert.ToString(setParams["dpi"]));
      List<string> list2 = dictionary.Select<KeyValuePair<string, string>, string>((Func<KeyValuePair<string, string>, string>) (x => x.Key + "=" + x.Value)).ToList<string>();
      list2.AddRange(list1.SelectMany<string[], string>((Func<string[], IEnumerable<string>>) (x => (IEnumerable<string>) x)));
      string str = string.Join(" ", list2.ToArray());
      RegistryManager.Instance.Guest[vmName].BootParameters = str;
    }

    public static void DeviceProfileUpdated(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData requestData = HTTPUtils.ParseRequest(req);
        string requestVmName = requestData.RequestVmName;
        HTTPUtils.SendRequestToClient("deviceProfileUpdated", ((IEnumerable<string>) requestData.Data.AllKeys).ToDictionary<string, string, string>((Func<string, string>) (k => k), (Func<string, string>) (k => requestData.Data[k])), requestVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
        HTTPUtils.WriteSuccessArrayJson(res, "");
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in DeviceProfileUpdated: {0}", (object) ex.Message);
        HTTPUtils.WriteErrorArrayJson(res, ex.Message);
      }
    }

    public static void ApkInstalled(HttpListenerRequest req, HttpListenerResponse res)
    {
      bool flag1 = false;
      bool flag2 = false;
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        string requestVmName = request.RequestVmName;
        string clientVersion = RegistryManager.Instance.ClientVersion;
        string campaignName = Utils.GetCampaignName();
        string empty = string.Empty;
        foreach (string allKey1 in request.Data.AllKeys)
        {
          string str1 = request.Data[allKey1];
          Logger.Info(str1);
          JObject json = JObject.Parse(str1);
          TimelineStatsSender.HandleAppInstallEvents(json);
          string str2 = json["source"].ToString().Trim();
          if (string.Compare(str2, "s2p") == 0)
          {
            foreach (string allKey2 in request.Files.AllKeys)
            {
              string file = request.Files[allKey2];
              if (System.IO.File.Exists(file))
                System.IO.File.Delete(file);
            }
            HTTPUtils.WriteSuccessArrayJson(res, "");
            flag1 = true;
          }
          string isUpdate = json["update"].ToString().Trim();
          string str3 = json["package"].ToString().Trim();
          Logger.Info("package: {0}", (object) str3);
          string str4 = json["version"].ToString().Trim();
          string str5 = json["versionName"].ToString().Trim();
          bool isVideoPresent = false;
          bool isGl3Required = false;
          try
          {
            isGl3Required = json["gl3required"].ToObject<bool>();
          }
          catch
          {
            Logger.Warning("GL3 required field is missing");
          }
          if (json["systemApp"].ToObject<bool>() || str3 == "com.android.vending")
          {
            Logger.Info("HTTPHandler: Not creating shortcut for " + str3);
            break;
          }
          if (!flag1)
          {
            Logger.Info("Removing package if present");
            lock (HTTPHandler.s_sync)
              AppUninstaller.RemoveFromJson(str3, requestVmName);
            foreach (string allKey2 in request.Files.AllKeys)
            {
              string file = request.Files[allKey2];
              string str6 = Path.Combine(RegistryStrings.GadgetDir, Path.GetFileName(file));
              try
              {
                if (System.IO.File.Exists(str6))
                  System.IO.File.Delete(str6);
                System.IO.File.Move(file, str6);
              }
              catch (Exception ex)
              {
                Logger.Error("Exception when handling app icons");
                Logger.Error(ex.ToString());
              }
            }
          }
          string apkType = string.Empty;
          if (HTTPHandler.sXapkPackageToBeInstalledList.Contains(str3))
          {
            apkType = "xapk";
            HTTPHandler.sXapkPackageToBeInstalledList.Remove(str3);
          }
          string str7 = json["activities"].ToString().Trim();
          Logger.Info(str7);
          JArray jarray = JArray.Parse(str7);
          foreach (JObject jobject in jarray)
          {
            string img = jobject["img"].ToString().Trim();
            string activity = jobject["activity"].ToString().Trim();
            string str6 = jobject["name"].ToString().Trim();
            if (flag1)
            {
              Stats.SendAppInstallStats(str6, str3, str4, str5, "true", isUpdate, str2, requestVmName, "", "", apkType);
              return;
            }
            lock (HTTPHandler.s_sync)
              ApkInstall.AppInstalled(str6, str3, activity, img, str4, isUpdate, requestVmName, str2, campaignName, clientVersion, isGl3Required, isVideoPresent, apkType, str5);
            if (string.Compare(Oem.Instance.OEM, "bcgp_tw", true) == 0)
              HTTPHandler.ReportAppInstallFinish(str6, str3, activity, img, requestVmName);
          }
          if (jarray.Count == 0 && !str3.Contains("android"))
          {
            if (Oem.Instance.IsMessageBoxToBeDisplayed)
              flag2 = true;
          }
          else
            flag2 = false;
        }
        Logger.Info("returning from appinstalled");
        HTTPUtils.WriteSuccessArrayJson(res, "");
        if (flag2)
        {
          int num = (int) MessageBox.Show("App has been installed");
        }
        Utils.UpdateBlueStacksSizeToRegistryASync();
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in Server ApkInstalled");
        Logger.Error(ex.ToString());
        HTTPUtils.WriteErrorArrayJson(res, ex.Message);
      }
    }

    internal static void EnableDebugLogs(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        Logger.EnableDebugLogs();
        HTTPUtils.WriteSuccessArrayJson(res, "");
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in EnableDebugLogs... Err : " + ex.ToString());
        HTTPUtils.WriteErrorArrayJson(res, ex.Message);
      }
    }

    private static void ReportAppInstallFinish(
      string appName,
      string package,
      string activity,
      string img,
      string vmName)
    {
      HTTPUtils.SendRequestToClient("appinstallfinished", new Dictionary<string, string>()
      {
        {
          "app_name",
          appName
        },
        {
          nameof (package),
          package
        },
        {
          nameof (activity),
          activity
        },
        {
          nameof (img),
          img
        }
      }, vmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
    }

    public static void StopInstance(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        string vmName = "";
        string vmData = "";
        string reason1 = "Success Reason";
        bool isSuccess = false;
        if (!HTTPHandler.GetVmNameFromRequest(HTTPUtils.ParseRequest(req), ref vmName, ref vmData))
          HTTPUtils.WriteErrorJson(res, "");
        else if (!HTTPHandler.CheckForInstance(vmName))
        {
          string reason2 = MultiInstanceErrorCodesEnum.VmNotExist.ToString();
          HTTPHandler.WriteVmConfigJson(res, vmName, reason2, false, true);
        }
        else
        {
          Logger.Info("Trying to stop vm {0}", (object) vmName);
          if (Oem.Instance.IsOEMWithBGPClient)
          {
            try
            {
              HTTPUtils.SendRequestToClientAsync("stopInstance", (Dictionary<string, string>) null, vmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
              bool createdNew;
              using (Mutex mutex = new Mutex(true, BlueStacks.Common.Strings.GetClientInstanceLockName(vmName, "bgp"), out createdNew))
              {
                if (!createdNew)
                {
                  try
                  {
                    mutex.WaitOne(-1);
                  }
                  catch (AbandonedMutexException ex)
                  {
                    Logger.Info("Client closed: " + ex.Message);
                  }
                  catch (Exception ex)
                  {
                    Logger.Error("Could not check if client is running." + ex.Message);
                  }
                }
              }
              isSuccess = true;
            }
            catch
            {
              reason1 = "Unable to connect to client server";
            }
          }
          else if (HTTPHandler.QuitFrontend(vmName))
            isSuccess = true;
          else
            reason1 = "Unable to connect to engine server";
          Logger.Info("Reason: " + reason1);
          if (isSuccess)
          {
            for (int index = 30; index > 0; --index)
            {
              if (!ProcessUtils.IsAlreadyRunning(BlueStacks.Common.Strings.GetPlayerLockName(vmName, "bgp")))
              {
                HTTPHandler.WriteVmConfigJson(res, vmName, reason1, isSuccess, false);
                return;
              }
              Thread.Sleep(2000);
            }
            HTTPHandler.WriteVmConfigJson(res, vmName, "Failed to stop vm", false, false);
          }
          else
            HTTPHandler.WriteVmConfigJson(res, vmName, reason1, isSuccess, false);
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in StopInstance, ex: {0}", (object) ex.Message);
        HTTPUtils.WriteErrorJson(res, ex.Message);
      }
    }

    private static bool QuitFrontend(string vmName)
    {
      try
      {
        Logger.Info("Quiting Frontend for vmId: " + vmName);
        HTTPUtils.SendRequestToEngine("quitFrontend", new Dictionary<string, string>()
        {
          {
            "reason",
            "stop vm instance"
          }
        }, vmName, 0, (Dictionary<string, string>) null, false, 1, 0, "", "bgp");
        return true;
      }
      catch (Exception ex)
      {
        Logger.Error("Exception occured in QuitFrontend: {0}" + ex?.ToString());
        return false;
      }
    }

    public static void GetRunningInstances(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        List<string> stringList = new List<string>();
        List<string> runningInstancesList = Utils.GetRunningInstancesList();
        HTTPHandler.WriteRunningVmsList(res, runningInstancesList);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in GetRunningInstances, ex: {0}", (object) ex.Message);
        HTTPUtils.WriteErrorJson(res, "");
      }
    }

    private static void WriteRunningVmsList(HttpListenerResponse res, List<string> lsVm)
    {
      Logger.Info("In method WriteRunningVmsList");
      string str = string.Join(",", lsVm.ToArray());
      Logger.Info("Running instances: " + str);
      HTTPUtils.Write(new JObject()
      {
        {
          "success",
          (JToken) true
        },
        {
          "vmname",
          (JToken) str
        }
      }.ToString(Formatting.None), res);
    }

    public static void StartInstance(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        string vmName = "";
        string vmData = "";
        bool flag = true;
        bool isSuccess = true;
        int num = 0;
        RequestData request = HTTPUtils.ParseRequest(req);
        if (!HTTPHandler.GetVmNameFromRequest(request, ref vmName, ref vmData))
        {
          string reason = MultiInstanceErrorCodesEnum.VmNameNotValid.ToString();
          HTTPHandler.WriteVmConfigJson(res, vmName, reason, false, true);
        }
        else if (!HTTPHandler.CheckForInstance(vmName))
        {
          string reason = MultiInstanceErrorCodesEnum.VmNotExist.ToString();
          HTTPHandler.WriteVmConfigJson(res, vmName, reason, false, true);
        }
        else
        {
          if (request.Data["hidden"] != null)
            flag = Convert.ToBoolean(request.Data["hidden"]);
          string str = flag ? "-h " : "";
          if (Oem.Instance.IsOEMWithBGPClient)
          {
            ProcessUtils.GetProcessObject(Utils.GetPartnerExecutablePath(), string.Format("{0} -vmname:{1}", (object) str, (object) vmName), false).Start();
          }
          else
          {
            Logger.Info("Running HD-RunApp.exe -h -vm:" + vmName);
            Process process = Process.Start(Path.Combine(HDAgent.s_InstallDir, "HD-RunApp.exe"), str + "-vmname:" + vmName);
            process.WaitForExit();
            num = process.ExitCode;
            Logger.Info("Runapp ExitCode: " + num.ToString());
          }
          string reason;
          if (num < 0)
          {
            reason = System.Enum.GetName(typeof (MultiInstanceErrorCodesEnum), (object) num);
            isSuccess = false;
          }
          else
            reason = "Success Reason";
          Logger.Info("Reason: " + reason);
          HTTPHandler.WriteVmConfigJson(res, vmName, reason, isSuccess, false);
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in StartInstance, ex: {0}", (object) ex.Message);
        HTTPUtils.WriteErrorJson(res, "");
      }
    }

    public static void DeleteInstance(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        string vmName = "";
        string vmData = "";
        DateTime now = DateTime.Now;
        string eventType = "delete";
        RequestData request = HTTPUtils.ParseRequest(req);
        string str = request.Data["isMim"];
        if (!HTTPHandler.GetVmNameFromRequest(request, ref vmName, ref vmData))
          HTTPUtils.WriteErrorJson(res, "");
        else if (!HTTPHandler.CheckForInstanceForDeleteVM(req, res, vmName))
        {
          string reason = MultiInstanceErrorCodesEnum.VmNotExist.ToString();
          HTTPHandler.WriteVmConfigJson(res, vmName, reason, false, true);
        }
        else
        {
          HTTPHandler.CloseInstanceIfRunning(vmName);
          if (vmName == "Android")
          {
            string reason = MultiInstanceErrorCodesEnum.CannotDeleteDefaultVm.ToString();
            HTTPHandler.WriteVmConfigJson(res, vmName, reason, false, true);
          }
          else
          {
            Logger.Info("Running HD-VmManager.exe with deleteinstance " + vmName);
            Process process = new Process();
            process.StartInfo.FileName = Path.Combine(HDAgent.s_InstallDir, "HD-VmManager.exe");
            process.StartInfo.Arguments = "deleteinstance " + vmName;
            process.Start();
            process.WaitForExit();
            int exitCode = process.ExitCode;
            Logger.Info("ExitCode: " + exitCode.ToString());
            bool flag = false;
            string reason;
            if (exitCode == 0)
            {
              flag = true;
              if (RegistryManager.Instance.Guest.ContainsKey(vmName))
              {
                RegistryManager.Instance.Guest.Remove(vmName);
                HTTPHandler.ReinitRegistryInstance(vmName);
              }
              reason = "success";
            }
            else
              reason = System.Enum.GetName(typeof (MultiInstanceErrorCodesEnum), (object) exitCode) ?? MultiInstanceErrorCodesEnum.UnknownException.ToString();
            Logger.Info("Reason: " + reason);
            string vmId = vmName.Split('_')[1];
            string vmType = "";
            if (string.IsNullOrEmpty(str))
              HTTPHandler.SendMultiInstanceStatsAsync(vmType, vmId, now, eventType, exitCode);
            HTTPHandler.WriteVmConfigJson(res, vmName, reason, flag, flag);
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in DeleteInstance, ex: {0}", (object) ex.Message);
        HTTPUtils.WriteErrorJson(res, "");
      }
    }

    internal static void ResetSharedFolders(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        string vmName = "";
        string vmData = "";
        DateTime now = DateTime.Now;
        string eventType = "delete";
        RequestData request = HTTPUtils.ParseRequest(req);
        string str = request.Data["isMim"];
        if (!HTTPHandler.GetVmNameFromRequest(request, ref vmName, ref vmData))
        {
          HTTPUtils.WriteErrorJson(res, "");
        }
        else
        {
          Logger.Info("Running HD-VmManager.exe with resetSharedFolders " + vmName);
          Process process = new Process();
          process.StartInfo.FileName = Path.Combine(HDAgent.s_InstallDir, "HD-VmManager.exe");
          process.StartInfo.Arguments = "resetSharedFolders " + vmName;
          process.Start();
          process.WaitForExit();
          int exitCode = process.ExitCode;
          Logger.Info("ExitCode: " + exitCode.ToString());
          bool flag = false;
          string reason;
          if (exitCode == 0)
          {
            flag = true;
            reason = "success";
          }
          else
            reason = System.Enum.GetName(typeof (MultiInstanceErrorCodesEnum), (object) exitCode) ?? MultiInstanceErrorCodesEnum.UnknownException.ToString();
          Logger.Info("Reason: " + reason);
          string vmId = vmName.Split('_')[1];
          string vmType = "";
          if (string.IsNullOrEmpty(str))
            HTTPHandler.SendMultiInstanceStatsAsync(vmType, vmId, now, eventType, exitCode);
          HTTPHandler.WriteVmConfigJson(res, vmName, reason, flag, flag);
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in ResetSharedFolders, ex: {0}", (object) ex.Message);
        HTTPUtils.WriteErrorJson(res, "");
      }
    }

    internal static void GetInstanceStatus(HttpListenerRequest req, HttpListenerResponse res)
    {
      JArray jarray = new JArray();
      JObject jobject = new JObject();
      try
      {
        string vmName = HTTPUtils.ParseRequest(req).Data["vmname"];
        if (!string.IsNullOrEmpty(vmName) && ((IEnumerable<string>) RegistryManager.Instance.VmList).Contains<string>(vmName))
        {
          if (ProcessUtils.IsLockInUse(BlueStacks.Common.Strings.GetClientInstanceLockName(vmName, "bgp")))
          {
            if (Utils.IsSharedFolderMounted(vmName))
            {
              jobject.Add("success", (JToken) true);
              jobject.Add("status", (JToken) "ready");
              jarray.Add((JToken) jobject);
            }
            else if (Utils.IsGuestBooted(vmName, "bgp"))
            {
              jobject.Add("success", (JToken) true);
              jobject.Add("status", (JToken) "booted");
              jarray.Add((JToken) jobject);
            }
            else
            {
              jobject.Add("success", (JToken) true);
              jobject.Add("status", (JToken) "starting");
              jarray.Add((JToken) jobject);
            }
          }
          else
          {
            jobject.Add("success", (JToken) true);
            jobject.Add("status", (JToken) "stopped");
            jarray.Add((JToken) jobject);
          }
          HTTPUtils.Write(jarray.ToString(Formatting.None), res);
        }
        else
        {
          jobject.Add("success", (JToken) false);
          jobject.Add("status", (JToken) "Invalid vmname");
          jarray.Add((JToken) jobject);
          HTTPUtils.Write(jarray.ToString(Formatting.None), res);
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to get instance status. Err : " + ex.Message);
        jobject.Add("success", (JToken) false);
        jobject.Add("status", (JToken) ex.Message);
        jarray.Add((JToken) jobject);
        HTTPUtils.Write(jarray.ToString(Formatting.None), res);
      }
    }

    internal static void GetCallbackStatus(HttpListenerRequest req, HttpListenerResponse res)
    {
    }

    private static bool CloseInstanceIfRunning(string vmName)
    {
      if (!ProcessUtils.CheckAlreadyRunningAndTakeLock("Global\\BlueStacks_BlueStacksUI_Lockbgp", out Mutex _))
        return true;
      Logger.Info("Instance with name {0} is running, so closing it first", (object) vmName);
      if (Utils.IsAndroidPlayerRunning(vmName, "bgp"))
        return HTTPHandler.QuitFrontend(vmName);
      Logger.Info("No Android Player Running");
      System.Enum.GetName(typeof (MultiInstanceErrorCodesEnum), (object) MultiInstanceErrorCodesEnum.VmNotRunning);
      return false;
    }

    private static bool GetVmNameFromRequest(
      RequestData requestData,
      ref string vmName,
      ref string vmData)
    {
      Logger.Info("In method GetVmNameFromRequest");
      vmName = requestData.Data["vmname"];
      vmData = requestData.Data["data"];
      Logger.Info("vmName: " + vmName);
      Logger.Info("data: " + requestData?.ToString());
      return !string.IsNullOrEmpty(vmName);
    }

    private static bool GetVmDataFromRequest(
      HttpListenerRequest req,
      HttpListenerResponse res,
      ref string vmData)
    {
      RequestData request = HTTPUtils.ParseRequest(req);
      vmData = request.Data["data"];
      return !string.IsNullOrEmpty(vmData);
    }

    public static void CreateInstance(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        string str1 = request.Data["vmtype"];
        bool isSuccess = false;
        string str2 = request.Data["isMim"];
        DateTime now = DateTime.Now;
        string eventType = "create";
        int vmIdToCreate;
        if (((IEnumerable<string>) request.Data.AllKeys).Contains<string>("vmname"))
        {
          Logger.Info("Create Instance data contains vmname");
          vmIdToCreate = int.Parse(Utils.GetVmIdFromVmName(request.Data["vmname"]));
        }
        else
        {
          Logger.Info("Create Instance data doesn't contains vmname");
          vmIdToCreate = Utils.GetVmIdToCreate("bgp");
        }
        Logger.Info("GetVmIdToCreate returns vmId: " + vmIdToCreate.ToString());
        int exitCode = -5;
        Logger.Info("vmType: " + str1);
        if (string.IsNullOrEmpty(str1))
          str1 = "fresh";
        string str3 = request.Data["settings"];
        string vmName = "Android";
        if (string.Equals(str1, "clone", StringComparison.InvariantCultureIgnoreCase))
        {
          if (((IEnumerable<string>) request.Data.AllKeys).Contains<string>("clonefromvm"))
            vmName = request.Data["clonefromvm"];
          if (!((IEnumerable<string>) RegistryManager.Instance.VmList).Contains<string>(vmName))
          {
            HTTPHandler.WriteVmConfigJson(res, "", "VmNotExist", false, true);
            return;
          }
          if (ProcessUtils.IsAlreadyRunning(BlueStacks.Common.Strings.GetPlayerLockName(vmName, "bgp")))
          {
            HTTPHandler.WriteVmConfigJson(res, "", "CannotCloneRunningVm", false, true);
            return;
          }
        }
        string str4 = string.IsNullOrEmpty(str3) ? "\"\"" : "\"" + str3 + "\"";
        string reason;
        if (string.Equals(str1, "fresh", StringComparison.InvariantCultureIgnoreCase) || string.Equals(str1, "clone", StringComparison.InvariantCultureIgnoreCase))
        {
          string str5 = string.Format("createinstance {0} Android_{1} {2} {3}", (object) str1, (object) vmIdToCreate, (object) vmName, (object) str4);
          Logger.Info("Running HD-VmManager with createinstance with args : " + str5);
          exitCode = HTTPHandler.LaunchVmManager(str5);
          reason = HTTPHandler.GetReasonFromExitCodeAndClearRegistryIfExitSuccessful(ref isSuccess, vmIdToCreate, exitCode);
        }
        else
          reason = "InvalidVmType";
        if (reason.Equals("success", StringComparison.InvariantCultureIgnoreCase))
          HTTPHandler.ReinitRegistryInstance("Android_" + vmIdToCreate.ToString());
        Logger.Info("Reason: " + reason);
        if (string.IsNullOrEmpty(str2))
          HTTPHandler.SendMultiInstanceStatsAsync(str1, vmIdToCreate.ToString(), now, eventType, exitCode);
        HTTPHandler.WriteVmConfigJson(res, vmIdToCreate.ToString(), reason, isSuccess, !isSuccess);
      }
      catch (Win32Exception ex)
      {
        if (ex.NativeErrorCode != 1223)
          return;
        HTTPHandler.WriteVmConfigJson(res, "", "user_cancel", false, true);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in CreateInstance : " + ex.ToString());
        HTTPHandler.WriteVmConfigJson(res, "", MultiInstanceErrorCodesEnum.UnknownException.ToString(), false, true);
      }
    }

    private static void ReinitRegistryInstance(string ignoreVM)
    {
      foreach (string vm in RegistryManager.Instance.VmList)
      {
        if (vm != ignoreVM && ProcessUtils.IsLockInUse(BlueStacks.Common.Strings.GetPlayerLockName(vm, "bgp")))
          HTTPUtils.SendRequestToEngineAsync("reinitGuestRegistry", (Dictionary<string, string>) null, vm, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
      }
      if (!ProcessUtils.IsLockInUse("Global\\BlueStacks_BlueStacksUI_Lockbgp"))
        return;
      HTTPUtils.SendRequestToClientAsync("reinitRegistry", (Dictionary<string, string>) null, "Android", 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
    }

    private static string GetReasonFromExitCodeAndClearRegistryIfExitSuccessful(
      ref bool isSuccess,
      int vmId,
      int exitCode)
    {
      string str;
      if (exitCode != 0)
      {
        str = System.Enum.GetName(typeof (MultiInstanceErrorCodesEnum), (object) exitCode) ?? MultiInstanceErrorCodesEnum.UnknownException.ToString();
      }
      else
      {
        isSuccess = true;
        str = "success";
        if (!RegistryManager.Instance.Guest.ContainsKey("Android_" + vmId.ToString()))
          RegistryManager.ClearRegistryMangerInstance();
      }
      return str;
    }

    private static int LaunchVmManager(string arg)
    {
      Process process = new Process();
      process.StartInfo.FileName = Path.Combine(HDAgent.s_InstallDir, "HD-VmManager.exe");
      process.StartInfo.Arguments = arg;
      process.Start();
      process.WaitForExit();
      return process.ExitCode;
    }

    private static void SendMultiInstanceStatsAsync(
      string vmType,
      string vmId,
      DateTime dtStartTime,
      string eventType,
      int exitCode)
    {
      Logger.Info("In SendMultiInstanceStats for vmType: " + vmType + " vmId: " + vmId + " eventType: " + eventType + " starttime: " + dtStartTime.ToString());
      string timeCompletion = (DateTime.Now - dtStartTime).TotalSeconds.ToString();
      Stats.SendMultiInstanceStatsAsync(vmId, Oem.Instance.OEM, vmType, eventType, timeCompletion, exitCode.ToString(), false);
    }

    private static void WriteVmConfigJson(
      HttpListenerResponse res,
      string vmId,
      string reason,
      bool isSuccess,
      bool isIgnore)
    {
      try
      {
        string vmConfig;
        if (string.IsNullOrEmpty(vmId))
        {
          vmConfig = HTTPHandler.PackVmInstanceAndSerialize(vmId, isIgnore);
        }
        else
        {
          if (!vmId.Contains("Android"))
            vmId = "Android_" + vmId;
          vmConfig = HTTPHandler.PackVmInstanceAndSerialize(vmId, isIgnore);
        }
        HTTPHandler.FillJsonWithVmConfig(res, reason, vmConfig, isSuccess);
      }
      catch (Exception ex)
      {
        Logger.Info("Exception in WriteVmConfigJson: " + ex.ToString());
      }
    }

    private static string PackVmInstanceAndSerialize(string vmName, bool isIgnore)
    {
      Logger.Info("In mthod PackVmInstanceAndSerialize");
      VmInstance vmInstance = new VmInstance()
      {
        vmname = vmName,
        vmdisplayname = vmName
      };
      if (string.IsNullOrEmpty(vmName))
      {
        vmInstance.vmram = "";
        vmInstance.vmdpi = "";
      }
      else
      {
        if (!RegistryManager.Instance.Guest.ContainsKey(vmName) && !isIgnore)
        {
          InstanceRegistry instanceRegistry = new InstanceRegistry(vmName, "bgp");
          RegistryManager.Instance.Guest.Add(vmName, instanceRegistry);
        }
        vmInstance.vmram = RegistryManager.Instance.Guest.ContainsKey(vmName) ? RegistryManager.Instance.Guest[vmName].Memory.ToString() : "";
        string str = RegistryManager.Instance.Guest.ContainsKey(vmName) ? RegistryManager.Instance.Guest[vmName].BootParameters : "";
        if (!string.IsNullOrEmpty(str))
        {
          Dictionary<string, string> dictionary = ((IEnumerable<string>) str.Split(new char[1]
          {
            ' '
          }, StringSplitOptions.RemoveEmptyEntries)).Select<string, string[]>((Func<string, string[]>) (part => part.Split('='))).Where<string[]>((Func<string[], bool>) (x => x.Length == 2)).ToDictionary<string[], string, string>((Func<string[], string>) (split => split[0]), (Func<string[], string>) (split => split[1]));
          vmInstance.vmdpi = !dictionary.ContainsKey("DPI") ? "" : dictionary["DPI"];
        }
        else
          vmInstance.vmdpi = "";
      }
      return JsonConvert.SerializeObject((object) vmInstance);
    }

    private static void FillJsonWithVmConfig(
      HttpListenerResponse res,
      string reason,
      string vmConfig,
      bool isSuccess)
    {
      Logger.Info("In method FillJsonWithVmConfig");
      HTTPUtils.Write(new JObject()
      {
        {
          "success",
          (JToken) isSuccess
        },
        {
          nameof (reason),
          (JToken) reason
        },
        {
          "vmconfig",
          (JToken) vmConfig
        }
      }.ToString(Formatting.None), res);
    }

    public static void QueryInstance(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        VmInfo vmInfo = new VmInfo();
        HTTPHandler.PackVmInfoObj(vmInfo);
        HTTPHandler.WriteVmInstances(res, vmInfo);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in QueryInstance");
        Logger.Error(ex.ToString());
        HTTPUtils.WriteErrorArrayJson(res, ex.Message);
      }
    }

    private static void PackVmInfoObj(VmInfo vmInfo)
    {
      Logger.Info("In method PackVmInfoObj");
      vmInfo.vminstances = new List<VmInstance>();
      for (int index = 0; index < RegistryManager.Instance.Guest.Count; ++index)
      {
        VmInstance vmInstance = new VmInstance()
        {
          vmdisplayname = RegistryManager.Instance.VmList[index],
          vmname = RegistryManager.Instance.VmList[index]
        };
        vmInstance.vmram = RegistryManager.Instance.Guest[vmInstance.vmname].Memory.ToString();
        Dictionary<string, string> dictionary = ((IEnumerable<string>) RegistryManager.Instance.Guest[vmInstance.vmname].BootParameters.Split(new char[1]
        {
          ' '
        }, StringSplitOptions.RemoveEmptyEntries)).Select<string, string[]>((Func<string, string[]>) (part => part.Split('='))).Where<string[]>((Func<string[], bool>) (x => x.Length == 2)).ToDictionary<string[], string, string>((Func<string[], string>) (split => split[0]), (Func<string[], string>) (split => split[1]));
        if (dictionary.ContainsKey("DPI"))
          vmInstance.vmdpi = dictionary["DPI"];
        Logger.Info("vmdisplayname: " + vmInstance.vmdisplayname + " vmname: " + vmInstance.vmname + " vmram: " + vmInstance.vmram + " vmdpi: " + vmInstance.vmdpi);
        vmInfo.vminstances.Add(vmInstance);
      }
    }

    private static void WriteVmInstances(HttpListenerResponse res, VmInfo vmInfo)
    {
      Logger.Info("In method WriteVmInstances");
      HTTPUtils.Write(JsonConvert.SerializeObject((object) vmInfo), res);
    }

    public static void AppUninstalled(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        string clientVersion = "";
        string campaignName = "";
        RequestData request = HTTPUtils.ParseRequest(req);
        string requestVmName = request.RequestVmName;
        if (Oem.Instance.IsOEMWithBGPClient)
        {
          clientVersion = RegistryManager.Instance.ClientVersion;
          campaignName = Utils.GetCampaignName();
        }
        Logger.Info("Data:");
        foreach (string allKey in request.Data.AllKeys)
        {
          Logger.Info("Key: {0}, Value: {1}", (object) allKey, (object) request.Data[allKey]);
          JObject json = JObject.Parse(request.Data[allKey]);
          TimelineStatsSender.HandleAppUninstallEvents(json);
          string packageName = json["package"].ToString().Trim();
          string str = json["source"].ToString().Trim();
          if (string.Compare(str, "s2p") == 0)
          {
            string versionFromPackage = HDAgent.GetVersionFromPackage(packageName, requestVmName);
            string versionNameFromPackage = HDAgent.GetVersionNameFromPackage(packageName, requestVmName);
            Stats.SendAppInstallStats(new JsonParser(requestVmName).GetAppNameFromPackage(packageName), packageName, versionFromPackage, versionNameFromPackage, "false", "false", str, requestVmName, "", "", "");
            HTTPUtils.WriteSuccessArrayJson(res, "");
            return;
          }
          Logger.Info("package: {0}", (object) packageName);
          lock (HTTPHandler.s_sync)
            AppUninstaller.AppUninstalled(packageName, requestVmName, str, campaignName, clientVersion);
        }
        HTTPUtils.WriteSuccessArrayJson(res, "");
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in Server AppUninstalled");
        Logger.Error(ex.ToString());
        HTTPUtils.WriteErrorArrayJson(res, ex.Message);
      }
    }

    public static void GetAppList(HttpListenerRequest req, HttpListenerResponse res)
    {
      RequestData request = HTTPUtils.ParseRequest(req);
      Logger.Info("QueryString:");
      foreach (string allKey in request.QueryString.AllKeys)
        Logger.Info("Key: {0}, Value: {1}", (object) allKey, (object) request.QueryString[allKey]);
      string failReason;
      string installedPackages = Utils.GetInstalledPackages(request.RequestVmName, out failReason, out bool _, 0);
      if (string.IsNullOrEmpty(failReason))
        HTTPUtils.Write(installedPackages, res);
      else
        HTTPUtils.WriteErrorArrayJson(res, failReason);
    }

    public static void GetAppImage(HttpListenerRequest req, HttpListenerResponse res)
    {
      RequestData request = HTTPUtils.ParseRequest(req);
      Logger.Info("QueryString:");
      foreach (string allKey in request.QueryString.AllKeys)
        Logger.Info("Key: {0}, Value: {1}", (object) allKey, (object) request.QueryString[allKey]);
      string path = Path.Combine(RegistryStrings.GadgetDir, request.QueryString["image"]);
      if (System.IO.File.Exists(path))
      {
        byte[] buffer = System.IO.File.ReadAllBytes(path);
        res.Headers.Add("Cache-Control: max-age=2592000");
        res.OutputStream.Write(buffer, 0, buffer.Length);
      }
      else
      {
        res.StatusCode = 404;
        res.StatusDescription = "Not Found.";
      }
    }

    public static void ReleaseApkInstallThread(HttpListenerRequest req, HttpListenerResponse res)
    {
      HTTPUtils.ParseRequest(req);
      if (HTTPHandler.sApkInstallThread != null)
      {
        Logger.Info("sApkInstallThread  is not null, making it null now");
        HTTPHandler.sApkInstallThread = (Thread) null;
      }
      else
        Logger.Info("sApkInstallThread already marked null");
    }

    public static void ApkInstall(HttpListenerRequest req, HttpListenerResponse res)
    {
      string result = "";
      Dictionary<string, object> data = new Dictionary<string, object>();
      if (HTTPHandler.sApkInstallThread != null)
      {
        data.Add("success", (object) true);
        data.Add("reason", (object) "APK_INSTALLATION_IN_PROGRESS");
        data.Add("errorCode", (object) 7);
        HTTPUtils.WriteArrayJson(res, data);
      }
      else if (HTTPHandler.sAppUninstallationInProgress)
      {
        data.Add("success", (object) true);
        data.Add("reason", (object) "APP_UNINSTALLATION_IN_PROGRESS");
        data.Add("errorCode", (object) 19);
        HTTPUtils.WriteArrayJson(res, data);
      }
      else
      {
        try
        {
          RequestData requestData = HTTPUtils.ParseRequest(req);
          string vmName = requestData.RequestVmName;
          Logger.Info("Data:");
          foreach (string allKey in requestData.Data.AllKeys)
            Logger.Info("Key: {0}, Value: {1}", (object) allKey, (object) requestData.Data[allKey]);
          if (!((IEnumerable<string>) RegistryManager.Instance.VmList).Contains<string>(vmName))
          {
            Logger.Info("VM: " + vmName + " does not exist");
            data.Add("success", (object) false);
            data.Add("reason", (object) "VM_DOES_NOT_EXIST");
            data.Add("errorCode", (object) 23);
            HTTPUtils.WriteArrayJson(res, data);
          }
          else
          {
            string str1 = requestData.Data["path"];
            string packageName = string.Empty;
            if (string.Equals(Path.GetExtension(str1), ".xapk", StringComparison.InvariantCultureIgnoreCase))
            {
              string extractDirectory = Path.Combine(Path.GetTempPath(), Path.GetFileName(str1));
              CommonInstallUtils.ExtractZip(str1, extractDirectory);
              JToken infoFromXapk = Utils.ExtractInfoFromXapk(str1);
              if (infoFromXapk != null)
              {
                packageName = infoFromXapk.GetValue("package_name");
                Logger.Debug("Package name from manifest.json.." + packageName);
              }
              if (!string.IsNullOrEmpty(packageName))
                HTTPHandler.sXapkPackageToBeInstalledList.Add(packageName);
            }
            else
              packageName = Utils.GetPackageNameFromAPK(str1);
            new Thread((ThreadStart) (() => Utils.SendKeymappingFiledownloadRequest(packageName, vmName)))
            {
              IsBackground = true
            }.Start();
            HTTPHandler.sApkInstallThread = new Thread((ThreadStart) (() =>
            {
              if (Oem.Instance.IsOEMWithBGPClient)
              {
                try
                {
                  HTTPUtils.SendRequestToClient("appInstallStarted", new Dictionary<string, string>()
                  {
                    {
                      "filePath",
                      requestData.Data["path"]
                    }
                  }, vmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
                }
                catch (Exception ex)
                {
                  Logger.Error("Error sending message: " + ex.ToString());
                }
              }
              result = ApkInstall.InstallApk(requestData.Data["path"], vmName);
              Logger.Info("Apk installation result : " + result);
              try
              {
                if (!Oem.Instance.IsOEMWithBGPClient)
                  return;
                string str = Convert.ToString(24);
                try
                {
                  str = Convert.ToString((int) System.Enum.Parse(typeof (ReturnCodes), result));
                }
                catch
                {
                  try
                  {
                    str = Convert.ToString((int) System.Enum.Parse(typeof (GuestApkInstallFailCodes), result));
                  }
                  catch
                  {
                  }
                }
                if (result.Equals("Success", StringComparison.InvariantCultureIgnoreCase))
                  return;
                HTTPUtils.SendRequestToClient("appInstallFailed", new Dictionary<string, string>()
                {
                  {
                    "filePath",
                    requestData.Data["path"]
                  },
                  {
                    "errorCode",
                    str
                  }
                }, vmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
              }
              catch (Exception ex)
              {
                Logger.Error("Error sending message: " + ex.ToString());
              }
            }))
            {
              IsBackground = true
            };
            HTTPHandler.sApkInstallThread.Start();
            HTTPHandler.sApkInstallThread.Join();
            Logger.Info("Apk Install thread has returned");
            if (result == "")
            {
              Logger.Info("the apkinstallationresult is {0}", (object) HTTPHandler.sApkInstallResult);
              result = HTTPHandler.sApkInstallResult;
            }
            HTTPHandler.sApkInstallResult = "";
            HTTPHandler.sApkInstallThread = (Thread) null;
            if (result.Equals("Success", StringComparison.InvariantCultureIgnoreCase))
            {
              data.Add("success", (object) true);
              data.Add("reason", (object) "SUCCESS");
              data.Add("errorCode", (object) 0);
              HTTPUtils.WriteArrayJson(res, data);
            }
            else
            {
              int num = 24;
              try
              {
                num = (int) System.Enum.Parse(typeof (ReturnCodes), result);
              }
              catch
              {
                try
                {
                  num = (int) System.Enum.Parse(typeof (GuestApkInstallFailCodes), result);
                }
                catch
                {
                  Logger.Error("unable to parse install apk error code: {0}", (object) result);
                }
              }
              data.Add("success", (object) false);
              data.Add("reason", (object) result);
              data.Add("errorCode", (object) num);
              HTTPUtils.WriteArrayJson(res, data);
            }
          }
        }
        catch (Exception ex)
        {
          Logger.Error("Exception in Server AppInstall");
          Logger.Error(ex.ToString());
          data.Add("success", (object) false);
          data.Add("reason", (object) ex.Message);
          data.Add("errorCode", (object) 24);
          HTTPUtils.WriteArrayJson(res, data);
        }
        finally
        {
          HTTPHandler.sApkInstallResult = "";
          HTTPHandler.sApkInstallThread = (Thread) null;
        }
      }
    }

    public static void AppUninstall(HttpListenerRequest req, HttpListenerResponse res)
    {
      string reason1 = "";
      if (HTTPHandler.sApkInstallThread != null)
      {
        string reason2 = "APK_INSTALLATION_IN_PROGRESS";
        HTTPUtils.WriteErrorArrayJson(res, reason2);
      }
      else if (HTTPHandler.sAppUninstallationInProgress)
      {
        string reason2 = "APP_UNINSTALLATION_IN_PROGRESS";
        HTTPUtils.WriteErrorArrayJson(res, reason2);
      }
      else
      {
        try
        {
          HTTPHandler.sAppUninstallationInProgress = true;
          RequestData request = HTTPUtils.ParseRequest(req);
          string requestVmName = request.RequestVmName;
          Logger.Info("Data:");
          foreach (string allKey in request.Data.AllKeys)
            Logger.Info("Key: {0}, Value: {1}", (object) allKey, (object) request.Data[allKey]);
          string package = request.Data["package"];
          Logger.Info("package: {0}", (object) package);
          string str = request.Data["nolookup"];
          Logger.Info("nolookup: {0}", (object) str);
          if (AppUninstaller.SilentUninstallApp(package, str != null, requestVmName, out reason1) == 0)
          {
            HDAgent.sApkUpgradingList.Remove(requestVmName + package);
            HDAgent.sApkDownloadInstallStatusList.Remove(requestVmName + package);
            HTTPUtils.WriteSuccessArrayJson(res, reason1);
          }
          else
            HTTPUtils.WriteErrorArrayJson(res, reason1);
        }
        catch (Exception ex)
        {
          Logger.Error("Exception in Server AppUninstall");
          Logger.Error(ex.ToString());
          HTTPUtils.WriteErrorArrayJson(res, ex.Message);
        }
        finally
        {
          HTTPHandler.sAppUninstallationInProgress = false;
        }
      }
    }

    public static void SetLocale(HttpListenerRequest req, HttpListenerResponse res)
    {
      string json = (string) null;
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        string requestVmName = request.RequestVmName;
        Logger.Info("Data:");
        foreach (string allKey in request.Data.AllKeys)
          Logger.Info("Key: {0}, Value: {1}", (object) allKey, (object) request.Data[allKey]);
        string str = request.Data["locale"];
        if (Oem.Instance.IsOEMWithBGPClient)
        {
          if (!Utils.PingPartner(Oem.Instance.OEM, requestVmName))
            Process.Start(Utils.GetPartnerExecutablePath(), "-h");
          if (Utils.WaitForBGPClientPing(40))
            HTTPUtils.SendRequestToClient("updateLocale", new Dictionary<string, string>()
            {
              {
                "locale",
                str
              }
            }, requestVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
        }
        json = Utils.PostToBstCmdProcessorAfterServiceStart("setLocale", new Dictionary<string, string>()
        {
          {
            "arg",
            str
          }
        }, requestVmName, false);
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to set android locale... Err : " + ex.ToString());
      }
      if (!string.IsNullOrEmpty(json))
      {
        try
        {
          if (JObject.Parse(json)["result"].ToString().Equals("ok", StringComparison.InvariantCultureIgnoreCase))
            HTTPHandler.WriteJSON(new Dictionary<string, string>()
            {
              {
                "result_code",
                "0"
              }
            }, res);
          else
            HTTPHandler.WriteJSON(new Dictionary<string, string>()
            {
              {
                "result_code",
                "-1"
              }
            }, res);
        }
        catch
        {
          HTTPHandler.WriteJSON(new Dictionary<string, string>()
          {
            {
              "result_code",
              "-1"
            }
          }, res);
        }
      }
      else
        HTTPHandler.WriteJSON(new Dictionary<string, string>()
        {
          {
            "result_code",
            "-1"
          }
        }, res);
    }

    public static void RunApp(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData request1 = HTTPUtils.ParseRequest(req);
        string requestVmName = request1.RequestVmName;
        if (string.IsNullOrEmpty(requestVmName) || !((IEnumerable<string>) RegistryManager.Instance.VmList).Contains<string>(requestVmName))
        {
          HTTPUtils.WriteErrorArrayJson(res, "Invalid vmname");
        }
        else
        {
          Logger.Info("Data:");
          foreach (string allKey in request1.Data.AllKeys)
            Logger.Info("Key: {0}, Value: {1}", (object) allKey, (object) request1.Data[allKey]);
          HTTPHandler.sCurrentAppPackageFromRunApp = "";
          string str1 = request1.Data["package"];
          string str2 = request1.Data["activity"];
          string str3 = request1.Data["apkUrl"];
          string str4 = request1.Data["json"];
          if (req.UserAgent != null && !req.UserAgent.Contains("BlueStacks"))
          {
            Stats.SendMiscellaneousStatsAsyncForDMM(Stats.DMMEvent.runapp_started.ToString(), str1, str2, str3, str4, "Android", 0);
            if (string.IsNullOrEmpty(str1))
            {
              if (Oem.IsOEMDmm)
              {
                HTTPUtils.WriteArrayJson(res, new Dictionary<string, string>()
                {
                  {
                    "result_code",
                    Convert.ToString(268435457)
                  }
                });
                return;
              }
              HTTPUtils.WriteErrorArrayJson(res, "Please give a non-empty package name");
              return;
            }
            if (string.IsNullOrEmpty(str2))
            {
              if (Oem.IsOEMDmm)
              {
                HTTPUtils.WriteArrayJson(res, new Dictionary<string, string>()
                {
                  {
                    "result_code",
                    Convert.ToString(268435458)
                  }
                });
                return;
              }
              HTTPUtils.WriteErrorArrayJson(res, "Please give a non-empty activity name");
              return;
            }
            if (Oem.Instance.IsLaunchUIOnRunApp)
            {
              if (Oem.IsOEMDmm)
              {
                if (!Utils.IsAllUIProcessAlive(requestVmName))
                  Utils.sIsGuestReady[requestVmName] = false;
                ProcessUtils.StartExe(RegistryManager.Instance.PartnerExePath, requestVmName, false);
              }
              else
              {
                ProcessUtils.GetProcessObject(RegistryManager.Instance.PartnerExePath, string.Format("-vmname {0}", (object) requestVmName), false).Start();
                if (!Utils.CheckIfGuestReady(requestVmName, 180))
                {
                  Logger.Error("Guest not booted after 3 minutes");
                  HTTPUtils.WriteErrorArrayJson(res, "Guest boot failed");
                  return;
                }
              }
            }
            else
              Utils.StartHiddenFrontend(requestVmName, "bgp");
            string failReason;
            if (!Utils.IsAppInstalled(str1, requestVmName, out string _, out failReason, false))
            {
              if (Oem.IsOEMDmm)
              {
                if (failReason.Equals("Guest boot failed", StringComparison.OrdinalIgnoreCase))
                {
                  HTTPUtils.WriteArrayJson(res, new Dictionary<string, string>()
                  {
                    {
                      "result_code",
                      Convert.ToString(268435460)
                    }
                  });
                  return;
                }
                HTTPUtils.WriteArrayJson(res, new Dictionary<string, string>()
                {
                  {
                    "result_code",
                    Convert.ToString(268435459)
                  }
                });
                return;
              }
              HTTPUtils.WriteErrorArrayJson(res, failReason);
              return;
            }
            if (Oem.IsOEMDmm)
            {
              bool flag = false;
              try
              {
                flag = Convert.ToBoolean(request1.Data["is_keymap_enabled"].ToString());
              }
              catch
              {
              }
              if (HDAgent.sAppKeymapStateForDMM.ContainsKey(str1))
                HDAgent.sAppKeymapStateForDMM.Remove(str1);
              HDAgent.sAppKeymapStateForDMM.Add(str1, flag);
            }
            Dictionary<string, string> data = new Dictionary<string, string>()
            {
              {
                "appPackage",
                str1
              }
            };
            if (Utils.WaitForFrontendPingResponse(requestVmName))
            {
              HTTPUtils.SendRequestToEngine("runAppInfo", data, requestVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "", "bgp");
            }
            else
            {
              Logger.Error("Frontend not responding to ping response");
              if (Oem.IsOEMDmm)
              {
                HTTPUtils.WriteArrayJson(res, new Dictionary<string, string>()
                {
                  {
                    "result_code",
                    Convert.ToString(268435460)
                  }
                });
                return;
              }
              HTTPUtils.WriteErrorArrayJson(res, "Frontend Server not running");
              return;
            }
          }
          if (str1.Equals("com.tencent.tmgp.rxcq", StringComparison.CurrentCultureIgnoreCase))
          {
            Logger.Info("Before 15 second sleep for package : {0}", (object) str1);
            Thread.Sleep(15000);
            Logger.Info("After 15 second sleep for package : {0}", (object) str1);
          }
          bool flag1;
          if (string.IsNullOrEmpty(str4))
          {
            string request2 = string.Format("runex {0}/{1}", (object) str1, (object) str2);
            if (str3 != null)
              request2 = string.Format("{0} {1}", (object) request2, (object) str3);
            flag1 = HDAgent.DoRunCmd(request2, requestVmName);
            if (flag1)
            {
              Logger.Info("Assigning value {0} to sCurrentAppPackageFromRunApp", (object) str1);
              HTTPHandler.sCurrentAppPackageFromRunApp = str1;
            }
          }
          else
          {
            string guest = HTTPUtils.SendRequestToGuest("customStartActivity", new Dictionary<string, string>()
            {
              {
                "component",
                str1 + "/" + str2
              },
              {
                "extras",
                str4
              }
            }, requestVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
            Logger.Info("The response we get is: " + guest);
            if (JObject.Parse(guest)["result"].ToString().Trim() == "ok")
            {
              flag1 = true;
              Logger.Info("Assigning value {0} to sCurrentAppPackageFromRunApp", (object) str1);
              HTTPHandler.sCurrentAppPackageFromRunApp = str1;
            }
            else
              flag1 = false;
          }
          if (Oem.IsOEMDmm)
          {
            if (!flag1)
              HTTPUtils.WriteArrayJson(res, new Dictionary<string, string>()
              {
                {
                  "result_code",
                  Convert.ToString(268435461)
                }
              });
            else
              HTTPUtils.WriteArrayJson(res, new Dictionary<string, string>()
              {
                {
                  "result_code",
                  Convert.ToString(0)
                }
              });
            Stats.SendMiscellaneousStatsAsyncForDMM(Stats.DMMEvent.runapp_completed.ToString(), str1, str2, str3, flag1.ToString(), "Android", 0);
          }
          else
            HTTPUtils.WriteSuccessArrayJson(res, "");
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in Server RunApp");
        Logger.Error(ex.ToString());
        if (Oem.IsOEMDmm)
          HTTPUtils.WriteArrayJson(res, new Dictionary<string, string>()
          {
            {
              "result_code",
              Convert.ToString(268435461)
            }
          });
        else
          HTTPUtils.WriteErrorArrayJson(res, ex.Message);
      }
    }

    public static void Restart(HttpListenerRequest req, HttpListenerResponse res)
    {
      HTTPUtils.WriteSuccessArrayJson(res, "");
      string requestVmName = HTTPUtils.ParseRequest(req).RequestVmName;
      if (string.IsNullOrEmpty(requestVmName))
        return;
      HTTPUtils.SendRequestToEngine("quitFrontend", (Dictionary<string, string>) null, requestVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "", "bgp");
    }

    public static void Ping(HttpListenerRequest req, HttpListenerResponse res)
    {
      HTTPUtils.ParseRequest(req);
      try
      {
        JArray jarray = new JArray();
        JObject jobject = new JObject();
        jobject.Add((object) new JProperty("success", (object) true));
        jarray.Add((JToken) jobject);
        HTTPUtils.Write(HTTPHandler.CheckForJsonp(jarray.ToString(Formatting.None), req), res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in Server Ping");
        Logger.Error(ex.ToString());
        HTTPUtils.WriteErrorArrayJson(res, ex.Message);
      }
    }

    public static void AppCrashedInfo(HttpListenerRequest req, HttpListenerResponse res)
    {
      string packageName = "";
      string str1 = "";
      string str2 = "";
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        Logger.Info("AppCrashedInfo Recieved for vmName:{0}", (object) request.RequestVmName);
        Logger.Info("Data:");
        foreach (string allKey in request.Data.AllKeys)
        {
          Logger.Info("Key: {0}, Value: {1}", (object) allKey, (object) request.Data[allKey]);
          JObject jobject1 = JObject.Parse(request.Data[allKey]);
          string str3 = jobject1["shortPackageName"].ToString().Trim();
          Logger.Info("shortPackageName: {0}", (object) str3);
          try
          {
            packageName = jobject1["packageName"].ToString().Trim();
            Logger.Info("packageName: {0}", (object) packageName);
            str1 = jobject1["versionCode"].ToString().Trim();
            Logger.Info("versionCode: {0}", (object) str1);
            str2 = jobject1["versionName"].ToString().Trim();
            Logger.Info("versionName: {0}", (object) str2);
          }
          catch
          {
            Logger.Error("Only shortPackageName received");
          }
          if (Features.IsFeatureEnabled(268435456UL))
          {
            int num = 10;
            while (num-- > 0)
            {
              if (!ProcessUtils.CheckAlreadyRunningAndTakeLock("App_C_Info.txt", out HTTPHandler.sAppCrashInfoWriteLock))
              {
                try
                {
                  string str4 = "";
                  string path = Path.Combine(RegistryManager.Instance.LogDir, "App_C_Info.txt");
                  if (System.IO.File.Exists(path))
                    str4 = System.IO.File.ReadAllText(path, Encoding.UTF8) + "\n";
                  JObject jobject2 = new JObject()
                  {
                    {
                      "shortPackageName",
                      (JToken) str3
                    },
                    {
                      "packageName",
                      (JToken) packageName
                    },
                    {
                      "versionName",
                      (JToken) str2
                    },
                    {
                      "versionCode",
                      (JToken) str1
                    },
                    {
                      "time",
                      (JToken) DateTime.Now.ToString()
                    }
                  };
                  System.IO.File.WriteAllText(path, str4 + jobject2.ToString(Formatting.None), Encoding.UTF8);
                }
                catch (Exception ex)
                {
                  Logger.Error("Error Occurred, Err: {0}", (object) ex.ToString());
                }
                HTTPHandler.sAppCrashInfoWriteLock.Close();
                break;
              }
              Thread.Sleep(1000);
            }
          }
        }
        try
        {
          Logger.Info("Closing app: {0}", (object) packageName);
          HTTPUtils.SendRequestToClient("closeCrashedAppTab", new Dictionary<string, string>()
          {
            {
              "package",
              packageName
            }
          }, request.RequestVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
        }
        catch (Exception ex)
        {
          Logger.Error("There occured an exception while trying to post data " + ex.ToString());
        }
        Logger.Info("Files:");
        foreach (string allKey in request.Files.AllKeys)
        {
          Logger.Info("Key: {0}, Value: {1}", (object) allKey, (object) request.Files[allKey]);
          Path.GetFileName(request.Files[allKey]);
        }
        if (!string.IsNullOrEmpty(Oem.Instance.SendCrashLogForApkWithString) && packageName.Contains(Oem.Instance.SendCrashLogForApkWithString, StringComparison.InvariantCultureIgnoreCase) && !HDAgent.sApkCrashLogsUploadBlackListedApps.Any<string>((Func<string, bool>) (pkg => packageName.StartsWith(pkg))))
          HTTPHandler.StartLogCollection("APP_CRASHED", packageName);
        HTTPUtils.WriteSuccessArrayJson(res, "");
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in Server AppCrashedInfo");
        Logger.Error(ex.ToString());
        HTTPUtils.WriteErrorArrayJson(res, ex.Message);
      }
    }

    public static void StartLogCollection(string error, string detail)
    {
      if (!Oem.Instance.IsReportExeAppCrashLogs)
        return;
      Logger.Info("starting the logging of apk installation failure");
      Process process = new Process();
      string installDir = RegistryStrings.InstallDir;
      process.StartInfo.FileName = Path.Combine(installDir, "HD-LogCollector.exe");
      string str = "-ReportCrashLogs " + error + " " + detail + " -hidden";
      Logger.Info("The arguments being passed to log collector is :{0}", (object) str);
      process.StartInfo.Arguments = str;
      process.Start();
      process.WaitForExit();
    }

    public static void NotifyLogReportingToParentWindow(int param)
    {
      try
      {
        Dictionary<string, string[]> oemWindowMapper = new Dictionary<string, string[]>();
        Utils.AddMessagingSupport(out oemWindowMapper);
        if (oemWindowMapper == null || !oemWindowMapper.ContainsKey(Oem.Instance.OEM))
          return;
        string cls = oemWindowMapper[Oem.Instance.OEM][0];
        string name = oemWindowMapper[Oem.Instance.OEM][1];
        Logger.Info("Sending WM_USER_LOGS_REPORTING message to class = {0}, window = {1}", (object) cls, (object) name);
        IntPtr window = InteropWindow.FindWindow(cls, name);
        if (window == IntPtr.Zero)
        {
          Logger.Info("Unable to find window : {0}", (object) cls);
        }
        else
        {
          uint num = (uint) param;
          Logger.Info("Sending wparam : {0}", (object) num);
          InteropWindow.SendMessage(window, 1072U, (IntPtr) (long) num, IntPtr.Zero);
        }
      }
      catch (Exception ex)
      {
        Logger.Error(string.Format("Error Occured, Err: {0}", (object) ex.ToString()));
      }
    }

    public static void GetUserData(HttpListenerRequest req, HttpListenerResponse res)
    {
      HTTPUtils.ParseRequest(req);
      try
      {
        HTTPUtils.Write(HTTPHandler.CheckForJsonp(new JObject()
        {
          {
            "guid",
            (JToken) RegistryManager.Instance.UserGuid
          },
          {
            "email",
            (JToken) RegistryManager.Instance.RegisteredEmail
          },
          {
            "version",
            (JToken) "4.250.0.1070"
          },
          {
            "culture",
            (JToken) CultureInfo.CurrentCulture.Name.ToLower()
          },
          {
            "success",
            (JToken) true
          }
        }.ToString(Formatting.None), req), res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in Server GetUserData");
        Logger.Error(ex.ToString());
        HTTPUtils.WriteErrorArrayJson(res, ex.Message);
      }
    }

    public static void ShowNotification(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        Logger.Info("Data");
        foreach (string allKey in request.Data.AllKeys)
          Logger.Info("Key: {0}, Value: {1}", (object) allKey, (object) request.Data[allKey]);
        string action = request.Data["msgAction"];
        string title = request.Data["displayTitle"];
        string message = request.Data["displayMsg"];
        string actionURL = request.Data["actionURL"];
        string fileName = request.Data["fileName"];
        string imageURL = request.Data["imageURL"];
        string vmName;
        try
        {
          vmName = request.Data["vmname"];
        }
        catch (Exception ex)
        {
          vmName = "Android";
        }
        CloudAnnouncement.ShowNotification(action, title, message, actionURL, fileName, imageURL, vmName);
        HTTPUtils.WriteSuccessJson(res, "");
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in Server ShowNotification");
        Logger.Error(ex.ToString());
        HTTPUtils.WriteErrorArrayJson(res, ex.Message);
      }
    }

    public static void ShowFeNotification(HttpListenerRequest req, HttpListenerResponse res)
    {
      RequestData request = HTTPUtils.ParseRequest(req);
      string requestVmName = request.RequestVmName;
      try
      {
        HTTPUtils.SendRequestToEngine("showFENotification", new Dictionary<string, string>()
        {
          {
            "data",
            JObject.Parse(request.Data["data"]).ToString()
          }
        }, requestVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "", "bgp");
        HTTPUtils.WriteSuccessJson(res, "");
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in ShowFeNotification: " + ex.ToString());
        HTTPUtils.WriteErrorArrayJson(res, ex.Message);
      }
    }

    internal static void OpenNotificationsSettingsInGM(string vmName)
    {
      new Thread((ThreadStart) (() =>
      {
        int partnerServerPort = RegistryManager.Instance.PartnerServerPort;
        try
        {
          HTTPUtils.SendRequestToClient("showWindow", new Dictionary<string, string>()
          {
            ["showNotifications"] = "false"
          }, vmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
        }
        catch
        {
        }
        HTTPUtils.SendRequestToClient("openNotificationSettings", (Dictionary<string, string>) null, vmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
      }))
      {
        IsBackground = true
      }.Start();
    }

    public static void SendAppDataToFE(
      string package,
      string activity,
      string callingPackage,
      string vmName)
    {
      Logger.Info("HTTPHandler:SendAppDataToFE(\"{0}\",\"{1}\")", (object) package, (object) activity);
      try
      {
        HTTPUtils.SendRequestToEngine("appDataFeUrl", new Dictionary<string, string>()
        {
          {
            nameof (package),
            package
          },
          {
            nameof (activity),
            activity
          },
          {
            nameof (callingPackage),
            callingPackage
          }
        }, vmName, 0, (Dictionary<string, string>) null, false, 1, 0, "", "bgp");
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in SendAppDataToFE: " + ex.ToString());
      }
    }

    public static void SetCpu(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        int num = Convert.ToInt32(request.Data["value"]);
        string index = request.Data["vmname"];
        if (string.IsNullOrEmpty(index))
          index = "Android";
        if (num <= 0)
        {
          HTTPUtils.WriteErrorArrayJson(res, string.Format("Cpu value should be in 1 to {0} range.", (object) 8));
        }
        else
        {
          if (num > 8)
            num = 8;
          if (!Utils.IsEngineRaw())
          {
            RegistryManager.Instance.Guest[index].VCPUs = num;
            HTTPUtils.WriteSuccessArrayJson(res, "");
          }
          else
            HTTPUtils.WriteErrorArrayJson(res, "Cannot set cpu in raw mode");
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to set cpu. Ex : " + ex.ToString());
        HTTPUtils.WriteErrorArrayJson(res, ex.Message);
      }
    }

    internal static void SetDpi(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        string updatedValue = request.Data["value"];
        string vmName = request.Data["vmname"];
        if (string.IsNullOrEmpty(vmName))
          vmName = "Android";
        if (updatedValue == "160" || updatedValue == "240" || updatedValue == "320")
        {
          Utils.SetDPIInBootParameters(RegistryManager.Instance.Guest[vmName].BootParameters, updatedValue, vmName, "bgp");
          HTTPUtils.WriteSuccessArrayJson(res, "");
        }
        else
          HTTPUtils.WriteErrorArrayJson(res, "wrong value");
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to set dpi. Ex : " + ex.ToString());
        HTTPUtils.WriteErrorArrayJson(res, "failed to set dpi");
      }
    }

    public static void SetRam(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        int int32 = Convert.ToInt32(request.Data["value"]);
        string vmName = request.Data["vmname"];
        if (string.IsNullOrEmpty(vmName))
          vmName = "Android";
        switch (HTTPHandler.SetRam(int32, vmName))
        {
          case -1:
            HTTPUtils.WriteErrorArrayJson(res, "cant set ram in legacy mode");
            break;
          case 0:
            HTTPUtils.WriteSuccessArrayJson(res, "");
            break;
          default:
            HTTPUtils.WriteErrorArrayJson(res, "wrong value");
            break;
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Error while setting ram. Ex : " + ex.ToString());
        HTTPUtils.WriteErrorArrayJson(res, "failed to set ram");
      }
    }

    internal static void SetResolution(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        string index = request.Data["vmname"];
        int int32_1 = Convert.ToInt32(request.Data["width"]);
        int int32_2 = Convert.ToInt32(request.Data["height"]);
        if (string.IsNullOrEmpty(index))
          index = "Android";
        if (int32_1 < 0 || int32_2 < 0)
        {
          HTTPUtils.WriteErrorArrayJson(res, "width or height cannot be negative");
        }
        else
        {
          RegistryManager.Instance.Guest[index].GuestWidth = int32_1;
          RegistryManager.Instance.Guest[index].GuestHeight = int32_2;
          HTTPUtils.WriteSuccessArrayJson(res, "");
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to set resolution. Ex : " + ex.ToString());
        HTTPUtils.WriteErrorArrayJson(res, "failed to set resolution");
      }
    }

    public static void ShowSysTrayNotification(HttpListenerRequest req, HttpListenerResponse res)
    {
      RequestData request = HTTPUtils.ParseRequest(req);
      string requestVmName = request.RequestVmName;
      try
      {
        foreach (string allKey in request.Data.AllKeys)
          Logger.Debug("Key: {0}, Value: {1}", (object) allKey, (object) request.Data[allKey]);
        string message = request.Data["message"];
        string str = request.Data["title"];
        int int32 = Convert.ToInt32(request.Data["timeout"]);
        string packageNameFromAppName = new JsonParser(requestVmName).GetPackageNameFromAppName(str);
        if (request.Data["status"] != null && request.Data["status"].Equals("error"))
          SysTray.ShowTrayStatus(ToolTipIcon.Error, str, message, int32, requestVmName, packageNameFromAppName, "0");
        else
          SysTray.ShowTrayStatus(ToolTipIcon.Info, str, message, int32, requestVmName, packageNameFromAppName, "0");
        HTTPUtils.WriteSuccessJson(res, "");
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in ShowSysTrayNotification: " + ex.ToString());
        HTTPUtils.WriteErrorArrayJson(res, ex.Message);
      }
    }

    public static void ExitAgent(HttpListenerRequest req, HttpListenerResponse res)
    {
      HTTPUtils.ParseRequest(req);
      try
      {
        HTTPUtils.WriteSuccessJson(res, "");
        SysTray.DisposeIcon();
        Environment.Exit(0);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in ExitAgent: " + ex.ToString());
        HTTPUtils.WriteErrorArrayJson(res, ex.Message);
        Environment.Exit(-1);
      }
    }

    public static void QuitFrontend(HttpListenerRequest req, HttpListenerResponse res)
    {
      string requestVmName = HTTPUtils.ParseRequest(req).RequestVmName;
      Dictionary<string, string> data = new Dictionary<string, string>()
      {
        {
          "reason",
          "app_exiting"
        }
      };
      try
      {
        HTTPUtils.SendRequestToEngine("quitFrontend", data, requestVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "", "bgp");
        HTTPUtils.WriteSuccessJson(res, "");
      }
      catch (Exception ex)
      {
        Logger.Warning("An error occured while sending quit frontend: {0}", (object) ex.Message);
        HTTPUtils.WriteErrorJson(res, "");
      }
    }

    public static void GuestStatusUpdate(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        TimelineStatsSender.HandleGuestStatusUpdate(HTTPUtils.ParseRequest(req));
        HTTPUtils.WriteSuccessJson(res, "");
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in HandleGuestBootStatusUpdate");
        Logger.Error(ex.ToString());
        HTTPUtils.WriteErrorJson(res, ex.Message);
      }
    }

    public static void FrontendStatusUpdate(HttpListenerRequest req, HttpListenerResponse res)
    {
      Logger.Info("HTTPHandler: Got HandleFrontendStatusUpdate {0} request from {1}", (object) req.HttpMethod, (object) req.RemoteEndPoint.ToString());
      try
      {
        TimelineStatsSender.HandleFrontendStatusUpdate(HTTPUtils.ParseRequest(req));
        HTTPUtils.WriteSuccessJson(res, "");
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in HandleFrontendStatusUpdate");
        Logger.Error(ex.ToString());
        HTTPUtils.WriteErrorJson(res, ex.Message);
      }
    }

    public static void SetNewLocation(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        string requestVmName = request.RequestVmName;
        double num1 = Convert.ToDouble(request.Data["latitude"], (IFormatProvider) CultureInfo.InvariantCulture);
        double num2 = Convert.ToDouble(request.Data["longitude"], (IFormatProvider) CultureInfo.InvariantCulture);
        JObject jobject1 = new JObject();
        jobject1.Add((object) new JProperty("latitude", (object) num1));
        jobject1.Add((object) new JProperty("longitude", (object) num2));
        JObject jobject2 = jobject1;
        Logger.Info("latitude={0} longitude={1}", (object) num1, (object) num2);
        string guest = HTTPUtils.SendRequestToGuest("setNewLocation", new Dictionary<string, string>()
        {
          {
            "data",
            jobject2.ToString(Formatting.None)
          }
        }, requestVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
        Logger.Info("the response is {0}", (object) guest);
        if (JObject.Parse(guest)["result"].ToString() == "ok")
          HTTPUtils.WriteSuccessArrayJson(res, "");
        else
          HTTPUtils.WriteErrorArrayJson(res, guest);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in SetNewLocation");
        Logger.Error(ex.ToString());
        HTTPUtils.WriteErrorArrayJson(res, ex.Message);
      }
    }

    public static void HandleAdEvents(HttpListenerRequest req, HttpListenerResponse res)
    {
      Logger.Info("HTTPHandler: Got HandleAdEvents {0} request from {1}", (object) req.HttpMethod, (object) req.RemoteEndPoint.ToString());
      try
      {
        TimelineStatsSender.HandleAdEvents(HTTPUtils.ParseRequest(req));
        HTTPUtils.WriteSuccessJson(res, "");
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in HandleAdEvents");
        Logger.Error(ex.ToString());
        HTTPUtils.WriteErrorArrayJson(res, ex.Message);
      }
    }

    public static void SetClipboardData(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData requestData = HTTPUtils.ParseRequest(req);
        Thread thread = new Thread((ThreadStart) (() => Clipboard.SetText(requestData.Data["text"])));
        thread.SetApartmentState(ApartmentState.STA);
        thread.IsBackground = true;
        thread.Start();
        HTTPUtils.WriteSuccessJson(res, "");
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in Server SetClipboardData");
        Logger.Error(ex.ToString());
        HTTPUtils.WriteErrorArrayJson(res, ex.Message);
      }
    }

    private static void SendPendingNotifications(object obj, string vmName, string id)
    {
      Logger.Info("Inside SendPendingNotifications");
      HTTPHandler.s_NotificationLockHelper[1] = true;
      HTTPHandler.s_LockForTurn = 0;
      if (HTTPHandler.s_NotificationLockHelper[0] && HTTPHandler.s_LockForTurn == 0)
      {
        Logger.Info(string.Format("Critical resources are being used, returning"));
        HTTPHandler.s_NotificationLockHelper[1] = false;
      }
      else
      {
        try
        {
          while (HTTPHandler.s_PendingNotifications.Count > 0)
          {
            if (HTTPHandler.s_PendingNotifications.First.Value.NotificationSent)
            {
              HTTPHandler.s_PendingNotifications.RemoveFirst();
            }
            else
            {
              string package = HTTPHandler.s_PendingNotifications.First.Value.Package;
              string appName = HTTPHandler.s_PendingNotifications.First.Value.AppName;
              string message = HTTPHandler.s_PendingNotifications.First.Value.Message;
              string id1 = HTTPHandler.s_PendingNotifications.First.Value.Id;
              string vmName1 = HTTPHandler.s_PendingNotifications.First.Value.VmName;
              SysTray.ShowInfoShort(appName, message, vmName1, package, id1);
              HTTPHandler.s_PendingNotifications.RemoveFirst();
              if (HTTPHandler.s_PendingNotifications.Count == 0)
              {
                HTTPHandler.s_PendingNotifications.AddFirst(new AndroidNotification(package, appName, message, vmName1, id1));
                HTTPHandler.s_PendingNotifications.First.Value.NotificationSent = true;
                break;
              }
              break;
            }
          }
          if (HTTPHandler.s_PendingNotifications.Count == 0)
          {
            HTTPHandler.s_NotificationTimer.Dispose();
            HTTPHandler.s_NotificationTimer = (System.Threading.Timer) null;
          }
        }
        catch (Exception ex)
        {
          Logger.Error(string.Format("Error Occured, Err : {0}", (object) ex.ToString()));
        }
        HTTPHandler.s_NotificationLockHelper[1] = false;
      }
    }

    internal static void AppDownloadStatus(HttpListenerRequest req, HttpListenerResponse res)
    {
      RequestData request = HTTPUtils.ParseRequest(req, false);
      Dictionary<string, string> data = new Dictionary<string, string>();
      foreach (string allKey in request.Data.AllKeys)
        data[allKey] = request.Data[allKey];
      HTTPUtils.SendRequestToClient("googlePlayAppInstall", data, request.RequestVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
      if (!data.ContainsKey("status") || !string.Equals(data["status"], "Started", StringComparison.InvariantCultureIgnoreCase) || !data.ContainsKey("packageName"))
        return;
      Utils.SendKeymappingFiledownloadRequest(data["packageName"], request.RequestVmName);
    }

    internal static void BindMount(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        HTTPUtils.Write(HTTPUtils.SendRequestToGuest("bindmount", (Dictionary<string, string>) null, HTTPUtils.ParseRequest(req, false).RequestVmName, 1000, (Dictionary<string, string>) null, false, 1, 0, "bgp"), res);
      }
      catch (Exception ex)
      {
        Logger.Error(string.Format("Error Occured, Err : {0}", (object) ex.ToString()));
        HTTPUtils.Write(ex.ToString(), res);
      }
    }

    internal static void UnbindMount(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        HTTPUtils.Write(HTTPUtils.SendRequestToGuest("unbindmount", (Dictionary<string, string>) null, HTTPUtils.ParseRequest(req, false).RequestVmName, 1000, (Dictionary<string, string>) null, false, 1, 0, "bgp"), res);
      }
      catch (Exception ex)
      {
        Logger.Error(string.Format("Error Occured, Err : {0}", (object) ex.ToString()));
        HTTPUtils.Write(ex.ToString(), res);
      }
    }

    public static void NotificationHandler(HttpListenerRequest req, HttpListenerResponse res)
    {
      if (!Features.IsFeatureEnabled(1024UL))
      {
        Logger.Info("Android notifications disabled. Not showing.");
        HTTPUtils.WriteSuccessJson(res, "");
      }
      else
      {
        HTTPHandler.s_NotificationLockHelper[0] = true;
        HTTPHandler.s_LockForTurn = 1;
        while (HTTPHandler.s_NotificationLockHelper[1] && HTTPHandler.s_LockForTurn == 1)
          Thread.Sleep(100);
        try
        {
          RequestData request = HTTPUtils.ParseRequest(req, false);
          foreach (string allKey in request.Data.AllKeys)
          {
            Logger.Debug("Key: {0}, Value: {1}", (object) allKey, (object) request.Data[allKey]);
            JObject jobject = JObject.Parse(request.Data[allKey]);
            string str1 = jobject["pkg"].ToString().Trim();
            string id = jobject["id"].ToString().Trim();
            string str2 = jobject["title"].ToString().Trim();
            string str3 = jobject["text"].ToString().Trim();
            string vmName = request.RequestVmName;
            string str4 = !string.IsNullOrEmpty(str2) || !string.IsNullOrEmpty(str3) ? str2 + "\n" + str3 : LocaleStrings.GetLocalizedString("STRING_BLANK_NOTIFICATION", "");
            if (str4.Contains("%"))
            {
              Logger.Debug("HTTPHandler: Not showing notification for {0} because the content seems to show download info", (object) str1);
              Dictionary<string, string> data = new Dictionary<string, string>();
              string[] strArray = str4.Split('\n');
              data.Add("app_name", strArray[0]);
              data.Add("pkg", str1);
              data.Add("vmname", vmName);
              if (strArray.Length == 2)
                data.Add("progress", strArray[1].Replace("%", ""));
              else if (strArray.Length == 3)
              {
                data.Add("time_left", strArray[1]);
                data.Add("progress", strArray[2].Replace("%", ""));
              }
              if (!Oem.Instance.IsOEMWithBGPClient)
                HTTPUtils.SendRequestToClient("appinstallprogress", data, vmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
            }
            else if (int.Parse(id) == -1050172287 && str1.Equals("com.android.vending"))
            {
              Dictionary<string, string> data = new Dictionary<string, string>()
              {
                {
                  "app_name",
                  str4.Split('\n')[0]
                },
                {
                  "pkg",
                  str1
                },
                {
                  "progress",
                  "100"
                },
                {
                  "vmname",
                  vmName
                }
              };
              if (!Oem.Instance.IsOEMWithBGPClient)
                HTTPUtils.SendRequestToClient("appinstallprogress", data, vmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
            }
            else if (str1 == "bn.ereader" || str1 == "com.amazon.venezia" || (str1 == "getjar.android.client" || str1 == "me.onemobile.android") || (str1 == "com.movend.gamebox" || str1 == "com.android.vending"))
            {
              Logger.Info("HTTPHandler: Not showing notification for " + str1);
            }
            else
            {
              Logger.Info("HTTPHandler: Showing notification for " + str1);
              JsonParser jsonParser;
              if (string.Equals(str1, "com.bluestacks.filemanager", StringComparison.InvariantCultureIgnoreCase))
              {
                jsonParser = new JsonParser(string.Empty);
                if (id.Equals("200", StringComparison.InvariantCultureIgnoreCase))
                {
                  id = "200";
                  HTTPUtils.SendRequestToClient("showImageUploadedInfo", (Dictionary<string, string>) null, vmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
                }
              }
              else
                jsonParser = new JsonParser(vmName);
              string appName;
              string imageName;
              if (!jsonParser.GetAppInfoFromPackageName(str1, out appName, out imageName, out string _, out string _))
              {
                Logger.Warning("Systray: Notifying app {0} not found!", (object) str1);
              }
              else
              {
                Path.Combine(RegistryStrings.GadgetDir, imageName);
                lock (HTTPHandler.s_NotificationLock)
                {
                  bool flag = false;
                  while (HTTPHandler.s_PendingNotifications.Count > 0 && string.Compare(HTTPHandler.s_PendingNotifications.Last.Value.AppName, appName, true) == 0 && !HTTPHandler.s_PendingNotifications.Last.Value.NotificationSent)
                  {
                    if (!HTTPHandler.s_PendingNotifications.First.Value.OldNotificationFlag)
                      flag = true;
                    HTTPHandler.s_PendingNotifications.RemoveLast();
                  }
                  if (flag)
                  {
                    HTTPHandler.s_PendingNotifications.AddLast(new AndroidNotification(str1, appName, str4, vmName, id));
                    HTTPHandler.s_PendingNotifications.Last.Value.NotificationSent = true;
                  }
                  HTTPHandler.s_PendingNotifications.AddLast(new AndroidNotification(str1, appName, str4, vmName, id));
                  if (HTTPHandler.s_PendingNotifications.Count == 1)
                  {
                    SysTray.ShowInfoShort(appName, str4, vmName, str1, id);
                    HTTPHandler.s_PendingNotifications.Last.Value.NotificationSent = true;
                    if (HTTPHandler.s_NotificationTimer == null)
                      HTTPHandler.s_NotificationTimer = new System.Threading.Timer((TimerCallback) (x => HTTPHandler.SendPendingNotifications(x, vmName, id)), (object) null, HTTPHandler.s_NotificationTimeout, HTTPHandler.s_NotificationTimeout);
                    else
                      HTTPHandler.s_NotificationTimer.Change(HTTPHandler.s_NotificationTimeout, HTTPHandler.s_NotificationTimeout);
                  }
                  else
                  {
                    if (HTTPHandler.s_PendingNotifications.Count > 0 && HTTPHandler.s_PendingNotifications.First.Value.NotificationSent)
                      HTTPHandler.s_PendingNotifications.RemoveFirst();
                    while (HTTPHandler.s_PendingNotifications.Count > 0)
                    {
                      if (string.Compare(HTTPHandler.s_PendingNotifications.First.Value.AppName, appName, true) != 0)
                      {
                        SysTray.ShowInfoShort(HTTPHandler.s_PendingNotifications.First.Value.AppName, HTTPHandler.s_PendingNotifications.First.Value.Message, HTTPHandler.s_PendingNotifications.First.Value.VmName, HTTPHandler.s_PendingNotifications.First.Value.Package, id);
                        HTTPHandler.s_PendingNotifications.RemoveFirst();
                      }
                      else
                        break;
                    }
                  }
                }
              }
            }
          }
          HTTPUtils.WriteSuccessJson(res, "");
        }
        catch (Exception ex)
        {
          Logger.Error("Exception in Server NotificationHandler");
          Logger.Error(ex.ToString());
          HTTPUtils.WriteErrorArrayJson(res, ex.Message);
        }
        HTTPHandler.s_NotificationLockHelper[0] = false;
      }
    }

    public static void IsAppInstalled(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        string requestVmName = request.RequestVmName;
        foreach (string allKey in request.Data.AllKeys)
          Logger.Info("Key: {0}, Value: {1}", (object) allKey, (object) request.Data[allKey]);
        string package = request.Data["package"];
        Utils.StartHiddenFrontend(requestVmName, "bgp");
        string vmName = requestVmName;
        string str1;
        ref string local1 = ref str1;
        string str2;
        ref string local2 = ref str2;
        bool flag = Utils.IsAppInstalled(package, vmName, out local1, out local2, true);
        if (!flag && !string.IsNullOrEmpty(str2) && string.Compare(str2, "package not installed", true) != 0)
        {
          HTTPUtils.WriteErrorArrayJson(res, str2);
        }
        else
        {
          JObject jobject = new JObject();
          if (flag)
            jobject.Add("version", (JToken) str1);
          jobject.Add("success", (JToken) true);
          jobject.Add("installed", (JToken) flag);
          Logger.Info("Sending response: " + jobject.ToString());
          HTTPUtils.Write(jobject.ToString(Formatting.None), res);
          Logger.Info("Sent response");
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in Server IsAppInstalled");
        Logger.Error(ex.ToString());
        HTTPUtils.WriteErrorArrayJson(res, ex.Message);
      }
    }

    public static void RestartAgent(HttpListenerRequest req, HttpListenerResponse res)
    {
      HTTPUtils.ParseRequest(req);
      string str = Path.Combine(RegistryStrings.InstallDir, "HD-Agent.exe");
      Logger.Info(string.Format("Agent Path {0}", (object) str));
      string path = Path.Combine(Path.GetTempPath(), "BstBatFile.bat");
      Logger.Info(string.Format("Temp File Path {0}", (object) path));
      if (!System.IO.File.Exists(path))
      {
        using (FileStream fileStream = System.IO.File.Create(path))
          fileStream.Close();
      }
      Logger.Info(string.Format("Temp File {0} Created", (object) path));
      using (StreamWriter streamWriter = new StreamWriter(path))
      {
        streamWriter.WriteLine("ping 192.0.2.2 -n 1 -w 100000 > nul");
        streamWriter.WriteLine("call \"" + str + "\"");
      }
      Logger.Info(string.Format("Temp File {0} Data Written", (object) path));
      using (Process process = new Process())
      {
        process.StartInfo.FileName = "cmd.exe";
        process.StartInfo.Arguments = "/c \"" + path + "\"";
        Logger.Info(string.Format("Calling {0} {1}", (object) process.StartInfo.FileName, (object) process.StartInfo.Arguments));
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;
        process.Start();
      }
      Logger.Info("Bat File Called");
      HTTPUtils.WriteSuccessJson(res, "");
      Environment.Exit(0);
    }

    public static void SystrayVisibility(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        if (string.Compare(HTTPUtils.ParseRequest(req).Data["visible"].Trim(), "true", StringComparison.InvariantCultureIgnoreCase) == 0)
          SysTray.SetTrayIconVisibility(true);
        else
          SysTray.SetTrayIconVisibility(false);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in Server SystrayVisibility");
        Logger.Error(ex.ToString());
      }
    }

    public static void TopActivityInfo(HttpListenerRequest req, HttpListenerResponse res)
    {
      RequestData request = HTTPUtils.ParseRequest(req);
      string requestVmName = request.RequestVmName;
      try
      {
        Logger.Info("Data");
        foreach (string allKey in request.Data.AllKeys)
          Logger.Info("Key: {0}, Value: {1}", (object) allKey, (object) request.Data[allKey]);
        string index = request.Data["packageName"];
        string activity = request.Data["activityName"];
        string callingPackage = request.Data["callingPackage"];
        Logger.Info("packageName = {0}, activityName = {1}", (object) index, (object) activity);
        TimelineStatsSender.HandleTopActivityInfo(request);
        HTTPHandler.SendAppDataToFE(index, activity, callingPackage, requestVmName);
        HTTPUtils.WriteSuccessJson(res, "");
        if (!Oem.IsOEMDmm || !HDAgent.sAppKeymapStateForDMM.ContainsKey(index))
          return;
        Thread.Sleep(1000);
        HTTPUtils.SendRequestToClient("setDMMKeymapping", new Dictionary<string, string>()
        {
          {
            "package",
            index
          },
          {
            "enablekeymap",
            HDAgent.sAppKeymapStateForDMM[index].ToString()
          }
        }, requestVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
        HDAgent.sAppKeymapStateForDMM.Remove(index);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in Server TopActivityInfo");
        Logger.Error(ex.ToString());
        HTTPUtils.WriteErrorArrayJson(res, ex.Message);
      }
    }

    private static string CheckForJsonp(string json, HttpListenerRequest req)
    {
      string str = HTTPUtils.ParseRequest(req, false).QueryString["callback"];
      return str == null ? json : string.Format("{0}({1});", (object) str, (object) json);
    }

    public static void RestartGameManager(HttpListenerRequest req, HttpListenerResponse res)
    {
      HTTPUtils.WriteSuccessArrayJson(res, "");
      string processName = Path.GetFileNameWithoutExtension(RegistryManager.Instance.PartnerExePath);
      new Thread((ThreadStart) (() =>
      {
        while (ProcessUtils.FindProcessByName(processName))
          Thread.Sleep(200);
        string vmName = "";
        string vmData = "";
        if (HTTPHandler.GetVmNameFromRequest(HTTPUtils.ParseRequest(req), ref vmName, ref vmData))
          return;
        ProcessUtils.GetProcessObject(RegistryManager.Instance.PartnerExePath, vmName, false).Start();
      }))
      {
        IsBackground = true
      }.Start();
    }

    public static void DoesInstanceExist(HttpListenerRequest req, HttpListenerResponse res)
    {
      string vmName = "";
      string vmData = "";
      try
      {
        if (!HTTPHandler.GetVmNameFromRequest(HTTPUtils.ParseRequest(req), ref vmName, ref vmData) || !HTTPHandler.CheckForInstance(vmName))
          HTTPUtils.WriteErrorJson(res, "");
        else
          HTTPUtils.WriteSuccessJson(res, "");
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in checking if instance exists, Ex: {0}", (object) ex.Message);
        HTTPUtils.WriteErrorJson(res, "");
      }
    }

    private static bool CheckForInstance(string vmName)
    {
      Logger.Info("CheckForInstance for vm: " + vmName);
      if (RegistryManager.Instance.VmList == null || RegistryManager.Instance.Guest.Count == 0 || !((IEnumerable<string>) RegistryManager.Instance.VmList).Contains<string>(vmName))
      {
        Logger.Warning("No instance found in VmList for vm: " + vmName);
        return false;
      }
      if (!Directory.Exists(Path.Combine(RegistryStrings.DataDir, vmName)))
      {
        Logger.Info("DataDir not exist for vm: " + vmName);
        return false;
      }
      if (RegistryManager.Instance.Guest.ContainsKey(vmName))
      {
        if (!string.IsNullOrEmpty(RegistryManager.Instance.Guest[vmName].BootParameters))
          return true;
        Logger.Warning("mVmKeys not found");
        return false;
      }
      Logger.Info("Instance {0} not found", (object) vmName);
      return false;
    }

    private static int SetRam(int ram, string vmName)
    {
      Logger.Info("In method SetRam");
      Convert.ToInt32(Profile.RAM);
      if (Utils.IsEngineRaw())
      {
        if (ram < 3072)
        {
          RegistryManager.Instance.Guest[vmName].Memory = ram;
          return 0;
        }
        Logger.Info("wrong value");
        return -2;
      }
      if (ram < 4096)
      {
        RegistryManager.Instance.Guest[vmName].Memory = ram;
        return 0;
      }
      Logger.Info("wrong value");
      return -2;
    }

    public static void Backup(HttpListenerRequest req, HttpListenerResponse res)
    {
      bool flag = false;
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        string str1 = request.Data["path"];
        bool boolean1 = Convert.ToBoolean(request.Data["silent"]);
        bool boolean2 = Convert.ToBoolean(request.Data["relaunch"]);
        flag = Convert.ToBoolean(request.Data["sendResponseImmediately"]);
        string str2 = "-backup";
        if (!string.IsNullOrEmpty(str1))
          str2 = str2 + " -path=" + str1;
        if (boolean1)
          str2 += " -s";
        Process process = new Process();
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.FileName = Path.Combine(RegistryStrings.InstallDir, "HD-DataManager.exe");
        process.StartInfo.Arguments = str2;
        process.StartInfo.UseShellExecute = true;
        process.StartInfo.Verb = "runas";
        process.Start();
        if (flag)
          HTTPUtils.WriteSuccessArrayJson(res, "");
        process.WaitForExit();
        int exitCode = process.ExitCode;
        if (exitCode == 0)
        {
          Logger.Info("DataManager exe exited with return code " + exitCode.ToString());
          if (!flag)
            HTTPUtils.WriteSuccessArrayJson(res, "");
        }
        else if (!flag)
          HTTPUtils.WriteErrorArrayJson(res, "Backup Failed");
        if (!boolean2)
          return;
        Utils.StartPartnerExe("Android");
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in creating Backup " + ex.ToString());
        if (flag)
          return;
        HTTPUtils.WriteErrorArrayJson(res, ex.Message);
      }
    }

    public static void Restore(HttpListenerRequest req, HttpListenerResponse res)
    {
      bool flag = false;
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        string str1 = request.Data["path"];
        bool boolean1 = Convert.ToBoolean(request.Data["silent"]);
        bool boolean2 = Convert.ToBoolean(request.Data["relaunch"]);
        flag = Convert.ToBoolean(request.Data["sendResponseImmediately"]);
        string str2 = "-restore";
        if (!string.IsNullOrEmpty(str1))
          str2 = str2 + " -path=" + str1;
        if (boolean1)
          str2 += " -s";
        Process process = new Process();
        process.StartInfo.UseShellExecute = true;
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.FileName = Path.Combine(RegistryStrings.InstallDir, "HD-DataManager.exe");
        process.StartInfo.Arguments = str2;
        process.StartInfo.Verb = "runas";
        process.Start();
        if (flag)
          HTTPUtils.WriteSuccessArrayJson(res, "");
        process.WaitForExit();
        int exitCode = process.ExitCode;
        if (exitCode == 0)
        {
          try
          {
            Logger.Info("DataManager exe exited with return code " + exitCode.ToString());
            RegistryManager.ClearRegistryMangerInstance();
            if (!flag)
              HTTPUtils.WriteSuccessArrayJson(res, "");
          }
          catch (Exception ex)
          {
            Logger.Error("Error in sending response " + ex.ToString());
          }
        }
        else if (!flag)
          HTTPUtils.WriteErrorArrayJson(res, "Failed restore");
        if (!boolean2)
          return;
        Utils.StartPartnerExe("Android");
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in restoring " + ex.ToString());
        if (flag)
          return;
        HTTPUtils.WriteErrorArrayJson(res, ex.Message);
      }
    }

    public static void AppJsonUpdatedForVideo(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        string vmName = request.RequestVmName;
        string str = request.Data["packageName"];
        bool flag = bool.Parse(request.Data["videoPresent"]);
        JsonParser jsonParser = new JsonParser(vmName);
        List<AppInfo> list = ((IEnumerable<AppInfo>) jsonParser.GetAppList()).ToList<AppInfo>();
        foreach (AppInfo appInfo in list)
        {
          if (appInfo.Package.Equals(str))
            appInfo.VideoPresent = flag;
        }
        lock (HTTPHandler.s_sync)
          jsonParser.WriteJson(list.ToArray());
        new Thread((ThreadStart) (() =>
        {
          try
          {
            HTTPUtils.SendRequestToClient("appJsonChanged", (Dictionary<string, string>) null, vmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
          }
          catch (Exception ex)
          {
            Logger.Error("Exception while sending appsync update to client: " + ex.ToString());
          }
        }))
        {
          IsBackground = true
        }.Start();
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in httphandler AppJsonUpdatedForVideo: " + ex.ToString());
      }
    }

    public static void InstanceStopped(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        string requestVmName = request.RequestVmName;
        string oem = request.Oem;
        if (Utils.sIsGuestBooted.ContainsKey(requestVmName + "_" + oem))
          Utils.sIsGuestBooted[requestVmName + "_" + oem] = false;
        if (Utils.sIsSharedFolderMounted.ContainsKey(requestVmName))
          Utils.sIsSharedFolderMounted[requestVmName] = false;
        if (!Utils.sIsGuestReady.ContainsKey(requestVmName))
          return;
        Utils.sIsGuestReady[requestVmName] = false;
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in httphandler InstanceStopped: " + ex.ToString());
      }
    }

    private static bool CheckForInstanceForDeleteVM(
      HttpListenerRequest req,
      HttpListenerResponse res,
      string vmName)
    {
      Logger.Info("In CheckForInstanceForDeleteVM for vm: " + vmName);
      bool flag1 = true;
      bool flag2 = true;
      bool flag3 = true;
      if (RegistryManager.Instance.VmList == null || RegistryManager.Instance.Guest.Count == 0 || !((IEnumerable<string>) RegistryManager.Instance.VmList).Contains<string>(vmName))
      {
        Logger.Warning("No instance found in VmList for vm: " + vmName);
        flag1 = false;
      }
      if (!Directory.Exists(Path.Combine(RegistryStrings.DataDir, vmName)))
      {
        Logger.Info("DataDir not exist for vm: " + vmName);
        flag2 = false;
      }
      if (RegistryManager.Instance.Guest.ContainsKey(vmName))
      {
        if (string.IsNullOrEmpty(RegistryManager.Instance.Guest[vmName].BootParameters))
        {
          Logger.Info("mVmKeys not found");
          flag3 = false;
        }
      }
      else
      {
        Logger.Info("mVmKeys not found");
        flag3 = false;
      }
      return flag1 | flag2 | flag3;
    }

    public static void TestCloudAnnouncement(HttpListenerRequest req, HttpListenerResponse res)
    {
      RequestData request = HTTPUtils.ParseRequest(req);
      try
      {
        NameValueCollection data = request.Data;
        string vmName = "Android";
        if (data["vmName"] != null)
          vmName = data["vmName"];
        CloudAnnouncement.ShowAnnouncementResponse(data["response"], vmName);
      }
      catch (Exception ex)
      {
        HTTPUtils.Write(HTTPHandler.CheckForJsonp(ex.ToString(), req), res);
      }
    }

    public static void ShowClientNotification(HttpListenerRequest req, HttpListenerResponse res)
    {
      RequestData request = HTTPUtils.ParseRequest(req);
      try
      {
        NotificationWindow.Instance.ShowSimplePopupForClient(request.Data["title"], request.Data["description"]);
      }
      catch (Exception ex)
      {
        HTTPUtils.Write(HTTPHandler.CheckForJsonp(ex.ToString(), req), res);
      }
    }

    internal enum DMMRunAppErrorCodes
    {
      SUCCESS = 0,
      PACKAGE_NAME_EMPTY = 268435457, // 0x10000001
      ACTIVITY_NAME_EMPTY = 268435458, // 0x10000002
      PACKAGE_NOT_INSTALLED = 268435459, // 0x10000003
      ENGINE_BOOT_FAILED = 268435460, // 0x10000004
      UNKNOWN_ERROR = 268435461, // 0x10000005
    }
  }
}
