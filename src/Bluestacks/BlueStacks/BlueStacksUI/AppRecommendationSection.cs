// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.AppRecommendationSection
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;

namespace BlueStacks.BlueStacksUI
{
  public class AppRecommendationSection
  {
    [JsonProperty(PropertyName = "section_header")]
    public string AppSuggestionHeader { get; set; }

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, PropertyName = "client_show_count")]
    [DefaultValue(3)]
    public int ClientShowCount { get; set; } = 3;

    [JsonProperty(PropertyName = "suggested_apps")]
    public List<AppRecommendation> AppSuggestions { get; } = new List<AppRecommendation>();
  }
}
