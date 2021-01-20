// Decompiled with JetBrains decompiler
// Type: BlueStacks.Core.DragBehavior
// Assembly: BlueStacks.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C36AABCB-E7F4-4E86-A72F-981C56431F94
// Assembly location: C:\Program Files\BlueStacks\BlueStacks.Core.dll

using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace BlueStacks.Core
{
  public class DragBehavior : Behavior<UIElement>
  {
    protected override void OnAttached()
    {
      base.OnAttached();
      this.AssociatedObject.AllowDrop = true;
      this.AssociatedObject.MouseDown += new MouseButtonEventHandler(this.Parent_MouseDown);
    }

    private void Parent_MouseDown(object sender, MouseButtonEventArgs e)
    {
      try
      {
        if (e.ChangedButton != MouseButton.Left || Mouse.LeftButton != MouseButtonState.Pressed)
          return;
        Window.GetWindow((DependencyObject) (sender as FrameworkElement)).DragMove();
      }
      catch (Exception ex)
      {
      }
    }

    protected override void OnDetaching()
    {
      base.OnDetaching();
      this.AssociatedObject.MouseDown -= new MouseButtonEventHandler(this.Parent_MouseDown);
    }
  }
}
