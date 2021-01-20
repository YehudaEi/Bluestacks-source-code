// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.ISampleGrabber
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using System;
using System.Runtime.InteropServices;
using System.Security;

namespace BlueStacks.Player
{
  [SuppressUnmanagedCodeSecurity]
  [Guid("6B652FFF-11FE-4fce-92AD-0266B5D7C78F")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  public interface ISampleGrabber
  {
    int SetOneShot([MarshalAs(UnmanagedType.Bool), In] bool OneShot);

    int SetMediaType([MarshalAs(UnmanagedType.LPStruct), In] AMMediaType pmt);

    int GetConnectedMediaType([MarshalAs(UnmanagedType.LPStruct), Out] AMMediaType pmt);

    int SetBufferSamples([MarshalAs(UnmanagedType.Bool), In] bool BufferThem);

    int GetCurrentBuffer(ref int pBufferSize, IntPtr pBuffer);

    int GetCurrentSample(out IMediaSample ppSample);

    int SetCallback(ISampleGrabberCB pCallback, int WhichMethodToCallback);
  }
}
