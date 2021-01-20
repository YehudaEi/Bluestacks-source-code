// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.EnumHelper
// Assembly: HD-ServiceInstaller, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 15F93427-26B3-4C7E-BAB1-0A00688BC4D4
// Assembly location: C:\Program Files\BlueStacks\HD-ServiceInstaller.exe

using System;

namespace BlueStacks.Common
{
  public static class EnumHelper
  {
    public static TEnum Parse<TEnum>(string value, TEnum defaultValue)
    {
      return (value == null ? 0 : (Enum.IsDefined(typeof (TEnum), (object) value) ? 1 : 0)) == 0 ? defaultValue : (TEnum) Enum.Parse(typeof (TEnum), value);
    }

    public static bool TryParse<TEnum>(string value, out TEnum result) where TEnum : struct, IConvertible
    {
      bool flag = value != null && Enum.IsDefined(typeof (TEnum), (object) value);
      result = flag ? (TEnum) Enum.Parse(typeof (TEnum), value) : default (TEnum);
      return flag;
    }
  }
}
