// Decompiled with JetBrains decompiler
// Type: BstkTypeLib.IErrorInfo
// Assembly: BstkTypeLib, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 38E91E34-8BF8-4856-A23F-FE231831C5D8
// Assembly location: C:\Program Files\BlueStacks\BstkTypeLib.dll

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BstkTypeLib
{
  [InterfaceType(1)]
  [Guid("1CF2B120-547D-101B-8E65-08002B2BD119")]
  [ComImport]
  public interface IErrorInfo
  {
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void GetGUID(out Guid pGUID);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void GetSource([MarshalAs(UnmanagedType.BStr)] out string pBstrSource);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void GetDescription([MarshalAs(UnmanagedType.BStr)] out string pBstrDescription);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void GetHelpFile([MarshalAs(UnmanagedType.BStr)] out string pBstrHelpFile);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void GetHelpContext(out uint pdwHelpContext);
  }
}
