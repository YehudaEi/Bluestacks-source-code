// Decompiled with JetBrains decompiler
// Type: BlueStacks.Core.MinMaxRangeValidationRule
// Assembly: BlueStacks.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C36AABCB-E7F4-4E86-A72F-981C56431F94
// Assembly location: C:\Program Files\BlueStacks\BlueStacks.Core.dll

using System;
using System.Globalization;
using System.Windows.Controls;

namespace BlueStacks.Core
{
  public class MinMaxRangeValidationRule : ValidationRule
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
