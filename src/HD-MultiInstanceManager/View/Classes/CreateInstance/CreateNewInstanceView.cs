// Decompiled with JetBrains decompiler
// Type: MultiInstanceManagerMVVM.View.Classes.CreateInstance.CreateNewInstanceView
// Assembly: HD-MultiInstanceManager, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 0C4C54E8-53C7-4A46-AE96-CC8E89A6B884
// Assembly location: C:\Program Files\BlueStacks\HD-MultiInstanceManager.exe

using BlueStacks.Common;
using BlueStacks.Core;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace MultiInstanceManagerMVVM.View.Classes.CreateInstance
{
  public class CreateNewInstanceView : UiWindowBase, IComponentConnector
  {
    internal CustomTextBox CustomRam;
    internal CustomComboBox OrientationComboBox;
    internal CustomComboBox ResolutionComboBox;
    internal StackPanel CustomResolutionTextBoxes;
    internal CustomTextBox CustomResolutionWidth;
    internal CustomTextBox CustomResolutionHeight;
    internal StackPanel mAbi64AutoRadioButtonPanel;
    internal CustomRadioButton mAbi64AutoRadioButton;
    internal StackPanel mAbi64ARMRadioButtonPanel;
    internal CustomRadioButton mAbi64ARMRadioButton;
    internal CustomPopUp mDisabledAbiPopup;
    internal Border mDisabledAbiPopupBorder;
    private bool _contentLoaded;

    public CreateNewInstanceView()
    {
      this.InitializeComponent();
    }

    private void CustomRadioButtonAuto_MouseEnter(object sender, MouseEventArgs e)
    {
      if (this.mAbi64AutoRadioButton.IsEnabled || this.mAbi64AutoRadioButton.IsChecked.GetValueOrDefault())
        return;
      this.mDisabledAbiPopup.PlacementTarget = (UIElement) this.mAbi64AutoRadioButtonPanel;
      this.mDisabledAbiPopup.IsOpen = true;
    }

    private void CustomRadioButtonAuto_MouseLeave(object sender, MouseEventArgs e)
    {
      this.mDisabledAbiPopup.IsOpen = false;
    }

    private void CustomRadioButtonARM_MouseEnter(object sender, MouseEventArgs e)
    {
      if (this.mAbi64ARMRadioButton.IsEnabled || this.mAbi64ARMRadioButton.IsChecked.GetValueOrDefault())
        return;
      this.mDisabledAbiPopup.PlacementTarget = (UIElement) this.mAbi64ARMRadioButtonPanel;
      this.mDisabledAbiPopup.IsOpen = true;
    }

    private void CustomRadioButtonARM_MouseLeave(object sender, MouseEventArgs e)
    {
      this.mDisabledAbiPopup.IsOpen = false;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/HD-MultiInstanceManager;component/view.classes/createinstance/createnewinstanceview.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.CustomRam = (CustomTextBox) target;
          break;
        case 2:
          this.OrientationComboBox = (CustomComboBox) target;
          break;
        case 3:
          this.ResolutionComboBox = (CustomComboBox) target;
          break;
        case 4:
          this.CustomResolutionTextBoxes = (StackPanel) target;
          break;
        case 5:
          this.CustomResolutionWidth = (CustomTextBox) target;
          break;
        case 6:
          this.CustomResolutionHeight = (CustomTextBox) target;
          break;
        case 7:
          this.mAbi64AutoRadioButtonPanel = (StackPanel) target;
          this.mAbi64AutoRadioButtonPanel.MouseEnter += new MouseEventHandler(this.CustomRadioButtonAuto_MouseEnter);
          this.mAbi64AutoRadioButtonPanel.MouseLeave += new MouseEventHandler(this.CustomRadioButtonAuto_MouseLeave);
          break;
        case 8:
          this.mAbi64AutoRadioButton = (CustomRadioButton) target;
          break;
        case 9:
          this.mAbi64ARMRadioButtonPanel = (StackPanel) target;
          this.mAbi64ARMRadioButtonPanel.MouseEnter += new MouseEventHandler(this.CustomRadioButtonARM_MouseEnter);
          this.mAbi64ARMRadioButtonPanel.MouseLeave += new MouseEventHandler(this.CustomRadioButtonARM_MouseLeave);
          break;
        case 10:
          this.mAbi64ARMRadioButton = (CustomRadioButton) target;
          break;
        case 11:
          this.mDisabledAbiPopup = (CustomPopUp) target;
          break;
        case 12:
          this.mDisabledAbiPopupBorder = (Border) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
