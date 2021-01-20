// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.MacroTopBarRecordControl
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Threading;

namespace BlueStacks.BlueStacksUI
{
  public class MacroTopBarRecordControl : UserControl, IComponentConnector
  {
    private bool mShowRecordingIcon = true;
    private MainWindow ParentWindow;
    private DispatcherTimer mTimer;
    private DispatcherTimer mBlinkRecordingIconTimer;
    private DateTime mStartTime;
    private DateTime mPauseTime;
    internal Border mMaskBorder;
    internal CustomPictureBox mRecordingImage;
    internal TextBlock TimerDisplay;
    internal CustomPictureBox mPauseMacroImg;
    internal CustomPictureBox mPlayMacroImg;
    internal CustomPictureBox mStopMacroImg;
    private bool _contentLoaded;

    public MacroTopBarRecordControl()
    {
      this.InitializeComponent();
    }

    internal void Init(MainWindow window)
    {
      this.ParentWindow = window;
      this.mTimer = new DispatcherTimer(new TimeSpan(0, 0, 0, 0, 50), DispatcherPriority.Render, new EventHandler(this.T_Tick), Dispatcher.CurrentDispatcher);
      this.mPlayMacroImg.Visibility = Visibility.Collapsed;
      this.mPauseMacroImg.Visibility = Visibility.Visible;
      this.mBlinkRecordingIconTimer = new DispatcherTimer(new TimeSpan(0, 0, 0, 0, 500), DispatcherPriority.Render, new EventHandler(this.BlinkRecordingIcon_Tick), Dispatcher.CurrentDispatcher);
      if (FeatureManager.Instance.IsCustomUIForNCSoft)
        BlueStacksUIBinding.Bind(this.ParentWindow.mNCTopBar.mMacroRecordingTooltip, "STRING_PAUSE_RECORDING_TOOLTIP", "");
      else
        BlueStacksUIBinding.Bind(this.ParentWindow.mTopBar.mMacroRecordingTooltip, "STRING_PAUSE_RECORDING_TOOLTIP", "");
    }

    private void BlinkRecordingIcon_Tick(object sender, EventArgs e)
    {
      this.ToggleRecordingIcon();
    }

    private void PauseMacroRecording_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.ParentWindow.mFrontendHandler.SendFrontendRequestAsync("pauseRecordingCombo", (Dictionary<string, string>) null);
      this.PauseTimer();
      this.mPauseMacroImg.Visibility = Visibility.Collapsed;
      this.mPlayMacroImg.Visibility = Visibility.Visible;
      ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "macro_record_pause", (string) null, RecordingTypes.SingleRecording.ToString(), (string) null, (string) null, (string) null, "Android");
    }

    private void ResumeMacroRecording_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.ParentWindow.mFrontendHandler.SendFrontendRequestAsync("startRecordingCombo", (Dictionary<string, string>) null);
      this.ResumeTimer();
      this.mPlayMacroImg.Visibility = Visibility.Collapsed;
      this.mPauseMacroImg.Visibility = Visibility.Visible;
    }

    private void StopMacroRecording_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.ParentWindow.mCommonHandler.StopMacroRecording();
      this.mBlinkRecordingIconTimer.Stop();
      this.mRecordingImage.ImageName = "recording_macro_title_bar";
      ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "macro_record_stop", (string) null, RecordingTypes.SingleRecording.ToString(), (string) null, (string) null, (string) null, "Android");
    }

    internal void StopTimer()
    {
      this.mTimer.Stop();
      this.mBlinkRecordingIconTimer.Stop();
    }

    internal void StartTimer()
    {
      this.mTimer.Start();
      this.mStartTime = DateTime.Now;
      this.mBlinkRecordingIconTimer.Start();
    }

    private void T_Tick(object sender, EventArgs e)
    {
      TimeSpan timeSpan = DateTime.Now - this.mStartTime;
      this.TimerDisplay.Text = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0:00}:{1:00}:{2:00}", (object) timeSpan.Minutes, (object) timeSpan.Seconds, (object) (timeSpan.Milliseconds / 10));
    }

    private void ToggleRecordingIcon()
    {
      if (this.mShowRecordingIcon)
      {
        this.mRecordingImage.ImageName = "recording_macro_active";
        this.mShowRecordingIcon = false;
      }
      else
      {
        this.mRecordingImage.ImageName = "recording_macro";
        this.mShowRecordingIcon = true;
      }
    }

    internal void PauseTimer()
    {
      this.mTimer.IsEnabled = false;
      this.mTimer.Stop();
      this.mPauseTime = DateTime.Now;
      this.mBlinkRecordingIconTimer.Stop();
      this.mShowRecordingIcon = true;
      this.ToggleRecordingIcon();
    }

    internal void ResumeTimer()
    {
      this.mStartTime += DateTime.Now - this.mPauseTime;
      this.mTimer.IsEnabled = true;
      this.mTimer.Start();
      this.mBlinkRecordingIconTimer.Start();
      this.mShowRecordingIcon = true;
      this.ToggleRecordingIcon();
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/macrotopbarrecordcontrol.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.mMaskBorder = (Border) target;
          break;
        case 2:
          this.mRecordingImage = (CustomPictureBox) target;
          break;
        case 3:
          this.TimerDisplay = (TextBlock) target;
          break;
        case 4:
          this.mPauseMacroImg = (CustomPictureBox) target;
          this.mPauseMacroImg.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.PauseMacroRecording_MouseLeftButtonUp);
          break;
        case 5:
          this.mPlayMacroImg = (CustomPictureBox) target;
          this.mPlayMacroImg.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.ResumeMacroRecording_MouseLeftButtonUp);
          break;
        case 6:
          this.mStopMacroImg = (CustomPictureBox) target;
          this.mStopMacroImg.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.StopMacroRecording_PreviewMouseLeftButtonUp);
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
