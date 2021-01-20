// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.InverseVisibilityConverter2
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BlueStacks.Common
{
  public class InverseVisibilityConverter2 : IValueConverter
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
