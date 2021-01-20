// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.Decoding.GifDecoderException
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.Runtime.Serialization;

namespace BlueStacks.Common.Decoding
{
  [Serializable]
  public class GifDecoderException : Exception
  {
    internal GifDecoderException()
    {
    }

    internal GifDecoderException(string message)
      : base(message)
    {
    }

    internal GifDecoderException(string message, Exception inner)
      : base(message, inner)
    {
    }

    protected GifDecoderException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
