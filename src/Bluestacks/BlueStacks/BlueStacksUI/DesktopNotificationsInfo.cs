// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.DesktopNotificationsInfo
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System;

namespace BlueStacks.BlueStacksUI
{
  [Serializable]
  public class DesktopNotificationsInfo
  {
    public AppPackageListObject ChatApplicationPackages { get; set; } = new AppPackageListObject();
  }
}
