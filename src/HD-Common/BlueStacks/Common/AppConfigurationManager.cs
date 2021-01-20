// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.AppConfigurationManager
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BlueStacks.Common
{
  [Serializable]
  public class AppConfigurationManager
  {
    private static object syncRoot = new object();
    private static volatile AppConfigurationManager sInstance;

    public static AppConfigurationManager Instance
    {
      get
      {
        if (AppConfigurationManager.sInstance == null)
        {
          lock (AppConfigurationManager.syncRoot)
          {
            if (AppConfigurationManager.sInstance == null)
              AppConfigurationManager.Init();
          }
        }
        return AppConfigurationManager.sInstance;
      }
    }

    private AppConfigurationManager()
    {
    }

    private static void Init()
    {
      try
      {
        AppConfigurationManager.sInstance = JsonConvert.DeserializeObject<AppConfigurationManager>(RegistryManager.Instance.AppConfiguration, Utils.GetSerializerSettings());
      }
      catch (Exception ex)
      {
        Logger.Warning("Error loading app configurations. Ex: " + ex.ToString());
      }
    }

    public static void Save()
    {
      if (AppConfigurationManager.sInstance == null)
        return;
      RegistryManager.Instance.AppConfiguration = JsonConvert.SerializeObject((object) AppConfigurationManager.sInstance, Utils.GetSerializerSettings());
    }

    [JsonProperty("VmAppConfig", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
    public Dictionary<string, Dictionary<string, AppSettings>> VmAppConfig { get; } = new Dictionary<string, Dictionary<string, AppSettings>>();

    public bool CheckIfTrueInAnyVm(string package, Predicate<AppSettings> rule, out string vmName)
    {
      vmName = string.Empty;
      foreach (KeyValuePair<string, Dictionary<string, AppSettings>> keyValuePair in this.VmAppConfig)
      {
        if (keyValuePair.Value.ContainsKey(package) && rule(keyValuePair.Value[package]))
        {
          vmName = keyValuePair.Key;
          return true;
        }
      }
      return false;
    }
  }
}
