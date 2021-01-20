// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.LocalizedStringMultiConverter
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace BlueStacks.Common
{
  public class LocalizedStringMultiConverter : MarkupExtension, IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      return values == null ? (object) string.Empty : (object) LocaleStrings.GetLocalizedString(values[0].ToString(), "");
    }

    public object[] ConvertBack(
      object value,
      Type[] targetTypes,
      object parameter,
      CultureInfo culture)
    {
      throw new NotSupportedException();
    }

    public override object ProvideValue(System.IServiceProvider serviceProvider)
    {
      return (object) this;
    }
  }
}
