// Decompiled with JetBrains decompiler
// Type: BstkTypeLib.SessionState
// Assembly: BstkTypeLib, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 38E91E34-8BF8-4856-A23F-FE231831C5D8
// Assembly location: C:\Program Files\BlueStacks\BstkTypeLib.dll

using System.Runtime.InteropServices;

namespace BstkTypeLib
{
  [Guid("CF2700C0-EA4B-47AE-9725-7810114B94D8")]
  public enum SessionState
  {
    SessionState_Null,
    SessionState_Unlocked,
    SessionState_Locked,
    SessionState_Spawning,
    SessionState_Unlocking,
  }
}
