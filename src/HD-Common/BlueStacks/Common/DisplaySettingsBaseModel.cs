// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.DisplaySettingsBaseModel
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace BlueStacks.Common
{
  [Serializable]
  public class DisplaySettingsBaseModel : INotifyPropertyChanged
  {
    private string mDpi = "240";
    private ResolutionModel mResolutionType;

    public event PropertyChangedEventHandler PropertyChanged;

    public void OnPropertyChanged(string name)
    {
      PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
      if (propertyChanged != null)
        propertyChanged((object) this, new PropertyChangedEventArgs(name));
      CommandManager.InvalidateRequerySuggested();
    }

    public ObservableCollection<ResolutionModel> ResolutionsList { get; set; }

    public ResolutionModel ResolutionType
    {
      get
      {
        return this.mResolutionType;
      }
      set
      {
        if (value == null || this.mResolutionType == value)
          return;
        this.mResolutionType = value;
        this.OnPropertyChanged(nameof (ResolutionType));
      }
    }

    public string Dpi
    {
      get
      {
        return this.mDpi;
      }
      set
      {
        this.mDpi = value;
        this.OnPropertyChanged(nameof (Dpi));
      }
    }

    public DisplaySettingsBaseModel(string dpi, int resolutionWidth, int resolutionHeight)
    {
      this.BuildResolutionsList();
      this.InitDisplaySettingsBaseModel(dpi, resolutionWidth, resolutionHeight);
    }

    public void InitDisplaySettingsBaseModel(string dpi, int resolutionWidth, int resolutionHeight)
    {
      this.Dpi = string.IsNullOrEmpty(dpi) ? "240" : dpi;
      ResolutionModel resolutionModel = this.ResolutionsList.FirstOrDefault<ResolutionModel>((Func<ResolutionModel, bool>) (x => x.AvailableResolutionsDict.ContainsValue(string.Format("{0} x {1}", (object) resolutionWidth, (object) resolutionHeight)))) ?? this.ResolutionsList.First<ResolutionModel>((Func<ResolutionModel, bool>) (x => x.OrientationType == OrientationType.Custom));
      resolutionModel.CombinedResolution = resolutionModel.OrientationType == OrientationType.Landscape || resolutionModel.OrientationType == OrientationType.Custom ? string.Format("{0} x {1}", (object) resolutionWidth, (object) resolutionHeight) : string.Format("{0} x {1}", (object) resolutionHeight, (object) resolutionWidth);
      this.ResolutionType = resolutionModel;
    }

    private void BuildResolutionsList()
    {
      int width;
      int height;
      Utils.GetWindowWidthAndHeight(out width, out height);
      ObservableCollection<ResolutionModel> observableCollection = new ObservableCollection<ResolutionModel>();
      observableCollection.Add(new ResolutionModel()
      {
        OrientationType = OrientationType.Landscape,
        OrientationName = LocaleStrings.GetLocalizedString("STRING_ORIENTATION_LANDSCAPE", ""),
        AvailableResolutionsDict = new Dictionary<string, string>()
        {
          {
            "960 x 540",
            "960 x 540"
          },
          {
            "1280 x 720",
            "1280 x 720"
          },
          {
            "1600 x 900",
            "1600 x 900"
          },
          {
            "1920 x 1080",
            "1920 x 1080"
          },
          {
            "2560 x 1440",
            "2560 x 1440"
          }
        },
        CombinedResolution = string.Format("{0} x {1}", (object) width, (object) height),
        SystemDefaultResolution = string.Format("{0} x {1}", (object) width, (object) height)
      });
      observableCollection.Add(new ResolutionModel()
      {
        OrientationType = OrientationType.Portrait,
        OrientationName = LocaleStrings.GetLocalizedString("STRING_ORIENTATION_PORTRAIT", ""),
        AvailableResolutionsDict = new Dictionary<string, string>()
        {
          {
            "960 x 540",
            "540 x 960"
          },
          {
            "1280 x 720",
            "720 x 1280"
          },
          {
            "1600 x 900",
            "900 x 1600"
          },
          {
            "1920 x 1080",
            "1080 x 1920"
          },
          {
            "2560 x 1440",
            "1440 x 2560"
          }
        },
        CombinedResolution = string.Format("{0} x {1}", (object) width, (object) height),
        SystemDefaultResolution = string.Format("{0} x {1}", (object) height, (object) width)
      });
      observableCollection.Add(new ResolutionModel()
      {
        OrientationType = OrientationType.Custom,
        OrientationName = LocaleStrings.GetLocalizedString("STRING_CUSTOM1", ""),
        AvailableResolutionsDict = new Dictionary<string, string>(),
        CombinedResolution = string.Format("{0} x {1}", (object) width, (object) height),
        SystemDefaultResolution = string.Format("{0} x {1}", (object) width, (object) height)
      });
      this.ResolutionsList = observableCollection;
    }
  }
}
