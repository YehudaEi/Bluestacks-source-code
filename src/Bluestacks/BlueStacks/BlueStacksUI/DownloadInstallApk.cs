// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.DownloadInstallApk
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Forms;

namespace BlueStacks.BlueStacksUI
{
  internal class DownloadInstallApk
  {
    private static Dictionary<string, SerialWorkQueue> mSerialQueue = new Dictionary<string, SerialWorkQueue>();
    internal static List<string> sDownloadedApkList = new List<string>();
    internal static List<string> sApkInstalledFromChooser = new List<string>();
    internal const string mIconUrl = "https://cloud.bluestacks.com/app/icon?pkg={0}&fallback=false";
    private Thread mDownloadThread;
    public bool mIsDownloading;
    private LegacyDownloader mDownloader;
    private MainWindow ParentWindow;

    public DownloadInstallApk(MainWindow mainWindow)
    {
      this.ParentWindow = mainWindow;
    }

    public static SerialWorkQueue SerialWorkQueueInstaller(string vmName)
    {
      if (!DownloadInstallApk.mSerialQueue.ContainsKey(vmName))
        DownloadInstallApk.mSerialQueue[vmName] = new SerialWorkQueue();
      return DownloadInstallApk.mSerialQueue[vmName];
    }

    internal void DownloadAndInstallAppFromJson(string campaignJson)
    {
      try
      {
        JObject resJson = JObject.Parse(campaignJson);
        string iconUrl = "";
        string appName = "";
        string apkUrl = "";
        string package = "";
        resJson.AssignIfContains<string>("app_icon_url", (System.Action<string>) (x => iconUrl = x.Trim()));
        resJson.AssignIfContains<string>("app_name", (System.Action<string>) (x => appName = x.Trim()));
        resJson.AssignIfContains<string>("app_url", (System.Action<string>) (x => apkUrl = x.Trim()));
        resJson.AssignIfContains<string>("app_pkg", (System.Action<string>) (x => package = x.Trim()));
        this.ParentWindow.Dispatcher.Invoke((Delegate) (() =>
        {
          if (string.IsNullOrEmpty(apkUrl))
            this.ParentWindow.mWelcomeTab.OpenFrontendAppTabControl(package, PlayStoreAction.OpenApp);
          else
            this.DownloadAndInstallApp(iconUrl, appName, apkUrl, package, true, true, "");
        }));
      }
      catch (Exception ex)
      {
        Logger.Info("Error in Fle. Json string : " + campaignJson + "Error: " + ex.ToString());
      }
    }

    internal void DownloadAndInstallApp(
      string iconUrl,
      string appName,
      string apkUrl,
      string packageName,
      bool isLaunchAfterInstall,
      bool isDeleteApk,
      string timestamp = "")
    {
      if (this.ParentWindow.mWelcomeTab.mHomeAppManager.GetAppIcon(packageName) != null && !this.ParentWindow.mWelcomeTab.mHomeAppManager.GetAppIcon(packageName).IsAppSuggestionActive)
      {
        if (this.ParentWindow.mAppHandler.IsAppInstalled(packageName))
        {
          if (!string.IsNullOrEmpty(timestamp))
          {
            bool flag = true;
            DateTime dateTime1 = DateTime.Parse(timestamp, (IFormatProvider) CultureInfo.InvariantCulture);
            DateTime maxValue = DateTime.MaxValue;
            if (this.ParentWindow.mAppHandler.CdnAppdict.ContainsKey(packageName))
            {
              DateTime dateTime2 = this.ParentWindow.mAppHandler.CdnAppdict[packageName];
              if (dateTime1 <= dateTime2)
                flag = false;
            }
            if (flag)
            {
              CustomMessageWindow customMessageWindow = new CustomMessageWindow();
              BlueStacksUIBinding.Bind(customMessageWindow.TitleTextBlock, "STRING_INSTALL_UPDATE", "");
              BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlock, "STRING_APP_UPGRADE", "");
              customMessageWindow.AddButton(ButtonColors.Blue, "STRING_UPGRADE_TEXT", (EventHandler) ((sender1, e1) => this.DownloadApk(iconUrl, appName, apkUrl, packageName, isLaunchAfterInstall, isDeleteApk, timestamp)), (string) null, false, (object) null, true);
              customMessageWindow.AddButton(ButtonColors.White, "STRING_CONTINUE_ANYWAY", (EventHandler) ((sender1, e1) => this.ParentWindow.mAppHandler.SendRunAppRequestAsync(packageName, "", false)), (string) null, false, (object) null, true);
              customMessageWindow.Owner = (Window) this.ParentWindow;
              customMessageWindow.ShowDialog();
            }
            else
              this.ParentWindow.mAppHandler.SendRunAppRequestAsync(packageName, "", false);
          }
          else
            this.ParentWindow.mAppHandler.SendRunAppRequestAsync(packageName, "", false);
        }
        else
          this.ParentWindow.mTopBar.mAppTabButtons.GoToTab("Home", true, false);
      }
      else
        this.DownloadApk(iconUrl, appName, apkUrl, packageName, isLaunchAfterInstall, isDeleteApk, timestamp);
    }

    internal void DownloadApk(
      string iconUrl,
      string appName,
      string apkUrl,
      string packageName,
      bool isLaunchAfterInstall,
      bool isDeleteApk,
      string timestamp)
    {
      if (string.IsNullOrEmpty(apkUrl))
        return;
      Logger.Info("apkUrl: " + apkUrl);
      Utils.TinyDownloader(iconUrl, packageName + ".png", "", false);
      this.ParentWindow.mWelcomeTab.mHomeAppManager.AddAppIcon(packageName, appName, apkUrl, this);
      this.DownloadApk(apkUrl, packageName, isLaunchAfterInstall, isDeleteApk, timestamp);
    }

    public void DownloadApk(
      string apkUrl,
      string packageName,
      bool isLaunchAfterInstall,
      bool isDeleteApk,
      string timestamp = "")
    {
      string str = Path.Combine(RegistryStrings.DataDir, "DownloadedApk");
      if (!Directory.Exists(str))
        Directory.CreateDirectory(str);
      string path2 = Regex.Replace(packageName + ".apk", "[\\x22\\\\\\/:*?|<>]", " ");
      string apkFilePath = Path.Combine(str, path2);
      Logger.Info("Downloading Apk file to: " + apkFilePath);
      this.ParentWindow.mWelcomeTab.mHomeAppManager.DownloadStarted(packageName);
      ClientStats.SendClientStatsAsync("download", "unknown", "app_install", packageName, "", "");
      this.mDownloadThread = new Thread((ThreadStart) (() =>
      {
        string apkUrl1 = apkUrl;
        if (DownloadInstallApk.IsContainsGoogleAdId(apkUrl1))
          apkUrl = this.AddGoogleAdidWithApk(apkUrl1);
        apkUrl = BlueStacksUIUtils.GetFinalRedirectedUrl(apkUrl);
        if (string.IsNullOrEmpty(apkUrl))
          return;
        this.mIsDownloading = true;
        this.mDownloader = new LegacyDownloader(3, apkUrl, apkFilePath);
        this.mDownloader.Download((LegacyDownloader.UpdateProgressCallback) (percent => this.ParentWindow.mWelcomeTab.mHomeAppManager.UpdateAppDownloadProgress(packageName, percent)), (LegacyDownloader.DownloadCompletedCallback) (filePath =>
        {
          ClientStats.SendClientStatsAsync("download", "success", "app_install", packageName, "", "");
          this.mIsDownloading = false;
          this.ParentWindow.mWelcomeTab.mHomeAppManager.DownloadCompleted(packageName, filePath);
          this.InstallApk(packageName, filePath, isLaunchAfterInstall, isDeleteApk, timestamp);
          DownloadInstallApk.sDownloadedApkList.Add(packageName);
        }), (LegacyDownloader.ExceptionCallback) (ex =>
        {
          ClientStats.SendClientStatsAsync("download", "fail", "app_install", packageName, "", "");
          this.ParentWindow.mWelcomeTab.mHomeAppManager.DownloadFailed(packageName);
          Logger.Error("Failed to download file: {0}. err: {1}", (object) apkFilePath, (object) ex.Message);
        }), (LegacyDownloader.ContentTypeCallback) null, (LegacyDownloader.SizeDownloadedCallback) null, (LegacyDownloader.PayloadInfoCallback) null);
      }))
      {
        IsBackground = true
      };
      this.mDownloadThread.Start();
    }

    private string AddGoogleAdidWithApk(string apkUrl)
    {
      Logger.Info("In AddGoogleAdidWithApk");
      string newValue1 = "google_aid=00000000-0000-0000-0000-000000000000";
      string str;
      try
      {
        JObject jobject = JObject.Parse(HTTPUtils.SendRequestToGuest("getGoogleAdID", (Dictionary<string, string>) null, this.ParentWindow.mVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp"));
        if (jobject["result"].ToString() == "ok")
        {
          string newValue2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "google_aid={0}", (object) jobject["googleadid"].ToString());
          str = apkUrl.Replace("google_aid={google_aid}", newValue2);
        }
        else
          str = apkUrl.Replace("google_aid={google_aid}", newValue1);
      }
      catch (Exception ex)
      {
        Logger.Error("Error in adding google adId" + ex.ToString());
        str = apkUrl.Replace("google_aid={google_aid}", newValue1);
      }
      return str;
    }

    private static bool IsContainsGoogleAdId(string apkUrl)
    {
      return apkUrl.ToLower(CultureInfo.InvariantCulture).Contains("google_aid={google_aid}");
    }

    internal void AbortApkDownload(string packageName)
    {
      ClientStats.SendClientStatsAsync("download", "cancel", "app_install", packageName, "", "");
      if (this.mDownloader != null)
        this.mDownloader.AbortDownload();
      if (this.mDownloadThread == null)
        return;
      this.mDownloadThread.Abort();
    }

    internal void ChooseAndInstallApk()
    {
      OpenFileDialog openFileDialog1 = new OpenFileDialog();
      openFileDialog1.Filter = "Android Files (*.apk, *.xapk)|*.apk; *.xapk";
      openFileDialog1.Multiselect = true;
      openFileDialog1.RestoreDirectory = true;
      using (OpenFileDialog openFileDialog2 = openFileDialog1)
      {
        if (openFileDialog2.ShowDialog() == DialogResult.OK)
        {
          foreach (string fileName in openFileDialog2.FileNames)
          {
            Logger.Info("File Selected : " + fileName);
            this.InstallApk(fileName, true);
          }
        }
        ClientStats.SendMiscellaneousStatsAsync("close_window_apk", RegistryManager.Instance.UserGuid, "close_apk_window", string.Empty, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
      }
    }

    internal void InstallApk(string apkPath, bool addToChooseApkList = false)
    {
      Logger.Info("Console: Installing apk: {0}", (object) apkPath);
      string package = string.Empty;
      string appName = string.Empty;
      if (string.Equals(Path.GetExtension(apkPath), ".xapk", StringComparison.InvariantCultureIgnoreCase))
      {
        JToken infoFromXapk = Utils.ExtractInfoFromXapk(apkPath);
        if (infoFromXapk != null)
        {
          package = infoFromXapk.GetValue("package_name");
          appName = infoFromXapk.GetValue("name");
          Logger.Debug("Package name from manifest.json.." + package);
        }
      }
      else
      {
        AppInfoExtractor apkInfo = AppInfoExtractor.GetApkInfo(apkPath);
        package = apkInfo.PackageName;
        appName = apkInfo.AppName;
      }
      ClientStats.SendMiscellaneousStatsAsync("open_apk", RegistryManager.Instance.UserGuid, "import_apk", package, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
      if (addToChooseApkList)
        DownloadInstallApk.sApkInstalledFromChooser.Add(package);
      if (!string.IsNullOrEmpty(package))
        this.ParentWindow.Dispatcher.Invoke((Delegate) (() => this.ParentWindow.mWelcomeTab.mHomeAppManager.AddAppIcon(package, appName, string.Empty, this)));
      this.InstallApk(package, apkPath, false, false, "");
    }

    internal void InstallApk(
      string packageName,
      string apkPath,
      bool isLaunchAfterInstall,
      bool isDeleteApk,
      string timestamp = "")
    {
      this.ParentWindow.mWelcomeTab.mHomeAppManager.ApkInstallStart(packageName, apkPath);
      DownloadInstallApk.SerialWorkQueueInstaller(this.ParentWindow.mVmName).Enqueue((SerialWorkQueue.Work) (() =>
      {
        Logger.Info("Installing apk: {0}", (object) apkPath);
        int num = BluestacksProcessHelper.RunApkInstaller(apkPath, true, this.ParentWindow.mVmName);
        Logger.Info("Apk installer exit code: {0}", (object) num);
        if (num == 0)
        {
          if (DownloadInstallApk.sDownloadedApkList.Contains(packageName))
          {
            ClientStats.SendClientStatsAsync("install_from_download", "success", "app_install", packageName, "", "");
            DownloadInstallApk.sDownloadedApkList.Remove(packageName);
            this.UpdateCdnAppEntry(true, packageName, timestamp);
          }
          else if (DownloadInstallApk.sApkInstalledFromChooser.Contains(packageName))
          {
            ClientStats.SendClientStatsAsync("install", "success", "app_install", packageName, "", "");
            DownloadInstallApk.sApkInstalledFromChooser.Remove(packageName);
          }
          this.ParentWindow.mWelcomeTab.mHomeAppManager.ApkInstallCompleted(packageName);
          if (isLaunchAfterInstall)
            this.ParentWindow.Utils.RunAppOrCreateTabButton(packageName);
          Logger.Info("Installation successful.");
          if (isDeleteApk)
            File.Delete(apkPath);
          Logger.Info("Install Completed : " + packageName);
        }
        else
        {
          if (DownloadInstallApk.sDownloadedApkList.Contains(packageName))
          {
            ClientStats.SendClientStatsAsync("install_from_download", "fail", "app_install", packageName, num.ToString((IFormatProvider) CultureInfo.InvariantCulture), "");
            DownloadInstallApk.sDownloadedApkList.Remove(packageName);
          }
          else if (DownloadInstallApk.sApkInstalledFromChooser.Contains(packageName))
          {
            ClientStats.SendClientStatsAsync("install", "fail", "app_install", packageName, num.ToString((IFormatProvider) CultureInfo.InvariantCulture), "");
            DownloadInstallApk.sApkInstalledFromChooser.Remove(packageName);
          }
          ClientStats.SendGeneralStats("apk_inst_error", new Dictionary<string, string>()
          {
            {
              "errcode",
              Convert.ToString(num, (IFormatProvider) CultureInfo.InvariantCulture)
            },
            {
              "precode",
              "0"
            },
            {
              "app_pkg",
              packageName
            }
          });
          this.ParentWindow.mWelcomeTab.mHomeAppManager.ApkInstallFailed(packageName);
        }
      }));
    }

    internal int InstallFLEApk(string packageName, string apkPath)
    {
      Logger.Info("Installing apk: {0}", (object) apkPath);
      int num = BluestacksProcessHelper.RunApkInstaller(apkPath, true, this.ParentWindow.mVmName);
      Logger.Info("Apk installer exit code: {0}", (object) num);
      if (num == 0)
      {
        if (DownloadInstallApk.sDownloadedApkList.Contains(packageName))
        {
          ClientStats.SendClientStatsAsync("install_from_download", "success", "app_install", packageName, "", "");
          DownloadInstallApk.sDownloadedApkList.Remove(packageName);
          this.UpdateCdnAppEntry(true, packageName, "");
        }
        else if (DownloadInstallApk.sApkInstalledFromChooser.Contains(packageName))
        {
          ClientStats.SendClientStatsAsync("install", "success", "app_install", packageName, "", "");
          DownloadInstallApk.sApkInstalledFromChooser.Remove(packageName);
        }
        this.ParentWindow.Utils.RunAppOrCreateTabButton(packageName);
        Logger.Info("Installation successful.");
        File.Delete(apkPath);
      }
      else
      {
        if (DownloadInstallApk.sDownloadedApkList.Contains(packageName))
        {
          ClientStats.SendClientStatsAsync("install_from_download", "fail", "app_install", packageName, num.ToString((IFormatProvider) CultureInfo.InvariantCulture), "");
          DownloadInstallApk.sDownloadedApkList.Remove(packageName);
        }
        else if (DownloadInstallApk.sApkInstalledFromChooser.Contains(packageName))
        {
          ClientStats.SendClientStatsAsync("install", "fail", "app_install", packageName, num.ToString((IFormatProvider) CultureInfo.InvariantCulture), "");
          DownloadInstallApk.sApkInstalledFromChooser.Remove(packageName);
        }
        ClientStats.SendGeneralStats("apk_inst_error", new Dictionary<string, string>()
        {
          {
            "errcode",
            Convert.ToString(num, (IFormatProvider) CultureInfo.InvariantCulture)
          },
          {
            "precode",
            "0"
          },
          {
            "app_pkg",
            packageName
          }
        });
      }
      Logger.Info("Install Completed : " + packageName);
      return num;
    }

    internal void UninstallApp(string packageName)
    {
      DownloadInstallApk.SerialWorkQueueInstaller(this.ParentWindow.mVmName).Enqueue((SerialWorkQueue.Work) (() =>
      {
        Logger.Info("Uninstall started : " + packageName);
        Dictionary<string, string> data = new Dictionary<string, string>()
        {
          ["package"] = packageName
        };
        try
        {
          JArray jarray = JArray.Parse(HTTPUtils.SendRequestToAgent("uninstall", data, this.ParentWindow.mVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp", true));
          try
          {
            if (!JObject.Parse(jarray[0].ToString())["success"].ToObject<bool>())
              ClientStats.SendClientStatsAsync("uninstall", "fail", "app_install", packageName, "", "");
            else
              this.UpdateCdnAppEntry(false, packageName, "");
          }
          catch
          {
          }
        }
        catch (Exception ex)
        {
          Logger.Error("Failed to uninstall app. Err: " + ex.Message);
        }
        Logger.Info("Uninstall completed for " + packageName);
      }));
    }

    internal void UpdateCdnAppEntry(bool isAdd, string packageName, string timestamp)
    {
      DateTime minValue = DateTime.MinValue;
      if (!string.IsNullOrEmpty(timestamp))
        minValue = DateTime.Parse(timestamp, (IFormatProvider) CultureInfo.InvariantCulture);
      this.ParentWindow.mAppHandler.WriteXMl(isAdd, packageName, minValue);
    }
  }
}
