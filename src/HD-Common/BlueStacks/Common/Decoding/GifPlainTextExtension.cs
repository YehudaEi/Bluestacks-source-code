// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.Decoding.GifPlainTextExtension
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BlueStacks.Common.Decoding
{
  internal class GifPlainTextExtension : GifExtension
  {
    internal const int ExtensionLabel = 1;

    public int BlockSize { get; private set; }

    public int Left { get; private set; }

    public int Top { get; private set; }

    public int Width { get; private set; }

    public int Height { get; private set; }

    public int CellWidth { get; private set; }

    public int CellHeight { get; private set; }

    public int ForegroundColorIndex { get; private set; }

    public int BackgroundColorIndex { get; private set; }

    public string Text { get; private set; }

    public IList<GifExtension> Extensions { get; private set; }

    private GifPlainTextExtension()
    {
    }

    internal override GifBlockKind Kind
    {
      get
      {
        return GifBlockKind.GraphicRendering;
      }
    }

    internal static GifPlainTextExtension ReadPlainText(
      Stream stream,
      IEnumerable<GifExtension> controlExtensions,
      bool metadataOnly)
    {
      GifPlainTextExtension plainTextExtension = new GifPlainTextExtension();
      plainTextExtension.Read(stream, controlExtensions, metadataOnly);
      return plainTextExtension;
    }

    private void Read(
      Stream stream,
      IEnumerable<GifExtension> controlExtensions,
      bool metadataOnly)
    {
      byte[] buffer = new byte[13];
      stream.ReadAll(buffer, 0, buffer.Length);
      this.BlockSize = (int) buffer[0];
      if (this.BlockSize != 12)
        throw GifHelpers.InvalidBlockSizeException("Plain Text Extension", 12, this.BlockSize);
      this.Left = (int) BitConverter.ToUInt16(buffer, 1);
      this.Top = (int) BitConverter.ToUInt16(buffer, 3);
      this.Width = (int) BitConverter.ToUInt16(buffer, 5);
      this.Height = (int) BitConverter.ToUInt16(buffer, 7);
      this.CellWidth = (int) buffer[9];
      this.CellHeight = (int) buffer[10];
      this.ForegroundColorIndex = (int) buffer[11];
      this.BackgroundColorIndex = (int) buffer[12];
      this.Text = Encoding.ASCII.GetString(GifHelpers.ReadDataBlocks(stream, metadataOnly));
      this.Extensions = (IList<GifExtension>) controlExtensions.ToList<GifExtension>().AsReadOnly();
    }
  }
}
