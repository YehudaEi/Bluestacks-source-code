// Decompiled with JetBrains decompiler
// Type: BlueStacks.ViewModel.Classes.Settings.EngineSettingViewModel
// Assembly: HD-MultiInstanceManager, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 0C4C54E8-53C7-4A46-AE96-CC8E89A6B884
// Assembly location: C:\Program Files\BlueStacks\HD-MultiInstanceManager.exe

using BlueStacks.Common;
using System.Windows;

namespace BlueStacks.ViewModel.Classes.Settings
{
  public class EngineSettingViewModel : EngineSettingBaseViewModel
  {
    public EngineSettingViewModel(
      Window owner,
      string vmName,
      EngineSettingBase engineSettingBase,
      string oem,
      bool isEcoModeEnabled)
      : base(owner, vmName, engineSettingBase, true, oem, isEcoModeEnabled)
    {
    }

    protected override void Save(object param)
    {
      if (this.Status == Status.Progress)
      {
        Logger.Info("Compatibility check is running");
      }
      else
      {
        if (!this.IsDirty())
          return;
        this.SaveEngineSettings("");
        this.AddToastPopup(LocaleStrings.GetLocalizedString("STRING_CHANGES_SAVED", ""));
      }
    }
  }
}
