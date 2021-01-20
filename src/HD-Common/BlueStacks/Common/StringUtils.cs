// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.StringUtils
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace BlueStacks.Common
{
  public static class StringUtils
  {
    public static string GetControlCharFreeString(string s)
    {
      return new string(s.Where<char>((Func<char, bool>) (c => !char.IsControl(c))).ToArray<char>());
    }

    public static string Encode(Dictionary<string, string> data)
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (data != null)
      {
        foreach (KeyValuePair<string, string> keyValuePair in data)
          stringBuilder.AppendFormat("{0}={1}&", (object) keyValuePair.Key, (object) HttpUtility.UrlEncode(keyValuePair.Value));
      }
      return stringBuilder.ToString().TrimEnd('&');
    }
  }
}
