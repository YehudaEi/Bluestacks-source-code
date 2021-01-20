// Decompiled with JetBrains decompiler
// Type: BstkTypeLib.ISystemProperties
// Assembly: BstkTypeLib, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 38E91E34-8BF8-4856-A23F-FE231831C5D8
// Assembly location: C:\Program Files\BlueStacks\BstkTypeLib.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BstkTypeLib
{
  [Guid("C1E71681-8CED-40F1-9E50-AEA75E9D321E")]
  [TypeLibType(20544)]
  [ComImport]
  public interface ISystemProperties
  {
    [DispId(1610743808)]
    uint MinGuestRAM { [DispId(1610743808), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743809)]
    uint MaxGuestRAM { [DispId(1610743809), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743810)]
    uint MinGuestVRAM { [DispId(1610743810), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743811)]
    uint MaxGuestVRAM { [DispId(1610743811), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743812)]
    uint MinGuestCPUCount { [DispId(1610743812), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743813)]
    uint MaxGuestCPUCount { [DispId(1610743813), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743814)]
    uint MaxGuestMonitors { [DispId(1610743814), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743815)]
    long InfoVDSize { [DispId(1610743815), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743816)]
    uint SerialPortCount { [DispId(1610743816), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743817)]
    uint ParallelPortCount { [DispId(1610743817), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743818)]
    uint MaxBootPosition { [DispId(1610743818), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743819)]
    int RawModeSupported { [DispId(1610743819), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743820)]
    int ExclusiveHwVirt { [DispId(1610743820), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(1610743820), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(1610743822)]
    string DefaultMachineFolder { [DispId(1610743822), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; [DispId(1610743822), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: MarshalAs(UnmanagedType.BStr), In] set; }

    [DispId(1610743824)]
    string LoggingLevel { [DispId(1610743824), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; [DispId(1610743824), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: MarshalAs(UnmanagedType.BStr), In] set; }

    [DispId(1610743826)]
    IMediumFormat[] MediumFormats { [DispId(1610743826), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_DISPATCH)] get; }

    [DispId(1610743827)]
    string DefaultHardDiskFormat { [DispId(1610743827), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; [DispId(1610743827), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: MarshalAs(UnmanagedType.BStr), In] set; }

    [DispId(1610743829)]
    long FreeDiskSpaceWarning { [DispId(1610743829), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(1610743829), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(1610743831)]
    uint FreeDiskSpacePercentWarning { [DispId(1610743831), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(1610743831), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(1610743833)]
    long FreeDiskSpaceError { [DispId(1610743833), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(1610743833), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(1610743835)]
    uint FreeDiskSpacePercentError { [DispId(1610743835), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(1610743835), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(1610743837)]
    string VRDEAuthLibrary { [DispId(1610743837), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; [DispId(1610743837), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: MarshalAs(UnmanagedType.BStr), In] set; }

    [DispId(1610743839)]
    string WebServiceAuthLibrary { [DispId(1610743839), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; [DispId(1610743839), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: MarshalAs(UnmanagedType.BStr), In] set; }

    [DispId(1610743841)]
    string DefaultVRDEExtPack { [DispId(1610743841), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; [DispId(1610743841), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: MarshalAs(UnmanagedType.BStr), In] set; }

    [DispId(1610743843)]
    uint LogHistoryCount { [DispId(1610743843), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(1610743843), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [ComAliasName("BstkTypeLib.AudioDriverType")]
    [DispId(1610743845)]
    AudioDriverType DefaultAudioDriver { [DispId(1610743845), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: ComAliasName("BstkTypeLib.AudioDriverType")] get; }

    [DispId(1610743846)]
    string AutostartDatabasePath { [DispId(1610743846), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; [DispId(1610743846), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: MarshalAs(UnmanagedType.BStr), In] set; }

    [DispId(1610743848)]
    string DefaultAdditionsISO { [DispId(1610743848), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; [DispId(1610743848), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: MarshalAs(UnmanagedType.BStr), In] set; }

    [DispId(1610743850)]
    string DefaultFrontend { [DispId(1610743850), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; [DispId(1610743850), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: MarshalAs(UnmanagedType.BStr), In] set; }

    [DispId(1610743852)]
    BitmapFormat[] ScreenShotFormats { [DispId(1610743852), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_I4)] get; }

    [DispId(1610743853)]
    uint InternalAndReservedAttribute1ISystemProperties { [DispId(1610743853), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743854)]
    uint InternalAndReservedAttribute2ISystemProperties { [DispId(1610743854), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743855)]
    uint InternalAndReservedAttribute3ISystemProperties { [DispId(1610743855), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743856)]
    uint InternalAndReservedAttribute4ISystemProperties { [DispId(1610743856), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743857)]
    uint InternalAndReservedAttribute5ISystemProperties { [DispId(1610743857), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743858)]
    uint InternalAndReservedAttribute6ISystemProperties { [DispId(1610743858), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743859)]
    uint InternalAndReservedAttribute7ISystemProperties { [DispId(1610743859), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743860)]
    uint InternalAndReservedAttribute8ISystemProperties { [DispId(1610743860), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743861)]
    uint InternalAndReservedAttribute9ISystemProperties { [DispId(1610743861), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743862)]
    uint InternalAndReservedAttribute10ISystemProperties { [DispId(1610743862), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743863)]
    uint InternalAndReservedAttribute11ISystemProperties { [DispId(1610743863), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743864)]
    uint InternalAndReservedAttribute12ISystemProperties { [DispId(1610743864), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743865)]
    uint InternalAndReservedAttribute13ISystemProperties { [DispId(1610743865), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743866)]
    uint InternalAndReservedAttribute14ISystemProperties { [DispId(1610743866), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743867)]
    uint InternalAndReservedAttribute15ISystemProperties { [DispId(1610743867), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743868)]
    uint InternalAndReservedAttribute16ISystemProperties { [DispId(1610743868), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743869)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    uint GetMaxNetworkAdapters([ComAliasName("BstkTypeLib.ChipsetType"), In] ChipsetType aChipset);

    [DispId(1610743870)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    uint GetMaxNetworkAdaptersOfType([ComAliasName("BstkTypeLib.ChipsetType"), In] ChipsetType aChipset, [ComAliasName("BstkTypeLib.NetworkAttachmentType"), In] NetworkAttachmentType aType);

    [DispId(1610743871)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    uint GetMaxDevicesPerPortForStorageBus([ComAliasName("BstkTypeLib.StorageBus"), In] StorageBus aBus);

    [DispId(1610743872)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    uint GetMinPortCountForStorageBus([ComAliasName("BstkTypeLib.StorageBus"), In] StorageBus aBus);

    [DispId(1610743873)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    uint GetMaxPortCountForStorageBus([ComAliasName("BstkTypeLib.StorageBus"), In] StorageBus aBus);

    [DispId(1610743874)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    uint GetMaxInstancesOfStorageBus([ComAliasName("BstkTypeLib.ChipsetType"), In] ChipsetType aChipset, [ComAliasName("BstkTypeLib.StorageBus"), In] StorageBus aBus);

    [DispId(1610743875)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_I4)]
    DeviceType[] GetDeviceTypesForStorageBus([ComAliasName("BstkTypeLib.StorageBus"), In] StorageBus aBus);

    [DispId(1610743876)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    int GetDefaultIoCacheSettingForStorageController([ComAliasName("BstkTypeLib.StorageControllerType"), In] StorageControllerType aControllerType);

    [DispId(1610743877)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    int GetStorageControllerHotplugCapable([ComAliasName("BstkTypeLib.StorageControllerType"), In] StorageControllerType aControllerType);

    [DispId(1610743878)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    uint GetMaxInstancesOfUSBControllerType([ComAliasName("BstkTypeLib.ChipsetType"), In] ChipsetType aChipset, [ComAliasName("BstkTypeLib.USBControllerType"), In] USBControllerType aType);

    [DispId(1610743879)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void InternalAndReservedMethod1ISystemProperties();

    [DispId(1610743880)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void InternalAndReservedMethod2ISystemProperties();

    [DispId(1610743881)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void InternalAndReservedMethod3ISystemProperties();

    [DispId(1610743882)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void InternalAndReservedMethod4ISystemProperties();
  }
}
