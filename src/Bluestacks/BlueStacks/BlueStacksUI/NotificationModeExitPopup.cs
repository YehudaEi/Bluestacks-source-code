// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.NotificationModeExitPopup
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;

namespace BlueStacks.BlueStacksUI
{
  public class NotificationModeExitPopup : UserControl, IDimOverlayControl, IComponentConnector
  {
    internal Border mBackground;
    internal Grid mMainGrid;
    internal Border mMaskBorder;
    internal CustomPictureBox mClosebtn;
    internal Border mIconBorder;
    internal Button mYesBtn;
    internal TextBlock mCloseBluestacks;
    internal CustomCheckbox mPreferenceCheckBox;
    private bool _contentLoaded;

    bool IDimOverlayControl.IsCloseOnOverLayClick
    {
      get
      {
        return false;
      }
      set
      {
      }
    }

    public bool ShowControlInSeparateWindow { get; set; } = true;

    public bool ShowTransparentWindow { get; set; }

    public MainWindow ParentWindow { get; private set; }

    public string PackageName { get; private set; }

    public NotificationModeExitPopup(MainWindow window, string packageName)
    {
      this.InitializeComponent();
      this.ParentWindow = window;
      this.PackageName = packageName;
      string uriString = !File.Exists(Path.Combine(RegistryStrings.GadgetDir, packageName) + ".ico") ? (!File.Exists(Path.Combine(RegistryStrings.GadgetDir, packageName) + ".png") ? Path.Combine(RegistryManager.Instance.ClientInstallDir, "Assets\\BlueStacks.ico") : Path.Combine(RegistryStrings.GadgetDir, packageName) + ".png") : Path.Combine(RegistryStrings.GadgetDir, packageName) + ".ico";
      this.mIconBorder.Background = (Brush) new ImageBrush((ImageSource) new BitmapImage(new Uri(uriString)));
      BlueStacks.Common.Stats.SendCommonClientStatsAsync("notification_mode", "exitpopup_shown", this.ParentWindow?.mVmName, packageName, "", "", "");
      BitmapImage bitmapImage = new BitmapImage(new Uri(uriString));
      CroppedBitmap croppedBitmap = new CroppedBitmap((BitmapSource) bitmapImage, new Int32Rect((int) bitmapImage.Width / 10, (int) bitmapImage.Height * 2 / 10, (int) bitmapImage.Width * 8 / 10, (int) bitmapImage.Height * 8 / 10));
      Image image = new Image();
      image.Source = (ImageSource) croppedBitmap;
      image.Effect = (Effect) new BlurEffect()
      {
        Radius = 24.0,
        KernelType = KernelType.Gaussian
      };
      image.ClipToBounds = true;
      VisualBrush visualBrush = new VisualBrush((Visual) image);
      visualBrush.Opacity = 0.4;
      this.mBackground.Background = (Brush) visualBrush;
    }

    bool IDimOverlayControl.Close()
    {
      this.Close();
      return true;
    }

    bool IDimOverlayControl.Show()
    {
      this.Visibility = Visibility.Visible;
      return true;
    }

    private void Close()
    {
      try
      {
        this.ParentWindow.HideDimOverlay();
        BlueStacksUIUtils.CloseContainerWindow((FrameworkElement) this);
        this.Visibility = Visibility.Hidden;
      }
      catch (Exception ex)
      {
        Logger.Error("Exception while trying to close CloseBluestacksControl from dimoverlay " + ex.ToString());
      }
    }

    private void mClosebtn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      BlueStacks.Common.Stats.SendCommonClientStatsAsync("notification_mode", "exitpopup_closed", this.ParentWindow.mVmName, "", "", "", "");
      this.Close();
    }

    private void mYesBtn_Click(object sender, RoutedEventArgs e)
    {
      BlueStacksUIBinding.BindColor((DependencyObject) this.mYesBtn, Control.BackgroundProperty, "BlueMouseDownBorderBackground");
      BlueStacks.Common.Stats.SendCommonClientStatsAsync("notification_mode", "exitpopup_yes", this.ParentWindow.mVmName, "", "", "", "");
      this.ParentWindow.EngineInstanceRegistry.IsMinimizeSelectedOnReceiveGameNotificationPopup = true;
      RegistryManager.Instance.IsNotificationModeAlwaysOn = true;
      NotificationManager.Instance.UpdateMuteState(MuteState.NotMuted, new JsonParser(this.ParentWindow.mVmName).GetAppNameFromPackage(this.PackageName), this.ParentWindow.mVmName);
      this.Close();
      this.ParentWindow.MinimizeWindowHandler();
    }

    private void mCloseBluestacks_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      BlueStacks.Common.Stats.SendCommonClientStatsAsync("notification_mode", "exitpopup_no", this.ParentWindow.mVmName, "", "", "", "");
      ++this.ParentWindow.EngineInstanceRegistry.NotificationModePopupShownCount;
      this.ParentWindow.EngineInstanceRegistry.IsMinimizeSelectedOnReceiveGameNotificationPopup = false;
      RegistryManager.Instance.IsNotificationModeAlwaysOn = false;
      this.Close();
      this.ParentWindow.CloseWindowHandler(false);
    }

    private void mYesBtn_MouseEnter(object sender, MouseEventArgs e)
    {
      BlueStacksUIBinding.BindColor((DependencyObject) this.mYesBtn, Control.BackgroundProperty, "BlueMouseInGridBackGround");
    }

    private void mYesBtn_MouseLeave(object sender, MouseEventArgs e)
    {
      BlueStacksUIBinding.BindColor((DependencyObject) this.mYesBtn, Control.BackgroundProperty, "BlueMouseOutGridBackground");
    }

    private void mPreferenceCheckBox_Checked(object sender, RoutedEventArgs e)
    {
      this.ParentWindow.EngineInstanceRegistry.IsShowMinimizeBlueStacksPopupOnClose = !this.mPreferenceCheckBox.IsChecked.GetValueOrDefault(true);
      BlueStacks.Common.Stats.SendCommonClientStatsAsync("notification_mode", "exit_popup_preference", this.ParentWindow.mVmName, "", "", "", "");
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/notificationmodeexitpopup.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.mBackground = (Border) target;
          break;
        case 2:
          this.mMainGrid = (Grid) target;
          break;
        case 3:
          this.mMaskBorder = (Border) target;
          break;
        case 4:
          this.mClosebtn = (CustomPictureBox) target;
          this.mClosebtn.MouseLeftButtonUp += new MouseButtonEventHandler(this.mClosebtn_MouseLeftButtonUp);
          break;
        case 5:
          this.mIconBorder = (Border) target;
          break;
        case 6:
          this.mYesBtn = (Button) target;
          this.mYesBtn.Click += new RoutedEventHandler(this.mYesBtn_Click);
          this.mYesBtn.MouseEnter += new MouseEventHandler(this.mYesBtn_MouseEnter);
          this.mYesBtn.MouseLeave += new MouseEventHandler(this.mYesBtn_MouseLeave);
          break;
        case 7:
          this.mCloseBluestacks = (TextBlock) target;
          this.mCloseBluestacks.MouseLeftButtonUp += new MouseButtonEventHandler(this.mCloseBluestacks_MouseLeftButtonUp);
          break;
        case 8:
          this.mPreferenceCheckBox = (CustomCheckbox) target;
          this.mPreferenceCheckBox.Checked += new RoutedEventHandler(this.mPreferenceCheckBox_Checked);
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
