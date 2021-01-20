// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.Decoding.GifImageDescriptor
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.IO;

namespace BlueStacks.Common.Decoding
{
  internal class GifImageDescriptor
  {
    public int Left { get; private set; }

    public int Top { get; private set; }

    public int Width { get; private set; }

    public int Height { get; private set; }

    public bool HasLocalColorTable { get; private set; }

    public bool Interlace { get; private set; }

    public bool IsLocalColorTableSorted { get; private set; }

    public int LocalColorTableSize { get; private set; }

    private GifImageDescriptor()
    {
    }

    internal static GifImageDescriptor ReadImageDescriptor(Stream stream)
    {
      GifImageDescriptor gifImageDescriptor = new GifImageDescriptor();
      gifImageDescriptor.Read(stream);
      return gifImageDescriptor;
    }

    private void Read(Stream stream)
    {
      byte[] buffer = new byte[9];
      stream.ReadAll(buffer, 0, buffer.Length);
      this.Left = (int) BitConverter.ToUInt16(buffer, 0);
      this.Top = (int) BitConverter.ToUInt16(buffer, 2);
      this.Width = (int) BitConverter.ToUInt16(buffer, 4);
      this.Height = (int) BitConverter.ToUInt16(buffer, 6);
      byte num = buffer[8];
      this.HasLocalColorTable = ((uint) num & 128U) > 0U;
      this.Interlace = ((uint) num & 64U) > 0U;
      this.IsLocalColorTableSorted = ((uint) num & 32U) > 0U;
      this.LocalColorTableSize = 1 << ((int) num & 7) + 1;
    }
  }
}
