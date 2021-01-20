// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.EngineSettingModel
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System.ComponentModel;

namespace BlueStacks.Common
{
  public class EngineSettingModel : INotifyPropertyChanged
  {
    private PerformanceSetting performanceSettingType;
    private string displayText;
    private int coreCount;
    private int ram;
    private int ramInGB;

    public PerformanceSetting PerformanceSettingType
    {
      get
      {
        return this.performanceSettingType;
      }
      set
      {
        if (this.performanceSettingType == value)
          return;
        this.performanceSettingType = value;
        this.OnPropertyChanged(nameof (PerformanceSettingType));
      }
    }

    public string DisplayText
    {
      get
      {
        return this.displayText;
      }
      set
      {
        if (!(this.displayText != value))
          return;
        this.displayText = value;
        this.OnPropertyChanged(nameof (DisplayText));
      }
    }

    public int CoreCount
    {
      get
      {
        return this.coreCount;
      }
      set
      {
        if (this.coreCount == value)
          return;
        this.coreCount = value;
        this.OnPropertyChanged(nameof (CoreCount));
      }
    }

    public int RAM
    {
      get
      {
        return this.ram;
      }
      set
      {
        if (this.ram == value)
          return;
        this.ram = value;
        this.OnPropertyChanged(nameof (RAM));
      }
    }

    public int RAMInGB
    {
      get
      {
        return this.ramInGB;
      }
      set
      {
        if (this.ramInGB == value)
          return;
        this.ramInGB = value;
        this.OnPropertyChanged(nameof (RAMInGB));
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string property)
    {
      PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
      if (propertyChanged == null)
        return;
      propertyChanged((object) this, new PropertyChangedEventArgs(property));
    }
  }
}
