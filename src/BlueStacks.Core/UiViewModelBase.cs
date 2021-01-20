// Decompiled with JetBrains decompiler
// Type: BlueStacks.Core.UiViewModelBase
// Assembly: BlueStacks.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C36AABCB-E7F4-4E86-A72F-981C56431F94
// Assembly location: C:\Program Files\BlueStacks\BlueStacks.Core.dll

using GalaSoft.MvvmLight;
using System.ComponentModel;

namespace BlueStacks.Core
{
  public abstract class UiViewModelBase : ViewModelBase, IViewModel, INotifyPropertyChanged
  {
    public IView View { get; set; }
  }
}
