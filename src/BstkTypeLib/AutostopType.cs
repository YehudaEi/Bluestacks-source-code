// Decompiled with JetBrains decompiler
// Type: BstkTypeLib.AutostopType
// Assembly: BstkTypeLib, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 38E91E34-8BF8-4856-A23F-FE231831C5D8
// Assembly location: C:\Program Files\BlueStacks\BstkTypeLib.dll

using System.Runtime.InteropServices;

namespace BstkTypeLib
{
  [Guid("6BB96740-CF34-470D-AAB2-2CD48EA2E10E")]
  public enum AutostopType
  {
    AutostopType_Disabled = 1,
    AutostopType_SaveState = 2,
    AutostopType_PowerOff = 3,
    AutostopType_AcpiShutdown = 4,
  }
}
