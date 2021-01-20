// Decompiled with JetBrains decompiler
// Type: BlueStacks.VmManager.FactoryReset
// Assembly: HD-VmManager, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: A61734DC-C88B-40C1-967A-B786E5EAF3CF
// Assembly location: C:\Program Files\BlueStacks\HD-VmManager.exe

using BlueStacks.Common;

namespace BlueStacks.VmManager
{
  public class FactoryReset : ICommand, ICommandFactory
  {
    public string Description
    {
      get
      {
        return "Factory Reset android";
      }
    }

    public string CommandName
    {
      get
      {
        return "factoryreset";
      }
    }

    public ICommand MakeCommand(string[] args)
    {
      return (ICommand) new FactoryReset();
    }

    public int Execute()
    {
      return VBoxManager.CloneForFactoryReset("Android");
    }

    public void Undo()
    {
      Logger.Info("No implementation for Undo for FactoryResetCommand");
    }
  }
}
