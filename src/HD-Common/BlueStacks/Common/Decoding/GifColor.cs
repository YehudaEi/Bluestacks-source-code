// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.Decoding.GifColor
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.Globalization;

namespace BlueStacks.Common.Decoding
{
  internal struct GifColor
  {
    private readonly byte R;
    private readonly byte G;
    private readonly byte B;

    internal GifColor(byte r, byte g, byte b)
    {
      this.R = r;
      this.G = g;
      this.B = b;
    }

    public override string ToString()
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "#{0:x2}{1:x2}{2:x2}", (object) this.R, (object) this.G, (object) this.B);
    }
  }
}
