// Decompiled with JetBrains decompiler
// Type: BstkTypeLib.IBandwidthControl
// Assembly: BstkTypeLib, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 38E91E34-8BF8-4856-A23F-FE231831C5D8
// Assembly location: C:\Program Files\BlueStacks\BstkTypeLib.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BstkTypeLib
{
  [TypeLibType(20544)]
  [Guid("9AF9FAE7-CDB9-4DA2-ADE3-37E8F5F663EC")]
  [ComImport]
  public interface IBandwidthControl
  {
    [DispId(1610743808)]
    uint NumGroups { [DispId(1610743808), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743809)]
    uint InternalAndReservedAttribute1IBandwidthControl { [DispId(1610743809), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743810)]
    uint InternalAndReservedAttribute2IBandwidthControl { [DispId(1610743810), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743811)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void CreateBandwidthGroup([MarshalAs(UnmanagedType.BStr), In] string aName, [ComAliasName("BstkTypeLib.BandwidthGroupType"), In] BandwidthGroupType aType, [In] long aMaxBytesPerSec);

    [DispId(1610743812)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void DeleteBandwidthGroup([MarshalAs(UnmanagedType.BStr), In] string aName);

    [DispId(1610743813)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    IBandwidthGroup GetBandwidthGroup([MarshalAs(UnmanagedType.BStr), In] string aName);

    [DispId(1610743814)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_DISPATCH)]
    IBandwidthGroup[] GetAllBandwidthGroups();

    [DispId(1610743815)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void InternalAndReservedMethod1IBandwidthControl();

    [DispId(1610743816)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void InternalAndReservedMethod2IBandwidthControl();
  }
}
