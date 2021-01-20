// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.Decoding.GifGraphicControlExtension
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.IO;

namespace BlueStacks.Common.Decoding
{
  internal class GifGraphicControlExtension : GifExtension
  {
    internal const int ExtensionLabel = 249;

    public int BlockSize { get; private set; }

    public int DisposalMethod { get; private set; }

    public bool UserInput { get; private set; }

    public bool HasTransparency { get; private set; }

    public int Delay { get; private set; }

    public int TransparencyIndex { get; private set; }

    private GifGraphicControlExtension()
    {
    }

    internal override GifBlockKind Kind
    {
      get
      {
        return GifBlockKind.Control;
      }
    }

    internal static GifGraphicControlExtension ReadGraphicsControl(
      Stream stream)
    {
      GifGraphicControlExtension controlExtension = new GifGraphicControlExtension();
      controlExtension.Read(stream);
      return controlExtension;
    }

    private void Read(Stream stream)
    {
      byte[] buffer = new byte[6];
      stream.ReadAll(buffer, 0, buffer.Length);
      this.BlockSize = (int) buffer[0];
      if (this.BlockSize != 4)
        throw GifHelpers.InvalidBlockSizeException("Graphic Control Extension", 4, this.BlockSize);
      byte num = buffer[1];
      this.DisposalMethod = ((int) num & 28) >> 2;
      this.UserInput = ((uint) num & 2U) > 0U;
      this.HasTransparency = ((uint) num & 1U) > 0U;
      this.Delay = (int) BitConverter.ToUInt16(buffer, 2) * 10;
      this.TransparencyIndex = (int) buffer[4];
    }
  }
}
