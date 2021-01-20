// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.BGPHelper
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.BlueStacksUI.BTv;
using BlueStacks.Common;
using System;
using System.Collections.Generic;
using System.Threading;

namespace BlueStacks.BlueStacksUI
{
  internal class BGPHelper
  {
    internal static void InitHttpServerAsync()
    {
      new Thread(new ThreadStart(BGPHelper.SetupHTTPServer))
      {
        IsBackground = true
      }.Start();
    }

    internal static void SetupHTTPServer()
    {
      HttpHandlerSetup.InitHTTPServer(new Dictionary<string, HTTPServer.RequestHandler>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
      {
        {
          "/ping",
          new HTTPServer.RequestHandler(HTTPHandler.PingHandler)
        },
        {
          "/appDisplayed",
          new HTTPServer.RequestHandler(HTTPHandler.AppDisplayedHandler)
        },
        {
          "/appLaunched",
          new HTTPServer.RequestHandler(HTTPHandler.AppLaunchedHandler)
        },
        {
          "/showApp",
          new HTTPServer.RequestHandler(HTTPHandler.ShowAppHandler)
        },
        {
          "/showWindow",
          new HTTPServer.RequestHandler(HTTPHandler.ShowWindowHandler)
        },
        {
          "/isVisible",
          new HTTPServer.RequestHandler(HTTPHandler.IsVisibleHandler)
        },
        {
          "/appUninstalled",
          new HTTPServer.RequestHandler(HTTPHandler.AppUninstalledHandler)
        },
        {
          "/appInstalled",
          new HTTPServer.RequestHandler(HTTPHandler.AppInstalledHandler)
        },
        {
          "/enableWndProcLogging",
          new HTTPServer.RequestHandler(HTTPHandler.EnableWndProcLogging)
        },
        {
          "/quit",
          new HTTPServer.RequestHandler(HTTPHandler.ForceQuitHandler)
        },
        {
          "/google",
          new HTTPServer.RequestHandler(HTTPHandler.OpenGoogleHandler)
        },
        {
          "/closeCrashedAppTab",
          new HTTPServer.RequestHandler(HTTPHandler.AppCrashedHandler)
        },
        {
          "/showWebPage",
          new HTTPServer.RequestHandler(HTTPHandler.ShowWebPageHandler)
        },
        {
          "/showHomeTab",
          new HTTPServer.RequestHandler(HTTPHandler.ShowHomeTabHandler)
        },
        {
          "/closeTab",
          new HTTPServer.RequestHandler(HTTPHandler.CloseTabHandler)
        },
        {
          "/updateUserInfo",
          new HTTPServer.RequestHandler(HTTPHandler.UpdateUserInfoHandler)
        },
        {
          "/oneTimeSetupCompleted",
          new HTTPServer.RequestHandler(HTTPHandler.OneTimeSetupCompletedHandler)
        },
        {
          "/appInstallStarted",
          new HTTPServer.RequestHandler(HTTPHandler.AppInstallStarted)
        },
        {
          "/appInstallFailed",
          new HTTPServer.RequestHandler(HTTPHandler.AppInstallFailed)
        },
        {
          "/googlePlayAppInstall",
          new HTTPServer.RequestHandler(HTTPHandler.GooglePlayAppInstall)
        },
        {
          "/bootFailedPopup",
          new HTTPServer.RequestHandler(HTTPHandler.BootFailedPopupHandler)
        },
        {
          "/dragDropInstall",
          new HTTPServer.RequestHandler(HTTPHandler.DragDropInstallHandler)
        },
        {
          "/openPackage",
          new HTTPServer.RequestHandler(HTTPHandler.OpenOrInstallPackageHandler)
        },
        {
          "/stopInstance",
          new HTTPServer.RequestHandler(HTTPHandler.StopInstanceHandler)
        },
        {
          "/minimizeInstance",
          new HTTPServer.RequestHandler(HTTPHandler.MinimizeInstanceHandler)
        },
        {
          "/startInstance",
          new HTTPServer.RequestHandler(HTTPHandler.StartInstanceHandler)
        },
        {
          "/hideBluestacks",
          new HTTPServer.RequestHandler(HTTPHandler.HideBluestacksHandler)
        },
        {
          "/tileWindow",
          new HTTPServer.RequestHandler(HTTPHandler.TileWindow)
        },
        {
          "/cascadeWindow",
          new HTTPServer.RequestHandler(HTTPHandler.CascadeWindow)
        },
        {
          "/launchWebTab",
          new HTTPServer.RequestHandler(HTTPHandler.LaunchWebTab)
        },
        {
          "/openNotificationSettings",
          new HTTPServer.RequestHandler(HTTPHandler.ShowSettingWindow)
        },
        {
          "/isAnyAppRunning",
          new HTTPServer.RequestHandler(HTTPHandler.IsAnyAppRunning)
        },
        {
          "/launchDefaultWebApp",
          new HTTPServer.RequestHandler(HTTPHandler.LaunchDefaultWebApp)
        },
        {
          "/toggleFarmMode",
          new HTTPServer.RequestHandler(HTTPHandler.ToggleFarmMode)
        },
        {
          "/changeTextOTS",
          new HTTPServer.RequestHandler(HTTPHandler.ChangeTextOTSHandler)
        },
        {
          "/macroCompleted",
          new HTTPServer.RequestHandler(HTTPHandler.MacroCompleted)
        },
        {
          "/appInfoUpdated",
          new HTTPServer.RequestHandler(HTTPHandler.AppInfoUpdated)
        },
        {
          "/sendAppDisplayed",
          new HTTPServer.RequestHandler(HTTPHandler.SendAppDisplayed)
        },
        {
          "/static",
          new HTTPServer.RequestHandler(HTTPHandler.IsBlueStacksUIVisible)
        },
        {
          "/restartFrontend",
          new HTTPServer.RequestHandler(HTTPHandler.RestartFrontend)
        },
        {
          "/gcCollect",
          new HTTPServer.RequestHandler(HTTPHandler.GCCollect)
        },
        {
          "/showWindowAndApp",
          new HTTPServer.RequestHandler(HTTPHandler.ShowWindowAndAppHandler)
        },
        {
          "/unsupportedCpuError",
          new HTTPServer.RequestHandler(HTTPHandler.UnsupportedCPUError)
        },
        {
          "/changeOrientaion",
          new HTTPServer.RequestHandler(HTTPHandler.ChangeOrientaionHandler)
        },
        {
          "/shootingModeChanged",
          new HTTPServer.RequestHandler(HTTPHandler.ShootingModeChanged)
        },
        {
          "/guestBootCompleted",
          new HTTPServer.RequestHandler(HTTPHandler.GuestBootCompleted)
        },
        {
          "/getRunningInstances",
          new HTTPServer.RequestHandler(HTTPHandler.GetRunningInstances)
        },
        {
          "/appJsonChanged",
          new HTTPServer.RequestHandler(HTTPHandler.AppJsonChangedHandler)
        },
        {
          "/getCurrentAppDetails",
          new HTTPServer.RequestHandler(HTTPHandler.GetCurrentAppDetails)
        },
        {
          "/maintenanceWarning",
          new HTTPServer.RequestHandler(HTTPHandler.ShowMaintenanceWarning)
        },
        {
          "/updateSizeOfOverlay",
          new HTTPServer.RequestHandler(HTTPHandler.UpdateSizeOfOverlay)
        },
        {
          "/androidLocaleChanged",
          new HTTPServer.RequestHandler(HTTPHandler.AndroidLocaleChanged)
        },
        {
          "/saveComboEvents",
          new HTTPServer.RequestHandler(HTTPHandler.SaveComboEvents)
        },
        {
          "/handleClientOperation",
          new HTTPServer.RequestHandler(HTTPHandler.HandleClientOperation)
        },
        {
          "/handleGamepadConnection",
          new HTTPServer.RequestHandler(HTTPHandler.HandleGamepadConnection)
        },
        {
          "/macroPlaybackComplete",
          new HTTPServer.RequestHandler(HTTPHandler.MacroPlaybackCompleteHandler)
        },
        {
          "/toggleStreamingMode",
          new HTTPServer.RequestHandler(HTTPHandler.ToggleStreamingMode)
        },
        {
          "/handleClientGamepadButton",
          new HTTPServer.RequestHandler(HTTPHandler.HandleClientGamepadButtonHandler)
        },
        {
          "/handleGamepadGuidanceButton",
          new HTTPServer.RequestHandler(HTTPHandler.GamepadGuidanceButtonHandler)
        },
        {
          "/deviceProvisioned",
          new HTTPServer.RequestHandler(HTTPHandler.DeviceProvisionedHandler)
        },
        {
          "/googleSignin",
          new HTTPServer.RequestHandler(HTTPHandler.GoogleSigninHandler)
        },
        {
          "/hideTopSidebar",
          new HTTPServer.RequestHandler(HTTPHandler.HideTopSideBarHandler)
        },
        {
          "/showFullscreenSidebarButton",
          new HTTPServer.RequestHandler(HTTPHandler.FullScreenSidebarButtonHandler)
        },
        {
          "/showFullscreenTopbarButton",
          new HTTPServer.RequestHandler(HTTPHandler.FullScreenTopbarButtonHandler)
        },
        {
          "/showFullscreenSidebar",
          new HTTPServer.RequestHandler(HTTPHandler.FullScreenSidebarHandler)
        },
        {
          "/setCurrentVolumeFromAndroid",
          new HTTPServer.RequestHandler(HTTPHandler.SetCurrentVolumeFromAndroidHandler)
        },
        {
          "/enableDebugLogs",
          new HTTPServer.RequestHandler(HTTPHandler.EnableDebugLogs)
        },
        {
          "/setDMMKeymapping",
          new HTTPServer.RequestHandler(HTTPHandler.SetDMMKeymapping)
        },
        {
          "/ncSetGameInfoOnTopBar",
          new HTTPServer.RequestHandler(HTTPHandler.NCSetGameInfoOnTopBarHandler)
        },
        {
          "/updateLocale",
          new HTTPServer.RequestHandler(HTTPHandler.UpdateLocale)
        },
        {
          "/screenshotCaptured",
          new HTTPServer.RequestHandler(HTTPHandler.ScreenshotCaptured)
        },
        {
          "/hotKeyEvents",
          new HTTPServer.RequestHandler(HTTPHandler.ClientHotkeyHandler)
        },
        {
          "/launchPlay",
          new HTTPServer.RequestHandler(HTTPHandler.LaunchPlay)
        },
        {
          "/enableKeyboardHookLogging",
          new HTTPServer.RequestHandler(HTTPHandler.EnableKeyboardHookLogging)
        },
        {
          "/muteAllInstances",
          new HTTPServer.RequestHandler(HTTPHandler.MuteAllInstancesHandler)
        },
        {
          "/screenLock",
          new HTTPServer.RequestHandler(HTTPHandler.ScreenLock)
        },
        {
          "/getHeightWidth",
          new HTTPServer.RequestHandler(HTTPHandler.GetHeightWidth)
        },
        {
          "/accountSetupCompleted",
          new HTTPServer.RequestHandler(HTTPHandler.AccountSetupCompleted)
        },
        {
          "/openThemeEditor",
          new HTTPServer.RequestHandler(HTTPHandler.OpenThemeEditor)
        },
        {
          "/setStreamingStatus",
          new HTTPServer.RequestHandler(HTTPHandler.SetStreamingStatus)
        },
        {
          "/playerScriptModifierClick",
          new HTTPServer.RequestHandler(HTTPHandler.PlayerScriptModifierKeyUp)
        },
        {
          "/reloadShortcuts",
          new HTTPServer.RequestHandler(HTTPHandler.ReloadShortcuts)
        },
        {
          "/reloadPromotions",
          new HTTPServer.RequestHandler(HTTPHandler.ReloadPromotions)
        },
        {
          "/overlayControlsVisibility",
          new HTTPServer.RequestHandler(HTTPHandler.HandleOverlayControlsVisibility)
        },
        {
          "/showGrmAndLaunchApp",
          new HTTPServer.RequestHandler(HTTPHandler.ShowGrmAndLaunchAppHandler)
        },
        {
          "/reinitRegistry",
          new HTTPServer.RequestHandler(HTTPHandler.ReinitRegistry)
        },
        {
          "/openCFGReorderTool",
          new HTTPServer.RequestHandler(HTTPHandler.OpenCFGReorderTool)
        },
        {
          "/updateCrc",
          new HTTPServer.RequestHandler(HTTPHandler.UpdateCrc)
        },
        {
          "/configFileChanged",
          new HTTPServer.RequestHandler(HTTPHandler.ConfigFileChanged)
        },
        {
          "/addNotificationInDrawer",
          new HTTPServer.RequestHandler(HTTPHandler.AddNotificationInDrawer)
        },
        {
          "/markNotificationInDrawer",
          new HTTPServer.RequestHandler(HTTPHandler.MarkNotificationInDrawer)
        },
        {
          "/checkCallbackEnabledStatus",
          new HTTPServer.RequestHandler(HTTPHandler.CheckCallbackEnabledStatus)
        },
        {
          "/hideOverlayWhenIMEActive",
          new HTTPServer.RequestHandler(HTTPHandler.HideOverlayWhenIMEActive)
        },
        {
          "/showImageUploadedInfo",
          new HTTPServer.RequestHandler(HTTPHandler.ShowImageUploadedInfo)
        },
        {
          "/showFullscreenTopbar",
          new HTTPServer.RequestHandler(HTTPHandler.FullScreenTopbarHandler)
        },
        {
          "/obsStatus",
          new HTTPServer.RequestHandler(BTVManager.ObsStatusHandler)
        },
        {
          "/reportObsError",
          new HTTPServer.RequestHandler(BTVManager.ReportObsErrorHandler)
        },
        {
          "/capturingError",
          new HTTPServer.RequestHandler(BTVManager.ReportCaptureError)
        },
        {
          "/openGLCapturingError",
          new HTTPServer.RequestHandler(BTVManager.ReportOpenGLCaptureError)
        }
      });
    }
  }
}
