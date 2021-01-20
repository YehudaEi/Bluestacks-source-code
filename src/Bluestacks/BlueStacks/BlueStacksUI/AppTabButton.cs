// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.AppTabButton
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
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace BlueStacks.BlueStacksUI
{
  public class AppTabButton : Button, IComponentConnector
  {
    internal Dictionary<string, bool> mDictGamepadEligibility = new Dictionary<string, bool>();
    private string mActivityName = string.Empty;
    private bool mIsTabsSkewed = true;
    private MainWindow mMainWindow;
    internal const int IconModeMinWidth = 38;
    internal const int ParallelogramModeMinWidth = 48;
    private bool mIsPortraitModeTab;
    internal TabType mTabType;
    internal bool mRestartPubgTab;
    internal bool mRestartCODTab;
    internal bool mIsKeyMappingTipDisplayed;
    internal bool mIsOverlayTooltipDisplayed;
    internal bool mIsShootingModeToastDisplayed;
    internal bool mShootingModeToastIsOpen;
    internal bool mGuidanceWindowOpen;
    internal bool mShootingModeToastWhenGuidanceOpen;
    internal bool mIsAnyOperationPendingForTab;
    internal bool mIsSwitchedBackFromHomeTab;
    internal bool mIsNativeGamepadEnabledForApp;
    private bool mIsMoreTabsButton;
    private bool mIsDMMKeyMapEnabled;
    internal Grid ParallelogramGrid;
    internal Border mBorder;
    internal ColumnDefinition mImageColumn;
    internal CustomPictureBox mAppTabIcon;
    internal Label mTabLabel;
    internal CustomPictureBox CloseTabButtonPortrait;
    internal CustomPictureBox CloseTabButtonLandScape;
    internal CustomPictureBox CloseTabButtonDropDown;
    internal Grid mDownArrowGrid;
    internal System.Windows.Shapes.Path Arrow;
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

    public EventHandler<TabChangeEventArgs> EventOnTabChanged { get; set; }

    internal bool IsSelectedSchemeStatSent { get; set; }

    internal bool IsCursorClipped { get; set; }

    internal GameOnboardingControl OnboardingControl { get; set; }

    public bool IsPortraitModeTab
    {
      get
      {
        return this.mIsPortraitModeTab;
      }
      set
      {
        this.mIsPortraitModeTab = value;
        if (!this.IsSelected || this.ParentWindow.IsUIInPortraitMode == this.mIsPortraitModeTab)
          return;
        this.ParentWindow.SwitchToPortraitMode(this.mIsPortraitModeTab);
      }
    }

    public bool IsMoreTabsButton
    {
      get
      {
        return this.mIsMoreTabsButton;
      }
      set
      {
        this.mIsMoreTabsButton = value;
        this.mAppTabIcon.IsEnabled = false;
      }
    }

    public bool IsButtonInDropDown { get; set; }

    public bool IsSelected { get; private set; }

    public bool IsShootingModeTooltipEnabled { get; set; } = true;

    internal void Select(bool value, bool receivedFromImap = false)
    {
      if (this.ParentWindow.StaticComponents.mSelectedTabButton == this && value)
        return;
      if (this.ParentWindow.StaticComponents.mSelectedTabButton != null)
      {
        if (!string.Equals(KMManager.sPackageName, this.PackageName, StringComparison.InvariantCulture))
        {
          KMManager.CloseWindows();
          if (KMManager.sGuidanceWindow != null)
            return;
        }
        AppTabButton selectedTabButton = this.ParentWindow.StaticComponents.mSelectedTabButton;
        this.mIsSwitchedBackFromHomeTab = selectedTabButton.mTabType == TabType.HomeTab;
        this.ParentWindow.StaticComponents.mSelectedTabButton = (AppTabButton) null;
        selectedTabButton.Select(false, false);
        if (selectedTabButton.IsCursorClipped && this.mTabType == TabType.AppTab)
          this.IsCursorClipped = true;
        selectedTabButton.IsCursorClipped = false;
        this.ParentWindow.StaticComponents.mPreviousSelectedTabWeb = selectedTabButton.mTabType == TabType.WebTab;
      }
      this.ParentWindow.ToggleFullScreenToastVisibility(false, "", "", "");
      this.IsSelected = value;
      if (this.IsSelected)
      {
        Publisher.PublishMessage(BrowserControlTags.tabSwitched, this.ParentWindow.mVmName, new JObject((object) new JProperty("PackageName", (object) this.PackageName)));
        this.ParentWindow.mTopBar.mAppTabButtons.ListTabHistory.RemoveAll((Predicate<string>) (n => n.Equals(this.TabKey, StringComparison.OrdinalIgnoreCase)));
        this.ParentWindow.mTopBar.mAppTabButtons.ListTabHistory.Add(this.TabKey);
        this.ParentWindow.StaticComponents.mSelectedTabButton = this;
        this.ParentWindow.Utils.ResetPendingUIOperations();
        this.ParentWindow.mImageUploadedPopup.IsOpen = false;
        if (this.mTabType == TabType.WebTab)
        {
          if (this.ParentWindow.mIsFullScreen)
            this.ParentWindow.RestoreWindows(false);
          BrowserControl browserControl = this.GetBrowserControl();
          if (browserControl == null)
          {
            this.mControlGrid = this.ParentWindow.AddBrowser(this.PackageName);
            this.Init(this.AppName, this.PackageName, this.mAppTabIcon.ImageName, this.mControlGrid, this.TabKey);
          }
          else
          {
            try
            {
              object[] args = new object[0];
              if (browserControl.CefBrowser != null)
                browserControl.CefBrowser.CallJs("webtabselected", args);
            }
            catch (Exception ex)
            {
              Logger.Warning("Ignoring webtabselected exception. " + ex.Message);
            }
          }
          this.ParentWindow.ChangeWindowOrientaion((object) this, new ChangeOrientationEventArgs(this.ParentWindow.mAspectRatio < (Fraction) 1L));
          this.ParentWindow.mAppHandler.CurrentActivityName = string.Empty;
        }
        this.ParentWindow.ShowControlGrid(this.mControlGrid);
        if (this.mTabType == TabType.AppTab || this.mTabType == TabType.HomeTab)
        {
          if (this.ParentWindow.AppForcedOrientationDict.ContainsKey(this.PackageName))
            this.ParentWindow.ChangeOrientationFromClient(this.ParentWindow.AppForcedOrientationDict[this.PackageName], true);
          else
            this.ParentWindow.ChangeOrientationFromClient(false, false);
        }
        if (this.mTabType == TabType.HomeTab)
        {
          this.ParentWindow.mWelcomeTab.mHomeAppManager.HomeTabSwitchActions(true);
          if (this.ParentWindow.mIsFullScreen)
            this.ParentWindow.RestoreWindows(false);
          this.ParentWindow.mAppHandler.CurrentActivityName = string.Empty;
        }
        else if (!Oem.IsOEMDmm && this.mTabType == TabType.AppTab && (!this.ParentWindow.mSidebar.mIsOverlayTooltipClosed && !this.mIsOverlayTooltipDisplayed) && KMManager.KeyMappingFilesAvailable(this.PackageName))
        {
          this.mIsOverlayTooltipDisplayed = true;
          this.ParentWindow.mSidebar.ShowOverlayTooltip(true, false);
        }
        if (this.mTabType != TabType.HomeTab)
          this.ParentWindow.mWelcomeTab.mHomeAppManager.HomeTabSwitchActions(false);
        AppUsageTimer.StartTimer(this.ParentWindow.mVmName, this.TabKey);
        if (this.IsLaunchOnSelection)
          this.LaunchApp();
        else
          this.IsLaunchOnSelection = true;
        BlueStacksUIBinding.BindColor((DependencyObject) this.mBorder, Panel.BackgroundProperty, "SelectedTabBackgroundColor");
        BlueStacksUIBinding.BindColor((DependencyObject) this.mTabLabel, Control.ForegroundProperty, "SelectedTabForegroundColor");
        BlueStacksUIBinding.BindColor((DependencyObject) this.mBorder, Border.BorderBrushProperty, "SelectedTabBorderColor");
        if (!Oem.IsOEMDmm)
        {
          if (this.mTabType == TabType.AppTab)
          {
            this.ParentWindow.mTopBar.mAppTabButtons.KillWebTabs();
            if (this.ParentWindow.mWelcomeTab.mHomeAppManager.GetAppIcon(this.PackageName)?.IsGamepadCompatible.GetValueOrDefault())
              this.ParentWindow.mCommonHandler.OnGamepadButtonVisibilityChanged(true);
            else
              this.ParentWindow.mCommonHandler.OnGamepadButtonVisibilityChanged(false);
            KMManager.LoadIMActions(this.ParentWindow, this.PackageName);
            this.ParentWindow.mCallbackEnabled = "False";
            Logger.Info("Callback: Select(): " + this.ParentWindow.mCallbackEnabled);
            KMManager.mOnboardingCounter = 1;
            KMManager.CheckForGamepadKeymapping(this.ParentWindow);
            try
            {
              if (!this.IsSelectedSchemeStatSent)
              {
                AppInfo infoFromPackageName = new JsonParser(this.ParentWindow.mVmName).GetAppInfoFromPackageName(this.PackageName);
                string str1 = string.Empty;
                if (infoFromPackageName != null && !string.IsNullOrEmpty(infoFromPackageName.Version))
                  str1 = infoFromPackageName.Version;
                string str2 = RegistryManager.Instance.TranslucentControlsTransparency == 0.0 ? "false" : "true";
                string str3 = this.ParentWindow.SelectedConfig.SelectedControlScheme.BuiltIn ? "true" : "false";
                string str4 = this.ParentWindow.IsGamepadConnected ? "true" : "false";
                string str5 = this.ParentWindow.mCommonHandler.CheckIfGamepadOverlaySelectedForApp(this.PackageName) ? "GamepadOverlay" : "KeyboardOverlay";
                ClientStats.SendMiscellaneousStatsAsync("SelectedSchemeName", RegistryManager.Instance.UserGuid, this.PackageName, this.ParentWindow.SelectedConfig.SelectedControlScheme.Name, str3, str1, str5, str2, str4, "Android");
                this.IsSelectedSchemeStatSent = true;
              }
            }
            catch (Exception ex)
            {
              Logger.Warning("Exception when sending SelectedSchemeName stat, " + ex.ToString());
            }
            if (this.mTabType == TabType.AppTab && !this.mIsKeyMappingTipDisplayed && (!BlueStacksUIUtils.sSyncInvolvedInstances.Contains(this.ParentWindow.mVmName) && KMManager.KeyMappingFilesAvailable(this.PackageName)))
            {
              this.mIsKeyMappingTipDisplayed = true;
              if (this.ParentWindow.SelectedConfig != null && this.ParentWindow.SelectedConfig.SelectedControlScheme != null && this.ParentWindow.SelectedConfig.SelectedControlScheme.GameControls != null && this.ParentWindow.SelectedConfig.SelectedControlScheme.GameControls.Any<IMAction>((Func<IMAction, bool>) (action => action.Guidance.Any<KeyValuePair<string, string>>())))
                this.ParentWindow.mSidebar.ShowKeyMapPopup(true);
            }
            else if (this.mGuidanceWindowOpen)
              KMManager.HandleInputMapperWindow(this.ParentWindow, "");
            if (RegistryManager.Instance.ShowKeyControlsOverlay && !KMManager.CheckIfKeymappingWindowVisible(false))
              KMManager.ShowOverlayWindow(this.ParentWindow, true, true);
            if (this.mIsSwitchedBackFromHomeTab && KMManager.KeyMappingFilesAvailable(this.PackageName))
              this.ParentWindow.mFrontendHandler.SendFrontendRequestAsync("handleLoadConfigOnTabSwitch", new Dictionary<string, string>()
              {
                {
                  "package",
                  this.PackageName
                }
              });
            this.ParentWindow.mCommonHandler.ToggleMacroAndSyncVisibility();
            this.ParentWindow.mCommonHandler.SetCustomCursorForApp(this.PackageName);
            this.mIsNativeGamepadEnabledForApp = this.ParentWindow.EngineInstanceRegistry.NativeGamepadState != NativeGamepadState.ForceOff;
            if (this.ParentWindow.EngineInstanceRegistry.NativeGamepadState == NativeGamepadState.Auto)
              new Thread((ThreadStart) (() =>
              {
                try
                {
                  bool flag = this.ParentWindow.mCommonHandler.CheckNativeGamepadState(this.PackageName);
                  this.mIsNativeGamepadEnabledForApp = flag;
                  this.ParentWindow.mFrontendHandler.SendFrontendRequest("enableNativeGamepad", new Dictionary<string, string>()
                  {
                    {
                      "isEnabled",
                      flag.ToString((IFormatProvider) CultureInfo.InvariantCulture)
                    }
                  });
                }
                catch (Exception ex)
                {
                  Logger.Error(string.Format("Error while checking CheckNativeGamepadState: {0}", (object) ex));
                }
              })).Start();
          }
          else
          {
            KMManager.ShowOverlayWindow(this.ParentWindow, false, false);
            if (this.ParentWindow.mCommonHandler != null)
              this.ParentWindow.mCommonHandler.ClipMouseCursorHandler(true, true, "", "");
          }
          if (this.mTabType == TabType.HomeTab)
            this.ParentWindow.mCommonHandler.ToggleMacroAndSyncVisibility();
          else
            this.ParentWindow.mWelcomeTab.mHomeAppManager.CloseAppSuggestionPopup();
          List<GenericNotificationItem> notificationItemList = new List<GenericNotificationItem>();
          foreach (GenericNotificationItem notificationItem in PromotionManager.sPassedDeferredNotificationsList.Where<GenericNotificationItem>((Func<GenericNotificationItem, bool>) (_ => string.Compare(_.DeferredApp, this.PackageName, StringComparison.OrdinalIgnoreCase) == 0)))
          {
            BlueStacksUIUtils.DictWindows[BlueStacks.Common.Strings.CurrentDefaultVmName].HandleGenericNotificationPopup(notificationItem);
            GenericNotificationManager.AddNewNotification(notificationItem, false);
            BlueStacksUIUtils.DictWindows[BlueStacks.Common.Strings.CurrentDefaultVmName].mTopBar.RefreshNotificationCentreButton();
            notificationItemList.Add(notificationItem);
          }
          foreach (GenericNotificationItem notificationItem in notificationItemList)
            PromotionManager.sPassedDeferredNotificationsList.Remove(notificationItem);
          if (this.ParentWindow.SendClientActions && !receivedFromImap)
          {
            Dictionary<string, string> data = new Dictionary<string, string>();
            Dictionary<string, string> dictionary = new Dictionary<string, string>()
            {
              {
                "EventAction",
                "TabSelected"
              },
              {
                "tabKey",
                this.TabKey
              }
            };
            JsonSerializerSettings serializerSettings = Utils.GetSerializerSettings();
            serializerSettings.Formatting = Formatting.None;
            data.Add("operationData", JsonConvert.SerializeObject((object) dictionary, serializerSettings));
            this.ParentWindow.mFrontendHandler.SendFrontendRequestAsync("handleClientOperation", data);
          }
        }
        if (this.mTabType == TabType.AppTab && KMManager.KeyMappingFilesAvailable(this.PackageName) && (this.ParentWindow.SelectedConfig.ControlSchemes != null && this.ParentWindow.SelectedConfig.ControlSchemes.Count > 0))
          this.ParentWindow.mCommonHandler.OnGameGuideButtonVisibilityChanged(true);
        else
          this.ParentWindow.mCommonHandler.OnGameGuideButtonVisibilityChanged(false);
        EventHandler<EventArgs> eventOnTabChanged = this.ParentWindow.mTopBar.mAppTabButtons.EventOnTabChanged;
        if (eventOnTabChanged != null)
          eventOnTabChanged((object) null, (EventArgs) null);
        if (this.mRestartPubgTab)
        {
          this.Dispatcher.Invoke((Delegate) (() =>
          {
            CustomMessageWindow customMessageWindow = new CustomMessageWindow();
            string path1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_RESTART_OBJECT", ""), (object) "PUBG Mobile");
            BlueStacksUIBinding.Bind(customMessageWindow.TitleTextBlock, path1, "");
            string path2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_SETTING_CHANGED_RESTART_APP_MESSAGE", ""), (object) "PUBG Mobile");
            BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlock, path2, "");
            customMessageWindow.AddButton(ButtonColors.Blue, "STRING_RESTART_BLUESTACKS", new EventHandler(this.RestartConfirmationAcceptedHandler), (string) null, false, (object) null, true);
            customMessageWindow.AddButton(ButtonColors.White, "STRING_CANCEL", (EventHandler) null, (string) null, false, (object) null, true);
            this.ParentWindow.ShowDimOverlay((IDimOverlayControl) null);
            customMessageWindow.Owner = (Window) this.ParentWindow.mDimOverlay;
            customMessageWindow.ShowDialog();
            this.ParentWindow.HideDimOverlay();
          }));
          this.mRestartPubgTab = false;
        }
        if (this.mRestartCODTab)
        {
          this.Dispatcher.Invoke((Delegate) (() =>
          {
            CustomMessageWindow customMessageWindow = new CustomMessageWindow();
            string path1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_RESTART_OBJECT", ""), (object) "Call of Duty: Mobile");
            BlueStacksUIBinding.Bind(customMessageWindow.TitleTextBlock, path1, "");
            string path2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_SETTING_CHANGED_RESTART_APP_MESSAGE", ""), (object) "Call of Duty: Mobile");
            BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlock, path2, "");
            customMessageWindow.AddButton(ButtonColors.Blue, "STRING_RESTART_BLUESTACKS", new EventHandler(this.RestartConfirmationAcceptedHandler), (string) null, false, (object) null, true);
            customMessageWindow.AddButton(ButtonColors.White, "STRING_CANCEL", (EventHandler) null, (string) null, false, (object) null, true);
            this.ParentWindow.ShowDimOverlay((IDimOverlayControl) null);
            customMessageWindow.Owner = (Window) this.ParentWindow.mDimOverlay;
            customMessageWindow.ShowDialog();
            this.ParentWindow.HideDimOverlay();
          }));
          this.mRestartCODTab = false;
        }
        if (string.Equals(this.PackageName, "com.android.vending", StringComparison.InvariantCultureIgnoreCase) && !RegistryManager.Instance.Guest[this.ParentWindow.mVmName].IsGoogleSigninDone && this.ParentWindow.IsFarmingInstance)
          this.ParentWindow.ShowPersistentGeneralToast(LocaleStrings.GetLocalizedString("STRING_SIGN_IN_WITH_DIFFERENT_ACCOUNT_FOR_FARMING", "Sign in with a different Gmail address to start farming"));
        else if (this.ParentWindow.mGeneraltoast.IsOpen)
          this.ParentWindow.mGeneraltoast.IsOpen = false;
        bool? nullable1;
        if (this.mTabType == TabType.AppTab && File.Exists(Utils.GetInputmapperDefaultFilePath(this.PackageName)) && Oem.Instance.IsShowGameBlurb)
        {
          nullable1 = this.ParentWindow.mInstanceWiseCloudInfoManager.mInstanceWiseCloudInfo?.GameFeaturePopupInfo.GameFeaturePopupPackages?.IsPackageAvailable(this.PackageName);
          if (nullable1.GetValueOrDefault() && !Utils.IsEngineRaw() && (string.Equals(this.ParentWindow.mInstanceWiseCloudInfoManager.mInstanceWiseCloudInfo?.GameFeaturePopupInfo.GameFeaturePopupPackages.GetAppPackageObject(this.PackageName).ExtraInfo["isPopupShown"], "false", StringComparison.InvariantCultureIgnoreCase) && this.ParentWindow.Width > 800.0))
          {
            this.OnboardingControl = new GameOnboardingControl(this.ParentWindow, this.PackageName, "applaunch", this.ParentWindow.mInstanceWiseCloudInfoManager.mInstanceWiseCloudInfo?.GameFeaturePopupInfo.GameFeaturePopupPackages.GetAppPackageObject(this.PackageName).ExtraInfo["game_popup_id"]);
            KMManager.sGuidanceWindow?.DimOverLayVisibility(Visibility.Visible);
            this.ParentWindow.ShowDimOverlay((IDimOverlayControl) this.OnboardingControl);
          }
          else
          {
            PostBootCloudInfo postBootCloudInfo = PostBootCloudInfoManager.Instance.mPostBootCloudInfo;
            bool? nullable2;
            if (postBootCloudInfo == null)
            {
              nullable1 = new bool?();
              nullable2 = nullable1;
            }
            else
            {
              AppPackageListObject boardingAppPackages = postBootCloudInfo.OnBoardingInfo.OnBoardingAppPackages;
              if (boardingAppPackages == null)
              {
                nullable1 = new bool?();
                nullable2 = nullable1;
              }
              else
                nullable2 = new bool?(boardingAppPackages.IsPackageAvailable(this.PackageName));
            }
            nullable1 = nullable2;
            string vmName;
            if (nullable1.GetValueOrDefault() && !AppConfigurationManager.Instance.CheckIfTrueInAnyVm(this.PackageName, (Predicate<AppSettings>) (appSetting => appSetting.IsAppOnboardingCompleted), out vmName))
            {
              this.OnboardingControl = new GameOnboardingControl(this.ParentWindow, this.PackageName, "applaunch");
              KMManager.sGuidanceWindow?.DimOverLayVisibility(Visibility.Visible);
              this.ParentWindow.ShowDimOverlay((IDimOverlayControl) this.OnboardingControl);
            }
            else
            {
              this.ParentWindow.ShowDimOverlay((IDimOverlayControl) null);
              if (!AppConfigurationManager.Instance.CheckIfTrueInAnyVm(this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName, (Predicate<AppSettings>) (appSettings => appSettings.IsGeneralAppOnBoardingCompleted), out vmName) && KMManager.onBoardingPopupWindows.Count == 0)
                this.ShowDefaultBlurbOnboarding();
              this.ParentWindow.HideDimOverlay();
            }
          }
        }
        if (this.mTabType == TabType.AppTab)
        {
          PostBootCloudInfo postBootCloudInfo1 = PostBootCloudInfoManager.Instance.mPostBootCloudInfo;
          bool? nullable2;
          if (postBootCloudInfo1 == null)
          {
            nullable1 = new bool?();
            nullable2 = nullable1;
          }
          else
          {
            AppPackageListObject converterAppPackages = postBootCloudInfo1.UtcConverterInfo.UtcConverterAppPackages;
            if (converterAppPackages == null)
            {
              nullable1 = new bool?();
              nullable2 = nullable1;
            }
            else
              nullable2 = new bool?(converterAppPackages.IsPackageAvailable(this.PackageName));
          }
          nullable1 = nullable2;
          if (nullable1.GetValueOrDefault())
          {
            this.ParentWindow.mCommonHandler.OnUtcConverterVisibilityChanged(true);
            int num = 3;
            PostBootCloudInfo postBootCloudInfo2 = PostBootCloudInfoManager.Instance.mPostBootCloudInfo;
            bool? nullable3;
            if (postBootCloudInfo2 == null)
            {
              nullable1 = new bool?();
              nullable3 = nullable1;
            }
            else
            {
              AppPackageListObject converterAppPackages = postBootCloudInfo2.UtcConverterInfo.UtcConverterAppPackages;
              if (converterAppPackages == null)
              {
                nullable1 = new bool?();
                nullable3 = nullable1;
              }
              else
              {
                AppPackageObject appPackageObject = converterAppPackages.GetAppPackageObject(this.PackageName);
                if (appPackageObject == null)
                {
                  nullable1 = new bool?();
                  nullable3 = nullable1;
                }
                else
                  nullable3 = new bool?(appPackageObject.ExtraInfo.ContainsKey("red_dot_visibility_days"));
              }
            }
            nullable1 = nullable3;
            int result1;
            if (nullable1.GetValueOrDefault() && int.TryParse(PostBootCloudInfoManager.Instance.mPostBootCloudInfo?.UtcConverterInfo.UtcConverterAppPackages?.GetAppPackageObject(this.PackageName)?.ExtraInfo["red_dot_visibility_days"], out result1))
              num = result1;
            DateTime result2;
            if (!RegistryManager.Instance.IsUtcConverterBlurbOnboardingCompleted && (string.IsNullOrEmpty(AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName][this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName].AppInstallTime) || DateTime.TryParse(AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName][this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName].AppInstallTime, (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.None, out result2) && (DateTime.Now.Date - result2.Date).Days >= num))
            {
              Sidebar mSidebar = this.ParentWindow.mSidebar;
              if (mSidebar != null)
              {
                mSidebar.ShowUtcConverterPopup();
                goto label_126;
              }
              else
                goto label_126;
            }
            else
              goto label_126;
          }
        }
        this.ParentWindow.mCommonHandler.OnUtcConverterVisibilityChanged(false);
label_126:
        if (this.ParentWindow.mTopBar.mAppTabButtons.mMoreTabButton.Visibility != Visibility.Visible)
          return;
        this.MoreTabsButtonHandling();
      }
      else
      {
        BlueStacksUIBinding.BindColor((DependencyObject) this.mBorder, Panel.BackgroundProperty, "TabBackgroundColor");
        BlueStacksUIBinding.BindColor((DependencyObject) this.mTabLabel, Control.ForegroundProperty, "TabForegroundColor");
        BlueStacksUIBinding.BindColor((DependencyObject) this.mBorder, Border.BorderBrushProperty, "AppTabBorderBrush");
      }
    }

    private void RestartConfirmationAcceptedHandler(object sender, EventArgs e)
    {
      Logger.Info("Restarting Pubg/COD Tab.");
      new Thread((ThreadStart) (() => this.ParentWindow.mTopBar.mAppTabButtons.RestartTab(this.PackageName)))
      {
        IsBackground = true
      }.Start();
    }

    public string PackageName { get; set; } = string.Empty;

    public string TabKey { get; set; }

    public string AppName { get; set; } = string.Empty;

    public bool IsDMMKeymapEnabled
    {
      get
      {
        return this.mIsDMMKeyMapEnabled;
      }
      set
      {
        this.mIsDMMKeyMapEnabled = value;
        this.IsDMMKeymapUIVisible = value;
        this.ParentWindow.mCommonHandler.SetDMMKeymapButtonsAndTransparency();
      }
    }

    public bool IsDMMKeymapUIVisible { get; set; }

    public string AppLabel
    {
      get
      {
        return this.mTabLabel.Content.ToString();
      }
    }

    public bool IsLaunchOnSelection { get; set; }

    public Grid mControlGrid { get; set; }

    public AppTabButton()
    {
      this.InitializeComponent();
    }

    internal void Init(
      string appName,
      string packageName,
      string activityName,
      string imageName,
      Grid controlGrid,
      string tabKey)
    {
      bool flag = false;
      if (!string.IsNullOrEmpty(tabKey))
        flag = true;
      this.Init(appName, packageName, imageName, controlGrid, flag ? tabKey : packageName);
      this.mActivityName = activityName;
      this.mTabType = TabType.AppTab;
      if (!string.Equals(packageName, "Home", StringComparison.InvariantCulture) && !string.Equals(packageName, "Setup", StringComparison.InvariantCulture))
        return;
      this.mTabType = TabType.HomeTab;
      BlueStacksUIBinding.BindCornerRadius((DependencyObject) this, FrameworkElement.MarginProperty, "TabMarginLandScape");
    }

    internal void Init(
      string title,
      string url,
      string imageName,
      Grid controlGrid,
      string tabKey)
    {
      BlueStacksUIBinding.Bind((DependencyObject) this, title, FrameworkElement.ToolTipProperty);
      BlueStacksUIBinding.Bind(this.mTabLabel, title);
      this.AppName = title;
      this.PackageName = url;
      this.TabKey = tabKey;
      this.mTabType = TabType.WebTab;
      this.mControlGrid = controlGrid;
      if (!this.IsSelected)
        this.mControlGrid.Visibility = Visibility.Hidden;
      if (string.IsNullOrEmpty(imageName))
        this.mImageColumn.Width = new GridLength(0.0);
      else
        this.mAppTabIcon.ImageName = imageName;
    }

    internal void ResizeButton(double tabWidth)
    {
      if (this.ParentWindow.IsUIInPortraitMode)
        this.MakeTabParallelogram(false);
      else
        this.MakeTabParallelogram(true);
      if (tabWidth == this.ActualWidth)
        return;
      DoubleAnimation doubleAnimation1 = new DoubleAnimation();
      doubleAnimation1.From = new double?(this.ActualWidth);
      doubleAnimation1.To = new double?(tabWidth);
      doubleAnimation1.Duration = new Duration(TimeSpan.FromMilliseconds(200.0));
      DoubleAnimation doubleAnimation2 = doubleAnimation1;
      this.BeginAnimation(FrameworkElement.WidthProperty, (AnimationTimeline) doubleAnimation2);
    }

    internal void MakeTabParallelogram(bool isSkewTab)
    {
      if (isSkewTab)
        BlueStacksUIBinding.BindCornerRadius((DependencyObject) this, FrameworkElement.MarginProperty, "TabMarginLandScape");
      else
        BlueStacksUIBinding.BindCornerRadius((DependencyObject) this, FrameworkElement.MarginProperty, "TabMarginPortrait");
      if (isSkewTab == this.mIsTabsSkewed)
        return;
      if (isSkewTab)
      {
        this.mIsTabsSkewed = true;
        this.CloseTabButtonPortrait.Visibility = Visibility.Hidden;
        this.ParallelogramGrid.RenderTransform = (Transform) new SkewTransform(BlueStacksUIColorManager.AppliedTheme.TabTransform.AngleX, BlueStacksUIColorManager.AppliedTheme.TabTransform.AngleY);
        DoubleAnimation doubleAnimation1 = new DoubleAnimation(BlueStacksUIColorManager.AppliedTheme.TabTransformPortrait.AngleX, BlueStacksUIColorManager.AppliedTheme.TabTransform.AngleX, (Duration) TimeSpan.FromMilliseconds(200.0));
        DoubleAnimation doubleAnimation2 = new DoubleAnimation(BlueStacksUIColorManager.AppliedTheme.TabTransformPortrait.AngleY, BlueStacksUIColorManager.AppliedTheme.TabTransform.AngleY, (Duration) TimeSpan.FromMilliseconds(200.0));
        doubleAnimation2.Completed += new EventHandler(this.SkewY_Completed);
        this.ParallelogramGrid.RenderTransform.BeginAnimation(SkewTransform.AngleXProperty, (AnimationTimeline) doubleAnimation1);
        this.ParallelogramGrid.RenderTransform.BeginAnimation(SkewTransform.AngleYProperty, (AnimationTimeline) doubleAnimation2);
        BlueStacksUIBinding.BindCornerRadius((DependencyObject) this, FrameworkElement.MarginProperty, "TabMarginLandScape");
      }
      else
      {
        this.mIsTabsSkewed = false;
        this.CloseTabButtonLandScape.Visibility = Visibility.Hidden;
        this.ParallelogramGrid.RenderTransform = (Transform) new SkewTransform(BlueStacksUIColorManager.AppliedTheme.TabTransform.AngleX, BlueStacksUIColorManager.AppliedTheme.TabTransform.AngleY);
        DoubleAnimation doubleAnimation1 = new DoubleAnimation(BlueStacksUIColorManager.AppliedTheme.TabTransform.AngleX, BlueStacksUIColorManager.AppliedTheme.TabTransformPortrait.AngleX, (Duration) TimeSpan.FromMilliseconds(200.0));
        DoubleAnimation doubleAnimation2 = new DoubleAnimation(BlueStacksUIColorManager.AppliedTheme.TabTransformPortrait.AngleY, BlueStacksUIColorManager.AppliedTheme.TabTransform.AngleY, (Duration) TimeSpan.FromMilliseconds(200.0));
        this.ParallelogramGrid.RenderTransform.BeginAnimation(SkewTransform.AngleXProperty, (AnimationTimeline) doubleAnimation1);
        this.ParallelogramGrid.RenderTransform.BeginAnimation(SkewTransform.AngleYProperty, (AnimationTimeline) doubleAnimation2);
        doubleAnimation2.Completed += new EventHandler(this.SkewY_Completed);
        BlueStacksUIBinding.BindCornerRadius((DependencyObject) this, FrameworkElement.MarginProperty, "TabMarginPortrait");
      }
      if (!this.mIsMoreTabsButton)
        return;
      this.mBorder.Visibility = Visibility.Visible;
      BlueStacksUIBinding.BindColor((DependencyObject) this.mBorder, Panel.BackgroundProperty, "TabBackgroundColor");
    }

    private void SkewY_Completed(object sender, EventArgs e)
    {
      if (this.mIsTabsSkewed)
        BlueStacksUIBinding.BindTransform((DependencyObject) this.ParallelogramGrid, UIElement.RenderTransformProperty, "TabTransform");
      else
        BlueStacksUIBinding.BindTransform((DependencyObject) this.ParallelogramGrid, UIElement.RenderTransformProperty, "TabTransformPortrait");
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      if (this.IsMoreTabsButton)
        return;
      int num = sender.GetHashCode() == this.ParentWindow.StaticComponents.mSelectedTabButton.GetHashCode() ? 1 : 0;
      bool flag = this.CloseTabButtonLandScape.IsMouseOver || this.CloseTabButtonPortrait.IsMouseOver || this.CloseTabButtonDropDown.IsMouseOver;
      if (num != 0)
      {
        if (!flag)
          return;
        if (KMManager.sGuidanceWindow != null && !KMManager.sGuidanceWindow.IsClosed)
          this.HandlePendingOperationsForTab("guidance");
        if (KMManager.sGuidanceWindow != null)
          return;
        this.ParentWindow.mTopBar.mAppTabButtons.CloseTab(this.TabKey, true, false, false, false, "");
      }
      else if (flag)
      {
        this.ParentWindow.mTopBar.mAppTabButtons.CloseTab(this.TabKey, true, false, false, false, "");
      }
      else
      {
        if (KMManager.sGuidanceWindow != null && !KMManager.sGuidanceWindow.IsClosed)
          this.HandlePendingOperationsForTab("guidance");
        if (KMManager.sGuidanceWindow != null)
          return;
        this.Select(true, false);
        this.Button_PreviewMouseUp((object) null, (MouseButtonEventArgs) null);
        EventHandler<TabChangeEventArgs> eventOnTabChanged = this.EventOnTabChanged;
        if (eventOnTabChanged == null)
          return;
        eventOnTabChanged((object) this, new TabChangeEventArgs(this.AppName, this.PackageName, this.mTabType));
      }
    }

    private void HandlePendingOperationsForTab(string pendingOperation)
    {
      switch (pendingOperation)
      {
        case "guidance":
          KMManager.CloseWindows();
          break;
      }
    }

    internal void UpdateUIForDropDown(bool isInDropDown)
    {
      if (isInDropDown)
      {
        this.IsButtonInDropDown = true;
        this.MakeTabParallelogram(false);
        this.MinWidth = 150.0;
        this.mTabLabel.Margin = new Thickness(3.0, 1.0, 3.0, 1.0);
        if (!this.IsSelected)
          this.mBorder.Background = (Brush) Brushes.Transparent;
        this.mBorder.BorderThickness = new Thickness(0.0);
      }
      else
      {
        this.IsButtonInDropDown = false;
        this.mBorder.BorderThickness = new Thickness(1.0);
        this.MinWidth = 0.0;
        this.mTabLabel.Margin = new Thickness(3.0, 1.0, 24.0, 1.0);
        if (this.IsSelected)
        {
          BlueStacksUIBinding.BindColor((DependencyObject) this.mBorder, Panel.BackgroundProperty, "SelectedTabBackgroundColor");
          BlueStacksUIBinding.BindColor((DependencyObject) this.mBorder, Border.BorderBrushProperty, "SelectedTabBorderColor");
        }
        else
        {
          BlueStacksUIBinding.BindColor((DependencyObject) this.mBorder, Panel.BackgroundProperty, "TabBackgroundColor");
          BlueStacksUIBinding.BindColor((DependencyObject) this.mBorder, Border.BorderBrushProperty, "AppTabBorderBrush");
        }
      }
    }

    internal void LaunchApp()
    {
      if (!string.IsNullOrEmpty(this.PackageName) && this.mTabType == TabType.AppTab)
      {
        this.ParentWindow.mAppHandler.SendRunAppRequestAsync(this.PackageName, this.mActivityName, false);
      }
      else
      {
        if (this.mTabType != TabType.HomeTab && this.mTabType != TabType.WebTab || !RegistryManager.Instance.SwitchToAndroidHome)
          return;
        this.ParentWindow.mAppHandler.GoHome();
      }
    }

    private void Button_MouseEnter(object sender, MouseEventArgs e)
    {
      if (!this.IsSelected)
        BlueStacksUIBinding.BindColor((DependencyObject) this.mBorder, Panel.BackgroundProperty, "TabBackgroundHoverColor");
      if (this.IsButtonInDropDown)
      {
        if (this.mTabType != TabType.HomeTab)
          this.CloseTabButtonDropDown.Visibility = Visibility.Visible;
      }
      else if (this.mTabType != TabType.HomeTab && !this.mIsMoreTabsButton)
      {
        this.CloseTabButtonLandScape.Visibility = Visibility.Visible;
        if (!this.mIsTabsSkewed)
          this.CloseTabButtonPortrait.Visibility = Visibility.Visible;
      }
      if (!this.IsMoreTabsButton)
        return;
      this.mAppTabIcon.SetHoverImage();
      BlueStacksUIBinding.BindColor((DependencyObject) this.mBorder, Panel.BackgroundProperty, "TabBackgroundHoverColor");
    }

    private void Button_MouseLeave(object sender, MouseEventArgs e)
    {
      if (!this.IsSelected)
        BlueStacksUIBinding.BindColor((DependencyObject) this.mBorder, Panel.BackgroundProperty, "TabBackgroundColor");
      if (this.IsMoreTabsButton)
      {
        this.mAppTabIcon.SetDefaultImage();
        BlueStacksUIBinding.BindColor((DependencyObject) this.mBorder, Panel.BackgroundProperty, "TabBackgroundColor");
      }
      this.CloseTabButtonLandScape.Visibility = Visibility.Hidden;
      this.CloseTabButtonPortrait.Visibility = Visibility.Hidden;
      this.CloseTabButtonDropDown.Visibility = Visibility.Hidden;
    }

    private void Button_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
      if (this.IsButtonInDropDown)
        return;
      if (this.IsMoreTabsButton)
      {
        this.mAppTabIcon.SetClickedImage();
        BlueStacksUIBinding.BindColor((DependencyObject) this.mBorder, Panel.BackgroundProperty, "SelectedTabBackgroundColor");
      }
      else
      {
        if (this.IsSelected)
          return;
        BlueStacksUIBinding.BindColor((DependencyObject) this.mBorder, Panel.BackgroundProperty, "TabBackgroundColor");
        BlueStacksUIBinding.BindColor((DependencyObject) this.mBorder, Border.BorderBrushProperty, "AppTabBorderBrush");
      }
    }

    private void Button_PreviewMouseUp(object sender, MouseButtonEventArgs e)
    {
      if (this.IsButtonInDropDown)
        return;
      if (!this.IsSelected)
        BlueStacksUIBinding.BindColor((DependencyObject) this.mBorder, Panel.BackgroundProperty, "TabBackgroundColor");
      if (!this.mIsMoreTabsButton)
        return;
      if (this.IsMouseOver)
      {
        this.mAppTabIcon.SetHoverImage();
        BlueStacksUIBinding.BindColor((DependencyObject) this.mBorder, Panel.BackgroundProperty, "TabBackgroundHoverColor");
      }
      else
      {
        this.mAppTabIcon.SetDefaultImage();
        BlueStacksUIBinding.BindColor((DependencyObject) this.mBorder, Panel.BackgroundProperty, "TabBackgroundColor");
      }
    }

    private void Button_IsEnabledChanged(object _1, DependencyPropertyChangedEventArgs _2)
    {
      if (this.IsEnabled)
        this.Opacity = 1.0;
      else
        this.Opacity = 0.3;
    }

    internal BrowserControl GetBrowserControl()
    {
      try
      {
        return this.mControlGrid.Children[0] as BrowserControl;
      }
      catch (Exception ex)
      {
        Logger.Warning("No BrowserControl associated with tabkey: " + this.TabKey + " Error: " + ex.ToString());
        return (BrowserControl) null;
      }
    }

    internal void EnableKeymapForDMM(bool enable)
    {
      this.mIsDMMKeyMapEnabled = enable;
    }

    internal void MoreTabsButtonHandling()
    {
      AppTabButton mMoreTabButton = this.ParentWindow.mTopBar.mAppTabButtons.mMoreTabButton;
      mMoreTabButton.mTabLabel.Visibility = Visibility.Collapsed;
      mMoreTabButton.mDownArrowGrid.Visibility = Visibility.Visible;
      if (this.ParentWindow.mTopBar.mAppTabButtons.mHiddenButtons.Children.Contains((UIElement) this.ParentWindow.StaticComponents.mSelectedTabButton))
        mMoreTabButton.mAppTabIcon.ImageName = this.ParentWindow.StaticComponents.mSelectedTabButton.mAppTabIcon.ImageName;
      else
        mMoreTabButton.mAppTabIcon.ImageName = (this.ParentWindow.mTopBar.mAppTabButtons.mHiddenButtons.Children[0] as AppTabButton).mAppTabIcon.ImageName;
    }

    internal void ShowBlurbOnboarding(JObject res)
    {
      if (!res["is_show_blurbs"].ToObject<bool>() || AppConfigurationManager.Instance.CheckIfTrueInAnyVm(this.PackageName, (Predicate<AppSettings>) (appSettings => appSettings.IsGeneralAppOnBoardingCompleted), out string _))
        return;
      KMManager.sGuidanceWindow?.DimOverLayVisibility(Visibility.Collapsed);
      if (res.ContainsKey("blurbs"))
      {
        JArray jarray = JArray.Parse(res["blurbs"].ToString());
        for (int index = 0; index < jarray.Count; ++index)
        {
          JObject jobject = JObject.Parse(jarray[index].ToString());
          if (Enum.IsDefined(typeof (OnboardingBlurbTags), (object) jobject["tag"].ToString()))
          {
            switch ((OnboardingBlurbTags) Enum.Parse(typeof (OnboardingBlurbTags), jobject["tag"].ToString()))
            {
              case OnboardingBlurbTags.schemeselect:
                IMConfig selectedConfig = this.ParentWindow.SelectedConfig;
                int num1;
                if (selectedConfig == null)
                {
                  num1 = 0;
                }
                else
                {
                  int? count = selectedConfig.ControlSchemesDict?.Count;
                  int num2 = 1;
                  num1 = count.GetValueOrDefault() > num2 & count.HasValue ? 1 : 0;
                }
                if (num1 != 0)
                {
                  OnBoardingPopupWindow boardingPopupWindow = KMManager.sGuidanceWindow?.GuidanceSchemeOnboardingBlurb();
                  if (boardingPopupWindow != null)
                  {
                    KMManager.onBoardingPopupWindows.Add(boardingPopupWindow);
                    continue;
                  }
                  continue;
                }
                continue;
              case OnboardingBlurbTags.guidancedescription:
                OnBoardingPopupWindow boardingPopupWindow1 = KMManager.sGuidanceWindow?.GuidanceOnboardingBlurb();
                if (boardingPopupWindow1 != null)
                {
                  KMManager.onBoardingPopupWindows.Add(boardingPopupWindow1);
                  continue;
                }
                continue;
              case OnboardingBlurbTags.fullscreen:
                if (RegistryManager.Instance.OnboardingBlurbShownCount < 3)
                {
                  OnBoardingPopupWindow boardingPopupWindow2 = this.ParentWindow.mSidebar?.FullscreenOnboardingBlurb();
                  if (boardingPopupWindow2 != null)
                  {
                    KMManager.onBoardingPopupWindows.Add(boardingPopupWindow2);
                    continue;
                  }
                  continue;
                }
                continue;
              case OnboardingBlurbTags.guidancevideo:
                OnBoardingPopupWindow boardingPopupWindow3 = KMManager.sGuidanceWindow?.GuidanceVideoOnboardingBlurb();
                if (boardingPopupWindow3 != null)
                {
                  KMManager.onBoardingPopupWindows.Add(boardingPopupWindow3);
                  continue;
                }
                continue;
              case OnboardingBlurbTags.guidanceclose:
                if (AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName].ContainsKey(this.PackageName))
                {
                  AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName][this.PackageName].IsCloseGuidanceOnboardingCompleted = false;
                  continue;
                }
                continue;
              case OnboardingBlurbTags.keyboardicon:
                if (RegistryManager.Instance.OnboardingBlurbShownCount < 3)
                {
                  OnBoardingPopupWindow boardingPopupWindow2 = this.ParentWindow.mSidebar?.ShowKeyboardIconOnboardingBlurb();
                  if (boardingPopupWindow2 != null)
                  {
                    KMManager.onBoardingPopupWindows.Add(boardingPopupWindow2);
                    continue;
                  }
                  continue;
                }
                continue;
              default:
                continue;
            }
          }
        }
        this.StartBlurbOnboarding();
      }
      else
      {
        IMConfig selectedConfig = this.ParentWindow.SelectedConfig;
        int num1;
        if (selectedConfig == null)
        {
          num1 = 0;
        }
        else
        {
          int? count = selectedConfig.ControlSchemesDict?.Count;
          int num2 = 1;
          num1 = count.GetValueOrDefault() > num2 & count.HasValue ? 1 : 0;
        }
        if (num1 != 0)
        {
          OnBoardingPopupWindow boardingPopupWindow = KMManager.sGuidanceWindow?.GuidanceSchemeOnboardingBlurb();
          if (boardingPopupWindow != null)
            KMManager.onBoardingPopupWindows.Add(boardingPopupWindow);
        }
        this.ShowDefaultBlurbOnboarding();
      }
    }

    internal void ShowDefaultBlurbOnboarding()
    {
      OnBoardingPopupWindow boardingPopupWindow1 = KMManager.sGuidanceWindow?.GuidanceVideoOnboardingBlurb();
      if (boardingPopupWindow1 != null)
        KMManager.onBoardingPopupWindows.Add(boardingPopupWindow1);
      OnBoardingPopupWindow boardingPopupWindow2 = KMManager.sGuidanceWindow?.GuidanceOnboardingBlurb();
      if (boardingPopupWindow2 != null)
        KMManager.onBoardingPopupWindows.Add(boardingPopupWindow2);
      if (RegistryManager.Instance.OnboardingBlurbShownCount < 3)
      {
        OnBoardingPopupWindow boardingPopupWindow3 = this.ParentWindow.mSidebar?.ShowKeyboardIconOnboardingBlurb();
        if (boardingPopupWindow3 != null)
          KMManager.onBoardingPopupWindows.Add(boardingPopupWindow3);
      }
      if (RegistryManager.Instance.OnboardingBlurbShownCount < 3)
      {
        OnBoardingPopupWindow boardingPopupWindow3 = this.ParentWindow.mSidebar?.FullscreenOnboardingBlurb();
        if (boardingPopupWindow3 != null)
          KMManager.onBoardingPopupWindows.Add(boardingPopupWindow3);
      }
      if (AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName].ContainsKey(this.PackageName))
        AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName][this.PackageName].IsCloseGuidanceOnboardingCompleted = false;
      this.StartBlurbOnboarding();
    }

    internal void StartBlurbOnboarding()
    {
      try
      {
        this.ParentWindow.mSidebar.IsOnboardingRunning = true;
        foreach (OnBoardingPopupWindow boardingPopupWindow in KMManager.onBoardingPopupWindows.ToList<OnBoardingPopupWindow>())
        {
          KMManager.onBoardingPopupWindows.Remove(boardingPopupWindow);
          if (!boardingPopupWindow.IsBlurbRelatedToGuidance)
            this.ParentWindow.HideDimOverlay();
          if (KMManager.onBoardingPopupWindows.Count == 0)
            boardingPopupWindow.IsLastPopup = true;
          boardingPopupWindow.ShowDialog();
        }
        if (AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName].ContainsKey(this.PackageName))
          AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName][this.PackageName].IsGeneralAppOnBoardingCompleted = true;
        this.ParentWindow.mSidebar.IsOnboardingRunning = false;
        ++RegistryManager.Instance.OnboardingBlurbShownCount;
      }
      catch (Exception ex)
      {
        Logger.Info("Exception in StartBlurbOnboarding : " + ex.ToString());
      }
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/apptabbutton.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          ((ButtonBase) target).Click += new RoutedEventHandler(this.Button_Click);
          ((UIElement) target).MouseEnter += new MouseEventHandler(this.Button_MouseEnter);
          ((UIElement) target).MouseLeave += new MouseEventHandler(this.Button_MouseLeave);
          ((UIElement) target).PreviewMouseDown += new MouseButtonEventHandler(this.Button_PreviewMouseDown);
          ((UIElement) target).IsEnabledChanged += new DependencyPropertyChangedEventHandler(this.Button_IsEnabledChanged);
          break;
        case 2:
          this.ParallelogramGrid = (Grid) target;
          break;
        case 3:
          this.mBorder = (Border) target;
          break;
        case 4:
          this.mImageColumn = (ColumnDefinition) target;
          break;
        case 5:
          this.mAppTabIcon = (CustomPictureBox) target;
          break;
        case 6:
          this.mTabLabel = (Label) target;
          break;
        case 7:
          this.CloseTabButtonPortrait = (CustomPictureBox) target;
          break;
        case 8:
          this.CloseTabButtonLandScape = (CustomPictureBox) target;
          break;
        case 9:
          this.CloseTabButtonDropDown = (CustomPictureBox) target;
          break;
        case 10:
          this.mDownArrowGrid = (Grid) target;
          break;
        case 11:
          this.Arrow = (System.Windows.Shapes.Path) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
