// Decompiled with JetBrains decompiler
// Type: MultiInstanceManagerMVVM.ViewModel.Classes.MultiInstanceViewModel
// Assembly: HD-MultiInstanceManager, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 0C4C54E8-53C7-4A46-AE96-CC8E89A6B884
// Assembly location: C:\Program Files\BlueStacks\HD-MultiInstanceManager.exe

using BlueStacks.Common;
using BlueStacks.Core;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Win32;
using MultiInstanceManagerMVVM.Helper;
using MultiInstanceManagerMVVM.MessageModel.Classes;
using MultiInstanceManagerMVVM.Model.Classes;
using MultiInstanceManagerMVVM.View.Classes.CreateInstance;
using MultiInstanceManagerMVVM.View.Classes.Settings;
using MultiInstanceManagerMVVM.View.Classes.Update;
using MultiInstanceManagerMVVM.ViewModel.Classes.CreateInstance;
using MultiInstanceManagerMVVM.ViewModel.Classes.Settings;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MultiInstanceManagerMVVM.ViewModel.Classes
{
  public class MultiInstanceViewModel : UiViewModelBase, IDisposable
  {
    private BackgroundWorker mBgwCreateNewInstance = (BackgroundWorker) null;
    private BackgroundWorker mBgwDeleteInstance = (BackgroundWorker) null;
    private Visibility mPromotionalTeaserVisibility = Visibility.Visible;
    private BackgroundWorker mBgDiskCleanUp = (BackgroundWorker) null;
    private string mCurrentDownloadingVmname = string.Empty;
    private long mSizeInBytes = 0;
    private AppPlayerModel mCurrentDownloadingAppPlayerModel = (AppPlayerModel) null;
    private bool mIsRefreshAvailable = true;
    private bool mTriggerDownload = false;
    private string mSetSettingsAfterDownload = string.Empty;
    private string mTitle = string.Empty;
    private BitmapImage mIcon = (BitmapImage) null;
    private bool mIsShowMimHelpIcon = true;
    private bool mIsShowMimUpdateIcon = true;
    private string mMenuButtonImage = "cfgmenu_mim";
    private bool mOverlayGridVisibility;
    private string mSearchInstanceText;
    private bool mNoInstanceTextVisibility;
    private bool mFarmButtonVisibility;
    private bool mIsFarmModeEnabled;
    private bool mIsFarming;
    private bool mBatchStartButtonVisibility;
    private bool mBatchStopButtonVisibility;
    private bool mBatchVerticalSeparatorVisibility;
    private bool mHelpSupportVisibility;
    private bool mBatchDeleteButtonVisibility;
    private bool mStopAllButtonVisibility;
    private bool mSelectAllChecked;
    private Window mCurrentWindow;
    private bool mTriggerSelectCommand;
    private InstanceSortOptionModel mSelectedInstanceSortOption;
    private MimStatsModel mimStatsModel;
    private bool mConfigButtonVisibility;

    public MultiInstanceViewModel()
    {
      this.InstanceMenuViewModelDictionary = new Dictionary<string, InstanceViewModel>();
      this.InstanceList = new ObservableCollection<InstanceViewModel>();
      this.RunningInstanceList = new List<string>();
      this.DeletingInstanceList = new List<string>();
      this.CreatingNewInstancesList = new List<NewInstanceMessage>();
      this.SelectedInstancesList = new List<string>();
      ObservableCollection<InstanceSortOptionModel> observableCollection = new ObservableCollection<InstanceSortOptionModel>();
      observableCollection.Add(new InstanceSortOptionModel()
      {
        SortOption = InstanceSortOption.Name,
        SelectedDisplayText = LocaleStrings.GetLocalizedString("STRING_SORT_BY_NAME", "")
      });
      observableCollection.Add(new InstanceSortOptionModel()
      {
        SortOption = InstanceSortOption.Engine,
        SelectedDisplayText = LocaleStrings.GetLocalizedString("STRING_SORT_BY_ENGINE", "")
      });
      this.InstanceSortOptionList = observableCollection;
      this.NoInstanceTextVisibility = false;
      this.SearchInstanceText = string.Empty;
      this.OverlayGridVisibility = false;
      this.BatchStartButtonVisibility = false;
      this.BatchDeleteButtonVisibility = false;
      this.BatchStopButtonVisibility = false;
      this.StopAllButtonVisibility = false;
      this.TriggerSelectCommand = true;
      this.PromotionalTeaserVisibility = !Oem.Instance.IsShowMimPromotionalTeaser || !RegistryManager.Instance.IsShowPromotionalTeaser ? Visibility.Collapsed : Visibility.Visible;
      this.HelpSupportVisibility = Oem.Instance.IsShowMimHelpIcon;
      this.ConfigButtonVisibility = FeatureManager.Instance.ShowMiManagerMenuButton;
      this.IsShowMimUpdateIcon = Oem.Instance.IsShowMimUpdateIcon;
      this.FarmButtonVisibility = !FeatureManager.Instance.IsFarmingModeDisabled;
      InstanceSortOption sortOption = InstanceSortOption.Engine;
      if (System.Enum.IsDefined(typeof (InstanceSortOption), (object) RegistryManager.Instance.InstanceSortOption))
        sortOption = (InstanceSortOption) System.Enum.Parse(typeof (InstanceSortOption), RegistryManager.Instance.InstanceSortOption);
      this.SelectedInstanceSortOption = this.InstanceSortOptionList.First<InstanceSortOptionModel>((Func<InstanceSortOptionModel, bool>) (x => x.SortOption == sortOption));
      this.Title = LocaleStrings.GetLocalizedString("STRING_MULTIINSTANCE_MANAGER", "");
      this.Icon = new BitmapImage(new Uri(Path.Combine(RegistryManager.Instance.ClientInstallDir, "Assets\\MultiInstanceManagerLogo.ico")));
      this.InstanceList.CollectionChanged -= new NotifyCollectionChangedEventHandler(this.InstanceList_CollectionChanged);
      this.InstanceList.CollectionChanged += new NotifyCollectionChangedEventHandler(this.InstanceList_CollectionChanged);
      this.AddAllInstances();
      this.ToggleBatchOptionsVisibilty();
      this.GetWindowCommand = (ICommand) new RelayCommand<Window>(new System.Action<Window>(this.OnGetWindow), false);
      this.CloseWindowCommand = (ICommand) new RelayCommand(new System.Action(this.OnCloseWindow), false);
      this.ClosingWindowCommand = (ICommand) new RelayCommand(new System.Action(this.OnClosingWindow), false);
      this.MinimizeWindowCommand = (ICommand) new RelayCommand(new System.Action(this.OnMinimizeWindow), false);
      this.CheckForUpdateCommand = (ICommand) new RelayCommand(new System.Action(this.OnCheckForUpdate), false);
      this.SearchInstanceCommand = (ICommand) new RelayCommand(new System.Action(this.OnSearchInstance), false);
      this.CreateNewInstanceCommand = (ICommand) new RelayCommand(new System.Action(this.OnCreateNewInstance), false);
      this.ArrangeButtonClickCommand = (ICommand) new RelayCommand(new System.Action(this.OnArrangeButtonClick), false);
      this.ToggleFarmButtonClickCommand = (ICommand) new RelayCommand(new System.Action(this.OnToggleFarmButtonClick), false);
      this.SettingsButtonClickCommand = (ICommand) new RelayCommand(new System.Action(this.OnSettingsButtonClick), false);
      this.BatchStartButtonClickCommand = (ICommand) new RelayCommand(new System.Action(this.OnBatchStartButtonClick), false);
      this.BatchStopButtonClickCommand = (ICommand) new RelayCommand(new System.Action(this.OnBatchStopButtonClick), false);
      this.BatchDeleteButtonClickCommand = (ICommand) new RelayCommand(new System.Action(this.OnBatchDeleteButtonClick), false);
      this.SelectAllCheckedCommand = (ICommand) new RelayCommand(new System.Action(this.OnSelectAllChecked), false);
      this.SelectAllCheckedUnCheckedCommand = (ICommand) new RelayCommand(new System.Action(this.OnSelectAllUnChecked), false);
      this.SearchInstanceLostFocusCommand = (ICommand) new RelayCommand(new System.Action(this.OnSearchInstanceLostFocus), false);
      this.StopAllButtonClickCommand = (ICommand) new RelayCommand(new System.Action(this.OnStopAllButtonClick), false);
      this.ClosePromotionalTeaserCommand = (ICommand) new RelayCommand(new System.Action(this.OnClosePromotionalTeaser), false);
      this.Create64BitInstanceClickCommand = (ICommand) new RelayCommand(new System.Action(this.OnCreate64BitInstance), false);
      this.MIMHelpCommand = (ICommand) new RelayCommand(new System.Action(this.OnMIMHelpClick), false);
      this.LogCollectorCommand = (ICommand) new RelayCommand(new System.Action(this.OnLogCollectClick), false);
      this.RefreshWindowCommand = (ICommand) new RelayCommand(new System.Action(this.OnRefreshWindow), false);
      this.OpenedMenuPopupCommand = (ICommand) new RelayCommand(new System.Action(this.OpenedMenuPopup), false);
      this.ClosedMenuPopupCommand = (ICommand) new RelayCommand(new System.Action(this.ClosedMenuPopup), false);
      this.SortInstanceComboBoxOpenedCommand = (ICommand) new RelayCommand(new System.Action(this.OpenedSortComboBox), false);
      this.EcoModeHelpCommand = (ICommand) new RelayCommand(new System.Action(this.EcoModeHelp), false);
      Messenger.Default.Register<InstanceModelMessage>((object) this, (System.Action<InstanceModelMessage>) (action => this.ReceiveMessage(action)), false);
      Messenger.Default.Register<NewInstanceMessage>((object) this, (System.Action<NewInstanceMessage>) (action => this.CreateInstanceMessage(action)), false);
      Messenger.Default.Register<InstanceOperationMessage>((object) this, (System.Action<InstanceOperationMessage>) (action => this.PerformInstanceOperation(action)), false);
      Messenger.Default.Register<ControlVisibilityMessage>((object) this, (System.Action<ControlVisibilityMessage>) (action => this.SetOverLayGrid(action)), false);
      Messenger.Default.Register<InstanceCreationMessage>((object) this, (System.Action<InstanceCreationMessage>) (action => this.InstanceCreationWindow(action)), false);
      Messenger.Default.Register<InstanceCreationOemMessage>((object) this, (System.Action<InstanceCreationOemMessage>) (action => this.InstanceCreationOemWindow(action)), false);
      Messenger.Default.Register<int>((object) this, (System.Action<int>) (action => this.SetAndSendFpsChangeToInstancesAsync(action)), false);
      Messenger.Default.Register<bool>((object) this, (System.Action<bool>) (action => this.SendMuteUnmuteToAllRunningInstances(action)), false);
      Messenger.Default.Register<ToastPopupMessage>((object) this, (System.Action<ToastPopupMessage>) (action => this.AddToastPopup(action)), false);
      Messenger.Default.Register<CancelInstallerMessage>((object) this, (System.Action<CancelInstallerMessage>) (action => this.CancelDownLoad(action)), false);
      Messenger.Default.Register<ToggleFarmModeMessage>((object) this, (System.Action<ToggleFarmModeMessage>) (action => this.ToggleFarmMode(action)), false);
    }

    public ICommand CheckForUpdateCommand { get; set; }

    public ICommand GetWindowCommand { get; set; }

    public ICommand CloseWindowCommand { get; set; }

    public ICommand ClosingWindowCommand { get; set; }

    public ICommand MinimizeWindowCommand { get; set; }

    public ICommand SearchInstanceCommand { get; set; }

    public ICommand CreateNewInstanceCommand { get; set; }

    public ICommand ArrangeButtonClickCommand { get; set; }

    public ICommand ToggleFarmButtonClickCommand { get; set; }

    public ICommand SettingsButtonClickCommand { get; set; }

    public ICommand BatchStartButtonClickCommand { get; set; }

    public ICommand BatchStopButtonClickCommand { get; set; }

    public ICommand BatchDeleteButtonClickCommand { get; set; }

    public ICommand SelectAllCheckedCommand { get; set; }

    public ICommand SelectAllCheckedUnCheckedCommand { get; set; }

    public ICommand SearchInstanceLostFocusCommand { get; set; }

    public ICommand StopAllButtonClickCommand { get; set; }

    public ICommand ClosePromotionalTeaserCommand { get; set; }

    public ICommand Create64BitInstanceClickCommand { get; set; }

    public ICommand MIMHelpCommand { get; set; }

    public ICommand LogCollectorCommand { get; set; }

    public ICommand RefreshWindowCommand { get; set; }

    public ICommand OpenedMenuPopupCommand { get; set; }

    public ICommand ClosedMenuPopupCommand { get; set; }

    public ICommand SortInstanceComboBoxOpenedCommand { get; set; }

    public ICommand EcoModeHelpCommand { get; set; }

    public string MenuButtonImage
    {
      get
      {
        return this.mMenuButtonImage;
      }
      set
      {
        this.mMenuButtonImage = value;
        this.RaisePropertyChanged(nameof (MenuButtonImage));
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

    public Dictionary<string, InstanceViewModel> InstanceMenuViewModelDictionary { get; set; }

    public ObservableCollection<InstanceSortOptionModel> InstanceSortOptionList { get; set; }

    public List<string> RunningInstanceList { get; set; }

    public List<string> DeletingInstanceList { get; set; }

    public List<string> SelectedInstancesList { get; set; }

    public List<NewInstanceMessage> CreatingNewInstancesList { get; set; }

    public ObservableCollection<InstanceViewModel> InstanceList { get; set; }

    public Visibility PromotionalTeaserVisibility
    {
      get
      {
        return this.mPromotionalTeaserVisibility;
      }
      set
      {
        this.mPromotionalTeaserVisibility = value;
        this.RaisePropertyChanged(nameof (PromotionalTeaserVisibility));
      }
    }

    public bool IsFarmModeEnabled
    {
      get
      {
        return this.mIsFarmModeEnabled;
      }
      set
      {
        this.mIsFarmModeEnabled = value;
        this.RaisePropertyChanged(nameof (IsFarmModeEnabled));
      }
    }

    public bool FarmButtonVisibility
    {
      get
      {
        return this.mFarmButtonVisibility;
      }
      set
      {
        this.mFarmButtonVisibility = value;
        this.RaisePropertyChanged(nameof (FarmButtonVisibility));
      }
    }

    public bool IsRefreshAvailable
    {
      get
      {
        return this.mIsRefreshAvailable;
      }
      set
      {
        this.mIsRefreshAvailable = value;
        if (value)
          Application.Current.Dispatcher.Invoke((Delegate) (() =>
          {
            this.InstanceMenuViewModelDictionary.Clear();
            this.InstanceList.Clear();
            this.AddAllInstances();
            this.ToggleBatchOptionsVisibilty();
          }));
        this.RaisePropertyChanged(nameof (IsRefreshAvailable));
      }
    }

    public bool IsFarming
    {
      get
      {
        return this.mIsFarming;
      }
      set
      {
        this.mIsFarming = value;
        this.RaisePropertyChanged(nameof (IsFarming));
      }
    }

    public bool NoInstanceTextVisibility
    {
      get
      {
        return this.mNoInstanceTextVisibility;
      }
      set
      {
        this.mNoInstanceTextVisibility = value;
        this.RaisePropertyChanged(nameof (NoInstanceTextVisibility));
      }
    }

    public string SearchInstanceText
    {
      get
      {
        return this.mSearchInstanceText;
      }
      set
      {
        if (!(this.SearchInstanceText != value))
          return;
        this.mSearchInstanceText = value;
        this.RaisePropertyChanged(nameof (SearchInstanceText));
        this.SearchInstancesAndDisplayResult(this.SearchInstanceText);
      }
    }

    public bool OverlayGridVisibility
    {
      get
      {
        return this.mOverlayGridVisibility;
      }
      set
      {
        this.mOverlayGridVisibility = value;
        this.RaisePropertyChanged(nameof (OverlayGridVisibility));
      }
    }

    public bool BatchStartButtonVisibility
    {
      get
      {
        return this.mBatchStartButtonVisibility;
      }
      set
      {
        this.mBatchStartButtonVisibility = value;
        this.RaisePropertyChanged(nameof (BatchStartButtonVisibility));
      }
    }

    public bool BatchStopButtonVisibility
    {
      get
      {
        return this.mBatchStopButtonVisibility;
      }
      set
      {
        this.mBatchStopButtonVisibility = value;
        this.RaisePropertyChanged(nameof (BatchStopButtonVisibility));
      }
    }

    public bool BatchDeleteButtonVisibility
    {
      get
      {
        return this.mBatchDeleteButtonVisibility;
      }
      set
      {
        this.mBatchDeleteButtonVisibility = value;
        this.RaisePropertyChanged(nameof (BatchDeleteButtonVisibility));
      }
    }

    public bool BatchVerticalSeperatorVisibility
    {
      get
      {
        return this.mBatchVerticalSeparatorVisibility;
      }
      set
      {
        this.mBatchVerticalSeparatorVisibility = value;
        this.RaisePropertyChanged(nameof (BatchVerticalSeperatorVisibility));
      }
    }

    public bool HelpSupportVisibility
    {
      get
      {
        return this.mHelpSupportVisibility;
      }
      set
      {
        this.mHelpSupportVisibility = value;
        this.RaisePropertyChanged(nameof (HelpSupportVisibility));
      }
    }

    public bool StopAllButtonVisibility
    {
      get
      {
        return this.mStopAllButtonVisibility;
      }
      set
      {
        this.mStopAllButtonVisibility = value;
        this.RaisePropertyChanged(nameof (StopAllButtonVisibility));
      }
    }

    public bool SelectAllChecked
    {
      get
      {
        return this.mSelectAllChecked;
      }
      set
      {
        this.mSelectAllChecked = value;
        this.RaisePropertyChanged(nameof (SelectAllChecked));
      }
    }

    public bool TriggerSelectCommand
    {
      get
      {
        return this.mTriggerSelectCommand;
      }
      set
      {
        this.mTriggerSelectCommand = value;
        this.RaisePropertyChanged(nameof (TriggerSelectCommand));
      }
    }

    public InstanceSortOptionModel SelectedInstanceSortOption
    {
      get
      {
        return this.mSelectedInstanceSortOption;
      }
      set
      {
        if (this.mSelectedInstanceSortOption == value)
          return;
        if (this.mSelectedInstanceSortOption != null)
          Stats.SendMultiInstanceStatsAsync("sort_clicked", "", "", "", 0, "", this.InstanceList.Count, "bgp", "", value == null || value.SortOption != InstanceSortOption.Engine ? "byname" : "byengine", "", "", RegistryManager.Instance.CampaignMD5, true, "");
        this.mSelectedInstanceSortOption = value;
        this.RaisePropertyChanged(nameof (SelectedInstanceSortOption));
        RegistryManager.Instance.InstanceSortOption = this.mSelectedInstanceSortOption.SortOption.ToString();
        this.SortInstances();
      }
    }

    internal BackgroundWorker BGCreateNewInstance
    {
      get
      {
        if (this.mBgwCreateNewInstance == null)
        {
          this.mBgwCreateNewInstance = new BackgroundWorker();
          this.mBgwCreateNewInstance.DoWork += new DoWorkEventHandler(this.BGCreateNewInstance_DoWork);
          this.mBgwCreateNewInstance.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.BGCreateNewInstance_RunWorkerCompleted);
        }
        return this.mBgwCreateNewInstance;
      }
    }

    internal BackgroundWorker BGDeleteInstance
    {
      get
      {
        if (this.mBgwDeleteInstance == null)
        {
          this.mBgwDeleteInstance = new BackgroundWorker();
          this.mBgwDeleteInstance.DoWork += new DoWorkEventHandler(this.BGDeleteInstance_DoWork);
          this.mBgwDeleteInstance.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.BGDeleteInstance_RunWorkerCompleted);
        }
        return this.mBgwDeleteInstance;
      }
    }

    internal BackgroundWorker BGDiskCleanUp
    {
      get
      {
        if (this.mBgDiskCleanUp == null)
        {
          this.mBgDiskCleanUp = new BackgroundWorker();
          this.mBgDiskCleanUp.DoWork += new DoWorkEventHandler(this.BGDiskCleanUp_DoWork);
          this.mBgDiskCleanUp.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.BGDiskCleanUp_RunWorkerCompleted);
        }
        return this.mBgDiskCleanUp;
      }
    }

    public bool MIsDownloading { get; set; } = false;

    public string MInstallerSizeInMBString { get; set; } = "0 MB";

    public bool IsShowMimHelpIcon
    {
      get
      {
        return this.mIsShowMimHelpIcon;
      }
      set
      {
        this.mIsShowMimHelpIcon = value;
        this.RaisePropertyChanged(nameof (IsShowMimHelpIcon));
      }
    }

    public bool IsShowMimUpdateIcon
    {
      get
      {
        return this.mIsShowMimUpdateIcon;
      }
      set
      {
        this.mIsShowMimUpdateIcon = value;
        this.RaisePropertyChanged(nameof (IsShowMimUpdateIcon));
      }
    }

    public string Title
    {
      get
      {
        return this.mTitle;
      }
      set
      {
        this.mTitle = value;
        this.RaisePropertyChanged(nameof (Title));
      }
    }

    public BitmapImage Icon
    {
      get
      {
        return this.mIcon;
      }
      set
      {
        this.mIcon = value;
        this.RaisePropertyChanged(nameof (Icon));
      }
    }

    public bool ConfigButtonVisibility
    {
      get
      {
        return this.mConfigButtonVisibility;
      }
      set
      {
        this.mConfigButtonVisibility = value;
        this.RaisePropertyChanged(nameof (ConfigButtonVisibility));
      }
    }

    private void ClosedMenuPopup()
    {
      this.MenuButtonImage = "cfgmenu_mim";
    }

    private void OpenedMenuPopup()
    {
      this.MenuButtonImage = "cfgmenu_mim_hover";
    }

    private void OnGetWindow(Window window)
    {
      if (window == null)
        return;
      this.mCurrentWindow = window;
    }

    private void SetOverLayGrid(ControlVisibilityMessage action)
    {
      this.OverlayGridVisibility = action.IsVisible;
    }

    private void OnSelectAllChecked()
    {
      if (!this.TriggerSelectCommand)
        return;
      foreach (KeyValuePair<string, InstanceViewModel> instanceMenuViewModel in this.InstanceMenuViewModelDictionary)
      {
        instanceMenuViewModel.Value.InstanceCheckBoxChecked = true;
        if (!this.SelectedInstancesList.Contains(instanceMenuViewModel.Key))
          this.SelectedInstancesList.Add(instanceMenuViewModel.Key);
      }
      this.ToggleBatchOptionsVisibilty();
    }

    private void OnSelectAllUnChecked()
    {
      if (!this.TriggerSelectCommand)
        return;
      foreach (KeyValuePair<string, InstanceViewModel> instanceMenuViewModel in this.InstanceMenuViewModelDictionary)
        instanceMenuViewModel.Value.InstanceCheckBoxChecked = false;
      this.SelectedInstancesList.Clear();
      this.ToggleBatchOptionsVisibilty();
    }

    private void CreateInstanceMessage(NewInstanceMessage action)
    {
      if (action != null && this.mTriggerDownload)
      {
        this.mSetSettingsAfterDownload = action.NewInstanceSettings;
      }
      else
      {
        string str1 = action.Oem.Equals("bgp", StringComparison.InvariantCultureIgnoreCase) ? "" : "_" + action.Oem;
        this.mimStatsModel = action.MimStatsModel;
        if (action.InstanceType == NewInstanceType.Clone && this.RunningInstanceList.Contains(action.CloneFromVmName + str1))
        {
          this.ShowInstanceRunningPopup();
        }
        else
        {
          for (int index = 0; index < action.InstanceCount; ++index)
          {
            string str2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Android_{0}", (object) Utils.GetVmIdToCreate(action.Oem));
            Logger.Info(str2 + "predicted in AddInstanceControl");
            this.StartInstanceCreation(new NewInstanceMessage()
            {
              InstanceType = action.InstanceType,
              VmName = str2,
              Oem = action.Oem,
              NewInstanceSettings = action.NewInstanceSettings,
              CloneFromVmName = action.CloneFromVmName,
              DisplayName = action.DisplayName
            });
          }
        }
      }
    }

    private void StartInstanceCreation(NewInstanceMessage action)
    {
      this.CreatingNewInstancesList.Add(action);
      this.CreateNewInstance(action.InstanceType == NewInstanceType.Fresh, action.VmName, action.CloneFromVmName, action.NewInstanceSettings, action.Oem, action.DisplayName);
      this.OverlayGridVisibility = false;
      if (this.mimStatsModel == null)
        return;
      Stats.SendMultiInstanceStatsAsync(action.InstanceType == NewInstanceType.Fresh ? "instance_creation_started" : "clone_instance_started", "", this.mimStatsModel.Performance, this.mimStatsModel.Resolution, this.mimStatsModel.Abi, this.mimStatsModel.Dpi, this.mimStatsModel.InstanceCount, this.mimStatsModel.Oem, "", this.mimStatsModel.Arg1, this.mimStatsModel.InstanceType.ToString(), "", RegistryManager.Instance.CampaignMD5, true, "");
    }

    private void ShowInstanceRunningPopup()
    {
      this.mCurrentWindow.Dispatcher.Invoke((Delegate) (() =>
      {
        CustomMessageWindow customMessageWindow = new CustomMessageWindow();
        BlueStacksUIBinding.Bind(customMessageWindow.TitleTextBlock, "STRING_CANNOT_CLONE_INSTANCE", "");
        BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlock, "STRING_SELECTED_INSTANCE_RUNNING_WHILE_CLONING", "");
        customMessageWindow.AddButton(ButtonColors.Blue, "STRING_OK", (EventHandler) null, (string) null, false, (object) null, true);
        customMessageWindow.Owner = this.mCurrentWindow;
        customMessageWindow.ShowDialog();
      }));
    }

    private void ReceiveMessage(InstanceModelMessage action)
    {
      if (string.IsNullOrEmpty(action.VmName))
        return;
      this.TriggerSelectCommand = false;
      if (action.RemoveVM)
      {
        if (this.SelectedInstancesList.Contains(action.VmName))
          this.SelectedInstancesList.Remove(action.VmName);
        this.ToggleSelectAllCheckbox();
        this.ToggleBatchOptionsVisibilty();
      }
      else
      {
        if (!this.SelectedInstancesList.Contains(action.VmName))
          this.SelectedInstancesList.Add(action.VmName);
        this.ToggleSelectAllCheckbox();
        this.ToggleBatchOptionsVisibilty();
      }
      this.TriggerSelectCommand = true;
    }

    private void OnCloseWindow()
    {
      if (this.mCurrentWindow != null)
      {
        if (this.mCurrentDownloadingAppPlayerModel != null)
          this.CancelDownLoadOnClose(new CancelInstallerMessage()
          {
            IsDownLoadCanceled = true
          });
        else
          this.mCurrentWindow.Close();
      }
      if (this.mCurrentDownloadingAppPlayerModel != null)
        return;
      App.ExitApplication();
    }

    private void OnClosingWindow()
    {
      if (!ProcessUtils.IsAlreadyRunning("Global\\BlueStacks_BlueStacksUI_Lockbgp"))
        Utils.RunHDQuit(false, false, true, "bgp");
      App.ExitApplication();
    }

    private void OnCheckForUpdate()
    {
      Stats.SendMultiInstanceStatsAsync("check_for_update_icon_clicked", "", "", "", 0, "", 0, "bgp", "", "", "", "", RegistryManager.Instance.CampaignMD5, true, "");
      SimpleIoc.Default.Unregister<UpdateViewModel>();
      SimpleIoc.Default.Register<UpdateViewModel>();
      UpdateView updateView1 = new UpdateView();
      updateView1.Owner = this.mCurrentWindow;
      updateView1.WindowStartupLocation = WindowStartupLocation.CenterOwner;
      UpdateView updateView2 = updateView1;
      SimpleIoc.Default.GetInstance<UpdateViewModel>().View = (IView) updateView2;
      this.OverlayGridVisibility = true;
      updateView2.ShowDialog();
      this.ClearCheckboxSelection();
    }

    private void OnMinimizeWindow()
    {
      this.mCurrentWindow.WindowState = WindowState.Minimized;
    }

    private void OnSearchInstanceLostFocus()
    {
      if (!this.SearchInstanceText.Equals(LocaleStrings.GetLocalizedString("STRING_SEARCH", ""), StringComparison.InvariantCulture))
        return;
      this.NoInstanceTextVisibility = false;
      foreach (InstanceViewModel instanceViewModel in this.InstanceMenuViewModelDictionary.Values)
        instanceViewModel.InstanceViewVisibility = true;
    }

    private void SetAndSendFpsChangeToInstancesAsync(int action)
    {
      int num = action;
      foreach (string vm in RegistryManager.Instance.VmList)
      {
        RegistryManager.Instance.Guest[vm].FPS = num;
        Utils.UpdateValueInBootParams("fps", RegistryManager.Instance.Guest[vm].FPS.ToString((IFormatProvider) CultureInfo.InvariantCulture), vm, true, "bgp");
        if (this.RunningInstanceList.Contains(vm))
          Utils.SendChangeFPSToInstanceASync(vm, int.MaxValue, "bgp");
      }
    }

    private void SendMuteUnmuteToAllRunningInstances(bool muted)
    {
      if (this.RunningInstanceList.Count <= 0)
        return;
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      foreach (string runningInstance in this.RunningInstanceList)
      {
        string vmnameWithSuffix = InstalledOem.GetOemFromVmnameWithSuffix(runningInstance);
        string oldValue = vmnameWithSuffix.Equals("bgp", StringComparison.InvariantCultureIgnoreCase) ? "" : "_" + vmnameWithSuffix;
        string str = runningInstance;
        if (!string.IsNullOrEmpty(oldValue))
          str = runningInstance.Replace(oldValue, "");
        if (!dictionary.ContainsKey(vmnameWithSuffix))
          dictionary[vmnameWithSuffix] = str;
      }
      foreach (string key in dictionary.Keys)
        HTTPUtils.SendRequestToClientAsync("muteAllInstances", new Dictionary<string, string>()
        {
          {
            "muteInstance",
            muted.ToString((IFormatProvider) CultureInfo.InvariantCulture)
          }
        }, dictionary[key], 0, (Dictionary<string, string>) null, false, 1, 0, key);
    }

    private void OnSearchInstance()
    {
      this.SearchInstancesAndDisplayResult(this.SearchInstanceText);
    }

    private void SearchInstancesAndDisplayResult(string compareText)
    {
      if (!string.IsNullOrEmpty(compareText) && !compareText.Equals(LocaleStrings.GetLocalizedString("STRING_SEARCH", ""), StringComparison.InvariantCulture))
      {
        int num = 0;
        foreach (InstanceViewModel instanceViewModel in this.InstanceMenuViewModelDictionary.Values)
        {
          if (!instanceViewModel.InstanceDisplayName.Contains(compareText, StringComparison.InvariantCultureIgnoreCase))
          {
            instanceViewModel.InstanceViewVisibility = false;
          }
          else
          {
            ++num;
            instanceViewModel.InstanceViewVisibility = true;
          }
        }
        if (num == 0)
          this.NoInstanceTextVisibility = true;
        else
          this.NoInstanceTextVisibility = false;
      }
      else
      {
        this.NoInstanceTextVisibility = false;
        foreach (InstanceViewModel instanceViewModel in this.InstanceMenuViewModelDictionary.Values)
          instanceViewModel.InstanceViewVisibility = true;
      }
    }

    private void OnCreateNewInstance()
    {
      if (this.MIsDownloading)
      {
        this.ShowDownLoadingMessage();
      }
      else
      {
        Stats.SendMultiInstanceStatsAsync("New_instance_clicked", "", "", "", 0, "", 0, "bgp", "", "", "", "", RegistryManager.Instance.CampaignMD5, true, "");
        NewInstanceSelectorView instanceSelectorView1 = new NewInstanceSelectorView();
        instanceSelectorView1.Owner = this.mCurrentWindow;
        instanceSelectorView1.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        NewInstanceSelectorView instanceSelectorView2 = instanceSelectorView1;
        SimpleIoc.Default.GetInstance<NewInstanceSelectorViewModel>().View = (IView) instanceSelectorView2;
        this.OverlayGridVisibility = true;
        instanceSelectorView2.ShowDialog();
        this.ClearCheckboxSelection();
      }
    }

    private void ShowDownLoadingMessage()
    {
      CustomMessageWindow cw = new CustomMessageWindow();
      cw.AddWarning(LocaleStrings.GetLocalizedString("BlueStacks", "") + " " + LocaleStrings.GetLocalizedString("STRING_WARNING", ""), "message_error");
      cw.BodyTextBlock.Text = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}", (object) LocaleStrings.GetLocalizedString("STRING_DOWNLOADING_RUNTIME_MESSAGE", ""));
      cw.AddButton(ButtonColors.Red, LocaleStrings.GetLocalizedString("STRING_CLOSE", ""), (EventHandler) ((o, args) => cw.Close()), (string) null, false, (object) null, true);
      cw.CloseButtonHandle((Predicate<object>) null, (object) null);
      cw.Owner = this.mCurrentWindow;
      cw.ShowDialog();
    }

    private void OnArrangeButtonClick()
    {
      if (!ProcessUtils.IsAlreadyRunning("Global\\BlueStacks_BlueStacksUI_Lockbgp"))
        return;
      try
      {
        if (RegistryManager.Instance.ArrangeWindowMode == 0)
          HTTPUtils.SendRequestToClientAsync("tileWindow", (Dictionary<string, string>) null, "Android", 0, (Dictionary<string, string>) null, true, 1, 0, "bgp");
        else
          HTTPUtils.SendRequestToClientAsync("cascadeWindow", (Dictionary<string, string>) null, "Android", 0, (Dictionary<string, string>) null, true, 1, 0, "bgp");
      }
      catch (Exception ex)
      {
        Logger.Error(" arrange button in error" + ex.ToString());
      }
    }

    private void ToggleFarmMode(ToggleFarmModeMessage _)
    {
      this.ToggleFarmMode();
      Stats.SendMiscellaneousStatsAsync("mim_eco_mode_changed", RegistryManager.Instance.UserGuid, this.IsFarming ? "On" : "Off", "shortcut_key", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android", 0);
    }

    private void OnToggleFarmButtonClick()
    {
      this.ToggleFarmMode();
      Stats.SendMiscellaneousStatsAsync("mim_eco_mode_changed", RegistryManager.Instance.UserGuid, this.IsFarming ? "On" : "Off", "mim_button", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android", 0);
    }

    private void ToggleFarmMode()
    {
      this.IsFarming = !this.IsFarming;
      HTTPUtils.SendRequestToClientAsync("toggleFarmMode", new Dictionary<string, string>()
      {
        {
          "state",
          this.IsFarming.ToString((IFormatProvider) CultureInfo.InvariantCulture)
        }
      }, "Android", 0, (Dictionary<string, string>) null, true, 1, 0, "bgp");
    }

    private void OnSettingsButtonClick()
    {
      SimpleIoc.Default.Unregister<OptimizationSettingsViewModel>();
      SimpleIoc.Default.Register<OptimizationSettingsViewModel>();
      OptimizationSettingsView optimizationSettingsView1 = new OptimizationSettingsView();
      optimizationSettingsView1.Owner = this.mCurrentWindow;
      optimizationSettingsView1.WindowStartupLocation = WindowStartupLocation.CenterOwner;
      OptimizationSettingsView optimizationSettingsView2 = optimizationSettingsView1;
      this.OverlayGridVisibility = true;
      optimizationSettingsView2.ShowDialog();
    }

    private void AddInstance(
      string vmName,
      string oem = "bgp",
      string engineName = "",
      string name = null,
      string abi = "")
    {
      if (oem == null)
        oem = "bgp";
      string str1 = oem.Equals("bgp", StringComparison.InvariantCultureIgnoreCase) ? "" : "_" + oem;
      vmName += str1;
      if (this.InstanceMenuViewModelDictionary.ContainsKey(vmName))
        this.RemoveInstanceControl(vmName);
      SimpleIoc.Default.Unregister<InstanceViewModel>(vmName);
      SimpleIoc.Default.Register<InstanceViewModel>((Func<InstanceViewModel>) (() => new InstanceViewModel(vmName)), vmName);
      InstanceViewModel instance = SimpleIoc.Default.GetInstance<InstanceViewModel>(vmName);
      instance.InstanceViewVisibility = true;
      instance.InitProperties(abi);
      if (string.IsNullOrEmpty(engineName))
        instance.SetEngineName();
      else
        instance.EngineName = engineName;
      if (name != null)
      {
        string str2 = string.Empty;
        if (!string.IsNullOrEmpty(abi))
          str2 = InstalledOem.GetAppPlayerModel(oem, abi).Suffix;
        instance.InstanceDisplayName = name + " " + str2;
      }
      this.InstanceMenuViewModelDictionary.Add(vmName, instance);
    }

    private void AddAllInstances()
    {
      foreach (string installedCoexistingOem in InstalledOem.InstalledCoexistingOemList)
      {
        string str = installedCoexistingOem.Equals("bgp", StringComparison.InvariantCultureIgnoreCase) ? "" : "_" + installedCoexistingOem;
        RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("Software\\BlueStacks" + str + "\\Config");
        if (registryKey != null)
        {
          string[] strArray = (string[]) registryKey.GetValue("VmList", (object) new string[0]);
          if (strArray != null && (uint) strArray.Length > 0U)
          {
            foreach (string vmName in strArray)
              this.AddInstance(vmName, installedCoexistingOem, "", (string) null, "");
          }
          registryKey.Close();
        }
      }
      foreach (KeyValuePair<string, InstanceViewModel> instanceMenuViewModel in this.InstanceMenuViewModelDictionary)
      {
        if (!this.InstanceList.Contains(instanceMenuViewModel.Value))
          this.AddToInstanceList(instanceMenuViewModel.Value);
      }
    }

    private void RemoveInstanceControl(string vmName)
    {
      if (!this.InstanceMenuViewModelDictionary.ContainsKey(vmName))
        return;
      this.InstanceList.Remove(this.InstanceMenuViewModelDictionary[vmName]);
      this.InstanceMenuViewModelDictionary.Remove(vmName);
    }

    private void ToggleBatchOptionsVisibilty()
    {
      IEnumerable<string> strings = this.SelectedInstancesList.Except<string>((IEnumerable<string>) this.RunningInstanceList);
      if (strings != null && strings.Any<string>())
      {
        this.BatchStartButtonVisibility = true;
        this.BatchDeleteButtonVisibility = strings.Except<string>((IEnumerable<string>) new List<string>()
        {
          "Android"
        }).Any<string>();
      }
      else
      {
        this.BatchStartButtonVisibility = false;
        this.BatchDeleteButtonVisibility = false;
      }
      this.BatchStopButtonVisibility = this.SelectedInstancesList.Intersect<string>((IEnumerable<string>) this.RunningInstanceList).Any<string>();
      if (!this.AnyBatchOperationVisible())
        this.BatchVerticalSeperatorVisibility = false;
      else
        this.BatchVerticalSeperatorVisibility = true;
    }

    private bool AnyBatchOperationVisible()
    {
      return this.BatchStartButtonVisibility || this.BatchStopButtonVisibility || this.BatchDeleteButtonVisibility;
    }

    private void OnBatchStartButtonClick()
    {
      List<string> stoppedInstances = this.SelectedInstancesList.Except<string>((IEnumerable<string>) this.RunningInstanceList).ToList<string>();
      Logger.Info("OnBatchStartButtonClick SelectedInstancesList is: " + string.Join(",", this.SelectedInstancesList?.ToArray()));
      Logger.Info("OnBatchStartButtonClick RunningInstanceList is: " + string.Join(",", this.RunningInstanceList?.ToArray()));
      Logger.Info("OnBatchStartButtonClick stoppedInstances is: " + string.Join(",", stoppedInstances?.ToArray()));
      new Thread((ThreadStart) (() =>
      {
        try
        {
          foreach (string vmName in stoppedInstances)
          {
            try
            {
              Logger.Info("Vm present in SelectedInstancesList " + vmName);
              this.RunInstance(vmName, true);
              Thread.Sleep(Math.Max(1, RegistryManager.Instance.BatchInstanceStartInterval) * 1000);
            }
            catch (Exception ex)
            {
              Logger.Error("Failed to start instance : " + vmName);
              Logger.Error(ex.ToString());
            }
          }
        }
        catch
        {
        }
      }))
      {
        IsBackground = true
      }.Start();
      this.ClearCheckboxSelection();
    }

    private void OnBatchStopButtonClick()
    {
      CustomMessageWindow customMessageWindow = new CustomMessageWindow();
      customMessageWindow.TitleTextBlock.Text = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}", (object) LocaleStrings.GetLocalizedString("STRING_CLOSE_INSTANCES", ""));
      customMessageWindow.BodyTextBlock.Text = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}", (object) LocaleStrings.GetLocalizedString("STRING_BATCH_INSTANCE_CLOSE_CONFIRMATION", ""));
      customMessageWindow.AddButton(ButtonColors.Red, "STRING_CLOSE_SELECTED", (EventHandler) ((o, e1) => this.StopSelectedInstances()), (string) null, false, (object) null, true);
      customMessageWindow.AddButton(ButtonColors.White, "STRING_NO", (EventHandler) null, (string) null, false, (object) null, true);
      customMessageWindow.CloseButtonHandle((Predicate<object>) null, (object) null);
      customMessageWindow.Owner = this.mCurrentWindow;
      customMessageWindow.ShowDialog();
    }

    private void StopSelectedInstances()
    {
      foreach (string vmName in this.SelectedInstancesList.Intersect<string>((IEnumerable<string>) this.RunningInstanceList).ToList<string>())
      {
        Logger.Info("Vm present in SelectedInstancesList " + vmName);
        this.StopInstance(vmName);
      }
      this.ClearCheckboxSelection();
    }

    private void OnStopAllButtonClick()
    {
      CustomMessageWindow customMessageWindow = new CustomMessageWindow();
      customMessageWindow.TitleTextBlock.Text = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}", (object) LocaleStrings.GetLocalizedString("STRING_CLOSE_INSTANCES", ""));
      customMessageWindow.BodyTextBlock.Text = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}", (object) LocaleStrings.GetLocalizedString("STRING_CLOSE_ALL_INSTANCES_CONFIRMATION", ""));
      customMessageWindow.AddButton(ButtonColors.Red, "STRING_CLOSE_ALL", (EventHandler) ((o, e1) => this.StopAllInstances()), (string) null, false, (object) null, true);
      customMessageWindow.AddButton(ButtonColors.White, "STRING_NO", (EventHandler) null, (string) null, false, (object) null, true);
      customMessageWindow.CloseButtonHandle((Predicate<object>) null, (object) null);
      customMessageWindow.Owner = this.mCurrentWindow;
      customMessageWindow.ShowDialog();
    }

    private void StopAllInstances()
    {
      foreach (string vmName in this.RunningInstanceList.ToList<string>())
      {
        Logger.Info("Vm present in sRunningInstanceList" + vmName);
        this.StopInstance(vmName);
      }
      this.ClearCheckboxSelection();
    }

    private void OnBatchDeleteButtonClick()
    {
      CustomMessageWindow customMessageWindow = new CustomMessageWindow();
      customMessageWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_DELETE_INSTANCES", "");
      customMessageWindow.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_BATCH_INSTANCE_DELETE_CONFIRMATION", "");
      customMessageWindow.BodyWarningTextBlock.Visibility = Visibility.Visible;
      customMessageWindow.BodyWarningTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_BATCH_DELETE_WARNING_MESSAGE", "");
      BlueStacksUIBinding.BindColor((DependencyObject) customMessageWindow.BodyWarningTextBlock, TextBlock.ForegroundProperty, "ContextMenuItemForegroundHighlighterColor");
      customMessageWindow.AddButton(ButtonColors.Red, "STRING_DELETE", (EventHandler) ((o, e1) => this.DeleteSelectedInstances()), (string) null, false, (object) null, true);
      customMessageWindow.AddButton(ButtonColors.White, "STRING_CANCEL", (EventHandler) null, (string) null, false, (object) null, true);
      customMessageWindow.CloseButtonHandle((Predicate<object>) null, (object) null);
      customMessageWindow.Owner = this.mCurrentWindow;
      customMessageWindow.ShowDialog();
    }

    private void DeleteSelectedInstances()
    {
      foreach (string str1 in this.SelectedInstancesList.Except<string>((IEnumerable<string>) this.RunningInstanceList).ToList<string>())
      {
        string vmnameWithSuffix = InstalledOem.GetOemFromVmnameWithSuffix(str1);
        string oldValue = vmnameWithSuffix.Equals("bgp", StringComparison.InvariantCultureIgnoreCase) ? "" : "_" + vmnameWithSuffix;
        string str2 = str1;
        if (!string.IsNullOrEmpty(oldValue))
          str2 = str1.Replace(oldValue, "");
        Logger.Info("Vm present in SelectedInstancesList " + str1);
        if (!str2.Equals("Android", StringComparison.InvariantCulture))
          this.DeleteInstance(str1);
      }
      this.ClearCheckboxSelection();
    }

    private void ClearCheckboxSelection()
    {
      foreach (KeyValuePair<string, InstanceViewModel> instanceMenuViewModel in this.InstanceMenuViewModelDictionary)
        instanceMenuViewModel.Value.InstanceCheckBoxChecked = false;
      this.SelectAllChecked = false;
    }

    private void ToggleSelectAllCheckbox()
    {
      this.SelectAllChecked = this.SelectedInstancesList.Count == this.InstanceMenuViewModelDictionary.Count;
    }

    private string DeleteInstanceAgent(string vmName, string oem)
    {
      return HTTPUtils.SendRequestToAgent("deleteInstance", new Dictionary<string, string>()
      {
        ["vmname"] = vmName,
        ["isMim"] = "true"
      }, vmName, 0, (Dictionary<string, string>) null, false, 1, 0, oem, true);
    }

    private void CreateNewInstance(
      bool isFresh,
      string vmName,
      string cloneFromVm,
      string newInstanceSettings,
      string oem = "bgp",
      string name = null)
    {
      if (this.BGCreateNewInstance.IsBusy)
        return;
      JObject jobject = JObject.Parse(newInstanceSettings);
      string empty = string.Empty;
      if (jobject.ContainsKey("abi"))
        empty = jobject["abi"].ToString();
      this.AddInstance(vmName, oem, "", name, empty);
      this.BGCreateNewInstance.RunWorkerAsync((object) new List<object>()
      {
        (object) isFresh,
        (object) vmName,
        (object) cloneFromVm,
        (object) newInstanceSettings,
        (object) oem
      });
    }

    private void DeleteInstanceConfirmation(string vmName)
    {
      CustomMessageWindow customMessageWindow = new CustomMessageWindow();
      customMessageWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_DELETE_INSTANCE", "");
      customMessageWindow.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_DELETE_CONFIRMATION", "");
      customMessageWindow.AddButton(ButtonColors.Red, "STRING_DELETE", (EventHandler) ((o, e1) => this.DeleteInstance(vmName)), (string) null, false, (object) null, true);
      customMessageWindow.AddButton(ButtonColors.White, "STRING_CANCEL", (EventHandler) null, (string) null, false, (object) null, true);
      customMessageWindow.CloseButtonHandle((Predicate<object>) null, (object) null);
      customMessageWindow.Owner = this.mCurrentWindow;
      customMessageWindow.ShowDialog();
    }

    private void DeleteInstance(string vmName)
    {
      foreach (string installedCoexistingOem in InstalledOem.InstalledCoexistingOemList)
      {
        string oem = installedCoexistingOem;
        if (!RegistryManager.CheckOemInRegistry(oem, ""))
        {
          foreach (KeyValuePair<string, InstanceViewModel> keyValuePair in this.InstanceMenuViewModelDictionary.Where<KeyValuePair<string, InstanceViewModel>>((Func<KeyValuePair<string, InstanceViewModel>, bool>) (kvp => kvp.Value.OEM == oem)).ToList<KeyValuePair<string, InstanceViewModel>>())
          {
            this.InstanceMenuViewModelDictionary.Remove(keyValuePair.Key);
            this.InstanceList.Remove(keyValuePair.Value);
          }
        }
      }
      if (!this.CheckOemAndUpdateCollection(vmName))
        return;
      this.InstanceMenuViewModelDictionary[vmName].UpdateState(InstanceState.Deleting, "");
      this.DeletingInstanceList.Add(vmName);
      this.StartDeletingInstance(vmName);
      this.OverlayGridVisibility = false;
    }

    private void StartDeletingInstance(string vmName)
    {
      if (this.BGDeleteInstance.IsBusy)
        return;
      this.BGDeleteInstance.RunWorkerAsync((object) vmName);
    }

    private void RunInstance(string vmName, bool isForBatch)
    {
      try
      {
        if (this.RunningInstanceList.Contains(vmName))
          return;
        string partnerExePath = RegistryManager.Instance.PartnerExePath;
        string vmnameWithSuffix = InstalledOem.GetOemFromVmnameWithSuffix(vmName);
        string oldValue = vmnameWithSuffix.Equals("bgp", StringComparison.InvariantCultureIgnoreCase) ? "" : "_" + vmnameWithSuffix;
        RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("Software\\BlueStacks" + oldValue + "\\Config");
        if (registryKey != null)
        {
          string exePath = (string) registryKey.GetValue("PartnerExePath", (object) "");
          if (!string.IsNullOrEmpty(oldValue))
            vmName = vmName.Replace(oldValue, "");
          if (!ProcessUtils.IsAlreadyRunning("Global\\BlueStacks_BlueStacksUI_Lock" + vmnameWithSuffix))
          {
            ProcessUtils.GetProcessObject(exePath, "-vmname " + vmName, false).Start();
            Thread.Sleep(500);
            int num = 30;
            while (((Utils.PingPartner(vmnameWithSuffix, vmName) ? 0 : (num > 0 ? 1 : 0)) & (isForBatch ? 1 : 0)) != 0)
            {
              --num;
              Thread.Sleep(1000);
            }
          }
          else
          {
            Dictionary<string, string> data = new Dictionary<string, string>()
            {
              {
                "vmname",
                vmName
              }
            };
            Logger.Info("sending show window for vmName: {0}", (object) vmName);
            HTTPUtils.SendRequestToClient("showWindow", data, "Android", 0, (Dictionary<string, string>) null, false, 1, 0, vmnameWithSuffix);
            Thread.Sleep(500);
          }
          registryKey.Close();
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in running instance with vm = " + vmName + " Exception: " + ex.ToString());
      }
    }

    private void StopInstanceConfirmation(string vmName)
    {
      this.mCurrentWindow.Dispatcher.Invoke((Delegate) (() =>
      {
        CustomMessageWindow customMessageWindow = new CustomMessageWindow();
        customMessageWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_INSTANCE_CLOSE_TITLE", "");
        customMessageWindow.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_CLOSE_CONFIRMATION", "");
        customMessageWindow.AddButton(ButtonColors.Red, "STRING_CLOSE", (EventHandler) ((o, e1) =>
        {
          this.InstanceMenuViewModelDictionary[vmName].UpdateState(InstanceState.Stopping, "");
          this.StopInstance(vmName);
        }), (string) null, false, (object) null, true);
        customMessageWindow.AddButton(ButtonColors.White, "STRING_NO", (EventHandler) null, (string) null, false, (object) null, true);
        customMessageWindow.CloseButtonHandle((Predicate<object>) null, (object) null);
        customMessageWindow.Owner = this.mCurrentWindow;
        customMessageWindow.ShowDialog();
      }));
    }

    private void StopInstance(string vmName)
    {
      try
      {
        if (!this.RunningInstanceList.Contains(vmName))
          return;
        string vmnameWithSuffix = InstalledOem.GetOemFromVmnameWithSuffix(vmName);
        string oldValue = vmnameWithSuffix.Equals("bgp", StringComparison.InvariantCultureIgnoreCase) ? "" : "_" + vmnameWithSuffix;
        if (!string.IsNullOrEmpty(oldValue))
          vmName = vmName.Replace(oldValue, "");
        HTTPUtils.SendRequestToClientAsync("stopInstance", (Dictionary<string, string>) null, vmName, 0, (Dictionary<string, string>) null, false, 1, 0, vmnameWithSuffix);
      }
      catch (Exception ex)
      {
        Logger.Error("Error in stopping instance : " + vmName + " - " + ex?.ToString());
      }
    }

    private void UpdateInstanceShortcutName(string lastDisplayName, string vmName)
    {
      try
      {
        string vmnameWithSuffix = InstalledOem.GetOemFromVmnameWithSuffix(vmName);
        string oldValue = vmnameWithSuffix.Equals("bgp", StringComparison.InvariantCultureIgnoreCase) ? "" : "_" + vmnameWithSuffix;
        if (!string.IsNullOrEmpty(oldValue))
          vmName = vmName.Replace(oldValue, "");
        string path2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}-{1}.lnk", (object) BlueStacks.Common.Strings.ProductDisplayName, (object) lastDisplayName);
        string str = Path.Combine(ShortcutHelper.sDesktopPath, path2);
        if (!System.IO.File.Exists(str))
          return;
        string destFileName = Path.Combine(ShortcutHelper.sDesktopPath, this.GetShortcutName(vmName, vmnameWithSuffix));
        System.IO.File.Move(str, destFileName);
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to update instance shortcut name. Ex : " + ex.ToString());
      }
    }

    private string GetShortcutName(string vMName, string oem = "bgp")
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}-{1}.lnk", (object) BlueStacks.Common.Strings.ProductDisplayName, (object) Utils.GetDisplayName(vMName, oem));
    }

    private bool CreateInstanceShortcut(string vmName, string oem = "bgp")
    {
      try
      {
        string empty = string.Empty;
        string targetApplication = !System.IO.File.Exists(Path.Combine(RegistryManager.RegistryManagers[oem].InstallDir, "BlueStacks.exe")) ? Path.Combine(RegistryManager.RegistryManagers[oem].ClientInstallDir, "BlueStacks.exe") : Path.Combine(RegistryManager.RegistryManagers[oem].InstallDir, "BlueStacks.exe");
        ShortcutHelper.CreateDesktopShortcut(this.GetShortcutName(vmName, oem), Path.Combine(RegistryManager.RegistryManagers[oem].InstallDir, "ProductLogo.ico"), targetApplication, "-vmname " + vmName, string.Empty, "");
        return true;
      }
      catch (Exception ex)
      {
        Logger.Error("Error Creating shortcut" + ex.ToString());
        return false;
      }
    }

    private void AddInstanceShortCut(string vmName)
    {
      try
      {
        string vmnameWithSuffix = InstalledOem.GetOemFromVmnameWithSuffix(vmName);
        string oldValue = vmnameWithSuffix.Equals("bgp", StringComparison.InvariantCultureIgnoreCase) ? "" : "_" + vmnameWithSuffix;
        if (!string.IsNullOrEmpty(oldValue))
          vmName = vmName.Replace(oldValue, "");
        if (!this.CreateInstanceShortcut(vmName, vmnameWithSuffix))
          return;
        CustomMessageWindow customMessageWindow = new CustomMessageWindow();
        customMessageWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_SHORTCUT_CREATED", "");
        customMessageWindow.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_SHORTCUT_ADDED_SUCCESS", "");
        customMessageWindow.AddButton(ButtonColors.Blue, "STRING_CONFIRM", (EventHandler) null, (string) null, false, (object) null, true);
        customMessageWindow.CloseButtonHandle((Predicate<object>) null, (object) null);
        customMessageWindow.Owner = this.mCurrentWindow;
        customMessageWindow.ShowDialog();
      }
      catch (Exception ex)
      {
        Logger.Error("Error in adding instance to UI" + ex?.ToString());
      }
    }

    private void StartDiskCleanUp(string args)
    {
      if (this.BGDiskCleanUp.IsBusy || ProcessUtils.IsAlreadyRunning("Global\\BlueStacks_DiskCompactor_Lockbgp"))
      {
        CustomMessageWindow customMessageWindow = new CustomMessageWindow()
        {
          ImageName = "disk_cleanup_popup_window"
        };
        customMessageWindow.TitleTextBlock.Text = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}", (object) LocaleStrings.GetLocalizedString("STRING_DISK_CLEANUP_MULTIPLE_RUN_HEADING", ""));
        customMessageWindow.BodyTextBlock.Text = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}", (object) LocaleStrings.GetLocalizedString("STRING_DISK_CLEANUP_MULTIPLE_RUN_MESSAGE", ""));
        customMessageWindow.AddButton(ButtonColors.Blue, "STRING_OK", (EventHandler) null, (string) null, false, (object) null, true);
        customMessageWindow.CloseButtonHandle((Predicate<object>) null, (object) null);
        customMessageWindow.Owner = this.mCurrentWindow;
        customMessageWindow.ShowDialog();
      }
      else
      {
        CustomMessageWindow customMessageWindow = new CustomMessageWindow()
        {
          ImageName = "disk_cleanup_popup_window"
        };
        customMessageWindow.TitleTextBlock.Text = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}", (object) LocaleStrings.GetLocalizedString("STRING_DISK_CLEANUP", ""));
        customMessageWindow.BodyTextBlockTitle.Text = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}", (object) LocaleStrings.GetLocalizedString("STRING_DISK_CLEANUP_MESSAGE", ""));
        customMessageWindow.BodyTextBlockTitle.Visibility = Visibility.Visible;
        customMessageWindow.BodyTextBlockTitle.FontWeight = FontWeights.Regular;
        customMessageWindow.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_CONTINUE_CONFIRMATION", "");
        customMessageWindow.AddButton(ButtonColors.White, "STRING_CLOSE", (EventHandler) null, (string) null, false, (object) null, true);
        customMessageWindow.AddButton(ButtonColors.Blue, "STRING_CONTINUE", (EventHandler) ((o, e1) => this.BGDiskCleanUp.RunWorkerAsync((object) args)), (string) null, false, (object) null, true);
        customMessageWindow.CloseButtonHandle((Predicate<object>) null, (object) null);
        customMessageWindow.Owner = this.mCurrentWindow;
        customMessageWindow.ShowDialog();
      }
    }

    private bool CheckOemAndUpdateCollection(string vmName)
    {
      string oem = InstalledOem.GetOemFromVmnameWithSuffix(vmName);
      if (RegistryManager.CheckOemInRegistry(oem, ""))
        return true;
      foreach (KeyValuePair<string, InstanceViewModel> keyValuePair in this.InstanceMenuViewModelDictionary.Where<KeyValuePair<string, InstanceViewModel>>((Func<KeyValuePair<string, InstanceViewModel>, bool>) (kvp => kvp.Value.OEM == oem)).ToList<KeyValuePair<string, InstanceViewModel>>())
      {
        this.InstanceMenuViewModelDictionary.Remove(keyValuePair.Key);
        this.InstanceList.Remove(keyValuePair.Value);
      }
      return false;
    }

    private void PerformInstanceOperation(InstanceOperationMessage instanceOperation)
    {
      if (!this.CheckOemAndUpdateCollection(instanceOperation.VmName))
        return;
      switch (instanceOperation.Operation)
      {
        case Operation.AddFromShortCut:
          this.AddInstanceShortCut(instanceOperation.VmName);
          break;
        case Operation.DeleteInstance:
          this.DeleteInstanceConfirmation(instanceOperation.VmName);
          break;
        case Operation.UpdateInstanceName:
          this.UpdateInstanceShortcutName(instanceOperation.LastVMName, instanceOperation.VmName);
          this.SortInstances();
          ThreadPool.QueueUserWorkItem((WaitCallback) (obj => this.mCurrentWindow.Dispatcher.Invoke((Delegate) (() =>
          {
            if (!(this.InstanceMenuViewModelDictionary[instanceOperation.VmName].View is FrameworkElement view))
              return;
            view.BringIntoView();
          }))));
          break;
        case Operation.DiskCleanUp:
          this.StartDiskCleanUp(instanceOperation.VmName);
          break;
        case Operation.CloneInstance:
          this.InstanceCreationWindow(new InstanceCreationMessage()
          {
            AppPlayerModel = instanceOperation.AppPlayerModel,
            InstanceType = NewInstanceType.Clone,
            CloneFromVm = instanceOperation.VmName
          });
          break;
        case Operation.RunInstance:
          this.RunInstance(instanceOperation.VmName, false);
          break;
        case Operation.StopInstance:
          this.StopInstanceConfirmation(instanceOperation.VmName);
          break;
        case Operation.AddToRunningInstance:
          if (!this.RunningInstanceList.Contains(instanceOperation.VmName))
          {
            this.RunningInstanceList.Add(instanceOperation.VmName);
            this.StopAllButtonVisibility = true;
          }
          this.IsFarmModeEnabled = this.RunningInstanceList.Count > 0;
          break;
        case Operation.RemoveFromInstance:
          this.RunningInstanceList.Remove(instanceOperation.VmName);
          if (this.RunningInstanceList.Count == 0)
            this.StopAllButtonVisibility = false;
          this.IsFarmModeEnabled = this.RunningInstanceList.Count > 0;
          if (this.RunningInstanceList.Count != 0)
            break;
          this.IsFarming = false;
          break;
        case Operation.OpenInstanceSettings:
          SettingsView settingsView = new SettingsView(instanceOperation.VmName, "", this.IsFarming);
          settingsView.Owner = this.mCurrentWindow;
          settingsView.WindowStartupLocation = WindowStartupLocation.CenterOwner;
          settingsView.ShowDialog();
          break;
        case Operation.VerifyInstanceName:
          bool flag = true;
          foreach (InstanceViewModel instance in (Collection<InstanceViewModel>) this.InstanceList)
          {
            if (string.Equals(instanceOperation.NewVmDisplayName, instance.InstanceDisplayName.Trim(), StringComparison.InvariantCultureIgnoreCase))
            {
              flag = false;
              IMessenger messenger = Messenger.Default;
              InstanceOperationMessage message = new InstanceOperationMessage();
              message.Operation = Operation.VerifyInstanceName;
              message.IsNewVmDisplayNameVerified = false;
              string vmName = instanceOperation.VmName;
              messenger.Send<InstanceOperationMessage>(message, (object) vmName);
              break;
            }
          }
          if (!flag)
            break;
          IMessenger messenger1 = Messenger.Default;
          InstanceOperationMessage message1 = new InstanceOperationMessage();
          message1.Operation = Operation.VerifyInstanceName;
          message1.IsNewVmDisplayNameVerified = true;
          string vmName1 = instanceOperation.VmName;
          messenger1.Send<InstanceOperationMessage>(message1, (object) vmName1);
          break;
      }
    }

    private void InstanceCreationWindow(InstanceCreationMessage action)
    {
      SimpleIoc.Default.Unregister<CreateNewInstanceViewModel>(action.InstanceType.ToString());
      if (this.mTriggerDownload)
        SimpleIoc.Default.Register<CreateNewInstanceViewModel>((Func<CreateNewInstanceViewModel>) (() => new CreateNewInstanceViewModel(action.InstanceType, action.AppPlayerModel, action.CloneFromVm, action.Oem, action.ABIValue, false)), action.InstanceType.ToString());
      else
        SimpleIoc.Default.Register<CreateNewInstanceViewModel>((Func<CreateNewInstanceViewModel>) (() => new CreateNewInstanceViewModel(action.InstanceType, action.AppPlayerModel, action.CloneFromVm, action.Oem, action.ABIValue, true)), action.InstanceType.ToString());
      CreateNewInstanceViewModel instance = SimpleIoc.Default.GetInstance<CreateNewInstanceViewModel>(action.InstanceType.ToString());
      CreateNewInstanceView createNewInstanceView1 = new CreateNewInstanceView();
      createNewInstanceView1.Owner = this.mCurrentWindow;
      createNewInstanceView1.WindowStartupLocation = WindowStartupLocation.CenterOwner;
      createNewInstanceView1.DataContext = (object) instance;
      CreateNewInstanceView createNewInstanceView2 = createNewInstanceView1;
      instance.View = (IView) createNewInstanceView2;
      this.OverlayGridVisibility = true;
      createNewInstanceView2.ShowDialog();
    }

    private void InstanceCreationOemWindow(InstanceCreationOemMessage action)
    {
      try
      {
        if (this.MIsDownloading)
        {
          CustomMessageWindow customMessageWindow = new CustomMessageWindow();
          customMessageWindow.MessageIcon.ImageName = "message_error";
          customMessageWindow.MessageIcon.Visibility = Visibility.Visible;
          BlueStacksUIBinding.Bind(customMessageWindow.TitleTextBlock, "STRING_PLEASE_WAIT", "");
          BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlock, "STRING_ANDROID_DOWNLOAD_IS_IN_PROGRESS", "");
          customMessageWindow.AddButton(ButtonColors.Red, "STRING_CLOSE", (EventHandler) ((o, e1) => {}), (string) null, false, (object) null, true);
          customMessageWindow.ShowDialog();
        }
        else
        {
          this.mTriggerDownload = true;
          this.InstanceCreationWindow(new InstanceCreationMessage()
          {
            InstanceType = NewInstanceType.Fresh,
            AppPlayerModel = action.AppPlayerModel,
            Oem = action.AppPlayerModel.AppPlayerOem,
            ABIValue = action.AppPlayerModel.AbiValue.ToString((IFormatProvider) CultureInfo.InvariantCulture)
          });
          if (!this.mTriggerDownload)
            return;
          this.DownLoadInstanceOperation(action);
          this.mTriggerDownload = false;
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to create oem instance: " + ex?.ToString());
      }
    }

    private void DownLoadInstanceOperation(InstanceCreationOemMessage action)
    {
      try
      {
        this.mCurrentDownloadingVmname = "Android" + (action.AppPlayerModel.AppPlayerOem.Equals("bgp", StringComparison.InvariantCultureIgnoreCase) ? "" : "_" + action.AppPlayerModel.AppPlayerOem);
        this.mCurrentDownloadingAppPlayerModel = action.AppPlayerModel;
        this.mOverlayGridVisibility = false;
        if (string.IsNullOrEmpty(action.AppPlayerModel.DownLoadUrl))
          return;
        this.AddInstance("Android", action.AppPlayerModel.AppPlayerOem, action.AppPlayerModel.AppPlayerOemDisplayName, BlueStacks.Common.Strings.ProductDisplayName, action.AppPlayerModel.AbiValue.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        InstanceViewModel instanceMenuViewModel = this.InstanceMenuViewModelDictionary[this.mCurrentDownloadingVmname];
        instanceMenuViewModel.UpdateState(InstanceState.Stopped, "");
        this.AddToInstanceList(instanceMenuViewModel);
        instanceMenuViewModel.RedDotVisibility = true;
        instanceMenuViewModel.StatusTextVisibility = false;
        instanceMenuViewModel.ProgressBarVisibility = true;
        instanceMenuViewModel.UpdateState(InstanceState.Downloading, "");
        if (action.AppPlayerModel.DownLoadOem(new Downloader.DownloadExceptionEventHandler(this.Downloader_DownloadException), new Downloader.DownloadProgressChangedEventHandler(this.Downloader_DownloadProgressChanged), new Downloader.DownloadFileCompletedEventHandler(this.Downloader_DownloadFileCompleted), new Downloader.FilePayloadInfoReceivedHandler(this.Downloader_FilePayloadInfoReceived), new Downloader.UnsupportedResumeEventHandler(this.Downloader_UnsupportedResume), false))
        {
          this.MIsDownloading = true;
          Stats.SendMultiInstanceStatsAsync("oem_download_started", action.AppPlayerModel.AppPlayerOemDisplayName, "", "", 0, "", 0, action.AppPlayerModel.AppPlayerOem, action.AppPlayerModel.AppPlayerProdVer, "", "", "", RegistryManager.Instance.CampaignMD5, true, "");
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to create oem instance: " + ex?.ToString());
        if (action.AppPlayerModel != null)
          Stats.SendMultiInstanceStatsAsync("oem_download_failed", action.AppPlayerModel.AppPlayerOemDisplayName, "", "", 0, "", 0, action.AppPlayerModel.AppPlayerOem, action.AppPlayerModel.AppPlayerProdVer, "", "", "", RegistryManager.Instance.CampaignMD5, true, "");
        this.UpdateDataForDownloadFailure();
      }
    }

    private void DownLoadComplete()
    {
      try
      {
        InstanceViewModel instanceViewModel = this.InstanceList.Where<InstanceViewModel>((Func<InstanceViewModel, bool>) (i => i.VmNameWithSuffix == this.mCurrentDownloadingVmname)).FirstOrDefault<InstanceViewModel>();
        instanceViewModel.UpdateState(InstanceState.Installing, "");
        this.MIsDownloading = false;
        instanceViewModel.StatusTextVisibility = true;
        instanceViewModel.ProgressBarVisibility = false;
        this.InstallOemOperation();
      }
      catch (Exception ex)
      {
        Logger.Error("Failed after downloading oem installer: " + ex?.ToString());
        this.UpdateDataForDownloadFailure();
      }
    }

    private void InstallOemOperation()
    {
      try
      {
        int code = this.mCurrentDownloadingAppPlayerModel.InstallOem();
        if (code != 0 || !RegistryManager.CheckOemInRegistry(this.mCurrentDownloadingAppPlayerModel.AppPlayerOem, "Android"))
        {
          Logger.Warning("Installation failed");
          Stats.SendMultiInstanceStatsAsync("oem_installation_failed", this.mCurrentDownloadingAppPlayerModel.AppPlayerOemDisplayName, "", "", 0, "", 0, this.mCurrentDownloadingAppPlayerModel.AppPlayerOem, "", "", "", "", RegistryManager.Instance.CampaignMD5, true, "");
          this.ShowInstallationError(code);
        }
        else
        {
          InstanceViewModel instanceViewModel = this.InstanceList.Where<InstanceViewModel>((Func<InstanceViewModel, bool>) (i => i.VmNameWithSuffix == this.mCurrentDownloadingVmname)).FirstOrDefault<InstanceViewModel>();
          if (instanceViewModel == null)
            return;
          instanceViewModel.UpdateState(InstanceState.Stopped, "");
          InstalledOem.SetInstalledCoexistingOems();
          if (instanceViewModel.OEM.Contains("bgp64"))
            Utils.UpdateValueInBootParams("abivalue", this.mCurrentDownloadingAppPlayerModel.AbiValue.ToString((IFormatProvider) CultureInfo.InvariantCulture), instanceViewModel.VmName, true, instanceViewModel.OEM);
          RegistryManager.RegistryManagers[instanceViewModel.OEM].Guest[instanceViewModel.VmName].DisplayName = BlueStacks.Common.Strings.ProductDisplayName + " " + this.mCurrentDownloadingAppPlayerModel.Suffix;
          if (!string.IsNullOrEmpty(this.mSetSettingsAfterDownload))
          {
            foreach (KeyValuePair<string, JToken> keyValuePair in JObject.Parse(this.mSetSettingsAfterDownload))
            {
              switch (keyValuePair.Key)
              {
                case "cpu":
                  RegistryManager.RegistryManagers[instanceViewModel.OEM].Guest[instanceViewModel.VmName].VCPUs = (int) keyValuePair.Value;
                  break;
                case "ram":
                  RegistryManager.RegistryManagers[instanceViewModel.OEM].Guest[instanceViewModel.VmName].Memory = (int) keyValuePair.Value;
                  break;
                case "resolutionheight":
                  RegistryManager.RegistryManagers[instanceViewModel.OEM].Guest[instanceViewModel.VmName].GuestHeight = (int) keyValuePair.Value;
                  break;
                case "resolutionwidth":
                  RegistryManager.RegistryManagers[instanceViewModel.OEM].Guest[instanceViewModel.VmName].GuestWidth = (int) keyValuePair.Value;
                  break;
                case "dpi":
                  Utils.UpdateValueInBootParams("DPI", keyValuePair.Value.ToString(), instanceViewModel.VmName, true, instanceViewModel.OEM);
                  break;
              }
            }
            this.mSetSettingsAfterDownload = string.Empty;
          }
          Stats.SendMultiInstanceStatsAsync("oem_installation_completed", instanceViewModel.InstanceDisplayName, "", "", 0, "", 0, instanceViewModel.OEM, "", "", "", "", RegistryManager.Instance.CampaignMD5, true, "");
          this.mCurrentDownloadingVmname = string.Empty;
          this.mCurrentDownloadingAppPlayerModel = (AppPlayerModel) null;
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Failed after running installer process: " + ex?.ToString());
        this.UpdateDataForDownloadFailure();
      }
    }

    private void ShowInstallationError(int code)
    {
      this.mCurrentWindow.Dispatcher.Invoke((Delegate) (() =>
      {
        string str = code == 0 ? LocaleStrings.GetLocalizedString("STRING_INSTALLATION_FAILED", "") : InstallerErrorHandling.AssignErrorStringForInstallerExitCodes(code, "STRING_INSTALLATION_FAILED");
        CustomMessageWindow customMessageWindow = new CustomMessageWindow();
        customMessageWindow.TitleTextBlock.Text = string.Format((IFormatProvider) CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_INSTALL_FAIL", ""));
        customMessageWindow.BodyTextBlock.Text = str;
        customMessageWindow.AddButton(ButtonColors.Blue, LocaleStrings.GetLocalizedString("STRING_OKAY", ""), new EventHandler(this.HandleInstallationErrorButton), (string) null, false, (object) null, true);
        customMessageWindow.CloseButtonHandle(new EventHandler(this.HandleInstallationErrorButton), (object) null);
        customMessageWindow.Owner = this.mCurrentWindow;
        customMessageWindow.ShowDialog();
      }));
    }

    private void HandleInstallationErrorButton(object sender, EventArgs e)
    {
      this.UpdateDataForDownloadFailure();
    }

    private void Downloader_UnsupportedResume(HttpStatusCode sc)
    {
      this.mCurrentWindow.Dispatcher.Invoke((Delegate) (() =>
      {
        CustomMessageWindow customMessageWindow = new CustomMessageWindow();
        customMessageWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_DOWNLOAD_FAILED", "");
        customMessageWindow.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_FAILED_DOWNLOAD_RETRY", "");
        customMessageWindow.AddButton(ButtonColors.Blue, "STRING_RETRY", new EventHandler(this.HandleRetryDownload), (string) null, false, (object) null, true);
        customMessageWindow.CloseButtonHandle((EventHandler) ((sender1, e1) => this.UpdateDataForDownloadFailure()), (object) null);
        customMessageWindow.Owner = this.mCurrentWindow;
        customMessageWindow.ShowDialog();
      }));
    }

    private void HandleRetryDownload(object sender, EventArgs e)
    {
      this.mCurrentDownloadingAppPlayerModel.DownLoadOem(new Downloader.DownloadExceptionEventHandler(this.Downloader_DownloadException), new Downloader.DownloadProgressChangedEventHandler(this.Downloader_DownloadProgressChanged), new Downloader.DownloadFileCompletedEventHandler(this.Downloader_DownloadFileCompleted), new Downloader.FilePayloadInfoReceivedHandler(this.Downloader_FilePayloadInfoReceived), new Downloader.UnsupportedResumeEventHandler(this.Downloader_UnsupportedResume), true);
    }

    private void Downloader_DownloadFileCompleted(object sender, EventArgs args)
    {
      Stats.SendMultiInstanceStatsAsync("oem_download_completed", this.mCurrentDownloadingAppPlayerModel.AppPlayerOemDisplayName, "", "", 0, "", 0, this.mCurrentDownloadingAppPlayerModel.AppPlayerOem, this.mCurrentDownloadingAppPlayerModel.AppPlayerProdVer, "", "", "", RegistryManager.Instance.CampaignMD5, true, "");
      this.DownLoadComplete();
    }

    private void Downloader_FilePayloadInfoReceived(long fileSize)
    {
      this.mCurrentWindow.Dispatcher.Invoke((Delegate) (() =>
      {
        this.mSizeInBytes = fileSize;
        this.MInstallerSizeInMBString = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} MB", (object) (this.mSizeInBytes / 1048576L));
      }));
    }

    private void Downloader_DownloadProgressChanged(long size)
    {
      Decimal percent = Decimal.Divide((Decimal) size, (Decimal) this.mSizeInBytes) * new Decimal(100);
      this.mCurrentWindow.Dispatcher.Invoke((Delegate) (() =>
      {
        InstanceViewModel instanceViewModel = this.InstanceList.Where<InstanceViewModel>((Func<InstanceViewModel, bool>) (i => i.VmNameWithSuffix == this.mCurrentDownloadingVmname)).FirstOrDefault<InstanceViewModel>();
        if (instanceViewModel == null)
          return;
        instanceViewModel.UpdateDownloadProgress = (int) percent;
      }));
    }

    private void Downloader_DownloadException(Exception e)
    {
      this.mCurrentWindow.Dispatcher.Invoke((Delegate) (() =>
      {
        Logger.Error("Failed to download file: {0}.", (object) e.Message);
        this.UpdateDataForDownloadFailure();
        if (this.mCurrentDownloadingAppPlayerModel != null)
          Stats.SendMultiInstanceStatsAsync("oem_download_failed", this.mCurrentDownloadingAppPlayerModel.AppPlayerOemDisplayName, "", "", 0, "", 0, this.mCurrentDownloadingAppPlayerModel.AppPlayerOem, this.mCurrentDownloadingAppPlayerModel.AppPlayerProdVer, "", "", "", RegistryManager.Instance.CampaignMD5, true, "");
        CustomMessageWindow customMessageWindow = new CustomMessageWindow();
        customMessageWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_DOWNLOAD_FAILED", "");
        customMessageWindow.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_ERROR_RECORDER_DOWNLOAD", "");
        customMessageWindow.AddButton(ButtonColors.Blue, "STRING_OKAY", (EventHandler) null, (string) null, false, (object) null, true);
        customMessageWindow.CloseButtonHandle((Predicate<object>) null, (object) null);
        customMessageWindow.Owner = this.mCurrentWindow;
        customMessageWindow.ShowDialog();
      }));
    }

    private void CancelDownLoadOnClose(CancelInstallerMessage action)
    {
      if (action == null || !action.IsDownLoadCanceled)
        return;
      CustomMessageWindow customMessageWindow = new CustomMessageWindow();
      customMessageWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_DOWNLOAD_IN_PROGRESS", "");
      customMessageWindow.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_MULTI_INSTANCE_DOWNLOAD_CANCEL", "");
      customMessageWindow.AddButton(ButtonColors.White, LocaleStrings.GetLocalizedString("STRING_RETURN_APP", ""), (EventHandler) null, (string) null, false, (object) null, true);
      customMessageWindow.AddButton(ButtonColors.Red, LocaleStrings.GetLocalizedString("STRING_CLOSE", ""), (EventHandler) ((sender1, e1) => this.CancelDownLoadAndSendCancelStat()), (string) null, false, (object) null, true);
      customMessageWindow.CloseButtonHandle((Predicate<object>) null, (object) null);
      customMessageWindow.ShowDialog();
    }

    private void CancelDownLoad(CancelInstallerMessage action)
    {
      if (action == null)
        return;
      if (action.IsDownLoadCanceled)
      {
        CustomMessageWindow customMessageWindow = new CustomMessageWindow();
        customMessageWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_ASK_CANCEL_DOWNLOAD", "");
        customMessageWindow.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_ASK_CANCEL_DOWNLOAD_MESSAGE", "");
        customMessageWindow.AddButton(ButtonColors.Red, LocaleStrings.GetLocalizedString("STRING_YES", ""), (EventHandler) ((sender1, e1) => this.CancelDownLoadAndSendCancelStat()), (string) null, false, (object) null, true);
        customMessageWindow.AddButton(ButtonColors.White, LocaleStrings.GetLocalizedString("STRING_NO", ""), (EventHandler) null, (string) null, false, (object) null, true);
        customMessageWindow.CloseButtonHandle((Predicate<object>) null, (object) null);
        customMessageWindow.ShowDialog();
      }
      if (action.IsCancelledBeforeDownload)
        this.mTriggerDownload = false;
    }

    private void CancelDownLoadAndSendCancelStat()
    {
      if (this.mCurrentDownloadingAppPlayerModel != null)
        Stats.SendMultiInstanceStatsAsync("oem_download_canceled", this.mCurrentDownloadingAppPlayerModel.AppPlayerOemDisplayName, "", "", 0, "", 0, this.mCurrentDownloadingAppPlayerModel.AppPlayerOem, this.mCurrentDownloadingAppPlayerModel.AppPlayerProdVer, "", "", "", RegistryManager.Instance.CampaignMD5, true, "");
      this.UpdateDataForDownloadFailure();
    }

    private void UpdateDataForDownloadFailure()
    {
      this.mCurrentWindow.Dispatcher.Invoke((Delegate) (() =>
      {
        try
        {
          this.mCurrentDownloadingAppPlayerModel.CancelOemDownload();
          this.MIsDownloading = false;
          InstanceViewModel instanceViewModel = this.InstanceList.Where<InstanceViewModel>((Func<InstanceViewModel, bool>) (i => i.VmNameWithSuffix == this.mCurrentDownloadingVmname)).FirstOrDefault<InstanceViewModel>();
          if (instanceViewModel != null)
            this.InstanceList.Remove(instanceViewModel);
          if (instanceViewModel != null && this.InstanceMenuViewModelDictionary.ContainsKey(instanceViewModel.VmNameWithSuffix))
            this.RemoveInstanceControl(instanceViewModel.VmNameWithSuffix);
          this.mCurrentDownloadingVmname = string.Empty;
          this.mCurrentDownloadingAppPlayerModel = (AppPlayerModel) null;
        }
        catch (Exception ex)
        {
          Logger.Error("Exception while handling installation failure: {0}.", (object) ex);
        }
      }));
    }

    private void SetNewInstanceProperties(string vmName)
    {
      InstanceViewModel instanceMenuViewModel = this.InstanceMenuViewModelDictionary[vmName];
      string vmnameWithSuffix = InstalledOem.GetOemFromVmnameWithSuffix(vmName);
      string oldValue = vmnameWithSuffix.Equals("bgp", StringComparison.InvariantCultureIgnoreCase) ? "" : "_" + vmnameWithSuffix;
      if (!string.IsNullOrEmpty(oldValue))
        vmName = vmName.Replace(oldValue, "");
      RegistryManager.RegistryManagers[vmnameWithSuffix].Guest[vmName].DisplayName = instanceMenuViewModel.InstanceDisplayName;
      instanceMenuViewModel.SetEngineName();
      instanceMenuViewModel.UpdateState(InstanceState.Stopped, "");
      this.AddToInstanceList(instanceMenuViewModel);
      instanceMenuViewModel.RedDotVisibility = true;
    }

    private void AddToInstanceList(InstanceViewModel newInstance)
    {
      if (this.InstanceList.Contains(newInstance))
        return;
      this.InstanceList.Add(newInstance);
      this.SortInstances();
    }

    private void AddToastPopup(ToastPopupMessage toastPopupMessage)
    {
      Application.Current.Dispatcher.Invoke((Delegate) (() =>
      {
        try
        {
          CustomToastPopupControl toastPopupControl = new CustomToastPopupControl(this.mCurrentWindow);
          if (toastPopupMessage.IsShowCloseImage)
          {
            toastPopupControl.Init(this.mCurrentWindow, toastPopupMessage.Message, (Brush) Brushes.Black, (Brush) null, HorizontalAlignment.Center, VerticalAlignment.Top, new Thickness?(), 12, new Thickness?(), (Brush) null, toastPopupMessage.IsShowCloseImage, false);
            toastPopupControl.Margin = new Thickness(0.0, 40.0, 0.0, 0.0);
          }
          else
            toastPopupControl.Init(this.mCurrentWindow, toastPopupMessage.Message, (Brush) Brushes.Black, (Brush) null, HorizontalAlignment.Center, VerticalAlignment.Center, new Thickness?(), 12, new Thickness?(), (Brush) null, false, false);
          toastPopupControl.ShowPopup(toastPopupMessage.Duration);
        }
        catch (Exception ex)
        {
          Logger.Error("Exception in showing toast popup: " + ex.ToString());
        }
      }));
    }

    private void EcoModeHelp()
    {
      Stats.SendMultiInstanceStatsAsync("mim_eco_mode_faq_icon_clicked", "", "", "", 0, "", 0, "bgp", "", "", "", "", RegistryManager.Instance.CampaignMD5, true, "");
      Utils.OpenUrl(WebHelper.GetUrlWithParams(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) WebHelper.GetServerHost(), (object) "help_articles"), (string) null, (string) null, (string) null) + "&article=EcoMode_help");
    }

    private void OnMIMHelpClick()
    {
      Stats.SendMultiInstanceStatsAsync("mim_faq_icon_clicked", "", "", "", 0, "", 0, "bgp", "", "", "", "", RegistryManager.Instance.CampaignMD5, true, "");
      Utils.OpenUrl(WebHelper.GetUrlWithParams(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) WebHelper.GetServerHost(), (object) "help_articles"), (string) null, (string) null, (string) null) + "&article=MIM_help");
    }

    private void OnLogCollectClick()
    {
      string installDir = RegistryStrings.InstallDir;
      Logger.Info("Open log collector through mim from dir: " + installDir);
      Process.Start(new ProcessStartInfo()
      {
        Arguments = "-Source=MIM",
        CreateNoWindow = true,
        WindowStyle = ProcessWindowStyle.Hidden,
        FileName = Path.Combine(installDir, "HD-LogCollector.exe"),
        UseShellExecute = false
      });
    }

    private void OnCreate64BitInstance()
    {
      if (this.MIsDownloading)
      {
        this.ShowDownLoadingMessage();
      }
      else
      {
        Stats.SendMultiInstanceStatsAsync("mim_teaser_createInstanceButton_clicked", "", "", "", 0, "", 0, "bgp", "", "", "", "", RegistryManager.Instance.CampaignMD5, true, "");
        Messenger.Default.Send<ControlVisibilityMessage>(new ControlVisibilityMessage()
        {
          IsVisible = true
        });
        SimpleIoc.Default.Unregister<NewEngineSelectorViewModel>();
        SimpleIoc.Default.Register<NewEngineSelectorViewModel>();
        NewEngineSelectorView engineSelectorView1 = new NewEngineSelectorView();
        engineSelectorView1.Owner = this.mCurrentWindow;
        engineSelectorView1.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        NewEngineSelectorView engineSelectorView2 = engineSelectorView1;
        SimpleIoc.Default.GetInstance<NewEngineSelectorViewModel>().View = (IView) engineSelectorView2;
        SimpleIoc.Default.GetInstance<NewEngineSelectorViewModel>().SetOemData(InstalledOem.GetAppPlayerModel("bgp64", ABISetting.ARM64.GetDescription()));
        engineSelectorView2.ShowDialog();
        this.ClearCheckboxSelection();
      }
    }

    private void OnClosePromotionalTeaser()
    {
      this.PromotionalTeaserVisibility = Visibility.Collapsed;
      Stats.SendMultiInstanceStatsAsync("mim_teaser_closeButton_clicked", "", "", "", 0, "", 0, "bgp", "", "", "", "", RegistryManager.Instance.CampaignMD5, true, "");
      RegistryManager.Instance.IsShowPromotionalTeaser = false;
    }

    private void InstanceList_CollectionChanged(
      object sender,
      NotifyCollectionChangedEventArgs args)
    {
      for (int index = 0; index < this.InstanceList.Count; ++index)
        this.InstanceList[index].InstanceIndex = index;
    }

    private void OnRefreshWindow()
    {
      this.IsRefreshAvailable = false;
      new Thread((ThreadStart) (() =>
      {
        InstalledOem.GetCoexistingOemsFromCloud();
        do
          ;
        while (!InstalledOem.CloudResponseRecieved);
        this.IsRefreshAvailable = true;
      }))
      {
        IsBackground = true
      }.Start();
    }

    private void SortInstances()
    {
      Logger.Info("Sort instance by: " + this.SelectedInstanceSortOption.SortOption.ToString());
      List<InstanceViewModel> instanceViewModelList = new List<InstanceViewModel>((IEnumerable<InstanceViewModel>) this.InstanceList);
      switch (this.SelectedInstanceSortOption.SortOption)
      {
        case InstanceSortOption.Name:
          NaturalSortComparer comparer = new NaturalSortComparer();
          instanceViewModelList.Sort((Comparison<InstanceViewModel>) ((x, y) => comparer.Compare(x.InstanceName.Trim(), y.InstanceName.Trim())));
          break;
        case InstanceSortOption.Engine:
          instanceViewModelList.Sort((Comparison<InstanceViewModel>) ((x, y) => string.Equals(x.EngineName, y.EngineName, StringComparison.InvariantCultureIgnoreCase) ? (x.VmName.Length == y.VmName.Length ? string.Compare(x.VmName, y.VmName, StringComparison.InvariantCultureIgnoreCase) : x.VmName.Length.CompareTo(y.VmName.Length)) : string.Compare(x.EngineName, y.EngineName, StringComparison.InvariantCultureIgnoreCase)));
          break;
      }
      for (int newIndex = 0; newIndex < instanceViewModelList.Count; ++newIndex)
      {
        if (this.InstanceList.IndexOf(instanceViewModelList[newIndex]) != newIndex)
          this.InstanceList.Move(this.InstanceList.IndexOf(instanceViewModelList[newIndex]), newIndex);
      }
    }

    private void OpenedSortComboBox()
    {
      Stats.SendMultiInstanceStatsAsync("sort_clicked", "", "", "", 0, "", this.InstanceList.Count, "bgp", "", "", "", "", RegistryManager.Instance.CampaignMD5, true, "");
    }

    private void BGDiskCleanUp_DoWork(object sender, DoWorkEventArgs e)
    {
      try
      {
        if (this.RunningInstanceList.Count > 0)
          HTTPUtils.SendRequestToClient("hideBluestacks", (Dictionary<string, string>) null, "Android", 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
      }
      catch (Exception ex)
      {
        Logger.Info("Client server not running");
      }
      string vmNameWithSuffix = e.Argument.ToString();
      string vmnameWithSuffix = InstalledOem.GetOemFromVmnameWithSuffix(vmNameWithSuffix);
      string oldValue = vmnameWithSuffix.Equals("bgp", StringComparison.InvariantCultureIgnoreCase) ? "" : "_" + vmnameWithSuffix;
      if (!string.IsNullOrEmpty(oldValue))
        vmNameWithSuffix = vmNameWithSuffix.Replace(oldValue, "");
      Process process = new Process();
      process.StartInfo.FileName = Path.Combine(RegistryManager.RegistryManagers[vmnameWithSuffix].InstallDir, "DiskCompactionTool.exe");
      process.StartInfo.Arguments = vmNameWithSuffix;
      process.Start();
      process.WaitForExit();
      e.Result = (object) process.ExitCode;
    }

    private void BGDiskCleanUp_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      if ((uint) (int) e?.Result > 0U)
        Logger.Error("Some error occured exitcode: " + e.Result.ToString());
      else
        Logger.Info("Disk cleaup successfully completed");
    }

    private void BGCreateNewInstance_DoWork(object sender, DoWorkEventArgs e)
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      try
      {
        Dictionary<string, string> data = new Dictionary<string, string>();
        List<object> objectList = e.Argument as List<object>;
        string oem = (string) objectList[4] ?? "bgp";
        if ((bool) objectList[0])
        {
          dictionary["vmtype"] = "fresh";
          data["vmtype"] = "fresh";
        }
        else
        {
          dictionary["vmtype"] = "clone";
          data["vmtype"] = "clone";
          data["clonefromvm"] = (string) objectList[2];
        }
        data["settings"] = (string) objectList[3];
        data["vmname"] = (string) objectList[1];
        data["isMim"] = "true";
        string str1 = oem.Equals("bgp", StringComparison.InvariantCultureIgnoreCase) ? "" : "_" + oem;
        dictionary["vmname"] = (string) objectList[1] + str1;
        int timeout = 240000;
        JObject jobject = JObject.Parse(HTTPUtils.SendRequestToAgent("createInstance", data, "Android", timeout, (Dictionary<string, string>) null, false, 1, 0, oem, true));
        if (jobject["success"].ToObject<bool>())
        {
          RegistryManager.ClearRegistryMangerInstance();
          string str2 = JObject.Parse(jobject["vmconfig"].ToString().Trim())["vmname"].ToString().Trim();
          dictionary["vmname"] = str2 + str1;
          dictionary["status"] = "success";
        }
        else
        {
          dictionary["status"] = "fail";
          dictionary["reason"] = jobject["reason"].ToString().Trim();
        }
      }
      catch (Exception ex)
      {
        Logger.Error("error in creating new instance" + ex.ToString());
        dictionary["status"] = "fail";
        dictionary["reason"] = "UnknownException";
      }
      finally
      {
        e.Result = (object) dictionary;
      }
    }

    private void BGCreateNewInstance_RunWorkerCompleted(
      object sender,
      RunWorkerCompletedEventArgs e)
    {
      if (!(e.Result is Dictionary<string, string> result))
        return;
      if (result["status"].Equals("success", StringComparison.InvariantCultureIgnoreCase))
      {
        InstalledOem.SetInstalledCoexistingOems();
        this.SetNewInstanceProperties(result["vmname"]);
        string vmIdFromVmName = Utils.GetVmIdFromVmName(result["vmname"]);
        if (this.mimStatsModel != null)
          Stats.SendMultiInstanceStatsAsync(this.mimStatsModel.InstanceType == NewInstanceType.Fresh ? "instance_creation_completed" : "clone_instance_completed", "", this.mimStatsModel.Performance, this.mimStatsModel.Resolution, this.mimStatsModel.Abi, this.mimStatsModel.Dpi, this.mimStatsModel.InstanceCount, this.mimStatsModel.Oem, "", this.mimStatsModel.Arg1, this.mimStatsModel.InstanceType.ToString(), vmIdFromVmName, RegistryManager.Instance.CampaignMD5, true, "");
      }
      else if (result["status"].Equals("fail", StringComparison.InvariantCultureIgnoreCase))
      {
        Logger.Info("Error in creating instance : " + result["reason"]);
        CustomMessageWindow customMessageWindow = new CustomMessageWindow();
        MultiInstanceErrorCodesEnum instanceErrorCodesEnum = EnumHelper.Parse<MultiInstanceErrorCodesEnum>(result["reason"], MultiInstanceErrorCodesEnum.UnknownException);
        BlueStacksUIBinding.Bind(customMessageWindow.TitleTextBlock, "STRING_CREATE_INSTANCE_ERROR", "");
        string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}{2} {3}", (object) LocaleStrings.GetLocalizedString("STRING_MULTI_INSTANCE_CREATE_ERROR", ""), (object) Environment.NewLine, (object) LocaleStrings.GetLocalizedString("STRING_ERROR_CODE_COLON", ""), (object) instanceErrorCodesEnum);
        customMessageWindow.BodyTextBlock.Text = str;
        customMessageWindow.AddButton(ButtonColors.Blue, "STRING_OK", (EventHandler) null, (string) null, false, (object) null, true);
        customMessageWindow.Owner = this.mCurrentWindow;
        customMessageWindow.ShowDialog();
        this.RemoveInstanceControl(result["vmname"]);
        if (this.mimStatsModel != null)
          Stats.SendMultiInstanceStatsAsync(this.mimStatsModel.InstanceType == NewInstanceType.Fresh ? "instance_creation_failed" : "clone_instance_failed", "", this.mimStatsModel.Performance, this.mimStatsModel.Resolution, this.mimStatsModel.Abi, this.mimStatsModel.Dpi, this.mimStatsModel.InstanceCount, this.mimStatsModel.Oem, "", this.mimStatsModel.Arg1, this.mimStatsModel.InstanceType.ToString(), "", RegistryManager.Instance.CampaignMD5, true, "");
      }
      string vmName = result["vmname"];
      string vmnameWithSuffix = InstalledOem.GetOemFromVmnameWithSuffix(vmName);
      string oldValue = vmnameWithSuffix.Equals("bgp", StringComparison.InvariantCultureIgnoreCase) ? "" : "_" + vmnameWithSuffix;
      if (!string.IsNullOrEmpty(oldValue))
        vmName = vmName.Replace(oldValue, "");
      if (this.CreatingNewInstancesList.FirstOrDefault<NewInstanceMessage>((Func<NewInstanceMessage, bool>) (x => x.VmName == vmName)) != null)
        this.CreatingNewInstancesList.Remove(this.CreatingNewInstancesList.FirstOrDefault<NewInstanceMessage>((Func<NewInstanceMessage, bool>) (x => x.VmName == vmName)));
      if (this.CreatingNewInstancesList.Count > 0)
      {
        NewInstanceMessage newInstanceMessage = this.CreatingNewInstancesList.First<NewInstanceMessage>();
        this.CreateNewInstance(newInstanceMessage.InstanceType == NewInstanceType.Fresh, newInstanceMessage.VmName, newInstanceMessage.CloneFromVmName, newInstanceMessage.NewInstanceSettings, vmnameWithSuffix, (string) null);
      }
    }

    private void BGDeleteInstance_DoWork(object sender, DoWorkEventArgs e)
    {
      string str = e.Argument as string;
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      try
      {
        dictionary["vmname"] = str;
        string vmnameWithSuffix = InstalledOem.GetOemFromVmnameWithSuffix(str);
        string displayName = Utils.GetDisplayName(str, vmnameWithSuffix);
        string oldValue = vmnameWithSuffix.Equals("bgp", StringComparison.InvariantCultureIgnoreCase) ? "" : "_" + vmnameWithSuffix;
        if (!string.IsNullOrEmpty(oldValue))
          str = str.Replace(oldValue, "");
        JObject jobject = JObject.Parse(this.DeleteInstanceAgent(str, vmnameWithSuffix));
        string vmIdFromVmName = Utils.GetVmIdFromVmName(dictionary["vmname"]);
        if (jobject["success"].ToObject<bool>())
        {
          dictionary["status"] = "success";
          ShortcutHelper.DeleteDesktopShortcut(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}-{1}.lnk", (object) BlueStacks.Common.Strings.ProductDisplayName, (object) displayName));
          Stats.SendMultiInstanceStatsAsync("instance_deleted", str, "", "", 0, "", 0, vmnameWithSuffix, "", dictionary["status"], "", vmIdFromVmName, RegistryManager.Instance.CampaignMD5, true, "");
        }
        else
        {
          dictionary["status"] = "fail";
          Stats.SendMultiInstanceStatsAsync("instance_deletion_failed", str, "", "", 0, "", 0, vmnameWithSuffix, "", dictionary["status"], "", vmIdFromVmName, RegistryManager.Instance.CampaignMD5, true, "");
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in deleting instance... Err : " + ex.ToString());
        dictionary["status"] = "fail";
      }
      finally
      {
        e.Result = (object) dictionary;
      }
    }

    private void BGDeleteInstance_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      Dictionary<string, string> result = e.Result as Dictionary<string, string>;
      Logger.Info("Deleting vm = " + result["vmname"] + " status = " + result["status"]);
      this.DeletingInstanceList.Remove(result["vmname"]);
      this.RemoveInstanceControl(result["vmname"]);
      if (this.SelectedInstancesList.Contains(result["vmname"]))
        this.SelectedInstancesList.Remove(result["vmname"]);
      InstalledOem.SetInstalledCoexistingOems();
      this.ToggleSelectAllCheckbox();
      if (this.DeletingInstanceList.Count <= 0)
        return;
      this.StartDeletingInstance(this.DeletingInstanceList[0]);
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      if (this.mBgwCreateNewInstance != null)
      {
        this.mBgwCreateNewInstance.Dispose();
        this.mBgwCreateNewInstance = (BackgroundWorker) null;
      }
      if (this.mBgwDeleteInstance != null)
      {
        this.mBgwDeleteInstance.Dispose();
        this.mBgwDeleteInstance = (BackgroundWorker) null;
      }
      if (this.mBgDiskCleanUp != null)
      {
        this.mBgDiskCleanUp.Dispose();
        this.mBgDiskCleanUp = (BackgroundWorker) null;
      }
    }
  }
}
