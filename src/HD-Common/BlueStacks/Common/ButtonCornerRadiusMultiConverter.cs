// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.ButtonCornerRadiusMultiConverter
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BlueStacks.Common
{
  public class ButtonCornerRadiusMultiConverter : IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      if (values == null)
        return Binding.DoNothing;
      CornerRadius cornerRadius = (CornerRadius) values[0];
      double topLeft = cornerRadius.TopLeft == 0.0 ? 0.0 : (double) values[1] / cornerRadius.TopLeft;
      double num1 = cornerRadius.TopRight == 0.0 ? 0.0 : (double) values[1] / cornerRadius.TopRight;
      double num2 = cornerRadius.BottomRight == 0.0 ? 0.0 : (double) values[1] / cornerRadius.BottomRight;
      double num3 = cornerRadius.BottomLeft == 0.0 ? 0.0 : (double) values[1] / cornerRadius.BottomLeft;
      double topRight = num1;
      double bottomRight = num2;
      double bottomLeft = num3;
      return (object) new CornerRadius(topLeft, topRight, bottomRight, bottomLeft);
    }

    public object[] ConvertBack(
      object value,
      Type[] targetTypes,
      object parameter,
      CultureInfo culture)
    {
      return (object[]) null;
    }
  }
}
