// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.ColorUtils
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;

namespace BlueStacks.Common
{
  [Serializable]
  public class ColorUtils
  {
    public byte R { get; set; }

    public byte G { get; set; }

    public byte B { get; set; }

    public byte A { get; set; }

    public ColorUtils()
    {
      this.R = byte.MaxValue;
      this.G = byte.MaxValue;
      this.B = byte.MaxValue;
      this.A = byte.MaxValue;
    }

    public ColorUtils(System.Windows.Media.Color value)
    {
      this.R = value.R;
      this.G = value.G;
      this.B = value.B;
      this.A = value.A;
    }

    public ColorUtils(System.Drawing.Color value)
    {
      this.R = value.R;
      this.G = value.G;
      this.B = value.B;
      this.A = value.A;
    }

    public static implicit operator System.Drawing.Color(ColorUtils rgb)
    {
      return rgb != null ? System.Drawing.Color.FromArgb((int) rgb.A, (int) rgb.R, (int) rgb.G, (int) rgb.B) : new System.Drawing.Color();
    }

    public static explicit operator ColorUtils(System.Drawing.Color c)
    {
      return new ColorUtils(c);
    }

    public static ColorUtils FromHSL(double H, double S, double L)
    {
      return ColorUtils.FromHSLA(H, S, L, 1.0);
    }

    public static ColorUtils FromHSLA(double H, double S, double L, double A)
    {
      if (H > 1.0)
        H = 1.0;
      if (S > 1.0)
        S = 1.0;
      if (L > 1.0)
        L = 1.0;
      if (H < 0.0)
        H = 0.0;
      if (S < 0.0)
        S = 0.0;
      if (L < 0.0)
        L = 0.0;
      if (A > 1.0)
        A = 1.0;
      double num1 = L;
      double num2 = L;
      double num3 = L;
      double num4 = L <= 0.5 ? L * (1.0 + S) : L + S - L * S;
      if (num4 > 0.0)
      {
        double num5 = L + L - num4;
        double num6 = (num4 - num5) / num4;
        H *= 6.0;
        int num7 = (int) H;
        double num8 = H - (double) num7;
        double num9 = num4 * num6 * num8;
        double num10 = num5 + num9;
        double num11 = num4 - num9;
        switch (num7)
        {
          case 0:
            num1 = num4;
            num2 = num10;
            num3 = num5;
            break;
          case 1:
            num1 = num11;
            num2 = num4;
            num3 = num5;
            break;
          case 2:
            num1 = num5;
            num2 = num4;
            num3 = num10;
            break;
          case 3:
            num1 = num5;
            num2 = num11;
            num3 = num4;
            break;
          case 4:
            num1 = num10;
            num2 = num5;
            num3 = num4;
            break;
          case 5:
            num1 = num4;
            num2 = num5;
            num3 = num11;
            break;
        }
      }
      return new ColorUtils()
      {
        R = Convert.ToByte(num1 * (double) byte.MaxValue),
        G = Convert.ToByte(num2 * (double) byte.MaxValue),
        B = Convert.ToByte(num3 * (double) byte.MaxValue),
        A = Convert.ToByte(A * (double) byte.MaxValue)
      };
    }

    public float H
    {
      get
      {
        return ((System.Drawing.Color) this).GetHue() / 360f;
      }
    }

    public float S
    {
      get
      {
        return ((System.Drawing.Color) this).GetSaturation();
      }
    }

    public float L
    {
      get
      {
        return ((System.Drawing.Color) this).GetBrightness();
      }
    }

    public System.Windows.Media.Color WPFColor
    {
      get
      {
        return System.Windows.Media.Color.FromArgb(this.A, this.R, this.G, this.B);
      }
    }
  }
}
