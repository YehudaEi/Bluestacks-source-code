// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.GameSetting
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using Newtonsoft.Json;
using System.Collections.Generic;

namespace BlueStacks.BlueStacksUI
{
  internal class GameSetting
  {
    [JsonProperty(PropertyName = "setting_type")]
    public string SettingType { get; set; }

    public List<Dictionary<string, object>> SettingsData { get; set; } = new List<Dictionary<string, object>>();
  }
}
