// Decompiled with JetBrains decompiler
// Type: BstkTypeLib.AdditionsFacilityType
// Assembly: BstkTypeLib, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 38E91E34-8BF8-4856-A23F-FE231831C5D8
// Assembly location: C:\Program Files\BlueStacks\BstkTypeLib.dll

using System.Runtime.InteropServices;

namespace BstkTypeLib
{
  [Guid("98F7F957-89FB-49B6-A3B1-31E3285EB1D8")]
  public enum AdditionsFacilityType
  {
    AdditionsFacilityType_None = 0,
    AdditionsFacilityType_VBoxGuestDriver = 20, // 0x00000014
    AdditionsFacilityType_AutoLogon = 90, // 0x0000005A
    AdditionsFacilityType_VBoxService = 100, // 0x00000064
    AdditionsFacilityType_VBoxTrayClient = 101, // 0x00000065
    AdditionsFacilityType_Seamless = 1000, // 0x000003E8
    AdditionsFacilityType_Graphics = 1100, // 0x0000044C
    AdditionsFacilityType_All = 2147483646, // 0x7FFFFFFE
  }
}
