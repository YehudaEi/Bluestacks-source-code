// Decompiled with JetBrains decompiler
// Type: BlueStacks.DataManager.Restore
// Assembly: HD-DataManager, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 4AB16B4A-CAF7-4470-9488-3C5B163E3D07
// Assembly location: C:\Program Files\BlueStacks\HD-DataManager.exe

using BlueStacks.Common;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Web;
using System.Windows;
using System.Windows.Forms;
using System.Xml;

namespace BlueStacks.DataManager
{
  public class Restore
  {
    internal static List<string> sVmDisplayNameList = new List<string>();
    public static bool sContinueButtonClickedFlag = false;
    public static Dictionary<string, string> sBackUpInfoDict = (Dictionary<string, string>) null;
    public static Dictionary<string, object> sOldConfigRegistryValues = new Dictionary<string, object>();
    private ProgressWindow progress = new ProgressWindow();
    private const string ROOT_VDI_UUID = "{fca296ce-8268-4ed7-a57f-d32ec11ab304}";
    private BackgroundWorker bgRestoreThread;
    private bool mOldForceDedicatedGPU;
    private bool mDeleteOldCustomKeyMappingsWhileRestore;
    private bool mConversionToImap14Required;

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern bool MoveFileEx(
      string lpExistingFileName,
      string lpNewFileName,
      Restore.MoveFileFlags dwFlags);

    public static event EventHandler ContinueBtnhandler = null;

    public Restore(ProgressWindow progress)
    {
      this.progress = progress;
    }

    public void RestoreData(string dataDir, string dirPath)
    {
      Logger.Info("In Restore Data");
      this.bgRestoreThread = new BackgroundWorker()
      {
        WorkerReportsProgress = true
      };
      this.bgRestoreThread.DoWork += (DoWorkEventHandler) ((obj, e) => this.RestoreThread_DoWork(obj, e, dataDir, dirPath));
      this.bgRestoreThread.ProgressChanged += (ProgressChangedEventHandler) ((obj, e) => this.RestoreThread_ProgressChanged(obj, e));
      this.bgRestoreThread.RunWorkerCompleted += (RunWorkerCompletedEventHandler) ((obj, e) => this.RestoreThread_RunWorkerCompleted(obj, e));
      this.bgRestoreThread.RunWorkerAsync();
    }

    private void RestoreThread_DoWork(
      object sender,
      DoWorkEventArgs e,
      string dataDir,
      string dirPath)
    {
      Logger.Info("In Do Work");
      DataManagerCodes flag = DataManagerCodes.DEFAULT;
      if (dirPath == null)
      {
        this.progress.Dispatcher.Invoke((Delegate) (() =>
        {
          this.progress.mProgressHeader.Text = LocaleStrings.GetLocalizedString("STRING_RESTORING_BACKUP", "");
          this.progress.mProgressText.Text = LocaleStrings.GetLocalizedString("STRING_RESTORE_PLEASE_WAIT", "");
          this.progress.MinimizeBtn.Visibility = Visibility.Visible;
          FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog()
          {
            Description = LocaleStrings.GetLocalizedString("STRING_SELECT_RESTORE", "")
          };
          if (folderBrowserDialog.ShowDialog() == DialogResult.Cancel)
            flag = DataManagerCodes.CANCEL;
          dirPath = folderBrowserDialog.SelectedPath;
        }));
        if (flag == DataManagerCodes.CANCEL)
        {
          e.Result = (object) flag;
          return;
        }
      }
      if (!string.IsNullOrEmpty(dirPath))
      {
        if (Directory.Exists(dirPath))
        {
          if (!this.CheckIfRestorePossible(dirPath))
          {
            Logger.Error("Path is incorrect");
            DataManagerUtils.ShowErrorMsg(LocaleStrings.GetLocalizedString("STRING_INCORRECT_PATH", ""), LocaleStrings.GetLocalizedString("STRING_RESTORE_FAILURE", ""), this.progress);
            e.Result = (object) DataManagerCodes.WRONG_PATH_CHOSEN;
          }
          else if (!this.CheckValidOEM(dirPath))
          {
            Logger.Error("Invalid OEM passed, showing prompt to restart");
            DataManagerUtils.ShowErrorMsg(LocaleStrings.GetLocalizedString("STRING_DATA_MANAGER_INVALID_OEM", ""), LocaleStrings.GetLocalizedString("STRING_RESTORE_FAILURE", ""), this.progress);
            e.Result = (object) DataManagerCodes.INVALID_OEM;
          }
          else
          {
            long diskSpaceOfDrive = IOUtils.GetAvailableDiskSpaceOfDrive(RegistryStrings.DataDir);
            long num = IOUtils.GetDirectorySize(dirPath) + 1073741824L;
            if (diskSpaceOfDrive < num)
            {
              Logger.Error("Available space: {0} < Backup size {1}, exiting restore", (object) diskSpaceOfDrive, (object) num);
              DataManagerUtils.ShowErrorMsg(string.Format("{0} {1}GB", (object) LocaleStrings.GetLocalizedString("STRING_COULD_NOT_RESOTRE_INSUFFICIENT_SPACE", ""), (object) Utils.RoundUp((double) num * 1.0 / 1073741824.0, 2)), LocaleStrings.GetLocalizedString("STRING_RESTORE_FAILURE", ""), this.progress);
              e.Result = (object) DataManagerCodes.INSUFFICIENT_DISK_SPACE;
            }
            else
            {
              this.GenerateBackUpInfoDictionary(dirPath);
              if (Restore.sBackUpInfoDict == null)
              {
                Logger.Error("Bst_UserInfo_file is missing");
                DataManagerUtils.ShowErrorMsg(LocaleStrings.GetLocalizedString("STRING_DATA_MANAGER_INFO_FILE_MISSING", ""), LocaleStrings.GetLocalizedString("STRING_RESTORE_FAILURE", ""), this.progress);
                e.Result = (object) DataManagerCodes.EXIT_AS_USERINFO_FILE_MISSING_IN_BACKUP;
              }
              else if (this.CheckBackUpVersionStatus("2.52.66.8704") == BackupVersionStatus.Lesser)
              {
                Logger.Info("Backup version less than minimum version required, restore not possible");
                DataManagerUtils.ShowErrorMsg(LocaleStrings.GetLocalizedString("STRING_RESTORE_NOT_POSSIBLE", ""), LocaleStrings.GetLocalizedString("STRING_RESTORE_FAILURE", ""), this.progress);
                e.Result = (object) DataManagerCodes.BACKUP_FROM_KK_BUILD;
              }
              else
              {
                if (this.CheckBackUpVersionStatus(RegistryManager.Instance.Version) == BackupVersionStatus.Lesser)
                {
                  Logger.Info("Backup version lesser, restore possible showing warning");
                  Restore.sContinueButtonClickedFlag = false;
                  DataManagerUtils.ShowWarningMsg(LocaleStrings.GetLocalizedString("STRING_RESTORE_WARNING", ""), LocaleStrings.GetLocalizedString("STRING_BEFORE_YOU_RESTORE", ""), this.progress, new EventHandler(this.ContinueBtnhandler_action));
                  if (!Restore.sContinueButtonClickedFlag)
                  {
                    e.Result = (object) DataManagerCodes.USER_CANCELLED_ON_RESTORE_WARNING;
                    return;
                  }
                }
                else
                {
                  if (this.CheckBackUpVersionStatus(RegistryManager.Instance.Version) == BackupVersionStatus.Greater)
                  {
                    Logger.Info("Backup version greater, restore not possible");
                    DataManagerUtils.ShowErrorMsg(LocaleStrings.GetLocalizedString("STRING_RESTORE_NOT_PERMITTED", ""), LocaleStrings.GetLocalizedString("STRING_RESTORE_FAILURE", ""), this.progress);
                    e.Result = (object) DataManagerCodes.BACKUP_VERSION_GREATER_THAN_CURRENT_BUILD_VERSION;
                    return;
                  }
                  if (this.CheckBackUpVersionStatus(RegistryManager.Instance.Version) == BackupVersionStatus.Equal)
                    Logger.Info("Version Same Restore Continued");
                }
                if (this.CheckBackUpVersionStatus("4.30.33.1590") == BackupVersionStatus.Lesser)
                {
                  Logger.Error("Version restored is before the IMAP version, so user cfg files will not be restored");
                  Restore.ContinueBtnhandler += new EventHandler(this.ContinueBtnhandler_action);
                  Restore.sContinueButtonClickedFlag = false;
                  DataManagerUtils.ShowWarningMsg(LocaleStrings.GetLocalizedString("STRING_CAUTION_CUSTOM_CFG_UNUSABLE_AFTER_RESTORE", ""), LocaleStrings.GetLocalizedString("STRING_BEFORE_YOU_RESTORE", ""), this.progress, Restore.ContinueBtnhandler);
                  if (!Restore.sContinueButtonClickedFlag)
                  {
                    e.Result = (object) DataManagerCodes.USER_CANCELLED_DUE_TO_CFG_FILE;
                    return;
                  }
                  this.mDeleteOldCustomKeyMappingsWhileRestore = true;
                }
                if (this.CheckBackUpVersionStatus("4.140.00.0000") == BackupVersionStatus.Lesser)
                {
                  Logger.Info("Conversion to IMAP 14 parser version required");
                  this.mConversionToImap14Required = true;
                }
                Stats.SendMiscellaneousStatsAsync("BackupAndRestoreStats", nameof (Restore), RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, "Restore Started", (string) null, (string) null, (string) null, "Android", 0);
                e.Result = (object) this.ActualRestore(dirPath, dataDir);
              }
            }
          }
        }
        else
        {
          Logger.Error("Path is incorrect");
          DataManagerUtils.ShowErrorMsg(LocaleStrings.GetLocalizedString("STRING_INCORRECT_PATH", ""), LocaleStrings.GetLocalizedString("STRING_RESTORE_FAILURE", ""), this.progress);
          e.Result = (object) DataManagerCodes.WRONG_PATH_CHOSEN;
        }
      }
      else
      {
        Logger.Error("Path is empty");
        DataManagerUtils.ShowErrorMsg(LocaleStrings.GetLocalizedString("STRING_INCORRECT_PATH", ""), LocaleStrings.GetLocalizedString("STRING_RESTORE_FAILURE", ""), this.progress);
        e.Result = (object) DataManagerCodes.WRONG_PATH_CHOSEN;
      }
    }

    private bool CheckIfRestorePossible(string backupDirPath)
    {
      return Directory.Exists(Path.Combine(backupDirPath, "Engine"));
    }

    private bool CheckValidOEM(string backupDirPath)
    {
      try
      {
        Logger.Info("Checking if backup matched current OEM: " + Oem.Instance.OEM);
        string str = string.Format("\"{0}\"", (object) Oem.Instance.OEM);
        foreach (string readAllLine in File.ReadAllLines(Path.Combine(backupDirPath, "RegHKLMConfig.reg")))
        {
          if (readAllLine.StartsWith("\"Oem"))
          {
            string[] strArray = readAllLine.Split('=');
            Logger.Info("Backup OEM: " + strArray[1]);
            if (!str.Equals(strArray[1]))
              return false;
          }
        }
        return true;
      }
      catch (Exception ex)
      {
        Logger.Error(ex.ToString());
        Logger.Error("Couldn't parse OEM, continuing with restore");
        return false;
      }
    }

    private void GenerateBackUpInfoDictionary(string dirPath)
    {
      string path = Path.Combine(dirPath, string.Format(".{0}", (object) "Bst_UserInfo_Backup"));
      if (!File.Exists(path))
        return;
      if (Restore.sBackUpInfoDict == null)
        Restore.sBackUpInfoDict = new Dictionary<string, string>();
      foreach (string readAllLine in File.ReadAllLines(path))
      {
        string[] strArray = readAllLine.Split(" ".ToCharArray(), 2);
        Restore.sBackUpInfoDict.Add(strArray[0], strArray[1]);
      }
    }

    private BackupVersionStatus CheckBackUpVersionStatus(
      string versionToCheckAgainst)
    {
      System.Version version1 = new System.Version(Restore.sBackUpInfoDict["EngineVersion"].ToString());
      System.Version version2 = new System.Version(versionToCheckAgainst);
      System.Version version3 = version1;
      string str1 = (object) version3 != null ? version3.ToString() : (string) null;
      System.Version version4 = version2;
      string str2 = (object) version4 != null ? version4.ToString() : (string) null;
      Logger.Info("Version check: " + str1 + " - " + str2);
      if (version1 == version2)
        return BackupVersionStatus.Equal;
      return version1 < version2 ? BackupVersionStatus.Lesser : BackupVersionStatus.Greater;
    }

    private void ContinueBtnhandler_action(object sender, EventArgs e)
    {
      Logger.Info("Continue Button Clicked");
      Restore.sContinueButtonClickedFlag = true;
    }

    private DataManagerCodes ActualRestore(string backupDir, string dataDir)
    {
      Logger.Info("Restoring data backup");
      string str1 = Path.Combine(backupDir, "Engine");
      string path2 = Path.Combine("Android", "SDCard.vdi");
      bool isBackupFromKK = File.Exists(Path.Combine(str1, path2));
      string str2 = Path.Combine(RegistryManager.Instance.UserDefinedDir, Path.GetRandomFileName());
      Logger.Info("srcPath = {0} and destPath = {1}", (object) str1, (object) dataDir);
      if (File.Exists(Path.Combine(backupDir, string.Format(".{0}", (object) "Bst_UserInfo_Backup"))))
      {
        try
        {
          Utils.KillCurrentOemProcessByName("BstkSVC", (string) null);
          this.bgRestoreThread.ReportProgress(10);
          if (Directory.Exists(dataDir))
          {
            bool flag = this.MoveContentFromDirectoryWithFilter(dataDir, str2);
            this.bgRestoreThread.ReportProgress(30);
            if (!flag)
            {
              DataManagerUtils.ShowErrorMsg(LocaleStrings.GetLocalizedString("STRING_RESTORE_FAILED", ""), "", this.progress);
              this.MoveContentFromDirectoryWithFilter(str2, dataDir);
              return DataManagerCodes.ERROR_IN_MOVING_EXISTING_DATA_TO_TEMP;
            }
            string bootParameters = RegistryManager.Instance.Guest["Android"].BootParameters;
            this.DirectoryCopyForRestore(str1, dataDir, str2);
            this.bgRestoreThread.ReportProgress(50);
            string targetDir = Path.Combine(RegistryStrings.DataDir, "UserData\\Library\\");
            string path = Path.Combine(RegistryStrings.DataDir, "UserData\\Library\\Icons");
            if (Directory.Exists(path))
            {
              this.CopyAllIconsInGadgetFolder(path);
              this.DeleteDirectory(targetDir);
            }
            this.CopyAndroidDataFromOld(str2, dataDir);
            this.bgRestoreThread.ReportProgress(60);
            Restore.CopyOemCfgFileFromOld(str2, dataDir);
            this.bgRestoreThread.ReportProgress(65);
            this.CopyUserDataFromOld(str2, dataDir);
            if (this.mConversionToImap14Required)
              this.HandlingForUserCfgFile();
            this.bgRestoreThread.ReportProgress(70);
            this.CopyEngineDataFromTemp(str2, dataDir);
            this.bgRestoreThread.ReportProgress(75);
            this.RenamingRegistryBeforeRestore();
            List<string> list = ((IEnumerable<string>) RegistryManager.Instance.VmList).ToList<string>();
            this.DeletingExistingInstancesRegistry("Software\\BlueStacks" + BlueStacks.Common.Strings.GetOemTag() + "\\Guests", list);
            this.RestoreOldConfigRegistryValue();
            this.ImportRegistry(backupDir, bootParameters, isBackupFromKK);
            RegistryManager.ClearRegistryMangerInstance();
            this.UpdateGuestConfigRegistryAfterRestore(backupDir);
            this.UpdateBlueStacksConfigRegistryAfterRestore();
            this.bgRestoreThread.ReportProgress(78);
            int work = (int) this.ConfiguringFilesForTheRestoreToWork(backupDir, dataDir);
            this.FixAndroidbstkInAllVms(dataDir);
            this.DeletingBstkPrevFilesIfExists(dataDir);
            this.DeletingExistingInstancesRegistry("Software\\BlueStacks" + BlueStacks.Common.Strings.GetOemTag() + "\\Guests.old", list);
            RegistryManager.ClearRegistryMangerInstance();
            this.bgRestoreThread.ReportProgress(85);
            this.DeleteRecursiveWithPendMove(str2);
            Restore.sVmDisplayNameList = this.GetVmDesktopShortcutIconList();
            this.DeleteShortcuts(Restore.sVmDisplayNameList);
            this.DeleteAppIconsShortCuts();
            this.bgRestoreThread.ReportProgress(95);
            this.CreateAppIconsShortCuts();
            this.bgRestoreThread.ReportProgress(100);
            this.progress.ExitWindow();
            Logger.Info("Data Successfully Restored");
            Stats.SendMiscellaneousStatsSync("BackupAndRestoreStats", nameof (Restore), RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, "Restore Completed", (string) null, (string) null, (string) null, (string) null, 15000);
            return DataManagerCodes.SUCCESS;
          }
          DataManagerUtils.ShowErrorMsg(LocaleStrings.GetLocalizedString("STRING_RESTORE_FAILED", ""), "", this.progress);
          return DataManagerCodes.ERROR_IN_MOVING_EXISTING_DATA_TO_TEMP;
        }
        catch (Exception ex)
        {
          Logger.Error("Exception restoring {0}", (object) ex.ToString());
          DataManagerUtils.ShowErrorMsg(LocaleStrings.GetLocalizedString("STRING_RESTORE_FAILED", ""), "", this.progress);
          return DataManagerCodes.EXCEPTION_IN_RESTORE;
        }
      }
      else
      {
        Logger.Error("User Backup info file not exist");
        DataManagerUtils.ShowErrorMsg(LocaleStrings.GetLocalizedString("STRING_RESTORE_FAILED", ""), "", this.progress);
        return DataManagerCodes.BACKUP_INFO_FILE_NOT_EXIST;
      }
    }

    private void HandlingForUserCfgFile()
    {
      try
      {
        Process process = new Process();
        process.StartInfo.FileName = Path.Combine(RegistryManager.Instance.InstallDir, "Bluestacks.exe");
        process.StartInfo.Arguments = "--mergeCfg --newPDPath " + RegistryManager.Instance.UserDefinedDir;
        process.Start();
        process.WaitForExit();
      }
      catch (Exception ex)
      {
        Logger.Error("Error while launching Bluestacks exe err: " + ex.ToString());
      }
    }

    private void RestoreOldConfigRegistryValue()
    {
      Restore.sOldConfigRegistryValues.Add("AgentServerPort", (object) RegistryManager.Instance.AgentServerPort);
      Restore.sOldConfigRegistryValues.Add("PartnerServerPort", (object) RegistryManager.Instance.PartnerServerPort);
      Restore.sOldConfigRegistryValues.Add("MultiInstanceServerPort", (object) RegistryManager.Instance.MultiInstanceServerPort);
      Restore.sOldConfigRegistryValues.Add("InstallID", (object) RegistryManager.Instance.InstallID);
      this.mOldForceDedicatedGPU = RegistryManager.Instance.ForceDedicatedGPU;
    }

    private void UpdateBlueStacksConfigRegistryAfterRestore()
    {
      foreach (KeyValuePair<string, object> configRegistryValue in Restore.sOldConfigRegistryValues)
      {
        PropertyInfo property = typeof (RegistryManager).GetProperty(configRegistryValue.Key);
        object obj = Convert.ChangeType(configRegistryValue.Value, System.Type.GetTypeCode(property.PropertyType));
        property.SetValue((object) RegistryManager.Instance, obj, (object[]) null);
      }
      RegistryManager.Instance.VmId = Utils.GetMaxVmIdFromVmList(RegistryManager.Instance.VmList);
      this.HandleForceDedicatedGPU();
    }

    private void HandleForceDedicatedGPU()
    {
      try
      {
        if (this.mOldForceDedicatedGPU == RegistryManager.Instance.ForceDedicatedGPU)
          return;
        bool flag = ForceDedicatedGPU.ToggleDedicatedGPU(RegistryManager.Instance.ForceDedicatedGPU, (string) null);
        Logger.Info("Is toogle dedicated GPU compatible with the given machine: " + flag.ToString());
        if (flag)
          return;
        RegistryManager.Instance.ForceDedicatedGPU = this.mOldForceDedicatedGPU;
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in running force dedicated GPU binary: " + ex.ToString());
      }
    }

    private void UpdateGuestConfigRegistryAfterRestore(string sourceDirName)
    {
      foreach (string vm in RegistryManager.Instance.VmList)
      {
        string registryPath1 = Path.Combine(Path.Combine(BlueStacks.Common.Strings.RegistryBaseKeyPath + "\\Guests", vm), "Config");
        string registryPath2 = Path.Combine(Path.Combine(BlueStacks.Common.Strings.RegistryBaseKeyPath + "\\Guests.old", vm), "Config");
        if (File.Exists(Path.Combine(sourceDirName, "bgp.reg")))
          RegistryManager.Instance.Guest[vm].IsOneTimeSetupDone = true;
        if (RegistryUtils.GetRegistryValue(registryPath1, "IsGoogleSigninDone", (object) null, RegistryKeyKind.HKEY_LOCAL_MACHINE) == null)
          RegistryManager.Instance.Guest[vm].IsGoogleSigninDone = RegistryManager.Instance.Guest[vm].IsOneTimeSetupDone;
        Utils.UpdateValueInBootParams("ApiToken", RegistryManager.Instance.ApiToken, vm, true, "bgp");
        RegistryManager.Instance.Guest[vm].FrontendServerPort = (int) RegistryUtils.GetRegistryValue(registryPath2, "FrontendServerPort", (object) 2881, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      }
    }

    private void RestoreThread_ProgressChanged(object sender, ProgressChangedEventArgs e)
    {
      Logger.Info("In Progress Changed");
      this.progress.Dispatcher.Invoke((Delegate) (() => this.progress.mProgressBar.Value = (double) e.ProgressPercentage));
    }

    private void RestoreThread_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      Logger.Info("In Restore thread RunWorkerCompleted");
      int result;
      if ((int) e.Result != 0)
      {
        result = (int) e.Result;
        Stats.SendMiscellaneousStatsSync("BackupAndRestoreStats", nameof (Restore), RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, "Restore Failed, Exit Code: " + result.ToString(), (string) null, (string) null, (string) null, (string) null, 15000);
      }
      result = (int) e.Result;
      Logger.Info("Exiting data manager with the exit code: " + result.ToString());
      Environment.Exit((int) e.Result);
    }

    private bool MoveContentFromDirectoryWithFilter(string srcDir, string dstDir)
    {
      Logger.Info("Moving directory {0} to {1}", (object) srcDir, (object) dstDir);
      if (!Directory.Exists(dstDir))
      {
        try
        {
          string directoryName = Path.GetDirectoryName(dstDir);
          if (!Directory.Exists(directoryName))
            Directory.CreateDirectory(directoryName);
        }
        catch (Exception ex)
        {
          Logger.Error("Got exception in creating directory ex:{0}", (object) ex.ToString());
          return false;
        }
      }
      try
      {
        if (!Directory.Exists(dstDir))
          Directory.CreateDirectory(dstDir);
        foreach (string file in Directory.GetFiles(srcDir))
        {
          if (!file.Contains("\\Logs") || file.Contains("\\Android"))
          {
            FileInfo fileInfo = new FileInfo(file);
            string str = Path.Combine(dstDir, fileInfo.Name);
            try
            {
              if (File.Exists(str))
              {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(str);
              }
              File.Move(file, str);
            }
            catch (Exception ex)
            {
              Logger.Error("Exception in file move {0} to {1}. ex:{2}", (object) file, (object) str, (object) ex.ToString());
              return false;
            }
          }
        }
        foreach (string directory in Directory.GetDirectories(srcDir))
        {
          DirectoryInfo directoryInfo = new DirectoryInfo(directory);
          string dstDir1 = Path.Combine(dstDir, directoryInfo.Name);
          if (!this.MoveContentFromDirectoryWithFilter(Path.Combine(srcDir, directoryInfo.Name), dstDir1))
            return false;
        }
        Logger.Info("Starting to delete directory in the programData folder " + srcDir);
        try
        {
          if (Directory.Exists(srcDir))
          {
            try
            {
              Directory.Delete(srcDir, true);
            }
            catch (Exception ex)
            {
              Logger.Error("Exception in deleting directory: " + ex.ToString());
            }
          }
        }
        catch (Exception ex)
        {
          Logger.Error("Exception in checking directory existence: " + ex.ToString());
        }
        return true;
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in directory copy: " + ex.ToString());
        return false;
      }
    }

    private void DirectoryCopyForRestore(
      string sourceDirName,
      string destDirName,
      string tmpFolder)
    {
      DirectoryInfo directoryInfo1 = new DirectoryInfo(sourceDirName);
      if (!directoryInfo1.Exists)
        throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " + sourceDirName);
      DirectoryInfo[] directories = directoryInfo1.GetDirectories();
      if (!Directory.Exists(destDirName))
        Directory.CreateDirectory(destDirName);
      List<string> stringList = new List<string>()
      {
        "fastboot.vdi",
        "root.vdi",
        "oem.cfg",
        "Android.bstk.in",
        "Bst_UserInfo_Backup"
      };
      foreach (FileInfo file1 in directoryInfo1.GetFiles())
      {
        FileInfo file = file1;
        if (stringList.FindIndex((Predicate<string>) (x => x.Equals(file.Name, StringComparison.OrdinalIgnoreCase))) != -1)
          Logger.Info("Not copying {0} from backup", (object) file.FullName);
        else if (!directoryInfo1.FullName.ToString().Contains("InputMapper") || directoryInfo1.FullName.ToString().Contains("UserFiles") || directoryInfo1.FullName.ToString().Contains("UserScripts"))
        {
          if (directoryInfo1.FullName.ToString().Contains("InputMapper"))
          {
            if (directoryInfo1.FullName.ToString().Contains("UserFiles"))
            {
              try
              {
                string str = Path.Combine(destDirName, file.Name);
                JObject jobject1 = JObject.Parse(File.ReadAllText(file.FullName));
                if (this.mConversionToImap14Required)
                {
                  if (!ConfigConverter.Convert(file.FullName, str, "17", false, true))
                  {
                    Logger.Warning("Could not convert file {0}. Copy file to destination folder", (object) file.Name);
                    file.CopyTo(str, true);
                    continue;
                  }
                  continue;
                }
                int? configVersion = ConfigConverter.GetConfigVersion(jobject1);
                int num = 16;
                if (configVersion.GetValueOrDefault() < num & configVersion.HasValue && Utils.CheckIfImagesArrayPresentInCfg(jobject1))
                {
                  JObject jobject2 = jobject1;
                  foreach (JObject scheme in (IEnumerable<JToken>) jobject1["ControlSchemes"])
                    scheme["Images"] = (JToken) ConfigConverter.ConvertImagesArrayForPV16(scheme);
                  jobject2["MetaData"][(object) "Comment"] = (JToken) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Generated automatically from ver {0}", (object) (int) jobject2["MetaData"][(object) "ParserVersion"]);
                  jobject2["MetaData"][(object) "ParserVersion"] = (JToken) 16;
                  if (jobject2 != null)
                  {
                    File.WriteAllText(str, jobject2.ToString());
                    continue;
                  }
                  continue;
                }
                file.CopyTo(str, true);
                continue;
              }
              catch (Exception ex)
              {
                Logger.Error("Some error while copying Userfiles err: " + ex.ToString());
                continue;
              }
            }
          }
          string destFileName = Path.Combine(destDirName, file.Name);
          file.CopyTo(destFileName, true);
        }
        else
          break;
      }
      foreach (DirectoryInfo directoryInfo2 in directories)
      {
        if (this.mDeleteOldCustomKeyMappingsWhileRestore && directoryInfo2.ToString().Contains("InputMapper"))
        {
          string path = Path.Combine(destDirName, "InputMapper\\UserFiles");
          Logger.Info("Creating Empty Directory: " + path);
          if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        }
        else if (!directoryInfo2.ToString().Contains("Logs") && !directoryInfo2.ToString().Contains("Backup") && !directoryInfo2.ToString().Contains("Locales"))
        {
          string destDirName1 = Path.Combine(destDirName, directoryInfo2.Name);
          this.DirectoryCopyForRestore(directoryInfo2.FullName, destDirName1, tmpFolder);
        }
      }
    }

    private void CopyAllIconsInGadgetFolder(string path)
    {
      Logger.Info("In CopyingAllIconsInGadgetFolder");
      try
      {
        string str = Path.Combine(RegistryStrings.DataDir, "UserData\\Gadget");
        if (Directory.Exists(str))
        {
          foreach (FileInfo file in new DirectoryInfo(path).GetFiles())
            file.CopyTo(Path.Combine(str, file.Name));
        }
        else
          Logger.Warning("Gadget folder don't exists");
      }
      catch (Exception ex)
      {
        Logger.Error("Error in copying icon files to Gadget folder. Err: " + ex.ToString());
      }
    }

    private void DeleteDirectory(string targetDir)
    {
      try
      {
        Directory.Delete(targetDir, true);
      }
      catch (Exception ex1)
      {
        Logger.Error("Got exception when deleting {0} , err:{1}", (object) targetDir, (object) ex1.ToString());
        foreach (string file in Directory.GetFiles(targetDir))
        {
          File.SetAttributes(file, FileAttributes.Normal);
          File.Delete(file);
        }
        foreach (string directory in Directory.GetDirectories(targetDir))
          this.DeleteDirectory(directory);
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

    private void CopyAndroidDataFromOld(string oldDestPath, string newDestPath)
    {
      Logger.Info("Copying files from old folder engine folder");
      string path1_1 = Path.Combine(oldDestPath, "Android");
      string path1_2 = Path.Combine(newDestPath, "Android");
      foreach (string path2 in new List<string>()
      {
        "Root.vdi",
        "fastboot.vdi",
        "Android.bstk.in"
      })
      {
        string str = Path.Combine(path1_1, path2);
        string destFileName = Path.Combine(path1_2, path2);
        if (File.Exists(str))
          File.Copy(str, destFileName, true);
        else
          Logger.Warning("File does not exist: {0}", (object) str);
      }
    }

    private static void CopyOemCfgFileFromOld(string tempDestPath, string newDestPath)
    {
      Logger.Info("Copying oem.cfg file from temp folder engine folder");
      string str = Path.Combine(tempDestPath, "Oem.cfg");
      string destFileName = Path.Combine(newDestPath, "Oem.cfg");
      if (File.Exists(str))
        File.Copy(str, destFileName, true);
      else
        Logger.Error("Oem.cfg missing at path " + str);
    }

    private void CopyUserDataFromOld(string oldDestPath, string newDestPath)
    {
      Logger.Info("Copying files from old folder UserData folder");
      string str1 = Path.Combine(oldDestPath, "UserData\\Gadget");
      string path1_1 = Path.Combine(newDestPath, "UserData\\Gadget");
      foreach (string path2 in new List<string>()
      {
        "systemApps.json",
        "help_center.png",
        "instance_manager.png"
      })
      {
        string str2 = Path.Combine(str1, path2);
        string destFileName = Path.Combine(path1_1, path2);
        if (File.Exists(str2))
          File.Copy(str2, destFileName, true);
        else
          Logger.Warning(str2 + " file doesn't exist");
      }
      foreach (string file in Directory.GetFiles(str1, "sidebar*.json", SearchOption.AllDirectories))
      {
        string destFileName = Path.Combine(path1_1, Path.GetFileName(file));
        try
        {
          Logger.Info("Copying {0}", (object) file);
          File.Copy(file, destFileName, true);
        }
        catch (Exception ex)
        {
          Logger.Warning(file + " file could not be copied. Ex: {0}", (object) ex.Message);
        }
      }
      string str3 = Path.Combine(oldDestPath, "UserData\\InputMapper");
      string path1_2 = Path.Combine(newDestPath, "UserData\\InputMapper");
      foreach (FileInfo file in new DirectoryInfo(str3).GetFiles())
        File.Copy(Path.Combine(str3, file.ToString()), Path.Combine(path1_2, file.ToString()));
    }

    private void CopyEngineDataFromTemp(string sourceDirName, string destDirName)
    {
      Logger.Info("Copying engine data from old folder");
      DirectoryInfo directoryInfo1 = new DirectoryInfo(sourceDirName);
      DirectoryInfo directoryInfo2 = new DirectoryInfo(sourceDirName);
      if (!directoryInfo1.Exists && !directoryInfo2.Exists)
        throw new DirectoryNotFoundException(string.Format("Source {0} and Destination {1} directory does not exist or could not be found: ", (object) sourceDirName, (object) destDirName));
      foreach (DirectoryInfo directory in directoryInfo1.GetDirectories())
      {
        if (directory.Name == "BackupData" || directory.Name == "Locales")
        {
          string str = Path.Combine(destDirName, directory.Name);
          if (!Directory.Exists(str))
            Directory.CreateDirectory(str);
          foreach (FileInfo file in directory.GetFiles())
          {
            string destFileName = Path.Combine(str, file.Name);
            file.CopyTo(destFileName, true);
          }
        }
      }
    }

    private void RenamingRegistryBeforeRestore()
    {
      using (RegistryKey parentKey = Registry.LocalMachine.OpenSubKey(BlueStacks.Common.Strings.RegistryBaseKeyPath, true))
        this.RenameSubKey(parentKey, "Guests", "Guests.old");
    }

    private bool RenameSubKey(RegistryKey parentKey, string subKeyName, string newSubKeyName)
    {
      this.CopyKey(parentKey, subKeyName, newSubKeyName);
      return true;
    }

    private bool CopyKey(RegistryKey parentKey, string keyNameToCopy, string newKeyName)
    {
      RegistryKey subKey = parentKey.CreateSubKey(newKeyName);
      RegistryKey sourceKey = parentKey.OpenSubKey(keyNameToCopy);
      this.RecurseCopyKey(sourceKey, subKey);
      subKey.Close();
      sourceKey.Close();
      return true;
    }

    private void RecurseCopyKey(RegistryKey sourceKey, RegistryKey destinationKey)
    {
      foreach (string valueName in sourceKey.GetValueNames())
      {
        object obj = sourceKey.GetValue(valueName);
        RegistryValueKind valueKind = sourceKey.GetValueKind(valueName);
        destinationKey.SetValue(valueName, obj, valueKind);
      }
      foreach (string subKeyName in sourceKey.GetSubKeyNames())
        this.RecurseCopyKey(sourceKey.OpenSubKey(subKeyName), destinationKey.CreateSubKey(subKeyName));
    }

    private void DeletingExistingInstancesRegistry(string path, List<string> vmList)
    {
      Logger.Info("Deleting existing guest registries from Bluestacks before importing from backup");
      foreach (string vm in vmList)
      {
        Path.Combine("HKLM\\Software\\BlueStacks\\Guests", vm);
        try
        {
          Registry.LocalMachine.DeleteSubKeyTree(Path.Combine(path, vm));
        }
        catch (Exception ex)
        {
          Logger.Error("Failed to delete Bluestacks :{0}", (object) ex.ToString());
        }
      }
    }

    private void ImportRegistry(string srcPath, string bootParams, bool isBackupFromKK = false)
    {
      Logger.Info("Importing registry while restoring");
      if (isBackupFromKK)
      {
        Logger.Info("Backup is from KitKat, updating bootParams");
        bootParams = Utils.GetUpdatedBootParamsString("SDCARD", "/dev/sdc1", bootParams);
        Logger.Info("BootParams being set: {0}", (object) bootParams);
      }
      bootParams = Utils.RemoveKeyFromBootParam("PREBUNDLEDAPPSFS", bootParams);
      bootParams = Utils.GetUpdatedBootParamsString("virttype", 1.ToString(), bootParams);
      string partnerExePath = RegistryManager.Instance.PartnerExePath;
      Utils.RunCmd("reg.exe", string.Format("IMPORT \"{0}\"", (object) Path.Combine(srcPath, "RegHKLMConfig.reg")), (string) null);
      RegistryManager.Instance.PartnerExePath = partnerExePath;
      Utils.RunCmd("reg.exe", string.Format("IMPORT \"{0}\"", (object) Path.Combine(srcPath, "BlueStacksGuest.reg")), (string) null);
      RegistryManager.ClearRegistryMangerInstance();
      foreach (string vm in RegistryManager.Instance.VmList)
      {
        bootParams = this.AddValueInBootParamsFromBackup("GlMode", vm, bootParams);
        bootParams = this.AddValueInBootParamsFromBackup("pcode", vm, bootParams);
        bootParams = this.AddValueInBootParamsFromBackup("abivalue", vm, bootParams);
        bootParams = this.AddValueInBootParamsFromBackup("MEMALLOCATOR", vm, bootParams);
        RegistryManager.Instance.Guest[vm].BootParameters = bootParams;
        RegistryManager.Instance.Guest[vm].FixVboxConfig = true;
        try
        {
          RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(RegistryManager.Instance.BaseKeyPath + "\\Config", false);
          if (registryKey != null)
          {
            string str = (string) registryKey.GetValue("SYSTEM_GUID", (object) "");
            if (!string.IsNullOrEmpty(str))
            {
              Dictionary<string, string> fromBootParamString = Utils.GetBootParamsDictFromBootParamString(RegistryManager.Instance.Guest[vm].BootParameters);
              if (fromBootParamString != null)
              {
                if (!fromBootParamString.ContainsKey("winid"))
                {
                  bootParams = string.Format("{0} winid={1}", (object) bootParams, (object) str);
                  RegistryManager.Instance.Guest[vm].BootParameters = bootParams;
                }
              }
            }
          }
        }
        catch (Exception ex)
        {
          Logger.Error("Error in getting system guid err:" + ex.ToString());
        }
      }
      Utils.RunCmd("reg.exe", string.Format("IMPORT \"{0}\"", (object) Path.Combine(srcPath, "RegHKLMUser.reg")), (string) null);
      string path = Path.Combine(srcPath, "RegHKLMClient.reg");
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      if (!File.Exists(path))
        return;
      foreach (string readAllLine in File.ReadAllLines(path))
      {
        if (readAllLine.Contains("="))
        {
          char[] charArray = "=".ToCharArray();
          string[] strArray = readAllLine.Split(charArray, 2);
          dictionary.Add(strArray[0].Replace("\"", ""), strArray[1].Replace("\"", ""));
        }
      }
      if (dictionary.Keys.Contains<string>("ScreenShotsPath"))
        RegistryManager.Instance.ScreenShotsPath = dictionary["ScreenShotsPath"].Replace("\\\\", "\\");
      if (dictionary.Keys.Contains<string>("ScreenshotsLocationPopupEnabled"))
      {
        string str = dictionary["ScreenshotsLocationPopupEnabled"];
        RegistryManager.Instance.IsScreenshotsLocationPopupEnabled = (int) str[str.Length - 1] - 48 == 1;
      }
      if (dictionary.Keys.Contains<string>("FirstAppLaunchedState"))
        RegistryManager.Instance.FirstAppLaunchState = (AppLaunchState) Enum.Parse(typeof (AppLaunchState), dictionary["FirstAppLaunchedState"]);
      if (dictionary.Keys.Contains<string>("TranslucentControlsTransparency"))
        RegistryManager.Instance.TranslucentControlsTransparency = double.Parse(dictionary["TranslucentControlsTransparency"], (IFormatProvider) CultureInfo.InvariantCulture);
      if (dictionary.Keys.Contains<string>("ShowKeyControlsOverlay"))
      {
        string str = dictionary["ShowKeyControlsOverlay"];
        RegistryManager.Instance.ShowKeyControlsOverlay = (int) str[str.Length - 1] - 48 == 1;
      }
      if (dictionary.Keys.Contains<string>("InstanceSortOption"))
        RegistryManager.Instance.InstanceSortOption = dictionary["InstanceSortOption"];
      if (!dictionary.Keys.Contains<string>("OnboardingBlurbShownCount"))
        return;
      string str1 = dictionary["OnboardingBlurbShownCount"];
      RegistryManager.Instance.OnboardingBlurbShownCount = (int) str1[str1.Length - 1] - 48;
    }

    private string AddValueInBootParamsFromBackup(string value, string vm, string bootParams)
    {
      Logger.Info("In AddValueInBootParamsFromBackup");
      string bootParameters = RegistryManager.Instance.Guest[vm].BootParameters;
      char[] separator = new char[1]{ ' ' };
      foreach (string str in ((IEnumerable<string>) bootParameters.Split(separator, StringSplitOptions.RemoveEmptyEntries)).ToList<string>())
      {
        if (str.Contains(value))
        {
          string[] strArray = str.Split('=');
          if (string.Equals(strArray[0], value, StringComparison.InvariantCultureIgnoreCase))
            bootParams = Utils.GetUpdatedBootParamsString(strArray[0], strArray[1], bootParams);
        }
      }
      return bootParams;
    }

    private DataManagerCodes ConfiguringFilesForTheRestoreToWork(
      string srcDir,
      string destPath)
    {
      Logger.Info("In ConfiguringFilesForTheRestoreToWork");
      string path = Path.Combine(srcDir, string.Format(".{0}", (object) "Bst_UserInfo_Backup"));
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      if (File.Exists(path))
      {
        Logger.Info("the user info temp file exists : " + path);
        try
        {
          foreach (string readAllLine in File.ReadAllLines(path))
          {
            string[] strArray = readAllLine.Split(" ".ToCharArray(), 2);
            dictionary.Add(strArray[0], strArray[1]);
          }
          if (dictionary["UserDefinedDir"] != RegistryManager.Instance.UserDefinedDir)
            this.ReplaceTextInConfigurationFiles(dictionary["UserDefinedDir"], RegistryManager.Instance.UserDefinedDir, destPath);
          if (dictionary["InstallDir"] != RegistryStrings.InstallDir)
            this.ReplaceTextInConfigurationFiles(dictionary["InstallDir"], RegistryStrings.InstallDir, destPath);
          string environmentVariable = Environment.GetEnvironmentVariable("userprofile");
          if (dictionary["UsersPath"] != environmentVariable)
            this.ReplaceTextInConfigurationFiles(dictionary["UsersPath"], environmentVariable, destPath);
        }
        catch (Exception ex)
        {
          Logger.Info(string.Format("Some error in writing configuration files " + ex.ToString()));
          return DataManagerCodes.ERROR_IN_CHANGING_CONFIGURATION_FILES;
        }
        return DataManagerCodes.SUCCESS;
      }
      Logger.Info("the user info file does not exist : " + path);
      return DataManagerCodes.BACKUP_INFO_FILE_NOT_EXIST;
    }

    private void ReplaceTextInConfigurationFiles(
      string patternString,
      string replaceString,
      string path)
    {
      Logger.Info("In ReplaceTextInConfigurationFiles");
      foreach (string vm in RegistryManager.Instance.VmList)
      {
        string str1 = Path.Combine(path, vm);
        if (Directory.Exists(str1))
        {
          string str2 = Path.Combine(str1, string.Format("{0}.bstk", (object) vm));
          string contents1 = HttpUtility.HtmlDecode(File.ReadAllText(str2)).Replace(patternString, replaceString);
          File.WriteAllText(str2, contents1);
          string oldValue = patternString.Replace('\\', '/');
          string newValue = replaceString.Replace('\\', '/');
          string contents2 = contents1.Replace(oldValue, newValue);
          File.WriteAllText(str2, contents2);
          this.SaveFileInUnicode(str2);
        }
        else
          Logger.Error("Vm directory not exist");
      }
      string str3 = Path.Combine(path, "Manager");
      if (!Directory.Exists(str3))
        return;
      string str4 = Path.Combine(str3, "BstkGlobal.xml");
      string contents = HttpUtility.HtmlDecode(File.ReadAllText(str4)).Replace(patternString, replaceString);
      File.WriteAllText(str4, contents);
      this.SaveFileInUnicode(str4);
    }

    private void SaveFileInUnicode(string filePath)
    {
      string str = File.ReadAllText(filePath);
      using (StreamWriter streamWriter = new StreamWriter(filePath, false, Encoding.Unicode))
      {
        streamWriter.Write(str);
        streamWriter.Flush();
        streamWriter.Close();
      }
    }

    private void FixAndroidbstkInAllVms(string destPath)
    {
      string prebundledVdiUid = CommonInstallUtils.GetPrebundledVdiUid(Path.Combine(Path.Combine(destPath, "Android"), string.Format("{0}.bstk", (object) "Android")));
      foreach (string vm in RegistryManager.Instance.VmList)
      {
        string str = Path.Combine(Path.Combine(destPath, vm), string.Format("{0}.bstk", (object) vm));
        if (File.Exists(str))
        {
          this.UpdateRootPrebundledVdiEntry(str, prebundledVdiUid);
          this.AddExtraDataItemTSCOffSet(str);
        }
        else
          Logger.Error("File {0} not present", (object) str);
      }
    }

    private void UpdateRootPrebundledVdiEntry(string bstkFilePath, string prebundledUuid)
    {
      Logger.Info("In UpdateRootPrebundledVdiEntry");
      try
      {
        Utils.ReplaceOldVirtualBoxNamespaceWithNew(bstkFilePath);
        string xml = File.ReadAllText(bstkFilePath);
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(xml);
        XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDocument.NameTable);
        nsmgr.AddNamespace("bstk", "http://www.virtualbox.org/");
        XmlNodeList xmlNodeList = xmlDocument.SelectNodes("descendant::bstk:Machine//bstk:MediaRegistry//bstk:HardDisks//bstk:HardDisk", nsmgr);
        foreach (XmlNode xmlNode in xmlNodeList)
        {
          if (xmlNode.Attributes["location"].Value.Equals("Root.vdi", StringComparison.InvariantCultureIgnoreCase))
          {
            if (xmlNode.Attributes["type"].Value.Equals("Normal"))
            {
              xmlNode.Attributes["type"].Value = "Readonly";
              break;
            }
            break;
          }
        }
        foreach (XmlNode oldChild in xmlNodeList)
        {
          if (oldChild.Attributes["uuid"].Value.Equals(prebundledUuid, StringComparison.InvariantCultureIgnoreCase))
          {
            oldChild.ParentNode.RemoveChild(oldChild);
            break;
          }
        }
        foreach (XmlNode selectNode in xmlDocument.SelectNodes("descendant::bstk:Machine//bstk:StorageControllers//bstk:StorageController//bstk:AttachedDevice//bstk:Image", nsmgr))
        {
          if (selectNode.Attributes["uuid"].Value.Equals(prebundledUuid, StringComparison.InvariantCultureIgnoreCase))
          {
            XmlAttribute attribute = selectNode.ParentNode.ParentNode.Attributes["PortCount"];
            int result;
            if (int.TryParse(attribute.Value, out result))
            {
              int num;
              attribute.Value = (num = result - 1).ToString();
            }
            selectNode.ParentNode.ParentNode.RemoveChild(selectNode.ParentNode);
            break;
          }
        }
        foreach (XmlNode selectNode in xmlDocument.SelectNodes("descendant::bstk:Machine//bstk:StorageControllers//bstk:StorageController", nsmgr))
        {
          if (selectNode.Attributes["name"].Value.Equals("SATA", StringComparison.InvariantCultureIgnoreCase))
          {
            foreach (XmlNode xmlNode in selectNode)
            {
              if (xmlNode.Attributes["port"].Value.Equals("0") && xmlNode.FirstChild.Name == "Image")
              {
                xmlNode.FirstChild.Attributes["uuid"].Value = "{fca296ce-8268-4ed7-a57f-d32ec11ab304}";
                break;
              }
            }
          }
        }
        xmlDocument.Save(bstkFilePath);
      }
      catch (Exception ex)
      {
        Logger.Error("Error in Updating RootVdiEntry in bstk file" + ex.ToString());
      }
    }

    public void AddExtraDataItemTSCOffSet(string bstkFilePath)
    {
      Logger.Info("In AddExtraDataItemTSCOffSet");
      try
      {
        Utils.ReplaceOldVirtualBoxNamespaceWithNew(bstkFilePath);
        string xml = File.ReadAllText(bstkFilePath);
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(xml);
        XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDocument.NameTable);
        nsmgr.AddNamespace("bstk", "http://www.virtualbox.org/");
        XmlNodeList xmlNodeList = xmlDocument.SelectNodes("descendant::bstk:Machine//bstk:ExtraData//bstk:ExtraDataItem", nsmgr);
        bool flag = false;
        foreach (XmlNode xmlNode in xmlNodeList)
        {
          if (xmlNode.Attributes["name"].Value.Equals("VBoxInternal/TM/TSCMode", StringComparison.InvariantCultureIgnoreCase))
          {
            flag = true;
            Logger.Info("TSCOffSet is present in the file");
            break;
          }
        }
        if (!flag)
        {
          XmlNode xmlNode = xmlDocument.SelectSingleNode("descendant::bstk:Machine//bstk:ExtraData", nsmgr);
          XmlElement element = xmlDocument.CreateElement("ExtraDataItem", "http://www.virtualbox.org/");
          element.SetAttribute("name", "VBoxInternal/TM/TSCMode");
          element.SetAttribute("value", "RealTSCOffset");
          xmlNode.AppendChild((XmlNode) element);
        }
        xmlDocument.Save(bstkFilePath);
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to add TSCOffSet extraitem in bstk file " + ex.ToString());
      }
    }

    private void DeletingBstkPrevFilesIfExists(string destPath)
    {
      Logger.Info("Deleting bstk-prev files if present");
      try
      {
        foreach (string vm in RegistryManager.Instance.VmList)
        {
          string path = Path.Combine(Path.Combine(destPath, vm), string.Format("{0}.bstk-prev", (object) vm));
          if (File.Exists(path))
            File.Delete(path);
          else
            Logger.Error("File {0} not present", (object) path);
        }
      }
      catch (Exception ex)
      {
        Logger.Warning("Unable to delete Bstk-prev file err: " + ex.ToString());
      }
    }

    private void DeleteRecursiveWithPendMove(string delPath)
    {
      Logger.Info(string.Format("Deleting {0}", (object) delPath));
      try
      {
        Directory.Delete(delPath, true);
      }
      catch (Exception ex1)
      {
        Logger.Info("Caught exception in deleting Folder {0} , will try to delete folder recursively now {1}", (object) delPath, (object) ex1.ToString());
        DirectoryInfo directoryInfo = new DirectoryInfo(delPath);
        foreach (FileInfo file in directoryInfo.GetFiles())
        {
          try
          {
            file.Delete();
          }
          catch (Exception ex2)
          {
            Logger.Error(string.Format("Error Occured, Err: {0}", (object) ex2.ToString()));
            try
            {
              Logger.Info(string.Format("Scheduling Pend delete for {0}", (object) file.FullName));
              if (File.Exists(file.FullName + ".old"))
                File.Delete(file.FullName + ".old");
              File.Move(file.FullName, file.FullName + ".old");
              if (File.Exists(file.FullName + ".old"))
              {
                if (!Restore.MoveFileEx(file.FullName + ".old", (string) null, Restore.MoveFileFlags.DelayUntilReboot))
                  Logger.Error(string.Format("Error While Scheduling. Err: {0}", (object) Marshal.GetLastWin32Error()));
              }
            }
            catch (Exception ex3)
            {
              Logger.Error(string.Format("Error Occured on putting file for pend delete, Err: {0}", (object) ex3.ToString()));
            }
          }
        }
        foreach (DirectoryInfo directory in directoryInfo.GetDirectories())
          this.DeleteRecursiveWithPendMove(Path.Combine(delPath, directory.Name));
        try
        {
          this.DeleteDirectory(delPath);
        }
        catch (Exception ex2)
        {
          Logger.Error(string.Format("Error Occured, Err: {0}", (object) ex2.ToString()));
        }
      }
    }

    private List<string> GetVmDesktopShortcutIconList()
    {
      Logger.Info("In GetVmDesktopShortcutIconList ");
      List<string> stringList = new List<string>();
      foreach (string vm in RegistryManager.Instance.VmList)
      {
        if (!"Android".Equals(vm))
          stringList.Add(string.Format("{0}-{1}.lnk", (object) BlueStacks.Common.Strings.ProductDisplayName, (object) Utils.GetDisplayName(vm, "bgp")));
      }
      return stringList;
    }

    private void DeleteShortcuts(List<string> shortcutIconList)
    {
      foreach (string shortcutIcon in shortcutIconList)
        ShortcutHelper.DeleteDesktopShortcut(shortcutIcon);
    }

    private void DeleteAppIconsShortCuts()
    {
      Logger.Info("In DeleteAppIconsShortCuts");
      string[] files = Directory.GetFiles(ShortcutHelper.sDesktopPath, "*.lnk", SearchOption.AllDirectories);
      if (!Oem.Instance.IsCreateDesktopIconForApp)
        return;
      foreach (string str in files)
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

    private void CreateAppIconsShortCuts()
    {
      try
      {
        Logger.Info("In CreateAppIconsShortCuts");
        string str = Path.Combine(RegistryStrings.DataDir, "UserData\\Gadget");
        string path = Path.Combine(str, "apps_Android.json");
        if (!File.Exists(path) || !Oem.Instance.IsCreateDesktopIconForApp || !RegistryManager.Instance.AddDesktopShortcuts)
          return;
        string json = (string) null;
        using (StreamReader streamReader = new StreamReader(path))
          json = streamReader.ReadToEnd();
        foreach (JObject jobject1 in JArray.Parse(json))
        {
          string ico = Utils.ConvertToIco(Path.Combine(str, jobject1["img"].ToString().Trim()), str);
          Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), jobject1["name"].ToString().Trim() + ".lnk");
          string targetApplication = Path.Combine(RegistryStrings.InstallDir, "HD-RunApp.exe");
          JObject jobject2 = new JObject()
          {
            {
              "app_icon_url",
              (JToken) ""
            },
            {
              "app_name",
              (JToken) jobject1["name"].ToString().Trim()
            },
            {
              "app_url",
              (JToken) ""
            },
            {
              "app_pkg",
              (JToken) jobject1["package"].ToString().Trim()
            }
          };
          string paramsString = "-json \"" + jobject2.ToString(Newtonsoft.Json.Formatting.None).Replace("\"", "\\\"") + "\"";
          ShortcutHelper.CreateDesktopShortcut(jobject2["app_name"].ToString(), ico, targetApplication, paramsString, "", jobject2["app_pkg"].ToString());
        }
      }
      catch (Exception ex)
      {
        Logger.Warning("Could not create app shortcut, err: {0}", (object) ex.Message);
      }
    }

    public enum MoveFileFlags
    {
      None = 0,
      ReplaceExisting = 1,
      CopyAllowed = 2,
      DelayUntilReboot = 4,
      WriteThrough = 8,
      CreateHardlink = 16, // 0x00000010
      FailIfNotTrackable = 32, // 0x00000020
    }
  }
}
