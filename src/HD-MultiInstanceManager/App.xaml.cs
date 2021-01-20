// Decompiled with JetBrains decompiler
// Type: MultiInstanceManagerMVVM.App
// Assembly: HD-MultiInstanceManager, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 0C4C54E8-53C7-4A46-AE96-CC8E89A6B884
// Assembly location: C:\Program Files\BlueStacks\HD-MultiInstanceManager.exe

using BlueStacks.Common;
using MultiInstanceManagerMVVM.Helper;
using MultiInstanceManagerMVVM.HTTPHandlers;
using MultiInstanceManagerMVVM.View.Classes.MultiInstance;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Markup;

namespace MultiInstanceManagerMVVM
{
  public partial class App : System.Windows.Application, IComponentConnector
  {
    private static Mutex sMultiInstanceManagerLock;
    private static Opt opt;
    private bool _contentLoaded;

    [STAThread]
    public static void Main(string[] args)
    {
      App.Init();
      App.opt = new Opt();
      App.opt.Parse(args);
      App app = new App();
      app.Startup += new StartupEventHandler(App.Application_Startup);
      app.ShutdownMode = ShutdownMode.OnExplicitShutdown;
      app.Exit += new ExitEventHandler(App.Application_Exit);
      app.InitializeComponent();
      app.Run();
    }

    private static void Application_Exit(object sender, ExitEventArgs e)
    {
      App.ExitApplication();
    }

    private static void Init()
    {
      Logger.InitUserLog();
      System.Windows.Forms.Application.ThreadException += new ThreadExceptionEventHandler(App.Application_ThreadException);
      System.Windows.Forms.Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
      AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(App.CurrentDomain_UnhandledException);
      App.CheckIfAlreadyRunning();
      App.StartAgent();
      HttpHelper.InitHttpServerAsync();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
      Logger.Info(nameof (OnStartup));
      if (App.opt.IsMIMLaunchedFromClient)
        Stats.SendMultiInstanceStatsAsync("mim_launched_from_side_toolbar", "", "", "", 0, "", 0, "bgp", "", "", "", "", RegistryManager.Instance.CampaignMD5, true, "");
      else
        Stats.SendMultiInstanceStatsAsync("mim_launched_from_windows_shortcut", "", "", "", 0, "", 0, "bgp", "", "", "", "", RegistryManager.Instance.CampaignMD5, true, "");
      new MultiInstanceView().ShowDialog();
      base.OnStartup(e);
    }

    private static void CurrentDomain_UnhandledException(
      object sender,
      UnhandledExceptionEventArgs e)
    {
      Logger.Error(e.ExceptionObject.ToString());
      int num = (int) System.Windows.MessageBox.Show(e.ExceptionObject.ToString());
    }

    private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
    {
      Logger.Error(e.Exception.ToString());
      int num = (int) System.Windows.MessageBox.Show(e.Exception.ToString());
    }

    private static void Application_Startup(object sender, StartupEventArgs e)
    {
      ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(App.ValidateRemoteCertificate);
      ServicePointManager.DefaultConnectionLimit = 1000;
    }

    private static bool ValidateRemoteCertificate(
      object sender,
      X509Certificate certificate,
      X509Chain chain,
      SslPolicyErrors sslPolicyErrors)
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
              CustomMessageWindow customMessageWindow = new CustomMessageWindow()
              {
                ImageName = "ProductLogo"
              };
              customMessageWindow.TitleTextBlock.Text = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}", (object) LocaleStrings.GetLocalizedString("STRING_EXIT_BLUESTACKS_DUE_TO_DISK_COMPACTION_HEADING", ""));
              customMessageWindow.BodyTextBlock.Text = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}", (object) LocaleStrings.GetLocalizedString("STRING_EXIT_BLUESTACKS_DUE_TO_DISK_COMPACTION_MESSAGE", ""));
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
      if (ProcessUtils.IsAnyInstallerProcesRunning(out runningProcName) && !string.IsNullOrEmpty(runningProcName))
      {
        Logger.Info(runningProcName + " process is running. Exiting BlueStacks MultiInstance Manager");
        Environment.Exit(-1);
      }
      if (!ProcessUtils.CheckAlreadyRunningAndTakeLock("Global\\BlueStacks_MultiInstanceManager_Lockbgp", out App.sMultiInstanceManagerLock))
        return;
      Utils.BringToFront("BlueStacks Multi-Instance Manager");
      App.ExitApplication();
    }

    public static void ExitApplication()
    {
      Logger.Info("Exiting application");
      if (App.sMultiInstanceManagerLock != null)
      {
        App.sMultiInstanceManagerLock.Close();
        App.sMultiInstanceManagerLock = (Mutex) null;
      }
      Environment.Exit(0);
    }

    private static void StartAgent()
    {
      Logger.Info("Launching agent");
      Process.Start(Path.Combine(RegistryStrings.InstallDir, "HD-Agent.exe"));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      System.Windows.Application.LoadComponent((object) this, new Uri("/HD-MultiInstanceManager;component/app.xaml", UriKind.Relative));
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
