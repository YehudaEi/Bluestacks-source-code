// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.CloudNotificationItem
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

namespace BlueStacks.Common
{
  public class CloudNotificationItem
  {
    public string Title { get; set; } = string.Empty;

    public string Url { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public string ImagePath { get; set; } = string.Empty;

    public bool IsRead { get; set; }

    public CloudNotificationItem()
    {
    }

    public CloudNotificationItem(string title, string content, string imagePath, string url)
    {
      this.Title = title;
      this.Message = content;
      this.ImagePath = imagePath;
      this.Url = url;
    }
  }
}
