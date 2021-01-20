// Decompiled with JetBrains decompiler
// Type: BstkTypeLib.IUSBDeviceFilters
// Assembly: BstkTypeLib, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 38E91E34-8BF8-4856-A23F-FE231831C5D8
// Assembly location: C:\Program Files\BlueStacks\BstkTypeLib.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BstkTypeLib
{
  [Guid("FB5276D9-F893-457A-AAAB-974DC060FDFD")]
  [TypeLibType(20544)]
  [ComImport]
  public interface IUSBDeviceFilters
  {
    [DispId(1610743808)]
    IUSBDeviceFilter[] DeviceFilters { [DispId(1610743808), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_DISPATCH)] get; }

    [DispId(1610743809)]
    uint InternalAndReservedAttribute1IUSBDeviceFilters { [DispId(1610743809), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743810)]
    uint InternalAndReservedAttribute2IUSBDeviceFilters { [DispId(1610743810), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743811)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    IUSBDeviceFilter CreateDeviceFilter([MarshalAs(UnmanagedType.BStr), In] string aName);

    [DispId(1610743812)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void InsertDeviceFilter([In] uint aPosition, [MarshalAs(UnmanagedType.Interface), In] IUSBDeviceFilter aFilter);

    [DispId(1610743813)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    IUSBDeviceFilter RemoveDeviceFilter([In] uint aPosition);

    [DispId(1610743814)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void InternalAndReservedMethod1IUSBDeviceFilters();

    [DispId(1610743815)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void InternalAndReservedMethod2IUSBDeviceFilters();
  }
}
