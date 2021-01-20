// Decompiled with JetBrains decompiler
// Type: BlueStacks.Uninstaller.UninstallerProperties
// Assembly: BlueStacksUninstaller, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: DBF002A0-6BF3-43CC-B5E7-0E90D1C19949
// Assembly location: C:\Program Files\BlueStacks\BlueStacksUninstaller.exe

using System.Collections.Generic;

namespace BlueStacks.Uninstaller
{
  public class UninstallerProperties
  {
    private static string sGUID = "";
    private static string sCampaignName = "";
    private static string sCampaignMD5 = "";
    private static bool mIsRunningInSilentMode = false;
    private static string sCloudHost = "https://cloud.bluestacks.com";
    private static string sLogFileName = string.Empty;
    private static string sCurrentLocale = "en-US";
    private static string sUninstallId = string.Empty;
    public static List<string> VmDisplayNameList = new List<string>();

    public static string GUID
    {
      get
      {
        return UninstallerProperties.sGUID;
      }
      set
      {
        UninstallerProperties.sGUID = value;
      }
    }

    public static string CampaignName
    {
      get
      {
        return UninstallerProperties.sCampaignName;
      }
      set
      {
        UninstallerProperties.sCampaignName = value;
      }
    }

    public static string CampaignMD5
    {
      get
      {
        return UninstallerProperties.sCampaignMD5;
      }
      set
      {
        UninstallerProperties.sCampaignMD5 = value;
      }
    }

    public static bool IsRunningInSilentMode
    {
      get
      {
        return UninstallerProperties.mIsRunningInSilentMode;
      }
      set
      {
        UninstallerProperties.mIsRunningInSilentMode = value;
      }
    }

    public static string CloudHost
    {
      get
      {
        return UninstallerProperties.sCloudHost;
      }
      set
      {
        UninstallerProperties.sCloudHost = value;
      }
    }

    public static string LogFilePath
    {
      get
      {
        return UninstallerProperties.sLogFileName;
      }
      set
      {
        UninstallerProperties.sLogFileName = value;
      }
    }

    public static string CurrentLocale
    {
      get
      {
        return UninstallerProperties.sCurrentLocale;
      }
      set
      {
        UninstallerProperties.sCurrentLocale = value;
      }
    }

    public static string UninstallId
    {
      get
      {
        return UninstallerProperties.sUninstallId;
      }
      set
      {
        UninstallerProperties.sUninstallId = value;
      }
    }
  }
}
