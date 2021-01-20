// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.GuidanceCategoryEditModel
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System;
using System.Collections.ObjectModel;

namespace BlueStacks.BlueStacksUI
{
  [Serializable]
  public class GuidanceCategoryEditModel : ViewModelBase
  {
    private ObservableCollection<GuidanceEditModel> mGuidanceEditModels = new ObservableCollection<GuidanceEditModel>();
    private string mCategory;

    public ObservableCollection<GuidanceEditModel> GuidanceEditModels
    {
      get
      {
        return this.mGuidanceEditModels;
      }
      set
      {
        this.SetProperty<ObservableCollection<GuidanceEditModel>>(ref this.mGuidanceEditModels, value, (string) null);
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
