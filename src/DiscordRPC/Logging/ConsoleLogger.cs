// Decompiled with JetBrains decompiler
// Type: DiscordRPC.Logging.ConsoleLogger
// Assembly: DiscordRPC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B95F11B0-E129-4510-93B3-855D1EDEA68A
// Assembly location: C:\Program Files\BlueStacks\DiscordRPC.dll

using System;

namespace DiscordRPC.Logging
{
  public class ConsoleLogger : ILogger
  {
    public LogLevel Level { get; set; }

    public bool Coloured { get; set; }

    public bool Colored
    {
      get
      {
        return this.Coloured;
      }
      set
      {
        this.Coloured = value;
      }
    }

    public void Info(string message, params object[] args)
    {
      if (this.Level != LogLevel.Info)
        return;
      if (this.Coloured)
        Console.ForegroundColor = ConsoleColor.White;
      Console.WriteLine("INFO: " + message, args);
    }

    public void Warning(string message, params object[] args)
    {
      if (this.Level != LogLevel.Info && this.Level != LogLevel.Warning)
        return;
      if (this.Coloured)
        Console.ForegroundColor = ConsoleColor.Yellow;
      Console.WriteLine("WARN: " + message, args);
    }

    public void Error(string message, params object[] args)
    {
      if (this.Level != LogLevel.Info && this.Level != LogLevel.Warning && this.Level != LogLevel.Error)
        return;
      if (this.Coloured)
        Console.ForegroundColor = ConsoleColor.Red;
      Console.WriteLine("ERR : " + message, args);
    }
  }
}
