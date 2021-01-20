// Decompiled with JetBrains decompiler
// Type: MultiInstanceManagerMVVM.MessageModel.Classes.NewInstanceMessage
// Assembly: HD-MultiInstanceManager, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 0C4C54E8-53C7-4A46-AE96-CC8E89A6B884
// Assembly location: C:\Program Files\BlueStacks\HD-MultiInstanceManager.exe

using BlueStacks.Common;
using MultiInstanceManagerMVVM.Model.Classes;

namespace MultiInstanceManagerMVVM.MessageModel.Classes
{
  public class NewInstanceMessage
  {
    public string VmName { get; set; }

    public string Oem { get; set; } = "bgp";

    public NewInstanceType InstanceType { get; set; }

    public string NewInstanceSettings { get; set; }

    public string CloneFromVmName { get; set; }

    public int InstanceCount { get; set; }

    public string DisplayName { get; set; } = (string) null;

    public MimStatsModel MimStatsModel { get; set; } = (MimStatsModel) null;
  }
}
