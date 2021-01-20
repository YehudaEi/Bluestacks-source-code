// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.COPYDATASTRUCT
// Assembly: HD-ServiceInstaller, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 15F93427-26B3-4C7E-BAB1-0A00688BC4D4
// Assembly location: C:\Program Files\BlueStacks\HD-ServiceInstaller.exe

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BlueStacks.Common
{
  public struct COPYDATASTRUCT
  {
    public IntPtr dwData { [IsReadOnly] get; set; }

    public int cbData { [IsReadOnly] get; set; }

    public IntPtr lpData { [IsReadOnly] get; set; }

    public static COPYDATASTRUCT CreateForString(int dwData, string value, bool _ = false)
    {
      return new COPYDATASTRUCT()
      {
        dwData = (IntPtr) dwData,
        cbData = (value != null ? value.Length : 1) * 2,
        lpData = Marshal.StringToHGlobalUni(value)
      };
    }
  }
}
