// Decompiled with JetBrains decompiler
// Type: BlueStacks.Uninstaller.App
// Assembly: BlueStacksUninstaller, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: DBF002A0-6BF3-43CC-B5E7-0E90D1C19949
// Assembly location: C:\Program Files\BlueStacks\BlueStacksUninstaller.exe

using BlueStacks.Common;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Markup;

namespace BlueStacks.Uninstaller
{
  public partial class App : System.Windows.Application, IComponentConnector
  {
    public static Stopwatch sClock = (Stopwatch) null;
    private static List<string> sRequiredFilesFromInstallDir = new List<string>()
    {
      "BlueStacksUninstaller.exe",
      "BlueStacksUninstaller.exe.config",
      "HD-Common.dll"
    };
    private static List<string> sRequiredGameFilesFromClientInstallDir = new List<string>()
    {
      "game_config.json"
    };
    private static List<string> sRequiredGameAssets = new List<string>()
    {
      "app_icon.ico",
      "gameinstaller_bg.png",
      "gameinstaller_bg_blurred.png",
      "gameinstaller_logo.png"
    };
    private static string sUninstallerLogFileName = "";
    public static Mutex sBlueStacksUninstallerLock;
    private bool _contentLoaded;

    private static void Application_Startup(object sender, StartupEventArgs e)
    {
      App.CheckIfAlreadyRunning();
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

    protected override void OnStartup(StartupEventArgs e)
    {
      base.OnStartup(e);
      this.SetUninstallerProperties(e.Args);
      UninstallerStats.SendStatsAsync(UninstallerStatsEvent.UninstallStarted);
      new MainWindow().Show();
    }

    private void SetUninstallerProperties(string[] args)
    {
      UninstallerProperties.IsRunningInSilentMode = this.IsRunInHiddenMode(args);
      UninstallerProperties.GUID = RegistryManager.Instance.UserGuid;
      UninstallerProperties.CampaignName = RegistryManager.Instance.CampaignName;
      UninstallerProperties.CampaignMD5 = RegistryManager.Instance.CampaignMD5;
      UninstallerProperties.CloudHost = App.GetCloudHost;
      UninstallerProperties.CurrentLocale = CommonInstallUtils.GetCurrentLocale();
      UninstallerProperties.UninstallId = RegistryManager.Instance.InstallID;
      this.GetVmDesktopShortcutIconList();
    }

    private void GetVmDesktopShortcutIconList()
    {
      foreach (string vm in RegistryManager.Instance.VmList)
      {
        if ("Android".Equals(vm))
          UninstallerProperties.VmDisplayNameList.Add(Oem.Instance.DesktopShortcutFileName);
        UninstallerProperties.VmDisplayNameList.Add(string.Format("{0}-{1}.lnk", (object) BlueStacks.Common.Strings.ProductDisplayName, (object) Utils.GetDisplayName(vm, "bgp")));
      }
    }

    public static string GetCloudHost
    {
      get
      {
        return (string) RegistryUtils.GetRegistryValue(BlueStacks.Common.Strings.RegistryBaseKeyPath, "Host", (object) "https://cloud.bluestacks.com", RegistryKeyKind.HKEY_CURRENT_USER);
      }
    }

    private bool IsRunInHiddenMode(string[] args)
    {
      try
      {
        foreach (string str in args)
        {
          if (str.Equals("-s"))
            return true;
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Got exception while parsing and comparing args, ex:{0}", (object) ex.ToString());
      }
      return false;
    }

    private static void Init(string logFileName)
    {
      Logger.InitLogAtPath(App.GetUninstallerLogPath(logFileName), "BlueStacksUninstaller", true);
      App.SetupExceptionHandlers();
    }

    private static void CheckIfAlreadyRunning()
    {
      if (!ProcessUtils.CheckAlreadyRunningAndTakeLock("Global\\BlueStacks_Uninstaller_Lockbgp", out App.sBlueStacksUninstallerLock))
        return;
      App.ExitApplication();
    }

    private static void SetupExceptionHandlers()
    {
      System.Windows.Forms.Application.ThreadException += new ThreadExceptionEventHandler(App.Application_ThreadException);
      System.Windows.Forms.Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
      AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(App.CurrentDomain_UnhandledException);
    }

    [STAThread]
    public static void Main(string[] args)
    {
      App.sClock = new Stopwatch();
      App.sClock.Start();
      App.Init(App.GetLogFilePath(args));
      if (args.Length != 0 && args[0].Equals("-tmp", StringComparison.OrdinalIgnoreCase))
      {
        List<string> list = ((IEnumerable<string>) args).ToList<string>();
        list.RemoveAt(0);
        App.CopyAndRunUninstallerFromTemp(list.ToArray());
      }
      App app = new App();
      app.Startup += new StartupEventHandler(App.Application_Startup);
      app.Exit += new ExitEventHandler(App.Application_Exit);
      app.InitializeComponent();
      app.Run();
    }

    private static string GetLogFilePath(string[] args)
    {
      foreach (string str in args)
      {
        try
        {
          if (str.StartsWith("-log:"))
            return str.Replace("-log:", "");
        }
        catch
        {
        }
      }
      return (string) null;
    }

    private static void CopyAndRunUninstallerFromTemp(string[] args)
    {
      string tempPath = Path.GetTempPath();
      string installDir = RegistryStrings.InstallDir;
      string str1 = Path.Combine(installDir, "Assets");
      string clientInstallDir = RegistryManager.Instance.ClientInstallDir;
      Utils.CopyRecursive(Path.Combine(installDir, "Assets"), Path.Combine(tempPath, "Assets"));
      if (RegistryManager.Instance.InstallationType == InstallationTypes.GamingEdition)
      {
        App.CopyListOfFiles((IEnumerable<string>) App.sRequiredGameFilesFromClientInstallDir, clientInstallDir, tempPath);
        App.CopyListOfFiles((IEnumerable<string>) App.sRequiredGameAssets, str1, tempPath);
      }
      else
      {
        List<string> stringList = new List<string>();
        foreach (string file in Directory.GetFiles(str1))
          stringList.Add(Path.GetFileName(file));
        App.CopyListOfFiles((IEnumerable<string>) stringList, str1, Path.Combine(tempPath, "Assets"));
      }
      App.CopyListOfFiles((IEnumerable<string>) App.sRequiredFilesFromInstallDir, installDir, tempPath);
      Logger.Info("Copying oem.cfg");
      System.IO.File.Copy(Path.Combine(RegistryStrings.DataDir, "Oem.cfg"), Path.Combine(tempPath, "Oem.cfg"), true);
      Logger.Info("Copying theme file");
      System.IO.File.Copy(Path.Combine(clientInstallDir, "Assets\\ThemeFile"), Path.Combine(tempPath, "ThemeFile"), true);
      Logger.Info("All files copied, starting File exectuion from temp path");
      string str2 = Path.Combine(tempPath, "BlueStacksUninstaller.exe");
      new Process()
      {
        StartInfo = {
          FileName = str2,
          Arguments = string.Format("-log:{0} {1}", (object) App.sUninstallerLogFileName, (object) string.Join(" ", args))
        }
      }.Start();
      Logger.Info("Main unistall process started, exiting current application.");
      App.ExitApplication();
    }

    private static void CopyListOfFiles(
      IEnumerable<string> listOfFiles,
      string srcDir,
      string dstDir)
    {
      Logger.Info("Copying files from {0} to {1}", (object) srcDir, (object) dstDir);
      if (!Directory.Exists(dstDir))
        Directory.CreateDirectory(dstDir);
      foreach (string listOfFile in listOfFiles)
      {
        if (string.IsNullOrEmpty(listOfFile))
        {
          Logger.Warning("FileCopy called for an empty filename or a non-file");
          break;
        }
        string str = Path.Combine(srcDir, listOfFile);
        string destFileName = Path.Combine(dstDir, listOfFile);
        try
        {
          if (System.IO.File.Exists(str))
            System.IO.File.Copy(str, destFileName, true);
        }
        catch (Exception ex)
        {
          Logger.Error("Failed to copy file {0}. Err: {1}", (object) listOfFile, (object) ex.Message);
          App.ExitApplication();
        }
      }
    }

    public static string GetUninstallerLogPath(string logFileName)
    {
      string path1 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "BlueStacks");
      if (string.IsNullOrEmpty(logFileName))
      {
        DateTime now = DateTime.Now;
        App.sUninstallerLogFileName = string.Format("BlueStacks-Uninstaller_{0}-{1}-{2}_{3}-{4}-{5}.log", (object) now.Year, (object) now.Month, (object) now.Day, (object) now.Hour, (object) now.Minute, (object) now.Second);
      }
      else
        App.sUninstallerLogFileName = logFileName;
      UninstallerProperties.LogFilePath = Path.Combine(path1, App.sUninstallerLogFileName);
      return UninstallerProperties.LogFilePath;
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

    internal static void ExitApplication()
    {
      if (App.sClock.IsRunning)
        App.sClock.Stop();
      if (App.sBlueStacksUninstallerLock != null)
        App.sBlueStacksUninstallerLock.Close();
      Environment.Exit(0);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      System.Windows.Application.LoadComponent((object) this, new Uri("/BlueStacksUninstaller;component/app.xaml", UriKind.Relative));
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
