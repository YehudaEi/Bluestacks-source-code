// Decompiled with JetBrains decompiler
// Type: MultiInstanceManagerMVVM.Helper.Converters.NewInstanceTypeToTextMultiConverter
// Assembly: HD-MultiInstanceManager, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 0C4C54E8-53C7-4A46-AE96-CC8E89A6B884
// Assembly location: C:\Program Files\BlueStacks\HD-MultiInstanceManager.exe

using BlueStacks.Common;
using System;
using System.Globalization;
using System.Windows.Data;

namespace MultiInstanceManagerMVVM.Helper.Converters
{
  public class NewInstanceTypeToTextMultiConverter : IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      return values == null ? Binding.DoNothing : (!BlueStacksUIBinding.Instance.LocaleModel.ContainsKey("STRING_" + values[0].ToString()) || !BlueStacksUIBinding.Instance.LocaleModel.ContainsKey("STRING_INSTANCE") ? (object) "Create instance" : (object) (BlueStacksUIBinding.Instance.LocaleModel["STRING_" + values[0].ToString()] + " " + BlueStacksUIBinding.Instance.LocaleModel["STRING_INSTANCE"]));
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
