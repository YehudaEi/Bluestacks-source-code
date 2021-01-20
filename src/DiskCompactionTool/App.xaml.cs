// Decompiled with JetBrains decompiler
// Type: BlueStacks.DiskCompactionTool.App
// Assembly: DiskCompactionTool, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 0D774D0F-793E-496D-B768-12A2EDB900B5
// Assembly location: C:\Program Files\BlueStacks\DiskCompactionTool.exe

using BlueStacks.Common;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Markup;

namespace BlueStacks.DiskCompactionTool
{
  public partial class App : System.Windows.Application, IComponentConnector
  {
    internal static Mutex sDiskCompactionToolLock = (Mutex) null;
    internal static EventHandler confirmButtonHandler = (EventHandler) null;
    internal static App.Opt sOpt;
    private bool _contentLoaded;

    [STAThread]
    public static void Main(string[] args)
    {
      App.Init();
      App.sOpt = new App.Opt();
      App.sOpt.Parse(args);
      App app = new App();
      app.Startup += new StartupEventHandler(App.Application_Startup);
      app.Exit += new ExitEventHandler(App.Application_Exit);
      app.InitializeComponent();
      app.Run();
    }

    private static void Application_Startup(object sender, StartupEventArgs e)
    {
      Logger.Info("In Application_Startup");
      ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(App.ValidateRemoteCertificate);
      ServicePointManager.DefaultConnectionLimit = 1000;
    }

    protected override void OnStartup(StartupEventArgs e)
    {
      base.OnStartup(e);
      if (!this.CheckForSupportedVersion())
      {
        if (!App.sOpt.s)
        {
          CustomMessageWindow customMessageWindow = new CustomMessageWindow();
          customMessageWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_UNSUPPORTED_VERSION", "");
          customMessageWindow.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_VERSION_NOT_SUPPORTED", "");
          customMessageWindow.AddButton(ButtonColors.Blue, LocaleStrings.GetLocalizedString("STRING_EXIT", ""), (EventHandler) ((o, args) => App.ExitApplication(-1)), (string) null, false, (object) null, true);
          customMessageWindow.CloseButtonHandle((EventHandler) ((o, args) => App.ExitApplication(-1)), (object) null);
          customMessageWindow.ShowDialog();
        }
        else
          App.ExitApplication(-1);
      }
      else
      {
        MainWindow mainWindow = new MainWindow();
        if (!App.sOpt.s)
        {
          mainWindow.Show();
        }
        else
        {
          Stats.SendMiscellaneousStatsAsync("DiskCompactionStats", "DiskCompaction Started", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, App.sOpt.vmname, App.sOpt.relaunch.ToString(), App.sOpt.s.ToString(), (string) null, "Android", 0);
          DiskCompactionHelper.Instance.StartCompaction();
        }
      }
    }

    private bool CheckForSupportedVersion()
    {
      Logger.Info("In CheckForSupportedVersion");
      return new System.Version(RegistryManager.Instance.Version) >= new System.Version("4.60.00.0000");
    }

    private static bool ValidateRemoteCertificate(
      object sender,
      X509Certificate certificate,
      X509Chain chain,
      SslPolicyErrors sslPolicyErrors)
    {
      return true;
    }

    private static void Init()
    {
      Logger.InitUserLog();
      System.Windows.Forms.Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
      System.Windows.Forms.Application.ThreadException += new ThreadExceptionEventHandler(App.Application_ThreadException);
      AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(App.CurrentDomain_UnhandledException);
      App.CheckIfAlreadyRunning();
    }

    private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
    {
      Logger.Error("Unhandled Exception:" + e.Exception.ToString());
      App.ExitApplication(-3);
    }

    private static void CurrentDomain_UnhandledException(
      object sender,
      UnhandledExceptionEventArgs e)
    {
      Logger.Error("Unhandled Exception:" + e.ExceptionObject.ToString());
      App.ExitApplication(-3);
    }

    private static void CheckIfAlreadyRunning()
    {
      if (!ProcessUtils.CheckAlreadyRunningAndTakeLock("Global\\BlueStacks_DiskCompactor_Lockbgp", out App.sDiskCompactionToolLock))
        return;
      Utils.BringToFront("Disk cleanup");
      Logger.Warning("Instance already running, exiting");
      App.ExitApplication(0);
    }

    private static void Application_Exit(object sender, ExitEventArgs e)
    {
      App.ExitApplication(0);
    }

    internal static void ExitApplication(int exitCode = 0)
    {
      Logger.Info("Exiting Application with errorcode: {0} {1} ", (object) exitCode, (object) ((DiskCompactionToolCodes) exitCode).ToString());
      System.Windows.Forms.Application.ThreadException -= new ThreadExceptionEventHandler(App.Application_ThreadException);
      AppDomain.CurrentDomain.UnhandledException -= new UnhandledExceptionEventHandler(App.CurrentDomain_UnhandledException);
      if (App.sDiskCompactionToolLock != null)
        App.sDiskCompactionToolLock.Close();
      Environment.Exit(exitCode);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      System.Windows.Application.LoadComponent((object) this, new Uri("/DiskCompactionTool;component/app.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      this._contentLoaded = true;
    }

    public class Opt : GetOpt
    {
      public string vmname = "Android";
      public bool s = false;
      public bool relaunch = false;
    }
  }
}
