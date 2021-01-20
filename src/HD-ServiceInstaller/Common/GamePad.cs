// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.GamePad
// Assembly: HD-ServiceInstaller, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 15F93427-26B3-4C7E-BAB1-0A00688BC4D4
// Assembly location: C:\Program Files\BlueStacks\HD-ServiceInstaller.exe

using System.Runtime.CompilerServices;

namespace BlueStacks.Common
{
  public struct GamePad
  {
    public int X { [IsReadOnly] get; set; }

    public int Y { [IsReadOnly] get; set; }

    public int Z { [IsReadOnly] get; set; }

    public int Rx { [IsReadOnly] get; set; }

    public int Ry { [IsReadOnly] get; set; }

    public int Rz { [IsReadOnly] get; set; }

    public int Hat { [IsReadOnly] get; set; }

    public uint Mask { [IsReadOnly] get; set; }
  }
}
