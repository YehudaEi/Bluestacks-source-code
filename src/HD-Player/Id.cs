// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.Id
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using BlueStacks.Common;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

namespace BlueStacks.Player
{
  internal class Id
  {
    private const uint HKEY_LOCAL_MACHINE = 2147483650;
    private const uint KEY_READ = 131097;
    private const uint KEY_WOW64_64KEY = 256;

    [DllImport("advapi32.dll", SetLastError = true)]
    private static extern int RegOpenKeyEx(
      IntPtr hKey,
      string lpSubKey,
      uint ulOptions,
      uint samDesired,
      ref UIntPtr phkResult);

    [DllImport("advapi32.dll", SetLastError = true)]
    private static extern int RegQueryValueEx(
      UIntPtr hKey,
      string lpValueName,
      int lpReserved,
      ref RegistryValueKind lpType,
      IntPtr lpData,
      ref int lpcbData);

    [DllImport("advapi32.dll", SetLastError = true)]
    private static extern int RegCloseKey(UIntPtr hKey);

    public static string GenerateID()
    {
      string str = "";
      bool flag = true;
      try
      {
        string prog = Encoding.UTF8.GetString(new byte[4]
        {
          Convert.ToByte('w'),
          Convert.ToByte('m'),
          Convert.ToByte('i'),
          Convert.ToByte('c')
        });
        string args = Encoding.UTF8.GetString(new byte[18]
        {
          Convert.ToByte('c'),
          Convert.ToByte('s'),
          Convert.ToByte('p'),
          Convert.ToByte('r'),
          Convert.ToByte('o'),
          Convert.ToByte('d'),
          Convert.ToByte('u'),
          Convert.ToByte('c'),
          Convert.ToByte('t'),
          Convert.ToByte(' '),
          Convert.ToByte('g'),
          Convert.ToByte('e'),
          Convert.ToByte('t'),
          Convert.ToByte(' '),
          Convert.ToByte('U'),
          Convert.ToByte('U'),
          Convert.ToByte('I'),
          Convert.ToByte('D')
        });
        string oldValue = Encoding.UTF8.GetString(new byte[4]
        {
          Convert.ToByte('U'),
          Convert.ToByte('U'),
          Convert.ToByte('I'),
          Convert.ToByte('D')
        });
        str = Utils.RunCmdNoLog(prog, args, 3000);
        str = str.Replace(oldValue, "").Trim();
        str = str.Replace("\n", "");
        str = str.Replace("\r", "");
        str = str.Replace("\t", "");
        str = str.Replace(" ", "");
        foreach (char ch in str)
        {
          switch (ch)
          {
            case '-':
            case 'F':
              continue;
            default:
              flag = false;
              goto label_7;
          }
        }
      }
      catch
      {
        Logger.Error("Unable to query intended string");
      }
label_7:
      if (str != string.Empty)
      {
        if (flag)
        {
          try
          {
            string prog = Encoding.UTF8.GetString(new byte[4]
            {
              Convert.ToByte('w'),
              Convert.ToByte('m'),
              Convert.ToByte('i'),
              Convert.ToByte('c')
            });
            string args = Encoding.UTF8.GetString(new byte[21]
            {
              Convert.ToByte('b'),
              Convert.ToByte('i'),
              Convert.ToByte('o'),
              Convert.ToByte('s'),
              Convert.ToByte(' '),
              Convert.ToByte('g'),
              Convert.ToByte('e'),
              Convert.ToByte('t'),
              Convert.ToByte(' '),
              Convert.ToByte('s'),
              Convert.ToByte('e'),
              Convert.ToByte('r'),
              Convert.ToByte('i'),
              Convert.ToByte('a'),
              Convert.ToByte('l'),
              Convert.ToByte('n'),
              Convert.ToByte('u'),
              Convert.ToByte('m'),
              Convert.ToByte('b'),
              Convert.ToByte('e'),
              Convert.ToByte('r')
            });
            string oldValue = Encoding.UTF8.GetString(new byte[12]
            {
              Convert.ToByte('S'),
              Convert.ToByte('e'),
              Convert.ToByte('r'),
              Convert.ToByte('i'),
              Convert.ToByte('a'),
              Convert.ToByte('l'),
              Convert.ToByte('N'),
              Convert.ToByte('u'),
              Convert.ToByte('m'),
              Convert.ToByte('b'),
              Convert.ToByte('e'),
              Convert.ToByte('r')
            });
            str = Utils.RunCmdNoLog(prog, args, 3000);
            str = str.Replace(oldValue, "").Trim();
            str = str.Replace("\n", "");
            str = str.Replace("\r", "");
            str = str.Replace("\t", "");
            str = str.Replace(" ", "");
          }
          catch
          {
            Logger.Error("Unable to query another intended string");
          }
        }
      }
      if (str == string.Empty)
        str = Id.FallBackID();
      return str;
    }

    private static string FallBackID()
    {
      IntPtr hKey = (IntPtr) -2147483646;
      UIntPtr zero = UIntPtr.Zero;
      string lpSubKey = Encoding.UTF8.GetString(new byte[31]
      {
        Convert.ToByte('S'),
        Convert.ToByte('o'),
        Convert.ToByte('f'),
        Convert.ToByte('t'),
        Convert.ToByte('w'),
        Convert.ToByte('a'),
        Convert.ToByte('r'),
        Convert.ToByte('e'),
        Convert.ToByte('\\'),
        Convert.ToByte('M'),
        Convert.ToByte('i'),
        Convert.ToByte('c'),
        Convert.ToByte('r'),
        Convert.ToByte('o'),
        Convert.ToByte('s'),
        Convert.ToByte('o'),
        Convert.ToByte('f'),
        Convert.ToByte('t'),
        Convert.ToByte('\\'),
        Convert.ToByte('C'),
        Convert.ToByte('r'),
        Convert.ToByte('y'),
        Convert.ToByte('p'),
        Convert.ToByte('t'),
        Convert.ToByte('o'),
        Convert.ToByte('g'),
        Convert.ToByte('r'),
        Convert.ToByte('a'),
        Convert.ToByte('p'),
        Convert.ToByte('h'),
        Convert.ToByte('y')
      });
      int error1 = Id.RegOpenKeyEx(hKey, lpSubKey, 0U, 131353U, ref zero);
      if (error1 != 0)
        throw new ApplicationException("Cannot open 64-bit HKLM\\Software", (Exception) new Win32Exception(error1));
      int lpcbData = 0;
      RegistryValueKind lpType = RegistryValueKind.Unknown;
      string lpValueName = Encoding.UTF8.GetString(new byte[11]
      {
        Convert.ToByte('M'),
        Convert.ToByte('a'),
        Convert.ToByte('c'),
        Convert.ToByte('h'),
        Convert.ToByte('i'),
        Convert.ToByte('n'),
        Convert.ToByte('e'),
        Convert.ToByte('G'),
        Convert.ToByte('u'),
        Convert.ToByte('i'),
        Convert.ToByte('d')
      });
      Id.RegQueryValueEx(zero, lpValueName, 0, ref lpType, IntPtr.Zero, ref lpcbData);
      IntPtr num = Marshal.AllocHGlobal(lpcbData);
      int error2 = Id.RegQueryValueEx(zero, lpValueName, 0, ref lpType, num, ref lpcbData);
      if (error2 != 0)
        throw new ApplicationException("Cannot read 64-bit registry", (Exception) new Win32Exception(error2));
      string stringAnsi = Marshal.PtrToStringAnsi(num);
      if (num != IntPtr.Zero)
        Marshal.FreeHGlobal(num);
      Id.RegCloseKey(zero);
      return stringAnsi;
    }
  }
}
