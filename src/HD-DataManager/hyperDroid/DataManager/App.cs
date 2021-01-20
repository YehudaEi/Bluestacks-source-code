// Decompiled with JetBrains decompiler
// Type: BlueStacks.hyperDroid.DataManager.App
// Assembly: HD-DataManager, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 4AB16B4A-CAF7-4470-9488-3C5B163E3D07
// Assembly location: C:\Program Files\BlueStacks\HD-DataManager.exe

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;

namespace BlueStacks.hyperDroid.DataManager
{
  public class App : Application, IComponentConnector
  {
    private bool _contentLoaded;

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/HD-DataManager;component/app.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      this._contentLoaded = true;
    }
  }
}
