// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.App
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using Microsoft.Win32;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Markup;
using Xilium.CefGlue;

namespace BlueStacks.BlueStacksUI
{
  public class App : System.Windows.Application, IComponentConnector
  {
    private static Mutex mBluestacksUILock;
    internal static Fraction defaultResolution;
    private bool _contentLoaded;

    public static Mutex BlueStacksUILock
    {
      get
      {
        return App.mBluestacksUILock;
      }
      set
      {
        App.mBluestacksUILock = value;
      }
    }

    internal static bool IsApplicationActive { get; set; }

    [STAThread]
    public static void Main(string[] args)
    {
      Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
      App.InitExceptionAndLogging();
      ProcessUtils.LogParentProcessDetails();
      if (args != null)
      {
        App.ParseWebMagnetArgs(ref args);
        Opt.Instance.Parse(args);
      }
      string path = Path.Combine(Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory.Trim('\\')).FullName, "Engine"), "Oem.cfg");
      Oem.CurrentOemFilePath = System.IO.File.Exists(path) ? path : Path.Combine(RegistryManager.Instance.EngineDataDir, "Oem.cfg");
      PortableInstaller.CheckAndRunPortableInstaller();
      if (!RegistryManager.Instance.Guest.ContainsKey(Opt.Instance.vmname))
        Opt.Instance.vmname = "Android";
      BlueStacks.Common.Strings.CurrentDefaultVmName = Opt.Instance.vmname;
      if (Opt.Instance.mergeCfg)
      {
        KMManager.MergeConfig(Opt.Instance.newPDPath);
        Environment.Exit(0);
      }
      string mBSTProcessIdentifier = "Bluestacks/" + RegistryManager.Instance.ClientVersion;
      if (!string.Join(string.Empty, args).Contains(mBSTProcessIdentifier))
      {
        Logger.Info("BOOT_STAGE: Client starting");
        if (Oem.IsOEMDmm)
        {
          Logger.Info("checking DMMGamePlayer process");
          if (!ProcessUtils.FindProcessByName("DMMGamePlayer") && !ProcessUtils.FindProcessByName("dmmemulatorsandboxlauncher"))
          {
            Logger.Info("DMM game player not running, so exiting");
            Environment.Exit(0);
          }
        }
        RegistryManager.ClientThemeName = RegistryManager.Instance.GetClientThemeNameFromRegistry();
        if (Oem.IsOEMDmm || !BlueStacksUpdater.CheckIfDownloadedFileExist())
        {
          App app = new App();
          app.Startup += new StartupEventHandler(App.Application_Startup);
          app.ShutdownMode = ShutdownMode.OnExplicitShutdown;
          app.InitializeComponent();
          App.CheckIfAlreadyRunning();
          RegistryManager.Instance.ClientLaunchParams = Opt.Instance.Json;
          App.defaultResolution = new Fraction((long) RegistryManager.Instance.Guest[BlueStacks.Common.Strings.CurrentDefaultVmName].GuestWidth, (long) RegistryManager.Instance.Guest[BlueStacks.Common.Strings.CurrentDefaultVmName].GuestHeight);
          SystemEvents.DisplaySettingsChanged += new EventHandler(App.HandleDisplaySettingsChanged);
          BGPHelper.InitHttpServerAsync();
          BlueStacksUIUtils.RunInstance(BlueStacks.Common.Strings.CurrentDefaultVmName, Opt.Instance.h, Opt.Instance.Json);
          AppUsageTimer.SessionEventHandler();
          if (!Oem.IsOEMDmm)
          {
            PromotionManager.ReloadPromotionsAsync();
            GrmManager.UpdateGrmAsync((IEnumerable<string>) null);
            GuidanceCloudInfoManager.Instance.AppsGuidanceCloudInfoRefresh();
          }
          if (!FeatureManager.Instance.IsHtmlHome)
            BlueStacksUIUtils.DictWindows[BlueStacks.Common.Strings.CurrentDefaultVmName].CreateFirebaseBrowserControl();
          MemoryManager.TrimMemory(true);
          app.Run();
        }
        else
          BlueStacksUpdater.HandleUpgrade(RegistryManager.Instance.DownloadedUpdateFile);
      }
      else
        CefHelper.InitCef(args, mBSTProcessIdentifier);
      AppUsageTimer.DetachSessionEventHandler();
      CefRuntime.Shutdown();
      App.ExitApplication();
    }

    private static void AddHypervToAdminGroupAndRestartConfirmation(object sender, EventArgs e)
    {
      Logger.Info("Adding to Hyperv admin group and restarting windows.");
      Process process = ProcessUtils.StartExe(Path.Combine(RegistryStrings.InstallDir, "HD-AddToHVAdmin.exe"), (HttpContext.Current != null ? HttpContext.Current.User.Identity as WindowsIdentity : WindowsIdentity.GetCurrent()).User.ToString(), true);
      process.WaitForExit();
      if (process.ExitCode != 0)
        Logger.Error("Unable to add user to HyperV admin group exitcode: " + process.ExitCode.ToString());
      Process.Start("shutdown", "/r /t 0");
    }

    private static void HandleDisplaySettingsChanged(object sender, EventArgs e)
    {
      try
      {
        foreach (MainWindow mainWindow in BlueStacksUIUtils.DictWindows.Values.ToList<MainWindow>())
        {
          if (mainWindow != null && !mainWindow.IsClosed)
            mainWindow.HandleDisplaySettingsChanged();
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in HandleDisplaySettingsChanged. Exception: " + ex.ToString());
      }
    }

    private static void ParseWebMagnetArgs(ref string[] args)
    {
      if (args.Length == 0 || !args[0].StartsWith("bluestacksgp:", StringComparison.InvariantCultureIgnoreCase))
        return;
      Logger.Info("Handling web uri: " + args[0]);
      string[] strArray1 = args[0].Split(new char[1]{ ':' }, 2);
      string[] strArray2 = new string[args.Length + 1];
      string[] strArray3 = Uri.UnescapeDataString(strArray1[1]).TrimStart().Split(new char[1]
      {
        ' '
      }, 2);
      if (strArray3.Length > 1)
      {
        Array.Copy((Array) strArray3, 0, (Array) strArray2, 0, 2);
        Array.Copy((Array) args, 1, (Array) strArray2, 2, args.Length - 1);
        args = strArray2;
      }
      else
        args[0] = strArray3[0];
    }

    private static void InitExceptionAndLogging()
    {
      Logger.InitLog("BlueStacksUI", "BlueStacksUI", true);
      System.Windows.Forms.Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
      System.Windows.Forms.Application.ThreadException += new ThreadExceptionEventHandler(App.Application_ThreadException);
      AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(App.CurrentDomain_UnhandledException);
    }

    private static void Application_Startup(object sender, StartupEventArgs e)
    {
      Logger.Info("In Application_Startup");
      ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(App.ValidateRemoteCertificate);
      ServicePointManager.DefaultConnectionLimit = 1000;
    }

    private static bool ValidateRemoteCertificate(
      object sender,
      X509Certificate cert,
      X509Chain chain,
      SslPolicyErrors policyErrors)
    {
      return true;
    }

    private static void CheckIfAlreadyRunning()
    {
      try
      {
        if (ProcessUtils.IsAlreadyRunning("Global\\BlueStacks_DiskCompactor_Lockbgp"))
        {
          Logger.Info("Disk compaction is running in background");
          foreach (string str in GetProcessExecutionPath.GetApplicationPath(Process.GetProcessesByName("DiskCompactionTool")))
          {
            if (str.Equals(Path.Combine(RegistryStrings.InstallDir, "DiskCompactionTool.exe"), StringComparison.InvariantCultureIgnoreCase))
            {
              CustomMessageWindow customMessageWindow = new CustomMessageWindow();
              customMessageWindow.ImageName = "ProductLogo";
              customMessageWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_EXIT_BLUESTACKS_DUE_TO_DISK_COMPACTION_HEADING", "");
              customMessageWindow.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_EXIT_BLUESTACKS_DUE_TO_DISK_COMPACTION_MESSAGE", "");
              customMessageWindow.AddButton(ButtonColors.Blue, "STRING_OK", (EventHandler) null, (string) null, false, (object) null, true);
              customMessageWindow.CloseButtonHandle((Predicate<object>) null, (object) null);
              customMessageWindow.ShowDialog();
              Logger.Info("Disk compaction running for this instance. Exiting this instance");
              App.ExitApplication();
            }
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to check if disk compaction is running: " + ex.Message);
      }
      string runningProcName;
      if (!Opt.Instance.force && ProcessUtils.IsAnyInstallerProcesRunning(out runningProcName) && !string.IsNullOrEmpty(runningProcName))
      {
        Logger.Info(runningProcName + " process is running. Exiting BlueStacks");
        App.ExitApplication();
      }
      if (ProcessUtils.CheckAlreadyRunningAndTakeLock("Global\\BlueStacks_BlueStacksUI_Lockbgp", out App.mBluestacksUILock))
      {
        try
        {
          Logger.Info("Relaunching client for vm : " + Opt.Instance.vmname);
          Dictionary<string, string> data = new Dictionary<string, string>()
          {
            {
              "vmname",
              Opt.Instance.vmname
            },
            {
              "hidden",
              Opt.Instance.h.ToString((IFormatProvider) CultureInfo.InvariantCulture)
            }
          };
          if (Opt.Instance.launchedFromSysTray)
            data.Add("all", "True");
          if (!string.IsNullOrEmpty(Opt.Instance.Json))
          {
            data.Add("json", Opt.Instance.Json);
            Logger.Debug("OpenPackage result: " + HTTPUtils.SendRequestToClient("openPackage", data, Opt.Instance.vmname, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp"));
          }
          else
            Logger.Debug("ShowWindow result: " + HTTPUtils.SendRequestToClient("showWindow", data, Opt.Instance.vmname, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp"));
        }
        catch (Exception ex)
        {
          Logger.Error(ex.ToString());
        }
        Logger.Info("BlueStacksUI already running. Exiting this instance");
        App.ExitApplication();
      }
      else
      {
        try
        {
          Logger.Debug("Checking for existing process not exited");
          List<Process> list = ((IEnumerable<Process>) Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName)).ToList<Process>();
          if (!ProcessUtils.IsLockInUse("Global\\BlueStacks_BlueStacksUI_Closing_Lockbgp"))
            return;
          foreach (Process process in list)
          {
            if (process.Id != Process.GetCurrentProcess().Id)
              process.Kill();
          }
        }
        catch (Exception ex)
        {
          Logger.Warning("Ignoring error closing previous instances" + ex.ToString());
        }
      }
    }

    private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
    {
      if (!App.CheckForIgnoredExceptions(e.Exception.ToString()))
        return;
      Logger.Error("Unhandled Thread Exception:");
      Logger.Error(e.Exception.ToString());
      if (!FeatureManager.Instance.IsCustomUIForNCSoft)
      {
        int num = (int) System.Windows.Forms.MessageBox.Show("BlueStacks App Player.\nError: " + e.Exception.ToString());
      }
      App.ExitApplication();
    }

    private static void CurrentDomain_UnhandledException(
      object sender,
      UnhandledExceptionEventArgs e)
    {
      if (!App.CheckForIgnoredExceptions(((Exception) e.ExceptionObject).ToString()))
        return;
      App.CheckForIgnoredExceptions(((Exception) e.ExceptionObject).ToString());
      Logger.Error("Unhandled Application Exception.");
      Logger.Error("Err: " + e.ExceptionObject.ToString());
      if (!FeatureManager.Instance.IsCustomUIForNCSoft)
      {
        int num = (int) System.Windows.Forms.MessageBox.Show("BlueStacks App Player.\nError: " + ((Exception) e.ExceptionObject).ToString());
      }
      App.ExitApplication();
    }

    private static bool CheckForIgnoredExceptions(string s)
    {
      if (!s.Contains("GetFocusedElementFromWinEvent"))
        return true;
      Logger.Warning("Ignoring Unhandled Application Exception: " + s);
      return false;
    }

    internal static void ExitApplication()
    {
      foreach (MainWindow mainWindow in BlueStacksUIUtils.DictWindows.Values.ToList<MainWindow>())
      {
        if (mainWindow != null && !mainWindow.IsClosed)
          mainWindow.ForceCloseWindow(false);
      }
      App.UnwindEvents();
      App.ReleaseLock();
      Process.GetCurrentProcess().Kill();
    }

    internal static void UnwindEvents()
    {
      try
      {
        SystemEvents.DisplaySettingsChanged -= new EventHandler(App.HandleDisplaySettingsChanged);
      }
      catch (Exception ex)
      {
        Logger.Error("Couldn't unwind events properly; " + ex?.ToString());
      }
    }

    internal static void ReleaseLock()
    {
      try
      {
        BluestacksProcessHelper.TakeLock("Global\\BlueStacks_BlueStacksUI_Closing_Lockbgp");
        if (App.BlueStacksUILock == null)
          return;
        App.BlueStacksUILock.Close();
        App.BlueStacksUILock = (Mutex) null;
      }
      catch (Exception ex)
      {
        Logger.Warning("Ignoring Exception while releasing lock. Err : " + ex.ToString());
      }
    }

    private void Application_Activated(object sender, EventArgs e)
    {
      App.IsApplicationActive = true;
      foreach (MainWindow mainWindow in BlueStacksUIUtils.DictWindows.Values.ToList<MainWindow>())
        mainWindow.SendTempGamepadState(true);
    }

    private void Application_Deactivated(object sender, EventArgs e)
    {
      App.IsApplicationActive = false;
      foreach (MainWindow mainWindow in BlueStacksUIUtils.DictWindows.Values.ToList<MainWindow>())
      {
        if (mainWindow.mStreamingModeEnabled)
          mainWindow.SendTempGamepadState(true);
        else
          mainWindow.SendTempGamepadState(false);
      }
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      this.Activated += new EventHandler(this.Application_Activated);
      this.Deactivated += new EventHandler(this.Application_Deactivated);
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      System.Windows.Application.LoadComponent((object) this, new Uri("/Bluestacks;component/app.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      this._contentLoaded = true;
    }
  }
}
