// Decompiled with JetBrains decompiler
// Type: BlueStacks.LogCollector.App
// Assembly: HD-LogCollector, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: FD44426D-0F78-46B3-8118-1E64B979C51B
// Assembly location: C:\Program Files\BlueStacks\HD-LogCollector.exe

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

namespace BlueStacks.LogCollector
{
  public partial class App : System.Windows.Application, IComponentConnector
  {
    internal static List<string> sOemApplicableForLogCollection = new List<string>()
    {
      "bgp"
    };
    internal static Opt sOpt = (Opt) null;
    private static Mutex sLogCollectorLock;
    private bool _contentLoaded;

    [STAThread]
    private static void Main(string[] args)
    {
      App.InitExceptionAndLogging();
      string runningProcName;
      if (ProcessUtils.IsAnyInstallerProcesRunning(out runningProcName) && !string.IsNullOrEmpty(runningProcName))
      {
        Logger.Info(runningProcName + " process is running. Exiting BlueStacks LogCollector");
        Environment.Exit(-1);
      }
      App.PrintAndPraseArgs(args);
      App.SetVmPropertiesOrExit(App.sOpt.Vmname);
      App app = new App();
      app.Startup += (StartupEventHandler) ((sender, e) => App.Application_Startup(sender, e, args));
      app.Exit += new ExitEventHandler(App.Application_Exit);
      RegistryManager.ClientThemeName = RegistryManager.Instance.GetClientThemeNameFromRegistry();
      app.Run();
    }

    private static void PrintAndPraseArgs(string[] args)
    {
      App.PrintArgs(args);
      App.sOpt = new Opt();
      App.sOpt.Parse(args);
    }

    private static void SetVmPropertiesOrExit(string vmName)
    {
      if (!((IEnumerable<string>) RegistryManager.Instance.VmList).Contains<string>(BlueStacks.Common.Strings.CurrentDefaultVmName))
      {
        Logger.Info("VM: " + BlueStacks.Common.Strings.CurrentDefaultVmName + " does not exist");
        App.KillApplication();
      }
      if (!vmName.Contains("Android"))
        vmName = "Android";
      BlueStacks.Common.Strings.CurrentDefaultVmName = vmName;
      MultiInstanceStrings.VmName = vmName;
    }

    private static void Application_Exit(object sender, ExitEventArgs e)
    {
      App.KillApplication();
    }

    private static void PrintArgs(string[] args)
    {
      for (int index = 0; index < args.Length; ++index)
        Logger.Info("args[{0}] = {1}", (object) index, (object) args[index]);
    }

    private static void Application_Startup(object _1, StartupEventArgs _2, string[] args)
    {
      Logger.Info("In Application_Startup");
      ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(App.ValidateRemoteCertificate);
      ServicePointManager.DefaultConnectionLimit = 1000;
      App.CheckIfAlreadyRunning();
      App.StartLogCollection(args);
    }

    private static void StartLogCollection(string[] args)
    {
      bool showUI = false;
      if (string.Equals("bgp", "bgp", StringComparison.InvariantCultureIgnoreCase) || string.Equals("bgp", "bgp64", StringComparison.InvariantCultureIgnoreCase))
        App.sOemApplicableForLogCollection = InstalledOem.AllInstalledOemList.Where<string>((Func<string, bool>) (oem => string.Equals(oem, "bgp", StringComparison.InvariantCultureIgnoreCase) || string.Equals(oem, "bgp64", StringComparison.InvariantCultureIgnoreCase))).ToList<string>();
      for (int index = 0; index < args.Length; ++index)
      {
        if (App.sOpt.Extra)
        {
          CollectLogs.s_StartLogcat = true;
          break;
        }
        if (App.sOpt.Silent)
        {
          CollectLogs.s_SilentCollector = true;
          using (CollectLogs collectLogs = new CollectLogs(Path.GetTempPath(), true, App.sOpt.LogAllOems, App.sOpt.StartAllOems))
          {
            collectLogs.StartSilentLogCollection(showUI);
            App.KillApplication();
          }
        }
        if (App.sOpt.Thin)
        {
          CollectLogs.s_ThinCollector = true;
          using (CollectLogs collectLogs = new CollectLogs(Path.GetTempPath(), true, App.sOpt.LogAllOems, App.sOpt.StartAllOems))
          {
            collectLogs.StartSilentLogCollection(showUI);
            App.KillApplication();
          }
        }
        if (args[index].Equals("-boot", StringComparison.Ordinal))
        {
          CollectLogs.s_BootFailureLogs = true;
          bool uploadZip = true;
          string errorReason = "Generic";
          string errorCode = "-1";
          if (args.Length > index + 2)
          {
            errorReason = args[index + 1];
            errorCode = args[index + 2];
          }
          using (CollectLogs collectLogs = new CollectLogs(Path.GetTempPath(), uploadZip, errorReason, errorCode, App.sOpt.LogAllOems, App.sOpt.StartAllOems))
          {
            collectLogs.StartSilentLogCollection(showUI);
            App.KillApplication();
          }
        }
        else if (args[index].Equals("-startwithparam", StringComparison.Ordinal))
        {
          if (args.Length > index + 1)
          {
            string empty1 = string.Empty;
            string empty2 = string.Empty;
            string empty3 = string.Empty;
            string empty4 = string.Empty;
            string empty5 = string.Empty;
            string empty6 = string.Empty;
            List<string> stringList = new List<string>();
            string[] strArray = args[index + 1].Split('&');
            if (strArray.Length != 0)
              empty1 = strArray[0];
            if (strArray.Length > 1)
              empty2 = strArray[1];
            if (strArray.Length > 2)
              empty3 = strArray[2];
            if (strArray.Length > 3)
              empty4 = strArray[3];
            if (strArray.Length > 4)
              empty5 = strArray[4];
            if (strArray.Length > 5)
              empty6 = strArray[5];
            if (strArray.Length > 6)
              stringList.Add(strArray[6]);
            using (new CollectLogs(empty1, empty2, empty3, empty4, empty5, empty6, (ICollection<string>) stringList, App.sOpt.LogAllOems, App.sOpt.StartAllOems))
              CollectLogs.sProgressWindow.ShowDialog();
          }
          App.KillApplication();
        }
        else if (args[index].Equals("-apk", StringComparison.Ordinal) || args[index].Equals("-xapk", StringComparison.Ordinal))
        {
          bool uploadZip = true;
          CollectLogs.s_ApkInstallFailureLogCollector = true;
          string errorReason = "Apk-Generic";
          string errorCode = "-1";
          if (args.Length >= index + 3)
          {
            errorCode = args[index + 1];
            errorReason = args[index + 2];
            CollectLogs.s_InstallFailedApkName = args[index + 3];
          }
          using (CollectLogs collectLogs = new CollectLogs(Path.GetTempPath(), uploadZip, errorReason, errorCode, App.sOpt.LogAllOems, App.sOpt.StartAllOems))
          {
            collectLogs.StartSilentLogCollection(showUI);
            App.KillApplication();
          }
        }
        else if (args[index].Equals("-ReportCrashLogs", StringComparison.Ordinal))
        {
          bool flag = true;
          string tempPath = Path.GetTempPath();
          string str1 = "-1";
          string str2 = args[index + 1];
          CollectLogs.s_CrashLogs = true;
          string str3 = args[index + 2];
          int num1 = flag ? 1 : 0;
          string errorReason = str2;
          string errorDetails = str3;
          string errorCode = str1;
          int num2 = App.sOpt.LogAllOems ? 1 : 0;
          int num3 = App.sOpt.StartAllOems ? 1 : 0;
          using (CollectLogs collectLogs = new CollectLogs(tempPath, num1 != 0, errorReason, errorDetails, errorCode, num2 != 0, num3 != 0))
          {
            collectLogs.StartSilentLogCollection(showUI);
            App.KillApplication();
          }
        }
      }
      if (args.Length != 0)
      {
        int num = -1;
        for (int index = 0; index < args.Length; ++index)
        {
          if (args[index].Equals("-d", StringComparison.Ordinal))
            num = index;
        }
        if (num != -1)
        {
          bool uploadZip = false;
          string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
          if (args.Length > num + 1 && args[num + 1].Equals("-extra", StringComparison.Ordinal))
            folderPath = args[num + 1];
          if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);
          using (CollectLogs collectLogs = new CollectLogs(folderPath, uploadZip, App.sOpt.LogAllOems, App.sOpt.StartAllOems))
          {
            collectLogs.StartSilentLogCollection(showUI);
            App.KillApplication();
          }
        }
        else
          App.ShowMainWindow();
      }
      else
        App.ShowMainWindow();
    }

    private static void ShowMainWindow()
    {
      new MainWindow(App.sOpt.QuickLogs).Show();
    }

    private static void CheckIfAlreadyRunning()
    {
      if (!ProcessUtils.CheckAlreadyRunningAndTakeLock("Global\\BlueStacks_Log_Collector_Lockbgp", out App.sLogCollectorLock))
        return;
      bool flag = false;
      Logger.Info("LogCollector already running.");
      if (App.sOpt.Boot || App.sOpt.Apk || (App.sOpt.D || App.sOpt.Thin) || (App.sOpt.Silent || App.sOpt.Hidden))
        flag = true;
      if (!flag)
      {
        IntPtr window = InteropWindow.FindWindow((string) null, LocaleStrings.GetLocalizedString("STRING_BST_SUPPORT_UTILITY", ""));
        if (window != IntPtr.Zero)
        {
          InteropWindow.SetForegroundWindow(window);
        }
        else
        {
          CustomMessageWindow customMessageWindow = new CustomMessageWindow();
          customMessageWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_BST_SUPPORT_UTILITY", "");
          customMessageWindow.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_LOGCOLLECTOR_RUNNING", "");
          customMessageWindow.AddButton(ButtonColors.Blue, LocaleStrings.GetLocalizedString("STRING_OK", ""), (EventHandler) null, (string) null, false, (object) null, true);
          customMessageWindow.ShowDialog();
        }
      }
      App.KillApplication();
    }

    private static bool ValidateRemoteCertificate(
      object sender,
      X509Certificate cert,
      X509Chain chain,
      SslPolicyErrors policyErrors)
    {
      return true;
    }

    private static void InitExceptionAndLogging()
    {
      Logger.InitUserLog();
      Logger.Info("LogCollector: Starting LogCollector PID {0}", (object) Process.GetCurrentProcess().Id);
      Logger.Info("LogCollector: CLR version {0}", (object) Environment.Version);
      Logger.Info("LogCollector: IsAdministrator: {0}", (object) SystemUtils.IsAdministrator());
      System.Windows.Forms.Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
      System.Windows.Forms.Application.ThreadException += new ThreadExceptionEventHandler(App.Application_ThreadException);
      AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(App.CurrentDomain_UnhandledException);
    }

    private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
    {
      Logger.Error("Unhandled Thread Exception:");
      Logger.Error(e.Exception.ToString());
      App.KillApplication();
    }

    private static void CurrentDomain_UnhandledException(
      object sender,
      UnhandledExceptionEventArgs e)
    {
      Logger.Error("Unhandled Application Exception:");
      Logger.Error(e.ExceptionObject.ToString());
      App.KillApplication();
    }

    private static void KillApplication()
    {
      if (App.sLogCollectorLock != null)
      {
        App.sLogCollectorLock.Close();
        App.sLogCollectorLock = (Mutex) null;
      }
      Process.GetCurrentProcess().Kill();
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      System.Windows.Application.LoadComponent((object) this, new Uri("/HD-LogCollector;component/app.xaml", UriKind.Relative));
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
