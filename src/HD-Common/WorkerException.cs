// Decompiled with JetBrains decompiler
// Type: WorkerException
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.Runtime.Serialization;

[Serializable]
public class WorkerException : Exception
{
  public WorkerException(string msg, Exception e)
    : base(msg, e)
  {
  }

  public WorkerException()
  {
  }

  public WorkerException(string message)
    : base(message)
  {
  }

  protected WorkerException(SerializationInfo serializationInfo, StreamingContext streamingContext)
    : base(serializationInfo, streamingContext)
  {
  }
}
