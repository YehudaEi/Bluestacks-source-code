// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.FullScreenToastPopupControl
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
using System.Windows.Media.Animation;

namespace BlueStacks.BlueStacksUI
{
  public class FullScreenToastPopupControl : UserControl, IComponentConnector
  {
    private MainWindow ParentWindow;
    internal Border mToastPopupBorder;
    internal Border mMaskBorder;
    internal DockPanel mToastPanel;
    internal TextBlock mTipTextblock;
    internal Border mKeyBorder;
    internal TextBlock mKeyTextBlock;
    internal TextBlock mInfoTextblock;
    internal CustomPictureBox mToastIcon;
    private bool _contentLoaded;

    public FullScreenToastPopupControl()
    {
      this.InitializeComponent();
    }

    public FullScreenToastPopupControl(MainWindow window)
    {
      this.InitializeComponent();
      if (window == null)
        return;
      this.ParentWindow = window;
      Grid grid = new Grid();
      object content = window.Content;
      window.Content = (object) grid;
      grid.Children.Add(content as UIElement);
      grid.Children.Add((UIElement) this);
    }

    public void Init(MainWindow window, string text)
    {
      if (window != null)
      {
        this.ParentWindow = window;
        this.mToastPanel.MaxWidth = this.ParentWindow.ActualWidth - 15.0;
      }
      this.mTipTextblock.Text = text;
      this.mInfoTextblock.Visibility = Visibility.Collapsed;
      this.mKeyBorder.Visibility = Visibility.Collapsed;
    }

    public void Init(MainWindow window, string tip, string key, string info)
    {
      if (window != null)
      {
        this.ParentWindow = window;
        this.mToastPanel.MaxWidth = this.ParentWindow.ActualWidth - 15.0;
      }
      this.mTipTextblock.Text = tip;
      this.mKeyTextBlock.Text = key;
      this.mInfoTextblock.Text = info;
      this.mInfoTextblock.Visibility = Visibility.Visible;
      this.mKeyBorder.Visibility = Visibility.Visible;
    }

    public void ShowPopup(double seconds = 4.0)
    {
      this.Visibility = Visibility.Visible;
      this.Opacity = 0.0;
      DoubleAnimation doubleAnimation1 = new DoubleAnimation();
      doubleAnimation1.From = new double?(0.0);
      doubleAnimation1.To = new double?(seconds);
      doubleAnimation1.Duration = new Duration(TimeSpan.FromSeconds(0.3));
      DoubleAnimation doubleAnimation2 = doubleAnimation1;
      Storyboard storyboard1 = new Storyboard();
      storyboard1.Children.Add((Timeline) doubleAnimation2);
      Storyboard.SetTarget((DependencyObject) doubleAnimation2, (DependencyObject) this);
      Storyboard.SetTargetProperty((DependencyObject) doubleAnimation2, new PropertyPath((object) UIElement.OpacityProperty));
      storyboard1.Completed += (EventHandler) ((_param1_1, _param2_1) =>
      {
        this.Visibility = Visibility.Visible;
        DoubleAnimation doubleAnimation3 = new DoubleAnimation()
        {
          From = new double?(seconds),
          To = new double?(0.0),
          FillBehavior = FillBehavior.Stop,
          BeginTime = new TimeSpan?(TimeSpan.FromSeconds(seconds)),
          Duration = new Duration(TimeSpan.FromSeconds(seconds / 2.0))
        };
        Storyboard storyboard2 = new Storyboard();
        storyboard2.Children.Add((Timeline) doubleAnimation3);
        Storyboard.SetTarget((DependencyObject) doubleAnimation3, (DependencyObject) this);
        Storyboard.SetTargetProperty((DependencyObject) doubleAnimation3, new PropertyPath((object) UIElement.OpacityProperty));
        storyboard2.Completed += (EventHandler) ((_param1_2, _param2_2) => this.Visibility = Visibility.Collapsed);
        storyboard2.Begin();
      });
      storyboard1.Begin();
    }

    private void ToastIcon_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.ParentWindow.CloseFullScreenToastAndStopTimer();
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/fullscreentoastpopupcontrol.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.mToastPopupBorder = (Border) target;
          break;
        case 2:
          this.mMaskBorder = (Border) target;
          break;
        case 3:
          this.mToastPanel = (DockPanel) target;
          break;
        case 4:
          this.mTipTextblock = (TextBlock) target;
          break;
        case 5:
          this.mKeyBorder = (Border) target;
          break;
        case 6:
          this.mKeyTextBlock = (TextBlock) target;
          break;
        case 7:
          this.mInfoTextblock = (TextBlock) target;
          break;
        case 8:
          this.mToastIcon = (CustomPictureBox) target;
          this.mToastIcon.MouseLeftButtonUp += new MouseButtonEventHandler(this.ToastIcon_MouseLeftButtonUp);
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
