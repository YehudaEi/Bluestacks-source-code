// Decompiled with JetBrains decompiler
// Type: BlueStacks.Uninstaller.UninstallProgress
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
  public partial class UninstallProgress : UserControl, IComponentConnector
  {
    internal StackPanel mProgressStackPanel;
    internal Label mInstallProgressStatus;
    internal Label mInstallProgressPercentage;
    internal ProgressBar mInstallProgressBar;
    private bool _contentLoaded;

    public UninstallProgress()
    {
      this.InitializeComponent();
      if (!Oem.IsOEMDmm)
        return;
      this.Height = 100.0;
      this.mProgressStackPanel.Margin = new Thickness(-90.0, -20.0, 20.0, 0.0);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/BlueStacksUninstaller;component/uninstallprogress.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.mProgressStackPanel = (StackPanel) target;
          break;
        case 2:
          this.mInstallProgressStatus = (Label) target;
          break;
        case 3:
          this.mInstallProgressPercentage = (Label) target;
          break;
        case 4:
          this.mInstallProgressBar = (ProgressBar) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
