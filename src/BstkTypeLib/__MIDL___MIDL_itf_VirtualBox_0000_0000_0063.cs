// Decompiled with JetBrains decompiler
// Type: BstkTypeLib.__MIDL___MIDL_itf_VirtualBox_0000_0000_0063
// Assembly: BstkTypeLib, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 38E91E34-8BF8-4856-A23F-FE231831C5D8
// Assembly location: C:\Program Files\BlueStacks\BstkTypeLib.dll

namespace BstkTypeLib
{
  public enum __MIDL___MIDL_itf_VirtualBox_0000_0000_0063
  {
    ProcessStatus_Undefined = 0,
    ProcessStatus_Starting = 10, // 0x0000000A
    ProcessStatus_Started = 100, // 0x00000064
    ProcessStatus_Paused = 110, // 0x0000006E
    ProcessStatus_Terminating = 480, // 0x000001E0
    ProcessStatus_TerminatedNormally = 500, // 0x000001F4
    ProcessStatus_TerminatedSignal = 510, // 0x000001FE
    ProcessStatus_TerminatedAbnormally = 511, // 0x000001FF
    ProcessStatus_TimedOutKilled = 512, // 0x00000200
    ProcessStatus_TimedOutAbnormally = 513, // 0x00000201
    ProcessStatus_Down = 600, // 0x00000258
    ProcessStatus_Error = 800, // 0x00000320
  }
}
