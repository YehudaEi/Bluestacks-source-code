// Decompiled with JetBrains decompiler
// Type: BstkTypeLib.ProcessCreateFlag
// Assembly: BstkTypeLib, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 38E91E34-8BF8-4856-A23F-FE231831C5D8
// Assembly location: C:\Program Files\BlueStacks\BstkTypeLib.dll

using System.Runtime.InteropServices;

namespace BstkTypeLib
{
  [Guid("C544CD2B-F02D-4886-9901-71C523DB8DC5")]
  public enum ProcessCreateFlag
  {
    ProcessCreateFlag_None = 0,
    ProcessCreateFlag_WaitForProcessStartOnly = 1,
    ProcessCreateFlag_IgnoreOrphanedProcesses = 2,
    ProcessCreateFlag_Hidden = 4,
    ProcessCreateFlag_Profile = 8,
    ProcessCreateFlag_WaitForStdOut = 16, // 0x00000010
    ProcessCreateFlag_WaitForStdErr = 32, // 0x00000020
    ProcessCreateFlag_ExpandArguments = 64, // 0x00000040
    ProcessCreateFlag_UnquotedArguments = 128, // 0x00000080
  }
}
