// Decompiled with JetBrains decompiler
// Type: BstkTypeLib.ExportOptions
// Assembly: BstkTypeLib, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 38E91E34-8BF8-4856-A23F-FE231831C5D8
// Assembly location: C:\Program Files\BlueStacks\BstkTypeLib.dll

using System.Runtime.InteropServices;

namespace BstkTypeLib
{
  [Guid("8F45EB08-FD34-41EE-AF95-A880BDEE5554")]
  public enum ExportOptions
  {
    ExportOptions_CreateManifest = 1,
    ExportOptions_ExportDVDImages = 2,
    ExportOptions_StripAllMACs = 3,
    ExportOptions_StripAllNonNATMACs = 4,
  }
}
