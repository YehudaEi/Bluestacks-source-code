// Decompiled with JetBrains decompiler
// Type: BstkTypeLib.INATRedirectEvent
// Assembly: BstkTypeLib, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 38E91E34-8BF8-4856-A23F-FE231831C5D8
// Assembly location: C:\Program Files\BlueStacks\BstkTypeLib.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BstkTypeLib
{
  [TypeLibType(20544)]
  [Guid("10206EB7-CB74-40D0-A912-97A8D8C4B067")]
  [ComImport]
  public interface INATRedirectEvent : IMachineEvent
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
    new string MachineId { [DispId(1610809344), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; }

    [DispId(1610874880)]
    uint Slot { [DispId(1610874880), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610874881)]
    int Remove { [DispId(1610874881), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610874882)]
    string Name { [DispId(1610874882), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; }

    [DispId(1610874883)]
    [ComAliasName("BstkTypeLib.NATProtocol")]
    NATProtocol Proto { [DispId(1610874883), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: ComAliasName("BstkTypeLib.NATProtocol")] get; }

    [DispId(1610874884)]
    string HostIP { [DispId(1610874884), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; }

    [DispId(1610874885)]
    int HostPort { [DispId(1610874885), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610874886)]
    string GuestIP { [DispId(1610874886), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; }

    [DispId(1610874887)]
    int GuestPort { [DispId(1610874887), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }
  }
}
