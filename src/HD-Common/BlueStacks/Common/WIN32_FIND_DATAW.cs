// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.WIN32_FIND_DATAW
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BlueStacks.Common
{
  [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
  public struct WIN32_FIND_DATAW
  {
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
    public string cFileName;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
    public string cAlternateFileName;

    public uint dwFileAttributes { [IsReadOnly] get; set; }

    public long ftCreationTime { [IsReadOnly] get; set; }

    public long ftLastAccessTime { [IsReadOnly] get; set; }

    public long ftLastWriteTime { [IsReadOnly] get; set; }

    public uint nFileSizeHigh { [IsReadOnly] get; set; }

    public uint nFileSizeLow { [IsReadOnly] get; set; }

    public uint dwReserved0 { [IsReadOnly] get; set; }

    public uint dwReserved1 { [IsReadOnly] get; set; }
  }
}
