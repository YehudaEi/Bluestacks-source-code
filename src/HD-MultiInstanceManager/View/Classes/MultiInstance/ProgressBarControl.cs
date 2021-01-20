// Decompiled with JetBrains decompiler
// Type: MultiInstanceManagerMVVM.View.Classes.MultiInstance.ProgressBarControl
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

namespace MultiInstanceManagerMVVM.View.Classes.MultiInstance
{
  public class ProgressBarControl : UiUserControlBase, IComponentConnector
  {
    internal TextBlock mDownloading;
    internal TextBlock mPercentage;
    internal BlueProgressBar mUpdateDownloadProgressBar;
    private bool _contentLoaded;

    public ProgressBarControl()
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
      Application.LoadComponent((object) this, new Uri("/HD-MultiInstanceManager;component/view.classes/multiinstance/progressbarcontrol.xaml", UriKind.Relative));
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
          this.mDownloading = (TextBlock) target;
          break;
        case 2:
          this.mPercentage = (TextBlock) target;
          break;
        case 3:
          this.mUpdateDownloadProgressBar = (BlueProgressBar) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
