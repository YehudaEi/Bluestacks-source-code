// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.NCSoftTopBar
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Shapes;

namespace BlueStacks.BlueStacksUI
{
  public class NCSoftTopBar : UserControl, ITopBar, IComponentConnector
  {
    private MainWindow mMainWindow;
    internal Grid mMainGrid;
    internal CustomPictureBox mTitleIcon;
    internal StackPanel mWindowHeaderGrid;
    internal TextBlock mAppName;
    internal Line mGamenameSeparator;
    internal TextBlock mCharacterName;
    internal Grid mStreamingTopbarGrid;
    internal Border mBorder;
    internal Grid mNcTopBarControlGrid;
    internal Grid mMacroRecordGrid;
    internal MacroTopBarRecordControl mMacroRecordControl;
    internal Grid mMacroPlayGrid;
    internal MacroTopBarPlayControl mMacroPlayControl;
    internal Grid mVideoRecordingStatusGrid;
    internal VideoRecordingStatus mVideoRecordStatusControl;
    internal Grid mOperationsSyncGrid;
    internal Border mSyncMaskBorder;
    internal CustomPictureBox mStopSyncButton;
    internal StackPanel mControlBtnPanel;
    internal Grid mSettingsButton;
    internal CustomPictureBox mSettingsButtonImage;
    internal TextBlock mSettingsButtonText;
    internal Grid mMinimizeButton;
    internal CustomPictureBox mMinimizeButtonImage;
    internal TextBlock mMinimizeButtonText;
    internal Grid mMaximizeButton;
    internal CustomPictureBox mMaximizeButtonImage;
    internal TextBlock mMaximizeButtonText;
    internal Grid mCloseButton;
    internal CustomPictureBox mCloseButtonImage;
    internal TextBlock mCloseButtonText;
    internal Grid mSidebarButton;
    internal CustomPictureBox mSidebarButtonImage;
    internal TextBlock mSidebarButtonText;
    internal BlueStacks.Common.CustomPopUp mMacroRecorderToolTipPopup;
    internal Grid dummyGrid;
    internal TextBlock mMacroRecordingTooltip;
    internal Path mUpArrow;
    internal BlueStacks.Common.CustomPopUp mMacroRunningToolTipPopup;
    internal Grid grid;
    internal TextBlock mMacroRunningTooltip;
    internal BlueStacks.Common.CustomPopUp mSettingsDropdownPopup;
    internal Border mSettingsDropdownBorder;
    internal Grid mGrid;
    internal Border mMaskBorder;
    internal SettingsWindowDropdown mSettingsDropDownControl;
    internal CustomPopUp mSyncInstancesToolTipPopup;
    internal Grid mDummyGrid;
    internal Path mUpwardArrow;
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

    string ITopBar.AppName
    {
      get
      {
        return this.mAppName.Text;
      }
      set
      {
        this.mAppName.Text = value;
      }
    }

    string ITopBar.CharacterName
    {
      get
      {
        return this.mCharacterName.Text;
      }
      set
      {
        this.mCharacterName.Text = value;
      }
    }

    public NCSoftTopBar()
    {
      this.InitializeComponent();
    }

    public void ChangeTopBarColor(string color)
    {
      BlueStacksUIBinding.BindColor((DependencyObject) this, Control.BackgroundProperty, color);
    }

    private void ParentWindow_GuestBootCompletedEvent(object sender, EventArgs args)
    {
      if (!this.ParentWindow.EngineInstanceRegistry.IsSidebarVisible || this.Visibility != Visibility.Visible || (this.ParentWindow.mSidebar.Visibility == Visibility.Visible || Oem.IsOEMDmm))
        return;
      this.ParentWindow.Dispatcher.Invoke((Delegate) (() => this.ParentWindow.mCommonHandler.FlipSidebarVisibility(this.mSidebarButtonImage, this.mSidebarButtonText)));
    }

    private void NCSoftTopBar_Loaded(object sender, RoutedEventArgs e)
    {
      if (!this.ParentWindow.mGuestBootCompleted)
      {
        this.ParentWindow.mCommonHandler.SetSidebarImageProperties(false, this.mSidebarButtonImage, this.mSidebarButtonText);
        this.ParentWindow.GuestBootCompleted += new MainWindow.GuestBootCompletedEventHandler(this.ParentWindow_GuestBootCompletedEvent);
      }
      this.ParentWindow.mCommonHandler.ScreenRecordingStateChangedEvent += new CommonHandlers.ScreenRecordingStateChanged(this.NCTopBar_ScreenRecordingStateChangedEvent);
      this.mVideoRecordStatusControl.RecordingStoppedEvent += new System.Action(this.NCTopBar_RecordingStoppedEvent);
    }

    private void NCTopBar_ScreenRecordingStateChangedEvent(bool isRecording)
    {
      this.ParentWindow.Dispatcher.Invoke((Delegate) (() =>
      {
        if (isRecording)
        {
          if (this.mVideoRecordingStatusGrid.Visibility == Visibility.Visible || !CommonHandlers.sIsRecordingVideo)
            return;
          this.mVideoRecordStatusControl.Init(this.ParentWindow);
          this.mVideoRecordingStatusGrid.Visibility = Visibility.Visible;
        }
        else
        {
          this.mVideoRecordStatusControl.ResetTimer();
          this.mVideoRecordingStatusGrid.Visibility = Visibility.Collapsed;
        }
      }));
    }

    private void NCTopBar_RecordingStoppedEvent()
    {
      this.ParentWindow.Dispatcher.Invoke((Delegate) (() => this.mVideoRecordingStatusGrid.Visibility = Visibility.Collapsed));
    }

    private void MinimizeButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      Logger.Info("Clicked minimize button");
      this.ParentWindow.MinimizeWindow();
    }

    internal void MaxmizeButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      Logger.Info("Clicked Maximize\\Restore button");
      if (this.ParentWindow.WindowState == WindowState.Normal)
        this.ParentWindow.MaximizeWindow();
      else
        this.ParentWindow.RestoreWindows(false);
    }

    private void CloseButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      Logger.Info("Clicked close Bluestacks button");
      this.ParentWindow.CloseWindow();
    }

    private void SettingsButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      if ((sender as Grid).Children[0].Visibility == Visibility.Visible)
      {
        this.mSettingsDropDownControl.LateInit();
        this.mSettingsDropdownPopup.IsOpen = true;
        this.mSettingsButtonImage.ImageName = "cfgmenu_selected";
      }
      else
        this.ParentWindow.mCommonHandler.LaunchSettingsWindow("");
    }

    private void PinOnTop_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      if (!this.ParentWindow.EngineInstanceRegistry.IsClientOnTop)
      {
        this.ParentWindow.EngineInstanceRegistry.IsClientOnTop = true;
        this.ParentWindow.Topmost = true;
      }
      else
      {
        this.ParentWindow.EngineInstanceRegistry.IsClientOnTop = false;
        this.ParentWindow.Topmost = false;
      }
    }

    private void MSidebarButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.ParentWindow.mCommonHandler.FlipSidebarVisibility(this.mSidebarButtonImage, this.mSidebarButtonText);
    }

    internal void ShowMacroPlaybackOnTopBar(MacroRecording record)
    {
      if (this.ParentWindow.IsUIInPortraitMode)
        this.mSettingsButton.Visibility = Visibility.Collapsed;
      this.mMacroPlayControl.Init(this.ParentWindow, record);
      this.mMacroPlayGrid.Visibility = Visibility.Visible;
    }

    internal void HideMacroPlaybackFromTopBar()
    {
      this.mSettingsButton.Visibility = Visibility.Visible;
      this.mMacroPlayGrid.Visibility = Visibility.Collapsed;
    }

    internal void ShowRecordingIcons()
    {
      if (this.ParentWindow.IsUIInPortraitMode)
        this.mSettingsButton.Visibility = Visibility.Collapsed;
      this.mMacroRecordControl.Init(this.ParentWindow);
      this.mMacroRecordGrid.Visibility = Visibility.Visible;
      this.mMacroRecordControl.StartTimer();
    }

    internal void HideRecordingIcons()
    {
      this.mSettingsButton.Visibility = Visibility.Visible;
      this.mMacroRecordGrid.Visibility = Visibility.Collapsed;
      this.mMacroRecorderToolTipPopup.IsOpen = false;
    }

    private void NCSoftTopBar_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      DesignerProperties.GetIsInDesignMode((DependencyObject) this);
    }

    private void SettingsDropDownControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      e.Handled = true;
    }

    private void SettingsPopup_Opened(object sender, EventArgs e)
    {
      this.mSettingsDropdownPopup.IsEnabled = false;
      this.mSettingsButtonImage.ImageName = "cfgmenu";
    }

    private void SettingsPopup_Closed(object sender, EventArgs e)
    {
      this.mSettingsDropdownPopup.IsEnabled = true;
      this.mSettingsButtonImage.ImageName = "cfgmenu";
    }

    void ITopBar.ShowSyncPanel(bool isSource)
    {
      this.mOperationsSyncGrid.Visibility = Visibility.Visible;
      if (!isSource)
        return;
      this.mStopSyncButton.Visibility = Visibility.Visible;
    }

    void ITopBar.HideSyncPanel()
    {
      this.mOperationsSyncGrid.Visibility = Visibility.Collapsed;
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

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/ncsofttopbar.xaml", UriKind.Relative));
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
          ((FrameworkElement) target).Loaded += new RoutedEventHandler(this.NCSoftTopBar_Loaded);
          ((FrameworkElement) target).SizeChanged += new SizeChangedEventHandler(this.NCSoftTopBar_SizeChanged);
          break;
        case 2:
          this.mMainGrid = (Grid) target;
          break;
        case 3:
          this.mTitleIcon = (CustomPictureBox) target;
          break;
        case 4:
          this.mWindowHeaderGrid = (StackPanel) target;
          break;
        case 5:
          this.mAppName = (TextBlock) target;
          break;
        case 6:
          this.mGamenameSeparator = (Line) target;
          break;
        case 7:
          this.mCharacterName = (TextBlock) target;
          break;
        case 8:
          this.mStreamingTopbarGrid = (Grid) target;
          break;
        case 9:
          this.mBorder = (Border) target;
          break;
        case 10:
          this.mNcTopBarControlGrid = (Grid) target;
          break;
        case 11:
          this.mMacroRecordGrid = (Grid) target;
          break;
        case 12:
          this.mMacroRecordControl = (MacroTopBarRecordControl) target;
          break;
        case 13:
          this.mMacroPlayGrid = (Grid) target;
          break;
        case 14:
          this.mMacroPlayControl = (MacroTopBarPlayControl) target;
          break;
        case 15:
          this.mVideoRecordingStatusGrid = (Grid) target;
          break;
        case 16:
          this.mVideoRecordStatusControl = (VideoRecordingStatus) target;
          break;
        case 17:
          this.mOperationsSyncGrid = (Grid) target;
          this.mOperationsSyncGrid.MouseEnter += new MouseEventHandler(this.OperationsSyncGrid_MouseEnter);
          this.mOperationsSyncGrid.MouseLeave += new MouseEventHandler(this.OperationsSyncGrid_MouseLeave);
          break;
        case 18:
          this.mSyncMaskBorder = (Border) target;
          break;
        case 19:
          this.mStopSyncButton = (CustomPictureBox) target;
          this.mStopSyncButton.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.StopSyncButton_PreviewMouseLeftButtonUp);
          break;
        case 20:
          this.mControlBtnPanel = (StackPanel) target;
          break;
        case 21:
          this.mSettingsButton = (Grid) target;
          this.mSettingsButton.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.SettingsButton_MouseLeftButtonUp);
          break;
        case 22:
          this.mSettingsButtonImage = (CustomPictureBox) target;
          break;
        case 23:
          this.mSettingsButtonText = (TextBlock) target;
          break;
        case 24:
          this.mMinimizeButton = (Grid) target;
          this.mMinimizeButton.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.MinimizeButton_MouseLeftButtonUp);
          break;
        case 25:
          this.mMinimizeButtonImage = (CustomPictureBox) target;
          break;
        case 26:
          this.mMinimizeButtonText = (TextBlock) target;
          break;
        case 27:
          this.mMaximizeButton = (Grid) target;
          this.mMaximizeButton.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.MaxmizeButton_MouseLeftButtonUp);
          break;
        case 28:
          this.mMaximizeButtonImage = (CustomPictureBox) target;
          break;
        case 29:
          this.mMaximizeButtonText = (TextBlock) target;
          break;
        case 30:
          this.mCloseButton = (Grid) target;
          this.mCloseButton.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.CloseButton_MouseLeftButtonUp);
          break;
        case 31:
          this.mCloseButtonImage = (CustomPictureBox) target;
          break;
        case 32:
          this.mCloseButtonText = (TextBlock) target;
          break;
        case 33:
          this.mSidebarButton = (Grid) target;
          this.mSidebarButton.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.MSidebarButton_MouseLeftButtonUp);
          break;
        case 34:
          this.mSidebarButtonImage = (CustomPictureBox) target;
          break;
        case 35:
          this.mSidebarButtonText = (TextBlock) target;
          break;
        case 36:
          this.mMacroRecorderToolTipPopup = (BlueStacks.Common.CustomPopUp) target;
          break;
        case 37:
          this.dummyGrid = (Grid) target;
          break;
        case 38:
          this.mMacroRecordingTooltip = (TextBlock) target;
          break;
        case 39:
          this.mUpArrow = (Path) target;
          break;
        case 40:
          this.mMacroRunningToolTipPopup = (BlueStacks.Common.CustomPopUp) target;
          break;
        case 41:
          this.grid = (Grid) target;
          break;
        case 42:
          this.mMacroRunningTooltip = (TextBlock) target;
          break;
        case 43:
          this.mSettingsDropdownPopup = (BlueStacks.Common.CustomPopUp) target;
          this.mSettingsDropdownPopup.Opened += new EventHandler(this.SettingsPopup_Opened);
          this.mSettingsDropdownPopup.Closed += new EventHandler(this.SettingsPopup_Closed);
          break;
        case 44:
          this.mSettingsDropdownBorder = (Border) target;
          break;
        case 45:
          this.mGrid = (Grid) target;
          break;
        case 46:
          this.mMaskBorder = (Border) target;
          break;
        case 47:
          this.mSettingsDropDownControl = (SettingsWindowDropdown) target;
          break;
        case 48:
          this.mSyncInstancesToolTipPopup = (CustomPopUp) target;
          break;
        case 49:
          this.mDummyGrid = (Grid) target;
          break;
        case 50:
          this.mUpwardArrow = (Path) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
