// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.GuidanceEditModel
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System;
using System.Collections.ObjectModel;

namespace BlueStacks.BlueStacksUI
{
  [Serializable]
  public abstract class GuidanceEditModel : ViewModelBase
  {
    private bool mIsEnabled = true;
    private double mMaxValue = 10.0;
    private ObservableCollection<IMActionItem> mIMActionItems = new ObservableCollection<IMActionItem>();
    private string mGuidanceText;
    private string mOriginalGuidanceKey;
    private string mGuidanceKey;
    private Type mPropertyType;
    private KeyActionType mActionType;

    public string GuidanceText
    {
      get
      {
        return this.mGuidanceText;
      }
      set
      {
        this.SetProperty<string>(ref this.mGuidanceText, value, (string) null);
      }
    }

    public bool IsEnabled
    {
      get
      {
        return this.mIsEnabled;
      }
      set
      {
        this.SetProperty<bool>(ref this.mIsEnabled, value, (string) null);
      }
    }

    public string OriginalGuidanceKey
    {
      get
      {
        return this.mOriginalGuidanceKey;
      }
      set
      {
        this.mOriginalGuidanceKey = value;
        this.GuidanceKey = this.mOriginalGuidanceKey;
      }
    }

    public string GuidanceKey
    {
      get
      {
        return this.mGuidanceKey;
      }
      set
      {
        this.SetProperty<string>(ref this.mGuidanceKey, value, (string) null);
      }
    }

    public double MaxValue
    {
      get
      {
        return this.mMaxValue;
      }
      set
      {
        this.SetProperty<double>(ref this.mMaxValue, value, (string) null);
      }
    }

    public Type PropertyType
    {
      get
      {
        return this.mPropertyType;
      }
      set
      {
        this.SetProperty<Type>(ref this.mPropertyType, value, (string) null);
      }
    }

    public KeyActionType ActionType
    {
      get
      {
        return this.mActionType;
      }
      set
      {
        this.SetProperty<KeyActionType>(ref this.mActionType, value, (string) null);
      }
    }

    public ObservableCollection<IMActionItem> IMActionItems
    {
      get
      {
        return this.mIMActionItems;
      }
      set
      {
        this.SetProperty<ObservableCollection<IMActionItem>>(ref this.mIMActionItems, value, (string) null);
      }
    }
  }
}
