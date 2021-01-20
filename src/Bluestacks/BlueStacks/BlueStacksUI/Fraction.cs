// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.Fraction
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using System;
using System.Globalization;

namespace BlueStacks.BlueStacksUI
{
  public class Fraction
  {
    private long m_iNumerator;
    private long m_iDenominator;
    private double m_iDoubleValue;

    public Fraction()
    {
      this.Initialize(0L, 1L);
    }

    public Fraction(long iWholeNumber)
    {
      this.Initialize(iWholeNumber, 1L);
    }

    public Fraction(double dDecimalValue)
    {
      Fraction fraction = Fraction.ToFraction(dDecimalValue);
      this.Initialize(fraction.Numerator, fraction.Denominator);
    }

    public Fraction(string strValue)
    {
      Fraction fraction = Fraction.ToFraction(strValue);
      this.Initialize(fraction.Numerator, fraction.Denominator);
    }

    public Fraction(long iNumerator, long iDenominator)
    {
      this.Initialize(iNumerator, iDenominator);
    }

    private void Initialize(long iNumerator, long iDenominator)
    {
      this.Numerator = iNumerator;
      this.Denominator = iDenominator;
      Fraction.ReduceFraction(this);
    }

    public long Denominator
    {
      get
      {
        return this.m_iDenominator;
      }
      set
      {
        if (value == 0L)
          throw new FractionException("Denominator cannot be assigned a ZERO Value");
        this.m_iDenominator = value;
        this.CalculateDoubleValue();
      }
    }

    public long Numerator
    {
      get
      {
        return this.m_iNumerator;
      }
      set
      {
        this.m_iNumerator = value;
        this.CalculateDoubleValue();
      }
    }

    public double DoubleValue
    {
      get
      {
        return this.m_iDoubleValue;
      }
      set
      {
        this.m_iDoubleValue = value;
      }
    }

    private void CalculateDoubleValue()
    {
      this.m_iDoubleValue = (double) this.m_iNumerator / (double) this.m_iDenominator;
    }

    public double ToDouble()
    {
      return (double) this.Numerator / (double) this.Denominator;
    }

    public override string ToString()
    {
      return this.Denominator != 1L ? this.Numerator.ToString() + "/" + this.Denominator.ToString() : this.Numerator.ToString((IFormatProvider) CultureInfo.InvariantCulture);
    }

    public static Fraction ToFraction(string strValue)
    {
      if (string.IsNullOrEmpty(strValue))
        return (Fraction) null;
      int length = 0;
      while (length < strValue.Length && strValue[length] != '/')
        ++length;
      return length == strValue.Length ? (Fraction) Convert.ToDouble(strValue, (IFormatProvider) CultureInfo.InvariantCulture) : new Fraction(Convert.ToInt64(strValue.Substring(0, length), (IFormatProvider) CultureInfo.InvariantCulture), Convert.ToInt64(strValue.Substring(length + 1), (IFormatProvider) CultureInfo.InvariantCulture));
    }

    public static Fraction ToFraction(double dValue)
    {
      try
      {
        Fraction fraction;
        if (dValue % 1.0 == 0.0)
        {
          fraction = new Fraction(checked ((long) dValue));
        }
        else
        {
          double a = dValue;
          long iDenominator = 1;
          string str;
          for (str = dValue.ToString((IFormatProvider) CultureInfo.InvariantCulture); str.IndexOf("E", StringComparison.InvariantCulture) > 0; str = a.ToString((IFormatProvider) CultureInfo.InvariantCulture))
          {
            a *= 10.0;
            checked { iDenominator *= 10L; }
          }
          int index = 0;
          while (str[index] != '.')
            checked { ++index; }
          int num = checked (str.Length - index - 1);
          while (num > 0)
          {
            a *= 10.0;
            checked { iDenominator *= 10L; }
            checked { --num; }
          }
          fraction = new Fraction((long) checked ((int) Math.Round(a)), iDenominator);
        }
        return fraction;
      }
      catch (OverflowException ex)
      {
        throw new FractionException("Conversion not possible due to overflow");
      }
      catch (Exception ex)
      {
        throw new FractionException("Conversion not possible");
      }
    }

    public Fraction Duplicate()
    {
      return new Fraction()
      {
        Numerator = this.Numerator,
        Denominator = this.Denominator
      };
    }

    public static Fraction Inverse(Fraction frac1)
    {
      if ((Fraction) null == frac1 || frac1.Numerator == 0L)
        throw new FractionException("Operation not possible (Denominator cannot be assigned a ZERO Value)");
      return new Fraction(frac1.Denominator, frac1.Numerator);
    }

    public static Fraction operator -(Fraction frac1)
    {
      return !((Fraction) null != frac1) ? (Fraction) null : Fraction.Negate(frac1);
    }

    public static Fraction operator +(Fraction frac1, Fraction frac2)
    {
      return !((Fraction) null != frac1) || !((Fraction) null != frac2) ? (Fraction) null : Fraction.Add(frac1, frac2);
    }

    public static Fraction operator +(int iNo, Fraction frac1)
    {
      return !((Fraction) null != frac1) ? (Fraction) null : Fraction.Add(frac1, new Fraction((long) iNo));
    }

    public static Fraction operator +(Fraction frac1, int iNo)
    {
      return !((Fraction) null != frac1) ? (Fraction) null : Fraction.Add(frac1, new Fraction((long) iNo));
    }

    public static Fraction operator +(double dbl, Fraction frac1)
    {
      return !((Fraction) null != frac1) ? (Fraction) null : Fraction.Add(frac1, Fraction.ToFraction(dbl));
    }

    public static Fraction operator +(Fraction frac1, double dbl)
    {
      return !((Fraction) null != frac1) ? (Fraction) null : Fraction.Add(frac1, Fraction.ToFraction(dbl));
    }

    public static Fraction operator -(Fraction frac1, Fraction frac2)
    {
      return !((Fraction) null != frac1) || !((Fraction) null != frac2) ? (Fraction) null : Fraction.Add(frac1, -frac2);
    }

    public static Fraction operator -(int iNo, Fraction frac1)
    {
      return !((Fraction) null != frac1) ? (Fraction) null : Fraction.Add(-frac1, new Fraction((long) iNo));
    }

    public static Fraction operator -(Fraction frac1, int iNo)
    {
      return !((Fraction) null != frac1) ? (Fraction) null : Fraction.Add(frac1, -new Fraction((long) iNo));
    }

    public static Fraction operator -(double dbl, Fraction frac1)
    {
      return !((Fraction) null != frac1) ? (Fraction) null : Fraction.Add(-frac1, Fraction.ToFraction(dbl));
    }

    public static Fraction operator -(Fraction frac1, double dbl)
    {
      return !((Fraction) null != frac1) ? (Fraction) null : Fraction.Add(frac1, -Fraction.ToFraction(dbl));
    }

    public static Fraction operator *(Fraction frac1, Fraction frac2)
    {
      return !((Fraction) null != frac1) || !((Fraction) null != frac2) ? (Fraction) null : Fraction.Multiply(frac1, frac2);
    }

    public static Fraction operator *(int iNo, Fraction frac1)
    {
      return !((Fraction) null != frac1) ? (Fraction) null : Fraction.Multiply(frac1, new Fraction((long) iNo));
    }

    public static Fraction operator *(Fraction frac1, int iNo)
    {
      return !((Fraction) null != frac1) ? (Fraction) null : Fraction.Multiply(frac1, new Fraction((long) iNo));
    }

    public static Fraction operator *(double dbl, Fraction frac1)
    {
      return !((Fraction) null != frac1) ? (Fraction) null : Fraction.Multiply(frac1, Fraction.ToFraction(dbl));
    }

    public static Fraction operator *(Fraction frac1, double dbl)
    {
      return !((Fraction) null != frac1) ? (Fraction) null : Fraction.Multiply(frac1, Fraction.ToFraction(dbl));
    }

    public static Fraction operator /(Fraction frac1, Fraction frac2)
    {
      return !((Fraction) null != frac1) || !((Fraction) null != frac2) ? (Fraction) null : Fraction.Multiply(frac1, Fraction.Inverse(frac2));
    }

    public static Fraction operator /(int iNo, Fraction frac1)
    {
      return !((Fraction) null != frac1) ? (Fraction) null : Fraction.Multiply(Fraction.Inverse(frac1), new Fraction((long) iNo));
    }

    public static Fraction operator /(Fraction frac1, int iNo)
    {
      return !((Fraction) null != frac1) ? (Fraction) null : Fraction.Multiply(frac1, Fraction.Inverse(new Fraction((long) iNo)));
    }

    public static Fraction operator /(double dbl, Fraction frac1)
    {
      return !((Fraction) null != frac1) ? (Fraction) null : Fraction.Multiply(Fraction.Inverse(frac1), Fraction.ToFraction(dbl));
    }

    public static Fraction operator /(Fraction frac1, double dbl)
    {
      return !((Fraction) null != frac1) ? (Fraction) null : Fraction.Multiply(frac1, Fraction.Inverse(Fraction.ToFraction(dbl)));
    }

    public static bool operator ==(Fraction frac1, Fraction frac2)
    {
      return (object) frac1 == null ? (object) frac2 == null : frac1.Equals((object) frac2);
    }

    public static bool operator !=(Fraction frac1, Fraction frac2)
    {
      return !(frac1 == frac2);
    }

    public static bool operator ==(Fraction frac1, int iNo)
    {
      return (object) frac1 != null && frac1.Equals((object) new Fraction((long) iNo));
    }

    public static bool operator !=(Fraction frac1, int iNo)
    {
      return !(frac1 == iNo);
    }

    public static bool operator ==(Fraction frac1, double dbl)
    {
      return (object) frac1 != null && frac1.Equals((object) new Fraction(dbl));
    }

    public static bool operator !=(Fraction frac1, double dbl)
    {
      return !(frac1 == dbl);
    }

    public static bool operator <(Fraction frac1, Fraction frac2)
    {
      return (Fraction) null != frac1 && (Fraction) null != frac2 && frac1.Numerator * frac2.Denominator < frac2.Numerator * frac1.Denominator;
    }

    public static bool operator >(Fraction frac1, Fraction frac2)
    {
      return (Fraction) null != frac1 && (Fraction) null != frac2 && frac1.Numerator * frac2.Denominator > frac2.Numerator * frac1.Denominator;
    }

    public static bool operator <=(Fraction frac1, Fraction frac2)
    {
      if ((Fraction) null != frac1 && (Fraction) null != frac2)
        return frac1.Numerator * frac2.Denominator <= frac2.Numerator * frac1.Denominator;
      return (Fraction) null == frac1 && (Fraction) null == frac2;
    }

    public static bool operator >=(Fraction frac1, Fraction frac2)
    {
      if ((Fraction) null != frac1 && (Fraction) null != frac2)
        return frac1.Numerator * frac2.Denominator >= frac2.Numerator * frac1.Denominator;
      return (Fraction) null == frac1 && (Fraction) null == frac2;
    }

    public static implicit operator Fraction(long lNo)
    {
      return new Fraction(lNo);
    }

    public static implicit operator Fraction(double dNo)
    {
      return new Fraction(dNo);
    }

    public static implicit operator Fraction(string strNo)
    {
      return new Fraction(strNo);
    }

    public static explicit operator double(Fraction frac)
    {
      return !((Fraction) null != frac) ? 0.0 : frac.ToDouble();
    }

    public static implicit operator string(Fraction frac)
    {
      return !((Fraction) null != frac) ? string.Empty : frac.ToString();
    }

    public override bool Equals(object obj)
    {
      Fraction fraction = (Fraction) obj;
      long numerator = this.Numerator;
      long? nullable = (object) fraction != null ? new long?(fraction.Numerator) : new long?();
      if (!(numerator == nullable.GetValueOrDefault() & nullable.HasValue))
        return false;
      long denominator = this.Denominator;
      nullable = (object) fraction != null ? new long?(fraction.Denominator) : new long?();
      long valueOrDefault = nullable.GetValueOrDefault();
      return denominator == valueOrDefault & nullable.HasValue;
    }

    public override int GetHashCode()
    {
      return Convert.ToInt32((this.Numerator ^ this.Denominator) & (long) uint.MaxValue);
    }

    public static Fraction Negate(Fraction frac1)
    {
      return (object) frac1 != null ? new Fraction(-frac1.Numerator, frac1.Denominator) : (Fraction) null;
    }

    public static Fraction Add(Fraction frac1, Fraction frac2)
    {
      try
      {
        if ((object) frac1 != null && (object) frac2 != null)
          return new Fraction(checked (frac1.Numerator * frac2.Denominator + frac2.Numerator * frac1.Denominator), checked (frac1.Denominator * frac2.Denominator));
        return (object) frac1 != null ? frac1 : frac2;
      }
      catch (OverflowException ex)
      {
        throw new FractionException("Overflow occurred while performing arithemetic operation");
      }
      catch (Exception ex)
      {
        throw new FractionException("An error occurred while performing arithemetic operation");
      }
    }

    public static Fraction Multiply(Fraction frac1, Fraction frac2)
    {
      try
      {
        return (object) frac1 != null && (object) frac2 != null ? new Fraction(checked (frac1.Numerator * frac2.Numerator), checked (frac1.Denominator * frac2.Denominator)) : (Fraction) null;
      }
      catch (OverflowException ex)
      {
        throw new FractionException("Overflow occurred while performing arithemetic operation");
      }
      catch (Exception ex)
      {
        throw new FractionException("An error occurred while performing arithemetic operation");
      }
    }

    private static long GCD(long iNo1, long iNo2)
    {
      if (iNo1 < 0L)
        iNo1 = -iNo1;
      if (iNo2 < 0L)
        iNo2 = -iNo2;
      do
      {
        if (iNo1 < iNo2)
        {
          long num = iNo1;
          iNo1 = iNo2;
          iNo2 = num;
        }
        iNo1 %= iNo2;
      }
      while (iNo1 != 0L);
      return iNo2;
    }

    public static void ReduceFraction(Fraction frac)
    {
      try
      {
        if (!((Fraction) null != frac))
          return;
        if (frac.Numerator == 0L)
        {
          frac.Denominator = 1L;
        }
        else
        {
          long num = Fraction.GCD(frac.Numerator, frac.Denominator);
          frac.Numerator /= num;
          frac.Denominator /= num;
          if (frac.Denominator >= 0L)
            return;
          frac.Numerator *= -1L;
          frac.Denominator *= -1L;
        }
      }
      catch (Exception ex)
      {
        throw new FractionException("Cannot reduce Fraction: " + ex.Message);
      }
    }

    public static Fraction Subtract(Fraction frac1, Fraction frac2)
    {
      return !((Fraction) null != frac1) || !((Fraction) null != frac2) ? (Fraction) null : Fraction.Add(frac1, -frac2);
    }

    public static Fraction Divide(Fraction frac1, Fraction frac2)
    {
      return !((Fraction) null != frac1) || !((Fraction) null != frac2) ? (Fraction) null : Fraction.Multiply(frac1, Fraction.Inverse(frac2));
    }

    public int CompareTo(Fraction frac)
    {
      if ((object) frac == null)
        return 1;
      if (this.Numerator * this.Denominator == frac.Numerator * frac.Denominator)
        return 0;
      return this.Numerator * this.Denominator < frac.Numerator * frac.Denominator ? 1 : -1;
    }
  }
}
