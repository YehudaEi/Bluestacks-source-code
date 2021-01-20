// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.InputMapper
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using BlueStacks.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace BlueStacks.Player
{
  public class InputMapper
  {
    private static object syncRoot = new object();
    private static object syncRoot1 = new object();
    private static InputMapper sInstance = (InputMapper) null;
    private static uint[] sMapableKeyArray = new uint[42]
    {
      2U,
      3U,
      4U,
      5U,
      6U,
      7U,
      8U,
      9U,
      10U,
      11U,
      16U,
      17U,
      18U,
      19U,
      20U,
      21U,
      22U,
      23U,
      24U,
      25U,
      30U,
      31U,
      32U,
      33U,
      34U,
      35U,
      36U,
      37U,
      38U,
      44U,
      45U,
      46U,
      47U,
      48U,
      49U,
      50U,
      57U,
      28U,
      57416U,
      57424U,
      57419U,
      57421U
    };
    private static Dictionary<uint, int> sMapableKeySet = (Dictionary<uint, int>) null;
    internal static bool s_UserKeyMappingEnabled = true;
    internal static bool mIsVMWindowsActivated = false;
    internal static bool mIsGamepadConnected = false;
    internal static bool isSendSensorDeviceData = false;
    internal static bool IsMacroPlaying = false;
    private Monitor.TouchPoint[] touchPoints = new Monitor.TouchPoint[16];
    private Monitor.TouchPoint[] mTouchPoints = new Monitor.TouchPoint[16];
    private object mCursorLock = new object();
    private Point mLastMouseMoveLocation = new Point(0, 0);
    public const int CURSOR_SLOTS = 4;
    private const int GAMEPAD_AXIS_MAX = 1000;
    private const int CURSOR_MOVE_FACTOR = 10;
    private const string TEMPLATE = "TEMPLATE.cfg";
    private const string DEFAULT = "DEFAULT.cfg";
    private const int MAX_X = 32767;
    private const int MAX_Y = 32767;
    private const int MOUSE_SLOT = 13;
    public const int RIDEV_INPUTSINK = 256;
    public const int RIDEV_NOLEGACY = 48;
    public const int RIDEV_REMOVE = 1;
    public const int RID_INPUT = 268435459;
    public const int RIM_TYPEMOUSE = 0;
    private string mUserFolder;
    private Monitor mMonitor;
    private string mCurrentPackage;
    private SerialWorkQueue mSerialQueue;
    private Point[] mCursorDeltas;
    private InputMapper.TiltHandler mTiltHandler;
    private InputMapper.ClientActionHandler mClientActionHandler;
    private InputMapper.ClientGetGamepadButtonHandler mClientGetGamepadButtonHandler;
    private InputMapper.SetGamepadStatusHandler mSetGamepadStatusHandler;
    private InputMapper.GamepadBackButtonPressedHandler mGamepadBackButtonPressedHandler;
    private InputMapper.PlaybackCompleteHandler mPlaybackCompleteHandler;
    private InputMapper.GameControlStatusUpdate mGameControlStatusUpdateHandler;
    private InputMapper.RegisterRawInputMouseHandler mRegisterRawInputMouseHandler;
    private InputMapper.SetMouseCursorPos mSetMouseCursorPos;
    private InputMapper.ShowMouseCursor mShowMouseCursor;
    private static InputMapper.HandleGuestBootLogs mHandleGuestBootLogs;
    private InputMapper.ImapSendImageInfoHandler mImapSendImageInfoHandler;
    public float mSoftControlBarHeightLandscape;
    public float mSoftControlBarHeightPortrait;
    public bool mSoftControlEnabled;

    [DllImport("User32.dll")]
    private static extern uint GetRawInputData(
      IntPtr hRawInput,
      uint uiCommand,
      IntPtr pData,
      ref uint pcbSize,
      uint cbSizeHeader);

    [DllImport("user32.dll")]
    public static extern bool RegisterRawInputDevices(
      [MarshalAs(UnmanagedType.LPArray)] InputMapper.RAWINPUTDEVICE[] pRawInputDevices,
      int uiNumDevices,
      int cbSize);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SystemParametersInfo(
      int uiAction,
      int uiParam,
      uint pvParam,
      int fWinIni);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SystemParametersInfo(
      int uiAction,
      int uiParam,
      ref uint pvParam,
      int fWinIni);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)] string lpLibFileName);

    [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
    private static extern IntPtr GetProcAddress(IntPtr hModule, [MarshalAs(UnmanagedType.LPStr)] string lpProcName);

    [DllImport("kernel32.dll")]
    private static extern bool FreeLibrary(int hModule);

    [DllImport("HD-Imap-Native.dll", CharSet = CharSet.Unicode)]
    private static extern int ImapShake();

    [DllImport("HD-Imap-Native.dll", CharSet = CharSet.Unicode)]
    private static extern void ImapSetState(int state);

    [DllImport("HD-Imap-Native.dll", CharSet = CharSet.Unicode)]
    private static extern int ImapStartRecording();

    [DllImport("HD-Imap-Native.dll", CharSet = CharSet.Ansi)]
    private static extern int ImapValidateScriptCommands(
      StringBuilder commands,
      ref int errIndex,
      ref bool isValid);

    [DllImport("HD-Imap-Native.dll", CharSet = CharSet.Ansi)]
    private static extern int ImapGetInteractionStats(
      ref int numInteractions,
      ref int numMappedInteractions,
      ref int numTextInteractions,
      ref int nativeGamepadUsed,
      StringBuilder interactionHist);

    [DllImport("HD-Imap-Native.dll", CharSet = CharSet.Unicode)]
    private static extern int ImapStopRecording(StringBuilder events, ref int size);

    [DllImport("HD-Imap-Native.dll", CharSet = CharSet.Ansi)]
    private static extern void ImapPauseRecording();

    [DllImport("HD-Imap-Native.dll", CharSet = CharSet.Unicode)]
    private static extern int ImapGetParserVersion();

    [DllImport("HD-Imap-Native.dll", CharSet = CharSet.Unicode)]
    private static extern int ImapGetMinParserVersion();

    [DllImport("HD-Imap-Native.dll", CharSet = CharSet.Unicode)]
    private static extern int ImapLoadConfig(
      [MarshalAs(UnmanagedType.LPWStr)] string pkg,
      [MarshalAs(UnmanagedType.LPWStr)] string activity,
      ref bool SendSensorDataToGuest);

    [DllImport("HD-Imap-Native.dll", CharSet = CharSet.Unicode)]
    private static extern int ImapInitPlayback(
      [MarshalAs(UnmanagedType.LPWStr)] string filePath,
      InputMapper.PlaybackCompleteHandler mPlaybackCompleteHandler);

    [DllImport("HD-Imap-Native.dll", CharSet = CharSet.Unicode)]
    private static extern int ImapStopPlayback();

    [DllImport("HD-Imap-Native.dll", CharSet = CharSet.Unicode)]
    internal static extern int ImapRunMacroUnit([MarshalAs(UnmanagedType.LPWStr)] string macroName, double acceleration);

    [DllImport("HD-Imap-Native.dll", CharSet = CharSet.Unicode)]
    private static extern void ImapStartSync(bool stopAllThreads);

    [DllImport("HD-Imap-Native.dll", CharSet = CharSet.Unicode)]
    private static extern void ImapStopSync(bool stopAllThreads);

    [DllImport("HD-Imap-Native.dll", CharSet = CharSet.Unicode)]
    private static extern void ImapStartSyncConsumer([MarshalAs(UnmanagedType.LPWStr)] string sourceVmName);

    [DllImport("HD-Imap-Native.dll", CharSet = CharSet.Unicode)]
    private static extern void ImapStopSyncConsumer();

    [DllImport("HD-Imap-Native.dll", CharSet = CharSet.Unicode)]
    private static extern int ImapHandleConfigChange([MarshalAs(UnmanagedType.LPWStr)] string pkg, [MarshalAs(UnmanagedType.LPWStr)] string activity);

    [DllImport("HD-Imap-Native.dll", CharSet = CharSet.Unicode)]
    private static extern int ImapInit(
      int guestWidth,
      int guestHeight,
      [MarshalAs(UnmanagedType.LPWStr)] string configFolder,
      [MarshalAs(UnmanagedType.LPWStr)] string userConfigFolder,
      Logger.HdLoggerCallback cb,
      InputMapper.SetMouseCursorPos mSetMouseCursorPos,
      InputMapper.ShowMouseCursor mShowMouseCursor,
      IntPtr monitorSendTouchState,
      IntPtr monitorSendScanCode,
      IntPtr monitorSendGamepadState,
      IntPtr monitorSendImeMsg,
      InputMapper.TiltHandler mTiltHandler,
      InputMapper.RegisterRawInputMouseHandler mRegisterRawInputMouseHandler,
      InputMapper.ClientActionHandler mClientActionHandler,
      InputMapper.ClientGetGamepadButtonHandler mClientGetGamepadButtonHandler,
      [MarshalAs(UnmanagedType.LPWStr)] string vmName,
      InputMapper.GameControlStatusUpdate mGameControlStatusUpdateHandler);

    [DllImport("HD-Imap-Native.dll", CharSet = CharSet.Unicode)]
    private static extern int ImapRegisterGamepadConnectedCb(
      InputMapper.SetGamepadStatusHandler mSetGamepadStatusHandler,
      InputMapper.GamepadBackButtonPressedHandler mGamepadBackButtonPressedHandler);

    [DllImport("HD-Imap-Native.dll", CharSet = CharSet.Unicode)]
    private static extern int ImapHandleTouch([In] Monitor.TouchPoint[] tps, int nTouchPoints);

    [DllImport("HD-PgaSocketHgcm.dll", CharSet = CharSet.Ansi)]
    private static extern void BstGetGuestBootLogs(InputMapper.HandleGuestBootLogs guestBootLogsCb);

    [DllImport("HD-Plus-Frontend-Native.dll")]
    private static extern bool MonitorSendTouchState(
      InputMapper.BST_INPUT_TOUCH_POINT[] points,
      int count);

    [DllImport("HD-Imap-Native.dll", CharSet = CharSet.Unicode)]
    public static extern int ImapHandleIme([MarshalAs(UnmanagedType.LPWStr)] string buf);

    [DllImport("HD-Imap-Native.dll", CharSet = CharSet.Unicode)]
    public static extern int ImapHandleClient([MarshalAs(UnmanagedType.LPWStr)] string buf, bool needCb);

    [DllImport("HD-Imap-Native.dll")]
    private static extern int ImapHandleKey(uint keyCode, int down);

    [DllImport("HD-Imap-Native.dll")]
    private static extern int ImapHandleRawInput(IntPtr buffer);

    [DllImport("HD-Imap-Native.dll")]
    private static extern int ImapHandleMouse(
      int evt,
      int x,
      int y,
      bool leftButtonPressed,
      bool rightButtonPressed,
      bool middleButtonPressed,
      bool xButton1Pressed,
      bool xButton2Pressed,
      int delta);

    [DllImport("User32.dll")]
    private static extern uint MapVirtualKey(uint code, uint mapType);

    [DllImport("HD-Imap-Native.dll", CharSet = CharSet.Unicode)]
    private static extern int ImapEnableGamepadButtonCb(bool enable);

    [DllImport("HD-Imap-Native.dll", CharSet = CharSet.Unicode)]
    private static extern void ImapGamepadEnable(bool enable);

    [DllImport("HD-Imap-Native.dll", CharSet = CharSet.Unicode)]
    private static extern void ImapSetGamepadMode(int mode);

    [DllImport("HD-Imap-Native.dll", CharSet = CharSet.Unicode)]
    private static extern int ImapSwitchBackFromHomeCallback([MarshalAs(UnmanagedType.LPWStr)] string pkg);

    [DllImport("HD-Imap-Native.dll", CharSet = CharSet.Unicode)]
    private static extern void ImapEnableScrollOnEdge(bool enable);

    public static InputMapper Instance
    {
      get
      {
        if (InputMapper.sInstance == null)
        {
          lock (InputMapper.syncRoot1)
          {
            if (InputMapper.sInstance == null)
              InputMapper.sInstance = new InputMapper();
          }
        }
        return InputMapper.sInstance;
      }
    }

    public static bool IsMapableKey(uint scanCode)
    {
      lock (InputMapper.syncRoot)
      {
        if (InputMapper.sMapableKeySet == null)
        {
          InputMapper.sMapableKeySet = new Dictionary<uint, int>();
          for (int index = 0; index < InputMapper.sMapableKeyArray.Length; ++index)
            InputMapper.sMapableKeySet.Add(InputMapper.sMapableKeyArray[index], 1);
        }
        return InputMapper.sMapableKeySet.ContainsKey(scanCode);
      }
    }

    public static string GetKeyMappingParserVersion()
    {
      int parserVersion = InputMapper.ImapGetParserVersion();
      Logger.Info("the parserVersion returned is {0}", (object) parserVersion);
      return parserVersion.ToString();
    }

    public static string GetMinKeyMappingParserVersion()
    {
      int minParserVersion = InputMapper.ImapGetMinParserVersion();
      Logger.Info("the minParserVersion returned is {0}", (object) minParserVersion);
      return minParserVersion.ToString();
    }

    public InputMapper()
    {
      this.mSerialQueue = new SerialWorkQueue();
      this.mSerialQueue.Start();
    }

    internal static void RegisterGuestBootLogsHandler()
    {
      try
      {
        InputMapper.mHandleGuestBootLogs = new InputMapper.HandleGuestBootLogs(InputMapper.HandleGuestBootLogsImpl);
        InputMapper.BstGetGuestBootLogs(InputMapper.mHandleGuestBootLogs);
      }
      catch (Exception ex)
      {
        Logger.Error("Error in RegisterGuestBootLogsHandler " + ex?.ToString());
      }
    }

    internal void InputmapperInit()
    {
      this.mUserFolder = Path.Combine(RegistryStrings.InputMapperFolder, "UserFiles");
      this.mCursorDeltas = new Point[4];
      this.mTiltHandler = new InputMapper.TiltHandler(this.TiltHandlerImpl);
      this.mClientActionHandler = new InputMapper.ClientActionHandler(this.ClientActionHandlerImpl);
      this.mClientGetGamepadButtonHandler = new InputMapper.ClientGetGamepadButtonHandler(this.ClientGetGamepadButtonImpl);
      this.mSetGamepadStatusHandler = new InputMapper.SetGamepadStatusHandler(this.SetGamepadStatusHandlerImpl);
      this.mGamepadBackButtonPressedHandler = new InputMapper.GamepadBackButtonPressedHandler(this.GamepadBackButtonPressedHandlerImpl);
      this.mPlaybackCompleteHandler = new InputMapper.PlaybackCompleteHandler(MacroManager.Instance.PlaybackCompleteHandlerImpl);
      this.mGameControlStatusUpdateHandler = new InputMapper.GameControlStatusUpdate(this.GameControlsStatusUpdateHandlerImpl);
      this.mRegisterRawInputMouseHandler = new InputMapper.RegisterRawInputMouseHandler(this.RegisterRawInputMouseImpl);
      this.mSetMouseCursorPos = new InputMapper.SetMouseCursorPos(this.SetMouseCursorPosImpl);
      this.mShowMouseCursor = new InputMapper.ShowMouseCursor(this.ShowMouseCursorImpl);
      int guestWidth = RegistryManager.Instance.DefaultGuest.GuestWidth;
      int guestHeight = RegistryManager.Instance.DefaultGuest.GuestHeight;
      IntPtr hModule = InputMapper.LoadLibrary("HD-Plus-Frontend-Native.dll");
      if (hModule == IntPtr.Zero)
      {
        Debugger.Break();
      }
      else
      {
        IntPtr procAddress1 = InputMapper.GetProcAddress(hModule, "MonitorSendTouchState");
        IntPtr procAddress2 = InputMapper.GetProcAddress(hModule, "MonitorSendScanCode");
        IntPtr procAddress3 = InputMapper.GetProcAddress(hModule, "MonitorSendGamepadState");
        IntPtr procAddress4 = InputMapper.GetProcAddress(hModule, "MonitorSendImeMsg");
        InputMapper.ImapGamepadEnable(RegistryManager.Instance.GamepadDetectionEnabled);
        InputMapper.ImapInit(guestWidth, guestHeight, RegistryStrings.InputMapperFolder, this.mUserFolder, Logger.GetHdLoggerCallback(), this.mSetMouseCursorPos, this.mShowMouseCursor, procAddress1, procAddress2, procAddress3, procAddress4, this.mTiltHandler, this.mRegisterRawInputMouseHandler, this.mClientActionHandler, this.mClientGetGamepadButtonHandler, MultiInstanceStrings.VmName, this.mGameControlStatusUpdateHandler);
        InputMapper.ImapRegisterGamepadConnectedCb(this.mSetGamepadStatusHandler, this.mGamepadBackButtonPressedHandler);
      }
    }

    internal static void SoftControlBarVisible(bool visible)
    {
      float landscape = 0.0f;
      float portrait = 0.0f;
      InputMapper.Instance.mSoftControlEnabled = visible;
      if (visible)
      {
        double barHeightLandscape = (double) RegistryManager.Instance.DefaultGuest.SoftControlBarHeightLandscape;
        float barHeightPortrait = (float) RegistryManager.Instance.DefaultGuest.SoftControlBarHeightPortrait;
        double height = (double) LayoutManager.mConfiguredDisplaySize.Height;
        landscape = (float) (barHeightLandscape / height);
        portrait = barHeightPortrait / (float) LayoutManager.mConfiguredDisplaySize.Width;
      }
      InputMapper.Instance.SetSoftControlBarHeight(landscape, portrait);
    }

    internal void SetupSoftControlBar()
    {
      if (Utils.IsAndroidFeatureBitEnabled(1U, MultiInstanceStrings.VmName))
        return;
      Logger.Info("Soft Control Bar Enabled");
      InputMapper.SoftControlBarVisible(true);
    }

    internal void SetSoftControlBarHeight(float landscape, float portrait)
    {
      Logger.Info("SetSoftControlBarHeight({0}, {1})", (object) landscape, (object) portrait);
      this.mSoftControlBarHeightLandscape = landscape;
      this.mSoftControlBarHeightPortrait = portrait;
    }

    public void SetMonitor(Monitor monitor)
    {
      this.mMonitor = monitor;
    }

    internal string GetMacroFileName(bool isUserDirectoryRequested, string PackageName)
    {
      string path2 = PackageName + "_macro.cfg";
      string str = Path.Combine(RegistryStrings.InputMapperFolder, path2);
      string path = Path.Combine(this.mUserFolder, path2);
      return System.IO.File.Exists(path) | isUserDirectoryRequested ? path : str;
    }

    public string GetPackage()
    {
      return this.mCurrentPackage;
    }

    public void SetPackage(string package)
    {
      this.mCurrentPackage = package;
      MacroData.Instance.LoadMacroData(this.mCurrentPackage);
      this.mSerialQueue.Enqueue((SerialWorkQueue.Work) (() =>
      {
        Logger.Info("Package name is {0}", (object) package);
        InputMapper.ImapLoadConfig(package, "", ref InputMapper.isSendSensorDeviceData);
      }));
    }

    public void InitMacroPlayback(string filePath)
    {
      InputMapper.ImapInitPlayback(filePath, this.mPlaybackCompleteHandler);
      InputMapper.IsMacroPlaying = true;
    }

    public void StopMacroPlayback()
    {
      InputMapper.ImapStopPlayback();
      InputMapper.IsMacroPlaying = false;
    }

    public void RunMacroUnit(string macroName, double acceleration)
    {
      InputMapper.ImapRunMacroUnit(macroName, acceleration);
    }

    internal void StartOperationsSync()
    {
      InputMapper.ImapStartSync(true);
    }

    internal void StopOperationsSync()
    {
      InputMapper.ImapStopSync(true);
    }

    internal void PlayPauseOperationsSync(bool isPause)
    {
      if (isPause)
        InputMapper.ImapStopSync(false);
      else
        InputMapper.ImapStartSync(false);
    }

    internal void StartSyncConsumer(string fromVmName)
    {
      InputMapper.ImapStartSyncConsumer(fromVmName);
    }

    internal void StopSyncConsumer()
    {
      InputMapper.ImapStopSyncConsumer();
    }

    public void RefreshKeymapping(string package)
    {
      this.mCurrentPackage = package;
      MacroData.Instance.LoadMacroData(this.mCurrentPackage);
      this.mSerialQueue.Enqueue((SerialWorkQueue.Work) (() =>
      {
        Logger.Info("Package name is {0}", (object) package);
        this.SetPackage(package);
      }));
    }

    public void HandleLoadConfigAfterHomeSwitch(string package)
    {
      InputMapper.ImapSwitchBackFromHomeCallback(package);
    }

    public void ShowConfigDialog()
    {
      int num = (int) new InputMapperForm(this.mCurrentPackage, new InputMapperForm.EditHandler(this.EditHandler), new InputMapperForm.ManageHandler(this.ManageHandler)).ShowDialog();
    }

    private void EditHandler(string package)
    {
      string path2 = package + ".cfg";
      string sourceFileName = Path.Combine(RegistryStrings.InputMapperFolder, "TEMPLATE.cfg");
      string str1 = Path.Combine(RegistryStrings.InputMapperFolder, path2);
      string str2 = Path.Combine(this.mUserFolder, path2);
      if (!System.IO.File.Exists(str2) && System.IO.File.Exists(str1))
        System.IO.File.Copy(str1, str2);
      string str3 = str2;
      Logger.Info("Editing input mapper file '{0}'", (object) str3);
      try
      {
        if (!System.IO.File.Exists(str3))
          System.IO.File.Copy(sourceFileName, str3);
        new Process()
        {
          StartInfo = {
            FileName = "notepad.exe",
            Arguments = ("\"" + str3 + "\"")
          }
        }.Start();
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in edit input mapper file Err : " + ex.ToString());
      }
    }

    private void ManageHandler(string package)
    {
      string path = Path.Combine(this.mUserFolder, package + ".cfg");
      string str = RegistryStrings.InputMapperFolder;
      if (System.IO.File.Exists(path))
        str = this.mUserFolder;
      try
      {
        new Process() { StartInfo = { FileName = str } }.Start();
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in open input mapper folder. Err : " + ex.ToString());
      }
    }

    public void DispatchKeyboardEvent(uint keyCode, bool down)
    {
      if (string.IsNullOrEmpty(AppHandler.mCurrentAppPackage))
        return;
      VMWindow.Instance.SerialQueue.Enqueue((SerialWorkQueue.Work) (() => InputMapper.ImapHandleKey(keyCode, down ? 1 : 0)));
      if (AppHandler.sAppPackagesCountClicks.ContainsKey(AppHandler.mCurrentAppPackage.ToLower()) & down)
      {
        AppHandler.sAppPackagesCountClicks[AppHandler.mCurrentAppPackage.ToLower()]++;
        Logger.Debug("DispatchKeyboardEvent " + AppHandler.mCurrentAppPackage + AppHandler.sAppPackagesCountClicks[AppHandler.mCurrentAppPackage.ToLower()].ToString());
      }
      if (new JsonParser(string.Empty).IsPackageNameSystemApp(AppHandler.mCurrentAppPackage) || AppHandler.mCurrentAppPackage.Equals("Home"))
        return;
      if (AppHandler.sAppPackagesCountClicks.ContainsKey("*") & down)
        ++AppHandler.sAppPackagesCountClicks["*"];
      if (!(AppHandler.sAppPackagesCountClicks.ContainsKey("?") & down))
        return;
      if (!AppHandler.sDictCountClicks.ContainsKey(AppHandler.mCurrentAppPackage.ToLower()))
      {
        AppHandler.sDictCountClicks.Add(AppHandler.mCurrentAppPackage.ToLower(), 0L);
        AppHandler.sDictCountClicks[AppHandler.mCurrentAppPackage.ToLower()]++;
      }
      else
        AppHandler.sDictCountClicks[AppHandler.mCurrentAppPackage.ToLower()]++;
    }

    public void DispatchControllerEvent(int identity, uint button, int down)
    {
      this.mSerialQueue.Enqueue((SerialWorkQueue.Work) (() =>
      {
        long num;
        if (AppHandler.sAppPackagesCountClicks.ContainsKey(AppHandler.mCurrentAppPackage.ToLower()))
        {
          AppHandler.sAppPackagesCountClicks[AppHandler.mCurrentAppPackage.ToLower()]++;
          string currentAppPackage = AppHandler.mCurrentAppPackage;
          num = AppHandler.sAppPackagesCountClicks[AppHandler.mCurrentAppPackage.ToLower()];
          string str = num.ToString();
          Logger.Debug("DispatchControllerEvent " + currentAppPackage + str);
        }
        if (new JsonParser(string.Empty).IsPackageNameSystemApp(AppHandler.mCurrentAppPackage) || AppHandler.mCurrentAppPackage.Equals("Home"))
          return;
        if (AppHandler.sAppPackagesCountClicks.ContainsKey("*"))
          num = AppHandler.sAppPackagesCountClicks["*"]++;
        if (!AppHandler.sAppPackagesCountClicks.ContainsKey("?"))
          return;
        if (!AppHandler.sDictCountClicks.ContainsKey(AppHandler.mCurrentAppPackage.ToLower()))
        {
          AppHandler.sDictCountClicks.Add(AppHandler.mCurrentAppPackage.ToLower(), 0L);
          num = AppHandler.sDictCountClicks[AppHandler.mCurrentAppPackage.ToLower()]++;
        }
        else
          num = AppHandler.sDictCountClicks[AppHandler.mCurrentAppPackage.ToLower()]++;
      }));
    }

    public void DispatchGamePadUpdate(int identity, GamePad gamepad)
    {
      this.mSerialQueue.Enqueue((SerialWorkQueue.Work) (() =>
      {
        long num;
        if (AppHandler.sAppPackagesCountClicks.ContainsKey(AppHandler.mCurrentAppPackage.ToLower()))
        {
          AppHandler.sAppPackagesCountClicks[AppHandler.mCurrentAppPackage.ToLower()]++;
          string currentAppPackage = AppHandler.mCurrentAppPackage;
          num = AppHandler.sAppPackagesCountClicks[AppHandler.mCurrentAppPackage.ToLower()];
          string str = num.ToString();
          Logger.Debug("DispatchGamePadUpdate " + currentAppPackage + str);
        }
        if (new JsonParser(string.Empty).IsPackageNameSystemApp(AppHandler.mCurrentAppPackage) || AppHandler.mCurrentAppPackage.Equals("Home"))
          return;
        if (AppHandler.sAppPackagesCountClicks.ContainsKey("*"))
          num = AppHandler.sAppPackagesCountClicks["*"]++;
        if (!AppHandler.sAppPackagesCountClicks.ContainsKey("?"))
          return;
        if (!AppHandler.sDictCountClicks.ContainsKey(AppHandler.mCurrentAppPackage.ToLower()))
        {
          AppHandler.sDictCountClicks.Add(AppHandler.mCurrentAppPackage.ToLower(), 0L);
          num = AppHandler.sDictCountClicks[AppHandler.mCurrentAppPackage.ToLower()]++;
        }
        else
          num = AppHandler.sDictCountClicks[AppHandler.mCurrentAppPackage.ToLower()]++;
      }));
    }

    internal void SendAndroidString(string formattedString)
    {
      InputMapper.ImapHandleIme(formattedString);
    }

    internal void SendClientString(string formattedString)
    {
      InputMapper.ImapHandleClient(formattedString, false);
    }

    public void TouchHandlerImpl(IntPtr array, int count, int offset)
    {
      this.TouchHandlerImpl(array, count, offset, true);
    }

    public void TouchHandlerImpl(IntPtr array, int count, int offset, bool adjustForControlBar)
    {
      try
      {
        this.TouchHandlerImplInternal(array, count, offset, adjustForControlBar);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in sending mapped touch points: " + ex.ToString());
      }
    }

    public void TouchHandlerImplInternal(
      IntPtr array,
      int count,
      int offset,
      bool adjustForControlBar)
    {
      InputMapper.TouchPoint[] points = new InputMapper.TouchPoint[count];
      int num = Marshal.SizeOf(typeof (InputMapper.TouchPoint));
      for (int index = 0; index < count; ++index)
      {
        IntPtr ptr = new IntPtr(array.ToInt64() + (long) (index * num));
        points[index] = (InputMapper.TouchPoint) Marshal.PtrToStructure(ptr, typeof (InputMapper.TouchPoint));
      }
      this.TouchHandlerImpl(points, offset, adjustForControlBar);
    }

    public void TouchHandlerImpl(
      InputMapper.TouchPoint[] points,
      int offset,
      bool adjustForControlBar)
    {
      for (int index = 0; index + offset < this.mTouchPoints.Length && index < points.Length; ++index)
      {
        InputMapper.TouchPoint point = points[index];
        if (!point.Down)
        {
          this.mTouchPoints[index + offset].PosX = int.MaxValue;
          this.mTouchPoints[index + offset].PosY = int.MaxValue;
        }
        else
        {
          int num1 = (int) ((double) point.X * (double) short.MaxValue);
          int num2 = (int) ((double) point.Y * (double) short.MaxValue);
          float num3 = 0.0f;
          if (!LayoutManager.mEmulatedPortraitMode)
          {
            if (adjustForControlBar)
              num3 = this.mSoftControlBarHeightLandscape;
            if (index + offset != 13)
              num2 = (int) ((double) num2 * (1.0 - (double) num3));
            if (!LayoutManager.mRotateGuest180)
            {
              this.mTouchPoints[index + offset].PosX = num1;
              this.mTouchPoints[index + offset].PosY = num2;
            }
            else
            {
              this.mTouchPoints[index + offset].PosX = (int) short.MaxValue - num1;
              this.mTouchPoints[index + offset].PosY = (int) short.MaxValue - num2;
            }
          }
          else
          {
            if (adjustForControlBar)
              num3 = this.mSoftControlBarHeightPortrait;
            if (index + offset != 13)
              num2 = (int) ((double) num2 * (1.0 - (double) num3));
            if (!LayoutManager.mRotateGuest180)
            {
              this.mTouchPoints[index + offset].PosX = (int) short.MaxValue - num2;
              this.mTouchPoints[index + offset].PosY = num1;
            }
            else
            {
              this.mTouchPoints[index + offset].PosX = num2;
              this.mTouchPoints[index + offset].PosY = (int) short.MaxValue - num1;
            }
          }
        }
      }
      if (this.mMonitor == null)
        return;
      this.mMonitor.SendTouchState(this.mTouchPoints);
    }

    public void UpdateMouseStatus(
      int guestX,
      int guestY,
      int msg,
      MouseButtons button,
      int delta = 0)
    {
      int x = (int) ((double) guestX * (double) LayoutManager.mScaledDisplayArea.Width / 32768.0);
      int y = (int) ((double) guestY * (double) LayoutManager.mScaledDisplayArea.Height / 32768.0);
      InputMapper.ImapHandleMouse(msg, x, y, (button & MouseButtons.Left) == MouseButtons.Left, (button & MouseButtons.Right) == MouseButtons.Right, (button & MouseButtons.Middle) == MouseButtons.Middle, (button & MouseButtons.XButton1) == MouseButtons.XButton1, (button & MouseButtons.XButton2) == MouseButtons.XButton2, delta);
    }

    private void TiltHandlerImpl(IntPtr context, float x, float y, float z)
    {
      SensorDevice.Instance.SetAccelerometerVector(x, y, z);
    }

    private void PlaybackCompleteHandlerImpl()
    {
      HTTPUtils.SendRequestToClientAsync("macroPlaybackComplete", (Dictionary<string, string>) null, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
    }

    private void ClientActionHandlerImpl([MarshalAs(UnmanagedType.LPStr), In] string action)
    {
      HTTPUtils.SendRequestToClientAsync("handleClientOperation", new Dictionary<string, string>()
      {
        {
          "data",
          action
        }
      }, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
    }

    private void GameControlsStatusUpdateHandlerImpl([MarshalAs(UnmanagedType.LPStr), In] string values, [In] int valueCount, string vmname)
    {
      Logger.Info("Got callback Gamecontrolstatus...");
      Dictionary<string, string> data = new Dictionary<string, string>()
      {
        {
          "data",
          values
        }
      };
      Logger.Info("Vmname from Imap: " + vmname);
      HTTPUtils.SendRequestToClientAsync("overlayControlsVisibility", data, vmname, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
    }

    private void ClientGetGamepadButtonImpl([MarshalAs(UnmanagedType.LPStr), In] string gamepadButton, [In] int down)
    {
      HTTPUtils.SendRequestToClientAsync("handleClientGamepadButton", new Dictionary<string, string>()
      {
        {
          "data",
          gamepadButton
        },
        {
          "isDown",
          down == 1 ? "true" : "false"
        }
      }, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
    }

    private void SetGamepadStatusHandlerImpl(int gamepadStatus)
    {
      if (!(!InputMapper.mIsGamepadConnected ^ gamepadStatus == 0))
        return;
      InputMapper.mIsGamepadConnected = gamepadStatus != 0;
      if (Opt.Instance.sysPrep || !Oem.Instance.IsSendGameManagerRequest)
        return;
      HTTPUtils.SendRequestToClientAsync("handleGamepadConnection", new Dictionary<string, string>()
      {
        {
          "status",
          InputMapper.mIsGamepadConnected.ToString()
        }
      }, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>) null, false, 5, 1000, "bgp");
    }

    private void GamepadBackButtonPressedHandlerImpl()
    {
      HTTPUtils.SendRequestToClientAsync("handleGamepadGuidanceButton", new Dictionary<string, string>(), MultiInstanceStrings.VmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
    }

    private void RegisterRawInputMouseImpl(bool register)
    {
      InputMapper.RAWINPUTDEVICE[] pRawInputDevices = new InputMapper.RAWINPUTDEVICE[1];
      pRawInputDevices[0].usUsagePage = (ushort) 1;
      pRawInputDevices[0].usUsage = (ushort) 2;
      if (register)
      {
        pRawInputDevices[0].hwndTarget = VMWindow.Instance.Handle;
        pRawInputDevices[0].dwFlags = 304;
      }
      else
      {
        pRawInputDevices[0].hwndTarget = IntPtr.Zero;
        pRawInputDevices[0].dwFlags = 1;
      }
      InputMapper.RegisterRawInputDevices(pRawInputDevices, 1, Marshal.SizeOf(typeof (InputMapper.RAWINPUTDEVICE)));
    }

    private void SetMouseCursorPosImpl(int cx, int cy, bool clipInGuestWindow)
    {
      if (VMWindow.Instance == null)
        return;
      VMWindow.Instance.SetMouseCursorPos(cx, cy, clipInGuestWindow);
    }

    private static void HandleGuestBootLogsImpl(IntPtr bootLogs, int nLogs)
    {
      ThreadPool.QueueUserWorkItem((WaitCallback) (_ =>
      {
        try
        {
          IntPtr[] destination = new IntPtr[nLogs];
          Marshal.Copy(bootLogs, destination, 0, nLogs);
          if (!Marshal.PtrToStringAnsi(destination[0]).Equals("timeline", StringComparison.OrdinalIgnoreCase))
            return;
          TimelineStatsSender.HandleEngineBootEvent(Marshal.PtrToStringAnsi(destination[1]));
        }
        catch (Exception ex)
        {
          Logger.Error("Failed to parse boot event");
          Logger.Error(ex.ToString());
        }
      }));
    }

    private void ShowMouseCursorImpl(bool show, bool clippingEnabled = true)
    {
      if (VMWindow.Instance == null)
        return;
      VMWindow.Instance.ShowMouseCursor(show, clippingEnabled);
    }

    internal void RefreshKeyMapping(string appPkg)
    {
      Logger.Info("Refresh Keymapping");
      if (string.IsNullOrEmpty(appPkg) && !string.IsNullOrEmpty(AppHandler.mCurrentAppPackage))
        this.RefreshKeymapping(AppHandler.mCurrentAppPackage);
      else
        this.RefreshKeymapping(appPkg);
    }

    internal void LaunchBlueStacksKeyMapper()
    {
      try
      {
        string guest = HTTPUtils.SendRequestToGuest("customStartService", new Dictionary<string, string>()
        {
          {
            "action",
            "com.bluestacks.appguidance.GuidanceScreen"
          },
          {
            "extras",
            "{\"event\":\"show_guidance_app_player\"}"
          }
        }, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>) null, true, 10, 500, "bgp");
        if (JObject.Parse(guest)["result"].ToString().Trim() == "ok")
          Logger.Info("The key mapping tool launched successfully");
        else
          Logger.Error("The key mapping tool could not be launched, Response: {0}", (object) guest);
      }
      catch (Exception ex)
      {
        Logger.Error(string.Format("Exception occured in trying to launch key mapping tool. Err : {0}", (object) ex.ToString()));
      }
    }

    internal void HandleRawInput(IntPtr lParam)
    {
      uint pcbSize = 0;
      int rawInputData = (int) InputMapper.GetRawInputData(lParam, 268435459U, IntPtr.Zero, ref pcbSize, (uint) Marshal.SizeOf(typeof (InputMapper.RAWINPUTHEADER)));
      IntPtr num = Marshal.AllocHGlobal((int) pcbSize);
      if ((int) InputMapper.GetRawInputData(lParam, 268435459U, num, ref pcbSize, (uint) Marshal.SizeOf(typeof (InputMapper.RAWINPUTHEADER))) != (int) pcbSize)
      {
        Logger.Error("GetRawInputData does not return correct size\n");
      }
      else
      {
        InputMapper.ImapHandleRawInput(num);
        Marshal.FreeHGlobal(num);
      }
    }

    internal void HandleTouchEvent(object sender, WMTouchForm.WMTouchEventArgs e)
    {
      VMWindow.Instance.UpdateUserActivityStatus();
      int nTouchPoints = 0;
      for (int ndx = 0; ndx < 16; ++ndx)
      {
        WMTouchForm.TouchPoint point = e.GetPoint(ndx);
        if (point.Id != -1)
        {
          int landscapeGuestX = LayoutManager.GetLandscapeGuestX(point.X, point.Y);
          int landscapeGuestY = LayoutManager.GetLandscapeGuestY(point.X, point.Y);
          this.touchPoints[ndx].PosX = (int) ((double) landscapeGuestX * (double) LayoutManager.mScaledDisplayArea.Width / 32768.0);
          this.touchPoints[ndx].PosY = (int) ((double) landscapeGuestY * (double) LayoutManager.mScaledDisplayArea.Height / 32768.0);
          ++nTouchPoints;
        }
        else
        {
          this.touchPoints[ndx].PosX = int.MaxValue;
          this.touchPoints[ndx].PosY = int.MaxValue;
        }
      }
      InputMapper.ImapHandleTouch(this.touchPoints, nTouchPoints);
    }

    internal void HandleMouseWheel(object sender, MouseEventArgs e)
    {
      VMWindow.Instance.UpdateUserActivityStatus();
      if (Input.IsEventFromTouch())
        return;
      this.UpdateMouseStatus(LayoutManager.GetLandscapeGuestX(e.X, e.Y), LayoutManager.GetLandscapeGuestY(e.X, e.Y), 522, e.Button, e.Delta);
    }

    internal void HandleMouseUp(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Right)
      {
        if (VMWindow.Instance.mIsInScriptMode)
          return;
        if (VMWindow.Instance.mIsCustomCursorEnabled)
          VMWindow.Instance.ChangeCursorStyle(Constants.CustomCursorPath, false);
        else
          VMWindow.Instance.ChangeCursorStyle("", false);
      }
      VMWindow.Instance.UpdateUserActivityStatus();
      MacroForm.RecordMouse((double) e.X, (double) e.Y, (double) VMWindow.Instance.ClientSize.Width, (double) VMWindow.Instance.ClientSize.Height, e.Button, ActionType.MouseUp);
      if (AppHandler.mCurrentAppPackage != null)
      {
        long num;
        if (AppHandler.sAppPackagesCountClicks.ContainsKey(AppHandler.mCurrentAppPackage.ToLower()))
        {
          AppHandler.sAppPackagesCountClicks[AppHandler.mCurrentAppPackage.ToLower()]++;
          string currentAppPackage = AppHandler.mCurrentAppPackage;
          num = AppHandler.sAppPackagesCountClicks[AppHandler.mCurrentAppPackage.ToLower()];
          string str = num.ToString();
          Logger.Debug("HandleMouseUp " + currentAppPackage + str);
        }
        if (!new JsonParser(string.Empty).IsPackageNameSystemApp(AppHandler.mCurrentAppPackage) && !AppHandler.mCurrentAppPackage.Equals("Home"))
        {
          if (AppHandler.sAppPackagesCountClicks.ContainsKey("*"))
            num = AppHandler.sAppPackagesCountClicks["*"]++;
          if (AppHandler.sAppPackagesCountClicks.ContainsKey("?"))
          {
            if (!AppHandler.sDictCountClicks.ContainsKey(AppHandler.mCurrentAppPackage.ToLower()))
            {
              AppHandler.sDictCountClicks.Add(AppHandler.mCurrentAppPackage.ToLower(), 0L);
              num = AppHandler.sDictCountClicks[AppHandler.mCurrentAppPackage.ToLower()]++;
            }
            else
              num = AppHandler.sDictCountClicks[AppHandler.mCurrentAppPackage.ToLower()]++;
          }
        }
      }
      this.HandleMouseButton(e.X, e.Y, e.Button, false, false);
    }

    internal void HandleMouseDown(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Right)
      {
        if (VMWindow.Instance.mIsInScriptMode)
          return;
        if (RegistryManager.Instance.CustomCursorEnabled && VMWindow.Instance.IsPackageAvailableForCustomCursor(AppHandler.mCurrentAppPackage))
          VMWindow.Instance.ChangeCursorStyle(Constants.MOBACursorPath, true);
      }
      VMWindow.Instance.UpdateUserActivityStatus();
      MacroForm.RecordMouse((double) e.X, (double) e.Y, (double) VMWindow.Instance.ClientSize.Width, (double) VMWindow.Instance.ClientSize.Height, e.Button, ActionType.MouseDown);
      if (e.Button == MouseButtons.Left)
      {
        Logger.Debug("left button");
        Logger.Debug("{0},{1}", (object) e.X, (object) e.Y);
        object[] objArray = new object[2];
        Size clientSize = VMWindow.Instance.ClientSize;
        objArray[0] = (object) clientSize.Width;
        clientSize = VMWindow.Instance.ClientSize;
        objArray[1] = (object) clientSize.Height;
        Logger.Debug("{0},{1}", objArray);
      }
      this.HandleMouseButton(e.X, e.Y, e.Button, true, false);
    }

    internal void HandleMouseMove(object sender, MouseEventArgs e)
    {
      Point mouseMoveLocation = this.mLastMouseMoveLocation;
      if (this.mLastMouseMoveLocation != e.Location)
        VMWindow.Instance.UpdateUserActivityStatus();
      this.mLastMouseMoveLocation = e.Location;
      if (Math.Abs(Environment.TickCount - VMWindow.sLastTouchTime) <= 1000 || Input.IsEventFromTouch() || AndroidBootUp.mMonitor == null)
        return;
      int x = e.X;
      int y = e.Y;
      this.UpdateMouseStatus(LayoutManager.GetLandscapeGuestX(x, y), LayoutManager.GetLandscapeGuestY(x, y), 512, e.Button, 0);
      if (VMWindow.Instance.s_KeyMapTeachMode || VMWindow.Instance.mIsInScriptMode)
      {
        double x1;
        double y1;
        this.GetMousePercentLocationFromPointToClient(out x1, out y1);
        string text = !VMWindow.Instance.s_KeyMapTeachMode ? string.Format(" X: {0}, Y: {1}", (object) Math.Round(x1, 2), (object) Math.Round(y1, 2)) : string.Format("  [ x={0}%, y={1}% - {2}]", (object) Math.Round(x1, 2), (object) Math.Round(y1, 2), (object) AppHandler.mCurrentAppPackage);
        VMWindow.Instance.sKeyMapToolTip.Show(text, (IWin32Window) VMWindow.Instance, x + 10, y + 15, 100000);
      }
      else
        VMWindow.Instance.sKeyMapToolTip.Hide((IWin32Window) VMWindow.Instance);
    }

    internal void GetMousePercentLocationFromPointToClient(out double x1, out double y1)
    {
      x1 = y1 = 0.0;
      Point client = VMWindow.Instance.PointToClient(Cursor.Position);
      int x = client.X;
      int y = client.Y;
      x1 = 100.0 * (double) LayoutManager.GetGuestX(x, y) / 32768.0;
      y1 = 100.0 * (double) LayoutManager.GetGuestY(x, y) / 32768.0;
      if (LayoutManager.mEmulatedPortraitMode)
      {
        double num = x1;
        x1 *= 1.0 - (double) this.mSoftControlBarHeightPortrait;
        if (!LayoutManager.mRotateGuest180)
        {
          x1 = y1;
          y1 = 100.0 - num;
        }
        else
        {
          x1 = 100.0 - y1;
          y1 = num;
        }
      }
      if (!this.mSoftControlEnabled)
        return;
      double num1 = y1 * (double) LayoutManager.mScaledDisplayArea.Height / 100.0;
      double num2 = (double) LayoutManager.mScaledDisplayArea.Height - (double) this.mSoftControlBarHeightLandscape * (double) LayoutManager.mConfiguredDisplaySize.Height;
      y1 = num1 / num2 * 100.0;
      if (y1 <= 100.0 && y1 >= 0.0)
        return;
      VMWindow.Instance.sKeyMapToolTip.Hide((IWin32Window) VMWindow.Instance);
    }

    internal void HandleMouseButton(int x, int y, MouseButtons button, bool pressed, bool force)
    {
      VMWindow.Instance.UpdateUserActivityStatus();
      if (Input.IsEventFromTouch() || AndroidBootUp.mMonitor == null)
        return;
      object[] objArray = new object[4];
      Size clientSize = VMWindow.Instance.ClientSize;
      objArray[0] = (object) clientSize.Width;
      clientSize = VMWindow.Instance.ClientSize;
      objArray[1] = (object) clientSize.Height;
      objArray[2] = pressed ? (object) "down" : (object) "up";
      objArray[3] = button == MouseButtons.Left ? (object) "left" : (object) "right";
      Logger.Debug("{3} Mouse {2} at {0}, {1}", objArray);
      switch (button)
      {
        case MouseButtons.Left:
          this.UpdateMouseStatus(LayoutManager.GetLandscapeGuestX(x, y), LayoutManager.GetLandscapeGuestY(x, y), pressed ? 513 : 514, button, 0);
          break;
        case MouseButtons.Right:
          this.UpdateMouseStatus(LayoutManager.GetLandscapeGuestX(x, y), LayoutManager.GetLandscapeGuestY(x, y), pressed ? 516 : 517, button, 0);
          break;
        case MouseButtons.Middle:
          this.UpdateMouseStatus(LayoutManager.GetLandscapeGuestX(x, y), LayoutManager.GetLandscapeGuestY(x, y), pressed ? 519 : 520, button, 0);
          break;
        case MouseButtons.XButton1:
        case MouseButtons.XButton2:
          this.UpdateMouseStatus(LayoutManager.GetLandscapeGuestX(x, y), LayoutManager.GetLandscapeGuestY(x, y), pressed ? 523 : 524, button, 0);
          break;
        default:
          Mouse.Instance.UpdateButton((uint) LayoutManager.GetGuestX(x, y), (uint) LayoutManager.GetGuestY(x, y), button, pressed);
          AndroidBootUp.mMonitor.SendMouseState(Mouse.Instance.X, Mouse.Instance.Y, Mouse.Instance.Mask);
          break;
      }
    }

    internal static void Shake()
    {
      InputMapper.isSendSensorDeviceData = true;
      InputMapper.ImapShake();
    }

    internal static void StartRecording()
    {
      InputMapper.ImapStartRecording();
    }

    internal static void StopRecording(StringBuilder events, ref int size)
    {
      InputMapper.ImapStopRecording(events, ref size);
    }

    internal static void PauseRecording()
    {
      InputMapper.ImapPauseRecording();
    }

    internal static void SetInputMapperState(int state)
    {
      InputMapper.ImapSetState(state);
    }

    internal static void ToggleGamepadButton(bool isEnable)
    {
      InputMapper.ImapEnableGamepadButtonCb(isEnable);
    }

    internal static void GetInteractionStats(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        int numInteractions = 0;
        int numMappedInteractions = 0;
        int numTextInteractions = 0;
        int nativeGamepadUsed = 0;
        StringBuilder interactionHist = new StringBuilder(2400);
        InputMapper.ImapGetInteractionStats(ref numInteractions, ref numMappedInteractions, ref numTextInteractions, ref nativeGamepadUsed, interactionHist);
        HTTPUtils.Write(new JObject()
        {
          {
            "success",
            (JToken) true
          },
          {
            "s1",
            (JToken) numInteractions
          },
          {
            "s2",
            (JToken) numMappedInteractions
          },
          {
            "s3",
            (JToken) numTextInteractions
          },
          {
            "s4",
            (JToken) interactionHist.ToString()
          },
          {
            "s5",
            (JToken) nativeGamepadUsed
          }
        }.ToString(Formatting.None), res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in GetInteractionStats. Err : " + ex.ToString());
        HTTPHandler.WriteErrorJson(ex.Message, res);
      }
    }

    internal static void EnableGamepad(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        InputMapper.ImapGamepadEnable(Convert.ToBoolean(HTTPUtils.ParseRequest(req).Data["enable"].ToString()));
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in EnableGamepad: " + ex.ToString());
        HTTPHandler.WriteErrorJson(ex.Message, res);
      }
    }

    internal static void EnableNativeGamepadControls(
      HttpListenerRequest req,
      HttpListenerResponse res)
    {
      try
      {
        bool boolean = Convert.ToBoolean(HTTPUtils.ParseRequest(req).Data["isEnabled"].ToString());
        Logger.Debug("NATIVE_GAMEPAD: IsEnabled= " + boolean.ToString());
        InputMapper.ImapSetGamepadMode(boolean ? 1 : 0);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in EnableNativeGamepadControls: " + ex.ToString());
        HTTPHandler.WriteErrorJson(ex.Message, res);
      }
    }

    internal static void ExportCfgFile(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        string str = HTTPUtils.ParseRequest(req).Data["path"].ToString();
        if (!string.IsNullOrEmpty(InputMapper.Instance.mCurrentPackage))
        {
          if (System.IO.File.Exists(str))
            System.IO.File.Delete(str);
          System.IO.File.Copy(InputMapper.Instance.GetInputmapperFile(InputMapper.Instance.mCurrentPackage), str, true);
          HTTPHandler.WriteSuccessJson(res);
        }
        else
          HTTPHandler.WriteErrorJson("App not running", res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in ExportCfgFile: " + ex.ToString());
        HTTPHandler.WriteErrorJson(ex.Message, res);
      }
    }

    private string GetInputmapperFile(string packageName)
    {
      string str = string.Empty;
      try
      {
        string path1 = Path.Combine(Path.Combine(RegistryStrings.InputMapperFolder, "UserFiles"), packageName + ".cfg");
        string path2 = Path.Combine(RegistryStrings.InputMapperFolder, packageName + ".cfg");
        if (System.IO.File.Exists(path1))
          str = path1;
        else if (System.IO.File.Exists(path2))
          str = path2;
      }
      catch (Exception ex)
      {
        Logger.Error("Excpetion in GetInputMapper: " + ex.ToString());
      }
      return str;
    }

    internal static void ImportCfgFile(HttpListenerRequest req, HttpListenerResponse res)
    {
      try
      {
        string str = HTTPUtils.ParseRequest(req).Data["path"].ToString();
        if (!string.IsNullOrEmpty(InputMapper.Instance.mCurrentPackage))
        {
          Logger.Info("CFg file Selected : " + str);
          if (!InputMapper.Instance.IsValidCfg(str))
          {
            HTTPHandler.WriteErrorJson("Cfg not valid", res);
          }
          else
          {
            string destFileName = Path.Combine(Path.Combine(RegistryStrings.InputMapperFolder, "UserFiles"), InputMapper.Instance.mCurrentPackage + ".cfg");
            System.IO.File.Copy(str, destFileName, true);
            InputMapper.Instance.RefreshKeymapping(InputMapper.Instance.mCurrentPackage);
            HTTPHandler.WriteSuccessJson(res);
          }
        }
        else
          HTTPHandler.WriteErrorJson("App not running", res);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in Import CfgFile: " + ex.ToString());
        HTTPHandler.WriteErrorJson(ex.Message, res);
      }
    }

    internal bool IsValidCfg(string fileName)
    {
      try
      {
        return JsonConvert.DeserializeObject(System.IO.File.ReadAllText(fileName)) != null;
      }
      catch (Exception ex)
      {
        Logger.Error("invalid cfg file: {0}", (object) fileName);
        return false;
      }
    }

    internal bool IsScriptCommandsValid(string commandObj)
    {
      bool isValid = false;
      int errIndex = 0;
      int num = InputMapper.ImapValidateScriptCommands(new StringBuilder(commandObj), ref errIndex, ref isValid);
      Logger.Info(string.Format("Return value for ImapValidateScriptCommands : {0}", (object) num));
      if (isValid)
        Logger.Error(string.Format("Failed to parse script command : {0}. Error Index : {1}", (object) commandObj, (object) errIndex));
      return num == 0;
    }

    internal static void EnableScrollOnEdgeFeature(bool enable)
    {
      try
      {
        Logger.Info("SCROLL_ON_EDGE: IsEnabled= " + enable.ToString());
        InputMapper.ImapEnableScrollOnEdge(enable);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in EnableScrollOnEdgeFeature: " + ex.ToString());
      }
    }

    [System.Flags]
    public enum RawMouseFlags : ushort
    {
      MoveRelative = 0,
      MoveAbsolute = 1,
      VirtualDesktop = 2,
      AttributesChanged = 4,
    }

    [System.Flags]
    public enum RawMouseButtons : ushort
    {
      None = 0,
      LeftDown = 1,
      LeftUp = 2,
      RightDown = 4,
      RightUp = 8,
      MiddleDown = 16, // 0x0010
      MiddleUp = 32, // 0x0020
      Button4Down = 64, // 0x0040
      Button4Up = 128, // 0x0080
      Button5Down = 256, // 0x0100
      Button5Up = 512, // 0x0200
      MouseWheel = 1024, // 0x0400
    }

    [System.Flags]
    public enum RawKeyboardFlags : ushort
    {
      KeyMake = 0,
      KeyBreak = 1,
      KeyE0 = 2,
      KeyE1 = 4,
      TerminalServerSetLED = 8,
      TerminalServerShadow = 16, // 0x0010
      TerminalServerVKPACKET = 32, // 0x0020
    }

    internal struct RAWINPUTHEADER
    {
      [MarshalAs(UnmanagedType.U4)]
      public int dwType;
      [MarshalAs(UnmanagedType.U4)]
      public int dwSize;
      public IntPtr hDevice;
      [MarshalAs(UnmanagedType.U4)]
      public int wParam;
    }

    public struct RAWINPUTMOUSE
    {
      public InputMapper.RawMouseFlags Flags;
      public ushort ButtonData;
      public InputMapper.RawMouseButtons ButtonFlags;
      public ulong RawButtons;
      public long LastX;
      public long LastY;
      public ulong ExtraInformation;
    }

    public struct RAWINPUTKEYBOARD
    {
      public short MakeCode;
      public InputMapper.RawKeyboardFlags Flags;
      public short Reserved;
      public ushort VirtualKey;
      public uint Message;
      public int ExtraInformation;
    }

    public struct RAWINPUTHID
    {
      public int Size;
      public int Count;
      public IntPtr Data;
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct RAWINPUT32
    {
      [FieldOffset(0)]
      public InputMapper.RAWINPUTHEADER header;
      [FieldOffset(16)]
      public InputMapper.RAWINPUTMOUSE mouse;
      [FieldOffset(16)]
      public InputMapper.RAWINPUTKEYBOARD keyboard;
      [FieldOffset(16)]
      public InputMapper.RAWINPUTHID hid;
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct RAWINPUT64
    {
      [FieldOffset(0)]
      public InputMapper.RAWINPUTHEADER header;
      [FieldOffset(24)]
      public InputMapper.RAWINPUTMOUSE mouse;
      [FieldOffset(24)]
      public InputMapper.RAWINPUTKEYBOARD keyboard;
      [FieldOffset(24)]
      public InputMapper.RAWINPUTHID hid;
    }

    public struct TouchPoint
    {
      public float X;
      public float Y;
      public bool Down;
    }

    public struct BST_INPUT_TOUCH_POINT
    {
      public int X;
      public int Y;
    }

    public enum Direction
    {
      None,
      Up,
      Down,
      Left,
      Right,
    }

    public enum GamepadEvent
    {
      None,
      Attach,
      Detach,
      GuidancePress,
      GuidanceRelease,
    }

    public delegate void ModeHandlerNet(string mode);

    private delegate void KeyHandler(IntPtr context, byte code);

    private delegate void TouchHandler(IntPtr context, IntPtr list, int count, int offset);

    private delegate void TiltHandler(IntPtr context, float x, float y, float z);

    private delegate void ClientActionHandler([MarshalAs(UnmanagedType.LPStr), In] string action);

    private delegate void GameControlStatusUpdate([MarshalAs(UnmanagedType.LPStr), In] string values, int valueCount, string vmName);

    private delegate void ClientGetGamepadButtonHandler([MarshalAs(UnmanagedType.LPStr), In] string gamepadButton, [In] int down);

    private delegate void SetGamepadStatusHandler(int state);

    private delegate void GamepadBackButtonPressedHandler();

    private delegate void PlaybackCompleteHandler();

    private delegate void RegisterRawInputMouseHandler(bool register);

    private delegate void ModeHandler(IntPtr context, string mode);

    private delegate void MoveHandler(IntPtr context, int identity, int x, int y);

    private delegate void ClickHandler(IntPtr context, int identity, int down);

    private delegate void SpecialHandler(IntPtr context, string cmd);

    private delegate void GamepadHandler(
      IntPtr context,
      int identity,
      InputMapper.GamepadEvent evt,
      string layout);

    private delegate void MouseHandler(IntPtr context);

    private delegate IntPtr ShootHandler(IntPtr context, int identity);

    private delegate void SetMouseCursorPos(int x, int y, bool clipInGuestWindow);

    private delegate void ShowMouseCursor(bool show, bool clippingEnabled);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    private delegate void HandleGuestBootLogs(IntPtr bootLogs, int nLogs);

    private delegate void LogHandler(string msg);

    public struct RAWINPUTDEVICE
    {
      [MarshalAs(UnmanagedType.U2)]
      public ushort usUsagePage;
      [MarshalAs(UnmanagedType.U2)]
      public ushort usUsage;
      [MarshalAs(UnmanagedType.U4)]
      public int dwFlags;
      public IntPtr hwndTarget;
    }

    private delegate void ImapSendImageInfoHandler(IntPtr sceneArray, int nImages);
  }
}
