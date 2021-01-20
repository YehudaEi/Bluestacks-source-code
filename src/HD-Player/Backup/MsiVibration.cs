// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.MsiVibration
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using BlueStacks.Common;
using System;
using System.Runtime.InteropServices;

namespace BlueStacks.Player
{
  public class MsiVibration
  {
    private static MsiVibration.ReleaseDll releaseDll = (MsiVibration.ReleaseDll) null;
    private static MsiVibration.SetKBVibration setKBVibration = (MsiVibration.SetKBVibration) null;
    private static IntPtr hModule = IntPtr.Zero;

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)] string lpLibFileName);

    [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
    private static extern IntPtr GetProcAddress(IntPtr hModule, [MarshalAs(UnmanagedType.LPStr)] string lpProcName);

    [DllImport("kernel32.dll")]
    private static extern bool FreeLibrary(int hModule);

    public static bool Init()
    {
      MsiVibration.hModule = !SystemUtils.IsOs64Bit() ? MsiVibration.LoadLibrary("MsiKBVibration.dll") : MsiVibration.LoadLibrary("MsiKBVibration64.dll");
      if (MsiVibration.hModule == IntPtr.Zero || !((MsiVibration.InitDll) Marshal.GetDelegateForFunctionPointer(MsiVibration.GetProcAddress(MsiVibration.hModule, "InitDLL"), typeof (MsiVibration.InitDll)))())
        return false;
      MsiVibration.releaseDll = (MsiVibration.ReleaseDll) Marshal.GetDelegateForFunctionPointer(MsiVibration.GetProcAddress(MsiVibration.hModule, "ReleaseDLL"), typeof (MsiVibration.ReleaseDll));
      MsiVibration.setKBVibration = (MsiVibration.SetKBVibration) Marshal.GetDelegateForFunctionPointer(MsiVibration.GetProcAddress(MsiVibration.hModule, "SetKBVibration"), typeof (MsiVibration.SetKBVibration));
      return true;
    }

    public static void Release()
    {
      int num = MsiVibration.releaseDll() ? 1 : 0;
      MsiVibration.FreeLibrary(MsiVibration.hModule.ToInt32());
    }

    public static void SetVibration(int duration)
    {
      MsiVibration.setKBVibration(duration);
    }

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate bool InitDll();

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate bool ReleaseDll();

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void SetKBVibration(int milliSeconds);
  }
}
