// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.XamlSizeConverter
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace BlueStacks.Common
{
  public class XamlSizeConverter : MarkupExtension, IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return (object) (Convert.ToDouble(value, (IFormatProvider) culture) * Convert.ToDouble(parameter, (IFormatProvider) culture));
    }

    public object ConvertBack(
      object value,
      Type targetType,
      object parameter,
      CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    public override object ProvideValue(System.IServiceProvider serviceProvider)
    {
      return (object) this;
    }
  }
}
