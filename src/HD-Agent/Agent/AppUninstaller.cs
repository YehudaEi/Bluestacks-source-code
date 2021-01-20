// Decompiled with JetBrains decompiler
// Type: BlueStacks.Agent.AppUninstaller
// Assembly: HD-Agent, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 06DAED18-1D79-40C2-83F8-3A28B5222574
// Assembly location: C:\Program Files\BlueStacks\HD-Agent.exe

using BlueStacks.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;

namespace BlueStacks.Agent
{
  public class AppUninstaller
  {
    public static int s_systemApps;
    public static AppInfo[] s_originalJson;

    public static void AppUninstalled(
      string packageName,
      string vmName,
      string source,
      string campaignName,
      string clientVersion)
    {
      string appName = AppUninstaller.RemoveFromJson(packageName, vmName);
      try
      {
        if (Oem.Instance.IsCreateDesktopIconForApp)
        {
          if (string.Compare(vmName, "Android", true) == 0)
          {
            foreach (string file in Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "*.lnk", SearchOption.AllDirectories))
            {
              try
              {
                if (Utils.IsShortcutArgumentContainsPackage(file, packageName))
                  File.Delete(file);
              }
              catch (Exception ex)
              {
                Logger.Warning("Error while checking file for deleting shortcut, filename: " + file);
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Warning("Error while deleting shortcut icons for apps from desktop. Error: " + ex.ToString());
      }
      if (Oem.Instance.IsOEMWithBGPClient)
      {
        try
        {
          HTTPUtils.SendRequestToClient("appUninstalled", new Dictionary<string, string>()
          {
            {
              "package",
              packageName
            },
            {
              "name",
              appName
            }
          }, vmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
        }
        catch (Exception ex)
        {
          Logger.Error("Exception in AppUninstalled: " + ex.ToString());
        }
      }
      AppUninstaller.DeleteGlAppDataIfExist(packageName);
      Logger.Info("Sending App Uninstall stats");
      string versionFromPackage = HDAgent.GetVersionFromPackage(packageName, vmName);
      string versionNameFromPackage = HDAgent.GetVersionNameFromPackage(packageName, vmName);
      Stats.SendAppInstallStats(appName, packageName, versionFromPackage, versionNameFromPackage, "false", "false", source, vmName, campaignName, clientVersion, "");
      if (appName == "")
        appName = packageName;
      string message = string.Format("{0} {1}", (object) appName, (object) LocaleStrings.GetLocalizedString("STRING_UNINSTALL_SUCCESS", ""));
      if (!Features.IsFeatureEnabled(4UL))
        return;
      SysTray.ShowInfoShort(LocaleStrings.GetLocalizedString("STRING_BLUESTACKS", ""), message, vmName, packageName, "0");
    }

    public static int SilentUninstallApp(
      string package,
      bool nolookup,
      string vmName,
      out string reason)
    {
      reason = "";
      JsonParser jsonParser = new JsonParser(vmName);
      AppUninstaller.s_originalJson = jsonParser.GetAppList();
      Logger.Info("nolookup: " + nolookup.ToString());
      string str;
      if (!nolookup)
      {
        string imageName;
        jsonParser.GetAppInfoFromPackageName(package, out string _, out imageName, out string _, out str);
        Logger.Info("AppUninstaller: Got image name: " + imageName);
        if (imageName == null)
        {
          Logger.Info("AppUninstaller: App not found");
          return -1;
        }
      }
      if (!Utils.IsAppInstalled(package, vmName, out str))
      {
        Logger.Info("App not installed, removing entry from apps.json if there");
        AppUninstaller.RemoveFromJson(package, vmName);
        reason = "App Not Installed";
        return 1;
      }
      int num = AppUninstaller.UninstallApp(package, vmName);
      if (num == 0)
      {
        Logger.Info("AppUninstaller: Uninstallation successful");
        return num;
      }
      Logger.Warning("AppUninstaller: Uninstallation failed");
      return num;
    }

    private static void DeleteGlAppDataIfExist(string packageName)
    {
      try
      {
        string path = Path.Combine(Path.Combine(RegistryManager.Instance.UserDefinedDir, "UCT"), packageName);
        if (!Directory.Exists(path))
          return;
        Directory.Delete(path, true);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in deleting gl texture data for package name: {0} and ex: {1}", (object) packageName, (object) ex);
      }
    }

    public static int UninstallApp(string packageName, string vmName)
    {
      try
      {
        Logger.Info("AppUninstaller: In uninstall app");
        Dictionary<string, string> data = new Dictionary<string, string>()
        {
          {
            "pkg",
            packageName
          }
        };
        string guest;
        try
        {
          guest = HTTPUtils.SendRequestToGuest("uninstall", data, vmName, 0, (Dictionary<string, string>) null, true, 30, 2000, "bgp");
        }
        catch (Exception ex)
        {
          Logger.Error("Exception when sending uninstall post request, ex: {0}", (object) ex.Message);
          return 1;
        }
        return (string) JObject.Parse(guest)["result"] == "ok" ? 0 : 1;
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in UninstallApp :" + ex.ToString());
        return 1;
      }
    }

    public static string RemoveFromJson(string packageName, string vmName)
    {
      Logger.Info("AppUninstaller: Removing app from json: " + packageName);
      JsonParser jsonParser = new JsonParser(vmName);
      AppUninstaller.s_originalJson = jsonParser.GetAppList();
      int num = 0;
      string str = "";
      for (int index = 0; index < AppUninstaller.s_originalJson.Length; ++index)
      {
        if (AppUninstaller.s_originalJson[index].Package == packageName)
        {
          str = AppUninstaller.s_originalJson[index].Name;
          ++num;
        }
      }
      AppInfo[] json = new AppInfo[AppUninstaller.s_originalJson.Length - num];
      int index1 = 0;
      int index2 = 0;
      for (; index1 < AppUninstaller.s_originalJson.Length; ++index1)
      {
        if (AppUninstaller.s_originalJson[index1].Package == packageName)
        {
          AppUninstaller.RemoveFromLibrary(AppUninstaller.s_originalJson[index1].Name, AppUninstaller.s_originalJson[index1].Package, AppUninstaller.s_originalJson[index1].Img);
          AppUninstaller.RemoveAppTile(AppUninstaller.s_originalJson[index1].Package);
        }
        else
        {
          json[index2] = AppUninstaller.s_originalJson[index1];
          ++index2;
        }
      }
      jsonParser.WriteJson(json);
      return str;
    }

    private static void RemoveIcon(string imageFile)
    {
      try
      {
        Logger.Info("AppUninstaller: Removing icon " + imageFile);
        string path = Path.Combine(RegistryStrings.GadgetDir, imageFile);
        if (!File.Exists(path))
          return;
        File.Delete(path);
      }
      catch
      {
        Logger.Warning("AppUninstaller: Error Removing icon ");
      }
    }

    private static void RemoveFromLibrary(string appName, string packageName, string img)
    {
      try
      {
        Logger.Info("Removing {0} from library", (object) appName);
        string path1 = Path.Combine(RegistryStrings.LibraryDir, "Icons");
        string path2 = Path.Combine(RegistryStrings.LibraryDir, "App Stores");
        string str1 = appName + ".lnk";
        string str2 = img.Substring(img.LastIndexOf("."));
        string str3 = img.Substring(0, img.Length - str2.Length) + ".ico";
        try
        {
          foreach (string file in Directory.GetFiles(path2))
          {
            if (Path.GetFileName(file) == str1)
            {
              Logger.Info("Deleting {0}", (object) file);
              File.Delete(file);
            }
          }
        }
        catch (Exception ex)
        {
          Logger.Error("Exception when deleting from {0}", (object) path2);
          Logger.Error(ex.Message);
        }
        foreach (string file in Directory.GetFiles(path1))
        {
          if (Path.GetFileName(file) == str3)
          {
            Logger.Info("Deleting {0}", (object) file);
            File.Delete(file);
          }
        }
      }
      catch
      {
        Logger.Warning("Error Removing {0} from library", (object) appName);
      }
    }

    private static void RemoveAppTile(string packageName)
    {
      try
      {
        string path = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Microsoft\\Windows\\Application Shortcuts\\BlueStacks\\"), packageName + ".lnk");
        if (!File.Exists(path))
          return;
        Logger.Info("AppUninstaller: Removing app tile " + path);
        File.Delete(path);
      }
      catch
      {
        Logger.Warning("Error RemoveAppTile");
      }
    }
  }
}
