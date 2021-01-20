// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.JsonExtensions
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BlueStacks.Common
{
  public static class JsonExtensions
  {
    public static IEnumerable<KeyValuePair<string, string>> ToStringStringEnumerableKvp(
      this JToken obj)
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      if (obj != null)
      {
        foreach (KeyValuePair<string, string> keyValuePair in obj.ToObject<Dictionary<string, string>>())
          dictionary.Add(keyValuePair.Key, keyValuePair.Value);
      }
      return (IEnumerable<KeyValuePair<string, string>>) dictionary;
    }

    public static IDictionary<string, T> ToDictionary<T>(this JToken obj)
    {
      Dictionary<string, T> dictionary = new Dictionary<string, T>();
      if (obj != null)
      {
        foreach (KeyValuePair<string, T> keyValuePair in (IEnumerable<KeyValuePair<string, T>>) obj.ToObject<IDictionary<string, T>>())
          dictionary.Add(keyValuePair.Key, keyValuePair.Value);
      }
      return (IDictionary<string, T>) dictionary;
    }

    public static SerializableDictionary<string, T> ToSerializableDictionary<T>(
      this JToken obj)
    {
      SerializableDictionary<string, T> serializableDictionary = new SerializableDictionary<string, T>();
      if (obj != null)
      {
        foreach (KeyValuePair<string, T> keyValuePair in (Dictionary<string, T>) obj.ToObject<SerializableDictionary<string, T>>())
          serializableDictionary.Add(keyValuePair.Key, keyValuePair.Value);
      }
      return serializableDictionary;
    }

    public static IEnumerable<string> ToIenumerableString(this JToken obj)
    {
      return obj != null ? (IEnumerable<string>) obj.ToObject<List<string>>() : (IEnumerable<string>) null;
    }

    public static bool AssignIfContains<T>(this JToken resJson, string key, System.Action<T> setter)
    {
      if (resJson == null || resJson[(object) key] == null || setter == null)
        return false;
      setter(resJson.Value<T>((object) key));
      return true;
    }

    public static void AssignStringIfContains(this JToken resJson, string key, ref string result)
    {
      if (resJson == null || resJson[(object) key] == null)
        return;
      result = resJson[(object) key].ToString();
    }

    public static void AssignDoubleIfContains(this JToken resJson, string key, ref double result)
    {
      if (resJson == null || resJson[(object) key] == null)
        return;
      result = resJson[(object) key].ToObject<double>();
    }

    public static bool IsNullOrEmptyBrackets(string str)
    {
      str = Regex.Replace(str, "\\s+", "");
      return string.IsNullOrEmpty(str) || string.Compare(str, "{}", StringComparison.OrdinalIgnoreCase) == 0;
    }

    public static string GetValue(this JToken obj, string key)
    {
      return obj != null && obj[(object) key] != null ? obj[(object) key].ToString() : string.Empty;
    }

    public static bool IsNullOrEmpty(this JToken token)
    {
      return token == null || token.Type == JTokenType.Array && !token.HasValues || (token.Type == JTokenType.Object && !token.HasValues || token.Type == JTokenType.String && string.IsNullOrEmpty(token.ToString())) || token.Type == JTokenType.Null;
    }
  }
}
