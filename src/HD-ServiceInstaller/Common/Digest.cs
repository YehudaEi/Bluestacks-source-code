﻿// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.Digest
// Assembly: HD-ServiceInstaller, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 15F93427-26B3-4C7E-BAB1-0A00688BC4D4
// Assembly location: C:\Program Files\BlueStacks\HD-ServiceInstaller.exe

using System;
using System.Globalization;

namespace BlueStacks.Common
{
  public class Digest
  {
    public uint A { get; set; }

    public uint B { get; set; }

    public uint C { get; set; }

    public uint D { get; set; }

    public Digest()
    {
      this.A = 1732584193U;
      this.B = 4023233417U;
      this.C = 2562383102U;
      this.D = 271733878U;
    }

    public string GetString()
    {
      uint num = Digest.ReverseByte(this.A);
      string str1 = num.ToString("X8", (IFormatProvider) CultureInfo.InvariantCulture);
      num = Digest.ReverseByte(this.B);
      string str2 = num.ToString("X8", (IFormatProvider) CultureInfo.InvariantCulture);
      num = Digest.ReverseByte(this.C);
      string str3 = num.ToString("X8", (IFormatProvider) CultureInfo.InvariantCulture);
      num = Digest.ReverseByte(this.D);
      string str4 = num.ToString("X8", (IFormatProvider) CultureInfo.InvariantCulture);
      return str1 + str2 + str3 + str4;
    }

    private static uint ReverseByte(uint uiNumber)
    {
      return (uint) (((int) uiNumber & (int) byte.MaxValue) << 24 | (int) (uiNumber >> 24) | (int) ((uiNumber & 16711680U) >> 8) | ((int) uiNumber & 65280) << 8);
    }
  }
}