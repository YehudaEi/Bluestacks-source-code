// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.Grm.GrmMessageWindow
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using Newtonsoft.Json;
using System.Collections.Generic;

namespace BlueStacks.BlueStacksUI.Grm
{
  public class GrmMessageWindow
  {
    [JsonProperty(PropertyName = "messageType")]
    public string MessageType { get; set; } = "None";

    [JsonProperty(PropertyName = "headerStringKey")]
    public string HeaderStringKey { get; set; } = string.Empty;

    [JsonProperty(PropertyName = "messageStringKey")]
    public string MessageStringKey { get; set; } = string.Empty;

    [JsonProperty(PropertyName = "dontShowOption")]
    public bool DontShowOption { get; set; }

    [JsonProperty(PropertyName = "buttons")]
    public List<GrmMessageButton> Buttons { get; set; } = new List<GrmMessageButton>();
  }
}
