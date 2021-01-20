// Decompiled with JetBrains decompiler
// Type: BstkTypeLib.VirtualBoxClass
// Assembly: BstkTypeLib, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 38E91E34-8BF8-4856-A23F-FE231831C5D8
// Assembly location: C:\Program Files\BlueStacks\BstkTypeLib.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BstkTypeLib
{
  [ClassInterface(0)]
  [Guid("477297DB-D260-4198-8820-F97B966D38C9")]
  [TypeLibType(2)]
  [ComImport]
  public class VirtualBoxClass : IVirtualBox, VirtualBox
  {
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public extern VirtualBoxClass();

    [DispId(1610743808)]
    public virtual extern string Version { [DispId(1610743808), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; }

    [DispId(1610743809)]
    public virtual extern string VersionNormalized { [DispId(1610743809), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; }

    [DispId(1610743810)]
    public virtual extern uint Revision { [DispId(1610743810), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743811)]
    public virtual extern string PackageType { [DispId(1610743811), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; }

    [DispId(1610743812)]
    public virtual extern string APIVersion { [DispId(1610743812), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; }

    [DispId(1610743813)]
    public virtual extern long APIRevision { [DispId(1610743813), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743814)]
    public virtual extern string HomeFolder { [DispId(1610743814), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; }

    [DispId(1610743815)]
    public virtual extern string SettingsFilePath { [DispId(1610743815), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; }

    [DispId(1610743816)]
    public virtual extern IHost Host { [DispId(1610743816), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.Interface)] get; }

    [DispId(1610743817)]
    public virtual extern ISystemProperties SystemProperties { [DispId(1610743817), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.Interface)] get; }

    [DispId(1610743818)]
    public virtual extern IMachine[] Machines { [DispId(1610743818), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_DISPATCH)] get; }

    [DispId(1610743819)]
    public virtual extern string[] MachineGroups { [DispId(1610743819), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR)] get; }

    [DispId(1610743820)]
    public virtual extern IMedium[] HardDisks { [DispId(1610743820), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_DISPATCH)] get; }

    [DispId(1610743821)]
    public virtual extern IMedium[] DVDImages { [DispId(1610743821), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_DISPATCH)] get; }

    [DispId(1610743822)]
    public virtual extern IMedium[] FloppyImages { [DispId(1610743822), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_DISPATCH)] get; }

    [DispId(1610743823)]
    public virtual extern IProgress[] ProgressOperations { [DispId(1610743823), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_DISPATCH)] get; }

    [DispId(1610743824)]
    public virtual extern IGuestOSType[] GuestOSTypes { [DispId(1610743824), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_DISPATCH)] get; }

    [DispId(1610743825)]
    public virtual extern ISharedFolder[] SharedFolders { [DispId(1610743825), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_DISPATCH)] get; }

    [DispId(1610743826)]
    public virtual extern IPerformanceCollector PerformanceCollector { [DispId(1610743826), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.Interface)] get; }

    [DispId(1610743827)]
    public virtual extern IDHCPServer[] DHCPServers { [DispId(1610743827), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_DISPATCH)] get; }

    [DispId(1610743828)]
    public virtual extern INATNetwork[] NATNetworks { [DispId(1610743828), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_DISPATCH)] get; }

    [DispId(1610743829)]
    public virtual extern IEventSource EventSource { [DispId(1610743829), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.Interface)] get; }

    [DispId(1610743830)]
    public virtual extern IExtPackManager ExtensionPackManager { [DispId(1610743830), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.Interface)] get; }

    [DispId(1610743831)]
    public virtual extern string[] InternalNetworks { [DispId(1610743831), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR)] get; }

    [DispId(1610743832)]
    public virtual extern string[] GenericNetworkDrivers { [DispId(1610743832), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR)] get; }

    [DispId(1610743833)]
    public virtual extern uint InternalAndReservedAttribute1IVirtualBox { [DispId(1610743833), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743834)]
    public virtual extern uint InternalAndReservedAttribute2IVirtualBox { [DispId(1610743834), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743835)]
    public virtual extern uint InternalAndReservedAttribute3IVirtualBox { [DispId(1610743835), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743836)]
    public virtual extern uint InternalAndReservedAttribute4IVirtualBox { [DispId(1610743836), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743837)]
    public virtual extern uint InternalAndReservedAttribute5IVirtualBox { [DispId(1610743837), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743838)]
    public virtual extern uint InternalAndReservedAttribute6IVirtualBox { [DispId(1610743838), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743839)]
    public virtual extern uint InternalAndReservedAttribute7IVirtualBox { [DispId(1610743839), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743840)]
    public virtual extern uint InternalAndReservedAttribute8IVirtualBox { [DispId(1610743840), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743841)]
    public virtual extern uint InternalAndReservedAttribute9IVirtualBox { [DispId(1610743841), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743842)]
    public virtual extern uint InternalAndReservedAttribute10IVirtualBox { [DispId(1610743842), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743843)]
    public virtual extern uint InternalAndReservedAttribute11IVirtualBox { [DispId(1610743843), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743844)]
    public virtual extern uint InternalAndReservedAttribute12IVirtualBox { [DispId(1610743844), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743845)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.BStr)]
    public virtual extern string ComposeMachineFilename(
      [MarshalAs(UnmanagedType.BStr), In] string aName,
      [MarshalAs(UnmanagedType.BStr), In] string aGroup,
      [MarshalAs(UnmanagedType.BStr), In] string aCreateFlags,
      [MarshalAs(UnmanagedType.BStr), In] string aBaseFolder);

    [DispId(1610743846)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    public virtual extern IMachine CreateMachine(
      [MarshalAs(UnmanagedType.BStr), In] string aSettingsFile,
      [MarshalAs(UnmanagedType.BStr), In] string aName,
      [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR), In] string[] aGroups,
      [MarshalAs(UnmanagedType.BStr), In] string aOSTypeId,
      [MarshalAs(UnmanagedType.BStr), In] string aFlags);

    [DispId(1610743847)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    public virtual extern IMachine OpenMachine([MarshalAs(UnmanagedType.BStr), In] string aSettingsFile);

    [DispId(1610743848)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void RegisterMachine([MarshalAs(UnmanagedType.Interface), In] IMachine aMachine);

    [DispId(1610743849)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    public virtual extern IMachine FindMachine([MarshalAs(UnmanagedType.BStr), In] string aNameOrId);

    [DispId(1610743850)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_DISPATCH)]
    public virtual extern IMachine[] GetMachinesByGroups([MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR), In] string[] aGroups);

    [DispId(1610743851)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_I4)]
    public virtual extern MachineState[] GetMachineStates([MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_DISPATCH), In] IMachine[] aMachines);

    [DispId(1610743852)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    public virtual extern IAppliance CreateAppliance();

    [DispId(1610743853)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    public virtual extern IMedium CreateMedium(
      [MarshalAs(UnmanagedType.BStr), In] string aFormat,
      [MarshalAs(UnmanagedType.BStr), In] string aLocation,
      [ComAliasName("BstkTypeLib.AccessMode"), In] AccessMode aAccessMode,
      [ComAliasName("BstkTypeLib.DeviceType"), In] DeviceType aADeviceTypeType);

    [DispId(1610743854)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    public virtual extern IMedium OpenMedium(
      [MarshalAs(UnmanagedType.BStr), In] string aLocation,
      [ComAliasName("BstkTypeLib.DeviceType"), In] DeviceType aDeviceType,
      [ComAliasName("BstkTypeLib.AccessMode"), In] AccessMode aAccessMode,
      [In] int aForceNewUuid);

    [DispId(1610743855)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    public virtual extern IGuestOSType GetGuestOSType([MarshalAs(UnmanagedType.BStr), In] string aId);

    [DispId(1610743856)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void CreateSharedFolder(
      [MarshalAs(UnmanagedType.BStr), In] string aName,
      [MarshalAs(UnmanagedType.BStr), In] string aHostPath,
      [In] int aWritable,
      [In] int aAutoMount);

    [DispId(1610743857)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void RemoveSharedFolder([MarshalAs(UnmanagedType.BStr), In] string aName);

    [DispId(1610743858)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR)]
    public virtual extern string[] GetExtraDataKeys();

    [DispId(1610743859)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.BStr)]
    public virtual extern string GetExtraData([MarshalAs(UnmanagedType.BStr), In] string aKey);

    [DispId(1610743860)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void SetExtraData([MarshalAs(UnmanagedType.BStr), In] string aKey, [MarshalAs(UnmanagedType.BStr), In] string aValue);

    [DispId(1610743861)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void SetSettingsSecret([MarshalAs(UnmanagedType.BStr), In] string aPassword);

    [DispId(1610743862)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    public virtual extern IDHCPServer CreateDHCPServer([MarshalAs(UnmanagedType.BStr), In] string aName);

    [DispId(1610743863)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    public virtual extern IDHCPServer FindDHCPServerByNetworkName([MarshalAs(UnmanagedType.BStr), In] string aName);

    [DispId(1610743864)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void RemoveDHCPServer([MarshalAs(UnmanagedType.Interface), In] IDHCPServer aServer);

    [DispId(1610743865)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    public virtual extern INATNetwork CreateNATNetwork([MarshalAs(UnmanagedType.BStr), In] string aNetworkName);

    [DispId(1610743866)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    public virtual extern INATNetwork FindNATNetworkByName([MarshalAs(UnmanagedType.BStr), In] string aNetworkName);

    [DispId(1610743867)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void RemoveNATNetwork([MarshalAs(UnmanagedType.Interface), In] INATNetwork aNetwork);

    [DispId(1610743868)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern int CheckFirmwarePresent(
      [ComAliasName("BstkTypeLib.FirmwareType"), In] FirmwareType aFirmwareType,
      [MarshalAs(UnmanagedType.BStr), In] string aVersion,
      [MarshalAs(UnmanagedType.BStr)] out string aUrl,
      [MarshalAs(UnmanagedType.BStr)] out string aFile);

    [DispId(1610743869)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void InternalAndReservedMethod1IVirtualBox();

    [DispId(1610743870)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void InternalAndReservedMethod2IVirtualBox();

    [DispId(1610743871)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void InternalAndReservedMethod3IVirtualBox();

    [DispId(1610743872)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void InternalAndReservedMethod4IVirtualBox();

    [DispId(1610743873)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void InternalAndReservedMethod5IVirtualBox();

    [DispId(1610743874)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void InternalAndReservedMethod6IVirtualBox();

    [DispId(1610743875)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void InternalAndReservedMethod7IVirtualBox();

    [DispId(1610743876)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void InternalAndReservedMethod8IVirtualBox();
  }
}
