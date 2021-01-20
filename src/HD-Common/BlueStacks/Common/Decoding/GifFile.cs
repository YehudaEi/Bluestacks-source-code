// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.Decoding.GifFile
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BlueStacks.Common.Decoding
{
  internal class GifFile
  {
    public GifHeader Header { get; private set; }

    public GifColor[] GlobalColorTable { get; set; }

    public IList<GifFrame> Frames { get; set; }

    public IList<GifExtension> Extensions { get; set; }

    public ushort RepeatCount { get; set; }

    private GifFile()
    {
    }

    internal static GifFile ReadGifFile(Stream stream, bool metadataOnly)
    {
      GifFile gifFile = new GifFile();
      gifFile.Read(stream, metadataOnly);
      return gifFile;
    }

    private void Read(Stream stream, bool metadataOnly)
    {
      this.Header = GifHeader.ReadHeader(stream);
      if (this.Header.LogicalScreenDescriptor.HasGlobalColorTable)
        this.GlobalColorTable = GifHelpers.ReadColorTable(stream, this.Header.LogicalScreenDescriptor.GlobalColorTableSize);
      this.ReadFrames(stream, metadataOnly);
      GifApplicationExtension ext = this.Extensions.OfType<GifApplicationExtension>().FirstOrDefault<GifApplicationExtension>(new Func<GifApplicationExtension, bool>(GifHelpers.IsNetscapeExtension));
      if (ext != null)
        this.RepeatCount = GifHelpers.GetRepeatCount(ext);
      else
        this.RepeatCount = (ushort) 1;
    }

    private void ReadFrames(Stream stream, bool metadataOnly)
    {
      List<GifFrame> gifFrameList = new List<GifFrame>();
      List<GifExtension> gifExtensionList1 = new List<GifExtension>();
      List<GifExtension> gifExtensionList2 = new List<GifExtension>();
      while (true)
      {
        GifBlock gifBlock = GifBlock.ReadBlock(stream, (IEnumerable<GifExtension>) gifExtensionList1, metadataOnly);
        if (gifBlock.Kind == GifBlockKind.GraphicRendering)
          gifExtensionList1 = new List<GifExtension>();
        switch (gifBlock)
        {
          case GifFrame _:
            gifFrameList.Add((GifFrame) gifBlock);
            continue;
          case GifExtension gifExtension:
            switch (gifExtension.Kind)
            {
              case GifBlockKind.Control:
                gifExtensionList1.Add(gifExtension);
                continue;
              case GifBlockKind.SpecialPurpose:
                gifExtensionList2.Add(gifExtension);
                continue;
              default:
                continue;
            }
          case GifTrailer _:
            goto label_8;
          default:
            continue;
        }
      }
label_8:
      this.Frames = (IList<GifFrame>) gifFrameList.AsReadOnly();
      this.Extensions = (IList<GifExtension>) gifExtensionList2.AsReadOnly();
    }
  }
}
