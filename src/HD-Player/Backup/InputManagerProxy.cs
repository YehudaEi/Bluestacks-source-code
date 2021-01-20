// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.InputManagerProxy
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using BlueStacks.Common;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;

namespace BlueStacks.Player
{
  internal class InputManagerProxy : IDisposable
  {
    private int mId;
    private const int CONTROL_TYPE_SHUTDOWN = 1;
    private static Monitor.LoggerCallback mLoggerCallback;

    [DllImport("HD-Plus-Service-Native.dll", SetLastError = true)]
    private static extern bool MonitorSendControl(int control);

    [DllImport("kernel32.dll")]
    public static extern IntPtr LoadLibrary(string dllToLoad);

    [DllImport("kernel32.dll")]
    public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

    public InputManagerProxy(int id)
    {
      this.mId = id;
    }

    internal static void SetUp()
    {
      InputManagerProxy.mLoggerCallback = (Monitor.LoggerCallback) (msg => Logger.Info("HyperV: " + msg));
      string dllToLoad = "HD-Plus-Service-Native.dll";
      IntPtr hModule = InputManagerProxy.LoadLibrary(dllToLoad);
      if (hModule == IntPtr.Zero)
      {
        Logger.Info("Failed to {0} dll", (object) dllToLoad);
      }
      else
      {
        IntPtr procAddress = InputManagerProxy.GetProcAddress(hModule, "HyperVLog");
        if (procAddress == IntPtr.Zero)
          Logger.Info("function pointer is null");
        else
          ((InputManagerProxy.HyperVLog) Marshal.GetDelegateForFunctionPointer(procAddress, typeof (InputManagerProxy.HyperVLog)))(InputManagerProxy.mLoggerCallback);
      }
    }

    public void SendControlShutdown()
    {
      Logger.Info("{0}", (object) MethodBase.GetCurrentMethod().Name);
      if (InputManagerProxy.MonitorSendControl(1))
        return;
      InputManagerProxy.ThrowLastWin32Error("Cannot send shutdown control");
    }

    public void Dispose()
    {
      GC.SuppressFinalize((object) this);
    }

    private static void ThrowLastWin32Error(string msg)
    {
      throw new SystemException(msg, (Exception) new Win32Exception(Marshal.GetLastWin32Error()));
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void HyperVLog(Monitor.LoggerCallback logger);
  }
}
