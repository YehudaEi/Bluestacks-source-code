// Decompiled with JetBrains decompiler
// Type: BlueStacks.LogCollector.ProgressWindow
// Assembly: HD-LogCollector, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: FD44426D-0F78-46B3-8118-1E64B979C51B
// Assembly location: C:\Program Files\BlueStacks\HD-LogCollector.exe

using BlueStacks.Common;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace BlueStacks.LogCollector
{
  public partial class ProgressWindow : Window, IComponentConnector
  {
    internal Border mBorder;
    internal CustomPictureBox MinimizeBtn;
    internal TextBlock mProgressText;
    internal BlueProgressBar mProgressBar;
    internal CustomButton CancelGrid;
    private bool _contentLoaded;

    public ProgressWindow()
    {
      this.InitializeComponent();
      this.Title = LocaleStrings.GetLocalizedString("STRING_BST_SUPPORT_UTILITY", "");
    }

    private void CancelGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      Console.WriteLine("::::: User clicked cancel button :::::");
      this.Hide();
      this.Close();
    }

    private void MinimizeBtn_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.WindowState = WindowState.Minimized;
    }

    private void DraggableGrid_MouseDown(object sender, MouseButtonEventArgs e)
    {
      if (e.OriginalSource.GetType() == typeof (CustomPictureBox))
        return;
      try
      {
        this.DragMove();
      }
      catch
      {
      }
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/HD-LogCollector;component/progresswindow.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.mBorder = (Border) target;
          this.mBorder.MouseDown += new MouseButtonEventHandler(this.DraggableGrid_MouseDown);
          break;
        case 2:
          this.MinimizeBtn = (CustomPictureBox) target;
          this.MinimizeBtn.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.MinimizeBtn_PreviewMouseLeftButtonUp);
          break;
        case 3:
          this.mProgressText = (TextBlock) target;
          break;
        case 4:
          this.mProgressBar = (BlueProgressBar) target;
          break;
        case 5:
          this.CancelGrid = (CustomButton) target;
          this.CancelGrid.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.CancelGrid_MouseLeftButtonUp);
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
