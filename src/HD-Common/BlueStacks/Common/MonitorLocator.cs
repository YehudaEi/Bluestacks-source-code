// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.MonitorLocator
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using Microsoft.Win32;
using System.IO;

namespace BlueStacks.Common
{
  public static class MonitorLocator
  {
    private static readonly string REG_PATH = Path.Combine(RegistryManager.Instance.BaseKeyPath, "Monitors");

    public static void Publish(string vmName, uint vmId)
    {
      RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(MonitorLocator.REG_PATH, true);
      foreach (string valueName in registryKey.GetValueNames())
      {
        if (registryKey.GetValueKind(valueName) != RegistryValueKind.DWord)
        {
          registryKey.DeleteValue(valueName);
        }
        else
        {
          uint num = (uint) (int) registryKey.GetValue(valueName, (object) 0);
          if ((int) vmId == (int) num)
            registryKey.DeleteValue(valueName);
        }
      }
      registryKey.SetValue(vmName, (object) vmId, RegistryValueKind.DWord);
    }

    public static string[] Fetch()
    {
      return Registry.LocalMachine.OpenSubKey(MonitorLocator.REG_PATH, true).GetValueNames();
    }

    public static uint Lookup(string vmName)
    {
      return (uint) (int) Registry.LocalMachine.OpenSubKey(MonitorLocator.REG_PATH).GetValue(vmName, (object) 0);
    }
  }
}
