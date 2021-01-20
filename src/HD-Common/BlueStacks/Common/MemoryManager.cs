// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.MemoryManager
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;

namespace BlueStacks.Common
{
  public static class MemoryManager
  {
    private static Thread trimMemoryThread;
    private static Thread androidtrimMemoryThread;

    [DllImport("KERNEL32.DLL", EntryPoint = "SetProcessWorkingSetSize", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    private static extern bool SetProcessWorkingSetSize32(
      IntPtr pProcess,
      int dwMinimumWorkingSetSize,
      int dwMaximumWorkingSetSize);

    [DllImport("KERNEL32.DLL", EntryPoint = "SetProcessWorkingSetSize", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    private static extern bool SetProcessWorkingSetSize64(
      IntPtr pProcess,
      long dwMinimumWorkingSetSize,
      long dwMaximumWorkingSetSize);

    [DllImport("psapi.dll")]
    private static extern int EmptyWorkingSet(IntPtr hwProc);

    public static void TrimMemory(bool isForceMemoryTrim = false)
    {
      if (!isForceMemoryTrim && (MemoryManager.trimMemoryThread?.IsAlive.GetValueOrDefault() || !RegistryManager.Instance.EnableMemoryTrim))
        return;
      MemoryManager.trimMemoryThread = new Thread((ThreadStart) (() =>
      {
        int millisecondsTimeout = RegistryManager.Instance.TrimMemoryDuration * 1000;
        Logger.Info("Setting trim memory duration to: " + millisecondsTimeout.ToString());
        while (true)
        {
          Thread.Sleep(millisecondsTimeout);
          try
          {
            if (!isForceMemoryTrim && !RegistryManager.Instance.EnableMemoryTrim)
              break;
            GC.Collect(2, GCCollectionMode.Forced);
            GC.WaitForPendingFinalizers();
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
              if (IntPtr.Size == 8)
                MemoryManager.SetProcessWorkingSetSize64(Process.GetCurrentProcess().Handle, -1L, -1L);
              else
                MemoryManager.SetProcessWorkingSetSize32(Process.GetCurrentProcess().Handle, -1, -1);
            }
            using (Process currentProcess = Process.GetCurrentProcess())
            {
              Logger.Debug("Trimming memory");
              MemoryManager.EmptyWorkingSet(currentProcess.Handle);
            }
          }
          catch (Exception ex)
          {
            Logger.Error("Exception while trimming memory ex: " + ex.ToString());
          }
        }
      }))
      {
        IsBackground = true
      };
      MemoryManager.trimMemoryThread.Start();
    }

    public static void CheckAndTrimAndroidMemory()
    {
      if (MemoryManager.androidtrimMemoryThread?.IsAlive.GetValueOrDefault() || !RegistryManager.Instance.EnableMemoryTrim)
        return;
      int timerInterval = 60000;
      int triggerThreshold = 700;
      timerInterval = RegistryManager.Instance.DefaultGuest.TriggerMemoryTrimTimerInterval;
      triggerThreshold = RegistryManager.Instance.DefaultGuest.TriggerMemoryTrimThreshold;
      MemoryManager.androidtrimMemoryThread = new Thread((ThreadStart) (() =>
      {
        while (true)
        {
          long workingSet64;
          int num;
          do
          {
            Thread.Sleep(timerInterval);
            if (RegistryManager.Instance.EnableMemoryTrim)
            {
              workingSet64 = Process.GetCurrentProcess().WorkingSet64;
              num = triggerThreshold * 1024 * 1024;
            }
            else
              goto label_3;
          }
          while (workingSet64 <= (long) num);
          Logger.Info("Current Process Working set exceeds {0} MB, its {1} now.", (object) triggerThreshold, (object) (workingSet64 / 1048576L));
          MemoryManager.TriggerMemoryTrimInAndroid();
        }
label_3:;
      }))
      {
        IsBackground = true
      };
      MemoryManager.androidtrimMemoryThread.Start();
    }

    private static void TriggerMemoryTrimInAndroid()
    {
      try
      {
        int commandProcessorPort = Utils.GetBstCommandProcessorPort(MultiInstanceStrings.VmName);
        string str1 = "triggerMemoryTrim";
        string url = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "http://127.0.0.1:{0}/{1}", (object) commandProcessorPort, (object) str1);
        Logger.Info("Sending request to: " + url);
        string str2 = JObject.Parse(BstHttpClient.Get(url, (Dictionary<string, string>) null, false, MultiInstanceStrings.VmName, 0, 1, 0, false, "bgp"))["result"].ToString();
        Logger.Info("the result we get from {0} is {1}", (object) str1, (object) str2);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception occured when calling triggerMemoryTrim API of BstCommandProcessor. Err : {0}", (object) ex.ToString());
      }
    }
  }
}
