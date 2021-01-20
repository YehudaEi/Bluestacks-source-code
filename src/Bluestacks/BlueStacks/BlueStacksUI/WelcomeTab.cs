// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.WelcomeTab
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Threading;

namespace BlueStacks.BlueStacksUI
{
  public class WelcomeTab : UserControl, IComponentConnector
  {
    private string CefErrorCode = "";
    private string CefErrorText = "";
    private DateTime mHomeHtmlLoadStartTime = DateTime.Now;
    internal HomeAppManager mHomeAppManager;
    private DispatcherTimer browserExpectedLoadTimer;
    private bool IsHomeHtmlLoaded;
    private bool IsCefErrorReceivedInHomeHtml;
    private bool IsHomeHtmlRefreshed;
    private MainWindow mMainWindow;
    internal Grid mContentGrid;
    internal CustomPictureBox mBackground;
    internal FrontendPopupControl mFrontendPopupControl;
    internal Grid mPromotionGrid;
    internal PromotionControl mPromotionControl;
    private bool _contentLoaded;

    private BrowserControl mBrowser { get; set; }

    public MainWindow ParentWindow
    {
      get
      {
        if (this.mMainWindow == null)
          this.mMainWindow = Window.GetWindow((DependencyObject) this) as MainWindow;
        return this.mMainWindow;
      }
    }

    internal bool IsPromotionVisible
    {
      get
      {
        return this.mPromotionGrid.Visibility == Visibility.Visible;
      }
    }

    public WelcomeTab()
    {
      this.InitializeComponent();
    }

    internal void Init()
    {
      HomeApp homeApp = (HomeApp) null;
      if (FeatureManager.Instance.IsHtmlHome)
      {
        this.mBrowser = this.AddBrowser(this.ParentWindow.Utils.GetHtmlHomeUrl(false), true);
        this.mHomeHtmlLoadStartTime = DateTime.Now;
        this.browserExpectedLoadTimer = new DispatcherTimer();
        this.browserExpectedLoadTimer.Tick += new EventHandler(this.DispatcherTimer_Tick);
        this.browserExpectedLoadTimer.Interval = new TimeSpan(0, 0, 0, 0, RegistryManager.Instance.AvgHomeHtmlLoadTime);
        this.browserExpectedLoadTimer.Start();
      }
      else
      {
        homeApp = new HomeApp(this.ParentWindow);
        if (!this.mContentGrid.Children.Contains((UIElement) homeApp))
          this.mContentGrid.Children.Add((UIElement) homeApp);
      }
      this.mHomeAppManager = new HomeAppManager(homeApp, this.ParentWindow);
      if (RegistryManager.Instance.InstallationType == InstallationTypes.GamingEdition)
      {
        this.mHomeAppManager.ChangeHomeAppVisibility(Visibility.Hidden);
        this.mBackground.ImageName = Path.Combine(RegistryManager.Instance.ClientInstallDir, "Promo\\boot_promo_0.png");
        this.mBackground.Visibility = Visibility.Visible;
      }
      if (!FeatureManager.Instance.IsPromotionDisabled && !Opt.Instance.hiddenBootMode)
        return;
      this.RemovePromotionGrid();
      this.mHomeAppManager.ChangeHomeAppLoadingGridVisibility(Visibility.Visible);
    }

    private void DispatcherTimer_Tick(object _1, EventArgs _2)
    {
      this.browserExpectedLoadTimer.Stop();
      if (!RegistryManager.Instance.HomeHtmlErrorHandling || !this.IsCefErrorReceivedInHomeHtml || this.IsHomeHtmlLoaded)
        return;
      this.BrowserFallbackUrlEvent();
    }

    private void BrowserFallbackUrlEvent()
    {
      if (string.IsNullOrEmpty(RegistryManager.Instance.OfflineHtmlHomeUrl) || string.Equals(this.mBrowser.mUrl, RegistryManager.Instance.OfflineHtmlHomeUrl, StringComparison.InvariantCultureIgnoreCase))
        return;
      Logger.Warning("Loading offline home html");
      this.mBrowser.UpdateUrlAndRefresh(RegistryManager.Instance.OfflineHtmlHomeUrl);
      BlueStacks.Common.Stats.SendCommonClientStatsAsync("html_home", "OfflineHtmlHome_loaded", this.ParentWindow.mVmName, "", this.CefErrorCode, this.CefErrorText, "");
    }

    private BrowserControl AddBrowser(string url, bool isFallbackUrlRequired = false)
    {
      BrowserControl browserControl = new BrowserControl();
      if (isFallbackUrlRequired)
        browserControl.BrowserFallbackUrlEvent += new System.Action(this.BrowserFallbackUrlEvent);
      browserControl.BrowserLoadCompleteEvent += new System.Action(this.BrowserLoadCompleteEvent);
      browserControl.BrowserLoadErrorEvent += new System.Action<string, string>(this.BrowserLoadErrorEvent);
      browserControl.InitBaseControl(url, 0.0f);
      browserControl.Visibility = Visibility.Visible;
      browserControl.ParentWindow = this.ParentWindow;
      this.mContentGrid.Children.Add((UIElement) browserControl);
      return browserControl;
    }

    private void BrowserLoadErrorEvent(string errorCode, string errorText)
    {
      Logger.Warning("Browser load error in home html");
      this.IsCefErrorReceivedInHomeHtml = true;
      this.CefErrorCode = errorCode;
      this.CefErrorText = errorText;
      if (!RegistryManager.Instance.HomeHtmlErrorHandling || this.browserExpectedLoadTimer.IsEnabled || this.IsHomeHtmlLoaded)
        return;
      this.BrowserFallbackUrlEvent();
    }

    private void BrowserLoadCompleteEvent()
    {
      Logger.Info("Successfully loaded home html");
      this.IsHomeHtmlLoaded = true;
      int num = (int) (DateTime.Now - this.mHomeHtmlLoadStartTime).TotalSeconds * 1000;
      if (!this.IsHomeHtmlRefreshed)
        RegistryManager.Instance.AvgHomeHtmlLoadTime = (RegistryManager.Instance.AvgHomeHtmlLoadTime * 2 + num) / 3;
      BlueStacks.Common.Stats.SendCommonClientStatsAsync("html_home", "HtmlHome_loaded", this.ParentWindow.mVmName, "", num.ToString((IFormatProvider) CultureInfo.InvariantCulture), this.IsHomeHtmlRefreshed.ToString((IFormatProvider) CultureInfo.InvariantCulture), "");
    }

    internal void ReInitHtmlHome()
    {
      this.IsHomeHtmlLoaded = false;
      this.IsCefErrorReceivedInHomeHtml = false;
      this.IsHomeHtmlRefreshed = true;
      this.mBrowser.UpdateUrlAndRefresh(this.ParentWindow.Utils.GetHtmlHomeUrl(true));
      this.mHomeHtmlLoadStartTime = DateTime.Now;
      if (this.browserExpectedLoadTimer == null)
      {
        this.browserExpectedLoadTimer = new DispatcherTimer();
        this.browserExpectedLoadTimer.Tick += new EventHandler(this.DispatcherTimer_Tick);
      }
      this.browserExpectedLoadTimer.Interval = new TimeSpan(0, 0, 0, 0, RegistryManager.Instance.AvgHomeHtmlLoadTime);
      this.browserExpectedLoadTimer.Start();
    }

    internal void DisposeHtmHomeBrowser()
    {
      if (this.mBrowser == null)
        return;
      this.mBrowser.DisposeBrowser();
      this.mContentGrid.Children.Remove((UIElement) this.mBrowser);
      this.mBrowser = (BrowserControl) null;
    }

    internal void RemovePromotionGrid()
    {
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        this.mPromotionGrid.Visibility = Visibility.Hidden;
        this.mPromotionControl.Stop();
        this.mHomeAppManager?.EnableSearchTextBox(true);
      }));
    }

    internal void OpenFrontendAppTabControl(string packageName, PlayStoreAction action)
    {
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        if (action == PlayStoreAction.OpenApp && this.ParentWindow.mAppHandler.IsAppInstalled(packageName) && !"com.android.vending".Equals(packageName, StringComparison.InvariantCultureIgnoreCase))
        {
          AppIconModel appIcon = this.mHomeAppManager.GetAppIcon(packageName);
          if (appIcon == null)
            return;
          if (appIcon.AppIncompatType != AppIncompatType.None && !this.ParentWindow.mTopBar.mAppTabButtons.mDictTabs.ContainsKey(packageName))
            GrmHandler.HandleCompatibility(appIcon.PackageName, this.ParentWindow.mVmName);
          else
            this.ParentWindow.mTopBar.mAppTabButtons.AddAppTab(appIcon.AppName, appIcon.PackageName, appIcon.ActivityName, appIcon.ImageName, true, true, false);
        }
        else
        {
          if (string.IsNullOrEmpty(packageName))
            return;
          AppIconModel appIcon = this.mHomeAppManager.GetAppIcon("com.android.vending");
          if (appIcon == null)
            return;
          if (action == PlayStoreAction.OpenApp)
          {
            this.ParentWindow.mTopBar.mAppTabButtons.AddAppTab(appIcon.AppName, appIcon.PackageName, appIcon.ActivityName, appIcon.ImageName, false, true, false);
            this.ParentWindow.mAppHandler.SwitchWhenPackageNameRecieved = "com.android.vending";
            this.ParentWindow.mAppHandler.LaunchPlayRequestAsync(packageName);
          }
          else
          {
            if (action != PlayStoreAction.SearchApp)
              return;
            this.ParentWindow.mTopBar.mAppTabButtons.AddAppTab(appIcon.AppName, appIcon.PackageName, appIcon.ActivityName, appIcon.ImageName, false, true, false);
            this.ParentWindow.mAppHandler.SwitchWhenPackageNameRecieved = "com.android.vending";
            this.ParentWindow.mAppHandler.SendSearchPlayRequestAsync(packageName);
          }
        }
      }));
    }

    internal void ReloadHomeTabIME()
    {
      this.mBrowser?.CefBrowser?.Focus();
      this.mBrowser?.CefBrowser?.ReloadIME();
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/welcometab.xaml", UriKind.Relative));
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
          this.mContentGrid = (Grid) target;
          break;
        case 2:
          this.mBackground = (CustomPictureBox) target;
          break;
        case 3:
          this.mFrontendPopupControl = (FrontendPopupControl) target;
          break;
        case 4:
          this.mPromotionGrid = (Grid) target;
          break;
        case 5:
          this.mPromotionControl = (PromotionControl) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
