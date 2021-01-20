// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.DimControlWithProgresBar
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
using System.Windows.Input;
using System.Windows.Markup;

namespace BlueStacks.BlueStacksUI
{
  public class DimControlWithProgresBar : UserControl, IComponentConnector
  {
    private MainWindow mMainWindow;
    internal Grid mControlGrid;
    internal Grid mTopBar;
    internal CustomPictureBox mBackButton;
    internal Label mTitleLabel;
    internal CustomPictureBox mCloseButton;
    internal Grid mControlParentGrid;
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

    public Control ParentControl { get; set; }

    public Panel ChildControl { get; set; }

    public DimControlWithProgresBar()
    {
      this.InitializeComponent();
    }

    internal void Init(Control parentControl, Panel childControl, bool isWindowForced, bool _)
    {
      this.ParentControl = parentControl;
      this.ChildControl = childControl;
      this.FixUpUILayout();
      if (isWindowForced)
      {
        this.mBackButton.Visibility = Visibility.Hidden;
        this.mCloseButton.Visibility = Visibility.Hidden;
      }
      this.ParentWindow.SizeChanged += new SizeChangedEventHandler(this.MainWindow_SizeChanged);
    }

    private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      this.FixUpUILayout();
    }

    private void FixUpUILayout()
    {
      this.mControlGrid.Height = (double) ((long) (int) (this.ParentWindow.mWelcomeTab.ActualHeight * 0.8 / (double) this.ParentWindow.mAspectRatio.Denominator) * this.ParentWindow.mAspectRatio.Denominator);
      if (this.ParentWindow.mWelcomeTab.ActualHeight * 0.9 - this.mControlGrid.Height > 10.0)
        this.mControlGrid.Height = this.ParentWindow.mWelcomeTab.ActualHeight * 0.8;
      this.mControlGrid.Height += 50.0;
      this.mControlGrid.Width = (this.mControlGrid.Height - 50.0) * this.ParentWindow.mAspectRatio.DoubleValue + 10.0;
    }

    internal void ShowContent()
    {
      this.DimBackground();
      if (this.ChildControl != null)
      {
        if (this.ChildControl.Parent != null)
          (this.ChildControl.Parent as Panel).Children.Remove((UIElement) this.ChildControl);
        this.mControlParentGrid.Children.Add((UIElement) this.ChildControl);
      }
      this.mControlGrid.Visibility = Visibility.Visible;
    }

    private void CloseButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      Logger.Info("Cicked DimControl close button");
      this.HideWindow();
    }

    internal void DimBackground()
    {
      Logger.Info("Diming popup window");
      if (this.ParentControl != null)
        this.ParentControl.Visibility = Visibility.Visible;
      this.Visibility = Visibility.Visible;
    }

    private void BackButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      Logger.Info("Clicked Back Button");
      this.ParentWindow.mCommonHandler.BackButtonHandler(false);
    }

    internal void HideWindow()
    {
      Logger.Debug("Hiding popup window");
      this.Visibility = Visibility.Hidden;
      this.mControlGrid.Visibility = Visibility.Hidden;
      if (this.ParentControl == null)
        return;
      this.ParentControl.Visibility = Visibility.Hidden;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/dimcontrolwithprogresbar.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.mControlGrid = (Grid) target;
          break;
        case 2:
          this.mTopBar = (Grid) target;
          break;
        case 3:
          this.mBackButton = (CustomPictureBox) target;
          this.mBackButton.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.BackButton_PreviewMouseLeftButtonUp);
          break;
        case 4:
          this.mTitleLabel = (Label) target;
          break;
        case 5:
          this.mCloseButton = (CustomPictureBox) target;
          this.mCloseButton.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.CloseButton_PreviewMouseLeftButtonUp);
          break;
        case 6:
          this.mControlParentGrid = (Grid) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
