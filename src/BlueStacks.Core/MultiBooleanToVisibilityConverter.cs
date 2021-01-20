// Decompiled with JetBrains decompiler
// Type: BlueStacks.Core.MultiBooleanToVisibilityConverter
// Assembly: BlueStacks.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C36AABCB-E7F4-4E86-A72F-981C56431F94
// Assembly location: C:\Program Files\BlueStacks\BlueStacks.Core.dll

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BlueStacks.Core
{
  public class MultiBooleanToVisibilityConverter : IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      bool flag1 = true;
      if (values != null)
      {
        foreach (object obj in values)
        {
          if (obj is bool flag2)
            flag1 = flag1 && flag2;
        }
      }
      return (object) (Visibility) (flag1 ? 0 : 2);
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
