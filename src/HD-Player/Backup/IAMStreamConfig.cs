// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.IAMStreamConfig
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

namespace BlueStacks.Player
{
  [SuppressUnmanagedCodeSecurity]
  [Guid("C6E13340-30AC-11d0-A18C-00A0C9118956")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  public interface IAMStreamConfig
  {
    [MethodImpl(MethodImplOptions.PreserveSig)]
    int SetFormat([MarshalAs(UnmanagedType.LPStruct), In] AMMediaType pmt);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int GetFormat(out AMMediaType pmt);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int GetNumberOfCapabilities(out int piCount, out int piSize);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int GetStreamCaps([In] int iIndex, out AMMediaType ppmt, [In] IntPtr pSCC);
  }
}
