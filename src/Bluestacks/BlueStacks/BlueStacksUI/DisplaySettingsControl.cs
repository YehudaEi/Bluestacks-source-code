// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.DisplaySettingsControl
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System;
using System.Windows;

namespace BlueStacks.BlueStacksUI
{
  public class DisplaySettingsControl : DisplaySettingsBase
  {
    public MainWindow ParentWindow { get; private set; }

    public DisplaySettingsControl(MainWindow window)
      : base((Window) window, window?.mVmName, "")
    {
      this.ParentWindow = window;
    }

    protected override void Save(object param)
    {
      if (Oem.IsOEMDmm)
      {
        this.SaveDisplaySetting();
        BlueStacksUIUtils.CloseContainerWindow((FrameworkElement) this);
        this.ParentWindow.Close();
      }
      else
      {
        if (!this.IsDirty())
          return;
        CustomMessageWindow customMessageWindow = new CustomMessageWindow();
        customMessageWindow.Owner = (Window) this.ParentWindow;
        customMessageWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        customMessageWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_RESTART_BLUESTACKS", "");
        customMessageWindow.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_RESTART_BLUESTACKS_MESSAGE", "");
        customMessageWindow.AddButton(ButtonColors.Blue, "STRING_RESTART_NOW", (EventHandler) ((o, e) =>
        {
          this.SaveDisplaySetting();
          if (BlueStacksUIUtils.DictWindows.Count == 1)
          {
            App.defaultResolution = new Fraction((long) RegistryManager.Instance.Guest[Strings.CurrentDefaultVmName].GuestWidth, (long) RegistryManager.Instance.Guest[Strings.CurrentDefaultVmName].GuestHeight);
            PromotionManager.ReloadPromotionsAsync();
          }
          BlueStacksUIUtils.CloseContainerWindow((FrameworkElement) this);
          BlueStacksUIUtils.RestartInstance(this.VmName, false);
        }), (string) null, false, (object) null, true);
        customMessageWindow.AddButton(ButtonColors.White, "STRING_DISCARD_CHANGES", (EventHandler) ((o, e) => this.DiscardCurrentChangingModel()), (string) null, false, (object) null, true);
        customMessageWindow.ShowDialog();
      }
    }
  }
}
