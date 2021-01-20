// Decompiled with JetBrains decompiler
// Type: BlueStacks.Core.EnumToBoolConverter
// Assembly: BlueStacks.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C36AABCB-E7F4-4E86-A72F-981C56431F94
// Assembly location: C:\Program Files\BlueStacks\BlueStacks.Core.dll

using System;
using System.Globalization;
using System.Windows.Data;

namespace BlueStacks.Core
{
  public class EnumToBoolConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return parameter == null || value == null ? Binding.DoNothing : (object) value.ToString().Equals(parameter.ToString(), StringComparison.InvariantCultureIgnoreCase);
    }

    public object ConvertBack(
      object value,
      Type targetType,
      object parameter,
      CultureInfo culture)
    {
      return parameter == null || value == null || !value.Equals((object) true) ? Binding.DoNothing : (object) parameter.ToString();
    }
  }
}
