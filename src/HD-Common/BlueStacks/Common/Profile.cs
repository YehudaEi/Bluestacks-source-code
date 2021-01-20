// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.Profile
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using Microsoft.VisualBasic.Devices;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace BlueStacks.Common
{
  public static class Profile
  {
    private static ulong mTotalPhysicalMemory = 0;
    private static int mNumberOfLogicalProcessors = 0;
    private static string sOS = "";
    private static Dictionary<string, string> s_Info;

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool IsWow64Process(IntPtr proc, ref bool isWow);

    public static string GlVendor { get; set; } = "";

    public static string GlRenderer { get; set; } = "";

    public static string GlVersion { get; set; } = "";

    private static bool IsOs64Bit()
    {
      switch (IntPtr.Size)
      {
        case 4:
          if (!Profile.Is32BitProcessOn64BitProcessor())
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
      Profile.IsWow64Process(Process.GetCurrentProcess().Handle, ref isWow);
      return isWow;
    }

    public static Dictionary<string, string> Info()
    {
      if (Profile.s_Info != null)
        return Profile.s_Info;
      string index = "Android";
      Dictionary<string, string> dictionary = new Dictionary<string, string>()
      {
        {
          "ProcessorId",
          SystemUtils.GetSysInfo("Select processorID from Win32_Processor")
        },
        {
          "Processor",
          Profile.CPU
        }
      };
      string sysInfo = SystemUtils.GetSysInfo("Select NumberOfLogicalProcessors from Win32_Processor");
      Logger.Info("the length of numOfProcessor string is {0}", (object) sysInfo.Length.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      dictionary.Add("NumberOfProcessors", sysInfo);
      dictionary.Add("GPU", Profile.GPU);
      dictionary.Add("GPUDriver", SystemUtils.GetSysInfo("Select DriverVersion from Win32_VideoController"));
      dictionary.Add("OS", Profile.OS);
      string str1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}", (object) Environment.OSVersion.Version.Major, (object) Environment.OSVersion.Version.Minor);
      dictionary.Add("OSVersion", str1);
      dictionary.Add("RAM", Profile.RAM);
      try
      {
        string version = RegistryManager.Instance.Version;
        dictionary.Add("BlueStacksVersion", version);
      }
      catch
      {
      }
      int num1;
      try
      {
        num1 = RegistryManager.Instance.Guest[index].GlMode;
      }
      catch
      {
        num1 = -1;
      }
      dictionary.Add("GlMode", num1.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      int num2;
      try
      {
        num2 = RegistryManager.Instance.Guest[index].GlRenderMode;
      }
      catch
      {
        num2 = -1;
      }
      dictionary.Add("GlRenderMode", num2.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      string str2 = "";
      try
      {
        RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\OEMInformation");
        str2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} {1}", (object) (string) registryKey.GetValue("Manufacturer", (object) ""), (object) (string) registryKey.GetValue("Model", (object) ""));
      }
      catch
      {
      }
      dictionary.Add("OEMInfo", str2);
      int width = Screen.PrimaryScreen.Bounds.Width;
      int num3 = Screen.PrimaryScreen.Bounds.Height;
      dictionary.Add("ScreenResolution", width.ToString((IFormatProvider) CultureInfo.InvariantCulture) + "x" + num3.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      try
      {
        int windowWidth = RegistryManager.Instance.Guest[index].WindowWidth;
        num3 = RegistryManager.Instance.Guest[index].WindowHeight;
        dictionary.Add("BlueStacksResolution", windowWidth.ToString((IFormatProvider) CultureInfo.InvariantCulture) + "x" + num3.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      }
      catch
      {
      }
      string str3 = "";
      try
      {
        RegistryKey registryKey1 = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP");
        foreach (string subKeyName in registryKey1.GetSubKeyNames())
        {
          if (subKeyName.StartsWith("v", StringComparison.OrdinalIgnoreCase))
          {
            RegistryKey registryKey2 = registryKey1.OpenSubKey(subKeyName);
            if (registryKey2.GetValue("Install") != null && (int) registryKey2.GetValue("Install") == 1)
              str3 = (string) registryKey2.GetValue("Version");
            if (subKeyName == "v4")
            {
              RegistryKey registryKey3 = registryKey2.OpenSubKey("Client");
              if (registryKey3 != null && (int) registryKey3.GetValue("Install") == 1)
                str3 = (string) registryKey3.GetValue("Version") + " Client";
              RegistryKey registryKey4 = registryKey2.OpenSubKey("Full");
              if (registryKey4 != null && (int) registryKey4.GetValue("Install") == 1)
                str3 = (string) registryKey4.GetValue("Version") + " Full";
            }
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Got exception when checking dot net version,err: {0}", (object) ex.ToString());
      }
      dictionary.Add("DotNetVersion", str3);
      if (Profile.IsOs64Bit())
        dictionary.Add("OSVERSIONTYPE", "64 bit");
      else
        dictionary.Add("OSVERSIONTYPE", "32 bit");
      Profile.s_Info = dictionary;
      return Profile.s_Info;
    }

    public static Dictionary<string, string> InfoForGraphicsDriverCheck()
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>()
      {
        {
          "os_version",
          SystemUtils.GetSysInfo("Select Caption from Win32_OperatingSystem")
        },
        {
          "os_arch",
          SystemUtils.GetSysInfo("Select OSArchitecture from Win32_OperatingSystem")
        },
        {
          "processor_vendor",
          SystemUtils.GetSysInfo("Select Manufacturer from Win32_Processor")
        },
        {
          "processor",
          SystemUtils.GetSysInfo("Select Name from Win32_Processor")
        }
      };
      string sysInfo1 = SystemUtils.GetSysInfo("Select Caption from Win32_VideoController");
      string str1 = "";
      string[] strArray1 = sysInfo1.Split(new string[3]
      {
        Environment.NewLine,
        "\r\n",
        "\n"
      }, StringSplitOptions.RemoveEmptyEntries);
      if (!string.IsNullOrEmpty(sysInfo1))
      {
        foreach (string str2 in strArray1)
          str1 = str1 + str2.Substring(0, str2.IndexOf(" ", StringComparison.OrdinalIgnoreCase)) + "\r\n";
        str1 = str1.Trim();
      }
      string sysInfo2 = SystemUtils.GetSysInfo("Select DriverVersion from Win32_VideoController");
      string sysInfo3 = SystemUtils.GetSysInfo("Select DriverDate from Win32_VideoController");
      string[] strArray2 = str1.Split(new string[3]
      {
        Environment.NewLine,
        "\r\n",
        "\n"
      }, StringSplitOptions.RemoveEmptyEntries);
      string[] strArray3 = sysInfo2.Split(new string[3]
      {
        Environment.NewLine,
        "\r\n",
        "\n"
      }, StringSplitOptions.RemoveEmptyEntries);
      string[] strArray4 = sysInfo3.Split(new string[3]
      {
        Environment.NewLine,
        "\r\n",
        "\n"
      }, StringSplitOptions.RemoveEmptyEntries);
      for (int index = 0; index < strArray1.Length; ++index)
      {
        if (strArray1[index] == Profile.GlRenderer || Profile.GlVendor.Contains(strArray2[index]))
        {
          sysInfo1 = strArray1[index];
          str1 = strArray2[index];
          sysInfo2 = strArray3[index];
          sysInfo3 = strArray4[index];
          break;
        }
      }
      dictionary.Add("gpu", sysInfo1);
      dictionary.Add("gpu_vendor", str1);
      dictionary.Add("driver_version", sysInfo2);
      dictionary.Add("driver_date", sysInfo3);
      string str3 = "";
      RegistryKey registryKey1;
      using (registryKey1 = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\OEMInformation"))
      {
        if (registryKey1 != null)
          str3 = (string) registryKey1.GetValue("Manufacturer", (object) "");
      }
      dictionary.Add("oem_manufacturer", str3);
      string str4 = "";
      RegistryKey registryKey2;
      using (registryKey2 = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\OEMInformation"))
      {
        if (registryKey2 != null)
          str4 = (string) registryKey2.GetValue("Model", (object) "");
      }
      dictionary.Add("oem_model", str4);
      dictionary.Add("bst_oem", RegistryManager.Instance.Oem);
      return dictionary;
    }

    private static string ToUpper(string id)
    {
      return id.ToUpperInvariant();
    }

    public static ulong TotalPhysicalMemory
    {
      get
      {
        if (Profile.mTotalPhysicalMemory == 0UL)
        {
          try
          {
            Profile.mTotalPhysicalMemory = ulong.Parse(new ComputerInfo().TotalPhysicalMemory.ToString((IFormatProvider) CultureInfo.InvariantCulture), (IFormatProvider) CultureInfo.InvariantCulture);
          }
          catch (Exception ex)
          {
            Logger.Error("Couldn't get TotalPhysicalMemory, Ex: {0}", (object) ex.Message);
          }
        }
        return Profile.mTotalPhysicalMemory;
      }
    }

    public static int NumberOfLogicalProcessors
    {
      get
      {
        if (Profile.mNumberOfLogicalProcessors == 0)
        {
          try
          {
            int.TryParse(SystemUtils.GetSysInfo("Select NumberOfLogicalProcessors from Win32_Processor"), out Profile.mNumberOfLogicalProcessors);
          }
          catch (Exception ex)
          {
            Logger.Error("Couldn't get NumberOfLogicalProcessors, Ex: {0}", (object) ex.Message);
          }
        }
        return Profile.mNumberOfLogicalProcessors;
      }
    }

    public static string RAM
    {
      get
      {
        int num = 0;
        try
        {
          num = (int) (Convert.ToUInt64(SystemUtils.GetSysInfo("Select TotalPhysicalMemory from Win32_ComputerSystem"), (IFormatProvider) CultureInfo.InvariantCulture) / 1048576UL);
        }
        catch (Exception ex)
        {
          Logger.Error("Exception when finding ram");
          Logger.Error(ex.ToString());
        }
        return num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      }
    }

    public static string CPU
    {
      get
      {
        return SystemUtils.GetSysInfo("Select Name from Win32_Processor");
      }
    }

    public static string GPU
    {
      get
      {
        return SystemUtils.GetSysInfo("Select Caption from Win32_VideoController");
      }
    }

    public static string OS
    {
      get
      {
        if (string.IsNullOrEmpty(Profile.sOS))
          Profile.sOS = SystemUtils.GetSysInfo("Select Caption from Win32_OperatingSystem");
        return Profile.sOS;
      }
    }
  }
}
