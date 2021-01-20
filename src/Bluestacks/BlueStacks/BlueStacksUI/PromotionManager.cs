// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.PromotionManager
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using System.Xml;
using System.Xml.Serialization;

namespace BlueStacks.BlueStacksUI
{
  internal class PromotionManager
  {
    internal static Dictionary<string, long> combinedPackages = new Dictionary<string, long>();
    private static System.Timers.Timer mQuestTimer = new System.Timers.Timer(15000.0);
    private static Dictionary<string, int> mDictRecurringCount = new Dictionary<string, int>();
    private static List<string> mRuleIdAlreadyPassed = new List<string>();
    internal static Dictionary<GenericNotificationItem, long> sDeferredNotificationsList = new Dictionary<GenericNotificationItem, long>();
    internal static List<GenericNotificationItem> sPassedDeferredNotificationsList = new List<GenericNotificationItem>();

    internal static BootPromotion AddBootPromotion(JToken promoImage)
    {
      BootPromotion bootPromotion = new BootPromotion();
      string url = promoImage.GetValue("image_url");
      bootPromotion.ImageUrl = promoImage.GetValue("image_url");
      bootPromotion.Id = promoImage.GetValue("id");
      string fileNameWithExtension = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_{1}", (object) "BootPromo", (object) bootPromotion.Id);
      bootPromotion.Order = int.Parse(promoImage.GetValue("order"), (IFormatProvider) CultureInfo.InvariantCulture);
      if (!JsonExtensions.IsNullOrEmptyBrackets(promoImage.GetValue("extra_payload")))
      {
        bootPromotion.ExtraPayload.ClearAddRange<string, string>((Dictionary<string, string>) promoImage[(object) "extra_payload"].ToSerializableDictionary<string>());
        PromotionManager.PopulateAndDownloadFavicon((IDictionary<string, string>) bootPromotion.ExtraPayload, fileNameWithExtension + "_" + bootPromotion.Id, false);
      }
      bootPromotion.ButtonText = promoImage.GetValue("button_text");
      bootPromotion.ThemeEnabled = promoImage.GetValue("theme_enabled");
      bootPromotion.ThemeName = promoImage.GetValue("theme_name");
      bootPromotion.ImagePath = Utils.TinyDownloader(url, fileNameWithExtension, RegistryStrings.PromotionDirectory, false);
      bootPromotion.PromoBtnClickStatusText = promoImage.GetValue("promo_button_click_status_text");
      return bootPromotion;
    }

    internal static SearchRecommendation AddSearchRecommendation(
      JToken searchItem)
    {
      SearchRecommendation searchRecommendation = new SearchRecommendation()
      {
        IconId = searchItem.GetValue("app_icon_id")
      };
      string url = searchItem.GetValue("app_icon");
      string fileNameWithExtension = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_{1}", (object) "recommendation", (object) searchRecommendation.IconId);
      searchRecommendation.ImagePath = Utils.TinyDownloader(url, fileNameWithExtension, RegistryStrings.PromotionDirectory, false);
      if (!JsonExtensions.IsNullOrEmptyBrackets(searchItem.GetValue("extra_payload")))
        searchRecommendation.ExtraPayload.ClearAddRange<string, string>((Dictionary<string, string>) searchItem[(object) "extra_payload"].ToSerializableDictionary<string>());
      return searchRecommendation;
    }

    internal static void SendAppUsageStats()
    {
      string urlWithParams = WebHelper.GetUrlWithParams(RegistryManager.Instance.Host + "/bs3/stats/v4/usage", (string) null, (string) null, (string) null);
      string str = AppUsageTimer.DecryptString(RegistryManager.Instance.AInfo);
      if (string.IsNullOrEmpty(str))
        return;
      Dictionary<string, string> data = new Dictionary<string, string>()
      {
        {
          "usage",
          str
        }
      };
      try
      {
        BstHttpClient.Post(urlWithParams, data, (Dictionary<string, string>) null, false, string.Empty, 0, 1, 0, false, "bgp");
        RegistryManager.Instance.AInfo = string.Empty;
      }
      catch (Exception ex)
      {
        Logger.Error(ex.ToString());
        Logger.Error("Post failed. url = {0}", (object) urlWithParams);
      }
    }

    internal static string AddDiscordClientVersionInUrl(string url)
    {
      string str = string.Empty;
      string regPath = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\Discord";
      try
      {
        str = (string) Utils.GetRegistryHKCUValue(regPath, "DisplayVersion", (object) string.Empty);
        if (string.IsNullOrEmpty(str))
          str = (string) Utils.GetRegistryHKLMValue(regPath, "DisplayVersion", (object) string.Empty);
      }
      catch (Exception ex)
      {
        Logger.Error("exception in getting discord client version.." + ex.ToString());
      }
      url += "&discord_version=";
      url += str;
      return url;
    }

    private static Dictionary<string, string> GetPromotionCallData()
    {
      Dictionary<string, string> data = PromotionManager.GetInstalledAppsData();
      Dictionary<string, string> resolutionData = BlueStacksUIUtils.GetResolutionData();
      try
      {
        resolutionData.ToList<KeyValuePair<string, string>>().ForEach((System.Action<KeyValuePair<string, string>>) (kvp => data[kvp.Key] = kvp.Value));
        Logger.Info("RESOLUTION : " + data["resolution"]);
        Logger.Info("RESOLUTION TYPE : " + data["resolution_type"]);
      }
      catch (Exception ex)
      {
        Logger.Error("Merge dictionary failed. Ex : " + ex.ToString());
      }
      return data;
    }

    internal static Dictionary<string, string> GetInstalledAppsData()
    {
      List<AppInfo> list = ((IEnumerable<AppInfo>) new JsonParser("Android").GetAppList()).ToList<AppInfo>();
      JArray jarray1 = new JArray();
      foreach (AppInfo appInfo in list)
      {
        JObject jobject = new JObject();
        string package = appInfo.Package;
        string name = appInfo.Name;
        jobject.Add(package, (JToken) name);
        jarray1.Add((JToken) jobject);
      }
      Dictionary<string, string> dictionary = new Dictionary<string, string>()
      {
        {
          "installed_apps",
          jarray1.ToString(Newtonsoft.Json.Formatting.None)
        }
      };
      dictionary.Add("all_installed_apps", Utils.GetInstalledAppDataFromAllVms());
      dictionary.Add("campaign_json", RegistryManager.Instance.CampaignJson);
      dictionary.Add("email", RegistryManager.Instance.RegisteredEmail);
      if (!string.IsNullOrEmpty(Opt.Instance.Json))
      {
        JObject jobject = JObject.Parse(Opt.Instance.Json);
        if (jobject["fle_pkg"] != null)
          dictionary.Add("fle_packagename", jobject["fle_pkg"].ToString().Trim());
      }
      if (RegistryManager.Instance.IsClientFirstLaunch == 1)
      {
        if (RegistryManager.Instance.IsClientUpgraded)
          dictionary.Add("first_boot_update", bool.TrueString);
        else
          dictionary.Add("first_boot", bool.TrueString);
      }
      try
      {
        string path = Path.Combine(RegistryStrings.PromotionDirectory, "app_suggestion_removed");
        if (File.Exists(path))
        {
          string data = File.ReadAllText(path);
          List<string> stringList = new List<string>();
          if (!string.IsNullOrEmpty(data))
            stringList = PromotionManager.DoDeserialize<List<string>>(data);
          JArray jarray2 = new JArray();
          foreach (string str in stringList)
            jarray2.Add((JToken) str);
          dictionary.Add("cross_promotion_closed_apps_list", jarray2.ToString(Newtonsoft.Json.Formatting.None));
        }
      }
      catch (Exception ex)
      {
        Logger.Warning("Error in adding cross promotion closed app list " + ex.ToString());
        if (!dictionary.ContainsKey("cross_promotion_closed_apps_list"))
          dictionary.Add("cross_promotion_closed_apps_list", "[]");
      }
      return dictionary;
    }

    internal static void ReloadPromotionsAsync()
    {
      ThreadPool.QueueUserWorkItem((WaitCallback) (obj =>
      {
        if (PromotionObject.Instance == null)
          PromotionObject.LoadDataFromFile();
        try
        {
          PromotionManager.SendAppUsageStats();
          PromotionManager.CheckIsUserPremium();
          JToken promotionData = PromotionManager.GetPromotionData();
          if (promotionData != null)
          {
            PromotionManager.SetBootPromotion(promotionData);
            PromotionManager.SetDiscordId(promotionData);
            PromotionManager.SetFeatures(promotionData);
            PromotionManager.SetMyAppsCrossPromotion(promotionData);
            PromotionManager.SetMyAppsBackgroundPromotion(promotionData);
            PromotionManager.SetSearchRecommendations(promotionData);
            PromotionManager.SetAppRecommendations(promotionData);
            PromotionManager.SetStartupTab(promotionData);
            PromotionManager.SetIconOrder(promotionData);
            PromotionManager.ReadQuests(promotionData, false);
            PromotionManager.PopulateAppSpecificRules(promotionData);
            PromotionManager.SetSecurityMetrics(promotionData);
            PromotionManager.SetCustomCursorRuleForApp(promotionData);
          }
          PromotionObject.Save();
          PromotionObject.Instance.PromotionLoaded();
        }
        catch (Exception ex)
        {
          Logger.Info("Error Loading Promotions" + ex.ToString());
        }
      }));
    }

    private static JToken GetPromotionData()
    {
      JToken jtoken = (JToken) null;
      try
      {
        string url = PromotionManager.AddSamsungStoreParamsIfPresent(PromotionManager.AddDiscordClientVersionInUrl(WebHelper.GetUrlWithParams(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) WebHelper.GetServerHost(), (object) "promotions"), (string) null, (string) null, (string) null)));
        string json = BstHttpClient.Post(url, PromotionManager.GetPromotionCallData(), (Dictionary<string, string>) null, false, BlueStacks.Common.Strings.CurrentDefaultVmName, 0, 1, 0, false, "bgp");
        Logger.Debug("Promotion Url: " + url);
        Logger.Debug("Promotion Response: " + json);
        jtoken = JToken.Parse(json);
      }
      catch (Exception ex)
      {
        Logger.Info("Error Getting PromotionData " + ex.ToString());
      }
      return jtoken;
    }

    private static void PopulateAppSpecificRules(JToken res)
    {
      try
      {
        if (JsonExtensions.IsNullOrEmptyBrackets(res.GetValue("macro_rules")))
          return;
        foreach (JToken jtoken in JArray.Parse(res[(object) "macro_rules"].ToString()))
          PromotionObject.Instance.AppSpecificRulesList.Add(jtoken.ToString());
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in PopulateAppSpecificRules: " + ex.ToString());
      }
    }

    private static void SetSearchRecommendations(JToken res)
    {
      try
      {
        if (JsonExtensions.IsNullOrEmptyBrackets(res.GetValue("search_recommendations")))
        {
          foreach (KeyValuePair<string, SearchRecommendation> searchRecommendation in (Dictionary<string, SearchRecommendation>) PromotionObject.Instance.SearchRecommendations)
            searchRecommendation.Value.DeleteFile();
          PromotionObject.Instance.SearchRecommendations.ClearSync<string, SearchRecommendation>();
        }
        else
        {
          SerializableDictionary<string, SearchRecommendation> tempDict = new SerializableDictionary<string, SearchRecommendation>();
          foreach (JToken searchItem in JArray.Parse(res[(object) "search_recommendations"].ToString()).ToObject<List<JToken>>())
          {
            string index = searchItem.GetValue("app_icon_id");
            if (!JsonExtensions.IsNullOrEmptyBrackets(index))
            {
              SearchRecommendation searchRecommendation = !PromotionObject.Instance.SearchRecommendations.ContainsKey(index) ? PromotionManager.AddSearchRecommendation(searchItem) : PromotionObject.Instance.SearchRecommendations[index];
              if (searchRecommendation != null)
                tempDict[searchRecommendation.IconId] = searchRecommendation;
            }
          }
          foreach (string path in PromotionObject.Instance.SearchRecommendations.Values.Select<SearchRecommendation, string>((Func<SearchRecommendation, string>) (_ => _.ImagePath)).Where<string>((Func<string, bool>) (_ => !tempDict.Values.Select<SearchRecommendation, string>((Func<SearchRecommendation, string>) (x => x.ImagePath)).Contains<string>(_))))
          {
            try
            {
              File.Delete(path);
            }
            catch (Exception ex)
            {
            }
          }
          PromotionObject.Instance.SearchRecommendations.ClearAddRange<string, SearchRecommendation>((Dictionary<string, SearchRecommendation>) tempDict);
        }
      }
      catch (Exception ex)
      {
        PromotionObject.Instance.SearchRecommendations.ClearSync<string, SearchRecommendation>();
        Logger.Info("Error Loading Search Recommendations" + ex.ToString());
      }
    }

    private static void SetAppRecommendations(JToken res)
    {
      try
      {
        if (JsonExtensions.IsNullOrEmptyBrackets(res.GetValue("app_recommendations")))
        {
          foreach (AppRecommendation appSuggestion in PromotionObject.Instance.AppRecommendations.AppSuggestions)
            appSuggestion.DeleteFile();
          PromotionObject.Instance.AppRecommendations = new AppRecommendationSection();
        }
        else
        {
          List<AppRecommendationSection> recommendationSectionList = JsonConvert.DeserializeObject<List<AppRecommendationSection>>(res[(object) "app_recommendations"].ToString(), Utils.GetSerializerSettings());
          if (recommendationSectionList != null)
          {
            foreach (AppRecommendation appSuggestion in recommendationSectionList[0].AppSuggestions)
            {
              if (!JsonExtensions.IsNullOrEmptyBrackets(appSuggestion.IconId))
              {
                string icon = appSuggestion.Icon;
                string fileNameWithExtension = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_{1}", (object) "AppRecommendation", (object) appSuggestion.IconId);
                appSuggestion.ImagePath = Utils.TinyDownloader(icon, fileNameWithExtension, RegistryStrings.PromotionDirectory, false);
              }
            }
          }
          PromotionObject.Instance.AppRecommendations = recommendationSectionList[0];
        }
      }
      catch (Exception ex)
      {
        PromotionObject.Instance.AppRecommendations = new AppRecommendationSection();
        Logger.Info("Error Loading App Recommendations" + ex.ToString());
      }
    }

    private static void SetStartupTab(JToken res)
    {
      try
      {
        if (JsonExtensions.IsNullOrEmptyBrackets(res.GetValue("startup_tab")))
        {
          PromotionObject.Instance.StartupTab.ClearSync<string, string>();
        }
        else
        {
          if (JsonExtensions.IsNullOrEmptyBrackets(res[(object) "startup_tab"].GetValue("extra_payload")))
            return;
          PromotionObject.Instance.StartupTab.ClearAddRange<string, string>((Dictionary<string, string>) res[(object) "startup_tab"][(object) "extra_payload"].ToSerializableDictionary<string>());
          PromotionManager.PopulateAndDownloadFavicon((IDictionary<string, string>) PromotionObject.Instance.StartupTab, "startup_favicon", false);
        }
      }
      catch (Exception ex)
      {
        PromotionObject.Instance.StartupTab.ClearSync<string, string>();
        Logger.Error("Exception while setting the startup tab. " + ex.ToString());
      }
    }

    public static void PopulateAndDownloadFavicon(
      IDictionary<string, string> payload,
      string id,
      bool redownload = false)
    {
      if (payload.ContainsKey("click_action_app_icon_id"))
        id += payload["click_action_app_icon_id"];
      if (!payload.ContainsKey("click_action_app_icon_url"))
        return;
      string str = Utils.TinyDownloader(payload["click_action_app_icon_url"], id, RegistryStrings.PromotionDirectory, redownload);
      if (string.IsNullOrEmpty(str))
        return;
      payload["icon_path"] = str;
    }

    private static void SetIconOrder(JToken res)
    {
      try
      {
        if (JsonExtensions.IsNullOrEmptyBrackets(res.GetValue("order")))
        {
          PromotionObject.Instance.SetDefaultOrder(true);
        }
        else
        {
          PromotionManager.SetMyAppsOrder(res[(object) "order"]);
          PromotionManager.SetDockOrder(res[(object) "order"]);
          PromotionManager.SetMoreAppsOrder(res[(object) "order"]);
        }
      }
      catch (Exception ex)
      {
        PromotionObject.Instance.SetDefaultOrder(false);
        Logger.Info("Error Loading icon order" + ex.ToString());
      }
    }

    private static void SetMoreAppsOrder(JToken res)
    {
      try
      {
        if (JsonExtensions.IsNullOrEmptyBrackets(res.GetValue("more_apps_order")))
        {
          PromotionObject.Instance.SetDefaultMoreAppsOrder(true);
        }
        else
        {
          PromotionObject.Instance.MoreAppsDockOrder.ClearSync<string, int>();
          foreach (KeyValuePair<string, int> serializable in (Dictionary<string, int>) res[(object) "more_apps_order"].ToSerializableDictionary<int>())
            PromotionObject.Instance.MoreAppsDockOrder[serializable.Key] = serializable.Value;
        }
      }
      catch (Exception ex)
      {
        PromotionObject.Instance.SetDefaultMoreAppsOrder(true);
        Logger.Info("Error Loading more_apps_order" + ex.ToString());
      }
    }

    private static void SetMyAppsOrder(JToken res)
    {
      try
      {
        if (JsonExtensions.IsNullOrEmptyBrackets(res.GetValue("myapps_order")))
        {
          PromotionObject.Instance.SetDefaultMyAppsOrder(true);
        }
        else
        {
          PromotionObject.Instance.MyAppsOrder.ClearSync<string, int>();
          foreach (KeyValuePair<string, int> serializable in (Dictionary<string, int>) res[(object) "myapps_order"].ToSerializableDictionary<int>())
            PromotionObject.Instance.MyAppsOrder[serializable.Key] = serializable.Value;
        }
      }
      catch (Exception ex)
      {
        PromotionObject.Instance.SetDefaultMyAppsOrder(true);
        Logger.Info("Error Loading My apps order" + ex.ToString());
      }
    }

    private static void SetDiscordId(JToken res)
    {
      try
      {
        if (JsonExtensions.IsNullOrEmptyBrackets(res.GetValue("discord_client_id")))
          return;
        PromotionObject.Instance.DiscordClientID = res.GetValue("discord_client_id");
      }
      catch (Exception ex)
      {
        Logger.Error("Error while getting discord id : {0}", (object) ex.ToString());
      }
    }

    private static void SetFeatures(JToken res)
    {
      try
      {
        if (!JsonExtensions.IsNullOrEmptyBrackets(res.GetValue("is_root_access_enabled")))
          PromotionObject.Instance.IsRootAccessEnabled = res[(object) "is_root_access_enabled"].ToObject<bool>();
        if (!JsonExtensions.IsNullOrEmptyBrackets(res.GetValue("is_timeline_stats4_enabled")))
          RegistryManager.Instance.IsTimelineStats4Enabled = res[(object) "is_timeline_stats4_enabled"].ToObject<bool>();
      }
      catch (Exception ex)
      {
        Logger.Error("Error in SetFeatures: {0}", (object) ex);
      }
      try
      {
        if (JsonExtensions.IsNullOrEmptyBrackets(res.GetValue("geo")))
          return;
        string str = res[(object) "geo"].ToString();
        if (string.IsNullOrEmpty(str))
          return;
        RegistryManager.Instance.Geo = str;
      }
      catch (Exception ex)
      {
        Logger.Error("Error while getting geo feature: {0}", (object) ex);
      }
    }

    private static void SetDockOrder(JToken res)
    {
      try
      {
        if (JsonExtensions.IsNullOrEmptyBrackets(res.GetValue("dock_order")))
        {
          PromotionObject.Instance.SetDefaultDockOrder(true);
        }
        else
        {
          PromotionObject.Instance.DockOrder.ClearSync<string, int>();
          JToken re = res[(object) "dock_order"];
          SerializableDictionary<string, int> serializableDictionary = re != null ? re.ToSerializableDictionary<int>() : (SerializableDictionary<string, int>) null;
          if (serializableDictionary != null && serializableDictionary.Count > 0)
          {
            foreach (KeyValuePair<string, int> keyValuePair in (Dictionary<string, int>) serializableDictionary)
              PromotionObject.Instance.DockOrder[keyValuePair.Key] = keyValuePair.Value;
          }
          else
            PromotionObject.Instance.SetDefaultDockOrder(true);
        }
      }
      catch (Exception ex)
      {
        PromotionObject.Instance.SetDefaultDockOrder(true);
        Logger.Info("Error Loading dock order" + ex.ToString());
      }
    }

    private static void SetBootPromotion(JToken res)
    {
      try
      {
        if (JsonExtensions.IsNullOrEmptyBrackets(res.GetValue("boot_promotion_obj")))
        {
          foreach (KeyValuePair<string, BootPromotion> dictBootPromotion in (Dictionary<string, BootPromotion>) PromotionObject.Instance.DictBootPromotions)
            dictBootPromotion.Value.DeleteFile();
          PromotionObject.Instance.DictBootPromotions.ClearSync<string, BootPromotion>();
        }
        else
        {
          JToken jtoken = JToken.Parse(res.GetValue("boot_promotion_obj"));
          if (jtoken[(object) "boot_promotion_display_time"] != null)
            PromotionObject.Instance.BootPromoDisplaytime = jtoken[(object) "boot_promotion_display_time"].ToObject<int>();
          SerializableDictionary<string, BootPromotion> serializableDictionary = new SerializableDictionary<string, BootPromotion>();
          foreach (JToken promoImage in JArray.Parse(jtoken[(object) "boot_promotion_images"].ToString()))
          {
            string index = promoImage.GetValue("id");
            if (!JsonExtensions.IsNullOrEmptyBrackets(index))
            {
              BootPromotion bootPromotion = !PromotionObject.Instance.DictBootPromotions.ContainsKey(index) ? PromotionManager.AddBootPromotion(promoImage) : PromotionObject.Instance.DictBootPromotions[index];
              if (bootPromotion != null)
                serializableDictionary[bootPromotion.Id] = bootPromotion;
            }
          }
          PromotionObject.Instance.DictBootPromotions.ClearAddRange<string, BootPromotion>((Dictionary<string, BootPromotion>) serializableDictionary);
        }
      }
      catch (Exception ex)
      {
        PromotionObject.Instance.DictBootPromotions.ClearSync<string, BootPromotion>();
        Logger.Info("Error Loading Boot Promotions" + ex.ToString());
      }
      PromotionObject.mIsBootPromotionLoading = false;
      EventHandler promotionHandler = PromotionObject.BootPromotionHandler;
      if (promotionHandler != null)
        promotionHandler((object) PromotionObject.Instance, new EventArgs());
      try
      {
        foreach (KeyValuePair<string, BootPromotion> oldBootPromotion in (Dictionary<string, BootPromotion>) PromotionObject.Instance.DictOldBootPromotions)
        {
          if (!PromotionObject.Instance.DictBootPromotions.ContainsKey(oldBootPromotion.Key))
            oldBootPromotion.Value.DeleteFile();
        }
      }
      catch (Exception ex)
      {
        Logger.Warning("Error Loading myapp cross Promotions" + ex.ToString());
      }
    }

    private static void SetMyAppsBackgroundPromotion(JToken res)
    {
      bool flag = false;
      try
      {
        if (!JsonExtensions.IsNullOrEmptyBrackets(res.GetValue("myapps_background_id")))
        {
          if (!string.Equals(PromotionObject.Instance.BackgroundPromotionID, res.GetValue("myapps_background_id"), StringComparison.InvariantCulture))
          {
            PromotionObject.Instance.BackgroundPromotionID = res.GetValue("myapps_background_id");
            PromotionObject.Instance.BackgroundPromotionImagePath = Utils.TinyDownloader(res.GetValue("myapps_background_url"), "BackPromo", RegistryStrings.PromotionDirectory, true);
          }
        }
        else
          flag = true;
      }
      catch (Exception ex)
      {
        Logger.Warning("Error Loading myapp background Promotions" + ex.ToString());
        flag = true;
      }
      if (!flag)
        return;
      PromotionObject.Instance.BackgroundPromotionID = "";
      PromotionObject.Instance.BackgroundPromotionImagePath = "";
      IOUtils.DeleteIfExists((IEnumerable<string>) new List<string>()
      {
        Path.Combine(RegistryStrings.PromotionDirectory, "BackPromo")
      });
    }

    internal static void SetMyAppsCrossPromotion(JToken res)
    {
      bool flag = false;
      try
      {
        if (!JsonExtensions.IsNullOrEmptyBrackets(res.GetValue("nyapps_cross_promotion")))
        {
          List<AppSuggestionPromotion> suggestionPromotionList = res[(object) "nyapps_cross_promotion"].ToObject<IEnumerable<AppSuggestionPromotion>>().ToList<AppSuggestionPromotion>();
          if (suggestionPromotionList == null)
          {
            suggestionPromotionList = new List<AppSuggestionPromotion>();
          }
          else
          {
            foreach (JToken jtoken in JArray.Parse(res[(object) "nyapps_cross_promotion"].ToString()))
            {
              JToken x = jtoken;
              if (x[(object) "extra_payload"] != null && x[(object) "app_icon_id"] != null)
                suggestionPromotionList.Where<AppSuggestionPromotion>((Func<AppSuggestionPromotion, bool>) (_ => _.AppIconId == x[(object) "app_icon_id"].ToString())).First<AppSuggestionPromotion>().ExtraPayload.ClearAddRange<string, string>((Dictionary<string, string>) x[(object) "extra_payload"].ToSerializableDictionary<string>());
            }
          }
          lock (((ICollection) PromotionObject.Instance.AppSuggestionList).SyncRoot)
          {
            foreach (AppSuggestionPromotion appSuggestion in PromotionObject.Instance.AppSuggestionList)
            {
              AppSuggestionPromotion item = appSuggestion;
              if (!suggestionPromotionList.Any<AppSuggestionPromotion>((Func<AppSuggestionPromotion, bool>) (_ => string.Equals(_.AppIconId, item.AppIconId, StringComparison.InvariantCulture))))
              {
                IOUtils.DeleteIfExists((IEnumerable<string>) new List<string>()
                {
                  Path.Combine(RegistryStrings.PromotionDirectory, "AppSuggestion" + item.AppIconId)
                });
                PromotionManager.DeleteFavicon((IDictionary<string, string>) item.ExtraPayload);
              }
              if (!suggestionPromotionList.Any<AppSuggestionPromotion>((Func<AppSuggestionPromotion, bool>) (_ => string.Equals(_.IconBorderId, item.IconBorderId, StringComparison.InvariantCulture))))
                IOUtils.DeleteIfExists((IEnumerable<string>) new List<string>()
                {
                  Path.Combine(RegistryStrings.PromotionDirectory, item.IconBorderId + "app_suggestion_icon_border.png"),
                  Path.Combine(RegistryStrings.PromotionDirectory, item.IconBorderId + "app_suggestion_icon_border_hover.png"),
                  Path.Combine(RegistryStrings.PromotionDirectory, item.IconBorderId + "app_suggestion_icon_border_click.png")
                });
            }
            PromotionObject.Instance.AppSuggestionList.ClearAddRange<AppSuggestionPromotion>(suggestionPromotionList);
            foreach (AppSuggestionPromotion appSuggestion in PromotionObject.Instance.AppSuggestionList)
            {
              appSuggestion.AppIconPath = Utils.TinyDownloader(appSuggestion.AppIcon, "AppSuggestion" + appSuggestion.AppIconId, RegistryStrings.PromotionDirectory, false);
              if (!string.IsNullOrEmpty(appSuggestion.IconBorderId) && appSuggestion.IsIconBorder)
              {
                Utils.TinyDownloader(appSuggestion.IconBorderUrl, appSuggestion.IconBorderId + "app_suggestion_icon_border.png", RegistryStrings.PromotionDirectory, false);
                Utils.TinyDownloader(appSuggestion.IconBorderHoverUrl, appSuggestion.IconBorderId + "app_suggestion_icon_border_hover.png", RegistryStrings.PromotionDirectory, false);
                Utils.TinyDownloader(appSuggestion.IconBorderClickUrl, appSuggestion.IconBorderId + "app_suggestion_icon_border_click.png", RegistryStrings.PromotionDirectory, false);
              }
            }
          }
        }
        else
          flag = true;
      }
      catch (Exception ex)
      {
        Logger.Info("Error Loading myapp cross Promotions" + ex.ToString());
        flag = true;
      }
      if (!flag)
        return;
      lock (((ICollection) PromotionObject.Instance.AppSuggestionList).SyncRoot)
        PromotionObject.Instance.AppSuggestionList.ClearSync<AppSuggestionPromotion>();
    }

    private static void DeleteFavicon(IDictionary<string, string> payload)
    {
      if (!payload.ContainsKey("favicon_path"))
        return;
      IOUtils.DeleteIfExists((IEnumerable<string>) new List<string>()
      {
        payload["favicon_path"]
      });
    }

    internal static void ReadQuests(JToken res, bool writePromo)
    {
      bool flag = false;
      SerializableDictionary<string, long[]> serializableDictionary1 = new SerializableDictionary<string, long[]>();
      SerializableDictionary<string, long> serializableDictionary2 = new SerializableDictionary<string, long>();
      try
      {
        if (!JsonExtensions.IsNullOrEmptyBrackets(res.GetValue("quest")))
        {
          PromotionObject.Instance.QuestName = res[(object) "quest"].GetValue("quest_name");
          PromotionObject.Instance.QuestActionType = res[(object) "quest"].GetValue("action_type");
          List<QuestRule> list = res[(object) "quest"][(object) "details"].ToObject<IEnumerable<QuestRule>>().ToList<QuestRule>();
          foreach (QuestRule questRule in list)
          {
            QuestRule rule = questRule;
            if (PromotionObject.Instance.QuestRules.Any<QuestRule>((Func<QuestRule, bool>) (_ => string.Equals(_.RuleId, rule.RuleId, StringComparison.InvariantCulture))))
            {
              if (!serializableDictionary2.ContainsKey(rule.AppPackage.ToLower(CultureInfo.InvariantCulture)))
                serializableDictionary2[rule.AppPackage.ToLower(CultureInfo.InvariantCulture)] = long.MaxValue;
              if (PromotionObject.Instance.ResetQuestRules.ContainsKey(rule.RuleId))
                serializableDictionary1.Add(rule.RuleId, PromotionObject.Instance.ResetQuestRules[rule.RuleId]);
            }
            else
            {
              serializableDictionary2[rule.AppPackage] = 0L;
              long packageAcrossInstances = AppUsageTimer.GetTotalTimeForPackageAcrossInstances(rule.AppPackage);
              long num = 0;
              if (PromotionManager.combinedPackages.ContainsKey(rule.AppPackage))
                num = PromotionManager.combinedPackages[rule.AppPackage];
              serializableDictionary1.Add(rule.RuleId, new long[2]
              {
                num,
                packageAcrossInstances
              });
            }
          }
          PromotionObject.Instance.QuestRules.ClearAddRange<QuestRule>(list);
          PromotionObject.Instance.ResetQuestRules.ClearAddRange<string, long[]>((Dictionary<string, long[]>) serializableDictionary1);
          PromotionObject.Instance.QuestHdPlayerRules.ClearAddRange<string, long>((Dictionary<string, long>) serializableDictionary2);
        }
        else
          flag = true;
      }
      catch (Exception ex)
      {
        Logger.Info("Error Loading promotion quests" + ex.ToString());
        flag = true;
      }
      if (flag)
      {
        PromotionObject.Instance.QuestName = "";
        PromotionObject.Instance.QuestActionType = "";
        PromotionObject.Instance.QuestRules.ClearSync<QuestRule>();
        PromotionObject.Instance.QuestHdPlayerRules.ClearSync<string, long>();
      }
      if (!writePromo)
        return;
      PromotionObject.Save();
      System.Action questHandler = PromotionObject.QuestHandler;
      if (questHandler == null)
        return;
      questHandler();
    }

    private static T DoDeserialize<T>(string data) where T : class
    {
      using (XmlReader xmlReader = XmlReader.Create((Stream) new MemoryStream(Encoding.UTF8.GetBytes(data))))
        return (T) new XmlSerializer(typeof (T)).Deserialize(xmlReader);
    }

    internal static void AddNewMyAppsCrossPromotion(JToken res)
    {
      try
      {
        AppSuggestionPromotion suggestionPromotion1 = res[(object) "nyapps_cross_promotion"].ToObject<AppSuggestionPromotion>();
        if (suggestionPromotion1 != null && res[(object) "nyapps_cross_promotion"][(object) "extra_payload"] != null && res[(object) "nyapps_cross_promotion"][(object) "app_icon_id"] != null)
        {
          suggestionPromotion1.ExtraPayload.ClearAddRange<string, string>((Dictionary<string, string>) res[(object) "nyapps_cross_promotion"][(object) "extra_payload"].ToSerializableDictionary<string>());
          PromotionManager.PopulateAndDownloadFavicon((IDictionary<string, string>) suggestionPromotion1.ExtraPayload, "AppSuggestion", false);
        }
        List<AppSuggestionPromotion> suggestionPromotionList = new List<AppSuggestionPromotion>();
        lock (((ICollection) PromotionObject.Instance.AppSuggestionList).SyncRoot)
        {
          foreach (AppSuggestionPromotion appSuggestion in PromotionObject.Instance.AppSuggestionList)
          {
            if (string.Equals(suggestionPromotion1.AppIconId, appSuggestion.AppIconId, StringComparison.InvariantCulture))
            {
              suggestionPromotionList.Add(appSuggestion);
              IOUtils.DeleteIfExists((IEnumerable<string>) new List<string>()
              {
                Path.Combine(RegistryStrings.PromotionDirectory, "AppSuggestion" + appSuggestion.AppIconId)
              });
            }
          }
          foreach (AppSuggestionPromotion suggestionPromotion2 in suggestionPromotionList)
            PromotionObject.Instance.AppSuggestionList.Remove(suggestionPromotion2);
          suggestionPromotion1.AppIconPath = Utils.TinyDownloader(suggestionPromotion1.AppIcon, "AppSuggestion" + suggestionPromotion1.AppIconId, RegistryStrings.PromotionDirectory, false);
          if (!string.IsNullOrEmpty(suggestionPromotion1.IconBorderId) && suggestionPromotion1.IsIconBorder)
          {
            Utils.TinyDownloader(suggestionPromotion1.IconBorderUrl, suggestionPromotion1.IconBorderId + "app_suggestion_icon_border.png", RegistryStrings.PromotionDirectory, false);
            Utils.TinyDownloader(suggestionPromotion1.IconBorderHoverUrl, suggestionPromotion1.IconBorderId + "app_suggestion_icon_border_hover.png", RegistryStrings.PromotionDirectory, false);
            Utils.TinyDownloader(suggestionPromotion1.IconBorderClickUrl, suggestionPromotion1.IconBorderId + "app_suggestion_icon_border_click.png", RegistryStrings.PromotionDirectory, false);
          }
          PromotionObject.Instance.AppSuggestionList.Add(suggestionPromotion1);
        }
      }
      catch (Exception ex)
      {
        Logger.Info("Error Loading myapp cross Promotions by notification: " + ex.ToString());
      }
    }

    internal static void CheckIsUserPremium()
    {
      string registeredEmail = RegistryManager.Instance.RegisteredEmail;
      string token = RegistryManager.Instance.Token;
      string userGuid = RegistryManager.Instance.UserGuid;
      string version = RegistryManager.Instance.Version;
      string clientVersion = RegistryManager.Instance.ClientVersion;
      string str = "bgp";
      if (string.IsNullOrEmpty(registeredEmail) || string.IsNullOrEmpty(token))
        return;
      string url = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/bs-accounts/getuser?email={1}&guid={2}&token={3}&eng_ver={4}&client_ver={5}&oem={6}", (object) RegistryManager.Instance.Host, (object) registeredEmail, (object) userGuid, (object) token, (object) version, (object) clientVersion, (object) str);
      string json;
      while (true)
      {
        try
        {
          json = BstHttpClient.Get(url, (Dictionary<string, string>) null, false, "", 0, 1, 0, false, "bgp");
          break;
        }
        catch
        {
          Thread.Sleep(20000);
        }
      }
      Logger.Debug("Response string from cloud for bs-accounts/getuser : " + json);
      try
      {
        JObject jobject = JObject.Parse(json);
        if (!string.Equals(jobject["status"].ToString().Trim(), "success", StringComparison.InvariantCulture))
          return;
        RegistryManager.Instance.RegisteredEmail = jobject["message"][(object) "email"].ToString().Trim();
        if (string.Compare(jobject["message"][(object) "subscription_status"].ToString().Trim(), "PAID", StringComparison.OrdinalIgnoreCase) == 0)
          RegistryManager.Instance.IsPremium = true;
        else
          RegistryManager.Instance.IsPremium = false;
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to parse string received from cloud... Err : " + ex.ToString());
      }
    }

    internal static void StartQuestRulesProcessor()
    {
      foreach (QuestRule questRule in PromotionObject.Instance.QuestRules)
      {
        if (questRule.IsRecurring)
        {
          if (!PromotionManager.mDictRecurringCount.ContainsKey(questRule.RuleId))
            PromotionManager.mDictRecurringCount.Add(questRule.RuleId, questRule.RecurringCount);
          else
            PromotionManager.mDictRecurringCount[questRule.RuleId] = questRule.RecurringCount;
        }
      }
      if (PromotionManager.mQuestTimer.Enabled)
      {
        if (PromotionObject.Instance.QuestHdPlayerRules.Count != 0)
          return;
        PromotionManager.mQuestTimer.Stop();
      }
      else
      {
        if (PromotionObject.Instance.QuestHdPlayerRules.Count <= 0)
          return;
        PromotionManager.mQuestTimer.Elapsed -= new ElapsedEventHandler(PromotionManager.QuestTimer_Elapsed);
        PromotionManager.mQuestTimer.Elapsed += new ElapsedEventHandler(PromotionManager.QuestTimer_Elapsed);
        PromotionManager.mQuestTimer.Start();
      }
    }

    private static void QuestTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
      try
      {
        PromotionManager.combinedPackages.Clear();
        foreach (MainWindow mainWindow in BlueStacksUIUtils.DictWindows.Values.ToList<MainWindow>())
        {
          string json = mainWindow.mFrontendHandler.SendFrontendRequest("getInteractionForPackage", (Dictionary<string, string>) null);
          if (!string.IsNullOrEmpty(json))
          {
            Logger.Debug("Package interaction Json received from frontend: " + json);
            foreach (KeyValuePair<string, long> keyValuePair in JToken.Parse(json).ToDictionary<long>() as Dictionary<string, long>)
            {
              if (PromotionManager.combinedPackages.ContainsKey(keyValuePair.Key))
                PromotionManager.combinedPackages[keyValuePair.Key] += keyValuePair.Value;
              else
                PromotionManager.combinedPackages.Add(keyValuePair.Key, keyValuePair.Value);
            }
          }
        }
        List<QuestRuleState> questRuleStateList = new List<QuestRuleState>();
        Dictionary<string, long> dictionary = new Dictionary<string, long>();
        List<QuestRule> source = new List<QuestRule>();
        string empty = string.Empty;
        foreach (QuestRule questRule in PromotionObject.Instance.QuestRules.Where<QuestRule>((Func<QuestRule, bool>) (_ => !PromotionManager.mRuleIdAlreadyPassed.Contains(_.RuleId))))
        {
          if (PromotionManager.combinedPackages.ContainsKey(questRule.AppPackage.ToLower(CultureInfo.InvariantCulture)))
          {
            if (PromotionManager.combinedPackages.ContainsKey("?"))
            {
              foreach (KeyValuePair<string, long> combinedPackage in PromotionManager.combinedPackages)
              {
                if (!combinedPackage.Key.ToString((IFormatProvider) CultureInfo.InvariantCulture).Equals("?", StringComparison.OrdinalIgnoreCase))
                  empty = combinedPackage.Key.ToString((IFormatProvider) CultureInfo.InvariantCulture);
              }
            }
            if ((long) questRule.MinUserInteraction <= PromotionManager.combinedPackages[questRule.AppPackage.ToLower(CultureInfo.InvariantCulture)] - PromotionObject.Instance.ResetQuestRules[questRule.RuleId][0])
            {
              source.Add(questRule);
              if (dictionary.ContainsKey(questRule.AppPackage))
                dictionary[questRule.AppPackage] = PromotionManager.combinedPackages[questRule.AppPackage.ToLower(CultureInfo.InvariantCulture)];
              else
                dictionary.Add(questRule.AppPackage, PromotionManager.combinedPackages[questRule.AppPackage.ToLower(CultureInfo.InvariantCulture)]);
              Logger.Debug("Interaction rule passed for package " + questRule.AppPackage + PromotionManager.combinedPackages[questRule.AppPackage.ToLower(CultureInfo.InvariantCulture)].ToString());
            }
          }
        }
        foreach (QuestRule questRule in source.Where<QuestRule>((Func<QuestRule, bool>) (_ => !PromotionManager.mRuleIdAlreadyPassed.Contains(_.RuleId))))
        {
          QuestRuleState questRuleState = new QuestRuleState();
          if (string.Equals(questRule.AppPackage, "*", StringComparison.InvariantCulture))
          {
            long timeForAllPackages = AppUsageTimer.GetTotalTimeForAllPackages();
            if ((long) questRule.AppUsageTime <= timeForAllPackages - PromotionObject.Instance.ResetQuestRules[questRule.RuleId][1])
            {
              questRuleState.TotalTime = timeForAllPackages;
              questRuleState.QuestRules = questRule;
              questRuleState.Interaction = dictionary[questRule.AppPackage];
              questRuleStateList.Add(questRuleState);
              if (PromotionManager.combinedPackages.ContainsKey(questRule.AppPackage.ToLower(CultureInfo.InvariantCulture)))
              {
                PromotionObject.Instance.ResetQuestRules[questRule.RuleId][0] = PromotionManager.combinedPackages[questRule.AppPackage.ToLower(CultureInfo.InvariantCulture)];
                PromotionObject.Instance.ResetQuestRules[questRule.RuleId][1] = questRuleState.TotalTime;
              }
            }
          }
          else if (string.Equals(questRule.AppPackage, "?", StringComparison.InvariantCulture))
          {
            long packageAfterReset = AppUsageTimer.GetTotalTimeForPackageAfterReset(empty);
            if ((long) questRule.AppUsageTime <= packageAfterReset - PromotionObject.Instance.ResetQuestRules[questRule.RuleId][1])
            {
              questRuleState.TotalTime = packageAfterReset;
              questRuleState.QuestRules = questRule;
              questRuleState.Interaction = dictionary[questRule.AppPackage];
              questRuleStateList.Add(questRuleState);
              if (PromotionManager.combinedPackages.ContainsKey(questRule.AppPackage.ToLower(CultureInfo.InvariantCulture)))
              {
                PromotionObject.Instance.ResetQuestRules[questRule.RuleId][0] = PromotionManager.combinedPackages[questRule.AppPackage.ToLower(CultureInfo.InvariantCulture)];
                PromotionObject.Instance.ResetQuestRules[questRule.RuleId][1] = questRuleState.TotalTime;
              }
            }
          }
          else
          {
            long packageAfterReset = AppUsageTimer.GetTotalTimeForPackageAfterReset(questRule.AppPackage.ToLower(CultureInfo.InvariantCulture));
            if ((long) questRule.AppUsageTime <= packageAfterReset - PromotionObject.Instance.ResetQuestRules[questRule.RuleId][1])
            {
              questRuleState.TotalTime = packageAfterReset;
              questRuleState.QuestRules = questRule;
              questRuleState.Interaction = dictionary[questRule.AppPackage];
              questRuleStateList.Add(questRuleState);
              if (PromotionManager.combinedPackages.ContainsKey(questRule.AppPackage.ToLower(CultureInfo.InvariantCulture)))
              {
                PromotionObject.Instance.ResetQuestRules[questRule.RuleId][0] = PromotionManager.combinedPackages[questRule.AppPackage.ToLower(CultureInfo.InvariantCulture)];
                PromotionObject.Instance.ResetQuestRules[questRule.RuleId][1] = questRuleState.TotalTime;
              }
            }
          }
        }
        if (questRuleStateList.Count > 0)
        {
          SerializableDictionary<string, long> serializableDictionary = new SerializableDictionary<string, long>();
          bool flag = false;
          foreach (QuestRule questRule in PromotionObject.Instance.QuestRules)
            serializableDictionary[questRule.AppPackage.ToLower(CultureInfo.InvariantCulture)] = long.MaxValue;
          foreach (QuestRuleState questRuleState in questRuleStateList)
          {
            QuestRuleState ruleState = questRuleState;
            string str = ruleState.QuestRules.CloudHandler;
            string jsonObjectString = JSONUtils.GetJSONObjectString(AppUsageTimer.GetRealtimeDictionary());
            if (string.IsNullOrEmpty(str))
              str = "/pika_points/quest_rule_accomplished";
            string url = WebHelper.GetUrlWithParams(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}", (object) WebHelper.GetServerHost(), (object) str), (string) null, (string) null, (string) null) + string.Format((IFormatProvider) CultureInfo.InvariantCulture, "&email={5}&quest_name={0}&rule_id={1}&app_pkg={2}&usage_time={3}&user_interactions={4}&usage_data={6}", (object) PromotionObject.Instance.QuestName, (object) ruleState.QuestRules.RuleId, (object) ruleState.QuestRules.AppPackage, (object) ruleState.TotalTime, (object) ruleState.Interaction, (object) RegistryManager.Instance.RegisteredEmail, (object) jsonObjectString);
            int num = 3;
            while (num > 0)
            {
              try
              {
                Logger.Info("Quest rule passed response from cloud " + BstHttpClient.Get(url, (Dictionary<string, string>) null, false, string.Empty, 5000, 1, 0, false, "bgp") + " ruleId: " + ruleState.QuestRules.RuleId);
                break;
              }
              catch (Exception ex)
              {
                Logger.Warning("Exception while calling cloud for quest rule passed, RETRYING " + num.ToString() + Environment.NewLine + ex.ToString());
                --num;
                Thread.Sleep(1000);
              }
            }
            if (num == 0)
              Logger.Error("Could not send quest rule passed, to cloud after retries.");
            if (!ruleState.QuestRules.IsRecurring)
              PromotionManager.mRuleIdAlreadyPassed.Add(ruleState.QuestRules.RuleId);
            else if (PromotionManager.mDictRecurringCount[ruleState.QuestRules.RuleId] == -1)
            {
              if (string.Equals(ruleState.QuestRules.AppPackage, "*", StringComparison.InvariantCulture))
                AppUsageTimer.GetTotalTimeForAllPackages();
              else
                AppUsageTimer.GetTotalTimeForPackageAfterReset(ruleState.QuestRules.AppPackage);
              if (PromotionObject.Instance.QuestRules.Any<QuestRule>((Func<QuestRule, bool>) (_ => string.Equals(_.RuleId, ruleState.QuestRules.RuleId, StringComparison.InvariantCulture))) && serializableDictionary.ContainsKey(ruleState.QuestRules.AppPackage.ToLower(CultureInfo.InvariantCulture)))
              {
                if (ruleState.QuestRules.RecurringCount != -1)
                  --ruleState.QuestRules.RecurringCount;
                serializableDictionary[ruleState.QuestRules.AppPackage.ToLower(CultureInfo.InvariantCulture)] = 0L;
                flag = true;
              }
            }
            else
            {
              PromotionManager.mDictRecurringCount[ruleState.QuestRules.RuleId] = PromotionManager.mDictRecurringCount[ruleState.QuestRules.RuleId] - 1;
              if (PromotionManager.mDictRecurringCount[ruleState.QuestRules.RuleId] == 0)
                PromotionManager.mRuleIdAlreadyPassed.Add(ruleState.QuestRules.RuleId);
              else if (PromotionManager.mDictRecurringCount[ruleState.QuestRules.RuleId] > 0)
              {
                if (string.Equals(ruleState.QuestRules.AppPackage, "*", StringComparison.InvariantCulture))
                {
                  AppUsageTimer.AddPackageForReset("*", AppUsageTimer.GetTotalTimeForAllPackages());
                }
                else
                {
                  long packageAfterReset = AppUsageTimer.GetTotalTimeForPackageAfterReset(ruleState.QuestRules.AppPackage);
                  AppUsageTimer.AddPackageForReset(ruleState.QuestRules.AppPackage.ToLower(CultureInfo.InvariantCulture), packageAfterReset);
                }
                if (PromotionObject.Instance.QuestRules.Any<QuestRule>((Func<QuestRule, bool>) (_ => string.Equals(_.RuleId, ruleState.QuestRules.RuleId, StringComparison.InvariantCulture))) && serializableDictionary.ContainsKey(ruleState.QuestRules.AppPackage.ToLower(CultureInfo.InvariantCulture)))
                {
                  if (ruleState.QuestRules.RecurringCount != -1)
                    --ruleState.QuestRules.RecurringCount;
                  serializableDictionary[ruleState.QuestRules.AppPackage.ToLower(CultureInfo.InvariantCulture)] = 0L;
                  flag = true;
                }
              }
            }
          }
          if (flag)
          {
            PromotionObject.Instance.QuestHdPlayerRules.ClearAddRange<string, long>((Dictionary<string, long>) serializableDictionary);
            PromotionObject.Save();
            System.Action questHandler = PromotionObject.QuestHandler;
            if (questHandler != null)
              questHandler();
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in QuestTimer_Elapsed " + ex.ToString());
      }
      try
      {
        List<GenericNotificationItem> notificationItemList = new List<GenericNotificationItem>();
        foreach (KeyValuePair<GenericNotificationItem, long> deferredNotifications in PromotionManager.sDeferredNotificationsList)
        {
          if (AppUsageTimer.GetTotalTimeForPackageAfterReset(deferredNotifications.Key.DeferredApp.ToLower(CultureInfo.InvariantCulture)) - deferredNotifications.Value >= deferredNotifications.Key.DeferredAppUsage)
          {
            if (string.Equals(BlueStacksUIUtils.DictWindows[BlueStacks.Common.Strings.CurrentDefaultVmName].mTopBar.mAppTabButtons.SelectedTab.PackageName, deferredNotifications.Key.DeferredApp, StringComparison.InvariantCulture))
            {
              BlueStacksUIUtils.DictWindows[BlueStacks.Common.Strings.CurrentDefaultVmName].HandleGenericNotificationPopup(deferredNotifications.Key);
              GenericNotificationManager.AddNewNotification(deferredNotifications.Key, false);
              BlueStacksUIUtils.DictWindows[BlueStacks.Common.Strings.CurrentDefaultVmName].Dispatcher.Invoke((Delegate) (() => BlueStacksUIUtils.DictWindows[BlueStacks.Common.Strings.CurrentDefaultVmName].mTopBar.RefreshNotificationCentreButton()));
              notificationItemList.Add(deferredNotifications.Key);
            }
            else
              PromotionManager.sPassedDeferredNotificationsList.Add(deferredNotifications.Key);
          }
        }
        foreach (GenericNotificationItem key in notificationItemList)
          PromotionManager.sDeferredNotificationsList.Remove(key);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in checking deferred notification: " + ex.ToString());
      }
    }

    private static void SetSecurityMetrics(JToken res)
    {
      try
      {
        PromotionObject.Instance.IsSecurityMetricsEnable = !JsonExtensions.IsNullOrEmptyBrackets(res.GetValue("security_metrics_enable_user")) && res[(object) "security_metrics_enable_user"].ToObject<bool>();
        if (!JsonExtensions.IsNullOrEmptyBrackets(res.GetValue("security_metrics_blacklisted_apps")))
        {
          PromotionObject.Instance.BlackListedApplicationsList.ClearSync<string>();
          foreach (JToken jtoken in JArray.Parse(res[(object) "security_metrics_blacklisted_apps"].ToString()))
            PromotionObject.Instance.BlackListedApplicationsList.Add(jtoken.ToString());
        }
        else
          PromotionObject.Instance.BlackListedApplicationsList.ClearSync<string>();
      }
      catch (Exception ex)
      {
        Logger.Error("Error while getting security metrics info: {0}", (object) ex.ToString());
        PromotionObject.Instance.IsSecurityMetricsEnable = false;
        PromotionObject.Instance.BlackListedApplicationsList.ClearSync<string>();
      }
    }

    private static void SetCustomCursorRuleForApp(JToken res)
    {
      try
      {
        if (!JsonExtensions.IsNullOrEmptyBrackets(res.GetValue("exclude_custom_cursor")))
        {
          PromotionObject.Instance.CustomCursorExcludedAppsList.ClearSync<string>();
          foreach (JToken jtoken in JArray.Parse(res[(object) "exclude_custom_cursor"].ToString()))
            PromotionObject.Instance.CustomCursorExcludedAppsList.Add(jtoken.ToString());
        }
        else
          PromotionObject.Instance.CustomCursorExcludedAppsList.ClearSync<string>();
      }
      catch (Exception ex)
      {
        Logger.Error("Error while getting custom cursor exclude list of apps: {0}", (object) ex);
      }
    }

    private static string AddSamsungStoreParamsIfPresent(string url)
    {
      try
      {
        url = url + "&samsung_store_present=" + RegistryManager.Instance.IsSamsungStorePresent.ToString((IFormatProvider) CultureInfo.InvariantCulture).ToLowerInvariant();
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to add samsung store parameter. Ex : " + ex.ToString());
      }
      return url;
    }
  }
}
