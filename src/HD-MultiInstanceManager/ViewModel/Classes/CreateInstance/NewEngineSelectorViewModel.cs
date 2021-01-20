// Decompiled with JetBrains decompiler
// Type: MultiInstanceManagerMVVM.ViewModel.Classes.CreateInstance.NewEngineSelectorViewModel
// Assembly: HD-MultiInstanceManager, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 0C4C54E8-53C7-4A46-AE96-CC8E89A6B884
// Assembly location: C:\Program Files\BlueStacks\HD-MultiInstanceManager.exe

using BlueStacks.Common;
using BlueStacks.Core;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MultiInstanceManagerMVVM.MessageModel.Classes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;

namespace MultiInstanceManagerMVVM.ViewModel.Classes.CreateInstance
{
  public class NewEngineSelectorViewModel : UiViewModelBase
  {
    private ObservableCollection<AppPlayerModel> mOemList;
    private AppPlayerModel mSelectedOem;
    private string mNextButtonText;
    private bool mNotInstalledTextVisibility;
    private Uri mNotSureUri;

    public ObservableCollection<AppPlayerModel> OemList
    {
      get
      {
        return this.mOemList;
      }
      set
      {
        if (this.mOemList == value)
          return;
        this.mOemList = value;
        this.RaisePropertyChanged(nameof (OemList));
      }
    }

    public AppPlayerModel SelectedOem
    {
      get
      {
        return this.mSelectedOem;
      }
      set
      {
        if (this.mSelectedOem == value)
          return;
        this.mSelectedOem = value;
        this.RaisePropertyChanged(nameof (SelectedOem));
        this.SetTextVisibility();
      }
    }

    private void SetTextVisibility()
    {
      if (RegistryManager.CheckOemInRegistry(this.SelectedOem?.AppPlayerOem, ""))
      {
        this.NotInstalledTextVisibility = false;
        this.NextButtonText = LocaleStrings.GetLocalizedString("STRING_NEXT", "");
      }
      else
      {
        this.NotInstalledTextVisibility = true;
        this.NextButtonText = LocaleStrings.GetLocalizedString("STRING_DOWNLOAD", "");
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

    public ICommand CloseWindowCommand { get; set; }

    public bool NotInstalledTextVisibility
    {
      get
      {
        return this.mNotInstalledTextVisibility;
      }
      set
      {
        this.mNotInstalledTextVisibility = value;
        this.RaisePropertyChanged(nameof (NotInstalledTextVisibility));
      }
    }

    public string NextButtonText
    {
      get
      {
        return this.mNextButtonText;
      }
      set
      {
        this.mNextButtonText = value;
        this.RaisePropertyChanged(nameof (NextButtonText));
      }
    }

    public ICommand NextButtonClickCommand { get; set; }

    public Uri NotSureUri
    {
      get
      {
        return this.mNotSureUri;
      }
      set
      {
        this.mNotSureUri = value;
        this.RaisePropertyChanged(nameof (NotSureUri));
      }
    }

    public ICommand NotSureRequestNavigateCommand { get; set; }

    public NewEngineSelectorViewModel()
    {
      this.OemList = new ObservableCollection<AppPlayerModel>();
      this.NextButtonClickCommand = (ICommand) new RelayCommand(new System.Action(this.OnNextButtonClick), false);
      this.CloseWindowCommand = (ICommand) new RelayCommand(new System.Action(this.CloseCurrentWindow), false);
      this.NotInstalledTextVisibility = false;
      this.NextButtonText = LocaleStrings.GetLocalizedString("STRING_NEXT", "");
      this.NotSureUri = new Uri(WebHelper.GetUrlWithParams(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) WebHelper.GetServerHost(), (object) "help_articles"), (string) null, (string) null, (string) null) + "&article=android_version_help");
      this.NotSureRequestNavigateCommand = (ICommand) new RelayCommand<RequestNavigateEventArgs>(new System.Action<RequestNavigateEventArgs>(this.OnNotSureLinkRequestNavigate), false);
    }

    private void OnNextButtonClick()
    {
      try
      {
        if (this.SelectedOem == null)
          return;
        this.CloseCurrentWindow();
        this.HandleNextButtonAction();
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to get oem err: {0}", (object) ex.Message);
      }
    }

    private void HandleNextButtonAction()
    {
      if (RegistryManager.CheckOemInRegistry(this.SelectedOem.AppPlayerOem, ""))
        this.OemInstalledCheckABI();
      else
        Messenger.Default.Send<InstanceCreationOemMessage>(new InstanceCreationOemMessage()
        {
          AppPlayerModel = this.SelectedOem
        });
    }

    public void OemInstalledCheckABI()
    {
      if (this.SelectedOem.AppPlayerOem.Contains("bgp64"))
        Messenger.Default.Send<InstanceCreationMessage>(new InstanceCreationMessage()
        {
          AppPlayerModel = this.SelectedOem,
          InstanceType = NewInstanceType.Fresh,
          CloneFromVm = (string) null,
          Oem = this.SelectedOem.AppPlayerOem,
          ABIValue = this.SelectedOem.AbiValue.ToString((IFormatProvider) CultureInfo.InvariantCulture)
        });
      else
        Messenger.Default.Send<InstanceCreationMessage>(new InstanceCreationMessage()
        {
          AppPlayerModel = this.SelectedOem,
          InstanceType = NewInstanceType.Fresh,
          CloneFromVm = (string) null,
          Oem = this.SelectedOem.AppPlayerOem
        });
    }

    public string GetNewInstanceSettings(int abiValue)
    {
      return new JObject()
      {
        {
          "cpu",
          (JToken) 2
        },
        {
          "ram",
          (JToken) 2048
        },
        {
          "dpi",
          (JToken) 240
        },
        {
          "abi",
          (JToken) abiValue
        },
        {
          "resolutionwidth",
          (JToken) 1920
        },
        {
          "resolutionheight",
          (JToken) 1080
        }
      }.ToString(Formatting.None);
    }

    internal void SetOemData(AppPlayerModel oem = null)
    {
      this.OemList = InstalledOem.CoexistingOemList;
      AppPlayerModel appPlayerModel;
      if (oem == null)
      {
        ObservableCollection<AppPlayerModel> oemList = this.OemList;
        appPlayerModel = oemList != null ? oemList.Where<AppPlayerModel>((Func<AppPlayerModel, bool>) (o => o.AppPlayerOem == "bgp")).FirstOrDefault<AppPlayerModel>() : (AppPlayerModel) null;
      }
      else
        appPlayerModel = oem;
      this.SelectedOem = appPlayerModel;
    }

    private void CloseCurrentWindow()
    {
      if (this.View is Window view)
        view.Close();
      Messenger.Default.Send<ControlVisibilityMessage>(new ControlVisibilityMessage()
      {
        IsVisible = false
      });
    }

    private void OnNotSureLinkRequestNavigate(RequestNavigateEventArgs requestNavigateEventArgs)
    {
      Utils.OpenUrl(requestNavigateEventArgs.Uri.OriginalString);
      Stats.SendMultiInstanceStatsAsync("not_sure_clicked", "", "", "", 0, "", 0, RegistryManager.Instance.Oem, RegistryManager.Instance.Version, RegistryManager.Instance.UserGuid, RegistryManager.Instance.RegisteredEmail, "", RegistryManager.Instance.CampaignMD5, true, "");
    }
  }
}
