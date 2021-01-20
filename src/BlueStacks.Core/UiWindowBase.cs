// Decompiled with JetBrains decompiler
// Type: BlueStacks.Core.UiWindowBase
// Assembly: BlueStacks.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C36AABCB-E7F4-4E86-A72F-981C56431F94
// Assembly location: C:\Program Files\BlueStacks\BlueStacks.Core.dll

using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace BlueStacks.Core
{
  public class UiWindowBase : Window, IView
  {
    public virtual bool ShowWithParentWindow { get; set; }

    public bool IsShowGlWindow { get; set; }

    public UiWindowBase()
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
      hwndSource.AddHook(new HwndSourceHook(UiWindowBase.WndProc));
    }

    private static IntPtr WndProc(
      IntPtr hwnd,
      int msg,
      IntPtr wParam,
      IntPtr lParam,
      ref bool handled)
    {
      switch (msg)
      {
        case 260:
          if (wParam == (IntPtr) 18 || wParam == (IntPtr) 121)
            break;
          goto default;
        case 262:
          if (!(wParam == (IntPtr) 32))
            goto default;
          else
            break;
        default:
label_4:
          return IntPtr.Zero;
      }
      handled = true;
      goto label_4;
    }

    public IViewModel ViewModel { get; set; }
  }
}
