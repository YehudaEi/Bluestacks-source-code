// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.HelpArticle
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Reflection;

namespace BlueStacks.BlueStacksUI
{
  public class HelpArticle
  {
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Include, PropertyName = "gamepad")]
    public HelpArticleInfo Gamepad { get; set; }

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Include, PropertyName = "moba")]
    public HelpArticleInfo Moba { get; set; }

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Include, PropertyName = "pan")]
    public HelpArticleInfo Pan { get; set; }

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Include, PropertyName = "special")]
    public HelpArticleInfo Special { get; set; }

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Include, PropertyName = "default")]
    public HelpArticleInfo Default { get; set; }

    [JsonProperty(PropertyName = "package")]
    public string Package { get; set; }

    [JsonProperty(PropertyName = "schemespecific")]
    public Dictionary<string, HelpArticleInfo> SchemeSpecific { get; set; } = new Dictionary<string, HelpArticleInfo>();

    public object this[string propertyName]
    {
      get
      {
        PropertyInfo property = typeof (HelpArticle).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
        return (object) property == null ? (object) null : property.GetValue((object) this, (object[]) null);
      }
    }
  }
}
