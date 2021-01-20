// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.BlueStacksUpdater
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BlueStacks.BlueStacksUI
{
  internal class BlueStacksUpdater
  {
    internal static BlueStacksUpdateData sBstUpdateData = (BlueStacksUpdateData) null;
    private static BlueStacksUpdater.UpdateState sUpdateState = BlueStacksUpdater.UpdateState.NO_UPDATE;
    private static MainWindow ParentWindow;
    internal static BackgroundWorker sCheckUpdateBackgroundWorker;
    private static UpdateDownloadProgress sUpdateDownloadProgress;
    internal static Downloader sDownloader;

    internal static bool IsDownloadingInHiddenMode { get; set; } = true;

    internal static BlueStacksUpdater.UpdateState SUpdateState
    {
      get
      {
        return BlueStacksUpdater.sUpdateState;
      }
      set
      {
        BlueStacksUpdater.sUpdateState = value;
        System.Action stateChanged = BlueStacksUpdater.StateChanged;
        if (stateChanged == null)
          return;
        stateChanged();
      }
    }

    internal static event System.Action<BlueStacks.Common.Tuple<BlueStacksUpdateData, bool>> DownloadCompleted;

    internal static event System.Action StateChanged;

    public static void SetupBlueStacksUpdater(MainWindow window, bool isStartup)
    {
      BlueStacksUpdater.ParentWindow = window;
      if (BlueStacksUpdater.sCheckUpdateBackgroundWorker == null)
      {
        BlueStacksUpdater.sCheckUpdateBackgroundWorker = new BackgroundWorker();
        BlueStacksUpdater.sCheckUpdateBackgroundWorker.DoWork += (DoWorkEventHandler) ((sender, e) =>
        {
          bool flag = (bool) e.Argument;
          BlueStacksUpdateData stacksUpdateData = BlueStacksUpdater.CheckForUpdate(!flag);
          BlueStacksUpdater.sBstUpdateData = stacksUpdateData;
          e.Result = (object) new BlueStacks.Common.Tuple<BlueStacksUpdateData, bool>(stacksUpdateData, flag);
        });
        BlueStacksUpdater.sCheckUpdateBackgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BlueStacksUpdater.CheckUpdateBackgroundWorker_RunWorkerCompleted);
      }
      if (!BlueStacksUpdater.sCheckUpdateBackgroundWorker.IsBusy)
        BlueStacksUpdater.sCheckUpdateBackgroundWorker.RunWorkerAsync((object) isStartup);
      else
        Logger.Info("Not launching update checking thread, since already running");
    }

    private static void CheckUpdateBackgroundWorker_RunWorkerCompleted(
      object sender,
      RunWorkerCompletedEventArgs e)
    {
      BlueStacks.Common.Tuple<BlueStacksUpdateData, bool> result = (BlueStacks.Common.Tuple<BlueStacksUpdateData, bool>) e.Result;
      BlueStacksUpdateData bstUpdateData = result.Item1;
      bool flag = result.Item2;
      if (bstUpdateData.IsUpdateAvailble)
      {
        BlueStacksUpdater.ParentWindow.mTopBar.mConfigButton.ImageName = "cfgmenu_update";
        BlueStacksUpdater.ParentWindow.mTopBar.mPreferenceDropDownControl.mUpgradeBluestacksStatus.Visibility = Visibility.Visible;
        BlueStacksUIBinding.Bind(BlueStacksUpdater.ParentWindow.mTopBar.mPreferenceDropDownControl.mUpgradeBluestacksStatusTextBlock, "STRING_DOWNLOAD_UPDATE", "");
        BlueStacksUpdater.ParentWindow.mTopBar.mPreferenceDropDownControl.mUpdateDownloadProgressPercentage.Visibility = Visibility.Collapsed;
        if (bstUpdateData.IsFullInstaller)
        {
          if (!flag)
            return;
          if (bstUpdateData.UpdateType.Equals("hard", StringComparison.InvariantCultureIgnoreCase))
          {
            Logger.Info("Forced full installer update, starting download.");
            BlueStacksUpdater.DownloadNow(bstUpdateData, true);
          }
          else
          {
            if (!bstUpdateData.UpdateType.Equals("soft", StringComparison.InvariantCultureIgnoreCase) || string.Compare(bstUpdateData.EngineVersion.Trim(), RegistryManager.Instance.LastUpdateSkippedVersion.Trim(), StringComparison.OrdinalIgnoreCase) == 0)
              return;
            ClientStats.SendBluestacksUpdaterUIStatsAsync(ClientStatsEvent.UpgradePopup, "");
            UpdatePrompt updatePrompt1 = new UpdatePrompt(bstUpdateData);
            updatePrompt1.Height = 215.0;
            updatePrompt1.Width = 400.0;
            UpdatePrompt updatePrompt2 = updatePrompt1;
            ContainerWindow containerWindow = new ContainerWindow(BlueStacksUpdater.ParentWindow, (UserControl) updatePrompt2, (double) (int) updatePrompt2.Width, (double) (int) updatePrompt2.Height, false, true, false, -1.0, (Brush) null);
          }
        }
        else
        {
          Logger.Info("Only client installer update, starting download.");
          BlueStacksUpdater.DownloadNow(bstUpdateData, true);
        }
      }
      else
        BlueStacksUpdater.SUpdateState = BlueStacksUpdater.UpdateState.NO_UPDATE;
    }

    private static BlueStacksUpdateData CheckForUpdate(bool isManualCheck)
    {
      BlueStacksUpdateData stacksUpdateData = new BlueStacksUpdateData();
      try
      {
        string urlWithParams = WebHelper.GetUrlWithParams(WebHelper.GetServerHost() + "/check_upgrade", (string) null, (string) null, (string) null);
        Logger.Debug("The URL for checking upgrade: {0}", (object) urlWithParams);
        string osName;
        SystemUtils.GetOSInfo(out osName, out string _, out string _);
        string str = InstallerArchitectures.AMD64;
        if (!SystemUtils.IsOs64Bit())
          str = InstallerArchitectures.X86;
        Dictionary<string, string> data = new Dictionary<string, string>()
        {
          {
            "installer_arch",
            str
          },
          {
            "os",
            osName
          },
          {
            "manual_check",
            isManualCheck.ToString((IFormatProvider) CultureInfo.InvariantCulture)
          }
        };
        string json = BstHttpClient.Post(urlWithParams, data, (Dictionary<string, string>) null, false, string.Empty, 5000, 1, 0, false, "bgp");
        Logger.Info("Response received for check for update: " + Environment.NewLine + json);
        JObject jobject = JObject.Parse(json);
        if (jobject["update_available"].ToString().Equals("true", StringComparison.InvariantCultureIgnoreCase) && RegistryManager.Instance.FailedUpgradeVersion != jobject["update_details"][(object) "client_version"].ToString())
        {
          stacksUpdateData.IsUpdateAvailble = true;
          stacksUpdateData.UpdateType = jobject["update_details"][(object) "upgrade_type"].ToString();
          stacksUpdateData.IsFullInstaller = jobject["update_details"][(object) "is_full_installer"].ToObject<bool>();
          stacksUpdateData.Md5 = jobject["update_details"][(object) "md5"].ToString();
          stacksUpdateData.ClientVersion = jobject["update_details"][(object) "client_version"].ToString();
          stacksUpdateData.EngineVersion = jobject["update_details"][(object) "engine_version"].ToString();
          stacksUpdateData.DownloadUrl = jobject["update_details"][(object) "download_url"].ToString();
          stacksUpdateData.DetailedChangeLogsUrl = jobject["update_details"][(object) "detailed_changelogs_url"].ToString();
          if (!Directory.Exists(RegistryManager.Instance.SetupFolder))
            Directory.CreateDirectory(RegistryManager.Instance.SetupFolder);
          stacksUpdateData.UpdateDownloadLocation = !stacksUpdateData.IsFullInstaller ? Path.Combine(RegistryManager.Instance.SetupFolder, "BlueStacksInstaller_" + stacksUpdateData.ClientVersion + "_client.zip") : Path.Combine(RegistryManager.Instance.SetupFolder, "BlueStacksInstaller_" + stacksUpdateData.ClientVersion + "_full.exe");
          RegistryManager.Instance.DownloadedUpdateFile = stacksUpdateData.UpdateDownloadLocation;
          BlueStacksUpdater.sBstUpdateData = stacksUpdateData;
          BlueStacksUpdater.SUpdateState = BlueStacksUpdater.UpdateState.UPDATE_AVAILABLE;
        }
        return stacksUpdateData;
      }
      catch (Exception ex)
      {
        Logger.Warning("Got error in checking for upgrade: {0}", (object) ex.ToString());
        return new BlueStacksUpdateData()
        {
          IsTryAgain = true
        };
      }
    }

    private static void DownloadUpdate(BlueStacksUpdateData bluestacksUpdateData)
    {
      BlueStacksUpdater.sDownloader = new Downloader();
      BlueStacksUpdater.sDownloader.DownloadException += new Downloader.DownloadExceptionEventHandler(BlueStacksUpdater.Downloader_DownloadException);
      BlueStacksUpdater.sDownloader.DownloadProgressPercentChanged += new Downloader.DownloadProgressPercentChangedEventHandler(BlueStacksUpdater.Downloader_DownloadProgressPercentChanged);
      BlueStacksUpdater.sDownloader.DownloadFileCompleted += new Downloader.DownloadFileCompletedEventHandler(BlueStacksUpdater.Downloader_DownloadFileCompleted);
      BlueStacksUpdater.sDownloader.UnsupportedResume += new Downloader.UnsupportedResumeEventHandler(BlueStacksUpdater.Downloader_UnsupportedResume);
      BlueStacksUpdater.sDownloader.DownloadFile(bluestacksUpdateData.DownloadUrl, bluestacksUpdateData.UpdateDownloadLocation);
    }

    private static void Downloader_DownloadProgressPercentChanged(double percentDouble)
    {
      Logger.Info("File downloaded {0}%", (object) percentDouble);
      int percent = Convert.ToInt32(Math.Floor(percentDouble));
      BlueStacksUpdater.ParentWindow.mTopBar.ChangeDownloadPercent(percent);
      if (BlueStacksUpdater.sUpdateDownloadProgress == null)
        return;
      BlueStacksUpdater.ParentWindow.Dispatcher.Invoke((Delegate) (() =>
      {
        BlueStacksUpdater.sUpdateDownloadProgress.mUpdateDownloadProgressPercentage.Content = (object) (percent.ToString() + "%");
        BlueStacksUpdater.sUpdateDownloadProgress.mUpdateDownloadProgressBar.Value = (double) percent;
        BlueStacksUpdater.ParentWindow.mTopBar.mPreferenceDropDownControl.mUpdateDownloadProgressPercentage.Content = (object) (percent.ToString() + "%");
      }));
    }

    private static void Downloader_UnsupportedResume(HttpStatusCode sc)
    {
      Logger.Error("UnsupportedResume, HTTPStatusCode: {0}", (object) sc);
      System.IO.File.Delete(BlueStacksUpdater.sBstUpdateData.UpdateDownloadLocation);
      BlueStacksUpdater.sDownloader.DownloadFile(BlueStacksUpdater.sBstUpdateData.DownloadUrl, BlueStacksUpdater.sBstUpdateData.UpdateDownloadLocation);
    }

    private static void Downloader_DownloadFileCompleted(object sender, EventArgs args)
    {
      Logger.Info("File downloaded successfully at {0}", (object) BlueStacksUpdater.sBstUpdateData?.UpdateDownloadLocation);
      BlueStacksUpdater.DownloadComplete();
    }

    private static void Downloader_DownloadException(Exception e)
    {
      Logger.Error("Failed to download file: {0}. err: {1}", (object) BlueStacksUpdater.sBstUpdateData.DownloadUrl, (object) e.Message);
      BlueStacksUpdater.ParentWindow.Dispatcher.Invoke((Delegate) (() =>
      {
        CustomMessageWindow customMessageWindow = new CustomMessageWindow();
        BlueStacksUIBinding.Bind(customMessageWindow.TitleTextBlock, "STRING_UPGRADE_FAILED", "");
        BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlock, "STRING_SOME_ERROR_OCCURED_DOWNLOAD", "");
        customMessageWindow.AddButton(ButtonColors.Blue, "STRING_RETRY", new EventHandler(BlueStacksUpdater.RetryDownload), (string) null, false, (object) null, true);
        customMessageWindow.AddButton(ButtonColors.White, "STRING_CANCEL", new EventHandler(BlueStacksUpdater.DownloadCancelled), (string) null, false, (object) null, true);
        BlueStacksUpdater.ParentWindow.ShowDimOverlay((IDimOverlayControl) null);
        customMessageWindow.Owner = (Window) BlueStacksUpdater.ParentWindow.mDimOverlay;
        customMessageWindow.ShowDialog();
        BlueStacksUpdater.ParentWindow.HideDimOverlay();
        BlueStacksUpdater.sUpdateDownloadProgress.Hide();
      }));
    }

    private static void DownloadCancelled(object sender, EventArgs e)
    {
      BlueStacksUpdater.ParentWindow.Dispatcher.Invoke((Delegate) (() => BlueStacksUpdater.ParentWindow.mTopBar.mPreferenceDropDownControl.mUpgradeBluestacksStatus.Visibility = Visibility.Collapsed));
    }

    private static void RetryDownload(object sender, EventArgs e)
    {
      new Thread((ThreadStart) (() => BlueStacksUpdater.sDownloader.DownloadFile(BlueStacksUpdater.sBstUpdateData.DownloadUrl, BlueStacksUpdater.sBstUpdateData.UpdateDownloadLocation)))
      {
        IsBackground = true
      }.Start();
    }

    private static void DownloadComplete()
    {
      Logger.Info("Installer download completed");
      BlueStacksUpdater.SUpdateState = BlueStacksUpdater.UpdateState.DOWNLOADED;
      BlueStacksUpdater.ParentWindow.Dispatcher.Invoke((Delegate) (() =>
      {
        BlueStacksUIBinding.Bind(BlueStacksUpdater.ParentWindow.mTopBar.mPreferenceDropDownControl.mUpgradeBluestacksStatusTextBlock, "STRING_INSTALL_UPDATE", "");
        BlueStacksUpdater.ParentWindow.mTopBar.mPreferenceDropDownControl.mUpdateDownloadProgressPercentage.Visibility = Visibility.Collapsed;
        if (BlueStacksUpdater.sUpdateDownloadProgress == null)
          return;
        BlueStacksUpdater.sUpdateDownloadProgress.Close();
      }));
      BlueStacksUpdater.DownloadCompleted(new BlueStacks.Common.Tuple<BlueStacksUpdateData, bool>(BlueStacksUpdater.sBstUpdateData, BlueStacksUpdater.IsDownloadingInHiddenMode));
    }

    internal static void DownloadNow(BlueStacksUpdateData bstUpdateData, bool hiddenMode)
    {
      new Thread((ThreadStart) (() =>
      {
        BlueStacksUpdater.IsDownloadingInHiddenMode = hiddenMode;
        BlueStacksUpdater.SUpdateState = BlueStacksUpdater.UpdateState.DOWNLOADING;
        if (System.IO.File.Exists(bstUpdateData.UpdateDownloadLocation))
        {
          BlueStacksUpdater.DownloadComplete();
        }
        else
        {
          BlueStacksUpdater.ParentWindow.Dispatcher.Invoke((Delegate) (() =>
          {
            BlueStacksUIBinding.Bind(BlueStacksUpdater.ParentWindow.mTopBar.mPreferenceDropDownControl.mUpgradeBluestacksStatusTextBlock, "STRING_DOWNLOADING_UPDATE", "");
            BlueStacksUpdater.ParentWindow.mTopBar.mPreferenceDropDownControl.mUpdateDownloadProgressPercentage.Visibility = Visibility.Visible;
            BlueStacksUpdater.ParentWindow.mTopBar.mPreferenceDropDownControl.mUpdateDownloadProgressPercentage.Content = (object) "0%";
            BlueStacksUpdater.sUpdateDownloadProgress = new UpdateDownloadProgress();
            BlueStacksUpdater.sUpdateDownloadProgress.mUpdateDownloadProgressPercentage.Content = (object) "0%";
            BlueStacksUpdater.sUpdateDownloadProgress.Owner = (Window) BlueStacksUpdater.ParentWindow;
            if (hiddenMode)
              return;
            BlueStacksUpdater.sUpdateDownloadProgress.Show();
          }));
          BlueStacksUpdater.DownloadUpdate(bstUpdateData);
        }
      }))
      {
        IsBackground = true
      }.Start();
    }

    internal static void ShowDownloadProgress()
    {
      if (BlueStacksUpdater.sUpdateDownloadProgress == null)
        return;
      BlueStacksUpdater.sUpdateDownloadProgress.Show();
    }

    internal static void CheckDownloadedUpdateFileAndUpdate()
    {
      using (BackgroundWorker backgroundWorker = new BackgroundWorker())
      {
        backgroundWorker.DoWork += (DoWorkEventHandler) ((sender, args) => BlueStacksUpdater.HandleUpgrade(RegistryManager.Instance.DownloadedUpdateFile));
        backgroundWorker.RunWorkerCompleted += (RunWorkerCompletedEventHandler) ((sender, args2) => App.ExitApplication());
        backgroundWorker.RunWorkerAsync();
      }
    }

    internal static void HandleUpgrade(string downloadedFilePath)
    {
      if (BlueStacksUpdater.CheckIfUpdateIsFullOrClientOnly(downloadedFilePath) == BlueStacksUpdater.UpdateType.ClientOnly)
        BlueStacksUpdater.HandleClientOnlyUpgrade(downloadedFilePath);
      else
        BlueStacksUpdater.HandleFullUpgrade(downloadedFilePath);
      RegistryManager.Instance.UpdaterFileDeletePath = RegistryManager.Instance.DownloadedUpdateFile;
      RegistryManager.Instance.DownloadedUpdateFile = "";
    }

    private static void HandleFullUpgrade(string downloadedFilePath)
    {
      Logger.Info("In HandleFullUpgrade");
      BluestacksProcessHelper.RunUpdateInstaller(downloadedFilePath, "-u -upgradesourcepath BluestacksUI", false);
    }

    private static void HandleClientOnlyUpgrade(string downloadedFilePath)
    {
      Logger.Info("In HandleClientOnlyUpgrade");
      try
      {
        int num = BlueStacksUpdater.ExtractingClientInstaller(downloadedFilePath);
        if (num == 0)
        {
          BluestacksProcessHelper.RunUpdateInstaller(Path.Combine(Path.Combine(RegistryManager.Instance.SetupFolder, Path.GetFileNameWithoutExtension(downloadedFilePath)), "Bootstrapper.exe"), "", false);
        }
        else
        {
          Logger.Warning("Update extraction failed, ExitCode: {0}", (object) num);
          System.IO.File.Delete(downloadedFilePath);
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Some Error in Client Upgrade err: ", (object) ex.ToString());
      }
    }

    internal static bool CheckIfDownloadedFileExist()
    {
      string downloadedUpdateFile = RegistryManager.Instance.DownloadedUpdateFile;
      if (!string.IsNullOrEmpty(downloadedUpdateFile) && System.IO.File.Exists(downloadedUpdateFile))
        return true;
      string updaterFileDeletePath = RegistryManager.Instance.UpdaterFileDeletePath;
      if (!string.IsNullOrEmpty(updaterFileDeletePath) && System.IO.File.Exists(updaterFileDeletePath))
      {
        if (RegistryManager.Instance.IsClientFirstLaunch == 1)
        {
          try
          {
            System.IO.File.Delete(updaterFileDeletePath);
            RegistryManager.Instance.UpdaterFileDeletePath = "";
          }
          catch (Exception ex)
          {
            Logger.Warning("Error in Deleting Updater File : ", (object) ex.ToString());
          }
        }
      }
      return false;
    }

    private static BlueStacksUpdater.UpdateType CheckIfUpdateIsFullOrClientOnly(
      string downloadedFilePath)
    {
      return string.Equals(Path.GetExtension(downloadedFilePath), ".zip", StringComparison.InvariantCultureIgnoreCase) ? BlueStacksUpdater.UpdateType.ClientOnly : BlueStacksUpdater.UpdateType.FullUpdate;
    }

    private static int ExtractingClientInstaller(string updateFile)
    {
      string extractDirectory = Path.Combine(RegistryManager.Instance.SetupFolder, Path.GetFileNameWithoutExtension(updateFile));
      Logger.Info("Extracting Zip file {0} at {1}", (object) updateFile, (object) extractDirectory);
      return MiscUtils.Extract7Zip(updateFile, extractDirectory);
    }

    internal enum UpdateState
    {
      NO_UPDATE,
      UPDATE_AVAILABLE,
      DOWNLOADING,
      DOWNLOADED,
    }

    internal enum UpdateType
    {
      FullUpdate,
      ClientOnly,
    }
  }
}
