// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.GuidanceViewModel
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System;
using System.Collections.ObjectModel;

namespace BlueStacks.BlueStacksUI
{
  [Serializable]
  public class GuidanceViewModel : ViewModelBase
  {
    private ObservableCollection<string> mGuidanceTexts = new ObservableCollection<string>();
    private ObservableCollection<string> mGuidanceKeys = new ObservableCollection<string>();

    public Type PropertyType { get; set; }

    public ObservableCollection<string> GuidanceTexts
    {
      get
      {
        return this.mGuidanceTexts;
      }
      set
      {
        this.SetProperty<ObservableCollection<string>>(ref this.mGuidanceTexts, value, (string) null);
      }
    }

    public ObservableCollection<string> GuidanceKeys
    {
      get
      {
        return this.mGuidanceKeys;
      }
      set
      {
        this.SetProperty<ObservableCollection<string>>(ref this.mGuidanceKeys, value, (string) null);
      }
    }
  }
}
