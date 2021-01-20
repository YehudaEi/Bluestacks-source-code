// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.GpsHelper
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using BlueStacks.Common;
using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace BlueStacks.Player
{
  internal class GpsHelper
  {
    [DllImport("HD-GpsLocator-Native.dll", CallingConvention = CallingConvention.StdCall)]
    private static extern void HdLoggerInit(Logger.HdLoggerCallback cb);

    [DllImport("HD-GpsLocator-Native.dll", CallingConvention = CallingConvention.StdCall)]
    public static extern int LaunchGpsLocator();

    internal static void Start()
    {
      try
      {
        Logger.Info("Starting Gps Locator");
        new Thread(new ThreadStart(GpsHelper.StartGpsLocator))
        {
          IsBackground = true
        }.Start();
      }
      catch (Exception ex)
      {
        Logger.Error("Error Occured, Err: {0}", (object) ex.ToString());
      }
    }

    private static void StartGpsLocator()
    {
      Logger.Info("Inside Start GpsLocator");
      try
      {
        try
        {
          Logger.Info("Checking if Gps Enabled");
          if (RegistryManager.Instance.DefaultGuest.GpsMode == 0)
          {
            Logger.Info("GpsMode is Disabled.");
            return;
          }
        }
        catch (Exception ex)
        {
          Logger.Error(string.Format("Error Occured, Err: {0}", (object) ex.ToString()));
        }
        System.Version version = new System.Version(6, 2, 9200, 0);
        if (Environment.OSVersion.Platform == PlatformID.Win32NT)
        {
          if (Environment.OSVersion.Version >= version)
          {
            try
            {
              GpsHelper.HdLoggerInit(Logger.GetHdLoggerCallback());
              GpsHelper.LaunchGpsLocator();
              Logger.Info("Back from Native Call");
              return;
            }
            catch (Exception ex)
            {
              Logger.Error(string.Format("Error Occured, Err: {0}", (object) ex.ToString()));
              return;
            }
          }
        }
        Logger.Warning("Need Windows 8 or Higher for GpsLocator to work.");
      }
      catch (Exception ex)
      {
        Logger.Error(string.Format("Exception Occured in StartGpsLocator. Err : {0}", (object) ex.ToString()));
      }
    }
  }
}
