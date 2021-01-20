// Decompiled with JetBrains decompiler
// Type: DiscordRPC.RPC.Payload.Command
// Assembly: DiscordRPC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B95F11B0-E129-4510-93B3-855D1EDEA68A
// Assembly location: C:\Program Files\BlueStacks\DiscordRPC.dll

using DiscordRPC.Converters;
using System;

namespace DiscordRPC.RPC.Payload
{
  public enum Command
  {
    [EnumValue("DISPATCH")] Dispatch,
    [EnumValue("SET_ACTIVITY")] SetActivity,
    [EnumValue("SUBSCRIBE")] Subscribe,
    [EnumValue("UNSUBSCRIBE")] Unsubscribe,
    [EnumValue("SEND_ACTIVITY_JOIN_INVITE")] SendActivityJoinInvite,
    [EnumValue("CLOSE_ACTIVITY_JOIN_REQUEST")] CloseActivityJoinRequest,
    [Obsolete("This value is appart of the RPC API and is not supported by this library.", true)] Authorize,
    [Obsolete("This value is appart of the RPC API and is not supported by this library.", true)] Authenticate,
    [Obsolete("This value is appart of the RPC API and is not supported by this library.", true)] GetGuild,
    [Obsolete("This value is appart of the RPC API and is not supported by this library.", true)] GetGuilds,
    [Obsolete("This value is appart of the RPC API and is not supported by this library.", true)] GetChannel,
    [Obsolete("This value is appart of the RPC API and is not supported by this library.", true)] GetChannels,
    [Obsolete("This value is appart of the RPC API and is not supported by this library.", true)] SetUserVoiceSettings,
    [Obsolete("This value is appart of the RPC API and is not supported by this library.", true)] SelectVoiceChannel,
    [Obsolete("This value is appart of the RPC API and is not supported by this library.", true)] GetSelectedVoiceChannel,
    [Obsolete("This value is appart of the RPC API and is not supported by this library.", true)] SelectTextChannel,
    [Obsolete("This value is appart of the RPC API and is not supported by this library.", true)] GetVoiceSettings,
    [Obsolete("This value is appart of the RPC API and is not supported by this library.", true)] SetVoiceSettings,
    [Obsolete("This value is appart of the RPC API and is not supported by this library.", true)] CaptureShortcut,
  }
}
