// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.Interop.EUUIDException
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.Runtime.Serialization;

namespace BlueStacks.Common.Interop
{
  [Serializable]
  public class EUUIDException : Exception
  {
    public EUUIDException()
    {
    }

    public EUUIDException(string reason)
      : base(reason)
    {
    }

    public EUUIDException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected EUUIDException(SerializationInfo serializationInfo, StreamingContext streamingContext)
      : base(serializationInfo, streamingContext)
    {
    }
  }
}
