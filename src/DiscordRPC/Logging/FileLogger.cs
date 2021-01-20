// Decompiled with JetBrains decompiler
// Type: DiscordRPC.Logging.FileLogger
// Assembly: DiscordRPC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B95F11B0-E129-4510-93B3-855D1EDEA68A
// Assembly location: C:\Program Files\BlueStacks\DiscordRPC.dll

namespace DiscordRPC.Logging
{
  public class FileLogger : ILogger
  {
    private object filelock;

    public LogLevel Level { get; set; }

    public string File { get; set; }

    public FileLogger(string path)
    {
      this.File = path;
      this.filelock = new object();
    }

    public void Info(string message, params object[] args)
    {
      if (this.Level != LogLevel.Info)
        return;
      lock (this.filelock)
        System.IO.File.AppendAllText(this.File, "\r\nINFO: " + (args.Length != 0 ? string.Format(message, args) : message));
    }

    public void Warning(string message, params object[] args)
    {
      if (this.Level != LogLevel.Info && this.Level != LogLevel.Warning)
        return;
      lock (this.filelock)
        System.IO.File.AppendAllText(this.File, "\r\nWARN: " + (args.Length != 0 ? string.Format(message, args) : message));
    }

    public void Error(string message, params object[] args)
    {
      if (this.Level != LogLevel.Info && this.Level != LogLevel.Warning && this.Level != LogLevel.Error)
        return;
      lock (this.filelock)
        System.IO.File.AppendAllText(this.File, "\r\nERR : " + (args.Length != 0 ? string.Format(message, args) : message));
    }
  }
}
