// Decompiled with JetBrains decompiler
// Type: BlueStacks.Core.BooleanToImageNameConverter
// Assembly: BlueStacks.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C36AABCB-E7F4-4E86-A72F-981C56431F94
// Assembly location: C:\Program Files\BlueStacks\BlueStacks.Core.dll

using System;
using System.Globalization;
using System.Windows.Data;

namespace BlueStacks.Core
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
