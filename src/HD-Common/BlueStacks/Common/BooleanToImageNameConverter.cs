// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.BooleanToImageNameConverter
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.Globalization;
using System.Windows.Data;

namespace BlueStacks.Common
{
  public class BooleanToImageNameConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null || parameter == null)
        return Binding.DoNothing;
      string[] strArray = parameter.ToString().Split('|');
      if (strArray.Length != 2)
        return Binding.DoNothing;
      flag = false;
      if (!(value is bool flag) && value is bool? nullable)
        flag = nullable.HasValue && nullable.Value;
      return !flag ? (object) strArray[1] : (object) strArray[0];
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
