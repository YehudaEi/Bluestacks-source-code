// Decompiled with JetBrains decompiler
// Type: BstkTypeLib.IEventListener
// Assembly: BstkTypeLib, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 38E91E34-8BF8-4856-A23F-FE231831C5D8
// Assembly location: C:\Program Files\BlueStacks\BstkTypeLib.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BstkTypeLib
{
  [Guid("12F4D0F7-05B6-4798-BC72-2F2CB7AA6B34")]
  [TypeLibType(20544)]
  [ComImport]
  public interface IEventListener
  {
    [DispId(1610743808)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void HandleEvent([MarshalAs(UnmanagedType.Interface), In] IEvent aEvent);
  }
}
