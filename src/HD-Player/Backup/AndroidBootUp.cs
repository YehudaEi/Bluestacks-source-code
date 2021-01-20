// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.AndroidBootUp
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using BlueStacks.Common;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace BlueStacks.Player
{
  internal static class AndroidBootUp
  {
    internal static bool isAndroidBooted = false;
    internal static bool isAndroidReady = false;
    private static object sLogFailureLogRegLock = new object();
    private static bool mFirstMonitorAttachAttempt = true;
    internal static bool forceVideoModeChange = false;
    internal static Monitor mMonitor = (Monitor) null;
    internal static Manager mManager = (Manager) null;
    private static object mSendingBootFailureLogsLock = new object();
    internal static bool sHasNotifiedClientForGuestBooted = false;
    private static readonly object sSendBootCompleteLockObject = new object();
    internal static CameraManager camManager;

    internal static void Start()
    {
      try
      {
        LayoutManager.InitOpengl();
        new Thread((ThreadStart) (() =>
        {
          try
          {
            AndroidBootUp.AttachMonitor();
          }
          catch (Exception ex)
          {
            Logger.Error("Exception in AndroidBootUp.Start. Err : ", (object) ex.ToString());
            AndroidBootUp.HandleBootError();
            throw ex;
          }
        }))
        {
          IsBackground = true
        }.Start();
      }
      catch (Exception ex)
      {
        Logger.Error("Error in Android bootup start: " + ex.ToString());
        AndroidBootUp.HandleBootError();
        throw ex;
      }
    }

    public static void GuestBootCompletedEvent(object sender, EventArgs e)
    {
      if (AndroidBootUp.sHasNotifiedClientForGuestBooted)
        return;
      lock (AndroidBootUp.sSendBootCompleteLockObject)
      {
        if (AndroidBootUp.sHasNotifiedClientForGuestBooted)
          return;
        try
        {
          Logger.Info("BOOT_STAGE: Sending boot completed event");
          AndroidBootUp.sHasNotifiedClientForGuestBooted = true;
          try
          {
            if (!Opt.Instance.sysPrep)
            {
              if (Oem.Instance.IsSendGameManagerRequest)
                HTTPUtils.SendRequestToClientAsync("guestBootCompleted", (Dictionary<string, string>) null, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
            }
          }
          catch (Exception ex)
          {
            Logger.Error("An exception in sending boot completed request to client: {0}", (object) ex.ToString());
          }
          if (LoadingScreen.mLoadingScreen != null)
            LoadingScreen.RemoveLoadingScreen();
          AndroidBootUp.isAndroidBooted = true;
          StateMachine.mForceShutdownDueTime = 60000;
          Stats.SendFrontendStatusUpdate("frontend-ready", MultiInstanceStrings.VmName);
          AndroidBootUp.SendSecurityMessageToAndroidOnBootFinish();
          if (AndroidBootUp.HideBootProgress())
          {
            AndroidBootUp.ShowConnectedView();
            AndroidBootUp.PerformDeferredSetup();
            AndroidBootUp.CheckVtxAndShowPopup();
          }
          UIHelper.RunOnUIThread((Control) VMWindow.Instance, (UIHelper.Action) (() =>
          {
            --VMWindow.Instance.Width;
            ++VMWindow.Instance.Width;
          }));
          Utils.SyncAppJson(MultiInstanceStrings.VmName);
        }
        catch (Exception ex)
        {
          Logger.Error("Exception on GuestBootCompletedEvent. Err : " + ex.ToString());
        }
      }
    }

    private static void PerformDeferredSetup()
    {
      if (Opt.Instance.sysPrep)
        return;
      Logger.Info("Console PerformDeferredSetup");
      Stats.SendBootStats("frontend", true, false, MultiInstanceStrings.VmName);
      VMWindow.Instance.SendOrientationToGuest();
      if (RegistryManager.Instance.DefaultGuest.ConfigSynced == 0)
      {
        Logger.Info("Config not synced. Syncing now.");
        ThreadPool.QueueUserWorkItem((WaitCallback) (stateInfo =>
        {
          VmCmdHandler.SyncConfig(InputMapper.GetKeyMappingParserVersion(), MultiInstanceStrings.VmName);
          VmCmdHandler.SetKeyboard(LayoutManager.IsDesktop(), MultiInstanceStrings.VmName);
        }));
      }
      else
      {
        string currentKeyboardLayout = Utils.GetCurrentKeyboardLayout();
        VMWindow.Instance.SetKeyboardLayout(currentKeyboardLayout);
        Logger.Info("Config already synced.");
      }
      ThreadPool.QueueUserWorkItem((WaitCallback) (stateInfo =>
      {
        Logger.Info("Started fqdnSender thread for agent");
        VmCmdHandler.FqdnSend(0, "Agent", MultiInstanceStrings.VmName);
        Logger.Info("fqdnSender thread exiting");
      }));
      ThreadPool.QueueUserWorkItem((WaitCallback) (stateInfo =>
      {
        Logger.Info("Started fqdnSender thread for frontend");
        VmCmdHandler.FqdnSend(0, "frontend", MultiInstanceStrings.VmName);
        Logger.Info("fqdnSender thread exiting");
      }));
      UIHelper.RunOnUIThread((Control) VMWindow.Instance, (UIHelper.Action) (() =>
      {
        try
        {
          string text = System.Windows.Clipboard.GetText(System.Windows.TextDataFormat.Text);
          Logger.Debug("sending clipboard data to android.." + text);
          HTTPUtils.SendRequestToGuestAsync("clipboard", new Dictionary<string, string>()
          {
            {
              "text",
              text
            }
          }, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
        }
        catch (Exception ex)
        {
          Logger.Error(" error in sending clipboard data to android.." + ex?.ToString());
        }
      }));
      ThreadPool.QueueUserWorkItem((WaitCallback) (stateInfo => VmCmdHandler.SetMachineType(LayoutManager.IsDesktop(), MultiInstanceStrings.VmName)));
      AndroidBootUp.GpsAttach();
      SensorDevice.Instance.Start(MultiInstanceStrings.VmName);
      AndroidBootUp.CameraAttach();
      AndroidBootUp.SendControllerEventInternal("controller_flush", (UIHelper.Action) (() =>
      {
        foreach (int key in VMWindow.Instance.mControllerMap.Keys)
        {
          SensorDevice.Instance.ControllerAttach(SensorDevice.Type.Accelerometer);
          AndroidBootUp.SendControllerEvent("attach", key, VMWindow.Instance.mControllerMap[key]);
        }
        VMWindow.Instance.mControllerMap.Clear();
      }));
      ThreadPool.QueueUserWorkItem((WaitCallback) (stateInfo =>
      {
        Logger.Info("Checking for Black Screen Error");
        AndroidBootUp.CheckBlackScreenAndRestartGMifOccurs();
      }));
    }

    private static void GpsAttach()
    {
      new Thread((ThreadStart) (() =>
      {
        Logger.Info(nameof (GpsAttach));
        try
        {
          GPSManager.Init();
        }
        catch (Exception ex)
        {
          Logger.Error("Exception in GpsAttach. Err : " + ex.ToString());
        }
      }))
      {
        IsBackground = true
      }.Start();
    }

    internal static void GpsDetach()
    {
      Logger.Info(nameof (GpsDetach));
      GPSManager.Shutdown();
    }

    private static void CameraAttach()
    {
      if (AndroidBootUp.camManager != null)
      {
        Logger.Info("cam Manager is already attached");
      }
      else
      {
        AndroidBootUp.camManager = new CameraManager();
        Logger.Info(nameof (CameraAttach));
        try
        {
          CameraManager.Monitor = AndroidBootUp.mMonitor;
          AndroidBootUp.camManager.InitCamera(new string[1]
          {
            MultiInstanceStrings.VmName
          });
        }
        catch (Exception ex)
        {
          Logger.Error("Exception in CameraAttach. Err : " + ex.ToString());
        }
      }
    }

    internal static void CameraDetach()
    {
      if (AndroidBootUp.camManager == null)
      {
        Logger.Info("Cannot detach camera, which is not yet attached");
      }
      else
      {
        Logger.Info(nameof (CameraDetach));
        AndroidBootUp.camManager.Shutdown();
        AndroidBootUp.camManager = (CameraManager) null;
      }
    }

    private static void CheckBlackScreenAndRestartGMifOccurs()
    {
      Logger.Info("In method CheckBlackScreenAndRestartGMifOccurs");
      int num = 0;
      while (VMWindow.Instance.CheckBlackScreen() && num < 300)
      {
        ++num;
        Thread.Sleep(1000);
      }
      if (num >= 300)
      {
        Logger.Info("Black Screen occurs for 5 mins");
        if (System.Windows.Forms.MessageBox.Show(LocaleStrings.GetLocalizedString("STRING_BLACKSCREEN_FORM", ""), LocaleStrings.GetLocalizedString("STRING_TROUBLESHOOTER", ""), MessageBoxButtons.OKCancel) == DialogResult.OK)
        {
          Logger.Info("User click Yes, Restartig GameManager");
          HTTPUtils.SendRequestToClient("restartFrontend", new Dictionary<string, string>()
          {
            {
              "vmname",
              MultiInstanceStrings.VmName
            }
          }, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
        }
        else
          Logger.Info("User clicked No");
      }
      else
      {
        Logger.Info("Frontend launched Successfully");
        Stats.SendHomeScreenDisplayedStats(MultiInstanceStrings.VmName);
      }
    }

    private static void SendControllerEvent(string name, int identity, string type)
    {
      AndroidBootUp.SendControllerEventInternal(string.Format("controller_{0} {1} {2}", (object) name, (object) identity, (object) type), (UIHelper.Action) null);
    }

    private static void SendControllerEventInternal(string cmd, UIHelper.Action continuation)
    {
      Logger.Info("Sending controller event " + cmd);
      VmCmdHandler.RunCommandAsync(cmd, continuation, (Control) VMWindow.Instance, MultiInstanceStrings.VmName);
    }

    private static void CheckVtxAndShowPopup()
    {
      Logger.Info("In CheckVtxAndShowPopup");
      int systemInfoStats2 = RegistryManager.Instance.SystemInfoStats2;
      string deviceCaps = RegistryManager.Instance.DeviceCaps;
      if (systemInfoStats2 == 1 && !deviceCaps.Equals(""))
      {
        Logger.Info("Sending DeviceCaps stats");
        Dictionary<string, string> data = new Dictionary<string, string>();
        Logger.Info("DeviceCaps: " + deviceCaps);
        data.Add("data", deviceCaps);
        data.Add("install_id", RegistryManager.Instance.InstallID);
        try
        {
          BstHttpClient.Post(RegistryManager.Instance.Host + "/stats/systeminfostats2", data, (Dictionary<string, string>) null, false, MultiInstanceStrings.VmName, 0, 1, 0, false, "bgp");
          RegistryManager.Instance.SystemInfoStats2 = 0;
        }
        catch (Exception ex)
        {
          Logger.Error("Exception in Sending systeminfostats2. Err : " + ex.ToString());
        }
      }
      try
      {
        JObject jobject = JObject.Parse(deviceCaps);
        if (!Oem.Instance.IsVTPopupEnabled || !jobject["cpu_hvm"].ToString().Equals("True", StringComparison.OrdinalIgnoreCase) || !jobject["bios_hvm"].ToString().Equals("False", StringComparison.OrdinalIgnoreCase) || !jobject["engine_enabled"].ToString().Equals("legacy", StringComparison.OrdinalIgnoreCase) && !jobject["engine_enabled"].ToString().Equals("raw", StringComparison.OrdinalIgnoreCase))
          return;
        AndroidBootUp.ShowVtxPopup();
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in showing vtxpopup {0}. Err : ", (object) ex.ToString());
      }
    }

    private static void ShowVtxPopup()
    {
      Logger.Info("User shown vtx enable popup");
      HTTPUtils.SendRequestToClient("showenablevtpopup", new Dictionary<string, string>()
      {
        {
          "url",
          "http://bluestacks-cloud.appspot.com/performance_with_vt"
        },
        {
          "title",
          "enablevt"
        }
      }, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
      try
      {
        Stats.SendMiscellaneousStatsSync("EnableVtx", (string) null, RegistryManager.Instance.UserGuid, "Enable vt popup shown", RegistryManager.Instance.Version, (string) null, (string) null, (string) null, (string) null, MultiInstanceStrings.VmName, 0);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in sending enablevtx stats. Err : {0}", (object) ex.Message);
      }
    }

    private static void AttachMonitor()
    {
      UIHelper.RunOnUIThread((Control) VMWindow.Instance, (UIHelper.Action) (() =>
      {
        if (AndroidBootUp.mManager != null)
          throw new SystemException("A connection to the manager is already open");
        if (AndroidBootUp.mMonitor != null)
          throw new SystemException("Another monitor is already attached");
        uint id = MonitorLocator.Lookup(MultiInstanceStrings.VmName);
        Manager manager = (Manager) null;
        Monitor monitor1 = (Monitor) null;
        bool verbose = false;
        try
        {
          verbose = AndroidBootUp.mFirstMonitorAttachAttempt;
          AndroidBootUp.mFirstMonitorAttachAttempt = false;
          manager = Manager.Open();
          monitor1 = manager.Attach(id, verbose, false);
        }
        catch (Exception ex)
        {
          if (!AndroidBootUp.IsExceptionFileNotFound(ex))
            Logger.Error(ex.ToString());
          if (manager != null)
            manager = (Manager) null;
        }
        if (monitor1 == null)
          return;
        Monitor monitor2 = manager.Attach(id, verbose, true);
        if (monitor2 == null)
          Logger.Info("Could not Attach to a monitor");
        AndroidBootUp.forceVideoModeChange = true;
        AndroidBootUp.mMonitor = monitor2;
        AndroidBootUp.mManager = manager;
        if (AndroidBootUp.HideBootProgress())
          return;
        AndroidBootUp.ShowConnectedView();
      }));
    }

    private static void ShowConnectedView()
    {
      Logger.Info(nameof (ShowConnectedView));
      if (VMWindow.Instance.IsShownOnce)
        MediaManager.UnmuteEngine();
      else
        MediaManager.MuteEngine(false);
      InputMapper.Instance.SetMonitor(AndroidBootUp.mMonitor);
      GPSManager.Instance().SetMonitor(AndroidBootUp.mMonitor);
      Logger.Debug("Raising Layout event");
      Opengl.userInteracted = true;
      if (!Opengl.IsSubWindowVisible())
      {
        Logger.Info("showing window");
        Opengl.glWindowAction = GlWindowAction.Show;
        Opengl.userInteracted = false;
      }
      LayoutManager.FixupGuestDisplay();
      AndroidBootUp.isAndroidReady = true;
      VMWindow.Instance.BootUpTasks();
    }

    private static void SendSecurityMessageToAndroidOnBootFinish()
    {
      try
      {
        Regex regex = new Regex("[^a-zA-Z0-9]");
        Assembly executingAssembly = Assembly.GetExecutingAssembly();
        X509Certificate signerCertificate = executingAssembly.GetModules()[0].GetSignerCertificate();
        string str1 = string.Empty;
        if (signerCertificate != null)
        {
          string input = BitConverter.ToString(signerCertificate.GetSerialNumber());
          str1 = !string.IsNullOrEmpty(input) ? regex.Replace(input, ".") : string.Empty;
        }
        string location = executingAssembly.Location;
        string str2 = !string.IsNullOrEmpty(location) ? regex.Replace(location, ".") : string.Empty;
        string baseKeyPath = RegistryManager.Instance.BaseKeyPath;
        VmCmdHandler.RunCommand(string.Format("{0} {1}", (object) "setBlueStacksConfig", (object) ("{" + string.Format("\"regPath\":\"{0}\",", !string.IsNullOrEmpty(baseKeyPath) ? (object) regex.Replace(baseKeyPath, ".") : (object) string.Empty) + string.Format("\"location\":\"{0}\",", (object) str2) + string.Format("\"serial\":\"{0}\"", (object) str1) + "}")), MultiInstanceStrings.VmName, "bgp");
      }
      catch (Exception ex)
      {
        int num = (int) System.Windows.Forms.MessageBox.Show(ex.ToString());
      }
    }

    internal static bool HideBootProgress()
    {
      return RegistryManager.Instance.DefaultGuest.HideBootProgress == 1;
    }

    private static bool IsExceptionFileNotFound(Exception exc)
    {
      Exception innerException = exc.InnerException;
      return innerException != null && innerException.GetType() == typeof (Win32Exception) && ((Win32Exception) innerException).NativeErrorCode == 2;
    }

    internal static void HandleBootError()
    {
      lock (AndroidBootUp.mSendingBootFailureLogsLock)
      {
        Logger.Error("Handling Boot Error");
        Logger.Error(new StackTrace().ToString());
        AndroidBootUp.SendBootFailureLogs();
        if (string.Equals(Oem.Instance.OEM, "dmm", StringComparison.InvariantCultureIgnoreCase))
          HTTPUtils.SendRequestToAgent("guestBootFailed", (Dictionary<string, string>) null, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp", true);
        if (Oem.Instance.IsOEMWithBGPClient)
        {
          HTTPUtils.SendRequestToClientAsync("bootFailedPopup", (Dictionary<string, string>) null, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
        }
        else
        {
          int num = (int) System.Windows.Forms.MessageBox.Show(LocaleStrings.GetLocalizedString("STRING_SOME_ERROR_OCCURED", ""));
        }
        Environment.Exit(-1);
      }
    }

    internal static void SendBootFailureLogs()
    {
      try
      {
        Logger.Info("In SendBootFailureLogs");
        Process.Start(Path.Combine(RegistryStrings.InstallDir, "HD-LogCollector.exe"), "-boot");
      }
      catch (Exception ex)
      {
        Logger.Error("Exception occured in SendBootFailureLogs. Err : {0}", (object) ex.ToString());
      }
    }

    public static bool CheckIfErrorLogsAlreadySent(string category, int exitCode)
    {
      Logger.Info("Checking if logs sent for category: {0} with exitcode: {1}", (object) category, (object) exitCode);
      lock (AndroidBootUp.sLogFailureLogRegLock)
      {
        RegistryKey subKey = Registry.LocalMachine.CreateSubKey(RegistryManager.Instance.HostConfigKeyPath + "\\FailureLogsInfo\\");
        string str1 = (string) subKey.GetValue(category, (object) "");
        if (!string.IsNullOrEmpty(str1))
        {
          string str2 = str1;
          char[] chArray = new char[1]{ ',' };
          foreach (string strA in str2.Split(chArray))
          {
            if (string.Compare(strA, exitCode.ToString()) == 0)
            {
              Logger.Info("Logs already sent");
              return true;
            }
          }
          subKey.SetValue(category, (object) (str1 + "," + exitCode.ToString()));
          Logger.Info("Logs not sent, will send this time");
          return false;
        }
        subKey.SetValue(category, (object) exitCode.ToString());
        Logger.Info("Logs not sent, will send this time");
        return false;
      }
    }
  }
}
