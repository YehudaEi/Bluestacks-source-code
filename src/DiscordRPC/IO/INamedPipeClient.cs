// Decompiled with JetBrains decompiler
// Type: DiscordRPC.IO.INamedPipeClient
// Assembly: DiscordRPC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B95F11B0-E129-4510-93B3-855D1EDEA68A
// Assembly location: C:\Program Files\BlueStacks\DiscordRPC.dll

using DiscordRPC.Logging;
using System;

namespace DiscordRPC.IO
{
  public interface INamedPipeClient : IDisposable
  {
    ILogger Logger { get; set; }

    bool IsConnected { get; }

    int ConnectedPipe { get; }

    bool Connect(int pipe);

    bool ReadFrame(out PipeFrame frame);

    bool WriteFrame(PipeFrame frame);

    void Close();
  }
}
