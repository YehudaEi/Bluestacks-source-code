// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.NoInternetControl
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
  public class NoInternetControl : UserControl, IComponentConnector
  {
    private BrowserControl AssociatedControl;
    internal TextBlock mFailureTextBox;
    internal TextBlock mErrorLine1;
    internal TextBlock mErrorLine2;
    internal CustomButton mBlueButton;
    private bool _contentLoaded;

    public NoInternetControl(BrowserControl browserControl)
    {
      this.InitializeComponent();
      this.AssociatedControl = browserControl;
      BlueStacksUIBinding.Bind(this.mFailureTextBox, "STRING_NAVIGATE_FAILED", "");
      BlueStacksUIBinding.Bind((Button) this.mBlueButton, "STRING_RETRY_CONNECTION_ISSUE_TEXT1");
    }

    private void mBlueButton_Click(object sender, RoutedEventArgs e)
    {
      this.AssociatedControl.NavigateTo(this.AssociatedControl.mFailedUrl);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/nointernetcontrol.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.mFailureTextBox = (TextBlock) target;
          break;
        case 2:
          this.mErrorLine1 = (TextBlock) target;
          break;
        case 3:
          this.mErrorLine2 = (TextBlock) target;
          break;
        case 4:
          this.mBlueButton = (CustomButton) target;
          this.mBlueButton.Click += new RoutedEventHandler(this.mBlueButton_Click);
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
