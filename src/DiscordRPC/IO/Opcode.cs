﻿// Decompiled with JetBrains decompiler
// Type: DiscordRPC.IO.Opcode
// Assembly: DiscordRPC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B95F11B0-E129-4510-93B3-855D1EDEA68A
// Assembly location: C:\Program Files\BlueStacks\DiscordRPC.dll

namespace DiscordRPC.IO
{
  public enum Opcode : uint
  {
    Handshake,
    Frame,
    Close,
    Ping,
    Pong,
  }
}
