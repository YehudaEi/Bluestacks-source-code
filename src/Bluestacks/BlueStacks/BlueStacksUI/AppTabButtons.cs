// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.AppTabButtons
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace BlueStacks.BlueStacksUI
{
  public class AppTabButtons : UserControl, IComponentConnector
  {
    private int mTabMinWidth = 48;
    private DateTime mLastTimeOfSizeChangeEventRecieved = DateTime.Now;
    private int mSizeChangedEventCountInLast2Seconds = 1;
    internal Dictionary<string, AppTabButton> mDictTabs = new Dictionary<string, AppTabButton>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    internal string mLastPackageForQuitPopupDisplayed = "";
    private MainWindow mMainWindow;
    internal AppTabButton mHomeAppTabButton;
    internal StackPanel mPanel;
    internal AppTabButton mMoreTabButton;
    internal CustomPopUp mPopup;
    internal Border mMaskBorder;
    internal StackPanel mHiddenButtons;
    private bool _contentLoaded;

    public MainWindow ParentWindow
    {
      get
      {
        if (this.mMainWindow == null)
          this.mMainWindow = Window.GetWindow((DependencyObject) this) as MainWindow;
        return this.mMainWindow;
      }
    }

    public EventHandler<EventArgs> EventOnTabChanged { get; set; }

    public List<string> ListTabHistory { get; } = new List<string>();

    public int AreaForTABS
    {
      get
      {
        int num = (int) (this.ActualWidth - 20.0);
        if (num < 0)
          num = 0;
        return num;
      }
    }

    internal AppTabButton SelectedTab
    {
      get
      {
        return this.ParentWindow.StaticComponents.mSelectedTabButton;
      }
    }

    public AppTabButtons()
    {
      this.InitializeComponent();
      if (DesignerProperties.GetIsInDesignMode((DependencyObject) this))
        return;
      this.SizeChanged += new SizeChangedEventHandler(this.Window_SizeChanged);
      this.Loaded += new RoutedEventHandler(this.AppTabButtons_Loaded);
    }

    private void AppTabButtons_Loaded(object sender, RoutedEventArgs e)
    {
      this.Loaded -= new RoutedEventHandler(this.AppTabButtons_Loaded);
      if (Oem.IsOEMDmm || RegistryManager.Instance.InstallationType != InstallationTypes.FullEdition)
        return;
      Logger.Info("Test logs: AppTabButtons_Loaded()");
      this.AddHomeTab();
    }

    internal void AddHomeTab()
    {
      Logger.Info("Test logs: AddHomeTab()");
      AppTabButton appTabButton = new AppTabButton();
      this.mHomeAppTabButton = appTabButton;
      this.mPanel.Children.Insert(0, (UIElement) appTabButton);
      appTabButton.Init("STRING_HOME", "Home", string.Empty, "home", this.ParentWindow.WelcomeTabParentGrid, "Home");
      BlueStacksUIBinding.Bind(appTabButton.mTabLabel, "STRING_HOME");
      appTabButton.MouseUp += new MouseButtonEventHandler(this.AppTabButton_MouseUp);
      this.mDictTabs[appTabButton.PackageName] = appTabButton;
      if (RegistryManager.Instance.InstallationType == InstallationTypes.GamingEdition)
        appTabButton.Visibility = Visibility.Collapsed;
      this.ResizeTabs();
      this.GoToTab("Home", false, false);
    }

    internal void AddHiddenAppTabAndLaunch(string packageName, string activityName)
    {
      this.AddAppTab("", packageName, activityName, "", true, true, false);
      this.ParentWindow.StaticComponents.mSelectedTabButton.Visibility = Visibility.Collapsed;
    }

    internal void AddAppTab(
      string appName,
      string packageName,
      string activityName,
      string imageName,
      bool isSwitch,
      bool isLaunch,
      bool receivedFromImap = false)
    {
      this.DoExtraHandlingForApp(packageName);
      if (PostBootCloudInfoManager.Instance.mPostBootCloudInfo?.GameNotificationAppPackages?.NotificationModeAppPackages?.IsPackageAvailable(packageName).GetValueOrDefault())
        this.ParentWindow.EngineInstanceRegistry.LastNotificationEnabledAppLaunched = packageName;
      if (this.mDictTabs.ContainsKey(packageName))
      {
        this.GoToTab(packageName, isLaunch, receivedFromImap);
      }
      else
      {
        AppTabButton selectedTabButton = this.ParentWindow.StaticComponents.mSelectedTabButton;
        AppTabButton button = new AppTabButton();
        button.Init(appName, packageName, activityName, imageName, this.ParentWindow.FrontendParentGrid, packageName);
        button.MouseUp += new MouseButtonEventHandler(this.AppTabButton_MouseUp);
        if (this.ParentWindow.mDiscordhandler != null)
          this.ParentWindow.mDiscordhandler.AssignTabChangeEvent(button);
        if (Oem.IsOEMDmm && this.ParentWindow.mDmmBottomBar != null)
          button.EventOnTabChanged += new EventHandler<TabChangeEventArgs>(this.ParentWindow.mDmmBottomBar.Tab_Changed);
        this.mDictTabs.Add(packageName, button);
        this.mPanel.Children.Add((UIElement) button);
        if (Oem.Instance.SendAppClickStatsFromClient)
          ThreadPool.QueueUserWorkItem((WaitCallback) (obj =>
          {
            AppInfo infoFromPackageName = new JsonParser(this.ParentWindow.mVmName).GetAppInfoFromPackageName(packageName);
            string appVersion = string.Empty;
            string appVersionName = string.Empty;
            if (infoFromPackageName != null)
            {
              if (!string.IsNullOrEmpty(infoFromPackageName.Version))
                appVersion = infoFromPackageName.Version;
              if (!string.IsNullOrEmpty(infoFromPackageName.VersionName))
                appVersionName = infoFromPackageName.VersionName;
            }
            BlueStacks.Common.Stats.SendAppStats(appName, packageName, appVersion, "HomeVersionNotKnown", BlueStacks.Common.Stats.AppType.app, this.ParentWindow.mVmName, appVersionName);
          }));
        if (RegistryManager.Instance.InstallationType == InstallationTypes.GamingEdition)
          button.Visibility = Visibility.Collapsed;
        else if (selectedTabButton != null && selectedTabButton.IsPortraitModeTab && selectedTabButton.mTabType == TabType.AppTab)
          button.IsPortraitModeTab = true;
        this.ResizeTabs();
        GrmHandler.RefreshGrmIndicationForAllInstances(packageName);
        Publisher.PublishMessage(BrowserControlTags.currentlyRunningApps, this.ParentWindow.mVmName, new JObject((object) new JProperty("packages", (object) new JArray((object) this.mDictTabs.Values.Select<AppTabButton, string>((Func<AppTabButton, string>) (_ => _.TabKey))))));
        if (!isSwitch)
          return;
        this.GoToTab(packageName, isLaunch, receivedFromImap);
      }
    }

    private void DoExtraHandlingForApp(string packageName)
    {
      if (RegistryManager.Instance.FirstAppLaunchState == AppLaunchState.Installed && JsonParser.GetInstalledAppsList(this.ParentWindow.mVmName).Contains(packageName))
        RegistryManager.Instance.FirstAppLaunchState = AppLaunchState.Launched;
      if (!AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName].ContainsKey(packageName))
        AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName][packageName] = new AppSettings();
      if (this.ParentWindow.mGamepadOverlaySelectedDict.ContainsKey(packageName))
        return;
      this.ParentWindow.mGamepadOverlaySelectedDict[packageName] = false;
    }

    internal void AddWebTab(
      string url,
      string tabName,
      string imageName,
      bool isSwitch,
      string tabKey = "",
      bool forceRefresh = false)
    {
      if (FeatureManager.Instance.IsCustomUIForNCSoft)
        return;
      if (RegistryManager.Instance.InstallationType == InstallationTypes.GamingEdition)
      {
        Process.Start(url);
      }
      else
      {
        bool flag = false;
        if (!string.IsNullOrEmpty(tabKey))
          flag = true;
        if (this.mDictTabs.ContainsKey(flag ? tabKey : url))
        {
          if (this.mDictTabs[flag ? tabKey : url].GetBrowserControl() == null)
          {
            this.mDictTabs[tabKey].mControlGrid = this.ParentWindow.AddBrowser(url);
            this.mDictTabs[tabKey].Init(tabName, url, imageName, this.mDictTabs[tabKey].mControlGrid, tabKey);
          }
          if (flag && string.Compare(url, this.mDictTabs[tabKey].PackageName, StringComparison.OrdinalIgnoreCase) != 0)
          {
            BrowserControl browserControl = this.mDictTabs[tabKey].GetBrowserControl();
            this.mDictTabs[tabKey].Init(tabName, url, imageName, this.mDictTabs[tabKey].mControlGrid, tabKey);
            browserControl?.UpdateUrlAndRefresh(url);
          }
          else if (forceRefresh)
          {
            BrowserControl browserControl = this.mDictTabs[flag ? tabKey : url].GetBrowserControl();
            browserControl.UpdateUrlAndRefresh(browserControl.mUrl);
          }
          this.GoToTab(flag ? tabKey : url, true, false);
        }
        else
        {
          AppTabButton button = new AppTabButton();
          Grid controlGrid = this.ParentWindow.AddBrowser(url);
          controlGrid.Visibility = Visibility.Visible;
          button.Init(tabName, url, imageName, controlGrid, flag ? tabKey : url);
          button.MouseUp += new MouseButtonEventHandler(this.AppTabButton_MouseUp);
          if (this.ParentWindow.mDiscordhandler != null)
            this.ParentWindow.mDiscordhandler.AssignTabChangeEvent(button);
          this.mDictTabs.Add(flag ? tabKey : url, button);
          this.mPanel.Children.Add((UIElement) button);
          this.ResizeTabs();
          if (isSwitch)
            this.GoToTab(flag ? tabKey : url, true, false);
          ClientStats.SendMiscellaneousStatsAsync("WebTabLaunched", RegistryManager.Instance.UserGuid, url, button.AppLabel, RegistryManager.Instance.Version, Oem.Instance.OEM, (string) null, (string) null, (string) null, "Android");
        }
      }
    }

    internal void KillWebTabs()
    {
      if (!RegistryManager.Instance.SwitchKillWebTab)
        return;
      foreach (KeyValuePair<string, AppTabButton> mDictTab in this.mDictTabs)
      {
        if (mDictTab.Value.mTabType == TabType.WebTab)
        {
          foreach (object child in mDictTab.Value.mControlGrid.Children)
          {
            if (child is BrowserControl browserControl && browserControl.CefBrowser != null && browserControl.CheckForBrowserCloseOnAppTabSwitch())
            {
              browserControl.UnwindOnBeforeCloseEvent();
              foreach (BrowserControlTags key in browserControl.TagsSubscribedDict.Keys)
                browserControl.mSubscriber?.UnsubscribeTag(key);
              browserControl.CefBrowser.Dispose();
              browserControl.CefBrowser = (Browser) null;
            }
          }
        }
      }
    }

    private void AppTabButton_MouseUp(object sender, MouseButtonEventArgs e)
    {
      if (e.ChangedButton != MouseButton.Middle)
        return;
      string tabKey = (sender as AppTabButton).TabKey;
      if (string.IsNullOrEmpty(tabKey))
        return;
      this.CloseTab(tabKey, true, false, false, false, "");
    }

    private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      if (this.ParentWindow.mIsFullScreen)
        return;
      this.RefreshUI();
    }

    private void RefreshUI()
    {
      if ((DateTime.Now - this.mLastTimeOfSizeChangeEventRecieved).TotalSeconds > 2.0)
      {
        this.mLastTimeOfSizeChangeEventRecieved = DateTime.Now;
        this.mSizeChangedEventCountInLast2Seconds = 1;
      }
      else
        ++this.mSizeChangedEventCountInLast2Seconds;
      if (this.mSizeChangedEventCountInLast2Seconds > 500)
        return;
      if (this.ParentWindow.IsUIInPortraitMode)
        this.SwitchToIconMode(true);
      else
        this.SwitchToIconMode(false);
      this.ResizeTabs();
    }

    private void SwitchToIconMode(bool isSwitchToIconMode)
    {
      if (isSwitchToIconMode)
      {
        this.mTabMinWidth = 38;
        if (RegistryManager.Instance.InstallationType == InstallationTypes.GamingEdition)
        {
          this.mMoreTabButton.Visibility = Visibility.Hidden;
          this.ParentWindow.mTopBar.mTitleText.Visibility = Visibility.Collapsed;
        }
        else if (!Oem.IsOEMDmm)
          this.ParentWindow.mTopBar.mTitleTextGrid.Visibility = Visibility.Collapsed;
        this.mMoreTabButton.MakeTabParallelogram(false);
      }
      else
      {
        if (RegistryManager.Instance.InstallationType == InstallationTypes.GamingEdition)
          this.ParentWindow.mTopBar.mTitleText.Visibility = Visibility.Visible;
        this.mTabMinWidth = 48;
        this.mMoreTabButton.MakeTabParallelogram(true);
      }
      this.ParentWindow.mTopBar.RefreshWarningButton();
    }

    internal void ResizeTabs()
    {
      if (this.ParentWindow.mIsFullScreen)
        return;
      double num1 = this.MacroGridHandling() + this.VideoRecordingGridHandling();
      if (this.ParentWindow.mTopBar.ActualWidth > this.ParentWindow.mTopBar.mMinimumExpectedTopBarWidth + num1 + 40.0)
        this.ParentWindow.mTopBar.mTitleIcon.Visibility = Visibility.Visible;
      else
        this.ParentWindow.mTopBar.mTitleIcon.Visibility = Visibility.Collapsed;
      if (this.ParentWindow.mTopBar.ActualWidth > this.ParentWindow.mTopBar.mMinimumExpectedTopBarWidth + 140.0 + num1 + (double) (this.mDictTabs.Count * 48))
        this.ParentWindow.mTopBar.mTitleTextGrid.Visibility = Visibility.Visible;
      else
        this.ParentWindow.mTopBar.mTitleTextGrid.Visibility = Visibility.Collapsed;
      int num2 = this.mPanel.Children.Count + this.mHiddenButtons.Children.Count;
      if (num2 > 0)
      {
        double tabWidth = (double) this.mTabMinWidth;
        if (this.AreaForTABS >= num2 * this.mTabMinWidth)
          tabWidth = (double) (this.AreaForTABS / num2);
        for (int index = 0; index < this.mPanel.Children.Count; ++index)
          (this.mPanel.Children[index] as AppTabButton).ResizeButton(tabWidth);
        if ((double) this.AreaForTABS >= tabWidth * (double) num2)
        {
          if (this.mHiddenButtons.Children.Count > 0)
            this.ShowXTabs(this.mHiddenButtons.Children.Count, tabWidth);
        }
        else
        {
          int num3 = this.AreaForTABS / this.mTabMinWidth - 1;
          if (Oem.IsOEMDmm)
          {
            int num4 = (int) Math.Floor(BlueStacksUIBinding.Instance.CornerRadiusModel["TabMarginPortrait"].TopLeft);
            num3 = (this.AreaForTABS - ((int) Math.Floor(this.mMoreTabButton.ActualWidth) + num4)) / (this.mTabMinWidth + num4);
          }
          if (num3 > num2)
            num3 = num2;
          if (num3 > this.mPanel.Children.Count || num2 == 1)
            this.ShowXTabs(num3 - this.mPanel.Children.Count, tabWidth);
          else if (num3 < this.mPanel.Children.Count)
            this.HideXTabs(this.mPanel.Children.Count - num3);
        }
      }
      if (this.mHiddenButtons.Children.Count > 0)
      {
        this.mMoreTabButton.Visibility = Visibility.Visible;
        this.mMoreTabButton.MoreTabsButtonHandling();
      }
      else
        this.mMoreTabButton.Visibility = Visibility.Hidden;
    }

    private double MacroGridHandling()
    {
      double num = 0.0;
      if (this.ParentWindow.mTopBar.mMacroRecordControl.Visibility == Visibility.Visible)
        num = this.ParentWindow.mTopBar.mMacroRecordControl.MaxWidth;
      else if (this.ParentWindow.mTopBar.mMacroPlayControl.Visibility == Visibility.Visible)
        num = this.ParentWindow.mTopBar.mMacroPlayControl.MaxWidth;
      if (num > 0.0)
      {
        if (this.ParentWindow.mTopBar.ActualWidth > this.ParentWindow.mTopBar.mMinimumExpectedTopBarWidth + num)
        {
          this.ParentWindow.mTopBar.mMacroRecordControl.TimerDisplay.Visibility = Visibility.Visible;
          this.ParentWindow.mTopBar.mMacroPlayControl.mDescriptionPanel.Visibility = Visibility.Visible;
        }
        else
        {
          this.ParentWindow.mTopBar.mMacroRecordControl.TimerDisplay.Visibility = Visibility.Collapsed;
          this.ParentWindow.mTopBar.mMacroPlayControl.mDescriptionPanel.Visibility = Visibility.Collapsed;
        }
      }
      return num;
    }

    private double VideoRecordingGridHandling()
    {
      double num = 0.0;
      if (this.ParentWindow.mTopBar.mVideoRecordStatusControl.Visibility == Visibility.Visible)
        num = this.ParentWindow.mTopBar.mVideoRecordStatusControl.MaxWidth;
      if (num > 0.0)
      {
        if (this.ParentWindow.mTopBar.ActualWidth > this.ParentWindow.mTopBar.mMinimumExpectedTopBarWidth + num)
          this.ParentWindow.mTopBar.mVideoRecordStatusControl.mDescriptionPanel.Visibility = Visibility.Visible;
        else
          this.ParentWindow.mTopBar.mVideoRecordStatusControl.mDescriptionPanel.Visibility = Visibility.Collapsed;
      }
      return num;
    }

    private void ShowXTabs(int x, double tabWidth)
    {
      for (int index = 0; index < x; ++index)
      {
        AppTabButton appTabButton1 = this.mDictTabs.Values.First<AppTabButton>();
        foreach (AppTabButton appTabButton2 in this.mDictTabs.Values)
        {
          if (this.mHiddenButtons.Children.Contains((UIElement) appTabButton2))
          {
            appTabButton1 = appTabButton2;
            break;
          }
        }
        appTabButton1.ResizeButton(tabWidth);
        appTabButton1.UpdateUIForDropDown(false);
        if (!this.mPanel.Children.Contains((UIElement) appTabButton1))
        {
          this.mHiddenButtons.Children.Remove((UIElement) appTabButton1);
          if (appTabButton1.mTabType == TabType.HomeTab)
            this.mPanel.Children.Insert(0, (UIElement) appTabButton1);
          else
            this.mPanel.Children.Add((UIElement) appTabButton1);
        }
      }
    }

    private void HideXTabs(int x)
    {
      for (int index1 = 0; index1 < x; ++index1)
      {
        AppTabButton appTabButton1 = this.mDictTabs.Values.Last<AppTabButton>();
        for (int index2 = this.mDictTabs.Count - 1; index2 >= 0; --index2)
        {
          AppTabButton appTabButton2 = this.mDictTabs.ElementAt<KeyValuePair<string, AppTabButton>>(index2).Value;
          if (this.mPanel.Children.Contains((UIElement) appTabButton2))
          {
            appTabButton1 = appTabButton2;
            break;
          }
        }
        appTabButton1.UpdateUIForDropDown(true);
        if (!this.mHiddenButtons.Children.Contains((UIElement) appTabButton1))
        {
          this.mPanel.Children.Remove((UIElement) appTabButton1);
          this.mHiddenButtons.Children.Add((UIElement) appTabButton1);
        }
      }
    }

    internal void CloseTab(
      string tabKey,
      bool sendStopAppToAndroid = false,
      bool forceClose = false,
      bool dontCheckQuitPopup = false,
      bool receivedFromImap = false,
      string topActivityPackageName = "")
    {
      if (!this.mDictTabs.ContainsKey(tabKey))
        return;
      if (this.ParentWindow.SendClientActions && !receivedFromImap)
      {
        Dictionary<string, string> data = new Dictionary<string, string>();
        Dictionary<string, string> dictionary = new Dictionary<string, string>()
        {
          {
            "EventAction",
            "TabClosed"
          },
          {
            nameof (tabKey),
            tabKey
          },
          {
            nameof (sendStopAppToAndroid),
            sendStopAppToAndroid.ToString((IFormatProvider) CultureInfo.InvariantCulture)
          },
          {
            nameof (forceClose),
            forceClose.ToString((IFormatProvider) CultureInfo.InvariantCulture)
          }
        };
        JsonSerializerSettings serializerSettings = Utils.GetSerializerSettings();
        serializerSettings.Formatting = Formatting.None;
        data.Add("operationData", JsonConvert.SerializeObject((object) dictionary, serializerSettings));
        this.ParentWindow.mFrontendHandler.SendFrontendRequestAsync("handleClientOperation", data);
      }
      AppTabButton mDictTab = this.mDictTabs[tabKey];
      if (mDictTab.mTabType == TabType.WebTab)
      {
        browserControl = (BrowserControl) null;
        foreach (object child in mDictTab.mControlGrid.Children)
        {
          if (child is BrowserControl browserControl)
            break;
        }
        string str = string.Empty;
        if (browserControl != null)
        {
          str = browserControl.mUrl;
          mDictTab.mControlGrid.Children.Remove((UIElement) browserControl);
          if (browserControl.CefBrowser != null)
          {
            foreach (BrowserControlTags key in browserControl.TagsSubscribedDict.Keys)
              browserControl.mSubscriber?.UnsubscribeTag(key);
            browserControl.CefBrowser.Dispose();
          }
        }
        ClientStats.SendMiscellaneousStatsAsync("WebTabClosed", RegistryManager.Instance.UserGuid, str, mDictTab.AppLabel, RegistryManager.Instance.Version, Oem.Instance.OEM, (string) null, (string) null, (string) null, "Android");
      }
      if (FeatureManager.Instance.IsCheckForQuitPopup && !RegistryManager.Instance.Guest[this.ParentWindow.mVmName].IsGoogleSigninDone && (mDictTab.mTabType == TabType.AppTab && mDictTab.PackageName == "com.android.vending"))
      {
        QuitPopupControl quitPopupControl = new QuitPopupControl(this.ParentWindow);
        string tag = "exit_popup_ots";
        quitPopupControl.CurrentPopupTag = tag;
        BlueStacksUIBinding.Bind(quitPopupControl.TitleTextBlock, "STRING_YOU_ARE_ONE_STEP_AWAY", "");
        BlueStacksUIBinding.Bind((Button) quitPopupControl.mCloseBlueStacksButton, "STRING_CLOSE_TAB");
        quitPopupControl.AddQuitActionItem(QuitActionItem.WhyGoogleAccount);
        quitPopupControl.AddQuitActionItem(QuitActionItem.TroubleSigningIn);
        quitPopupControl.AddQuitActionItem(QuitActionItem.SomethingElseWrong);
        quitPopupControl.CloseBlueStacksButton.PreviewMouseUp += (MouseButtonEventHandler) ((sender, e) => this.CloseTabAfterQuitPopup(tabKey, sendStopAppToAndroid, forceClose));
        quitPopupControl.CrossButtonPictureBox.PreviewMouseUp += (MouseButtonEventHandler) ((sender, e) =>
        {
          if (!string.Equals(topActivityPackageName, "com.bluestacks.appmart", StringComparison.InvariantCulture))
            return;
          this.CloseTabAfterQuitPopup(tabKey, sendStopAppToAndroid, forceClose);
        });
        this.ParentWindow.HideDimOverlay();
        this.ParentWindow.ShowDimOverlay((IDimOverlayControl) quitPopupControl);
        ClientStats.SendLocalQuitPopupStatsAsync(tag, "popup_shown");
      }
      else if (!Oem.IsOEMDmm && !dontCheckQuitPopup && (mDictTab.mTabType == TabType.AppTab && tabKey != this.mLastPackageForQuitPopupDisplayed) && (!this.ParentWindow.SendClientActions && !receivedFromImap) && this.ParentWindow.mWelcomeTab.mHomeAppManager.CheckDictAppIconFor(tabKey, (Predicate<AppIconModel>) (_ => _.IsInstalledApp)) && this.ParentWindow.mWelcomeTab.mHomeAppManager.CheckDictAppIconFor(tabKey, (Predicate<AppIconModel>) (_ => !_.IsAppSuggestionActive)))
      {
        ProgressBar progressBar = new ProgressBar();
        progressBar.ProgressText = "STRING_LOADING_MESSAGE";
        progressBar.Visibility = Visibility.Hidden;
        this.ParentWindow.ShowDimOverlay((IDimOverlayControl) progressBar);
        this.mLastPackageForQuitPopupDisplayed = tabKey;
        new Thread((ThreadStart) (() =>
        {
          if (this.ParentWindow.Utils.CheckQuitPopupFromCloud(tabKey))
            return;
          this.Dispatcher.Invoke((Delegate) (() => this.CloseTabAfterQuitPopup(tabKey, sendStopAppToAndroid, forceClose)));
        }))
        {
          IsBackground = true
        }.Start();
      }
      else
        this.CloseTabAfterQuitPopup(tabKey, sendStopAppToAndroid, forceClose);
    }

    internal void CloseTabAfterQuitPopup(string tabKey, bool sendStopAppToAndroid, bool forceClose)
    {
      try
      {
        Logger.Info("CloseTab after quitpopup, key: {0}, sendStopApp: {1}, forceClose: {2}", (object) tabKey, (object) sendStopAppToAndroid, (object) forceClose);
        if (!this.mDictTabs.ContainsKey(tabKey))
          return;
        Logger.Info("mDict Tab contains key");
        AppTabButton mDictTab = this.mDictTabs[tabKey];
        if (mDictTab.mTabType != TabType.HomeTab)
        {
          Logger.Info("Button was not hometab");
          if (this.ParentWindow.mDimOverlay != null && this.ParentWindow.mDimOverlay.Control != null)
          {
            Logger.Info("DimOverlay and control exist");
            if (FeatureManager.Instance.IsCustomUIForNCSoft && (object) this.ParentWindow.mDimOverlay.Control.GetType() == (object) this.ParentWindow.ScreenLockInstance.GetType() || !FeatureManager.Instance.IsCustomUIForNCSoft)
            {
              Logger.Info("hiding");
              this.ParentWindow.HideDimOverlay();
              this.mPopup.IsOpen = false;
            }
          }
        }
        this.mLastPackageForQuitPopupDisplayed = "";
        Logger.Info("Trying non hometab");
        if (!(mDictTab.mTabType != TabType.HomeTab | forceClose))
          return;
        Logger.Info("Button is not hometab or forceclose");
        Publisher.PublishMessage(BrowserControlTags.tabClosing, this.ParentWindow.mVmName, new JObject((object) new JProperty("PackageName", (object) mDictTab.PackageName)));
        (mDictTab.Parent as Panel).Children.Remove((UIElement) mDictTab);
        this.mDictTabs.Remove(tabKey);
        Logger.Info("XXXSR: Tab removed");
        if (mDictTab.mTabType == TabType.AppTab || mDictTab.mTabType == TabType.HomeTab)
          this.ParentWindow.mCommonHandler.ToggleMacroAndSyncVisibility();
        if (sendStopAppToAndroid && mDictTab.mTabType == TabType.AppTab)
          this.ParentWindow.mAppHandler.StopAppRequest(mDictTab.PackageName);
        Logger.Info("MacroAndSync gone and StopApp sent");
        this.ListTabHistory.RemoveAll((Predicate<string>) (n => n.Equals(tabKey, StringComparison.OrdinalIgnoreCase)));
        Logger.Info("TabHistory cleared");
        if (this.ParentWindow.mDiscordhandler != null)
          this.ParentWindow.mDiscordhandler.RemoveAppFromTimestampList(tabKey);
        if (mDictTab.mTabType == TabType.AppTab)
          GrmHandler.RefreshGrmIndicationForAllInstances(mDictTab.PackageName);
        Publisher.PublishMessage(BrowserControlTags.currentlyRunningApps, this.ParentWindow.mVmName, new JObject((object) new JProperty("packages", (object) new JArray((object) this.mDictTabs.Values.Select<AppTabButton, string>((Func<AppTabButton, string>) (_ => _.TabKey))))));
        if (Oem.IsOEMDmm && this.ListTabHistory.Count == 0)
        {
          this.ParentWindow.Hide();
          this.ParentWindow.RestoreWindows(false);
          if (this.ParentWindow.mDMMRecommendedWindow != null)
            this.ParentWindow.mDMMRecommendedWindow.Visibility = Visibility.Hidden;
          this.ParentWindow.StaticComponents.mSelectedTabButton.IsPortraitModeTab = false;
        }
        else if (mDictTab.IsSelected)
        {
          Logger.Info("Button was selected");
          if (this.ListTabHistory.Count != 0)
          {
            Logger.Info("goto tab");
            this.GoToTab(this.ListTabHistory[this.ListTabHistory.Count - 1], true, false);
          }
          else
            Logger.Fatal("No tab to go back to! Ignoring");
        }
        Logger.Info("Resizing tabs");
        this.ResizeTabs();
      }
      catch (Exception ex)
      {
        Logger.Error("XXXSR-UFD945 Couldn't close tab after quit popup.Ex: {0}", (object) ex);
      }
    }

    internal bool GoToTab(string key, bool isLaunch = true, bool receivedFromImap = false)
    {
      Logger.Info("Test logs: GoToTab() key: " + key + ", isPresentInmDict: " + this.mDictTabs.ContainsKey(key).ToString());
      bool flag = false;
      if (InteropWindow.GetForegroundWindow() != this.ParentWindow.Handle)
        this.ParentWindow.mIsFocusComeFromImap = true;
      if (this.mDictTabs.ContainsKey(key))
      {
        if (Oem.IsOEMDmm && this.ParentWindow.mFrontendGrid.Visibility != Visibility.Visible)
        {
          this.ParentWindow.mFrontendGrid.Visibility = Visibility.Visible;
          this.ParentWindow.mDmmProgressControl.Visibility = Visibility.Hidden;
        }
        AppTabButton mDictTab = this.mDictTabs[key];
        if (!mDictTab.IsSelected)
        {
          mDictTab.IsLaunchOnSelection = isLaunch;
          mDictTab.mIsAnyOperationPendingForTab = KMManager.sGuidanceWindow != null && GuidanceWindow.sIsDirty;
          mDictTab.Select(true, receivedFromImap);
          flag = true;
          EventHandler<TabChangeEventArgs> eventOnTabChanged = mDictTab.EventOnTabChanged;
          if (eventOnTabChanged != null)
            eventOnTabChanged((object) null, new TabChangeEventArgs(mDictTab.AppName, mDictTab.PackageName, mDictTab.mTabType));
        }
        else
          flag = true;
      }
      return flag;
    }

    internal bool GoToTab(int index)
    {
      return this.mDictTabs.Count > index && this.GoToTab(this.mPanel.Children.OfType<AppTabButton>().Last<AppTabButton>().TabKey, true, false);
    }

    internal AppTabButton GetTab(string packageName)
    {
      return this.mDictTabs.ContainsKey(packageName) ? this.mDictTabs[packageName] : (AppTabButton) null;
    }

    private void MoreTabButton_Click(object sender, RoutedEventArgs e)
    {
      this.mPopup.IsOpen = true;
    }

    private void NotificationPopup_Opened(object sender, EventArgs e)
    {
      this.mMoreTabButton.IsEnabled = false;
    }

    private void NotificationPopup_Closed(object sender, EventArgs e)
    {
      this.mMoreTabButton.IsEnabled = true;
    }

    private void NotificaitonPopup_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      new Thread((ThreadStart) (() =>
      {
        Thread.Sleep(100);
        this.Dispatcher.Invoke((Delegate) (() => this.mPopup.IsOpen = false));
      }))
      {
        IsBackground = true
      }.Start();
    }

    internal void EnableAppTabs(bool isEnableTab)
    {
      foreach (KeyValuePair<string, AppTabButton> mDictTab in this.mDictTabs)
      {
        if (mDictTab.Value.mTabType == TabType.AppTab)
          mDictTab.Value.IsEnabled = isEnableTab;
      }
    }

    internal bool IsAppRunning()
    {
      foreach (KeyValuePair<string, AppTabButton> mDictTab in this.mDictTabs)
      {
        if (mDictTab.Value.mTabType == TabType.AppTab)
          return true;
      }
      return false;
    }

    internal void RestartTab(string package)
    {
      this.Dispatcher.Invoke((Delegate) (() => this.CloseTab(package, true, true, true, false, "")));
      Thread.Sleep(1000);
      this.Dispatcher.Invoke((Delegate) (() => this.ParentWindow.mWelcomeTab.mHomeAppManager.OpenApp(package, false)));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/apptabbuttons.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    internal Delegate _CreateDelegate(Type delegateType, string handler)
    {
      return Delegate.CreateDelegate(delegateType, (object) this, handler);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.mPanel = (StackPanel) target;
          break;
        case 2:
          this.mMoreTabButton = (AppTabButton) target;
          break;
        case 3:
          this.mPopup = (CustomPopUp) target;
          break;
        case 4:
          this.mMaskBorder = (Border) target;
          break;
        case 5:
          this.mHiddenButtons = (StackPanel) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }

    private enum TabMode
    {
      ParallelogramMode,
      IconMode,
    }
  }
}
