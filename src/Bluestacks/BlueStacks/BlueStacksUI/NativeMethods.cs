// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.NativeMethods
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BlueStacks.BlueStacksUI
{
  internal static class NativeMethods
  {
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool GetWindowRect(IntPtr hWnd, out BlueStacks.Common.RECT lpRect);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern bool SetWindowPos(
      IntPtr hWnd,
      IntPtr hWndInsertAfter,
      int X,
      int Y,
      int cx,
      int cy,
      uint uFlags);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern IntPtr SetWindowsHookEx(
      int idHook,
      GlobalKeyBoardMouseHooks.LowLevelMouseProc lpfn,
      IntPtr hMod,
      uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern IntPtr CallNextHookEx(
      IntPtr hhk,
      int nCode,
      IntPtr wParam,
      IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern IntPtr GetModuleHandle(string lpModuleName);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern IntPtr SetWindowsHookEx(
      int idHook,
      GlobalKeyBoardMouseHooks.LowLevelKeyboardProc lpfn,
      IntPtr hMod,
      uint dwThreadId);

    [DllImport("User32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern uint GetRawInputData(
      IntPtr hRawInput,
      uint uiCommand,
      IntPtr pData,
      ref uint pcbSize,
      uint cbSizeHeader);

    [DllImport("User32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern bool RegisterRawInputDevices(
      RawInputClass.RAWINPUTDEVICE[] pRawInputDevice,
      uint uiNumDevices,
      uint cbSize);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool GetCursorPos(ref NativeMethods.Win32Point pt);

    [DllImport("winmm.dll", CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern int waveOutGetVolume(IntPtr h, out uint dwVolume);

    [DllImport("winmm.dll", CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern int waveOutSetVolume(IntPtr h, uint dwVolume);

    [DllImport("urlmon.dll", CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern uint FindMimeFromData(
      uint pBC,
      [MarshalAs(UnmanagedType.LPStr)] string pwzUrl,
      [MarshalAs(UnmanagedType.LPArray)] byte[] pBuffer,
      uint cbSize,
      [MarshalAs(UnmanagedType.LPStr)] string pwzMimeProposed,
      uint dwMimeFlags,
      out uint ppwzMimeOut,
      uint dwReserverd);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern IntPtr SendMessage(
      IntPtr hWnd,
      uint msg,
      IntPtr wParam,
      IntPtr lParam);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);

    [DllImport("User32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern bool GetMonitorInfo(
      IntPtr hmonitor,
      [In, Out] WindowWndProcHandler.MONITORINFOEX info);

    [DllImport("shell32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern IntPtr SHAppBarMessage(
      int msg,
      ref WindowWndProcHandler.APPBARDATA data);

    [DllImport("user32.dll")]
    internal static extern bool ClientToScreen(IntPtr hWnd, ref Point lpPoint);

    [DllImport("user32.dll")]
    internal static extern uint SendInput(uint nInputs, [MarshalAs(UnmanagedType.LPArray), In] NativeMethods.Input[] pInputs, int cbSize);

    internal struct Win32Point
    {
      public int X;
      public int Y;
    }

    internal struct Input
    {
      public uint Type;
      public NativeMethods.MouseKeyboardHardwareInput Data;
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct MouseKeyboardHardwareInput
    {
      [FieldOffset(0)]
      public NativeMethods.MouseInput Mouse;
    }

    internal struct MouseInput
    {
      public int X { [IsReadOnly] get; set; }

      public int Y { [IsReadOnly] get; set; }

      public uint MouseData { [IsReadOnly] get; set; }

      public uint Flags { [IsReadOnly] get; set; }

      public uint Time { [IsReadOnly] get; set; }

      public IntPtr ExtraInfo { [IsReadOnly] get; set; }
    }

    [Flags]
    internal enum MouseEventFlags : uint
    {
      Move = 1,
      LeftDown = 2,
      LeftUp = 4,
      RightDown = 8,
      RightUp = 16, // 0x00000010
      MiddleDown = 32, // 0x00000020
      MiddleUp = 64, // 0x00000040
      XDown = 128, // 0x00000080
      XUp = 256, // 0x00000100
      Wheel = 2048, // 0x00000800
      Absolute = 32768, // 0x00008000
    }
  }
}
