// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.ArithmeticValueConverter
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.Globalization;
using System.Windows.Data;

namespace BlueStacks.Common
{
  public class ArithmeticValueConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return value == null || parameter == null ? Binding.DoNothing : (object) (Convert.ToDouble(value, (IFormatProvider) CultureInfo.InvariantCulture) / Convert.ToDouble(parameter, (IFormatProvider) CultureInfo.InvariantCulture));
    }

    public object ConvertBack(
      object value,
      Type targetType,
      object parameter,
      CultureInfo culture)
    {
      return Binding.DoNothing;
    }
  }
}
