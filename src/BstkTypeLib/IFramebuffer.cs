// Decompiled with JetBrains decompiler
// Type: BstkTypeLib.IFramebuffer
// Assembly: BstkTypeLib, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 38E91E34-8BF8-4856-A23F-FE231831C5D8
// Assembly location: C:\Program Files\BlueStacks\BstkTypeLib.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BstkTypeLib
{
  [Guid("CA8187B2-2038-4AE1-B00C-8800214EF538")]
  [TypeLibType(20544)]
  [ComImport]
  public interface IFramebuffer
  {
    [DispId(1610743808)]
    uint Width { [DispId(1610743808), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743809)]
    uint Height { [DispId(1610743809), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743810)]
    uint BitsPerPixel { [DispId(1610743810), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743811)]
    uint BytesPerLine { [DispId(1610743811), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743812)]
    [ComAliasName("BstkTypeLib.BitmapFormat")]
    BitmapFormat PixelFormat { [DispId(1610743812), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: ComAliasName("BstkTypeLib.BitmapFormat")] get; }

    [DispId(1610743813)]
    uint HeightReduction { [DispId(1610743813), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743814)]
    IFramebufferOverlay Overlay { [DispId(1610743814), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.Interface)] get; }

    [DispId(1610743815)]
    long WinId { [DispId(1610743815), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743816)]
    FramebufferCapabilities[] Capabilities { [DispId(1610743816), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_I4)] get; }

    [DispId(1610743817)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void NotifyUpdate([In] uint aX, [In] uint aY, [In] uint aWidth, [In] uint aHeight);

    [DispId(1610743818)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void NotifyUpdateImage([In] uint aX, [In] uint aY, [In] uint aWidth, [In] uint aHeight, [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UI1), In] byte[] aImage);

    [DispId(1610743819)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void NotifyChange([In] uint aScreenId, [In] uint aXOrigin, [In] uint aYOrigin, [In] uint aWidth, [In] uint aHeight);

    [DispId(1610743820)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    int VideoModeSupported([In] uint aWidth, [In] uint aHeight, [In] uint aBpp);

    [DispId(1610743821)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    uint GetVisibleRegion([In] ref byte aRectangles, [In] uint aCount);

    [DispId(1610743822)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void SetVisibleRegion([In] ref byte aRectangles, [In] uint aCount);

    [DispId(1610743823)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void ProcessVHWACommand([In] ref byte aCommand);

    [DispId(1610743824)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void Notify3DEvent([In] uint aType, [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UI1), In] byte[] aData);
  }
}
