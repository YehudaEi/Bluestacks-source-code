// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.DMMRecommendedWindow
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
using Xilium.CefGlue.WPF;

namespace BlueStacks.BlueStacksUI
{
  public class DMMRecommendedWindow : CustomWindow, IComponentConnector
  {
    private MainWindow ParentWindow;
    internal CustomPictureBox mCloseBtn;
    internal BrowserControl mRecommendedBrowserControl;
    private bool _contentLoaded;

    public DMMRecommendedWindow(MainWindow window)
    {
      this.InitializeComponent();
      this.IsShowGLWindow = true;
      this.ParentWindow = window;
      this.Owner = (Window) this.ParentWindow;
      this.Topmost = false;
      this.Left = (this.ParentWindow != null ? this.ParentWindow.Left : 0.0) + (this.ParentWindow != null ? this.ParentWindow.ActualWidth : 0.0);
      this.Top = this.ParentWindow != null ? this.ParentWindow.Top : 0.0;
      this.Height = this.ParentWindow != null ? this.ParentWindow.Height : 0.0;
      this.Width = (this.Height - (this.ParentWindow != null ? (double) this.ParentWindow.ParentWindowHeightDiff : 0.0)) * 9.0 / 16.0 + (this.ParentWindow != null ? (double) this.ParentWindow.ParentWindowWidthDiff : 0.0);
      this.Closing += new CancelEventHandler(this.RecommendedWindow_Closing);
      this.IsVisibleChanged += new DependencyPropertyChangedEventHandler(this.RecommendedWindow_IsVisibleChanged);
    }

    private void RecommendedWindow_IsVisibleChanged(
      object _1,
      DependencyPropertyChangedEventArgs _2)
    {
      this.ParentWindow.mDmmBottomBar.mRecommendedWindowBtn.ImageName = this.Visibility != Visibility.Visible ? "recommend" : "recommend_click";
      this.UpdateSize();
    }

    private void RecommendedWindow_Closing(object sender, CancelEventArgs e)
    {
      this.ParentWindow.mDMMRecommendedWindow.mRecommendedBrowserControl.DisposeBrowser();
      this.ParentWindow.mDMMRecommendedWindow = (DMMRecommendedWindow) null;
    }

    public void Init(string url)
    {
      this.mRecommendedBrowserControl.mUrl = url;
      this.mRecommendedBrowserControl.mGrid = new Grid();
      this.mRecommendedBrowserControl.Content = (object) this.mRecommendedBrowserControl.mGrid;
      this.mRecommendedBrowserControl.CreateNewBrowser();
      this.mRecommendedBrowserControl.ProcessMessageRecieved += new ProcessMessageEventHandler(this.MRecommendedBrowserControl_ProcessMessageRecieved);
    }

    public void UpdateSize()
    {
      this.Top = this.ParentWindow.Top;
      this.Left = this.ParentWindow.Left + this.ParentWindow.Width;
      this.Height = this.ParentWindow.Height;
      this.Width = (this.ParentWindow.Height - (double) this.ParentWindow.ParentWindowHeightDiff) * 9.0 / 16.0 + (double) this.ParentWindow.ParentWindowWidthDiff;
      this.ParentWindow.Focus();
    }

    public void UpdateLocation()
    {
      this.Top = this.ParentWindow.Top;
      this.Left = this.ParentWindow.Left + this.ParentWindow.Width;
    }

    private void MRecommendedBrowserControl_ProcessMessageRecieved(
      object sender,
      ProcessMessageEventArgs e)
    {
    }

    private void mCloseBtn_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.ParentWindow.mIsDMMRecommendedWindowOpen = false;
      this.Hide();
      InteropWindow.ShowWindow(this.ParentWindow.Handle, 9);
      if (this.ParentWindow.Topmost)
        return;
      this.ParentWindow.Topmost = true;
      this.ParentWindow.Topmost = false;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/dmmrecommendedwindow.xaml", UriKind.Relative));
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
      if (connectionId != 1)
      {
        if (connectionId == 2)
          this.mRecommendedBrowserControl = (BrowserControl) target;
        else
          this._contentLoaded = true;
      }
      else
      {
        this.mCloseBtn = (CustomPictureBox) target;
        this.mCloseBtn.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.mCloseBtn_PreviewMouseLeftButtonUp);
      }
    }
  }
}
