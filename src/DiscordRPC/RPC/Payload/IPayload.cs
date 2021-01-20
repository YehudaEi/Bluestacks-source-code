// Decompiled with JetBrains decompiler
// Type: DiscordRPC.RPC.Payload.IPayload
// Assembly: DiscordRPC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B95F11B0-E129-4510-93B3-855D1EDEA68A
// Assembly location: C:\Program Files\BlueStacks\DiscordRPC.dll

using DiscordRPC.Converters;
using Newtonsoft.Json;

namespace DiscordRPC.RPC.Payload
{
  internal abstract class IPayload
  {
    [JsonProperty("cmd")]
    [JsonConverter(typeof (EnumSnakeCaseConverter))]
    public Command Command { get; set; }

    [JsonProperty("nonce")]
    public string Nonce { get; set; }

    protected IPayload()
    {
    }

    protected IPayload(long nonce)
    {
      this.Nonce = nonce.ToString();
    }

    public override string ToString()
    {
      return "Payload || Command: " + this.Command.ToString() + ", Nonce: " + (this.Nonce != null ? this.Nonce.ToString() : "NULL");
    }
  }
}
