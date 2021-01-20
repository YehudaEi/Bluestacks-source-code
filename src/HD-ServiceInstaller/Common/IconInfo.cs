// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.IconInfo
// Assembly: HD-ServiceInstaller, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 15F93427-26B3-4C7E-BAB1-0A00688BC4D4
// Assembly location: C:\Program Files\BlueStacks\HD-ServiceInstaller.exe

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
