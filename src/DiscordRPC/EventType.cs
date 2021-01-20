// Decompiled with JetBrains decompiler
// Type: DiscordRPC.EventType
// Assembly: DiscordRPC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B95F11B0-E129-4510-93B3-855D1EDEA68A
// Assembly location: C:\Program Files\BlueStacks\DiscordRPC.dll

using System;

namespace DiscordRPC
{
  [Flags]
  public enum EventType
  {
    None = 0,
    Spectate = 1,
    Join = 2,
    JoinRequest = 4,
  }
}
