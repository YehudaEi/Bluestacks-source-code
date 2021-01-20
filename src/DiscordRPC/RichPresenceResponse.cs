// Decompiled with JetBrains decompiler
// Type: DiscordRPC.RichPresenceResponse
// Assembly: DiscordRPC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B95F11B0-E129-4510-93B3-855D1EDEA68A
// Assembly location: C:\Program Files\BlueStacks\DiscordRPC.dll

using Newtonsoft.Json;

namespace DiscordRPC
{
  internal class RichPresenceResponse : RichPresence
  {
    [JsonProperty("application_id")]
    public string ClientID { get; private set; }

    [JsonProperty("name")]
    public string Name { get; private set; }
  }
}
