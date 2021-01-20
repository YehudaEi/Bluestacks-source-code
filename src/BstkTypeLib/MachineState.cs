// Decompiled with JetBrains decompiler
// Type: BstkTypeLib.MachineState
// Assembly: BstkTypeLib, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 38E91E34-8BF8-4856-A23F-FE231831C5D8
// Assembly location: C:\Program Files\BlueStacks\BstkTypeLib.dll

using System.Runtime.InteropServices;

namespace BstkTypeLib
{
  [Guid("87F085C3-CA67-4E45-9225-6057F32E9E8E")]
  public enum MachineState
  {
    MachineState_Null = 0,
    MachineState_PoweredOff = 1,
    MachineState_Saved = 2,
    MachineState_Teleported = 3,
    MachineState_Aborted = 4,
    MachineState_FirstOnline = 5,
    MachineState_Running = 5,
    MachineState_Paused = 6,
    MachineState_Stuck = 7,
    MachineState_FirstTransient = 8,
    MachineState_Teleporting = 8,
    MachineState_LiveSnapshotting = 9,
    MachineState_Starting = 10, // 0x0000000A
    MachineState_Stopping = 11, // 0x0000000B
    MachineState_Saving = 12, // 0x0000000C
    MachineState_Restoring = 13, // 0x0000000D
    MachineState_TeleportingPausedVM = 14, // 0x0000000E
    MachineState_TeleportingIn = 15, // 0x0000000F
    MachineState_FaultTolerantSyncing = 16, // 0x00000010
    MachineState_DeletingSnapshotOnline = 17, // 0x00000011
    MachineState_DeletingSnapshotPaused = 18, // 0x00000012
    MachineState_LastOnline = 19, // 0x00000013
    MachineState_OnlineSnapshotting = 19, // 0x00000013
    MachineState_RestoringSnapshot = 20, // 0x00000014
    MachineState_DeletingSnapshot = 21, // 0x00000015
    MachineState_SettingUp = 22, // 0x00000016
    MachineState_LastTransient = 23, // 0x00000017
    MachineState_Snapshotting = 23, // 0x00000017
  }
}
