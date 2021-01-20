// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.Decoding.GifExtension
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System.Collections.Generic;
using System.IO;

namespace BlueStacks.Common.Decoding
{
  internal abstract class GifExtension : GifBlock
  {
    internal const int ExtensionIntroducer = 33;

    internal static GifExtension ReadExtension(
      Stream stream,
      IEnumerable<GifExtension> controlExtensions,
      bool metadataOnly)
    {
      int extensionLabel = stream.ReadByte();
      if (extensionLabel < 0)
        throw GifHelpers.UnexpectedEndOfStreamException();
      switch (extensionLabel)
      {
        case 1:
          return (GifExtension) GifPlainTextExtension.ReadPlainText(stream, controlExtensions, metadataOnly);
        case 249:
          return (GifExtension) GifGraphicControlExtension.ReadGraphicsControl(stream);
        case 254:
          return (GifExtension) GifCommentExtension.ReadComment(stream);
        case (int) byte.MaxValue:
          return (GifExtension) GifApplicationExtension.ReadApplication(stream);
        default:
          throw GifHelpers.UnknownExtensionTypeException(extensionLabel);
      }
    }
  }
}
