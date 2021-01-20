// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.CustomWindow
// Assembly: HD-ServiceInstaller, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 15F93427-26B3-4C7E-BAB1-0A00688BC4D4
// Assembly location: C:\Program Files\BlueStacks\HD-ServiceInstaller.exe

using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace BlueStacks.Common
{
  public class CustomWindow : Window
  {
    private bool mShowWithParentWindow;

    public bool IsClosed { get; private set; }

    public virtual bool ShowWithParentWindow
    {
      get
      {
        return this.mShowWithParentWindow;
      }
      set
      {
        this.mShowWithParentWindow = value;
      }
    }

    public bool IsShowGLWindow { get; set; }

    public CustomWindow()
    {
      this.SetWindowTitle();
      this.SourceInitialized += new EventHandler(this.CustomWindow_SourceInitialized);
    }

    private void SetWindowTitle()
    {
      this.Title = this.GetType().Name;
    }

    private void CustomWindow_SourceInitialized(object sender, EventArgs e)
    {
      RenderHelper.ChangeRenderModeToSoftware(sender);
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
      base.OnSourceInitialized(e);
      if (!(PresentationSource.FromVisual((Visual) this) is HwndSource hwndSource))
        return;
      hwndSource.AddHook(new HwndSourceHook(CustomWindow.WndProc));
    }

    private static IntPtr WndProc(
      IntPtr hwnd,
      int msg,
      IntPtr wParam,
      IntPtr lParam,
      ref bool handled)
    {
      if (msg == 260 && (wParam == (IntPtr) 18 || wParam == (IntPtr) 121))
        handled = true;
      if (msg == 262 && wParam == (IntPtr) 32)
        handled = true;
      return IntPtr.Zero;
    }

    protected override void OnClosed(EventArgs e)
    {
      this.IsClosed = true;
      base.OnClosed(e);
    }
  }
}
