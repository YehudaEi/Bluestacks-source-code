// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.AppIconModel
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

namespace BlueStacks.BlueStacksUI
{
  public class AppIconModel : INotifyPropertyChanged
  {
    private string mImageName = string.Empty;
    private string mAppName = string.Empty;
    private TextWrapping mAppNameTextWrapping = TextWrapping.NoWrap;
    private string mApplyImageBorder = string.Empty;
    private AppInfo mAppInfoItem;
    private Visibility mAppIconVisibility;
    private string mAppNameTooltip;
    private TextTrimming mAppNameTextTrimming;
    private bool mIsGamepadCompatible;
    private bool mIsGamepadConnected;
    private bool mIsRedDotVisible;
    private AppIncompatType mAppIncompatType;

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string property)
    {
      PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
      if (propertyChanged == null)
        return;
      propertyChanged((object) this, new PropertyChangedEventArgs(property));
    }

    public string PackageName { get; set; } = string.Empty;

    public string ActivityName { get; set; } = string.Empty;

    public Visibility AppIconVisibility
    {
      get
      {
        return this.mAppIconVisibility;
      }
      set
      {
        this.mAppIconVisibility = value;
        this.OnPropertyChanged(nameof (AppIconVisibility));
      }
    }

    public string ImageName
    {
      get
      {
        return this.mImageName;
      }
      set
      {
        this.mImageName = value;
        this.OnPropertyChanged(nameof (ImageName));
      }
    }

    public string AppName
    {
      get
      {
        return this.mAppName;
      }
      set
      {
        this.mAppName = value;
        this.OnPropertyChanged(nameof (AppName));
        this.AppNameTooltip = value;
      }
    }

    public string AppNameTooltip
    {
      get
      {
        return this.mAppNameTooltip;
      }
      set
      {
        this.mAppNameTooltip = value;
        this.OnPropertyChanged(nameof (AppNameTooltip));
      }
    }

    public TextTrimming AppNameTextTrimming
    {
      get
      {
        return this.mAppNameTextTrimming;
      }
      set
      {
        this.mAppNameTextTrimming = value;
        this.OnPropertyChanged(nameof (AppNameTextTrimming));
      }
    }

    public TextWrapping AppNameTextWrapping
    {
      get
      {
        return this.mAppNameTextWrapping;
      }
      set
      {
        this.mAppNameTextWrapping = value;
        this.OnPropertyChanged(nameof (AppNameTextWrapping));
      }
    }

    public bool IsGamepadCompatible
    {
      get
      {
        return this.mIsGamepadCompatible;
      }
      set
      {
        this.mIsGamepadCompatible = value;
        this.OnPropertyChanged(nameof (IsGamepadCompatible));
      }
    }

    public bool IsGamepadConnected
    {
      get
      {
        return this.mIsGamepadConnected;
      }
      set
      {
        this.mIsGamepadConnected = value;
        this.OnPropertyChanged(nameof (IsGamepadConnected));
      }
    }

    public bool IsRedDotVisible
    {
      get
      {
        return this.mIsRedDotVisible;
      }
      set
      {
        this.mIsRedDotVisible = value;
        this.OnPropertyChanged(nameof (IsRedDotVisible));
      }
    }

    public string ApkUrl { get; set; } = string.Empty;

    public bool IsGifIcon { get; set; }

    public bool IsAppSuggestionActive { get; set; }

    public bool mIsAppRemovable { get; set; } = true;

    public bool IsGl3App { get; set; }

    public AppIncompatType AppIncompatType
    {
      get
      {
        return this.mAppIncompatType;
      }
      set
      {
        this.mAppIncompatType = value;
        this.OnPropertyChanged(nameof (AppIncompatType));
      }
    }

    public bool IsDownloading { get; set; }

    public double DownloadPercentage { get; set; }

    public bool IsInstalling { get; set; }

    public bool IsDownLoadingFailed { get; set; }

    public bool IsInstallingFailed { get; set; }

    public bool IsInstalledApp { get; set; } = true;

    public int MyAppPriority { get; set; } = 999;

    public bool IsRerollIcon { get; set; }

    public string ApkFilePath { get; set; } = string.Empty;

    public bool mIsAppInstalled { get; set; } = true;

    public AppIconLocation IconLocation { get; set; }

    public double IconHeight { get; set; } = 60.0;

    public double IconWidth { get; set; } = 60.0;

    public string ApplyImageBorder
    {
      get
      {
        return this.mApplyImageBorder;
      }
      set
      {
        this.mApplyImageBorder = value;
        this.OnPropertyChanged(nameof (ApplyImageBorder));
      }
    }

    public string mPromotionId { get; private set; }

    public event System.Action<AppIconDownloadingPhases> mAppDownloadingEvent;

    public AppSuggestionPromotion AppSuggestionInfo { get; private set; }

    private void Init(string package, string appName)
    {
      if (AppHandler.ListIgnoredApps.Contains<string>(package, (IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase) || string.Equals(this.PackageName, "macro_recorder", StringComparison.InvariantCulture))
        this.AppIconVisibility = Visibility.Collapsed;
      this.PackageName = package;
      this.AppName = appName;
      if (!RegistryManager.Instance.IsShowIconBorder)
        return;
      this.ApplyBorder("appFrameIcon");
    }

    internal void InitRerollIcon(string package, string appname)
    {
      this.Init(package, appname);
      this.IsRerollIcon = true;
    }

    internal void Init(string package, string appName, string apkUrl)
    {
      this.Init(package, appName);
      this.ApkUrl = apkUrl;
    }

    internal void Init(AppInfo item)
    {
      this.mAppInfoItem = item;
      this.Init(item.Package, item.Name);
      this.LoadDownloadAppIcon();
      this.ActivityName = item.Activity;
      if (item.Gl3Required)
        this.IsGl3App = true;
      this.IsGamepadCompatible = item.IsGamepadCompatible;
      if (!this.IsGamepadCompatible)
        return;
      this.AppNameTextWrapping = TextWrapping.NoWrap;
    }

    internal void Init(AppSuggestionPromotion appSuggestionInfo)
    {
      this.AppSuggestionInfo = appSuggestionInfo;
      this.AppName = appSuggestionInfo.AppName;
      this.ActivityName = appSuggestionInfo.AppActivity;
      this.IsAppSuggestionActive = true;
      this.AppNameTooltip = string.IsNullOrEmpty(this.AppSuggestionInfo.ToolTip) ? this.AppName : (string) null;
      this.AppNameTextWrapping = TextWrapping.Wrap;
      this.AppNameTextTrimming = TextTrimming.CharacterEllipsis;
      if (this.AppSuggestionInfo.ExtraPayload.ContainsKey("click_generic_action") && (EnumHelper.Parse<GenericAction>(this.AppSuggestionInfo.ExtraPayload["click_generic_action"], GenericAction.None) & (GenericAction.SettingsMenu | GenericAction.KeyBasedPopup | GenericAction.OpenSystemApp)) != (GenericAction) 0)
      {
        this.mIsAppRemovable = false;
        this.AppNameTextWrapping = TextWrapping.NoWrap;
        this.AppNameTextTrimming = TextTrimming.None;
      }
      this.mIsAppInstalled = false;
      this.mPromotionId = this.AppSuggestionInfo.AppIconId;
      if (this.AppSuggestionInfo.AppIcon.EndsWith(".gif", StringComparison.InvariantCulture))
        this.IsGifIcon = true;
      this.ImageName = this.AppSuggestionInfo.AppIconPath;
      if (!this.AppSuggestionInfo.IsIconBorder)
        return;
      this.ApplyBorder(Path.Combine(RegistryStrings.PromotionDirectory, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}.png", (object) this.AppSuggestionInfo.IconBorderId, (object) "app_suggestion_icon_border")));
    }

    private void LoadDownloadAppIcon()
    {
      if (string.IsNullOrEmpty(this.PackageName) || this.IsAppSuggestionActive)
        return;
      string str = Path.Combine(RegistryStrings.GadgetDir, Regex.Replace(this.PackageName + ".png", "[\\x22\\\\\\/:*?|<>]", " "));
      if (!AppHandler.ListIgnoredApps.Contains<string>(this.PackageName, (IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase) && (!string.IsNullOrEmpty(this.mAppInfoItem?.Img) && !File.Exists(str) && File.Exists(Path.Combine(RegistryStrings.GadgetDir, this.mAppInfoItem.Img))))
        File.Copy(Path.Combine(RegistryStrings.GadgetDir, this.mAppInfoItem.Img), str, false);
      if (!File.Exists(str))
        return;
      this.ImageName = str;
    }

    internal void AddRedDot()
    {
      this.IsRedDotVisible = true;
    }

    internal void AddToDock(double height = 50.0, double width = 50.0)
    {
      this.IconLocation = AppIconLocation.Dock;
      this.IconHeight = height;
      this.IconWidth = width;
      this.mIsAppRemovable = false;
      if (!PromotionObject.Instance.DockOrder.ContainsKey(this.PackageName) || this.MyAppPriority == PromotionObject.Instance.DockOrder[this.PackageName])
        return;
      this.MyAppPriority = PromotionObject.Instance.DockOrder[this.PackageName];
    }

    internal void AddToMoreAppsDock(double height = 55.0, double width = 55.0)
    {
      this.IconLocation = AppIconLocation.Moreapps;
      this.IconHeight = height;
      this.IconWidth = width;
      this.mIsAppRemovable = false;
      if (!PromotionObject.Instance.MoreAppsDockOrder.ContainsKey(this.PackageName) || this.MyAppPriority == PromotionObject.Instance.MoreAppsDockOrder[this.PackageName])
        return;
      this.MyAppPriority = PromotionObject.Instance.MoreAppsDockOrder[this.PackageName];
    }

    internal void AddToInstallDrawer()
    {
      if (string.Compare(this.PackageName, "com.android.vending", StringComparison.OrdinalIgnoreCase) == 0)
        this.MyAppPriority = 1;
      if (!PromotionObject.Instance.MyAppsOrder.ContainsKey(this.PackageName) || this.MyAppPriority == PromotionObject.Instance.MyAppsOrder[this.PackageName])
        return;
      this.MyAppPriority = PromotionObject.Instance.MyAppsOrder[this.PackageName];
    }

    internal void AddPromotionBorderInstalledApp(AppSuggestionPromotion appSuggestionInfo)
    {
      this.AppSuggestionInfo = appSuggestionInfo;
      if (!this.AppSuggestionInfo.IsIconBorder)
        return;
      this.ApplyBorder(this.AppSuggestionInfo.IconBorderId + "app_suggestion_icon_border");
    }

    internal void RemovePromotionBorderInstalledApp()
    {
      this.IsAppSuggestionActive = false;
      this.ApplyBorder("");
    }

    private void ApplyBorder(string path)
    {
      if (this.mPromotionId != null)
        return;
      this.ApplyImageBorder = path;
    }

    internal void DownloadStarted()
    {
      this.mIsAppInstalled = false;
      this.IsDownLoadingFailed = false;
      this.IsDownloading = true;
      System.Action<AppIconDownloadingPhases> downloadingEvent = this.mAppDownloadingEvent;
      if (downloadingEvent == null)
        return;
      downloadingEvent(AppIconDownloadingPhases.DownloadStarted);
    }

    internal void UpdateAppDownloadProgress(int percent)
    {
      this.DownloadPercentage = (double) percent;
      System.Action<AppIconDownloadingPhases> downloadingEvent = this.mAppDownloadingEvent;
      if (downloadingEvent == null)
        return;
      downloadingEvent(AppIconDownloadingPhases.Downloading);
    }

    internal void DownloadFailed()
    {
      this.IsDownLoadingFailed = true;
      System.Action<AppIconDownloadingPhases> downloadingEvent = this.mAppDownloadingEvent;
      if (downloadingEvent == null)
        return;
      downloadingEvent(AppIconDownloadingPhases.DownloadFailed);
    }

    internal void DownloadCompleted(string filePath)
    {
      this.IsDownloading = false;
      this.IsInstalling = true;
      this.ApkFilePath = filePath;
      System.Action<AppIconDownloadingPhases> downloadingEvent = this.mAppDownloadingEvent;
      if (downloadingEvent == null)
        return;
      downloadingEvent(AppIconDownloadingPhases.DownloadCompleted);
    }

    internal void ApkInstallStart(string filePath)
    {
      this.mIsAppInstalled = false;
      this.IsInstalling = true;
      this.IsInstallingFailed = false;
      this.ApkFilePath = filePath;
      System.Action<AppIconDownloadingPhases> downloadingEvent = this.mAppDownloadingEvent;
      if (downloadingEvent == null)
        return;
      downloadingEvent(AppIconDownloadingPhases.InstallStarted);
    }

    internal void ApkInstallFailed()
    {
      if (!this.mIsAppInstalled)
        this.IsInstallingFailed = true;
      System.Action<AppIconDownloadingPhases> downloadingEvent = this.mAppDownloadingEvent;
      if (downloadingEvent == null)
        return;
      downloadingEvent(AppIconDownloadingPhases.InstallFailed);
    }

    internal void ApkInstallCompleted()
    {
      this.mIsAppInstalled = true;
      this.IsInstalling = false;
      System.Action<AppIconDownloadingPhases> downloadingEvent = this.mAppDownloadingEvent;
      if (downloadingEvent == null)
        return;
      downloadingEvent(AppIconDownloadingPhases.InstallCompleted);
    }
  }
}
