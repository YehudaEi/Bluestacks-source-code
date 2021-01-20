// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.IEnumMediaTypes
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using System;
using System.Runtime.InteropServices;
using System.Security;

namespace BlueStacks.Player
{
  [SuppressUnmanagedCodeSecurity]
  [Guid("89c31040-846b-11ce-97d3-00aa0055595a")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  public interface IEnumMediaTypes
  {
    int Next([In] int cMediaTypes, [MarshalAs(UnmanagedType.LPArray), In, Out] AMMediaType[] ppMediaTypes, [In] IntPtr pcFetched);

    int Skip([In] int cMediaTypes);

    int Reset();

    int Clone(out IEnumMediaTypes ppEnum);
  }
}
