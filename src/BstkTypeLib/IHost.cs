// Decompiled with JetBrains decompiler
// Type: BstkTypeLib.IHost
// Assembly: BstkTypeLib, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 38E91E34-8BF8-4856-A23F-FE231831C5D8
// Assembly location: C:\Program Files\BlueStacks\BstkTypeLib.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BstkTypeLib
{
  [Guid("4C79A7E2-14DC-4BD9-94EF-C7E8DC32A85F")]
  [TypeLibType(20544)]
  [ComImport]
  public interface IHost
  {
    [DispId(1610743808)]
    IMedium[] DVDDrives { [DispId(1610743808), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_DISPATCH)] get; }

    [DispId(1610743809)]
    IMedium[] FloppyDrives { [DispId(1610743809), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_DISPATCH)] get; }

    [DispId(1610743810)]
    IHostUSBDevice[] USBDevices { [DispId(1610743810), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_DISPATCH)] get; }

    [DispId(1610743811)]
    IHostUSBDeviceFilter[] USBDeviceFilters { [DispId(1610743811), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_DISPATCH)] get; }

    [DispId(1610743812)]
    IHostNetworkInterface[] NetworkInterfaces { [DispId(1610743812), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_DISPATCH)] get; }

    [DispId(1610743813)]
    string[] NameServers { [DispId(1610743813), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR)] get; }

    [DispId(1610743814)]
    string DomainName { [DispId(1610743814), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; }

    [DispId(1610743815)]
    string[] SearchStrings { [DispId(1610743815), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR)] get; }

    [DispId(1610743816)]
    uint ProcessorCount { [DispId(1610743816), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743817)]
    uint ProcessorOnlineCount { [DispId(1610743817), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743818)]
    uint ProcessorCoreCount { [DispId(1610743818), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743819)]
    uint ProcessorOnlineCoreCount { [DispId(1610743819), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743820)]
    uint MemorySize { [DispId(1610743820), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743821)]
    uint MemoryAvailable { [DispId(1610743821), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743822)]
    string OperatingSystem { [DispId(1610743822), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; }

    [DispId(1610743823)]
    string OSVersion { [DispId(1610743823), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; }

    [DispId(1610743824)]
    long UTCTime { [DispId(1610743824), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743825)]
    int Acceleration3DAvailable { [DispId(1610743825), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743826)]
    IHostVideoInputDevice[] VideoInputDevices { [DispId(1610743826), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_DISPATCH)] get; }

    [DispId(1610743827)]
    uint InternalAndReservedAttribute1IHost { [DispId(1610743827), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743828)]
    uint InternalAndReservedAttribute2IHost { [DispId(1610743828), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743829)]
    uint InternalAndReservedAttribute3IHost { [DispId(1610743829), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743830)]
    uint InternalAndReservedAttribute4IHost { [DispId(1610743830), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743831)]
    uint InternalAndReservedAttribute5IHost { [DispId(1610743831), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743832)]
    uint InternalAndReservedAttribute6IHost { [DispId(1610743832), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743833)]
    uint InternalAndReservedAttribute7IHost { [DispId(1610743833), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743834)]
    uint InternalAndReservedAttribute8IHost { [DispId(1610743834), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743835)]
    uint InternalAndReservedAttribute9IHost { [DispId(1610743835), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743836)]
    uint InternalAndReservedAttribute10IHost { [DispId(1610743836), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743837)]
    uint InternalAndReservedAttribute11IHost { [DispId(1610743837), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743838)]
    uint InternalAndReservedAttribute12IHost { [DispId(1610743838), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743839)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    uint GetProcessorSpeed([In] uint aCpuId);

    [DispId(1610743840)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    int GetProcessorFeature([ComAliasName("BstkTypeLib.ProcessorFeature"), In] ProcessorFeature aFeature);

    [DispId(1610743841)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.BStr)]
    string GetProcessorDescription([In] uint aCpuId);

    [DispId(1610743842)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void GetProcessorCPUIDLeaf(
      [In] uint aCpuId,
      [In] uint aLeaf,
      [In] uint aSubLeaf,
      out uint aValEax,
      out uint aValEbx,
      out uint aValEcx,
      out uint aValEdx);

    [DispId(1610743843)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    IProgress CreateHostOnlyNetworkInterface([MarshalAs(UnmanagedType.Interface)] out IHostNetworkInterface aHostInterface);

    [DispId(1610743844)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    IProgress RemoveHostOnlyNetworkInterface([MarshalAs(UnmanagedType.BStr), In] string aId);

    [DispId(1610743845)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    IHostUSBDeviceFilter CreateUSBDeviceFilter([MarshalAs(UnmanagedType.BStr), In] string aName);

    [DispId(1610743846)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void InsertUSBDeviceFilter([In] uint aPosition, [MarshalAs(UnmanagedType.Interface), In] IHostUSBDeviceFilter aFilter);

    [DispId(1610743847)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void RemoveUSBDeviceFilter([In] uint aPosition);

    [DispId(1610743848)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    IMedium FindHostDVDDrive([MarshalAs(UnmanagedType.BStr), In] string aName);

    [DispId(1610743849)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    IMedium FindHostFloppyDrive([MarshalAs(UnmanagedType.BStr), In] string aName);

    [DispId(1610743850)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    IHostNetworkInterface FindHostNetworkInterfaceByName([MarshalAs(UnmanagedType.BStr), In] string aName);

    [DispId(1610743851)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    IHostNetworkInterface FindHostNetworkInterfaceById([MarshalAs(UnmanagedType.BStr), In] string aId);

    [DispId(1610743852)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_DISPATCH)]
    IHostNetworkInterface[] FindHostNetworkInterfacesOfType(
      [ComAliasName("BstkTypeLib.HostNetworkInterfaceType"), In] HostNetworkInterfaceType aType);

    [DispId(1610743853)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    IHostUSBDevice FindUSBDeviceById([MarshalAs(UnmanagedType.BStr), In] string aId);

    [DispId(1610743854)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    IHostUSBDevice FindUSBDeviceByAddress([MarshalAs(UnmanagedType.BStr), In] string aName);

    [DispId(1610743855)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.BStr)]
    string GenerateMACAddress();

    [DispId(1610743856)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void AddUSBDeviceSource(
      [MarshalAs(UnmanagedType.BStr), In] string aBackend,
      [MarshalAs(UnmanagedType.BStr), In] string aId,
      [MarshalAs(UnmanagedType.BStr), In] string aAddress,
      [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR), In] string[] aPropertyNames,
      [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR), In] string[] aPropertyValues);

    [DispId(1610743857)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void RemoveUSBDeviceSource([MarshalAs(UnmanagedType.BStr), In] string aId);
  }
}
