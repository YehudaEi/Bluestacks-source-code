// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.AppHandler
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
using System.Text;
using System.Threading;
using System.Windows.Controls;
using System.Xml;
using System.Xml.Serialization;

namespace BlueStacks.BlueStacksUI
{
  public class AppHandler
  {
    private object mOtsCheckLock = new object();
    private int oneSecond = 1000;
    internal string mDefaultLauncher = "com.bluestacks.appmart";
    private object sLockObject = new object();
    private object sOTSLock = new object();
    private string mSwitchWhenPackageNameRecieved = string.Empty;
    private MainWindow ParentWindow;
    private Thread mOtsCheckThread;
    private bool mIsOneTimeSetupCompleted;
    private bool mIsGuestReady;
    internal bool mGuestReadyCheckStarted;

    public SerializableDictionary<string, DateTime> CdnAppdict { get; set; } = new SerializableDictionary<string, DateTime>();

    public string mLastAppDisplayed { get; set; } = string.Empty;

    public string mLastRunAppSentForSynced { get; set; } = string.Empty;

    public string mAppDisplayedOccured { get; set; } = string.Empty;

    internal string CurrentActivityName { get; set; } = string.Empty;

    public static List<string> ListIgnoredApps { get; } = new List<string>()
    {
      "tv.gamepop.home",
      "com.pop.store",
      "com.pop.store51",
      "com.bluestacks.s2p5105",
      "com.bluestacks.help",
      "mpi.v23",
      "com.google.android.gms",
      "com.google.android.gsf.login",
      "com.android.deskclock",
      "me.onemobile.android",
      "me.onemobile.lite.android",
      "android.rk.RockVideoPlayer.RockVideoPlayer",
      "com.bluestacks.chartapp",
      "com.bluestacks.setupapp",
      "com.android.gallery3d",
      "com.bluestacks.keymappingtool",
      "com.baidu.appsearch",
      "com.bluestacks.s2p",
      "com.bluestacks.windowsfilemanager",
      "com.android.quicksearchbox",
      "com.bluestacks.setup",
      "com.bluestacks.appsettings",
      "mpi.v23",
      "com.bluestacks.setup",
      "com.bluestacks.gamepophome",
      "com.bluestacks.appfinder",
      "com.android.providers.downloads.ui",
      "com.google.android.instantapps.supervisor"
    };

    public bool IsOneTimeSetupCompleted
    {
      get
      {
        if (RegistryManager.Instance.InstallationType == InstallationTypes.GamingEdition && GameConfig.Instance.AppGenericAction == GenericAction.InstallCDN)
          return true;
        if (!this.mIsOneTimeSetupCompleted)
          this.StartOTSCheckThread();
        return this.mIsOneTimeSetupCompleted;
      }
      set
      {
        this.mIsOneTimeSetupCompleted = value;
        this.ParentWindow.EngineInstanceRegistry.IsOneTimeSetupDone = value;
        Logger.Info("One time setup completed. Will perform tasks now");
        lock (this.sOTSLock)
        {
          Logger.Info("Performing OTS completed tasks");
          if (!value || this.EventOnOneTimeSetupCompleted == null)
            return;
          this.EventOnOneTimeSetupCompleted((object) this.ParentWindow, new EventArgs());
          this.EventOnOneTimeSetupCompleted = (EventHandler<EventArgs>) null;
        }
      }
    }

    private void StartOTSCheckThread()
    {
      if (this.mOtsCheckThread != null)
        return;
      lock (this.mOtsCheckLock)
      {
        if (this.mOtsCheckThread != null)
          return;
        try
        {
          this.mOtsCheckThread = new Thread((ThreadStart) (() =>
          {
            Logger.Info("Checking for if OTS completed");
            while (!this.mIsOneTimeSetupCompleted)
            {
              this.CheckingOneTimeSetupCompleted();
              Thread.Sleep(2 * this.oneSecond);
            }
          }))
          {
            IsBackground = true
          };
          this.mOtsCheckThread.Start();
        }
        catch (Exception ex)
        {
          Logger.Error("Failed to create ots check thread.");
          Logger.Error(ex.ToString());
        }
      }
    }

    public bool IsGuestReady
    {
      get
      {
        return this.mIsGuestReady;
      }
      set
      {
        this.mIsGuestReady = value;
        if (!this.mIsGuestReady)
          return;
        this.SignalGuestReady();
      }
    }

    private void SignalGuestReady()
    {
      if (!FeatureManager.Instance.IsCustomUIForNCSoft)
      {
        Logger.Info("Boot install: Signal Guest Ready");
        this.ParentWindow.GuestBoot_Completed();
      }
      else
      {
        this.ParentWindow.Utils.sBootCheckTimer.Enabled = false;
        this.ParentWindow.mEnableLaunchPlayForNCSoft = true;
      }
    }

    private void CheckingOneTimeSetupCompleted()
    {
      try
      {
        string str = JObject.Parse(HTTPUtils.SendRequestToGuest("isOTSCompleted", (Dictionary<string, string>) null, this.ParentWindow.mVmName, 1000, (Dictionary<string, string>) null, false, 1, 0, "bgp"))["result"].ToString();
        if (!str.Equals("ok", StringComparison.InvariantCultureIgnoreCase))
          return;
        Logger.Info("OTS result: {0}", (object) str);
        this.IsOneTimeSetupCompleted = true;
      }
      catch (Exception ex)
      {
        Logger.Warning("Exception in checking OneTimeSetupCompleted with vmName {0}. Err: {1}", (object) this.ParentWindow.mVmName, (object) ex.Message);
      }
    }

    public string SwitchWhenPackageNameRecieved
    {
      get
      {
        return this.mSwitchWhenPackageNameRecieved;
      }
      set
      {
        this.mSwitchWhenPackageNameRecieved = value;
        if (string.IsNullOrEmpty(this.mSwitchWhenPackageNameRecieved) || !this.mSwitchWhenPackageNameRecieved.Equals(this.mLastAppDisplayed, StringComparison.InvariantCultureIgnoreCase))
          return;
        this.AppLaunched(this.mSwitchWhenPackageNameRecieved, "", true);
      }
    }

    public EventHandler<EventArgs> EventOnOneTimeSetupCompleted { get; set; }

    public static EventHandler<EventArgs> EventOnAppDisplayed { get; set; }

    internal AppHandler(MainWindow window)
    {
      this.ParentWindow = window;
      string cdnAppsTimeStamp = RegistryManager.Instance.CDNAppsTimeStamp;
      if (!string.IsNullOrEmpty(cdnAppsTimeStamp))
      {
        using (XmlReader xmlReader = XmlReader.Create((TextReader) new StringReader(cdnAppsTimeStamp)))
          this.CdnAppdict = (SerializableDictionary<string, DateTime>) new XmlSerializer(typeof (SerializableDictionary<string, DateTime>)).Deserialize(xmlReader);
      }
      this.mIsOneTimeSetupCompleted = this.ParentWindow.EngineInstanceRegistry.IsOneTimeSetupDone;
    }

    public bool IsAppInstalled(string package)
    {
      bool flag = false;
      if (new JsonParser(this.ParentWindow.mVmName).IsAppInstalled(package, out string _))
        flag = true;
      return flag;
    }

    public bool IsAppInstalled(string package, out string version)
    {
      bool flag = false;
      if (new JsonParser(this.ParentWindow.mVmName).IsAppInstalled(package, out version))
        flag = true;
      return flag;
    }

    public void AppLaunched(string packageName, string currentActivity = "", bool forced = false)
    {
      lock (this.sLockObject)
      {
        if (this.ParentWindow.IsClosed)
          return;
        this.CurrentActivityName = currentActivity;
        if ((packageName == BlueStacksUIUtils.sUserAccountPackageName || packageName == "com.android.vending") && this.mSwitchWhenPackageNameRecieved == "com.android.vending")
        {
          packageName = this.mSwitchWhenPackageNameRecieved;
          if (string.Compare(this.mLastRunAppSentForSynced, packageName, StringComparison.OrdinalIgnoreCase) == 0)
            this.mSwitchWhenPackageNameRecieved = "";
        }
        if (!forced && string.Equals(packageName, this.mLastAppDisplayed, StringComparison.InvariantCultureIgnoreCase))
          return;
        if (!this.mIsOneTimeSetupCompleted)
        {
          if (string.IsNullOrEmpty(packageName) || !packageName.StartsWith("com.google.android.gms", StringComparison.InvariantCultureIgnoreCase) && !packageName.Equals("com.google.android.setupwizard", StringComparison.InvariantCultureIgnoreCase))
            return;
          this.StartOTSCheckThread();
        }
        else
        {
          Logger.Info("SwitchWhenPackageNameRecieved: {0}", (object) this.mSwitchWhenPackageNameRecieved);
          this.ParentWindow.ShowLoadingGrid(false);
          bool receivedFromImap = string.Compare(this.mLastRunAppSentForSynced, packageName, StringComparison.OrdinalIgnoreCase) == 0;
          if (receivedFromImap)
            this.mLastRunAppSentForSynced = "";
          if (!string.IsNullOrEmpty(this.mSwitchWhenPackageNameRecieved) && string.Equals(packageName, this.mSwitchWhenPackageNameRecieved, StringComparison.OrdinalIgnoreCase))
          {
            this.mSwitchWhenPackageNameRecieved = string.Empty;
            if (AppHandler.EventOnAppDisplayed == null)
            {
              this.ParentWindow.Dispatcher.Invoke((Delegate) (() =>
              {
                this.ParentWindow.mTopBar.mAppTabButtons.GoToTab(packageName, receivedFromImap, false);
                Publisher.PublishMessage(BrowserControlTags.tabSwitched, this.ParentWindow.mVmName, new JObject((object) new JProperty("PackageName", (object) packageName)));
              }));
            }
            else
            {
              EventHandler<EventArgs> eventOnAppDisplayed = AppHandler.EventOnAppDisplayed;
              AppHandler.EventOnAppDisplayed = (EventHandler<EventArgs>) null;
              MainWindow parentWindow = this.ParentWindow;
              EventArgs e = new EventArgs();
              eventOnAppDisplayed((object) parentWindow, e);
            }
          }
          else if (this.mDefaultLauncher.Equals(packageName, StringComparison.InvariantCultureIgnoreCase))
          {
            if (!Oem.IsOEMDmm)
            {
              Logger.Info("Assuming app is crashed/exited going to last tab");
              this.ParentWindow.Dispatcher.Invoke((Delegate) (() =>
              {
                if (this.ParentWindow.mFrontendGrid == null)
                  return;
                if (this.ParentWindow.mFrontendGrid.Parent as Grid == this.ParentWindow.FrontendParentGrid)
                {
                  if (this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab != null && this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.mTabType == TabType.AppTab)
                    this.ParentWindow.mTopBar.mAppTabButtons.CloseTab(this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.TabKey, false, false, true, false, packageName);
                  if (RegistryManager.Instance.InstallationType != InstallationTypes.GamingEdition)
                    return;
                  this.PerformGamingAction("", "");
                }
                else
                  this.ParentWindow.mWelcomeTab.mFrontendPopupControl.HideWindow();
              }));
            }
          }
          else
          {
            AppIconModel icon = this.ParentWindow.mWelcomeTab.mHomeAppManager.GetAppIcon(packageName);
            if (icon != null)
              this.ParentWindow.Dispatcher.Invoke((Delegate) (() => this.ParentWindow.mTopBar.mAppTabButtons.AddAppTab(icon.AppName, icon.PackageName, icon.ActivityName, icon.ImageName, true, false, receivedFromImap)));
          }
          this.mLastAppDisplayed = packageName;
        }
      }
    }

    public void HandleAppDisplayed(string packageName)
    {
      if (!string.Equals(packageName, this.mDefaultLauncher, StringComparison.InvariantCultureIgnoreCase))
        return;
      Logger.Info("Home app is displayed...closing tab");
      this.ParentWindow.Dispatcher.Invoke((Delegate) (() =>
      {
        if (this.ParentWindow.mFrontendGrid == null || this.ParentWindow.mFrontendGrid.Parent as Grid != this.ParentWindow.FrontendParentGrid || (this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab == null || this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.mTabType != TabType.AppTab))
          return;
        this.ParentWindow.mTopBar.mAppTabButtons.CloseTab(this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.TabKey, false, false, true, false, "");
      }));
    }

    internal void GoHome()
    {
      ThreadPool.QueueUserWorkItem((WaitCallback) (obj => VmCmdHandler.RunCommand("home", this.ParentWindow.mVmName, "bgp")));
    }

    public string GetDefaultLauncher()
    {
      string str1 = "com.bluestacks.appmart";
      try
      {
        string guest = HTTPUtils.SendRequestToGuest("getDefaultLauncher", (Dictionary<string, string>) null, this.ParentWindow.mVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
        Logger.Info("GetDefaultLauncher response = " + guest);
        JObject jobject = JObject.Parse(guest);
        string str2 = jobject["result"].ToString().Trim();
        if (str2 == "ok")
          str1 = jobject["defaultLauncher"].ToString().Trim();
        else if (str2 == "error")
        {
          if (jobject["reason"].ToString().Trim() == "no default launcher")
            str1 = "none";
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in GetDefauntLauncher. Err." + ex.ToString());
      }
      return str1;
    }

    internal void StartCustomActivity(Dictionary<string, string> data)
    {
      ThreadPool.QueueUserWorkItem((WaitCallback) (obj =>
      {
        try
        {
          Logger.Info("Starting a custom activity");
          foreach (KeyValuePair<string, string> keyValuePair in data)
            Logger.Debug("Data = {0} , {1}", (object) keyValuePair.Key, (object) keyValuePair.Value);
          HTTPUtils.SendRequestToGuest("customStartActivity", data, this.ParentWindow.mVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
        }
        catch (Exception ex)
        {
          Logger.Error("Exception in launching custom activity. Err: " + ex.Message);
        }
      }));
    }

    internal void SetDefaultLauncher(string launcherName)
    {
      ThreadPool.QueueUserWorkItem((WaitCallback) (obj =>
      {
        try
        {
          Logger.Info("Setlauncher res: {0}", (object) HTTPUtils.SendRequestToGuest("setDefaultLauncher", new Dictionary<string, string>()
          {
            {
              "d",
              launcherName
            }
          }, this.ParentWindow.mVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp"));
          Logger.Info("the response for home command is {0}", (object) HTTPUtils.SendRequestToGuest("home", new Dictionary<string, string>()
          {
            {
              "arg",
              ""
            }
          }, this.ParentWindow.mVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp"));
        }
        catch (Exception ex)
        {
          Logger.Error("Exception in SetDefaultLauncher. Err:{0}", (object) ex.ToString());
        }
      }));
    }

    internal void AppUninstalled(string package)
    {
      this.ParentWindow.mWelcomeTab.mHomeAppManager.RemoveAppAfterUninstall(package);
      this.ParentWindow.mTopBar.mAppTabButtons.CloseTab(package, false, false, true, false, "");
      if (!AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName].ContainsKey(package))
        return;
      if (AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName][package].IsForcedLandscapeEnabled)
      {
        Utils.SetCustomAppSize(this.ParentWindow.mVmName, package, ScreenMode.original);
        KMManager.SelectSchemeIfPresent(this.ParentWindow, "Portrait", "appuninstalled", false);
        AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName][package].IsForcedLandscapeEnabled = false;
      }
      if (!AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName][package].IsForcedPortraitEnabled)
        return;
      Utils.SetCustomAppSize(this.ParentWindow.mVmName, package, ScreenMode.original);
      KMManager.SelectSchemeIfPresent(this.ParentWindow, "Landscape", "appuninstalled", false);
      AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName][package].IsForcedPortraitEnabled = false;
    }

    internal void AppInstalled(string package, bool isUpdate)
    {
      AppInfo appInfo = this.ParentWindow.mWelcomeTab.mHomeAppManager.AddAppIcon(package);
      Publisher.PublishMessage(BrowserControlTags.appInstalled, this.ParentWindow.mVmName, new JObject()
      {
        ["PackageName"] = (JToken) package,
        ["AppName"] = (JToken) appInfo?.Name,
        ["IsGamepadCompatible"] = (JToken) appInfo?.IsGamepadCompatible
      });
      if (FeatureManager.Instance.IsShowAppRecommendations || !RegistryManager.Instance.IsPremium)
        this.ParentWindow.mWelcomeTab.mHomeAppManager.UpdateRecommendedAppsInstallStatus(package);
      GrmHandler.RefreshGrmIndication(package, this.ParentWindow.mVmName);
      GrmHandler.SendUpdateGrmPackagesToAndroid(this.ParentWindow.mVmName);
      GrmHandler.SendUpdateGrmPackagesToBrowser(this.ParentWindow.mVmName);
      GuidanceCloudInfoManager.Instance.AppsGuidanceCloudInfoRefresh();
      NotificationManager.Instance.SetDefaultState(package, this.ParentWindow.mVmName);
      if (RegistryManager.Instance.FirstAppLaunchState == AppLaunchState.Fresh)
        RegistryManager.Instance.FirstAppLaunchState = AppLaunchState.Installed;
      if (!AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName].ContainsKey(package))
        AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName][package] = new BlueStacks.Common.AppSettings();
      if (!isUpdate)
      {
        AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName][package].AppInstallTime = DateTime.Now.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName][package].IsAppOnboardingCompleted = false;
        AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName][package].IsGeneralAppOnBoardingCompleted = false;
      }
      string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, Path.Combine(RegistryStrings.InputMapperFolder, package) + ".cfg");
      if (!File.Exists(str))
        return;
      string md5HashFromFile = Utils.GetMD5HashFromFile(str);
      AppConfigurationManager.Instance.VmAppConfig[this.ParentWindow.mVmName][package].CfgStored = md5HashFromFile;
    }

    internal void UpdateDefaultLauncher()
    {
      ThreadPool.QueueUserWorkItem((WaitCallback) (obj =>
      {
        string launcherName = this.GetDefaultLauncher();
        Logger.Info("DefaultLauncher " + launcherName);
        if (launcherName.Equals("none", StringComparison.InvariantCultureIgnoreCase))
        {
          launcherName = "com.bluestacks.appmart";
          this.SetDefaultLauncher(launcherName);
        }
        if (launcherName.Equals("com.android.provision", StringComparison.InvariantCultureIgnoreCase))
          launcherName = "com.bluestacks.appmart";
        this.mDefaultLauncher = launcherName;
      }));
    }

    internal void SendSearchPlayRequestAsync(string searchQuery)
    {
      ThreadPool.QueueUserWorkItem((WaitCallback) (obj =>
      {
        if (searchQuery.Contains("search::"))
          searchQuery = searchQuery.Remove(0, 8);
        VmCmdHandler.RunCommand(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "searchplay {0}", (object) searchQuery), this.ParentWindow.mVmName, "bgp");
      }));
    }

    internal void LaunchPlayRequestAsync(string packageName)
    {
      ThreadPool.QueueUserWorkItem((WaitCallback) (obj => VmCmdHandler.RunCommand(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "launchplay?pkgname={0}", (object) packageName), this.ParentWindow.mVmName, "bgp")));
    }

    public void SendRunAppRequestAsync(string package, string activity = "", bool receivedFromImap = false)
    {
      ThreadPool.QueueUserWorkItem((WaitCallback) (obj =>
      {
        if (this.ParentWindow.SendClientActions && !receivedFromImap)
        {
          Dictionary<string, string> data = new Dictionary<string, string>();
          Dictionary<string, string> dictionary = new Dictionary<string, string>()
          {
            {
              "EventAction",
              "RunApp"
            },
            {
              "Package",
              package
            },
            {
              "Activity",
              activity
            }
          };
          JsonSerializerSettings serializerSettings = Utils.GetSerializerSettings();
          serializerSettings.Formatting = Newtonsoft.Json.Formatting.None;
          data.Add("operationData", JsonConvert.SerializeObject((object) dictionary, serializerSettings));
          this.ParentWindow.mFrontendHandler.SendFrontendRequestAsync("handleClientOperation", data);
        }
        if (receivedFromImap)
        {
          this.mLastRunAppSentForSynced = package;
          if (package == "com.android.vending")
            this.mSwitchWhenPackageNameRecieved = package;
        }
        if (string.IsNullOrEmpty(activity))
        {
          AppIconModel appIcon = this.ParentWindow.mWelcomeTab.mHomeAppManager.GetAppIcon(package);
          if (appIcon != null)
            activity = appIcon.ActivityName;
          if (string.IsNullOrEmpty(activity))
          {
            activity = ".Main";
            Logger.Info("Empty activity name ovveriding .Main for package: " + package);
          }
        }
        if (PackageActivityNames.ThirdParty.AllPUBGPackageNames.Contains(package))
        {
          string displayQualityPubg = RegistryManager.Instance.Guest[this.ParentWindow.mVmName].DisplayQualityPubg;
          string gamingResolutionPubg = RegistryManager.Instance.Guest[this.ParentWindow.mVmName].GamingResolutionPubg;
          if (string.Equals(displayQualityPubg, "-1", StringComparison.InvariantCulture) && string.Equals(gamingResolutionPubg, "1", StringComparison.InvariantCulture))
          {
            this.SendRunex(package, activity);
          }
          else
          {
            StringBuilder sb = new StringBuilder();
            using (JsonWriter jsonWriter = (JsonWriter) new JsonTextWriter((TextWriter) new StringWriter(sb)))
            {
              jsonWriter.WriteStartObject();
              if (string.Equals(RegistryManager.Instance.Guest[this.ParentWindow.mVmName].DisplayQualityPubg, "-1", StringComparison.InvariantCulture))
              {
                jsonWriter.WritePropertyName("renderqualitylevel");
                jsonWriter.WriteValue("0");
              }
              else
              {
                jsonWriter.WritePropertyName("renderqualitylevel");
                jsonWriter.WriteValue(RegistryManager.Instance.Guest[this.ParentWindow.mVmName].DisplayQualityPubg);
              }
              jsonWriter.WritePropertyName("contentscale");
              jsonWriter.WriteValue(RegistryManager.Instance.Guest[this.ParentWindow.mVmName].GamingResolutionPubg);
              jsonWriter.WriteEndObject();
            }
            Logger.Info("The response we get is: " + HTTPUtils.SendRequestToGuest("customStartActivity", new Dictionary<string, string>()
            {
              {
                "component",
                package + "/" + activity
              },
              {
                "extras",
                sb.ToString()
              }
            }, this.ParentWindow.mVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp"));
          }
        }
        else if (PackageActivityNames.ThirdParty.AllCallOfDutyPackageNames.Contains(package))
        {
          int num1 = int.Parse(RegistryManager.Instance.Guest[this.ParentWindow.mVmName].DisplayQualityCOD, (IFormatProvider) CultureInfo.InvariantCulture);
          int num2 = int.Parse(RegistryManager.Instance.Guest[this.ParentWindow.mVmName].GamingResolutionCOD, (IFormatProvider) CultureInfo.InvariantCulture);
          int num3 = int.Parse("1", (IFormatProvider) CultureInfo.InvariantCulture);
          StringBuilder sb = new StringBuilder();
          using (JsonWriter jsonWriter = (JsonWriter) new JsonTextWriter((TextWriter) new StringWriter(sb)))
          {
            jsonWriter.WriteStartObject();
            if (string.Equals(num1.ToString((IFormatProvider) CultureInfo.InvariantCulture), "-1", StringComparison.InvariantCulture))
            {
              jsonWriter.WritePropertyName("QualityLevel");
              jsonWriter.WriteValue(int.Parse("0", (IFormatProvider) CultureInfo.InvariantCulture));
            }
            else
            {
              jsonWriter.WritePropertyName("QualityLevel");
              jsonWriter.WriteValue(num1);
            }
            jsonWriter.WritePropertyName("ResolutionHeight");
            jsonWriter.WriteValue(num2);
            jsonWriter.WritePropertyName("FrameRateLevel");
            jsonWriter.WriteValue(num3);
            jsonWriter.WriteEndObject();
          }
          Logger.Info("The response we get is: " + HTTPUtils.SendRequestToGuest("customStartActivity", new Dictionary<string, string>()
          {
            {
              "component",
              package + "/" + activity
            },
            {
              "extras",
              sb.ToString()
            }
          }, this.ParentWindow.mVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp"));
        }
        else if ("com.android.chrome".Equals(package, StringComparison.InvariantCultureIgnoreCase))
          HTTPUtils.SendRequestToGuest("launchchrome", (Dictionary<string, string>) null, this.ParentWindow.mVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
        else
          this.SendRunex(package, activity);
      }));
    }

    internal void SendRunex(string package, string activity)
    {
      VmCmdHandler.RunCommand(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "runex {0}/{1}", (object) package, (object) activity), this.ParentWindow.mVmName, "bgp");
    }

    internal void StopAppRequest(string packageName)
    {
      ThreadPool.QueueUserWorkItem((WaitCallback) (obj =>
      {
        try
        {
          Logger.Info("Will send stop {0} request", (object) packageName);
          Logger.Info("the response we get is {0}", (object) this.ParentWindow.mFrontendHandler.SendFrontendRequest("stopAppInfo", new Dictionary<string, string>()
          {
            {
              "appPackage",
              packageName
            }
          }));
          Logger.Info(VmCmdHandler.RunCommand(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "StopApp {0}", (object) packageName), this.ParentWindow.mVmName, "bgp"));
        }
        catch (Exception ex)
        {
          Logger.Error("Exception in StopAppRequest. Err : " + ex.ToString());
        }
      }));
    }

    internal void SendRequestToRemoveAccountAndCloseWindowASync(bool closeWindow = false)
    {
      ThreadPool.QueueUserWorkItem((WaitCallback) (obj =>
      {
        try
        {
          Logger.Info("Account removed response: " + HTTPUtils.SendRequestToGuest("removeAccountsInfo", (Dictionary<string, string>) null, this.ParentWindow.mVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp"));
        }
        catch (Exception ex)
        {
          Logger.Error("Exception in removing account, Ex: " + ex.Message);
        }
        if (!closeWindow)
          return;
        this.ParentWindow.Dispatcher.Invoke((Delegate) (() => this.ParentWindow.ForceCloseWindow(false)));
      }));
    }

    internal void WriteXMl(bool isAppInstall, string packageName, DateTime timestamp)
    {
      if (isAppInstall)
      {
        this.CdnAppdict[packageName] = timestamp;
        using (StringWriter stringWriter = new StringWriter())
        {
          new XmlSerializer(typeof (SerializableDictionary<string, DateTime>)).Serialize((TextWriter) stringWriter, (object) this.CdnAppdict);
          RegistryManager.Instance.CDNAppsTimeStamp = stringWriter.ToString();
        }
      }
      else
      {
        if (!this.CdnAppdict.ContainsKey(packageName))
          return;
        this.CdnAppdict.Remove(packageName);
        using (StringWriter stringWriter = new StringWriter())
        {
          new XmlSerializer(typeof (SerializableDictionary<string, DateTime>)).Serialize((TextWriter) stringWriter, (object) this.CdnAppdict);
          RegistryManager.Instance.CDNAppsTimeStamp = stringWriter.ToString();
        }
      }
    }

    internal void PerformGamingAction(string pkgName = "", string activityName = "")
    {
      GenericAction action;
      if (string.IsNullOrEmpty(pkgName))
      {
        pkgName = GameConfig.Instance.PkgName;
        activityName = GameConfig.Instance.ActivityName;
        action = GameConfig.Instance.AppGenericAction;
      }
      else
        action = GenericAction.InstallPlay;
      this.ParentWindow.Dispatcher.Invoke((Delegate) (() =>
      {
        if (this.IsAppInstalled(pkgName))
        {
          this.SendRunAppRequestAsync(pkgName, "", false);
        }
        else
        {
          if (action != GenericAction.InstallPlay)
            return;
          this.LaunchPlayRequestAsync(pkgName);
        }
      }));
    }
  }
}
