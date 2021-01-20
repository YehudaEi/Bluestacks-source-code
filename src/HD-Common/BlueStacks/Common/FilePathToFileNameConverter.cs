// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.FilePathToFileNameConverter
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace BlueStacks.Common
{
  public class FilePathToFileNameConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return value is string ? (object) Path.GetFileName(value.ToString()) : Binding.DoNothing;
    }

    public object ConvertBack(
      object value,
      Type targetType,
      object parameter,
      CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
