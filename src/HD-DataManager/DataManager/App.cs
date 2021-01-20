// Decompiled with JetBrains decompiler
// Type: BlueStacks.DataManager.App
// Assembly: HD-DataManager, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 4AB16B4A-CAF7-4470-9488-3C5B163E3D07
// Assembly location: C:\Program Files\BlueStacks\HD-DataManager.exe

using BlueStacks.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Windows;
using System.Windows.Forms;

namespace BlueStacks.DataManager
{
  public class App : System.Windows.Application
  {
    public static App.Opt sOpt;
    public static Mutex sDataManagerLock;
    public static Dictionary<string, string> sBackUpInfoDict;
    public static bool DeleteOldCustomKeyMappings;

    [STAThread]
    public static void Main(string[] args)
    {
      App.InitExceptionHandlers();
      App.sOpt = new App.Opt();
      App.sOpt.Parse(args);
      Strings.CurrentDefaultVmName = App.sOpt.vmname;
      MultiInstanceStrings.VmName = App.sOpt.vmname;
      RegistryManager.ClientThemeName = RegistryManager.Instance.GetClientThemeNameFromRegistry();
      App app = new App();
      app.Startup += new StartupEventHandler(App.Application_Startup);
      app.Exit += new ExitEventHandler(App.Application_Exit);
      app.Run();
    }

    public static void Application_Startup(object sender, StartupEventArgs ev)
    {
      Logger.Info("In Application_Startup");
      ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(App.ValidateRemoteCertificate);
      ServicePointManager.DefaultConnectionLimit = 1000;
      App.CheckIfAlreadyRunning();
      App.StartingProgressWindow(new ProgressWindow()
      {
        mLastRow = {
          Height = new GridLength(0.0, GridUnitType.Star)
        }
      });
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
      if (!ProcessUtils.CheckAlreadyRunningAndTakeLock("Global\\BlueStacks_Downloader_Lockbgp", out App.sDataManagerLock))
        return;
      Logger.Info(Process.GetCurrentProcess().ProcessName + " already running");
      App.ExitApplication();
    }

    private static void ShowWindow(ProgressWindow progress)
    {
      progress.Visibility = Visibility.Visible;
      progress.Show();
      progress.BringIntoView();
      if (progress.Topmost)
        return;
      progress.Topmost = true;
      progress.Topmost = false;
    }

    private static void StartingProgressWindow(ProgressWindow progressWindow)
    {
      Logger.Info("In Preference Manage");
      if (!App.sOpt.s)
      {
        progressWindow.mProgressHeader.Text = LocaleStrings.GetLocalizedString("STRING_QUITTING", "");
        if (App.sOpt.backup)
          progressWindow.mProgressText.Text = LocaleStrings.GetLocalizedString("STRING_QUITTING_BLUESTACKS_WHILE_BACKUP_PROMPT", "");
        else if (App.sOpt.restore)
          progressWindow.mProgressText.Text = LocaleStrings.GetLocalizedString("STRING_QUITTING_BLUESTACKS_WHILE_RESTORE_PROMPT", "");
        App.ShowWindow(progressWindow);
      }
      string dataDir = RegistryStrings.DataDir;
      if (App.sOpt.restore)
      {
        new Restore(progressWindow).RestoreData(dataDir, App.sOpt.path);
      }
      else
      {
        if (!App.sOpt.backup)
          return;
        new Backup(progressWindow).BackupData(dataDir, App.sOpt.path);
      }
    }

    private static void InitExceptionHandlers()
    {
      Logger.InitLog("DataManager", "DataManager", false);
      Logger.Info("DataManager: Starting DataManager PID {0}", (object) Process.GetCurrentProcess().Id);
      Logger.Info("DataManager: CLR version {0}", (object) Environment.Version);
      Logger.Info("DataManager: IsAdministrator: {0}", (object) SystemUtils.IsAdministrator());
      System.Windows.Forms.Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
      System.Windows.Forms.Application.ThreadException += new ThreadExceptionEventHandler(App.Application_ThreadException);
      AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(App.CurrentDomain_UnhandledException);
    }

    private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
    {
      Logger.Error("Unhandled Thread Exception:");
      Logger.Error(e.Exception.ToString());
      App.ExitApplication();
    }

    private static void CurrentDomain_UnhandledException(
      object sender,
      UnhandledExceptionEventArgs e)
    {
      Logger.Error("Unhandled Application Exception:");
      Logger.Error(e.ExceptionObject.ToString());
      App.ExitApplication();
    }

    private static void Application_Exit(object sender, ExitEventArgs e)
    {
      App.ExitApplication();
    }

    private static void ExitApplication()
    {
      if (App.sDataManagerLock != null)
        App.sDataManagerLock.Close();
      Process.GetCurrentProcess().Kill();
    }

    public static void LaunchBlueStacks()
    {
      new Process()
      {
        StartInfo = {
          FileName = RegistryManager.Instance.PartnerExePath,
          Arguments = string.Format("-vmName={0}", (object) App.sOpt.vmname),
          UseShellExecute = false
        }
      }.Start();
    }

    public class Opt : GetOpt
    {
      public string vmname = "Android";
      public bool restore;
      public bool backup;
      public string path;
      public bool s;
    }
  }
}
