﻿// Decompiled with JetBrains decompiler
// Type: BstkTypeLib.IMousePointerShapeChangedEvent
// Assembly: BstkTypeLib, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 38E91E34-8BF8-4856-A23F-FE231831C5D8
// Assembly location: C:\Program Files\BlueStacks\BstkTypeLib.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BstkTypeLib
{
  [Guid("7DE22105-1FFC-4946-A75B-EA1899B5F5AD")]
  [TypeLibType(20544)]
  [ComImport]
  public interface IMousePointerShapeChangedEvent : IEvent
  {
    [DispId(1610743808)]
    [ComAliasName("BstkTypeLib.VBoxEventType")]
    new VBoxEventType Type { [DispId(1610743808), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: ComAliasName("BstkTypeLib.VBoxEventType")] get; }

    [DispId(1610743809)]
    new IEventSource Source { [DispId(1610743809), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.Interface)] get; }

    [DispId(1610743810)]
    new int Waitable { [DispId(1610743810), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743811)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    new void SetProcessed();

    [DispId(1610743812)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    new int WaitProcessed([In] int aTimeout);

    [DispId(1610809344)]
    int Visible { [DispId(1610809344), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610809345)]
    int Alpha { [DispId(1610809345), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610809346)]
    uint Xhot { [DispId(1610809346), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610809347)]
    uint Yhot { [DispId(1610809347), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610809348)]
    uint Width { [DispId(1610809348), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610809349)]
    uint Height { [DispId(1610809349), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610809350)]
    byte[] Shape { [DispId(1610809350), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UI1)] get; }
  }
}
