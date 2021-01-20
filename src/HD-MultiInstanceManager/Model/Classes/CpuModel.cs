// Decompiled with JetBrains decompiler
// Type: MultiInstanceManagerMVVM.Model.Classes.CpuModel
// Assembly: HD-MultiInstanceManager, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 0C4C54E8-53C7-4A46-AE96-CC8E89A6B884
// Assembly location: C:\Program Files\BlueStacks\HD-MultiInstanceManager.exe

using BlueStacks.Common;
using System.ComponentModel;

namespace MultiInstanceManagerMVVM.Model.Classes
{
  public class CpuModel : INotifyPropertyChanged
  {
    private string cpuDisplayText;
    private PerformanceSetting performanceSetting;
    private int coreCount;
    private string tag;
    private string toolTip;
    private int ram;

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

    public string CpuDisplayText
    {
      get
      {
        return this.cpuDisplayText;
      }
      set
      {
        if (!(this.cpuDisplayText != value))
          return;
        this.cpuDisplayText = value;
        this.OnPropertyChanged(nameof (CpuDisplayText));
      }
    }

    public PerformanceSetting PerformanceSettingType
    {
      get
      {
        return this.performanceSetting;
      }
      set
      {
        if (this.performanceSetting == value)
          return;
        this.performanceSetting = value;
        this.OnPropertyChanged(nameof (PerformanceSettingType));
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

    public string Tag
    {
      get
      {
        return this.tag;
      }
      set
      {
        if (!(this.tag != value))
          return;
        this.tag = value;
        this.OnPropertyChanged(nameof (Tag));
      }
    }

    public string ToolTip
    {
      get
      {
        return this.toolTip;
      }
      set
      {
        if (!(this.toolTip != value))
          return;
        this.toolTip = value;
        this.OnPropertyChanged(nameof (ToolTip));
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
