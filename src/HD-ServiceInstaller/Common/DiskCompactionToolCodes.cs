// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.DiskCompactionToolCodes
// Assembly: HD-ServiceInstaller, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 15F93427-26B3-4C7E-BAB1-0A00688BC4D4
// Assembly location: C:\Program Files\BlueStacks\HD-ServiceInstaller.exe

namespace BlueStacks.Common
{
  public enum DiskCompactionToolCodes
  {
    VM_DONT_EXIST = -9, // 0xFFFFFFF7
    VBOX_ERROR_IN_FINAL_COMPACTION = -8, // 0xFFFFFFF8
    UNABLE_TO_FREE_DISK = -7, // 0xFFFFFFF9
    GUEST_NOT_BOOTED_FOR_RUNNING_ADB_COMMANDS = -6, // 0xFFFFFFFA
    ERROR_IN_MERGING_DISK = -5, // 0xFFFFFFFB
    VIRTUAL_BOX_INIT_FAILED = -4, // 0xFFFFFFFC
    UNHANDLED_EXCEPTION = -3, // 0xFFFFFFFD
    SOME_ERROR_WHILE_MERGING = -2, // 0xFFFFFFFE
    VERSION_NOT_SUPPORTED = -1, // 0xFFFFFFFF
    SUCCESS = 0,
  }
}
