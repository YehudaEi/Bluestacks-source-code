// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.Tuple`4
// Assembly: HD-ServiceInstaller, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 15F93427-26B3-4C7E-BAB1-0A00688BC4D4
// Assembly location: C:\Program Files\BlueStacks\HD-ServiceInstaller.exe

namespace BlueStacks.Common
{
  public class Tuple<T1, T2, T3, T4> : Tuple<T1, T2, T3>
  {
    public Tuple(T1 item1, T2 item2, T3 item3, T4 item4)
      : base(item1, item2, item3)
    {
      this.Item4 = item4;
    }

    public T4 Item4 { get; set; }
  }
}
