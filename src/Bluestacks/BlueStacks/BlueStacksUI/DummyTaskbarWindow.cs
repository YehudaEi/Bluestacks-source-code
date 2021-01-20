// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.DummyTaskbarWindow
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using Vanara.PInvoke;

namespace BlueStacks.BlueStacksUI
{
  public class DummyTaskbarWindow : CustomWindow, IComponentConnector
  {
    private static readonly object sync = new object();
    private static IntPtr sThisHandle = IntPtr.Zero;
    private const int WM_DWMSENDICONICTHUMBNAIL = 803;
    internal DummyTaskbarWindow mDummyTaskbarWindow;
    private bool _contentLoaded;

    public string TaskbarThumbnailPath { get; set; }

    public MainWindow ParentWindow { get; set; }

    public DummyTaskbarWindow(MainWindow window)
    {
      this.InitializeComponent();
      this.ParentWindow = window;
    }

    private void DummyTaskbarWindow_Loaded(object sender, RoutedEventArgs e)
    {
      try
      {
        DummyTaskbarWindow.sThisHandle = new WindowInteropHelper((Window) this).Handle;
        int num1 = Marshal.SizeOf((object) 1);
        IntPtr num2 = Marshal.AllocHGlobal(num1);
        Marshal.WriteInt32(num2, 0, 1);
        HWND hwnd = new HWND(DummyTaskbarWindow.sThisHandle);
        DwmApi.DwmSetWindowAttribute(hwnd, DwmApi.DWMWINDOWATTRIBUTE.DWMWA_FORCE_ICONIC_REPRESENTATION, num2, num1);
        DwmApi.DwmSetWindowAttribute(hwnd, DwmApi.DWMWINDOWATTRIBUTE.DWMWA_HAS_ICONIC_BITMAP, num2, num1);
        Marshal.FreeHGlobal(num2);
        HwndSource.FromHwnd(DummyTaskbarWindow.sThisHandle).AddHook(new HwndSourceHook(this.WndProc));
      }
      catch (Exception ex)
      {
        Logger.Warning("Exception in setting window porperties for taskbar thumbnail : " + ex.ToString());
      }
    }

    private IntPtr WndProc(
      IntPtr hwnd,
      int msg,
      IntPtr wParam,
      IntPtr lParam,
      ref bool handled)
    {
      try
      {
        if (msg == 803)
        {
          lock (DummyTaskbarWindow.sync)
          {
            int num1 = lParam.ToInt32() >> 16 & (int) ushort.MaxValue;
            int num2 = lParam.ToInt32() & (int) ushort.MaxValue;
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri(this.TaskbarThumbnailPath);
            bitmapImage.DecodePixelWidth = num1;
            bitmapImage.DecodePixelHeight = num2;
            bitmapImage.EndInit();
            Bitmap bitmap = ImageUtils.BitmapImage2Bitmap(bitmapImage);
            DwmApi.DwmSetIconicThumbnail(new HWND(DummyTaskbarWindow.sThisHandle), (HBITMAP) bitmap.GetHbitmap(), DwmApi.DWM_SETICONICPREVIEW_Flags.DWM_SIT_NONE);
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Warning("Exception in setting taskbar thumbnail : " + ex.ToString());
      }
      return IntPtr.Zero;
    }

    private void DummyTaskbarWindow_Closing(object sender, CancelEventArgs e)
    {
      this.ParentWindow.DummyWindow = (DummyTaskbarWindow) null;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/dummytaskbarwindow.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      if (connectionId == 1)
      {
        this.mDummyTaskbarWindow = (DummyTaskbarWindow) target;
        this.mDummyTaskbarWindow.Loaded += new RoutedEventHandler(this.DummyTaskbarWindow_Loaded);
        this.mDummyTaskbarWindow.Closing += new CancelEventHandler(this.DummyTaskbarWindow_Closing);
      }
      else
        this._contentLoaded = true;
    }
  }
}
