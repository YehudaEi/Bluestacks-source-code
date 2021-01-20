// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.MacroEvents
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BlueStacks.Common
{
  [Serializable]
  public class MacroEvents
  {
    [JsonProperty("Timestamp", NullValueHandling = NullValueHandling.Ignore)]
    public long Timestamp { get; set; }

    [JsonExtensionData]
    public IDictionary<string, object> ExtraData { get; set; }
  }
}
