// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.GuidUtils
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using Microsoft.Win32;
using System;
using System.IO;

namespace BlueStacks.Common
{
  public static class GuidUtils
  {
    private static string sBlueStacksMachineId;
    private static string sBlueStacksVersionId;

    public static string ReuseOrGenerateMachineId()
    {
      string newId = "";
      try
      {
        string blueStacksMachineId = GuidUtils.GetBlueStacksMachineId();
        if (!string.IsNullOrEmpty(blueStacksMachineId))
          return blueStacksMachineId;
        newId = Guid.NewGuid().ToString();
        GuidUtils.SetBlueStacksMachineId(newId);
      }
      catch (Exception ex)
      {
        Logger.Error("Couldn't generate/find Machine ID. Ex: {0}", (object) ex.Message);
      }
      return newId;
    }

    public static string ReuseOrGenerateVersionId()
    {
      string newId = "";
      try
      {
        string blueStacksVersionId = GuidUtils.GetBlueStacksVersionId();
        if (!string.IsNullOrEmpty(blueStacksVersionId))
          return blueStacksVersionId;
        newId = Guid.NewGuid().ToString();
        GuidUtils.SetBlueStacksVersionId(newId);
      }
      catch (Exception ex)
      {
        Logger.Error("Couldn't generate/find Version ID. Ex: {0}", (object) ex.Message);
      }
      return newId;
    }

    public static string GetBlueStacksMachineId()
    {
      if (string.IsNullOrEmpty(GuidUtils.sBlueStacksMachineId))
        GuidUtils.sBlueStacksMachineId = StringUtils.GetControlCharFreeString(GuidUtils.GetIdFromRegistryOrFile("MachineID").Trim());
      return GuidUtils.sBlueStacksMachineId;
    }

    public static bool SetBlueStacksMachineId(string newId)
    {
      return GuidUtils.SetIdInRegistryAndFile("MachineID", newId);
    }

    public static string GetBlueStacksVersionId()
    {
      if (string.IsNullOrEmpty(GuidUtils.sBlueStacksVersionId))
        GuidUtils.sBlueStacksVersionId = StringUtils.GetControlCharFreeString(GuidUtils.GetIdFromRegistryOrFile("VersionMachineId_4.250.0.1070").Trim());
      return GuidUtils.sBlueStacksVersionId;
    }

    public static bool SetBlueStacksVersionId(string newId)
    {
      return GuidUtils.SetIdInRegistryAndFile("VersionMachineId_4.250.0.1070", newId);
    }

    public static string GetIdFromRegistryOrFile(string id)
    {
      string str1 = (string) RegistryUtils.GetRegistryValue("Software\\BlueStacksInstaller", id, (object) "", RegistryKeyKind.HKEY_LOCAL_MACHINE);
      if (!string.IsNullOrEmpty(str1))
        return str1;
      try
      {
        string str2 = Path.Combine(new DirectoryInfo(ShortcutHelper.CommonDesktopPath).Parent.FullName, "BlueStacks");
        if (!Directory.Exists(str2))
          Directory.CreateDirectory(str2);
        string path = Path.Combine(str2, id);
        if (File.Exists(path))
        {
          string str3 = File.ReadAllText(path);
          if (!string.IsNullOrEmpty(str3))
            str1 = str3;
        }
      }
      catch
      {
      }
      if (!string.IsNullOrEmpty(str1))
        return str1;
      RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Software\\BlueStacksInstaller");
      if (registryKey != null)
        str1 = (string) registryKey.GetValue(id, (object) "");
      return str1;
    }

    public static bool SetIdInRegistryAndFile(string id, string value)
    {
      value = value?.Trim();
      bool flag = RegistryUtils.SetRegistryValue("Software\\BlueStacksInstaller", id, (object) value, RegistryValueKind.String, RegistryKeyKind.HKEY_LOCAL_MACHINE);
      try
      {
        string str = Path.Combine(new DirectoryInfo(ShortcutHelper.CommonDesktopPath).Parent.FullName, "BlueStacks");
        if (!Directory.Exists(str))
          Directory.CreateDirectory(str);
        string path = Path.Combine(str, id);
        if (File.Exists(path))
          File.Delete(path);
        File.WriteAllText(path, value);
        flag = true;
      }
      catch (Exception ex)
      {
        Logger.Warning("Failed to write in in file. Error: " + ex.Message);
      }
      try
      {
        Registry.CurrentUser.CreateSubKey("Software\\BlueStacksInstaller").SetValue(id, (object) value);
        flag = true;
      }
      catch (Exception ex)
      {
        Logger.Warning("Failed to write id in HKCU. Error: " + ex.Message);
      }
      return flag;
    }
  }
}
