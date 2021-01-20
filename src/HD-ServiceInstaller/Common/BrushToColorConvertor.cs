// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.BrushToColorConvertor
// Assembly: HD-ServiceInstaller, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 15F93427-26B3-4C7E-BAB1-0A00688BC4D4
// Assembly location: C:\Program Files\BlueStacks\HD-ServiceInstaller.exe

using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace BlueStacks.Common
{
  public class BrushToColorConvertor : MarkupExtension, IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return BrushToColorConvertor.Convert(value, targetType);
    }

    public object ConvertBack(
      object value,
      Type targetType,
      object parameter,
      CultureInfo culture)
    {
      return BrushToColorConvertor.Convert(value, targetType);
    }

    public static object Convert(object value, Type targetType)
    {
      return typeof (SolidColorBrush).IsSubclassOf(targetType) || value == null ? value : (object) (value is SolidColorBrush solidColorBrush ? new Color?(solidColorBrush.Color) : new Color?());
    }

    public override object ProvideValue(System.IServiceProvider serviceProvider)
    {
      return (object) this;
    }
  }
}
