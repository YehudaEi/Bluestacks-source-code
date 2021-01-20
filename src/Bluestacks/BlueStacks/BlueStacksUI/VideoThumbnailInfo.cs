// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.VideoThumbnailInfo
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using Newtonsoft.Json;
using System;
using System.IO;

namespace BlueStacks.BlueStacksUI
{
  public class VideoThumbnailInfo
  {
    [JsonProperty(PropertyName = "thumbnail_id")]
    public string ThumbnailId { get; set; }

    [JsonProperty(PropertyName = "thumbnail_url")]
    public string ThumbnailUrl { get; set; }

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Include, PropertyName = "type")]
    public GuidanceVideoType ThumbnailType { get; set; }

    public string ImagePath { get; set; } = string.Empty;

    internal void DeleteFile()
    {
      try
      {
        File.Delete(this.ImagePath);
      }
      catch (Exception ex)
      {
        Logger.Error("Couldn't delete AppRecommendation file: " + this.ImagePath);
        Logger.Error(ex.ToString());
      }
    }
  }
}
