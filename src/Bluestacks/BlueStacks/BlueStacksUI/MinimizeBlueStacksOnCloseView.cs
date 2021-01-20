// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.MinimizeBlueStacksOnCloseView
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace BlueStacks.BlueStacksUI
{
  public class MinimizeBlueStacksOnCloseView : UserControl, IComponentConnector
  {
    internal Grid mTitleGrid;
    internal CustomPictureBox mCrossButtonPictureBox;
    internal TextBlock mTitleText;
    internal TextBlock mHeaderText;
    internal CustomRadioButton mMinimizeRadioBtn;
    internal TextBlock mMinimizeBtnBodyText;
    internal CustomRadioButton mQuitRadioBtn;
    internal TextBlock mQuitBtnBodyText;
    internal CustomCheckbox mDoNotShowChkBox;
    internal StackPanel mBtnActionPanel;
    internal CustomButton mCancelBtn;
    internal CustomButton mMinimizeBtn;
    internal CustomButton mQuitBtn;
    private bool _contentLoaded;

    public MinimizeBlueStacksOnCloseView(MainWindow window)
    {
      this.InitializeComponent();
      this.DataContext = (object) new MinimizeBlueStacksOnCloseViewModel(window);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/minimizebluestacksonclose/view/minimizebluestacksoncloseview.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.mTitleGrid = (Grid) target;
          break;
        case 2:
          this.mCrossButtonPictureBox = (CustomPictureBox) target;
          break;
        case 3:
          this.mTitleText = (TextBlock) target;
          break;
        case 4:
          this.mHeaderText = (TextBlock) target;
          break;
        case 5:
          this.mMinimizeRadioBtn = (CustomRadioButton) target;
          break;
        case 6:
          this.mMinimizeBtnBodyText = (TextBlock) target;
          break;
        case 7:
          this.mQuitRadioBtn = (CustomRadioButton) target;
          break;
        case 8:
          this.mQuitBtnBodyText = (TextBlock) target;
          break;
        case 9:
          this.mDoNotShowChkBox = (CustomCheckbox) target;
          break;
        case 10:
          this.mBtnActionPanel = (StackPanel) target;
          break;
        case 11:
          this.mCancelBtn = (CustomButton) target;
          break;
        case 12:
          this.mMinimizeBtn = (CustomButton) target;
          break;
        case 13:
          this.mQuitBtn = (CustomButton) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
