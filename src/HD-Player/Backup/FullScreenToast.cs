// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.FullScreenToast
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using BlueStacks.Common;
using System;
using System.Windows.Forms;

namespace BlueStacks.Player
{
  public class FullScreenToast
  {
    private Control mParent;
    private Toast mToast;
    private Timer mTimer;

    public FullScreenToast(Control parent)
    {
      this.mParent = parent;
      this.mTimer = new Timer() { Interval = 5000 };
      this.mTimer.Tick += new EventHandler(this.Timeout);
    }

    public void Show()
    {
      this.Hide();
      this.mToast = new Toast(this.mParent, LocaleStrings.GetLocalizedString("STRING_FULL_SCREEN_TOAST", ""));
      Animate.AnimateWindow(this.mToast.Handle, 500, 262148);
      this.mToast.Show();
      this.mTimer.Start();
    }

    public void Hide()
    {
      this.mTimer.Stop();
      if (this.mToast == null)
        return;
      this.mToast.Hide();
      this.mToast = (Toast) null;
    }

    private void Timeout(object obj, EventArgs evt)
    {
      this.mTimer.Stop();
      int dwFlags = 327688;
      if (this.mToast == null)
        return;
      Animate.AnimateWindow(this.mToast.Handle, 500, dwFlags);
      this.mToast.Hide();
    }
  }
}
