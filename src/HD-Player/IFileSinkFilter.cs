// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.IFileSinkFilter
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using System.Runtime.InteropServices;
using System.Security;

namespace BlueStacks.Player
{
  [SuppressUnmanagedCodeSecurity]
  [Guid("a2104830-7c70-11cf-8bce-00aa00a3f1a6")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  public interface IFileSinkFilter
  {
    int SetFileName([MarshalAs(UnmanagedType.LPWStr), In] string pszFileName, [MarshalAs(UnmanagedType.LPStruct), In] AMMediaType pmt);

    int GetCurFile([MarshalAs(UnmanagedType.LPWStr)] out string pszFileName, [MarshalAs(UnmanagedType.LPStruct), Out] AMMediaType pmt);
  }
}
