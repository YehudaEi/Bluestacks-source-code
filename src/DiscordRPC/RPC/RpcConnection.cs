// Decompiled with JetBrains decompiler
// Type: DiscordRPC.RPC.RpcConnection
// Assembly: DiscordRPC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B95F11B0-E129-4510-93B3-855D1EDEA68A
// Assembly location: C:\Program Files\BlueStacks\DiscordRPC.dll

using DiscordRPC.Converters;
using DiscordRPC.Helper;
using DiscordRPC.IO;
using DiscordRPC.Logging;
using DiscordRPC.Message;
using DiscordRPC.RPC.Commands;
using DiscordRPC.RPC.Payload;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;

namespace DiscordRPC.RPC
{
  public class RpcConnection : IDisposable
  {
    public static readonly int VERSION = 1;
    public static readonly int POLL_RATE = 1000;
    private static readonly bool CLEAR_ON_SHUTDOWN = true;
    private static readonly bool LOCK_STEP = false;
    private readonly object l_states = new object();
    private readonly object l_config = new object();
    private readonly object l_rtqueue = new object();
    private readonly object l_rxqueue = new object();
    private AutoResetEvent queueUpdatedEvent = new AutoResetEvent(false);
    private ILogger _logger;
    private RpcState _state;
    private Configuration _configuration;
    private volatile bool aborting;
    private volatile bool shutdown;
    private string applicationID;
    private int processID;
    private long nonce;
    private Thread thread;
    private INamedPipeClient namedPipe;
    private int targetPipe;
    private Queue<ICommand> _rtqueue;
    private Queue<IMessage> _rxqueue;
    private BackoffDelay delay;

    public ILogger Logger
    {
      get
      {
        return this._logger;
      }
      set
      {
        this._logger = value;
        if (this.namedPipe == null)
          return;
        this.namedPipe.Logger = value;
      }
    }

    public RpcState State
    {
      get
      {
        lock (this.l_states)
          return this._state;
      }
    }

    public Configuration Configuration
    {
      get
      {
        lock (this.l_config)
          return this._configuration;
      }
    }

    public bool IsRunning
    {
      get
      {
        return this.thread != null;
      }
    }

    public bool ShutdownOnly { get; set; }

    public RpcConnection(
      string applicationID,
      int processID,
      int targetPipe,
      INamedPipeClient client)
    {
      this.applicationID = applicationID;
      this.processID = processID;
      this.targetPipe = targetPipe;
      this.namedPipe = client;
      this.ShutdownOnly = true;
      this.Logger = (ILogger) new ConsoleLogger();
      this.delay = new BackoffDelay(500, 60000);
      this._rtqueue = new Queue<ICommand>();
      this._rxqueue = new Queue<IMessage>();
      this.nonce = 0L;
    }

    private long GetNextNonce()
    {
      ++this.nonce;
      return this.nonce;
    }

    internal void EnqueueCommand(ICommand command)
    {
      if (this.aborting || this.shutdown)
        return;
      lock (this.l_rtqueue)
        this._rtqueue.Enqueue(command);
    }

    private void EnqueueMessage(IMessage message)
    {
      lock (this.l_rxqueue)
        this._rxqueue.Enqueue(message);
    }

    internal IMessage DequeueMessage()
    {
      lock (this.l_rxqueue)
        return this._rxqueue.Count == 0 ? (IMessage) null : this._rxqueue.Dequeue();
    }

    internal IMessage[] DequeueMessages()
    {
      lock (this.l_rxqueue)
      {
        IMessage[] array = this._rxqueue.ToArray();
        this._rxqueue.Clear();
        return array;
      }
    }

    private void MainLoop()
    {
      this.Logger.Info("Initializing Thread. Creating pipe object.");
      while (!this.aborting)
      {
        if (!this.shutdown)
        {
          try
          {
            if (this.namedPipe == null)
            {
              this.Logger.Error("Something bad has happened with our pipe client!");
              this.aborting = true;
              return;
            }
            this.Logger.Info("Connecting to the pipe through the {0}", (object) this.namedPipe.GetType().FullName);
            if (this.namedPipe.Connect(this.targetPipe))
            {
              this.Logger.Info("Connected to the pipe. Attempting to establish handshake...");
              this.EnqueueMessage((IMessage) new ConnectionEstablishedMessage()
              {
                ConnectedPipe = this.namedPipe.ConnectedPipe
              });
              this.EstablishHandshake();
              this.Logger.Info("Connection Established. Starting reading loop...");
              bool flag = true;
              while (flag && !this.aborting && (!this.shutdown && this.namedPipe.IsConnected))
              {
                PipeFrame frame;
                if (this.namedPipe.ReadFrame(out frame))
                {
                  this.Logger.Info("Read Payload: {0}", (object) frame.Opcode);
                  switch (frame.Opcode)
                  {
                    case Opcode.Frame:
                      if (this.shutdown)
                      {
                        this.Logger.Warning("Skipping frame because we are shutting down.");
                        break;
                      }
                      if (frame.Data == null)
                      {
                        this.Logger.Error("We received no data from the frame so we cannot get the event payload!");
                        break;
                      }
                      EventPayload response = (EventPayload) null;
                      try
                      {
                        response = frame.GetObject<EventPayload>();
                      }
                      catch (Exception ex)
                      {
                        this.Logger.Error("Failed to parse event! " + ex.Message);
                        this.Logger.Error("Data: " + frame.Message);
                      }
                      if (response != null)
                      {
                        this.ProcessFrame(response);
                        break;
                      }
                      break;
                    case Opcode.Close:
                      ClosePayload closePayload = frame.GetObject<ClosePayload>();
                      this.Logger.Warning("We have been told to terminate by discord: ({0}) {1}", (object) closePayload.Code, (object) closePayload.Reason);
                      this.EnqueueMessage((IMessage) new CloseMessage()
                      {
                        Code = closePayload.Code,
                        Reason = closePayload.Reason
                      });
                      flag = false;
                      break;
                    case Opcode.Ping:
                      this.Logger.Info("PING");
                      frame.Opcode = Opcode.Pong;
                      this.namedPipe.WriteFrame(frame);
                      break;
                    case Opcode.Pong:
                      this.Logger.Info("PONG");
                      break;
                    default:
                      this.Logger.Error("Invalid opcode: {0}", (object) frame.Opcode);
                      flag = false;
                      break;
                  }
                }
                if (!this.aborting && this.namedPipe.IsConnected)
                {
                  this.ProcessCommandQueue();
                  this.queueUpdatedEvent.WaitOne(RpcConnection.POLL_RATE);
                }
              }
              this.Logger.Info("Left main read loop for some reason. Aborting: {0}, Shutting Down: {1}", (object) this.aborting, (object) this.shutdown);
            }
            else
            {
              this.Logger.Error("Failed to connect for some reason.");
              this.EnqueueMessage((IMessage) new ConnectionFailedMessage()
              {
                FailedPipe = this.targetPipe
              });
            }
            if (!this.aborting)
            {
              if (!this.shutdown)
              {
                this.Logger.Info("Waiting {0}ms before attempting to connect again", (object) (long) this.delay.NextDelay());
                Thread.Sleep(this.delay.NextDelay());
              }
            }
          }
          catch (InvalidPipeException ex)
          {
            this.Logger.Error("Invalid Pipe Exception: {0}", (object) ex.Message);
          }
          catch (Exception ex)
          {
            this.Logger.Error("Unhandled Exception: {0}", (object) ex.GetType().FullName);
            this.Logger.Error(ex.Message);
            this.Logger.Error(ex.StackTrace);
          }
          finally
          {
            if (this.namedPipe.IsConnected)
            {
              this.Logger.Info("Closing the named pipe.");
              this.namedPipe.Close();
            }
            this.SetConnectionState(RpcState.Disconnected);
          }
        }
        else
          break;
      }
      this.Logger.Info("Left Main Loop");
      if (this.namedPipe != null)
        this.namedPipe.Dispose();
      this.Logger.Info("Thread Terminated, no longer performing RPC connection.");
    }

    private void ProcessFrame(EventPayload response)
    {
      this.Logger.Info("Handling Response. Cmd: {0}, Event: {1}", (object) response.Command, (object) response.Event);
      ServerEvent? nullable;
      if (response.Event.HasValue)
      {
        nullable = response.Event;
        if (nullable.Value == ServerEvent.Error)
        {
          this.Logger.Error("Error received from the RPC");
          ErrorMessage errorMessage = response.GetObject<ErrorMessage>();
          this.Logger.Error("Server responded with an error message: ({0}) {1}", (object) errorMessage.Code.ToString(), (object) errorMessage.Message);
          this.EnqueueMessage((IMessage) errorMessage);
          return;
        }
      }
      if (this.State == RpcState.Connecting && response.Command == Command.Dispatch)
      {
        nullable = response.Event;
        if (nullable.HasValue)
        {
          nullable = response.Event;
          if (nullable.Value == ServerEvent.Ready)
          {
            this.Logger.Info("Connection established with the RPC");
            this.SetConnectionState(RpcState.Connected);
            this.delay.Reset();
            ReadyMessage readyMessage = response.GetObject<ReadyMessage>();
            lock (this.l_config)
            {
              this._configuration = readyMessage.Configuration;
              readyMessage.User.SetConfiguration(this._configuration);
            }
            this.EnqueueMessage((IMessage) readyMessage);
            return;
          }
        }
      }
      if (this.State == RpcState.Connected)
      {
        switch (response.Command)
        {
          case Command.Dispatch:
            this.ProcessDispatch(response);
            break;
          case Command.SetActivity:
            if (response.Data == null)
            {
              this.EnqueueMessage((IMessage) new PresenceMessage());
              break;
            }
            this.EnqueueMessage((IMessage) new PresenceMessage(response.GetObject<RichPresenceResponse>()));
            break;
          case Command.Subscribe:
          case Command.Unsubscribe:
            new JsonSerializer().Converters.Add((JsonConverter) new EnumSnakeCaseConverter());
            nullable = response.GetObject<EventPayload>().Event;
            ServerEvent evt = nullable.Value;
            if (response.Command == Command.Subscribe)
            {
              this.EnqueueMessage((IMessage) new SubscribeMessage(evt));
              break;
            }
            this.EnqueueMessage((IMessage) new UnsubscribeMessage(evt));
            break;
          case Command.SendActivityJoinInvite:
            this.Logger.Info("Got invite response ack.");
            break;
          case Command.CloseActivityJoinRequest:
            this.Logger.Info("Got invite response reject ack.");
            break;
          default:
            this.Logger.Error("Unkown frame was received! {0}", (object) response.Command);
            break;
        }
      }
      else
        this.Logger.Info("Received a frame while we are disconnected. Ignoring. Cmd: {0}, Event: {1}", (object) response.Command, (object) response.Event);
    }

    private void ProcessDispatch(EventPayload response)
    {
      if (response.Command != Command.Dispatch)
        return;
      ServerEvent? nullable = response.Event;
      if (!nullable.HasValue)
        return;
      nullable = response.Event;
      switch (nullable.Value)
      {
        case ServerEvent.ActivityJoin:
          this.EnqueueMessage((IMessage) response.GetObject<JoinMessage>());
          break;
        case ServerEvent.ActivitySpectate:
          this.EnqueueMessage((IMessage) response.GetObject<SpectateMessage>());
          break;
        case ServerEvent.ActivityJoinRequest:
          this.EnqueueMessage((IMessage) response.GetObject<JoinRequestMessage>());
          break;
        default:
          ILogger logger = this.Logger;
          object[] objArray = new object[1];
          nullable = response.Event;
          objArray[0] = (object) nullable.Value;
          logger.Warning("Ignoring {0}", objArray);
          break;
      }
    }

    private void ProcessCommandQueue()
    {
      if (this.State != RpcState.Connected)
        return;
      if (this.aborting)
        this.Logger.Warning("We have been told to write a queue but we have also been aborted.");
      bool flag = true;
      ICommand command = (ICommand) null;
      while (flag && this.namedPipe.IsConnected)
      {
        lock (this.l_rtqueue)
        {
          flag = this._rtqueue.Count > 0;
          if (!flag)
            break;
          command = this._rtqueue.Peek();
        }
        if (this.shutdown || !this.aborting && RpcConnection.LOCK_STEP)
          flag = false;
        IPayload ipayload = command.PreparePayload(this.GetNextNonce());
        this.Logger.Info("Attempting to send payload: " + (object) ipayload.Command);
        PipeFrame frame = new PipeFrame();
        if (command is CloseCommand)
        {
          this.SendHandwave();
          this.Logger.Info("Handwave sent, ending queue processing.");
          lock (this.l_rtqueue)
          {
            this._rtqueue.Dequeue();
            break;
          }
        }
        else if (this.aborting)
        {
          this.Logger.Warning("- skipping frame because of abort.");
          lock (this.l_rtqueue)
            this._rtqueue.Dequeue();
        }
        else
        {
          frame.SetObject(Opcode.Frame, (object) command.PreparePayload(this.GetNextNonce()));
          this.Logger.Info("Sending payload: " + (object) ipayload.Command);
          if (this.namedPipe.WriteFrame(frame))
          {
            this.Logger.Info("Sent Successfully.");
            lock (this.l_rtqueue)
              this._rtqueue.Dequeue();
          }
          else
          {
            this.Logger.Warning("Something went wrong during writing!");
            break;
          }
        }
      }
    }

    private void EstablishHandshake()
    {
      this.Logger.Info("Attempting to establish a handshake...");
      if (this.State != RpcState.Disconnected)
      {
        this.Logger.Error("State must be disconnected in order to start a handshake!");
      }
      else
      {
        this.Logger.Info("Sending Handshake...");
        if (!this.namedPipe.WriteFrame(new PipeFrame(Opcode.Handshake, (object) new Handshake()
        {
          Version = RpcConnection.VERSION,
          ClientID = this.applicationID
        })))
          this.Logger.Error("Failed to write a handshake.");
        else
          this.SetConnectionState(RpcState.Connecting);
      }
    }

    private void SendHandwave()
    {
      this.Logger.Info("Attempting to wave goodbye...");
      if (this.State == RpcState.Disconnected)
        this.Logger.Error("State must NOT be disconnected in order to send a handwave!");
      else if (!this.namedPipe.WriteFrame(new PipeFrame(Opcode.Close, (object) new Handshake()
      {
        Version = RpcConnection.VERSION,
        ClientID = this.applicationID
      })))
        this.Logger.Error("failed to write a handwave.");
      else
        this.Logger.Info("Goodbye sent.");
    }

    public bool AttemptConnection()
    {
      this.Logger.Info("Attempting a new connection");
      if (this.thread != null)
      {
        this.Logger.Error("Cannot attempt a new connection as the previous connection thread is not null!");
        return false;
      }
      if (this.State != RpcState.Disconnected)
      {
        this.Logger.Warning("Cannot attempt a new connection as the previous connection hasn't changed state yet.");
        return false;
      }
      if (this.aborting)
      {
        this.Logger.Error("Cannot attempt a new connection while aborting!");
        return false;
      }
      this.thread = new Thread(new ThreadStart(this.MainLoop));
      this.thread.Name = "Discord IPC Thread";
      this.thread.IsBackground = true;
      this.thread.Start();
      return true;
    }

    private void SetConnectionState(RpcState state)
    {
      this.Logger.Info("Setting the connection state to {0}", (object) state.ToString().ToSnakeCase().ToUpperInvariant());
      lock (this.l_states)
        this._state = state;
    }

    public void Shutdown()
    {
      this.Logger.Info("Initiated shutdown procedure");
      this.shutdown = true;
      lock (this.l_rtqueue)
      {
        this._rtqueue.Clear();
        if (RpcConnection.CLEAR_ON_SHUTDOWN)
          this._rtqueue.Enqueue((ICommand) new PresenceCommand()
          {
            PID = this.processID,
            Presence = (RichPresence) null
          });
        this._rtqueue.Enqueue((ICommand) new CloseCommand());
      }
      this.queueUpdatedEvent.Set();
    }

    public void Close()
    {
      if (this.thread == null)
        this.Logger.Error("Cannot close as it is not available!");
      else if (this.aborting)
        this.Logger.Error("Cannot abort as it has already been aborted");
      else if (this.ShutdownOnly)
      {
        this.Shutdown();
      }
      else
      {
        this.Logger.Info("Updating Abort State...");
        this.aborting = true;
        this.queueUpdatedEvent.Set();
      }
    }

    public void Dispose()
    {
      this.ShutdownOnly = false;
      this.Close();
    }
  }
}
