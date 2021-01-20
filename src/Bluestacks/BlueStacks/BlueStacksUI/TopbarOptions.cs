// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.TopbarOptions
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace BlueStacks.BlueStacksUI
{
  public class TopbarOptions : UserControl, IComponentConnector
  {
    private readonly object mSyncRoot = new object();
    private bool mIsLoadedOnce;
    private bool mIsInFullscreenMode;
    private MainWindow mMainWindow;
    internal Grid TopMenu;
    internal ColumnDefinition GameGuideColumn;
    internal TextBlock mFullScreenTextBlock;
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

    public TopbarOptions()
    {
      this.InitializeComponent();
    }

    private void Topbar_Loaded(object sender, RoutedEventArgs e)
    {
      if (!this.mIsLoadedOnce)
      {
        this.mIsLoadedOnce = true;
        this.BindEvents();
      }
      this.SetLabel();
    }

    public void SetLabel()
    {
      this.mFullScreenTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_EXIT_FULL_SCREEN", "") + " (" + this.ParentWindow.mCommonHandler.GetShortcutKeyFromName("STRING_UPDATED_FULLSCREEN_BUTTON_TOOLTIP", false) + ")";
    }

    internal void BindEvents()
    {
      this.ParentWindow.FullScreenChanged += new MainWindow.FullScreenChangedEventHandler(this.ParentWindow_FullScreenChangedEvent);
    }

    private void ParentWindow_FullScreenChangedEvent(
      object sender,
      MainWindowEventArgs.FullScreenChangedEventArgs args)
    {
      lock (this.mSyncRoot)
      {
        this.mIsInFullscreenMode = args.IsFullscreen;
        if (!this.mIsInFullscreenMode)
        {
          this.ParentWindow.mFullscreenTopbarPopupButton.IsOpen = false;
          this.ParentWindow.mFullscreenTopbarPopup.IsOpen = false;
        }
        else
          this.GameGuideColumn.Width = this.ParentWindow.SelectedConfig == null || this.ParentWindow.SelectedConfig.SelectedControlScheme == null || this.ParentWindow.SelectedConfig.SelectedControlScheme.GameControls == null || !this.ParentWindow.SelectedConfig.SelectedControlScheme.GameControls.Any<IMAction>((Func<IMAction, bool>) (action => action.Guidance.Any<KeyValuePair<string, string>>())) ? new GridLength(0.0, GridUnitType.Star) : new GridLength(1.0, GridUnitType.Star);
      }
    }

    internal void HideTopBarInFullscreen()
    {
      this.ParentWindow.mFullscreenTopbarPopupButton.IsOpen = false;
      this.ParentWindow.mFullscreenTopbarPopup.IsOpen = false;
    }

    internal void ToggleTopbarButtonVisibilityInFullscreen(bool isVisible)
    {
      if (isVisible && !this.ParentWindow.mFullscreenTopbarPopup.IsOpen && (!this.ParentWindow.mFullscreenSidebarPopup.IsOpen && this.ParentWindow.EngineInstanceRegistry.ShowBlueHighlighter))
      {
        this.ParentWindow.mFullscreenTopbarPopupButtonInnerGrid.Width = this.ParentWindow.MainGrid.ActualWidth;
        this.ParentWindow.mFullscreenTopbarPopupButton.Width = this.ParentWindow.MainGrid.ActualWidth;
        this.ParentWindow.mFullscreenTopbarPopupButton.VerticalOffset = -(this.ParentWindow.MainGrid.ActualHeight / 2.0);
        this.ParentWindow.mFullscreenTopbarPopupButton.IsOpen = this.mIsInFullscreenMode;
      }
      else
      {
        if (isVisible)
          return;
        this.ParentWindow.mFullscreenTopbarPopupButton.IsOpen = false;
      }
    }

    internal void ToggleTopbarVisibilityInFullscreen(bool isVisible)
    {
      if (isVisible)
      {
        this.ParentWindow.mFullscreenTopbarPopup.Width = this.ParentWindow.MainGrid.ActualWidth;
        this.ParentWindow.mFullscreenTopbarPopup.VerticalOffset = -(this.ParentWindow.MainGrid.ActualHeight / 2.0);
        this.ParentWindow.mFullscreenTopbarPopupInnerGrid.Width = this.ParentWindow.MainGrid.ActualWidth;
        ClientStats.SendMiscellaneousStatsAsync("fullscreen", RegistryManager.Instance.UserGuid, "topBarButton", "MouseClick", RegistryManager.Instance.ClientVersion, Oem.Instance.OEM, (string) null, (string) null, (string) null, "Android");
      }
      this.ParentWindow.mFullscreenTopbarPopup.IsOpen = isVisible;
      this.ParentWindow.mFullscreenTopbarPopupButton.IsOpen = false;
    }

    private void Label_MouseEnter(object sender, MouseEventArgs e)
    {
      if (!(sender is Label label) || !(this.TryFindResource((object) "LabelMouseHoverBackground") is SolidColorBrush resource))
        return;
      label.Background = (Brush) resource;
    }

    private void Label_MouseLeave(object sender, MouseEventArgs e)
    {
      if (!(sender is Label label) || !(this.TryFindResource((object) "LabelBackground") is SolidColorBrush resource))
        return;
      label.Background = (Brush) resource;
    }

    private void FullScreen_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      if (this.ParentWindow.mStreamingModeEnabled)
        return;
      this.ParentWindow.mCommonHandler.FullScreenButtonHandler("topbar", "MouseClick");
    }

    private void GameGuide_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      if (!this.ParentWindow.mCommonHandler.ToggleGamepadAndKeyboardGuidance("gamepad", false))
        KMManager.HandleInputMapperWindow(this.ParentWindow, "gamepad");
      ClientStats.SendMiscellaneousStatsAsync("topbar", RegistryManager.Instance.UserGuid, "gameGuide", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab?.PackageName, (string) null, "Android");
    }

    private void Setting_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      string tabName = string.Empty;
      if (this.ParentWindow.StaticComponents.mSelectedTabButton.mTabType == TabType.AppTab && !PackageActivityNames.SystemApps.Contains(this.ParentWindow.StaticComponents.mSelectedTabButton.PackageName))
        tabName = "STRING_GAME_SETTINGS";
      ClientStats.SendMiscellaneousStatsAsync("topbar", RegistryManager.Instance.UserGuid, "Settings", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
      this.ParentWindow.mCommonHandler.LaunchSettingsWindow(tabName);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/topbaroptions.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          ((FrameworkElement) target).Loaded += new RoutedEventHandler(this.Topbar_Loaded);
          break;
        case 2:
          this.TopMenu = (Grid) target;
          break;
        case 3:
          this.GameGuideColumn = (ColumnDefinition) target;
          break;
        case 4:
          ((UIElement) target).MouseEnter += new MouseEventHandler(this.Label_MouseEnter);
          ((UIElement) target).MouseLeave += new MouseEventHandler(this.Label_MouseLeave);
          ((UIElement) target).MouseLeftButtonDown += new MouseButtonEventHandler(this.FullScreen_MouseLeftButtonDown);
          break;
        case 5:
          this.mFullScreenTextBlock = (TextBlock) target;
          break;
        case 6:
          ((UIElement) target).MouseEnter += new MouseEventHandler(this.Label_MouseEnter);
          ((UIElement) target).MouseLeave += new MouseEventHandler(this.Label_MouseLeave);
          ((UIElement) target).MouseLeftButtonDown += new MouseButtonEventHandler(this.GameGuide_MouseLeftButtonDown);
          break;
        case 7:
          ((UIElement) target).MouseEnter += new MouseEventHandler(this.Label_MouseEnter);
          ((UIElement) target).MouseLeave += new MouseEventHandler(this.Label_MouseLeave);
          ((UIElement) target).MouseLeftButtonDown += new MouseButtonEventHandler(this.Setting_MouseLeftButtonDown);
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
