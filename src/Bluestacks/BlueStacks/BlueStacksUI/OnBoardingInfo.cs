// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.OnBoardingInfo
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using Newtonsoft.Json;
using System;

namespace BlueStacks.BlueStacksUI
{
  [Serializable]
  public class OnBoardingInfo
  {
    public AppPackageListObject OnBoardingAppPackages { get; set; }

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Include, PropertyName = "skip_button_timer")]
    public int OnBoardingSkipTimer { get; set; } = 5;
  }
}
