// Decompiled with JetBrains decompiler
// Type: BlueStacks.ApkInstaller.HDApkInstaller
// Assembly: HD-ApkHandler, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: ED0E0699-2F58-4C86-8605-22FA780CB789
// Assembly location: C:\Program Files\BlueStacks\HD-ApkHandler.exe

using BlueStacks.Common;
using Microsoft.Win32;
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
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Windows.Forms;

namespace BlueStacks.ApkInstaller
{
  public class HDApkInstaller : Form
  {
    private static string s_InstallPath = "install";
    private static string s_InstallDir = (string) null;
    private static string s_AppName = "";
    private static string s_AppIcon = "";
    private static string s_AppPackage = "";
    private static string s_ApkPath = "";
    private static bool s_IsSilent = false;
    private static Dictionary<string, string> data = new Dictionary<string, string>();
    private const string logName = "HD-ApkHandler";
    private ProgressBar m_ProgressBar;
    private Label m_Label;
    private static Mutex s_HDApkInstallerLock;
    private const int PROC_KILL_TIMEOUT = 10000;
    private static string unUsed;
    private static int s_AgentPort;
    private static DateTime sApkHandlerLaunchTime;
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
      HDApkInstaller.InitExceptionHandlers();
      args = Utils.AddVmNameInArgsIfNotPresent(args);
      HDApkInstaller.Opt opt = new HDApkInstaller.Opt();
      opt.Parse(args);
      HDApkInstaller.s_IsSilent = opt.s;
      string location = Assembly.GetEntryAssembly().Location;
      Logger.Info("the exe path is " + location);
      Directory.GetParent(location).ToString();
      BlueStacks.Common.Strings.CurrentDefaultVmName = opt.vmname;
      Logger.Info("the installvm is {0}", (object) BlueStacks.Common.Strings.CurrentDefaultVmName);
      HDApkInstaller.HandleAlreadyRunning();
      Logger.Info("IsAdministrator: {0}", (object) SystemUtils.IsAdministrator());
      Logger.Debug("pkg name = " + opt.p);
      Logger.Debug("app name = " + opt.name);
      Logger.Debug("silentmode = " + opt.s.ToString());
      Logger.Debug("apk = " + opt.apk);
      Logger.Debug("uninstallmode = " + opt.u.ToString());
      Logger.Debug("vm name =" + BlueStacks.Common.Strings.CurrentDefaultVmName);
      HDApkInstaller.sApkHandlerLaunchTime = DateTime.Now;
      Application.EnableVisualStyles();
      ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(HDApkInstaller.ValidateRemoteCertificate);
      HDApkInstaller.s_AppName = opt.name;
      HDApkInstaller.s_AppPackage = opt.p;
      HDApkInstaller.s_ApkPath = opt.apk;
      if (!string.IsNullOrEmpty(HDApkInstaller.s_ApkPath) && !Path.IsPathRooted(HDApkInstaller.s_ApkPath))
        HDApkInstaller.s_ApkPath = Path.Combine(Directory.GetCurrentDirectory(), HDApkInstaller.s_ApkPath);
      if (!((IEnumerable<string>) RegistryManager.Instance.VmList).Contains<string>(BlueStacks.Common.Strings.CurrentDefaultVmName))
      {
        Logger.Info("VM: " + BlueStacks.Common.Strings.CurrentDefaultVmName + " does not exist");
        Environment.Exit(-8);
      }
      if (!opt.u)
      {
        Logger.Debug("in Install mode");
        if (args.Length >= 1 && HDApkInstaller.s_ApkPath.Equals(""))
        {
          Logger.Debug("ApkHandler called with older types of arguments");
          HDApkInstaller.s_ApkPath = args[0];
          if (!string.IsNullOrEmpty(HDApkInstaller.s_ApkPath) && !Path.IsPathRooted(HDApkInstaller.s_ApkPath))
            HDApkInstaller.s_ApkPath = Path.Combine(Directory.GetCurrentDirectory(), HDApkInstaller.s_ApkPath);
          if (!opt.s && args.Length == 2)
            HDApkInstaller.s_IsSilent = args[1].Equals("silent");
        }
        if (!System.IO.File.Exists(HDApkInstaller.s_ApkPath))
        {
          Logger.Info("Exiting with exit code {0}", (object) 15);
          if (Features.IsFeatureEnabled(8589934592UL))
            HDApkInstaller.StartLogCollection(15, "File not found", HDApkInstaller.GetApkNameFromPath(HDApkInstaller.s_ApkPath));
          Environment.Exit(15);
        }
        HDApkInstaller hdApkInstaller = new HDApkInstaller(HDApkInstaller.s_ApkPath, BlueStacks.Common.Strings.CurrentDefaultVmName);
        if (HDApkInstaller.s_IsSilent)
          return;
        Application.Run((Form) hdApkInstaller);
      }
      else
      {
        try
        {
          HDApkInstaller.s_AgentPort = RegistryManager.Instance.AgentServerPort;
          JsonParser jsonParser = new JsonParser(BlueStacks.Common.Strings.CurrentDefaultVmName);
          if (HDApkInstaller.s_AppPackage != "")
            jsonParser.GetAppInfoFromPackageName(HDApkInstaller.s_AppPackage, out HDApkInstaller.s_AppName, out HDApkInstaller.s_AppIcon, out HDApkInstaller.unUsed, out HDApkInstaller.unUsed);
          else if (HDApkInstaller.s_AppName != "")
          {
            jsonParser.GetAppInfoFromAppName(HDApkInstaller.s_AppName, out HDApkInstaller.s_AppPackage, out HDApkInstaller.s_AppIcon, out HDApkInstaller.unUsed);
            HDApkInstaller.CleanUpUninstallEntry();
          }
          if (string.IsNullOrEmpty(HDApkInstaller.s_AppPackage))
          {
            Logger.Error("PackageName can not be null for uninstalling an app");
          }
          else
          {
            int exitCode = 0;
            if (jsonParser.IsPackageNameSystemApp(HDApkInstaller.s_AppPackage))
            {
              int num = (int) MessageBox.Show("Uninstalling a pre-bundled app is not supported.", string.Format("{0} Error", (object) BlueStacks.Common.Strings.ProductDisplayName), MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            else
            {
              HDApkInstaller.data.Clear();
              HDApkInstaller.data.Add("package", HDApkInstaller.s_AppPackage);
              HDApkInstaller.data.Add("name", HDApkInstaller.s_AppName);
              exitCode = !(JArray.Parse(HTTPUtils.SendRequestToAgent("uninstall", HDApkInstaller.data, BlueStacks.Common.Strings.CurrentDefaultVmName, 0, (Dictionary<string, string>) null, false, 10, 500, "bgp", true))[0] as JObject)["success"].ToObject<bool>() ? 1 : 0;
            }
            Environment.Exit(exitCode);
          }
        }
        catch (Exception ex)
        {
          Logger.Error("Got Exception");
          Logger.Error(ex.ToString());
          HDApkInstaller.CleanUpUninstallEntry();
        }
      }
    }

    private static void HandleAlreadyRunning()
    {
      if (!ProcessUtils.CheckAlreadyRunningAndTakeLock(BlueStacks.Common.Strings.GetHDApkInstallerLockName(BlueStacks.Common.Strings.CurrentDefaultVmName), out HDApkInstaller.s_HDApkInstallerLock))
        return;
      Logger.Info("ApkInstaller already running");
      if (Oem.Instance.IsMessageBoxToBeDisplayed)
      {
        string stacksApkHandlerTitle = Oem.Instance.BlueStacksApkHandlerTitle;
        HDApkInstaller hdApkInstaller = new HDApkInstaller((string) null, "");
        hdApkInstaller.Show();
        int num = (int) MessageBox.Show((IWin32Window) hdApkInstaller, LocaleStrings.GetLocalizedString("STRING_APKINSTALLER_ALREADY_RUNNING", ""), stacksApkHandlerTitle, MessageBoxButtons.OK, MessageBoxIcon.Hand);
        hdApkInstaller.Close();
      }
      Logger.Info("HD-ApkHandler already running");
      if (Features.IsFeatureEnabled(8589934592UL))
        HDApkInstaller.StartLogCollection(2, "PROCESS_ALREADY_RUNNING", "");
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

    public static void CleanUpUninstallEntry()
    {
      bool flag = false;
      string appName = HDApkInstaller.s_AppName;
      string subkey = "Bst-" + appName;
      Logger.Info("Cleaning up uninstall entry for {0}", (object) appName);
      RegistryKey subKey = Registry.LocalMachine.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall");
      try
      {
        string name = "Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\" + subkey;
        if ((string) Registry.LocalMachine.OpenSubKey(name).GetValue("Silent") == "yes")
          flag = true;
      }
      catch (Exception ex)
      {
      }
      Logger.Info("Key: " + subKey.ToString());
      subKey.DeleteSubKeyTree(subkey);
      subKey.Close();
      string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
      string path2_1 = appName + ".lnk";
      string path2_2 = path2_1;
      string path1 = Path.Combine(folderPath, path2_2);
      string path2 = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu), "Apps"), path2_1);
      try
      {
        Logger.Info("Deleting shortcut file: " + path1);
        System.IO.File.Delete(path1);
        Logger.Info("Deleting shortcut file: " + path2);
        System.IO.File.Delete(path2);
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to remove shortcut entry. err: " + ex.ToString());
      }
      if (flag)
        return;
      int num = (int) MessageBox.Show(appName + " has been uninstalled.", "App Player", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
    }

    private void InstallApk(object apk, string vmName)
    {
      string apkPath = (string) apk;
      HDApkInstaller.s_InstallDir = RegistryStrings.InstallDir;
      Logger.Info("HDApkInstaller: Installing {0}", (object) apkPath);
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
          process.StartInfo.FileName = Path.Combine(HDApkInstaller.s_InstallDir, "HD-Agent.exe");
          Logger.Info("Utils: Starting Agent");
          process.Start();
          if (!Utils.WaitForAgentPingResponse(vmName, "bgp"))
          {
            Logger.Info("Exiting with exit code {0}", (object) 11);
            if (Features.IsFeatureEnabled(8589934592UL))
              HDApkInstaller.StartLogCollection(11, "Agent or agent-server not running", apkPath);
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
          HDApkInstaller.StartLogCollection(9, string.Format("status = {0}, error = {1}", (object) ex.Status, (object) ex.Message), apkPath);
        Environment.Exit(9);
      }
      catch (Exception ex)
      {
        this.ReleaseApkInstallThread(vmName);
        Logger.Error("Exception in install request");
        Logger.Error(ex.ToString());
        Logger.Info("Exiting with exit code {0}", (object) 8);
        if (Features.IsFeatureEnabled(8589934592UL))
          HDApkInstaller.StartLogCollection(8, ex.Message, apkPath);
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
        Logger.Error("HDApkInstaller: Failed to recognize Installer Codes : " + errorReason);
        if (Features.IsFeatureEnabled(8589934592UL))
          HDApkInstaller.StartLogCollection(5, errorReason, apkPath);
        Logger.Info("Exiting with exit code {0}", (object) 5);
        Environment.Exit(5);
      }
      if (num1 == 18)
      {
        Logger.Error("HDApkInstaller: Installation failed, disk space insufficient in host");
        Logger.Info("Exiting with exit code {0}", (object) 18);
        Environment.Exit(num1);
      }
      if (num1 == 0)
      {
        Logger.Info("HDApkInstaller: Installation Successful");
        string str = "Apk " + LocaleStrings.GetLocalizedString("STRING_INSTALL_SUCCESS", "");
        Logger.Info("HDApkInstaller: Exit with code 0");
        Environment.Exit(0);
      }
      else
      {
        Logger.Info("HDApkInstaller: Installation Failed");
        Logger.Info("HDApkInstaller: Got Error: {0}", (object) errorReason);
        if (!HDApkInstaller.s_IsSilent)
        {
          string empty = string.Empty;
          int num2 = (int) MessageBox.Show(num1 != 7 ? "Apk " + LocaleStrings.GetLocalizedString("STRING_INSTALL_FAIL", "") + ": " + errorReason : "Apk " + LocaleStrings.GetLocalizedString("STRING_INSTALL_FAIL", "") + ": " + LocaleStrings.GetLocalizedString("STRING_APKINSTALLER_ALREADY_RUNNING", ""), this.Text, MessageBoxButtons.OK, MessageBoxIcon.None);
        }
        if (Features.IsFeatureEnabled(8589934592UL))
        {
          HDApkInstaller.StartLogCollection(num1, errorReason, apkPath);
          if (num1 == 10)
          {
            int exitCode = -1;
            try
            {
              Utils.AddMessagingSupport(out HDApkInstaller.sOemWindowMapper);
              Utils.NotifyBootFailureToParentWindow(HDApkInstaller.sOemWindowMapper[Oem.Instance.OEM][0], HDApkInstaller.sOemWindowMapper[Oem.Instance.OEM][1], exitCode, vmName);
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
      Logger.Info("starting the logging of apk installation failure");
      Process process = new Process();
      process.StartInfo.FileName = Path.Combine(RegistryStrings.InstallDir, "HD-LogCollector.exe");
      string apkNameFromPath = HDApkInstaller.GetApkNameFromPath(apkPath);
      string str = "-apk " + errorCode.ToString() + " " + HDApkInstaller.AddQuotes(errorReason) + " " + HDApkInstaller.AddQuotes(apkNameFromPath);
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
        HDApkInstaller.StartLogCollection(-1, "Unhandled Exception:", "");
        Logger.Error("HDApkInstaller: Unhandled Exception:");
        Logger.Error(evt.Exception.ToString());
        Environment.Exit(-1);
      });
      Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
      AppDomain.CurrentDomain.UnhandledException += (UnhandledExceptionEventHandler) ((obj, evt) =>
      {
        HDApkInstaller.StartLogCollection(-1, "Unhandled Exception:", "");
        Logger.Error("HDApkInstaller: Unhandled Exception:");
        Logger.Error(evt.ExceptionObject.ToString());
        Environment.Exit(-1);
      });
    }

    private HDApkInstaller(string apk, string vmName)
    {
      InteropWindow.FreeConsole();
      if (!HDApkInstaller.s_IsSilent)
        this.InitializeComponents();
      if (apk == null)
        return;
      this.Install(apk, vmName);
    }

    private void Install(string apk, string vmName)
    {
      new Thread((ThreadStart) (() => this.InstallApk((object) apk, vmName))).Start();
    }

    private void InitializeComponents()
    {
      int height = 70;
      int width = 220;
      this.SuspendLayout();
      this.StartPosition = FormStartPosition.CenterScreen;
      this.Icon = Utils.GetApplicationIcon();
      this.Text = Oem.Instance.BlueStacksApkHandlerTitle;
      this.SizeGripStyle = SizeGripStyle.Hide;
      this.ShowIcon = true;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.ShowInTaskbar = true;
      this.FormBorderStyle = FormBorderStyle.FixedDialog;
      this.ClientSize = new Size(width, height);
      this.m_Label = new Label();
      this.m_Label.Location = new Point(width / 4, 5);
      this.m_Label.Size = new Size(width, 35);
      this.m_Label.Text = LocaleStrings.GetLocalizedString("STRING_USER_WAIT", "");
      this.m_ProgressBar = new ProgressBar();
      this.m_ProgressBar.Location = new Point(width / 4, 40);
      this.m_ProgressBar.Size = new Size(width / 2, 20);
      this.m_ProgressBar.Style = ProgressBarStyle.Marquee;
      this.m_ProgressBar.MarqueeAnimationSpeed = 25;
      this.Controls.Add((Control) this.m_Label);
      this.Controls.Add((Control) this.m_ProgressBar);
      this.ResumeLayout(false);
      this.PerformLayout();
      Logger.Info("HDApkInstaller: Components Initialized");
    }

    protected override void OnClosing(CancelEventArgs evt)
    {
      Environment.Exit(4);
    }

    public class Opt : GetOpt
    {
      public string apk = "";
      public string name = "";
      public string p = "";
      public string vmname = "Android";
      public bool s;
      public bool u;
    }
  }
}
