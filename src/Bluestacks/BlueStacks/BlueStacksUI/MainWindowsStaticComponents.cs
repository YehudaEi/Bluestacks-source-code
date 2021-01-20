// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.MainWindowsStaticComponents
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using System;

namespace BlueStacks.BlueStacksUI
{
  internal class MainWindowsStaticComponents
  {
    internal IntPtr mLastMappableWindowHandle = IntPtr.Zero;
    internal AppTabButton mSelectedTabButton;
    internal bool mPreviousSelectedTabWeb;
    internal HomeAppTabButton mSelectedHomeAppTabButton;
    internal bool IsDeleteButtonVisible;

    internal event EventHandler ShowAllUninstallButtons;

    internal event EventHandler HideAllUninstallButtons;

    internal event Action PlayAllGifs;

    internal event Action PauseAllGifs;

    internal void ShowUninstallButtons(bool isShow)
    {
      this.IsDeleteButtonVisible = isShow;
      if (isShow)
      {
        EventHandler uninstallButtons = this.ShowAllUninstallButtons;
        if (uninstallButtons == null)
          return;
        uninstallButtons((object) null, new EventArgs());
      }
      else
      {
        EventHandler uninstallButtons = this.HideAllUninstallButtons;
        if (uninstallButtons == null)
          return;
        uninstallButtons((object) null, new EventArgs());
      }
    }

    internal void PlayPauseGifs(bool isPlay)
    {
      if (isPlay)
      {
        Action playAllGifs = this.PlayAllGifs;
        if (playAllGifs == null)
          return;
        playAllGifs();
      }
      else
      {
        Action pauseAllGifs = this.PauseAllGifs;
        if (pauseAllGifs == null)
          return;
        pauseAllGifs();
      }
    }
  }
}
