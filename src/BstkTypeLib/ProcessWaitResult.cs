// Decompiled with JetBrains decompiler
// Type: BstkTypeLib.ProcessWaitResult
// Assembly: BstkTypeLib, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 38E91E34-8BF8-4856-A23F-FE231831C5D8
// Assembly location: C:\Program Files\BlueStacks\BstkTypeLib.dll

using System.Runtime.InteropServices;

namespace BstkTypeLib
{
  [Guid("40719CBE-F192-4FE9-A231-6697B3C8E2B4")]
  public enum ProcessWaitResult
  {
    ProcessWaitResult_None,
    ProcessWaitResult_Start,
    ProcessWaitResult_Terminate,
    ProcessWaitResult_Status,
    ProcessWaitResult_Error,
    ProcessWaitResult_Timeout,
    ProcessWaitResult_StdIn,
    ProcessWaitResult_StdOut,
    ProcessWaitResult_StdErr,
    ProcessWaitResult_WaitFlagNotSupported,
  }
}
