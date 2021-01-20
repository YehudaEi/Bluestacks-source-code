// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.Grm.Comparers.BooleanComparer
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System;
using System.Globalization;

namespace BlueStacks.BlueStacksUI.Grm.Comparers
{
  internal class BooleanComparer : IGrmOperatorComparer<bool>
  {
    public bool Contains(bool left, string right)
    {
      return left.ToString((IFormatProvider) CultureInfo.InvariantCulture).Contains(right, StringComparison.InvariantCultureIgnoreCase);
    }

    public bool Equal(bool left, string right)
    {
      bool boolean = Convert.ToBoolean(right, (IFormatProvider) CultureInfo.InvariantCulture);
      return left == boolean;
    }

    public bool GreaterThan(bool left, string right)
    {
      throw new ArgumentException("Operator GreaterThan is not supported with boolean expression");
    }

    public bool GreaterThanEqual(bool left, string right)
    {
      throw new ArgumentException("Operator GreaterThanEqual is not supported with boolean expression");
    }

    public bool In(bool left, string right)
    {
      throw new ArgumentException("Operator In is not supported with boolean expression");
    }

    public bool LessThan(bool left, string right)
    {
      throw new ArgumentException("Operator LessThan is not supported with boolean expression");
    }

    public bool LessThanEqual(bool left, string right)
    {
      throw new ArgumentException("Operator LessThanEqual is not supported with boolean expression");
    }

    public bool LikeRegex(bool left, string right, string contextJson)
    {
      throw new ArgumentException("Operator LikeRegex is not supported with boolean expression");
    }

    public bool NotEqual(bool left, string right)
    {
      bool boolean = Convert.ToBoolean(right, (IFormatProvider) CultureInfo.InvariantCulture);
      return left != boolean;
    }

    public bool NotIn(bool left, string right)
    {
      throw new ArgumentException("Operator notin is not supported with boolean expression");
    }

    public bool StartsWith(bool left, string right, string contextJson)
    {
      throw new ArgumentException("Operator StartsWith is not supported with boolean expression");
    }
  }
}
