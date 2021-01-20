// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.ISampleGrabberCB
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
  [Guid("0579154A-2B53-4994-B0D0-E773148EFF85")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  public interface ISampleGrabberCB
  {
    [MethodImpl(MethodImplOptions.PreserveSig)]
    int SampleCB(double SampleTime, IMediaSample pSample);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    int BufferCB(double SampleTime, IntPtr pBuffer, int BufferLen);
  }
}
