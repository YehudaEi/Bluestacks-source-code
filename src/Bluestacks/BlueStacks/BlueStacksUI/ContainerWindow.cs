// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.ContainerWindow
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
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace BlueStacks.BlueStacksUI
{
  public class ContainerWindow : CustomWindow, IComponentConnector
  {
    public static readonly DependencyProperty CustomCornerRadiusProperty = DependencyProperty.Register(nameof (CustomCornerRadius), typeof (CornerRadius), typeof (CustomWindow), new PropertyMetadata((object) new CornerRadius(-1.0)));
    public static readonly DependencyProperty CustomBorderBrushProperty = DependencyProperty.Register(nameof (CustomBorderBrush), typeof (Brush), typeof (Border), new PropertyMetadata((object) Brushes.Transparent));
    private bool IsCustomCornerRadius;
    private bool IsCustomBorderBrush;
    internal Border mShadowBorder;
    internal Border mOuterBorder;
    internal Grid mMainGrid;
    internal Border mMaskBorder;
    internal Grid ContentGrid;
    private bool _contentLoaded;

    public CornerRadius CustomCornerRadius
    {
      get
      {
        return (CornerRadius) this.GetValue(ContainerWindow.CustomCornerRadiusProperty);
      }
      set
      {
        this.SetValue(ContainerWindow.CustomCornerRadiusProperty, (object) value);
        this.IsCustomCornerRadius = true;
      }
    }

    public Brush CustomBorderBrush
    {
      get
      {
        return (Brush) this.GetValue(ContainerWindow.CustomBorderBrushProperty);
      }
      set
      {
        this.SetValue(ContainerWindow.CustomBorderBrushProperty, (object) value);
        this.IsCustomBorderBrush = true;
      }
    }

    public ContainerWindow(
      MainWindow window,
      UserControl control,
      double width,
      double height,
      bool autoHeight = false,
      bool isShow = true,
      bool isWindowTransparent = false,
      double radius = -1.0,
      Brush brush = null)
    {
      ContainerWindow containerWindow = this;
      this.InitializeComponent();
      this.Closing += (CancelEventHandler) ((o, e) => containerWindow.ClosingContainerWindow(o, e, control));
      if (radius != -1.0)
        this.CustomCornerRadius = new CornerRadius(radius);
      if (brush != null)
        this.CustomBorderBrush = brush;
      if (!isWindowTransparent)
      {
        this.SetShadowBorder();
        this.SetOuterBorder();
        this.SetMaskGrid();
      }
      if (autoHeight)
      {
        this.Width = width + (isWindowTransparent ? 0.0 : 64.0);
        this.SizeToContent = SizeToContent.Height;
      }
      else
      {
        this.Width = width + (isWindowTransparent ? 0.0 : 64.0);
        this.Height = height + (isWindowTransparent ? 0.0 : 64.0);
      }
      this.Owner = (Window) window;
      if (window == null)
        return;
      if (window.mDMMRecommendedWindow != null && window.mDMMRecommendedWindow.Visibility == Visibility.Visible && window.IsUIInPortraitMode)
      {
        double num1 = (window.Width + window.mDMMRecommendedWindow.Width - this.Width) / 2.0 + window.Left;
        double num2 = (window.Height - this.Height) / 2.0 + window.Top;
        double num3 = num1 + this.Width;
        double num4 = num2 + this.Height;
        if (num1 > 0.0 && num3 < SystemParameters.PrimaryScreenWidth && (num2 > 0.0 && num4 < SystemParameters.PrimaryScreenHeight))
        {
          this.Left = num1;
          this.Top = num2;
        }
        else
          this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
      }
      else if (window.WindowState == WindowState.Minimized)
        this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
      else
        this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
      this.ContentGrid.Children.Add((UIElement) control);
      if (!isShow)
        return;
      if (window != null)
      {
        window.ShowDimOverlay((IDimOverlayControl) null);
        this.Owner = (Window) window.mDimOverlay;
      }
      this.ShowDialog();
      window?.HideDimOverlay();
    }

    private void ClosingContainerWindow(object _1, CancelEventArgs _2, UserControl control)
    {
      if (control is IDimOverlayControl dimOverlayControl)
        dimOverlayControl.Close();
      if (control == null)
        return;
      this.ContentGrid.Children.Remove((UIElement) control);
    }

    private void SetShadowBorder()
    {
      this.mShadowBorder.SnapsToDevicePixels = true;
      this.mShadowBorder.BorderThickness = new Thickness(1.0);
      this.mShadowBorder.Margin = new Thickness(30.0);
      this.mShadowBorder.SetValue(RenderOptions.EdgeModeProperty, (object) EdgeMode.Aliased);
      if (this.IsCustomCornerRadius)
        this.mShadowBorder.CornerRadius = this.CustomCornerRadius;
      else
        BlueStacksUIBinding.BindCornerRadius((DependencyObject) this.mShadowBorder, Border.CornerRadiusProperty, "SettingsWindowRadius");
      DropShadowEffect dropShadowEffect = new DropShadowEffect();
      BlueStacksUIBinding.BindColor((DependencyObject) dropShadowEffect, DropShadowEffect.ColorProperty, "PopupShadowColor");
      dropShadowEffect.Direction = 270.0;
      dropShadowEffect.ShadowDepth = 0.0;
      dropShadowEffect.BlurRadius = 15.0;
      dropShadowEffect.Opacity = 0.7;
      this.mShadowBorder.Effect = (Effect) dropShadowEffect;
    }

    private void SetOuterBorder()
    {
      this.mOuterBorder.BorderThickness = new Thickness(1.0);
      BlueStacksUIBinding.BindColor((DependencyObject) this.mOuterBorder, Control.BackgroundProperty, "ContextMenuItemBackgroundColor");
      if (this.IsCustomBorderBrush)
        this.mOuterBorder.BorderBrush = this.CustomBorderBrush;
      else
        BlueStacksUIBinding.BindColor((DependencyObject) this.mOuterBorder, Control.BorderBrushProperty, "PopupBorderBrush");
      if (this.IsCustomCornerRadius)
        this.mOuterBorder.CornerRadius = this.CustomCornerRadius;
      else
        BlueStacksUIBinding.BindCornerRadius((DependencyObject) this.mOuterBorder, Border.CornerRadiusProperty, "SettingsWindowRadius");
    }

    private void SetMaskGrid()
    {
      this.mMaskBorder.SnapsToDevicePixels = true;
      this.mMaskBorder.SetValue(RenderOptions.EdgeModeProperty, (object) EdgeMode.Aliased);
      BlueStacksUIBinding.BindColor((DependencyObject) this.mMaskBorder, Control.BackgroundProperty, "ContextMenuItemBackgroundColor");
      if (this.IsCustomCornerRadius)
        this.mMaskBorder.CornerRadius = this.CustomCornerRadius;
      else
        BlueStacksUIBinding.BindCornerRadius((DependencyObject) this.mMaskBorder, Border.CornerRadiusProperty, "SettingsWindowRadius");
      VisualBrush visualBrush = new VisualBrush((Visual) this.mMaskBorder);
      visualBrush.Stretch = Stretch.None;
      this.mMainGrid.OpacityMask = (Brush) visualBrush;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/containerwindow.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.mShadowBorder = (Border) target;
          break;
        case 2:
          this.mOuterBorder = (Border) target;
          break;
        case 3:
          this.mMainGrid = (Grid) target;
          break;
        case 4:
          this.mMaskBorder = (Border) target;
          break;
        case 5:
          this.ContentGrid = (Grid) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
