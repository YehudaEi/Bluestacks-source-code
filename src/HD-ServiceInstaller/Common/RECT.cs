// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.RECT
// Assembly: HD-ServiceInstaller, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 15F93427-26B3-4C7E-BAB1-0A00688BC4D4
// Assembly location: C:\Program Files\BlueStacks\HD-ServiceInstaller.exe

using System;
using System.Runtime.CompilerServices;

namespace BlueStacks.Common
{
  [Serializable]
  public struct RECT
  {
    public int Left { [IsReadOnly] get; set; }

    public int Top { [IsReadOnly] get; set; }

    public int Right { [IsReadOnly] get; set; }

    public int Bottom { [IsReadOnly] get; set; }

    public RECT(int left, int top, int right, int bottom)
    {
      this.Left = left;
      this.Top = top;
      this.Right = right;
      this.Bottom = bottom;
    }
  }
}
