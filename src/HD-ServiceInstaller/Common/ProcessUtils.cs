// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.ProcessUtils
// Assembly: HD-ServiceInstaller, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 15F93427-26B3-4C7E-BAB1-0A00688BC4D4
// Assembly location: C:\Program Files\BlueStacks\HD-ServiceInstaller.exe

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace BlueStacks.Common
{
  public static class ProcessUtils
  {
    private static readonly Guid SID_STopLevelBrowser = new Guid("4C96BE40-915C-11CF-99D3-00AA004AE837");
    private const int CSIDL_Desktop = 0;
    private const int SWC_DESKTOP = 8;
    private const int SWFO_NEEDDISPATCH = 1;
    private const int SW_SHOWNORMAL = 1;
    private const int SVGIO_BACKGROUND = 0;

    public static Dictionary<string, string> LockToProcessMap
    {
      get
      {
        return new Dictionary<string, string>()
        {
          {
            "Global\\BlueStacks_Installer_Lockbgp",
            "Installer"
          },
          {
            "Global\\BlueStacks_MicroInstaller_Lockbgp",
            "MicroInstaller"
          },
          {
            "Global\\BlueStacks_Uninstaller_Lockbgp",
            "Uninstaller"
          }
        };
      }
    }

    public static bool FindProcessByName(string name)
    {
      return (uint) Process.GetProcessesByName(name).Length > 0U;
    }

    public static void KillProcessByName(string name)
    {
      foreach (Process process in Process.GetProcessesByName(name))
      {
        try
        {
          Logger.Debug("Attempting to kill: {0}", (object) process.ProcessName);
          process.Kill();
          if (!process.WaitForExit(5000))
            Logger.Info("Timeout waiting for process {0} to die", (object) process.ProcessName);
        }
        catch (Exception ex)
        {
          Logger.Error("Exception in killing process " + ex.Message);
        }
      }
    }

    public static void KillProcessesByName(string[] nameList)
    {
      if (nameList == null)
        return;
      foreach (string name in nameList)
        ProcessUtils.KillProcessByName(name);
    }

    public static Process GetProcessObject(string exePath, string args, bool isAdmin = false)
    {
      Process process = new Process();
      process.StartInfo.Arguments = args;
      process.StartInfo.UseShellExecute = false;
      process.StartInfo.CreateNoWindow = true;
      process.StartInfo.FileName = exePath;
      if (isAdmin)
      {
        process.StartInfo.Verb = "runas";
        process.StartInfo.UseShellExecute = true;
      }
      return process;
    }

    public static bool IsProcessAlive(int pid)
    {
      bool flag = false;
      try
      {
        Process.GetProcessById(pid);
        flag = true;
      }
      catch (ArgumentException ex)
      {
      }
      return flag;
    }

    public static bool IsLockInUse(string lockName)
    {
      return ProcessUtils.IsLockInUse(lockName, true);
    }

    public static bool IsLockInUse(string lockName, bool printLog)
    {
      Mutex lck;
      if (ProcessUtils.CheckAlreadyRunningAndTakeLock(lockName, out lck))
      {
        if (printLog)
          Logger.Info(lockName + " running.");
        return true;
      }
      lck?.Close();
      return false;
    }

    public static bool IsAnyInstallerProcesRunning(out string runningProcName)
    {
      runningProcName = (string) null;
      foreach (string key in ProcessUtils.LockToProcessMap.Keys)
      {
        if (ProcessUtils.IsAlreadyRunning(key))
        {
          runningProcName = ProcessUtils.LockToProcessMap[key];
          return true;
        }
      }
      return false;
    }

    public static bool IsAlreadyRunning(string name)
    {
      Mutex lck;
      if (ProcessUtils.CheckAlreadyRunningAndTakeLock(name, out lck))
        return true;
      lck?.Close();
      return false;
    }

    public static bool CheckAlreadyRunningAndTakeLock(string name, out Mutex lck)
    {
      bool createdNew;
      try
      {
        lck = new Mutex(true, name, out createdNew);
      }
      catch (AbandonedMutexException ex)
      {
        lck = (Mutex) null;
        Logger.Warning("Abandoned mutex : " + name + ".  " + ex.ToString());
        return false;
      }
      catch (UnauthorizedAccessException ex)
      {
        lck = (Mutex) null;
        Logger.Warning("UnauthorisedAccess on mutex : " + name + ".  " + ex.ToString());
        return true;
      }
      if (!createdNew)
      {
        lck.Close();
        lck = (Mutex) null;
      }
      return !createdNew;
    }

    public static void KillProcessByNameIgnoreDirectory(string name, string IgnoreDirectory)
    {
      foreach (Process process in Process.GetProcessesByName(name))
      {
        string path = "";
        try
        {
          path = process.MainModule.FileName;
        }
        catch (Win32Exception ex)
        {
          Logger.Error("Got the excpetion {0}", (object) ex.Message);
          Logger.Info("Giving the exit code to start as admin");
          Environment.Exit(2);
        }
        catch (Exception ex)
        {
          Logger.Error("Got exception: err {0}", (object) ex.ToString());
        }
        string str = Directory.GetParent(path).ToString();
        Logger.Debug("The Process Dir is {0}", (object) str);
        if (str.Equals(IgnoreDirectory, StringComparison.CurrentCultureIgnoreCase))
        {
          Logger.Debug("Process:{0} not killed since the process sir:{1} and ignore dir:{2} are the same", (object) process.ProcessName, (object) str, (object) IgnoreDirectory);
        }
        else
        {
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

    public static void LogParentProcessDetails()
    {
      try
      {
        Process currentProcessParent = ProcessDetails.CurrentProcessParent;
        if (currentProcessParent == null)
          Logger.Info("Unable to retrieve information about invoking process");
        else
          Logger.Info("Invoking Process Details: (Name: {0}, Pid: {1})", (object) currentProcessParent.ProcessName, (object) currentProcessParent.Id);
      }
      catch (Exception ex)
      {
        Logger.Error("Unable to get parent process details, Err: {0}", (object) ex.ToString());
      }
    }

    public static void LogProcessContextDetails()
    {
      Logger.Info("PID {0}, CLR version {0}", (object) Process.GetCurrentProcess().Id, (object) Environment.Version);
      Logger.Info("IsAdministrator: {0}", (object) SystemUtils.IsAdministrator());
    }

    public static Process StartExe(string exePath, string args, bool isAdmin = false)
    {
      Process process = new Process();
      process.StartInfo.Arguments = args;
      process.StartInfo.UseShellExecute = false;
      process.StartInfo.CreateNoWindow = true;
      process.StartInfo.FileName = exePath;
      if (isAdmin)
      {
        process.StartInfo.Verb = "runas";
        process.StartInfo.UseShellExecute = true;
      }
      try
      {
        Logger.Info("Utils: Starting Process: {0} with args: {1}", (object) exePath, (object) args);
      }
      catch
      {
      }
      process.Start();
      return process;
    }

    public static void ExecuteProcessUnElevated(
      string process,
      string args,
      string currentDirectory = "")
    {
      ProcessUtils.IShellWindows shellWindows = (ProcessUtils.IShellWindows) new ProcessUtils.CShellWindows();
      object obj1 = (object) 0;
      object obj2 = new object();
      ref object local1 = ref obj1;
      ref object local2 = ref obj2;
      int num;
      ref int local3 = ref num;
      ProcessUtils.IServiceProvider windowSw = (ProcessUtils.IServiceProvider) shellWindows.FindWindowSW(ref local1, ref local2, 8, out local3, 1);
      Guid stopLevelBrowser = ProcessUtils.SID_STopLevelBrowser;
      Guid guid1 = typeof (ProcessUtils.IShellBrowser).GUID;
      ref Guid local4 = ref stopLevelBrowser;
      ref Guid local5 = ref guid1;
      ProcessUtils.IShellBrowser shellBrowser = (ProcessUtils.IShellBrowser) windowSw.QueryService(ref local4, ref local5);
      Guid guid2 = typeof (ProcessUtils.IDispatch).GUID;
      ((ProcessUtils.IShellDispatch2) ((ProcessUtils.IShellFolderViewDual) shellBrowser.QueryActiveShellView().GetItemObject(0U, ref guid2)).Application).ShellExecute(process, (object) args, (object) currentDirectory, (object) string.Empty, (object) 1);
    }

    [Guid("9BA05972-F6A8-11CF-A442-00A0C90A8F39")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComImport]
    private class CShellWindows
    {
      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      public extern CShellWindows();
    }

    [Guid("85CB6900-4D95-11CF-960C-0080C7F4EE85")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    [ComImport]
    private interface IShellWindows
    {
      [return: MarshalAs(UnmanagedType.IDispatch)]
      object FindWindowSW(
        [MarshalAs(UnmanagedType.Struct)] ref object pvarloc,
        [MarshalAs(UnmanagedType.Struct)] ref object pvarlocRoot,
        int swClass,
        out int pHWND,
        int swfwOptions);
    }

    [Guid("6d5140c1-7436-11ce-8034-00aa006009fa")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    private interface IServiceProvider
    {
      [return: MarshalAs(UnmanagedType.Interface)]
      object QueryService(ref Guid guidService, ref Guid riid);
    }

    [Guid("000214E2-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    private interface IShellBrowser
    {
      void VTableGap01();

      void VTableGap02();

      void VTableGap03();

      void VTableGap04();

      void VTableGap05();

      void VTableGap06();

      void VTableGap07();

      void VTableGap08();

      void VTableGap09();

      void VTableGap10();

      void VTableGap11();

      void VTableGap12();

      ProcessUtils.IShellView QueryActiveShellView();
    }

    [Guid("000214E3-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    private interface IShellView
    {
      void VTableGap01();

      void VTableGap02();

      void VTableGap03();

      void VTableGap04();

      void VTableGap05();

      void VTableGap06();

      void VTableGap07();

      void VTableGap08();

      void VTableGap09();

      void VTableGap10();

      void VTableGap11();

      void VTableGap12();

      [return: MarshalAs(UnmanagedType.Interface)]
      object GetItemObject(uint aspectOfView, ref Guid riid);
    }

    [Guid("00020400-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    [ComImport]
    private interface IDispatch
    {
    }

    [Guid("E7A1AF80-4D96-11CF-960C-0080C7F4EE85")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    [ComImport]
    private interface IShellFolderViewDual
    {
      object Application { [return: MarshalAs(UnmanagedType.IDispatch)] get; }
    }

    [Guid("A4C6892C-3BA9-11D2-9DEA-00C04FB16162")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    [ComImport]
    public interface IShellDispatch2
    {
      void ShellExecute([MarshalAs(UnmanagedType.BStr)] string File, [MarshalAs(UnmanagedType.Struct)] object vArgs, [MarshalAs(UnmanagedType.Struct)] object vDir, [MarshalAs(UnmanagedType.Struct)] object vOperation, [MarshalAs(UnmanagedType.Struct)] object vShow);
    }
  }
}
