// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.RegistryManager
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace BlueStacks.Common
{
  public sealed class RegistryManager
  {
    private static string mUPGRADE_TAG = string.Empty;
    private static RegistryManager sInstance = (RegistryManager) null;
    private static object sLock = new object();
    private string sCurrentEngine = "";
    private string mHost = string.Empty;
    public const string UPGRADE_TAG_NEW = ".new";
    private RegistryKey mBaseKey;
    private RegistryKey mClientKey;
    private RegistryKey mBTVKey;
    private RegistryKey mBTVFilterKey;
    private RegistryKey mUserKey;
    private RegistryKey mHostConfigKey;
    private RegistryKey mGuestsKey;
    private RegistryKey mMonitorsKey;
    private bool mIsAdmin;
    private static Dictionary<string, RegistryManager> _RegistryManagers;
    private string sUserDefinedDir;
    private string sVersion;
    private InstallationTypes mInstallationType;
    private AppLaunchState mFirstAppLaunchState;

    public static string UPGRADE_TAG
    {
      get
      {
        return RegistryManager.mUPGRADE_TAG;
      }
      set
      {
        RegistryManager.ClearRegistryMangerInstance();
        RegistryManager.mUPGRADE_TAG = value;
      }
    }

    public static RegistryManager Instance
    {
      get
      {
        if (RegistryManager.sInstance == null)
        {
          lock (RegistryManager.sLock)
          {
            if (RegistryManager.sInstance == null)
            {
              RegistryManager registryManager = new RegistryManager();
              registryManager.mIsAdmin = SystemUtils.IsAdministrator();
              registryManager.Init("bgp");
              RegistryManager.sInstance = registryManager;
              if (RegistryManager.RegistryManagers.ContainsKey("bgp"))
                RegistryManager.RegistryManagers["bgp"] = RegistryManager.sInstance;
            }
          }
        }
        return RegistryManager.sInstance;
      }
      set
      {
        RegistryManager.sInstance = value;
      }
    }

    public static void SetRegistryManagers(List<string> oems)
    {
      if (oems == null || oems.Count == 0)
        oems = new List<string>() { "bgp" };
      lock (RegistryManager.sLock)
      {
        Dictionary<string, RegistryManager> dictionary = new Dictionary<string, RegistryManager>();
        foreach (string oem in oems)
        {
          RegistryManager registryManager = new RegistryManager()
          {
            mIsAdmin = SystemUtils.IsAdministrator()
          };
          registryManager.Init(oem);
          dictionary.Add(oem, registryManager);
        }
        RegistryManager.RegistryManagers = dictionary;
      }
    }

    public static bool CheckOemInRegistry(string oemToCheck, string vmId)
    {
      bool flag = false;
      if (!string.IsNullOrEmpty(oemToCheck))
      {
        string name1 = "Software\\BlueStacks" + (oemToCheck.Equals("bgp", StringComparison.InvariantCultureIgnoreCase) ? "" : "_" + oemToCheck) + RegistryManager.UPGRADE_TAG;
        RegistryKey registryKey;
        if (!string.IsNullOrEmpty(vmId))
        {
          string name2 = name1 + "\\Guests\\" + vmId;
          registryKey = Registry.LocalMachine.OpenSubKey(name2);
        }
        else
          registryKey = Registry.LocalMachine.OpenSubKey(name1);
        flag = registryKey != null;
        if (flag)
          registryKey.Close();
      }
      return flag;
    }

    public static Dictionary<string, RegistryManager> RegistryManagers
    {
      get
      {
        if (RegistryManager._RegistryManagers == null)
          RegistryManager._RegistryManagers = new Dictionary<string, RegistryManager>()
          {
            {
              "bgp",
              RegistryManager.Instance
            }
          };
        return RegistryManager._RegistryManagers;
      }
      set
      {
        RegistryManager._RegistryManagers = value;
      }
    }

    public Dictionary<string, InstanceRegistry> Guest { get; } = new Dictionary<string, InstanceRegistry>();

    public InstanceRegistry DefaultGuest
    {
      get
      {
        return this.Guest[Strings.CurrentDefaultVmName];
      }
    }

    public string BaseKeyPath { get; private set; } = "";

    public string ClientBaseKeyPath { get; private set; } = "";

    public string BTVKeyPath { get; private set; } = "";

    public string HostConfigKeyPath { get; private set; } = "";

    private RegistryManager()
    {
    }

    private RegistryKey InitKeyWithSecurityCheck(string keyName)
    {
      return !this.mIsAdmin ? Registry.LocalMachine.OpenSubKey(keyName) : Registry.LocalMachine.CreateSubKey(keyName);
    }

    public static void ClearRegistryMangerInstance()
    {
      RegistryManager.sInstance = (RegistryManager) null;
    }

    private void Init(string oem = "bgp")
    {
      this.BaseKeyPath = "Software\\BlueStacks" + (oem.Equals("bgp", StringComparison.InvariantCultureIgnoreCase) ? "" : "_" + oem) + RegistryManager.UPGRADE_TAG;
      this.HostConfigKeyPath = this.BaseKeyPath + "\\Config";
      this.ClientBaseKeyPath = this.BaseKeyPath + "\\Client";
      this.BTVKeyPath = this.BaseKeyPath + "\\BTV";
      this.mBaseKey = this.InitKeyWithSecurityCheck(this.BaseKeyPath);
      this.mBTVKey = RegistryUtils.InitKey(this.BaseKeyPath + "\\BTV");
      this.mBTVFilterKey = RegistryUtils.InitKey(this.BaseKeyPath + "\\BTV\\Filters");
      this.mClientKey = RegistryUtils.InitKey(this.BaseKeyPath + "\\Client");
      this.mUserKey = RegistryUtils.InitKey(this.BaseKeyPath + "\\User");
      this.mHostConfigKey = RegistryUtils.InitKey(this.BaseKeyPath + "\\Config");
      this.mGuestsKey = RegistryUtils.InitKey(this.BaseKeyPath + "\\Guests");
      this.mMonitorsKey = RegistryUtils.InitKey(this.BaseKeyPath + "\\Monitors");
      if (this.mClientKey == null)
        this.mClientKey = !SystemUtils.IsOs64Bit() ? RegistryUtils.InitKey("Software\\BlueStacksGP") : RegistryUtils.InitKey("Software\\Wow6432Node\\BlueStacksGP");
      foreach (string vm in this.VmList)
        this.Guest[vm] = new InstanceRegistry(vm, oem);
    }

    public static void InitVmKeysForInstaller(List<string> vmList)
    {
      if (RegistryManager.sInstance == null)
      {
        RegistryManager.sInstance = new RegistryManager()
        {
          mIsAdmin = SystemUtils.IsAdministrator()
        };
        RegistryManager.sInstance.Init("bgp");
        if (RegistryManager.RegistryManagers.ContainsKey("bgp"))
          RegistryManager.RegistryManagers["bgp"] = RegistryManager.sInstance;
      }
      if (vmList == null)
        return;
      foreach (string vm in vmList)
      {
        if (!RegistryManager.sInstance.Guest.ContainsKey(vm))
          RegistryManager.sInstance.Guest[vm] = new InstanceRegistry(vm, "bgp");
      }
    }

    public void SetAccessPermissions()
    {
      RegistryUtils.GrantAllAccessPermission(this.mHostConfigKey);
      RegistryUtils.GrantAllAccessPermission(this.mUserKey);
      RegistryUtils.GrantAllAccessPermission(this.mBTVKey);
      RegistryUtils.GrantAllAccessPermission(this.mClientKey);
      RegistryUtils.GrantAllAccessPermission(this.mGuestsKey);
      RegistryUtils.GrantAllAccessPermission(this.mMonitorsKey);
    }

    public bool DeleteAndroidSubKey(string vmName)
    {
      try
      {
        string subkey = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}\\Guests\\{1}", (object) this.BaseKeyPath, (object) vmName);
        Registry.LocalMachine.DeleteSubKeyTree(subkey);
        this.Guest.Remove(vmName);
      }
      catch
      {
        return false;
      }
      return true;
    }

    public string[] VmList
    {
      get
      {
        return (string[]) this.mHostConfigKey.GetValue(nameof (VmList), (object) new string[1]
        {
          "Android"
        });
      }
      set
      {
        this.mHostConfigKey.SetValue(nameof (VmList), (object) value, RegistryValueKind.MultiString);
        this.mHostConfigKey.Flush();
      }
    }

    public string[] UpgradeVersionList
    {
      get
      {
        return (string[]) this.mHostConfigKey.GetValue(nameof (UpgradeVersionList), (object) new string[0]);
      }
      set
      {
        this.mHostConfigKey.SetValue(nameof (UpgradeVersionList), (object) value, RegistryValueKind.MultiString);
        this.mHostConfigKey.Flush();
      }
    }

    public bool IsShootingModeTooltipVisible
    {
      get
      {
        return (int) this.mHostConfigKey.GetValue(nameof (IsShootingModeTooltipVisible), (object) 1) != 0;
      }
      set
      {
        this.mHostConfigKey.SetValue(nameof (IsShootingModeTooltipVisible), (object) (!value ? 0 : 1));
        this.mHostConfigKey.Flush();
      }
    }

    public bool KeyMappingAvailablePromptEnabled
    {
      get
      {
        return (int) this.mClientKey.GetValue(nameof (KeyMappingAvailablePromptEnabled), (object) 1) != 0;
      }
      set
      {
        this.mClientKey.SetValue(nameof (KeyMappingAvailablePromptEnabled), (object) (!value ? 0 : 1));
        this.mClientKey.Flush();
      }
    }

    public bool ForceDedicatedGPU
    {
      get
      {
        return (int) this.mHostConfigKey.GetValue(nameof (ForceDedicatedGPU), (object) Strings.ForceDedicatedGPUDefaultValue) != 0;
      }
      set
      {
        this.mHostConfigKey.SetValue(nameof (ForceDedicatedGPU), (object) (!value ? 0 : 1));
        this.mHostConfigKey.Flush();
      }
    }

    public string AvailableGPUDetails
    {
      get
      {
        return (string) this.mHostConfigKey.GetValue(nameof (AvailableGPUDetails), (object) "");
      }
      set
      {
        this.mHostConfigKey.SetValue(nameof (AvailableGPUDetails), (object) value);
        this.mHostConfigKey.Flush();
      }
    }

    public bool OverlayAvailablePromptEnabled
    {
      get
      {
        return (int) this.mClientKey.GetValue(nameof (OverlayAvailablePromptEnabled), (object) 0) != 0;
      }
      set
      {
        this.mClientKey.SetValue(nameof (OverlayAvailablePromptEnabled), (object) (!value ? 0 : 1));
        this.mClientKey.Flush();
      }
    }

    public bool DisableImageDetection
    {
      get
      {
        return (int) this.mClientKey.GetValue(nameof (DisableImageDetection), (object) 0) != 0;
      }
      set
      {
        this.mClientKey.SetValue(nameof (DisableImageDetection), (object) (!value ? 0 : 1));
        this.mClientKey.Flush();
      }
    }

    public bool ShowKeyControlsOverlay
    {
      get
      {
        return (int) this.mClientKey.GetValue(nameof (ShowKeyControlsOverlay), (object) 0) != 0;
      }
      set
      {
        this.mClientKey.SetValue(nameof (ShowKeyControlsOverlay), (object) (!value ? 0 : 1));
        this.mClientKey.Flush();
      }
    }

    public bool TranslucentControlsEnabled
    {
      get
      {
        return (int) this.mClientKey.GetValue(nameof (TranslucentControlsEnabled), (object) 0) != 0;
      }
      set
      {
        this.mClientKey.SetValue(nameof (TranslucentControlsEnabled), (object) (!value ? 0 : 1));
        this.mClientKey.Flush();
      }
    }

    public double TranslucentControlsTransparency
    {
      get
      {
        return double.Parse((string) this.mClientKey.GetValue(nameof (TranslucentControlsTransparency), (object) 0.8.ToString((IFormatProvider) CultureInfo.InvariantCulture)), (IFormatProvider) CultureInfo.InvariantCulture);
      }
      set
      {
        this.mClientKey.SetValue(nameof (TranslucentControlsTransparency), (object) value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        this.mClientKey.Flush();
      }
    }

    public bool ShowGamingSummary
    {
      get
      {
        return (int) this.mClientKey.GetValue(nameof (ShowGamingSummary), (object) 1) == 1;
      }
      set
      {
        this.mClientKey.SetValue(nameof (ShowGamingSummary), (object) (value ? 1 : 0));
        this.mClientKey.Flush();
      }
    }

    public bool DiscordEnabled
    {
      get
      {
        return (int) this.mClientKey.GetValue(nameof (DiscordEnabled), (object) 1) == 1;
      }
      set
      {
        this.mClientKey.SetValue(nameof (DiscordEnabled), (object) (value ? 1 : 0));
        this.mClientKey.Flush();
      }
    }

    public bool CustomCursorEnabled
    {
      get
      {
        return (int) this.mClientKey.GetValue(nameof (CustomCursorEnabled), (object) 1) == 1;
      }
      set
      {
        this.mClientKey.SetValue(nameof (CustomCursorEnabled), (object) (value ? 1 : 0));
        this.mClientKey.Flush();
      }
    }

    public bool GamepadDetectionEnabled
    {
      get
      {
        return (int) this.mClientKey.GetValue(nameof (GamepadDetectionEnabled), (object) 1) == 1;
      }
      set
      {
        this.mClientKey.SetValue(nameof (GamepadDetectionEnabled), (object) (value ? 1 : 0));
        this.mClientKey.Flush();
      }
    }

    public List<string> IgnoreAutoPlayPackageList
    {
      get
      {
        return ((IEnumerable<string>) (string[]) this.mClientKey.GetValue("ShownVideoOnFirstLaunchPackageList", (object) new string[0])).ToList<string>();
      }
      set
      {
        this.mClientKey.SetValue("ShownVideoOnFirstLaunchPackageList", (object) value?.ToArray(), RegistryValueKind.MultiString);
        this.mClientKey.Flush();
      }
    }

    public bool UpdateBstConfig
    {
      get
      {
        return (int) this.mHostConfigKey.GetValue(nameof (UpdateBstConfig), (object) 1) != 0;
      }
      set
      {
        this.mHostConfigKey.SetValue(nameof (UpdateBstConfig), (object) (!value ? 0 : 1));
        this.mHostConfigKey.Flush();
      }
    }

    public bool IsEcoModeBlurbShown
    {
      get
      {
        return (int) this.mHostConfigKey.GetValue(nameof (IsEcoModeBlurbShown), (object) 0) != 0;
      }
      set
      {
        this.mHostConfigKey.SetValue(nameof (IsEcoModeBlurbShown), (object) (!value ? 0 : 1));
        this.mHostConfigKey.Flush();
      }
    }

    public bool IsGameTvEnabled
    {
      get
      {
        return (int) this.mClientKey.GetValue(nameof (IsGameTvEnabled), (object) 0) != 0;
      }
      set
      {
        this.mClientKey.SetValue(nameof (IsGameTvEnabled), (object) (!value ? 0 : 1));
        this.mClientKey.Flush();
      }
    }

    public int OnboardingBlurbShownCount
    {
      get
      {
        return (int) this.mClientKey.GetValue(nameof (OnboardingBlurbShownCount), (object) 0);
      }
      set
      {
        this.mClientKey.SetValue(nameof (OnboardingBlurbShownCount), (object) value);
        this.mClientKey.Flush();
      }
    }

    public int CommonFPS
    {
      get
      {
        return (int) this.mHostConfigKey.GetValue(nameof (CommonFPS), (object) 60);
      }
      set
      {
        this.mHostConfigKey.SetValue(nameof (CommonFPS), (object) value);
        this.mHostConfigKey.Flush();
      }
    }

    public int TrimMemoryDuration
    {
      get
      {
        return (int) this.mHostConfigKey.GetValue(nameof (TrimMemoryDuration), (object) 15);
      }
      set
      {
        this.mHostConfigKey.SetValue(nameof (TrimMemoryDuration), (object) value);
        this.mHostConfigKey.Flush();
      }
    }

    public int DevEnv
    {
      get
      {
        return (int) this.mHostConfigKey.GetValue(nameof (DevEnv), (object) 0);
      }
    }

    public int ArrangeWindowMode
    {
      get
      {
        return (int) this.mHostConfigKey.GetValue("ArrangeWindowModeConfig", (object) 0);
      }
      set
      {
        this.mHostConfigKey.SetValue("ArrangeWindowModeConfig", (object) value);
        this.mHostConfigKey.Flush();
      }
    }

    public long TileWindowColumnCount
    {
      get
      {
        return long.Parse(this.mHostConfigKey.GetValue(nameof (TileWindowColumnCount), (object) 2).ToString(), (IFormatProvider) CultureInfo.InvariantCulture);
      }
      set
      {
        this.mHostConfigKey.SetValue(nameof (TileWindowColumnCount), (object) value);
        this.mHostConfigKey.Flush();
      }
    }

    public bool ManageGooglePlayPromptEnabled
    {
      get
      {
        return (int) this.mClientKey.GetValue(nameof (ManageGooglePlayPromptEnabled), (object) 1) != 0;
      }
      set
      {
        this.mClientKey.SetValue(nameof (ManageGooglePlayPromptEnabled), (object) (!value ? 0 : 1));
        this.mClientKey.Flush();
      }
    }

    public bool UseEscapeToExitFullScreen
    {
      get
      {
        return (int) this.mClientKey.GetValue(nameof (UseEscapeToExitFullScreen), (object) 0) == 1;
      }
      set
      {
        this.mClientKey.SetValue(nameof (UseEscapeToExitFullScreen), (object) (value ? 1 : 0));
        this.mClientKey.Flush();
      }
    }

    public bool IsVTXPopupEnable
    {
      get
      {
        return (int) this.mClientKey.GetValue(nameof (IsVTXPopupEnable), (object) 1) == 1;
      }
      set
      {
        this.mClientKey.SetValue(nameof (IsVTXPopupEnable), (object) (value ? 1 : 0));
        this.mClientKey.Flush();
      }
    }

    public int FrontendHeight
    {
      get
      {
        return (int) this.mClientKey.GetValue(nameof (FrontendHeight), (object) 0);
      }
      set
      {
        this.mClientKey.SetValue(nameof (FrontendHeight), (object) value);
        this.mClientKey.Flush();
      }
    }

    public int FrontendWidth
    {
      get
      {
        return (int) this.mClientKey.GetValue(nameof (FrontendWidth), (object) 0);
      }
      set
      {
        this.mClientKey.SetValue(nameof (FrontendWidth), (object) value);
        this.mClientKey.Flush();
      }
    }

    public string BossKey
    {
      get
      {
        return (string) this.mClientKey.GetValue(nameof (BossKey), (object) "");
      }
      set
      {
        this.mClientKey.SetValue(nameof (BossKey), (object) value);
      }
    }

    public string CampaignMD5
    {
      get
      {
        return (string) this.mClientKey.GetValue(nameof (CampaignMD5), (object) "");
      }
      set
      {
        this.mClientKey.SetValue(nameof (CampaignMD5), (object) value);
        this.mClientKey.SetValue("FLECampaignMD5", (object) value);
        this.mClientKey.Flush();
      }
    }

    public string FLECampaignMD5
    {
      get
      {
        return (string) this.mClientKey.GetValue(nameof (FLECampaignMD5), (object) string.Empty);
      }
    }

    public string CampaignJson
    {
      get
      {
        return (string) this.mClientKey.GetValue(nameof (CampaignJson), (object) "");
      }
      set
      {
        this.mClientKey.SetValue(nameof (CampaignJson), (object) value);
        if (!string.IsNullOrEmpty(value))
          this.DeleteFLECampaignMD5();
        this.mClientKey.Flush();
      }
    }

    public void DeleteFLECampaignMD5()
    {
      if (this.mClientKey.GetValue("FLECampaignMD5", (object) null) == null)
        return;
      this.mClientKey.DeleteValue("FLECampaignMD5");
    }

    public string CDNAppsTimeStamp
    {
      get
      {
        return (string) this.mClientKey.GetValue(nameof (CDNAppsTimeStamp), (object) "");
      }
      set
      {
        this.mClientKey.SetValue(nameof (CDNAppsTimeStamp), (object) value);
        this.mClientKey.Flush();
      }
    }

    public string SetupFolder
    {
      get
      {
        return (string) this.mClientKey.GetValue(nameof (SetupFolder), (object) Strings.BlueStacksSetupFolder);
      }
      set
      {
        this.mClientKey.SetValue(nameof (SetupFolder), (object) value);
      }
    }

    public string EngineDataDir
    {
      get
      {
        return (string) this.mClientKey.GetValue(nameof (EngineDataDir), (object) "");
      }
      set
      {
        this.mClientKey.SetValue(nameof (EngineDataDir), (object) value);
      }
    }

    public string ClientInstallDir
    {
      get
      {
        return (string) this.mClientKey.GetValue(nameof (ClientInstallDir), (object) "");
      }
      set
      {
        this.mClientKey.SetValue(nameof (ClientInstallDir), (object) value);
      }
    }

    public static string ClientThemeName { get; set; } = "Assets";

    public bool OpenThemeEditor
    {
      get
      {
        return (int) this.mClientKey.GetValue(nameof (OpenThemeEditor), (object) 0) == 1;
      }
      set
      {
        this.mClientKey.SetValue(nameof (OpenThemeEditor), (object) (value ? 1 : 0));
        this.mClientKey.Flush();
      }
    }

    public string CefDataPath
    {
      get
      {
        return (string) this.mClientKey.GetValue(nameof (CefDataPath), (object) string.Empty);
      }
      set
      {
        this.mClientKey.SetValue(nameof (CefDataPath), (object) value);
        this.mClientKey.Flush();
      }
    }

    public string OfflineHtmlHomeUrl
    {
      get
      {
        string str = string.Empty;
        if (File.Exists(Path.Combine(this.ClientInstallDir, "OfflineHtmlHome\\offline.html")))
          str = Path.Combine(this.ClientInstallDir, "OfflineHtmlHome\\offline.html");
        return string.IsNullOrEmpty((string) this.mClientKey.GetValue(nameof (OfflineHtmlHomeUrl), (object) string.Empty)) ? str : (string) this.mClientKey.GetValue(nameof (OfflineHtmlHomeUrl), (object) string.Empty);
      }
      set
      {
        this.mClientKey.SetValue(nameof (OfflineHtmlHomeUrl), (object) value);
        this.mClientKey.Flush();
      }
    }

    public bool HomeHtmlErrorHandling
    {
      get
      {
        return (int) this.mClientKey.GetValue(nameof (HomeHtmlErrorHandling), (object) 1) == 1;
      }
      set
      {
        this.mClientKey.SetValue(nameof (HomeHtmlErrorHandling), (object) (!value ? 0 : 1));
        this.mClientKey.Flush();
      }
    }

    public int CefDevEnv
    {
      get
      {
        return (int) this.mClientKey.GetValue(nameof (CefDevEnv), (object) 0);
      }
    }

    public int CefDebugPort
    {
      get
      {
        return (int) this.mClientKey.GetValue(nameof (CefDebugPort), (object) 0);
      }
    }

    public int LastBootTime
    {
      get
      {
        return (int) this.mClientKey.GetValue(nameof (LastBootTime), (object) 120000);
      }
      set
      {
        this.mClientKey.SetValue(nameof (LastBootTime), (object) value);
      }
    }

    public int AvgBootTime
    {
      get
      {
        return (int) this.mClientKey.GetValue(nameof (AvgBootTime), (object) 20000);
      }
      set
      {
        this.mClientKey.SetValue(nameof (AvgBootTime), (object) value);
        this.mClientKey.Flush();
      }
    }

    public int NoOfBootCompleted
    {
      get
      {
        return (int) this.mClientKey.GetValue(nameof (NoOfBootCompleted), (object) 0);
      }
      set
      {
        this.mClientKey.SetValue(nameof (NoOfBootCompleted), (object) value);
        this.mClientKey.Flush();
      }
    }

    public int AvgHomeHtmlLoadTime
    {
      get
      {
        return (int) this.mClientKey.GetValue(nameof (AvgHomeHtmlLoadTime), (object) 10000);
      }
      set
      {
        this.mClientKey.SetValue(nameof (AvgHomeHtmlLoadTime), (object) value);
        this.mClientKey.Flush();
      }
    }

    public bool IsScreenshotsLocationPopupEnabled
    {
      get
      {
        return (int) this.mClientKey.GetValue("ScreenshotsLocationPopupEnabled", (object) 1) == 1;
      }
      set
      {
        this.mClientKey.SetValue("ScreenshotsLocationPopupEnabled", (object) (!value ? 0 : 1));
      }
    }

    public string ScreenShotsPath
    {
      get
      {
        return (string) this.mClientKey.GetValue(nameof (ScreenShotsPath), (object) RegistryStrings.ScreenshotDefaultPath);
      }
      set
      {
        this.mClientKey.SetValue(nameof (ScreenShotsPath), (object) value);
        this.mClientKey.Flush();
      }
    }

    public bool RequirementConfigUpdateRequired
    {
      get
      {
        return (int) this.mClientKey.GetValue(nameof (RequirementConfigUpdateRequired), (object) 0) == 1;
      }
      set
      {
        this.mClientKey.SetValue(nameof (RequirementConfigUpdateRequired), (object) (!value ? 0 : 1));
        this.mClientKey.Flush();
      }
    }

    public bool IsShowIconBorder
    {
      get
      {
        return (int) this.mClientKey.GetValue("ShowIconBorder", (object) 0) == 1;
      }
    }

    public string UserSelectedLocale
    {
      get
      {
        return (string) this.mClientKey.GetValue(nameof (UserSelectedLocale), (object) "");
      }
      set
      {
        if (string.IsNullOrEmpty(value))
          return;
        this.mClientKey.SetValue(nameof (UserSelectedLocale), (object) value);
        this.mClientKey.Flush();
      }
    }

    public string TargetLocale
    {
      get
      {
        return (string) this.mClientKey.GetValue(nameof (TargetLocale), (object) "");
      }
    }

    public string TargetLocaleUrl
    {
      get
      {
        return (string) this.mClientKey.GetValue(nameof (TargetLocaleUrl), (object) "");
      }
    }

    public string FailedUpgradeVersion
    {
      get
      {
        return (string) this.mClientKey.GetValue(nameof (FailedUpgradeVersion), (object) "");
      }
      set
      {
        this.mClientKey.SetValue(nameof (FailedUpgradeVersion), (object) value);
        this.mClientKey.Flush();
      }
    }

    public string LastUpdateSkippedVersion
    {
      get
      {
        return (string) this.mClientKey.GetValue(nameof (LastUpdateSkippedVersion), (object) "");
      }
      set
      {
        this.mClientKey.SetValue(nameof (LastUpdateSkippedVersion), (object) value);
        this.mClientKey.Flush();
      }
    }

    public string Partner
    {
      get
      {
        return (string) this.mClientKey.GetValue(nameof (Partner), (object) "");
      }
      set
      {
        this.mClientKey.SetValue(nameof (Partner), (object) value);
        this.mClientKey.Flush();
      }
    }

    public string DownloadedUpdateFile
    {
      get
      {
        return (string) this.mClientKey.GetValue(nameof (DownloadedUpdateFile), (object) "");
      }
      set
      {
        this.mClientKey.SetValue(nameof (DownloadedUpdateFile), (object) value);
        this.mClientKey.Flush();
      }
    }

    public string ClientVersion
    {
      get
      {
        string str = (string) this.mBaseKey.GetValue(nameof (ClientVersion), (object) "");
        if (string.IsNullOrEmpty(str))
          str = (string) this.mClientKey.GetValue(nameof (ClientVersion), (object) "");
        return str;
      }
      set
      {
        this.mBaseKey.SetValue(nameof (ClientVersion), (object) value);
        this.mBaseKey.Flush();
      }
    }

    public int IsClientFirstLaunch
    {
      get
      {
        return (int) this.mClientKey.GetValue(nameof (IsClientFirstLaunch), (object) 1);
      }
      set
      {
        this.mClientKey.SetValue(nameof (IsClientFirstLaunch), (object) value);
        this.mClientKey.Flush();
      }
    }

    public int IsEngineUpgraded
    {
      get
      {
        return (int) this.mClientKey.GetValue(nameof (IsEngineUpgraded), (object) 0);
      }
      set
      {
        this.mClientKey.SetValue(nameof (IsEngineUpgraded), (object) value);
        this.mClientKey.Flush();
      }
    }

    public bool IsShowRibbonNotification
    {
      get
      {
        return (int) this.mClientKey.GetValue(nameof (IsShowRibbonNotification), (object) 1) == 1;
      }
      set
      {
        this.mClientKey.SetValue(nameof (IsShowRibbonNotification), (object) (value ? 1 : 0));
        this.mClientKey.Flush();
      }
    }

    public bool IsShowToastNotification
    {
      get
      {
        return (int) this.mClientKey.GetValue(nameof (IsShowToastNotification), (object) 1) == 1;
      }
      set
      {
        this.mClientKey.SetValue(nameof (IsShowToastNotification), (object) (value ? 1 : 0));
        this.mClientKey.Flush();
      }
    }

    public bool IsShowGamepadDesktopNotification
    {
      get
      {
        return (int) this.mClientKey.GetValue(nameof (IsShowGamepadDesktopNotification), (object) 1) == 1;
      }
      set
      {
        this.mClientKey.SetValue(nameof (IsShowGamepadDesktopNotification), (object) (value ? 1 : 0));
        this.mClientKey.Flush();
      }
    }

    public bool IsShowPromotionalTeaser
    {
      get
      {
        return (int) this.mClientKey.GetValue(nameof (IsShowPromotionalTeaser), (object) 1) == 1;
      }
      set
      {
        this.mClientKey.SetValue(nameof (IsShowPromotionalTeaser), (object) (value ? 1 : 0));
        this.mClientKey.Flush();
      }
    }

    public bool IsClientUpgraded
    {
      get
      {
        return (int) this.mClientKey.GetValue(nameof (IsClientUpgraded), (object) 0) == 1;
      }
      set
      {
        this.mClientKey.SetValue(nameof (IsClientUpgraded), (object) (value ? 1 : 0));
        this.mClientKey.Flush();
      }
    }

    public string AInfo
    {
      get
      {
        return (string) this.mClientKey.GetValue(nameof (AInfo), (object) "");
      }
      set
      {
        this.mClientKey.SetValue(nameof (AInfo), (object) value);
        this.mClientKey.Flush();
      }
    }

    public string BGPDevUrl
    {
      get
      {
        return (string) this.mClientKey.GetValue(nameof (BGPDevUrl), (object) "");
      }
      set
      {
        this.mClientKey.SetValue(nameof (BGPDevUrl), (object) value);
        this.mClientKey.Flush();
      }
    }

    public string FriendsDevServer
    {
      get
      {
        return (string) this.mClientKey.GetValue(nameof (FriendsDevServer), (object) "");
      }
    }

    public string PromotionId
    {
      get
      {
        return (string) this.mClientKey.GetValue(nameof (PromotionId), (object) "");
      }
      set
      {
        this.mClientKey.SetValue(nameof (PromotionId), (object) value);
        this.mClientKey.Flush();
      }
    }

    public string DMMRecommendedWindowUrl
    {
      get
      {
        return (string) this.mClientKey.GetValue("RecommendedWindowUrl", (object) "http://site-gameplayer.dmm.com/emulator-recommend");
      }
      set
      {
        this.mClientKey.SetValue("RecommendedWindowUrl", (object) value);
        this.mClientKey.Flush();
      }
    }

    public string DeviceProfileFromCloud
    {
      get
      {
        return (string) this.mClientKey.GetValue(nameof (DeviceProfileFromCloud), (object) string.Empty);
      }
      set
      {
        this.mClientKey.SetValue(nameof (DeviceProfileFromCloud), (object) value);
        this.mClientKey.Flush();
      }
    }

    public int GlPlusTransportConfig
    {
      get
      {
        return (int) this.mHostConfigKey.GetValue(nameof (GlPlusTransportConfig), (object) 3);
      }
      set
      {
        this.mHostConfigKey.SetValue(nameof (GlPlusTransportConfig), (object) value);
        this.mHostConfigKey.Flush();
      }
    }

    public int GlLegacyTransportConfig
    {
      get
      {
        return (int) this.mHostConfigKey.GetValue(nameof (GlLegacyTransportConfig), (object) 0);
      }
      set
      {
        this.mHostConfigKey.SetValue(nameof (GlLegacyTransportConfig), (object) value);
        this.mHostConfigKey.Flush();
      }
    }

    public string CurrentEngine
    {
      get
      {
        if (string.IsNullOrEmpty(this.sCurrentEngine))
          this.sCurrentEngine = (string) this.mHostConfigKey.GetValue(nameof (CurrentEngine), (object) "plus");
        return this.sCurrentEngine;
      }
      set
      {
        this.sCurrentEngine = value;
        this.mHostConfigKey.SetValue(nameof (CurrentEngine), (object) value);
        this.mHostConfigKey.Flush();
      }
    }

    public string EnginePreference
    {
      get
      {
        return (string) this.mHostConfigKey.GetValue(nameof (EnginePreference), (object) "plus");
      }
      set
      {
        this.mHostConfigKey.SetValue(nameof (EnginePreference), (object) value);
        this.mHostConfigKey.Flush();
      }
    }

    public string InstallDir
    {
      get
      {
        return (string) this.mBaseKey.GetValue(nameof (InstallDir), (object) "");
      }
      set
      {
        this.mBaseKey.SetValue(nameof (InstallDir), (object) value);
        this.mBaseKey.Flush();
      }
    }

    public bool IsUpgrade
    {
      get
      {
        return (int) this.mBaseKey.GetValue(nameof (IsUpgrade), (object) 0) == 1;
      }
      set
      {
        this.mBaseKey.SetValue(nameof (IsUpgrade), (object) (value ? 1 : 0));
        this.mBaseKey.Flush();
      }
    }

    public string DataDir
    {
      get
      {
        return (string) this.mBaseKey.GetValue(nameof (DataDir), (object) "");
      }
      set
      {
        this.mBaseKey.SetValue(nameof (DataDir), (object) value);
        this.mBaseKey.Flush();
      }
    }

    public string UserDefinedDir
    {
      get
      {
        if (this.sUserDefinedDir == null)
          this.sUserDefinedDir = (string) this.mBaseKey.GetValue(nameof (UserDefinedDir), (object) Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData));
        return this.sUserDefinedDir;
      }
      set
      {
        this.sUserDefinedDir = value;
        this.mBaseKey.SetValue(nameof (UserDefinedDir), (object) value);
        this.mBaseKey.Flush();
      }
    }

    public string LogDir
    {
      get
      {
        return (string) this.mBaseKey.GetValue(nameof (LogDir), (object) null) ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Bluestacks\\Logs");
      }
      set
      {
        this.mBaseKey.SetValue(nameof (LogDir), (object) value);
        this.mBaseKey.Flush();
      }
    }

    public int PlusDebug
    {
      get
      {
        return (int) this.mBaseKey.GetValue(nameof (PlusDebug), (object) 0);
      }
      set
      {
        this.mBaseKey.SetValue(nameof (PlusDebug), (object) value);
        this.mBaseKey.Flush();
      }
    }

    public string Version
    {
      get
      {
        if (this.sVersion != null)
          return this.sVersion;
        this.sVersion = (string) this.mBaseKey.GetValue(nameof (Version), (object) "");
        return this.sVersion;
      }
      set
      {
        this.mBaseKey.SetValue(nameof (Version), (object) value);
        this.mBaseKey.Flush();
        this.sVersion = value;
      }
    }

    public string UserGuid
    {
      get
      {
        return (string) this.mBaseKey.GetValue("USER_GUID", (object) "");
      }
      set
      {
        this.mBaseKey.SetValue("USER_GUID", (object) value);
        this.mBaseKey.Flush();
      }
    }

    public string WebAppVersion
    {
      get
      {
        return (string) this.mClientKey.GetValue(nameof (WebAppVersion), (object) string.Empty);
      }
      set
      {
        this.mClientKey.SetValue(nameof (WebAppVersion), (object) value);
        this.mClientKey.Flush();
      }
    }

    public string InstanceSortOption
    {
      get
      {
        return (string) this.mClientKey.GetValue(nameof (InstanceSortOption), (object) string.Empty);
      }
      set
      {
        this.mClientKey.SetValue(nameof (InstanceSortOption), (object) value);
        this.mClientKey.Flush();
      }
    }

    public string OpenExternalLink
    {
      get
      {
        return (string) this.mClientKey.GetValue(nameof (OpenExternalLink), (object) string.Empty);
      }
      set
      {
        this.mClientKey.SetValue(nameof (OpenExternalLink), (object) value);
        this.mClientKey.Flush();
      }
    }

    public string ClientLaunchParams
    {
      get
      {
        return (string) this.mClientKey.GetValue(nameof (ClientLaunchParams), (object) "");
      }
      set
      {
        this.mClientKey.SetValue(nameof (ClientLaunchParams), (object) value);
        this.mClientKey.Flush();
      }
    }

    public string ApiToken
    {
      get
      {
        return (string) this.mBaseKey.GetValue(nameof (ApiToken), (object) "");
      }
      set
      {
        this.mBaseKey.SetValue(nameof (ApiToken), (object) value);
        this.mBaseKey.Flush();
      }
    }

    public bool IsBTVCheckedAfterUpdate
    {
      get
      {
        return (int) this.mHostConfigKey.GetValue(nameof (IsBTVCheckedAfterUpdate), (object) 0) == 1;
      }
      set
      {
        this.mHostConfigKey.SetValue(nameof (IsBTVCheckedAfterUpdate), (object) (value ? 1 : 0));
        this.mHostConfigKey.Flush();
      }
    }

    public string CurrentBtvVersionInstalled
    {
      get
      {
        return (string) this.mHostConfigKey.GetValue(nameof (CurrentBtvVersionInstalled), (object) "");
      }
      set
      {
        this.mHostConfigKey.SetValue(nameof (CurrentBtvVersionInstalled), (object) value);
        this.mHostConfigKey.Flush();
      }
    }

    public bool IsFirstTimeCheck
    {
      get
      {
        int num = (int) this.mHostConfigKey.GetValue(nameof (IsFirstTimeCheck), (object) 1) == 1 ? 1 : 0;
        this.mHostConfigKey.SetValue(nameof (IsFirstTimeCheck), (object) 0);
        this.mHostConfigKey.Flush();
        return num != 0;
      }
    }

    public int SystemInfoStats2
    {
      get
      {
        return (int) this.mHostConfigKey.GetValue(nameof (SystemInfoStats2), (object) 0);
      }
      set
      {
        this.mHostConfigKey.SetValue(nameof (SystemInfoStats2), (object) value);
        this.mHostConfigKey.Flush();
      }
    }

    public int Features
    {
      get
      {
        return (int) this.mHostConfigKey.GetValue(nameof (Features), (object) 0);
      }
      set
      {
        this.mHostConfigKey.SetValue(nameof (Features), (object) value);
        this.mHostConfigKey.Flush();
      }
    }

    public int FeaturesHigh
    {
      get
      {
        return (int) this.mHostConfigKey.GetValue(nameof (FeaturesHigh), (object) 0);
      }
      set
      {
        this.mHostConfigKey.SetValue(nameof (FeaturesHigh), (object) value);
        this.mHostConfigKey.Flush();
      }
    }

    public int SystemStats
    {
      get
      {
        return (int) this.mHostConfigKey.GetValue(nameof (SystemStats), (object) 0);
      }
      set
      {
        this.mHostConfigKey.SetValue(nameof (SystemStats), (object) value);
        this.mHostConfigKey.Flush();
      }
    }

    public int SendBotsCheckStats
    {
      get
      {
        return (int) this.mHostConfigKey.GetValue(nameof (SendBotsCheckStats), (object) 0);
      }
      set
      {
        this.mHostConfigKey.SetValue(nameof (SendBotsCheckStats), (object) 1);
      }
    }

    public string BotsCheckStatsTime
    {
      get
      {
        return (string) this.mHostConfigKey.GetValue(nameof (BotsCheckStatsTime), (object) "");
      }
      set
      {
        this.mHostConfigKey.SetValue(nameof (BotsCheckStatsTime), (object) value);
      }
    }

    public string Host
    {
      get
      {
        if (string.IsNullOrEmpty(this.mHost))
        {
          string str = (string) this.mHostConfigKey.GetValue(nameof (Host), (object) "https://cloud.bluestacks.com");
          if (string.IsNullOrEmpty(str))
            str = "https://cloud.bluestacks.com";
          this.mHost = str;
        }
        return this.mHost;
      }
      set
      {
        this.mHostConfigKey.SetValue(nameof (Host), (object) value);
        this.mHostConfigKey.Flush();
        this.mHost = value;
      }
    }

    public string Host2
    {
      get
      {
        return (string) this.mHostConfigKey.GetValue(nameof (Host2), (object) "https://23.23.194.123");
      }
      set
      {
        this.mHostConfigKey.SetValue(nameof (Host2), (object) value);
        this.mHostConfigKey.Flush();
      }
    }

    public string RedDotShownOnIcon
    {
      get
      {
        return (string) this.mHostConfigKey.GetValue(nameof (RedDotShownOnIcon), (object) "");
      }
      set
      {
        this.mHostConfigKey.SetValue(nameof (RedDotShownOnIcon), (object) value);
        this.mHostConfigKey.Flush();
      }
    }

    public string TwitchServerPath
    {
      get
      {
        return (string) this.mBTVKey.GetValue(nameof (TwitchServerPath), (object) "");
      }
      set
      {
        this.mBTVKey.SetValue(nameof (TwitchServerPath), (object) value);
        this.mBTVKey.Flush();
      }
    }

    public int CLRBrowserServerPort
    {
      get
      {
        return (int) this.mBTVKey.GetValue(nameof (CLRBrowserServerPort), (object) 2911);
      }
      set
      {
        this.mBTVKey.SetValue(nameof (CLRBrowserServerPort), (object) value);
        this.mBTVKey.Flush();
      }
    }

    public string BtvDevServer
    {
      get
      {
        return (string) this.mBTVKey.GetValue(nameof (BtvDevServer), (object) "");
      }
      set
      {
        this.mBTVKey.SetValue(nameof (BtvDevServer), (object) value);
        this.mBTVKey.Flush();
      }
    }

    public string BtvNetwork
    {
      get
      {
        return (string) this.mBTVKey.GetValue("Network", (object) "");
      }
      set
      {
        this.mBTVKey.SetValue("Network", (object) value);
        this.mBTVKey.Flush();
      }
    }

    public int StreamingResolution
    {
      get
      {
        return (int) this.mBTVKey.GetValue(nameof (StreamingResolution), (object) 0);
      }
      set
      {
        this.mBTVKey.SetValue(nameof (StreamingResolution), (object) value);
        this.mBTVKey.Flush();
      }
    }

    public string SelectedCam
    {
      get
      {
        return (string) this.mBTVKey.GetValue(nameof (SelectedCam), (object) string.Empty);
      }
      set
      {
        this.mBTVKey.SetValue(nameof (SelectedCam), (object) value);
        this.mBTVKey.Flush();
      }
    }

    public int ReplayBufferEnabled
    {
      get
      {
        return (int) this.mBTVKey.GetValue(nameof (ReplayBufferEnabled), (object) 0);
      }
      set
      {
        this.mBTVKey.SetValue(nameof (ReplayBufferEnabled), (object) value);
        this.mBTVKey.Flush();
      }
    }

    public int BTVServerPort
    {
      get
      {
        return (int) this.mBTVKey.GetValue(nameof (BTVServerPort), (object) 2885);
      }
      set
      {
        this.mBTVKey.SetValue(nameof (BTVServerPort), (object) value);
        this.mBTVKey.Flush();
      }
    }

    public int AppViewLayout
    {
      get
      {
        return (int) this.mBTVKey.GetValue(nameof (AppViewLayout), (object) 0);
      }
      set
      {
        this.mBTVKey.SetValue(nameof (AppViewLayout), (object) value);
        this.mBTVKey.Flush();
      }
    }

    public string FilterUrl
    {
      get
      {
        return (string) this.mBTVKey.GetValue(nameof (FilterUrl), (object) "");
      }
    }

    public string LayoutUrl
    {
      get
      {
        return (string) this.mBTVKey.GetValue(nameof (LayoutUrl), (object) "");
      }
    }

    public string LayoutTheme
    {
      get
      {
        return (string) this.mBTVKey.GetValue(nameof (LayoutTheme), (object) "");
      }
      set
      {
        this.mBTVKey.SetValue(nameof (LayoutTheme), (object) value);
        this.mBTVKey.Flush();
      }
    }

    public string LastCameraLayoutTheme
    {
      get
      {
        return (string) this.mBTVKey.GetValue(nameof (LastCameraLayoutTheme), (object) "");
      }
      set
      {
        this.mBTVKey.SetValue(nameof (LastCameraLayoutTheme), (object) value);
        this.mBTVKey.Flush();
      }
    }

    public int ScreenWidth
    {
      get
      {
        return (int) this.mBTVKey.GetValue(nameof (ScreenWidth), (object) 0);
      }
      set
      {
        this.mBTVKey.SetValue(nameof (ScreenWidth), (object) value);
        this.mBTVKey.Flush();
      }
    }

    public int ScreenHeight
    {
      get
      {
        return (int) this.mBTVKey.GetValue(nameof (ScreenHeight), (object) 0);
      }
      set
      {
        this.mBTVKey.SetValue(nameof (ScreenHeight), (object) value);
        this.mBTVKey.Flush();
      }
    }

    public bool IsImeDebuggingEnabled
    {
      get
      {
        return (int) this.mHostConfigKey.GetValue(nameof (IsImeDebuggingEnabled), (object) 0) == 1;
      }
    }

    public int OBSServerPort
    {
      get
      {
        return (int) this.mBTVKey.GetValue(nameof (OBSServerPort), (object) 2851);
      }
      set
      {
        this.mBTVKey.SetValue(nameof (OBSServerPort), (object) value);
        this.mBTVKey.Flush();
      }
    }

    public bool IsGameCaptureSupportedInMachine
    {
      get
      {
        return (int) this.mBTVKey.GetValue(nameof (IsGameCaptureSupportedInMachine), (object) 1) != 0;
      }
      set
      {
        this.mBTVKey.SetValue(nameof (IsGameCaptureSupportedInMachine), (object) (!value ? 0 : 1));
        this.mBTVKey.Flush();
      }
    }

    public string StreamName
    {
      get
      {
        return (string) this.mBTVKey.GetValue(nameof (StreamName), (object) "");
      }
      set
      {
        this.mBTVKey.SetValue(nameof (StreamName), (object) value);
        this.mBTVKey.Flush();
      }
    }

    public string ServerLocation
    {
      get
      {
        return (string) this.mBTVKey.GetValue(nameof (ServerLocation), (object) "");
      }
      set
      {
        this.mBTVKey.SetValue(nameof (ServerLocation), (object) value);
        this.mBTVKey.Flush();
      }
    }

    public string ChannelName
    {
      get
      {
        return (string) this.mBTVFilterKey.GetValue(nameof (ChannelName), (object) "");
      }
      set
      {
        this.mBTVFilterKey.SetValue(nameof (ChannelName), (object) value);
        this.mBTVFilterKey.Flush();
      }
    }

    public string NotificationData
    {
      get
      {
        return (string) this.mHostConfigKey.GetValue(nameof (NotificationData), (object) string.Empty);
      }
      set
      {
        this.mHostConfigKey.SetValue(nameof (NotificationData), (object) value);
        this.mHostConfigKey.Flush();
      }
    }

    public string DeviceCaps
    {
      get
      {
        return (string) this.mHostConfigKey.GetValue(nameof (DeviceCaps), (object) "");
      }
      set
      {
        this.mHostConfigKey.SetValue(nameof (DeviceCaps), (object) value);
        this.mHostConfigKey.Flush();
      }
    }

    public int AgentServerPort
    {
      get
      {
        return (int) this.mHostConfigKey.GetValue(nameof (AgentServerPort), (object) 2861);
      }
      set
      {
        this.mHostConfigKey.SetValue(nameof (AgentServerPort), (object) value);
        this.mHostConfigKey.Flush();
      }
    }

    public int MultiInstanceServerPort
    {
      get
      {
        return (int) this.mHostConfigKey.GetValue(nameof (MultiInstanceServerPort), (object) 2961);
      }
      set
      {
        this.mHostConfigKey.SetValue(nameof (MultiInstanceServerPort), (object) value);
        this.mHostConfigKey.Flush();
      }
    }

    public string Oem
    {
      get
      {
        return (string) this.mHostConfigKey.GetValue(nameof (Oem), (object) "gamemanager");
      }
      set
      {
        this.mHostConfigKey.SetValue(nameof (Oem), (object) value);
        this.mHostConfigKey.Flush();
      }
    }

    public int BatchInstanceStartInterval
    {
      get
      {
        return (int) this.mHostConfigKey.GetValue(nameof (BatchInstanceStartInterval), (object) 2);
      }
      set
      {
        this.mHostConfigKey.SetValue(nameof (BatchInstanceStartInterval), (object) value);
        this.mHostConfigKey.Flush();
      }
    }

    public string CampaignName
    {
      get
      {
        return (string) this.mHostConfigKey.GetValue(nameof (CampaignName), (object) "");
      }
      set
      {
        this.mHostConfigKey.SetValue(nameof (CampaignName), (object) value);
        this.mHostConfigKey.Flush();
      }
    }

    public string PartnerExePath
    {
      get
      {
        return (string) this.mHostConfigKey.GetValue(nameof (PartnerExePath), (object) "");
      }
      set
      {
        this.mHostConfigKey.SetValue(nameof (PartnerExePath), (object) value);
        this.mHostConfigKey.Flush();
      }
    }

    public int CamStatus
    {
      get
      {
        return (int) this.mHostConfigKey.GetValue(nameof (CamStatus), (object) 0);
      }
      set
      {
        this.mHostConfigKey.SetValue(nameof (CamStatus), (object) value);
        this.mHostConfigKey.Flush();
      }
    }

    public int PartnerServerPort
    {
      get
      {
        return (int) this.mHostConfigKey.GetValue(nameof (PartnerServerPort), (object) 2871);
      }
      set
      {
        this.mHostConfigKey.SetValue(nameof (PartnerServerPort), (object) value);
        this.mHostConfigKey.Flush();
      }
    }

    public string RegisteredEmail
    {
      get
      {
        return (string) this.mUserKey.GetValue(nameof (RegisteredEmail), (object) "");
      }
      set
      {
        this.mUserKey.SetValue(nameof (RegisteredEmail), (object) value);
        this.mUserKey.Flush();
      }
    }

    public bool IsTimelineStats4Enabled
    {
      get
      {
        return (int) this.mHostConfigKey.GetValue(nameof (IsTimelineStats4Enabled), (object) 0) != 0;
      }
      set
      {
        this.mHostConfigKey.SetValue(nameof (IsTimelineStats4Enabled), (object) (!value ? 0 : 1));
        this.mHostConfigKey.Flush();
      }
    }

    public bool EnableAutomation
    {
      get
      {
        return (int) this.mHostConfigKey.GetValue(nameof (EnableAutomation), (object) 0) != 0;
      }
      set
      {
        this.mHostConfigKey.SetValue(nameof (EnableAutomation), (object) (!value ? 0 : 1));
        this.mHostConfigKey.Flush();
      }
    }

    public string PikaWorldId
    {
      get
      {
        return (string) this.mUserKey.GetValue(nameof (PikaWorldId), (object) "");
      }
      set
      {
        this.mUserKey.SetValue(nameof (PikaWorldId), (object) value);
        this.mUserKey.Flush();
      }
    }

    public string Token
    {
      get
      {
        return (string) this.mUserKey.GetValue(nameof (Token), (object) "");
      }
      set
      {
        this.mUserKey.SetValue(nameof (Token), (object) value);
        this.mUserKey.Flush();
      }
    }

    public bool IsPremium
    {
      get
      {
        return (int) this.mUserKey.GetValue(nameof (IsPremium), (object) 0) == 1 || true;
      }
      set
      {
        this.mUserKey.SetValue(nameof (IsPremium), (object) (value ? 1 : 0));
        this.mUserKey.Flush();
      }
    }

    public bool AddDesktopShortcuts
    {
      get
      {
        return (int) this.mHostConfigKey.GetValue(nameof (AddDesktopShortcuts), (object) 1) != 0;
      }
      set
      {
        this.mHostConfigKey.SetValue(nameof (AddDesktopShortcuts), (object) (!value ? 0 : 1));
        this.mHostConfigKey.Flush();
      }
    }

    public bool SwitchToAndroidHome
    {
      get
      {
        return (int) this.mHostConfigKey.GetValue(nameof (SwitchToAndroidHome), (object) 0) != 0;
      }
      set
      {
        this.mHostConfigKey.SetValue(nameof (SwitchToAndroidHome), (object) (!value ? 0 : 1));
        this.mHostConfigKey.Flush();
      }
    }

    public bool SwitchKillWebTab
    {
      get
      {
        return (int) this.mClientKey.GetValue(nameof (SwitchKillWebTab), (object) 1) != 0;
      }
      set
      {
        this.mClientKey.SetValue(nameof (SwitchKillWebTab), (object) (!value ? 0 : 1));
        this.mClientKey.Flush();
      }
    }

    public bool EnableMemoryTrim
    {
      get
      {
        return (int) this.mHostConfigKey.GetValue(nameof (EnableMemoryTrim), (object) 0) != 0;
      }
      set
      {
        this.mHostConfigKey.SetValue(nameof (EnableMemoryTrim), (object) (!value ? 0 : 1));
        this.mHostConfigKey.Flush();
      }
    }

    public bool GLES3
    {
      get
      {
        return (int) this.mHostConfigKey.GetValue(nameof (GLES3), (object) 0) != 0;
      }
      set
      {
        this.mHostConfigKey.SetValue(nameof (GLES3), (object) (!value ? 0 : 1));
        this.mHostConfigKey.Flush();
      }
    }

    public bool IsAutoShowGuidance
    {
      get
      {
        return (int) this.mHostConfigKey.GetValue(nameof (IsAutoShowGuidance), (object) 1) != 0;
      }
      set
      {
        this.mHostConfigKey.SetValue(nameof (IsAutoShowGuidance), (object) (!value ? 0 : 1));
        this.mHostConfigKey.Flush();
      }
    }

    public string[] DisabledGuidancePackages
    {
      get
      {
        return (string[]) this.mHostConfigKey.GetValue(nameof (DisabledGuidancePackages), (object) new string[0]);
      }
      set
      {
        this.mHostConfigKey.SetValue(nameof (DisabledGuidancePackages), (object) value);
        this.mHostConfigKey.Flush();
      }
    }

    public bool IsRememberWindowPositionEnabled
    {
      get
      {
        return (int) this.mHostConfigKey.GetValue(nameof (IsRememberWindowPositionEnabled), (object) 1) != 0;
      }
      set
      {
        this.mHostConfigKey.SetValue(nameof (IsRememberWindowPositionEnabled), (object) (!value ? 0 : 1));
        this.mHostConfigKey.Flush();
      }
    }

    public string InstallID
    {
      get
      {
        return (string) this.mHostConfigKey.GetValue(nameof (InstallID), (object) string.Empty);
      }
      set
      {
        this.mHostConfigKey.SetValue(nameof (InstallID), (object) value);
        this.mHostConfigKey.Flush();
      }
    }

    public string OldInstallID
    {
      get
      {
        return (string) this.mHostConfigKey.GetValue(nameof (OldInstallID), (object) string.Empty);
      }
      set
      {
        this.mHostConfigKey.SetValue(nameof (OldInstallID), (object) value);
        this.mHostConfigKey.Flush();
      }
    }

    public string HelperVersion
    {
      get
      {
        return (string) this.mHostConfigKey.GetValue(nameof (HelperVersion), (object) string.Empty);
      }
      set
      {
        this.mHostConfigKey.SetValue(nameof (HelperVersion), (object) value);
        this.mHostConfigKey.Flush();
      }
    }

    public string InstallerPkgName
    {
      get
      {
        return (string) this.mHostConfigKey.GetValue(nameof (InstallerPkgName), (object) string.Empty);
      }
      set
      {
        this.mHostConfigKey.SetValue(nameof (InstallerPkgName), (object) value);
        this.mHostConfigKey.Flush();
      }
    }

    public InstallationTypes InstallationType
    {
      get
      {
        if (this.mInstallationType == InstallationTypes.None)
          this.mInstallationType = (InstallationTypes) Enum.Parse(typeof (InstallationTypes), (string) this.mHostConfigKey.GetValue(nameof (InstallationType), (object) InstallationTypes.FullEdition.ToString()), true);
        return this.mInstallationType;
      }
      set
      {
        this.mHostConfigKey.SetValue(nameof (InstallationType), (object) value);
        this.mHostConfigKey.Flush();
        this.mInstallationType = value;
      }
    }

    public string CurrentFirebaseHost
    {
      get
      {
        return (string) this.mClientKey.GetValue(nameof (CurrentFirebaseHost), (object) string.Empty);
      }
      set
      {
        this.mClientKey.SetValue(nameof (CurrentFirebaseHost), (object) value);
        this.mClientKey.Flush();
      }
    }

    public string PendingLaunchAction
    {
      get
      {
        return (string) this.mClientKey.GetValue(nameof (PendingLaunchAction), (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0},{1}", (object) GenericAction.None, (object) string.Empty));
      }
      set
      {
        this.mClientKey.SetValue(nameof (PendingLaunchAction), (object) value);
        this.mClientKey.Flush();
      }
    }

    public DateTime AnnouncementTime
    {
      get
      {
        string s = (string) this.mHostConfigKey.GetValue(nameof (AnnouncementTime), (object) string.Empty);
        DateTime dateTime = DateTime.Now;
        try
        {
          if (!string.IsNullOrEmpty(s))
            dateTime = DateTime.ParseExact(s, "dd/MM/yyyy HH:mm:ss", (IFormatProvider) CultureInfo.InvariantCulture);
        }
        catch
        {
        }
        return dateTime;
      }
      set
      {
        this.mHostConfigKey.SetValue(nameof (AnnouncementTime), (object) value.ToString("dd/MM/yyyy HH:mm:ss", (IFormatProvider) CultureInfo.InvariantCulture));
        this.mHostConfigKey.Flush();
      }
    }

    public string RootVdiMd5Hash
    {
      get
      {
        return (string) this.mHostConfigKey.GetValue(nameof (RootVdiMd5Hash), (object) string.Empty);
      }
      set
      {
        this.mHostConfigKey.SetValue(nameof (RootVdiMd5Hash), (object) value);
        this.mHostConfigKey.Flush();
      }
    }

    public string Geo
    {
      get
      {
        return (string) this.mClientKey.GetValue(nameof (Geo), (object) "");
      }
      set
      {
        this.mClientKey.SetValue(nameof (Geo), (object) value);
        this.mClientKey.Flush();
      }
    }

    public string QuitDefaultOption
    {
      get
      {
        return (string) this.mClientKey.GetValue(nameof (QuitDefaultOption), (object) "STRING_CLOSE_CURRENT_INSTANCE");
      }
      set
      {
        this.mClientKey.SetValue(nameof (QuitDefaultOption), (object) value);
        this.mClientKey.Flush();
      }
    }

    public bool IsQuitOptionSaved
    {
      get
      {
        return (int) this.mClientKey.GetValue("QuitOptionSaved", (object) 0) != 0;
      }
      set
      {
        this.mClientKey.SetValue("QuitOptionSaved", (object) (!value ? 0 : 1));
        this.mClientKey.Flush();
      }
    }

    public int VmId
    {
      get
      {
        return (int) this.mHostConfigKey.GetValue(nameof (VmId), (object) 1);
      }
      set
      {
        this.mHostConfigKey.SetValue(nameof (VmId), (object) value);
        this.mHostConfigKey.Flush();
      }
    }

    public string[] BookmarkedScriptList
    {
      get
      {
        return (string[]) this.mHostConfigKey.GetValue(nameof (BookmarkedScriptList), (object) new string[0]);
      }
      set
      {
        this.mHostConfigKey.SetValue(nameof (BookmarkedScriptList), (object) value, RegistryValueKind.MultiString);
      }
    }

    public string DefaultShortcuts
    {
      get
      {
        return (string) this.mHostConfigKey.GetValue(nameof (DefaultShortcuts), (object) "");
      }
      set
      {
        this.mHostConfigKey.SetValue(nameof (DefaultShortcuts), (object) value);
        this.mHostConfigKey.Flush();
      }
    }

    public string UserDefinedShortcuts
    {
      get
      {
        return (string) this.mHostConfigKey.GetValue(nameof (UserDefinedShortcuts), (object) "");
      }
      set
      {
        this.mHostConfigKey.SetValue(nameof (UserDefinedShortcuts), (object) Regex.Replace(value, "\\n|\\r", ""));
        this.mHostConfigKey.Flush();
      }
    }

    public string[] DefaultSidebarElements
    {
      get
      {
        return (string[]) this.mHostConfigKey.GetValue(nameof (DefaultSidebarElements), (object) new string[0]);
      }
      set
      {
        this.mHostConfigKey.SetValue(nameof (DefaultSidebarElements), (object) value, RegistryValueKind.MultiString);
        this.mHostConfigKey.Flush();
      }
    }

    public string[] UserDefinedSidebarElements
    {
      get
      {
        return (string[]) this.mHostConfigKey.GetValue(nameof (UserDefinedSidebarElements), (object) new string[0]);
      }
      set
      {
        this.mHostConfigKey.SetValue(nameof (UserDefinedSidebarElements), (object) value, RegistryValueKind.MultiString);
        this.mHostConfigKey.Flush();
      }
    }

    public bool AreAllInstancesMuted
    {
      get
      {
        return (int) this.mHostConfigKey.GetValue(nameof (AreAllInstancesMuted), (object) 0) != 0;
      }
      set
      {
        this.mHostConfigKey.SetValue(nameof (AreAllInstancesMuted), (object) (!value ? 0 : 1));
        this.mHostConfigKey.Flush();
      }
    }

    public bool IsSamsungStorePresent
    {
      get
      {
        return (int) this.mHostConfigKey.GetValue(nameof (IsSamsungStorePresent), (object) 0) != 0;
      }
      set
      {
        this.mHostConfigKey.SetValue(nameof (IsSamsungStorePresent), (object) (!value ? 0 : 1));
        this.mHostConfigKey.Flush();
      }
    }

    public bool IsCacodeValid
    {
      get
      {
        return (int) this.mHostConfigKey.GetValue(nameof (IsCacodeValid), (object) 0) != 0;
      }
      set
      {
        this.mHostConfigKey.SetValue(nameof (IsCacodeValid), (object) (value ? 1 : 0));
        this.mHostConfigKey.Flush();
      }
    }

    public void SetClientThemeNameInRegistry(string themeName)
    {
      this.mClientKey.SetValue("ClientThemeName", (object) themeName);
      RegistryManager.ClientThemeName = themeName;
      this.mClientKey.Flush();
    }

    public string GetClientThemeNameFromRegistry()
    {
      return this.mClientKey.GetValue("ClientThemeName", (object) "Assets").ToString();
    }

    public int AdvancedControlTransparencyLevel
    {
      get
      {
        return (int) this.mClientKey.GetValue(nameof (AdvancedControlTransparencyLevel), (object) 50);
      }
      set
      {
        this.mClientKey.SetValue(nameof (AdvancedControlTransparencyLevel), (object) value);
        this.mClientKey.Flush();
      }
    }

    public bool IsUtcConverterBlurbOnboardingCompleted
    {
      get
      {
        return (int) this.mHostConfigKey.GetValue(nameof (IsUtcConverterBlurbOnboardingCompleted), (object) 0) != 0;
      }
      set
      {
        this.mHostConfigKey.SetValue(nameof (IsUtcConverterBlurbOnboardingCompleted), (object) (value ? 1 : 0));
        this.mHostConfigKey.Flush();
      }
    }

    public bool IsUtcConverterRedDotOnboardingCompleted
    {
      get
      {
        return (int) this.mHostConfigKey.GetValue(nameof (IsUtcConverterRedDotOnboardingCompleted), (object) 0) != 0;
      }
      set
      {
        this.mHostConfigKey.SetValue(nameof (IsUtcConverterRedDotOnboardingCompleted), (object) (value ? 1 : 0));
        this.mHostConfigKey.Flush();
      }
    }

    public AppLaunchState FirstAppLaunchState
    {
      get
      {
        if (this.mFirstAppLaunchState == AppLaunchState.Unknown)
          this.mFirstAppLaunchState = (AppLaunchState) Enum.Parse(typeof (AppLaunchState), (string) this.mClientKey.GetValue("FirstAppLaunchedState", (object) AppLaunchState.Launched.ToString()), true);
        return this.mFirstAppLaunchState;
      }
      set
      {
        this.mClientKey.SetValue("FirstAppLaunchedState", (object) value);
        this.mClientKey.Flush();
        this.mFirstAppLaunchState = value;
      }
    }

    public string AppConfiguration
    {
      get
      {
        return (string) this.mHostConfigKey.GetValue(nameof (AppConfiguration), (object) "{ }");
      }
      set
      {
        this.mHostConfigKey.SetValue(nameof (AppConfiguration), (object) value);
        this.mHostConfigKey.Flush();
      }
    }

    public string AppPlayerEngineInfo
    {
      get
      {
        return (string) this.mHostConfigKey.GetValue(nameof (AppPlayerEngineInfo), (object) Constants.DefaultAppPlayerEngineInfo);
      }
      set
      {
        this.mHostConfigKey.SetValue(nameof (AppPlayerEngineInfo), (object) value);
        this.mHostConfigKey.Flush();
      }
    }

    public int CloudABIValue
    {
      get
      {
        return (int) this.mHostConfigKey.GetValue(nameof (CloudABIValue), (object) 0);
      }
      set
      {
        this.mHostConfigKey.SetValue(nameof (CloudABIValue), (object) value);
        this.mHostConfigKey.Flush();
      }
    }

    public int NotificationModeCounter
    {
      get
      {
        return (int) this.mHostConfigKey.GetValue(nameof (NotificationModeCounter), (object) 3);
      }
      set
      {
        this.mHostConfigKey.SetValue(nameof (NotificationModeCounter), (object) value);
        this.mHostConfigKey.Flush();
      }
    }

    public bool IsNotificationModeAlwaysOn
    {
      get
      {
        return (int) this.mHostConfigKey.GetValue(nameof (IsNotificationModeAlwaysOn), (object) 0) != 0;
      }
      set
      {
        this.mHostConfigKey.SetValue(nameof (IsNotificationModeAlwaysOn), (object) (!value ? 0 : 1));
        this.mHostConfigKey.Flush();
      }
    }

    public bool IsNotificationSoundsActive
    {
      get
      {
        return (int) this.mHostConfigKey.GetValue(nameof (IsNotificationSoundsActive), (object) 1) != 0;
      }
      set
      {
        this.mHostConfigKey.SetValue(nameof (IsNotificationSoundsActive), (object) (!value ? 0 : 1));
        this.mHostConfigKey.Flush();
      }
    }

    public bool FixNotificationDataAfterFirstPostBootCloudInfo
    {
      get
      {
        return (int) this.mHostConfigKey.GetValue(nameof (FixNotificationDataAfterFirstPostBootCloudInfo), (object) 0) != 0;
      }
      set
      {
        this.mHostConfigKey.SetValue(nameof (FixNotificationDataAfterFirstPostBootCloudInfo), (object) (!value ? 0 : 1));
        this.mHostConfigKey.Flush();
      }
    }

    public string UpdaterFileDeletePath
    {
      get
      {
        return (string) this.mClientKey.GetValue(nameof (UpdaterFileDeletePath), (object) "");
      }
      set
      {
        this.mClientKey.SetValue(nameof (UpdaterFileDeletePath), (object) value);
        this.mClientKey.Flush();
      }
    }
  }
}
