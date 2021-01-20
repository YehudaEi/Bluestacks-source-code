// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.MainWindow
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.BlueStacksUI.BTv;
using BlueStacks.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace BlueStacks.BlueStacksUI
{
  public class MainWindow : CustomWindow, IDisposable, IComponentConnector, IStyleConnector
  {
    internal static double sScalingFactor = 1.0;
    internal static bool sIsClosingForBackupRestore = false;
    internal static bool sShowNotifications = true;
    internal static Dictionary<string, string> sMacroMapping = new Dictionary<string, string>();
    private bool mAllowFrontendFocusOnClientClick = true;
    private int heightDiffScaled = 42;
    private int widthDiffScaled = 2;
    internal Fraction mAspectRatio = new Fraction(16L, 9L);
    internal int MaxHeightScaled = 10000;
    internal int MaxWidthScaled = 10000;
    internal bool mIsDMMRecommendedWindowOpen = true;
    internal Rect DmmRestoreWindowRectangle = new Rect(0.0, 0.0, 0.0, 0.0);
    internal Dictionary<string, bool> AppForcedOrientationDict = new Dictionary<string, bool>();
    internal Dictionary<string, bool> mGamepadOverlaySelectedDict = new Dictionary<string, bool>();
    internal string mCallbackEnabled = "False";
    internal DateTime mBootStartTime = DateTime.Now;
    internal List<string> mSelectedInstancesForSync = new List<string>();
    internal string mMacroPlaying = string.Empty;
    internal string mVmName = BlueStacks.Common.Strings.CurrentDefaultVmName;
    private readonly SerialWorkQueue pikaNotificationWorkQueue = new SerialWorkQueue(nameof (pikaNotificationWorkQueue));
    private readonly DispatcherTimer pikaNotificationTimer = new DispatcherTimer();
    private readonly DispatcherTimer toastTimer = new DispatcherTimer();
    private readonly DispatcherTimer mFullScreenToastTimer = new DispatcherTimer();
    private readonly DispatcherTimer mImageUploadedToastTimer = new DispatcherTimer();
    internal InstanceWiseCloudInfoManager mInstanceWiseCloudInfoManager = new InstanceWiseCloudInfoManager();
    private Mutex mBlueStacksClientInstanceLock;
    private const long OneGB = 1073741824;
    internal int MinWidthScaled;
    internal int MinHeightScaled;
    internal bool mIsDmmMaximised;
    internal bool mIsDMMMaximizedFromPortrait;
    internal DMMFullScreenTopBar mDMMFST;
    internal DMMRecommendedWindow mDMMRecommendedWindow;
    private bool mIsWindowResizedOnce;
    internal bool mIsFullScreenFromMaximized;
    internal bool mIsMinimizedThroughCloseButton;
    internal bool mIsStreaming;
    private bool isSetupDone;
    private double? mPreviousWidth;
    private double? mPreviousHeight;
    internal bool IsUIInPortraitMode;
    internal bool IsUIInPortraitModeWhenMaximized;
    private Grid mLastVisibleGrid;
    internal QuitPopupBrowserControl mQuitPopupBrowserControl;
    internal bool mIsFullScreen;
    internal bool mIsMaximized;
    internal bool mFullScreenRestoredButNotSidebar;
    internal bool mIsFocusComeFromImap;
    internal bool mIsManualCheck;
    internal bool mKeymappingFilesDownloaded;
    private IMConfig mSelectedConfig;
    private IMConfig mOriginalLoadedConfig;
    private bool mIsGamepadConnected;
    private bool mSkipNextGamepadStatus;
    private Grid mResizeGrid;
    internal bool mIsResizing;
    internal EventHandler ResizeBegin;
    internal EventHandler ResizeEnd;
    private bool mClosing;
    internal bool mGuestBootCompleted;
    internal bool mEnableLaunchPlayForNCSoft;
    internal volatile bool mIsWindowInFocus;
    internal string mBrowserCallbackFunctionName;
    internal bool IsQuitPopupNotficationReceived;
    private Grid mFirebaseBrowserControlGrid;
    internal MacroRecording mAutoRunMacro;
    private ScreenLockControl mScreenLock;
    private MacroOverlay mMacroOverlay;
    internal CommonHandlers mCommonHandler;
    internal FrontendHandler mFrontendHandler;
    internal DownloadInstallApk mAppInstaller;
    internal AppHandler mAppHandler;
    internal bool mStreamingModeEnabled;
    internal PostOtsWelcomeWindowControl mPostOtsWelcomeWindow;
    private MacroRecorderWindow mMacroRecorderWindow;
    internal SynchronizerWindow mSynchronizerWindow;
    internal bool mIsMacroPlaying;
    internal bool mIsScriptsPresent;
    internal System.Timers.Timer mMacroTimer;
    internal bool mIsSyncMaster;
    private BlueStacksUIUtils mUtils;
    internal WindowWndProcHandler mResizeHandler;
    private MainWindowsStaticComponents mStaticComponents;
    private bool mIsTokenAvailable;
    private readonly bool mIsWindowLoadedOnce;
    internal DimOverlayControl mDimOverlay;
    internal IntPtr Handle;
    internal bool mIsRestart;
    private Storyboard mStoryBoard;
    internal bool mIsMacroRecorderActive;
    internal bool mIsSynchronisationActive;
    internal bool mStartupTabLaunched;
    internal bool mLaunchStartupTabWhenTokenReceived;
    private bool isPikaPopOpen;
    private bool mIsLockScreenActionPending;
    private bool disposedValue;
    private bool mIsSideButtonDragging;
    private System.Windows.Point mSideButtonOldPosition;
    private Thickness mOldSideButtonMargin;
    private bool mIsTopButtonDragging;
    private System.Windows.Point mTopButtonOldPosition;
    private Thickness mOldTopButtonMargin;
    internal MainWindow mMainWindow;
    internal Border OuterBorder;
    internal Grid MainGrid;
    internal CustomPopUp pikaPop;
    internal Canvas pikaCanvas;
    internal PikaNotificationControl pikaPopControl;
    internal CustomPopUp toastPopup;
    internal Canvas toastCanvas;
    internal CustomToastPopupControl toastControl;
    internal CustomPopUp mFullScreenToastPopup;
    internal Canvas mFullScreenToastCanvas;
    internal FullScreenToastPopupControl mFullScreenToastControl;
    internal CustomPopUp mUnlockMouseToastPopup;
    internal Canvas mUnlockMouseToastCanvas;
    internal UnlockMouseToastPopupControl mUnlockMouseToastControl;
    internal CustomPopUp mConfigUpdatedPopup;
    internal Canvas mConfigUpdatedCanvas;
    internal CustomToastPopupControl mConfigUpdatedControl;
    internal CustomPopUp mImageUploadedPopup;
    internal Canvas mImageUploadedCanvas;
    internal CustomToastPopupControl mImageUploadedControl;
    internal CustomPopUp mGeneraltoast;
    internal Canvas mGeneraltoastCanvas;
    internal CustomToastPopupControl mGeneraltoastControl;
    internal CustomPopUp mShootingModePopup;
    internal Canvas mShootingModePopupCanvas;
    internal CustomPersistentToastPopupControl mToastControl;
    internal CustomPopUp mTopBarPopup;
    internal FullScreenTopBar mFullScreenTopBar;
    internal CustomPopUp mFullscreenSidebarPopupButton;
    internal Grid mFullscreenSidebarPopupButtonInnerGrid;
    internal System.Windows.Controls.Button mFullscreenSidebarButton;
    internal CustomPopUp mFullscreenSidebarPopup;
    internal Grid mFullscreenSidebarPopupInnerGrid;
    internal CustomPopUp mFullscreenTopbarPopupButton;
    internal Grid mFullscreenTopbarPopupButtonInnerGrid;
    internal System.Windows.Controls.Button mFullscreenTopbarButton;
    internal CustomPopUp mFullscreenTopbarPopup;
    internal Grid mFullscreenTopbarPopupInnerGrid;
    internal TopbarOptions mTopbarOptions;
    internal Grid mMainWindowTopGrid;
    internal BlueStacks.BlueStacksUI.TopBar mTopBar;
    internal NCSoftTopBar mNCTopBar;
    internal FrontendOTSControl mFrontendOTSControl;
    internal Grid dummyPika;
    internal Grid mContentGrid;
    internal Grid WelcomeTabParentGrid;
    internal WelcomeTab mWelcomeTab;
    internal Grid FrontendParentGrid;
    internal DMMProgressControl mDmmProgressControl;
    internal Grid mFrontendGrid;
    internal DMMBottomBar mDmmBottomBar;
    internal Grid dummyToast2;
    internal Sidebar mSidebar;
    internal Grid dummyToast;
    internal Grid dummyTooltip;
    internal ProgressBar mExitProgressGrid;
    internal Grid mQuitPopupBrowserLoadGrid;
    private bool _contentLoaded;

    internal string WindowLaunchParams { get; set; } = "";

    internal bool IsFarmingInstance { get; set; }

    public bool AllowFrontendFocusOnClientClick
    {
      get
      {
        return this.mAllowFrontendFocusOnClientClick;
      }
      set
      {
        if (this.mAllowFrontendFocusOnClientClick == value)
          return;
        this.mAllowFrontendFocusOnClientClick = value;
        if (this.mAllowFrontendFocusOnClientClick)
          this.mFrontendHandler.ShowGLWindow();
        else
          this.Focus();
        Logger.Info(string.Format("Enable FrontendFocusOnClient: {0}", (object) this.mAllowFrontendFocusOnClientClick));
      }
    }

    internal int ParentWindowHeightDiff { get; set; } = 42;

    internal int ParentWindowWidthDiff { get; set; } = 2;

    public System.Windows.Controls.UserControl TopBar
    {
      get
      {
        return !FeatureManager.Instance.IsCustomUIForNCSoft ? (System.Windows.Controls.UserControl) this.mTopBar : (System.Windows.Controls.UserControl) this.mNCTopBar;
      }
    }

    internal ITopBar _TopBar
    {
      get
      {
        return !FeatureManager.Instance.IsCustomUIForNCSoft ? (ITopBar) this.mTopBar : (ITopBar) this.mNCTopBar;
      }
    }

    internal IMConfig SelectedConfig
    {
      get
      {
        if (this.mSelectedConfig == null)
          this.mSelectedConfig = new IMConfig();
        return this.mSelectedConfig;
      }
      set
      {
        this.mSelectedConfig = value;
      }
    }

    internal IMConfig OriginalLoadedConfig
    {
      get
      {
        if (this.mOriginalLoadedConfig == null)
          this.mOriginalLoadedConfig = new IMConfig();
        return this.mOriginalLoadedConfig;
      }
      set
      {
        this.mOriginalLoadedConfig = value;
      }
    }

    internal Dictionary<string, int> AppNotificationCountDictForEachVM { get; set; } = new Dictionary<string, int>();

    internal bool SkipNextGamepadStatus
    {
      get
      {
        return this.mSkipNextGamepadStatus;
      }
      set
      {
        this.mSkipNextGamepadStatus = value;
        if (!this.mSkipNextGamepadStatus)
          return;
        this.WasGamepadStatusSkipped = value;
      }
    }

    public static void OpenSettingsWindow(MainWindow window, string startTab)
    {
      if (window == null)
        return;
      if (KMManager.sGuidanceWindow != null && !KMManager.sGuidanceWindow.IsClosed && !KMManager.sGuidanceWindow.IsViewState)
      {
        CustomMessageWindow customMessageWindow = new CustomMessageWindow();
        customMessageWindow.Owner = (Window) window.mDimOverlay;
        customMessageWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_BLUESTACKS_GAME_CONTROLS", "");
        customMessageWindow.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_CANNOT_OPEN_SETTING", "");
        customMessageWindow.AddButton(ButtonColors.White, LocaleStrings.GetLocalizedString("STRING_OK", ""), (EventHandler) ((o, evt) => {}), (string) null, false, (object) null, true);
        window.ShowDimOverlay((IDimOverlayControl) null);
        customMessageWindow.ShowDialog();
        window.HideDimOverlay();
      }
      else if (MainWindow.SettingsWindow == null)
      {
        MainWindow.SettingsWindow = new SettingsWindow(window, startTab);
        int num1 = 500;
        int num2 = 750;
        ContainerWindow containerWindow = new ContainerWindow(window, (System.Windows.Controls.UserControl) MainWindow.SettingsWindow, (double) num2, (double) num1, false, true, false, -1.0, (System.Windows.Media.Brush) null);
      }
      else
        MainWindow.SettingsWindow.ChangeSettingsTab(window, startTab);
    }

    public static void CloseSettingsWindow(SettingsWindow window)
    {
      MainWindow.SettingsWindow = window;
      if (MainWindow.SettingsWindow == null)
        return;
      BlueStacksUIUtils.CloseContainerWindow((FrameworkElement) MainWindow.SettingsWindow);
    }

    internal bool WasGamepadStatusSkipped { get; set; }

    internal bool IsGamepadConnected
    {
      get
      {
        return this.mIsGamepadConnected;
      }
      set
      {
        this.mIsGamepadConnected = value;
        if (RegistryManager.Instance.IsShowToastNotification && !this.SkipNextGamepadStatus)
        {
          this.ShowGamepadToast(value);
          if (this.mVmName.Equals(BlueStacks.Common.Strings.CurrentDefaultVmName, StringComparison.InvariantCultureIgnoreCase) & value && RegistryManager.Instance.IsShowGamepadDesktopNotification)
            HTTPUtils.SendRequestToAgentAsync("showClientNotification", new Dictionary<string, string>()
            {
              {
                "title",
                LocaleStrings.GetLocalizedString("STRING_BLUESTACKS", "")
              },
              {
                "description",
                LocaleStrings.GetLocalizedString("STRING_GAMEPAD_CONNECTED_DESKTOP_NOTIFICATION", "")
              }
            }, this.mVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
        }
        this.SkipNextGamepadStatus = false;
        BlueStacksUIUtils.SendGamepadStatusToBrowsers(value);
        this.mWelcomeTab.mHomeAppManager.UpdateGamepadIcons(value);
      }
    }

    public DummyTaskbarWindow DummyWindow { get; set; }

    private void ShowGamepadToast(bool state)
    {
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        try
        {
          if (!this.mIsWindowInFocus)
            return;
          if (this.toastPopup.IsOpen)
          {
            this.toastTimer.Stop();
            this.toastPopup.IsOpen = false;
          }
          double right = 10.0 + this.mSidebar.ActualWidth > 0.0 ? this.mSidebar.ActualWidth : 0.0 + this.mWelcomeTab.mHomeAppManager.GetAppRecommendationsGridWidth();
          if (state)
          {
            this.toastControl.Init((Window) this, LocaleStrings.GetLocalizedString("STRING_GAMEPAD_CONNECTED", ""), (System.Windows.Media.Brush) null, (System.Windows.Media.Brush) new SolidColorBrush(System.Windows.Media.Color.FromArgb((byte) 85, byte.MaxValue, byte.MaxValue, byte.MaxValue)), System.Windows.HorizontalAlignment.Right, VerticalAlignment.Bottom, new Thickness?(new Thickness(0.0, 0.0, right, 20.0)), 5, new Thickness?(), (System.Windows.Media.Brush) null, false, false);
            this.toastControl.AddImage("gamepad_connected", 16.0, 24.0, new Thickness?(new Thickness(0.0, 5.0, 10.0, 5.0)));
          }
          else
          {
            this.toastControl.Init((Window) this, LocaleStrings.GetLocalizedString("STRING_GAMEPAD_DISCONNECTED", ""), (System.Windows.Media.Brush) null, (System.Windows.Media.Brush) new SolidColorBrush(System.Windows.Media.Color.FromArgb((byte) 85, byte.MaxValue, byte.MaxValue, byte.MaxValue)), System.Windows.HorizontalAlignment.Right, VerticalAlignment.Bottom, new Thickness?(new Thickness(0.0, 0.0, right, 20.0)), 5, new Thickness?(), (System.Windows.Media.Brush) null, false, false);
            this.toastControl.AddImage("gamepad_disconnected", 19.0, 24.0, new Thickness?(new Thickness(0.0, 5.0, 10.0, 5.0)));
          }
          this.dummyToast.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
          this.dummyToast.VerticalAlignment = VerticalAlignment.Bottom;
          this.toastControl.Visibility = Visibility.Visible;
          this.toastPopup.IsOpen = true;
          this.toastCanvas.Width = this.toastControl.ActualWidth;
          this.toastCanvas.Height = this.toastControl.ActualHeight;
          this.toastPopup.VerticalOffset = -1.0 * this.toastControl.ActualHeight - 50.0;
          this.toastPopup.HorizontalOffset = -20.0;
          this.toastTimer.Start();
        }
        catch (Exception ex)
        {
          Logger.Error("Exception in showing toast popup for gamepad : " + ex.ToString());
        }
      }));
    }

    internal void ShowGeneralToast(string message)
    {
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        try
        {
          if (!this.mIsWindowInFocus)
            return;
          if (this.mGeneraltoast.IsOpen)
          {
            this.toastTimer.Stop();
            this.mGeneraltoast.IsOpen = false;
          }
          this.mGeneraltoastControl.Init((Window) this, message, (System.Windows.Media.Brush) System.Windows.Media.Brushes.Black, (System.Windows.Media.Brush) new SolidColorBrush(System.Windows.Media.Color.FromArgb((byte) 85, byte.MaxValue, byte.MaxValue, byte.MaxValue)), System.Windows.HorizontalAlignment.Center, VerticalAlignment.Bottom, new Thickness?(), 5, new Thickness?(), (System.Windows.Media.Brush) null, false, false);
          this.dummyToast.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
          this.dummyToast.VerticalAlignment = VerticalAlignment.Bottom;
          this.mGeneraltoastControl.Visibility = Visibility.Visible;
          this.mGeneraltoast.IsOpen = true;
          this.mGeneraltoastCanvas.Height = this.mGeneraltoastControl.ActualHeight;
          this.mGeneraltoast.VerticalOffset = -1.0 * this.mGeneraltoastControl.ActualHeight - 50.0;
          this.mGeneraltoast.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
          this.toastTimer.Start();
        }
        catch (Exception ex)
        {
          Logger.Error("Exception in showing general toast popup : " + ex.ToString());
        }
      }));
    }

    private void ToastTimer_Tick(object sender, EventArgs e)
    {
      this.toastTimer.Stop();
      this.toastPopup.IsOpen = false;
      this.mGeneraltoast.IsOpen = false;
    }

    internal void CloseFullScreenToastAndStopTimer()
    {
      this.mFullScreenToastTimer.Stop();
      this.mFullScreenToastPopup.IsOpen = false;
    }

    private void FullScreenToastTimer_Tick(object sender, EventArgs e)
    {
      this.CloseFullScreenToastAndStopTimer();
    }

    private event EventHandler CloseWindowConfirmationAcceptedHandler;

    private event EventHandler CloseWindowConfirmationResetAccountAcceptedHandler;

    public event MainWindow.GuestBootCompletedEventHandler GuestBootCompleted;

    internal event MainWindow.CursorLockChangedEventHandler CursorLockChangedEvent;

    internal event MainWindow.FullScreenChangedEventHandler FullScreenChanged;

    internal event MainWindow.FrontendGridVisibilityChangedEventHandler FrontendGridVisibilityChanged;

    private event EventHandler mEventOnAllWindowClosed;

    private event EventHandler mEventOnInstanceClosed;

    internal event EventHandler RestartEngineConfirmationAcceptedHandler;

    internal event EventHandler RestartPcConfirmationAcceptedHandler;

    internal event MainWindow.BrowserOTSCompletedCallbackEventHandler BrowserOTSCompletedCallback;

    internal Grid FirebaseBrowserControlGrid
    {
      get
      {
        if (this.mFirebaseBrowserControlGrid == null)
          this.mFirebaseBrowserControlGrid = this.AddBrowser(WebHelper.GetUrlWithParams(WebHelper.GetServerHost() + "/page/notification", (string) null, (string) null, (string) null));
        return this.mFirebaseBrowserControlGrid;
      }
    }

    internal ScreenLockControl ScreenLockInstance
    {
      get
      {
        if (this.mScreenLock == null)
          this.mScreenLock = new ScreenLockControl();
        return this.mScreenLock;
      }
    }

    private void GetMacroShortcutKeyMappingsWithRestrictedKeysandNames()
    {
      foreach (MacroRecording vertex in (Collection<BiDirectionalVertex<MacroRecording>>) MacroGraph.Instance.Vertices)
      {
        if (vertex.Shortcut.Length == 1 && !MainWindow.sMacroMapping.ContainsKey(vertex.Shortcut))
          MainWindow.sMacroMapping.Add(vertex.Shortcut, vertex.Name);
        if (vertex.PlayOnStart)
        {
          if (this.mAutoRunMacro == null)
          {
            this.mAutoRunMacro = vertex;
          }
          else
          {
            vertex.PlayOnStart = false;
            CommonHandlers.SaveMacroJson(vertex, vertex.Name + ".json");
          }
        }
      }
      HTTPUtils.SendRequestToEngineAsync("updateMacroShortcutsDict", MainWindow.sMacroMapping, this.mVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
    }

    internal MacroOverlay MacroOverlayControl
    {
      get
      {
        if (this.mMacroOverlay == null)
          this.mMacroOverlay = new MacroOverlay(this);
        return this.mMacroOverlay;
      }
    }

    internal InstanceRegistry EngineInstanceRegistry
    {
      get
      {
        return RegistryManager.Instance.Guest[this.mVmName];
      }
    }

    internal MacroRecorderWindow MacroRecorderWindow
    {
      get
      {
        if (this.mMacroRecorderWindow == null)
        {
          MacroRecorderWindow macroRecorderWindow = new MacroRecorderWindow(this);
          macroRecorderWindow.Owner = (Window) this;
          this.mMacroRecorderWindow = macroRecorderWindow;
        }
        return this.mMacroRecorderWindow;
      }
    }

    internal BlueStacksUIUtils Utils
    {
      get
      {
        if (this.mUtils == null)
          this.mUtils = new BlueStacksUIUtils(this);
        return this.mUtils;
      }
    }

    internal MainWindowsStaticComponents StaticComponents
    {
      get
      {
        if (this.mStaticComponents == null)
          this.mStaticComponents = new MainWindowsStaticComponents();
        return this.mStaticComponents;
      }
    }

    internal bool IsDefaultVM
    {
      get
      {
        return string.Equals(this.mVmName, BlueStacks.Common.Strings.CurrentDefaultVmName, StringComparison.InvariantCulture);
      }
    }

    internal Storyboard StoryBoard
    {
      get
      {
        if (this.mStoryBoard == null)
          this.mStoryBoard = this.FindResource((object) "mStoryBoard") as Storyboard;
        return this.mStoryBoard;
      }
    }

    public Discord mDiscordhandler { get; set; }

    internal bool SendClientActions
    {
      get
      {
        return this.mIsMacroRecorderActive || this.mIsSynchronisationActive;
      }
    }

    public static SettingsWindow SettingsWindow { get; set; }

    public bool IsInNotificationMode { get; set; }

    public string mPostBootNotificationAction { get; set; }

    internal bool IsMuted
    {
      get
      {
        return this.EngineInstanceRegistry.IsMuted || RegistryManager.Instance.AreAllInstancesMuted;
      }
    }

    private bool mIsWindowHidden { get; set; }

    public MainWindow(
      string vmName,
      FrontendHandler frontendHandler,
      string windowLaunchParams,
      bool hiddenMode = false)
    {
      Logger.Info("main window init");
      this.mVmName = vmName;
      this.WindowLaunchParams = windowLaunchParams;
      this.GetLockOfCurrentInstance();
      this.SetMultiInstanceEventWaitHandle();
      if (frontendHandler != null)
      {
        this.mFrontendHandler = frontendHandler;
        frontendHandler.ParentWindow = this;
      }
      this.mCommonHandler = new CommonHandlers(this);
      this.InitializeComponent();
      if (!Oem.IsOEMDmm)
      {
        this.mWelcomeTab.Init();
        this.mFrontendGrid.Visibility = Visibility.Visible;
      }
      else
      {
        this.ParentWindowHeightDiff = this.heightDiffScaled = 94;
        this.WelcomeTabParentGrid.Visibility = Visibility.Hidden;
        this.mWelcomeTab.Init();
        this.mWelcomeTab.Visibility = Visibility.Hidden;
        this.mWelcomeTab.mPromotionGrid.Visibility = Visibility.Hidden;
        this.mWelcomeTab.mPromotionControl.IsEnabled = false;
        this.FrontendParentGrid.Visibility = Visibility.Visible;
        this.mDmmProgressControl.Visibility = Visibility.Visible;
        this.mFrontendGrid.Visibility = Visibility.Hidden;
        this.mFrontendGrid.Margin = new Thickness(0.0, 0.0, 0.0, 2.0);
        this.mDmmBottomBar.Visibility = Visibility.Visible;
        this.mDMMFST = new DMMFullScreenTopBar();
        this.mDmmBottomBar.Init(this);
        this.mTopBarPopup.Child = (UIElement) this.mDMMFST;
        this.mDMMFST.Init(this);
        this.mDMMFST.MouseLeave += new System.Windows.Input.MouseEventHandler(this.TopBarPopup_MouseLeave);
      }
      this.mIsWindowHidden = hiddenMode;
      this.SizeChanged += new SizeChangedEventHandler(this.MainWindow_SizeChanged);
      this.LocationChanged += new EventHandler(this.MainWindow_LocationChanged);
      this.SetupInitialSize();
      this.SetWindowTitle(vmName);
      this.mResizeHandler = new WindowWndProcHandler(this);
      this.mExitProgressGrid.ProgressText = "STRING_CLOSING_BLUESTACKS";
      this.mAppHandler = new AppHandler(this);
      if (FeatureManager.Instance.IsCustomUIForNCSoft)
      {
        this.mTopBar.Visibility = Visibility.Collapsed;
        this.mNCTopBar.Visibility = Visibility.Visible;
      }
      if (this.EngineInstanceRegistry.IsClientOnTop)
        this.Topmost = true;
      this.mCommonHandler.InitShortcuts();
      if (!Oem.IsOEMDmm)
        this.mSidebar.InitElements();
      if (!string.IsNullOrEmpty(RegistryManager.Instance.Token))
        this.mIsTokenAvailable = true;
      if (this.IsDefaultVM && this.mAppHandler.IsOneTimeSetupCompleted)
        PromotionObject.PromotionHandler += new EventHandler(this.MainWindow_PromotionHandler);
      AppRequirementsParser.Instance.RequirementConfigUpdated += new EventHandler(this.MainWindow_RequirementConfigUpdated);
      this.PreviewKeyDown += new System.Windows.Input.KeyEventHandler(this.MainWindow_PreviewKeyDown);
      this.PreviewKeyUp += new System.Windows.Input.KeyEventHandler(this.MainWindow_PreviewKeyUp);
      RegistryManager.Instance.BossKey = this.mCommonHandler.GetShortcutKeyFromName("STRING_BOSSKEY_SETTING", true);
      try
      {
        if (!AppConfigurationManager.Instance.VmAppConfig.ContainsKey(this.mVmName))
          AppConfigurationManager.Instance.VmAppConfig[this.mVmName] = new Dictionary<string, AppSettings>();
      }
      catch (Exception ex)
      {
        Logger.Error("error {0}", (object) ex);
      }
      this.mIsWindowLoadedOnce = true;
    }

    private void GetLockOfCurrentInstance()
    {
      Logger.Debug("Getting lock of instance.." + this.mVmName);
      ProcessUtils.CheckAlreadyRunningAndTakeLock(BlueStacks.Common.Strings.GetClientInstanceLockName(this.mVmName, "bgp"), out this.mBlueStacksClientInstanceLock);
      if (this.mBlueStacksClientInstanceLock != null)
        return;
      Logger.Error("Client lock is not created for vmName: {0}", (object) this.mVmName);
    }

    private void SetMultiInstanceEventWaitHandle()
    {
      try
      {
        using (EventWaitHandle eventWaitHandle = EventWaitHandle.OpenExisting(BlueStacks.Common.Utils.GetMultiInstanceEventName(this.mVmName)))
          eventWaitHandle.Set();
      }
      catch (Exception ex)
      {
        Logger.Error("Error while setting event wait handle for vmName: {0} ex: {1}", (object) this.mVmName, (object) ex);
      }
    }

    private void MainWindow_RequirementConfigUpdated(object sender, EventArgs args)
    {
      GrmHandler.RequirementConfigUpdated(this.mVmName);
    }

    private void MainWindow_PromotionHandler(object sender, EventArgs e)
    {
      if (!this.IsDefaultVM || !this.mAppHandler.IsOneTimeSetupCompleted || this.mGuestBootCompleted)
        return;
      this.HandleFLEorAppPopupBeforeBoot();
    }

    private void SetTaskbarProperties()
    {
      this.Icon = (ImageSource) new BitmapImage(new Uri(System.IO.Path.Combine(RegistryStrings.InstallDir, "app_icon.ico")));
      this.Title = GameConfig.Instance.AppName;
    }

    internal void RestartFrontend()
    {
      this.mFrontendHandler.mEventOnFrontendClosed -= new EventHandler(this.FrontendHandler_StartFrontend);
      this.mFrontendHandler.mEventOnFrontendClosed += new EventHandler(this.FrontendHandler_StartFrontend);
      this.CloseFrontend();
    }

    private void FrontendHandler_StartFrontend(object sender, EventArgs e)
    {
      this.mFrontendHandler.StartFrontend();
      if (Oem.IsOEMDmm || this.Utils.IsRequiredFreeRAMAvailable() || this.IsFarmingInstance)
        return;
      this.mFrontendHandler.mIsSufficientRAMAvailable = false;
      this.Dispatcher.Invoke((Delegate) (() => this.mFrontendHandler.FrontendHandler_ShowLowRAMMessage()));
    }

    internal void CloseFrontend()
    {
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        this.ShowLoadingGrid(true);
        this.mTopBar.mAppTabButtons.GoToTab("Home", true, false);
        if (this.mWelcomeTab != null)
          this.mWelcomeTab.mFrontendPopupControl.HideWindow();
        if (this.mAppHandler != null)
        {
          this.mAppHandler.IsGuestReady = false;
          this.mAppHandler.mGuestReadyCheckStarted = false;
        }
        this.mFrontendHandler.mFrontendHandle = IntPtr.Zero;
      }));
      this.mFrontendHandler.KillFrontend(true);
    }

    internal void SwitchToPortraitMode(bool isSwitchForPortraitMode)
    {
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        try
        {
          if (this.WindowState == WindowState.Normal)
          {
            this.mPreviousWidth = new double?(this.Width);
            this.mPreviousHeight = new double?(this.Height);
          }
          bool flag = false;
          if (isSwitchForPortraitMode && this.WindowState != WindowState.Maximized)
          {
            if (isSwitchForPortraitMode != this.IsUIInPortraitMode)
            {
              flag = true;
              this.IsUIInPortraitMode = true;
              this.mTopBar.RefreshNotificationCentreButton();
              this.mTopBar.UpdateMacroRecordingProgress();
            }
          }
          else
          {
            if (isSwitchForPortraitMode && Oem.IsOEMDmm)
            {
              this.IsUIInPortraitMode = true;
              this.WindowState = WindowState.Normal;
              this.mIsMaximized = false;
              this.SetSizeForDMMPortraitMaximisedWindow();
              this.mTopBar.RefreshNotificationCentreButton();
              this.mTopBar.RefreshWarningButton();
              return;
            }
            if (isSwitchForPortraitMode != this.IsUIInPortraitMode)
            {
              flag = true;
              this.IsUIInPortraitMode = false;
              if (this.mIsDmmMaximised)
              {
                this.WindowState = WindowState.Maximized;
                this.mIsMaximized = true;
              }
              this.mTopBar.UpdateMacroRecordingProgress();
              this.mTopBar.RefreshNotificationCentreButton();
            }
          }
          if (this.WindowState == WindowState.Normal)
          {
            if (Oem.IsOEMDmm && this.mIsDmmMaximised && this.DmmRestoreWindowRectangle.Height != 0.0)
            {
              this.SetDMMSizeOnRestoreWindow();
            }
            else
            {
              this.ChangeHeightWidthAndPosition(this.GetWidthFromHeight((double) (int) this.Height, false, false), (double) (int) this.Height, flag || this.IsUIInPortraitMode ^ this.IsUIInPortraitModeWhenMaximized);
              if (this.IsUIInPortraitMode && Oem.IsOEMDmm && string.IsNullOrEmpty(this.EngineInstanceRegistry.WindowPlacement))
                this.DMMFirstLaunchPortraitModeAppHeightFix();
            }
          }
          this.mTopBar.RefreshWarningButton();
          this.UIChangesOnMainWindowSizeChanged();
          if (this.mStreamingModeEnabled)
            this.mFrontendHandler.ChangeFrontendToPortraitMode();
          if (StreamManager.Instance != null)
            StreamManager.Instance.OrientationChangeHandler();
          if (KMManager.sGuidanceWindow == null || this.mIsFullScreen)
            return;
          KMManager.sGuidanceWindow.ResizeGuidanceWindow();
        }
        catch (Exception ex)
        {
          this.SetupInitialSize();
          Logger.Info("Error occured setting size." + ex.ToString());
        }
      }));
    }

    private void SetDMMSizeOnRestoreWindow()
    {
      this.ChangeHeightWidthAndPosition(this.GetWidthFromHeight((double) (int) this.DmmRestoreWindowRectangle.Height, false, false), (double) (int) this.DmmRestoreWindowRectangle.Height, false);
      if (this.mIsDMMMaximizedFromPortrait != this.IsUIInPortraitMode)
      {
        if (this.IsUIInPortraitMode)
          this.Left = this.DmmRestoreWindowRectangle.Left + (this.DmmRestoreWindowRectangle.Width - this.Width) / 2.0;
        else
          this.Left = this.DmmRestoreWindowRectangle.Left - (this.Width - this.DmmRestoreWindowRectangle.Width) / 2.0;
      }
      else
        this.Left = this.DmmRestoreWindowRectangle.Left;
      this.Top = this.DmmRestoreWindowRectangle.Top;
    }

    private void DMMFirstLaunchPortraitModeAppHeightFix()
    {
      if (this.Width >= 310.0)
        return;
      this.Width = 310.0;
      this.ChangeHeightWidthAndPosition(this.Width, this.GetHeightFromWidth(this.Width, false, false), true);
      this.Top = (SystemParameters.MaximizedPrimaryScreenHeight - this.Height) / 2.0;
    }

    private void UIChangesOnMainWindowSizeChanged()
    {
      ++this.pikaPop.HorizontalOffset;
      --this.pikaPop.HorizontalOffset;
      ++this.toastPopup.HorizontalOffset;
      --this.toastPopup.HorizontalOffset;
      this.mCommonHandler.ClipMouseCursorHandler(false, false, "", "");
      this.SetMaxSizeOfWindow();
    }

    private void SetMaxSizeOfWindow()
    {
      System.Windows.Size andHeightOfMonitor = WindowPlacement.GetMaxWidthAndHeightOfMonitor(new WindowInteropHelper((Window) this).Handle);
      this.MaxHeightScaled = (int) andHeightOfMonitor.Height;
      this.MaxWidthScaled = (int) this.GetWidthFromHeight((double) this.MaxHeightScaled, false, false);
      if ((double) this.MaxWidthScaled <= andHeightOfMonitor.Width)
        return;
      this.MaxWidthScaled = (int) andHeightOfMonitor.Width;
      this.MaxHeightScaled = (int) this.GetHeightFromWidth((double) this.MaxWidthScaled, false, false);
    }

    private void ChangeHeightWidthAndPosition(double width, double height, bool changePosition)
    {
      try
      {
        this.Height = height;
        this.Width = width;
        if (Oem.IsOEMDmm && !this.mIsWindowResizedOnce)
        {
          this.Left = (SystemParameters.MaximizedPrimaryScreenWidth - this.Width - ((this.Height - (double) this.ParentWindowHeightDiff) * 9.0 / 16.0 + (double) this.ParentWindowWidthDiff)) / 2.0;
          this.Top = (SystemParameters.MaximizedPrimaryScreenHeight - this.Height) / 2.0;
          this.mIsWindowResizedOnce = true;
        }
        else if (changePosition)
        {
          if (this.IsUIInPortraitMode)
            this.Left += (this.mPreviousWidth.Value - this.Width) / 2.0;
          else
            this.Left -= (this.Width - this.mPreviousWidth.Value) / 2.0;
        }
        this.mPreviousWidth = new double?(this.Width);
        this.mPreviousHeight = new double?(this.Height);
      }
      catch (Exception ex)
      {
        Logger.Info("Error occured setting size." + ex.ToString());
      }
    }

    internal void ChangeHeightWidthTopLeft(double width, double height, double top, double left)
    {
      try
      {
        if (this.WindowState == WindowState.Maximized)
          this.RestoreWindows(false);
        this.Height = height / MainWindow.sScalingFactor;
        this.Width = width / MainWindow.sScalingFactor;
        this.Top = top / MainWindow.sScalingFactor;
        this.Left = left / MainWindow.sScalingFactor;
        this.mSidebar?.ArrangeAllSidebarElements();
      }
      catch (Exception ex)
      {
        Logger.Error("Error occured setting size of the window. err:" + ex.ToString());
      }
    }

    private void SetWindowTitle(string vmName)
    {
      this.Title = BlueStacks.Common.Utils.GetDisplayName(vmName, "bgp");
      this.mTopBar.mTitleText.Text = this.Title;
      int num = FeatureManager.Instance.IsCustomUIForNCSoft ? 1 : 0;
      if (RegistryManager.Instance.InstallationType != InstallationTypes.GamingEdition)
        return;
      this.mTopBar.mTitleText.Text = GameConfig.Instance.AppName;
      this.mTopBar.mTitleIcon.ImageName = System.IO.Path.Combine(RegistryStrings.InstallDir, "app_icon.ico");
      this.SetTaskbarProperties();
    }

    internal void ShowRerollOverlay()
    {
      this.ShowDimOverlay((IDimOverlayControl) this.MacroOverlayControl);
    }

    internal void HandleGenericNotificationPopup(GenericNotificationItem notifItem)
    {
      GenericNotificationDesignItem designItem = notifItem.NotificationDesignItem;
      if (!RegistryManager.Instance.IsShowRibbonNotification || RegistryManager.Instance.InstallationType == InstallationTypes.GamingEdition)
        return;
      this.pikaNotificationWorkQueue.Enqueue((SerialWorkQueue.Work) (() =>
      {
        while (!this.mIsWindowInFocus || this.isPikaPopOpen)
          Thread.Sleep(2000);
        this.Dispatcher.Invoke((Delegate) (() =>
        {
          this.isPikaPopOpen = true;
          this.pikaPopControl.Init(notifItem);
          Canvas.SetLeft((UIElement) this.pikaPopControl, 0.0);
          this.pikaPop.IsOpen = true;
          Storyboard storyboard = new Storyboard();
          this.pikaCanvas.Width = this.pikaPopControl.ActualWidth;
          this.pikaPop.HorizontalOffset = this.pikaPopControl.ActualWidth * -0.5;
          PennerDoubleAnimation.Equations type = PennerDoubleAnimation.Equations.QuadEaseInOut;
          double actualWidth = this.pikaPopControl.ActualWidth;
          double to = 0.0;
          int durationMS = 700;
          Animator.AnimatePenner((DependencyObject) this.pikaPopControl, Canvas.LeftProperty, type, new double?(actualWidth), to, durationMS, (EventHandler) null);
          string str = "Home";
          if (this.mTopBar.mAppTabButtons.SelectedTab != null)
            str = this.mTopBar.mAppTabButtons.SelectedTab.AppLabel;
          ClientStats.SendMiscellaneousStatsAsync("RibbonShown", RegistryManager.Instance.UserGuid, JsonConvert.SerializeObject((object) notifItem.ExtraPayload), str, RegistryManager.Instance.ClientVersion, Oem.Instance.OEM, notifItem.Id, notifItem.Title, (string) null, "Android");
        }));
        this.pikaNotificationTimer.Interval = TimeSpan.FromMilliseconds(designItem.AutoHideTime);
        this.pikaNotificationTimer.Start();
      }));
    }

    private void PikaNotificationTimer_Tick(object sender, EventArgs e)
    {
      this.pikaNotificationTimer.Stop();
      if (!this.isPikaPopOpen)
        return;
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        PennerDoubleAnimation.Equations type = PennerDoubleAnimation.Equations.QuadEaseInOut;
        double num = 0.0;
        double actualWidth = this.pikaPopControl.ActualWidth;
        int durationMS = 400;
        Animator.AnimatePenner((DependencyObject) this.pikaPopControl, Canvas.LeftProperty, type, new double?(num), actualWidth, durationMS, (EventHandler) ((s, ev) =>
        {
          this.pikaPop.IsOpen = false;
          this.isPikaPopOpen = false;
        }));
      }));
    }

    internal void ShowDimOverlay(IDimOverlayControl el = null)
    {
      this.Dispatcher.Invoke((Delegate) (() => this.ShowDimOverlayUIThread(el)));
    }

    private void ShowDimOverlayUIThread(IDimOverlayControl el = null)
    {
      try
      {
        Logger.Debug("showing dim overlay");
        if (this.mDimOverlay == null || this.mDimOverlay.IsClosed)
          this.mDimOverlay = new DimOverlayControl(this);
        if (PresentationSource.FromVisual((Visual) this) == null)
          return;
        this.mDimOverlay.Owner = this;
        this.mDimOverlay.Control = el;
        this.mDimOverlay.UpadteSizeLocation();
        this.mDimOverlay.ShowWindow();
        this.mFrontendHandler.ShowGLWindow();
      }
      catch (Exception ex)
      {
        Logger.Error("Exception while showing dimoverlay control. " + ex.ToString());
      }
    }

    internal void HideDimOverlay()
    {
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        try
        {
          Logger.Debug("Hide dim overlay");
          if (this.mDimOverlay != null)
          {
            if (this.mIsLockScreenActionPending)
            {
              this.ShowDimOverlay((IDimOverlayControl) this.ScreenLockInstance);
            }
            else
            {
              this.mDimOverlay.HideWindow(false);
              this.mDimOverlay.Control = (IDimOverlayControl) null;
              this.mFrontendHandler.ShowGLWindow();
            }
          }
        }
        catch (Exception ex)
        {
        }
        this.Focus();
      }));
    }

    private void MainWindow_IsVisibleChanged(object _1, DependencyPropertyChangedEventArgs _2)
    {
      foreach (Window ownedWindow in this.OwnedWindows)
      {
        if (ownedWindow != null)
        {
          try
          {
            CustomWindow customWindow = (CustomWindow) ownedWindow;
            if (customWindow != null)
            {
              if (!customWindow.ShowWithParentWindow)
                continue;
            }
            ownedWindow.Visibility = this.Visibility;
          }
          catch (Exception ex)
          {
            Logger.Error("Exception in showing child windows: {0}", (object) ex.ToString());
          }
        }
      }
    }

    public void MainWindow_StateChanged(object sender, EventArgs e)
    {
      if (this.WindowState != WindowState.Minimized)
      {
        this.SendTempGamepadState(true);
        try
        {
          this.Icon = (ImageSource) BitmapFrame.Create(new Uri(RegistryStrings.ProductIconCompletePath));
          SerializableDictionary<string, GenericNotificationItem> notificationItems = GenericNotificationManager.GetNotificationItems((Predicate<GenericNotificationItem>) (x =>
          {
            if (x.IsDeleted || x.IsRead)
              return false;
            return string.Equals(x.VmName, this.mVmName, StringComparison.InvariantCulture) || !x.IsAndroidNotification;
          }));
          notificationItems.Count.ToString((IFormatProvider) CultureInfo.InvariantCulture);
          if (this.IsInNotificationMode)
          {
            foreach (string key in this.AppNotificationCountDictForEachVM.Keys)
              BlueStacks.Common.Stats.SendCommonClientStatsAsync("notification_mode", "notification_number", this.mVmName, key, this.AppNotificationCountDictForEachVM[key].ToString((IFormatProvider) CultureInfo.InvariantCulture), "NM_On", "");
            this.AppNotificationCountDictForEachVM.Clear();
            this.DummyWindow?.Close();
            HTTPUtils.SendRequestToAgentAsync("overrideDesktopNotificationSettings", new Dictionary<string, string>()
            {
              {
                "override",
                "False"
              }
            }, this.mVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
            this.mIsMinimizedThroughCloseButton = false;
            if (notificationItems.Count > 0 && MainWindow.sShowNotifications)
              new Thread((ThreadStart) (() => this.Dispatcher.Invoke((Delegate) (() =>
              {
                BlueStacksUIBinding.BindColor((DependencyObject) this.mTopBar.mNotificationCaret, Shape.FillProperty, "SliderButtonColor");
                BlueStacksUIBinding.BindColor((DependencyObject) this.mTopBar.mNotificationCaret, Shape.StrokeProperty, "SliderButtonColor");
                BlueStacksUIBinding.BindColor((DependencyObject) this.mTopBar.mNotificationCentreDropDownBorder, System.Windows.Controls.Control.BorderBrushProperty, "SliderButtonColor");
                this.mTopBar.mNotificationDrawerControl.mAnimationRect.Visibility = Visibility.Visible;
                this.mTopBar.mNotificationCentreButton_MouseLeftButtonUp((object) null, (MouseButtonEventArgs) null);
              }))))
              {
                IsBackground = true
              }.Start();
          }
          MainWindow.sShowNotifications = true;
          this.IsInNotificationMode = false;
          Logger.Info("Notification mode : Off");
        }
        catch (Exception ex)
        {
          Logger.Error("Error in setting window's icon: " + ex?.ToString());
        }
      }
      else
      {
        Logger.Debug("KMP MainWindow_StateChanged " + this.mVmName);
        if (BlueStacksUIUtils.ActivatedWindow == this)
          BlueStacksUIUtils.ActivatedWindow = (MainWindow) null;
        AppUsageTimer.StopTimer();
        this.mFrontendHandler.DeactivateFrontend();
        this.mCommonHandler.ClipMouseCursorHandler(true, true, "", "");
        this.mIsWindowInFocus = false;
        if (!this.IsInNotificationMode)
          BlueStacksUIUtils.SetWindowTaskbarIcon(this);
      }
      BlueStacksUIUtils.LastActivatedWindow.mFrontendHandler.UpdateOverlaySizeStatus();
      this.OnResizeMainWindow();
    }

    internal void SendTempGamepadState(bool enable)
    {
      if (!RegistryManager.Instance.GamepadDetectionEnabled)
        return;
      if (enable)
      {
        if (this.IsGamepadConnected)
          return;
        if (this.WasGamepadStatusSkipped)
        {
          this.SkipNextGamepadStatus = true;
          this.WasGamepadStatusSkipped = false;
        }
        this.mFrontendHandler.SendFrontendRequestAsync("enableGamepad", new Dictionary<string, string>()
        {
          {
            nameof (enable),
            "true"
          }
        });
      }
      else
      {
        this.SkipNextGamepadStatus = true;
        this.mFrontendHandler.SendFrontendRequestAsync("enableGamepad", new Dictionary<string, string>()
        {
          {
            nameof (enable),
            "false"
          }
        });
      }
    }

    private void MainWindow_Deactivated(object sender, EventArgs e)
    {
      Logger.Debug("KMP MainWindow_Deactivated " + this.mVmName);
      if (BlueStacksUIUtils.ActivatedWindow == this)
        BlueStacksUIUtils.ActivatedWindow = (MainWindow) null;
      this.ClosePopUps();
      this.mFrontendHandler.DeactivateFrontend();
      this.mCommonHandler.ClipMouseCursorHandler(true, true, "", "");
      this.mIsWindowInFocus = false;
    }

    private void MainWindow_Activated(object sender, EventArgs e)
    {
      Logger.Debug("In MainWindow_Activated");
      BlueStacksUIUtils.LastActivatedWindow = this;
      BlueStacksUIUtils.ActivatedWindow = this;
      App.IsApplicationActive = true;
      this.mIsWindowInFocus = true;
      if (!string.IsNullOrEmpty(this.mVmName) && this.mTopBar != null && (this.mTopBar.mAppTabButtons != null && this.mTopBar.mAppTabButtons.SelectedTab != null) && !string.IsNullOrEmpty(this.mTopBar.mAppTabButtons.SelectedTab.TabKey))
        AppUsageTimer.StartTimer(this.mVmName, this.mTopBar.mAppTabButtons.SelectedTab.TabKey);
      if (this.mFrontendGrid.IsVisible)
      {
        Logger.Debug("KMP MainWindow_Activated focusfrontend " + this.mVmName);
        if (!this.mTopBar.mAppTabButtons.SelectedTab.mGuidanceWindowOpen || this.mTopBar.mAppTabButtons.SelectedTab.mShootingModeToastWhenGuidanceOpen)
          KMManager.ShowShootingModeTooltip(this, this.mTopBar.mAppTabButtons.SelectedTab.PackageName);
        else
          this.mTopBar.mAppTabButtons.SelectedTab.mShootingModeToastWhenGuidanceOpen = true;
      }
      else
      {
        Logger.Debug("KMP MainWindow_Activated DeactivateFrontend " + this.mVmName);
        this.mFrontendHandler.DeactivateFrontend();
      }
      this.SendTempGamepadState(true);
      if (!this.mSidebar.IsEcoModeEnabledFromMIM)
        return;
      this.mSidebar.ShowEcoModeOnboardingBlurb();
    }

    private void MainWindow_SourceInitialized(object sender, EventArgs e)
    {
      this.Handle = ((HwndSource) PresentationSource.FromVisual((Visual) this)).Handle;
    }

    internal void MainWindow_ResizeBegin(object sender, EventArgs e)
    {
      this.mIsResizing = true;
    }

    private void OnResizeMainWindow()
    {
      this.mSidebar?.SetHeight();
      System.Action<bool> recommendationHandler = PromotionObject.AppRecommendationHandler;
      if (recommendationHandler != null)
        recommendationHandler(false);
      ++this.mShootingModePopup.HorizontalOffset;
      --this.mShootingModePopup.HorizontalOffset;
      ++this.mFullScreenToastPopup.HorizontalOffset;
      --this.mFullScreenToastPopup.HorizontalOffset;
      if (this.mUnlockMouseToastPopup.IsOpen)
        this.mUnlockMouseToastControl.mToastPanel.MaxWidth = this.ActualWidth - 180.0;
      ++this.mUnlockMouseToastPopup.HorizontalOffset;
      --this.mUnlockMouseToastPopup.HorizontalOffset;
      ++this.mConfigUpdatedPopup.HorizontalOffset;
      --this.mConfigUpdatedPopup.HorizontalOffset;
      ++this.mImageUploadedPopup.HorizontalOffset;
      --this.mImageUploadedPopup.HorizontalOffset;
      ++this.mGeneraltoast.HorizontalOffset;
      --this.mGeneraltoast.HorizontalOffset;
      this.mSidebar.mMoreElements.IsOpen = false;
    }

    internal void MainWindow_ResizeEnd(object sender, EventArgs e)
    {
      this.mIsResizing = false;
      if (this.WindowState == WindowState.Normal)
      {
        try
        {
          this.EngineInstanceRegistry.WindowPlacement = this.GetPlacement();
        }
        catch (Exception ex)
        {
          Logger.Error("Exception in MainWindow_ResizeEnd. Exception: " + ex.ToString());
        }
      }
      this.UIChangesOnMainWindowSizeChanged();
      this.mFrontendHandler.ShowGLWindow();
    }

    internal void ChangeWindowOrientaion(object sender, ChangeOrientationEventArgs e)
    {
      this.SwitchToPortraitMode(e.IsPotrait);
    }

    private void SetupInitialSize()
    {
      this.mAspectRatio = new Fraction((long) this.EngineInstanceRegistry.GuestWidth, (long) this.EngineInstanceRegistry.GuestHeight);
      this.mPreviousWidth = new double?(this.Width);
      this.mPreviousHeight = new double?(this.Height);
      this.ChangeHeightWidthAndPosition(this.GetWidthFromHeight(BlueStacksUIUtils.GetDefaultHeight(), false, false), BlueStacksUIUtils.GetDefaultHeight(), true);
    }

    internal void ChangeOrientationFromClient(bool isPortrait, bool stopFurtherOrientation = true)
    {
      new Thread((ThreadStart) (() =>
      {
        if (!BlueStacks.Common.Utils.IsGuestBooted(this.mVmName, "bgp"))
          return;
        this.SwitchOrientationFromClient(isPortrait, stopFurtherOrientation);
        this.SendOrientationChangeToAndroid(isPortrait);
      }))
      {
        IsBackground = true
      }.Start();
    }

    private void SendOrientationChangeToAndroid(bool isPortrait)
    {
      try
      {
        HTTPUtils.SendRequestToGuest("guestorientation", new Dictionary<string, string>()
        {
          {
            "d",
            isPortrait ? "1" : "0"
          }
        }, this.mVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in sending GuestOrientation to android: " + ex.ToString());
      }
    }

    private void SwitchOrientationFromClient(bool orientation, bool stopFurtherOrientation)
    {
      try
      {
        BstHttpClient.Get(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1}", (object) "http://127.0.0.1", (object) RegistryManager.Instance.Guest[this.mVmName].FrontendServerPort), (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}?{1}", (object) "switchOrientation", (object) ("orientation=" + (orientation ? "1" : "0") + "&package=" + this.mTopBar.mAppTabButtons.SelectedTab.PackageName + "&stopFurtherOrientationChange=" + (stopFurtherOrientation ? "1" : "0") + "&isPreviousSelectedTabWeb=" + (this.StaticComponents.mPreviousSelectedTabWeb ? "1" : "0")))), (Dictionary<string, string>) null, false, this.mVmName, 0, 1, 0, false, "bgp");
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in sending switch orientation from client: " + ex.ToString());
      }
    }

    internal void HandleDisplaySettingsChanged()
    {
      try
      {
        if (PresentationSource.FromVisual((Visual) this) != null)
          MainWindow.sScalingFactor = PresentationSource.FromVisual((Visual) this).CompositionTarget.TransformToDevice.M11;
        this.MinWidthScaled = (int) (this.MinWidth * MainWindow.sScalingFactor);
        this.MinHeightScaled = (int) (this.MinHeight * MainWindow.sScalingFactor);
        this.heightDiffScaled = (int) ((double) this.ParentWindowHeightDiff * MainWindow.sScalingFactor);
        this.widthDiffScaled = (int) ((double) this.ParentWindowWidthDiff * MainWindow.sScalingFactor);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in HandleDisplaySettingsChanged. Exception: " + ex.ToString());
      }
    }

    internal void ShowWindow(bool updateBootStartTime = false)
    {
      if (!this.mIsWindowLoadedOnce)
        return;
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        if (Oem.IsOEMDmm)
          KMManager.ShowOverlayWindow(this, false, false);
        this.ShowInTaskbar = true;
        this.Visibility = Visibility.Visible;
        this.Show();
        this.BringIntoView();
        if (this.WindowState == WindowState.Minimized)
          InteropWindow.ShowWindow(this.Handle, 9);
        if (Oem.IsOEMDmm && this.mDMMRecommendedWindow == null)
        {
          this.mDMMRecommendedWindow = new DMMRecommendedWindow(this);
          this.mDMMRecommendedWindow.Init(RegistryManager.Instance.DMMRecommendedWindowUrl);
          this.mDMMRecommendedWindow.Visibility = Visibility.Visible;
        }
        if (!this.Topmost)
        {
          this.Topmost = true;
          ThreadPool.QueueUserWorkItem((WaitCallback) (obj => this.Dispatcher.Invoke((Delegate) (() => this.Topmost = false))));
        }
        if (updateBootStartTime)
          this.mBootStartTime = DateTime.Now;
        this.mIsWindowHidden = false;
      }));
    }

    internal double GetWidthFromHeight(double height, bool isScaled = false, bool isIgnoreMinWidth = false)
    {
      if (this.IsUIInPortraitMode)
      {
        if (isScaled)
        {
          try
          {
            return Math.Max((height - (double) this.heightDiffScaled) / this.mAspectRatio.DoubleValue + (double) this.widthDiffScaled, isIgnoreMinWidth ? 0.0 : (double) this.MinWidthScaled);
          }
          catch
          {
          }
        }
        return Math.Max((height - (double) this.ParentWindowHeightDiff) / this.mAspectRatio.DoubleValue + (double) this.ParentWindowWidthDiff, isIgnoreMinWidth ? 0.0 : this.MinWidth);
      }
      if (isScaled)
      {
        try
        {
          return Math.Max((height - (double) this.heightDiffScaled) * this.mAspectRatio.DoubleValue + (double) this.widthDiffScaled, isIgnoreMinWidth ? 0.0 : (double) this.MinWidthScaled);
        }
        catch
        {
        }
      }
      return Math.Max((height - (double) this.ParentWindowHeightDiff) * this.mAspectRatio.DoubleValue + (double) this.ParentWindowWidthDiff, isIgnoreMinWidth ? 0.0 : this.MinWidth);
    }

    internal double GetHeightFromWidth(double width, bool isScaled = false, bool isIgnoreMinWidth = false)
    {
      if (this.IsUIInPortraitMode)
      {
        if (isScaled)
        {
          try
          {
            return Math.Max((width - (double) this.widthDiffScaled) * this.mAspectRatio.DoubleValue + (double) this.heightDiffScaled, isIgnoreMinWidth ? 0.0 : (double) this.MinHeightScaled);
          }
          catch
          {
          }
        }
        return Math.Max((width - (double) this.ParentWindowWidthDiff) * this.mAspectRatio.DoubleValue + (double) this.ParentWindowHeightDiff, isIgnoreMinWidth ? 0.0 : this.MinHeight);
      }
      if (isScaled)
      {
        try
        {
          return Math.Max((width - (double) this.widthDiffScaled) / this.mAspectRatio.DoubleValue + (double) this.heightDiffScaled, isIgnoreMinWidth ? 0.0 : (double) this.MinHeightScaled);
        }
        catch
        {
        }
      }
      return Math.Max((width - (double) this.ParentWindowWidthDiff) / this.mAspectRatio.DoubleValue + (double) this.ParentWindowHeightDiff, isIgnoreMinWidth ? 0.0 : this.MinHeight);
    }

    private void MainWindow_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
    {
      if (this.mIsResizing)
        return;
      this.Cursor = System.Windows.Input.Cursors.Arrow;
    }

    private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      this.mDMMRecommendedWindow?.UpdateSize();
      this.OnResizeMainWindow();
      KMManager.sGuidanceWindow?.UpdateSize();
      this.mTopBar.ClosePopups();
    }

    private void MainWindow_LocationChanged(object sender, EventArgs e)
    {
      this.mDMMRecommendedWindow?.UpdateLocation();
      this.OnResizeMainWindow();
      KMManager.sGuidanceWindow?.UpdateSize();
    }

    internal void RestartInstanceAndPerform(EventHandler action = null, bool isWaitForPlayerClosing = false)
    {
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        if (action != null)
        {
          this.mFrontendHandler.mEventOnFrontendClosed -= action;
          this.mFrontendHandler.mEventOnFrontendClosed += action;
        }
        this.mFrontendHandler.mEventOnFrontendClosed -= new EventHandler(this.FrontendHandler_RunInstance);
        this.mFrontendHandler.mEventOnFrontendClosed += new EventHandler(this.FrontendHandler_RunInstance);
        this.CloseCurrentInstanceForRestart(isWaitForPlayerClosing);
      }));
    }

    internal void CloseAllWindowAndPerform(EventHandler action = null)
    {
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        if (action != null)
        {
          this.mEventOnAllWindowClosed -= action;
          this.mEventOnAllWindowClosed += action;
        }
        this.ForceCloseWindow(true);
      }));
    }

    internal void FrontendHandler_RunInstance(object sender, EventArgs e)
    {
      this.CloseMainWindow();
      BlueStacksUIUtils.RunInstance(this.mVmName, false, "");
    }

    internal void CloseMainWindow()
    {
      HTTPUtils.SendRequestToAgentAsync("instanceStopped", (Dictionary<string, string>) null, this.mVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        if (BlueStacksUIUtils.DictWindows.Keys.Count == 1)
          Publisher.PublishMessage(BrowserControlTags.appPlayerClosing, BlueStacksUIUtils.DictWindows.First<KeyValuePair<string, MainWindow>>().Key, (JObject) null);
        this.mGuestBootCompleted = false;
        this.mClosing = true;
        this.Close();
      }));
    }

    internal void CloseCurrentInstanceForRestart(bool isWaitForPlayerClosing = false)
    {
      this.mIsRestart = true;
      this.ForceCloseWindow(isWaitForPlayerClosing);
    }

    internal void ForceCloseWindow(bool isWaitForPlayerClosing = false)
    {
      try
      {
        this.CloseWindowHandler(isWaitForPlayerClosing);
      }
      catch (Exception ex)
      {
        Logger.Error("Error occured in ForceClose" + ex.ToString());
      }
    }

    internal void CloseWindow()
    {
      if (this.IsClosed)
        return;
      if (Oem.Instance.IsRemoveAccountOnExit)
      {
        CustomMessageWindow customMessageWindow = new CustomMessageWindow();
        BlueStacksUIBinding.Bind(customMessageWindow.TitleTextBlock, "STRING_CLOSING_BLUESTACKS", "");
        BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlock, "STRING_REMOVE_ACCOUNT_ON_EXIT", "");
        customMessageWindow.AddButton(ButtonColors.Red, "STRING_REMOVE_CLOSE", this.CloseWindowConfirmationResetAccountAcceptedHandler, (string) null, false, (object) null, true);
        customMessageWindow.AddButton(ButtonColors.White, "STRING_CLOSE", this.CloseWindowConfirmationAcceptedHandler, (string) null, false, (object) null, true);
        this.ShowDimOverlay((IDimOverlayControl) null);
        customMessageWindow.Owner = (Window) this.mDimOverlay;
        customMessageWindow.ShowDialog();
        this.HideDimOverlay();
      }
      else
      {
        ProgressBar progressBar = new ProgressBar();
        progressBar.ProgressText = "STRING_LOADING_MESSAGE";
        progressBar.Visibility = Visibility.Hidden;
        this.ShowDimOverlay((IDimOverlayControl) progressBar);
        new Thread((ThreadStart) (() =>
        {
          if (FeatureManager.Instance.IsCheckForQuitPopup && !Oem.IsOEMDmm && (this.Utils.CheckQuitPopupFromCloud("") || this.Utils.CheckQuitPopupLocal()))
            return;
          this.Dispatcher.Invoke((Delegate) (() =>
          {
            this.HideDimOverlay();
            if (FeatureManager.Instance.IsShowAdvanceExitOption)
            {
              if (RegistryManager.Instance.IsQuitOptionSaved)
              {
                this.BlueStacksAdvancedExitAcceptedHandler((object) null, (MouseButtonEventArgs) null);
              }
              else
              {
                BlueStacksAdvancedExit stacksAdvancedExit = new BlueStacksAdvancedExit(this);
                stacksAdvancedExit.YesButton.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.BlueStacksAdvancedExitAcceptedHandler);
                stacksAdvancedExit.NoButton.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.BlueStacksAdvancedExitDeclinedHandler);
                stacksAdvancedExit.CrossButton.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.BlueStacksAdvancedExitDeclinedHandler);
                ContainerWindow containerWindow = new ContainerWindow(this, (System.Windows.Controls.UserControl) stacksAdvancedExit, 440.0, 400.0, false, true, false, -1.0, (System.Windows.Media.Brush) null);
              }
            }
            else
            {
              CustomMessageWindow customMessageWindow = new CustomMessageWindow();
              if (BlueStacksUIUtils.DictWindows.Where<KeyValuePair<string, MainWindow>>((Func<KeyValuePair<string, MainWindow>, bool>) (_ => !_.Value.IsClosed)).Count<KeyValuePair<string, MainWindow>>() == 1)
              {
                BlueStacksUIBinding.Bind(customMessageWindow.TitleTextBlock, "STRING_CLOSE_BLUESTACKS", "");
                BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlock, "STRING_EXIT_BLUESTACKS", "");
                if (Oem.IsOEMDmm)
                  customMessageWindow.ContentMaxWidth = 400.0;
              }
              else
              {
                BlueStacksUIBinding.Bind(customMessageWindow.TitleTextBlock, "STRING_INSTANCE_CLOSE_TITLE", "");
                BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlock, "STRING_EXIT_INSTANCE", "");
                if (Oem.IsOEMDmm)
                  customMessageWindow.ContentMaxWidth = 400.0;
              }
              customMessageWindow.AddButton(ButtonColors.Red, "STRING_CLOSE", this.CloseWindowConfirmationAcceptedHandler, (string) null, false, (object) null, true);
              customMessageWindow.AddButton(ButtonColors.White, "STRING_CANCEL", new EventHandler(this.CloseWindowConfirmationDeniedHandler), (string) null, false, (object) null, true);
              this.ShowDimOverlayUIThread((IDimOverlayControl) null);
              customMessageWindow.Owner = (Window) this.mDimOverlay;
              customMessageWindow.ShowDialog();
              if (this.mDimOverlay == null || this.mDimOverlay.OwnedWindows.OfType<ContainerWindow>().Any<ContainerWindow>())
                return;
              this.HideDimOverlay();
            }
          }));
        }))
        {
          IsBackground = true
        }.Start();
      }
    }

    private void BlueStacksAdvancedExitAcceptedHandler(object sender, MouseButtonEventArgs e)
    {
      switch (RegistryManager.Instance.QuitDefaultOption)
      {
        case "STRING_CLOSE_ALL_RUNNING_INSTANCES":
          BlueStacks.Common.Utils.StopClientInstanceAsync("");
          break;
        case "STRING_RESTART_CURRENT_INSTANCE":
          BlueStacksUIUtils.RestartInstance(this.mVmName, false);
          break;
        default:
          this.CloseWindowHandler(false);
          break;
      }
    }

    private void BlueStacksAdvancedExitDeclinedHandler(object sender, MouseButtonEventArgs e)
    {
      if (RegistryManager.Instance.InstallationType != InstallationTypes.GamingEdition || !this.mGuestBootCompleted)
        return;
      this.mTopBar.mAppTabButtons.AddHiddenAppTabAndLaunch(GameConfig.Instance.PkgName, GameConfig.Instance.ActivityName);
    }

    private void CloseWindowConfirmationDeniedHandler(object sender, EventArgs e)
    {
      if (RegistryManager.Instance.InstallationType != InstallationTypes.GamingEdition || !this.mGuestBootCompleted)
        return;
      this.mTopBar.mAppTabButtons.AddHiddenAppTabAndLaunch(GameConfig.Instance.PkgName, GameConfig.Instance.ActivityName);
    }

    internal void MainWindow_CloseWindowConfirmationAcceptedHandler(object sender, EventArgs e)
    {
      this.CloseWindowHandler(false);
    }

    private void MainWindow_CloseWindowConfirmationResetAccountAcceptedHandler(
      object sender,
      EventArgs e)
    {
      if (this.Visibility == Visibility.Visible)
      {
        this.mFrontendGrid.Visibility = Visibility.Hidden;
        this.mExitProgressGrid.ProgressText = !this.mIsRestart ? "STRING_CLOSING_BLUESTACKS" : "STRING_RESTARTING";
        this.mExitProgressGrid.Visibility = Visibility.Visible;
      }
      this.mAppHandler.SendRequestToRemoveAccountAndCloseWindowASync(true);
    }

    internal void ShowDimOverlayForUpgrade()
    {
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        this.CloseChildOwnedWindows();
        this.GotoHomeTab();
        this.mWelcomeTab.mFrontendPopupControl.HideWindow();
        this.mExitProgressGrid.ProgressText = "STRING_UPGRADING_TEXT";
        this.mExitProgressGrid.Visibility = Visibility.Visible;
        this.HideDimOverlay();
      }));
    }

    private void CloseChildOwnedWindows()
    {
      foreach (Window ownedWindow1 in this.OwnedWindows)
      {
        if (ownedWindow1 != null)
        {
          foreach (Window ownedWindow2 in ownedWindow1.OwnedWindows)
            ownedWindow2?.Close();
          ownedWindow1.Close();
        }
      }
    }

    private void GotoHomeTab()
    {
      if (Oem.IsOEMDmm || this.mTopBar.mAppTabButtons.GoToTab("Home", true, false))
        return;
      Logger.Info("Test logs: GotoHomeTab()");
      this.mTopBar.mAppTabButtons.AddHomeTab();
      this.mTopBar.mAppTabButtons.CloseTab("Setup", false, true, false, false, "");
    }

    internal void MinimizeWindowHandler()
    {
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        try
        {
          this.IsInNotificationMode = true;
          this.mIsMinimizedThroughCloseButton = true;
          HTTPUtils.SendRequestToAgentAsync("overrideDesktopNotificationSettings", new Dictionary<string, string>()
          {
            {
              "override",
              "True"
            }
          }, this.mVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
          Logger.Info("Notification mode : On");
          Dictionary<string, AppTabButton> dictionary = new Dictionary<string, AppTabButton>();
          foreach (KeyValuePair<string, AppTabButton> mDictTab in this.mTopBar.mAppTabButtons.mDictTabs)
            dictionary.Add(mDictTab.Key, mDictTab.Value);
          foreach (KeyValuePair<string, AppTabButton> keyValuePair in dictionary)
            this.mTopBar.mAppTabButtons.CloseTabAfterQuitPopup(keyValuePair.Key, true, false);
          BlueStacksUIUtils.HideUnhideParentWindow(true, this);
        }
        catch (Exception ex)
        {
          Logger.Error("Error occured in MinimizeWindowHandler " + ex.ToString());
        }
      }));
    }

    internal void CloseWindowHandler(bool isWaitForPlayerClosing = false)
    {
      if (this.IsClosed)
        return;
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        try
        {
          if (this.IsClosed)
            return;
          HTTPUtils.SendRequestToAgentAsync("notificationStatsOnClosing", (Dictionary<string, string>) null, this.mVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
          this.CloseChildOwnedWindows();
          if (CommonHandlers.sIsRecordingVideo && string.Equals(CommonHandlers.sRecordingInstance, this.mVmName, StringComparison.InvariantCulture))
            this.mCommonHandler.StopRecordVideo();
          this.GotoHomeTab();
          if (RegistryManager.Instance.InstallationType == InstallationTypes.GamingEdition)
            this.mWelcomeTab.mBackground.Visibility = Visibility.Visible;
          this.mWelcomeTab.mFrontendPopupControl.HideWindow();
          this.mExitProgressGrid.ProgressText = !this.mIsRestart ? "STRING_CLOSING_BLUESTACKS" : "STRING_RESTARTING";
          this.mExitProgressGrid.Visibility = Visibility.Visible;
          this.HideDimOverlay();
          if (!this.mIsRestart)
          {
            this.mFrontendHandler.mEventOnFrontendClosed -= new EventHandler(this.FrontendHandler_CloseMainWindow);
            this.mFrontendHandler.mEventOnFrontendClosed += new EventHandler(this.FrontendHandler_CloseMainWindow);
          }
          if (isWaitForPlayerClosing)
            this.mFrontendHandler.KillFrontendAsync();
          else
            this.mFrontendHandler.KillFrontend(false);
          if (this.mDiscordhandler != null)
          {
            this.mDiscordhandler.Dispose();
            this.mDiscordhandler = (Discord) null;
          }
          if (!SecurityMetrics.SecurityMetricsInstanceList.ContainsKey(this.mVmName))
            return;
          SecurityMetrics.SecurityMetricsInstanceList[this.mVmName].SendSecurityBreachesStatsToCloud(true);
        }
        catch (Exception ex)
        {
          Logger.Error("Error occured in CloseWindowHandler " + ex.ToString());
        }
      }));
    }

    private void FrontendHandler_CloseMainWindow(object sender, EventArgs e)
    {
      this.CloseMainWindow();
    }

    private void UpdateSynchronizerInstancesList()
    {
      try
      {
        this.Dispatcher.Invoke((Delegate) (() =>
        {
          foreach (KeyValuePair<string, MainWindow> dictWindow in BlueStacksUIUtils.DictWindows)
          {
            if (dictWindow.Value.mSynchronizerWindow != null && dictWindow.Value.mSynchronizerWindow.IsVisible)
              dictWindow.Value.mSynchronizerWindow.Init(false);
          }
        }));
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in updating instances for sync operation: " + ex.ToString());
      }
    }

    private void MainWindow_Closing(object sender, CancelEventArgs e)
    {
      if (this.mClosing)
      {
        this.mClosing = false;
        if (!Opt.Instance.h)
        {
          if (this.WindowState == WindowState.Normal)
          {
            try
            {
              this.EngineInstanceRegistry.WindowPlacement = this.GetPlacement();
            }
            catch (Exception ex)
            {
              Logger.Error("Exception in MainWindow_Closing. Exception: " + ex?.ToString());
            }
          }
        }
        BlueStacksUIUtils.DictWindows[this.mVmName].mWelcomeTab.mHomeAppManager.DisposeHtmlSidePanel();
        BlueStacksUIUtils.DictWindows[this.mVmName].mWelcomeTab.DisposeHtmHomeBrowser();
        BlueStacksUIUtils.DictWindows.Remove(this.mVmName);
        foreach (string key in BlueStacksUIUtils.DictWindows.Keys)
          GrmHandler.RequirementConfigUpdated(key);
        this.UpdateSynchronizationState();
        this.UpdateSynchronizerInstancesList();
        if (this.mVmName == "Android")
          BlueStacksUpdater.DownloadCompleted -= new System.Action<BlueStacks.Common.Tuple<BlueStacksUpdateData, bool>>(this.BlueStacksUpdater_DownloadCompleted);
        EventHandler onInstanceClosed = this.mEventOnInstanceClosed;
        if (onInstanceClosed != null)
          onInstanceClosed((object) this.mVmName, (EventArgs) null);
        this.ReleaseClientGlobalLock();
        if (BlueStacksUIUtils.DictWindows.Count == 0 && !this.mIsRestart)
        {
          AppUsageTimer.SaveData();
          GlobalKeyBoardMouseHooks.UnHookGlobalHooks();
          App.UnwindEvents();
          App.ReleaseLock();
          EventHandler onAllWindowClosed = this.mEventOnAllWindowClosed;
          if (onAllWindowClosed != null)
            onAllWindowClosed((object) this.mVmName, (EventArgs) null);
          if (HttpHandlerSetup.Server != null)
            HttpHandlerSetup.Server.Stop();
          BlueStacksUIUtils.sStopStatSendingThread = true;
          if (!ProcessUtils.IsAlreadyRunning("Global\\BlueStacks_MultiInstanceManager_Lockbgp"))
          {
            if (MainWindow.sIsClosingForBackupRestore)
              BlueStacks.Common.Utils.RunHDQuit(false, true, false, "bgp");
            else
              BlueStacks.Common.Utils.RunHDQuit(false, true, true, "bgp");
          }
          System.Windows.Application.Current.Shutdown();
        }
        this.mIsRestart = false;
      }
      else
      {
        e.Cancel = true;
        this.CloseWindow();
      }
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
      this.HandleDisplaySettingsChanged();
      if (string.IsNullOrEmpty(this.EngineInstanceRegistry.WindowPlacement) || !RegistryManager.Instance.IsRememberWindowPositionEnabled)
      {
        this.Left = (SystemParameters.MaximizedPrimaryScreenWidth - this.Width) / 2.0;
        this.Top = (SystemParameters.MaximizedPrimaryScreenHeight - this.Height) / 2.0;
      }
      else
        this.SetPlacement(this.EngineInstanceRegistry.WindowPlacement);
      bool flag = false;
      IntereopRect fullscreenMonitorSize = WindowWndProcHandler.GetFullscreenMonitorSize(this.Handle, true);
      if ((this.Left + this.Width + (this.EngineInstanceRegistry.IsSidebarVisible ? 62.0 : 0.0)) * MainWindow.sScalingFactor > (double) (fullscreenMonitorSize.Left + fullscreenMonitorSize.Width))
      {
        this.Left = (double) (fullscreenMonitorSize.X + fullscreenMonitorSize.Width) / MainWindow.sScalingFactor - this.Width - 62.0;
        if (this.Left < 0.0)
          this.Left = 0.0;
        flag = true;
      }
      if ((this.Top + this.Height) * MainWindow.sScalingFactor > (double) (fullscreenMonitorSize.Top + fullscreenMonitorSize.Height))
      {
        this.Top = (double) (fullscreenMonitorSize.Y + fullscreenMonitorSize.Height) / MainWindow.sScalingFactor - this.Height;
        if (this.Top < 0.0)
          this.Top = 0.0;
        flag = true;
      }
      if (flag)
        this.mSidebar?.ArrangeAllSidebarElements();
      if (string.IsNullOrEmpty(this.EngineInstanceRegistry.WindowPlacement) && Oem.IsOEMDmm)
        this.Left -= (Oem.IsOEMDmm ? (this.Height - (double) this.ParentWindowHeightDiff) * 9.0 / 16.0 + (double) this.ParentWindowWidthDiff : 0.0) / 2.0;
      this.mTopBar.mPreferenceDropDownControl.Init(this);
      if (FeatureManager.Instance.IsCustomUIForNCSoft)
        this.mNCTopBar.mSettingsDropDownControl.Init(this);
      this.mFullScreenTopBar.Init(this);
      this.CloseWindowConfirmationAcceptedHandler += new EventHandler(this.MainWindow_CloseWindowConfirmationAcceptedHandler);
      this.CloseWindowConfirmationResetAccountAcceptedHandler += new EventHandler(this.MainWindow_CloseWindowConfirmationResetAccountAcceptedHandler);
      this.RestartEngineConfirmationAcceptedHandler += new EventHandler(this.MainWindow_RestartEngineConfirmationAcceptedHandler);
      this.RestartPcConfirmationAcceptedHandler += new EventHandler(this.MainWindow_RestartPcConfirmationHandler);
      GlobalKeyBoardMouseHooks.SetBossKeyHook();
      this.mTopBar.ChangeUserPremiumButton(RegistryManager.Instance.IsPremium);
      if (this.IsDefaultVM)
      {
        this.pikaNotificationTimer.Interval = TimeSpan.FromMilliseconds(3500.0);
        this.pikaNotificationTimer.Tick += new EventHandler(this.PikaNotificationTimer_Tick);
        this.pikaPopControl.ParentWindow = this;
        this.pikaNotificationWorkQueue.Start();
      }
      this.ClientLaunchedStats();
      this.toastTimer.Interval = TimeSpan.FromMilliseconds(3000.0);
      this.toastTimer.Tick += new EventHandler(this.ToastTimer_Tick);
      this.mFullScreenToastTimer.Interval = TimeSpan.FromMilliseconds(5000.0);
      this.mFullScreenToastTimer.Tick += new EventHandler(this.FullScreenToastTimer_Tick);
      this.mImageUploadedToastTimer.Interval = TimeSpan.FromMilliseconds(10000.0);
      this.mImageUploadedToastTimer.Tick += new EventHandler(this.ImageUploadedToastTimer_Tick);
      this.IsFarmingInstance = !string.IsNullOrEmpty(this.WindowLaunchParams) && JObject.Parse(this.WindowLaunchParams).ContainsKey("isFarmingInstance");
      if (this.IsFarmingInstance)
        this.ArrangeNewWindowInCascade();
      this.SetMaxSizeOfWindow();
    }

    private void ArrangeNewWindowInCascade()
    {
      int width1 = Screen.PrimaryScreen.WorkingArea.Width;
      double windowWidth = this.Width;
      double y = (double) Screen.PrimaryScreen.WorkingArea.Top;
      double x = (double) Screen.PrimaryScreen.WorkingArea.Left;
      JObject jobject = new JObject();
      if (!string.IsNullOrEmpty(this.WindowLaunchParams))
        jobject = JObject.Parse(this.WindowLaunchParams);
      if (jobject["source_vmname"] != null && BlueStacksUIUtils.DictWindows.ContainsKey(jobject["source_vmname"].ToString()))
      {
        double width2 = BlueStacksUIUtils.DictWindows[jobject["source_vmname"].ToString()].Width;
        if (width2 > 0.0)
          windowWidth = width2;
        if (!BlueStacksUIUtils.DictWindows[jobject["source_vmname"].ToString()].mIsFullScreen && !BlueStacksUIUtils.DictWindows[jobject["source_vmname"].ToString()].mIsMaximized)
        {
          y = BlueStacksUIUtils.DictWindows[jobject["source_vmname"].ToString()].Top;
          x = BlueStacksUIUtils.DictWindows[jobject["source_vmname"].ToString()].Left;
        }
        x += 45.0;
        y += 45.0;
      }
      if (!BlueStacksUIUtils.DictWindows.ContainsKey(this.mVmName))
        return;
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        if (this.WindowState == WindowState.Minimized)
          this.RestoreWindows(false);
        KMManager.CloseWindows();
        double heightFromWidth = this.GetHeightFromWidth(windowWidth * MainWindow.sScalingFactor, false, false);
        this.ChangeHeightWidthTopLeft(windowWidth * MainWindow.sScalingFactor, heightFromWidth, y * MainWindow.sScalingFactor, x * MainWindow.sScalingFactor);
        this.Focus();
      }));
    }

    private void ImageUploadedToastTimer_Tick(object sender, EventArgs e)
    {
      this.mImageUploadedToastTimer.Stop();
      this.mImageUploadedPopup.IsOpen = false;
    }

    private void ClientLaunchedStats()
    {
      if (RegistryManager.Instance.IsClientUpgraded && RegistryManager.Instance.IsClientFirstLaunch == 1)
        ClientStats.SendClientStatsAsync("update_init", "success", "emulator_activity", "", "", this.mVmName);
      else if (RegistryManager.Instance.IsClientFirstLaunch == 1)
        ClientStats.SendClientStatsAsync("first_init", "success", "emulator_activity", "", "", this.mVmName);
      else
        ClientStats.SendClientStatsAsync("init", "success", "emulator_activity", "", "", this.mVmName);
    }

    internal void CreateFirebaseBrowserControl()
    {
      Logger.Info("In CreateFirebaseBrowserControl");
      (this.FirebaseBrowserControlGrid.Children[0] as BrowserControl).CreateNewBrowser();
    }

    private void MainWindow_ContentRendered(object sender, EventArgs e)
    {
      if (this.isSetupDone)
        return;
      this.isSetupDone = true;
      if (!Oem.IsOEMDmm)
      {
        if (!this.Utils.IsRequiredFreeRAMAvailable() && !this.IsFarmingInstance)
        {
          this.mFrontendHandler.mIsSufficientRAMAvailable = false;
          this.mFrontendHandler.FrontendHandler_ShowLowRAMMessage();
        }
        else
          this.Utils.CheckGuestFailedAsync();
        if (this.mVmName == BlueStacks.Common.Strings.CurrentDefaultVmName)
        {
          BlueStacksUpdater.DownloadCompleted += new System.Action<BlueStacks.Common.Tuple<BlueStacksUpdateData, bool>>(this.BlueStacksUpdater_DownloadCompleted);
          BlueStacksUpdater.SetupBlueStacksUpdater(this, true);
        }
      }
      this.ContentRendered -= new EventHandler(this.MainWindow_ContentRendered);
    }

    private void BlueStacksUpdater_DownloadCompleted(BlueStacks.Common.Tuple<BlueStacksUpdateData, bool> result)
    {
      if (!result.Item1.IsUpdateAvailble || !result.Item1.IsFullInstaller)
        return;
      this.Dispatcher.Invoke((Delegate) (() => this.ShowInstallPopup()));
    }

    public void ShowInstallPopup()
    {
      this.ShowDimOverlay((IDimOverlayControl) null);
      CustomMessageWindow customMessageWindow = new CustomMessageWindow();
      BlueStacksUIBinding.Bind(customMessageWindow.TitleTextBlock, "STRING_UPDATE_AVAILABLE", "");
      customMessageWindow.ImageName = "update_icon";
      BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlock, "STRING_NEW_UPDATE_READY", "");
      customMessageWindow.BodyWarningTextBlock.Visibility = Visibility.Visible;
      BlueStacksUIBinding.Bind(customMessageWindow.BodyWarningTextBlock, "STRING_NEW_UPDATE_READY_WARNING", "");
      customMessageWindow.BodyWarningTextBlock.Foreground = (System.Windows.Media.Brush) new SolidColorBrush((System.Windows.Media.Color) System.Windows.Media.ColorConverter.ConvertFromString("#F09200"));
      customMessageWindow.UrlTextBlock.Visibility = Visibility.Visible;
      customMessageWindow.UrlLink.Inlines.Add(LocaleStrings.GetLocalizedString("STRING_LEARN_WHATS_NEW", "Learn What's New"));
      customMessageWindow.UrlLink.NavigateUri = new Uri(BlueStacksUpdater.sBstUpdateData.DetailedChangeLogsUrl);
      customMessageWindow.UrlLink.RequestNavigate += new RequestNavigateEventHandler(this.OpenRecentChangelogs);
      customMessageWindow.CloseButtonHandle((EventHandler) ((s, ev) => ClientStats.SendBluestacksUpdaterUIStatsAsync(ClientStatsEvent.InstallPopupCross, "")), (object) null);
      customMessageWindow.AddButton(ButtonColors.Blue, "STRING_INSTALL_NOW", (EventHandler) ((s, ev) =>
      {
        ClientStats.SendBluestacksUpdaterUIStatsAsync(ClientStatsEvent.InstallPopupNow, "");
        BlueStacksUpdater.CheckDownloadedUpdateFileAndUpdate();
      }), (string) null, false, (object) null, true);
      customMessageWindow.AddButton(ButtonColors.White, "STRING_INSTALL_NEXT_BOOT", (EventHandler) ((s, ev) => ClientStats.SendBluestacksUpdaterUIStatsAsync(ClientStatsEvent.InstallPopupLater, "")), (string) null, false, (object) null, true);
      customMessageWindow.Owner = (Window) this.mDimOverlay;
      customMessageWindow.ShowDialog();
      this.HideDimOverlay();
    }

    private void OpenRecentChangelogs(object sender, RequestNavigateEventArgs e)
    {
      BlueStacksUIUtils.OpenUrl(e.Uri.OriginalString);
      e.Handled = true;
    }

    private void MainWindow_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
    {
      if (e.Key == Key.Snapshot || e.Key == Key.O)
        this.HandleKeyDown(e.Key);
      if (e.SystemKey != Key.Snapshot)
        return;
      this.HandleKeyDown(e.SystemKey);
    }

    private void MainWindow_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
      if (e.Key == Key.System)
        this.HandleKeyDown(e.SystemKey);
      else
        this.HandleKeyDown(e.Key);
    }

    internal static string GetShortcutKey(Key key)
    {
      string str = string.Empty;
      if (key != Key.None)
      {
        if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
          str = IMAPKeys.GetStringForFile(Key.LeftCtrl) + " + ";
        if (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt))
          str = str + IMAPKeys.GetStringForFile(Key.LeftAlt) + " + ";
        if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
          str = str + IMAPKeys.GetStringForFile(Key.LeftShift) + " + ";
        str += IMAPKeys.GetStringForFile(key);
      }
      return str;
    }

    internal void HandleKeyDown(Key key)
    {
      string shortcutKey = MainWindow.GetShortcutKey(key);
      Logger.Debug("SHORTCUT: KeyPressed.." + shortcutKey);
      if (this.mCommonHandler.mShortcutsConfigInstance == null)
        return;
      foreach (ShortcutKeys shortcutKeys in this.mCommonHandler.mShortcutsConfigInstance.Shortcut)
      {
        if (string.Equals(shortcutKeys.ShortcutKey, shortcutKey, StringComparison.InvariantCulture))
        {
          this.HandleClientHotKey((ClientHotKeys) Enum.Parse(typeof (ClientHotKeys), shortcutKeys.ShortcutName));
          Logger.Debug("SHORTCUT: Shortcut Name.." + shortcutKeys.ShortcutName);
        }
      }
    }

    internal void HandleClientHotKey(ClientHotKeys clienthotKey)
    {
      try
      {
        switch (clienthotKey)
        {
          case ClientHotKeys.STRING_TOGGLE_KEYMAP_WINDOW:
            ThreadPool.QueueUserWorkItem((WaitCallback) (obj =>
            {
              try
              {
                this.Dispatcher.Invoke((Delegate) (() =>
                {
                  if (this.mTopBar.mAppTabButtons.SelectedTab.mTabType != TabType.AppTab)
                    return;
                  this.mCommonHandler.GameGuideButtonHandler("shortcut", "sidebar", false);
                }));
              }
              catch
              {
              }
            }));
            break;
          case ClientHotKeys.STRING_TOGGLE_FARM_MODE:
            if (FeatureManager.Instance.IsFarmingModeDisabled)
              break;
            HTTPUtils.SendRequestToMultiInstanceAsync("toggleMIMFarmMode", (Dictionary<string, string>) null, "Android", 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
            break;
          case ClientHotKeys.AddWebTab:
            if (Oem.IsOEMDmm)
              break;
            ThreadPool.QueueUserWorkItem((WaitCallback) (obj =>
            {
              try
              {
                this.Dispatcher.Invoke((Delegate) (() => this.mTopBar.mAppTabButtons.AddWebTab("https://www.google.com/", "Google", string.Empty, true, DateTime.Now.ToString((IFormatProvider) CultureInfo.InvariantCulture), true)));
              }
              catch (Exception ex)
              {
                Logger.Error("Exception while ading web tab using key shortcut:{0}", (object) ex);
              }
            }));
            break;
          case ClientHotKeys.STRING_TOGGLE_OVERLAY:
            ThreadPool.QueueUserWorkItem((WaitCallback) (obj => this.Dispatcher.Invoke((Delegate) (() =>
            {
              if (Oem.IsOEMDmm)
              {
                this.mDmmBottomBar.mTranslucentControlsSliderButton_PreviewMouseLeftButtonUp((object) null, (MouseButtonEventArgs) null);
              }
              else
              {
                if (!this.mSidebar.mIsKeymappingStateOn)
                  return;
                if (RegistryManager.Instance.TranslucentControlsTransparency != 0.0)
                {
                  ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "Overlay", "shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, "toggleOff", (string) null, "Android");
                  KMManager.ShowOverlayWindow(this, false, false);
                  this.mCommonHandler.OnOverlayStateChanged(false);
                }
                else
                {
                  if (KMManager.CheckIfKeymappingWindowVisible(false) || this.mTopBar.mAppTabButtons.SelectedTab.mTabType != TabType.AppTab)
                    return;
                  ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "Overlay", "shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, "toggleOn", (string) null, "Android");
                  this.mCommonHandler.OnOverlayStateChanged(true);
                  KMManager.ShowOverlayWindow(this, true, false);
                }
              }
            }))));
            break;
          case ClientHotKeys.STRING_UPDATED_FULLSCREEN_BUTTON_TOOLTIP:
            ThreadPool.QueueUserWorkItem((WaitCallback) (obj => this.Dispatcher.Invoke((Delegate) (() =>
            {
              if (this.mCommonHandler == null || this.mStreamingModeEnabled)
                return;
              this.mCommonHandler.FullScreenButtonHandler("sidebar", "shortcut");
            }))));
            break;
          case ClientHotKeys.STRING_TOGGLE_LOCK_CURSOR:
            if (Oem.IsOEMDmm)
              break;
            ThreadPool.QueueUserWorkItem((WaitCallback) (obj => this.Dispatcher.Invoke((Delegate) (() =>
            {
              if (this.mCommonHandler == null)
                return;
              this.mCommonHandler.ClipMouseCursorHandler(false, true, "shortcut", "sidebar");
            }))));
            break;
          case ClientHotKeys.RestoreWindow:
            if (!this.mIsFullScreen || !RegistryManager.Instance.UseEscapeToExitFullScreen)
              break;
            ThreadPool.QueueUserWorkItem((WaitCallback) (obj => this.Dispatcher.Invoke((Delegate) (() => this.RestoreWindows(false)))));
            break;
          case ClientHotKeys.STRING_TOGGLE_KEYMAPPING_STATE:
            if (this.mTopBar.mAppTabButtons.SelectedTab == null || this.mTopBar.mAppTabButtons.SelectedTab.mTabType != TabType.AppTab)
              break;
            ThreadPool.QueueUserWorkItem((WaitCallback) (obj => this.Dispatcher.Invoke((Delegate) (() =>
            {
              if (Oem.IsOEMDmm)
                this.mCommonHandler.DMMSwitchKeyMapButtonHandler();
              else
                this.mSidebar.KeyMapSwitchButtonHandler((SidebarElement) null, false);
            }))));
            break;
          case ClientHotKeys.STRING_TRANSLATOR_TOOL:
            if (Oem.IsOEMDmm)
              break;
            ThreadPool.QueueUserWorkItem((WaitCallback) (obj => this.Dispatcher.Invoke((Delegate) (() =>
            {
              try
              {
                if (this.mTopBar.mAppTabButtons.SelectedTab.mTabType != TabType.AppTab)
                  return;
                this.mCommonHandler.ImageTranslationHandler();
                ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "translatorTool", "shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
              }
              catch (Exception ex)
              {
                Logger.Error("error while calling image translation function.." + ex.ToString());
              }
            }))));
            break;
          case ClientHotKeys.STRING_ALWAYS_ON_TOP:
            this.EngineInstanceRegistry.IsClientOnTop = !this.EngineInstanceRegistry.IsClientOnTop;
            this.Topmost = this.EngineInstanceRegistry.IsClientOnTop;
            ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, this.EngineInstanceRegistry.IsClientOnTop ? "PinToTopOn" : "PinToTopOff", "shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
            break;
          case ClientHotKeys.STRING_INCREASE_VOLUME:
            if (this.mSidebar == null || this.mSidebar.GetElementFromTag("sidebar_volume") == null || (this.mSidebar.GetElementFromTag("sidebar_volume").Visibility != Visibility.Visible || !this.mSidebar.GetElementFromTag("sidebar_volume").IsEnabled))
              break;
            if (this.mSidebar.mVolumeSliderPopupTimer == null)
            {
              this.mSidebar.mVolumeSliderPopupTimer = new DispatcherTimer()
              {
                Interval = new TimeSpan(0, 0, 2)
              };
              this.mSidebar.mVolumeSliderPopupTimer.Tick += new EventHandler(this.mSidebar.VolumeSliderPopupTimer_Tick);
            }
            else
              this.mSidebar.mVolumeSliderPopupTimer.Stop();
            this.mSidebar.mVolumeSliderPopupTimer.Start();
            this.mSidebar.mVolumeSliderPopup.IsOpen = true;
            this.Utils.VolumeUpHandler();
            ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "VolumeUp", "shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
            break;
          case ClientHotKeys.STRING_DECREASE_VOLUME:
            if (this.mSidebar == null || this.mSidebar.GetElementFromTag("sidebar_volume") == null || (this.mSidebar.GetElementFromTag("sidebar_volume").Visibility != Visibility.Visible || !this.mSidebar.GetElementFromTag("sidebar_volume").IsEnabled))
              break;
            if (this.mSidebar.mVolumeSliderPopupTimer == null)
            {
              this.mSidebar.mVolumeSliderPopupTimer = new DispatcherTimer()
              {
                Interval = new TimeSpan(0, 0, 2)
              };
              this.mSidebar.mVolumeSliderPopupTimer.Tick += new EventHandler(this.mSidebar.VolumeSliderPopupTimer_Tick);
            }
            else
              this.mSidebar.mVolumeSliderPopupTimer.Stop();
            this.mSidebar.mVolumeSliderPopupTimer.Start();
            this.mSidebar.mVolumeSliderPopup.IsOpen = true;
            this.Utils.VolumeDownHandler();
            ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "VolumeDown", "shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
            break;
          case ClientHotKeys.STRING_TOGGLE_MUTE_STATE:
            if (this.mSidebar == null)
              break;
            this.mCommonHandler.MuteUnmuteButtonHandler();
            ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, this.EngineInstanceRegistry.IsMuted ? "VolumeOn" : "VolumeOff", "shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
            break;
          case ClientHotKeys.STRING_TOOLBAR_CAMERA:
            if (this.mSidebar == null || this.mSidebar.GetElementFromTag("sidebar_screenshot") == null || (this.mSidebar.GetElementFromTag("sidebar_screenshot").Visibility != Visibility.Visible || !this.mSidebar.GetElementFromTag("sidebar_screenshot").IsEnabled))
              break;
            this.mCommonHandler.ScreenShotButtonHandler();
            ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "Screenshot", "shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
            break;
          case ClientHotKeys.STRING_MACRO_RECORDER:
            if (this.mSidebar == null || this.mSidebar.GetElementFromTag("sidebar_macro") == null || (this.mSidebar.GetElementFromTag("sidebar_macro").Visibility != Visibility.Visible || !this.mSidebar.GetElementFromTag("sidebar_macro").IsEnabled) || !FeatureManager.Instance.IsMacroRecorderEnabled && !FeatureManager.Instance.IsCustomUIForNCSoft)
              break;
            if (this.mIsMacroRecorderActive)
              this.ShowToast(LocaleStrings.GetLocalizedString("STRING_STOP_RECORDING_FIRST", ""), "", "", false);
            else
              this.mCommonHandler.ShowMacroRecorderWindow();
            ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "MacroRecorder", "shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
            break;
          case ClientHotKeys.STRING_SYNCHRONISER:
            if (!FeatureManager.Instance.IsOperationsSyncEnabled)
              break;
            if (!BlueStacksUIUtils.sSyncInvolvedInstances.Contains(this.mVmName) || this.mIsSyncMaster)
              this.ShowSynchronizerWindow();
            ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "OperationSync", "shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
            break;
          case ClientHotKeys.STRING_RECORD_SCREEN:
            if (this.mSidebar == null || this.mSidebar.GetElementFromTag("sidebar_video_capture") == null || (this.mSidebar.GetElementFromTag("sidebar_video_capture").Visibility != Visibility.Visible || !this.mSidebar.GetElementFromTag("sidebar_video_capture").IsEnabled))
              break;
            this.mCommonHandler.DownloadAndLaunchRecording("sidebar", "shortcut");
            break;
          case ClientHotKeys.STRING_HOME:
            this.mCommonHandler.HomeButtonHandler(true, false);
            ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "Home", "shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
            break;
          case ClientHotKeys.STRING_BACK:
            this.mCommonHandler.BackButtonHandler(false);
            ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "Back", "shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
            break;
          case ClientHotKeys.STRING_SHAKE:
            this.mCommonHandler.ShakeButtonHandler();
            ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "Shake", "shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
            break;
          case ClientHotKeys.STRING_ROTATE:
            if (this.mSidebar == null)
              break;
            this.mSidebar.RotateButtonHandler("shortcut");
            break;
          case ClientHotKeys.STRING_OPEN_MEDIA_FOLDER:
            CommonHandlers.OpenMediaFolder();
            ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "MediaFolder", "shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
            break;
          case ClientHotKeys.STRING_TOGGLE_MULTIINSTANCE_WINDOW:
            try
            {
              if (FeatureManager.Instance.IsCustomUIForNCSoft)
                break;
              Process.Start(System.IO.Path.Combine(RegistryStrings.InstallDir, "HD-MultiInstanceManager.exe"));
              ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "MultiInstance", "shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
              break;
            }
            catch (Exception ex)
            {
              Logger.Error("Couldn't launch MI Manager. Ex: {0}", (object) ex.Message);
              break;
            }
          case ClientHotKeys.STRING_SET_LOCATION:
            this.mCommonHandler.LocationButtonHandler();
            ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "SetLocation", "shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
            break;
          case ClientHotKeys.STRING_GAMEPAD_CONTROLS:
            if (this.mTopBar.mAppTabButtons.SelectedTab == null || this.mTopBar.mAppTabButtons.SelectedTab.mTabType != TabType.AppTab || (!this.mWelcomeTab.mHomeAppManager.CheckDictAppIconFor(this.mTopBar.mAppTabButtons.SelectedTab.PackageName, (Predicate<AppIconModel>) (_ => _.IsGamepadCompatible)) || this.mCommonHandler.ToggleGamepadAndKeyboardGuidance("gamepad", false)))
              break;
            KMManager.HandleInputMapperWindow(this, "gamepad");
            ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "GamePad", "shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, this.mTopBar.mAppTabButtons.SelectedTab?.PackageName, (string) null, "Android");
            break;
          case ClientHotKeys.STRING_MINIMIZE_TOOLTIP:
            this.MinimizeWindow();
            ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "Minimize", "shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
            break;
          case ClientHotKeys.STRING_AUTOMATIC_SORTING:
            CommonHandlers.ArrangeWindow();
            ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "ArrangeWindow", "shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
            break;
          case ClientHotKeys.STRING_START_STREAMING:
            bool mIsStreaming = this.mIsStreaming;
            NCSoftUtils.Instance.SendStreamingEvent(this.mVmName, this.mIsStreaming ? "off" : "on");
            ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, mIsStreaming ? "StreamVideoOff" : "StreamVideoOn", "shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
            break;
          case ClientHotKeys.STRING_SETTINGS:
            this.mTopBar.mSettingsMenuPopup.IsOpen = false;
            string tabName = string.Empty;
            if (this.StaticComponents.mSelectedTabButton.mTabType == TabType.AppTab && !PackageActivityNames.SystemApps.Contains(this.StaticComponents.mSelectedTabButton.PackageName))
              tabName = "STRING_GAME_SETTINGS";
            ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "Setting", "shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
            this.mCommonHandler.LaunchSettingsWindow(tabName);
            break;
          case ClientHotKeys.STRING_CONTROLS_EDITOR:
            ThreadPool.QueueUserWorkItem((WaitCallback) (obj =>
            {
              try
              {
                this.Dispatcher.Invoke((Delegate) (() =>
                {
                  if (this.mTopBar.mAppTabButtons.SelectedTab.mTabType != TabType.AppTab)
                    return;
                  this.mCommonHandler.KeyMapButtonHandler("shortcut", "sidebar");
                }));
              }
              catch
              {
              }
            }));
            break;
          case ClientHotKeys.STRING_IMAGE_PICKER:
            try
            {
              HTTPUtils.SendRequestToEngineAsync("toggleImagePickerMode", new Dictionary<string, string>()
              {
                {
                  "isInImagePickerMode",
                  "true"
                }
              }, this.mVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
              break;
            }
            catch (Exception ex)
            {
              Logger.Error("Exception in image picker mode: " + ex.ToString());
              break;
            }
          case ClientHotKeys.STRING_UNLOCK_MOUSE:
            if (this.mTopBar.mAppTabButtons.SelectedTab == null || this.mTopBar.mAppTabButtons.SelectedTab.mTabType != TabType.AppTab || !this.mTopBar.mAppTabButtons.SelectedTab.IsCursorClipped)
              break;
            ThreadPool.QueueUserWorkItem((WaitCallback) (obj => this.Dispatcher.Invoke((Delegate) (() =>
            {
              this.mCommonHandler?.ClipMouseCursorHandler(true, true, "", "");
              ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "UnlockMouseEsc", "shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, this.mTopBar.mAppTabButtons.SelectedTab.PackageName, (string) null, "Android");
            }))));
            break;
          case ClientHotKeys.STRING_UTC_CONVERTER:
            if (this.mSidebar == null || this.mSidebar.GetElementFromTag("sidebar_utc_converter") == null || (this.mSidebar.GetElementFromTag("sidebar_utc_converter").Visibility != Visibility.Visible || !this.mSidebar.GetElementFromTag("sidebar_utc_converter").IsEnabled))
              break;
            ThreadPool.QueueUserWorkItem((WaitCallback) (obj => this.Dispatcher.Invoke((Delegate) (() =>
            {
              try
              {
                SidebarElement elementFromTag = this.mSidebar.GetElementFromTag("sidebar_utc_converter");
                Sidebar.UpdateImage(elementFromTag, "sidebar_video_loading");
                elementFromTag.Image.Visibility = Visibility.Hidden;
                elementFromTag.Image.IsImageToBeRotated = true;
                elementFromTag.Image.Visibility = Visibility.Visible;
                new SideHtmlWidgetWindow(this, this.StaticComponents.mSelectedTabButton.PackageName).Show();
                ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "UtcConverter", "shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, this.mTopBar.mAppTabButtons.SelectedTab?.PackageName, (string) null, "Android");
              }
              catch (Exception ex)
              {
                Logger.Error("Exception in launching utc converter, " + ex.ToString());
              }
            }))));
            break;
          case ClientHotKeys.STRING_INSTALL_APK:
            if (this.mSidebar == null || this.mSidebar.GetElementFromTag("sidebar_installapk") == null || (this.mSidebar.GetElementFromTag("sidebar_installapk").Visibility != Visibility.Visible || !this.mSidebar.GetElementFromTag("sidebar_installapk").IsEnabled))
              break;
            ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "InstallApk", "shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
            this.mCommonHandler.InstallApkHandler();
            break;
          case ClientHotKeys.STRING_SWITCH_OVERLAY_CONTROLS:
            try
            {
              if (!this.mSidebar.mIsKeymappingStateOn || !RegistryManager.Instance.ShowKeyControlsOverlay)
                break;
              if (this.mCommonHandler.CheckIfGamepadOverlaySelectedForApp(this.mTopBar.mAppTabButtons.SelectedTab.PackageName))
              {
                this.mCommonHandler.SwitchOverlayControls(false);
                ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "KeyboardOverlay", "shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
                break;
              }
              this.mCommonHandler.SwitchOverlayControls(true);
              ClientStats.SendMiscellaneousStatsAsync("sidebar", RegistryManager.Instance.UserGuid, "GamepadOverlay", "shortcut", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
              break;
            }
            catch (Exception ex)
            {
              Logger.Error("Exception in shortcut: switch overlay: " + ex.ToString());
              break;
            }
          case ClientHotKeys.STRING_TOGGLE_MICROPHONE_MUTE_STATE:
            Publisher.PublishMessage(BrowserControlTags.toggleMicrophoneMuteState, this.mVmName, (JObject) null);
            break;
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in executing shortcut: " + ex.ToString());
      }
    }

    internal void ResizeMainWindowForKeyMapSidebar()
    {
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        bool flag = false;
        IntereopRect fullscreenMonitorSize = WindowWndProcHandler.GetFullscreenMonitorSize(this.Handle, false);
        double width = (double) fullscreenMonitorSize.Width;
        if (this.WindowState == WindowState.Maximized || this.mIsFullScreen)
        {
          this.RestoreWindows(false);
          flag = true;
        }
        if (flag || this.ActualWidth * MainWindow.sScalingFactor > width - MainWindow.sScalingFactor * 241.0)
        {
          double num = width - 241.0 * MainWindow.sScalingFactor;
          double heightFromWidth = this.GetHeightFromWidth(num, true, false);
          InteropWindow.SetWindowPos(this.Handle, (IntPtr) 0, 0, Convert.ToInt32(Math.Floor(((double) fullscreenMonitorSize.Height - heightFromWidth) / 2.0)), Convert.ToInt32(Math.Floor(num)), Convert.ToInt32(Math.Floor(heightFromWidth)), 80U);
        }
        else
        {
          if (this.ActualWidth * MainWindow.sScalingFactor + this.Left * MainWindow.sScalingFactor <= width - MainWindow.sScalingFactor * 241.0)
            return;
          InteropWindow.SetWindowPos(this.Handle, (IntPtr) 0, Convert.ToInt32(Math.Floor(width - MainWindow.sScalingFactor * 241.0 - this.ActualWidth * MainWindow.sScalingFactor)), Convert.ToInt32(Math.Floor(this.Top * MainWindow.sScalingFactor)), Convert.ToInt32(Math.Floor(this.ActualWidth * MainWindow.sScalingFactor)), Convert.ToInt32(Math.Floor(this.ActualHeight * MainWindow.sScalingFactor)), 80U);
        }
      }));
    }

    internal Grid AddBrowser(string url)
    {
      Grid grid = new Grid();
      BrowserControl browserControl1 = new BrowserControl(url);
      browserControl1.Visibility = Visibility.Visible;
      BrowserControl browserControl2 = browserControl1;
      CustomPictureBox customPictureBox1 = new CustomPictureBox();
      customPictureBox1.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
      customPictureBox1.VerticalAlignment = VerticalAlignment.Center;
      customPictureBox1.Height = 30.0;
      customPictureBox1.Width = 30.0;
      customPictureBox1.ImageName = "loader";
      customPictureBox1.IsImageToBeRotated = true;
      CustomPictureBox customPictureBox2 = customPictureBox1;
      grid.Children.Add((UIElement) browserControl2);
      grid.Children.Add((UIElement) customPictureBox2);
      grid.Visibility = Visibility.Hidden;
      this.mContentGrid.Children.Add((UIElement) grid);
      return grid;
    }

    internal void Frontend_OrientationChanged(string packagename, bool isPortrait)
    {
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        if (!string.IsNullOrEmpty(packagename))
        {
          AppTabButton tab = this.mTopBar.mAppTabButtons.GetTab(packagename);
          if (tab != null)
            tab.IsPortraitModeTab = isPortrait;
          if (this.AppForcedOrientationDict.ContainsKey(packagename))
            this.AppForcedOrientationDict[packagename] = isPortrait;
        }
        this.mCommonHandler.ClipMouseCursorHandler(false, false, "", "");
      }));
    }

    internal void GuestBoot_Completed()
    {
      Logger.Info("BOOT_STAGE: In Guestboot_completed ");
      this.ShowLoadingGrid(false);
      if (!this.mGuestBootCompleted)
      {
        this.mGuestBootCompleted = true;
        Publisher.PublishMessage(BrowserControlTags.bootComplete, this.mVmName, (JObject) null);
        this.OnGuestBootCompleted();
        if (Oem.IsOEMDmm)
          this.Dispatcher.Invoke((Delegate) (() => this.FrontendParentGrid.Visibility = Visibility.Visible));
        this.HideQuitPopupIfShown();
        this.mWelcomeTab.mHomeAppManager.InitAppPromotionEvents();
        if (RegistryManager.Instance.InstallationType == InstallationTypes.GamingEdition)
          this.Dispatcher.Invoke((Delegate) (() => this.mWelcomeTab.mBackground.Visibility = Visibility.Hidden));
        AppUsageTimer.StartTimer(this.mVmName, "Home");
        KMManager.GetCurrentParserVersion(this);
        this.Utils.GetCurrentVolumeAtBootAsyncAndSetMuteInstancesState();
        BlueStacksUIUtils.InvokeMIManagerEvents(this.mVmName);
        this.EngineInstanceRegistry.LastBootDate = DateTime.Now.Date.ToShortDateString();
        this.Utils.sBootCheckTimer.Enabled = false;
        this.mTopBar.InitializeSnailButton();
        if (!Opt.Instance.hiddenBootMode)
          this.CheckIfVtxDisabledOrUnavailableAndShowPopup();
        FrontendHandler.UpdateBootTimeInregistry(this.mBootStartTime);
        GrmHandler.RequirementConfigUpdated(this.mVmName);
        if (!FeatureManager.Instance.IsPromotionDisabled)
          this.mWelcomeTab.RemovePromotionGrid();
        if (this.mIsWindowHidden)
          this.HandleShowWindowForFarmingInstance();
        if (this.IsMuted)
          this.Utils.MuteApplication();
        else
          this.Utils.UnmuteApplication();
        if (!Oem.IsOEMDmm)
        {
          if (RegistryManager.Instance.InstallationType == InstallationTypes.GamingEdition)
          {
            if (this.mTopBar.CheckForRam())
              this.mTopBar.AddRamNotification();
            bool flag = false;
            if (!string.IsNullOrEmpty(this.WindowLaunchParams))
            {
              JObject jobject = JObject.Parse(this.WindowLaunchParams);
              if (jobject["app_pkg"] != null && !string.IsNullOrEmpty(jobject["app_pkg"].ToString().Trim().Trim()))
              {
                this.mAppHandler.PerformGamingAction(jobject["app_pkg"].ToString().Trim(), "");
                flag = true;
              }
            }
            if (!flag)
              this.mAppHandler.PerformGamingAction("", "");
          }
          else
          {
            this.PerformPendingRegistryActionIfAny();
            if (this.EngineInstanceRegistry.IsGoogleSigninDone)
            {
              this.PostGoogleSigninCompleteTask();
            }
            else
            {
              if (this.mTopBar.CheckForRam())
                this.mTopBar.AddRamNotification();
              this.HandleSslConnectionError();
              System.Action<bool> suggestionHandler = PromotionObject.AppSuggestionHandler;
              if (suggestionHandler != null)
                suggestionHandler(false);
            }
            this.PostBootCompleteTask();
          }
        }
        this.BootCompletedStats();
      }
      if (this.EngineInstanceRegistry.NativeGamepadState == NativeGamepadState.ForceOn)
        this.mFrontendHandler.SendFrontendRequestAsync("enableNativeGamepad", new Dictionary<string, string>()
        {
          {
            "isEnabled",
            "true"
          }
        });
      this.mCommonHandler.SetTouchSoundSettings();
    }

    private void OnFullScreenChanged(bool isFullScreen)
    {
      MainWindow.FullScreenChangedEventHandler fullScreenChanged = this.FullScreenChanged;
      if (fullScreenChanged == null)
        return;
      fullScreenChanged((object) this, new MainWindowEventArgs.FullScreenChangedEventArgs()
      {
        IsFullscreen = isFullScreen
      });
    }

    private void OnGuestBootCompleted()
    {
      MainWindow.GuestBootCompletedEventHandler guestBootCompleted = this.GuestBootCompleted;
      if (guestBootCompleted == null)
        return;
      guestBootCompleted((object) this, new EventArgs());
    }

    internal void OnCursorLockChanged(bool locked)
    {
      MainWindow.CursorLockChangedEventHandler lockChangedEvent = this.CursorLockChangedEvent;
      if (lockChangedEvent == null)
        return;
      lockChangedEvent((object) this, new MainWindowEventArgs.CursorLockChangedEventArgs()
      {
        IsLocked = locked
      });
    }

    private QuitPopupControl GetQuitPopupFromDimOverlay()
    {
      return this.mDimOverlay != null ? this.mDimOverlay.Control as QuitPopupControl : (QuitPopupControl) null;
    }

    private void HideQuitPopupIfShown()
    {
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        try
        {
          QuitPopupControl popupFromDimOverlay = this.GetQuitPopupFromDimOverlay();
          if (popupFromDimOverlay == null)
            return;
          popupFromDimOverlay.Close();
          ClientStats.SendLocalQuitPopupStatsAsync(popupFromDimOverlay.CurrentPopupTag, "popup_auto_hidden");
        }
        catch (Exception ex)
        {
          Logger.Error("Couldn't notify QuitPopup for boot complete. Ex: {0}", (object) ex);
        }
      }));
    }

    internal void InitDiscord()
    {
      try
      {
        if (!RegistryManager.Instance.DiscordEnabled || !this.IsDefaultVM)
          return;
        if (this.mDiscordhandler == null)
          this.mDiscordhandler = new Discord(this);
        this.mDiscordhandler.Init();
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in init discord: {0}", (object) ex.ToString());
      }
    }

    private void HandleSslConnectionError()
    {
      try
      {
        string guest = HTTPUtils.SendRequestToGuest("checkSSLConnection", (Dictionary<string, string>) null, this.mVmName, 10000, (Dictionary<string, string>) null, false, 1, 0, "bgp");
        JObject jobject = JObject.Parse(guest);
        if (string.Equals(jobject["result"].ToString(), "error", StringComparison.InvariantCulture) && jobject["reason"].ToString().Contains("SSLHandshakeException"))
          this.Dispatcher.Invoke((Delegate) (() =>
          {
            CustomMessageWindow customMessageWindow = new CustomMessageWindow();
            customMessageWindow.ImageName = "security_icon";
            BlueStacksUIBinding.Bind(customMessageWindow.TitleTextBlock, "STRING_ANTIVIRUS_ISSUE", "");
            BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlockTitle, "STRING_ANTIVIRUS_ISSUE_HEADER", "");
            customMessageWindow.BodyTextBlockTitle.Visibility = Visibility.Visible;
            customMessageWindow.BodyTextBlockTitle.FontWeight = FontWeights.Regular;
            customMessageWindow.BodyTextBlock.Inlines.Clear();
            customMessageWindow.BodyTextBlock.Inlines.Add((UIElement) new TextBlock()
            {
              Text = LocaleStrings.GetLocalizedString("STRING_TECHNICAL_TIP", ""),
              FontWeight = FontWeights.Bold
            });
            customMessageWindow.BodyTextBlock.Inlines.Add(" ");
            customMessageWindow.BodyTextBlock.Inlines.Add(LocaleStrings.GetLocalizedString("STRING_ANTIVIRUS_ISSUE_FIX", ""));
            customMessageWindow.AddButton(ButtonColors.Blue, "STRING_SEE_HOW_TO_FIX", (EventHandler) ((sender1, e1) => BlueStacksUIUtils.OpenUrl(WebHelper.GetUrlWithParams(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) WebHelper.GetServerHost(), (object) "help_articles"), (string) null, (string) null, (string) null) + "&article=failed_ssl_connection")), "external_link", true, (object) null, true);
            this.ShowDimOverlay((IDimOverlayControl) null);
            customMessageWindow.Owner = (Window) this;
            customMessageWindow.ShowDialog();
            if (this.mDimOverlay == null || this.mDimOverlay.OwnedWindows.OfType<ContainerWindow>().Any<ContainerWindow>())
              return;
            this.HideDimOverlay();
          }));
        ClientStats.SendMiscellaneousStatsAsync("SslConnectionResponse", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.UserSelectedLocale, guest, (string) null, (string) null, (string) null, (string) null, "Android");
      }
      catch (Exception ex)
      {
        Logger.Error("Exception when testing whether facing any problem in reaching google. " + ex.ToString());
      }
    }

    private void PerformPendingRegistryActionIfAny()
    {
      string pendingAction = RegistryManager.Instance.PendingLaunchAction;
      if (string.IsNullOrEmpty(pendingAction))
        return;
      GenericAction action = (GenericAction) Enum.Parse(typeof (GenericAction), pendingAction.Split(',')[0].Trim(), true);
      string actionValue = pendingAction.Split(',')[1].Trim();
      if (action != GenericAction.None)
        this.Dispatcher.Invoke((Delegate) (() =>
        {
          Logger.Info("Performing pending registry action: {0}", (object) pendingAction);
          if (this.mAppHandler.IsAppInstalled(actionValue))
          {
            this.mAppHandler.SendRunAppRequestAsync(actionValue, "", false);
          }
          else
          {
            if (action != GenericAction.InstallPlay)
              return;
            this.mAppHandler.LaunchPlayRequestAsync(actionValue);
          }
        }));
      RegistryManager.Instance.PendingLaunchAction = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0},{1}", (object) GenericAction.None, (object) string.Empty);
    }

    internal void CheckIfVtxDisabledOrUnavailableAndShowPopup()
    {
      if (Oem.IsOEMDmm)
        return;
      Logger.Info("In CheckIfVtxDisabledOrUnavailableAndShowPopup");
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        string deviceCaps = RegistryManager.Instance.DeviceCaps;
        if (string.IsNullOrEmpty(deviceCaps))
          return;
        JObject jobject = JObject.Parse(deviceCaps);
        if (jobject["cpu_hvm"].ToString().Equals("True", StringComparison.OrdinalIgnoreCase) && jobject["bios_hvm"].ToString().Equals("False", StringComparison.OrdinalIgnoreCase))
          this.ShowImprovePerformanceWarningPopup(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}&article={1}", (object) WebHelper.GetUrlWithParams(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) WebHelper.GetServerHost(), (object) "help_articles"), (string) null, (string) null, (string) null), (object) "enable_virtualization"), "STRING_VTX_DISABLED_WARNING_MESSAGE");
        else if (jobject["cpu_hvm"].ToString().Equals("False", StringComparison.OrdinalIgnoreCase))
        {
          this.ShowImprovePerformanceWarningPopup(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}&article={1}", (object) WebHelper.GetUrlWithParams(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) WebHelper.GetServerHost(), (object) "help_articles"), (string) null, (string) null, (string) null), (object) "vtx_unavailable"), "STRING_VTX_UNAVAILABLE_WARNING_MESSAGE");
        }
        else
        {
          if (!jobject["cpu_hvm"].ToString().Equals("True", StringComparison.OrdinalIgnoreCase) || !jobject["bios_hvm"].ToString().Equals("True", StringComparison.OrdinalIgnoreCase) || !jobject["engine_enabled"].ToString().Equals(EngineState.raw.ToString(), StringComparison.InvariantCultureIgnoreCase))
            return;
          this.ShowImprovePerformanceWarningPopup(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}&article={1}", (object) WebHelper.GetUrlWithParams(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) WebHelper.GetServerHost(), (object) "help_articles"), (string) null, (string) null, (string) null), (object) "disable_antivirus"), "STRING_VTX_DISABLED_WARNING_MESSAGE");
        }
      }));
    }

    private void ShowImprovePerformanceWarningPopup(string url, string bodyTextKeyValue)
    {
      CustomMessageWindow window = new CustomMessageWindow();
      BlueStacksUIBinding.Bind(window.TitleTextBlock, "STRING_IMPROVE_PERFORMANCE", "");
      window.AddWarning(LocaleStrings.GetLocalizedString("STRING_IMPROVE_PERFORMANCE_WARNING", ""), "message_error");
      window.Owner = (Window) this;
      BlueStacksUIBinding.Bind(window.BodyTextBlock, bodyTextKeyValue, "");
      window.AddButton(ButtonColors.Blue, "STRING_CHECK_FAQ", (EventHandler) ((sender1, e1) => BlueStacksUIUtils.OpenUrl(url)), (string) null, false, (object) null, true);
      window.AddButton(ButtonColors.White, "STRING_CONTINUE_ANYWAY", (EventHandler) ((sender1, e1) => window.Close()), (string) null, false, (object) null, true);
      window.Show();
    }

    internal void CloseBrowserQuitPopup()
    {
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        if (this.mQuitPopupBrowserControl == null)
          return;
        this.mQuitPopupBrowserControl.Close();
      }));
    }

    private void BootCompletedStats()
    {
      if (RegistryManager.Instance.IsClientFirstLaunch == 1)
      {
        if (RegistryManager.Instance.IsEngineUpgraded == 1)
          ClientStats.SendClientStatsAsync("update_init", "success", "engine_activity", "", "", this.mVmName);
        else
          ClientStats.SendClientStatsAsync("first_init", "success", "engine_activity", "", "", this.mVmName);
        RegistryManager.Instance.IsClientFirstLaunch = 0;
        NativeMethods.waveOutSetVolume(IntPtr.Zero, uint.MaxValue);
        HTTPUtils.SendRequestToAgentAsync("downloadInstalledAppsCfg", (Dictionary<string, string>) null, this.mVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
      }
      else
        ClientStats.SendClientStatsAsync("init", "success", "engine_activity", "", "", this.mVmName);
    }

    private void PostBootCompleteTask()
    {
      this.GetMacroShortcutKeyMappingsWithRestrictedKeysandNames();
      System.Action<bool> recommendationHandler = PromotionObject.AppRecommendationHandler;
      if (recommendationHandler != null)
        recommendationHandler(false);
      PostBootCloudInfoManager.Instance.GetPostBootDataAsync(this);
      this.mInstanceWiseCloudInfoManager.GetInstanceWisePostBootDataAsync(this);
      MainWindow.CheckUserPremiumAsync();
      if (RegistryManager.Instance.RequirementConfigUpdateRequired)
      {
        HTTPUtils.SendRequestToGuest("getConfigList", (Dictionary<string, string>) null, this.mVmName, 1000, (Dictionary<string, string>) null, false, 1, 0, "bgp");
        RegistryManager.Instance.RequirementConfigUpdateRequired = false;
      }
      this.Dispatcher.Invoke((Delegate) (() => this.mTopBar.ChangeUserPremiumButton(RegistryManager.Instance.IsPremium)));
      PromotionObject.QuestHandler -= new System.Action(this.HandleQuestForFrontend);
      PromotionObject.QuestHandler += new System.Action(this.HandleQuestForFrontend);
      this.HandleQuestForFrontend();
      this.mAppHandler.UpdateDefaultLauncher();
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        this.mAppInstaller = new DownloadInstallApk(this);
        if (Oem.Instance.IsDragDropEnabled)
          FileImporter.Init(this);
        this.mWelcomeTab.mPromotionControl.HandlePromotionEventAfterBoot();
      }));
      try
      {
        DownloadInstallApk.SerialWorkQueueInstaller(this.mVmName).Start();
      }
      catch (ThreadStateException ex)
      {
        Logger.Info("Thread Already Started" + ex.ToString());
      }
      if (this.mStartupTabLaunched)
        this.Dispatcher.Invoke((Delegate) (() => this.mTopBar.mAppTabButtons.GoToTab(1)));
      else if (this.mPostBootNotificationAction != null)
      {
        AppInfo infoFromPackageName = new JsonParser(this.mVmName).GetAppInfoFromPackageName(this.mPostBootNotificationAction);
        if (infoFromPackageName != null)
          Process.Start(RegistryStrings.InstallDir + "\\HD-RunApp.exe", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} -vmname {1}", (object) ("-json \"" + new JObject()
          {
            {
              "app_icon_url",
              (JToken) ""
            },
            {
              "app_name",
              (JToken) infoFromPackageName.Name
            },
            {
              "app_url",
              (JToken) ""
            },
            {
              "app_pkg",
              (JToken) this.mPostBootNotificationAction
            }
          }.ToString(Formatting.None).Replace("\"", "\\\"") + "\""), (object) this.mVmName));
        else
          this.Utils.HandleGenericActionFromDictionary(new Dictionary<string, string>()
          {
            {
              "click_generic_action",
              GenericAction.InstallPlay.ToString()
            },
            {
              "click_action_packagename",
              this.mPostBootNotificationAction
            }
          }, "notification_drawer", "");
        this.mPostBootNotificationAction = (string) null;
      }
      this.HandleFLEorAppPopupPostBoot();
      this.mCommonHandler.CheckForMacroScriptOnRestart();
      this.UpdateSynchronizerInstancesList();
      this.InitDiscord();
      BlueStacks.Common.Utils.SetGoogleAdIdAndAndroidIdFromAndroid(this.mVmName);
      if (PromotionObject.Instance != null && PromotionObject.Instance.IsSecurityMetricsEnable)
      {
        SecurityMetrics.Init(this.mVmName);
        this.Dispatcher.Invoke((Delegate) (() => this.mResizeHandler.AddRawInputHandler()));
      }
      RegistryManager.Instance.Guest[this.mVmName].IsGoogleSigninPopupShown = true;
      if (new DriveInfo(System.IO.Path.GetPathRoot(RegistryManager.Instance.UserDefinedDir)).AvailableFreeSpace < 1073741824L)
        this.ShowLowDiskSpaceWarning();
      if (RegistryManager.Instance.IsEngineUpgraded == 1 && RegistryManager.Instance.IsClientFirstLaunch == 1 && MainWindow.VersionCheckForSmartControl())
        this.ShowAppPopupAfterUpgrade("com.dts.freefireth");
      if (string.Compare(this.mVmName, BlueStacks.Common.Strings.CurrentDefaultVmName, StringComparison.OrdinalIgnoreCase) != 0)
        return;
      CloudNotificationManager.PostBootCompleted();
    }

    private void ShowAppPopupAfterUpgrade(string packageName)
    {
      try
      {
        this.Dispatcher.Invoke((Delegate) (() =>
        {
          if (this.mWelcomeTab.mHomeAppManager.GetAppIcon(packageName) == null || !File.Exists(BlueStacks.Common.Utils.GetInputmapperUserFilePath(packageName)))
            return;
          CustomMessageWindow customMessageWindow = new CustomMessageWindow();
          customMessageWindow.TitleTextBlock.Text = string.Format((IFormatProvider) CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_SMART_CONTROLS_ENABLED_0", ""), (object) "Garena Free Fire");
          customMessageWindow.BodyTextBlock.Text = string.Format((IFormatProvider) CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_FREEFIRE_NOTIFICATION_MESSAGE", ""), (object) "Garena Free Fire");
          customMessageWindow.BodyWarningTextBlock.Visibility = Visibility.Visible;
          BlueStacksUIBinding.Bind(customMessageWindow.BodyWarningTextBlock, "STRING_FREEFIRE_NOTIFICATION_DETAIL", "");
          customMessageWindow.BodyWarningTextBlock.FontWeight = FontWeights.Light;
          BlueStacksUIBinding.BindColor((DependencyObject) customMessageWindow.BodyWarningTextBlock, System.Windows.Controls.Control.ForegroundProperty, "SettingsWindowForegroundDimDimColor");
          customMessageWindow.AddButton(ButtonColors.Blue, "STRING_OK", (EventHandler) null, (string) null, false, (object) null, true);
          customMessageWindow.UrlTextBlock.Visibility = Visibility.Visible;
          customMessageWindow.UrlLink.Inlines.Add(LocaleStrings.GetLocalizedString("STRING_FREEFIRE_NOTIFICATION_LINK", ""));
          string uriString = WebHelper.GetUrlWithParams(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) WebHelper.GetServerHost(), (object) "help_articles"), (string) null, (string) null, (string) null) + "&article=smart_control";
          customMessageWindow.UrlLink.NavigateUri = new Uri(uriString);
          customMessageWindow.UrlLink.RequestNavigate += new RequestNavigateEventHandler(this.OpenSmartControlHelp);
          this.ShowDimOverlay((IDimOverlayControl) null);
          customMessageWindow.Owner = (Window) this.mDimOverlay;
          customMessageWindow.ShowDialog();
          if (this.mDimOverlay == null || !this.mDimOverlay.OwnedWindows.OfType<ContainerWindow>().Any<ContainerWindow>())
            return;
          this.HideDimOverlay();
        }));
      }
      catch (Exception ex)
      {
        Logger.Warning("Exception in showing app notifications after upgrade: " + ex.ToString());
      }
    }

    private void OpenSmartControlHelp(object sender, RequestNavigateEventArgs e)
    {
      BlueStacksUIUtils.OpenUrl(e.Uri.OriginalString);
    }

    private static bool VersionCheckForSmartControl()
    {
      return RegistryManager.Instance.UpgradeVersionList.Length != 0 && new System.Version(((IEnumerable<string>) RegistryManager.Instance.UpgradeVersionList).Last<string>()) < new System.Version("4.140.00.0000");
    }

    private void ShowLowDiskSpaceWarning()
    {
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        CustomMessageWindow customMessageWindow = new CustomMessageWindow();
        customMessageWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_LOW_DISK_SPACE", "");
        customMessageWindow.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_LOW_DISK_SPACE_MESSAGE", "");
        customMessageWindow.AddWarning(LocaleStrings.GetLocalizedString("STRING_LOW_DISK_SPACE_WARNING", ""), "");
        customMessageWindow.BodyWarningTextBlock.Visibility = Visibility.Visible;
        customMessageWindow.AddButton(ButtonColors.Blue, "STRING_OK", (EventHandler) null, (string) null, false, (object) null, true);
        this.ShowDimOverlay((IDimOverlayControl) null);
        customMessageWindow.Owner = (Window) this.mDimOverlay;
        customMessageWindow.ShowDialog();
        this.HideDimOverlay();
      }));
    }

    internal void PostGoogleSigninCompleteTask()
    {
      if (!this.mIsTokenAvailable && RegistryManager.Instance.InstallationType != InstallationTypes.GamingEdition)
        BlueStacksUIUtils.SendBluestacksLoginRequest(this.mVmName);
      if (!Oem.IsOEMDmm)
      {
        this.Dispatcher.Invoke((Delegate) (() =>
        {
          this.mTopBar.TopBarOptionsPanelElementVisibility((FrameworkElement) this.mTopBar.mUserAccountBtn, true);
          this.mTopBar.TopBarOptionsPanelElementVisibility((FrameworkElement) this.mTopBar.mNotificationGrid, true);
        }));
        System.Action<bool> suggestionHandler = PromotionObject.AppSuggestionHandler;
        if (suggestionHandler != null)
          suggestionHandler(false);
      }
      MainWindow.BrowserOTSCompletedCallbackEventHandler completedCallback = this.BrowserOTSCompletedCallback;
      if (completedCallback == null)
        return;
      completedCallback((object) this, new MainWindowEventArgs.BrowserOTSCompletedCallbackEventArgs()
      {
        CallbackFunction = this.mBrowserCallbackFunctionName
      });
    }

    internal static void CheckUserPremiumAsync()
    {
      ThreadPool.QueueUserWorkItem((WaitCallback) (obj =>
      {
        try
        {
          PromotionManager.CheckIsUserPremium();
        }
        catch (Exception ex)
        {
          Logger.Error("PostOTSBootComplete: call for premium failed" + ex.ToString());
        }
      }));
    }

    private void HandleQuestForFrontend()
    {
      if (PromotionObject.Instance.QuestHdPlayerRules.Count <= 0)
        return;
      this.mFrontendHandler.SendFrontendRequest("setPackagesForInteraction", new Dictionary<string, string>()
      {
        {
          "data",
          JsonConvert.SerializeObject((object) PromotionObject.Instance.QuestHdPlayerRules, Formatting.None)
        }
      });
      PromotionManager.StartQuestRulesProcessor();
    }

    private void HandleFLEorAppPopupPostBoot()
    {
      this.GetFleCampaignJson();
      if (!RegistryManager.Instance.Guest[this.mVmName].IsGoogleSigninPopupShown && !RegistryManager.Instance.Guest[this.mVmName].IsGoogleSigninDone)
        return;
      if (!string.IsNullOrEmpty(this.WindowLaunchParams))
      {
        JObject jobject = JObject.Parse(this.WindowLaunchParams);
        if (jobject["app_pkg"] != null && !jobject.ContainsKey("check_fle_pkg") && !string.IsNullOrEmpty(jobject["app_pkg"].ToString().Trim().Trim()))
        {
          new DownloadInstallApk(this).DownloadAndInstallAppFromJson(this.WindowLaunchParams);
          return;
        }
      }
      if (this.mStartupTabLaunched || PromotionObject.Instance.StartupTab.Count <= 0)
        return;
      if (PromotionObject.Instance.StartupTab.ContainsKey("click_generic_action") && EnumHelper.Parse<GenericAction>(PromotionObject.Instance.StartupTab["click_generic_action"], GenericAction.None) != GenericAction.None && (string.IsNullOrEmpty(RegistryManager.Instance.RegisteredEmail.Trim()) || string.IsNullOrEmpty(RegistryManager.Instance.Token.Trim())))
        this.mLaunchStartupTabWhenTokenReceived = true;
      if (this.mLaunchStartupTabWhenTokenReceived)
        return;
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        this.mStartupTabLaunched = true;
        this.Utils.HandleGenericActionFromDictionary((Dictionary<string, string>) PromotionObject.Instance.StartupTab, "startup_tab", "");
      }));
    }

    public void PublishForFlePopupToBrowser(string json)
    {
      if (string.IsNullOrEmpty(json))
        return;
      JObject jobject = JObject.Parse(json);
      if (jobject["fle_pkg"] == null)
        return;
      Publisher.PublishMessage(BrowserControlTags.showFlePopup, this.mVmName, new JObject((object) new JProperty("PackageName", (object) jobject["fle_pkg"].ToString().Trim())));
    }

    public void PublishForHandleFleToBrowser(string json)
    {
      if (string.IsNullOrEmpty(json))
        return;
      JObject jobject = JObject.Parse(json);
      if (jobject["check_fle_pkg"] == null)
        return;
      Publisher.PublishMessage(BrowserControlTags.handleFle, this.mVmName, new JObject((object) new JProperty("PackageName", (object) jobject["check_fle_pkg"].ToString().Trim())));
    }

    private void GetFleCampaignJson()
    {
      string fleCampaignMd5 = RegistryManager.Instance.FLECampaignMD5;
      if (string.IsNullOrEmpty(fleCampaignMd5))
        return;
      try
      {
        string str = BstHttpClient.Get(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/bs3/getcampaigninfo?md5_hash={1}", (object) RegistryManager.Instance.Host, (object) fleCampaignMd5), (Dictionary<string, string>) null, false, this.mVmName, 0, 1, 0, false, "bgp");
        RegistryManager.Instance.DeleteFLECampaignMD5();
        RegistryManager.Instance.CampaignJson = str;
      }
      catch
      {
        Logger.Info("Error fetching campaign json");
      }
    }

    private void HandleFLEorAppPopupBeforeBoot()
    {
      this.GetFleCampaignJson();
      if (!RegistryManager.Instance.Guest[this.mVmName].IsGoogleSigninPopupShown && !RegistryManager.Instance.Guest[this.mVmName].IsGoogleSigninDone)
        return;
      if (!string.IsNullOrEmpty(this.WindowLaunchParams) && string.Equals("Android", this.mVmName, StringComparison.InvariantCulture))
      {
        JObject jobject = JObject.Parse(this.WindowLaunchParams);
        if (jobject["app_pkg"] != null && !string.IsNullOrEmpty(jobject["app_pkg"].ToString().Trim().Trim()))
          return;
      }
      if (PromotionObject.Instance.StartupTab.Count <= 0)
        return;
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        if (!PromotionObject.Instance.StartupTab.ContainsKey("click_generic_action") || (EnumHelper.Parse<GenericAction>(PromotionObject.Instance.StartupTab["click_generic_action"], GenericAction.None) & (GenericAction.ApplicationBrowser | GenericAction.HomeAppTab)) == (GenericAction) 0)
          return;
        this.mStartupTabLaunched = true;
        this.Utils.HandleGenericActionFromDictionary((Dictionary<string, string>) PromotionObject.Instance.StartupTab, "startup_tab", "");
      }));
    }

    internal void ShowLoadingGrid(bool isShow)
    {
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        try
        {
          if (isShow)
          {
            this.mTopBar.mAppTabButtons.EnableAppTabs(false);
            if (!Oem.IsOEMDmm)
              this.mWelcomeTab.mHomeAppManager.ChangeHomeAppLoadingGridVisibility(Visibility.Visible);
          }
          else
          {
            this.mTopBar.mAppTabButtons.EnableAppTabs(true);
            if (!Oem.IsOEMDmm)
              this.mWelcomeTab.mHomeAppManager.ChangeHomeAppLoadingGridVisibility(Visibility.Hidden);
          }
        }
        catch (Exception ex)
        {
          Logger.Error("Exception in ShowLoadingGrid. " + ex.ToString());
        }
        Logger.Info("BOOT_STAGE: Removing progress bar");
      }));
    }

    internal void ShowControlGrid(Grid controlGrid)
    {
      if (this.mLastVisibleGrid != null && controlGrid != this.mLastVisibleGrid)
        this.mLastVisibleGrid.Visibility = Visibility.Hidden;
      this.mLastVisibleGrid = controlGrid;
      controlGrid.Visibility = Visibility.Visible;
    }

    private void TopBar_MouseDown(object sender, MouseButtonEventArgs e)
    {
      if (e.OriginalSource.GetType().Equals(typeof (CustomPictureBox)) && !this.mTopBar.WindowHeaderGrid.IsMouseOver)
        return;
      try
      {
        this.DragMove();
      }
      catch
      {
      }
      this.UIChangesOnMainWindowSizeChanged();
    }

    private void TopBar_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      if (e.OriginalSource.GetType().Equals(typeof (CustomPictureBox)))
        return;
      if (this.WindowState == WindowState.Maximized)
        this.RestoreWindows(false);
      else
        this.MaximizeWindow();
    }

    internal void RestoreWindows(bool isReArrange = false)
    {
      if (!this.mResizeHandler.IsMinMaxEnabled)
        return;
      if (Oem.IsOEMDmm)
        this.mTopBarPopup.IsOpen = false;
      if (this.mGeneraltoast.IsOpen)
      {
        this.toastTimer.Stop();
        this.mGeneraltoast.IsOpen = false;
      }
      if (!Oem.IsOEMDmm)
      {
        this.OnFullScreenChanged(false);
        this.ToggleFullScreenToastVisibility(false, "", "", "");
      }
      this.TopBar.Visibility = Visibility.Visible;
      this.OuterBorder.BorderThickness = new Thickness(1.0);
      if (Oem.IsOEMDmm)
        this.mDmmBottomBar.Visibility = Visibility.Visible;
      if (!isReArrange && this.mIsFullScreenFromMaximized && this.mIsFullScreen)
      {
        IntereopRect fullscreenMonitorSize = WindowWndProcHandler.GetFullscreenMonitorSize(this.Handle, true);
        InteropWindow.SetWindowPos(this.Handle, (IntPtr) 0, fullscreenMonitorSize.Left, fullscreenMonitorSize.Top, fullscreenMonitorSize.Width, fullscreenMonitorSize.Height, 80U);
        this.UIChangesOnMainWindowSizeChanged();
      }
      else
      {
        this.mIsFullScreenFromMaximized = false;
        if (Oem.IsOEMDmm && this.mDMMRecommendedWindow != null && this.mIsDMMRecommendedWindowOpen)
          this.mDMMRecommendedWindow.Visibility = Visibility.Visible;
        this.mResizeHandler.mAdjustingWidth = false;
        if (this.mTopBar.mAppTabButtons.SelectedTab != null)
          this.IsUIInPortraitMode = this.mTopBar.mAppTabButtons.SelectedTab.IsPortraitModeTab;
        this.mResizeGrid.Visibility = Visibility.Visible;
        this.FrontendParentGrid.Margin = new Thickness(1.0);
        this.WindowState = WindowState.Normal;
        this.mIsMaximized = false;
        this.ChangeHeightWidthAndPosition(this.GetWidthFromHeight(this.mPreviousHeight.Value, false, false), this.mPreviousHeight.Value, true);
        this.SwitchToPortraitMode(this.IsUIInPortraitMode);
        this.mIsDmmMaximised = false;
        if (Oem.IsOEMDmm)
          this.DmmRestoreWindowRectangle = new Rect(0.0, 0.0, 0.0, 0.0);
        else if (this.IsUIInPortraitMode)
          this.mTopBar.RefreshNotificationCentreButton();
        this.mResizeHandler.IsResizingEnabled = true;
        this.mTopBar.mMaximizeButton.ImageName = "maximize";
        BlueStacksUIBinding.Bind((System.Windows.Controls.Image) this.mTopBar.mMaximizeButton, "STRING_MAXIMIZE_TOOLTIP");
        this.mNCTopBar.mMaximizeButtonImage.ImageName = "maximize";
        BlueStacksUIBinding.Bind((System.Windows.Controls.Image) this.mNCTopBar.mMaximizeButtonImage, "STRING_MAXIMIZE_TOOLTIP");
        this.mCommonHandler.ClipMouseCursorHandler(false, false, "", "");
        if (KMManager.sGuidanceWindow != null)
        {
          KMManager.sGuidanceWindow.Visibility = Visibility.Visible;
          this.Dispatcher.Invoke((Delegate) (() => this.Focus()));
        }
      }
      this.mTopBar.UpdateMacroRecordingProgress();
      this.mIsFullScreen = false;
      HTTPUtils.SendRequestToEngineAsync("setIsFullscreen", new Dictionary<string, string>()
      {
        {
          "isFullscreen",
          "false"
        }
      }, this.mVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
      this.mCommonHandler.ClipMouseCursorHandler(false, false, "", "");
    }

    internal void MinimizeWindow()
    {
      this.WindowState = WindowState.Minimized;
    }

    internal void MaximizeWindow()
    {
      if (!this.mResizeHandler.IsMinMaxEnabled)
        return;
      this.mIsDMMMaximizedFromPortrait = this.IsUIInPortraitMode;
      if (Oem.IsOEMDmm && this.mDMMRecommendedWindow != null)
        this.mDMMRecommendedWindow.Visibility = Visibility.Hidden;
      if (this.WindowState == WindowState.Normal)
      {
        this.mPreviousWidth = new double?(this.Width);
        this.mPreviousHeight = new double?(this.Height);
      }
      this.mIsDmmMaximised = true;
      if (Oem.IsOEMDmm && this.IsUIInPortraitMode && !this.mIsFullScreen)
      {
        this.SetDMMRestoreWindowSizeAndPosition();
        this.SetSizeForDMMPortraitMaximisedWindow();
      }
      else
      {
        if (Oem.IsOEMDmm && !this.mIsFullScreen)
          this.SetDMMRestoreWindowSizeAndPosition();
        this.IsUIInPortraitModeWhenMaximized = this.IsUIInPortraitMode;
        this.IsUIInPortraitMode = !(this.mAspectRatio > (Fraction) 1L);
        this.WindowState = WindowState.Maximized;
        this.mIsMaximized = true;
      }
      this.mResizeHandler.IsResizingEnabled = false;
      this.mTopBar.mMaximizeButton.ImageName = "restore";
      this.mTopBar.RefreshNotificationCentreButton();
      this.mTopBar.UpdateMacroRecordingProgress();
      BlueStacksUIBinding.Bind((System.Windows.Controls.Image) this.mTopBar.mMaximizeButton, "STRING_RESTORE_BUTTON");
      this.mNCTopBar.mMaximizeButtonImage.ImageName = "restore";
      BlueStacksUIBinding.Bind((System.Windows.Controls.Image) this.mNCTopBar.mMaximizeButtonImage, "STRING_RESTORE_BUTTON");
      this.mTopBar.RefreshWarningButton();
      this.UIChangesOnMainWindowSizeChanged();
      if (KMManager.sGuidanceWindow == null || !string.Equals(KMManager.sGuidanceWindow.ParentWindow?.mVmName, this.mVmName, StringComparison.InvariantCultureIgnoreCase))
        return;
      KMManager.sGuidanceWindow.Visibility = Visibility.Collapsed;
      if (!AppConfigurationManager.Instance.VmAppConfig[this.mVmName].ContainsKey(this.mTopBar.mAppTabButtons.SelectedTab.PackageName) || (AppConfigurationManager.Instance.CheckIfTrueInAnyVm(this.mTopBar.mAppTabButtons.SelectedTab.PackageName, (Predicate<AppSettings>) (appSettings => appSettings.IsCloseGuidanceOnboardingCompleted), out string _) || this.mIsFullScreen))
        return;
      this.mSidebar?.ShowViewGuidancePopup();
      AppConfigurationManager.Instance.VmAppConfig[this.mVmName][this.mTopBar.mAppTabButtons.SelectedTab.PackageName].IsCloseGuidanceOnboardingCompleted = true;
    }

    internal void RestrictWindowResize(bool enable)
    {
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        this.mResizeHandler.IsMinMaxEnabled = !enable;
        this.mResizeHandler.IsResizingEnabled = !enable;
        this.mTopBar.mMaximizeButton.IsEnabled = !enable;
        this.mNCTopBar.mMaximizeButtonImage.IsEnabled = !enable;
        if (enable)
        {
          this.mTopBar.mMaximizeButton.SetDisabledState();
          this.mNCTopBar.mMaximizeButtonImage.SetDisabledState();
        }
        else
        {
          this.mTopBar.mMaximizeButton.SetNormalState();
          this.mNCTopBar.mMaximizeButtonImage.SetNormalState();
        }
      }));
    }

    internal void FullScreenWindow()
    {
      if (!Oem.IsOEMDmm && this.mTopBar.mAppTabButtons.SelectedTab.mTabType != TabType.AppTab)
        return;
      if (this.WindowState == WindowState.Normal)
      {
        this.mPreviousWidth = new double?(this.Width);
        this.mPreviousHeight = new double?(this.Height);
      }
      this.mIsFullScreen = true;
      this.OuterBorder.BorderThickness = new Thickness(0.0);
      if (Oem.IsOEMDmm)
      {
        this.mDmmBottomBar.Visibility = Visibility.Collapsed;
        this.mDmmBottomBar.ShowKeyMapPopup(false);
      }
      if (!Oem.IsOEMDmm)
        this.OnFullScreenChanged(true);
      this.TopBar.Visibility = Visibility.Collapsed;
      this.mResizeGrid.Visibility = Visibility.Collapsed;
      this.FrontendParentGrid.Margin = new Thickness(0.0);
      if (this.WindowState == WindowState.Maximized)
      {
        this.mIsFullScreenFromMaximized = true;
        IntereopRect fullscreenMonitorSize = WindowWndProcHandler.GetFullscreenMonitorSize(this.Handle, false);
        InteropWindow.SetWindowPos(this.Handle, (IntPtr) 0, fullscreenMonitorSize.Left, fullscreenMonitorSize.Top, fullscreenMonitorSize.Width, fullscreenMonitorSize.Height, 80U);
      }
      else
      {
        if (Oem.IsOEMDmm && this.WindowState != WindowState.Maximized && !this.mIsDmmMaximised)
          this.SetDMMRestoreWindowSizeAndPosition();
        this.MaximizeWindow();
      }
      System.Windows.Forms.Cursor.Clip = System.Drawing.Rectangle.Empty;
      if (Oem.IsOEMDmm)
      {
        this.mTopBarPopup.IsOpen = true;
      }
      else
      {
        string[] strArray = LocaleStrings.GetLocalizedString("STRING_FULLSCREEN_EXIT_POPUP_TEXT", "").Split('{', '}');
        this.ToggleFullScreenToastVisibility(true, strArray[0], this.mCommonHandler.GetShortcutKeyFromName("STRING_UPDATED_FULLSCREEN_BUTTON_TOOLTIP", false), strArray[2]);
      }
      if (KMManager.sGuidanceWindow != null)
        KMManager.sGuidanceWindow.Visibility = Visibility.Collapsed;
      HTTPUtils.SendRequestToEngineAsync("setIsFullscreen", new Dictionary<string, string>()
      {
        {
          "isFullscreen",
          "true"
        }
      }, this.mVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
      if (Oem.IsOEMDmm)
        new Thread((ThreadStart) (() =>
        {
          Thread.Sleep(1000);
          this.Dispatcher.Invoke((Delegate) (() =>
          {
            if (this.mDMMFST.IsMouseOver || this.mDMMFST.mVolumePopup.IsOpen || this.mDMMFST.mChangeTransparencyPopup.IsOpen)
              return;
            this.mTopBarPopup.IsOpen = false;
          }));
        }))
        {
          IsBackground = true
        }.Start();
      this.Dispatcher.BeginInvoke((Delegate) (() => this.UIChangesOnMainWindowSizeChanged()), DispatcherPriority.Render);
    }

    public void ToggleFullScreenToastVisibility(
      bool isFullScreen,
      string tip = "",
      string key = "",
      string info = "")
    {
      if (isFullScreen)
        this.ShowToast(tip, key, info, true);
      else
        this.CloseFullScreenToastAndStopTimer();
    }

    internal void ShowToast(string tip, string key = "", string info = "", bool isForced = false)
    {
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        try
        {
          if (!isForced && (!this.mIsWindowInFocus || Oem.IsOEMDmm))
            return;
          if (this.mFullScreenToastPopup.IsOpen)
          {
            this.mFullScreenToastTimer.Stop();
            this.mFullScreenToastPopup.IsOpen = false;
          }
          if (isForced)
          {
            if (string.IsNullOrEmpty(key))
              return;
            this.mFullScreenToastControl.Init(this, tip, key, info);
          }
          else
            this.mFullScreenToastControl.Init(this, tip);
          if (this.mUnlockMouseToastPopup.IsOpen)
            this.mUnlockMouseToastControl.CloseUnlockMouseToastAndStopTimer();
          this.dummyToast.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
          this.dummyToast.VerticalAlignment = VerticalAlignment.Top;
          this.mFullScreenToastControl.Visibility = Visibility.Visible;
          this.mFullScreenToastPopup.IsOpen = true;
          this.mFullScreenToastCanvas.Height = this.mFullScreenToastControl.ActualHeight;
          this.mFullScreenToastPopup.VerticalOffset = this.mFullScreenToastControl.ActualHeight + 20.0;
          this.mFullScreenToastPopup.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
          this.mFullScreenToastTimer.Interval = !this.mTopBar.mAppTabButtons.SelectedTab.mShootingModeToastIsOpen ? TimeSpan.FromMilliseconds(5000.0) : TimeSpan.FromMilliseconds(3000.0);
          this.mFullScreenToastTimer.Start();
        }
        catch (Exception ex)
        {
          Logger.Error("Exception in showing fullscreen toast : " + ex.ToString());
        }
      }));
    }

    private void SetDMMRestoreWindowSizeAndPosition()
    {
      this.DmmRestoreWindowRectangle = new Rect(this.Left, this.Top, this.Width, this.Height);
    }

    private void SetSizeForDMMPortraitMaximisedWindow()
    {
      double height = SystemParameters.WorkArea.Height;
      double width = this.GetWidthFromHeight(height, false, false);
      if (width > SystemParameters.WorkArea.Width / MainWindow.sScalingFactor)
      {
        width = SystemParameters.WorkArea.Width / MainWindow.sScalingFactor;
        height = this.GetHeightFromWidth(width, false, false);
      }
      if (width < this.MinWidth || height < this.MinHeight)
      {
        width = this.MinWidth;
        height = this.MinHeight;
      }
      this.Height = height;
      this.Width = width;
      this.Left = (SystemParameters.WorkArea.Width - this.Width) / 2.0;
      this.Top = 0.0;
    }

    private void BottomBar_MouseDown(object sender, MouseButtonEventArgs e)
    {
      if (e.OriginalSource.GetType().Equals(typeof (CustomPictureBox)))
        return;
      try
      {
        this.DragMove();
      }
      catch
      {
      }
    }

    private void ResizeGrid_Loaded(object sender, RoutedEventArgs e)
    {
      if (this.mResizeGrid != null)
        return;
      this.mResizeGrid = sender as Grid;
      this.WireSizingEvents();
    }

    private void WireSizingEvents()
    {
      foreach (UIElement child in this.mResizeGrid.Children)
      {
        if (child is System.Windows.Shapes.Rectangle rectangle)
        {
          rectangle.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(this.mResizeHandler.ResizeRectangle_PreviewMouseDown);
          rectangle.MouseMove += new System.Windows.Input.MouseEventHandler(this.mResizeHandler.ResizeRectangle_MouseMove);
        }
      }
    }

    private void FrontendGrid_IsVisibleChanged(object _, DependencyPropertyChangedEventArgs e)
    {
      this.mFrontendHandler.FrontendVisibleChanged((bool) e.NewValue);
      Logger.Debug("KMP FrontendGrid_IsVisibleChanged " + e.NewValue?.ToString() + this.mVmName);
      if ((bool) e.NewValue)
        this.OnFrontendGridVisible();
      else
        this.OnFrontendGridHidden();
      this.mFrontendHandler.ShowGLWindow();
    }

    private void OnFrontendGridHidden()
    {
      this.mFrontendHandler.DeactivateFrontend();
      MainWindow.FrontendGridVisibilityChangedEventHandler visibilityChanged = this.FrontendGridVisibilityChanged;
      if (visibilityChanged == null)
        return;
      visibilityChanged((object) this, new MainWindowEventArgs.FrontendGridVisibilityChangedEventArgs()
      {
        IsVisible = false
      });
    }

    private void OnFrontendGridVisible()
    {
      MainWindow.FrontendGridVisibilityChangedEventHandler visibilityChanged = this.FrontendGridVisibilityChanged;
      if (visibilityChanged == null)
        return;
      visibilityChanged((object) this, new MainWindowEventArgs.FrontendGridVisibilityChangedEventArgs()
      {
        IsVisible = true
      });
    }

    private void FrontendGrid_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      this.mFrontendHandler.ShowGLWindow();
    }

    private void FrontendParentGrid_IsVisibleChanged(
      object _1,
      DependencyPropertyChangedEventArgs _2)
    {
      if (this.FrontendParentGrid.Visibility != Visibility.Visible)
        return;
      if (!this.FrontendParentGrid.Children.Contains((UIElement) this.mFrontendGrid))
      {
        if (this.mFrontendGrid.Parent != null)
          (this.mFrontendGrid.Parent as Grid).Children.Remove((UIElement) this.mFrontendGrid);
        this.FrontendParentGrid.Children.Add((UIElement) this.mFrontendGrid);
      }
      if (!this.mGuestBootCompleted || !Oem.IsOEMDmm)
        return;
      this.mDmmProgressControl.Visibility = Visibility.Hidden;
      this.mFrontendGrid.Visibility = Visibility.Visible;
    }

    internal void HandleRestartPopup()
    {
      Logger.Info("Showing restart option to the user");
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        try
        {
          if (this.mIsWindowHidden)
            this.HandleShowWindowForFarmingInstance();
          CustomMessageWindow customMessageWindow = new CustomMessageWindow();
          BlueStacksUIBinding.Bind(customMessageWindow.TitleTextBlock, "STRING_ENGINE_FAIL_HEADER", "");
          BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlock, "STRING_ENGINE_RESTART", "");
          customMessageWindow.AddButton(ButtonColors.Blue, "STRING_RESTART_ENGINE", this.RestartEngineConfirmationAcceptedHandler, (string) null, false, (object) null, true);
          customMessageWindow.AddButton(ButtonColors.White, "STRING_RESTART_PC", this.RestartPcConfirmationAcceptedHandler, (string) null, false, (object) null, true);
          this.ShowDimOverlay((IDimOverlayControl) null);
          customMessageWindow.Owner = (Window) this.mDimOverlay;
          customMessageWindow.ShowDialog();
          this.HideDimOverlay();
        }
        catch (Exception ex)
        {
          Logger.Error("Error window probably closed");
          Logger.Error(ex.ToString());
        }
      }));
    }

    internal void MainWindow_RestartEngineConfirmationAcceptedHandler(object sender, EventArgs e)
    {
      BlueStacksUIUtils.RestartInstance(this.mVmName, false);
    }

    private void MainWindow_RestartPcConfirmationHandler(object sender, EventArgs e)
    {
      Process.Start("shutdown.exe", "-r -t 0");
    }

    private void WelcomeTabParentGrid_IsVisibleChanged(
      object _1,
      DependencyPropertyChangedEventArgs _2)
    {
      this.mWelcomeTab.Visibility = this.WelcomeTabParentGrid.Visibility;
    }

    private void PikaPopControl_CloseClicked(object sender, EventArgs e)
    {
      this.pikaPop.IsOpen = false;
      this.isPikaPopOpen = false;
    }

    internal void ClosePopUps()
    {
      this.PikaPopControl_CloseClicked((object) this, (EventArgs) null);
      this.mWelcomeTab.mHomeAppManager.CloseHomeAppPopups();
      this.toastPopup.IsOpen = false;
      this.mShootingModePopup.IsOpen = false;
      this.mFullScreenToastPopup.IsOpen = false;
      this.mFullscreenTopbarPopup.IsOpen = false;
      this.mFullscreenTopbarPopupButton.IsOpen = false;
      this.mFullscreenSidebarPopup.IsOpen = false;
      this.mFullscreenSidebarPopupButton.IsOpen = false;
      if (this.mSidebar.mListPopups != null)
      {
        foreach (Popup mListPopup in this.mSidebar.mListPopups)
          mListPopup.IsOpen = false;
      }
      this.AllowFrontendFocusOnClientClick = true;
    }

    private void BackButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      Logger.Info("Clicked back button setup bottombar ");
      this.mCommonHandler.BackButtonHandler(false);
    }

    private void TopBarPopup_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
    {
      if (Oem.IsOEMDmm)
      {
        if (this.mDMMFST.IsMouseOver || this.mDMMFST.mVolumePopup.IsOpen || this.mDMMFST.mChangeTransparencyPopup.IsOpen)
          return;
        this.mTopBarPopup.IsOpen = false;
      }
      else
      {
        if (this.mFullScreenTopBar.mChangeTransparencyPopup.IsOpen)
          return;
        this.mTopBarPopup.IsOpen = false;
      }
    }

    internal void SetMacroPlayBackEventHandle()
    {
      try
      {
        using (EventWaitHandle eventWaitHandle = EventWaitHandle.OpenExisting(BlueStacksUIUtils.GetMacroPlaybackEventName(this.mVmName)))
          eventWaitHandle.Set();
      }
      catch (Exception ex)
      {
        Logger.Warning("Unable to set macro playback event err:" + ex.ToString());
      }
    }

    internal void StartTimerForAppPlayerRestart(int interval)
    {
      this.mMacroTimer = new System.Timers.Timer((double) (interval * 60 * 1000));
      this.mMacroTimer.Elapsed -= new ElapsedEventHandler(this.MacroTimer_Elapsed);
      this.mMacroTimer.Elapsed += new ElapsedEventHandler(this.MacroTimer_Elapsed);
      this.mMacroTimer.Start();
    }

    private void Fullscreentopbar_opened(object sender, EventArgs e)
    {
      if (this.mTopBarPopup.IsOpen)
      {
        this.MouseMove -= new System.Windows.Input.MouseEventHandler(this.MainWindow_MouseMove);
        this.MouseMove += new System.Windows.Input.MouseEventHandler(this.MainWindow_MouseMove);
      }
      else
        this.MouseMove -= new System.Windows.Input.MouseEventHandler(this.MainWindow_MouseMove);
    }

    private void MacroTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
      if (!this.mMacroTimer.Enabled)
        return;
      this.mMacroTimer.Enabled = false;
      this.mMacroTimer.AutoReset = false;
      this.mMacroTimer.Dispose();
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        this.mTopBar.HideMacroPlaybackFromTopBar();
        if (FeatureManager.Instance.IsCustomUIForNCSoft)
          this.mNCTopBar.HideMacroPlaybackFromTopBar();
        this.mIsMacroPlaying = false;
        this.mMacroPlaying = string.Empty;
        BlueStacksUIUtils.RestartInstance(this.mVmName, false);
      }));
    }

    internal void ShowSynchronizerWindow()
    {
      this.mTopBar.mSettingsMenuPopup.IsOpen = false;
      if (this.mSynchronizerWindow == null)
        this.mSynchronizerWindow = new SynchronizerWindow(this);
      this.mSynchronizerWindow.Init(false);
      this.mSynchronizerWindow.Show();
      this.mSynchronizerWindow.ShowWithParentWindow = true;
    }

    private void ReleaseClientGlobalLock()
    {
      try
      {
        if (this.mBlueStacksClientInstanceLock == null)
          return;
        this.mBlueStacksClientInstanceLock.ReleaseMutex();
        this.mBlueStacksClientInstanceLock.Close();
        this.mBlueStacksClientInstanceLock = (Mutex) null;
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in releasing client global lock.." + ex?.ToString());
      }
    }

    private void MainWindow_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
    {
      try
      {
        if (!this.mIsFullScreen || !this.mTopBarPopup.IsOpen || (e.GetPosition((IInputElement) this.mDMMFST).Y <= 80.0 || this.mDMMFST.mChangeTransparencyPopup.IsOpen))
          return;
        this.mTopBarPopup.IsOpen = false;
      }
      catch
      {
      }
    }

    internal void ShowLockScreen()
    {
      if (this.mIsLockScreenActionPending)
        return;
      if (this.EngineInstanceRegistry.IsClientOnTop)
      {
        this.Topmost = false;
        this.EngineInstanceRegistry.IsClientOnTop = false;
      }
      if (this.mDimOverlay != null && this.mDimOverlay.OwnedWindows.Count > 0)
      {
        foreach (Window ownedWindow in this.mDimOverlay.OwnedWindows)
          ownedWindow.Close();
      }
      else if (KMManager.CanvasWindow != null && KMManager.CanvasWindow.SidebarWindow != null && KMManager.CanvasWindow.SidebarWindow.Visibility == Visibility.Visible)
        KMManager.CanvasWindow.SidebarWindow.Close();
      else if (KMManager.sGuidanceWindow != null && !KMManager.sGuidanceWindow.IsClosed && KMManager.sGuidanceWindow.Visibility == Visibility.Visible)
        KMManager.sGuidanceWindow.Close();
      KMManager.ShowOverlayWindow(this, false, false);
      if (this.mMacroRecorderWindow != null)
        this.mCommonHandler.HideMacroRecorderWindow();
      this.mIsLockScreenActionPending = true;
      this.ShowDimOverlay((IDimOverlayControl) this.ScreenLockInstance);
    }

    internal void HideLockScreen()
    {
      if (this.mDimOverlay == null || this.ScreenLockInstance.Visibility != Visibility.Visible)
        return;
      this.mIsLockScreenActionPending = false;
      this.HideDimOverlay();
      this.ShowWindow(false);
      this.Activate();
      if (!RegistryManager.Instance.ShowKeyControlsOverlay || KMManager.CheckIfKeymappingWindowVisible(false))
        return;
      KMManager.ShowOverlayWindow(this, true, false);
    }

    private void UpdateSynchronizationState()
    {
      this._TopBar.HideSyncPanel();
      if (this.mIsSyncMaster)
      {
        this.mSynchronizerWindow.StopAllSyncOperations();
      }
      else
      {
        if (BlueStacksUIUtils.sSyncInvolvedInstances.Contains(this.mVmName))
        {
          HTTPUtils.SendRequestToEngineAsync("stopSyncConsumer", (Dictionary<string, string>) null, this.mVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
          BlueStacksUIUtils.sSyncInvolvedInstances.Remove(this.mVmName);
        }
        foreach (string key in BlueStacksUIUtils.DictWindows.Keys)
        {
          if (key != this.mVmName && BlueStacksUIUtils.DictWindows[key].mSelectedInstancesForSync.Contains(this.mVmName))
          {
            MainWindow dictWindow = BlueStacksUIUtils.DictWindows[key];
            dictWindow.mSelectedInstancesForSync.Remove(this.mVmName);
            if (dictWindow.mSelectedInstancesForSync.Count == 0)
            {
              dictWindow.mIsSynchronisationActive = false;
              dictWindow.mIsSyncMaster = false;
              if (BlueStacksUIUtils.sSyncInvolvedInstances.Contains(dictWindow.mVmName))
                BlueStacksUIUtils.sSyncInvolvedInstances.Remove(dictWindow.mVmName);
              dictWindow._TopBar.HideSyncPanel();
              dictWindow.mFrontendHandler.SendFrontendRequestAsync("stopOperationsSync", new Dictionary<string, string>());
            }
          }
        }
      }
    }

    protected virtual void Dispose(bool disposing)
    {
      if (this.disposedValue)
        return;
      int num = disposing ? 1 : 0;
      this.ReleaseClientGlobalLock();
      if (this.mMacroTimer != null)
      {
        this.mMacroTimer.Elapsed -= new ElapsedEventHandler(this.MacroTimer_Elapsed);
        this.mMacroTimer.Dispose();
        this.mPostOtsWelcomeWindow.Dispose();
      }
      this.mDiscordhandler?.Dispose();
      this.mCommonHandler?.Dispose();
      this.mMacroRecorderWindow?.Dispose();
      this.mUtils?.Dispose();
      this.disposedValue = true;
    }

    ~MainWindow()
    {
      this.Dispose(false);
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    private void mFullscreenSidebarButton_Click(object sender, RoutedEventArgs e)
    {
      this.mSidebar.ToggleSidebarVisibilityInFullscreen(true);
    }

    private void mFullscreenSidebarButton_MouseRightButtonDown(
      object sender,
      MouseButtonEventArgs e)
    {
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        this.mFullscreenSidebarPopupButton.IsPopupEventTransparent = true;
        this.GenerateMouseInput(this.mFrontendHandler.mFrontendHandle, new System.Drawing.Point((int) (MainWindow.sScalingFactor * Mouse.GetPosition((IInputElement) this).X), (int) (MainWindow.sScalingFactor * Mouse.GetPosition((IInputElement) this).Y)), new NativeMethods.Input[1]
        {
          new NativeMethods.Input()
          {
            Type = 0U,
            Data = {
              Mouse = {
                Flags = 8U
              }
            }
          }
        });
      }), DispatcherPriority.Input);
    }

    private void mFullscreenSidebarButton_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        this.mFullscreenSidebarPopupButton.IsPopupEventTransparent = true;
        this.GenerateMouseInput(this.mFrontendHandler.mFrontendHandle, new System.Drawing.Point((int) (MainWindow.sScalingFactor * Mouse.GetPosition((IInputElement) this).X), (int) (MainWindow.sScalingFactor * Mouse.GetPosition((IInputElement) this).Y)), new NativeMethods.Input[1]
        {
          new NativeMethods.Input()
          {
            Type = 0U,
            Data = {
              Mouse = {
                Flags = 16U
              }
            }
          }
        });
      }), DispatcherPriority.Input);
    }

    private void GenerateMouseInput(
      IntPtr wndHandle,
      System.Drawing.Point clientPoint,
      NativeMethods.Input[] inputs)
    {
      try
      {
        if (inputs == null)
          return;
        System.Drawing.Point position = System.Windows.Forms.Cursor.Position;
        NativeMethods.ClientToScreen(wndHandle, ref clientPoint);
        System.Windows.Forms.Cursor.Position = new System.Drawing.Point(clientPoint.X, clientPoint.Y);
        int num = (int) NativeMethods.SendInput((uint) inputs.Length, inputs, Marshal.SizeOf(typeof (NativeMethods.Input)));
        System.Windows.Forms.Cursor.Position = position;
      }
      catch (Exception ex)
      {
        Logger.Warning("Error in generating mouse input, " + ex.ToString());
      }
    }

    private void mFullscreenSidebarPopupButton_OpenedClosed(object sender, EventArgs e)
    {
      this.mFullscreenSidebarPopupButton.IsPopupEventTransparent = false;
    }

    private void SidebarButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      this.mIsSideButtonDragging = true;
      this.mOldSideButtonMargin = (sender as System.Windows.Controls.Button).Margin;
      this.mSideButtonOldPosition = e.GetPosition((IInputElement) this);
    }

    private void SidebarButton_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
    {
      System.Windows.Point position = e.GetPosition((IInputElement) this);
      if (!this.mIsSideButtonDragging || position.Y <= 0.0 || position.Y >= this.mFullscreenSidebarPopupButtonInnerGrid.ActualHeight)
        return;
      this.mFullscreenSidebarButton.Margin = new Thickness(0.0, this.mOldSideButtonMargin.Top + 2.0 * (position.Y - this.mSideButtonOldPosition.Y), 0.0, 0.0);
    }

    private void SidebarButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      if (!this.mIsSideButtonDragging)
        return;
      this.mIsSideButtonDragging = false;
      this.mSideButtonOldPosition = new System.Windows.Point();
    }

    private void mFullscreenTopbarButton_Click(object sender, RoutedEventArgs e)
    {
      this.mTopbarOptions.ToggleTopbarVisibilityInFullscreen(true);
    }

    private void TopbarButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      this.mIsTopButtonDragging = true;
      this.mOldTopButtonMargin = (sender as System.Windows.Controls.Button).Margin;
      this.mTopButtonOldPosition = e.GetPosition((IInputElement) this);
    }

    private void TopbarButton_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
    {
      System.Windows.Point position = e.GetPosition((IInputElement) this);
      if (!this.mIsTopButtonDragging || position.X <= 0.0 || position.X >= this.mFullscreenTopbarPopupButtonInnerGrid.ActualWidth)
        return;
      this.mFullscreenTopbarButton.Margin = new Thickness(this.mOldTopButtonMargin.Left + 2.0 * (position.X - this.mTopButtonOldPosition.X), 0.0, 0.0, 0.0);
    }

    private void TopbarButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      if (!this.mIsTopButtonDragging)
        return;
      this.mIsTopButtonDragging = false;
      this.mTopButtonOldPosition = new System.Windows.Point();
    }

    internal void ShowImageUploadedPopup()
    {
      try
      {
        if (this.mTopBar.mAppTabButtons.SelectedTab.PackageName.Equals("com.lilithgame.roc.gp", StringComparison.InvariantCultureIgnoreCase))
        {
          if (this.mImageUploadedPopup.IsOpen)
          {
            this.mImageUploadedToastTimer.Stop();
            this.mImageUploadedPopup.IsOpen = false;
          }
          this.mImageUploadedControl.Init((Window) this, LocaleStrings.GetLocalizedString("STRING_IMAGE_UPLOADED_INFO", ""), (System.Windows.Media.Brush) System.Windows.Media.Brushes.Black, (System.Windows.Media.Brush) new SolidColorBrush(System.Windows.Media.Color.FromArgb((byte) 85, (byte) 168, (byte) 168, (byte) 168)), System.Windows.HorizontalAlignment.Center, VerticalAlignment.Center, new Thickness?(), 1, new Thickness?(), (System.Windows.Media.Brush) null, true, true);
          this.mImageUploadedControl.AddImage("toast_checked", 23.0, 23.0, new Thickness?(new Thickness(0.0, 0.0, 10.0, 0.0)));
          this.mImageUploadedControl.Visibility = Visibility.Visible;
          this.mImageUploadedPopup.IsOpen = true;
          this.mImageUploadedCanvas.Width = this.mImageUploadedControl.ActualWidth;
          this.mImageUploadedCanvas.Height = this.mImageUploadedControl.ActualHeight;
          this.dummyToast.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
          this.dummyToast.VerticalAlignment = VerticalAlignment.Top;
          this.mImageUploadedPopup.VerticalAlignment = VerticalAlignment.Top;
          this.mImageUploadedPopup.VerticalOffset = this.mImageUploadedControl.ActualHeight + this.mTopBar.ActualHeight + 20.0;
          if (this.mImageUploadedToastTimer.IsEnabled)
            this.mImageUploadedToastTimer.Stop();
          this.mImageUploadedToastTimer.Start();
        }
        if (this.mAppHandler.CurrentActivityName.Equals("com.lilithgame.roc.gp/sh.lilith.lilithchat.activities.ImagePickerActivity", StringComparison.InvariantCultureIgnoreCase) || this.mAppHandler.CurrentActivityName.Equals("com.android.gallery3d /.app.GalleryActivity", StringComparison.InvariantCultureIgnoreCase))
          ClientStats.SendMiscellaneousStatsAsync("img_drag_drop_in_game", RegistryManager.Instance.UserGuid, this.mTopBar.mAppTabButtons.SelectedTab.PackageName, (string) null, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
        else
          ClientStats.SendMiscellaneousStatsAsync("img_upload", RegistryManager.Instance.UserGuid, this.mTopBar.mAppTabButtons.SelectedTab.PackageName, (string) null, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
      }
      catch (Exception ex)
      {
        Logger.Error("Exception showing image uploaded toast: " + ex.ToString());
      }
    }

    internal void ShowPersistentGeneralToast(string message)
    {
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        try
        {
          if (!this.mIsWindowInFocus)
            return;
          if (this.mGeneraltoast.IsOpen)
            this.mGeneraltoast.IsOpen = false;
          this.mGeneraltoastControl.Init((Window) this, message, BlueStacksUIBinding.Instance.ColorModel["ContextMenuItemBackgroundColor"], (System.Windows.Media.Brush) new SolidColorBrush(System.Windows.Media.Color.FromArgb((byte) 85, byte.MaxValue, byte.MaxValue, byte.MaxValue)), System.Windows.HorizontalAlignment.Center, VerticalAlignment.Top, new Thickness?(), 1, new Thickness?(new Thickness(24.0, 12.0, 12.0, 12.0)), (System.Windows.Media.Brush) null, true, false);
          this.dummyToast.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
          this.dummyToast.VerticalAlignment = VerticalAlignment.Top;
          this.mGeneraltoastControl.Visibility = Visibility.Visible;
          this.mGeneraltoast.IsOpen = true;
          this.mGeneraltoastCanvas.Height = this.mGeneraltoastControl.ActualHeight;
          this.mGeneraltoast.VerticalOffset = this.mGeneraltoastControl.ActualHeight + 20.0;
          this.mGeneraltoast.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
        }
        catch (Exception ex)
        {
          Logger.Error("Exception in showing general persistent toast popup : " + ex.ToString());
        }
      }));
    }

    private void HandleShowWindowForFarmingInstance()
    {
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        if (string.IsNullOrEmpty(this.WindowLaunchParams))
          return;
        JObject jobject = JObject.Parse(this.WindowLaunchParams);
        if (jobject["isFarmingInstance"] == null)
          return;
        if (jobject["source_vmname"] != null && BlueStacksUIUtils.DictWindows.ContainsKey(jobject["source_vmname"].ToString()) && BlueStacksUIUtils.DictWindows[jobject["source_vmname"].ToString()].mTopBar.mAppTabButtons.SelectedTab.OnboardingControl != null)
        {
          BlueStacksUIUtils.DictWindows[jobject["source_vmname"].ToString()].mTopBar.mAppTabButtons.SelectedTab.OnboardingControl?.Close();
          BlueStacksUIUtils.DictWindows[jobject["source_vmname"].ToString()].HideDimOverlay();
        }
        this.ShowWindow(false);
        if (!this.EngineInstanceRegistry.IsSidebarVisible || this.Visibility != Visibility.Visible || (this.mSidebar.Visibility == Visibility.Visible || Oem.IsOEMDmm))
          return;
        if (this.mTopBar.mSidebarButton.ImageName == "sidebar_hide")
        {
          this.mSidebar.Visibility = Visibility.Visible;
          this.EngineInstanceRegistry.IsSidebarVisible = true;
        }
        else
        {
          this.mSidebar.Visibility = Visibility.Collapsed;
          this.EngineInstanceRegistry.IsSidebarVisible = true;
        }
        this.mSidebar.SidebarVisiblityChanged(this.mSidebar.Visibility);
      }));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      System.Windows.Application.LoadComponent((object) this, new Uri("/Bluestacks;component/mainwindow.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    internal Delegate _CreateDelegate(System.Type delegateType, string handler)
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
          this.mMainWindow = (MainWindow) target;
          this.mMainWindow.IsVisibleChanged += new DependencyPropertyChangedEventHandler(this.MainWindow_IsVisibleChanged);
          this.mMainWindow.StateChanged += new EventHandler(this.MainWindow_StateChanged);
          this.mMainWindow.Activated += new EventHandler(this.MainWindow_Activated);
          this.mMainWindow.Deactivated += new EventHandler(this.MainWindow_Deactivated);
          this.mMainWindow.PreviewMouseMove += new System.Windows.Input.MouseEventHandler(this.MainWindow_PreviewMouseMove);
          this.mMainWindow.ContentRendered += new EventHandler(this.MainWindow_ContentRendered);
          this.mMainWindow.Loaded += new RoutedEventHandler(this.MainWindow_Loaded);
          this.mMainWindow.Closing += new CancelEventHandler(this.MainWindow_Closing);
          this.mMainWindow.SourceInitialized += new EventHandler(this.MainWindow_SourceInitialized);
          break;
        case 3:
          this.OuterBorder = (Border) target;
          break;
        case 4:
          this.MainGrid = (Grid) target;
          break;
        case 5:
          this.pikaPop = (CustomPopUp) target;
          break;
        case 6:
          this.pikaCanvas = (Canvas) target;
          break;
        case 7:
          this.pikaPopControl = (PikaNotificationControl) target;
          break;
        case 8:
          this.toastPopup = (CustomPopUp) target;
          break;
        case 9:
          this.toastCanvas = (Canvas) target;
          break;
        case 10:
          this.toastControl = (CustomToastPopupControl) target;
          break;
        case 11:
          this.mFullScreenToastPopup = (CustomPopUp) target;
          break;
        case 12:
          this.mFullScreenToastCanvas = (Canvas) target;
          break;
        case 13:
          this.mFullScreenToastControl = (FullScreenToastPopupControl) target;
          break;
        case 14:
          this.mUnlockMouseToastPopup = (CustomPopUp) target;
          break;
        case 15:
          this.mUnlockMouseToastCanvas = (Canvas) target;
          break;
        case 16:
          this.mUnlockMouseToastControl = (UnlockMouseToastPopupControl) target;
          break;
        case 17:
          this.mConfigUpdatedPopup = (CustomPopUp) target;
          break;
        case 18:
          this.mConfigUpdatedCanvas = (Canvas) target;
          break;
        case 19:
          this.mConfigUpdatedControl = (CustomToastPopupControl) target;
          break;
        case 20:
          this.mImageUploadedPopup = (CustomPopUp) target;
          break;
        case 21:
          this.mImageUploadedCanvas = (Canvas) target;
          break;
        case 22:
          this.mImageUploadedControl = (CustomToastPopupControl) target;
          break;
        case 23:
          this.mGeneraltoast = (CustomPopUp) target;
          break;
        case 24:
          this.mGeneraltoastCanvas = (Canvas) target;
          break;
        case 25:
          this.mGeneraltoastControl = (CustomToastPopupControl) target;
          break;
        case 26:
          this.mShootingModePopup = (CustomPopUp) target;
          break;
        case 27:
          this.mShootingModePopupCanvas = (Canvas) target;
          break;
        case 28:
          this.mToastControl = (CustomPersistentToastPopupControl) target;
          break;
        case 29:
          this.mTopBarPopup = (CustomPopUp) target;
          break;
        case 30:
          this.mFullScreenTopBar = (FullScreenTopBar) target;
          break;
        case 31:
          this.mFullscreenSidebarPopupButton = (CustomPopUp) target;
          break;
        case 32:
          this.mFullscreenSidebarPopupButtonInnerGrid = (Grid) target;
          break;
        case 33:
          this.mFullscreenSidebarButton = (System.Windows.Controls.Button) target;
          this.mFullscreenSidebarButton.Click += new RoutedEventHandler(this.mFullscreenSidebarButton_Click);
          this.mFullscreenSidebarButton.MouseRightButtonDown += new MouseButtonEventHandler(this.mFullscreenSidebarButton_MouseRightButtonDown);
          this.mFullscreenSidebarButton.MouseRightButtonUp += new MouseButtonEventHandler(this.mFullscreenSidebarButton_MouseRightButtonUp);
          this.mFullscreenSidebarButton.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(this.SidebarButton_PreviewMouseLeftButtonDown);
          this.mFullscreenSidebarButton.PreviewMouseMove += new System.Windows.Input.MouseEventHandler(this.SidebarButton_MouseMove);
          this.mFullscreenSidebarButton.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.SidebarButton_PreviewMouseLeftButtonUp);
          break;
        case 34:
          this.mFullscreenSidebarPopup = (CustomPopUp) target;
          break;
        case 35:
          this.mFullscreenSidebarPopupInnerGrid = (Grid) target;
          break;
        case 36:
          this.mFullscreenTopbarPopupButton = (CustomPopUp) target;
          break;
        case 37:
          this.mFullscreenTopbarPopupButtonInnerGrid = (Grid) target;
          break;
        case 38:
          this.mFullscreenTopbarButton = (System.Windows.Controls.Button) target;
          this.mFullscreenTopbarButton.Click += new RoutedEventHandler(this.mFullscreenTopbarButton_Click);
          this.mFullscreenTopbarButton.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(this.TopbarButton_PreviewMouseLeftButtonDown);
          this.mFullscreenTopbarButton.PreviewMouseMove += new System.Windows.Input.MouseEventHandler(this.TopbarButton_MouseMove);
          this.mFullscreenTopbarButton.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.TopbarButton_PreviewMouseLeftButtonUp);
          break;
        case 39:
          this.mFullscreenTopbarPopup = (CustomPopUp) target;
          break;
        case 40:
          this.mFullscreenTopbarPopupInnerGrid = (Grid) target;
          break;
        case 41:
          this.mTopbarOptions = (TopbarOptions) target;
          break;
        case 42:
          this.mMainWindowTopGrid = (Grid) target;
          break;
        case 43:
          this.mTopBar = (BlueStacks.BlueStacksUI.TopBar) target;
          break;
        case 44:
          this.mNCTopBar = (NCSoftTopBar) target;
          break;
        case 45:
          this.mFrontendOTSControl = (FrontendOTSControl) target;
          break;
        case 46:
          this.dummyPika = (Grid) target;
          break;
        case 47:
          this.mContentGrid = (Grid) target;
          break;
        case 48:
          this.WelcomeTabParentGrid = (Grid) target;
          this.WelcomeTabParentGrid.IsVisibleChanged += new DependencyPropertyChangedEventHandler(this.WelcomeTabParentGrid_IsVisibleChanged);
          break;
        case 49:
          this.mWelcomeTab = (WelcomeTab) target;
          break;
        case 50:
          this.FrontendParentGrid = (Grid) target;
          this.FrontendParentGrid.IsVisibleChanged += new DependencyPropertyChangedEventHandler(this.FrontendParentGrid_IsVisibleChanged);
          break;
        case 51:
          this.mDmmProgressControl = (DMMProgressControl) target;
          break;
        case 52:
          this.mFrontendGrid = (Grid) target;
          this.mFrontendGrid.IsVisibleChanged += new DependencyPropertyChangedEventHandler(this.FrontendGrid_IsVisibleChanged);
          this.mFrontendGrid.SizeChanged += new SizeChangedEventHandler(this.FrontendGrid_SizeChanged);
          break;
        case 53:
          this.mDmmBottomBar = (DMMBottomBar) target;
          break;
        case 54:
          this.dummyToast2 = (Grid) target;
          break;
        case 55:
          this.mSidebar = (Sidebar) target;
          break;
        case 56:
          this.dummyToast = (Grid) target;
          break;
        case 57:
          this.dummyTooltip = (Grid) target;
          break;
        case 58:
          this.mExitProgressGrid = (ProgressBar) target;
          break;
        case 59:
          this.mQuitPopupBrowserLoadGrid = (Grid) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IStyleConnector.Connect(int connectionId, object target)
    {
      if (connectionId != 2)
        return;
      ((FrameworkElement) target).Loaded += new RoutedEventHandler(this.ResizeGrid_Loaded);
    }

    public delegate void GuestBootCompletedEventHandler(object sender, EventArgs args);

    internal delegate void CursorLockChangedEventHandler(
      object sender,
      MainWindowEventArgs.CursorLockChangedEventArgs args);

    internal delegate void FullScreenChangedEventHandler(
      object sender,
      MainWindowEventArgs.FullScreenChangedEventArgs args);

    internal delegate void FrontendGridVisibilityChangedEventHandler(
      object sender,
      MainWindowEventArgs.FrontendGridVisibilityChangedEventArgs args);

    internal delegate void BrowserOTSCompletedCallbackEventHandler(
      object sender,
      MainWindowEventArgs.BrowserOTSCompletedCallbackEventArgs args);
  }
}
