// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.ICreateDevEnum
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security;

namespace BlueStacks.Player
{
  [SuppressUnmanagedCodeSecurity]
  [Guid("29840822-5B84-11D0-BD3B-00A0C911CE86")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  public interface ICreateDevEnum
  {
    int CreateClassEnumerator([MarshalAs(UnmanagedType.LPStruct), In] Guid pType, out IEnumMoniker ppEnumMoniker, [MarshalAs(UnmanagedType.I4), In] int dwFlags);
  }
}
