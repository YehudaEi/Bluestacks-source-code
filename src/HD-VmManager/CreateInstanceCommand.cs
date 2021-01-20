// Decompiled with JetBrains decompiler
// Type: BlueStacks.VmManager.CreateInstanceCommand
// Assembly: HD-VmManager, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: A61734DC-C88B-40C1-967A-B786E5EAF3CF
// Assembly location: C:\Program Files\BlueStacks\HD-VmManager.exe

using BlueStacks.Common;

namespace BlueStacks.VmManager
{
  public class CreateInstanceCommand : ICommandFactory, ICommand
  {
    private string vmType = "fresh";
    private string mCloneFromVm = "Android";
    private string mEngineSettings = string.Empty;
    private string mVmName;

    public string VmType
    {
      get
      {
        return "use this command to create a new vm instance";
      }
    }

    public string Description
    {
      get
      {
        return "use this command to create a new vm instance";
      }
    }

    public string CommandName
    {
      get
      {
        return "createinstance";
      }
    }

    public ICommand MakeCommand(string[] args)
    {
      return (ICommand) new CreateInstanceCommand()
      {
        vmType = args[1],
        mVmName = args[2],
        mCloneFromVm = args[3],
        mEngineSettings = args[4]
      };
    }

    public int Execute()
    {
      if (!string.IsNullOrEmpty(RegistryManager.Instance.DeviceCaps))
        return VBoxManager.CreateMachine(this.mVmName, this.vmType, this.mCloneFromVm, this.mEngineSettings);
      Logger.Error("Cannot create VM, Device Caps registry is not present");
      return -15;
    }

    public void Undo()
    {
      VBoxManager.DeleteMachine(this.mVmName);
    }
  }
}
