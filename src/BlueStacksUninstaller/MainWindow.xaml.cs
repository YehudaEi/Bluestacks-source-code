// Decompiled with JetBrains decompiler
// Type: BlueStacks.Uninstaller.MainWindow
// Assembly: BlueStacksUninstaller, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: DBF002A0-6BF3-43CC-B5E7-0E90D1C19949
// Assembly location: C:\Program Files\BlueStacks\BlueStacksUninstaller.exe

using BlueStacks.Common;
using Microsoft.Win32;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace BlueStacks.Uninstaller
{
  public partial class MainWindow : Window, IComponentConnector
  {
    private static bool sWasUninstallationClean = true;
    private string mUninstallReasonString = "";
    private string mUserCommentString = "";
    private System.Timers.Timer mProgressTextAnimationTimer;
    private BackgroundWorker mUninstallerBackgroundWorker;
    internal Image mUnInstallFeedbackBackground;
    internal Image mUnInstallBackground;
    internal UninstallFeedback mUninstallFeedbackUserControl;
    internal CustomPictureBox mCloseButton;
    internal Image mProductLogo;
    internal StartUninstall mStartUnInstallUserControl;
    internal UninstallProgress mUninstallProgresslUserControl;
    internal UninstallFinish mUninstallFinishUserControl;
    private bool _contentLoaded;

    public MainWindow()
    {
      this.InitializeComponent();
      this.Title = Strings.UninstallerTitleName;
      this.Loaded += new RoutedEventHandler(this.MainWindow_Loaded);
      string path1 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets");
      if (Oem.IsOEMDmm)
      {
        this.mProductLogo.Visibility = Visibility.Hidden;
        this.mUninstallFeedbackUserControl.Visibility = Visibility.Hidden;
        this.mUnInstallFeedbackBackground.Visibility = Visibility.Hidden;
        this.Width = 500.0;
        this.Height = 450.0;
        this.mUnInstallBackground.Width = 400.0;
        this.mUnInstallBackground.Height = 350.0;
        this.Background = (Brush) new SolidColorBrush((System.Windows.Media.Color) ColorConverter.ConvertFromString("#21233A"));
        this.mUnInstallBackground.Source = (ImageSource) ImageUtils.BitmapFromPath(Path.Combine(path1, "dmm_bluestacks.png"));
        this.mUnInstallBackground.VerticalAlignment = VerticalAlignment.Center;
        this.mUnInstallBackground.HorizontalAlignment = HorizontalAlignment.Center;
        this.mUnInstallBackground.Margin = new Thickness(-10.0, -70.0, -10.0, 0.0);
        this.mUnInstallBackground.Visibility = Visibility.Visible;
      }
      else if (RegistryManager.Instance.InstallationType == InstallationTypes.GamingEdition)
      {
        this.mUnInstallBackground.Source = (ImageSource) ImageUtils.BitmapFromPath(Path.Combine(path1, "gameinstaller_bg.png"));
        this.mUnInstallFeedbackBackground.Source = (ImageSource) ImageUtils.BitmapFromPath(Path.Combine(path1, "gameinstaller_bg_blurred.png"));
        this.mProductLogo.Source = (ImageSource) ImageUtils.BitmapFromPath(Path.Combine(path1, "gameinstaller_logo.png"));
        this.Icon = (ImageSource) BitmapFrame.Create(new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path.Combine(path1, "app_icon.ico"))));
      }
      else
      {
        this.mUnInstallBackground.Source = (ImageSource) ImageUtils.BitmapFromPath(Path.Combine(path1, "installer_bg.png"));
        this.mUnInstallFeedbackBackground.Source = (ImageSource) ImageUtils.BitmapFromPath(Path.Combine(path1, "installer_bg_blurred.png"));
        this.mProductLogo.Source = (ImageSource) ImageUtils.BitmapFromPath(Path.Combine(path1, "installer_logo.png"));
      }
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
      this.mStartUnInstallUserControl.mUninstallStart.Click += new RoutedEventHandler(this.UninstallStart_Click);
      this.mStartUnInstallUserControl.mUninstallCancel.Click += new RoutedEventHandler(this.UninstallCancel_Click);
      this.mUninstallFinishUserControl.mUninstallFinished.Click += new RoutedEventHandler(this.MUninstallFinished_Click);
      this.mUninstallFeedbackUserControl.mOtherReasonTextBox.GotFocus += new RoutedEventHandler(this.MOtherReasonTextBox_GotFocus);
      this.mUninstallProgresslUserControl.mInstallProgressBar.ValueChanged += new RoutedPropertyChangedEventHandler<double>(this.MInstallProgressBar_ValueChanged);
      if (UninstallerProperties.IsRunningInSilentMode)
      {
        this.Hide();
        this.UninstallStart_Click((object) null, (RoutedEventArgs) null);
      }
      this.mUnInstallBackground.MouseLeftButtonDown += new MouseButtonEventHandler(this.HandleMouseDrag);
      this.mStartUnInstallUserControl.MouseLeftButtonDown += new MouseButtonEventHandler(this.HandleMouseDrag);
      this.mUninstallFinishUserControl.MouseLeftButtonDown += new MouseButtonEventHandler(this.HandleMouseDrag);
      this.mUninstallFeedbackUserControl.MouseLeftButtonDown += new MouseButtonEventHandler(this.HandleMouseDrag);
      this.mUninstallProgresslUserControl.MouseLeftButtonDown += new MouseButtonEventHandler(this.HandleMouseDrag);
      this.FixUpUninstallInstallButtonSize();
    }

    private void HandleMouseDrag(object sender, MouseButtonEventArgs e)
    {
      try
      {
        this.DragMove();
      }
      catch
      {
      }
    }

    private void FixUpUninstallInstallButtonSize()
    {
      double actualWidth = this.mStartUnInstallUserControl.mUninstallCancel.ActualWidth;
      double num = Math.Max(this.mStartUnInstallUserControl.mUninstallCancel.ActualWidth, this.mStartUnInstallUserControl.mUninstallStart.ActualWidth);
      this.mStartUnInstallUserControl.mUninstallCancel.Width = num;
      this.mStartUnInstallUserControl.mUninstallStart.Width = num;
    }

    private void MOtherReasonTextBox_GotFocus(object sender, RoutedEventArgs e)
    {
      this.mUninstallFeedbackUserControl.mOtherReasonTextBox.Text = "";
      this.mUninstallFeedbackUserControl.mOtherReasonTextBox.Foreground = (Brush) Brushes.White;
    }

    private void MInstallProgressBar_ValueChanged(
      object sender,
      RoutedPropertyChangedEventArgs<double> e)
    {
      this.mUninstallProgresslUserControl.mInstallProgressPercentage.Content = (object) (((int) e.NewValue).ToString() + "%");
    }

    private void MUninstallFinished_Click(object sender, RoutedEventArgs e)
    {
      Application.Current.Shutdown();
    }

    private void UninstallCancel_Click(object sender, RoutedEventArgs e)
    {
      Application.Current.Shutdown();
    }

    private void UninstallStart_Click(object sender, RoutedEventArgs e)
    {
      if (UninstallerProperties.IsRunningInSilentMode || Oem.IsOEMDmm || RegistryManager.Instance.InstallationType == InstallationTypes.GamingEdition)
      {
        this.UninstallConfirmationAcceptedHandler((object) null, (EventArgs) null);
      }
      else
      {
        CustomMessageWindow customMessageWindow = new CustomMessageWindow();
        customMessageWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_UNINSTALL_TITLE", "Uninstall " + Strings.ProductDisplayName);
        customMessageWindow.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_UNINSTALL_BLUESTACKS", "Would you like to uninstall " + Strings.ProductDisplayName + "?");
        customMessageWindow.AddButton(ButtonColors.Red, "STRING_UNINSTALL", new EventHandler(this.UninstallConfirmationAcceptedHandler), (string) null, false, (object) null, true);
        customMessageWindow.AddButton((ButtonColors) Enum.Parse(typeof (ButtonColors), Strings.UninstallCancelBtnColor), "STRING_CANCEL", (EventHandler) null, (string) null, false, (object) null, true);
        if (Oem.Instance.IsBackupWarningVisible)
        {
          customMessageWindow.BodyWarningTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_UNINSTALL_BLUESTACKS_WARNING", "Your data will be lost. Go to Settings > Backup & restore to backup your data.");
          customMessageWindow.BodyWarningTextBlock.Visibility = Visibility.Visible;
        }
        customMessageWindow.Owner = (Window) this;
        customMessageWindow.ShowDialog();
      }
    }

    private void UninstallConfirmationAcceptedHandler(object sender, EventArgs e)
    {
      this.SetupWindowForUninstallAndUninstall();
    }

    private void SetupWindowForUninstallAndUninstall()
    {
      this.GetUninstallReason();
      this.mStartUnInstallUserControl.Visibility = Visibility.Hidden;
      this.mUnInstallFeedbackBackground.Visibility = Visibility.Hidden;
      this.mUninstallFeedbackUserControl.Visibility = Visibility.Hidden;
      this.mCloseButton.SetDisabledState();
      this.mUnInstallBackground.Visibility = Visibility.Visible;
      this.mUninstallProgresslUserControl.Visibility = Visibility.Visible;
      this.StartBackgroundWorkerForUninstall();
    }

    private int Power2toi(int i)
    {
      return (int) Math.Pow(2.0, (double) i);
    }

    private void GetUninstallReason()
    {
      this.mUninstallReasonString += this.mUninstallFeedbackUserControl.mInstallEngineFail.IsChecked ? "InstallEngineFail, " : "";
      this.mUninstallReasonString += this.mUninstallFeedbackUserControl.mInstallGameFail.IsChecked ? "InstallGameFail, " : "";
      this.mUninstallReasonString += this.mUninstallFeedbackUserControl.mConflictWithOthers.IsChecked ? "ConflictWithOthers, " : "";
      this.mUninstallReasonString += this.mUninstallFeedbackUserControl.mStartEngineFail.IsChecked ? "StartEngineFail, " : "";
      this.mUninstallReasonString += this.mUninstallFeedbackUserControl.mGameLag.IsChecked ? "GameLag, " : "";
      this.mUninstallReasonString += this.mUninstallFeedbackUserControl.mBlackScreen.IsChecked ? "BlackScreen, " : "";
      this.mUninstallReasonString += this.mUninstallFeedbackUserControl.mCantFindGame.IsChecked ? "CantFindGame, " : "";
      this.mUninstallReasonString += this.mUninstallFeedbackUserControl.mAppCrash.IsChecked ? "AppCrash, " : "";
      this.mUninstallReasonString += this.mUninstallFeedbackUserControl.mExeCrash.IsChecked ? "ExeCrash, " : "";
      if (!string.IsNullOrEmpty(this.mUninstallReasonString) && this.mUninstallReasonString.EndsWith(", "))
        this.mUninstallReasonString = this.mUninstallReasonString.Substring(0, this.mUninstallReasonString.LastIndexOf(", "));
      string text = this.mUninstallFeedbackUserControl.mOtherReasonTextBox.Text;
      if (string.Compare(text, LocaleStrings.GetLocalizedString("STRING_OTHER_REASON", ""), StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(text, "other reasons or information like game name etc.", StringComparison.OrdinalIgnoreCase) == 0)
        return;
      this.mUserCommentString = text;
    }

    private void StartBackgroundWorkerForUninstall()
    {
      this.mUninstallerBackgroundWorker = new BackgroundWorker();
      this.mUninstallerBackgroundWorker.DoWork += new DoWorkEventHandler(this.UninstallerBackgroundWorker_DoWork);
      this.mUninstallerBackgroundWorker.ProgressChanged += new ProgressChangedEventHandler(this.UninstallerBackgroundWorker_ProgressChanged);
      this.mUninstallerBackgroundWorker.WorkerReportsProgress = true;
      this.mUninstallerBackgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.UninstallerBackgroundWorker_RunWorkerCompleted);
      this.mUninstallerBackgroundWorker.RunWorkerAsync();
    }

    private void UninstallerBackgroundWorker_RunWorkerCompleted(
      object sender,
      RunWorkerCompletedEventArgs e)
    {
      Logger.Info("uninstallation clean value : " + UninstallerStats.UninstallerComment.ToString());
      if (!MainWindow.sWasUninstallationClean)
        UninstallerStats.ReportUninstallFailedStats(UninstallerStatsEvent.UninstallFailed);
      if (UninstallerProperties.IsRunningInSilentMode)
      {
        App.ExitApplication();
      }
      else
      {
        this.StopProgressAnimationTimers();
        this.mUninstallProgresslUserControl.Visibility = Visibility.Hidden;
        this.mUninstallFinishUserControl.Visibility = Visibility.Visible;
        this.mCloseButton.SetNormalState();
      }
    }

    private void UninstallerBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
    {
      this.StartProgressAnimationTimers();
      this.mUninstallerBackgroundWorker.ReportProgress(10, (object) LocaleStrings.GetLocalizedString("STRING_ENGINE_UNINSTALL", ""));
      this.QuitAndKillBlueStacks();
      if (string.Equals("bgp", "bgp", StringComparison.InvariantCultureIgnoreCase))
        this.RemoveHelper();
      string tempPath = Path.GetTempPath();
      string installDir = RegistryStrings.InstallDir;
      try
      {
        CommonInstallUtils.DeleteDirectory(RegistryStrings.InstallDir);
      }
      catch (Exception ex)
      {
        Logger.Warning("An error occured while deleting installDir");
        Logger.Warning(ex.ToString());
        UninstallerStats.UninstallerComment |= 1;
        MainWindow.sWasUninstallationClean = false;
      }
      try
      {
        CommonInstallUtils.DeleteDirectory(RegistryManager.Instance.UserDefinedDir);
      }
      catch (Exception ex)
      {
        Logger.Warning("An error occured while deleting datadir");
        Logger.Warning(ex.ToString());
        UninstallerStats.UninstallerComment |= 2;
        MainWindow.sWasUninstallationClean = false;
      }
      try
      {
        CommonInstallUtils.DeleteDirectory(RegistryManager.Instance.SetupFolder);
      }
      catch
      {
      }
      this.DeleteShortcuts();
      this.DeleteRegistryHelper(RegistryManager.Instance.BaseKeyPath);
      this.DeleteRegistryHelper(Path.Combine("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall", "BlueStacks" + Strings.OEMTag));
      try
      {
        if (!ServiceManager.UninstallService(Strings.BlueStacksDriverName, true))
        {
          UninstallerStats.UninstallerComment |= 8;
          MainWindow.sWasUninstallationClean = false;
        }
      }
      catch (Exception ex)
      {
        Logger.Warning("An error occured while uninstalling services");
        Logger.Warning(ex.ToString());
        UninstallerStats.UninstallerComment |= 8;
        MainWindow.sWasUninstallationClean = false;
      }
      this.mUninstallerBackgroundWorker.ReportProgress(80, (object) LocaleStrings.GetLocalizedString("STRING_CLIENT_UNINSTALL", ""));
      string path = Path.Combine(tempPath, "game_config.json");
      try
      {
        if (File.Exists(path))
          File.Delete(path);
      }
      catch (Exception ex)
      {
        Logger.Error("Couldnt delete " + path + ". Ex: " + ex.Message);
      }
      if (Oem.Instance.IsCreateInstallApkRegistry)
      {
        CommonInstallUtils.DeleteInstallApkScheme(installDir, "BlueStacks.Apk");
        CommonInstallUtils.DeleteInstallApkScheme(installDir, "BlueStacks.Xapk");
      }
      UninstallerStats.SendUninstallCompletedStats(this.mUserCommentString, this.mUninstallReasonString);
      Thread.Sleep(1000);
      this.mUninstallerBackgroundWorker.ReportProgress(100, (object) LocaleStrings.GetLocalizedString("STRING_UNINSTALL_FINISHED", ""));
    }

    private void RemoveHelper()
    {
      try
      {
        TaskScheduler.DeleteTask("BlueStacksHelper");
      }
      catch (Exception ex)
      {
        Logger.Warning("Couldn't delete task {0}, ex: ", (object) "BlueStacksHelper", (object) ex.Message);
      }
      try
      {
        TaskScheduler.DeleteTask("BlueStacksHelperTask");
      }
      catch (Exception ex)
      {
        Logger.Warning("Couldn't delete task {0}, ex: ", (object) "BlueStacksHelperTask", (object) ex.Message);
      }
    }

    public void DeleteShortcuts()
    {
      try
      {
        List<string> stringList = new List<string>();
        stringList.AddRange((IEnumerable<string>) Directory.GetFiles(ShortcutHelper.sDesktopPath, "*.lnk", SearchOption.AllDirectories));
        stringList.AddRange((IEnumerable<string>) Directory.GetFiles(ShortcutHelper.CommonStartMenuPath, "*.lnk", SearchOption.AllDirectories));
        if (Oem.Instance.IsCreateDesktopIconForApp)
        {
          foreach (string str in stringList)
          {
            try
            {
              if (Utils.IsTargetForShortcut(str, "HD-RunApp.exe"))
                File.Delete(str);
            }
            catch (Exception ex)
            {
              Logger.Warning("Could not delete app shortcut, err: {0}", (object) ex.Message);
            }
          }
        }
        if (!Oem.Instance.CreateDesktopIcons)
          return;
        foreach (string vmDisplayName in UninstallerProperties.VmDisplayNameList)
        {
          try
          {
            ShortcutHelper.DeleteDesktopShortcut(vmDisplayName);
          }
          catch (Exception ex)
          {
          }
        }
        ShortcutHelper.DeleteCommonDesktopShortcut(Oem.Instance.DesktopShortcutFileName);
        string shortcutFileName = Oem.Instance.DesktopShortcutFileName;
        if (!string.IsNullOrEmpty(shortcutFileName))
          ShortcutHelper.DeleteCommonStartMenuShortcut(shortcutFileName);
        ShortcutHelper.DeleteCommonDesktopShortcut(Oem.Instance.MultiInstanceManagerShortcutFileName);
        ShortcutHelper.DeleteCommonStartMenuShortcut(Oem.Instance.MultiInstanceManagerShortcutFileName);
      }
      catch (Exception ex)
      {
        Logger.Error(ex.ToString());
      }
    }

    private void QuitAndKillBlueStacks()
    {
      CommonInstallUtils.KillBlueStacksProcesses((string) null, true);
      ServiceManager.StopService(Strings.BlueStacksDriverName, true);
    }

    private void UninstallerBackgroundWorker_ProgressChanged(
      object sender,
      ProgressChangedEventArgs e)
    {
      this.AnimateProgressJump((double) e.ProgressPercentage);
      this.mUninstallProgresslUserControl.mInstallProgressBar.Value = (double) e.ProgressPercentage;
      this.mUninstallProgresslUserControl.mInstallProgressPercentage.Content = (object) (e.ProgressPercentage.ToString() + "%");
      this.mUninstallProgresslUserControl.mInstallProgressStatus.Content = (object) e.UserState.ToString();
    }

    private void AnimateProgressJump(double percentage)
    {
      TimeSpan timeSpan = TimeSpan.FromSeconds(1.0);
      DoubleAnimation doubleAnimation = new DoubleAnimation(percentage, (Duration) timeSpan);
      this.mUninstallProgresslUserControl.mInstallProgressBar.BeginAnimation(RangeBase.ValueProperty, (AnimationTimeline) doubleAnimation);
    }

    private void StartProgressAnimationTimers()
    {
      this.mProgressTextAnimationTimer = new System.Timers.Timer()
      {
        Enabled = true,
        Interval = 500.0
      };
      this.mProgressTextAnimationTimer.Elapsed += new ElapsedEventHandler(this.ProgressTextAnimationTimer_Elapsed);
      this.mProgressTextAnimationTimer.Start();
    }

    private void StopProgressAnimationTimers()
    {
      this.mProgressTextAnimationTimer.Stop();
    }

    private void ProgressTextAnimationTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
      Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (Delegate) (() =>
      {
        try
        {
          string str = (string) this.mUninstallProgresslUserControl.mInstallProgressStatus.Content;
          if (!string.IsNullOrEmpty(str))
            str = str.EndsWith(".") ? (!str.EndsWith("...") ? str + "." : str.Replace(".", "")) : str + ".";
          this.mUninstallProgresslUserControl.mInstallProgressStatus.Content = (object) str;
        }
        catch (Exception ex)
        {
          Logger.Error("Error occured, ex: {0}", (object) ex.Message);
        }
      }));
    }

    private void DeleteRegistryHelper(string keyPath)
    {
      try
      {
        Registry.LocalMachine.DeleteSubKeyTree(keyPath);
      }
      catch (Exception ex)
      {
        Logger.Warning("An error occured while deleting: " + keyPath);
        Logger.Warning(ex.ToString());
        UninstallerStats.UninstallerComment |= 4;
        MainWindow.sWasUninstallationClean = false;
      }
    }

    private void mCloseButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      if (!(sender is CustomPictureBox) || (sender as CustomPictureBox).ButtonState != CustomPictureBox.State.normal)
        return;
      App.ExitApplication();
    }

    private void Window_Closing(object sender, CancelEventArgs e)
    {
      this.mCloseButton_MouseLeftButtonUp(sender, (MouseButtonEventArgs) null);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/BlueStacksUninstaller;component/mainwindow.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    internal Delegate _CreateDelegate(Type delegateType, string handler)
    {
      return Delegate.CreateDelegate(delegateType, (object) this, handler);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          ((Window) target).Closing += new CancelEventHandler(this.Window_Closing);
          break;
        case 2:
          this.mUnInstallFeedbackBackground = (Image) target;
          break;
        case 3:
          this.mUnInstallBackground = (Image) target;
          break;
        case 4:
          this.mUninstallFeedbackUserControl = (UninstallFeedback) target;
          break;
        case 5:
          this.mCloseButton = (CustomPictureBox) target;
          this.mCloseButton.MouseLeftButtonUp += new MouseButtonEventHandler(this.mCloseButton_MouseLeftButtonUp);
          break;
        case 6:
          this.mProductLogo = (Image) target;
          break;
        case 7:
          this.mStartUnInstallUserControl = (StartUninstall) target;
          break;
        case 8:
          this.mUninstallProgresslUserControl = (UninstallProgress) target;
          break;
        case 9:
          this.mUninstallFinishUserControl = (UninstallFinish) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
