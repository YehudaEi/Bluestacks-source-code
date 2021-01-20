// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.CornerRadiusToDoubleConvertor
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
  public class CornerRadiusToDoubleConvertor : MarkupExtension, IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return CornerRadiusToDoubleConvertor.Convert(value, targetType);
    }

    public object ConvertBack(
      object value,
      Type targetType,
      object parameter,
      CultureInfo culture)
    {
      return CornerRadiusToDoubleConvertor.Convert(value, targetType);
    }

    public static object Convert(object value, Type targetType)
    {
      return typeof (double).Equals(targetType) ? (object) ((CornerRadius) value).TopLeft : value;
    }

    public override object ProvideValue(System.IServiceProvider serviceProvider)
    {
      return (object) this;
    }
  }
}
