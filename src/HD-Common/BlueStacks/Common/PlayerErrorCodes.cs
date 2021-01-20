// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.PlayerErrorCodes
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

namespace BlueStacks.Common
{
  public enum PlayerErrorCodes
  {
    HYPER_V_COMPUTE_PLATFORM_NOTAVAIL = -12, // 0xFFFFFFF4
    HYPER_V_DISABLED = -11, // 0xFFFFFFF5
    VTX_DISABLED = -10, // 0xFFFFFFF6
    FORCE_SHUTDOWN = -9, // 0xFFFFFFF7
    VBOX_BRIDGE_EXCEPTION = -8, // 0xFFFFFFF8
    VBOX_DRIVER_NOT_INSTALLED = -7, // 0xFFFFFFF9
    AUDIO_INITIALIZATION_FAILED = -6, // 0xFFFFFFFA
    HYPER_V_ENABLED = -5, // 0xFFFFFFFB
    INIT_FAILED = -4, // 0xFFFFFFFC
    UNHANDLED_EXCPETION = -3, // 0xFFFFFFFD
    CHECK_FILE_INTEGRITY_FAILED = -2, // 0xFFFFFFFE
    GENERIC_BOOT_ERROR = -1, // 0xFFFFFFFF
    SUCCESS = 0,
    INVALID_VMNAME = 1,
  }
}
