// Decompiled with JetBrains decompiler
// Type: MultiInstanceManagerMVVM.Model.Classes.AppPlayerUpdateModel
// Assembly: HD-MultiInstanceManager, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 0C4C54E8-53C7-4A46-AE96-CC8E89A6B884
// Assembly location: C:\Program Files\BlueStacks\HD-MultiInstanceManager.exe

using Newtonsoft.Json;

namespace MultiInstanceManagerMVVM.Model.Classes
{
  public class AppPlayerUpdateModel
  {
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Include, PropertyName = "requested_prod_ver")]
    public string RequestedProdVersion { get; set; }

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Include, PropertyName = "requested_oem")]
    public string RequestedOem { get; set; }

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Include, PropertyName = "update_available")]
    public bool UpdateAvailable { get; set; }

    public string OemDisplayName { get; set; }
  }
}
