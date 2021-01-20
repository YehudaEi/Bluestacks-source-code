// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.Tuple`2
// Assembly: HD-ServiceInstaller, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 15F93427-26B3-4C7E-BAB1-0A00688BC4D4
// Assembly location: C:\Program Files\BlueStacks\HD-ServiceInstaller.exe

namespace BlueStacks.Common
{
  public class Tuple<T1, T2> : Tuple<T1>
  {
    public Tuple(T1 item1, T2 item2)
      : base(item1)
    {
      this.Item2 = item2;
    }

    public T2 Item2 { get; set; }
  }
}
