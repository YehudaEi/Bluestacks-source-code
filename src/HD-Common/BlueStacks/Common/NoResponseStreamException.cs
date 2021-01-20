// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.NoResponseStreamException
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

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
