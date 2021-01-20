// Decompiled with JetBrains decompiler
// Type: BstkTypeLib.IEventSource
// Assembly: BstkTypeLib, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 38E91E34-8BF8-4856-A23F-FE231831C5D8
// Assembly location: C:\Program Files\BlueStacks\BstkTypeLib.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BstkTypeLib
{
  [Guid("32D46250-0A15-477A-9DC8-6E1466ACC86A")]
  [TypeLibType(20544)]
  [ComImport]
  public interface IEventSource
  {
    [DispId(1610743808)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    IEventListener CreateListener();

    [DispId(1610743809)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    IEventSource CreateAggregator([MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_DISPATCH), In] IEventSource[] aSubordinates);

    [DispId(1610743810)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void RegisterListener([MarshalAs(UnmanagedType.Interface), In] IEventListener aListener, [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_I4), In] VBoxEventType[] aInteresting, [In] int aActive);

    [DispId(1610743811)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void UnregisterListener([MarshalAs(UnmanagedType.Interface), In] IEventListener aListener);

    [DispId(1610743812)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    int FireEvent([MarshalAs(UnmanagedType.Interface), In] IEvent aEvent, [In] int aTimeout);

    [DispId(1610743813)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    IEvent GetEvent([MarshalAs(UnmanagedType.Interface), In] IEventListener aListener, [In] int aTimeout);

    [DispId(1610743814)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void EventProcessed([MarshalAs(UnmanagedType.Interface), In] IEventListener aListener, [MarshalAs(UnmanagedType.Interface), In] IEvent aEvent);
  }
}
