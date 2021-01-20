// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.GameOnboardingControl
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
using System.Windows.Threading;

namespace BlueStacks.BlueStacksUI
{
  public class GameOnboardingControl : UserControl, IDimOverlayControl, IComponentConnector
  {
    private DispatcherTimer dispatcherTimer;
    internal Grid mBrowserGrid;
    internal Grid mCloseOnboardingGrid;
    internal CustomPictureBox mCloseOnboardingButton;
    internal CustomButton mSkipOnboardingButton;
    internal Grid mBrowserGridTemp;
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

    public bool ShowTransparentWindow { get; set; } = true;

    bool IDimOverlayControl.Close()
    {
      return true;
    }

    bool IDimOverlayControl.Show()
    {
      this.Visibility = Visibility.Visible;
      return true;
    }

    private BrowserControl mBrowser { get; set; }

    public string PackageName { get; set; }

    public MainWindow ParentWindow { get; set; }

    public Grid controlGrid { get; set; }

    public string InitiatedSource { get; set; }

    public string GameFeatureId { get; set; }

    public GameOnboardingControl(MainWindow mainWindow, string packageName, string source)
    {
      this.PackageName = packageName;
      this.ParentWindow = mainWindow;
      this.InitiatedSource = source;
      this.InitializeComponent();
    }

    public GameOnboardingControl(
      MainWindow mainWindow,
      string packageName,
      string source,
      string gameFeatureId)
    {
      this.PackageName = packageName;
      this.ParentWindow = mainWindow;
      this.InitiatedSource = source;
      this.GameFeatureId = gameFeatureId;
      this.InitializeComponent();
    }

    private void Control_Loaded(object sender, RoutedEventArgs e)
    {
      if (string.IsNullOrEmpty(this.GameFeatureId))
        BlueStacks.Common.Stats.SendCommonClientStatsAsync("onboarding-tutorial", "client_impression", this.ParentWindow.mVmName, this.PackageName, "", "", "");
      this.mBrowser = new BrowserControl();
      this.mBrowser.BrowserLoadCompleteEvent += new System.Action(this.BrowserLoadCompleteEvent);
      if (!string.IsNullOrEmpty(this.GameFeatureId))
      {
        this.mBrowser.InitBaseControl(BlueStacksUIUtils.GetGameFeaturePopupUrl(this.PackageName, this.GameFeatureId), 0.0f);
        this.mCloseOnboardingGrid.Visibility = Visibility.Visible;
      }
      else
        this.mBrowser.InitBaseControl(BlueStacksUIUtils.GetOnboardingUrl(this.PackageName, this.InitiatedSource), 0.0f);
      this.mBrowser.Visibility = Visibility.Visible;
      this.mBrowser.ParentWindow = this.ParentWindow;
      this.mBrowserGrid.Children.Add((UIElement) this.mBrowser);
      this.controlGrid = this.AddBrowser();
      this.controlGrid.Visibility = Visibility.Visible;
      this.mBrowserGrid.Children.Add((UIElement) this.controlGrid);
      if (!string.IsNullOrEmpty(this.GameFeatureId))
        return;
      this.dispatcherTimer = new DispatcherTimer();
      this.dispatcherTimer.Tick += new EventHandler(this.DispatcherTimer_Tick);
      this.dispatcherTimer.Interval = new TimeSpan(0, 0, PostBootCloudInfoManager.Instance.mPostBootCloudInfo.OnBoardingInfo.OnBoardingSkipTimer);
      this.dispatcherTimer.Start();
    }

    internal Grid AddBrowser()
    {
      Grid grid = new Grid();
      CustomPictureBox customPictureBox = new CustomPictureBox();
      customPictureBox.HorizontalAlignment = HorizontalAlignment.Center;
      customPictureBox.VerticalAlignment = VerticalAlignment.Center;
      customPictureBox.Height = 30.0;
      customPictureBox.Width = 30.0;
      customPictureBox.ImageName = "loader";
      customPictureBox.IsImageToBeRotated = true;
      grid.Children.Add((UIElement) customPictureBox);
      grid.Visibility = Visibility.Hidden;
      return grid;
    }

    private void BrowserLoadCompleteEvent()
    {
      if (!string.IsNullOrEmpty(this.GameFeatureId))
      {
        this.ParentWindow.mInstanceWiseCloudInfoManager.mInstanceWiseCloudInfo.GameFeaturePopupInfo.GameFeaturePopupPackages.GetAppPackageObject(this.PackageName).ExtraInfo.Remove("isPopupShown");
        this.ParentWindow.mInstanceWiseCloudInfoManager.mInstanceWiseCloudInfo.GameFeaturePopupInfo.GameFeaturePopupPackages.GetAppPackageObject(this.PackageName).ExtraInfo.Add("isPopupShown", "true");
      }
      else
        AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName][this.PackageName].IsAppOnboardingCompleted = true;
      this.mBrowserGrid.Children.Remove((UIElement) this.controlGrid);
    }

    private void DispatcherTimer_Tick(object _1, EventArgs _2)
    {
      this.mSkipOnboardingButton.Visibility = Visibility.Visible;
      this.dispatcherTimer.Stop();
    }

    private void SkipOnboardingButton_Click(object sender, RoutedEventArgs e)
    {
      AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName][this.PackageName].IsAppOnboardingCompleted = true;
      BlueStacks.Common.Stats.SendCommonClientStatsAsync("onboarding-tutorial", "onboarding_skipped", this.ParentWindow.mVmName, this.PackageName, "", "", "");
      this.Close();
      KMManager.sGuidanceWindow?.DimOverLayVisibility(Visibility.Collapsed);
      if (!AppConfigurationManager.Instance.CheckIfTrueInAnyVm(this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName, (Predicate<AppSettings>) (appSettings => appSettings.IsGeneralAppOnBoardingCompleted), out string _))
        this.ParentWindow.StaticComponents.mSelectedTabButton.ShowDefaultBlurbOnboarding();
      this.ParentWindow.HideDimOverlay();
    }

    private void Control_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key != Key.System || e.SystemKey != Key.F4)
        return;
      e.Handled = true;
    }

    internal bool Close()
    {
      try
      {
        if (this.mBrowser != null)
        {
          this.mBrowser.DisposeBrowser();
          this.mBrowserGrid.Children.Remove((UIElement) this.mBrowser);
          this.mBrowser = (BrowserControl) null;
        }
        BlueStacksUIUtils.CloseContainerWindow((FrameworkElement) this);
        this.Visibility = Visibility.Hidden;
        if (string.IsNullOrEmpty(this.GameFeatureId))
          BlueStacks.Common.Stats.SendCommonClientStatsAsync("onboarding-tutorial", "client_closed", this.ParentWindow.mVmName, this.PackageName, "", "", "");
        return true;
      }
      catch (Exception ex)
      {
        Logger.Error("Exception while trying to close gameonboardingcontrol from dimoverlay " + ex.ToString());
      }
      return false;
    }

    private void CloseOnboardingButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      BlueStacks.Common.Stats.SendCommonClientStatsAsync("game-feature-tutorial", "game_popup_close", this.ParentWindow.mVmName, this.PackageName, "", "", this.GameFeatureId);
      this.ParentWindow.mInstanceWiseCloudInfoManager.mInstanceWiseCloudInfo.GameFeaturePopupInfo.GameFeaturePopupPackages.GetAppPackageObject(this.PackageName).ExtraInfo.Remove("isPopupShown");
      this.ParentWindow.mInstanceWiseCloudInfoManager.mInstanceWiseCloudInfo.GameFeaturePopupInfo.GameFeaturePopupPackages.GetAppPackageObject(this.PackageName).ExtraInfo.Add("isPopupShown", "true");
      this.Close();
      KMManager.sGuidanceWindow?.DimOverLayVisibility(Visibility.Collapsed);
      this.ParentWindow.HideDimOverlay();
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/keymap/uielement/gameonboardingcontrol.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          ((FrameworkElement) target).Loaded += new RoutedEventHandler(this.Control_Loaded);
          ((UIElement) target).KeyDown += new KeyEventHandler(this.Control_KeyDown);
          break;
        case 2:
          this.mBrowserGrid = (Grid) target;
          break;
        case 3:
          this.mCloseOnboardingGrid = (Grid) target;
          break;
        case 4:
          this.mCloseOnboardingButton = (CustomPictureBox) target;
          this.mCloseOnboardingButton.MouseLeftButtonUp += new MouseButtonEventHandler(this.CloseOnboardingButton_MouseLeftButtonUp);
          break;
        case 5:
          this.mSkipOnboardingButton = (CustomButton) target;
          this.mSkipOnboardingButton.Click += new RoutedEventHandler(this.SkipOnboardingButton_Click);
          break;
        case 6:
          this.mBrowserGridTemp = (Grid) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
