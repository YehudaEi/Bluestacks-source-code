// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.ENoPortAvailableException
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.Runtime.Serialization;

namespace BlueStacks.Common
{
  [Serializable]
  public class ENoPortAvailableException : Exception
  {
    public ENoPortAvailableException(string reason)
      : base(reason)
    {
    }

    public ENoPortAvailableException()
    {
    }

    public ENoPortAvailableException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected ENoPortAvailableException(
      SerializationInfo serializationInfo,
      StreamingContext streamingContext)
      : base(serializationInfo, streamingContext)
    {
    }
  }
}
