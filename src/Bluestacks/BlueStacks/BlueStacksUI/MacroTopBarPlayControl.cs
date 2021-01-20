// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.MacroTopBarPlayControl
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System;
using System.CodeDom.Compiler;
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
  public class MacroTopBarPlayControl : UserControl, IComponentConnector
  {
    private bool mShowPlayingIcon = true;
    private MainWindow ParentWindow;
    internal MacroRecording mOperationsRecord;
    private DispatcherTimer mBlinkPlayingIconTimer;
    private DispatcherTimer mTimer;
    internal Border mMaskBorder;
    internal CustomPictureBox RecordingImage;
    internal StackPanel mDescriptionPanel;
    internal TextBlock mRunningScript;
    internal TextBlock mRunningIterations;
    internal TextBlock mTimerDisplay;
    internal CustomPictureBox StopMacroImg;
    private bool _contentLoaded;

    public DateTime mStartTime { get; set; }

    internal event MacroTopBarPlayControl.ScriptPlayDelegate ScriptPlayEvent;

    internal event MacroTopBarPlayControl.ScriptStopDelegate ScriptStopEvent;

    public MacroTopBarPlayControl()
    {
      this.InitializeComponent();
    }

    internal void OnScriptPlayEvent(string tag)
    {
      MacroTopBarPlayControl.ScriptPlayDelegate scriptPlayEvent = this.ScriptPlayEvent;
      if (scriptPlayEvent == null)
        return;
      scriptPlayEvent(tag);
    }

    private void BlinkPlayingIcon_Tick(object sender, EventArgs e)
    {
      this.ToggleRecordingIcon();
    }

    internal void StopTimer()
    {
      this.mBlinkPlayingIconTimer.Stop();
      this.mTimer.Stop();
    }

    internal void StartTimer()
    {
      this.mBlinkPlayingIconTimer.Start();
      this.mTimer.Start();
    }

    private void ToggleRecordingIcon()
    {
      if (this.mShowPlayingIcon)
      {
        this.RecordingImage.ImageName = "recording_macro_title_play";
        this.mShowPlayingIcon = false;
      }
      else
      {
        this.RecordingImage.ImageName = "recording_macro";
        this.mShowPlayingIcon = true;
      }
    }

    internal void OnScriptStopEvent(string tag)
    {
      MacroTopBarPlayControl.ScriptStopDelegate scriptStopEvent = this.ScriptStopEvent;
      if (scriptStopEvent == null)
        return;
      scriptStopEvent(tag);
    }

    internal void Init(MainWindow parentWindow, MacroRecording record)
    {
      this.ParentWindow = parentWindow;
      this.mOperationsRecord = record;
      this.mRunningScript.Text = this.mOperationsRecord.Name;
      this.mRunningIterations.Visibility = Visibility.Visible;
      this.mRunningScript.ToolTip = (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} {1}", (object) LocaleStrings.GetLocalizedString("STRING_PLAYING", ""), (object) this.mRunningScript.Text);
      if (this.mBlinkPlayingIconTimer != null)
        return;
      this.mBlinkPlayingIconTimer = new DispatcherTimer(new TimeSpan(0, 0, 0, 0, 500), DispatcherPriority.Render, new EventHandler(this.BlinkPlayingIcon_Tick), Dispatcher.CurrentDispatcher);
      this.mTimer = new DispatcherTimer(new TimeSpan(0, 0, 0, 0, 50), DispatcherPriority.Render, new EventHandler(this.T_Tick), Dispatcher.CurrentDispatcher);
      this.StartTimer();
    }

    private void T_Tick(object sender, EventArgs e)
    {
      TimeSpan timeSpan = DateTime.Now - this.mStartTime;
      this.mTimerDisplay.Text = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0:00}:{1:00}:{2:00}", (object) timeSpan.Minutes, (object) timeSpan.Seconds, (object) (timeSpan.Milliseconds / 10));
    }

    internal void IncreaseIteration(int iteration)
    {
      this.mRunningIterations.Text = string.Format((IFormatProvider) CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_RUNNING_X_TIME", ""), (object) Strings.AddOrdinal(iteration));
    }

    private void PauseMacro_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
    }

    private void PlayMacro_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.OnScriptPlayEvent(this.mOperationsRecord.Name);
    }

    private void StopMacro_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.StopMacro();
    }

    public void StopMacro()
    {
      try
      {
        this.StopTimer();
        this.mBlinkPlayingIconTimer = (DispatcherTimer) null;
        this.ParentWindow?.mCommonHandler?.StopMacroScriptHandling();
        if (this.mOperationsRecord == null)
          return;
        MacroTopBarPlayControl.ScriptStopDelegate scriptStopEvent = this.ScriptStopEvent;
        if (scriptStopEvent != null)
          scriptStopEvent(this.mOperationsRecord.Name);
        ClientStats.SendMiscellaneousStatsAsync("MacroOperations", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "macro_stop", (string) null, this.mOperationsRecord.RecordingType.ToString(), (string) null, (string) null, (string) null, "Android");
      }
      catch (Exception ex)
      {
        Logger.Error("Error in StopMacro: " + ex.Message);
      }
    }

    internal void UpdateUiForIterationTillTime()
    {
      this.mRunningIterations.Visibility = Visibility.Collapsed;
      this.mTimerDisplay.Visibility = Visibility.Visible;
      this.mRunningScript.ToolTip = (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}-{1}sec", (object) this.mOperationsRecord.Name, (object) this.mOperationsRecord.LoopTime);
    }

    internal void UpdateUiMacroPlaybackForInfiniteTime(int iteration)
    {
      this.mTimerDisplay.Visibility = Visibility.Collapsed;
      this.mRunningIterations.Visibility = Visibility.Visible;
      this.mRunningIterations.Text = string.Format((IFormatProvider) CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_RUNNING_X_TIME", ""), (object) Strings.AddOrdinal(iteration));
      this.mRunningScript.ToolTip = (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}", (object) this.mOperationsRecord.Name);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/macrotopbarplaycontrol.xaml", UriKind.Relative));
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
          this.RecordingImage = (CustomPictureBox) target;
          break;
        case 3:
          this.mDescriptionPanel = (StackPanel) target;
          break;
        case 4:
          this.mRunningScript = (TextBlock) target;
          break;
        case 5:
          this.mRunningIterations = (TextBlock) target;
          break;
        case 6:
          this.mTimerDisplay = (TextBlock) target;
          break;
        case 7:
          this.StopMacroImg = (CustomPictureBox) target;
          this.StopMacroImg.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.StopMacro_PreviewMouseLeftButtonUp);
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }

    internal delegate void ScriptPlayDelegate(string tag);

    internal delegate void ScriptStopDelegate(string tag);
  }
}
