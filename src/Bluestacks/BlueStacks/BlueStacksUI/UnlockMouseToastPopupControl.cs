// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.UnlockMouseToastPopupControl
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Threading;

namespace BlueStacks.BlueStacksUI
{
  public class UnlockMouseToastPopupControl : UserControl, IComponentConnector
  {
    private readonly DispatcherTimer mUnlockMouseToastTimer = new DispatcherTimer();
    private MainWindow mMainWindow;
    internal Border mToastPopupBorder;
    internal Border mMaskBorder;
    internal DockPanel mToastPanel;
    internal TextBlock mTipTextblock;
    internal Border mKeyBorder;
    internal TextBlock mKeyTextBlock;
    internal TextBlock mInfoTextblock;
    internal CustomPictureBox mToastIcon;
    private bool _contentLoaded;

    public MainWindow ParentWindow
    {
      get
      {
        if (this.mMainWindow == null)
          this.Dispatcher.Invoke((Delegate) (() => this.mMainWindow = Window.GetWindow((DependencyObject) this) as MainWindow));
        return this.mMainWindow;
      }
      set
      {
        this.mMainWindow = value;
      }
    }

    public UnlockMouseToastPopupControl()
    {
      this.InitializeComponent();
    }

    private void Init(string message)
    {
      this.mToastPanel.MaxWidth = this.ParentWindow.ActualWidth - 180.0;
      string[] strArray1;
      if (message == null)
        strArray1 = (string[]) null;
      else
        strArray1 = message.Split('{', '}');
      string[] strArray2 = strArray1;
      this.mTipTextblock.Text = strArray2[0];
      this.mKeyTextBlock.Text = this.ParentWindow.mCommonHandler.GetShortcutKeyFromName("STRING_UNLOCK_MOUSE", false);
      this.mInfoTextblock.Text = strArray2[2];
      this.mUnlockMouseToastTimer.Interval = TimeSpan.FromMilliseconds(15000.0);
      this.mUnlockMouseToastTimer.Tick += new EventHandler(this.MouseUnlockToastTimer_Tick);
    }

    internal void ShowUnlockMouseToast(string message)
    {
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        try
        {
          if (!this.ParentWindow.mIsWindowInFocus || Oem.IsOEMDmm)
            return;
          if (this.ParentWindow.mUnlockMouseToastPopup.IsOpen)
            this.CloseUnlockMouseToastAndStopTimer();
          if (this.ParentWindow.mFullScreenToastPopup.IsOpen)
            this.ParentWindow.CloseFullScreenToastAndStopTimer();
          this.Init(message);
          this.ParentWindow.mUnlockMouseToastPopup.IsOpen = true;
          this.ParentWindow.mUnlockMouseToastCanvas.Height = this.ActualHeight;
          this.ParentWindow.mUnlockMouseToastPopup.VerticalOffset = this.ParentWindow.mIsFullScreen ? this.ActualHeight + 20.0 : this.ActualHeight - 20.0;
          this.mUnlockMouseToastTimer.Start();
        }
        catch (Exception ex)
        {
          Logger.Error("Exception in showing Unlock Mouse toast : " + ex.ToString());
        }
      }));
    }

    private void ToastIcon_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.CloseUnlockMouseToastAndStopTimer();
    }

    private void MouseUnlockToastTimer_Tick(object sender, EventArgs e)
    {
      this.CloseUnlockMouseToastAndStopTimer();
    }

    internal void CloseUnlockMouseToastAndStopTimer()
    {
      this.mUnlockMouseToastTimer.Stop();
      this.ParentWindow.mUnlockMouseToastPopup.IsOpen = false;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/unlockmousetoastpopupcontrol.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.mToastPopupBorder = (Border) target;
          break;
        case 2:
          this.mMaskBorder = (Border) target;
          break;
        case 3:
          this.mToastPanel = (DockPanel) target;
          break;
        case 4:
          this.mTipTextblock = (TextBlock) target;
          break;
        case 5:
          this.mKeyBorder = (Border) target;
          break;
        case 6:
          this.mKeyTextBlock = (TextBlock) target;
          break;
        case 7:
          this.mInfoTextblock = (TextBlock) target;
          break;
        case 8:
          this.mToastIcon = (CustomPictureBox) target;
          this.mToastIcon.MouseLeftButtonUp += new MouseButtonEventHandler(this.ToastIcon_MouseLeftButtonUp);
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
