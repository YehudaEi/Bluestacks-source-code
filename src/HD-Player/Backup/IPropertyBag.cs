// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.IPropertyBag
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using System.Runtime.InteropServices;
using System.Security;

namespace BlueStacks.Player
{
  [SuppressUnmanagedCodeSecurity]
  [Guid("55272A00-42CB-11CE-8135-00AA004BB851")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  public interface IPropertyBag
  {
    int Read([MarshalAs(UnmanagedType.LPWStr), In] string pszPropName, [MarshalAs(UnmanagedType.Struct)] out object pVar, [In] IErrorLog pErrorLog);

    int Write([MarshalAs(UnmanagedType.LPWStr), In] string pszPropName, [MarshalAs(UnmanagedType.Struct), In] ref object pVar);
  }
}
