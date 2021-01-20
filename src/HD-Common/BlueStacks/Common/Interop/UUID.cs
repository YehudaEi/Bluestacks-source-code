// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.Interop.UUID
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace BlueStacks.Common.Interop
{
  public static class UUID
  {
    public static string GetUserGUID()
    {
      string str1 = (string) null;
      string registryBaseKeyPath = Strings.RegistryBaseKeyPath;
      RegistryKey registryKey1;
      using (registryKey1 = Registry.CurrentUser.OpenSubKey(registryBaseKeyPath))
      {
        if (registryKey1 != null)
        {
          str1 = (string) registryKey1.GetValue("USER_GUID", (object) null);
          if (str1 != null)
          {
            Logger.Info("TODO: Fix GUID generation. This should not happen. Detected GUID in HKCU: " + str1);
            return str1;
          }
        }
      }
      RegistryKey registryKey2;
      using (registryKey2 = Registry.LocalMachine.OpenSubKey(registryBaseKeyPath))
      {
        if (registryKey2 != null)
        {
          str1 = (string) registryKey2.GetValue("USER_GUID", (object) null);
          if (str1 != null)
          {
            Logger.Info("TODO: Fix GUID generation. This should not happen. Detected GUID in HKLM: " + str1);
            return str1;
          }
        }
      }
      try
      {
        string path = Path.Combine(Environment.GetEnvironmentVariable("TEMP"), "Bst_Guid_Backup");
        if (File.Exists(path))
        {
          string str2 = File.ReadAllText(path);
          if (!string.IsNullOrEmpty(str2))
          {
            str1 = str2;
            Logger.Info("Detected User GUID %temp%: " + str1);
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Error(ex.ToString());
      }
      return str1;
    }

    public static string GetGuidFromBackUp()
    {
      string empty = string.Empty;
      if (FeatureManager.Instance.IsGuidBackUpEnable)
      {
        string userGuid = UUID.GetUserGUID();
        if (!string.IsNullOrEmpty(userGuid))
        {
          try
          {
            return new Guid(userGuid).ToString();
          }
          catch
          {
            return string.Empty;
          }
        }
      }
      return string.Empty;
    }

    public static string Base64Encode(string plainText)
    {
      return Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));
    }

    public static string Base64Decode(string base64EncodedData)
    {
      return Encoding.UTF8.GetString(Convert.FromBase64String(base64EncodedData));
    }

    private static string getBluestacksID()
    {
      return "BGP";
    }

    private static RegistryKey _openSubKey(
      RegistryKey parentKey,
      string subKeyName,
      bool writable,
      UUID.RegWow64Options options)
    {
      if (parentKey == null || UUID._getRegistryKeyHandle(parentKey) == IntPtr.Zero)
        return (RegistryKey) null;
      int num = 131097;
      if (writable)
        num = 131078;
      int phkResult;
      return UUID.RegOpenKeyEx(UUID._getRegistryKeyHandle(parentKey), subKeyName, 0, (int) ((UUID.RegWow64Options) num | options), out phkResult) != 0 ? (RegistryKey) null : UUID._pointerToRegistryKey((IntPtr) phkResult, writable, false);
    }

    private static IntPtr _getRegistryKeyHandle(RegistryKey registryKey)
    {
      return ((SafeHandle) typeof (RegistryKey).GetField("hkey", BindingFlags.Instance | BindingFlags.NonPublic).GetValue((object) registryKey)).DangerousGetHandle();
    }

    private static RegistryKey _pointerToRegistryKey(
      IntPtr hKey,
      bool writable,
      bool ownsHandle)
    {
      BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.NonPublic;
      Type type1 = typeof (SafeHandleZeroOrMinusOneIsInvalid).Assembly.GetType("Microsoft.Win32.SafeHandles.SafeRegistryHandle");
      Type[] types1 = new Type[2]
      {
        typeof (IntPtr),
        typeof (bool)
      };
      object obj = type1.GetConstructor(bindingAttr, (Binder) null, types1, (ParameterModifier[]) null).Invoke(new object[2]
      {
        (object) hKey,
        (object) ownsHandle
      });
      Type type2 = typeof (RegistryKey);
      Type[] types2 = new Type[2]
      {
        type1,
        typeof (bool)
      };
      return (RegistryKey) type2.GetConstructor(bindingAttr, (Binder) null, types2, (ParameterModifier[]) null).Invoke(new object[2]
      {
        obj,
        (object) writable
      });
    }

    [DllImport("advapi32.dll", CharSet = CharSet.Auto)]
    public static extern int RegOpenKeyEx(
      IntPtr hKey,
      string subKey,
      int ulOptions,
      int samDesired,
      out int phkResult);

    private enum RegWow64Options
    {
      None = 0,
      KEY_WOW64_64KEY = 256, // 0x00000100
      KEY_WOW64_32KEY = 512, // 0x00000200
    }

    private enum RegistryRights
    {
      WriteKey = 131078, // 0x00020006
      ReadKey = 131097, // 0x00020019
    }
  }
}
