// Decompiled with JetBrains decompiler
// Type: DiscordRPC.RPC.Commands.ICommand
// Assembly: DiscordRPC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B95F11B0-E129-4510-93B3-855D1EDEA68A
// Assembly location: C:\Program Files\BlueStacks\DiscordRPC.dll

using DiscordRPC.RPC.Payload;

namespace DiscordRPC.RPC.Commands
{
  internal interface ICommand
  {
    IPayload PreparePayload(long nonce);
  }
}
