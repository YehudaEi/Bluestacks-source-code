// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.Decoding.GifHelpers
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace BlueStacks.Common.Decoding
{
  internal static class GifHelpers
  {
    public static string ReadString(Stream stream, int length)
    {
      byte[] numArray = new byte[length];
      stream.ReadAll(numArray, 0, length);
      return Encoding.ASCII.GetString(numArray);
    }

    public static byte[] ReadDataBlocks(Stream stream, bool discard)
    {
      MemoryStream memoryStream = discard ? (MemoryStream) null : new MemoryStream();
      using (memoryStream)
      {
        int count;
        while ((count = stream.ReadByte()) > 0)
        {
          byte[] buffer = new byte[count];
          stream.ReadAll(buffer, 0, count);
          memoryStream?.Write(buffer, 0, count);
        }
        return memoryStream?.ToArray();
      }
    }

    public static GifColor[] ReadColorTable(Stream stream, int size)
    {
      int count = 3 * size;
      byte[] buffer = new byte[count];
      stream.ReadAll(buffer, 0, count);
      GifColor[] gifColorArray = new GifColor[size];
      for (int index = 0; index < size; ++index)
      {
        byte r = buffer[3 * index];
        byte g = buffer[3 * index + 1];
        byte b = buffer[3 * index + 2];
        gifColorArray[index] = new GifColor(r, g, b);
      }
      return gifColorArray;
    }

    public static bool IsNetscapeExtension(GifApplicationExtension ext)
    {
      return ext.ApplicationIdentifier == "NETSCAPE" && Encoding.ASCII.GetString(ext.AuthenticationCode) == "2.0";
    }

    public static ushort GetRepeatCount(GifApplicationExtension ext)
    {
      return ext.Data.Length >= 3 ? BitConverter.ToUInt16(ext.Data, 1) : (ushort) 1;
    }

    public static Exception UnexpectedEndOfStreamException()
    {
      return (Exception) new GifDecoderException("Unexpected end of stream before trailer was encountered");
    }

    public static Exception UnknownBlockTypeException(int blockId)
    {
      return (Exception) new GifDecoderException("Unknown block type: 0x" + blockId.ToString("x2", (IFormatProvider) CultureInfo.InvariantCulture));
    }

    public static Exception UnknownExtensionTypeException(int extensionLabel)
    {
      return (Exception) new GifDecoderException("Unknown extension type: 0x" + extensionLabel.ToString("x2", (IFormatProvider) CultureInfo.InvariantCulture));
    }

    public static Exception InvalidBlockSizeException(
      string blockName,
      int expectedBlockSize,
      int actualBlockSize)
    {
      return (Exception) new GifDecoderException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Invalid block size for {0}. Expected {1}, but was {2}", (object) blockName, (object) expectedBlockSize, (object) actualBlockSize));
    }

    public static Exception InvalidSignatureException(string signature)
    {
      return (Exception) new GifDecoderException("Invalid file signature: " + signature);
    }

    public static Exception UnsupportedVersionException(string version)
    {
      return (Exception) new GifDecoderException("Unsupported version: " + version);
    }

    public static void ReadAll(this Stream stream, byte[] buffer, int offset, int count)
    {
      int num = 0;
      while (num < count)
        num += stream.Read(buffer, offset + num, count - num);
    }
  }
}
