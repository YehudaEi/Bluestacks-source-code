// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.Browser
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using Xilium.CefGlue.WPF;

namespace BlueStacks.BlueStacksUI
{
  public class Browser : WpfCefBrowser
  {
    private bool isLoaded;
    private string url;
    private double zoomLevel;
    private float mCustomZoomLevel;
    private MainWindow mMainWindow;

    public MainWindow ParentWindow
    {
      get
      {
        if (this.mMainWindow == null)
          this.mMainWindow = Window.GetWindow((DependencyObject) this) as MainWindow;
        return this.mMainWindow;
      }
    }

    public Browser(float zoomLevel = 0.0f)
    {
      if (!CefHelper.CefInited)
      {
        string str = "Bluestacks/" + RegistryManager.Instance.ClientVersion;
        string[] args = new string[0];
        Logger.Info("Init cef");
        string mBSTProcessIdentifier = str;
        CefHelper.InitCef(args, mBSTProcessIdentifier);
      }
      this.Loaded += new RoutedEventHandler(this.Browser_Loaded);
      this.LoadingStateChange += new LoadingStateChangeEventHandler(this.Browser_LoadingStateChange);
      this.mCustomZoomLevel = zoomLevel;
      if (RegistryManager.Instance.CefDevEnv != 1)
        return;
      this.mAllowDevTool = true;
      this.mDevToolHeader = this.StartUrl;
    }

    public Browser(string url)
    {
      this.url = url;
    }

    private void Browser_Loaded(object sender, RoutedEventArgs e)
    {
      try
      {
        this.isLoaded = true;
        Matrix transformToDevice = PresentationSource.FromVisual((Visual) sender).CompositionTarget.TransformToDevice;
        ScaleTransform scaleTransform = new ScaleTransform(1.0 / transformToDevice.M11, 1.0 / transformToDevice.M22);
        if (scaleTransform.CanFreeze)
          scaleTransform.Freeze();
        this.LayoutTransform = (Transform) scaleTransform;
        this.zoomLevel = Math.Log(transformToDevice.M11) / Math.Log(1.2);
        if ((double) this.mCustomZoomLevel == 0.0)
          return;
        this.zoomLevel += Math.Log10((double) this.mCustomZoomLevel) / Math.Log10(1.2);
      }
      catch (Exception ex)
      {
        Logger.Warning("Failed to get zoom factor of browser with url {0} and error :{1}", (object) this.url, (object) ex.ToString());
      }
    }

    private void Browser_LoadingStateChange(object sender, LoadingStateChangeEventArgs e)
    {
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        try
        {
          string url = this.getURL();
          if (string.IsNullOrEmpty(url))
            return;
          string packageName = url.Substring(url.LastIndexOf("=", StringComparison.InvariantCulture) + 1);
          if (!url.Contains("play.google.com"))
            return;
          this.ParentWindow.mAppHandler.LaunchPlayRequestAsync(packageName);
        }
        catch (Exception ex)
        {
        }
      }));
      if (e.IsLoading)
        return;
      try
      {
        this.SetZoomLevel(this.zoomLevel);
      }
      catch (Exception ex)
      {
        Logger.Error("Error while setting zoom in browser with url {0} and error :{1}", (object) this.url, (object) ex.ToString());
      }
    }

    public void CallJs(string methodName, object[] args)
    {
      if (!this.isLoaded)
        return;
      new Thread((ThreadStart) (() =>
      {
        try
        {
          if (args.Length == 1)
          {
            string str = args[0].ToString().Replace("%27", "&#39;").Replace("'", "&#39;");
            string code = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "javascript:{0}('{1}')", (object) methodName, (object) str);
            Logger.Info("calling " + methodName);
            this.ExecuteJavaScript(code, this.getURL(), 0);
          }
          else if (args.Length == 0)
          {
            string code = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "javascript:{0}()", (object) methodName);
            Logger.Info("calling " + methodName);
            this.ExecuteJavaScript(code, this.getURL(), 0);
          }
          else
            Logger.Error("Error: function supported for one length array object to be changed later");
        }
        catch (Exception ex)
        {
          Logger.Error(ex.ToString());
        }
      }))
      {
        IsBackground = true
      }.Start();
    }
  }
}
