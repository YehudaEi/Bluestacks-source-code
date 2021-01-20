// Decompiled with JetBrains decompiler
// Type: BstkTypeLib.PointingHIDType
// Assembly: BstkTypeLib, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 38E91E34-8BF8-4856-A23F-FE231831C5D8
// Assembly location: C:\Program Files\BlueStacks\BstkTypeLib.dll

using System.Runtime.InteropServices;

namespace BstkTypeLib
{
  [Guid("19964E93-0050-45C4-9382-A7BCCC53E666")]
  public enum PointingHIDType
  {
    PointingHIDType_None = 1,
    PointingHIDType_PS2Mouse = 2,
    PointingHIDType_USBMouse = 3,
    PointingHIDType_USBTablet = 4,
    PointingHIDType_ComboMouse = 5,
    PointingHIDType_USBMultiTouch = 6,
  }
}
