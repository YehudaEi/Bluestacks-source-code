// Decompiled with JetBrains decompiler
// Type: DiscordRPC.Message.SubscribeMessage
// Assembly: DiscordRPC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B95F11B0-E129-4510-93B3-855D1EDEA68A
// Assembly location: C:\Program Files\BlueStacks\DiscordRPC.dll

using DiscordRPC.RPC.Payload;

namespace DiscordRPC.Message
{
  public class SubscribeMessage : IMessage
  {
    public override MessageType Type
    {
      get
      {
        return MessageType.Subscribe;
      }
    }

    public EventType Event { get; internal set; }

    internal SubscribeMessage(ServerEvent evt)
    {
      switch (evt)
      {
        case ServerEvent.ActivitySpectate:
          this.Event = EventType.Spectate;
          break;
        case ServerEvent.ActivityJoinRequest:
          this.Event = EventType.JoinRequest;
          break;
        default:
          this.Event = EventType.Join;
          break;
      }
    }
  }
}
