// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.TabChangeEventArgs
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using System;

namespace BlueStacks.BlueStacksUI
{
  public class TabChangeEventArgs : EventArgs
  {
    public string AppName { get; set; } = string.Empty;

    public string PackageName { get; set; } = string.Empty;

    public TabType TabType { get; set; }

    public TabChangeEventArgs(string appName, string packageName, TabType tabType)
    {
      this.AppName = appName;
      this.PackageName = packageName;
      this.TabType = tabType;
    }
  }
}
