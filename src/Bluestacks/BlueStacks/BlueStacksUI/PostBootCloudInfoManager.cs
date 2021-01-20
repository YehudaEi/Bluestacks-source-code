// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.PostBootCloudInfoManager
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
using System.Threading;

namespace BlueStacks.BlueStacksUI
{
  internal class PostBootCloudInfoManager
  {
    private static PostBootCloudInfoManager sInstance = (PostBootCloudInfoManager) null;
    private static readonly object sLock = new object();
    private string mUrl = string.Empty;
    private const string sPostBootCloudInfoFilename = "bst_postboot";
    internal PostBootCloudInfo mPostBootCloudInfo;

    private static string BstPostBootFilePath
    {
      get
      {
        return Path.Combine(RegistryStrings.PromotionDirectory, "bst_postboot");
      }
    }

    private PostBootCloudInfoManager()
    {
    }

    internal static PostBootCloudInfoManager Instance
    {
      get
      {
        if (PostBootCloudInfoManager.sInstance == null)
        {
          lock (PostBootCloudInfoManager.sLock)
          {
            if (PostBootCloudInfoManager.sInstance == null)
              PostBootCloudInfoManager.sInstance = new PostBootCloudInfoManager();
          }
        }
        return PostBootCloudInfoManager.sInstance;
      }
      set
      {
        PostBootCloudInfoManager.sInstance = value;
      }
    }

    internal string Url
    {
      get
      {
        if (string.IsNullOrEmpty(this.mUrl))
          this.mUrl = WebHelper.GetUrlWithParams(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}", (object) RegistryManager.Instance.Host, (object) "/bs4/post_boot"), (string) null, (string) null, (string) null);
        return this.mUrl;
      }
      private set
      {
        this.mUrl = value;
      }
    }

    internal JToken GetPostBootData()
    {
      JToken jtoken = (JToken) null;
      try
      {
        string json = BstHttpClient.Get(this.Url, (Dictionary<string, string>) null, false, "Android", 0, 1, 0, false, "bgp");
        Logger.Debug("Postboot data Url: " + this.Url);
        jtoken = JToken.Parse(json);
      }
      catch (Exception ex)
      {
        Logger.Error("Error Getting Post Boot Data err: " + ex.ToString());
      }
      return jtoken;
    }

    internal void GetPostBootDataAsync(MainWindow mainWindow)
    {
      if (this.mPostBootCloudInfo == null)
        new Thread((ThreadStart) (() =>
        {
          this.mPostBootCloudInfo = new PostBootCloudInfo();
          if (File.Exists(PostBootCloudInfoManager.BstPostBootFilePath))
          {
            try
            {
              this.mPostBootCloudInfo = JsonConvert.DeserializeObject<PostBootCloudInfo>(File.ReadAllText(PostBootCloudInfoManager.BstPostBootFilePath));
            }
            catch (Exception ex)
            {
              Logger.Warning("Failed to parse cached bst_postboot file, " + ex.ToString());
            }
          }
          JToken postBootData = this.GetPostBootData();
          if (postBootData == null)
            return;
          PostBootCloudInfo postBootCloudInfo = new PostBootCloudInfo();
          PostBootCloudInfoManager.SetMinimizeGameNotificationsPackages(postBootCloudInfo, postBootData);
          PostBootCloudInfoManager.SetDesktopNotificationsChatPackages(postBootCloudInfo, postBootData);
          PostBootCloudInfoManager.SetOnBoardingGamePackages(postBootCloudInfo, postBootData);
          PostBootCloudInfoManager.SetUtcConverterGamePackages(postBootCloudInfo, postBootData);
          PostBootCloudInfoManager.SetGameAwareOnboardingPackages(postBootCloudInfo, postBootData);
          PostBootCloudInfoManager.SetCustomCursorGamePackages(postBootCloudInfo, postBootData);
          PostBootCloudInfoManager.SetIgnoreActivities(postBootCloudInfo, postBootData);
          PostBootCloudInfoManager.SetHomeHtmlErrorHandlingFlag(postBootData);
          PostBootCloudInfoManager.SaveToFile(postBootCloudInfo);
          this.mPostBootCloudInfo = postBootCloudInfo;
          if (RegistryManager.Instance.FixNotificationDataAfterFirstPostBootCloudInfo)
          {
            PostBootCloudInfoManager.FixNotificationDataForDesktopNotificationsOnUpgrade();
            RegistryManager.Instance.FixNotificationDataAfterFirstPostBootCloudInfo = false;
          }
          NotificationManager.Instance.ChatApplications = this.mPostBootCloudInfo.DesktopNotificationsChatPackages.ChatApplicationPackages;
          PostBootCloudInfoManager.SendCustomCursorAppsListToPlayer(mainWindow);
        }))
        {
          IsBackground = true
        }.Start();
      else
        PostBootCloudInfoManager.SendCustomCursorAppsListToPlayer(mainWindow);
    }

    public static void FixNotificationDataForDesktopNotificationsOnUpgrade()
    {
      try
      {
        JsonParser jsonParser = new JsonParser("Android");
        foreach (NotificationItem notificationItem in NotificationManager.Instance.DictNotificationItems.Values)
        {
          string packageNameFromAppName = jsonParser.GetPackageNameFromAppName(notificationItem.ID);
          if (string.IsNullOrEmpty(packageNameFromAppName))
          {
            notificationItem.ShowDesktopNotifications = false;
          }
          else
          {
            DesktopNotificationsInfo notificationsChatPackages = PostBootCloudInfoManager.Instance.mPostBootCloudInfo.DesktopNotificationsChatPackages;
            int num;
            if (notificationsChatPackages == null)
            {
              num = 0;
            }
            else
            {
              bool? nullable = notificationsChatPackages.ChatApplicationPackages?.IsPackageAvailable(packageNameFromAppName);
              bool flag = true;
              num = nullable.GetValueOrDefault() == flag & nullable.HasValue ? 1 : 0;
            }
            notificationItem.ShowDesktopNotifications = num != 0;
          }
        }
        NotificationManager.Instance.UpdateNotificationsSettings();
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in FixNotificationDataForDesktopNotificationsOnUpgrade: " + ex.ToString());
      }
    }

    private static void SendCustomCursorAppsListToPlayer(MainWindow mainWindow)
    {
      try
      {
        string empty = string.Empty;
        foreach (AppPackageObject cloudPackage in PostBootCloudInfoManager.Instance.mPostBootCloudInfo.AppSpecificCustomCursorInfo.CustomCursorAppPackages.CloudPackageList)
          empty += string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} ", (object) cloudPackage.Package);
        mainWindow.mFrontendHandler.SendFrontendRequestAsync("sendCustomCursorEnabledApps", new Dictionary<string, string>()
        {
          {
            "packages",
            empty.Trim()
          }
        });
        Logger.Debug("CURSOR: vmName:{0} packages: {1} ", (object) mainWindow.mVmName, (object) empty.Trim());
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in SendCustomCursorAppsListToPlayer: " + ex.ToString());
      }
    }

    private static void SaveToFile(PostBootCloudInfo postBootCloudInfo)
    {
      try
      {
        string contents = JsonConvert.SerializeObject((object) postBootCloudInfo, Formatting.Indented, Utils.GetSerializerSettings());
        if (!Directory.Exists(RegistryStrings.PromotionDirectory))
          Directory.CreateDirectory(RegistryStrings.PromotionDirectory);
        File.WriteAllText(PostBootCloudInfoManager.BstPostBootFilePath, contents);
      }
      catch (Exception ex)
      {
        Logger.Warning("Error in saving PostBootCloudInfo to file");
      }
    }

    private static void SetMinimizeGameNotificationsPackages(
      PostBootCloudInfo currentPostBootCloudInfo,
      JToken res)
    {
      try
      {
        JToken jtoken = JToken.Parse(res.GetValue("minimize_game_notification_apps"));
        if (jtoken[(object) "app_pkg_list"] != null && jtoken[(object) "app_pkg_list"] is JArray jarray)
          currentPostBootCloudInfo.GameNotificationAppPackages.NotificationModeAppPackages = new AppPackageListObject(jarray.ToObject<List<string>>());
        int result;
        if (jtoken[(object) "consecutive_session_count_number"] == null || !int.TryParse(jtoken[(object) "consecutive_session_count_number"].ToString(), out result))
          return;
        currentPostBootCloudInfo.GameNotificationAppPackages.ExitPopupCount = result;
        RegistryManager.Instance.NotificationModeCounter = result;
      }
      catch (Exception ex)
      {
        Logger.Warning("Exception in parsing game notification packages: " + ex.ToString());
      }
    }

    private static void SetDesktopNotificationsChatPackages(
      PostBootCloudInfo currentPostBootCloudInfo,
      JToken res)
    {
      try
      {
        JToken jtoken = JToken.Parse(res.GetValue("DesktopNotificationDefaultApps"));
        if (jtoken[(object) "app_pkg_list"] == null || !(jtoken[(object) "app_pkg_list"] is JArray jarray))
          return;
        currentPostBootCloudInfo.DesktopNotificationsChatPackages.ChatApplicationPackages = new AppPackageListObject(jarray.ToObject<List<string>>());
      }
      catch (Exception ex)
      {
        Logger.Warning("Exception in parsing game notification packages: " + ex.ToString());
      }
    }

    private static void SetOnBoardingGamePackages(
      PostBootCloudInfo currentPostBootCloudInfo,
      JToken res)
    {
      try
      {
        JToken jtoken = JToken.Parse(res.GetValue("onboarding_tutorial_apps"));
        if (jtoken[(object) "app_pkg_list"] != null && jtoken[(object) "app_pkg_list"] is JArray jarray)
          currentPostBootCloudInfo.OnBoardingInfo.OnBoardingAppPackages = new AppPackageListObject(jarray.ToObject<List<string>>());
        if (jtoken[(object) "skip_button_timer"] == null)
          return;
        currentPostBootCloudInfo.OnBoardingInfo.OnBoardingSkipTimer = int.Parse(jtoken[(object) "skip_button_timer"].ToString(), (IFormatProvider) CultureInfo.InvariantCulture);
      }
      catch (Exception ex)
      {
        Logger.Warning("Exception in parsing onboarding packages: " + ex.ToString());
      }
    }

    private static void SetUtcConverterGamePackages(
      PostBootCloudInfo currentPostBootCloudInfo,
      JToken res)
    {
      try
      {
        JToken jtoken = JToken.Parse(res.GetValue("utc_converter_apps"));
        if (jtoken[(object) "app_pkg_list"] == null || !(jtoken[(object) "app_pkg_list"] is JArray jarray))
          return;
        currentPostBootCloudInfo.UtcConverterInfo.UtcConverterAppPackages = new AppPackageListObject(jarray.ToObject<List<AppPackageObject>>());
      }
      catch (Exception ex)
      {
        Logger.Warning("Exception in parsing utc converter packages: " + ex.ToString());
      }
    }

    private static void SetCustomCursorGamePackages(
      PostBootCloudInfo currentPostBootCloudInfo,
      JToken res)
    {
      try
      {
        JToken jtoken = JToken.Parse(JToken.Parse(res.GetValue("custom_cursor_apps")).GetValue("moba"));
        if (jtoken[(object) "app_pkg_list"] == null || !(jtoken[(object) "app_pkg_list"] is JArray jarray))
          return;
        currentPostBootCloudInfo.AppSpecificCustomCursorInfo.CustomCursorAppPackages = new AppPackageListObject(jarray.ToObject<List<string>>());
      }
      catch (Exception ex)
      {
        Logger.Warning("Exception in parsing SetCustomCursorGamePackages: " + ex.ToString());
      }
    }

    private static void SetIgnoreActivities(PostBootCloudInfo currentPostBootCloudInfo, JToken res)
    {
      try
      {
        if (!JsonExtensions.IsNullOrEmptyBrackets(res.GetValue("ignore_activities_for_tab")))
        {
          currentPostBootCloudInfo.IgnoredActivitiesForTabs.ClearSync<string>();
          foreach (JToken jtoken in JArray.Parse(res[(object) "ignore_activities_for_tab"].ToString()))
            currentPostBootCloudInfo.IgnoredActivitiesForTabs.Add(jtoken.ToString());
        }
        else
          currentPostBootCloudInfo.IgnoredActivitiesForTabs.ClearSync<string>();
      }
      catch (Exception ex)
      {
        Logger.Error("Error while getting ignore activities for tab list: {0}", (object) ex);
      }
    }

    private static void SetGameAwareOnboardingPackages(
      PostBootCloudInfo currentPostBootCloudInfo,
      JToken res)
    {
      try
      {
        JToken jtoken = JToken.Parse(res.GetValue("game_aware_onboarding"));
        if (jtoken[(object) "app_pkg_list"] == null || !(jtoken[(object) "app_pkg_list"] is JArray jarray))
          return;
        currentPostBootCloudInfo.GameAwareOnboardingInfo.GameAwareOnBoardingAppPackages = new AppPackageListObject(jarray.ToObject<List<string>>());
      }
      catch (Exception ex)
      {
        Logger.Warning("Exception in parsing game aware onboarding packages: " + ex.ToString());
      }
    }

    private static void SetHomeHtmlErrorHandlingFlag(JToken res)
    {
      try
      {
        if (res[(object) "homehtml_error_handling"] == null)
          return;
        RegistryManager.Instance.HomeHtmlErrorHandling = bool.Parse(res[(object) "homehtml_error_handling"].ToString().Trim());
      }
      catch (Exception ex)
      {
        Logger.Warning("Exception in SetHomeHtmlErrorHandlingFlag: " + ex.ToString());
      }
    }
  }
}
