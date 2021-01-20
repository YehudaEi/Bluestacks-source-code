// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.GuidanceCloudInfoManager
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;

namespace BlueStacks.BlueStacksUI
{
  internal sealed class GuidanceCloudInfoManager
  {
    private static GuidanceCloudInfoManager sInstance = (GuidanceCloudInfoManager) null;
    private static readonly object sLock = new object();
    internal GuidanceCloudInfo mGuidanceCloudInfo = new GuidanceCloudInfo();
    private const string sGuidanceCloudInfoFilename = "bst_guidance";

    private static string BstGuidanceFilePath
    {
      get
      {
        return Path.Combine(RegistryStrings.PromotionDirectory, "bst_guidance");
      }
    }

    private GuidanceCloudInfoManager()
    {
    }

    public static GuidanceCloudInfoManager Instance
    {
      get
      {
        if (GuidanceCloudInfoManager.sInstance == null)
        {
          lock (GuidanceCloudInfoManager.sLock)
          {
            if (GuidanceCloudInfoManager.sInstance == null)
              GuidanceCloudInfoManager.sInstance = new GuidanceCloudInfoManager();
          }
        }
        return GuidanceCloudInfoManager.sInstance;
      }
      set
      {
        GuidanceCloudInfoManager.sInstance = value;
      }
    }

    private static JToken GetGuidanceCloudInfoData()
    {
      JToken jtoken = (JToken) null;
      try
      {
        string urlWithParams = WebHelper.GetUrlWithParams(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}/{2}", (object) RegistryManager.Instance.Host, (object) "bs4", (object) "guidance_window"), (string) null, (string) null, (string) null);
        string json = BstHttpClient.Post(urlWithParams, new Dictionary<string, string>()
        {
          {
            "app_pkgs",
            GuidanceCloudInfoManager.GetInstalledAppDataFromAllVms()
          }
        }, (Dictionary<string, string>) null, false, "Android", 0, 1, 0, false, "bgp");
        Logger.Debug("Guidance Cloud Info Url: " + urlWithParams);
        Logger.Debug("Guidance Cloud Info Response: " + json);
        jtoken = JToken.Parse(json);
      }
      catch (Exception ex)
      {
        Logger.Warning("Error Getting GetGuidanceCloudInfoData " + ex.ToString());
      }
      return jtoken;
    }

    private static string GetInstalledAppDataFromAllVms()
    {
      string[] vmList = RegistryManager.Instance.VmList;
      JArray jarray = new JArray();
      try
      {
        foreach (string vmName in vmList)
        {
          foreach (AppInfo appInfo in ((IEnumerable<AppInfo>) new JsonParser(vmName).GetAppList()).ToList<AppInfo>())
          {
            string package = appInfo.Package;
            jarray.Add((JToken) package);
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in getting all installed apps from all Vms: {0}", (object) ex.ToString());
      }
      return jarray.ToString(Formatting.None);
    }

    internal void AppsGuidanceCloudInfoRefresh()
    {
      new Thread((ThreadStart) (() =>
      {
        if (File.Exists(GuidanceCloudInfoManager.BstGuidanceFilePath))
          this.mGuidanceCloudInfo = JsonConvert.DeserializeObject<GuidanceCloudInfo>(File.ReadAllText(GuidanceCloudInfoManager.BstGuidanceFilePath), Utils.GetSerializerSettings());
        JToken guidanceCloudInfoData = GuidanceCloudInfoManager.GetGuidanceCloudInfoData();
        if (guidanceCloudInfoData == null)
          return;
        GuidanceCloudInfo guidanceCloudInfo = new GuidanceCloudInfo();
        GuidanceCloudInfoManager.SetAppsVideoThumbnail(guidanceCloudInfo, guidanceCloudInfoData);
        GuidanceCloudInfoManager.SetAppsReadArticle(guidanceCloudInfo, guidanceCloudInfoData);
        GuidanceCloudInfoManager.SetGameSettings(guidanceCloudInfo, guidanceCloudInfoData);
        GuidanceCloudInfoManager.SaveToFile(guidanceCloudInfo);
        this.mGuidanceCloudInfo = guidanceCloudInfo;
      }))
      {
        IsBackground = true
      }.Start();
    }

    private static void SaveToFile(GuidanceCloudInfo guidanceCloudInfo)
    {
      try
      {
        JsonSerializerSettings serializerSettings = Utils.GetSerializerSettings();
        serializerSettings.Formatting = Formatting.Indented;
        string contents = JsonConvert.SerializeObject((object) guidanceCloudInfo, serializerSettings);
        if (!Directory.Exists(RegistryStrings.PromotionDirectory))
          Directory.CreateDirectory(RegistryStrings.PromotionDirectory);
        File.WriteAllText(GuidanceCloudInfoManager.BstGuidanceFilePath, contents);
      }
      catch (Exception ex)
      {
        Logger.Warning("Error in saving GuidanceCloudInfo to file");
      }
    }

    private static void SetAppsVideoThumbnail(
      GuidanceCloudInfo currentAppsGuidanceCloudInfo,
      JToken res)
    {
      try
      {
        foreach (object obj in JArray.Parse(res.GetValue("custom_thumbnails").ToString((IFormatProvider) CultureInfo.InvariantCulture)))
        {
          CustomThumbnail customThumbnail = JsonConvert.DeserializeObject<CustomThumbnail>(obj.ToString(), Utils.GetSerializerSettings());
          foreach (GuidanceVideoType guidanceVideoType in Enum.GetValues(typeof (GuidanceVideoType)))
          {
            if (guidanceVideoType == GuidanceVideoType.SchemeSpecific)
            {
              foreach (KeyValuePair<string, VideoThumbnailInfo> keyValuePair in (Dictionary<string, VideoThumbnailInfo>) customThumbnail[guidanceVideoType.ToString()])
              {
                VideoThumbnailInfo videoThumbnailInfo = keyValuePair.Value;
                videoThumbnailInfo.ThumbnailType = guidanceVideoType;
                videoThumbnailInfo.ImagePath = Utils.TinyDownloader(videoThumbnailInfo.ThumbnailUrl, "VideoThumbnail_" + customThumbnail.Package + videoThumbnailInfo.ThumbnailId, RegistryStrings.PromotionDirectory, false);
              }
            }
            else if (customThumbnail[guidanceVideoType.ToString()] != null)
            {
              VideoThumbnailInfo videoThumbnailInfo = (VideoThumbnailInfo) customThumbnail[guidanceVideoType.ToString()];
              videoThumbnailInfo.ThumbnailType = guidanceVideoType;
              videoThumbnailInfo.ImagePath = Utils.TinyDownloader(videoThumbnailInfo.ThumbnailUrl, "VideoThumbnail_" + customThumbnail.Package + videoThumbnailInfo.ThumbnailId, RegistryStrings.PromotionDirectory, false);
            }
          }
          currentAppsGuidanceCloudInfo.CustomThumbnails[customThumbnail.Package] = customThumbnail;
        }
        foreach (object obj in JArray.Parse(res.GetValue("default_thumbnails").ToString((IFormatProvider) CultureInfo.InvariantCulture)))
        {
          VideoThumbnailInfo videoThumbnailInfo = JsonConvert.DeserializeObject<VideoThumbnailInfo>(obj.ToString(), Utils.GetSerializerSettings());
          videoThumbnailInfo.ImagePath = Utils.TinyDownloader(videoThumbnailInfo.ThumbnailUrl, "VideoThumbnail_DefaultPackage_" + videoThumbnailInfo.ThumbnailId, RegistryStrings.PromotionDirectory, false);
          currentAppsGuidanceCloudInfo.DefaultThumbnails[videoThumbnailInfo.ThumbnailType] = videoThumbnailInfo;
        }
      }
      catch (Exception ex)
      {
        Logger.Warning("Error Loading Apps VideoThumbnail" + ex.ToString());
      }
    }

    private static void SetAppsReadArticle(
      GuidanceCloudInfo currentAppsGuidanceCloudInfo,
      JToken res)
    {
      try
      {
        foreach (object obj in JArray.Parse(res.GetValue("help_article").ToString((IFormatProvider) CultureInfo.InvariantCulture)))
        {
          HelpArticle helpArticle = JsonConvert.DeserializeObject<HelpArticle>(obj.ToString(), Utils.GetSerializerSettings());
          currentAppsGuidanceCloudInfo.HelpArticles[helpArticle.Package] = helpArticle;
        }
      }
      catch (Exception ex)
      {
        Logger.Warning("Error Loading Apps ReadArticle" + ex.ToString());
      }
    }

    private static void SetGameSettings(GuidanceCloudInfo guidanceCloudInfoDict, JToken res)
    {
      try
      {
        if (!(res[(object) "game_settings"] is JArray re))
          return;
        foreach (JToken jtoken1 in re)
        {
          try
          {
            GameSetting gameSetting = new GameSetting()
            {
              SettingType = jtoken1[(object) "setting_type"].Value<string>()
            };
            if (jtoken1[(object) "setting_data"] is JArray jarray)
            {
              foreach (JToken jtoken2 in jarray)
              {
                Dictionary<string, object> dictionary = new Dictionary<string, object>();
                if (jtoken2[(object) "app_pkg_list"] is JArray jarray)
                  dictionary.Add("app_pkg_list", (object) new AppPackageListObject(jarray.ToObject<List<string>>()));
                switch (gameSetting.SettingType)
                {
                  case "OrientationMode":
                    dictionary.Add("mode", (object) jtoken2[(object) "mode"].Value<string>());
                    gameSetting.SettingsData.Add(dictionary);
                    continue;
                  case "ApplyCfgUpdateImmediately":
                    dictionary.Add("setting", (object) jtoken2[(object) "setting"].Value<string>());
                    gameSetting.SettingsData.Add(dictionary);
                    continue;
                  default:
                    continue;
                }
              }
              guidanceCloudInfoDict.GameSettings.Add(gameSetting);
            }
          }
          catch (Exception ex)
          {
            Logger.Warning("Error while loading game settings from cloud data " + ex?.ToString());
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Warning("Error while loading game settings from cloud data " + ex?.ToString());
      }
    }

    internal static string GetCloudOrientationForPackage(string package)
    {
      string str = string.Empty;
      if (GuidanceCloudInfoManager.Instance.mGuidanceCloudInfo != null && GuidanceCloudInfoManager.Instance.mGuidanceCloudInfo.GameSettings.Any<GameSetting>())
      {
        GameSetting gameSetting = GuidanceCloudInfoManager.Instance.mGuidanceCloudInfo.GameSettings.Where<GameSetting>((Func<GameSetting, bool>) (setting => string.Equals(setting.SettingType, "OrientationMode", StringComparison.InvariantCultureIgnoreCase))).FirstOrDefault<GameSetting>();
        if (gameSetting != null)
        {
          foreach (Dictionary<string, object> dictionary in gameSetting.SettingsData)
          {
            if (dictionary.ContainsKey("mode") && dictionary.ContainsKey("app_pkg_list") && (dictionary["app_pkg_list"] is AppPackageListObject packageListObject && packageListObject.IsPackageAvailable(package)))
            {
              str = dictionary["mode"].ToString().ToLower(CultureInfo.InvariantCulture);
              break;
            }
          }
        }
      }
      return str;
    }

    internal static bool GetApplyCfgUpdateSettingForPackage(string package)
    {
      try
      {
        string str = string.Empty;
        if (GuidanceCloudInfoManager.Instance.mGuidanceCloudInfo != null && GuidanceCloudInfoManager.Instance.mGuidanceCloudInfo.GameSettings.Any<GameSetting>())
        {
          GameSetting gameSetting = GuidanceCloudInfoManager.Instance.mGuidanceCloudInfo.GameSettings.Where<GameSetting>((Func<GameSetting, bool>) (setting => string.Equals(setting.SettingType, "ApplyCfgUpdateImmediately", StringComparison.InvariantCultureIgnoreCase))).FirstOrDefault<GameSetting>();
          if (gameSetting != null)
          {
            foreach (Dictionary<string, object> dictionary in gameSetting.SettingsData)
            {
              if (dictionary.ContainsKey("setting") && dictionary.ContainsKey("app_pkg_list") && (dictionary["app_pkg_list"] is AppPackageListObject packageListObject && packageListObject.IsPackageAvailable(package)))
              {
                str = dictionary["setting"].ToString().ToLower(CultureInfo.InvariantCulture);
                break;
              }
            }
          }
        }
        return str.Equals("true", StringComparison.InvariantCultureIgnoreCase);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in GetApplyCfgUpdateSettingForPackage: " + ex.ToString());
      }
      return false;
    }
  }
}
