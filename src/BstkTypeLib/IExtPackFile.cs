// Decompiled with JetBrains decompiler
// Type: BstkTypeLib.IExtPackFile
// Assembly: BstkTypeLib, Version=1.3.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 38E91E34-8BF8-4856-A23F-FE231831C5D8
// Assembly location: C:\Program Files\BlueStacks\BstkTypeLib.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BstkTypeLib
{
  [Guid("357DE5F9-D7B3-4533-94B2-C99753662EB9")]
  [TypeLibType(20544)]
  [ComImport]
  public interface IExtPackFile : IExtPackBase
  {
    [DispId(1610743808)]
    new string Name { [DispId(1610743808), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; }

    [DispId(1610743809)]
    new string Description { [DispId(1610743809), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; }

    [DispId(1610743810)]
    new string Version { [DispId(1610743810), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; }

    [DispId(1610743811)]
    new uint Revision { [DispId(1610743811), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743812)]
    new string Edition { [DispId(1610743812), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; }

    [DispId(1610743813)]
    new string VRDEModule { [DispId(1610743813), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; }

    [DispId(1610743814)]
    new IExtPackPlugIn[] PlugIns { [DispId(1610743814), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_DISPATCH)] get; }

    [DispId(1610743815)]
    new int Usable { [DispId(1610743815), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743816)]
    new string WhyUnusable { [DispId(1610743816), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; }

    [DispId(1610743817)]
    new int ShowLicense { [DispId(1610743817), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743818)]
    new string License { [DispId(1610743818), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; }

    [DispId(1610743819)]
    new uint InternalAndReservedAttribute1IExtPackBase { [DispId(1610743819), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743820)]
    new uint InternalAndReservedAttribute2IExtPackBase { [DispId(1610743820), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743821)]
    new uint InternalAndReservedAttribute3IExtPackBase { [DispId(1610743821), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743822)]
    new uint InternalAndReservedAttribute4IExtPackBase { [DispId(1610743822), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610743823)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.BStr)]
    new string QueryLicense([MarshalAs(UnmanagedType.BStr), In] string aPreferredLocale, [MarshalAs(UnmanagedType.BStr), In] string aPreferredLanguage, [MarshalAs(UnmanagedType.BStr), In] string aFormat);

    [DispId(1610743824)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    new void InternalAndReservedMethod1IExtPackBase();

    [DispId(1610809344)]
    string FilePath { [DispId(1610809344), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] [return: MarshalAs(UnmanagedType.BStr)] get; }

    [DispId(1610809345)]
    uint InternalAndReservedAttribute1IExtPackFile { [DispId(1610809345), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610809346)]
    uint InternalAndReservedAttribute2IExtPackFile { [DispId(1610809346), MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)] get; }

    [DispId(1610809347)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    [return: MarshalAs(UnmanagedType.Interface)]
    IProgress Install([In] int aReplace, [MarshalAs(UnmanagedType.BStr), In] string aDisplayInfo);

    [DispId(1610809348)]
    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void InternalAndReservedMethod1IExtPackFile();
  }
}
