// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.AppRecommendation
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using Newtonsoft.Json;
using System;
using System.IO;

namespace BlueStacks.BlueStacksUI
{
  public class AppRecommendation
  {
    [JsonProperty(PropertyName = "extra_payload")]
    public SerializableDictionary<string, string> ExtraPayload { get; set; } = new SerializableDictionary<string, string>();

    [JsonProperty(PropertyName = "app_icon_id")]
    public string IconId { get; set; }

    [JsonProperty(PropertyName = "app_icon")]
    public string Icon { get; set; }

    [JsonProperty(PropertyName = "game_genre")]
    public string GameGenre { get; set; }

    [JsonProperty(PropertyName = "app_pkg")]
    public string AppPackage { get; set; }

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
