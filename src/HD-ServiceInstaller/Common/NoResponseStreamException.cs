// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.NoResponseStreamException
// Assembly: HD-ServiceInstaller, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 15F93427-26B3-4C7E-BAB1-0A00688BC4D4
// Assembly location: C:\Program Files\BlueStacks\HD-ServiceInstaller.exe

using System;
using System.Runtime.Serialization;

namespace BlueStacks.Common
{
  [Serializable]
  public class NoResponseStreamException : Exception
  {
    public int ErrorCode { get; set; } = 2;

    public long BytesRecieved { get; private set; }

    public override string Message
    {
      get
      {
        return "Could not get a response stream from the remote server.";
      }
    }

    public NoResponseStreamException(Exception innerException)
      : base("", innerException)
    {
    }

    public NoResponseStreamException()
    {
    }

    public NoResponseStreamException(string message)
      : base(message)
    {
    }

    public NoResponseStreamException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected NoResponseStreamException(
      SerializationInfo serializationInfo,
      StreamingContext streamingContext)
      : base(serializationInfo, streamingContext)
    {
    }
  }
}
