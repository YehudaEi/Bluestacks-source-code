// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.RelayCommand2
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.Windows.Input;

namespace BlueStacks.Common
{
  public class RelayCommand2 : ICommand
  {
    private readonly Func<object, bool> canExecute;
    private readonly System.Action<object> execute;

    public RelayCommand2(System.Action<object> execute)
    {
      this.canExecute = (Func<object, bool>) (obj => true);
      this.execute = execute;
    }

    public RelayCommand2(Func<object, bool> canExecute, System.Action<object> execute)
    {
      this.canExecute = canExecute;
      this.execute = execute;
    }

    public bool CanExecute(object parameter)
    {
      return this.canExecute != null && this.canExecute(parameter);
    }

    public void Execute(object parameter)
    {
      System.Action<object> execute = this.execute;
      if (execute == null)
        return;
      execute(parameter);
    }

    public event EventHandler CanExecuteChanged
    {
      add
      {
        CommandManager.RequerySuggested += value;
      }
      remove
      {
        CommandManager.RequerySuggested -= value;
      }
    }
  }
}
