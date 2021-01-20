// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.Publisher
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using Newtonsoft.Json.Linq;

namespace BlueStacks.Common
{
  public static class Publisher
  {
    public static void PublishMessage(BrowserControlTags tag, string vmName = "", JObject extraData = null)
    {
      Logger.Info(string.Format("Install boot: PublishMessage {0}", (object) tag));
      switch (tag)
      {
        case BrowserControlTags.bootComplete:
          EventAggregator.Publish<BootCompleteEventArgs>(new BootCompleteEventArgs(BrowserControlTags.bootComplete, vmName, extraData));
          break;
        case BrowserControlTags.googleSigninComplete:
          EventAggregator.Publish<GoogleSignInCompleteEventArgs>(new GoogleSignInCompleteEventArgs(BrowserControlTags.googleSigninComplete, vmName, extraData));
          break;
        case BrowserControlTags.appPlayerClosing:
          EventAggregator.Publish<AppPlayerClosingEventArgs>(new AppPlayerClosingEventArgs(BrowserControlTags.appPlayerClosing, vmName, extraData));
          break;
        case BrowserControlTags.tabClosing:
          EventAggregator.Publish<TabClosingEventArgs>(new TabClosingEventArgs(BrowserControlTags.tabClosing, vmName, extraData));
          break;
        case BrowserControlTags.tabSwitched:
          EventAggregator.Publish<TabSwitchedEventArgs>(new TabSwitchedEventArgs(BrowserControlTags.tabSwitched, vmName, extraData));
          break;
        case BrowserControlTags.appInstalled:
          EventAggregator.Publish<AppInstalledEventArgs>(new AppInstalledEventArgs(BrowserControlTags.appInstalled, vmName, extraData));
          break;
        case BrowserControlTags.appUninstalled:
          EventAggregator.Publish<AppUninstalledEventArgs>(new AppUninstalledEventArgs(BrowserControlTags.appUninstalled, vmName, extraData));
          break;
        case BrowserControlTags.grmAppListUpdate:
          EventAggregator.Publish<GrmAppListUpdateEventArgs>(new GrmAppListUpdateEventArgs(BrowserControlTags.grmAppListUpdate, vmName, extraData));
          break;
        case BrowserControlTags.apkDownloadStarted:
          EventAggregator.Publish<ApkDownloadStartedEventArgs>(new ApkDownloadStartedEventArgs(BrowserControlTags.apkDownloadStarted, vmName, extraData));
          break;
        case BrowserControlTags.apkDownloadFailed:
          EventAggregator.Publish<ApkDownloadFailedEventArgs>(new ApkDownloadFailedEventArgs(BrowserControlTags.apkDownloadFailed, vmName, extraData));
          break;
        case BrowserControlTags.apkDownloadCurrentProgress:
          EventAggregator.Publish<ApkDownloadCurrentProgressEventArgs>(new ApkDownloadCurrentProgressEventArgs(BrowserControlTags.apkDownloadCurrentProgress, vmName, extraData));
          break;
        case BrowserControlTags.apkDownloadCompleted:
          EventAggregator.Publish<ApkDownloadCompletedEventArgs>(new ApkDownloadCompletedEventArgs(BrowserControlTags.apkDownloadCompleted, vmName, extraData));
          break;
        case BrowserControlTags.apkInstallStarted:
          EventAggregator.Publish<ApkInstallStartedEventArgs>(new ApkInstallStartedEventArgs(BrowserControlTags.apkInstallStarted, vmName, extraData));
          break;
        case BrowserControlTags.apkInstallFailed:
          EventAggregator.Publish<ApkInstallFailedEventArgs>(new ApkInstallFailedEventArgs(BrowserControlTags.apkInstallFailed, vmName, extraData));
          break;
        case BrowserControlTags.apkInstallCompleted:
          EventAggregator.Publish<ApkInstallCompletedEventArgs>(new ApkInstallCompletedEventArgs(BrowserControlTags.apkInstallCompleted, vmName, extraData));
          break;
        case BrowserControlTags.getVmInfo:
          EventAggregator.Publish<GetVmInfoEventArgs>(new GetVmInfoEventArgs(BrowserControlTags.getVmInfo, vmName, extraData));
          break;
        case BrowserControlTags.userInfoUpdated:
          EventAggregator.Publish<UserInfoUpdatedEventArgs>(new UserInfoUpdatedEventArgs(BrowserControlTags.userInfoUpdated, vmName, extraData));
          break;
        case BrowserControlTags.themeChange:
          EventAggregator.Publish<ThemeChangeEventArgs>(new ThemeChangeEventArgs(BrowserControlTags.themeChange, vmName, extraData));
          break;
        case BrowserControlTags.oemDownloadStarted:
          EventAggregator.Publish<OemDownloadStartedEventArgs>(new OemDownloadStartedEventArgs(BrowserControlTags.oemDownloadStarted, vmName, extraData));
          break;
        case BrowserControlTags.oemDownloadFailed:
          EventAggregator.Publish<OemDownloadFailedEventArgs>(new OemDownloadFailedEventArgs(BrowserControlTags.oemDownloadFailed, vmName, extraData));
          break;
        case BrowserControlTags.oemDownloadCurrentProgress:
          EventAggregator.Publish<OemDownloadCurrentProgressEventArgs>(new OemDownloadCurrentProgressEventArgs(BrowserControlTags.oemDownloadCurrentProgress, vmName, extraData));
          break;
        case BrowserControlTags.oemDownloadCompleted:
          EventAggregator.Publish<OemDownloadCompletedEventArgs>(new OemDownloadCompletedEventArgs(BrowserControlTags.oemDownloadCompleted, vmName, extraData));
          break;
        case BrowserControlTags.oemInstallStarted:
          EventAggregator.Publish<OemInstallStartedEventArgs>(new OemInstallStartedEventArgs(BrowserControlTags.oemInstallStarted, vmName, extraData));
          break;
        case BrowserControlTags.oemInstallFailed:
          EventAggregator.Publish<OemInstallFailedEventArgs>(new OemInstallFailedEventArgs(BrowserControlTags.oemInstallFailed, vmName, extraData));
          break;
        case BrowserControlTags.oemInstallCompleted:
          EventAggregator.Publish<OemInstallCompletedEventArgs>(new OemInstallCompletedEventArgs(BrowserControlTags.oemInstallCompleted, vmName, extraData));
          break;
        case BrowserControlTags.showFlePopup:
          EventAggregator.Publish<ShowFlePopupEventArgs>(new ShowFlePopupEventArgs(BrowserControlTags.showFlePopup, vmName, extraData));
          break;
        case BrowserControlTags.currentlyRunningApps:
          EventAggregator.Publish<CurrentlyRunningAppsEventArgs>(new CurrentlyRunningAppsEventArgs(BrowserControlTags.currentlyRunningApps, vmName, extraData));
          break;
        case BrowserControlTags.handleFle:
          EventAggregator.Publish<HandleFleEventArgs>(new HandleFleEventArgs(BrowserControlTags.handleFle, vmName, extraData));
          break;
        case BrowserControlTags.getVolumeLevel:
          EventAggregator.Publish<GetVolumeLevelEventArgs>(new GetVolumeLevelEventArgs(BrowserControlTags.getVolumeLevel, vmName, extraData));
          break;
        case BrowserControlTags.toggleMicrophoneMuteState:
          EventAggregator.Publish<ToggleMicrophoneMuteStateEventArgs>(new ToggleMicrophoneMuteStateEventArgs(BrowserControlTags.toggleMicrophoneMuteState, vmName, extraData));
          break;
      }
    }
  }
}
