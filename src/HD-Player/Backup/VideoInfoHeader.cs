// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.VideoInfoHeader
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using BlueStacks.Common;
using System.Runtime.InteropServices;

namespace BlueStacks.Player
{
  [StructLayout(LayoutKind.Sequential)]
  public class VideoInfoHeader
  {
    public RECT SrcRect;
    public RECT TargetRect;
    public int BitRate;
    public int BitErrorRate;
    public long AvgTimePerFrame;
    public BitmapInfoHeader BmiHeader;
  }
}
