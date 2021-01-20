// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.CenterToolTipConverter
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace BlueStacks.Common
{
  public class CenterToolTipConverter : MarkupExtension, IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      if (((IEnumerable<object>) values).FirstOrDefault<object>((Func<object, bool>) (v => v == DependencyProperty.UnsetValue)) != null)
        return (object) double.NaN;
      double num1 = 0.0;
      double num2 = 0.0;
      if (values != null)
      {
        num1 = (double) values[0];
        num2 = (double) values[1];
      }
      return (object) (num1 / 2.0 - num2 / 2.0);
    }

    public object[] ConvertBack(
      object value,
      Type[] targetTypes,
      object parameter,
      CultureInfo culture)
    {
      throw new NotSupportedException();
    }

    public override object ProvideValue(System.IServiceProvider serviceProvider)
    {
      return (object) this;
    }
  }
}
