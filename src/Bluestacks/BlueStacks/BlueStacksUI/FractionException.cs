// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.FractionException
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using System;
using System.Runtime.Serialization;

namespace BlueStacks.BlueStacksUI
{
  [Serializable]
  public class FractionException : Exception
  {
    public FractionException()
    {
    }

    public FractionException(string Message)
      : base(Message)
    {
    }

    public FractionException(string Message, Exception InnerException)
      : base(Message, InnerException)
    {
    }

    protected FractionException(
      SerializationInfo serializationInfo,
      StreamingContext streamingContext)
      : base(serializationInfo, streamingContext)
    {
    }
  }
}
