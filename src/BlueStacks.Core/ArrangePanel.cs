// Decompiled with JetBrains decompiler
// Type: BlueStacks.Core.ArrangePanel
// Assembly: BlueStacks.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C36AABCB-E7F4-4E86-A72F-981C56431F94
// Assembly location: C:\Program Files\BlueStacks\BlueStacks.Core.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace BlueStacks.Core
{
  public class ArrangePanel : Panel
  {
    public static readonly DependencyProperty OrderProperty = DependencyProperty.RegisterAttached("Order", typeof (int), typeof (ArrangePanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) -1, FrameworkPropertyMetadataOptions.AffectsParentMeasure));
    public static readonly DependencyProperty PositionProperty = DependencyProperty.RegisterAttached("Position", typeof (Rect), typeof (ArrangePanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) new Rect(double.NaN, double.NaN, double.NaN, double.NaN), FrameworkPropertyMetadataOptions.AffectsParentArrange));
    public static readonly DependencyProperty DesiredPositionProperty = DependencyProperty.RegisterAttached("DesiredPosition", typeof (Rect), typeof (ArrangePanel), (PropertyMetadata) new FrameworkPropertyMetadata((object) new Rect(double.NaN, double.NaN, double.NaN, double.NaN), new PropertyChangedCallback(ArrangePanel.OnDesiredPositionChanged)));
    private readonly ILayoutStrategy _strategy = (ILayoutStrategy) new TableLayoutStrategy();
    private UIElement _draggingObject;
    private Vector _delta;
    private Point _startPosition;
    private Size[] _measures;

    public event Action ReOrderedEvent;

    public static int GetOrder(DependencyObject obj)
    {
      return (int) obj?.GetValue(ArrangePanel.OrderProperty);
    }

    public static void SetOrder(DependencyObject obj, int value)
    {
      obj?.SetValue(ArrangePanel.OrderProperty, (object) value);
    }

    public static Rect GetPosition(DependencyObject obj)
    {
      return (Rect) obj?.GetValue(ArrangePanel.PositionProperty);
    }

    public static void SetPosition(DependencyObject obj, Rect value)
    {
      obj?.SetValue(ArrangePanel.PositionProperty, (object) value);
    }

    public static Rect GetDesiredPosition(DependencyObject obj)
    {
      return (Rect) obj?.GetValue(ArrangePanel.DesiredPositionProperty);
    }

    public static void SetDesiredPosition(DependencyObject obj, Rect value)
    {
      obj?.SetValue(ArrangePanel.DesiredPositionProperty, (object) value);
    }

    private static void OnDesiredPositionChanged(
      DependencyObject d,
      DependencyPropertyChangedEventArgs e)
    {
      Rect newValue = (Rect) e.NewValue;
      ArrangePanel.AnimateToPosition(d, newValue);
    }

    protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
    {
      this.StartReordering((MouseEventArgs) e);
    }

    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
    {
      this.StopReordering();
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
      if (this._draggingObject == null || e == null)
        return;
      if (e.LeftButton == MouseButtonState.Released)
        this.StopReordering();
      else
        this.DoReordering(e);
    }

    protected override void OnMouseLeave(MouseEventArgs e)
    {
      this.StopReordering();
      base.OnMouseLeave(e);
    }

    private void StartReordering(MouseEventArgs e)
    {
      if (e == null || !(e.OriginalSource is UIElement originalSource))
        return;
      this._startPosition = e.GetPosition((IInputElement) this);
      this._draggingObject = this.GetMyChildOfUiElement(originalSource);
      this._draggingObject.SetValue(Panel.ZIndexProperty, (object) 100);
      Rect position = ArrangePanel.GetPosition((DependencyObject) this._draggingObject);
      this._delta = position.TopLeft - this._startPosition;
      this._draggingObject.BeginAnimation(ArrangePanel.PositionProperty, (AnimationTimeline) null);
      ArrangePanel.SetPosition((DependencyObject) this._draggingObject, position);
    }

    private void DoReordering(MouseEventArgs e)
    {
      e.Handled = true;
      Point position = e.GetPosition((IInputElement) this);
      ArrangePanel.SetOrder((DependencyObject) this._draggingObject, this._strategy.GetIndex(position));
      ArrangePanel.SetPosition((DependencyObject) this._draggingObject, new Rect(position + this._delta, ArrangePanel.GetPosition((DependencyObject) this._draggingObject).Size));
    }

    private void StopReordering()
    {
      if (this._draggingObject == null)
        return;
      this._draggingObject.ClearValue(Panel.ZIndexProperty);
      this.InvalidateMeasure();
      ArrangePanel.AnimateToPosition((DependencyObject) this._draggingObject, ArrangePanel.GetDesiredPosition((DependencyObject) this._draggingObject));
      this._draggingObject = (UIElement) null;
      Action reOrderedEvent = this.ReOrderedEvent;
      if (reOrderedEvent == null)
        return;
      reOrderedEvent();
    }

    private UIElement GetMyChildOfUiElement(UIElement element)
    {
      if (VisualTreeHelper.GetParent((DependencyObject) element) is UIElement uiElement)
      {
        for (; uiElement != null && uiElement != this; uiElement = VisualTreeHelper.GetParent((DependencyObject) element) is UIElement parent ? parent : (UIElement) null)
          element = uiElement;
      }
      return element;
    }

    protected override Size MeasureOverride(Size availableSize)
    {
      this.InitializeEmptyOrder();
      if (this._draggingObject != null)
        this.ReorderOthers();
      Size[] sizes = this.MeasureChildren();
      this._strategy.Calculate(availableSize, sizes);
      int index = -1;
      foreach (UIElement uiElement in (IEnumerable<UIElement>) this.Children.OfType<UIElement>().OrderBy<UIElement, int>(new Func<UIElement, int>(ArrangePanel.GetOrder)))
      {
        ++index;
        if (uiElement != this._draggingObject)
          ArrangePanel.SetDesiredPosition((DependencyObject) uiElement, this._strategy.GetPosition(index));
      }
      return this._strategy.ResultSize;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
      foreach (UIElement uiElement in (IEnumerable<UIElement>) this.Children.OfType<UIElement>().OrderBy<UIElement, int>(new Func<UIElement, int>(ArrangePanel.GetOrder)))
      {
        Rect finalRect = ArrangePanel.GetPosition((DependencyObject) uiElement);
        if (double.IsNaN(finalRect.Top))
          finalRect = ArrangePanel.GetDesiredPosition((DependencyObject) uiElement);
        uiElement.Arrange(finalRect);
      }
      return this._strategy.ResultSize;
    }

    private Size[] MeasureChildren()
    {
      if (this._measures == null || this.Children.Count != this._measures.Length)
      {
        this._measures = new Size[this.Children.Count];
        Size availableSize = new Size(double.PositiveInfinity, double.PositiveInfinity);
        foreach (UIElement child in this.Children)
          child.Measure(availableSize);
        int index = 0;
        foreach (Size size in this.Children.OfType<UIElement>().OrderBy<UIElement, int>(new Func<UIElement, int>(ArrangePanel.GetOrder)).Select<UIElement, Size>((Func<UIElement, Size>) (ch => ch.DesiredSize)))
        {
          this._measures[index] = size;
          ++index;
        }
      }
      return this._measures;
    }

    private void ReorderOthers()
    {
      int order1 = ArrangePanel.GetOrder((DependencyObject) this._draggingObject);
      int num = 0;
      foreach (UIElement uiElement in (IEnumerable<UIElement>) this.Children.OfType<UIElement>().OrderBy<UIElement, int>(new Func<UIElement, int>(ArrangePanel.GetOrder)))
      {
        if (num == order1)
          ++num;
        if (uiElement != this._draggingObject)
        {
          int order2 = ArrangePanel.GetOrder((DependencyObject) uiElement);
          if (num != order2)
            ArrangePanel.SetOrder((DependencyObject) uiElement, num);
          ++num;
        }
      }
    }

    private void InitializeEmptyOrder()
    {
      int num = this.Children.OfType<UIElement>().Max<UIElement>((Func<UIElement, int>) (ch => ArrangePanel.GetOrder((DependencyObject) ch))) + 1;
      foreach (DependencyObject dependencyObject in this.Children.OfType<UIElement>().Where<UIElement>((Func<UIElement, bool>) (child => ArrangePanel.GetOrder((DependencyObject) child) == -1)))
      {
        ArrangePanel.SetOrder(dependencyObject, num);
        ++num;
      }
    }

    private static void AnimateToPosition(DependencyObject d, Rect desiredPosition)
    {
      Rect position = ArrangePanel.GetPosition(d);
      if (double.IsNaN(position.X))
      {
        ArrangePanel.SetPosition(d, desiredPosition);
      }
      else
      {
        Vector vector = desiredPosition.TopLeft - position.TopLeft;
        double length1 = vector.Length;
        vector = desiredPosition.BottomRight - position.BottomRight;
        double length2 = vector.Length;
        TimeSpan timeSpan = TimeSpan.FromMilliseconds(Math.Max(length1, length2) * 1.0);
        RectAnimation rectAnimation1 = new RectAnimation(position, desiredPosition, new Duration(timeSpan));
        rectAnimation1.DecelerationRatio = 1.0;
        RectAnimation rectAnimation2 = rectAnimation1;
        if (!(d is UIElement uiElement))
          return;
        uiElement.BeginAnimation(ArrangePanel.PositionProperty, (AnimationTimeline) rectAnimation2);
      }
    }
  }
}
