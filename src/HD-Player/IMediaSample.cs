// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.IMediaSample
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using System;
using System.Runtime.InteropServices;
using System.Security;

namespace BlueStacks.Player
{
  [SuppressUnmanagedCodeSecurity]
  [Guid("56a8689a-0ad4-11ce-b03a-0020af0ba770")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  public interface IMediaSample
  {
    int GetPointer(out IntPtr ppBuffer);

    int GetSize();

    int GetTime(out long pTimeStart, out long pTimeEnd);

    int SetTime([In] long pTimeStart, [In] long pTimeEnd);

    int IsSyncPoint();

    int SetSyncPoint([MarshalAs(UnmanagedType.Bool), In] bool bIsSyncPoint);

    int IsPreroll();

    int SetPreroll([MarshalAs(UnmanagedType.Bool), In] bool bIsPreroll);

    int GetActualDataLength();

    int SetActualDataLength([In] int len);

    int GetMediaType([MarshalAs(UnmanagedType.LPStruct)] out AMMediaType ppMediaType);

    int SetMediaType([MarshalAs(UnmanagedType.LPStruct), In] AMMediaType pMediaType);

    int IsDiscontinuity();

    int SetDiscontinuity([MarshalAs(UnmanagedType.Bool), In] bool bDiscontinuity);

    int GetMediaTime(out long pTimeStart, out long pTimeEnd);

    int SetMediaTime([In] long pTimeStart, [In] long pTimeEnd);
  }
}
