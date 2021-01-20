// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.DMMBottomBar
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Shapes;

namespace BlueStacks.BlueStacksUI
{
  public class DMMBottomBar : UserControl, IComponentConnector
  {
    private static double sCurrentTransparency = 0.0;
    private static double sPreviousTransparency = 0.0;
    private static int sCurrentVolume = 33;
    public static readonly DependencyProperty VolumeImageNameProperty = DependencyProperty.Register(nameof (VolumeImageName), typeof (string), typeof (DMMBottomBar), (PropertyMetadata) new FrameworkPropertyMetadata((object) "volume_small", new PropertyChangedCallback(DMMBottomBar.OnVolumeImageNameChanged)));
    private MainWindow ParentWindow;
    internal Grid DMMBottomGrid;
    internal CustomPictureBox mKeyMapSwitch;
    internal CustomPictureBox mKeyMapButton;
    internal CustomPictureBox mTranslucentControlsButton;
    internal CustomPictureBox mScreenshotBtn;
    internal CustomPictureBox mVolumeBtn;
    internal CustomPictureBox mFullscreenBtn;
    internal CustomPictureBox mSettingsBtn;
    internal CustomPictureBox mRecommendedWindowBtn;
    internal CustomPopUp mVolumePopup;
    internal CustomPictureBox volumesSliderImage;
    internal Slider mVolumeSlider;
    internal CustomPopUp mKeyMapPopup;
    internal TextBlock mKeyMappingPopUp1;
    internal TextBlock mKeyMappingPopUp3;
    internal CustomPictureBox mDoNotPromptChkBx;
    internal TextBlock mKeyMappingDontShowPopUp;
    internal Path DownArrow;
    internal CustomPopUp mChangeTransparencyPopup;
    internal CustomPictureBox mTranslucentControlsSliderButton;
    internal Slider transSlider;
    private bool _contentLoaded;

    internal double CurrentTransparency
    {
      get
      {
        return DMMBottomBar.sCurrentTransparency;
      }
      set
      {
        DMMBottomBar.sCurrentTransparency = value;
        this.transSlider.ValueChanged -= new RoutedPropertyChangedEventHandler<double>(this.TransparencySlider_ValueChanged);
        if (this.ParentWindow.mDMMFST != null)
          this.ParentWindow.mDMMFST.transSlider.ValueChanged -= new RoutedPropertyChangedEventHandler<double>(this.ParentWindow.mDMMFST.TransparencySlider_ValueChanged);
        this.transSlider.Value = DMMBottomBar.sCurrentTransparency;
        if (this.ParentWindow.mDMMFST != null)
          this.ParentWindow.mDMMFST.transSlider.Value = DMMBottomBar.sCurrentTransparency;
        this.transSlider.ValueChanged += new RoutedPropertyChangedEventHandler<double>(this.TransparencySlider_ValueChanged);
        if (this.ParentWindow.mDMMFST == null)
          return;
        this.ParentWindow.mDMMFST.transSlider.ValueChanged += new RoutedPropertyChangedEventHandler<double>(this.ParentWindow.mDMMFST.TransparencySlider_ValueChanged);
      }
    }

    internal int CurrentVolume
    {
      get
      {
        return DMMBottomBar.sCurrentVolume;
      }
      set
      {
        DMMBottomBar.sCurrentVolume = value;
        this.mVolumeSlider.Value = (double) DMMBottomBar.sCurrentVolume;
        if (this.ParentWindow.mDMMFST != null && this.ParentWindow.mDMMFST.mVolumeSlider != null)
          this.ParentWindow.mDMMFST.mVolumeSlider.Value = (double) DMMBottomBar.sCurrentVolume;
        if (DMMBottomBar.sCurrentVolume < 1)
          this.VolumeImageName = "volume_mute";
        else if (DMMBottomBar.sCurrentVolume <= 50)
          this.VolumeImageName = "volume_small";
        else
          this.VolumeImageName = "volume_large";
      }
    }

    public string VolumeImageName
    {
      get
      {
        return (string) this.GetValue(DMMBottomBar.VolumeImageNameProperty);
      }
      set
      {
        this.SetValue(DMMBottomBar.VolumeImageNameProperty, (object) value);
      }
    }

    private static void OnVolumeImageNameChanged(
      DependencyObject d,
      DependencyPropertyChangedEventArgs e)
    {
      (d as DMMBottomBar).ParentWindow.mDmmBottomBar.mVolumeBtn.ImageName = e.NewValue.ToString();
      (d as DMMBottomBar).ParentWindow.mDmmBottomBar.volumesSliderImage.ImageName = e.NewValue.ToString();
      (d as DMMBottomBar).ParentWindow.mDMMFST.mVolumeBtn.ImageName = e.NewValue.ToString();
      (d as DMMBottomBar).ParentWindow.mDMMFST.volumeSliderImage.ImageName = e.NewValue.ToString();
    }

    public DMMBottomBar()
    {
      this.InitializeComponent();
    }

    public void Init(MainWindow window)
    {
      this.ParentWindow = window;
      this.VolumeImageName = "volume_small";
      this.mVolumeBtn.ImageName = "volume_small";
      this.CurrentTransparency = DMMBottomBar.sPreviousTransparency = RegistryManager.Instance.TranslucentControlsTransparency;
      if (this.ParentWindow == null)
        return;
      this.ParentWindow.mCommonHandler.VolumeChangedEvent += new CommonHandlers.VolumeChanged(this.DMMBottomBar_VolumeChangedEvent);
      this.ParentWindow.mCommonHandler.VolumeMutedEvent += new CommonHandlers.VolumeMuted(this.DMMBottomBar_VolumeMutedEvent);
    }

    private void DMMBottomBar_VolumeMutedEvent(bool muted)
    {
      if (muted)
        this.VolumeImageName = "volume_mute";
      else
        this.CurrentVolume = (int) this.mVolumeSlider.Value;
    }

    private void DMMBottomBar_VolumeChangedEvent(int volumeLevel)
    {
      this.Dispatcher.Invoke((Delegate) (() => this.CurrentVolume = volumeLevel));
    }

    internal void Tab_Changed(object sender, EventArgs e)
    {
      this.ParentWindow.mCommonHandler.SetDMMKeymapButtonsAndTransparency();
    }

    private void FullScreenBtn_MouseUp(object sender, MouseButtonEventArgs e)
    {
      this.ParentWindow.mCommonHandler.FullScreenButtonHandler("bottombarDmm", "MouseClick");
    }

    private void ScreenshotBtn_MouseUp(object sender, MouseButtonEventArgs e)
    {
      this.ParentWindow.mCommonHandler.ScreenShotButtonHandler();
    }

    private void VolumeBtn_MouseUp(object sender, MouseButtonEventArgs e)
    {
      this.mVolumePopup.IsOpen = !this.mVolumePopup.IsOpen;
    }

    private void SettingsBtn_MouseUp(object sender, MouseButtonEventArgs e)
    {
      this.ParentWindow.mCommonHandler.LaunchSettingsWindow("");
    }

    private void RecommendedWindowBtn_PreviewMouseLeftButtonUp(
      object sender,
      MouseButtonEventArgs e)
    {
      if (this.ParentWindow.WindowState != WindowState.Normal)
        return;
      if (this.ParentWindow.mDMMRecommendedWindow == null)
      {
        this.ParentWindow.mDMMRecommendedWindow = new DMMRecommendedWindow(this.ParentWindow);
        this.ParentWindow.mDMMRecommendedWindow.Init(RegistryManager.Instance.DMMRecommendedWindowUrl);
      }
      if (this.ParentWindow.mDMMRecommendedWindow.Visibility != Visibility.Visible)
      {
        this.ParentWindow.mDMMRecommendedWindow.Visibility = Visibility.Visible;
        this.ParentWindow.mIsDMMRecommendedWindowOpen = true;
      }
      else
      {
        this.ParentWindow.mDMMRecommendedWindow.Visibility = Visibility.Hidden;
        this.ParentWindow.mIsDMMRecommendedWindowOpen = false;
      }
      ThreadPool.QueueUserWorkItem((WaitCallback) (obj =>
      {
        Thread.Sleep(500);
        this.ParentWindow.Dispatcher.Invoke((Delegate) (() => this.ParentWindow.Activate()));
      }));
    }

    private void DoNotPromptManageGP_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      if (string.Equals(this.mDoNotPromptChkBx.ImageName, "bgpcheckbox", StringComparison.InvariantCulture))
      {
        this.mDoNotPromptChkBx.ImageName = "bgpcheckbox_checked";
        RegistryManager.Instance.KeyMappingAvailablePromptEnabled = false;
      }
      else
      {
        this.mDoNotPromptChkBx.ImageName = "bgpcheckbox";
        RegistryManager.Instance.KeyMappingAvailablePromptEnabled = true;
      }
      e.Handled = true;
    }

    private void ClosePopup_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.mKeyMapPopup.IsOpen = false;
      e.Handled = true;
    }

    private void KeyMapPopup_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.mKeyMapPopup.IsOpen = false;
      this.ParentWindow.mCommonHandler.KeyMapButtonHandler("MouseClick", "bottombarpopup");
    }

    private void SwitchKeyMapButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.ParentWindow.mCommonHandler.DMMSwitchKeyMapButtonHandler();
    }

    private void KeyMapButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      if (this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab == null || this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName == null)
        return;
      this.ParentWindow.mCommonHandler.KeyMapButtonHandler("MouseClick", "bottombar");
    }

    private void TranslucentControlsButton_PreviewMouseLeftButtonUp(
      object sender,
      MouseButtonEventArgs e)
    {
      this.transSlider.Value = RegistryManager.Instance.TranslucentControlsTransparency;
      this.mChangeTransparencyPopup.PlacementTarget = (UIElement) this.mTranslucentControlsButton;
      this.mChangeTransparencyPopup.IsOpen = true;
    }

    private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      if (DesignerProperties.GetIsInDesignMode((DependencyObject) this))
        return;
      this.UpdateLayoutAndBounds();
    }

    internal void UpdateLayoutAndBounds()
    {
      if (!this.mKeyMapPopup.IsOpen)
        return;
      this.ShowKeyMapPopup(true);
    }

    internal void ShowKeyMapPopup(bool isShow)
    {
      if (isShow)
        new Thread((ThreadStart) (() =>
        {
          Thread.Sleep(500);
          this.Dispatcher.Invoke((Delegate) (() =>
          {
            this.mKeyMapPopup.IsOpen = false;
            this.mKeyMapPopup.PlacementTarget = (UIElement) this.mKeyMapButton;
            if (!RegistryManager.Instance.KeyMappingAvailablePromptEnabled)
              return;
            this.mKeyMapPopup.IsOpen = true;
          }));
        }))
        {
          IsBackground = true
        }.Start();
      else
        this.Dispatcher.Invoke((Delegate) (() => this.mKeyMapPopup.IsOpen = false));
    }

    internal void TransparencySlider_ValueChanged(
      object sender,
      RoutedPropertyChangedEventArgs<double> e)
    {
      this.CurrentTransparency = e.NewValue;
      DMMBottomBar.sPreviousTransparency = e.NewValue;
      this.ChangeTransparency();
    }

    private void ChangeTransparency()
    {
      KMManager.ChangeTransparency(this.ParentWindow, this.CurrentTransparency);
      if (this.CurrentTransparency == 0.0)
      {
        KMManager.ShowOverlayWindow(this.ParentWindow, false, false);
        this.ParentWindow.mCommonHandler.SetTranslucentControlsBtnImageForDMM("eye_off");
      }
      else
      {
        KMManager.ShowOverlayWindow(this.ParentWindow, true, false);
        this.ParentWindow.mCommonHandler.SetTranslucentControlsBtnImageForDMM("eye");
      }
    }

    internal void VolumeSlider_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      Slider slider = (Slider) sender;
      if (this.ParentWindow == null)
        return;
      this.ParentWindow.Utils.SetVolumeInFrontendAsync((int) slider.Value);
    }

    internal void mTranslucentControlsSliderButton_PreviewMouseLeftButtonUp(
      object sender,
      MouseButtonEventArgs e)
    {
      this.CurrentTransparency = this.CurrentTransparency == 0.0 ? DMMBottomBar.sPreviousTransparency : 0.0;
      this.ChangeTransparency();
    }

    internal void VolumeSliderImage_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      if (this.ParentWindow == null)
        return;
      if (this.ParentWindow.EngineInstanceRegistry.IsMuted)
        this.ParentWindow.Utils.UnmuteApplication();
      else
        this.ParentWindow.Utils.MuteApplication();
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/dmmbottombar.xaml", UriKind.Relative));
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
          ((FrameworkElement) target).SizeChanged += new SizeChangedEventHandler(this.UserControl_SizeChanged);
          break;
        case 2:
          this.DMMBottomGrid = (Grid) target;
          break;
        case 3:
          this.mKeyMapSwitch = (CustomPictureBox) target;
          this.mKeyMapSwitch.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.SwitchKeyMapButton_PreviewMouseLeftButtonUp);
          break;
        case 4:
          this.mKeyMapButton = (CustomPictureBox) target;
          this.mKeyMapButton.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.KeyMapButton_PreviewMouseLeftButtonUp);
          break;
        case 5:
          this.mTranslucentControlsButton = (CustomPictureBox) target;
          this.mTranslucentControlsButton.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.TranslucentControlsButton_PreviewMouseLeftButtonUp);
          break;
        case 6:
          this.mScreenshotBtn = (CustomPictureBox) target;
          this.mScreenshotBtn.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.ScreenshotBtn_MouseUp);
          break;
        case 7:
          this.mVolumeBtn = (CustomPictureBox) target;
          this.mVolumeBtn.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.VolumeBtn_MouseUp);
          break;
        case 8:
          this.mFullscreenBtn = (CustomPictureBox) target;
          this.mFullscreenBtn.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.FullScreenBtn_MouseUp);
          break;
        case 9:
          this.mSettingsBtn = (CustomPictureBox) target;
          this.mSettingsBtn.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.SettingsBtn_MouseUp);
          break;
        case 10:
          this.mRecommendedWindowBtn = (CustomPictureBox) target;
          this.mRecommendedWindowBtn.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.RecommendedWindowBtn_PreviewMouseLeftButtonUp);
          break;
        case 11:
          this.mVolumePopup = (CustomPopUp) target;
          break;
        case 12:
          this.volumesSliderImage = (CustomPictureBox) target;
          this.volumesSliderImage.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.VolumeSliderImage_PreviewMouseLeftButtonUp);
          break;
        case 13:
          this.mVolumeSlider = (Slider) target;
          this.mVolumeSlider.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.VolumeSlider_PreviewMouseLeftButtonUp);
          break;
        case 14:
          this.mKeyMapPopup = (CustomPopUp) target;
          break;
        case 15:
          ((UIElement) target).MouseLeftButtonUp += new MouseButtonEventHandler(this.KeyMapPopup_PreviewMouseLeftButtonUp);
          break;
        case 16:
          ((UIElement) target).MouseLeftButtonUp += new MouseButtonEventHandler(this.ClosePopup_MouseLeftButtonUp);
          break;
        case 17:
          this.mKeyMappingPopUp1 = (TextBlock) target;
          break;
        case 18:
          this.mKeyMappingPopUp3 = (TextBlock) target;
          break;
        case 19:
          this.mDoNotPromptChkBx = (CustomPictureBox) target;
          this.mDoNotPromptChkBx.MouseLeftButtonUp += new MouseButtonEventHandler(this.DoNotPromptManageGP_MouseLeftButtonUp);
          break;
        case 20:
          this.mKeyMappingDontShowPopUp = (TextBlock) target;
          this.mKeyMappingDontShowPopUp.MouseLeftButtonUp += new MouseButtonEventHandler(this.DoNotPromptManageGP_MouseLeftButtonUp);
          break;
        case 21:
          this.DownArrow = (Path) target;
          break;
        case 22:
          this.mChangeTransparencyPopup = (CustomPopUp) target;
          break;
        case 23:
          this.mTranslucentControlsSliderButton = (CustomPictureBox) target;
          this.mTranslucentControlsSliderButton.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.mTranslucentControlsSliderButton_PreviewMouseLeftButtonUp);
          break;
        case 24:
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
