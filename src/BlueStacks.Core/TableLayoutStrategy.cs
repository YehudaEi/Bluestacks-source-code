// Decompiled with JetBrains decompiler
// Type: BlueStacks.Core.TableLayoutStrategy
// Assembly: BlueStacks.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C36AABCB-E7F4-4E86-A72F-981C56431F94
// Assembly location: C:\Program Files\BlueStacks\BlueStacks.Core.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace BlueStacks.Core
{
  public class TableLayoutStrategy : ILayoutStrategy
  {
    private readonly List<double> _rowHeights = new List<double>();
    private int _columnCount;
    private double[] _colWidths;
    private int _elementCount;

    public Size ResultSize
    {
      get
      {
        return this._colWidths == null || !this._rowHeights.Any<double>() ? new Size(0.0, 0.0) : new Size(((IEnumerable<double>) this._colWidths).Sum(), this._rowHeights.Sum());
      }
    }

    public void Calculate(Size availableSize, Size[] measures)
    {
      this.BaseCalculation(availableSize, measures);
      this.AdjustEmptySpace(availableSize);
    }

    private void BaseCalculation(Size availableSize, Size[] measures)
    {
      if (measures == null)
        return;
      this._elementCount = measures.Length;
      this._columnCount = TableLayoutStrategy.GetColumnCount(availableSize, measures);
      if (this._colWidths == null || this._colWidths.Length < this._columnCount)
        this._colWidths = new double[this._columnCount];
      bool flag = true;
      while (flag)
      {
        flag = false;
        this.ResetSizes();
        for (int index1 = 0; index1 * this._columnCount < measures.Length; ++index1)
        {
          double val1 = 0.0;
          for (int index2 = 0; index2 < this._columnCount; ++index2)
          {
            int index3 = index1 * this._columnCount + index2;
            if (index3 < measures.Length)
            {
              this._colWidths[index2] = Math.Max(this._colWidths[index2], measures[index3].Width);
              val1 = Math.Max(val1, measures[index3].Height);
            }
            else
              break;
          }
          if (this._columnCount > 1 && ((IEnumerable<double>) this._colWidths).Sum() > availableSize.Width)
          {
            --this._columnCount;
            flag = true;
            break;
          }
          this._rowHeights.Add(val1);
        }
      }
    }

    public Rect GetPosition(int index)
    {
      int index1 = index % this._columnCount;
      int index2 = index / this._columnCount;
      double x = 0.0;
      for (int index3 = 0; index3 < index1; ++index3)
        x += this._colWidths[index3];
      double y = 0.0;
      for (int index3 = 0; index3 < index2; ++index3)
        y += this._rowHeights[index3];
      return new Rect(new Point(x, y), new Size(this._colWidths[index1], this._rowHeights[index2]));
    }

    public int GetIndex(Point position)
    {
      int index1 = 0;
      for (double num = 0.0; num < position.X && this._columnCount > index1; ++index1)
        num += this._colWidths[index1];
      int num1 = index1 - 1;
      int index2 = 0;
      for (double num2 = 0.0; num2 < position.Y && this._rowHeights.Count > index2; ++index2)
        num2 += this._rowHeights[index2];
      int num3 = index2 - 1;
      if (num3 < 0)
        num3 = 0;
      if (num1 < 0)
        num1 = 0;
      if (num1 >= this._columnCount)
        num1 = this._columnCount - 1;
      int num4 = num3 * this._columnCount + num1;
      if (num4 > this._elementCount)
        num4 = this._elementCount - 1;
      return num4;
    }

    private void AdjustEmptySpace(Size availableSize)
    {
      double num1 = ((IEnumerable<double>) this._colWidths).Sum();
      if (double.IsNaN(availableSize.Width) || availableSize.Width <= num1)
        return;
      double num2 = (availableSize.Width - num1) / (double) this._columnCount;
      for (int index = 0; index < this._columnCount; ++index)
        this._colWidths[index] += num2;
    }

    private void ResetSizes()
    {
      this._rowHeights.Clear();
      for (int index = 0; index < this._colWidths.Length; ++index)
        this._colWidths[index] = 0.0;
    }

    private static int GetColumnCount(Size availableSize, Size[] measures)
    {
      double num1 = 0.0;
      for (int val2 = 0; val2 < measures.Length; ++val2)
      {
        double num2 = num1 + measures[val2].Width;
        if (num2 > availableSize.Width)
          return Math.Max(1, val2);
        num1 = num2;
      }
      return measures.Length;
    }
  }
}
