// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.IBaseFilter
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using System;
using System.Runtime.InteropServices;
using System.Security;

namespace BlueStacks.Player
{
  [SuppressUnmanagedCodeSecurity]
  [Guid("56a86895-0ad4-11ce-b03a-0020af0ba770")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  public interface IBaseFilter : IMediaFilter, IPersist
  {
    new int GetClassID(out Guid pClassID);

    new int Stop();

    new int Pause();

    new int Run(long tStart);

    new int GetState([In] int dwMilliSecsTimeout, out FilterState filtState);

    new int SetSyncSource([In] IReferenceClock pClock);

    new int GetSyncSource(out IReferenceClock pClock);

    int EnumPins(out IEnumPins ppEnum);

    int FindPin([MarshalAs(UnmanagedType.LPWStr), In] string Id, out IPin ppPin);

    int QueryFilterInfo(out FilterInfo pInfo);

    int JoinFilterGraph([In] IFilterGraph pGraph, [MarshalAs(UnmanagedType.LPWStr), In] string pName);

    int QueryVendorInfo([MarshalAs(UnmanagedType.LPWStr)] out string pVendorInfo);
  }
}
