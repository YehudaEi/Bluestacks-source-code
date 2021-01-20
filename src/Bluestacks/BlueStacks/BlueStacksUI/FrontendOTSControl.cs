// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.FrontendOTSControl
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
  public class FrontendOTSControl : UserControl, IComponentConnector
  {
    private MainWindow mMainWindow;
    private EventHandler<EventArgs> OneTimeSetupCompletedEventHandle;
    internal DimControlWithProgresBar mBaseControl;
    private bool _contentLoaded;

    public MainWindow ParentWindow
    {
      get
      {
        if (this.mMainWindow == null)
          this.mMainWindow = Window.GetWindow((DependencyObject) this) as MainWindow;
        return this.mMainWindow;
      }
    }

    public FrontendOTSControl()
    {
      this.InitializeComponent();
      this.OneTimeSetupCompletedEventHandle += new EventHandler<EventArgs>(this.OneTimeSetup_Completed);
    }

    private void UserControl_IsVisibleChanged(object _1, DependencyPropertyChangedEventArgs _2)
    {
      if (this.Visibility != Visibility.Visible)
        return;
      BlueStacksUIBinding.Bind(this.mBaseControl.mTitleLabel, "STRING_GOOGLE_LOGIN_MESSAGE");
      this.mBaseControl.Init((Control) this, (Panel) this.ParentWindow.mFrontendGrid, true, true);
      this.mBaseControl.ShowContent();
      this.ParentWindow.mAppHandler.EventOnOneTimeSetupCompleted = this.OneTimeSetupCompletedEventHandle;
    }

    private void OneTimeSetup_Completed(object sender, EventArgs e)
    {
      this.Dispatcher.Invoke((Delegate) (() => this.mBaseControl.HideWindow()));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/frontendotscontrol.xaml", UriKind.Relative));
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
      if (connectionId != 1)
      {
        if (connectionId == 2)
          this.mBaseControl = (DimControlWithProgresBar) target;
        else
          this._contentLoaded = true;
      }
      else
        ((UIElement) target).IsVisibleChanged += new DependencyPropertyChangedEventHandler(this.UserControl_IsVisibleChanged);
    }
  }
}
