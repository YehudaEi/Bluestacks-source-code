// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.MyCustomCefV8Handler
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.BlueStacksUI.BTv;
using BlueStacks.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Xilium.CefGlue;

namespace BlueStacks.BlueStacksUI
{
  internal class MyCustomCefV8Handler : CefV8Handler
  {
    private static object sLock = new object();
    private static string vmName = "";
    private string mCallBackFunction;

    protected override bool Execute(
      string name,
      CefV8Value obj,
      CefV8Value[] arguments,
      out CefV8Value returnValue,
      out string exception)
    {
      returnValue = CefV8Value.CreateString("");
      this.ReceiveJsFunctionCall(arguments, ref returnValue);
      exception = (string) null;
      return true;
    }

    private void ReceiveJsFunctionCall(CefV8Value[] arguments, ref CefV8Value returnValue)
    {
      JObject jobject = JObject.Parse(arguments[0].GetStringValue());
      string json = jobject["data"].ToString();
      if (string.IsNullOrEmpty(json) || json.Equals("null", StringComparison.InvariantCultureIgnoreCase))
        json = "[]";
      JArray token = JArray.Parse(json);
      object[] objArray = (object[]) null;
      if (!token.IsNullOrEmpty())
      {
        objArray = new object[token.Count];
        for (int index = 0; index < token.Count; ++index)
          objArray[index] = (object) token[index].ToString();
      }
      try
      {
        try
        {
          if (jobject.ContainsKey("callbackFunction"))
            this.mCallBackFunction = jobject["callbackFunction"].ToString();
        }
        catch
        {
          Logger.Info("Error in callback function name.");
        }
        if (jobject["calledFunction"].ToString().IndexOf("LogInfo", StringComparison.InvariantCulture) == -1)
          Logger.Debug("Calling function from GM API.." + jobject["calledFunction"].ToString());
        jobject["calledFunction"].ToString();
        MethodInfo method = this.GetType().GetMethod(jobject["calledFunction"].ToString());
        ParameterInfo[] parameters1 = method.GetParameters();
        object[] parameters2 = new object[parameters1.Length];
        for (int index = 0; index < parameters2.Length; ++index)
        {
          if (objArray != null && index < objArray.Length)
          {
            parameters2[index] = objArray[index];
          }
          else
          {
            if (!parameters1[index].IsOptional)
              throw new ArgumentException("Not enough arguments provided");
            parameters2[index] = parameters1[index].DefaultValue;
          }
        }
        object obj = method.Invoke((object) this, parameters2);
        if (obj == null)
          return;
        returnValue = CefV8Value.CreateString((string) obj);
      }
      catch (Exception ex)
      {
        Logger.Info("Error in ReceiveJSFunctionCall: " + ex.ToString());
      }
    }

    internal void OnProcessMessageReceived(CefProcessMessage message)
    {
      Logger.Info("message received in render process." + message.Name);
      switch (message.Name)
      {
        case "SetVmName":
          MyCustomCefV8Handler.vmName = message.Arguments.GetString(0);
          break;
      }
    }

    public string isBTVInstalled()
    {
      if (!BTVManager.IsBTVInstalled())
      {
        using (CefProcessMessage message = CefProcessMessage.Create("DownloadBTV"))
        {
          CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, message);
          return "false";
        }
      }
      else
      {
        if (BTVManager.IsDirectXComponentsInstalled())
          return "true";
        using (CefProcessMessage message = CefProcessMessage.Create("DownloadDirectX"))
        {
          CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, message);
          return "false";
        }
      }
    }

    public void sendFirebaseNotification(string data)
    {
      Logger.Debug("Got call for sendFirebaseNotification");
      using (CefProcessMessage message = CefProcessMessage.Create(nameof (sendFirebaseNotification)))
      {
        message.Arguments.SetString(0, data);
        CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, message);
      }
    }

    public void subscribeModule(string tag)
    {
      Logger.Info("Subscribe html module");
      using (CefProcessMessage message = CefProcessMessage.Create(nameof (subscribeModule)))
      {
        message.Arguments.SetString(0, tag);
        CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, message);
      }
    }

    public void UnsubscribeModule(string tag)
    {
      Logger.Info("Unsubscribe html module");
      using (CefProcessMessage message = CefProcessMessage.Create("unsubscribeModule"))
      {
        message.Arguments.SetString(0, tag);
        CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, message);
      }
    }

    public void subscribeToClientTags(string json)
    {
      Logger.Info("Subscribe to client tags");
      using (CefProcessMessage message = CefProcessMessage.Create("subscribeClientTags"))
      {
        message.Arguments.SetString(0, json);
        CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, message);
      }
    }

    public void subscribeToVmSpecificClientTags(string json)
    {
      Logger.Info("Subscribe to vm specific client tags");
      using (CefProcessMessage message = CefProcessMessage.Create("subscribeVmSpecificClientTags"))
      {
        message.Arguments.SetString(0, json);
        CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, message);
      }
    }

    public void UnsubscribeToClientTags(string json)
    {
      Logger.Info("Unsubscribe to client tags");
      using (CefProcessMessage message = CefProcessMessage.Create("unsubscribeClientTags"))
      {
        message.Arguments.SetString(0, json);
        CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, message);
      }
    }

    public void HandleClick(string json)
    {
      using (CefProcessMessage message = CefProcessMessage.Create(nameof (HandleClick)))
      {
        message.Arguments.SetString(0, json);
        CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, message);
      }
    }

    public void UpdateQuestRules(string json)
    {
      using (CefProcessMessage message = CefProcessMessage.Create(nameof (UpdateQuestRules)))
      {
        message.Arguments.SetString(0, json);
        CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, message);
      }
    }

    public void pikaworldprofileadded(string profileId)
    {
      Logger.Debug("Got call for PikaWorldProfileAdded");
      using (CefProcessMessage message = CefProcessMessage.Create("PikaWorldProfileAdded"))
      {
        message.Arguments.SetString(0, profileId);
        CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, message);
      }
    }

    public string getPikaWorldUserId()
    {
      return RegistryManager.Instance.PikaWorldId;
    }

    public string getBootTime()
    {
      return RegistryManager.Instance.LastBootTime.ToString((IFormatProvider) CultureInfo.InvariantCulture);
    }

    public void getGamepadConnectionStatus()
    {
      Logger.Debug("Got call for getGamepadConnectionStatus");
      using (CefProcessMessage message = CefProcessMessage.Create("GetGamepadConnectionStatus"))
        CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, message);
    }

    public string IsAnyAppRunning()
    {
      string route = "isAnyAppRunning";
      try
      {
        return HTTPUtils.SendRequestToClient(route, (Dictionary<string, string>) null, "Android", 0, (Dictionary<string, string>) null, false, 1, 0, "bgp").ToLower(CultureInfo.InvariantCulture);
      }
      catch (Exception ex)
      {
        Logger.Error("An unexpected error occured in {0}. Err: {1}", (object) route, (object) ex.ToString());
        return (string) null;
      }
    }

    public void goToMapsTab()
    {
      using (CefProcessMessage message = CefProcessMessage.Create("GoToMapsTab"))
        CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, message);
    }

    public void clearmapsnotification()
    {
      Logger.Info("Got call from browser for maps clear notification");
      using (CefProcessMessage message = CefProcessMessage.Create("ClearMapsNotification"))
        CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, message);
    }

    public string installapp(
      string appIcon,
      string appName,
      string apkUrl,
      string packageName,
      string timestamp = "",
      string source = "")
    {
      Logger.Info("Get Call from browser of Install App :" + appName);
      using (CefProcessMessage message = CefProcessMessage.Create("InstallApp"))
      {
        CefListValue arguments = message.Arguments;
        arguments.SetString(0, appIcon);
        arguments.SetString(1, appName);
        arguments.SetString(2, apkUrl);
        arguments.SetString(3, packageName);
        arguments.SetString(4, timestamp);
        arguments.SetString(5, source);
        CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, message);
        return "true";
      }
    }

    public void installapp_google(
      string appIcon,
      string appName,
      string apkUrl,
      string packageName,
      string source = "")
    {
      Logger.Info("Get Call from browser of Install App from googleplay :" + appName);
      using (CefProcessMessage message = CefProcessMessage.Create("InstallAppGooglePlay"))
      {
        CefListValue arguments = message.Arguments;
        arguments.SetString(0, appIcon);
        arguments.SetString(1, appName);
        arguments.SetString(2, apkUrl);
        arguments.SetString(3, packageName);
        arguments.SetString(4, source);
        CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, message);
      }
    }

    public void installapp_google_popup(
      string appIcon,
      string appName,
      string apkUrl,
      string packageName,
      string source = "")
    {
      Logger.Info("Get Call from browser of Install App from googleplay in popup :" + appName);
      using (CefProcessMessage message = CefProcessMessage.Create("InstallAppGooglePlayPopup"))
      {
        CefListValue arguments = message.Arguments;
        arguments.SetString(0, appIcon);
        arguments.SetString(1, appName);
        arguments.SetString(2, apkUrl);
        arguments.SetString(3, packageName);
        arguments.SetString(4, source);
        CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, message);
      }
    }

    public void downloadinstalloem(string oem, string abiValue)
    {
      Logger.Info("Get Call from browser of downloadoem oem: " + oem + ", abiValue: " + abiValue);
      using (CefProcessMessage message = CefProcessMessage.Create("DownloadInstallOem"))
      {
        CefListValue arguments = message.Arguments;
        arguments.SetString(0, oem);
        arguments.SetString(1, abiValue);
        CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, message);
      }
    }

    public void canceloemdownload(string oem, string abiValue)
    {
      Logger.Info("Get Call from browser of canceloemdownload oem: " + oem + ", abiValue: " + abiValue);
      using (CefProcessMessage message = CefProcessMessage.Create("CancelOemDownload"))
      {
        CefListValue arguments = message.Arguments;
        arguments.SetString(0, oem);
        arguments.SetString(1, abiValue);
        CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, message);
      }
    }

    public void launchappindifferentoem(
      string oem,
      string abiValue,
      string vmname,
      string packageName,
      string actionWithRemainingInstances)
    {
      Logger.Info("Get Call from browser of launchappindifferentoem oem: " + oem + ", abiValue: " + abiValue + ", packageName: " + packageName + ", actionWithRemainingInstances: " + actionWithRemainingInstances);
      using (CefProcessMessage message = CefProcessMessage.Create("LaunchAppInDifferentOem"))
      {
        CefListValue arguments = message.Arguments;
        arguments.SetString(0, oem);
        arguments.SetString(1, abiValue);
        arguments.SetString(2, vmname);
        arguments.SetString(3, packageName);
        arguments.SetString(4, actionWithRemainingInstances);
        CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, message);
      }
    }

    public void uninstallapp(string packageName)
    {
      Logger.Info("Get Call from browser of Uninstall App for packagename :" + packageName);
      using (CefProcessMessage message = CefProcessMessage.Create("UninstallApp"))
      {
        message.Arguments.SetString(0, packageName);
        CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, message);
      }
    }

    public void getupdatedgrm()
    {
      Logger.Info("Got call from browser to get updated grm");
      using (CefProcessMessage message = CefProcessMessage.Create("GetUpdatedGrm"))
        CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, message);
    }

    public void retryapkinstall(string apkFilePath)
    {
      Logger.Info("Get Call from browser of RetryApkInstall :" + apkFilePath);
      using (CefProcessMessage message = CefProcessMessage.Create("RetryApkInstall"))
      {
        message.Arguments.SetString(0, apkFilePath);
        CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, message);
      }
    }

    public void chooseandinstallapk()
    {
      Logger.Info("Get Call from browser of ChooseAndInstallApk :");
      using (CefProcessMessage message = CefProcessMessage.Create("ChooseAndInstallApk"))
        CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, message);
    }

    public void checkifpremium(string isPremium)
    {
      Logger.Info("Got call from blocker ad browser of premium subscription");
      using (CefProcessMessage message = CefProcessMessage.Create("CheckIfPremium"))
      {
        message.Arguments.SetString(0, isPremium);
        CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, message);
      }
    }

    public void applyTheme(string themeName)
    {
      using (CefProcessMessage message = CefProcessMessage.Create("ApplyThemeName"))
      {
        message.Arguments.SetString(0, themeName);
        CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, message);
      }
    }

    public List<string> getsupportedactiontypes()
    {
      return ((IEnumerable<string>) Enum.GetNames(typeof (GenericAction))).ToList<string>();
    }

    public void getimpressionid(string impressionId)
    {
      Logger.Info("Get call from browser of impression_id :" + impressionId);
      using (CefProcessMessage message = CefProcessMessage.Create("GetImpressionId"))
      {
        message.Arguments.SetString(0, impressionId);
        CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, message);
        message.Dispose();
      }
    }

    public string installedapps()
    {
      List<AppInfo> list = ((IEnumerable<AppInfo>) new JsonParser("Android").GetAppList()).ToList<AppInfo>();
      string str = string.Empty;
      foreach (AppInfo appInfo in list)
        str = str + appInfo.Package + ",";
      return str;
    }

    public string installedappsforvm(string vmName)
    {
      if (string.IsNullOrEmpty(vmName))
        vmName = "Android";
      List<AppInfo> list = ((IEnumerable<AppInfo>) new JsonParser(vmName).GetAppList()).ToList<AppInfo>();
      string str = string.Empty;
      foreach (AppInfo appInfo in list)
        str = str + appInfo.Package + ",";
      return str;
    }

    public void openapp(string appIcon, string appName, string apkUrl, string packageName)
    {
      Logger.Info("Get Call from browser of open App :" + appName);
      using (CefProcessMessage message = CefProcessMessage.Create("InstallApp"))
      {
        CefListValue arguments = message.Arguments;
        arguments.SetString(0, appIcon);
        arguments.SetString(1, appName);
        arguments.SetString(2, apkUrl);
        arguments.SetString(3, packageName);
        CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, message);
      }
    }

    public void showdetails(string url)
    {
      using (CefBrowser browser = CefV8Context.GetCurrentContext().GetBrowser())
      {
        CefProcessMessage message = CefProcessMessage.Create(url);
        browser.SendProcessMessage(CefProcessId.Browser, message);
        message.Dispose();
      }
    }

    public void searchappcenter(string searchString)
    {
      using (CefProcessMessage message = CefProcessMessage.Create("SearchAppcenter"))
      {
        message.Arguments.SetString(0, searchString);
        CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, message);
      }
    }

    public void browser2Client()
    {
    }

    public string getguid()
    {
      return RegistryManager.Instance.UserGuid;
    }

    public string GetUserGUID()
    {
      return RegistryManager.Instance.UserGuid;
    }

    public void feedback(string email, string appName, string description, string downloadURl)
    {
      string clientVersion = RegistryManager.Instance.ClientVersion;
      description = description.Replace("&", " ");
      description += "\n From Client VER=";
      description += clientVersion;
      string str = "-startwithparam \"" + email + "&Others&" + appName + "&" + description + "&" + downloadURl + "\"";
      using (Process process = new Process())
      {
        process.StartInfo.FileName = Path.Combine(RegistryStrings.InstallDir, "HD-LogCollector.exe");
        Logger.Info("The arguments being passed to log collector is :{0}", (object) str);
        process.StartInfo.Arguments = str;
        process.Start();
      }
    }

    public void openLogCollector(string data = "")
    {
      string installDir = RegistryStrings.InstallDir;
      using (Process process = new Process())
      {
        process.StartInfo.FileName = Path.Combine(installDir, "HD-LogCollector.exe");
        string str = string.Empty;
        if (!string.IsNullOrEmpty(MyCustomCefV8Handler.vmName))
          str = "-Vmname=" + MyCustomCefV8Handler.vmName;
        if (!string.IsNullOrEmpty(data))
        {
          str += " -QuickLogs";
          if (!string.IsNullOrEmpty(RegistryManager.Instance.ClientInstallDir))
            File.WriteAllText(Path.Combine(RegistryManager.Instance.ClientInstallDir, "logCollectorSourceData.txt"), data);
        }
        process.StartInfo.Arguments = str;
        Logger.Info("Open log collector through gmApi from dir: " + installDir);
        process.Start();
      }
    }

    public void closesearch()
    {
      using (CefProcessMessage message = CefProcessMessage.Create("CloseSearch"))
        CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, message);
    }

    public void refresh_search(string searchString = "")
    {
      using (CefProcessMessage message = CefProcessMessage.Create("RefreshSearch"))
      {
        message.Arguments.SetString(0, searchString);
        CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, message);
      }
    }

    public void offlinehtmlhomeurl(string url)
    {
      using (CefProcessMessage message = CefProcessMessage.Create("OfflineHtmlHomeUrl"))
      {
        message.Arguments.SetString(0, url);
        CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, message);
      }
    }

    public void refreshhomehtml()
    {
      Logger.Info("Got call from browser to refresh home html");
      using (CefProcessMessage message = CefProcessMessage.Create("RefreshHomeHtml"))
        CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, message);
    }

    public void setwebappversion(string version)
    {
      Logger.Info("Got call from browser to setWebAppVersion: " + version);
      using (CefProcessMessage message = CefProcessMessage.Create("SetWebAppVersion"))
      {
        message.Arguments.SetString(0, version);
        CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, message);
      }
    }

    public void unmuteappplayer()
    {
      Logger.Info("Got call from browser to unmute appPlayer");
      using (CefProcessMessage message = CefProcessMessage.Create("UnmuteAppPlayer"))
        CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, message);
    }

    public string getwebappversion()
    {
      return RegistryManager.Instance.WebAppVersion;
    }

    public void google_search(string searchString)
    {
      using (CefProcessMessage message = CefProcessMessage.Create("GoogleSearch"))
      {
        message.Arguments.SetString(0, searchString);
        CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, message);
      }
    }

    public string getusertoken()
    {
      return new JObject()
      {
        {
          "email",
          (JToken) RegistryManager.Instance.RegisteredEmail
        },
        {
          "token",
          (JToken) RegistryManager.Instance.Token
        }
      }.ToString(Formatting.None);
    }

    public void preinstallapp()
    {
    }

    public void openurl(string url)
    {
      Process.Start(url);
    }

    public string prod_ver()
    {
      return RegistryManager.Instance.Version;
    }

    public string getengineguid()
    {
      return RegistryManager.Instance.Version;
    }

    public string EngineVersion()
    {
      return RegistryManager.Instance.Version;
    }

    public string ClientVersion()
    {
      return RegistryManager.Instance.ClientVersion;
    }

    public string InstallID()
    {
      return RegistryManager.Instance.InstallID;
    }

    public string IsPremiumUser()
    {
      return RegistryManager.Instance.IsPremium.ToString((IFormatProvider) CultureInfo.InvariantCulture);
    }

    public string IsGoogleSigninDone(string vmName)
    {
      if (string.IsNullOrEmpty(vmName))
        vmName = "Android";
      return !RegistryManager.Instance.Guest.ContainsKey(vmName) ? RegistryManager.Instance.Guest["Android"].IsGoogleSigninDone.ToString((IFormatProvider) CultureInfo.InvariantCulture) : RegistryManager.Instance.Guest[vmName].IsGoogleSigninDone.ToString((IFormatProvider) CultureInfo.InvariantCulture);
    }

    public string IsOemAlreadyInstalled(string oem, string abi)
    {
      return InstalledOem.CheckIfOemInstancePresent(oem, abi).ToString((IFormatProvider) CultureInfo.InvariantCulture);
    }

    public string CampaignName()
    {
      try
      {
        string campaignJson = RegistryManager.Instance.CampaignJson;
        if (!string.IsNullOrEmpty(campaignJson))
        {
          JObject jobject = JObject.Parse(campaignJson);
          if (jobject["campaign_name"] != null)
            return jobject["campaign_name"].ToString();
        }
        return "";
      }
      catch (Exception ex)
      {
        Logger.Error("error while sending campaign name in gm api: " + ex.ToString());
        return "";
      }
    }

    public string CampaignJson()
    {
      return RegistryManager.Instance.CampaignJson;
    }

    public string get_oem()
    {
      return RegistryManager.Instance.Oem;
    }

    public string isAutomationEnabled()
    {
      return RegistryManager.Instance.EnableAutomation.ToString((IFormatProvider) CultureInfo.InvariantCulture);
    }

    public string bgp_uuid()
    {
      return RegistryManager.Instance.UserGuid;
    }

    public string BGPDevUrl()
    {
      return RegistryManager.Instance.BGPDevUrl;
    }

    public string DevCloudUrl()
    {
      return RegistryManager.Instance.Host;
    }

    public string GetMachineId()
    {
      return GuidUtils.GetBlueStacksMachineId();
    }

    public string GetVersionId()
    {
      return GuidUtils.GetBlueStacksVersionId();
    }

    public string SetFirebaseHost(string hostName)
    {
      lock (MyCustomCefV8Handler.sLock)
      {
        if (!string.IsNullOrEmpty(RegistryManager.Instance.CurrentFirebaseHost))
          return "false";
        RegistryManager.Instance.CurrentFirebaseHost = hostName;
        return "true";
      }
    }

    public void closeAnyPopup()
    {
      Logger.Info("Got call from browser of closeAnyPopup");
      using (CefProcessMessage message = CefProcessMessage.Create("CloseAnyPopup"))
        CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, message);
    }

    public void closeself()
    {
      Logger.Info("Got call from browser of closeself");
      using (CefProcessMessage message = CefProcessMessage.Create("CloseSelf"))
        CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, message);
    }

    public void closequitpopup()
    {
      Logger.Info("Got call from browser of closequitpopup");
      using (CefProcessMessage message = CefProcessMessage.Create("CloseBrowserQuitPopup"))
        CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, message);
    }

    public void downloadMacro(string macroData)
    {
      Logger.Info("Got call from browser of downloadmacro");
      using (CefProcessMessage message = CefProcessMessage.Create("DownloadMacro"))
      {
        message.Arguments.SetString(0, macroData);
        CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, message);
      }
    }

    public string getCurrentMacros()
    {
      Logger.Info("Got call from browser of getcurrentmacros");
      return string.Join("|", BlueStacksUIUtils.GetMacroList().ToArray());
    }

    public string uploadLocalMacro(string macroName)
    {
      Logger.Info("Got call from browser of uploadlocalmacro");
      return BlueStacksUIUtils.GetBase64MacroData(macroName);
    }

    public void performOTS()
    {
      Logger.Info("Got call from browser of performOTS");
      using (CefProcessMessage message = CefProcessMessage.Create("PerformOTS"))
      {
        message.Arguments.SetString(0, this.mCallBackFunction);
        CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, message);
      }
    }

    public void changeControlScheme(string schemeSelected)
    {
      Logger.Info("Got call from browser of changeControlScheme");
      using (CefProcessMessage message = CefProcessMessage.Create("ChangeControlScheme"))
      {
        message.Arguments.SetString(0, schemeSelected);
        CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, message);
      }
    }

    public void closeOnBoarding(string json)
    {
      Logger.Info("Got call from browser of closeOnBoarding");
      using (CefProcessMessage message = CefProcessMessage.Create("CloseOnBoarding"))
      {
        message.Arguments.SetString(0, json);
        CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, message);
      }
    }

    public void browserLoaded()
    {
      Logger.Info("Got call from browser of browserLoaded");
      using (CefProcessMessage message = CefProcessMessage.Create("BrowserLoaded"))
        CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, message);
    }

    public void getSystemInfo()
    {
      Logger.Info("Got call from browser of getSystemInfo");
      using (CefProcessMessage message = CefProcessMessage.Create("GetSystemInfo"))
      {
        message.Arguments.SetString(0, this.mCallBackFunction);
        CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, message);
      }
    }

    public void LogInfo(string info)
    {
      Logger.Info("HtmlLog: " + info);
    }

    public string GetSystemRAM()
    {
      return Profile.RAM;
    }

    public string GetLocale()
    {
      return RegistryManager.Instance.UserSelectedLocale;
    }

    public string GetSystemCPU()
    {
      return Profile.CPU;
    }

    public string GetSystemGPU()
    {
      return Profile.GPU;
    }

    public string GetSystemOS()
    {
      return Profile.OS;
    }

    public string GetCurrentSessionId()
    {
      Logger.Info("In GetCurrentSessionId");
      return Stats.GetSessionId();
    }

    public void showWebPage(string title, string webUrl)
    {
      using (CefProcessMessage message = CefProcessMessage.Create("ShowWebPage"))
      {
        CefListValue arguments = message.Arguments;
        arguments.SetString(0, title);
        arguments.SetString(1, webUrl);
        CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, message);
      }
    }

    public void HidePreview()
    {
      using (CefProcessMessage message = CefProcessMessage.Create(nameof (HidePreview)))
        CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, message);
    }

    public void ShowPreview()
    {
      using (CefProcessMessage message = CefProcessMessage.Create(nameof (ShowPreview)))
        CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, message);
    }

    public void StartObs(string callbackFunction)
    {
      using (CefProcessMessage message = CefProcessMessage.Create(nameof (StartObs)))
      {
        message.Arguments.SetString(0, callbackFunction);
        CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, message);
      }
    }

    public void StartStreamViewStatsRecorder(string label, string jsonString)
    {
      using (CefProcessMessage message = CefProcessMessage.Create(nameof (StartStreamViewStatsRecorder)))
      {
        CefListValue arguments = message.Arguments;
        arguments.SetString(0, label);
        arguments.SetString(1, jsonString);
        CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, message);
      }
    }

    public string GetStreamConfig()
    {
      Logger.Info("In GetStreamConfig");
      return StreamManager.GetStreamConfig();
    }

    public void LaunchDialog(string jsonString)
    {
      using (CefProcessMessage message = CefProcessMessage.Create(nameof (LaunchDialog)))
      {
        message.Arguments.SetString(0, jsonString);
        CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, message);
      }
    }

    public void StartStreamV2(
      string jsonString,
      string callbackStreamStatus,
      string callbackTabChanged)
    {
      Logger.Info("Got StartStreamV2");
      using (CefProcessMessage message = CefProcessMessage.Create(nameof (StartStreamV2)))
      {
        CefListValue arguments = message.Arguments;
        arguments.SetString(0, jsonString);
        arguments.SetString(1, callbackStreamStatus);
        arguments.SetString(2, callbackTabChanged);
        CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, message);
      }
    }

    public void StopStream()
    {
      Logger.Info("Got StopStream");
      using (CefProcessMessage message = CefProcessMessage.Create(nameof (StopStream)))
        CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, message);
    }

    public void StartRecord()
    {
      MyCustomCefV8Handler.StartRecordV2("{}");
    }

    public static void StartRecordV2(string jsonString)
    {
      Logger.Info("Got StartRecordV2");
      using (CefProcessMessage message = CefProcessMessage.Create(nameof (StartRecordV2)))
      {
        message.Arguments.SetString(0, jsonString);
        CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, message);
      }
    }

    public void StopRecord()
    {
      Logger.Info("Got StopStream");
      using (CefProcessMessage message = CefProcessMessage.Create(nameof (StopRecord)))
        CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, message);
    }

    public void SetSystemVolume(string level)
    {
      using (CefProcessMessage message = CefProcessMessage.Create(nameof (SetSystemVolume)))
      {
        message.Arguments.SetString(0, level);
        CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, message);
      }
    }

    public void SetMicVolume(string level)
    {
      using (CefProcessMessage message = CefProcessMessage.Create(nameof (SetMicVolume)))
      {
        message.Arguments.SetString(0, level);
        CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, message);
      }
    }

    public void EnableWebcam(string width, string height, string position)
    {
      using (CefProcessMessage message = CefProcessMessage.Create(nameof (EnableWebcam)))
      {
        Logger.Info("Got EnableWebcam");
        CefListValue arguments = message.Arguments;
        arguments.SetString(0, width);
        arguments.SetString(1, height);
        arguments.SetString(2, position);
        CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, message);
      }
    }

    public void DisableWebcamV2(string jsonString)
    {
      using (CefProcessMessage message = CefProcessMessage.Create(nameof (DisableWebcamV2)))
      {
        Logger.Info("Got DisableWebcamV2");
        message.Arguments.SetString(0, jsonString);
        CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, message);
      }
    }

    public void MoveWebcam(string horizontal, string vertical)
    {
      using (CefProcessMessage message = CefProcessMessage.Create(nameof (MoveWebcam)))
      {
        CefListValue arguments = message.Arguments;
        arguments.SetString(0, horizontal);
        arguments.SetString(1, vertical);
        CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, message);
      }
    }

    public void SetStreamName(string name)
    {
      Logger.Info("Got SetStreamName: " + name);
      RegistryManager.Instance.StreamName = name;
    }

    public void SetServerLocation(string location)
    {
      Logger.Info("Got SetServerLocation: " + location);
      RegistryManager.Instance.ServerLocation = location;
    }

    public void SetChannelName(string channelName)
    {
      RegistryManager.Instance.ChannelName = channelName;
    }

    public string GetRealtimeAppUsage()
    {
      using (CefProcessMessage message = CefProcessMessage.Create(nameof (GetRealtimeAppUsage)))
      {
        message.Arguments.SetString(0, this.mCallBackFunction);
        CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, message);
        return "";
      }
    }

    public string GetInstalledAppsJsonforJS()
    {
      using (CefProcessMessage message = CefProcessMessage.Create(nameof (GetInstalledAppsJsonforJS)))
      {
        message.Arguments.SetString(0, this.mCallBackFunction);
        CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, message);
        return "";
      }
    }

    public string GetInstalledAppsForAllOems()
    {
      using (CefProcessMessage message = CefProcessMessage.Create(nameof (GetInstalledAppsForAllOems)))
      {
        message.Arguments.SetString(0, this.mCallBackFunction);
        CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, message);
        return "";
      }
    }

    public string GetLoginAccountsForCurrentVm()
    {
      using (CefProcessMessage message = CefProcessMessage.Create(nameof (GetLoginAccountsForCurrentVm)))
      {
        message.Arguments.SetString(0, this.mCallBackFunction);
        CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, message);
        return "";
      }
    }

    public string GetCurrentAppInfo()
    {
      using (CefProcessMessage message = CefProcessMessage.Create(nameof (GetCurrentAppInfo)))
      {
        message.Arguments.SetString(0, this.mCallBackFunction);
        CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, message);
        return "";
      }
    }

    public string GetGMPort()
    {
      Logger.Info("In GetGMPort");
      return RegistryManager.Instance.PartnerServerPort.ToString((IFormatProvider) CultureInfo.InvariantCulture);
    }

    public string ResetSessionId()
    {
      Logger.Info("In ResetSessionId");
      return Stats.ResetSessionId();
    }

    public void makeWebCall(string url, string scriptToInvoke)
    {
      using (CefProcessMessage message = CefProcessMessage.Create(nameof (makeWebCall)))
      {
        CefListValue arguments = message.Arguments;
        arguments.SetString(0, url);
        arguments.SetString(1, scriptToInvoke);
        CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, message);
      }
    }

    public void ShowWebPageInBrowser(string url)
    {
      Logger.Info("Showing " + url + " in default browser");
      BlueStacksUIUtils.OpenUrl(url);
    }

    public void DialogClickHandler(string jsonString)
    {
      using (CefProcessMessage message = CefProcessMessage.Create(nameof (DialogClickHandler)))
      {
        message.Arguments.SetString(0, jsonString);
        CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, message);
      }
    }

    public void CloseDialog(string jsonString)
    {
      using (CefProcessMessage message = CefProcessMessage.Create(nameof (CloseDialog)))
      {
        message.Arguments.SetString(0, jsonString);
        CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, message);
      }
    }

    public void ShowAdvancedSettings()
    {
      using (CefProcessMessage message = CefProcessMessage.Create(nameof (ShowAdvancedSettings)))
        CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, message);
    }

    public void LaunchFilterWindow(string channel, string sessionId)
    {
      using (CefProcessMessage message = CefProcessMessage.Create(nameof (LaunchFilterWindow)))
      {
        CefListValue arguments = message.Arguments;
        arguments.SetString(0, channel);
        arguments.SetString(1, sessionId);
        CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, message);
      }
    }

    public void ChangeFilterTheme(string theme)
    {
      using (CefProcessMessage message = CefProcessMessage.Create(nameof (ChangeFilterTheme)))
      {
        message.Arguments.SetString(0, theme);
        CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, message);
      }
    }

    public void UpdateThemeSettings(string currentTheme, string settingsJson)
    {
      using (CefProcessMessage message = CefProcessMessage.Create(nameof (UpdateThemeSettings)))
      {
        CefListValue arguments = message.Arguments;
        arguments.SetString(0, currentTheme);
        arguments.SetString(1, settingsJson);
        CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, message);
      }
    }

    public void CloseFilterWindow(string jsonArray)
    {
      using (CefProcessMessage message = CefProcessMessage.Create(nameof (CloseFilterWindow)))
      {
        message.Arguments.SetString(0, jsonArray);
        CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, message);
      }
    }

    public string IsFacebook()
    {
      return string.Equals(RegistryManager.Instance.BtvNetwork, "facebook", StringComparison.InvariantCulture) ? "true" : "false";
    }

    public void launchinstance(
      string vmname,
      string packageName,
      string campaignId,
      string launchMode)
    {
      Logger.Info("Get Call from browser of launchfarminstance vmname:" + vmname + ", campaignId: " + campaignId);
      using (CefProcessMessage message = CefProcessMessage.Create("LaunchInstance"))
      {
        CefListValue arguments = message.Arguments;
        arguments.SetString(0, vmname);
        arguments.SetString(1, campaignId);
        arguments.SetString(2, launchMode);
        arguments.SetString(3, packageName);
        CefV8Context.GetCurrentContext().GetBrowser().SendProcessMessage(CefProcessId.Browser, message);
      }
    }
  }
}
