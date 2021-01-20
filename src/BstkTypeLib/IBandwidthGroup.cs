// Decompiled with JetBrains decompiler
// Type: BstkTypeLib.IBandwidthGroup
// Assembly: BstkTypeLib, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 38E91E34-8BF8-4856-A23F-FE231831C5D8
// Assembly location: C:\Program Files\BlueStacks\BstkTypeLib.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BstkTypeLib
{
  [TypeLibType(20544)]
  [Guid("DDC7BA22-6CDC-44D8-A63B-FB4B2DFC3434")]
  [ComImport]
  public interface IBandwidthGroup
  {
    [DispId(1610743808)]
    string Name { [DispId(1610743808), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; }

    [ComAliasName("BstkTypeLib.BandwidthGroupType")]
    [DispId(1610743809)]
    BandwidthGroupType Type { [DispId(1610743809), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: ComAliasName("BstkTypeLib.BandwidthGroupType")] get; }

    [DispId(1610743810)]
    uint Reference { [DispId(1610743810), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743811)]
    long MaxBytesPerSec { [DispId(1610743811), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; [DispId(1610743811), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: In] set; }

    [DispId(1610743813)]
    uint InternalAndReservedAttribute1IBandwidthGroup { [DispId(1610743813), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743814)]
    uint InternalAndReservedAttribute2IBandwidthGroup { [DispId(1610743814), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743815)]
    uint InternalAndReservedAttribute3IBandwidthGroup { [DispId(1610743815), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743816)]
    uint InternalAndReservedAttribute4IBandwidthGroup { [DispId(1610743816), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }
  }
}
