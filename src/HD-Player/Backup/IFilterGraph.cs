// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.IFilterGraph
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using System.Runtime.InteropServices;
using System.Security;

namespace BlueStacks.Player
{
  [SuppressUnmanagedCodeSecurity]
  [Guid("56a8689f-0ad4-11ce-b03a-0020af0ba770")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  public interface IFilterGraph
  {
    int AddFilter([In] IBaseFilter pFilter, [MarshalAs(UnmanagedType.LPWStr), In] string pName);

    int RemoveFilter([In] IBaseFilter pFilter);

    int EnumFilters(out IEnumFilters ppEnum);

    int FindFilterByName([MarshalAs(UnmanagedType.LPWStr), In] string pName, out IBaseFilter ppFilter);

    int ConnectDirect([In] IPin ppinOut, [In] IPin ppinIn, [MarshalAs(UnmanagedType.LPStruct), In] AMMediaType pmt);

    int Reconnect([In] IPin ppin);

    int Disconnect([In] IPin ppin);

    int SetDefaultSyncSource();
  }
}
