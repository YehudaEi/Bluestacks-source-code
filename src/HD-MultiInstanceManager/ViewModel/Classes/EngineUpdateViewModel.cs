// Decompiled with JetBrains decompiler
// Type: MultiInstanceManagerMVVM.ViewModel.Classes.EngineUpdateViewModel
// Assembly: HD-MultiInstanceManager, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 0C4C54E8-53C7-4A46-AE96-CC8E89A6B884
// Assembly location: C:\Program Files\BlueStacks\HD-MultiInstanceManager.exe

using BlueStacks.Common;
using BlueStacks.Core;
using MultiInstanceManagerMVVM.Model.Classes;
using System.Collections.Generic;
using System.Windows.Media;

namespace MultiInstanceManagerMVVM.ViewModel.Classes
{
  public class EngineUpdateViewModel : UiViewModelBase
  {
    private string _androidRuntime;
    private string _statusImageName;
    private string _statusText;
    private Brush _statusTextColor;
    private AppPlayerUpdateModel _appPlayerUpdateModel;

    public string AndroidRuntime
    {
      get
      {
        return this._androidRuntime;
      }
      set
      {
        this._androidRuntime = value;
        this.RaisePropertyChanged(nameof (AndroidRuntime));
      }
    }

    public string StatusImageName
    {
      get
      {
        return this._statusImageName;
      }
      set
      {
        this._statusImageName = value;
        this.RaisePropertyChanged(nameof (StatusImageName));
      }
    }

    public string StatusText
    {
      get
      {
        return this._statusText;
      }
      set
      {
        this._statusText = value;
        this.RaisePropertyChanged(nameof (StatusText));
      }
    }

    public AppPlayerUpdateModel AppPlayerUpdteModel
    {
      get
      {
        return this._appPlayerUpdateModel;
      }
      set
      {
        this._appPlayerUpdateModel = value;
        this.RaisePropertyChanged(nameof (AppPlayerUpdteModel));
      }
    }

    public Dictionary<string, string> LocaleModel
    {
      get
      {
        return BlueStacksUIBinding.Instance.LocaleModel;
      }
    }

    public Dictionary<string, Brush> ColorModel
    {
      get
      {
        return BlueStacksUIBinding.Instance.ColorModel;
      }
    }

    public Brush StatusTextColor
    {
      get
      {
        return this._statusTextColor;
      }
      set
      {
        this._statusTextColor = value;
        this.RaisePropertyChanged(nameof (StatusTextColor));
      }
    }

    public EngineUpdateViewModel(AppPlayerUpdateModel appPlayerUpdateModel)
    {
      this.AppPlayerUpdteModel = appPlayerUpdateModel;
      this.SetBindings();
    }

    private void SetBindings()
    {
      this.AndroidRuntime = this.AppPlayerUpdteModel.OemDisplayName;
      if (this.AppPlayerUpdteModel.UpdateAvailable)
      {
        this.StatusImageName = "available";
        this.StatusText = LocaleStrings.GetLocalizedString("STRING_UPDATE_AVAILABLE", "");
        this.StatusTextColor = this.ColorModel["SettingsWindowTabMenuItemForeground"];
      }
      else
      {
        this.StatusImageName = "uptodate";
        this.StatusText = LocaleStrings.GetLocalizedString("STRING_UP_TO_DATE", "");
        this.StatusTextColor = this.ColorModel["ContextMenuItemForegroundDimDimColor"];
      }
    }
  }
}
