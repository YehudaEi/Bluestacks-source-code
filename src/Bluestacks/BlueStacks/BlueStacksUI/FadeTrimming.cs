// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.FadeTrimming
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace BlueStacks.BlueStacksUI
{
  public static class FadeTrimming
  {
    public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof (bool), typeof (FadeTrimming), new PropertyMetadata((object) false, new PropertyChangedCallback(FadeTrimming.HandleIsEnabledChanged)));
    private static readonly DependencyProperty FaderProperty = DependencyProperty.RegisterAttached("Fader", typeof (FadeTrimming.Fader), typeof (FadeTrimming), new PropertyMetadata((PropertyChangedCallback) null));
    public static readonly DependencyProperty IsVerticalFadingEnabledProperty = DependencyProperty.RegisterAttached(nameof (IsVerticalFadingEnabledProperty), typeof (bool), typeof (FadeTrimming), new PropertyMetadata((object) false, new PropertyChangedCallback(FadeTrimming.HandleVerticalFadingEnabled)));
    private const double Epsilon = 1E-05;
    private const double FadeWidth = 10.0;
    private const double FadeHeight = 20.0;

    public static bool GetIsEnabled(DependencyObject obj)
    {
      return (bool) obj?.GetValue(FadeTrimming.IsEnabledProperty);
    }

    public static void SetIsEnabled(DependencyObject obj, bool value)
    {
      obj?.SetValue(FadeTrimming.IsEnabledProperty, (object) value);
    }

    public static void SetIsVerticalFadingEnabled(DependencyObject obj, bool value)
    {
      obj?.SetValue(FadeTrimming.IsVerticalFadingEnabledProperty, (object) value);
    }

    private static FadeTrimming.Fader GetFader(DependencyObject obj)
    {
      return (FadeTrimming.Fader) obj.GetValue(FadeTrimming.FaderProperty);
    }

    private static void SetFader(DependencyObject obj, FadeTrimming.Fader value)
    {
      obj.SetValue(FadeTrimming.FaderProperty, (object) value);
    }

    private static void HandleVerticalFadingEnabled(
      DependencyObject source,
      DependencyPropertyChangedEventArgs e)
    {
      if (!(source is TextBlock textBlock))
        return;
      FadeTrimming.GetFader((DependencyObject) textBlock)?.ToggleVerticalFading((bool) e.NewValue);
    }

    private static void HandleIsEnabledChanged(
      DependencyObject source,
      DependencyPropertyChangedEventArgs e)
    {
      if (!(source is TextBlock textBlock))
        return;
      if ((bool) e.OldValue)
      {
        FadeTrimming.Fader fader = FadeTrimming.GetFader((DependencyObject) textBlock);
        if (fader != null)
        {
          fader.Detach();
          FadeTrimming.SetFader((DependencyObject) textBlock, (FadeTrimming.Fader) null);
        }
        textBlock.Loaded -= new RoutedEventHandler(FadeTrimming.HandleTextBlockLoaded);
        textBlock.Unloaded -= new RoutedEventHandler(FadeTrimming.HandleTextBlockUnloaded);
      }
      if (!(bool) e.NewValue)
        return;
      textBlock.Loaded += new RoutedEventHandler(FadeTrimming.HandleTextBlockLoaded);
      textBlock.Unloaded += new RoutedEventHandler(FadeTrimming.HandleTextBlockUnloaded);
      FadeTrimming.Fader fader1 = new FadeTrimming.Fader(textBlock);
      FadeTrimming.SetFader((DependencyObject) textBlock, fader1);
      fader1.Attach();
    }

    private static void HandleTextBlockUnloaded(object sender, RoutedEventArgs e)
    {
      if (!(sender is DependencyObject dependencyObject))
        return;
      FadeTrimming.GetFader(dependencyObject)?.Detach();
    }

    private static void HandleTextBlockLoaded(object sender, RoutedEventArgs e)
    {
      if (!(sender is DependencyObject dependencyObject))
        return;
      FadeTrimming.GetFader(dependencyObject)?.Attach();
    }

    private static bool HorizontalBrushNeedsUpdating(LinearGradientBrush brush, double visibleWidth)
    {
      return brush.EndPoint.X < visibleWidth - 1E-05 || brush.EndPoint.X > visibleWidth + 1E-05;
    }

    private static bool VerticalBrushNeedsUpdating(LinearGradientBrush brush, double visibleHeight)
    {
      return brush.EndPoint.Y < visibleHeight - 1E-05 || brush.EndPoint.Y > visibleHeight + 1E-05;
    }

    private class Fader
    {
      private readonly TextBlock _textBlock;
      private bool _isAttached;
      private LinearGradientBrush _brush;
      private Brush _opacityMask;
      private bool _isClipped;
      private bool _verticalFadingEnabled;

      public Fader(TextBlock textBlock)
      {
        this._textBlock = textBlock;
      }

      public void Attach()
      {
        if (!(VisualTreeHelper.GetParent((DependencyObject) this._textBlock) is FrameworkElement parent) || this._isAttached)
          return;
        parent.SizeChanged += new SizeChangedEventHandler(this.UpdateForegroundBrush);
        this._textBlock.SizeChanged += new SizeChangedEventHandler(this.UpdateForegroundBrush);
        this._opacityMask = this._textBlock.OpacityMask;
        if (this._verticalFadingEnabled || this._textBlock.TextWrapping == TextWrapping.NoWrap)
          this._textBlock.TextTrimming = TextTrimming.None;
        this.UpdateForegroundBrush((object) this._textBlock, EventArgs.Empty);
        this._isAttached = true;
      }

      public void Detach()
      {
        this._textBlock.SizeChanged -= new SizeChangedEventHandler(this.UpdateForegroundBrush);
        if (VisualTreeHelper.GetParent((DependencyObject) this._textBlock) is FrameworkElement parent)
          parent.SizeChanged -= new SizeChangedEventHandler(this.UpdateForegroundBrush);
        this._textBlock.OpacityMask = this._opacityMask;
        this._isAttached = false;
      }

      public void ToggleVerticalFading(bool newValue)
      {
        this._verticalFadingEnabled = newValue;
        this.UpdateForegroundBrush((object) this._textBlock, EventArgs.Empty);
      }

      private void UpdateForegroundBrush(object sender, EventArgs e)
      {
        Geometry layoutClip = LayoutInformation.GetLayoutClip((FrameworkElement) this._textBlock);
        Rect bounds;
        int num;
        if (layoutClip != null)
        {
          if (this._textBlock.TextWrapping == TextWrapping.NoWrap)
          {
            bounds = layoutClip.Bounds;
            if (bounds.Width > 0.0)
            {
              bounds = layoutClip.Bounds;
              if (bounds.Width < this._textBlock.ActualWidth)
              {
                num = 1;
                goto label_10;
              }
            }
          }
          if (this._verticalFadingEnabled && this._textBlock.TextWrapping == TextWrapping.Wrap)
          {
            bounds = layoutClip.Bounds;
            if (bounds.Height > 0.0)
            {
              bounds = layoutClip.Bounds;
              num = bounds.Height < this._textBlock.ActualHeight ? 1 : 0;
              goto label_10;
            }
          }
          num = 0;
        }
        else
          num = 0;
label_10:
        bool flag1 = num != 0;
        if (this._isClipped && !flag1)
        {
          this._textBlock.OpacityMask = this._opacityMask;
          this._brush = (LinearGradientBrush) null;
          this._isClipped = false;
        }
        if (!flag1)
          return;
        bounds = layoutClip.Bounds;
        double width = bounds.Width;
        bounds = layoutClip.Bounds;
        double height = bounds.Height;
        bool flag2 = this._textBlock.TextWrapping == TextWrapping.Wrap;
        if (this._brush == null)
        {
          this._brush = flag2 ? this.GetVerticalClipBrush(height) : this.GetHorizontalClipBrush(width);
          this._textBlock.OpacityMask = (Brush) this._brush;
        }
        else if (flag2 && FadeTrimming.VerticalBrushNeedsUpdating(this._brush, height))
        {
          this._brush.EndPoint = new Point(0.0, height);
          this._brush.GradientStops[1].Offset = (height - 20.0) / height;
        }
        else if (!flag2 && FadeTrimming.HorizontalBrushNeedsUpdating(this._brush, width))
        {
          this._brush.EndPoint = new Point(width, 0.0);
          this._brush.GradientStops[1].Offset = (width - 10.0) / width;
        }
        this._isClipped = true;
      }

      private LinearGradientBrush GetHorizontalClipBrush(double visibleWidth)
      {
        LinearGradientBrush linearGradientBrush = new LinearGradientBrush();
        linearGradientBrush.MappingMode = BrushMappingMode.Absolute;
        linearGradientBrush.StartPoint = new Point(0.0, 0.0);
        linearGradientBrush.EndPoint = new Point(visibleWidth, 0.0);
        linearGradientBrush.GradientStops.Add(new GradientStop()
        {
          Color = Colors.Black,
          Offset = 0.0
        });
        linearGradientBrush.GradientStops.Add(new GradientStop()
        {
          Color = Colors.Black,
          Offset = (visibleWidth - 10.0) / visibleWidth
        });
        linearGradientBrush.GradientStops.Add(new GradientStop()
        {
          Color = Colors.Transparent,
          Offset = 1.0
        });
        return linearGradientBrush;
      }

      private LinearGradientBrush GetVerticalClipBrush(double visibleHeight)
      {
        LinearGradientBrush linearGradientBrush = new LinearGradientBrush();
        linearGradientBrush.MappingMode = BrushMappingMode.Absolute;
        linearGradientBrush.StartPoint = new Point(0.0, 0.0);
        linearGradientBrush.EndPoint = new Point(0.0, visibleHeight);
        linearGradientBrush.GradientStops.Add(new GradientStop()
        {
          Color = Colors.Black,
          Offset = 0.0
        });
        linearGradientBrush.GradientStops.Add(new GradientStop()
        {
          Color = Colors.Black,
          Offset = (visibleHeight - 20.0) / visibleHeight
        });
        linearGradientBrush.GradientStops.Add(new GradientStop()
        {
          Color = Colors.Transparent,
          Offset = 1.0
        });
        return linearGradientBrush;
      }
    }
  }
}
