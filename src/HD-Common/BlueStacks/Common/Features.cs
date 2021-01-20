// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.Features
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;

namespace BlueStacks.Common
{
  public static class Features
  {
    internal static string ConfigFeature = "net.";
    public const ulong BROADCAST_MESSAGES = 1;
    public const ulong INSTALL_NOTIFICATIONS = 2;
    public const ulong UNINSTALL_NOTIFICATIONS = 4;
    public const ulong CREATE_APP_SHORTCUTS = 8;
    public const ulong LAUNCH_SETUP_APP = 16;
    public const ulong SHOW_USAGE_STATS = 32;
    public const ulong SYS_TRAY_SUPPORT = 64;
    public const ulong SUGGESTED_APPS_SUPPORT = 128;
    public const ulong OTA_SUPPORT = 256;
    public const ulong SHOW_RESTART = 512;
    public const ulong ANDROID_NOTIFICATIONS = 1024;
    public const ulong RIGHT_ALIGN_PORTRAIT_MODE = 2048;
    public const ulong LAUNCH_FRONTEND_AFTER_INSTALLTION = 4096;
    public const ulong CREATE_LIBRARY = 8192;
    public const ulong SHOW_AGENT_ICON_IN_SYSTRAY = 16384;
    public const ulong IS_HOME_BUTTON_ENABLED = 32768;
    public const ulong IS_GRAPHICS_DRIVER_REMINDER_ENABLED = 65536;
    public const ulong EXIT_ON_HOME = 131072;
    public const ulong MULTI_INSTANCE_SUPPORT = 262144;
    public const ulong UPDATE_FRONTEND_APP_TITLE = 524288;
    public const ulong USE_DEFAULT_NETWORK_TEXT = 1048576;
    public const ulong IS_FULL_SCREEN_TOGGLE_ENABLED = 2097152;
    public const ulong SET_CHINA_LOCALE_AND_TIMEZONE = 4194304;
    public const ulong SHOW_TOGGLE_BUTTON_IN_LOADING_SCREEN = 8388608;
    public const ulong ENABLE_ALT_CTRL_I_SHORTCUTS = 16777216;
    public const ulong CREATE_LIBRARY_SHORTCUT_AT_DESKTOP = 33554432;
    public const ulong CREATE_START_LAUNCHER_SHORTCUT = 67108864;
    public const ulong WRITE_APP_CRASH_LOGS = 268435456;
    public const ulong CHINA_CLOUD = 536870912;
    public const ulong FORCE_DESKTOP_MODE = 1073741824;
    public const ulong NOT_TO_BE_USED = 2147483648;
    public const ulong ENABLE_ALT_CTRL_M_SHORTCUTS = 4294967296;
    public const ulong COLLECT_APK_HANDLER_LOGS = 8589934592;
    public const ulong SHOW_FRONTEND_FULL_SCREEN_TOAST = 17179869184;
    public const ulong IS_CHINA_UI = 34359738368;
    public const ulong NOT_TO_BE_USED_2 = 9223372036854775808;
    public const ulong ALL_FEATURES = 9223372034707292159;
    public const uint BST_HIDE_NAVIGATIONBAR = 1;
    public const uint BST_HIDE_STATUSBAR = 2;
    public const uint BST_HIDE_BACKBUTTON = 4;
    public const uint BST_HIDE_HOMEBUTTON = 8;
    public const uint BST_HIDE_RECENTSBUTTON = 16;
    public const uint BST_HIDE_SCREENSHOTBUTTON = 32;
    public const uint BST_HIDE_TOGGLEBUTTON = 64;
    public const uint BST_HIDE_CLOSEBUTTON = 128;
    public const uint BST_HIDE_GPS = 512;
    public const uint BST_SHOW_APKINSTALLBUTTON = 2048;
    public const uint BST_HIDE_HOMEAPPNEWLOADER = 65536;
    public const uint BST_SENDLETSGOS2PCLICKREPORT = 131072;
    public const uint BST_DISABLE_P2DM = 262144;
    public const uint BST_DISABLE_ARMTIPS = 524288;
    public const uint BST_DISABLE_S2P = 1048576;
    public const uint BST_SOGOUIME = 268435456;
    public const uint BST_BAIDUIME = 1073741824;
    public const uint BST_QQIME = 2147483648;
    public const uint BST_QEMU_3BT_COEXISTENCE_BIT = 536870912;
    public const uint BST_HIDE_S2P_SEARCH_BAIDU_IN_HOMEAPPNEW = 4194304;
    public const uint BST_NEW_TASK_ON_HOME = 2097152;
    public const uint BST_NO_REINSTALL = 67108864;
    public const int BST_HIDE_GUIDANCESCREEN = 1024;
    public const int BST_USE_CHINESE_CDN = 4096;
    public const int BST_ENALBE_ABOUT_PHONE_OPTION = 16777216;
    public const int BST_ENABLE_SECURITY_OPTION = 33554432;
    public const uint BST_SKIP_S2P_WHILE_LAUNCHING_APP = 2048;

    public static ulong GetEnabledFeatures()
    {
      return (ulong) Convert.ToUInt32(RegistryManager.Instance.FeaturesHigh) << 32 | (ulong) Convert.ToUInt32(RegistryManager.Instance.Features);
    }

    public static void SetEnabledFeatures(ulong feature)
    {
      uint featuresHigh;
      uint featuresLow;
      Features.GetHighLowFeatures(feature, out featuresHigh, out featuresLow);
      RegistryManager.Instance.Features = (int) featuresLow;
      RegistryManager.Instance.FeaturesHigh = (int) featuresHigh;
    }

    public static void GetHighLowFeatures(
      ulong features,
      out uint featuresHigh,
      out uint featuresLow)
    {
      featuresLow = (uint) (features & (ulong) uint.MaxValue);
      featuresHigh = (uint) (features >> 32);
    }

    public static bool IsFeatureEnabled(ulong featureMask)
    {
      ulong features = Features.GetEnabledFeatures();
      if (features == 0UL)
        features = Oem.Instance.WindowsOEMFeatures;
      return Features.IsFeatureEnabled(featureMask, features);
    }

    public static bool IsFeatureEnabled(ulong featureMask, ulong features)
    {
      return ((long) features & (long) featureMask) != 0L;
    }

    public static void DisableFeature(ulong featureMask)
    {
      ulong enabledFeatures = Features.GetEnabledFeatures();
      if (((long) enabledFeatures & (long) featureMask) == 0L)
        return;
      Features.SetEnabledFeatures(enabledFeatures & ~featureMask);
    }

    public static void EnableFeature(ulong featureMask)
    {
      ulong enabledFeatures = Features.GetEnabledFeatures();
      if (((long) enabledFeatures & (long) featureMask) != 0L)
        return;
      Features.SetEnabledFeatures(enabledFeatures | featureMask);
    }

    public static void EnableAllFeatures()
    {
      Features.SetEnabledFeatures(9223372034707292159UL);
    }

    public static void EnableFeaturesOfOem()
    {
      Features.SetEnabledFeatures(Oem.Instance.WindowsOEMFeatures);
    }

    public static bool IsFullScreenToggleEnabled()
    {
      return Features.IsFeatureEnabled(2097152UL);
    }

    public static bool IsHomeButtonEnabled()
    {
      return Features.IsFeatureEnabled(32768UL);
    }

    public static bool IsShareButtonEnabled()
    {
      return false;
    }

    public static bool IsGraphicsDriverReminderEnabled()
    {
      return Features.IsFeatureEnabled(65536UL);
    }

    public static bool IsSettingsButtonEnabled()
    {
      return false;
    }

    public static bool IsBackButtonEnabled()
    {
      return false;
    }

    public static bool IsMenuButtonEnabled()
    {
      return false;
    }

    public static bool ExitOnHome()
    {
      return Features.IsFeatureEnabled(131072UL);
    }

    public static bool UpdateFrontendAppTitle()
    {
      return Features.IsFeatureEnabled(524288UL);
    }

    public static bool UseDefaultNetworkText()
    {
      return Features.IsFeatureEnabled(1048576UL);
    }
  }
}
