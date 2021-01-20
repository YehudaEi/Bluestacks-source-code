// Decompiled with JetBrains decompiler
// Type: DiscordRPC.RPC.Commands.CloseCommand
// Assembly: DiscordRPC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B95F11B0-E129-4510-93B3-855D1EDEA68A
// Assembly location: C:\Program Files\BlueStacks\DiscordRPC.dll

using DiscordRPC.RPC.Payload;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DiscordRPC.RPC.Commands
{
  internal class CloseCommand : ICommand
  {
    [JsonProperty("close_reason")]
    public string value = "Unity 5.5 doesn't handle thread aborts. Can you please close me discord?";

    [JsonProperty("pid")]
    public int PID { get; set; }

    public IPayload PreparePayload(long nonce)
    {
      ArgumentPayload argumentPayload = new ArgumentPayload();
      argumentPayload.Command = Command.Dispatch;
      argumentPayload.Nonce = (string) null;
      argumentPayload.Arguments = (JObject) null;
      return (IPayload) argumentPayload;
    }
  }
}
