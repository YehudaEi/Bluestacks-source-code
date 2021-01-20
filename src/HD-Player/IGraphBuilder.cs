// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.IGraphBuilder
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using System;
using System.Runtime.InteropServices;
using System.Security;

namespace BlueStacks.Player
{
  [SuppressUnmanagedCodeSecurity]
  [Guid("56a868a9-0ad4-11ce-b03a-0020af0ba770")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  public interface IGraphBuilder : IFilterGraph
  {
    new int AddFilter([In] IBaseFilter pFilter, [MarshalAs(UnmanagedType.LPWStr), In] string pName);

    new int RemoveFilter([In] IBaseFilter pFilter);

    new int EnumFilters(out IEnumFilters ppEnum);

    new int FindFilterByName([MarshalAs(UnmanagedType.LPWStr), In] string pName, out IBaseFilter ppFilter);

    new int ConnectDirect([In] IPin ppinOut, [In] IPin ppinIn, [MarshalAs(UnmanagedType.LPStruct), In] AMMediaType pmt);

    new int Reconnect([In] IPin ppin);

    new int Disconnect([In] IPin ppin);

    new int SetDefaultSyncSource();

    int Connect([In] IPin ppinOut, [In] IPin ppinIn);

    int Render([In] IPin ppinOut);

    int RenderFile([MarshalAs(UnmanagedType.LPWStr), In] string lpcwstrFile, [MarshalAs(UnmanagedType.LPWStr), In] string lpcwstrPlayList);

    int AddSourceFilter([MarshalAs(UnmanagedType.LPWStr), In] string lpcwstrFileName, [MarshalAs(UnmanagedType.LPWStr), In] string lpcwstrFilterName, out IBaseFilter ppFilter);

    int SetLogFile(IntPtr hFile);

    int Abort();

    int ShouldOperationContinue();
  }
}
