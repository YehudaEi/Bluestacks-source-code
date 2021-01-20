// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.PopupBrowserControl
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
  public class PopupBrowserControl : UserControl, IDimOverlayControl, IComponentConnector
  {
    private MainWindow ParentWindow;
    internal Border mGridBorder;
    internal Grid mOuterGrid;
    internal TextBlock mTitle;
    internal CustomPictureBox CloseBtn;
    internal Grid mGrid;
    internal BrowserControl mBrowser;
    private bool _contentLoaded;

    bool IDimOverlayControl.Close()
    {
      this.Visibility = Visibility.Hidden;
      this.ClosePopupBrowser();
      return true;
    }

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

    bool IDimOverlayControl.Show()
    {
      this.Visibility = Visibility.Visible;
      return true;
    }

    public bool ShowControlInSeparateWindow { get; set; } = true;

    public bool ShowTransparentWindow { get; set; }

    public PopupBrowserControl()
    {
      this.InitializeComponent();
    }

    public void Init(string url, string title, MainWindow window)
    {
      this.mTitle.Text = title;
      this.mBrowser.mUrl = url;
      this.mBrowser.mGrid = new Grid();
      this.mBrowser.Content = (object) this.mBrowser.mGrid;
      this.mBrowser.CreateNewBrowser();
      if (window != null)
        window.SizeChanged += new SizeChangedEventHandler(this.Window_SizeChanged);
      this.ParentWindow = window;
      this.mBrowser.ParentWindow = window;
      this.FixUpUILayout();
    }

    private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      this.FixUpUILayout();
    }

    private void FixUpUILayout()
    {
      if (this.ParentWindow.mIsFullScreen || this.ParentWindow.WindowState == WindowState.Maximized)
      {
        this.Width = 880.0;
        this.Height = 530.0;
      }
      else
      {
        this.Width = 780.0;
        this.Height = 480.0;
      }
    }

    public void ClosePopupBrowser()
    {
      ClientStats.SendPopupBrowserStatsInMiscASync("closed", this.mBrowser.mUrl);
      if (this.ParentWindow != null)
        this.ParentWindow.HideDimOverlay();
      if (this.mBrowser.CefBrowser != null)
        this.mBrowser.DisposeBrowser();
      this.Visibility = Visibility.Hidden;
    }

    private void CloseBtn_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.ClosePopupBrowser();
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/popupbrowsercontrol.xaml", UriKind.Relative));
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
          this.mGridBorder = (Border) target;
          break;
        case 2:
          this.mOuterGrid = (Grid) target;
          break;
        case 3:
          this.mTitle = (TextBlock) target;
          break;
        case 4:
          this.CloseBtn = (CustomPictureBox) target;
          this.CloseBtn.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.CloseBtn_PreviewMouseLeftButtonUp);
          break;
        case 5:
          this.mGrid = (Grid) target;
          break;
        case 6:
          this.mBrowser = (BrowserControl) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
