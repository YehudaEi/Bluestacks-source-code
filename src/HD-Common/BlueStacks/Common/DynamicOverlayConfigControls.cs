// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.DynamicOverlayConfigControls
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BlueStacks.Common
{
  [Serializable]
  public class DynamicOverlayConfigControls
  {
    private static object syncRoot = new object();
    public List<DynamicOverlayConfig> GameControls = new List<DynamicOverlayConfig>();
    private static volatile DynamicOverlayConfigControls sInstance;

    public static DynamicOverlayConfigControls Instance
    {
      get
      {
        if (DynamicOverlayConfigControls.sInstance == null)
        {
          lock (DynamicOverlayConfigControls.syncRoot)
          {
            if (DynamicOverlayConfigControls.sInstance == null)
              DynamicOverlayConfigControls.sInstance = new DynamicOverlayConfigControls();
          }
        }
        return DynamicOverlayConfigControls.sInstance;
      }
    }

    private DynamicOverlayConfigControls()
    {
    }

    public static void Init(string data)
    {
      try
      {
        DynamicOverlayConfigControls.sInstance = JsonConvert.DeserializeObject<DynamicOverlayConfigControls>(data, Utils.GetSerializerSettings());
      }
      catch (Exception ex)
      {
        Logger.Warning("Error loading dynamic overlay data. Ex: " + ex.ToString());
      }
    }
  }
}
