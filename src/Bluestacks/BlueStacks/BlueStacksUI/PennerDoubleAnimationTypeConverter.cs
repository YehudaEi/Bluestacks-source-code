// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.PennerDoubleAnimationTypeConverter
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using System;
using System.ComponentModel;
using System.Globalization;

namespace BlueStacks.BlueStacksUI
{
  public class PennerDoubleAnimationTypeConverter : TypeConverter
  {
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
      return (object) sourceType == (object) typeof (string);
    }

    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
      return (object) destinationType == (object) typeof (Enum);
    }

    public override object ConvertFrom(
      ITypeDescriptorContext context,
      CultureInfo culture,
      object value)
    {
      foreach (int num in Enum.GetValues(typeof (PennerDoubleAnimation.Equations)))
      {
        if (Enum.GetName(typeof (PennerDoubleAnimation.Equations), (object) num) == value?.ToString())
          return (object) (PennerDoubleAnimation.Equations) num;
      }
      return (object) null;
    }

    public override object ConvertTo(
      ITypeDescriptorContext context,
      CultureInfo culture,
      object value,
      Type destinationType)
    {
      return value != null ? (object) ((PennerDoubleAnimation.Equations) value).ToString() : (object) null;
    }
  }
}
