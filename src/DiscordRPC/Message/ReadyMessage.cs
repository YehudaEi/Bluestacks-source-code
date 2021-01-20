// Decompiled with JetBrains decompiler
// Type: DiscordRPC.Message.ReadyMessage
// Assembly: DiscordRPC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B95F11B0-E129-4510-93B3-855D1EDEA68A
// Assembly location: C:\Program Files\BlueStacks\DiscordRPC.dll

using Newtonsoft.Json;

namespace DiscordRPC.Message
{
  public class ReadyMessage : IMessage
  {
    public override MessageType Type
    {
      get
      {
        return MessageType.Ready;
      }
    }

    [JsonProperty("config")]
    public Configuration Configuration { get; set; }

    [JsonProperty("user")]
    public User User { get; set; }

    [JsonProperty("v")]
    public int Version { get; set; }
  }
}
