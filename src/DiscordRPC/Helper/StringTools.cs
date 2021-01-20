// Decompiled with JetBrains decompiler
// Type: DiscordRPC.Helper.StringTools
// Assembly: DiscordRPC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B95F11B0-E129-4510-93B3-855D1EDEA68A
// Assembly location: C:\Program Files\BlueStacks\DiscordRPC.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordRPC.Helper
{
  public static class StringTools
  {
    public static string NullEmpty(this string str)
    {
      return !string.IsNullOrEmpty(str) ? str : (string) null;
    }

    public static bool WithinLength(this string str, int bytes)
    {
      return str.WithinLength(bytes, Encoding.UTF8);
    }

    public static bool WithinLength(this string str, int bytes, Encoding encoding)
    {
      return encoding.GetByteCount(str) <= bytes;
    }

    public static string ToCamelCase(this string str)
    {
      if (str == null)
        return (string) null;
      return ((IEnumerable<string>) str.ToLower().ToLower().Split(new string[2]
      {
        "_",
        " "
      }, StringSplitOptions.RemoveEmptyEntries)).Select<string, string>((Func<string, string>) (s => char.ToUpper(s[0]).ToString() + s.Substring(1, s.Length - 1))).Aggregate<string, string>(string.Empty, (Func<string, string, string>) ((s1, s2) => s1 + s2));
    }

    public static string ToSnakeCase(this string str)
    {
      return str == null ? (string) null : string.Concat(str.Select<char, string>((Func<char, int, string>) ((x, i) => i <= 0 || !char.IsUpper(x) ? x.ToString() : "_" + x.ToString())).ToArray<string>()).ToUpper();
    }
  }
}
