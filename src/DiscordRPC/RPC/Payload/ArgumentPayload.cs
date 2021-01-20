// Decompiled with JetBrains decompiler
// Type: DiscordRPC.RPC.Payload.ArgumentPayload
// Assembly: DiscordRPC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B95F11B0-E129-4510-93B3-855D1EDEA68A
// Assembly location: C:\Program Files\BlueStacks\DiscordRPC.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DiscordRPC.RPC.Payload
{
  internal class ArgumentPayload : IPayload
  {
    [JsonProperty("args", NullValueHandling = NullValueHandling.Ignore)]
    public JObject Arguments { get; set; }

    public ArgumentPayload()
    {
      this.Arguments = (JObject) null;
    }

    public ArgumentPayload(long nonce)
      : base(nonce)
    {
      this.Arguments = (JObject) null;
    }

    public ArgumentPayload(object args, long nonce)
      : base(nonce)
    {
      this.SetObject(args);
    }

    public void SetObject(object obj)
    {
      this.Arguments = JObject.FromObject(obj);
    }

    public T GetObject<T>()
    {
      return this.Arguments.ToObject<T>();
    }

    public override string ToString()
    {
      return "Argument " + base.ToString();
    }
  }
}
