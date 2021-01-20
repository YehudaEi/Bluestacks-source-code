// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.RawInputClass
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System;
using System.Runtime.InteropServices;

namespace BlueStacks.BlueStacksUI
{
  internal class RawInputClass
  {
    private const int RID_INPUT = 268435459;
    private const int RIDEV_INPUTSINK = 256;

    internal static int GetDeviceID(IntPtr lParam)
    {
      try
      {
        uint pcbSize = 0;
        int rawInputData1 = (int) NativeMethods.GetRawInputData(lParam, 268435459U, IntPtr.Zero, ref pcbSize, (uint) Marshal.SizeOf(typeof (RawInputClass.RawInputHeader)));
        IntPtr num = Marshal.AllocHGlobal((int) pcbSize);
        int rawInputData2 = (int) NativeMethods.GetRawInputData(lParam, 268435459U, num, ref pcbSize, (uint) Marshal.SizeOf(typeof (RawInputClass.RawInputHeader)));
        RawInputClass.RawInput structure = (RawInputClass.RawInput) Marshal.PtrToStructure(num, typeof (RawInputClass.RawInput));
        Marshal.FreeHGlobal(num);
        return structure.Data.Mouse.ButtonFlags == RawMouseButtons.LeftDown || structure.Data.Mouse.ButtonFlags == RawMouseButtons.RightDown ? (int) structure.Header.Device : -1;
      }
      catch (Exception ex)
      {
        Logger.Info("Exception in raw input constructor : {0}", (object) ex.ToString());
      }
      return -1;
    }

    public RawInputClass(IntPtr hwnd)
    {
      try
      {
        RawInputClass.RAWINPUTDEVICE[] pRawInputDevice = new RawInputClass.RAWINPUTDEVICE[3];
        pRawInputDevice[0].usUsagePage = (ushort) 1;
        pRawInputDevice[0].usUsage = (ushort) 2;
        pRawInputDevice[0].dwFlags = 256;
        pRawInputDevice[0].hwndTarget = hwnd;
        pRawInputDevice[1].usUsagePage = (ushort) 1;
        pRawInputDevice[1].usUsage = (ushort) 5;
        pRawInputDevice[1].dwFlags = 256;
        pRawInputDevice[1].hwndTarget = hwnd;
        pRawInputDevice[2].usUsagePage = (ushort) 1;
        pRawInputDevice[2].usUsage = (ushort) 4;
        pRawInputDevice[2].dwFlags = 256;
        pRawInputDevice[2].hwndTarget = hwnd;
        if (!NativeMethods.RegisterRawInputDevices(pRawInputDevice, (uint) pRawInputDevice.Length, (uint) Marshal.SizeOf((object) pRawInputDevice[0])))
          Logger.Info("Failed to register raw input device(s).");
        else
          Logger.Info("Successfully registered raw input device(s).");
      }
      catch (Exception ex)
      {
        Logger.Info("Exception in raw input constructor : {0}", (object) ex.ToString());
      }
    }

    internal struct RAWINPUTDEVICE
    {
      [MarshalAs(UnmanagedType.U2)]
      public ushort usUsagePage;
      [MarshalAs(UnmanagedType.U2)]
      public ushort usUsage;
      [MarshalAs(UnmanagedType.U4)]
      public int dwFlags;
      public IntPtr hwndTarget;
    }

    internal struct RAWHID
    {
      [MarshalAs(UnmanagedType.U4)]
      public int dwSizHid;
      [MarshalAs(UnmanagedType.U4)]
      public int dwCount;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct RawMouse
    {
      [FieldOffset(0)]
      public RawMouseFlags Flags;
      [FieldOffset(4)]
      public RawMouseButtons ButtonFlags;
      [FieldOffset(6)]
      public ushort ButtonData;
      [FieldOffset(8)]
      public uint RawButtons;
      [FieldOffset(12)]
      public int LastX;
      [FieldOffset(16)]
      public int LastY;
      [FieldOffset(20)]
      public uint ExtraInformation;
    }

    internal struct RAWKEYBOARD
    {
      [MarshalAs(UnmanagedType.U2)]
      public ushort MakeCode;
      [MarshalAs(UnmanagedType.U2)]
      public ushort Flags;
      [MarshalAs(UnmanagedType.U2)]
      public ushort Reserved;
      [MarshalAs(UnmanagedType.U2)]
      public ushort VKey;
      [MarshalAs(UnmanagedType.U4)]
      public uint Message;
      [MarshalAs(UnmanagedType.U4)]
      public uint ExtraInformation;
    }

    public enum RawInputType
    {
      Mouse,
      Keyboard,
      HID,
    }

    public struct RawInput
    {
      public RawInputClass.RawInputHeader Header;
      public RawInputClass.RawInput.Union Data;

      public RawInput(RawInputClass.RawInputHeader _header, RawInputClass.RawInput.Union _data)
      {
        this.Header = _header;
        this.Data = _data;
      }

      [StructLayout(LayoutKind.Explicit)]
      public struct Union
      {
        [FieldOffset(0)]
        public RawInputClass.RawMouse Mouse;
        [FieldOffset(0)]
        public RawInputClass.RAWKEYBOARD Keyboard;
        [FieldOffset(0)]
        public RawInputClass.RAWHID HID;
      }
    }

    internal struct RawInputHeader
    {
      public RawInputClass.RawInputType Type;
      public int Size;
      public IntPtr Device;
      public IntPtr wParam;
    }
  }
}
