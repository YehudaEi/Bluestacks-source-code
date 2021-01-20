// Decompiled with JetBrains decompiler
// Type: MultiInstanceManagerMVVM.MessageModel.Classes.InstanceCreationMessage
// Assembly: HD-MultiInstanceManager, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 0C4C54E8-53C7-4A46-AE96-CC8E89A6B884
// Assembly location: C:\Program Files\BlueStacks\HD-MultiInstanceManager.exe

using BlueStacks.Common;

namespace MultiInstanceManagerMVVM.MessageModel.Classes
{
  public class InstanceCreationMessage
  {
    public NewInstanceType InstanceType { get; set; }

    public string CloneFromVm { get; set; } = (string) null;

    public string Oem { get; set; }

    public string ABIValue { get; set; } = (string) null;

    public AppPlayerModel AppPlayerModel { get; set; }
  }
}
