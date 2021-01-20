// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.MultiInstanceUtils
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace BlueStacks.Common
{
  public static class MultiInstanceUtils
  {
    public static bool VerifyVmId(string vmId)
    {
      foreach (string vm in RegistryManager.Instance.VmList)
      {
        if (vm.Equals(vmId, StringComparison.OrdinalIgnoreCase))
          return true;
      }
      return false;
    }

    public static void SetDeviceCapsRegistry(string legacyReason, string engine)
    {
      MultiInstanceUtils.SetDeviceCapsRegistry(legacyReason, engine, CpuHvmState.Unknown, BiosHvmState.Unknown);
    }

    public static void SetDeviceCapsRegistry(
      string legacyReason,
      string engine,
      CpuHvmState cpuHvm,
      BiosHvmState biosHvm)
    {
      string deviceCaps = RegistryManager.Instance.DeviceCaps;
      JObject jobject1 = new JObject()
      {
        {
          "engine_enabled",
          (JToken) engine
        },
        {
          "legacy_reason",
          (JToken) legacyReason
        },
        {
          "cpu_hvm",
          (JToken) cpuHvm.ToString()
        },
        {
          "bios_hvm",
          (JToken) biosHvm.ToString()
        }
      };
      JObject jobject2 = JObject.Parse(deviceCaps);
      if (!string.IsNullOrEmpty(deviceCaps))
      {
        string str = jobject2["engine_enabled"].ToString();
        Logger.Info("Old engine was {0}", (object) str);
        if (!str.Equals(engine, StringComparison.OrdinalIgnoreCase))
          RegistryManager.Instance.SystemInfoStats2 = 1;
      }
      RegistryManager.Instance.CurrentEngine = engine;
      RegistryManager.Instance.DeviceCaps = jobject1.ToString(Formatting.None);
    }
  }
}
