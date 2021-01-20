// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.ICaptureGraphBuilder2
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using System;
using System.Runtime.InteropServices;
using System.Security;

namespace BlueStacks.Player
{
  [SuppressUnmanagedCodeSecurity]
  [Guid("93E5A4E0-2D50-11d2-ABFA-00A0C9C6E38D")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  public interface ICaptureGraphBuilder2
  {
    int SetFiltergraph([In] IGraphBuilder pfg);

    int GetFiltergraph(out IGraphBuilder ppfg);

    int SetOutputFileName(
      [MarshalAs(UnmanagedType.LPStruct), In] Guid pType,
      [MarshalAs(UnmanagedType.LPWStr), In] string lpstrFile,
      out IBaseFilter ppbf,
      out IFileSinkFilter ppSink);

    int FindInterface([MarshalAs(UnmanagedType.LPStruct), In] Guid pCategory, [MarshalAs(UnmanagedType.LPStruct), In] Guid pType, [In] IBaseFilter pf, [MarshalAs(UnmanagedType.LPStruct), In] Guid riid, [MarshalAs(UnmanagedType.IUnknown)] out object ppint);

    int RenderStream(
      [MarshalAs(UnmanagedType.LPStruct), In] Guid pCategory,
      [MarshalAs(UnmanagedType.LPStruct), In] Guid pType,
      [MarshalAs(UnmanagedType.IUnknown), In] object pSource,
      [In] IBaseFilter pCompressor,
      [In] IBaseFilter pRenderer);

    int ControlStream(
      [MarshalAs(UnmanagedType.LPStruct), In] Guid pCategory,
      [MarshalAs(UnmanagedType.LPStruct), In] Guid pType,
      [MarshalAs(UnmanagedType.Interface), In] IBaseFilter pFilter,
      [In] long pstart,
      [In] long pstop,
      [In] short wStartCookie,
      [In] short wStopCookie);

    int AllocCapFile([MarshalAs(UnmanagedType.LPWStr), In] string lpstr, [In] long dwlSize);

    int CopyCaptureFile(
      [MarshalAs(UnmanagedType.LPWStr), In] string lpwstrOld,
      [MarshalAs(UnmanagedType.LPWStr), In] string lpwstrNew,
      [MarshalAs(UnmanagedType.Bool), In] bool fAllowEscAbort,
      [In] IAMCopyCaptureFileProgress pCallback);

    int FindPin(
      [MarshalAs(UnmanagedType.IUnknown), In] object pSource,
      [In] PinDirection pindir,
      [MarshalAs(UnmanagedType.LPStruct), In] Guid pCategory,
      [MarshalAs(UnmanagedType.LPStruct), In] Guid pType,
      [MarshalAs(UnmanagedType.Bool), In] bool fUnconnected,
      [In] int num,
      [MarshalAs(UnmanagedType.Interface)] out IPin ppPin);
  }
}
