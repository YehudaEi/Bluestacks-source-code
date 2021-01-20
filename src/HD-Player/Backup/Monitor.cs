// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.Monitor
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using BlueStacks.Common;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace BlueStacks.Player
{
  public class Monitor
  {
    private static Monitor.LoggerCallback sLoggerCallback = (Monitor.LoggerCallback) (msg => Logger.Info("Monitor: " + msg));
    private static Monitor.LoggerCallback camLoggerCallback;
    private uint mId;
    private IntPtr handle;
    private uint id;

    public Monitor(IntPtr handle, uint id, Monitor.ExitHandler exitHandler)
    {
      this.handle = handle;
      this.id = id;
      new Thread((ThreadStart) (() =>
      {
        while (ProcessUtils.IsProcessAlive(Convert.ToInt32(id)))
          Thread.Sleep(1000);
        exitHandler();
      }))
      {
        IsBackground = true
      }.Start();
    }

    static Monitor()
    {
      HDPlusModule.MonitorSetLogger(Monitor.sLoggerCallback);
      Monitor.camLoggerCallback = (Monitor.LoggerCallback) (msg => Logger.Info("Camera: " + msg));
      HDPlusModule.CameraSetLogger(Monitor.camLoggerCallback);
    }

    public Monitor(uint id, bool verbose)
    {
      this.mId = id;
    }

    public Monitor(uint id)
    {
      this.mId = id;
    }

    public void Close()
    {
    }

    public Video VideoAttach(bool verbose)
    {
      IntPtr zero = IntPtr.Zero;
      IntPtr addr = HDPlusModule.MonitorVideoAttach(this.mId, verbose);
      if (addr == IntPtr.Zero)
        CommonError.ThrowLastWin32Error(string.Format("FATAL ERROR: Cannot attach to monitor video: {0}", (object) Marshal.GetLastWin32Error()));
      Video video = new Video(addr);
      try
      {
        video.CheckMagic();
      }
      catch (Exception ex)
      {
        HDPlusModule.MonitorVideoDetach(addr);
        throw;
      }
      Logger.Info("Video Attached");
      return video;
    }

    public void SendScanCode(byte code)
    {
      if (!HDPlusModule.MonitorSendScanCode(code))
        throw new IOException("Cannot send keyboard scan code");
    }

    public void SendLocation(GPSManager.GpsLocation location)
    {
      if (HDPlusModule.MonitorSendLocation(location))
        return;
      CommonError.ThrowLastWin32Error("Cannot send GPS location update");
    }

    public void SendMouseState(uint x, uint y, uint mask)
    {
      if (!HDPlusModule.MonitorSendMouseState(x, y, mask))
        throw new IOException("Cannot send mouse state");
    }

    public void SendControl(Monitor.BstInputControlType type)
    {
      if (!HDPlusModule.MonitorSendControl(type))
        throw new IOException("Cannot send control state state");
    }

    public void SendTouchState(Monitor.TouchPoint[] points)
    {
      if (points == null)
        points = new Monitor.TouchPoint[0];
      if (!HDPlusModule.MonitorSendTouchState(points, points.Length))
        throw new IOException("Cannot send touch state");
    }

    internal void SendAndroidString(string formattedString)
    {
      try
      {
        byte[] bytes = Encoding.UTF8.GetBytes(formattedString);
        HDPlusModule.MonitorSendImeMsg(bytes, bytes.Length);
      }
      catch
      {
      }
    }

    public delegate void LoggerCallback(string msg);

    public delegate void ExitHandler();

    private delegate void ReadCallback();

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TouchPoint
    {
      public int PosX;
      public int PosY;

      public TouchPoint(int dummy = 0)
      {
        this.PosX = (int) ushort.MaxValue;
        this.PosY = (int) ushort.MaxValue;
      }
    }

    public enum BstInputControlType
    {
      BST_INPUT_CONTROL_TYPE_NONE,
      BST_INPUT_CONTROL_TYPE_SHUTDOWN,
      BST_INPUT_CONTROL_TYPE_STOP,
      BST_INPUT_CONTROL_TYPE_START,
    }
  }
}
