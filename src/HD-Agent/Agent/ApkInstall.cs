// Decompiled with JetBrains decompiler
// Type: BlueStacks.Agent.ApkInstall
// Assembly: HD-Agent, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 06DAED18-1D79-40C2-83F8-3A28B5222574
// Assembly location: C:\Program Files\BlueStacks\HD-Agent.exe

using BlueStacks.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace BlueStacks.Agent
{
  public class ApkInstall
  {
    public static AppInfo[] sOriginalJson = (AppInfo[]) null;
    public static string sAppName = (string) null;
    private static string sReturnString = (string) null;
    public static string[] sIgnoreEvents = new string[1]
    {
      "com.bluestacks.chartapp"
    };
    public static string sPackageName;
    public static string sAppIcon;
    public static string sLaunchableActivityName;
    public static string sVersion;
    public static bool sGl3Required;
    public static bool sVideoPresent;
    public static string sVersionName;
    public static bool sIsGamepadCompatible;

    [DllImport("Kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern bool CreateHardLink(
      string lpFileName,
      string lpExistingFileName,
      IntPtr lpSecurityAttributes);

    private static bool IsDiskFull(Exception ex)
    {
      int num = Marshal.GetHRForException(ex) & (int) ushort.MaxValue;
      return num == 39 || num == 112;
    }

    public static bool CreateHardLinkForFile(string existingFilePath, string newFilePath)
    {
      try
      {
        Logger.Info("Creating hard link from " + existingFilePath + " to " + newFilePath);
        int num = ApkInstall.CreateHardLink(newFilePath, existingFilePath, IntPtr.Zero) ? 1 : 0;
        if (num == 0)
          Logger.Error("Failed to create hard link: " + Marshal.GetLastWin32Error().ToString() + ".." + new Win32Exception()?.ToString());
        else
          Logger.Info("Hard link created successfully");
        return num != 0;
      }
      catch (Exception ex)
      {
        Logger.Error("Error in creating hard link.." + ex?.ToString());
        return false;
      }
    }

    public static string InstallApk(string apk, string vmName)
    {
      string str1 = "";
      Logger.Info("apk to be installed is " + apk);
      Stats.SendMiscellaneousStatsAsyncForDMM(Stats.DMMEvent.app_install_started.ToString(), apk, (string) null, (string) null, (string) null, "Android", 0);
      try
      {
        int num1 = RegistryManager.Instance.Guest[vmName].FileSystem;
        string json = (string) null;
        if (num1 == 1)
        {
          try
          {
            string sharedFolder0Name = RegistryManager.Instance.Guest[vmName].SharedFolder0Name;
            string sharedFolder0Path = RegistryManager.Instance.Guest[vmName].SharedFolder0Path;
            if (string.IsNullOrEmpty(sharedFolder0Name) || string.IsNullOrEmpty(sharedFolder0Path))
            {
              Logger.Error("Name or Path missing in sharedfolder regkey");
              num1 = 0;
            }
            else
            {
              str1 = Path.Combine(sharedFolder0Path, "Bst-" + DateTime.Now.Ticks.ToString() + Path.GetExtension(apk));
              Logger.Info("apk = {0}, newPath = {1}", (object) apk, (object) str1);
              try
              {
                if (!ApkInstall.CreateHardLinkForFile(apk, str1))
                  System.IO.File.Copy(apk, str1, true);
              }
              catch (Exception ex)
              {
                if (ApkInstall.IsDiskFull(ex))
                {
                  Logger.Info("Disk Space is full");
                  Stats.SendMiscellaneousStatsAsyncForDMM(Stats.DMMEvent.app_install_failed.ToString(), "INSTALL_FAILED_INSUFFICIENT_STORAGE_HOST", apk, (string) null, (string) null, "Android", 0);
                  return "INSTALL_FAILED_INSUFFICIENT_STORAGE_HOST";
                }
                Logger.Error("Unable to copy file" + ex.ToString());
                num1 = 0;
                throw new Exception("File_Not_Copied");
              }
              System.IO.File.SetAttributes(str1, FileAttributes.Normal);
              string str2 = "/mnt/sdcard/windows/" + sharedFolder0Name + "/" + Path.GetFileName(str1);
              Logger.Info("androidPath: " + str2);
              Dictionary<string, string> data = new Dictionary<string, string>()
              {
                {
                  "path",
                  str2
                }
              };
              if (!Utils.IsUIProcessAlive(vmName, "bgp"))
              {
                Logger.Info("Starting Frontend in hidden mode.");
                Utils.StartHiddenFrontend(vmName, "bgp");
              }
              new Thread((ThreadStart) (() =>
              {
                if (!Utils.WaitForFrontendPingResponse(vmName))
                {
                  Logger.Info("Frontend not responding to ping response for 50 seconds, aborting installtion with FRONTEND_NOT_STARTING error");
                  HTTPHandler.sApkInstallResult = "FRONTEND_NOT_STARTING";
                  Stats.SendMiscellaneousStatsAsyncForDMM(Stats.DMMEvent.app_install_failed.ToString(), "FRONTEND_NOT_STARTING", apk, (string) null, (string) null, "Android", 0);
                  if (HTTPHandler.sApkInstallThread == null)
                    return;
                  HTTPHandler.sApkInstallThread.Abort();
                }
                else
                {
                  Logger.Info("Frontend is running gave ping response");
                  while (HTTPHandler.sApkInstallThread != null)
                  {
                    if (Utils.IsUIProcessAlive(vmName, "bgp"))
                    {
                      Thread.Sleep(500);
                    }
                    else
                    {
                      Logger.Info("Frontend Process Stopped, aborting Installation with FRONTEND_NOT_RUNNING error");
                      Stats.SendMiscellaneousStatsAsyncForDMM(Stats.DMMEvent.app_install_failed.ToString(), "FRONTEND_NOT_RUNNING", apk, (string) null, (string) null, "Android", 0);
                      HTTPHandler.sApkInstallResult = "FRONTEND_NOT_RUNNING";
                      HTTPHandler.sApkInstallThread.Abort();
                      break;
                    }
                  }
                }
              }))
              {
                IsBackground = true
              }.Start();
              if ((RegistryManager.Instance.Guest[vmName].ConfigSynced == 0 ? 1 : 0) != 0 && !Utils.WaitForSyncConfig(vmName))
              {
                Logger.Info("Config still not sycned");
                Stats.SendMiscellaneousStatsAsyncForDMM(Stats.DMMEvent.app_install_failed.ToString(), "CONFIG_NOT_SYNCED", apk, (string) null, (string) null, "Android", 0);
                return "CONFIG_NOT_SYNCED";
              }
              int retries = 180;
              if (Oem.IsOEMDmm)
                retries = 1;
              if (!Utils.CheckIfGuestReady(vmName, retries))
                return Oem.IsOEMDmm ? "ANDROID_BOOT_FAILURE" : "GUEST_NOT_READY_YET";
              try
              {
                if (string.Equals(Path.GetExtension(apk), ".xapk", StringComparison.InvariantCultureIgnoreCase))
                {
                  Logger.Info("HTTPHandler: Sending /xinstall request to guest");
                  json = HTTPUtils.SendRequestToGuest("xinstall", data, vmName, 300000, (Dictionary<string, string>) null, false, 1, 0, "bgp");
                }
                else
                {
                  Logger.Info("HTTPHandler: Sending /install request to guest");
                  json = HTTPUtils.SendRequestToGuest("install", data, vmName, 300000, (Dictionary<string, string>) null, false, 1, 0, "bgp");
                }
              }
              catch (WebException ex)
              {
                Logger.Error("WebException occured while installing apk.");
                Logger.Error(ex.ToString());
                return ex.Status == WebExceptionStatus.Timeout ? "INSTALL_APK_TIMEOUT" : "INSTALL_APK_CONNECTION_TERMINATED";
              }
              Logger.Info("Install apk respnse : " + json);
              int num2 = 20;
              while (json.Contains("INSTALL_FAILED_INVALID_URI") && num2 > 0)
              {
                Logger.Info("Retrying install since INSTALL_FAILED_INVALID_URI..." + num2.ToString());
                --num2;
                try
                {
                  json = !string.Equals(Path.GetExtension(apk), ".xapk", StringComparison.InvariantCultureIgnoreCase) ? HTTPUtils.SendRequestToGuest("install", data, vmName, 180000, (Dictionary<string, string>) null, false, 1, 0, "bgp") : HTTPUtils.SendRequestToGuest("xinstall", data, vmName, 180000, (Dictionary<string, string>) null, false, 1, 0, "bgp");
                }
                catch
                {
                }
                Thread.Sleep(1000);
              }
              if (!json.Contains("INSTALL_FAILED_INSUFFICIENT_STORAGE"))
              {
                if (!json.Contains("INSTALL_FAILED_INVALID_URI"))
                  goto label_36;
              }
              num1 = 0;
            }
          }
          catch (Exception ex1)
          {
            Logger.Error(ex1.ToString());
            if (!ex1.Message.Equals("File_Not_Copied"))
            {
              if (System.IO.File.Exists(str1))
              {
                Logger.Info("Deleting copied apk from sharedfolder");
                try
                {
                  System.IO.File.Delete(str1);
                }
                catch (Exception ex2)
                {
                  Logger.Error("Unable to delete file {0}, Error: {1}", (object) str1, (object) ex2.ToString());
                }
              }
            }
          }
        }
label_36:
        try
        {
          if (num1 == 0)
          {
            Logger.Info("Sending apk");
            json = !string.Equals(Path.GetExtension(apk), ".xapk", StringComparison.InvariantCultureIgnoreCase) ? ApkInstall.PostFileToGuest("install", apk, vmName) : ApkInstall.PostFileToGuest("xinstall", apk, vmName);
            if (json == null)
            {
              Logger.Error("No response received yet.");
              return "INSTALL_FAILED_UPLOAD_APK_ERROR";
            }
          }
        }
        catch (Exception ex)
        {
          Logger.Error("Exception when sending install post request");
          Logger.Error(ex.ToString());
          return "INSTALL_FAILED_SERVER_ERROR";
        }
        JObject jobject = JObject.Parse(json);
        ApkInstall.sReturnString = jobject["result"].ToString() == "ok" ? "SUCCESS" : jobject["reason"].ToString();
        if (System.IO.File.Exists(str1))
        {
          try
          {
            System.IO.File.Delete(str1);
          }
          catch (Exception ex)
          {
            Logger.Error("Unable to delete file {0}, Error: {1}", (object) str1, (object) ex.ToString());
          }
        }
        return ApkInstall.sReturnString;
      }
      catch (Exception ex)
      {
        Logger.Error(ex.ToString());
        return "UNKNOWN_ERROR";
      }
    }

    public static string PostFileToGuest(string route, string file, string vmName)
    {
      string address = string.Format("{0}/{1}", (object) HTTPUtils.GuestServerUrl(vmName, "bgp"), (object) route);
      Logger.Info("Uploading {0} to {1}", (object) file, (object) address);
      ExtendedWebClient extendedWebClient = new ExtendedWebClient(200000);
      string str = (string) null;
      int num = 1;
      while (num > 0)
      {
        try
        {
          str = Encoding.UTF8.GetString(extendedWebClient.UploadFile(address, file));
          break;
        }
        catch (WebException ex)
        {
          Logger.Warning("Exception in post file");
          HttpWebResponse response = (HttpWebResponse) ex.Response;
          Logger.Error(ex.Message);
          if (response != null)
          {
            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
              if (num == 1)
                throw;
              else
                num = 2;
            }
          }
        }
        --num;
        if (num > 0)
          Thread.Sleep(2000);
      }
      Logger.Info("Got response for {0}: {1}", (object) route, (object) str);
      return str;
    }

    public static void AppInstalled(
      string name,
      string package,
      string activity,
      string img,
      string version,
      string isUpdate,
      string vmName,
      string source,
      string campaignName,
      string clientVersion,
      bool isGl3Required,
      bool isVideoPresent,
      string apkType,
      string versionName)
    {
      Logger.Info("Replacing invalid characters, if any, with whitespace");
      ApkInstall.sAppName = Regex.Replace(name, "[\\x22\\\\\\/:*?|<>]", " ");
      ApkInstall.sPackageName = package;
      ApkInstall.sLaunchableActivityName = activity;
      ApkInstall.sAppIcon = img;
      ApkInstall.sVersion = version;
      ApkInstall.sGl3Required = isGl3Required;
      ApkInstall.sVideoPresent = isVideoPresent;
      ApkInstall.sVersionName = versionName;
      ApkInstall.sIsGamepadCompatible = Utils.CheckGamepadCompatible(package);
      ApkInstall.sOriginalJson = new JsonParser(vmName).GetAppList();
      ApkInstall.AddToJson(vmName);
      Stats.SendAppInstallStats(name, package, version, versionName, "true", isUpdate, source, vmName, campaignName, clientVersion, apkType);
      string str1 = Path.Combine(RegistryStrings.GadgetDir, string.Format("{0}.{1}.png", (object) package, (object) activity));
      string str2 = Path.Combine(RegistryStrings.GadgetDir, string.Format("{0}.png", (object) package));
      if (System.IO.File.Exists(str2))
      {
        if (!System.IO.File.Exists(str1))
          System.IO.File.Copy(str2, str1, false);
      }
      else if (Oem.Instance.IsDownloadIconFromWeb)
      {
        try
        {
          Utils.DownloadIcon(package, "", false);
        }
        catch
        {
        }
      }
      if (Oem.Instance.IsOEMWithBGPClient)
        ApkInstall.InformPartner(vmName, package, isUpdate);
      if (Features.IsFeatureEnabled(8192UL))
        ApkInstall.CreateAppShortcut(vmName, isUpdate);
      Logger.Info("InstallApk: Got AppName: {0}", (object) ApkInstall.sAppName);
      string str3 = ApkInstall.sAppName + " ";
      string message = string.Compare(isUpdate, "true", true) != 0 ? str3 + LocaleStrings.GetLocalizedString("STRING_INSTALL_SUCCESS", "") : str3 + LocaleStrings.GetLocalizedString("STRING_UPDATE_SUCCESS", "");
      try
      {
        if (Features.IsFeatureEnabled(2UL))
        {
          if (Array.IndexOf<string>(ApkInstall.sIgnoreEvents, package) == -1)
            SysTray.ShowInfoShort(LocaleStrings.GetLocalizedString("STRING_BLUESTACKS", ""), message, vmName, package, "0");
          else
            Logger.Debug("Not showing notification for: " + package);
        }
        else
          Logger.Info("Not showing install notification...");
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to get ShowInstallerNotification value. err: " + ex.Message);
      }
    }

    private static void CreateAppShortcut(string vmName, string isUpdate)
    {
      try
      {
        Logger.Info("Creating app shortcut");
        string iconCompletePath = RegistryStrings.ProductIconCompletePath;
        string str1 = Path.Combine(RegistryStrings.GadgetDir, ApkInstall.sPackageName + ".png");
        string imagePath = Path.Combine(RegistryStrings.GadgetDir, ApkInstall.sPackageName + "." + ApkInstall.sLaunchableActivityName + ".png");
        if (!System.IO.File.Exists(str1))
          str1 = Path.Combine(RegistryStrings.GadgetDir, ApkInstall.sAppIcon);
        Logger.Info("Image path " + str1);
        string str2;
        try
        {
          str2 = Utils.ConvertToIco(str1, RegistryStrings.GadgetDir);
        }
        catch (Exception ex)
        {
          Logger.Error(ex.ToString());
          str2 = Utils.ConvertToIco(imagePath, RegistryStrings.GadgetDir);
        }
        if (!System.IO.File.Exists(str2))
          str2 = iconCompletePath;
        if (string.Compare(isUpdate, "true", true) == 0 || !Oem.Instance.IsCreateDesktopIconForApp || (!RegistryManager.Instance.AddDesktopShortcuts || string.Compare(vmName, "Android", true) != 0))
          return;
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), ApkInstall.sAppName + ".lnk");
        string targetApplication = Path.Combine(RegistryStrings.InstallDir, "HD-RunApp.exe");
        string appRunAppJsonArg = Utils.GetAppRunAppJsonArg(ApkInstall.sAppName, ApkInstall.sPackageName);
        ShortcutHelper.CreateDesktopShortcut(ApkInstall.sAppName, str2, targetApplication, appRunAppJsonArg, "", ApkInstall.sPackageName);
      }
      catch (Exception ex)
      {
        Logger.Warning("Couldn't create desktop shortcut, ex: " + ex.ToString());
      }
    }

    private static void ResizeImage(string imagePath)
    {
      int num = 5;
      while (num > 0)
      {
        --num;
        try
        {
          Utils.ResizeImage(imagePath);
          break;
        }
        catch (Exception ex)
        {
          Logger.Error("Failed to resize image. Err: " + ex.ToString());
          Thread.Sleep(1000);
        }
      }
    }

    private static void InformPartner(string vmName, string packageName, string isUpdate)
    {
      try
      {
        HTTPUtils.SendRequestToClient("appInstalled", new Dictionary<string, string>()
        {
          {
            "package",
            packageName
          },
          {
            nameof (isUpdate),
            isUpdate
          }
        }, vmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in AppInstalled: " + ex.ToString());
      }
    }

    private static void AddToJson(string vmName)
    {
      Logger.Info("InstallApk: Adding app to json: " + ApkInstall.sAppName);
      AppInfo[] json = new AppInfo[ApkInstall.sOriginalJson.Length + 1];
      int num = 1;
      string sAppName = ApkInstall.sAppName;
      int index;
      for (index = 0; index < ApkInstall.sOriginalJson.Length; ++index)
      {
        if (ApkInstall.sOriginalJson[index].Name == ApkInstall.sAppName)
        {
          if (ApkInstall.sOriginalJson[index].Package == ApkInstall.sPackageName && ApkInstall.sOriginalJson[index].Activity == ApkInstall.sLaunchableActivityName)
          {
            ApkInstall.sAppName = ApkInstall.sOriginalJson[index].Name;
            return;
          }
          ApkInstall.sAppName = sAppName + "-" + num.ToString();
          ++num;
          index = 0;
        }
        json[index] = ApkInstall.sOriginalJson[index];
      }
      json[index] = new AppInfo(ApkInstall.sAppName, ApkInstall.sAppIcon, ApkInstall.sPackageName, ApkInstall.sLaunchableActivityName, "0", "no", ApkInstall.sVersion, ApkInstall.sGl3Required, ApkInstall.sVideoPresent, ApkInstall.sVersionName, ApkInstall.sIsGamepadCompatible);
      new JsonParser(vmName).WriteJson(json);
    }
  }
}
