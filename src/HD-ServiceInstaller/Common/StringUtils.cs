// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.StringUtils
// Assembly: HD-ServiceInstaller, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 15F93427-26B3-4C7E-BAB1-0A00688BC4D4
// Assembly location: C:\Program Files\BlueStacks\HD-ServiceInstaller.exe

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
