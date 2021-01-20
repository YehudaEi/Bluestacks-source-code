// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.FrontendHandler
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace BlueStacks.BlueStacksUI
{
  public class FrontendHandler
  {
    internal DateTime mFrontendStartTime = DateTime.Now;
    internal bool mIsSufficientRAMAvailable = true;
    private object mLockObject = new object();
    private int frontendRestartAttempts;
    internal MainWindow ParentWindow;
    private string mVmName;
    internal string mWindowTitle;
    private bool sIsfrontendAlreadyVisible;
    internal IntPtr mFrontendHandle;
    internal bool IsShootingModeActivated;
    internal System.Action<MainWindow> ReparentingCompletedAction;

    internal event EventHandler mEventOnFrontendClosed;

    internal bool IsRestartFrontendWhenClosed { get; set; }

    public FrontendHandler(string vmName)
    {
      this.mVmName = vmName;
      this.mWindowTitle = Oem.Instance.CommonAppTitleText + vmName;
      this.StartFrontend();
    }

    internal void FrontendHandler_ShowLowRAMMessage()
    {
      if (!this.ParentWindow?.IsLoaded.GetValueOrDefault())
        return;
      CustomMessageWindow cmw = new CustomMessageWindow();
      cmw.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_PERF_WARNING", "");
      cmw.AddWarning(LocaleStrings.GetLocalizedString("STRING_LOW_AVAILABLE_RAM_TITLE", ""), "message_error");
      cmw.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_LOW_AVAILABLE_RAM_TEXT1", "") + Environment.NewLine + LocaleStrings.GetLocalizedString("STRING_LOW_AVAILABLE_RAM_TEXT2", "");
      cmw.AddButton(ButtonColors.Red, LocaleStrings.GetLocalizedString("STRING_CONTINUE_ANYWAY", ""), (EventHandler) ((o, args) => cmw.Close()), (string) null, false, (object) null, true);
      cmw.AddButton(ButtonColors.White, LocaleStrings.GetLocalizedString("STRING_CLOSE_BLUESTACKS", ""), (EventHandler) ((o, args) => this.ParentWindow.Close()), (string) null, false, (object) null, true);
      this.ParentWindow.ShowDimOverlay((IDimOverlayControl) null);
      cmw.Owner = (Window) this.ParentWindow.mDimOverlay;
      cmw.ShowDialog();
      this.ParentWindow.HideDimOverlay();
    }

    internal void StartFrontend()
    {
      ThreadPool.QueueUserWorkItem((WaitCallback) (obj =>
      {
        Logger.Info("BOOT_STAGE: Starting player");
        if (ProcessUtils.IsLockInUse(Strings.GetPlayerLockName(this.mVmName, "bgp")))
          this.KillFrontend(true);
        this.mEventOnFrontendClosed = (EventHandler) null;
        this.mIsSufficientRAMAvailable = true;
        this.IsRestartFrontendWhenClosed = true;
        this.mFrontendStartTime = DateTime.Now;
        int num = BluestacksProcessHelper.StartFrontend(this.mVmName);
        if (this.ParentWindow == null)
          this.WaitForParentWindowInit();
        if (this.ParentWindow != null)
        {
          switch (num)
          {
            case -10:
              this.ParentWindow.Dispatcher.Invoke((Delegate) (() =>
              {
                string url = (string) null;
                url = WebHelper.GetUrlWithParams(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) WebHelper.GetServerHost(), (object) "help_articles"), (string) null, (string) null, (string) null);
                url = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}&article={1}", (object) url, (object) "enable_virtualization");
                string path = "STRING_VTX_DISABLED_ENABLEIT_BODY";
                CustomMessageWindow customMessageWindow = new CustomMessageWindow();
                BlueStacksUIBinding.Bind(customMessageWindow.TitleTextBlock, "STRING_RESTART_UTILITY_CANNOT_START", "");
                customMessageWindow.AddAboveBodyWarning(LocaleStrings.GetLocalizedString("STRING_VTX_DISABLED_WARNING", ""));
                customMessageWindow.AboveBodyWarningTextBlock.Visibility = Visibility.Visible;
                customMessageWindow.MessageIcon.VerticalAlignment = VerticalAlignment.Center;
                BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlock, path, "");
                customMessageWindow.AddButton(ButtonColors.Blue, "STRING_CHECK_FAQ", (EventHandler) ((sender1, e1) => BlueStacksUIUtils.OpenUrl(url)), (string) null, false, (object) null, true);
                customMessageWindow.AddButton(ButtonColors.White, "STRING_EXIT", (EventHandler) null, (string) null, false, (object) null, true);
                customMessageWindow.ShowDialog();
                App.ExitApplication();
              }));
              break;
            case -7:
              this.ParentWindow.Dispatcher.Invoke((Delegate) (() =>
              {
                Logger.Error("VBox couldn't detect driver");
                CustomMessageWindow customMessageWindow = new CustomMessageWindow();
                BlueStacksUIBinding.Bind(customMessageWindow.TitleTextBlock, "STRING_ENGINE_FAIL_HEADER", "");
                BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlock, "STRING_COULDNT_BOOT_TRY_RESTART", "");
                customMessageWindow.AddButton(ButtonColors.Blue, "STRING_RESTART_PC", new EventHandler(this.RestartPCEvent), (string) null, false, (object) null, true);
                customMessageWindow.AddButton(ButtonColors.White, "STRING_EXIT", (EventHandler) null, (string) null, false, (object) null, true);
                customMessageWindow.ShowDialog();
                App.ExitApplication();
              }));
              break;
            case -6:
              this.ParentWindow.Dispatcher.Invoke((Delegate) (() =>
              {
                Logger.Error("Unable to initialise audio on this machine");
                CustomMessageWindow customMessageWindow = new CustomMessageWindow();
                customMessageWindow.ImageName = "sound_error";
                BlueStacksUIBinding.Bind(customMessageWindow.TitleTextBlock, "STRING_AUDIO_SERVICE_FAILURE", "");
                BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlockTitle, "STRING_AUDIO_SERVICE_FAILUE_FIX", "");
                customMessageWindow.BodyTextBlockTitle.Visibility = Visibility.Visible;
                BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlock, "STRING_AUDIO_SERVICE_FAILURE_ALTERNATE_FIX", "");
                customMessageWindow.AddButton(ButtonColors.Blue, "STRING_READ_MORE", (EventHandler) ((sender1, e1) => BlueStacksUIUtils.OpenUrl(WebHelper.GetUrlWithParams(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) WebHelper.GetServerHost(), (object) "help_articles"), (string) null, (string) null, (string) null) + "&article=audio_service_issue")), "external_link", true, (object) null, true);
                customMessageWindow.ShowDialog();
                App.ExitApplication();
              }));
              break;
            case -5:
              this.ParentWindow.Dispatcher.Invoke((Delegate) (() =>
              {
                Logger.Error("Hyper v enabled on this machine");
                CustomMessageWindow customMessageWindow = new CustomMessageWindow();
                BlueStacksUIBinding.Bind(customMessageWindow.TitleTextBlock, "STRING_RESTART_UTILITY_CANNOT_START", "");
                customMessageWindow.AddWarning(LocaleStrings.GetLocalizedString("STRING_HYPERV_ENABLED_WARNING", ""), "message_error");
                BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlock, "STRING_HYPERV_ENABLED_MESSAGE", "");
                customMessageWindow.AddButton(ButtonColors.Blue, "STRING_CHECK_FAQ", (EventHandler) ((sender1, e1) => BlueStacksUIUtils.OpenUrl(WebHelper.GetUrlWithParams(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) WebHelper.GetServerHost(), (object) "help_articles"), (string) null, (string) null, (string) null) + "&article=disable_hypervisor")), (string) null, false, (object) null, true);
                customMessageWindow.ShowDialog();
                App.ExitApplication();
              }));
              break;
            case -2:
              this.ParentWindow.Dispatcher.Invoke((Delegate) (() =>
              {
                Logger.Error("Android File Integrity check failed");
                CustomMessageWindow customMessageWindow = new CustomMessageWindow();
                BlueStacksUIBinding.Bind(customMessageWindow.TitleTextBlock, "STRING_CORRUPT_INSTALLATION", "");
                BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlock, "STRING_CORRUPT_INSTALLATION_MESSAGE", "");
                customMessageWindow.AddButton(ButtonColors.Blue, "STRING_EXIT", (EventHandler) null, (string) null, false, (object) null, true);
                customMessageWindow.ShowDialog();
                App.ExitApplication();
              }));
              break;
            default:
              if (!this.IsRestartFrontendWhenClosed)
                break;
              this.ParentWindow.Dispatcher.Invoke((Delegate) (() =>
              {
                if (this.frontendRestartAttempts >= 2)
                  return;
                ++this.frontendRestartAttempts;
                this.ParentWindow.RestartFrontend();
              }));
              break;
          }
        }
        else
          Logger.Error("parent window is null for vmName: {0} and frontend Exit code: {1}", (object) this.mVmName, (object) num);
      }));
    }

    private void WaitForParentWindowInit()
    {
      Logger.Info("In method WaitForParentWindowInit for vmName: " + this.mVmName);
      int num = 20;
      while (num > 0)
      {
        --num;
        try
        {
          if (this.ParentWindow != null && BlueStacksUIUtils.DictWindows.ContainsKey(this.mVmName))
          {
            Logger.Info("parent window init for vmName: " + this.mVmName);
            return;
          }
          Thread.Sleep(200);
        }
        catch (Exception ex)
        {
          Logger.Error("Exception in wait for mainwindow init: " + ex.ToString());
          Thread.Sleep(200);
        }
      }
      Logger.Error("Parent window not init after {0} retries", (object) num);
    }

    private void RestartPCEvent(object sender, EventArgs e)
    {
      Process.Start("shutdown.exe", "/r /t 0");
    }

    internal void KillFrontendAsync()
    {
      ThreadPool.QueueUserWorkItem((WaitCallback) (obj => this.KillFrontend(true)));
    }

    internal void KillFrontend(bool isWaitForPlayerClosing = false)
    {
      try
      {
        this.IsRestartFrontendWhenClosed = false;
        Utils.StopFrontend(this.mVmName, isWaitForPlayerClosing);
      }
      catch (Exception ex)
      {
        Logger.Warning("Exception in killing frontend: " + ex.ToString());
      }
      finally
      {
        Logger.Info("FrontendHandler-> KillFrontend -> raise event mEventOnFrontendClosed:");
        EventHandler onFrontendClosed = this.mEventOnFrontendClosed;
        if (onFrontendClosed != null)
          onFrontendClosed((object) this.mVmName, (EventArgs) null);
      }
    }

    internal void EnableKeyMapping(bool isEnabled)
    {
      try
      {
        this.SendFrontendRequestAsync("setKeymappingState", new Dictionary<string, string>()
        {
          {
            "keymapping",
            isEnabled.ToString((IFormatProvider) CultureInfo.InvariantCulture)
          }
        });
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to send EnableKeyMapping to frontend... Err : " + ex.ToString());
      }
    }

    internal void GetScreenShot(string filePath)
    {
      this.SendFrontendRequestAsync("getScreenshot", new Dictionary<string, string>()
      {
        {
          "path",
          filePath
        },
        {
          "showSavedInfo",
          true.ToString((IFormatProvider) CultureInfo.InvariantCulture)
        }
      });
    }

    internal void FrontendVisibleChanged(bool value)
    {
      try
      {
        Dictionary<string, string> data = new Dictionary<string, string>()
        {
          {
            "new_value",
            Convert.ToString(value, (IFormatProvider) CultureInfo.InvariantCulture)
          }
        };
        if (this.ParentWindow.IsMuted)
          data.Add("is_mute", Convert.ToString(true, (IFormatProvider) CultureInfo.InvariantCulture));
        else
          data.Add("is_mute", Convert.ToString(false, (IFormatProvider) CultureInfo.InvariantCulture));
        this.SendFrontendRequestAsync("frontendVisibleChanged", data);
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to send refresh keymap to frontend... Err : " + ex.ToString());
      }
    }

    internal void RefreshKeyMap(string packageName)
    {
      try
      {
        this.SendFrontendRequestAsync("refreshKeymap", new Dictionary<string, string>()
        {
          {
            "package",
            packageName
          }
        });
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to send refresh keymap to frontend... Err : " + ex.ToString());
      }
    }

    internal void DeactivateFrontend()
    {
      try
      {
        if (!(this.mFrontendHandle != IntPtr.Zero))
          return;
        Logger.Debug("KMP deactivateFrontend");
        this.SendFrontendRequestAsync("deactivateFrontend", (Dictionary<string, string>) null);
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to send deactivate to frontend.. Err : " + ex.ToString());
      }
    }

    internal void ToggleStreamingMode(bool state)
    {
      this.ParentWindow.mTopBar.mSettingsMenuPopup.IsOpen = false;
      System.Action action;
      this.ParentWindow.Dispatcher.Invoke((Delegate) (() =>
      {
        Logger.Info("Streaming mode toggle called with state.." + state.ToString());
        this.ParentWindow.mStreamingModeEnabled = state;
        string str = this.ParentWindow.Handle.ToString();
        if (state)
          str = "0";
        Rectangle windowRectangle = this.GetWindowRectangle();
        if (windowRectangle.Width == 0 && windowRectangle.Height == 0)
        {
          windowRectangle.Width = (int) this.ParentWindow.Width;
          windowRectangle.Height = (int) this.ParentWindow.Height;
        }
        Dictionary<string, string> dict = new Dictionary<string, string>()
        {
          {
            "ParentHandle",
            str
          },
          {
            "X",
            windowRectangle.X.ToString((IFormatProvider) CultureInfo.InvariantCulture)
          },
          {
            "Y",
            windowRectangle.Y.ToString((IFormatProvider) CultureInfo.InvariantCulture)
          },
          {
            "Width",
            windowRectangle.Width.ToString((IFormatProvider) CultureInfo.InvariantCulture)
          },
          {
            "Height",
            windowRectangle.Height.ToString((IFormatProvider) CultureInfo.InvariantCulture)
          }
        };
        ThreadPool.QueueUserWorkItem((WaitCallback) (obj1 =>
        {
          try
          {
            JObject jobject = JObject.Parse(JArray.Parse(this.SendFrontendRequest("setparent", dict))[0].ToString());
            if (!jobject["success"].ToObject<bool>())
              return;
            this.mFrontendHandle = new IntPtr(jobject["frontendhandle"].ToObject<int>());
            this.ParentWindow.Dispatcher.Invoke((Delegate) (action ?? (action = (System.Action) (() => this.ShowGLWindow()))));
          }
          catch (Exception ex)
          {
            Logger.Error("Failed to send Show event to engine... err : " + ex.ToString());
          }
        }));
      }));
    }

    internal void ShowGLWindow()
    {
      if (this.CanfrontendBeResizedAndFocused())
        this.ResizeWindow();
      else if (this.ParentWindow.mFrontendGrid.IsVisible)
      {
        if (this.ParentWindow.Handle.ToString().Equals("0", StringComparison.OrdinalIgnoreCase))
          return;
        this.sIsfrontendAlreadyVisible = true;
        if (this.mFrontendHandle == IntPtr.Zero)
          ThreadPool.QueueUserWorkItem((WaitCallback) (obj =>
          {
            try
            {
              this.ParentWindow.Dispatcher.Invoke((Delegate) (() =>
              {
                Rectangle windowRectangle = this.GetWindowRectangle();
                Dictionary<string, string> dict = new Dictionary<string, string>()
                {
                  {
                    "ParentHandle",
                    !this.ParentWindow.mStreamingModeEnabled ? this.ParentWindow.Handle.ToString() : "0"
                  },
                  {
                    "X",
                    windowRectangle.X.ToString((IFormatProvider) CultureInfo.InvariantCulture)
                  },
                  {
                    "Y",
                    windowRectangle.Y.ToString((IFormatProvider) CultureInfo.InvariantCulture)
                  },
                  {
                    "Width",
                    windowRectangle.Width.ToString((IFormatProvider) CultureInfo.InvariantCulture)
                  },
                  {
                    "Height",
                    windowRectangle.Height.ToString((IFormatProvider) CultureInfo.InvariantCulture)
                  },
                  {
                    "isMute",
                    this.ParentWindow.IsMuted.ToString((IFormatProvider) CultureInfo.InvariantCulture)
                  }
                };
                if (windowRectangle.Width == 0 || windowRectangle.Height == 0)
                  this.sIsfrontendAlreadyVisible = false;
                else
                  ThreadPool.QueueUserWorkItem((WaitCallback) (obj1 =>
                  {
                    try
                    {
                      lock (this.mLockObject)
                      {
                        if (!(this.mFrontendHandle == IntPtr.Zero))
                          return;
                        JObject jobject = JObject.Parse(JArray.Parse(this.SendFrontendRequest("setParent", dict))[0].ToString());
                        if (jobject["success"].ToObject<bool>())
                          this.mFrontendHandle = new IntPtr(jobject["frontendhandle"].ToObject<int>());
                        System.Action<MainWindow> reparentingCompletedAction = this.ReparentingCompletedAction;
                        if (reparentingCompletedAction != null)
                          reparentingCompletedAction(this.ParentWindow);
                        Logger.Debug("Set parent call completed. handle: " + this.mFrontendHandle.ToString());
                      }
                    }
                    catch (Exception ex)
                    {
                      Logger.Error("Failed to send Show event to engine... err : " + ex.ToString());
                    }
                  }));
              }));
            }
            catch (Exception ex)
            {
              Logger.Error("Failed to send Show event to engine... err : " + ex.ToString());
            }
          }));
        else
          this.ResizeWindow();
      }
      else
      {
        this.sIsfrontendAlreadyVisible = false;
        if (!(this.mFrontendHandle != IntPtr.Zero))
          return;
        InteropWindow.ShowWindow(this.mFrontendHandle, 0);
        if (!KMManager.dictOverlayWindow.ContainsKey(this.ParentWindow) || this.ParentWindow.WindowState == WindowState.Maximized)
          return;
        KMManager.ShowOverlayWindow(this.ParentWindow, false, false);
      }
    }

    private bool CanfrontendBeResizedAndFocused()
    {
      return (this.ParentWindow.mDimOverlay == null || !this.ParentWindow.mDimOverlay.IsWindowVisible) && this.ParentWindow.mFrontendGrid.IsVisible && this.sIsfrontendAlreadyVisible;
    }

    internal void ResizeWindow()
    {
      Rectangle windowRectangle = this.GetWindowRectangle();
      if (this.ParentWindow.mStreamingModeEnabled)
        InteropWindow.ShowWindow(this.mFrontendHandle, 5);
      else
        InteropWindow.SetWindowPos(this.mFrontendHandle, (IntPtr) 0, windowRectangle.X, windowRectangle.Y, windowRectangle.Width, windowRectangle.Height, 16448U);
      if (KMManager.dictOverlayWindow.ContainsKey(this.ParentWindow))
      {
        if (this.ParentWindow.StaticComponents.mLastMappableWindowHandle == IntPtr.Zero)
          this.ParentWindow.StaticComponents.mLastMappableWindowHandle = this.mFrontendHandle;
        KMManager.dictOverlayWindow[this.ParentWindow].UpdateSize();
      }
      this.FocusFrontend();
      RegistryManager.Instance.FrontendHeight = windowRectangle.Height;
      RegistryManager.Instance.FrontendWidth = windowRectangle.Width;
    }

    internal Rectangle GetWindowRectangle()
    {
      Grid mFrontendGrid = this.ParentWindow.mFrontendGrid;
      System.Windows.Point point = mFrontendGrid.TranslatePoint(new System.Windows.Point(0.0, 0.0), (UIElement) this.ParentWindow);
      return new Rectangle(new System.Drawing.Point((int) (MainWindow.sScalingFactor * point.X), (int) (MainWindow.sScalingFactor * point.Y)), new System.Drawing.Size((int) (mFrontendGrid.ActualWidth * MainWindow.sScalingFactor), (int) (mFrontendGrid.ActualHeight * MainWindow.sScalingFactor)));
    }

    internal void ChangeFrontendToPortraitMode()
    {
      Rectangle windowRectangle = this.GetWindowRectangle();
      InteropWindow.SetWindowPos(this.mFrontendHandle, (IntPtr) 0, windowRectangle.X, windowRectangle.Y, windowRectangle.Width, windowRectangle.Height, 16448U);
    }

    internal void FocusFrontend()
    {
      if (this.CanfrontendBeResizedAndFocused() && !this.ParentWindow.mStreamingModeEnabled && (!this.ParentWindow.mIsFocusComeFromImap && this.ParentWindow.IsActive))
      {
        InteropWindow.SetFocus(this.mFrontendHandle);
        Logger.Debug("KMP REFRESH Frontend...." + Environment.StackTrace);
        this.SendFrontendRequestAsync("refreshWindow", (Dictionary<string, string>) null);
      }
      else
        Logger.Debug("KMP CanfrontendBeResizedAndFocused false " + this.ParentWindow.mFrontendGrid.IsVisible.ToString() + this.sIsfrontendAlreadyVisible.ToString());
    }

    internal void SendFrontendRequestAsync(string path, Dictionary<string, string> data = null)
    {
      ThreadPool.QueueUserWorkItem((WaitCallback) (obj => this.SendFrontendRequest(path, data)));
    }

    internal string SendFrontendRequest(string path, Dictionary<string, string> data = null)
    {
      string str = string.Empty;
      try
      {
        str = HTTPUtils.SendRequestToEngine(path, data, this.ParentWindow.mVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "", "bgp");
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in SendFrontendRequest: " + ex.ToString());
      }
      return str;
    }

    internal static void UpdateBootTimeInregistry(DateTime time)
    {
      try
      {
        int num = (int) (DateTime.Now - time).TotalSeconds * 1000;
        int noOfBootCompleted = RegistryManager.Instance.NoOfBootCompleted;
        RegistryManager.Instance.LastBootTime = num;
        RegistryManager.Instance.AvgBootTime = (RegistryManager.Instance.AvgBootTime * 2 + num) / 3;
        RegistryManager.Instance.NoOfBootCompleted = noOfBootCompleted + 1;
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in UpdateBootTimeInregistry: " + ex.ToString());
      }
    }

    internal void UpdateOverlaySizeStatus()
    {
      this.SendFrontendRequestAsync("sendGlWindowSize", new Dictionary<string, string>()
      {
        {
          "updateSize",
          (this.ParentWindow.WindowState == WindowState.Maximized).ToString((IFormatProvider) CultureInfo.InvariantCulture)
        }
      });
    }
  }
}
