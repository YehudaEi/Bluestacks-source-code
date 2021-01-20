// Decompiled with JetBrains decompiler
// Type: Range
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

public class Range
{
  public Range(long from, long to)
  {
    this.From = from;
    this.To = to;
  }

  public long From { get; }

  public long To { get; }

  public long Length
  {
    get
    {
      return this.To - this.From + 1L;
    }
  }
}
