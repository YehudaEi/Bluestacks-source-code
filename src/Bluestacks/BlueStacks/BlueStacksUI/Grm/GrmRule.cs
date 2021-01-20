// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.Grm.GrmRule
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using Newtonsoft.Json;
using System.Collections.Generic;

namespace BlueStacks.BlueStacksUI.Grm
{
  public class GrmRule
  {
    [JsonProperty(PropertyName = "expressions")]
    public List<GrmExpression> Expressions { get; set; } = new List<GrmExpression>();
  }
}
