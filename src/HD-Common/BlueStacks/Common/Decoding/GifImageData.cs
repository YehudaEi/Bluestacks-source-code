// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.Decoding.GifImageData
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System.IO;

namespace BlueStacks.Common.Decoding
{
  internal class GifImageData
  {
    public byte LzwMinimumCodeSize { get; set; }

    public byte[] CompressedData { get; set; }

    private GifImageData()
    {
    }

    internal static GifImageData ReadImageData(Stream stream, bool metadataOnly)
    {
      GifImageData gifImageData = new GifImageData();
      gifImageData.Read(stream, metadataOnly);
      return gifImageData;
    }

    private void Read(Stream stream, bool metadataOnly)
    {
      this.LzwMinimumCodeSize = (byte) stream.ReadByte();
      this.CompressedData = GifHelpers.ReadDataBlocks(stream, metadataOnly);
    }
  }
}
