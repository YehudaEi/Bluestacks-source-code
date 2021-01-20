// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.Animator
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace BlueStacks.BlueStacksUI
{
  public static class Animator
  {
    public static AnimationClock AnimatePenner(
      DependencyObject element,
      DependencyProperty prop,
      PennerDoubleAnimation.Equations type,
      double to,
      int durationMS,
      EventHandler callbackFunc)
    {
      return Animator.AnimatePenner(element, prop, type, new double?(), to, durationMS, callbackFunc);
    }

    public static AnimationClock AnimatePenner(
      DependencyObject element,
      DependencyProperty prop,
      PennerDoubleAnimation.Equations type,
      double? from,
      double to,
      int durationMS,
      EventHandler callbackFunc)
    {
      double defaultValue = double.IsNaN((double) element?.GetValue(prop)) ? 0.0 : (double) element.GetValue(prop);
      PennerDoubleAnimation pennerDoubleAnimation = new PennerDoubleAnimation(type, from.GetValueOrDefault(defaultValue), to);
      return Animator.Animate(element, prop, (AnimationTimeline) pennerDoubleAnimation, durationMS, new double?(), new double?(), callbackFunc);
    }

    public static AnimationClock AnimateDouble(
      DependencyObject element,
      DependencyProperty prop,
      double? from,
      double to,
      int durationMS,
      double? accel,
      double? decel,
      EventHandler callbackFunc)
    {
      double defaultValue = double.IsNaN((double) element?.GetValue(prop)) ? 0.0 : (double) element.GetValue(prop);
      DoubleAnimation doubleAnimation = new DoubleAnimation()
      {
        From = new double?(from.GetValueOrDefault(defaultValue)),
        To = new double?(to)
      };
      return Animator.Animate(element, prop, (AnimationTimeline) doubleAnimation, durationMS, accel, decel, callbackFunc);
    }

    public static void ClearAnimation(DependencyObject animatable, DependencyProperty property)
    {
      if (animatable == null)
        return;
      animatable.SetValue(property, animatable.GetValue(property));
      ((IAnimatable) animatable).ApplyAnimationClock(property, (AnimationClock) null);
    }

    private static AnimationClock Animate(
      DependencyObject animatable,
      DependencyProperty prop,
      AnimationTimeline anim,
      int duration,
      double? accel,
      double? decel,
      EventHandler func)
    {
      anim.AccelerationRatio = accel.GetValueOrDefault(0.0);
      anim.DecelerationRatio = decel.GetValueOrDefault(0.0);
      anim.Duration = (Duration) TimeSpan.FromMilliseconds((double) duration);
      anim.Freeze();
      AnimationClock animClock = anim.CreateClock();
      animClock.Completed += new EventHandler(animClock_Completed);
      if (func != null)
        animClock.Completed += func;
      animClock.Controller.Begin();
      Animator.ClearAnimation(animatable, prop);
      ((IAnimatable) animatable).ApplyAnimationClock(prop, animClock);
      return animClock;

      void animClock_Completed(object sender, EventArgs e)
      {
        Animator.ClearAnimation(animatable, prop);
        // ISSUE: method pointer
        animClock.Completed -= new EventHandler((object) this, __methodptr(\u003CAnimate\u003Eg__animClock_Completed\u007C0));
      }
    }
  }
}
