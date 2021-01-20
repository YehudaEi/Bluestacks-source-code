// Decompiled with JetBrains decompiler
// Type: BlueStacks.XapkInstaller.HDXapkInstaller
// Assembly: HD-XapkHandler, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: B014B8CF-EF95-484D-84B7-5EFE7C0DA59F
// Assembly location: C:\Program Files\BlueStacks\HD-XapkHandler.exe

using BlueStacks.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Windows.Forms;

namespace BlueStacks.XapkInstaller
{
  public class HDXapkInstaller : Form
  {
    private static string sXapkPath = "";
    private static bool sIsSilent = false;
    private static Dictionary<string, string> data = new Dictionary<string, string>();
    private ProgressBar mProgressBar;
    private Label mLabel;
    private static Mutex sHDXapkInstallerLock;
    private const int PROC_KILL_TIMEOUT = 10000;
    private static DateTime sXapkHandlerLaunchTime;
    public static Dictionary<string, string[]> sOemWindowMapper;

    public static void Main(string[] args)
    {
      Logger.InitUserLog();
      string runningProcName;
      if (ProcessUtils.IsAnyInstallerProcesRunning(out runningProcName) && !string.IsNullOrEmpty(runningProcName))
      {
        Logger.Info(runningProcName + " process is running. Exiting BlueStacks Apk Handler");
        Environment.Exit(-1);
      }
      LocaleStrings.InitLocalization((string) null, "Android", false);
      HDXapkInstaller.InitExceptionHandlers();
      args = Utils.AddVmNameInArgsIfNotPresent(args);
      HDXapkInstaller.Opt opt = new HDXapkInstaller.Opt();
      opt.Parse(args);
      HDXapkInstaller.sIsSilent = opt.s;
      BlueStacks.Common.Strings.CurrentDefaultVmName = opt.vmname;
      Logger.Info("the installvm is {0}", (object) BlueStacks.Common.Strings.CurrentDefaultVmName);
      HDXapkInstaller.HandleAlreadyRunning();
      Logger.Info("IsAdministrator: {0}", (object) SystemUtils.IsAdministrator());
      Logger.Debug("silentmode = " + opt.s.ToString());
      Logger.Debug("apk = " + opt.xapk);
      Logger.Debug("uninstallmode = " + opt.u.ToString());
      Logger.Debug("vm name =" + BlueStacks.Common.Strings.CurrentDefaultVmName);
      HDXapkInstaller.sXapkHandlerLaunchTime = DateTime.Now;
      Application.EnableVisualStyles();
      ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(HDXapkInstaller.ValidateRemoteCertificate);
      HDXapkInstaller.sXapkPath = opt.xapk;
      if (!string.IsNullOrEmpty(HDXapkInstaller.sXapkPath) && !Path.IsPathRooted(HDXapkInstaller.sXapkPath))
        HDXapkInstaller.sXapkPath = Path.Combine(Directory.GetCurrentDirectory(), HDXapkInstaller.sXapkPath);
      if (!((IEnumerable<string>) RegistryManager.Instance.VmList).Contains<string>(BlueStacks.Common.Strings.CurrentDefaultVmName))
      {
        Logger.Info("VM: " + BlueStacks.Common.Strings.CurrentDefaultVmName + " does not exist");
        Environment.Exit(-8);
      }
      if (opt.u)
        return;
      Logger.Debug("in Install mode");
      if (args.Length >= 1 && HDXapkInstaller.sXapkPath.Equals(""))
      {
        Logger.Debug("ApkHandler called with older types of arguments");
        HDXapkInstaller.sXapkPath = args[0];
        if (!string.IsNullOrEmpty(HDXapkInstaller.sXapkPath) && !Path.IsPathRooted(HDXapkInstaller.sXapkPath))
          HDXapkInstaller.sXapkPath = Path.Combine(Directory.GetCurrentDirectory(), HDXapkInstaller.sXapkPath);
        if (!opt.s && args.Length == 2)
          HDXapkInstaller.sIsSilent = args[1].Equals("silent");
      }
      if (!System.IO.File.Exists(HDXapkInstaller.sXapkPath))
      {
        Logger.Info("Exiting with exit code {0}", (object) 15);
        if (Features.IsFeatureEnabled(8589934592UL))
          HDXapkInstaller.StartLogCollection(15, "File not found", HDXapkInstaller.GetApkNameFromPath(HDXapkInstaller.sXapkPath));
        Environment.Exit(15);
      }
      HDXapkInstaller hdXapkInstaller = new HDXapkInstaller(HDXapkInstaller.sXapkPath, BlueStacks.Common.Strings.CurrentDefaultVmName);
      if (HDXapkInstaller.sIsSilent)
        return;
      Application.Run((Form) hdXapkInstaller);
    }

    private static void HandleAlreadyRunning()
    {
      if (!ProcessUtils.CheckAlreadyRunningAndTakeLock(BlueStacks.Common.Strings.GetHDXapkInstallerLockName(BlueStacks.Common.Strings.CurrentDefaultVmName), out HDXapkInstaller.sHDXapkInstallerLock))
        return;
      Logger.Warning("XapkInstaller already running");
      if (Oem.Instance.IsMessageBoxToBeDisplayed)
      {
        string localizedString = LocaleStrings.GetLocalizedString("STRING_BLUESTACKS_XAPK_HANDLER_TITLE", "");
        HDXapkInstaller hdXapkInstaller = new HDXapkInstaller((string) null, "");
        hdXapkInstaller.Show();
        int num = (int) MessageBox.Show((IWin32Window) hdXapkInstaller, LocaleStrings.GetLocalizedString("STRING_APKINSTALLER_ALREADY_RUNNING", ""), localizedString, MessageBoxButtons.OK, MessageBoxIcon.Hand);
        hdXapkInstaller.Close();
      }
      if (Features.IsFeatureEnabled(8589934592UL))
        HDXapkInstaller.StartLogCollection(2, "PROCESS_ALREADY_RUNNING", "");
      Environment.Exit(2);
    }

    private static bool ValidateRemoteCertificate(
      object sender,
      X509Certificate cert,
      X509Chain chain,
      SslPolicyErrors policyErrors)
    {
      return true;
    }

    private void InstallXapk(object xApk, string vmName)
    {
      string apkPath = (string) xApk;
      Logger.Info("HDXapkInstaller: Installing {0}", (object) apkPath);
      Dictionary<string, string> data = new Dictionary<string, string>();
      data.Add("path", apkPath);
      Dictionary<string, string> headers = new Dictionary<string, string>();
      if (!vmName.Equals("Android"))
      {
        Logger.Info("the vmname is not Android");
        headers.Add("vmid", vmName.Split('_')[1]);
      }
      string json = "";
      try
      {
        if (!ProcessUtils.IsLockInUse("Global\\BlueStacks_HDAgent_Lockbgp"))
        {
          Process process = new Process();
          process.StartInfo.UseShellExecute = false;
          process.StartInfo.CreateNoWindow = true;
          process.StartInfo.FileName = Path.Combine(RegistryStrings.InstallDir, "HD-Agent.exe");
          Logger.Info("Utils: Starting Agent");
          process.Start();
          if (!Utils.WaitForAgentPingResponse(vmName, "bgp"))
          {
            Logger.Info("Exiting with exit code {0}", (object) 11);
            if (Features.IsFeatureEnabled(8589934592UL))
              HDXapkInstaller.StartLogCollection(11, "Agent or agent-server not running", apkPath);
            Environment.Exit(11);
          }
        }
        json = HTTPUtils.SendRequestToAgent("install", data, vmName, 600000, headers, false, 1, 0, "bgp", true);
      }
      catch (WebException ex)
      {
        this.ReleaseApkInstallThread(vmName);
        Logger.Error("WebException in install request");
        Logger.Error(ex.ToString());
        Logger.Error("WebException Response", (object) ex.Response);
        Logger.Info("Exiting with exit code {0}", (object) 9);
        if (Features.IsFeatureEnabled(8589934592UL))
          HDXapkInstaller.StartLogCollection(9, string.Format("status = {0}, error = {1}", (object) ex.Status, (object) ex.Message), apkPath);
        Environment.Exit(9);
      }
      catch (Exception ex)
      {
        this.ReleaseApkInstallThread(vmName);
        Logger.Error("Exception in install request");
        Logger.Error(ex.ToString());
        Logger.Info("Exiting with exit code {0}", (object) 8);
        if (Features.IsFeatureEnabled(8589934592UL))
          HDXapkInstaller.StartLogCollection(8, ex.Message, apkPath);
        Environment.Exit(8);
      }
      string errorReason = string.Empty;
      int num1 = 0;
      try
      {
        JObject jobject = JObject.Parse(JArray.Parse(json)[0].ToString());
        errorReason = jobject["reason"].ToString().Trim().ToUpper();
        num1 = jobject["errorCode"].ToObject<int>();
      }
      catch
      {
        Logger.Error("HDXapkInstaller: Failed to recognize Installer Codes : " + errorReason);
        if (Features.IsFeatureEnabled(8589934592UL))
          HDXapkInstaller.StartLogCollection(5, errorReason, apkPath);
        Logger.Info("Exiting with exit code {0}", (object) 5);
        Environment.Exit(5);
      }
      if (num1 == 18)
      {
        Logger.Error("HDXapkInstaller: Installation failed, disk space insufficient in host");
        Logger.Info("Exiting with exit code {0}", (object) 18);
        Environment.Exit(18);
      }
      if (num1 == 0)
      {
        Logger.Info("HDXapkInstaller: Installation Successful");
        string str = "XApk " + LocaleStrings.GetLocalizedString("STRING_INSTALL_SUCCESS", "");
        Logger.Info("HDXapkInstaller: Exit with code 0");
        Environment.Exit(0);
      }
      else
      {
        Logger.Info("HDXapkInstaller: Installation Failed");
        Logger.Info("HDXapkInstaller: Got Error: {0}", (object) errorReason);
        if (!HDXapkInstaller.sIsSilent)
        {
          string empty = string.Empty;
          int num2 = (int) MessageBox.Show(num1 != 7 ? "XApk " + LocaleStrings.GetLocalizedString("STRING_INSTALL_FAIL", "") + ": " + errorReason : "XApk " + LocaleStrings.GetLocalizedString("STRING_INSTALL_FAIL", "") + ": " + LocaleStrings.GetLocalizedString("STRING_APKINSTALLER_ALREADY_RUNNING", ""), this.Text, MessageBoxButtons.OK, MessageBoxIcon.None);
        }
        if (Features.IsFeatureEnabled(8589934592UL))
        {
          HDXapkInstaller.StartLogCollection(num1, errorReason, apkPath);
          if (num1 == 10)
          {
            int exitCode = -1;
            try
            {
              Utils.AddMessagingSupport(out HDXapkInstaller.sOemWindowMapper);
              Utils.NotifyBootFailureToParentWindow(HDXapkInstaller.sOemWindowMapper[Oem.Instance.OEM][0], HDXapkInstaller.sOemWindowMapper[Oem.Instance.OEM][1], exitCode, vmName);
            }
            catch (Exception ex)
            {
              Logger.Error("caught exception in checking reason for android boot failure ex : {0}", (object) ex.ToString());
            }
          }
        }
        Logger.Info("Exiting with exit code {0}", (object) num1);
        Environment.Exit(num1);
      }
    }

    private void ReleaseApkInstallThread(string vmName)
    {
      if (!ProcessUtils.IsLockInUse("Global\\BlueStacks_HDAgent_Lockbgp"))
        return;
      HTTPUtils.SendRequestToAgent("releaseApkInstallThread", (Dictionary<string, string>) null, vmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp", true);
    }

    private static void StartLogCollection(int errorCode, string errorReason, string apkPath)
    {
      Logger.Info("starting the logging of xapk installation failure");
      Process process = new Process();
      process.StartInfo.FileName = Path.Combine(RegistryStrings.InstallDir, "HD-LogCollector.exe");
      string apkNameFromPath = HDXapkInstaller.GetApkNameFromPath(apkPath);
      string str = "-xapk " + errorCode.ToString() + " " + HDXapkInstaller.AddQuotes(errorReason) + " " + HDXapkInstaller.AddQuotes(apkNameFromPath);
      Logger.Info("The arguments being passed to log collector is :{0}", (object) str);
      process.StartInfo.Arguments = str;
      process.Start();
    }

    private static string AddQuotes(string value)
    {
      return "\"" + value + "\"";
    }

    private static string GetApkNameFromPath(string apkPath)
    {
      int startIndex = apkPath.LastIndexOf('\\') + 1;
      int length = apkPath.Length - startIndex;
      return apkPath.Substring(startIndex, length);
    }

    private static void InitExceptionHandlers()
    {
      Application.ThreadException += (ThreadExceptionEventHandler) ((obj, evt) =>
      {
        HDXapkInstaller.StartLogCollection(-1, "Unhandled Exception:", "");
        Logger.Error("HDXapkInstaller: Unhandled Exception:");
        Logger.Error(evt.Exception.ToString());
        Environment.Exit(-1);
      });
      Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
      AppDomain.CurrentDomain.UnhandledException += (UnhandledExceptionEventHandler) ((obj, evt) =>
      {
        HDXapkInstaller.StartLogCollection(-1, "Unhandled Exception:", "");
        Logger.Error("HDXapkInstaller: Unhandled Exception:");
        Logger.Error(evt.ExceptionObject.ToString());
        Environment.Exit(-1);
      });
    }

    private HDXapkInstaller(string apk, string vmName)
    {
      InteropWindow.FreeConsole();
      if (!HDXapkInstaller.sIsSilent)
        this.InitializeComponents();
      if (apk == null)
        return;
      this.Install(apk, vmName);
    }

    private void Install(string apk, string vmName)
    {
      new Thread((ThreadStart) (() => this.InstallXapk((object) apk, vmName))).Start();
    }

    private void InitializeComponents()
    {
      int height = 70;
      int width = 220;
      this.SuspendLayout();
      this.StartPosition = FormStartPosition.CenterScreen;
      this.Icon = Utils.GetApplicationIcon();
      this.Text = LocaleStrings.GetLocalizedString("STRING_BLUESTACKS_XAPK_HANDLER_TITLE", "");
      this.SizeGripStyle = SizeGripStyle.Hide;
      this.ShowIcon = true;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.ShowInTaskbar = true;
      this.FormBorderStyle = FormBorderStyle.FixedDialog;
      this.ClientSize = new Size(width, height);
      this.mLabel = new Label();
      this.mLabel.Location = new Point(width / 4, 5);
      this.mLabel.Size = new Size(width, 35);
      this.mLabel.Text = LocaleStrings.GetLocalizedString("STRING_USER_WAIT", "");
      this.mProgressBar = new ProgressBar();
      this.mProgressBar.Location = new Point(width / 4, 40);
      this.mProgressBar.Size = new Size(width / 2, 20);
      this.mProgressBar.Style = ProgressBarStyle.Marquee;
      this.mProgressBar.MarqueeAnimationSpeed = 25;
      this.Controls.Add((Control) this.mLabel);
      this.Controls.Add((Control) this.mProgressBar);
      this.ResumeLayout(false);
      this.PerformLayout();
      Logger.Info("HDXapkInstaller: Components Initialized");
    }

    protected override void OnClosing(CancelEventArgs evt)
    {
      Environment.Exit(4);
    }

    public class Opt : GetOpt
    {
      public string xapk = "";
      public string name = "";
      public string p = "";
      public string vmname = "Android";
      public bool s;
      public bool u;
    }
  }
}
