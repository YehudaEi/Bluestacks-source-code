// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.GenericNotificationItem
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;

namespace BlueStacks.Common
{
  public class GenericNotificationItem
  {
    public string Id { get; set; }

    public string Title { get; set; }

    public string Message { get; set; }

    public NotificationPriority Priority { get; set; } = NotificationPriority.Normal;

    public bool ShowRibbon { get; set; }

    public DateTime CreationTime { get; set; } = DateTime.Now;

    public string NotificationMenuImageUrl { get; set; }

    public string NotificationMenuImageName { get; set; }

    public bool IsRead { get; set; }

    public bool IsShown { get; set; }

    public NotificationPayloadType PayloadType { get; set; }

    public bool IsDeleted { get; set; }

    public bool IsDeferred { get; set; }

    public string DeferredApp { get; set; }

    public long DeferredAppUsage { get; set; }

    public GenericNotificationDesignItem NotificationDesignItem { get; set; }

    public SerializableDictionary<string, string> ExtraPayload { get; set; } = new SerializableDictionary<string, string>();

    public bool IsAndroidNotification { get; set; }

    public bool IsReceivedStatSent { get; set; }

    public string VmName { get; set; } = "Android";

    public string Package { get; set; }
  }
}
