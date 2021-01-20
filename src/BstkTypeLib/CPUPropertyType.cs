// Decompiled with JetBrains decompiler
// Type: BstkTypeLib.CPUPropertyType
// Assembly: BstkTypeLib, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 38E91E34-8BF8-4856-A23F-FE231831C5D8
// Assembly location: C:\Program Files\BlueStacks\BstkTypeLib.dll

using System.Runtime.InteropServices;

namespace BstkTypeLib
{
  [Guid("CC6ECDAD-A07C-4E81-9C0E-D767E0678D5A")]
  public enum CPUPropertyType
  {
    CPUPropertyType_Null,
    CPUPropertyType_PAE,
    CPUPropertyType_LongMode,
    CPUPropertyType_TripleFaultReset,
    CPUPropertyType_APIC,
    CPUPropertyType_X2APIC,
  }
}
