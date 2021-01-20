// Decompiled with JetBrains decompiler
// Type: BlueStacks.VmManager.DeleteInstanceCommand
// Assembly: HD-VmManager, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: A61734DC-C88B-40C1-967A-B786E5EAF3CF
// Assembly location: C:\Program Files\BlueStacks\HD-VmManager.exe

using BlueStacks.Common;
using System.Collections.Generic;
using System.Linq;

namespace BlueStacks.VmManager
{
  public class DeleteInstanceCommand : ICommand, ICommandFactory
  {
    private string mVmName;

    public string Description
    {
      get
      {
        return "deleteinstance vmName(the vmName that has to be deleted";
      }
    }

    public string CommandName
    {
      get
      {
        return "deleteinstance";
      }
    }

    public ICommand MakeCommand(string[] args)
    {
      if (args.Length == 1)
        return (ICommand) new NotFoundCommand()
        {
          Name = args[0]
        };
      return (ICommand) new DeleteInstanceCommand()
      {
        mVmName = args[1]
      };
    }

    public int Execute()
    {
      return this.mVmName == "Android" ? -10 : VBoxManager.DeleteMachine(this.mVmName);
    }

    private bool ValidateVmName()
    {
      if (((IEnumerable<string>) RegistryManager.Instance.VmList).Contains<string>(this.mVmName))
        return true;
      Logger.Info("The vmName {0} is not in vmList", (object) this.mVmName);
      return false;
    }

    public void Undo()
    {
      Logger.Info("No implementation for Undo for DeleteInstanceCommand");
    }
  }
}
