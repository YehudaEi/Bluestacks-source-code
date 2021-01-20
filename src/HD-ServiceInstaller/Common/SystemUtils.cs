// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.SystemUtils
// Assembly: HD-ServiceInstaller, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 15F93427-26B3-4C7E-BAB1-0A00688BC4D4
// Assembly location: C:\Program Files\BlueStacks\HD-ServiceInstaller.exe

using Microsoft.VisualBasic.Devices;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Management;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace BlueStacks.Common
{
  public static class SystemUtils
  {
    private static int currentDPI = int.MinValue;
    public const int DEFAULT_DPI = 96;

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool IsWow64Process(IntPtr proc, ref bool isWow);

    [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern int GetDeviceCaps(IntPtr hDC, int nIndex);

    public static bool IsOSWinXP()
    {
      return Environment.OSVersion.Version.Major == 5;
    }

    public static bool IsOSVista()
    {
      return Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor == 0;
    }

    public static bool IsOSWin7()
    {
      return Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor == 1;
    }

    public static bool IsOSWin8()
    {
      return Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor == 2;
    }

    public static bool IsOSWin81()
    {
      return Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor == 3;
    }

    private static bool IsOSWin10()
    {
      return ((string) RegistryUtils.GetRegistryValue("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion", "ProductName", (object) "", RegistryKeyKind.HKEY_LOCAL_MACHINE)).Contains("Windows 10");
    }

    public static int GetOSArchitecture()
    {
      string environmentVariable = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE", EnvironmentVariableTarget.Machine);
      return !string.IsNullOrEmpty(environmentVariable) && string.Compare(environmentVariable, 0, "x86", 0, 3, StringComparison.OrdinalIgnoreCase) != 0 ? 64 : 32;
    }

    public static bool GetOSInfo(out string osName, out string servicePack, out string osArch)
    {
      osName = "";
      servicePack = "";
      osArch = "";
      OperatingSystem osVersion = Environment.OSVersion;
      System.Version version = osVersion.Version;
      if (osVersion.Platform == PlatformID.Win32Windows)
      {
        switch (version.Minor)
        {
          case 0:
            osName = "95";
            break;
          case 10:
            int revision = version.Revision;
            osName = !(revision.ToString((IFormatProvider) CultureInfo.InvariantCulture) == "2222A") ? "98" : "98SE";
            break;
          case 90:
            osName = "Me";
            break;
        }
      }
      else if (osVersion.Platform == PlatformID.Win32NT)
      {
        switch (version.Major)
        {
          case 3:
            osName = "NT 3.51";
            break;
          case 4:
            osName = "NT 4.0";
            break;
          case 5:
            osName = version.Minor != 0 ? "XP" : "2000";
            break;
          case 6:
            if (version.Minor == 0)
            {
              osName = "Vista";
              break;
            }
            if (version.Minor == 1)
            {
              osName = "7";
              break;
            }
            if (version.Minor == 2)
            {
              osName = "8";
              break;
            }
            if (version.Minor == 3)
            {
              osName = "8.1";
              break;
            }
            break;
          case 10:
            osName = "10";
            break;
        }
      }
      string str1 = osName;
      if (string.IsNullOrEmpty(str1))
        return false;
      string str2 = "Windows " + str1;
      if (!string.IsNullOrEmpty(osVersion.ServicePack))
      {
        servicePack = osVersion.ServicePack.Substring(osVersion.ServicePack.LastIndexOf(' ') + 1);
        str2 = str2 + " " + osVersion.ServicePack;
      }
      osArch = SystemUtils.GetOSArchitecture().ToString((IFormatProvider) CultureInfo.InvariantCulture) + "-bit";
      Logger.Info("Operating system details: " + (str2 + " " + osArch));
      return true;
    }

    public static ulong GetSystemTotalPhysicalMemory()
    {
      ulong num = 0;
      try
      {
        num = ulong.Parse(new ComputerInfo().TotalPhysicalMemory.ToString((IFormatProvider) CultureInfo.InvariantCulture), (IFormatProvider) CultureInfo.InvariantCulture);
      }
      catch (Exception ex)
      {
        Logger.Error("Couldn't get TotalPhysicalMemory, Ex: {0}", (object) ex.Message);
      }
      return num;
    }

    public static bool IsOs64Bit()
    {
      switch (IntPtr.Size)
      {
        case 4:
          if (!SystemUtils.Is32BitProcessOn64BitProcessor())
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
      SystemUtils.IsWow64Process(Process.GetCurrentProcess().Handle, ref isWow);
      return isWow;
    }

    public static DateTime FromUnixEpochToLocal(long secs)
    {
      return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds((double) secs).ToLocalTime();
    }

    public static int CurrentDPI
    {
      get
      {
        if (SystemUtils.currentDPI.Equals(int.MinValue))
          SystemUtils.currentDPI = SystemUtils.GetDPI();
        return SystemUtils.currentDPI;
      }
    }

    public static int GetDPI()
    {
      Logger.Info("Getting DPI");
      IntPtr hdc = Graphics.FromHwnd(IntPtr.Zero).GetHdc();
      int num = (int) ((double) SystemUtils.GetDeviceCaps(hdc, 88) * (double) ((float) SystemUtils.GetDeviceCaps(hdc, 10) / (float) SystemUtils.GetDeviceCaps(hdc, 117)));
      Logger.Info("DPI = {0}", (object) num);
      return num;
    }

    public static bool IsAdministrator()
    {
      bool flag = false;
      try
      {
        WindowsIdentity current = WindowsIdentity.GetCurrent();
        if (current == null)
          return false;
        flag = new WindowsPrincipal(current).IsInRole(WindowsBuiltInRole.Administrator);
      }
      catch
      {
      }
      return flag;
    }

    public static string GetSysInfo(string query)
    {
      int num = 0;
      string str = "";
      try
      {
        using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(query))
        {
          foreach (ManagementObject managementObject in managementObjectSearcher.Get())
          {
            ++num;
            foreach (PropertyData property in managementObject.Properties)
              str = str + property.Value.ToString() + "\n";
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in getting sysinfo err:" + ex.ToString());
      }
      return str.Trim();
    }

    public enum DeviceCap
    {
      VERTRES = 10, // 0x0000000A
      LOGPIXELSX = 88, // 0x00000058
      LOGPIXELSY = 90, // 0x0000005A
      DESKTOPVERTRES = 117, // 0x00000075
    }
  }
}
