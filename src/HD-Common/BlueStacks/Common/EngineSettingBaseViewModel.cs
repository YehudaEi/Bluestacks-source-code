// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.EngineSettingBaseViewModel
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace BlueStacks.Common
{
  public abstract class EngineSettingBaseViewModel : ViewModelBase
  {
    private string _OEM = "bgp";
    private int _MinRam = 600;
    private int _MaxRam = 4096;
    private int _UserMachineCpuCores = Environment.ProcessorCount;
    private int _UserSupportedVCPU = 1;
    private int _MaxFPS = 60;
    private bool _IsGraphicModeEnabled = true;
    private Uri _DirectXUri = new Uri(WebHelper.GetUrlWithParams(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) WebHelper.GetServerHost(), (object) "help_articles"), (string) null, (string) null, (string) null) + "&article=bgp_kk_compat_version");
    private Visibility _CpuMemoryVisibility = Visibility.Collapsed;
    private ObservableCollection<string> _PerformanceSettingList = new ObservableCollection<string>();
    private int _CpuCores = 2;
    private ObservableCollection<int> _CpuCoresList = new ObservableCollection<int>();
    private int _Ram = 1100;
    private bool _IsRamSliderEnabled = true;
    private int _FrameRate = 60;
    private ABISetting _ABISetting = ABISetting.Auto;
    private readonly int _OneGB = 1024;
    private Dictionary<int, bool> _DictForGraphicsCompatibility = new Dictionary<int, bool>();
    private object lockObject = new object();
    private bool mIsEcoModeEnabled;
    private static int? _RamInMB;
    private Status _Status;
    private int _GlMode;
    private int _CurrentGraphicsBitPattern;
    private GraphicsMode _GraphicsMode;
    private string _WarningMessage;
    private string _ProgressMessage;
    private string _ErrorMessage;
    private bool _UseAdvancedGraphicEngine;
    private bool _UseDedicatedGPU;
    private string _PreferDedicatedGPUText;
    private string _UseDedicatedGPUText;
    private bool _IsGPUAvailable;
    private ASTCTexture _ASTCTexture;
    private ASTCOption _ASTCOption;
    private bool _EnableHardwareDecoding;
    private bool _EnableCaching;
    private bool _customPerformanceSettingVisibility;
    private string _RecommendedRamText;
    private bool _IsFrameRateEnabled;
    private bool _EnableHighFrameRates;
    private bool _EnableVSync;
    private bool _DisplayFPS;
    private bool _IsAndroidBooted;
    private bool _Is64BitABIValid;
    private bool _IsCustomABI;
    private bool _IsOpenedFromMultiInstane;
    private readonly string _VmName;
    private bool _HighEndMachine;
    private EngineSettingModel selectedCPU;
    private EngineSettingModel selectedRAM;
    private bool customRamVisibility;
    private int coreCount;
    private bool cpuCoreCustomListVisibility;
    private bool maxCoreWarningTextVisibility;
    private GraphicsMode _newMode;
    private GraphicsMode _oldMode;
    private bool _newEngine;
    private bool _oldGPU;
    private bool _newGPU;
    private CustomToastPopupControl mToastPopup;

    public string OEM
    {
      get
      {
        return this._OEM;
      }
      set
      {
        this.SetProperty<string>(ref this._OEM, value, (string) null);
      }
    }

    public int MinRam
    {
      get
      {
        return this._MinRam;
      }
      set
      {
        this.SetProperty<int>(ref this._MinRam, value, (string) null);
      }
    }

    public int MaxRam
    {
      get
      {
        return this._MaxRam;
      }
      set
      {
        this.SetProperty<int>(ref this._MaxRam, value, (string) null);
      }
    }

    public static int UserMachineRAM
    {
      get
      {
        if (!EngineSettingBaseViewModel._RamInMB.HasValue)
        {
          try
          {
            EngineSettingBaseViewModel._RamInMB = new int?((int) (new ComputerInfo().TotalPhysicalMemory / 1048576UL));
          }
          catch (Exception ex)
          {
            Logger.Error("Exception when finding ram " + ex.ToString());
          }
        }
        return EngineSettingBaseViewModel._RamInMB.GetValueOrDefault();
      }
    }

    public int UserMachineCpuCores
    {
      get
      {
        return this._UserMachineCpuCores;
      }
      set
      {
        this.SetProperty<int>(ref this._UserMachineCpuCores, value, (string) null);
      }
    }

    public int MaxFPS
    {
      get
      {
        return this._MaxFPS;
      }
      set
      {
        this.SetProperty<int>(ref this._MaxFPS, value, (string) null);
      }
    }

    public Status Status
    {
      get
      {
        return this._Status;
      }
      set
      {
        this.SetProperty<Status>(ref this._Status, value, (string) null);
      }
    }

    public bool IsGraphicModeEnabled
    {
      get
      {
        return this._IsGraphicModeEnabled;
      }
      set
      {
        this.SetProperty<bool>(ref this._IsGraphicModeEnabled, value, (string) null);
      }
    }

    public GraphicsMode GraphicsMode
    {
      get
      {
        return this._GraphicsMode;
      }
      set
      {
        if (this._GraphicsMode != value)
          this.ValidateGraphicMode(this._GraphicsMode, value);
        this.NotifyPropertyChanged(nameof (GraphicsMode));
      }
    }

    public Uri DirectXUri
    {
      get
      {
        return this._DirectXUri;
      }
      set
      {
        this.SetProperty<Uri>(ref this._DirectXUri, value, (string) null);
      }
    }

    public string WarningMessage
    {
      get
      {
        return this._WarningMessage;
      }
      set
      {
        this.SetProperty<string>(ref this._WarningMessage, value, (string) null);
      }
    }

    public string ProgressMessage
    {
      get
      {
        return this._ProgressMessage;
      }
      set
      {
        this.SetProperty<string>(ref this._ProgressMessage, value, (string) null);
      }
    }

    public string ErrorMessage
    {
      get
      {
        return this._ErrorMessage;
      }
      set
      {
        this.SetProperty<string>(ref this._ErrorMessage, value, (string) null);
      }
    }

    public bool UseAdvancedGraphicEngine
    {
      get
      {
        return this._UseAdvancedGraphicEngine;
      }
      set
      {
        if (this._UseAdvancedGraphicEngine != value)
          this.ValidateGraphicEngine(value);
        else
          this.NotifyPropertyChanged(nameof (UseAdvancedGraphicEngine));
      }
    }

    public bool UseDedicatedGPU
    {
      get
      {
        return this._UseDedicatedGPU;
      }
      set
      {
        if (this._UseDedicatedGPU != value)
          this.ValidateGPU(this._UseDedicatedGPU, value);
        else
          this.NotifyPropertyChanged(nameof (UseDedicatedGPU));
      }
    }

    public string PreferDedicatedGPUText
    {
      get
      {
        return this._PreferDedicatedGPUText;
      }
      set
      {
        this.SetProperty<string>(ref this._PreferDedicatedGPUText, value, (string) null);
      }
    }

    public string UseDedicatedGPUText
    {
      get
      {
        return this._UseDedicatedGPUText;
      }
      set
      {
        this.SetProperty<string>(ref this._UseDedicatedGPUText, value, (string) null);
      }
    }

    public bool IsGPUAvailable
    {
      get
      {
        return this._IsGPUAvailable;
      }
      set
      {
        this.SetProperty<bool>(ref this._IsGPUAvailable, value, (string) null);
      }
    }

    public ASTCTexture ASTCTexture
    {
      get
      {
        return this._ASTCTexture;
      }
      set
      {
        if (!this.SetProperty<ASTCTexture>(ref this._ASTCTexture, value, (string) null))
          return;
        this.SetASTCOption();
      }
    }

    public bool EnableHardwareDecoding
    {
      get
      {
        return this._EnableHardwareDecoding;
      }
      set
      {
        this.SetProperty<bool>(ref this._EnableHardwareDecoding, value, (string) null);
      }
    }

    public bool EnableCaching
    {
      get
      {
        return this._EnableCaching;
      }
      set
      {
        if (!this.SetProperty<bool>(ref this._EnableCaching, value, (string) null))
          return;
        this.SetASTCOption();
      }
    }

    public Visibility CpuMemoryVisibility
    {
      get
      {
        return this._CpuMemoryVisibility;
      }
      set
      {
        this.SetProperty<Visibility>(ref this._CpuMemoryVisibility, value, (string) null);
      }
    }

    public bool CustomPerformanceSettingVisibility
    {
      get
      {
        return this._customPerformanceSettingVisibility;
      }
      set
      {
        this.SetProperty<bool>(ref this._customPerformanceSettingVisibility, value, (string) null);
      }
    }

    public ObservableCollection<string> PerformanceSettingList
    {
      get
      {
        return this._PerformanceSettingList;
      }
      set
      {
        this.SetProperty<ObservableCollection<string>>(ref this._PerformanceSettingList, value, (string) null);
      }
    }

    public bool CpuCoreCountChanged { get; set; }

    public int CpuCores
    {
      get
      {
        return this._CpuCores;
      }
      set
      {
        this.SetProperty<int>(ref this._CpuCores, value, (string) null);
        this.CpuCoreCountChanged = this.EngineData.CpuCores != this.CpuCores;
        if (!this.CpuCoreCustomListVisibility)
        {
          int? coreCount = this.SelectedCPU?.CoreCount;
          int userSupportedVcpu = this._UserSupportedVCPU;
          if (coreCount.GetValueOrDefault() == userSupportedVcpu & coreCount.HasValue)
            goto label_3;
        }
        if (!this.CpuCoreCustomListVisibility || this.CpuCores != this._UserSupportedVCPU)
        {
          this.MaxCoreWarningTextVisibility = false;
          return;
        }
label_3:
        if (this.EngineData.CpuCores == 0)
          return;
        this.MaxCoreWarningTextVisibility = true;
        Stats.SendMiscellaneousStatsAsync("core_warning_viewed", RegistryManager.RegistryManagers[this.OEM].UserGuid, RegistryManager.RegistryManagers[this.OEM].ClientVersion, "Engine-Settings", (string) null, RegistryManager.Instance.Oem, (string) null, (string) null, RegistryManager.Instance.UserSelectedLocale, "Android", 0);
      }
    }

    public ObservableCollection<int> CpuCoresList
    {
      get
      {
        return this._CpuCoresList;
      }
      set
      {
        this.SetProperty<ObservableCollection<int>>(ref this._CpuCoresList, value, (string) null);
      }
    }

    public int Ram
    {
      get
      {
        return this._Ram;
      }
      set
      {
        this.SetProperty<int>(ref this._Ram, value, (string) null);
      }
    }

    public bool IsRamSliderEnabled
    {
      get
      {
        return this._IsRamSliderEnabled;
      }
      set
      {
        this.SetProperty<bool>(ref this._IsRamSliderEnabled, value, (string) null);
      }
    }

    public string RecommendedRamText
    {
      get
      {
        return this._RecommendedRamText;
      }
      set
      {
        this.SetProperty<string>(ref this._RecommendedRamText, value, (string) null);
      }
    }

    public int FrameRate
    {
      get
      {
        return this._FrameRate;
      }
      set
      {
        this.SetProperty<int>(ref this._FrameRate, value, (string) null);
      }
    }

    public bool IsFrameRateEnabled
    {
      get
      {
        return this._IsFrameRateEnabled;
      }
      set
      {
        this.SetProperty<bool>(ref this._IsFrameRateEnabled, value, (string) null);
      }
    }

    public bool EnableHighFrameRates
    {
      get
      {
        return this._EnableHighFrameRates;
      }
      set
      {
        this.SetProperty<bool>(ref this._EnableHighFrameRates, value, (string) null);
        if (this.EnableHighFrameRates)
        {
          this.MaxFPS = 240;
          if (!this.EnableVSync)
            return;
          this.EnableVSync = false;
        }
        else
        {
          this.MaxFPS = 60;
          if (this.FrameRate <= 60)
            return;
          this.FrameRate = 60;
        }
      }
    }

    public bool EnableVSync
    {
      get
      {
        return this._EnableVSync;
      }
      set
      {
        this.SetProperty<bool>(ref this._EnableVSync, value, (string) null);
        if (!this._EnableVSync || !this.EnableHighFrameRates)
          return;
        this.EnableHighFrameRates = false;
      }
    }

    public bool DisplayFPS
    {
      get
      {
        return this._DisplayFPS;
      }
      set
      {
        this.SetProperty<bool>(ref this._DisplayFPS, value, (string) null);
      }
    }

    public bool IsAndroidBooted
    {
      get
      {
        return this._IsAndroidBooted;
      }
      set
      {
        this.SetProperty<bool>(ref this._IsAndroidBooted, value, (string) null);
      }
    }

    public ABISetting ABISetting
    {
      get
      {
        return this._ABISetting;
      }
      set
      {
        this.SetProperty<ABISetting>(ref this._ABISetting, value, (string) null);
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
        this.SetProperty<bool>(ref this._Is64BitABIValid, value, (string) null);
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
        this.SetProperty<bool>(ref this._IsCustomABI, value, (string) null);
      }
    }

    public bool IsOpenedFromMultiInstane
    {
      get
      {
        return this._IsOpenedFromMultiInstane;
      }
      set
      {
        this.SetProperty<bool>(ref this._IsOpenedFromMultiInstane, value, (string) null);
      }
    }

    public ICommand SaveCommand { get; set; }

    public EngineData EngineData { get; } = new EngineData();

    public Window Owner { get; private set; }

    public EngineSettingBase ParentView { get; }

    public bool CustomRamVisibility
    {
      get
      {
        return this.customRamVisibility;
      }
      set
      {
        this.SetProperty<bool>(ref this.customRamVisibility, value, (string) null);
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
        this.SetProperty<int>(ref this.coreCount, value, (string) null);
      }
    }

    public bool CpuCoreCustomListVisibility
    {
      get
      {
        return this.cpuCoreCustomListVisibility;
      }
      set
      {
        this.SetProperty<bool>(ref this.cpuCoreCustomListVisibility, value, (string) null);
      }
    }

    public bool MaxCoreWarningTextVisibility
    {
      get
      {
        return this.maxCoreWarningTextVisibility;
      }
      set
      {
        this.SetProperty<bool>(ref this.maxCoreWarningTextVisibility, value, (string) null);
      }
    }

    public ObservableCollection<EngineSettingModel> CPUList { get; set; }

    public ObservableCollection<EngineSettingModel> RamList { get; set; }

    public EngineSettingModel SelectedCPU
    {
      get
      {
        return this.selectedCPU;
      }
      set
      {
        if (value == null)
          return;
        this.SetProperty<EngineSettingModel>(ref this.selectedCPU, value, (string) null);
        this.UpdateCustomCPUDetails();
        this.CpuCoreCountChanged = this.EngineData.CpuCores != (this.SelectedCPU.PerformanceSettingType == PerformanceSetting.Custom ? this.CpuCores : this.SelectedCPU.CoreCount);
        if (!this.CpuCoreCustomListVisibility)
        {
          int? coreCount = this.SelectedCPU?.CoreCount;
          int userSupportedVcpu = this._UserSupportedVCPU;
          if (coreCount.GetValueOrDefault() == userSupportedVcpu & coreCount.HasValue)
            goto label_4;
        }
        if (!this.CpuCoreCustomListVisibility || this.CpuCores != this._UserSupportedVCPU)
        {
          this.MaxCoreWarningTextVisibility = false;
          return;
        }
label_4:
        if (this.EngineData.CpuCores == 0)
          return;
        this.MaxCoreWarningTextVisibility = true;
        Stats.SendMiscellaneousStatsAsync("core_warning_viewed", RegistryManager.RegistryManagers[this.OEM].UserGuid, RegistryManager.RegistryManagers[this.OEM].ClientVersion, "Engine-Settings", (string) null, RegistryManager.Instance.Oem, (string) null, (string) null, RegistryManager.Instance.UserSelectedLocale, "Android", 0);
      }
    }

    public EngineSettingModel SelectedRAM
    {
      get
      {
        return this.selectedRAM;
      }
      set
      {
        if (value == null)
          return;
        this.SetProperty<EngineSettingModel>(ref this.selectedRAM, value, (string) null);
        this.UpdateCustomRamDetails();
      }
    }

    private void UpdateCustomCPUDetails()
    {
      this.CpuCoreCustomListVisibility = this.SelectedCPU.PerformanceSettingType == PerformanceSetting.Custom;
      this.CustomPerformanceSettingVisibility = this.CustomRamVisibility || this.CpuCoreCustomListVisibility;
    }

    private void UpdateCustomRamDetails()
    {
      this.CustomRamVisibility = this.SelectedRAM.PerformanceSettingType == PerformanceSetting.Custom;
      this.CustomPerformanceSettingVisibility = this.CustomRamVisibility || this.CpuCoreCustomListVisibility;
    }

    public EngineSettingBaseViewModel(
      Window owner,
      string vmName,
      EngineSettingBase parentView,
      bool isOpenedFromMultiInstane = false,
      string oem = "",
      bool isEcoModeEnabled = false)
    {
      if ((double) EngineSettingBaseViewModel.UserMachineRAM >= 7782.4 && Environment.ProcessorCount >= 8)
        this._HighEndMachine = true;
      this.OEM = string.IsNullOrEmpty(oem) ? "bgp" : oem;
      this.ParentView = parentView;
      this.Owner = owner;
      this.mIsEcoModeEnabled = isEcoModeEnabled;
      this._VmName = vmName;
      this._IsOpenedFromMultiInstane = isOpenedFromMultiInstane;
      this.SaveCommand = (ICommand) new RelayCommand2(new Func<object, bool>(this.CanSave), new System.Action<object>(this.Save));
    }

    private bool CanSave(object obj)
    {
      return this.IsDirty();
    }

    public static bool Is64BitABIValuesValid(string oem = "bgp")
    {
      return ((IEnumerable<string>) Constants.All64BitOems).Contains<string>(oem);
    }

    public void Init()
    {
      this.Status = Status.None;
      this.CustomPerformanceSettingVisibility = false;
      this.CpuCoresList.Clear();
      this.PreferDedicatedGPUText = LocaleStrings.GetLocalizedString("STRING_USE_DEDICATED_GPU", "") + " " + LocaleStrings.GetLocalizedString("STRING_NVIDIA_ONLY", "");
      this.Ram = RegistryManager.RegistryManagers[this.OEM].Guest[this._VmName].Memory;
      this._UseDedicatedGPU = RegistryManager.RegistryManagers[this.OEM].ForceDedicatedGPU;
      this._GraphicsMode = (GraphicsMode) RegistryManager.RegistryManagers[this.OEM].Guest[this._VmName].GlRenderMode;
      this._GlMode = RegistryManager.RegistryManagers[this.OEM].Guest[this._VmName].GlMode;
      if (!string.Equals(RegistryManager.RegistryManagers[this.OEM].CurrentEngine, "raw", StringComparison.InvariantCulture))
        this._UserSupportedVCPU = Environment.ProcessorCount > 8 ? 8 : Environment.ProcessorCount;
      this.CpuCoresList = new ObservableCollection<int>(Enumerable.Range(1, this._UserSupportedVCPU));
      this.CpuCores = RegistryManager.RegistryManagers[this.OEM].Guest[this._VmName].VCPUs;
      this.CpuCores = Math.Min(this.CpuCores, this._UserSupportedVCPU);
      this.SetRam();
      this.BuildCPUCombinationList();
      this.BuildRAMCombinationList();
      this.SetSelectedRAMAndCPU();
      this.SetUseAdvancedGraphicMode(Utils.GetValueInBootParams("GlMode", this._VmName, string.Empty, this.OEM) == "2");
      this.EnableHighFrameRates = (uint) RegistryManager.RegistryManagers[this.OEM].Guest[this._VmName].EnableHighFPS > 0U;
      this.EnableVSync = (uint) RegistryManager.RegistryManagers[this.OEM].Guest[this._VmName].EnableVSync > 0U;
      this.FrameRate = RegistryManager.RegistryManagers[this.OEM].Guest[this._VmName].FPS;
      this.IsFrameRateEnabled = !this.mIsEcoModeEnabled;
      this.DisplayFPS = RegistryManager.RegistryManagers[this.OEM].Guest[this._VmName].ShowFPS == 1;
      this.IsAndroidBooted = Utils.IsGuestBooted(this._VmName, "bgp");
      this.SetASTCTexture();
      if (EngineSettingBaseViewModel.Is64BitABIValuesValid(this.OEM))
        this.Is64BitABIValid = true;
      string valueInBootParams = Utils.GetValueInBootParams("abivalue", this._VmName, string.Empty, this.OEM);
      if (!string.IsNullOrEmpty(valueInBootParams))
      {
        if (EngineSettingBaseViewModel.Is64BitABIValuesValid(this.OEM))
        {
          ABISetting abiSetting;
          if (valueInBootParams != null)
          {
            if (!(valueInBootParams == "7"))
            {
              if (valueInBootParams == "15")
              {
                abiSetting = ABISetting.ARM64;
                goto label_12;
              }
            }
            else
            {
              abiSetting = ABISetting.Auto64;
              goto label_12;
            }
          }
          abiSetting = ABISetting.Custom;
label_12:
          this.ABISetting = abiSetting;
        }
        else
        {
          ABISetting abiSetting;
          if (valueInBootParams != null)
          {
            if (!(valueInBootParams == "15"))
            {
              if (valueInBootParams == "4")
              {
                abiSetting = ABISetting.ARM;
                goto label_19;
              }
            }
            else
            {
              abiSetting = ABISetting.Auto;
              goto label_19;
            }
          }
          abiSetting = ABISetting.Custom;
label_19:
          this.ABISetting = abiSetting;
        }
      }
      else if (EngineSettingBaseViewModel.Is64BitABIValuesValid(this.OEM))
        Utils.UpdateValueInBootParams("abivalue", ABISetting.Auto64.GetDescription(), this._VmName, true, this.OEM);
      else
        Utils.UpdateValueInBootParams("abivalue", ABISetting.Auto.GetDescription(), this._VmName, true, this.OEM);
      if (this.ABISetting == ABISetting.Custom)
        this.IsCustomABI = true;
      if (!string.IsNullOrEmpty(RegistryManager.RegistryManagers[this.OEM].AvailableGPUDetails))
      {
        this.IsGPUAvailable = true;
        this.UseDedicatedGPUText = LocaleStrings.GetLocalizedString("STRING_GPU_IN_USE", "") + " " + RegistryManager.RegistryManagers[this.OEM].AvailableGPUDetails;
      }
      this._CurrentGraphicsBitPattern = EngineSettingBaseViewModel.GenerateGraphicsBitPattern(this._GlMode, (int) this.GraphicsMode);
      this.NotifyPropertyChanged(string.Empty);
      this.LockForModification();
    }

    private void SetSelectedRAMAndCPU()
    {
      this.SelectedCPU = this.CpuCores != Math.Min(this._UserSupportedVCPU, 4) ? (this.CpuCores != Math.Min(this._UserSupportedVCPU, 2) ? (this.CpuCores != Math.Min(this._UserSupportedVCPU, 1) ? this.CPUList.Where<EngineSettingModel>((Func<EngineSettingModel, bool>) (c => c.PerformanceSettingType == PerformanceSetting.Custom)).FirstOrDefault<EngineSettingModel>() : this.CPUList.Where<EngineSettingModel>((Func<EngineSettingModel, bool>) (c => c.PerformanceSettingType == PerformanceSetting.Low)).FirstOrDefault<EngineSettingModel>()) : this.CPUList.Where<EngineSettingModel>((Func<EngineSettingModel, bool>) (c => c.PerformanceSettingType == PerformanceSetting.Medium)).FirstOrDefault<EngineSettingModel>()) : this.CPUList.Where<EngineSettingModel>((Func<EngineSettingModel, bool>) (c => c.PerformanceSettingType == PerformanceSetting.High)).FirstOrDefault<EngineSettingModel>();
      if (this.Ram == Math.Min(this.MaxRam, this._HighEndMachine ? 4096 : 3072))
        this.SelectedRAM = this.RamList.Where<EngineSettingModel>((Func<EngineSettingModel, bool>) (c => c.PerformanceSettingType == PerformanceSetting.High)).FirstOrDefault<EngineSettingModel>();
      else if (this.Ram == Math.Min(this.MaxRam, 2048))
        this.SelectedRAM = this.RamList.Where<EngineSettingModel>((Func<EngineSettingModel, bool>) (c => c.PerformanceSettingType == PerformanceSetting.Medium)).FirstOrDefault<EngineSettingModel>();
      else if (this.Ram == Math.Min(this.MaxRam, 1024))
        this.SelectedRAM = this.RamList.Where<EngineSettingModel>((Func<EngineSettingModel, bool>) (c => c.PerformanceSettingType == PerformanceSetting.Low)).FirstOrDefault<EngineSettingModel>();
      else
        this.SelectedRAM = this.RamList.Where<EngineSettingModel>((Func<EngineSettingModel, bool>) (c => c.PerformanceSettingType == PerformanceSetting.Custom)).FirstOrDefault<EngineSettingModel>();
    }

    private void SetASTCTexture()
    {
      this._ASTCOption = RegistryManager.RegistryManagers[this.OEM].Guest[this._VmName].ASTCOption;
      this.EnableHardwareDecoding = RegistryManager.RegistryManagers[this.OEM].Guest[this._VmName].IsHardwareAstcSupported;
      switch (this._ASTCOption)
      {
        case ASTCOption.Disabled:
          this.ASTCTexture = ASTCTexture.Disabled;
          break;
        case ASTCOption.SoftwareDecoding:
          this.ASTCTexture = ASTCTexture.Software;
          this.EnableCaching = false;
          break;
        case ASTCOption.SoftwareDecodingCache:
          this.ASTCTexture = ASTCTexture.Software;
          this.EnableCaching = true;
          break;
        case ASTCOption.HardwareDecoding:
          this.ASTCTexture = this.EnableHardwareDecoding ? ASTCTexture.Hardware : ASTCTexture.Disabled;
          break;
      }
    }

    private void SetASTCOption()
    {
      switch (this.ASTCTexture)
      {
        case ASTCTexture.Disabled:
          this._ASTCOption = ASTCOption.Disabled;
          this.EnableCaching = false;
          break;
        case ASTCTexture.Software:
          this._ASTCOption = this.EnableCaching ? ASTCOption.SoftwareDecodingCache : ASTCOption.SoftwareDecoding;
          break;
        case ASTCTexture.Hardware:
          this._ASTCOption = ASTCOption.HardwareDecoding;
          this.EnableCaching = false;
          break;
      }
    }

    private void BuildCPUCombinationList()
    {
      this.CPUList = new ObservableCollection<EngineSettingModel>();
      foreach (PerformanceSetting performanceSetting in Enum.GetValues(typeof (PerformanceSetting)))
      {
        EngineSettingModel engineSettingModel = new EngineSettingModel();
        if (performanceSetting != PerformanceSetting.Custom)
        {
          string str1 = "";
          switch (performanceSetting)
          {
            case PerformanceSetting.High:
              str1 = LocaleStrings.GetLocalizedString("STRING_HIGH", "");
              engineSettingModel.CoreCount = Math.Min(this._UserSupportedVCPU, 4);
              break;
            case PerformanceSetting.Medium:
              str1 = LocaleStrings.GetLocalizedString("STRING_MEDIUM", "");
              engineSettingModel.CoreCount = Math.Min(this._UserSupportedVCPU, 2);
              break;
            case PerformanceSetting.Low:
              str1 = LocaleStrings.GetLocalizedString("STRING_LOW", "");
              engineSettingModel.CoreCount = Math.Min(this._UserSupportedVCPU, 1);
              break;
          }
          string str2 = engineSettingModel.CoreCount == 1 ? LocaleStrings.GetLocalizedString("STRING_CORE", "") : LocaleStrings.GetLocalizedString("STRING_CORES", "");
          string str3 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} {1}", (object) engineSettingModel.CoreCount, (object) str2);
          engineSettingModel.DisplayText = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} {1}", (object) str1, (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_BRACKETS_0", ""), (object) str3));
        }
        else
        {
          engineSettingModel.DisplayText = LocaleStrings.GetLocalizedString("STRING_CUSTOM1", "");
          engineSettingModel.CoreCount = 1;
        }
        engineSettingModel.PerformanceSettingType = performanceSetting;
        this.CPUList.Add(engineSettingModel);
      }
    }

    private void BuildRAMCombinationList()
    {
      this.RamList = new ObservableCollection<EngineSettingModel>();
      foreach (PerformanceSetting performanceSetting in Enum.GetValues(typeof (PerformanceSetting)))
      {
        EngineSettingModel engineSettingModel = new EngineSettingModel();
        if (performanceSetting != PerformanceSetting.Custom)
        {
          string str1 = "";
          switch (performanceSetting)
          {
            case PerformanceSetting.High:
              str1 = LocaleStrings.GetLocalizedString("STRING_HIGH", "");
              engineSettingModel.RAM = Math.Min(this.MaxRam, this._HighEndMachine ? 4096 : 3072);
              engineSettingModel.RAMInGB = Math.Min(Convert.ToInt32(this.MaxRam / 1024), this._HighEndMachine ? 4 : 3);
              break;
            case PerformanceSetting.Medium:
              str1 = LocaleStrings.GetLocalizedString("STRING_MEDIUM", "");
              engineSettingModel.RAM = Math.Min(this.MaxRam, 2048);
              engineSettingModel.RAMInGB = Math.Min(Convert.ToInt32(this.MaxRam / 1024), 2);
              break;
            case PerformanceSetting.Low:
              str1 = LocaleStrings.GetLocalizedString("STRING_LOW", "");
              engineSettingModel.RAM = Math.Min(this.MaxRam, 1024);
              engineSettingModel.RAMInGB = Math.Min(Convert.ToInt32(this.MaxRam / 1024), 1);
              break;
          }
          string str2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} {1}", (object) engineSettingModel.RAMInGB, (object) LocaleStrings.GetLocalizedString("STRING_MEMORY_GB", ""));
          engineSettingModel.DisplayText = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} {1}", (object) str1, (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_BRACKETS_0", ""), (object) str2));
        }
        else
        {
          engineSettingModel.DisplayText = LocaleStrings.GetLocalizedString("STRING_CUSTOM1", "");
          engineSettingModel.RAM = 1024;
          engineSettingModel.RAMInGB = 1;
        }
        engineSettingModel.PerformanceSettingType = performanceSetting;
        this.RamList.Add(engineSettingModel);
      }
    }

    private void SetRam()
    {
      this._MaxRam = (int) ((double) EngineSettingBaseViewModel.UserMachineRAM * 0.75);
      if (this._MaxRam <= this._MinRam)
        this.IsRamSliderEnabled = false;
      else if (this._MaxRam >= 4096 && !Oem.Instance.IsAndroid64Bit)
        this._MaxRam = 4096;
      if (string.Equals(RegistryManager.RegistryManagers[this.OEM].CurrentEngine, "raw", StringComparison.InvariantCulture) && this._MaxRam >= 3072)
        this._MaxRam = 3072;
      this.Ram = Math.Min(this.Ram, this.MaxRam);
      this.RecommendedRamText = LocaleStrings.GetLocalizedString("STRING_REC_MEM", "") + " " + (!string.Equals(this._VmName, Strings.CurrentDefaultVmName, StringComparison.OrdinalIgnoreCase) ? (SystemUtils.IsOs64Bit() ? (EngineSettingBaseViewModel.UserMachineRAM < 4 * this._OneGB ? "800" : "1100") : "600") : (EngineSettingBaseViewModel.UserMachineRAM >= 3072 ? (!SystemUtils.IsOs64Bit() ? "900" : (EngineSettingBaseViewModel.UserMachineRAM > 4 * this._OneGB ? (EngineSettingBaseViewModel.UserMachineRAM > 5 * this._OneGB ? (EngineSettingBaseViewModel.UserMachineRAM > 6 * this._OneGB ? (EngineSettingBaseViewModel.UserMachineRAM >= 8 * this._OneGB ? (!(RegistryManager.RegistryManagers[this.OEM].CurrentEngine == "raw") ? "4096" : "3072") : "1800") : "1500") : "1200") : "900")) : "600"));
    }

    private void CreateGraphicsCompatibilityDictionary()
    {
      lock (this.lockObject)
      {
        if (this._DictForGraphicsCompatibility.Any<KeyValuePair<int, bool>>())
          return;
        string str = "";
        for (int key = 0; key < 4; ++key)
        {
          string args = (key & 1) != 0 ? str + "1" : str + "4";
          if ((key & 2) == 2)
            args += " 2";
          int exitCode = RunCommand.RunCmd(Path.Combine(RegistryStrings.InstallDir, "HD-GlCheck"), args, true, true, false, 10000).ExitCode;
          this._DictForGraphicsCompatibility.Add(key, exitCode == 0);
          str = "";
        }
      }
    }

    private static int GenerateGraphicsBitPattern(int glMode, int glRenderMode)
    {
      int num = 0;
      switch (glMode)
      {
        case 0:
          num |= 0;
          break;
        case 2:
          num |= 2;
          break;
      }
      switch (glRenderMode)
      {
        case 1:
          num |= 1;
          break;
        case 4:
          num |= 0;
          break;
      }
      return num;
    }

    private void ValidateGraphicMode(GraphicsMode oldMode_, GraphicsMode newMode_)
    {
      this._oldMode = oldMode_;
      this._newMode = newMode_;
      if (!this._DictForGraphicsCompatibility.Any<KeyValuePair<int, bool>>())
      {
        using (BackgroundWorker backgroundWorker = new BackgroundWorker())
        {
          this.ProgressMessage = string.Format((IFormatProvider) CultureInfo.CurrentCulture, LocaleStrings.GetLocalizedString("STRING_CHECKING_GRAPHICS_COMPATIBILITY", ""), (object) newMode_);
          backgroundWorker.DoWork += new DoWorkEventHandler(this.BcwWorker_DoWork);
          backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.BcwWorker_RunWorkerCompleted);
          backgroundWorker.RunWorkerAsync();
        }
      }
      else
        this.HandleChangesForGlRenderModeValueChange();
    }

    private void BcwWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      Dispatcher.CurrentDispatcher.BeginInvoke((Delegate) (() =>
      {
        this.IsGraphicModeEnabled = true;
        this.HandleChangesForGlRenderModeValueChange();
      }));
    }

    private void BcwWorker_DoWork(object sender, DoWorkEventArgs e)
    {
      this.IsGraphicModeEnabled = false;
      this.Status = Status.Progress;
      this.CreateGraphicsCompatibilityDictionary();
    }

    private void HandleChangesForGlRenderModeValueChange()
    {
      int graphicsBitPattern = EngineSettingBaseViewModel.GenerateGraphicsBitPattern(this._GlMode, (int) this._GraphicsMode);
      if (this._DictForGraphicsCompatibility.ContainsKey(graphicsBitPattern) && this._DictForGraphicsCompatibility[graphicsBitPattern])
      {
        this.SetGraphicMode(this._newMode);
      }
      else
      {
        this.ErrorMessage = string.Format((IFormatProvider) CultureInfo.CurrentCulture, LocaleStrings.GetLocalizedString("STRING_GRAPHICS_NOT_SUPPORTED_ON_MACHINE", ""), (object) this._newMode);
        this.Status = Status.Error;
        this.SetGraphicMode(this._oldMode);
      }
    }

    private void ValidateGraphicEngine(bool newEngine_)
    {
      this._newEngine = newEngine_;
      if (this._CurrentGraphicsBitPattern != EngineSettingBaseViewModel.GenerateGraphicsBitPattern(newEngine_ ? 2 : 0, (int) this.GraphicsMode))
      {
        if (!this._DictForGraphicsCompatibility.Any<KeyValuePair<int, bool>>())
        {
          using (BackgroundWorker backgroundWorker = new BackgroundWorker())
          {
            this.ProgressMessage = string.Format((IFormatProvider) CultureInfo.CurrentCulture, LocaleStrings.GetLocalizedString("STRING_CHECKING_ENGINE_COMPATIBILITY", ""), this._newEngine ? (object) LocaleStrings.GetLocalizedString("STRING_ADVANCED_MODE", "") : (object) LocaleStrings.GetLocalizedString("STRING_LEGACY_MODE", ""));
            backgroundWorker.DoWork += new DoWorkEventHandler(this.BcwForGlMode_DoWork);
            backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.BcwForGlMode_RunWorkerCompleted);
            backgroundWorker.RunWorkerAsync();
          }
        }
        else
          this.ChangesForGlMode();
      }
      else
        this.RevertToOriginalGlMode(this._CurrentGraphicsBitPattern);
    }

    private void RevertToOriginalGlMode(int mGraphicsBitPattern)
    {
      if (mGraphicsBitPattern == 0 || mGraphicsBitPattern == 1)
        this.SetUseAdvancedGraphicMode(false);
      else
        this.SetUseAdvancedGraphicMode(true);
    }

    private void BcwForGlMode_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      Dispatcher.CurrentDispatcher.BeginInvoke((Delegate) (() =>
      {
        this.IsGraphicModeEnabled = true;
        this.Status = Status.None;
        this.ChangesForGlMode();
      }));
    }

    private void ChangesForGlMode()
    {
      int graphicsBitPattern = EngineSettingBaseViewModel.GenerateGraphicsBitPattern(this._newEngine ? 2 : 0, (int) this.GraphicsMode);
      this.SetUseAdvancedGraphicMode(this.UseAdvancedGraphicEngine ? this._DictForGraphicsCompatibility[graphicsBitPattern] && this._newEngine : !this._DictForGraphicsCompatibility[graphicsBitPattern] || this._newEngine);
      this.NotifyPropertyChanged("UseAdvancedGraphicEngine");
      Logger.Info(string.Format("Setting GlMode to {0}", (object) (this.UseAdvancedGraphicEngine ? 2 : 1)));
    }

    private void BcwForGlMode_DoWork(object sender, DoWorkEventArgs e)
    {
      this.IsGraphicModeEnabled = false;
      this.Status = Status.Progress;
      this.CreateGraphicsCompatibilityDictionary();
    }

    public void SetUseAdvancedGraphicMode(bool useAdvancedGraphicMode)
    {
      this._UseAdvancedGraphicEngine = useAdvancedGraphicMode;
      this.ParentView.SetAdvancedGraphicMode(this._UseAdvancedGraphicEngine);
      this.NotifyPropertyChanged("UseAdvancedGraphicEngine");
    }

    private void ValidateGPU(bool oldGPU_, bool newGPU_)
    {
      this._oldGPU = oldGPU_;
      this._newGPU = newGPU_;
      using (BackgroundWorker backgroundWorker = new BackgroundWorker())
      {
        backgroundWorker.DoWork += new DoWorkEventHandler(this.AddDedicatedGPUProfile_DoWork);
        backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.AddDedicatedGPUProfile_RunWorkerCompleted);
        backgroundWorker.RunWorkerAsync((object) this._newGPU);
      }
    }

    private void AddDedicatedGPUProfile_RunWorkerCompleted(
      object sender,
      RunWorkerCompletedEventArgs e)
    {
      Dispatcher.CurrentDispatcher.BeginInvoke((Delegate) (() =>
      {
        this.Status = Status.None;
        this._UseDedicatedGPU = this._newGPU && (bool) e.Result;
        this.NotifyPropertyChanged("UseDedicatedGPU");
      }));
    }

    public void SetGraphicMode(GraphicsMode newMode)
    {
      this._GraphicsMode = newMode;
      this.NotifyPropertyChanged("GraphicsMode");
      this.Status = this.EngineData.GraphicsMode == this.GraphicsMode ? Status.None : Status.Warning;
      this.WarningMessage = string.Format((IFormatProvider) CultureInfo.CurrentCulture, LocaleStrings.GetLocalizedString(this.IsOpenedFromMultiInstane ? "STRING_LAUNCH_BLUESTACKS_AFTER_GRAPHICS_CHANGE" : "STRING_RESTART_BLUESTACKS_AFTER_GRAPHICS_CHANGE", ""), this.GraphicsMode == GraphicsMode.DirectX ? (object) "DirectX" : (object) "OpenGL");
    }

    private void AddDedicatedGPUProfile_DoWork(object sender, DoWorkEventArgs e)
    {
      this.Status = Status.Progress;
      bool flag = ForceDedicatedGPU.ToggleDedicatedGPU((bool) e.Argument, (string) null);
      e.Result = (object) flag;
    }

    protected virtual void Save(object param)
    {
    }

    public void NotifyPropertyChangedAllProperties()
    {
      this.NotifyPropertyChanged(string.Empty);
    }

    public void LockForModification()
    {
      int num1 = this.SelectedRAM == null || this.SelectedRAM.PerformanceSettingType == PerformanceSetting.Custom ? this.Ram : this.SelectedRAM.RAM;
      int num2 = this.SelectedCPU == null || this.SelectedCPU.PerformanceSettingType == PerformanceSetting.Custom ? this.CpuCores : this.SelectedCPU.CoreCount;
      this.EngineData.GraphicsMode = this.GraphicsMode;
      this.EngineData.UseAdvancedGraphicEngine = this.UseAdvancedGraphicEngine;
      this.EngineData.UseDedicatedGPU = this.UseDedicatedGPU;
      this.EngineData.ASTCOption = this._ASTCOption;
      this.EngineData.Ram = num1;
      this.EngineData.CpuCores = num2;
      this.EngineData.FrameRate = this.FrameRate;
      this.EngineData.EnableHighFrameRates = this.EnableHighFrameRates;
      this.EngineData.EnableVSync = this.EnableVSync;
      this.EngineData.DisplayFPS = this.DisplayFPS;
      this.EngineData.ABISetting = this.ABISetting;
    }

    public bool IsDirty()
    {
      return this.IsRestartRequired() || this.EngineData.ASTCOption != this._ASTCOption || (this.EngineData.FrameRate != this.FrameRate || this.EngineData.EnableHighFrameRates != this.EnableHighFrameRates) || this.EngineData.EnableVSync != this.EnableVSync || this.EngineData.DisplayFPS != this.DisplayFPS;
    }

    public bool IsRestartRequired()
    {
      int num1 = this.SelectedRAM == null || this.SelectedRAM.PerformanceSettingType == PerformanceSetting.Custom ? this.Ram : this.SelectedRAM.RAM;
      int num2 = this.SelectedCPU == null || this.SelectedCPU.PerformanceSettingType == PerformanceSetting.Custom ? this.CpuCores : this.SelectedCPU.CoreCount;
      return this.EngineData.GraphicsMode != this.GraphicsMode || this.EngineData.UseAdvancedGraphicEngine != this.UseAdvancedGraphicEngine || (this.EngineData.UseDedicatedGPU != this.UseDedicatedGPU || this.EngineData.Ram != num1) || this.EngineData.CpuCores != num2 || this.EngineData.ABISetting != this.ABISetting;
    }

    public void SaveEngineSettings(string abiResult = "")
    {
      if (this.EngineData.GraphicsMode != this.GraphicsMode)
        RegistryManager.RegistryManagers[this.OEM].Guest[this._VmName].GlRenderMode = (int) this.GraphicsMode;
      if (this.EngineData.UseAdvancedGraphicEngine != this.UseAdvancedGraphicEngine)
      {
        RegistryManager.RegistryManagers[this.OEM].Guest[this._VmName].GlMode = this.UseAdvancedGraphicEngine ? 2 : 1;
        Utils.UpdateValueInBootParams("GlMode", RegistryManager.RegistryManagers[this.OEM].Guest[this._VmName].GlMode.ToString((IFormatProvider) CultureInfo.InvariantCulture), this._VmName, true, this.OEM);
      }
      if (this.EngineData.UseDedicatedGPU != this.UseDedicatedGPU)
        RegistryManager.RegistryManagers[this.OEM].ForceDedicatedGPU = this.UseDedicatedGPU;
      if (this.EngineData.ASTCOption != this._ASTCOption)
      {
        Utils.SetAstcOption(this._VmName, this._ASTCOption, this.OEM);
        Stats.SendMiscellaneousStatsAsync("ASTCOption", RegistryManager.RegistryManagers[this.OEM].UserGuid, RegistryManager.RegistryManagers[this.OEM].ClientVersion, "ASTCOption", this._ASTCOption.ToString(), (string) null, (string) null, (string) null, (string) null, this._VmName, 0);
      }
      if (this.EngineData.CpuCores != (this.SelectedCPU.PerformanceSettingType == PerformanceSetting.Custom ? this.CpuCores : this.SelectedCPU.CoreCount))
        RegistryManager.RegistryManagers[this.OEM].Guest[this._VmName].VCPUs = this.SelectedCPU.PerformanceSettingType == PerformanceSetting.Custom ? this.CpuCores : this.SelectedCPU.CoreCount;
      if (this.EngineData.Ram != (this.SelectedRAM.PerformanceSettingType == PerformanceSetting.Custom ? this.Ram : this.SelectedRAM.RAM))
        RegistryManager.RegistryManagers[this.OEM].Guest[this._VmName].Memory = this.SelectedRAM.PerformanceSettingType == PerformanceSetting.Custom ? this.Ram : this.SelectedRAM.RAM;
      if (this.EngineData.EnableHighFrameRates != this.EnableHighFrameRates)
        RegistryManager.RegistryManagers[this.OEM].Guest[this._VmName].EnableHighFPS = this.EnableHighFrameRates ? 1 : 0;
      if (this.EngineData.EnableVSync != this.EnableVSync)
      {
        RegistryManager.RegistryManagers[this.OEM].Guest[this._VmName].EnableVSync = this.EnableVSync ? 1 : 0;
        Utils.SendEnableVSyncToInstanceASync(this.EnableVSync, this._VmName, "bgp");
      }
      if (this.EngineData.DisplayFPS != this.DisplayFPS)
      {
        RegistryManager.RegistryManagers[this.OEM].Guest[this._VmName].ShowFPS = this.DisplayFPS ? 1 : 0;
        Utils.SendShowFPSToInstanceASync(this._VmName, RegistryManager.RegistryManagers[this.OEM].Guest[this._VmName].ShowFPS);
        Stats.SendMiscellaneousStatsAsync("DisplayFPSCheckboxClicked", RegistryManager.RegistryManagers[this.OEM].UserGuid, RegistryManager.RegistryManagers[this.OEM].ClientVersion, "enginesettings", this.DisplayFPS ? "checked" : "unchecked", (string) null, this._VmName, (string) null, (string) null, "Android", 0);
      }
      if (this.EngineData.FrameRate != this.FrameRate)
      {
        RegistryManager.RegistryManagers[this.OEM].Guest[this._VmName].FPS = this.FrameRate;
        Utils.UpdateValueInBootParams("fps", RegistryManager.RegistryManagers[this.OEM].Guest[this._VmName].FPS.ToString((IFormatProvider) CultureInfo.InvariantCulture), this._VmName, true, this.OEM);
        Utils.SendChangeFPSToInstanceASync(this._VmName, int.MaxValue, "bgp");
      }
      if (string.Equals(abiResult, "ok", StringComparison.InvariantCulture))
      {
        Utils.UpdateValueInBootParams("abivalue", this.ABISetting.GetDescription(), this._VmName, true, this.OEM);
        Stats.SendMiscellaneousStatsAsync("ABIChanged", RegistryManager.RegistryManagers[this.OEM].UserGuid, RegistryManager.RegistryManagers[this.OEM].ClientVersion, this.ABISetting.ToString(), "bgp", (string) null, (string) null, (string) null, (string) null, "Android", 0);
      }
      if (this.CpuCoreCountChanged && this.MaxCoreWarningTextVisibility)
        Stats.SendMiscellaneousStatsAsync("core_all_assigned", RegistryManager.RegistryManagers[this.OEM].UserGuid, RegistryManager.RegistryManagers[this.OEM].ClientVersion, "Engine-Settings", (string) null, RegistryManager.Instance.Oem, (string) null, (string) null, RegistryManager.Instance.UserSelectedLocale, "Android", 0);
      this.LockForModification();
      Stats.SendMiscellaneousStatsAsync("Setting-save", RegistryManager.RegistryManagers[this.OEM].UserGuid, RegistryManager.RegistryManagers[this.OEM].ClientVersion, "Engine-Settings", "", (string) null, this._VmName, (string) null, (string) null, "Android", 0);
    }

    protected void AddToastPopup(string message)
    {
      try
      {
        if (this.mToastPopup == null)
          this.mToastPopup = new CustomToastPopupControl(this.Owner);
        this.mToastPopup.Init(this.Owner, message, (Brush) null, (Brush) null, HorizontalAlignment.Center, VerticalAlignment.Bottom, new Thickness?(), 12, new Thickness?(), (Brush) null, false, false);
        this.mToastPopup.ShowPopup(1.3);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in showing toast popup: " + ex.ToString());
      }
    }

    protected void AddToastPopupUserControl(string message)
    {
      try
      {
        if (this.mToastPopup == null)
          this.mToastPopup = new CustomToastPopupControl((UserControl) this.ParentView);
        this.mToastPopup.Init(this.Owner, message, (Brush) null, (Brush) null, HorizontalAlignment.Center, VerticalAlignment.Bottom, new Thickness?(), 12, new Thickness?(), (Brush) null, false, false);
        this.mToastPopup.ShowPopup(1.3);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in showing toast popup: " + ex.ToString());
      }
    }
  }
}
