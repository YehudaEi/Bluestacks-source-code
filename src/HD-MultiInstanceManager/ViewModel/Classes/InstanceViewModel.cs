// Decompiled with JetBrains decompiler
// Type: MultiInstanceManagerMVVM.ViewModel.Classes.InstanceViewModel
// Assembly: HD-MultiInstanceManager, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 0C4C54E8-53C7-4A46-AE96-CC8E89A6B884
// Assembly location: C:\Program Files\BlueStacks\HD-MultiInstanceManager.exe

using BlueStacks.Common;
using BlueStacks.Core;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MultiInstanceManagerMVVM.Helper;
using MultiInstanceManagerMVVM.MessageModel.Classes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace MultiInstanceManagerMVVM.ViewModel.Classes
{
  public class InstanceViewModel : UiViewModelBase
  {
    private AppPlayerModel mAppPlayerModel = (AppPlayerModel) null;
    private string mVmName = string.Empty;
    private string mLastVmDisplayName = string.Empty;
    private bool mReadOnlyInstanceName;
    private string mInstanceName;
    private bool mEditInstanceVisibility;
    private bool mIsChildInstance;
    private string mEditInstanceImage;
    private string mInstanceDisplayName;
    private bool mInstanceCheckBoxChecked;
    private int mInstanceIndex;
    private bool mStartButtonVisibility;
    private bool mStopButtonVisibility;
    private bool mOptionPanelVisibility;
    private bool mInstanceViewVisibility;
    private string mStatusText;
    private InstanceState mCurrentState;
    private bool mSetFocusOnInstanceName;
    private bool mRedDotVisibility;
    private bool mStatusTextVisibility;
    private bool mProgressBarVisibility;
    private string mEngineName;
    private int mUpdateDownloadProgress;
    private bool mIsDisplayNameVerified;

    public InstanceViewModel(string vmName)
    {
      this.ReadOnlyInstanceName = true;
      this.InstanceName = string.Empty;
      this.StartButtonVisibility = true;
      this.StopButtonVisibility = false;
      this.OptionPanelVisibility = true;
      this.InstanceViewVisibility = true;
      this.EditInstanceVisibility = false;
      this.IsChildInstance = true;
      this.EditInstanceImage = "edit_icon";
      this.StatusText = this.LocaleModel.ContainsKey("STRING_STOPPED") ? this.LocaleModel["STRING_STOPPED"] : "Stopped";
      this.SetFocusOnInstanceName = false;
      this.RedDotVisibility = false;
      this.StatusTextVisibility = true;
      this.ProgressBarVisibility = false;
      this.UpdateDownloadProgress = 0;
      this.InstanceIndex = 0;
      this.VmNameWithSuffix = vmName;
      this.InstanceViewMouseEnterCommand = (ICommand) new RelayCommand(new System.Action(this.OnInstanceViewMouseEnter), false);
      this.InstanceViewMouseLeaveCommand = (ICommand) new RelayCommand(new System.Action(this.OnInstanceViewMouseLeave), false);
      this.DeleteInstanceClickCommand = (ICommand) new RelayCommand(new System.Action(this.OnDeleteInstanceClick), false);
      this.InstanceCheckedCommand = (ICommand) new RelayCommand(new System.Action(this.OnInstanceChecked), false);
      this.InstanceUnCheckedCommand = (ICommand) new RelayCommand(new System.Action(this.OnInstanceUnChecked), false);
      this.InstanceNameLostFocusCommand = (ICommand) new RelayCommand(new System.Action(this.OnInstanceNameLostFocus), false);
      this.InstanceNameKeyDownCommand = (ICommand) new RelayCommand<KeyEventArgs>(new System.Action<KeyEventArgs>(this.OnInstanceNameKeyDown), false);
      this.InstanceNameLostKeyboardFocusCommand = (ICommand) new RelayCommand(new System.Action(this.OnInstanceNameLostKeyboardFocus), false);
      this.StopButtonClickCommand = (ICommand) new RelayCommand(new System.Action(this.OnStopButtonClick), false);
      this.StartButtonClickCommand = (ICommand) new RelayCommand(new System.Action(this.OnStartButtonClick), false);
      this.DiskCleanUpClickCommand = (ICommand) new RelayCommand(new System.Action(this.OnDiskCleanUpClick), false);
      this.CloneInstanceClickCommand = (ICommand) new RelayCommand(new System.Action(this.OnCloneInstanceClick), false);
      this.AddInstanceShortCutCommand = (ICommand) new RelayCommand(new System.Action(this.OnAddInstanceShortCut), false);
      this.SettingsClickCommand = (ICommand) new RelayCommand(new System.Action(this.OnSettingsClick), false);
      this.EditInstanceClickCommand = (ICommand) new RelayCommand<MouseButtonEventArgs>(new System.Action<MouseButtonEventArgs>(this.OnEditInstanceClick), false);
      this.CancelDownloadCommand = (ICommand) new RelayCommand(new System.Action(this.OnCancelDownload), false);
      Messenger.Default.Register<InstanceOperationMessage>((object) this, (object) vmName, (System.Action<InstanceOperationMessage>) (action => this.PerformInstanceOperation(action)), false);
    }

    public ICommand InstanceViewMouseEnterCommand { get; set; }

    public ICommand InstanceViewMouseLeaveCommand { get; set; }

    public ICommand DeleteInstanceClickCommand { get; set; }

    public ICommand InstanceCheckedCommand { get; set; }

    public ICommand InstanceUnCheckedCommand { get; set; }

    public ICommand InstanceNameLostFocusCommand { get; set; }

    public ICommand InstanceNameKeyDownCommand { get; set; }

    public ICommand InstanceNameLostKeyboardFocusCommand { get; set; }

    public ICommand StopButtonClickCommand { get; set; }

    public ICommand DiskCleanUpClickCommand { get; set; }

    public ICommand CloneInstanceClickCommand { get; set; }

    public ICommand StartButtonClickCommand { get; set; }

    public ICommand AddInstanceShortCutCommand { get; set; }

    public ICommand SettingsClickCommand { get; set; }

    public ICommand EditInstanceClickCommand { get; set; }

    public ICommand CancelDownloadCommand { get; set; }

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

    public bool EditInstanceVisibility
    {
      get
      {
        return this.mEditInstanceVisibility;
      }
      set
      {
        this.mEditInstanceVisibility = value;
        this.RaisePropertyChanged(nameof (EditInstanceVisibility));
      }
    }

    public bool IsChildInstance
    {
      get
      {
        return this.mIsChildInstance;
      }
      set
      {
        this.mIsChildInstance = value;
        this.EditInstanceImage = value ? "edit_icon" : "UnEdit";
        this.RaisePropertyChanged(nameof (IsChildInstance));
      }
    }

    public string EditInstanceImage
    {
      get
      {
        return this.mEditInstanceImage;
      }
      set
      {
        this.mEditInstanceImage = value;
        this.RaisePropertyChanged(nameof (EditInstanceImage));
      }
    }

    public string OEM { get; private set; }

    public string EngineName
    {
      get
      {
        return this.mEngineName;
      }
      set
      {
        this.mEngineName = value;
        this.RaisePropertyChanged(nameof (EngineName));
      }
    }

    public string VmNameWithSuffix
    {
      get
      {
        return this.mVmName;
      }
      set
      {
        try
        {
          if (value != null)
          {
            this.mVmName = value;
            this.OEM = InstalledOem.GetOemFromVmnameWithSuffix(this.mVmName);
            string oldValue = this.OEM.Equals("bgp", StringComparison.InvariantCultureIgnoreCase) ? "" : "_" + this.OEM;
            this.VmName = this.mVmName;
            if (!string.IsNullOrEmpty(oldValue))
              this.VmName = this.mVmName.Replace(oldValue, "");
            if (this.VmName == "Android")
              this.IsChildInstance = false;
          }
        }
        catch (Exception ex)
        {
          Logger.Error("Error in setting vmname" + ex?.ToString() + InstalledOem.CoexistingOemList.ToString());
        }
        this.RaisePropertyChanged(nameof (VmNameWithSuffix));
      }
    }

    public string InstanceName
    {
      get
      {
        return this.mInstanceName;
      }
      set
      {
        if (!(this.mInstanceName != value))
          return;
        this.mInstanceName = value;
        this.RaisePropertyChanged(nameof (InstanceName));
      }
    }

    public bool ReadOnlyInstanceName
    {
      get
      {
        return this.mReadOnlyInstanceName;
      }
      set
      {
        if (this.mReadOnlyInstanceName == value)
          return;
        this.mReadOnlyInstanceName = value;
        this.RaisePropertyChanged(nameof (ReadOnlyInstanceName));
      }
    }

    public bool InstanceCheckBoxChecked
    {
      get
      {
        return this.mInstanceCheckBoxChecked;
      }
      set
      {
        this.mInstanceCheckBoxChecked = value;
        this.RaisePropertyChanged(nameof (InstanceCheckBoxChecked));
      }
    }

    public int InstanceIndex
    {
      get
      {
        return this.mInstanceIndex;
      }
      set
      {
        this.mInstanceIndex = value;
        this.RaisePropertyChanged(nameof (InstanceIndex));
      }
    }

    public string InstanceDisplayName
    {
      get
      {
        return this.mInstanceDisplayName;
      }
      set
      {
        this.mInstanceDisplayName = value;
        this.InstanceName = value;
        this.RaisePropertyChanged(nameof (InstanceDisplayName));
        this.RaisePropertyChanged("InstanceName");
      }
    }

    public bool StopButtonVisibility
    {
      get
      {
        return this.mStopButtonVisibility;
      }
      set
      {
        this.mStopButtonVisibility = value;
        this.RaisePropertyChanged(nameof (StopButtonVisibility));
      }
    }

    public bool StartButtonVisibility
    {
      get
      {
        return this.mStartButtonVisibility;
      }
      set
      {
        this.mStartButtonVisibility = value;
        this.RaisePropertyChanged(nameof (StartButtonVisibility));
      }
    }

    public bool OptionPanelVisibility
    {
      get
      {
        return this.mOptionPanelVisibility;
      }
      set
      {
        this.mOptionPanelVisibility = value;
        this.RaisePropertyChanged(nameof (OptionPanelVisibility));
      }
    }

    public bool InstanceViewVisibility
    {
      get
      {
        return this.mInstanceViewVisibility;
      }
      set
      {
        this.mInstanceViewVisibility = value;
        this.RaisePropertyChanged(nameof (InstanceViewVisibility));
      }
    }

    public InstanceState CurrentInstanceState
    {
      get
      {
        return this.mCurrentState;
      }
      set
      {
        this.mCurrentState = value;
        this.RaisePropertyChanged(nameof (CurrentInstanceState));
      }
    }

    public string StatusText
    {
      get
      {
        return this.mStatusText;
      }
      set
      {
        this.mStatusText = value;
        this.RaisePropertyChanged(nameof (StatusText));
      }
    }

    public bool SetFocusOnInstanceName
    {
      get
      {
        return this.mSetFocusOnInstanceName;
      }
      set
      {
        this.mSetFocusOnInstanceName = value;
        this.RaisePropertyChanged(nameof (SetFocusOnInstanceName));
      }
    }

    public bool RedDotVisibility
    {
      get
      {
        return this.mRedDotVisibility;
      }
      set
      {
        this.mRedDotVisibility = value;
        this.RaisePropertyChanged(nameof (RedDotVisibility));
      }
    }

    public bool StatusTextVisibility
    {
      get
      {
        return this.mStatusTextVisibility;
      }
      set
      {
        this.mStatusTextVisibility = value;
        this.RaisePropertyChanged(nameof (StatusTextVisibility));
      }
    }

    public bool ProgressBarVisibility
    {
      get
      {
        return this.mProgressBarVisibility;
      }
      set
      {
        this.mProgressBarVisibility = value;
        this.RaisePropertyChanged(nameof (ProgressBarVisibility));
      }
    }

    public int UpdateDownloadProgress
    {
      get
      {
        return this.mUpdateDownloadProgress;
      }
      set
      {
        this.mUpdateDownloadProgress = value;
        this.RaisePropertyChanged(nameof (UpdateDownloadProgress));
        this.UpdateDownloadProgressString = this.UpdateDownloadProgress.ToString() + " %";
        this.RaisePropertyChanged("UpdateDownloadProgressString");
      }
    }

    public string UpdateDownloadProgressString { get; set; }

    public string VmName { get; private set; }

    public string Abi { get; set; }

    public void SetEngineName()
    {
      try
      {
        if (this.mAppPlayerModel == null)
          this.mAppPlayerModel = InstalledOem.GetAppPlayerModel(this.OEM, this.Abi);
        this.EngineName = this.mAppPlayerModel.AppPlayerOemDisplayName;
      }
      catch (Exception ex)
      {
        Logger.Error("Error in setting vmname" + ex?.ToString() + InstalledOem.CoexistingOemList.ToString());
      }
    }

    internal void InitProperties(string abi = "")
    {
      try
      {
        string vmIdFromVmName = Utils.GetVmIdFromVmName(this.VmName);
        string empty = string.Empty;
        this.Abi = string.IsNullOrEmpty(abi) ? Utils.GetValueInBootParams("abivalue", this.VmName, string.Empty, this.OEM) : abi;
        this.mAppPlayerModel = InstalledOem.GetAppPlayerModel(this.OEM, this.Abi);
        string suffix = this.mAppPlayerModel.Suffix;
        if (RegistryManager.RegistryManagers.ContainsKey(this.OEM) && ((IEnumerable<string>) RegistryManager.RegistryManagers[this.OEM].VmList).Contains<string>(this.VmName))
        {
          if (string.IsNullOrEmpty(RegistryManager.RegistryManagers[this.OEM].Guest[this.VmName].DisplayName) && string.IsNullOrEmpty(this.InstanceDisplayName))
          {
            if (string.Equals(this.VmName, "Android", StringComparison.InvariantCultureIgnoreCase))
              this.InstanceDisplayName = BlueStacks.Common.Strings.ProductDisplayName + " " + suffix;
            else
              this.InstanceDisplayName = BlueStacks.Common.Strings.ProductDisplayName + " " + vmIdFromVmName + " " + suffix;
            RegistryManager.RegistryManagers[this.OEM].Guest[this.VmName].DisplayName = this.InstanceDisplayName.Trim();
          }
          else if (!string.IsNullOrEmpty(this.InstanceDisplayName))
            RegistryManager.RegistryManagers[this.OEM].Guest[this.VmName].DisplayName = this.InstanceDisplayName + " " + suffix;
          else
            this.InstanceDisplayName = RegistryManager.RegistryManagers[this.OEM].Guest[this.VmName].DisplayName;
        }
        else if (string.IsNullOrEmpty(this.InstanceDisplayName))
        {
          if (string.Equals(this.VmName, "Android", StringComparison.InvariantCultureIgnoreCase))
            this.InstanceDisplayName = BlueStacks.Common.Strings.ProductDisplayName + " " + suffix;
          else
            this.InstanceDisplayName = BlueStacks.Common.Strings.ProductDisplayName + " " + vmIdFromVmName + " " + suffix;
        }
        this.InstanceCheckBoxChecked = false;
        this.InitInstanceState();
      }
      catch (Exception ex)
      {
        Logger.Error("Error in setting properties of instance" + ex?.ToString() + this.InstanceDisplayName + this.OEM + this.VmName + this.VmNameWithSuffix);
      }
    }

    internal void InitInstanceState()
    {
      if (RegistryManager.RegistryManagers.ContainsKey(this.OEM) && !((IEnumerable<string>) RegistryManager.RegistryManagers[this.OEM].VmList).Contains<string>(this.VmName))
        this.UpdateState(InstanceState.Creating, "");
      this.UpdateStateBasedOnClientRunning(BlueStacks.Common.Strings.GetClientInstanceLockName(this.VmName, this.OEM));
    }

    public Thread UpdateStateBasedOnClientRunning(string lockName)
    {
      Thread thread = new Thread((ThreadStart) (() =>
      {
        bool createdNew = false;
        Mutex mutex = (Mutex) null;
        EventWaitHandle eventWaitHandle = (EventWaitHandle) null;
        try
        {
          while (true)
          {
            Logger.Info("Checking vm state of vm: " + lockName);
            mutex = new Mutex(true, lockName, out createdNew);
            Logger.Info(" Checking Owned = " + createdNew.ToString((IFormatProvider) CultureInfo.InvariantCulture));
            if (!createdNew)
            {
              this.UpdateState(InstanceState.Running, "");
              Application.Current.Dispatcher.Invoke((Delegate) (() => Messenger.Default.Send<InstanceOperationMessage>(new InstanceOperationMessage()
              {
                Operation = Operation.AddToRunningInstance,
                VmName = this.mVmName
              })));
              try
              {
                mutex.WaitOne(-1);
              }
              catch (AbandonedMutexException ex)
              {
                Logger.Warning("Could not check if vm is running." + ex.Message);
              }
              catch (Exception ex)
              {
                Logger.Error("Could not check if vm is running." + ex.Message);
              }
              this.UpdateState(InstanceState.Stopped, "");
              Application.Current.Dispatcher.Invoke((Delegate) (() => Messenger.Default.Send<InstanceOperationMessage>(new InstanceOperationMessage()
              {
                Operation = Operation.RemoveFromInstance,
                VmName = this.mVmName
              })));
            }
            else
            {
              if (mutex != null)
              {
                mutex.ReleaseMutex();
                mutex.Close();
                mutex = (Mutex) null;
              }
              string instanceEventName = Utils.GetMultiInstanceEventName(this.VmName);
              if (eventWaitHandle == null)
                eventWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset, instanceEventName);
              else
                eventWaitHandle.Reset();
              Logger.Info("Waiting for signal from " + this.mVmName);
              eventWaitHandle.WaitOne();
            }
            if (mutex != null)
            {
              mutex.ReleaseMutex();
              mutex.Close();
              mutex = (Mutex) null;
            }
            Thread.Sleep(1000);
          }
        }
        catch (Exception ex)
        {
          Logger.Error("Exception in updating state based on the player running: " + ex.ToString());
          if (mutex == null)
            return;
          mutex.ReleaseMutex();
          mutex.Close();
        }
      }))
      {
        IsBackground = true
      };
      thread.Start();
      return thread;
    }

    private void OnInstanceViewMouseEnter()
    {
      this.RedDotVisibility = false;
      this.EditInstanceVisibility = true;
    }

    private void OnInstanceViewMouseLeave()
    {
      this.EditInstanceVisibility = false;
    }

    private void OnSettingsClick()
    {
      Messenger.Default.Send<InstanceOperationMessage>(new InstanceOperationMessage()
      {
        AppPlayerModel = this.mAppPlayerModel,
        Operation = Operation.OpenInstanceSettings,
        VmName = this.mVmName
      });
    }

    private void OnAddInstanceShortCut()
    {
      Messenger.Default.Send<InstanceOperationMessage>(new InstanceOperationMessage()
      {
        Operation = Operation.AddFromShortCut,
        VmName = this.mVmName
      });
    }

    private void OnDiskCleanUpClick()
    {
      string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "-vmname:{0}", (object) this.VmNameWithSuffix);
      Messenger.Default.Send<InstanceOperationMessage>(new InstanceOperationMessage()
      {
        Operation = Operation.DiskCleanUp,
        VmName = str
      });
    }

    private void OnCloneInstanceClick()
    {
      Messenger.Default.Send<InstanceOperationMessage>(new InstanceOperationMessage()
      {
        AppPlayerModel = this.mAppPlayerModel,
        Operation = Operation.CloneInstance,
        VmName = this.VmNameWithSuffix
      });
    }

    private void OnStartButtonClick()
    {
      Messenger.Default.Send<InstanceOperationMessage>(new InstanceOperationMessage()
      {
        Operation = Operation.RunInstance,
        VmName = this.mVmName
      });
    }

    private void OnStopButtonClick()
    {
      Messenger.Default.Send<InstanceOperationMessage>(new InstanceOperationMessage()
      {
        Operation = Operation.StopInstance,
        VmName = this.mVmName
      });
    }

    private void OnDeleteInstanceClick()
    {
      if (!this.IsChildInstance)
        return;
      Messenger.Default.Send<InstanceOperationMessage>(new InstanceOperationMessage()
      {
        Operation = Operation.DeleteInstance,
        VmName = this.mVmName
      });
    }

    private void OnEditInstanceClick(MouseButtonEventArgs e)
    {
      if (string.Equals(this.EditInstanceImage, "edit_icon", StringComparison.InvariantCulture))
      {
        e.Handled = true;
        this.mLastVmDisplayName = this.mInstanceDisplayName;
        this.EditInstanceImage = "macro_name_save";
        this.ReadOnlyInstanceName = false;
        this.SetFocusOnInstanceName = true;
      }
      else if (string.Equals(this.EditInstanceImage, "UnEdit", StringComparison.InvariantCulture))
        e.Handled = true;
      else
        this.PerformSaveInstanceNameOperations();
    }

    private void OnCancelDownload()
    {
      Messenger.Default.Send<CancelInstallerMessage>(new CancelInstallerMessage()
      {
        IsDownLoadCanceled = true
      });
    }

    private void PerformInstanceOperation(InstanceOperationMessage instanceOperation)
    {
      if (instanceOperation.Operation != Operation.VerifyInstanceName)
        return;
      this.mIsDisplayNameVerified = instanceOperation.IsNewVmDisplayNameVerified;
    }

    private void PerformSaveInstanceNameOperations()
    {
      try
      {
        this.ReadOnlyInstanceName = true;
        this.SetFocusOnInstanceName = false;
        if (this.EditInstanceImage == "macro_name_save")
        {
          this.InstanceName = string.IsNullOrEmpty(this.InstanceName.Trim()) ? this.mLastVmDisplayName : this.InstanceName.Trim();
          if (!this.InstanceName.IsValidFileName())
          {
            string str = LocaleStrings.GetLocalizedString("STRING_INSTANCE_NAME_ERROR", "") + " \n \\ / : * ? \" < > |";
            Messenger.Default.Send<ToastPopupMessage>(new ToastPopupMessage()
            {
              Message = str,
              Duration = 3.0,
              IsShowCloseImage = true
            });
            this.InstanceName = this.mLastVmDisplayName;
          }
          if (this.mLastVmDisplayName.Trim() != this.InstanceName)
          {
            Messenger.Default.Send<InstanceOperationMessage>(new InstanceOperationMessage()
            {
              Operation = Operation.VerifyInstanceName,
              VmName = this.mVmName,
              LastVMName = this.mLastVmDisplayName,
              NewVmDisplayName = this.InstanceName
            });
            if (!this.mIsDisplayNameVerified)
            {
              string str = LocaleStrings.GetLocalizedString("STRING_INSTANCE_NAME_ALREADY_EXISTS", "") ?? "";
              Messenger.Default.Send<ToastPopupMessage>(new ToastPopupMessage()
              {
                Message = str,
                Duration = 3.0,
                IsShowCloseImage = true
              });
              this.InstanceName = this.mLastVmDisplayName;
            }
          }
          RegistryManager.RegistryManagers[this.OEM].Guest[this.VmName].DisplayName = this.InstanceName;
          this.mInstanceDisplayName = this.InstanceName;
          if (!string.IsNullOrEmpty(this.InstanceName) && this.mLastVmDisplayName != this.InstanceName)
            Messenger.Default.Send<InstanceOperationMessage>(new InstanceOperationMessage()
            {
              Operation = Operation.UpdateInstanceName,
              VmName = this.mVmName,
              LastVMName = this.mLastVmDisplayName
            });
        }
        this.EditInstanceImage = "edit_icon";
      }
      catch (Exception ex)
      {
        Logger.Error("Error in naming instance : " + ex?.ToString());
      }
    }

    private void OnInstanceNameKeyDown(KeyEventArgs e)
    {
      if (e.Key != Key.Return)
        return;
      this.PerformSaveInstanceNameOperations();
    }

    private void OnInstanceNameLostKeyboardFocus()
    {
      this.PerformSaveInstanceNameOperations();
    }

    private void OnInstanceNameLostFocus()
    {
      this.PerformSaveInstanceNameOperations();
    }

    private void OnInstanceChecked()
    {
      Messenger.Default.Send<InstanceModelMessage>(new InstanceModelMessage()
      {
        VmName = this.VmNameWithSuffix,
        RemoveVM = false
      });
      this.InstanceCheckBoxChecked = true;
    }

    private void OnInstanceUnChecked()
    {
      Messenger.Default.Send<InstanceModelMessage>(new InstanceModelMessage()
      {
        VmName = this.VmNameWithSuffix,
        RemoveVM = true
      });
      this.InstanceCheckBoxChecked = false;
    }

    internal void UpdateState(InstanceState state, string percentage = "")
    {
      this.mCurrentState = state;
      if (state == InstanceState.Downloading)
      {
        this.StatusText = (this.LocaleModel.ContainsKey("STRING_DOWNLOADING") ? this.LocaleModel["STRING_DOWNLOADING"] : "Downloading") + " " + percentage;
        this.StopButtonVisibility = false;
        this.StartButtonVisibility = false;
        this.OptionPanelVisibility = false;
      }
      if (state == InstanceState.Installing)
      {
        this.StatusText = this.LocaleModel.ContainsKey("STRING_INSTALLING") ? this.LocaleModel["STRING_INSTALLING"] : "Installing";
        this.StopButtonVisibility = false;
        this.StartButtonVisibility = false;
        this.OptionPanelVisibility = false;
      }
      if (state == InstanceState.Running)
      {
        this.StatusText = this.LocaleModel.ContainsKey("STRING_RUNNING") ? this.LocaleModel["STRING_RUNNING"] : "Running";
        this.StopButtonVisibility = true;
        this.StartButtonVisibility = false;
        this.OptionPanelVisibility = false;
      }
      if (state == InstanceState.Stopped)
      {
        this.StatusText = this.LocaleModel.ContainsKey("STRING_STOPPED") ? this.LocaleModel["STRING_STOPPED"] : "Stopped";
        this.StopButtonVisibility = false;
        this.StartButtonVisibility = true;
        this.OptionPanelVisibility = true;
      }
      if (state == InstanceState.Deleting)
      {
        this.StatusText = this.LocaleModel.ContainsKey("STRING_DELETING") ? this.LocaleModel["STRING_DELETING"] : "Deleting";
        this.OptionPanelVisibility = false;
        this.StopButtonVisibility = false;
        this.StartButtonVisibility = false;
      }
      if (state == InstanceState.Stopping)
      {
        this.StatusText = this.LocaleModel.ContainsKey("STRING_STOPPING") ? this.LocaleModel["STRING_STOPPING"] : "Stopping";
        this.OptionPanelVisibility = false;
        this.StopButtonVisibility = false;
        this.StartButtonVisibility = false;
      }
      if (state != InstanceState.Creating)
        return;
      this.StatusText = this.LocaleModel.ContainsKey("STRING_CREATING") ? this.LocaleModel["STRING_CREATING"] : "Creating";
      this.OptionPanelVisibility = false;
      this.StopButtonVisibility = false;
      this.StartButtonVisibility = false;
    }
  }
}
