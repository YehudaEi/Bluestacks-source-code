// Decompiled with JetBrains decompiler
// Type: DiscordRPC.RPC.Payload.EventPayload
// Assembly: DiscordRPC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B95F11B0-E129-4510-93B3-855D1EDEA68A
// Assembly location: C:\Program Files\BlueStacks\DiscordRPC.dll

using DiscordRPC.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DiscordRPC.RPC.Payload
{
  internal class EventPayload : IPayload
  {
    [JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
    public JObject Data { get; set; }

    [JsonProperty("evt")]
    [JsonConverter(typeof (EnumSnakeCaseConverter))]
    public ServerEvent? Event { get; set; }

    public EventPayload()
    {
      this.Data = (JObject) null;
    }

    public EventPayload(long nonce)
      : base(nonce)
    {
      this.Data = (JObject) null;
    }

    public T GetObject<T>()
    {
      return this.Data == null ? default (T) : this.Data.ToObject<T>();
    }

    public override string ToString()
    {
      return "Event " + base.ToString() + ", Event: " + (this.Event.HasValue ? this.Event.ToString() : "N/A");
    }
  }
}
