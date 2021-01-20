// Decompiled with JetBrains decompiler
// Type: BstkTypeLib.AdditionsFacilityClass
// Assembly: BstkTypeLib, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 38E91E34-8BF8-4856-A23F-FE231831C5D8
// Assembly location: C:\Program Files\BlueStacks\BstkTypeLib.dll

using System.Runtime.InteropServices;

namespace BstkTypeLib
{
  [Guid("446451B2-C88D-4E5D-84C9-91BC7F533F5F")]
  public enum AdditionsFacilityClass
  {
    AdditionsFacilityClass_None = 0,
    AdditionsFacilityClass_Driver = 10, // 0x0000000A
    AdditionsFacilityClass_Service = 30, // 0x0000001E
    AdditionsFacilityClass_Program = 50, // 0x00000032
    AdditionsFacilityClass_Feature = 100, // 0x00000064
    AdditionsFacilityClass_ThirdParty = 999, // 0x000003E7
    AdditionsFacilityClass_All = 2147483646, // 0x7FFFFFFE
  }
}
