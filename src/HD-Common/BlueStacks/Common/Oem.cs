// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.Oem
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace BlueStacks.Common
{
  public sealed class Oem
  {
    private static object syncRoot = new object();
    private Dictionary<string, string> mPropertiesDictionary = new Dictionary<string, string>();
    private string mDefaultTitle = nameof (DefaultTitle);
    private string mBlueStacksApkHandlerTitle = nameof (BlueStacksApkHandlerTitle);
    private string mSnapShotShareString = nameof (SnapShotShareString);
    private static volatile Oem sInstance;
    public const string sFileName = "Oem.cfg";

    public static Oem Instance
    {
      get
      {
        if (Oem.sInstance == null)
        {
          lock (Oem.syncRoot)
          {
            if (Oem.sInstance == null)
            {
              Oem oem = new Oem();
              oem.LoadOem();
              Oem.sInstance = oem;
            }
          }
        }
        return Oem.sInstance;
      }
    }

    private void LoadOem()
    {
      try
      {
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Oem.cfg");
        if (!File.Exists(path))
        {
          if (!File.Exists(Oem.CurrentOemFilePath))
            throw new Exception(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} file not found", (object) "Oem.cfg"));
        }
        else
          Oem.CurrentOemFilePath = path;
        Logger.Info("Attempting to load {0} from path: {1}", (object) "Oem.cfg", (object) Oem.CurrentOemFilePath);
        foreach (string readAllLine in File.ReadAllLines(Oem.CurrentOemFilePath))
        {
          if (readAllLine.IndexOf("=", StringComparison.OrdinalIgnoreCase) != -1)
          {
            string[] strArray = readAllLine.Split('=');
            this.mPropertiesDictionary[strArray[0].Trim()] = strArray[1].Trim();
          }
        }
        this.LoadProperties();
      }
      catch (Exception ex)
      {
        Logger.Error("An exception occured while loading Oem config file, loading default values: " + ex.Message);
        Oem.sInstance = new Oem();
      }
    }

    private T GetObjectValueForKey<T>(string propertyName, T defaultValue)
    {
      return this.mPropertiesDictionary.ContainsKey(propertyName) ? this.mPropertiesDictionary[propertyName].GetObjectOfType<T>(defaultValue) : defaultValue;
    }

    private string GetStringValueOrDefault(string propertyName, string defaultValue)
    {
      return this.mPropertiesDictionary.ContainsKey(propertyName) ? this.mPropertiesDictionary[propertyName] : defaultValue;
    }

    private void LoadProperties()
    {
      this.WindowsOEMFeatures = this.GetObjectValueForKey<ulong>("WindowsOEMFeatures", 0UL);
      this.AppsOEMFeaturesBits = this.GetObjectValueForKey<ulong>("AppsOEMFeaturesBits", 0UL);
      this.PartnerControlBarHeight = this.GetObjectValueForKey<int>("PartnerControlBarHeight", 0);
      this.AndroidFeatureBits = this.GetObjectValueForKey<uint>("AndroidFeatureBits", 0U);
      this.SendAppClickStatsFromClient = this.GetObjectValueForKey<bool>("SendAppClickStatsFromClient", false);
      this.IsPixelParityToBeIgnored = this.GetObjectValueForKey<bool>("IsPixelParityToBeIgnored", false);
      this.IsShowVersionOnSysTrayToolTip = this.GetObjectValueForKey<bool>("IsShowVersionOnSysTrayToolTip", true);
      this.IsVTPopupEnabled = this.GetObjectValueForKey<bool>("IsVTPopupEnabled", false);
      this.IsUseFrontendBanner = this.GetObjectValueForKey<bool>("IsUseFrontendBanner", false);
      this.IsFrontendFormLocation6 = this.GetObjectValueForKey<bool>("IsFrontendFormLocation6", false);
      this.IsMessageBoxToBeDisplayed = this.GetObjectValueForKey<bool>("IsMessageBoxToBeDisplayed", true);
      this.IsResizeFrontendWindow = this.GetObjectValueForKey<bool>("IsResizeFrontendWindow", true);
      this.IsFrontendBorderHidden = this.GetObjectValueForKey<bool>("IsFrontendBorderHidden", false);
      this.IsOnlyStopButtonToBeAddedInContextMenuOFSysTray = this.GetObjectValueForKey<bool>("IsOnlyStopButtonToBeAddedInContextMenuOFSysTray", false);
      this.IsCountryChina = this.GetObjectValueForKey<bool>("IsCountryChina", false);
      this.IsLoadCACodeFromCloud = this.GetObjectValueForKey<bool>("IsLoadCACodeFromCloud", true);
      this.IsSendGameManagerRequest = this.GetObjectValueForKey<bool>("IsSendGameManagerRequest", false);
      this.IsHideMessageBoxIconInTaskBar = this.GetObjectValueForKey<bool>("IsHideMessageBoxIconInTaskBar", false);
      this.IsLefClickOnTrayIconLaunchPartner = this.GetObjectValueForKey<bool>("IsLefClickOnTrayIconLaunchPartner", false);
      this.IsGameManagerToBeStartedOnRunApp = this.GetObjectValueForKey<bool>("IsGameManagerToBeStartedOnRunApp", false);
      this.IsCreateDesktopIconForApp = this.GetObjectValueForKey<bool>("IsCreateDesktopIconForApp", false);
      this.IsDownloadIconFromWeb = this.GetObjectValueForKey<bool>("IsDownloadIconFromWeb", false);
      this.IsSysTrayIconTextToBeBlueStacks3 = this.GetObjectValueForKey<bool>("IsSysTrayIconTextToBeBlueStacks3", false);
      this.IsDownloadBstConfigWhileInstalling = this.GetObjectValueForKey<bool>("IsDownloadBstConfigWhileInstalling", true);
      this.IsOEMWithBGPClient = this.GetObjectValueForKey<bool>("IsOEMWithBGPClient", false);
      this.IsLaunchUIOnRunApp = this.GetObjectValueForKey<bool>("IsLaunchUIOnRunApp", false);
      this.IsRemoveAccountOnExit = this.GetObjectValueForKey<bool>("IsRemoveAccountOnExit", false);
      this.IsFormBorderStyleFixedSingle = this.GetObjectValueForKey<bool>("IsFormBorderStyleFixedSingle", false);
      this.CreateDesktopIcons = this.GetObjectValueForKey<bool>("CreateDesktopIcons", true);
      this.CreateMultiInstanceManagerIcon = this.GetObjectValueForKey<bool>("CreateMultiInstanceManagerIcon", false);
      this.IsBackupWarningVisible = this.GetObjectValueForKey<bool>("IsBackupWarningVisible", true);
      this.IsWriteRegistryInfoInFile = this.GetObjectValueForKey<bool>("IsWriteRegistryInfoInFile", false);
      this.IsCreateInstallApkRegistry = this.GetObjectValueForKey<bool>("IsCreateInstallApkRegistry", true);
      this.IsCreateDesktopAndStartMenuShortcut = this.GetObjectValueForKey<bool>("IsCreateDesktopAndStartMenuShortcut", true);
      this.IsDragDropEnabled = this.GetObjectValueForKey<bool>("IsDragDropEnabled", true);
      this.IsProductBeta = this.GetObjectValueForKey<bool>("IsProductBeta", false);
      this.IsSwitchToAndroidHome = this.GetObjectValueForKey<bool>("IsSwitchToAndroidHome", false);
      this.IsResetSigninRegistryForFreshVM = this.GetObjectValueForKey<bool>("IsResetSigninRegistryForFreshVM", true);
      this.CreateUninstallEntry = this.GetObjectValueForKey<bool>("CreateUninstallEntry", true);
      this.CheckForAGAInInstaller = this.GetObjectValueForKey<bool>("CheckForAGAInInstaller", false);
      this.IsReportExeAppCrashLogs = this.GetObjectValueForKey<bool>("IsReportExeAppCrashLogs", false);
      this.IsPortableInstaller = this.GetObjectValueForKey<bool>("IsPortableInstaller", false);
      this.IsAndroid64Bit = this.GetObjectValueForKey<bool>("IsAndroid64Bit", false);
      this.ChangePerformanceSettingOnInstanceCountChange = this.GetObjectValueForKey<bool>("ChangePerformanceSettingOnInstanceCountChange", false);
      this.SetDefaultPreferenceHigh = this.GetObjectValueForKey<bool>("SetDefaultPreferenceHigh", false);
      this.IsShowMimPromotionalTeaser = this.GetObjectValueForKey<bool>("IsShowMimPromotionalTeaser", true);
      this.IsShowMimHelpIcon = this.GetObjectValueForKey<bool>("IsShowMimHelpIcon", true);
      this.IsShowMimUpdateIcon = this.GetObjectValueForKey<bool>("IsShowMimUpdateIcon", true);
      this.IsShowMimOtherOEM = this.GetObjectValueForKey<bool>("IsShowMimOtherOEM", true);
      this.IsShowGameBlurb = this.GetObjectValueForKey<bool>("IsShowGameBlurb", true);
      this.MaxBatchInstanceCreationCount = this.GetObjectValueForKey<int>("MaxBatchInstanceCreationCount", 5);
      this.MimAllowPerformanceResetForInstanceCreation = this.GetObjectValueForKey<bool>("MimAllowPerformanceResetForInstanceCreation", false);
      this.MimHighPerformanceInstanceMaxCount = this.GetObjectValueForKey<int>("MimHighPerformanceInstanceMaxCount", int.MaxValue);
      this.MimMediumPerformanceInstanceMaxCount = this.GetObjectValueForKey<int>("MimMediumPerformanceInstanceMaxCount", int.MaxValue);
      this.MsgWindowClassName = this.GetStringValueOrDefault("MsgWindowClassName", (string) null);
      this.MsgWindowTitle = this.GetStringValueOrDefault("MsgWindowTitle", (string) null);
      this.OEM = this.GetStringValueOrDefault("OEM", "gamemanager");
      this.DNSValue = this.GetStringValueOrDefault("DNSValue", "8.8.8.8");
      this.DNS2Value = this.GetStringValueOrDefault("DNS2Value", "10.0.2.3");
      this.mDefaultTitle = this.GetStringValueOrDefault("DefaultTitle", "DefaultTitle");
      this.DesktopShortcutFileName = this.GetStringValueOrDefault("DesktopShortcutFileName", "");
      this.mBlueStacksApkHandlerTitle = this.GetStringValueOrDefault("BlueStacksApkHandlerTitle", "BlueStacksApkHandlerTitle");
      this.CommonAppTitleText = this.GetStringValueOrDefault("CommonAppTitleText", "BlueStacks Android Plugin");
      this.mSnapShotShareString = this.GetStringValueOrDefault("SnapShotShareString", "SnapShotShareString");
      this.DMMApiPrefix = this.GetStringValueOrDefault("DMMApiPrefix", string.Empty);
      this.ControlPanelDisplayName = this.GetStringValueOrDefault("ControlPanelDisplayName", "BlueStacks App Player");
      this.SendCrashLogForApkWithString = this.GetStringValueOrDefault("SendCrashLogForApkWithString", "");
      this.MultiInstanceManagerShortcutFileName = this.GetStringValueOrDefault("MultiInstanceManagerShortcutFileName", "BlueStacks Multi-Instance Manager.lnk");
      this.IsShowAllInstancesToBeAddedInContextMenuOfSysTray = this.GetObjectValueForKey<bool>("IsShowAllInstancesToBeAddedInContextMenuOfSysTray", false);
    }

    public string GetTitle(string title)
    {
      return this.DefaultTitle.Equals("DefaultTitle", StringComparison.OrdinalIgnoreCase) ? title : this.DefaultTitle;
    }

    public ulong WindowsOEMFeatures { get; set; }

    public ulong AppsOEMFeaturesBits { get; set; }

    public string MsgWindowClassName { get; set; }

    public string MsgWindowTitle { get; set; }

    public uint AndroidFeatureBits { get; set; }

    public bool SendAppClickStatsFromClient { get; set; }

    public bool IsPixelParityToBeIgnored { get; set; }

    public bool IsShowVersionOnSysTrayToolTip { get; set; } = true;

    public bool IsVTPopupEnabled { get; set; }

    public bool IsUseFrontendBanner { get; set; }

    public bool IsFrontendFormLocation6 { get; set; }

    public bool IsMessageBoxToBeDisplayed { get; set; } = true;

    public int PartnerControlBarHeight { get; set; }

    public bool IsResizeFrontendWindow { get; set; } = true;

    public bool IsFrontendBorderHidden { get; set; }

    public bool IsOnlyStopButtonToBeAddedInContextMenuOFSysTray { get; set; }

    public bool IsCountryChina { get; set; }

    public bool IsLoadCACodeFromCloud { get; set; } = true;

    public bool IsSendGameManagerRequest { get; set; }

    public bool IsHideMessageBoxIconInTaskBar { get; set; }

    public bool IsLefClickOnTrayIconLaunchPartner { get; set; }

    public bool IsGameManagerToBeStartedOnRunApp { get; set; }

    public bool IsCreateDesktopIconForApp { get; set; }

    public bool IsDownloadBstConfigWhileInstalling { get; set; } = true;

    public bool IsDownloadIconFromWeb { get; set; }

    public bool IsSysTrayIconTextToBeBlueStacks3 { get; set; }

    public bool IsOEMWithBGPClient { get; set; }

    public bool IsLaunchUIOnRunApp { get; set; }

    public bool IsRemoveAccountOnExit { get; set; }

    public bool IsFormBorderStyleFixedSingle { get; set; }

    public bool IsBackupWarningVisible { get; set; } = true;

    public bool IsWriteRegistryInfoInFile { get; set; }

    public bool IsCreateInstallApkRegistry { get; set; } = true;

    public bool IsCreateDesktopAndStartMenuShortcut { get; set; } = true;

    public bool IsDragDropEnabled { get; set; } = true;

    public bool IsProductBeta { get; set; }

    public bool IsResetSigninRegistryForFreshVM { get; set; } = true;

    public bool IsReportExeAppCrashLogs { get; set; }

    public bool CreateUninstallEntry { get; private set; } = true;

    public bool CheckForAGAInInstaller { get; private set; }

    public bool CreateDesktopIcons { get; set; } = true;

    public bool CreateMultiInstanceManagerIcon { get; set; }

    public bool IsSwitchToAndroidHome { get; set; }

    public bool IsPortableInstaller { get; set; }

    public bool IsAndroid64Bit { get; set; }

    public bool ChangePerformanceSettingOnInstanceCountChange { get; set; }

    public bool SetDefaultPreferenceHigh { get; set; }

    public bool IsShowGameBlurb { get; set; } = true;

    public string OEM { get; set; } = "gamemanager";

    public string DNSValue { get; set; } = "8.8.8.8";

    public string DNS2Value { get; set; } = "10.0.2.3";

    public string DefaultTitle
    {
      get
      {
        if (string.IsNullOrEmpty(this.mDefaultTitle) || this.mDefaultTitle.Equals(nameof (DefaultTitle), StringComparison.OrdinalIgnoreCase))
          this.mDefaultTitle = LocaleStrings.GetLocalizedString(nameof (DefaultTitle), "");
        return this.mDefaultTitle;
      }
      set
      {
        this.mDefaultTitle = value;
      }
    }

    public string DesktopShortcutFileName { get; set; } = "";

    public string MultiInstanceManagerShortcutFileName { get; set; } = "BlueStacks Multi-Instance Manager.lnk";

    public string BlueStacksApkHandlerTitle
    {
      get
      {
        if (string.IsNullOrEmpty(this.mBlueStacksApkHandlerTitle) || this.mBlueStacksApkHandlerTitle.Equals(nameof (BlueStacksApkHandlerTitle), StringComparison.OrdinalIgnoreCase))
          this.mBlueStacksApkHandlerTitle = LocaleStrings.GetLocalizedString("STRING_BLUESTACKS_APK_HANDLER_TITLE", "");
        return this.mBlueStacksApkHandlerTitle;
      }
      set
      {
        this.mBlueStacksApkHandlerTitle = value;
      }
    }

    public string CommonAppTitleText { get; set; } = "BlueStacks Android Plugin";

    public string SnapShotShareString
    {
      get
      {
        if (string.IsNullOrEmpty(this.mSnapShotShareString) || this.mSnapShotShareString.Equals(nameof (SnapShotShareString), StringComparison.OrdinalIgnoreCase))
          this.mSnapShotShareString = LocaleStrings.GetLocalizedString("STRING_SNAPSHOT_SHARE_STRING", "");
        return this.mSnapShotShareString;
      }
      set
      {
        this.mSnapShotShareString = value;
      }
    }

    public string DMMApiPrefix { get; set; } = string.Empty;

    public string ControlPanelDisplayName { get; set; } = "BlueStacks App Player";

    public string SendCrashLogForApkWithString { get; set; } = "";

    public bool IsShowMimPromotionalTeaser { get; set; }

    public bool IsShowMimUpdateIcon { get; set; }

    public bool IsShowMimHelpIcon { get; set; }

    public bool IsShowMimOtherOEM { get; set; }

    public bool MimAllowPerformanceResetForInstanceCreation { get; set; }

    public int MimHighPerformanceInstanceMaxCount { get; set; } = int.MaxValue;

    public int MimMediumPerformanceInstanceMaxCount { get; set; } = int.MaxValue;

    public int MaxBatchInstanceCreationCount { get; set; } = 5;

    public bool IsShowAllInstancesToBeAddedInContextMenuOfSysTray { get; set; }

    public static bool IsOEMDmm
    {
      get
      {
        return false;
      }
    }

    public static string CurrentOemFilePath { get; set; } = Path.Combine(RegistryStrings.DataDir, "Oem.cfg");
  }
}
