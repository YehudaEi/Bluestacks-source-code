// Decompiled with JetBrains decompiler
// Type: BlueStacks.Uninstaller.UninstallFinish
// Assembly: BlueStacksUninstaller, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: DBF002A0-6BF3-43CC-B5E7-0E90D1C19949
// Assembly location: C:\Program Files\BlueStacks\BlueStacksUninstaller.exe

using BlueStacks.Common;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace BlueStacks.Uninstaller
{
  public partial class UninstallFinish : UserControl, IComponentConnector
  {
    internal Button mUninstallFinished;
    internal Label mUninstallFinishedLabel;
    internal Label mThankLabel;
    private bool _contentLoaded;

    public UninstallFinish()
    {
      this.InitializeComponent();
      if (!Oem.IsOEMDmm)
        return;
      this.Height = 100.0;
      this.mUninstallFinishedLabel.FontSize = 12.5;
      this.mUninstallFinishedLabel.Margin = new Thickness(-100.0, 15.0, 0.0, 0.0);
      this.mThankLabel.FontSize = 12.5;
      this.mThankLabel.Margin = new Thickness(-90.0, 10.0, 0.0, 30.0);
      this.mUninstallFinished.FontSize = 20.0;
      this.mUninstallFinished.Margin = new Thickness(-45.0, 0.0, 0.0, 15.0);
      BlueStacksUIBinding.BindColor((DependencyObject) this.mUninstallFinishedLabel, Control.ForegroundProperty, "InstallerWindowLightTextForeground");
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/BlueStacksUninstaller;component/uninstallfinish.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.mUninstallFinished = (Button) target;
          break;
        case 2:
          this.mUninstallFinishedLabel = (Label) target;
          break;
        case 3:
          this.mThankLabel = (Label) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
