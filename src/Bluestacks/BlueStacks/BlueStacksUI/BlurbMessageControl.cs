// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.BlurbMessageControl
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;

namespace BlueStacks.BlueStacksUI
{
  public class BlurbMessageControl : UserControl, IComponentConnector
  {
    internal Run FirstMessage;
    internal TextBlock KeyMessage;
    internal Run SecondMessage;
    private bool _contentLoaded;

    public BlurbMessageControl()
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
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/blurbmessagecontrol.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.FirstMessage = (Run) target;
          break;
        case 2:
          this.KeyMessage = (TextBlock) target;
          break;
        case 3:
          this.SecondMessage = (Run) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
