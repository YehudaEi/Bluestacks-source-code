// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.Tuple`2
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

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
