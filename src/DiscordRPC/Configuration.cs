// Decompiled with JetBrains decompiler
// Type: DiscordRPC.Configuration
// Assembly: DiscordRPC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B95F11B0-E129-4510-93B3-855D1EDEA68A
// Assembly location: C:\Program Files\BlueStacks\DiscordRPC.dll

using Newtonsoft.Json;

namespace DiscordRPC
{
  public class Configuration
  {
    [JsonProperty("api_endpoint")]
    public string ApiEndpoint { get; set; }

    [JsonProperty("cdn_host")]
    public string CdnHost { get; set; }

    [JsonProperty("enviroment")]
    public string Enviroment { get; set; }
  }
}
