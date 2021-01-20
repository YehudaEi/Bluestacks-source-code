// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.Grm.Comparers.StringListComparer
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace BlueStacks.BlueStacksUI.Grm.Comparers
{
  internal class StringListComparer : IGrmOperatorComparer<List<string>>
  {
    public bool Contains(List<string> left, string right)
    {
      foreach (string source in left)
      {
        if (source.Contains(right, StringComparison.InvariantCultureIgnoreCase))
          return true;
      }
      return false;
    }

    public bool Equal(List<string> left, string right)
    {
      foreach (string str in left)
      {
        if (str.Equals(right, StringComparison.InvariantCultureIgnoreCase))
          return true;
      }
      return false;
    }

    public bool GreaterThan(List<string> left, string right)
    {
      throw new ArgumentException("Operator GreaterThan is not supported with list of string expression");
    }

    public bool GreaterThanEqual(List<string> left, string right)
    {
      throw new ArgumentException("Operator GreaterThanEqual is not supported with list of string expression");
    }

    public bool In(List<string> left, string right)
    {
      return ((IEnumerable<string>) right.Split(',')).Select<string, string>((Func<string, string>) (_ => _.Trim())).ToList<string>().Intersect<string>((IEnumerable<string>) left).Any<string>();
    }

    public bool LessThan(List<string> left, string right)
    {
      throw new ArgumentException("Operator LessThan is not supported with list of string expression");
    }

    public bool LessThanEqual(List<string> left, string right)
    {
      throw new ArgumentException("Operator LessThanEqual is not supported with list of string expression");
    }

    public bool LikeRegex(List<string> left, string right, string contextJson)
    {
      JObject jobject = JsonConvert.DeserializeObject(contextJson, Utils.GetSerializerSettings()) as JObject;
      int num = 1;
      if (jobject != null && jobject.ContainsKey("regexOptions") && !string.IsNullOrEmpty(jobject["regexOptions"].Value<string>()))
        num = int.Parse(jobject["regexOptions"].Value<string>(), (IFormatProvider) CultureInfo.InvariantCulture);
      Regex regex = new Regex(right, (RegexOptions) num);
      foreach (string input in left)
      {
        if (regex.IsMatch(input))
          return true;
      }
      return false;
    }

    public bool NotEqual(List<string> left, string right)
    {
      foreach (string str in left)
      {
        if (str.Equals(right, StringComparison.InvariantCultureIgnoreCase))
          return false;
      }
      return true;
    }

    public bool NotIn(List<string> left, string right)
    {
      return !((IEnumerable<string>) right.Split(',')).Select<string, string>((Func<string, string>) (_ => _.Trim())).ToList<string>().Intersect<string>((IEnumerable<string>) left).Any<string>();
    }

    public bool StartsWith(List<string> left, string right, string contextJson)
    {
      JObject jobject = JsonConvert.DeserializeObject(contextJson, Utils.GetSerializerSettings()) as JObject;
      int num = 3;
      if (jobject != null && jobject.ContainsKey("stringComparison") && !string.IsNullOrEmpty(jobject["stringComparison"].Value<string>()))
        num = int.Parse(jobject["stringComparison"].Value<string>(), (IFormatProvider) CultureInfo.InvariantCulture);
      foreach (string str in left)
      {
        if (str.StartsWith(right, (StringComparison) num))
          return true;
      }
      return false;
    }
  }
}
