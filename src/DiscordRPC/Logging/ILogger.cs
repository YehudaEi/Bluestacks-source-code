﻿// Decompiled with JetBrains decompiler
// Type: DiscordRPC.Logging.ILogger
// Assembly: DiscordRPC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B95F11B0-E129-4510-93B3-855D1EDEA68A
// Assembly location: C:\Program Files\BlueStacks\DiscordRPC.dll

namespace DiscordRPC.Logging
{
  public interface ILogger
  {
    LogLevel Level { get; set; }

    void Info(string message, params object[] args);

    void Warning(string message, params object[] args);

    void Error(string message, params object[] args);
  }
}
