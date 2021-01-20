// Decompiled with JetBrains decompiler
// Type: NaturalSortComparer
// Assembly: BlueStacks.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C36AABCB-E7F4-4E86-A72F-981C56431F94
// Assembly location: C:\Program Files\BlueStacks\BlueStacks.Core.dll

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class NaturalSortComparer : Comparer<string>
{
  private Dictionary<string, string[]> splitTable;

  public NaturalSortComparer()
  {
    this.splitTable = new Dictionary<string, string[]>();
  }

  public override int Compare(string x, string y)
  {
    if (string.IsNullOrEmpty(x) || string.IsNullOrEmpty(y) || x == y)
      return 0;
    string[] strArray1;
    if (!this.splitTable.TryGetValue(x, out strArray1))
    {
      strArray1 = Regex.Split(x.Replace(" ", ""), "([0-9]+)");
      this.splitTable.Add(x, strArray1);
    }
    string[] strArray2;
    if (!this.splitTable.TryGetValue(y, out strArray2))
    {
      strArray2 = Regex.Split(y.Replace(" ", ""), "([0-9]+)");
      this.splitTable.Add(y, strArray2);
    }
    for (int index = 0; index < strArray1.Length && index < strArray2.Length; ++index)
    {
      int num = NaturalSortComparer.PartCompare(strArray1[index], strArray2[index]);
      if (num != 0)
        return num;
    }
    return strArray1.Length == strArray2.Length ? x.Length.CompareTo(y.Length) : strArray1.Length.CompareTo(strArray2.Length);
  }

  private static int PartCompare(string left, string right)
  {
    int result1;
    int result2;
    return !int.TryParse(left, out result1) || !int.TryParse(right, out result2) ? string.Compare(left, right, StringComparison.InvariantCultureIgnoreCase) : result1.CompareTo(result2);
  }
}
