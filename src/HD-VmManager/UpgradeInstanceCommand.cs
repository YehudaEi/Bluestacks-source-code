// Decompiled with JetBrains decompiler
// Type: BlueStacks.VmManager.UpgradeInstanceCommand
// Assembly: HD-VmManager, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: A61734DC-C88B-40C1-967A-B786E5EAF3CF
// Assembly location: C:\Program Files\BlueStacks\HD-VmManager.exe

using System;

namespace BlueStacks.VmManager
{
  public class UpgradeInstanceCommand : ICommandFactory, ICommand
  {
    public string CommandName
    {
      get
      {
        return "upgradeinstance";
      }
    }

    public string Description
    {
      get
      {
        return "use this command to upgrade the VM instance";
      }
    }

    public ICommand MakeCommand(string[] args)
    {
      return (ICommand) new UpgradeInstanceCommand();
    }

    public int Execute()
    {
      return VBoxManager.UpgradeMachine();
    }

    public void Undo()
    {
      throw new NotImplementedException();
    }
  }
}
