// Decompiled with JetBrains decompiler
// Type: BstkTypeLib.IDisplay
// Assembly: BstkTypeLib, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 38E91E34-8BF8-4856-A23F-FE231831C5D8
// Assembly location: C:\Program Files\BlueStacks\BstkTypeLib.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BstkTypeLib
{
  [Guid("73250BA8-36BE-4612-B7D9-FBE329A1421A")]
  [TypeLibType(20544)]
  [ComImport]
  public interface IDisplay
  {
    [DispId(1610743808)]
    IGuestScreenInfo[] GuestScreenLayout { [DispId(1610743808), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_DISPATCH)] get; }

    [DispId(1610743809)]
    uint InternalAndReservedAttribute1IDisplay { [DispId(1610743809), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743810)]
    uint InternalAndReservedAttribute2IDisplay { [DispId(1610743810), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743811)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void GetScreenResolution(
      [In] uint aScreenId,
      out uint aWidth,
      out uint aHeight,
      out uint aBitsPerPixel,
      out int aXOrigin,
      out int aYOrigin,
      [ComAliasName("BstkTypeLib.GuestMonitorStatus")] out GuestMonitorStatus aGuestMonitorStatus);

    [DispId(1610743812)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.BStr)]
    string AttachFramebuffer([In] uint aScreenId, [MarshalAs(UnmanagedType.Interface), In] IFramebuffer aFramebuffer);

    [DispId(1610743813)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void DetachFramebuffer([In] uint aScreenId, [MarshalAs(UnmanagedType.BStr), In] string aId);

    [DispId(1610743814)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    IFramebuffer QueryFramebuffer([In] uint aScreenId);

    [DispId(1610743815)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void SetVideoModeHint(
      [In] uint aDisplay,
      [In] int aEnabled,
      [In] int aChangeOrigin,
      [In] int aOriginX,
      [In] int aOriginY,
      [In] uint aWidth,
      [In] uint aHeight,
      [In] uint aBitsPerPixel);

    [DispId(1610743816)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void SetSeamlessMode([In] int aEnabled);

    [DispId(1610743817)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void TakeScreenShot(
      [In] uint aScreenId,
      [In] ref byte aAddress,
      [In] uint aWidth,
      [In] uint aHeight,
      [ComAliasName("BstkTypeLib.BitmapFormat"), In] BitmapFormat aBitmapFormat);

    [DispId(1610743818)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UI1)]
    byte[] TakeScreenShotToArray(
      [In] uint aScreenId,
      [In] uint aWidth,
      [In] uint aHeight,
      [ComAliasName("BstkTypeLib.BitmapFormat"), In] BitmapFormat aBitmapFormat);

    [DispId(1610743819)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void DrawToScreen(
      [In] uint aScreenId,
      [In] ref byte aAddress,
      [In] uint aX,
      [In] uint aY,
      [In] uint aWidth,
      [In] uint aHeight);

    [DispId(1610743820)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void InvalidateAndUpdate();

    [DispId(1610743821)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void InvalidateAndUpdateScreen([In] uint aScreenId);

    [DispId(1610743822)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void CompleteVHWACommand([In] ref byte aCommand);

    [DispId(1610743823)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void ViewportChanged([In] uint aScreenId, [In] uint aX, [In] uint aY, [In] uint aWidth, [In] uint aHeight);

    [DispId(1610743824)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void QuerySourceBitmap([In] uint aScreenId, [MarshalAs(UnmanagedType.Interface)] out IDisplaySourceBitmap aDisplaySourceBitmap);

    [DispId(1610743825)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void NotifyScaleFactorChange(
      [In] uint aScreenId,
      [In] uint aU32ScaleFactorWMultiplied,
      [In] uint aU32ScaleFactorHMultiplied);

    [DispId(1610743826)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void NotifyHiDPIOutputPolicyChange([In] int aFUnscaledHiDPI);

    [DispId(1610743827)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void SetScreenLayout([ComAliasName("BstkTypeLib.ScreenLayoutMode"), In] ScreenLayoutMode aScreenLayoutMode, [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_DISPATCH), In] IGuestScreenInfo[] aGuestScreenInfo);

    [DispId(1610743828)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void InternalAndReservedMethod1IDisplay();

    [DispId(1610743829)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void InternalAndReservedMethod2IDisplay();

    [DispId(1610743830)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void InternalAndReservedMethod3IDisplay();

    [DispId(1610743831)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void InternalAndReservedMethod4IDisplay();
  }
}
