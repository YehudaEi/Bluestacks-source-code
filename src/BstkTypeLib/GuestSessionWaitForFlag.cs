// Decompiled with JetBrains decompiler
// Type: BstkTypeLib.GuestSessionWaitForFlag
// Assembly: BstkTypeLib, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 38E91E34-8BF8-4856-A23F-FE231831C5D8
// Assembly location: C:\Program Files\BlueStacks\BstkTypeLib.dll

using System.Runtime.InteropServices;

namespace BstkTypeLib
{
  [Guid("BB7A372A-F635-4E11-A81A-E707F3A52EF5")]
  public enum GuestSessionWaitForFlag
  {
    GuestSessionWaitForFlag_None = 0,
    GuestSessionWaitForFlag_Start = 1,
    GuestSessionWaitForFlag_Terminate = 2,
    GuestSessionWaitForFlag_Status = 4,
  }
}
