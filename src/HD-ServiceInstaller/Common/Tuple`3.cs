// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.Tuple`3
// Assembly: HD-ServiceInstaller, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 15F93427-26B3-4C7E-BAB1-0A00688BC4D4
// Assembly location: C:\Program Files\BlueStacks\HD-ServiceInstaller.exe

namespace BlueStacks.Common
{
  public class Tuple<T1, T2, T3> : Tuple<T1, T2>
  {
    public Tuple(T1 item1, T2 item2, T3 item3)
      : base(item1, item2)
    {
      this.Item3 = item3;
    }

    public T3 Item3 { get; set; }
  }
}
