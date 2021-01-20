// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.WindowWndProcHandler
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace BlueStacks.BlueStacksUI
{
  internal class WindowWndProcHandler
  {
    internal bool IsResizingEnabled = true;
    internal bool IsMinMaxEnabled = true;
    private const int ABM_GETTASKBARPOS = 5;
    internal bool mAdjustingWidth;
    private MainWindow mWindowInstance;
    private RawInputClass mRawInput;
    private HwndSource _hwndSource;
    internal static bool isLogWndProc;
    private const int MONITOR_DEFAULTTOPRIMARY = 1;

    internal WindowWndProcHandler(MainWindow window)
    {
      this.mWindowInstance = window;
      this.mWindowInstance.ResizeBegin += new EventHandler(this.mWindowInstance.MainWindow_ResizeBegin);
      this.mWindowInstance.ResizeEnd += new EventHandler(this.mWindowInstance.MainWindow_ResizeEnd);
      this.mWindowInstance.SourceInitialized += new EventHandler(this.Instance_SourceInitialized);
      WindowWndProcHandler.SetMenuDropDownAlignment();
    }

    private void Instance_SourceInitialized(object sender, EventArgs e)
    {
      this._hwndSource = (HwndSource) PresentationSource.FromVisual((Visual) this.mWindowInstance);
      this._hwndSource.AddHook(new HwndSourceHook(this.WndProc));
    }

    internal void AddRawInputHandler()
    {
      try
      {
        if (PromotionObject.Instance == null || !PromotionObject.Instance.IsSecurityMetricsEnable)
          return;
        this.mRawInput = new RawInputClass(new WindowInteropHelper((Window) this.mWindowInstance).Handle);
        Logger.Info("Adding raw input handle");
      }
      catch (Exception ex)
      {
        Logger.Error("Error while adding raw input handle: {0}", (object) ex.ToString());
      }
    }

    internal void ResizeRectangle_MouseMove(object sender, MouseEventArgs e)
    {
      if (!this.IsResizingEnabled)
        return;
      string name = (sender as System.Windows.Shapes.Rectangle).Name;
      if (name == null)
        return;
      // ISSUE: reference to a compiler-generated method
      switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(name))
      {
        case 306900080:
          if (!(name == "left"))
            break;
          this.mWindowInstance.Cursor = Cursors.SizeWE;
          break;
        case 591972422:
          if (!(name == "topRight"))
            break;
          this.mWindowInstance.Cursor = Cursors.SizeNESW;
          break;
        case 873742264:
          if (!(name == "bottomRight"))
            break;
          this.mWindowInstance.Cursor = Cursors.SizeNWSE;
          break;
        case 1319594794:
          if (!(name == "bottom"))
            break;
          this.mWindowInstance.Cursor = Cursors.SizeNS;
          break;
        case 2028154341:
          if (!(name == "right"))
            break;
          this.mWindowInstance.Cursor = Cursors.SizeWE;
          break;
        case 2059707271:
          if (!(name == "bottomLeft"))
            break;
          this.mWindowInstance.Cursor = Cursors.SizeNESW;
          break;
        case 2387400333:
          if (!(name == "topLeft"))
            break;
          this.mWindowInstance.Cursor = Cursors.SizeNWSE;
          break;
        case 2802900028:
          if (!(name == "top"))
            break;
          this.mWindowInstance.Cursor = Cursors.SizeNS;
          break;
      }
    }

    internal void ResizeRectangle_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
      if (!this.IsResizingEnabled)
        return;
      e.Handled = true;
      switch ((sender as System.Windows.Shapes.Rectangle).Name)
      {
        case "bottom":
          this.mWindowInstance.Cursor = Cursors.SizeNS;
          this.mAdjustingWidth = false;
          this.ResizeWindow(WindowWndProcHandler.ResizeDirection.Bottom);
          break;
        case "bottomLeft":
          this.mWindowInstance.Cursor = Cursors.SizeNESW;
          this.ResizeWindow(WindowWndProcHandler.ResizeDirection.BottomLeft);
          break;
        case "bottomRight":
          this.mWindowInstance.Cursor = Cursors.SizeNWSE;
          this.ResizeWindow(WindowWndProcHandler.ResizeDirection.BottomRight);
          break;
        case "left":
          this.mWindowInstance.Cursor = Cursors.SizeWE;
          this.mAdjustingWidth = true;
          this.ResizeWindow(WindowWndProcHandler.ResizeDirection.Left);
          break;
        case "right":
          this.mWindowInstance.Cursor = Cursors.SizeWE;
          this.mAdjustingWidth = true;
          this.ResizeWindow(WindowWndProcHandler.ResizeDirection.Right);
          break;
        case "top":
          this.mWindowInstance.Cursor = Cursors.SizeNS;
          this.mAdjustingWidth = false;
          this.ResizeWindow(WindowWndProcHandler.ResizeDirection.Top);
          break;
        case "topLeft":
          this.mWindowInstance.Cursor = Cursors.SizeNWSE;
          this.ResizeWindow(WindowWndProcHandler.ResizeDirection.TopLeft);
          break;
        case "topRight":
          this.mWindowInstance.Cursor = Cursors.SizeNESW;
          this.ResizeWindow(WindowWndProcHandler.ResizeDirection.TopRight);
          break;
        default:
          e.Handled = false;
          break;
      }
    }

    internal void ResizeWindow(WindowWndProcHandler.ResizeDirection direction)
    {
      this.mWindowInstance.ResizeBegin((object) this.mWindowInstance, new EventArgs());
      NativeMethods.SendMessage(this._hwndSource.Handle, 274U, (IntPtr) (int) (61440 + direction), IntPtr.Zero);
      this.mWindowInstance.ResizeEnd((object) this.mWindowInstance, new EventArgs());
    }

    internal System.Drawing.Point GetMousePosition()
    {
      NativeMethods.Win32Point pt = new NativeMethods.Win32Point();
      NativeMethods.GetCursorPos(ref pt);
      return new System.Drawing.Point(pt.X, pt.Y);
    }

    internal IntPtr WndProc(
      IntPtr hwnd,
      int msg,
      IntPtr wParam,
      IntPtr lParam,
      ref bool handled)
    {
      if (WindowWndProcHandler.isLogWndProc)
        Logger.Info("WndProcMessage: " + msg.ToString() + "~~" + wParam.ToString() + "~~" + lParam.ToString() + "~~");
      switch ((WindowWndProcHandler.WM) msg)
      {
        case WindowWndProcHandler.WM.SETFOCUS:
          ThreadPool.QueueUserWorkItem((WaitCallback) (obj => this.mWindowInstance.Dispatcher.Invoke((Delegate) (() =>
          {
            try
            {
              bool flag2 = true;
              foreach (Window ownedWindow in this.mWindowInstance.OwnedWindows)
              {
                if (ownedWindow is CustomWindow customWindow)
                {
                  if (!customWindow.IsShowGLWindow && !KMManager.sIsInScriptEditingMode)
                  {
                    flag2 = false;
                    Logger.Debug("OnFocusChanged window IsShowGLWindow false: " + customWindow.Name);
                  }
                }
                else
                  Logger.Debug("OnFocusChanged Non Custom window found! " + ownedWindow.Name);
              }
              if (flag2 && !this.mWindowInstance.mIsFocusComeFromImap && this.mWindowInstance.AllowFrontendFocusOnClientClick)
                this.mWindowInstance.mFrontendHandler.ShowGLWindow();
              this.mWindowInstance.mIsFocusComeFromImap = false;
            }
            catch
            {
            }
          }))));
          break;
        case WindowWndProcHandler.WM.SYSCOLORCHANGE:
        case WindowWndProcHandler.WM.WININICHANGE:
        case WindowWndProcHandler.WM.DISPLAYCHANGE:
        case WindowWndProcHandler.WM.DEVICECHANGE:
        case WindowWndProcHandler.WM.THEMECHANGED:
          using (new Timer((TimerCallback) (x => WindowWndProcHandler.SetMenuDropDownAlignment()), (object) null, TimeSpan.FromMilliseconds(2.0), TimeSpan.FromMilliseconds(-1.0)))
            break;
        case WindowWndProcHandler.WM.GETMINMAXINFO:
          this.WmGetMinMaxInfo(hwnd, lParam);
          handled = true;
          break;
        case WindowWndProcHandler.WM.WINDOWPOSCHANGING:
          WindowWndProcHandler.WINDOWPOS structure = (WindowWndProcHandler.WINDOWPOS) Marshal.PtrToStructure(lParam, typeof (WindowWndProcHandler.WINDOWPOS));
          if ((structure.flags & 2) != 0 || (Window) HwndSource.FromHwnd(hwnd).RootVisual == null || this.mWindowInstance.WindowState != WindowState.Normal)
            return IntPtr.Zero;
          bool flag1 = true;
          if (this.mWindowInstance.MinWidthScaled > structure.cx)
          {
            structure.cx = this.mWindowInstance.MinWidthScaled;
            structure.cy = (int) this.mWindowInstance.GetHeightFromWidth((double) structure.cx, true, false);
            flag1 = false;
          }
          else if (this.mWindowInstance.MinHeightScaled > structure.cy)
          {
            structure.cy = this.mWindowInstance.MinHeightScaled;
            structure.cx = (int) this.mWindowInstance.GetWidthFromHeight((double) structure.cy, true, false);
            flag1 = false;
          }
          if (structure.cx > this.mWindowInstance.MaxWidthScaled || structure.cy > this.mWindowInstance.MaxHeightScaled)
          {
            structure.cx = this.mWindowInstance.MaxWidthScaled;
            structure.cy = this.mWindowInstance.MaxHeightScaled;
            flag1 = false;
          }
          if (flag1)
          {
            if (this.mAdjustingWidth)
              structure.cy = (int) this.mWindowInstance.GetHeightFromWidth((double) structure.cx, true, false);
            else
              structure.cx = (int) this.mWindowInstance.GetWidthFromHeight((double) structure.cy, true, false);
          }
          Marshal.StructureToPtr((object) structure, lParam, true);
          handled = true;
          break;
        case WindowWndProcHandler.WM.INPUT:
          int num = -1;
          if (this.mRawInput != null)
            num = RawInputClass.GetDeviceID(lParam);
          if (num == 0 && SecurityMetrics.SecurityMetricsInstanceList.ContainsKey(this.mWindowInstance.mVmName))
          {
            SecurityMetrics.SecurityMetricsInstanceList[this.mWindowInstance.mVmName].AddSecurityBreach(SecurityBreach.SYNTHETIC_INPUT, string.Empty);
            break;
          }
          break;
        case WindowWndProcHandler.WM.SYSCOMMAND:
          if (wParam == (IntPtr) 61696)
          {
            handled = true;
            break;
          }
          break;
        case WindowWndProcHandler.WM.ENTERMENULOOP:
          handled = true;
          break;
      }
      return IntPtr.Zero;
    }

    private static void SetMenuDropDownAlignment()
    {
      try
      {
        if (!SystemParameters.MenuDropAlignment)
          return;
        typeof (SystemParameters).GetField("_menuDropAlignment", BindingFlags.Static | BindingFlags.NonPublic).SetValue((object) null, (object) false);
        int num = SystemParameters.MenuDropAlignment ? 1 : 0;
      }
      catch (Exception ex)
      {
        Logger.Error("error setting _menuDropAlignment" + ex.ToString());
      }
    }

    private void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam)
    {
      WindowWndProcHandler.MINMAXINFO structure = (WindowWndProcHandler.MINMAXINFO) Marshal.PtrToStructure(lParam, typeof (WindowWndProcHandler.MINMAXINFO));
      IntPtr hmonitor = NativeMethods.MonitorFromWindow(hwnd, 1);
      if (hmonitor != IntPtr.Zero)
      {
        WindowWndProcHandler.MONITORINFOEX info = new WindowWndProcHandler.MONITORINFOEX()
        {
          cbSize = Marshal.SizeOf(typeof (MONITORINFO))
        };
        NativeMethods.GetMonitorInfo(hmonitor, info);
        IntereopRect rcWork = info.rcWork;
        IntereopRect rcMonitor = info.rcMonitor;
        WindowWndProcHandler.TaskbarLocation taskbarPosition = WindowWndProcHandler.GetTaskbarPosition();
        if (!this.mWindowInstance.mIsFullScreen)
        {
          structure.ptMaxPosition.X = Math.Abs(rcWork.Left - rcMonitor.Left);
          structure.ptMaxPosition.Y = Math.Abs(rcWork.Top - rcMonitor.Top);
          structure.ptMaxSize.X = Math.Abs(rcWork.Width);
          structure.ptMaxSize.Y = Math.Abs(rcWork.Height);
          if (rcWork == rcMonitor)
          {
            switch (taskbarPosition)
            {
              case WindowWndProcHandler.TaskbarLocation.Left:
                structure.ptMaxPosition.X += 2;
                break;
              case WindowWndProcHandler.TaskbarLocation.Top:
                structure.ptMaxPosition.Y += 2;
                break;
              case WindowWndProcHandler.TaskbarLocation.Right:
                structure.ptMaxSize.X -= 2;
                break;
              case WindowWndProcHandler.TaskbarLocation.Bottom:
                structure.ptMaxSize.Y -= 2;
                break;
            }
          }
        }
        else
        {
          structure.ptMaxPosition.X = 0;
          structure.ptMaxPosition.Y = 0;
          structure.ptMaxSize.X = Math.Abs(rcMonitor.Width);
          structure.ptMaxSize.Y = Math.Abs(rcMonitor.Height);
        }
        structure.ptMaxTrackSize.X = structure.ptMaxSize.X;
        structure.ptMaxTrackSize.Y = structure.ptMaxSize.Y;
      }
      Marshal.StructureToPtr((object) structure, lParam, true);
    }

    internal static IntereopRect GetFullscreenMonitorSize(
      IntPtr hwnd,
      bool isWorkAreaRequired = false)
    {
      IntPtr hmonitor = NativeMethods.MonitorFromWindow(hwnd, 1);
      if (!(hmonitor != IntPtr.Zero))
        return new IntereopRect();
      WindowWndProcHandler.MONITORINFOEX info = new WindowWndProcHandler.MONITORINFOEX();
      NativeMethods.GetMonitorInfo(hmonitor, info);
      return isWorkAreaRequired ? info.rcWork : info.rcMonitor;
    }

    private static WindowWndProcHandler.TaskbarLocation GetTaskbarPosition()
    {
      WindowWndProcHandler.TaskbarLocation taskbarLocation = WindowWndProcHandler.TaskbarLocation.None;
      WindowWndProcHandler.APPBARDATA data = new WindowWndProcHandler.APPBARDATA();
      data.cbSize = Marshal.SizeOf((object) data);
      if (NativeMethods.SHAppBarMessage(5, ref data) == IntPtr.Zero)
        return taskbarLocation;
      if (data.rc.Left == data.rc.Top)
      {
        if (data.rc.Right < data.rc.Bottom)
          taskbarLocation = WindowWndProcHandler.TaskbarLocation.Left;
        if (data.rc.Right > data.rc.Bottom)
          taskbarLocation = WindowWndProcHandler.TaskbarLocation.Top;
      }
      if (data.rc.Left > data.rc.Top)
        taskbarLocation = WindowWndProcHandler.TaskbarLocation.Right;
      if (data.rc.Left < data.rc.Top)
        taskbarLocation = WindowWndProcHandler.TaskbarLocation.Bottom;
      return taskbarLocation;
    }

    internal enum ResizeDirection
    {
      Left = 1,
      Right = 2,
      Top = 3,
      TopLeft = 4,
      TopRight = 5,
      Bottom = 6,
      BottomLeft = 7,
      BottomRight = 8,
    }

    private struct WINDOWPOS
    {
      public IntPtr hwnd;
      public IntPtr hwndInsertAfter;
      public int x;
      public int y;
      public int cx;
      public int cy;
      public int flags;
    }

    private enum SWP
    {
      NOMOVE = 2,
    }

    private enum WM
    {
      ACTIVATE = 6,
      SETFOCUS = 7,
      SYSCOLORCHANGE = 21, // 0x00000015
      WININICHANGE = 26, // 0x0000001A
      GETMINMAXINFO = 36, // 0x00000024
      WINDOWPOSCHANGING = 70, // 0x00000046
      DISPLAYCHANGE = 126, // 0x0000007E
      NCCALCSIZE = 131, // 0x00000083
      INPUT = 255, // 0x000000FF
      SYSCOMMAND = 274, // 0x00000112
      ENTERMENULOOP = 529, // 0x00000211
      DEVICECHANGE = 537, // 0x00000219
      EXITSIZEMOVE = 562, // 0x00000232
      THEMECHANGED = 794, // 0x0000031A
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Auto)]
    public class MONITORINFOEX
    {
      public int cbSize = Marshal.SizeOf(typeof (WindowWndProcHandler.MONITORINFOEX));
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
      public char[] szDevice = new char[32];
      public IntereopRect rcMonitor;
      public IntereopRect rcWork;
      public int dwFlags;
    }

    internal struct MINMAXINFO
    {
      public WindowWndProcHandler.POINT ptReserved;
      public WindowWndProcHandler.POINT ptMaxSize;
      public WindowWndProcHandler.POINT ptMaxPosition;
      public WindowWndProcHandler.POINT ptMinTrackSize;
      public WindowWndProcHandler.POINT ptMaxTrackSize;
    }

    public struct POINT
    {
      public int X;
      public int Y;

      public POINT(int x, int y)
      {
        this.X = x;
        this.Y = y;
      }

      public POINT(System.Drawing.Point pt)
        : this(pt.X, pt.Y)
      {
      }

      public static implicit operator System.Drawing.Point(WindowWndProcHandler.POINT p)
      {
        return new System.Drawing.Point(p.X, p.Y);
      }

      public static implicit operator WindowWndProcHandler.POINT(System.Drawing.Point p)
      {
        return new WindowWndProcHandler.POINT(p.X, p.Y);
      }
    }

    internal struct APPBARDATA
    {
      public int cbSize;
      public IntPtr hWnd;
      public int uCallbackMessage;
      public int uEdge;
      public BlueStacks.Common.RECT rc;
      public IntPtr lParam;
    }

    private enum TaskbarLocation
    {
      None,
      Left,
      Top,
      Right,
      Bottom,
    }
  }
}
