// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.Grm.GrmRuleSet
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using Newtonsoft.Json;
using System.Collections.Generic;

namespace BlueStacks.BlueStacksUI.Grm
{
  public class GrmRuleSet
  {
    public string RuleId { get; set; }

    public string Description { get; set; }

    [JsonProperty(PropertyName = "rules")]
    public List<GrmRule> Rules { get; set; } = new List<GrmRule>();

    [JsonProperty(PropertyName = "messageWindow")]
    public GrmMessageWindow MessageWindow { get; set; }
  }
}
