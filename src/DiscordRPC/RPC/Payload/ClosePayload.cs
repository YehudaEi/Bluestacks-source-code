// Decompiled with JetBrains decompiler
// Type: DiscordRPC.RPC.Payload.ClosePayload
// Assembly: DiscordRPC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B95F11B0-E129-4510-93B3-855D1EDEA68A
// Assembly location: C:\Program Files\BlueStacks\DiscordRPC.dll

using Newtonsoft.Json;

namespace DiscordRPC.RPC.Payload
{
  internal class ClosePayload : IPayload
  {
    [JsonProperty("code")]
    public int Code { get; set; }

    [JsonProperty("message")]
    public string Reason { get; set; }
  }
}
