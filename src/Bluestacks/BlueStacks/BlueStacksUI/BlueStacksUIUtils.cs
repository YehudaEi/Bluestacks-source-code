// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.BlueStacksUIUtils
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using Microsoft.VisualBasic.Devices;
using Microsoft.WindowsAPICodePack.Taskbar;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace BlueStacks.BlueStacksUI
{
  internal class BlueStacksUIUtils : IDisposable
  {
    internal static object mLaunchPlaySyncObj = new object();
    internal static string sLoggedInImageName = "loggedin";
    internal static string sPremiumUserImageName = "premiumuser";
    internal static string sUserAccountPackageName = "com.uncube.account";
    internal static string sUserAccountActivityName = "com.bluestacks.account.activities.AccountActivity_";
    internal static string sAndroidSettingsPackageName = "com.android.settings";
    internal static string sAndroidAccountSettingsActivityName = "com.android.settings.BstAccountsSettings";
    internal static bool sStopStatSendingThread = false;
    internal static List<string> lstCreatingWindows = new List<string>();
    internal static Dictionary<string, MainWindow> DictWindows = new Dictionary<string, MainWindow>();
    internal static bool sIsSynchronizationActive = false;
    internal static List<string> sSelectedInstancesForSync = new List<string>();
    public static Dictionary<string, List<EventHandler>> BootEventsForMIManager = new Dictionary<string, List<EventHandler>>();
    public static List<string> sSyncInvolvedInstances = new List<string>();
    private static bool? isOglSupported = new bool?();
    private int mCurrentVolumeLevel = 33;
    internal System.Timers.Timer sBootCheckTimer = new System.Timers.Timer(360000.0);
    private MainWindow ParentWindow;
    public static MainWindow LastActivatedWindow;
    public static MainWindow ActivatedWindow;
    private bool disposedValue;

    public int CurrentVolumeLevel
    {
      get
      {
        return this.mCurrentVolumeLevel;
      }
      private set
      {
        this.mCurrentVolumeLevel = value > 0 ? (value < 100 ? value : 100) : 0;
        this.ParentWindow.EngineInstanceRegistry.Volume = this.mCurrentVolumeLevel;
        this.ParentWindow.mCommonHandler.OnVolumeChanged(this.mCurrentVolumeLevel);
      }
    }

    internal static bool IsAlphabet(char c)
    {
      if (c >= 'A' && c <= 'Z')
        return true;
      return c >= 'a' && c <= 'z';
    }

    internal BlueStacksUIUtils(MainWindow window)
    {
      this.ParentWindow = window;
      this.mCurrentVolumeLevel = this.ParentWindow.EngineInstanceRegistry.Volume;
    }

    internal static void CloseContainerWindow(FrameworkElement control)
    {
      FrameworkElement frameworkElement = control;
      while (true)
      {
        switch (frameworkElement)
        {
          case null:
          case ContainerWindow _:
            goto label_3;
          default:
            frameworkElement = frameworkElement.Parent as FrameworkElement;
            continue;
        }
      }
label_3:
      if (frameworkElement == null)
        return;
      (frameworkElement as ContainerWindow).Close();
    }

    internal static void RefreshKeyMap(string packageName, MainWindow window = null)
    {
      foreach (KeyValuePair<string, MainWindow> dictWindow in BlueStacksUIUtils.DictWindows)
      {
        KeyValuePair<string, MainWindow> item = dictWindow;
        try
        {
          if (string.Equals(item.Value.mTopBar.mAppTabButtons.SelectedTab.PackageName, packageName, StringComparison.InvariantCulture))
          {
            item.Value.mFrontendHandler.RefreshKeyMap(packageName);
            if (RegistryManager.Instance.ShowKeyControlsOverlay)
            {
              if (window == null)
                KMManager.LoadIMActions(item.Value, packageName);
              else if (window.GetHashCode() != item.Value.GetHashCode())
                KMManager.LoadIMActions(item.Value, packageName);
              Dispatcher.CurrentDispatcher.BeginInvoke((Delegate) (() =>
              {
                if (KMManager.CanvasWindow != null && KMManager.CanvasWindow.IsVisible && KMManager.CanvasWindow.Owner.GetHashCode() == item.Value.GetHashCode() || (!KMManager.dictOverlayWindow.ContainsKey(item.Value) || BlueStacksUIUtils.LastActivatedWindow == item.Value))
                  return;
                KMManager.dictOverlayWindow[item.Value].Init();
              }));
            }
          }
        }
        catch (Exception ex)
        {
          Logger.Error(ex.ToString() + Environment.NewLine + "Exception refreshing mapping of package : " + packageName + " for instance : " + item.Value.mVmName);
        }
      }
    }

    public static bool IsModal(Window window)
    {
      return (bool) typeof (Window).GetField("_showingAsDialog", BindingFlags.Instance | BindingFlags.NonPublic).GetValue((object) window);
    }

    private static void CloseWindows(Window win)
    {
      for (int index = win.OwnedWindows.Count - 1; index >= 0; --index)
      {
        BlueStacksUIUtils.CloseWindows(win.OwnedWindows[index]);
        if (win.OwnedWindows[index] != null && win.OwnedWindows[index].IsLoaded && win.OwnedWindows[index].Visibility == Visibility.Visible)
        {
          if (BlueStacksUIUtils.IsModal(win.OwnedWindows[index]))
            win.OwnedWindows[index].Close();
          else
            win.OwnedWindows[index].Visibility = Visibility.Hidden;
        }
      }
    }

    internal static void HideUnhideBlueStacks(bool isHide)
    {
      foreach (MainWindow window in BlueStacksUIUtils.DictWindows.Values)
      {
        if (!window.mIsMinimizedThroughCloseButton)
          BlueStacksUIUtils.HideUnhideParentWindow(isHide, window);
      }
    }

    internal static void HideUnhideParentWindow(bool isHide, MainWindow window)
    {
      if (isHide)
      {
        window.Dispatcher.Invoke((Delegate) (() =>
        {
          BlueStacksUIUtils.CloseWindows((Window) window);
          window.WindowState = WindowState.Minimized;
          window.Hide();
          window.ShowInTaskbar = false;
        }));
      }
      else
      {
        window.ShowInTaskbar = true;
        window.ShowActivated = true;
        window.Show();
        if (window.mIsFullScreen || window.mIsMaximized)
        {
          window.WindowState = WindowState.Maximized;
          window.MaximizeWindow();
        }
        else
          window.WindowState = WindowState.Normal;
        if (!window.Topmost)
        {
          window.Topmost = true;
          ThreadPool.QueueUserWorkItem((WaitCallback) (obj => window.Dispatcher.Invoke((Delegate) (() => window.Topmost = false))));
        }
        if (!RegistryManager.Instance.ShowKeyControlsOverlay)
          return;
        KMManager.ShowOverlayWindow(window, true, false);
      }
    }

    public static void SetWindowTaskbarIcon(MainWindow window)
    {
      try
      {
        using (Bitmap originalIcon = new Bitmap(RegistryStrings.ProductIconCompletePath))
        {
          Uri uri = new Uri(RegistryStrings.ProductIconCompletePath);
          if (GenericNotificationManager.GetNotificationItems((Predicate<GenericNotificationItem>) (x =>
          {
            if (x.IsDeleted || x.IsRead)
              return false;
            return string.Equals(x.VmName, window.mVmName, StringComparison.InvariantCulture) || !x.IsAndroidNotification;
          })).Count <= 0 || !window.IsInNotificationMode)
            return;
          if (window.DummyWindow == null)
          {
            MainWindow mainWindow = window;
            DummyTaskbarWindow dummyTaskbarWindow = new DummyTaskbarWindow(window);
            dummyTaskbarWindow.Icon = (ImageSource) new BitmapImage(new Uri(Path.Combine(RegistryManager.Instance.ClientInstallDir, Path.Combine("Assets", "ProductLogo.ico"))));
            dummyTaskbarWindow.Title = BlueStacks.Common.Strings.ProductDisplayName;
            dummyTaskbarWindow.TaskbarThumbnailPath = Path.Combine(CustomPictureBox.AssetsDir, "PreviewThumbnail.png");
            dummyTaskbarWindow.WindowState = WindowState.Minimized;
            mainWindow.DummyWindow = dummyTaskbarWindow;
            window.DummyWindow.StateChanged -= new EventHandler(BlueStacksUIUtils.DummyWindow_StateChanged);
            window.DummyWindow.StateChanged += new EventHandler(BlueStacksUIUtils.DummyWindow_StateChanged);
            window.DummyWindow.Show();
          }
          BlueStacksUIUtils.AddIconOverlay((Window) window.DummyWindow, originalIcon, window.mVmName);
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Exception while setting taskbar icon " + ex?.ToString());
      }
    }

    private static void DummyWindow_StateChanged(object sender, EventArgs e)
    {
      if ((sender is DummyTaskbarWindow dummyTaskbarWindow ? (dummyTaskbarWindow.WindowState != WindowState.Minimized ? 1 : 0) : 1) == 0)
        return;
      BlueStacksUIUtils.HideUnhideParentWindow(false, (sender as DummyTaskbarWindow).ParentWindow);
      BlueStacks.Common.Stats.SendCommonClientStatsAsync("notification_mode", "taskbar_bluestacksicon_clicked", sender is DummyTaskbarWindow dummyTaskbarWindow ? dummyTaskbarWindow.ParentWindow?.mVmName : (string) null, "", "", "", "");
      if (!(sender is DummyTaskbarWindow dummyTaskbarWindow))
        return;
      dummyTaskbarWindow.Close();
    }

    public static void AddIconOverlay(Window window, Bitmap originalIcon, string vmName)
    {
      try
      {
        SerializableDictionary<string, GenericNotificationItem> notificationItems = GenericNotificationManager.GetNotificationItems((Predicate<GenericNotificationItem>) (x =>
        {
          if (x.IsDeleted || x.IsRead)
            return false;
          return string.Equals(x.VmName, vmName, StringComparison.InvariantCulture) || !x.IsAndroidNotification;
        }));
        string str = notificationItems.Count.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        if (notificationItems.Count > 99)
          str = "99+";
        using (Bitmap bitmap1 = new Bitmap(256, 256))
        {
          using (Graphics graphics1 = Graphics.FromImage((System.Drawing.Image) bitmap1))
          {
            graphics1.SmoothingMode = SmoothingMode.AntiAlias;
            int width = (str.Length + 1) * 45 + 10;
            int num1 = 256 - width + 5;
            if (width < 120)
            {
              num1 = 206 - width / 2;
              width = 120;
            }
            Rectangle rectangle1 = new Rectangle(256 - width, 111, width, 120);
            int num2 = 120;
            System.Drawing.Size size1 = new System.Drawing.Size(num2, num2);
            Rectangle rect1 = new Rectangle(rectangle1.Location, size1);
            using (GraphicsPath path1 = new GraphicsPath())
            {
              path1.AddArc(rect1, 180f, 90f);
              rect1.X = rectangle1.Right - num2;
              path1.AddArc(rect1, 270f, 90f);
              rect1.Y = rectangle1.Bottom - num2;
              path1.AddArc(rect1, 0.0f, 90f);
              rect1.X = rectangle1.Left;
              path1.AddArc(rect1, 90f, 90f);
              path1.CloseFigure();
              graphics1.FillPath(System.Drawing.Brushes.OrangeRed, path1);
              System.Drawing.Image image1 = (System.Drawing.Image) originalIcon;
              System.Drawing.Image image2 = (System.Drawing.Image) bitmap1;
              using (Bitmap bitmap2 = new Bitmap(256, 256))
              {
                Graphics graphics2 = Graphics.FromImage((System.Drawing.Image) bitmap2);
                Rectangle rect2 = new Rectangle(0, 0, 256, 256);
                graphics2.DrawImage(image1, rect2);
                graphics2.DrawImage(image2, new System.Drawing.Point(0, 0));
                using (Font font1 = new Font("Arial", (float) (70.0 / MainWindow.sScalingFactor), System.Drawing.FontStyle.Regular))
                {
                  graphics2.DrawString(str, font1, System.Drawing.Brushes.White, (float) num1, 117f);
                  graphics2.Save();
                  window.Icon = (ImageSource) System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(bitmap2.GetHicon(), Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                  using (Bitmap bitmap3 = new Bitmap(256, 256))
                  {
                    using (Graphics graphics3 = Graphics.FromImage((System.Drawing.Image) bitmap3))
                    {
                      graphics3.SmoothingMode = SmoothingMode.AntiAlias;
                      Rectangle rectangle2 = new Rectangle(0, 0, 256, 256);
                      int num3 = 256;
                      System.Drawing.Size size2 = new System.Drawing.Size(num3, num3);
                      Rectangle rect3 = new Rectangle(rectangle2.Location, size2);
                      using (GraphicsPath path2 = new GraphicsPath())
                      {
                        path2.AddArc(rect3, 180f, 90f);
                        rect3.X = rectangle2.Right - num3;
                        path2.AddArc(rect3, 270f, 90f);
                        rect3.Y = rectangle2.Bottom - num3;
                        path2.AddArc(rect3, 0.0f, 90f);
                        rect3.X = rectangle2.Left;
                        path2.AddArc(rect3, 90f, 90f);
                        path2.CloseFigure();
                        graphics3.FillPath(System.Drawing.Brushes.OrangeRed, path2);
                        int num4 = 175 - (str.Length - 1) * 35;
                        int num5 = 10 + (str.Length - 1) * 22;
                        int num6 = -5;
                        if (str.Length == 1)
                          num6 = 35;
                        if (num4 > 150)
                        {
                          num4 = 150;
                          num5 += 14;
                        }
                        using (Font font2 = new Font("Arial", (float) num4 / (float) MainWindow.sScalingFactor, System.Drawing.FontStyle.Regular))
                        {
                          graphics3.DrawString(str, font2, System.Drawing.Brushes.White, (float) num6, (float) num5);
                          TaskbarManager.Instance.SetOverlayIcon(window, Icon.FromHandle(bitmap3.GetHicon()), str);
                        }
                      }
                    }
                  }
                }
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Info("error" + ex?.ToString());
      }
    }

    internal static void SendMuteUnmuteRequestToAllInstances(bool isMute)
    {
      RegistryManager.Instance.AreAllInstancesMuted = isMute;
      foreach (string vm in RegistryManager.Instance.VmList)
      {
        if (BlueStacksUIUtils.DictWindows.Keys.Contains<string>(vm))
        {
          if (isMute)
            BlueStacksUIUtils.DictWindows[vm].Utils.MuteApplication();
          else
            BlueStacksUIUtils.DictWindows[vm].Utils.UnmuteApplication();
        }
      }
    }

    internal void MuteApplication()
    {
      NativeMethods.waveOutSetVolume(IntPtr.Zero, 0U);
      try
      {
        if (this.ParentWindow.mFrontendGrid.IsVisible)
          HTTPUtils.SendRequestToEngineAsync("mute", new Dictionary<string, string>()
          {
            ["explicit"] = "true"
          }, this.ParentWindow.mVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
        this.ParentWindow.EngineInstanceRegistry.IsMuted = true;
        this.ParentWindow.mCommonHandler.OnVolumeMuted(true);
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to send mute to frontend. Ex: " + ex.Message);
      }
    }

    internal void UnmuteApplication()
    {
      if (!Oem.IsOEMDmm && RegistryManager.Instance.AreAllInstancesMuted)
      {
        RegistryManager.Instance.AreAllInstancesMuted = false;
        foreach (MainWindow mainWindow in BlueStacksUIUtils.DictWindows.Values)
          mainWindow.mSidebar.UpdateMuteAllInstancesCheckbox();
      }
      NativeMethods.waveOutSetVolume(IntPtr.Zero, uint.MaxValue);
      try
      {
        if (this.ParentWindow.mFrontendGrid.IsVisible)
          HTTPUtils.SendRequestToEngineAsync("unmute", (Dictionary<string, string>) null, this.ParentWindow.mVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
        this.ParentWindow.EngineInstanceRegistry.IsMuted = false;
        this.ParentWindow.mCommonHandler.OnVolumeMuted(false);
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to send mute to frontend. Ex: " + ex.Message);
      }
    }

    internal void SetCurrentVolumeForDMM(int previousVolume, int newVolume)
    {
      new Thread((ThreadStart) (() =>
      {
        try
        {
          this.ParentWindow.Utils.SetVolumeInFrontendAsync(newVolume);
          this.ParentWindow.Dispatcher.Invoke((Delegate) (() => this.ParentWindow.mDmmBottomBar.CurrentVolume = previousVolume));
        }
        catch (Exception ex)
        {
          Logger.Error("Failed to set volume... Err : " + ex.ToString());
          this.ParentWindow.Dispatcher.Invoke((Delegate) (() => this.ParentWindow.mDmmBottomBar.CurrentVolume = previousVolume));
        }
      }))
      {
        IsBackground = true
      }.Start();
    }

    internal void SetVolumeLevelFromAndroid(int volume)
    {
      this.CurrentVolumeLevel = volume;
    }

    internal void SetVolumeInFrontendAsync(int newVolume)
    {
      int currentVolumeLevel = this.CurrentVolumeLevel;
      new Thread((ThreadStart) (() =>
      {
        try
        {
          if (this.ParentWindow.mGuestBootCompleted)
          {
            if (Convert.ToBoolean((object) JArray.Parse(HTTPUtils.SendRequestToEngine("setVolume", new Dictionary<string, string>()
            {
              {
                "vol",
                newVolume.ToString((IFormatProvider) CultureInfo.InvariantCulture)
              }
            }, this.ParentWindow.mVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "", "bgp"))[0][(object) "success"], (IFormatProvider) CultureInfo.InvariantCulture))
              this.CurrentVolumeLevel = newVolume;
          }
        }
        catch (Exception ex)
        {
          Logger.Error("Failed to set volume. Ex: " + ex.ToString());
        }
        if (this.CurrentVolumeLevel == 0)
          this.MuteApplication();
        if (!this.ParentWindow.IsMuted || this.CurrentVolumeLevel == 0)
          return;
        this.UnmuteApplication();
      }))
      {
        IsBackground = true
      }.Start();
    }

    internal void GetCurrentVolumeAtBootAsyncAndSetMuteInstancesState()
    {
      new Thread((ThreadStart) (() =>
      {
        try
        {
          int millisecondsTimeout = 1000;
          int num1 = this.mCurrentVolumeLevel;
          int num2 = 60;
          while (num2 > 0)
          {
            --num2;
            try
            {
              JObject jobject = JObject.Parse(HTTPUtils.SendRequestToGuest("getVolume", (Dictionary<string, string>) null, this.ParentWindow.mVmName, 0, (Dictionary<string, string>) null, true, 1, 0, "bgp"));
              if (jobject["result"].ToString() != "ok")
              {
                Thread.Sleep(millisecondsTimeout);
              }
              else
              {
                num1 = Convert.ToInt32(jobject["volume"].ToString(), (IFormatProvider) CultureInfo.InvariantCulture);
                break;
              }
            }
            catch (Exception ex)
            {
              Logger.Warning("Failed to get volume from guest: {0}", (object) ex.Message);
              Thread.Sleep(millisecondsTimeout);
            }
          }
          this.CurrentVolumeLevel = num1;
        }
        catch (Exception ex)
        {
          Logger.Error("Failed to get volume: " + ex.ToString());
        }
      }))
      {
        IsBackground = true
      }.Start();
    }

    internal static void RestartInstance(string vmName, bool isWaitForPlayerClosing = false)
    {
      if (!BlueStacksUIUtils.DictWindows.ContainsKey(vmName))
        return;
      BlueStacksUIUtils.DictWindows[vmName].RestartInstanceAndPerform((EventHandler) null, isWaitForPlayerClosing);
    }

    internal static void SwitchAndRestartInstanceInAgl(string vmName)
    {
      if (!BlueStacksUIUtils.DictWindows.ContainsKey(vmName))
        return;
      BlueStacksUIUtils.DictWindows[vmName].EngineInstanceRegistry.GlRenderMode = 1;
      Utils.UpdateValueInBootParams("GlMode", "2", vmName, true, "bgp");
      BlueStacksUIUtils.DictWindows[vmName].EngineInstanceRegistry.GlMode = 2;
      BlueStacksUIUtils.RestartInstance(vmName, false);
    }

    internal static void SwitchAndRestartInstanceInOglAfterRunningGlCheck(
      string vmName,
      System.Action openApp)
    {
      if (!BlueStacksUIUtils.DictWindows.ContainsKey(vmName))
        return;
      if (!BlueStacksUIUtils.isOglSupported.HasValue)
      {
        BlueStacksUIUtils.DictWindows[vmName].mExitProgressGrid.ProgressText = "STRING_RUNNING_CHECKS";
        BlueStacksUIUtils.DictWindows[vmName].mExitProgressGrid.Visibility = Visibility.Visible;
        using (BackgroundWorker backgroundWorker = new BackgroundWorker())
        {
          backgroundWorker.DoWork += new DoWorkEventHandler(BlueStacksUIUtils.Bgw_DoWork);
          backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BlueStacksUIUtils.Bgw_RunWorkerCompleted);
          backgroundWorker.RunWorkerAsync((object) new object[2]
          {
            (object) vmName,
            (object) openApp
          });
        }
      }
      else
        BlueStacksUIUtils.Bgw_RunWorkerCompleted(new object(), new RunWorkerCompletedEventArgs((object) new object[2]
        {
          (object) vmName,
          (object) openApp
        }, (Exception) null, false));
    }

    private static void Bgw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      if (e.Error != null)
        return;
      string vmName = ((object[]) e.Result)[0].ToString();
      System.Action openApp = (System.Action) ((object[]) e.Result)[1];
      BlueStacksUIUtils.DictWindows[vmName].mExitProgressGrid.ProgressText = "STRING_CLOSING_BLUESTACKS";
      BlueStacksUIUtils.DictWindows[vmName].mExitProgressGrid.Visibility = Visibility.Hidden;
      bool? isOglSupported = BlueStacksUIUtils.isOglSupported;
      bool flag = true;
      if (isOglSupported.GetValueOrDefault() == flag & isOglSupported.HasValue)
      {
        BlueStacksUIUtils.DictWindows[vmName].EngineInstanceRegistry.GlRenderMode = 1;
        Utils.UpdateValueInBootParams("GlMode", "1", vmName, true, "bgp");
        BlueStacksUIUtils.DictWindows[vmName].EngineInstanceRegistry.GlMode = 1;
        BlueStacksUIUtils.RestartInstance(vmName, false);
      }
      else
      {
        CustomMessageWindow customMessageWindow = new CustomMessageWindow();
        customMessageWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_OPENGL_NOT_SUPPORTED", "");
        customMessageWindow.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_OPENGL_NOTSUPPORTED_BODY", "");
        customMessageWindow.AddButton(ButtonColors.Blue, LocaleStrings.GetLocalizedString("STRING_CONTINUE", ""), (EventHandler) ((o, args) => openApp()), (string) null, false, (object) null, true);
        customMessageWindow.Owner = (Window) BlueStacksUIUtils.DictWindows[vmName];
        BlueStacksUIUtils.DictWindows[vmName].ShowDimOverlay((IDimOverlayControl) null);
        customMessageWindow.ShowDialog();
        BlueStacksUIUtils.DictWindows[vmName].HideDimOverlay();
      }
    }

    private static void Bgw_DoWork(object sender, DoWorkEventArgs e)
    {
      BlueStacksUIUtils.isOglSupported = new bool?(Utils.CheckOpenGlSupport(out int _, out string _, out string _, out string _, RegistryStrings.InstallDir));
      e.Result = e.Argument;
    }

    internal static void SwitchAndRestartInstanceInDx(string vmName)
    {
      if (!BlueStacksUIUtils.DictWindows.ContainsKey(vmName))
        return;
      BlueStacksUIUtils.DictWindows[vmName].EngineInstanceRegistry.GlRenderMode = 4;
      Utils.UpdateValueInBootParams("GlMode", "1", vmName, true, "bgp");
      BlueStacksUIUtils.DictWindows[vmName].EngineInstanceRegistry.GlMode = 1;
      BlueStacksUIUtils.RestartInstance(vmName, false);
    }

    internal static void SwitchAndRestartInstanceInAdx(string vmName)
    {
      if (!BlueStacksUIUtils.DictWindows.ContainsKey(vmName))
        return;
      BlueStacksUIUtils.DictWindows[vmName].EngineInstanceRegistry.GlRenderMode = 4;
      Utils.UpdateValueInBootParams("GlMode", "2", vmName, true, "bgp");
      BlueStacksUIUtils.DictWindows[vmName].EngineInstanceRegistry.GlMode = 2;
      BlueStacksUIUtils.RestartInstance(vmName, false);
    }

    internal void RunAppOrCreateTabButton(string packageName)
    {
      this.ParentWindow.Dispatcher.Invoke((Delegate) (() =>
      {
        if (this.ParentWindow.mTopBar.mAppTabButtons.mHomeAppTabButton.IsSelected)
        {
          this.ParentWindow.mAppHandler.SendRunAppRequestAsync(packageName, "", false);
        }
        else
        {
          AppIconModel appIcon = this.ParentWindow.mWelcomeTab.mHomeAppManager.GetAppIcon(packageName);
          if (appIcon == null)
            return;
          this.ParentWindow.mTopBar.mAppTabButtons.AddAppTab(appIcon.AppName, appIcon.PackageName, appIcon.ActivityName, appIcon.ImageName, false, false, false);
        }
      }));
    }

    internal void ResetPendingUIOperations()
    {
      try
      {
        if (!this.ParentWindow.mGuestBootCompleted)
          return;
        this.ParentWindow.mAppHandler.SwitchWhenPackageNameRecieved = string.Empty;
        AppHandler.EventOnAppDisplayed = (EventHandler<EventArgs>) null;
        if (Oem.IsOEMDmm)
        {
          this.ParentWindow.mDmmBottomBar.ShowKeyMapPopup(false);
        }
        else
        {
          this.ParentWindow.mSidebar.ShowKeyMapPopup(false);
          this.ParentWindow.mSidebar.ShowOverlayTooltip(false, false);
        }
        this.ParentWindow.mWelcomeTab.mFrontendPopupControl.HideWindow();
        this.ParentWindow.StaticComponents.ShowUninstallButtons(false);
        this.ParentWindow.ClosePopUps();
      }
      catch (Exception ex)
      {
        Logger.Info("Error in ResetPendingUIOperations " + ex.ToString());
      }
    }

    internal static void AddBootEventHandler(string vmName, EventHandler bootedEvennt)
    {
      if (BlueStacksUIUtils.BootEventsForMIManager.ContainsKey(vmName))
        BlueStacksUIUtils.BootEventsForMIManager[vmName].Add(bootedEvennt);
      else
        BlueStacksUIUtils.BootEventsForMIManager.Add(vmName, new List<EventHandler>()
        {
          bootedEvennt
        });
    }

    internal static void InvokeMIManagerEvents(string VmName)
    {
      if (!BlueStacksUIUtils.BootEventsForMIManager.ContainsKey(VmName))
        return;
      foreach (EventHandler eventHandler in BlueStacksUIUtils.BootEventsForMIManager[VmName])
        eventHandler((object) null, (EventArgs) null);
    }

    internal void ShakeWindow()
    {
      if (this.ParentWindow.WindowState == WindowState.Maximized)
      {
        this.ParentWindow.StoryBoard.Begin();
      }
      else
      {
        int num1 = 10;
        int num2 = 5;
        int num3 = 0;
        int num4 = 0;
        while (num1 > 0)
        {
          switch (num4)
          {
            case 0:
              num3 = num2;
              break;
            case 1:
              num3 = num2 * -1;
              break;
            case 2:
              num3 = num2 * -1;
              break;
            case 3:
              num3 = num2;
              break;
          }
          ++num4;
          if (num4 == 4)
          {
            num4 = 0;
            --num1;
          }
          this.ParentWindow.Left += (double) num3;
          Thread.Sleep(30);
        }
      }
    }

    internal static void RunInstance(string vmName, bool hiddenMode = false, string windowLaunchParams = "")
    {
      if (BlueStacksUIUtils.lstCreatingWindows.Contains(vmName))
        return;
      if (BlueStacksUIUtils.DictWindows.ContainsKey(vmName) && !hiddenMode)
        BlueStacksUIUtils.DictWindows[vmName].Dispatcher.Invoke((Delegate) (() => BlueStacksUIUtils.DictWindows[vmName].ShowWindow(false)));
      else
        System.Windows.Application.Current.Dispatcher.Invoke((Delegate) (() =>
        {
          BlueStacksUIUtils.lstCreatingWindows.Add(vmName);
          MainWindow mainWindow = new MainWindow(vmName, new FrontendHandler(vmName), windowLaunchParams, hiddenMode);
          BlueStacksUIUtils.DictWindows[vmName] = mainWindow;
          BlueStacksUIUtils.lstCreatingWindows.Remove(vmName);
          if (hiddenMode)
            return;
          mainWindow.ShowWindow(true);
        }));
    }

    internal void CheckGuestFailedAsync()
    {
      this.sBootCheckTimer.Elapsed += new ElapsedEventHandler(this.SBootCheckTimer_Elapsed);
      this.sBootCheckTimer.Enabled = true;
    }

    internal static void HideAllBlueStacks()
    {
      foreach (MainWindow mainWindow in BlueStacksUIUtils.DictWindows.Values.ToList<MainWindow>())
      {
        BlueStacksUIUtils.CloseWindows((Window) mainWindow);
        mainWindow.ShowInTaskbar = false;
        mainWindow.Hide();
      }
    }

    private void SBootCheckTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
      (sender as System.Timers.Timer).Enabled = false;
      if (this.ParentWindow.mGuestBootCompleted)
        return;
      this.SendGuestBootFailureStats("boot timeout exception");
    }

    public static string GetFinalRedirectedUrl(string url)
    {
      HttpWebRequest httpWebRequest = (HttpWebRequest) WebRequest.Create(url);
      httpWebRequest.Method = "GET";
      httpWebRequest.AllowAutoRedirect = true;
      string str1 = "Bluestacks/" + RegistryManager.Instance.ClientVersion;
      httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/49.0.2623.110 Safari/537.36 " + str1;
      httpWebRequest.Headers.Add("x_oem", RegistryManager.Instance.Oem);
      httpWebRequest.Headers.Set("x_email", RegistryManager.Instance.RegisteredEmail);
      httpWebRequest.Headers.Add("x_guid", RegistryManager.Instance.UserGuid);
      httpWebRequest.Headers.Add("x_prod_ver", RegistryManager.Instance.Version);
      httpWebRequest.Headers.Add("x_home_app_ver", RegistryManager.Instance.ClientVersion);
      try
      {
        string str2 = (string) null;
        using (WebResponse response = httpWebRequest.GetResponse())
          str2 = response.ResponseUri.ToString();
        return str2;
      }
      catch (Exception ex)
      {
        Logger.Error("Error in getting redirected url" + ex.ToString());
        return (string) null;
      }
    }

    internal void SendGuestBootFailureStats(string errorString)
    {
      if (RegistryManager.Instance.IsEngineUpgraded == 1 && RegistryManager.Instance.IsClientFirstLaunch == 1)
        ClientStats.SendClientStatsAsync("update_init", "fail", "engine_activity", "", errorString, this.ParentWindow.mVmName);
      else if (RegistryManager.Instance.IsClientFirstLaunch == 1)
        ClientStats.SendClientStatsAsync("first_init", "fail", "engine_activity", "", errorString, this.ParentWindow.mVmName);
      else
        ClientStats.SendClientStatsAsync("init", "fail", "engine_activity", "", errorString, this.ParentWindow.mVmName);
      this.ParentWindow.HandleRestartPopup();
    }

    internal static bool CheckForMacrAvailable(string packageName)
    {
      string path1 = Path.Combine(Path.Combine(RegistryManager.Instance.EngineDataDir, "UserData\\InputMapper"), packageName + "_macro.cfg");
      string path2 = Path.Combine(Path.Combine(RegistryManager.Instance.EngineDataDir, "UserData\\InputMapper\\UserFiles"), packageName + "_macro.cfg");
      return System.IO.File.Exists(path1) || System.IO.File.Exists(path2);
    }

    internal static string GetVideoTutorialUrl(
      string packageName,
      string videoMode,
      string selectedSchemeName)
    {
      string serverHost = WebHelper.GetServerHost();
      string str1 = serverHost.Substring(0, serverHost.Length - 4);
      string urlWithParams;
      if (GuidanceVideoType.SchemeSpecific.ToString().ToLower(CultureInfo.InvariantCulture).Equals(videoMode, StringComparison.InvariantCulture))
        urlWithParams = WebHelper.GetUrlWithParams(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}?app_pkg={2}&mode={3}&scheme={4}", (object) str1, (object) "videoTutorial", (object) packageName, (object) videoMode, (object) selectedSchemeName), (string) null, (string) null, (string) null);
      else
        urlWithParams = WebHelper.GetUrlWithParams(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}?app_pkg={2}&mode={3}", (object) str1, (object) "videoTutorial", (object) packageName, (object) videoMode), (string) null, (string) null, (string) null);
      string str2;
      if (!RegistryManager.Instance.IgnoreAutoPlayPackageList.Contains(packageName))
      {
        str2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}&autoplay=1", (object) urlWithParams);
        List<string> autoPlayPackageList = RegistryManager.Instance.IgnoreAutoPlayPackageList;
        autoPlayPackageList.Add(packageName);
        RegistryManager.Instance.IgnoreAutoPlayPackageList = autoPlayPackageList;
      }
      else
        str2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}&autoplay=0", (object) urlWithParams);
      if (!string.IsNullOrEmpty(RegistryManager.Instance.Partner))
        str2 += string.Format((IFormatProvider) CultureInfo.InvariantCulture, "&partner={0}", (object) RegistryManager.Instance.Partner);
      return str2;
    }

    internal static string GetOnboardingUrl(string packageName, string source)
    {
      return WebHelper.GetUrlWithParams(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}?app_pkg={2}&source={3}", (object) RegistryManager.Instance.Host, (object) "bs3/page/onboarding-tutorial", (object) packageName, (object) source), (string) null, (string) null, (string) null);
    }

    internal static string GetGameFeaturePopupUrl(string packageName, string gamePopupId)
    {
      return WebHelper.GetUrlWithParams(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}?app_pkg={2}&game_popup_id={3}", (object) RegistryManager.Instance.Host, (object) "bs3/page/game-feature-tutorial", (object) packageName, (object) gamePopupId), (string) null, (string) null, (string) null);
    }

    internal static string GetUtcConverterUrl(string packageName)
    {
      return WebHelper.GetUrlWithParams(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}?app_pkg={2}", (object) RegistryManager.Instance.Host, (object) "bs3/page/utcConverter", (object) packageName), (string) null, (string) null, (string) null);
    }

    internal void SetKeyMapping(string packageName, string source)
    {
      string path1 = Path.Combine(RegistryManager.Instance.EngineDataDir, "UserData\\InputMapper\\UserFiles");
      string destFileName = Path.Combine(path1, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.cfg", (object) packageName));
      string sourceFileName = Path.Combine(path1, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_{1}.cfg", (object) packageName, (object) source));
      try
      {
        System.IO.File.Copy(sourceFileName, destFileName, true);
      }
      catch (Exception ex)
      {
        Logger.Error("Faield to copy cfgs... Err : " + ex.ToString());
        return;
      }
      this.ParentWindow.mFrontendHandler.RefreshKeyMap(packageName);
    }

    internal static void OpenUrl(string url)
    {
      try
      {
        Process.Start(url);
      }
      catch (Win32Exception ex1)
      {
        try
        {
          Process.Start("IExplore.exe", url);
        }
        catch (Exception ex2)
        {
          Logger.Warning("Not able to launch the url " + url + "Ignoring Exception: " + ex2.ToString());
        }
      }
      catch (Exception ex)
      {
        Logger.Warning("Not able to launch the url " + url + "Ignoring Exception: " + ex.ToString());
      }
    }

    internal bool IsSufficientRAMAvailable()
    {
      Logger.Info("Checking for physical memory...");
      return long.Parse(SystemUtils.GetSysInfo("Select TotalPhysicalMemory from Win32_ComputerSystem"), (IFormatProvider) CultureInfo.InvariantCulture) >= 1073741824L;
    }

    public void SendMessageToAndroidForAffiliate(string pkgName, string source)
    {
      try
      {
        Logger.Info("Sending message to Android for affiliate");
        Dictionary<string, string> data = new Dictionary<string, string>()
        {
          {
            "action",
            "com.bluestacks.home.AFFILIATE_HANDLER_HTML"
          }
        };
        JObject jobject = new JObject()
        {
          {
            "success",
            (JToken) true
          },
          {
            "app_pkg",
            (JToken) pkgName
          },
          {
            "WINDOWS_SOURCE",
            (JToken) source
          }
        };
        data.Add("extras", jobject.ToString(Formatting.None));
        HTTPUtils.SendRequestToGuest("customStartService", data, this.ParentWindow.mVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
      }
      catch (Exception ex)
      {
        Logger.Error("Couldn't send message to Adnroid: " + ex.ToString());
      }
    }

    public void AppendUrlWithCommonParamsAndOpenTab(
      string url,
      string title,
      string imagePath,
      string tabKey = "")
    {
      try
      {
        url = WebHelper.GetUrlWithParams(url, (string) null, (string) null, (string) null);
        if (new Uri(url).Host.Contains("bluestacks", StringComparison.InvariantCultureIgnoreCase))
        {
          string registeredEmail = RegistryManager.Instance.RegisteredEmail;
          string token = RegistryManager.Instance.Token;
          if (string.IsNullOrEmpty(registeredEmail))
            Logger.Warning("User email not found. Not opening webpage.");
          if (string.IsNullOrEmpty(token))
            Logger.Warning("User token not found. Not opening webpage.");
        }
        this.ParentWindow.Dispatcher.Invoke((Delegate) (() => this.ParentWindow.mTopBar.mAppTabButtons.AddWebTab(url, title, imagePath, true, tabKey, false)));
      }
      catch (Exception ex)
      {
        Logger.Error("Exception when parsing uri for opening in webtab " + ex.ToString());
      }
    }

    public void ApplyTheme(string themeName)
    {
      BlueStacksUIColorManager.ReloadAppliedTheme(themeName);
      Publisher.PublishMessage(BrowserControlTags.themeChange, this.ParentWindow.mVmName, new JObject((object) new JProperty("Theme", (object) themeName)));
      BlueStacksUIUtils.RefreshAppCenterUrl();
      BlueStacksUIUtils.RefreshHtmlSidePanelUrl();
      this.ParentWindow.mCommonHandler.SetCustomCursorForApp(this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName);
      ClientStats.SendMiscellaneousStatsAsync("SkinChangedStats", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.ClientThemeName, (string) null, (string) null, (string) null, (string) null, "Android");
    }

    public void RestoreWallpaperImageForAllVms()
    {
      foreach (MainWindow mainWindow in BlueStacksUIUtils.DictWindows.Values.ToList<MainWindow>())
        mainWindow.mWelcomeTab.mHomeAppManager.RestoreWallpaper();
    }

    public void ChooseWallpaper()
    {
      try
      {
        OpenFileDialog openFileDialog1 = new OpenFileDialog();
        openFileDialog1.Title = LocaleStrings.GetLocalizedString("STRING_CHANGE_WALLPAPER", "");
        openFileDialog1.RestoreDirectory = true;
        openFileDialog1.DefaultExt = ".jpg";
        openFileDialog1.Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png";
        using (OpenFileDialog openFileDialog2 = openFileDialog1)
        {
          if (openFileDialog2.ShowDialog() != DialogResult.OK)
            return;
          Bitmap bitmap = new Bitmap(openFileDialog2.FileName);
          bitmap.Save(HomeAppManager.BackgroundImagePath);
          bitmap.Dispose();
          foreach (MainWindow mainWindow in BlueStacksUIUtils.DictWindows.Values.ToList<MainWindow>())
            mainWindow.mWelcomeTab.mHomeAppManager.ApplyWallpaper();
          ClientStats.SendMiscellaneousStatsAsync("WallPaperStats", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "Premium", "Changed_Wallpaper", (string) null, (string) null, (string) null, (string) null, "Android");
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in changing wallpaper:" + ex.ToString());
        int num = (int) System.Windows.MessageBox.Show("Cannot change wallpaper.Please try again.", "Error");
      }
    }

    internal static string GetAppCenterUrl(string tabId)
    {
      string str = WebHelper.GetUrlWithParams(WebHelper.GetServerHost() + "/page/appcenter-v2", (string) null, (string) null, (string) null) + "&theme=" + RegistryManager.ClientThemeName + "&naked=1";
      if (!string.IsNullOrEmpty(tabId))
        str = str + "&tabid=" + tabId;
      return str;
    }

    internal static string GetHtmlSidePanelUrl()
    {
      return WebHelper.GetUrlWithParams(WebHelper.GetServerHost() + "/page/myapps-sidepanel", (string) null, (string) null, (string) null) + "&theme=" + RegistryManager.ClientThemeName;
    }

    internal string GetHtmlHomeUrl(bool isRefresh)
    {
      string str = WebHelper.GetUrlWithParams(WebHelper.GetServerHost() + "/page/bgp-home-html", (string) null, (string) null, (string) null) + "&theme=" + RegistryManager.ClientThemeName + "&partner=msi2&vmId=" + Utils.GetVmIdFromVmName(this.ParentWindow.mVmName) + "&country=msi2&vmName=" + this.ParentWindow.mVmName + "#Library#&firstLaunchedVmName=" + BlueStacks.Common.Strings.CurrentDefaultVmName + "&oem=" + RegistryManager.Instance.Oem;
      if (!isRefresh && !string.IsNullOrEmpty(this.ParentWindow.WindowLaunchParams))
      {
        JObject jobject = JObject.Parse(this.ParentWindow.WindowLaunchParams);
        if (jobject["fle_pkg"] != null)
          str = str + "&flePackageName=" + jobject["fle_pkg"].ToString().Trim();
        if (jobject["check_fle_pkg"] != null)
          str = str + "&checkFlePkg=" + jobject["check_fle_pkg"].ToString().Trim();
        if (jobject["campaign_id"] != null)
          str = str + "&campaignId=" + jobject["campaign_id"].ToString().Trim();
        if (jobject["source"] != null)
          str = str + "&source=" + jobject["source"].ToString().Trim();
      }
      return str.Replace("&oem=bgp", "&oem=msi2");
    }

    internal void RefreshHtmlHomeUrl()
    {
      try
      {
        this.ParentWindow.mWelcomeTab.ReInitHtmlHome();
      }
      catch (Exception ex)
      {
        Logger.Error("Error while refreshing side html panel for vmname: {0} and exception is: {1}", (object) this.ParentWindow.mVmName, (object) ex);
      }
    }

    internal static string GetGiftTabUrl()
    {
      return WebHelper.GetUrlWithParams(WebHelper.GetServerHost() + "/gift", (string) null, (string) null, (string) null) + "&theme=" + RegistryManager.ClientThemeName;
    }

    internal static string GetPikaWorldUrl()
    {
      return WebHelper.GetUrlWithParams(WebHelper.GetServerHost() + "/pikaworld", (string) null, (string) null, (string) null) + "&naked=1";
    }

    internal void SwitchProfile(string vmName, string pcode)
    {
      try
      {
        Dictionary<string, string> data = new Dictionary<string, string>();
        JObject jobject1 = new JObject()
        {
          {
            nameof (pcode),
            (JToken) pcode
          },
          {
            "createcustomprofile",
            (JToken) "false"
          }
        };
        data.Add("data", jobject1.ToString(Formatting.None));
        BlueStacksUIUtils.DictWindows[vmName].mExitProgressGrid.ProgressText = "STRING_SWITCHING_PROFILE";
        BlueStacksUIUtils.DictWindows[vmName].mExitProgressGrid.Visibility = Visibility.Visible;
        ThreadPool.QueueUserWorkItem((WaitCallback) (obj =>
        {
          JObject jobject1 = new JObject()
          {
            [nameof (pcode)] = (JToken) Utils.GetValueInBootParams(nameof (pcode), this.ParentWindow.mVmName, "", "bgp")
          };
          string guest = HTTPUtils.SendRequestToGuest("changeDeviceProfile", data, vmName, 0, (Dictionary<string, string>) null, false, 3, 60000, "bgp");
          Logger.Info("Response for ChangeDeviceProfile: " + guest);
          JObject jobject2 = JObject.Parse(guest);
          this.ParentWindow.Dispatcher.Invoke((Delegate) (closure_0 ?? (closure_0 = (System.Action) (() =>
          {
            this.ParentWindow.mExitProgressGrid.ProgressText = "STRING_CLOSING_BLUESTACKS";
            this.ParentWindow.mExitProgressGrid.Visibility = Visibility.Hidden;
          }))));
          JObject jobject3 = new JObject()
          {
            [nameof (pcode)] = (JToken) pcode
          };
          if (jobject2["result"].ToString() == "ok")
          {
            Logger.Info("Successfully updated Device Profile.");
            Utils.UpdateValueInBootParams(nameof (pcode), pcode, this.ParentWindow.mVmName, false, "bgp");
            this.ParentWindow.mCommonHandler.AddToastPopup((Window) this.ParentWindow, LocaleStrings.GetLocalizedString("STRING_SWITCH_PROFILE_UPDATED", ""), 1.3, false);
            ClientStats.SendMiscellaneousStatsAsync("DeviceProfileChangeStats", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "success", JsonConvert.SerializeObject((object) jobject3), JsonConvert.SerializeObject((object) jobject1), RegistryManager.Instance.Version, "GRM", (string) null, "Android");
          }
          else
          {
            Logger.Warning("DeviceProfile Update failed in android");
            this.ParentWindow.mCommonHandler.AddToastPopup((Window) this.ParentWindow, LocaleStrings.GetLocalizedString("STRING_SWITCH_PROFILE_FAILED", ""), 1.3, false);
            ClientStats.SendMiscellaneousStatsAsync("DeviceProfileChangeStats", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "failed", JsonConvert.SerializeObject((object) jobject3), JsonConvert.SerializeObject((object) jobject1), RegistryManager.Instance.Version, "GRM", (string) null, "Android");
          }
        }));
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in SwitchProfileAndRestart: {0}", (object) ex);
      }
    }

    internal static string GetHelpCenterUrl()
    {
      return WebHelper.GetUrlWithParams(WebHelper.GetServerHost() + "/feedback", (string) null, (string) null, (string) null);
    }

    internal static void RefreshHtmlSidePanelUrl()
    {
      foreach (MainWindow mainWindow in BlueStacksUIUtils.DictWindows.Values)
      {
        try
        {
          mainWindow.mWelcomeTab.mHomeAppManager.ReinitHtmlSidePanel();
        }
        catch (Exception ex)
        {
          Logger.Error("Error while refreshing side html panel for vmname: {0} and exception is: {1}", (object) mainWindow.mVmName, (object) ex);
        }
      }
    }

    internal static void RefreshAppCenterUrl()
    {
      if (!BlueStacksUIUtils.DictWindows.ContainsKey(BlueStacks.Common.Strings.CurrentDefaultVmName))
        return;
      MainWindow dictWindow = BlueStacksUIUtils.DictWindows[BlueStacks.Common.Strings.CurrentDefaultVmName];
      AppTabButton tab1 = dictWindow.mTopBar.mAppTabButtons.GetTab("appcenter");
      if (tab1 != null && tab1.GetBrowserControl() != null)
        tab1.GetBrowserControl().NavigateTo(BlueStacksUIUtils.GetAppCenterUrl(""));
      AppTabButton tab2 = dictWindow.mTopBar.mAppTabButtons.GetTab("gift");
      if (tab2 == null || tab2.GetBrowserControl() == null)
        return;
      tab2.GetBrowserControl().NavigateTo(BlueStacksUIUtils.GetGiftTabUrl());
    }

    internal static string GetMacroCommunityUrl(string currentAppPackage)
    {
      return WebHelper.GetUrlWithParams(WebHelper.GetServerHost() + "/page/macro-share", (string) null, (string) null, (string) null) + "&pkg=" + currentAppPackage;
    }

    internal bool IsRequiredFreeRAMAvailable()
    {
      try
      {
        ulong num1 = 1048576;
        ulong availablePhysicalMemory = new ComputerInfo().AvailablePhysicalMemory;
        int num2 = this.ParentWindow.EngineInstanceRegistry.Memory + 100;
        if (num2 > 2148)
          num2 = 2148;
        ulong num3 = (ulong) num2 * num1;
        if (availablePhysicalMemory < num3)
        {
          Logger.Warning("Available physical memory is less than required. {0} < {1}", (object) (availablePhysicalMemory / num1), (object) num2);
          return false;
        }
      }
      catch (Exception ex)
      {
        Logger.Error("An error occurred while finding free RAM");
        Logger.Error(ex.ToString());
      }
      return true;
    }

    public bool CheckQuitPopupLocal()
    {
      try
      {
        if (!this.ParentWindow.mGuestBootCompleted)
        {
          this.ParentWindow.Dispatcher.Invoke((Delegate) (() =>
          {
            QuitPopupControl quitPopupControl = new QuitPopupControl(this.ParentWindow);
            string tag = "exit_popup_boot";
            quitPopupControl.CurrentPopupTag = tag;
            BlueStacksUIBinding.Bind(quitPopupControl.TitleTextBlock, "STRING_TROUBLE_STARTING_BLUESTACKS", "");
            BlueStacksUIBinding.Bind((System.Windows.Controls.Button) quitPopupControl.mCloseBlueStacksButton, "STRING_CLOSE_BLUESTACKS");
            quitPopupControl.AddQuitActionItem(QuitActionItem.StuckAtBoot);
            quitPopupControl.AddQuitActionItem(QuitActionItem.SlowPerformance);
            quitPopupControl.AddQuitActionItem(QuitActionItem.SomethingElseWrong);
            quitPopupControl.CloseBlueStacksButton.PreviewMouseUp += new MouseButtonEventHandler(this.ParentWindow.MainWindow_CloseWindowConfirmationAcceptedHandler);
            this.ParentWindow.HideDimOverlay();
            this.ParentWindow.ShowDimOverlay((IDimOverlayControl) quitPopupControl);
            ClientStats.SendLocalQuitPopupStatsAsync(tag, "popup_shown");
          }));
          return true;
        }
        if (!RegistryManager.Instance.Guest[this.ParentWindow.mVmName].IsGoogleSigninDone && string.Equals(this.ParentWindow.StaticComponents.mSelectedTabButton.PackageName, "com.android.vending", StringComparison.InvariantCultureIgnoreCase))
        {
          this.ParentWindow.Dispatcher.Invoke((Delegate) (() =>
          {
            QuitPopupControl quitPopupControl = new QuitPopupControl(this.ParentWindow);
            string tag = "exit_popup_ots";
            quitPopupControl.CurrentPopupTag = tag;
            BlueStacksUIBinding.Bind(quitPopupControl.TitleTextBlock, "STRING_YOU_ARE_ONE_STEP_AWAY", "");
            BlueStacksUIBinding.Bind((System.Windows.Controls.Button) quitPopupControl.mCloseBlueStacksButton, "STRING_CLOSE_BLUESTACKS");
            quitPopupControl.AddQuitActionItem(QuitActionItem.WhyGoogleAccount);
            quitPopupControl.AddQuitActionItem(QuitActionItem.TroubleSigningIn);
            quitPopupControl.AddQuitActionItem(QuitActionItem.SomethingElseWrong);
            quitPopupControl.CloseBlueStacksButton.PreviewMouseUp += new MouseButtonEventHandler(this.ParentWindow.MainWindow_CloseWindowConfirmationAcceptedHandler);
            this.ParentWindow.HideDimOverlay();
            this.ParentWindow.ShowDimOverlay((IDimOverlayControl) quitPopupControl);
            ClientStats.SendLocalQuitPopupStatsAsync(tag, "popup_shown");
          }));
          return true;
        }
        if (this.ParentWindow.mVmName == "Android" && RegistryManager.Instance.FirstAppLaunchState != AppLaunchState.Launched)
        {
          this.ParentWindow.Dispatcher.Invoke((Delegate) (() =>
          {
            QuitPopupControl quitPopupControl = new QuitPopupControl(this.ParentWindow);
            string tag = "exit_popup_no_app";
            quitPopupControl.CurrentPopupTag = tag;
            BlueStacksUIBinding.Bind(quitPopupControl.TitleTextBlock, "STRING_HAVING_TROUBLE_STARTING_GAME", "");
            BlueStacksUIBinding.Bind((System.Windows.Controls.Button) quitPopupControl.ReturnBlueStacksButton, "STRING_RETURN_BLUESTACKS");
            BlueStacksUIBinding.Bind((System.Windows.Controls.Button) quitPopupControl.CloseBlueStacksButton, "STRING_CLOSE_BLUESTACKS");
            quitPopupControl.AddQuitActionItem(QuitActionItem.UnsureWhereStart);
            quitPopupControl.AddQuitActionItem(QuitActionItem.IssueInstallingGame);
            quitPopupControl.AddQuitActionItem(QuitActionItem.FacingOtherTroubles);
            quitPopupControl.CloseBlueStacksButton.PreviewMouseUp += new MouseButtonEventHandler(this.ParentWindow.MainWindow_CloseWindowConfirmationAcceptedHandler);
            this.ParentWindow.HideDimOverlay();
            this.ParentWindow.ShowDimOverlay((IDimOverlayControl) quitPopupControl);
            ClientStats.SendLocalQuitPopupStatsAsync(tag, "popup_shown");
          }));
          return true;
        }
        if (!RegistryManager.Instance.IsNotificationModeAlwaysOn)
        {
          if (this.ParentWindow.EngineInstanceRegistry.IsShowMinimizeBlueStacksPopupOnClose)
          {
            if (this.ParentWindow.EngineInstanceRegistry.NotificationModePopupShownCount < RegistryManager.Instance.NotificationModeCounter)
            {
              string package;
              if (this.CheckIfNotificationModePopupToBeShown(this.ParentWindow, out package))
              {
                if (string.Compare("Android", this.ParentWindow.mVmName, StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                  this.ParentWindow.Dispatcher.Invoke((Delegate) (() =>
                  {
                    this.ParentWindow.HideDimOverlay();
                    NotificationModeExitPopup notificationModeExitPopup = new NotificationModeExitPopup(this.ParentWindow, package);
                    ContainerWindow containerWindow = new ContainerWindow(this.ParentWindow, (System.Windows.Controls.UserControl) notificationModeExitPopup, notificationModeExitPopup.Width, notificationModeExitPopup.Height, false, true, false, 12.0, (System.Windows.Media.Brush) new BrushConverter().ConvertFrom((object) "#4CFFFFFF"));
                  }));
                  return true;
                }
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Exception while trying to show local quit popup. " + ex.ToString());
      }
      return false;
    }

    private bool CheckIfNotificationModePopupToBeShown(MainWindow window, out string packageName)
    {
      packageName = window.StaticComponents.mSelectedTabButton.PackageName;
      JsonParser jsonParser = new JsonParser(window.mVmName);
      if (window.StaticComponents.mSelectedTabButton.mTabType == TabType.AppTab)
      {
        bool flag1 = true;
        bool flag2 = true;
        foreach (string key in window.mTopBar.mAppTabButtons.mDictTabs.Keys)
        {
          bool flag3 = PostBootCloudInfoManager.Instance.mPostBootCloudInfo.GameNotificationAppPackages?.NotificationModeAppPackages?.IsPackageAvailable(key).Value;
          flag1 &= flag3;
          flag2 = flag2 && !flag3;
        }
        return flag1 || !flag2 && PostBootCloudInfoManager.Instance.mPostBootCloudInfo.GameNotificationAppPackages?.NotificationModeAppPackages?.IsPackageAvailable(packageName).Value;
      }
      bool flag4 = true;
      List<string> list = ((IEnumerable<string>) Utils.GetInstalledPackagesFromAppsJSon(this.ParentWindow.mVmName).Split(',')).ToList<string>();
      foreach (string appPackage in list)
      {
        if (!string.IsNullOrEmpty(appPackage))
        {
          bool flag1 = PostBootCloudInfoManager.Instance.mPostBootCloudInfo.GameNotificationAppPackages?.NotificationModeAppPackages?.IsPackageAvailable(appPackage).Value;
          flag4 = flag4 && !flag1;
        }
      }
      if (flag4)
        return false;
      packageName = window.EngineInstanceRegistry.LastNotificationEnabledAppLaunched;
      if (string.IsNullOrEmpty(packageName) || !list.Contains(packageName))
      {
        foreach (string appPackage in list)
        {
          NotificationModeInfo notificationAppPackages = PostBootCloudInfoManager.Instance.mPostBootCloudInfo.GameNotificationAppPackages;
          bool? nullable1;
          bool? nullable2;
          if (notificationAppPackages == null)
          {
            nullable1 = new bool?();
            nullable2 = nullable1;
          }
          else
          {
            AppPackageListObject notificationModeAppPackages = notificationAppPackages.NotificationModeAppPackages;
            if (notificationModeAppPackages == null)
            {
              nullable1 = new bool?();
              nullable2 = nullable1;
            }
            else
              nullable2 = new bool?(notificationModeAppPackages.IsPackageAvailable(appPackage));
          }
          nullable1 = nullable2;
          if (nullable1.Value)
          {
            packageName = appPackage;
            break;
          }
        }
      }
      return true;
    }

    public void OpenPikaAccountPage()
    {
      if (!this.ParentWindow.mAppHandler.IsOneTimeSetupCompleted || string.IsNullOrEmpty(RegistryManager.Instance.RegisteredEmail))
        return;
      this.ParentWindow.mTopBar.mAppTabButtons.AddWebTab(WebHelper.GetUrlWithParams(WebHelper.GetServerHost() + "/bluestacks_account?extra=section:pika", (string) null, (string) null, (string) null) + "&email=" + RegistryManager.Instance.RegisteredEmail + "&token=" + RegistryManager.Instance.Token, "STRING_ACCOUNT", "account_tab", true, "account_tab", true);
    }

    internal void HandleApplicationBrowserClick(
      string clickActionValue,
      string title,
      string key,
      bool paramsOnlyActionValue = false,
      string customImageName = "")
    {
      title = title.Trim();
      string imagePath = "cef_tab";
      switch (key)
      {
        case "APP_CENTER_TEXT":
        case "appcenter":
          clickActionValue = HTTPUtils.MergeQueryParams(BlueStacksUIUtils.GetAppCenterUrl(""), clickActionValue, paramsOnlyActionValue);
          if (string.IsNullOrEmpty(title))
            title = LocaleStrings.GetLocalizedString("STRING_APP_CENTER", "");
          key = "appcenter";
          imagePath = "appcenter";
          break;
        case "FEEDBACK_TEXT":
          clickActionValue = HTTPUtils.MergeQueryParams(BlueStacksUIUtils.GetHelpCenterUrl(), clickActionValue, paramsOnlyActionValue);
          Process.Start(clickActionValue);
          return;
        case "GIFT_TEXT":
        case "gift":
          clickActionValue = HTTPUtils.MergeQueryParams(BlueStacksUIUtils.GetGiftTabUrl(), clickActionValue, paramsOnlyActionValue);
          if (string.IsNullOrEmpty(title))
            title = LocaleStrings.GetLocalizedString("STRING_GIFT", "");
          key = "gift";
          imagePath = "gift";
          break;
        case "MAPS_TEXT":
        case "pikaworld":
          clickActionValue = HTTPUtils.MergeQueryParams(BlueStacksUIUtils.GetPikaWorldUrl(), clickActionValue, paramsOnlyActionValue);
          if (string.IsNullOrEmpty(title))
            title = LocaleStrings.GetLocalizedString("STRING_MAPS", "");
          key = "pikaworld";
          imagePath = "pikaworld";
          break;
        case "preregistration":
          if (string.IsNullOrEmpty(title))
            title = LocaleStrings.GetLocalizedString("STRING_PREREGISTER", "");
          imagePath = "preregistration";
          break;
      }
      if (!string.IsNullOrEmpty(customImageName))
        imagePath = customImageName;
      this.ParentWindow.Utils.AppendUrlWithCommonParamsAndOpenTab(clickActionValue, title, imagePath, key);
    }

    private static string GetImagePath(Dictionary<string, string> payload, string customImageName = "")
    {
      if (!string.IsNullOrEmpty(customImageName))
        return customImageName;
      if (payload.ContainsKey("icon_path"))
        return payload["icon_path"];
      PromotionManager.PopulateAndDownloadFavicon((IDictionary<string, string>) payload, "AppSuggestion", false);
      return !payload.ContainsKey("icon_path") ? "" : payload["icon_path"];
    }

    public void HandleGenericActionFromDictionary(
      Dictionary<string, string> payload,
      string source,
      string customImageName = "")
    {
      try
      {
        if (!payload.ContainsKey("click_generic_action"))
          return;
        switch (EnumHelper.Parse<GenericAction>(payload["click_generic_action"], GenericAction.None))
        {
          case GenericAction.InstallPlay:
            if (!this.ParentWindow.mAppHandler.IsAppInstalled(payload["click_action_packagename"]))
              this.ParentWindow.Utils.SendMessageToAndroidForAffiliate(payload["click_action_packagename"], source);
            this.ParentWindow.mWelcomeTab.OpenFrontendAppTabControl(payload["click_action_packagename"], PlayStoreAction.OpenApp);
            break;
          case GenericAction.InstallCDN:
            if (!this.ParentWindow.mAppHandler.IsAppInstalled(payload["click_action_packagename"]))
              this.ParentWindow.Utils.SendMessageToAndroidForAffiliate(payload["click_action_packagename"], source);
            this.ParentWindow.mAppInstaller.DownloadAndInstallApp(string.Empty, payload["click_action_title"], payload["click_action_value"], payload["click_action_packagename"], false, true, "");
            break;
          case GenericAction.ApplicationBrowser:
            this.HandleApplicationBrowserClick(payload["click_action_value"], payload["click_action_title"], payload.ContainsKey("click_action_key") ? payload["click_action_key"] : "", false, BlueStacksUIUtils.GetImagePath(payload, customImageName));
            break;
          case GenericAction.UserBrowser:
            BlueStacksUIUtils.OpenUrl(payload["click_action_value"]);
            break;
          case GenericAction.HomeAppTab:
            this.ParentWindow.mTopBar.mAppTabButtons.GoToTab("Home", true, false);
            if (string.Compare(payload["click_action_value"], "my_app_text", StringComparison.OrdinalIgnoreCase) == 0)
              break;
            if (payload.ContainsKey("query_params") && !string.IsNullOrEmpty(payload["query_params"].Trim()))
            {
              this.HandleApplicationBrowserClick(payload["query_params"], "", payload["click_action_value"], true, customImageName);
              break;
            }
            this.HandleApplicationBrowserClick("", "", payload["click_action_value"], true, customImageName);
            break;
          case GenericAction.SettingsMenu:
            this.ParentWindow.Dispatcher.Invoke((Delegate) (() => MainWindow.OpenSettingsWindow(this.ParentWindow, payload["click_action_value"])));
            break;
          case GenericAction.KeyBasedPopup:
            switch (payload["click_action_key"].Trim().ToLower(CultureInfo.InvariantCulture))
            {
              case "instance_manager":
                BlueStacksUIUtils.LaunchMultiInstanceManager();
                return;
              case "macro_recorder":
                this.ParentWindow.mCommonHandler.ShowMacroRecorderWindow();
                return;
              case null:
                return;
              default:
                return;
            }
          case GenericAction.OpenSystemApp:
            this.ParentWindow.mTopBar.mAppTabButtons.AddAppTab(payload["click_action_title"], payload["click_action_packagename"], payload["click_action_app_activity"], BlueStacksUIUtils.GetImagePath(payload, customImageName), false, true, false);
            this.ParentWindow.mAppHandler.SwitchWhenPackageNameRecieved = payload["click_action_packagename"];
            this.ParentWindow.mAppHandler.SendRunAppRequestAsync(payload["click_action_packagename"], payload["click_action_app_activity"], false);
            break;
          case GenericAction.PopupBrowser:
            this.ParentWindow.mCommonHandler.OpenBrowserInPopup(payload);
            break;
          case GenericAction.QuickLaunch:
            BlueStacksUIUtils.LaunchAllInstancesAndArrange();
            break;
          case GenericAction.InstallPlayPopup:
            if (!this.ParentWindow.mAppHandler.IsAppInstalled(payload["click_action_packagename"]))
              this.ParentWindow.Utils.SendMessageToAndroidForAffiliate(payload["click_action_packagename"], source);
            this.ParentWindow.mWelcomeTab.mFrontendPopupControl.Init(payload["click_action_packagename"], payload["click_action_title"], PlayStoreAction.OpenApp, true);
            break;
          case GenericAction.OpenGuestUrl:
            JObject jobject = new JObject();
            jobject.Add((object) new JProperty("url", (object) payload["click_action_value"]));
            HTTPUtils.SendRequestToGuestAsync("openurl", new Dictionary<string, string>()
            {
              {
                "key",
                jobject.ToString(Formatting.None)
              }
            }, this.ParentWindow.mVmName, 0, (Dictionary<string, string>) null, true, 1, 0, "bgp");
            break;
          case GenericAction.CreateInstanceSameEngine:
            new DownloadInstallOem(this.ParentWindow).CreateInstanceSameEngine(payload["vmdisplayname"]);
            break;
          default:
            Logger.Warning("Unknown case {0}", (object) payload["click_generic_action"]);
            break;
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Exception on handling click event for payload " + payload.ToDebugString<string, string>() + Environment.NewLine + "Exception: " + ex.ToString());
      }
    }

    internal bool CheckQuitPopupFromCloud(string appPackage = "")
    {
      try
      {
        Logger.Info("IsQuitPopupNotificationReceived status: " + this.ParentWindow.IsQuitPopupNotficationReceived.ToString());
        if (!RegistryManager.Instance.ShowGamingSummary || !this.ParentWindow.IsQuitPopupNotficationReceived || string.IsNullOrEmpty(appPackage) && !this.ParentWindow.mQuitPopupBrowserControl.ShowOnQuit || !string.Equals(this.ParentWindow.mQuitPopupBrowserControl.PackageName, appPackage, StringComparison.InvariantCulture) && !string.Equals(this.ParentWindow.mQuitPopupBrowserControl.PackageName, "*", StringComparison.InvariantCulture) && !string.IsNullOrEmpty(appPackage))
          return false;
        this.ParentWindow.IsQuitPopupNotficationReceived = false;
        if (this.ParentWindow.mQuitPopupBrowserControl.IsForceReload)
          this.ParentWindow.mQuitPopupBrowserControl.RefreshBrowserUrl(WebHelper.GetUrlWithParams(HTTPUtils.MergeQueryParams(this.ParentWindow.mQuitPopupBrowserControl.QuitPopupUrl, "usage_data=" + JsonConvert.SerializeObject((object) AppUsageTimer.GetRealtimeDictionary()[this.ParentWindow.mVmName], Formatting.None), true), (string) null, (string) null, (string) null));
        if (string.IsNullOrEmpty(this.ParentWindow.mQuitPopupBrowserControl.QuitPopupUrl))
          return false;
        this.ParentWindow.Dispatcher.Invoke((Delegate) (() =>
        {
          this.ParentWindow.HideDimOverlay();
          this.ParentWindow.mQuitPopupBrowserControl.Init(appPackage);
        }));
        return true;
      }
      catch (Exception ex)
      {
        Logger.Error("Exception while trying to show quit popup. " + ex.ToString());
        return false;
      }
    }

    internal static void LaunchMultiInstanceManager()
    {
      if (FeatureManager.Instance.IsCustomUIForNCSoft)
        return;
      ProcessUtils.GetProcessObject(Path.Combine(RegistryStrings.InstallDir, "HD-MultiInstanceManager.exe"), (string) null, false).Start();
    }

    internal static void RemoveChildFromParent(UIElement child)
    {
      DependencyObject parent = VisualTreeHelper.GetParent((DependencyObject) child);
      if (parent is System.Windows.Controls.Panel panel)
        panel.Children.Remove(child);
      if (parent is ContentControl contentControl)
        contentControl.Content = (object) null;
      if (!(parent is Decorator decorator))
        return;
      decorator.Child = (UIElement) null;
    }

    public static void UpdateLocale(string locale, string vmToIgnore = "")
    {
      List<string> stringList = new List<string>();
      foreach (string str in ((IEnumerable<string>) RegistryManager.Instance.VmList).ToList<string>())
      {
        string vmName = str;
        try
        {
          if (!RegistryManager.Instance.Guest.ContainsKey(vmName))
          {
            InstanceRegistry instanceRegistry = new InstanceRegistry(vmName, "bgp");
            RegistryManager.Instance.Guest.Add(vmName, instanceRegistry);
          }
          if (RegistryManager.Instance.UserSelectedLocale != RegistryManager.Instance.Guest[vmName].Locale)
          {
            RegistryManager.Instance.Guest[vmName].Locale = locale;
            Utils.UpdateValueInBootParams("LANG", locale, vmName, false, "bgp");
            if (BlueStacksUIUtils.DictWindows.ContainsKey(vmName))
            {
              if (string.Compare(vmName, vmToIgnore.Trim(), StringComparison.OrdinalIgnoreCase) != 0)
              {
                string cmd = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "setlocale {0}", (object) locale);
                new Thread((ThreadStart) (() =>
                {
                  if (VmCmdHandler.RunCommand(cmd, vmName, "bgp") != null)
                    return;
                  Logger.Error("Set locale did not work for vm " + vmName);
                }))
                {
                  IsBackground = true
                }.Start();
              }
            }
          }
        }
        catch (Exception ex)
        {
          Logger.Error("Failed to change locale for vm : " + vmName);
          Logger.Error(ex.ToString());
        }
      }
      HTTPUtils.SendRequestToAgentAsync("reinitlocalization", (Dictionary<string, string>) null, "Android", 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
      LocaleStrings.InitLocalization((string) null, "Android", false);
    }

    internal static bool SendBluestacksLoginRequest(string vmName)
    {
      bool flag = false;
      try
      {
        Dictionary<string, string> data = new Dictionary<string, string>()
        {
          {
            "action",
            "com.bluestacks.account.RETRY_BLUESTACKS_LOGIN"
          }
        };
        JObject jobject = new JObject()
        {
          {
            "windows",
            (JToken) "true"
          }
        };
        data.Add("extras", jobject.ToString(Formatting.None));
        Logger.Info("Sending bluestacks login request");
        HTTPUtils.SendRequestToGuest("customStartService".ToLower(CultureInfo.InvariantCulture), data, vmName, 500, (Dictionary<string, string>) null, false, 1, 0, "bgp");
        flag = true;
      }
      catch (Exception ex)
      {
        Logger.Error("Couldn't send request to guest for login. Ex: {0}", (object) ex.Message);
      }
      return flag;
    }

    internal static bool CheckIfMacroScriptBookmarked(string fileName)
    {
      return ((IEnumerable<string>) RegistryManager.Instance.BookmarkedScriptList).Contains<string>(fileName);
    }

    internal static string GetMacroPlaybackEventName(string vmname)
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}-{1}", (object) "MacroPlayBack", (object) vmname);
    }

    internal void HandleLaunchPlay(string package)
    {
      lock (BlueStacksUIUtils.mLaunchPlaySyncObj)
      {
        int num = 180000;
        while (num > 0)
        {
          --num;
          if (!this.ParentWindow.mEnableLaunchPlayForNCSoft && (FeatureManager.Instance.IsCustomUIForNCSoft || !this.ParentWindow.mGuestBootCompleted))
            Thread.Sleep(1000);
          else
            break;
        }
        if (num <= 0)
          return;
        HTTPUtils.SendRequestToGuest(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "launchplay?pkgname={0}", (object) package), (Dictionary<string, string>) null, this.ParentWindow.mVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
      }
    }

    internal void VolumeDownHandler()
    {
      if (this.CurrentVolumeLevel == 0)
        return;
      int newVolume = this.CurrentVolumeLevel - 7;
      if (newVolume <= 0)
        newVolume = 0;
      this.SetVolumeInFrontendAsync(newVolume);
    }

    internal void VolumeUpHandler()
    {
      if (this.CurrentVolumeLevel >= 100)
        return;
      int newVolume = this.CurrentVolumeLevel + 7;
      if (newVolume >= 100)
        newVolume = 100;
      this.SetVolumeInFrontendAsync(newVolume);
    }

    internal void ToggleTopBarSidebarEnabled(bool isEnabled)
    {
      this.ParentWindow.TopBar.IsEnabled = isEnabled;
      this.ParentWindow.mSidebar.IsEnabled = isEnabled;
    }

    internal static void SendGamepadStatusToBrowsers(bool status)
    {
      try
      {
        object[] args = new object[1]{ (object) "" };
        args[0] = (object) status.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        foreach (BrowserControl allBrowserControl in BrowserControl.sAllBrowserControls)
        {
          try
          {
            if (allBrowserControl != null)
            {
              if (allBrowserControl.CefBrowser != null)
                allBrowserControl.CefBrowser.CallJs("toggleGamePadSupport", args);
            }
          }
          catch (Exception ex)
          {
            Logger.Error("Exception in sending gamepad status to browser:" + allBrowserControl.mUrl + Environment.NewLine + ex.ToString());
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in sending gamepad status to browser:" + ex.ToString());
      }
    }

    internal static double GetDefaultHeight()
    {
      return Oem.IsOEMDmm ? SystemParameters.MaximizedPrimaryScreenHeight * 0.6 + 94.0 : SystemParameters.MaximizedPrimaryScreenHeight * 0.75 + 94.0;
    }

    protected virtual void Dispose(bool disposing)
    {
      if (this.disposedValue)
        return;
      int num = disposing ? 1 : 0;
      if (this.sBootCheckTimer != null)
      {
        this.sBootCheckTimer.Elapsed -= new ElapsedEventHandler(this.SBootCheckTimer_Elapsed);
        this.sBootCheckTimer.Dispose();
      }
      this.disposedValue = true;
    }

    ~BlueStacksUIUtils()
    {
      this.Dispose(false);
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    internal static Dictionary<string, string> GetEngineSettingsData(string vmName)
    {
      return new Dictionary<string, string>()
      {
        {
          "cpu",
          RegistryManager.Instance.Guest[vmName].VCPUs.ToString((IFormatProvider) CultureInfo.InvariantCulture)
        },
        {
          "ram",
          RegistryManager.Instance.Guest[vmName].Memory.ToString((IFormatProvider) CultureInfo.InvariantCulture)
        },
        {
          "glMode",
          RegistryManager.Instance.Guest[vmName].GlMode.ToString((IFormatProvider) CultureInfo.InvariantCulture)
        },
        {
          "glRenderMode",
          RegistryManager.Instance.Guest[vmName].GlRenderMode.ToString((IFormatProvider) CultureInfo.InvariantCulture)
        },
        {
          "gpu",
          RegistryManager.Instance.AvailableGPUDetails
        }
      };
    }

    internal static Dictionary<string, string> GetResolutionData()
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      int guestWidth = RegistryManager.Instance.Guest[BlueStacks.Common.Strings.CurrentDefaultVmName].GuestWidth;
      int guestHeight = RegistryManager.Instance.Guest[BlueStacks.Common.Strings.CurrentDefaultVmName].GuestHeight;
      string str1 = Convert.ToString(guestWidth, (IFormatProvider) CultureInfo.InvariantCulture) + "x" + Convert.ToString(guestHeight, (IFormatProvider) CultureInfo.InvariantCulture);
      dictionary.Add("resolution", str1);
      double num = (double) guestWidth / (double) guestHeight;
      string str2 = "landscape";
      if (num < 1.0)
        str2 = "portrait";
      dictionary.Add("resolution_type", str2);
      return dictionary;
    }

    internal void DownloadAndUpdateMacro(string macroData)
    {
      try
      {
        JObject jobject = JObject.Parse(macroData);
        string str = jobject["macro_name"].ToString();
        string url = jobject["download_link"].ToString();
        string userName = jobject["nickname"].ToString();
        string authorPageUrl = jobject["author_url"].ToString();
        string macroId = jobject["macro_id"].ToString();
        string macroPageUrl = jobject["macro_url"].ToString();
        string invalidCharsFreeName;
        ref string local1 = ref invalidCharsFreeName;
        if (!str.GetValidFileName(out local1))
          return;
        string filePath = Path.Combine(Path.GetTempPath(), string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.json", (object) invalidCharsFreeName));
        new Thread((ThreadStart) (() =>
        {
          using (WebClient webClient = new WebClient())
          {
            try
            {
              webClient.DownloadFile(url, filePath);
            }
            catch (Exception ex)
            {
              Logger.Error("Failed to download macro at path : " + filePath + ". Ex : " + ex.ToString());
            }
            finally
            {
              webClient?.Dispose();
            }
            if (!System.IO.File.Exists(filePath))
              return;
            this.ParentWindow.Dispatcher.Invoke((Delegate) (() =>
            {
              try
              {
                MacroRecording macroRecording = JsonConvert.DeserializeObject<MacroRecording>(System.IO.File.ReadAllText(filePath), Utils.GetSerializerSettings());
                int retCode;
                try
                {
                  if (!string.IsNullOrEmpty(userName))
                    macroRecording.User = userName;
                  Uri result1;
                  if (!string.IsNullOrEmpty(authorPageUrl) && Uri.TryCreate(authorPageUrl, UriKind.RelativeOrAbsolute, out result1))
                    macroRecording.AuthorPageUrl = result1;
                  if (!string.IsNullOrEmpty(macroId))
                    macroRecording.MacroId = macroId;
                  Uri result2;
                  if (!string.IsNullOrEmpty(macroPageUrl) && Uri.TryCreate(macroPageUrl, UriKind.RelativeOrAbsolute, out result2))
                    macroRecording.MacroPageUrl = result2;
                  macroRecording.Name = invalidCharsFreeName;
                  if (string.IsNullOrEmpty(macroRecording.TimeCreated))
                    macroRecording.TimeCreated = DateTime.Now.ToString("yyyyMMddTHHmmss", (IFormatProvider) CultureInfo.InvariantCulture);
                  bool flag = false;
                  this.ParentWindow.MacroRecorderWindow.mRenamingMacrosList.Clear();
                  this.ParentWindow.MacroRecorderWindow.mImportMultiMacroAsUnified = new bool?(true);
                  MacroRecorderWindow macroRecorderWindow = this.ParentWindow.MacroRecorderWindow;
                  List<MacroRecording> recordingsToImport = new List<MacroRecording>();
                  recordingsToImport.Add(macroRecording);
                  ref bool local = ref flag;
                  retCode = macroRecorderWindow.ImportMacroRecordings(recordingsToImport, ref local);
                  if (flag)
                    retCode = 3;
                }
                catch (Exception ex)
                {
                  Logger.Error("Failed to import macro recording.");
                  Logger.Error(ex.ToString());
                  retCode = 2;
                }
                if (retCode == 0)
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
                this.ParentWindow.mCommonHandler.ShowMacroRecorderWindow();
                this.ParentWindow.MacroRecorderWindow.ValidateReturnCode(retCode);
              }
              catch (Exception ex)
              {
                Logger.Error("Failed to deserialize downloaded macro.");
                Logger.Error(ex.ToString());
              }
            }));
            try
            {
              System.IO.File.Delete(filePath);
            }
            catch
            {
            }
          }
        }))
        {
          IsBackground = true
        }.Start();
      }
      catch (Exception ex)
      {
        Logger.Error(string.Format("Invalid data in DowloadMacro api : {0}", (object) ex));
      }
    }

    internal static List<string> GetMacroList()
    {
      List<string> stringList = new List<string>();
      try
      {
        MacroGraph.ReCreateMacroGraphInstance();
        foreach (MacroRecording vertex in (Collection<BiDirectionalVertex<MacroRecording>>) MacroGraph.Instance.Vertices)
        {
          if (!string.IsNullOrEmpty(vertex.Name) && string.IsNullOrEmpty(vertex.MacroId))
            stringList.Add(vertex.Name);
        }
      }
      catch (Exception ex)
      {
        Logger.Debug("Failed to get macro list. Ex : " + ex.ToString());
      }
      return stringList;
    }

    internal static string GetBase64MacroData(string macroName)
    {
      string str = string.Empty;
      try
      {
        JsonSerializerSettings serializerSettings = Utils.GetSerializerSettings();
        string path = Path.Combine(RegistryStrings.MacroRecordingsFolderPath, macroName.ToLower(CultureInfo.InvariantCulture).Trim()) + ".json";
        MacroRecording macroRecording1 = MacroGraph.Instance.Vertices.Cast<MacroRecording>().Where<MacroRecording>((Func<MacroRecording, bool>) (macro => string.Equals(macroName, macro.Name, StringComparison.InvariantCultureIgnoreCase))).FirstOrDefault<MacroRecording>();
        if (macroRecording1.RecordingType == RecordingTypes.SingleRecording)
        {
          Logger.Info("Uploading single recording macro");
          str = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject((object) macroRecording1, serializerSettings)));
        }
        else
        {
          List<string> stringList = new List<string>();
          foreach (MacroRecording allChild in MacroGraph.Instance.GetAllChilds((BiDirectionalVertex<MacroRecording>) macroRecording1))
            stringList.Add(System.IO.File.ReadAllText(Path.Combine(RegistryStrings.MacroRecordingsFolderPath, allChild.Name.ToLower(CultureInfo.InvariantCulture).Trim() + ".json")));
          MacroRecording macroRecording2 = JsonConvert.DeserializeObject<MacroRecording>(System.IO.File.ReadAllText(path), serializerSettings);
          macroRecording2.SourceRecordings = stringList;
          str = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject((object) macroRecording2, serializerSettings)));
          Logger.Info("Uploading merged macro");
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Coulnd't upload macro recording {0}, Ex: {1}", (object) macroName, (object) ex);
      }
      return str;
    }

    private static void LaunchAllInstancesAndArrange()
    {
      ThreadPool.QueueUserWorkItem((WaitCallback) (job =>
      {
        RegistryManager.ClearRegistryMangerInstance();
        foreach (string vm in RegistryManager.Instance.VmList)
        {
          if (!BlueStacksUIUtils.DictWindows.ContainsKey(vm))
          {
            BlueStacksUIUtils.RunInstance(vm, false, "");
            int num = RegistryManager.Instance.BatchInstanceStartInterval;
            if (num <= 0)
              num = 2;
            Thread.Sleep(num * 1000);
          }
        }
      }));
    }
  }
}
