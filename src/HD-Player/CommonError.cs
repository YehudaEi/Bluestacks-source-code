// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.CommonError
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace BlueStacks.Player
{
  public class CommonError
  {
    public static void ThrowLastWin32Error(string msg)
    {
      throw new SystemException(msg, (Exception) new Win32Exception(Marshal.GetLastWin32Error()));
    }
  }
}
