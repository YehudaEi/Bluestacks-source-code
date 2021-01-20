// Decompiled with JetBrains decompiler
// Type: BlueStacks.VmManager.VmManager
// Assembly: HD-VmManager, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: A61734DC-C88B-40C1-967A-B786E5EAF3CF
// Assembly location: C:\Program Files\BlueStacks\HD-VmManager.exe

using BlueStacks.Common;
using System;
using System.Collections.Generic;
using System.Threading;

namespace BlueStacks.VmManager
{
  public class VmManager
  {
    public static void Main(string[] args)
    {
      Logger.InitUserLog();
      if (ProcessUtils.CheckAlreadyRunningAndTakeLock("Global\\BlueStacks_MULTI_INS_Frontend_Lockbgp", out Mutex _))
      {
        Logger.Info("Process already running, exiting");
        Environment.Exit(-17);
      }
      IEnumerable<ICommandFactory> availableCommands = BlueStacks.VmManager.VmManager.GetAvailableCommands();
      if (args.Length == 0)
      {
        BlueStacks.VmManager.VmManager.PrintUsage(availableCommands);
        Environment.Exit(-6);
      }
      ICommand command = new CommandParser(availableCommands).ParseCommand(args);
      int exitCode = command.Execute();
      Logger.Info("Command Execute returns: " + exitCode.ToString());
      if (exitCode < 0)
      {
        switch (exitCode)
        {
          case -15:
            Environment.Exit(-15);
            break;
          case -11:
            Environment.Exit(-11);
            break;
          case -10:
            Environment.Exit(-10);
            break;
          case -7:
            Environment.Exit(-7);
            break;
          case -1:
            Environment.Exit(-1);
            break;
          default:
            command.Undo();
            break;
        }
      }
      Logger.Info("Exiting process with result {0}", (object) exitCode);
      Environment.Exit(exitCode);
    }

    private static IEnumerable<ICommandFactory> GetAvailableCommands()
    {
      return (IEnumerable<ICommandFactory>) new ICommandFactory[6]
      {
        (ICommandFactory) new CreateInstanceCommand(),
        (ICommandFactory) new DeleteInstanceCommand(),
        (ICommandFactory) new FactoryReset(),
        (ICommandFactory) new UpgradeInstanceCommand(),
        (ICommandFactory) new RemoveDiskCommand(),
        (ICommandFactory) new ResetSharedFoldersCommand()
      };
    }

    private static void PrintUsage(IEnumerable<ICommandFactory> availableCommands)
    {
      Logger.Error("Usage MultiInstanceManager CommandName Arguments");
      Logger.Info("Commands");
      foreach (ICommandFactory availableCommand in availableCommands)
        Logger.Info(availableCommand.CommandName + "   desc: " + availableCommand.Description);
    }
  }
}
