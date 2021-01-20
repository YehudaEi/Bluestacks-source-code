// Decompiled with JetBrains decompiler
// Type: BlueStacks.VmManager.RemoveDiskCommand
// Assembly: HD-VmManager, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: A61734DC-C88B-40C1-967A-B786E5EAF3CF
// Assembly location: C:\Program Files\BlueStacks\HD-VmManager.exe

using System;

namespace BlueStacks.VmManager
{
  internal class RemoveDiskCommand : ICommand, ICommandFactory
  {
    public string mDiskName;

    public string Description
    {
      get
      {
        return "To remove attached disk";
      }
    }

    public string CommandName
    {
      get
      {
        return "removedisk";
      }
    }

    public int Execute()
    {
      return VBoxManager.RemoveDisk(this.mDiskName);
    }

    public ICommand MakeCommand(string[] args)
    {
      if (args.Length == 1)
        return (ICommand) new NotFoundCommand()
        {
          Name = args[0]
        };
      return (ICommand) new RemoveDiskCommand()
      {
        mDiskName = args[1]
      };
    }

    public void Undo()
    {
      throw new NotImplementedException();
    }
  }
}
