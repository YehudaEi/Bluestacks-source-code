// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.HDPlusModule
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;

namespace BlueStacks.Player
{
  public static class HDPlusModule
  {
    public const string CAMERA_DLL = "HD-Plus-Camera-Native.dll";
    public const string FRONTEND_DLL = "HD-Plus-Frontend-Native.dll";

    private static string CameraDllName()
    {
      return "HD-Plus-Camera-Native.dll";
    }

    private static string FrontendDllName()
    {
      return "HD-Plus-Frontend-Native.dll";
    }

    [DllImport("HD-Plus-Frontend-Native.dll", SetLastError = true)]
    private static extern IntPtr ManagerOpen();

    [DllImport("HD-Plus-Camera-Native.dll")]
    public static extern void SetStartStopCamerCB(CameraManager.fpStartStopCamera func);

    [DllImport("HD-Plus-Camera-Native.dll")]
    public static extern int CameraIoProcessMessages(IntPtr ioHandle);

    [DllImport("HD-Plus-Camera-Native.dll")]
    public static extern bool CameraSendCaptureStream(
      IntPtr handle,
      IntPtr stream,
      int size,
      int width,
      int height,
      int stride);

    [DllImport("HD-Plus-Camera-Native.dll")]
    public static extern bool MonitorSendCaptureStream(
      IntPtr handle,
      IntPtr stream,
      int size,
      IntPtr over,
      int width,
      int height,
      int stride);

    [DllImport("HD-Plus-Camera-Native.dll", SetLastError = true)]
    public static extern IntPtr CameraIoAttach(uint vmId);

    [DllImport("HD-Plus-Camera-Native.dll", SetLastError = true)]
    public static extern IntPtr MonitorCreateOverWrite();

    [DllImport("HD-Plus-Camera-Native.dll", SetLastError = true)]
    public static extern bool convertRGB24toYUV422(IntPtr src, int w, int h, IntPtr dst);

    [DllImport("HD-Plus-Frontend-Native.dll", SetLastError = true)]
    public static extern bool ManagerAttachWithListener(SafeFileHandle handle, uint id, uint cls);

    [DllImport("HD-Plus-Frontend-Native.dll", SetLastError = true)]
    public static extern bool MonitorSendMesg(SafeFileHandle handle, IntPtr msg);

    [DllImport("HD-Plus-Frontend-Native.dll", SetLastError = true)]
    public static extern bool MonitorRecvMesg(
      SafeFileHandle handle,
      VmMonitor.ReceiverCallback callback,
      SafeWaitHandle wakeupEvent);

    [DllImport("HD-Plus-Frontend-Native.dll", SetLastError = true)]
    public static extern bool SensorSendMsg(IntPtr msg);

    [DllImport("HD-Plus-Frontend-Native.dll", SetLastError = true)]
    public static extern bool SensorRecvMsg(VmMonitor.ReceiverCallback callback);

    [DllImport("HD-Plus-Frontend-Native.dll", SetLastError = true)]
    public static extern int ManagerList(IntPtr handle, uint[] list, int count);

    [DllImport("HD-Plus-Frontend-Native.dll", SetLastError = true)]
    public static extern bool ManagerAttach(IntPtr handle, uint id);

    [DllImport("HD-Plus-Frontend-Native.dll", SetLastError = true)]
    public static extern bool ManagerIsVmxActive();

    [DllImport("HD-Plus-Frontend-Native.dll", SetLastError = true)]
    public static extern void MonitorSetLogger(Monitor.LoggerCallback callback);

    [DllImport("HD-Plus-Camera-Native.dll", SetLastError = true)]
    public static extern void CameraSetLogger(Monitor.LoggerCallback callback);

    [DllImport("HD-Plus-Frontend-Native.dll", SetLastError = true)]
    public static extern IntPtr MonitorVideoAttach(IntPtr handle);

    [DllImport("HD-Plus-Frontend-Native.dll", SetLastError = true)]
    public static extern IntPtr MonitorVideoAttach(uint id, bool verbose);

    [DllImport("HD-Plus-Frontend-Native.dll", SetLastError = true)]
    public static extern bool MonitorVideoDetach(IntPtr addr);

    [DllImport("HD-Plus-Frontend-Native.dll")]
    public static extern bool MonitorSendScanCode(byte code);

    [DllImport("HD-Plus-Frontend-Native.dll")]
    public static extern bool MonitorSendMouseState(uint x, uint y, uint mask);

    [DllImport("HD-Plus-Frontend-Native.dll")]
    public static extern bool MonitorSendTouchState(Monitor.TouchPoint[] points, int count);

    [DllImport("HD-Plus-Frontend-Native.dll")]
    public static extern bool MonitorSendControl(Monitor.BstInputControlType type);

    [DllImport("HD-Plus-Frontend-Native.dll")]
    public static extern bool MonitorSendLocation([MarshalAs(UnmanagedType.Struct)] GPSManager.GpsLocation location);

    [DllImport("HD-Plus-Frontend-Native.dll", SetLastError = true)]
    public static extern bool MonitorSendCaptureStream(IntPtr handle, IntPtr streamBuf, int size);

    [DllImport("HD-Plus-Frontend-Native.dll")]
    public static extern bool VideoCheckMagic(IntPtr addr, ref uint magic);

    [DllImport("HD-Plus-Frontend-Native.dll")]
    public static extern void VideoGetMode(
      IntPtr addr,
      ref uint width,
      ref uint height,
      ref uint depth);

    [DllImport("HD-Plus-Frontend-Native.dll")]
    public static extern bool VideoGetAndClearDirty(IntPtr addr);

    [DllImport("HD-Plus-Frontend-Native.dll")]
    public static extern bool SetMouseHWheelCallback(MouseHWheel.MouseHWheelCallback func);

    [DllImport("HD-Plus-Frontend-Native.dll")]
    public static extern int MonitorSendImeMsg(byte[] buf, int len);

    private static SafeFileHandle ManagerOpenSafe()
    {
      return new SafeFileHandle(HDPlusModule.ManagerOpen(), true);
    }
  }
}
