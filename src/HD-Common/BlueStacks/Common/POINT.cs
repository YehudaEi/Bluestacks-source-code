﻿// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.POINT
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.Runtime.CompilerServices;

namespace BlueStacks.Common
{
  [Serializable]
  public struct POINT
  {
    public int X { [IsReadOnly] get; set; }

    public int Y { [IsReadOnly] get; set; }

    public POINT(int x, int y)
    {
      this.X = x;
      this.Y = y;
    }
  }
}
