// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.Sidebar
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
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
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;

namespace BlueStacks.BlueStacksUI
{
  public class Sidebar : UserControl, IComponentConnector
  {
    private Dictionary<SidebarElement, EventHandler> mDictActions = new Dictionary<SidebarElement, EventHandler>();
    internal List<SidebarElement> mListSidebarElements = new List<SidebarElement>();
    private bool mEnableMoreElementPopupTimer = true;
    private readonly object mSyncRoot = new object();
    internal bool mIsKeymappingStateOn = true;
    private string FarmSensitivity = "1";
    internal bool IsOnboardingRunning;
    internal bool IsEcoModeEnabledFromMIM;
    internal bool mIsEcoModeEnabled;
    private int mCurrentVolumeLevel;
    private int mTotalVisibleElements;
    private double mSidebarElementApproxHeight;
    internal bool mIsUIInPortraitModeBeforeChange;
    internal bool mIsOverlayTooltipClosed;
    private bool mIsPendingShowOverlayTooltip;
    internal double mLastSliderValue;
    private bool mIsLoadedOnce;
    private bool mIsInFullscreenMode;
    private DispatcherTimer mMacroBookmarkTimer;
    private DispatcherTimer mGameControlBookmarkTimer;
    private DispatcherTimer mChangeTransparencyPopupTimer;
    internal DispatcherTimer mVolumeSliderPopupTimer;
    internal DispatcherTimer mFarmModePopupTimer;
    private DispatcherTimer mSidebarPopupTimer;
    private string currentScreenshotSavedPath;
    private MainWindow mMainWindow;
    private bool mIsOneSidebarElementLoadedBinded;
    internal List<CustomPopUp> mListPopups;
    private bool mMutedStateBeforeFarmMode;
    internal Sidebar mSidebar;
    internal Border mBorder;
    internal Grid mGrid;
    internal Grid mTopGrid;
    internal StackPanel mElementsStackPanel;
    internal SidebarElement mMoreButton;
    internal CustomPopUp mChangeTransparencyPopup;
    internal Border mMaskBorder2;
    internal CustomPopUp mVolumeSliderPopup;
    internal Border mMaskBorder3;
    internal CustomPictureBox mVolumeMuteUnmuteImage;
    internal Slider mVolumeSlider;
    internal TextBlock mCurrentVolumeValue;
    internal CustomPictureBox mMuteInstancesCheckboxImage;
    internal TextBlock mMuteAllInstancesTextBlock;
    internal CustomPopUp mOverlayTooltip;
    internal Border mMaskBorder4;
    internal TextBlock mOverlayPopUpTitle;
    internal TextBlock mOverlayPopUpMessage;
    internal CustomPictureBox mOverlayDoNotShowCheckboxImage;
    internal TextBlock mOverlayDontShowPopUp;
    internal CustomPopUp mMacroButtonPopup;
    internal Border mMaskBorder5;
    internal MacroBookmarksPopup mMacroBookmarkPopup;
    internal Grid mCustomiseSectionTag;
    internal Separator mCustomiseSectionBorderLine;
    internal TextBlock mOpenMacroTextbox;
    internal CustomPopUp mGameControlButtonPopup;
    internal Border mMaskBorder6;
    internal CustomToggleButtonWithState mGameControlsToggle;
    internal CustomPictureBox mHelpImage;
    internal StackPanel mControlPanel;
    internal CustomToggleButtonWithState mOverlayToggleButton;
    internal CustomTextBlock mControlsForTxtBlock;
    internal CustomPictureBox mGamepadIconImage;
    internal CustomPictureBox mKeyboardIconImage;
    internal Slider transSlider;
    internal DockPanel mViewGameControlsPanel;
    internal CustomTextBlock mViewControlsTxtBlock;
    internal CustomTextBlock mOpenEditorTxtBlock;
    internal StackPanel mBookmarkStackPanel;
    internal StackPanel mBookmarkedSchemesStackPanel;
    internal CustomPopUp mRecordScreenPopup;
    internal Border mMaskBorder7;
    internal CustomPictureBox mRecordScreenClose;
    internal TextBlock RecordScreenPopupHeader;
    internal TextBlock RecordScreenPopupBody;
    internal TextBlock RecordScreenPopupHyperlink;
    internal TextBlock mRecorderClickLink;
    internal CustomPopUp mScreenshotPopup;
    internal Border mMaskBorder8;
    internal CustomPictureBox mScreenshotClose;
    internal TextBlock ScreenshotPopupHeader;
    internal TextBlock mViewInGalleryTextBlock;
    internal CustomPopUp mGameControlsBlurbPopup;
    internal Border mMaskBorder10;
    internal CustomPopUp mEcoModeBlurbPopup;
    internal Border mMaskBorder12;
    internal CustomPopUp mUtcConverterBlurbPopup;
    internal Border mMaskBorder11;
    internal CustomPopUp mMoreElements;
    internal Grid mPopupGrid;
    internal Border mMaskBorder;
    internal SidebarPopup mSidebarPopup;
    internal CustomPopUp mFarmModePopup;
    internal Border mMaskBorder9;
    internal CustomToggleButtonWithState mFarmingModeToggleButton;
    internal CustomPictureBox mEcoModeHelp;
    internal CustomToggleButtonWithState mEcoModeSoundToggleButton;
    internal StepperTextBox mFarmSettingStepperControl;
    internal Grid mBottomGrid;
    internal StackPanel mStaticButtonsStackPanel;
    private bool _contentLoaded;

    internal static Dictionary<string, string> mSidebarIconDict { get; } = new Dictionary<string, string>()
    {
      {
        "sidebar_fullscreen",
        "STRING_UPDATED_FULLSCREEN_BUTTON_TOOLTIP"
      },
      {
        "sidebar_volume",
        "STRING_CHANGE_VOLUME"
      },
      {
        "sidebar_lock_cursor",
        "STRING_TOGGLE_LOCK_CURSOR"
      },
      {
        "sidebar_controls",
        "STRING_GAME_CONTROLS_WINDOW_HEADER"
      },
      {
        "sidebar_macro",
        "STRING_MACRO_RECORDER"
      },
      {
        "sidebar_installapk",
        "STRING_INSTALL_APK"
      },
      {
        "sidebar_screenshot",
        "STRING_TOOLBAR_CAMERA"
      },
      {
        "sidebar_video_capture",
        "STRING_RECORD_SCREEN"
      },
      {
        "sidebar_media_folder",
        "STRING_OPEN_MEDIA_FOLDER"
      },
      {
        "sidebar_utc_converter",
        "STRING_UTC_CONVERTER"
      },
      {
        "sidebar_mm",
        "STRING_TOGGLE_MULTIINSTANCE_WINDOW"
      },
      {
        "sidebar_location",
        "STRING_SET_LOCATION"
      },
      {
        "sidebar_shake",
        "STRING_SHAKE"
      },
      {
        "sidebar_rotate",
        "STRING_ROTATE"
      },
      {
        "sidebar_farmmode_inactive",
        "STRING_ECO_MODE"
      },
      {
        "sidebar_stream_video",
        "STRING_START_STREAMING"
      },
      {
        "sidebar_toggle",
        "STRING_TOGGLE_KEYMAPPING_STATE"
      },
      {
        "sidebar_overlay",
        "STRING_TOGGLE_OVERLAY"
      },
      {
        "sidebar_operation",
        "STRING_SYNCHRONISER"
      },
      {
        "sidebar_gamepad",
        "STRING_GAMEPAD_CONTROLS"
      }
    };

    public MainWindow ParentWindow
    {
      get
      {
        if (this.mMainWindow == null)
          this.mMainWindow = Window.GetWindow((DependencyObject) this) as MainWindow;
        return this.mMainWindow;
      }
    }

    public Sidebar()
    {
      this.InitializeComponent();
      this.mMoreButton.Image.ImageName = "sidebar_options_close";
      if (this.mListPopups == null)
        this.mListPopups = new List<CustomPopUp>(8)
        {
          this.mChangeTransparencyPopup,
          this.mVolumeSliderPopup,
          this.mOverlayTooltip,
          this.mMacroButtonPopup,
          this.mGameControlButtonPopup,
          this.mRecordScreenPopup,
          this.mScreenshotPopup,
          this.mFarmModePopup,
          this.mMoreElements,
          this.mEcoModeBlurbPopup
        };
      BlueStacksUIBinding.Instance.PropertyChanged += new PropertyChangedEventHandler(this.BlueStacksUIBinding_PropertyChanged);
    }

    private void BlueStacksUIBinding_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!(e.PropertyName == "LocaleModel"))
        return;
      this.ParentWindow.mCommonHandler.ReloadTooltips();
    }

    public bool IsFarmingModeEnabled { get; set; }

    internal void BindEvents()
    {
      this.ParentWindow.CursorLockChangedEvent += new MainWindow.CursorLockChangedEventHandler(this.ParentWindow_CursorLockChangedEvent);
      this.ParentWindow.FullScreenChanged += new MainWindow.FullScreenChangedEventHandler(this.ParentWindow_FullScreenChangedEvent);
      this.ParentWindow.FrontendGridVisibilityChanged += new MainWindow.FrontendGridVisibilityChangedEventHandler(this.ParentWindow_FrontendGridVisibleChangedEvent);
      this.ParentWindow.mCommonHandler.ScreenRecordingStateChangedEvent += new CommonHandlers.ScreenRecordingStateChanged(this.ParentWindow_ScreenRecordingStateChangedEvent);
      this.ParentWindow.mCommonHandler.OverlayStateChangedEvent += new CommonHandlers.OverlayStateChanged(this.ParentWindow_OverlayStateChangedEvent);
      this.ParentWindow.mCommonHandler.MacroButtonVisibilityChangedEvent += new CommonHandlers.MacroButtonVisibilityChanged(this.ParentWindow_MacroButtonVisibilityChangedEvent);
      this.ParentWindow.mCommonHandler.OperationSyncButtonVisibilityChangedEvent += new CommonHandlers.OperationSyncButtonVisibilityChanged(this.ParentWindow_OperationSyncButtonVisibilityChangedEvent);
      this.ParentWindow.mCommonHandler.ScreenRecorderStateTransitioningEvent += new CommonHandlers.ScreenRecorderStateTransitioning(this.ParentWindow_ScreenRecordingInitingEvent);
      this.ParentWindow.mCommonHandler.OBSResponseTimeoutEvent += new CommonHandlers.OBSResponseTimeout(this.ParentWindow_OBSResponseTimeoutEvent);
      this.ParentWindow.mCommonHandler.BTvDownloaderMinimizedEvent += new CommonHandlers.BTvDownloaderMinimized(this.ParentWindow_BTvDownloaderMinimizedEvent);
      this.ParentWindow.mCommonHandler.GamepadButtonVisibilityChangedEvent += new CommonHandlers.GamepadButtonVisibilityChanged(this.ParentWindow_GamepadButtonVisibilityChangedEvent);
      this.ParentWindow.mCommonHandler.VolumeChangedEvent += new CommonHandlers.VolumeChanged(this.ParentWindow_VolumeChangedEvent);
      this.ParentWindow.mCommonHandler.VolumeMutedEvent += new CommonHandlers.VolumeMuted(this.ParentWindow_VolumeMutedEvent);
      this.ParentWindow.mCommonHandler.UtcConverterLoadedEvent += new CommonHandlers.UtcConverterLoaded(this.ParentWindow_UtcConverterLoadedEvent);
      this.ParentWindow.mCommonHandler.UtcConverterVisibilityChangedEvent += new CommonHandlers.UtcConverterVisibilityChanged(this.ParentWindow_UtcConverterVisibilityChangedEvent);
      PromotionObject.AppSpecificRulesHandler += new EventHandler(this.PromotionUpdated);
      this.ParentWindow.mCommonHandler.GameGuideButtonVisibilityChangedEvent += new CommonHandlers.GameGuideButtonVisibilityChanged(this.ParentWindow_GameGuideButtonVisibilityChangedEvent);
      if (this.ParentWindow.mGuestBootCompleted)
        this.ToggleBootCompletedState();
      else
        this.ParentWindow.GuestBootCompleted += new MainWindow.GuestBootCompletedEventHandler(this.ParentWindow_GuestBootCompletedEvent);
    }

    private void ParentWindow_GameGuideButtonVisibilityChangedEvent(bool visibility)
    {
      this.ChangeElementState("sidebar_gameguide", visibility);
    }

    private void ParentWindow_UtcConverterVisibilityChangedEvent(bool visibility)
    {
      int num = 3;
      int result1;
      if (PostBootCloudInfoManager.Instance.mPostBootCloudInfo?.UtcConverterInfo.UtcConverterAppPackages?.GetAppPackageObject(this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName)?.ExtraInfo.ContainsKey("red_dot_visibility_days").GetValueOrDefault() && int.TryParse(PostBootCloudInfoManager.Instance.mPostBootCloudInfo?.UtcConverterInfo.UtcConverterAppPackages?.GetAppPackageObject(this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName)?.ExtraInfo["red_dot_visibility_days"], out result1))
        num = result1;
      DateTime result2;
      if (visibility && !RegistryManager.Instance.IsUtcConverterRedDotOnboardingCompleted && (string.IsNullOrEmpty(AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName][this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName].AppInstallTime) || DateTime.TryParse(AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName][this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName].AppInstallTime, (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.None, out result2) && (DateTime.Now.Date - result2.Date).Days >= num))
        this.ShowRedDotOnSidebarElement(this.GetElementFromTag("sidebar_utc_converter"), true);
      else
        this.ShowRedDotOnSidebarElement(this.GetElementFromTag("sidebar_utc_converter"), false);
      this.ToggleElementVisibilty("sidebar_utc_converter", visibility);
    }

    private void PromotionUpdated(object sender, EventArgs e)
    {
      this.ParentWindow.mCommonHandler.ToggleMacroAndSyncVisibility();
    }

    public void UpdateMuteAllInstancesCheckbox()
    {
      this.ParentWindow.Dispatcher.Invoke((Delegate) (() =>
      {
        if (RegistryManager.Instance.AreAllInstancesMuted)
          this.mMuteInstancesCheckboxImage.ImageName = "bgpcheckbox_checked";
        else
          this.mMuteInstancesCheckboxImage.ImageName = "bgpcheckbox";
      }));
    }

    private void ParentWindow_VolumeMutedEvent(bool muted)
    {
      this.ParentWindow.Dispatcher.Invoke((Delegate) (() =>
      {
        if (muted)
        {
          this.mVolumeMuteUnmuteImage.ImageName = "sidebar_volume_muted_popup";
          this.UpdateImage("sidebar_volume", "sidebar_volume_muted");
          if (this.mIsEcoModeEnabled)
            this.mEcoModeSoundToggleButton.BoolValue = false;
        }
        else
        {
          this.mVolumeMuteUnmuteImage.ImageName = "sidebar_volume_popup";
          this.UpdateToDefaultImage("sidebar_volume");
          if (this.mIsEcoModeEnabled)
            this.mEcoModeSoundToggleButton.BoolValue = true;
        }
        this.UpdateMuteAllInstancesCheckbox();
        this.mVolumeSlider.Value = (double) this.ParentWindow.Utils.CurrentVolumeLevel;
        this.mCurrentVolumeValue.Text = this.ParentWindow.Utils.CurrentVolumeLevel.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        Publisher.PublishMessage(BrowserControlTags.getVolumeLevel, this.ParentWindow.mVmName, new JObject()
        {
          ["MutedState"] = (JToken) this.ParentWindow.IsMuted,
          ["VolumeLevel"] = (JToken) this.ParentWindow.Utils.CurrentVolumeLevel
        });
      }));
    }

    private void ParentWindow_VolumeChangedEvent(int volumeLevel)
    {
      this.ParentWindow.Dispatcher.Invoke((Delegate) (() =>
      {
        this.mVolumeSlider.Value = (double) volumeLevel;
        this.mCurrentVolumeValue.Text = volumeLevel.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        Publisher.PublishMessage(BrowserControlTags.getVolumeLevel, this.ParentWindow.mVmName, new JObject()
        {
          ["MutedState"] = (JToken) this.ParentWindow.IsMuted,
          ["VolumeLevel"] = (JToken) this.ParentWindow.Utils.CurrentVolumeLevel
        });
      }));
    }

    private void ParentWindow_GamepadButtonVisibilityChangedEvent(bool visibility)
    {
      this.ChangeElementState("sidebar_gamepad", visibility);
    }

    private void ParentWindow_BTvDownloaderMinimizedEvent()
    {
      this.RecordScreenPopupHeader.Visibility = Visibility.Collapsed;
      this.RecordScreenPopupBody.Visibility = Visibility.Visible;
      this.RecordScreenPopupHyperlink.Visibility = Visibility.Collapsed;
      BlueStacksUIBinding.Bind(this.RecordScreenPopupBody, "STRING_DOWNLOAD_BACKGROUND", "");
      this.mRecordScreenPopup.StaysOpen = false;
      this.mRecordScreenPopup.IsOpen = true;
    }

    private void ParentWindow_OBSResponseTimeoutEvent()
    {
      this.ParentWindow.Dispatcher.Invoke((Delegate) (() =>
      {
        SidebarElement elementFromTag = this.GetElementFromTag("sidebar_video_capture");
        elementFromTag.Image.IsImageToBeRotated = false;
        Sidebar.UpdateToDefaultImage(elementFromTag);
      }));
    }

    private void ParentWindow_ScreenRecordingInitingEvent()
    {
      SidebarElement elementFromTag = this.GetElementFromTag("sidebar_video_capture");
      Sidebar.UpdateImage(elementFromTag, "sidebar_video_loading");
      elementFromTag.Image.Visibility = Visibility.Hidden;
      elementFromTag.Image.IsImageToBeRotated = true;
      elementFromTag.Image.Visibility = Visibility.Visible;
      this.RecordScreenPopupHyperlink.Visibility = Visibility.Collapsed;
      this.RecordScreenPopupBody.Visibility = Visibility.Collapsed;
      this.mRecordScreenClose.Visibility = Visibility.Collapsed;
    }

    private void ParentWindow_OperationSyncButtonVisibilityChangedEvent(bool isVisible)
    {
      if (FeatureManager.Instance.IsCustomUIForNCSoft)
        return;
      this.ToggleElementVisibilty("sidebar_operation", isVisible);
    }

    private void ToggleElementVisibilty(SidebarElement ele, bool isVisible)
    {
      if (ele == null)
        return;
      if (isVisible)
        ele.Visibility = Visibility.Visible;
      else
        ele.Visibility = Visibility.Collapsed;
      if (ele.IsInMainSidebar)
      {
        int index = this.mElementsStackPanel.Children.IndexOf((UIElement) ele);
        int num = this.mListSidebarElements.IndexOf(ele);
        if (index != -1 && index != num)
        {
          this.mElementsStackPanel.Children.RemoveAt(index);
          int count = this.mElementsStackPanel.Children.Count;
          if (num >= count)
            ele.IsInMainSidebar = false;
          else
            this.mElementsStackPanel.Children.Insert(num + 1, (UIElement) ele);
        }
      }
      this.FixMarginOfSurroundingElement(ele);
      this.UpdateTotalVisibleElementCount();
      this.ArrangeAllSidebarElements();
    }

    private SidebarElement GetPreviousVisibleSidebarElement(SidebarElement ele)
    {
      SidebarElement previousSidebarElement = this.GetPreviousSidebarElement(ele);
      return previousSidebarElement.Visibility != Visibility.Visible ? this.GetPreviousSidebarElement(previousSidebarElement) : previousSidebarElement;
    }

    private SidebarElement GetPreviousSidebarElement(SidebarElement ele)
    {
      int num = this.mListSidebarElements.IndexOf(ele);
      return num != 0 ? this.mListSidebarElements[num - 1] : ele;
    }

    private void FixMarginOfSurroundingElement(SidebarElement currentElement)
    {
      if (currentElement != null && currentElement.Visibility == Visibility.Visible)
      {
        if (!currentElement.IsLastElementOfGroup || currentElement.IsCurrentLastElementOfGroup)
          return;
        currentElement.IsCurrentLastElementOfGroup = true;
        Sidebar.IncreaseElementBottomMarginIfLast(currentElement);
        SidebarElement visibleSidebarElement = this.GetPreviousVisibleSidebarElement(currentElement);
        if (visibleSidebarElement == currentElement)
          return;
        visibleSidebarElement.IsCurrentLastElementOfGroup = false;
        Sidebar.DecreaseElementBottomMargin(visibleSidebarElement);
        Thickness margin = visibleSidebarElement.Margin;
        margin.Bottom = 2.0;
        visibleSidebarElement.Margin = margin;
      }
      else
      {
        if (!currentElement.IsCurrentLastElementOfGroup)
          return;
        currentElement.IsCurrentLastElementOfGroup = false;
        SidebarElement visibleSidebarElement = this.GetPreviousVisibleSidebarElement(currentElement);
        if (visibleSidebarElement == currentElement)
          return;
        visibleSidebarElement.IsCurrentLastElementOfGroup = true;
        Sidebar.IncreaseElementBottomMarginIfLast(visibleSidebarElement);
      }
    }

    private void ToggleElementVisibilty(string elementKey, bool isVisible)
    {
      this.ParentWindow.Dispatcher.Invoke((Delegate) (() => this.ToggleElementVisibilty(this.GetElementFromTag(elementKey), isVisible)));
    }

    private void ParentWindow_MacroButtonVisibilityChangedEvent(bool isVisible)
    {
      if (FeatureManager.Instance.IsCustomUIForNCSoft)
        return;
      this.ToggleElementVisibilty("sidebar_macro", isVisible);
    }

    private void OverlayStateChange(bool isEnabled)
    {
      if (isEnabled)
      {
        if (RegistryManager.Instance.TranslucentControlsTransparency == 0.0)
        {
          if (this.mLastSliderValue == 0.0)
          {
            RegistryManager.Instance.TranslucentControlsTransparency = 0.5;
            this.transSlider.Value = 0.5;
          }
          else
          {
            RegistryManager.Instance.TranslucentControlsTransparency = this.mLastSliderValue;
            this.transSlider.Value = this.mLastSliderValue;
          }
        }
      }
      else
      {
        double num = this.transSlider.Value;
        this.transSlider.Value = 0.0;
        this.mLastSliderValue = num;
      }
      KMManager.ShowOverlayWindow(this.ParentWindow, isEnabled, false);
    }

    private void ParentWindow_OverlayStateChangedEvent(bool isEnabled)
    {
      bool flag = !RegistryManager.Instance.ShowKeyControlsOverlay;
      if (this.mOverlayToggleButton != null)
        this.mOverlayToggleButton.BoolValue = flag;
      RegistryManager.Instance.ShowKeyControlsOverlay = flag;
      this.OverlayStateChange(flag);
      KMManager.ShowOverlayWindow(this.ParentWindow, flag, false);
    }

    private void ParentWindow_ScreenRecordingStateChangedEvent(bool isRecording)
    {
      this.ParentWindow.Dispatcher.Invoke((Delegate) (() =>
      {
        SidebarElement elementFromTag = this.GetElementFromTag("sidebar_video_capture");
        elementFromTag.Image.IsImageToBeRotated = false;
        if (isRecording)
        {
          Sidebar.UpdateImage(elementFromTag, "sidebar_video_capture_active");
          this.ChangeElementState("sidebar_fullscreen", false);
          BlueStacksUIBinding.Bind(this.RecordScreenPopupHeader, "STRING_STOP_RECORDING", "");
          this.RecordScreenPopupHeader.Visibility = Visibility.Visible;
          this.RecordScreenPopupHyperlink.Visibility = Visibility.Collapsed;
          this.RecordScreenPopupBody.Visibility = Visibility.Collapsed;
          this.mRecordScreenClose.Visibility = Visibility.Collapsed;
        }
        else
        {
          Sidebar.UpdateToDefaultImage(elementFromTag);
          this.RecordScreenPopupBody.Visibility = Visibility.Visible;
          this.RecordScreenPopupHeader.Visibility = Visibility.Visible;
          BlueStacksUIBinding.Bind(this.RecordScreenPopupHeader, "STRING_RECORDING_SAVED", "");
          BlueStacksUIBinding.Bind(this.RecordScreenPopupBody, "STRING_CLICK_TO_SEE_VIDEO", "");
          this.RecordScreenPopupBody.Visibility = Visibility.Collapsed;
          this.RecordScreenPopupHyperlink.Visibility = Visibility.Visible;
          BlueStacksUIBinding.Bind(this.mRecorderClickLink, "STRING_CLICK_TO_SEE_VIDEO", "");
          this.RecordScreenPopupBody.Visibility = Visibility.Visible;
          this.mRecordScreenClose.Visibility = Visibility.Visible;
          if (this.ParentWindow.mIsWindowInFocus && elementFromTag.IsInMainSidebar)
          {
            this.mRecordScreenPopup.PlacementTarget = (UIElement) elementFromTag;
            this.mRecordScreenPopup.StaysOpen = false;
            this.mRecordScreenPopup.IsOpen = true;
          }
          if (RegistryManager.Instance.IsShowToastNotification)
            this.ParentWindow.ShowGeneralToast(LocaleStrings.GetLocalizedString("STRING_RECORDING_SAVED", ""));
          if (this.ParentWindow.mFrontendGrid.IsVisible)
            this.ChangeElementState("sidebar_fullscreen", true);
        }
        this.SetVideoRecordingTooltip(isRecording);
      }));
    }

    internal void ShowScreenshotSavedPopup(string screenshotPath)
    {
      this.ParentWindow.Dispatcher.Invoke((Delegate) (() =>
      {
        SidebarElement elementFromTag = this.GetElementFromTag("sidebar_screenshot");
        this.SetSidebarElementTooltip(elementFromTag, "STRING_TOOLBAR_CAMERA");
        if (!this.ParentWindow.mIsWindowInFocus || !elementFromTag.IsInMainSidebar)
          return;
        this.mViewInGalleryTextBlock.Visibility = this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.mTabType == TabType.AppTab ? Visibility.Visible : Visibility.Collapsed;
        this.mScreenshotPopup.PlacementTarget = (UIElement) elementFromTag;
        this.mScreenshotPopup.StaysOpen = false;
        this.mScreenshotPopup.IsOpen = true;
        this.currentScreenshotSavedPath = screenshotPath;
      }));
    }

    private void ParentWindow_FrontendGridVisibleChangedEvent(
      object sender,
      MainWindowEventArgs.FrontendGridVisibilityChangedEventArgs args)
    {
      this.ChangeElementState("sidebar_lock_cursor", args.IsVisible);
      if (!CommonHandlers.sIsRecordingVideo)
        this.ChangeElementState("sidebar_fullscreen", args.IsVisible);
      this.ChangeElementState("sidebar_toggle", args.IsVisible);
      this.ChangeElementState("sidebar_controls", args.IsVisible);
      this.ChangeElementState("sidebar_overlay", args.IsVisible);
      this.ChangeElementState("sidebar_back", args.IsVisible);
      this.ChangeElementState("sidebar_home", args.IsVisible);
      this.ChangeElementState("sidebar_video_capture", args.IsVisible);
      this.ChangeElementState("sidebar_farmmode_inactive", args.IsVisible);
      if (args.IsVisible)
        return;
      this.ChangeElementState("sidebar_gamepad", args.IsVisible);
      this.ChangeElementState("sidebar_gameguide", args.IsVisible);
    }

    private void InitDefaultSettings()
    {
      if (this.ParentWindow.mIsFullScreen)
        this.UpdateImage("sidebar_fullscreen", "sidebar_fullscreen_minimize");
      if (!FeatureManager.Instance.IsCustomUIForNCSoft)
      {
        this.ToggleElementVisibilty("sidebar_macro", false);
        this.ToggleElementVisibilty("sidebar_operation", false);
        if (RegistryManager.Instance.TranslucentControlsTransparency == 0.0)
          this.UpdateImage("sidebar_overlay", "sidebar_overlay_inactive");
      }
      else
      {
        this.ToggleElementVisibilty("sidebar_overlay", false);
        this.ToggleElementVisibilty("sidebar_overlay_inactive", false);
      }
      this.transSlider.Value = RegistryManager.Instance.TranslucentControlsTransparency;
      this.mOverlayPopUpMessage.Text = string.Format((IFormatProvider) CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_ON_SCREEN_CONTROLS_BODY", ""), (object) this.ParentWindow.mCommonHandler.GetShortcutKeyFromName("STRING_TOGGLE_OVERLAY", false));
      if (FeatureManager.Instance.IsFarmingModeDisabled)
        this.ToggleElementVisibilty("sidebar_farmmode_inactive", false);
      else
        this.SetupEcoModeInitState();
      this.ToggleElementVisibilty("sidebar_utc_converter", false);
    }

    private void SetupEcoModeInitState()
    {
      this.mFarmingModeToggleButton.BoolValue = this.mIsEcoModeEnabled;
      this.UpdateImage("sidebar_farmmode_inactive", this.mFarmingModeToggleButton.BoolValue ? "sidebar_farmmode_active" : "sidebar_farmmode_inactive");
      this.mEcoModeSoundToggleButton.BoolValue = false;
      this.FarmSensitivity = RegistryManager.Instance.Guest[this.ParentWindow.mVmName].EcoModeFPS;
      this.mFarmSettingStepperControl.Text = this.FarmSensitivity;
    }

    internal void HideSideBarInFullscreen()
    {
      this.ParentWindow.mFullscreenSidebarPopupButton.IsOpen = false;
      this.ParentWindow.mFullscreenSidebarPopup.IsOpen = false;
    }

    internal void ToggleSidebarVisibilityInFullscreen(bool isVisible)
    {
      if (isVisible)
      {
        this.ParentWindow.mFullscreenSidebarPopup.Height = this.ParentWindow.MainGrid.ActualHeight;
        this.ParentWindow.mFullscreenSidebarPopup.HorizontalOffset = this.ParentWindow.MainGrid.ActualWidth / 2.0;
        this.ParentWindow.mFullscreenSidebarPopupInnerGrid.Height = this.ParentWindow.MainGrid.ActualHeight;
        ClientStats.SendMiscellaneousStatsAsync("fullscreen", RegistryManager.Instance.UserGuid, "sideBarButton", "MouseClick", RegistryManager.Instance.ClientVersion, Oem.Instance.OEM, (string) null, (string) null, (string) null, "Android");
      }
      else
      {
        this.mListPopups.All<CustomPopUp>((Func<CustomPopUp, bool>) (x => x.IsOpen = false));
        this.ParentWindow.AllowFrontendFocusOnClientClick = true;
      }
      this.ParentWindow.mFullscreenSidebarPopup.IsOpen = isVisible;
      this.ParentWindow.mFullscreenSidebarPopupButton.IsOpen = false;
    }

    internal void ToggleSidebarButtonVisibilityInFullscreen(bool isVisible)
    {
      if (isVisible && !this.ParentWindow.mFullscreenTopbarPopup.IsOpen && (!this.ParentWindow.mFullscreenSidebarPopup.IsOpen && this.ParentWindow.EngineInstanceRegistry.ShowBlueHighlighter))
      {
        this.ParentWindow.mFullscreenSidebarPopupButtonInnerGrid.Height = this.ParentWindow.MainGrid.ActualHeight;
        this.ParentWindow.mFullscreenSidebarPopupButton.Height = this.ParentWindow.MainGrid.ActualHeight;
        this.ParentWindow.mFullscreenSidebarPopupButton.HorizontalOffset = this.ParentWindow.MainGrid.ActualWidth / 2.0;
        this.ParentWindow.mFullscreenSidebarPopupButton.IsOpen = true;
      }
      else
      {
        if (isVisible)
          return;
        this.mListPopups.All<CustomPopUp>((Func<CustomPopUp, bool>) (x => x.IsOpen = false));
        this.ParentWindow.AllowFrontendFocusOnClientClick = true;
        this.ParentWindow.mFullscreenSidebarPopupButton.IsOpen = false;
      }
    }

    private void ParentWindow_FullScreenChangedEvent(
      object sender,
      MainWindowEventArgs.FullScreenChangedEventArgs args)
    {
      lock (this.mSyncRoot)
      {
        this.mIsInFullscreenMode = args.IsFullscreen;
        if (this.mIsInFullscreenMode)
        {
          this.UpdateImage("sidebar_fullscreen", "sidebar_fullscreen_minimize");
        }
        else
        {
          this.UpdateImage("sidebar_fullscreen", "sidebar_fullscreen");
          this.ParentWindow.mFullscreenSidebarPopup.IsOpen = false;
          this.ParentWindow.mFullscreenSidebarPopupButton.IsOpen = false;
          if (this.ParentWindow.mIsFullScreen && this.ParentWindow.EngineInstanceRegistry.IsSidebarVisible)
            this.ParentWindow.mFullScreenRestoredButNotSidebar = true;
        }
        this.SetupSidebarForFullscreen(this.mIsInFullscreenMode);
        this.ArrangeAllSidebarElements();
      }
    }

    private void SetupSidebarForFullscreen(bool isFullScreen)
    {
      this.ParentWindow.Dispatcher.BeginInvoke((Delegate) (() =>
      {
        if (isFullScreen)
        {
          if (this.ParentWindow.mMainWindowTopGrid.Children.Contains((UIElement) this))
          {
            this.ParentWindow.mMainWindowTopGrid.Children.Remove((UIElement) this);
            this.ParentWindow.mFullscreenSidebarPopupInnerGrid.Children.Add((UIElement) this);
          }
          this.Visibility = Visibility.Visible;
        }
        else
        {
          if (this.ParentWindow.mFullscreenSidebarPopupInnerGrid.Children.Contains((UIElement) this))
          {
            this.ParentWindow.mFullscreenSidebarPopupInnerGrid.Children.Remove((UIElement) this);
            this.ParentWindow.mMainWindowTopGrid.Children.Add((UIElement) this);
          }
          this.Visibility = this.ParentWindow.EngineInstanceRegistry.IsSidebarVisible ? Visibility.Visible : Visibility.Collapsed;
          this.ParentWindow.mFullScreenRestoredButNotSidebar = false;
        }
      }), DispatcherPriority.DataBind);
    }

    private void ParentWindow_CursorLockChangedEvent(
      object sender,
      MainWindowEventArgs.CursorLockChangedEventArgs args)
    {
      if (args.IsLocked)
        this.UpdateImage("sidebar_lock_cursor", "sidebar_lock_cursor_active");
      else
        this.UpdateImage("sidebar_lock_cursor", "sidebar_lock_cursor");
    }

    private void ParentWindow_GuestBootCompletedEvent(object sender, EventArgs args)
    {
      this.ToggleBootCompletedState();
    }

    private void ToggleBootCompletedState()
    {
      this.ParentWindow.Dispatcher.Invoke((Delegate) (() =>
      {
        this.ChangeElementState("sidebar_stream_video", true);
        this.ChangeElementState("sidebar_volume", true);
        this.ChangeElementState("sidebar_macro", true);
        this.ChangeElementState("sidebar_operation", true);
        this.ChangeElementState("sidebar_location", true);
        this.ChangeElementState("sidebar_rotate", true);
        this.ChangeElementState("sidebar_installapk", true);
      }));
    }

    private static void ChangeElementState(SidebarElement ele, bool isEnabled)
    {
      if (ele == null)
        return;
      ele.IsEnabled = isEnabled;
    }

    private void ChangeElementState(string elementTag, bool isEnabled)
    {
      Sidebar.ChangeElementState(this.GetElementFromTag(elementTag), isEnabled);
    }

    private void Sidebar_Loaded(object sender, RoutedEventArgs e)
    {
      if (Oem.IsOEMDmm)
        return;
      if (!this.mIsLoadedOnce)
      {
        this.mIsLoadedOnce = true;
        this.BindEvents();
        this.SetPlacementTargets();
        this.InitDefaultSettings();
        this.mMacroBookmarkPopup.SetParentWindowAndBindEvents(this.ParentWindow);
        this.ParentWindow.mCommonHandler.ToggleMacroAndSyncVisibility();
      }
      this.ParentWindow.mCommonHandler.ClipMouseCursorHandler(false, false, "", "");
      this.SetVideoRecordingTooltipForNCSoft();
    }

    private void MMacroButtonAndPopup_MouseLeave(object sender, MouseEventArgs e)
    {
      if (!this.mMacroButtonPopup.IsOpen)
        return;
      if (this.mMacroBookmarkTimer == null)
      {
        this.mMacroBookmarkTimer = new DispatcherTimer()
        {
          Interval = new TimeSpan(0, 0, 0, 0, 500)
        };
        this.mMacroBookmarkTimer.Tick += new EventHandler(this.MMacroBookmarkTimer_Tick);
      }
      else
        this.mMacroBookmarkTimer.Stop();
      this.mMacroBookmarkTimer.Start();
    }

    private void MMacroBookmarkTimer_Tick(object sender, EventArgs e)
    {
      if (!this.mMacroButtonPopup.IsMouseOver && !this.GetElementFromTag("sidebar_macro").IsMouseOver)
      {
        this.mMacroButtonPopup.IsOpen = false;
        if (this.mIsInFullscreenMode && !this.IsMouseOver)
          this.ToggleSidebarVisibilityInFullscreen(false);
      }
      (sender as DispatcherTimer).Stop();
    }

    private void MacroButtonHandler(object sender, EventArgs e)
    {
      if (this.ParentWindow.mIsMacroRecorderActive)
        this.ParentWindow.ShowToast(LocaleStrings.GetLocalizedString("STRING_STOP_RECORDING_FIRST", ""), "", "", false);
      else if (RegistryManager.Instance.BookmarkedScriptList.Length != 0 && !this.mMoreElements.IsOpen)
      {
        this.mMacroButtonPopup.IsOpen = true;
      }
      else
      {
        this.ParentWindow.mCommonHandler.ShowMacroRecorderWindow();
        this.mMacroButtonPopup.IsOpen = false;
        this.ToggleSidebarVisibilityInFullscreen(false);
        ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "MacroRecorder", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
      }
    }

    private void OperationSyncHandler(object sender, EventArgs e)
    {
      this.ParentWindow.ShowSynchronizerWindow();
      ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "OperationSync", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
    }

    private void KeymapToggleHandler(object sender, EventArgs e)
    {
      this.KeyMapSwitchButtonHandler(sender as SidebarElement, false);
    }

    private void KeyMapControlsButton_PreviewMouseRightButtonUp(
      object sender,
      MouseButtonEventArgs e)
    {
      Logger.Info("Right Click on keymap control UI button ");
      try
      {
        KMManager.sIsDeveloperModeOn = (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) && (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt));
        KMManager.LoadIMActions(this.ParentWindow, this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName);
        KMManager.ShowAdvancedSettings(this.ParentWindow);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception on right click on keymap button: " + ex.ToString());
      }
    }

    internal void KeyMapSwitchButtonHandler(SidebarElement ele, bool fromIconClick = false)
    {
      this.ParentWindow.Dispatcher.Invoke((Delegate) (() =>
      {
        if (KMManager.sIsComboRecordingOn)
          return;
        if (!this.mIsKeymappingStateOn)
        {
          Sidebar.UpdateToDefaultImage(ele);
          this.mIsKeymappingStateOn = true;
          if (!fromIconClick)
            this.mGameControlsToggle.BoolValue = true;
          this.ParentWindow.mFrontendHandler.SendFrontendRequest("enableNativeGamepad", new Dictionary<string, string>()
          {
            {
              "isEnabled",
              (this.ParentWindow?.StaticComponents?.mSelectedTabButton != null && this.ParentWindow.StaticComponents.mSelectedTabButton.mIsNativeGamepadEnabledForApp).ToString((IFormatProvider) CultureInfo.InvariantCulture)
            }
          });
          this.ParentWindow.mFrontendHandler.EnableKeyMapping(true);
          ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "ToggleKeymapOn", fromIconClick ? "MouseClick" : "Shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
        }
        else
        {
          Sidebar.UpdateImage(ele, "sidebar_toggle_off");
          this.mIsKeymappingStateOn = false;
          if (!fromIconClick)
            this.mGameControlsToggle.BoolValue = false;
          this.ParentWindow.mFrontendHandler.EnableKeyMapping(false);
          ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "ToggleKeymapOff", fromIconClick ? "MouseClick" : "Shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
        }
      }));
    }

    private void SetPlacementTargets()
    {
      this.mChangeTransparencyPopup.PlacementTarget = (UIElement) this.GetElementFromTag("sidebar_overlay");
      this.mVolumeSliderPopup.PlacementTarget = (UIElement) this.GetElementFromTag("sidebar_volume");
      this.mOverlayTooltip.PlacementTarget = (UIElement) this.GetElementFromTag("sidebar_overlay");
      this.mRecordScreenPopup.PlacementTarget = (UIElement) this.GetElementFromTag("sidebar_video_capture");
      this.mScreenshotPopup.PlacementTarget = (UIElement) this.GetElementFromTag("sidebar_screenshot");
      this.mMacroButtonPopup.PlacementTarget = (UIElement) this.GetElementFromTag("sidebar_macro");
      this.mGameControlButtonPopup.PlacementTarget = (UIElement) this.GetElementFromTag("sidebar_controls");
      this.mFarmModePopup.PlacementTarget = (UIElement) this.GetElementFromTag("sidebar_farmmode_inactive");
    }

    internal void InitElements()
    {
      this.CreateAndAddElementsToStackPanel();
      this.InitStaticElements();
      this.UpdateTotalVisibleElementCount();
    }

    private void UpdateTotalVisibleElementCount()
    {
      this.mTotalVisibleElements = this.mListSidebarElements.Where<SidebarElement>((Func<SidebarElement, bool>) (item => item.Visibility == Visibility.Visible)).Count<SidebarElement>();
      if (FeatureManager.Instance.IsCustomUIForNCSoft)
        --this.mTotalVisibleElements;
      else
        this.mTotalVisibleElements -= 3;
    }

    private void InitStaticElements()
    {
      SidebarElement element1 = this.CreateElement("sidebar_gameguide", "STRING_TOGGLE_KEYMAP_WINDOW", new EventHandler(this.OpenGameGuideButtonHandler));
      this.AddElement(element1, true);
      Sidebar.ChangeElementState(element1, false);
      if (!FeatureManager.Instance.IsCustomUIForNCSoft)
        this.AddElement(this.CreateElement("sidebar_settings", "STRING_SETTINGS", new EventHandler(this.GoSettingsHandler)), true);
      SidebarElement element2 = this.CreateElement("sidebar_back", "STRING_BACK", new EventHandler(this.GoBackHandler));
      this.AddElement(element2, true);
      Sidebar.ChangeElementState(element2, false);
    }

    public void RearrangeSidebarIcons()
    {
      Dictionary<string, SidebarElement> dictionary = new Dictionary<string, SidebarElement>();
      foreach (SidebarElement listSidebarElement in this.mListSidebarElements)
      {
        if (listSidebarElement != null)
        {
          SidebarElement sidebarElement = listSidebarElement;
          sidebarElement.IsInMainSidebar = true;
          dictionary.Add(sidebarElement.Tag.ToString(), sidebarElement);
        }
      }
      SidebarElement sidebarElement1 = this.mListSidebarElements.Where<SidebarElement>((Func<SidebarElement, bool>) (ele => string.Equals(ele.Tag.ToString(), "sidebar_gameguide", StringComparison.InvariantCultureIgnoreCase))).FirstOrDefault<SidebarElement>();
      SidebarElement sidebarElement2 = this.mListSidebarElements.Where<SidebarElement>((Func<SidebarElement, bool>) (ele => string.Equals(ele.Tag.ToString(), "sidebar_settings", StringComparison.InvariantCultureIgnoreCase))).FirstOrDefault<SidebarElement>();
      SidebarElement sidebarElement3 = this.mListSidebarElements.Where<SidebarElement>((Func<SidebarElement, bool>) (ele => string.Equals(ele.Tag.ToString(), "sidebar_back", StringComparison.InvariantCultureIgnoreCase))).FirstOrDefault<SidebarElement>();
      this.mElementsStackPanel.Children.Clear();
      this.mListSidebarElements.Clear();
      foreach (string key in RegistryManager.Instance.UserDefinedSidebarElements.Length == 0 ? RegistryManager.Instance.DefaultSidebarElements : RegistryManager.Instance.UserDefinedSidebarElements)
      {
        if (dictionary.ContainsKey(key))
        {
          this.mElementsStackPanel.Children.Add((UIElement) dictionary[key]);
          this.mListSidebarElements.Add(dictionary[key]);
        }
      }
      if (sidebarElement1 != null)
        this.mListSidebarElements.Add(sidebarElement1);
      if (sidebarElement2 != null)
        this.mListSidebarElements.Add(sidebarElement2);
      if (sidebarElement3 != null)
        this.mListSidebarElements.Add(sidebarElement3);
      this.ArrangeAllSidebarElements();
    }

    private void CreateAndAddElementsToStackPanel()
    {
      foreach (string str in RegistryManager.Instance.UserDefinedSidebarElements.Length == 0 ? RegistryManager.Instance.DefaultSidebarElements : RegistryManager.Instance.UserDefinedSidebarElements)
      {
        switch (str)
        {
          case "sidebar_controls":
            SidebarElement element1 = this.CreateElement("sidebar_controls", "STRING_GAME_CONTROLS_WINDOW_HEADER", new EventHandler(this.GameControlButtonHandler));
            element1.PreviewMouseRightButtonUp += new MouseButtonEventHandler(this.KeyMapControlsButton_PreviewMouseRightButtonUp);
            element1.MouseLeave += new MouseEventHandler(this.GameControlButtonPopup_MouseLeave);
            this.AddElement(element1, false);
            Sidebar.ChangeElementState(element1, false);
            break;
          case "sidebar_farmmode_inactive":
            SidebarElement element2 = this.CreateElement("sidebar_farmmode_inactive", "STRING_ECO_MODE", new EventHandler(this.OpenFarmModePopupButtonHandler));
            element2.MouseLeave += new MouseEventHandler(this.FarmModePopup_MouseLeave);
            this.AddElement(element2, false);
            Sidebar.ChangeElementState(element2, false);
            this.mFarmingModeToggleButton.BoolValue = this.IsFarmingModeEnabled;
            break;
          case "sidebar_fullscreen":
            SidebarElement element3 = this.CreateElement("sidebar_fullscreen", "STRING_UPDATED_FULLSCREEN_BUTTON_TOOLTIP", new EventHandler(this.FullScreenHandler));
            this.AddElement(element3, false);
            Sidebar.ChangeElementState(element3, false);
            break;
          case "sidebar_gamepad":
            SidebarElement element4 = this.CreateElement("sidebar_gamepad", "STRING_GAMEPAD_CONTROLS", new EventHandler(this.GamepadControlsWindowHandler));
            this.AddElement(element4, false);
            Sidebar.ChangeElementState(element4, false);
            break;
          case "sidebar_installapk":
            SidebarElement element5 = this.CreateElement("sidebar_installapk", "STRING_INSTALL_APK", new EventHandler(this.InstallApkHandler));
            this.AddElement(element5, false);
            Sidebar.ChangeElementState(element5, false);
            break;
          case "sidebar_location":
            SidebarElement element6 = this.CreateElement("sidebar_location", "STRING_SET_LOCATION", new EventHandler(this.LocationHandler));
            this.AddElement(element6, false);
            Sidebar.ChangeElementState(element6, false);
            break;
          case "sidebar_lock_cursor":
            SidebarElement element7 = this.CreateElement("sidebar_lock_cursor", "STRING_TOGGLE_LOCK_CURSOR", new EventHandler(this.LockCursorHandler));
            this.AddElement(element7, false);
            Sidebar.ChangeElementState(element7, false);
            break;
          case "sidebar_macro":
            SidebarElement element8 = this.CreateElement("sidebar_macro", "STRING_MACRO_RECORDER", new EventHandler(this.MacroButtonHandler));
            element8.MouseLeave += new MouseEventHandler(this.MMacroButtonAndPopup_MouseLeave);
            this.AddElement(element8, false);
            break;
          case "sidebar_media_folder":
            this.AddElement(this.CreateElement("sidebar_media_folder", "STRING_OPEN_MEDIA_FOLDER", new EventHandler(this.MediaFolderHandler)), false);
            break;
          case "sidebar_mm":
            this.AddElement(this.CreateElement("sidebar_mm", "STRING_TOGGLE_MULTIINSTANCE_WINDOW", new EventHandler(this.MIManagerHandler)), false);
            break;
          case "sidebar_operation":
            this.AddElement(this.CreateElement("sidebar_operation", "STRING_SYNCHRONISER", new EventHandler(this.OperationSyncHandler)), false);
            break;
          case "sidebar_overlay":
            SidebarElement element9 = this.CreateElement("sidebar_overlay", "STRING_TOGGLE_OVERLAY", new EventHandler(this.KeymappingControlsTransparencyButtonHandler));
            this.AddElement(element9, false);
            element9.MouseLeave += new MouseEventHandler(this.ChangeTransparencyPopup_MouseLeave);
            Sidebar.ChangeElementState(element9, false);
            break;
          case "sidebar_rotate":
            SidebarElement element10 = this.CreateElement("sidebar_rotate", "STRING_ROTATE", new EventHandler(this.RotateHandler));
            this.AddElement(element10, false);
            Sidebar.ChangeElementState(element10, false);
            break;
          case "sidebar_screenshot":
            this.AddElement(this.CreateElement("sidebar_screenshot", "STRING_TOOLBAR_CAMERA", new EventHandler(this.ScreenshotHandler)), false);
            break;
          case "sidebar_shake":
            this.AddElement(this.CreateElement("sidebar_shake", "STRING_SHAKE", new EventHandler(this.ShakeHandler)), false);
            break;
          case "sidebar_stream_video":
            SidebarElement element11 = this.CreateElement("sidebar_stream_video", "STRING_START_STREAMING", new EventHandler(this.StreamingHandler));
            this.AddElement(element11, false);
            Sidebar.ChangeElementState(element11, false);
            break;
          case "sidebar_toggle":
            SidebarElement element12 = this.CreateElement("sidebar_toggle", "STRING_TOGGLE_KEYMAPPING_STATE", new EventHandler(this.KeymapToggleHandler));
            this.AddElement(element12, false);
            Sidebar.ChangeElementState(element12, false);
            break;
          case "sidebar_utc_converter":
            this.AddElement(this.CreateElement("sidebar_utc_converter", "STRING_UTC_CONVERTER", new EventHandler(this.OpenUtcConverterHandler)), false);
            break;
          case "sidebar_video_capture":
            SidebarElement element13 = this.CreateElement("sidebar_video_capture", "STRING_RECORD_SCREEN", new EventHandler(this.ScreenRecorderButtonHandler));
            this.AddElement(element13, false);
            Sidebar.ChangeElementState(element13, false);
            break;
          case "sidebar_volume":
            SidebarElement element14 = this.CreateElement("sidebar_volume", "STRING_CHANGE_VOLUME", new EventHandler(this.VolumeButtonHandler));
            this.AddElement(element14, false);
            element14.MouseLeave += new MouseEventHandler(this.VolumeSliderPopup_MouseLeave);
            Sidebar.ChangeElementState(element14, false);
            break;
          default:
            Logger.Warning("Unhandled sidebar element found: {0}", (object) str);
            break;
        }
      }
    }

    private void InstallApkHandler(object sender, EventArgs e)
    {
      ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "InstallApk", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
      this.ParentWindow.mCommonHandler.InstallApkHandler();
    }

    private void StreamingHandler(object sender, EventArgs e)
    {
      bool mIsStreaming = this.ParentWindow.mIsStreaming;
      NCSoftUtils.Instance.SendStreamingEvent(this.ParentWindow.mVmName, mIsStreaming ? "off" : "on");
      ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, mIsStreaming ? "StreamVideoOff" : "StreamVideoOn", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
    }

    private void OpenUtcConverterHandler(object sender, EventArgs e)
    {
      SidebarElement elementFromTag = this.GetElementFromTag("sidebar_utc_converter");
      Sidebar.UpdateImage(elementFromTag, "sidebar_video_loading");
      elementFromTag.Image.Visibility = Visibility.Hidden;
      elementFromTag.Image.IsImageToBeRotated = true;
      elementFromTag.Image.Visibility = Visibility.Visible;
      new SideHtmlWidgetWindow(this.ParentWindow, this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab?.PackageName).Show();
      ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "UtcConverter", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab?.PackageName, (string) null, "Android");
    }

    private void ParentWindow_UtcConverterLoadedEvent()
    {
      this.ParentWindow.Dispatcher.Invoke((Delegate) (() =>
      {
        SidebarElement elementFromTag = this.GetElementFromTag("sidebar_utc_converter");
        elementFromTag.Image.IsImageToBeRotated = false;
        Sidebar.UpdateImage(elementFromTag, "sidebar_utc_converter");
      }));
    }

    private void GamepadControlsWindowHandler(object sender, EventArgs e)
    {
      if (!this.ParentWindow.mCommonHandler.ToggleGamepadAndKeyboardGuidance("gamepad", false))
        KMManager.HandleInputMapperWindow(this.ParentWindow, "gamepad");
      ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "GamePad", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab?.PackageName, (string) null, "Android");
    }

    private void MediaFolderHandler(object sender, EventArgs e)
    {
      CommonHandlers.OpenMediaFolder();
      ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "MediaFolder", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
    }

    private void GoBackHandler(object sender, EventArgs e)
    {
      this.ParentWindow.mCommonHandler.BackButtonHandler(false);
      ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "Back", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
    }

    private void GoHomeHandler(object sender, EventArgs e)
    {
      this.ParentWindow.mCommonHandler.HomeButtonHandler(true, false);
    }

    private void GoSettingsHandler(object sender, EventArgs e)
    {
      string tabName = string.Empty;
      if (this.ParentWindow.StaticComponents.mSelectedTabButton.mTabType == TabType.AppTab && !PackageActivityNames.SystemApps.Contains(this.ParentWindow.StaticComponents.mSelectedTabButton.PackageName))
        tabName = "STRING_GAME_SETTINGS";
      ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "Settings", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
      this.ParentWindow.mCommonHandler.LaunchSettingsWindow(tabName);
    }

    private void MIManagerHandler(object sender, EventArgs e)
    {
      try
      {
        Process.Start(Path.Combine(RegistryStrings.InstallDir, "HD-MultiInstanceManager.exe"), "-IsMIMLaunchedFromClient");
        ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "MultiInstance", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
      }
      catch (Exception ex)
      {
        Logger.Error("Couldn't launch MI Manager. Ex: {0}", (object) ex.Message);
      }
    }

    private void RotateHandler(object sender, EventArgs e)
    {
      this.RotateButtonHandler("MouseClick");
    }

    internal void RotateButtonHandler(string action)
    {
      this.mIsUIInPortraitModeBeforeChange = this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.IsPortraitModeTab;
      this.ParentWindow.AppForcedOrientationDict[this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName] = !this.mIsUIInPortraitModeBeforeChange;
      this.ParentWindow.ChangeOrientationFromClient(!this.mIsUIInPortraitModeBeforeChange, true);
      string str = this.mIsUIInPortraitModeBeforeChange ? "landscape" : "portrait";
      ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "Rotate", action, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, str, this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab?.PackageName, "Android");
    }

    private void ShakeHandler(object sender, EventArgs e)
    {
      this.ParentWindow.mCommonHandler.ShakeButtonHandler();
      ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "Shake", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab?.PackageName, (string) null, "Android");
    }

    private void LocationHandler(object sender, EventArgs e)
    {
      this.ParentWindow.mCommonHandler.LocationButtonHandler();
      ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "SetLocation", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
    }

    private void LockCursorHandler(object sender, EventArgs e)
    {
      this.ParentWindow.mCommonHandler.ClipMouseCursorHandler(false, true, "MouseClick", "sidebar");
    }

    private void ScreenshotHandler(object sender, EventArgs e)
    {
      ToolTipService.SetToolTip((DependencyObject) (sender as SidebarElement), (object) null);
      this.mScreenshotPopup.IsOpen = false;
      ThreadPool.QueueUserWorkItem((WaitCallback) (obj =>
      {
        Thread.Sleep(100);
        this.ParentWindow.mCommonHandler.ScreenShotButtonHandler();
        ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "Screenshot", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
      }));
    }

    private void VolumeButtonHandler(object sender, EventArgs e)
    {
      if (this.mVolumeSliderPopup.IsOpen)
        this.mVolumeSliderPopup.IsOpen = false;
      else
        this.mVolumeSliderPopup.IsOpen = true;
    }

    private void OpenFarmModePopupButtonHandler(object sender, EventArgs e)
    {
      this.mFarmModePopup.IsTopmost = true;
      this.mFarmSettingStepperControl.LostFocus -= new RoutedEventHandler(this.FarmSenstivityLostFocus);
      if (string.IsNullOrEmpty(this.mFarmSettingStepperControl.Text))
        this.mFarmSettingStepperControl.Text = this.FarmSensitivity;
      this.mFarmSettingStepperControl.LostFocus += new RoutedEventHandler(this.FarmSenstivityLostFocus);
      if (this.mFarmModePopup.IsOpen)
        this.mFarmModePopup.IsOpen = false;
      else
        this.mFarmModePopup.IsOpen = true;
      ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "EcoMode", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
    }

    private void FullScreenHandler(object sender, EventArgs e)
    {
      if (this.ParentWindow.mStreamingModeEnabled)
        return;
      this.ParentWindow.mCommonHandler.FullScreenButtonHandler("sidebar", "MouseClick");
    }

    internal SidebarElement GetElementFromTag(string tag)
    {
      return this.mListSidebarElements.Count >= 1 ? this.mListSidebarElements.Where<SidebarElement>((Func<SidebarElement, bool>) (item => (string) item.Tag == tag)).FirstOrDefault<SidebarElement>() : (SidebarElement) null;
    }

    public void AddElement(SidebarElement ele, bool isStaticElement = false)
    {
      if (isStaticElement)
        this.mStaticButtonsStackPanel.Children.Add((UIElement) ele);
      else
        this.mElementsStackPanel.Children.Add((UIElement) ele);
    }

    public void UpdateToDefaultImage(string tag)
    {
      Sidebar.UpdateToDefaultImage(this.GetElementFromTag(tag));
    }

    public void UpdateImage(string tag, string newImage)
    {
      Sidebar.UpdateImage(this.GetElementFromTag(tag), newImage);
    }

    public static void UpdateToDefaultImage(SidebarElement ele)
    {
      if (ele == null)
        return;
      ele.Image.ImageName = (string) ele.Tag;
    }

    public static void UpdateImage(SidebarElement ele, string newImage)
    {
      if (ele == null)
        return;
      ele.Image.ImageName = newImage;
    }

    private static void DecreaseElementBottomMargin(SidebarElement ele)
    {
      Thickness margin = ele.Margin;
      margin.Bottom = 2.0;
      ele.Margin = margin;
    }

    private static void IncreaseElementBottomMarginIfLast(SidebarElement ele)
    {
      if (!ele.IsCurrentLastElementOfGroup)
        return;
      Thickness margin = ele.Margin;
      margin.Bottom = 10.0;
      ele.Margin = margin;
    }

    private SidebarElement CreateElement(
      string imageName,
      string toolTipKey,
      EventHandler evt)
    {
      SidebarElement sidebarElement1 = new SidebarElement();
      sidebarElement1.Margin = new Thickness(0.0, 2.0, 0.0, 2.0);
      sidebarElement1.Visibility = Visibility.Visible;
      sidebarElement1.mSidebarElementTooltipKey = toolTipKey;
      SidebarElement sidebarElement2 = sidebarElement1;
      sidebarElement2.Image.ImageName = imageName;
      sidebarElement2.Tag = (object) imageName;
      sidebarElement2.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.SidebarElement_PreviewMouseLeftButtonUp);
      sidebarElement2.IsVisibleChanged += new DependencyPropertyChangedEventHandler(this.SidebarElement_IsVisibleChanged);
      this.SetSidebarElementTooltip(sidebarElement2, toolTipKey);
      this.mDictActions.Add(sidebarElement2, evt);
      this.mListSidebarElements.Add(sidebarElement2);
      return sidebarElement2;
    }

    internal void SetSidebarElementTooltip(SidebarElement ele, string toolTipKey)
    {
      this.ParentWindow.Dispatcher.Invoke((Delegate) (() =>
      {
        string str;
        if (ele.Tag.ToString() == "sidebar_volume")
        {
          str = Sidebar.GetTooltip(LocaleStrings.GetLocalizedString("STRING_INCREASE_VOLUME", ""), this.ParentWindow.mCommonHandler.GetShortcutKeyFromName("STRING_INCREASE_VOLUME", false), " ") + "\n" + Sidebar.GetTooltip(LocaleStrings.GetLocalizedString("STRING_DECREASE_VOLUME", ""), this.ParentWindow.mCommonHandler.GetShortcutKeyFromName("STRING_DECREASE_VOLUME", false), " ");
          this.mVolumeMuteUnmuteImage.ToolTip = (object) Sidebar.GetTooltip(LocaleStrings.GetLocalizedString("STRING_TOGGLE_MUTE_STATE", ""), this.ParentWindow.mCommonHandler.GetShortcutKeyFromName("STRING_TOGGLE_MUTE_STATE", false), "\n");
        }
        else
          str = !(ele.Tag.ToString() == "sidebar_lock_cursor") ? (!(ele.Tag.ToString() == "sidebar_farmmode_inactive") ? Sidebar.GetTooltip(LocaleStrings.GetLocalizedString(toolTipKey, ""), this.ParentWindow.mCommonHandler.GetShortcutKeyFromName(toolTipKey, false), "\n") : Sidebar.GetTooltip(LocaleStrings.GetLocalizedString(toolTipKey, ""), "", "")) : Sidebar.GetTooltip(LocaleStrings.GetLocalizedString("STRING_TOGGLE_LOCK_CURSOR", ""), this.ParentWindow.mCommonHandler.GetShortcutKeyFromName("STRING_TOGGLE_LOCK_CURSOR", false), " ") + "\n" + Sidebar.GetTooltip(LocaleStrings.GetLocalizedString("STRING_UNLOCK_MOUSE", ""), this.ParentWindow.mCommonHandler.GetShortcutKeyFromName("STRING_UNLOCK_MOUSE", false), " ");
        SidebarElement sidebarElement = ele;
        sidebarElement.ToolTip = (object) new ToolTip()
        {
          Content = (object) str
        };
      }));
    }

    private static string GetTooltip(string text, string shortcut, string delimiter = "\n")
    {
      return !string.IsNullOrEmpty(shortcut) ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, text + delimiter + "(" + shortcut + ")") : (!string.IsNullOrEmpty(text) ? text : (string) null);
    }

    private void SidebarElement_IsVisibleChanged(
      object sender,
      DependencyPropertyChangedEventArgs e)
    {
      if (!this.mIsOneSidebarElementLoadedBinded && (bool) e.NewValue)
      {
        this.mIsOneSidebarElementLoadedBinded = true;
        if (sender is SidebarElement sidebarElement && this.mSidebarElementApproxHeight == 0.0)
        {
          int num1 = (int) sidebarElement.Height + 2 * (int) sidebarElement.Margin.Top;
          int num2 = this.mListSidebarElements.Where<SidebarElement>((Func<SidebarElement, bool>) (item => item.IsLastElementOfGroup)).Count<SidebarElement>();
          int count = this.mElementsStackPanel.Children.Count;
          this.mSidebarElementApproxHeight = (double) ((num1 * count + (num2 - 1) * 8) / this.mElementsStackPanel.Children.Count + 2);
          Logger.Info("Aprrox: {0}", (object) this.mSidebarElementApproxHeight);
        }
      }
      this.UpdateTotalVisibleElementCount();
    }

    private void SidebarElement_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      SidebarElement index = sender as SidebarElement;
      if (this.mDictActions.ContainsKey(index))
      {
        EventHandler mDictAction = this.mDictActions[index];
        if (mDictAction != null)
          mDictAction((object) index, new EventArgs());
      }
      if (!index.RedDotVisibility)
        return;
      if (index.Tag.ToString() == "sidebar_utc_converter")
        RegistryManager.Instance.IsUtcConverterRedDotOnboardingCompleted = true;
      this.ShowRedDotOnSidebarElement(index, false);
    }

    private void MSidebar_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      this.ArrangeAllSidebarElements();
    }

    internal void ArrangeAllSidebarElements()
    {
      try
      {
        if (this.Visibility != Visibility.Visible)
          return;
        this.UpdateTotalVisibleElementCount();
        int num1 = Math.Min((int) (Math.Max(0.0, this.ActualHeight - this.mBottomGrid.ActualHeight - 54.0) / this.mSidebarElementApproxHeight), this.mTotalVisibleElements);
        List<SidebarElement> list = this.mElementsStackPanel.Children.OfType<SidebarElement>().Where<SidebarElement>((Func<SidebarElement, bool>) (item => item.Visibility == Visibility.Visible)).ToList<SidebarElement>();
        int count = list.Count;
        if (count > num1)
        {
          int num2 = count - num1;
          for (int index = 1; index <= num2; ++index)
          {
            SidebarElement sidebarElement = list[count - index];
            this.mElementsStackPanel.Children.Remove((UIElement) sidebarElement);
            sidebarElement.IsInMainSidebar = false;
          }
        }
        else if (count < num1)
        {
          int num2 = num1 - count;
          SidebarElement[] array = this.mListSidebarElements.Where<SidebarElement>((Func<SidebarElement, bool>) (item => !item.IsInMainSidebar)).ToArray<SidebarElement>();
          for (int index = 0; index < num2; ++index)
          {
            if (array.Length > index)
            {
              SidebarElement ele = array[index];
              ele.IsInMainSidebar = true;
              this.AddToVisibleElementsPanel(ele);
            }
          }
        }
        if (this.mListSidebarElements.Any<SidebarElement>((Func<SidebarElement, bool>) (x => !x.IsInMainSidebar)))
          this.mMoreButton.Visibility = Visibility.Visible;
        else
          this.mMoreButton.Visibility = Visibility.Collapsed;
        this.mMoreButton.RedDotVisibility = this.mListSidebarElements.Any<SidebarElement>((Func<SidebarElement, bool>) (x => !x.IsInMainSidebar && x.RedDotVisibility));
      }
      catch (Exception ex)
      {
        Logger.Warning("XXX SR: An error occured while rearranging elements. Ex: {0}", (object) ex);
      }
    }

    private void AddToVisibleElementsPanel(SidebarElement ele)
    {
      if (ele.Parent is StackPanel parent)
        parent.Children.Remove((UIElement) ele);
      this.mElementsStackPanel.Children.Add((UIElement) ele);
      Sidebar.IncreaseElementBottomMarginIfLast(ele);
    }

    public void SetHeight()
    {
      this.Height = this.ParentWindow.mContentGrid.ActualHeight;
    }

    private void MMoreElements_Opened(object sender, EventArgs e)
    {
      this.SidebarPopupContentClear();
      this.mSidebarPopup.InitAllElements(this.mListSidebarElements.Where<SidebarElement>((Func<SidebarElement, bool>) (x => !x.IsInMainSidebar)));
      Sidebar.UpdateImage(this.mMoreButton, "sidebar_options_open");
      BlueStacksUIBinding.Bind((UserControl) this.mMoreButton, "STRING_CLOSE");
    }

    private void MMoreElements_Closed(object sender, EventArgs e)
    {
      this.SidebarPopupContentClear();
      this.ParentWindow.AllowFrontendFocusOnClientClick = true;
      Sidebar.UpdateImage(this.mMoreButton, "sidebar_options_close");
      BlueStacksUIBinding.Bind((UserControl) this.mMoreButton, "STRING_MORE_BUTTON");
    }

    private void SidebarPopupContentClear()
    {
      foreach (Panel child in this.mSidebarPopup.mMainStackPanel.Children)
        child.Children.Clear();
      this.mSidebarPopup.mMainStackPanel.Children.Clear();
    }

    private void MSidebarPopup_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      e.Handled = true;
    }

    private void MMoreButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.mMoreElements.IsOpen = !this.mMoreElements.IsOpen;
    }

    private void MMoreButton_MouseLeave(object sender, MouseEventArgs e)
    {
      if (!this.mEnableMoreElementPopupTimer || this.mMoreButton.IsMouseOver || (!this.mListPopups.All<CustomPopUp>((Func<CustomPopUp, bool>) (x => !x.IsMouseOver)) || !this.mMoreElements.IsOpen))
        return;
      if (this.mSidebarPopupTimer == null)
      {
        this.mSidebarPopupTimer = new DispatcherTimer()
        {
          Interval = new TimeSpan(0, 0, 0, 0, 500)
        };
        this.mSidebarPopupTimer.Tick += new EventHandler(this.SidebarPopupTimer_Tick);
      }
      else
        this.mSidebarPopupTimer.Stop();
      this.mSidebarPopupTimer.Start();
    }

    private void SidebarPopupTimer_Tick(object sender, EventArgs e)
    {
      if (!this.mMoreButton.IsMouseOver && this.mListPopups.All<CustomPopUp>((Func<CustomPopUp, bool>) (x => !x.IsMouseOver)))
      {
        this.mListPopups.Select<CustomPopUp, CustomPopUp>((Func<CustomPopUp, CustomPopUp>) (c =>
        {
          c.IsOpen = false;
          return c;
        })).ToList<CustomPopUp>();
        this.ParentWindow.AllowFrontendFocusOnClientClick = true;
        if (this.mIsInFullscreenMode && !this.IsMouseOver)
          this.ToggleSidebarVisibilityInFullscreen(false);
      }
      (sender as DispatcherTimer).Stop();
    }

    private void ClosePopup_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      e.Handled = true;
    }

    private void mTranslucentControlsSliderButton_PreviewMouseLeftButtonUp(
      object sender,
      MouseButtonEventArgs e)
    {
      bool flag = !this.mOverlayToggleButton.BoolValue;
      RegistryManager.Instance.ShowKeyControlsOverlay = flag;
      this.OverlayStateChange(flag);
      KMManager.ShowOverlayWindow(this.ParentWindow, flag, false);
      ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "Overlay", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
    }

    private void TransparencySlider_ValueChanged(
      object sender,
      RoutedPropertyChangedEventArgs<double> e)
    {
      KMManager.ChangeTransparency(this.ParentWindow, this.transSlider.Value);
      if (this.transSlider.Value == 0.0)
      {
        if (!RegistryManager.Instance.ShowKeyControlsOverlay)
          KMManager.ShowOverlayWindow(this.ParentWindow, false, false);
        this.OverlayStateChange(false);
      }
      else
      {
        KMManager.ShowOverlayWindow(this.ParentWindow, true, false);
        this.OverlayStateChange(true);
      }
      this.mLastSliderValue = this.transSlider.Value;
    }

    private void OverlayTooltipCPB_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.mOverlayTooltip.IsOpen = false;
      this.mIsOverlayTooltipClosed = true;
      e.Handled = true;
    }

    private void OverlayDoNotShowCheckbox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      if (string.Equals(this.mOverlayDoNotShowCheckboxImage.ImageName, "bgpcheckbox", StringComparison.InvariantCulture))
      {
        this.mOverlayDoNotShowCheckboxImage.ImageName = "bgpcheckbox_checked";
        RegistryManager.Instance.OverlayAvailablePromptEnabled = false;
      }
      else
      {
        this.mOverlayDoNotShowCheckboxImage.ImageName = "bgpcheckbox";
        RegistryManager.Instance.OverlayAvailablePromptEnabled = true;
      }
      e.Handled = true;
    }

    private void ScreenRecorderButtonHandler(object sender, EventArgs e)
    {
      this.ParentWindow.mCommonHandler.DownloadAndLaunchRecording("sidebar", "MouseClick");
    }

    private void RecordScreenPopupClose_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.mRecordScreenPopup.IsOpen = false;
    }

    private void RecordScreenPopup_Closed(object sender, EventArgs e)
    {
      if (CommonHandlers.sIsRecordingVideo)
      {
        BlueStacksUIBinding.Bind(this.RecordScreenPopupHeader, "STRING_STOP_RECORDING", "");
        this.RecordScreenPopupHeader.Visibility = Visibility.Visible;
        this.RecordScreenPopupBody.Visibility = Visibility.Collapsed;
        this.mRecordScreenClose.Visibility = Visibility.Collapsed;
      }
      else
      {
        BlueStacksUIBinding.Bind(this.RecordScreenPopupHeader, "STRING_RECORD_SCREEN", "");
        BlueStacksUIBinding.Bind(this.RecordScreenPopupBody, "STRING_RECORD_SCREEN_PLAYING", "");
        this.RecordScreenPopupHeader.Visibility = Visibility.Visible;
        this.RecordScreenPopupBody.Visibility = Visibility.Visible;
        this.mRecordScreenClose.Visibility = Visibility.Collapsed;
      }
      this.mRecordScreenPopup.StaysOpen = true;
      this.RecordScreenPopupHyperlink.Visibility = Visibility.Collapsed;
    }

    private void KeymappingControlsTransparencyButtonHandler(object sender, EventArgs e)
    {
      RegistryManager.Instance.ShowKeyControlsOverlay = true;
      RegistryManager.Instance.OverlayAvailablePromptEnabled = false;
      KMManager.ShowOverlayWindow(this.ParentWindow, true, false);
      this.mChangeTransparencyPopup.IsOpen = true;
      ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "Overlay", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
    }

    private void RecordScreenPopupHyperlink_Click(object sender, RoutedEventArgs e)
    {
      CommonHandlers.OpenMediaFolderWithFileSelected(CommonHandlers.mSavedVideoRecordingFilePath);
      this.mRecordScreenPopup.IsOpen = false;
    }

    private void RecordScreenClose_IsVisibleChanged(object _, DependencyPropertyChangedEventArgs e)
    {
      if ((bool) e.NewValue)
        this.RecordScreenPopupHeader.Margin = new Thickness(0.0, 0.0, 20.0, 0.0);
      else
        this.RecordScreenPopupHeader.Margin = new Thickness(0.0);
    }

    internal void SidebarVisiblityChanged(Visibility currentVisibility)
    {
      if (this.mIsInFullscreenMode || !this.IsLoaded)
        return;
      if (currentVisibility == Visibility.Visible)
      {
        this.ParentWindow.ParentWindowWidthDiff = 62;
        this.ParentWindow.Width = this.ParentWindow.ActualWidth + this.Width;
        this.ArrangeAllSidebarElements();
      }
      else
      {
        this.ParentWindow.ParentWindowWidthDiff = 2;
        this.ParentWindow.Width = Math.Max(this.ParentWindow.ActualWidth - this.Width, this.ParentWindow.MinWidth);
      }
      this.ParentWindow.HandleDisplaySettingsChanged();
      this.ParentWindow.Height = this.ParentWindow.GetHeightFromWidth(this.ParentWindow.Width, false, false);
    }

    internal void ShowOverlayTooltip(bool isShow, bool force = false)
    {
      if (this.GetElementFromTag("sidebar_overlay") == null || !RegistryManager.Instance.OverlayAvailablePromptEnabled)
        return;
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        if (isShow)
        {
          this.mIsPendingShowOverlayTooltip = true;
          this.ActualOverlayTooltip(force);
        }
        else
          this.mOverlayTooltip.IsOpen = false;
      }));
    }

    private void ActualOverlayTooltip(bool force = false)
    {
      if (!RegistryManager.Instance.OverlayAvailablePromptEnabled || this.mIsOverlayTooltipClosed || !this.mIsPendingShowOverlayTooltip || (((!RegistryManager.Instance.IsAutoShowGuidance ? 1 : (Array.Exists<string>(RegistryManager.Instance.DisabledGuidancePackages, (Predicate<string>) (element => element == this.ParentWindow.StaticComponents.mSelectedTabButton.PackageName)) ? 1 : 0)) | (force ? 1 : 0)) == 0 || this.mIsInFullscreenMode || (FeatureManager.Instance.IsCustomUIForNCSoft || this.Visibility != Visibility.Visible)))
        return;
      this.mIsPendingShowOverlayTooltip = false;
      this.mOverlayTooltip.IsOpen = true;
    }

    private void ActualKeymapPopup()
    {
      if (RegistryManager.Instance.OverlayAvailablePromptEnabled && !this.mIsOverlayTooltipClosed)
        return;
      string packageName = this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName;
      if (!AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName].ContainsKey(packageName))
        AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName][packageName] = new AppSettings();
      if (AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName][packageName].IsKeymappingTooltipShown)
        return;
      AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName][packageName].IsKeymappingTooltipShown = true;
    }

    internal void ShowKeyMapPopup(bool isShow)
    {
      if (this.GetElementFromTag("sidebar_controls") == null || !isShow)
        return;
      if (!Array.Exists<string>(RegistryManager.Instance.DisabledGuidancePackages, (Predicate<string>) (element => element == this.ParentWindow.StaticComponents.mSelectedTabButton.PackageName)) && RegistryManager.Instance.IsAutoShowGuidance)
        KMManager.HandleInputMapperWindow(this.ParentWindow, "");
      else
        this.ActualKeymapPopup();
    }

    private void OpenMacroGridPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.ParentWindow.mCommonHandler.ShowMacroRecorderWindow();
      this.mMacroButtonPopup.IsOpen = false;
      this.ToggleSidebarVisibilityInFullscreen(false);
      ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "MacroRecorder", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, "MacroBookmarkPopup", (string) null, "Android");
    }

    private void OpenMacroGridMouseEnter(object sender, MouseEventArgs e)
    {
      BlueStacksUIBinding.BindColor((DependencyObject) (sender as Grid), Panel.BackgroundProperty, "ContextMenuItemBackgroundHoverColor");
    }

    private void OpenMacroGridMouseLeave(object sender, MouseEventArgs e)
    {
      BlueStacksUIBinding.BindColor((DependencyObject) (sender as Grid), Panel.BackgroundProperty, "SidebarBackground");
    }

    private void VolumeImage_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.ParentWindow.mCommonHandler.MuteUnmuteButtonHandler();
      if (this.mIsEcoModeEnabled)
        return;
      if (this.ParentWindow.IsMuted)
        ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "VolumeOn", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
      else
        ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "VolumeOff", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
    }

    private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
      this.mCurrentVolumeValue.Text = Math.Round(e.NewValue).ToString((IFormatProvider) CultureInfo.InvariantCulture);
    }

    private void VolumeSlider_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.ParentWindow.Utils.SetVolumeInFrontendAsync(Convert.ToInt32(this.mVolumeSlider.Value));
    }

    private void MuteInstancesCheckboxImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      if (this.mMuteInstancesCheckboxImage.ImageName.Equals("bgpcheckbox", StringComparison.OrdinalIgnoreCase))
      {
        this.mMuteInstancesCheckboxImage.ImageName = "bgpcheckbox_checked";
        BlueStacksUIUtils.SendMuteUnmuteRequestToAllInstances(true);
        ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "VolumeOffAll", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
      }
      else
      {
        this.mMuteInstancesCheckboxImage.ImageName = "bgpcheckbox";
        BlueStacksUIUtils.SendMuteUnmuteRequestToAllInstances(false);
        ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "VolumeOnAll", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
      }
      e.Handled = true;
    }

    private void ChangeTransparencyPopup_MouseLeave(object sender, MouseEventArgs e)
    {
      if (!this.mChangeTransparencyPopup.IsOpen)
        return;
      if (this.mChangeTransparencyPopupTimer == null)
      {
        this.mChangeTransparencyPopupTimer = new DispatcherTimer()
        {
          Interval = new TimeSpan(0, 0, 0, 0, 500)
        };
        this.mChangeTransparencyPopupTimer.Tick += new EventHandler(this.ChangeTransparencyPopupTimer_Tick);
      }
      else
        this.mChangeTransparencyPopupTimer.Stop();
      this.mChangeTransparencyPopupTimer.Start();
    }

    private void ChangeTransparencyPopupTimer_Tick(object sender, EventArgs e)
    {
      if (!this.mChangeTransparencyPopup.IsMouseOver && !this.GetElementFromTag("sidebar_overlay").IsMouseOver)
      {
        this.mChangeTransparencyPopup.IsOpen = false;
        if (this.mIsInFullscreenMode && !this.IsMouseOver)
          this.ToggleSidebarVisibilityInFullscreen(false);
      }
      (sender as DispatcherTimer).Stop();
    }

    private void VolumeSliderPopup_MouseLeave(object sender, MouseEventArgs e)
    {
      if (!this.mVolumeSliderPopup.IsOpen)
        return;
      if (this.mVolumeSliderPopupTimer == null)
      {
        this.mVolumeSliderPopupTimer = new DispatcherTimer()
        {
          Interval = new TimeSpan(0, 0, 1)
        };
        this.mVolumeSliderPopupTimer.Tick += new EventHandler(this.VolumeSliderPopupTimer_Tick);
      }
      else
        this.mVolumeSliderPopupTimer.Stop();
      this.mVolumeSliderPopupTimer.Start();
    }

    internal void VolumeSliderPopupTimer_Tick(object sender, EventArgs e)
    {
      if (!this.mVolumeSliderPopup.IsMouseOver && !this.GetElementFromTag("sidebar_volume").IsMouseOver)
      {
        this.mVolumeSliderPopup.IsOpen = false;
        if (this.mIsInFullscreenMode && !this.IsMouseOver)
          this.ToggleSidebarVisibilityInFullscreen(false);
      }
      (sender as DispatcherTimer).Stop();
    }

    private void ScreenshotPopupClose_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.mScreenshotPopup.IsOpen = false;
    }

    private void GameControlButtonPopup_MouseLeave(object sender, MouseEventArgs e)
    {
      if (!this.mGameControlButtonPopup.IsOpen)
        return;
      if (this.mGameControlBookmarkTimer == null)
      {
        this.mGameControlBookmarkTimer = new DispatcherTimer()
        {
          Interval = new TimeSpan(0, 0, 0, 0, 500)
        };
        this.mGameControlBookmarkTimer.Tick += new EventHandler(this.GameControlBookmarkTimer_Tick);
      }
      else
        this.mGameControlBookmarkTimer.Stop();
      this.mGameControlBookmarkTimer.Start();
    }

    private void GameControlBookmarkTimer_Tick(object sender, EventArgs e)
    {
      if (!this.mGameControlButtonPopup.IsMouseOver && !this.GetElementFromTag("sidebar_controls").IsMouseOver)
      {
        this.mGameControlButtonPopup.IsOpen = false;
        if (this.ParentWindow.mIsFullScreen)
          this.ToggleSidebarVisibilityInFullscreen(false);
      }
      (sender as DispatcherTimer).Stop();
    }

    private void GameControlButtonHandler(object sender, EventArgs e)
    {
      if (this.mGameControlButtonPopup.IsOpen)
      {
        this.mGameControlButtonPopup.IsOpen = false;
      }
      else
      {
        IEnumerable<IMControlScheme> source = this.ParentWindow.SelectedConfig.ControlSchemes.Where<IMControlScheme>((Func<IMControlScheme, bool>) (scheme => scheme.IsBookMarked));
        this.mBookmarkStackPanel.Visibility = source.Any<IMControlScheme>() ? Visibility.Visible : Visibility.Collapsed;
        this.mBookmarkedSchemesStackPanel.Children.Clear();
        double maxWidth = this.mControlPanel.ActualWidth > 0.0 ? this.mControlPanel.ActualWidth : 180.0;
        foreach (IMControlScheme scheme in source)
          this.mBookmarkedSchemesStackPanel.Children.Add((UIElement) new SchemeBookmarkControl(scheme, this.ParentWindow, maxWidth));
        SidebarElement elementFromTag1 = this.GetElementFromTag("sidebar_gameguide");
        // ISSUE: explicit non-virtual call
        if ((elementFromTag1 != null ? (__nonvirtual (elementFromTag1.IsEnabled) ? 1 : 0) : 0) == 0)
        {
          SidebarElement elementFromTag2 = this.GetElementFromTag("sidebar_gameguide_active");
          // ISSUE: explicit non-virtual call
          if ((elementFromTag2 != null ? (__nonvirtual (elementFromTag2.IsEnabled) ? 1 : 0) : 0) == 0)
          {
            this.mViewControlsTxtBlock.IsEnabled = false;
            this.mViewGameControlsPanel.IsEnabled = false;
            goto label_13;
          }
        }
        this.mViewControlsTxtBlock.IsEnabled = true;
        this.mViewGameControlsPanel.IsEnabled = true;
label_13:
        this.mOverlayToggleButton.BoolValue = RegistryManager.Instance.ShowKeyControlsOverlay;
        this.transSlider.Value = RegistryManager.Instance.TranslucentControlsTransparency;
        this.mGamepadIconImage.IsEnabled = this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.mDictGamepadEligibility[this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName];
        if (this.ParentWindow.mCommonHandler.CheckIfGamepadOverlaySelectedForApp(this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName))
        {
          this.mGamepadIconImage.ImageName = "gamepad_overlay_icon_click";
          this.mKeyboardIconImage.ImageName = "keyboard_overlay_icon";
        }
        else
        {
          this.mKeyboardIconImage.ImageName = "keyboard_overlay_icon_click";
          this.mGamepadIconImage.ImageName = "gamepad_overlay_icon";
        }
        if (!this.mGamepadIconImage.IsEnabled)
          this.mGamepadIconImage.ImageName = "gamepad_overlay_icon_disabled";
        this.SetShortcutTooltips();
        this.mGameControlButtonPopup.IsOpen = true;
        ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "AdvanceGameControls", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
      }
    }

    private void SetShortcutTooltips()
    {
      this.mOverlayToggleButton.ToolTip = (object) Sidebar.GetTooltip(LocaleStrings.GetLocalizedString("STRING_TOGGLE_OVERLAY", ""), this.ParentWindow.mCommonHandler.GetShortcutKeyFromName("STRING_TOGGLE_OVERLAY", false), "\n");
      this.mViewControlsTxtBlock.ToolTip = (object) this.ParentWindow.mCommonHandler.GetShortcutKeyFromName("STRING_TOGGLE_KEYMAP_WINDOW", false);
      this.mOpenEditorTxtBlock.ToolTip = (object) this.ParentWindow.mCommonHandler.GetShortcutKeyFromName("STRING_CONTROLS_EDITOR", false);
      this.mControlsForTxtBlock.ToolTip = (object) Sidebar.GetTooltip(LocaleStrings.GetLocalizedString("STRING_SWITCH_OVERLAY_CONTROLS", ""), this.ParentWindow.mCommonHandler.GetShortcutKeyFromName("STRING_SWITCH_OVERLAY_CONTROLS", false), "\n");
      this.mGameControlsToggle.ToolTip = (object) Sidebar.GetTooltip(LocaleStrings.GetLocalizedString("STRING_TOGGLE_KEYMAPPING_STATE", ""), this.ParentWindow.mCommonHandler.GetShortcutKeyFromName("STRING_TOGGLE_KEYMAPPING_STATE", false), "\n");
    }

    private void OpenGameControlPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.mGameControlButtonPopup.IsOpen = false;
      if (KMManager.sGuidanceWindow != null && !KMManager.sGuidanceWindow.IsClosed)
        KMManager.sGuidanceWindow.Close();
      this.ParentWindow.mCommonHandler.KeyMapButtonHandler("MouseClick", "sidebar");
      this.ToggleSidebarVisibilityInFullscreen(false);
    }

    internal void ChangeVideoRecordingImage(string imageName)
    {
      SidebarElement elementFromTag = this.GetElementFromTag("sidebar_video_capture");
      if (elementFromTag == null)
        return;
      elementFromTag.Image.IsImageToBeRotated = false;
      Sidebar.UpdateImage(elementFromTag, imageName);
    }

    private void SetVideoRecordingTooltipForNCSoft()
    {
      if (!FeatureManager.Instance.IsCustomUIForNCSoft)
        return;
      SidebarElement elementFromTag = this.GetElementFromTag("sidebar_video_capture");
      if (elementFromTag == null)
        return;
      ToolTip toolTip = elementFromTag.ToolTip as ToolTip;
      toolTip.Content = (object) Convert.ToString(toolTip.Content, (IFormatProvider) CultureInfo.InvariantCulture).Replace(LocaleStrings.GetLocalizedString("STRING_RECORD_SCREEN", ""), LocaleStrings.GetLocalizedString("STRING_RECORD_SCREEN_WITHOUT_BETA", ""));
    }

    internal void SetVideoRecordingTooltip(bool isRecording)
    {
      SidebarElement elementFromTag = this.GetElementFromTag("sidebar_video_capture");
      if (elementFromTag == null)
        return;
      ToolTip toolTip = elementFromTag.ToolTip as ToolTip;
      if (isRecording)
      {
        if (FeatureManager.Instance.IsCustomUIForNCSoft)
          toolTip.Content = (object) Convert.ToString(toolTip.Content, (IFormatProvider) CultureInfo.InvariantCulture).Replace(LocaleStrings.GetLocalizedString("STRING_RECORD_SCREEN_WITHOUT_BETA", ""), LocaleStrings.GetLocalizedString("STRING_STOP_RECORDING", ""));
        else
          toolTip.Content = (object) Convert.ToString(toolTip.Content, (IFormatProvider) CultureInfo.InvariantCulture).Replace(LocaleStrings.GetLocalizedString("STRING_RECORD_SCREEN", ""), LocaleStrings.GetLocalizedString("STRING_STOP_RECORDING", ""));
      }
      else if (FeatureManager.Instance.IsCustomUIForNCSoft)
        toolTip.Content = (object) Convert.ToString(toolTip.Content, (IFormatProvider) CultureInfo.InvariantCulture).Replace(LocaleStrings.GetLocalizedString("STRING_STOP_RECORDING", ""), LocaleStrings.GetLocalizedString("STRING_RECORD_SCREEN_WITHOUT_BETA", ""));
      else
        toolTip.Content = (object) Convert.ToString(toolTip.Content, (IFormatProvider) CultureInfo.InvariantCulture).Replace(LocaleStrings.GetLocalizedString("STRING_STOP_RECORDING", ""), LocaleStrings.GetLocalizedString("STRING_RECORD_SCREEN", ""));
    }

    private void VolumeSliderPopup_Closed(object sender, EventArgs e)
    {
      if (this.mIsEcoModeEnabled && this.mCurrentVolumeLevel != RegistryManager.Instance.Guest[this.ParentWindow.mVmName].Volume)
        ClientStats.SendMiscellaneousStatsAsync("volume_eco_mode_changed", RegistryManager.Instance.UserGuid, this.mCurrentVolumeValue.Text, (string) null, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, this.ParentWindow.mVmName, (string) null, "Android");
      if (this.GetElementFromTag("sidebar_volume").IsInMainSidebar)
        return;
      this.MMoreButton_MouseLeave((object) null, (MouseEventArgs) null);
    }

    private void VolumeSliderPopup_Opened(object sender, EventArgs e)
    {
      this.mCurrentVolumeLevel = RegistryManager.Instance.Guest[this.ParentWindow.mVmName].Volume;
    }

    private void GameControlButtonPopup_Closed(object sender, EventArgs e)
    {
      if (this.GetElementFromTag("sidebar_controls").IsInMainSidebar)
        return;
      this.MMoreButton_MouseLeave((object) null, (MouseEventArgs) null);
    }

    private void ChangeTransparencyPopup_Closed(object sender, EventArgs e)
    {
      if (this.GetElementFromTag("sidebar_overlay").IsInMainSidebar)
        return;
      this.MMoreButton_MouseLeave((object) null, (MouseEventArgs) null);
    }

    private void OpenGameGuideButtonHandler(object sender, EventArgs e)
    {
      this.ParentWindow.mCommonHandler.GameGuideButtonHandler("MouseClick", "sidebar", false);
    }

    public void ShowRedDotOnSidebarElement(SidebarElement sidebarElement, bool visibility)
    {
      if (sidebarElement != null)
        sidebarElement.RedDotVisibility = visibility;
      this.mMoreButton.RedDotVisibility = this.mListSidebarElements.Any<SidebarElement>((Func<SidebarElement, bool>) (x => !x.IsInMainSidebar && x.RedDotVisibility));
    }

    public OnBoardingPopupWindow FullscreenOnboardingBlurb()
    {
      if (this.ParentWindow.mSidebar.Visibility == Visibility.Collapsed || string.IsNullOrEmpty(this.ParentWindow.mCommonHandler.GetShortcutKeyFromName("STRING_UPDATED_FULLSCREEN_BUTTON_TOOLTIP", false)))
        return (OnBoardingPopupWindow) null;
      SidebarElement sidebarElement = this.ParentWindow.mSidebar.mElementsStackPanel.Children.Cast<SidebarElement>().Where<SidebarElement>((Func<SidebarElement, bool>) (s => s.Image.ImageName == "sidebar_fullscreen")).FirstOrDefault<SidebarElement>();
      if (sidebarElement == null)
        return (OnBoardingPopupWindow) null;
      OnBoardingPopupWindow boardingPopupWindow1 = new OnBoardingPopupWindow(this.ParentWindow, this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName);
      boardingPopupWindow1.Owner = (Window) this.ParentWindow;
      boardingPopupWindow1.PlacementTarget = (UIElement) sidebarElement;
      boardingPopupWindow1.Title = "FullScreenBlurb";
      boardingPopupWindow1.LeftMargin = 310;
      boardingPopupWindow1.TopMargin = 45;
      boardingPopupWindow1.IsBlurbRelatedToGuidance = false;
      boardingPopupWindow1.HeaderContent = LocaleStrings.GetLocalizedString("STRING_PLAY_BIGGER_HEADER", "");
      OnBoardingPopupWindow boardingPopupWindow2 = boardingPopupWindow1;
      boardingPopupWindow2.bodyTextBlock.Visibility = Visibility.Collapsed;
      boardingPopupWindow2.bodyContentBlurbControl.Visibility = Visibility.Visible;
      boardingPopupWindow2.bodyContentBlurbControl.FirstMessage.Text = LocaleStrings.GetLocalizedString("STRING_PLAY_BIGGER_MESSAGE", "");
      boardingPopupWindow2.bodyContentBlurbControl.KeyMessage.Text = this.ParentWindow.mCommonHandler.GetShortcutKeyFromName("STRING_UPDATED_FULLSCREEN_BUTTON_TOOLTIP", false);
      boardingPopupWindow2.bodyContentBlurbControl.SecondMessage.Text = LocaleStrings.GetLocalizedString("STRING_PLAY_BIGGER_FULL_SCREEN_MESSAGE", "");
      boardingPopupWindow2.PopArrowAlignment = PopupArrowAlignment.Right;
      boardingPopupWindow2.SetValue(Window.LeftProperty, (object) (sidebarElement.PointToScreen(new Point(0.0, 0.0)).X / MainWindow.sScalingFactor - (double) boardingPopupWindow2.LeftMargin));
      boardingPopupWindow2.SetValue(Window.TopProperty, (object) (sidebarElement.PointToScreen(new Point(0.0, 0.0)).Y / MainWindow.sScalingFactor - (double) boardingPopupWindow2.TopMargin));
      boardingPopupWindow2.RightArrow.Margin = new Thickness(0.0, 20.0, 0.0, 0.0);
      boardingPopupWindow2.RightArrow.VerticalAlignment = VerticalAlignment.Top;
      return boardingPopupWindow2;
    }

    internal void ShowEcoModeOnboardingBlurb()
    {
      Logger.Info("Inside method: ShowEcoModeOnboardingBlurb");
      if (this.ParentWindow.mSidebar.Visibility == Visibility.Collapsed || RegistryManager.Instance.IsEcoModeBlurbShown)
        return;
      SidebarElement sidebarElement = this.ParentWindow.mSidebar.mListSidebarElements.Where<SidebarElement>((Func<SidebarElement, bool>) (s => s.Image.ImageName == "sidebar_farmmode_active" || s.Image.ImageName == "sidebar_farmmode_inactive")).FirstOrDefault<SidebarElement>();
      if (sidebarElement == null)
        return;
      if (!sidebarElement.IsInMainSidebar)
      {
        this.mMoreElements.Opened += new EventHandler(this.MMoreElements_Opened1);
        this.mEnableMoreElementPopupTimer = false;
        this.mMoreElements.IsOpen = true;
        this.mMoreElements.Opened -= new EventHandler(this.MMoreElements_Opened1);
      }
      else
        this.ShowEcoModeBlurb();
    }

    private void ShowEcoModeBlurb()
    {
      SidebarElement sidebarElement = this.ParentWindow.mSidebar.mListSidebarElements.Where<SidebarElement>((Func<SidebarElement, bool>) (s => s.Image.ImageName == "sidebar_farmmode_active" || s.Image.ImageName == "sidebar_farmmode_inactive")).FirstOrDefault<SidebarElement>();
      if (sidebarElement == null)
        return;
      ThreadPool.QueueUserWorkItem((WaitCallback) (x =>
      {
        while (this.IsOnboardingRunning)
          Thread.Sleep(200);
        this.ParentWindow.Dispatcher.Invoke((Delegate) (() =>
        {
          this.mEcoModeBlurbPopup.PlacementTarget = (UIElement) sidebarElement;
          this.mEcoModeBlurbPopup.IsTopmost = true;
          this.mEcoModeBlurbPopup.IsOpen = true;
          this.mMoreElements.Opened -= new EventHandler(this.MMoreElements_Opened1);
          this.IsEcoModeEnabledFromMIM = false;
        }));
      }));
    }

    private void MMoreElements_Opened1(object sender, EventArgs e)
    {
      this.ShowEcoModeBlurb();
    }

    public void ShowUtcConverterPopup()
    {
      SidebarElement sidebarElement = this.ParentWindow.mSidebar.mElementsStackPanel.Children.Cast<SidebarElement>().Where<SidebarElement>((Func<SidebarElement, bool>) (s => s.Image.ImageName == "sidebar_utc_converter")).FirstOrDefault<SidebarElement>();
      if (sidebarElement == null)
        return;
      this.mUtcConverterBlurbPopup.PlacementTarget = (UIElement) sidebarElement;
      this.mUtcConverterBlurbPopup.IsOpen = true;
      this.mUtcConverterBlurbPopup.IsTopmost = true;
      BlueStacks.Common.Stats.SendCommonClientStatsAsync("general-onboarding", "utc_blurb_viewed", this.ParentWindow.mVmName, this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab?.PackageName, "", "", "");
    }

    private void UtcConverterOnBoardingPopupNext_Click(object sender, RoutedEventArgs e)
    {
      this.mUtcConverterBlurbPopup.IsOpen = false;
    }

    private void UtcConverterBlurbPopup_Closed(object sender, EventArgs e)
    {
      RegistryManager.Instance.IsUtcConverterBlurbOnboardingCompleted = true;
    }

    public void ShowViewGuidancePopup()
    {
      SidebarElement sidebarElement = this.ParentWindow.mSidebar.mStaticButtonsStackPanel.Children.Cast<SidebarElement>().Where<SidebarElement>((Func<SidebarElement, bool>) (s => s.Image.ImageName == "sidebar_gameguide" || s.Image.ImageName == "sidebar_gameguide_active")).FirstOrDefault<SidebarElement>();
      if (sidebarElement == null)
        return;
      this.mGameControlsBlurbPopup.PlacementTarget = (UIElement) sidebarElement;
      this.mGameControlsBlurbPopup.IsOpen = true;
      this.mGameControlsBlurbPopup.IsTopmost = true;
    }

    private void OnBoardingPopupNext_Click(object sender, RoutedEventArgs e)
    {
      BlueStacks.Common.Stats.SendCommonClientStatsAsync("general-onboarding", "okay_clicked", this.ParentWindow.mVmName, this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName, "ViewControlBlurb", "", "");
      this.mGameControlsBlurbPopup.IsOpen = false;
    }

    private void EcoModeBlurbPopupNext_Click(object sender, RoutedEventArgs e)
    {
      BlueStacks.Common.Stats.SendCommonClientStatsAsync("ecoMode-onboarding", "okay_clicked", this.ParentWindow.mVmName, "", "EcoModeBlurb", "", "");
      this.mEcoModeBlurbPopup.IsOpen = false;
      this.mMoreElements.IsOpen = false;
    }

    private void FarmModePopup_Opened(object sender, EventArgs e)
    {
      if (!this.GetElementFromTag("sidebar_farmmode_inactive").IsInMainSidebar)
        this.MMoreButton_MouseLeave((object) null, (MouseEventArgs) null);
      this.ParentWindow.AllowFrontendFocusOnClientClick = false;
      RegistryManager.Instance.IsEcoModeBlurbShown = true;
    }

    private void EcoModeBlurbPopup_Opened(object sender, EventArgs e)
    {
      --this.mEcoModeBlurbPopup.HorizontalOffset;
      RegistryManager.Instance.IsEcoModeBlurbShown = true;
    }

    private void EcoModeBlurbPopup_Closed(object sender, EventArgs e)
    {
      this.mEnableMoreElementPopupTimer = true;
    }

    private void FarmModePopup_Closed(object sender, EventArgs e)
    {
      if (!this.GetElementFromTag("sidebar_farmmode_inactive").IsInMainSidebar)
        this.MMoreButton_MouseLeave((object) null, (MouseEventArgs) null);
      this.ParentWindow.AllowFrontendFocusOnClientClick = true;
      this.UpdateEcoModeFps();
    }

    private void FarmModePopup_MouseLeave(object sender, MouseEventArgs e)
    {
      if (!this.mFarmModePopup.IsOpen)
        return;
      if (this.mFarmModePopupTimer == null)
      {
        this.mFarmModePopupTimer = new DispatcherTimer()
        {
          Interval = new TimeSpan(0, 0, 1)
        };
        this.mFarmModePopupTimer.Tick += new EventHandler(this.FarmModePopupTimer_Tick);
      }
      else
        this.mFarmModePopupTimer.Stop();
      this.mFarmModePopupTimer.Start();
    }

    private void FarmModePopupTimer_Tick(object sender, EventArgs e)
    {
      if (!this.mFarmModePopup.IsMouseOver && !this.GetElementFromTag("sidebar_farmmode_inactive").IsMouseOver)
      {
        this.mFarmModePopup.IsOpen = false;
        if (this.mIsInFullscreenMode && !this.IsMouseOver)
          this.ToggleSidebarVisibilityInFullscreen(false);
      }
      (sender as DispatcherTimer).Stop();
    }

    private void FarmingModeToggleButton_PreviewMouseLeftButtonUp(
      object sender,
      MouseButtonEventArgs e)
    {
      bool isfarmModeEnabled = !this.mFarmingModeToggleButton.BoolValue;
      this.ToggleFarmMode(isfarmModeEnabled);
      ClientStats.SendMiscellaneousStatsAsync("side_toolbar_eco_mode_changed", RegistryManager.Instance.UserGuid, isfarmModeEnabled ? "On" : "Off", (string) null, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, this.ParentWindow.mVmName, (string) null, "Android");
    }

    private void FarmingModeSoundToggleButton_MouseLeftButtonUp(
      object sender,
      MouseButtonEventArgs e)
    {
      bool boolValue = this.mEcoModeSoundToggleButton.BoolValue;
      this.ParentWindow.mCommonHandler.MuteUnmuteButtonHandler();
      ClientStats.SendMiscellaneousStatsAsync("side_toolbar_eco_mode_sound", RegistryManager.Instance.UserGuid, boolValue ? "Off" : "On", (string) null, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, this.ParentWindow.mVmName, (string) null, "Android");
    }

    private void ToggleFarmMode(bool isfarmModeEnabled)
    {
      if (isfarmModeEnabled)
      {
        this.mMutedStateBeforeFarmMode = this.ParentWindow.IsMuted;
        this.ParentWindow.Utils.MuteApplication();
      }
      else if (this.mMutedStateBeforeFarmMode)
        this.ParentWindow.Utils.MuteApplication();
      else
        this.ParentWindow.Utils.UnmuteApplication();
      this.IsFarmingModeEnabled = isfarmModeEnabled;
      this.UpdateImage("sidebar_farmmode_inactive", isfarmModeEnabled ? "sidebar_farmmode_active" : "sidebar_farmmode_inactive");
      this.HandleFarmMode(isfarmModeEnabled);
      this.mEcoModeSoundToggleButton.BoolValue = false;
    }

    public void ToggleFarmModeForMIM(bool isfarmModeEnabled)
    {
      this.IsEcoModeEnabledFromMIM = true;
      this.mFarmingModeToggleButton.BoolValue = isfarmModeEnabled;
      this.ToggleFarmMode(this.mFarmingModeToggleButton.BoolValue);
    }

    private void HandleFarmMode(bool isfarmModeEnabled)
    {
      this.mIsEcoModeEnabled = isfarmModeEnabled;
      RegistryManager.Instance.Guest[this.ParentWindow.mVmName].EcoModeFPS = this.FarmSensitivity;
      this.ParentWindow.mFrontendHandler.SendFrontendRequestAsync("farmModeHandler", new Dictionary<string, string>()
      {
        {
          "enable",
          isfarmModeEnabled.ToString((IFormatProvider) CultureInfo.InvariantCulture)
        },
        {
          "fps",
          string.IsNullOrEmpty(this.FarmSensitivity) ? "1" : this.FarmSensitivity.ToString((IFormatProvider) CultureInfo.InvariantCulture)
        }
      });
    }

    private void FarmSenstivityLostFocus(object sender, RoutedEventArgs e)
    {
      this.UpdateEcoModeFps();
    }

    private void UpdateEcoModeFps()
    {
      this.FarmSensitivity = !string.IsNullOrEmpty(this.mFarmSettingStepperControl.Text) ? this.mFarmSettingStepperControl.Text : "1";
      if (!(RegistryManager.Instance.Guest[this.ParentWindow.mVmName].EcoModeFPS != this.FarmSensitivity))
        return;
      this.HandleFarmMode(this.IsFarmingModeEnabled);
      ClientStats.SendMiscellaneousStatsAsync("side_toolbar_eco_mode_fpsnum", RegistryManager.Instance.UserGuid, this.FarmSensitivity, (string) null, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, this.ParentWindow.mVmName, (string) null, "Android");
    }

    private void EcoModeHelp_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
      ClientStats.SendMiscellaneousStatsAsync("side_toolbar_eco_mode_help", RegistryManager.Instance.UserGuid, (string) null, (string) null, RegistryManager.Instance.ClientVersion, Oem.Instance.OEM, (string) null, (string) null, (string) null, "Android");
      Utils.OpenUrl(WebHelper.GetUrlWithParams(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) WebHelper.GetServerHost(), (object) "help_articles"), (string) null, (string) null, (string) null) + "&article=EcoMode_help");
    }

    private void KeyboardIconImage_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.ParentWindow.mCommonHandler.SwitchOverlayControls(false);
      ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "KeyboardOverlay", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
    }

    private void GamepadIconImage_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.ParentWindow.mCommonHandler.SwitchOverlayControls(true);
      ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "GamepadOverlay", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
    }

    private void HelpImage_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      Utils.OpenUrl(WebHelper.GetUrlWithParams(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) WebHelper.GetServerHost(), (object) "help_articles"), (string) null, (string) null, (string) null) + "&article=game_controls_help");
      ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "GameControlsHelpIcon", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
    }

    private void GameControlsToggle_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.KeyMapSwitchButtonHandler((SidebarElement) null, true);
    }

    private void ViewControlsPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.mGameControlButtonPopup.IsOpen = false;
      if (KMManager.sGuidanceWindow != null && KMManager.sGuidanceWindow.Visibility == Visibility.Visible)
      {
        KMManager.sGuidanceWindow.Highlight();
        ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "GameGuide", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
      }
      else
        this.ParentWindow.mCommonHandler.GameGuideButtonHandler("MouseClick", "sidebar", true);
    }

    internal OnBoardingPopupWindow ShowKeyboardIconOnboardingBlurb()
    {
      try
      {
        if (this.ParentWindow.mSidebar.Visibility == Visibility.Collapsed)
          return (OnBoardingPopupWindow) null;
        SidebarElement sidebarElement = this.ParentWindow.mSidebar.mElementsStackPanel.Children.Cast<SidebarElement>().Where<SidebarElement>((Func<SidebarElement, bool>) (s => s.Image.ImageName == "sidebar_controls")).FirstOrDefault<SidebarElement>();
        if (sidebarElement == null)
          return (OnBoardingPopupWindow) null;
        OnBoardingPopupWindow boardingPopupWindow1 = new OnBoardingPopupWindow(this.ParentWindow, this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName);
        boardingPopupWindow1.Owner = (Window) this.ParentWindow;
        boardingPopupWindow1.PlacementTarget = (UIElement) sidebarElement;
        boardingPopupWindow1.Title = "OnScreenControlsBlurb";
        boardingPopupWindow1.LeftMargin = 310;
        boardingPopupWindow1.TopMargin = 45;
        boardingPopupWindow1.IsBlurbRelatedToGuidance = false;
        boardingPopupWindow1.HeaderContent = LocaleStrings.GetLocalizedString("STRING_ONSCREEN_CONTROLS", "");
        OnBoardingPopupWindow boardingPopupWindow2 = boardingPopupWindow1;
        boardingPopupWindow2.bodyTextBlock.Visibility = Visibility.Visible;
        boardingPopupWindow2.BodyContent = string.Format((IFormatProvider) CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_KEYBOARD_ICON_ONBOARDING_BODY_TEXT", ""));
        boardingPopupWindow2.PopArrowAlignment = PopupArrowAlignment.Right;
        boardingPopupWindow2.SetValue(Window.LeftProperty, (object) (sidebarElement.PointToScreen(new Point(0.0, 0.0)).X / MainWindow.sScalingFactor - (double) boardingPopupWindow2.LeftMargin));
        boardingPopupWindow2.SetValue(Window.TopProperty, (object) (sidebarElement.PointToScreen(new Point(0.0, 0.0)).Y / MainWindow.sScalingFactor - (double) boardingPopupWindow2.TopMargin));
        boardingPopupWindow2.RightArrow.Margin = new Thickness(0.0, 20.0, 0.0, 0.0);
        boardingPopupWindow2.RightArrow.VerticalAlignment = VerticalAlignment.Top;
        return boardingPopupWindow2;
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in ShowKeyboardIconOnboardingBlurb: " + ex.ToString());
      }
      return (OnBoardingPopupWindow) null;
    }

    private void GameControlsPanel_MouseEnter(object sender, MouseEventArgs e)
    {
      BlueStacksUIBinding.BindColor((DependencyObject) (sender as DockPanel), Panel.BackgroundProperty, "ContextMenuItemBackgroundHoverColor");
    }

    private void GameControlsPanel_MouseLeave(object sender, MouseEventArgs e)
    {
      (sender as DockPanel).Background = (Brush) Brushes.Transparent;
    }

    private void ViewWindowsFolderTextBlock_PreviewMouseLeftButtonUp(
      object sender,
      MouseButtonEventArgs e)
    {
      CommonHandlers.OpenMediaFolderWithFileSelected(this.currentScreenshotSavedPath);
      this.mScreenshotPopup.IsOpen = false;
      ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "ss_open_gallery", "windows_folder", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
    }

    private void ViewInGalleryTextBlock_PreviewMouseLeftButtonUp(
      object sender,
      MouseButtonEventArgs e)
    {
      try
      {
        this.mScreenshotPopup.IsOpen = false;
        AppIconModel appIcon = this.ParentWindow.mWelcomeTab.mHomeAppManager.GetAppIcon("com.bluestacks.filemanager");
        if (appIcon != null && this.ParentWindow.mGuestBootCompleted)
          this.ParentWindow.mTopBar.mAppTabButtons.AddAppTab(appIcon.AppName, appIcon.PackageName, appIcon.ActivityName, appIcon.ImageName, true, true, false);
        ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "ss_open_gallery", "gallery", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in opening media manager.." + ex.ToString());
      }
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/sidebar.xaml", UriKind.Relative));
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
          this.mSidebar = (Sidebar) target;
          this.mSidebar.SizeChanged += new SizeChangedEventHandler(this.MSidebar_SizeChanged);
          this.mSidebar.Loaded += new RoutedEventHandler(this.Sidebar_Loaded);
          break;
        case 2:
          this.mBorder = (Border) target;
          break;
        case 3:
          this.mGrid = (Grid) target;
          break;
        case 4:
          this.mTopGrid = (Grid) target;
          break;
        case 5:
          this.mElementsStackPanel = (StackPanel) target;
          break;
        case 6:
          this.mMoreButton = (SidebarElement) target;
          break;
        case 7:
          this.mChangeTransparencyPopup = (CustomPopUp) target;
          break;
        case 8:
          this.mMaskBorder2 = (Border) target;
          break;
        case 9:
          this.mVolumeSliderPopup = (CustomPopUp) target;
          break;
        case 10:
          this.mMaskBorder3 = (Border) target;
          break;
        case 11:
          this.mVolumeMuteUnmuteImage = (CustomPictureBox) target;
          this.mVolumeMuteUnmuteImage.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.VolumeImage_PreviewMouseLeftButtonUp);
          break;
        case 12:
          this.mVolumeSlider = (Slider) target;
          this.mVolumeSlider.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.VolumeSlider_PreviewMouseLeftButtonUp);
          this.mVolumeSlider.ValueChanged += new RoutedPropertyChangedEventHandler<double>(this.VolumeSlider_ValueChanged);
          break;
        case 13:
          this.mCurrentVolumeValue = (TextBlock) target;
          break;
        case 14:
          this.mMuteInstancesCheckboxImage = (CustomPictureBox) target;
          this.mMuteInstancesCheckboxImage.MouseLeftButtonUp += new MouseButtonEventHandler(this.MuteInstancesCheckboxImage_MouseLeftButtonUp);
          break;
        case 15:
          this.mMuteAllInstancesTextBlock = (TextBlock) target;
          this.mMuteAllInstancesTextBlock.MouseLeftButtonUp += new MouseButtonEventHandler(this.MuteInstancesCheckboxImage_MouseLeftButtonUp);
          break;
        case 16:
          this.mOverlayTooltip = (CustomPopUp) target;
          break;
        case 17:
          this.mMaskBorder4 = (Border) target;
          break;
        case 18:
          ((UIElement) target).MouseLeftButtonUp += new MouseButtonEventHandler(this.OverlayTooltipCPB_MouseLeftButtonUp);
          break;
        case 19:
          this.mOverlayPopUpTitle = (TextBlock) target;
          break;
        case 20:
          this.mOverlayPopUpMessage = (TextBlock) target;
          break;
        case 21:
          this.mOverlayDoNotShowCheckboxImage = (CustomPictureBox) target;
          this.mOverlayDoNotShowCheckboxImage.MouseLeftButtonUp += new MouseButtonEventHandler(this.OverlayDoNotShowCheckbox_MouseLeftButtonUp);
          break;
        case 22:
          this.mOverlayDontShowPopUp = (TextBlock) target;
          this.mOverlayDontShowPopUp.MouseLeftButtonUp += new MouseButtonEventHandler(this.OverlayDoNotShowCheckbox_MouseLeftButtonUp);
          break;
        case 23:
          this.mMacroButtonPopup = (CustomPopUp) target;
          break;
        case 24:
          this.mMaskBorder5 = (Border) target;
          break;
        case 25:
          this.mMacroBookmarkPopup = (MacroBookmarksPopup) target;
          break;
        case 26:
          this.mCustomiseSectionTag = (Grid) target;
          break;
        case 27:
          this.mCustomiseSectionBorderLine = (Separator) target;
          break;
        case 28:
          ((UIElement) target).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.OpenMacroGridPreviewMouseLeftButtonUp);
          ((UIElement) target).MouseEnter += new MouseEventHandler(this.OpenMacroGridMouseEnter);
          ((UIElement) target).MouseLeave += new MouseEventHandler(this.OpenMacroGridMouseLeave);
          break;
        case 29:
          this.mOpenMacroTextbox = (TextBlock) target;
          break;
        case 30:
          this.mGameControlButtonPopup = (CustomPopUp) target;
          break;
        case 31:
          this.mMaskBorder6 = (Border) target;
          break;
        case 32:
          this.mGameControlsToggle = (CustomToggleButtonWithState) target;
          this.mGameControlsToggle.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.GameControlsToggle_PreviewMouseLeftButtonUp);
          break;
        case 33:
          this.mHelpImage = (CustomPictureBox) target;
          this.mHelpImage.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.HelpImage_PreviewMouseLeftButtonUp);
          break;
        case 34:
          this.mControlPanel = (StackPanel) target;
          break;
        case 35:
          this.mOverlayToggleButton = (CustomToggleButtonWithState) target;
          this.mOverlayToggleButton.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.mTranslucentControlsSliderButton_PreviewMouseLeftButtonUp);
          break;
        case 36:
          this.mControlsForTxtBlock = (CustomTextBlock) target;
          break;
        case 37:
          this.mGamepadIconImage = (CustomPictureBox) target;
          this.mGamepadIconImage.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.GamepadIconImage_PreviewMouseLeftButtonUp);
          break;
        case 38:
          this.mKeyboardIconImage = (CustomPictureBox) target;
          this.mKeyboardIconImage.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.KeyboardIconImage_PreviewMouseLeftButtonUp);
          break;
        case 39:
          this.transSlider = (Slider) target;
          this.transSlider.ValueChanged += new RoutedPropertyChangedEventHandler<double>(this.TransparencySlider_ValueChanged);
          break;
        case 40:
          this.mViewGameControlsPanel = (DockPanel) target;
          this.mViewGameControlsPanel.MouseEnter += new MouseEventHandler(this.GameControlsPanel_MouseEnter);
          this.mViewGameControlsPanel.MouseLeave += new MouseEventHandler(this.GameControlsPanel_MouseLeave);
          break;
        case 41:
          this.mViewControlsTxtBlock = (CustomTextBlock) target;
          this.mViewControlsTxtBlock.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.ViewControlsPreviewMouseLeftButtonUp);
          break;
        case 42:
          ((UIElement) target).MouseEnter += new MouseEventHandler(this.GameControlsPanel_MouseEnter);
          ((UIElement) target).MouseLeave += new MouseEventHandler(this.GameControlsPanel_MouseLeave);
          break;
        case 43:
          this.mOpenEditorTxtBlock = (CustomTextBlock) target;
          this.mOpenEditorTxtBlock.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.OpenGameControlPreviewMouseLeftButtonUp);
          break;
        case 44:
          this.mBookmarkStackPanel = (StackPanel) target;
          break;
        case 45:
          this.mBookmarkedSchemesStackPanel = (StackPanel) target;
          break;
        case 46:
          this.mRecordScreenPopup = (CustomPopUp) target;
          break;
        case 47:
          this.mMaskBorder7 = (Border) target;
          break;
        case 48:
          this.mRecordScreenClose = (CustomPictureBox) target;
          this.mRecordScreenClose.IsVisibleChanged += new DependencyPropertyChangedEventHandler(this.RecordScreenClose_IsVisibleChanged);
          this.mRecordScreenClose.MouseLeftButtonUp += new MouseButtonEventHandler(this.RecordScreenPopupClose_MouseLeftButtonUp);
          break;
        case 49:
          this.RecordScreenPopupHeader = (TextBlock) target;
          break;
        case 50:
          this.RecordScreenPopupBody = (TextBlock) target;
          break;
        case 51:
          this.RecordScreenPopupHyperlink = (TextBlock) target;
          break;
        case 52:
          ((Hyperlink) target).Click += new RoutedEventHandler(this.RecordScreenPopupHyperlink_Click);
          break;
        case 53:
          this.mRecorderClickLink = (TextBlock) target;
          break;
        case 54:
          this.mScreenshotPopup = (CustomPopUp) target;
          break;
        case 55:
          this.mMaskBorder8 = (Border) target;
          break;
        case 56:
          this.mScreenshotClose = (CustomPictureBox) target;
          this.mScreenshotClose.MouseLeftButtonUp += new MouseButtonEventHandler(this.ScreenshotPopupClose_MouseLeftButtonUp);
          break;
        case 57:
          this.ScreenshotPopupHeader = (TextBlock) target;
          break;
        case 58:
          ((UIElement) target).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.ViewWindowsFolderTextBlock_PreviewMouseLeftButtonUp);
          break;
        case 59:
          this.mViewInGalleryTextBlock = (TextBlock) target;
          this.mViewInGalleryTextBlock.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.ViewInGalleryTextBlock_PreviewMouseLeftButtonUp);
          break;
        case 60:
          this.mGameControlsBlurbPopup = (CustomPopUp) target;
          break;
        case 61:
          this.mMaskBorder10 = (Border) target;
          break;
        case 62:
          ((ButtonBase) target).Click += new RoutedEventHandler(this.OnBoardingPopupNext_Click);
          break;
        case 63:
          this.mEcoModeBlurbPopup = (CustomPopUp) target;
          break;
        case 64:
          this.mMaskBorder12 = (Border) target;
          break;
        case 65:
          ((ButtonBase) target).Click += new RoutedEventHandler(this.EcoModeBlurbPopupNext_Click);
          break;
        case 66:
          this.mUtcConverterBlurbPopup = (CustomPopUp) target;
          break;
        case 67:
          this.mMaskBorder11 = (Border) target;
          break;
        case 68:
          ((ButtonBase) target).Click += new RoutedEventHandler(this.UtcConverterOnBoardingPopupNext_Click);
          break;
        case 69:
          this.mMoreElements = (CustomPopUp) target;
          break;
        case 70:
          this.mPopupGrid = (Grid) target;
          break;
        case 71:
          this.mMaskBorder = (Border) target;
          break;
        case 72:
          this.mSidebarPopup = (SidebarPopup) target;
          break;
        case 73:
          this.mFarmModePopup = (CustomPopUp) target;
          break;
        case 74:
          this.mMaskBorder9 = (Border) target;
          break;
        case 75:
          this.mFarmingModeToggleButton = (CustomToggleButtonWithState) target;
          this.mFarmingModeToggleButton.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.FarmingModeToggleButton_PreviewMouseLeftButtonUp);
          break;
        case 76:
          this.mEcoModeHelp = (CustomPictureBox) target;
          this.mEcoModeHelp.PreviewMouseDown += new MouseButtonEventHandler(this.EcoModeHelp_PreviewMouseDown);
          break;
        case 77:
          this.mEcoModeSoundToggleButton = (CustomToggleButtonWithState) target;
          this.mEcoModeSoundToggleButton.MouseLeftButtonUp += new MouseButtonEventHandler(this.FarmingModeSoundToggleButton_MouseLeftButtonUp);
          break;
        case 78:
          this.mFarmSettingStepperControl = (StepperTextBox) target;
          break;
        case 79:
          this.mBottomGrid = (Grid) target;
          break;
        case 80:
          this.mStaticButtonsStackPanel = (StackPanel) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
