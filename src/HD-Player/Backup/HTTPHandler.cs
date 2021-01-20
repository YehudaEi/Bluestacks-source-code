// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.HTTPHandler
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using BlueStacks.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using SystemInfo;

namespace BlueStacks.Player
{
  internal class HTTPHandler
  {
    private static readonly object sPlayerShuttingDownLockObject = new object();
    private static bool mIsPlayerShuttingDown = false;
    private static bool IsDeviceProvisionReceived = false;

    [DllImport("HD-Imap-Native.dll")]
    private static extern int ImapHandleOrientation(int Orientation);

    internal static void StartServer()
    {
      new Thread((ThreadStart) (() => HttpHandlerSetup.InitHTTPServer(HTTPHandler.GetRoutes(), MultiInstanceStrings.VmName)))
      {
        IsBackground = true
      }.Start();
    }

    internal static Dictionary<string, HTTPServer.RequestHandler> GetRoutes()
    {
      return new Dictionary<string, HTTPServer.RequestHandler>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
      {
        {
          "/ping",
          new HTTPServer.RequestHandler(HTTPHandler.PingHandler)
        },
        {
          "/refreshKeymap",
          new HTTPServer.RequestHandler(HTTPHandler.RefreshKeyMappingHandler)
        },
        {
          "/shutdown",
          new HTTPServer.RequestHandler(HTTPHandler.Shutdown)
        },
        {
          "/switchOrientation",
          new HTTPServer.RequestHandler(HTTPHandler.SwitchOrientation)
        },
        {
          "/showWindow",
          new HTTPServer.RequestHandler(HTTPHandler.ShowWindow)
        },
        {
          "/refreshWindow",
          new HTTPServer.RequestHandler(HTTPHandler.RefreshWindow)
        },
        {
          "/setParent",
          new HTTPServer.RequestHandler(HTTPHandler.SetParent)
        },
        {
          "/shareScreenshot",
          new HTTPServer.RequestHandler(HTTPHandler.ShareScreenshot)
        },
        {
          "/goBack",
          new HTTPServer.RequestHandler(HTTPHandler.GoBack)
        },
        {
          "/closeScreen",
          new HTTPServer.RequestHandler(HTTPHandler.CloseScreen)
        },
        {
          "/softControlBarEvent",
          new HTTPServer.RequestHandler(HTTPHandler.HandleSoftControlBarEvent)
        },
        {
          "/inputMapperFilesDownloaded",
          new HTTPServer.RequestHandler(HTTPHandler.InputMapperFilesDownloaded)
        },
        {
          "/enableWndProcLogging",
          new HTTPServer.RequestHandler(HTTPHandler.EnableWndProcLogging)
        },
        {
          "/pingVm",
          new HTTPServer.RequestHandler(HTTPHandler.PingVMHandler)
        },
        {
          "/copyFiles",
          new HTTPServer.RequestHandler(HTTPHandler.SendFilesToWindows)
        },
        {
          "/getWindowsFiles",
          new HTTPServer.RequestHandler(HTTPHandler.PickFilesFromWindows)
        },
        {
          "/gpsCoordinates",
          new HTTPServer.RequestHandler(HTTPHandler.UpdateGpsCoordinates)
        },
        {
          "/initGamepad",
          new HTTPServer.RequestHandler(HTTPHandler.InitGamePad)
        },
        {
          "/getVolume",
          new HTTPServer.RequestHandler(HTTPHandler.GetProductVolume)
        },
        {
          "/setVolume",
          new HTTPServer.RequestHandler(HTTPHandler.SetProductVolume)
        },
        {
          "/topDisplayedActivityInfo",
          new HTTPServer.RequestHandler(HTTPHandler.TopDisplayedActivityInfo)
        },
        {
          "/appDisplayed",
          new HTTPServer.RequestHandler(HTTPHandler.GetAppDisplayedInfo)
        },
        {
          "/goHome",
          new HTTPServer.RequestHandler(HTTPHandler.GoHome)
        },
        {
          "/isKeyboardEnabled",
          new HTTPServer.RequestHandler(HTTPHandler.IsKeyboardEnabled)
        },
        {
          "/setKeymappingState",
          new HTTPServer.RequestHandler(HTTPHandler.SetKeyMappingState)
        },
        {
          "/keymap",
          new HTTPServer.RequestHandler(HTTPHandler.KeyMappingHandler)
        },
        {
          "/setFrontendVisibility",
          new HTTPServer.RequestHandler(HTTPHandler.SetFrontendVisibility)
        },
        {
          "/getFeSize",
          new HTTPServer.RequestHandler(HTTPHandler.GetFESize)
        },
        {
          "/mute",
          new HTTPServer.RequestHandler(HTTPHandler.MuteHandler)
        },
        {
          "/unmute",
          new HTTPServer.RequestHandler(HTTPHandler.UnmuteHandler)
        },
        {
          "/getCurrentKeymappingStatus",
          new HTTPServer.RequestHandler(HTTPHandler.IsKeyMappingEnabled)
        },
        {
          "/shake",
          new HTTPServer.RequestHandler(HTTPHandler.ShakeHandler)
        },
        {
          "/isKeyNameFocussed",
          new HTTPServer.RequestHandler(HTTPHandler.IsKeyNameFocussed)
        },
        {
          "/androidImeSelected",
          new HTTPServer.RequestHandler(HTTPHandler.AndroidImeSelected)
        },
        {
          "/isGpsSupported",
          new HTTPServer.RequestHandler(HTTPHandler.IsGPSSupported)
        },
        {
          "/installApk",
          new HTTPServer.RequestHandler(HTTPHandler.InstallApk)
        },
        {
          "/injectCopy",
          new HTTPServer.RequestHandler(HTTPHandler.InjectCopyHandler)
        },
        {
          "/injectPaste",
          new HTTPServer.RequestHandler(HTTPHandler.InjectPasteHandler)
        },
        {
          "/stopZygote",
          new HTTPServer.RequestHandler(HTTPHandler.StopZygote)
        },
        {
          "/startZygote",
          new HTTPServer.RequestHandler(HTTPHandler.StartZygote)
        },
        {
          "/getKeyMappingParserVersion",
          new HTTPServer.RequestHandler(HTTPHandler.KeyMappingParserVersion)
        },
        {
          "/vibrateHostWindow",
          new HTTPServer.RequestHandler(HTTPHandler.VibrateHostWindowHandler)
        },
        {
          "/localeChanged",
          new HTTPServer.RequestHandler(HTTPHandler.LocaleChangedHandler)
        },
        {
          "/getScreenshot",
          new HTTPServer.RequestHandler(HTTPHandler.GetScreenShot)
        },
        {
          "/setPcImeWorkflow",
          new HTTPServer.RequestHandler(HTTPHandler.SetPcImeWorkflow)
        },
        {
          "/setUserInfo",
          new HTTPServer.RequestHandler(HTTPHandler.SetUserInfoHandler)
        },
        {
          "/getUserInfo",
          new HTTPServer.RequestHandler(HTTPHandler.GetUserInfoHandler)
        },
        {
          "/getPremium",
          new HTTPServer.RequestHandler(HTTPHandler.GetPremiumHandler)
        },
        {
          "/setCursorStyle",
          new HTTPServer.RequestHandler(HTTPHandler.SetCursorStyle)
        },
        {
          "/openMacroWindow",
          new HTTPServer.RequestHandler(HTTPHandler.OpenMacroWindow)
        },
        {
          "/startReroll",
          new HTTPServer.RequestHandler(HTTPHandler.StartReroll)
        },
        {
          "/abortReroll",
          new HTTPServer.RequestHandler(HTTPHandler.AbortReroll)
        },
        {
          "/setPackagesForInteraction",
          new HTTPServer.RequestHandler(AppHandler.SetPackagesForCountingInteractions)
        },
        {
          "/getInteractionForPackage",
          new HTTPServer.RequestHandler(HTTPHandler.GetInteractionCountForPackages)
        },
        {
          "/toggleScreen",
          new HTTPServer.RequestHandler(HTTPHandler.ToggleScreen)
        },
        {
          "/sendGlWindowSize",
          new HTTPServer.RequestHandler(HTTPHandler.SendGLWindowSize)
        },
        {
          "/deactivateFrontend",
          new HTTPServer.RequestHandler(HTTPHandler.DeactivateFrontend)
        },
        {
          "/startRecordingCombo",
          new HTTPServer.RequestHandler(HTTPHandler.StartRecordingCombo)
        },
        {
          "/stopRecordingCombo",
          new HTTPServer.RequestHandler(HTTPHandler.StopRecordingCombo)
        },
        {
          "/pauseRecordingCombo",
          new HTTPServer.RequestHandler(HTTPHandler.PauseRecordingCombo)
        },
        {
          "/handleClientOperation",
          new HTTPServer.RequestHandler(HTTPHandler.HandleClientOperation)
        },
        {
          "/initMacroPlayback",
          new HTTPServer.RequestHandler(HTTPHandler.InitMacroPlayback)
        },
        {
          "/stopMacroPlayback",
          new HTTPServer.RequestHandler(HTTPHandler.StopMacroPlayback)
        },
        {
          "/runMacroUnit",
          new HTTPServer.RequestHandler(HTTPHandler.RunMacroUnit)
        },
        {
          "/farmModeHandler",
          new HTTPServer.RequestHandler(HTTPHandler.FarmModeHandler)
        },
        {
          "/startOperationsSync",
          new HTTPServer.RequestHandler(HTTPHandler.StartOperationsSync)
        },
        {
          "/stopOperationsSync",
          new HTTPServer.RequestHandler(HTTPHandler.StopOperationsSync)
        },
        {
          "/startSyncConsumer",
          new HTTPServer.RequestHandler(HTTPHandler.StartSyncConsumer)
        },
        {
          "/stopSyncConsumer",
          new HTTPServer.RequestHandler(HTTPHandler.StopSyncConsumer)
        },
        {
          "/showFPS",
          new HTTPServer.RequestHandler(HTTPHandler.ShowFPS)
        },
        {
          "/enableVSync",
          new HTTPServer.RequestHandler(HTTPHandler.EnableVSync)
        },
        {
          "/frontendVisibleChanged",
          new HTTPServer.RequestHandler(HTTPHandler.FrontendVisibleChangedHandler)
        },
        {
          "/oneTimeSetupCompleted",
          new HTTPServer.RequestHandler(HTTPHandler.OTSCompletedHandler)
        },
        {
          "/closeCrashedAppTab",
          new HTTPServer.RequestHandler(HTTPHandler.CloseCrashedTabHandler)
        },
        {
          "/appDataFeUrl",
          new HTTPServer.RequestHandler(HTTPHandler.SetCurrentAppData)
        },
        {
          "/runAppInfo",
          new HTTPServer.RequestHandler(HTTPHandler.RunAppInfo)
        },
        {
          "/stopAppInfo",
          new HTTPServer.RequestHandler(HTTPHandler.StopAppInfo)
        },
        {
          "/quitFrontend",
          new HTTPServer.RequestHandler(HTTPHandler.QuitFrontend)
        },
        {
          "/toggleGamepadButton",
          new HTTPServer.RequestHandler(HTTPHandler.ToggleGamepadButton)
        },
        {
          "/deviceProvisioned",
          new HTTPServer.RequestHandler(HTTPHandler.DeviceProvisionedHandler)
        },
        {
          "/deviceProvisionedReceived",
          new HTTPServer.RequestHandler(HTTPHandler.DeviceProvisionedReceived)
        },
        {
          "/googleSignin",
          new HTTPServer.RequestHandler(HTTPHandler.GoogleSigninHandler)
        },
        {
          "/isAppPlayerRooted",
          new HTTPServer.RequestHandler(HTTPHandler.IsAppPlayerRooted)
        },
        {
          "/setIsFullscreen",
          new HTTPServer.RequestHandler(HTTPHandler.SetFullScreenState)
        },
        {
          "/getInteractionStats",
          new HTTPServer.RequestHandler(InputMapper.GetInteractionStats)
        },
        {
          "/enableGamepad",
          new HTTPServer.RequestHandler(InputMapper.EnableGamepad)
        },
        {
          "/exportCfgFile",
          new HTTPServer.RequestHandler(InputMapper.ExportCfgFile)
        },
        {
          "/importCfgFile",
          new HTTPServer.RequestHandler(InputMapper.ImportCfgFile)
        },
        {
          "/enableDebugLogs",
          new HTTPServer.RequestHandler(HTTPHandler.EnableDebugLogs)
        },
        {
          "/reloadShortcutsConfig",
          new HTTPServer.RequestHandler(HTTPHandler.ReloadShortcutsConfig)
        },
        {
          "/accountSetupCompleted",
          new HTTPServer.RequestHandler(HTTPHandler.AccountSetupCompleted)
        },
        {
          "/scriptEditingModeEntered",
          new HTTPServer.RequestHandler(HTTPHandler.ScriptEditingModeStateChanged)
        },
        {
          "/playPauseSync",
          new HTTPServer.RequestHandler(HTTPHandler.PlayPauseSyncHandler)
        },
        {
          "/reinitGuestRegistry",
          new HTTPServer.RequestHandler(HTTPHandler.ReinitGuestRegistry)
        },
        {
          "/updateMacroShortcutsDict",
          new HTTPServer.RequestHandler(HTTPHandler.UpdateMacroShortcutsDict)
        },
        {
          "/setAstcOption",
          new HTTPServer.RequestHandler(HTTPHandler.SetAstcOption)
        },
        {
          "/validateScriptCommands",
          new HTTPServer.RequestHandler(HTTPHandler.ValidateSciptCommands)
        },
        {
          "/changeimei",
          new HTTPServer.RequestHandler(HTTPHandler.ChangeImei)
        },
        {
          "/enableNativeGamepad",
          new HTTPServer.RequestHandler(InputMapper.EnableNativeGamepadControls)
        },
        {
          "/sendImagePickerCoordinates",
          new HTTPServer.RequestHandler(HTTPHandler.SendImagePickerCoordinates)
        },
        {
          "/toggleImagePickerMode",
          new HTTPServer.RequestHandler(HTTPHandler.ImagePickerModeStateChanged)
        },
        {
          "/handleLoadConfigOnTabSwitch",
          new HTTPServer.RequestHandler(HTTPHandler.HandleLoadConfigAfterHomeSwitch)
        },
        {
          "/sendCustomCursorEnabledApps",
          new HTTPServer.RequestHandler(HTTPHandler.SendCustomCursorEnabledApps)
        },
        {
          "/toggleScrollOnEdgeFeature",
          new HTTPServer.RequestHandler(HTTPHandler.EnableScrollOnEdgeMode)
        },
        {
          "/forceShutdown",
          new HTTPServer.RequestHandler(HTTPHandler.ForceExit)
        },
        {
          "/bootcompleted",
          new HTTPServer.RequestHandler(HTTPHandler.BootCompletedHandler)
        },
        {
          "/enableMemoryTrim",
          new HTTPServer.RequestHandler(HTTPHandler.EnableMemoryTrim)
        },
        {
          "/checkIfGuestBooted",
          new HTTPServer.RequestHandler(HTTPHandler.CheckIfGuestBootedHandler)
        },
        {
          "/toggleIsMouseLocked",
          new HTTPServer.RequestHandler(HTTPHandler.ToggleIsMouseLocked)
        }
      };
    }

    private static void EnableVSync(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        Opengl.SetVsync(bool.Parse(HTTPUtils.ParseRequest(req).Data["enableVsync"]) ? 1 : 0);
        HTTPHandler.WriteSuccessJson(res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in enable vsync. Err : " + ex.ToString());
        HTTPHandler.WriteErrorJson(ex.Message, res);
      }
    }

    private static void ValidateSciptCommands(HttpListenerRequest req, HttpListenerResponse res)
    {
      bool flag = false;
      try
      {
        flag = InputMapper.Instance.IsScriptCommandsValid(HTTPUtils.ParseRequest(req).Data["script"]);
      }
      catch (Exception ex)
      {
        Logger.Error(string.Format("Failed to validate script commands. Er : {0}", (object) ex));
      }
      if (flag)
        HTTPHandler.WriteSuccessJson(res);
      else
        HTTPHandler.WriteErrorJson("invalid script", res);
    }

    private static void SendImagePickerCoordinates(
      HttpListenerRequest req,
      HttpListenerResponse res)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        float result1;
        float.TryParse(request.Data["X"], out result1);
        float result2;
        float.TryParse(request.Data["Y"], out result2);
        uint crc = 0;
        Logger.Info("ImagePicker: X: " + result1.ToString() + "  Y:" + result2.ToString());
        Opengl.ImgdUpdateScreenPoint(ref result1, ref result2, ref crc);
        HTTPUtils.SendRequestToClient("updateCrc", new Dictionary<string, string>()
        {
          {
            "X",
            result1.ToString((IFormatProvider) CultureInfo.InvariantCulture)
          },
          {
            "Y",
            result2.ToString((IFormatProvider) CultureInfo.InvariantCulture)
          },
          {
            "Crc",
            crc.ToString((IFormatProvider) CultureInfo.InvariantCulture)
          }
        }, "Android", 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in SendImagePickerCoordinates. Err : " + ex.ToString());
        HTTPHandler.WriteErrorJson(ex.Message, res);
      }
    }

    private static void ImagePickerModeStateChanged(
      HttpListenerRequest req,
      HttpListenerResponse res)
    {
      try
      {
        string str = HTTPUtils.ParseRequest(req).Data["isInImagePickerMode"];
        VMWindow.Instance.ImagePickerModeStateChanged(bool.Parse(str));
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in ImagePickerModeStateChanged. Err : " + ex.ToString());
        HTTPHandler.WriteErrorJson(ex.Message, res);
      }
    }

    private static void HandleLoadConfigAfterHomeSwitch(
      HttpListenerRequest req,
      HttpListenerResponse res)
    {
      try
      {
        InputMapper.Instance.HandleLoadConfigAfterHomeSwitch(HTTPUtils.ParseRequest(req).Data["package"]);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in HandleLoadConfigAfterHomeSwitch. Err : " + ex.ToString());
        HTTPHandler.WriteErrorJson(ex.Message, res);
      }
    }

    private static void SendCustomCursorEnabledApps(
      HttpListenerRequest req,
      HttpListenerResponse res)
    {
      try
      {
        string str = HTTPUtils.ParseRequest(req).Data["packages"];
        if (string.IsNullOrEmpty(str))
          return;
        VMWindow.Instance.mCustomCursorAppsList = ((IEnumerable<string>) str.Split(' ')).ToList<string>();
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in SendCustomCursorEnabledApps. Err : " + ex.ToString());
        HTTPHandler.WriteErrorJson(ex.Message, res);
      }
    }

    private static void EnableScrollOnEdgeMode(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        bool boolean = Convert.ToBoolean(HTTPUtils.ParseRequest(req).Data["isEnabled"].ToString());
        VMWindow.Instance.ToggleScrollOnEdgeState(boolean);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in EnableScrollOnEdgeMode. Err : " + ex.ToString());
        HTTPHandler.WriteErrorJson(ex.Message, res);
      }
    }

    private static void ToggleIsMouseLocked(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        bool boolean = Convert.ToBoolean(HTTPUtils.ParseRequest(req).Data["isLocked"].ToString());
        VMWindow.Instance.ToggleMouseLockedState(boolean);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in ToggleIsMouseLocked. Err : " + ex.ToString());
        HTTPHandler.WriteErrorJson(ex.Message, res);
      }
    }

    private static void EnableMemoryTrim(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        MemoryManager.CheckAndTrimAndroidMemory();
        MemoryManager.TrimMemory(false);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in EnableMemoryTrim. Err : " + ex.ToString());
        HTTPHandler.WriteErrorJson(ex.Message, res);
      }
    }

    private static void SetAstcOption(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        Opengl.SetAstcOption(Convert.ToInt32(HTTPUtils.ParseRequest(req).Data["AstcOption"]));
        HTTPHandler.WriteSuccessJson(res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in Change Astc. Err : " + ex.ToString());
        HTTPHandler.WriteErrorJson(ex.Message, res);
      }
    }

    private static void ReinitGuestRegistry(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RegistryManager.ClearRegistryMangerInstance();
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to reinit registry. Err : " + ex.ToString());
      }
    }

    private static void PlayPauseSyncHandler(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        bool boolean = Convert.ToBoolean(HTTPUtils.ParseRequest(req).Data["pause"]);
        Logger.Info("isPause : " + boolean.ToString());
        OperationsSyncManager.Instance.PlayPauseOperationsSync(boolean);
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to play pause sync. Err : " + ex.ToString());
      }
    }

    private static void ScriptEditingModeStateChanged(
      HttpListenerRequest req,
      HttpListenerResponse res)
    {
      string str = HTTPUtils.ParseRequest(req).Data["isInScriptMode"];
      VMWindow.Instance.ClientScriptModeStateChanged(bool.Parse(str));
    }

    private static void AccountSetupCompleted(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        HTTPUtils.SendRequestToClient("accountSetupCompleted", (Dictionary<string, string>) null, HTTPUtils.ParseRequest(req).RequestVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
        HTTPHandler.WriteSuccessJson(res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in AccountSetupCompleted Handler: " + ex.ToString());
        HTTPHandler.WriteErrorJson(ex.Message, res);
      }
    }

    private static void StopOperationsSync(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        HTTPUtils.ParseRequest(req);
        OperationsSyncManager.Instance.StopOperationsSync();
        HTTPHandler.WriteSuccessJson(res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in StopOperationsSync. Err : " + ex.ToString());
        HTTPHandler.WriteErrorJson(ex.Message, res);
      }
    }

    private static void StartOperationsSync(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        OperationsSyncManager.Instance.StartSyncToInstances((IEnumerable<string>) HTTPUtils.ParseRequest(req).Data["instances"].Split(new char[1]
        {
          ','
        }, StringSplitOptions.RemoveEmptyEntries));
        HTTPHandler.WriteSuccessJson(res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in StartOperationsSync. Err : " + ex.ToString());
        HTTPHandler.WriteErrorJson(ex.Message, res);
      }
    }

    private static void StartSyncConsumer(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        OperationsSyncManager.Instance.StartSyncConsumer(HTTPUtils.ParseRequest(req).Data["instance"]);
        HTTPHandler.WriteSuccessJson(res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in StartSyncConsumer. Err : " + ex.ToString());
        HTTPHandler.WriteErrorJson(ex.Message, res);
      }
    }

    private static void StopSyncConsumer(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        OperationsSyncManager.Instance.StopSyncConsumer();
        HTTPHandler.WriteSuccessJson(res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in StopSyncConsumer. Err : " + ex.ToString());
        HTTPHandler.WriteErrorJson(ex.Message, res);
      }
    }

    private static void ShowFPS(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        Opengl.ShowFPS(int.Parse(HTTPUtils.ParseRequest(req).Data["isshowfps"]));
        HTTPHandler.WriteSuccessJson(res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in Change FPS. Err : " + ex.ToString());
        HTTPHandler.WriteErrorJson(ex.Message, res);
      }
    }

    private static void HandleClientOperation(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        InputMapper.Instance.SendClientString(HTTPUtils.ParseRequest(req).Data["operationData"]);
        HTTPHandler.WriteSuccessJson(res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in HandleClientOperation. Err : " + ex.ToString());
        HTTPHandler.WriteErrorJson(ex.Message, res);
      }
    }

    private static void GetInteractionCountForPackages(
      HttpListenerRequest req,
      HttpListenerResponse res)
    {
      try
      {
        if (AppHandler.sAppPackagesCountClicks.ContainsKey("?"))
        {
          string empty = string.Empty;
          long minValue = long.MinValue;
          foreach (KeyValuePair<string, long> sDictCountClick in AppHandler.sDictCountClicks)
          {
            if (sDictCountClick.Value > minValue)
            {
              minValue = sDictCountClick.Value;
              empty = sDictCountClick.Key.ToString();
            }
          }
          if (!AppHandler.sAppPackagesCountClicks.ContainsKey(empty))
            AppHandler.sAppPackagesCountClicks.Add(empty, minValue);
          else
            AppHandler.sAppPackagesCountClicks[empty] = minValue;
          AppHandler.sAppPackagesCountClicks["?"] = AppHandler.sDictCountClicks.Count != 0 ? minValue : 0L;
        }
        HTTPUtils.Write(JsonConvert.SerializeObject((object) AppHandler.sAppPackagesCountClicks, Formatting.None).ToString(), res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in GetInteractionCountForPackages... Err : " + ex.ToString());
      }
    }

    private static void EnableWndProcLogging(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        VMWindow.isLogWndProc = !VMWindow.isLogWndProc;
        Logger.Debug("Got request for EnableWndProcLogging" + VMWindow.isLogWndProc.ToString());
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in set EnableWndProcLogging... Err : " + ex.ToString());
      }
    }

    private static void EnableDebugLogs(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        Logger.EnableDebugLogs();
        HTTPHandler.WriteSuccessJson(res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in EnableDebugLogs... Err : " + ex.ToString());
        HTTPHandler.WriteErrorJson(ex.ToString(), res);
      }
    }

    private static void ReloadShortcutsConfig(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        VMWindow.Instance.mShortcutConfig = ShortcutConfig.LoadShortcutsConfig();
        if (VMWindow.Instance.mShortcutConfig != null)
          HTTPHandler.WriteSuccessJson(res);
        else
          HTTPHandler.WriteErrorJson("ShortcutConfig is null, see inner exception", res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in ReloadShortcutsConfig... Err : " + ex.ToString());
        HTTPHandler.WriteErrorJson(ex.ToString(), res);
      }
    }

    private static void SetPcImeWorkflow(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        VMWindow.Instance.SetPcImeWorkflow(true);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in set PC Ime workflow... Err : " + ex.ToString());
      }
    }

    private static void SetCursorStyle(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        string path = HTTPUtils.ParseRequest(req).Data["path"];
        if (path != null)
        {
          UIHelper.RunOnUIThread((Control) VMWindow.Instance, (UIHelper.Action) (() => VMWindow.Instance.ChangeCursorStyle(path, false)));
          HTTPHandler.WriteSuccessJson(res);
        }
        else
          HTTPHandler.WriteErrorJson("Path is invalid", res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in SetCursorStyle. Err : " + ex.ToString());
        HTTPHandler.WriteErrorJson(ex.ToString(), res);
      }
    }

    private static void OpenMacroWindow(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        UIHelper.RunOnUIThread((Control) VMWindow.Instance, (UIHelper.Action) (() => MacroForm.Instance.Show()));
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in OpenMacroWindow: " + ex.ToString());
        HTTPHandler.WriteErrorJson(ex.ToString(), res);
      }
    }

    private static void StartReroll(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        UIHelper.RunOnUIThread((Control) VMWindow.Instance, (UIHelper.Action) (() =>
        {
          RequestData request = HTTPUtils.ParseRequest(req);
          string packageName = request.Data["packageName"];
          string MacroName = request.Data["rerollName"];
          MacroData.Instance.LoadMacroData(packageName);
          MacroForm.PlayMacro(MacroName);
        }));
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in OpenMacroWindow. Err : " + ex.ToString());
        HTTPHandler.WriteErrorJson(ex.ToString(), res);
      }
    }

    private static void AbortReroll(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        MacroForm.AbortReroll();
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in AbortReroll. Err : " + ex.ToString());
        HTTPHandler.WriteErrorJson(ex.ToString(), res);
      }
    }

    private static void GetPremiumHandler(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        bool boolean = Convert.ToBoolean(HTTPUtils.ParseRequest(req).Data["value"]);
        Logger.Info("Is User Premium : " + boolean.ToString());
        RegistryManager.Instance.IsPremium = boolean;
        HTTPHandler.WriteSuccessJson(res);
        foreach (string vm in RegistryManager.Instance.VmList)
        {
          try
          {
            HTTPUtils.SendRequestToClient("updateUserInfo", (Dictionary<string, string>) null, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
          }
          catch (Exception ex)
          {
            Logger.Error("Exception in validating acc icon for vm. Err : " + vm + "... Reason : " + ex.ToString());
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in GetPremiumHandler. Err : " + ex.ToString());
        HTTPHandler.WriteErrorJson(ex.ToString(), res);
      }
    }

    private static void GetUserInfoHandler(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        JObject jobject = new JObject()
        {
          {
            "email",
            (JToken) RegistryManager.Instance.RegisteredEmail
          },
          {
            "token",
            (JToken) RegistryManager.Instance.Token
          },
          {
            "state",
            (JToken) (RegistryManager.Instance.IsPremium ? "PAID" : "AD-SUPPORTED")
          },
          {
            "success",
            (JToken) true
          }
        };
        Logger.Info("sending json : " + jobject.ToString(Formatting.None));
        HTTPUtils.Write(jobject.ToString(), res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in GetUserInfoHandler... Err : " + ex.ToString());
        HTTPHandler.WriteErrorJson(ex.ToString(), res);
      }
    }

    private static void SetUserInfoHandler(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        Dictionary<string, string> data = new Dictionary<string, string>();
        string str1 = request.Data["result"].Trim();
        string str2 = request.Data["error_msg"];
        data.Add("result", str1);
        if (str1.Equals("false", StringComparison.InvariantCultureIgnoreCase))
          Logger.Info("Error message in receiving token from android: " + str2);
        if (str1.Equals("true", StringComparison.InvariantCultureIgnoreCase))
        {
          string json = request.Data["user_info"];
          JObject jobject = JObject.Parse(json);
          Logger.Info("User info : " + json);
          string str3 = jobject["email"].ToString().Trim();
          if (!string.Equals(RegistryManager.Instance.RegisteredEmail, str3, StringComparison.InvariantCultureIgnoreCase))
          {
            RegistryManager.Instance.RegisteredEmail = str3;
            RegistryManager.Instance.Token = jobject["token"].ToString().Trim();
            RegistryManager.Instance.IsPremium = jobject["state"].ToString().Equals("PAID", StringComparison.InvariantCultureIgnoreCase);
            Stats.SendUnifiedInstallStatsAsync("bluestacks_login_completed", str3, "Android", "");
          }
        }
        HTTPUtils.SendRequestToClient("updateUserInfo", data, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
        HTTPHandler.WriteSuccessJson(res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in SetUserInfoHandler... Err : " + ex.ToString());
        HTTPHandler.WriteErrorJson(ex.ToString(), res);
      }
    }

    private static void CloseCrashedTabHandler(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        foreach (string allKey in request.Data.AllKeys)
          Logger.Debug("Key: {0}, Value: {1}", (object) allKey, (object) request.Data[allKey]);
        HTTPUtils.SendRequestToClient("closeCrashedAppTab", new Dictionary<string, string>()
        {
          {
            "package",
            request.Data["package"]
          }
        }, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
        HTTPHandler.WriteSuccessJson(res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in CloseCrashedTabHandler... Err : " + ex.ToString());
        HTTPHandler.WriteErrorJson(ex.ToString(), res);
      }
    }

    private static void OTSCompletedHandler(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        HTTPUtils.SendRequestToClient("oneTimeSetupCompleted", (Dictionary<string, string>) null, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
        HTTPHandler.WriteSuccessJson(res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in OTSCompletedHandler... Err : " + ex.ToString());
        HTTPHandler.WriteErrorJson(ex.ToString(), res);
      }
    }

    private static void FrontendVisibleChangedHandler(
      HttpListenerRequest req,
      HttpListenerResponse res)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        foreach (string key in request.Data.Keys)
          Logger.Warning("{0} {1}", (object) key, (object) request.Data[key]);
        int num = Convert.ToBoolean(request.Data["new_value"]) ? 1 : 0;
        bool boolean = Convert.ToBoolean(request.Data["is_mute"]);
        if (num != 0)
        {
          if (boolean)
            MediaManager.MuteEngine(true);
          else
            MediaManager.UnmuteEngine();
        }
        else
          MediaManager.MuteEngine(false);
        HTTPHandler.WriteSuccessJson(res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in FrontendVisibleChangedHandler... Err : " + ex.ToString());
        HTTPHandler.WriteErrorJson(ex.ToString(), res);
      }
    }

    private static void GetScreenShot(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        if (StringExtensions.IsValidPath(RegistryManager.Instance.ScreenShotsPath))
        {
          RequestData request = HTTPUtils.ParseRequest(req);
          string path = request.Data["path"];
          bool showSaved;
          if (!bool.TryParse(request.Data["showSavedInfo"] ?? "true", out showSaved))
            showSaved = true;
          string extension = Path.GetExtension(path);
          if (!string.IsNullOrEmpty(path) && (extension == ".png" || extension == ".jpeg"))
          {
            UIHelper.RunOnUIThread((Control) VMWindow.Instance, (UIHelper.Action) (() => VMWindow.Instance.SaveScreenShot(path, showSaved)));
            HTTPHandler.WriteSuccessJson(res);
          }
          else
            HTTPHandler.WriteErrorJson("Path is invalid", res);
        }
        else
          HTTPHandler.WriteErrorJson("Screenshot path is invalid:" + RegistryManager.Instance.ScreenShotsPath, res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in GetScreenShot. Err : " + ex.ToString());
        HTTPHandler.WriteErrorJson(ex.ToString(), res);
      }
    }

    private static void LocaleChangedHandler(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        if (Features.IsFeatureEnabled(4194304UL))
        {
          HTTPHandler.WriteSuccessJson(res);
        }
        else
        {
          string requestedLocale = HTTPUtils.ParseRequest(req).Data["result"];
          if (BlueStacks.Common.Globalization.sSupportedLocales.Keys.ToList<string>().FindIndex((Predicate<string>) (x => x.Equals(requestedLocale, StringComparison.InvariantCultureIgnoreCase))) == -1)
          {
            Logger.Warning("We do not support {0}, finding closest match", (object) requestedLocale);
            requestedLocale = BlueStacks.Common.Globalization.FindClosestMatchingLocale(requestedLocale);
            Logger.Info("Response for setlocale from guest : " + HTTPUtils.SendRequestToGuest("setLocale", new Dictionary<string, string>()
            {
              {
                "arg",
                requestedLocale
              }
            }, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp"));
          }
          Logger.Info("Setting locale: {0}", (object) requestedLocale);
          RegistryManager.Instance.UserSelectedLocale = requestedLocale;
          RegistryManager.Instance.Guest[MultiInstanceStrings.VmName].Locale = requestedLocale;
          Utils.UpdateValueInBootParams("LANG", requestedLocale, MultiInstanceStrings.VmName, false, "bgp");
          if (Opt.Instance.sysPrep || !Oem.Instance.IsSendGameManagerRequest)
            return;
          HTTPUtils.SendRequestToClientAsync("androidLocaleChanged", new Dictionary<string, string>()
          {
            {
              "locale",
              requestedLocale
            }
          }, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in AndroidLocalechanged. Err : " + ex.ToString());
      }
    }

    private static void VibrateHostWindowHandler(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        int duration = JObject.Parse(HTTPUtils.ParseRequest(req).Data["result"])["duration"].ToObject<int>();
        if (VMWindow.Instance.cSysInfo == null)
          VMWindow.Instance.cSysInfo = new CSysInfo();
        if (VMWindow.Instance.cSysInfo.JoystickIsConnected() == 1)
          VMWindow.Instance.cSysInfo.SetJoystickVibration(duration);
        if (VMWindow.Instance.mIsKBVibrationDllLoaded)
          MsiVibration.SetVibration(duration);
        HTTPHandler.WriteSuccessJson(res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception when sending vibration to MSI. Err : {0}", (object) ex.ToString());
      }
    }

    private static void KeyMappingParserVersion(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        HTTPUtils.Write(new JObject()
        {
          {
            "parserversion",
            (JToken) InputMapper.GetKeyMappingParserVersion()
          },
          {
            "minparserversion",
            (JToken) InputMapper.GetMinKeyMappingParserVersion()
          },
          {
            "success",
            (JToken) true
          }
        }.ToString(Formatting.None), res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in Server KeyMappingParserVersion. Err : " + ex.ToString());
        HTTPHandler.WriteErrorJson(ex.Message, res);
      }
    }

    private static void StartZygote(HttpListenerRequest req, HttpListenerResponse res)
    {
      RequestData request = HTTPUtils.ParseRequest(req);
      Logger.Info("Got request for startzygote for vm : " + request.Data["vmName"]);
      string vmName = request.Data["vmName"];
      try
      {
        Opengl.StartZygote(vmName);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in StartZygote. Err : " + ex.ToString());
      }
    }

    private static void StopZygote(HttpListenerRequest req, HttpListenerResponse res)
    {
      RequestData request = HTTPUtils.ParseRequest(req);
      Logger.Info("Got request for stopzygote for vm : " + request.Data["vmName"]);
      string vmName = request.Data["vmName"];
      try
      {
        Opengl.StopZygote(vmName);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in StopZygote. Err : " + ex.ToString());
      }
    }

    private static void InjectPasteHandler(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        SendKeys.SendWait("^V");
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in sending ctrl + v. Err : " + ex.ToString());
      }
    }

    private static void InjectCopyHandler(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        SendKeys.SendWait("^C");
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in sending ctrl + c. Err : " + ex.ToString());
      }
    }

    private static void InstallApk(HttpListenerRequest req, HttpListenerResponse res)
    {
      UIHelper.RunOnUIThread((Control) VMWindow.Instance, (UIHelper.Action) (() =>
      {
        try
        {
          OpenFileDialog openFileDialog = new OpenFileDialog()
          {
            Filter = "Android Files (*.apk, *.xapk) | *.apk; *.xapk"
          };
          if (openFileDialog.ShowDialog((IWin32Window) VMWindow.Instance) != DialogResult.OK)
            return;
          Logger.Info("File Selected : " + openFileDialog.FileName);
          string apkPath = openFileDialog.FileName;
          Logger.Info("Installing apk: {0}", (object) apkPath);
          ThreadPool.QueueUserWorkItem((WaitCallback) (obj => Utils.CallApkInstaller(apkPath, false)));
        }
        catch (Exception ex)
        {
          Logger.Error("Exception in getting install apk. Err : " + ex.ToString());
          HTTPHandler.WriteErrorJson(ex.ToString(), res);
        }
      }));
    }

    private static void StopAppInfo(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        string str = HTTPUtils.ParseRequest(req).Data["appPackage"];
        Logger.Info("Received stop app package = {0}", (object) str);
        if (string.IsNullOrEmpty(str) || !AppHandler.sLastAppDisplayed.Contains(str))
          return;
        AppHandler.sLastAppDisplayed = "";
        Logger.Info("assigned empty value to sLastAppDisplayed");
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in StopAppInfo. Err : " + ex.ToString());
        HTTPHandler.WriteErrorJson(ex.Message, res);
      }
    }

    private static void RunAppInfo(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        string str = HTTPUtils.ParseRequest(req).Data["appPackage"];
        Logger.Info("Received appPackage = {0}", (object) str);
        if (string.IsNullOrEmpty(str))
          return;
        AppHandler.sAppPackage = str;
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in RunAppInfo. Err : " + ex.ToString());
        HTTPHandler.WriteErrorJson(ex.Message, res);
      }
    }

    private static void IsGPSSupported(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        if (SystemUtils.IsOSWinXP() || SystemUtils.IsOSWin7() || SystemUtils.IsOSVista())
          HTTPHandler.WriteErrorJson("not supported", res);
        else if (RegistryManager.Instance.DefaultGuest.GPSAvailable == 1)
          HTTPHandler.WriteSuccessJson(res);
        else
          HTTPHandler.WriteErrorJson("not supported", res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in getting GPS device status. Err : " + ex.ToString());
        HTTPHandler.WriteErrorJson("not supported", res);
      }
    }

    private static void AndroidImeSelected(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        string imeSelected = HTTPUtils.ParseRequest(req).Data["result"];
        if (!imeSelected.Equals("com.android.inputmethod.latin/.LatinIME"))
        {
          Logger.Info("Android Ime Selected in not latinIme");
          VMWindow.Instance.ChangeImeMode(false, true);
        }
        Utils.SetImeSelectedInReg(imeSelected, MultiInstanceStrings.VmName);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in AndroidImeSelected. Err : " + ex.ToString());
      }
    }

    private static void IsKeyNameFocussed(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        string str = HTTPUtils.ParseRequest(req).Data["state"].Trim();
        Logger.Info("The focussed state is " + str);
        if (str.Equals("true", StringComparison.InvariantCultureIgnoreCase))
          VMWindow.Instance.ChangeImeMode(false, true);
        else
          VMWindow.Instance.ChangeImeMode(true, true);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception Occured in IsKeyNameFocussed. Err : " + ex.ToString());
      }
    }

    private static void ShakeHandler(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        InputMapper.Shake();
        HTTPHandler.WriteSuccessJson(res);
      }
      catch (Exception ex)
      {
        Logger.Error(string.Format("Exception Occured in ShakeHandler. Err : {0}", (object) ex.ToString()));
        HTTPHandler.WriteErrorJson("Error in api", res);
      }
    }

    private static void IsKeyMappingEnabled(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        HTTPUtils.Write(new JObject()
        {
          {
            "success",
            (JToken) true
          },
          {
            "keymapping",
            (JToken) (!VMWindow.Instance.mIsTextInputBoxInFocus && InputMapper.s_UserKeyMappingEnabled)
          }
        }.ToString(Formatting.None), res);
      }
      catch (Exception ex)
      {
        Logger.Error(string.Format("Exception Occured IsKeyMappingEnabled. Err : {0}", (object) ex.ToString()));
        HTTPHandler.WriteErrorJson(ex.Message, res);
      }
    }

    private static void UnmuteHandler(HttpListenerRequest req, HttpListenerResponse res)
    {
      MediaManager.UnmuteEngine();
    }

    private static void MuteHandler(HttpListenerRequest req, HttpListenerResponse res)
    {
      string str = HTTPUtils.ParseRequest(req).Data["explicit"];
      bool result = true;
      if (!string.IsNullOrEmpty(str) && !bool.TryParse(str, out result))
        result = true;
      MediaManager.MuteEngine(result);
    }

    private static void GetFESize(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        HTTPUtils.Write(new JArray()
        {
          (JToken) new JObject()
          {
            {
              "success",
              (JToken) true
            },
            {
              "Height",
              (JToken) VMWindow.Instance.Size.Height
            },
            {
              "Width",
              (JToken) VMWindow.Instance.Size.Width
            },
            {
              "ClientHeight",
              (JToken) VMWindow.Instance.ClientSize.Height
            },
            {
              "ClientWidth",
              (JToken) VMWindow.Instance.ClientSize.Width
            }
          }
        }.ToString(Formatting.None), res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in Server GetFESize. Err : " + ex.ToString());
        HTTPHandler.WriteErrorJson(ex.Message, res);
      }
    }

    private static void SetFrontendVisibility(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        string strA = request.Data["visible"];
        string str = request.Data["appPackage"];
        Logger.Info("Received visible = {0} and appPackage = {1}", (object) strA, (object) str);
        if (!string.IsNullOrEmpty(str))
          AppHandler.sAppPackage = str;
        if (string.Compare(strA, "false", StringComparison.InvariantCultureIgnoreCase) == 0)
          UIHelper.RunOnUIThread((Control) VMWindow.Instance, (UIHelper.Action) (() =>
          {
            Logger.Info("Hiding frontend");
            VMWindow.Instance.HandleUserHideWindow();
          }));
        else
          UIHelper.RunOnUIThread((Control) VMWindow.Instance, (UIHelper.Action) (() =>
          {
            Logger.Info("Showing frontend");
            VMWindow.Instance.HandleUserShowWindow(0, 0);
          }));
        HTTPHandler.WriteSuccessJson(res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in SetFrontendVisibility. Err : " + ex.ToString());
        HTTPHandler.WriteErrorJson(ex.Message, res);
      }
    }

    private static void RefreshKeyMappingHandler(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        string appPkg = (string) null;
        try
        {
          appPkg = HTTPUtils.ParseRequest(req).Data["package"].ToString();
        }
        catch
        {
        }
        InputMapper.Instance.RefreshKeyMapping(appPkg);
        HTTPHandler.WriteSuccessJson(res);
      }
      catch (Exception ex)
      {
        Logger.Error(string.Format("Exception Occured in RefreshKeyMappingHandler. Err : {0}", (object) ex.ToString()));
        HTTPHandler.WriteErrorJson(ex.Message, res);
      }
    }

    private static void KeyMappingHandler(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        InputMapper.Instance.LaunchBlueStacksKeyMapper();
        HTTPHandler.WriteSuccessJson(res);
      }
      catch (Exception ex)
      {
        Logger.Error(string.Format("Exception Occured in KeyMappingHandler. Err : {0}", (object) ex.ToString()));
        HTTPHandler.WriteErrorJson(ex.Message, res);
      }
    }

    private static void SetKeyMappingState(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        if (string.Compare(HTTPUtils.ParseRequest(req).Data["keymapping"], "true", true) == 0)
        {
          InputMapper.s_UserKeyMappingEnabled = true;
          VMWindow.Instance.InputMapperHandlingState = VMWindow.InputHandlingState.IMAP_STATE_MAPPING;
        }
        else
        {
          InputMapper.s_UserKeyMappingEnabled = false;
          VMWindow.Instance.InputMapperHandlingState = VMWindow.InputHandlingState.IMAP_STATE_RAW;
        }
        HTTPUtils.Write(new JObject()
        {
          {
            "success",
            (JToken) true
          }
        }.ToString(Formatting.None), res);
      }
      catch (Exception ex)
      {
        Logger.Error(string.Format("Exception Occured in SetKeyMappingState. Err : {0}", (object) ex.ToString()));
        HTTPHandler.WriteErrorJson(ex.Message, res);
      }
    }

    private static void IsKeyboardEnabled(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        if (string.Compare(HTTPUtils.ParseRequest(req).Data["isinput"], "true", true) == 0)
        {
          Logger.Info("calling change ime with true");
          VMWindow.Instance.ChangeImeMode(true, true);
        }
        else
        {
          Logger.Info("calling change ime with false");
          VMWindow.Instance.ChangeImeMode(false, true);
        }
        if (!InputMapper.s_UserKeyMappingEnabled)
          HTTPHandler.WriteErrorJson("Cannot entertain this request as the user/keymappingtool has disabled keymapping, Will force this value when user enables keymapping", res);
        else
          HTTPUtils.Write(new JObject()
          {
            {
              "success",
              (JToken) true
            }
          }.ToString(), res);
      }
      catch (Exception ex)
      {
        Logger.Error(string.Format("Exception Occured IsKeyboardEnabled. Err : {0}", (object) ex.ToString()));
        HTTPHandler.WriteErrorJson(ex.Message, res);
      }
    }

    private static void GoHome(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        VmCmdHandler.RunCommand("home", MultiInstanceStrings.VmName, "bgp");
        HTTPHandler.WriteSuccessJson(res);
      }
      catch (Exception ex)
      {
        Logger.Error(string.Format("Exception Occured in GoHome. Err: {0}", (object) ex.ToString()));
        HTTPHandler.WriteErrorJson("unable to go home", res);
      }
    }

    private static void GetAppDisplayedInfo(HttpListenerRequest req, HttpListenerResponse res)
    {
      HTTPUtils.Write(new JArray()
      {
        (JToken) new JObject()
        {
          {
            "success",
            (JToken) true
          },
          {
            "LastAppDisplayed",
            (JToken) AppHandler.sLastAppDisplayed
          }
        }
      }.ToString(Formatting.None), res);
    }

    private static void TopDisplayedActivityInfo(HttpListenerRequest req, HttpListenerResponse res)
    {
      string str1 = "";
      string strA = "";
      try
      {
        str1 = HTTPUtils.ParseRequest(req).Data["appToken"];
        Logger.Info("appToken = " + str1);
        if (str1.IndexOf("com.bluestacks.appmart") != -1)
          AndroidBootUp.GuestBootCompletedEvent((object) null, new EventArgs());
        if (str1.IndexOf("com.bluestacks.keymappingtool") == -1)
        {
          string[] separator = new string[1]
          {
            "ActivityRecord"
          };
          strA = str1.Split(separator, StringSplitOptions.None)[1].Split(new string[1]
          {
            "u0 "
          }, StringSplitOptions.None)[1].Replace("}", "").Split(' ')[0];
          string str2 = strA;
          string package = strA.Split('/')[0];
          InputMapper.Instance.SetPackage(package);
          AppHandler.mCurrentAppPackage = package;
          if (!Opt.Instance.sysPrep && Oem.Instance.IsSendGameManagerRequest)
            HTTPUtils.SendRequestToClient("appDisplayed", new Dictionary<string, string>()
            {
              {
                "token",
                str1
              },
              {
                "packageName",
                package
              },
              {
                "appDisplayed",
                str2
              }
            }, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
        }
        if (!string.IsNullOrEmpty(strA) && string.Compare(strA, AppHandler.sLastAppDisplayed, true) != 0)
        {
          Logger.Info("appDisplayed = {0}, s_Console.sLastAppDisplayed= {1}", (object) strA, (object) AppHandler.sLastAppDisplayed);
          lock (AppHandler.sCurrentAppDisplayedLockObject)
            AppHandler.sLastAppDisplayed = strA;
        }
        HTTPUtils.Write("true", res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in Server TopDisplayedActivityInfo appToken = {0}. Err : {1}", (object) str1, (object) ex.ToString());
        HTTPHandler.WriteErrorJson(ex.Message, res);
      }
    }

    private static void SetCurrentAppData(HttpListenerRequest req, HttpListenerResponse res)
    {
      AppHandler.SetCurrentAppData(req, res);
    }

    private static void SetProductVolume(HttpListenerRequest req, HttpListenerResponse res)
    {
      if (MediaManager.SetGuestVolume(Convert.ToInt32(HTTPUtils.ParseRequest(req).Data["vol"])))
        HTTPHandler.WriteSuccessJson(res);
      else
        HTTPHandler.WriteErrorJson("", res);
    }

    private static void GetProductVolume(HttpListenerRequest req, HttpListenerResponse res)
    {
      int guestVolume = MediaManager.GetGuestVolume(HTTPUtils.ParseRequest(req).Data["mediatype"]);
      if (guestVolume != -1)
        HTTPUtils.Write(new JArray()
        {
          (JToken) new JObject()
          {
            {
              "success",
              (JToken) true
            },
            {
              "volume",
              (JToken) guestVolume
            }
          }
        }.ToString(Formatting.None), res);
      else
        HTTPHandler.WriteErrorJson("", res);
    }

    private static void InitGamePad(HttpListenerRequest req, HttpListenerResponse res)
    {
    }

    private static void UpdateGpsCoordinates(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        int gpsMode = RegistryManager.Instance.DefaultGuest.GpsMode;
        int gpsSource = RegistryManager.Instance.DefaultGuest.GpsSource;
        string gpsLatitude = RegistryManager.Instance.DefaultGuest.GpsLatitude;
        string gpsLongitude = RegistryManager.Instance.DefaultGuest.GpsLongitude;
        if (gpsMode == 0 || gpsSource == 0 || gpsSource != 8 && HTTPHandler.IsWindows7AndBelow())
        {
          Logger.Info(string.Format("Stopping Gps Service, gpsMode = {0}, gpsSource = {1}, IsWindows7AndBelow() = {2}", (object) gpsMode, (object) gpsSource, (object) HTTPHandler.IsWindows7AndBelow()));
          Logger.Info("No Coordinates Available so far");
          HTTPUtils.Write("", res);
        }
        else
          HTTPUtils.Write(gpsLatitude + "," + gpsLongitude, res);
      }
      catch (Exception ex)
      {
        Logger.Error(string.Format("Exception Occured in UpdateGpsCoordinates. Err : ", (object) ex.ToString()));
        HTTPUtils.Write("exception", res);
      }
    }

    private static bool IsWindows7AndBelow()
    {
      System.Version version = new System.Version(6, 2, 9200, 0);
      return Environment.OSVersion.Platform != PlatformID.Win32NT || !(Environment.OSVersion.Version >= version);
    }

    private static void PickFilesFromWindows(HttpListenerRequest req, HttpListenerResponse res)
    {
      RequestData requestData = HTTPUtils.ParseRequest(req);
      bool? isFileListEmpty = new bool?();
      UIHelper.RunOnUIThread((Control) VMWindow.Instance, (UIHelper.Action) (() =>
      {
        try
        {
          foreach (string allKey in requestData.Data.AllKeys)
            Logger.Info(string.Format("Key = {0}, Value = {1}", (object) allKey, (object) requestData.Data[allKey]));
          string bstSharedFolder = RegistryStrings.SharedFolderDir;
          string str1 = "";
          OpenFileDialog openFileDialog = new OpenFileDialog();
          if (string.Compare(requestData.Data["filesNo"].ToUpper(), "MULTIPLE") == 0)
            openFileDialog.Multiselect = true;
          openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
          if (requestData.Data["mimeType"].ToUpper().Contains("VIDEO") || requestData.Data["mimeType"].ToUpper().Contains("AUDIO"))
            str1 = "Video & Audio Files | *.dat; *.wmv; *.3g2; *.3gp; *.3gp2; *.3gpp; *.amv; *.asf;*.avi; *.bin; *.cue; *.divx; *.dv; *.flv; *.gxf; *.iso; *.m1v;*.m2v; *.m2t; *.m2ts; *.m4v; *.mkv; *.mov; *.mp2; *.mp2v; *.mp4;*.mp4v; *.mpa; *.mpe; *.mpeg; *.mpeg1; *.mpeg2; *.mpeg4; *.mpg;*.mpv2; *.mts; *.nsv; *.nuv; *.ogg; *.ogm; *.ogv; *.ogx; *.ps; *.rec;*.rm; *.rmvb; *.tod; *.ts; *.tts; *.vob; *.vro; *.webm; *.mp3";
          else if (requestData.Data["mimeType"].ToUpper().Contains("IMAGE"))
          {
            str1 = "Image Files|*.jpg;*.jpeg;*.png;*.gif";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
          }
          else if (str1.Length < 1)
            str1 = "All Files|*.*";
          openFileDialog.Filter = str1;
          JObject json = new JObject();
          Dictionary<string, string> data = new Dictionary<string, string>()
          {
            {
              "action",
              "com.bluestacks.FILES_FROM_WINDOWS_SERVICE"
            }
          };
          string folderName = Utils.CreateRandomBstSharedFolder(bstSharedFolder);
          bstSharedFolder = Path.Combine(bstSharedFolder, folderName);
          DialogResult result = openFileDialog.ShowDialog((IWin32Window) VMWindow.Instance);
          string[] str = openFileDialog.FileNames;
          ThreadPool.QueueUserWorkItem((WaitCallback) (obj =>
          {
            try
            {
              if (result == DialogResult.OK)
              {
                for (int index = 0; index < str.Length; ++index)
                {
                  string fileName = Path.GetFileName(str[index]);
                  Logger.Info(string.Format("Copying : {0}  to {1}", (object) str[index], (object) Path.Combine(bstSharedFolder, fileName)));
                  System.IO.File.Copy(str[index], Path.Combine(bstSharedFolder, fileName));
                  System.IO.File.SetAttributes(Path.Combine(bstSharedFolder, fileName), FileAttributes.Normal);
                }
                json.Add("success", (JToken) true.ToString());
                json.Add("files", (JToken) folderName);
                isFileListEmpty = new bool?(false);
              }
              else
              {
                json.Add("success", (JToken) false);
                try
                {
                  Directory.Delete(bstSharedFolder, true);
                }
                catch
                {
                }
                isFileListEmpty = new bool?(true);
              }
              data.Add("extras", json.ToString());
              HTTPUtils.SendRequestToGuest("customStartService", data, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>) null, false, 10, 500, "bgp");
            }
            catch (Exception ex)
            {
              isFileListEmpty = new bool?(true);
              Logger.Error(string.Format("Error Occured while calling to service, Err: {0}", (object) ex.ToString()));
            }
          }));
        }
        catch (Exception ex)
        {
          isFileListEmpty = new bool?(true);
          Logger.Error(string.Format("Error Occured while calling to service, Err: {0}", (object) ex.ToString()));
        }
      }));
      try
      {
        for (int index = 300; index >= 0 && !isFileListEmpty.HasValue; --index)
          Thread.Sleep(2000);
        if (!isFileListEmpty.HasValue || isFileListEmpty.Value)
          HTTPUtils.Write("false", res);
        else
          HTTPUtils.Write("true", res);
      }
      catch
      {
      }
    }

    private static void SendFilesToWindows(HttpListenerRequest req, HttpListenerResponse res)
    {
      RequestData request = HTTPUtils.ParseRequest(req);
      try
      {
        HTTPUtils.Write("true", res);
      }
      catch
      {
      }
      if (request.Data.Count > 1)
        HTTPHandler.SendMultipleFilesToWindows(request, res);
      else
        HTTPHandler.SendSingleFileToWindows(request, res);
    }

    public static void SendMultipleFilesToWindows(RequestData requestData, HttpListenerResponse res)
    {
      string responseStringSuccess = (string) null;
      string responseStringFailure = (string) null;
      string bstSharedFolder = RegistryStrings.SharedFolderDir;
      DialogResult result;
      string userCopyDir;
      UIHelper.RunOnUIThread((Control) VMWindow.Instance, (UIHelper.Action) (() =>
      {
        FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog()
        {
          ShowNewFolderButton = true,
          Description = "Choose folder to copy files",
          RootFolder = Environment.SpecialFolder.MyComputer
        };
        result = folderBrowserDialog.ShowDialog((IWin32Window) VMWindow.Instance);
        userCopyDir = folderBrowserDialog.SelectedPath;
        ThreadPool.QueueUserWorkItem((WaitCallback) (obj =>
        {
          if (result.Equals((object) DialogResult.OK))
          {
            Logger.Debug(string.Format("User Select {0} Directory", (object) userCopyDir));
            foreach (string allKey in requestData.Data.AllKeys)
            {
              try
              {
                Logger.Info("Key: {0}, Value: {1}", (object) allKey, (object) requestData.Data[allKey]);
                if (System.IO.File.Exists(Path.Combine(bstSharedFolder, requestData.Data[allKey].Trim())))
                {
                  if (string.Compare(userCopyDir, bstSharedFolder, false) != 0)
                  {
                    if (System.IO.File.Exists(Path.Combine(userCopyDir, requestData.Data[allKey].Trim())))
                    {
                      result = MessageBox.Show((IWin32Window) VMWindow.Instance, string.Format("Overwrite {0}?", (object) requestData.Data[allKey].Trim()), "File already exists", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk);
                      if (result == DialogResult.No)
                      {
                        System.IO.File.Delete(Path.Combine(bstSharedFolder, requestData.Data[allKey].Trim()));
                        continue;
                      }
                      if (System.IO.File.Exists(Path.Combine(userCopyDir, requestData.Data[allKey].Trim())))
                        System.IO.File.Delete(Path.Combine(userCopyDir, requestData.Data[allKey].Trim()));
                    }
                    System.IO.File.Move(Path.Combine(bstSharedFolder, requestData.Data[allKey].Trim()), Path.Combine(userCopyDir, requestData.Data[allKey].Trim()));
                  }
                  responseStringSuccess = responseStringSuccess != null ? responseStringSuccess + "\n" + Path.Combine(userCopyDir, requestData.Data[allKey].Trim()) : Path.Combine(userCopyDir, requestData.Data[allKey].Trim());
                }
                else
                {
                  responseStringFailure = responseStringFailure != null ? responseStringFailure + "\n" + Path.Combine(userCopyDir, requestData.Data[allKey].Trim()) : Path.Combine(userCopyDir, requestData.Data[allKey].Trim());
                  Logger.Error(string.Format("{0} does not exist in sharedfolder", (object) requestData.Data[allKey]));
                }
              }
              catch (Exception ex)
              {
                Logger.Error(string.Format("Error Occured, Err: {0}", (object) ex.ToString()));
                responseStringFailure = responseStringFailure != null ? responseStringFailure + "\n" + Path.Combine(userCopyDir, requestData.Data[allKey].Trim()) : Path.Combine(userCopyDir, requestData.Data[allKey].Trim());
              }
            }
            if (responseStringSuccess != null)
              HTTPHandler.SendSysTrayNotification("Successfully copied files:", "success", responseStringSuccess);
            if (responseStringFailure == null)
              return;
            HTTPHandler.SendSysTrayNotification("Cannot copy files:", "error", responseStringFailure);
          }
          else
          {
            Logger.Info("User cancelled browser dialog");
            foreach (string allKey in requestData.Data.AllKeys)
            {
              try
              {
                System.IO.File.Delete(Path.Combine(bstSharedFolder, requestData.Data[allKey].Trim()));
              }
              catch (Exception ex)
              {
                Logger.Error(string.Format("Error Occured, Err : {0}", (object) ex.ToString()));
              }
            }
          }
        }));
      }));
    }

    public static void SendSingleFileToWindows(RequestData requestData, HttpListenerResponse res)
    {
      DialogResult result = DialogResult.Cancel;
      string fileKey = (string) null;
      string[] allKeys = requestData.Data.AllKeys;
      int index = 0;
      if (index < allKeys.Length)
        fileKey = allKeys[index];
      string responseStringSuccess = (string) null;
      string responseStringFailure = (string) null;
      string bstSharedFolder = RegistryStrings.SharedFolderDir;
      string fileName = (string) null;
      if (fileKey == null)
        return;
      string ext = Path.GetExtension(requestData.Data[fileKey]).Replace(".", "");
      SaveFileDialog fileSaver;
      string userDefinedName;
      UIHelper.RunOnUIThread((Control) VMWindow.Instance, (UIHelper.Action) (() =>
      {
        Logger.Debug(string.Format("File Extension = {0}", (object) ext));
        fileSaver = new SaveFileDialog()
        {
          Filter = ext + " files (*." + ext + ")| *." + ext,
          AddExtension = true,
          Title = "Save File",
          AutoUpgradeEnabled = true,
          CheckPathExists = true,
          DefaultExt = ext,
          InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal),
          FileName = requestData.Data[fileKey],
          OverwritePrompt = true,
          ValidateNames = true
        };
        result = fileSaver.ShowDialog((IWin32Window) VMWindow.Instance);
        fileName = fileSaver.FileName;
        ThreadPool.QueueUserWorkItem((WaitCallback) (obj =>
        {
          if (result.Equals((object) DialogResult.OK))
          {
            userDefinedName = fileName;
            Logger.Info(string.Format("User Selected {0} Path", (object) userDefinedName));
            try
            {
              Logger.Info("Key: {0}, Value: {1}", (object) fileKey, (object) requestData.Data[fileKey]);
              if (System.IO.File.Exists(Path.Combine(bstSharedFolder, requestData.Data[fileKey].Trim())))
              {
                if (string.Compare(userDefinedName, Path.Combine(bstSharedFolder, requestData.Data[fileKey]), false) != 0)
                {
                  if (System.IO.File.Exists(userDefinedName))
                    System.IO.File.Delete(userDefinedName);
                  System.IO.File.Move(Path.Combine(bstSharedFolder, requestData.Data[fileKey].Trim()), userDefinedName);
                }
                responseStringSuccess = userDefinedName;
              }
              else
              {
                responseStringFailure = userDefinedName;
                Logger.Error(string.Format("{0} does not exist in sharedfolder", (object) requestData.Data[fileKey]));
              }
            }
            catch (Exception ex)
            {
              Logger.Error(string.Format("Error Occured, Err: {0}", (object) ex.ToString()));
              responseStringFailure = userDefinedName;
            }
            if (responseStringSuccess != null)
            {
              HTTPHandler.SendSysTrayNotification("Successfully copied files:", "success", responseStringSuccess);
            }
            else
            {
              if (responseStringFailure == null)
                return;
              HTTPHandler.SendSysTrayNotification("Cannot copy files:", "error", responseStringFailure);
            }
          }
          else
          {
            Logger.Info("User cancelled save file dialog");
            try
            {
              System.IO.File.Delete(Path.Combine(bstSharedFolder, requestData.Data[fileKey].Trim()));
            }
            catch (Exception ex)
            {
              Logger.Error(string.Format("Error Occured, Err : {0}", (object) ex.ToString()));
            }
          }
        }));
      }));
    }

    public static void SendSysTrayNotification(string title, string status, string message)
    {
      HTTPUtils.SendRequestToAgentAsync("showTrayNotification", new Dictionary<string, string>()
      {
        {
          nameof (message),
          message
        },
        {
          nameof (title),
          title
        },
        {
          nameof (status),
          status
        }
      }, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
    }

    private static void PingVMHandler(HttpListenerRequest req, HttpListenerResponse res)
    {
      JArray jarray = new JArray();
      JObject jobject = new JObject();
      try
      {
        if (HTTPUtils.ParseRequest(req).RequestVmName.Equals(MultiInstanceStrings.VmName))
        {
          bool flag = false;
          try
          {
            flag = AndroidService.HDQuitEvent.WaitOne(0);
          }
          catch (AbandonedMutexException ex)
          {
            flag = true;
          }
          catch
          {
          }
          if (flag)
          {
            jobject.Add("success", (JToken) false);
            jobject.Add("status", (JToken) "stopped");
            jarray.Add((JToken) jobject);
            HTTPUtils.Write(jarray.ToString(Formatting.None), res);
          }
          else if (Utils.CheckIfGuestReady(MultiInstanceStrings.VmName, 1))
          {
            jobject.Add("success", (JToken) true);
            jobject.Add("status", (JToken) "ready");
            jarray.Add((JToken) jobject);
            HTTPUtils.Write(jarray.ToString(Formatting.None), res);
          }
          else
          {
            jobject.Add("success", (JToken) false);
            jobject.Add("status", (JToken) "started");
            jarray.Add((JToken) jobject);
            HTTPUtils.Write(jarray.ToString(Formatting.None), res);
          }
        }
        else
        {
          jobject.Add("success", (JToken) false);
          jobject.Add("status", (JToken) "invalid vmname");
          jarray.Add((JToken) jobject);
          HTTPUtils.Write(jarray.ToString(Formatting.None), res);
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in Server PingVM. Err : " + ex.ToString());
        Logger.Error(ex.ToString());
        jobject.Add("success", (JToken) false);
        jobject.Add("status", (JToken) ex.Message);
        jarray.Add((JToken) jobject);
        HTTPUtils.Write(jarray.ToString(Formatting.None), res);
      }
    }

    private static void PingHandler(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        HTTPHandler.WriteSuccessJsonWithVmName(res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in Server Ping. Err : " + ex.ToString());
        HTTPHandler.WriteErrorJson(ex.Message, res);
      }
    }

    private static void InputMapperFilesDownloaded(
      HttpListenerRequest req,
      HttpListenerResponse res)
    {
      try
      {
        if (!Opt.Instance.sysPrep && Oem.Instance.IsSendGameManagerRequest)
        {
          JObject jobject1 = JObject.Parse(req.QueryString["data"]);
          Dictionary<string, string> data = new Dictionary<string, string>()
          {
            {
              "packageName",
              jobject1["pkg_name"].ToString()
            }
          };
          JObject jobject2 = JObject.Parse(jobject1["response"].ToString().Trim());
          if (jobject2.Property("macro") != null && jobject2["macro"].ToObject<bool>())
            data["macro"] = true.ToString();
          if (jobject2.Property("is_video_present") != null)
            data["videoPresent"] = jobject2["is_video_present"].ToString();
          HTTPUtils.SendRequestToClient("appInfoUpdated", data, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
        }
        HTTPHandler.WriteSuccessJsonWithVmName(res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in InputMapperFilesDownloaded. Err : " + ex.ToString());
        HTTPHandler.WriteErrorJson(ex.Message, res);
      }
    }

    private static void HandleSoftControlBarEvent(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        string str = req.QueryString["visible"];
        if (str == null)
          return;
        InputMapper.SoftControlBarVisible(str != "0");
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in HandleSoftControlBarEvent. Err : " + ex.ToString());
      }
    }

    private static void CloseScreen(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        HTTPHandler.ShutdownPlayer(HTTPUtils.ParseRequest(req).RequestVmName);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in CloseScreen. Err : " + ex.ToString());
      }
    }

    private static void GoBack(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        VmCmdHandler.RunCommand("back", MultiInstanceStrings.VmName, "bgp");
        HTTPHandler.WriteSuccessJson(res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in Server BackPress. Err : " + ex.ToString());
        HTTPHandler.WriteErrorJson(ex.Message, res);
      }
    }

    private static void ShareScreenshot(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        VMWindow.Instance.HandleShareButtonClicked();
        HTTPHandler.WriteSuccessJson(res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in Server ShareScreenshot. Err : " + ex.ToString());
        HTTPHandler.WriteErrorJson(ex.Message, res);
      }
    }

    private static void SetParent(HttpListenerRequest req, HttpListenerResponse res)
    {
      UIHelper.RunOnUIThread((Control) VMWindow.Instance, (UIHelper.Action) (() =>
      {
        try
        {
          RequestData request = HTTPUtils.ParseRequest(req);
          IntPtr mParentHanlde = new IntPtr(Convert.ToInt32(request.Data["ParentHandle"]));
          bool flag = false;
          if (request.Data["isMute"] != null)
            flag = Convert.ToBoolean(request.Data["isMute"]);
          Rectangle rect = new Rectangle()
          {
            X = Convert.ToInt32(request.Data["X"]),
            Y = Convert.ToInt32(request.Data["Y"]),
            Width = Convert.ToInt32(request.Data["Width"]),
            Height = Convert.ToInt32(request.Data["Height"])
          };
          int num = flag ? 1 : 0;
          HTTPHandler.ReparentWindow(mParentHanlde, rect, num != 0);
          HTTPHandler.InitGamePad(req, res);
          HTTPUtils.Write(new JArray()
          {
            (JToken) new JObject()
            {
              {
                "success",
                (JToken) true
              },
              {
                "frontendhandle",
                (JToken) VMWindow.Instance.GetHandle().ToInt32()
              }
            }
          }.ToString(Formatting.None), res);
        }
        catch (Exception ex)
        {
          Logger.Error("Exception in Server SetParent. Err : " + ex.ToString());
          HTTPHandler.WriteErrorJson(ex.Message, res);
        }
      }));
    }

    private static void ReparentWindow(IntPtr mParentHanlde, Rectangle rect, bool isMute)
    {
      IntPtr handle = VMWindow.Instance.GetHandle();
      if (mParentHanlde != IntPtr.Zero)
      {
        VMWindow.Instance.isStreamingModeEnabled = false;
        int dwNewLong = (int) ((long) (InteropWindow.GetWindowLong(handle, -16) & -13565953) | 1073741824L);
        InteropWindow.SetWindowLong(handle, -16, dwNewLong);
        InteropWindow.SetParent(handle, mParentHanlde);
        VMWindow.Instance.ShowVmWindow(rect.Width, rect.Height);
        InteropWindow.SetWindowPos(handle, IntPtr.Zero, rect.X, rect.Y, rect.Width, rect.Height, 16384U);
        VMWindow.Instance.Text = Oem.Instance.CommonAppTitleText + MultiInstanceStrings.VmName;
      }
      else
      {
        VMWindow.Instance.isStreamingModeEnabled = true;
        int dwNewLong = (int) ((long) InteropWindow.GetWindowLong(handle, -16) & 3221225471L | 13565952L);
        InteropWindow.SetWindowLong(handle, -16, dwNewLong);
        InteropWindow.SetParent(handle, mParentHanlde);
        VMWindow.Instance.SetAspectRationAndMinMaxOfForm();
        VMWindow.Instance.ShowVmWindow(rect.Width + (int) VMWindow.Instance.widthDiff, rect.Height + (int) VMWindow.Instance.heightDiff);
        InteropWindow.SetWindowPos(handle, IntPtr.Zero, rect.X, rect.Y, rect.Width, rect.Height, 16448U);
        VMWindow.Instance.Text = LocaleStrings.GetLocalizedString("STRING_STREAMING_WINDOW_TITLE", "");
      }
      VMWindow.Instance.PostVmWindowShowTasks(isMute);
    }

    private static void RefreshWindow(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        VMWindow.Instance.HandleFrontendActivated();
      }
      catch (Exception ex)
      {
        HTTPHandler.WriteErrorJson(ex.ToString(), res);
      }
    }

    private static void DeactivateFrontend(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        VMWindow.Instance.HandleFrontendDeactivated();
      }
      catch (Exception ex)
      {
        HTTPHandler.WriteErrorJson(ex.ToString(), res);
      }
    }

    private static void ShowWindow(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        UIHelper.RunOnUIThread((Control) VMWindow.Instance, (UIHelper.Action) (() => VMWindow.Instance.HandleUserShowWindow(0, 0)));
        HTTPHandler.WriteSuccessJson(res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in Server ShowWindow. Err : " + ex.ToString());
        HTTPHandler.WriteErrorJson(ex.Message, res);
      }
    }

    private static void SwitchOrientation(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        string s = req.QueryString["orientation"];
        string empty = string.Empty;
        int orientationFromAndroid = int.Parse(s);
        try
        {
          empty = req.QueryString["package"];
        }
        catch
        {
        }
        if (req.QueryString["stopFurtherOrientationChange"] != null)
        {
          if (VMWindow.Instance.AppOrientationDict.ContainsKey(empty))
            VMWindow.Instance.AppOrientationDict[empty] = s;
          else if (string.Equals(req.QueryString["stopFurtherOrientationChange"], "1"))
          {
            VMWindow.Instance.AppOrientationDict.Add(empty, s);
          }
          else
          {
            if (empty == VMWindow.Instance.mLlastPackageName && VMWindow.Instance.mLastCallFromAndroid && string.Equals(req.QueryString["isPreviousSelectedTabWeb"], "0"))
              return;
            if (VMWindow.Instance.mLlastOrientationFromAndroid != orientationFromAndroid && string.Compare(empty, "Home", true) != 0)
              orientationFromAndroid = VMWindow.Instance.mLlastOrientationFromAndroid;
          }
          VMWindow.Instance.mLastCallFromAndroid = false;
        }
        else
        {
          VMWindow.Instance.mLastCallFromAndroid = true;
          VMWindow.Instance.mLlastOrientationFromAndroid = orientationFromAndroid;
          if (VMWindow.Instance.AppOrientationDict.ContainsKey(empty))
            return;
        }
        VMWindow.Instance.mLlastPackageName = empty;
        try
        {
          Logger.Info("Changing orientation to {0}", (object) orientationFromAndroid);
          LayoutManager.OrientationHandler(orientationFromAndroid);
          Dictionary<string, string> data = new Dictionary<string, string>()
          {
            {
              "is_portrait",
              Convert.ToString(LayoutManager.mEmulatedPortraitMode)
            },
            {
              "package",
              empty
            }
          };
          try
          {
            HTTPUtils.SendRequestToClient("changeOrientaion", data, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
          }
          catch (Exception ex)
          {
            Logger.Error("Failed to send orientaion change event to client... err : " + ex.ToString());
          }
          HTTPHandler.ImapHandleOrientation(orientationFromAndroid);
        }
        catch (Exception ex)
        {
          Logger.Info("Got exec in orientation change");
          Logger.Info(ex.ToString());
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in SwitchOrientation. Err : " + ex.ToString());
      }
    }

    private static void QuitFrontend(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        HTTPHandler.ShutdownPlayer(HTTPUtils.ParseRequest(req).RequestVmName);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in QuitFrontend. Err : " + ex.ToString());
      }
    }

    private static void Shutdown(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        HTTPHandler.ShutdownPlayer(HTTPUtils.ParseRequest(req).RequestVmName);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in Shutdown. Err : " + ex.ToString());
      }
    }

    private static void ShutdownPlayer(string vmName)
    {
      if (HTTPHandler.mIsPlayerShuttingDown)
        return;
      lock (HTTPHandler.sPlayerShuttingDownLockObject)
      {
        if (HTTPHandler.mIsPlayerShuttingDown)
          return;
        HTTPHandler.mIsPlayerShuttingDown = true;
        try
        {
          bool createdNew;
          using (Mutex mutex = new Mutex(true, "Global\\BlueStacks_PlayerClosing_Lockbgp", out createdNew))
          {
            if (!createdNew)
            {
              try
              {
                Logger.Info("Shutdown waiting " + vmName);
                mutex.WaitOne(-1);
              }
              catch (Exception ex)
              {
                Logger.Error("Shutdown mutex wait exited: " + ex.Message);
              }
            }
            Logger.Info("Shutdown started " + vmName);
            VMWindow.Instance.Close();
            Logger.Info("Shutdown ended " + vmName);
            mutex.ReleaseMutex();
          }
        }
        catch (Exception ex)
        {
          Logger.Error("Exception in Shutdown player. Err : " + ex.ToString());
        }
        finally
        {
          if (Program.sFrontendLock != null)
          {
            Logger.Info("Releasing frontend lock");
            Program.sFrontendLock.Close();
            Program.sFrontendLock = (Mutex) null;
          }
          Environment.Exit(0);
        }
      }
    }

    internal static void SendSetFrontendPositionRequest(int width, int height)
    {
      new Thread((ThreadStart) (() =>
      {
        try
        {
          HTTPUtils.SendRequestToClient("setfrontendposition", new Dictionary<string, string>()
          {
            {
              nameof (width),
              width.ToString()
            },
            {
              nameof (height),
              height.ToString()
            }
          }, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
        }
        catch (Exception ex)
        {
          Logger.Error("Exception in sending request setfrontendposition to gamemanager. Err : {0}", (object) ex.ToString());
        }
      }))
      {
        IsBackground = true
      }.Start();
    }

    private static void ToggleScreen(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        UIHelper.RunOnUIThread((Control) VMWindow.Instance, (UIHelper.Action) (() => LayoutManager.ToggleFullScreen()));
        HTTPHandler.WriteSuccessJson(res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in Server ToggleScreen: " + ex.ToString());
        HTTPHandler.WriteErrorJson(ex.Message, res);
      }
    }

    private static void StartRecordingCombo(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        InputMapper.StartRecording();
        HTTPHandler.WriteSuccessJson(res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in StartRecordingCombo: " + ex.ToString());
        HTTPHandler.WriteErrorJson(ex.Message, res);
      }
    }

    private static void PauseRecordingCombo(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        InputMapper.PauseRecording();
        HTTPHandler.WriteSuccessJson(res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in PauseRecordingCombo. err:" + ex.ToString());
        HTTPHandler.WriteErrorJson(ex.Message, res);
      }
    }

    private static void StopRecordingCombo(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        int size = 0;
        StringBuilder sb = (StringBuilder) null;
        InputMapper.StopRecording((StringBuilder) null, ref size);
        sb = new StringBuilder(size);
        InputMapper.StopRecording(sb, ref size);
        new Thread((ThreadStart) (() =>
        {
          try
          {
            HTTPUtils.SendRequestToClient("saveComboEvents", new Dictionary<string, string>()
            {
              {
                "events",
                sb.ToString()
              }
            }, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
          }
          catch (Exception ex)
          {
            Logger.Error("Exception in sending request setfrontendposition to gamemanager. Err : {0}", (object) ex.ToString());
          }
        }))
        {
          IsBackground = true
        }.Start();
        HTTPHandler.WriteSuccessJson(res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in StopRecordingCombo: " + ex.ToString());
        HTTPHandler.WriteErrorJson(ex.Message, res);
      }
    }

    private static void SendGLWindowSize(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        LayoutManager.UpdateGlWindowSize = Convert.ToBoolean(HTTPUtils.ParseRequest(req).Data["updateSize"]);
        HTTPHandler.WriteSuccessJson(res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in SendGLWindowSize: " + ex.ToString());
        HTTPHandler.WriteErrorJson(ex.Message, res);
      }
    }

    private static void InitMacroPlayback(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        MacroManager.Instance.InitMacroPlayback(HTTPUtils.ParseRequest(req).Data["scriptFilePath"].ToString());
        HTTPHandler.WriteSuccessJson(res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in InitMacroPlayback: " + ex.ToString());
        HTTPHandler.WriteErrorJson(ex.Message, res);
      }
    }

    private static void RunMacroUnit(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        MacroManager.Instance.RunMacroUnit();
        HTTPHandler.WriteSuccessJson(res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in RunMacroUnit: " + ex.ToString());
        HTTPHandler.WriteErrorJson(ex.Message, res);
      }
    }

    private static void StopMacroPlayback(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        MacroManager.Instance.StopMacroPlayback();
        HTTPHandler.WriteSuccessJson(res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in StopMacroPlayback: " + ex.ToString());
        HTTPHandler.WriteErrorJson(ex.Message, res);
      }
    }

    private static void FarmModeHandler(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        bool boolean = Convert.ToBoolean(request.Data["enable"]);
        int int32 = Convert.ToInt32(request.Data["fps"]);
        Opengl.ToggleFarmMode(boolean);
        if (boolean)
          Utils.SendChangeFPSToInstanceASync(MultiInstanceStrings.VmName, int32, "bgp");
        else
          Utils.SendChangeFPSToInstanceASync(MultiInstanceStrings.VmName, int.MaxValue, "bgp");
        BstHttpClient.Get(string.Format("http://127.0.0.1:{0}/farmmodevalue?d={1}", (object) Utils.GetBstCommandProcessorPort(MultiInstanceStrings.VmName), boolean ? (object) "1" : (object) "0"), (Dictionary<string, string>) null, false, MultiInstanceStrings.VmName, 0, 1, 0, false, "bgp");
        HTTPHandler.WriteSuccessJson(res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in FarmModeHandler: " + ex.ToString());
        HTTPHandler.WriteErrorJson(ex.Message, res);
      }
    }

    public static void ToggleGamepadButton(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        InputMapper.ToggleGamepadButton(Convert.ToBoolean(HTTPUtils.ParseRequest(req).Data["enable"].ToString()));
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in ToggleGamepadButton: " + ex.ToString());
        HTTPHandler.WriteErrorJson(ex.Message, res);
      }
    }

    public static void IsAppPlayerRooted(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        HTTPUtils.ParseRequest(req);
        bool flag = VBoxBridgeService.Instance.CheckIfAppPlayerRooted();
        Logger.Info("IsRooted = " + flag.ToString());
        HTTPUtils.Write(new JArray()
        {
          (JToken) new JObject()
          {
            {
              "success",
              (JToken) true
            },
            {
              "isRooted",
              (JToken) flag.ToString()
            },
            {
              "vmname",
              (JToken) MultiInstanceStrings.VmName
            }
          }
        }.ToString(Formatting.None), res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in IsAppPlayerRooted err: {0}", (object) ex);
        HTTPHandler.WriteErrorJson(ex.Message, res);
      }
    }

    public static void DeviceProvisionedReceived(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        HTTPUtils.ParseRequest(req);
        if (HTTPHandler.IsDeviceProvisionReceived)
          HTTPHandler.WriteSuccessJson(res);
        else
          HTTPHandler.WriteErrorJson("Not provisioned", res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in DeviceProvisionedHandler: " + ex.ToString());
        HTTPHandler.WriteErrorJson(ex.Message, res);
      }
    }

    public static void DeviceProvisionedHandler(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        Logger.Info("Device provisioned player");
        RequestData request = HTTPUtils.ParseRequest(req);
        Stats.SendUnifiedInstallStats("device_provisioned", "", "Android", "");
        HTTPHandler.IsDeviceProvisionReceived = true;
        if (!Opt.Instance.sysPrep && Oem.Instance.IsSendGameManagerRequest)
          HTTPUtils.SendRequestToClient("deviceProvisioned", (Dictionary<string, string>) null, request.RequestVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
        HTTPHandler.WriteSuccessJson(res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in DeviceProvisionedHandler: " + ex.ToString());
        HTTPHandler.WriteErrorJson(ex.Message, res);
      }
    }

    public static void GoogleSigninHandler(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        HTTPUtils.SendRequestToClient("googleSignin", new Dictionary<string, string>()
        {
          {
            "email",
            req.QueryString["email"]
          }
        }, request.RequestVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
        HTTPHandler.WriteSuccessJson(res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in GoogleSigninHandler: " + ex.ToString());
        HTTPHandler.WriteErrorJson(ex.Message, res);
      }
    }

    private static void SetFullScreenState(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        string str = HTTPUtils.ParseRequest(req).Data["isFullscreen"];
        VMWindow.Instance.IsFullscreen = bool.Parse(str);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in setting Fullscreen state... Err : " + ex.ToString());
      }
    }

    private static void UpdateMacroShortcutsDict(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        VMWindow.Instance.mMacroShortcutsDict.Clear();
        foreach (string allKey in request.Data.AllKeys)
          VMWindow.Instance.mMacroShortcutsDict.Add(allKey, request.Data[allKey]);
        if (VMWindow.Instance.mMacroShortcutsDict != null)
          HTTPHandler.WriteSuccessJson(res);
        else
          HTTPHandler.WriteErrorJson("Macroshortcuts dict is null, see inner exception", res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in UpdateMacroShortcutsDict... Err : " + ex.ToString());
        HTTPHandler.WriteErrorJson(ex.ToString(), res);
      }
    }

    private static void ChangeImei(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        string guest = HTTPUtils.SendRequestToGuest("changeimei", new Dictionary<string, string>()
        {
          {
            "imei",
            HTTPUtils.ParseRequest(req).Data["imei"].ToString()
          }
        }, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
        Logger.Info("resp from android.." + guest);
        JObject jobject = JObject.Parse(guest);
        if (jobject["result"].ToString() == "ok")
          HTTPHandler.WriteSuccessJson(res);
        else
          HTTPHandler.WriteErrorJson(jobject["value"].ToString(), res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in sending imei to android... Err : " + ex.ToString());
        HTTPHandler.WriteErrorJson(ex.ToString(), res);
      }
    }

    private static void ForceExit(HttpListenerRequest req, HttpListenerResponse res)
    {
      if (Program.sFrontendLock != null)
      {
        Logger.Info("Releasing frontend lock");
        Program.sFrontendLock.Close();
        Program.sFrontendLock = (Mutex) null;
      }
      Environment.Exit(-9);
    }

    private static void BootCompletedHandler(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        Logger.Info("BOOT_STAGE: Sending boot completed event received from BootCompleteHandler");
        AndroidBootUp.GuestBootCompletedEvent((object) null, new EventArgs());
        HTTPHandler.WriteSuccessJson(res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in BootCompleteHandler: " + ex.ToString());
        HTTPHandler.WriteErrorJson(ex.Message, res);
      }
    }

    public static void CheckIfGuestBootedHandler(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        HTTPUtils.ParseRequest(req);
        if (AndroidBootUp.sHasNotifiedClientForGuestBooted)
          HTTPHandler.WriteSuccessJson(res);
        else
          HTTPHandler.WriteErrorJson("Guest not booted yet", res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in CheckIfGuestBootedHandler err: {0}", (object) ex);
        HTTPHandler.WriteErrorJson(ex.Message, res);
      }
    }

    public static void WriteErrorJson(string reason, HttpListenerResponse res)
    {
      HTTPUtils.Write(new JArray()
      {
        (JToken) new JObject()
        {
          {
            "success",
            (JToken) false
          },
          {
            nameof (reason),
            (JToken) reason
          }
        }
      }.ToString(Formatting.None), res);
    }

    private static void WriteSuccessJsonWithVmName(HttpListenerResponse res)
    {
      HTTPUtils.Write(new JArray()
      {
        (JToken) new JObject()
        {
          {
            "success",
            (JToken) true
          },
          {
            "vmname",
            (JToken) MultiInstanceStrings.VmName
          }
        }
      }.ToString(Formatting.None), res);
    }

    public static void WriteSuccessJson(HttpListenerResponse res)
    {
      HTTPUtils.Write(new JArray()
      {
        (JToken) new JObject()
        {
          {
            "success",
            (JToken) true
          }
        }
      }.ToString(Formatting.None), res);
    }
  }
}
