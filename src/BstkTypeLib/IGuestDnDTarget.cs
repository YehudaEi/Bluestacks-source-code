// Decompiled with JetBrains decompiler
// Type: BstkTypeLib.IGuestDnDTarget
// Assembly: BstkTypeLib, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 38E91E34-8BF8-4856-A23F-FE231831C5D8
// Assembly location: C:\Program Files\BlueStacks\BstkTypeLib.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BstkTypeLib
{
  [TypeLibType(20544)]
  [Guid("B96C6CFB-F6A0-473D-B071-5FDA603DD5AE")]
  [ComImport]
  public interface IGuestDnDTarget : IDnDTarget
  {
    [DispId(1610743808)]
    new string[] Formats { [DispId(1610743808), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR)] get; }

    [DispId(1610743809)]
    new uint ProtocolVersion { [DispId(1610743809), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743810)]
    new uint InternalAndReservedAttribute1IDnDBase { [DispId(1610743810), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743811)]
    new uint InternalAndReservedAttribute2IDnDBase { [DispId(1610743811), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743812)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    new int IsFormatSupported([MarshalAs(UnmanagedType.BStr), In] string aFormat);

    [DispId(1610743813)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    new void AddFormats([MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR), In] string[] aFormats);

    [DispId(1610743814)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    new void RemoveFormats([MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR), In] string[] aFormats);

    [DispId(1610743815)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    new void InternalAndReservedMethod1IDnDBase();

    [DispId(1610809344)]
    new uint InternalAndReservedAttribute1IDnDTarget { [DispId(1610809344), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610809345)]
    new uint InternalAndReservedAttribute2IDnDTarget { [DispId(1610809345), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610809346)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: ComAliasName("BstkTypeLib.DnDAction")]
    new DnDAction Enter(
      [In] uint aScreenId,
      [In] uint aY,
      [In] uint aX,
      [ComAliasName("BstkTypeLib.DnDAction"), In] DnDAction aDefaultAction,
      [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_I4), In] DnDAction[] aAllowedActions,
      [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR), In] string[] aFormats);

    [DispId(1610809347)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: ComAliasName("BstkTypeLib.DnDAction")]
    new DnDAction Move(
      [In] uint aScreenId,
      [In] uint aX,
      [In] uint aY,
      [ComAliasName("BstkTypeLib.DnDAction"), In] DnDAction aDefaultAction,
      [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_I4), In] DnDAction[] aAllowedActions,
      [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR), In] string[] aFormats);

    [DispId(1610809348)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    new void Leave([In] uint aScreenId);

    [DispId(1610809349)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: ComAliasName("BstkTypeLib.DnDAction")]
    new DnDAction Drop(
      [In] uint aScreenId,
      [In] uint aX,
      [In] uint aY,
      [ComAliasName("BstkTypeLib.DnDAction"), In] DnDAction aDefaultAction,
      [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_I4), In] DnDAction[] aAllowedActions,
      [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR), In] string[] aFormats,
      [MarshalAs(UnmanagedType.BStr)] out string aFormat);

    [DispId(1610809350)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    new IProgress SendData([In] uint aScreenId, [MarshalAs(UnmanagedType.BStr), In] string aFormat, [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UI1), In] byte[] aData);

    [DispId(1610809351)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    new int Cancel();

    [DispId(1610809352)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    new void InternalAndReservedMethod1IDnDTarget();

    [DispId(1610874880)]
    int MidlDoesNotLikeEmptyInterfaces { [DispId(1610874880), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }
  }
}
