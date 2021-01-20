// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.MacroExceutionSpeedToIndexConverter
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.Globalization;
using System.Windows.Data;

namespace BlueStacks.Common
{
  public class MacroExceutionSpeedToIndexConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      double num = Convert.ToDouble(value, (IFormatProvider) CultureInfo.InvariantCulture);
      return num == 0.0 ? (object) 1 : (object) (int) (num / 0.5 - 1.0);
    }

    public object ConvertBack(
      object value,
      Type targetType,
      object parameter,
      CultureInfo culture)
    {
      int int32 = Convert.ToInt32(value, (IFormatProvider) CultureInfo.InvariantCulture);
      return int32 < 0 ? (object) 1.0 : (object) ((double) (int32 + 1) * 0.5);
    }
  }
}
