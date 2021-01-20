// Decompiled with JetBrains decompiler
// Type: BstkTypeLib.IAdditionsFacility
// Assembly: BstkTypeLib, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 38E91E34-8BF8-4856-A23F-FE231831C5D8
// Assembly location: C:\Program Files\BlueStacks\BstkTypeLib.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BstkTypeLib
{
  [Guid("BC28A9E7-D12F-4C3F-9960-E41F4D8CA986")]
  [TypeLibType(20544)]
  [ComImport]
  public interface IAdditionsFacility
  {
    [ComAliasName("BstkTypeLib.AdditionsFacilityClass")]
    [DispId(1610743808)]
    AdditionsFacilityClass ClassType { [DispId(1610743808), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: ComAliasName("BstkTypeLib.AdditionsFacilityClass")] get; }

    [DispId(1610743809)]
    long LastUpdated { [DispId(1610743809), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743810)]
    string Name { [DispId(1610743810), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; }

    [ComAliasName("BstkTypeLib.AdditionsFacilityStatus")]
    [DispId(1610743811)]
    AdditionsFacilityStatus Status { [DispId(1610743811), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: ComAliasName("BstkTypeLib.AdditionsFacilityStatus")] get; }

    [DispId(1610743812)]
    [ComAliasName("BstkTypeLib.AdditionsFacilityType")]
    AdditionsFacilityType Type { [DispId(1610743812), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: ComAliasName("BstkTypeLib.AdditionsFacilityType")] get; }

    [DispId(1610743813)]
    uint InternalAndReservedAttribute1IAdditionsFacility { [DispId(1610743813), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743814)]
    uint InternalAndReservedAttribute2IAdditionsFacility { [DispId(1610743814), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }
  }
}
