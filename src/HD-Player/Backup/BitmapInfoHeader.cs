// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.BitmapInfoHeader
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using System.Runtime.InteropServices;

namespace BlueStacks.Player
{
  [StructLayout(LayoutKind.Sequential, Pack = 2)]
  public class BitmapInfoHeader
  {
    public int Size;
    public int Width;
    public int Height;
    public short Planes;
    public short BitCount;
    public int Compression;
    public int ImageSize;
    public int XPelsPerMeter;
    public int YPelsPerMeter;
    public int ClrUsed;
    public int ClrImportant;
  }
}
