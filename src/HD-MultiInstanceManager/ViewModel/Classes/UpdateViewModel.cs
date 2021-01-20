// Decompiled with JetBrains decompiler
// Type: MultiInstanceManagerMVVM.ViewModel.Classes.UpdateViewModel
// Assembly: HD-MultiInstanceManager, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 0C4C54E8-53C7-4A46-AE96-CC8E89A6B884
// Assembly location: C:\Program Files\BlueStacks\HD-MultiInstanceManager.exe

using BlueStacks.Common;
using BlueStacks.Core;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using MultiInstanceManagerMVVM.MessageModel.Classes;
using MultiInstanceManagerMVVM.Model.Classes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;

namespace MultiInstanceManagerMVVM.ViewModel.Classes
{
  public class UpdateViewModel : UiViewModelBase, IDisposable
  {
    private BackgroundWorker _bgGetUpdateDetails = (BackgroundWorker) null;
    private string _lastUpdateMessage;
    private bool _checkUpdateButtonVisibility;
    private bool _hyperLinkTextVisibility;
    private bool _checkUpdateGridVisibility;
    private bool _updateMessageVisibility;
    private bool _lastUpdateMessageVisibility;
    private string _hyperLinkText;
    private ObservableCollection<AppPlayerUpdateModel> _oemUpdateList;
    private Uri mHowToUpdateUri;

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

    public ObservableCollection<AppPlayerUpdateModel> OemUpdateList
    {
      get
      {
        return this._oemUpdateList;
      }
      set
      {
        if (this._oemUpdateList == value)
          return;
        this._oemUpdateList = value;
      }
    }

    public ObservableCollection<EngineUpdateViewModel> EngineUpdateViewModelList { get; set; }

    public string LastUpdateMessage
    {
      get
      {
        return this._lastUpdateMessage;
      }
      set
      {
        this._lastUpdateMessage = value;
        this.RaisePropertyChanged(nameof (LastUpdateMessage));
      }
    }

    public bool CheckUpdateButtonVisibility
    {
      get
      {
        return this._checkUpdateButtonVisibility;
      }
      set
      {
        this._checkUpdateButtonVisibility = value;
        this.RaisePropertyChanged(nameof (CheckUpdateButtonVisibility));
      }
    }

    public bool HyperLinkTextVisibility
    {
      get
      {
        return this._hyperLinkTextVisibility;
      }
      set
      {
        this._hyperLinkTextVisibility = value;
        this.RaisePropertyChanged(nameof (HyperLinkTextVisibility));
      }
    }

    public bool CheckUpdateGridVisibility
    {
      get
      {
        return this._checkUpdateGridVisibility;
      }
      set
      {
        this._checkUpdateGridVisibility = value;
        this.RaisePropertyChanged(nameof (CheckUpdateGridVisibility));
      }
    }

    public bool UpdateMessageVisibility
    {
      get
      {
        return this._updateMessageVisibility;
      }
      set
      {
        this._updateMessageVisibility = value;
        this.RaisePropertyChanged(nameof (UpdateMessageVisibility));
      }
    }

    public bool LastUpdateMessageVisibility
    {
      get
      {
        return this._lastUpdateMessageVisibility;
      }
      set
      {
        this._lastUpdateMessageVisibility = value;
        this.RaisePropertyChanged(nameof (LastUpdateMessageVisibility));
      }
    }

    public Uri HowToUpdateUri
    {
      get
      {
        return this.mHowToUpdateUri;
      }
      set
      {
        this.mHowToUpdateUri = value;
        this.RaisePropertyChanged(nameof (HowToUpdateUri));
      }
    }

    public string HyperLinkText
    {
      get
      {
        return this._hyperLinkText;
      }
      set
      {
        this._hyperLinkText = value;
        this.RaisePropertyChanged(nameof (HyperLinkText));
      }
    }

    public ICommand CloseWindowCommand { get; set; }

    public ICommand CheckUpdateButtonClickCommand { get; set; }

    public ICommand HyperlinkRequestNavigateCommand { get; set; }

    internal BackgroundWorker BGGetUpdateDetails
    {
      get
      {
        if (this._bgGetUpdateDetails == null)
        {
          this._bgGetUpdateDetails = new BackgroundWorker();
          this._bgGetUpdateDetails.DoWork += new DoWorkEventHandler(this._bgGetUpdateDetails_DoWork);
          this._bgGetUpdateDetails.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this._bgGetUpdateDetails_RunWorkerCompleted);
        }
        return this._bgGetUpdateDetails;
      }
    }

    public UpdateViewModel()
    {
      this.EngineUpdateViewModelList = new ObservableCollection<EngineUpdateViewModel>();
      this.CloseWindowCommand = (ICommand) new RelayCommand(new System.Action(this.CloseCurrentWindow), false);
      this.CheckUpdateButtonClickCommand = (ICommand) new RelayCommand(new System.Action(this.OnCheckUpdate), false);
      this.HyperlinkRequestNavigateCommand = (ICommand) new RelayCommand<RequestNavigateEventArgs>(new System.Action<RequestNavigateEventArgs>(this.OnHyperlinkRequestNavigate), false);
      this.UpdateMessageVisibility = true;
      this.LastUpdateMessageVisibility = false;
      this.CheckUpdateButtonVisibility = true;
      this.HyperLinkTextVisibility = false;
      this.CheckUpdateGridVisibility = false;
      this.HyperLinkText = LocaleStrings.GetLocalizedString("STRING_LEARN_HOW_TO_UPDATE_YOUR_INSTANCE", "");
      this.LastUpdateMessage = "Last Checked Yesterday";
      this.mHowToUpdateUri = new Uri(WebHelper.GetUrlWithParams(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) WebHelper.GetServerHost(), (object) "help_articles"), (string) null, (string) null, (string) null) + "&article=how_to_update_help");
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

    private void OnCheckUpdate()
    {
      Stats.SendMultiInstanceStatsAsync("check_for_update_button_clicked", "", "", "", 0, "", 0, "bgp", "", "", "", "", RegistryManager.Instance.CampaignMD5, true, "");
      this.CheckUpdateGridVisibility = true;
      this.CheckUpdateButtonVisibility = false;
      this.UpdateMessageVisibility = false;
      this.LastUpdateMessageVisibility = false;
      this.SetOemUpdateData();
    }

    private void OnHyperlinkRequestNavigate(RequestNavigateEventArgs requestNavigateEventArgs)
    {
      Stats.SendMultiInstanceStatsAsync("mim_update_checker_faq_url_clicked", "", "", "", 0, "", 0, "bgp", "", "", "", "", RegistryManager.Instance.CampaignMD5, true, "");
      Utils.OpenUrl(requestNavigateEventArgs.Uri.OriginalString);
    }

    internal void SetOemUpdateData()
    {
      if (this.BGGetUpdateDetails.IsBusy && this.EngineUpdateViewModelList.Count > 0)
        return;
      this.BGGetUpdateDetails.RunWorkerAsync();
    }

    private void _bgGetUpdateDetails_RunWorkerCompleted(
      object sender,
      RunWorkerCompletedEventArgs e)
    {
      try
      {
        if (!(e.Result is JToken result))
          return;
        this.OemUpdateList = new ObservableCollection<AppPlayerUpdateModel>();
        IJEnumerable<JToken> source = result.First<JToken>().Children().Children<JToken>();
        if (source != null && source.Any<JToken>())
        {
          foreach (object obj in (IEnumerable<JToken>) source)
            this.OemUpdateList.Add(JsonConvert.DeserializeObject<AppPlayerUpdateModel>(obj.ToString(), Utils.GetSerializerSettings()));
        }
        foreach (AppPlayerModel coexistingOem in (Collection<AppPlayerModel>) InstalledOem.CoexistingOemList)
        {
          AppPlayerModel item = coexistingOem;
          AppPlayerUpdateModel playerUpdateModel = this.OemUpdateList.Where<AppPlayerUpdateModel>((Func<AppPlayerUpdateModel, bool>) (u => u.RequestedOem == item.AppPlayerOem)).FirstOrDefault<AppPlayerUpdateModel>();
          if (playerUpdateModel != null)
            playerUpdateModel.OemDisplayName = item.AppPlayerOemDisplayName;
        }
        foreach (AppPlayerUpdateModel oemUpdate in (Collection<AppPlayerUpdateModel>) this.OemUpdateList)
        {
          AppPlayerUpdateModel item = oemUpdate;
          if (item.OemDisplayName != null && InstalledOem.InstalledCoexistingOemList.Contains(item.RequestedOem))
          {
            SimpleIoc.Default.Unregister<EngineUpdateViewModel>(item.OemDisplayName);
            SimpleIoc.Default.Register<EngineUpdateViewModel>((Func<EngineUpdateViewModel>) (() => new EngineUpdateViewModel(item)), item.OemDisplayName);
            EngineUpdateViewModel instance = SimpleIoc.Default.GetInstance<EngineUpdateViewModel>(item.OemDisplayName);
            if (!this.EngineUpdateViewModelList.Contains(instance))
              this.EngineUpdateViewModelList.Add(instance);
          }
        }
        this.RaisePropertyChanged("EngineUpdateViewModelList");
        this.CheckUpdateGridVisibility = false;
        this.HyperLinkTextVisibility = true;
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to get update details err: {0}", (object) ex.Message);
      }
    }

    private Dictionary<string, string> CreateRequestData()
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      StringBuilder sb = new StringBuilder();
      JsonTextWriter jsonTextWriter = new JsonTextWriter((TextWriter) new StringWriter(sb));
      jsonTextWriter.Formatting = Formatting.Indented;
      using (JsonWriter jsonWriter = (JsonWriter) jsonTextWriter)
      {
        dictionary.Add("source", "multi_instance_manager");
        dictionary.Add("language", RegistryManager.Instance.UserSelectedLocale);
        dictionary.Add("os_arch", "x" + SystemUtils.GetOSArchitecture().ToString());
        jsonWriter.WriteStartArray();
        foreach (RegistryManager registryManager in RegistryManager.RegistryManagers.Values)
        {
          jsonWriter.WriteStartObject();
          jsonWriter.WritePropertyName("requested_prod_ver");
          jsonWriter.WriteValue(registryManager.Version);
          jsonWriter.WritePropertyName("requested_oem");
          jsonWriter.WriteValue(registryManager.Oem);
          jsonWriter.WriteEndObject();
        }
        jsonWriter.WriteEnd();
        string str = sb.ToString();
        dictionary.Add("requested_upgrades_list", str);
        return dictionary;
      }
    }

    private void _bgGetUpdateDetails_DoWork(object sender, DoWorkEventArgs e)
    {
      JToken jtoken = (JToken) null;
      try
      {
        jtoken = JToken.Parse(BstHttpClient.Post(RegistryManager.Instance.Host + "/bs4/check_upgrade?", this.CreateRequestData(), (Dictionary<string, string>) null, false, "Android", 0, 1, 0, false, "bgp"));
        Logger.Info("Response received for check for update: " + Environment.NewLine + jtoken?.ToString());
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to get oem err: {0}", (object) ex.Message);
      }
      finally
      {
        e.Result = (object) jtoken;
      }
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!disposing || this._bgGetUpdateDetails == null)
        return;
      this._bgGetUpdateDetails.Dispose();
      this._bgGetUpdateDetails = (BackgroundWorker) null;
    }
  }
}
