// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.PSP_DEVICE_INTERFACE_DETAIL_DATA
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BlueStacks.Common
{
  public struct PSP_DEVICE_INTERFACE_DETAIL_DATA
  {
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
    public string DevicePath;

    public int cbSize { [IsReadOnly] get; set; }
  }
}
