// Decompiled with JetBrains decompiler
// Type: DiscordRPC.IO.Handshake
// Assembly: DiscordRPC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B95F11B0-E129-4510-93B3-855D1EDEA68A
// Assembly location: C:\Program Files\BlueStacks\DiscordRPC.dll

using Newtonsoft.Json;

namespace DiscordRPC.IO
{
  internal class Handshake
  {
    [JsonProperty("v")]
    public int Version { get; set; }

    [JsonProperty("client_id")]
    public string ClientID { get; set; }
  }
}
