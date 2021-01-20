// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.RECT
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

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
