// Decompiled with JetBrains decompiler
// Type: BstkTypeLib.USBDeviceState
// Assembly: BstkTypeLib, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 38E91E34-8BF8-4856-A23F-FE231831C5D8
// Assembly location: C:\Program Files\BlueStacks\BstkTypeLib.dll

using System.Runtime.InteropServices;

namespace BstkTypeLib
{
  [Guid("B99A2E65-67FB-4882-82FD-F3E5E8193AB4")]
  public enum USBDeviceState
  {
    USBDeviceState_NotSupported,
    USBDeviceState_Unavailable,
    USBDeviceState_Busy,
    USBDeviceState_Available,
    USBDeviceState_Held,
    USBDeviceState_Captured,
  }
}
