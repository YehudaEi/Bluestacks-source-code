// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.SearchRecommendation
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System;
using System.IO;

namespace BlueStacks.BlueStacksUI
{
  public class SearchRecommendation
  {
    public SerializableDictionary<string, string> ExtraPayload { get; set; } = new SerializableDictionary<string, string>();

    public string IconId { get; set; }

    public string ImagePath { get; set; } = string.Empty;

    internal void DeleteFile()
    {
      try
      {
        File.Delete(this.ImagePath);
      }
      catch (Exception ex)
      {
        Logger.Error("Couldn't delete SearchRecommendation file: " + this.ImagePath);
        Logger.Error(ex.ToString());
      }
    }
  }
}
