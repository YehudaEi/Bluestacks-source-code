// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.AMMediaType
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using System;
using System.Runtime.InteropServices;

namespace BlueStacks.Player
{
  [StructLayout(LayoutKind.Sequential)]
  public class AMMediaType
  {
    public Guid majorType;
    public Guid subType;
    [MarshalAs(UnmanagedType.Bool)]
    public bool fixedSizeSamples;
    [MarshalAs(UnmanagedType.Bool)]
    public bool temporalCompression;
    public int sampleSize;
    public Guid formatType;
    public IntPtr pUnk;
    public int cbFormat;
    public IntPtr pbFormat;
  }
}
