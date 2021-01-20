// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.MONITORINFO
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System.Runtime.CompilerServices;

namespace BlueStacks.Common
{
  public struct MONITORINFO
  {
    public int cbSize { [IsReadOnly] get; set; }

    public RECT rcMonitor { [IsReadOnly] get; set; }

    public RECT rcWork { [IsReadOnly] get; set; }

    public uint dwFlags { [IsReadOnly] get; set; }
  }
}
