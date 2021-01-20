// Decompiled with JetBrains decompiler
// Type: BstkTypeLib.MediumVariant
// Assembly: BstkTypeLib, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 38E91E34-8BF8-4856-A23F-FE231831C5D8
// Assembly location: C:\Program Files\BlueStacks\BstkTypeLib.dll

using System.Runtime.InteropServices;

namespace BstkTypeLib
{
  [Guid("0282E97F-4EF3-4411-A8E0-47C384803CB6")]
  public enum MediumVariant
  {
    MediumVariant_Standard = 0,
    MediumVariant_VmdkSplit2G = 1,
    MediumVariant_VmdkRawDisk = 2,
    MediumVariant_VmdkStreamOptimized = 4,
    MediumVariant_VmdkESX = 8,
    MediumVariant_VdiZeroExpand = 256, // 0x00000100
    MediumVariant_Fixed = 65536, // 0x00010000
    MediumVariant_Diff = 131072, // 0x00020000
    MediumVariant_NoCreateDir = 1073741824, // 0x40000000
  }
}
