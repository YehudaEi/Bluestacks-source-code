// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.Tuple
// Assembly: HD-ServiceInstaller, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 15F93427-26B3-4C7E-BAB1-0A00688BC4D4
// Assembly location: C:\Program Files\BlueStacks\HD-ServiceInstaller.exe

namespace BlueStacks.Common
{
  public static class Tuple
  {
    public static Tuple<T1> Create<T1>(T1 item1)
    {
      return new Tuple<T1>(item1);
    }

    public static Tuple<T1, T2> Create<T1, T2>(T1 item1, T2 item2)
    {
      return new Tuple<T1, T2>(item1, item2);
    }

    public static Tuple<T1, T2, T3> Create<T1, T2, T3>(T1 item1, T2 item2, T3 item3)
    {
      return new Tuple<T1, T2, T3>(item1, item2, item3);
    }
  }
}
