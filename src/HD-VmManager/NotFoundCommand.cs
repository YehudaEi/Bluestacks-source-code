// Decompiled with JetBrains decompiler
// Type: BlueStacks.VmManager.NotFoundCommand
// Assembly: HD-VmManager, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: A61734DC-C88B-40C1-967A-B786E5EAF3CF
// Assembly location: C:\Program Files\BlueStacks\HD-VmManager.exe

using BlueStacks.Common;

namespace BlueStacks.VmManager
{
  public class NotFoundCommand : ICommand
  {
    public string Name { get; set; }

    public int Execute()
    {
      Logger.Error("the following Command is not implemented : {0}", (object) this.Name);
      return -6;
    }

    public void Undo()
    {
      Logger.Error("the following Command is not implemented : {0}", (object) this.Name);
    }
  }
}
