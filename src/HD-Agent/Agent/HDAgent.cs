// Decompiled with JetBrains decompiler
// Type: BlueStacks.Agent.HDAgent
// Assembly: HD-Agent, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 06DAED18-1D79-40C2-83F8-3A28B5222574
// Assembly location: C:\Program Files\BlueStacks\HD-Agent.exe

using BlueStacks.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace BlueStacks.Agent
{
  public class HDAgent : ApplicationContext
  {
    private static Dictionary<string, int> s_InstalledPackages = (Dictionary<string, int>) null;
    public static int s_AgentPort = 2861;
    public static string s_RootDir = Path.Combine(RegistryStrings.BstUserDataDir, "www");
    private static Dictionary<string, string> sPowerValues = new Dictionary<string, string>();
    public static List<string> sBootFailed = new List<string>();
    public static Dictionary<string, ApkDownloadInstallStatus> sApkDownloadInstallStatusList = new Dictionary<string, ApkDownloadInstallStatus>();
    public static Dictionary<string, bool> sApkUpgradingList = new Dictionary<string, bool>();
    public static Dictionary<string, bool> sAppKeymapStateForDMM = new Dictionary<string, bool>();
    public static Dictionary<string, string> sVersionNameOfInstalledAppsDict = (Dictionary<string, string>) null;
    public static ShortcutConfig sShortcutConfig = (ShortcutConfig) null;
    public static List<string> sApkCrashLogsUploadBlackListedApps = new List<string>()
    {
      "com.bluestacks",
      "com.google",
      "com.android",
      "com.uncube"
    };
    private static Mutex s_HDAgentLock;
    public static string s_InstallDir;
    public static ClipboardMgr clipboardClient;
    public static Dictionary<string, string[]> sOemWindowMapper;
    private static ManagementEventWatcher sManagementEventWatcher;

    public static void InitPowerEvents()
    {
      try
      {
        WqlEventQuery wqlEventQuery1 = new WqlEventQuery();
        ManagementScope scope = new ManagementScope("root\\CIMV2");
        wqlEventQuery1.EventClassName = "Win32_PowerManagementEvent";
        WqlEventQuery wqlEventQuery2 = wqlEventQuery1;
        HDAgent.sManagementEventWatcher = new ManagementEventWatcher(scope, (EventQuery) wqlEventQuery2);
        HDAgent.sManagementEventWatcher.EventArrived += new EventArrivedEventHandler(HDAgent.PowerEventArrive);
        HDAgent.sManagementEventWatcher.Start();
      }
      catch (Exception ex)
      {
        Logger.Error("An error occured while capturing power events...Err : " + ex.ToString());
      }
    }

    private static void PowerEventArrive(object sender, EventArrivedEventArgs e)
    {
      try
      {
        foreach (PropertyData property in e.NewEvent.Properties)
        {
          if (property != null && property.Value != null && string.Compare(HDAgent.sPowerValues.ContainsKey(property.Value.ToString()) ? HDAgent.sPowerValues[property.Value.ToString()] : property.Value.ToString(), "Resume from Suspend", true) == 0)
          {
            long num = (DateTime.UtcNow.Ticks - 621355968000000000L) / 10000L;
            string cmd = string.Format("settime {0}", (object) num);
            Logger.Info("Number of ticks in milliseconds since epoch : " + num.ToString());
            foreach (string vm in RegistryManager.Instance.VmList)
            {
              if (Utils.IsGuestBooted(vm, "bgp"))
              {
                Logger.Info("Response from bstcmdprocessor for time correction after sleep/hibernate : " + VmCmdHandler.RunCommand(cmd, vm, "bgp"));
                if (Oem.IsOEMDmm)
                  Utils.SetTimeZoneInGuest(vm);
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Error("An error ocured while setting time....Err : " + ex.ToString());
      }
    }

    [STAThread]
    private static void Main(string[] args)
    {
      Logger.InitLog((string) null, "Agent", true);
      HDAgent.InitExceptionHandlers();
      ProcessUtils.LogProcessContextDetails();
      if (ProcessUtils.CheckAlreadyRunningAndTakeLock("Global\\BlueStacks_HDAgent_Lockbgp", out HDAgent.s_HDAgentLock))
        HDAgent.HandleAlreadyRunning();
      NotificationWindow.Init();
      NotificationPopup.SettingsImageClickedHandle(new EventHandler(HTTPHandler.SettingsImageMouseUp), (object) null);
      HDAgent.sPowerValues.Add("4", "Entering Suspend");
      HDAgent.sPowerValues.Add("7", "Resume from Suspend");
      HDAgent.sPowerValues.Add("10", "Power Status Change");
      HDAgent.sPowerValues.Add("18", "Resume Automatic");
      HDAgent.InitPowerEvents();
      MemoryManager.TrimMemory(true);
      HDAgent.s_InstallDir = RegistryStrings.InstallDir;
      Directory.SetCurrentDirectory(HDAgent.s_InstallDir);
      Logger.Info("HDAgent: CurrentDirectory: {0}", (object) Directory.GetCurrentDirectory());
      LocaleStrings.InitLocalization((string) null, "Android", false);
      ServicePointManager.DefaultConnectionLimit = 10;
      ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(HDAgent.ValidateRemoteCertificate);
      Application.EnableVisualStyles();
      new Thread(new ThreadStart(HDAgent.SetupHTTPServer))
      {
        IsBackground = true
      }.Start();
      try
      {
        EventLog eventLog1 = new EventLog("Application");
        eventLog1.EntryWritten += new EntryWrittenEventHandler(HDAgent.EventLogWritten);
        eventLog1.EnableRaisingEvents = true;
        EventLog eventLog2 = new EventLog("System");
        eventLog2.EntryWritten += new EntryWrittenEventHandler(HDAgent.EventLogWritten);
        eventLog2.EnableRaisingEvents = true;
      }
      catch (Exception ex)
      {
        Logger.Warning("Got excecption while hooking to event log ex:{0}", (object) ex.ToString());
      }
      Stats.SendMiscellaneousStatsAsyncForDMM(Stats.DMMEvent.agent_launched.ToString(), RegistryManager.Instance.AgentServerPort.ToString(), (string) null, (string) null, (string) null, "Android", 0);
      Application.Run((ApplicationContext) new HDAgent());
      Logger.Info("Exiting HDAgent PID {0}", (object) Process.GetCurrentProcess().Id);
    }

    private static bool ValidateRemoteCertificate(
      object sender,
      X509Certificate cert,
      X509Chain chain,
      SslPolicyErrors policyErrors)
    {
      return true;
    }

    private static void HandleAlreadyRunning()
    {
      Logger.Info("Agent already running");
      if (!ProcessUtils.CheckAlreadyRunningAndTakeLock("Global\\BlueStacks_HDAgent_Lockbgp", out HDAgent.s_HDAgentLock))
        return;
      Dictionary<string, string> data = new Dictionary<string, string>();
      data.Add("visible", "true");
      string vmName = "";
      HTTPUtils.SendRequestToAgent("sysTrayVisibility", data, vmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp", true);
      Environment.Exit(1);
    }

    private static void EventLogWritten(object source, EntryWrittenEventArgs e)
    {
      if (e.Entry.EntryType != EventLogEntryType.Error)
        return;
      Logger.Debug("EventLog written");
      string message = e.Entry.Message;
      Match match = new Regex("(HD-.+).exe").Match(message);
      if (!match.Success)
        return;
      string str = match.Groups[1].Value;
      Logger.Info("Event log for {0} written", (object) str);
      Logger.Info("Message:\n{0}", (object) message);
      try
      {
        if (!HDAgent.sOemWindowMapper.ContainsKey(Oem.Instance.OEM) || str.Equals("HD-Agent", StringComparison.CurrentCultureIgnoreCase))
          return;
        HTTPHandler.StartLogCollection("EXE_CRASHED", "binName");
        HDAgent.NotifyExeCrashToParentWindow(HDAgent.sOemWindowMapper[Oem.Instance.OEM][0], HDAgent.sOemWindowMapper[Oem.Instance.OEM][1]);
      }
      catch (Exception ex)
      {
        Logger.Error(string.Format("Error Occured, Err: {0}", (object) ex.ToString()));
      }
    }

    private static void SetupHTTPServer()
    {
      Dictionary<string, HTTPServer.RequestHandler> routes = new Dictionary<string, HTTPServer.RequestHandler>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      routes.Add(string.Format("{0}/install-app", (object) Oem.Instance.DMMApiPrefix), new HTTPServer.RequestHandler(HTTPHandler.InstallApp));
      routes.Add(string.Format("{0}/get-progress", (object) Oem.Instance.DMMApiPrefix), new HTTPServer.RequestHandler(HTTPHandler.GetDownloadInstallProgress));
      routes.Add(string.Format("{0}/{1}", (object) Oem.Instance.DMMApiPrefix, (object) "maintenanceWarning"), new HTTPServer.RequestHandler(HTTPHandler.MaintenanceWarning));
      routes.Add("/guestBootFailed", new HTTPServer.RequestHandler(HTTPHandler.GuestBootFailedHandler));
      routes.Add("/launchDefaultWebApp", new HTTPServer.RequestHandler(HTTPHandler.LaunchDefaultWebApp));
      routes.Add("/installed", new HTTPServer.RequestHandler(HTTPHandler.ApkInstalled));
      routes.Add("/uninstalled", new HTTPServer.RequestHandler(HTTPHandler.AppUninstalled));
      routes.Add("/getAppList", new HTTPServer.RequestHandler(HTTPHandler.GetAppList));
      routes.Add("/install", new HTTPServer.RequestHandler(HTTPHandler.ApkInstall));
      routes.Add("/uninstall", new HTTPServer.RequestHandler(HTTPHandler.AppUninstall));
      routes.Add("/runApp", new HTTPServer.RequestHandler(HTTPHandler.RunApp));
      routes.Add("/setLocale", new HTTPServer.RequestHandler(HTTPHandler.SetLocale));
      routes.Add("/ping", new HTTPServer.RequestHandler(HTTPHandler.Ping));
      routes.Add("/appCrashedInfo", new HTTPServer.RequestHandler(HTTPHandler.AppCrashedInfo));
      routes.Add("/getUserData", new HTTPServer.RequestHandler(HTTPHandler.GetUserData));
      routes.Add("/showNotification", new HTTPServer.RequestHandler(HTTPHandler.ShowNotification));
      routes.Add("/appDownloadStatus", new HTTPServer.RequestHandler(HTTPHandler.AppDownloadStatus));
      routes.Add("/showFeNotification", new HTTPServer.RequestHandler(HTTPHandler.ShowFeNotification));
      routes.Add("/bindmount", new HTTPServer.RequestHandler(HTTPHandler.BindMount));
      routes.Add("/unbindmount", new HTTPServer.RequestHandler(HTTPHandler.UnbindMount));
      routes.Add("/quitFrontend", new HTTPServer.RequestHandler(HTTPHandler.QuitFrontend));
      routes.Add("/getAppImage", new HTTPServer.RequestHandler(HTTPHandler.GetAppImage));
      routes.Add("/showTrayNotification", new HTTPServer.RequestHandler(HTTPHandler.ShowSysTrayNotification));
      routes.Add("/restart", new HTTPServer.RequestHandler(HTTPHandler.Restart));
      routes.Add("/notification", new HTTPServer.RequestHandler(HTTPHandler.NotificationHandler));
      routes.Add("/clipboard", new HTTPServer.RequestHandler(HTTPHandler.SetClipboardData));
      routes.Add("/isAppInstalled", new HTTPServer.RequestHandler(HTTPHandler.IsAppInstalled));
      routes.Add("/topActivityInfo", new HTTPServer.RequestHandler(HTTPHandler.TopActivityInfo));
      routes.Add("/FrontendStatusUpdate", new HTTPServer.RequestHandler(HTTPHandler.FrontendStatusUpdate));
      routes.Add("/GuestStatusUpdate", new HTTPServer.RequestHandler(HTTPHandler.GuestStatusUpdate));
      routes.Add("/sysTrayVisibility", new HTTPServer.RequestHandler(HTTPHandler.SystrayVisibility));
      routes.Add("/restartAgent", new HTTPServer.RequestHandler(HTTPHandler.RestartAgent));
      routes.Add("/showTileInterface", new HTTPServer.RequestHandler(HTTPHandler.ShowTileInterface));
      routes.Add("/setNewLocation", new HTTPServer.RequestHandler(HTTPHandler.SetNewLocation));
      routes.Add("/adEvents", new HTTPServer.RequestHandler(HTTPHandler.HandleAdEvents));
      routes.Add("/exitAgent", new HTTPServer.RequestHandler(HTTPHandler.ExitAgent));
      routes.Add("/stopApp", new HTTPServer.RequestHandler(HTTPHandler.StopAppHandler));
      routes.Add("/releaseApkInstallThread", new HTTPServer.RequestHandler(HTTPHandler.ReleaseApkInstallThread));
      routes.Add("/clearAppData", new HTTPServer.RequestHandler(HTTPHandler.ClearAppDataHandler));
      routes.Add("/restartGameManager", new HTTPServer.RequestHandler(HTTPHandler.RestartGameManager));
      routes.Add("/postHttpUrl", new HTTPServer.RequestHandler(HTTPHandler.PostHttpUrl));
      routes.Add("/instanceExist", new HTTPServer.RequestHandler(HTTPHandler.DoesInstanceExist));
      routes.Add("/queryInstances", new HTTPServer.RequestHandler(HTTPHandler.QueryInstance));
      routes.Add("/createInstance", new HTTPServer.RequestHandler(HTTPHandler.CreateInstance));
      routes.Add("/deleteInstance", new HTTPServer.RequestHandler(HTTPHandler.DeleteInstance));
      routes.Add("/resetSharedFolders", new HTTPServer.RequestHandler(HTTPHandler.ResetSharedFolders));
      routes.Add("/startInstance", new HTTPServer.RequestHandler(HTTPHandler.StartInstance));
      routes.Add("/getRunningInstances", new HTTPServer.RequestHandler(HTTPHandler.GetRunningInstances));
      routes.Add("/stopInstance", new HTTPServer.RequestHandler(HTTPHandler.StopInstance));
      routes.Add("/setVmConfig", new HTTPServer.RequestHandler(HTTPHandler.SetVmConfig));
      routes.Add("/isMultiInstanceSupported", new HTTPServer.RequestHandler(HTTPHandler.IsMultiInstanceSupported));
      routes.Add("/setCpu", new HTTPServer.RequestHandler(HTTPHandler.SetCpu));
      routes.Add("/setDpi", new HTTPServer.RequestHandler(HTTPHandler.SetDpi));
      routes.Add("/setRam", new HTTPServer.RequestHandler(HTTPHandler.SetRam));
      routes.Add("/setResolution", new HTTPServer.RequestHandler(HTTPHandler.SetResolution));
      routes.Add("/getGuid", new HTTPServer.RequestHandler(HTTPHandler.GetGuid));
      routes.Add("/backup", new HTTPServer.RequestHandler(HTTPHandler.Backup));
      routes.Add("/restore", new HTTPServer.RequestHandler(HTTPHandler.Restore));
      routes.Add("/appJsonUpdatedForVideo", new HTTPServer.RequestHandler(HTTPHandler.AppJsonUpdatedForVideo));
      routes.Add("/deviceProfileUpdated", new HTTPServer.RequestHandler(HTTPHandler.DeviceProfileUpdated));
      routes.Add("/instanceStopped", new HTTPServer.RequestHandler(HTTPHandler.InstanceStopped));
      routes.Add("/getInstanceStatus", new HTTPServer.RequestHandler(HTTPHandler.GetInstanceStatus));
      routes.Add("/isEngineReady", new HTTPServer.RequestHandler(HTTPHandler.IsEngineReady));
      routes.Add("/copyToAndroid", new HTTPServer.RequestHandler(HTTPHandler.CopyToAndroid));
      routes.Add("/copyToWindows", new HTTPServer.RequestHandler(HTTPHandler.CopyToWindows));
      routes.Add("/setCurrentVolume", new HTTPServer.RequestHandler(HTTPHandler.SetCurrentVolume));
      routes.Add("/downloadInstalledAppsCfg", new HTTPServer.RequestHandler(HTTPHandler.DownloadInstalledAppsCfg));
      routes.Add("/setVMDisplayName", new HTTPServer.RequestHandler(HTTPHandler.SetVMDisplayName));
      routes.Add("/sortWindows", new HTTPServer.RequestHandler(HTTPHandler.SortWindows));
      routes.Add("/enableDebugLogs", new HTTPServer.RequestHandler(HTTPHandler.EnableDebugLogs));
      routes.Add("/logAppClick", new HTTPServer.RequestHandler(HTTPHandler.LogAndroidClickEvent));
      routes.Add("/setNCPlayerCharacterName", new HTTPServer.RequestHandler(HTTPHandler.SetNCPlayerCharacterName));
      routes.Add("/launchPlay", new HTTPServer.RequestHandler(HTTPHandler.LaunchPlay));
      routes.Add("/removeAccount", new HTTPServer.RequestHandler(HTTPHandler.RemoveAccount));
      routes.Add("/setDeviceProfile", new HTTPServer.RequestHandler(HTTPHandler.SetDeviceProfile));
      routes.Add("/screenLock", new HTTPServer.RequestHandler(HTTPHandler.ScreenLock));
      routes.Add("/makeDir", new HTTPServer.RequestHandler(HTTPHandler.MakeDir));
      routes.Add("/getHeightWidth", new HTTPServer.RequestHandler(HTTPHandler.GetHeightWidth));
      routes.Add("/setStreamingStatus", new HTTPServer.RequestHandler(HTTPHandler.SetStreamingStatus));
      routes.Add("/getShortcut", new HTTPServer.RequestHandler(HTTPHandler.GetShortcut));
      routes.Add("/setShortcut", new HTTPServer.RequestHandler(HTTPHandler.SetShortcut));
      routes.Add("/sendEngineTimelineStats", new HTTPServer.RequestHandler(HTTPHandler.SendEngineTimelineStats));
      routes.Add("/grmAppLaunch", new HTTPServer.RequestHandler(HTTPHandler.GrmAppLaunch));
      routes.Add("/reinitlocalization", new HTTPServer.RequestHandler(HTTPHandler.ReInitLocalization));
      routes.Add("/testCloudAnnouncement", new HTTPServer.RequestHandler(HTTPHandler.TestCloudAnnouncement));
      routes.Add("/overrideDesktopNotificationSettings", new HTTPServer.RequestHandler(HTTPHandler.OverrideDesktopNotificationSettings));
      routes.Add("/notificationStatsOnClosing", new HTTPServer.RequestHandler(HTTPHandler.NotificationStatsOnClosing));
      routes.Add("/configFileChanged", new HTTPServer.RequestHandler(HTTPHandler.ConfigFileChanged));
      routes.Add("/getCallbackStatus", new HTTPServer.RequestHandler(HTTPHandler.GetCallbackStatus));
      routes.Add("/showClientNotification", new HTTPServer.RequestHandler(HTTPHandler.ShowClientNotification));
      int startingPort = 2861;
      HTTPServer httpServer = HTTPUtils.SetupServer(startingPort, startingPort + 10, routes, HDAgent.s_RootDir);
      HDAgent.s_AgentPort = httpServer.Port;
      HDAgent.SetAgentPortInBootParams();
      RegistryManager.Instance.AgentServerPort = httpServer.Port;
      if (Oem.Instance.IsWriteRegistryInfoInFile)
        Utils.WriteAgentPortInFile(httpServer.Port);
      httpServer.Run();
    }

    private static void SetAgentPortInBootParams()
    {
      try
      {
        foreach (string index in ((IEnumerable<string>) RegistryManager.Instance.VmList).ToList<string>())
        {
          string bootParameters = RegistryManager.Instance.Guest[index].BootParameters;
          string[] strArray = bootParameters.Split(' ');
          string str1 = "";
          string str2 = string.Format("10.0.2.2:{0}", (object) HDAgent.s_AgentPort);
          if (bootParameters.IndexOf("WINDOWSAGENT") == -1)
          {
            str1 = bootParameters + " WINDOWSAGENT=" + str2;
          }
          else
          {
            foreach (string str3 in strArray)
            {
              if (str3.IndexOf("WINDOWSAGENT") != -1)
              {
                if (!string.IsNullOrEmpty(str1))
                  str1 += " ";
                str1 = str1 + "WINDOWSAGENT=" + str2;
              }
              else
              {
                if (!string.IsNullOrEmpty(str1))
                  str1 += " ";
                str1 += str3;
              }
            }
          }
          RegistryManager.Instance.Guest[index].BootParameters = str1;
        }
      }
      catch (Exception ex)
      {
        Logger.Error(string.Format("Error Occured, Err: {0}", (object) ex.ToString()));
      }
    }

    private static void SendFqdn(string vmName, int retries = 120)
    {
      Logger.Info("Starting sending fqdn to " + vmName + " for agent port " + HDAgent.s_AgentPort.ToString());
      while (retries > 0 && VmCmdHandler.FqdnSend(HDAgent.s_AgentPort, "Agent", vmName) == null)
      {
        --retries;
        Thread.Sleep(2000);
      }
    }

    public HDAgent()
    {
      string vmName = "Android";
      SysTray.Init(vmName);
      Utils.AddMessagingSupport(out HDAgent.sOemWindowMapper);
      HDAgent.clipboardClient = new ClipboardMgr();
      HDAgent.clipboardClient.Show();
      HDAgent.CheckAnnouncementAsync(vmName);
      TimelineStatsSender.Init(vmName);
    }

    private static void CheckAnnouncementAsync(string vmName)
    {
      new Thread((ThreadStart) (() =>
      {
        Thread.Sleep(180000);
label_1:
        try
        {
          if (!CloudAnnouncement.ShowAnnouncement(vmName))
            Logger.Info("No new announcement to show.");
        }
        catch (Exception ex)
        {
          Logger.Debug("Failed to show announcement. err: " + ex.ToString());
        }
        RegistryManager.Instance.AnnouncementTime = DateTime.Now;
        while (true)
        {
          if (!Utils.HasOneDayPassed(RegistryManager.Instance.AnnouncementTime))
            Thread.Sleep(300000);
          else
            goto label_1;
        }
      }))
      {
        IsBackground = true
      }.Start();
    }

    public static bool DoRunCmd(string request, string vmName)
    {
      bool flag = false;
      if (VmCmdHandler.RunCommand(request, vmName, "bgp") == "ok")
      {
        flag = true;
        if (request.Contains("mpi.v23"))
        {
          Logger.Info("starting amidebug. not sending message to frontend.");
          return flag;
        }
        IntPtr window = InteropWindow.FindWindow((string) null, BlueStacks.Common.Strings.AppTitle);
        if (window != IntPtr.Zero)
        {
          Logger.Info("Sending WM_USER_SHOW_WINDOW to Frontend Handle {0}", (object) window);
          InteropWindow.SendMessage(window, 1025U, IntPtr.Zero, IntPtr.Zero);
        }
      }
      string appName = "";
      string packageName = "";
      string activityName = "";
      string imageName = "";
      string appstore = "";
      if (request.StartsWith("runex"))
      {
        string str = new Regex("^runex\\s+").Replace(request, "");
        packageName = str.Substring(0, str.IndexOf('/'));
        if (!new JsonParser(vmName).GetAppInfoFromPackageName(packageName, out appName, out imageName, out activityName, out appstore))
        {
          Logger.Error("Failed to get App info for: {0}. Not adding in launcher dock.", (object) packageName);
          return flag;
        }
      }
      HDAgent.GetVersionFromPackage(packageName, vmName);
      string str1 = RegistryStrings.GadgetDir + imageName;
      return flag;
    }

    public static void LaunchDataManager(string argument)
    {
      string installDir = RegistryStrings.InstallDir;
      Process process = new Process();
      process.StartInfo.FileName = Path.Combine(installDir, "HD-DataManager.exe");
      process.StartInfo.Arguments = argument;
      process.Start();
      process.WaitForExit();
    }

    public static string GetVersionFromPackage(string packageName, string vmName)
    {
      string str = "";
      if (HDAgent.s_InstalledPackages == null)
        HDAgent.s_InstalledPackages = new Dictionary<string, int>();
      if (!HDAgent.s_InstalledPackages.ContainsKey(packageName))
        HDAgent.GetInstalledPackages(vmName);
      int num;
      if (HDAgent.s_InstalledPackages.TryGetValue(packageName, out num))
        str = Convert.ToString(num);
      return str;
    }

    public static string GetVersionNameFromPackage(string packageName, string vmName)
    {
      string str1 = "";
      if (HDAgent.sVersionNameOfInstalledAppsDict == null)
        HDAgent.sVersionNameOfInstalledAppsDict = new Dictionary<string, string>();
      if (!HDAgent.sVersionNameOfInstalledAppsDict.ContainsKey(packageName))
        HDAgent.InitAppVersionDictionary(vmName);
      string str2;
      if (HDAgent.sVersionNameOfInstalledAppsDict.TryGetValue(packageName, out str2))
        str1 = str2;
      return str1;
    }

    private static void InitAppVersionDictionary(string vmName)
    {
      try
      {
        JObject jobject1 = JObject.Parse(HTTPUtils.SendRequestToGuest("installedPackages", (Dictionary<string, string>) null, vmName, 0, (Dictionary<string, string>) null, false, 30, 2000, "bgp"));
        string str1 = jobject1["result"].ToString().Trim();
        if (str1 != "ok")
        {
          Logger.Error("result: {0}", (object) str1);
        }
        else
        {
          string str2 = jobject1["installed_packages"].ToString();
          Logger.Debug(str2);
          JArray jarray = JArray.Parse(str2);
          for (int index = 0; index < jarray.Count; ++index)
          {
            JObject jobject2 = JObject.Parse(jarray[index].ToString());
            string key = jobject2["package"].ToString().Trim();
            string str3 = jobject2["versionName"].ToString().Trim();
            HDAgent.sVersionNameOfInstalledAppsDict.Add(key, str3);
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in GetVersionNameOfInstalledApps:" + ex.ToString());
      }
    }

    private static void GetInstalledPackages(string vmName)
    {
      Logger.Info("In GetInstalledPackages");
      JObject jobject1 = JObject.Parse(HTTPUtils.SendRequestToGuest("installedPackages", (Dictionary<string, string>) null, vmName, 0, (Dictionary<string, string>) null, false, 30, 2000, "bgp"));
      string str1 = jobject1["result"].ToString().Trim();
      if (str1 != "ok")
      {
        Logger.Error("result: {0}", (object) str1);
      }
      else
      {
        string str2 = jobject1["installed_packages"].ToString();
        Logger.Debug(str2);
        JArray jarray = JArray.Parse(str2);
        for (int index = 0; index < jarray.Count; ++index)
        {
          JObject jobject2 = JObject.Parse(jarray[index].ToString());
          string key = jobject2["package"].ToString().Trim();
          int num = jobject2["version"].ToObject<int>();
          try
          {
            HDAgent.s_InstalledPackages.Add(key, num);
          }
          catch (Exception ex)
          {
          }
        }
      }
    }

    private static void InitExceptionHandlers()
    {
      Application.ThreadException += (ThreadExceptionEventHandler) ((obj, evt) =>
      {
        Logger.Error("HDAgent: Unhandled Exception:");
        Logger.Error(evt.Exception.ToString());
        Environment.Exit(1);
      });
      Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
      AppDomain.CurrentDomain.UnhandledException += (UnhandledExceptionEventHandler) ((obj, evt) =>
      {
        Logger.Error("HDAgent: Unhandled Exception:");
        Logger.Error(evt.ExceptionObject.ToString());
        Environment.Exit(1);
      });
    }

    public static void NotifyAppCrashToParentWindow(string className, string windowName, int vmId)
    {
      Logger.Info("Sending WM_USER_APP_CRASHED message to class = {0}, window = {1}", (object) className, (object) windowName);
      try
      {
        IntPtr window = InteropWindow.FindWindow(className, windowName);
        if (window == IntPtr.Zero)
        {
          Logger.Info("Unable to find window : {0}", (object) className);
        }
        else
        {
          uint num = (uint) vmId;
          InteropWindow.SendMessage(window, 1034U, IntPtr.Zero, (IntPtr) (long) num);
        }
      }
      catch (Exception ex)
      {
        Logger.Error(string.Format("Error Occured, Err: {0}", (object) ex.ToString()));
      }
    }

    public static void NotifyExeCrashToParentWindow(string className, string windowName)
    {
      Logger.Info("Sending WM_USER_EXE_CRASHED message to class = {0}, window = {1}", (object) className, (object) windowName);
      try
      {
        IntPtr window = InteropWindow.FindWindow(className, windowName);
        if (window == IntPtr.Zero)
          Logger.Info("Unable to find window : {0}", (object) className);
        else
          InteropWindow.SendMessage(window, 1035U, IntPtr.Zero, IntPtr.Zero);
      }
      catch (Exception ex)
      {
        Logger.Error(string.Format("Error Occured, Err: {0}", (object) ex.ToString()));
      }
    }
  }
}
