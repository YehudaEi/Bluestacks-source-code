// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.CommonHandlers
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.BlueStacksUI.BTv;
using BlueStacks.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Media;
using System.Threading;
using System.Timers;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace BlueStacks.BlueStacksUI
{
  internal class CommonHandlers : IDisposable
  {
    internal static bool sIsRecordingVideo = false;
    internal static string sRecordingInstance = "";
    private static bool sIsOBSStartingStopping = false;
    internal static string mSavedVideoRecordingFilePath = (string) null;
    private MainWindow ParentWindow;
    private static bool sDownloading;
    private LegacyDownloader mDownloader;
    private static CustomMessageWindow sWindow;
    internal ShortcutConfig mShortcutsConfigInstance;
    private System.Timers.Timer mScreenshotTimer;
    private System.Timers.Timer mObsResponseTimeoutTimer;
    private long mDownloadedSize;
    private long mLastSizeChecked;
    private DispatcherTimer mDownloadStatusTimer;
    private float mRecorderSizeMb;
    private bool disposedValue;

    public event CommonHandlers.MacroBookmarkChanged MacroBookmarkChangedEvent;

    public event CommonHandlers.MacroSettingsChanged MacroSettingChangedEvent;

    public event CommonHandlers.ShortcutKeysChanged ShortcutKeysChangedEvent;

    public event CommonHandlers.ShortcutKeysRefresh ShortcutKeysRefreshEvent;

    public event CommonHandlers.MacroDeleted MacroDeletedEvent;

    public event CommonHandlers.OverlayStateChanged OverlayStateChangedEvent;

    public event CommonHandlers.MacroButtonVisibilityChanged MacroButtonVisibilityChangedEvent;

    public event CommonHandlers.OperationSyncButtonVisibilityChanged OperationSyncButtonVisibilityChangedEvent;

    public event CommonHandlers.OBSResponseTimeout OBSResponseTimeoutEvent;

    public event CommonHandlers.ScreenRecorderStateTransitioning ScreenRecorderStateTransitioningEvent;

    public event CommonHandlers.BTvDownloaderMinimized BTvDownloaderMinimizedEvent;

    public event CommonHandlers.GamepadButtonVisibilityChanged GamepadButtonVisibilityChangedEvent;

    public event CommonHandlers.ScreenRecordingStateChanged ScreenRecordingStateChangedEvent;

    public event CommonHandlers.VolumeChanged VolumeChangedEvent;

    public event CommonHandlers.VolumeMuted VolumeMutedEvent;

    public event CommonHandlers.GameGuideButtonVisibilityChanged GameGuideButtonVisibilityChangedEvent;

    public event CommonHandlers.UtcConverterLoaded UtcConverterLoadedEvent;

    public event CommonHandlers.UtcConverterVisibilityChanged UtcConverterVisibilityChangedEvent;

    internal void OnVolumeMuted(bool muted)
    {
      CommonHandlers.VolumeMuted volumeMutedEvent = this.VolumeMutedEvent;
      if (volumeMutedEvent == null)
        return;
      volumeMutedEvent(muted);
    }

    internal void OnVolumeChanged(int volumeLevel)
    {
      CommonHandlers.VolumeChanged volumeChangedEvent = this.VolumeChangedEvent;
      if (volumeChangedEvent == null)
        return;
      volumeChangedEvent(volumeLevel);
    }

    internal void OnScreenRecordingStateChanged(bool isRecording)
    {
      CommonHandlers.ScreenRecordingStateChanged stateChangedEvent = this.ScreenRecordingStateChangedEvent;
      if (stateChangedEvent == null)
        return;
      stateChangedEvent(isRecording);
    }

    internal void OnGamepadButtonVisibilityChanged(bool visiblity)
    {
      CommonHandlers.GamepadButtonVisibilityChanged visibilityChangedEvent = this.GamepadButtonVisibilityChangedEvent;
      if (visibilityChangedEvent == null)
        return;
      visibilityChangedEvent(visiblity);
    }

    internal void OnGameGuideButtonVisibilityChanged(bool visiblity)
    {
      CommonHandlers.GameGuideButtonVisibilityChanged visibilityChangedEvent = this.GameGuideButtonVisibilityChangedEvent;
      if (visibilityChangedEvent == null)
        return;
      visibilityChangedEvent(visiblity);
    }

    internal void OnUtcConverterVisibilityChanged(bool visiblity)
    {
      CommonHandlers.UtcConverterVisibilityChanged visibilityChangedEvent = this.UtcConverterVisibilityChangedEvent;
      if (visibilityChangedEvent == null)
        return;
      visibilityChangedEvent(visiblity);
    }

    private void OnOBSResponseTimeout()
    {
      CommonHandlers.OBSResponseTimeout responseTimeoutEvent = this.OBSResponseTimeoutEvent;
      if (responseTimeoutEvent == null)
        return;
      responseTimeoutEvent();
    }

    private void OnBTvDownloaderMinimized()
    {
      CommonHandlers.BTvDownloaderMinimized downloaderMinimizedEvent = this.BTvDownloaderMinimizedEvent;
      if (downloaderMinimizedEvent == null)
        return;
      downloaderMinimizedEvent();
    }

    internal void OnScreenRecorderStateTransitioning()
    {
      CommonHandlers.ScreenRecorderStateTransitioning transitioningEvent = this.ScreenRecorderStateTransitioningEvent;
      if (transitioningEvent == null)
        return;
      transitioningEvent();
    }

    internal void OnUtcConverterLoaded()
    {
      CommonHandlers.UtcConverterLoaded converterLoadedEvent = this.UtcConverterLoadedEvent;
      if (converterLoadedEvent == null)
        return;
      converterLoadedEvent();
    }

    internal void OnMacroButtonVisibilityChanged(bool isVisible)
    {
      CommonHandlers.MacroButtonVisibilityChanged visibilityChangedEvent = this.MacroButtonVisibilityChangedEvent;
      if (visibilityChangedEvent == null)
        return;
      visibilityChangedEvent(isVisible);
    }

    internal void OnOperationSyncButtonVisibilityChanged(bool isVisible)
    {
      CommonHandlers.OperationSyncButtonVisibilityChanged visibilityChangedEvent = this.OperationSyncButtonVisibilityChangedEvent;
      if (visibilityChangedEvent == null)
        return;
      visibilityChangedEvent(isVisible);
    }

    internal void OnMacroBookmarkChanged(string fileName, bool wasBookmarked)
    {
      foreach (KeyValuePair<string, MainWindow> dictWindow in BlueStacksUIUtils.DictWindows)
      {
        CommonHandlers mCommonHandler = dictWindow.Value.mCommonHandler;
        if (mCommonHandler != null)
        {
          CommonHandlers.MacroBookmarkChanged bookmarkChangedEvent = mCommonHandler.MacroBookmarkChangedEvent;
          if (bookmarkChangedEvent != null)
            bookmarkChangedEvent(fileName, wasBookmarked);
        }
      }
    }

    internal static void OnMacroSettingChanged(MacroRecording record)
    {
      foreach (KeyValuePair<string, MainWindow> dictWindow in BlueStacksUIUtils.DictWindows)
      {
        CommonHandlers mCommonHandler = dictWindow.Value.mCommonHandler;
        if (mCommonHandler != null)
        {
          CommonHandlers.MacroSettingsChanged settingChangedEvent = mCommonHandler.MacroSettingChangedEvent;
          if (settingChangedEvent != null)
            settingChangedEvent(record);
        }
      }
    }

    internal static void OnMacroDeleted(string fileName)
    {
      foreach (KeyValuePair<string, MainWindow> dictWindow in BlueStacksUIUtils.DictWindows)
      {
        CommonHandlers mCommonHandler = dictWindow.Value.mCommonHandler;
        if (mCommonHandler != null)
        {
          CommonHandlers.MacroDeleted macroDeletedEvent = mCommonHandler.MacroDeletedEvent;
          if (macroDeletedEvent != null)
            macroDeletedEvent(fileName);
        }
      }
    }

    internal void OnShortcutKeysChanged(bool isEnabled)
    {
      CommonHandlers.ShortcutKeysChanged keysChangedEvent = this.ShortcutKeysChangedEvent;
      if (keysChangedEvent == null)
        return;
      keysChangedEvent(isEnabled);
    }

    internal void OnShortcutKeysRefresh()
    {
      CommonHandlers.ShortcutKeysRefresh keysRefreshEvent = this.ShortcutKeysRefreshEvent;
      if (keysRefreshEvent == null)
        return;
      keysRefreshEvent();
    }

    internal void OnOverlayStateChanged(bool isEnabled)
    {
      CommonHandlers.OverlayStateChanged stateChangedEvent = this.OverlayStateChangedEvent;
      if (stateChangedEvent == null)
        return;
      stateChangedEvent(isEnabled);
    }

    internal CommonHandlers(MainWindow window)
    {
      this.ParentWindow = window;
    }

    public void LocationButtonHandler()
    {
      this.ParentWindow.mTopBar.mAppTabButtons.AddAppTab("STRING_MAP", "com.location.provider", "com.location.provider.MapsActivity", "ico_fakegps", true, true, false);
    }

    public void ImageTranslationHandler()
    {
      Logger.Info("Saving screenshot automatically for image translater");
      if (ImageTranslateControl.Instance != null)
        return;
      using (Bitmap bitmap = this.CaptureSreenShot())
      {
        ImageTranslateControl translateControl = new ImageTranslateControl(this.ParentWindow);
        translateControl.GetTranslateImage(bitmap);
        this.ParentWindow.ShowDimOverlay((IDimOverlayControl) translateControl);
      }
    }

    internal static void ToggleFarmMode(bool newStatus)
    {
      ThreadPool.QueueUserWorkItem((WaitCallback) (obj =>
      {
        foreach (KeyValuePair<string, MainWindow> dictWindow in BlueStacksUIUtils.DictWindows)
        {
          try
          {
            MainWindow window = dictWindow.Value;
            window.Dispatcher.Invoke((Delegate) (() => window.mSidebar.ToggleFarmModeForMIM(newStatus)));
          }
          catch (Exception ex)
          {
            Logger.Error("Error in ToggleFarmMode: " + ex.Message);
          }
        }
      }));
    }

    internal void SearchAppCenter(string searchString)
    {
      AppTabButton tab = this.ParentWindow.mTopBar.mAppTabButtons.GetTab("appcenter");
      if (tab?.GetBrowserControl()?.CefBrowser != null)
      {
        tab.GetBrowserControl().CefBrowser.ExecuteJavaScript(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "openSearch(\"{0}\")", (object) HttpUtility.UrlEncode(searchString)), tab.GetBrowserControl().CefBrowser.StartUrl, 0);
        this.ParentWindow.mTopBar.mAppTabButtons.GoToTab("appcenter", true, false);
      }
      else
        this.ParentWindow.Utils.HandleApplicationBrowserClick(BlueStacksUIUtils.GetAppCenterUrl((string) null) + "&query=" + HttpUtility.UrlEncode(searchString), LocaleStrings.GetLocalizedString("STRING_APP_CENTER", ""), "appcenter", false, "");
    }

    internal void HideMacroRecorderWindow()
    {
      this.ParentWindow.MacroRecorderWindow.Owner = (Window) null;
      this.ParentWindow.MacroRecorderWindow.Hide();
      this.ParentWindow.MacroRecorderWindow.Topmost = false;
      this.ParentWindow.MacroRecorderWindow.ShowWithParentWindow = false;
    }

    internal void RefreshMacroRecorderWindow()
    {
      this.ParentWindow.MacroRecorderWindow.mScriptsStackPanel.Children.Clear();
      this.ParentWindow.MacroRecorderWindow.Init();
    }

    internal static void RefreshAllMacroRecorderWindow()
    {
      try
      {
        foreach (KeyValuePair<string, MainWindow> dictWindow in BlueStacksUIUtils.DictWindows)
        {
          if (dictWindow.Value.MacroRecorderWindow != null)
          {
            dictWindow.Value.MacroRecorderWindow.mScriptsStackPanel.Children.Clear();
            dictWindow.Value.MacroRecorderWindow.Init();
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Debug("Error in refreshing operation recorder window" + ex?.ToString());
      }
    }

    internal static void RefreshAllMacroWindowWithScroll()
    {
      try
      {
        foreach (KeyValuePair<string, MainWindow> dictWindow in BlueStacksUIUtils.DictWindows)
        {
          if (dictWindow.Value.MacroRecorderWindow != null)
          {
            dictWindow.Value.MacroRecorderWindow.mScriptsStackPanel.Children.Clear();
            dictWindow.Value.MacroRecorderWindow.Init();
            dictWindow.Value.MacroRecorderWindow.mScriptsListScrollbar.ScrollToEnd();
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Debug("Error in refreshing operation recorder window" + ex?.ToString());
      }
    }

    internal void ShowMacroRecorderWindow()
    {
      this.ParentWindow.MacroRecorderWindow.Owner = (Window) this.ParentWindow;
      this.ParentWindow.MacroRecorderWindow.Topmost = this.ParentWindow.Topmost;
      this.ParentWindow.MacroRecorderWindow.ShowWithParentWindow = true;
      this.ParentWindow.MacroRecorderWindow.Show();
      this.ParentWindow.Activate();
    }

    private Bitmap CaptureSreenShot()
    {
      System.Windows.Point screen1 = this.ParentWindow.mContentGrid.PointToScreen(new System.Windows.Point(0.0, 0.0));
      System.Windows.Point point = new System.Windows.Point((double) Convert.ToInt32(screen1.X), (double) Convert.ToInt32(screen1.Y));
      System.Windows.Point screen2 = this.ParentWindow.mContentGrid.PointToScreen(new System.Windows.Point((double) (int) this.ParentWindow.mContentGrid.ActualWidth, (double) ((int) this.ParentWindow.mContentGrid.ActualHeight - 40)));
      System.Drawing.Size blockRegionSize = new System.Drawing.Size(Convert.ToInt32(screen2.X - point.X), Convert.ToInt32(screen2.Y - point.Y));
      Bitmap bitmap = new Bitmap(blockRegionSize.Width, blockRegionSize.Height);
      System.Drawing.Point upperLeftSource = new System.Drawing.Point((int) point.X, (int) point.Y);
      using (Graphics graphics = Graphics.FromImage((System.Drawing.Image) bitmap))
        graphics.CopyFromScreen(upperLeftSource, System.Drawing.Point.Empty, blockRegionSize);
      return bitmap;
    }

    public void InstallApkHandler()
    {
      this.ParentWindow.Dispatcher.BeginInvoke((Delegate) (() => new DownloadInstallApk(this.ParentWindow).ChooseAndInstallApk()));
    }

    public void ScreenShotButtonHandler()
    {
      try
      {
        this.ParentWindow.Dispatcher.Invoke((Delegate) (() =>
        {
          this.CheckAndShowScreenshotFolderUpdatePopup();
          SidebarElement elementFromTag = this.ParentWindow.mSidebar.GetElementFromTag("sidebar_screenshot");
          Sidebar.UpdateImage(elementFromTag, "sidebar_video_loading");
          elementFromTag.Image.Visibility = Visibility.Hidden;
          elementFromTag.Image.IsImageToBeRotated = true;
          elementFromTag.Image.Visibility = Visibility.Visible;
          this.ParentWindow.mSidebar.mScreenshotPopup.IsOpen = false;
          if (this.mScreenshotTimer == null)
          {
            this.mScreenshotTimer = new System.Timers.Timer(15000.0);
            this.mScreenshotTimer.Elapsed += new ElapsedEventHandler(this.ScreenshotTimer_Elapsed);
          }
          else
            this.mScreenshotTimer.Stop();
          this.mScreenshotTimer.Start();
        }));
        string screenshotTime = DateTime.Now.ToString("yyyy.MM.dd_HH.mm.ss", (IFormatProvider) CultureInfo.InvariantCulture);
        ThreadPool.QueueUserWorkItem((WaitCallback) (obj =>
        {
          if (this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.mTabType == TabType.AppTab)
          {
            string packageName = this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName;
            this.ParentWindow.mFrontendHandler.GetScreenShot((packageName.Length > 30 ? packageName.Substring(0, 30) : packageName) + "_Screenshot_" + screenshotTime + ".jpeg");
          }
          else
            this.ParentWindow.Dispatcher.BeginInvoke((Delegate) (() =>
            {
              string appLabel = this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.AppLabel;
              string str = Path.Combine(Path.GetTempPath(), (appLabel.Length > 30 ? appLabel.Substring(0, 30) : appLabel) + "_Screenshot_" + screenshotTime + ".jpeg");
              CommonHandlers.SnapShotPNG((UIElement) this.ParentWindow.mContentGrid, str, 1);
              this.PostScreenShotWork(str, true);
            }));
        }));
        try
        {
          if (!Oem.IsOEMDmm)
            return;
          using (SoundPlayer soundPlayer = new SoundPlayer(Path.Combine(Path.Combine(RegistryManager.Instance.ClientInstallDir, "Assets"), "camera_shutter_click.wav")))
            soundPlayer.Play();
        }
        catch
        {
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Error in screenshot button handler: {0}", (object) ex);
      }
    }

    private void ScreenshotTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
      this.ParentWindow.Dispatcher.BeginInvoke((Delegate) (() =>
      {
        this.mScreenshotTimer.Stop();
        SidebarElement elementFromTag = this.ParentWindow.mSidebar.GetElementFromTag("sidebar_screenshot");
        elementFromTag.Image.IsImageToBeRotated = false;
        Sidebar.UpdateImage(elementFromTag, "sidebar_screenshot");
        this.ParentWindow.mSidebar.SetSidebarElementTooltip(elementFromTag, "STRING_TOOLBAR_CAMERA");
      }));
    }

    public static void SnapShotPNG(UIElement source, string filePath, int zoom)
    {
      try
      {
        double height = source.RenderSize.Height;
        double width = source.RenderSize.Width;
        double num = height * (double) zoom;
        RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int) (width * (double) zoom), (int) num, 96.0, 96.0, PixelFormats.Pbgra32);
        VisualBrush visualBrush = new VisualBrush((Visual) source);
        DrawingVisual drawingVisual = new DrawingVisual();
        DrawingContext drawingContext = drawingVisual.RenderOpen();
        using (drawingContext)
        {
          drawingContext.PushTransform((Transform) new ScaleTransform((double) zoom, (double) zoom));
          drawingContext.DrawRectangle((System.Windows.Media.Brush) visualBrush, (System.Windows.Media.Pen) null, new Rect(new System.Windows.Point(0.0, 0.0), new System.Windows.Point(width, height)));
        }
        renderTargetBitmap.Render((Visual) drawingVisual);
        PngBitmapEncoder pngBitmapEncoder = new PngBitmapEncoder();
        pngBitmapEncoder.Frames.Add(BitmapFrame.Create((BitmapSource) renderTargetBitmap));
        using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
          pngBitmapEncoder.Save((Stream) fileStream);
      }
      catch (Exception ex)
      {
        Logger.Error("Error in screenshot button handler: {0}", (object) ex);
      }
    }

    internal void PostScreenShotWork(string screenshotFileFullPath, bool showScreenShotSaved)
    {
      new Thread((ThreadStart) (() => this.ParentWindow.Dispatcher.BeginInvoke((Delegate) (() =>
      {
        try
        {
          Logger.Debug("screen shot path..." + screenshotFileFullPath);
          string str = Path.Combine(RegistryManager.Instance.ScreenShotsPath, Path.GetFileName(screenshotFileFullPath));
          Logger.Info("Screen shot filename.." + str);
          if (string.Compare(Path.GetFullPath(screenshotFileFullPath).TrimEnd('\\'), Path.GetFullPath(str).TrimEnd('\\'), StringComparison.InvariantCultureIgnoreCase) == 0)
          {
            str = screenshotFileFullPath;
          }
          else
          {
            if (File.Exists(str))
              File.Delete(str);
            File.Move(screenshotFileFullPath, str);
            Logger.Info("File moved from " + screenshotFileFullPath + " to " + str);
          }
          ClientStats.SendMiscellaneousStatsAsync("MediaFileSaveSuccess", RegistryManager.Instance.UserGuid, "ScreenShot", RegistryManager.Instance.ScreenShotsPath, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
          if (showScreenShotSaved && RegistryManager.Instance.IsShowToastNotification)
            this.ParentWindow.ShowGeneralToast(LocaleStrings.GetLocalizedString("STRING_SCREENSHOT_SAVED", ""));
          if (showScreenShotSaved && this.ParentWindow.EngineInstanceRegistry.IsSidebarVisible)
            this.ParentWindow.mSidebar.ShowScreenshotSavedPopup(str);
          SidebarElement elementFromTag = this.ParentWindow.mSidebar.GetElementFromTag("sidebar_screenshot");
          elementFromTag.Image.IsImageToBeRotated = false;
          Sidebar.UpdateImage(elementFromTag, "sidebar_screenshot");
          if (!this.mScreenshotTimer.Enabled)
            return;
          this.mScreenshotTimer.Stop();
        }
        catch (Exception ex)
        {
          Logger.Error("Error in post screenshot work: {0}", (object) ex);
        }
      })))).Start();
    }

    private void CheckAndShowScreenshotFolderUpdatePopup()
    {
      if (!RegistryManager.Instance.IsScreenshotsLocationPopupEnabled && StringExtensions.IsValidPath(RegistryManager.Instance.ScreenShotsPath))
        return;
      Utils.ValidateScreenShotFolder();
      string screenShotsPath = RegistryManager.Instance.ScreenShotsPath;
      CustomMessageWindow customMessageWindow = new CustomMessageWindow();
      BlueStacksUIBinding.Bind(customMessageWindow.TitleTextBlock, "STRING_OPEN_MEDIA_FOLDER", "");
      customMessageWindow.AddButton(ButtonColors.Blue, "STRING_CHOOSE_CUSTOM", new EventHandler(this.ChooseCustomFolder), (string) null, false, (object) null, true);
      customMessageWindow.AddButton(ButtonColors.White, "STRING_USE_CURRENT", (EventHandler) null, (string) null, false, (object) null, StringExtensions.IsValidPath(RegistryManager.Instance.ScreenShotsPath));
      if (!StringExtensions.IsValidPath(RegistryManager.Instance.ScreenShotsPath))
        customMessageWindow.IsWindowCloseButtonDisabled = true;
      BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlock, "STRING_CHOOSE_FOLDER_TEXT", "");
      customMessageWindow.BodyWarningTextBlock.Visibility = Visibility.Visible;
      customMessageWindow.BodyWarningTextBlock.Text = screenShotsPath;
      BlueStacksUIBinding.BindColor((DependencyObject) customMessageWindow.BodyWarningTextBlock, TextBlock.ForegroundProperty, "HyperLinkForegroundColor");
      this.ParentWindow.ShowDimOverlay((IDimOverlayControl) null);
      customMessageWindow.Owner = (Window) this.ParentWindow.mDimOverlay;
      customMessageWindow.ShowDialog();
      this.ParentWindow.HideDimOverlay();
      this.ParentWindow.Focus();
      RegistryManager.Instance.IsScreenshotsLocationPopupEnabled = false;
      ClientStats.SendMiscellaneousStatsAsync("MediaFilesPathSet", RegistryManager.Instance.UserGuid, "PathChangeFromPopUp", screenShotsPath, RegistryManager.Instance.ScreenShotsPath, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, "Android");
    }

    internal void AddCoordinatesToScriptText(double x, double y)
    {
      if (!KMManager.sIsInScriptEditingMode || KMManager.CanvasWindow == null)
        return;
      KMManager.CanvasWindow.SidebarWindow?.InsertXYInScript(x, y);
    }

    private void ChooseCustomFolder(object sender, EventArgs e)
    {
      this.ShowFolderBrowserDialog(RegistryManager.Instance.ScreenShotsPath);
    }

    internal void ShowFolderBrowserDialog(string currentScreenshotPath)
    {
      using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog()
      {
        ShowNewFolderButton = true
      })
      {
        if (StringExtensions.IsValidPath(currentScreenshotPath))
          folderBrowserDialog.SelectedPath = currentScreenshotPath;
        if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
        {
          string selectedPath = folderBrowserDialog.SelectedPath;
          Logger.Info("dialoge selected path.." + folderBrowserDialog.SelectedPath);
          bool flag = Utils.CheckWritePermissionForFolder(selectedPath);
          Logger.Info("Permission.." + flag.ToString() + "..path.." + selectedPath);
          if (flag && StringExtensions.IsValidPath(currentScreenshotPath))
            RegistryManager.Instance.ScreenShotsPath = selectedPath;
          else
            this.ShowInvalidPathPopUp();
        }
        else if (!StringExtensions.IsValidPath(currentScreenshotPath))
          this.ShowInvalidPathPopUp();
        else
          RegistryManager.Instance.ScreenShotsPath = currentScreenshotPath;
      }
    }

    private void ShowInvalidPathPopUp()
    {
      string screenshotDefaultPath = RegistryStrings.ScreenshotDefaultPath;
      CustomMessageWindow customMessageWindow = new CustomMessageWindow();
      BlueStacksUIBinding.Bind(customMessageWindow.TitleTextBlock, "STRING_OPEN_MEDIA_FOLDER", "");
      customMessageWindow.AddButton(ButtonColors.Blue, "STRING_CHOOSE_ANOTHER", new EventHandler(this.ChooseCustomFolder), (string) null, false, (object) null, true);
      customMessageWindow.AddButton(ButtonColors.White, "STRING_USE_DEFAULT", (EventHandler) null, (string) null, false, (object) null, StringExtensions.IsValidPath(RegistryStrings.ScreenshotDefaultPath));
      if (!StringExtensions.IsValidPath(RegistryManager.Instance.ScreenShotsPath))
        customMessageWindow.IsWindowCloseButtonDisabled = true;
      customMessageWindow.BodyTextBlockTitle.Visibility = Visibility.Visible;
      customMessageWindow.BodyTextBlockTitle.Text = LocaleStrings.GetLocalizedString("STRING_SCREENSHOT_INVALID_PATH", "");
      BlueStacksUIBinding.BindColor((DependencyObject) customMessageWindow.BodyTextBlockTitle, TextBlock.ForegroundProperty, "DeleteComboTextForeground");
      customMessageWindow.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_SCREENSHOT_USE_DEFAULT", "");
      customMessageWindow.BodyWarningTextBlock.Visibility = Visibility.Visible;
      customMessageWindow.BodyWarningTextBlock.Text = screenshotDefaultPath;
      BlueStacksUIBinding.BindColor((DependencyObject) customMessageWindow.BodyWarningTextBlock, TextBlock.ForegroundProperty, "HyperLinkForegroundColor");
      customMessageWindow.Owner = (Window) this.ParentWindow.mDimOverlay;
      customMessageWindow.ShowDialog();
      customMessageWindow.Close();
    }

    public void ShakeButtonHandler()
    {
      this.ParentWindow.Utils.ShakeWindow();
      this.ParentWindow.mFrontendHandler.SendFrontendRequestAsync("shake", (Dictionary<string, string>) null);
    }

    public void BackButtonHandler(bool receivedFromImap = false)
    {
      if (!this.ParentWindow.mGuestBootCompleted)
        return;
      new Thread((ThreadStart) (() => VmCmdHandler.RunCommand("back", this.ParentWindow.mVmName, "bgp")))
      {
        IsBackground = true
      }.Start();
      if (!this.ParentWindow.SendClientActions || receivedFromImap)
        return;
      Dictionary<string, string> data = new Dictionary<string, string>();
      Dictionary<string, string> dictionary = new Dictionary<string, string>()
      {
        {
          "EventAction",
          "BackButton"
        }
      };
      JsonSerializerSettings serializerSettings = Utils.GetSerializerSettings();
      serializerSettings.Formatting = Formatting.None;
      data.Add("operationData", JsonConvert.SerializeObject((object) dictionary, serializerSettings));
      this.ParentWindow.mFrontendHandler.SendFrontendRequestAsync("handleClientOperation", data);
    }

    public void OpenBrowserInPopup(Dictionary<string, string> payload)
    {
      this.ParentWindow.Dispatcher.Invoke((Delegate) (() =>
      {
        try
        {
          string localizedString = LocaleStrings.GetLocalizedString(payload["click_action_title"], "");
          string url = payload["click_action_value"].Trim();
          string urlWithParams = WebHelper.GetUrlWithParams(url, (string) null, (string) null, (string) null);
          ClientStats.SendPopupBrowserStatsInMiscASync("request", url);
          PopupBrowserControl popupBrowserControl = new PopupBrowserControl();
          popupBrowserControl.Init(urlWithParams, localizedString, this.ParentWindow);
          ClientStats.SendPopupBrowserStatsInMiscASync("impression", url);
          this.ParentWindow.ShowDimOverlay((IDimOverlayControl) popupBrowserControl);
        }
        catch (Exception ex)
        {
          Logger.Error("Couldn't open popup. An exception occured. {0}", (object) ex);
        }
      }));
    }

    public void HomeButtonHandler(bool isLaunch = true, bool receivedFromImap = false)
    {
      this.ParentWindow.mTopBar.mAppTabButtons.GoToTab("Home", isLaunch, false);
      if (!this.ParentWindow.SendClientActions || receivedFromImap)
        return;
      Dictionary<string, string> data = new Dictionary<string, string>();
      Dictionary<string, string> dictionary = new Dictionary<string, string>()
      {
        {
          "EventAction",
          "HomeButton"
        },
        {
          "IsLaunch",
          isLaunch.ToString((IFormatProvider) CultureInfo.InvariantCulture)
        }
      };
      JsonSerializerSettings serializerSettings = Utils.GetSerializerSettings();
      serializerSettings.Formatting = Formatting.None;
      data.Add("operationData", JsonConvert.SerializeObject((object) dictionary, serializerSettings));
      this.ParentWindow.mFrontendHandler.SendFrontendRequestAsync("handleClientOperation", data);
    }

    public void FullScreenButtonHandler(string source, string actionPerformed)
    {
      if (!this.ParentWindow.mResizeHandler.IsMinMaxEnabled)
        return;
      this.ParentWindow.Dispatcher.Invoke((Delegate) (() =>
      {
        if (this.ParentWindow.mIsFullScreen)
        {
          this.ParentWindow.RestoreWindows(false);
          this.ParentWindow.mCommonHandler.ToggleScrollOnEdgeMode("false");
          ClientStats.SendMiscellaneousStatsAsync(source, RegistryManager.Instance.UserGuid, "RestoreFullscreen", actionPerformed, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
        }
        else
        {
          this.ParentWindow.FullScreenWindow();
          this.ParentWindow.mCommonHandler.ToggleScrollOnEdgeMode("true");
          ClientStats.SendMiscellaneousStatsAsync(source, RegistryManager.Instance.UserGuid, "Fullscreen", actionPerformed, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
        }
      }));
    }

    internal void AddToastPopup(
      Window window,
      string message,
      double duration = 1.3,
      bool isShowCloseImage = false)
    {
      this.ParentWindow.Dispatcher.Invoke((Delegate) (() =>
      {
        try
        {
          CustomToastPopupControl toastPopupControl = new CustomToastPopupControl(window);
          if (isShowCloseImage)
          {
            toastPopupControl.Init(window, message, (System.Windows.Media.Brush) System.Windows.Media.Brushes.Black, (System.Windows.Media.Brush) null, System.Windows.HorizontalAlignment.Center, VerticalAlignment.Top, new Thickness?(), 12, new Thickness?(), (System.Windows.Media.Brush) null, isShowCloseImage, false);
            toastPopupControl.Margin = new Thickness(0.0, 40.0, 0.0, 0.0);
          }
          else
            toastPopupControl.Init(window, message, (System.Windows.Media.Brush) System.Windows.Media.Brushes.Black, (System.Windows.Media.Brush) null, System.Windows.HorizontalAlignment.Center, VerticalAlignment.Center, new Thickness?(), 12, new Thickness?(), (System.Windows.Media.Brush) null, false, false);
          toastPopupControl.ShowPopup(duration);
        }
        catch (Exception ex)
        {
          Logger.Error("Exception in showing toast popup: " + ex.ToString());
        }
      }));
    }

    internal void HandleClientOperation(string operationString)
    {
      try
      {
        JObject jobject = JObject.Parse(operationString);
        switch ((string) jobject["EventAction"])
        {
          case "RunApp":
            string str = (string) jobject["Package"];
            AppIconModel appIcon = this.ParentWindow.mWelcomeTab.mHomeAppManager.GetAppIcon(str);
            if (appIcon != null && appIcon.AppIncompatType != AppIncompatType.None && !this.ParentWindow.mTopBar.mAppTabButtons.mDictTabs.ContainsKey(str))
            {
              GrmHandler.HandleCompatibility(appIcon.PackageName, this.ParentWindow.mVmName);
              break;
            }
            this.ParentWindow.mAppHandler.SendRunAppRequestAsync(str, (string) jobject["Activity"], true);
            break;
          case "BackButton":
            this.BackButtonHandler(true);
            break;
          case "HomeButton":
            bool isLaunch = jobject["IsLaunch"].ToObject<bool>();
            this.ParentWindow.Dispatcher.Invoke((Delegate) (() => this.HomeButtonHandler(isLaunch, true)));
            break;
          case "TabSelected":
            string tabKey1 = jobject["tabKey"].ToObject<string>();
            if (string.IsNullOrEmpty(tabKey1))
              break;
            this.ParentWindow.Dispatcher.Invoke((Delegate) (() => this.ParentWindow.mTopBar.mAppTabButtons.GoToTab(tabKey1, true, true)));
            break;
          case "TabClosed":
            string tabKey2 = jobject["tabKey"].ToObject<string>();
            bool sendStopAppToAndroid = jobject["sendStopAppToAndroid"].ToObject<bool>();
            bool forceClose = jobject["forceClose"].ToObject<bool>();
            if (string.IsNullOrEmpty(tabKey2))
              break;
            this.ParentWindow.Dispatcher.Invoke((Delegate) (() => this.ParentWindow.mTopBar.mAppTabButtons.CloseTab(tabKey2, sendStopAppToAndroid, forceClose, true, true, "")));
            break;
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in HandleClientOperation. OperationString: " + operationString + " Error:" + ex?.ToString());
      }
    }

    private bool CheckForMacroVisibility()
    {
      return !this.ParentWindow.mAppHandler.IsOneTimeSetupCompleted || CommonHandlers.ShowMacroForSelectedApp(this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.TabKey);
    }

    private static bool ShowMacroForSelectedApp(string appPackage)
    {
      if (PromotionObject.Instance.AppSpecificRulesList != null)
      {
        foreach (string appSpecificRules in PromotionObject.Instance.AppSpecificRulesList)
        {
          string str = appSpecificRules;
          if (appSpecificRules.EndsWith("*", StringComparison.InvariantCulture))
            str = appSpecificRules.Substring(0, appSpecificRules.Length - 2);
          if (str.StartsWith("~", StringComparison.InvariantCulture))
          {
            if (appPackage.StartsWith(str.Substring(1), StringComparison.InvariantCulture))
              return false;
          }
          else if (appPackage.StartsWith(str, StringComparison.InvariantCulture))
            return true;
        }
      }
      return false;
    }

    private static bool IsCustomCursorEnableForApp(string appPackage)
    {
      try
      {
        if (!RegistryManager.Instance.CustomCursorEnabled || !FeatureManager.Instance.IsCustomCursorEnabled)
          return false;
        string empty = string.Empty;
        if (PromotionObject.Instance.CustomCursorExcludedAppsList != null)
        {
          foreach (string cursorExcludedApps in PromotionObject.Instance.CustomCursorExcludedAppsList)
          {
            string str = cursorExcludedApps;
            if (cursorExcludedApps.EndsWith("*", StringComparison.InvariantCulture))
              str = cursorExcludedApps.Substring(0, cursorExcludedApps.Length - 1);
            if (str.StartsWith("~", StringComparison.InvariantCulture))
            {
              if (appPackage.StartsWith(str.Substring(1), StringComparison.InvariantCulture))
                return true;
            }
            else if (appPackage.StartsWith(str, StringComparison.InvariantCulture))
              return false;
          }
        }
        return true;
      }
      catch
      {
        return false;
      }
    }

    internal void SetCustomCursorForApp(string appPackage)
    {
      this.ToggleCursorStyle(CommonHandlers.IsCustomCursorEnableForApp(appPackage));
    }

    internal void ClipMouseCursorHandler(
      bool forceDisable = false,
      bool switchState = true,
      string statAction = "",
      string sourceLocation = "")
    {
      try
      {
        if (Oem.IsOEMDmm)
          return;
        if (forceDisable)
          this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.IsCursorClipped = false;
        else if (switchState)
          this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.IsCursorClipped = !this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.IsCursorClipped;
        if (this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab != null && this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.mTabType == TabType.AppTab && this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.IsCursorClipped)
        {
          BlueStacks.Common.RECT rect = new BlueStacks.Common.RECT();
          if (this.ParentWindow.StaticComponents.mLastMappableWindowHandle == IntPtr.Zero)
            this.ParentWindow.StaticComponents.mLastMappableWindowHandle = this.ParentWindow.mFrontendHandler.mFrontendHandle;
          InteropWindow.GetWindowRect(this.ParentWindow.StaticComponents.mLastMappableWindowHandle, ref rect);
          System.Windows.Forms.Cursor.Clip = new Rectangle(new System.Drawing.Point(rect.Left, rect.Top), new System.Drawing.Size(rect.Right - rect.Left, rect.Bottom - rect.Top));
          this.ParentWindow.OnCursorLockChanged(true);
          this.ParentWindow.mCommonHandler.ToggleScrollOnEdgeMode("true");
          this.ParentWindow.mCommonHandler.ToggleIsMouseLocked("true");
          if (string.IsNullOrEmpty(statAction))
            return;
          ClientStats.SendMiscellaneousStatsAsync(sourceLocation, RegistryManager.Instance.UserGuid, "LockMouseCursor", statAction, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName, (string) null, "Android");
          if (!RegistryManager.Instance.IsShowToastNotification || (!this.ParentWindow.IsUIInPortraitMode || this.ParentWindow.Width <= 365.0) && (this.ParentWindow.IsUIInPortraitMode || this.ParentWindow.Width <= 425.0))
            return;
          this.ParentWindow.mUnlockMouseToastControl.ShowUnlockMouseToast(LocaleStrings.GetLocalizedString("STRING_UNLOCK_MOUSE_POPUP_TEXT", "Tip: Press {0} to unlock Mouse Cursor"));
        }
        else
        {
          System.Windows.Forms.Cursor.Clip = Rectangle.Empty;
          this.ParentWindow.OnCursorLockChanged(false);
          this.ParentWindow.mCommonHandler.ToggleScrollOnEdgeMode("false");
          this.ParentWindow.mCommonHandler.ToggleIsMouseLocked("false");
          this.ParentWindow.mUnlockMouseToastControl.CloseUnlockMouseToastAndStopTimer();
          if (string.IsNullOrEmpty(statAction))
            return;
          ClientStats.SendMiscellaneousStatsAsync(sourceLocation, RegistryManager.Instance.UserGuid, "UnlockMouseCursor", statAction, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName, (string) null, "Android");
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in ClipMouseCursorHandler. Exception: " + ex.ToString());
      }
    }

    internal string GetShortcutKeyFromName(string shortcutName, bool isBossKey = false)
    {
      try
      {
        if (this.mShortcutsConfigInstance != null)
        {
          foreach (ShortcutKeys shortcutKeys in this.mShortcutsConfigInstance.Shortcut)
          {
            if (string.Equals(shortcutKeys.ShortcutName, shortcutName, StringComparison.InvariantCulture))
            {
              if (isBossKey)
                return shortcutKeys.ShortcutKey;
              string[] strArray = shortcutKeys.ShortcutKey.Split(new char[2]
              {
                '+',
                ' '
              }, StringSplitOptions.RemoveEmptyEntries);
              string str = string.Empty;
              foreach (string key in strArray)
                str = str + LocaleStrings.GetLocalizedString(Constants.ImapLocaleStringsConstant + IMAPKeys.GetStringForUI(key), "") + " + ";
              if (!string.IsNullOrEmpty(str))
                return str.Substring(0, str.Length - 3);
            }
          }
        }
        else
        {
          string str;
          switch (shortcutName)
          {
            case "STRING_TOGGLE_LOCK_CURSOR":
              str = "Ctrl + Shift + F8";
              break;
            case "STRING_TOGGLE_KEYMAP_WINDOW":
              str = "Ctrl + Shift + H";
              break;
            case "STRING_TOGGLE_OVERLAY":
              str = "Ctrl + Shift + F6";
              break;
            default:
              str = "";
              break;
          }
          return str;
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in GetShortcutKeyFromName: " + ex.ToString());
      }
      return "";
    }

    internal static void SaveMacroJson(MacroRecording record, string destFileName)
    {
      try
      {
        JsonSerializerSettings serializerSettings = Utils.GetSerializerSettings();
        serializerSettings.Formatting = Formatting.Indented;
        string contents = JsonConvert.SerializeObject((object) record, serializerSettings);
        if (!Directory.Exists(RegistryStrings.MacroRecordingsFolderPath))
          Directory.CreateDirectory(RegistryStrings.MacroRecordingsFolderPath);
        File.WriteAllText(Path.Combine(RegistryStrings.MacroRecordingsFolderPath, Path.GetFileName(destFileName.ToLower(CultureInfo.InvariantCulture).Trim())), contents);
      }
      catch (Exception ex)
      {
        Logger.Error("Could not serialize the macro recording object. Ex: {0}", (object) ex);
      }
    }

    internal void ToggleMacroAndSyncVisibility()
    {
      try
      {
        if (FeatureManager.Instance.ForceEnableMacroAndSync)
        {
          this.OnMacroButtonVisibilityChanged(true);
          this.OnOperationSyncButtonVisibilityChanged(true);
        }
        else if (FeatureManager.Instance.IsMacroRecorderEnabled || FeatureManager.Instance.IsOperationsSyncEnabled)
        {
          bool isVisible = this.CheckForMacroVisibility();
          if (FeatureManager.Instance.IsMacroRecorderEnabled)
            this.OnMacroButtonVisibilityChanged(isVisible);
          if (!FeatureManager.Instance.IsOperationsSyncEnabled)
            return;
          this.OnOperationSyncButtonVisibilityChanged(isVisible);
        }
        else
        {
          this.OnMacroButtonVisibilityChanged(false);
          this.OnOperationSyncButtonVisibilityChanged(false);
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in ToggleMacroAndSyncVisibility: " + ex.ToString());
      }
    }

    private void ToggleCursorStyle(bool enable)
    {
      try
      {
        Dictionary<string, string> data = new Dictionary<string, string>();
        if (enable)
          data.Add("path", RegistryStrings.CursorPath);
        else
          data.Add("path", string.Empty);
        ThreadPool.QueueUserWorkItem((WaitCallback) (obj1 =>
        {
          try
          {
            HTTPUtils.SendRequestToEngine("setCursorStyle", data, this.ParentWindow.mVmName, 3000, (Dictionary<string, string>) null, false, 1, 0, "", "bgp");
            this.SetDefaultCursorForClient();
          }
          catch (Exception ex)
          {
            Logger.Error("Failed to send Show event to engine... err : " + ex.ToString());
            this.SetDefaultCursorForClient();
          }
        }));
      }
      catch (Exception ex)
      {
        this.SetDefaultCursorForClient();
      }
    }

    private void SetDefaultCursorForClient()
    {
      ThreadPool.QueueUserWorkItem((WaitCallback) (obj1 => this.ParentWindow.Dispatcher.Invoke((Delegate) (() =>
      {
        try
        {
          Mouse.OverrideCursor = (System.Windows.Input.Cursor) null;
        }
        catch (Exception ex)
        {
          Logger.Error("Failed to set default cursor for client... err : " + ex.ToString());
        }
      }))));
    }

    public void LaunchSettingsWindow(string tabName = "")
    {
      if (MainWindow.SettingsWindow != null)
        return;
      MainWindow.OpenSettingsWindow(this.ParentWindow, tabName);
    }

    public void DMMSwitchKeyMapButtonHandler()
    {
      this.ParentWindow.Dispatcher.Invoke((Delegate) (() =>
      {
        if (this.ParentWindow.mDmmBottomBar.mKeyMapSwitch.ImageName.EndsWith("_off", StringComparison.InvariantCulture))
        {
          this.ParentWindow.mDmmBottomBar.mKeyMapSwitch.ImageName = this.ParentWindow.mDmmBottomBar.mKeyMapSwitch.ImageName.Replace("_off", string.Empty);
          BlueStacksUIBinding.Bind((System.Windows.Controls.Image) this.ParentWindow.mDmmBottomBar.mKeyMapSwitch, "STRING_KEYMAPPING_ENABLED");
          BlueStacksUIBinding.Bind((System.Windows.Controls.Image) this.ParentWindow.mDMMFST.mKeyMapSwitch, "STRING_KEYMAPPING_ENABLED");
          this.ParentWindow.mFrontendHandler.EnableKeyMapping(true);
          this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.EnableKeymapForDMM(true);
        }
        else
        {
          this.ParentWindow.mDmmBottomBar.mKeyMapSwitch.ImageName += "_off";
          BlueStacksUIBinding.Bind((System.Windows.Controls.Image) this.ParentWindow.mDmmBottomBar.mKeyMapSwitch, "STRING_KEYMAPPING_DISABLED");
          BlueStacksUIBinding.Bind((System.Windows.Controls.Image) this.ParentWindow.mDMMFST.mKeyMapSwitch, "STRING_KEYMAPPING_DISABLED");
          this.ParentWindow.mFrontendHandler.EnableKeyMapping(false);
          this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.EnableKeymapForDMM(false);
        }
        this.ParentWindow.mDMMFST.mKeyMapSwitch.ImageName = this.ParentWindow.mDmmBottomBar.mKeyMapSwitch.ImageName;
      }));
    }

    public void SetDMMKeymapButtonsAndTransparency()
    {
      if (this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.IsDMMKeymapUIVisible)
      {
        this.ParentWindow.mCommonHandler.EnableKeymapButtonsForDmm(Visibility.Visible);
        this.ParentWindow.mDmmBottomBar.ShowKeyMapPopup(true);
        KMManager.ShowOverlayWindow(this.ParentWindow, true, true);
        if (!KMManager.sPackageName.Equals(this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName, StringComparison.InvariantCultureIgnoreCase))
        {
          Logger.Info("diff package.." + KMManager.sPackageName + "..." + this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName);
          KMManager.dictOverlayWindow[this.ParentWindow].ReloadCanvasWindow();
        }
        BlueStacksUIBinding.Bind((System.Windows.Controls.Image) this.ParentWindow.mDmmBottomBar.mKeyMapSwitch, "STRING_KEYMAPPING_ENABLED");
        if (this.ParentWindow.mDmmBottomBar.CurrentTransparency > 0.0)
          this.SetTranslucentControlsBtnImageForDMM("eye");
        else
          this.SetTranslucentControlsBtnImageForDMM("eye_off");
      }
      else
      {
        this.ParentWindow.mCommonHandler.EnableKeymapButtonsForDmm(Visibility.Collapsed);
        this.ParentWindow.mDmmBottomBar.ShowKeyMapPopup(false);
        KMManager.ShowOverlayWindow(this.ParentWindow, false, false);
      }
      if (this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.IsDMMKeymapEnabled)
      {
        this.ParentWindow.mDmmBottomBar.mKeyMapSwitch.ImageName = "keymapswitch";
        this.ParentWindow.mDMMFST.mKeyMapSwitch.ImageName = "keymapswitch";
        this.ParentWindow.mFrontendHandler.EnableKeyMapping(true);
      }
      else
      {
        this.ParentWindow.mDmmBottomBar.mKeyMapSwitch.ImageName = "keymapswitch_off";
        this.ParentWindow.mDMMFST.mKeyMapSwitch.ImageName = "keymapswitch_off";
        this.ParentWindow.mFrontendHandler.EnableKeyMapping(false);
      }
    }

    public void EnableKeymapButtonsForDmm(Visibility isVisible)
    {
      this.ParentWindow.mDmmBottomBar.mKeyMapButton.Visibility = isVisible;
      this.ParentWindow.mDmmBottomBar.mKeyMapSwitch.Visibility = isVisible;
      this.ParentWindow.mDmmBottomBar.mTranslucentControlsButton.Visibility = isVisible;
      this.ParentWindow.mDMMFST.mKeyMapButton.Visibility = isVisible;
      this.ParentWindow.mDMMFST.mKeyMapSwitch.Visibility = isVisible;
      this.ParentWindow.mDMMFST.mTranslucentControlsButton.Visibility = isVisible;
    }

    internal void SetTranslucentControlsBtnImageForDMM(string imageName)
    {
      this.ParentWindow.mDmmBottomBar.mTranslucentControlsButton.ImageName = imageName;
      this.ParentWindow.mDmmBottomBar.mTranslucentControlsSliderButton.ImageName = this.ParentWindow.mDmmBottomBar.mTranslucentControlsButton.ImageName;
      this.ParentWindow.mDMMFST.mTranslucentControlsButton.ImageName = this.ParentWindow.mDmmBottomBar.mTranslucentControlsButton.ImageName;
      this.ParentWindow.mDMMFST.mTranslucentControlsSliderButton.ImageName = this.ParentWindow.mDmmBottomBar.mTranslucentControlsButton.ImageName;
    }

    internal void KeyMapButtonHandler(string action, string location)
    {
      KMManager.ShowAdvancedSettings(this.ParentWindow);
      ClientStats.SendMiscellaneousStatsAsync(location, RegistryManager.Instance.UserGuid, "KeyMap", action, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
    }

    public void DMMScreenshotHandler()
    {
      using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog()
      {
        ShowNewFolderButton = true,
        Description = LocaleStrings.GetLocalizedString("STRING_CHOOSE_SCREENSHOT_FOLDER_TEXT", "")
      })
      {
        if (folderBrowserDialog.ShowDialog(Utils.GetIWin32Window(this.ParentWindow.Handle)) != DialogResult.OK || string.IsNullOrEmpty(folderBrowserDialog.SelectedPath))
          return;
        RegistryManager.Instance.ScreenShotsPath = Directory.Exists(folderBrowserDialog.SelectedPath) ? folderBrowserDialog.SelectedPath : RegistryStrings.ScreenshotDefaultPath;
      }
    }

    public void RecordVideoOfApp()
    {
      this.CheckAndShowScreenshotFolderUpdatePopup();
      Logger.Debug("OBS start or stop status: {0}", (object) CommonHandlers.sIsOBSStartingStopping);
      if (CommonHandlers.sIsOBSStartingStopping)
        return;
      CommonHandlers.sIsOBSStartingStopping = true;
      ClientStats.SendMiscellaneousStatsAsync("VideoRecording", RegistryManager.Instance.UserGuid, "VideoRecordingStarting", RegistryManager.Instance.ScreenShotsPath, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
      ThreadPool.QueueUserWorkItem((WaitCallback) (obj =>
      {
        try
        {
          string path = Path.Combine(RegistryManager.Instance.ScreenShotsPath, string.Format((IFormatProvider) CultureInfo.InvariantCulture, BlueStacks.Common.Strings.ProductTopBarDisplayName + "_Recording_{0}.mp4", (object) DateTime.Now.ToString("yyyy.MM.dd_HH.mm.ss.ff", (IFormatProvider) CultureInfo.InvariantCulture)));
          CommonHandlers.sRecordingInstance = this.ParentWindow.mVmName;
          if (StreamManager.Instance == null)
            StreamManager.Instance = new StreamManager(this.ParentWindow);
          string str = this.ParentWindow.mFrontendHandler.mFrontendHandle.ToString();
          this.ParentWindow.Dispatcher.Invoke((Delegate) (() =>
          {
            this.ParentWindow.RestrictWindowResize(true);
            this.OnScreenRecorderStateTransitioning();
            this.StartLoadingTimeoutTimer();
          }));
          Process currentProcess = Process.GetCurrentProcess();
          StreamManager.Instance.Init(str, currentProcess.Id.ToString((IFormatProvider) CultureInfo.InvariantCulture));
          StreamManager.sStopInitOBSQueue = false;
          try
          {
            StreamManager.Instance.StartObs();
          }
          catch (Exception ex)
          {
            Logger.Error("Exception in StartObs: {0}", (object) ex);
            this.ShowErrorRecordingVideoPopup();
            return;
          }
          StreamManager.Instance.SetMicVolume("0");
          StreamManager.Instance.SetHwnd(str);
          StreamManager.Instance.SetSavePath(path);
          CommonHandlers.mSavedVideoRecordingFilePath = path;
          StreamManager.Instance.EnableVideoRecording(true);
          StreamManager.Instance.StartRecordForVideo();
          CommonHandlers.sIsRecordingVideo = true;
        }
        catch (Exception ex)
        {
          Logger.Error("Error in RecordVideoOfApp: {0}", (object) ex);
        }
      }));
    }

    private void StartLoadingTimeoutTimer()
    {
      if (this.mObsResponseTimeoutTimer == null)
      {
        this.mObsResponseTimeoutTimer = new System.Timers.Timer(20000.0);
        this.mObsResponseTimeoutTimer.Elapsed += new ElapsedEventHandler(this.ObsResponseTimeoutTimer_Elapsed);
        this.mObsResponseTimeoutTimer.AutoReset = false;
      }
      if (this.mObsResponseTimeoutTimer.Enabled)
        return;
      this.mObsResponseTimeoutTimer.Start();
    }

    private void ObsResponseTimeoutTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
      this.OnOBSResponseTimeout();
      CommonHandlers.sIsRecordingVideo = false;
      CommonHandlers.sIsOBSStartingStopping = false;
      CommonHandlers.sRecordingInstance = "";
      this.ParentWindow.RestrictWindowResize(false);
      if (StreamManager.Instance != null)
        StreamManager.Instance.ShutDownForcefully();
      this.ShowErrorRecordingVideoPopup();
    }

    internal void StopRecordVideo()
    {
      try
      {
        this.OnScreenRecorderStateTransitioning();
        this.StartLoadingTimeoutTimer();
        StreamManager.Instance.StopRecord();
      }
      catch (Exception ex)
      {
        Logger.Error("error in stop record video : {0}", (object) ex);
      }
    }

    internal void RecordingStopped()
    {
      this.mObsResponseTimeoutTimer?.Stop();
      this.ParentWindow.RestrictWindowResize(false);
      this.OnScreenRecordingStateChanged(false);
      ClientStats.SendMiscellaneousStatsAsync("VideoRecording", RegistryManager.Instance.UserGuid, "VideoRecordingDone", RegistryManager.Instance.ScreenShotsPath, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
    }

    internal void DownloadAndLaunchRecording(string location, string action)
    {
      Logger.Debug("value of sRecordingInstance: {0} and sIsRecordingVideo: {1}", (object) CommonHandlers.sRecordingInstance, (object) CommonHandlers.sIsRecordingVideo);
      if (CommonHandlers.sIsRecordingVideo)
      {
        if (string.Equals(CommonHandlers.sRecordingInstance, this.ParentWindow.mVmName, StringComparison.InvariantCulture))
        {
          this.StopRecordVideo();
          ClientStats.SendMiscellaneousStatsAsync(location, RegistryManager.Instance.UserGuid, "VideoRecordingStop", action, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
        }
        else
          this.ShowAlreadyRunningPopUpForOBS();
      }
      else
      {
        if (Directory.Exists(RegistryStrings.ObsDir) && File.Exists(RegistryStrings.ObsBinaryPath))
        {
          if (!RegistryManager.Instance.IsBTVCheckedAfterUpdate && !CommonHandlers.IsBtvLatestVersionDownloaded())
          {
            this.DownloadObsPopup();
            ClientStats.SendMiscellaneousStatsAsync(location, RegistryManager.Instance.UserGuid, "VideoRecordingDownload", action, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
          }
          else if (!ProcessUtils.FindProcessByName("HD-OBS"))
          {
            if (!this.InsufficientSpacePopup())
            {
              this.RecordVideoOfApp();
              ClientStats.SendMiscellaneousStatsAsync(location, RegistryManager.Instance.UserGuid, "VideoRecordingStart", action, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
            }
          }
          else
            this.ShowAlreadyRunningPopUpForOBS();
        }
        else
        {
          this.DownloadObsPopup();
          ClientStats.SendMiscellaneousStatsAsync(location, RegistryManager.Instance.UserGuid, "VideoRecordingDownload", action, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
        }
        RegistryManager.Instance.IsBTVCheckedAfterUpdate = true;
      }
    }

    private bool InsufficientSpacePopup()
    {
      bool recording = true;
      for (double availableSpaceinMb = CommonHandlers.FindAvailableSpaceinMB(RegistryManager.Instance.ScreenShotsPath); availableSpaceinMb < 30.0 && recording; availableSpaceinMb = CommonHandlers.FindAvailableSpaceinMB(RegistryManager.Instance.ScreenShotsPath))
      {
        RegistryManager.Instance.IsScreenshotsLocationPopupEnabled = false;
        string screenShotsPath = RegistryManager.Instance.ScreenShotsPath;
        CustomMessageWindow customMessageWindow = new CustomMessageWindow();
        BlueStacksUIBinding.Bind(customMessageWindow.TitleTextBlock, "STRING_INSUFFICIENT_SPACE", "");
        customMessageWindow.AddButton(ButtonColors.Blue, "STRING_CHANGE_PATH", new EventHandler(this.ChooseCustomFolder), (string) null, false, (object) null, true);
        customMessageWindow.AddButton(ButtonColors.White, "STRING_STOP_RECORDING", (EventHandler) ((o, evt) => recording = false), (string) null, false, (object) null, true);
        customMessageWindow.CloseButton.PreviewMouseUp += (MouseButtonEventHandler) ((o, evt) => recording = false);
        BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlock, "STRING_INSUFFICIENT_RECORDING_SPACE", "");
        customMessageWindow.BodyWarningTextBlock.Visibility = Visibility.Visible;
        customMessageWindow.BodyWarningTextBlock.Text = screenShotsPath;
        BlueStacksUIBinding.BindColor((DependencyObject) customMessageWindow.BodyWarningTextBlock, TextBlock.ForegroundProperty, "HyperLinkForegroundColor");
        this.ParentWindow.ShowDimOverlay((IDimOverlayControl) null);
        customMessageWindow.Owner = (Window) this.ParentWindow.mDimOverlay;
        customMessageWindow.ShowDialog();
        this.ParentWindow.HideDimOverlay();
      }
      return !recording;
    }

    private static double FindAvailableSpaceinMB(string path)
    {
      double num1 = double.MaxValue;
      string pathRoot = Path.GetPathRoot(path);
      double num2 = Math.Pow(2.0, 20.0);
      foreach (DriveInfo drive in DriveInfo.GetDrives())
      {
        if (drive.IsReady && drive.Name == pathRoot)
          num1 = (double) drive.AvailableFreeSpace / num2;
      }
      return num1;
    }

    private void ShowAlreadyRunningPopUpForOBS()
    {
      CustomMessageWindow customMessageWindow = new CustomMessageWindow();
      BlueStacksUIBinding.Bind(customMessageWindow.TitleTextBlock, "STRING_NOT_START_RECORDER", "");
      BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlock, "STRING_RECORDER_ALREADY_RUNNING", "");
      customMessageWindow.AddButton(ButtonColors.Blue, "STRING_OK", (EventHandler) null, (string) null, false, (object) null, true);
      customMessageWindow.Owner = (Window) this.ParentWindow;
      this.ParentWindow.ShowDimOverlay((IDimOverlayControl) null);
      customMessageWindow.ShowDialog();
      this.ParentWindow.HideDimOverlay();
    }

    private static bool IsBtvLatestVersionDownloaded()
    {
      return string.Compare(RegistryManager.Instance.CurrentBtvVersionInstalled, Path.GetFileNameWithoutExtension(new Uri(CommonHandlers.GetBtvUrl()).LocalPath), StringComparison.InvariantCulture) >= 0;
    }

    private static string GetBtvUrl()
    {
      string url = WebHelper.GetUrlWithParams(RegistryManager.Instance.Host + "/bs4/btv/GetBTVFile", (string) null, (string) null, (string) null);
      if (!string.IsNullOrEmpty(RegistryManager.Instance.BtvDevServer))
        url = RegistryManager.Instance.BtvDevServer;
      return BTVManager.GetRedirectedUrl(url);
    }

    private void DownloadObsPopup()
    {
      if (CommonHandlers.sDownloading && CommonHandlers.sWindow != null && !CommonHandlers.sWindow.IsClosed)
        this.DownloadObs((object) null, (EventArgs) null);
      else
        this.ParentWindow.Dispatcher.Invoke((Delegate) (() =>
        {
          CustomMessageWindow customMessageWindow = new CustomMessageWindow();
          customMessageWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_RECORDER_REQUIRED", "");
          customMessageWindow.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_VIDEO_RECORDER_DOWNLOAD_BODY", "");
          customMessageWindow.AddButton(ButtonColors.Blue, "STRING_DOWNLOAD_NOW", new EventHandler(this.DownloadObs), (string) null, false, (object) null, true);
          customMessageWindow.Owner = (Window) this.ParentWindow;
          customMessageWindow.ContentMaxWidth = 450.0;
          this.ParentWindow.ShowDimOverlay((IDimOverlayControl) null);
          customMessageWindow.ShowDialog();
          this.ParentWindow.HideDimOverlay();
        }));
    }

    private void DownloadObs(object sender, EventArgs e)
    {
      if (CommonHandlers.sDownloading && CommonHandlers.sWindow != null && !CommonHandlers.sWindow.IsClosed)
        BTVManager.BringToFront((CustomWindow) CommonHandlers.sWindow);
      else if (!BTVManager.IsDirectXComponentsInstalled())
      {
        CustomMessageWindow downloadReqWindow = new CustomMessageWindow();
        downloadReqWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_ADDITIONAL_FILES_REQUIRED", "");
        downloadReqWindow.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_SOME_WINDOW_FILES_MISSING", "");
        string directXDownloadURL = "http://www.microsoft.com/en-us/download/details.aspx?id=35";
        downloadReqWindow.AddHyperLinkInUI(directXDownloadURL, new Uri(directXDownloadURL), (RequestNavigateEventHandler) ((o, arg) =>
        {
          BlueStacksUIUtils.OpenUrl(arg.Uri.ToString());
          downloadReqWindow.CloseWindow();
        }));
        downloadReqWindow.AddButton(ButtonColors.Blue, "STRING_DOWNLOAD_NOW", (EventHandler) ((o, args) => BlueStacksUIUtils.OpenUrl(directXDownloadURL)), (string) null, false, (object) null, true);
        downloadReqWindow.Owner = (Window) this.ParentWindow;
        downloadReqWindow.ContentMaxWidth = 450.0;
        this.ParentWindow.ShowDimOverlay((IDimOverlayControl) null);
        downloadReqWindow.ShowDialog();
        this.ParentWindow.HideDimOverlay();
      }
      else
      {
        CommonHandlers.sDownloading = true;
        CommonHandlers.sWindow = new CustomMessageWindow();
        BlueStacksUIBinding.Bind(CommonHandlers.sWindow.TitleTextBlock, "STRING_DOWNLOAD_ADDITIONAL", "");
        BlueStacksUIBinding.Bind(CommonHandlers.sWindow.BodyWarningTextBlock, "STRING_NOT_CLOSE_DOWNLOAD_COMPLETE", "");
        CommonHandlers.sWindow.BodyWarningTextBlock.Visibility = Visibility.Visible;
        CommonHandlers.sWindow.BodyTextBlock.Visibility = Visibility.Collapsed;
        CommonHandlers.sWindow.CloseButtonHandle(new Predicate<object>(this.RecorderDownloadCancelledHandler), (object) null);
        CommonHandlers.sWindow.MinimizeEventHandler += new EventHandler(this.BtvDownloadWindowMinimizedHandler);
        CommonHandlers.sWindow.ProgressBarEnabled = true;
        CommonHandlers.sWindow.IsWindowMinizable = true;
        CommonHandlers.sWindow.IsWindowClosable = true;
        CommonHandlers.sWindow.ShowInTaskbar = false;
        CommonHandlers.sWindow.IsWithoutButtons = true;
        CommonHandlers.sWindow.ContentMaxWidth = 450.0;
        CommonHandlers.sWindow.IsDraggable = true;
        CommonHandlers.sWindow.Owner = (Window) this.ParentWindow;
        CommonHandlers.sWindow.IsShowGLWindow = true;
        CommonHandlers.sWindow.Show();
        new Thread((ThreadStart) (() =>
        {
          string btvUrl = CommonHandlers.GetBtvUrl();
          if (btvUrl == null)
          {
            Logger.Error("The download url was null");
            this.ShowErrorDownloadingRecorder();
          }
          else
          {
            string fileName = Path.GetFileName(new Uri(btvUrl).LocalPath);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(new Uri(btvUrl).LocalPath);
            string downloadPath = Path.Combine(Path.GetTempPath(), fileName);
            this.mDownloader = new LegacyDownloader(3, btvUrl, downloadPath);
            this.mDownloader.Download((LegacyDownloader.UpdateProgressCallback) (percent => this.ParentWindow.Dispatcher.Invoke((Delegate) (() =>
            {
              if (CommonHandlers.sWindow == null)
                return;
              CommonHandlers.sWindow.CustomProgressBar.Value = (double) percent;
            }))), (LegacyDownloader.DownloadCompletedCallback) (filePath =>
            {
              this.ParentWindow.Dispatcher.Invoke((Delegate) (() =>
              {
                if (CommonHandlers.sWindow == null)
                  return;
                CommonHandlers.sWindow.CustomProgressBar.Value = 100.0;
              }));
              Logger.Info("Successfully downloaded BlueStacks TV");
              RegistryManager.Instance.CurrentBtvVersionInstalled = fileNameWithoutExtension;
              if (BTVManager.ExtractBTv(downloadPath))
              {
                Utils.DeleteFile(downloadPath);
                this.ParentWindow.Dispatcher.Invoke((Delegate) (() =>
                {
                  CommonHandlers.sWindow?.Close();
                  CommonHandlers.sWindow = (CustomMessageWindow) null;
                  if (this.ParentWindow.IsClosed)
                    return;
                  CustomMessageWindow customMessageWindow = new CustomMessageWindow();
                  BlueStacksUIBinding.Bind(customMessageWindow.TitleTextBlock, "STRING_RECORDER_DOWNLOADED", "");
                  BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlock, "STRING_RECORDER_READY_BODY", "");
                  customMessageWindow.AddButton(ButtonColors.Blue, "STRING_OK", (EventHandler) null, (string) null, false, (object) null, true);
                  customMessageWindow.Owner = (Window) this.ParentWindow;
                  customMessageWindow.ContentMaxWidth = 450.0;
                  this.ParentWindow.ShowDimOverlay((IDimOverlayControl) null);
                  customMessageWindow.ShowDialog();
                  this.ParentWindow.HideDimOverlay();
                }));
              }
              else
              {
                Utils.DeleteFile(downloadPath);
                this.ShowErrorDownloadingRecorder();
              }
            }), (LegacyDownloader.ExceptionCallback) (ex =>
            {
              Logger.Error("Failed to download file: {0}. err: {1}", (object) downloadPath, (object) ex.Message);
              if (ex.InnerException is OperationCanceledException)
                return;
              this.ShowErrorDownloadingRecorder();
            }), (LegacyDownloader.ContentTypeCallback) null, (LegacyDownloader.SizeDownloadedCallback) (size => this.ParentWindow.Dispatcher.Invoke((Delegate) (() =>
            {
              if (CommonHandlers.sWindow == null)
                return;
              CommonHandlers.sWindow.ProgressStatusTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_DOWNLOADING", "");
              CommonHandlers.sWindow.ProgressPercentageTextBlock.Content = (object) (((float) size / 1048576f).ToString("F", (IFormatProvider) CultureInfo.InvariantCulture) + " MB / " + this.mRecorderSizeMb.ToString("F", (IFormatProvider) CultureInfo.InvariantCulture) + " MB ");
              this.mDownloadedSize = size;
            }))), (LegacyDownloader.PayloadInfoCallback) (size => this.mRecorderSizeMb = (float) size / 1048576f));
            CommonHandlers.sDownloading = false;
          }
        }))
        {
          IsBackground = true
        }.Start();
        this.mDownloadStatusTimer = new DispatcherTimer()
        {
          Interval = new TimeSpan(0, 0, 5)
        };
        this.mDownloadStatusTimer.Tick += new EventHandler(this.DownloadStatusTimerTick);
        this.mDownloadStatusTimer.Start();
      }
    }

    private void BtvDownloadWindowMinimizedHandler(object sender, EventArgs e)
    {
      this.ParentWindow.Dispatcher.Invoke((Delegate) (() =>
      {
        this.OnBTvDownloaderMinimized();
        this.ParentWindow.Focus();
      }));
    }

    private void DownloadStatusTimerTick(object sender, EventArgs e)
    {
      if (CommonHandlers.sDownloading || CommonHandlers.sWindow == null)
      {
        if (CommonHandlers.sWindow != null)
        {
          try
          {
            if (this.mLastSizeChecked != this.mDownloadedSize)
            {
              this.mLastSizeChecked = this.mDownloadedSize;
              CommonHandlers.sWindow.ProgressStatusTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_DOWNLOADING", "");
              return;
            }
            CommonHandlers.sWindow.ProgressStatusTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_WAITING_FOR_INTERNET", "");
            return;
          }
          catch (Exception ex)
          {
            Logger.Error("Exception in DownloadStatusTimerTick. Exception: " + ex?.ToString());
            return;
          }
        }
      }
      this.mDownloadStatusTimer.Stop();
    }

    private void ShowErrorDownloadingRecorder()
    {
      this.ParentWindow.Dispatcher.Invoke((Delegate) (() =>
      {
        CustomMessageWindow customMessageWindow = new CustomMessageWindow();
        BlueStacksUIBinding.Bind(customMessageWindow.TitleTextBlock, "STRING_DOWNLOAD_FAILED", "");
        BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlock, "STRING_ERROR_RECORDER_DOWNLOAD", "");
        customMessageWindow.AddButton(ButtonColors.Blue, "STRING_CLOSE", (EventHandler) null, (string) null, false, (object) null, true);
        customMessageWindow.Owner = (Window) this.ParentWindow;
        customMessageWindow.ContentMaxWidth = 450.0;
        this.ParentWindow.ShowDimOverlay((IDimOverlayControl) null);
        customMessageWindow.ShowDialog();
        this.ParentWindow.HideDimOverlay();
        CommonHandlers.sWindow?.Close();
        CommonHandlers.sWindow = (CustomMessageWindow) null;
      }));
    }

    internal void ShowErrorRecordingVideoPopup()
    {
      this.ParentWindow.Dispatcher.Invoke((Delegate) (() =>
      {
        CustomMessageWindow customMessageWindow = new CustomMessageWindow();
        BlueStacksUIBinding.Bind(customMessageWindow.TitleTextBlock, "STRING_RECORDING_ERROR", "");
        BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlock, "STRING_RECORDING_ERROR_BODY", "");
        customMessageWindow.AddButton(ButtonColors.Blue, "STRING_OK", (EventHandler) null, (string) null, false, (object) null, true);
        customMessageWindow.Owner = (Window) this.ParentWindow;
        customMessageWindow.ContentMaxWidth = 450.0;
        this.ParentWindow.ShowDimOverlay((IDimOverlayControl) null);
        customMessageWindow.ShowDialog();
        this.ParentWindow.HideDimOverlay();
      }));
    }

    private bool RecorderDownloadCancelledHandler(object sender)
    {
      CustomMessageWindow cancelDownloadConfirmation = new CustomMessageWindow();
      BlueStacksUIBinding.Bind(cancelDownloadConfirmation.TitleTextBlock, "STRING_DOWNLOAD_IN_PROGRESS", "");
      BlueStacksUIBinding.Bind(cancelDownloadConfirmation.BodyTextBlock, "STRING_DOWNLOAD_NOT_COMPLETE", "");
      cancelDownloadConfirmation.AddButton(ButtonColors.Red, "STRING_CANCEL", (EventHandler) ((o, args) =>
      {
        CommonHandlers.sWindow?.Close();
        CommonHandlers.sWindow = (CustomMessageWindow) null;
        this.mDownloader?.AbortDownload();
      }), (string) null, false, (object) null, true);
      cancelDownloadConfirmation.AddButton(ButtonColors.White, "STRING_CONTINUE", (EventHandler) ((o, args) => cancelDownloadConfirmation.DialogResult = new bool?(true)), (string) null, false, (object) null, true);
      cancelDownloadConfirmation.Owner = (Window) this.ParentWindow;
      this.ParentWindow.ShowDimOverlay((IDimOverlayControl) null);
      bool? nullable1 = cancelDownloadConfirmation.ShowDialog();
      this.ParentWindow.HideDimOverlay();
      bool? nullable2 = nullable1;
      bool flag = true;
      return nullable2.GetValueOrDefault() == flag & nullable2.HasValue;
    }

    public void RecordingStarted()
    {
      this.ParentWindow.Dispatcher.Invoke((Delegate) (() =>
      {
        CommonHandlers.sIsOBSStartingStopping = false;
        this.mObsResponseTimeoutTimer?.Stop();
        this.OnScreenRecordingStateChanged(true);
        if (!RegistryManager.Instance.IsShowToastNotification)
          return;
        this.ParentWindow.ShowGeneralToast(LocaleStrings.GetLocalizedString("STRING_RECORDING_STARTED", ""));
      }));
      ClientStats.SendMiscellaneousStatsAsync("VideoRecording", RegistryManager.Instance.UserGuid, "VideoRecordingStarted", RegistryManager.Instance.ScreenShotsPath, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
    }

    public void StopMacroRecording()
    {
      this.ParentWindow.mFrontendHandler.SendFrontendRequestAsync("stopRecordingCombo", (Dictionary<string, string>) null);
      foreach (SingleMacroControl child in this.ParentWindow.MacroRecorderWindow.mScriptsStackPanel.Children)
        CommonHandlers.EnableScriptControl(child);
      this.ParentWindow.MacroRecorderWindow.mStartMacroRecordingBtn.Visibility = Visibility.Visible;
      this.ParentWindow.MacroRecorderWindow.mStopMacroRecordingBtn.Visibility = Visibility.Collapsed;
      this.ParentWindow.MacroRecorderWindow.mScriptsStackPanel.Visibility = Visibility.Visible;
      this.ParentWindow.mTopBar.mMacroRecorderToolTipPopup.IsOpen = false;
      if (FeatureManager.Instance.IsCustomUIForNCSoft)
      {
        this.ParentWindow.mNCTopBar.mMacroRecordGrid.Visibility = Visibility.Collapsed;
        this.ParentWindow.mNCTopBar.mMacroRecordControl.StopTimer();
      }
      else
      {
        this.ParentWindow.mTopBar.mMacroRecordControl.Visibility = Visibility.Collapsed;
        this.ParentWindow.mTopBar.mMacroRecordControl.StopTimer();
      }
    }

    public void StartMacroRecording()
    {
      this.ParentWindow.mIsMacroRecorderActive = true;
      if (FeatureManager.Instance.IsCustomUIForNCSoft)
        this.ParentWindow.mNCTopBar.ShowRecordingIcons();
      else
        this.ParentWindow.mTopBar.ShowRecordingIcons();
      this.ParentWindow.MacroRecorderWindow.mStartMacroRecordingBtn.Visibility = Visibility.Collapsed;
      this.ParentWindow.MacroRecorderWindow.mStopMacroRecordingBtn.Visibility = Visibility.Visible;
      foreach (SingleMacroControl child in this.ParentWindow.MacroRecorderWindow.mScriptsStackPanel.Children)
        CommonHandlers.DisableScriptControl(child);
      this.ParentWindow.mCommonHandler.HideMacroRecorderWindow();
      this.ParentWindow.Focus();
      this.ParentWindow.mFrontendHandler.SendFrontendRequestAsync("startRecordingCombo", (Dictionary<string, string>) null);
    }

    internal void InitUiOnMacroPlayback(MacroRecording recording)
    {
      this.ParentWindow.Dispatcher.Invoke((Delegate) (() =>
      {
        this.ParentWindow.Focus();
        this.ParentWindow.MacroRecorderWindow.mStartMacroRecordingBtn.IsEnabled = false;
        this.ParentWindow.MacroRecorderWindow.mStartMacroRecordingBtn.Opacity = 0.6;
        if (FeatureManager.Instance.IsCustomUIForNCSoft)
        {
          this.ParentWindow.mNCTopBar.ShowMacroPlaybackOnTopBar(recording);
          this.ParentWindow.mNCTopBar.mMacroPlayControl.mStartTime = DateTime.Now;
        }
        else
        {
          this.ParentWindow.mTopBar.ShowMacroPlaybackOnTopBar(recording);
          this.ParentWindow.mTopBar.mMacroPlayControl.mStartTime = DateTime.Now;
        }
        this.ParentWindow.mMacroPlaying = recording.Name;
        if (!recording.RestartPlayer)
          return;
        this.ParentWindow.StartTimerForAppPlayerRestart(recording.RestartPlayerAfterMinutes);
      }));
    }

    internal void PlayMacroScript(MacroRecording record)
    {
      this.ParentWindow.Dispatcher.Invoke((Delegate) (() =>
      {
        this.ParentWindow.MacroRecorderWindow.mStartMacroRecordingBtn.Visibility = Visibility.Visible;
        this.ParentWindow.MacroRecorderWindow.mStartMacroRecordingBtn.IsEnabled = false;
        this.ParentWindow.MacroRecorderWindow.mStartMacroRecordingBtn.Opacity = 0.6;
        foreach (SingleMacroControl child in this.ParentWindow.MacroRecorderWindow.mScriptsStackPanel.Children)
        {
          if (child.mRecording.Name != record.Name)
            CommonHandlers.DisableScriptControl(child);
          else
            child.mEditNameImg.IsEnabled = false;
        }
        this.ParentWindow.MacroRecorderWindow.RunMacroOperation(record);
      }));
    }

    internal void FullMacroScriptPlayHandler(MacroRecording record)
    {
      string name = record.Name;
      this.ParentWindow.mCommonHandler.PlayMacroScript(record);
      if (FeatureManager.Instance.IsCustomUIForNCSoft)
        this.ParentWindow.mNCTopBar.mMacroPlayControl.OnScriptPlayEvent(name);
      else
        this.ParentWindow.mTopBar.mMacroPlayControl.OnScriptPlayEvent(name);
    }

    internal void StopMacroScriptHandling()
    {
      this.ParentWindow.MacroRecorderWindow.mBGMacroPlaybackWorker.CancelAsync();
      this.StopMacroPlaybackOperation();
      this.ParentWindow.SetMacroPlayBackEventHandle();
    }

    internal void StopMacroPlaybackOperation()
    {
      Logger.Info("In StopMacroPlaybackOperation");
      this.ParentWindow.mIsMacroPlaying = false;
      foreach (SingleMacroControl child in this.ParentWindow.MacroRecorderWindow.mScriptsStackPanel.Children)
        CommonHandlers.EnableScriptControl(child);
      this.ParentWindow.MacroRecorderWindow.mStartMacroRecordingBtn.Visibility = Visibility.Visible;
      this.ParentWindow.MacroRecorderWindow.mStartMacroRecordingBtn.IsEnabled = true;
      this.ParentWindow.MacroRecorderWindow.mStartMacroRecordingBtn.Opacity = 1.0;
      this.ParentWindow.MacroRecorderWindow.mScriptsStackPanel.Visibility = Visibility.Visible;
      if (FeatureManager.Instance.IsCustomUIForNCSoft)
        this.ParentWindow.mNCTopBar.HideMacroPlaybackFromTopBar();
      else
        this.ParentWindow.mTopBar.HideMacroPlaybackFromTopBar();
      this.ParentWindow.mMacroPlaying = string.Empty;
      if (this.ParentWindow.mMacroTimer != null && this.ParentWindow.mMacroTimer.Enabled)
      {
        this.ParentWindow.mMacroTimer.Enabled = false;
        this.ParentWindow.mMacroTimer.AutoReset = false;
        this.ParentWindow.mMacroTimer.Dispose();
      }
      this.ParentWindow.mTopBar.mMacroRunningToolTipPopup.IsOpen = false;
      this.ParentWindow.mFrontendHandler.SendFrontendRequestAsync("stopMacroPlayback", (Dictionary<string, string>) null);
    }

    public static void EnableScriptControl(SingleMacroControl mScriptControl)
    {
      mScriptControl.Opacity = 1.0;
      mScriptControl.mBookmarkImg.IsEnabled = true;
      mScriptControl.mEditNameImg.IsEnabled = true;
      mScriptControl.mPlayScriptImg.IsEnabled = true;
      mScriptControl.mScriptSettingsImg.IsEnabled = true;
      mScriptControl.mMergeScriptSettingsImg.IsEnabled = true;
      mScriptControl.mDeleteScriptImg.IsEnabled = true;
    }

    public static void DisableScriptControl(SingleMacroControl mScriptControl)
    {
      mScriptControl.Opacity = 0.4;
      mScriptControl.mBookmarkImg.IsEnabled = false;
      mScriptControl.mEditNameImg.IsEnabled = false;
      mScriptControl.mPlayScriptImg.IsEnabled = false;
      mScriptControl.mScriptSettingsImg.IsEnabled = false;
      mScriptControl.mMergeScriptSettingsImg.IsEnabled = false;
      mScriptControl.mDeleteScriptImg.IsEnabled = false;
    }

    internal void CheckForMacroScriptOnRestart()
    {
      foreach (MacroRecording record in MacroGraph.Instance.Vertices.Cast<MacroRecording>().Where<MacroRecording>((Func<MacroRecording, bool>) (macro => macro.PlayOnStart)))
        this.InitUiAndPlayMacroScript(record);
    }

    private void InitUiAndPlayMacroScript(MacroRecording record)
    {
      this.ParentWindow.Dispatcher.Invoke((Delegate) (() =>
      {
        this.RefreshMacroRecorderWindow();
        this.ParentWindow.mTopBar.mMacroPlayControl.OnScriptPlayEvent(record.Name.ToLower(CultureInfo.InvariantCulture));
        this.PlayMacroScript(record);
      }));
    }

    public static void OpenMediaFolder()
    {
      if (!Directory.Exists(RegistryManager.Instance.ScreenShotsPath))
        return;
      using (Process process = new Process())
      {
        process.StartInfo.UseShellExecute = true;
        process.StartInfo.FileName = RegistryManager.Instance.ScreenShotsPath;
        process.Start();
      }
    }

    public static void OpenMediaFolderWithFileSelected(string selectedFile)
    {
      if (!Directory.Exists(RegistryManager.Instance.ScreenShotsPath))
        return;
      using (Process process = new Process())
      {
        process.StartInfo.UseShellExecute = true;
        process.StartInfo.FileName = "explorer.exe";
        process.StartInfo.Arguments = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/select,\"{0}\"", (object) selectedFile);
        process.Start();
      }
    }

    internal void SetSidebarImageProperties(bool isVisible, CustomPictureBox cpb, TextBlock tb)
    {
      if (isVisible)
      {
        if (cpb != null)
        {
          cpb.ImageName = "sidebar_hide";
          BlueStacksUIBinding.Bind((System.Windows.Controls.Image) cpb, "STRING_CLOSE_SIDEBAR");
        }
        if (tb == null)
          return;
        BlueStacksUIBinding.Bind(tb, "STRING_CLOSE_SIDEBAR", "");
      }
      else
      {
        if (cpb != null)
        {
          cpb.ImageName = "sidebar_show";
          BlueStacksUIBinding.Bind((System.Windows.Controls.Image) cpb, "STRING_OPEN_SIDEBAR");
        }
        if (tb == null)
          return;
        BlueStacksUIBinding.Bind(tb, "STRING_OPEN_SIDEBAR", "");
      }
    }

    internal void FlipSidebarVisibility(CustomPictureBox cpb, TextBlock tb)
    {
      if (cpb.ImageName == "sidebar_hide")
      {
        this.ParentWindow.mSidebar.Visibility = Visibility.Collapsed;
        cpb.ImageName = "sidebar_show";
        BlueStacksUIBinding.Bind((System.Windows.Controls.Image) cpb, "STRING_OPEN_SIDEBAR");
        if (tb != null)
          BlueStacksUIBinding.Bind(tb, "STRING_OPEN_SIDEBAR", "");
        this.ParentWindow.EngineInstanceRegistry.IsSidebarVisible = false;
      }
      else
      {
        this.ParentWindow.mSidebar.Visibility = Visibility.Visible;
        cpb.ImageName = "sidebar_hide";
        BlueStacksUIBinding.Bind((System.Windows.Controls.Image) cpb, "STRING_CLOSE_SIDEBAR");
        if (tb != null)
          BlueStacksUIBinding.Bind(tb, "STRING_CLOSE_SIDEBAR", "");
        this.ParentWindow.EngineInstanceRegistry.IsSidebarVisible = true;
      }
      this.ParentWindow.mSidebar.SidebarVisiblityChanged(this.ParentWindow.mSidebar.Visibility);
    }

    internal void InitShortcuts()
    {
      try
      {
        this.mShortcutsConfigInstance = ShortcutConfig.LoadShortcutsConfig();
        if (this.mShortcutsConfigInstance == null)
          return;
        List<ShortcutKeys> shortcutKeysList = new List<ShortcutKeys>();
        foreach (ShortcutKeys shortcutKeys in this.mShortcutsConfigInstance.Shortcut)
        {
          if (string.Equals(shortcutKeys.ShortcutName, "STRING_MACRO_RECORDER", StringComparison.InvariantCulture))
          {
            if (!FeatureManager.Instance.IsMacroRecorderEnabled && !FeatureManager.Instance.IsCustomUIForNCSoft)
              shortcutKeysList.Add(shortcutKeys);
          }
          else if (string.Equals(shortcutKeys.ShortcutName, "STRING_SYNCHRONISER", StringComparison.InvariantCulture))
          {
            if (!FeatureManager.Instance.IsOperationsSyncEnabled)
              shortcutKeysList.Add(shortcutKeys);
          }
          else if (string.Equals(shortcutKeys.ShortcutName, "STRING_TOGGLE_FARM_MODE", StringComparison.InvariantCulture) && FeatureManager.Instance.IsFarmingModeDisabled)
            shortcutKeysList.Add(shortcutKeys);
        }
        foreach (ShortcutKeys shortcutKeys in shortcutKeysList)
          this.mShortcutsConfigInstance.Shortcut.Remove(shortcutKeys);
      }
      catch (Exception ex)
      {
        Logger.Error("error while init shortcut : {0}", (object) ex);
      }
    }

    internal void SaveAndReloadShortcuts()
    {
      try
      {
        this.mShortcutsConfigInstance.SaveUserDefinedShortcuts();
        CommonHandlers.ReloadShortcutsForAllInstances();
        BlueStacks.Common.Stats.SendMiscellaneousStatsAsync("KeyboardShortcuts", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "shortcut_save", (string) null, (string) null, (string) null, (string) null, (string) null, "Android", 0);
      }
      catch (Exception ex)
      {
        Logger.Error("Error saving shortcut registry" + ex.ToString());
      }
    }

    internal static void ReloadShortcutsForAllInstances()
    {
      foreach (string runningInstances in Utils.GetRunningInstancesList())
      {
        if (BlueStacksUIUtils.DictWindows.ContainsKey(runningInstances))
        {
          BlueStacksUIUtils.DictWindows[runningInstances].mCommonHandler.InitShortcuts();
          BlueStacksUIUtils.DictWindows[runningInstances].mCommonHandler.ReloadBossKey();
          BlueStacksUIUtils.DictWindows[runningInstances].mCommonHandler.ReloadTooltips();
        }
        HTTPUtils.SendRequestToEngineAsync("reloadShortcutsConfig", (Dictionary<string, string>) null, runningInstances, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
      }
    }

    internal void ReloadTooltips()
    {
      foreach (SidebarElement listSidebarElement in this.ParentWindow.mSidebar.mListSidebarElements)
        this.ParentWindow.mSidebar.SetSidebarElementTooltip(listSidebarElement, listSidebarElement.mSidebarElementTooltipKey);
    }

    private void ReloadBossKey()
    {
      RegistryManager.Instance.BossKey = this.GetShortcutKeyFromName("STRING_BOSSKEY_SETTING", true);
      if (string.IsNullOrEmpty(RegistryManager.Instance.BossKey))
        GlobalKeyBoardMouseHooks.UnsetKey();
      else
        GlobalKeyBoardMouseHooks.SetKey(RegistryManager.Instance.BossKey);
    }

    internal static void ArrangeWindowInTiles()
    {
      long columns = RegistryManager.Instance.TileWindowColumnCount;
      long rows = (long) Math.Ceiling((double) BlueStacksUIUtils.DictWindows.Count / (double) columns);
      double val1 = (double) Screen.PrimaryScreen.WorkingArea.Height;
      double y = (double) Screen.PrimaryScreen.WorkingArea.Top;
      double x = (double) Screen.PrimaryScreen.WorkingArea.Left;
      int num = 0;
      foreach (KeyValuePair<string, MainWindow> dictWindow in BlueStacksUIUtils.DictWindows)
      {
        KeyValuePair<string, MainWindow> item = dictWindow;
        double windowWidth = (double) ((long) Screen.PrimaryScreen.WorkingArea.Width / columns);
        double windowHeight = (double) ((long) Screen.PrimaryScreen.WorkingArea.Height / rows);
        double overlapWidth = 0.0;
        double overlapHeight = 0.0;
        item.Value.Dispatcher.Invoke((Delegate) (() =>
        {
          if (item.Value.WindowState == WindowState.Minimized || item.Value.WindowState == WindowState.Maximized)
            item.Value.RestoreWindows(true);
          KMManager.CloseWindows();
          if (item.Value.mAspectRatio < (Fraction) 1L)
          {
            if (item.Value.GetWidthFromHeight(windowHeight, true, true) > windowWidth)
              windowHeight = item.Value.GetHeightFromWidth(windowWidth, true, true);
            else
              windowWidth = item.Value.GetWidthFromHeight(windowHeight, true, true);
            if (windowWidth < (double) item.Value.MinWidthScaled)
            {
              windowWidth = (double) item.Value.MinWidthScaled;
              windowHeight = item.Value.GetHeightFromWidth(windowWidth, true, true);
              CommonHandlers.CalculateOverlappingLength(windowWidth, windowHeight, rows, columns, out overlapWidth, out overlapHeight);
            }
            item.Value.ChangeHeightWidthTopLeft(windowWidth, windowHeight, y, x);
          }
          else
          {
            if (item.Value.GetHeightFromWidth(windowWidth, true, true) > windowHeight)
              windowWidth = item.Value.GetWidthFromHeight(windowHeight, true, true);
            else
              windowHeight = item.Value.GetHeightFromWidth(windowWidth, true, true);
            if (windowHeight < (double) item.Value.MinHeightScaled)
            {
              windowHeight = (double) item.Value.MinHeightScaled;
              windowWidth = item.Value.GetWidthFromHeight(windowHeight, true, true);
              CommonHandlers.CalculateOverlappingLength(windowWidth, windowHeight, rows, columns, out overlapWidth, out overlapHeight);
            }
            item.Value.ChangeHeightWidthTopLeft(windowWidth, windowHeight, y, x);
          }
          if (item.Value.Topmost)
            return;
          item.Value.Topmost = true;
          ThreadPool.QueueUserWorkItem(closure_1 ?? (closure_1 = (WaitCallback) (obj => item.Value.Dispatcher.Invoke((Delegate) (() => item.Value.Topmost = false)))));
        }));
        x += windowWidth - overlapWidth;
        val1 = Math.Min(val1, windowHeight);
        ++num;
        if ((long) num % columns == 0L)
        {
          y += Math.Max(val1 - overlapHeight, 0.0);
          x = 0.0;
        }
      }
    }

    internal static void CalculateOverlappingLength(
      double windowWidth,
      double windowHeight,
      long rows,
      long columns,
      out double overlapWidth,
      out double overlapHeight)
    {
      overlapHeight = 0.0;
      overlapWidth = 0.0;
      if (windowWidth * (double) columns > (double) Screen.PrimaryScreen.WorkingArea.Width)
      {
        double num = windowWidth * (double) columns - (double) Screen.PrimaryScreen.WorkingArea.Width;
        overlapWidth = num / (double) (columns - 1L);
      }
      if (windowHeight * (double) rows <= (double) Screen.PrimaryScreen.WorkingArea.Height)
        return;
      double num1 = windowHeight * (double) rows - (double) Screen.PrimaryScreen.WorkingArea.Height;
      overlapHeight = Math.Max(overlapHeight, num1 / (double) (rows - 1L));
    }

    internal static void ArrangeWindowInCascade()
    {
      double top = (double) Screen.PrimaryScreen.WorkingArea.Top;
      double bottom = (double) Screen.PrimaryScreen.WorkingArea.Bottom;
      double left = (double) Screen.PrimaryScreen.WorkingArea.Left;
      double right = (double) Screen.PrimaryScreen.WorkingArea.Right;
      double width = (double) Screen.PrimaryScreen.WorkingArea.Width;
      double height = (double) Screen.PrimaryScreen.WorkingArea.Height;
      double windowWidth = (double) (int) (width / 3.0);
      double windowHeight = (double) (int) (height / 3.0);
      double y = top;
      double x = left;
      foreach (KeyValuePair<string, MainWindow> dictWindow in BlueStacksUIUtils.DictWindows)
      {
        KeyValuePair<string, MainWindow> item = dictWindow;
        IntPtr handle = item.Value.Handle;
        item.Value.Dispatcher.Invoke((Delegate) (() =>
        {
          if (item.Value.WindowState == WindowState.Minimized)
            item.Value.RestoreWindows(false);
          KMManager.CloseWindows();
          windowHeight = item.Value.GetHeightFromWidth(windowWidth, false, false);
          item.Value.ChangeHeightWidthTopLeft(windowWidth, windowHeight, y, x);
          item.Value.Focus();
        }));
        x += 40.0;
        y += 40.0;
        if (y >= bottom || x >= right)
        {
          y = top + 40.0;
          x = left + 40.0;
        }
      }
    }

    public void SetNcSoftStreamingStatus(string status)
    {
      if (status.Equals("on", StringComparison.InvariantCultureIgnoreCase))
      {
        SidebarElement elementFromTag = this.ParentWindow.mSidebar.GetElementFromTag("sidebar_stream_video");
        this.ParentWindow.mSidebar.UpdateImage("sidebar_stream_video", "sidebar_stream_video_active");
        elementFromTag.Image.Width = 44.0;
        elementFromTag.Image.Height = 44.0;
        this.ParentWindow.mNCTopBar.ChangeTopBarColor("StreamingTopBarColor");
        this.ParentWindow.mNCTopBar.mStreamingTopbarGrid.Visibility = Visibility.Visible;
        this.ParentWindow.mIsStreaming = true;
      }
      else
      {
        SidebarElement elementFromTag = this.ParentWindow.mSidebar.GetElementFromTag("sidebar_stream_video");
        this.ParentWindow.mSidebar.UpdateImage("sidebar_stream_video", "sidebar_stream_video");
        elementFromTag.Image.Width = 24.0;
        elementFromTag.Image.Height = 24.0;
        this.ParentWindow.mNCTopBar.ChangeTopBarColor("TopBarColor");
        this.ParentWindow.mNCTopBar.mStreamingTopbarGrid.Visibility = Visibility.Collapsed;
        this.ParentWindow.mIsStreaming = false;
      }
    }

    internal static void ArrangeWindow()
    {
      if (RegistryManager.Instance.ArrangeWindowMode == 0)
        CommonHandlers.ArrangeWindowInTiles();
      else
        CommonHandlers.ArrangeWindowInCascade();
    }

    internal void MuteUnmuteButtonHandler()
    {
      if (this.ParentWindow.IsMuted)
        this.ParentWindow.Utils.UnmuteApplication();
      else
        this.ParentWindow.Utils.MuteApplication();
    }

    internal static string GetMacroName(string baseSchemeName = "Macro")
    {
      int length = baseSchemeName.Length;
      int num = 1;
      while (true)
      {
        if (MacroGraph.Instance.Vertices.Cast<MacroRecording>().Select<MacroRecording, string>((Func<MacroRecording, string>) (macro => macro.Name.ToLower(CultureInfo.InvariantCulture))).Contains<string>(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} ({1})", (object) baseSchemeName.ToLower(CultureInfo.InvariantCulture), (object) num).Trim()))
          ++num;
        else
          break;
      }
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} ({1})", (object) baseSchemeName, (object) num);
    }

    internal void MouseMoveOverFrontend()
    {
      if (!KMManager.sIsInScriptEditingMode || this.ParentWindow.mIsWindowInFocus)
        return;
      Logger.Info("Script focused");
      this.ParentWindow.mFrontendHandler.FocusFrontend();
    }

    protected virtual void Dispose(bool disposing)
    {
      if (this.disposedValue)
        return;
      int num = disposing ? 1 : 0;
      if (this.mObsResponseTimeoutTimer != null)
      {
        this.mObsResponseTimeoutTimer.Elapsed -= new ElapsedEventHandler(this.ObsResponseTimeoutTimer_Elapsed);
        this.mObsResponseTimeoutTimer.Dispose();
      }
      this.disposedValue = true;
    }

    ~CommonHandlers()
    {
      this.Dispose(false);
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    internal static void ReloadMacroShortcutsForAllInstances()
    {
      foreach (string runningInstances in Utils.GetRunningInstancesList())
      {
        if (BlueStacksUIUtils.DictWindows.ContainsKey(runningInstances))
          HTTPUtils.SendRequestToEngineAsync("updateMacroShortcutsDict", MainWindow.sMacroMapping, runningInstances, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
      }
    }

    internal void GameGuideButtonHandler(
      string action,
      string location,
      bool fromControlsMenuPopup = false)
    {
      if (!this.ToggleGamepadAndKeyboardGuidance("default", fromControlsMenuPopup))
      {
        KMManager.HandleInputMapperWindow(this.ParentWindow, "");
        string packageName = this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName;
      }
      ClientStats.SendMiscellaneousStatsAsync(location, RegistryManager.Instance.UserGuid, "GameGuide", action, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
    }

    internal static string GetCompleteMacroRecordingPath(string macroName)
    {
      return Path.Combine(RegistryStrings.MacroRecordingsFolderPath, macroName.ToLower(CultureInfo.InvariantCulture) + ".json");
    }

    internal bool ToggleGamepadAndKeyboardGuidance(string selectedTab, bool fromControlsMenuPopup = false)
    {
      if (!KMManager.CheckIfKeymappingWindowVisible(true))
        return false;
      if (KMManager.sGuidanceWindow != null)
      {
        if (fromControlsMenuPopup)
          return true;
        GuidanceWindow.HideOnNextLaunch(true);
        KMManager.sGuidanceWindow.mIsOnboardingPopupToBeShownOnGuidanceClose = true;
        KMManager.CloseWindows();
        this.ParentWindow.mSidebar.UpdateImage("sidebar_gameguide", "sidebar_gameguide");
        this.ParentWindow.StaticComponents.mSelectedTabButton.mGuidanceWindowOpen = false;
      }
      else
      {
        string a = "default";
        if (KMManager.sGuidanceWindow.mIsGamePadTabSelected)
          a = "gamepad";
        if (!string.Equals(a, selectedTab, StringComparison.InvariantCultureIgnoreCase))
          KMManager.sGuidanceWindow.GuidanceWindowTabSelected(selectedTab);
      }
      return true;
    }

    internal void ToggleScrollOnEdgeMode(string enable)
    {
      this.ParentWindow.mFrontendHandler.SendFrontendRequestAsync("toggleScrollOnEdgeFeature", new Dictionary<string, string>()
      {
        {
          "isEnabled",
          enable
        }
      });
    }

    internal bool CheckNativeGamepadState(string packageName)
    {
      try
      {
        string str = JObject.Parse(HTTPUtils.SendRequestToGuest("checknativegamepadstatus", new Dictionary<string, string>()
        {
          {
            nameof (packageName),
            packageName
          }
        }, this.ParentWindow.mVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp"))["isEnabled"].ToString().Trim();
        Logger.Debug("NATIVE_GAMEPAD: isEnabled: " + str);
        if (str.Equals("true", StringComparison.InvariantCultureIgnoreCase))
          return true;
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in CheckNativeGampeadState: " + ex.ToString());
      }
      return false;
    }

    internal void ToggleIsMouseLocked(string locked)
    {
      this.ParentWindow.mFrontendHandler.SendFrontendRequestAsync("toggleIsMouseLocked", new Dictionary<string, string>()
      {
        {
          "isLocked",
          locked
        }
      });
    }

    internal void SwitchOverlayControls(bool state)
    {
      if (!this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.mDictGamepadEligibility.ContainsKey(this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName) || !this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.mDictGamepadEligibility[this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName] & state)
        return;
      this.ParentWindow.mGamepadOverlaySelectedDict[this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName] = state;
      if (state)
      {
        this.ParentWindow.mSidebar.mGamepadIconImage.ImageName = "gamepad_overlay_icon_click";
        this.ParentWindow.mSidebar.mKeyboardIconImage.ImageName = "keyboard_overlay_icon";
      }
      else
      {
        if (this.ParentWindow.mSidebar.mGamepadIconImage.IsEnabled)
          this.ParentWindow.mSidebar.mGamepadIconImage.ImageName = "gamepad_overlay_icon";
        this.ParentWindow.mSidebar.mKeyboardIconImage.ImageName = "keyboard_overlay_icon_click";
      }
      if (!RegistryManager.Instance.ShowKeyControlsOverlay)
        return;
      KMManager.ShowOverlayWindow(this.ParentWindow, true, true);
    }

    internal bool CheckIfGamepadOverlaySelectedForApp(string package)
    {
      return this.ParentWindow.mGamepadOverlaySelectedDict.ContainsKey(package) && this.ParentWindow.mGamepadOverlaySelectedDict[package];
    }

    internal void SetTouchSoundSettings()
    {
      HTTPUtils.SendRequestToGuestAsync("settouchsounds", new Dictionary<string, string>()
      {
        {
          "data",
          "{" + string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\"touchSound\":\"{0}\"", (object) (this.ParentWindow.EngineInstanceRegistry.TouchSoundEnabled ? "enable" : "disable")) + "}"
        }
      }, this.ParentWindow.mVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
    }

    public delegate void MacroBookmarkChanged(string fileName, bool wasBookmarked);

    public delegate void MacroSettingsChanged(MacroRecording record);

    public delegate void ShortcutKeysChanged(bool isEnabled);

    public delegate void ShortcutKeysRefresh();

    public delegate void MacroDeleted(string fileName);

    public delegate void OverlayStateChanged(bool isEnabled);

    public delegate void MacroButtonVisibilityChanged(bool isVisible);

    public delegate void OperationSyncButtonVisibilityChanged(bool isVisible);

    public delegate void OBSResponseTimeout();

    public delegate void ScreenRecorderStateTransitioning();

    public delegate void BTvDownloaderMinimized();

    public delegate void GamepadButtonVisibilityChanged(bool visibility);

    public delegate void ScreenRecordingStateChanged(bool isRecording);

    public delegate void VolumeChanged(int volumeLevel);

    public delegate void VolumeMuted(bool muted);

    public delegate void GameGuideButtonVisibilityChanged(bool visibility);

    public delegate void UtcConverterLoaded();

    public delegate void UtcConverterVisibilityChanged(bool visibility);
  }
}
