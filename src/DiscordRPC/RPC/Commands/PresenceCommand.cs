// Decompiled with JetBrains decompiler
// Type: DiscordRPC.RPC.Commands.PresenceCommand
// Assembly: DiscordRPC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B95F11B0-E129-4510-93B3-855D1EDEA68A
// Assembly location: C:\Program Files\BlueStacks\DiscordRPC.dll

using DiscordRPC.RPC.Payload;
using Newtonsoft.Json;

namespace DiscordRPC.RPC.Commands
{
  internal class PresenceCommand : ICommand
  {
    [JsonProperty("pid")]
    public int PID { get; set; }

    [JsonProperty("activity")]
    public RichPresence Presence { get; set; }

    public IPayload PreparePayload(long nonce)
    {
      ArgumentPayload argumentPayload = new ArgumentPayload((object) this, nonce);
      argumentPayload.Command = Command.SetActivity;
      return (IPayload) argumentPayload;
    }
  }
}
