// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.HomeAppManager
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

namespace BlueStacks.BlueStacksUI
{
  public class HomeAppManager
  {
    internal static string BackgroundImagePath = Path.Combine(RegistryManager.Instance.UserDefinedDir, "Client\\Assets\\backgroundImage");
    private Dictionary<string, AppIconModel> dictAppIcons = new Dictionary<string, AppIconModel>();
    private HomeApp mHomeApp;
    private MainWindow mParentWindow;

    public HomeAppManager(HomeApp homeApp, MainWindow parentWindow)
    {
      this.mHomeApp = homeApp;
      this.mParentWindow = parentWindow;
      this.InitSystemIcons();
      this.InitIcons();
    }

    internal void InitAppPromotionEvents()
    {
      if (PromotionObject.Instance == null)
        return;
      PromotionObject.AppSuggestionHandler += new System.Action<bool>(this.HomeApp_AppSuggestionHandler);
      this.mHomeApp?.InitUIAppPromotionEvents();
    }

    private void HomeApp_AppSuggestionHandler(bool checkForAnimationIcon)
    {
      this.mParentWindow?.Dispatcher.Invoke((Delegate) (() =>
      {
        try
        {
          this.RemoveIconIfExists();
          lock (((ICollection) PromotionObject.Instance.AppSuggestionList).SyncRoot)
          {
            foreach (AppSuggestionPromotion appSuggestion in PromotionObject.Instance.AppSuggestionList)
            {
              if (!new JsonParser(this.mParentWindow.mVmName).IsAppInstalled(appSuggestion.AppPackage))
              {
                if (HomeAppManager.CheckIfPresentInRedDotShownRegistry(appSuggestion.AppPackage))
                  appSuggestion.IsShowRedDot = false;
                this.AddAppSuggestionIcon(appSuggestion);
              }
              else
              {
                if (!HomeAppManager.CheckIfPresentInRedDotShownRegistry(appSuggestion.AppPackage) && appSuggestion.IsShowRedDot)
                  this.GetAppIcon(appSuggestion.AppPackage)?.AddRedDot();
                else
                  appSuggestion.IsShowRedDot = false;
                this.GetAppIcon(appSuggestion.AppPackage)?.AddPromotionBorderInstalledApp(appSuggestion);
              }
            }
          }
          bool enable = this.dictAppIcons.Keys.Intersect<string>((IEnumerable<string>) PackageActivityNames.ThirdParty.AllOneStorePackageNames).Any<string>();
          foreach (string storePackageName in PackageActivityNames.ThirdParty.AllOneStorePackageNames)
            Utils.EnableDisableApp(storePackageName, enable, this.mParentWindow.mVmName);
          this.mParentWindow.StaticComponents.PlayPauseGifs(true);
        }
        catch (Exception ex)
        {
          Logger.Error("Error in HomeApp_AppSuggestionHandler", (object) ex);
        }
      }));
    }

    private void RemoveIconIfExists()
    {
      List<string> stringList = new List<string>();
      JsonParser jsonParser = new JsonParser(this.mParentWindow.mVmName);
      foreach (AppIconModel appIconModel in this.dictAppIcons.Values)
      {
        AppIconModel icon = appIconModel;
        lock (((ICollection) PromotionObject.Instance.AppSuggestionList).SyncRoot)
        {
          if (!icon.IsAppSuggestionActive)
          {
            if (!icon.IsInstalledApp)
            {
              if (!PromotionObject.Instance.AppSuggestionList.Any<AppSuggestionPromotion>((Func<AppSuggestionPromotion, bool>) (_ => string.Equals(_.AppLocation, "more_apps", StringComparison.InvariantCulture))))
                continue;
            }
            else
              continue;
          }
          if (!PromotionObject.Instance.AppSuggestionList.Any<AppSuggestionPromotion>((Func<AppSuggestionPromotion, bool>) (_ => string.Equals(_.AppPackage, icon.PackageName, StringComparison.InvariantCultureIgnoreCase))))
          {
            if (!jsonParser.IsAppInstalled(icon.PackageName))
              stringList.Add(icon.PackageName);
            else
              icon.RemovePromotionBorderInstalledApp();
          }
        }
      }
      foreach (string package in stringList)
        this.RemoveAppIcon(package, (AppIconModel) null);
    }

    private void InitSystemIcons()
    {
      List<AppInfo> list = ((IEnumerable<AppInfo>) new JsonParser(string.Empty).GetAppList()).ToList<AppInfo>();
      this.mHomeApp?.InitMoreAppsIcon();
      foreach (AppInfo appInfo in list)
      {
        if (string.Compare(appInfo.Package, "com.android.vending", StringComparison.OrdinalIgnoreCase) != 0 && string.Compare(appInfo.Package, "com.google.android.play.games", StringComparison.OrdinalIgnoreCase) != 0)
        {
          AppIconModel newIconForKey = this.GetNewIconForKey(appInfo.Package);
          newIconForKey.Init(appInfo);
          newIconForKey.AppName = "STRING_" + appInfo.Name.ToUpper(CultureInfo.InvariantCulture).Trim().Replace(" ", "_") + "_APP";
          newIconForKey.IsInstalledApp = false;
          newIconForKey.AddToMoreAppsDock(55.0, 55.0);
          this.mHomeApp?.AddMoreAppsDockPanelIcon(newIconForKey, (DownloadInstallApk) null);
        }
        else
        {
          AppIconModel newIconForKey = this.GetNewIconForKey(appInfo.Package);
          newIconForKey.Init(appInfo);
          newIconForKey.IsInstalledApp = false;
          newIconForKey.mIsAppRemovable = false;
          newIconForKey.AppName = "STRING_" + appInfo.Name.ToUpper(CultureInfo.InvariantCulture).Trim().Replace(" ", "_") + "_APP";
          newIconForKey.AddToInstallDrawer();
          this.mHomeApp?.AddInstallDrawerIcon(newIconForKey, (DownloadInstallApk) null);
        }
      }
    }

    internal void InitIcons()
    {
      foreach (AppInfo appInfo in ((IEnumerable<AppInfo>) new JsonParser(this.mParentWindow.mVmName).GetAppList()).ToList<AppInfo>())
        this.AddIcon(appInfo);
    }

    internal AppInfo AddAppIcon(string package)
    {
      AppInfo infoFromPackageName = new JsonParser(this.mParentWindow.mVmName).GetAppInfoFromPackageName(package);
      if (infoFromPackageName != null)
        this.AddIcon(infoFromPackageName);
      return infoFromPackageName;
    }

    private void AddAppSuggestionIcon(AppSuggestionPromotion appSuggestionInfo)
    {
      string appPackage = appSuggestionInfo.AppPackage;
      double height = 50.0;
      double width = 50.0;
      AppIconModel newIconForKey = this.GetNewIconForKey(appPackage);
      try
      {
        if (newIconForKey == null)
          return;
        newIconForKey.IsAppSuggestionActive = true;
        newIconForKey.PackageName = appPackage;
        if (appSuggestionInfo.IsShowRedDot)
          newIconForKey.IsRedDotVisible = true;
        newIconForKey.Init(appSuggestionInfo);
        if (appSuggestionInfo.IsEmailRequired && !RegistryManager.Instance.Guest[this.mParentWindow.mVmName].IsGoogleSigninDone)
          return;
        if (string.Equals(appSuggestionInfo.AppLocation, "dock", StringComparison.InvariantCultureIgnoreCase))
        {
          if (appSuggestionInfo.IconHeight != 0.0)
            height = appSuggestionInfo.IconHeight;
          if (appSuggestionInfo.IconWidth != 0.0)
            width = appSuggestionInfo.IconWidth;
          newIconForKey.AddToDock(height, width);
          this.mHomeApp?.AddDockPanelIcon(newIconForKey, (DownloadInstallApk) null);
        }
        else if (string.Equals(appSuggestionInfo.AppLocation, "more_apps", StringComparison.InvariantCultureIgnoreCase))
        {
          newIconForKey.AddToMoreAppsDock(55.0, 55.0);
          this.mHomeApp?.AddMoreAppsDockPanelIcon(newIconForKey, (DownloadInstallApk) null);
        }
        else
        {
          newIconForKey.AddToInstallDrawer();
          this.mHomeApp?.AddInstallDrawerIcon(newIconForKey, (DownloadInstallApk) null);
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in adding app suggestion icon: " + ex.ToString());
      }
    }

    internal void AddIconWithRedDot(string appPackage)
    {
      lock (((ICollection) PromotionObject.Instance.AppSuggestionList).SyncRoot)
      {
        JsonParser jsonParser = new JsonParser(this.mParentWindow.mVmName);
        foreach (AppSuggestionPromotion appSuggestion in PromotionObject.Instance.AppSuggestionList)
        {
          if (string.Equals(appSuggestion.AppPackage, appPackage, StringComparison.InvariantCulture))
          {
            if (!jsonParser.IsAppInstalled(appSuggestion.AppPackage))
            {
              HomeAppManager.RemovePackageInRedDotShownRegistry(appSuggestion.AppPackage);
              this.AddAppSuggestionIcon(appSuggestion);
            }
            else
            {
              HomeAppManager.RemovePackageInRedDotShownRegistry(appSuggestion.AppPackage);
              this.GetAppIcon(appPackage)?.AddRedDot();
            }
          }
        }
      }
    }

    internal void AddMacroAppIcon(string package)
    {
      if (string.IsNullOrEmpty(package))
        return;
      AppIconModel newIconForKey = this.GetNewIconForKey(package + "_macro");
      string appname = LocaleStrings.GetLocalizedString("STRING_REROLL_APP_PREFIX", "") + " - " + newIconForKey.AppName;
      newIconForKey.InitRerollIcon(package, appname);
      newIconForKey.AddToInstallDrawer();
      this.mHomeApp?.AddInstallDrawerIcon(newIconForKey, (DownloadInstallApk) null);
    }

    internal void AddAppIcon(
      string package,
      string appName,
      string apkUrl,
      DownloadInstallApk downloader)
    {
      if (string.IsNullOrEmpty(package))
        return;
      AppIconModel newIconForKey = this.GetNewIconForKey(package);
      newIconForKey.Init(package, appName, apkUrl);
      newIconForKey.AddToInstallDrawer();
      this.mHomeApp?.AddInstallDrawerIcon(newIconForKey, downloader);
    }

    private void AddIcon(AppInfo item)
    {
      AppIconModel newIconForKey = this.GetNewIconForKey(item.Package);
      newIconForKey.Init(item);
      newIconForKey.AddToInstallDrawer();
      this.mHomeApp?.AddInstallDrawerIcon(newIconForKey, (DownloadInstallApk) null);
    }

    private AppIconModel GetNewIconForKey(string key)
    {
      AppIconModel newAppIconCreated = new AppIconModel();
      this.RemoveAppIcon(key, newAppIconCreated);
      this.dictAppIcons[key] = newAppIconCreated;
      return newAppIconCreated;
    }

    internal bool CheckDictAppIconFor(string packagename, Predicate<AppIconModel> pred)
    {
      return this.dictAppIcons.ContainsKey(packagename) && pred(this.dictAppIcons[packagename]);
    }

    internal AppIconModel GetAppIcon(string packageName)
    {
      if (FeatureManager.Instance.IsCustomUIForNCSoft && packageName == BlueStacksUIUtils.sUserAccountPackageName)
      {
        Logger.Info("Setting packageName to com.android.vending when com.uncube.account is received");
        packageName = "com.android.vending";
      }
      AppIconModel appIconModel = (AppIconModel) null;
      if (this.dictAppIcons.ContainsKey(packageName) && !string.IsNullOrEmpty(packageName))
        appIconModel = this.dictAppIcons[packageName];
      return appIconModel;
    }

    internal AppIconModel GetMacroAppIcon(string packageName)
    {
      return this.GetAppIcon(packageName + "_macro");
    }

    internal void RemoveAppIcon(string package, AppIconModel newAppIconCreated = null)
    {
      if (package == null || !this.dictAppIcons.ContainsKey(package))
        return;
      if (newAppIconCreated != null)
        newAppIconCreated.AppIncompatType = this.dictAppIcons[package].AppIncompatType;
      this.mHomeApp?.RemoveAppIconFromUI(this.dictAppIcons[package]);
      this.dictAppIcons.Remove(package);
    }

    internal void RemoveAppAfterUninstall(string package)
    {
      GrmHandler.RemovePackageFromGrmList(package, this.mParentWindow.mVmName);
      this.RemoveAppIcon(package, (AppIconModel) null);
      this.RemoveAppIcon(package + "_macro", (AppIconModel) null);
      try
      {
        string path = Path.Combine(RegistryStrings.GadgetDir, Regex.Replace(package + ".png", "[\\x22\\\\\\/:*?|<>]", " "));
        if (!File.Exists(path))
          return;
        File.Delete(path);
      }
      catch (Exception ex)
      {
        Logger.Info("Not able to delete image file : " + ex.ToString());
      }
    }

    internal void UpdateGamepadIcons(bool isGamepadConnected)
    {
      foreach (KeyValuePair<string, AppIconModel> dictAppIcon in this.dictAppIcons)
      {
        if (dictAppIcon.Value.IsGamepadCompatible)
          dictAppIcon.Value.IsGamepadConnected = isGamepadConnected;
      }
    }

    internal void OpenApp(string packageName, bool isCheckForGrm = true)
    {
      AppIconModel appIcon = this.GetAppIcon(packageName);
      if (appIcon == null)
        return;
      if ((uint) appIcon.AppIncompatType > 0U & isCheckForGrm && !this.mParentWindow.mTopBar.mAppTabButtons.mDictTabs.ContainsKey(packageName))
      {
        GrmHandler.HandleCompatibility(appIcon.PackageName, this.mParentWindow.mVmName);
      }
      else
      {
        this.mParentWindow.mTopBar.mAppTabButtons.AddAppTab(appIcon.AppName, appIcon.PackageName, appIcon.ActivityName, appIcon.ImageName, false, true, false);
        this.mParentWindow.mAppHandler.SwitchWhenPackageNameRecieved = appIcon.PackageName;
        this.mParentWindow.mAppHandler.SendRunAppRequestAsync(appIcon.PackageName, "", false);
        if (appIcon.IsRedDotVisible)
        {
          appIcon.IsRedDotVisible = false;
          HomeAppManager.AddPackageInRedDotShownRegistry(appIcon.PackageName);
        }
        HomeAppManager.SendStats(appIcon.PackageName);
      }
    }

    private static void SendStats(string packageName)
    {
      if (packageName == "com.android.vending")
        ClientStats.SendGPlayClickStats(new Dictionary<string, string>()
        {
          {
            "source",
            "bs3_myapps"
          }
        });
      ClientStats.SendClientStatsAsync("init", "success", "app_activity", packageName, "", "");
    }

    private static bool CheckIfPresentInRedDotShownRegistry(string package)
    {
      string redDotShownOnIcon = RegistryManager.Instance.RedDotShownOnIcon;
      if (!string.IsNullOrEmpty(redDotShownOnIcon))
      {
        char[] separator = new char[1]{ ',' };
        foreach (string str in redDotShownOnIcon.Split(separator, StringSplitOptions.None))
        {
          if (!string.IsNullOrEmpty(package) && str.Equals(package, StringComparison.InvariantCultureIgnoreCase))
            return true;
        }
      }
      return false;
    }

    private static void RemovePackageInRedDotShownRegistry(string appPackage)
    {
      string[] array = ((IEnumerable<string>) RegistryManager.Instance.RedDotShownOnIcon.Split(new char[1]
      {
        ','
      }, StringSplitOptions.None)).Where<string>((Func<string, bool>) (w => !w.Contains(appPackage))).ToArray<string>();
      string str1 = string.Empty;
      foreach (string str2 in array)
      {
        if (!string.IsNullOrEmpty(str2))
          str1 = str1 + str2.ToString((IFormatProvider) CultureInfo.InvariantCulture) + ",";
      }
      RegistryManager.Instance.RedDotShownOnIcon = str1;
    }

    internal static void AddPackageInRedDotShownRegistry(string appPackage)
    {
      string redDotShownOnIcon = RegistryManager.Instance.RedDotShownOnIcon;
      RegistryManager.Instance.RedDotShownOnIcon = string.IsNullOrEmpty(redDotShownOnIcon) ? appPackage : redDotShownOnIcon + "," + appPackage;
    }

    internal void DownloadStarted(string packageName)
    {
      if (!this.dictAppIcons.ContainsKey(packageName))
        return;
      this.dictAppIcons[packageName].DownloadStarted();
      Publisher.PublishMessage(BrowserControlTags.apkDownloadStarted, this.mParentWindow.mVmName, new JObject()
      {
        ["PackageName"] = (JToken) packageName,
        ["AppName"] = (JToken) this.dictAppIcons[packageName].AppName,
        ["ApkUrl"] = (JToken) this.dictAppIcons[packageName].ApkUrl
      });
    }

    internal void UpdateAppDownloadProgress(string packageName, int percent)
    {
      if (!this.dictAppIcons.ContainsKey(packageName))
        return;
      this.dictAppIcons[packageName].UpdateAppDownloadProgress(percent);
      Publisher.PublishMessage(BrowserControlTags.apkDownloadCurrentProgress, this.mParentWindow.mVmName, new JObject()
      {
        ["PackageName"] = (JToken) packageName,
        ["DownloadPercent"] = (JToken) percent
      });
    }

    internal void DownloadFailed(string packageName)
    {
      if (!this.dictAppIcons.ContainsKey(packageName))
        return;
      this.dictAppIcons[packageName].DownloadFailed();
      if (FeatureManager.Instance.IsHtmlHome)
        this.RemoveAppIcon(packageName, (AppIconModel) null);
      Publisher.PublishMessage(BrowserControlTags.apkDownloadFailed, this.mParentWindow.mVmName, new JObject((object) new JProperty("PackageName", (object) packageName)));
    }

    internal void DownloadCompleted(string packageName, string filePath)
    {
      if (!this.dictAppIcons.ContainsKey(packageName))
        return;
      this.dictAppIcons[packageName].DownloadCompleted(filePath);
      Publisher.PublishMessage(BrowserControlTags.apkDownloadCompleted, this.mParentWindow.mVmName, new JObject((object) new JProperty("PackageName", (object) packageName)));
    }

    internal void ApkInstallStart(string packageName, string filePath)
    {
      if (!this.dictAppIcons.ContainsKey(packageName))
        return;
      this.dictAppIcons[packageName].ApkInstallStart(filePath);
      Publisher.PublishMessage(BrowserControlTags.apkInstallStarted, this.mParentWindow.mVmName, new JObject()
      {
        ["PackageName"] = (JToken) packageName,
        ["AppName"] = (JToken) this.dictAppIcons[packageName].AppName,
        ["ApkFilePath"] = (JToken) filePath
      });
    }

    internal void ApkInstallFailed(string packageName)
    {
      if (!this.dictAppIcons.ContainsKey(packageName))
        return;
      this.dictAppIcons[packageName].ApkInstallFailed();
      if (FeatureManager.Instance.IsHtmlHome)
        this.RemoveAppIcon(packageName, (AppIconModel) null);
      Publisher.PublishMessage(BrowserControlTags.apkInstallFailed, this.mParentWindow.mVmName, new JObject((object) new JProperty("PackageName", (object) packageName)));
    }

    internal void ApkInstallCompleted(string packageName)
    {
      if (!this.dictAppIcons.ContainsKey(packageName))
        return;
      this.dictAppIcons[packageName].ApkInstallCompleted();
      Publisher.PublishMessage(BrowserControlTags.apkInstallCompleted, this.mParentWindow.mVmName, new JObject((object) new JProperty("PackageName", (object) packageName)));
    }

    internal void HomeTabSwitchActions(bool isHomeTabSelected)
    {
      if (isHomeTabSelected)
      {
        HomeApp mHomeApp = this.mHomeApp;
        if ((mHomeApp != null ? (mHomeApp.mSearchTextBox.IsFocused ? 1 : 0) : 0) != 0)
          this.SetSearchTextBoxFocus(100);
        this.mParentWindow.mWelcomeTab.ReloadHomeTabIME();
        this.mParentWindow.StaticComponents.PlayPauseGifs(true);
      }
      else
        this.mParentWindow.StaticComponents.PlayPauseGifs(false);
    }

    internal void SetSearchTextBoxFocus(int delay)
    {
      MiscUtils.SetFocusAsync((UIElement) this.mHomeApp?.mSearchTextBox, delay);
    }

    internal void EnableSearchTextBox(bool isEnable)
    {
      if (this.mHomeApp == null)
        return;
      this.mHomeApp.mSearchTextBox.IsEnabled = isEnable;
    }

    internal void ChangeHomeAppVisibility(Visibility visibility)
    {
      if (this.mHomeApp == null)
        return;
      this.mHomeApp.Visibility = visibility;
    }

    internal void RestoreWallpaper()
    {
      this.mHomeApp?.RestoreWallpaperImage();
    }

    internal void ApplyWallpaper()
    {
      this.mHomeApp?.ApplyWallpaperImage();
    }

    internal void ClearAppRecommendationPool()
    {
      this.mHomeApp?.sAppRecommendationsPool.Clear();
    }

    internal void AddToAppRecommendationPool(RecommendedApps recomApp)
    {
      this.mHomeApp?.sAppRecommendationsPool.Add(recomApp);
    }

    internal void UpdateRecommendedAppsInstallStatus(string package)
    {
      this.mHomeApp?.UpdateRecommendedAppsInstallStatus(package);
    }

    internal void InitiateHtmlSidePanel()
    {
      HomeApp mHomeApp = this.mHomeApp;
      if ((mHomeApp != null ? (!mHomeApp.SideHtmlBrowserInited ? 1 : 0) : 0) == 0)
        return;
      this.mHomeApp?.InitiateSideHtmlBrowser();
    }

    internal void DisposeHtmlSidePanel()
    {
      this.mHomeApp?.SideHtmlBrowser?.DisposeBrowser();
    }

    internal void ReinitHtmlSidePanel()
    {
      this.mHomeApp?.SideHtmlBrowser?.UpdateUrlAndRefresh(BlueStacksUIUtils.GetHtmlSidePanelUrl());
    }

    internal void CloseHomeAppPopups()
    {
      if (this.mHomeApp == null)
        return;
      this.mHomeApp.mSuggestedAppPopUp.IsOpen = false;
      this.mHomeApp.mMoreAppsDockPopup.IsOpen = false;
    }

    internal void ChangeHomeAppLoadingGridVisibility(Visibility visibility)
    {
      if (this.mHomeApp == null)
        return;
      this.mHomeApp.mLoadingGrid.Visibility = visibility;
    }

    internal double GetAppRecommendationsGridWidth()
    {
      HomeApp mHomeApp = this.mHomeApp;
      return (mHomeApp != null ? (mHomeApp.mAppRecommendationsGrid.ActualWidth > 0.0 ? 1 : 0) : 0) == 0 ? 0.0 : this.mHomeApp.mAppRecommendationsGrid.ActualWidth;
    }

    internal void ShowDockIconTooltip(AppIconUI icon, bool isOpen)
    {
      if (this.mHomeApp == null)
        return;
      if (isOpen)
      {
        this.mHomeApp.mDockIconText.Text = icon.mAppIconModel.AppName;
        this.mHomeApp.mDockAppIconToolTipPopup.PlacementTarget = (UIElement) icon.mAppImage;
        this.mHomeApp.mDockAppIconToolTipPopup.IsOpen = true;
        this.mHomeApp.mDockAppIconToolTipPopup.StaysOpen = true;
      }
      else
        this.mHomeApp.mDockAppIconToolTipPopup.IsOpen = false;
    }

    internal void CloseAppSuggestionPopup()
    {
      if (this.mHomeApp == null)
        return;
      this.mHomeApp.mSuggestedAppPopUp.IsOpen = false;
    }

    internal void OpenAppSuggestionPopup(
      AppSuggestionPromotion appInfoForShowingPopup,
      UIElement appNameTextBlock,
      bool staysOpen = true)
    {
      if (this.mHomeApp == null || appInfoForShowingPopup.ToolTip == null)
        return;
      this.mHomeApp.mSuggestedAppPopUp.PlacementTarget = appNameTextBlock;
      this.mHomeApp.mSuggestedAppPopUp.IsOpen = true;
      this.mHomeApp.mSuggestedAppPopUp.StaysOpen = staysOpen;
      this.mHomeApp.mAppSuggestionPopUp.Text = appInfoForShowingPopup.ToolTip;
    }
  }
}
