// Decompiled with JetBrains decompiler
// Type: BstkTypeLib.IDisplaySourceBitmap
// Assembly: BstkTypeLib, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 38E91E34-8BF8-4856-A23F-FE231831C5D8
// Assembly location: C:\Program Files\BlueStacks\BstkTypeLib.dll

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BstkTypeLib
{
  [TypeLibType(20544)]
  [ComConversionLoss]
  [Guid("25D3E21B-452C-43EA-B0C2-0D2DB23130AB")]
  [ComImport]
  public interface IDisplaySourceBitmap
  {
    [DispId(1610743808)]
    uint ScreenId { [DispId(1610743808), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743809)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void QueryBitmapInfo(
      [Out] IntPtr aAddress,
      out uint aWidth,
      out uint aHeight,
      out uint aBitsPerPixel,
      out uint aBytesPerLine,
      [ComAliasName("BstkTypeLib.BitmapFormat")] out BitmapFormat aBitmapFormat);
  }
}
