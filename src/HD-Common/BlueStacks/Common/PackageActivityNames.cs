// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.PackageActivityNames
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System.Collections.Generic;

namespace BlueStacks.Common
{
  public static class PackageActivityNames
  {
    public const string DefaultBundledLauncher = "com.bluestacks.appmart";
    public const string DefaultBundledLauncherMainActivity = "com.bluestacks.appmart.StartTopAppsActivity";

    public static List<string> SystemApps { get; } = new List<string>()
    {
      "com.android.vending",
      "com.android.camera2",
      "com.android.chrome",
      "com.bluestacks.settings",
      "com.bluestacks.filemanager"
    };

    public static class Google
    {
      public const string GooglePlayStore = "com.android.vending";
      public const string GooglePlayServices = "com.google.android.gms";
      public const string Chrome = "com.android.chrome";
    }

    public static class BlueStacks
    {
      public const string AppMart = "com.bluestacks.appmart";
      public const string AppMartMainActivity = "com.bluestacks.appmart.StartTopAppsActivity";
      public const string GamePopHome = "com.bluestacks.gamepophome";
      public const string FileManager = "com.bluestacks.filemanager";
      public const string Settings = "com.bluestacks.settings";
      public const string ProvisionPackage = "com.android.provision";
      public const string AccountSigninPackage = "com.uncube.account";
    }

    public static class ThirdParty
    {
      public static readonly List<string> AllPUBGPackageNames = new List<string>()
      {
        "com.tencent.ig",
        "com.rekoo.pubgm",
        "com.vng.pubgmobile",
        "com.pubg.krmobile",
        "com.tencent.tmgp.pubgmhd"
      };
      public static readonly List<string> AllCallOfDutyPackageNames = new List<string>()
      {
        "com.tencent.tmgp.kr.codm",
        "com.garena.game.codm",
        "com.activision.callofduty.shooter",
        "com.vng.codmvn"
      };
      public static readonly List<string> AllOneStorePackageNames = new List<string>()
      {
        "com.skt.skaf.A000Z00040",
        "com.skt.skaf.OA00018282"
      };
      public const string Camera = "com.android.camera2";
      public const string PUBG_International = "com.tencent.ig";
      public const string FreeFire = "com.dts.freefireth";
      public const string BrawlStars = "com.supercell.brawlstars";
      public const string GalaxyStrore = "com.sec.android.app.samsungapps";
      public const string Warface = "com.my.warface.online.fps.pvp.action.shooter";
      public const string RiseOfKingdoms = "com.lilithgame.roc.gp";
      public const string ROKImagePickerActivity = "com.lilithgame.roc.gp/sh.lilith.lilithchat.activities.ImagePickerActivity";
      public const string AndroidGalleryActivity = "com.android.gallery3d /.app.GalleryActivity";
      public const string CODAppName = "Call of Duty: Mobile";
      public const string PUBGAppName = "PUBG Mobile";
      public const string FreeFireAppName = "Garena Free Fire";
    }
  }
}
