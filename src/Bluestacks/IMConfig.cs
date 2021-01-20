// Decompiled with JetBrains decompiler
// Type: IMConfig
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
internal class IMConfig
{
  public MetaData MetaData { get; set; } = new MetaData();

  public List<IMControlScheme> ControlSchemes { get; set; } = new List<IMControlScheme>();

  [JsonIgnore]
  public Dictionary<string, IMControlScheme> ControlSchemesDict { get; private set; } = new Dictionary<string, IMControlScheme>();

  public Dictionary<string, Dictionary<string, string>> Strings { get; set; } = new Dictionary<string, Dictionary<string, string>>();

  [JsonIgnore]
  public IMControlScheme SelectedControlScheme { get; set; } = new IMControlScheme();

  internal string GetUIString(string key)
  {
    string str = key;
    if (this.Strings.ContainsKey(LocaleStrings.Locale) && this.Strings[LocaleStrings.Locale].ContainsKey(key))
      str = this.Strings[LocaleStrings.Locale][key];
    else if (this.Strings.ContainsKey("en-US") && this.Strings["en-US"].ContainsKey(key))
      str = this.Strings["en-US"][key];
    else if (this.Strings.ContainsKey("User-Defined") && this.Strings["User-Defined"].ContainsKey(key))
      str = this.Strings["User-Defined"][key];
    return str;
  }

  internal void AddString(string key)
  {
    if (!this.Strings.ContainsKey("User-Defined"))
      this.Strings.Add("User-Defined", new Dictionary<string, string>());
    this.Strings["User-Defined"][key] = key;
  }

  public IMConfig DeepCopy()
  {
    IMConfig imConfig = (IMConfig) this.MemberwiseClone();
    MetaData metaData = this.MetaData;
    imConfig.MetaData = metaData != null ? metaData.DeepCopy<MetaData>() : (MetaData) null;
    imConfig.ControlSchemes = this.ControlSchemes?.ConvertAll<IMControlScheme>((Converter<IMControlScheme, IMControlScheme>) (cs => cs?.DeepCopy()));
    Dictionary<string, IMControlScheme> controlSchemesDict = this.ControlSchemesDict;
    imConfig.ControlSchemesDict = controlSchemesDict != null ? controlSchemesDict.ToDictionary<KeyValuePair<string, IMControlScheme>, string, IMControlScheme>((Func<KeyValuePair<string, IMControlScheme>, string>) (kvp => kvp.Key), (Func<KeyValuePair<string, IMControlScheme>, IMControlScheme>) (kvp => kvp.Value?.DeepCopy())) : (Dictionary<string, IMControlScheme>) null;
    Dictionary<string, Dictionary<string, string>> strings = this.Strings;
    imConfig.Strings = strings != null ? strings.ToDictionary<KeyValuePair<string, Dictionary<string, string>>, string, Dictionary<string, string>>((Func<KeyValuePair<string, Dictionary<string, string>>, string>) (kvp => kvp.Key), (Func<KeyValuePair<string, Dictionary<string, string>>, Dictionary<string, string>>) (kvp => kvp.Value)) : (Dictionary<string, Dictionary<string, string>>) null;
    imConfig.SelectedControlScheme = this.SelectedControlScheme?.DeepCopy();
    return imConfig;
  }
}
