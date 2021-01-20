// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.LocaleStrings
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;

namespace BlueStacks.Common
{
  public static class LocaleStrings
  {
    private static string sResourceLocation;
    private static Dictionary<string, string> sDictLocalizedString;

    public static event EventHandler SourceUpdatedEvent;

    public static Dictionary<string, string> DictLocalizedString
    {
      get
      {
        if (LocaleStrings.sDictLocalizedString == null)
          LocaleStrings.InitLocalization((string) null, "Android", false);
        return LocaleStrings.sDictLocalizedString;
      }
      set
      {
        LocaleStrings.sDictLocalizedString = value;
      }
    }

    public static string Locale { get; set; }

    public static void InitLocalization(
      string localeDir = null,
      string vmName = "Android",
      bool skipLocalePickFromRegistry = false)
    {
      LocaleStrings.sResourceLocation = localeDir != null ? localeDir : Path.Combine(RegistryManager.Instance.UserDefinedDir, "Locales");
      LocaleStrings.sDictLocalizedString = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase);
      LocaleStrings.Locale = LocaleStrings.GetLocaleName(vmName, skipLocalePickFromRegistry);
      BlueStacks.Common.Globalization.PopulateLocaleStrings(LocaleStrings.sResourceLocation, LocaleStrings.sDictLocalizedString, "en-US");
      if (string.Compare(LocaleStrings.Locale, "en-US", StringComparison.OrdinalIgnoreCase) != 0)
        BlueStacks.Common.Globalization.PopulateLocaleStrings(LocaleStrings.sResourceLocation, LocaleStrings.sDictLocalizedString, LocaleStrings.Locale);
      EventHandler sourceUpdatedEvent = LocaleStrings.SourceUpdatedEvent;
      if (sourceUpdatedEvent == null)
        return;
      sourceUpdatedEvent((object) "Locale_Updated", (EventArgs) null);
    }

    public static string GetLocaleName(string vmName, bool skipLocalePickFromRegistry = false)
    {
      string str = skipLocalePickFromRegistry ? (string) null : RegistryManager.Instance.Guest[vmName].Locale;
      if (string.IsNullOrEmpty(str))
      {
        if (Oem.IsOEMDmm)
          return "ja-JP";
        str = BlueStacks.Common.Globalization.FindClosestMatchingLocale(Thread.CurrentThread.CurrentCulture.Name);
      }
      return str;
    }

    public static string GetLocalizedString(string id, string fallbackValue = "")
    {
      if (id == null)
        return string.Empty;
      string str = id.Trim();
      try
      {
        if (LocaleStrings.sDictLocalizedString == null)
          LocaleStrings.InitLocalization((string) null, "Android", false);
        str = !LocaleStrings.sDictLocalizedString.ContainsKey(id.ToUpper(CultureInfo.InvariantCulture)) ? (!string.IsNullOrEmpty(fallbackValue) ? fallbackValue : LocaleStrings.RemoveConstants(id)) : LocaleStrings.sDictLocalizedString[id.ToUpper(CultureInfo.InvariantCulture)];
      }
      catch
      {
        Logger.Warning("Localized string not available for: {0}", (object) id);
      }
      return str;
    }

    internal static string RemoveConstants(string path)
    {
      if (path.Contains(Constants.ImapLocaleStringsConstant))
      {
        path = path.Replace(Constants.ImapLocaleStringsConstant, "");
        path = path.Replace("_", " ");
      }
      else if (path.Contains(Constants.LocaleStringsConstant))
      {
        path = path.Replace(Constants.LocaleStringsConstant, "");
        path = path.Replace("_", " ");
      }
      return path;
    }

    public static bool AppendLocaleIfDoesntExist(string key, string value)
    {
      bool flag = false;
      try
      {
        if (!LocaleStrings.sDictLocalizedString.ContainsKey(key))
        {
          LocaleStrings.sDictLocalizedString.Add(key, value);
          flag = true;
        }
      }
      catch (Exception ex)
      {
        Logger.Warning("Error appending locale entry: {0}" + ex.Message);
      }
      return flag;
    }
  }
}
