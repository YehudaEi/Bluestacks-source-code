// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.IShellLink
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace BlueStacks.Common
{
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("000214F9-0000-0000-C000-000000000046")]
  [ComImport]
  public interface IShellLink
  {
    void GetPath([MarshalAs(UnmanagedType.LPWStr), Out] StringBuilder pszFile, int cchMaxPath, out WIN32_FIND_DATAW pfd, int fFlags);

    void GetIDList(out IntPtr ppidl);

    void SetIDList(IntPtr pidl);

    void GetDescription([MarshalAs(UnmanagedType.LPWStr), Out] StringBuilder pszName, int cchMaxName);

    void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string pszName);

    void GetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr), Out] StringBuilder pszDir, int cchMaxPath);

    void SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string pszDir);

    void GetArguments([MarshalAs(UnmanagedType.LPWStr), Out] StringBuilder pszArgs, int cchMaxPath);

    void SetArguments([MarshalAs(UnmanagedType.LPWStr)] string pszArgs);

    void GetHotkey(out short pwHotkey);

    void SetHotkey(short wHotkey);

    void GetShowCmd(out int piShowCmd);

    void SetShowCmd(int iShowCmd);

    void GetIconLocation([MarshalAs(UnmanagedType.LPWStr), Out] StringBuilder pszIconPath, int cchIconPath, out int piIcon);

    void SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string pszIconPath, int iIcon);

    void SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string pszPathRel, int dwReserved);

    void Resolve(IntPtr hwnd, int fFlags);

    void SetPath([MarshalAs(UnmanagedType.LPWStr)] string pszFile);
  }
}
