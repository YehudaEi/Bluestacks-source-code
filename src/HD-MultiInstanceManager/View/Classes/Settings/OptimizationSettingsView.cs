// Decompiled with JetBrains decompiler
// Type: MultiInstanceManagerMVVM.View.Classes.Settings.OptimizationSettingsView
// Assembly: HD-MultiInstanceManager, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 0C4C54E8-53C7-4A46-AE96-CC8E89A6B884
// Assembly location: C:\Program Files\BlueStacks\HD-MultiInstanceManager.exe

using BlueStacks.Core;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace MultiInstanceManagerMVVM.View.Classes.Settings
{
  public class OptimizationSettingsView : UiWindowBase, IComponentConnector
  {
    internal OptimizationSettingsView OptimizationSettingsWindow;
    internal Grid mMainGrid;
    private bool _contentLoaded;

    public OptimizationSettingsView()
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
      Application.LoadComponent((object) this, new Uri("/HD-MultiInstanceManager;component/view.classes/settings/optimizationsettingsview.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.OptimizationSettingsWindow = (OptimizationSettingsView) target;
          break;
        case 2:
          this.mMainGrid = (Grid) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
