// Decompiled with JetBrains decompiler
// Type: BlueStacks.DataManager.ProgressWindow
// Assembly: HD-DataManager, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 4AB16B4A-CAF7-4470-9488-3C5B163E3D07
// Assembly location: C:\Program Files\BlueStacks\HD-DataManager.exe

using BlueStacks.Common;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace BlueStacks.DataManager
{
  public class ProgressWindow : Window, IComponentConnector
  {
    public int percentDone;
    public static bool isCanceled;
    internal Border clipMask;
    internal Grid mProgressWindowGrid;
    internal RowDefinition mLastRow;
    internal CustomPictureBox MinimizeBtn;
    internal TextBlock mProgressHeader;
    internal TextBlock mProgressText;
    internal BlueProgressBar mProgressBar;
    internal CustomButton mBtnGrid;
    private bool _contentLoaded;

    public event ProgressWindow.oncancelbuttonclick CancelBtnClicked;

    public ProgressWindow()
    {
      this.InitializeComponent();
      this.Title = Strings.ProductDisplayName;
      this.Loaded += new RoutedEventHandler(this.ProgressWindow_Loaded);
      this.Closing += new CancelEventHandler(this.ProgressWindow_Closing);
    }

    private void ProgressWindow_Loaded(object sender, RoutedEventArgs e)
    {
      if (!App.sOpt.s)
        return;
      this.Visibility = Visibility.Hidden;
      this.ShowInTaskbar = false;
    }

    private void ProgressWindow_Closing(object sender, CancelEventArgs e)
    {
      Application.Current.Shutdown();
    }

    public void ExitWindow()
    {
      this.Dispatcher.Invoke((Delegate) (() => this.Hide()));
    }

    private void MinimizeBtn_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.WindowState = WindowState.Minimized;
    }

    private void DraggableGrid_MouseDown(object sender, MouseButtonEventArgs e)
    {
      this.DragMove();
    }

    private void CancelBtn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.CancelBtnClicked();
      this.Hide();
      ProgressWindow.isCanceled = true;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/HD-DataManager;component/progresswindow.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.clipMask = (Border) target;
          break;
        case 2:
          this.mProgressWindowGrid = (Grid) target;
          break;
        case 3:
          this.mLastRow = (RowDefinition) target;
          break;
        case 4:
          ((UIElement) target).MouseDown += new MouseButtonEventHandler(this.DraggableGrid_MouseDown);
          break;
        case 5:
          ((UIElement) target).MouseDown += new MouseButtonEventHandler(this.DraggableGrid_MouseDown);
          break;
        case 6:
          this.MinimizeBtn = (CustomPictureBox) target;
          this.MinimizeBtn.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.MinimizeBtn_PreviewMouseLeftButtonUp);
          break;
        case 7:
          ((UIElement) target).MouseDown += new MouseButtonEventHandler(this.DraggableGrid_MouseDown);
          break;
        case 8:
          this.mProgressHeader = (TextBlock) target;
          break;
        case 9:
          this.mProgressText = (TextBlock) target;
          break;
        case 10:
          this.mProgressBar = (BlueProgressBar) target;
          break;
        case 11:
          this.mBtnGrid = (CustomButton) target;
          this.mBtnGrid.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.CancelBtn_MouseLeftButtonUp);
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }

    public delegate void oncancelbuttonclick();
  }
}
