// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.ResolutionRadioButtonContentConverter
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace BlueStacks.Common
{
  public class ResolutionRadioButtonContentConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (parameter == null || value == null)
        return Binding.DoNothing;
      Dictionary<string, string> dictionary = (Dictionary<string, string>) value;
      return dictionary.ContainsKey(parameter.ToString()) ? (object) dictionary[parameter.ToString()] : Binding.DoNothing;
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
