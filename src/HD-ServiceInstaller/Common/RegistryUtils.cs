// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.RegistryUtils
// Assembly: HD-ServiceInstaller, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 15F93427-26B3-4C7E-BAB1-0A00688BC4D4
// Assembly location: C:\Program Files\BlueStacks\HD-ServiceInstaller.exe

using Microsoft.Win32;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;

namespace BlueStacks.Common
{
  public static class RegistryUtils
  {
    public static readonly UIntPtr HKEY_LOCAL_MACHINE = new UIntPtr(2147483650U);
    public static readonly UIntPtr HKEY_CURRENT_USER = new UIntPtr(2147483649U);

    [DllImport("advapi32", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern int RegRenameKey(UIntPtr hKey, [MarshalAs(UnmanagedType.LPWStr)] string oldname, [MarshalAs(UnmanagedType.LPWStr)] string newname);

    [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern int RegOpenKeyEx(
      UIntPtr hKey,
      string subKey,
      int ulOptions,
      RegSAM samDesired,
      out UIntPtr hkResult);

    public static RegistryKey InitKeyWithSecurityCheck(string keyName)
    {
      return !SystemUtils.IsAdministrator() ? Registry.LocalMachine.OpenSubKey(keyName) : Registry.LocalMachine.CreateSubKey(keyName);
    }

    public static RegistryKey InitKey(string keyName)
    {
      try
      {
        return Registry.LocalMachine.CreateSubKey(keyName);
      }
      catch
      {
        return (RegistryKey) null;
      }
    }

    public static void DeleteKey(string hklmPath, bool throwOnError = true)
    {
      try
      {
        Registry.LocalMachine.DeleteSubKeyTree(hklmPath);
      }
      catch
      {
        if (!throwOnError)
          return;
        throw;
      }
    }

    public static object GetRegistryValue(
      string registryPath,
      string key,
      object defaultValue,
      RegistryKeyKind registryKind = RegistryKeyKind.HKEY_LOCAL_MACHINE)
    {
      RegistryKey registryKey = (RegistryKey) null;
      object obj = defaultValue;
      switch (registryKind)
      {
        case RegistryKeyKind.HKEY_LOCAL_MACHINE:
          registryKey = Registry.LocalMachine.OpenSubKey(registryPath);
          break;
        case RegistryKeyKind.HKEY_CURRENT_USER:
          registryKey = Registry.CurrentUser.OpenSubKey(registryPath);
          break;
      }
      if (registryKey != null)
      {
        obj = registryKey.GetValue(key, defaultValue);
        registryKey.Close();
      }
      return obj;
    }

    public static bool SetRegistryValue(
      string registryPath,
      string key,
      object value,
      RegistryValueKind type,
      RegistryKeyKind registryKind = RegistryKeyKind.HKEY_LOCAL_MACHINE)
    {
      RegistryKey registryKey = (RegistryKey) null;
      bool flag = true;
      try
      {
        switch (registryKind)
        {
          case RegistryKeyKind.HKEY_LOCAL_MACHINE:
            registryKey = Registry.LocalMachine.CreateSubKey(registryPath);
            break;
          case RegistryKeyKind.HKEY_CURRENT_USER:
            registryKey = Registry.CurrentUser.CreateSubKey(registryPath);
            break;
        }
        registryKey?.SetValue(key, value, type);
      }
      catch
      {
        Logger.Warning("Failed to set registry value at {0} for {1}:{2}", (object) registryPath, (object) key, value);
        flag = false;
      }
      finally
      {
        registryKey?.Close();
      }
      return flag;
    }

    public static int RenameKey(
      string basePath,
      string oldName,
      string newName,
      bool deleteNewIfExist)
    {
      if (deleteNewIfExist)
      {
        try
        {
          Registry.LocalMachine.DeleteSubKeyTree(Path.Combine(basePath, newName));
        }
        catch (Exception ex)
        {
          Logger.Warning("Couldn't delete new subkeytree: {0}\\{1}, ex: {2}", (object) basePath, (object) newName, (object) ex.Message);
        }
      }
      UIntPtr hkResult;
      int num = RegistryUtils.RegOpenKeyEx(RegistryUtils.HKEY_LOCAL_MACHINE, basePath, 0, RegSAM.Write, out hkResult);
      if (num == 0)
        num = RegistryUtils.RegRenameKey(hkResult, oldName, newName);
      return num;
    }

    public static void GrantAllAccessPermission(RegistryKey rk)
    {
      NTAccount ntAccount = new SecurityIdentifier(WellKnownSidType.WorldSid, (SecurityIdentifier) null).Translate(typeof (NTAccount)) as NTAccount;
      RegistrySecurity registrySecurity = new RegistrySecurity();
      RegistryAccessRule rule = new RegistryAccessRule(ntAccount.ToString(), RegistryRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow);
      registrySecurity.AddAccessRule(rule);
      rk?.SetAccessControl(registrySecurity);
    }

    public static void MoveUnifiedInstallerRegistryFromWow64()
    {
      try
      {
        RegistryKey registryKey1 = Registry.LocalMachine.OpenSubKey(Strings.RegistryBaseKeyPath);
        if (registryKey1 != null && !string.IsNullOrEmpty((string) registryKey1.GetValue("Version", (object) null)))
          return;
        RegistryKey registryKey2 = Registry.LocalMachine.OpenSubKey("Software", true);
        RegistryKey sourceKey = Registry.LocalMachine.OpenSubKey("Software\\WOW6432Node\\BlueStacks" + Strings.GetOemTag());
        if (sourceKey == null)
          return;
        RegistryKey subKey = registryKey2.CreateSubKey("BlueStacks" + Strings.GetOemTag());
        RegistryUtils.RecurseCopyKey(sourceKey, subKey);
        registryKey2.DeleteSubKeyTree("WOW6432Node\\BlueStacks" + Strings.GetOemTag());
        RegistryUtils.GrantAllAccessPermission(subKey);
      }
      catch
      {
      }
    }

    private static void RecurseCopyKey(RegistryKey sourceKey, RegistryKey destinationKey)
    {
      foreach (string valueName in sourceKey.GetValueNames())
      {
        object obj = sourceKey.GetValue(valueName);
        RegistryValueKind valueKind = sourceKey.GetValueKind(valueName);
        destinationKey.SetValue(valueName, obj, valueKind);
      }
      foreach (string subKeyName in sourceKey.GetSubKeyNames())
        RegistryUtils.RecurseCopyKey(sourceKey.OpenSubKey(subKeyName), destinationKey.CreateSubKey(subKeyName));
    }
  }
}
