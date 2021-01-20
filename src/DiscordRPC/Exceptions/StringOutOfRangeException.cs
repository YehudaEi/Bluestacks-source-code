// Decompiled with JetBrains decompiler
// Type: DiscordRPC.Exceptions.StringOutOfRangeException
// Assembly: DiscordRPC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B95F11B0-E129-4510-93B3-855D1EDEA68A
// Assembly location: C:\Program Files\BlueStacks\DiscordRPC.dll

using System;

namespace DiscordRPC.Exceptions
{
  public class StringOutOfRangeException : Exception
  {
    internal StringOutOfRangeException(string message)
      : base(message)
    {
    }

    internal StringOutOfRangeException(int max)
      : this("String", max)
    {
    }

    internal StringOutOfRangeException(string argument, int max)
      : this(string.Format("{0} is too long. Expected a maximum length of {1}", (object) argument, (object) max))
    {
    }

    internal StringOutOfRangeException(int size, int max)
      : this("String", size, max)
    {
    }

    internal StringOutOfRangeException(string argument, int size, int max)
      : this(string.Format("{0} is too long. Expected a maximum length of {1} but got {2}.", (object) argument, (object) size, (object) max))
    {
    }
  }
}
