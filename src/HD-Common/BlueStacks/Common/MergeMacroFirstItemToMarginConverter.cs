// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.MergeMacroFirstItemToMarginConverter
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BlueStacks.Common
{
  public class MergeMacroFirstItemToMarginConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return (object) ((bool) value ? new Thickness(0.0, 1.0, 0.0, 0.0) : new Thickness(0.0));
    }

    public object ConvertBack(
      object value,
      Type targetType,
      object parameter,
      CultureInfo culture)
    {
      throw new NotImplementedException("ConvertBack() of MergeMacroLastItemToBorderThicknessConverter is not implemented");
    }
  }
}
