// Decompiled with JetBrains decompiler
// Type: DiscordRPC.Message.ErrorCode
// Assembly: DiscordRPC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B95F11B0-E129-4510-93B3-855D1EDEA68A
// Assembly location: C:\Program Files\BlueStacks\DiscordRPC.dll

namespace DiscordRPC.Message
{
  public enum ErrorCode
  {
    Success = 0,
    PipeException = 1,
    ReadCorrupt = 2,
    NotImplemented = 10, // 0x0000000A
    UnkownError = 1000, // 0x000003E8
    InvalidPayload = 4000, // 0x00000FA0
    InvalidCommand = 4002, // 0x00000FA2
    InvalidEvent = 4004, // 0x00000FA4
  }
}
