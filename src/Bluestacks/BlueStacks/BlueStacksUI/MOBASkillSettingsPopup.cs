// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.MOBASkillSettingsPopup
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Shapes;

namespace BlueStacks.BlueStacksUI
{
  public class MOBASkillSettingsPopup : CustomPopUp, IComponentConnector
  {
    private CanvasElement mCanvasElement;
    internal MOBASkillSettingsPopup mMOBASkillSettingsPopup;
    internal StackPanel popupPanel;
    internal Border mBorder;
    internal Border mMaskBorder3;
    internal Grid mHeaderGrid;
    internal CustomPictureBox mHelpIcon;
    internal CustomRadioButton mQuickSkill;
    internal CustomRadioButton mAutoSkill;
    internal CustomRadioButton mManualSkill;
    internal Grid mOtherSettingsGrid;
    internal CustomPictureBox mOtherSettingsHelpIcon;
    internal CustomCheckbox mStopMovementCheckbox;
    internal Grid mMoreSettingsGrid;
    internal Path DownArrow;
    private bool _contentLoaded;

    public MOBASkillSettingsPopup(CanvasElement canvasElement)
    {
      this.mCanvasElement = canvasElement;
      this.InitializeComponent();
      this.PlacementTarget = (UIElement) this.mCanvasElement?.mSkillImage;
    }

    private void MobaSkillsRadioButton_PreviewMouseLeftButtonDown(
      object sender,
      MouseButtonEventArgs e)
    {
      this.mCanvasElement.MOBASkillSettingsMoreInfoPopup.IsOpen = false;
      this.mCanvasElement.MOBAOtherSettingsMoreInfoPopup.IsOpen = false;
      CustomRadioButton customRadioButton = sender as CustomRadioButton;
      customRadioButton.IsChecked = new bool?(true);
      KeymapCanvasWindow.sIsDirty = true;
      string skillName;
      switch (customRadioButton.Name)
      {
        case "mManualSkill":
          this.mCanvasElement.AssignMobaSkill(false, false);
          skillName = "ManualSkill";
          break;
        case "mAutoSkill":
          this.mCanvasElement.AssignMobaSkill(true, false);
          skillName = "AutoSkill";
          break;
        case "mQuickSkill":
          this.mCanvasElement.AssignMobaSkill(true, true);
          skillName = "QuickSkill";
          break;
        default:
          this.mCanvasElement.AssignMobaSkill(true, true);
          skillName = "QuickSkill";
          break;
      }
      this.mCanvasElement.SendMOBAStats("moba_skill_changed", skillName);
    }

    private void MOBASkillSettingsPopup_Opened(object sender, EventArgs e)
    {
      this.mCanvasElement.mSkillImage.IsEnabled = false;
    }

    private void MOBASkillSettingsPopup_Closed(object sender, EventArgs e)
    {
      this.mCanvasElement.mSkillImage.IsEnabled = true;
      this.mCanvasElement.MOBAOtherSettingsMoreInfoPopup.IsOpen = false;
      this.mCanvasElement.MOBASkillSettingsMoreInfoPopup.IsOpen = false;
    }

    private void HelpIcon_MouseEnter(object sender, MouseEventArgs e)
    {
      this.mCanvasElement.MOBAOtherSettingsMoreInfoPopup.IsOpen = false;
      this.mCanvasElement.MOBASkillSettingsMoreInfoPopup.IsOpen = true;
      this.mCanvasElement.MOBASkillSettingsMoreInfoPopup.StaysOpen = true;
    }

    private void OtherSettingsHelpIcon_MouseEnter(object sender, MouseEventArgs e)
    {
      this.mCanvasElement.MOBASkillSettingsMoreInfoPopup.IsOpen = false;
      this.mCanvasElement.MOBAOtherSettingsMoreInfoPopup.IsOpen = true;
      this.mCanvasElement.MOBAOtherSettingsMoreInfoPopup.StaysOpen = true;
    }

    private void StopMovementCheckbox_Checked(object sender, RoutedEventArgs e)
    {
      this.SetStopMobaDpadValue(true);
      this.mCanvasElement.SendMOBAStats("stop_moba_dpad_checked", "");
    }

    private void SetStopMobaDpadValue(bool isChecked)
    {
      this.mCanvasElement.MOBASkillSettingsMoreInfoPopup.IsOpen = false;
      this.mCanvasElement.MOBAOtherSettingsMoreInfoPopup.IsOpen = false;
      KMManager.CheckAndCreateNewScheme();
      this.mStopMovementCheckbox.IsChecked = new bool?(isChecked);
      if (this.mCanvasElement.ListActionItem.First<IMAction>().Type == KeyActionType.MOBASkill)
        ((MOBASkill) this.mCanvasElement.ListActionItem.First<IMAction>()).StopMOBADpad = isChecked;
      KeymapCanvasWindow.sIsDirty = true;
    }

    private void StopMovementCheckbox_Unchecked(object sender, RoutedEventArgs e)
    {
      this.SetStopMobaDpadValue(false);
      this.mCanvasElement.SendMOBAStats("stop_moba_dpad_unchecked", "");
    }

    private void MoreSettingsGrid_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.mCanvasElement.MOBASkillSettingsMoreInfoPopup.IsOpen = false;
      this.mCanvasElement.MOBAOtherSettingsMoreInfoPopup.IsOpen = false;
      this.mCanvasElement.MOBASkillSettingsPopup.IsOpen = false;
      this.mCanvasElement.OpenPopup();
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/keymap/uielement/mobaskillsettingspopup.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    internal Delegate _CreateDelegate(Type delegateType, string handler)
    {
      return Delegate.CreateDelegate(delegateType, (object) this, handler);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.mMOBASkillSettingsPopup = (MOBASkillSettingsPopup) target;
          break;
        case 2:
          this.popupPanel = (StackPanel) target;
          break;
        case 3:
          this.mBorder = (Border) target;
          break;
        case 4:
          this.mMaskBorder3 = (Border) target;
          break;
        case 5:
          this.mHeaderGrid = (Grid) target;
          break;
        case 6:
          this.mHelpIcon = (CustomPictureBox) target;
          this.mHelpIcon.MouseEnter += new MouseEventHandler(this.HelpIcon_MouseEnter);
          break;
        case 7:
          this.mQuickSkill = (CustomRadioButton) target;
          this.mQuickSkill.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(this.MobaSkillsRadioButton_PreviewMouseLeftButtonDown);
          break;
        case 8:
          this.mAutoSkill = (CustomRadioButton) target;
          this.mAutoSkill.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(this.MobaSkillsRadioButton_PreviewMouseLeftButtonDown);
          break;
        case 9:
          this.mManualSkill = (CustomRadioButton) target;
          this.mManualSkill.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(this.MobaSkillsRadioButton_PreviewMouseLeftButtonDown);
          break;
        case 10:
          this.mOtherSettingsGrid = (Grid) target;
          break;
        case 11:
          this.mOtherSettingsHelpIcon = (CustomPictureBox) target;
          this.mOtherSettingsHelpIcon.MouseEnter += new MouseEventHandler(this.OtherSettingsHelpIcon_MouseEnter);
          break;
        case 12:
          this.mStopMovementCheckbox = (CustomCheckbox) target;
          this.mStopMovementCheckbox.Checked += new RoutedEventHandler(this.StopMovementCheckbox_Checked);
          this.mStopMovementCheckbox.Unchecked += new RoutedEventHandler(this.StopMovementCheckbox_Unchecked);
          break;
        case 13:
          this.mMoreSettingsGrid = (Grid) target;
          this.mMoreSettingsGrid.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.MoreSettingsGrid_PreviewMouseLeftButtonUp);
          break;
        case 14:
          this.DownArrow = (Path) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
