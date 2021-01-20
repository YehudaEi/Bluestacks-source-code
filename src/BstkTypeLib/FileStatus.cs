// Decompiled with JetBrains decompiler
// Type: BstkTypeLib.FileStatus
// Assembly: BstkTypeLib, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 38E91E34-8BF8-4856-A23F-FE231831C5D8
// Assembly location: C:\Program Files\BlueStacks\BstkTypeLib.dll

using System.Runtime.InteropServices;

namespace BstkTypeLib
{
  [Guid("8C86468B-B97B-4080-8914-E29F5B0ABD2C")]
  public enum FileStatus
  {
    FileStatus_Undefined = 0,
    FileStatus_Opening = 10, // 0x0000000A
    FileStatus_Open = 100, // 0x00000064
    FileStatus_Closing = 150, // 0x00000096
    FileStatus_Closed = 200, // 0x000000C8
    FileStatus_Down = 600, // 0x00000258
    FileStatus_Error = 800, // 0x00000320
  }
}
