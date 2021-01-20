// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.Tuple`3
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

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
