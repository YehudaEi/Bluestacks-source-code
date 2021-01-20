// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.Decoding.GifApplicationExtension
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.IO;
using System.Text;

namespace BlueStacks.Common.Decoding
{
  internal class GifApplicationExtension : GifExtension
  {
    internal const int ExtensionLabel = 255;

    public int BlockSize { get; private set; }

    public string ApplicationIdentifier { get; private set; }

    public byte[] AuthenticationCode { get; private set; }

    public byte[] Data { get; private set; }

    private GifApplicationExtension()
    {
    }

    internal override GifBlockKind Kind
    {
      get
      {
        return GifBlockKind.SpecialPurpose;
      }
    }

    internal static GifApplicationExtension ReadApplication(Stream stream)
    {
      GifApplicationExtension applicationExtension = new GifApplicationExtension();
      applicationExtension.Read(stream);
      return applicationExtension;
    }

    private void Read(Stream stream)
    {
      byte[] numArray1 = new byte[12];
      stream.ReadAll(numArray1, 0, numArray1.Length);
      this.BlockSize = (int) numArray1[0];
      if (this.BlockSize != 11)
        throw GifHelpers.InvalidBlockSizeException("Application Extension", 11, this.BlockSize);
      this.ApplicationIdentifier = Encoding.ASCII.GetString(numArray1, 1, 8);
      byte[] numArray2 = new byte[3];
      Array.Copy((Array) numArray1, 9, (Array) numArray2, 0, 3);
      this.AuthenticationCode = numArray2;
      this.Data = GifHelpers.ReadDataBlocks(stream, false);
    }
  }
}
