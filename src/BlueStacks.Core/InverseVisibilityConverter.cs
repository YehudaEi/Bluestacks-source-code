// Decompiled with JetBrains decompiler
// Type: BlueStacks.Core.InverseVisibilityConverter
// Assembly: BlueStacks.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C36AABCB-E7F4-4E86-A72F-981C56431F94
// Assembly location: C:\Program Files\BlueStacks\BlueStacks.Core.dll

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BlueStacks.Core
{
  public class InverseVisibilityConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return value == null ? Binding.DoNothing : (object) (Visibility) ((Visibility) value == Visibility.Visible ? 2 : 0);
    }

    public object ConvertBack(
      object value,
      Type targetType,
      object parameter,
      CultureInfo culture)
    {
      return value == null ? Binding.DoNothing : (object) (Visibility) ((Visibility) value == Visibility.Visible ? 2 : 0);
    }
  }
}
