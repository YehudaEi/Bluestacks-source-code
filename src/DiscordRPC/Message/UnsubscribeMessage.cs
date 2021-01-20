// Decompiled with JetBrains decompiler
// Type: DiscordRPC.Message.UnsubscribeMessage
// Assembly: DiscordRPC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B95F11B0-E129-4510-93B3-855D1EDEA68A
// Assembly location: C:\Program Files\BlueStacks\DiscordRPC.dll

using DiscordRPC.RPC.Payload;

namespace DiscordRPC.Message
{
  public class UnsubscribeMessage : IMessage
  {
    public override MessageType Type
    {
      get
      {
        return MessageType.Unsubscribe;
      }
    }

    public EventType Event { get; internal set; }

    internal UnsubscribeMessage(ServerEvent evt)
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
