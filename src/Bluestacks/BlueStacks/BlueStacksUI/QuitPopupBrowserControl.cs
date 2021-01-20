// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.QuitPopupBrowserControl
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
  public class QuitPopupBrowserControl : UserControl, IDimOverlayControl, IComponentConnector
  {
    internal Grid mGrid;
    internal Grid mBrowserGrid;
    internal Grid mCloseButtonGrid;
    internal CustomPictureBox mCloseButton;
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

    bool IDimOverlayControl.Close()
    {
      return true;
    }

    bool IDimOverlayControl.Show()
    {
      this.Visibility = Visibility.Visible;
      return true;
    }

    private MainWindow ParentWindow { get; set; }

    private BrowserControl mBrowser { get; set; }

    internal string PackageName { get; set; } = "";

    internal string QuitPopupUrl { get; set; } = string.Empty;

    internal bool IsForceReload { get; set; }

    internal bool ShowOnQuit { get; set; }

    public QuitPopupBrowserControl(MainWindow mainWindow)
    {
      this.ParentWindow = mainWindow;
      this.InitializeComponent();
    }

    internal void SetQuitPopParams(
      string url,
      string package,
      bool isForceReload,
      bool showOnQuit)
    {
      this.QuitPopupUrl = url;
      this.IsForceReload = isForceReload;
      this.PackageName = package;
      this.ShowOnQuit = showOnQuit;
    }

    internal void Init(string appPackage)
    {
      this.Width = 740.0;
      this.Height = 490.0;
      this.PackageName = appPackage;
      ClientStats.SendMiscellaneousStatsAsync("quitpopupdisplayed", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, this.QuitPopupUrl, this.PackageName, (string) null, (string) null, (string) null, (string) null, "Android");
      if (this.mBrowser == null)
        this.LoadBrowser();
      this.ParentWindow.mQuitPopupBrowserLoadGrid.Children.Remove((UIElement) this.mBrowser);
      this.mBrowserGrid.Children.Add((UIElement) this.mBrowser);
      this.mBrowser.Visibility = Visibility.Visible;
      this.ParentWindow.ShowDimOverlay((IDimOverlayControl) this);
    }

    internal void LoadBrowser()
    {
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        this.DisposeBrowser();
        this.mBrowser = new BrowserControl();
        this.mBrowser.InitBaseControl(this.QuitPopupUrl, 0.0f);
        this.mBrowser.ParentWindow = this.ParentWindow;
        this.ParentWindow.mQuitPopupBrowserLoadGrid.Children.Add((UIElement) this.mBrowser);
        this.ParentWindow.mQuitPopupBrowserLoadGrid.Visibility = Visibility.Hidden;
        this.mBrowser.CreateNewBrowser();
      }));
    }

    internal void RefreshBrowserUrl(string url)
    {
      try
      {
        this.QuitPopupUrl = url;
        this.mBrowser.UpdateUrlAndRefresh(url);
      }
      catch (Exception ex)
      {
        Logger.Warning("Exception in refreshing quitpopup borwser url: " + ex.ToString());
      }
    }

    private void DisposeBrowser()
    {
      if (this.mBrowser == null)
        return;
      this.mBrowser.DisposeBrowser();
      this.mBrowserGrid.Children.Remove((UIElement) this.mBrowser);
      this.ParentWindow.mQuitPopupBrowserLoadGrid.Children.Remove((UIElement) this.mBrowser);
      this.mBrowser = (BrowserControl) null;
    }

    internal void Close()
    {
      try
      {
        this.Dispatcher.Invoke((Delegate) (() =>
        {
          this.DisposeBrowser();
          this.ParentWindow.mTopBar.mAppTabButtons.mLastPackageForQuitPopupDisplayed = "";
          this.Visibility = Visibility.Hidden;
          ClientStats.SendMiscellaneousStatsAsync("quitpopupclosed", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, this.QuitPopupUrl, this.PackageName, (string) null, (string) null, (string) null, (string) null, "Android");
          this.ParentWindow.HideDimOverlay();
          if (string.IsNullOrEmpty(this.PackageName))
            this.ParentWindow.ForceCloseWindow(false);
          else
            this.ParentWindow.mTopBar.mAppTabButtons.CloseTab(this.PackageName, true, false, false, false, "");
        }));
      }
      catch (Exception ex)
      {
        Logger.Error("Exception when trying to close quit popup. " + ex.ToString());
      }
    }

    private void CloseButtonGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.Close();
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/quitpopupbrowsercontrol.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.mGrid = (Grid) target;
          break;
        case 2:
          this.mBrowserGrid = (Grid) target;
          break;
        case 3:
          this.mCloseButtonGrid = (Grid) target;
          this.mCloseButtonGrid.MouseLeftButtonUp += new MouseButtonEventHandler(this.CloseButtonGrid_MouseLeftButtonUp);
          break;
        case 4:
          this.mCloseButton = (CustomPictureBox) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
