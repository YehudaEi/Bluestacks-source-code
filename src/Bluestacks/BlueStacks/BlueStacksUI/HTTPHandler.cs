// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.HTTPHandler
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;

namespace BlueStacks.BlueStacksUI
{
  internal class HTTPHandler
  {
    private static object sLockObject = new object();
    internal static string lastPackage = string.Empty;
    private static Dictionary<string, string> dictFileNamesPackageName = new Dictionary<string, string>();
    private static bool mSendGamepadStats = false;
    private static object syncRoot = new object();
    private static string mPreviousActivityReported = "";

    private static void WriteSuccessJsonArray(HttpListenerResponse res)
    {
      JArray jarray = new JArray();
      JObject jobject = new JObject();
      jobject.Add((object) new JProperty("success", (object) true));
      jarray.Add((JToken) jobject);
      HTTPUtils.Write(jarray.ToString(Formatting.None), res);
    }

    private static void WriteErrorJsonArray(string reason, HttpListenerResponse res)
    {
      JArray jarray = new JArray();
      JObject jobject = new JObject();
      jobject.Add((object) new JProperty("success", (object) false));
      jobject.Add((object) new JProperty(nameof (reason), (object) reason));
      jarray.Add((JToken) jobject);
      HTTPUtils.Write(jarray.ToString(Formatting.None), res);
    }

    private static void WriteErrorJSONObjectWithoutReason(HttpListenerResponse res)
    {
      HTTPUtils.Write(new JObject()
      {
        {
          "success",
          (JToken) false
        }
      }.ToString(Formatting.None), res);
    }

    public static void PingHandler(HttpListenerRequest req, HttpListenerResponse res)
    {
      HTTPHandler.WriteSuccessJsonWithVmName(HTTPUtils.ParseRequest(req).RequestVmName, res);
    }

    internal static void EnableWndProcLogging(HttpListenerRequest _1, HttpListenerResponse _2)
    {
      try
      {
        WindowWndProcHandler.isLogWndProc = !WindowWndProcHandler.isLogWndProc;
        Logger.Info("Got request for EnableWndProcLogging" + WindowWndProcHandler.isLogWndProc.ToString());
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in set EnableWndProcLogging... Err : " + ex.ToString());
      }
    }

    internal static void EnableKeyboardHookLogging(HttpListenerRequest _1, HttpListenerResponse _2)
    {
      try
      {
        GlobalKeyBoardMouseHooks.sIsEnableKeyboardHookLogging = !GlobalKeyBoardMouseHooks.sIsEnableKeyboardHookLogging;
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in set EnableKeyboardHookLogging... Err : " + ex.ToString());
      }
    }

    internal static void EnableDebugLogs(HttpListenerRequest _1, HttpListenerResponse res)
    {
      try
      {
        Logger.EnableDebugLogs();
        HTTPHandler.WriteSuccessJsonArray(res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in EnableDebugLogs... Err : " + ex.ToString());
        HTTPHandler.WriteErrorJSONObjectWithoutReason(res);
      }
    }

    public static void SendAppDisplayed(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        JObject jobject = new JObject();
        if (BlueStacksUIUtils.DictWindows.ContainsKey(request.RequestVmName))
          jobject.Add("success", (JToken) BlueStacksUIUtils.DictWindows[request.RequestVmName].mAppHandler.mAppDisplayedOccured);
        HTTPUtils.Write(jobject.ToString(Formatting.None), res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in Server SendAppDisplayed. Err : " + ex.ToString());
        HTTPHandler.WriteErrorJSONObjectWithoutReason(res);
      }
    }

    internal static void RestartFrontend(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        if (!BlueStacksUIUtils.DictWindows.ContainsKey(request.RequestVmName))
          return;
        BlueStacksUIUtils.DictWindows[request.RequestVmName].RestartFrontend();
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in Server RestartFrontend. Err : " + ex.ToString());
        HTTPHandler.WriteErrorJSONObjectWithoutReason(res);
      }
    }

    internal static void GCCollect(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        HTTPHandler.WriteSuccessJsonWithVmName(request.RequestVmName, res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in Server GCCollect. Err : " + ex.ToString());
        HTTPHandler.WriteErrorJSONObjectWithoutReason(res);
      }
    }

    public static void IsBlueStacksUIVisible(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        JObject jobject = new JObject();
        if (BlueStacksUIUtils.DictWindows.ContainsKey(request.RequestVmName))
          jobject.Add("success", (JToken) BlueStacksUIUtils.DictWindows[request.RequestVmName].IsVisible);
        HTTPUtils.Write(jobject.ToString(Formatting.None), res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in Server IsBlueStacksUIVisible. Err : " + ex.ToString());
        HTTPHandler.WriteErrorJSONObjectWithoutReason(res);
      }
    }

    internal static void ToggleFarmMode(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        CommonHandlers.ToggleFarmMode(bool.Parse(request.Data["state"]));
        HTTPHandler.WriteSuccessJsonWithVmName(request.RequestVmName, res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in Server ToggleFarmMode. Err : " + ex.ToString());
        HTTPHandler.WriteErrorJSONObjectWithoutReason(res);
      }
    }

    internal static void ToggleStreamingMode(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        bool state = bool.Parse(request.Data["state"]);
        if (BlueStacksUIUtils.DictWindows.ContainsKey(request.RequestVmName))
        {
          MainWindow mWindow = BlueStacksUIUtils.DictWindows[request.RequestVmName];
          mWindow.Dispatcher.Invoke((Delegate) (() =>
          {
            mWindow.mTopBar.mPreferenceDropDownControl.ToggleStreamingMode(state);
            mWindow.mFrontendHandler.ToggleStreamingMode(state);
          }));
        }
        HTTPHandler.WriteSuccessJsonWithVmName(request.RequestVmName, res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in Server ToggleStreamingMode. Err : " + ex.ToString());
        HTTPHandler.WriteErrorJSONObjectWithoutReason(res);
      }
    }

    internal static void GamepadGuidanceButtonHandler(
      HttpListenerRequest req,
      HttpListenerResponse res)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        if (BlueStacksUIUtils.DictWindows.ContainsKey(request.RequestVmName))
        {
          MainWindow mWindow = BlueStacksUIUtils.DictWindows[request.RequestVmName];
          if (mWindow != BlueStacksUIUtils.LastActivatedWindow || RegistryManager.Instance.GamepadDetectionEnabled && mWindow.IsGamepadConnected && mWindow.mTopBar.mAppTabButtons.SelectedTab.mIsNativeGamepadEnabledForApp)
            return;
          mWindow.Dispatcher.Invoke((Delegate) (() =>
          {
            if (KMManager.CheckIfKeymappingWindowVisible(true))
            {
              KMManager.CloseWindows();
              mWindow.mSidebar.UpdateImage("sidebar_gameguide", "sidebar_gameguide");
            }
            else
            {
              if (KeymapCanvasWindow.sIsDirty)
                return;
              KMManager.HandleInputMapperWindow(mWindow, "gamepad");
            }
          }));
        }
        HTTPHandler.WriteSuccessJsonWithVmName(request.RequestVmName, res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in GamepadGuidanceButtonHandler. Err : " + ex.ToString());
        HTTPHandler.WriteErrorJSONObjectWithoutReason(res);
      }
    }

    internal static void SetCurrentVolumeFromAndroidHandler(
      HttpListenerRequest req,
      HttpListenerResponse _)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        string requestVmName = request.RequestVmName;
        int int32 = Convert.ToInt32(request.Data["volume"], (IFormatProvider) CultureInfo.InvariantCulture);
        if (!BlueStacksUIUtils.DictWindows.ContainsKey(requestVmName))
          return;
        BlueStacksUIUtils.DictWindows[requestVmName].Utils.SetVolumeLevelFromAndroid(int32);
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to set volume level. Er : " + ex.ToString());
      }
    }

    internal static void ReinitRegistry(HttpListenerRequest req, HttpListenerResponse res)
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

    internal static void UpdateCrc(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData requestData = HTTPUtils.ParseRequest(req);
        string requestVmName = requestData.RequestVmName;
        if (!BlueStacksUIUtils.DictWindows.ContainsKey(requestVmName))
          return;
        BlueStacksUIUtils.DictWindows[requestVmName].Dispatcher.Invoke((Delegate) (() =>
        {
          uint result1;
          float result2;
          float result3;
          if (!uint.TryParse(requestData.Data["Crc"], out result1) || !float.TryParse(requestData.Data["X"], out result2) || !float.TryParse(requestData.Data["Y"], out result3))
            return;
          string text = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "X: {0}   Y: {1}   Crc: {2}", (object) result2.ToString((IFormatProvider) CultureInfo.InvariantCulture), (object) result3.ToString((IFormatProvider) CultureInfo.InvariantCulture), (object) result1.ToString("X", (IFormatProvider) CultureInfo.InvariantCulture));
          Logger.Info("IMAGEPICKER: " + text);
          int num = (int) System.Windows.Forms.MessageBox.Show(text);
          System.Windows.Forms.Clipboard.SetText(text);
        }));
      }
      catch (Exception ex)
      {
        Logger.Error("Failed in UpdateCrc. Err : " + ex.ToString());
      }
    }

    internal static void ConfigFileChanged(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        string requestVmName = request.RequestVmName;
        string config = request.Data["config"];
        if (!BlueStacksUIUtils.DictWindows.ContainsKey(requestVmName))
          return;
        MainWindow window = BlueStacksUIUtils.DictWindows[requestVmName];
        window.Dispatcher.Invoke((Delegate) (() => window.mTopBar.SetConfigIndicator(config)));
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to set GameInfo err : " + ex.ToString());
      }
    }

    internal static void CheckCallbackEnabledStatus(
      HttpListenerRequest req,
      HttpListenerResponse res)
    {
      JArray jarray = new JArray();
      JObject jobject = new JObject();
      try
      {
        string key = HTTPUtils.ParseRequest(req).Data["vmname"];
        if (!BlueStacksUIUtils.DictWindows.ContainsKey(key))
          return;
        MainWindow dictWindow = BlueStacksUIUtils.DictWindows[key];
        Logger.Info("Callback: vmname: " + key + " value: " + dictWindow.mCallbackEnabled);
        jobject.Add("success", (JToken) true);
        jobject.Add("Enabled", (JToken) dictWindow.mCallbackEnabled);
        jarray.Add((JToken) jobject);
        HTTPUtils.Write(jarray.ToString(Formatting.None), res);
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to get callback status. Err : " + ex.Message);
        jobject.Add("success", (JToken) false);
        jobject.Add("status", (JToken) ex.Message);
        jarray.Add((JToken) jobject);
        HTTPUtils.Write(jarray.ToString(Formatting.None), res);
      }
    }

    internal static void HideOverlayWhenIMEActive(HttpListenerRequest req, HttpListenerResponse res)
    {
      JArray jarray = new JArray();
      JObject jobject = new JObject();
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        string requestVmName = request.RequestVmName;
        bool hideOverlay = Convert.ToBoolean(request.Data["hideOverlay"], (IFormatProvider) CultureInfo.InvariantCulture);
        if (!BlueStacksUIUtils.DictWindows.ContainsKey(requestVmName))
          return;
        MainWindow window = BlueStacksUIUtils.DictWindows[requestVmName];
        if (!KMManager.dictOverlayWindow.ContainsKey(window) || KMManager.dictOverlayWindow[window] == null)
          return;
        BlueStacksUIUtils.DictWindows[requestVmName].Dispatcher.Invoke((Delegate) (() => KMManager.dictOverlayWindow[window].Opacity = hideOverlay ? 0.0 : 1.0));
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to get callback status. Err : " + ex.Message);
        jobject.Add("success", (JToken) false);
        jobject.Add("status", (JToken) ex.Message);
        jarray.Add((JToken) jobject);
        HTTPUtils.Write(jarray.ToString(Formatting.None), res);
      }
    }

    internal static void ShowImageUploadedInfo(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        string vmName = HTTPUtils.ParseRequest(req).RequestVmName;
        BlueStacksUIUtils.DictWindows[vmName].Dispatcher.Invoke((Delegate) (() =>
        {
          Logger.Info("Showing imageUpload popup");
          BlueStacksUIUtils.DictWindows[vmName].ShowImageUploadedPopup();
        }));
      }
      catch (Exception ex)
      {
        Logger.Error("Failed in ShowImageUploadedInfo. Err : " + ex.ToString());
      }
    }

    internal static void AddNotificationInDrawer(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        string vmName = request.Data["vmname"];
        string productDisplayName = request.Data["pkg"];
        string a = request.Data["app_name"];
        string str1 = request.Data["msg"];
        string str2 = request.Data["id"];
        string str3 = "bluestackslogo";
        string str4 = str3;
        JsonParser jsonParser = new JsonParser(vmName);
        try
        {
          if (!string.IsNullOrEmpty(str4))
          {
            AppInfo infoFromPackageName = jsonParser.GetAppInfoFromPackageName(productDisplayName);
            if (infoFromPackageName != null)
            {
              if (System.IO.File.Exists(Path.Combine(RegistryStrings.GadgetDir, infoFromPackageName.Img)))
              {
                str3 = Path.Combine(RegistryStrings.GadgetDir, infoFromPackageName.Img);
                string img = infoFromPackageName.Img;
              }
            }
            else
              Logger.Info("GetAppInfoFromAppName returns false");
          }
        }
        catch
        {
          Logger.Error("Error loading app icon file");
        }
        if (BlueStacksUIUtils.DictWindows.ContainsKey(vmName))
        {
          MainWindow dictWindow = BlueStacksUIUtils.DictWindows[vmName];
          GenericNotificationItem notificationItem = new GenericNotificationItem();
          notificationItem.Title = a;
          notificationItem.Message = str1;
          notificationItem.ShowRibbon = true;
          notificationItem.NotificationMenuImageUrl = str3;
          notificationItem.NotificationMenuImageName = str3;
          notificationItem.IsAndroidNotification = true;
          notificationItem.Id = str2;
          notificationItem.VmName = vmName;
          notificationItem.Package = productDisplayName;
          if (productDisplayName == null)
            productDisplayName = BlueStacks.Common.Strings.ProductDisplayName;
          if (string.Equals(a, BlueStacks.Common.Strings.ProductDisplayName, StringComparison.InvariantCultureIgnoreCase))
          {
            if (BlueStacksUIUtils.DictWindows[vmName].AppNotificationCountDictForEachVM.ContainsKey(BlueStacks.Common.Strings.ProductDisplayName))
              BlueStacksUIUtils.DictWindows[vmName].AppNotificationCountDictForEachVM[BlueStacks.Common.Strings.ProductDisplayName]++;
            else
              BlueStacksUIUtils.DictWindows[vmName].AppNotificationCountDictForEachVM.Add(BlueStacks.Common.Strings.ProductDisplayName, 1);
          }
          else if (BlueStacksUIUtils.DictWindows[vmName].AppNotificationCountDictForEachVM.ContainsKey(productDisplayName))
            BlueStacksUIUtils.DictWindows[vmName].AppNotificationCountDictForEachVM[productDisplayName]++;
          else
            BlueStacksUIUtils.DictWindows[vmName].AppNotificationCountDictForEachVM.Add(productDisplayName, 1);
          GenericNotificationManager.AddNewNotification(notificationItem, false);
          BlueStacksUIUtils.DictWindows[vmName].Dispatcher.Invoke((Delegate) (() =>
          {
            BlueStacksUIUtils.DictWindows[vmName].mTopBar.RefreshNotificationCentreButton();
            SerializableDictionary<string, GenericNotificationItem> notificationItems = GenericNotificationManager.GetNotificationItems((Predicate<GenericNotificationItem>) (x =>
            {
              if (x.IsDeleted)
                return false;
              return string.Equals(x.VmName, vmName, StringComparison.InvariantCulture) || !x.IsAndroidNotification;
            }));
            BlueStacksUIUtils.DictWindows[vmName].mTopBar.mNotificationDrawerControl.Populate(notificationItems);
            if (BlueStacksUIUtils.DictWindows[vmName].WindowState != WindowState.Minimized)
              return;
            BlueStacksUIUtils.SetWindowTaskbarIcon(BlueStacksUIUtils.DictWindows[vmName]);
          }));
        }
        if (!RegistryManager.Instance.IsNotificationSoundsActive || BlueStacksUIUtils.DictWindows[vmName].StaticComponents.mSelectedTabButton.mTabType == TabType.AppTab)
          return;
        MediaPlayer mediaPlayer = new MediaPlayer();
        mediaPlayer.Open(new Uri(Path.Combine(Path.Combine(RegistryManager.Instance.ClientInstallDir, "Assets"), "NotificationSound.wav")));
        mediaPlayer.Play();
      }
      catch (Exception ex)
      {
        Logger.Error("Failed in UpdateCrc. Err : " + ex.ToString());
      }
    }

    internal static void MarkNotificationInDrawer(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        string vmName = request.Data["vmname"];
        GenericNotificationManager.MarkNotification((IEnumerable<string>) new List<string>()
        {
          request.Data["id"]
        }, (System.Action<GenericNotificationItem>) (x => x.IsRead = true));
        BlueStacksUIUtils.DictWindows[vmName].Dispatcher.Invoke((Delegate) (() =>
        {
          BlueStacksUIUtils.DictWindows[vmName].mTopBar.RefreshNotificationCentreButton();
          SerializableDictionary<string, GenericNotificationItem> notificationItems = GenericNotificationManager.GetNotificationItems((Predicate<GenericNotificationItem>) (x =>
          {
            if (x.IsDeleted)
              return false;
            return string.Equals(x.VmName, vmName, StringComparison.InvariantCulture) || !x.IsAndroidNotification;
          }));
          BlueStacksUIUtils.DictWindows[vmName].mTopBar.mNotificationDrawerControl.Populate(notificationItems);
          if (BlueStacksUIUtils.DictWindows[vmName].WindowState != WindowState.Minimized)
            return;
          BlueStacksUIUtils.SetWindowTaskbarIcon(BlueStacksUIUtils.DictWindows[vmName]);
        }));
      }
      catch (Exception ex)
      {
        Logger.Error("Error in marking notification read: " + ex?.ToString());
      }
    }

    internal static void NCSetGameInfoOnTopBarHandler(
      HttpListenerRequest req,
      HttpListenerResponse res)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        string requestVmName = request.RequestVmName;
        string gameName = request.Data["game"];
        string characterName = request.Data["character"];
        MainWindow mWindow = BlueStacksUIUtils.DictWindows[requestVmName];
        if (BlueStacksUIUtils.DictWindows.ContainsKey(requestVmName))
        {
          mWindow.Dispatcher.Invoke((Delegate) (() =>
          {
            mWindow.mNCTopBar.mAppName.Text = gameName;
            mWindow.mNCTopBar.mAppName.ToolTip = (object) gameName;
            mWindow.mNCTopBar.mGamenameSeparator.Visibility = Visibility.Visible;
            mWindow.mNCTopBar.mCharacterName.Text = characterName;
            mWindow.mNCTopBar.mCharacterName.ToolTip = (object) characterName;
          }));
          HTTPHandler.WriteSuccessJsonWithVmName(requestVmName, res);
        }
        else
          HTTPHandler.WriteErrorJsonArray("Client Instance not running", res);
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to set GameInfo err : " + ex.ToString());
        HTTPHandler.WriteErrorJsonArray(ex.Message, res);
      }
    }

    internal static void OpenCFGReorderTool(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        BlueStacksUIUtils.DictWindows.Values.ToList<MainWindow>()[0].Dispatcher.Invoke((Delegate) (() => CFGReorderWindow.Instance.Show()));
      }
      catch (Exception ex)
      {
        Logger.Error("Couldn't open cfg reorder window. Ex: {0}", (object) ex);
      }
    }

    internal static void OpenThemeEditor(HttpListenerRequest _1, HttpListenerResponse _2)
    {
      try
      {
        if (!RegistryManager.Instance.OpenThemeEditor)
          return;
        BlueStacksUIUtils.DictWindows.Values.ToList<MainWindow>()[0].Dispatcher.Invoke((Delegate) (() => ThemeEditorWindow.Instance.Show()));
      }
      catch (Exception ex)
      {
      }
    }

    internal static void MuteAllInstancesHandler(HttpListenerRequest req, HttpListenerResponse _)
    {
      BlueStacksUIUtils.SendMuteUnmuteRequestToAllInstances(Convert.ToBoolean(HTTPUtils.ParseRequest(req).Data["muteInstance"], (IFormatProvider) CultureInfo.InvariantCulture));
    }

    internal static void AccountSetupCompleted(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        string requestVmName = request.RequestVmName;
        if (BlueStacksUIUtils.DictWindows.ContainsKey(request.RequestVmName) && FeatureManager.Instance.IsCustomUIForNCSoft)
          NCSoftUtils.Instance.SendGoogleLoginEventAsync(requestVmName);
        HTTPHandler.WriteSuccessJsonArray(res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in AccountSetupCompleted Handler: " + ex.ToString());
        HTTPHandler.WriteErrorJSONObjectWithoutReason(res);
      }
    }

    internal static void GetHeightWidth(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        string vmName = HTTPUtils.ParseRequest(req).RequestVmName;
        if (BlueStacksUIUtils.DictWindows.ContainsKey(vmName) && BlueStacksUIUtils.DictWindows[vmName] != null)
          BlueStacksUIUtils.DictWindows[vmName].Dispatcher.Invoke((Delegate) (() =>
          {
            try
            {
              MainWindow dictWindow = BlueStacksUIUtils.DictWindows[vmName];
              JArray jarray = new JArray();
              JObject jobject1 = new JObject()
              {
                (object) new JProperty("success", (object) true)
              };
              JObject jobject2 = new JObject()
              {
                (object) new JProperty("cHeight", (object) dictWindow.ActualHeight),
                (object) new JProperty("cWidth", (object) dictWindow.ActualWidth),
                (object) new JProperty("gHeight", (object) dictWindow.mContentGrid.ActualHeight),
                (object) new JProperty("gWidth", (object) dictWindow.mContentGrid.ActualWidth)
              };
              jarray.Add((JToken) jobject1);
              jarray.Add((JToken) new JObject()
              {
                (object) new JProperty("result", (object) jobject2)
              });
              HTTPUtils.Write(jarray.ToString(Formatting.None), res);
            }
            catch (Exception ex)
            {
              Logger.Error("Some error in finding MainWindow instance err: " + ex.ToString());
              HTTPHandler.WriteErrorJsonArray(ex.Message, res);
            }
          }));
        else
          HTTPHandler.WriteErrorJsonArray("Client Instance not running", res);
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to GetHeightWidth err : " + ex.ToString());
        HTTPHandler.WriteErrorJsonArray(ex.Message, res);
      }
    }

    internal static void ScreenLock(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        string vmName = request.RequestVmName;
        bool lockScreen = Convert.ToBoolean(request.Data["lock"], (IFormatProvider) CultureInfo.InvariantCulture);
        if (BlueStacksUIUtils.DictWindows.ContainsKey(vmName))
          BlueStacksUIUtils.DictWindows[vmName].Dispatcher.Invoke((Delegate) (() =>
          {
            if (lockScreen)
              BlueStacksUIUtils.DictWindows[vmName].ShowLockScreen();
            else
              BlueStacksUIUtils.DictWindows[vmName].HideLockScreen();
          }));
        HTTPHandler.WriteSuccessJsonArray(res);
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to lock screen err : " + ex.ToString());
        HTTPHandler.WriteErrorJsonArray(ex.Message, res);
      }
    }

    internal static void SetStreamingStatus(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        string vmName = request.RequestVmName;
        string status = request.Data["status"];
        if (BlueStacksUIUtils.DictWindows.ContainsKey(vmName))
          BlueStacksUIUtils.DictWindows[vmName].Dispatcher.Invoke((Delegate) (() => BlueStacksUIUtils.DictWindows[vmName].mCommonHandler.SetNcSoftStreamingStatus(status)));
        HTTPHandler.WriteSuccessJsonArray(res);
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to SetStreamingStatus err : " + ex.ToString());
        HTTPHandler.WriteErrorJsonArray(ex.Message, res);
      }
    }

    internal static void PlayerScriptModifierKeyUp(HttpListenerRequest req, HttpListenerResponse _)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        string vmName = request.RequestVmName;
        double x = Convert.ToDouble(request.Data["X"], (IFormatProvider) CultureInfo.InvariantCulture);
        double y = Convert.ToDouble(request.Data["Y"], (IFormatProvider) CultureInfo.InvariantCulture);
        if (!BlueStacksUIUtils.DictWindows.ContainsKey(vmName))
          return;
        BlueStacksUIUtils.DictWindows[vmName].Dispatcher.Invoke((Delegate) (() => BlueStacksUIUtils.DictWindows[vmName].mCommonHandler.AddCoordinatesToScriptText(x, y)));
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to handle player script modifier key up: " + ex.ToString());
      }
    }

    internal static void LaunchPlay(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        string key = request.Data["vmname"];
        string package = request.Data["package"];
        if (BlueStacksUIUtils.DictWindows.ContainsKey(key))
          BlueStacksUIUtils.DictWindows[key].Utils.HandleLaunchPlay(package);
        HTTPHandler.WriteSuccessJsonArray(res);
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to launch play store err : " + ex.ToString());
        HTTPHandler.WriteErrorJsonArray(ex.Message, res);
      }
    }

    internal static void FullScreenSidebarHandler(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        string requestVmName = request.RequestVmName;
        bool isVisible = Convert.ToBoolean(request.Data["visible"], (IFormatProvider) CultureInfo.InvariantCulture);
        if (!BlueStacksUIUtils.DictWindows.ContainsKey(requestVmName))
          return;
        MainWindow window = BlueStacksUIUtils.ActivatedWindow;
        if (window == null || !window.mIsFullScreen || window.mFrontendHandler.IsShootingModeActivated)
          return;
        window.Dispatcher.Invoke((Delegate) (() => window.mSidebar.ToggleSidebarVisibilityInFullscreen(isVisible)));
      }
      catch (Exception ex)
      {
        Logger.Error("Error in FullScreenSidebarHandler : " + ex.ToString());
        HTTPHandler.WriteErrorJsonArray(ex.Message, res);
      }
    }

    internal static void HideTopSideBarHandler(HttpListenerRequest req, HttpListenerResponse res)
    {
      string requestVmName = HTTPUtils.ParseRequest(req).RequestVmName;
      if (!BlueStacksUIUtils.DictWindows.ContainsKey(requestVmName))
        return;
      MainWindow window = BlueStacksUIUtils.ActivatedWindow;
      if (window == null || !window.mIsFullScreen || window.mFrontendHandler.IsShootingModeActivated)
        return;
      window.Dispatcher.Invoke((Delegate) (() =>
      {
        window.mSidebar.HideSideBarInFullscreen();
        window.mTopbarOptions.HideTopBarInFullscreen();
      }));
    }

    internal static void FullScreenTopbarButtonHandler(
      HttpListenerRequest req,
      HttpListenerResponse res)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        string requestVmName = request.RequestVmName;
        bool isVisible = Convert.ToBoolean(request.Data["visible"], (IFormatProvider) CultureInfo.InvariantCulture);
        if (!BlueStacksUIUtils.DictWindows.ContainsKey(requestVmName))
          return;
        MainWindow window = BlueStacksUIUtils.ActivatedWindow;
        if (window == null || !window.mIsFullScreen || window.mFrontendHandler.IsShootingModeActivated)
          return;
        window.Dispatcher.Invoke((Delegate) (() => window.mTopbarOptions.ToggleTopbarButtonVisibilityInFullscreen(isVisible)));
      }
      catch (Exception ex)
      {
        Logger.Error("Error in FullScreenTopbarButtonHandler : " + ex.ToString());
        HTTPHandler.WriteErrorJsonArray(ex.Message, res);
      }
    }

    internal static void FullScreenSidebarButtonHandler(
      HttpListenerRequest req,
      HttpListenerResponse res)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        string requestVmName = request.RequestVmName;
        bool isVisible = Convert.ToBoolean(request.Data["visible"], (IFormatProvider) CultureInfo.InvariantCulture);
        if (!BlueStacksUIUtils.DictWindows.ContainsKey(requestVmName))
          return;
        MainWindow window = BlueStacksUIUtils.ActivatedWindow;
        if (window == null || !window.mIsFullScreen || window.mFrontendHandler.IsShootingModeActivated)
          return;
        window.Dispatcher.Invoke((Delegate) (() => window.mSidebar.ToggleSidebarButtonVisibilityInFullscreen(isVisible)));
      }
      catch (Exception ex)
      {
        Logger.Error("Errro in FullScreenSidebarButtonHandler : " + ex.ToString());
        HTTPHandler.WriteErrorJsonArray(ex.Message, res);
      }
    }

    internal static void FullScreenTopbarHandler(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        string requestVmName = request.RequestVmName;
        bool isVisible = Convert.ToBoolean(request.Data["visible"], (IFormatProvider) CultureInfo.InvariantCulture);
        if (!BlueStacksUIUtils.DictWindows.ContainsKey(requestVmName))
          return;
        MainWindow window = BlueStacksUIUtils.DictWindows[requestVmName];
        if (window == null || !window.mIsFullScreen || window.mFrontendHandler.IsShootingModeActivated)
          return;
        window.Dispatcher.Invoke((Delegate) (() =>
        {
          if (!window.mTopBarPopup.IsOpen & isVisible)
            window.mTopBarPopup.IsOpen = true;
          else
            window.mTopBarPopup.IsOpen = false;
        }));
      }
      catch (Exception ex)
      {
        Logger.Error("Error FullScreenTopbarHandler : " + ex.ToString());
        HTTPHandler.WriteErrorJsonArray(ex.Message, res);
      }
    }

    internal static void HandleGamepadConnection(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        bool flag = bool.Parse(request.Data["status"]);
        if (BlueStacksUIUtils.DictWindows.ContainsKey(request.RequestVmName))
        {
          BlueStacksUIUtils.DictWindows[request.RequestVmName].IsGamepadConnected = flag;
          if (!HTTPHandler.mSendGamepadStats)
          {
            ClientStats.SendMiscellaneousStatsAsync("GamePadConnectedStats", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, (string) null, (string) null, (string) null, (string) null, (string) null, (string) null, "Android");
            HTTPHandler.mSendGamepadStats = true;
          }
        }
        HTTPHandler.WriteSuccessJsonWithVmName(request.RequestVmName, res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in HandleGamepadConnection. Err : " + ex.ToString());
        HTTPHandler.WriteErrorJSONObjectWithoutReason(res);
      }
    }

    internal static void TileWindow(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        HTTPUtils.ParseRequest(req);
        CommonHandlers.ArrangeWindowInTiles();
        HTTPHandler.WriteSuccessJsonArray(res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in tiling window. Err : " + ex.ToString());
        HTTPHandler.WriteErrorJSONObjectWithoutReason(res);
      }
    }

    internal static void CascadeWindow(HttpListenerRequest _, HttpListenerResponse res)
    {
      try
      {
        CommonHandlers.ArrangeWindowInCascade();
        HTTPHandler.WriteSuccessJsonArray(res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in Cascading window. Err : " + ex.ToString());
        HTTPHandler.WriteErrorJsonArray(ex.ToString(), res);
      }
    }

    internal static void UpdateLocale(HttpListenerRequest req, HttpListenerResponse res)
    {
      Logger.Info("Got UpdateLocale {0} request from {1}", (object) req.HttpMethod, (object) req.RemoteEndPoint.ToString());
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        string requestVmName = request.RequestVmName;
        string str = request.Data["locale"].ToString((IFormatProvider) CultureInfo.InvariantCulture);
        if (BlueStacksUIUtils.DictWindows.ContainsKey(requestVmName))
        {
          RegistryManager.Instance.UserSelectedLocale = str;
          Utils.UpdateValueInBootParams("LANG", str, requestVmName, false, "bgp");
          BlueStacksUIUtils.DictWindows[requestVmName].Dispatcher.Invoke((Delegate) (() => LocaleStrings.InitLocalization((string) null, "Android", false)));
          HTTPUtils.SendRequestToAgentAsync("reinitlocalization", (Dictionary<string, string>) null, "Android", 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
        }
        HTTPHandler.WriteSuccessJsonArray(res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in UpdateLocale: " + ex.ToString());
        HTTPHandler.WriteErrorJsonArray(ex.ToString(), res);
      }
    }

    internal static void ScreenshotCaptured(HttpListenerRequest req, HttpListenerResponse res)
    {
      Logger.Info("Got ScreenshotCaptured {0} request from {1}", (object) req.HttpMethod, (object) req.RemoteEndPoint.ToString());
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        string vmName = request.RequestVmName;
        string path = request.Data["path"].ToString((IFormatProvider) CultureInfo.InvariantCulture);
        string str = request.Data["showSavedInfo"] ?? "false";
        Logger.Info("ScreenshotCaptured: VmName: " + vmName + " Path: " + path);
        bool showSaved;
        bool.TryParse(str, out showSaved);
        if (Oem.IsOEMDmm)
          showSaved = false;
        if (BlueStacksUIUtils.DictWindows.ContainsKey(vmName))
          BlueStacksUIUtils.DictWindows[vmName].Dispatcher.Invoke((Delegate) (() => BlueStacksUIUtils.DictWindows[vmName].mCommonHandler.PostScreenShotWork(path, showSaved)));
        HTTPHandler.WriteSuccessJsonArray(res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in ScreenshotCaptured: " + ex.ToString());
        HTTPHandler.WriteErrorJsonArray(ex.ToString(), res);
      }
    }

    internal static void ClientHotkeyHandler(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        string vmName = request.RequestVmName;
        ClientHotKeys clientHotKey = (ClientHotKeys) System.Enum.Parse(typeof (ClientHotKeys), request.Data["keyevent"].ToString((IFormatProvider) CultureInfo.InvariantCulture));
        if (BlueStacksUIUtils.DictWindows.ContainsKey(vmName))
          BlueStacksUIUtils.DictWindows[vmName].Dispatcher.Invoke((Delegate) (() => BlueStacksUIUtils.DictWindows[vmName].HandleClientHotKey(clientHotKey)));
        HTTPHandler.WriteSuccessJsonArray(res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in ClientHotkeyHandler: " + ex.ToString());
        HTTPHandler.WriteErrorJsonArray(ex.ToString(), res);
      }
    }

    internal static void AndroidLocaleChanged(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        BlueStacksUIUtils.UpdateLocale(RegistryManager.Instance.UserSelectedLocale, HTTPUtils.ParseRequest(req).RequestVmName);
        HTTPHandler.WriteSuccessJsonArray(res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in AndroidLocaleChanged. Err : " + ex.ToString());
        HTTPHandler.WriteErrorJSONObjectWithoutReason(res);
      }
    }

    internal static void HandleClientOperation(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        if (BlueStacksUIUtils.DictWindows.ContainsKey(request.RequestVmName))
        {
          string operationString = request.Data["data"];
          BlueStacksUIUtils.DictWindows[request.RequestVmName].mCommonHandler.HandleClientOperation(operationString);
        }
        HTTPHandler.WriteSuccessJsonArray(res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in HandleClientOperation. Err : " + ex.ToString());
        HTTPHandler.WriteErrorJSONObjectWithoutReason(res);
      }
    }

    internal static void MacroPlaybackCompleteHandler(
      HttpListenerRequest req,
      HttpListenerResponse res)
    {
      try
      {
        RequestData requestData = HTTPUtils.ParseRequest(req);
        if (!BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
          return;
        BlueStacksUIUtils.DictWindows[requestData.RequestVmName].Dispatcher.Invoke((Delegate) (() => BlueStacksUIUtils.DictWindows[requestData.RequestVmName].SetMacroPlayBackEventHandle()));
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in MacroPlaybackCompleteHandler. Err : " + ex.ToString());
        HTTPHandler.WriteErrorJSONObjectWithoutReason(res);
      }
    }

    internal static void HandleClientGamepadButtonHandler(
      HttpListenerRequest req,
      HttpListenerResponse res)
    {
      try
      {
        RequestData requestData = HTTPUtils.ParseRequest(req);
        if (!BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
          return;
        BlueStacksUIUtils.DictWindows[requestData.RequestVmName].Dispatcher.Invoke((Delegate) (() =>
        {
          string text = requestData.Data["data"];
          bool result;
          if (bool.TryParse(requestData.Data["isDown"], out result))
            KMManager.UpdateUIForGamepadEvent(text, result);
          else
            Logger.Error("Error in HandleClientGamepadButtonHandler: Could not parse gamepad event isDown:'{0}'", (object) requestData.Data["isDown"]);
        }));
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in HandleClientGamepadButtonHandler. Err : " + ex.ToString());
        HTTPHandler.WriteErrorJSONObjectWithoutReason(res);
      }
    }

    internal static void SaveComboEvents(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        string events = request.Data["events"];
        if (BlueStacksUIUtils.DictWindows.ContainsKey(request.RequestVmName))
        {
          if (BlueStacksUIUtils.DictWindows[request.RequestVmName].mIsMacroRecorderActive)
            BlueStacksUIUtils.DictWindows[request.RequestVmName].MacroRecorderWindow.SaveOperation(events);
          else
            BlueStacksUIUtils.DictWindows[request.RequestVmName].Dispatcher.Invoke((Delegate) (() => KMManager.mComboEvents = events));
        }
        HTTPHandler.WriteSuccessJsonArray(res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in SaveComboEvents. Err : " + ex.ToString());
        HTTPHandler.WriteErrorJSONObjectWithoutReason(res);
      }
    }

    internal static void MacroCompleted(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData requestData = HTTPUtils.ParseRequest(req);
        if (BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
          BlueStacksUIUtils.DictWindows[requestData.RequestVmName].Dispatcher.Invoke((Delegate) (() => BlueStacksUIUtils.DictWindows[requestData.RequestVmName].MacroOverlayControl.ShowPromptAndHideOverlay()));
        HTTPHandler.WriteSuccessJsonArray(res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in MacroCompleted. Err : " + ex.ToString());
        HTTPHandler.WriteErrorJSONObjectWithoutReason(res);
      }
    }

    internal static void ShowMaintenanceWarning(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        string vmName = request.RequestVmName;
        string message = request.Data["message"];
        HTTPHandler.WriteJSON(new Dictionary<string, string>()
        {
          {
            "result_code",
            "0"
          }
        }, res);
        BlueStacksUIUtils.DictWindows[vmName].Dispatcher.Invoke((Delegate) (() =>
        {
          CustomMessageWindow customMessageWindow = new CustomMessageWindow();
          customMessageWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("BlueStacks", "") + " " + LocaleStrings.GetLocalizedString("STRING_WARNING", "");
          customMessageWindow.BodyTextBlock.Text = message;
          customMessageWindow.AddButton(ButtonColors.Blue, LocaleStrings.GetLocalizedString("STRING_OK", ""), (EventHandler) null, (string) null, false, (object) null, true);
          customMessageWindow.Owner = (Window) BlueStacksUIUtils.DictWindows[vmName];
          BlueStacksUIUtils.DictWindows[vmName].ShowDimOverlay((IDimOverlayControl) null);
          customMessageWindow.ShowDialog();
          BlueStacksUIUtils.DictWindows[vmName].HideDimOverlay();
        }));
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to ShowMaintenanceWarning app... Err : " + ex.ToString());
        HTTPHandler.WriteJSON(new Dictionary<string, string>()
        {
          {
            "result_code",
            "-1"
          }
        }, res);
      }
    }

    internal static void WriteJSON(Dictionary<string, string> data, HttpListenerResponse res)
    {
      HTTPUtils.Write(JSONUtils.GetJSONArrayString(data), res);
    }

    internal static void LaunchDefaultWebApp(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        string str1 = request.Data["action"];
        Logger.Info("Action : " + str1);
        if (str1 != null)
        {
          if (!(str1 == "browser"))
          {
            if (str1 == "email")
            {
              string str2 = "";
              string str3 = "";
              string str4 = "";
              string str5 = "";
              string str6 = "";
              string str7 = "";
              try
              {
                str2 = request.Data["to"];
                str3 = request.Data["cc"];
                str4 = request.Data["bcc"];
                str5 = request.Data["message"];
                str6 = request.Data["subject"];
                str7 = request.Data["mailto"];
              }
              catch
              {
              }
              bool flag = false;
              if (!string.IsNullOrEmpty(str2))
                flag = str2.Split('@').Length > 1;
              Logger.Info("to : " + str2 + ", cc : " + str3 + ", bcc : " + str4 + ", subject = " + str6);
              Logger.Info("mailto : " + str7);
              string fileName;
              if (flag)
                fileName = "mailto:" + str2 + "?cc=" + str3 + "&bcc=" + str4 + "&subject=" + str6 + "&body=" + str5;
              else if (!string.IsNullOrEmpty(str7))
              {
                fileName = str7;
              }
              else
              {
                HTTPHandler.WriteErrorJsonArray("to and mailto field cannot be empty", res);
                return;
              }
              Logger.Info("mail to request : " + fileName);
              Process.Start(fileName);
              HTTPHandler.WriteSuccessJsonArray(res);
              return;
            }
          }
          else
          {
            string fileName = request.Data["url"];
            Logger.Info("Url : " + fileName);
            try
            {
              Process.Start(fileName);
              HTTPHandler.WriteSuccessJsonArray(res);
              return;
            }
            catch
            {
              HTTPHandler.WriteErrorJsonArray("Invalid or empty url", res);
              return;
            }
          }
        }
        HTTPHandler.WriteErrorJsonArray("wrong or empty action", res);
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to LaunchDefaultWebApp app... Err : " + ex.ToString());
        HTTPHandler.WriteErrorJsonArray(ex.Message, res);
      }
    }

    public static void GetRunningInstances(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        List<string> stringList = new List<string>((IEnumerable<string>) BlueStacksUIUtils.DictWindows.Keys);
        HTTPUtils.ParseRequest(req);
        JObject jobject = new JObject();
        string str = string.Join(",", stringList.ToArray());
        Logger.Info("Running instances: " + str);
        jobject.Add("success", (JToken) true);
        jobject.Add("vmname", (JToken) str);
        HTTPUtils.Write(jobject.ToString(Formatting.None), res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in GetRunningInstances");
        Logger.Error(ex.ToString());
        HTTPHandler.WriteErrorJSONObjectWithoutReason(res);
      }
    }

    internal static void IsAnyAppRunning(HttpListenerRequest _1, HttpListenerResponse res)
    {
      try
      {
        JObject jobject = new JObject()
        {
          {
            "success",
            (JToken) true
          }
        };
        bool isAppRunning = false;
        if (BlueStacksUIUtils.DictWindows.Count > 0)
        {
          MainWindow window = BlueStacksUIUtils.DictWindows[BlueStacksUIUtils.DictWindows.Keys.ToList<string>()[0]];
          window.Dispatcher.Invoke((Delegate) (() => isAppRunning = window.mTopBar.mAppTabButtons.IsAppRunning()));
          jobject.Add("isanyapprunning", (JToken) isAppRunning);
        }
        HTTPUtils.Write(jobject.ToString(Formatting.None), res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in IsAnyAppRunning: {0}", (object) ex);
        HTTPHandler.WriteErrorJsonArray(ex.ToString(), res);
      }
    }

    internal static void GetCurrentAppDetails(HttpListenerRequest _1, HttpListenerResponse res)
    {
      try
      {
        JObject jobject = new JObject()
        {
          {
            "success",
            (JToken) true
          }
        };
        if (BlueStacksUIUtils.DictWindows.Count > 0)
        {
          MainWindow window = BlueStacksUIUtils.DictWindows[BlueStacksUIUtils.DictWindows.Keys.ToList<string>()[0]];
          string pkg = string.Empty;
          string appName = string.Empty;
          string tabType = string.Empty;
          window.Dispatcher.Invoke((Delegate) (() =>
          {
            pkg = window.mTopBar.mAppTabButtons.SelectedTab.PackageName;
            appName = (string) window.mTopBar.mAppTabButtons.SelectedTab.mTabLabel.Content;
            tabType = window.mTopBar.mAppTabButtons.SelectedTab.mTabType.ToString();
          }));
          jobject.Add("pkgname", (JToken) pkg);
          jobject.Add("appname", (JToken) appName);
          jobject.Add("tabtype", (JToken) tabType);
        }
        HTTPUtils.Write(jobject.ToString(Formatting.None), res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in GetCurrentAppDetails: {0}", (object) ex);
        HTTPHandler.WriteErrorJsonArray(ex.ToString(), res);
      }
    }

    internal static void ShowSettingWindow(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        if (BlueStacksUIUtils.DictWindows.ContainsKey(request.RequestVmName))
        {
          MainWindow window = BlueStacksUIUtils.DictWindows[request.RequestVmName];
          window.Dispatcher.Invoke((Delegate) (() => MainWindow.OpenSettingsWindow(window, "STRING_NOTIFICATION")));
        }
        HTTPHandler.WriteSuccessJsonArray(res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in Server ShowSettingWindow: " + ex.ToString());
        HTTPHandler.WriteErrorJsonArray(ex.ToString(), res);
      }
    }

    internal static void LaunchWebTab(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData requestData = HTTPUtils.ParseRequest(req);
        if (!BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
          return;
        MainWindow window = BlueStacksUIUtils.DictWindows[requestData.RequestVmName];
        window.Dispatcher.Invoke((Delegate) (() => window.mTopBar.mAppTabButtons.AddWebTab(requestData.Data["url"], requestData.Data["name"], requestData.Data["image"], true, "", false)));
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in Server OneTimeSetupCompletedHandler: " + ex.ToString());
        HTTPHandler.WriteErrorJsonArray(ex.ToString(), res);
      }
    }

    internal static void OneTimeSetupCompletedHandler(
      HttpListenerRequest req,
      HttpListenerResponse res)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        MainWindow dictWindow = BlueStacksUIUtils.DictWindows[request.RequestVmName];
        if (!BlueStacksUIUtils.DictWindows.ContainsKey(request.RequestVmName))
          return;
        string userGuid1 = RegistryManager.Instance.UserGuid;
        string clientVersion1 = RegistryManager.Instance.ClientVersion;
        string currentEngine1 = RegistryManager.Instance.CurrentEngine;
        int num1 = dictWindow.EngineInstanceRegistry.GlMode;
        string str1 = num1.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        num1 = dictWindow.EngineInstanceRegistry.GlRenderMode;
        string str2 = num1.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        ClientStats.SendMiscellaneousStatsAsync("OTSActivityDisplayed", userGuid1, clientVersion1, "OTS Completed", "OTS Completed", (string) null, currentEngine1, str1, str2, "Android");
        dictWindow.mAppHandler.IsOneTimeSetupCompleted = true;
        string userGuid2 = RegistryManager.Instance.UserGuid;
        string clientVersion2 = RegistryManager.Instance.ClientVersion;
        string installId = RegistryManager.Instance.InstallID;
        string currentEngine2 = RegistryManager.Instance.CurrentEngine;
        int num2 = dictWindow.EngineInstanceRegistry.GlMode;
        string str3 = num2.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        num2 = dictWindow.EngineInstanceRegistry.GlRenderMode;
        string str4 = num2.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        ClientStats.SendMiscellaneousStatsAsync("OTSActivityDisplayed", userGuid2, clientVersion2, "OTS Completed", "OTS Completed", installId, currentEngine2, str3, str4, "Android");
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in Server OneTimeSetupCompletedHandler: " + ex.ToString());
        HTTPHandler.WriteErrorJsonArray(ex.ToString(), res);
      }
    }

    internal static void AppJsonChangedHandler(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData requestData = HTTPUtils.ParseRequest(req);
        if (!BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
          return;
        BlueStacksUIUtils.DictWindows[requestData.RequestVmName].Dispatcher.Invoke((Delegate) (() => BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mWelcomeTab.mHomeAppManager.InitIcons()));
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in AppjsonChangedHabdler " + ex.ToString());
        HTTPHandler.WriteErrorJsonArray(ex.ToString(), res);
      }
    }

    internal static void StartInstanceHandler(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        string requestVmName = HTTPUtils.ParseRequest(req).RequestVmName;
        Logger.Info("start instance vm name :" + requestVmName);
        RegistryManager.ClearRegistryMangerInstance();
        BlueStacksUIUtils.RunInstance(requestVmName, false, "");
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in Server StartInstanceHandler: " + ex.ToString());
        HTTPHandler.WriteErrorJsonArray(ex.ToString(), res);
      }
    }

    internal static void StopInstanceHandler(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        string requestVmName = HTTPUtils.ParseRequest(req).RequestVmName;
        if (BlueStacksUIUtils.DictWindows.ContainsKey(requestVmName))
          BlueStacksUIUtils.DictWindows[requestVmName].ForceCloseWindow(false);
        HTTPHandler.WriteSuccessJsonArray(res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in Server StopInstanceHandler: " + ex.ToString());
        HTTPHandler.WriteErrorJsonArray(ex.ToString(), res);
      }
    }

    internal static void MinimizeInstanceHandler(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        string vmName = HTTPUtils.ParseRequest(req).RequestVmName;
        if (BlueStacksUIUtils.DictWindows.ContainsKey(vmName))
          BlueStacksUIUtils.DictWindows[vmName].Dispatcher.Invoke((Delegate) (() => BlueStacksUIUtils.DictWindows[vmName].MinimizeWindow()));
        HTTPHandler.WriteSuccessJsonArray(res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in Server MinimizeInstanceHandler: " + ex.ToString());
        HTTPHandler.WriteErrorJsonArray(ex.ToString(), res);
      }
    }

    internal static void HideBluestacksHandler(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        HTTPUtils.ParseRequest(req);
        Logger.Info("Hide Bluestacks received");
        BlueStacksUIUtils.HideUnhideBlueStacks(true);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in Server HideBluestacksHandler: " + ex.ToString());
        HTTPHandler.WriteErrorJsonArray(ex.ToString(), res);
      }
    }

    internal static void OpenOrInstallPackageHandler(
      HttpListenerRequest req,
      HttpListenerResponse res)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        string requestVmName = request.RequestVmName;
        string json = request.Data["json"].ToString((IFormatProvider) CultureInfo.InvariantCulture);
        if (!string.IsNullOrEmpty(json))
        {
          JObject jobject = JObject.Parse(json);
          if (jobject != null && jobject["campaign_id"] != null)
            RegistryManager.Instance.ClientLaunchParams = json;
        }
        HTTPHandler.ShowWindowHandler(request);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in OpenOrInstallPackageHandler. Err : " + ex.ToString());
        HTTPHandler.WriteErrorJSONObjectWithoutReason(res);
      }
    }

    internal static void GuestBootCompleted(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        if (!BlueStacksUIUtils.DictWindows.ContainsKey(request.RequestVmName))
          return;
        BlueStacksUIUtils.DictWindows[request.RequestVmName].mAppHandler.IsGuestReady = true;
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in server GuestBootCompleted: " + ex.ToString());
        HTTPHandler.WriteErrorJsonArray(ex.ToString(), res);
      }
    }

    public static void AppDisplayedHandler(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        foreach (string allKey in request.Data.AllKeys)
          Logger.Debug("Key: {0}, Value: {1}", (object) allKey, (object) request.Data[allKey]);
        string packageName = request.Data["packageName"];
        string str = request.Data["appDisplayed"];
        lock (HTTPHandler.sLockObject)
        {
          if (BlueStacksUIUtils.DictWindows.ContainsKey(request.RequestVmName))
          {
            MainWindow dictWindow = BlueStacksUIUtils.DictWindows[request.RequestVmName];
            if (Oem.IsOEMDmm)
              dictWindow.mAppHandler.HandleAppDisplayed(packageName);
            if (!dictWindow.EngineInstanceRegistry.IsOneTimeSetupDone)
            {
              if (packageName != "com.bluestacks.appmart")
              {
                if (!dictWindow.mGuestBootCompleted)
                {
                  int num = 20;
                  while (!dictWindow.mAppHandler.IsGuestReady && num > 0)
                  {
                    --num;
                    Thread.Sleep(1000);
                  }
                  if (packageName == dictWindow.mAppHandler.GetDefaultLauncher())
                  {
                    if (!FeatureManager.Instance.IsCustomUIForNCSoft)
                    {
                      Logger.Info("BOOT_STAGE: Calling guestboot_completed from AppDisplayedHandler");
                      dictWindow.mAppHandler.IsGuestReady = true;
                    }
                    else
                    {
                      dictWindow.Utils.sBootCheckTimer.Enabled = false;
                      dictWindow.mEnableLaunchPlayForNCSoft = true;
                    }
                  }
                }
              }
            }
          }
        }
        HTTPHandler.WriteSuccessJsonArray(res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in Server AppDisplayedHandler: " + ex.ToString());
        HTTPHandler.WriteErrorJsonArray(ex.ToString(), res);
      }
    }

    public static void AppLaunchedHandler(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        foreach (string allKey in request.Data.AllKeys)
          Logger.Debug("Key: {0}, Value: {1}", (object) allKey, (object) request.Data[allKey]);
        string packageName = request.Data["package"];
        string str1 = request.Data["activity"];
        string str2 = request.Data["callingPackage"];
        Logger.Info("Package: {0}, activity: {1}, callingPackage: {2}", (object) packageName, (object) str1, (object) str2);
        if (BlueStacksUIUtils.DictWindows.ContainsKey(request.RequestVmName))
        {
          MainWindow dictWindow = BlueStacksUIUtils.DictWindows[request.RequestVmName];
          if (!RegistryManager.Instance.Guest[request.RequestVmName].IsGoogleSigninDone)
          {
            lock (HTTPHandler.syncRoot)
            {
              if (string.Compare(HTTPHandler.mPreviousActivityReported.Replace("/", ""), str1.Replace("/", ""), StringComparison.OrdinalIgnoreCase) != 0)
              {
                HTTPHandler.mPreviousActivityReported = str1;
                ClientStats.SendMiscellaneousStatsAsync("OTSActivityDisplayed", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, packageName, str1, RegistryManager.Instance.InstallID, RegistryManager.Instance.CurrentEngine, dictWindow.EngineInstanceRegistry.GlMode.ToString((IFormatProvider) CultureInfo.InvariantCulture), dictWindow.EngineInstanceRegistry.GlRenderMode.ToString((IFormatProvider) CultureInfo.InvariantCulture), "Android");
              }
            }
          }
          if (FeatureManager.Instance.IsCustomUIForNCSoft && !dictWindow.mGuestBootCompleted && (!packageName.Equals("com.bluestacks.appmart", StringComparison.OrdinalIgnoreCase) && !packageName.Equals("com.android.provision", StringComparison.OrdinalIgnoreCase)))
            dictWindow.mAppHandler.IsGuestReady = true;
          if (dictWindow.mGuestBootCompleted)
          {
            if (!Oem.IsOEMDmm)
            {
              PostBootCloudInfo postBootCloudInfo1 = PostBootCloudInfoManager.Instance.mPostBootCloudInfo;
              if (!(postBootCloudInfo1 != null ? new bool?(postBootCloudInfo1.IgnoredActivitiesForTabs.Contains<string>(str1, (IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase)) : new bool?()).GetValueOrDefault())
              {
                PostBootCloudInfo postBootCloudInfo2 = PostBootCloudInfoManager.Instance.mPostBootCloudInfo;
                if ((postBootCloudInfo2 != null ? new bool?(postBootCloudInfo2.IgnoredActivitiesForTabs.Contains<string>(str1.TrimStart(packageName + "/"), (IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase)) : new bool?()).GetValueOrDefault())
                  goto label_17;
              }
              else
                goto label_17;
            }
            dictWindow.mAppHandler.AppLaunched(packageName, str1, false);
            if (str1.Equals("com.lilithgame.roc.gp/sh.lilith.lilithchat.activities.ImagePickerActivity", StringComparison.InvariantCultureIgnoreCase))
              ClientStats.SendMiscellaneousStatsAsync("ss_open_gallery_in_game", RegistryManager.Instance.UserGuid, packageName, (string) null, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
          }
        }
label_17:
        HTTPHandler.WriteSuccessJsonArray(res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in Server AppLaunchedHandler: " + ex.ToString());
        HTTPHandler.WriteErrorJsonArray(ex.ToString(), res);
      }
    }

    public static void AppCrashedHandler(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        foreach (string allKey in request.Data.AllKeys)
          Logger.Debug("Key: {0}, Value: {1}", (object) allKey, (object) request.Data[allKey]);
        string vmName = request.RequestVmName;
        string package = request.Data["package"];
        Logger.Info("package: " + package);
        if (BlueStacksUIUtils.DictWindows.ContainsKey(vmName))
        {
          BlueStacksUIUtils.DictWindows[vmName].Dispatcher.Invoke((Delegate) (() => BlueStacksUIUtils.DictWindows[vmName].mTopBar.mAppTabButtons.CloseTab("app:" + package, false, false, true, false, "")));
          if (FeatureManager.Instance.IsCustomUIForNCSoft && !NCSoftUtils.Instance.BlackListedApps.Any<string>((Func<string, bool>) (pkg => package.StartsWith(pkg, StringComparison.InvariantCulture))))
            NCSoftUtils.Instance.SendAppCrashEvent("check android logs", vmName);
        }
        HTTPHandler.WriteSuccessJsonArray(res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in Server AppCrashedHandler: " + ex.ToString());
        HTTPHandler.WriteErrorJsonArray(ex.ToString(), res);
      }
    }

    internal static void AppInfoUpdated(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        string package = request.Data["packageName"];
        MainWindow dictWindow1 = BlueStacksUIUtils.DictWindows[request.RequestVmName];
        if (!string.IsNullOrEmpty(request.Data["macro"]) && request.Data["macro"].Equals("true", StringComparison.InvariantCultureIgnoreCase))
        {
          foreach (string allKey in request.Data.AllKeys)
            Logger.Debug("Key: {0}, Value: {1}", (object) allKey, (object) request.Data[allKey]);
          BlueStacksUIUtils.DictWindows[request.RequestVmName].Dispatcher.Invoke((Delegate) (() =>
          {
            foreach (KeyValuePair<string, MainWindow> dictWindow in BlueStacksUIUtils.DictWindows)
            {
              if (dictWindow.Value.mWelcomeTab.mHomeAppManager.GetAppIcon(package) != null && dictWindow.Value.mWelcomeTab.mHomeAppManager.GetMacroAppIcon(package) == null)
                dictWindow.Value.mWelcomeTab.mHomeAppManager.AddMacroAppIcon(package);
            }
          }));
          HTTPHandler.WriteSuccessJsonArray(res);
        }
        if (!string.IsNullOrEmpty(request.Data["videoPresent"]) && BlueStacksUIUtils.DictWindows.ContainsKey(request.RequestVmName))
        {
          string str = request.Data["videoPresent"].ToString((IFormatProvider) CultureInfo.InvariantCulture);
          HTTPUtils.SendRequestToAgentAsync("appJsonUpdatedForVideo", new Dictionary<string, string>()
          {
            {
              "packageName",
              package
            },
            {
              "videoPresent",
              str
            }
          }, request.RequestVmName, 0, (Dictionary<string, string>) null, true, 1, 0, "bgp");
        }
        KMManager.ControlSchemesHandlingWhileCfgUpdateFromCloud(dictWindow1, package);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in Server AppInfoDownload: " + ex.ToString());
        HTTPHandler.WriteErrorJsonArray(ex.ToString(), res);
      }
    }

    public static void CloseTabHandler(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData requestData = HTTPUtils.ParseRequest(req);
        foreach (string allKey in requestData.Data.AllKeys)
          Logger.Debug("Key: {0}, Value: {1}", (object) allKey, (object) requestData.Data[allKey]);
        string package = requestData.Data["package"];
        Logger.Info("package: " + package);
        if (BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
          BlueStacksUIUtils.DictWindows[requestData.RequestVmName].Dispatcher.Invoke((Delegate) (() =>
          {
            try
            {
              BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mTopBar.mAppTabButtons.CloseTab(package, false, false, true, false, "");
            }
            catch (Exception ex)
            {
              Logger.Error("Exception in closing tab. Err : ", (object) ex.ToString());
            }
          }));
        HTTPHandler.WriteSuccessJsonArray(res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in Server TabCloseHandler: " + ex.ToString());
        HTTPHandler.WriteErrorJsonArray(ex.ToString(), res);
      }
    }

    public static void ShowAppHandler(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        HTTPHandler.ShowAppHandler(HTTPUtils.ParseRequest(req));
        HTTPHandler.WriteSuccessJsonArray(res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in Server ShowAppHandler: " + ex.ToString());
        HTTPHandler.WriteErrorJsonArray(ex.ToString(), res);
      }
    }

    public static void ShowAppHandler(RequestData requestData)
    {
      foreach (string allKey in requestData.Data.AllKeys)
        Logger.Debug("Key: {0}, Value: {1}", (object) allKey, (object) requestData.Data[allKey]);
      string package = requestData.Data["package"];
      string activity = requestData.Data["activity"];
      string str = requestData.Data["title"];
      Logger.Info("package: " + package);
      Logger.Info("activity: " + activity);
      Logger.Info("title : " + str);
      if (!BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
        return;
      new Thread((ThreadStart) (() => BlueStacksUIUtils.DictWindows[requestData.RequestVmName].Dispatcher.Invoke((Delegate) (() =>
      {
        if (string.IsNullOrEmpty(package) || string.IsNullOrEmpty(activity))
          return;
        BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mAppHandler.SendRunAppRequestAsync(package, activity, false);
      }))))
      {
        IsBackground = true
      }.Start();
    }

    public static void ShowWindowHandler(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        HTTPHandler.ShowWindowHandler(HTTPUtils.ParseRequest(req));
        HTTPHandler.WriteSuccessJsonArray(res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in Server ShowWindowHandler: " + ex.ToString());
        HTTPHandler.WriteErrorJsonArray(ex.ToString(), res);
      }
    }

    public static void ShowWindowHandler(RequestData requestData)
    {
      string requestVmName = requestData.RequestVmName;
      if (((IEnumerable<string>) requestData.Data.AllKeys).Contains<string>("showNotifications"))
        MainWindow.sShowNotifications = Convert.ToBoolean(requestData.Data["showNotifications"], (IFormatProvider) CultureInfo.InvariantCulture);
      if (((IEnumerable<string>) requestData.Data.AllKeys).Contains<string>("all"))
      {
        foreach (KeyValuePair<string, MainWindow> dictWindow in BlueStacksUIUtils.DictWindows)
          dictWindow.Value.ShowWindow(false);
      }
      else
      {
        if (((IEnumerable<string>) requestData.Data.AllKeys).Contains<string>("vmname"))
          requestVmName = requestData.Data["vmname"];
        bool hiddenMode = requestData.Data["hidden"] != null && Convert.ToBoolean(requestData.Data["hidden"], (IFormatProvider) CultureInfo.InvariantCulture);
        string str = requestData.Data["json"] != null ? requestData.Data["json"].ToString((IFormatProvider) CultureInfo.InvariantCulture) : "";
        if (BlueStacksUIUtils.DictWindows.ContainsKey(requestVmName))
        {
          if (!hiddenMode)
            BlueStacksUIUtils.DictWindows[requestVmName].ShowWindow(false);
          if (string.IsNullOrEmpty(str))
            return;
          JObject jobject = JObject.Parse(str);
          if (BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mAppHandler.IsOneTimeSetupCompleted && !jobject.ContainsKey("check_fle_pkg"))
          {
            BlueStacksUIUtils.DictWindows[requestVmName].PublishForFlePopupToBrowser(str);
            new DownloadInstallApk(BlueStacksUIUtils.DictWindows[requestVmName]).DownloadAndInstallAppFromJson(str);
          }
          else
          {
            BlueStacksUIUtils.DictWindows[requestVmName].WindowLaunchParams = str;
            if (!jobject.ContainsKey("check_fle_pkg"))
              return;
            BlueStacksUIUtils.DictWindows[requestVmName].PublishForHandleFleToBrowser(str);
          }
        }
        else
        {
          RegistryManager.ClearRegistryMangerInstance();
          BlueStacksUIUtils.RunInstance(requestVmName, hiddenMode, str);
        }
      }
    }

    public static void ShowWindowAndAppHandler(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData requestData = HTTPUtils.ParseRequest(req);
        if (BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
          BlueStacksUIUtils.DictWindows[requestData.RequestVmName].Dispatcher.Invoke((Delegate) (() =>
          {
            HTTPHandler.ShowWindowHandler(requestData);
            HTTPHandler.ShowAppHandler(requestData);
          }));
        HTTPHandler.WriteSuccessJsonArray(res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in Server ShowWindowAndAppHandler: " + ex.ToString());
        HTTPHandler.WriteErrorJsonArray(ex.ToString(), res);
      }
    }

    public static void IsVisibleHandler(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        if (!BlueStacksUIUtils.DictWindows.ContainsKey(request.RequestVmName))
          return;
        if (BlueStacksUIUtils.DictWindows[request.RequestVmName].IsVisible)
          HTTPHandler.WriteSuccessJsonArray(res);
        else
          HTTPHandler.WriteErrorJsonArray("unused", res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in Server IsVisibleHandler: " + ex.ToString());
        HTTPHandler.WriteErrorJsonArray(ex.ToString(), res);
      }
    }

    public static void AppUninstalledHandler(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData requestData = HTTPUtils.ParseRequest(req);
        foreach (string allKey in requestData.Data.AllKeys)
          Logger.Debug("Key: {0}, Value: {1}", (object) allKey, (object) requestData.Data[allKey]);
        string package = requestData.Data["package"];
        string title = requestData.Data["name"];
        Logger.Info("package: " + package);
        if (BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
          BlueStacksUIUtils.DictWindows[requestData.RequestVmName].Dispatcher.Invoke((Delegate) (() => BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mAppHandler.AppUninstalled(package)));
        NotificationManager.Instance.RemoveNotificationItem(title, package);
        Publisher.PublishMessage(BrowserControlTags.appUninstalled, requestData.RequestVmName, new JObject((object) new JProperty("PackageName", (object) package)));
        ClientStats.SendClientStatsAsync("uninstall", "success", "app_install", package, "", "");
        HTTPHandler.WriteSuccessJsonArray(res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in Server AppUninstalledHandler: " + ex.ToString());
        HTTPHandler.WriteErrorJsonArray(ex.ToString(), res);
      }
    }

    public static void AppInstalledHandler(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData requestData = HTTPUtils.ParseRequest(req);
        foreach (string allKey in requestData.Data.AllKeys)
          Logger.Info("Key: {0}, Value: {1}", (object) allKey, (object) requestData.Data[allKey]);
        string package = requestData.Data["package"];
        bool isUpdate = false;
        if (((IEnumerable<string>) requestData.Data.AllKeys).Contains<string>("isUpdate"))
          isUpdate = string.Equals(requestData.Data["isUpdate"], "true", StringComparison.InvariantCultureIgnoreCase);
        Logger.Info("package: " + package + ", isUpdate: " + isUpdate.ToString());
        if (BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
          BlueStacksUIUtils.DictWindows[requestData.RequestVmName].Dispatcher.Invoke((Delegate) (() => BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mAppHandler.AppInstalled(package, isUpdate)));
        HTTPHandler.WriteSuccessJsonArray(res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in Server AppInstalledHandler: " + ex.ToString());
        HTTPHandler.WriteErrorJsonArray(ex.ToString(), res);
      }
    }

    public static void ShowHomeTabHandler(HttpListenerRequest req, HttpListenerResponse _)
    {
      RequestData requestData = HTTPUtils.ParseRequest(req);
      if (!BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
        return;
      BlueStacksUIUtils.DictWindows[requestData.RequestVmName].Dispatcher.Invoke((Delegate) (() =>
      {
        Logger.Info("Switching to Welcome tab");
        BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mTopBar.mAppTabButtons.GoToTab("Home", true, false);
      }));
    }

    public static void ShowWebPageHandler(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData requestData = HTTPUtils.ParseRequest(req);
        string title = requestData.Data["title"].ToString((IFormatProvider) CultureInfo.InvariantCulture);
        string webUrl = requestData.Data["url"].ToString((IFormatProvider) CultureInfo.InvariantCulture);
        if (!BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
          return;
        BlueStacksUIUtils.DictWindows[requestData.RequestVmName].Dispatcher.Invoke((Delegate) (() =>
        {
          HTTPHandler.ShowWindowHandler(requestData);
          BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mTopBar.mAppTabButtons.AddWebTab(webUrl, title, "cef_tab", true, "", false);
        }));
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in Server ShowWebPageHandler : " + ex.ToString());
      }
    }

    public static void ForceQuitHandler(HttpListenerRequest req, HttpListenerResponse _)
    {
      Logger.Info("Quiting BlueStacksUI");
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        bool flag = false;
        try
        {
          flag = Convert.ToBoolean(request.Data["softclose"], (IFormatProvider) CultureInfo.InvariantCulture);
        }
        catch
        {
        }
        if (!BlueStacksUIUtils.DictWindows.ContainsKey(request.RequestVmName))
          return;
        if (flag)
          BlueStacksUIUtils.DictWindows[request.RequestVmName].CloseWindow();
        BlueStacksUIUtils.DictWindows[request.RequestVmName].Dispatcher.Invoke((Delegate) (() => App.ExitApplication()));
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in ForceQuit... Err : " + ex.ToString());
      }
    }

    public static void OpenGoogleHandler(HttpListenerRequest req, HttpListenerResponse _)
    {
      RequestData requestData = HTTPUtils.ParseRequest(req);
      string tabName = "tab_" + (new Random().Next(100) + 1).ToString();
      if (!BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
        return;
      BlueStacksUIUtils.DictWindows[requestData.RequestVmName].Dispatcher.Invoke((Delegate) (() => BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mTopBar.mAppTabButtons.AddWebTab("http://www.google.com", tabName, "cef_tab", true, "", false)));
    }

    private static void WriteSuccessJsonWithVmName(string vmName, HttpListenerResponse res)
    {
      JArray jarray = new JArray();
      JObject jobject = new JObject();
      jobject.Add((object) new JProperty("success", (object) true));
      jobject.Add((object) new JProperty("vmname", (object) vmName));
      jarray.Add((JToken) jobject);
      HTTPUtils.Write(jarray.ToString(Formatting.None), res);
    }

    public static void UnsupportedCPUError(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData requestData = HTTPUtils.ParseRequest(req);
        string reason = requestData.Data["PlusFailureReason"];
        if (!BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
          return;
        BlueStacksUIUtils.DictWindows[requestData.RequestVmName].Dispatcher.Invoke((Delegate) (() =>
        {
          if (System.Windows.Forms.MessageBox.Show(LocaleStrings.GetLocalizedString("STRING_INCOMPATIBLE_FRONTEND_QUIT", ""), LocaleStrings.GetLocalizedString("STRING_INCOMPATIBLE_FRONTEND_QUIT_CAPTION", ""), MessageBoxButtons.OK) != DialogResult.OK)
            return;
          Logger.Info("Quit BlueStacksUI End with reason {0}", (object) reason);
          HTTPHandler.WriteSuccessJsonArray(res);
          BlueStacksUIUtils.DictWindows[requestData.RequestVmName].ForceCloseWindow(false);
        }));
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in QuitBlueStacksUI: " + ex.ToString());
        HTTPHandler.WriteErrorJsonArray(ex.Message, res);
      }
    }

    public static void UpdateUserInfoHandler(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData requestData = HTTPUtils.ParseRequest(req);
        string status = requestData.Data["result"].Trim();
        if (BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mPostOtsWelcomeWindow != null)
          BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mPostOtsWelcomeWindow.ChangeBasedonTokenReceived(status);
        if (status.Equals("true", StringComparison.InvariantCultureIgnoreCase) && BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
        {
          BlueStacksUIUtils.DictWindows[requestData.RequestVmName].Dispatcher.Invoke((Delegate) (() => BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mTopBar.ChangeUserPremiumButton(RegistryManager.Instance.IsPremium)));
          PromotionManager.CheckIsUserPremium();
          System.Action<bool> recommendationHandler = PromotionObject.AppRecommendationHandler;
          if (recommendationHandler != null)
            recommendationHandler(false);
          BlueStacksUIUtils.DictWindows[requestData.RequestVmName].Dispatcher.Invoke((Delegate) (() =>
          {
            if (!BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mLaunchStartupTabWhenTokenReceived || PromotionObject.Instance.StartupTab.Count <= 0)
              return;
            BlueStacksUIUtils.DictWindows[requestData.RequestVmName].Utils.HandleGenericActionFromDictionary((Dictionary<string, string>) PromotionObject.Instance.StartupTab, "startup_action", "");
          }));
          Publisher.PublishMessage(BrowserControlTags.userInfoUpdated, requestData.RequestVmName, (JObject) null);
        }
        HTTPHandler.WriteSuccessJsonArray(res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in UpdateUserInfoHandler: " + ex.ToString());
        HTTPHandler.WriteErrorJsonArray(ex.Message, res);
      }
    }

    internal static void AppInstallStarted(HttpListenerRequest req, HttpListenerResponse _)
    {
      try
      {
        RequestData requestData = HTTPUtils.ParseRequest(req);
        foreach (string allKey in requestData.Data.AllKeys)
          Logger.Info("Key: {0}, Value: {1}", (object) allKey, (object) requestData.Data[allKey]);
        string apkPath = requestData.Data["filePath"];
        if (!BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
          return;
        string package = string.Empty;
        string appName = string.Empty;
        DownloadInstallApk downloader = new DownloadInstallApk(BlueStacksUIUtils.DictWindows[requestData.RequestVmName]);
        if (string.Equals(Path.GetExtension(apkPath), ".xapk", StringComparison.InvariantCultureIgnoreCase))
        {
          JToken infoFromXapk = Utils.ExtractInfoFromXapk(apkPath);
          if (infoFromXapk != null)
          {
            package = infoFromXapk.GetValue("package_name");
            appName = infoFromXapk.GetValue("name");
            Logger.Debug("Package name from manifest.json.." + package);
          }
        }
        else
        {
          AppInfoExtractor apkInfo = AppInfoExtractor.GetApkInfo(apkPath);
          appName = apkInfo.AppName;
          package = apkInfo.PackageName;
        }
        HTTPHandler.dictFileNamesPackageName[apkPath] = package;
        BlueStacksUIUtils.DictWindows[requestData.RequestVmName].Dispatcher.Invoke((Delegate) (() =>
        {
          BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mWelcomeTab.mHomeAppManager.AddAppIcon(package, appName, string.Empty, downloader);
          BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mWelcomeTab.mHomeAppManager.ApkInstallStart(package, apkPath);
        }));
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in GetUserInfo: " + ex.ToString());
      }
    }

    public static void AppInstallFailed(HttpListenerRequest req, HttpListenerResponse _)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        foreach (string allKey in request.Data.AllKeys)
          Logger.Debug("Key: {0}, Value: {1}", (object) allKey, (object) request.Data[allKey]);
        string apkPath = request.Data["filePath"];
        int errorCode = Convert.ToInt32(request.Data["errorCode"], (IFormatProvider) CultureInfo.InvariantCulture);
        string vmName = request.RequestVmName;
        if (!BlueStacksUIUtils.DictWindows.ContainsKey(vmName))
          return;
        BlueStacksUIUtils.DictWindows[vmName].Dispatcher.Invoke((Delegate) (() =>
        {
          try
          {
            BlueStacksUIUtils.DictWindows[vmName].mWelcomeTab.mHomeAppManager.ApkInstallFailed(HTTPHandler.dictFileNamesPackageName[apkPath]);
            HTTPHandler.ShowErrorPromptIfNeeded(vmName, errorCode);
          }
          catch (Exception ex)
          {
            Logger.Error("error in install failed http call: {0}", (object) ex);
          }
        }));
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in AppInstallFailed. Err : " + ex.ToString());
      }
    }

    private static void ShowErrorPromptIfNeeded(string vmName, int errorCode)
    {
      string empty = string.Empty;
      string str = errorCode != 10 ? LocaleStrings.GetLocalizedString("STRING_INVALID_APK_BLACKLISTED_ERROR", "") : LocaleStrings.GetLocalizedString("STRING_INVALID_APK_BLACKLISTED_ERROR", "");
      if (string.IsNullOrEmpty(str))
        return;
      CustomMessageWindow customMessageWindow = new CustomMessageWindow();
      customMessageWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_INSTALLATION_ERROR", "");
      customMessageWindow.BodyTextBlock.Text = str;
      customMessageWindow.AddButton(ButtonColors.Blue, LocaleStrings.GetLocalizedString("STRING_OK", ""), (EventHandler) null, (string) null, false, (object) null, true);
      customMessageWindow.Owner = (Window) BlueStacksUIUtils.DictWindows[vmName];
      BlueStacksUIUtils.DictWindows[vmName].ShowDimOverlay((IDimOverlayControl) null);
      customMessageWindow.ShowDialog();
      BlueStacksUIUtils.DictWindows[vmName].HideDimOverlay();
    }

    public static void GooglePlayAppInstall(HttpListenerRequest req, HttpListenerResponse _)
    {
      try
      {
        RequestData requestData = HTTPUtils.ParseRequest(req);
        foreach (string allKey in requestData.Data.AllKeys)
          Logger.Info("Key: {0}, Value: {1}", (object) allKey, (object) requestData.Data[allKey]);
        string packageName = requestData.Data["packageName"];
        string appName = requestData.Data["appName"];
        string isAdditionalFile = requestData.Data["isAdditionalFile"];
        string status = requestData.Data["status"];
        if (string.IsNullOrEmpty(status) || !BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
          return;
        BlueStacksUIUtils.DictWindows[requestData.RequestVmName].Dispatcher.Invoke((Delegate) (() =>
        {
          AppIconModel appIcon = BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mWelcomeTab.mHomeAppManager.GetAppIcon(packageName);
          if (appIcon != null && appIcon.mIsAppInstalled)
            return;
          if (status.Equals("STARTED", StringComparison.InvariantCultureIgnoreCase))
          {
            BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mWelcomeTab.mHomeAppManager.AddAppIcon(packageName, appName, string.Empty, (DownloadInstallApk) null);
            BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mWelcomeTab.mHomeAppManager.ApkInstallStart(packageName, string.Empty);
          }
          if (status.Equals("SUCCESS", StringComparison.InvariantCultureIgnoreCase) && isAdditionalFile.Equals("false", StringComparison.OrdinalIgnoreCase))
          {
            BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mWelcomeTab.mHomeAppManager.ApkInstallCompleted(packageName);
          }
          else
          {
            if (!status.Equals("CANCELED", StringComparison.InvariantCultureIgnoreCase))
              return;
            BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mWelcomeTab.mHomeAppManager.RemoveAppIcon(packageName, (AppIconModel) null);
          }
        }));
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in GooglePlayAppInstall: " + ex.ToString());
      }
    }

    internal static void ChangeTextOTSHandler(HttpListenerRequest req, HttpListenerResponse _)
    {
      try
      {
        RequestData requestData = HTTPUtils.ParseRequest(req);
        if (!BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
          return;
        BlueStacksUIUtils.DictWindows[requestData.RequestVmName].Dispatcher.Invoke((Delegate) (() =>
        {
          BlueStacksUIBinding.Bind(BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mFrontendOTSControl.mBaseControl.mTitleLabel, "STRING_WELCOME_TO_BLUESTACKS");
          Logger.Info("string set after change text OTS .." + BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mFrontendOTSControl.mBaseControl.mTitleLabel.Content?.ToString());
        }));
      }
      catch (Exception ex)
      {
        Logger.Error("error in change ots text." + ex.ToString());
      }
    }

    internal static void ShootingModeChanged(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData requestData = HTTPUtils.ParseRequest(req);
        if (!BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
          return;
        BlueStacksUIUtils.DictWindows[requestData.RequestVmName].Dispatcher.Invoke((Delegate) (() =>
        {
          BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mFrontendHandler.IsShootingModeActivated = Convert.ToBoolean(requestData.Data["IsShootingModeActivated"], (IFormatProvider) CultureInfo.InvariantCulture);
          if (BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mFrontendHandler.IsShootingModeActivated)
          {
            BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mFullscreenSidebarPopup.IsOpen = false;
            BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mFullscreenTopbarPopup.IsOpen = false;
            BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mFullscreenSidebarPopupButton.IsOpen = false;
            BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mFullscreenTopbarPopupButton.IsOpen = false;
          }
          else
            BlueStacksUIUtils.DictWindows[requestData.RequestVmName].mCommonHandler.ClipMouseCursorHandler(false, false, "", "");
        }));
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in Shooting Mode Changed: " + ex.ToString());
        HTTPHandler.WriteErrorJsonArray(ex.Message, res);
      }
    }

    internal static void ChangeOrientaionHandler(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        string packagename = request.Data["package"].ToString((IFormatProvider) CultureInfo.InvariantCulture);
        bool boolean = Convert.ToBoolean(request.Data["is_portrait"], (IFormatProvider) CultureInfo.InvariantCulture);
        if (BlueStacksUIUtils.DictWindows.ContainsKey(request.RequestVmName))
          BlueStacksUIUtils.DictWindows[request.RequestVmName].Frontend_OrientationChanged(packagename, boolean);
        HTTPHandler.WriteSuccessJsonArray(res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in ChangeOrientaionHandler: " + ex.ToString());
        HTTPHandler.WriteErrorJSONObjectWithoutReason(res);
      }
    }

    internal static void ShowGrmAndLaunchAppHandler(
      HttpListenerRequest req,
      HttpListenerResponse res)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        string packageName = request.Data["package"].ToString((IFormatProvider) CultureInfo.InvariantCulture);
        string vmName = request.RequestVmName;
        BlueStacksUIUtils.DictWindows[vmName].Dispatcher.Invoke((Delegate) (() =>
        {
          if (!BlueStacksUIUtils.DictWindows.ContainsKey(vmName) || BlueStacksUIUtils.DictWindows[vmName].mWelcomeTab.mHomeAppManager.GetAppIcon(packageName) == null)
            return;
          BlueStacksUIUtils.DictWindows[vmName].mWelcomeTab.mHomeAppManager.OpenApp(packageName, true);
        }));
        HTTPHandler.WriteSuccessJsonArray(res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in ShowGrmAndLaunchAppHandler: " + ex.ToString());
        HTTPHandler.WriteErrorJSONObjectWithoutReason(res);
      }
    }

    internal static void UpdateSizeOfOverlay(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData requestData = HTTPUtils.ParseRequest(req);
        if (!BlueStacksUIUtils.DictWindows.ContainsKey(requestData.RequestVmName))
          return;
        BlueStacksUIUtils.DictWindows[requestData.RequestVmName].Dispatcher.Invoke((Delegate) (() =>
        {
          try
          {
            IntPtr num = new IntPtr(Convert.ToInt32(requestData.Data["handle"], (IFormatProvider) CultureInfo.InvariantCulture));
            BlueStacksUIUtils.DictWindows[requestData.RequestVmName].StaticComponents.mLastMappableWindowHandle = num;
            if (!KMManager.dictOverlayWindow.ContainsKey(BlueStacksUIUtils.DictWindows[requestData.RequestVmName]))
              return;
            KMManager.dictOverlayWindow[BlueStacksUIUtils.DictWindows[requestData.RequestVmName]].UpdateSize();
          }
          catch (Exception ex)
          {
            Logger.Error("Exception in UpdateSizeOfOverlay: " + ex.ToString());
            HTTPHandler.WriteErrorJsonArray(ex.Message, res);
          }
        }));
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in UpdateSizeOfOverlay: " + ex.ToString());
        HTTPHandler.WriteErrorJsonArray(ex.Message, res);
      }
    }

    internal static void BootFailedPopupHandler(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        if (BlueStacksUIUtils.DictWindows.ContainsKey(request.RequestVmName))
          BlueStacksUIUtils.DictWindows[request.RequestVmName].Utils.SendGuestBootFailureStats("com exception");
        HTTPHandler.WriteSuccessJsonArray(res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in BootFailedPopupHandler: " + ex.ToString());
        HTTPHandler.WriteErrorJSONObjectWithoutReason(res);
      }
    }

    internal static void DragDropInstallHandler(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        string requestVmName = request.RequestVmName;
        string apkPath = request.Data["filePath"].ToString((IFormatProvider) CultureInfo.InvariantCulture);
        if (BlueStacksUIUtils.DictWindows.ContainsKey(request.RequestVmName))
          new DownloadInstallApk(BlueStacksUIUtils.DictWindows[requestVmName]).InstallApk(apkPath, true);
        HTTPHandler.WriteSuccessJsonArray(res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in DragDropInstallHandler: " + ex.ToString());
        HTTPHandler.WriteErrorJSONObjectWithoutReason(res);
      }
    }

    internal static void DeviceProvisionedHandler(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        Logger.Info("Device provisioned client");
        RequestData request = HTTPUtils.ParseRequest(req);
        string requestVmName = request.RequestVmName;
        if (BlueStacksUIUtils.DictWindows.ContainsKey(request.RequestVmName))
          BlueStacksUIUtils.DictWindows[request.RequestVmName].mAppHandler.IsOneTimeSetupCompleted = true;
        HTTPHandler.WriteSuccessJsonArray(res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in DeviceProvisionedHandler: " + ex.ToString());
        HTTPHandler.WriteErrorJSONObjectWithoutReason(res);
      }
    }

    internal static void GoogleSigninHandler(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        string requestVmName = request.RequestVmName;
        string email = request.Data["email"].ToString((IFormatProvider) CultureInfo.InvariantCulture).Trim();
        if (BlueStacksUIUtils.DictWindows.ContainsKey(request.RequestVmName))
        {
          RegistryManager.Instance.Guest[request.RequestVmName].IsGoogleSigninDone = true;
          string campaignID = "";
          if (!string.IsNullOrEmpty(BlueStacksUIUtils.DictWindows[request.RequestVmName].WindowLaunchParams) && JObject.Parse(BlueStacksUIUtils.DictWindows[request.RequestVmName].WindowLaunchParams)["campaign_id"] != null)
            campaignID = JObject.Parse(BlueStacksUIUtils.DictWindows[request.RequestVmName].WindowLaunchParams)["campaign_id"].ToString();
          BlueStacks.Common.Stats.SendUnifiedInstallStatsAsync("google_login_completed", email, request.RequestVmName, campaignID);
          BlueStacksUIUtils.DictWindows[request.RequestVmName].PostGoogleSigninCompleteTask();
          Publisher.PublishMessage(BrowserControlTags.googleSigninComplete, request.RequestVmName, (JObject) null);
        }
        HTTPHandler.WriteSuccessJsonArray(res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in GoogleSigninHandler: " + ex.ToString());
        HTTPHandler.WriteErrorJSONObjectWithoutReason(res);
      }
    }

    internal static void SetDMMKeymapping(HttpListenerRequest req, HttpListenerResponse _)
    {
      Logger.Info("Got SetKeymapping {0} request from {1}", (object) req.HttpMethod, (object) req.RemoteEndPoint.ToString());
      try
      {
        RequestData request = HTTPUtils.ParseRequest(req);
        string vmName = request.RequestVmName;
        string package = request.Data["package"].ToString((IFormatProvider) CultureInfo.InvariantCulture);
        bool isKeymapEnabled = Convert.ToBoolean(request.Data["enablekeymap"], (IFormatProvider) CultureInfo.InvariantCulture);
        Logger.Info("package : " + package + " enablekeymap : " + isKeymapEnabled.ToString());
        if (!BlueStacksUIUtils.DictWindows.ContainsKey(vmName))
          return;
        int retries = 3;
        while (retries > 0)
        {
          BlueStacksUIUtils.DictWindows[vmName].Dispatcher.Invoke((Delegate) (() =>
          {
            if (BlueStacksUIUtils.DictWindows[vmName].Visibility != Visibility.Visible || !BlueStacksUIUtils.DictWindows[vmName].mTopBar.mAppTabButtons.mDictTabs.ContainsKey(package))
              return;
            retries = 0;
            BlueStacksUIUtils.DictWindows[vmName].mTopBar.mAppTabButtons.mDictTabs[package].IsDMMKeymapEnabled = isKeymapEnabled;
          }));
          if (retries > 0)
          {
            retries--;
            Thread.Sleep(1000);
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in Server SetKeymapping: " + ex.ToString());
      }
    }

    internal static void ReloadShortcuts(HttpListenerRequest req, HttpListenerResponse _)
    {
      try
      {
        string requestVmName = HTTPUtils.ParseRequest(req).RequestVmName;
        if (!BlueStacksUIUtils.DictWindows.ContainsKey(requestVmName))
          return;
        BlueStacksUIUtils.DictWindows[requestVmName].Dispatcher.Invoke((Delegate) (() => CommonHandlers.ReloadShortcutsForAllInstances()));
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in ReloadShortcuts: " + ex.ToString());
      }
    }

    internal static void ReloadPromotions(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        string requestVmName = HTTPUtils.ParseRequest(req).RequestVmName;
        if (!BlueStacksUIUtils.DictWindows.ContainsKey(requestVmName))
          return;
        PromotionManager.ReloadPromotionsAsync();
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in ReloadPromotions: " + ex.ToString());
      }
    }

    internal static void HandleOverlayControlsVisibility(
      HttpListenerRequest req,
      HttpListenerResponse res)
    {
      try
      {
        RequestData requestData = HTTPUtils.ParseRequest(req);
        string vmName = requestData.RequestVmName;
        BlueStacksUIUtils.DictWindows[vmName].Dispatcher.Invoke((Delegate) (() =>
        {
          if (BlueStacksUIUtils.DictWindows.ContainsKey(vmName) && RegistryManager.Instance.IsGameTvEnabled)
          {
            string data = requestData.Data["data"];
            KMManager.HandleCallbackControl(BlueStacksUIUtils.DictWindows[vmName], data);
          }
          if (!BlueStacksUIUtils.DictWindows.ContainsKey(vmName) || !BlueStacksUIUtils.DictWindows[vmName].IsActive && (KMManager.sGuidanceWindow == null || KMManager.sGuidanceWindow.ParentWindow != BlueStacksUIUtils.DictWindows[vmName] || !KMManager.sGuidanceWindow.IsActive) && (KMManager.CanvasWindow == null || KMManager.CanvasWindow.ParentWindow != BlueStacksUIUtils.DictWindows[vmName] || (KMManager.CanvasWindow.SidebarWindow == null || !KMManager.CanvasWindow.SidebarWindow.IsActive)))
            return;
          string data1 = requestData.Data["data"];
          KMManager.ShowDynamicOverlay(BlueStacksUIUtils.DictWindows[vmName], true, false, data1);
        }));
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in HandleOverlayControlsVisibility: " + ex.ToString());
      }
    }
  }
}
