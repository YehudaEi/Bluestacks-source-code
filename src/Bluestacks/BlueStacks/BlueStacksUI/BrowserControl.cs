// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.BrowserControl
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.BlueStacksUI.BTv;
using BlueStacks.Common;
using Microsoft.VisualBasic.Devices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using Windows7.Multitouch.Manipulation;
using Windows7.Multitouch.WPF;
using Xilium.CefGlue;
using Xilium.CefGlue.WPF;

namespace BlueStacks.BlueStacksUI
{
  public class BrowserControl : UserControl, IDisposable
  {
    internal static List<BrowserControl> sAllBrowserControls = new List<BrowserControl>();
    internal static List<string> mFirebaseTagsSubscribed = new List<string>();
    private double zoomLevel = 1.0;
    internal string mFirebaseCallbackMethod = string.Empty;
    internal string mFailedUrl = string.Empty;
    private MainWindow mMainWindow;
    private NoInternetControl mNoInternetControl;
    private Browser mBrowser;
    private CefBrowserHost mBrowserHost;
    internal string mUrl;
    private float customZoomLevel;
    private bool disposedValue;

    public MainWindow ParentWindow
    {
      get
      {
        if (this.mMainWindow == null)
          this.Dispatcher.Invoke((Delegate) (() => this.mMainWindow = Window.GetWindow((DependencyObject) this) as MainWindow));
        return this.mMainWindow;
      }
      set
      {
        this.mMainWindow = value;
      }
    }

    public NoInternetControl NoInternetControl
    {
      get
      {
        if (this.mNoInternetControl == null)
          this.mNoInternetControl = new NoInternetControl(this);
        return this.mNoInternetControl;
      }
      set
      {
        this.mNoInternetControl = value;
      }
    }

    internal Browser CefBrowser
    {
      get
      {
        return this.mBrowser;
      }
      set
      {
        this.mBrowser = value;
        if (this.mBrowser != null)
          return;
        foreach (BrowserControlTags key in this.TagsSubscribedDict.Keys)
          this.mSubscriber?.UnsubscribeTag(key);
      }
    }

    public Grid mGrid { get; set; }

    public Dictionary<BrowserControlTags, JObject> TagsSubscribedDict { get; } = new Dictionary<BrowserControlTags, JObject>();

    public BrowserSubscriber mSubscriber { get; set; }

    public event ProcessMessageEventHandler ProcessMessageRecieved;

    public event System.Action BrowserLoadCompleteEvent;

    public event System.Action BrowserFallbackUrlEvent;

    public event System.Action<string, string> BrowserLoadErrorEvent;

    public BrowserControl()
    {
    }

    public void UpdateUrlAndRefresh(string newUrl)
    {
      this.mUrl = newUrl;
      if (this.CefBrowser == null)
        return;
      this.CefBrowser.StartUrl = this.mUrl;
      this.SetVisibilityOfLoader(Visibility.Visible);
      this.CefBrowser.NavigateTo(this.mUrl);
    }

    internal void NavigateTo(string url)
    {
      if (this.CefBrowser == null)
        return;
      this.SetVisibilityOfLoader(Visibility.Visible);
      this.CefBrowser.NavigateTo(url);
    }

    public void RefreshBrowser()
    {
      if (this.CefBrowser == null)
        return;
      this.CefBrowser.Refresh();
    }

    public BrowserControl(string url)
    {
      this.InitBaseControl(url, 0.0f);
      this.mSubscriber = new BrowserSubscriber(this);
    }

    public void CallJsForMaps(string methodName, string appName, string packageName)
    {
      object[] args = new object[1]{ (object) "" };
      if (!string.IsNullOrEmpty(appName) || !string.IsNullOrEmpty(packageName))
      {
        JObject jobject = new JObject()
        {
          {
            "name",
            (JToken) appName
          },
          {
            "pkg",
            (JToken) packageName
          }
        };
        args[0] = (object) jobject.ToString(Formatting.None);
      }
      if (this.CefBrowser == null)
        return;
      this.CefBrowser.CallJs(methodName, args);
    }

    internal void InitBaseControl(string url, float zoomLevel = 0.0f)
    {
      this.customZoomLevel = zoomLevel;
      this.mUrl = url;
      this.Visibility = Visibility.Hidden;
      this.IsVisibleChanged += new DependencyPropertyChangedEventHandler(this.BrowserControl_IsVisibleChanged);
      this.mGrid = new Grid();
      this.Content = (object) this.mGrid;
      if (!FeatureManager.Instance.IsCreateBrowserOnStart)
        return;
      this.CreateNewBrowser();
    }

    public void DisposeBrowser()
    {
      BrowserControl.sAllBrowserControls.Remove(this);
      if (this.CefBrowser == null)
        return;
      this.mGrid.Children.Remove((UIElement) this.CefBrowser);
      this.CefBrowser.Dispose();
      this.mBrowserHost = (CefBrowserHost) null;
      this.CefBrowser = (Browser) null;
    }

    private void BrowserControl_IsVisibleChanged(object _1, DependencyPropertyChangedEventArgs _2)
    {
      if (!this.IsVisible)
        return;
      Logger.Info("Install Boot: BrowserControl_IsVisibleChanged");
      this.CreateNewBrowser();
    }

    internal void CreateNewBrowser()
    {
      if (this.CefBrowser != null || string.IsNullOrEmpty(this.mUrl))
        return;
      Logger.Info("Install Boot: CreateNewBrowser");
      this.CefBrowser = new Browser(this.customZoomLevel);
      BrowserControl.sAllBrowserControls.Add(this);
      this.CefBrowser.StartUrl = this.mUrl;
      this.mGrid.Children.Add((UIElement) this.CefBrowser);
      this.CefBrowser.LoadEnd += new LoadEndEventHandler(this.MBrowser_LoadEnd);
      this.CefBrowser.ProcessMessageRecieved += new ProcessMessageEventHandler(this.Browser_ProcessMessageRecieved);
      this.CefBrowser.Loaded += new RoutedEventHandler(this.Browser_Loaded);
      this.CefBrowser.LoadError += new LoadErrorEventHandler(this.Browser_LoadError);
      this.CefBrowser.LoadingStateChange += new LoadingStateChangeEventHandler(this.Browser_LoadingStateChange);
      this.CefBrowser.OnBeforePopup += new Predicate<string>(this.CefBrowser_OnBeforePopup);
      this.CefBrowser.OnBeforeClose += new System.Action(this.CefBrowser_OnBeforeClose);
      this.CefBrowser.OnResourceResponse += new WpfCefBrowser.ResourceResponseHandler(this.CefBrowser_OnResourceResponse);
      this.CefBrowser.TitleChange += new System.Action<string>(this.CefBrowser_OnTitleChange);
      this.CefBrowser.FaviconChange += new System.Action<string[]>(this.CefBrowser_OnFaviconChange);
      this.CefBrowser.mWPFCefBrowserExceptionHandler += new ExceptionHandler(this.Browser_WPFCefBrowserExceptionHandler);
      if (RegistryManager.Instance.CefDevEnv == 1)
      {
        this.CefBrowser.mAllowDevTool = true;
        this.CefBrowser.mDevToolHeader = this.mUrl;
      }
      Logger.Info("Install Boot: CreateNewBrowser complete");
      try
      {
        this.AddTouchHandler();
      }
      catch (Exception ex)
      {
        Logger.Info("Install Boot: CreateNewBrowser error");
        Logger.Error("exception adding touch handler: {0}", (object) ex);
      }
    }

    private void CefBrowser_OnBeforeClose()
    {
      this.CloseSelf();
    }

    internal void UnwindOnBeforeCloseEvent()
    {
      this.CefBrowser.OnBeforeClose -= new System.Action(this.CefBrowser_OnBeforeClose);
    }

    internal bool CheckForBrowserCloseOnAppTabSwitch()
    {
      try
      {
        string url = this.CefBrowser?.getURL();
        if (url != null)
        {
          if (!string.IsNullOrEmpty(url))
          {
            Uri uri = new Uri(url);
            if (!string.IsNullOrEmpty(uri.Query))
            {
              NameValueCollection queryString = HttpUtility.ParseQueryString(uri.Query);
              if (queryString != null)
              {
                if (((IEnumerable<string>) queryString.AllKeys).Contains<string>("ForceKillTabOnAway"))
                  return bool.Parse(queryString["ForceKillTabOnAway"]);
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Warning("Exception in CheckForBrowserCloseOnAppTabSwitch, " + ex.ToString());
      }
      return true;
    }

    private void CefBrowser_OnTitleChange(string title)
    {
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        try
        {
          if (string.IsNullOrEmpty(title) || !this.ParentWindow?.mTopBar.mAppTabButtons.mDictTabs.ContainsKey(this.mUrl).GetValueOrDefault())
            return;
          this.ParentWindow.mTopBar.mAppTabButtons.mDictTabs[this.mUrl].mTabLabel.Content = (object) title;
          this.ParentWindow.mTopBar.mAppTabButtons.mDictTabs[this.mUrl].ToolTip = (object) title;
          this.ParentWindow.mTopBar.mAppTabButtons.mDictTabs[this.mUrl].AppName = title;
        }
        catch (Exception ex)
        {
          Logger.Warning("Error in setting title of cef browser: " + ex.ToString());
        }
      }));
    }

    private void CefBrowser_OnFaviconChange(string[] icons)
    {
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        try
        {
          string str = ((IEnumerable<string>) icons).LastOrDefault<string>();
          if (str == null || string.IsNullOrEmpty(str) || !this.ParentWindow?.mTopBar.mAppTabButtons.mDictTabs.ContainsKey(this.mUrl).GetValueOrDefault())
            return;
          string path = Utils.TinyDownloader(str, Path.GetFileNameWithoutExtension(new Uri(this.CefBrowser?.getURL()).LocalPath) + "_" + Path.GetFileNameWithoutExtension(new Uri(str).LocalPath) + ".png", RegistryStrings.PromotionDirectory, false);
          if (!string.IsNullOrEmpty(path) && !System.IO.File.Exists(path))
            path = Utils.TinyDownloader("https://s2.googleusercontent.com/s2/favicons?domain=" + this.CefBrowser?.getURL(), Path.GetFileNameWithoutExtension(new Uri(this.CefBrowser?.getURL()).LocalPath) + "_" + Path.GetFileNameWithoutExtension(new Uri(str).LocalPath) + ".png", RegistryStrings.PromotionDirectory, false);
          if (string.IsNullOrEmpty(path) || !System.IO.File.Exists(path))
            return;
          this.ParentWindow.mTopBar.mAppTabButtons.mDictTabs[this.mUrl].mAppTabIcon.ImageName = path;
        }
        catch (Exception ex)
        {
          Logger.Warning("Error in setting favicon of cef browser: " + ex.ToString());
        }
      }));
    }

    private bool CefBrowser_OnBeforePopup(string url)
    {
      try
      {
        if (Oem.IsOEMDmm)
        {
          BlueStacksUIUtils.OpenUrl(url);
        }
        else
        {
          OpenExternalBrowserLinks externalBrowserLinks = OpenExternalBrowserLinks.newtab;
          if (System.Enum.IsDefined(typeof (OpenExternalBrowserLinks), (object) RegistryManager.Instance.OpenExternalLink))
            externalBrowserLinks = (OpenExternalBrowserLinks) System.Enum.Parse(typeof (OpenExternalBrowserLinks), RegistryManager.Instance.OpenExternalLink);
          switch (externalBrowserLinks)
          {
            case OpenExternalBrowserLinks.externalbrowser:
              BlueStacksUIUtils.OpenUrl(url);
              break;
            case OpenExternalBrowserLinks.sametab:
              this.NavigateTo(url);
              break;
            case OpenExternalBrowserLinks.newtab:
              MainWindow parentWindow = this.ParentWindow;
              if (parentWindow != null)
              {
                parentWindow.Utils.AppendUrlWithCommonParamsAndOpenTab(url, "Browser", "cef_tab", "");
                break;
              }
              break;
          }
        }
        return true;
      }
      catch (Exception ex)
      {
        Logger.Warning("Error in opening external links from the cef browser: " + ex.ToString());
        return false;
      }
    }

    private bool CefBrowser_OnResourceResponse(CefRequest request, CefResponse response)
    {
      try
      {
        if (string.Equals(new Uri(this.mUrl).AbsoluteUri, request.Url, StringComparison.InvariantCultureIgnoreCase))
        {
          if (response.Status == 500)
          {
            if (this.BrowserLoadErrorEvent != null)
            {
              this.BrowserLoadErrorEvent("500:InternalServerError", "Internal Server Error");
              return true;
            }
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Warning("Error in OnResourceResponse in cef browser: " + ex.ToString());
      }
      return false;
    }

    private void SendMessageToBrowserRenderProcess(CefProcessMessage message)
    {
      try
      {
        this.CefBrowser.GetHost().GetBrowser().SendProcessMessage(CefProcessId.Renderer, message);
      }
      catch (Exception ex)
      {
        Logger.Error("exception in sending IPC message to cef render process.." + ex?.ToString());
      }
    }

    private void MBrowser_LoadEnd(object sender, LoadEndEventArgs e)
    {
      try
      {
        this.SetVisibilityOfLoader(Visibility.Hidden);
        if (this.ParentWindow == null)
          return;
        using (CefProcessMessage message = CefProcessMessage.Create("SetVmName"))
        {
          message.Arguments.SetString(0, this.ParentWindow.mVmName);
          this.SendMessageToBrowserRenderProcess(message);
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Error in browser_loadend " + ex?.ToString());
      }
    }

    private void Browser_LoadError(object sender, LoadErrorEventArgs e)
    {
      if (this.CefBrowser == null)
        return;
      Logger.Warning("Cef error code: {0}, error text: {1}", (object) e.ErrorCode, (object) e.ErrorText);
      if (e.ErrorCode == CefErrorCode.InternetDisconnected || e.ErrorCode == CefErrorCode.TunnelConnectionFailed || (e.ErrorCode == CefErrorCode.ConnectionReset || e.ErrorCode == (CefErrorCode) -21) || (e.ErrorCode == (CefErrorCode) -130 || e.ErrorCode == CefErrorCode.NameNotResolved && !Utils.CheckForInternetConnection()))
      {
        this.mFailedUrl = e.FailedUrl;
        this.Dispatcher.Invoke((Delegate) (() =>
        {
          if (this.BrowserFallbackUrlEvent != null)
          {
            this.BrowserFallbackUrlEvent();
          }
          else
          {
            if (this.mGrid.Children.Contains((UIElement) this.NoInternetControl))
              return;
            this.mGrid.Children.Add((UIElement) this.NoInternetControl);
          }
        }));
      }
      else
      {
        System.Action<string, string> browserLoadErrorEvent = this.BrowserLoadErrorEvent;
        if (browserLoadErrorEvent == null)
          return;
        browserLoadErrorEvent(e.ErrorCode.ToString(), e.ErrorText);
      }
    }

    private void SetVisibilityOfLoader(Visibility visibility)
    {
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        try
        {
          if (!(this.Parent is Grid parent))
            return;
          IEnumerable<CustomPictureBox> source = parent.Children.OfType<CustomPictureBox>();
          if (source == null || !source.Any<CustomPictureBox>())
            return;
          source.First<CustomPictureBox>().Visibility = visibility;
        }
        catch (Exception ex)
        {
          Logger.Error("Exception in set visibility of web page loader : " + ex.ToString());
        }
      }));
    }

    private void Browser_WPFCefBrowserExceptionHandler(object sender, Exception e)
    {
      Logger.Error("Handle Error in wpf cef browser.." + e.ToString());
    }

    private void Browser_LoadingStateChange(object sender, LoadingStateChangeEventArgs e)
    {
      try
      {
        if ((double) this.customZoomLevel == 0.0)
          this.CefBrowser.SetZoomLevel(this.zoomLevel);
        if (!e.IsLoading)
          return;
        this.Dispatcher.Invoke((Delegate) (() =>
        {
          if (!this.mGrid.Children.Contains((UIElement) this.NoInternetControl))
            return;
          this.mGrid.Children.Remove((UIElement) this.NoInternetControl);
        }));
      }
      catch (Exception ex)
      {
        Logger.Error("Error while setting zoom in browser with url {0} and error :{1}", (object) this.mUrl, (object) ex.ToString());
      }
    }

    private void Browser_Loaded(object sender, RoutedEventArgs e)
    {
      try
      {
        if ((double) this.customZoomLevel != 0.0)
          return;
        Matrix transformToDevice = PresentationSource.FromVisual((Visual) sender).CompositionTarget.TransformToDevice;
        ScaleTransform scaleTransform = new ScaleTransform(1.0 / transformToDevice.M11, 1.0 / transformToDevice.M22);
        if (scaleTransform.CanFreeze)
          scaleTransform.Freeze();
        this.CefBrowser.LayoutTransform = (Transform) scaleTransform;
        this.zoomLevel = Math.Log(transformToDevice.M11) / Math.Log(1.2);
      }
      catch (Exception ex)
      {
        Logger.Error("Error while getting zoom of browser with url {0} and error :{1}", (object) this.mUrl, (object) ex.ToString());
      }
    }

    private void AddTouchHandler()
    {
      try
      {
        if (!Windows7.Multitouch.Handler.DigitizerCapabilities.IsMultiTouchReady)
          return;
        Logger.Info("adding touch handler");
        ManipulationProcessor mManipulationProcessor = new ManipulationProcessor(ProcessorManipulations.TRANSLATE_Y);
        this.mBrowserHost = this.CefBrowser.GetHost();
        Factory.EnableStylusEvents((Window) this.ParentWindow);
        this.StylusDown += (StylusDownEventHandler) ((s, e) => mManipulationProcessor.ProcessDown((uint) e.StylusDevice.Id, e.GetPosition((IInputElement) this).ToDrawingPointF()));
        this.StylusUp += (StylusEventHandler) ((s, e) => mManipulationProcessor.ProcessUp((uint) e.StylusDevice.Id, e.GetPosition((IInputElement) this).ToDrawingPointF()));
        this.StylusMove += (StylusEventHandler) ((s, e) => mManipulationProcessor.ProcessMove((uint) e.StylusDevice.Id, e.GetPosition((IInputElement) this).ToDrawingPointF()));
        mManipulationProcessor.ManipulationDelta += new EventHandler<Windows7.Multitouch.Manipulation.ManipulationDeltaEventArgs>(this.ProcessManipulationDelta);
      }
      catch (Exception ex)
      {
        Logger.Error("exception in adding touch handler: {0}", (object) ex);
      }
    }

    private void ProcessManipulationDelta(object sender, Windows7.Multitouch.Manipulation.ManipulationDeltaEventArgs e)
    {
      Logger.Debug("ProcessManipulationDelta.." + e.TranslationDelta.Height.ToString() + "..." + e.Location.ToString());
      if (this.mBrowserHost == null)
        this.mBrowserHost = this.CefBrowser.GetHost();
      CefMouseEvent @event = new CefMouseEvent();
      ref CefMouseEvent local1 = ref @event;
      PointF location = e.Location;
      int x = (int) location.X;
      local1.X = x;
      ref CefMouseEvent local2 = ref @event;
      location = e.Location;
      int y = (int) location.Y;
      local2.Y = y;
      this.mBrowserHost.SendMouseWheelEvent(@event, 0, (int) e.TranslationDelta.Height);
      this.mBrowserHost.SendMouseMoveEvent(new CefMouseEvent(0, 0, CefEventFlags.None), false);
    }

    private void Browser_ProcessMessageRecieved(object sender, ProcessMessageEventArgs e)
    {
      Logger.Info("Browser to client web call :" + e.Message.Name);
      if (string.Equals(e.Message.Name, "InstallApp", StringComparison.InvariantCulture))
      {
        CefListValue arguments = e.Message.Arguments;
        string iconUrl = arguments.GetString(0);
        string appName = arguments.GetString(1);
        string apkUrl = arguments.GetString(2);
        string str = arguments.GetString(3);
        string timestamp = arguments.GetString(4);
        string source = string.IsNullOrEmpty(arguments.GetString(5)) ? "BSAppCenter" : arguments.GetString(5);
        this.InstallApp(iconUrl, appName, apkUrl, str, timestamp);
        this.ParentWindow.Utils.SendMessageToAndroidForAffiliate(str, source);
      }
      else if (string.Equals(e.Message.Name, "InstallAppGooglePlay", StringComparison.InvariantCulture))
      {
        CefListValue arguments = e.Message.Arguments;
        arguments.GetString(0);
        arguments.GetString(1);
        arguments.GetString(2);
        string str = arguments.GetString(3);
        string source = string.IsNullOrEmpty(arguments.GetString(4)) ? "BSAppCenter" : arguments.GetString(4);
        this.ShowAppInPlayStore(str);
        this.ParentWindow.Utils.SendMessageToAndroidForAffiliate(str, source);
      }
      else if (string.Equals(e.Message.Name, "InstallAppGooglePlayPopup", StringComparison.InvariantCulture))
      {
        CefListValue arguments = e.Message.Arguments;
        arguments.GetString(0);
        string appName = arguments.GetString(1);
        arguments.GetString(2);
        string str = arguments.GetString(3);
        string source = string.IsNullOrEmpty(arguments.GetString(4)) ? "BSAppCenter" : arguments.GetString(4);
        this.ShowAppInPlayStorePopup(str, appName);
        this.ParentWindow.Utils.SendMessageToAndroidForAffiliate(str, source);
      }
      else if (string.Equals(e.Message.Name, "DownloadInstallOem", StringComparison.InvariantCulture))
      {
        CefListValue arguments = e.Message.Arguments;
        string oem = arguments.GetString(0);
        string abiValue = arguments.GetString(1);
        this.Dispatcher.Invoke((Delegate) (() =>
        {
          if (string.IsNullOrEmpty(oem))
            return;
          AppPlayerModel appPlayerModel = InstalledOem.GetAppPlayerModel(oem, abiValue);
          if (appPlayerModel == null)
            return;
          new DownloadInstallOem(this.ParentWindow).DownloadOem(appPlayerModel);
        }));
      }
      else if (string.Equals(e.Message.Name, "CancelOemDownload", StringComparison.InvariantCulture))
      {
        CefListValue arguments = e.Message.Arguments;
        string oem = arguments.GetString(0);
        string abiValue = arguments.GetString(1);
        this.Dispatcher.Invoke((Delegate) (() =>
        {
          if (string.IsNullOrEmpty(oem))
            return;
          InstalledOem.GetAppPlayerModel(oem, abiValue)?.CancelOemDownload();
        }));
      }
      else if (string.Equals(e.Message.Name, "LaunchAppInDifferentOem", StringComparison.InvariantCulture))
      {
        CefListValue arguments = e.Message.Arguments;
        string oem = arguments.GetString(0);
        string abiValue = arguments.GetString(1);
        string vmname = arguments.GetString(2);
        string packageName = arguments.GetString(3);
        string actionWithRemainingInstances = arguments.GetString(4);
        this.Dispatcher.Invoke((Delegate) (() =>
        {
          if (string.IsNullOrEmpty(oem))
            return;
          InstalledOem.LaunchOemInstance(oem, abiValue, vmname, packageName, actionWithRemainingInstances);
        }));
      }
      else if (string.Equals(e.Message.Name, "UninstallApp", StringComparison.InvariantCulture))
      {
        string packageName = e.Message.Arguments.GetString(0);
        this.Dispatcher.Invoke((Delegate) (() => this.ParentWindow.mAppInstaller?.UninstallApp(packageName)));
      }
      else if (string.Equals(e.Message.Name, "GetUpdatedGrm", StringComparison.InvariantCulture))
        this.Dispatcher.Invoke((Delegate) (() => GrmHandler.SendUpdateGrmPackagesToBrowser(this.ParentWindow.mVmName)));
      else if (string.Equals(e.Message.Name, "RetryApkInstall", StringComparison.InvariantCulture))
      {
        string apkFilePath = e.Message.Arguments.GetString(0);
        this.Dispatcher.Invoke((Delegate) (() =>
        {
          if (!System.IO.File.Exists(apkFilePath))
            return;
          new DownloadInstallApk(this.ParentWindow).InstallApk(apkFilePath, false);
        }));
      }
      else if (string.Equals(e.Message.Name, "ChooseAndInstallApk", StringComparison.InvariantCulture))
        this.Dispatcher.Invoke((Delegate) (() => new DownloadInstallApk(this.ParentWindow).ChooseAndInstallApk()));
      else if (string.Equals(e.Message.Name, "GoogleSearch", StringComparison.InvariantCulture))
      {
        string searchString = e.Message.Arguments.GetString(0);
        this.SearchAppInPlayStore(searchString);
        ClientStats.SendGPlayClickStats(new Dictionary<string, string>()
        {
          {
            "query",
            searchString
          },
          {
            "source",
            "bs3_appsearch"
          }
        });
      }
      else if (string.Equals(e.Message.Name, "CloseSearch", StringComparison.InvariantCulture))
        this.CloseSearch();
      else if (string.Equals(e.Message.Name, "RefreshSearch", StringComparison.InvariantCulture))
        this.RefreshSearch(e.Message.Arguments.GetString(0));
      else if (string.Equals(e.Message.Name, "OfflineHtmlHomeUrl", StringComparison.InvariantCulture))
        RegistryManager.Instance.OfflineHtmlHomeUrl = e.Message.Arguments.GetString(0);
      else if (string.Equals(e.Message.Name, "RefreshHomeHtml", StringComparison.InvariantCulture))
        this.Dispatcher.Invoke((Delegate) (() => this.ParentWindow.Utils.RefreshHtmlHomeUrl()));
      else if (string.Equals(e.Message.Name, "SetWebAppVersion", StringComparison.InvariantCulture))
      {
        try
        {
          RegistryManager.Instance.WebAppVersion = e.Message.Arguments.GetString(0);
        }
        catch (Exception ex)
        {
          Logger.Error("Error in setting webappversion : " + ex.ToString());
        }
      }
      else if (string.Equals(e.Message.Name, "ShowWebPage", StringComparison.InvariantCulture))
      {
        CefListValue arguments = e.Message.Arguments;
        this.ShowWebPage(arguments.GetString(0), arguments.GetString(1));
      }
      else if (string.Equals(e.Message.Name, "UnmuteAppPlayer", StringComparison.InvariantCulture))
      {
        this.Dispatcher.Invoke((Delegate) (() => this.ParentWindow.Utils.SetVolumeInFrontendAsync(this.ParentWindow.EngineInstanceRegistry.Volume > 0 ? this.ParentWindow.EngineInstanceRegistry.Volume : 50)));
      }
      else
      {
        if (string.Equals(e.Message.Name, "CloseBlockerAd", StringComparison.InvariantCulture))
          return;
        if (string.Equals(e.Message.Name, "CheckIfPremium", StringComparison.InvariantCulture))
        {
          string isPremium = e.Message.Arguments.GetString(0);
          this.Dispatcher.Invoke((Delegate) (() =>
          {
            if (isPremium.Equals("true", StringComparison.InvariantCultureIgnoreCase))
            {
              RegistryManager.Instance.IsPremium = true;
              this.ParentWindow.mTopBar.ChangeUserPremiumButton(true);
            }
            else
            {
              RegistryManager.Instance.IsPremium = false;
              this.ParentWindow.mTopBar.ChangeUserPremiumButton(false);
            }
            System.Action<bool> recommendationHandler = PromotionObject.AppRecommendationHandler;
            if (recommendationHandler == null)
              return;
            recommendationHandler(true);
          }));
        }
        else
        {
          if (string.Equals(e.Message.Name, "GetImpressionId", StringComparison.InvariantCulture))
            return;
          if (string.Equals(e.Message.Name, "sendFirebaseNotification", StringComparison.InvariantCulture))
          {
            string json = e.Message.Arguments.GetString(0);
            this.Dispatcher.Invoke((Delegate) (() => CloudNotificationManager.Instance.HandleCloudNotification(json, this.ParentWindow.mVmName)));
          }
          else if (string.Equals(e.Message.Name, "PikaWorldProfileAdded", StringComparison.InvariantCulture))
            RegistryManager.Instance.PikaWorldId = e.Message.Arguments.GetString(0);
          else if (string.Equals(e.Message.Name, "subscribeModule", StringComparison.InvariantCulture))
          {
            string[] tagsList = e.Message.Arguments.GetString(0).Split(new char[1]
            {
              ','
            }, StringSplitOptions.None);
            this.PopulateTagsInfo(tagsList, tagsList[0]);
          }
          else if (string.Equals(e.Message.Name, "unsubscribeModule", StringComparison.InvariantCulture))
            this.RemoveTagsInfo(e.Message.Arguments.GetString(0).Split(new char[1]
            {
              ','
            }, StringSplitOptions.None));
          else if (string.Equals(e.Message.Name, "subscribeVmSpecificClientTags", StringComparison.InvariantCulture))
          {
            Dictionary<string, List<string>> dictionary = JToken.Parse(e.Message.Arguments.GetString(0)).ToObject<Dictionary<string, List<string>>>();
            if (this.mSubscriber == null)
              this.mSubscriber = new BrowserSubscriber(this);
            foreach (KeyValuePair<string, List<string>> keyValuePair in dictionary)
            {
              foreach (string str in keyValuePair.Value)
              {
                if (System.Enum.IsDefined(typeof (BrowserControlTags), (object) str))
                {
                  BrowserControlTags browserControlTags = (BrowserControlTags) System.Enum.Parse(typeof (BrowserControlTags), str);
                  if (!this.TagsSubscribedDict.ContainsKey(browserControlTags))
                  {
                    JObject jobject = new JObject()
                    {
                      ["ClientTag"] = (JToken) str,
                      ["CallbackFunction"] = (JToken) keyValuePair.Key,
                      ["IsReceiveFromAllVm"] = (JToken) false
                    };
                    this.TagsSubscribedDict.Add(browserControlTags, jobject);
                  }
                  this.mSubscriber?.SubscribeTag(browserControlTags);
                  if (browserControlTags == BrowserControlTags.getVmInfo)
                    Publisher.PublishMessage(BrowserControlTags.getVmInfo, this.ParentWindow.mVmName, new JObject((object) new JProperty("VmId", (object) Utils.GetVmIdFromVmName(this.ParentWindow.mVmName))));
                  if (browserControlTags == BrowserControlTags.bootComplete && this.ParentWindow.mGuestBootCompleted)
                  {
                    Logger.Info("Sending boot complete to browser immediately");
                    Publisher.PublishMessage(BrowserControlTags.bootComplete, this.ParentWindow.mVmName, (JObject) null);
                  }
                  if (browserControlTags == BrowserControlTags.getVolumeLevel)
                    Publisher.PublishMessage(BrowserControlTags.getVolumeLevel, this.ParentWindow.mVmName, new JObject()
                    {
                      ["MutedState"] = (JToken) this.ParentWindow.IsMuted,
                      ["VolumeLevel"] = (JToken) this.ParentWindow.Utils.CurrentVolumeLevel
                    });
                }
              }
            }
          }
          else if (string.Equals(e.Message.Name, "subscribeClientTags", StringComparison.InvariantCulture))
          {
            JArray jarray = JArray.Parse(e.Message.Arguments.GetString(0));
            if (this.mSubscriber == null)
              this.mSubscriber = new BrowserSubscriber(this);
            for (int index = 0; index < jarray.Count; ++index)
            {
              JObject jobject = JObject.Parse(jarray[index].ToString());
              if (System.Enum.IsDefined(typeof (BrowserControlTags), (object) jobject["ClientTag"].ToString()))
              {
                BrowserControlTags browserControlTags = (BrowserControlTags) System.Enum.Parse(typeof (BrowserControlTags), jobject["ClientTag"].ToString());
                if (!this.TagsSubscribedDict.ContainsKey(browserControlTags))
                  this.TagsSubscribedDict.Add(browserControlTags, jobject);
                this.mSubscriber?.SubscribeTag(browserControlTags);
                if (browserControlTags == BrowserControlTags.getVmInfo)
                  Publisher.PublishMessage(BrowserControlTags.getVmInfo, this.ParentWindow.mVmName, new JObject((object) new JProperty("VmId", (object) Utils.GetVmIdFromVmName(this.ParentWindow.mVmName))));
                if (browserControlTags == BrowserControlTags.bootComplete && this.ParentWindow.mGuestBootCompleted)
                {
                  Logger.Info("Sending boot complete to browser immediately");
                  Publisher.PublishMessage(BrowserControlTags.bootComplete, this.ParentWindow.mVmName, (JObject) null);
                }
                if (browserControlTags == BrowserControlTags.getVolumeLevel)
                  Publisher.PublishMessage(BrowserControlTags.getVolumeLevel, this.ParentWindow.mVmName, new JObject()
                  {
                    ["MutedState"] = (JToken) this.ParentWindow.IsMuted,
                    ["VolumeLevel"] = (JToken) this.ParentWindow.Utils.CurrentVolumeLevel
                  });
              }
            }
          }
          else if (string.Equals(e.Message.Name, "unsubscribeClientTags", StringComparison.InvariantCulture))
          {
            foreach (string str in JArray.Parse(e.Message.Arguments.GetString(0)).ToObject<List<string>>())
            {
              if (System.Enum.IsDefined(typeof (BrowserControlTags), (object) str))
              {
                BrowserControlTags browserControlTags = (BrowserControlTags) System.Enum.Parse(typeof (BrowserControlTags), str);
                if (this.TagsSubscribedDict.ContainsKey(browserControlTags))
                {
                  this.TagsSubscribedDict.Remove(browserControlTags);
                  this.mSubscriber?.UnsubscribeTag(browserControlTags);
                }
              }
            }
          }
          else if (string.Equals(e.Message.Name, "ApplyThemeName", StringComparison.InvariantCulture))
          {
            string themeName = e.Message.Arguments.GetString(0);
            this.Dispatcher.Invoke((Delegate) (() =>
            {
              this.ParentWindow.Utils.ApplyTheme(themeName);
              this.ParentWindow.Utils.RestoreWallpaperImageForAllVms();
              BlueStacksUIColorManager.ApplyTheme(themeName);
            }));
          }
          else if (string.Equals(e.Message.Name, "GoToMapsTab", StringComparison.InvariantCulture))
            this.Dispatcher.Invoke((Delegate) (() => this.GoToMapsTab()));
          else if (string.Equals(e.Message.Name, "HandleClick", StringComparison.InvariantCulture))
          {
            string json = "";
            try
            {
              json = e.Message.Arguments.GetString(0);
              JToken res = JToken.Parse(json);
              this.Dispatcher.Invoke((Delegate) (() =>
              {
                SerializableDictionary<string, string> serializableDictionary = res.ToSerializableDictionary<string>();
                if (!serializableDictionary.ContainsKey("click_generic_action") || serializableDictionary["click_generic_action"] != "CreateInstanceSameEngine")
                  this.ParentWindow.HideDimOverlay();
                this.ParentWindow.Utils.HandleGenericActionFromDictionary((Dictionary<string, string>) serializableDictionary, "handle_browser_click", "");
              }));
            }
            catch (Exception ex)
            {
              Logger.Error("Error when processing click action received from gmapi. JsonString: " + json + Environment.NewLine + "Error: " + ex.ToString());
            }
          }
          else if (string.Equals(e.Message.Name, "UpdateQuestRules", StringComparison.InvariantCulture))
          {
            string json = "";
            try
            {
              json = e.Message.Arguments.GetString(0);
              PromotionManager.ReadQuests(JToken.Parse(json), true);
            }
            catch (Exception ex)
            {
              Logger.Error("Error when processing UpdateQuestRules. JsonString: " + json + Environment.NewLine + "Error: " + ex.ToString());
            }
          }
          else if (string.Equals(e.Message.Name, "GetGamepadConnectionStatus", StringComparison.InvariantCulture))
            this.Dispatcher.Invoke((Delegate) (() =>
            {
              if (this.ParentWindow == null)
                return;
              BlueStacksUIUtils.SendGamepadStatusToBrowsers(this.ParentWindow.IsGamepadConnected);
            }));
          else if (string.Equals(e.Message.Name, "CloseAnyPopup", StringComparison.InvariantCulture))
            this.Dispatcher.Invoke((Delegate) (() =>
            {
              if (this.ParentWindow == null)
                return;
              this.ParentWindow.HideDimOverlay();
            }));
          else if (string.Equals(e.Message.Name, "SearchAppcenter", StringComparison.OrdinalIgnoreCase))
          {
            string searchText = e.Message.Arguments.GetString(0);
            this.Dispatcher.Invoke((Delegate) (() => this.ParentWindow.mCommonHandler.SearchAppCenter(searchText)));
          }
          else if (string.Equals(e.Message.Name, "DownloadMacro", StringComparison.OrdinalIgnoreCase))
          {
            string macroData = e.Message.Arguments.GetString(0);
            this.Dispatcher.Invoke((Delegate) (() => this.ParentWindow.Utils.DownloadAndUpdateMacro(macroData)));
          }
          else if (string.Equals(e.Message.Name, "ChangeControlScheme", StringComparison.OrdinalIgnoreCase))
          {
            string schemeSelected = e.Message.Arguments.GetString(0);
            this.Dispatcher.Invoke((Delegate) (() => KMManager.sGuidanceWindow?.SelectControlScheme(schemeSelected)));
          }
          else if (string.Equals(e.Message.Name, "CloseOnBoarding", StringComparison.OrdinalIgnoreCase))
          {
            string json = e.Message.Arguments.GetString(0);
            Logger.Info("CloseOnBoarding response from browser : " + json);
            JObject res = JObject.Parse(json);
            this.Dispatcher.Invoke((Delegate) (() =>
            {
              this.ParentWindow.StaticComponents.mSelectedTabButton.OnboardingControl?.Close();
              this.ParentWindow.StaticComponents.mSelectedTabButton.ShowBlurbOnboarding(res);
              this.ParentWindow.HideDimOverlay();
              KMManager.sGuidanceWindow?.DimOverLayVisibility(Visibility.Collapsed);
            }));
          }
          else if (string.Equals(e.Message.Name, "BrowserLoaded", StringComparison.OrdinalIgnoreCase))
            this.Dispatcher.Invoke((Delegate) (() =>
            {
              System.Action loadCompleteEvent = this.BrowserLoadCompleteEvent;
              if (loadCompleteEvent == null)
                return;
              loadCompleteEvent();
            }));
          else if (string.Equals(e.Message.Name, "LaunchInstance", StringComparison.OrdinalIgnoreCase))
          {
            CefListValue arguments = e.Message.Arguments;
            string newVmname = arguments.GetString(0);
            string campaignId = arguments.GetString(1);
            string instanceLaunchType = arguments.GetString(2);
            this.Dispatcher.Invoke((Delegate) (() =>
            {
              if (string.IsNullOrEmpty(newVmname))
                return;
              string str1 = Path.Combine(RegistryStrings.InstallDir, "HD-RunApp.exe");
              string str2 = "-vmname " + newVmname;
              JObject jobject = new JObject();
              if (!string.IsNullOrEmpty(campaignId))
                jobject.Add("campaign_id", (JToken) campaignId);
              if (!string.IsNullOrEmpty(instanceLaunchType))
              {
                int result = 0;
                if (int.TryParse(instanceLaunchType, out result) && result == 1)
                {
                  jobject.Add("isFarmingInstance", (JToken) "true");
                  str2 += " -h";
                }
              }
              jobject.Add("source_vmname", (JToken) this.ParentWindow.mVmName);
              if (jobject != null)
                str2 = str2 + " -json " + Uri.EscapeUriString(jobject.ToString(Formatting.None));
              Process.Start(new ProcessStartInfo()
              {
                Arguments = str2,
                UseShellExecute = false,
                CreateNoWindow = true,
                FileName = str1
              });
            }));
          }
          else
          {
            try
            {
              object[] parameters = (object[]) null;
              if (e.Message.Arguments.Count > 0)
              {
                parameters = new object[e.Message.Arguments.Count];
                for (int index = 0; index < e.Message.Arguments.Count; ++index)
                {
                  if (e.Message.Arguments.GetString(index) != null)
                  {
                    parameters[index] = (object) e.Message.Arguments.GetString(index).ToString((IFormatProvider) CultureInfo.InvariantCulture);
                    Logger.Info("web api call.." + e.Message.Name + "..with args.." + e.Message.Arguments.GetString(index).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                  }
                  else
                    parameters[index] = (object) string.Empty;
                }
              }
              this.GetType().GetMethod(e.Message.Name).Invoke((object) this, parameters);
            }
            catch (Exception ex)
            {
              Logger.Error("Error in executing gmapi " + e.Message.Name + ": " + ex.ToString());
            }
            ProcessMessageEventHandler processMessageRecieved = this.ProcessMessageRecieved;
            if (processMessageRecieved == null)
              return;
            processMessageRecieved(sender, e);
          }
        }
      }
    }

    internal void RemoveTagsInfo(string[] tagsList)
    {
      foreach (string tags in tagsList)
      {
        if (BrowserControl.mFirebaseTagsSubscribed.Contains(tags))
          BrowserControl.mFirebaseTagsSubscribed.Remove(tags);
      }
    }

    public void PopulateTagsInfo(string[] tagsList, string methodName)
    {
      if (tagsList != null)
      {
        foreach (string tags in tagsList)
        {
          if (!string.Equals(tags, methodName, StringComparison.InvariantCultureIgnoreCase))
            BrowserControl.mFirebaseTagsSubscribed.Add(tags);
        }
      }
      this.mFirebaseCallbackMethod = methodName;
    }

    public void GoToMapsTab()
    {
      if (this.ParentWindow == null)
        return;
      this.ParentWindow.mTopBar.mAppTabButtons.GoToTab("pikaworld", true, false);
    }

    private void InstallApp(
      string iconUrl,
      string appName,
      string apkUrl,
      string package,
      string timestamp = "")
    {
      if (string.IsNullOrEmpty(package))
        return;
      this.Dispatcher.Invoke((Delegate) (() => new DownloadInstallApk(this.ParentWindow).DownloadAndInstallApp(iconUrl, appName, apkUrl, package, false, true, timestamp)));
    }

    private void ShowAppInPlayStore(string packageName)
    {
      this.Dispatcher.Invoke((Delegate) (() => this.ParentWindow.mWelcomeTab.OpenFrontendAppTabControl(packageName, PlayStoreAction.OpenApp)));
    }

    private void ShowAppInPlayStorePopup(string packageName, string appName)
    {
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        this.ParentWindow.mWelcomeTab.mFrontendPopupControl.Init(packageName, appName, PlayStoreAction.OpenApp, false);
        this.ParentWindow.mWelcomeTab.mFrontendPopupControl.Visibility = Visibility.Visible;
      }));
    }

    private void SearchAppInPlayStore(string searchString)
    {
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        if (searchString == null)
          return;
        this.ParentWindow.mWelcomeTab.OpenFrontendAppTabControl(searchString, PlayStoreAction.SearchApp);
      }));
    }

    private void CloseSearch()
    {
      if (this.CefBrowser == null)
        return;
      this.CefBrowser.NavigateTo(this.mUrl);
    }

    private void RefreshSearch(string _)
    {
    }

    public void ShowWebPage(string title, string webUrl)
    {
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        if (title == null)
          title = "";
        if (this.ParentWindow != null)
        {
          this.ParentWindow.Utils.AppendUrlWithCommonParamsAndOpenTab(webUrl, title, "cef_tab", "");
        }
        else
        {
          MainWindow activatedWindow = (MainWindow) null;
          if (BlueStacksUIUtils.DictWindows.Count > 0)
            activatedWindow = BlueStacksUIUtils.DictWindows.Values.First<MainWindow>();
          activatedWindow.Dispatcher.Invoke((Delegate) (() => activatedWindow.Utils.AppendUrlWithCommonParamsAndOpenTab(webUrl, title, "cef_tab", "")));
        }
      }));
    }

    public void CloseSelf()
    {
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        try
        {
          foreach (AppTabButton appTabButton in this.ParentWindow.mTopBar.mAppTabButtons.mDictTabs.Values.Where<AppTabButton>((Func<AppTabButton, bool>) (tab => tab.mTabType == TabType.WebTab)).ToList<AppTabButton>())
          {
            BrowserControl browserControl = appTabButton.GetBrowserControl();
            if (browserControl != null && browserControl.GetHashCode() == this.GetHashCode())
            {
              this.ParentWindow.mTopBar.mAppTabButtons.CloseTab(appTabButton.TabKey, false, false, false, false, "");
              break;
            }
          }
        }
        catch (Exception ex)
        {
          Logger.Warning("Exception in closing browser tab: " + ex.ToString());
        }
      }));
    }

    public void CloseBrowserQuitPopup()
    {
      this.ParentWindow.CloseBrowserQuitPopup();
    }

    public void GetRealtimeAppUsage(string callBackFunction)
    {
      try
      {
        Dictionary<string, Dictionary<string, long>> realtimeDictionary = AppUsageTimer.GetRealtimeDictionary();
        if (string.IsNullOrEmpty(callBackFunction))
          return;
        this.CallBackToHtml(callBackFunction, JSONUtils.GetJSONObjectString<long>(realtimeDictionary[this.ParentWindow.mVmName]));
      }
      catch (Exception ex)
      {
        Logger.Error("Error while sending realtime dictionary to gmapi" + ex.ToString());
      }
    }

    public void GetLoginAccountsForCurrentVm(string callBackFunction)
    {
      if (string.IsNullOrEmpty(callBackFunction))
        return;
      string vmName = "Android";
      if (!string.IsNullOrEmpty(this.ParentWindow?.mVmName))
        vmName = this.ParentWindow.mVmName;
      string data = Regex.Replace(HTTPUtils.SendRequestToGuest("getGoogleAccounts", (Dictionary<string, string>) null, vmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp").Replace("\n", "").Replace("\r", ""), "\\s+", " ", RegexOptions.Multiline);
      this.CallBackToHtml(callBackFunction, data);
    }

    public void GetInstalledAppsForAllOems(string callBackFunction)
    {
      try
      {
        JArray jarray1 = new JArray();
        foreach (string installedCoexistingOem in InstalledOem.InstalledCoexistingOemList)
        {
          JArray jarray2 = new JArray();
          foreach (string vm in RegistryManager.RegistryManagers[installedCoexistingOem].VmList)
          {
            string path = Path.Combine(RegistryManager.RegistryManagers[installedCoexistingOem].DataDir, "UserData\\Gadget\\apps_" + vm + ".json");
            string json = "[]";
            using (Mutex mutex = new Mutex(false, "BlueStacks_AppJsonUpdate"))
            {
              if (mutex.WaitOne())
              {
                try
                {
                  StreamReader streamReader = new StreamReader(path);
                  json = streamReader.ReadToEnd();
                  streamReader.Close();
                }
                catch (Exception ex)
                {
                  Logger.Error("Failed to read apps json file... Err : " + ex.ToString());
                }
                finally
                {
                  mutex.ReleaseMutex();
                }
              }
            }
            string suffix = InstalledOem.GetAppPlayerModel(installedCoexistingOem, Utils.GetValueInBootParams("abivalue", vm, string.Empty, installedCoexistingOem))?.Suffix;
            if (string.IsNullOrEmpty(RegistryManager.RegistryManagers[installedCoexistingOem].Guest[vm].DisplayName))
            {
              string str;
              if (string.Equals(vm, "Android", StringComparison.InvariantCultureIgnoreCase))
                str = BlueStacks.Common.Strings.ProductDisplayName + " " + suffix;
              else
                str = BlueStacks.Common.Strings.ProductDisplayName + " " + Utils.GetVmIdFromVmName(vm) + " " + suffix;
              RegistryManager.RegistryManagers[installedCoexistingOem].Guest[vm].DisplayName = str.Trim();
            }
            jarray2.Add((JToken) new JObject()
            {
              {
                "vmname",
                (JToken) vm
              },
              {
                "vmdisplayname",
                (JToken) RegistryManager.RegistryManagers[installedCoexistingOem].Guest[vm].DisplayName
              },
              {
                "abiValue",
                (JToken) Utils.GetValueInBootParams("abivalue", vm, string.Empty, installedCoexistingOem)
              },
              {
                "oemSuffix",
                (JToken) (string.IsNullOrEmpty(suffix) ? "N-32" : suffix)
              },
              {
                "filedata",
                (JToken) JArray.Parse(json)
              }
            });
          }
          jarray1.Add((JToken) new JObject()
          {
            {
              "oem",
              (JToken) installedCoexistingOem
            },
            {
              "vmlist",
              (JToken) jarray2
            }
          });
        }
        if (string.IsNullOrEmpty(callBackFunction))
          return;
        string data = Regex.Replace(new JObject((object) new JProperty("oemlist", (object) jarray1.ToString(Formatting.None))).ToString(Formatting.None).Replace("\n", "").Replace("\r", ""), "\\s+", " ", RegexOptions.Multiline);
        this.CallBackToHtml(callBackFunction, data);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in GetInstalledAppsForAllOems " + ex.ToString());
      }
    }

    public void GetSystemInfo(string callbackFunction)
    {
      int num = 0;
      try
      {
        num = (int) (new ComputerInfo().TotalPhysicalMemory / 1048576UL);
      }
      catch (Exception ex)
      {
        Logger.Error(string.Format("Exception when finding ram {0}", (object) ex));
      }
      bool flag = false;
      string str1 = "";
      try
      {
        if (!string.IsNullOrEmpty(RegistryManager.RegistryManagers["bgp"].AvailableGPUDetails))
        {
          flag = true;
          str1 = RegistryManager.RegistryManagers["bgp"].AvailableGPUDetails;
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in getting gpu details " + ex?.ToString());
      }
      try
      {
        GlMode glModeForVm = Utils.GetGlModeForVm(this.ParentWindow.mVmName);
        EngineState engineState = EngineState.plus;
        if (RegistryManager.Instance.CurrentEngine == "raw")
          engineState = EngineState.raw;
        string str2 = RegistryManager.Instance.Guest[this.ParentWindow.mVmName].GuestWidth.ToString((IFormatProvider) CultureInfo.InvariantCulture) + "x" + RegistryManager.Instance.Guest[this.ParentWindow.mVmName].GuestHeight.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        JObject jobject = new JObject()
        {
          {
            "physicalCpu",
            (JToken) Environment.ProcessorCount
          },
          {
            "physicalRam",
            (JToken) num
          },
          {
            "isGpuAvailable",
            (JToken) flag
          },
          {
            "gpuText",
            (JToken) str1
          },
          {
            "engineMode",
            (JToken) engineState.ToString()
          },
          {
            "glMode",
            (JToken) glModeForVm.ToString()
          },
          {
            "ram",
            (JToken) RegistryManager.Instance.Guest[this.ParentWindow.mVmName].Memory
          },
          {
            "cpuAllocated",
            (JToken) RegistryManager.Instance.Guest[this.ParentWindow.mVmName].VCPUs
          },
          {
            "dpi",
            (JToken) Utils.GetDpiFromBootParameters(RegistryManager.Instance.Guest[this.ParentWindow.mVmName].BootParameters)
          },
          {
            "fps",
            (JToken) RegistryManager.Instance.Guest[this.ParentWindow.mVmName].FPS
          },
          {
            "res",
            (JToken) str2
          },
          {
            "installedOems",
            (JToken) string.Join(",", InstalledOem.AllInstalledOemList.ToArray())
          },
          {
            "pcode",
            (JToken) Utils.GetValueInBootParams("pcode", this.ParentWindow.mVmName, "", "bgp")
          },
          {
            "astcOption",
            (JToken) RegistryManager.Instance.Guest[this.ParentWindow.mVmName].ASTCOption.ToString()
          },
          {
            "abiValue",
            (JToken) Utils.GetValueInBootParams("abivalue", this.ParentWindow.mVmName, "", "bgp")
          }
        };
        if (string.IsNullOrEmpty(callbackFunction))
          return;
        this.CallBackToHtml(callbackFunction, jobject.ToString(Formatting.None));
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in getting system info details " + ex?.ToString());
      }
    }

    public void GetInstalledAppsJsonforJS(string callBackFunction)
    {
      bool flag = false;
      string str1 = "Android";
      if (!string.IsNullOrEmpty(this.ParentWindow?.mVmName))
        str1 = this.ParentWindow.mVmName;
      string path = Path.Combine(RegistryStrings.GadgetDir, "apps_" + str1 + ".json");
      string str2 = "[" + Environment.NewLine + "]";
      using (Mutex mutex = new Mutex(false, "BlueStacks_AppJsonUpdate"))
      {
        if (mutex.WaitOne())
        {
          try
          {
            StreamReader streamReader = new StreamReader(path);
            str2 = streamReader.ReadToEnd();
            streamReader.Close();
          }
          catch (Exception ex)
          {
            Logger.Error("Failed to read apps json file... Err : " + ex.ToString());
          }
          finally
          {
            mutex.ReleaseMutex();
          }
        }
      }
      if (flag)
        str2 = str2.Replace("\"", "\\\"");
      string data = Regex.Replace(str2.Replace("\n", "").Replace("\r", ""), "\\s+", " ", RegexOptions.Multiline);
      if (string.IsNullOrEmpty(callBackFunction))
        return;
      this.CallBackToHtml(callBackFunction, data);
    }

    public void PerformOTS(string callbackFunction)
    {
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        AppIconModel appIcon = this.ParentWindow.mWelcomeTab.mHomeAppManager.GetAppIcon("com.android.vending");
        if (appIcon == null)
          return;
        this.ParentWindow.mTopBar.mAppTabButtons.AddAppTab(appIcon.AppName, appIcon.PackageName, appIcon.ActivityName, appIcon.ImageName, true, true, false);
      }));
      if (string.IsNullOrEmpty(callbackFunction))
        return;
      this.ParentWindow.mBrowserCallbackFunctionName = callbackFunction;
      this.ParentWindow.BrowserOTSCompletedCallback += new MainWindow.BrowserOTSCompletedCallbackEventHandler(this.ParentWindow_BrowserOTSCompletedCallback);
    }

    private void ParentWindow_BrowserOTSCompletedCallback(
      object sender,
      MainWindowEventArgs.BrowserOTSCompletedCallbackEventArgs args)
    {
      string data = RegistryManager.Instance.Token + "@@" + RegistryManager.Instance.RegisteredEmail;
      this.CallBackToHtml(args.CallbackFunction, data);
      string communityWebTabKey = LocaleStrings.GetLocalizedString("STRING_MACRO_COMMUNITY", "");
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        if (!this.ParentWindow.mTopBar.mAppTabButtons.mDictTabs.ContainsKey(communityWebTabKey) || !(this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.TabKey != communityWebTabKey))
          return;
        this.ParentWindow.Dispatcher.Invoke((Delegate) (() => this.ParentWindow.mTopBar.mAppTabButtons.GoToTab(communityWebTabKey, true, false)));
      }));
    }

    public string GetCurrentAppInfo(string callBackFunction)
    {
      MainWindow mainWindow = (MainWindow) null;
      if (BlueStacksUIUtils.DictWindows.Count > 0)
        mainWindow = BlueStacksUIUtils.DictWindows.Values.First<MainWindow>();
      AppTabButton selectedTab = mainWindow.mTopBar.mAppTabButtons.SelectedTab;
      if (selectedTab == null)
        return "{}";
      JObject jobject = new JObject()
      {
        {
          "type",
          (JToken) "app"
        },
        {
          "name",
          (JToken) selectedTab.AppName
        },
        {
          "data",
          (JToken) selectedTab.PackageName
        }
      };
      if (!string.IsNullOrEmpty(callBackFunction))
        this.CallBackToHtml(callBackFunction, jobject.ToString(Formatting.None));
      return jobject.ToString(Formatting.None);
    }

    public static void DownloadBTV()
    {
      if (BlueStacksUIUtils.DictWindows.Count <= 0)
        return;
      MainWindow window = BlueStacksUIUtils.DictWindows[BlueStacksUIUtils.DictWindows.Keys.ToList<string>()[0]];
      window.Dispatcher.Invoke((Delegate) (() => BTVManager.Instance.MaybeDownloadAndLaunchBTv(window)));
    }

    public static void DownloadDirectX()
    {
      if (BlueStacksUIUtils.DictWindows.Count <= 0)
        return;
      MainWindow activatedWindow = BlueStacksUIUtils.DictWindows[BlueStacksUIUtils.DictWindows.Keys.ToList<string>()[0]];
      activatedWindow.Dispatcher.Invoke((Delegate) (() =>
      {
        string directXDownloadURL = "http://www.microsoft.com/en-us/download/details.aspx?id=35";
        CustomMessageWindow window = new CustomMessageWindow();
        window.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_ADDITIONAL_FILES_REQUIRED", "");
        window.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_SOME_WINDOW_FILES_MISSING", "");
        window.AddHyperLinkInUI(directXDownloadURL, new Uri(directXDownloadURL), (RequestNavigateEventHandler) ((o, arg) =>
        {
          BlueStacksUIUtils.OpenUrl(arg.Uri.ToString());
          window.CloseWindow();
        }));
        window.AddButton(ButtonColors.Blue, "STRING_DOWNLOAD_NOW", (EventHandler) ((o, args) => BlueStacksUIUtils.OpenUrl(directXDownloadURL)), (string) null, false, (object) null, true);
        window.AddButton(ButtonColors.White, "STRING_NO", (EventHandler) null, (string) null, false, (object) null, true);
        window.Owner = (Window) activatedWindow;
        window.ShowDialog();
        activatedWindow.BringIntoView();
      }));
    }

    public static void SetSystemVolume(string level)
    {
      StreamManager.Instance.SetSystemVolume(level);
    }

    public static void SetMicVolume(string level)
    {
      if (string.Equals(level?.Trim(), "0", StringComparison.InvariantCultureIgnoreCase))
        StreamManager.mIsMicDisabled = true;
      StreamManager.Instance.SetMicVolume(level);
    }

    public static void EnableWebcam(string width, string height, string position)
    {
      StreamManager.EnableWebcam(width, height, position);
    }

    public static void DisableWebcamV2(string jsonString)
    {
      StreamManager.Instance.DisableWebcamV2(jsonString);
    }

    public static void MoveWebcam(string horizontal, string vertical)
    {
      StreamManager.Instance.MoveWebcam(horizontal, vertical);
    }

    public static void StopRecord()
    {
      if (StreamManager.Instance == null)
        return;
      Logger.Info("Got StopRecord");
      StreamManager.Instance.StopRecord(true);
    }

    public static void StopStream()
    {
      StreamManager.Instance.StopStream();
    }

    public static void ShowPreview()
    {
    }

    public static void HidePreview()
    {
    }

    public void StartObs(string _)
    {
      this.InitStreamManager();
      StreamManager.Instance.StartObs();
    }

    public void StartStreamV2(
      string jsonString,
      string callbackStreamStatus,
      string callbackTabChanged)
    {
      Logger.Info("Got StartStreamV2");
      this.InitStreamManager();
      if (StreamManager.Instance.mReplayBufferEnabled)
        StreamManager.Instance.StartReplayBuffer();
      Logger.Info("Got StartStream");
      StreamManager.Instance.StartStream(jsonString, callbackStreamStatus, callbackTabChanged);
    }

    private StreamManager InitStreamManager()
    {
      if (StreamManager.Instance == null)
      {
        StreamManager.Instance = new StreamManager(this.CefBrowser);
      }
      else
      {
        string handle;
        StreamManager.GetStreamConfig(out handle, out string _);
        StreamManager.Instance.SetHwnd(handle);
      }
      return StreamManager.Instance;
    }

    public static void LaunchDialog(string jsonString)
    {
      try
      {
        JObject jobject = JObject.Parse(jsonString);
        if (jobject["parameter"] != null)
          jobject["parameter"].ToString();
        if (BlueStacksUIUtils.DictWindows.Count <= 0)
          return;
        BlueStacksUIUtils.DictWindows.Values.First<MainWindow>();
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in launchDialog gmApi : " + ex.ToString());
      }
    }

    public static void CloseFilterWindow(string _)
    {
    }

    public void CallBackToHtml(string callBackFunction, string data)
    {
      if (data == null)
        return;
      data = data?.Replace("\\", "\\\\");
      this.CefBrowser?.ExecuteJavaScript(callBackFunction + "('" + data?.Replace("'", "&#39;").Replace("%27", "&#39;") + "')", this.CefBrowser?.getURL(), 0);
    }

    public void makeWebCall(string url, string scriptToInvoke)
    {
      HttpWebRequest httpWebRequest = WebRequest.Create(url) as HttpWebRequest;
      httpWebRequest.Method = "GET";
      httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip;
      httpWebRequest.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip");
      string str1 = "Bluestacks/" + RegistryManager.Instance.ClientVersion;
      httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/49.0.2623.110 Safari/537.36 " + str1;
      Uri uri = new Uri(url);
      try
      {
        Logger.Info("making a webcall at url=" + url);
        string str2 = (string) null;
        using (HttpWebResponse response = httpWebRequest.GetResponse() as HttpWebResponse)
        {
          using (Stream responseStream = response.GetResponseStream())
          {
            using (StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8))
              str2 = streamReader.ReadToEnd();
          }
        }
        object[] args = new object[1]{ (object) str2 };
        this.CefBrowser.CallJs(scriptToInvoke, args);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in MakeWebCall. err : " + ex.ToString());
        object[] args = new object[1]{ (object) "" };
        this.CefBrowser.CallJs(scriptToInvoke, args);
      }
    }

    protected virtual void Dispose(bool disposing)
    {
      if (this.disposedValue)
        return;
      int num = disposing ? 1 : 0;
      if (this.CefBrowser != null)
      {
        this.CefBrowser.LoadEnd -= new LoadEndEventHandler(this.MBrowser_LoadEnd);
        this.CefBrowser.ProcessMessageRecieved -= new ProcessMessageEventHandler(this.Browser_ProcessMessageRecieved);
        this.CefBrowser.Loaded -= new RoutedEventHandler(this.Browser_Loaded);
        this.CefBrowser.LoadError -= new LoadErrorEventHandler(this.Browser_LoadError);
        this.CefBrowser.LoadingStateChange -= new LoadingStateChangeEventHandler(this.Browser_LoadingStateChange);
        this.CefBrowser.mWPFCefBrowserExceptionHandler -= new ExceptionHandler(this.Browser_WPFCefBrowserExceptionHandler);
        this.CefBrowser.Dispose();
        this.mBrowserHost?.Dispose();
        foreach (BrowserControlTags key in this.TagsSubscribedDict.Keys)
          this.mSubscriber?.UnsubscribeTag(key);
      }
      this.disposedValue = true;
    }

    ~BrowserControl()
    {
      this.Dispose(false);
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }
  }
}
