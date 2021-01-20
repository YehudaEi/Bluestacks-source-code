// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.Grm.Comparers.GenericComparer`1
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
  internal class GenericComparer<T> : IGrmOperatorComparer<T> where T : IComparable<T>, IConvertible
  {
    public bool Contains(T left, string right)
    {
      return left.ToString().Contains(right, StringComparison.InvariantCultureIgnoreCase);
    }

    public bool Equal(T left, string right)
    {
      T y = (T) Convert.ChangeType((object) right, typeof (T), (IFormatProvider) CultureInfo.InvariantCulture);
      return EqualityComparer<T>.Default.Equals(left, y);
    }

    public bool GreaterThan(T left, string right)
    {
      T other = (T) Convert.ChangeType((object) right, typeof (T), (IFormatProvider) CultureInfo.InvariantCulture);
      return left.CompareTo(other) > 0;
    }

    public bool GreaterThanEqual(T left, string right)
    {
      T other = (T) Convert.ChangeType((object) right, typeof (T), (IFormatProvider) CultureInfo.InvariantCulture);
      return left.CompareTo(other) >= 0;
    }

    public bool In(T left, string right)
    {
      return ((IEnumerable<string>) right.Split(',')).Select<string, T>((Func<string, T>) (element => (T) Convert.ChangeType((object) element.Trim(), typeof (T), (IFormatProvider) CultureInfo.InvariantCulture))).ToList<T>().Contains(left);
    }

    public bool LessThan(T left, string right)
    {
      T other = (T) Convert.ChangeType((object) right, typeof (T), (IFormatProvider) CultureInfo.InvariantCulture);
      return left.CompareTo(other) < 0;
    }

    public bool LessThanEqual(T left, string right)
    {
      T other = (T) Convert.ChangeType((object) right, typeof (T), (IFormatProvider) CultureInfo.InvariantCulture);
      return left.CompareTo(other) <= 0;
    }

    public bool LikeRegex(T left, string right, string contextJson)
    {
      JObject jobject = JsonConvert.DeserializeObject(contextJson, Utils.GetSerializerSettings()) as JObject;
      int num = 1;
      if (jobject != null && jobject.ContainsKey("regexOptions") && !string.IsNullOrEmpty(jobject["regexOptions"].Value<string>()))
        num = int.Parse(jobject["regexOptions"].Value<string>(), (IFormatProvider) CultureInfo.InvariantCulture);
      return new Regex(right, (RegexOptions) num).IsMatch(left.ToString());
    }

    public bool NotEqual(T left, string right)
    {
      T y = (T) Convert.ChangeType((object) right, typeof (T), (IFormatProvider) CultureInfo.InvariantCulture);
      return !EqualityComparer<T>.Default.Equals(left, y);
    }

    public bool NotIn(T left, string right)
    {
      return !((IEnumerable<string>) right.Split(',')).Select<string, T>((Func<string, T>) (element => (T) Convert.ChangeType((object) element.Trim(), typeof (T), (IFormatProvider) CultureInfo.InvariantCulture))).ToList<T>().Contains(left);
    }

    public bool StartsWith(T left, string right, string contextJson)
    {
      JObject jobject = JsonConvert.DeserializeObject(contextJson, Utils.GetSerializerSettings()) as JObject;
      int num = 3;
      if (jobject != null && jobject.ContainsKey("stringComparison") && !string.IsNullOrEmpty(jobject["stringComparison"].Value<string>()))
        num = int.Parse(jobject["stringComparison"].Value<string>(), (IFormatProvider) CultureInfo.InvariantCulture);
      return left.ToString().StartsWith(right, (StringComparison) num);
    }
  }
}
