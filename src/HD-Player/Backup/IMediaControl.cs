// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.IMediaControl
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using System;
using System.Runtime.InteropServices;
using System.Security;

namespace BlueStacks.Player
{
  [SuppressUnmanagedCodeSecurity]
  [Guid("56a868b1-0ad4-11ce-b03a-0020af0ba770")]
  [InterfaceType(ComInterfaceType.InterfaceIsDual)]
  [ComImport]
  public interface IMediaControl
  {
    int Run();

    int Pause();

    int Stop();

    int GetState([In] int msTimeout, out FilterState pfs);

    int RenderFile([MarshalAs(UnmanagedType.BStr), In] string strFilename);

    [Obsolete("MSDN: Intended for Visual Basic 6.0; not documented here.", false)]
    int AddSourceFilter([MarshalAs(UnmanagedType.BStr), In] string strFilename, [MarshalAs(UnmanagedType.IDispatch)] out object ppUnk);

    [Obsolete("MSDN: Intended for Visual Basic 6.0; not documented here.", false)]
    int get_FilterCollection([MarshalAs(UnmanagedType.IDispatch)] out object ppUnk);

    [Obsolete("MSDN: Intended for Visual Basic 6.0; not documented here.", false)]
    int get_RegFilterCollection([MarshalAs(UnmanagedType.IDispatch)] out object ppUnk);

    int StopWhenReady();
  }
}
