// Decompiled with JetBrains decompiler
// Type: BstkTypeLib.IUSBController
// Assembly: BstkTypeLib, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 38E91E34-8BF8-4856-A23F-FE231831C5D8
// Assembly location: C:\Program Files\BlueStacks\BstkTypeLib.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BstkTypeLib
{
  [Guid("DDF5BE6B-6B7F-440F-A570-9969991BF5E1")]
  [TypeLibType(20544)]
  [ComImport]
  public interface IUSBController
  {
    [DispId(1610743808)]
    string Name { [DispId(1610743808), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; [DispId(1610743808), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: MarshalAs(UnmanagedType.BStr), In] set; }

    [ComAliasName("BstkTypeLib.USBControllerType")]
    [DispId(1610743810)]
    USBControllerType Type { [DispId(1610743810), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: ComAliasName("BstkTypeLib.USBControllerType")] get; [DispId(1610743810), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [param: ComAliasName("BstkTypeLib.USBControllerType"), In] set; }

    [DispId(1610743812)]
    ushort USBStandard { [DispId(1610743812), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743813)]
    uint InternalAndReservedAttribute1IUSBController { [DispId(1610743813), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743814)]
    uint InternalAndReservedAttribute2IUSBController { [DispId(1610743814), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }
  }
}
