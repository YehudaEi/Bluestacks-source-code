// Decompiled with JetBrains decompiler
// Type: BlueStacks.DiskCompactionTool.MainWindow
// Assembly: DiskCompactionTool, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 0D774D0F-793E-496D-B768-12A2EDB900B5
// Assembly location: C:\Program Files\BlueStacks\DiskCompactionTool.exe

using BlueStacks.Common;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace BlueStacks.DiskCompactionTool
{
  public partial class MainWindow : CustomWindow, IComponentConnector
  {
    internal CustomPictureBox mTitleIcon;
    internal TextBlock mTitle;
    internal CustomPictureBox mMinimizeButton;
    internal CustomPictureBox mCrossButton;
    internal StackPanel mBodyPanel;
    internal TextBlock mBodyText;
    internal Grid mProgressGrid;
    internal TextBlock mProgressStatus;
    internal TextBlock mProgressPercent;
    internal BlueProgressBar mProgressBar;
    internal Grid mButtonGrid;
    internal CustomButton mCancelBtn;
    internal CustomButton mBtn;
    private bool _contentLoaded;

    public MainWindow()
    {
      this.InitializeComponent();
      DiskCompactionHelper.Instance.InitProperties(this);
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
      this.UpdateUi(MainWindow.UiStates.progress);
      Stats.SendMiscellaneousStatsAsync("DiskCompactionStats", "DiskCompaction Started", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, App.sOpt.vmname, App.sOpt.relaunch.ToString(), App.sOpt.s.ToString(), (string) null, "Android", 0);
      DiskCompactionHelper.Instance.StartCompaction();
    }

    internal void UpdateProgress(double value)
    {
      value = Math.Round(value, 0);
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        this.mProgressBar.Value = value;
        this.mProgressPercent.Text = string.Format("{0}%", (object) value.ToString());
      }));
    }

    internal void UpdateProgressStatus(string msg)
    {
      this.Dispatcher.Invoke((Delegate) (() => this.mProgressStatus.Text = msg));
    }

    internal void UpdateUi(MainWindow.UiStates val)
    {
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        switch (val)
        {
          case MainWindow.UiStates.progress:
            this.mProgressGrid.Visibility = Visibility.Visible;
            this.mBodyText.Text = LocaleStrings.GetLocalizedString("STRING_PLEASE_DO_NOT_LAUNCH", "");
            this.mTitle.Text = LocaleStrings.GetLocalizedString("STRING_DISK_CLEANUP_PROGRESS", "");
            this.mButtonGrid.Visibility = Visibility.Collapsed;
            this.mMinimizeButton.Visibility = Visibility.Visible;
            BlueStacksUIBinding.BindColor((DependencyObject) this.mBodyText, TextBlock.ForegroundProperty, "PopupWindowWarningForegroundColor");
            this.mCrossButton.IsEnabled = false;
            this.mCrossButton.Opacity = 0.5;
            break;
          case MainWindow.UiStates.error:
            this.mProgressGrid.Visibility = Visibility.Collapsed;
            this.mMinimizeButton.Visibility = Visibility.Collapsed;
            this.mTitle.Text = LocaleStrings.GetLocalizedString("STRING_DISK_CLEANUP", "");
            BlueStacksUIBinding.BindColor((DependencyObject) this.mBodyText, TextBlock.ForegroundProperty, "SettingsWindowTitleBarForeGround");
            this.mBodyText.Text = string.Format("{0} {1}", (object) LocaleStrings.GetLocalizedString("STRING_DISK_CLEANUP_COULD_NOT_COMPLETE", ""), (object) LocaleStrings.GetLocalizedString("STRING_PLEASE_RESTART_PC_TRY_AGAIN", ""));
            this.mButtonGrid.Visibility = Visibility.Visible;
            this.mBtn.Visibility = Visibility.Collapsed;
            this.mCancelBtn.Content = (object) LocaleStrings.GetLocalizedString("STRING_CLOSE", "");
            this.mCancelBtn.ButtonColor = ButtonColors.Blue;
            this.mCrossButton.IsEnabled = true;
            this.mCrossButton.Opacity = 0.5;
            break;
          case MainWindow.UiStates.success:
            this.mProgressGrid.Visibility = Visibility.Collapsed;
            this.mMinimizeButton.Visibility = Visibility.Collapsed;
            this.mTitle.Text = LocaleStrings.GetLocalizedString("STRING_DISK_CLEANUP_SUCCESSFUL", "");
            this.mBodyText.Text = string.Format("{0}\n{1}", (object) LocaleStrings.GetLocalizedString("STRING_DISK_SUCCESSFULLY_CLEANDED", ""), (object) LocaleStrings.GetLocalizedString("STRING_INSTANCE_LAUNCH_CONFIRMATION", ""));
            BlueStacksUIBinding.BindColor((DependencyObject) this.mBodyText, TextBlock.ForegroundProperty, "SettingsWindowTitleBarForeGround");
            this.mButtonGrid.Visibility = Visibility.Visible;
            this.mBtn.Content = (object) LocaleStrings.GetLocalizedString("STRING_LAUNCH", "");
            this.mCrossButton.IsEnabled = true;
            this.mCrossButton.Opacity = 1.0;
            break;
        }
      }));
    }

    private void Launch_Click(object sender, RoutedEventArgs e)
    {
      this.LaunchBlueStacks();
    }

    internal void LaunchBlueStacks()
    {
      new Process()
      {
        StartInfo = {
          FileName = RegistryManager.Instance.PartnerExePath,
          Arguments = string.Format("-vmName={0}", (object) App.sOpt.vmname),
          UseShellExecute = false
        }
      }.Start();
      App.ExitApplication(DiskCompactionHelper.sDiskCompactionStatusCode);
    }

    private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
    {
      if (e.ChangedButton != MouseButton.Left)
        return;
      this.DragMove();
    }

    private void MinimizeButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.WindowState = WindowState.Minimized;
    }

    internal void CloseButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      App.ExitApplication(DiskCompactionHelper.sDiskCompactionStatusCode);
    }

    private void Btn_Click(object sender, RoutedEventArgs e)
    {
      App.ExitApplication(DiskCompactionHelper.sDiskCompactionStatusCode);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/DiskCompactionTool;component/mainwindow.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          ((FrameworkElement) target).Loaded += new RoutedEventHandler(this.MainWindow_Loaded);
          break;
        case 2:
          this.mTitleIcon = (CustomPictureBox) target;
          break;
        case 3:
          this.mTitle = (TextBlock) target;
          this.mTitle.MouseDown += new MouseButtonEventHandler(this.TitleBar_MouseDown);
          break;
        case 4:
          this.mMinimizeButton = (CustomPictureBox) target;
          this.mMinimizeButton.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.MinimizeButton_MouseLeftButtonUp);
          break;
        case 5:
          this.mCrossButton = (CustomPictureBox) target;
          this.mCrossButton.MouseLeftButtonUp += new MouseButtonEventHandler(this.CloseButton_MouseLeftButtonUp);
          break;
        case 6:
          this.mBodyPanel = (StackPanel) target;
          break;
        case 7:
          this.mBodyText = (TextBlock) target;
          break;
        case 8:
          this.mProgressGrid = (Grid) target;
          break;
        case 9:
          this.mProgressStatus = (TextBlock) target;
          break;
        case 10:
          this.mProgressPercent = (TextBlock) target;
          break;
        case 11:
          this.mProgressBar = (BlueProgressBar) target;
          break;
        case 12:
          this.mButtonGrid = (Grid) target;
          break;
        case 13:
          this.mCancelBtn = (CustomButton) target;
          this.mCancelBtn.Click += new RoutedEventHandler(this.Btn_Click);
          break;
        case 14:
          this.mBtn = (CustomButton) target;
          this.mBtn.Click += new RoutedEventHandler(this.Launch_Click);
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }

    public enum UiStates
    {
      progress,
      error,
      success,
    }
  }
}
