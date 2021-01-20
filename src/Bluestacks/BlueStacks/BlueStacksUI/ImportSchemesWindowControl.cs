// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.ImportSchemesWindowControl
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
  public class ImportSchemesWindowControl : UserControl, IComponentConnector
  {
    internal ImportSchemesWindow mImportSchemesWindow;
    internal MainWindow ParentWindow;
    internal CustomCheckbox mContent;
    internal Grid mBlock;
    internal CustomTextBox mImportName;
    internal TextBlock mWarningMsg;
    private bool _contentLoaded;

    public ImportSchemesWindowControl(ImportSchemesWindow importSchemesWindow, MainWindow window)
    {
      this.InitializeComponent();
      this.mImportSchemesWindow = importSchemesWindow;
      this.ParentWindow = window;
    }

    private void box_Checked(object sender, RoutedEventArgs e)
    {
      this.mImportSchemesWindow.Box_Checked(sender, e);
    }

    private void box_Unchecked(object sender, RoutedEventArgs e)
    {
      this.mImportSchemesWindow.Box_Unchecked(sender, e);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/keymap/uielement/importschemeswindowcontrol.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.mContent = (CustomCheckbox) target;
          this.mContent.Checked += new RoutedEventHandler(this.box_Checked);
          this.mContent.Unchecked += new RoutedEventHandler(this.box_Unchecked);
          break;
        case 2:
          this.mBlock = (Grid) target;
          break;
        case 3:
          this.mImportName = (CustomTextBox) target;
          break;
        case 4:
          this.mWarningMsg = (TextBlock) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
