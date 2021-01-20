// Decompiled with JetBrains decompiler
// Type: MultiInstanceManagerMVVM.ViewModel.Classes.CreateInstance.CreateNewInstanceViewModel
// Assembly: HD-MultiInstanceManager, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 0C4C54E8-53C7-4A46-AE96-CC8E89A6B884
// Assembly location: C:\Program Files\BlueStacks\HD-MultiInstanceManager.exe

using BlueStacks.Common;
using BlueStacks.Core;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MultiInstanceManagerMVVM.MessageModel.Classes;
using MultiInstanceManagerMVVM.Model.Classes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;

namespace MultiInstanceManagerMVVM.ViewModel.Classes.CreateInstance
{
  public class CreateNewInstanceViewModel : UiViewModelBase
  {
    private int currentUserSuppeortedVCpus = 1;
    private NewInstanceType newInstanceType = NewInstanceType.Fresh;
    private string mCloneFromInstanceVmName = (string) null;
    private string mDpi = "240";
    private bool mIsAbiSetttingEnabled = true;
    private string mAbiSettingDisabledTooltip = string.Empty;
    private ABISetting mAbiValue = string.Equals(RegistryManager.Instance.Oem, "bgp64", StringComparison.InvariantCultureIgnoreCase) || string.Equals(RegistryManager.Instance.Oem, "hyperv", StringComparison.InvariantCultureIgnoreCase) ? ABISetting.Auto64 : ABISetting.Auto;
    private int mMinResolutionWidth = 540;
    private int mMinResolutionHeight = 540;
    private int mMaxResolutionWidth = 2560;
    private int mMaxResolutionHeight = 2560;
    private int mCurrentMaxRAM = 4096;
    private int mCurrentMinRAM = 600;
    private int mSystemRAM = 0;
    private int mUserCPUCores = Environment.ProcessorCount;
    private bool mIsAndroidDownloaded = true;
    private string mAppPlayerOemDisplayName = string.Empty;
    private bool _Is64BitABIValid = false;
    private bool _IsCustomABI = false;
    private int mInstanceCount;
    private CpuModel selectedCPU;
    private RamModel selectedRAM;
    private int coreCount;
    private int cpuCorePerformanceWidth;
    private int cpuCoreCustomListWidth;
    private bool cpuCoreCustomVisibility;
    private int performanceRamComboWidth;
    private int customRamTextBoxWidth;
    private bool customRamTextBoxVisibility;
    private ResolutionModel mResolutionType;
    private bool maxCoreWarningTextVisibility;

    private Dictionary<int, string> cpuCoreDict { get; set; }

    public CreateNewInstanceViewModel(
      NewInstanceType instanceType,
      AppPlayerModel appPlayerModel = null,
      string cloneFromVm = null,
      string oem = "",
      string abi = null,
      bool isAndroidDownloaded = true)
    {
      this.InstanceType = instanceType;
      this.SetAppPlayerModel(appPlayerModel);
      this.OEM = oem;
      this.IsAndroidDownloaded = isAndroidDownloaded;
      string oldValue = string.Empty;
      this.CpuCorePerformanceWidth = 320;
      this.CpuCoreCustomVisibility = false;
      this.PerformanceRamComboWidth = 320;
      this.CustomRamTextBoxVisibility = false;
      if (cloneFromVm != null)
      {
        this.OEM = InstalledOem.GetOemFromVmnameWithSuffix(cloneFromVm);
        oldValue = this.OEM.Equals("bgp", StringComparison.InvariantCultureIgnoreCase) ? "" : "_" + this.OEM;
        if (!string.IsNullOrEmpty(oldValue))
          cloneFromVm = cloneFromVm.Replace(oldValue, "");
      }
      if (string.IsNullOrEmpty(this.OEM))
        this.OEM = "bgp";
      if (this.InstanceType == NewInstanceType.Clone && !RegistryManager.RegistryManagers[this.OEM].Guest.ContainsKey(cloneFromVm))
        cloneFromVm = "Android";
      this.InitCPUInfo();
      this.InitRAMInfo();
      this.BuildCPUCombinationList();
      this.BuildRAMCombinationList();
      this.BuildResolutionsList();
      this.BuildInstanceCountList();
      this.BuildCloneFromInstanceList();
      this.ResolutionType = this.ResolutionsList.First<ResolutionModel>((Func<ResolutionModel, bool>) (x => x.OrientationType == OrientationType.Landscape));
      this.InstanceCount = 1;
      if (this.InstanceType == NewInstanceType.Clone)
        this.CloneFromInstanceVmName = cloneFromVm + oldValue;
      else
        this.InitABISettings("Android", abi);
      this.MaxResolutionWidth = Math.Max(this.MaxResolutionWidth, Screen.PrimaryScreen.Bounds.Width);
      this.MaxResolutionHeight = Math.Max(this.MaxResolutionHeight, Screen.PrimaryScreen.Bounds.Height);
      this.CloseWindowCommand = (ICommand) new RelayCommand(new System.Action(this.CloseCreateNewInstanceWindow), false);
      this.ABIHelpSupportCommand = (ICommand) new RelayCommand(new System.Action(this.ABIHelpSupport), false);
      this.CreateInstanceButtonClickCommand = (ICommand) new RelayCommand(new System.Action(this.CreateInstanceButtonClick), false);
    }

    private void SetAppPlayerModel(AppPlayerModel appPlayerModel)
    {
      this.AppPlayerOemDisplayName = this.InstanceType == NewInstanceType.Fresh ? " - " + appPlayerModel?.AppPlayerOemDisplayName : string.Empty;
      this.IsAbiSetttingEnabled = !string.Equals(appPlayerModel.AppPlayerOem, "bgp64", StringComparison.InvariantCultureIgnoreCase);
      this.AbiSettingDisabledTooltip = string.Format((IFormatProvider) CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_DISABLED_ABI_TOOLTIP", ""), (object) appPlayerModel?.AppPlayerOemDisplayName);
    }

    public ICommand CloseWindowCommand { get; set; }

    public ICommand ABIHelpSupportCommand { get; set; }

    public ICommand CreateInstanceButtonClickCommand { get; set; }

    public string AppPlayerOemDisplayName
    {
      get
      {
        return this.mAppPlayerOemDisplayName;
      }
      set
      {
        if (!(this.mAppPlayerOemDisplayName != value))
          return;
        this.mAppPlayerOemDisplayName = value;
        this.RaisePropertyChanged(nameof (AppPlayerOemDisplayName));
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

    public Dictionary<int, string> CPUCoreDict
    {
      get
      {
        return this.cpuCoreDict;
      }
      set
      {
        if (this.cpuCoreDict == value)
          return;
        this.cpuCoreDict = value;
        this.RaisePropertyChanged(nameof (CPUCoreDict));
      }
    }

    public NewInstanceType InstanceType
    {
      get
      {
        return this.newInstanceType;
      }
      set
      {
        if (this.newInstanceType == value)
          return;
        this.newInstanceType = value;
      }
    }

    public string OEM { get; private set; }

    public Dictionary<string, string> CloneFromInstanceDict { get; set; }

    public string CloneFromInstanceVmName
    {
      get
      {
        return this.mCloneFromInstanceVmName;
      }
      set
      {
        if (value == null || !(this.mCloneFromInstanceVmName != value))
          return;
        this.mCloneFromInstanceVmName = value;
        this.RaisePropertyChanged(nameof (CloneFromInstanceVmName));
        this.OEM = InstalledOem.GetOemFromVmnameWithSuffix(value);
        string oldValue = this.OEM.Equals("bgp", StringComparison.InvariantCultureIgnoreCase) ? "" : "_" + this.OEM;
        string vmName = this.CloneFromInstanceVmName;
        if (!string.IsNullOrEmpty(oldValue))
          vmName = this.mCloneFromInstanceVmName.Replace(oldValue, "");
        this.PreloadDataFromCloneInstance(vmName);
        this.SetAppPlayerModel(InstalledOem.GetAppPlayerModel(this.OEM, this.AbiValue.GetDescription()));
      }
    }

    public ObservableCollection<int> InstanceCountList { get; set; }

    public int MinResolutionWidth
    {
      get
      {
        return this.mMinResolutionWidth;
      }
      set
      {
        if (this.mMinResolutionWidth == value)
          return;
        this.mMinResolutionWidth = value;
        this.RaisePropertyChanged(nameof (MinResolutionWidth));
      }
    }

    public int MinResolutionHeight
    {
      get
      {
        return this.mMinResolutionHeight;
      }
      set
      {
        if (this.mMinResolutionHeight == value)
          return;
        this.mMinResolutionHeight = value;
        this.RaisePropertyChanged(nameof (MinResolutionHeight));
      }
    }

    public int MaxResolutionWidth
    {
      get
      {
        return this.mMaxResolutionWidth;
      }
      set
      {
        if (this.mMaxResolutionWidth == value)
          return;
        this.mMaxResolutionWidth = value;
        this.RaisePropertyChanged(nameof (MaxResolutionWidth));
      }
    }

    public int MaxResolutionHeight
    {
      get
      {
        return this.mMaxResolutionHeight;
      }
      set
      {
        if (this.mMaxResolutionHeight == value)
          return;
        this.mMaxResolutionHeight = value;
        this.RaisePropertyChanged(nameof (MaxResolutionHeight));
      }
    }

    public int CurrentMaxRAM
    {
      get
      {
        return this.mCurrentMaxRAM;
      }
      set
      {
        if (this.mCurrentMaxRAM == value)
          return;
        this.mCurrentMaxRAM = value;
        this.RaisePropertyChanged(nameof (CurrentMaxRAM));
      }
    }

    public int CurrentMinRAM
    {
      get
      {
        return this.mCurrentMinRAM;
      }
      set
      {
        if (this.mCurrentMinRAM == value)
          return;
        this.mCurrentMinRAM = value;
        this.RaisePropertyChanged(nameof (CurrentMinRAM));
      }
    }

    public int InstanceCount
    {
      get
      {
        return this.mInstanceCount;
      }
      set
      {
        if (this.mInstanceCount == value)
          return;
        this.mInstanceCount = value;
        this.RaisePropertyChanged(nameof (InstanceCount));
        if (Oem.Instance.MimAllowPerformanceResetForInstanceCreation)
          this.ResetPerformance();
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
        if (!(this.mDpi != value))
          return;
        this.mDpi = value;
        this.RaisePropertyChanged(nameof (Dpi));
      }
    }

    public ABISetting AbiValue
    {
      get
      {
        return this.mAbiValue;
      }
      set
      {
        if (this.mAbiValue == value)
          return;
        this.mAbiValue = value;
        this.RaisePropertyChanged(nameof (AbiValue));
      }
    }

    public bool IsAbiSetttingEnabled
    {
      get
      {
        return this.mIsAbiSetttingEnabled;
      }
      set
      {
        if (this.mIsAbiSetttingEnabled == value)
          return;
        this.mIsAbiSetttingEnabled = value;
        this.RaisePropertyChanged(nameof (IsAbiSetttingEnabled));
      }
    }

    public string AbiSettingDisabledTooltip
    {
      get
      {
        return this.mAbiSettingDisabledTooltip;
      }
      set
      {
        if (!(this.mAbiSettingDisabledTooltip != value))
          return;
        this.mAbiSettingDisabledTooltip = value;
        this.RaisePropertyChanged(nameof (AbiSettingDisabledTooltip));
      }
    }

    public bool IsAndroidDownloaded
    {
      get
      {
        return this.mIsAndroidDownloaded;
      }
      set
      {
        if (this.mIsAndroidDownloaded == value)
          return;
        this.mIsAndroidDownloaded = value;
        this.RaisePropertyChanged(nameof (IsAndroidDownloaded));
      }
    }

    public bool Is64BitABIValid
    {
      get
      {
        return this._Is64BitABIValid;
      }
      set
      {
        if (this._Is64BitABIValid == value)
          return;
        this._Is64BitABIValid = value;
        this.RaisePropertyChanged(nameof (Is64BitABIValid));
      }
    }

    public bool IsCustomABI
    {
      get
      {
        return this._IsCustomABI;
      }
      set
      {
        if (this._IsCustomABI == value)
          return;
        this._IsCustomABI = value;
        this.RaisePropertyChanged(nameof (IsCustomABI));
      }
    }

    public ObservableCollection<CpuModel> CPUList { get; set; }

    public ObservableCollection<RamModel> RamList { get; set; }

    public bool MaxCoreWarningTextVisibility
    {
      get
      {
        return this.maxCoreWarningTextVisibility;
      }
      set
      {
        if (this.maxCoreWarningTextVisibility == value)
          return;
        this.maxCoreWarningTextVisibility = value;
        this.RaisePropertyChanged(nameof (MaxCoreWarningTextVisibility));
      }
    }

    public CpuModel SelectedCPU
    {
      get
      {
        return this.selectedCPU;
      }
      set
      {
        if (this.selectedCPU == value)
          return;
        this.selectedCPU = value;
        this.RaisePropertyChanged(nameof (SelectedCPU));
        this.UpdateCustomCPUDetails();
        this.SetPerformanceWarningVisibility();
      }
    }

    private void UpdateCustomCPUDetails()
    {
      if (this.SelectedCPU.PerformanceSettingType == PerformanceSetting.Custom)
      {
        this.CpuCorePerformanceWidth = 154;
        this.CpuCoreCustomListWidth = 150;
        this.CpuCoreCustomVisibility = true;
      }
      else
      {
        this.CpuCorePerformanceWidth = 320;
        this.CpuCoreCustomVisibility = false;
      }
    }

    public RamModel SelectedRAM
    {
      get
      {
        return this.selectedRAM;
      }
      set
      {
        if (this.selectedRAM == value)
          return;
        this.selectedRAM = value;
        this.RaisePropertyChanged(nameof (SelectedRAM));
        this.UpdateCustomRamDetails();
      }
    }

    private void UpdateCustomRamDetails()
    {
      if (this.SelectedRAM.PerformanceSettingType == PerformanceSetting.Custom)
      {
        this.PerformanceRamComboWidth = 154;
        this.CustomRamTextBoxWidth = 150;
        this.CustomRamTextBoxVisibility = true;
      }
      else
      {
        this.PerformanceRamComboWidth = 320;
        this.CustomRamTextBoxVisibility = false;
      }
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
        this.RaisePropertyChanged(nameof (ResolutionType));
      }
    }

    public int CpuCorePerformanceWidth
    {
      get
      {
        return this.cpuCorePerformanceWidth;
      }
      set
      {
        if (value == this.cpuCorePerformanceWidth)
          return;
        this.cpuCorePerformanceWidth = value;
        this.RaisePropertyChanged(nameof (CpuCorePerformanceWidth));
      }
    }

    public int CpuCoreCustomListWidth
    {
      get
      {
        return this.cpuCoreCustomListWidth;
      }
      set
      {
        if (value == this.cpuCoreCustomListWidth)
          return;
        this.cpuCoreCustomListWidth = value;
        this.RaisePropertyChanged(nameof (CpuCoreCustomListWidth));
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
        if (value == this.coreCount)
          return;
        this.coreCount = value;
        this.RaisePropertyChanged(nameof (CoreCount));
        this.SetPerformanceWarningVisibility();
      }
    }

    public bool CpuCoreCustomVisibility
    {
      get
      {
        return this.cpuCoreCustomVisibility;
      }
      set
      {
        if (value == this.cpuCoreCustomVisibility)
          return;
        this.cpuCoreCustomVisibility = value;
        this.RaisePropertyChanged(nameof (CpuCoreCustomVisibility));
      }
    }

    public int PerformanceRamComboWidth
    {
      get
      {
        return this.performanceRamComboWidth;
      }
      set
      {
        if (value == this.performanceRamComboWidth)
          return;
        this.performanceRamComboWidth = value;
        this.RaisePropertyChanged(nameof (PerformanceRamComboWidth));
      }
    }

    public int CustomRamTextBoxWidth
    {
      get
      {
        return this.customRamTextBoxWidth;
      }
      set
      {
        if (value == this.customRamTextBoxWidth)
          return;
        this.customRamTextBoxWidth = value;
        this.RaisePropertyChanged(nameof (CustomRamTextBoxWidth));
      }
    }

    public bool CustomRamTextBoxVisibility
    {
      get
      {
        return this.customRamTextBoxVisibility;
      }
      set
      {
        if (value == this.customRamTextBoxVisibility)
          return;
        this.customRamTextBoxVisibility = value;
        this.RaisePropertyChanged(nameof (CustomRamTextBoxVisibility));
      }
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

    private void CloseCreateNewInstanceWindow()
    {
      this.CloseCurrentWindow();
      Messenger.Default.Send<CancelInstallerMessage>(new CancelInstallerMessage()
      {
        IsCancelledBeforeDownload = true
      });
    }

    private void InitCPUInfo()
    {
      if (RegistryManager.Instance.CurrentEngine != "raw")
        this.currentUserSuppeortedVCpus = Environment.ProcessorCount > 8 ? 8 : Environment.ProcessorCount;
      this.BuildCPUCoresList();
    }

    private void BuildCPUCoresList()
    {
      this.CPUCoreDict = new Dictionary<int, string>();
      for (int key = 1; key <= this.currentUserSuppeortedVCpus; ++key)
        this.CPUCoreDict.Add(key, key.ToString() + " Cores");
      this.CoreCount = this.CPUCoreDict.Keys.FirstOrDefault<int>();
    }

    private void InitRAMInfo()
    {
      try
      {
        this.mSystemRAM = (int) (SystemUtils.GetSystemTotalPhysicalMemory() / 1048576UL);
        int val2_1 = Math.Max((int) ((double) this.mSystemRAM * 0.75), this.CurrentMinRAM);
        int val2_2 = !Oem.Instance.IsAndroid64Bit ? Math.Min(4096, val2_1) : val2_1;
        if (RegistryManager.RegistryManagers[this.OEM].CurrentEngine == "raw")
          val2_2 = Math.Min(3072, val2_2);
        this.CurrentMaxRAM = val2_2;
      }
      catch (Exception ex)
      {
        Logger.Error("Failed to set RAM range, {0}", (object) ex.Message);
      }
    }

    private void BuildCPUCombinationList()
    {
      this.CPUList = new ObservableCollection<CpuModel>();
      foreach (PerformanceSetting performanceSetting in Enum.GetValues(typeof (PerformanceSetting)))
      {
        CpuModel cpuModel = new CpuModel();
        if (performanceSetting != PerformanceSetting.Custom)
        {
          string str1 = "";
          switch (performanceSetting)
          {
            case PerformanceSetting.High:
              str1 = LocaleStrings.GetLocalizedString("STRING_HIGH", "");
              cpuModel.CoreCount = Math.Min(this.currentUserSuppeortedVCpus, 4);
              break;
            case PerformanceSetting.Medium:
              str1 = LocaleStrings.GetLocalizedString("STRING_MEDIUM", "");
              cpuModel.CoreCount = Math.Min(this.currentUserSuppeortedVCpus, 2);
              break;
            case PerformanceSetting.Low:
              str1 = LocaleStrings.GetLocalizedString("STRING_LOW", "");
              cpuModel.CoreCount = Math.Min(this.currentUserSuppeortedVCpus, 1);
              break;
          }
          string str2 = cpuModel.CoreCount == 1 ? LocaleStrings.GetLocalizedString("STRING_CPU_CORE", "") : LocaleStrings.GetLocalizedString("STRING_CPU_CORES", "");
          string str3 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} {1}", (object) cpuModel.CoreCount, (object) str2);
          cpuModel.CpuDisplayText = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} {1}", (object) str1, (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_BRACKETS_0", ""), (object) str3));
        }
        else
        {
          cpuModel.CpuDisplayText = LocaleStrings.GetLocalizedString("STRING_CUSTOM1", "");
          cpuModel.CoreCount = 1;
        }
        cpuModel.PerformanceSettingType = performanceSetting;
        this.CPUList.Add(cpuModel);
      }
    }

    private void BuildRAMCombinationList()
    {
      this.RamList = new ObservableCollection<RamModel>();
      foreach (PerformanceSetting performanceSetting in Enum.GetValues(typeof (PerformanceSetting)))
      {
        RamModel ramModel = new RamModel();
        if (performanceSetting != PerformanceSetting.Custom)
        {
          string str1 = "";
          switch (performanceSetting)
          {
            case PerformanceSetting.High:
              bool flag = false;
              if ((double) this.mSystemRAM >= 7782.4 && this.mUserCPUCores >= 8)
                flag = true;
              str1 = LocaleStrings.GetLocalizedString("STRING_HIGH", "");
              ramModel.RAM = Math.Min(this.CurrentMaxRAM, flag ? 4096 : 3072);
              ramModel.RAMInGB = Math.Min(Convert.ToInt32(this.CurrentMaxRAM / 1024), flag ? 4 : 3);
              break;
            case PerformanceSetting.Medium:
              str1 = LocaleStrings.GetLocalizedString("STRING_MEDIUM", "");
              ramModel.RAM = Math.Min(this.CurrentMaxRAM, 2048);
              ramModel.RAMInGB = Math.Min(Convert.ToInt32(this.CurrentMaxRAM / 1024), 2);
              break;
            case PerformanceSetting.Low:
              str1 = LocaleStrings.GetLocalizedString("STRING_LOW", "");
              ramModel.RAM = Math.Min(this.CurrentMaxRAM, 1024);
              ramModel.RAMInGB = Math.Min(Convert.ToInt32(this.CurrentMaxRAM / 1024), 1);
              break;
          }
          string str2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} {1}", (object) ramModel.RAMInGB, (object) LocaleStrings.GetLocalizedString("STRING_MEMORY_GB", ""));
          ramModel.RamDisplayText = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} {1}", (object) str1, (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_BRACKETS_0", ""), (object) str2));
        }
        else
        {
          ramModel.RamDisplayText = LocaleStrings.GetLocalizedString("STRING_CUSTOM_MB", "");
          ramModel.RAM = 1024;
          ramModel.RAMInGB = 1;
        }
        ramModel.PerformanceSettingType = performanceSetting;
        this.RamList.Add(ramModel);
      }
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
        OrientationName = LocaleStrings.GetLocalizedString("STRING_LANDSCAPE", ""),
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
        OrientationName = LocaleStrings.GetLocalizedString("STRING_PORTRAIT", ""),
        AvailableResolutionsDict = new Dictionary<string, string>()
        {
          {
            "540 x 960",
            "540 x 960"
          },
          {
            "720 x 1280",
            "720 x 1280"
          },
          {
            "900 x 1600",
            "900 x 1600"
          },
          {
            "1080 x 1920",
            "1080 x 1920"
          },
          {
            "1440 x 2560",
            "1440 x 2560"
          }
        },
        CombinedResolution = string.Format("{0} x {1}", (object) height, (object) width),
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

    private void InitABISettings(string vmName = "Android", string abi = null)
    {
      if (this.Is64BitABIValuesValid())
        this.Is64BitABIValid = true;
      string a = abi;
      if (abi == null)
        a = RegistryManager.RegistryManagers[this.OEM].CloudABIValue.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      if (string.Equals(a, "0", StringComparison.InvariantCultureIgnoreCase))
        a = this.Is64BitABIValuesValid() ? ABISetting.Auto64.GetDescription() : ABISetting.Auto.GetDescription();
      if (!string.IsNullOrEmpty(a))
      {
        if (this.Is64BitABIValuesValid())
        {
          switch (a)
          {
            case "7":
              this.AbiValue = ABISetting.Auto64;
              break;
            case "15":
              this.AbiValue = ABISetting.ARM64;
              break;
            default:
              this.AbiValue = ABISetting.Custom;
              break;
          }
        }
        else
        {
          switch (a)
          {
            case "15":
              this.AbiValue = ABISetting.Auto;
              break;
            case "4":
              this.AbiValue = ABISetting.ARM;
              break;
            default:
              this.AbiValue = ABISetting.Custom;
              break;
          }
        }
      }
      else if (this.Is64BitABIValuesValid())
        Utils.UpdateValueInBootParams("abivalue", ABISetting.Auto64.GetDescription(), vmName, true, this.OEM);
      else
        Utils.UpdateValueInBootParams("abivalue", ABISetting.Auto.GetDescription(), vmName, true, this.OEM);
      if (this.AbiValue != ABISetting.Custom)
        return;
      this.IsCustomABI = true;
    }

    private void BuildInstanceCountList()
    {
      this.InstanceCountList = new ObservableCollection<int>(Enumerable.Range(1, Oem.Instance.MaxBatchInstanceCreationCount));
    }

    private void BuildCloneFromInstanceList()
    {
      if (this.InstanceType == NewInstanceType.Fresh)
        return;
      this.CloneFromInstanceDict = new Dictionary<string, string>();
      foreach (string installedCoexistingOem in InstalledOem.InstalledCoexistingOemList)
      {
        string str = installedCoexistingOem.Equals("bgp", StringComparison.InvariantCultureIgnoreCase) ? "" : "_" + installedCoexistingOem;
        foreach (string vm in RegistryManager.RegistryManagers[installedCoexistingOem].VmList)
        {
          if (RegistryManager.RegistryManagers[installedCoexistingOem].Guest.ContainsKey(vm))
            this.CloneFromInstanceDict.Add(vm + str, RegistryManager.RegistryManagers[installedCoexistingOem].Guest[vm].DisplayName);
        }
      }
    }

    private void ABIHelpSupport()
    {
      Utils.OpenUrl(WebHelper.GetUrlWithParams(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) WebHelper.GetServerHost(), (object) "help_articles"), (string) null, (string) null, (string) null) + "&article=ABI_Help");
    }

    private bool Is64BitABIValuesValid()
    {
      return ((IEnumerable<string>) Constants.All64BitOems).Contains<string>(this.OEM);
    }

    private void PreloadDataFromCloneInstance(string vmName)
    {
      this.Dpi = Utils.GetDpiFromBootParameters(RegistryManager.RegistryManagers[this.OEM].Guest[vmName].BootParameters);
      if (this.Is64BitABIValuesValid())
        this.Is64BitABIValid = true;
      string valueInBootParams = Utils.GetValueInBootParams("abivalue", vmName, string.Empty, this.OEM);
      if (!string.IsNullOrEmpty(valueInBootParams))
      {
        if (this.Is64BitABIValuesValid())
        {
          switch (valueInBootParams)
          {
            case "7":
              this.AbiValue = ABISetting.Auto64;
              break;
            case "15":
              this.AbiValue = ABISetting.ARM64;
              break;
            default:
              this.AbiValue = ABISetting.Custom;
              break;
          }
        }
        else
        {
          switch (valueInBootParams)
          {
            case "15":
              this.AbiValue = ABISetting.Auto;
              break;
            case "4":
              this.AbiValue = ABISetting.ARM;
              break;
            default:
              this.AbiValue = ABISetting.Custom;
              break;
          }
        }
      }
      else if (this.Is64BitABIValuesValid())
        Utils.UpdateValueInBootParams("abivalue", ABISetting.Auto64.GetDescription(), vmName, true, this.OEM);
      else
        Utils.UpdateValueInBootParams("abivalue", ABISetting.Auto.GetDescription(), vmName, true, this.OEM);
      if (this.AbiValue == ABISetting.Custom)
        this.IsCustomABI = true;
      int currentCpucount = RegistryManager.RegistryManagers[this.OEM].CurrentEngine == "raw" ? 1 : RegistryManager.RegistryManagers[this.OEM].Guest[vmName].VCPUs;
      CpuModel cpuModel = this.CPUList.FirstOrDefault<CpuModel>((Func<CpuModel, bool>) (x => x.CoreCount == currentCpucount));
      if (cpuModel == null)
      {
        cpuModel = this.CPUList.First<CpuModel>((Func<CpuModel, bool>) (x => x.PerformanceSettingType == PerformanceSetting.Custom));
        cpuModel.CoreCount = currentCpucount;
        this.CoreCount = currentCpucount;
      }
      this.SelectedCPU = cpuModel;
      RamModel ramModel = this.RamList.FirstOrDefault<RamModel>((Func<RamModel, bool>) (x => x.RAM == RegistryManager.RegistryManagers[this.OEM].Guest[vmName].Memory));
      if (ramModel == null)
      {
        ramModel = this.RamList.First<RamModel>((Func<RamModel, bool>) (x => x.PerformanceSettingType == PerformanceSetting.Custom));
        ramModel.RAM = RegistryManager.RegistryManagers[this.OEM].Guest[vmName].Memory;
      }
      this.SelectedRAM = ramModel;
      ResolutionModel resolutionModel = this.ResolutionsList.FirstOrDefault<ResolutionModel>((Func<ResolutionModel, bool>) (x => x.AvailableResolutionsDict.Keys.Contains<string>(string.Format("{0} x {1}", (object) RegistryManager.RegistryManagers[this.OEM].Guest[vmName].GuestWidth, (object) RegistryManager.RegistryManagers[this.OEM].Guest[vmName].GuestHeight)))) ?? this.ResolutionsList.First<ResolutionModel>((Func<ResolutionModel, bool>) (x => x.OrientationType == OrientationType.Custom));
      resolutionModel.CombinedResolution = string.Format("{0} x {1}", (object) RegistryManager.RegistryManagers[this.OEM].Guest[vmName].GuestWidth, (object) RegistryManager.RegistryManagers[this.OEM].Guest[vmName].GuestHeight);
      this.ResolutionType = resolutionModel;
    }

    private void CreateInstanceButtonClick()
    {
      this.CloseCurrentWindow();
      string vmName = this.GetVmName();
      Messenger.Default.Send<NewInstanceMessage>(new NewInstanceMessage()
      {
        InstanceType = this.InstanceType,
        NewInstanceSettings = this.GetNewInstanceSettings(),
        CloneFromVmName = vmName,
        InstanceCount = this.InstanceCount,
        Oem = this.OEM,
        MimStatsModel = this.GetStatsData()
      });
      if (!this.MaxCoreWarningTextVisibility || this.currentUserSuppeortedVCpus != (this.SelectedCPU.PerformanceSettingType == PerformanceSetting.Custom ? this.CoreCount : this.SelectedCPU.CoreCount))
        return;
      Stats.SendMiscellaneousStatsAsync("core_all_assigned", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "Engine-Settings", (string) null, RegistryManager.Instance.Oem, (string) null, (string) null, RegistryManager.Instance.UserSelectedLocale, "Android", 0);
    }

    private string GetVmName()
    {
      string oldValue = this.OEM.Equals("bgp", StringComparison.InvariantCultureIgnoreCase) ? "" : "_" + this.OEM;
      string str = this.CloneFromInstanceVmName;
      if (!string.IsNullOrEmpty(oldValue) && !string.IsNullOrEmpty(this.mCloneFromInstanceVmName))
        str = this.mCloneFromInstanceVmName.Replace(oldValue, "");
      return str;
    }

    private MimStatsModel GetStatsData()
    {
      string cpuPerformance;
      string ramPerformance;
      this.GetRamAndCPUValueForStats(out cpuPerformance, out ramPerformance);
      return new MimStatsModel()
      {
        Performance = cpuPerformance + " and " + ramPerformance,
        Resolution = this.ResolutionType.OrientationType.ToString() + " " + this.ResolutionType.CombinedResolution,
        Abi = this.GetABIValue(),
        Dpi = this.Dpi,
        InstanceCount = this.InstanceCount,
        Oem = this.OEM,
        Arg1 = this.GetVmName(),
        Arg2 = "",
        InstanceType = this.InstanceType
      };
    }

    private void GetRamAndCPUValueForStats(out string cpuPerformance, out string ramPerformance)
    {
      if (this.SelectedCPU.PerformanceSettingType == PerformanceSetting.Custom)
      {
        string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} {1}", (object) this.CoreCount, (object) (this.CoreCount == 1 ? LocaleStrings.GetLocalizedString("STRING_CPU_CORE", "") : LocaleStrings.GetLocalizedString("STRING_CPU_CORES", "")));
        cpuPerformance = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} {1}", (object) PerformanceSetting.Custom.ToString(), (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_BRACKETS_0", ""), (object) str));
      }
      else
        cpuPerformance = this.SelectedCPU.CpuDisplayText;
      if (this.SelectedRAM.PerformanceSettingType == PerformanceSetting.Custom)
      {
        string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} {1}", (object) this.SelectedRAM.RAM, (object) "MB");
        ramPerformance = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} {1}", (object) PerformanceSetting.Custom.ToString(), (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_BRACKETS_0", ""), (object) str));
      }
      else
        ramPerformance = this.SelectedRAM.RamDisplayText;
    }

    public string GetNewInstanceSettings()
    {
      int abiValue = this.GetABIValue();
      return new JObject()
      {
        {
          "cpu",
          (JToken) (this.SelectedCPU.PerformanceSettingType == PerformanceSetting.Custom ? this.CoreCount : this.selectedCPU.CoreCount)
        },
        {
          "ram",
          (JToken) this.SelectedRAM.RAM
        },
        {
          "dpi",
          (JToken) this.Dpi
        },
        {
          "abi",
          (JToken) abiValue
        },
        {
          "resolutionwidth",
          (JToken) this.ResolutionType.ResolutionWidth
        },
        {
          "resolutionheight",
          (JToken) this.ResolutionType.ResolutionHeight
        }
      }.ToString(Formatting.None);
    }

    private int GetABIValue()
    {
      int cloudAbiValue;
      if (this.AbiValue == ABISetting.Custom)
      {
        string oldValue = this.OEM.Equals("bgp", StringComparison.InvariantCultureIgnoreCase) ? "" : "_" + this.OEM;
        string vmName = this.CloneFromInstanceVmName;
        if (!string.IsNullOrEmpty(oldValue) && !string.IsNullOrEmpty(this.mCloneFromInstanceVmName))
          vmName = this.mCloneFromInstanceVmName.Replace(oldValue, "");
        if (vmName == null)
        {
          cloudAbiValue = RegistryManager.RegistryManagers[this.OEM].CloudABIValue;
          if (cloudAbiValue == 0)
            cloudAbiValue = int.Parse(this.Is64BitABIValuesValid() ? ABISetting.Auto64.GetDescription() : ABISetting.Auto.GetDescription(), (IFormatProvider) CultureInfo.InvariantCulture);
        }
        else
          cloudAbiValue = int.Parse(Utils.GetValueInBootParams("abivalue", vmName, string.Empty, this.OEM), (IFormatProvider) CultureInfo.InvariantCulture);
      }
      else
        cloudAbiValue = int.Parse(this.AbiValue.GetDescription(), (IFormatProvider) CultureInfo.InvariantCulture);
      return cloudAbiValue;
    }

    private void SetPerformanceWarningVisibility()
    {
      int num;
      if (!this.CpuCoreCustomVisibility)
      {
        int? coreCount = this.SelectedCPU?.CoreCount;
        int userSuppeortedVcpus = this.currentUserSuppeortedVCpus;
        if (coreCount.GetValueOrDefault() == userSuppeortedVcpus & coreCount.HasValue)
        {
          num = 1;
          goto label_4;
        }
      }
      num = !this.CpuCoreCustomVisibility ? 0 : (this.CoreCount == this.currentUserSuppeortedVCpus ? 1 : 0);
label_4:
      if (num != 0)
      {
        this.MaxCoreWarningTextVisibility = true;
        Stats.SendMiscellaneousStatsAsync("core_warning_viewed", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "Engine-Settings", (string) null, RegistryManager.Instance.Oem, (string) null, (string) null, RegistryManager.Instance.UserSelectedLocale, "Android", 0);
      }
      else
        this.MaxCoreWarningTextVisibility = false;
    }

    private void ResetPerformance()
    {
      if (this.InstanceCount <= Oem.Instance.MimHighPerformanceInstanceMaxCount)
      {
        this.SelectedCPU = this.CPUList.Where<CpuModel>((Func<CpuModel, bool>) (cpu => cpu.PerformanceSettingType == PerformanceSetting.High)).FirstOrDefault<CpuModel>();
        this.SelectedRAM = this.RamList.Where<RamModel>((Func<RamModel, bool>) (ram => ram.PerformanceSettingType == PerformanceSetting.High)).FirstOrDefault<RamModel>();
      }
      else if (this.InstanceCount <= Oem.Instance.MimMediumPerformanceInstanceMaxCount)
      {
        this.SelectedCPU = this.CPUList.Where<CpuModel>((Func<CpuModel, bool>) (cpu => cpu.PerformanceSettingType == PerformanceSetting.Medium)).FirstOrDefault<CpuModel>();
        this.SelectedRAM = this.RamList.Where<RamModel>((Func<RamModel, bool>) (ram => ram.PerformanceSettingType == PerformanceSetting.Medium)).FirstOrDefault<RamModel>();
      }
      else
      {
        this.SelectedCPU = this.CPUList.Where<CpuModel>((Func<CpuModel, bool>) (cpu => cpu.PerformanceSettingType == PerformanceSetting.Low)).FirstOrDefault<CpuModel>();
        this.SelectedRAM = this.RamList.Where<RamModel>((Func<RamModel, bool>) (ram => ram.PerformanceSettingType == PerformanceSetting.Low)).FirstOrDefault<RamModel>();
      }
    }
  }
}
