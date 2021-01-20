// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.MarginSetter
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace BlueStacks.Common
{
  public class MarginSetter : MarkupExtension
  {
    public static readonly DependencyProperty MarginProperty = DependencyProperty.RegisterAttached("Margin", typeof (Thickness), typeof (MarginSetter), (PropertyMetadata) new UIPropertyMetadata((object) new Thickness(), new PropertyChangedCallback(MarginSetter.CreateThicknesForChildren)));

    private static Thickness GetMargin(DependencyObject obj)
    {
      return (Thickness) obj.GetValue(MarginSetter.MarginProperty);
    }

    public static void SetMargin(DependencyObject obj, Thickness value)
    {
      obj?.SetValue(MarginSetter.MarginProperty, (object) value);
    }

    public static void CreateThicknesForChildren(
      object sender,
      DependencyPropertyChangedEventArgs e)
    {
      if (!(sender is Panel panel))
        return;
      foreach (object child in panel.Children)
      {
        if (child is FrameworkElement frameworkElement)
          frameworkElement.Margin = MarginSetter.GetMargin((DependencyObject) panel);
      }
    }

    public override object ProvideValue(System.IServiceProvider serviceProvider)
    {
      return (object) this;
    }
  }
}
