// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.MinMaxRangeValidationRule2
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.Globalization;
using System.Windows.Controls;

namespace BlueStacks.Common
{
  public class MinMaxRangeValidationRule2 : ValidationRule
  {
    public Wrapper Wrapper { get; set; }

    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
      if (value == null)
        return new ValidationResult(false, (object) "Illegal characters");
      return string.IsNullOrEmpty(value.ToString()) || int.Parse(value.ToString(), (IFormatProvider) CultureInfo.InvariantCulture) < this.Wrapper.Min || int.Parse(value.ToString(), (IFormatProvider) CultureInfo.InvariantCulture) > this.Wrapper.Max ? new ValidationResult(false, (object) this.Wrapper.ErrorMessage) : ValidationResult.ValidResult;
    }
  }
}
