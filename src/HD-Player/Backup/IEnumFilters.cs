// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.IEnumFilters
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using System;
using System.Runtime.InteropServices;
using System.Security;

namespace BlueStacks.Player
{
  [SuppressUnmanagedCodeSecurity]
  [Guid("56a86893-0ad4-11ce-b03a-0020af0ba770")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  public interface IEnumFilters
  {
    int Next([In] int cFilters, [MarshalAs(UnmanagedType.LPArray), Out] IBaseFilter[] ppFilter, [In] IntPtr pcFetched);

    int Skip([In] int cFilters);

    int Reset();

    int Clone(out IEnumFilters ppEnum);
  }
}
