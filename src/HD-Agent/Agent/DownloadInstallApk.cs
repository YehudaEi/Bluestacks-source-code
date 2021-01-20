// Decompiled with JetBrains decompiler
// Type: BlueStacks.Agent.DownloadInstallApk
// Assembly: HD-Agent, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 06DAED18-1D79-40C2-83F8-3A28B5222574
// Assembly location: C:\Program Files\BlueStacks\HD-Agent.exe

using BlueStacks.Common;
using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;

namespace BlueStacks.Agent
{
  internal class DownloadInstallApk
  {
    public static void DownloadApk(
      string apkUrl,
      string packageName,
      string vmName,
      bool isUpgrade)
    {
      string input = packageName + ".apk";
      string str1 = Path.Combine(RegistryStrings.DataDir, "DownloadedApk");
      if (!Directory.Exists(str1))
        Directory.CreateDirectory(str1);
      string path2 = Regex.Replace(input, "[\\x22\\\\\\/:*?|<>]", " ");
      string apkFilePath = Path.Combine(str1, path2);
      Stats.SendMiscellaneousStatsAsyncForDMM(Stats.DMMEvent.download_start.ToString(), apkUrl, packageName, isUpgrade.ToString(), (string) null, "Android", 0);
      if (!DownloadInstallApk.AddApkStatusToList(packageName, isUpgrade, vmName))
        DownloadInstallApk.DeleteFileParts(apkFilePath);
      else if (string.IsNullOrEmpty(apkUrl))
      {
        Stats.SendMiscellaneousStatsAsync("ApkDownloadFailure", apkUrl, "Empty Url", "Invalid Url", packageName, (string) null, (string) null, RegistryManager.Instance.UserGuid, RegistryManager.Instance.Version, "Android", 0);
        Stats.SendMiscellaneousStatsAsyncForDMM(Stats.DMMEvent.download_failed.ToString(), "Empty Url", packageName, isUpgrade.ToString(), (string) null, "Android", 0);
      }
      else
      {
        Logger.Info("Downloading Apk file to: " + apkFilePath);
        new Thread((ThreadStart) (() => new LegacyDownloader(1, apkUrl, apkFilePath).Download((LegacyDownloader.UpdateProgressCallback) (percent => {}), (LegacyDownloader.DownloadCompletedCallback) (filePath =>
        {
          Stats.SendMiscellaneousStatsAsyncForDMM(Stats.DMMEvent.download_complete.ToString(), apkUrl, packageName, (string) null, (string) null, "Android", 0);
          DownloadInstallApk.UpdateInstallStatus(packageName, vmName, 0U, isUpgrade ? DownloadInstallStatus.Upgrading : DownloadInstallStatus.Installing);
          DownloadInstallApk.InstallApk(packageName, filePath, vmName);
        }), (LegacyDownloader.ExceptionCallback) (ex =>
        {
          string str2 = "DownloadFailed";
          try
          {
            str2 = (ex as WebException).Status.ToString();
          }
          catch
          {
          }
          string message = ex.Message;
          DownloadInstallApk.DeleteFileParts(apkFilePath);
          if (message.Contains(Convert.ToString(ReturnCodesUInt.DOWNLOAD_FAILED_INVALID_STATUS_CODE)))
            DownloadInstallApk.UpdateDownloadStatus(packageName, vmName, ReturnCodesUInt.DOWNLOAD_FAILED_INVALID_STATUS_CODE);
          else if (message.Contains(Convert.ToString(ReturnCodesUInt.DOWNLOAD_FAILED_HOSTNAME_NOT_RESOLVED)))
            DownloadInstallApk.UpdateDownloadStatus(packageName, vmName, ReturnCodesUInt.DOWNLOAD_FAILED_HOSTNAME_NOT_RESOLVED);
          else if (message.Contains(Convert.ToString(ReturnCodesUInt.DOWNLOAD_FAILED_OPERATION_TIMEOUT)))
            DownloadInstallApk.UpdateDownloadStatus(packageName, vmName, ReturnCodesUInt.DOWNLOAD_FAILED_OPERATION_TIMEOUT);
          else
            DownloadInstallApk.UpdateDownloadStatus(packageName, vmName, ReturnCodesUInt.DOWNLOAD_FAILED);
          ApkDownloadInstallStatus downloadInstallStatus = HDAgent.sApkDownloadInstallStatusList[vmName + packageName];
          Stats.SendMiscellaneousStatsAsync("ApkDownloadFailure", apkUrl, message, str2, packageName, downloadInstallStatus.downloadedSize.ToString(), downloadInstallStatus.payloadSize.ToString(), RegistryManager.Instance.UserGuid, RegistryManager.Instance.Version, "Android", 0);
          Stats.SendMiscellaneousStatsAsyncForDMM(Stats.DMMEvent.download_failed.ToString(), apkUrl, message, str2, packageName, "Android", 0);
          Logger.Error("Failed to download apk file: {0}. err: {1}", (object) apkFilePath, (object) message);
        }), (LegacyDownloader.ContentTypeCallback) null, (LegacyDownloader.SizeDownloadedCallback) (size => DownloadInstallApk.UpdateDownloadedSize(packageName, vmName, size)), (LegacyDownloader.PayloadInfoCallback) (payloadSize => DownloadInstallApk.UpdatePayloadSize(packageName, vmName, payloadSize)))))
        {
          IsBackground = true
        }.Start();
      }
    }

    private static void DeleteFileParts(string apkFilePath)
    {
      string path = string.Format("{0}_part_0", (object) apkFilePath);
      try
      {
        System.IO.File.Delete(path);
      }
      catch
      {
      }
      try
      {
        System.IO.File.Delete(apkFilePath);
      }
      catch
      {
      }
    }

    private static bool AddApkStatusToList(string packageName, bool isUpgrade, string vmName)
    {
      if (HDAgent.sApkDownloadInstallStatusList.ContainsKey(vmName + packageName))
      {
        ApkDownloadInstallStatus downloadInstallStatus = HDAgent.sApkDownloadInstallStatusList[vmName + packageName];
        if (HDAgent.sApkUpgradingList.ContainsKey(vmName + packageName) && !HDAgent.sApkUpgradingList[vmName + packageName] || (downloadInstallStatus.status == DownloadInstallStatus.Downloading || downloadInstallStatus.status == DownloadInstallStatus.Installing))
          return false;
        downloadInstallStatus.status = DownloadInstallStatus.Downloading;
        return true;
      }
      ApkDownloadInstallStatus downloadInstallStatus1 = new ApkDownloadInstallStatus()
      {
        vmName = vmName,
        packageName = packageName,
        isUpgrade = isUpgrade,
        status = DownloadInstallStatus.Downloading
      };
      HDAgent.sApkDownloadInstallStatusList.Add(vmName + packageName, downloadInstallStatus1);
      return true;
    }

    public static ApkDownloadInstallStatus GetApkStatusObject(
      string packageName,
      string vmName)
    {
      if (HDAgent.sApkDownloadInstallStatusList.ContainsKey(vmName + packageName))
        return HDAgent.sApkDownloadInstallStatusList[vmName + packageName];
      ApkDownloadInstallStatus downloadInstallStatus = new ApkDownloadInstallStatus()
      {
        vmName = vmName,
        packageName = packageName
      };
      HDAgent.sApkDownloadInstallStatusList.Add(vmName + packageName, downloadInstallStatus);
      return downloadInstallStatus;
    }

    private static void UpdatePayloadSize(string packageName, string vmName, long size)
    {
      if (!HDAgent.sApkDownloadInstallStatusList.ContainsKey(vmName + packageName))
        return;
      HDAgent.sApkDownloadInstallStatusList[vmName + packageName].payloadSize = size;
    }

    private static void UpdateDownloadedSize(string packageName, string vmName, long size)
    {
      if (!HDAgent.sApkDownloadInstallStatusList.ContainsKey(vmName + packageName))
        return;
      HDAgent.sApkDownloadInstallStatusList[vmName + packageName].downloadedSize = size;
    }

    private static void UpdateDownloadStatus(string packageName, string vmName, uint errorCode)
    {
      if (!HDAgent.sApkDownloadInstallStatusList.ContainsKey(vmName + packageName))
        return;
      ApkDownloadInstallStatus downloadInstallStatus = HDAgent.sApkDownloadInstallStatusList[vmName + packageName];
      downloadInstallStatus.status = DownloadInstallStatus.DownloadFailed;
      downloadInstallStatus.downloadFailedCode = errorCode;
    }

    private static void UpdateInstallStatus(
      string packageName,
      string vmName,
      uint errorCode,
      DownloadInstallStatus dis)
    {
      if (!HDAgent.sApkDownloadInstallStatusList.ContainsKey(vmName + packageName))
        return;
      ApkDownloadInstallStatus downloadInstallStatus = HDAgent.sApkDownloadInstallStatusList[vmName + packageName];
      downloadInstallStatus.status = dis;
      downloadInstallStatus.installFailedCode = errorCode;
    }

    private static string GetFinalRedirectedUrl(string apkUrl, ApkDownloadInstallStatus apkStatus)
    {
      string message;
      try
      {
        HttpWebRequest httpWebRequest = (HttpWebRequest) WebRequest.Create(apkUrl);
        httpWebRequest.Method = "GET";
        httpWebRequest.AllowAutoRedirect = true;
        httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/49.0.2623.110 Safari/537.36 " + ("Bluestacks/" + RegistryManager.Instance.ClientVersion);
        return httpWebRequest.GetResponse().ResponseUri.ToString();
      }
      catch (WebException ex)
      {
        Logger.Error("Web Exception :" + ex.ToString());
        apkStatus.status = DownloadInstallStatus.DownloadFailed;
        message = ex.Message;
        if (ex.Status == WebExceptionStatus.NameResolutionFailure)
        {
          Logger.Error("Could not resolve host for url = " + apkUrl);
          apkStatus.downloadFailedCode = ReturnCodesUInt.DOWNLOAD_FAILED_HOSTNAME_NOT_RESOLVED;
        }
        else
        {
          Logger.Error("Exception status : " + ex.Status.ToString());
          apkStatus.downloadFailedCode = ReturnCodesUInt.DOWNLOAD_FAILED_INVALID_STATUS_CODE;
        }
      }
      catch (Exception ex)
      {
        message = ex.Message;
        Logger.Error("Error in getting redirected url" + ex.ToString());
        apkStatus.status = DownloadInstallStatus.DownloadFailed;
        apkStatus.downloadFailedCode = ReturnCodesUInt.DOWNLOAD_FAILED_INVALID_DOWNLOAD_URL;
      }
      Stats.SendMiscellaneousStatsAsync("ApkDownloadFailure", apkUrl, message, "Invalid Url", apkStatus.packageName, (string) null, (string) null, RegistryManager.Instance.UserGuid, RegistryManager.Instance.Version, "Android", 0);
      return (string) null;
    }

    private static void InstallApk(string packageName, string apkPath, string vmName)
    {
      Logger.Info("Installing apk: {0}", (object) apkPath);
      string str = ApkInstall.InstallApk(apkPath, vmName);
      Logger.Info("Apk installer exit result : {0}", (object) str);
      Thread.Sleep(2000);
      if (str.Equals("Success", StringComparison.InvariantCultureIgnoreCase))
      {
        Stats.SendMiscellaneousStatsAsyncForDMM(Stats.DMMEvent.app_install_success.ToString(), str, packageName, apkPath, (string) null, "Android", 0);
        HDAgent.sApkUpgradingList[vmName + packageName] = false;
        HDAgent.sApkDownloadInstallStatusList.Remove(vmName + packageName);
        Logger.Info("Installation successful : " + packageName);
        try
        {
          System.IO.File.Delete(apkPath);
        }
        catch
        {
        }
      }
      else
      {
        Stats.SendMiscellaneousStatsAsyncForDMM(Stats.DMMEvent.app_install_failed.ToString(), str, packageName, apkPath, (string) null, "Android", 0);
        int num = 24;
        try
        {
          num = (int) System.Enum.Parse(typeof (ReturnCodes), str);
        }
        catch
        {
          try
          {
            num = (int) System.Enum.Parse(typeof (GuestApkInstallFailCodes), str);
          }
          catch
          {
            Logger.Error("unable to parse install apk error code: {0}", (object) str);
          }
        }
        uint errorCode = 0;
        if (num > 0)
          errorCode = ReturnCodesUInt.INSTALL_FAILED_HOST_BASE_VALUE + (uint) num;
        else if (num < 0)
          errorCode = ReturnCodesUInt.INSTALL_FAILED_GUEST_BASE_VALUE - (uint) num;
        Logger.Info("Installation failed : " + packageName + ", Error code : " + errorCode.ToString());
        DownloadInstallApk.UpdateInstallStatus(packageName, vmName, errorCode, DownloadInstallStatus.InstallFailed);
        Stats.SendMiscellaneousStatsAsync("ApkInstallFailure", packageName, str, errorCode.ToString(), RegistryManager.Instance.UserGuid, RegistryManager.Instance.Version, "bgp", (string) null, (string) null, "Android", 0);
      }
    }
  }
}
