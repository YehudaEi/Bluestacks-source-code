// Decompiled with JetBrains decompiler
// Type: BstkTypeLib.ProcessInputStatus
// Assembly: BstkTypeLib, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 38E91E34-8BF8-4856-A23F-FE231831C5D8
// Assembly location: C:\Program Files\BlueStacks\BstkTypeLib.dll

using System.Runtime.InteropServices;

namespace BstkTypeLib
{
  [Guid("A4A0EF9C-29CC-4805-9803-C8215AE9DA6C")]
  public enum ProcessInputStatus
  {
    ProcessInputStatus_Undefined = 0,
    ProcessInputStatus_Broken = 1,
    ProcessInputStatus_Available = 10, // 0x0000000A
    ProcessInputStatus_Written = 50, // 0x00000032
    ProcessInputStatus_Overflow = 100, // 0x00000064
  }
}
