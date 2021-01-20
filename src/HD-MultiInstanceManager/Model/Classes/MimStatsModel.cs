// Decompiled with JetBrains decompiler
// Type: MultiInstanceManagerMVVM.Model.Classes.MimStatsModel
// Assembly: HD-MultiInstanceManager, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 0C4C54E8-53C7-4A46-AE96-CC8E89A6B884
// Assembly location: C:\Program Files\BlueStacks\HD-MultiInstanceManager.exe

using BlueStacks.Common;

namespace MultiInstanceManagerMVVM.Model.Classes
{
  public class MimStatsModel
  {
    public string Performance { get; set; }

    public string Resolution { get; set; }

    public int Abi { get; set; }

    public string Dpi { get; set; }

    public int InstanceCount { get; set; }

    public string Oem { get; set; }

    public string ProdVersion { get; set; }

    public string Arg1 { get; set; }

    public string Arg2 { get; set; }

    public NewInstanceType InstanceType { get; set; }
  }
}
