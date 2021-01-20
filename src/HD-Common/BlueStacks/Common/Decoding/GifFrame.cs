// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.Decoding.GifFrame
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BlueStacks.Common.Decoding
{
  internal class GifFrame : GifBlock
  {
    internal const int ImageSeparator = 44;

    public GifImageDescriptor Descriptor { get; private set; }

    public GifColor[] LocalColorTable { get; private set; }

    public IList<GifExtension> Extensions { get; private set; }

    public GifImageData ImageData { get; private set; }

    private GifFrame()
    {
    }

    internal override GifBlockKind Kind
    {
      get
      {
        return GifBlockKind.GraphicRendering;
      }
    }

    internal static GifFrame ReadFrame(
      Stream stream,
      IEnumerable<GifExtension> controlExtensions,
      bool metadataOnly)
    {
      GifFrame gifFrame = new GifFrame();
      gifFrame.Read(stream, controlExtensions, metadataOnly);
      return gifFrame;
    }

    private void Read(
      Stream stream,
      IEnumerable<GifExtension> controlExtensions,
      bool metadataOnly)
    {
      this.Descriptor = GifImageDescriptor.ReadImageDescriptor(stream);
      if (this.Descriptor.HasLocalColorTable)
        this.LocalColorTable = GifHelpers.ReadColorTable(stream, this.Descriptor.LocalColorTableSize);
      this.ImageData = GifImageData.ReadImageData(stream, metadataOnly);
      this.Extensions = (IList<GifExtension>) controlExtensions.ToList<GifExtension>().AsReadOnly();
    }
  }
}
