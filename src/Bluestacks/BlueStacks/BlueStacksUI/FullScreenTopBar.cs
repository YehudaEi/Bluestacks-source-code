// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.FullScreenTopBar
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

namespace BlueStacks.BlueStacksUI
{
  public class FullScreenTopBar : UserControl, IComponentConnector
  {
    private MainWindow ParentWindow;
    private double lastSliderValue;
    internal CustomPictureBox mEscCheckbox;
    internal CustomPictureBox mGamePadButtonFullScreen;
    internal CustomPictureBox mMacroRecorderFullScreen;
    internal CustomPictureBox mKeyMapSwitchFullScreen;
    internal CustomPictureBox mKeyMapButtonFullScreen;
    internal CustomPictureBox mTranslucentControlsButtonFullScreen;
    internal CustomPictureBox mFullScreenButton;
    internal CustomPictureBox mLocationButtonFullScreen;
    internal CustomPictureBox mShakeButtonFullScreen;
    internal CustomPopUp mChangeTransparencyPopup;
    internal Border borderSlider;
    internal CustomPictureBox mTranslucentControlsSliderButton;
    internal Slider transSlider;
    private bool _contentLoaded;

    public FullScreenTopBar()
    {
      this.InitializeComponent();
    }

    internal void Init(MainWindow window)
    {
      this.ParentWindow = window;
      if (!DesignerProperties.GetIsInDesignMode((DependencyObject) this) && !RegistryManager.Instance.UseEscapeToExitFullScreen)
        this.mEscCheckbox.ImageName = "checkbox_new";
      this.transSlider.Value = RegistryManager.Instance.TranslucentControlsTransparency;
      if (FeatureManager.Instance.IsCustomUIForDMMSandbox)
      {
        this.mKeyMapSwitchFullScreen.Visibility = Visibility.Collapsed;
        this.mKeyMapButtonFullScreen.Visibility = Visibility.Collapsed;
        this.mLocationButtonFullScreen.Visibility = Visibility.Collapsed;
        this.mShakeButtonFullScreen.Visibility = Visibility.Collapsed;
        this.mGamePadButtonFullScreen.Visibility = Visibility.Collapsed;
        this.mTranslucentControlsButtonFullScreen.Visibility = Visibility.Collapsed;
      }
      this.mMacroRecorderFullScreen.Visibility = Visibility.Collapsed;
    }

    private void BackButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.ParentWindow.mCommonHandler.BackButtonHandler(false);
    }

    private void HomeButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.ParentWindow.mCommonHandler.HomeButtonHandler(true, false);
    }

    private void SwitchKeyMapButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      ClientStats.SendMiscellaneousStatsAsync("SwitchKeyMapClicked", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "fullscreentopbar", (string) null, (string) null, (string) null, (string) null, (string) null, "Android");
    }

    private void KeyMapButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.ParentWindow.mTopBarPopup.IsOpen = false;
      this.ParentWindow.mCommonHandler.KeyMapButtonHandler("MouseClick", "fullscreentopbar");
    }

    private void FullScreenButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.ParentWindow.mCommonHandler.FullScreenButtonHandler("fullScreenTopbar", "MouseClick");
    }

    private void LocationButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.ParentWindow.mCommonHandler.LocationButtonHandler();
    }

    private void ScreenShotButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.ParentWindow.mTopBarPopup.IsOpen = false;
      this.ParentWindow.mCommonHandler.ScreenShotButtonHandler();
    }

    private void ShakeButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.ParentWindow.mCommonHandler.ShakeButtonHandler();
    }

    private void mEscCheckbox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      if (RegistryManager.Instance.UseEscapeToExitFullScreen)
      {
        this.mEscCheckbox.ImageName = "checkbox_new";
        RegistryManager.Instance.UseEscapeToExitFullScreen = false;
      }
      else
      {
        this.mEscCheckbox.ImageName = "checkbox_new_checked";
        RegistryManager.Instance.UseEscapeToExitFullScreen = true;
      }
    }

    private void mMacroRecorderLandscape_PreviewMouseLeftButtonUp(
      object sender,
      MouseButtonEventArgs e)
    {
      this.ParentWindow.mCommonHandler.ShowMacroRecorderWindow();
    }

    private void GamePadButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.ParentWindow.mTopBarPopup.IsOpen = false;
    }

    private void TranslucentControlsButton_PreviewMouseLeftButtonUp(
      object sender,
      MouseButtonEventArgs e)
    {
      RegistryManager.Instance.ShowKeyControlsOverlay = true;
      RegistryManager.Instance.OverlayAvailablePromptEnabled = false;
      KMManager.ShowOverlayWindow(this.ParentWindow, true, true);
      this.mChangeTransparencyPopup.PlacementTarget = (UIElement) this.mTranslucentControlsButtonFullScreen;
      this.mChangeTransparencyPopup.IsOpen = true;
    }

    private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
      KMManager.ChangeTransparency(this.ParentWindow, this.transSlider.Value);
      if (this.transSlider.Value == 0.0)
      {
        if (!RegistryManager.Instance.ShowKeyControlsOverlay)
          KMManager.ShowOverlayWindow(this.ParentWindow, false, false);
        this.ParentWindow.mCommonHandler.OnOverlayStateChanged(false);
      }
      else
      {
        KMManager.ShowOverlayWindow(this.ParentWindow, true, false);
        this.ParentWindow.mCommonHandler.OnOverlayStateChanged(true);
      }
      this.lastSliderValue = this.transSlider.Value;
    }

    private void mTranslucentControlsSliderButton_PreviewMouseLeftButtonUp(
      object sender,
      MouseButtonEventArgs e)
    {
      if (this.transSlider.Value == 0.0)
      {
        this.transSlider.Value = this.lastSliderValue;
      }
      else
      {
        double num = this.transSlider.Value;
        this.transSlider.Value = 0.0;
        this.lastSliderValue = num;
      }
    }

    private void mChangeTransparencyPopup_Closed(object sender, EventArgs e)
    {
      if (this.ParentWindow.mFullScreenTopBar.IsMouseOver)
        return;
      this.ParentWindow.mTopBarPopup.IsOpen = false;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/fullscreentopbar.xaml", UriKind.Relative));
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
          ((UIElement) target).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.BackButton_PreviewMouseLeftButtonUp);
          break;
        case 2:
          ((UIElement) target).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.HomeButton_PreviewMouseLeftButtonUp);
          break;
        case 3:
          this.mEscCheckbox = (CustomPictureBox) target;
          this.mEscCheckbox.MouseLeftButtonUp += new MouseButtonEventHandler(this.mEscCheckbox_MouseLeftButtonUp);
          break;
        case 4:
          this.mGamePadButtonFullScreen = (CustomPictureBox) target;
          this.mGamePadButtonFullScreen.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.GamePadButton_PreviewMouseLeftButtonUp);
          break;
        case 5:
          this.mMacroRecorderFullScreen = (CustomPictureBox) target;
          this.mMacroRecorderFullScreen.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.mMacroRecorderLandscape_PreviewMouseLeftButtonUp);
          break;
        case 6:
          this.mKeyMapSwitchFullScreen = (CustomPictureBox) target;
          this.mKeyMapSwitchFullScreen.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.SwitchKeyMapButton_PreviewMouseLeftButtonUp);
          break;
        case 7:
          this.mKeyMapButtonFullScreen = (CustomPictureBox) target;
          this.mKeyMapButtonFullScreen.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.KeyMapButton_PreviewMouseLeftButtonUp);
          break;
        case 8:
          this.mTranslucentControlsButtonFullScreen = (CustomPictureBox) target;
          this.mTranslucentControlsButtonFullScreen.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.TranslucentControlsButton_PreviewMouseLeftButtonUp);
          break;
        case 9:
          this.mFullScreenButton = (CustomPictureBox) target;
          this.mFullScreenButton.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.FullScreenButton_PreviewMouseLeftButtonUp);
          break;
        case 10:
          this.mLocationButtonFullScreen = (CustomPictureBox) target;
          this.mLocationButtonFullScreen.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.LocationButton_PreviewMouseLeftButtonUp);
          break;
        case 11:
          ((UIElement) target).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.ScreenShotButton_PreviewMouseLeftButtonUp);
          break;
        case 12:
          this.mShakeButtonFullScreen = (CustomPictureBox) target;
          this.mShakeButtonFullScreen.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.ShakeButton_PreviewMouseLeftButtonUp);
          break;
        case 13:
          this.mChangeTransparencyPopup = (CustomPopUp) target;
          break;
        case 14:
          this.borderSlider = (Border) target;
          break;
        case 15:
          this.mTranslucentControlsSliderButton = (CustomPictureBox) target;
          this.mTranslucentControlsSliderButton.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.mTranslucentControlsSliderButton_PreviewMouseLeftButtonUp);
          break;
        case 16:
          this.transSlider = (Slider) target;
          this.transSlider.ValueChanged += new RoutedPropertyChangedEventHandler<double>(this.Slider_ValueChanged);
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
