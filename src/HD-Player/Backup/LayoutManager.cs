// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.LayoutManager
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using BlueStacks.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace BlueStacks.Player
{
  internal static class LayoutManager
  {
    internal static Size mConfiguredDisplaySize;
    internal static Size mConfiguredGuestSize;
    internal static Size mCurrentDisplaySize;
    internal static Rectangle mScaledDisplayArea;
    internal static bool mEmulatedPortraitMode;
    internal static int sCurrentOrientation;
    internal static bool mRotateGuest180;
    private static bool mUpdateGlWindowSize;
    private static Rectangle mLastGLRectangle;
    internal static bool mFullScreen;

    internal static bool UpdateGlWindowSize
    {
      get
      {
        return LayoutManager.mUpdateGlWindowSize;
      }
      set
      {
        LayoutManager.mUpdateGlWindowSize = value;
        if (!value)
          return;
        LayoutManager.UpdateSizeToGM();
      }
    }

    [DllImport("HD-Imap-Native.dll", CharSet = CharSet.Unicode)]
    private static extern void ImapSetGLWindowParams(int cx, int cy, int width, int height);

    internal static void OrientationHandler(int orientation)
    {
      Logger.Info("Got orientation change notification for {0}", (object) orientation);
      bool flag = LayoutManager.ShouldEmulatePortraitMode();
      Logger.Info("ShouldEmulatePortraitMode => " + flag.ToString());
      if (LayoutManager.sCurrentOrientation == orientation)
      {
        Logger.Info("Not doing anything as current orientation is same as orientation requested");
      }
      else
      {
        LayoutManager.sCurrentOrientation = orientation;
        LayoutManager.mRotateGuest180 = orientation == 2 || orientation == 3;
        if (flag)
        {
          LayoutManager.mEmulatedPortraitMode = orientation == 1 || orientation == 3;
          LayoutManager.mRotateGuest180 = orientation == 2 || orientation == 3;
        }
        else
          LayoutManager.mEmulatedPortraitMode = false;
        UIHelper.RunOnUIThread((Control) VMWindow.Instance, (UIHelper.Action) (() =>
        {
          try
          {
            Logger.Info("Orientation handler calling fixguest");
            if (Oem.Instance.IsResizeFrontendWindow)
              LayoutManager.ResizeFrontendWindow();
            VMWindow.Instance.ResizeWindowOnStreamingMode();
            LayoutManager.FixupGuestDisplay();
          }
          catch (Exception ex)
          {
          }
        }));
      }
    }

    internal static void FixupGuestDisplay()
    {
      VMWindow.Instance.Invalidate();
      LayoutManager.FixupGuestDisplay_FixAspectRatio();
      LayoutManager.FixupGuestDisplay_FixOpenGLSubwindow();
    }

    private static void FixupGuestDisplay_FixAspectRatio()
    {
      float num1 = LayoutManager.IsPortrait() || LayoutManager.mEmulatedPortraitMode ? (float) LayoutManager.mConfiguredGuestSize.Height / (float) LayoutManager.mConfiguredGuestSize.Width : (float) LayoutManager.mConfiguredGuestSize.Width / (float) LayoutManager.mConfiguredGuestSize.Height;
      Size clientSize = VMWindow.Instance.ClientSize;
      double width1 = (double) clientSize.Width;
      clientSize = VMWindow.Instance.ClientSize;
      double height1 = (double) clientSize.Height;
      float num2 = (float) (width1 / height1);
      if ((double) num2 > (double) num1)
      {
        clientSize = VMWindow.Instance.ClientSize;
        float num3 = (float) clientSize.Width / num2 * num1;
        ref Rectangle local1 = ref LayoutManager.mScaledDisplayArea;
        clientSize = VMWindow.Instance.ClientSize;
        int num4 = (clientSize.Width - (int) num3) / 2;
        local1.X = num4;
        LayoutManager.mScaledDisplayArea.Y = 0;
        LayoutManager.mScaledDisplayArea.Width = (int) num3;
        ref Rectangle local2 = ref LayoutManager.mScaledDisplayArea;
        clientSize = VMWindow.Instance.ClientSize;
        int height2 = clientSize.Height;
        local2.Height = height2;
      }
      else
      {
        clientSize = VMWindow.Instance.ClientSize;
        float num3 = (float) clientSize.Height * num2 / num1;
        LayoutManager.mScaledDisplayArea.X = 0;
        ref Rectangle local1 = ref LayoutManager.mScaledDisplayArea;
        clientSize = VMWindow.Instance.ClientSize;
        int num4 = (clientSize.Height - (int) num3) / 2;
        local1.Y = num4;
        ref Rectangle local2 = ref LayoutManager.mScaledDisplayArea;
        clientSize = VMWindow.Instance.ClientSize;
        int width2 = clientSize.Width;
        local2.Width = width2;
        LayoutManager.mScaledDisplayArea.Height = (int) num3;
      }
    }

    private static void FixupGuestDisplay_FixOpenGLSubwindow()
    {
      int orientation = !LayoutManager.IsPortrait() ? (!LayoutManager.mEmulatedPortraitMode ? (LayoutManager.mRotateGuest180 ? 2 : 0) : (LayoutManager.mRotateGuest180 ? 3 : 1)) : LayoutManager.sCurrentOrientation;
      int width = LayoutManager.mScaledDisplayArea.Width;
      int height = LayoutManager.mScaledDisplayArea.Height;
      if (RegistryManager.Instance.IsImeDebuggingEnabled)
      {
        if (VMWindow.sIsWpfTextboxEnabled)
        {
          VMWindow.Instance.mCtrlHost.Location = new Point(0, 0);
          VMWindow.Instance.mCtrlHost.Size = new Size(200, 200);
        }
        else
        {
          VMWindow.Instance.mDummyInputKeyBoard.Location = new Point(0, 0);
          VMWindow.Instance.mDummyInputKeyBoard.Size = new Size(200, 200);
        }
        Opengl.ResizeSubWindow(200, 200, width - 200, height - 200);
      }
      else if (LayoutManager.mLastGLRectangle.Width != width || LayoutManager.mLastGLRectangle.Height != height || (LayoutManager.mLastGLRectangle.X != LayoutManager.mScaledDisplayArea.X || LayoutManager.mLastGLRectangle.Y != LayoutManager.mScaledDisplayArea.Y))
      {
        LayoutManager.mLastGLRectangle.Width = width;
        LayoutManager.mLastGLRectangle.Height = height;
        if (VMWindow.Instance.IsFullscreen)
          --height;
        LayoutManager.mLastGLRectangle.X = LayoutManager.mScaledDisplayArea.X;
        LayoutManager.mLastGLRectangle.Y = LayoutManager.mScaledDisplayArea.Y;
        Opengl.ResizeSubWindow(LayoutManager.mScaledDisplayArea.X, LayoutManager.mScaledDisplayArea.Y, width, height);
        if (LayoutManager.UpdateGlWindowSize)
          LayoutManager.UpdateSizeToGM();
      }
      Opengl.HandleOrientation(1f, 1f, orientation);
      LayoutManager.ImapSetGLWindowParams(LayoutManager.mScaledDisplayArea.X, LayoutManager.mScaledDisplayArea.Y, width, height);
    }

    private static void UpdateSizeToGM()
    {
      HTTPUtils.SendRequestToClientAsync("updateSizeOfOverlay", new Dictionary<string, string>()
      {
        {
          "handle",
          Opengl.GetSubWindow().ToInt32().ToString()
        }
      }, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
    }

    internal static void InitScreen()
    {
      Logger.Info("InitScreen()");
      LayoutManager.mConfiguredDisplaySize = LayoutManager.GetConfiguredDisplaySize();
      LayoutManager.mConfiguredGuestSize = LayoutManager.GetConfiguredGuestSize();
    }

    internal static Size GetConfiguredDisplaySize()
    {
      return new Size(RegistryManager.Instance.DefaultGuest.WindowWidth, RegistryManager.Instance.DefaultGuest.WindowHeight);
    }

    internal static Size GetConfiguredGuestSize()
    {
      return new Size(RegistryManager.Instance.DefaultGuest.GuestWidth, RegistryManager.Instance.DefaultGuest.GuestHeight);
    }

    private static bool ShouldEmulatePortraitMode()
    {
      if (RegistryManager.Instance.DefaultGuest.EmulatePortraitMode == 1)
        return true;
      return RegistryManager.Instance.DefaultGuest.EmulatePortraitMode != 0 && LayoutManager.IsDesktop();
    }

    internal static bool IsDesktop()
    {
      bool flag;
      if (Features.IsFeatureEnabled(1073741824UL))
        flag = true;
      else if (Utils.IsDesktopPC())
      {
        flag = true;
      }
      else
      {
        try
        {
          List<DeviceEnumerator> deviceEnumeratorList = DeviceEnumerator.ListDevices(Guids.VideoInputDeviceCategory);
          flag = deviceEnumeratorList.Count != 2;
          foreach (DeviceEnumerator deviceEnumerator in deviceEnumeratorList)
            deviceEnumerator.Dispose();
        }
        catch (Exception ex)
        {
          Logger.Info("Cannot enumerate camera devices: " + ex?.ToString());
          flag = false;
        }
      }
      return flag;
    }

    private static int GetBorderWidth(int width, int height)
    {
      BlueStacks.Common.RECT lpRect = new BlueStacks.Common.RECT();
      lpRect.Left = 0;
      lpRect.Top = 0;
      lpRect.Right = width;
      lpRect.Bottom = height;
      int dwStyle = 13565952;
      return !InteropWindow.AdjustWindowRect(out lpRect, dwStyle, false) ? 18 : lpRect.Right - lpRect.Left - width;
    }

    internal static bool IsPortrait()
    {
      ScreenOrientation screenOrientation = SystemInformation.ScreenOrientation;
      return screenOrientation == ScreenOrientation.Angle90 || screenOrientation == ScreenOrientation.Angle270;
    }

    internal static void HandleLayoutEvent()
    {
      if (VMWindow.Instance.WindowState == FormWindowState.Minimized)
        return;
      VMWindow.Instance.HandleFrontendActivated();
    }

    internal static void InitOpengl()
    {
      Logger.Info("Opengl.Init({0}, {1}, {2}, {3}, {4})", (object) VMWindow.Instance.Handle, (object) LayoutManager.mScaledDisplayArea.X, (object) LayoutManager.mScaledDisplayArea.Y, (object) LayoutManager.mConfiguredGuestSize.Width, (object) LayoutManager.mConfiguredGuestSize.Height);
      Opengl.Init(MultiInstanceStrings.VmName, VMWindow.Instance.Handle, LayoutManager.mScaledDisplayArea.X, LayoutManager.mScaledDisplayArea.Y, LayoutManager.mConfiguredGuestSize.Width, LayoutManager.mConfiguredGuestSize.Height, new Opengl.GlReadyHandler(LayoutManager.GlInitSuccess), new Opengl.GlInitFailedHandler(LayoutManager.GlInitFailed));
      TimelineStatsSender.HandleEngineBootEvent(EngineStatsEvent.graphics_inited.ToString());
      Logger.Info("Done Opengl.Init");
    }

    internal static void GlInitSuccess()
    {
      Logger.Info("BOOT_STAGE: Gl Init success");
      LayoutManager.FixupGuestDisplay();
      InputMapper.Instance.InputmapperInit();
    }

    internal static void GlInitFailed()
    {
      Logger.Error("Gl Init failed");
      AndroidBootUp.HandleBootError();
    }

    internal static int GetGuestX(int x, int y)
    {
      int landscapeGuestX = LayoutManager.GetLandscapeGuestX(x, y);
      int portraitGuestX = LayoutManager.GetPortraitGuestX(x, y);
      return LayoutManager.IsPortrait() || LayoutManager.mEmulatedPortraitMode ? (LayoutManager.mRotateGuest180 ? portraitGuestX : 32768 - portraitGuestX) : (LayoutManager.mRotateGuest180 ? 32768 - landscapeGuestX : landscapeGuestX);
    }

    internal static int GetGuestY(int x, int y)
    {
      int landscapeGuestY = LayoutManager.GetLandscapeGuestY(x, y);
      int portraitGuestY = LayoutManager.GetPortraitGuestY(x, y);
      return LayoutManager.IsPortrait() || LayoutManager.mEmulatedPortraitMode ? (LayoutManager.mRotateGuest180 ? 32768 - portraitGuestY : portraitGuestY) : (LayoutManager.mRotateGuest180 ? 32768 - landscapeGuestY : landscapeGuestY);
    }

    internal static int GetLandscapeGuestX(int x, int y)
    {
      int num = x - LayoutManager.mScaledDisplayArea.X;
      return LayoutManager.mScaledDisplayArea.Width == 0 ? 0 : (int) ((double) num * 32768.0 / (double) LayoutManager.mScaledDisplayArea.Width);
    }

    internal static int GetPortraitGuestX(int x, int y)
    {
      int num = y - LayoutManager.mScaledDisplayArea.Y;
      return LayoutManager.mScaledDisplayArea.Height == 0 ? 0 : (int) ((double) num * 32768.0 / (double) LayoutManager.mScaledDisplayArea.Height);
    }

    internal static int GetLandscapeGuestY(int x, int y)
    {
      int num = y - LayoutManager.mScaledDisplayArea.Y;
      return LayoutManager.mScaledDisplayArea.Height == 0 ? 0 : (int) ((double) num * 32768.0 / (double) LayoutManager.mScaledDisplayArea.Height);
    }

    internal static int GetPortraitGuestY(int x, int y)
    {
      int num = x - LayoutManager.mScaledDisplayArea.X;
      return LayoutManager.mScaledDisplayArea.Width == 0 ? 0 : (int) ((double) num * 32768.0 / (double) LayoutManager.mScaledDisplayArea.Width);
    }

    internal static void ToggleFullScreen()
    {
      if (!LayoutManager.mFullScreen)
      {
        LayoutManager.mFullScreen = true;
        LayoutManager.ResizeFrontendWindow();
        if (!Features.IsFeatureEnabled(17179869184UL))
          return;
        VMWindow.Instance.mFullScreenToast.Show();
      }
      else
      {
        LayoutManager.mFullScreen = false;
        LayoutManager.ResizeFrontendWindow();
        VMWindow.Instance.mFullScreenToast.Hide();
      }
    }

    internal static void ResizeFrontendWindow()
    {
      Logger.Info("ResizeFrontendWindow()");
      Logger.Info("Suspending Layout");
      VMWindow.Instance.SuspendLayout();
      if (LayoutManager.mFullScreen)
        LayoutManager.ResizeFrontendWindow_FullScreen();
      else
        LayoutManager.ResizeFrontendWindow_Windowed();
      Logger.Info("Resuming Layout");
      VMWindow.Instance.ResumeLayout();
      LayoutManager.FixupGuestDisplay();
      Logger.Info("ResizeFrontendWindow DONE");
    }

    internal static void ResizeFrontendWindow_FullScreen()
    {
      Logger.Info("ResizeFrontendWindow_FullScreen()");
      Logger.Info("Screen size is {0}x{1}", (object) InteropWindow.ScreenWidth, (object) InteropWindow.ScreenHeight);
      Logger.Info("Guest display area is {0}x{1}", (object) LayoutManager.mCurrentDisplaySize.Width, (object) LayoutManager.mCurrentDisplaySize.Height);
      VMWindow.Instance.FormBorderStyle = FormBorderStyle.None;
      if (LayoutManager.mEmulatedPortraitMode && Features.IsFeatureEnabled(2048UL))
      {
        float num = (float) LayoutManager.mConfiguredGuestSize.Width / (float) LayoutManager.mConfiguredGuestSize.Height;
        Size size = new Size()
        {
          Height = Screen.PrimaryScreen.WorkingArea.Height
        };
        size.Height -= Oem.Instance.PartnerControlBarHeight;
        size.Width = (int) ((double) size.Height / (double) num);
        int X = InteropWindow.ScreenWidth - size.Width;
        int Y = 0;
        int width = size.Width;
        int height = size.Height;
        InteropWindow.SetFullScreen(VMWindow.Instance.Handle, X, Y, width, height);
      }
      else
        InteropWindow.SetFullScreen(VMWindow.Instance.Handle);
      Logger.Info("ResizeFrontendWindow_FullScreen DONE");
    }

    internal static void ResizeFrontendWindow_Windowed()
    {
      Logger.Info("ResizeFrontendWindow_Windowed()");
      Logger.Info("mEmulatedPortraitMode: " + LayoutManager.mEmulatedPortraitMode.ToString());
      Size size = new Size();
      int x = 20;
      int y = 20;
      if (Oem.Instance.IsFrontendFormLocation6)
      {
        x = 6;
        y = 6;
      }
      int num1 = Screen.PrimaryScreen.WorkingArea.Height - SystemInformation.CaptionHeight - LayoutManager.GetBorderWidth(100, 100);
      if (LayoutManager.mEmulatedPortraitMode)
      {
        size.Height = num1;
        size.Height -= Oem.Instance.PartnerControlBarHeight;
        double num2 = (double) LayoutManager.mConfiguredGuestSize.Width / (double) LayoutManager.mConfiguredGuestSize.Height;
        double num3 = (double) VMWindow.Instance.ClientSize.Height / (double) VMWindow.Instance.ClientSize.Width;
        if (num2 == num3)
          size.Height = VMWindow.Instance.ClientSize.Height;
        size.Width = (int) ((double) size.Height / num2);
        x = Screen.PrimaryScreen.WorkingArea.Width - size.Width - LayoutManager.GetBorderWidth(100, 100) / 2;
        y = LayoutManager.GetBorderWidth(100, 100) / 2;
        Logger.Info("location: ({0}x{1})", (object) x, (object) y);
      }
      else if (!LayoutManager.IsPortrait())
      {
        size.Width = LayoutManager.mConfiguredDisplaySize.Width;
        size.Height = LayoutManager.mConfiguredDisplaySize.Height;
      }
      else
      {
        size.Width = LayoutManager.mConfiguredDisplaySize.Height;
        size.Height = LayoutManager.mConfiguredDisplaySize.Width;
        size.Height -= Oem.Instance.PartnerControlBarHeight;
      }
      LayoutManager.mCurrentDisplaySize = size;
      Logger.Info("Guest display area is {0}x{1}", (object) LayoutManager.mCurrentDisplaySize.Width, (object) LayoutManager.mCurrentDisplaySize.Height);
      Logger.Info("New window size is {0}x{1}", (object) size.Width, (object) size.Height);
      if (VMWindow.Instance.isStreamingModeEnabled)
      {
        VMWindow.Instance.MaximizeBox = true;
        VMWindow.Instance.WindowState = FormWindowState.Normal;
        VMWindow.Instance.FormBorderStyle = FormBorderStyle.Sizable;
      }
      else if (!Oem.Instance.IsFrontendBorderHidden)
        VMWindow.Instance.FormBorderStyle = VMWindow.Instance.mFormBorderStyle;
      VMWindow.Instance.StartPosition = FormStartPosition.Manual;
      VMWindow.Instance.Location = new Point(x, y);
      VMWindow.Instance.ClientSize = size;
      object[] objArray = new object[2];
      Size clientSize = VMWindow.Instance.ClientSize;
      objArray[0] = (object) clientSize.Width;
      clientSize = VMWindow.Instance.ClientSize;
      objArray[1] = (object) clientSize.Height;
      Logger.Info("New client size is {0}x{1}", objArray);
      Logger.Info("ResizeFrontendWindow_Windowed DONE");
    }
  }
}
