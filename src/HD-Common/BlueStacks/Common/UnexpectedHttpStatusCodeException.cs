﻿// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.UnexpectedHttpStatusCodeException
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.Net;
using System.Runtime.Serialization;

namespace BlueStacks.Common
{
  [Serializable]
  public class UnexpectedHttpStatusCodeException : Exception
  {
    public int ErrorCode { get; set; } = 4;

    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;

    public override string Message
    {
      get
      {
        return "The remote server returned an unexpected status code.";
      }
    }

    public UnexpectedHttpStatusCodeException(HttpStatusCode statusCode)
    {
      this.StatusCode = statusCode;
    }

    public UnexpectedHttpStatusCodeException()
    {
    }

    public UnexpectedHttpStatusCodeException(string message)
      : base(message)
    {
    }

    public UnexpectedHttpStatusCodeException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected UnexpectedHttpStatusCodeException(
      SerializationInfo serializationInfo,
      StreamingContext streamingContext)
      : base(serializationInfo, streamingContext)
    {
    }
  }
}
