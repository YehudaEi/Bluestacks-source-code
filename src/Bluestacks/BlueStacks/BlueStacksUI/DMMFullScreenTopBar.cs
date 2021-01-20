// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.DMMFullScreenTopBar
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
  public class DMMFullScreenTopBar : UserControl, IComponentConnector
  {
    private MainWindow ParentWindow;
    internal CustomPictureBox mEscCheckbox;
    internal CustomPictureBox mKeyMapSwitch;
    internal CustomPictureBox mKeyMapButton;
    internal CustomPictureBox mTranslucentControlsButton;
    internal CustomPictureBox mScreenshotBtn;
    internal CustomPictureBox mVolumeBtn;
    internal CustomPictureBox mWindowedBtn;
    internal CustomPictureBox mSettingsBtn;
    internal CustomPopUp mVolumePopup;
    internal CustomPictureBox volumeSliderImage;
    internal Slider mVolumeSlider;
    internal CustomPopUp mChangeTransparencyPopup;
    internal CustomPictureBox mTranslucentControlsSliderButton;
    internal Slider transSlider;
    private bool _contentLoaded;

    public DMMFullScreenTopBar()
    {
      this.InitializeComponent();
    }

    internal void Init(MainWindow window)
    {
      this.ParentWindow = window;
      if (!DesignerProperties.GetIsInDesignMode((DependencyObject) this) && !RegistryManager.Instance.UseEscapeToExitFullScreen)
        this.mEscCheckbox.ImageName = "checkbox_new";
      this.mVolumeBtn.ImageName = "volume_small";
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

    private void ScreenshotBtn_MouseUp(object sender, MouseButtonEventArgs e)
    {
      this.ParentWindow.mTopBarPopup.IsOpen = false;
      this.ParentWindow.mCommonHandler.ScreenShotButtonHandler();
    }

    private void VolumeBtn_MouseUp(object sender, MouseButtonEventArgs e)
    {
      this.mVolumePopup.IsOpen = !this.mVolumePopup.IsOpen;
    }

    private void WindowedBtn_MouseUp(object sender, MouseButtonEventArgs e)
    {
      this.ParentWindow.mCommonHandler.FullScreenButtonHandler("fullscreentopbarDmm", "MouseClick");
    }

    private void SettingsBtn_MouseUp(object sender, MouseButtonEventArgs e)
    {
      this.ParentWindow.mCommonHandler.LaunchSettingsWindow("");
    }

    internal void VolumeSlider_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      if (this.ParentWindow == null)
        return;
      this.ParentWindow.mDmmBottomBar.VolumeSlider_PreviewMouseLeftButtonUp(sender, e);
    }

    private void VolumeSliderImage_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      if (this.ParentWindow == null)
        return;
      this.ParentWindow.mDmmBottomBar.VolumeSliderImage_PreviewMouseLeftButtonUp(sender, e);
    }

    private void SwitchKeyMapButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.ParentWindow.mCommonHandler.DMMSwitchKeyMapButtonHandler();
    }

    private void KeyMapButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.ParentWindow.mTopBarPopup.IsOpen = false;
      if (this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab == null || this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName == null)
        return;
      this.ParentWindow.mCommonHandler.KeyMapButtonHandler("MouseClick", "fullscreentopbar");
    }

    private void TranslucentControlsButton_PreviewMouseLeftButtonUp(
      object sender,
      MouseButtonEventArgs e)
    {
      this.transSlider.Value = RegistryManager.Instance.TranslucentControlsTransparency;
      this.mChangeTransparencyPopup.PlacementTarget = (UIElement) this.mTranslucentControlsButton;
      this.mChangeTransparencyPopup.IsOpen = true;
    }

    internal void mTranslucentControlsSliderButton_PreviewMouseLeftButtonUp(
      object sender,
      MouseButtonEventArgs e)
    {
      this.ParentWindow.mDmmBottomBar.mTranslucentControlsSliderButton_PreviewMouseLeftButtonUp(sender, e);
    }

    internal void TransparencySlider_ValueChanged(
      object sender,
      RoutedPropertyChangedEventArgs<double> e)
    {
      this.ParentWindow.mDmmBottomBar.TransparencySlider_ValueChanged(sender, e);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/dmmfullscreentopbar.xaml", UriKind.Relative));
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
          this.mEscCheckbox = (CustomPictureBox) target;
          this.mEscCheckbox.MouseLeftButtonUp += new MouseButtonEventHandler(this.mEscCheckbox_MouseLeftButtonUp);
          break;
        case 2:
          this.mKeyMapSwitch = (CustomPictureBox) target;
          this.mKeyMapSwitch.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.SwitchKeyMapButton_PreviewMouseLeftButtonUp);
          break;
        case 3:
          this.mKeyMapButton = (CustomPictureBox) target;
          this.mKeyMapButton.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.KeyMapButton_PreviewMouseLeftButtonUp);
          break;
        case 4:
          this.mTranslucentControlsButton = (CustomPictureBox) target;
          this.mTranslucentControlsButton.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.TranslucentControlsButton_PreviewMouseLeftButtonUp);
          break;
        case 5:
          this.mScreenshotBtn = (CustomPictureBox) target;
          this.mScreenshotBtn.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.ScreenshotBtn_MouseUp);
          break;
        case 6:
          this.mVolumeBtn = (CustomPictureBox) target;
          this.mVolumeBtn.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.VolumeBtn_MouseUp);
          break;
        case 7:
          this.mWindowedBtn = (CustomPictureBox) target;
          this.mWindowedBtn.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.WindowedBtn_MouseUp);
          break;
        case 8:
          this.mSettingsBtn = (CustomPictureBox) target;
          this.mSettingsBtn.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.SettingsBtn_MouseUp);
          break;
        case 9:
          this.mVolumePopup = (CustomPopUp) target;
          break;
        case 10:
          this.volumeSliderImage = (CustomPictureBox) target;
          this.volumeSliderImage.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.VolumeSliderImage_PreviewMouseLeftButtonUp);
          break;
        case 11:
          this.mVolumeSlider = (Slider) target;
          this.mVolumeSlider.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.VolumeSlider_PreviewMouseLeftButtonUp);
          break;
        case 12:
          this.mChangeTransparencyPopup = (CustomPopUp) target;
          break;
        case 13:
          this.mTranslucentControlsSliderButton = (CustomPictureBox) target;
          this.mTranslucentControlsSliderButton.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.mTranslucentControlsSliderButton_PreviewMouseLeftButtonUp);
          break;
        case 14:
          this.transSlider = (Slider) target;
          this.transSlider.ValueChanged += new RoutedPropertyChangedEventHandler<double>(this.TransparencySlider_ValueChanged);
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
