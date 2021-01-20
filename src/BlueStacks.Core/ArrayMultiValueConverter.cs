// Decompiled with JetBrains decompiler
// Type: BlueStacks.Core.ArrayMultiValueConverter
// Assembly: BlueStacks.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C36AABCB-E7F4-4E86-A72F-981C56431F94
// Assembly location: C:\Program Files\BlueStacks\BlueStacks.Core.dll

using System;
using System.Globalization;
using System.Windows.Data;

namespace BlueStacks.Core
{
  public class ArrayMultiValueConverter : IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      return values == null ? Binding.DoNothing : values.Clone();
    }

    public object[] ConvertBack(
      object value,
      Type[] targetTypes,
      object parameter,
      CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
