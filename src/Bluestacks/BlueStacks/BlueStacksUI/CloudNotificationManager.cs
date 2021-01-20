// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.CloudNotificationManager
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace BlueStacks.BlueStacksUI
{
  internal sealed class CloudNotificationManager
  {
    private static object syncRoot = new object();
    private static SerialWorkQueue mWorkQueue = (SerialWorkQueue) null;
    private static volatile CloudNotificationManager sInstance;

    private CloudNotificationManager()
    {
    }

    public static CloudNotificationManager Instance
    {
      get
      {
        if (CloudNotificationManager.sInstance == null)
        {
          lock (CloudNotificationManager.syncRoot)
          {
            if (CloudNotificationManager.sInstance == null)
              CloudNotificationManager.sInstance = new CloudNotificationManager();
          }
        }
        return CloudNotificationManager.sInstance;
      }
    }

    private static SerialWorkQueue WorkQueue
    {
      get
      {
        if (CloudNotificationManager.mWorkQueue == null)
        {
          lock (CloudNotificationManager.syncRoot)
          {
            if (CloudNotificationManager.mWorkQueue == null)
            {
              CloudNotificationManager.mWorkQueue = new SerialWorkQueue("androidCloudNotifications");
              if (BlueStacksUIUtils.DictWindows[Strings.CurrentDefaultVmName].mGuestBootCompleted)
                CloudNotificationManager.mWorkQueue.Start();
            }
          }
        }
        return CloudNotificationManager.mWorkQueue;
      }
    }

    internal void HandleCloudNotification(string jsonReceived, string vmName)
    {
      try
      {
        Logger.Info("CloudFireBaseNotification response received: " + jsonReceived + " from vm: " + vmName);
        JObject jobject = JObject.Parse(jsonReceived);
        if (jobject["bluestacks_notification"] != null && jobject["bluestacks_notification"].ToObject<JObject>()["tag"] != null && !JsonExtensions.IsNullOrEmptyBrackets(jobject["bluestacks_notification"].GetValue("tag")))
          CloudNotificationManager.HandleTagsInfo(jobject, jsonReceived);
        if (jobject["bluestacks_notification"] == null || jobject["bluestacks_notification"].ToObject<JObject>()["type"] == null)
          return;
        switch (jobject["bluestacks_notification"][(object) "type"].ToString().ToLower(CultureInfo.InvariantCulture))
        {
          case "genericnotification":
            CloudNotificationManager.HandleGenericNotification(jobject, vmName);
            break;
          case "genericreddotnotification":
            CloudNotificationManager.HandleGenericRedDotNotification(jobject, vmName);
            break;
          case "callmethod":
            CloudNotificationManager.HandleCallMethod(jobject, vmName);
            break;
          default:
            Logger.Warning("No notification type found in HandleCloudNotification. json: " + jsonReceived);
            break;
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in HandleCloudNotification. json: " + jsonReceived + " Error: " + ex.ToString());
      }
    }

    private static void HandleTagsInfo(JObject json, string jsonReceived)
    {
      try
      {
        foreach (string _ in json["bluestacks_notification"][(object) "tag"].ToObject<List<string>>())
        {
          if (BrowserControl.mFirebaseTagsSubscribed.Contains(_))
            CloudNotificationManager.SendNotifJsonToHtmlTag(_, jsonReceived);
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in HandleTagsInfo: " + ex.ToString());
      }
    }

    private static void SendNotifJsonToHtmlTag(string _, string data)
    {
      try
      {
        object[] args = new object[1]{ (object) "" };
        if (!string.IsNullOrEmpty(data))
          args[0] = (object) data;
        foreach (BrowserControl allBrowserControl in BrowserControl.sAllBrowserControls)
        {
          if (allBrowserControl != null && allBrowserControl.CefBrowser != null)
            allBrowserControl.CefBrowser.CallJs(allBrowserControl.mFirebaseCallbackMethod, args);
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in sending json to appcenter:" + ex.ToString());
      }
    }

    internal static SerializableDictionary<string, string> HandleExtraPayload(
      JObject json,
      NotificationPayloadType payloadType)
    {
      return json.ToObject<SerializableDictionary<string, string>>();
    }

    internal static void HandleGenericRedDotNotification(JObject resJson, string vmName)
    {
      JObject jobject = JObject.Parse(resJson["bluestacks_notification"][(object) "payload"][(object) "GenericRedDotNotificationItem"].ToString());
      if (JsonExtensions.IsNullOrEmptyBrackets(jobject["nyapps_cross_promotion"].ToString()))
        return;
      PromotionManager.AddNewMyAppsCrossPromotion((JToken) jobject);
      PromotionObject.Save();
      string appPackage = jobject["nyapps_cross_promotion"][(object) "app_pkg"].ToString();
      BlueStacksUIUtils.DictWindows[vmName].Dispatcher.Invoke((Delegate) (() => BlueStacksUIUtils.DictWindows[vmName].mWelcomeTab.mHomeAppManager.AddIconWithRedDot(appPackage)));
    }

    internal static void HandleGenericNotification(JObject resJson, string vmName)
    {
      GenericNotificationItem genericItem = new GenericNotificationItem();
      try
      {
        JObject resJson1 = JObject.Parse(resJson["bluestacks_notification"][(object) "payload"][(object) "GenericNotificationItem"].ToString());
        resJson1.AssignIfContains<string>("id", (System.Action<string>) (x => genericItem.Id = x));
        resJson1.AssignIfContains<string>("priority", (System.Action<string>) (x => genericItem.Priority = EnumHelper.Parse<NotificationPriority>(x, NotificationPriority.Normal)));
        resJson1.AssignIfContains<string>("title", (System.Action<string>) (x => genericItem.Title = x));
        resJson1.AssignIfContains<string>("message", (System.Action<string>) (x => genericItem.Message = x));
        resJson1.AssignIfContains<bool>("showribbon", (System.Action<bool>) (x => genericItem.ShowRibbon = x));
        resJson1.AssignIfContains<string>("menuimagename", (System.Action<string>) (x => genericItem.NotificationMenuImageName = x));
        resJson1.AssignIfContains<string>("menuimageurl", (System.Action<string>) (x => genericItem.NotificationMenuImageUrl = x));
        resJson1.AssignIfContains<bool>("isread", (System.Action<bool>) (x => genericItem.IsRead = x));
        resJson1.AssignIfContains<bool>("isdeleted", (System.Action<bool>) (x => genericItem.IsDeleted = x));
        resJson1.AssignIfContains<bool>("deferred", (System.Action<bool>) (x => genericItem.IsDeferred = x));
        resJson1.AssignIfContains<string>("creationtime", (System.Action<string>) (x => genericItem.CreationTime = DateTime.ParseExact(x, "yyyy/MM/dd HH:mm:ss", (IFormatProvider) CultureInfo.InvariantCulture)));
        if (!string.IsNullOrEmpty(genericItem.NotificationMenuImageName) && !string.IsNullOrEmpty(genericItem.NotificationMenuImageUrl))
          genericItem.NotificationMenuImageName = Utils.TinyDownloader(genericItem.NotificationMenuImageUrl, genericItem.NotificationMenuImageName, RegistryStrings.PromotionDirectory, false);
        if (resJson1["ExtraPayload"] != null && !JsonExtensions.IsNullOrEmptyBrackets(resJson1.GetValue("ExtraPayload", StringComparison.InvariantCulture).ToString()))
        {
          resJson1["ExtraPayload"].AssignIfContains<string>("payloadtype", (System.Action<string>) (x => genericItem.PayloadType = EnumHelper.Parse<NotificationPayloadType>(x, NotificationPayloadType.Generic)));
          SerializableDictionary<string, string> extraPayload = genericItem.ExtraPayload;
          if (extraPayload != null)
            extraPayload.ClearAddRange<string, string>((Dictionary<string, string>) CloudNotificationManager.HandleExtraPayload(resJson1.GetValue("ExtraPayload", StringComparison.InvariantCulture).ToObject<JObject>(), genericItem.PayloadType));
        }
        ClientStats.SendMiscellaneousStatsAsync("notification_received", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, genericItem.Id, genericItem.Title, genericItem.ExtraPayload.ContainsKey("campaign_id") ? genericItem.ExtraPayload["campaign_id"] : "", (string) null, (string) null, (string) null, "Android");
        genericItem.IsReceivedStatSent = true;
        if (resJson1["conditions"] != null && !JsonExtensions.IsNullOrEmptyBrackets(resJson1.GetValue("conditions", StringComparison.InvariantCulture).ToString()))
        {
          resJson1["conditions"].AssignIfContains<string>("app_pkg_on_top", (System.Action<string>) (x => genericItem.DeferredApp = x));
          resJson1["conditions"].AssignIfContains<long>("app_usage_seconds", (System.Action<long>) (x => genericItem.DeferredAppUsage = x));
        }
        if (genericItem.ShowRibbon)
        {
          if (resJson["bluestacks_notification"][(object) "payload"].ToObject<JObject>()["RibbonDesign"] != null)
          {
            if (!JsonExtensions.IsNullOrEmptyBrackets(resJson["bluestacks_notification"][(object) "payload"].GetValue("RibbonDesign")))
            {
              genericItem.NotificationDesignItem = new GenericNotificationDesignItem();
              JObject resJson2 = JObject.Parse(resJson["bluestacks_notification"][(object) "payload"][(object) "RibbonDesign"].ToString());
              resJson2.AssignIfContains<string>("titleforegroundcolor", (System.Action<string>) (x => genericItem.NotificationDesignItem.TitleForeGroundColor = x));
              resJson2.AssignIfContains<string>("messageforegroundcolor", (System.Action<string>) (x => genericItem.NotificationDesignItem.MessageForeGroundColor = x));
              resJson2.AssignIfContains<string>("bordercolor", (System.Action<string>) (x => genericItem.NotificationDesignItem.BorderColor = x));
              resJson2.AssignIfContains<string>("ribboncolor", (System.Action<string>) (x => genericItem.NotificationDesignItem.Ribboncolor = x));
              resJson2.AssignIfContains<double>("auto_hide_timer", (System.Action<double>) (x => genericItem.NotificationDesignItem.AutoHideTime = x));
              resJson2.AssignIfContains<string>("hoverbordercolor", (System.Action<string>) (x => genericItem.NotificationDesignItem.HoverBorderColor = x));
              resJson2.AssignIfContains<string>("hoverribboncolor", (System.Action<string>) (x => genericItem.NotificationDesignItem.HoverRibboncolor = x));
              resJson2.AssignIfContains<string>("leftgifname", (System.Action<string>) (x => genericItem.NotificationDesignItem.LeftGifName = x));
              resJson2.AssignIfContains<string>("leftgifurl", (System.Action<string>) (x => genericItem.NotificationDesignItem.LeftGifUrl = x));
              if (!string.IsNullOrEmpty(genericItem.NotificationDesignItem.LeftGifName) && !string.IsNullOrEmpty(genericItem.NotificationDesignItem.LeftGifUrl))
                Utils.TinyDownloader(genericItem.NotificationDesignItem.LeftGifUrl, genericItem.NotificationDesignItem.LeftGifName, RegistryStrings.PromotionDirectory, false);
              if (resJson2["background_gradient"] != null)
              {
                foreach (JObject jobject in JArray.Parse(resJson2["background_gradient"].ToString()).ToObject<List<JObject>>())
                  genericItem.NotificationDesignItem.BackgroundGradient.Add(new SerializableKeyValuePair<string, double>(jobject["color"].ToString(), jobject["offset"].ToObject<double>()));
              }
              if (resJson2["hover_background_gradient"] != null)
              {
                foreach (JObject jobject in JArray.Parse(resJson2["hover_background_gradient"].ToString()).ToObject<List<JObject>>())
                  genericItem.NotificationDesignItem.HoverBackGroundGradient.Add(new SerializableKeyValuePair<string, double>(jobject["color"].ToString(), jobject["offset"].ToObject<double>()));
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Exception while parsing generic notification. Not showing notification and not adding in notification menu." + ex.ToString());
        return;
      }
      try
      {
        if (string.IsNullOrEmpty(genericItem.Title) && string.IsNullOrEmpty(genericItem.Message))
          genericItem.IsDeleted = true;
        if (!genericItem.IsDeferred)
          GenericNotificationManager.AddNewNotification(genericItem, false);
        if (genericItem.ShowRibbon && resJson["bluestacks_notification"][(object) "payload"].ToObject<JObject>()["RibbonDesign"] != null && !JsonExtensions.IsNullOrEmptyBrackets(resJson["bluestacks_notification"][(object) "payload"].GetValue("RibbonDesign")))
        {
          if (!genericItem.IsDeferred)
            BlueStacksUIUtils.DictWindows[vmName].HandleGenericNotificationPopup(genericItem);
          else
            CloudNotificationManager.HandleDeferredNotification(genericItem);
        }
        BlueStacksUIUtils.DictWindows[vmName].mTopBar.RefreshNotificationCentreButton();
      }
      catch (Exception ex)
      {
        Logger.Error("Exception when handling notification json. Id " + genericItem.Id + " Error: " + ex.ToString());
      }
    }

    private static void HandleDeferredNotification(GenericNotificationItem genericItem)
    {
      PromotionManager.sDeferredNotificationsList.Add(genericItem, AppUsageTimer.GetTotalTimeForPackageAfterReset(genericItem.DeferredApp.ToLower(CultureInfo.InvariantCulture)));
    }

    internal static void HandleCallMethod(JObject resJson, string vmName)
    {
      string result = "";
      JObject.Parse(resJson["bluestacks_notification"][(object) "payload"].ToString()).AssignStringIfContains("methodName", ref result);
      string lower = result.ToLower(CultureInfo.InvariantCulture);
      if (lower != null)
      {
        // ISSUE: reference to a compiler-generated method
        switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(lower))
        {
          case 443221764:
            if (lower == "updategrm")
            {
              GrmManager.UpdateGrmAsync(resJson["bluestacks_notification"][(object) "payload"][(object) "app_pkg_list"].ToIenumerableString());
              return;
            }
            break;
          case 886249708:
            if (lower == "openquitpopup")
            {
              CloudNotificationManager.OpenQuitPopup(resJson, vmName);
              return;
            }
            break;
          case 1238708169:
            if (lower == "calendarentry")
            {
              try
              {
                JObject androidPayload = (JObject) resJson["bluestacks_notification"][(object) "payload"][(object) "androidPayload"];
                ClientStats.SendCalendarStats("calendar_" + resJson["bluestacks_notification"][(object) "payload"][(object) "methodType"].ToString() + "_firebase", androidPayload.ContainsKey("startDate") ? androidPayload["startDate"].ToString() : "", androidPayload.ContainsKey("endDate") ? androidPayload["endDate"].ToString() : "", androidPayload["location"].ToString(), "", "");
                string str;
                switch (resJson["bluestacks_notification"][(object) "payload"][(object) "methodType"].ToString())
                {
                  case "add":
                    str = "addcalendarevent";
                    break;
                  case "update":
                    str = "updatecalendarevent";
                    break;
                  case "delete":
                    str = "deletecalendarevent";
                    break;
                  default:
                    throw new Exception("could not identify the methodType ");
                }
                string endpoint = str;
                JObject jobject1 = new JObject((object) new JProperty("event", (object) androidPayload));
                Dictionary<string, string> data = new Dictionary<string, string>()
                {
                  ["event"] = jobject1.ToString()
                };
                CloudNotificationManager.WorkQueue.Enqueue((SerialWorkQueue.Work) (() =>
                {
                  try
                  {
                    string guest = HTTPUtils.SendRequestToGuest(endpoint, data, vmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
                    Logger.Info("Response for calendarEntry " + guest);
                    JObject jobject = JObject.Parse(guest);
                    ClientStats.SendCalendarStats("calendar_" + resJson["bluestacks_notification"][(object) "payload"][(object) "methodType"].ToString() + "_android", androidPayload.ContainsKey("startDate") ? androidPayload["startDate"].ToString() : "", androidPayload.ContainsKey("endDate") ? androidPayload["endDate"].ToString() : "", androidPayload["location"].ToString(), string.Equals(jobject["result"].ToString(), "ok", StringComparison.InvariantCultureIgnoreCase).ToString((IFormatProvider) CultureInfo.InvariantCulture), jobject.ContainsKey("rowsDeleted") ? jobject["rowsDeleted"].ToString() : (jobject.ContainsKey("rowsUpdated") ? jobject["rowsUpdated"].ToString() : ""));
                  }
                  catch (Exception ex)
                  {
                    Logger.Warning(string.Format("Guest not booted, error in sending Calendar entry event: {0}", (object) ex));
                  }
                }));
                return;
              }
              catch (Exception ex)
              {
                Logger.Warning(string.Format("Error in sending Calendar entry event data to android.. Json:{0} error:  {1}", (object) resJson, (object) ex));
                return;
              }
            }
            else
              break;
          case 1286957143:
            if (lower == "updatebstconfig")
            {
              CloudNotificationManager.UpdateBstConfig();
              return;
            }
            break;
          case 1330308668:
            if (lower == "downloadkeymappingcfg")
            {
              try
              {
                if (resJson["bluestacks_notification"][(object) "payload"][(object) "parserVersionList"] != null && resJson["bluestacks_notification"][(object) "payload"][(object) "parserVersionList"] is JArray jarray)
                {
                  List<string> stringList = jarray.ToObject<List<string>>();
                  int num1 = int.Parse(KMManager.MinParserVersion, (IFormatProvider) CultureInfo.InvariantCulture);
                  int num2 = int.Parse(KMManager.ParserVersion, (IFormatProvider) CultureInfo.InvariantCulture);
                  using (List<string>.Enumerator enumerator = stringList.GetEnumerator())
                  {
                    while (enumerator.MoveNext())
                    {
                      int num3 = int.Parse(enumerator.Current, (IFormatProvider) CultureInfo.InvariantCulture);
                      if (num3 <= num2 && num3 >= num1)
                      {
                        string packageName = resJson["bluestacks_notification"][(object) "payload"][(object) "pkgName"].ToString();
                        Logger.Info("downloadkeymappingcfg request sent to Android due to pv " + num3.ToString() + " pkg:" + packageName);
                        Utils.SendKeymappingFiledownloadRequest(packageName, vmName);
                        break;
                      }
                    }
                    return;
                  }
                }
                else
                {
                  Logger.Warning("Not processing downloadkeymappingcfg as parserVersionList not found");
                  return;
                }
              }
              catch (Exception ex)
              {
                Logger.Warning("Error in sending download keymapping cfg request " + ex?.ToString());
                return;
              }
            }
            else
              break;
          case 3602408690:
            if (lower == "updatepromotions")
            {
              PromotionManager.ReloadPromotionsAsync();
              return;
            }
            break;
          case 4066957630:
            if (lower == "appusagestats")
            {
              CloudNotificationManager.HandleUsageNotification(resJson, vmName);
              return;
            }
            break;
        }
      }
      Logger.Error("No method type found in HandleCallMethod json: " + resJson?.ToString());
    }

    internal static void UpdateBstConfig()
    {
      RegistryManager.Instance.UpdateBstConfig = true;
      FeatureManager.Init(true, true);
    }

    internal static void OpenQuitPopup(JObject resJson, string vmName)
    {
      string result1 = "";
      string result2 = "";
      string result3 = "";
      string result4 = "";
      JObject resJson1 = JObject.Parse(resJson["bluestacks_notification"][(object) "payload"].ToString());
      resJson1.AssignStringIfContains("url", ref result1);
      if (!string.IsNullOrEmpty(result1))
      {
        resJson1.AssignStringIfContains("app_pkg", ref result2);
        resJson1.AssignStringIfContains("force_reload", ref result3);
        resJson1.AssignStringIfContains("show_on_quit", ref result4);
        bool result5;
        bool isForceReload = bool.TryParse(result3, out result5) & result5;
        bool result6;
        bool showOnQuit = bool.TryParse(result4, out result6) & result6;
        string urlWithParams = WebHelper.GetUrlWithParams(result1, (string) null, (string) null, (string) null);
        BlueStacksUIUtils.DictWindows[vmName].IsQuitPopupNotficationReceived = true;
        if (BlueStacksUIUtils.DictWindows[vmName].mQuitPopupBrowserControl == null)
          BlueStacksUIUtils.DictWindows[vmName].mQuitPopupBrowserControl = new QuitPopupBrowserControl(BlueStacksUIUtils.DictWindows[vmName]);
        BlueStacksUIUtils.DictWindows[vmName].mQuitPopupBrowserControl.SetQuitPopParams(urlWithParams, result2, isForceReload, showOnQuit);
        BlueStacksUIUtils.DictWindows[vmName].mQuitPopupBrowserControl.LoadBrowser();
      }
      else
      {
        Logger.Info("Quit Popup notification received without url");
        BlueStacksUIUtils.DictWindows[vmName].IsQuitPopupNotficationReceived = false;
      }
    }

    internal static void HandleUsageNotification(JObject resJson, string vmName)
    {
      try
      {
        string result = "";
        string jsonObjectString = JSONUtils.GetJSONObjectString(AppUsageTimer.GetRealtimeDictionary());
        JObject.Parse(resJson["bluestacks_notification"][(object) "payload"].ToString()).AssignStringIfContains("handler", ref result);
        string url = WebHelper.GetServerHost() + "/v2/" + result;
        Dictionary<string, string> dictionary = new Dictionary<string, string>()
        {
          ["oem"] = "bgp",
          ["client_ver"] = RegistryManager.Instance.ClientVersion,
          ["engine_ver"] = RegistryManager.Instance.Version,
          ["guid"] = RegistryManager.Instance.UserGuid,
          ["locale"] = RegistryManager.Instance.UserSelectedLocale,
          ["partner"] = RegistryManager.Instance.Partner,
          ["campaignMD5"] = RegistryManager.Instance.CampaignMD5
        };
        if (!string.IsNullOrEmpty(RegistryManager.Instance.RegisteredEmail))
          dictionary["email"] = RegistryManager.Instance.RegisteredEmail;
        dictionary["usage_data"] = jsonObjectString;
        if (!dictionary.ContainsKey("current_app"))
          dictionary.Add("current_app", BlueStacksUIUtils.DictWindows[vmName].mTopBar.mAppTabButtons.SelectedTab.PackageName);
        else
          dictionary["current_app"] = BlueStacksUIUtils.DictWindows[vmName].mTopBar.mAppTabButtons.SelectedTab.PackageName;
        Dictionary<string, string> data = dictionary;
        string empty = string.Empty;
        Logger.Info("real time app usage response:" + BstHttpClient.Post(url, data, (Dictionary<string, string>) null, false, empty, 0, 1, 0, false, "bgp"));
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in handling usage notification" + ex.ToString());
      }
    }

    internal static void PostBootCompleted()
    {
      if (CloudNotificationManager.mWorkQueue == null)
        return;
      CloudNotificationManager.mWorkQueue.Start();
    }
  }
}
