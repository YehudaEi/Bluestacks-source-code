// Decompiled with JetBrains decompiler
// Type: BstkTypeLib.CleanupMode
// Assembly: BstkTypeLib, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 38E91E34-8BF8-4856-A23F-FE231831C5D8
// Assembly location: C:\Program Files\BlueStacks\BstkTypeLib.dll

using System.Runtime.InteropServices;

namespace BstkTypeLib
{
  [Guid("67897C50-7CCA-47A9-83F6-CE8FD8EB5441")]
  public enum CleanupMode
  {
    CleanupMode_UnregisterOnly = 1,
    CleanupMode_DetachAllReturnNone = 2,
    CleanupMode_DetachAllReturnHardDisksOnly = 3,
    CleanupMode_Full = 4,
  }
}
