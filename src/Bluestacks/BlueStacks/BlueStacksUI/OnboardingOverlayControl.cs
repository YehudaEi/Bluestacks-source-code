// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.OnboardingOverlayControl
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
  public class OnboardingOverlayControl : UserControl, IComponentConnector
  {
    internal Border mBorder;
    internal Grid mGrid;
    internal CustomPictureBox mOnboardingImg;
    private bool _contentLoaded;

    public OnboardingOverlayControl()
    {
      this.InitializeComponent();
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/keymap/uielement/onboardingoverlaycontrol.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.mBorder = (Border) target;
          break;
        case 2:
          this.mGrid = (Grid) target;
          break;
        case 3:
          this.mOnboardingImg = (CustomPictureBox) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
