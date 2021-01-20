// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.Decoding.GifTrailer
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

namespace BlueStacks.Common.Decoding
{
  internal class GifTrailer : GifBlock
  {
    internal const int TrailerByte = 59;

    private GifTrailer()
    {
    }

    internal override GifBlockKind Kind
    {
      get
      {
        return GifBlockKind.Other;
      }
    }

    internal static GifTrailer ReadTrailer()
    {
      return new GifTrailer();
    }
  }
}
