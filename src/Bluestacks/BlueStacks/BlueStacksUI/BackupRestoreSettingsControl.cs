// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.BackupRestoreSettingsControl
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
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Shapes;

namespace BlueStacks.BlueStacksUI
{
  public class BackupRestoreSettingsControl : UserControl, IComponentConnector
  {
    private MainWindow ParentWindow;
    internal Grid mDiskCleanupGrid;
    internal CustomButton mDiskCleanupBtn;
    internal Line mLineSeperator;
    internal Grid mBackupRestoreGrid;
    internal CustomButton mRestoreBtn;
    internal CustomButton mBackupBtn;
    private bool _contentLoaded;

    public BackupRestoreSettingsControl(MainWindow window)
    {
      this.InitializeComponent();
      this.ParentWindow = window;
      if (this.ParentWindow != null && !this.ParentWindow.IsDefaultVM)
      {
        this.mBackupRestoreGrid.Visibility = Visibility.Collapsed;
        this.mLineSeperator.Visibility = Visibility.Collapsed;
      }
      this.Visibility = Visibility.Hidden;
    }

    private void RestoreBtn_Click(object sender, RoutedEventArgs e)
    {
      CustomMessageWindow customMessageWindow = new CustomMessageWindow();
      customMessageWindow.ImageName = "backup_restore_popup_window";
      BlueStacksUIBinding.Bind(customMessageWindow.TitleTextBlock, "STRING_RESTORE_BACKUP", "");
      BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlock, "STRING_MAKE_SURE_LATEST_WARNING", "");
      customMessageWindow.AddButton(ButtonColors.Blue, "STRING_RESTORE_BUTTON", (EventHandler) ((sender1, e1) => this.LaunchDataManager("restore")), (string) null, false, (object) null, true);
      customMessageWindow.AddButton(ButtonColors.White, "STRING_CANCEL", (EventHandler) null, (string) null, false, (object) null, true);
      customMessageWindow.Owner = (Window) this.ParentWindow;
      customMessageWindow.ShowDialog();
    }

    private void BackupBtn_Click(object sender, RoutedEventArgs e)
    {
      CustomMessageWindow customMessageWindow = new CustomMessageWindow();
      customMessageWindow.ImageName = "backup_restore_popup_window";
      BlueStacksUIBinding.Bind(customMessageWindow.TitleTextBlock, "STRING_BACKUP_WARNING", "");
      BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlock, "STRING_BLUESTACKS_BACKUP_PROMPT", "");
      customMessageWindow.AddButton(ButtonColors.Blue, "STRING_BACKUP", (EventHandler) ((sender1, e1) => this.LaunchDataManager("backup")), (string) null, false, (object) null, true);
      customMessageWindow.AddButton(ButtonColors.White, "STRING_CANCEL", (EventHandler) null, (string) null, false, (object) null, true);
      customMessageWindow.Owner = (Window) this.ParentWindow;
      customMessageWindow.ShowDialog();
    }

    private void DiskCleanupBtn_Click(object sender, RoutedEventArgs e)
    {
      this.ParentWindow.Dispatcher.Invoke((Delegate) (() =>
      {
        if (ProcessUtils.IsAlreadyRunning("Global\\BlueStacks_DiskCompactor_Lockbgp"))
        {
          CustomMessageWindow customMessageWindow = new CustomMessageWindow();
          customMessageWindow.ImageName = "disk_cleanup_popup_window";
          customMessageWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_DISK_CLEANUP_MULTIPLE_RUN_HEADING", "");
          customMessageWindow.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_DISK_CLEANUP_MULTIPLE_RUN_MESSAGE", "");
          customMessageWindow.AddButton(ButtonColors.Blue, "STRING_OK", (EventHandler) null, (string) null, false, (object) null, true);
          customMessageWindow.CloseButtonHandle((Predicate<object>) null, (object) null);
          customMessageWindow.Owner = (Window) this.ParentWindow;
          customMessageWindow.ShowDialog();
        }
        else
        {
          CustomMessageWindow customMessageWindow = new CustomMessageWindow();
          customMessageWindow.ImageName = "disk_cleanup_popup_window";
          customMessageWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_DISK_CLEANUP", "");
          customMessageWindow.BodyTextBlockTitle.Text = LocaleStrings.GetLocalizedString("STRING_DISK_CLEANUP_MESSAGE", "");
          customMessageWindow.BodyTextBlockTitle.Visibility = Visibility.Visible;
          customMessageWindow.BodyTextBlockTitle.FontWeight = FontWeights.Regular;
          customMessageWindow.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_CONTINUE_CONFIRMATION", "");
          customMessageWindow.AddButton(ButtonColors.White, "STRING_CLOSE", (EventHandler) null, (string) null, false, (object) null, true);
          customMessageWindow.AddButton(ButtonColors.Blue, "STRING_CONTINUE", (EventHandler) ((sender1, e1) => this.LaunchDiskCompaction(sender, (MouseButtonEventArgs) null)), (string) null, false, (object) null, true);
          customMessageWindow.CloseButtonHandle((Predicate<object>) null, (object) null);
          customMessageWindow.Owner = (Window) this.ParentWindow;
          customMessageWindow.ShowDialog();
        }
      }));
    }

    private void LaunchDataManager(string argument)
    {
      foreach (MainWindow mainWindow in BlueStacksUIUtils.DictWindows.Values.ToList<MainWindow>())
      {
        MainWindow.sIsClosingForBackupRestore = true;
        if (argument == "backup")
          mainWindow.CloseAllWindowAndPerform(new EventHandler(this.Closing_WindowHandlerForBackup));
        else if (argument == "restore")
          mainWindow.CloseAllWindowAndPerform(new EventHandler(this.Closing_WindowHandlerForRestore));
      }
    }

    private void LaunchDiskCompaction(object sender, MouseButtonEventArgs e)
    {
      try
      {
        this.ParentWindow.mFrontendHandler.IsRestartFrontendWhenClosed = false;
        BlueStacksUIUtils.HideUnhideBlueStacks(true);
        using (Process process = new Process())
        {
          process.StartInfo.FileName = System.IO.Path.Combine(RegistryStrings.InstallDir, "DiskCompactionTool.exe");
          process.StartInfo.Arguments = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "-vmname:{0} -relaunch", (object) this.ParentWindow.mVmName);
          process.Start();
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in starting disk compaction" + ex.ToString());
      }
    }

    internal void Closing_WindowHandlerForBackup(object sender, EventArgs e)
    {
      try
      {
        HTTPUtils.SendRequestToAgent("backup", new Dictionary<string, string>()
        {
          {
            "relaunch",
            "true"
          },
          {
            "sendResponseImmediately",
            "true"
          }
        }, this.ParentWindow.mVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp", true);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in closing window handler for backup" + ex.ToString());
      }
    }

    internal void Closing_WindowHandlerForRestore(object sender, EventArgs e)
    {
      try
      {
        Utils.KillCurrentOemProcessByName("HD-MultiInstanceManager", (string) null);
        HTTPUtils.SendRequestToAgent("restore", new Dictionary<string, string>()
        {
          {
            "relaunch",
            "true"
          },
          {
            "sendResponseImmediately",
            "true"
          }
        }, this.ParentWindow.mVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp", true);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in closing window handler for restore" + ex.ToString());
      }
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/settingswindows/backuprestoresettingscontrol.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.mDiskCleanupGrid = (Grid) target;
          break;
        case 2:
          this.mDiskCleanupBtn = (CustomButton) target;
          this.mDiskCleanupBtn.Click += new RoutedEventHandler(this.DiskCleanupBtn_Click);
          break;
        case 3:
          this.mLineSeperator = (Line) target;
          break;
        case 4:
          this.mBackupRestoreGrid = (Grid) target;
          break;
        case 5:
          this.mRestoreBtn = (CustomButton) target;
          this.mRestoreBtn.Click += new RoutedEventHandler(this.RestoreBtn_Click);
          break;
        case 6:
          this.mBackupBtn = (CustomButton) target;
          this.mBackupBtn.Click += new RoutedEventHandler(this.BackupBtn_Click);
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
