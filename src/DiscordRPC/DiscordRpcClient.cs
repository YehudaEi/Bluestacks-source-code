// Decompiled with JetBrains decompiler
// Type: DiscordRPC.DiscordRpcClient
// Assembly: DiscordRPC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B95F11B0-E129-4510-93B3-855D1EDEA68A
// Assembly location: C:\Program Files\BlueStacks\DiscordRPC.dll

using DiscordRPC.Events;
using DiscordRPC.Exceptions;
using DiscordRPC.IO;
using DiscordRPC.Logging;
using DiscordRPC.Message;
using DiscordRPC.Registry;
using DiscordRPC.RPC;
using DiscordRPC.RPC.Commands;
using DiscordRPC.RPC.Payload;
using System;
using System.Diagnostics;

namespace DiscordRPC
{
  public class DiscordRpcClient : IDisposable
  {
    private ILogger _logger = (ILogger) new NullLogger();
    private int _pipe = -1;
    private bool _shutdownOnly = true;
    private bool _disposed;
    private RpcConnection connection;
    private RichPresence _presence;
    private EventType _subscription;
    private User _user;
    private Configuration _configuration;
    private bool _initialized;

    public bool HasRegisteredUriScheme { get; private set; }

    public string ApplicationID { get; private set; }

    public string SteamID { get; private set; }

    public int ProcessID { get; private set; }

    public bool Disposed
    {
      get
      {
        return this._disposed;
      }
    }

    public ILogger Logger
    {
      get
      {
        return this._logger;
      }
      set
      {
        this._logger = value;
        if (this.connection == null)
          return;
        this.connection.Logger = value;
      }
    }

    public int TargetPipe
    {
      get
      {
        return this._pipe;
      }
    }

    public RichPresence CurrentPresence
    {
      get
      {
        return this._presence;
      }
    }

    public EventType Subscription
    {
      get
      {
        return this._subscription;
      }
    }

    public User CurrentUser
    {
      get
      {
        return this._user;
      }
    }

    public Configuration Configuration
    {
      get
      {
        return this._configuration;
      }
    }

    public bool IsInitialized
    {
      get
      {
        return this._initialized;
      }
    }

    public bool ShutdownOnly
    {
      get
      {
        return this._shutdownOnly;
      }
      set
      {
        this._shutdownOnly = value;
        if (this.connection == null)
          return;
        this.connection.ShutdownOnly = value;
      }
    }

    public event OnReadyEvent OnReady;

    public event OnCloseEvent OnClose;

    public event OnErrorEvent OnError;

    public event OnPresenceUpdateEvent OnPresenceUpdate;

    public event OnSubscribeEvent OnSubscribe;

    public event OnUnsubscribeEvent OnUnsubscribe;

    public event OnJoinEvent OnJoin;

    public event OnSpectateEvent OnSpectate;

    public event OnJoinRequestedEvent OnJoinRequested;

    public event OnConnectionEstablishedEvent OnConnectionEstablished;

    public event OnConnectionFailedEvent OnConnectionFailed;

    public DiscordRpcClient(string applicationID)
      : this(applicationID, -1)
    {
    }

    public DiscordRpcClient(string applicationID, int pipe)
      : this(applicationID, (string) null, false, pipe)
    {
    }

    public DiscordRpcClient(string applicationID, bool registerUriScheme)
      : this(applicationID, registerUriScheme, -1)
    {
    }

    public DiscordRpcClient(string applicationID, bool registerUriScheme, int pipe)
      : this(applicationID, (string) null, registerUriScheme, pipe)
    {
    }

    public DiscordRpcClient(string applicationID, string steamID, bool registerUriScheme)
      : this(applicationID, steamID, registerUriScheme, -1)
    {
    }

    public DiscordRpcClient(
      string applicationID,
      string steamID,
      bool registerUriScheme,
      int pipe)
      : this(applicationID, steamID, registerUriScheme, pipe, (INamedPipeClient) new ManagedNamedPipeClient())
    {
    }

    public DiscordRpcClient(
      string applicationID,
      string steamID,
      bool registerUriScheme,
      int pipe,
      INamedPipeClient client)
    {
      this.ApplicationID = applicationID;
      this.SteamID = steamID;
      this.HasRegisteredUriScheme = registerUriScheme;
      this.ProcessID = Process.GetCurrentProcess().Id;
      this._pipe = pipe;
      if (registerUriScheme)
        UriScheme.RegisterUriScheme(applicationID, steamID, (string) null);
      this.connection = new RpcConnection(this.ApplicationID, this.ProcessID, this.TargetPipe, client)
      {
        ShutdownOnly = this._shutdownOnly
      };
      this.connection.Logger = this._logger;
    }

    public IMessage[] Invoke()
    {
      IMessage[] imessageArray = this.connection.DequeueMessages();
      for (int index = 0; index < imessageArray.Length; ++index)
      {
        IMessage message = imessageArray[index];
        this.HandleMessage(message);
        switch (message.Type)
        {
          case MessageType.Ready:
            if (this.OnReady != null)
            {
              this.OnReady((object) this, message as ReadyMessage);
              break;
            }
            break;
          case MessageType.Close:
            if (this.OnClose != null)
            {
              this.OnClose((object) this, message as CloseMessage);
              break;
            }
            break;
          case MessageType.Error:
            if (this.OnError != null)
            {
              this.OnError((object) this, message as ErrorMessage);
              break;
            }
            break;
          case MessageType.PresenceUpdate:
            if (this.OnPresenceUpdate != null)
            {
              this.OnPresenceUpdate((object) this, message as PresenceMessage);
              break;
            }
            break;
          case MessageType.Subscribe:
            if (this.OnSubscribe != null)
            {
              this.OnSubscribe((object) this, message as SubscribeMessage);
              break;
            }
            break;
          case MessageType.Unsubscribe:
            if (this.OnUnsubscribe != null)
            {
              this.OnUnsubscribe((object) this, message as UnsubscribeMessage);
              break;
            }
            break;
          case MessageType.Join:
            if (this.OnJoin != null)
            {
              this.OnJoin((object) this, message as JoinMessage);
              break;
            }
            break;
          case MessageType.Spectate:
            if (this.OnSpectate != null)
            {
              this.OnSpectate((object) this, message as SpectateMessage);
              break;
            }
            break;
          case MessageType.JoinRequest:
            if (this.OnJoinRequested != null)
            {
              this.OnJoinRequested((object) this, message as JoinRequestMessage);
              break;
            }
            break;
          case MessageType.ConnectionEstablished:
            if (this.OnConnectionEstablished != null)
            {
              this.OnConnectionEstablished((object) this, message as ConnectionEstablishedMessage);
              break;
            }
            break;
          case MessageType.ConnectionFailed:
            if (this.OnConnectionFailed != null)
            {
              this.OnConnectionFailed((object) this, message as ConnectionFailedMessage);
              break;
            }
            break;
          default:
            this.Logger.Error("Message was queued with no appropriate handle! {0}", (object) message.Type);
            break;
        }
      }
      return imessageArray;
    }

    public IMessage Dequeue()
    {
      if (this.Disposed)
        throw new ObjectDisposedException("Discord IPC Client");
      IMessage message = this.connection.DequeueMessage();
      this.HandleMessage(message);
      return message;
    }

    public IMessage[] DequeueAll()
    {
      if (this.Disposed)
        throw new ObjectDisposedException("Discord IPC Client");
      IMessage[] imessageArray = this.connection.DequeueMessages();
      for (int index = 0; index < imessageArray.Length; ++index)
        this.HandleMessage(imessageArray[index]);
      return imessageArray;
    }

    private void HandleMessage(IMessage message)
    {
      if (message == null)
        return;
      switch (message.Type)
      {
        case MessageType.Ready:
          if (!(message is ReadyMessage readyMessage))
            break;
          this._configuration = readyMessage.Configuration;
          this._user = readyMessage.User;
          this.SynchronizeState();
          break;
        case MessageType.PresenceUpdate:
          if (!(message is PresenceMessage presenceMessage))
            break;
          if (this._presence == null)
            this._presence = presenceMessage.Presence;
          else if (presenceMessage.Presence == null)
            this._presence = (RichPresence) null;
          else
            this._presence.Merge(presenceMessage.Presence);
          presenceMessage.Presence = this._presence;
          break;
        case MessageType.Subscribe:
          this._subscription |= (message as SubscribeMessage).Event;
          break;
        case MessageType.Unsubscribe:
          this._subscription &= ~(message as UnsubscribeMessage).Event;
          break;
        case MessageType.JoinRequest:
          if (this.Configuration == null || !(message is JoinRequestMessage joinRequestMessage))
            break;
          joinRequestMessage.User.SetConfiguration(this.Configuration);
          break;
      }
    }

    public void Respond(JoinRequestMessage request, bool acceptRequest)
    {
      if (this.Disposed)
        throw new ObjectDisposedException("Discord IPC Client");
      if (this.connection == null)
        throw new ObjectDisposedException("Connection", "Cannot initialize as the connection has been deinitialized");
      this.connection.EnqueueCommand((ICommand) new RespondCommand()
      {
        Accept = acceptRequest,
        UserID = request.User.ID.ToString()
      });
    }

    public void SetPresence(RichPresence presence)
    {
      if (this.Disposed)
        throw new ObjectDisposedException("Discord IPC Client");
      if (this.connection == null)
        throw new ObjectDisposedException("Connection", "Cannot initialize as the connection has been deinitialized");
      this._presence = presence;
      if (!(bool) this._presence)
      {
        this.connection.EnqueueCommand((ICommand) new PresenceCommand()
        {
          PID = this.ProcessID,
          Presence = (RichPresence) null
        });
      }
      else
      {
        if (presence.HasSecrets() && !this.HasRegisteredUriScheme)
          throw new BadPresenceException("Cannot send a presence with secrets as this object has not registered a URI scheme!");
        if (presence.HasParty() && presence.Party.Max < presence.Party.Size)
          throw new BadPresenceException("Presence maximum party size cannot be smaller than the current size.");
        this.connection.EnqueueCommand((ICommand) new PresenceCommand()
        {
          PID = this.ProcessID,
          Presence = presence.Clone()
        });
      }
    }

    public RichPresence UpdateDetails(string details)
    {
      if (this._presence == null)
        this._presence = new RichPresence();
      this._presence.Details = details;
      try
      {
        this.SetPresence(this._presence);
      }
      catch (Exception ex)
      {
        throw;
      }
      return this._presence;
    }

    public RichPresence UpdateState(string state)
    {
      if (this._presence == null)
        this._presence = new RichPresence();
      this._presence.State = state;
      try
      {
        this.SetPresence(this._presence);
      }
      catch (Exception ex)
      {
        throw;
      }
      return this._presence;
    }

    public RichPresence UpdateParty(Party party)
    {
      if (this._presence == null)
        this._presence = new RichPresence();
      this._presence.Party = party;
      try
      {
        this.SetPresence(this._presence);
      }
      catch (Exception ex)
      {
        throw;
      }
      return this._presence;
    }

    public RichPresence UpdatePartySize(int size)
    {
      if (this._presence == null)
        return (RichPresence) null;
      if (this._presence.Party == null)
        throw new BadPresenceException("Cannot set the size of the party if the party does not exist");
      try
      {
        this.UpdatePartySize(size, this._presence.Party.Max);
      }
      catch (Exception ex)
      {
        throw;
      }
      return this._presence;
    }

    public RichPresence UpdatePartySize(int size, int max)
    {
      if (this._presence == null)
        return (RichPresence) null;
      if (this._presence.Party == null)
        throw new BadPresenceException("Cannot set the size of the party if the party does not exist");
      this._presence.Party.Size = size;
      this._presence.Party.Max = max;
      try
      {
        this.SetPresence(this._presence);
      }
      catch (Exception ex)
      {
        throw;
      }
      return this._presence;
    }

    public RichPresence UpdateLargeAsset(string key = null, string tooltip = null)
    {
      if (this._presence == null)
        this._presence = new RichPresence();
      if (this._presence.Assets == null)
        this._presence.Assets = new Assets();
      this._presence.Assets.LargeImageKey = key ?? this._presence.Assets.LargeImageKey;
      this._presence.Assets.LargeImageText = tooltip ?? this._presence.Assets.LargeImageText;
      try
      {
        this.SetPresence(this._presence);
      }
      catch (Exception ex)
      {
        throw;
      }
      return this._presence;
    }

    public RichPresence UpdateSmallAsset(string key = null, string tooltip = null)
    {
      if (this._presence == null)
        this._presence = new RichPresence();
      if (this._presence.Assets == null)
        this._presence.Assets = new Assets();
      this._presence.Assets.SmallImageKey = key ?? this._presence.Assets.SmallImageKey;
      this._presence.Assets.SmallImageText = tooltip ?? this._presence.Assets.SmallImageText;
      try
      {
        this.SetPresence(this._presence);
      }
      catch (Exception ex)
      {
        throw;
      }
      return this._presence;
    }

    public RichPresence UpdateSecrets(Secrets secrets)
    {
      if (this._presence == null)
        this._presence = new RichPresence();
      this._presence.Secrets = secrets;
      try
      {
        this.SetPresence(this._presence);
      }
      catch (Exception ex)
      {
        throw;
      }
      return this._presence;
    }

    public RichPresence UpdateStartTime()
    {
      try
      {
        return this.UpdateStartTime(DateTime.UtcNow);
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public RichPresence UpdateStartTime(DateTime time)
    {
      if (this._presence == null)
        this._presence = new RichPresence();
      if (this._presence.Timestamps == null)
        this._presence.Timestamps = new Timestamps();
      this._presence.Timestamps.Start = new DateTime?(time);
      try
      {
        this.SetPresence(this._presence);
      }
      catch (Exception ex)
      {
        throw;
      }
      return this._presence;
    }

    public RichPresence UpdateEndTime()
    {
      try
      {
        return this.UpdateEndTime(DateTime.UtcNow);
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public RichPresence UpdateEndTime(DateTime time)
    {
      if (this._presence == null)
        this._presence = new RichPresence();
      if (this._presence.Timestamps == null)
        this._presence.Timestamps = new Timestamps();
      this._presence.Timestamps.End = new DateTime?(time);
      try
      {
        this.SetPresence(this._presence);
      }
      catch (Exception ex)
      {
        throw;
      }
      return this._presence;
    }

    public RichPresence UpdateClearTime()
    {
      if (this._presence == null)
        return (RichPresence) null;
      this._presence.Timestamps = (Timestamps) null;
      try
      {
        this.SetPresence(this._presence);
      }
      catch (Exception ex)
      {
        throw;
      }
      return this._presence;
    }

    public void ClearPresence()
    {
      if (this.Disposed)
        throw new ObjectDisposedException("Discord IPC Client");
      if (this.connection == null)
        throw new ObjectDisposedException("Connection", "Cannot initialize as the connection has been deinitialized");
      this.SetPresence((RichPresence) null);
    }

    public void Subscribe(EventType type)
    {
      this.SetSubscription(this._subscription | type);
    }

    public void Unubscribe(EventType type)
    {
      this.SetSubscription(this._subscription & ~type);
    }

    public void SetSubscription(EventType type)
    {
      this.SubscribeToTypes(this._subscription & ~type, true);
      this.SubscribeToTypes(~this._subscription & type, false);
      this._subscription = type;
    }

    private void SubscribeToTypes(EventType type, bool isUnsubscribe)
    {
      if (type == EventType.None)
        return;
      if (this.Disposed)
        throw new ObjectDisposedException("Discord IPC Client");
      if (this.connection == null)
        throw new ObjectDisposedException("Connection", "Cannot initialize as the connection has been deinitialized");
      if (!this.HasRegisteredUriScheme)
        throw new InvalidConfigurationException("Cannot subscribe/unsubscribe to an event as this application has not registered a URI scheme.");
      if ((type & EventType.Spectate) == EventType.Spectate)
        this.connection.EnqueueCommand((ICommand) new SubscribeCommand()
        {
          Event = ServerEvent.ActivitySpectate,
          IsUnsubscribe = isUnsubscribe
        });
      if ((type & EventType.Join) == EventType.Join)
        this.connection.EnqueueCommand((ICommand) new SubscribeCommand()
        {
          Event = ServerEvent.ActivityJoin,
          IsUnsubscribe = isUnsubscribe
        });
      if ((type & EventType.JoinRequest) != EventType.JoinRequest)
        return;
      this.connection.EnqueueCommand((ICommand) new SubscribeCommand()
      {
        Event = ServerEvent.ActivityJoinRequest,
        IsUnsubscribe = isUnsubscribe
      });
    }

    public void SynchronizeState()
    {
      this.SetPresence(this._presence);
      this.SubscribeToTypes(this._subscription, false);
    }

    public bool Initialize()
    {
      if (this.Disposed)
        throw new ObjectDisposedException("Discord IPC Client");
      if (this.connection == null)
        throw new ObjectDisposedException("Connection", "Cannot initialize as the connection has been deinitialized");
      return this._initialized = this.connection.AttemptConnection();
    }

    public void Dispose()
    {
      if (this.Disposed)
        return;
      this.connection.Close();
      this._disposed = true;
    }
  }
}
