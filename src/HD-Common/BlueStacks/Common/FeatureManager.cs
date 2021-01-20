// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.FeatureManager
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;

namespace BlueStacks.Common
{
  public class FeatureManager
  {
    private static string sFilePath = string.Empty;
    private static object syncRoot = new object();
    private const string sConfigFilename = "bst_config";
    private static volatile FeatureManager sInstance;

    public static FeatureManager Instance
    {
      get
      {
        if (FeatureManager.sInstance == null)
        {
          lock (FeatureManager.syncRoot)
          {
            if (FeatureManager.sInstance == null)
            {
              FeatureManager.sInstance = new FeatureManager();
              FeatureManager.Init(true, true);
            }
          }
        }
        return FeatureManager.sInstance;
      }
    }

    public static void Init(bool isAsync = true, bool downloadBstConfig = true)
    {
      try
      {
        string clientInstallDir = RegistryManager.Instance.ClientInstallDir;
        if (string.IsNullOrEmpty(clientInstallDir))
        {
          Logger.Warning("ClientInstallDir is not set. FeatureManager should not be used before creating directory");
          FeatureManager.SetDefaultSettings();
        }
        else
        {
          FeatureManager.sFilePath = Path.Combine(clientInstallDir, "bst_config");
          Logger.Info(string.Format("Feature Manager Init - isAync:{0} downloadBstConfig:{1}", (object) isAsync, (object) downloadBstConfig));
          Logger.Info(string.Format("Feature Manager Init - UpdateBstConfig:{0} - {1} Path:{2} DoesExist: {3}", (object) RegistryManager.Instance.UpdateBstConfig, (object) "bst_config", (object) FeatureManager.sFilePath, (object) !System.IO.File.Exists(FeatureManager.sFilePath)));
          if (downloadBstConfig && RegistryManager.Instance.UpdateBstConfig || downloadBstConfig && !RegistryManager.Instance.UpdateBstConfig && !System.IO.File.Exists(FeatureManager.sFilePath))
            FeatureManager.DownloadBstConfig(isAsync);
          else if (System.IO.File.Exists(FeatureManager.sFilePath))
            FeatureManager.LoadFile(FeatureManager.sFilePath, true);
          else
            FeatureManager.SetDefaultSettings();
        }
      }
      catch (Exception ex)
      {
        Logger.Info(string.Format("Error loading {0}: {1}", (object) "bst_config", (object) ex));
      }
    }

    private static void DownloadBstConfig(bool isAsync)
    {
      FeatureManager.SetDefaultSettings();
      if (isAsync)
        FeatureManager.DownloadFileAsync();
      else
        FeatureManager.DownloadFile();
    }

    private static void DownloadFileAsync()
    {
      new Thread((ThreadStart) (() => FeatureManager.DownloadFile()))
      {
        IsBackground = true
      }.Start();
    }

    private static void DownloadFile()
    {
      try
      {
        string urlWithParams = WebHelper.GetUrlWithParams(WebHelper.GetServerHost() + "/serve_config_file", (string) null, (string) null, (string) null);
        string directoryName = Path.GetDirectoryName(FeatureManager.sFilePath);
        Logger.Info("bst_config file download url: " + urlWithParams + " and file path: " + FeatureManager.sFilePath);
        if (!Directory.Exists(directoryName))
        {
          Logger.Info("--------- For debugging -------. FeatureManager was used before creating directory. This should not happen.");
          Directory.CreateDirectory(directoryName);
        }
        using (WebClient webClient = new WebClient())
        {
          webClient.DownloadFile(urlWithParams, FeatureManager.sFilePath);
          RegistryManager.Instance.UpdateBstConfig = false;
        }
      }
      catch (Exception ex)
      {
        Logger.Warning(string.Format("Failed to download {0} file. Err: {1}", (object) "bst_config", (object) ex));
        RegistryManager.Instance.UpdateBstConfig = true;
        return;
      }
      FeatureManager.LoadFile(FeatureManager.sFilePath, false);
    }

    private static void SetDefaultSettings()
    {
      Logger.Info("FeatureManager->SetDefaultSettings");
      if (FeatureManager.sInstance != null)
        return;
      FeatureManager.sInstance = new FeatureManager();
    }

    public static void LoadFile(string filePath, bool retryOnError = true)
    {
      try
      {
        Logger.Info("Loading bst_config Settings from " + filePath);
        using (XmlReader xmlReader = XmlReader.Create((Stream) System.IO.File.OpenRead(filePath)))
          FeatureManager.sInstance = (FeatureManager) new XmlSerializer(typeof (FeatureManager)).Deserialize(xmlReader);
      }
      catch (Exception ex1)
      {
        FeatureManager.SetDefaultSettings();
        try
        {
          if (!retryOnError)
            return;
          if (System.IO.File.Exists(filePath))
            System.IO.File.Delete(filePath);
          FeatureManager.DownloadFileAsync();
        }
        catch (Exception ex2)
        {
        }
      }
    }

    public bool IsBTVEnabled { get; set; }

    public bool IsWallpaperChangeDisabled { get; set; }

    public bool IsCreateBrowserOnStart { get; set; }

    public bool IsOpenActivityFromAccountIcon { get; set; }

    public bool IsBrowserKilledOnTabSwitch { get; set; }

    public bool IsPromotionDisabled { get; set; }

    public bool IsGuidBackUpEnable { get; set; } = true;

    public bool IsCustomUIForDMMSandbox { get; set; }

    public bool IsThemeEnabled { get; set; } = true;

    public bool IsSearchBarVisible { get; set; } = true;

    public bool IsCustomResolutionInputAllowed { get; set; }

    public bool ShowBeginnersGuidePreference { get; set; } = true;

    public bool IsShowNotificationCentre { get; set; } = true;

    public bool IsUseWpfTextbox { get; set; }

    public bool IsComboKeysDisabled { get; set; }

    public bool IsMacroRecorderEnabled { get; set; }

    public bool IsFarmingModeDisabled { get; set; }

    public bool IsOperationsSyncEnabled { get; set; }

    public bool IsRotateScreenDisabled { get; set; }

    public bool IsUserAccountBtnEnabled { get; set; } = true;

    public bool IsWarningBtnEnabled { get; set; } = true;

    public bool IsAppCenterTabVisible { get; set; } = true;

    public bool IsMultiInstanceControlsGridVisible { get; set; } = true;

    public bool IsPromotionFixed { get; set; }

    public bool IsShowLanguagePreference { get; set; } = true;

    public bool IsShowDesktopShortcutPreference { get; set; } = true;

    public bool IsShowGamingSummaryPreference { get; set; } = true;

    public bool IsShowSpeedUpTips { get; set; } = true;

    public bool IsShowPostOTSScreen { get; set; } = true;

    public bool IsShowHelpCenter { get; set; } = true;

    public bool IsAppSettingsAvailable { get; set; } = true;

    public bool IsShowPerformancePreference { get; set; } = true;

    public bool IsShowDiscordPreference { get; set; } = true;

    public bool IsCustomUIForNCSoft { get; set; }

    public bool AllowADBSettingToggle { get; set; } = true;

    public bool ShowClientOnTopPreference { get; set; } = true;

    public bool IsAllowGameRecording { get; set; } = true;

    public bool IsShowAppRecommendations { get; set; } = true;

    public bool IsCheckForQuitPopup { get; set; } = true;

    public bool IsCustomCursorEnabled { get; set; }

    public bool ForceEnableMacroAndSync { get; set; }

    public bool IsHtmlSideBar { get; set; } = true;

    public bool IsTimelineStatsEnabled { get; set; } = true;

    public bool IsShowAdvanceExitOption { get; set; }

    public bool IsShowAndroidInputDebugSetting { get; set; } = true;

    public bool IsTopbarHelpEnabled { get; set; } = true;

    public bool ShowMiManagerMenuButton { get; set; } = true;

    public bool IsHtmlHome { get; set; } = true;

    public bool IsShowTouchSoundSetting { get; set; } = true;
  }
}
