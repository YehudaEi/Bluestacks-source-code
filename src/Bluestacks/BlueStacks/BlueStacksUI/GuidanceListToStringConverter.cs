// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.GuidanceListToStringConverter
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace BlueStacks.BlueStacksUI
{
  public class GuidanceListToStringConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (!(value is IEnumerable<string> strings))
        return (object) string.Empty;
      List<string> stringList = new List<string>();
      foreach (string text in strings)
        stringList.Add(KMManager.GetKeyUIValue(text));
      return (object) string.Join(" / ", stringList.ToArray());
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
