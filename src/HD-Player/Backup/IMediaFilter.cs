// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.IMediaFilter
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using System;
using System.Runtime.InteropServices;
using System.Security;

namespace BlueStacks.Player
{
  [SuppressUnmanagedCodeSecurity]
  [Guid("56a86899-0ad4-11ce-b03a-0020af0ba770")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  public interface IMediaFilter : IPersist
  {
    new int GetClassID(out Guid pClassID);

    int Stop();

    int Pause();

    int Run([In] long tStart);

    int GetState([In] int dwMilliSecsTimeout, out FilterState State);

    int SetSyncSource([In] IReferenceClock pClock);

    int GetSyncSource(out IReferenceClock pClock);
  }
}
