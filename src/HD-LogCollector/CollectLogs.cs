// Decompiled with JetBrains decompiler
// Type: BlueStacks.LogCollector.CollectLogs
// Assembly: HD-LogCollector, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: FD44426D-0F78-46B3-8118-1E64B979C51B
// Assembly location: C:\Program Files\BlueStacks\HD-LogCollector.exe

using BlueStacks.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Forms;

namespace BlueStacks.LogCollector
{
  internal class CollectLogs : IDisposable
  {
    private static int m_OemLogsToComplete = 0;
    public static ProgressWindow sProgressWindow = (ProgressWindow) null;
    private static string s_DestinationFolder = Path.GetTempPath();
    private static bool s_UploadZip = true;
    private static bool s_ShowUI = true;
    public static bool s_StartLogcat = false;
    public static bool s_ThinCollector = false;
    public static bool s_SilentCollector = false;
    public static bool s_ApkInstallFailureLogCollector = false;
    public static string s_InstallFailedApkName = "";
    public static bool s_BootFailureLogs = false;
    public static bool s_CrashLogs = false;
    private Dictionary<string, DirectoryInfo> m_OemDir = new Dictionary<string, DirectoryInfo>();
    private readonly List<BackgroundWorker> mGuestHostInfoCollectors = new List<BackgroundWorker>();
    private readonly Dictionary<string, bool> mIsGuestHostInfoAvailableDict = new Dictionary<string, bool>();
    private List<string> mOemToCloseAfterLogCollection = new List<string>();
    private ManualResetEvent mIsExtraInfoDumpedEvent = new ManualResetEvent(false);
    private string mSource = "";
    private Dictionary<string, List<string>> mRunningProcessAndFilenamesDict = new Dictionary<string, List<string>>();
    private SynchronizationContext mUiContext;
    private DirectoryInfo mTempLogsDir;
    private TextWriter mOrigOut;
    private const int PROGRESS_DEFINITE_COUNT = 9;
    private string mEmail;
    private string mCategory;
    private ICollection<string> mAttachment;
    private string mAppName;
    private string mAppPkgName;
    private string mErrorDetails;
    private string mDescription;
    private string mErrorReason;
    private string mErrorCode;
    private string mSubcategory;
    private bool mCollectLogsFromAllOems;
    private bool mStartAllOems;
    private bool mUploadSuccessful;
    private bool disposedValue;

    [DllImport("USER32.DLL")]
    public static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern IntPtr FindWindow(string sClass, string sWindow);

    public CollectLogs(
      string dest,
      bool uploadZip,
      bool collectLogsFromAllOems,
      bool startAllOems)
    {
      CollectLogs.s_DestinationFolder = dest;
      CollectLogs.s_UploadZip = uploadZip;
      this.mCollectLogsFromAllOems = collectLogsFromAllOems;
      CollectLogs.m_OemLogsToComplete = this.mCollectLogsFromAllOems ? App.sOemApplicableForLogCollection.Count : 1;
      this.mStartAllOems = startAllOems;
      this.SendStats("LogCollector", "SilentMode", this.mCollectLogsFromAllOems ? "AllOemsChecked" : "AllOemsUnChecked");
      this.SendStats("LogCollector", "SilentMode", this.mStartAllOems ? "StartAllOemsChecked" : "StartAllOemsUnChecked");
      this.CreateTempLogsDir();
      this.EnableDebugPrivilege();
      CollectLogs.LogDirectoryPaths("bgp");
    }

    public CollectLogs(
      string dest,
      bool uploadZip,
      string errorReason,
      string errorCode,
      bool collectLogsFromAllOems,
      bool startAllOems)
      : this(dest, uploadZip, collectLogsFromAllOems, startAllOems)
    {
      this.mErrorReason = errorReason;
      this.mErrorCode = errorCode;
    }

    public CollectLogs(
      string dest,
      bool uploadZip,
      string errorReason,
      string errorDetails,
      string errorCode,
      bool collectLogsFromAllOems,
      bool startAllOems)
      : this(dest, uploadZip, errorReason, errorCode, collectLogsFromAllOems, startAllOems)
    {
      this.mErrorDetails = errorDetails;
    }

    public CollectLogs(
      string email,
      string category,
      string appName,
      string appPkgName,
      string desc,
      string subcategory,
      ICollection<string> attachment,
      bool collectLogsFromAllOems,
      bool startAllOems)
    {
      this.mEmail = email;
      this.mCategory = category;
      this.mAppName = appName;
      this.mAppPkgName = appPkgName;
      this.mDescription = desc;
      this.mSubcategory = subcategory;
      this.mAttachment = attachment;
      this.mUiContext = SynchronizationContext.Current;
      this.mCollectLogsFromAllOems = collectLogsFromAllOems;
      CollectLogs.m_OemLogsToComplete = this.mCollectLogsFromAllOems ? App.sOemApplicableForLogCollection.Count : 1;
      this.mStartAllOems = startAllOems;
      CollectLogs.sProgressWindow = new ProgressWindow();
      CollectLogs.sProgressWindow.mProgressText.Text = LocaleStrings.GetLocalizedString("STRING_STATUS_INITIAL", "");
      CollectLogs.sProgressWindow.mProgressBar.Value = 0.0;
      CollectLogs.sProgressWindow.mProgressBar.Maximum = (double) (9 + CollectLogs.m_OemLogsToComplete * 4);
      this.CreateTempLogsDir();
      this.OpenLog();
      this.EnableDebugPrivilege();
      CollectLogs.UpdateProgress();
      new Thread(new ThreadStart(this.DumpExtraInfo))
      {
        IsBackground = true
      }.Start();
      new Thread(new ThreadStart(this.CollectArtifactsForOemAndVm))
      {
        IsBackground = true
      }.Start();
      this.SendStats("LogCollector", "UiMode", this.mCollectLogsFromAllOems ? "AllOemsChecked" : "AllOemsUnChecked");
      this.SendStats("LogCollector", "UiMode", this.mStartAllOems ? "StartAllOemsChecked" : "StartAllOemsUnChecked");
    }

    private void GetRunningProcessesFileNames(List<string> pNames)
    {
      foreach (string pName in pNames)
      {
        try
        {
          this.mRunningProcessAndFilenamesDict.Add(pName, ((IEnumerable<Process>) Process.GetProcessesByName(pName)).Select<Process, string>((Func<Process, string>) (process => process.MainModule.FileName)).ToList<string>());
        }
        catch (Exception ex)
        {
          Logger.Error(string.Format("Could not get processes: {0} Ex:{1}", (object) pName, (object) ex));
        }
      }
    }

    private void KillProcesses()
    {
      foreach (KeyValuePair<string, List<string>> keyValuePair in this.mRunningProcessAndFilenamesDict)
      {
        KeyValuePair<string, List<string>> processAndFilenames = keyValuePair;
        Logger.Info("Kill all processes not already running: " + processAndFilenames.Key);
        foreach (Process process in ((IEnumerable<Process>) Process.GetProcessesByName(processAndFilenames.Key)).Where<Process>((Func<Process, bool>) (process => !this.mRunningProcessAndFilenamesDict[processAndFilenames.Key].Contains(process.MainModule.FileName))))
        {
          try
          {
            process.Kill();
          }
          catch (Exception ex)
          {
            Logger.Error(string.Format("Could not kill process {0} Ex:{1}", (object) process.MainModule.FileName, (object) ex));
          }
        }
      }
    }

    public void StartSilentLogCollection(bool showUI)
    {
      CollectLogs.s_ShowUI = showUI;
      StreamWriter writer = this.OpenLog();
      this.DumpExtraInfo();
      this.StartSilentLogCollectionOemVm("bgp", MultiInstanceStrings.VmName);
      if (this.mCollectLogsFromAllOems)
      {
        foreach (string oem in App.sOemApplicableForLogCollection.Where<string>((Func<string, bool>) (oem => !string.Equals(oem, "bgp", StringComparison.InvariantCultureIgnoreCase))))
          this.StartSilentLogCollectionOemVm(oem, "Android");
      }
      this.CloseWriterAndDeleteTempDir(writer);
    }

    private void StartSilentLogCollectionOemVm(string oem, string vm)
    {
      this.m_OemDir.Add(oem, string.Equals(oem, "bgp", StringComparison.InvariantCultureIgnoreCase) ? this.mTempLogsDir : this.mTempLogsDir.CreateSubdirectory(oem));
      this.CollectGuestHostInfo(oem, vm);
      this.DoAfterLogCollection(oem, vm);
    }

    private void CollectGuestHostInfo(string oem, string vm)
    {
      this.mIsGuestHostInfoAvailableDict.Add(oem, false);
      this.CollectGuestArtifacts(oem, vm);
      this.CollectHostArtifacts(oem, vm);
      this.mIsGuestHostInfoAvailableDict[oem] = true;
    }

    private void CollectArtifactsForOemAndVm()
    {
      this.GetRunningProcessesFileNames(new List<string>()
      {
        "HD-Adb"
      });
      this.CollectArtifacts("bgp", MultiInstanceStrings.VmName);
      if (!this.mCollectLogsFromAllOems)
        return;
      foreach (string oem in App.sOemApplicableForLogCollection.Where<string>((Func<string, bool>) (oem => !string.Equals(oem, "bgp", StringComparison.InvariantCultureIgnoreCase))))
        this.CollectArtifacts(oem, "Android");
    }

    private void CollectArtifacts(string oem, string vmName)
    {
      this.m_OemDir.Add(oem, string.Equals(oem, "bgp", StringComparison.InvariantCultureIgnoreCase) ? this.mTempLogsDir : this.mTempLogsDir.CreateSubdirectory(oem));
      List<string> stringList = new List<string>()
      {
        oem,
        vmName
      };
      BackgroundWorker backgroundWorker = new BackgroundWorker()
      {
        WorkerSupportsCancellation = true
      };
      backgroundWorker.DoWork += new DoWorkEventHandler(this.CollectGuestHostArtifactsDoWork);
      backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.DoAfterLogCollectionCompleted);
      backgroundWorker.RunWorkerAsync((object) stringList);
      this.mGuestHostInfoCollectors.Add(backgroundWorker);
    }

    private void CollectGuestHostArtifactsDoWork(object sender, DoWorkEventArgs doWorkEventArgs)
    {
      if (doWorkEventArgs == null || !(doWorkEventArgs.Argument is List<string> stringList))
        return;
      doWorkEventArgs.Result = doWorkEventArgs.Argument;
      this.CollectGuestHostInfo(stringList[0], stringList[1]);
    }

    private void DoAfterLogCollectionCompleted(
      object sender,
      RunWorkerCompletedEventArgs workerCompletedArgs)
    {
      if (!(workerCompletedArgs.Result is List<string> result))
        return;
      this.DoAfterLogCollection(result[0], result[1]);
    }

    private void DoAfterLogCollection(string oem, string vmName)
    {
      if (!this.mIsGuestHostInfoAvailableDict[oem])
      {
        Console.WriteLine(string.Format("Guest & Host Info Available = {0}, ::::::: {1}", (object) this.mIsGuestHostInfoAvailableDict[oem], (object) DateTime.Now));
      }
      else
      {
        --CollectLogs.m_OemLogsToComplete;
        if (CollectLogs.m_OemLogsToComplete > 0)
          return;
        if (CollectLogs.s_ShowUI)
          CollectLogs.UpdateProgress();
        this.mIsExtraInfoDumpedEvent.WaitOne();
        this.KillProcesses();
        this.CloseLog();
        try
        {
          CollectLogs.UpdateMessage(LocaleStrings.GetLocalizedString("STRING_STATUS_ARCHIVING", ""));
          LogHelper.CreateZipFile(this.mTempLogsDir, CollectLogs.s_DestinationFolder, RegistryStrings.InstallDir);
        }
        catch (Exception ex)
        {
          Logger.Error("Exception in createzipfile");
          Logger.Error(ex.ToString());
        }
        if (CollectLogs.s_ShowUI)
          CollectLogs.UpdateProgress();
        if (CollectLogs.s_UploadZip)
        {
          if (CollectLogs.s_ShowUI)
            this.UploadZipFile(this.mEmail, this.mCategory, this.mAppName, this.mAppPkgName, this.mDescription, this.mSubcategory, this.mSource, oem, vmName);
          else if (CollectLogs.s_ApkInstallFailureLogCollector)
          {
            string category = "apk install failure";
            string desc = this.mErrorCode + "-->" + this.mErrorReason;
            string subcategory = "";
            this.UploadZipFile("developer@bluestacks.com", category, CollectLogs.s_InstallFailedApkName, (string) null, desc, subcategory, this.mSource, oem, vmName);
          }
          else
          {
            string category = "";
            string desc = "silent debug logs";
            if (CollectLogs.s_ThinCollector)
            {
              category = "ThinLogCollector Logs";
              desc = "silent thin debug logs";
            }
            else if (CollectLogs.s_BootFailureLogs)
            {
              category = "Boot Failure";
              desc = this.mErrorReason;
            }
            else if (CollectLogs.s_SilentCollector)
            {
              category = "Silent Log Collection";
              desc = "silent debug logs";
            }
            string subcategory = "";
            this.UploadZipFile("developer@bluestacks.com", category, "", (string) null, desc, subcategory, this.mSource, oem, vmName);
          }
        }
        if (!CollectLogs.s_ShowUI)
          return;
        CollectLogs.UpdateProgress();
        this.ShowFinish();
      }
    }

    private void CollectGuestArtifacts(string oem, string vmName)
    {
      Console.WriteLine("Starting CollectGuestArtifacts  :::::::  {0}", (object) DateTime.Now);
      if (this.StartAndroidPlusHost(vmName, oem))
      {
        this.CollectGuestLogs(oem, vmName);
        foreach (string vm in RegistryManager.RegistryManagers[oem].VmList)
        {
          if (!string.Equals(vm, vmName, StringComparison.InvariantCultureIgnoreCase) && Utils.IsUIProcessAlive(vm, oem) && (Utils.IsAndroidPlayerRunning(vm, oem) && Utils.IsGuestBooted(vm, oem)))
            this.CollectGuestLogs(oem, vm);
        }
      }
      else
        Console.WriteLine("Android service not running, Android-dump State logs will not be created, returing  :::::::  {0}", (object) DateTime.Now);
    }

    private void CollectGuestLogs(string oem, string vmName)
    {
      CollectLogs.UpdateMessage(LocaleStrings.GetLocalizedString("STRING_STATUS_COLLECTING_GUEST", ""));
      CollectLogs.UpdateProgress();
      string str1 = string.Format((IFormatProvider) CultureInfo.CurrentCulture, "127.0.0.1:{0}", (object) RegistryManager.RegistryManagers[oem].Guest[vmName].BstAdbPort);
      string prog = Path.Combine(RegistryManager.RegistryManagers[oem].InstallDir, "HD-Adb.exe");
      Console.WriteLine("Fetching Guest Information  :::::::  {0}", (object) DateTime.Now);
      try
      {
        Logger.Info("Checking sslConnection OEM:" + oem + ", Vmname:" + vmName);
        LogHelper.RunCmdWithList(prog, new string[2]
        {
          "connect",
          str1
        });
        string guest = HTTPUtils.SendRequestToGuest("checkSSLConnection", (Dictionary<string, string>) null, vmName, 10000, (Dictionary<string, string>) null, false, 1, 0, oem);
        File.AppendAllText(Path.Combine(this.m_OemDir[oem].FullName, "CheckSslConnection.txt"), guest);
        Logger.Info("Checked sslConnection OEM:" + oem + ", Vmname:" + vmName);
      }
      catch (Exception ex)
      {
        Console.WriteLine("Error while checking ssl connection " + ex.ToString());
      }
      if (CollectLogs.s_StartLogcat)
      {
        LogHelper.RunCmdWithList(prog, new string[6]
        {
          "-s",
          str1,
          "shell",
          "/system/xbin/bstk/su",
          "-c",
          "stop"
        }, Path.Combine(this.m_OemDir[oem].FullName, "stop.txt"));
        LogHelper.RunCmdWithList(prog, new string[7]
        {
          "-s",
          str1,
          "shell",
          "/system/xbin/bstk/su",
          "-c",
          "logcat",
          "-c"
        }, (string) null);
        LogHelper.RunCmdWithList(prog, new string[7]
        {
          "-s",
          str1,
          "shell",
          "/system/xbin/bstk/su",
          "-c",
          "start",
          "logcat"
        }, (string) null);
        LogHelper.RunCmdWithList(prog, new string[6]
        {
          "-s",
          str1,
          "shell",
          "/system/xbin/bstk/su",
          "-c",
          "start"
        }, Path.Combine(this.m_OemDir[oem].FullName, "start.txt"));
        Thread.Sleep(30000);
      }
      CultureInfo currentCulture = CultureInfo.CurrentCulture;
      DateTime now = DateTime.Now;
      // ISSUE: variable of a boxed type
      __Boxed<int> minute = (ValueType) now.Minute;
      now = DateTime.Now;
      // ISSUE: variable of a boxed type
      __Boxed<int> second = (ValueType) now.Second;
      string format = string.Format("Dumping Android Logs at {0}:{1}", (object) minute, (object) second);
      object[] objArray = new object[0];
      Console.WriteLine(string.Format((IFormatProvider) currentCulture, format, objArray) + string.Format("  ::::::: {0}", (object) DateTime.Now));
      string str2 = Path.Combine(this.m_OemDir[oem].FullName, vmName + "-DumpState.log");
      LogHelper.RunCmdWithList(prog, new string[4]
      {
        "-s",
        str1,
        "shell",
        "bugreport"
      }, str2);
      if (File.Exists(str2) && new FileInfo(str2).Length < 10240L)
      {
        Console.WriteLine("Dumpstate file size less than 10KB, kill server and start server and collect dumpstate  ::::::: {0}", (object) DateTime.Now);
        LogHelper.RunCmd(prog, "kill-server", (string) null);
        Console.WriteLine("kill-server done  ::::::: {0}", (object) DateTime.Now);
        LogHelper.RunCmd(prog, "start-server", (string) null);
        Console.WriteLine("start-server done  ::::::: {0}", (object) DateTime.Now);
        LogHelper.RunCmdWithList(prog, new string[2]
        {
          "connect",
          str1
        });
        LogHelper.RunCmdWithList(prog, new string[4]
        {
          "-s",
          str1,
          "shell",
          "dumpstate"
        }, str2);
      }
      if (!CollectLogs.s_ApkInstallFailureLogCollector && !CollectLogs.s_BootFailureLogs)
      {
        LogHelper.RunCmdWithList(prog, new string[7]
        {
          "-s",
          str1,
          "shell",
          "wget",
          "-O",
          "/dev/null",
          "http://www.google.com"
        }, Path.Combine(this.m_OemDir[oem].FullName, string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}_wget.txt", (object) vmName)));
        LogHelper.RunCmdWithList(prog, new string[5]
        {
          "-s",
          str1,
          "pull",
          "/data/anr/traces.txt.bugreport",
          Path.Combine(this.m_OemDir[oem].FullName, string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}_anr-traces.txt.bugreport", (object) vmName))
        }, Path.Combine(this.m_OemDir[oem].FullName, string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}_anr-out.txt", (object) vmName)));
        if (Utils.IsUIProcessAlive(vmName, oem))
          LogHelper.RunCmdWithList(prog, new string[5]
          {
            "-s",
            str1,
            "pull",
            "/data/anr/problemreport.png",
            Path.Combine(this.m_OemDir[oem].FullName, string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}_screenshot.png", (object) vmName))
          }, (string) null);
      }
      int num = 3;
      string path2 = "dummy.db";
      for (; num > 0; --num)
      {
        string str3 = "/data/downloads/";
        switch (num - 1)
        {
          case 0:
            str3 = string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}config.db", (object) str3);
            path2 = "config.db";
            break;
          case 1:
            str3 = string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}.config.db", (object) str3);
            path2 = ".config.db";
            break;
          case 2:
            str3 = string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}.config_user.db", (object) str3);
            path2 = ".config_user.db";
            break;
        }
        LogHelper.RunCmdWithList(prog, new string[5]
        {
          "-s",
          str1,
          "pull",
          str3,
          Path.Combine(this.m_OemDir[oem].FullName, path2)
        }, (string) null);
        if (File.Exists(Path.Combine(this.m_OemDir[oem].FullName, path2)))
          break;
      }
      Console.WriteLine("Done CollectGuestArtifacts  :::::::  {0}", (object) DateTime.Now);
    }

    private void CollectHostArtifacts(string oem, string vmName)
    {
      Console.WriteLine("Starting CollectHostArtifacts  :::::::  {0}", (object) DateTime.Now);
      CollectLogs.UpdateMessage(LocaleStrings.GetLocalizedString("STRING_STATUS_COLLECTING_PRODUCT", ""));
      Console.WriteLine("Copying Product Logs  :::::::  {0}", (object) DateTime.Now);
      LogHelper.DumpServiceInfo();
      if (Utils.IsUIProcessAlive(vmName, oem))
        CollectLogs.DumpPendingGlCalls();
      if (this.mOemToCloseAfterLogCollection.Contains(oem))
      {
        try
        {
          Logger.Info("Run HD-Quit for Oem: " + oem);
          Utils.RunHDQuit(false, false, false, oem);
          Logger.Info("Closed instance OEM:" + oem + " VmName:" + vmName);
        }
        catch (Exception ex)
        {
          Logger.Error(string.Format("Could not close oem {0}: {1}", (object) oem, (object) ex));
        }
      }
      try
      {
        LogHelper.CopyRecursive(RegistryManager.RegistryManagers[oem].LogDir, Path.Combine(this.m_OemDir[oem].FullName, "Logs"));
        Console.WriteLine("Engine logs copied successfully");
      }
      catch (Exception ex)
      {
        Console.WriteLine("Failed to copy BlueStacks logs");
        Console.WriteLine(ex.ToString());
      }
      try
      {
        string str = Path.Combine(RegistryManager.RegistryManagers[oem].DataDir, "Oem.cfg");
        if (File.Exists(str))
        {
          File.Copy(str, Path.Combine(this.m_OemDir[oem].FullName, "Oem.cfg"));
          Console.WriteLine("Oem.cfg file successfully copied from {0}", (object) str);
        }
        else
          Console.WriteLine("Oem config file does not exists at {0}", (object) str);
      }
      catch (Exception ex)
      {
        Console.WriteLine("Failed to copy oem.cfg file");
        Console.WriteLine(ex.ToString());
      }
      LogHelper.CollectVmLogs(this.m_OemDir[oem], oem);
      if (!CollectLogs.s_BootFailureLogs)
        LogHelper.CopyAllAppsJsonFiles(this.m_OemDir[oem], oem);
      try
      {
        LogHelper.RunCmd("reg.exe", string.Format((IFormatProvider) CultureInfo.CurrentCulture, "EXPORT HKLM\\{0} \"{1}\"", (object) RegistryManager.RegistryManagers[oem].BaseKeyPath, (object) Path.Combine(this.m_OemDir[oem].FullName, "RegHKLM.txt")), (string) null);
      }
      catch (Exception ex)
      {
        Logger.Error(ex.ToString());
      }
      try
      {
        Console.WriteLine("Dumping File listing  :::::::  {0}", (object) DateTime.Now);
        LogHelper.DumpDirectoryListing(RegistryManager.RegistryManagers[oem].InstallDir, "InstallDirListing.txt", this.m_OemDir[oem].FullName);
        LogHelper.DumpDirectoryListing(RegistryManager.RegistryManagers[oem].DataDir, "DataDirListing.txt", this.m_OemDir[oem].FullName);
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
      }
      if (CollectLogs.s_ApkInstallFailureLogCollector)
        Console.WriteLine("returning for apk isntall failure ::::::  {0}", (object) DateTime.Now);
      else if (CollectLogs.s_BootFailureLogs)
      {
        Console.WriteLine("returning for boot failure ::::::  {0}", (object) DateTime.Now);
      }
      else
      {
        CollectLogs.UpdateProgress();
        try
        {
          string path = Path.Combine(RegistryManager.RegistryManagers[oem].DataDir, vmName);
          Logger.Info("bstAndroidDir: " + path);
          foreach (string file in Directory.GetFiles(path))
          {
            if (file.EndsWith(".signature", StringComparison.Ordinal))
            {
              string fileName = Path.GetFileName(file);
              string destFileName = Path.Combine(this.m_OemDir[oem].FullName, fileName);
              File.Copy(file, destFileName, true);
            }
          }
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.ToString());
        }
        CollectLogs.UpdateMessage(LocaleStrings.GetLocalizedString("STRING_STATUS_COLLECTING_HOST", ""));
        CollectLogs.UpdateProgress();
        try
        {
          string path2_1 = "HD-DXCheck.exe";
          string args = "2";
          string path2_2 = "CurrentDXCheck.txt";
          switch (RegistryManager.RegistryManagers[oem].DefaultGuest.GlRenderMode)
          {
            case 1:
              args = (string) null;
              path2_1 = "HD-GLCheck.exe";
              path2_2 = "CurrentGLCheck.txt";
              break;
            case 3:
              args = "3";
              break;
          }
          Console.WriteLine("Dumping {0} output  :::::::  {1}", (object) path2_1, (object) DateTime.Now);
          LogHelper.RunCmd(Path.Combine(RegistryManager.RegistryManagers[oem].InstallDir, path2_1), args, Path.Combine(this.m_OemDir[oem].FullName, path2_2));
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.ToString());
        }
        LogHelper.CollectUserCfgFilesIfPresent(this.m_OemDir[oem], oem);
        LogHelper.CollectBlueStacksGPArtifactsIfExists(this.m_OemDir[oem], oem, ref this.mSource);
        CollectLogs.UpdateProgress();
        Console.WriteLine(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Done CollectHostArtifacts at {0}:{1}", (object) DateTime.Now.Minute, (object) DateTime.Now.Second));
      }
    }

    private static void DumpPendingGlCalls()
    {
      try
      {
        string commonAppTitleText = Oem.Instance.CommonAppTitleText;
        IntPtr window = CollectLogs.FindWindow((string) null, commonAppTitleText);
        Console.WriteLine("FE title = {1}, FE handle = {0}", (object) window, (object) commonAppTitleText);
        if (!(window != IntPtr.Zero))
          return;
        CollectLogs.SetForegroundWindow(window);
        SendKeys.SendWait("^%(G)");
      }
      catch (Exception ex)
      {
        Logger.Error("Error Occured while trying to send alt+ctrl+G keys to Frontend");
      }
    }

    private void DumpExtraInfo()
    {
      CollectLogs.LogDirectoryPaths("bgp");
      try
      {
        string str = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "BlueStacks");
        if (Directory.Exists(str))
          LogHelper.CopyRecursive(str, Path.Combine(this.mTempLogsDir.FullName, "Installer Logs"));
      }
      catch (Exception ex)
      {
        Console.WriteLine("Failed to copy installer logs");
        Console.WriteLine(ex.ToString());
      }
      if (!CollectLogs.s_ThinCollector && !CollectLogs.s_BootFailureLogs && !CollectLogs.s_ApkInstallFailureLogCollector)
      {
        Console.WriteLine("Dumping Product Configuration  :::::::  {0}", (object) DateTime.Now);
        try
        {
          LogHelper.DumpEventLogs("Application", Path.Combine(this.mTempLogsDir.FullName, "ApplicationEvents.txt"));
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.ToString());
        }
        try
        {
          LogHelper.DumpEventLogs("System", Path.Combine(this.mTempLogsDir.FullName, "SystemEvents.txt"));
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.ToString());
        }
      }
      try
      {
        Console.WriteLine("Windows Update Version Information  :::::::  {0}", (object) DateTime.Now);
        using (StreamWriter streamWriter = new StreamWriter(Path.Combine(this.mTempLogsDir.FullName, "SystemVersion.txt")))
        {
          string str = RegistryUtils.GetRegistryValue("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion", "ReleaseId", (object) "", RegistryKeyKind.HKEY_LOCAL_MACHINE).ToString();
          streamWriter.WriteLine("Windows Update Version: {0}", (object) str);
        }
      }
      catch (Exception ex)
      {
        Logger.Error(ex.ToString());
      }
      try
      {
        Console.WriteLine("Dumping System Information  :::::::  {0}", (object) DateTime.Now);
        LogHelper.RunCmd("SystemInfo", (string) null, Path.Combine(this.mTempLogsDir.FullName, "SystemInfo.txt"));
      }
      catch (Exception ex)
      {
        Logger.Error(ex.ToString());
      }
      try
      {
        CollectLogs.UpdateProgress();
        Console.WriteLine("Dumping Process Information  :::::::  {0}", (object) DateTime.Now);
        LogHelper.DumpProcessList(Path.Combine(this.mTempLogsDir.FullName, "TaskList.txt"));
      }
      catch (Exception ex)
      {
        Logger.Error("Error dumping process list" + ex.ToString());
      }
      try
      {
        Console.WriteLine("Dumping Startup Programs  :::::::  {0}", (object) DateTime.Now);
        LogHelper.DumpStartupPrograms(Path.Combine(this.mTempLogsDir.FullName, "Startup.txt"));
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
      }
      try
      {
        Console.WriteLine("Dumping Installed Programs  :::::::  {0}", (object) DateTime.Now);
        LogHelper.DumpInstalledPrograms(Path.Combine(this.mTempLogsDir.FullName, "InstalledPrograms.txt"));
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
      }
      try
      {
        LogHelper.RunCmd("reg.exe", string.Format((IFormatProvider) CultureInfo.CurrentCulture, "EXPORT HKLM\\System\\CurrentControlSet\\services\\{0} \"{1}\"", (object) BlueStacks.Common.Strings.BlueStacksDriverName, (object) Path.Combine(this.mTempLogsDir.FullName, "RegBstkDrv.txt")), (string) null);
      }
      catch (Exception ex)
      {
        Logger.Info("Ignoring Error of BstkDrv since may be case of no plus component has been installed");
        Logger.Error(ex.ToString());
      }
      try
      {
        CollectLogs.UpdateProgress();
        Console.WriteLine("trying to get host nslookup  :::::::  {0}", (object) DateTime.Now);
        LogHelper.RunCmdInternal("nslookup", "www.google.com", Path.Combine(this.mTempLogsDir.FullName, "Host-nslookup.txt"));
      }
      catch (Exception ex)
      {
        Console.WriteLine("Failed to get host nslookup");
        Console.WriteLine(ex.ToString());
      }
      try
      {
        CollectLogs.UpdateProgress();
        Console.WriteLine("trying to get host netstat  :::::::  {0}", (object) DateTime.Now);
        LogHelper.RunCmdInternal("netstat", "-aon", Path.Combine(this.mTempLogsDir.FullName, "Host-netstat.txt"));
      }
      catch (Exception ex)
      {
        Console.WriteLine("Failed to get host netstat");
        Console.WriteLine(ex.ToString());
      }
      try
      {
        CollectLogs.UpdateProgress();
        Console.WriteLine("trying to get host net statistics workstation  :::::::  {0}", (object) DateTime.Now);
        LogHelper.RunCmdInternal("net", "statistics workstation", Path.Combine(this.mTempLogsDir.FullName, "Host-netstatistics.txt"));
      }
      catch (Exception ex)
      {
        Console.WriteLine("Failed to get host net statistics");
        Console.WriteLine(ex.ToString());
      }
      try
      {
        CollectLogs.UpdateProgress();
        Console.WriteLine("trying to get host ipconfig  :::::::  {0}", (object) DateTime.Now);
        LogHelper.RunCmdInternal("ipconfig", "/all", Path.Combine(this.mTempLogsDir.FullName, "Host-ipconfig.txt"));
      }
      catch (Exception ex)
      {
        Console.WriteLine("Failed to get host ipconfig");
        Console.WriteLine(ex.ToString());
      }
      try
      {
        using (StreamWriter streamWriter = new StreamWriter(Path.Combine(this.mTempLogsDir.FullName, "FreeDiskSpace.txt")))
        {
          foreach (DriveInfo drive in DriveInfo.GetDrives())
          {
            streamWriter.WriteLine("Drive {0}", (object) drive.Name);
            streamWriter.WriteLine("  Drive type: {0}", (object) drive.DriveType);
            if (drive.IsReady)
            {
              streamWriter.WriteLine("  Volume label: {0}", (object) drive.VolumeLabel);
              streamWriter.WriteLine("  File system: {0}", (object) drive.DriveFormat);
              streamWriter.WriteLine("  Available space to current user:{0, 15} bytes", (object) drive.AvailableFreeSpace);
              streamWriter.WriteLine("  Total available space:          {0, 15} bytes", (object) drive.TotalFreeSpace);
              streamWriter.WriteLine("  Total size of drive:            {0, 15} bytes ", (object) drive.TotalSize);
            }
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Error(ex.ToString());
      }
      try
      {
        if (!CollectLogs.s_BootFailureLogs)
        {
          if (!CollectLogs.s_ApkInstallFailureLogCollector)
          {
            Console.WriteLine("Dumping Driver Query  :::::::  {0}", (object) DateTime.Now);
            LogHelper.RunCmd("driverquery.exe", "/V", Path.Combine(this.mTempLogsDir.FullName, "DriverQuery.txt"));
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Error(ex.ToString());
      }
      try
      {
        Console.WriteLine("Copying Attached File :::::::  {0}", (object) DateTime.Now);
        int num = 0;
        foreach (string str in (IEnumerable<string>) this.mAttachment)
        {
          string destFileName = num != 0 ? Path.Combine(this.mTempLogsDir.FullName, "UserAttachedFile (" + num.ToString() + ") " + Path.GetExtension(str)) : Path.Combine(this.mTempLogsDir.FullName, "UserAttachedFile" + Path.GetExtension(str));
          File.Copy(str, destFileName, true);
          ++num;
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
      }
      this.mIsExtraInfoDumpedEvent.Set();
    }

    private StreamWriter OpenLog()
    {
      StreamWriter writer = new StreamWriter(Path.Combine(this.mTempLogsDir.FullName, "LogCollector.log"))
      {
        AutoFlush = true
      };
      this.mOrigOut = Console.Out;
      Console.SetOut((TextWriter) writer);
      if (CollectLogs.sProgressWindow != null)
        CollectLogs.sProgressWindow.Closing += (CancelEventHandler) ((obj, evt) =>
        {
          this.mGuestHostInfoCollectors.ForEach((System.Action<BackgroundWorker>) (wrkr =>
          {
            if (!wrkr.IsBusy)
              return;
            wrkr.CancelAsync();
          }));
          this.CloseWriterAndDeleteTempDir(writer);
        });
      if (!string.IsNullOrEmpty(App.sOpt.Source))
        Console.WriteLine(string.Format("Log collection started for OEM:{0}   Source={1}:::::: {2}\n", (object) "bgp", (object) App.sOpt.Source, (object) DateTime.Now));
      else if (!string.IsNullOrEmpty(App.sOpt.Vmname))
        Console.WriteLine(string.Format("Log collection started for OEM:{0}   Source={1}:::::: {2}\n", (object) "bgp", (object) App.sOpt.Vmname, (object) DateTime.Now));
      return writer;
    }

    private void CloseWriterAndDeleteTempDir(StreamWriter writer)
    {
      Console.WriteLine("Deleting temporary directory,  ::::::: {0}", (object) DateTime.Now);
      writer.Close();
      try
      {
        if (!this.mTempLogsDir.Exists)
          return;
        this.mTempLogsDir.Delete(true);
      }
      catch (Exception ex)
      {
        Logger.Warning(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Error Occured in CloseWriterAndDeleteTempDir, Err : {0}", (object) ex.ToString()));
      }
    }

    private void CloseLog()
    {
      Console.Out.Close();
      Console.SetOut(this.mOrigOut);
    }

    private void CreateTempLogsDir()
    {
      string path = Path.Combine(Path.GetTempPath(), "Bst_Logs_" + Path.GetRandomFileName());
      Console.WriteLine("Creating temporary directory , {0}  ::::::: {1}", (object) path, (object) DateTime.Now);
      this.mTempLogsDir = Directory.CreateDirectory(path);
    }

    private void EnableDebugPrivilege()
    {
      try
      {
        Process.EnterDebugMode();
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
      }
    }

    private bool StartAndroidPlusHost(string vmName, string oem)
    {
      if (Utils.IsAndroidPlayerRunning(vmName, oem))
      {
        Console.WriteLine("Plus Host for VM {0} OEM {1} already running", (object) vmName, (object) oem);
        return true;
      }
      Console.WriteLine("Service for VM {0} OEM {1} is not running, starting it now", (object) vmName, (object) oem);
      if (!this.mStartAllOems && !string.Equals(oem, "bgp", StringComparison.InvariantCultureIgnoreCase))
        return false;
      this.mOemToCloseAfterLogCollection.Add(oem);
      return Utils.StartHiddenFrontend(vmName, oem) != null;
    }

    private static void LogDirectoryPaths(string oem)
    {
      Console.WriteLine("Install Dir:   " + RegistryManager.RegistryManagers[oem].InstallDir + "  ::::::: {0}", (object) DateTime.Now);
      Console.WriteLine("User Data Dir: " + RegistryManager.RegistryManagers[oem].DataDir + "  ::::::: {0}", (object) DateTime.Now);
      Console.WriteLine("Per User Dir:  " + Path.Combine(RegistryManager.RegistryManagers[oem].DataDir, "UserData") + "  ::::::: {0}", (object) DateTime.Now);
    }

    private void UploadZipFile(
      string email,
      string category,
      string appName,
      string appPkgName,
      string desc,
      string subcategory,
      string source,
      string oem,
      string vmName)
    {
      CollectLogs.UpdateMessage(LocaleStrings.GetLocalizedString("STRING_STATUS_SENDING", ""));
      string str1 = string.IsNullOrEmpty(CollectLogs.s_DestinationFolder) ? Path.Combine(Path.GetTempPath(), "BlueStacks-Support.7z") : Path.Combine(CollectLogs.s_DestinationFolder, "BlueStacks-Support.7z");
      string url = string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}/{1}", (object) RegistryManager.RegistryManagers[oem].Host, (object) "uploaddebuglogs");
      if (CollectLogs.s_ApkInstallFailureLogCollector)
        url = string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}/{1}", (object) RegistryManager.RegistryManagers[oem].Host, (object) "logs/appinstallfailurelog");
      else if (CollectLogs.s_BootFailureLogs)
        url = string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}/{1}", (object) RegistryManager.RegistryManagers[oem].Host, (object) "logs/bootfailurelog");
      else if (CollectLogs.s_CrashLogs)
        url = string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}/{1}", (object) RegistryManager.RegistryManagers[oem].Host, (object) "logs/crashlog");
      Dictionary<string, string> data = new Dictionary<string, string>()
      {
        {
          nameof (email),
          email
        },
        {
          nameof (desc),
          desc
        },
        {
          "culture",
          RegistryManager.RegistryManagers[oem].UserSelectedLocale
        }
      };
      if (!string.IsNullOrEmpty(category))
        data.Add(nameof (category), category);
      if (!string.IsNullOrEmpty(subcategory))
        data.Add(nameof (subcategory), subcategory);
      if (!string.IsNullOrEmpty(appName))
        data.Add("app", appName);
      if (!string.IsNullOrEmpty(appPkgName))
        data.Add("app_package", appPkgName);
      if (!string.IsNullOrEmpty(source))
        data.Add(nameof (source), source);
      if (CollectLogs.s_BootFailureLogs)
      {
        data.Add("error", this.mErrorReason);
        data.Add("ecode", this.mErrorCode);
        data.Add("firstboot", (RegistryManager.RegistryManagers[oem].DefaultGuest.ConfigSynced == 0).ToString((IFormatProvider) CultureInfo.InvariantCulture));
      }
      else if (CollectLogs.s_ApkInstallFailureLogCollector)
      {
        data.Add("error", this.mErrorReason);
        data.Add("ecode", this.mErrorCode);
        data.Add("apk", CollectLogs.s_InstallFailedApkName);
      }
      else if (CollectLogs.s_CrashLogs)
      {
        Console.WriteLine("the error reason is {0}", (object) this.mErrorReason);
        data.Add("crash_type", this.mErrorReason);
        data.Add("ecode", this.mErrorCode);
        data.Add("package", this.mErrorDetails);
      }
      try
      {
        if (string.IsNullOrEmpty(RegistryManager.RegistryManagers[oem].Version))
          Console.WriteLine("Version string is empty  :::::::  {0}", (object) DateTime.Now);
        BstHttpClient.HTTPGaeFileUploader(url, data, (Dictionary<string, string>) null, str1, "application/x-7z-compressed", false, vmName);
        this.mUploadSuccessful = true;
        File.Delete(str1);
      }
      catch
      {
        this.mUploadSuccessful = false;
        if (Oem.IsOEMDmm)
        {
          try
          {
            Logger.Info("Deleting log collector zip file for dmm: {0}", (object) str1);
            File.Delete(str1);
          }
          catch (Exception ex)
          {
            Logger.Error("Error in deleting log collector zip file for dmm: {0}", (object) ex);
          }
        }
        else
        {
          string str2 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "BlueStacks-Support.7z");
          if (File.Exists(str2))
            File.Delete(str2);
          File.Move(str1, str2);
        }
      }
    }

    private void ShowFinish()
    {
      SendOrPostCallback d = (SendOrPostCallback) (obj =>
      {
        CollectLogs.sProgressWindow.Hide();
        CustomMessageWindow customMessageWindow = new CustomMessageWindow();
        customMessageWindow.Title = LocaleStrings.GetLocalizedString("STRING_BST_SUPPORT_UTILITY", "");
        if (Oem.Instance.IsHideMessageBoxIconInTaskBar)
          customMessageWindow.Owner = (Window) CollectLogs.sProgressWindow;
        if (this.mUploadSuccessful)
        {
          customMessageWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_REPORT_SENT_SUCCESS", "");
          customMessageWindow.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_FINISH_REPORT_SEND", "");
        }
        else
        {
          customMessageWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_ERROR_SENDING_REPORT", "");
          customMessageWindow.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_PROMPT", "");
        }
        customMessageWindow.AddButton(ButtonColors.Blue, LocaleStrings.GetLocalizedString("STRING_CLOSE", ""), (EventHandler) null, (string) null, false, (object) null, true);
        customMessageWindow.ShowDialog();
        CollectLogs.sProgressWindow.Close();
      });
      try
      {
        this.mUiContext.Send(d, (object) null);
      }
      catch (Exception ex)
      {
      }
    }

    private static void UpdateMessage(string msg)
    {
      if (!CollectLogs.s_ShowUI)
        return;
      if (CollectLogs.sProgressWindow == null)
        return;
      try
      {
        System.Windows.Application.Current.Dispatcher.Invoke((Delegate) (() => CollectLogs.sProgressWindow.mProgressText.Text = msg));
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
      }
    }

    private static void UpdateProgress()
    {
      if (!CollectLogs.s_ShowUI)
        return;
      if (CollectLogs.sProgressWindow == null)
        return;
      try
      {
        System.Windows.Application.Current.Dispatcher.Invoke((Delegate) (() => ++CollectLogs.sProgressWindow.mProgressBar.Value));
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
      }
    }

    protected virtual void Dispose(bool disposing)
    {
      int num = disposing ? 1 : 0;
      if (this.disposedValue)
        return;
      this.mGuestHostInfoCollectors.ForEach((System.Action<BackgroundWorker>) (wrkr => wrkr?.Dispose()));
      this.mOrigOut?.Dispose();
      this.mIsExtraInfoDumpedEvent?.Close();
      this.disposedValue = true;
    }

    ~CollectLogs()
    {
      this.Dispose(false);
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    private void SendStats(string tag, string arg1, string arg2)
    {
      Stats.SendMiscellaneousStatsAsync(tag, RegistryManager.RegistryManagers["bgp"].UserGuid, arg1, arg2, RegistryManager.RegistryManagers["bgp"].ClientVersion, (string) null, (string) null, (string) null, "bgp", MultiInstanceStrings.VmName, 0);
    }
  }
}
