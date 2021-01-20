// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.GuidanceVideoWindow
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace BlueStacks.BlueStacksUI
{
  public class GuidanceVideoWindow : CustomWindow, IDisposable, IComponentConnector
  {
    private BrowserControl mBrowser;
    internal MainWindow ParentWindow;
    private bool disposedValue;
    internal GuidanceVideoWindow mWindow;
    internal Grid mMainBrowserGrid;
    internal Border mMaskBorder;
    internal Grid mBrowserGrid;
    private bool _contentLoaded;

    public GuidanceVideoWindow(MainWindow parentWindow)
    {
      this.ParentWindow = parentWindow;
      this.InitializeComponent();
    }

    private void GuidanceVideoWindow_IsVisibleChanged(
      object _1,
      DependencyPropertyChangedEventArgs eventArgs)
    {
      if (this.IsVisible)
      {
        ClientStats.SendKeyMappingUIStatsAsync("video_clicked", KMManager.sPackageName, KMManager.sVideoMode.ToString());
        this.mBrowser = new BrowserControl();
        this.mBrowser.InitBaseControl(BlueStacksUIUtils.GetVideoTutorialUrl(KMManager.sPackageName, KMManager.sVideoMode.ToString().ToLower(CultureInfo.InvariantCulture), this.ParentWindow?.SelectedConfig?.SelectedControlScheme?.Name), 0.0f);
        this.mBrowser.ParentWindow = this.ParentWindow;
        this.mBrowser.Visibility = Visibility.Visible;
        this.mBrowserGrid.Children.Add((UIElement) this.mBrowser);
      }
      try
      {
        if ((bool) eventArgs.NewValue)
        {
          HTTPUtils.SendRequestToEngineAsync("mute", new Dictionary<string, string>()
          {
            ["explicit"] = "False"
          }, this.ParentWindow.mVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
          this.ParentWindow.mCommonHandler.OnVolumeMuted(true);
        }
        else
        {
          if (this.ParentWindow.IsMuted)
            return;
          HTTPUtils.SendRequestToEngineAsync("unmute", (Dictionary<string, string>) null, this.ParentWindow.mVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
          this.ParentWindow.mCommonHandler.OnVolumeMuted(false);
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to send mute to frontend. Ex: " + ex.Message);
      }
    }

    internal void CloseWindow()
    {
      if (this.mBrowser == null)
        return;
      this.mBrowser.DisposeBrowser();
      this.mBrowserGrid.Children.Remove((UIElement) this.mBrowser);
      this.mBrowser = (BrowserControl) null;
    }

    private void CloseButton_PreviewMouseUp(object sender, MouseButtonEventArgs e)
    {
      this.Close();
    }

    private void mWindow_Closing(object sender, CancelEventArgs e)
    {
      this.CloseWindow();
    }

    protected virtual void Dispose(bool disposing)
    {
      if (this.disposedValue)
        return;
      int num = disposing ? 1 : 0;
      this.mBrowser?.Dispose();
      this.disposedValue = true;
    }

    ~GuidanceVideoWindow()
    {
      this.Dispose(false);
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/keymap/uielement/guidancevideowindow.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.mWindow = (GuidanceVideoWindow) target;
          this.mWindow.IsVisibleChanged += new DependencyPropertyChangedEventHandler(this.GuidanceVideoWindow_IsVisibleChanged);
          this.mWindow.Closing += new CancelEventHandler(this.mWindow_Closing);
          break;
        case 2:
          this.mMainBrowserGrid = (Grid) target;
          break;
        case 3:
          this.mMaskBorder = (Border) target;
          break;
        case 4:
          this.mBrowserGrid = (Grid) target;
          break;
        case 5:
          ((UIElement) target).PreviewMouseUp += new MouseButtonEventHandler(this.CloseButton_PreviewMouseUp);
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
