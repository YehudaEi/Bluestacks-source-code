// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.TopBar
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.BlueStacksUI.BTv;
using BlueStacks.Common;
using Microsoft.VisualBasic.Devices;
using Newtonsoft.Json.Linq;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace BlueStacks.BlueStacksUI
{
  public class TopBar : UserControl, ITopBar, IComponentConnector
  {
    private SortedList<int, KeyValuePair<FrameworkElement, double>> mOptionsPriorityPanel = new SortedList<int, KeyValuePair<FrameworkElement, double>>();
    internal double mMinimumExpectedTopBarWidth = 320.0;
    private ulong MB_MULTIPLIER = 1048576;
    private MainWindow mMainWindow;
    internal PerformanceState mSnailMode;
    private DispatcherTimer mMacroRunningPopupTimer;
    private DispatcherTimer mMacroRecordingPopupTimer;
    internal Grid mMainGrid;
    internal Grid WindowHeaderGrid;
    internal CustomPictureBox mTitleIcon;
    internal Grid mTitleTextGrid;
    internal TextBlock mTitleText;
    internal TextBlock mVersionText;
    internal DockPanel mOptionsDockPanel;
    internal CustomPictureBox mSidebarButton;
    internal CustomPictureBox mCloseButton;
    internal CustomPictureBox mMaximizeButton;
    internal CustomPictureBox mMinimizeButton;
    internal Grid mConfigButtonGrid;
    internal CustomPictureBox mConfigButton;
    internal Ellipse mSettingsBtnNotification;
    internal CustomPopUp mSettingsMenuPopup;
    internal Border mPreferenceDropDownBorder;
    internal Grid mGrid;
    internal Border mMaskBorder;
    internal PreferenceDropDownControl mPreferenceDropDownControl;
    internal CustomPictureBox mHelpButton;
    internal CustomPictureBox mUserAccountBtn;
    internal Grid mNotificationGrid;
    internal CustomPictureBox mNotificationCentreButton;
    internal Canvas mNotificationCountBadge;
    internal CustomPopUp mNotificationCentrePopup;
    internal Path mNotificationCaret;
    internal Border mNotificationCentreDropDownBorder;
    internal Border mMaskBorder1;
    internal NotificationDrawer mNotificationDrawerControl;
    internal CustomPictureBox mBtvButton;
    internal CustomPictureBox mWarningButton;
    internal Grid mOperationsSyncGrid;
    internal Border mSyncMaskBorder;
    internal CustomPictureBox mPlayPauseSyncButton;
    internal CustomPictureBox mStopSyncButton;
    internal CustomPopUp mSyncInstancesToolTipPopup;
    internal Grid mDummyGrid;
    internal Border mMaskBorder2;
    internal Path mUpwardArrow;
    internal CustomPictureBox mLocalConfigIndicator;
    internal AppTabButtons mAppTabButtons;
    internal Grid mVideoRecordingStatusGrid;
    internal VideoRecordingStatus mVideoRecordStatusControl;
    internal Grid mMacroGrid;
    internal MacroTopBarRecordControl mMacroRecordControl;
    internal MacroTopBarPlayControl mMacroPlayControl;
    internal CustomPopUp mMacroRecorderToolTipPopup;
    internal Grid dummyGrid;
    internal Border mMaskBorder3;
    internal TextBlock mMacroRecordingTooltip;
    internal Path mUpArrow;
    internal CustomPopUp mMacroRunningToolTipPopup;
    internal Grid grid;
    internal Border mMaskBorder4;
    internal TextBlock mMacroRunningTooltip;
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

    public event PercentageChangedEventHandler PercentChanged;

    string ITopBar.AppName
    {
      get
      {
        return (string) null;
      }
      set
      {
      }
    }

    string ITopBar.CharacterName
    {
      get
      {
        return (string) null;
      }
      set
      {
      }
    }

    public static Point GetMousePosition()
    {
      NativeMethods.Win32Point pt = new NativeMethods.Win32Point();
      NativeMethods.GetCursorPos(ref pt);
      return new Point((double) pt.X, (double) pt.Y);
    }

    public TopBar()
    {
      this.InitializeComponent();
      if (FeatureManager.Instance.IsCustomUIForDMMSandbox)
      {
        this.TopBarOptionsPanelElementVisibility((FrameworkElement) this.mUserAccountBtn, false);
        this.TopBarOptionsPanelElementVisibility((FrameworkElement) this.mWarningButton, false);
        this.TopBarOptionsPanelElementVisibility((FrameworkElement) this.mHelpButton, false);
      }
      else
      {
        if (!FeatureManager.Instance.IsUserAccountBtnEnabled)
          this.TopBarOptionsPanelElementVisibility((FrameworkElement) this.mUserAccountBtn, false);
        if (!FeatureManager.Instance.IsWarningBtnEnabled)
          this.TopBarOptionsPanelElementVisibility((FrameworkElement) this.mWarningButton, false);
        this.TopBarOptionsPanelElementVisibility((FrameworkElement) this.mHelpButton, FeatureManager.Instance.IsTopbarHelpEnabled);
      }
      if (Oem.IsOEMDmm)
      {
        this.mConfigButton.Visibility = Visibility.Collapsed;
        this.TopBarOptionsPanelElementVisibility((FrameworkElement) this.mNotificationGrid, false);
        this.WindowHeaderGrid.Visibility = Visibility.Collapsed;
        this.TopBarOptionsPanelElementVisibility((FrameworkElement) this.mUserAccountBtn, false);
        this.mWarningButton.ToolTip = (object) null;
        this.mSidebarButton.Visibility = Visibility.Collapsed;
        this.TopBarOptionsPanelElementVisibility((FrameworkElement) this.mHelpButton, false);
      }
      if (RegistryManager.Instance.InstallationType == InstallationTypes.GamingEdition)
      {
        this.TopBarOptionsPanelElementVisibility((FrameworkElement) this.mUserAccountBtn, false);
        this.TopBarOptionsPanelElementVisibility((FrameworkElement) this.mNotificationGrid, false);
      }
      if (!string.Equals(this.mTitleIcon.ImageName, BlueStacks.Common.Strings.TitleBarIconImageName, StringComparison.InvariantCulture))
        this.mTitleIcon.ImageName = BlueStacks.Common.Strings.TitleBarIconImageName;
      double? nullable = BlueStacks.Common.Strings.TitleBarProductIconWidth;
      if (nullable.HasValue)
      {
        CustomPictureBox mTitleIcon = this.mTitleIcon;
        nullable = BlueStacks.Common.Strings.TitleBarProductIconWidth;
        double num = nullable.Value;
        mTitleIcon.Width = num;
      }
      nullable = BlueStacks.Common.Strings.TitleBarTextMaxWidth;
      if (nullable.HasValue)
      {
        TextBlock mTitleText = this.mTitleText;
        nullable = BlueStacks.Common.Strings.TitleBarTextMaxWidth;
        double num = nullable.Value;
        mTitleText.MaxWidth = num;
      }
      this.mVersionText.Text = RegistryManager.Instance.ClientVersion;
    }

    private void ParentWindow_GuestBootCompletedEvent(object sender, EventArgs args)
    {
      if (!this.ParentWindow.EngineInstanceRegistry.IsSidebarVisible || this.Visibility != Visibility.Visible || (this.ParentWindow.mSidebar.Visibility == Visibility.Visible || Oem.IsOEMDmm))
        return;
      this.ParentWindow.Dispatcher.Invoke((Delegate) (() => this.ParentWindow.mCommonHandler.FlipSidebarVisibility(this.mSidebarButton, (TextBlock) null)));
    }

    internal void ChangeDownloadPercent(int percent)
    {
      PercentageChangedEventHandler percentChanged = this.PercentChanged;
      if (percentChanged == null)
        return;
      percentChanged((object) this, new PercentageChangedEventArgs()
      {
        Percentage = percent
      });
    }

    internal void InitializeSnailButton()
    {
      if (FeatureManager.Instance.IsCustomUIForDMMSandbox || !FeatureManager.Instance.IsWarningBtnEnabled)
        return;
      string deviceCaps = RegistryManager.Instance.DeviceCaps;
      if (string.IsNullOrEmpty(deviceCaps))
      {
        this.mSnailMode = PerformanceState.VtxEnabled;
        this.TopBarOptionsPanelElementVisibility((FrameworkElement) this.mWarningButton, false);
      }
      else
      {
        JObject deviceCapsData = JObject.Parse(deviceCaps);
        this.Dispatcher.Invoke((Delegate) (() =>
        {
          if (deviceCapsData["cpu_hvm"].ToString().Equals("True", StringComparison.OrdinalIgnoreCase) && deviceCapsData["bios_hvm"].ToString().Equals("False", StringComparison.OrdinalIgnoreCase))
          {
            if (deviceCapsData["engine_enabled"].ToString().Equals(EngineState.raw.ToString(), StringComparison.OrdinalIgnoreCase))
            {
              this.mSnailMode = PerformanceState.VtxDisabled;
              this.TopBarOptionsPanelElementVisibility((FrameworkElement) this.mWarningButton, true);
            }
          }
          else if (deviceCapsData["cpu_hvm"].ToString().Equals("False", StringComparison.OrdinalIgnoreCase))
          {
            this.mSnailMode = PerformanceState.NoVtxSupport;
            this.TopBarOptionsPanelElementVisibility((FrameworkElement) this.mWarningButton, true);
          }
          else if (deviceCapsData["cpu_hvm"].ToString().Equals("True", StringComparison.OrdinalIgnoreCase) && deviceCapsData["bios_hvm"].ToString().Equals("True", StringComparison.OrdinalIgnoreCase))
          {
            this.mSnailMode = PerformanceState.VtxEnabled;
            this.TopBarOptionsPanelElementVisibility((FrameworkElement) this.mWarningButton, false);
          }
          this.RefreshWarningButton();
        }));
      }
    }

    internal void RefreshWarningButton()
    {
      if (FeatureManager.Instance.IsCustomUIForDMMSandbox || !FeatureManager.Instance.IsWarningBtnEnabled)
        return;
      if (this.mSnailMode != PerformanceState.VtxEnabled)
      {
        this.TopBarOptionsPanelElementVisibility((FrameworkElement) this.mWarningButton, true);
        this.AddVtxNotification();
      }
      else
        this.TopBarOptionsPanelElementVisibility((FrameworkElement) this.mWarningButton, false);
    }

    internal void AddVtxNotification()
    {
      if (Oem.IsOEMDmm)
        return;
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        bool dontOverwrite = true;
        GenericNotificationItem notificationItem = new GenericNotificationItem()
        {
          CreationTime = DateTime.Now,
          IsDeferred = false,
          Priority = NotificationPriority.Important,
          ShowRibbon = false,
          Id = "VtxNotification",
          NotificationMenuImageName = "SlowPerformance.png",
          Title = LocaleStrings.GetLocalizedString("STRING_DISABLED_VT_TITLE", ""),
          Message = LocaleStrings.GetLocalizedString("STRING_DISABLED_VT", "")
        };
        SerializableDictionary<string, string> serializableDictionary = new SerializableDictionary<string, string>()
        {
          {
            "click_generic_action",
            "UserBrowser"
          },
          {
            "click_action_value",
            WebHelper.GetUrlWithParams(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) WebHelper.GetServerHost(), (object) "help_articles"), (string) null, (string) null, (string) null) + "&article=enable_virtualization"
          }
        };
        notificationItem.ExtraPayload.ClearAddRange<string, string>((Dictionary<string, string>) serializableDictionary);
        GenericNotificationManager.AddNewNotification(notificationItem, dontOverwrite);
        this.RefreshNotificationCentreButton();
      }));
    }

    internal void AddRamNotification()
    {
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        bool dontOverwrite = true;
        GenericNotificationItem notificationItem = new GenericNotificationItem()
        {
          IsDeferred = false,
          Priority = NotificationPriority.Important,
          ShowRibbon = false,
          Id = "ramNotification",
          NotificationMenuImageName = "SlowPerformance.png",
          Title = LocaleStrings.GetLocalizedString("STRING_RAM_NOTIF_TITLE", ""),
          Message = LocaleStrings.GetLocalizedString("STRING_RAM_NOTIF", "")
        };
        SerializableDictionary<string, string> serializableDictionary = new SerializableDictionary<string, string>()
        {
          {
            "click_generic_action",
            "UserBrowser"
          },
          {
            "click_action_value",
            WebHelper.GetUrlWithParams(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) WebHelper.GetServerHost(), (object) "help_articles"), (string) null, (string) null, (string) null) + "&article=bs3_nougat_min_requirements"
          }
        };
        notificationItem.ExtraPayload.ClearAddRange<string, string>((Dictionary<string, string>) serializableDictionary);
        GenericNotificationManager.AddNewNotification(notificationItem, dontOverwrite);
        this.RefreshNotificationCentreButton();
      }));
    }

    private void UserAccountButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      Logger.Info("Clicked account button");
      if (!this.ParentWindow.mGuestBootCompleted || !this.ParentWindow.mAppHandler.IsOneTimeSetupCompleted)
        return;
      if (FeatureManager.Instance.IsOpenActivityFromAccountIcon)
        this.mAppTabButtons.AddAppTab("STRING_ACCOUNT", BlueStacksUIUtils.sUserAccountPackageName, BlueStacksUIUtils.sUserAccountActivityName, "account_tab", true, true, false);
      else
        this.mAppTabButtons.AddWebTab(WebHelper.GetUrlWithParams(WebHelper.GetServerHost() + "/bluestacks_account", (string) null, (string) null, (string) null) + "&email=" + RegistryManager.Instance.RegisteredEmail + "&token=" + RegistryManager.Instance.Token, "STRING_ACCOUNT", "account_tab", true, "account_tab", false);
    }

    private void ConfigButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.mPreferenceDropDownControl.LateInit();
      this.mSettingsMenuPopup.IsOpen = true;
      this.mSettingsMenuPopup.HorizontalOffset = -(this.mPreferenceDropDownBorder.ActualWidth - 40.0);
      this.mConfigButton.ImageName = "cfgmenu_hover";
    }

    private void MinimizeButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      Logger.Info("Clicked minimize button");
      this.ParentWindow.MinimizeWindow();
    }

    internal void MaxmizeButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      Logger.Info("Clicked Maximize\\Restore button");
      if (this.ParentWindow.WindowState == WindowState.Normal && !this.ParentWindow.mIsDmmMaximised)
        this.ParentWindow.MaximizeWindow();
      else
        this.ParentWindow.RestoreWindows(false);
    }

    internal void SetConfigIndicator(string config)
    {
      this.mLocalConfigIndicator.Visibility = string.Equals(config, ".config_user.db", StringComparison.InvariantCultureIgnoreCase) ? Visibility.Visible : Visibility.Collapsed;
    }

    private void CloseButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      Logger.Info("Clicked close Bluestacks button");
      BlueStacks.Common.Stats.SendCommonClientStatsAsync("notification_mode", "BlueStacks_close", this.ParentWindow.mVmName, "", "", "", "");
      if (RegistryManager.Instance.IsNotificationModeAlwaysOn && string.Compare("Android", this.ParentWindow.mVmName, StringComparison.InvariantCultureIgnoreCase) == 0)
      {
        if (this.ParentWindow.Utils.CheckQuitPopupLocal())
          return;
        BlueStacks.Common.Stats.SendCommonClientStatsAsync("notification_mode", "notification_mode", this.ParentWindow.mVmName, string.Empty, "on", "", "");
        this.ParentWindow.EngineInstanceRegistry.IsMinimizeSelectedOnReceiveGameNotificationPopup = true;
        this.ParentWindow.IsInNotificationMode = true;
        foreach (string key in this.ParentWindow.AppNotificationCountDictForEachVM.Keys)
          BlueStacks.Common.Stats.SendCommonClientStatsAsync("notification_mode", "notification_number", this.ParentWindow.mVmName, key, this.ParentWindow.AppNotificationCountDictForEachVM[key].ToString((IFormatProvider) CultureInfo.InvariantCulture), "NM_Off", "");
        this.ParentWindow.AppNotificationCountDictForEachVM.Clear();
        this.ParentWindow.MinimizeWindowHandler();
      }
      else
      {
        BlueStacks.Common.Stats.SendCommonClientStatsAsync("notification_mode", "notification_mode", this.ParentWindow.mVmName, string.Empty, "off", "", "");
        this.ParentWindow.CloseWindow();
      }
    }

    private void NotificationPopup_Opened(object sender, EventArgs e)
    {
      this.mConfigButton.IsEnabled = false;
    }

    private void NotificationPopup_Closed(object sender, EventArgs e)
    {
      this.mConfigButton.IsEnabled = true;
      this.mConfigButton.ImageName = "cfgmenu";
    }

    internal void ChangeUserPremiumButton(bool isPremium)
    {
      if (isPremium)
        this.mUserAccountBtn.ImageName = BlueStacksUIUtils.sPremiumUserImageName;
      else
        this.mUserAccountBtn.ImageName = BlueStacksUIUtils.sLoggedInImageName;
    }

    private void PreferenceDropDownControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      e.Handled = true;
    }

    private void mWarningButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      Logger.Info("Clicked warning button for speed up Bluestacks ");
      this.mWarningButton.ImageName = "warning";
      SpeedUpBlueStacks speedUpBlueStacks = new SpeedUpBlueStacks();
      if (this.mSnailMode == PerformanceState.NoVtxSupport)
        speedUpBlueStacks.mUpgradeComputer.Visibility = Visibility.Visible;
      else if (this.mSnailMode == PerformanceState.VtxDisabled)
        speedUpBlueStacks.mEnableVt.Visibility = Visibility.Visible;
      ContainerWindow containerWindow = new ContainerWindow(this.ParentWindow, (UserControl) speedUpBlueStacks, 640.0, 200.0, false, true, false, -1.0, (Brush) null);
    }

    private void mBtvButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      Logger.Info("Clicked btv button");
      BTVManager.Instance.StartBlueStacksTV();
    }

    private void TopBar_Loaded(object sender, RoutedEventArgs e)
    {
      if (FeatureManager.Instance.IsBTVEnabled && string.Equals(BlueStacks.Common.Strings.CurrentDefaultVmName, this.ParentWindow.mVmName, StringComparison.InvariantCulture))
        this.TopBarOptionsPanelElementVisibility((FrameworkElement) this.mBtvButton, true);
      this.RefreshNotificationCentreButton();
      if (!this.ParentWindow.mGuestBootCompleted)
      {
        this.ParentWindow.mCommonHandler.SetSidebarImageProperties(false, this.mSidebarButton, (TextBlock) null);
        this.ParentWindow.GuestBootCompleted += new MainWindow.GuestBootCompletedEventHandler(this.ParentWindow_GuestBootCompletedEvent);
      }
      this.ParentWindow.mCommonHandler.ScreenRecordingStateChangedEvent += new CommonHandlers.ScreenRecordingStateChanged(this.TopBar_ScreenRecordingStateChangedEvent);
      this.mVideoRecordStatusControl.RecordingStoppedEvent += new System.Action(this.TopBar_RecordingStoppedEvent);
      if (!(this.ParentWindow.mVmName == "Android") || !this.mTitleIcon.ToolTip.ToString().Equals(BlueStacks.Common.Strings.ProductTopBarDisplayName, StringComparison.OrdinalIgnoreCase))
        return;
      CustomPictureBox mTitleIcon = this.mTitleIcon;
      ToolTip toolTip = new ToolTip();
      toolTip.Content = (object) (BlueStacks.Common.Strings.ProductDisplayName ?? "");
      mTitleIcon.ToolTip = (object) toolTip;
    }

    private void TopBar_RecordingStoppedEvent()
    {
      this.ParentWindow.Dispatcher.Invoke((Delegate) (() => this.mVideoRecordStatusControl.Visibility = Visibility.Collapsed));
    }

    private void TopBar_ScreenRecordingStateChangedEvent(bool isRecording)
    {
      this.ParentWindow.Dispatcher.Invoke((Delegate) (() =>
      {
        if (isRecording)
        {
          if (this.mVideoRecordStatusControl.Visibility == Visibility.Visible || !CommonHandlers.sIsRecordingVideo)
            return;
          this.mVideoRecordStatusControl.Init(this.ParentWindow);
          this.mVideoRecordStatusControl.Visibility = Visibility.Visible;
        }
        else
        {
          this.mVideoRecordStatusControl.ResetTimer();
          this.mVideoRecordStatusControl.Visibility = Visibility.Collapsed;
        }
      }));
    }

    public void mNotificationCentreButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      Logger.Info("Clicked notification_centre button");
      this.mNotificationDrawerControl.Width = 320.0;
      SerializableDictionary<string, GenericNotificationItem> notificationItems = GenericNotificationManager.GetNotificationItems((Predicate<GenericNotificationItem>) (x =>
      {
        if (x.IsDeleted)
          return false;
        return string.Equals(x.VmName, this.ParentWindow.mVmName, StringComparison.InvariantCulture) || !x.IsAndroidNotification;
      }));
      this.mNotificationDrawerControl.Populate(notificationItems);
      ClientStats.SendMiscellaneousStatsAsync("NotificationBellIconClicked", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, (string) null, (string) null, (string) null, (string) null, (string) null, (string) null, "Android");
      GenericNotificationManager.MarkNotification((IEnumerable<string>) notificationItems.Keys, (System.Action<GenericNotificationItem>) (x =>
      {
        if (!x.IsReceivedStatSent || x.IsDeleted || (x.IsShown || x.IsAndroidNotification))
          return;
        x.IsShown = true;
        ClientStats.SendMiscellaneousStatsAsync("notification_shown", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, x.Id, x.Title, x.ExtraPayload.ContainsKey("campaign_id") ? x.ExtraPayload["campaign_id"] : "", (string) null, (string) null, (string) null, "Android");
      }));
      this.mNotificationDrawerControl.UpdateNotificationCount();
      if (sender != null)
      {
        this.mNotificationCentreButton.ImageName = "notification";
        this.mNotificationCountBadge.Visibility = Visibility.Collapsed;
      }
      else
        NotificationDrawer.DrawerAnimationTimer.Start();
      this.mNotificationCentrePopup.IsOpen = true;
      this.mNotificationDrawerControl.mNotificationScroll.ScrollToTop();
      this.mNotificationCentreButton.ImageName = "notification_hover";
    }

    internal bool CheckForRam()
    {
      int num = 0;
      try
      {
        num = (int) (ulong.Parse(new ComputerInfo().TotalPhysicalMemory.ToString((IFormatProvider) CultureInfo.InvariantCulture), (IFormatProvider) CultureInfo.InvariantCulture) / this.MB_MULTIPLIER);
      }
      catch (Exception ex)
      {
        Logger.Error(ex.ToString());
      }
      return num < 4096;
    }

    internal void RefreshNotificationCentreButton()
    {
      if (this.ParentWindow.EngineInstanceRegistry.IsGoogleSigninDone && FeatureManager.Instance.IsShowNotificationCentre && RegistryManager.Instance.InstallationType != InstallationTypes.GamingEdition)
      {
        this.TopBarOptionsPanelElementVisibility((FrameworkElement) this.mNotificationGrid, true);
        this.mNotificationCentreButton.ImageName = GenericNotificationManager.GetNotificationItems((Predicate<GenericNotificationItem>) (x => !x.IsRead && !x.IsDeleted && x.Priority == NotificationPriority.Important)).Count <= 0 ? "notification" : "notification_crucial";
        this.mNotificationDrawerControl.UpdateNotificationCount();
      }
      else
        this.TopBarOptionsPanelElementVisibility((FrameworkElement) this.mNotificationGrid, false);
    }

    internal void mNotificationCentreDropDownBorder_LayoutUpdated(object sender, EventArgs e)
    {
      RectangleGeometry rectangleGeometry = new RectangleGeometry();
      Rect rect = new Rect()
      {
        Height = this.mNotificationCentreDropDownBorder.ActualHeight,
        Width = this.mNotificationCentreDropDownBorder.ActualWidth
      };
      BlueStacksUIBinding.BindCornerRadiusToDouble((DependencyObject) rectangleGeometry, RectangleGeometry.RadiusXProperty, "PreferenceDropDownRadius");
      BlueStacksUIBinding.BindCornerRadiusToDouble((DependencyObject) rectangleGeometry, RectangleGeometry.RadiusYProperty, "PreferenceDropDownRadius");
      rectangleGeometry.Rect = rect;
      this.mNotificationCentreDropDownBorder.Clip = (Geometry) rectangleGeometry;
    }

    internal void ShowRecordingIcons()
    {
      this.mMacroRecordControl.Init(this.ParentWindow);
      this.mMacroRecordControl.Visibility = Visibility.Visible;
      this.mMacroRecordControl.StartTimer();
      if (this.ParentWindow.mIsFullScreen)
        return;
      this.ParentWindow.mTopBar.mMacroRecorderToolTipPopup.IsOpen = true;
      this.ParentWindow.mTopBar.mMacroRecorderToolTipPopup.StaysOpen = true;
      this.mMacroRecordingPopupTimer = new DispatcherTimer()
      {
        Interval = new TimeSpan(0, 0, 0, 5, 0)
      };
      this.mMacroRecordingPopupTimer.Tick += new EventHandler(this.MacroRecordingPopupTimer_Tick);
      this.mMacroRecordingPopupTimer.Start();
    }

    private void MacroRecordingPopupTimer_Tick(object sender, EventArgs e)
    {
      this.ParentWindow.mTopBar.mMacroRecorderToolTipPopup.IsOpen = false;
      (sender as DispatcherTimer).Stop();
    }

    internal void HideRecordingIcons()
    {
      this.mConfigButton.Visibility = Visibility.Visible;
      if (this.ParentWindow.EngineInstanceRegistry.IsGoogleSigninDone)
      {
        this.TopBarOptionsPanelElementVisibility((FrameworkElement) this.mNotificationGrid, true);
        this.TopBarOptionsPanelElementVisibility((FrameworkElement) this.mUserAccountBtn, true);
      }
      this.mMacroRecordControl.Visibility = Visibility.Collapsed;
      this.mMacroRecorderToolTipPopup.IsOpen = false;
    }

    internal void ShowMacroPlaybackOnTopBar(MacroRecording record)
    {
      if (Oem.IsOEMDmm)
        return;
      this.mMacroPlayControl.Init(this.ParentWindow, record);
      this.mMacroPlayControl.Visibility = Visibility.Visible;
      if (this.ParentWindow.mIsFullScreen)
        return;
      this.ParentWindow.mTopBar.mMacroRunningToolTipPopup.IsOpen = true;
      this.ParentWindow.mTopBar.mMacroRunningToolTipPopup.StaysOpen = true;
      this.mMacroRunningPopupTimer = new DispatcherTimer()
      {
        Interval = new TimeSpan(0, 0, 0, 5, 0)
      };
      this.mMacroRunningPopupTimer.Tick += new EventHandler(this.MacroRunningPopupTimer_Tick);
      this.mMacroRunningPopupTimer.Start();
    }

    private void MacroRunningPopupTimer_Tick(object sender, EventArgs e)
    {
      this.ParentWindow.mTopBar.mMacroRunningToolTipPopup.IsOpen = false;
      (sender as DispatcherTimer).Stop();
    }

    internal void HideMacroPlaybackFromTopBar()
    {
      if (Oem.IsOEMDmm)
        return;
      this.mConfigButton.Visibility = Visibility.Visible;
      if (this.ParentWindow.EngineInstanceRegistry.IsGoogleSigninDone)
      {
        this.TopBarOptionsPanelElementVisibility((FrameworkElement) this.mNotificationGrid, true);
        this.TopBarOptionsPanelElementVisibility((FrameworkElement) this.mUserAccountBtn, true);
      }
      this.mMacroPlayControl.Visibility = Visibility.Collapsed;
    }

    internal void UpdateMacroRecordingProgress()
    {
      if (!this.ParentWindow.mIsMacroPlaying && !this.ParentWindow.mIsMacroRecorderActive)
        return;
      this.mConfigButton.Visibility = Visibility.Visible;
      if (!this.ParentWindow.EngineInstanceRegistry.IsGoogleSigninDone)
        return;
      this.TopBarOptionsPanelElementVisibility((FrameworkElement) this.mNotificationGrid, true);
      this.TopBarOptionsPanelElementVisibility((FrameworkElement) this.mUserAccountBtn, true);
    }

    internal void ShowSyncIcon()
    {
      this.TopBarOptionsPanelElementVisibility((FrameworkElement) this.mOperationsSyncGrid, true);
    }

    internal void HideSyncIcon()
    {
      this.TopBarOptionsPanelElementVisibility((FrameworkElement) this.mOperationsSyncGrid, false);
    }

    private void MSidebarButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.ParentWindow?.mCommonHandler?.FlipSidebarVisibility(sender as CustomPictureBox, (TextBlock) null);
    }

    private void TopBar_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      if (DesignerProperties.GetIsInDesignMode((DependencyObject) this))
        return;
      this.TopBarButtonsHandling();
    }

    private void TopBarButtonsHandling()
    {
      double num = this.ActualWidth - 180.0 - (double) (this.mAppTabButtons.mDictTabs.Count * 48);
      double actualWidth = this.mOptionsDockPanel.ActualWidth;
      if (actualWidth > num)
      {
        foreach (KeyValuePair<FrameworkElement, double> keyValuePair in (IEnumerable<KeyValuePair<FrameworkElement, double>>) this.mOptionsPriorityPanel.Values)
        {
          if (keyValuePair.Key.Visibility == Visibility.Visible)
          {
            keyValuePair.Key.Visibility = Visibility.Collapsed;
            actualWidth -= keyValuePair.Value;
          }
          if (actualWidth < num)
            break;
        }
      }
      else
      {
        for (int index = this.mOptionsPriorityPanel.Count - 1; index >= 0; --index)
        {
          KeyValuePair<FrameworkElement, double> keyValuePair = this.mOptionsPriorityPanel.ElementAt<KeyValuePair<int, KeyValuePair<FrameworkElement, double>>>(index).Value;
          if (keyValuePair.Key.Visibility == Visibility.Collapsed)
          {
            if (actualWidth + keyValuePair.Value >= num)
              break;
            keyValuePair.Key.Visibility = Visibility.Visible;
            actualWidth += keyValuePair.Value;
          }
        }
      }
    }

    private bool ContainsKey(FrameworkElement element)
    {
      foreach (KeyValuePair<FrameworkElement, double> keyValuePair in (IEnumerable<KeyValuePair<FrameworkElement, double>>) this.mOptionsPriorityPanel.Values)
      {
        if (keyValuePair.Key == element)
          return true;
      }
      return false;
    }

    private void RemoveKey(FrameworkElement element)
    {
      foreach (KeyValuePair<int, KeyValuePair<FrameworkElement, double>> keyValuePair in this.mOptionsPriorityPanel)
      {
        if (keyValuePair.Value.Key == element)
        {
          this.mOptionsPriorityPanel.Remove(keyValuePair.Key);
          break;
        }
      }
    }

    internal void TopBarOptionsPanelElementVisibility(FrameworkElement element, bool isVisible)
    {
      if (isVisible)
      {
        double num = this.ActualWidth - 180.0 - (double) (this.mAppTabButtons.mDictTabs.Count * 48);
        if (this.mOptionsDockPanel.ActualWidth + element.Width < num)
          element.Visibility = Visibility.Visible;
        else
          element.Visibility = Visibility.Collapsed;
        if (this.ContainsKey(element))
          return;
        this.mOptionsPriorityPanel.Add(int.Parse(element.Tag.ToString(), (IFormatProvider) CultureInfo.InvariantCulture), new KeyValuePair<FrameworkElement, double>(element, element.Width));
      }
      else
      {
        element.Visibility = Visibility.Collapsed;
        if (!this.ContainsKey(element))
          return;
        this.RemoveKey(element);
      }
    }

    void ITopBar.ShowSyncPanel(bool isSource)
    {
      this.mOperationsSyncGrid.Visibility = Visibility.Visible;
      if (!isSource)
        return;
      this.mPlayPauseSyncButton.ImageName = "pause_title_bar";
      this.mPlayPauseSyncButton.Visibility = Visibility.Visible;
      this.mStopSyncButton.Visibility = Visibility.Visible;
    }

    void ITopBar.HideSyncPanel()
    {
      this.mOperationsSyncGrid.Visibility = Visibility.Collapsed;
      this.mPlayPauseSyncButton.Visibility = Visibility.Collapsed;
      this.mStopSyncButton.Visibility = Visibility.Collapsed;
      this.mSyncInstancesToolTipPopup.IsOpen = false;
    }

    private void PlayPauseSyncButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      if ((sender as CustomPictureBox).ImageName.Equals("pause_title_bar", StringComparison.InvariantCultureIgnoreCase))
      {
        (sender as CustomPictureBox).ImageName = "play_title_bar";
        this.ParentWindow.mSynchronizerWindow.PauseAllSyncOperations();
      }
      else
      {
        (sender as CustomPictureBox).ImageName = "pause_title_bar";
        this.ParentWindow.mSynchronizerWindow.PlayAllSyncOperations();
      }
    }

    private void StopSyncButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      ((ITopBar) this).HideSyncPanel();
      this.ParentWindow.mSynchronizerWindow.StopAllSyncOperations();
      if (!RegistryManager.Instance.IsShowToastNotification)
        return;
      this.ParentWindow.ShowGeneralToast(LocaleStrings.GetLocalizedString("STRING_SYNC_STOPPED", ""));
    }

    private void OperationsSyncGrid_MouseEnter(object sender, MouseEventArgs e)
    {
      if (!this.ParentWindow.mIsSynchronisationActive)
        return;
      this.mSyncInstancesToolTipPopup.IsOpen = true;
    }

    private void OperationsSyncGrid_MouseLeave(object sender, MouseEventArgs e)
    {
      if (!this.ParentWindow.mIsSynchronisationActive || this.mOperationsSyncGrid.IsMouseOver || this.mSyncInstancesToolTipPopup.IsMouseOver)
        return;
      this.mSyncInstancesToolTipPopup.IsOpen = false;
    }

    private void SyncInstancesToolTip_MouseLeave(object sender, MouseEventArgs e)
    {
      if (this.mOperationsSyncGrid.IsMouseOver || this.mSyncInstancesToolTipPopup.IsMouseOver)
        return;
      this.mSyncInstancesToolTipPopup.IsOpen = false;
    }

    internal void ClosePopups()
    {
      if (this.mMacroRecorderToolTipPopup.IsOpen)
        this.mMacroRecorderToolTipPopup.IsOpen = false;
      if (this.mMacroRunningToolTipPopup.IsOpen)
        this.mMacroRunningToolTipPopup.IsOpen = false;
      if (this.mNotificationCentrePopup.IsOpen)
        this.mNotificationCentrePopup.IsOpen = false;
      if (this.mSettingsMenuPopup.IsOpen)
        this.mSettingsMenuPopup.IsOpen = false;
      if (!this.mSyncInstancesToolTipPopup.IsOpen)
        return;
      this.mSyncInstancesToolTipPopup.IsOpen = false;
    }

    private void HelpButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      string helpCenterUrl = BlueStacksUIUtils.GetHelpCenterUrl();
      if (RegistryManager.Instance.InstallationType == InstallationTypes.GamingEdition)
        BlueStacksUIUtils.OpenUrl(helpCenterUrl);
      else
        this.ParentWindow.mTopBar.mAppTabButtons.AddWebTab(helpCenterUrl, "STRING_FEEDBACK", "help_center", true, "FEEDBACK_TEXT", false);
    }

    private void mNotificationCentrePopup_Closed(object sender, EventArgs e)
    {
      GenericNotificationManager.MarkNotification((IEnumerable<string>) new List<string>((IEnumerable<string>) GenericNotificationManager.GetNotificationItems((Predicate<GenericNotificationItem>) (x => !x.IsDeleted && !x.IsRead && string.Equals(x.VmName, this.ParentWindow.mVmName, StringComparison.InvariantCulture))).Keys), (System.Action<GenericNotificationItem>) (x => x.IsRead = true));
      this.mNotificationDrawerControl.UpdateNotificationCount();
      this.mNotificationCentreButton.ImageName = "notification";
      this.mNotificationCentreButton.IsEnabled = true;
    }

    private void mNotificationCentrePopup_Opened(object sender, EventArgs e)
    {
      this.mNotificationCentreButton.IsEnabled = false;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/topbar.xaml", UriKind.Relative));
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
          ((FrameworkElement) target).Loaded += new RoutedEventHandler(this.TopBar_Loaded);
          ((FrameworkElement) target).SizeChanged += new SizeChangedEventHandler(this.TopBar_SizeChanged);
          break;
        case 2:
          this.mMainGrid = (Grid) target;
          break;
        case 3:
          this.WindowHeaderGrid = (Grid) target;
          break;
        case 4:
          this.mTitleIcon = (CustomPictureBox) target;
          break;
        case 5:
          this.mTitleTextGrid = (Grid) target;
          break;
        case 6:
          this.mTitleText = (TextBlock) target;
          break;
        case 7:
          this.mVersionText = (TextBlock) target;
          break;
        case 8:
          this.mOptionsDockPanel = (DockPanel) target;
          break;
        case 9:
          this.mSidebarButton = (CustomPictureBox) target;
          this.mSidebarButton.MouseLeftButtonUp += new MouseButtonEventHandler(this.MSidebarButton_MouseLeftButtonUp);
          break;
        case 10:
          this.mCloseButton = (CustomPictureBox) target;
          this.mCloseButton.MouseLeftButtonUp += new MouseButtonEventHandler(this.CloseButton_MouseLeftButtonUp);
          break;
        case 11:
          this.mMaximizeButton = (CustomPictureBox) target;
          this.mMaximizeButton.MouseLeftButtonUp += new MouseButtonEventHandler(this.MaxmizeButton_MouseLeftButtonUp);
          break;
        case 12:
          this.mMinimizeButton = (CustomPictureBox) target;
          this.mMinimizeButton.MouseLeftButtonUp += new MouseButtonEventHandler(this.MinimizeButton_MouseLeftButtonUp);
          break;
        case 13:
          this.mConfigButtonGrid = (Grid) target;
          break;
        case 14:
          this.mConfigButton = (CustomPictureBox) target;
          this.mConfigButton.MouseLeftButtonUp += new MouseButtonEventHandler(this.ConfigButton_MouseLeftButtonUp);
          break;
        case 15:
          this.mSettingsBtnNotification = (Ellipse) target;
          break;
        case 16:
          this.mSettingsMenuPopup = (CustomPopUp) target;
          break;
        case 17:
          this.mPreferenceDropDownBorder = (Border) target;
          break;
        case 18:
          this.mGrid = (Grid) target;
          break;
        case 19:
          this.mMaskBorder = (Border) target;
          break;
        case 20:
          this.mPreferenceDropDownControl = (PreferenceDropDownControl) target;
          break;
        case 21:
          this.mHelpButton = (CustomPictureBox) target;
          this.mHelpButton.MouseLeftButtonUp += new MouseButtonEventHandler(this.HelpButton_MouseLeftButtonUp);
          break;
        case 22:
          this.mUserAccountBtn = (CustomPictureBox) target;
          this.mUserAccountBtn.MouseLeftButtonUp += new MouseButtonEventHandler(this.UserAccountButton_MouseLeftButtonUp);
          break;
        case 23:
          this.mNotificationGrid = (Grid) target;
          break;
        case 24:
          this.mNotificationCentreButton = (CustomPictureBox) target;
          this.mNotificationCentreButton.MouseLeftButtonUp += new MouseButtonEventHandler(this.mNotificationCentreButton_MouseLeftButtonUp);
          break;
        case 25:
          this.mNotificationCountBadge = (Canvas) target;
          break;
        case 26:
          this.mNotificationCentrePopup = (CustomPopUp) target;
          break;
        case 27:
          this.mNotificationCaret = (Path) target;
          break;
        case 28:
          this.mNotificationCentreDropDownBorder = (Border) target;
          this.mNotificationCentreDropDownBorder.LayoutUpdated += new EventHandler(this.mNotificationCentreDropDownBorder_LayoutUpdated);
          break;
        case 29:
          this.mMaskBorder1 = (Border) target;
          break;
        case 30:
          this.mNotificationDrawerControl = (NotificationDrawer) target;
          break;
        case 31:
          this.mBtvButton = (CustomPictureBox) target;
          this.mBtvButton.MouseLeftButtonUp += new MouseButtonEventHandler(this.mBtvButton_MouseLeftButtonUp);
          break;
        case 32:
          this.mWarningButton = (CustomPictureBox) target;
          this.mWarningButton.MouseLeftButtonUp += new MouseButtonEventHandler(this.mWarningButton_MouseLeftButtonUp);
          break;
        case 33:
          this.mOperationsSyncGrid = (Grid) target;
          this.mOperationsSyncGrid.MouseEnter += new MouseEventHandler(this.OperationsSyncGrid_MouseEnter);
          this.mOperationsSyncGrid.MouseLeave += new MouseEventHandler(this.OperationsSyncGrid_MouseLeave);
          break;
        case 34:
          this.mSyncMaskBorder = (Border) target;
          break;
        case 35:
          this.mPlayPauseSyncButton = (CustomPictureBox) target;
          this.mPlayPauseSyncButton.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.PlayPauseSyncButton_PreviewMouseLeftButtonUp);
          break;
        case 36:
          this.mStopSyncButton = (CustomPictureBox) target;
          this.mStopSyncButton.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.StopSyncButton_PreviewMouseLeftButtonUp);
          break;
        case 37:
          this.mSyncInstancesToolTipPopup = (CustomPopUp) target;
          break;
        case 38:
          this.mDummyGrid = (Grid) target;
          break;
        case 39:
          this.mMaskBorder2 = (Border) target;
          break;
        case 40:
          this.mUpwardArrow = (Path) target;
          break;
        case 41:
          this.mLocalConfigIndicator = (CustomPictureBox) target;
          break;
        case 42:
          this.mAppTabButtons = (AppTabButtons) target;
          break;
        case 43:
          this.mVideoRecordingStatusGrid = (Grid) target;
          break;
        case 44:
          this.mVideoRecordStatusControl = (VideoRecordingStatus) target;
          break;
        case 45:
          this.mMacroGrid = (Grid) target;
          break;
        case 46:
          this.mMacroRecordControl = (MacroTopBarRecordControl) target;
          break;
        case 47:
          this.mMacroPlayControl = (MacroTopBarPlayControl) target;
          break;
        case 48:
          this.mMacroRecorderToolTipPopup = (CustomPopUp) target;
          break;
        case 49:
          this.dummyGrid = (Grid) target;
          break;
        case 50:
          this.mMaskBorder3 = (Border) target;
          break;
        case 51:
          this.mMacroRecordingTooltip = (TextBlock) target;
          break;
        case 52:
          this.mUpArrow = (Path) target;
          break;
        case 53:
          this.mMacroRunningToolTipPopup = (CustomPopUp) target;
          break;
        case 54:
          this.grid = (Grid) target;
          break;
        case 55:
          this.mMaskBorder4 = (Border) target;
          break;
        case 56:
          this.mMacroRunningTooltip = (TextBlock) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
