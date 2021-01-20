// Decompiled with JetBrains decompiler
// Type: BstkTypeLib.GuestSessionStatus
// Assembly: BstkTypeLib, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 38E91E34-8BF8-4856-A23F-FE231831C5D8
// Assembly location: C:\Program Files\BlueStacks\BstkTypeLib.dll

using System.Runtime.InteropServices;

namespace BstkTypeLib
{
  [Guid("AC2669DA-4624-44F2-85B5-0B0BFB8D8673")]
  public enum GuestSessionStatus
  {
    GuestSessionStatus_Undefined = 0,
    GuestSessionStatus_Starting = 10, // 0x0000000A
    GuestSessionStatus_Started = 100, // 0x00000064
    GuestSessionStatus_Terminating = 480, // 0x000001E0
    GuestSessionStatus_Terminated = 500, // 0x000001F4
    GuestSessionStatus_TimedOutKilled = 512, // 0x00000200
    GuestSessionStatus_TimedOutAbnormally = 513, // 0x00000201
    GuestSessionStatus_Down = 600, // 0x00000258
    GuestSessionStatus_Error = 800, // 0x00000320
  }
}
