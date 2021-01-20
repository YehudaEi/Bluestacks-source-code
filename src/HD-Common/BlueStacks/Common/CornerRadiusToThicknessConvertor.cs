// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.CornerRadiusToThicknessConvertor
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace BlueStacks.Common
{
  public class CornerRadiusToThicknessConvertor : MarkupExtension, IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return CornerRadiusToThicknessConvertor.Convert(value, targetType);
    }

    public object ConvertBack(
      object value,
      Type targetType,
      object parameter,
      CultureInfo culture)
    {
      return CornerRadiusToThicknessConvertor.Convert(value, targetType);
    }

    public static object Convert(object value, Type targetType)
    {
      if (!typeof (Thickness).Equals(targetType))
        return value;
      CornerRadius cornerRadius = (CornerRadius) value;
      return (object) new Thickness(cornerRadius.TopLeft, cornerRadius.TopRight, cornerRadius.BottomRight, cornerRadius.BottomLeft);
    }

    public override object ProvideValue(System.IServiceProvider serviceProvider)
    {
      return (object) this;
    }
  }
}
