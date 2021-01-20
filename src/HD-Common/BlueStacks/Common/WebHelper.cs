// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.WebHelper
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.Collections.Generic;
using System.Globalization;

namespace BlueStacks.Common
{
  public static class WebHelper
  {
    private static Uri sDefaultCloudHost = new Uri("https://cloud.bluestacks.com");
    private static Uri sRegistryHost = new Uri(RegistryManager.Instance.Host);

    public static string GetServerHost()
    {
      return RegistryManager.Instance.Host + "/bs3";
    }

    public static string GetServerHostForFirebase()
    {
      return "https://us-central1-bluestacks-friends.cloudfunctions.net";
    }

    public static string GetUrlWithParams(
      string url,
      string clientVer = null,
      string engVer = null,
      string userLocale = null)
    {
      string str1 = "bgp";
      string str2 = clientVer ?? RegistryManager.Instance.ClientVersion;
      string str3 = engVer ?? RegistryManager.Instance.Version;
      string userGuid = RegistryManager.Instance.UserGuid;
      string str4 = userLocale ?? RegistryManager.Instance.UserSelectedLocale;
      string partner = RegistryManager.Instance.Partner;
      string campaignMd5 = RegistryManager.Instance.CampaignMD5;
      string str5 = RegistryManager.Instance.InstallationType.ToString();
      string pkgName = GameConfig.Instance.PkgName;
      string webAppVersion = RegistryManager.Instance.WebAppVersion;
      string str6 = "oem=" + str1 + "&prod_ver=" + str2 + "&eng_ver=" + str3 + "&guid=" + userGuid + "&locale=" + str4 + "&launcher_version=" + webAppVersion;
      if (!string.IsNullOrEmpty(partner))
        str6 += "&partner=";
      string str7 = str6 + partner;
      if (!string.IsNullOrEmpty(campaignMd5))
        str7 += "&campaign_md5=";
      string str8 = str7 + campaignMd5;
      Uri uri = new Uri(url);
      if (uri.Host.Equals(WebHelper.sDefaultCloudHost.Host, StringComparison.InvariantCultureIgnoreCase) || uri.Host.Equals(WebHelper.sRegistryHost.Host, StringComparison.InvariantCultureIgnoreCase))
      {
        string registeredEmail = RegistryManager.Instance.RegisteredEmail;
        if (!string.IsNullOrEmpty(registeredEmail))
          str8 += "&email=";
        string str9 = str8 + registeredEmail;
        string token = RegistryManager.Instance.Token;
        if (!string.IsNullOrEmpty(token))
          str9 += "&token=";
        str8 = str9 + token;
      }
      string str10 = str8 + "&installation_type=" + str5;
      if (!string.IsNullOrEmpty(pkgName))
        str10 += "&gaming_pkg_name=";
      string urlOverideParams = str10 + pkgName;
      if (url != null && !url.Contains("://"))
        url = "http://" + url;
      url = HTTPUtils.MergeQueryParams(url, urlOverideParams, true);
      Logger.Debug("Returning updated URL: {0}", (object) url);
      return url;
    }

    public static string GetHelpArticleURL(string articleKey)
    {
      return WebHelper.GetUrlWithParams(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) WebHelper.GetServerHost(), (object) "help_articles"), (string) null, (string) null, (string) null) + "&article=" + articleKey;
    }

    public static Dictionary<string, string> GetCommonPOSTData()
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>()
      {
        {
          "oem",
          "bgp"
        },
        {
          "prod_ver",
          RegistryManager.Instance.ClientVersion
        },
        {
          "eng_ver",
          RegistryManager.Instance.Version
        },
        {
          "guid",
          RegistryManager.Instance.UserGuid
        },
        {
          "locale",
          RegistryManager.Instance.UserSelectedLocale
        },
        {
          "installation_type",
          RegistryManager.Instance.InstallationType.ToString()
        }
      };
      string partner = RegistryManager.Instance.Partner;
      string campaignMd5 = RegistryManager.Instance.CampaignMD5;
      string pkgName = GameConfig.Instance.PkgName;
      string registeredEmail = RegistryManager.Instance.RegisteredEmail;
      string token = RegistryManager.Instance.Token;
      if (!string.IsNullOrEmpty(partner))
        dictionary.Add("partner", partner);
      if (!string.IsNullOrEmpty(campaignMd5))
        dictionary.Add("campaign_md5", campaignMd5);
      if (!string.IsNullOrEmpty(registeredEmail))
        dictionary.Add("email", registeredEmail);
      if (!string.IsNullOrEmpty(token))
        dictionary.Add("token", token);
      if (!string.IsNullOrEmpty(pkgName))
        dictionary.Add("gaming_pkg_name", pkgName);
      return dictionary;
    }
  }
}
