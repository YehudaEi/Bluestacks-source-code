// Decompiled with JetBrains decompiler
// Type: DiscordRPC.IO.NativeNamedPipeClient
// Assembly: DiscordRPC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B95F11B0-E129-4510-93B3-855D1EDEA68A
// Assembly location: C:\Program Files\BlueStacks\DiscordRPC.dll

using DiscordRPC.Logging;
using System;
using System.IO;

namespace DiscordRPC.IO
{
  public class NativeNamedPipeClient : INamedPipeClient, IDisposable
  {
    private byte[] _buffer = new byte[PipeFrame.MAX_SIZE];
    private const string PIPE_NAME = "\\\\?\\pipe\\discord-ipc-{0}";
    private int _connectedPipe;
    private NativePipe.PipeReadError _lasterr;

    public ILogger Logger { get; set; }

    public bool IsConnected
    {
      get
      {
        return NativePipe.IsConnected();
      }
    }

    public int ConnectedPipe
    {
      get
      {
        return this._connectedPipe;
      }
    }

    internal NativePipe.PipeReadError LastError
    {
      get
      {
        return this._lasterr;
      }
    }

    public bool Connect(int pipe)
    {
      if (this.IsConnected)
      {
        this.Logger.Error("Cannot connect as the pipe is already connected");
        throw new InvalidPipeException("Cannot connect as the pipe is already connected");
      }
      if (pipe > 9)
      {
        this.Logger.Error("Argument cannot be greater than 9");
        throw new ArgumentOutOfRangeException(nameof (pipe), "Argument cannot be greater than 9");
      }
      if (pipe >= 0 && this.AttemptConnection(pipe))
        return true;
      for (int pipe1 = 0; pipe1 < 10; ++pipe1)
      {
        if (this.AttemptConnection(pipe1))
          return true;
      }
      return false;
    }

    private bool AttemptConnection(int pipe)
    {
      if (this.IsConnected)
      {
        this.Logger.Error("Cannot connect as the pipe is already connected");
        throw new InvalidPipeException("Cannot connect as the pipe is already connected");
      }
      string pipename = string.Format("\\\\?\\pipe\\discord-ipc-{0}", (object) pipe);
      this.Logger.Info("Attempting to connect to " + pipename);
      uint num = NativePipe.Open(pipename);
      if (num == 0U && this.IsConnected)
      {
        this.Logger.Info("Succesfully connected to " + pipename);
        this._connectedPipe = pipe;
        return true;
      }
      this.Logger.Error("Failed to connect to native pipe. Err: {0}", (object) num);
      return false;
    }

    public bool ReadFrame(out PipeFrame frame)
    {
      if (!this.IsConnected)
        throw new InvalidPipeException("Cannot read Native Stream as pipe is not connected");
      int count = NativePipe.ReadFrame(this._buffer, this._buffer.Length);
      if (count <= 0)
      {
        this._lasterr = NativePipe.PipeReadError.ReadEmptyMessage;
        if (count < 0)
        {
          this._lasterr = (NativePipe.PipeReadError) count;
          this.Logger.Error("Native pipe failed to read: {0}", (object) this._lasterr.ToString());
          this.Close();
        }
        frame = new PipeFrame();
        return false;
      }
      using (MemoryStream memoryStream = new MemoryStream(this._buffer, 0, count))
      {
        frame = new PipeFrame();
        if (frame.ReadStream((Stream) memoryStream) && frame.Length != 0U)
          return true;
        this.Logger.Error("Pipe failed to read from the data received by the stream.");
        return false;
      }
    }

    public bool WriteFrame(PipeFrame frame)
    {
      if (!this.IsConnected)
        throw new InvalidPipeException("Cannot write Native Stream as pipe is not connected");
      using (MemoryStream memoryStream = new MemoryStream())
      {
        frame.WriteStream((Stream) memoryStream);
        byte[] array = memoryStream.ToArray();
        return NativePipe.WriteFrame(array, array.Length);
      }
    }

    public void Close()
    {
      NativePipe.Close();
    }

    public void Dispose()
    {
      this.Close();
    }
  }
}
