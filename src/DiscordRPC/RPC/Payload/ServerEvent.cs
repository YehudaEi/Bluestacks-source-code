// Decompiled with JetBrains decompiler
// Type: DiscordRPC.RPC.Payload.ServerEvent
// Assembly: DiscordRPC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B95F11B0-E129-4510-93B3-855D1EDEA68A
// Assembly location: C:\Program Files\BlueStacks\DiscordRPC.dll

using DiscordRPC.Converters;

namespace DiscordRPC.RPC.Payload
{
  public enum ServerEvent
  {
    [EnumValue("READY")] Ready,
    [EnumValue("ERROR")] Error,
    [EnumValue("ACTIVITY_JOIN")] ActivityJoin,
    [EnumValue("ACTIVITY_SPECTATE")] ActivitySpectate,
    [EnumValue("ACTIVITY_JOIN_REQUEST")] ActivityJoinRequest,
  }
}
