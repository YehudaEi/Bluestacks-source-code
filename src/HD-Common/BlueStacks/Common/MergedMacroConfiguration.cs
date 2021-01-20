// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.MergedMacroConfiguration
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;

namespace BlueStacks.Common
{
  [Serializable]
  public class MergedMacroConfiguration : INotifyPropertyChanged
  {
    private int mLoopCount = 1;
    private double mAcceleration = 1.0;
    private int mLoopInterval;
    private int mDelayNextScript;
    private bool mIsGroupButtonVisible;
    private bool mIsUnGroupButtonVisible;
    private bool mIsSettingsVisible;
    private bool mIsFirstListBoxItem;
    private bool mIsLastListBoxItem;

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string property)
    {
      PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
      if (propertyChanged == null)
        return;
      propertyChanged((object) this, new PropertyChangedEventArgs(property));
    }

    [JsonIgnore]
    public int Tag { get; set; }

    [JsonProperty("MacrosToRun")]
    public ObservableCollection<string> MacrosToRun { get; } = new ObservableCollection<string>();

    [JsonProperty("LoopCount")]
    public int LoopCount
    {
      get
      {
        return this.mLoopCount;
      }
      set
      {
        this.mLoopCount = value;
        this.OnPropertyChanged(nameof (LoopCount));
      }
    }

    [JsonProperty("LoopInterval")]
    public int LoopInterval
    {
      get
      {
        return this.mLoopInterval;
      }
      set
      {
        this.mLoopInterval = value;
        this.OnPropertyChanged(nameof (LoopInterval));
      }
    }

    [JsonProperty("DelayNextScript")]
    public int DelayNextScript
    {
      get
      {
        return this.mDelayNextScript;
      }
      set
      {
        this.mDelayNextScript = value;
        this.OnPropertyChanged(nameof (DelayNextScript));
      }
    }

    [JsonProperty("Acceleration")]
    public double Acceleration
    {
      get
      {
        return this.mAcceleration;
      }
      set
      {
        this.mAcceleration = value;
        this.OnPropertyChanged(nameof (Acceleration));
      }
    }

    [JsonIgnore]
    public bool IsGroupButtonVisible
    {
      get
      {
        return this.mIsGroupButtonVisible;
      }
      set
      {
        this.mIsGroupButtonVisible = value;
        this.OnPropertyChanged(nameof (IsGroupButtonVisible));
      }
    }

    [JsonIgnore]
    public bool IsUnGroupButtonVisible
    {
      get
      {
        return this.mIsUnGroupButtonVisible;
      }
      set
      {
        this.mIsUnGroupButtonVisible = value;
        this.OnPropertyChanged(nameof (IsUnGroupButtonVisible));
      }
    }

    [JsonIgnore]
    public bool IsSettingsVisible
    {
      get
      {
        return this.mIsSettingsVisible;
      }
      set
      {
        this.mIsSettingsVisible = value;
        this.OnPropertyChanged(nameof (IsSettingsVisible));
      }
    }

    [JsonIgnore]
    public bool IsFirstListBoxItem
    {
      get
      {
        return this.mIsFirstListBoxItem;
      }
      set
      {
        this.mIsFirstListBoxItem = value;
        this.OnPropertyChanged(nameof (IsFirstListBoxItem));
      }
    }

    [JsonIgnore]
    public bool IsLastListBoxItem
    {
      get
      {
        return this.mIsLastListBoxItem;
      }
      set
      {
        this.mIsLastListBoxItem = value;
        this.OnPropertyChanged(nameof (IsLastListBoxItem));
      }
    }

    [JsonIgnore]
    public IEnumerable<string> AccelerationOptions
    {
      get
      {
        for (int i = -1; i <= 8; ++i)
          yield return ((double) (i + 2) * 0.5).ToString((IFormatProvider) CultureInfo.InvariantCulture) + "x";
      }
    }
  }
}
