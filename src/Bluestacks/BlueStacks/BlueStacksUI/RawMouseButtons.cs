// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.RawMouseButtons
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using System;

namespace BlueStacks.BlueStacksUI
{
  [Flags]
  public enum RawMouseButtons : ushort
  {
    None = 0,
    LeftDown = 1,
    LeftUp = 2,
    RightDown = 4,
    RightUp = 8,
    MiddleDown = 16, // 0x0010
    MiddleUp = 32, // 0x0020
    Button4Down = 64, // 0x0040
    Button4Up = 128, // 0x0080
    Button5Down = 256, // 0x0100
    Button5Up = 512, // 0x0200
    MouseWheel = 1024, // 0x0400
  }
}
