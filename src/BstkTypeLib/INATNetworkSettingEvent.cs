// Decompiled with JetBrains decompiler
// Type: BstkTypeLib.INATNetworkSettingEvent
// Assembly: BstkTypeLib, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 38E91E34-8BF8-4856-A23F-FE231831C5D8
// Assembly location: C:\Program Files\BlueStacks\BstkTypeLib.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BstkTypeLib
{
  [TypeLibType(20544)]
  [Guid("230C0C50-49D9-441E-AE7C-11EC2E2CFBDC")]
  [ComImport]
  public interface INATNetworkSettingEvent : INATNetworkAlterEvent
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
    new string NetworkName { [DispId(1610809344), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; }

    [DispId(1610874880)]
    new int MidlDoesNotLikeEmptyInterfaces { [DispId(1610874880), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610940416)]
    int Enabled { [DispId(1610940416), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610940417)]
    string Network { [DispId(1610940417), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; }

    [DispId(1610940418)]
    string Gateway { [DispId(1610940418), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; }

    [DispId(1610940419)]
    int AdvertiseDefaultIPv6RouteEnabled { [DispId(1610940419), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610940420)]
    int NeedDhcpServer { [DispId(1610940420), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }
  }
}
