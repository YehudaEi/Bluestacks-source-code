// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.DMMProgressControl
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
  public class DMMProgressControl : UserControl, IComponentConnector
  {
    internal TextBlock BootText;
    private bool _contentLoaded;

    public DMMProgressControl()
    {
      this.InitializeComponent();
      if (DesignerProperties.GetIsInDesignMode((DependencyObject) this))
        return;
      BlueStacksUIBinding.Bind(this.BootText, "STRING_BOOT_TIME", "");
      if (RegistryManager.Instance.LastBootTime / 400 > 0)
        return;
      RegistryManager.Instance.LastBootTime = 120000;
      RegistryManager.Instance.NoOfBootCompleted = 0;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/dmmprogresscontrol.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      if (connectionId == 1)
        this.BootText = (TextBlock) target;
      else
        this._contentLoaded = true;
    }
  }
}
