// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.WpfTextBoxControl
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace BlueStacks.Player
{
  public partial class WpfTextBoxControl : UserControl, IComponentConnector
  {
    internal TextBox mWpfTextBox;
    private bool _contentLoaded;

    public WpfTextBoxControl()
    {
      this.InitializeComponent();
    }

    protected override void OnGotFocus(RoutedEventArgs e)
    {
      base.OnGotFocus(e);
    }

    private void WpfTextBox_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
    {
      if (e.Command != ApplicationCommands.Copy && e.Command != ApplicationCommands.Cut && e.Command != ApplicationCommands.Paste)
        return;
      e.Handled = true;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/HD-Player;component/wpftextboxcontrol.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      if (connectionId == 1)
      {
        this.mWpfTextBox = (TextBox) target;
        this.mWpfTextBox.AddHandler(CommandManager.PreviewExecutedEvent, (Delegate) new ExecutedRoutedEventHandler(this.WpfTextBox_PreviewExecuted));
      }
      else
        this._contentLoaded = true;
    }
  }
}
