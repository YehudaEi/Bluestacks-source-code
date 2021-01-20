// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.IFilterGraph2
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security;

namespace BlueStacks.Player
{
  [SuppressUnmanagedCodeSecurity]
  [Guid("36b73882-c2c8-11cf-8b46-00805f6cef60")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  public interface IFilterGraph2 : IGraphBuilder, IFilterGraph
  {
    new int AddFilter([In] IBaseFilter pFilter, [MarshalAs(UnmanagedType.LPWStr), In] string pName);

    new int RemoveFilter([In] IBaseFilter pFilter);

    new int EnumFilters(out IEnumFilters ppEnum);

    new int FindFilterByName([MarshalAs(UnmanagedType.LPWStr), In] string pName, out IBaseFilter ppFilter);

    new int ConnectDirect([In] IPin ppinOut, [In] IPin ppinIn, [MarshalAs(UnmanagedType.LPStruct), In] AMMediaType pmt);

    new int Reconnect([In] IPin ppin);

    new int Disconnect([In] IPin ppin);

    new int SetDefaultSyncSource();

    new int Connect([In] IPin ppinOut, [In] IPin ppinIn);

    new int Render([In] IPin ppinOut);

    new int RenderFile([MarshalAs(UnmanagedType.LPWStr), In] string lpcwstrFile, [MarshalAs(UnmanagedType.LPWStr), In] string lpcwstrPlayList);

    new int AddSourceFilter(
      [MarshalAs(UnmanagedType.LPWStr), In] string lpcwstrFileName,
      [MarshalAs(UnmanagedType.LPWStr), In] string lpcwstrFilterName,
      out IBaseFilter ppFilter);

    new int SetLogFile(IntPtr hFile);

    new int Abort();

    new int ShouldOperationContinue();

    int AddSourceFilterForMoniker(
      [In] IMoniker pMoniker,
      [In] IBindCtx pCtx,
      [MarshalAs(UnmanagedType.LPWStr), In] string lpcwstrFilterName,
      out IBaseFilter ppFilter);

    int ReconnectEx([In] IPin ppin, [In] AMMediaType pmt);

    int RenderEx([In] IPin pPinOut, [In] int dwFlags, [In] IntPtr pvContext);
  }
}
