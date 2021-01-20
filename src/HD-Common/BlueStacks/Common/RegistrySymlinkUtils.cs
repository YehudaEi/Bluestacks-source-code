// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.RegistrySymlinkUtils
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;

namespace BlueStacks.Common
{
  public static class RegistrySymlinkUtils
  {
    private const uint REG_LINK = 6;
    private const uint HKEY_LOCAL_MACHINE = 2147483650;
    private const uint REG_OPTION_CREATE_LINK = 2;
    private const uint REG_OPTION_OPEN_LINK = 8;
    private const uint KEY_ALL_ACCESS = 983103;
    private const uint KEY_WOW64_64KEY = 256;
    private const uint KEY_QUERY_VALUE = 1;

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool IsWow64Process(IntPtr proc, ref bool isWow);

    [DllImport("advapi32.dll", SetLastError = true)]
    private static extern int RegOpenKeyEx(
      IntPtr hKey,
      string lpSubKey,
      uint ulOptions,
      uint samDesired,
      ref IntPtr phkResult);

    [DllImport("advapi32.dll", SetLastError = true)]
    private static extern int RegCreateKeyEx(
      IntPtr hKey,
      string lpSubKey,
      uint Reserved,
      string lpClass,
      uint dwOptions,
      uint samDesired,
      IntPtr lpSecurityAttributes,
      ref IntPtr phkResult,
      ref uint lpdwDisposition);

    [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern int RegSetValueEx(
      IntPtr hKey,
      string lpValueName,
      uint Reserved,
      uint dwType,
      string lpData,
      int cbData);

    [DllImport("ntdll.dll", SetLastError = true)]
    private static extern int ZwDeleteKey(IntPtr hKey);

    [DllImport("advapi32.dll", SetLastError = true)]
    private static extern int RegDeleteValue(IntPtr hKey, string lpValueName);

    [DllImport("advapi32.dll", SetLastError = true)]
    private static extern int RegCloseKey(IntPtr hKey);

    public static bool SymlinkCreator()
    {
      if (RegistrySymlinkUtils.IsOs64Bit())
      {
        try
        {
          RegistrySymlinkUtils.RemoveRegistrySymlink();
        }
        catch (Exception ex)
        {
        }
        RegistrySymlinkUtils.CreateRegistrySymlink();
      }
      return true;
    }

    public static bool IsOs64Bit()
    {
      switch (IntPtr.Size)
      {
        case 4:
          if (!RegistrySymlinkUtils.Is32BitProcessOn64BitProcessor())
            break;
          goto case 8;
        case 8:
          return true;
      }
      return false;
    }

    private static bool Is32BitProcessOn64BitProcessor()
    {
      bool isWow = false;
      RegistrySymlinkUtils.IsWow64Process(Process.GetCurrentProcess().Handle, ref isWow);
      return isWow;
    }

    public static void CreateRegistrySymlink()
    {
      IntPtr hKey = (IntPtr) -2147483646;
      IntPtr zero1 = IntPtr.Zero;
      IntPtr zero2 = IntPtr.Zero;
      uint lpdwDisposition = 0;
      ref IntPtr local = ref zero1;
      int error1 = RegistrySymlinkUtils.RegOpenKeyEx(hKey, "Software", 0U, 983359U, ref local);
      if (error1 != 0)
        throw new ApplicationException("Cannot open 64-bit HKLM\\Software", (Exception) new Win32Exception(error1));
      try
      {
        int keyEx = RegistrySymlinkUtils.RegCreateKeyEx(zero1, "BlueStacks" + Strings.GetOemTag(), 0U, (string) null, 2U, 983359U, IntPtr.Zero, ref zero2, ref lpdwDisposition);
        if (keyEx != 0)
          throw new ApplicationException("Cannot create 64-bit registry", (Exception) new Win32Exception(keyEx));
        string lpData = "\\Registry\\Machine\\Software\\Wow6432Node\\BlueStacks" + Strings.GetOemTag();
        int error2 = RegistrySymlinkUtils.RegSetValueEx(zero2, "SymbolicLinkValue", 0U, 6U, lpData, lpData.Length * 2);
        if (error2 != 0)
          throw new ApplicationException("Cannot set registry symlink value for target" + lpData, (Exception) new Win32Exception(error2));
      }
      finally
      {
        RegistrySymlinkUtils.RegCloseKey(zero1);
        if (zero2 != IntPtr.Zero)
          RegistrySymlinkUtils.RegCloseKey(zero2);
      }
    }

    public static void RemoveRegistrySymlink()
    {
      IntPtr hKey = (IntPtr) -2147483646;
      IntPtr zero1 = IntPtr.Zero;
      IntPtr zero2 = IntPtr.Zero;
      ref IntPtr local = ref zero1;
      int error1 = RegistrySymlinkUtils.RegOpenKeyEx(hKey, "Software", 0U, 983359U, ref local);
      if (error1 != 0)
        throw new ApplicationException("Cannot open 64-bit HKLM\\Software", (Exception) new Win32Exception(error1));
      try
      {
        int error2 = RegistrySymlinkUtils.RegOpenKeyEx(zero1, "BlueStacks" + Strings.GetOemTag(), 8U, 983359U, ref zero2);
        if (error2 != 0)
          throw new ApplicationException("Cannot open 64-bit registry", (Exception) new Win32Exception(error2));
        int num = RegistrySymlinkUtils.RegDeleteValue(zero2, "SymbolicLinkValue");
        num = RegistrySymlinkUtils.ZwDeleteKey(zero2);
      }
      finally
      {
        RegistrySymlinkUtils.RegCloseKey(zero1);
        if (zero2 != IntPtr.Zero)
          RegistrySymlinkUtils.RegCloseKey(zero2);
      }
    }

    public static bool IsSymlinkPresent()
    {
      if (!RegistrySymlinkUtils.IsOs64Bit())
        return false;
      try
      {
        IntPtr hKey = (IntPtr) -2147483646;
        IntPtr zero = IntPtr.Zero;
        ref IntPtr local = ref zero;
        int num1 = RegistrySymlinkUtils.RegOpenKeyEx(hKey, "Software", 0U, 257U, ref local);
        if (num1 != 0)
          throw new ApplicationException("Cannot open 64-bit HKLM\\Software: 0x" + num1.ToString("x", (IFormatProvider) CultureInfo.InvariantCulture));
        int num2 = RegistrySymlinkUtils.RegOpenKeyEx(zero, "BlueStacks" + Strings.GetOemTag(), 8U, 257U, ref zero);
        if (num2 != 0)
          throw new ApplicationException("Cannot open 64-bit registry: 0x" + num2.ToString("x", (IFormatProvider) CultureInfo.InvariantCulture));
        return true;
      }
      catch (Exception ex)
      {
        Logger.Warning("Some error while detecting symlink. Ex: {0}", (object) ex.Message);
        return false;
      }
    }
  }
}
