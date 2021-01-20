// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.COPYGAMEPADDATASTRUCT
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.Runtime.CompilerServices;

namespace BlueStacks.Common
{
  public struct COPYGAMEPADDATASTRUCT
  {
    public IntPtr dwData { [IsReadOnly] get; set; }

    public int size { [IsReadOnly] get; set; }

    public IntPtr lpData { [IsReadOnly] get; set; }
  }
}
