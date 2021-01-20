// Decompiled with JetBrains decompiler
// Type: DiscordRPC.IO.ManagedNamedPipeClient
// Assembly: DiscordRPC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B95F11B0-E129-4510-93B3-855D1EDEA68A
// Assembly location: C:\Program Files\BlueStacks\DiscordRPC.dll

using DiscordRPC.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Threading;

namespace DiscordRPC.IO
{
  public class ManagedNamedPipeClient : INamedPipeClient, IDisposable
  {
    private byte[] _buffer = new byte[PipeFrame.MAX_SIZE];
    private Queue<PipeFrame> _framequeue = new Queue<PipeFrame>();
    private object _framequeuelock = new object();
    private volatile bool _isClosed = true;
    private object l_stream = new object();
    private const string PIPE_NAME = "discord-ipc-{0}";
    private int _connectedPipe;
    private NamedPipeClientStream _stream;
    private volatile bool _isDisposed;

    public ILogger Logger { get; set; }

    public bool IsConnected
    {
      get
      {
        if (this._isClosed)
          return false;
        lock (this.l_stream)
          return this._stream != null && this._stream.IsConnected;
      }
    }

    public int ConnectedPipe
    {
      get
      {
        return this._connectedPipe;
      }
    }

    public ManagedNamedPipeClient()
    {
      this._buffer = new byte[PipeFrame.MAX_SIZE];
      this.Logger = (ILogger) new NullLogger();
      this._stream = (NamedPipeClientStream) null;
    }

    public bool Connect(int pipe)
    {
      if (this._isDisposed)
        throw new ObjectDisposedException("NamedPipe");
      if (pipe > 9)
        throw new ArgumentOutOfRangeException(nameof (pipe), "Argument cannot be greater than 9");
      if (pipe >= 0 && this.AttemptConnection(pipe))
      {
        this.tBeginRead();
        return true;
      }
      for (int pipe1 = 0; pipe1 < 10; ++pipe1)
      {
        if (this.AttemptConnection(pipe1))
        {
          this.tBeginRead();
          return true;
        }
      }
      return false;
    }

    private bool AttemptConnection(int pipe)
    {
      if (this._isDisposed)
        throw new ObjectDisposedException("_stream");
      string pipeName = string.Format("discord-ipc-{0}", (object) pipe);
      this.Logger.Info("Attempting to connect to " + pipeName);
      try
      {
        lock (this.l_stream)
        {
          this._stream = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
          this._stream.Connect(1000);
          this.Logger.Info("Waiting for connection...");
          do
          {
            Thread.Sleep(10);
          }
          while (!this._stream.IsConnected);
        }
        this.Logger.Info("Connected to " + pipeName);
        this._connectedPipe = pipe;
        this._isClosed = false;
      }
      catch (Exception ex)
      {
        this.Logger.Error("Failed connection to {0}. {1}", (object) pipeName, (object) ex.Message);
        this.Close();
      }
      return !this._isClosed;
    }

    private void tBeginRead()
    {
      if (this._isClosed)
        return;
      try
      {
        lock (this.l_stream)
        {
          if (this._stream == null || !this._stream.IsConnected)
            return;
          this.Logger.Info("Begining Read of {0} bytes", (object) this._buffer.Length);
          this._stream.BeginRead(this._buffer, 0, this._buffer.Length, new AsyncCallback(this.tEndRead), (object) this._stream.IsConnected);
        }
      }
      catch (ObjectDisposedException ex)
      {
        this.Logger.Warning("Attempted to start reading from a disposed pipe");
      }
      catch (InvalidOperationException ex)
      {
        this.Logger.Warning("Attempted to start reading from a closed pipe");
      }
      catch (Exception ex)
      {
        this.Logger.Error("An exception occured while starting to read a stream: {0}", (object) ex.Message);
        this.Logger.Error(ex.StackTrace);
      }
    }

    private void tEndRead(IAsyncResult callback)
    {
      this.Logger.Info("Ending Read");
      int count = 0;
      try
      {
        lock (this.l_stream)
        {
          if (this._stream == null || !this._stream.IsConnected)
            return;
          count = this._stream.EndRead(callback);
        }
      }
      catch (IOException ex)
      {
        this.Logger.Warning("Attempted to end reading from a closed pipe");
        return;
      }
      catch (NullReferenceException ex)
      {
        this.Logger.Warning("Attempted to read from a null pipe");
        return;
      }
      catch (ObjectDisposedException ex)
      {
        this.Logger.Warning("Attemped to end reading from a disposed pipe");
        return;
      }
      catch (Exception ex)
      {
        this.Logger.Error("An exception occured while ending a read of a stream: {0}", (object) ex.Message);
        this.Logger.Error(ex.StackTrace);
        return;
      }
      this.Logger.Info("Read {0} bytes", (object) count);
      if (count > 0)
      {
        using (MemoryStream memoryStream = new MemoryStream(this._buffer, 0, count))
        {
          try
          {
            PipeFrame pipeFrame = new PipeFrame();
            if (pipeFrame.ReadStream((Stream) memoryStream))
            {
              this.Logger.Info("Read a frame: {0}", (object) pipeFrame.Opcode);
              lock (this._framequeuelock)
                this._framequeue.Enqueue(pipeFrame);
            }
            else
            {
              this.Logger.Error("Pipe failed to read from the data received by the stream.");
              this.Close();
            }
          }
          catch (Exception ex)
          {
            this.Logger.Error("A exception has occured while trying to parse the pipe data: " + ex.Message);
            this.Close();
          }
        }
      }
      if (this._isClosed || !this.IsConnected)
        return;
      this.Logger.Info("Starting another read");
      this.tBeginRead();
    }

    public bool ReadFrame(out PipeFrame frame)
    {
      if (this._isDisposed)
        throw new ObjectDisposedException("_stream");
      lock (this._framequeuelock)
      {
        if (this._framequeue.Count == 0)
        {
          frame = new PipeFrame();
          return false;
        }
        frame = this._framequeue.Dequeue();
        return true;
      }
    }

    public bool WriteFrame(PipeFrame frame)
    {
      if (this._isDisposed)
        throw new ObjectDisposedException("_stream");
      if (!this._isClosed)
      {
        if (this.IsConnected)
        {
          try
          {
            frame.WriteStream((Stream) this._stream);
            return true;
          }
          catch (IOException ex)
          {
            this.Logger.Error("Failed to write frame because of a IO Exception: {0}", (object) ex.Message);
          }
          catch (ObjectDisposedException ex)
          {
            this.Logger.Warning("Failed to write frame as the stream was already disposed");
          }
          catch (InvalidOperationException ex)
          {
            this.Logger.Warning("Failed to write frame because of a invalid operation");
          }
          return false;
        }
      }
      this.Logger.Error("Failed to write frame because the stream is closed");
      return false;
    }

    public void Close()
    {
      if (this._isClosed)
      {
        this.Logger.Warning("Tried to close a already closed pipe.");
      }
      else
      {
        try
        {
          lock (this.l_stream)
          {
            if (this._stream != null)
            {
              try
              {
                this._stream.Flush();
                this._stream.Dispose();
              }
              catch (Exception ex)
              {
              }
              this._stream = (NamedPipeClientStream) null;
              this._isClosed = true;
            }
            else
              this.Logger.Warning("Stream was closed, but no stream was available to begin with!");
          }
        }
        catch (ObjectDisposedException ex)
        {
          this.Logger.Warning("Tried to dispose already disposed stream");
        }
        finally
        {
          this._isClosed = true;
          this._connectedPipe = -1;
        }
      }
    }

    public void Dispose()
    {
      if (this._isDisposed)
        return;
      if (!this._isClosed)
        this.Close();
      lock (this.l_stream)
      {
        if (this._stream != null)
        {
          this._stream.Dispose();
          this._stream = (NamedPipeClientStream) null;
        }
      }
      this._isDisposed = true;
    }
  }
}
