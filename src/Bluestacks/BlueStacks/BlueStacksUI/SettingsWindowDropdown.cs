// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.SettingsWindowDropdown
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
using System.Windows.Media;

namespace BlueStacks.BlueStacksUI
{
  public class SettingsWindowDropdown : UserControl, IComponentConnector
  {
    private MainWindow ParentWindow;
    internal Grid mPinOnTopButtonGrid;
    internal CustomPictureBox mPinOnTopButtonImage;
    internal TextBlock mPinOnTopButtonText;
    internal CustomPictureBox mPinOnTopToggleButton;
    internal Grid mFullScreenButtonGrid;
    internal CustomPictureBox mFullScreenImage;
    internal TextBlock mFullScreenButtonText;
    internal Grid mSyncOperationsButtonGrid;
    internal CustomPictureBox mSyncOperationsButtonImage;
    internal TextBlock mSyncOperationsButtonText;
    internal Grid mSortingGrid;
    internal CustomPictureBox mSortingButtonImage;
    internal TextBlock mSortingButtonText;
    internal Grid mAccountGrid;
    internal CustomPictureBox mAccountButtonImage;
    internal TextBlock mAccountButtonText;
    internal Grid mSettingsButtonGrid;
    internal CustomPictureBox mSettingsButtonImage;
    internal TextBlock mSettingsButtonText;
    private bool _contentLoaded;

    public SettingsWindowDropdown()
    {
      this.InitializeComponent();
    }

    internal void Init(MainWindow window)
    {
      this.ParentWindow = window;
    }

    private void Grid_MouseEnter(object sender, MouseEventArgs e)
    {
      BlueStacksUIBinding.BindColor((DependencyObject) (sender as Grid), Panel.BackgroundProperty, "ContextMenuItemBackgroundHoverColor");
      foreach (UIElement child in (sender as Grid).Children)
      {
        if (child is CustomPictureBox)
          (child as CustomPictureBox).ImageName = (child as CustomPictureBox).ImageName + "_hover";
        if (child is TextBlock)
          BlueStacksUIBinding.BindColor((DependencyObject) (child as TextBlock), Control.ForegroundProperty, "SettingsWindowTabMenuItemSelectedForeground");
      }
    }

    private void Grid_MouseLeave(object sender, MouseEventArgs e)
    {
      (sender as Grid).Background = (Brush) Brushes.Transparent;
      foreach (UIElement child in (sender as Grid).Children)
      {
        if (child is CustomPictureBox)
          (child as CustomPictureBox).ImageName = (child as CustomPictureBox).ImageName.Replace("_hover", "");
        if (child is TextBlock)
          BlueStacksUIBinding.BindColor((DependencyObject) (child as TextBlock), Control.ForegroundProperty, "SettingsWindowTabMenuItemForeground");
      }
    }

    private void SettingsButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.ParentWindow.mNCTopBar.mSettingsDropdownPopup.IsOpen = false;
      this.ParentWindow.mCommonHandler.LaunchSettingsWindow("");
    }

    private void FullscreenButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.ParentWindow.mNCTopBar.mSettingsDropdownPopup.IsOpen = false;
      if (!this.ParentWindow.mResizeHandler.IsMinMaxEnabled)
        return;
      this.ParentWindow.Dispatcher.Invoke((Delegate) (() =>
      {
        if (!Oem.IsOEMDmm && this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.mTabType != TabType.AppTab)
          return;
        if (this.ParentWindow.mIsFullScreen)
          this.ParentWindow.RestoreWindows(false);
        else
          this.ParentWindow.FullScreenWindow();
      }));
    }

    private void SortingButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.ParentWindow.mNCTopBar.mSettingsDropdownPopup.IsOpen = false;
      CommonHandlers.ArrangeWindow();
    }

    private void AccountButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.ParentWindow.mNCTopBar.mSettingsDropdownPopup.IsOpen = false;
      if (!this.ParentWindow.mGuestBootCompleted)
        return;
      this.ParentWindow.mTopBar.mAppTabButtons.AddAppTab("STRING_ACCOUNT", BlueStacksUIUtils.sAndroidSettingsPackageName, BlueStacksUIUtils.sAndroidAccountSettingsActivityName, "account_tab", true, true, false);
    }

    private void PinOnTop_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      CustomPictureBox customPictureBox = sender as CustomPictureBox;
      if (customPictureBox.ImageName.Contains("_off"))
      {
        customPictureBox.ImageName = "toggle_on";
        this.ParentWindow.EngineInstanceRegistry.IsClientOnTop = true;
        this.ParentWindow.Topmost = true;
      }
      else
      {
        customPictureBox.ImageName = "toggle_off";
        this.ParentWindow.EngineInstanceRegistry.IsClientOnTop = false;
        this.ParentWindow.Topmost = false;
      }
    }

    internal void LateInit()
    {
      if (BlueStacksUIUtils.DictWindows.Keys.Count == 1)
      {
        this.mSyncOperationsButtonGrid.PreviewMouseLeftButtonUp -= new MouseButtonEventHandler(this.SyncOperationsButton_PreviewMouseLeftButtonUp);
        this.mSyncOperationsButtonGrid.MouseEnter -= new MouseEventHandler(this.Grid_MouseEnter);
        this.mSyncOperationsButtonGrid.Opacity = 0.5;
        this.mSortingGrid.PreviewMouseLeftButtonUp -= new MouseButtonEventHandler(this.SortingButton_MouseLeftButtonUp);
        this.mSortingGrid.MouseEnter -= new MouseEventHandler(this.Grid_MouseEnter);
        this.mSortingGrid.Opacity = 0.5;
      }
      else
      {
        this.mSortingGrid.PreviewMouseLeftButtonUp -= new MouseButtonEventHandler(this.SortingButton_MouseLeftButtonUp);
        this.mSortingGrid.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.SortingButton_MouseLeftButtonUp);
        this.mSortingGrid.MouseEnter -= new MouseEventHandler(this.Grid_MouseEnter);
        this.mSortingGrid.MouseEnter += new MouseEventHandler(this.Grid_MouseEnter);
        this.mSortingGrid.Opacity = 1.0;
        if (BlueStacksUIUtils.sSyncInvolvedInstances.Contains(this.ParentWindow.mVmName) && this.ParentWindow.mIsSyncMaster || !BlueStacksUIUtils.sSyncInvolvedInstances.Contains(this.ParentWindow.mVmName) && BlueStacksUIUtils.DictWindows.Keys.Count - BlueStacksUIUtils.sSyncInvolvedInstances.Count > 1)
        {
          this.mSyncOperationsButtonGrid.PreviewMouseLeftButtonUp -= new MouseButtonEventHandler(this.SyncOperationsButton_PreviewMouseLeftButtonUp);
          this.mSyncOperationsButtonGrid.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.SyncOperationsButton_PreviewMouseLeftButtonUp);
          this.mSyncOperationsButtonGrid.MouseEnter -= new MouseEventHandler(this.Grid_MouseEnter);
          this.mSyncOperationsButtonGrid.MouseEnter += new MouseEventHandler(this.Grid_MouseEnter);
          this.mSyncOperationsButtonGrid.Opacity = 1.0;
        }
        else
        {
          this.mSyncOperationsButtonGrid.PreviewMouseLeftButtonUp -= new MouseButtonEventHandler(this.SyncOperationsButton_PreviewMouseLeftButtonUp);
          this.mSyncOperationsButtonGrid.MouseEnter -= new MouseEventHandler(this.Grid_MouseEnter);
          this.mSyncOperationsButtonGrid.Opacity = 0.5;
        }
      }
      if (this.ParentWindow.EngineInstanceRegistry.IsClientOnTop)
        this.mPinOnTopToggleButton.ImageName = "toggle_on";
      else
        this.mPinOnTopToggleButton.ImageName = "toggle_off";
    }

    private void SyncOperationsButton_PreviewMouseLeftButtonUp(
      object sender,
      MouseButtonEventArgs e)
    {
      this.ParentWindow.mNCTopBar.mSettingsDropdownPopup.IsOpen = false;
      this.ParentWindow.ShowSynchronizerWindow();
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/ncsettingsdropdown.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.mPinOnTopButtonGrid = (Grid) target;
          this.mPinOnTopButtonGrid.MouseEnter += new MouseEventHandler(this.Grid_MouseEnter);
          this.mPinOnTopButtonGrid.MouseLeave += new MouseEventHandler(this.Grid_MouseLeave);
          break;
        case 2:
          this.mPinOnTopButtonImage = (CustomPictureBox) target;
          break;
        case 3:
          this.mPinOnTopButtonText = (TextBlock) target;
          break;
        case 4:
          this.mPinOnTopToggleButton = (CustomPictureBox) target;
          this.mPinOnTopToggleButton.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.PinOnTop_MouseLeftButtonUp);
          break;
        case 5:
          this.mFullScreenButtonGrid = (Grid) target;
          this.mFullScreenButtonGrid.MouseEnter += new MouseEventHandler(this.Grid_MouseEnter);
          this.mFullScreenButtonGrid.MouseLeave += new MouseEventHandler(this.Grid_MouseLeave);
          this.mFullScreenButtonGrid.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.FullscreenButton_MouseLeftButtonUp);
          break;
        case 6:
          this.mFullScreenImage = (CustomPictureBox) target;
          break;
        case 7:
          this.mFullScreenButtonText = (TextBlock) target;
          break;
        case 8:
          this.mSyncOperationsButtonGrid = (Grid) target;
          this.mSyncOperationsButtonGrid.MouseLeave += new MouseEventHandler(this.Grid_MouseLeave);
          this.mSyncOperationsButtonGrid.MouseEnter += new MouseEventHandler(this.Grid_MouseEnter);
          this.mSyncOperationsButtonGrid.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.SyncOperationsButton_PreviewMouseLeftButtonUp);
          break;
        case 9:
          this.mSyncOperationsButtonImage = (CustomPictureBox) target;
          break;
        case 10:
          this.mSyncOperationsButtonText = (TextBlock) target;
          break;
        case 11:
          this.mSortingGrid = (Grid) target;
          this.mSortingGrid.MouseEnter += new MouseEventHandler(this.Grid_MouseEnter);
          this.mSortingGrid.MouseLeave += new MouseEventHandler(this.Grid_MouseLeave);
          this.mSortingGrid.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.SortingButton_MouseLeftButtonUp);
          break;
        case 12:
          this.mSortingButtonImage = (CustomPictureBox) target;
          break;
        case 13:
          this.mSortingButtonText = (TextBlock) target;
          break;
        case 14:
          this.mAccountGrid = (Grid) target;
          this.mAccountGrid.MouseEnter += new MouseEventHandler(this.Grid_MouseEnter);
          this.mAccountGrid.MouseLeave += new MouseEventHandler(this.Grid_MouseLeave);
          this.mAccountGrid.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.AccountButton_MouseLeftButtonUp);
          break;
        case 15:
          this.mAccountButtonImage = (CustomPictureBox) target;
          break;
        case 16:
          this.mAccountButtonText = (TextBlock) target;
          break;
        case 17:
          this.mSettingsButtonGrid = (Grid) target;
          this.mSettingsButtonGrid.MouseEnter += new MouseEventHandler(this.Grid_MouseEnter);
          this.mSettingsButtonGrid.MouseLeave += new MouseEventHandler(this.Grid_MouseLeave);
          this.mSettingsButtonGrid.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.SettingsButton_MouseLeftButtonUp);
          break;
        case 18:
          this.mSettingsButtonImage = (CustomPictureBox) target;
          break;
        case 19:
          this.mSettingsButtonText = (TextBlock) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
