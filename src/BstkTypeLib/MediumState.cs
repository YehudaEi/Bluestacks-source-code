// Decompiled with JetBrains decompiler
// Type: BstkTypeLib.MediumState
// Assembly: BstkTypeLib, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 38E91E34-8BF8-4856-A23F-FE231831C5D8
// Assembly location: C:\Program Files\BlueStacks\BstkTypeLib.dll

using System.Runtime.InteropServices;

namespace BstkTypeLib
{
  [Guid("EF41E980-E012-43CD-9DEA-479D4EF14D13")]
  public enum MediumState
  {
    MediumState_NotCreated,
    MediumState_Created,
    MediumState_LockedRead,
    MediumState_LockedWrite,
    MediumState_Inaccessible,
    MediumState_Creating,
    MediumState_Deleting,
  }
}
