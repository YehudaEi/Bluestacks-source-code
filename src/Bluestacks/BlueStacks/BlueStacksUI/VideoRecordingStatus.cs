// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.VideoRecordingStatus
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
  public class VideoRecordingStatus : UserControl, IComponentConnector
  {
    private bool mToggleBlinkImage = true;
    private MainWindow ParentWindow;
    private DispatcherTimer mBlinkPlayingIconTimer;
    private DispatcherTimer mTimer;
    internal System.Action RecordingStoppedEvent;
    internal Border mMaskBorder;
    internal CustomPictureBox mRecordingImage;
    internal StackPanel mDescriptionPanel;
    internal TextBlock mRunningVideo;
    internal TextBlock mTimerDisplay;
    internal CustomPictureBox mStopVideoRecordImg;
    private bool _contentLoaded;

    public DateTime mStartTime { get; set; }

    public VideoRecordingStatus()
    {
      this.InitializeComponent();
    }

    private void StopRecord_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.ResetTimer();
      System.Action recordingStoppedEvent = this.RecordingStoppedEvent;
      if (recordingStoppedEvent != null)
        recordingStoppedEvent();
      this.ParentWindow.mCommonHandler.StopRecordVideo();
    }

    private void BlinkPlayingIcon_Tick(object sender, EventArgs e)
    {
      this.ToggleRecordingIcon();
    }

    internal void StopTimer()
    {
      DispatcherTimer playingIconTimer = this.mBlinkPlayingIconTimer;
      if ((playingIconTimer != null ? (playingIconTimer.IsEnabled ? 1 : 0) : 0) != 0)
        this.mBlinkPlayingIconTimer?.Stop();
      DispatcherTimer mTimer = this.mTimer;
      if ((mTimer != null ? (mTimer.IsEnabled ? 1 : 0) : 0) == 0)
        return;
      this.mTimer?.Stop();
    }

    internal void StartTimer()
    {
      this.mBlinkPlayingIconTimer.Start();
    }

    private void ToggleRecordingIcon()
    {
      this.mRecordingImage.ImageName = !this.mToggleBlinkImage ? "sidebar_video_capture_active" : "sidebar_video_capture";
      if (FeatureManager.Instance.IsCustomUIForNCSoft && this.ParentWindow.mSidebar != null)
        this.ParentWindow.mSidebar.ChangeVideoRecordingImage(this.mRecordingImage.ImageName);
      this.mToggleBlinkImage = !this.mToggleBlinkImage;
    }

    internal void Init(MainWindow parentWindow)
    {
      this.ParentWindow = parentWindow;
      if (this.mBlinkPlayingIconTimer != null)
        return;
      this.mBlinkPlayingIconTimer = new DispatcherTimer(new TimeSpan(0, 0, 0, 0, 500), DispatcherPriority.Render, new EventHandler(this.BlinkPlayingIcon_Tick), Dispatcher.CurrentDispatcher);
      this.mStartTime = DateTime.Now;
      this.mTimer = new DispatcherTimer(new TimeSpan(0, 0, 0, 0, 50), DispatcherPriority.Render, new EventHandler(this.T_Tick), Dispatcher.CurrentDispatcher);
      this.StartTimer();
    }

    private void T_Tick(object sender, EventArgs e)
    {
      TimeSpan timeSpan = DateTime.Now - this.mStartTime;
      this.mTimerDisplay.Text = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0:00}:{1:00}:{2:00}", (object) timeSpan.Minutes, (object) timeSpan.Seconds, (object) (timeSpan.Milliseconds / 10));
    }

    internal void ResetTimer()
    {
      this.StopTimer();
      this.mBlinkPlayingIconTimer = (DispatcherTimer) null;
      this.mTimer = (DispatcherTimer) null;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/videorecordingstatus.xaml", UriKind.Relative));
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
          this.mDescriptionPanel = (StackPanel) target;
          break;
        case 4:
          this.mRunningVideo = (TextBlock) target;
          break;
        case 5:
          this.mTimerDisplay = (TextBlock) target;
          break;
        case 6:
          this.mStopVideoRecordImg = (CustomPictureBox) target;
          this.mStopVideoRecordImg.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.StopRecord_PreviewMouseLeftButtonUp);
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
