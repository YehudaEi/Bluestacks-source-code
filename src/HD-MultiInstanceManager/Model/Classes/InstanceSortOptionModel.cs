// Decompiled with JetBrains decompiler
// Type: MultiInstanceManagerMVVM.Model.Classes.InstanceSortOptionModel
// Assembly: HD-MultiInstanceManager, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 0C4C54E8-53C7-4A46-AE96-CC8E89A6B884
// Assembly location: C:\Program Files\BlueStacks\HD-MultiInstanceManager.exe

using BlueStacks.Common;
using System.ComponentModel;

namespace MultiInstanceManagerMVVM.Model.Classes
{
  public class InstanceSortOptionModel : INotifyPropertyChanged
  {
    private string selectedDisplayText;
    private InstanceSortOption sortOption;

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string property)
    {
      PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
      if (propertyChanged == null)
        return;
      propertyChanged((object) this, new PropertyChangedEventArgs(property));
    }

    public string SelectedDisplayText
    {
      get
      {
        return this.selectedDisplayText;
      }
      set
      {
        if (!(this.selectedDisplayText != value))
          return;
        this.selectedDisplayText = value;
        this.OnPropertyChanged(nameof (SelectedDisplayText));
      }
    }

    public InstanceSortOption SortOption
    {
      get
      {
        return this.sortOption;
      }
      set
      {
        if (this.sortOption == value)
          return;
        this.sortOption = value;
        this.OnPropertyChanged(nameof (SortOption));
      }
    }
  }
}
