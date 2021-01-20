// Decompiled with JetBrains decompiler
// Type: BlueStacks.VmManager.ICommand
// Assembly: HD-VmManager, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: A61734DC-C88B-40C1-967A-B786E5EAF3CF
// Assembly location: C:\Program Files\BlueStacks\HD-VmManager.exe

namespace BlueStacks.VmManager
{
  public interface ICommand
  {
    int Execute();

    void Undo();
  }
}
