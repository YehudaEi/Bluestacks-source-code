// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.UnknownErrorException
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.Runtime.Serialization;

namespace BlueStacks.Common
{
  [Serializable]
  public class UnknownErrorException : Exception
  {
    public int ErrorCode { get; set; } = 99;

    public override string Message
    {
      get
      {
        return "An exception of an unknown type has occurred.";
      }
    }

    public UnknownErrorException(Exception innerException)
      : base("", innerException)
    {
    }

    public UnknownErrorException()
    {
    }

    public UnknownErrorException(string message)
      : base(message)
    {
    }

    public UnknownErrorException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected UnknownErrorException(
      SerializationInfo serializationInfo,
      StreamingContext streamingContext)
      : base(serializationInfo, streamingContext)
    {
    }
  }
}
