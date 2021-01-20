// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.RawMouseFlags
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using System;

namespace BlueStacks.BlueStacksUI
{
  [Flags]
  public enum RawMouseFlags : ushort
  {
    MoveRelative = 0,
    MoveAbsolute = 1,
    VirtualDesktop = 2,
    AttributesChanged = 4,
  }
}
