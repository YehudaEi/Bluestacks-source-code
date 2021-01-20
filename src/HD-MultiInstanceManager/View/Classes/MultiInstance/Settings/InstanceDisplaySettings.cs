// Decompiled with JetBrains decompiler
// Type: MultiInstanceManagerMVVM.View.Classes.MultiInstance.Settings.InstanceDisplaySettings
// Assembly: HD-MultiInstanceManager, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 0C4C54E8-53C7-4A46-AE96-CC8E89A6B884
// Assembly location: C:\Program Files\BlueStacks\HD-MultiInstanceManager.exe

using BlueStacks.Common;
using System.Windows;

namespace MultiInstanceManagerMVVM.View.Classes.MultiInstance.Settings
{
  public class InstanceDisplaySettings : DisplaySettingsBase
  {
    public Window Window { get; private set; }

    public InstanceDisplaySettings(Window window, string vmName, string oem)
      : base(window, vmName, oem)
    {
      this.Window = window;
      this.IsOpenedFromMultiInstance = true;
    }

    protected override void Save(object param)
    {
      if (!this.IsDirty())
        return;
      this.SaveDisplaySetting();
      this.AddToastPopup(LocaleStrings.GetLocalizedString("STRING_CHANGES_SAVED", ""));
    }
  }
}
