// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.ConnectFailureException
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.Runtime.Serialization;

namespace BlueStacks.Common
{
  [Serializable]
  public class ConnectFailureException : Exception
  {
    public int ErrorCode { get; set; } = 1;

    public override string Message
    {
      get
      {
        return "The remote service point could not be contacted.";
      }
    }

    public ConnectFailureException(Exception innerException)
      : base("", innerException)
    {
    }

    public ConnectFailureException()
    {
    }

    public ConnectFailureException(string message)
      : base(message)
    {
    }

    public ConnectFailureException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected ConnectFailureException(
      SerializationInfo serializationInfo,
      StreamingContext streamingContext)
      : base(serializationInfo, streamingContext)
    {
    }
  }
}
