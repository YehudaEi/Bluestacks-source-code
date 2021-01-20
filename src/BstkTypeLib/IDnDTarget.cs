// Decompiled with JetBrains decompiler
// Type: BstkTypeLib.IDnDTarget
// Assembly: BstkTypeLib, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 38E91E34-8BF8-4856-A23F-FE231831C5D8
// Assembly location: C:\Program Files\BlueStacks\BstkTypeLib.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BstkTypeLib
{
  [Guid("09D1B1E8-F476-49E2-B7B0-6BAED7A86CF5")]
  [TypeLibType(20544)]
  [ComImport]
  public interface IDnDTarget : IDnDBase
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
    uint InternalAndReservedAttribute1IDnDTarget { [DispId(1610809344), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610809345)]
    uint InternalAndReservedAttribute2IDnDTarget { [DispId(1610809345), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610809346)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: ComAliasName("BstkTypeLib.DnDAction")]
    DnDAction Enter(
      [In] uint aScreenId,
      [In] uint aY,
      [In] uint aX,
      [ComAliasName("BstkTypeLib.DnDAction"), In] DnDAction aDefaultAction,
      [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_I4), In] DnDAction[] aAllowedActions,
      [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR), In] string[] aFormats);

    [DispId(1610809347)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: ComAliasName("BstkTypeLib.DnDAction")]
    DnDAction Move(
      [In] uint aScreenId,
      [In] uint aX,
      [In] uint aY,
      [ComAliasName("BstkTypeLib.DnDAction"), In] DnDAction aDefaultAction,
      [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_I4), In] DnDAction[] aAllowedActions,
      [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR), In] string[] aFormats);

    [DispId(1610809348)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void Leave([In] uint aScreenId);

    [DispId(1610809349)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: ComAliasName("BstkTypeLib.DnDAction")]
    DnDAction Drop(
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
    IProgress SendData([In] uint aScreenId, [MarshalAs(UnmanagedType.BStr), In] string aFormat, [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UI1), In] byte[] aData);

    [DispId(1610809351)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    int Cancel();

    [DispId(1610809352)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void InternalAndReservedMethod1IDnDTarget();
  }
}
