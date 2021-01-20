// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.EngineSettingViewModel
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System;
using System.Globalization;
using System.Windows;

namespace BlueStacks.BlueStacksUI
{
  public class EngineSettingViewModel : EngineSettingBaseViewModel
  {
    private string _VmName;
    private MainWindow ParentWindow;

    public EngineSettingViewModel(
      MainWindow owner,
      string vmName,
      EngineSettingBase engineSettingBase,
      bool isEcoModeEnabled)
      : base((Window) owner, vmName, engineSettingBase, false, "", isEcoModeEnabled)
    {
      this.ParentWindow = owner;
      this._VmName = vmName;
    }

    protected override void Save(object param)
    {
      if (this.Status == Status.Progress)
        Logger.Info("Compatibility check is running");
      else if (this.IsRestartRequired())
      {
        if (Oem.IsOEMDmm)
        {
          this.RestartInstanceHandler();
          this.ParentWindow.Close();
        }
        else
        {
          CustomMessageWindow customMessageWindow = new CustomMessageWindow();
          customMessageWindow.Owner = this.Owner;
          customMessageWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
          customMessageWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_RESTART_BLUESTACKS", "");
          customMessageWindow.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_RESTART_BLUESTACKS_MESSAGE", "");
          customMessageWindow.AddButton(ButtonColors.Blue, "STRING_RESTART_NOW", (EventHandler) ((o, e) =>
          {
            this.RestartInstanceHandler();
            BlueStacksUIUtils.RestartInstance(this._VmName, false);
          }), (string) null, false, (object) null, true);
          customMessageWindow.AddButton(ButtonColors.White, "STRING_DISCARD_CHANGES", (EventHandler) ((o, e) => this.Init()), (string) null, false, (object) null, true);
          customMessageWindow.ShowDialog();
        }
      }
      else
      {
        this.SaveEngineSettings("");
        this.AddToastPopupUserControl(LocaleStrings.GetLocalizedString("STRING_CHANGES_SAVED", ""));
      }
    }

    private void RestartInstanceHandler()
    {
      string abiResult = "";
      if (this.EngineData.ABISetting != this.ABISetting)
        abiResult = VmCmdHandler.RunCommand(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} {1}", (object) "switchAbi", (object) this.ABISetting.GetDescription()), this._VmName, "bgp");
      this.SaveEngineSettings(abiResult);
      BlueStacksUIUtils.CloseContainerWindow((FrameworkElement) this.ParentView);
    }
  }
}
