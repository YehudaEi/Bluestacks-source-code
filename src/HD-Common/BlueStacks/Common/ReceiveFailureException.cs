// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.ReceiveFailureException
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.Runtime.Serialization;

namespace BlueStacks.Common
{
  [Serializable]
  public class ReceiveFailureException : Exception
  {
    public int ErrorCode { get; set; } = 2;

    public long BytesRecieved { get; private set; }

    public override string Message
    {
      get
      {
        return "A complete response was not received from the remote server.";
      }
    }

    public ReceiveFailureException(long bytesRecieved, Exception innerException)
      : base("", innerException)
    {
      this.BytesRecieved = bytesRecieved;
    }

    public ReceiveFailureException()
    {
    }

    public ReceiveFailureException(string message)
      : base(message)
    {
    }

    public ReceiveFailureException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected ReceiveFailureException(
      SerializationInfo serializationInfo,
      StreamingContext streamingContext)
      : base(serializationInfo, streamingContext)
    {
    }
  }
}
