// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.QuestRule
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using Newtonsoft.Json;

namespace BlueStacks.BlueStacksUI
{
  [JsonObject(MemberSerialization.OptIn)]
  public class QuestRule
  {
    [JsonProperty("rule_id")]
    public string RuleId { get; set; }

    [JsonProperty("app_pkg", NullValueHandling = NullValueHandling.Ignore)]
    public string AppPackage { get; set; } = string.Empty;

    [JsonProperty("usage_time", NullValueHandling = NullValueHandling.Ignore)]
    public int AppUsageTime { get; set; }

    [JsonProperty("user_interactions", NullValueHandling = NullValueHandling.Ignore)]
    public int MinUserInteraction { get; set; }

    [JsonProperty("recurring", NullValueHandling = NullValueHandling.Ignore)]
    public bool IsRecurring { get; set; }

    [JsonProperty("num_of_occurances", NullValueHandling = NullValueHandling.Ignore)]
    public int RecurringCount { get; set; }

    [JsonProperty("cloud_handler", NullValueHandling = NullValueHandling.Ignore)]
    public string CloudHandler { get; set; }
  }
}
