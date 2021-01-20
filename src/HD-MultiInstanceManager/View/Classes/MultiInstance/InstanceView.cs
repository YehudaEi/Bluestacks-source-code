// Decompiled with JetBrains decompiler
// Type: MultiInstanceManagerMVVM.View.Classes.MultiInstance.InstanceView
// Assembly: HD-MultiInstanceManager, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 0C4C54E8-53C7-4A46-AE96-CC8E89A6B884
// Assembly location: C:\Program Files\BlueStacks\HD-MultiInstanceManager.exe

using BlueStacks.Common;
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
  public class InstanceView : UiUserControlBase, IView, IComponentConnector
  {
    internal Border mMainBorder;
    internal CustomCheckbox mInstanceCheckbox;
    internal CustomTextBox mInstanceName;
    internal TextBlock mEngineName;
    internal ProgressBarControl mUpdateDownloadProgressBar;
    private bool _contentLoaded;

    public IViewModel ViewModel { get; set; }

    public InstanceView()
    {
      this.InitializeComponent();
    }

    private void InstanceView_Loaded(object sender, RoutedEventArgs e)
    {
      ((IViewModel) this.DataContext).View = (IView) this;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/HD-MultiInstanceManager;component/view.classes/multiinstance/instanceview.xaml", UriKind.Relative));
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
          ((FrameworkElement) target).Loaded += new RoutedEventHandler(this.InstanceView_Loaded);
          break;
        case 2:
          this.mMainBorder = (Border) target;
          break;
        case 3:
          this.mInstanceCheckbox = (CustomCheckbox) target;
          break;
        case 4:
          this.mInstanceName = (CustomTextBox) target;
          break;
        case 5:
          this.mEngineName = (TextBlock) target;
          break;
        case 6:
          this.mUpdateDownloadProgressBar = (ProgressBarControl) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
