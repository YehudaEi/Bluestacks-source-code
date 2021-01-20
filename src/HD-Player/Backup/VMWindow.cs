// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.VMWindow
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using BlueStacks.Common;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using SystemInfo;

namespace BlueStacks.Player
{
  public class VMWindow : WMTouchForm
  {
    internal static VMWindow Instance = (VMWindow) null;
    internal static int sLastTouchTime = 0;
    private static Dictionary<Keys, int> sKeyStateSet = new Dictionary<Keys, int>();
    internal static bool isUsePcImeWorkflow = true;
    internal static bool sIsWpfTextboxEnabled = false;
    internal static bool isLogWndProc = false;
    internal Dictionary<string, string> AppOrientationDict = new Dictionary<string, string>();
    internal string mLlastPackageName = "";
    internal Dictionary<string, string> mMacroShortcutsDict = new Dictionary<string, string>();
    internal List<string> mCustomCursorAppsList = new List<string>();
    private VMWindow.InputHandlingState mInputHandlingState = VMWindow.InputHandlingState.IMAP_STATE_NOFOCUS;
    private bool mSendBootCheckStats = true;
    internal System.Windows.Forms.ToolTip sKeyMapToolTip = new System.Windows.Forms.ToolTip()
    {
      ForeColor = System.Drawing.Color.White,
      BackColor = System.Drawing.Color.Black
    };
    private string mInstallDir = string.Empty;
    private Monitor.TouchPoint[] touchPoints = new Monitor.TouchPoint[16];
    internal VMWindow.CustomTextBox mDummyInputKeyBoard = new VMWindow.CustomTextBox();
    internal VMWindow.CustomElementHost mCtrlHost = new VMWindow.CustomElementHost();
    internal WpfTextBoxControl mTextBoxControl = new WpfTextBoxControl();
    private double widthRatio = 16.0;
    private double heightRatio = 9.0;
    public double widthDiff = 26.0;
    public double heightDiff = 71.0;
    internal string mLastKeyBoardString = string.Empty;
    private string lastCompositeString = string.Empty;
    internal Dictionary<int, string> mControllerMap = new Dictionary<int, string>();
    internal Toast snapshotErrorToast;
    internal bool snapshotErrorShown;
    internal bool isMouseLocked;
    internal IntPtr m_hImc;
    internal FullScreenToast mFullScreenToast;
    internal FormBorderStyle mFormBorderStyle;
    internal ShortcutConfig mShortcutConfig;
    internal bool IsShownOnce;
    internal bool mLastCallFromAndroid;
    internal int mLlastOrientationFromAndroid;
    private bool mIsFullscreen;
    private int mVMWindowMouseX;
    internal CSysInfo cSysInfo;
    internal bool mIsKBVibrationDllLoaded;
    public bool mIsInImagePickerMode;
    internal bool mIsCustomCursorEnabled;
    private bool mIsChineseSimplifiedLangSelected;
    public bool mIsInScriptMode;
    private long lastLWinTimestamp;
    private const int SPI_GETMOUSESPEED = 112;
    private const int SPI_SETMOUSESPEED = 113;
    internal DateTime mFrontendLaunchTime;
    private Thread displayTimeOutThread;
    internal bool s_KeyMapTeachMode;
    internal bool mIsTextInputBoxInFocus;
    private bool mWasImeEnabled;
    internal bool isStreamingModeEnabled;
    private DateTime mLastFrontendStatusUpdateTime;
    private Keys mLastSentKeyToClient;
    private const int WM_SIZING = 532;
    private const int WMSZ_LEFT = 1;
    private const int WMSZ_RIGHT = 2;
    private const int WMSZ_TOP = 3;
    private const int WMSZ_BOTTOM = 6;
    private bool isKeyDown;
    private SerialWorkQueue mSerialQueue;
    private bool mIsSideBarVisible;
    private bool mIsTopBarVisible;
    private IContainer components;

    public bool IsFullscreen
    {
      get
      {
        return this.mIsFullscreen;
      }
      set
      {
        this.mIsFullscreen = value;
        this.FullScreenStateChanged(this.mIsFullscreen);
      }
    }

    private void FullScreenStateChanged(bool mIsFullscreen)
    {
      try
      {
        UIHelper.RunOnUIThread((System.Windows.Forms.Control) VMWindow.Instance, (UIHelper.Action) (() =>
        {
          if (mIsFullscreen)
          {
            if (RegistryManager.Instance.Guest[MultiInstanceStrings.VmName].ShowSidebarInFullScreen)
            {
              this.MouseMove -= new System.Windows.Forms.MouseEventHandler(this.VMWindow_MouseMove);
              this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.VMWindow_MouseMove);
            }
            InputMapper.EnableScrollOnEdgeFeature(true);
          }
          else
          {
            this.MouseMove -= new System.Windows.Forms.MouseEventHandler(this.VMWindow_MouseMove);
            InputMapper.EnableScrollOnEdgeFeature(false);
          }
        }));
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in FullScreenStateChanged: " + ex.ToString());
      }
    }

    internal void ToggleScrollOnEdgeState(bool isEnable)
    {
      if (isEnable || this.mIsFullscreen)
        InputMapper.EnableScrollOnEdgeFeature(true);
      else
        InputMapper.EnableScrollOnEdgeFeature(false);
    }

    internal void ToggleMouseLockedState(bool isLocked)
    {
      this.isMouseLocked = isLocked;
    }

    internal void ClientScriptModeStateChanged(bool isInScriptMode)
    {
      UIHelper.RunOnUIThread((System.Windows.Forms.Control) VMWindow.Instance, (UIHelper.Action) (() =>
      {
        this.mIsInScriptMode = isInScriptMode;
        if (isInScriptMode)
        {
          this.MouseUp -= new System.Windows.Forms.MouseEventHandler(this.VMWindow_MouseUp);
          this.MouseLeave -= new EventHandler(this.VMWindow_MouseLeave);
          this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.VMWindow_MouseUp);
          this.MouseLeave += new EventHandler(this.VMWindow_MouseLeave);
        }
        else
        {
          this.MouseUp -= new System.Windows.Forms.MouseEventHandler(this.VMWindow_MouseUp);
          this.MouseLeave -= new EventHandler(this.VMWindow_MouseLeave);
        }
      }));
    }

    internal void ImagePickerModeStateChanged(bool isInImagePickerMode)
    {
      UIHelper.RunOnUIThread((System.Windows.Forms.Control) VMWindow.Instance, (UIHelper.Action) (() =>
      {
        this.mIsInImagePickerMode = isInImagePickerMode;
        if (isInImagePickerMode)
        {
          this.MouseUp -= new System.Windows.Forms.MouseEventHandler(this.VMWindow_MouseUp);
          this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.VMWindow_MouseUp);
        }
        else
          this.MouseUp -= new System.Windows.Forms.MouseEventHandler(this.VMWindow_MouseUp);
      }));
    }

    private void VMWindow_MouseLeave(object sender, EventArgs e)
    {
      this.sKeyMapToolTip.Hide((IWin32Window) VMWindow.Instance);
    }

    private void VMWindow_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
    {
      try
      {
        if (e.Button != MouseButtons.Right)
          return;
        double x1;
        double y1;
        InputMapper.Instance.GetMousePercentLocationFromPointToClient(out x1, out y1);
        Dictionary<string, string> data = new Dictionary<string, string>()
        {
          ["X"] = x1.ToString(),
          ["Y"] = y1.ToString()
        };
        if (this.mIsInImagePickerMode)
        {
          HTTPUtils.SendRequestToEngineAsync("sendImagePickerCoordinates", data, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
          this.mIsInImagePickerMode = false;
          this.ImagePickerModeStateChanged(this.mIsInImagePickerMode);
        }
        else
          HTTPUtils.SendRequestToClientAsync("playerScriptModifierClick", data, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in VMWindow_MouseUp: " + ex.ToString());
      }
    }

    internal VMWindow.InputHandlingState InputMapperHandlingState
    {
      get
      {
        return this.mInputHandlingState;
      }
      set
      {
        this.mInputHandlingState = value;
        Logger.Debug("KMP " + value.ToString());
        if ((OperationsSyncManager.mIsBroadcasting || InputMapper.IsMacroPlaying || OperationsSyncManager.mIsReceiving) && value == VMWindow.InputHandlingState.IMAP_STATE_NOFOCUS)
          return;
        InputMapper.SetInputMapperState((int) value);
      }
    }

    internal string InstallDir
    {
      get
      {
        if (string.IsNullOrEmpty(this.mInstallDir))
          this.mInstallDir = RegistryStrings.InstallDir;
        return this.mInstallDir;
      }
    }

    internal bool WasImeEnabled
    {
      get
      {
        return this.mWasImeEnabled;
      }
      set
      {
        Logger.Debug("KMP WasImeEnabled " + value.ToString());
        this.mWasImeEnabled = value;
      }
    }

    public SerialWorkQueue SerialQueue
    {
      get
      {
        if (this.mSerialQueue == null)
        {
          this.mSerialQueue = new SerialWorkQueue();
          this.mSerialQueue.Start();
        }
        return this.mSerialQueue;
      }
    }

    public VMWindow()
    {
      VMWindow.Instance = this;
      this.InitializeComponent();
      this.Click += new EventHandler(this.VMWindow_Click);
    }

    private void VMWindow_Click(object sender, EventArgs e)
    {
      if (!this.IsFullscreen)
        return;
      HTTPUtils.SendRequestToClientAsync("hideTopSidebar", (Dictionary<string, string>) null, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
    }

    private void KeyMapToolTip_Draw(object sender, DrawToolTipEventArgs e)
    {
      e.DrawBackground();
      e.DrawBorder();
      e.DrawText();
    }

    private void AttachToShutdownEvent()
    {
      SystemEvents.SessionEnding -= new SessionEndingEventHandler(this.SystemEvents_SessionEnding);
      SystemEvents.SessionEnding += new SessionEndingEventHandler(this.SystemEvents_SessionEnding);
    }

    private void DetachFromShutdownEvent()
    {
      SystemEvents.SessionEnding -= new SessionEndingEventHandler(this.SystemEvents_SessionEnding);
    }

    private void SystemEvents_SessionEnding(object sender, SessionEndingEventArgs e)
    {
      Logger.Warning("System session ending. Reason: {0}", (object) e.Reason);
      this.CloseWindow();
    }

    public VMWindow(bool hidden, bool useWpfTextbox)
      : this()
    {
      this.AttachToShutdownEvent();
      this.Icon = Utils.GetApplicationIcon();
      this.Text = Oem.Instance.CommonAppTitleText + MultiInstanceStrings.VmName;
      this.ClientSize = LayoutManager.GetConfiguredDisplaySize();
      this.m_hImc = InteropWindow.ImmGetContext(this.Handle);
      if (Oem.Instance.IsFormBorderStyleFixedSingle)
        this.FormBorderStyle = FormBorderStyle.FixedSingle;
      if (this.FormBorderStyle != FormBorderStyle.None)
        this.MaximizeBox = false;
      this.mFormBorderStyle = this.FormBorderStyle;
      LocaleStrings.InitLocalization((string) null, MultiInstanceStrings.VmName, false);
      ServicePointManager.DefaultConnectionLimit = 10;
      Strings.AppTitle = Oem.Instance.CommonAppTitleText;
      this.mFullScreenToast = new FullScreenToast((System.Windows.Forms.Control) this);
      try
      {
        ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(VMWindow.ValidateRemoteCertificate);
        if (RegistryManager.Instance.SystemStats == 0)
          Stats.SendSystemInfoStats(MultiInstanceStrings.VmName);
      }
      catch (Exception ex)
      {
        Logger.Fatal("Exception while setting up VMWindow. Exiting");
        Logger.Fatal(ex.ToString());
        Environment.Exit(-4);
      }
      this.mFrontendLaunchTime = DateTime.Now;
      this.mLastFrontendStatusUpdateTime = DateTime.Now;
      BlueStacks.Player.Input.DisablePressAndHold(this.Handle);
      try
      {
        BlueStacks.Player.Input.HookKeyboard(new BlueStacks.Player.Input.KeyboardCallback(this.HandleKeyboardHook));
      }
      catch (Exception ex)
      {
        Logger.Error("Exception while keyboard hook. Exception: " + ex.ToString());
      }
      this.StartAgent();
      this.AddDummyTextBox(useWpfTextbox);
      MemoryManager.CheckAndTrimAndroidMemory();
      LayoutManager.InitScreen();
      InputMapper.Instance.SetupSoftControlBar();
      SensorDevice.Instance.StartThreads();
      GpsHelper.Start();
      SystemEvents.DisplaySettingsChanged += new EventHandler(this.HandleDisplaySettingsChanged);
      MemoryManager.TrimMemory(false);
      ThreadPool.QueueUserWorkItem((WaitCallback) (obj => this.PrintingGraphicsInfo()));
      if (!hidden)
        this.Show();
      Logger.Info("BOOT_STAGE: Starting Android boot now");
      AndroidBootUp.Start();
      this.mShortcutConfig = ShortcutConfig.LoadShortcutsConfig();
      this.sKeyMapToolTip.Draw += new DrawToolTipEventHandler(this.KeyMapToolTip_Draw);
      if (!SystemUtils.IsAdministrator() || !Oem.Instance.OEM.Equals("msi2", StringComparison.OrdinalIgnoreCase) && !Oem.Instance.OEM.Equals("msi64_hyperv", StringComparison.OrdinalIgnoreCase))
        return;
      this.mIsKBVibrationDllLoaded = MsiVibration.Init();
      if (this.mIsKBVibrationDllLoaded)
        Logger.Info("Msi vibration dll loaded successfully");
      else
        Logger.Info("Failed to load msi vibration dll");
    }

    internal bool CheckBlackScreen()
    {
      try
      {
        Logger.Debug("Inside CheckBlackScreen");
        System.Drawing.Size blockRegionSize;
        ref System.Drawing.Size local = ref blockRegionSize;
        System.Drawing.Size clientSize1 = this.ClientSize;
        int width1 = (int) ((double) clientSize1.Width * 0.993);
        clientSize1 = this.ClientSize;
        int height1 = (int) ((double) clientSize1.Height * 0.957);
        local = new System.Drawing.Size(width1, height1);
        double num1 = 0.0035;
        double num2 = 17.0 / 400.0;
        System.Drawing.Size clientSize2 = VMWindow.Instance.ClientSize;
        int width2 = clientSize2.Width;
        clientSize2 = this.ClientSize;
        int height2 = clientSize2.Height;
        Bitmap bitmap = new Bitmap(width2, height2);
        using (Graphics graphics = Graphics.FromImage((System.Drawing.Image) bitmap))
        {
          graphics.CopyFromScreen(new System.Drawing.Point((int) ((double) this.Left + (double) this.Width * num1), (int) ((double) this.Top + (double) this.Height * num2)), System.Drawing.Point.Empty, blockRegionSize);
          for (int x = 0; x < blockRegionSize.Width; ++x)
          {
            for (int y = 0; y < blockRegionSize.Height; ++y)
            {
              System.Drawing.Color pixel = bitmap.GetPixel(x, y);
              if ((int) pixel.A != (int) System.Drawing.Color.Black.A || (int) pixel.R != (int) System.Drawing.Color.Black.R || ((int) pixel.G != (int) System.Drawing.Color.Black.G || (int) pixel.B != (int) System.Drawing.Color.Black.B))
              {
                Logger.Info(string.Format("Pixel {0},{1} is not black", (object) x, (object) y));
                bitmap.Dispose();
                return false;
              }
            }
          }
          bitmap.Dispose();
          Logger.Error("Black Screen Detected");
          return true;
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Exception occured in CheckBlackScreen. Err : {0}", (object) ex.ToString());
        return false;
      }
    }

    private void PrintingGraphicsInfo()
    {
      Logger.Info("In PrintingGraphicsInfo");
      try
      {
        Dictionary<string, string> dictionary = Profile.InfoForGraphicsDriverCheck();
        dictionary.Add("guid", RegistryManager.Instance.UserGuid);
        Logger.Info("data being posted: ");
        foreach (KeyValuePair<string, string> keyValuePair in dictionary)
          Logger.Info("Key: " + keyValuePair.Key + " Value: " + keyValuePair.Value);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in print graphics info... Err : " + ex.ToString());
      }
    }

    internal void BootUpTasks()
    {
      UIHelper.RunOnUIThread((System.Windows.Forms.Control) VMWindow.Instance, (UIHelper.Action) (() =>
      {
        this.OnLayout(new LayoutEventArgs((System.Windows.Forms.Control) this, ""));
        this.displayTimeOutThread = new Thread((ThreadStart) (() =>
        {
          while (true)
          {
            Thread.Sleep(35);
            UIHelper.RunOnUIThread((System.Windows.Forms.Control) VMWindow.Instance, (UIHelper.Action) (() => this.HandleDisplayTimeout()));
          }
        }))
        {
          IsBackground = true
        };
        this.displayTimeOutThread.Start();
        this.MouseMove += new System.Windows.Forms.MouseEventHandler(InputMapper.Instance.HandleMouseMove);
        this.MouseDown += new System.Windows.Forms.MouseEventHandler(InputMapper.Instance.HandleMouseDown);
        this.MouseUp += new System.Windows.Forms.MouseEventHandler(InputMapper.Instance.HandleMouseUp);
        this.MouseWheel += new System.Windows.Forms.MouseEventHandler(InputMapper.Instance.HandleMouseWheel);
        this.TouchEvent += new EventHandler<WMTouchForm.WMTouchEventArgs>(this.HandleTouchEvent);
        this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.HandleKeyDown);
        this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.HandleKeyUp);
        this.SendLockedKeys();
      }));
    }

    private void HandleTouchEvent(object sender, WMTouchForm.WMTouchEventArgs e)
    {
      Logger.Info("In HandleTouchEvent in VMWindow");
      VMWindow.sLastTouchTime = Environment.TickCount;
      InputMapper.Instance.HandleTouchEvent(sender, e);
    }

    private void HandleDisplayTimeout()
    {
      switch (Opengl.glWindowAction)
      {
        case GlWindowAction.Show:
          Logger.Info("Showing subwindow");
          if (!Opengl.ShowSubWindow())
            break;
          Opengl.glWindowAction = GlWindowAction.None;
          break;
        case GlWindowAction.Hide:
          Logger.Info("Hiding subwindow");
          if (!Opengl.HideSubWindow())
            break;
          Opengl.glWindowAction = GlWindowAction.None;
          break;
      }
    }

    private void SendLockedKeys()
    {
      new Thread((ThreadStart) (() =>
      {
        Logger.Info("in sendLockedKeys -- mCapsLocked - {0} and mNumLocked - {1}", (object) System.Windows.Forms.Control.IsKeyLocked(Keys.Capital), (object) System.Windows.Forms.Control.IsKeyLocked(Keys.NumLock));
        Logger.Info("in sendLockedKeys - guest booted");
        if (System.Windows.Forms.Control.IsKeyLocked(Keys.Capital))
        {
          this.HandleKeyEvent(Keys.Capital, true);
          Logger.Info("in sendLockedKeys - sleeping for 100 ms for capslock toggle");
          Thread.Sleep(100);
          this.HandleKeyEvent(Keys.Capital, false);
        }
        if (!System.Windows.Forms.Control.IsKeyLocked(Keys.NumLock))
          return;
        this.HandleKeyEvent(Keys.NumLock, true);
        Logger.Info("in sendLockedKeys - sleeping for 100 ms for numLock toggle");
        Thread.Sleep(100);
        this.HandleKeyEvent(Keys.NumLock, false);
      }))
      {
        IsBackground = true
      }.Start();
    }

    private void HandleDisplaySettingsChanged(object sender, EventArgs e)
    {
      Logger.Info("HandleDisplaySettingsChanged()");
      this.SendOrientationToGuest();
    }

    internal void SendOrientationToGuest()
    {
      Logger.Info("Sending screen orientation to guest: {0}", (object) SystemInformation.ScreenOrientation);
      HTTPUtils.SendRequestToGuestAsync("hostOrientation", new Dictionary<string, string>()
      {
        {
          "data",
          SystemInformation.ScreenOrientation.ToString()
        }
      }, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
    }

    public void UpdateUserActivityStatus()
    {
      try
      {
        if (TimeSpan.Compare(DateTime.Now.Subtract(this.mLastFrontendStatusUpdateTime), new TimeSpan(0, 0, 10)) <= 0)
          return;
        Stats.SendFrontendStatusUpdate("user-active", MultiInstanceStrings.VmName);
        this.mLastFrontendStatusUpdateTime = DateTime.Now;
      }
      catch (Exception ex)
      {
        Logger.Error(string.Format("Error Occured, Err : {0}", (object) ex.ToString()));
      }
    }

    private void AddDummyTextBox(bool useWpfTextbox)
    {
      if (useWpfTextbox)
      {
        this.mCtrlHost.Dock = DockStyle.None;
        this.mCtrlHost.Location = new System.Drawing.Point(-500, -500);
        this.mCtrlHost.Size = new System.Drawing.Size(300, 300);
        this.Controls.Add((System.Windows.Forms.Control) this.mCtrlHost);
        this.mCtrlHost.Child = (UIElement) this.mTextBoxControl;
        this.mTextBoxControl.mWpfTextBox.PreviewKeyDown += (System.Windows.Input.KeyEventHandler) ((sender, e) => this.DummyInputKeyBoard_KeyDown(sender, e.ToWinforms()));
        this.mTextBoxControl.mWpfTextBox.PreviewKeyUp += (System.Windows.Input.KeyEventHandler) ((sender, e) => this.DummyInputKeyBoard_KeyUp(sender, e.ToWinforms()));
        this.mTextBoxControl.mWpfTextBox.TextChanged += new TextChangedEventHandler(this.DummyInputKeyBoard_TextChanged);
        VMWindow.sIsWpfTextboxEnabled = true;
      }
      else
      {
        this.mDummyInputKeyBoard.Multiline = true;
        this.mDummyInputKeyBoard.TabStop = false;
        this.mDummyInputKeyBoard.AcceptsTab = true;
        this.mDummyInputKeyBoard.Size = new System.Drawing.Size(150, 100);
        this.mDummyInputKeyBoard.Location = new System.Drawing.Point(-500, -500);
        this.mDummyInputKeyBoard.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DummyInputKeyBoard_KeyDown);
        this.mDummyInputKeyBoard.KeyUp += new System.Windows.Forms.KeyEventHandler(this.DummyInputKeyBoard_KeyUp);
        this.Controls.Add((System.Windows.Forms.Control) this.mDummyInputKeyBoard);
      }
    }

    private void DummyInputKeyBoard_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
    {
      this.isKeyDown = true;
      int length = this.mDummyInputKeyBoard.Text.Length;
      int selectionLength = this.mDummyInputKeyBoard.SelectionLength;
      int selectionStart = this.mDummyInputKeyBoard.SelectionStart;
      if (VMWindow.sIsWpfTextboxEnabled)
      {
        length = this.mTextBoxControl.mWpfTextBox.Text.Length;
        selectionLength = this.mTextBoxControl.mWpfTextBox.SelectionLength;
        selectionStart = this.mTextBoxControl.mWpfTextBox.SelectionStart;
      }
      if (e.KeyData == (Keys.V | Keys.Control) && VMWindow.isUsePcImeWorkflow)
      {
        e.Handled = true;
        e.SuppressKeyPress = true;
      }
      Keys keyData = e.KeyData;
      if (keyData == Keys.Tab || keyData == (Keys.Tab | Keys.Shift) || keyData == (Keys.Tab | Keys.Shift))
      {
        e.Handled = true;
        e.SuppressKeyPress = true;
      }
      if (this.mIsTextInputBoxInFocus && VMWindow.isUsePcImeWorkflow)
      {
        if (keyData == Keys.Back && length != 0 && (selectionStart == length && selectionLength == 0) && this.mIsTextInputBoxInFocus)
          return;
        if (keyData == Keys.Return || keyData == (Keys.Return | Keys.Shift) || keyData == (Keys.Return | Keys.Control))
        {
          if (!this.mIsChineseSimplifiedLangSelected)
            return;
          this.CleanUpTextBox();
          return;
        }
        if (keyData == Keys.Back)
          this.CleanUpTextBox();
        else if (selectionStart != length && (selectionLength != 1 || selectionStart != length - 1))
          this.CleanUpTextBox();
      }
      this.HandleKeyDown((object) null, e);
    }

    private void DummyInputKeyBoard_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
    {
      if (this.isKeyDown && !this.mIsChineseSimplifiedLangSelected && this.mDummyInputKeyBoard.Text.Equals(this.mLastKeyBoardString))
        this.CheckForImeString(this.mLastKeyBoardString);
      if (e.KeyData == (Keys.V | Keys.Control) && VMWindow.isUsePcImeWorkflow)
        return;
      this.HandleKeyUp((object) null, e);
    }

    private void CleanUpTextBox()
    {
      if (VMWindow.sIsWpfTextboxEnabled)
      {
        this.mTextBoxControl.mWpfTextBox.TextChanged -= new TextChangedEventHandler(this.DummyInputKeyBoard_TextChanged);
        this.mTextBoxControl.mWpfTextBox.Text = string.Empty;
        this.mTextBoxControl.mWpfTextBox.TextChanged += new TextChangedEventHandler(this.DummyInputKeyBoard_TextChanged);
      }
      else
      {
        this.mDummyInputKeyBoard.TextChanged -= new EventHandler(this.DummyInputKeyBoard_TextChanged);
        this.mDummyInputKeyBoard.Text = string.Empty;
        this.mDummyInputKeyBoard.TextChanged += new EventHandler(this.DummyInputKeyBoard_TextChanged);
      }
      this.mLastKeyBoardString = string.Empty;
    }

    private void DummyInputKeyBoard_TextChanged(object sender, EventArgs e)
    {
      this.isKeyDown = false;
      string lastKeyBoardString = this.mLastKeyBoardString;
      string c1 = !VMWindow.sIsWpfTextboxEnabled ? this.mDummyInputKeyBoard.Text : this.mTextBoxControl.mWpfTextBox.Text;
      int length = lastKeyBoardString.Length;
      if (lastKeyBoardString.Length > c1.Length)
        length = c1.Length;
      int startIndex = 0;
      while (startIndex < length && (int) lastKeyBoardString[startIndex] == (int) c1[startIndex])
        ++startIndex;
      string c2 = c1.Substring(startIndex);
      if (startIndex == length && c1.Length > lastKeyBoardString.Length)
      {
        if (c2 == Environment.NewLine)
          this.SendStringToAndroid(string.Empty, 0, 1);
        else if (string.IsNullOrEmpty(lastKeyBoardString))
          this.SendStringToAndroid(c1, 0, 0);
        else
          this.SendStringToAndroid(c1.Substring(length), 0, 0);
      }
      else
      {
        int passCharWithBackSpace = lastKeyBoardString.Length - startIndex;
        if (lastKeyBoardString.Substring(startIndex) == Environment.NewLine)
          passCharWithBackSpace = 1;
        if (passCharWithBackSpace > 0 || !string.IsNullOrEmpty(c2))
          this.SendStringToAndroid(c2, passCharWithBackSpace, 0);
      }
      this.mLastKeyBoardString = c1;
    }

    private void SendStringToAndroid(string c, int passCharWithBackSpace, int passNewLine = 0)
    {
      if (!this.mIsTextInputBoxInFocus)
        return;
      if (!VMWindow.isUsePcImeWorkflow)
        return;
      try
      {
        string formattedString = "start_" + c + "_end" + " del=" + (passCharWithBackSpace + this.lastCompositeString.Length).ToString() + " enter=" + passNewLine.ToString();
        this.lastCompositeString = string.Empty;
        Logger.Debug("the inputcharstring is through textbox");
        this.SerialQueue.Enqueue((SerialWorkQueue.Work) (() =>
        {
          try
          {
            InputMapper.Instance.SendAndroidString(formattedString);
          }
          catch
          {
          }
        }));
      }
      catch (Exception ex)
      {
        Logger.Error("Exception when sending char to android. Err : {0}", (object) ex.ToString());
      }
    }

    private void CheckForImeString(string str = "")
    {
      string c = InteropWindow.CurrentCompStr(this.mDummyInputKeyBoard.Handle);
      if (string.IsNullOrEmpty(c) || str.Equals(c))
        return;
      this.SendStringToAndroid(c, 0, 0);
      this.lastCompositeString = c;
    }

    public void HandleKeyDown(object obj, System.Windows.Forms.KeyEventArgs evt)
    {
      bool flag = false;
      if (Oem.Instance.IsOEMWithBGPClient)
        flag = this.HandleClientHotKeys(evt);
      this.UpdateUserActivityStatus();
      if (flag)
      {
        evt.Handled = true;
      }
      else
      {
        MacroForm.RecordKeys(evt.KeyCode, ActionType.KeyDown);
        if (evt.Alt && evt.Control)
        {
          if (this.mMacroShortcutsDict.ContainsKey(evt.KeyCode.ToString()))
          {
            Logger.Debug("MACRO: Shortcut pressed: " + evt.KeyCode.ToString());
            return;
          }
          if (evt.KeyCode == Keys.V || evt.KeyCode == Keys.O || (evt.KeyCode == Keys.G || evt.KeyCode == Keys.T) || evt.KeyCode == Keys.F)
            Opengl.HandleCommand((int) Keyboard.Instance.NativeToScanCodes(evt.KeyCode));
          else if (evt.KeyCode == Keys.K)
          {
            this.s_KeyMapTeachMode = !this.s_KeyMapTeachMode;
          }
          else
          {
            if (evt.KeyCode == Keys.I && Features.IsFeatureEnabled(16777216UL))
            {
              InputMapper.Instance.ShowConfigDialog();
              return;
            }
            if (evt.KeyCode == Keys.M && Features.IsFeatureEnabled(4294967296UL))
            {
              InputMapper.Instance.LaunchBlueStacksKeyMapper();
              return;
            }
          }
        }
        if (this.IgnoreKey(evt) || !this.LockdownIsKeyAllowed(evt.KeyCode))
          return;
        if (AppHandler.mCurrentAppPackage == null)
          Logger.Info("current app package name is null");
        if (evt.KeyCode == Keys.F11 && Features.IsFullScreenToggleEnabled() && !Oem.Instance.IsOEMWithBGPClient)
          LayoutManager.ToggleFullScreen();
        if (evt.KeyCode == Keys.Escape)
        {
          if (AppHandler.mCurrentAppPackage.Equals("com.uncube.account"))
            return;
          if (this.IsFullscreen && RegistryManager.Instance.UseEscapeToExitFullScreen)
            this.SendHotKeyEventToClient("RestoreWindow");
        }
        this.HandleKeyEvent(evt.KeyCode, true);
        if (evt.KeyCode == Keys.Capital)
          Logger.Info("caps lock pressed while in frontend");
        if (evt.KeyCode != Keys.NumLock)
          return;
        Logger.Info("numlock pressed while in frontend");
      }
    }

    private bool HandleClientHotKeys(System.Windows.Forms.KeyEventArgs evt)
    {
      bool flag = false;
      if (this.mLastSentKeyToClient == evt.KeyCode)
        return false;
      this.mLastSentKeyToClient = evt.KeyCode;
      string str = string.Empty;
      if (evt.KeyCode != Keys.None)
      {
        Key key = KeyInterop.KeyFromVirtualKey((int) evt.KeyCode);
        if (evt.Control)
          str = IMAPKeys.GetStringForFile(Key.LeftCtrl) + " + ";
        if (evt.Alt)
          str = str + IMAPKeys.GetStringForFile(Key.LeftAlt) + " + ";
        if (evt.Shift)
          str = str + IMAPKeys.GetStringForFile(Key.LeftShift) + " + ";
        str += IMAPKeys.GetStringForFile(key);
      }
      Logger.Debug("SHORTCUT: KeyPressed.." + str);
      if (this.mShortcutConfig != null)
      {
        foreach (ShortcutKeys shortcutKeys in this.mShortcutConfig.Shortcut)
        {
          if (shortcutKeys.ShortcutKey.Equals(str) && (shortcutKeys.ShortcutName != "STRING_UNLOCK_MOUSE" || shortcutKeys.ShortcutName == "STRING_UNLOCK_MOUSE" && this.isMouseLocked))
          {
            flag = true;
            this.SendHotKeyEventToClient(shortcutKeys.ShortcutName);
            if (shortcutKeys.ShortcutName.Equals("STRING_UPDATED_FULLSCREEN_BUTTON_TOOLTIP"))
            {
              if (this.isStreamingModeEnabled)
                LayoutManager.ToggleFullScreen();
              else
                this.IsFullscreen = !this.IsFullscreen;
            }
            Logger.Debug("SHORTCUT: Shortcut Name.." + shortcutKeys.ShortcutName);
          }
        }
      }
      return flag;
    }

    private void SendHotKeyEventToClient(string clientShortcut)
    {
      HTTPUtils.SendRequestToClientAsync("hotKeyEvents", new Dictionary<string, string>()
      {
        {
          "keyevent",
          clientShortcut.ToString()
        }
      }, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
    }

    private void HandleKeyUp(object obj, System.Windows.Forms.KeyEventArgs evt)
    {
      if ((evt.KeyCode == Keys.Snapshot || evt.KeyCode == Keys.O) && (Oem.Instance.IsOEMWithBGPClient && this.HandleClientHotKeys(evt)))
        return;
      this.mLastSentKeyToClient = Keys.None;
      this.UpdateUserActivityStatus();
      MacroForm.RecordKeys(evt.KeyCode, ActionType.KeyUp);
      if (this.IgnoreKey(evt))
        return;
      this.HandleKeyEvent(evt.KeyCode, false);
    }

    private bool LockdownIsKeyAllowed(Keys key)
    {
      return !Keyboard.Instance.IsAltDepressed() || key != Keys.Left && key != Keys.Right && (key != Keys.F1 && key != Keys.F2) && (key != Keys.F3 && key != Keys.F4);
    }

    private bool IgnoreKey(System.Windows.Forms.KeyEventArgs evt)
    {
      bool flag = false;
      if (evt.KeyCode == Keys.VolumeDown || evt.KeyCode == Keys.VolumeUp || evt.KeyCode == Keys.VolumeMute)
        flag = true;
      return flag;
    }

    private bool IsPrintingKey(Keys key)
    {
      return key >= Keys.A && key <= Keys.Z || key >= Keys.D0 && key <= Keys.D9 || (key >= Keys.NumPad0 && key <= Keys.NumPad9 || key >= Keys.OemSemicolon && key <= Keys.Oemtilde) || (key >= Keys.OemOpenBrackets && key <= Keys.OemQuotes || key >= Keys.Multiply && key <= Keys.Add || (key >= Keys.Subtract && key <= Keys.Divide || key == Keys.Space));
    }

    public void HandleKeyEvent(Keys key, bool pressed)
    {
      if (!AndroidBootUp.isAndroidBooted || this.InputMapperHandlingState == VMWindow.InputHandlingState.IMAP_STATE_NOFOCUS && (key == Keys.LShiftKey || key == Keys.Capital || key == Keys.NumLock) || InputMapper.IsMapableKey(Keyboard.Instance.NativeToScanCodes(key)) && !this.UpdateKeyState(key, pressed))
        return;
      InputMapper.Instance.DispatchKeyboardEvent((uint) (key & Keys.KeyCode), pressed);
    }

    private bool UpdateKeyState(Keys key, bool pressed)
    {
      bool flag = true;
      if (pressed && !VMWindow.sKeyStateSet.ContainsKey(key))
        VMWindow.sKeyStateSet.Add(key, 1);
      else if (!pressed && VMWindow.sKeyStateSet.ContainsKey(key))
        VMWindow.sKeyStateSet.Remove(key);
      else if (!pressed && !VMWindow.sKeyStateSet.ContainsKey(key))
        flag = false;
      return flag;
    }

    private void VMWindow_Resize(object sender, EventArgs e)
    {
      LayoutManager.FixupGuestDisplay();
    }

    private void StartAgent()
    {
      Logger.Info("Launching agent");
      Process.Start(Path.Combine(RegistryStrings.InstallDir, "HD-Agent.exe"));
    }

    private bool HandleKeyboardHook(bool pressed, uint key)
    {
      try
      {
        if (!this.Focused)
        {
          switch (key)
          {
            case 20:
              this.HandleKeyEvent(Keys.Capital, pressed);
              return true;
            case 144:
              this.HandleKeyEvent(Keys.NumLock, pressed);
              return true;
            case 160:
              this.HandleKeyEvent(Keys.LShiftKey, pressed);
              return true;
          }
        }
        if (!AndroidBootUp.isAndroidBooted || !this.Focused)
          return true;
        if (RegistryManager.Instance.DefaultGuest.GrabKeyboard != 0 && (key == 91U || key == 92U))
        {
          this.lastLWinTimestamp = DateTime.Now.Ticks;
          this.HandleKeyEvent(Keys.LWin, pressed);
          return false;
        }
        switch (key)
        {
          case 68:
            return DateTime.Now.Ticks - this.lastLWinTimestamp >= 1000000L;
          case 166:
            this.HandleKeyEvent(Keys.Escape, pressed);
            return false;
          case 172:
            this.HandleKeyEvent(Keys.F8, pressed);
            return false;
          case (uint) byte.MaxValue:
            this.HandleKeyEvent(Keys.Apps, pressed);
            return false;
          default:
            return true;
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in HandleKeyboardHook. Exception: " + ex.ToString());
      }
      return true;
    }

    internal static bool CheckAndroidFilesIntegrity()
    {
      Logger.Info("BOOT_STAGE: Inside CheckAndroidFilesIntegrity check");
      string bstAndroidDir = RegistryStrings.GetBstAndroidDir(MultiInstanceStrings.VmName);
      string bstManagerDir = RegistryStrings.BstManagerDir;
      string file1 = Path.Combine(bstAndroidDir, "Root.vdi");
      string file2 = Path.Combine(bstAndroidDir, "Fastboot.vdi");
      string file3 = Path.Combine(bstAndroidDir, "Data.vdi");
      string file4 = Path.Combine(bstAndroidDir, string.Format("{0}.bstk", (object) MultiInstanceStrings.VmName));
      string file5 = Path.Combine(bstAndroidDir, string.Format("{0}.bstk-prev", (object) MultiInstanceStrings.VmName));
      string file6 = Path.Combine(bstManagerDir, "BstkGlobal.xml");
      try
      {
        if (MultiInstanceStrings.VmName == "Android")
        {
          if (!Utils.IsFileNullOrMissing(file1) && !Utils.IsFileNullOrMissing(file3) && (!Utils.IsFileNullOrMissing(file2) && !Utils.IsFileNullOrMissing(file6)))
          {
            if (Utils.IsFileNullOrMissing(file4))
            {
              if (!Utils.IsFileNullOrMissing(file5))
                goto label_7;
            }
            else
              goto label_7;
          }
          return false;
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in checking AndroidFilesIntegrity. Err : " + ex.ToString());
      }
label_7:
      return true;
    }

    private static bool ValidateRemoteCertificate(
      object sender,
      X509Certificate cert,
      X509Chain chain,
      SslPolicyErrors policyErrors)
    {
      return true;
    }

    private void VMWindow_Activated(object sender, EventArgs e)
    {
      if (Oem.Instance.IsOEMWithBGPClient && !this.isStreamingModeEnabled)
        return;
      Logger.Debug("KMP VMWindow_Activated HandleFrontendActivated");
      this.HandleFrontendActivated();
    }

    private void VMWindow_Deactivate(object sender, EventArgs e)
    {
      if (Oem.Instance.IsOEMWithBGPClient && !this.isStreamingModeEnabled)
        return;
      Logger.Debug("KMP VMWindow_Deactivate HandleFrontendDeactivated");
      this.HandleFrontendDeactivated();
    }

    private void VMWindow_GotFocus(object sender, EventArgs e)
    {
      if (Oem.Instance.IsOEMWithBGPClient && !this.isStreamingModeEnabled)
        return;
      Logger.Debug("KMP VMWindow_GotFocus HandleFrontendActivated");
      this.HandleFrontendActivated();
    }

    private void VMWindow_FormClosing(object sender, FormClosingEventArgs e)
    {
      if (this.isStreamingModeEnabled)
      {
        e.Cancel = true;
        HTTPUtils.SendRequestToClientAsync("toggleStreamingMode", new Dictionary<string, string>()
        {
          {
            "state",
            "false"
          }
        }, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
      }
      else
      {
        if (this.mIsKBVibrationDllLoaded)
          MsiVibration.Release();
        Stats.SendFrontendStatusUpdate("frontend-closed", MultiInstanceStrings.VmName);
        this.CloseWindow();
      }
    }

    private void VMWindow_InputLanguageChanged(object sender, System.Windows.Forms.InputLanguageChangedEventArgs e)
    {
      this.FrontendInputLanguageChanged(e.InputLanguage.Culture.Name);
    }

    private void FrontendInputLanguageChanged(string inputLang)
    {
      Logger.Info("The inputlanguage changed to : " + inputLang);
      try
      {
        this.mIsChineseSimplifiedLangSelected = InputLanguageManager.Current.CurrentInputLanguage.KeyboardLayoutId == 2052;
      }
      catch (Exception ex)
      {
        Logger.Error("Error in getting current input language.." + ex?.ToString());
      }
      if (this.WasImeEnabled)
      {
        if (inputLang.Equals("zh-tw", StringComparison.InvariantCultureIgnoreCase))
        {
          this.mDummyInputKeyBoard.Location = this.PointToClient(new System.Drawing.Point(-500, -500));
          this.mDummyInputKeyBoard.Enabled = true;
        }
        else
        {
          this.mDummyInputKeyBoard.Location = this.PointToClient(new System.Drawing.Point(System.Windows.Forms.Cursor.Position.X + 20, System.Windows.Forms.Cursor.Position.Y + 20));
          this.mDummyInputKeyBoard.Enabled = true;
        }
      }
      this.SetKeyboardLayout(inputLang);
    }

    private void VMWindow_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
    {
      try
      {
        if (Oem.IsOEMDmm)
        {
          if (e.Y > 40)
            return;
          HTTPUtils.SendRequestToClientAsync("showFullscreenTopbar", new Dictionary<string, string>()
          {
            {
              "visible",
              "true"
            }
          }, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
        }
        else
        {
          if (e.X >= this.Width - 40)
          {
            HTTPUtils.SendRequestToClientAsync("showFullscreenSidebarButton", new Dictionary<string, string>()
            {
              {
                "visible",
                "true"
              }
            }, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
            this.mIsSideBarVisible = true;
          }
          else if (this.mIsSideBarVisible)
          {
            HTTPUtils.SendRequestToClientAsync("showFullscreenSidebarButton", new Dictionary<string, string>()
            {
              {
                "visible",
                "false"
              }
            }, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
            this.mIsSideBarVisible = false;
          }
          if (e.Y <= 40)
          {
            HTTPUtils.SendRequestToClientAsync("showFullscreenTopbarButton", new Dictionary<string, string>()
            {
              {
                "visible",
                "true"
              }
            }, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
            this.mIsTopBarVisible = true;
          }
          else
          {
            if (!this.mIsTopBarVisible)
              return;
            HTTPUtils.SendRequestToClientAsync("showFullscreenTopbarButton", new Dictionary<string, string>()
            {
              {
                "visible",
                "false"
              }
            }, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
            this.mIsTopBarVisible = false;
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Fatal(ex.ToString());
      }
    }

    public void SetKeyboardLayout(string keyboardLayout)
    {
      new Thread((ThreadStart) (() =>
      {
        try
        {
          string path = "setkeyboardlayout";
          Dictionary<string, string> data = new Dictionary<string, string>()
          {
            {
              "keyboardlayout",
              keyboardLayout
            }
          };
          Logger.Info("Sending request for " + path + " with data : ");
          foreach (KeyValuePair<string, string> keyValuePair in data)
            Logger.Info("key : " + keyValuePair.Key + " value : " + keyValuePair.Value);
          string str = VmCmdHandler.SendRequest(path, data, MultiInstanceStrings.VmName, out JObject _, "bgp");
          if (str == null || str.Contains("error"))
          {
            Logger.Error("Failed to set keyboard layout in syn config...checking current IME");
            if (Utils.IsLatinImeSelected(MultiInstanceStrings.VmName))
            {
              this.SetPcImeWorkflow(true);
            }
            else
            {
              if (!Oem.Instance.IsSendGameManagerRequest)
                return;
              HTTPUtils.SendRequestToClient("showIMESwitchPrompt", (Dictionary<string, string>) null, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
            }
          }
          else
          {
            if (Utils.IsLatinImeSelected(MultiInstanceStrings.VmName))
              return;
            this.SetPcImeWorkflow(false);
          }
        }
        catch (Exception ex)
        {
          Logger.Error("Exception in set keyboard layout... Err : " + ex.ToString());
        }
      }))
      {
        IsBackground = true
      }.Start();
    }

    public void SetPcImeWorkflow(bool isSet)
    {
      VMWindow.isUsePcImeWorkflow = isSet;
    }

    protected override void WndProc(ref Message m)
    {
      if (VMWindow.isLogWndProc)
        Logger.Info("WndProcMessage: " + m.Msg.ToString() + "~~" + m.WParam.ToString() + "~~" + m.LParam.ToString() + "~~");
      bool flag = false;
      switch (m.Msg)
      {
        case 74:
          COPYGAMEPADDATASTRUCT structure1 = (COPYGAMEPADDATASTRUCT) Marshal.PtrToStructure(m.LParam, typeof (COPYGAMEPADDATASTRUCT));
          byte[] destination = new byte[structure1.size];
          Marshal.Copy(structure1.lpData, destination, 0, destination.Length);
          int[] numArray = new int[destination.Length / 4];
          try
          {
            for (int startIndex = 0; startIndex < destination.Length; startIndex += 4)
              numArray[startIndex / 4] = BitConverter.ToInt32(destination, startIndex);
          }
          catch
          {
          }
          switch (m.WParam.ToInt32())
          {
            case 0:
            case 1:
              break;
            case 2:
              GamePad gamepad = new GamePad()
              {
                X = numArray[1],
                Y = numArray[2],
                Z = numArray[3],
                Rx = numArray[4],
                Ry = numArray[5],
                Rz = numArray[6],
                Hat = numArray[7],
                Mask = (uint) numArray[8]
              };
              InputMapper.Instance.DispatchGamePadUpdate(numArray[0], gamepad);
              break;
            default:
              Logger.Info("Recieved CopyData wParam: {0}", (object) m.WParam);
              break;
          }
          break;
        case 130:
          Logger.Error("----------------------------> Got WM_NCDESTROY");
          Stats.SendFrontendStatusUpdate("frontend-closed", MultiInstanceStrings.VmName);
          break;
        case (int) byte.MaxValue:
          InputMapper.Instance.HandleRawInput(m.LParam);
          break;
        case 274:
          Logger.Info("Received message WM_SYSCOMMAND");
          if (!this.HandleWMSysCommand(m.WParam.ToInt32()))
            return;
          break;
        case 532:
          if (!this.isStreamingModeEnabled)
            return;
          Logger.Info("size changed window message.." + m.Msg.ToString());
          this.SetAspectRationAndMinMaxOfForm();
          BlueStacks.Common.RECT structure2 = (BlueStacks.Common.RECT) Marshal.PtrToStructure(m.LParam, typeof (BlueStacks.Common.RECT));
          switch (m.WParam.ToInt32())
          {
            case 1:
            case 2:
              structure2.Bottom = structure2.Top + this.GetHeightFromWidth(this.Width);
              break;
            case 3:
            case 6:
              structure2.Right = structure2.Left + this.GetWidthFromHeight(this.Height);
              break;
            case 4:
              structure2.Left = structure2.Right - this.GetWidthFromHeight(this.Height);
              break;
            case 8:
              structure2.Bottom = structure2.Top + this.GetHeightFromWidth(this.Width);
              break;
          }
          Logger.Info("form width: {0} height : {1}", (object) (structure2.Right - structure2.Left), (object) (structure2.Bottom - structure2.Top));
          Marshal.StructureToPtr((object) structure2, m.LParam, true);
          flag = true;
          break;
        case 1025:
          Logger.Info("Received message WM_USER_SHOW_WINDOW");
          this.HandleUserShowWindow(0, 0);
          break;
        case 1058:
          Logger.Info("Received message WM_USER_SHOW_GUIDANCE");
          VmCmdHandler.RunCommand("controller_guidance_pressed", MultiInstanceStrings.VmName, "bgp");
          break;
        case 1059:
          Logger.Info("Received message WM_USER_AUDIO_MUTE");
          this.HandleFrontendMute();
          break;
        case 1060:
          Logger.Info("Received message WM_USER_AUDIO_UNMUTE");
          this.HandleFrontendUnMute();
          break;
        case 1062:
          Logger.Info("Received message WM_USER_ACTIVATE");
          this.HandleFrontendActivated();
          break;
        case 1063:
          Logger.Info("Received message WM_USER_HIDE_WINDOW");
          this.HandleUserHideWindow();
          break;
        case 1065:
          Logger.Info("Received message WM_USER_DEACTIVATE");
          this.HandleFrontendDeactivated();
          break;
        default:
          flag = false;
          break;
      }
      base.WndProc(ref m);
      if (!flag)
        return;
      try
      {
        m.Result = new IntPtr(1);
      }
      catch (Exception ex)
      {
      }
    }

    public void SetAspectRationAndMinMaxOfForm()
    {
      if (LayoutManager.mEmulatedPortraitMode)
      {
        this.widthRatio = 9.0;
        this.heightRatio = 16.0;
      }
      else
      {
        this.widthRatio = 16.0;
        this.heightRatio = 9.0;
      }
    }

    public static int GetWindowsScaling()
    {
      return (int) ((double) Screen.PrimaryScreen.Bounds.Width / SystemParameters.PrimaryScreenWidth);
    }

    public int GetWidthFromHeight(int height)
    {
      return (int) (this.widthRatio * ((double) height - this.heightDiff) / this.heightRatio) + (int) this.widthDiff;
    }

    public int GetHeightFromWidth(int width)
    {
      return (int) (this.heightRatio * ((double) width - this.widthDiff) / this.widthRatio) + (int) this.heightDiff;
    }

    public void ResizeWindowOnStreamingMode()
    {
      if (!this.isStreamingModeEnabled)
        return;
      this.SetAspectRationAndMinMaxOfForm();
      if (LayoutManager.mEmulatedPortraitMode)
        this.HandleUserShowWindow(this.GetWidthFromHeight(this.Height), this.Height);
      else
        this.HandleUserShowWindow(this.Width, this.GetHeightFromWidth(this.Width));
    }

    private bool HandleWMSysCommand(int command)
    {
      if (command == 61488 || command == 61490 || (command == 61728 || command == 61730))
      {
        Logger.Info("Received MAXIMIZE/RESTORE command");
        if (this.WindowState == FormWindowState.Minimized)
          return true;
      }
      return command != 61696;
    }

    internal void ChangeCursorStyle(string path, bool isMOBACursor = false)
    {
      try
      {
        if (RegistryManager.Instance.CustomCursorEnabled && !string.IsNullOrEmpty(path) && AppHandler.mCurrentAppPackage.Equals("com.supercell.brawlstars", StringComparison.InvariantCultureIgnoreCase))
          path = !isMOBACursor ? Constants.BrawlStarsCustomCursorPath : Constants.BrawlStarsMOBACursorPath;
        if (System.IO.File.Exists(path))
        {
          Logger.Info("CURSOR Changing to custom cursor " + path);
          this.Cursor = InteropWindow.LoadCustomCursor(path);
          if (isMOBACursor)
            return;
          this.mIsCustomCursorEnabled = true;
        }
        else
        {
          Logger.Info("CURSOR Changing to default cursor");
          this.Cursor = System.Windows.Forms.Cursors.Default;
          this.mIsCustomCursorEnabled = false;
        }
      }
      catch (Exception ex)
      {
        Logger.Error("CURSOR Error in changing cusor style: {0}", (object) ex);
        this.Cursor = System.Windows.Forms.Cursors.Default;
      }
    }

    internal void HandleUserHideWindow()
    {
      Logger.Info("UserHideWindow start");
      this.Hide();
      this.WindowState = FormWindowState.Minimized;
      MediaManager.MuteEngine(false);
    }

    internal void HandleUserShowWindow(int width = 0, int height = 0)
    {
      Logger.Info("UserShowWindow start");
      this.ShowVmWindow(width, height);
      this.PostVmWindowShowTasks(false);
    }

    internal void ShowVmWindow(int width, int height)
    {
      if (width != 0)
        this.Width = width;
      if (height != 0)
        this.Height = height;
      this.Show();
      this.IsShownOnce = true;
      this.BringToFront();
      this.WindowState = FormWindowState.Normal;
    }

    internal void PostVmWindowShowTasks(bool isMute = false)
    {
      this.HandleFrontendActivated();
      if (MediaManager.mIsMutedExplicitly | isMute)
      {
        Logger.Info("Not unmuting Engine, since it is muted explicitly");
      }
      else
      {
        Logger.Info("Unmuting Engine");
        MediaManager.UnmuteEngine();
      }
    }

    internal bool IsFrontendReparented()
    {
      return !(InteropWindow.GetParent(this.Handle) == IntPtr.Zero);
    }

    internal void HandleFrontendActivated()
    {
      Logger.Debug("KMP HandleFrontendActivated");
      if (!InputMapper.mIsVMWindowsActivated)
        Stats.SendFrontendStatusUpdate("frontend-activated", MultiInstanceStrings.VmName);
      InputMapper.mIsVMWindowsActivated = true;
      this.Focus();
      LayoutManager.FixupGuestDisplay();
      if (this.WasImeEnabled)
        this.ChangeImeMode(true, false);
      if (this.mIsTextInputBoxInFocus)
      {
        Logger.Debug("KMP InputMapperHandlingState = InputHandlingState.IMAP_STATE_TEXT");
        this.InputMapperHandlingState = VMWindow.InputHandlingState.IMAP_STATE_TEXT;
        if (VMWindow.sIsWpfTextboxEnabled)
        {
          this.mCtrlHost.Focus();
        }
        else
        {
          this.mDummyInputKeyBoard.Focus();
          this.ActiveControl = (System.Windows.Forms.Control) this.mDummyInputKeyBoard;
        }
      }
      else if (InputMapper.s_UserKeyMappingEnabled)
      {
        Logger.Debug("KMP InputMapperHandlingState = InputHandlingState.IMAP_STATE_MAPPING");
        this.InputMapperHandlingState = VMWindow.InputHandlingState.IMAP_STATE_MAPPING;
      }
      else
      {
        Logger.Debug("KMP InputMapperHandlingState = InputHandlingState.IMAP_STATE_RAW");
        this.InputMapperHandlingState = VMWindow.InputHandlingState.IMAP_STATE_RAW;
      }
      if (AndroidBootUp.camManager != null)
        AndroidBootUp.camManager.resumeCamera();
      BstCursor.Instance.RaiseFocusChange();
    }

    internal void HandleFrontendDeactivated()
    {
      Logger.Debug("KMP HandleFrontendDeactivated");
      if (InputMapper.mIsVMWindowsActivated)
        Stats.SendFrontendStatusUpdate("frontend-deactivated", MultiInstanceStrings.VmName);
      InputMapper.mIsVMWindowsActivated = false;
      VMWindow.sKeyStateSet.Clear();
      BstCursor.Instance.RaiseFocusChange();
      this.InputMapperHandlingState = VMWindow.InputHandlingState.IMAP_STATE_NOFOCUS;
      if (!this.WasImeEnabled)
        return;
      this.ChangeImeMode(false, false);
    }

    private void HandleFrontendMute()
    {
      MediaManager.MuteEngine(true);
    }

    private void HandleFrontendUnMute()
    {
      MediaManager.UnmuteEngine();
    }

    internal void CloseWindow()
    {
      try
      {
        Logger.Info("Closing VMWindow");
        this.DetachFromShutdownEvent();
        Utils.KillCurrentOemProcessByName("HD-RunApp", (string) null);
        AndroidService.Instance?.CloseService();
        try
        {
          if (AndroidBootUp.camManager != null)
            AndroidBootUp.camManager.pauseCamera();
          this.StateExitConnected();
        }
        catch (Exception ex)
        {
          Logger.Error("Exception in stateexitconnected. Err : {0}", (object) ex.ToString());
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in closing form. Err : " + ex.ToString());
      }
    }

    private void StateExitConnected()
    {
      Logger.Info("Exiting state Connected");
      Opengl.glWindowAction = GlWindowAction.Hide;
      AndroidBootUp.GpsDetach();
      try
      {
        SensorDevice.Instance.Stop();
      }
      catch (Exception ex)
      {
        Logger.Info(ex.Message);
      }
      try
      {
        AndroidBootUp.CameraDetach();
      }
      catch (Exception ex)
      {
        Logger.Info(ex.Message);
      }
      this.MouseMove -= new System.Windows.Forms.MouseEventHandler(InputMapper.Instance.HandleMouseMove);
      this.MouseDown -= new System.Windows.Forms.MouseEventHandler(InputMapper.Instance.HandleMouseDown);
      this.MouseUp -= new System.Windows.Forms.MouseEventHandler(InputMapper.Instance.HandleMouseUp);
      this.MouseWheel -= new System.Windows.Forms.MouseEventHandler(InputMapper.Instance.HandleMouseWheel);
      this.TouchEvent -= new EventHandler<WMTouchForm.WMTouchEventArgs>(this.HandleTouchEvent);
      this.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.HandleKeyDown);
      this.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.HandleKeyUp);
      try
      {
        this.displayTimeOutThread.Abort();
      }
      catch (Exception ex)
      {
        Logger.Info(ex.Message);
      }
      try
      {
        SystemEvents.DisplaySettingsChanged -= new EventHandler(this.HandleDisplaySettingsChanged);
      }
      catch (Exception ex)
      {
        Logger.Warning("Exception while unwinding DisplaySettingsChangedEvent. " + ex.Message);
      }
      AndroidBootUp.mMonitor.Close();
      AndroidBootUp.mMonitor = (Monitor) null;
      AndroidBootUp.mManager = (Manager) null;
      this.Invalidate();
    }

    public void UpdateMouse()
    {
      UIHelper.RunOnUIThread((System.Windows.Forms.Control) this, (UIHelper.Action) (() =>
      {
        if (!AndroidBootUp.isAndroidBooted)
          return;
        System.Drawing.Point client = this.PointToClient(System.Windows.Forms.Control.MousePosition);
        int x = client.X;
        int y = client.Y;
        Mouse.Instance.UpdateCursor((uint) LayoutManager.GetGuestX(x, y), (uint) LayoutManager.GetGuestY(x, y));
        AndroidBootUp.mMonitor.SendMouseState(Mouse.Instance.X, Mouse.Instance.Y, Mouse.Instance.Mask);
      }));
    }

    private void ClipCursorInGuestWindow()
    {
      System.Drawing.Point screen = this.PointToScreen(new System.Drawing.Point());
      int x = screen.X;
      int y = screen.Y + 15;
      int width = this.Size.Width;
      int height = this.ClientSize.Height - 15;
      Logger.Info("cursor cli: {0} {1} {2} {3} {4}", (object) this.Location.X, (object) x, (object) y, (object) width, (object) height);
      System.Windows.Forms.Cursor.Clip = new Rectangle(new System.Drawing.Point(x, y), new System.Drawing.Size(width, height));
    }

    public void SetMouseCursorPos(int cx, int cy, bool clipInGuestWindow)
    {
      ThreadPool.QueueUserWorkItem((WaitCallback) (obj => UIHelper.RunOnUIThread((System.Windows.Forms.Control) this, (UIHelper.Action) (() =>
      {
        System.Windows.Forms.Cursor.Position = this.PointToScreen(new System.Drawing.Point(cx, cy));
        if (clipInGuestWindow)
        {
          this.ClipCursorInGuestWindow();
        }
        else
        {
          Logger.Info("CURSOR clipping removed");
          System.Windows.Forms.Cursor.Clip = new Rectangle();
        }
      }))));
    }

    public void ShowMouseCursor(bool show, bool clippingEnabled = true)
    {
      Logger.Info(string.Format("CURSOR ShowMouseCursor called {0}", (object) show));
      ThreadPool.QueueUserWorkItem((WaitCallback) (obj => UIHelper.RunOnUIThread((System.Windows.Forms.Control) this, (UIHelper.Action) (() =>
      {
        if (show)
        {
          System.Windows.Forms.Cursor.Show();
          Logger.Info("CURSOR clipping removed and shown: " + System.Windows.Forms.Cursor.Position.ToString());
          if (clippingEnabled)
            System.Windows.Forms.Cursor.Clip = new Rectangle();
        }
        else
        {
          System.Windows.Forms.Cursor.Hide();
          Logger.Info("CURSOR clipping added and hidden" + System.Windows.Forms.Cursor.Position.ToString());
          if (clippingEnabled)
            this.ClipCursorInGuestWindow();
        }
        Logger.Info("CURSOR: Clipping enabled: " + clippingEnabled.ToString());
        HTTPUtils.SendRequestToClientAsync("shootingModeChanged", new Dictionary<string, string>()
        {
          {
            "IsShootingModeActivated",
            (!show).ToString()
          }
        }, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
      }))));
    }

    internal void SaveScreenShot(string path, bool showSaved)
    {
      new Thread((ThreadStart) (() =>
      {
        if (VMWindow.SaveScreenshotBstCommandProcessor(path, showSaved))
          return;
        VMWindow.SaveScreenshotAdb(path, showSaved);
      }))
      {
        IsBackground = true
      }.Start();
    }

    private static bool SaveScreenshotBstCommandProcessor(string path, bool showSaved)
    {
      Logger.Info("In SaveScreenshotBCP");
      try
      {
        string guest = HTTPUtils.SendRequestToGuest("screencap", (Dictionary<string, string>) null, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
        Logger.Info("Response from android: " + guest);
        JObject jobject = JObject.Parse(guest);
        if ((jobject.ContainsKey("response") ? jobject["response"].ToString().Trim() : jobject["result"].ToString().Trim()) == "ok")
        {
          string sourceFileName = Path.Combine(RegistryManager.Instance.Guest[MultiInstanceStrings.VmName].SharedFolder0Path.Trim('\\'), Path.GetFileName(jobject["filepath"].ToString().Trim()));
          string destFileName = Path.Combine(RegistryManager.Instance.ScreenShotsPath, path);
          System.IO.File.Move(sourceFileName, destFileName);
          Logger.Info("Screenshot moved from :" + sourceFileName + " to " + destFileName);
          HTTPUtils.SendRequestToClientAsync("screenshotCaptured", new Dictionary<string, string>()
          {
            {
              nameof (path),
              destFileName
            },
            {
              "showSavedInfo",
              showSaved.ToString()
            }
          }, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
          return true;
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in SaveScreenshotBCP " + ex?.ToString());
      }
      return false;
    }

    private static bool SaveScreenshotAdb(string fileName, bool showSaved)
    {
      try
      {
        Logger.Info("Trying to save screenshot using adb");
        using (AdbCommandRunner adbCommandRunner = new AdbCommandRunner(MultiInstanceStrings.VmName))
        {
          if (adbCommandRunner.Connect(MultiInstanceStrings.VmName))
          {
            string filePath = "/sdcard/DCIM/" + fileName;
            string screenShotsPath = RegistryManager.Instance.ScreenShotsPath;
            if (adbCommandRunner.RunShell(string.Format("screencap -p '{0}'", (object) filePath)))
            {
              if (adbCommandRunner.Pull(filePath, RegistryManager.Instance.ScreenShotsPath) && System.IO.File.Exists(Path.Combine(RegistryManager.Instance.ScreenShotsPath, fileName)))
              {
                HTTPUtils.SendRequestToClientAsync("screenshotCaptured", new Dictionary<string, string>()
                {
                  {
                    "path",
                    Path.Combine(RegistryManager.Instance.ScreenShotsPath, fileName)
                  },
                  {
                    "showSavedInfo",
                    showSaved.ToString()
                  }
                }, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
                return true;
              }
              Logger.Error("Cannot save screenshot.Error in adb pull");
            }
            else
              Logger.Error("Cannot save screenshot.Error in screencap");
          }
          else
            Logger.Error("Cannot connect to guest.  Please make sure BlueStacks is running.");
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in SaveScreenShotAdb: " + ex.ToString());
      }
      return false;
    }

    internal void HandleShareButtonClicked()
    {
      int num1 = (this.Width - this.ClientSize.Width) / 2;
      int num2 = this.Height - this.ClientSize.Height - 2 * num1;
      Bitmap bitmap = new Bitmap(this.Width + 2 * num1, this.Height + 2 * num1 + num2);
      Graphics.FromImage((System.Drawing.Image) bitmap).CopyFromScreen(new System.Drawing.Point(this.Left, this.Top), System.Drawing.Point.Empty, new System.Drawing.Size(this.Width, this.Height));
      string path2 = string.Format("bstSnapshot_{0}.jpg", (object) new Random().Next(0, 100000));
      string str1 = string.Format("final_{0}", (object) path2);
      if (RegistryManager.Instance.DefaultGuest.FileSystem == 0)
      {
        Logger.Info("Shared folders disabled");
      }
      else
      {
        string sharedFolderDir = RegistryStrings.SharedFolderDir;
        string str2 = "BstSharedFolder";
        string str3 = Path.Combine(sharedFolderDir, path2);
        string outputImage = Path.Combine(sharedFolderDir, str1);
        bitmap.Save(str3, ImageFormat.Jpeg);
        try
        {
          Utils.AddUploadTextToImage(str3, outputImage);
          System.IO.File.Delete(str3);
        }
        catch (Exception ex)
        {
          Logger.Error("Exception in adding upload text to snapshot. Err : " + ex.ToString());
          str1 = path2;
        }
        string url = string.Format("http://127.0.0.1:{0}/{1}", (object) MultiInstanceStrings.BstServerPort, (object) "sharepic");
        string str4 = "/mnt/sdcard/windows/" + str2 + "/" + Path.GetFileName(str1);
        Logger.Info("androidPath: " + str4);
        Dictionary<string, string> data = new Dictionary<string, string>()
        {
          {
            "data",
            str4
          }
        };
        Logger.Info("Sending snapshot upload request.");
        string str5 = "";
        try
        {
          str5 = BstHttpClient.Post(url, data, (Dictionary<string, string>) null, false, MultiInstanceStrings.VmName, 0, 1, 0, false, "bgp");
        }
        catch (Exception ex)
        {
          Logger.Error("Exception in sending post request. url = {0}, data = {1}. Err : {2}", (object) url, (object) data, (object) ex.ToString());
        }
        if (!str5.Contains("error") || this.snapshotErrorShown)
          return;
        this.snapshotErrorShown = true;
        if (this.snapshotErrorToast == null)
          this.snapshotErrorToast = new Toast((System.Windows.Forms.Control) this, LocaleStrings.GetLocalizedString("STRING_SNAPSHOT_ERROR_TOAST", ""));
        Animate.AnimateWindow(this.snapshotErrorToast.Handle, 500, 262148);
        this.snapshotErrorToast.Show();
        new Thread((ThreadStart) (() =>
        {
          Thread.Sleep(3000);
          Animate.AnimateWindow(this.snapshotErrorToast.Handle, 500, 327688);
          this.snapshotErrorShown = false;
        }))
        {
          IsBackground = true
        }.Start();
      }
    }

    internal IntPtr GetHandle()
    {
      if (this.Handle == IntPtr.Zero)
        this.CreateHandle();
      return this.Handle;
    }

    private void SendControllerEvent(string name, int identity, string type)
    {
      this.SendControllerEventInternal(string.Format("controller_{0} {1} {2}", (object) name, (object) identity, (object) type), (UIHelper.Action) null);
    }

    private void SendControllerEventInternal(string cmd, UIHelper.Action continuation)
    {
      Logger.Info("Sending controller event " + cmd);
      VmCmdHandler.RunCommandAsync(cmd, continuation, (System.Windows.Forms.Control) this, MultiInstanceStrings.VmName);
    }

    internal void ChangeImeMode(bool enableIme, bool fromAndroid = true)
    {
      Logger.Debug("KMP change enable ime called " + enableIme.ToString() + "," + this.mIsTextInputBoxInFocus.ToString());
      if (fromAndroid)
        HTTPUtils.SendRequestToClientAsync("hideOverlayWhenIMEActive", new Dictionary<string, string>()
        {
          {
            "hideOverlay",
            enableIme.ToString()
          }
        }, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
      if (enableIme && Utils.IsLatinImeSelected(MultiInstanceStrings.VmName))
      {
        Logger.Info("ime mode enabled");
        UIHelper.RunOnUIThread((System.Windows.Forms.Control) VMWindow.Instance, (UIHelper.Action) (() =>
        {
          try
          {
            this.mIsChineseSimplifiedLangSelected = InputLanguageManager.Current.CurrentInputLanguage.KeyboardLayoutId == 2052;
          }
          catch (Exception ex)
          {
            Logger.Error("Error in getting current input language." + ex?.ToString());
          }
          this.mIsTextInputBoxInFocus = true;
          this.WasImeEnabled = true;
          this.InputMapperHandlingState = VMWindow.InputHandlingState.IMAP_STATE_TEXT;
          this.SuspendLayout();
          if (VMWindow.sIsWpfTextboxEnabled)
          {
            this.mCtrlHost.Location = this.PointToClient(new System.Drawing.Point(System.Windows.Forms.Cursor.Position.X + 20, System.Windows.Forms.Cursor.Position.Y + 20));
            this.mTextBoxControl.mWpfTextBox.IsEnabled = true;
          }
          else if (InputLanguageManager.Current.CurrentInputLanguage.Name.Equals("zh-tw", StringComparison.InvariantCultureIgnoreCase))
          {
            this.mDummyInputKeyBoard.Location = this.PointToClient(new System.Drawing.Point(-500, -500));
            this.mDummyInputKeyBoard.Enabled = true;
          }
          else
          {
            this.mDummyInputKeyBoard.Location = this.PointToClient(new System.Drawing.Point(System.Windows.Forms.Cursor.Position.X + 20, System.Windows.Forms.Cursor.Position.Y + 20));
            this.mDummyInputKeyBoard.Enabled = true;
          }
          this.ResumeLayout(false);
          this.PerformLayout();
          this.CleanUpTextBox();
          if (!OperationsSyncManager.mIsReceiving || InputMapper.mIsVMWindowsActivated)
          {
            if (VMWindow.sIsWpfTextboxEnabled)
              this.mCtrlHost.Focus();
            else
              this.mDummyInputKeyBoard.Focus();
          }
          InteropWindow.ImmSetOpenStatus(this.m_hImc, true);
        }));
      }
      else
      {
        Logger.Info("ime mode disabled");
        UIHelper.RunOnUIThread((System.Windows.Forms.Control) VMWindow.Instance, (UIHelper.Action) (() =>
        {
          if (fromAndroid)
          {
            this.WasImeEnabled = false;
            if (this.InputMapperHandlingState != VMWindow.InputHandlingState.IMAP_STATE_NOFOCUS)
              this.InputMapperHandlingState = !InputMapper.s_UserKeyMappingEnabled ? VMWindow.InputHandlingState.IMAP_STATE_RAW : VMWindow.InputHandlingState.IMAP_STATE_MAPPING;
          }
          this.SuspendLayout();
          if (VMWindow.sIsWpfTextboxEnabled)
            this.mTextBoxControl.mWpfTextBox.IsEnabled = false;
          else
            this.mDummyInputKeyBoard.Enabled = false;
          this.ResumeLayout(false);
          this.PerformLayout();
          this.mIsTextInputBoxInFocus = false;
          InteropWindow.ImmSetOpenStatus(this.m_hImc, false);
          if (!fromAndroid || OperationsSyncManager.mIsReceiving && !InputMapper.mIsVMWindowsActivated)
            return;
          this.Focus();
        }));
      }
      InteropWindow.ImmSetOpenStatus(this.m_hImc, this.mIsTextInputBoxInFocus);
    }

    internal bool IsPackageAvailableForCustomCursor(string appPackage)
    {
      foreach (string customCursorApps in this.mCustomCursorAppsList)
      {
        string str = customCursorApps;
        if (customCursorApps.EndsWith("*", StringComparison.InvariantCulture))
          str = customCursorApps.TrimEnd('*');
        if (str.StartsWith("~", StringComparison.InvariantCulture))
        {
          if (appPackage.StartsWith(str.Substring(1), StringComparison.InvariantCulture))
            return false;
        }
        else if (appPackage.StartsWith(str, StringComparison.InvariantCulture))
          return true;
      }
      return false;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.components != null)
        this.components.Dispose();
      base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
      this.SuspendLayout();
      this.AutoScaleDimensions = new SizeF(6f, 13f);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.BackColor = System.Drawing.Color.Black;
      this.ClientSize = new System.Drawing.Size(1053, 675);
      this.DoubleBuffered = true;
      this.ForeColor = System.Drawing.Color.LightGray;
      this.Name = nameof (VMWindow);
      this.Text = nameof (VMWindow);
      this.AllowDrop = true;
      if (Oem.Instance.IsDragDropEnabled)
      {
        this.DragEnter += new System.Windows.Forms.DragEventHandler(FileImporter.HandleDragEnter);
        this.DragDrop += FileImporter.MakeDragDropHandler();
      }
      this.Resize += new EventHandler(this.VMWindow_Resize);
      this.Activated += new EventHandler(this.VMWindow_Activated);
      this.Deactivate += new EventHandler(this.VMWindow_Deactivate);
      this.GotFocus += new EventHandler(this.VMWindow_GotFocus);
      this.FormClosing += new FormClosingEventHandler(this.VMWindow_FormClosing);
      this.InputLanguageChanged += new InputLanguageChangedEventHandler(this.VMWindow_InputLanguageChanged);
      this.ResumeLayout(false);
      this.PerformLayout();
    }

    internal enum InputHandlingState
    {
      IMAP_STATE_NOFOCUS = 1,
      IMAP_STATE_MAPPING = 2,
      IMAP_STATE_TEXT = 3,
      IMAP_STATE_RAW = 4,
    }

    public enum tagINPUT_MESSAGE_ORIGIN_ID
    {
      IMO_UNAVAILABLE = 0,
      IMO_HARDWARE = 1,
      IMO_INJECTED = 2,
      IMO_SYSTEM = 4,
    }

    public enum tagINPUT_MESSAGE_DEVICE_TYPE
    {
      IMDT_UNAVAILABLE = 0,
      IMDT_KEYBOARD = 1,
      IMDT_MOUSE = 2,
      IMDT_TOUCH = 4,
      IMDT_PEN = 8,
    }

    public struct tagINPUT_MESSAGE_SOURCE
    {
      public VMWindow.tagINPUT_MESSAGE_DEVICE_TYPE deviceType;
      public VMWindow.tagINPUT_MESSAGE_ORIGIN_ID originId;
    }

    public struct CursorCoordinate
    {
      public float XPerc;
      public float YPerc;
    }

    internal class CustomTextBox : System.Windows.Forms.TextBox
    {
      public CustomTextBox()
      {
        this.SetStyle(ControlStyles.UserPaint | ControlStyles.SupportsTransparentBackColor | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
        this.BackColor = System.Drawing.Color.Transparent;
        this.ForeColor = System.Drawing.Color.Transparent;
        InteropWindow.SendMessage(this.Handle, 11, false, 0);
      }

      protected override void OnGotFocus(EventArgs e)
      {
        base.OnGotFocus(e);
        InteropWindow.HideCaret(this.Handle);
        InteropWindow.SendMessage(this.Handle, 11, false, 0);
        InteropWindow.SetFocus(this.Handle);
      }

      internal void Focus()
      {
        InteropWindow.SetFocus(this.Handle);
      }
    }

    internal class CustomElementHost : ElementHost
    {
      protected override void OnGotFocus(EventArgs e)
      {
        base.OnGotFocus(e);
        InteropWindow.SendMessage(this.Handle, 11, false, 0);
      }

      internal void Focus()
      {
        InteropWindow.SetFocus(this.Handle);
      }
    }
  }
}
