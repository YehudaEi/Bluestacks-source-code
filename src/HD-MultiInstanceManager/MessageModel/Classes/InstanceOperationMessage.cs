// Decompiled with JetBrains decompiler
// Type: MultiInstanceManagerMVVM.MessageModel.Classes.InstanceOperationMessage
// Assembly: HD-MultiInstanceManager, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 0C4C54E8-53C7-4A46-AE96-CC8E89A6B884
// Assembly location: C:\Program Files\BlueStacks\HD-MultiInstanceManager.exe

using BlueStacks.Common;
using MultiInstanceManagerMVVM.Helper;

namespace MultiInstanceManagerMVVM.MessageModel.Classes
{
  public class InstanceOperationMessage
  {
    public Operation Operation { get; set; }

    public string VmName { get; set; }

    public string LastVMName { get; set; }

    public string NewVmDisplayName { get; set; }

    public bool IsNewVmDisplayNameVerified { get; set; }

    public AppPlayerModel AppPlayerModel { get; set; }
  }
}
