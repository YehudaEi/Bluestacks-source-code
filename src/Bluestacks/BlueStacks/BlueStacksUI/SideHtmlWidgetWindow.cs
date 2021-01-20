// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.SideHtmlWidgetWindow
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using Microsoft.Win32;
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
  public class SideHtmlWidgetWindow : CustomWindow, IComponentConnector
  {
    private MainWindow ParentWindow;
    private DispatcherTimer dispatcherTimer;
    internal Border mMaskBorder;
    internal Grid mBrowserGrid;
    internal CustomPictureBox mCloseButton;
    private bool _contentLoaded;

    private BrowserControl mBrowser { get; set; }

    private string PackageName { get; set; }

    private bool IsClosing { get; set; }

    public SideHtmlWidgetWindow(MainWindow window, string packageName)
    {
      this.InitializeComponent();
      this.Owner = (Window) window;
      this.ParentWindow = window;
      this.PackageName = packageName;
    }

    private void CustomWindow_Loaded(object sender, RoutedEventArgs e)
    {
      this.Visibility = Visibility.Hidden;
      this.Height = 385.0;
      this.Width = 340.0;
      this.LoadBrowser();
    }

    private void ParentWindow_Activated(object sender, EventArgs e)
    {
      this.CloseWindow();
    }

    internal void LoadBrowser()
    {
      this.mBrowser = new BrowserControl();
      this.mBrowser.BrowserLoadCompleteEvent += new System.Action(this.BrowserLoadCompleteEvent);
      this.mBrowser.InitBaseControl(BlueStacksUIUtils.GetUtcConverterUrl(this.PackageName), 0.0f);
      this.mBrowser.ParentWindow = this.ParentWindow;
      this.mBrowserGrid.Children.Add((UIElement) this.mBrowser);
      this.mBrowser.CreateNewBrowser();
      this.dispatcherTimer = new DispatcherTimer();
      this.dispatcherTimer.Tick += new EventHandler(this.DispatcherTimer_Tick);
      this.dispatcherTimer.Interval = new TimeSpan(0, 0, 20);
      this.dispatcherTimer.Start();
    }

    private void BrowserLoadCompleteEvent()
    {
      Logger.Info("Successfully loaded the html widget");
      this.dispatcherTimer.Stop();
      this.ParentWindow.mCommonHandler.OnUtcConverterLoaded();
      if (this.ParentWindow.WindowState == WindowState.Minimized)
      {
        this.ParentWindow.StateChanged -= new EventHandler(this.ParentWindow_StateChanged);
        this.ParentWindow.StateChanged += new EventHandler(this.ParentWindow_StateChanged);
      }
      else
        this.ShowSideHtmlWidgetWindow();
    }

    private void ParentWindow_StateChanged(object sender, EventArgs e)
    {
      if (this.ParentWindow.WindowState == WindowState.Minimized)
        return;
      this.ShowSideHtmlWidgetWindow();
      this.ParentWindow.StateChanged -= new EventHandler(this.ParentWindow_StateChanged);
    }

    private void ShowSideHtmlWidgetWindow()
    {
      this.ParentWindow.Dispatcher.BeginInvoke((Delegate) (() =>
      {
        this.mBrowser.Visibility = Visibility.Visible;
        this.mCloseButton.Visibility = Visibility.Visible;
        this.Visibility = Visibility.Visible;
        this.Activate();
        if (this.ParentWindow.WindowState == WindowState.Maximized || this.ParentWindow.mIsFullScreen)
        {
          IntereopRect fullscreenMonitorSize = WindowWndProcHandler.GetFullscreenMonitorSize(this.ParentWindow.Handle, true);
          this.Left = (double) (fullscreenMonitorSize.Left + fullscreenMonitorSize.Width) / MainWindow.sScalingFactor - this.Width + 30.0 - (this.ParentWindow.mIsFullScreen || !this.ParentWindow.EngineInstanceRegistry.IsSidebarVisible ? 0.0 : 62.0);
          this.Top = (double) (fullscreenMonitorSize.Top + fullscreenMonitorSize.Height) / MainWindow.sScalingFactor - this.Height + 30.0;
        }
        else
        {
          this.Left = this.ParentWindow.Left + this.ParentWindow.Width - this.Width + 30.0 - (this.ParentWindow.EngineInstanceRegistry.IsSidebarVisible ? 62.0 : 0.0);
          this.Top = this.ParentWindow.Top + this.ParentWindow.Height - this.Height + 30.0;
        }
        if (this.ParentWindow != null)
        {
          this.ParentWindow.Activated -= new EventHandler(this.ParentWindow_Activated);
          this.ParentWindow.Activated += new EventHandler(this.ParentWindow_Activated);
        }
        SystemEvents.DisplaySettingsChanged -= new EventHandler(this.DisplaySettingsChanged);
        SystemEvents.DisplaySettingsChanged += new EventHandler(this.DisplaySettingsChanged);
      }));
    }

    private void DisplaySettingsChanged(object sender, EventArgs e)
    {
      this.CloseWindow();
    }

    private void DispatcherTimer_Tick(object _1, EventArgs _2)
    {
      Logger.Warning("Not able to load the html widget in 20sec");
      this.dispatcherTimer.Stop();
      this.ParentWindow.mCommonHandler.OnUtcConverterLoaded();
      this.CloseWindow();
    }

    private void CloseWindow()
    {
      if (this.IsClosing)
        return;
      this.IsClosing = true;
      if (this.mBrowser != null)
      {
        this.mBrowser.DisposeBrowser();
        this.mBrowserGrid.Children.Remove((UIElement) this.mBrowser);
        this.mBrowser = (BrowserControl) null;
      }
      SystemEvents.DisplaySettingsChanged -= new EventHandler(this.DisplaySettingsChanged);
      BlueStacks.Common.Stats.SendCommonClientStatsAsync("utc-converter", "utc_widget_closed", this.ParentWindow.mVmName, this.PackageName, "", "", "");
      this.Close();
    }

    private void CloseButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.CloseWindow();
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/sidehtmlwidgetwindow.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          ((FrameworkElement) target).Loaded += new RoutedEventHandler(this.CustomWindow_Loaded);
          break;
        case 2:
          this.mMaskBorder = (Border) target;
          break;
        case 3:
          this.mBrowserGrid = (Grid) target;
          break;
        case 4:
          this.mCloseButton = (CustomPictureBox) target;
          this.mCloseButton.MouseLeftButtonUp += new MouseButtonEventHandler(this.CloseButton_MouseLeftButtonUp);
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
