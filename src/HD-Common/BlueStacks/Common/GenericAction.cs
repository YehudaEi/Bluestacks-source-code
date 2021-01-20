// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.GenericAction
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

namespace BlueStacks.Common
{
  public enum GenericAction
  {
    InstallPlay = 1,
    InstallCDN = 2,
    ApplicationBrowser = 4,
    UserBrowser = 8,
    AppCenter = 16, // 0x00000010
    HomeAppTab = 32, // 0x00000020
    SettingsMenu = 64, // 0x00000040
    KeyBasedPopup = 128, // 0x00000080
    OpenSystemApp = 256, // 0x00000100
    PopupBrowser = 512, // 0x00000200
    QuickLaunch = 1024, // 0x00000400
    InstallPlayPopup = 2048, // 0x00000800
    OpenGuestUrl = 4096, // 0x00001000
    CreateInstanceSameEngine = 8192, // 0x00002000
    None = 65536, // 0x00010000
  }
}
