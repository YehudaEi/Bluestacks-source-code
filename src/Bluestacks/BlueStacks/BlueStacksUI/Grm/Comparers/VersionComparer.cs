// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.Grm.Comparers.VersionComparer
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
  internal class VersionComparer : IGrmOperatorComparer<System.Version>
  {
    public bool Contains(System.Version left, string right)
    {
      return left.ToString().Contains(right, StringComparison.InvariantCultureIgnoreCase);
    }

    public bool Equal(System.Version left, string right)
    {
      System.Version version = new System.Version(right);
      return left == version;
    }

    public bool GreaterThan(System.Version left, string right)
    {
      System.Version version = new System.Version(right);
      return left > version;
    }

    public bool GreaterThanEqual(System.Version left, string right)
    {
      System.Version version = new System.Version(right);
      return left >= version;
    }

    public bool In(System.Version left, string right)
    {
      return ((IEnumerable<string>) right.Split(',')).Select<string, System.Version>((Func<string, System.Version>) (_ => new System.Version(_.Trim()))).ToList<System.Version>().Contains(left);
    }

    public bool LessThan(System.Version left, string right)
    {
      System.Version version = new System.Version(right);
      return left < version;
    }

    public bool LessThanEqual(System.Version left, string right)
    {
      System.Version version = new System.Version(right);
      return left <= version;
    }

    public bool LikeRegex(System.Version left, string right, string contextJson)
    {
      JObject jobject = JsonConvert.DeserializeObject(contextJson, Utils.GetSerializerSettings()) as JObject;
      int num = 1;
      if (jobject != null && jobject.ContainsKey("regexOptions") && !string.IsNullOrEmpty(jobject["regexOptions"].Value<string>()))
        num = int.Parse(jobject["regexOptions"].Value<string>(), (IFormatProvider) CultureInfo.InvariantCulture);
      return new Regex(right, (RegexOptions) num).IsMatch(left.ToString());
    }

    public bool NotEqual(System.Version left, string right)
    {
      System.Version version = new System.Version(right);
      return left != version;
    }

    public bool NotIn(System.Version left, string right)
    {
      return !((IEnumerable<string>) right.Split(',')).Select<string, System.Version>((Func<string, System.Version>) (_ => new System.Version(_.Trim()))).ToList<System.Version>().Contains(left);
    }

    public bool StartsWith(System.Version left, string right, string contextJson)
    {
      JObject jobject = JsonConvert.DeserializeObject(contextJson, Utils.GetSerializerSettings()) as JObject;
      int num = 3;
      if (jobject != null && jobject.ContainsKey("stringComparison") && !string.IsNullOrEmpty(jobject["stringComparison"].Value<string>()))
        num = int.Parse(jobject["stringComparison"].Value<string>(), (IFormatProvider) CultureInfo.InvariantCulture);
      return left.ToString().StartsWith(right, (StringComparison) num);
    }
  }
}
