// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.NotificationItem
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;

namespace BlueStacks.Common
{
  [Serializable]
  public class NotificationItem
  {
    public string ID { get; set; } = string.Empty;

    public MuteState MuteState { get; set; } = MuteState.AutoHide;

    public DateTime MuteTime { get; set; } = DateTime.MinValue;

    public bool ShowDesktopNotifications { get; set; }

    public NotificationItem(
      string key,
      MuteState state,
      DateTime now,
      bool isShowDesktopNotifications = false)
    {
      this.ID = key;
      this.MuteState = state;
      this.MuteTime = now;
      this.ShowDesktopNotifications = isShowDesktopNotifications;
    }

    public NotificationItem()
    {
    }
  }
}
