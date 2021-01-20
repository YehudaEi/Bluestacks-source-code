// Decompiled with JetBrains decompiler
// Type: BstkTypeLib.AdditionsFacilityStatus
// Assembly: BstkTypeLib, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 38E91E34-8BF8-4856-A23F-FE231831C5D8
// Assembly location: C:\Program Files\BlueStacks\BstkTypeLib.dll

using System.Runtime.InteropServices;

namespace BstkTypeLib
{
  [Guid("CE06F9E1-394E-4FE9-9368-5A88C567DBDE")]
  public enum AdditionsFacilityStatus
  {
    AdditionsFacilityStatus_Inactive = 0,
    AdditionsFacilityStatus_Paused = 1,
    AdditionsFacilityStatus_PreInit = 20, // 0x00000014
    AdditionsFacilityStatus_Init = 30, // 0x0000001E
    AdditionsFacilityStatus_Active = 50, // 0x00000032
    AdditionsFacilityStatus_Terminating = 100, // 0x00000064
    AdditionsFacilityStatus_Terminated = 101, // 0x00000065
    AdditionsFacilityStatus_Failed = 800, // 0x00000320
    AdditionsFacilityStatus_Unknown = 999, // 0x000003E7
  }
}
