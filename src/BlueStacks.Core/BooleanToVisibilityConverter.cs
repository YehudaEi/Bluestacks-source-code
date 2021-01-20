// Decompiled with JetBrains decompiler
// Type: BlueStacks.Core.BooleanToVisibilityConverter
// Assembly: BlueStacks.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C36AABCB-E7F4-4E86-A72F-981C56431F94
// Assembly location: C:\Program Files\BlueStacks\BlueStacks.Core.dll

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BlueStacks.Core
{
  public sealed class BooleanToVisibilityConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      flag = false;
      if (!(value is bool flag) && value is bool? nullable)
        flag = nullable.HasValue && nullable.Value;
      return (object) (Visibility) (!flag ? 1 : 0);
    }

    public object ConvertBack(
      object value,
      Type targetType,
      object parameter,
      CultureInfo culture)
    {
      return value is Visibility visibility ? (object) (visibility == Visibility.Visible) : (object) false;
    }
  }
}
