// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.RegistryStrings
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.IO;

namespace BlueStacks.Common
{
  public static class RegistryStrings
  {
    private static string sDataDir;
    private static string sInstallDir;
    private static string sUserDefinedDir;
    private static string sProductIconCompletePath;
    private static string sProductImageCompletePath;
    private static string sBstUserDataDir;
    private static string sBstLogsDir;
    private static string sGadgetDir;
    private static string sInputMapperFolder;
    private static string sSharedFolderDir;
    private static string sLibraryDir;
    private static string sPromotionDirectory;
    private static string sOperationsScriptFolder;
    private static string sBstManagerDir;
    private static string sRegistryBaseKeyPath;
    private static string sUserAgent;
    public static string BtvDir;
    public static string ObsDir;
    public static string ObsBinaryPath;
    public static readonly string ScreenshotDefaultPath;

    public static string DataDir
    {
      get
      {
        if (RegistryStrings.sDataDir == null)
          RegistryStrings.sDataDir = (string) RegistryUtils.GetRegistryValue(Strings.RegistryBaseKeyPath, nameof (DataDir), (object) "", RegistryKeyKind.HKEY_LOCAL_MACHINE);
        return RegistryStrings.sDataDir;
      }
    }

    public static string InstallDir
    {
      get
      {
        if (RegistryStrings.sInstallDir == null)
          RegistryStrings.sInstallDir = (string) RegistryUtils.GetRegistryValue(Strings.RegistryBaseKeyPath, nameof (InstallDir), (object) "", RegistryKeyKind.HKEY_LOCAL_MACHINE);
        return RegistryStrings.sInstallDir;
      }
    }

    public static string UserDefinedDir
    {
      get
      {
        if (RegistryStrings.sUserDefinedDir == null)
          RegistryStrings.sUserDefinedDir = (string) RegistryUtils.GetRegistryValue(Strings.RegistryBaseKeyPath, nameof (UserDefinedDir), (object) "", RegistryKeyKind.HKEY_LOCAL_MACHINE);
        return RegistryStrings.sUserDefinedDir;
      }
    }

    public static string GetBstAndroidDir(string vmName)
    {
      return Path.Combine(RegistryStrings.DataDir, vmName);
    }

    public static string ProductIconCompletePath
    {
      get
      {
        if (RegistryStrings.sProductIconCompletePath == null)
          RegistryStrings.sProductIconCompletePath = Path.Combine(RegistryStrings.InstallDir, "ProductLogo.ico");
        return RegistryStrings.sProductIconCompletePath;
      }
    }

    public static string ProductImageCompletePath
    {
      get
      {
        if (RegistryStrings.sProductImageCompletePath == null)
          RegistryStrings.sProductImageCompletePath = Path.Combine(RegistryStrings.InstallDir, "ProductLogo.png");
        return RegistryStrings.sProductImageCompletePath;
      }
    }

    public static string BstUserDataDir
    {
      get
      {
        if (RegistryStrings.sBstUserDataDir == null)
          RegistryStrings.sBstUserDataDir = Path.Combine(RegistryStrings.DataDir, "UserData");
        return RegistryStrings.sBstUserDataDir;
      }
    }

    public static string BstLogsDir
    {
      get
      {
        if (RegistryStrings.sBstLogsDir == null)
          RegistryStrings.sBstLogsDir = Path.Combine(RegistryManager.Instance.UserDefinedDir, "Logs");
        return RegistryStrings.sBstLogsDir;
      }
    }

    public static string GadgetDir
    {
      get
      {
        if (RegistryStrings.sGadgetDir == null)
          RegistryStrings.sGadgetDir = Path.Combine(RegistryStrings.BstUserDataDir, "Gadget");
        return RegistryStrings.sGadgetDir;
      }
    }

    public static string InputMapperFolder
    {
      get
      {
        if (RegistryStrings.sInputMapperFolder == null)
          RegistryStrings.sInputMapperFolder = Path.Combine(RegistryStrings.BstUserDataDir, "InputMapper");
        return RegistryStrings.sInputMapperFolder;
      }
    }

    public static string SharedFolderDir
    {
      get
      {
        if (RegistryStrings.sSharedFolderDir == null)
          RegistryStrings.sSharedFolderDir = Path.Combine(RegistryStrings.BstUserDataDir, "SharedFolder");
        return RegistryStrings.sSharedFolderDir;
      }
    }

    public static string LibraryDir
    {
      get
      {
        if (RegistryStrings.sLibraryDir == null)
          RegistryStrings.sLibraryDir = Path.Combine(RegistryStrings.BstUserDataDir, "Library");
        return RegistryStrings.sLibraryDir;
      }
    }

    public static string PromotionDirectory
    {
      get
      {
        if (RegistryStrings.sPromotionDirectory == null)
          RegistryStrings.sPromotionDirectory = Path.Combine(RegistryManager.Instance.ClientInstallDir, "Promo");
        return RegistryStrings.sPromotionDirectory;
      }
    }

    public static string MacroRecordingsFolderPath
    {
      get
      {
        if (RegistryStrings.sOperationsScriptFolder == null)
          RegistryStrings.sOperationsScriptFolder = Path.Combine(RegistryStrings.InputMapperFolder, "UserScripts");
        return RegistryStrings.sOperationsScriptFolder;
      }
    }

    public static string BstManagerDir
    {
      get
      {
        if (RegistryStrings.sBstManagerDir == null)
          RegistryStrings.sBstManagerDir = Path.Combine(RegistryStrings.DataDir, "Manager");
        return RegistryStrings.sBstManagerDir;
      }
    }

    public static string RegistryBaseKeyPath
    {
      get
      {
        if (RegistryStrings.sRegistryBaseKeyPath == null)
          RegistryStrings.sRegistryBaseKeyPath = Strings.RegistryBaseKeyPath + RegistryManager.UPGRADE_TAG;
        return RegistryStrings.sRegistryBaseKeyPath;
      }
    }

    public static string UserAgent
    {
      get
      {
        if (RegistryStrings.sUserAgent == null)
          RegistryStrings.sUserAgent = Utils.GetUserAgent("bgp");
        return RegistryStrings.sUserAgent;
      }
    }

    public static string CursorPath
    {
      get
      {
        return Path.Combine(CustomPictureBox.AssetsDir, "Mouse_cursor.cur");
      }
    }

    static RegistryStrings()
    {
      try
      {
        RegistryStrings.BtvDir = Path.Combine(RegistryManager.Instance.UserDefinedDir, "BTV");
        RegistryStrings.ObsDir = Path.Combine(RegistryStrings.BtvDir, "OBS");
        RegistryStrings.ObsBinaryPath = Path.Combine(RegistryStrings.ObsDir, "HD-OBS.exe");
        RegistryStrings.ScreenshotDefaultPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), Strings.ProductTopBarDisplayName);
      }
      catch (Exception ex)
      {
        Logger.Error("Error in RegistryStrings " + ex.Message);
      }
    }
  }
}
