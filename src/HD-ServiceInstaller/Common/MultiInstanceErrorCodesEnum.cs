// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.MultiInstanceErrorCodesEnum
// Assembly: HD-ServiceInstaller, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 15F93427-26B3-4C7E-BAB1-0A00688BC4D4
// Assembly location: C:\Program Files\BlueStacks\HD-ServiceInstaller.exe

namespace BlueStacks.Common
{
  public enum MultiInstanceErrorCodesEnum
  {
    ErrorInResettingSharedFolders = -21, // 0xFFFFFFEB
    ErrorInRemovingDisk = -20, // 0xFFFFFFEC
    CannotCloneRunningVm = -19, // 0xFFFFFFED
    InvalidVmType = -18, // 0xFFFFFFEE
    ProcessAlreadyRunning = -17, // 0xFFFFFFEF
    FactoryResetUnHandledException = -16, // 0xFFFFFFF0
    DeviceCapsNotPresent = -15, // 0xFFFFFFF1
    WrongValue = -14, // 0xFFFFFFF2
    NotSupportedInLegacyMode = -13, // 0xFFFFFFF3
    VirtualBoxInitFailed = -12, // 0xFFFFFFF4
    NotSupportedInLegacyAndRawMode = -11, // 0xFFFFFFF5
    CannotDeleteDefaultVm = -10, // 0xFFFFFFF6
    VmNotRunning = -9, // 0xFFFFFFF7
    VmNotExist = -8, // 0xFFFFFFF8
    VmNameNotValid = -7, // 0xFFFFFFF9
    CommandNotFound = -6, // 0xFFFFFFFA
    UnknownException = -5, // 0xFFFFFFFB
    CreateServiceFailure = -4, // 0xFFFFFFFC
    RegistryCopyFailure = -3, // 0xFFFFFFFD
    CloneVmFailure = -2, // 0xFFFFFFFE
    ReachedMaxLimit = -1, // 0xFFFFFFFF
  }
}
