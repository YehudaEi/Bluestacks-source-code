// Decompiled with JetBrains decompiler
// Type: BstkTypeLib.__MIDL___MIDL_itf_VirtualBox_0000_0000_0105
// Assembly: BstkTypeLib, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 38E91E34-8BF8-4856-A23F-FE231831C5D8
// Assembly location: C:\Program Files\BlueStacks\BstkTypeLib.dll

namespace BstkTypeLib
{
  public enum __MIDL___MIDL_itf_VirtualBox_0000_0000_0105
  {
    VBoxEventType_Invalid = 0,
    VBoxEventType_Any = 1,
    VBoxEventType_Vetoable = 2,
    VBoxEventType_MachineEvent = 3,
    VBoxEventType_SnapshotEvent = 4,
    VBoxEventType_InputEvent = 5,
    VBoxEventType_LastWildcard = 31, // 0x0000001F
    VBoxEventType_OnMachineStateChanged = 32, // 0x00000020
    VBoxEventType_OnMachineDataChanged = 33, // 0x00000021
    VBoxEventType_OnExtraDataChanged = 34, // 0x00000022
    VBoxEventType_OnExtraDataCanChange = 35, // 0x00000023
    VBoxEventType_OnMediumRegistered = 36, // 0x00000024
    VBoxEventType_OnMachineRegistered = 37, // 0x00000025
    VBoxEventType_OnSessionStateChanged = 38, // 0x00000026
    VBoxEventType_OnSnapshotTaken = 39, // 0x00000027
    VBoxEventType_OnSnapshotDeleted = 40, // 0x00000028
    VBoxEventType_OnSnapshotChanged = 41, // 0x00000029
    VBoxEventType_OnGuestPropertyChanged = 42, // 0x0000002A
    VBoxEventType_OnMousePointerShapeChanged = 43, // 0x0000002B
    VBoxEventType_OnMouseCapabilityChanged = 44, // 0x0000002C
    VBoxEventType_OnKeyboardLedsChanged = 45, // 0x0000002D
    VBoxEventType_OnStateChanged = 46, // 0x0000002E
    VBoxEventType_OnAdditionsStateChanged = 47, // 0x0000002F
    VBoxEventType_OnNetworkAdapterChanged = 48, // 0x00000030
    VBoxEventType_OnSerialPortChanged = 49, // 0x00000031
    VBoxEventType_OnParallelPortChanged = 50, // 0x00000032
    VBoxEventType_OnStorageControllerChanged = 51, // 0x00000033
    VBoxEventType_OnMediumChanged = 52, // 0x00000034
    VBoxEventType_OnVRDEServerChanged = 53, // 0x00000035
    VBoxEventType_OnUSBControllerChanged = 54, // 0x00000036
    VBoxEventType_OnUSBDeviceStateChanged = 55, // 0x00000037
    VBoxEventType_OnSharedFolderChanged = 56, // 0x00000038
    VBoxEventType_OnRuntimeError = 57, // 0x00000039
    VBoxEventType_OnCanShowWindow = 58, // 0x0000003A
    VBoxEventType_OnShowWindow = 59, // 0x0000003B
    VBoxEventType_OnCPUChanged = 60, // 0x0000003C
    VBoxEventType_OnVRDEServerInfoChanged = 61, // 0x0000003D
    VBoxEventType_OnEventSourceChanged = 62, // 0x0000003E
    VBoxEventType_OnCPUExecutionCapChanged = 63, // 0x0000003F
    VBoxEventType_OnGuestKeyboard = 64, // 0x00000040
    VBoxEventType_OnGuestMouse = 65, // 0x00000041
    VBoxEventType_OnNATRedirect = 66, // 0x00000042
    VBoxEventType_OnHostPCIDevicePlug = 67, // 0x00000043
    VBoxEventType_OnVBoxSVCAvailabilityChanged = 68, // 0x00000044
    VBoxEventType_OnBandwidthGroupChanged = 69, // 0x00000045
    VBoxEventType_OnGuestMonitorChanged = 70, // 0x00000046
    VBoxEventType_OnStorageDeviceChanged = 71, // 0x00000047
    VBoxEventType_OnClipboardModeChanged = 72, // 0x00000048
    VBoxEventType_OnDnDModeChanged = 73, // 0x00000049
    VBoxEventType_OnNATNetworkChanged = 74, // 0x0000004A
    VBoxEventType_OnNATNetworkStartStop = 75, // 0x0000004B
    VBoxEventType_OnNATNetworkAlter = 76, // 0x0000004C
    VBoxEventType_OnNATNetworkCreationDeletion = 77, // 0x0000004D
    VBoxEventType_OnNATNetworkSetting = 78, // 0x0000004E
    VBoxEventType_OnNATNetworkPortForward = 79, // 0x0000004F
    VBoxEventType_OnGuestSessionStateChanged = 80, // 0x00000050
    VBoxEventType_OnGuestSessionRegistered = 81, // 0x00000051
    VBoxEventType_OnGuestProcessRegistered = 82, // 0x00000052
    VBoxEventType_OnGuestProcessStateChanged = 83, // 0x00000053
    VBoxEventType_OnGuestProcessInputNotify = 84, // 0x00000054
    VBoxEventType_OnGuestProcessOutput = 85, // 0x00000055
    VBoxEventType_OnGuestFileRegistered = 86, // 0x00000056
    VBoxEventType_OnGuestFileStateChanged = 87, // 0x00000057
    VBoxEventType_OnGuestFileOffsetChanged = 88, // 0x00000058
    VBoxEventType_OnGuestFileRead = 89, // 0x00000059
    VBoxEventType_OnGuestFileWrite = 90, // 0x0000005A
    VBoxEventType_OnVideoCaptureChanged = 91, // 0x0000005B
    VBoxEventType_OnGuestUserStateChanged = 92, // 0x0000005C
    VBoxEventType_OnGuestMultiTouch = 93, // 0x0000005D
    VBoxEventType_OnHostNameResolutionConfigurationChange = 94, // 0x0000005E
    VBoxEventType_OnSnapshotRestored = 95, // 0x0000005F
    VBoxEventType_OnMediumConfigChanged = 96, // 0x00000060
    VBoxEventType_Last = 97, // 0x00000061
  }
}
