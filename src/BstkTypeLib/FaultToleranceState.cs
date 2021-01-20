// Decompiled with JetBrains decompiler
// Type: BstkTypeLib.FaultToleranceState
// Assembly: BstkTypeLib, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 38E91E34-8BF8-4856-A23F-FE231831C5D8
// Assembly location: C:\Program Files\BlueStacks\BstkTypeLib.dll

using System.Runtime.InteropServices;

namespace BstkTypeLib
{
  [Guid("5124F7EC-6B67-493C-9DEE-EE45A44114E1")]
  public enum FaultToleranceState
  {
    FaultToleranceState_Inactive = 1,
    FaultToleranceState_Master = 2,
    FaultToleranceState_Standby = 3,
  }
}
