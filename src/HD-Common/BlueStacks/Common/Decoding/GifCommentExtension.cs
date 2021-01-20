// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.Decoding.GifCommentExtension
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System.IO;
using System.Text;

namespace BlueStacks.Common.Decoding
{
  internal class GifCommentExtension : GifExtension
  {
    internal const int ExtensionLabel = 254;

    public string Text { get; private set; }

    private GifCommentExtension()
    {
    }

    internal override GifBlockKind Kind
    {
      get
      {
        return GifBlockKind.SpecialPurpose;
      }
    }

    internal static GifCommentExtension ReadComment(Stream stream)
    {
      GifCommentExtension commentExtension = new GifCommentExtension();
      commentExtension.Read(stream);
      return commentExtension;
    }

    private void Read(Stream stream)
    {
      byte[] bytes = GifHelpers.ReadDataBlocks(stream, false);
      if (bytes == null)
        return;
      this.Text = Encoding.ASCII.GetString(bytes);
    }
  }
}
