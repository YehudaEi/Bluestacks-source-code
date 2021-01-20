// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.IconInfo
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.Runtime.CompilerServices;

namespace BlueStacks.Common
{
  public struct IconInfo
  {
    public bool fIcon { [IsReadOnly] get; set; }

    public int xHotspot { [IsReadOnly] get; set; }

    public int yHotspot { [IsReadOnly] get; set; }

    public IntPtr hbmMask { [IsReadOnly] get; set; }

    public IntPtr hbmColor { [IsReadOnly] get; set; }
  }
}
