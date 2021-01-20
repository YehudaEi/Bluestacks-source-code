// Decompiled with JetBrains decompiler
// Type: DiscordRPC.Logging.NullLogger
// Assembly: DiscordRPC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B95F11B0-E129-4510-93B3-855D1EDEA68A
// Assembly location: C:\Program Files\BlueStacks\DiscordRPC.dll

namespace DiscordRPC.Logging
{
  public class NullLogger : ILogger
  {
    public LogLevel Level { get; set; }

    public void Info(string message, params object[] args)
    {
    }

    public void Warning(string message, params object[] args)
    {
    }

    public void Error(string message, params object[] args)
    {
    }
  }
}
