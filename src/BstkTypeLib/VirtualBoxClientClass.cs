// Decompiled with JetBrains decompiler
// Type: BstkTypeLib.VirtualBoxClientClass
// Assembly: BstkTypeLib, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 38E91E34-8BF8-4856-A23F-FE231831C5D8
// Assembly location: C:\Program Files\BlueStacks\BstkTypeLib.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BstkTypeLib
{
  [ClassInterface(0)]
  [TypeLibType(2)]
  [Guid("88CF5620-6C94-4704-99DA-B9C4812754F4")]
  [ComImport]
  public class VirtualBoxClientClass : IVirtualBoxClient, VirtualBoxClient
  {
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public extern VirtualBoxClientClass();

    [DispId(1610743808)]
    public virtual extern VirtualBox VirtualBox { [DispId(1610743808), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.Interface)] get; }

    [DispId(1610743809)]
    public virtual extern Session Session { [DispId(1610743809), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.Interface)] get; }

    [DispId(1610743810)]
    public virtual extern IEventSource EventSource { [DispId(1610743810), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.Interface)] get; }

    [DispId(1610743811)]
    public virtual extern uint InternalAndReservedAttribute1IVirtualBoxClient { [DispId(1610743811), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743812)]
    public virtual extern uint InternalAndReservedAttribute2IVirtualBoxClient { [DispId(1610743812), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743813)]
    public virtual extern uint InternalAndReservedAttribute3IVirtualBoxClient { [DispId(1610743813), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743814)]
    public virtual extern uint InternalAndReservedAttribute4IVirtualBoxClient { [DispId(1610743814), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743815)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void CheckMachineError([MarshalAs(UnmanagedType.Interface), In] IMachine aMachine);

    [DispId(1610743816)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void InternalAndReservedMethod1IVirtualBoxClient();

    [DispId(1610743817)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void InternalAndReservedMethod2IVirtualBoxClient();

    [DispId(1610743818)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void InternalAndReservedMethod3IVirtualBoxClient();

    [DispId(1610743819)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    public virtual extern void InternalAndReservedMethod4IVirtualBoxClient();
  }
}
