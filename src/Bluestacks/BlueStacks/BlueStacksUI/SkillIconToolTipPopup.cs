// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.SkillIconToolTipPopup
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Shapes;

namespace BlueStacks.BlueStacksUI
{
  public class SkillIconToolTipPopup : CustomPopUp, IComponentConnector
  {
    internal Border mMaskBorder1;
    internal TextBlock mSkillIconHeaderText;
    internal Path mDownArrow;
    private bool _contentLoaded;

    public SkillIconToolTipPopup(CanvasElement canvasElement)
    {
      this.InitializeComponent();
      if (canvasElement.ListActionItem.First<IMAction>().Type == KeyActionType.MOBASkill)
        this.PlacementTarget = (UIElement) canvasElement?.mSkillImage;
      else
        this.PlacementTarget = (UIElement) canvasElement?.mSettingsIcon;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/keymap/uielement/skillicontooltippopup.xaml", UriKind.Relative));
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
          this.mMaskBorder1 = (Border) target;
          break;
        case 2:
          this.mSkillIconHeaderText = (TextBlock) target;
          break;
        case 3:
          this.mDownArrow = (Path) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
