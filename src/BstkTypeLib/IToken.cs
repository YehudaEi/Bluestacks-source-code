// Decompiled with JetBrains decompiler
// Type: BstkTypeLib.IToken
// Assembly: BstkTypeLib, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 38E91E34-8BF8-4856-A23F-FE231831C5D8
// Assembly location: C:\Program Files\BlueStacks\BstkTypeLib.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BstkTypeLib
{
  [Guid("05A6253C-90DA-494F-87D6-64E220DD0BB2")]
  [TypeLibType(20544)]
  [ComImport]
  public interface IToken
  {
    [DispId(1610743808)]
    uint InternalAndReservedAttribute1IToken { [DispId(1610743808), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743809)]
    uint InternalAndReservedAttribute2IToken { [DispId(1610743809), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743810)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void Abandon();

    [DispId(1610743811)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void Dummy();

    [DispId(1610743812)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void InternalAndReservedMethod1IToken();
  }
}
