// Decompiled with JetBrains decompiler
// Type: DiscordRPC.RPC.Commands.SubscribeCommand
// Assembly: DiscordRPC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B95F11B0-E129-4510-93B3-855D1EDEA68A
// Assembly location: C:\Program Files\BlueStacks\DiscordRPC.dll

using DiscordRPC.RPC.Payload;

namespace DiscordRPC.RPC.Commands
{
  internal class SubscribeCommand : ICommand
  {
    public ServerEvent Event { get; set; }

    public bool IsUnsubscribe { get; set; }

    public IPayload PreparePayload(long nonce)
    {
      EventPayload eventPayload = new EventPayload(nonce);
      eventPayload.Command = this.IsUnsubscribe ? Command.Unsubscribe : Command.Subscribe;
      eventPayload.Event = new ServerEvent?(this.Event);
      return (IPayload) eventPayload;
    }
  }
}
