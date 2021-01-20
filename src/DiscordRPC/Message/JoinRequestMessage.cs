// Decompiled with JetBrains decompiler
// Type: DiscordRPC.Message.JoinRequestMessage
// Assembly: DiscordRPC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B95F11B0-E129-4510-93B3-855D1EDEA68A
// Assembly location: C:\Program Files\BlueStacks\DiscordRPC.dll

using Newtonsoft.Json;

namespace DiscordRPC.Message
{
  public class JoinRequestMessage : IMessage
  {
    public override MessageType Type
    {
      get
      {
        return MessageType.JoinRequest;
      }
    }

    [JsonProperty("user")]
    public User User { get; internal set; }
  }
}
