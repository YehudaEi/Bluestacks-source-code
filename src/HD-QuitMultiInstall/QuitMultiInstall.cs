// Decompiled with JetBrains decompiler
// Type: BlueStacks.QuitMultiInstall.QuitMultiInstall
// Assembly: HD-QuitMultiInstall, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 0F5C1A35-D514-4726-A3F2-01FE37E3028F
// Assembly location: C:\Program Files\BlueStacks\HD-QuitMultiInstall.exe

using BlueStacks.Common;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Management;
using System.Security.AccessControl;
using System.ServiceProcess;
using System.Threading;
using System.Windows.Forms;

namespace BlueStacks.QuitMultiInstall
{
  public class QuitMultiInstall
  {
    private static string BlueStacksServicesPrefix = "BstHd";
    private static string[] BlueStacksServicePlusPrefixes = new string[2]
    {
      "BstkDrv",
      "BlueStacksDrv"
    };
    private static Mutex s_HDQuitMultiInstaceLock;

    private static void InitExceptionHandlers()
    {
      Application.ThreadException += (ThreadExceptionEventHandler) ((obj, evt) =>
      {
        Logger.Error("QuitMultiInstall: Unhandled Exception:");
        Logger.Error(evt.Exception.ToString());
        Environment.Exit(1);
      });
      Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
      AppDomain.CurrentDomain.UnhandledException += (UnhandledExceptionEventHandler) ((obj, evt) =>
      {
        Logger.Error("QuitMultiInstall: Unhandled Exception:");
        Logger.Error(evt.ExceptionObject.ToString());
        Environment.Exit(1);
      });
    }

    public static int Main(string[] args)
    {
      Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
      Logger.InitUserLog();
      BlueStacks.QuitMultiInstall.QuitMultiInstall.InitExceptionHandlers();
      ServiceController runningSvc = (ServiceController) null;
      if (ProcessUtils.CheckAlreadyRunningAndTakeLock("Global\\BlueStacks_HDQuitMultiInstall_Lockbgp", out BlueStacks.QuitMultiInstall.QuitMultiInstall.s_HDQuitMultiInstaceLock))
      {
        Logger.Info("QuitMultiInstall already running, returning");
        return 0;
      }
      if (args.Length != 0 && args[0] == "-in")
      {
        Logger.Info("quit multi install started with arg : -in...checking for another running version of parent proccess");
        string[] strArray = new string[9]
        {
          "HD-Agent",
          "HD-ApkHandler",
          "HD-Adb",
          "HD-RunApp",
          "HD-Updater",
          "HD-Player",
          "BlueStacks",
          "BstkSVC",
          "HD-XapkHandler"
        };
        bool flag = false;
        foreach (string procName in strArray)
        {
          if (Utils.IsRunningInstanceClashWithAnotherInstance(procName))
          {
            flag = true;
            break;
          }
        }
        if (Utils.IsRunningInstanceClashWithService(BlueStacks.QuitMultiInstall.QuitMultiInstall.BlueStacksServicePlusPrefixes, out runningSvc))
          flag = true;
        if (!flag)
          return 0;
        string str1 = LocaleStrings.GetLocalizedString("STRING_ANOTHER_BLUESTACKS_INSTANCE_RUNNING_PROMPT_TEXT1", "");
        string str2 = LocaleStrings.GetLocalizedString("STRING_ANOTHER_BLUESTACKS_INSTANCE_RUNNING_PROMPT_TEXT2", "");
        if ("bgp".Equals("dmm") && RegistryManager.Instance.UserSelectedLocale.Equals("ja-JP", StringComparison.InvariantCultureIgnoreCase))
        {
          str1 = "同時に起動できないプログラムが既に動いています。";
          str2 = "既に動いているプログラムを閉じて続行しますか？";
        }
        if (MessageBox.Show(str1 + Environment.NewLine + str2, string.Format("{0} Warning", (object) Strings.ProductDisplayName), MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.Cancel)
          return -1;
      }
      BlueStacks.QuitMultiInstall.QuitMultiInstall.QuitPreviousInstance();
      if (runningSvc != null)
        BlueStacks.QuitMultiInstall.QuitMultiInstall.StopService(runningSvc);
      ComRegistration.Register();
      return 0;
    }

    private static void SearchAndStopPlusHosts()
    {
      try
      {
        string str = "HD-Plus-Service.exe";
        foreach (ManagementObject managementObject in new ManagementObjectSearcher("SELECT ProcessId, ExecutablePath FROM Win32_Process WHERE Name = 'HD-Plus-Service.exe'").Get())
        {
          Logger.Info("Found process id {0}, process executable {1}", managementObject["ProcessId"], managementObject["ExecutablePath"]);
          string path1 = (string) managementObject["ExecutablePath"];
          int processId = (int) (uint) managementObject["ProcessId"];
          string path2 = Directory.GetParent(path1).ToString();
          string installDir = RegistryStrings.InstallDir;
          Logger.Debug("The Process Dir is {0}", (object) path2);
          if (string.Compare(Path.GetFullPath(installDir).TrimEnd('\\'), Path.GetFullPath(path2).TrimEnd('\\'), StringComparison.InvariantCultureIgnoreCase) == 0)
          {
            Logger.Debug("process:{0} not killed since the process Dir:{1} and Ignore Dir:{2} are same", (object) str, (object) path2, (object) installDir);
          }
          else
          {
            Logger.Info("Trying to stop vm {0}, pid {1}", (object) str, (object) processId);
            EventWaitHandle.OpenExisting(string.Format("HD-Plus-{0}", (object) processId), EventWaitHandleRights.Modify).Set();
            Process processById = Process.GetProcessById(processId);
            if (processById != null)
            {
              Logger.Info("Waiting for pid {0} to exit", (object) processId);
              processById.WaitForExit();
              Logger.Info("pid {0} has exited", (object) processId);
            }
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Exception while stopping HD-Plus-Service.exe");
        Logger.Error(ex.ToString());
      }
    }

    private static void SearchAndStopServices(ServiceController[] services)
    {
      foreach (ServiceController service in services)
      {
        string serviceName = service.ServiceName;
        foreach (string servicePlusPrefix in BlueStacks.QuitMultiInstall.QuitMultiInstall.BlueStacksServicePlusPrefixes)
        {
          if (serviceName.StartsWith(BlueStacks.QuitMultiInstall.QuitMultiInstall.BlueStacksServicesPrefix, true, (CultureInfo) null) || serviceName.StartsWith(servicePlusPrefix, true, (CultureInfo) null))
          {
            if (Strings.GetOemTag().Equals(""))
            {
              if (serviceName.ToLower().Contains("svc_") && service.Status == ServiceControllerStatus.Running)
              {
                Logger.Info("serviceName: {0} found running and stopping it", (object) serviceName);
                BlueStacks.QuitMultiInstall.QuitMultiInstall.StopService(service);
                Logger.Info("ServiceName : {0} is {1}", (object) serviceName, (object) service.Status.ToString());
              }
            }
            else if (!serviceName.EndsWith(Strings.GetOemTag(), true, (CultureInfo) null) && service.Status == ServiceControllerStatus.Running)
            {
              Logger.Info("serviceName: {0} found running and stopping it", (object) serviceName);
              BlueStacks.QuitMultiInstall.QuitMultiInstall.StopService(service);
              Logger.Info("ServiceName : {0} is {1}", (object) serviceName, (object) service.Status.ToString());
            }
          }
        }
      }
    }

    private static void QuitPreviousInstanceServices()
    {
      Logger.Debug("stopping all the PlusHost");
      BlueStacks.QuitMultiInstall.QuitMultiInstall.SearchAndStopPlusHosts();
      Logger.Info("Trying to kill BstkSVC if it exists...");
      Utils.KillComServerSafe();
      Logger.Debug("stopping driver Services if running");
      BlueStacks.QuitMultiInstall.QuitMultiInstall.SearchAndStopServices(ServiceController.GetDevices());
    }

    private static void StopService(ServiceController service)
    {
      try
      {
        service.Stop();
        Logger.Info("Stopping service " + service.ServiceName);
        TimeSpan timeout = TimeSpan.FromSeconds(15.0);
        if (!service.ServiceName.StartsWith("BstHdDrv", true, (CultureInfo) null) && !service.ServiceName.StartsWith("BstHdAndroidSvc", true, (CultureInfo) null) && !service.ServiceName.StartsWith("BstkDrv"))
          return;
        service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
        if (service.Status == ServiceControllerStatus.Stopped)
          return;
        Logger.Info("Service in state: {0} after {0}s, killing BstkSvc", (object) service.Status, (object) timeout.ToString());
        ProcessUtils.KillProcessByName("BstkSvc");
        service.Stop();
        service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
        Logger.Info("Service in state: {0} after {0}s, after killing", (object) service.Status, (object) timeout.ToString());
      }
      catch (Exception ex)
      {
        Logger.Error("Error occured , err {0}", (object) ex.ToString());
      }
    }

    public static void QuitPreviousInstance()
    {
      Logger.Info("Trying to quit previous instances");
      Thread thread1 = new Thread((ThreadStart) (() =>
      {
        Logger.Debug("Starting services quit thread");
        BlueStacks.QuitMultiInstall.QuitMultiInstall.QuitPreviousInstanceServices();
      }));
      Thread thread2 = new Thread((ThreadStart) (() =>
      {
        Logger.Info("Starting processes quit thread");
        BlueStacks.QuitMultiInstall.QuitMultiInstall.QuitPreviousInstanceProcesses();
      }));
      thread1.IsBackground = true;
      thread1.Start();
      thread2.IsBackground = true;
      thread2.Start();
      thread2.Join();
      thread1.Join();
      Logger.Info("Quit service and thread complete");
    }

    private static void QuitPreviousInstanceProcesses()
    {
      BlueStacks.QuitMultiInstall.QuitMultiInstall.QuitPreviousInstanceProcesses(new string[9]
      {
        "HD-Agent",
        "HD-ApkHandler",
        "HD-Adb",
        "HD-RunApp",
        "HD-Updater",
        "HD-Player",
        "BlueStacks",
        "HD-MultiInstanceManager",
        "HD-XapkHandler"
      });
    }

    private static void QuitPreviousInstanceProcesses(string[] processList)
    {
      Logger.Info("quitting previous instance all processes");
      string installDir = RegistryStrings.InstallDir;
      Logger.Debug("killing previous instance processes with ignoring the current Directory {0}", (object) installDir);
      BlueStacks.QuitMultiInstall.QuitMultiInstall.KillProcessesByName(processList, installDir);
    }

    public static void KillProcessesByName(string[] nameList, string IgnoreDirectory)
    {
      foreach (string name in nameList)
        BlueStacks.QuitMultiInstall.QuitMultiInstall.KillProcessByNameIgnoreDirectory(name, IgnoreDirectory);
    }

    public static void KillProcessByNameIgnoreDirectory(string name, string IgnoreDirectory)
    {
      foreach (Process process in Process.GetProcessesByName(name))
      {
        string path1 = "";
        try
        {
          if (SystemUtils.IsOSWinXP())
          {
            if (SystemUtils.IsAdministrator())
              path1 = process.MainModule.FileName.ToString();
          }
          else
            path1 = GetProcessExecutionPath.GetExecutablePathAboveVista(new UIntPtr((uint) process.Id));
        }
        catch (Win32Exception ex)
        {
          Logger.Error("got the excpetion {0}", (object) ex.ToString());
          Logger.Error("NativeErrorCode {0}", (object) ex.NativeErrorCode);
          Logger.Error("Exception Message {0}", (object) ex.Message);
          Logger.Info("giving the exit code to start as admin");
          Environment.Exit(2);
        }
        catch (Exception ex)
        {
          Logger.Error("got exception: err {0}", (object) ex.ToString());
        }
        if (!string.IsNullOrEmpty(path1))
        {
          string path2 = Directory.GetParent(path1).ToString();
          Logger.Debug("The Process Dir is {0}", (object) path2);
          if (string.Compare(Path.GetFullPath(IgnoreDirectory).TrimEnd('\\'), Path.GetFullPath(path2).TrimEnd('\\'), StringComparison.InvariantCultureIgnoreCase) == 0)
          {
            Logger.Debug("process:{0} not killed since the process Dir:{1} and Ignore Dir:{2} are same", (object) process.ProcessName, (object) path2, (object) IgnoreDirectory);
          }
          else
          {
            if (name.Equals("BlueStacks"))
            {
              string path3 = "";
              try
              {
                path3 = RegistryManager.Instance.ClientInstallDir;
              }
              catch
              {
              }
              if (!string.IsNullOrEmpty(path3))
              {
                if (string.Compare(Path.GetFullPath(path3).TrimEnd('\\'), Path.GetFullPath(path2).TrimEnd('\\'), StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                  Logger.Debug("process:{0} not killed since the process Dir:{1} and Ignore Dir:{2} are same", (object) process.ProcessName, (object) path2, (object) IgnoreDirectory);
                  continue;
                }
              }
            }
            Logger.Info("Killing PID " + process.Id.ToString() + " -> " + process.ProcessName);
            try
            {
              process.Kill();
            }
            catch (Exception ex)
            {
              Logger.Error(ex.ToString());
              continue;
            }
            if (!process.WaitForExit(5000))
              Logger.Info("Timeout waiting for process to die");
          }
        }
      }
    }
  }
}
