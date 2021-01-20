// Decompiled with JetBrains decompiler
// Type: WorkerException
// Assembly: HD-ServiceInstaller, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 15F93427-26B3-4C7E-BAB1-0A00688BC4D4
// Assembly location: C:\Program Files\BlueStacks\HD-ServiceInstaller.exe

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
