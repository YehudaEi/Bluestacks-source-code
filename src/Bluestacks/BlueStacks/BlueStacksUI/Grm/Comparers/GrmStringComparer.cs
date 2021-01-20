// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.Grm.Comparers.GrmStringComparer
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
  internal class GrmStringComparer : IGrmOperatorComparer<string>
  {
    public bool Contains(string left, string right)
    {
      return left.Contains(right, StringComparison.InvariantCultureIgnoreCase);
    }

    public bool Equal(string left, string right)
    {
      return left == right;
    }

    public bool GreaterThan(string left, string right)
    {
      throw new ArgumentException("Operator GreaterThan is not supported with string expression");
    }

    public bool GreaterThanEqual(string left, string right)
    {
      throw new ArgumentException("Operator GreaterThanEqual is not supported with string expression");
    }

    public bool In(string left, string right)
    {
      return ((IEnumerable<string>) right.Split(',')).Select<string, string>((Func<string, string>) (_ => _.Trim())).ToList<string>().Contains<string>(left.Trim(), (IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase);
    }

    public bool LessThan(string left, string right)
    {
      throw new ArgumentException("Operator LessThan is not supported with string expression");
    }

    public bool LessThanEqual(string left, string right)
    {
      throw new ArgumentException("Operator LessThanEqual is not supported with string expression");
    }

    public bool LikeRegex(string left, string right, string contextJson)
    {
      JObject jobject = JsonConvert.DeserializeObject(contextJson, Utils.GetSerializerSettings()) as JObject;
      int num = 1;
      if (jobject != null && jobject.ContainsKey("regexOptions") && !string.IsNullOrEmpty(jobject["regexOptions"].Value<string>()))
        num = int.Parse(jobject["regexOptions"].Value<string>(), (IFormatProvider) CultureInfo.InvariantCulture);
      return new Regex(right, (RegexOptions) num).IsMatch(left);
    }

    public bool NotEqual(string left, string right)
    {
      return left != right;
    }

    public bool NotIn(string left, string right)
    {
      return !((IEnumerable<string>) right.Split(',')).Select<string, string>((Func<string, string>) (_ => _.Trim())).ToList<string>().Contains<string>(left.Trim(), (IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase);
    }

    public bool StartsWith(string left, string right, string contextJson)
    {
      JObject jobject = JsonConvert.DeserializeObject(contextJson, Utils.GetSerializerSettings()) as JObject;
      int num = 3;
      if (jobject != null && jobject.ContainsKey("stringComparison") && !string.IsNullOrEmpty(jobject["stringComparison"].Value<string>()))
        num = int.Parse(jobject["stringComparison"].Value<string>(), (IFormatProvider) CultureInfo.InvariantCulture);
      return left.StartsWith(right, (StringComparison) num);
    }
  }
}
