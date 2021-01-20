// Decompiled with JetBrains decompiler
// Type: DiscordRPC.Message.IMessage
// Assembly: DiscordRPC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B95F11B0-E129-4510-93B3-855D1EDEA68A
// Assembly location: C:\Program Files\BlueStacks\DiscordRPC.dll

using System;

namespace DiscordRPC.Message
{
  public abstract class IMessage
  {
    private DateTime _timecreated;

    public abstract MessageType Type { get; }

    public DateTime TimeCreated
    {
      get
      {
        return this._timecreated;
      }
    }

    public IMessage()
    {
      this._timecreated = DateTime.Now;
    }
  }
}
