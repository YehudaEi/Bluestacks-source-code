// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.TextInputValidityToVisibilityConverter
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BlueStacks.Common
{
  public class TextInputValidityToVisibilityConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      Visibility visibility = Visibility.Collapsed;
      if (value != null)
      {
        if (parameter == null)
        {
          switch ((TextValidityOptions) Enum.Parse(typeof (TextValidityOptions), value.ToString()))
          {
            case TextValidityOptions.Warning:
            case TextValidityOptions.Info:
              visibility = Visibility.Visible;
              break;
          }
        }
        else
          visibility = object.Equals(value, parameter) ? Visibility.Visible : Visibility.Collapsed;
      }
      return (object) visibility;
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
