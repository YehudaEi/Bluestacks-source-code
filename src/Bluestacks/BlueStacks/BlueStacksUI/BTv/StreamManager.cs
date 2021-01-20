// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.BTv.StreamManager
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
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;

namespace BlueStacks.BlueStacksUI.BTv
{
  public class StreamManager : IDisposable
  {
    internal static string mSelectedMic = string.Empty;
    internal static bool mIsWebcamDisabled = true;
    internal static bool mIsMicDisabled = true;
    internal static Dictionary<string, string> mDictCameraDetails = new Dictionary<string, string>();
    internal static Dictionary<string, string> mDictMicDetails = new Dictionary<string, string>();
    internal static Dictionary<string, string> mDictLastCameraPosition = new Dictionary<string, string>();
    private object mObsCommandQueueObject = new object();
    private object mObsSendRequestObject = new object();
    private object mInitOBSLock = new object();
    private string mFailureReason = "";
    private string mAppHandle = "";
    private string mAppPid = "";
    private object stoppingOBSLock = new object();
    private int widthDiff = 14;
    internal Fraction mAspectRatio = new Fraction(16L, 9L);
    private int mObsRetryCount = 2;
    private static Queue<StreamManager.ObsCommand> mObsCommandQueue;
    private EventWaitHandle mObsCommandEventHandle;
    private bool mIsStreamStarted;
    private static int mMicVolume;
    internal static string mSelectedCamera;
    private static int mSystemVolume;
    internal int mStreamWidth;
    internal int mStreamHeight;
    private Browser mBrowser;
    private int heightDiff;
    private MainWindow mWindow;
    private string mLastVideoFilePath;
    private bool disposedValue;

    public static string ObsServerBaseURL { get; set; } = "http://localhost";

    public static int ObsServerPort { get; set; } = 2851;

    public static string DEFAULT_NETWORK { get; set; } = "twitch";

    public static bool DEFAULT_ENABLE_FILTER { get; set; } = true;

    public static bool DEFAULT_SQUARE_THEME { get; set; } = false;

    public static string DEFAULT_LAYOUT_THEME { get; set; } = (string) null;

    public static bool sStopInitOBSQueue { get; set; } = false;

    public string mCallbackStreamStatus { get; set; }

    public string mCallbackAppInfo { get; set; }

    public bool mIsObsRunning { get; set; }

    public bool mIsInitCalled { get; set; }

    public bool mIsStreaming { get; set; }

    public bool mStoppingOBS { get; set; }

    public bool mIsReconnecting { get; set; }

    public string mNetwork { get; set; } = StreamManager.DEFAULT_NETWORK;

    public bool mSquareTheme { get; set; } = StreamManager.DEFAULT_SQUARE_THEME;

    public string mLayoutTheme { get; set; } = StreamManager.DEFAULT_LAYOUT_THEME;

    public string mLastCameraLayoutTheme { get; set; } = StreamManager.DEFAULT_LAYOUT_THEME;

    public bool mAppViewLayout { get; set; }

    public bool mEnableFilter { get; set; } = StreamManager.DEFAULT_ENABLE_FILTER;

    public static string CamStatus { get; set; }

    public bool mReplayBufferEnabled { get; set; }

    public bool mCLRBrowserRunning { get; set; }

    public string mCurrentFilterAppPkg { get; set; }

    public static StreamManager Instance { get; set; } = (StreamManager) null;

    public event EventHandler<CustomVolumeEventArgs> EventGetSystemVolume;

    public event EventHandler<CustomVolumeEventArgs> EventGetMicVolume;

    public event EventHandler<CustomVolumeEventArgs> EventGetCameraDetails;

    public event EventHandler<CustomVolumeEventArgs> EventGetMicDetails;

    public bool isWindowCaptureActive { get; set; }

    public StreamManager(Browser browser)
    {
      StreamManager.Instance = this;
      this.mBrowser = browser;
      this.mReplayBufferEnabled = RegistryManager.Instance.ReplayBufferEnabled == 1;
      StreamManager.CamStatus = RegistryManager.Instance.CamStatus != 1 ? "false" : "true";
      StreamManager.mSelectedCamera = RegistryManager.Instance.SelectedCam;
      this.mObsCommandEventHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
      MainWindow mainWindow = (MainWindow) null;
      if (BlueStacksUIUtils.DictWindows.Count > 0)
        mainWindow = BlueStacksUIUtils.DictWindows.Values.First<MainWindow>();
      this.mWindow = mainWindow;
      this.CopySceneConfigFile(this.mWindow, false);
    }

    public StreamManager(MainWindow window)
    {
      StreamManager.Instance = this;
      this.mReplayBufferEnabled = true;
      StreamManager.CamStatus = RegistryManager.Instance.CamStatus != 1 ? "false" : "true";
      StreamManager.mSelectedCamera = RegistryManager.Instance.SelectedCam;
      this.mObsCommandEventHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
      this.mWindow = window;
      this.CopySceneConfigFile(this.mWindow, !RegistryManager.Instance.IsGameCaptureSupportedInMachine);
    }

    public void CopySceneConfigFile(MainWindow activatedWindow, bool forceWindowCaptureMode = false)
    {
      Logger.Debug("In Scene config file copy method with glmode: {0}", (object) activatedWindow?.EngineInstanceRegistry.GlRenderMode);
      string path = Path.Combine(RegistryStrings.ObsDir, "sceneCollection");
      try
      {
        if (!Directory.Exists(path))
          Directory.CreateDirectory(path);
        if (activatedWindow.EngineInstanceRegistry.GlRenderMode != 1 | forceWindowCaptureMode)
        {
          string sourceFileName = Path.Combine(RegistryStrings.ObsDir, "SceneConfigFiles\\scenes_window.xconfig");
          string destFileName1 = Path.Combine(RegistryStrings.ObsDir, "scenes.xconfig");
          string destFileName2 = Path.Combine(RegistryStrings.ObsDir, "sceneCollection\\scenes.xconfig");
          File.Copy(sourceFileName, destFileName1, true);
          File.Copy(sourceFileName, destFileName2, true);
          this.isWindowCaptureActive = true;
        }
        else
        {
          string sourceFileName = Path.Combine(RegistryStrings.ObsDir, "SceneConfigFiles\\scenes_graphics.xconfig");
          string destFileName1 = Path.Combine(RegistryStrings.ObsDir, "scenes.xconfig");
          string destFileName2 = Path.Combine(RegistryStrings.ObsDir, "sceneCollection\\scenes.xconfig");
          File.Copy(sourceFileName, destFileName1, true);
          File.Copy(sourceFileName, destFileName2, true);
          this.isWindowCaptureActive = false;
        }
        Logger.Debug("Is window capture active..: {0}", (object) this.isWindowCaptureActive);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in obs scene config file : {0}", (object) ex);
      }
    }

    internal void OrientationChangeHandler()
    {
      try
      {
        if (this.isWindowCaptureActive)
          this.SetCaptureSize();
        this.RefreshCapture();
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in OrientationChangeHandler : " + ex.ToString());
      }
    }

    private void RefreshCapture()
    {
      this.SendObsRequest("refreshCapture", (Dictionary<string, string>) null, (string) null, (string) null, 0, false);
    }

    public void SendCurrentAppInfoAtTabChange()
    {
      try
      {
        if (this.mBrowser == null || string.IsNullOrEmpty(this.mBrowser.getURL()))
          return;
        AppTabButton selectedTab = this.mWindow.mTopBar.mAppTabButtons.SelectedTab;
        if (selectedTab == null)
          return;
        selectedTab.mTabType.ToString();
        this.mBrowser.ExecuteJavaScript("getCurrentAppInfo('" + new JObject()
        {
          {
            "type",
            (JToken) "app"
          },
          {
            "name",
            (JToken) selectedTab.AppName
          },
          {
            "data",
            (JToken) selectedTab.PackageName
          }
        }.ToString(Formatting.None).Replace("'", "&#39;").Replace("%27", "&#39;") + "')", this.mBrowser.getURL(), 0);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in send current app status on tab changed " + ex.ToString());
      }
    }

    public void StartObs()
    {
      if (this.mIsInitCalled)
        return;
      this.InitObs();
    }

    private void InitObs()
    {
      this.mIsInitCalled = true;
      Utils.KillCurrentOemProcessByName("HD-OBS", (string) null);
      if (!ProcessUtils.FindProcessByName("HD-OBS") && !StreamManager.sStopInitOBSQueue)
        StreamManager.StartOBS();
      if (StreamManager.sStopInitOBSQueue)
        return;
      try
      {
        Logger.Info("response for ping is {0}", (object) this.SendObsRequestInternal("ping", (Dictionary<string, string>) null));
        this.mIsObsRunning = true;
      }
      catch (Exception ex)
      {
        if (StreamManager.sStopInitOBSQueue || StreamManager.Instance == null)
          return;
        Logger.Error("Exception in InitObs. err: " + ex.ToString());
        Thread.Sleep(100);
        if (this.mObsRetryCount > 0)
        {
          --this.mObsRetryCount;
          this.InitObs();
        }
        else
        {
          this.ShutDownForcefully();
          throw new Exception("Could not start OBS.");
        }
      }
      this.mObsRetryCount = 2;
      new Thread((ThreadStart) (() => this.ProcessObsCommandQueue()))
      {
        IsBackground = true
      }.Start();
      if (this.mReplayBufferEnabled)
        this.SetReplayBufferSavePath();
      this.GetParametersFromOBS();
      this.EnableSource("BlueStacks");
    }

    private void SetBackGroundImagePath()
    {
      this.EnableSource("BackGroundImage");
      this.SendObsRequest("setBackground", new Dictionary<string, string>()
      {
        {
          "path",
          Path.Combine(RegistryStrings.ObsDir, "backgrounds\\Background3.jpg")
        }
      }, (string) null, (string) null, 0);
    }

    public void SetSceneConfiguration(string layoutTheme)
    {
      this.mAppViewLayout = false;
      this.mLayoutTheme = layoutTheme;
      if (layoutTheme == null)
      {
        this.SendObsRequest("resettooriginalscene", (Dictionary<string, string>) null, (string) null, (string) null, 0, false);
      }
      else
      {
        this.SetCaptureSize();
        this.DisableSource("CLR Browser");
        try
        {
          JObject jobject1 = JObject.Parse(layoutTheme);
          Logger.Info(layoutTheme);
          bool flag = StreamManager.IsPortraitApp();
          if (jobject1["isPortrait"] != null)
            flag = jobject1["isPortrait"].ToObject<bool>();
          JObject jobject2 = !flag ? JObject.Parse(jobject1["landscape"].ToString()) : JObject.Parse(jobject1["portrait"].ToString());
          if (jobject2["BlueStacksWebcam"] != null)
          {
            bool result;
            if (bool.TryParse(jobject2["BlueStacksWebcam"][(object) "enableWebCam"].ToString(), out result) & result)
              this.SetCameraPosition(Convert.ToInt32(jobject2["BlueStacksWebcam"][(object) "x"].ToString(), (IFormatProvider) CultureInfo.InvariantCulture), Convert.ToInt32(jobject2["BlueStacksWebcam"][(object) "y"].ToString(), (IFormatProvider) CultureInfo.InvariantCulture), Convert.ToInt32(jobject2["BlueStacksWebcam"][(object) "width"].ToString(), (IFormatProvider) CultureInfo.InvariantCulture), Convert.ToInt32(jobject2["BlueStacksWebcam"][(object) "height"].ToString(), (IFormatProvider) CultureInfo.InvariantCulture), result ? 1 : 0);
            else
              this.DisableWebcamAndClearDictionary();
          }
          if (jobject2["BlueStacks"] != null)
          {
            string str1 = jobject2["BlueStacks"][(object) "width"].ToString();
            string str2 = jobject2["BlueStacks"][(object) "height"].ToString();
            string str3 = jobject2["BlueStacks"][(object) "x"].ToString();
            string str4 = jobject2["BlueStacks"][(object) "y"].ToString();
            if (!this.isWindowCaptureActive)
            {
              str1 = str2;
              if (jobject1["name"] != null)
              {
                string a = jobject1["name"].ToString();
                if (string.Equals(a, "layout_2", StringComparison.InvariantCulture) || string.Equals(a, "layout_3", StringComparison.InvariantCulture))
                {
                  str3 = "22";
                  if (flag && string.Equals(a, "layout_3", StringComparison.InvariantCulture))
                    str3 = "0";
                }
                else if (string.Equals(a, "layout_1", StringComparison.InvariantCulture))
                  str3 = "47";
              }
            }
            this.SetFrontendPosition(0, 0, Convert.ToInt32(str1, (IFormatProvider) CultureInfo.InvariantCulture), Convert.ToInt32(str2, (IFormatProvider) CultureInfo.InvariantCulture));
            this.SetPosition(Convert.ToInt32(str3, (IFormatProvider) CultureInfo.InvariantCulture), Convert.ToInt32(str4, (IFormatProvider) CultureInfo.InvariantCulture));
            this.EnableSource("BlueStacks");
          }
          else
            this.DisableSource("BlueStacks");
          string str5 = jobject1["order"].ToString();
          string str6 = jobject1["logo"].ToString();
          string str7 = str5 + ",BackGroundImage";
          string str8 = "watermarkFB,watermark,watermarkGif";
          if (jobject1["allLogo"] != null)
            str8 = jobject1["allLogo"].ToString();
          this.SendObsRequest("setorderandlogo", new Dictionary<string, string>()
          {
            {
              "order",
              str7
            },
            {
              "logo",
              str6
            },
            {
              "allLogo",
              str8
            }
          }, (string) null, (string) null, 0, false);
        }
        catch (Exception ex)
        {
          Logger.Error("SetSceneConfiguration: Error {0}", (object) ex);
        }
      }
    }

    public static bool IsPortraitApp()
    {
      return RegistryManager.Instance.FrontendWidth <= RegistryManager.Instance.FrontendHeight;
    }

    private static void StartOBS()
    {
      Logger.Info("starting obs");
      string obsBinaryPath = RegistryStrings.ObsBinaryPath;
      string str = "-port " + RegistryManager.Instance.PartnerServerPort.ToString();
      if (SystemUtils.IsOs64Bit())
        str += " -64bit";
      if (!string.IsNullOrEmpty(BlueStacks.Common.Strings.OEMTag))
        str = str + " -oem " + BlueStacks.Common.Strings.OEMTag;
      string args = str;
      ProcessUtils.GetProcessObject(obsBinaryPath, args, false).Start();
      Logger.Info("OBS started");
      StreamManager.ObsServerPort = RegistryManager.Instance.OBSServerPort;
    }

    public void SetHwnd(string handle)
    {
      this.SendObsRequest("sethwnd", new Dictionary<string, string>()
      {
        {
          "hwnd",
          handle
        }
      }, (string) null, (string) null, 0, false);
    }

    public void SetSavePath(string path = null)
    {
      Dictionary<string, string> data = new Dictionary<string, string>();
      string str = path ?? Path.Combine(RegistryStrings.BtvDir, "stream.flv");
      this.mLastVideoFilePath = str;
      data.Add("savepath", str);
      this.SendObsRequest("setsavepath", data, (string) null, "SetSaveFailed", 0, false);
    }

    private void SetSaveFailed()
    {
      Logger.Error("Exception in SetSaveFailed");
    }

    private void SetReplayBufferSavePath()
    {
      Dictionary<string, string> data = new Dictionary<string, string>();
      string str = Path.Combine(RegistryStrings.BtvDir, "replay.flv");
      data.Add("savepath", str);
      this.SendObsRequest("setreplaybuffersavepath", data, (string) null, (string) null, 0, false);
    }

    public static void SetStreamDimension(
      out int startX,
      out int startY,
      out int width,
      out int height)
    {
      try
      {
        BTVManager.GetStreamDimensionInfo(out startX, out startY, out width, out height);
      }
      catch (Exception ex)
      {
        Logger.Error("Got Exception in getting stream dimension... Err : " + ex.ToString());
        startX = startY = width = height = 0;
      }
    }

    public void SetFrontendPosition()
    {
      int startX;
      int startY;
      int width;
      int height;
      StreamManager.SetStreamDimension(out startX, out startY, out width, out height);
      int frontendHeight = Utils.GetInt(RegistryManager.Instance.FrontendHeight, height);
      this.SetFrontendPosition(!this.isWindowCaptureActive ? (int) this.GetWidthFromHeight((double) frontendHeight) : RegistryManager.Instance.FrontendWidth, frontendHeight, startX, startY, width, height);
    }

    public void SetFrontendPosition(int frontendWidth, int frontendHeight)
    {
      int startX;
      int startY;
      int width;
      int height;
      StreamManager.SetStreamDimension(out startX, out startY, out width, out height);
      this.SetFrontendPosition(frontendWidth, frontendHeight, startX, startY, width, height);
    }

    public void SetFrontendPosition(
      int frontendWidth,
      int frontendHeight,
      int startX,
      int startY,
      int width,
      int height)
    {
      startY += (height - frontendHeight) / 2;
      startX += (width - frontendWidth) / 2;
      if (this.mEnableFilter)
      {
        int width1 = frontendWidth * 100 / (frontendHeight * 16 / 9);
        int startX1 = (100 - width1) / 2;
        this.SetFrontendPosition(startX1, 0, width1, 100);
        this.SetPosition(startX1, 0);
      }
      else
      {
        this.SetFrontendPosition(0, 0, 100, 100);
        if (!this.mSquareTheme)
          this.SetPosition(0, 0);
      }
      this.SetCaptureSize(startX, startY, frontendWidth, frontendHeight);
    }

    public void SetFrontendPosition(int startX, int startY, int width, int height)
    {
      this.SendObsRequest("setsourceposition", new Dictionary<string, string>()
      {
        {
          nameof (width),
          width.ToString((IFormatProvider) CultureInfo.InvariantCulture)
        },
        {
          nameof (height),
          height.ToString((IFormatProvider) CultureInfo.InvariantCulture)
        },
        {
          "y",
          startY.ToString((IFormatProvider) CultureInfo.InvariantCulture)
        },
        {
          "x",
          startX.ToString((IFormatProvider) CultureInfo.InvariantCulture)
        },
        {
          "source",
          "BlueStacks"
        }
      }, (string) null, (string) null, 0, false);
    }

    public void SetPosition(int startX, int startY)
    {
      this.SendObsRequest("setposition", new Dictionary<string, string>()
      {
        {
          "y",
          startY.ToString((IFormatProvider) CultureInfo.InvariantCulture)
        },
        {
          "x",
          startX.ToString((IFormatProvider) CultureInfo.InvariantCulture)
        },
        {
          "source",
          "BlueStacks"
        }
      }, (string) null, (string) null, 0, false);
    }

    public void SetFrontEndCaptureSize(int width, int height)
    {
      this.SendObsRequest("SetFrontendCaptureSize", new Dictionary<string, string>()
      {
        {
          nameof (width),
          width.ToString((IFormatProvider) CultureInfo.InvariantCulture)
        },
        {
          nameof (height),
          height.ToString((IFormatProvider) CultureInfo.InvariantCulture)
        },
        {
          "source",
          "BlueStacks"
        }
      }, (string) null, (string) null, 0, false);
    }

    public void SetCaptureSize()
    {
      int startX1;
      int startY1;
      int width1;
      int height1;
      StreamManager.SetStreamDimension(out startX1, out startY1, out width1, out height1);
      int height2 = Utils.GetInt(RegistryManager.Instance.FrontendHeight, height1);
      int startY2 = startY1 + (height1 - height2) / 2;
      int width2 = this.isWindowCaptureActive ? RegistryManager.Instance.FrontendWidth : (int) this.GetWidthFromHeight((double) height2);
      int startX2 = startX1 + (width1 - width2) / 2;
      Logger.Info("frontendWidth for set capture size : " + width2.ToString());
      Logger.Info("frontendHeight for set capture size : " + height2.ToString());
      this.SetCaptureSize(startX2, startY2, width2, height2);
    }

    private void SetCaptureSize(int startX, int startY, int width, int height)
    {
      Dictionary<string, string> data = new Dictionary<string, string>();
      Logger.Info("width for set capture size : " + width.ToString());
      Logger.Info("height for set capture size : " + height.ToString());
      data.Add(nameof (width), width.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      data.Add(nameof (height), height.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      data.Add("x", startX.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      data.Add("y", startY.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      data.Add("source", "BlueStacks");
      this.SendObsRequest("setcapturesize", data, (string) null, (string) null, 0);
      if (!this.isWindowCaptureActive)
        return;
      this.SetFrontEndCaptureSize(RegistryManager.Instance.FrontendWidth, RegistryManager.Instance.FrontendHeight);
      if (StreamManager.IsPortraitApp())
        this.SetPosition(35, 0);
      else
        this.SetPosition(0, 0);
    }

    public void ResetCLRBrowser(bool isSetFrontEndPosition = true)
    {
      this.DisableSource("CLR Browser");
      if (isSetFrontEndPosition)
        this.SetFrontendPosition();
      if (string.Compare(StreamManager.CamStatus, "true", StringComparison.OrdinalIgnoreCase) == 0)
        this.EnableWebcamInternal("320", "240", "3");
      else
        this.DisableWebcamAndClearDictionary();
      this.mCLRBrowserRunning = false;
      this.mCurrentFilterAppPkg = (string) null;
    }

    internal void EnableVideoRecording(bool enable)
    {
      Dictionary<string, string> data = new Dictionary<string, string>();
      if (enable)
        data.Add("Enable", "1");
      else
        data.Add("Enable", "0");
      this.SendObsRequest(nameof (EnableVideoRecording), data, (string) null, (string) null, 0);
    }

    private void SetCameraPosition(int x, int y, int width, int height, int render)
    {
      Logger.Info("camera position width is : " + width.ToString() + " and height is :" + height.ToString());
      StreamManager.mIsWebcamDisabled = false;
      Dictionary<string, string> data = new Dictionary<string, string>()
      {
        {
          "source",
          "BlueStacksWebcam"
        },
        {
          nameof (width),
          width.ToString((IFormatProvider) CultureInfo.InvariantCulture)
        },
        {
          nameof (height),
          height.ToString((IFormatProvider) CultureInfo.InvariantCulture)
        },
        {
          nameof (x),
          x.ToString((IFormatProvider) CultureInfo.InvariantCulture)
        },
        {
          nameof (y),
          y.ToString((IFormatProvider) CultureInfo.InvariantCulture)
        },
        {
          nameof (render),
          render.ToString((IFormatProvider) CultureInfo.InvariantCulture)
        }
      };
      if (StreamManager.mDictCameraDetails.ContainsKey(StreamManager.mSelectedCamera))
        data.Add("camera", StreamManager.mDictCameraDetails[StreamManager.mSelectedCamera]);
      else
        data["camera"] = string.Empty;
      StreamManager.mDictLastCameraPosition = data;
      this.SendObsRequest("setcameraposition", data, "WebcamConfigured", (string) null, 0, false);
    }

    internal void UpdateCameraPosition(string camName)
    {
      this.DisableWebcam();
      if (!string.IsNullOrEmpty(camName))
        StreamManager.mSelectedCamera = camName;
      Dictionary<string, string> data = new Dictionary<string, string>()
      {
        ["source"] = "BlueStacksWebcam",
        ["width"] = "100",
        ["height"] = "100",
        ["x"] = "0",
        ["y"] = "0",
        ["render"] = "1"
      };
      data["camera"] = !StreamManager.mDictCameraDetails.ContainsKey(StreamManager.mSelectedCamera) ? string.Empty : StreamManager.mDictCameraDetails[StreamManager.mSelectedCamera];
      this.SendObsRequest("setcameraposition", data, "WebcamConfigured", (string) null, 0, false);
    }

    internal void ChangeCamera()
    {
      this.DisableWebcam();
      if (StreamManager.mIsWebcamDisabled)
        return;
      if (StreamManager.mDictLastCameraPosition.Count == 0)
      {
        StreamManager.mDictLastCameraPosition["source"] = "BlueStacksWebcam";
        StreamManager.mDictLastCameraPosition["width"] = "17";
        StreamManager.mDictLastCameraPosition["height"] = "23";
        StreamManager.mDictLastCameraPosition["x"] = "0";
        StreamManager.mDictLastCameraPosition["y"] = "77";
        StreamManager.mDictLastCameraPosition["render"] = "1";
      }
      StreamManager.mDictLastCameraPosition["camera"] = !StreamManager.mDictCameraDetails.ContainsKey(StreamManager.mSelectedCamera) ? string.Empty : StreamManager.mDictCameraDetails[StreamManager.mSelectedCamera];
      this.SendObsRequest("setcameraposition", StreamManager.mDictLastCameraPosition, "WebcamConfigured", (string) null, 0, false);
    }

    public void SetClrBrowserConfig(string width, string height, string url)
    {
      this.SendObsRequest("setclrbrowserconfig", new Dictionary<string, string>()
      {
        {
          nameof (width),
          width
        },
        {
          nameof (height),
          height
        },
        {
          nameof (url),
          url
        }
      }, (string) null, (string) null, 0);
    }

    public void ObsErrorStatus(string erroReason)
    {
      this.mIsStreaming = false;
      this.mFailureReason = "Error starting stream : " + erroReason;
      this.SendStreamStatus(false, false);
    }

    public void ReportObsError(string errorReason)
    {
      try
      {
        Logger.Info("error reason in obs :" + errorReason);
        string eventType = "stream_interrupted_error";
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        if (string.Equals(errorReason, "ConnectionSuccessfull", StringComparison.InvariantCultureIgnoreCase))
        {
          if (!this.mIsStreamStarted)
          {
            this.mIsStreamStarted = true;
            eventType = "obs_connected";
          }
          else
            eventType = "stream_resumed";
        }
        else if (!this.mIsStreamStarted)
          eventType = "went_live_error";
        if (string.Equals(errorReason, "OBSAlreadyRunning", StringComparison.InvariantCultureIgnoreCase))
        {
          string str = "obs_already_running";
          this.ReportStreamStatsToCloud(str, errorReason);
          dictionary.Add("reason", str);
          StreamManager.ReportObsErrorHandler(str);
        }
        else if (string.Equals(errorReason, "capture_error", StringComparison.InvariantCultureIgnoreCase))
        {
          string str = "capture_error";
          this.ReportStreamStatsToCloud(str, errorReason);
          StreamManager.sStopInitOBSQueue = true;
          dictionary.Add("reason", str);
          StreamManager.ReportObsErrorHandler(str);
        }
        else if (string.Equals(errorReason, "opengl_capture_error", StringComparison.InvariantCultureIgnoreCase))
        {
          string str = "opengl_capture_error";
          this.ReportStreamStatsToCloud(str, errorReason);
          StreamManager.sStopInitOBSQueue = true;
          dictionary.Add("reason", str);
          StreamManager.ReportObsErrorHandler(str);
        }
        else if (string.Equals(errorReason, "AccessDenied", StringComparison.InvariantCultureIgnoreCase) || string.Equals(errorReason, "ConnectServerError", StringComparison.InvariantCultureIgnoreCase) || string.Equals(errorReason, "obs_error", StringComparison.InvariantCultureIgnoreCase))
        {
          errorReason = "Error starting stream : " + errorReason;
          this.ReportStreamStatsToCloud(eventType, errorReason);
          dictionary.Add("reason", "obs_error");
          StreamManager.ReportObsErrorHandler("obs_error");
        }
        else if (string.Equals(errorReason, "ConnectionSuccessfull", StringComparison.InvariantCultureIgnoreCase))
        {
          this.ReportStreamStatsToCloud(eventType, errorReason);
        }
        else
        {
          errorReason = "Error starting stream : " + errorReason;
          this.ReportStreamStatsToCloud(eventType, errorReason);
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to report obs error.. Err : " + ex.ToString());
      }
    }

    public void RestartOBSInWindowCaptureMode()
    {
      try
      {
        this.ShutDownForcefully();
        this.CopySceneConfigFile(this.mWindow, true);
        Logger.Info("restarting obs in window capture mode");
        if (File.Exists(this.mLastVideoFilePath))
          File.Delete(this.mLastVideoFilePath);
        this.mWindow.mCommonHandler.RecordVideoOfApp();
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in restart obs in window capture mode: {0}", (object) ex.ToString());
      }
    }

    private static void ReportObsErrorHandler(string reason)
    {
      Logger.Error("Obs reported an error. " + reason);
      if (string.Equals(reason, "opengl_capture_error", StringComparison.OrdinalIgnoreCase))
      {
        RegistryManager.Instance.IsGameCaptureSupportedInMachine = false;
        StreamManager.Instance.RestartOBSInWindowCaptureMode();
      }
      else
        StreamManager.Instance?.mWindow.Dispatcher.Invoke((Delegate) (() => StreamManager.Instance.mWindow.mCommonHandler.ShowErrorRecordingVideoPopup()));
    }

    private void ReportStreamStatsToCloud(string eventType, string reason)
    {
      Logger.Info("StreamStats eventType: {0}, reason: {1}", (object) eventType, (object) reason);
      Dictionary<string, string> dictionary = new Dictionary<string, string>()
      {
        {
          "event_type",
          eventType
        },
        {
          "error_code",
          reason
        },
        {
          "guid",
          RegistryManager.Instance.UserGuid
        },
        {
          "streaming_platform",
          this.mNetwork
        },
        {
          "session_id",
          BlueStacks.BlueStacksUI.Stats.GetSessionId()
        },
        {
          "prod_ver",
          "4.250.0.1070"
        },
        {
          "created_at",
          DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss", (IFormatProvider) CultureInfo.CurrentCulture)
        }
      };
    }

    internal void GetParametersFromOBS()
    {
      this.SendObsRequest("getmicvolume", (Dictionary<string, string>) null, "SetMicVolumeLocal", (string) null, 0, false);
      this.SendObsRequest("getsystemvolume", (Dictionary<string, string>) null, "SetSystemVolumeLocal", (string) null, 0, false);
    }

    private void SetMicLocal(string response)
    {
      try
      {
        List<string> list1 = ((IEnumerable<string>) response.Split(new string[1]
        {
          "@@"
        }, StringSplitOptions.RemoveEmptyEntries)[0].Split(new char[1]
        {
          ','
        }, StringSplitOptions.RemoveEmptyEntries)).ToList<string>();
        List<string> list2 = ((IEnumerable<string>) response.Split(new string[1]
        {
          "@@"
        }, StringSplitOptions.RemoveEmptyEntries)[1].Split(new char[1]
        {
          ','
        }, StringSplitOptions.RemoveEmptyEntries)).ToList<string>();
        StreamManager.mSelectedMic = response.Split(new string[1]
        {
          "@@"
        }, StringSplitOptions.RemoveEmptyEntries)[2];
        StreamManager.mDictMicDetails.Clear();
        StreamManager.mSelectedMic = Regex.Replace(StreamManager.mSelectedMic, "[^\\u0000-\\u007F]+", string.Empty);
        if (list2.Count == 0)
        {
          StreamManager.mSelectedMic = string.Empty;
          StreamManager.mIsMicDisabled = true;
        }
        else
        {
          for (int index = 0; index < list2.Count; ++index)
          {
            if (!string.Equals(list2[index], "Default", StringComparison.InvariantCulture) && !string.Equals(list2[index], "Disable", StringComparison.InvariantCulture))
              StreamManager.mDictMicDetails.Add(Regex.Replace(list1[index], "[^\\u0000-\\u007F]+", string.Empty), list2[index]);
          }
          if (StreamManager.mDictMicDetails.Count == 0)
          {
            StreamManager.mSelectedMic = string.Empty;
            StreamManager.mIsMicDisabled = true;
          }
          else if (!StreamManager.mDictMicDetails.ContainsKey(StreamManager.mSelectedMic))
            StreamManager.mSelectedMic = StreamManager.mDictMicDetails.Keys.ToList<string>()[0];
        }
        EventHandler<CustomVolumeEventArgs> eventGetMicDetails = this.EventGetMicDetails;
        if (eventGetMicDetails == null)
          return;
        eventGetMicDetails((object) this, new CustomVolumeEventArgs(StreamManager.mDictMicDetails, StreamManager.mSelectedMic));
      }
      catch (Exception ex)
      {
        Logger.Error("Error in SetMicLocal. response: " + response);
        Logger.Error(ex.ToString());
      }
    }

    private void SetMicVolumeLocal(string volumeResponse)
    {
      try
      {
        StreamManager.mMicVolume = JObject.Parse(volumeResponse)["volume"].ToObject<int>();
        EventHandler<CustomVolumeEventArgs> eventGetMicVolume = this.EventGetMicVolume;
        if (eventGetMicVolume == null)
          return;
        eventGetMicVolume((object) this, new CustomVolumeEventArgs(StreamManager.mMicVolume));
      }
      catch (Exception ex)
      {
        Logger.Error("Error in SetMicVolumeLocal. response: " + volumeResponse);
        Logger.Error(ex.ToString());
      }
    }

    private void SetCameraLocal(string cameraResponse)
    {
      try
      {
        List<string> list1 = ((IEnumerable<string>) cameraResponse.Split(new string[1]
        {
          "@@"
        }, StringSplitOptions.RemoveEmptyEntries)[0].Split(new char[1]
        {
          ','
        }, StringSplitOptions.RemoveEmptyEntries)).ToList<string>();
        List<string> list2 = ((IEnumerable<string>) cameraResponse.Split(new string[1]
        {
          "@@"
        }, StringSplitOptions.RemoveEmptyEntries)[1].Split(new char[1]
        {
          ','
        }, StringSplitOptions.RemoveEmptyEntries)).ToList<string>();
        StreamManager.mDictCameraDetails.Clear();
        if (list2.Count == 0)
        {
          StreamManager.mSelectedCamera = string.Empty;
          StreamManager.mIsWebcamDisabled = true;
        }
        else
        {
          for (int index = 0; index < list2.Count; ++index)
            StreamManager.mDictCameraDetails.Add(Regex.Replace(list1[index], "[^\\u0000-\\u007F]+", string.Empty).Trim(), list2[index]);
          if (!StreamManager.mDictCameraDetails.ContainsKey(StreamManager.mSelectedCamera))
            StreamManager.mSelectedCamera = Regex.Replace(cameraResponse.Split(new string[1]
            {
              "@@"
            }, StringSplitOptions.RemoveEmptyEntries)[2], "[^\\u0000-\\u007F]+", string.Empty);
        }
        EventHandler<CustomVolumeEventArgs> getCameraDetails = this.EventGetCameraDetails;
        if (getCameraDetails == null)
          return;
        getCameraDetails((object) this, new CustomVolumeEventArgs(StreamManager.mDictCameraDetails, StreamManager.mSelectedCamera));
      }
      catch (Exception ex)
      {
        Logger.Error("Error in SetCameraLocal. response: " + cameraResponse);
        Logger.Error(ex.ToString());
      }
    }

    private void SetSystemVolumeLocal(string volumeResponse)
    {
      try
      {
        StreamManager.mSystemVolume = JObject.Parse(volumeResponse)["volume"].ToObject<int>();
        EventHandler<CustomVolumeEventArgs> eventGetSystemVolume = this.EventGetSystemVolume;
        if (eventGetSystemVolume == null)
          return;
        eventGetSystemVolume((object) this, new CustomVolumeEventArgs(StreamManager.mSystemVolume));
      }
      catch (Exception ex)
      {
        Logger.Error("Error in SetSystemVolumeLocal. response: " + volumeResponse);
        Logger.Error(ex.ToString());
      }
    }

    public static void EnableWebcam(string _1, string _2, string _3)
    {
    }

    public void DisableSource(string source)
    {
      this.SendObsRequest("setrender", new Dictionary<string, string>()
      {
        {
          nameof (source),
          source
        },
        {
          "render",
          "0"
        }
      }, (string) null, (string) null, 0, false);
    }

    public void EnableSource(string source)
    {
      this.SendObsRequest("setrender", new Dictionary<string, string>()
      {
        {
          nameof (source),
          source
        },
        {
          "render",
          "1"
        }
      }, (string) null, (string) null, 0, false);
    }

    public void EnableWebcamInternal(string widthStr, string heightStr, string position)
    {
      int int32_1 = Convert.ToInt32(widthStr, (IFormatProvider) CultureInfo.InvariantCulture);
      int int32_2 = Convert.ToInt32(heightStr, (IFormatProvider) CultureInfo.InvariantCulture);
      if (this.mStreamHeight == 0 || this.mStreamWidth == 0)
        return;
      int width = int32_1 * 100 / this.mStreamWidth;
      int height = int32_2 * 100 / this.mStreamHeight;
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      int x = 0;
      int y = 0;
      if (string.Equals(position, "2", StringComparison.InvariantCultureIgnoreCase))
        x = 100 - width;
      else if (string.Equals(position, "3", StringComparison.InvariantCultureIgnoreCase))
        y = 100 - height;
      else if (string.Equals(position, "4", StringComparison.InvariantCultureIgnoreCase))
      {
        x = 100 - width;
        y = 100 - width;
      }
      this.SetCameraPosition(x, y, width, height, 1);
    }

    public void DisableWebcamV2(string _)
    {
    }

    public void DisableWebcamAndClearDictionary()
    {
      StreamManager.mDictLastCameraPosition.Clear();
      StreamManager.mIsWebcamDisabled = true;
      this.DisableWebcam();
    }

    internal void DisableWebcam()
    {
      this.DisableSource("BlueStacksWebcam");
    }

    private void WebcamConfigured(string response)
    {
      try
      {
        JObject jobject = JObject.Parse(response);
        if (!jobject["success"].ToObject<bool>())
          return;
        StreamManager.CamStatus = jobject["webcam"].ToObject<bool>().ToString((IFormatProvider) CultureInfo.InvariantCulture);
        if (Convert.ToBoolean(StreamManager.CamStatus, (IFormatProvider) CultureInfo.InvariantCulture))
          RegistryManager.Instance.CamStatus = 1;
        else
          RegistryManager.Instance.CamStatus = 0;
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in Setting WebCamRegistry. response: {0} err : {1}", (object) response, (object) ex.ToString());
      }
    }

    public void ResetFlvStream()
    {
      this.SendObsRequest("resetflvstream", (Dictionary<string, string>) null, (string) null, (string) null, 0);
    }

    public void SetSquareConfig(int startX, int startY, int width, int height)
    {
      Dictionary<string, string> data = new Dictionary<string, string>();
      Logger.Info("Window size: ({0}, {1})", (object) width, (object) height);
      this.widthDiff = 0;
      int height1;
      MiscUtils.GetStreamWidthAndHeight(width, height, out int _, out height1);
      int widthFromHeight = (int) this.GetWidthFromHeight((double) height1);
      Logger.Info("Stream size: ({0}, {1})", (object) widthFromHeight, (object) height1);
      width = widthFromHeight;
      height = height1;
      height = width;
      string str;
      int num1;
      if (height1 <= 720)
      {
        str = "main";
        num1 = 2500;
      }
      else
      {
        str = "high";
        num1 = 3500;
      }
      float num2 = 1f;
      Logger.Info("x : " + startX.ToString());
      Logger.Info("y : " + startY.ToString());
      Logger.Info("width : " + width.ToString());
      Logger.Info("height : " + height.ToString());
      data.Clear();
      data.Add(nameof (startX), startX.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      data.Add(nameof (startY), startY.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      data.Add(nameof (width), width.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      data.Add(nameof (height), height.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      data.Add("x264Profile", str);
      data.Add("maxBitrate", num1.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      data.Add("downscale", num2.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      this.mStreamWidth = width;
      this.mStreamHeight = height;
      this.SendObsRequest("setconfig", data, (string) null, (string) null, 0, false);
    }

    public void SetConfig(int startX, int startY, int width, int height)
    {
      if (this.mSquareTheme)
        this.SetSquareConfig(startX, startY, width, height);
      else
        this.SetDefaultConfig(startX, startY, width, height);
    }

    public void RestartRecord()
    {
      this.StopRecord(true);
      this.StartRecord();
    }

    public void SetDefaultConfig(int startX, int startY, int width, int height)
    {
      Dictionary<string, string> data = new Dictionary<string, string>();
      Logger.Info("Window size: ({0}, {1})", (object) width, (object) height);
      this.widthDiff = 0;
      int height1;
      MiscUtils.GetStreamWidthAndHeight(width, height, out int _, out height1);
      int widthFromHeight = (int) this.GetWidthFromHeight((double) height1);
      width = widthFromHeight;
      height = height1;
      int num1 = width * 9 / 16;
      Logger.Info("Stream size: ({0}, {1})", (object) widthFromHeight, (object) height1);
      string str;
      int num2;
      switch (height1)
      {
        case 540:
          str = "main";
          num2 = 1200;
          break;
        case 720:
          str = "main";
          num2 = 2500;
          break;
        default:
          str = "high";
          num2 = 3500;
          break;
      }
      float num3 = (float) height / (float) height1;
      Logger.Info("x : " + startX.ToString());
      Logger.Info("y : " + startY.ToString());
      Logger.Info("width : " + width.ToString());
      Logger.Info("height : " + height.ToString());
      data.Clear();
      data.Add(nameof (startX), startX.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      data.Add(nameof (startY), startY.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      data.Add(nameof (width), width.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      data.Add(nameof (height), height.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      data.Add("x264Profile", str);
      data.Add("maxBitrate", num2.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      data.Add("downscale", num3.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      this.mStreamWidth = width;
      this.mStreamHeight = height;
      this.SendObsRequest("setconfig", data, (string) null, (string) null, 0, false);
    }

    public void StartStream(
      string key,
      string location,
      string callbackStreamStatus,
      string callbackAppInfo)
    {
      string service = "1";
      this.mCallbackStreamStatus = callbackStreamStatus;
      if (this.mCallbackAppInfo == null)
        this.mCallbackAppInfo = callbackAppInfo;
      this.SetStreamSettings(service, key, location);
      this.SendObsRequest("startstream", (Dictionary<string, string>) null, "StreamStarted", (string) null, 0);
    }

    public void StartStream(string jsonString, string callbackStreamStatus, string callbackAppInfo)
    {
      Logger.Info(jsonString);
      JObject jobject = JObject.Parse(jsonString);
      string playPath = jobject["key"].ToString();
      string service = jobject["service"].ToString();
      string url = jobject["streamUrl"].ToString();
      jobject["server"].ToString();
      this.mCallbackStreamStatus = callbackStreamStatus;
      this.mCallbackAppInfo = callbackAppInfo;
      this.SetStreamSettings(service, playPath, url);
      this.SendObsRequest("startstream", (Dictionary<string, string>) null, "StreamStarted", (string) null, 0);
    }

    public void StopStream()
    {
      this.SendObsRequest("stopstream", (Dictionary<string, string>) null, "StreamStopped", (string) null, 0);
    }

    private void SendStatus(string path, Dictionary<string, string> data)
    {
      try
      {
        if (string.Equals(path, "streamstarted", StringComparison.InvariantCulture))
          BTVManager.Instance.StreamStarted();
        else if (string.Equals(path, "streamstopped", StringComparison.InvariantCulture))
          BTVManager.Instance.StreamStopped();
        else if (string.Equals(path, "recordstarted", StringComparison.InvariantCulture))
          BTVManager.Instance.RecordStarted();
        else if (string.Equals(path, "recordstopped", StringComparison.InvariantCulture))
        {
          BTVManager.Instance.RecordStopped();
          CommonHandlers.sIsRecordingVideo = false;
          if (BlueStacksUIUtils.DictWindows.ContainsKey(CommonHandlers.sRecordingInstance))
            BlueStacksUIUtils.DictWindows[CommonHandlers.sRecordingInstance].mCommonHandler.RecordingStopped();
          CommonHandlers.sRecordingInstance = "";
          this.ShutDownForcefully();
        }
        else if (string.Equals(path, "streamstatus", StringComparison.InvariantCulture))
          BTVManager.Instance.sStreaming = string.Compare(data["isstreaming"], "true", StringComparison.OrdinalIgnoreCase) == 0;
        else if (string.Equals(path, "replaybuffersaved", StringComparison.InvariantCulture))
          BTVManager.ReplayBufferSaved();
        else if (string.Equals(path, "RecordStartedVideo", StringComparison.InvariantCulture) && BlueStacksUIUtils.DictWindows.ContainsKey(CommonHandlers.sRecordingInstance))
          BlueStacksUIUtils.DictWindows[CommonHandlers.sRecordingInstance].mCommonHandler.RecordingStarted();
        Logger.Info("Successfully sent status for {0}", (object) path);
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to send post request for {0}... Err : {1}", (object) path, (object) ex.ToString());
      }
    }

    public void StartRecordForVideo()
    {
      if (this.mIsObsRunning)
      {
        int startX;
        int startY;
        int width;
        int height;
        StreamManager.SetStreamDimension(out startX, out startY, out width, out height);
        this.SetConfig(startX, startY, width, height);
        this.SetSceneConfiguration(this.mLayoutTheme);
        this.ResetCLRBrowser(true);
      }
      this.SendObsRequest("startrecord", (Dictionary<string, string>) null, "RecordStartedVideo", (string) null, 0);
    }

    public void StartRecord()
    {
      this.StartRecord(this.mNetwork, this.mEnableFilter, this.mSquareTheme, this.mLayoutTheme, this.mCallbackAppInfo);
    }

    public void StartRecordInit(
      string network,
      bool enableFilter,
      bool squareTheme,
      string layoutTheme,
      string callbackAppInfo)
    {
      this.mNetwork = network;
      this.mEnableFilter = enableFilter;
      this.mSquareTheme = squareTheme;
      this.mLayoutTheme = layoutTheme;
      this.mCallbackAppInfo = callbackAppInfo;
    }

    public void StartRecord(
      string network,
      bool enableFilter,
      bool squareTheme,
      string layoutTheme,
      string callbackAppInfo)
    {
      lock (this.stoppingOBSLock)
      {
        this.mEnableFilter = enableFilter;
        this.mSquareTheme = squareTheme;
        this.mCallbackAppInfo = callbackAppInfo;
        StreamManager.SendNetworkName(network);
        if (layoutTheme != null)
        {
          this.mLayoutTheme = Utils.GetString(RegistryManager.Instance.LayoutTheme, layoutTheme);
          this.mLastCameraLayoutTheme = RegistryManager.Instance.LastCameraLayoutTheme;
          this.mAppViewLayout = RegistryManager.Instance.AppViewLayout == 1;
          if (string.IsNullOrEmpty(this.mLastCameraLayoutTheme))
            this.mLastCameraLayoutTheme = layoutTheme;
        }
        else
          this.mLayoutTheme = layoutTheme;
        this.mNetwork = network;
        if (this.mIsObsRunning)
        {
          int startX;
          int startY;
          int width;
          int height;
          StreamManager.SetStreamDimension(out startX, out startY, out width, out height);
          this.SetConfig(startX, startY, width, height);
          this.SetSceneConfiguration(this.mLayoutTheme);
        }
        this.EnableVideoRecording(false);
        this.SendObsRequest("startrecord", (Dictionary<string, string>) null, "RecordStarted", (string) null, 0);
      }
    }

    public void StopRecord()
    {
      this.StopRecord(false);
    }

    public void StopRecord(bool immediate)
    {
      this.SendObsRequest("stoprecord", new Dictionary<string, string>()
      {
        {
          nameof (immediate),
          immediate ? "1" : "0"
        }
      }, "RecordStopped", (string) null, 0);
    }

    public void SendAppInfo(string type, string name, string data)
    {
      if (this.mCallbackAppInfo == null)
        return;
      this.mBrowser.CallJs(this.mCallbackAppInfo, new object[1]
      {
        (object) new JObject()
        {
          {
            nameof (type),
            (JToken) type
          },
          {
            nameof (name),
            (JToken) name
          },
          {
            nameof (data),
            (JToken) data
          }
        }.ToString(Formatting.None)
      });
    }

    public static string GetStreamConfig()
    {
      string streamName = RegistryManager.Instance.StreamName;
      string serverLocation = RegistryManager.Instance.ServerLocation;
      JObject jobject = new JObject()
      {
        {
          "streamName",
          (JToken) streamName
        },
        {
          "camStatus",
          (JToken) Convert.ToBoolean(StreamManager.CamStatus, (IFormatProvider) CultureInfo.InvariantCulture)
        },
        {
          "micVolume",
          (JToken) StreamManager.mMicVolume
        },
        {
          "systemVolume",
          (JToken) StreamManager.mSystemVolume
        },
        {
          "serverLocation",
          (JToken) serverLocation
        }
      };
      Logger.Info("GetStreamConfig: " + jobject.ToString(Formatting.None));
      return jobject.ToString();
    }

    private void StreamStarted(string _)
    {
      this.SendStatus("streamstarted", (Dictionary<string, string>) null);
      this.mIsStreaming = true;
    }

    private void StreamStopped(string _)
    {
      this.SendStatus("streamstopped", (Dictionary<string, string>) null);
      this.mIsStreaming = false;
      this.mIsStreamStarted = false;
      this.SendObsRequest("close", (Dictionary<string, string>) null, (string) null, (string) null, 0);
      new Thread((ThreadStart) (() => this.KillOBS()))
      {
        IsBackground = true
      }.Start();
    }

    public static void killOBSForcelly()
    {
      Utils.KillCurrentOemProcessByName("HD-OBS", (string) null);
    }

    public void KillOBS()
    {
      if (this.mStoppingOBS)
        return;
      lock (this.stoppingOBSLock)
      {
        this.mStoppingOBS = true;
        try
        {
          int num1 = 0;
          int num2 = 20;
          while (num1 < num2 && Process.GetProcessesByName("HD-OBS").Length != 0)
          {
            ++num1;
            if (num1 < num2)
            {
              Logger.Info("Waiting for HD-OBS to quit gracefully, retry: {0}", (object) num1);
              Thread.Sleep(200);
            }
          }
          if (num1 >= num2)
            Utils.KillCurrentOemProcessByName("HD-OBS", (string) null);
          StreamManager.StartOBS();
        }
        catch (Exception ex)
        {
          Logger.Info("Failed to kill HD-OBS.exe...Err : " + ex.ToString());
        }
        this.mStoppingOBS = false;
      }
    }

    private void RecordStarted(string _)
    {
      this.SendStatus("recordstarted", (Dictionary<string, string>) null);
    }

    private void RecordStopped(string _)
    {
      this.SendStatus("recordstopped", (Dictionary<string, string>) null);
    }

    private void RecordStartedVideo(string _)
    {
      this.SendStatus(nameof (RecordStartedVideo), (Dictionary<string, string>) null);
    }

    public void StartReplayBuffer()
    {
      this.SendObsRequest("startreplaybuffer", (Dictionary<string, string>) null, (string) null, (string) null, 0);
    }

    public void StopReplayBuffer()
    {
      this.SendObsRequest("stopreplaybuffer", (Dictionary<string, string>) null, (string) null, (string) null, 2000);
    }

    private void SetStreamSettings(string service, string playPath, string url)
    {
      this.SendObsRequest("setstreamsettings", new Dictionary<string, string>()
      {
        {
          nameof (service),
          service
        },
        {
          nameof (playPath),
          playPath
        },
        {
          nameof (url),
          url
        }
      }, (string) null, (string) null, 0);
    }

    public void SetSystemVolume(string level)
    {
      StreamManager.mSystemVolume = Convert.ToInt32(level, (IFormatProvider) CultureInfo.InvariantCulture);
      this.SendObsRequest("setsystemvolume", new Dictionary<string, string>()
      {
        {
          "volume",
          level
        }
      }, (string) null, (string) null, 0);
    }

    internal void SetMic(string micName)
    {
      micName = StreamManager.mDictMicDetails[micName];
      this.SendObsRequest("setmic", new Dictionary<string, string>()
      {
        {
          "micId",
          micName
        }
      }, (string) null, (string) null, 0);
    }

    public void SetMicVolume(string level)
    {
      StreamManager.mMicVolume = Convert.ToInt32(level, (IFormatProvider) CultureInfo.InvariantCulture);
      if (StreamManager.mMicVolume > 0)
        StreamManager.mIsMicDisabled = false;
      this.SendObsRequest("setmicvolume", new Dictionary<string, string>()
      {
        {
          "volume",
          level
        }
      }, (string) null, (string) null, 0);
    }

    private void StartPollingOBS()
    {
      while (this.mIsObsRunning)
      {
        try
        {
          JObject jobject = JObject.Parse(Regex.Replace(this.SendObsRequestInternal("getstatus", (Dictionary<string, string>) null), "\\r\\n?|\\n", ""));
          bool streaming = jobject["streaming"].ToObject<bool>();
          bool reconnecting = jobject["reconnecting"].ToObject<bool>();
          if (!streaming)
          {
            try
            {
              this.mFailureReason = jobject["reason"].ToString();
            }
            catch
            {
            }
          }
          if (streaming != this.mIsStreaming)
            this.SendStreamStatus(streaming, reconnecting);
          this.mIsStreaming = streaming;
          this.mIsReconnecting = reconnecting;
          this.SendStatus("streamstatus", new Dictionary<string, string>()
          {
            {
              "isstreaming",
              this.mIsStreaming.ToString((IFormatProvider) CultureInfo.InvariantCulture)
            }
          });
        }
        catch (Exception ex)
        {
          Logger.Error("Exception in StartPollingOBS err : " + ex.ToString());
          if (!ProcessUtils.FindProcessByName("HD-OBS"))
          {
            this.mIsObsRunning = false;
            this.mIsStreaming = false;
            this.mIsReconnecting = false;
            this.mCLRBrowserRunning = false;
            this.mIsStreamStarted = false;
            if (!this.mStoppingOBS)
              this.UpdateFailureReason();
            this.SendStreamStatus(false, false);
            this.InitObs();
            this.mStoppingOBS = false;
            break;
          }
        }
        Thread.Sleep(5000);
      }
    }

    private void UpdateFailureReason()
    {
      if (!string.IsNullOrEmpty(this.mFailureReason))
        return;
      string s = "";
      string format = "yyyy-MM-dd-HHmm-ss";
      DateTime dateTime = DateTime.MinValue;
      foreach (string file in Directory.GetFiles(Path.Combine(RegistryStrings.BtvDir, "OBS\\Logs\\")))
      {
        s = Path.GetFileNameWithoutExtension(file);
        DateTime result;
        if (DateTime.TryParseExact(s, format, (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.None, out result) && dateTime < result)
          dateTime = result;
      }
      if (!dateTime.Equals(DateTime.MinValue))
        s = ((IEnumerable<string>) File.ReadAllLines(Path.Combine(RegistryStrings.BtvDir, "OBS\\Logs\\") + dateTime.ToString("yyyy-MM-dd-HHmm-ss", (IFormatProvider) CultureInfo.InvariantCulture) + ".log")).Last<string>();
      this.mFailureReason = "OBS crashed: " + s;
    }

    private static void SendNetworkName(string network)
    {
      try
      {
        BTVManager.sNetwork = network;
        RegistryManager.Instance.BtvNetwork = network;
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to send network name... Err : " + ex.ToString());
      }
    }

    private void SendStreamStatus(bool streaming, bool reconnecting)
    {
      Logger.Info("Sending stream status with data :: streaming : " + streaming.ToString() + ", reconnecting : " + reconnecting.ToString() + ", obsRunning : " + this.mIsObsRunning.ToString() + ", failureReason : " + this.mFailureReason);
      try
      {
        Dictionary<string, string> dictionary = new Dictionary<string, string>()
        {
          {
            "obs",
            this.mIsObsRunning.ToString((IFormatProvider) CultureInfo.InvariantCulture)
          },
          {
            nameof (streaming),
            streaming.ToString((IFormatProvider) CultureInfo.InvariantCulture)
          },
          {
            nameof (reconnecting),
            reconnecting.ToString((IFormatProvider) CultureInfo.InvariantCulture)
          },
          {
            "reason",
            this.mFailureReason.ToString((IFormatProvider) CultureInfo.InvariantCulture)
          }
        };
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to send stream status... Err : " + ex.ToString());
      }
    }

    public void ResizeStream(string width, string height)
    {
      if (StreamManager.mObsCommandQueue == null)
        return;
      this.SendObsRequest("windowresized", new Dictionary<string, string>()
      {
        {
          nameof (width),
          width
        },
        {
          nameof (height),
          height
        }
      }, (string) null, (string) null, 0);
    }

    public void ShowObs()
    {
      this.SendObsRequest("show", (Dictionary<string, string>) null, (string) null, (string) null, 0);
    }

    public void HideObs()
    {
      this.SendObsRequest("hide", (Dictionary<string, string>) null, (string) null, (string) null, 0);
    }

    public void MoveWebcam(string horizontal, string vertical)
    {
      this.SendObsRequest("movewebcam", new Dictionary<string, string>()
      {
        {
          nameof (horizontal),
          horizontal
        },
        {
          nameof (vertical),
          vertical
        }
      }, (string) null, (string) null, 0);
    }

    public void Shutdown()
    {
      if (this.mIsStreaming)
        this.StopStream();
      if (StreamManager.mObsCommandQueue == null)
        return;
      this.mIsObsRunning = false;
      this.mIsStreamStarted = false;
      this.SendObsRequest("close", (Dictionary<string, string>) null, "CloseSuccess", "CloseFailed", 0);
    }

    public static void CloseSuccess(string _)
    {
      StreamManager.Instance = (StreamManager) null;
    }

    public static void CloseFailed()
    {
      Utils.KillCurrentOemProcessByName("HD-OBS", (string) null);
      StreamManager.Instance = (StreamManager) null;
    }

    public static void StopOBS()
    {
      StreamManager.Instance.mStoppingOBS = true;
      StreamManager.sStopInitOBSQueue = true;
      StreamManager.Instance.Shutdown();
      int num;
      for (num = 0; Process.GetProcessesByName("HD-OBS").Length != 0 && num < 20; ++num)
        Thread.Sleep(500);
      if (num != 20)
        return;
      Logger.Info("Killing hd-obs as normal close failed");
      StreamManager.CloseFailed();
    }

    public void SaveReplayBuffer()
    {
      this.SendObsRequest("savereplaybuffer", (Dictionary<string, string>) null, (string) null, (string) null, 0);
    }

    public void ReplayBufferSaved()
    {
      this.SendStatus("replaybuffersaved", (Dictionary<string, string>) null);
    }

    public void SendObsRequest(
      string request,
      Dictionary<string, string> data,
      string responseCallback,
      string failureCallback,
      int pauseTime)
    {
      this.SendObsRequest(request, data, responseCallback, failureCallback, pauseTime, true);
    }

    public void SendObsRequest(
      string request,
      Dictionary<string, string> data,
      string responseCallback,
      string failureCallback,
      int pauseTime,
      bool waitForInit)
    {
      Logger.Info("got obs request: " + request);
      if (data != null && !data.ContainsKey("randomVal"))
        data.Add("randomVal", "0");
      new Thread((ThreadStart) (() =>
      {
        StreamManager.ObsCommand obsCommand = new StreamManager.ObsCommand(request, data, responseCallback, failureCallback, pauseTime);
        lock (this.mInitOBSLock)
        {
          if (!this.mIsInitCalled)
            this.InitObs();
        }
        if (StreamManager.mObsCommandQueue == null)
          StreamManager.mObsCommandQueue = new Queue<StreamManager.ObsCommand>();
        if (waitForInit)
        {
          lock (this.mInitOBSLock)
          {
            lock (this.mObsCommandQueueObject)
            {
              StreamManager.mObsCommandQueue.Enqueue(obsCommand);
              this.mObsCommandEventHandle.Set();
            }
          }
        }
        else
        {
          lock (this.mObsCommandQueueObject)
          {
            StreamManager.mObsCommandQueue.Enqueue(obsCommand);
            this.mObsCommandEventHandle.Set();
          }
        }
      }))
      {
        IsBackground = true
      }.Start();
    }

    private string SendObsRequestInternal(string request, Dictionary<string, string> data)
    {
      Logger.Info("waiting to send request: " + request);
      lock (this.mObsSendRequestObject)
      {
        string str = string.Empty;
        if (this.mIsObsRunning)
          str = BstHttpClient.Post(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1}/{2}", (object) StreamManager.ObsServerBaseURL, (object) StreamManager.ObsServerPort, (object) request), data, (Dictionary<string, string>) null, false, "Android", 0, 1, 0, false, "bgp");
        return str;
      }
    }

    private void ProcessObsCommandQueue()
    {
      while (this.mIsObsRunning)
      {
        this.mObsCommandEventHandle.WaitOne();
        while (StreamManager.mObsCommandQueue.Count != 0)
        {
          StreamManager.ObsCommand obsCommand;
          lock (this.mObsCommandQueueObject)
          {
            if (StreamManager.mObsCommandQueue.Count != 0)
              obsCommand = StreamManager.mObsCommandQueue.Dequeue();
            else
              break;
          }
          string empty = string.Empty;
          try
          {
            string str = this.SendObsRequestInternal(obsCommand.mRequest, obsCommand.mData);
            Logger.Info("Got response {0} for {1}", (object) str, (object) obsCommand.mRequest);
            if (obsCommand.mResponseCallback != null)
              this.GetType().GetMethod(obsCommand.mResponseCallback, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Invoke((object) this, new object[1]
              {
                (object) str
              });
          }
          catch (Exception ex1)
          {
            Logger.Error("Exception when sending " + obsCommand.mRequest);
            Logger.Error(ex1.ToString());
            try
            {
              if (obsCommand.mFailureCallback != null)
                this.GetType().GetMethod(obsCommand.mFailureCallback, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Invoke((object) this, new object[0]);
            }
            catch (Exception ex2)
            {
              Logger.Error("Error in failure call back for call {} and error {1}", (object) obsCommand.mFailureCallback, (object) ex2);
            }
          }
          Thread.Sleep(obsCommand.mPauseTime);
        }
      }
    }

    public void Init(string appHandle, string pid)
    {
      Logger.Info("App Handle : {0} and Process Id : {1}", (object) appHandle, (object) pid);
      if (!string.IsNullOrEmpty(this.mAppHandle) || !string.IsNullOrEmpty(this.mAppPid))
        return;
      this.mAppHandle = appHandle;
      this.mAppPid = pid;
    }

    private double GetWidthFromHeight(double height)
    {
      return (height - (double) this.heightDiff) * this.mAspectRatio.DoubleValue + (double) this.widthDiff;
    }

    public static void GetStreamConfig(out string handle, out string pid)
    {
      try
      {
        MainWindow activatedWindow = (MainWindow) null;
        if (BlueStacksUIUtils.DictWindows.Count > 0)
          activatedWindow = BlueStacksUIUtils.DictWindows.Values.First<MainWindow>();
        handle = activatedWindow.mFrontendHandler.mFrontendHandle.ToString();
        activatedWindow.Dispatcher.Invoke((Delegate) (() => activatedWindow.RestrictWindowResize(true)));
        Process currentProcess = Process.GetCurrentProcess();
        pid = currentProcess.Id.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to get window handle and process id... Err : " + ex.ToString());
        handle = pid = (string) null;
      }
    }

    public void ShutDownForcefully()
    {
      try
      {
        StreamManager.killOBSForcelly();
        this.mIsObsRunning = false;
        StreamManager.Instance.mStoppingOBS = true;
        StreamManager.mObsCommandQueue.Clear();
        StreamManager.Instance = (StreamManager) null;
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in shutdown obs : {0}", (object) ex);
      }
    }

    protected virtual void Dispose(bool disposing)
    {
      if (this.disposedValue)
        return;
      int num = disposing ? 1 : 0;
      this.mBrowser?.Dispose();
      this.mObsCommandEventHandle?.Close();
      this.disposedValue = true;
    }

    ~StreamManager()
    {
      this.Dispose(false);
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    private class ObsCommand
    {
      public string mRequest;
      public Dictionary<string, string> mData;
      public string mResponseCallback;
      public string mFailureCallback;
      public int mPauseTime;

      public ObsCommand(
        string request,
        Dictionary<string, string> data,
        string responseCallback,
        string failureCallback,
        int pauseTime)
      {
        this.mRequest = request;
        this.mData = data;
        this.mResponseCallback = responseCallback;
        this.mFailureCallback = failureCallback;
        this.mPauseTime = pauseTime;
      }
    }
  }
}
