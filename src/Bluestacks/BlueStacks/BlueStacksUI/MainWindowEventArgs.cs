// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.MainWindowEventArgs
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using System;

namespace BlueStacks.BlueStacksUI
{
  internal class MainWindowEventArgs
  {
    internal class CursorLockChangedEventArgs : EventArgs
    {
      public bool IsLocked { get; set; }
    }

    internal class FullScreenChangedEventArgs : EventArgs
    {
      public bool IsFullscreen { get; set; }
    }

    internal class FrontendGridVisibilityChangedEventArgs : EventArgs
    {
      public bool IsVisible { get; set; }
    }

    internal class BrowserOTSCompletedCallbackEventArgs : EventArgs
    {
      internal string CallbackFunction { get; set; }
    }
  }
}
