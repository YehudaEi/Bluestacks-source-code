﻿// Decompiled with JetBrains decompiler
// Type: BstkTypeLib.IMediumRegisteredEvent
// Assembly: BstkTypeLib, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 38E91E34-8BF8-4856-A23F-FE231831C5D8
// Assembly location: C:\Program Files\BlueStacks\BstkTypeLib.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BstkTypeLib
{
  [Guid("B2E04C59-5B1A-42DB-8BAC-FC3BD699E1F0")]
  [TypeLibType(20544)]
  [ComImport]
  public interface IMediumRegisteredEvent : IEvent
  {
    [ComAliasName("BstkTypeLib.VBoxEventType")]
    [DispId(1610743808)]
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
    string MediumId { [DispId(1610809344), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; }

    [ComAliasName("BstkTypeLib.DeviceType")]
    [DispId(1610809345)]
    DeviceType MediumType { [DispId(1610809345), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: ComAliasName("BstkTypeLib.DeviceType")] get; }

    [DispId(1610809346)]
    int Registered { [DispId(1610809346), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }
  }
}