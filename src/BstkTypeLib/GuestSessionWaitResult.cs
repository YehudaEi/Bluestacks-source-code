// Decompiled with JetBrains decompiler
// Type: BstkTypeLib.GuestSessionWaitResult
// Assembly: BstkTypeLib, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 38E91E34-8BF8-4856-A23F-FE231831C5D8
// Assembly location: C:\Program Files\BlueStacks\BstkTypeLib.dll

using System.Runtime.InteropServices;

namespace BstkTypeLib
{
  [Guid("C0F6A8A5-FDB6-42BF-A582-56C6F82BCD2D")]
  public enum GuestSessionWaitResult
  {
    GuestSessionWaitResult_None,
    GuestSessionWaitResult_Start,
    GuestSessionWaitResult_Terminate,
    GuestSessionWaitResult_Status,
    GuestSessionWaitResult_Error,
    GuestSessionWaitResult_Timeout,
    GuestSessionWaitResult_WaitFlagNotSupported,
  }
}
