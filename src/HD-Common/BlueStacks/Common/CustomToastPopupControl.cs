// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.CustomToastPopupControl
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace BlueStacks.Common
{
  public class CustomToastPopupControl : UserControl, IComponentConnector
  {
    private Window ParentWindow;
    private UserControl ParentControl;
    internal Border mToastPopupBorder;
    internal CustomPictureBox mToastIcon;
    internal TextBlock mToastTextblock;
    internal Grid mVerticalSeparator;
    internal CustomPictureBox mToastCloseIcon;
    private bool _contentLoaded;

    public CustomToastPopupControl()
    {
      this.InitializeComponent();
    }

    public CustomToastPopupControl(Window window)
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

    public CustomToastPopupControl(UserControl control)
    {
      this.InitializeComponent();
      if (control == null)
        return;
      this.ParentControl = control;
      Grid grid = new Grid();
      object content = control.Content;
      control.Content = (object) grid;
      grid.Children.Add(content as UIElement);
      grid.Children.Add((UIElement) this);
    }

    public void Init(
      Window window,
      string text,
      Brush background = null,
      Brush borderBackground = null,
      HorizontalAlignment horizontalAlign = HorizontalAlignment.Center,
      VerticalAlignment verticalAlign = VerticalAlignment.Bottom,
      Thickness? margin = null,
      int cornerRadius = 12,
      Thickness? toastTextMargin = null,
      Brush toastTextForeground = null,
      bool isShowCloseIcon = false,
      bool isShowVerticalSeparator = false)
    {
      this.mToastIcon.Visibility = Visibility.Collapsed;
      if (window != null)
        this.ParentWindow = window;
      if (isShowCloseIcon)
        this.mToastCloseIcon.Visibility = Visibility.Visible;
      else
        this.mToastCloseIcon.Visibility = Visibility.Collapsed;
      this.mVerticalSeparator.Visibility = isShowVerticalSeparator ? Visibility.Visible : Visibility.Collapsed;
      this.InitProperties(0, text, background, borderBackground, horizontalAlign, verticalAlign, margin, cornerRadius, toastTextMargin, toastTextForeground, isShowCloseIcon, isShowVerticalSeparator);
    }

    public void Init(
      UserControl control,
      string text,
      Brush background = null,
      Brush borderBackground = null,
      HorizontalAlignment horizontalAlign = HorizontalAlignment.Center,
      VerticalAlignment verticalAlign = VerticalAlignment.Bottom,
      Thickness? margin = null,
      int cornerRadius = 12,
      Thickness? toastTextMargin = null,
      Brush toastTextForeground = null)
    {
      this.mToastIcon.Visibility = Visibility.Collapsed;
      if (control != null)
        this.ParentControl = control;
      this.InitProperties(1, text, background, borderBackground, horizontalAlign, verticalAlign, margin, cornerRadius, toastTextMargin, toastTextForeground, false, false);
    }

    private void InitProperties(
      int callType,
      string text,
      Brush background = null,
      Brush borderBackground = null,
      HorizontalAlignment horizontalAlign = HorizontalAlignment.Center,
      VerticalAlignment verticalAlign = VerticalAlignment.Bottom,
      Thickness? margin = null,
      int cornerRadius = 12,
      Thickness? toastTextMargin = null,
      Brush toastTextForeground = null,
      bool isCloseIconVisible = false,
      bool isVerticalSeparatorVisible = false)
    {
      this.mToastPopupBorder.Background = background != null ? background : (Brush) new SolidColorBrush((Color) ColorConverter.ConvertFromString("#AE000000"));
      if (borderBackground == null)
      {
        this.mToastPopupBorder.BorderThickness = new Thickness(0.0);
      }
      else
      {
        this.mToastPopupBorder.BorderBrush = borderBackground;
        this.mToastPopupBorder.BorderThickness = new Thickness(1.0);
      }
      if (!margin.HasValue)
        this.mToastPopupBorder.Margin = new Thickness(0.0, 0.0, 0.0, 40.0);
      else
        this.mToastPopupBorder.Margin = margin.Value;
      if (!toastTextMargin.HasValue)
        this.mToastTextblock.Margin = new Thickness(0.0);
      else
        this.mToastTextblock.Margin = toastTextMargin.Value;
      this.mToastTextblock.Foreground = toastTextForeground != null ? toastTextForeground : (Brush) Brushes.White;
      this.mToastTextblock.FontSize = 15.0;
      this.mVerticalSeparator.Background = (Brush) new SolidColorBrush((Color) ColorConverter.ConvertFromString("#46474A"));
      this.mToastPopupBorder.CornerRadius = new CornerRadius((double) cornerRadius);
      this.mToastTextblock.Text = text;
      this.mToastPopupBorder.VerticalAlignment = verticalAlign;
      this.mToastPopupBorder.HorizontalAlignment = horizontalAlign;
      this.mToastTextblock.TextWrapping = TextWrapping.WrapWithOverflow;
      this.mToastCloseIcon.Margin = isVerticalSeparatorVisible ? new Thickness(20.0, 0.0, 0.0, 0.0) : new Thickness(8.0, 0.0, 2.0, 0.0);
      this.mToastCloseIcon.Height = isVerticalSeparatorVisible ? 16.0 : 12.0;
      this.mToastCloseIcon.Width = isVerticalSeparatorVisible ? 16.0 : 12.0;
      if (callType == 0)
        this.mToastTextblock.MaxWidth = this.ParentWindow.ActualWidth - (double) cornerRadius - 15.0;
      else
        this.mToastTextblock.MaxWidth = this.ParentControl.ActualWidth - (double) cornerRadius - 15.0;
      this.mToastTextblock.TextAlignment = TextAlignment.Center;
      if (!isCloseIconVisible)
        return;
      if (isVerticalSeparatorVisible)
      {
        this.mToastTextblock.FontSize = 16.0;
        this.mToastTextblock.Margin = new Thickness(0.0, 6.0, 0.0, 6.0);
        if (this.FindResource((object) "ShadowBorder") is Style resource)
          this.mToastPopupBorder.Style = resource;
        this.mToastTextblock.MaxWidth = this.ParentWindow.ActualWidth - (double) cornerRadius - 90.0;
      }
      else
        this.mToastTextblock.MaxWidth = this.ParentWindow.ActualWidth - (double) cornerRadius - 30.0;
    }

    public void AddImage(string imageName, double height = 0.0, double width = 0.0, Thickness? margin = null)
    {
      this.mToastIcon.ImageName = imageName;
      if (height != 0.0)
        this.mToastIcon.Height = height;
      if (width != 0.0)
        this.mToastIcon.Width = width;
      if (margin.HasValue)
        this.mToastIcon.Margin = margin.Value;
      this.mToastIcon.Visibility = Visibility.Visible;
    }

    public void ShowPopup(double seconds = 1.3)
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

    private void ToastCloseIcon_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.Visibility = Visibility.Collapsed;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/HD-Common;component/uielements/customtoastpopupcontrol.xaml", UriKind.Relative));
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
          this.mToastPopupBorder = (Border) target;
          break;
        case 2:
          this.mToastIcon = (CustomPictureBox) target;
          break;
        case 3:
          this.mToastTextblock = (TextBlock) target;
          break;
        case 4:
          this.mVerticalSeparator = (Grid) target;
          break;
        case 5:
          this.mToastCloseIcon = (CustomPictureBox) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
