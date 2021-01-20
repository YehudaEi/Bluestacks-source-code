// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.HelpArticleInfo
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using Newtonsoft.Json;

namespace BlueStacks.BlueStacksUI
{
  public class HelpArticleInfo
  {
    [JsonProperty(PropertyName = "url")]
    public string HelpArticleUrl { get; set; }
  }
}
