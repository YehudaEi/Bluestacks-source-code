// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.IErrorLog
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using System.Runtime.InteropServices;
using System.Security;

namespace BlueStacks.Player
{
  [SuppressUnmanagedCodeSecurity]
  [Guid("3127CA40-446E-11CE-8135-00AA004BB851")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  public interface IErrorLog
  {
    int AddError([MarshalAs(UnmanagedType.LPWStr), In] string pszPropName, [In] System.Runtime.InteropServices.ComTypes.EXCEPINFO pExcepInfo);
  }
}
