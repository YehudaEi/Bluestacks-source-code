// Decompiled with JetBrains decompiler
// Type: Range
// Assembly: HD-ServiceInstaller, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 15F93427-26B3-4C7E-BAB1-0A00688BC4D4
// Assembly location: C:\Program Files\BlueStacks\HD-ServiceInstaller.exe

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
