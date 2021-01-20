// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.ResolutionModel
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace BlueStacks.Common
{
  [Serializable]
  public class ResolutionModel : INotifyPropertyChanged
  {
    private OrientationType orientationType;
    private Dictionary<string, string> availableResolutionsDict;
    private string combinedResolution;
    private string systemDefaultResolution;
    private int mResolutionWidth;
    private int mResolutionHeight;
    private string orientationName;

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string property)
    {
      PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
      if (propertyChanged == null)
        return;
      propertyChanged((object) this, new PropertyChangedEventArgs(property));
    }

    public OrientationType OrientationType
    {
      get
      {
        return this.orientationType;
      }
      set
      {
        if (this.orientationType == value)
          return;
        this.orientationType = value;
        this.OnPropertyChanged(nameof (OrientationType));
      }
    }

    public string OrientationName
    {
      get
      {
        return this.orientationName;
      }
      set
      {
        if (!(this.orientationName != value))
          return;
        this.orientationName = value;
        this.OnPropertyChanged(nameof (OrientationName));
      }
    }

    public Dictionary<string, string> AvailableResolutionsDict
    {
      get
      {
        return this.availableResolutionsDict;
      }
      set
      {
        if (this.availableResolutionsDict == value)
          return;
        this.availableResolutionsDict = value;
        this.OnPropertyChanged(nameof (AvailableResolutionsDict));
      }
    }

    public string CombinedResolution
    {
      get
      {
        return this.combinedResolution;
      }
      set
      {
        if (!(this.combinedResolution != value))
          return;
        this.combinedResolution = value;
        this.OnPropertyChanged(nameof (CombinedResolution));
        int width;
        int height;
        ResolutionModel.ConvertResolution(this.availableResolutionsDict.ContainsKey(this.combinedResolution) ? this.availableResolutionsDict[this.combinedResolution] : this.combinedResolution, out width, out height);
        this.ResolutionWidth = width;
        this.ResolutionHeight = height;
      }
    }

    public string SystemDefaultResolution
    {
      get
      {
        return this.systemDefaultResolution;
      }
      set
      {
        if (!(this.systemDefaultResolution != value))
          return;
        this.systemDefaultResolution = value;
        this.OnPropertyChanged(nameof (SystemDefaultResolution));
      }
    }

    public int ResolutionWidth
    {
      get
      {
        return this.mResolutionWidth;
      }
      set
      {
        if (this.mResolutionWidth == value)
          return;
        this.mResolutionWidth = value;
        this.OnPropertyChanged(nameof (ResolutionWidth));
      }
    }

    public int ResolutionHeight
    {
      get
      {
        return this.mResolutionHeight;
      }
      set
      {
        if (this.mResolutionHeight == value)
          return;
        this.mResolutionHeight = value;
        this.OnPropertyChanged(nameof (ResolutionHeight));
      }
    }

    private static void ConvertResolution(string resolution, out int width, out int height)
    {
      string[] strArray = resolution.Split('x');
      width = int.Parse(strArray[0].Trim(), (IFormatProvider) CultureInfo.InvariantCulture);
      height = int.Parse(strArray[1].Trim(), (IFormatProvider) CultureInfo.InvariantCulture);
    }
  }
}
