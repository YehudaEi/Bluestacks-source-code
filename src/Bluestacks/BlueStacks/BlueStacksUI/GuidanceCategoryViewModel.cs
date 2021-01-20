// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.GuidanceCategoryViewModel
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System;
using System.Collections.ObjectModel;

namespace BlueStacks.BlueStacksUI
{
  [Serializable]
  public class GuidanceCategoryViewModel : ViewModelBase
  {
    private ObservableCollection<GuidanceViewModel> sGuidanceViewModels = new ObservableCollection<GuidanceViewModel>();
    private string mCategory;

    public ObservableCollection<GuidanceViewModel> GuidanceViewModels
    {
      get
      {
        return this.sGuidanceViewModels;
      }
      set
      {
        this.SetProperty<ObservableCollection<GuidanceViewModel>>(ref this.sGuidanceViewModels, value, (string) null);
      }
    }

    public string Category
    {
      get
      {
        return this.mCategory;
      }
      set
      {
        this.SetProperty<string>(ref this.mCategory, value, (string) null);
      }
    }
  }
}
