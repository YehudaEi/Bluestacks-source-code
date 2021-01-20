// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.Controls.SkinSelectorControl
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

namespace BlueStacks.BlueStacksUI.Controls
{
  public class SkinSelectorControl : UserControl, IComponentConnector
  {
    internal CustomPictureBox mThemeImage;
    internal TextBlock mThemeName;
    internal CustomButton mThemeCheckButton;
    internal TextBlock mThemeAppliedText;
    private bool _contentLoaded;

    public SkinSelectorControl()
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
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/skinselectorcontrol.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.mThemeImage = (CustomPictureBox) target;
          break;
        case 2:
          this.mThemeName = (TextBlock) target;
          break;
        case 3:
          this.mThemeCheckButton = (CustomButton) target;
          break;
        case 4:
          this.mThemeAppliedText = (TextBlock) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
