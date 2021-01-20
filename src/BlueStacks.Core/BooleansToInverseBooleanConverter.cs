// Decompiled with JetBrains decompiler
// Type: BlueStacks.Core.BooleansToInverseBooleanConverter
// Assembly: BlueStacks.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C36AABCB-E7F4-4E86-A72F-981C56431F94
// Assembly location: C:\Program Files\BlueStacks\BlueStacks.Core.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace BlueStacks.Core
{
  public class BooleansToInverseBooleanConverter : IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      if (values == null)
        return Binding.DoNothing;
      return values.Length != 0 && ((IEnumerable<object>) values).Where<object>((Func<object, bool>) (value => value is bool flag && flag)).ToList<object>().Count > 0 ? (object) false : (object) true;
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
