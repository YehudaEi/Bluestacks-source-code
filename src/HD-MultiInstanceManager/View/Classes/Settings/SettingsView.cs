// Decompiled with JetBrains decompiler
// Type: MultiInstanceManagerMVVM.View.Classes.Settings.SettingsView
// Assembly: HD-MultiInstanceManager, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 0C4C54E8-53C7-4A46-AE96-CC8E89A6B884
// Assembly location: C:\Program Files\BlueStacks\HD-MultiInstanceManager.exe

using BlueStacks.Core;
using MultiInstanceManagerMVVM.View.Classes.MultiInstance.Settings;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace MultiInstanceManagerMVVM.View.Classes.Settings
{
  public class SettingsView : UiWindowBase, IComponentConnector
  {
    internal Grid mInstanceSettingsControl;
    private bool _contentLoaded;

    public SettingsView()
    {
      this.InitializeComponent();
    }

    public SettingsView(string vmName, string startUpTab, bool isEcoModeEnabled)
      : this()
    {
      this.mInstanceSettingsControl.Children.Add((UIElement) new SettingsWindow((Window) this, vmName, startUpTab, isEcoModeEnabled));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/HD-MultiInstanceManager;component/view.classes/settings/settingsview.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      if (connectionId == 1)
        this.mInstanceSettingsControl = (Grid) target;
      else
        this._contentLoaded = true;
    }
  }
}
