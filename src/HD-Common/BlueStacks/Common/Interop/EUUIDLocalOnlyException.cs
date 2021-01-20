// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.Interop.EUUIDLocalOnlyException
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.Runtime.Serialization;

namespace BlueStacks.Common.Interop
{
  [Serializable]
  public class EUUIDLocalOnlyException : EUUIDException
  {
    public EUUIDLocalOnlyException()
    {
    }

    public EUUIDLocalOnlyException(string message)
      : base(message)
    {
    }

    public EUUIDLocalOnlyException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected EUUIDLocalOnlyException(
      SerializationInfo serializationInfo,
      StreamingContext streamingContext)
      : base(serializationInfo, streamingContext)
    {
    }
  }
}
