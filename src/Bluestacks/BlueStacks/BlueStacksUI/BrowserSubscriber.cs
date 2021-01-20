// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.BrowserSubscriber
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace BlueStacks.BlueStacksUI
{
  public class BrowserSubscriber : ISubscriber
  {
    private Dictionary<BrowserControlTags, object> mTokens = new Dictionary<BrowserControlTags, object>();
    private BrowserControl mControl;

    public BrowserSubscriber(BrowserControl control)
    {
      this.mControl = control;
      foreach (BrowserControlTags key in control?.TagsSubscribedDict.Keys)
        this.SubscribeTag(key);
    }

    public void SubscribeTag(BrowserControlTags args)
    {
      switch (args)
      {
        case BrowserControlTags.bootComplete:
          this.mTokens[BrowserControlTags.bootComplete] = (object) EventAggregator.Subscribe<BootCompleteEventArgs>(new System.Action<BootCompleteEventArgs>(this.Message));
          break;
        case BrowserControlTags.googleSigninComplete:
          this.mTokens[BrowserControlTags.googleSigninComplete] = (object) EventAggregator.Subscribe<GoogleSignInCompleteEventArgs>(new System.Action<GoogleSignInCompleteEventArgs>(this.Message));
          break;
        case BrowserControlTags.appPlayerClosing:
          this.mTokens[BrowserControlTags.appPlayerClosing] = (object) EventAggregator.Subscribe<AppPlayerClosingEventArgs>(new System.Action<AppPlayerClosingEventArgs>(this.Message));
          break;
        case BrowserControlTags.tabClosing:
          this.mTokens[BrowserControlTags.tabClosing] = (object) EventAggregator.Subscribe<TabClosingEventArgs>(new System.Action<TabClosingEventArgs>(this.Message));
          break;
        case BrowserControlTags.tabSwitched:
          this.mTokens[BrowserControlTags.tabSwitched] = (object) EventAggregator.Subscribe<TabSwitchedEventArgs>(new System.Action<TabSwitchedEventArgs>(this.Message));
          break;
        case BrowserControlTags.appInstalled:
          this.mTokens[BrowserControlTags.appInstalled] = (object) EventAggregator.Subscribe<AppInstalledEventArgs>(new System.Action<AppInstalledEventArgs>(this.Message));
          break;
        case BrowserControlTags.appUninstalled:
          this.mTokens[BrowserControlTags.appUninstalled] = (object) EventAggregator.Subscribe<AppUninstalledEventArgs>(new System.Action<AppUninstalledEventArgs>(this.Message));
          break;
        case BrowserControlTags.grmAppListUpdate:
          this.mTokens[BrowserControlTags.grmAppListUpdate] = (object) EventAggregator.Subscribe<GrmAppListUpdateEventArgs>(new System.Action<GrmAppListUpdateEventArgs>(this.Message));
          break;
        case BrowserControlTags.apkDownloadStarted:
          this.mTokens[BrowserControlTags.apkDownloadStarted] = (object) EventAggregator.Subscribe<ApkDownloadStartedEventArgs>(new System.Action<ApkDownloadStartedEventArgs>(this.Message));
          break;
        case BrowserControlTags.apkDownloadFailed:
          this.mTokens[BrowserControlTags.apkDownloadFailed] = (object) EventAggregator.Subscribe<ApkDownloadFailedEventArgs>(new System.Action<ApkDownloadFailedEventArgs>(this.Message));
          break;
        case BrowserControlTags.apkDownloadCurrentProgress:
          this.mTokens[BrowserControlTags.apkDownloadCurrentProgress] = (object) EventAggregator.Subscribe<ApkDownloadCurrentProgressEventArgs>(new System.Action<ApkDownloadCurrentProgressEventArgs>(this.Message));
          break;
        case BrowserControlTags.apkDownloadCompleted:
          this.mTokens[BrowserControlTags.apkDownloadCompleted] = (object) EventAggregator.Subscribe<ApkDownloadCompletedEventArgs>(new System.Action<ApkDownloadCompletedEventArgs>(this.Message));
          break;
        case BrowserControlTags.apkInstallStarted:
          this.mTokens[BrowserControlTags.apkInstallStarted] = (object) EventAggregator.Subscribe<ApkInstallStartedEventArgs>(new System.Action<ApkInstallStartedEventArgs>(this.Message));
          break;
        case BrowserControlTags.apkInstallFailed:
          this.mTokens[BrowserControlTags.apkInstallFailed] = (object) EventAggregator.Subscribe<ApkInstallFailedEventArgs>(new System.Action<ApkInstallFailedEventArgs>(this.Message));
          break;
        case BrowserControlTags.apkInstallCompleted:
          this.mTokens[BrowserControlTags.apkInstallCompleted] = (object) EventAggregator.Subscribe<ApkInstallCompletedEventArgs>(new System.Action<ApkInstallCompletedEventArgs>(this.Message));
          break;
        case BrowserControlTags.getVmInfo:
          this.mTokens[BrowserControlTags.getVmInfo] = (object) EventAggregator.Subscribe<GetVmInfoEventArgs>(new System.Action<GetVmInfoEventArgs>(this.Message));
          break;
        case BrowserControlTags.userInfoUpdated:
          this.mTokens[BrowserControlTags.userInfoUpdated] = (object) EventAggregator.Subscribe<UserInfoUpdatedEventArgs>(new System.Action<UserInfoUpdatedEventArgs>(this.Message));
          break;
        case BrowserControlTags.themeChange:
          this.mTokens[BrowserControlTags.themeChange] = (object) EventAggregator.Subscribe<ThemeChangeEventArgs>(new System.Action<ThemeChangeEventArgs>(this.Message));
          break;
        case BrowserControlTags.oemDownloadStarted:
          this.mTokens[BrowserControlTags.oemDownloadStarted] = (object) EventAggregator.Subscribe<OemDownloadStartedEventArgs>(new System.Action<OemDownloadStartedEventArgs>(this.Message));
          break;
        case BrowserControlTags.oemDownloadFailed:
          this.mTokens[BrowserControlTags.oemDownloadFailed] = (object) EventAggregator.Subscribe<OemDownloadFailedEventArgs>(new System.Action<OemDownloadFailedEventArgs>(this.Message));
          break;
        case BrowserControlTags.oemDownloadCurrentProgress:
          this.mTokens[BrowserControlTags.oemDownloadCurrentProgress] = (object) EventAggregator.Subscribe<OemDownloadCurrentProgressEventArgs>(new System.Action<OemDownloadCurrentProgressEventArgs>(this.Message));
          break;
        case BrowserControlTags.oemDownloadCompleted:
          this.mTokens[BrowserControlTags.oemDownloadCompleted] = (object) EventAggregator.Subscribe<OemDownloadCompletedEventArgs>(new System.Action<OemDownloadCompletedEventArgs>(this.Message));
          break;
        case BrowserControlTags.oemInstallStarted:
          this.mTokens[BrowserControlTags.oemInstallStarted] = (object) EventAggregator.Subscribe<OemInstallStartedEventArgs>(new System.Action<OemInstallStartedEventArgs>(this.Message));
          break;
        case BrowserControlTags.oemInstallFailed:
          this.mTokens[BrowserControlTags.oemInstallFailed] = (object) EventAggregator.Subscribe<OemInstallFailedEventArgs>(new System.Action<OemInstallFailedEventArgs>(this.Message));
          break;
        case BrowserControlTags.oemInstallCompleted:
          this.mTokens[BrowserControlTags.oemInstallCompleted] = (object) EventAggregator.Subscribe<OemInstallCompletedEventArgs>(new System.Action<OemInstallCompletedEventArgs>(this.Message));
          break;
        case BrowserControlTags.showFlePopup:
          this.mTokens[BrowserControlTags.showFlePopup] = (object) EventAggregator.Subscribe<ShowFlePopupEventArgs>(new System.Action<ShowFlePopupEventArgs>(this.Message));
          break;
        case BrowserControlTags.currentlyRunningApps:
          this.mTokens[BrowserControlTags.currentlyRunningApps] = (object) EventAggregator.Subscribe<CurrentlyRunningAppsEventArgs>(new System.Action<CurrentlyRunningAppsEventArgs>(this.Message));
          break;
        case BrowserControlTags.handleFle:
          this.mTokens[BrowserControlTags.handleFle] = (object) EventAggregator.Subscribe<HandleFleEventArgs>(new System.Action<HandleFleEventArgs>(this.Message));
          break;
        case BrowserControlTags.getVolumeLevel:
          this.mTokens[BrowserControlTags.getVolumeLevel] = (object) EventAggregator.Subscribe<GetVolumeLevelEventArgs>(new System.Action<GetVolumeLevelEventArgs>(this.Message));
          break;
        case BrowserControlTags.toggleMicrophoneMuteState:
          this.mTokens[BrowserControlTags.toggleMicrophoneMuteState] = (object) EventAggregator.Subscribe<ToggleMicrophoneMuteStateEventArgs>(new System.Action<ToggleMicrophoneMuteStateEventArgs>(this.Message));
          break;
      }
    }

    public void UnsubscribeTag(BrowserControlTags args)
    {
      switch (args)
      {
        case BrowserControlTags.bootComplete:
          EventAggregator.Unsubscribe<BootCompleteEventArgs>((Subscription<BootCompleteEventArgs>) this.mTokens[BrowserControlTags.bootComplete]);
          break;
        case BrowserControlTags.googleSigninComplete:
          EventAggregator.Unsubscribe<GoogleSignInCompleteEventArgs>((Subscription<GoogleSignInCompleteEventArgs>) this.mTokens[BrowserControlTags.googleSigninComplete]);
          break;
        case BrowserControlTags.appPlayerClosing:
          EventAggregator.Unsubscribe<AppPlayerClosingEventArgs>((Subscription<AppPlayerClosingEventArgs>) this.mTokens[BrowserControlTags.appPlayerClosing]);
          break;
        case BrowserControlTags.tabClosing:
          EventAggregator.Unsubscribe<TabClosingEventArgs>((Subscription<TabClosingEventArgs>) this.mTokens[BrowserControlTags.tabClosing]);
          break;
        case BrowserControlTags.tabSwitched:
          EventAggregator.Unsubscribe<TabSwitchedEventArgs>((Subscription<TabSwitchedEventArgs>) this.mTokens[BrowserControlTags.tabSwitched]);
          break;
        case BrowserControlTags.appInstalled:
          EventAggregator.Unsubscribe<AppInstalledEventArgs>((Subscription<AppInstalledEventArgs>) this.mTokens[BrowserControlTags.appInstalled]);
          break;
        case BrowserControlTags.appUninstalled:
          EventAggregator.Unsubscribe<AppUninstalledEventArgs>((Subscription<AppUninstalledEventArgs>) this.mTokens[BrowserControlTags.appUninstalled]);
          break;
        case BrowserControlTags.grmAppListUpdate:
          EventAggregator.Unsubscribe<GrmAppListUpdateEventArgs>((Subscription<GrmAppListUpdateEventArgs>) this.mTokens[BrowserControlTags.grmAppListUpdate]);
          break;
        case BrowserControlTags.apkDownloadStarted:
          EventAggregator.Unsubscribe<ApkDownloadStartedEventArgs>((Subscription<ApkDownloadStartedEventArgs>) this.mTokens[BrowserControlTags.apkDownloadStarted]);
          break;
        case BrowserControlTags.apkDownloadFailed:
          EventAggregator.Unsubscribe<ApkDownloadFailedEventArgs>((Subscription<ApkDownloadFailedEventArgs>) this.mTokens[BrowserControlTags.apkDownloadFailed]);
          break;
        case BrowserControlTags.apkDownloadCurrentProgress:
          EventAggregator.Unsubscribe<ApkDownloadCurrentProgressEventArgs>((Subscription<ApkDownloadCurrentProgressEventArgs>) this.mTokens[BrowserControlTags.apkDownloadCurrentProgress]);
          break;
        case BrowserControlTags.apkDownloadCompleted:
          EventAggregator.Unsubscribe<ApkDownloadCompletedEventArgs>((Subscription<ApkDownloadCompletedEventArgs>) this.mTokens[BrowserControlTags.apkDownloadCompleted]);
          break;
        case BrowserControlTags.apkInstallStarted:
          EventAggregator.Unsubscribe<ApkInstallStartedEventArgs>((Subscription<ApkInstallStartedEventArgs>) this.mTokens[BrowserControlTags.apkInstallStarted]);
          break;
        case BrowserControlTags.apkInstallFailed:
          EventAggregator.Unsubscribe<ApkInstallFailedEventArgs>((Subscription<ApkInstallFailedEventArgs>) this.mTokens[BrowserControlTags.apkInstallFailed]);
          break;
        case BrowserControlTags.apkInstallCompleted:
          EventAggregator.Unsubscribe<ApkInstallCompletedEventArgs>((Subscription<ApkInstallCompletedEventArgs>) this.mTokens[BrowserControlTags.apkInstallCompleted]);
          break;
        case BrowserControlTags.getVmInfo:
          EventAggregator.Unsubscribe<GetVmInfoEventArgs>((Subscription<GetVmInfoEventArgs>) this.mTokens[BrowserControlTags.getVmInfo]);
          break;
        case BrowserControlTags.userInfoUpdated:
          EventAggregator.Unsubscribe<UserInfoUpdatedEventArgs>((Subscription<UserInfoUpdatedEventArgs>) this.mTokens[BrowserControlTags.userInfoUpdated]);
          break;
        case BrowserControlTags.themeChange:
          EventAggregator.Unsubscribe<ThemeChangeEventArgs>((Subscription<ThemeChangeEventArgs>) this.mTokens[BrowserControlTags.themeChange]);
          break;
        case BrowserControlTags.oemDownloadStarted:
          EventAggregator.Unsubscribe<OemDownloadStartedEventArgs>((Subscription<OemDownloadStartedEventArgs>) this.mTokens[BrowserControlTags.oemDownloadStarted]);
          break;
        case BrowserControlTags.oemDownloadFailed:
          EventAggregator.Unsubscribe<OemDownloadFailedEventArgs>((Subscription<OemDownloadFailedEventArgs>) this.mTokens[BrowserControlTags.oemDownloadFailed]);
          break;
        case BrowserControlTags.oemDownloadCurrentProgress:
          EventAggregator.Unsubscribe<OemDownloadCurrentProgressEventArgs>((Subscription<OemDownloadCurrentProgressEventArgs>) this.mTokens[BrowserControlTags.oemDownloadCurrentProgress]);
          break;
        case BrowserControlTags.oemDownloadCompleted:
          EventAggregator.Unsubscribe<OemDownloadCompletedEventArgs>((Subscription<OemDownloadCompletedEventArgs>) this.mTokens[BrowserControlTags.oemDownloadCompleted]);
          break;
        case BrowserControlTags.oemInstallStarted:
          EventAggregator.Unsubscribe<OemInstallStartedEventArgs>((Subscription<OemInstallStartedEventArgs>) this.mTokens[BrowserControlTags.oemInstallStarted]);
          break;
        case BrowserControlTags.oemInstallFailed:
          EventAggregator.Unsubscribe<OemInstallFailedEventArgs>((Subscription<OemInstallFailedEventArgs>) this.mTokens[BrowserControlTags.oemInstallFailed]);
          break;
        case BrowserControlTags.oemInstallCompleted:
          EventAggregator.Unsubscribe<OemInstallCompletedEventArgs>((Subscription<OemInstallCompletedEventArgs>) this.mTokens[BrowserControlTags.oemInstallCompleted]);
          break;
        case BrowserControlTags.showFlePopup:
          EventAggregator.Unsubscribe<ShowFlePopupEventArgs>((Subscription<ShowFlePopupEventArgs>) this.mTokens[BrowserControlTags.showFlePopup]);
          break;
        case BrowserControlTags.currentlyRunningApps:
          EventAggregator.Unsubscribe<CurrentlyRunningAppsEventArgs>((Subscription<CurrentlyRunningAppsEventArgs>) this.mTokens[BrowserControlTags.currentlyRunningApps]);
          break;
        case BrowserControlTags.handleFle:
          EventAggregator.Unsubscribe<HandleFleEventArgs>((Subscription<HandleFleEventArgs>) this.mTokens[BrowserControlTags.handleFle]);
          break;
        case BrowserControlTags.getVolumeLevel:
          EventAggregator.Unsubscribe<GetVolumeLevelEventArgs>((Subscription<GetVolumeLevelEventArgs>) this.mTokens[BrowserControlTags.getVolumeLevel]);
          break;
        case BrowserControlTags.toggleMicrophoneMuteState:
          EventAggregator.Unsubscribe<ToggleMicrophoneMuteStateEventArgs>((Subscription<ToggleMicrophoneMuteStateEventArgs>) this.mTokens[BrowserControlTags.toggleMicrophoneMuteState]);
          break;
      }
    }

    public void Message(EventArgs eventArgs)
    {
      if (!(eventArgs is BrowserEventArgs browserEventArgs) || !string.Equals(this.mControl.ParentWindow.mVmName, browserEventArgs.mVmName, StringComparison.InvariantCultureIgnoreCase) && (!this.mControl.TagsSubscribedDict[browserEventArgs.ClientTag].ContainsKey("IsReceiveFromAllVm") || !this.mControl.TagsSubscribedDict[browserEventArgs.ClientTag]["IsReceiveFromAllVm"].ToObject<bool>()))
        return;
      JObject jobject = new JObject();
      jobject["eventRaised"] = (JToken) browserEventArgs.ClientTag.ToString();
      jobject["vmName"] = (JToken) browserEventArgs.mVmName;
      if (browserEventArgs.ExtraData != null)
        jobject["extraData"] = (JToken) browserEventArgs.ExtraData;
      this.mControl.CallBackToHtml(this.mControl.TagsSubscribedDict[browserEventArgs.ClientTag]["CallbackFunction"].ToString(), jobject.ToString(Formatting.None));
    }
  }
}
