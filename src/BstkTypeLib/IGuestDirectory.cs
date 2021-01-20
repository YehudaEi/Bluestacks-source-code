// Decompiled with JetBrains decompiler
// Type: BstkTypeLib.IGuestDirectory
// Assembly: BstkTypeLib, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 38E91E34-8BF8-4856-A23F-FE231831C5D8
// Assembly location: C:\Program Files\BlueStacks\BstkTypeLib.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BstkTypeLib
{
  [TypeLibType(20544)]
  [Guid("C49FDD19-A583-4C7F-BB53-AE078192598A")]
  [ComImport]
  public interface IGuestDirectory : IDirectory
  {
    [DispId(1610743808)]
    new string DirectoryName { [DispId(1610743808), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; }

    [DispId(1610743809)]
    new string Filter { [DispId(1610743809), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; }

    [DispId(1610743810)]
    new uint InternalAndReservedAttribute1IDirectory { [DispId(1610743810), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743811)]
    new uint InternalAndReservedAttribute2IDirectory { [DispId(1610743811), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743812)]
    new uint InternalAndReservedAttribute3IDirectory { [DispId(1610743812), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743813)]
    new uint InternalAndReservedAttribute4IDirectory { [DispId(1610743813), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743814)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    new void Close();

    [DispId(1610743815)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    new IFsObjInfo Read();

    [DispId(1610743816)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    new void InternalAndReservedMethod1IDirectory();

    [DispId(1610743817)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    new void InternalAndReservedMethod2IDirectory();

    [DispId(1610809344)]
    int MidlDoesNotLikeEmptyInterfaces { [DispId(1610809344), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }
  }
}
