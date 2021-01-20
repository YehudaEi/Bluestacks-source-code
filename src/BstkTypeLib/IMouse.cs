// Decompiled with JetBrains decompiler
// Type: BstkTypeLib.IMouse
// Assembly: BstkTypeLib, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 38E91E34-8BF8-4856-A23F-FE231831C5D8
// Assembly location: C:\Program Files\BlueStacks\BstkTypeLib.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BstkTypeLib
{
  [TypeLibType(20544)]
  [Guid("3C602D33-8CEA-43AF-9544-F4B0075664A8")]
  [ComImport]
  public interface IMouse
  {
    [DispId(1610743808)]
    int AbsoluteSupported { [DispId(1610743808), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743809)]
    int RelativeSupported { [DispId(1610743809), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743810)]
    int MultiTouchSupported { [DispId(1610743810), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743811)]
    int NeedsHostCursor { [DispId(1610743811), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743812)]
    IMousePointerShape PointerShape { [DispId(1610743812), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.Interface)] get; }

    [DispId(1610743813)]
    IEventSource EventSource { [DispId(1610743813), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.Interface)] get; }

    [DispId(1610743814)]
    uint InternalAndReservedAttribute1IMouse { [DispId(1610743814), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743815)]
    uint InternalAndReservedAttribute2IMouse { [DispId(1610743815), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743816)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void PutMouseEvent([In] int aDx, [In] int aDy, [In] int aDz, [In] int aDw, [In] int aButtonState);

    [DispId(1610743817)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void PutMouseEventAbsolute([In] int aX, [In] int aY, [In] int aDz, [In] int aDw, [In] int aButtonState);

    [DispId(1610743818)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void PutEventMultiTouch([In] int aCount, [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_I8), In] long[] aContacts, [In] uint aScanTime);

    [DispId(1610743819)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void PutEventMultiTouchString([In] int aCount, [MarshalAs(UnmanagedType.BStr), In] string aContacts, [In] uint aScanTime);

    [DispId(1610743820)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void InternalAndReservedMethod1IMouse();
  }
}
