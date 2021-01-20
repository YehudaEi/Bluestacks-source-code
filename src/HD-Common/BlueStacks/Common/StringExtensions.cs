// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.StringExtensions
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.IO;
using System.Text.RegularExpressions;

namespace BlueStacks.Common
{
  public static class StringExtensions
  {
    public static bool IsNullOrWhiteSpace(this string value)
    {
      if (value == null)
        return true;
      for (int index = 0; index < value.Length; ++index)
      {
        if (!char.IsWhiteSpace(value[index]))
          return false;
      }
      return true;
    }

    public static bool IsSubPathOf(this string baseDirPath, string path)
    {
      return Path.GetFullPath(path != null ? path.Replace('/', '\\').WithEnding("\\") : (string) null).StartsWith(Path.GetFullPath(baseDirPath != null ? baseDirPath.Replace('/', '\\').WithEnding("\\") : (string) null), StringComparison.OrdinalIgnoreCase);
    }

    public static string WithEnding(this string str, string ending)
    {
      if (str == null)
        return ending;
      string str1 = str;
      int length1 = 0;
      string str2;
      while (true)
      {
        int num = length1;
        int? length2 = ending?.Length;
        int valueOrDefault = length2.GetValueOrDefault();
        if (num <= valueOrDefault & length2.HasValue)
        {
          str2 = str1 + ending.Right(length1);
          if (!str2.EndsWith(ending, StringComparison.OrdinalIgnoreCase))
            ++length1;
          else
            break;
        }
        else
          goto label_7;
      }
      return str2;
label_7:
      return str1;
    }

    public static string Right(this string value, int length)
    {
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      if (length < 0)
        throw new ArgumentOutOfRangeException(nameof (length), (object) length, "Length is less than zero");
      return length >= value.Length ? value : value.Substring(value.Length - length);
    }

    public static bool IsValidFileName(this string value)
    {
      if (value == null)
        return false;
      value = value.Trim();
      return !string.IsNullOrEmpty(value) && value.IndexOfAny(Path.GetInvalidFileNameChars()) < 0;
    }

    public static bool GetValidFileName(this string value, out string fileName)
    {
      fileName = value;
      if (string.IsNullOrEmpty(fileName))
        return false;
      fileName = fileName.Trim();
      if (string.IsNullOrEmpty(fileName))
        return false;
      for (int startIndex = fileName.IndexOfAny(Path.GetInvalidFileNameChars()); startIndex >= 0; startIndex = fileName.IndexOfAny(Path.GetInvalidFileNameChars()))
        fileName = fileName.Remove(startIndex, 1);
      return !string.IsNullOrEmpty(fileName);
    }

    public static string CombinePathWith(this string path1, string path2)
    {
      return Path.Combine(path1, path2);
    }

    public static bool IsValidPath(string path)
    {
      if (!new Regex("^[a-zA-Z]:\\\\$").IsMatch(path?.Substring(0, 3)) || new Regex("[" + Regex.Escape(new string(Path.GetInvalidPathChars()) + ":/?*\"") + "]").IsMatch(path.Substring(3, path.Length - 3)))
        return false;
      DirectoryInfo directoryInfo = new DirectoryInfo(Path.GetFullPath(path));
      if (!directoryInfo.Exists)
      {
        try
        {
          directoryInfo.Create();
        }
        catch
        {
          return false;
        }
      }
      return true;
    }

    public static T ToEnum<T>(this string value, bool ignoreCase = true)
    {
      return (T) Enum.Parse(typeof (T), value, ignoreCase);
    }

    public static string TrimStart(this string target, string trimString)
    {
      if (string.IsNullOrEmpty(trimString))
        return target;
      string str = target;
      if (target != null)
      {
        while (str.StartsWith(trimString, StringComparison.InvariantCultureIgnoreCase))
          str = str.Substring(trimString.Length);
      }
      return str;
    }
  }
}
