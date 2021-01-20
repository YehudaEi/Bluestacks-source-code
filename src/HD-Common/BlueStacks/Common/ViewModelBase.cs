// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.ViewModelBase
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;

namespace BlueStacks.Common
{
  [Serializable]
  public abstract class ViewModelBase : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;

    protected bool SetProperty<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
    {
      if (EqualityComparer<T>.Default.Equals(field, newValue))
        return false;
      field = newValue;
      this.NotifyPropertyChanged(propertyName);
      return true;
    }

    protected void NotifyPropertyChanged(string name)
    {
      PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
      if (propertyChanged != null)
        propertyChanged((object) this, new PropertyChangedEventArgs(name));
      CommandManager.InvalidateRequerySuggested();
    }
  }
}
