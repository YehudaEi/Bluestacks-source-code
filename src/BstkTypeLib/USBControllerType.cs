// Decompiled with JetBrains decompiler
// Type: BstkTypeLib.USBControllerType
// Assembly: BstkTypeLib, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 38E91E34-8BF8-4856-A23F-FE231831C5D8
// Assembly location: C:\Program Files\BlueStacks\BstkTypeLib.dll

using System.Runtime.InteropServices;

namespace BstkTypeLib
{
  [Guid("8FDD1C6A-5412-41DA-AB07-7BAED7D6E18E")]
  public enum USBControllerType
  {
    USBControllerType_Null,
    USBControllerType_OHCI,
    USBControllerType_EHCI,
    USBControllerType_XHCI,
    USBControllerType_Last,
  }
}
