// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.Tuple`1
// Assembly: HD-ServiceInstaller, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 15F93427-26B3-4C7E-BAB1-0A00688BC4D4
// Assembly location: C:\Program Files\BlueStacks\HD-ServiceInstaller.exe

namespace BlueStacks.Common
{
  public class Tuple<T1>
  {
    public Tuple(T1 item1)
    {
      this.Item1 = item1;
    }

    public T1 Item1 { get; set; }
  }
}
