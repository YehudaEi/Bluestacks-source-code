// Decompiled with JetBrains decompiler
// Type: BlueStacks.Uninstaller.StartUninstall
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
  public partial class StartUninstall : UserControl, IComponentConnector
  {
    internal Button mUninstallCancel;
    internal Button mUninstallStart;
    private bool _contentLoaded;

    public StartUninstall()
    {
      this.InitializeComponent();
      if (!Oem.IsOEMDmm)
        return;
      this.Height = 100.0;
      this.mUninstallCancel.Width = 200.0;
      this.mUninstallStart.Width = 200.0;
      this.mUninstallCancel.Height = 55.0;
      this.mUninstallStart.Height = 55.0;
      this.mUninstallCancel.FontSize = 24.0;
      this.mUninstallStart.FontSize = 24.0;
      this.mUninstallCancel.Margin = new Thickness(0.0, 0.0, 0.0, 15.0);
      this.mUninstallStart.Margin = new Thickness(20.0, 0.0, 0.0, 15.0);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/BlueStacksUninstaller;component/startuninstall.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      if (connectionId != 1)
      {
        if (connectionId == 2)
          this.mUninstallStart = (Button) target;
        else
          this._contentLoaded = true;
      }
      else
        this.mUninstallCancel = (Button) target;
    }
  }
}
