// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.Discord
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using DiscordRPC;
using DiscordRPC.Events;
using DiscordRPC.Message;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Timers;

namespace BlueStacks.BlueStacksUI
{
  public class Discord : IDisposable
  {
    private List<AppSuggestionPromotion> mSystemApps = new List<AppSuggestionPromotion>();
    private string mDiscordClientID = string.Empty;
    private Dictionary<string, Timestamps> mAppStartTimestamp = new Dictionary<string, Timestamps>();
    private DiscordRpcClient mDiscordClient;
    private Timer mDiscordClientInvokeTimer;
    private string mPreviousAppPackage;
    private MainWindow ParentWindow;
    private bool mIsDiscordConnected;
    private bool disposedValue;

    public Discord(MainWindow window)
    {
      this.ParentWindow = window;
    }

    internal void Init()
    {
      this.SetSystemAppsAndClientID();
      this.InitDiscordClient();
    }

    private void AssignTabChangeEventOnOpenedTabs()
    {
      if (this.ParentWindow == null)
        return;
      this.ParentWindow.Dispatcher.Invoke((Delegate) (() =>
      {
        foreach (AppTabButton button in this.ParentWindow.mTopBar.mAppTabButtons.mDictTabs.Values.ToList<AppTabButton>())
        {
          if (button.EventOnTabChanged == null)
          {
            Logger.Info("discord attaching tab change event on tab.." + button.PackageName);
            this.AssignTabChangeEvent(button);
            if (button.IsSelected)
              this.Tab_ChangeOrCreateEvent((object) null, new TabChangeEventArgs(button.AppName, button.PackageName, button.mTabType));
          }
          else if (button.IsSelected)
            this.Tab_ChangeOrCreateEvent((object) null, new TabChangeEventArgs(button.AppName, button.PackageName, button.mTabType));
        }
      }));
    }

    private void RemoveTabChangeEventFromOpenedTabs()
    {
      if (this.ParentWindow == null)
        return;
      this.ParentWindow.Dispatcher.Invoke((Delegate) (() =>
      {
        foreach (AppTabButton appTabButton in this.ParentWindow.mTopBar.mAppTabButtons.mDictTabs.Values.ToList<AppTabButton>())
        {
          if (appTabButton.EventOnTabChanged != null)
            appTabButton.EventOnTabChanged -= new EventHandler<TabChangeEventArgs>(this.Tab_ChangeOrCreateEvent);
        }
      }));
    }

    private void SetSystemAppsAndClientID()
    {
      if (PromotionObject.Instance == null)
        return;
      this.mDiscordClientID = PromotionObject.Instance.DiscordClientID;
      this.mSystemApps = PromotionObject.Instance.AppSuggestionList.Where<AppSuggestionPromotion>((Func<AppSuggestionPromotion, bool>) (x => string.Equals(x.AppLocation, "more_apps", StringComparison.InvariantCulture))).ToList<AppSuggestionPromotion>();
    }

    internal void RemoveAppFromTimestampList(string package)
    {
      if (!this.mAppStartTimestamp.ContainsKey(package))
        return;
      this.mAppStartTimestamp.Remove(package);
    }

    internal bool IsDiscordClientReady()
    {
      return this.mDiscordClient != null && this.mDiscordClient.IsInitialized && this.mIsDiscordConnected;
    }

    private void Tab_ChangeOrCreateEvent(object sender, TabChangeEventArgs e)
    {
      try
      {
        if (string.Equals(this.mPreviousAppPackage, e.PackageName, StringComparison.InvariantCulture) || !this.IsDiscordClientReady())
          return;
        Logger.Info("Discord tab changed event. PkgName: {0}, AppName: {1}", (object) e.PackageName, (object) e.AppName);
        RichPresence presence = new RichPresence();
        switch (e.TabType)
        {
          case TabType.AppTab:
            if (this.mSystemApps.Any<AppSuggestionPromotion>((Func<AppSuggestionPromotion, bool>) (_ => object.Equals((object) (_.AppPackage == e.PackageName), (object) StringComparison.InvariantCulture))))
            {
              presence.State = "In Lobby";
              presence.Details = "About to start a game";
              presence.Assets = new DiscordRPC.Assets()
              {
                LargeImageKey = "bstk-logo",
                LargeImageText = "BlueStacks",
                SmallImageKey = "",
                SmallImageText = ""
              };
              break;
            }
            if (e.PackageName.Contains("android.vending"))
            {
              presence.State = "Searching";
              presence.Details = "Google Play Store";
              presence.Assets = new DiscordRPC.Assets()
              {
                LargeImageKey = "bstk-logo",
                LargeImageText = "BlueStacks",
                SmallImageKey = "com_android_vending",
                SmallImageText = "Google Play"
              };
              break;
            }
            if (this.mAppStartTimestamp.ContainsKey(e.PackageName))
            {
              presence.Timestamps = this.mAppStartTimestamp[e.PackageName];
            }
            else
            {
              presence.Timestamps = Timestamps.Now;
              this.mAppStartTimestamp.Add(e.PackageName, Timestamps.Now);
            }
            presence.State = "Playing";
            presence.Details = e.AppName;
            presence.Assets = new DiscordRPC.Assets()
            {
              LargeImageKey = this.GetMD5HashFromPackageName(e.PackageName),
              LargeImageText = e.AppName,
              SmallImageKey = "bstk-logo",
              SmallImageText = "BlueStacks"
            };
            break;
          case TabType.WebTab:
          case TabType.HomeTab:
            if (e.PackageName.Contains("bluestacks") && e.PackageName.Contains("appcenter"))
            {
              presence.State = "Searching";
              presence.Details = "Google Play Store";
              presence.Assets = new DiscordRPC.Assets()
              {
                LargeImageKey = "bstk-logo",
                LargeImageText = "BlueStacks",
                SmallImageKey = "com_android_vending",
                SmallImageText = "Google Play"
              };
              break;
            }
            presence.State = "In Lobby";
            presence.Details = "About to start a game";
            presence.Assets = new DiscordRPC.Assets()
            {
              LargeImageKey = "bstk-logo",
              LargeImageText = "BlueStacks",
              SmallImageKey = "",
              SmallImageText = ""
            };
            break;
        }
        this.SetPresence(presence);
        this.mPreviousAppPackage = e.PackageName;
      }
      catch (Exception ex)
      {
        Logger.Error("Error while setting presence in discord with exception : {0}", (object) ex.ToString());
      }
    }

    private string GetMD5HashFromPackageName(string package)
    {
      string lower = new _MD5() { Value = package }.FingerPrint.ToLower(CultureInfo.InvariantCulture);
      Logger.Info("Md5 hash for package name: {0}..is {1}", (object) package, (object) lower);
      return lower;
    }

    internal void AssignTabChangeEvent(AppTabButton button)
    {
      if (button.EventOnTabChanged != null)
        return;
      button.EventOnTabChanged += new EventHandler<TabChangeEventArgs>(this.Tab_ChangeOrCreateEvent);
    }

    private void InitDiscordClient()
    {
      try
      {
        if (this.mDiscordClient != null)
          return;
        Logger.Info("Initing discord");
        this.mDiscordClient = new DiscordRpcClient(this.mDiscordClientID);
        this.mDiscordClient.OnReady += (OnReadyEvent) ((sender, msg) => Logger.Info("Connected to discord with user {0}", (object) msg.User.Username));
        this.mDiscordClient.OnPresenceUpdate += new OnPresenceUpdateEvent(this.Client_OnPresenceUpdate);
        this.mDiscordClient.OnError += new OnErrorEvent(this.Client_OnError);
        this.mDiscordClient.OnConnectionFailed += new OnConnectionFailedEvent(this.Client_OnConnectionFailed);
        this.mDiscordClient.OnConnectionEstablished += new OnConnectionEstablishedEvent(this.Client_OnConnectionEstablished);
        this.mDiscordClientInvokeTimer = new Timer(150.0);
        this.mDiscordClientInvokeTimer.Elapsed += (ElapsedEventHandler) ((sender, evt) => this.mDiscordClient.Invoke());
        this.mDiscordClientInvokeTimer.Start();
        Logger.Info("Discord client init: {0}", (object) this.mDiscordClient.Initialize());
      }
      catch (Exception ex)
      {
        Logger.Info("Exception in Discord init. ex:  " + ex.ToString());
      }
    }

    private void Client_OnPresenceUpdate(object sender, PresenceMessage args)
    {
      Logger.Info("Discord presence has been updated with details." + args?.Presence?.Details);
      if (args.Presence.Assets.LargeImageKey != null)
        return;
      RichPresence presence = args.Presence.Clone();
      presence.Assets.LargeImageKey = "bstk-logo";
      presence.Assets.SmallImageKey = "";
      presence.Assets.SmallImageText = "";
      this.SetPresence(presence);
    }

    private void SetPresence(RichPresence presence)
    {
      if (this.mDiscordClient != null && this.mDiscordClient.IsInitialized)
        this.mDiscordClient.SetPresence(presence);
      else
        Logger.Warning("SetPresence called without a client being inited");
    }

    private void Client_OnConnectionEstablished(object sender, ConnectionEstablishedMessage args)
    {
      Logger.Info("Discord connection Established");
      this.mIsDiscordConnected = true;
      this.AssignTabChangeEventOnOpenedTabs();
      ClientStats.SendMiscellaneousStatsAsync("DiscordConnected", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.RegisteredEmail, Oem.Instance.OEM, (string) null, (string) null, (string) null, (string) null, "Android");
    }

    private void Client_OnConnectionFailed(object sender, ConnectionFailedMessage args)
    {
      Logger.Info("Discord connection failed. ErrorCode: {0}", (object) args.Type);
      this.mIsDiscordConnected = false;
      this.Dispose();
      ClientStats.SendMiscellaneousStatsAsync("DiscordNotConnected", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.RegisteredEmail, Oem.Instance.OEM, (string) null, (string) null, (string) null, (string) null, "Android");
    }

    private void Client_OnError(object sender, ErrorMessage args)
    {
      Logger.Info("Discord client error. ErrorCode: {0}, Message: {1}", (object) args.Code, (object) args.Message);
    }

    internal void ToggleDiscordState(bool state)
    {
      if (state)
      {
        if (this.mDiscordClient != null)
          return;
        this.Init();
      }
      else
        this.Dispose();
    }

    protected virtual void Dispose(bool disposing)
    {
      if (this.disposedValue)
        return;
      int num = disposing ? 1 : 0;
      if (this.mDiscordClient != null)
      {
        this.mDiscordClient.OnPresenceUpdate -= new OnPresenceUpdateEvent(this.Client_OnPresenceUpdate);
        this.mDiscordClient.OnError -= new OnErrorEvent(this.Client_OnError);
        this.mDiscordClient.OnConnectionFailed -= new OnConnectionFailedEvent(this.Client_OnConnectionFailed);
        this.mDiscordClient.OnConnectionEstablished -= new OnConnectionEstablishedEvent(this.Client_OnConnectionEstablished);
        this.mDiscordClient.Dispose();
        this.RemoveTabChangeEventFromOpenedTabs();
      }
      if (this.mDiscordClientInvokeTimer != null)
      {
        this.mDiscordClientInvokeTimer.Elapsed -= (ElapsedEventHandler) ((sender, evt) => this.mDiscordClient.Invoke());
        this.mDiscordClientInvokeTimer.Dispose();
      }
      this.disposedValue = true;
    }

    ~Discord()
    {
      this.Dispose(false);
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }
  }
}
