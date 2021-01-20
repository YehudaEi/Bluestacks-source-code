// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.GPSManager
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using BlueStacks.Common;
using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;

namespace BlueStacks.Player
{
  public class GPSManager
  {
    private static IntPtr s_IoHandle = IntPtr.Zero;
    private static object s_IoHandleLock = new object();
    private static GPSManager sInstance = new GPSManager();
    private const string NATIVE_DLL = "HD-Gps-Native.dll";
    public static GPSManager.GpsLocation location;
    private static Monitor sMonitor;

    [DllImport("kernel32.dll")]
    private static extern bool CloseHandle(IntPtr handle);

    [DllImport("HD-Gps-Native.dll", SetLastError = true)]
    private static extern IntPtr GpsIoAttach(uint vmId);

    [DllImport("HD-Gps-Native.dll", SetLastError = true)]
    private static extern int GpsIoProcessMessages(IntPtr ioHandle);

    public void SetMonitor(Monitor monitor)
    {
      GPSManager.sMonitor = monitor;
    }

    public static GPSManager Instance()
    {
      return GPSManager.sInstance;
    }

    public static void Init()
    {
      Logger.Debug("Waiting for Gps messages...");
      new Thread((ThreadStart) (() =>
      {
        while (true)
        {
          try
          {
            GPSManager.location.latitude = Convert.ToDouble(RegistryManager.Instance.DefaultGuest.GpsLatitude, (IFormatProvider) CultureInfo.InvariantCulture);
            GPSManager.location.longitude = Convert.ToDouble(RegistryManager.Instance.DefaultGuest.GpsLongitude, (IFormatProvider) CultureInfo.InvariantCulture);
          }
          catch (Exception ex)
          {
            Logger.Error(ex.ToString());
            Logger.Error("GPS: Exiting thread.");
            break;
          }
          Logger.Debug("Sending GPS location...");
          GPSManager.sMonitor.SendLocation(GPSManager.location);
          Thread.Sleep(60000);
        }
      }))
      {
        IsBackground = true
      }.Start();
    }

    public static void Shutdown()
    {
      lock (GPSManager.s_IoHandleLock)
      {
        if (!(GPSManager.s_IoHandle != IntPtr.Zero))
          return;
        Logger.Debug("Shutting down gps...\n");
        GPSManager.CloseHandle(GPSManager.s_IoHandle);
        GPSManager.s_IoHandle = IntPtr.Zero;
      }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct GpsLocation
    {
      public double latitude;
      public double longitude;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
      public string country;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
      public string city;
    }
  }
}
