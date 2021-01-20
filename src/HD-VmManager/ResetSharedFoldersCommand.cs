// Decompiled with JetBrains decompiler
// Type: BlueStacks.VmManager.ResetSharedFoldersCommand
// Assembly: HD-VmManager, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: A61734DC-C88B-40C1-967A-B786E5EAF3CF
// Assembly location: C:\Program Files\BlueStacks\HD-VmManager.exe

using BlueStacks.Common;

namespace BlueStacks.VmManager
{
  internal class ResetSharedFoldersCommand : ICommand, ICommandFactory
  {
    public string VmName { get; set; }

    public string CommandName
    {
      get
      {
        return "resetSharedFolders";
      }
    }

    public string Description
    {
      get
      {
        return "use this command to reset shared folders";
      }
    }

    public ICommand MakeCommand(string[] args)
    {
      if (args.Length == 1)
        return (ICommand) new NotFoundCommand()
        {
          Name = args[0]
        };
      return (ICommand) new ResetSharedFoldersCommand()
      {
        VmName = args[1]
      };
    }

    public int Execute()
    {
      return VBoxManager.ResetSharedFolders(this.VmName);
    }

    public void Undo()
    {
      Logger.Info("ResetSharedFoldersCommand->Undo");
      RegistryManager.Instance.Guest[this.VmName].CanAccessWindowsFolder = !RegistryManager.Instance.Guest[this.VmName].CanAccessWindowsFolder;
    }
  }
}
