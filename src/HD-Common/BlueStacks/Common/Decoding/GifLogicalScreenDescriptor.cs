// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.Decoding.GifLogicalScreenDescriptor
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.IO;

namespace BlueStacks.Common.Decoding
{
  internal class GifLogicalScreenDescriptor
  {
    public int Width { get; private set; }

    public int Height { get; private set; }

    public bool HasGlobalColorTable { get; private set; }

    public int ColorResolution { get; private set; }

    public bool IsGlobalColorTableSorted { get; private set; }

    public int GlobalColorTableSize { get; private set; }

    public int BackgroundColorIndex { get; private set; }

    public double PixelAspectRatio { get; private set; }

    internal static GifLogicalScreenDescriptor ReadLogicalScreenDescriptor(
      Stream stream)
    {
      GifLogicalScreenDescriptor screenDescriptor = new GifLogicalScreenDescriptor();
      screenDescriptor.Read(stream);
      return screenDescriptor;
    }

    private void Read(Stream stream)
    {
      byte[] buffer = new byte[7];
      stream.ReadAll(buffer, 0, buffer.Length);
      this.Width = (int) BitConverter.ToUInt16(buffer, 0);
      this.Height = (int) BitConverter.ToUInt16(buffer, 2);
      byte num = buffer[4];
      this.HasGlobalColorTable = ((uint) num & 128U) > 0U;
      this.ColorResolution = (((int) num & 112) >> 4) + 1;
      this.IsGlobalColorTableSorted = ((uint) num & 8U) > 0U;
      this.GlobalColorTableSize = 1 << ((int) num & 7) + 1;
      this.BackgroundColorIndex = (int) buffer[5];
      this.PixelAspectRatio = buffer[5] == (byte) 0 ? 0.0 : (double) (15 + (int) buffer[5]) / 64.0;
    }
  }
}
