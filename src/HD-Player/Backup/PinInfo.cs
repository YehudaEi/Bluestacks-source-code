// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.PinInfo
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using System.Runtime.InteropServices;

namespace BlueStacks.Player
{
  [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
  public struct PinInfo
  {
    [MarshalAs(UnmanagedType.Interface)]
    public IBaseFilter filter;
    public PinDirection dir;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
    public string name;
  }
}
