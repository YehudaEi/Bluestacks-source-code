// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.Constants
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System.IO;

namespace BlueStacks.Common
{
  public static class Constants
  {
    public static readonly System.Version win8version = new System.Version(6, 2, 9200, 0);
    public static readonly string LocaleStringsConstant = "STRING_";
    public static readonly string ImapLocaleStringsConstant = "IMAP_" + Constants.LocaleStringsConstant;
    public static readonly string[] ImapGamepadEvents = new string[26]
    {
      "GamepadDpadUp",
      "GamepadDpadDown",
      "GamepadDpadLeft",
      "GamepadDpadRight",
      "GamepadStart",
      "GamepadStop",
      "GamepadLeftThumb",
      "GamepadRightThumb",
      "GamepadLeftShoulder",
      "GamepadRightShoulder",
      "GamepadA",
      "GamepadB",
      "GamepadX",
      "GamepadY",
      "GamepadLStickUp",
      "GamepadLStickDown",
      "GamepadLStickLeft",
      "GamepadLStickRight",
      "GamepadRtickUp",
      "GamepadRStickDown",
      "GamepadRStickLeft",
      "GamepadRStickRight",
      "GamepadLTrigger",
      "GamepadRTrigger",
      "LeftStick",
      "RightStick"
    };
    public static readonly string[] ReservedFileNamesList = new string[23]
    {
      "con",
      "prn",
      "aux",
      "nul",
      "clock$",
      "com1",
      "com2",
      "com3",
      "com4",
      "com5",
      "com6",
      "com7",
      "com8",
      "com9",
      "lpt1",
      "lpt3",
      "lpt3",
      "lpt4",
      "lpt5",
      "lpt6",
      "lpt7",
      "lpt8",
      "lpt9"
    };
    public static readonly string[] ImapGameControlsHiddenInOverlayList = new string[5]
    {
      "Zoom",
      "Tilt",
      "Swipe",
      "State",
      "MouseZoom"
    };
    public static readonly string DefaultAppPlayerEngineInfo = "[{\"oem\":\"bgp64_hyperv\",\"prod_ver\":\"\",\"display_name\":\"Hyper-V\",\"download_url\":\"\",\"abi_value\":7,\"suffix\":\"\"},{\"oem\":\"bgp\",\"prod_ver\":\"\",\"display_name\":\"Nougat 32-bit\",\"download_url\":\"\",\"abi_value\":15,\"suffix\":\"\"},{\"oem\":\"bgp64\",\"prod_ver\":\"\",\"display_name\":\"Nougat 32-bit (Large virtual address)\",\"download_url\":\"\",\"abi_value\":7,\"suffix\":\"N-32 (Large virtual address)\"},{\"oem\":\"bgp64\",\"prod_ver\":\"\",\"display_name\":\"Nougat 64-bit\",\"download_url\":\"\",\"abi_value\":15,\"suffix\":\"N-64\"}]";
    public static readonly string[] All64BitOems = new string[4]
    {
      "bgp64",
      "bgp64_hyperv",
      "msi64_hyperv",
      "china_gmgr64"
    };
    public const string MacroPostFix = "_macro";
    public const string dateFormat = "yyyy-MM-dd HH:mm";
    public const int IDENTITY_OFFSET = 16;
    public const int GUEST_ABS_MAX_X = 32768;
    public const int GUEST_ABS_MAX_Y = 32768;
    public const int MaxAllowedCPUCores = 8;
    public const int TOUCH_POINTS_MAX = 16;
    public const int SWIPE_TOUCH_POINTS_MAX = 1;
    public const long LWIN_TIMEOUT_TICKS = 1000000;
    public const int CURSOR_HIDE_CLIP_LEN = 15;
    public const string ImapDependent = "Dependent";
    public const string ImapIndependent = "Independent";
    public const string ImapSubElement = "SubElement";
    public const string ImapParentElement = "ParentElement";
    public const string ImapNotCommon = "NotCommon";
    public const string ImapLinked = "Linked";
    public const string ImapCanvasElementY = "IMAP_CanvasElementX";
    public const string ImapCanvasElementX = "IMAP_CanvasElementY";
    public const string ImapCanvasElementRadius = "IMAP_CanvasElementRadius";
    public const string IMAPPopupUIElement = "IMAP_PopupUIElement";
    public const string IMAPKeypropertyPrefix = "Key";
    public const string IMAPUserDefined = "User-Defined";
    public const string ImapVideoHeaderConstant = "AAVideo";
    public const string ImapMiscHeaderConstant = "MISC";
    public const string ImapGlobalValid = "GlobalValidTag";
    public const string ImapDeveloperModeUIElemnt = "IMAP_DeveloperModeUIElemnt";
    public const string ImapGamepadStartKey = "GamepadStart";
    public const string ImapGamepadBackKey = "GamepadBack";
    public const string ImapGamepadLeftStickKey = "LeftStick";
    public const string ImapGamepadRightStickKey = "RightStick";
    public const string CustomCursorImageName = "yellow_cursor";
    public const string BrawlStarsCustomCursorImageName = "yellow_cursor_brawl";

    public static string MOBACursorPath
    {
      get
      {
        return Path.Combine(Path.Combine(RegistryManager.Instance.ClientInstallDir, RegistryManager.Instance.GetClientThemeNameFromRegistry()), "Mouse_cursor_MOBA.cur");
      }
    }

    public static string CustomCursorPath
    {
      get
      {
        return Path.Combine(Path.Combine(RegistryManager.Instance.ClientInstallDir, RegistryManager.Instance.GetClientThemeNameFromRegistry()), "Mouse_cursor.cur");
      }
    }

    public static string BrawlStarsMOBACursorPath
    {
      get
      {
        return Path.Combine(Path.Combine(RegistryManager.Instance.ClientInstallDir, "Assets"), "Mouse_cursor_MOBA_brawl.cur");
      }
    }

    public static string BrawlStarsCustomCursorPath
    {
      get
      {
        return Path.Combine(Path.Combine(RegistryManager.Instance.ClientInstallDir, "Assets"), "Mouse_cursor_brawl.cur");
      }
    }
  }
}
