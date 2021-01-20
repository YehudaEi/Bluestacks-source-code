// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.Grm.GrmMessageButton
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using Newtonsoft.Json;
using System.Collections.Generic;

namespace BlueStacks.BlueStacksUI.Grm
{
  public class GrmMessageButton
  {
    [JsonProperty(PropertyName = "buttonColor")]
    public string ButtonColor { get; set; } = "Blue";

    [JsonProperty(PropertyName = "buttonStringKey")]
    public string ButtonStringKey { get; set; } = string.Empty;

    [JsonProperty(PropertyName = "actions")]
    public List<GrmAction> Actions { get; set; } = new List<GrmAction>();
  }
}
