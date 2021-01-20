// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.BootPromotion
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System;
using System.IO;

namespace BlueStacks.BlueStacksUI
{
  [Serializable]
  public class BootPromotion
  {
    public int Order { get; set; }

    public SerializableDictionary<string, string> ExtraPayload { get; set; } = new SerializableDictionary<string, string>();

    public string Id { get; set; }

    public string ButtonText { get; set; } = string.Empty;

    public string ImagePath { get; set; } = string.Empty;

    public string ImageUrl { get; set; } = string.Empty;

    public string ThemeEnabled { get; set; } = string.Empty;

    public string ThemeName { get; set; } = string.Empty;

    public string PromoBtnClickStatusText { get; set; } = string.Empty;

    internal void DeleteFile()
    {
      try
      {
        File.Delete(this.ImagePath);
      }
      catch (Exception ex)
      {
        Logger.Error("Couldn't delete bootpromo file: " + this.ImagePath);
        Logger.Error(ex.ToString());
      }
    }
  }
}
