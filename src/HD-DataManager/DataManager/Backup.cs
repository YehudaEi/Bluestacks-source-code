// Decompiled with JetBrains decompiler
// Type: BlueStacks.DataManager.Backup
// Assembly: HD-DataManager, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 4AB16B4A-CAF7-4470-9488-3C5B163E3D07
// Assembly location: C:\Program Files\BlueStacks\HD-DataManager.exe

using BlueStacks.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Forms;

namespace BlueStacks.DataManager
{
  public class Backup
  {
    private ProgressWindow progress = new ProgressWindow();
    private BackgroundWorker bgBackupThread;
    private const long FiveGB = 5368709120;

    public Backup(ProgressWindow progress)
    {
      this.progress = progress;
    }

    public void BackupData(string dataDir, string path)
    {
      Logger.Info("In BackUp Data");
      string destination = (string) null;
      this.progress.CancelBtnClicked += new ProgressWindow.oncancelbuttonclick(this.CancelButtonClickAction);
      this.bgBackupThread = new BackgroundWorker();
      this.bgBackupThread.WorkerReportsProgress = true;
      this.bgBackupThread.WorkerSupportsCancellation = true;
      this.bgBackupThread.DoWork += (DoWorkEventHandler) ((obj, e) => this.BackupThread_DoWork(obj, e, dataDir, path, ref destination));
      this.bgBackupThread.ProgressChanged += (ProgressChangedEventHandler) ((obj, e) => this.BackupThread_ProgressChanged(obj, e));
      this.bgBackupThread.RunWorkerCompleted += (RunWorkerCompletedEventHandler) ((obj, e) => this.BackupThread_RunWorkerCompleted(obj, e, destination));
      this.bgBackupThread.RunWorkerAsync();
    }

    private void CancelButtonClickAction()
    {
      this.bgBackupThread.CancelAsync();
    }

    private void BackupThread_DoWork(
      object sender,
      DoWorkEventArgs e,
      string dataDir,
      string path,
      ref string destination)
    {
      Logger.Info("In Do Work");
      string dirPath = path;
      DataManagerCodes flag = DataManagerCodes.DEFAULT;
      if (path == null)
      {
        this.progress.Dispatcher.Invoke((Delegate) (() =>
        {
          this.progress.mLastRow.Height = new GridLength(20.0, GridUnitType.Star);
          this.progress.mProgressHeader.Text = LocaleStrings.GetLocalizedString("STRING_CREATING_BACKUP", "");
          this.progress.mProgressText.Text = LocaleStrings.GetLocalizedString("STRING_BACKUP_PLEASE_WAIT", "");
          flag = this.SelectBackupFolder(ref dirPath);
          if (flag != DataManagerCodes.CANCEL)
            return;
          e.Result = (object) flag;
        }));
        try
        {
          if (new DriveInfo(Path.GetPathRoot(dirPath)).AvailableFreeSpace < 5368709120L)
            this.progress.Dispatcher.Invoke((Delegate) (() =>
            {
              CustomMessageWindow customMessageWindow = new CustomMessageWindow();
              customMessageWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_INSUFFICIENT_SPACE", "");
              customMessageWindow.AddAboveBodyWarning(LocaleStrings.GetLocalizedString("STRING_BACKUP_DISK_SPACE_FULL_WARNING", ""));
              customMessageWindow.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_BACKUP_DISK_SPACE_FULL_MESSAGE", "");
              customMessageWindow.AboveBodyWarningTextBlock.Visibility = Visibility.Visible;
              customMessageWindow.AddButton(ButtonColors.Blue, "STRING_CHANGE_LOCATION", (EventHandler) ((obj, e1) =>
              {
                flag = this.SelectBackupFolder(ref dirPath);
                if (flag != DataManagerCodes.CANCEL)
                  return;
                e.Result = (object) flag;
              }), (string) null, false, (object) null, true);
              customMessageWindow.AddButton(ButtonColors.White, "STRING_CONTINUE", (EventHandler) null, (string) null, false, (object) null, true);
              customMessageWindow.Owner = (Window) this.progress;
              customMessageWindow.ShowInTaskbar = true;
              customMessageWindow.Title = Strings.ProductDisplayName;
              customMessageWindow.ShowDialog();
            }));
        }
        catch (Exception ex)
        {
          Logger.Warning("Exception while checking available space in the backup directory: " + ex.ToString());
          throw;
        }
      }
      if (!string.IsNullOrEmpty(dirPath))
      {
        if (this.CheckForValidBackupPath(dirPath))
        {
          Stats.SendMiscellaneousStatsAsync("BackupAndRestoreStats", nameof (Backup), RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, "Backup Started", (string) null, (string) null, (string) null, "Android", 0);
          e.Result = (object) this.ActualBackup(dataDir, dirPath, ref destination);
          if ((int) e.Result != -4)
            return;
          e.Cancel = true;
        }
        else
        {
          Logger.Error("Invalid Path selected path = " + dirPath);
          DataManagerUtils.ShowErrorMsg(LocaleStrings.GetLocalizedString("STRING_BACKUP_ERROR_INVALID_PATH_SELECTED", ""), LocaleStrings.GetLocalizedString("STRING_BACKUP_FAILURE", ""), this.progress);
          e.Result = (object) DataManagerCodes.ERROR_IN_CREATING_BACKUP_INVALID_PATH_SELECTED;
        }
      }
      else
      {
        Logger.Error("Path is empty");
        DataManagerUtils.ShowErrorMsg(LocaleStrings.GetLocalizedString("STRING_BACKUP_ERROR_INVALID_PATH_SELECTED", ""), LocaleStrings.GetLocalizedString("STRING_BACKUP_FAILURE", ""), this.progress);
        e.Result = (object) DataManagerCodes.NO_PATH_CHOSEN;
      }
    }

    private DataManagerCodes SelectBackupFolder(ref string dirPath)
    {
      FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
      folderBrowserDialog.Description = LocaleStrings.GetLocalizedString("STRING_SELECT_BACKUP", "");
      if (folderBrowserDialog.ShowDialog() == DialogResult.Cancel)
        return DataManagerCodes.CANCEL;
      dirPath = folderBrowserDialog.SelectedPath;
      return DataManagerCodes.DEFAULT;
    }

    private bool CheckForValidBackupPath(string backupPath)
    {
      string str = !SystemUtils.IsOs64Bit() ? Environment.ExpandEnvironmentVariables("%ProgramFiles%") : Environment.ExpandEnvironmentVariables("%ProgramFiles(x86)%");
      return !backupPath.Contains(str);
    }

    private DataManagerCodes ActualBackup(
      string srcPath,
      string destPath,
      ref string destination)
    {
      Logger.Info("In Actual Backup");
      int num = new Random().Next(0, 2147483646);
      DateTime.Now.ToString();
      string path2 = string.Format("BlueStacksBackup_{0}", (object) num);
      string dirPath = Path.Combine(destPath, path2);
      destination = dirPath;
      try
      {
        this.BackupFullBluestacksData(srcPath, dirPath);
        if (this.bgBackupThread.CancellationPending)
          return DataManagerCodes.BACKUP_CANCELLED_DURING_PROCESS;
        if (this.BackUpUserInfo(dirPath) != 0)
        {
          Backup.DeleteBackup(dirPath);
          DataManagerUtils.ShowErrorMsg(LocaleStrings.GetLocalizedString("STRING_BACKUP_FAILED", ""), LocaleStrings.GetLocalizedString("STRING_BACKUP_FAILURE", ""), this.progress);
          return DataManagerCodes.ERROR_IN_CREATING_USERINFO_FILE_WHILE_BACKUP;
        }
        if (this.bgBackupThread.CancellationPending)
          return DataManagerCodes.BACKUP_CANCELLED_DURING_PROCESS;
        this.ExportRegistry(dirPath);
        this.progress.Dispatcher.Invoke((Delegate) (() => this.progress.mProgressText.Text = string.Format("{0} {1}", (object) LocaleStrings.GetLocalizedString("STRING_BACKUP_SUCCESSFUL", ""), (object) dirPath)));
        this.bgBackupThread.ReportProgress(100);
        Thread.Sleep(2000);
        if (this.bgBackupThread.CancellationPending)
          return DataManagerCodes.BACKUP_CANCELLED_DURING_PROCESS;
        Logger.Info("Backup successfully taken");
        Stats.SendMiscellaneousStatsSync("BackupAndRestoreStats", nameof (Backup), RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, "Backup Completed", (string) null, (string) null, (string) null, (string) null, 15000);
        return DataManagerCodes.SUCCESS;
      }
      catch (Exception ex)
      {
        Logger.Error("Error in taking Backup " + ex.ToString());
        DataManagerUtils.ShowErrorMsg(LocaleStrings.GetLocalizedString("STRING_BACKUP_FAILED", ""), LocaleStrings.GetLocalizedString("STRING_BACKUP_FAILURE", ""), this.progress);
        Backup.DeleteDirectory(dirPath);
        return DataManagerCodes.EXCEPTION_IN_BACKUP;
      }
    }

    private void BackupThread_ProgressChanged(object sender, ProgressChangedEventArgs e)
    {
      Logger.Info("In Progress Changed");
      this.progress.Dispatcher.Invoke((Delegate) (() => this.progress.mProgressBar.Value = (double) e.ProgressPercentage));
    }

    private void BackupThread_RunWorkerCompleted(
      object sender,
      RunWorkerCompletedEventArgs e,
      string dirPath)
    {
      Logger.Info("In RunWorkerCompleted");
      if (e.Cancelled)
      {
        BackgroundWorker backgroundWorker = new BackgroundWorker();
        backgroundWorker.DoWork += (DoWorkEventHandler) ((obj, e1) => this.CancelThread_DoWork(obj, e1, dirPath));
        backgroundWorker.RunWorkerAsync();
      }
      else
      {
        if ((int) e.Result != 0)
          Stats.SendMiscellaneousStatsSync("BackupAndRestoreStats", nameof (Backup), RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, "Backup Failed, Exit Code: " + ((int) e.Result).ToString(), (string) null, (string) null, (string) null, (string) null, 15000);
        Logger.Info("Exiting data manager with the exit code: " + ((int) e.Result).ToString());
        Environment.Exit((int) e.Result);
      }
    }

    private void CancelThread_DoWork(object sender, DoWorkEventArgs e, string dirPath)
    {
      Logger.Info("In Cancel DoWork");
      this.progress.Dispatcher.Invoke((Delegate) (() =>
      {
        this.progress.mBtnGrid.Visibility = Visibility.Hidden;
        this.progress.mProgressHeader.Text = LocaleStrings.GetLocalizedString("STRING_DELETING_BACKUP", "");
        this.progress.mProgressText.Text = LocaleStrings.GetLocalizedString("STRING_DELETING_BACKUP_INFO", "");
      }));
      Backup.DeleteBackupOnCancel(dirPath);
      Thread.Sleep(3000);
      Stats.SendMiscellaneousStatsSync("BackupAndRestoreStats", nameof (Backup), RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, "Backup Cancelled", (string) null, (string) null, (string) null, (string) null, 15000);
      Logger.Info("Exiting data manager with the exit code: -4");
      Environment.Exit(-4);
    }

    private void BackupFullBluestacksData(string srcPath, string dirPath)
    {
      Logger.Info("In BackupFullBluestacksData");
      this.MakeDirectory(dirPath);
      Utils.KillCurrentOemProcessByName("BstkSVC", (string) null);
      this.bgBackupThread.ReportProgress(5);
      if (this.bgBackupThread.CancellationPending)
        return;
      int progressValue = 10;
      this.DirectoryCopyForBackup(ref progressValue, srcPath, Path.Combine(dirPath, "Engine"), true);
      if (this.bgBackupThread.CancellationPending)
        return;
      this.bgBackupThread.ReportProgress(70);
    }

    private void MakeDirectory(string path)
    {
      if (Directory.Exists(path))
        return;
      Directory.CreateDirectory(path);
    }

    private void DirectoryCopyForBackup(
      ref int progressValue,
      string sourceDirName,
      string destDirName,
      bool copySubDirs)
    {
      DirectoryInfo directoryInfo1 = new DirectoryInfo(sourceDirName);
      if (!directoryInfo1.Exists)
        throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " + sourceDirName);
      DirectoryInfo[] directories = directoryInfo1.GetDirectories();
      if (!Directory.Exists(destDirName))
        Directory.CreateDirectory(destDirName);
      if (this.bgBackupThread.CancellationPending)
        return;
      List<string> stringList = new List<string>()
      {
        "fastboot.vdi",
        "root.vdi",
        "oem.cfg"
      };
      foreach (FileInfo file1 in directoryInfo1.GetFiles())
      {
        FileInfo file = file1;
        if (stringList.FindIndex((Predicate<string>) (x => x.Equals(file.Name, StringComparison.OrdinalIgnoreCase))) != -1)
        {
          Logger.Info("Ignoring: {0}", (object) file.FullName);
        }
        else
        {
          string destFileName = Path.Combine(destDirName, file.Name);
          file.CopyTo(destFileName, true);
        }
      }
      this.bgBackupThread.ReportProgress(progressValue);
      if (this.bgBackupThread.CancellationPending || !copySubDirs)
        return;
      foreach (DirectoryInfo directoryInfo2 in directories)
      {
        if (!directoryInfo2.ToString().Contains("Logs") && !directoryInfo2.ToString().Contains(nameof (Backup)))
        {
          string destDirName1 = Path.Combine(destDirName, directoryInfo2.Name);
          progressValue += 5;
          this.DirectoryCopyForBackup(ref progressValue, directoryInfo2.FullName, destDirName1, copySubDirs);
        }
      }
    }

    private int BackUpUserInfo(string dirPath)
    {
      Logger.Info("BackUpUserInfo start");
      try
      {
        string path = Path.Combine(dirPath, string.Format(".{0}", (object) "Bst_UserInfo_Backup"));
        StreamWriter streamWriter = new StreamWriter(path);
        streamWriter.Write(string.Format("UserDefinedDir {0}", (object) RegistryManager.Instance.UserDefinedDir));
        streamWriter.WriteLine();
        streamWriter.Write(string.Format("DataDir {0}", (object) RegistryStrings.DataDir));
        streamWriter.WriteLine();
        streamWriter.Write(string.Format("InstallDir {0}", (object) RegistryStrings.InstallDir));
        streamWriter.WriteLine();
        streamWriter.Write(string.Format("EngineVersion {0}", (object) RegistryManager.Instance.Version));
        streamWriter.WriteLine();
        streamWriter.Write(string.Format("UsersPath {0}", (object) Environment.GetEnvironmentVariable("userprofile")));
        streamWriter.WriteLine();
        streamWriter.Close();
        Logger.Info("BackUpUserInfo end");
        File.SetAttributes(path, File.GetAttributes(path) | FileAttributes.ReadOnly);
        Logger.Info("Marked read only flag in file info");
        return 0;
      }
      catch (Exception ex)
      {
        Logger.Error("Error in creating userinfo file " + ex.ToString());
        return -6;
      }
    }

    private static void DeleteBackup(string dirPath)
    {
      Logger.Info("Deleting Backup");
      Backup.DeleteDirectory(dirPath);
    }

    private static void DeleteDirectory(string targetDir)
    {
      try
      {
        Directory.Delete(targetDir, true);
      }
      catch (Exception ex1)
      {
        Logger.Info("Got exception when deleting {0} , err:{1}", (object) targetDir, (object) ex1.ToString());
        foreach (string file in Directory.GetFiles(targetDir))
        {
          File.SetAttributes(file, FileAttributes.Normal);
          File.Delete(file);
        }
        foreach (string directory in Directory.GetDirectories(targetDir))
          Backup.DeleteDirectory(directory);
        try
        {
          Directory.Delete(targetDir, true);
        }
        catch (IOException ex2)
        {
          Thread.Sleep(100);
          try
          {
            Directory.Delete(targetDir, true);
          }
          catch
          {
          }
        }
        catch (UnauthorizedAccessException ex2)
        {
          Thread.Sleep(100);
          try
          {
            Directory.Delete(targetDir, true);
          }
          catch
          {
          }
        }
      }
    }

    private void ExportRegistry(string destPath)
    {
      Utils.RunCmd("reg.exe", string.Format("EXPORT HKLM\\{0} \"{1}\"", (object) (RegistryManager.Instance.BaseKeyPath + "\\Config"), (object) Path.Combine(destPath, "RegHKLMConfig.reg")), (string) null);
      Utils.RunCmd("reg.exe", string.Format("EXPORT HKLM\\{0} \"{1}\"", (object) (RegistryManager.Instance.BaseKeyPath + "\\Guests"), (object) Path.Combine(destPath, "BlueStacksGuest.reg")), (string) null);
      Utils.RunCmd("reg.exe", string.Format("EXPORT HKLM\\{0} \"{1}\"", (object) (RegistryManager.Instance.BaseKeyPath + "\\User"), (object) Path.Combine(destPath, "RegHKLMUser.reg")), (string) null);
      Utils.RunCmd("reg.exe", string.Format("EXPORT HKLM\\{0} \"{1}\"", (object) (RegistryManager.Instance.BaseKeyPath + "\\Client"), (object) Path.Combine(destPath, "RegHKLMClient.reg")), (string) null);
    }

    private static void DeleteBackupOnCancel(string dirPath)
    {
      Logger.Info("Cancel Button Pressed");
      Backup.DeleteBackup(dirPath);
    }
  }
}
