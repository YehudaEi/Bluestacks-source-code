// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.Globalization
// Assembly: HD-ServiceInstaller, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 15F93427-26B3-4C7E-BAB1-0A00688BC4D4
// Assembly location: C:\Program Files\BlueStacks\HD-ServiceInstaller.exe

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;

namespace BlueStacks.Common
{
  public static class Globalization
  {
    private static string sUserDefinedDir = (string) RegistryUtils.GetRegistryValue(Strings.RegistryBaseKeyPath, "UserDefinedDir", (object) "", RegistryKeyKind.HKEY_LOCAL_MACHINE);
    private static Dictionary<string, string> sLocalizedStringsDict = (Dictionary<string, string>) null;
    public static readonly Dictionary<string, string> sSupportedLocales = new Dictionary<string, string>()
    {
      {
        "en-US",
        new CultureInfo("en-US").NativeName
      },
      {
        "ar-EG",
        new CultureInfo("ar-EG").NativeName
      },
      {
        "de-DE",
        new CultureInfo("de-DE").NativeName
      },
      {
        "es-ES",
        new CultureInfo("es-ES").NativeName
      },
      {
        "fr-FR",
        new CultureInfo("fr-FR").NativeName
      },
      {
        "it-IT",
        new CultureInfo("it-IT").NativeName
      },
      {
        "ja-JP",
        new CultureInfo("ja-JP").NativeName
      },
      {
        "ko-KR",
        new CultureInfo("ko-KR").NativeName
      },
      {
        "pl-PL",
        new CultureInfo("pl-PL").NativeName
      },
      {
        "pt-BR",
        new CultureInfo("pt-BR").NativeName
      },
      {
        "ru-RU",
        new CultureInfo("ru-RU").NativeName
      },
      {
        "th-TH",
        new CultureInfo("th-TH").NativeName
      },
      {
        "tr-TR",
        new CultureInfo("tr-TR").NativeName
      },
      {
        "vi-VN",
        new CultureInfo("vi-VN").NativeName
      },
      {
        "zh-TW",
        new CultureInfo("zh-TW").NativeName
      }
    };
    public const string DEFAULT_LOCALE = "en-US";
    private static string sLocale;
    private static string sResourceLocation;

    public static void InitLocalization(string resourceLocation = null)
    {
      BlueStacks.Common.Globalization.sResourceLocation = !string.IsNullOrEmpty(resourceLocation) ? resourceLocation : Path.Combine(BlueStacks.Common.Globalization.sUserDefinedDir, "Locales");
      BlueStacks.Common.Globalization.sLocalizedStringsDict = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase);
      BlueStacks.Common.Globalization.sLocale = BlueStacks.Common.Globalization.GetCurrentCultureSupportedLocaleName();
      if (BlueStacks.Common.Globalization.PopulateLocaleStrings(BlueStacks.Common.Globalization.sResourceLocation, BlueStacks.Common.Globalization.sLocalizedStringsDict, "en-US"))
        Logger.Info("Successfully populated {0} strings", (object) "en-US");
      if (string.Compare(BlueStacks.Common.Globalization.sLocale, "en-US", StringComparison.OrdinalIgnoreCase) == 0)
        return;
      bool flag = BlueStacks.Common.Globalization.PopulateLocaleStrings(BlueStacks.Common.Globalization.sResourceLocation, BlueStacks.Common.Globalization.sLocalizedStringsDict, BlueStacks.Common.Globalization.sLocale);
      Logger.Info("Populated strings for {0}: {1}", (object) BlueStacks.Common.Globalization.sLocale, (object) flag);
    }

    private static string GetCurrentCultureSupportedLocaleName()
    {
      string key = Thread.CurrentThread.CurrentCulture.Name;
      if (!BlueStacks.Common.Globalization.sSupportedLocales.ContainsKey(key))
      {
        key = "en-US";
        string str = BlueStacks.Common.Globalization.sSupportedLocales.Keys.FirstOrDefault<string>((Func<string, bool>) (x => x.StartsWith(Thread.CurrentThread.CurrentCulture.Parent.Name, StringComparison.OrdinalIgnoreCase)));
        if (!string.IsNullOrEmpty(str))
          key = str;
      }
      return key;
    }

    public static string FindClosestMatchingLocale(string requestedLocale)
    {
      string str = "en-US";
      Logger.Info("Finding closest locale match to {0}", (object) requestedLocale);
      try
      {
        List<string> list = BlueStacks.Common.Globalization.sSupportedLocales.Keys.ToList<string>();
        bool flag = false;
        string languageNameFromLocale = BlueStacks.Common.Globalization.GetTwoLetterISOLanguageNameFromLocale(requestedLocale);
        string regionFromLocale = BlueStacks.Common.Globalization.GetRegionFromLocale(requestedLocale);
        foreach (string requestedLocale1 in list)
        {
          if (string.Equals(regionFromLocale, BlueStacks.Common.Globalization.GetRegionFromLocale(requestedLocale1), StringComparison.InvariantCultureIgnoreCase))
          {
            Logger.Info("Match found by region: {0}", (object) requestedLocale1);
            str = requestedLocale1;
            flag = true;
            break;
          }
          if (string.Equals(languageNameFromLocale, BlueStacks.Common.Globalization.GetTwoLetterISOLanguageNameFromLocale(requestedLocale1), StringComparison.InvariantCultureIgnoreCase))
          {
            Logger.Info("Match found by ISO language name: {0}", (object) requestedLocale1);
            str = requestedLocale1;
            flag = true;
            break;
          }
        }
        if (!flag)
        {
          Logger.Warning("No locale match could be found, defaulting to: {0}", (object) "en-US");
          str = "en-US";
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Some error occured. Ex: {0}", (object) ex.ToString());
        Logger.Warning("Defaulting to: {0}", (object) "en-US");
        str = "en-US";
      }
      return str;
    }

    public static string GetTwoLetterISOLanguageNameFromLocale(string requestedLocale)
    {
      try
      {
        return new CultureInfo(requestedLocale).TwoLetterISOLanguageName;
      }
      catch
      {
      }
      string[] strArray;
      if (requestedLocale == null)
        strArray = (string[]) null;
      else
        strArray = requestedLocale.Split('-');
      return strArray[0];
    }

    public static string GetRegionFromLocale(string requestedLocale)
    {
      try
      {
        return new RegionInfo(new CultureInfo(requestedLocale).LCID).Name;
      }
      catch
      {
      }
      string[] strArray;
      if (requestedLocale == null)
        strArray = (string[]) null;
      else
        strArray = requestedLocale.Split('-');
      return strArray[strArray.Length - 1];
    }

    public static string GetLocalizedString(string id)
    {
      string str = id?.Trim();
      try
      {
        if (BlueStacks.Common.Globalization.sLocalizedStringsDict == null)
          BlueStacks.Common.Globalization.InitLocalization((string) null);
        if (BlueStacks.Common.Globalization.sLocalizedStringsDict.ContainsKey(id?.ToUpper(CultureInfo.InvariantCulture)))
          str = BlueStacks.Common.Globalization.sLocalizedStringsDict[id?.ToUpper(CultureInfo.InvariantCulture)];
      }
      catch (Exception ex)
      {
        Logger.Warning("Localized string not available for: {0}. Ex: {1}", (object) id, (object) ex.Message);
      }
      return str;
    }

    public static bool PopulateLocaleStrings(
      string resourceLocation,
      Dictionary<string, string> dict,
      string locale)
    {
      try
      {
        string str = Path.Combine(resourceLocation, "i18n." + locale + ".txt");
        if (!File.Exists(str))
        {
          Logger.Info("String file {0} does not exist", (object) str);
          return false;
        }
        BlueStacks.Common.Globalization.FillDictionary(dict, str);
        Logger.Info("Successfully populated {0} strings", (object) locale);
        return true;
      }
      catch (Exception ex)
      {
        Logger.Error("Could not populate localized strings. Error: {0}", (object) ex);
        return false;
      }
    }

    public static void FillDictionary(Dictionary<string, string> dict, string filePath)
    {
      try
      {
        if (dict == null)
          throw new NullReferenceException("Dictionary to fill cannot be null");
        foreach (string readAllLine in File.ReadAllLines(filePath))
        {
          if (readAllLine.IndexOf("=", StringComparison.OrdinalIgnoreCase) != -1)
          {
            string[] strArray = readAllLine.Split('=');
            string str = strArray[1].Trim();
            if (str.Contains("@@STRING_PRODUCT_NAME@@"))
              str = str.Replace("@@STRING_PRODUCT_NAME@@", Strings.ProductDisplayName);
            dict[strArray[0].Trim().ToUpper(CultureInfo.InvariantCulture)] = str;
          }
        }
      }
      catch
      {
        throw;
      }
    }
  }
}
