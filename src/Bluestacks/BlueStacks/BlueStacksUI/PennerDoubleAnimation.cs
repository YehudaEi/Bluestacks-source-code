// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.PennerDoubleAnimation
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Media.Animation;

namespace BlueStacks.BlueStacksUI
{
  public class PennerDoubleAnimation : DoubleAnimationBase
  {
    public static readonly DependencyProperty EquationProperty = DependencyProperty.Register(nameof (Equation), typeof (PennerDoubleAnimation.Equations), typeof (PennerDoubleAnimation), new PropertyMetadata((object) PennerDoubleAnimation.Equations.Linear, new PropertyChangedCallback(PennerDoubleAnimation.HandleEquationChanged)));
    public static readonly DependencyProperty FromProperty = DependencyProperty.Register(nameof (From), typeof (double), typeof (PennerDoubleAnimation), new PropertyMetadata((object) 0.0));
    public static readonly DependencyProperty ToProperty = DependencyProperty.Register(nameof (To), typeof (double), typeof (PennerDoubleAnimation), new PropertyMetadata((object) 0.0));
    private MethodInfo _EasingMethod;

    public PennerDoubleAnimation()
    {
    }

    public PennerDoubleAnimation(PennerDoubleAnimation.Equations type, double from, double to)
    {
      this.Equation = type;
      this.From = from;
      this.To = to;
    }

    public PennerDoubleAnimation(
      PennerDoubleAnimation.Equations type,
      double from,
      double to,
      Duration duration)
    {
      this.Equation = type;
      this.From = from;
      this.To = to;
      this.Duration = duration;
    }

    protected override double GetCurrentValueCore(
      double startValue,
      double targetValue,
      AnimationClock clock)
    {
      try
      {
        return (double) this._EasingMethod.Invoke((object) this, new object[4]
        {
          (object) clock?.CurrentTime.Value.TotalSeconds,
          (object) this.From,
          (object) (this.To - this.From),
          (object) this.Duration.TimeSpan.TotalSeconds
        });
      }
      catch
      {
        return this.From;
      }
    }

    protected override Freezable CreateInstanceCore()
    {
      return (Freezable) new PennerDoubleAnimation();
    }

    public static double Linear(double t, double b, double c, double d)
    {
      return c * t / d + b;
    }

    public static double ExpoEaseOut(double t, double b, double c, double d)
    {
      return t != d ? c * (-Math.Pow(2.0, -10.0 * t / d) + 1.0) + b : b + c;
    }

    public static double ExpoEaseIn(double t, double b, double c, double d)
    {
      return t != 0.0 ? c * Math.Pow(2.0, 10.0 * (t / d - 1.0)) + b : b;
    }

    public static double ExpoEaseInOut(double t, double b, double c, double d)
    {
      if (t == 0.0)
        return b;
      if (t == d)
        return b + c;
      return (t /= d / 2.0) < 1.0 ? c / 2.0 * Math.Pow(2.0, 10.0 * (t - 1.0)) + b : c / 2.0 * (-Math.Pow(2.0, -10.0 * --t) + 2.0) + b;
    }

    public static double ExpoEaseOutIn(double t, double b, double c, double d)
    {
      return t < d / 2.0 ? PennerDoubleAnimation.ExpoEaseOut(t * 2.0, b, c / 2.0, d) : PennerDoubleAnimation.ExpoEaseIn(t * 2.0 - d, b + c / 2.0, c / 2.0, d);
    }

    public static double CircEaseOut(double t, double b, double c, double d)
    {
      return c * Math.Sqrt(1.0 - (t = t / d - 1.0) * t) + b;
    }

    public static double CircEaseIn(double t, double b, double c, double d)
    {
      return -c * (Math.Sqrt(1.0 - (t /= d) * t) - 1.0) + b;
    }

    public static double CircEaseInOut(double t, double b, double c, double d)
    {
      return (t /= d / 2.0) < 1.0 ? -c / 2.0 * (Math.Sqrt(1.0 - t * t) - 1.0) + b : c / 2.0 * (Math.Sqrt(1.0 - (t -= 2.0) * t) + 1.0) + b;
    }

    public static double CircEaseOutIn(double t, double b, double c, double d)
    {
      return t < d / 2.0 ? PennerDoubleAnimation.CircEaseOut(t * 2.0, b, c / 2.0, d) : PennerDoubleAnimation.CircEaseIn(t * 2.0 - d, b + c / 2.0, c / 2.0, d);
    }

    public static double QuadEaseOut(double t, double b, double c, double d)
    {
      return -c * (t /= d) * (t - 2.0) + b;
    }

    public static double QuadEaseIn(double t, double b, double c, double d)
    {
      return c * (t /= d) * t + b;
    }

    public static double QuadEaseInOut(double t, double b, double c, double d)
    {
      return (t /= d / 2.0) < 1.0 ? c / 2.0 * t * t + b : -c / 2.0 * (--t * (t - 2.0) - 1.0) + b;
    }

    public static double QuadEaseOutIn(double t, double b, double c, double d)
    {
      return t < d / 2.0 ? PennerDoubleAnimation.QuadEaseOut(t * 2.0, b, c / 2.0, d) : PennerDoubleAnimation.QuadEaseIn(t * 2.0 - d, b + c / 2.0, c / 2.0, d);
    }

    public static double SineEaseOut(double t, double b, double c, double d)
    {
      return c * Math.Sin(t / d * (Math.PI / 2.0)) + b;
    }

    public static double SineEaseIn(double t, double b, double c, double d)
    {
      return -c * Math.Cos(t / d * (Math.PI / 2.0)) + c + b;
    }

    public static double SineEaseInOut(double t, double b, double c, double d)
    {
      return (t /= d / 2.0) < 1.0 ? c / 2.0 * Math.Sin(Math.PI * t / 2.0) + b : -c / 2.0 * (Math.Cos(Math.PI * --t / 2.0) - 2.0) + b;
    }

    public static double SineEaseOutIn(double t, double b, double c, double d)
    {
      return t < d / 2.0 ? PennerDoubleAnimation.SineEaseOut(t * 2.0, b, c / 2.0, d) : PennerDoubleAnimation.SineEaseIn(t * 2.0 - d, b + c / 2.0, c / 2.0, d);
    }

    public static double CubicEaseOut(double t, double b, double c, double d)
    {
      return c * ((t = t / d - 1.0) * t * t + 1.0) + b;
    }

    public static double CubicEaseIn(double t, double b, double c, double d)
    {
      return c * (t /= d) * t * t + b;
    }

    public static double CubicEaseInOut(double t, double b, double c, double d)
    {
      return (t /= d / 2.0) < 1.0 ? c / 2.0 * t * t * t + b : c / 2.0 * ((t -= 2.0) * t * t + 2.0) + b;
    }

    public static double CubicEaseOutIn(double t, double b, double c, double d)
    {
      return t < d / 2.0 ? PennerDoubleAnimation.CubicEaseOut(t * 2.0, b, c / 2.0, d) : PennerDoubleAnimation.CubicEaseIn(t * 2.0 - d, b + c / 2.0, c / 2.0, d);
    }

    public static double QuartEaseOut(double t, double b, double c, double d)
    {
      return -c * ((t = t / d - 1.0) * t * t * t - 1.0) + b;
    }

    public static double QuartEaseIn(double t, double b, double c, double d)
    {
      return c * (t /= d) * t * t * t + b;
    }

    public static double QuartEaseInOut(double t, double b, double c, double d)
    {
      return (t /= d / 2.0) < 1.0 ? c / 2.0 * t * t * t * t + b : -c / 2.0 * ((t -= 2.0) * t * t * t - 2.0) + b;
    }

    public static double QuartEaseOutIn(double t, double b, double c, double d)
    {
      return t < d / 2.0 ? PennerDoubleAnimation.QuartEaseOut(t * 2.0, b, c / 2.0, d) : PennerDoubleAnimation.QuartEaseIn(t * 2.0 - d, b + c / 2.0, c / 2.0, d);
    }

    public static double QuintEaseOut(double t, double b, double c, double d)
    {
      return c * ((t = t / d - 1.0) * t * t * t * t + 1.0) + b;
    }

    public static double QuintEaseIn(double t, double b, double c, double d)
    {
      return c * (t /= d) * t * t * t * t + b;
    }

    public static double QuintEaseInOut(double t, double b, double c, double d)
    {
      return (t /= d / 2.0) < 1.0 ? c / 2.0 * t * t * t * t * t + b : c / 2.0 * ((t -= 2.0) * t * t * t * t + 2.0) + b;
    }

    public static double QuintEaseOutIn(double t, double b, double c, double d)
    {
      return t < d / 2.0 ? PennerDoubleAnimation.QuintEaseOut(t * 2.0, b, c / 2.0, d) : PennerDoubleAnimation.QuintEaseIn(t * 2.0 - d, b + c / 2.0, c / 2.0, d);
    }

    public static double ElasticEaseOut(double t, double b, double c, double d)
    {
      if ((t /= d) == 1.0)
        return b + c;
      double num1 = d * 0.3;
      double num2 = num1 / 4.0;
      return c * Math.Pow(2.0, -10.0 * t) * Math.Sin((t * d - num2) * (2.0 * Math.PI) / num1) + c + b;
    }

    public static double ElasticEaseIn(double t, double b, double c, double d)
    {
      if ((t /= d) == 1.0)
        return b + c;
      double num1 = d * 0.3;
      double num2 = num1 / 4.0;
      return -(c * Math.Pow(2.0, 10.0 * --t) * Math.Sin((t * d - num2) * (2.0 * Math.PI) / num1)) + b;
    }

    public static double ElasticEaseInOut(double t, double b, double c, double d)
    {
      if ((t /= d / 2.0) == 2.0)
        return b + c;
      double num1 = d * (9.0 / 20.0);
      double num2 = num1 / 4.0;
      return t < 1.0 ? -0.5 * (c * Math.Pow(2.0, 10.0 * --t) * Math.Sin((t * d - num2) * (2.0 * Math.PI) / num1)) + b : c * Math.Pow(2.0, -10.0 * --t) * Math.Sin((t * d - num2) * (2.0 * Math.PI) / num1) * 0.5 + c + b;
    }

    public static double ElasticEaseOutIn(double t, double b, double c, double d)
    {
      return t < d / 2.0 ? PennerDoubleAnimation.ElasticEaseOut(t * 2.0, b, c / 2.0, d) : PennerDoubleAnimation.ElasticEaseIn(t * 2.0 - d, b + c / 2.0, c / 2.0, d);
    }

    public static double BounceEaseOut(double t, double b, double c, double d)
    {
      if ((t /= d) < 4.0 / 11.0)
        return c * (121.0 / 16.0 * t * t) + b;
      if (t < 8.0 / 11.0)
        return c * (121.0 / 16.0 * (t -= 6.0 / 11.0) * t + 0.75) + b;
      return t < 10.0 / 11.0 ? c * (121.0 / 16.0 * (t -= 9.0 / 11.0) * t + 15.0 / 16.0) + b : c * (121.0 / 16.0 * (t -= 21.0 / 22.0) * t + 63.0 / 64.0) + b;
    }

    public static double BounceEaseIn(double t, double b, double c, double d)
    {
      return c - PennerDoubleAnimation.BounceEaseOut(d - t, 0.0, c, d) + b;
    }

    public static double BounceEaseInOut(double t, double b, double c, double d)
    {
      return t < d / 2.0 ? PennerDoubleAnimation.BounceEaseIn(t * 2.0, 0.0, c, d) * 0.5 + b : PennerDoubleAnimation.BounceEaseOut(t * 2.0 - d, 0.0, c, d) * 0.5 + c * 0.5 + b;
    }

    public static double BounceEaseOutIn(double t, double b, double c, double d)
    {
      return t < d / 2.0 ? PennerDoubleAnimation.BounceEaseOut(t * 2.0, b, c / 2.0, d) : PennerDoubleAnimation.BounceEaseIn(t * 2.0 - d, b + c / 2.0, c / 2.0, d);
    }

    public static double BackEaseOut(double t, double b, double c, double d)
    {
      return c * ((t = t / d - 1.0) * t * (2.70158 * t + 1.70158) + 1.0) + b;
    }

    public static double BackEaseIn(double t, double b, double c, double d)
    {
      return c * (t /= d) * t * (2.70158 * t - 1.70158) + b;
    }

    public static double BackEaseInOut(double t, double b, double c, double d)
    {
      double num1 = 1.70158;
      double num2;
      double num3;
      return (t /= d / 2.0) < 1.0 ? c / 2.0 * (t * t * (((num2 = num1 * 1.525) + 1.0) * t - num2)) + b : c / 2.0 * ((t -= 2.0) * t * (((num3 = num1 * 1.525) + 1.0) * t + num3) + 2.0) + b;
    }

    public static double BackEaseOutIn(double t, double b, double c, double d)
    {
      return t < d / 2.0 ? PennerDoubleAnimation.BackEaseOut(t * 2.0, b, c / 2.0, d) : PennerDoubleAnimation.BackEaseIn(t * 2.0 - d, b + c / 2.0, c / 2.0, d);
    }

    private static void HandleEquationChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      (sender as PennerDoubleAnimation)._EasingMethod = typeof (PennerDoubleAnimation).GetMethod(e.NewValue.ToString());
    }

    [TypeConverter(typeof (PennerDoubleAnimationTypeConverter))]
    public PennerDoubleAnimation.Equations Equation
    {
      get
      {
        return (PennerDoubleAnimation.Equations) this.GetValue(PennerDoubleAnimation.EquationProperty);
      }
      set
      {
        this.SetValue(PennerDoubleAnimation.EquationProperty, (object) value);
        this._EasingMethod = this.GetType().GetMethod(value.ToString());
      }
    }

    public double From
    {
      get
      {
        return (double) this.GetValue(PennerDoubleAnimation.FromProperty);
      }
      set
      {
        this.SetValue(PennerDoubleAnimation.FromProperty, (object) value);
      }
    }

    public double To
    {
      get
      {
        return (double) this.GetValue(PennerDoubleAnimation.ToProperty);
      }
      set
      {
        this.SetValue(PennerDoubleAnimation.ToProperty, (object) value);
      }
    }

    public enum Equations
    {
      Linear,
      QuadEaseOut,
      QuadEaseIn,
      QuadEaseInOut,
      QuadEaseOutIn,
      ExpoEaseOut,
      ExpoEaseIn,
      ExpoEaseInOut,
      ExpoEaseOutIn,
      CubicEaseOut,
      CubicEaseIn,
      CubicEaseInOut,
      CubicEaseOutIn,
      QuartEaseOut,
      QuartEaseIn,
      QuartEaseInOut,
      QuartEaseOutIn,
      QuintEaseOut,
      QuintEaseIn,
      QuintEaseInOut,
      QuintEaseOutIn,
      CircEaseOut,
      CircEaseIn,
      CircEaseInOut,
      CircEaseOutIn,
      SineEaseOut,
      SineEaseIn,
      SineEaseInOut,
      SineEaseOutIn,
      ElasticEaseOut,
      ElasticEaseIn,
      ElasticEaseInOut,
      ElasticEaseOutIn,
      BounceEaseOut,
      BounceEaseIn,
      BounceEaseInOut,
      BounceEaseOutIn,
      BackEaseOut,
      BackEaseIn,
      BackEaseInOut,
      BackEaseOutIn,
    }
  }
}
