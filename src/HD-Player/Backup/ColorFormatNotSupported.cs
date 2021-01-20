// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.ColorFormatNotSupported
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using System;

namespace BlueStacks.Player
{
  public class ColorFormatNotSupported : Exception
  {
    public ColorFormatNotSupported()
    {
    }

    public ColorFormatNotSupported(string message)
      : base(message)
    {
    }

    public ColorFormatNotSupported(string message, Exception inner)
      : base(message, inner)
    {
    }
  }
}
