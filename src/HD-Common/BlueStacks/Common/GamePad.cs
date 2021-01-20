// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.GamePad
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

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
