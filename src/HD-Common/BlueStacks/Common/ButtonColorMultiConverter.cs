// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.ButtonColorMultiConverter
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.Globalization;
using System.Windows.Data;

namespace BlueStacks.Common
{
  public class ButtonColorMultiConverter : IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      if (parameter == null || values == null)
        return Binding.DoNothing;
      string[] strArray = parameter.ToString().Split('_');
      return !BlueStacksUIBinding.Instance.ColorModel.ContainsKey(values[0].ToString() + strArray[0] + strArray[1]) ? (object) BlueStacksUIBinding.Instance.ColorModel[values[0].ToString() + "MouseOut" + strArray[1]] : (object) BlueStacksUIBinding.Instance.ColorModel[values[0].ToString() + strArray[0] + strArray[1]];
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
