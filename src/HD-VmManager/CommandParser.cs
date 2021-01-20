// Decompiled with JetBrains decompiler
// Type: BlueStacks.VmManager.CommandParser
// Assembly: HD-VmManager, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: A61734DC-C88B-40C1-967A-B786E5EAF3CF
// Assembly location: C:\Program Files\BlueStacks\HD-VmManager.exe

using System;
using System.Collections.Generic;
using System.Linq;

namespace BlueStacks.VmManager
{
  public class CommandParser
  {
    private IEnumerable<ICommandFactory> mAvailableCommands;

    public CommandParser(IEnumerable<ICommandFactory> availableCommands)
    {
      this.mAvailableCommands = availableCommands;
    }

    public ICommand ParseCommand(string[] args)
    {
      string commandName = args[0];
      ICommandFactory requestedCommand = this.FindRequestedCommand(commandName);
      if (requestedCommand != null)
        return requestedCommand.MakeCommand(args);
      return (ICommand) new NotFoundCommand()
      {
        Name = commandName
      };
    }

    private ICommandFactory FindRequestedCommand(string commandName)
    {
      return this.mAvailableCommands.FirstOrDefault<ICommandFactory>((Func<ICommandFactory, bool>) (cmd => cmd.CommandName == commandName));
    }
  }
}
