// Decompiled with JetBrains decompiler
// Type: MultiInstanceManagerMVVM.ViewModel.Classes.Settings.OptimizationSettingsViewModel
// Assembly: HD-MultiInstanceManager, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 0C4C54E8-53C7-4A46-AE96-CC8E89A6B884
// Assembly location: C:\Program Files\BlueStacks\HD-MultiInstanceManager.exe

using BlueStacks.Common;
using BlueStacks.Core;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using MultiInstanceManagerMVVM.MessageModel.Classes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace MultiInstanceManagerMVVM.ViewModel.Classes.Settings
{
  public class OptimizationSettingsViewModel : UiViewModelBase
  {
    private bool mFPSSliderEnabled = true;
    private int mFpsValue;
    private string mStartInterval;
    private bool mIsDisableAudioCheckBoxSelected;
    private bool mIsDisableGameControlCheckBoxSelected;
    private string mInstancesPerRowCount;
    private int minimumFPSValue;
    private int maximumFPSValue;
    private bool mIsRememberWindowPositionCheckboxSelected;
    private Window _currentWindow;
    private AlignmentType mAlignment;

    public OptimizationSettingsViewModel()
    {
      this.MinimumFPSValue = 1;
      this.MaximumFPSValue = 60;
      this.FpsValue = RegistryManager.Instance.CommonFPS;
      this.FPSSliderEnabled = SimpleIoc.Default.GetInstance<MultiInstanceViewModel>().RunningInstanceList.Count == 0;
      this.IsDisableAudioCheckBoxSelected = RegistryManager.Instance.AreAllInstancesMuted;
      this.IsDisableGameControlCheckBoxSelected = !RegistryManager.Instance.IsAutoShowGuidance;
      this.IsRememberWindowPositionCheckboxSelected = RegistryManager.Instance.IsRememberWindowPositionEnabled;
      this.InstancesPerRowCount = RegistryManager.Instance.TileWindowColumnCount.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      this.StartInterval = RegistryManager.Instance.BatchInstanceStartInterval.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      this.Alignment = RegistryManager.Instance.ArrangeWindowMode == 0 ? AlignmentType.Spread : AlignmentType.Overlay;
      this.GetWindowCommand = (ICommand) new RelayCommand<Window>(new System.Action<Window>(this.OnGetWindow), false);
      this.CloseWindowCommand = (ICommand) new RelayCommand(new System.Action(this.CloseWindow), false);
      this.SaveChangesCommand = (ICommand) new RelayCommand(new System.Action(this.SaveChanges), false);
    }

    public ICommand GetWindowCommand { get; set; }

    public ICommand CloseWindowCommand { get; set; }

    public ICommand SaveChangesCommand { get; set; }

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

    public int FpsValue
    {
      get
      {
        return this.mFpsValue;
      }
      set
      {
        this.mFpsValue = value;
        this.RaisePropertyChanged(nameof (FpsValue));
      }
    }

    public bool FPSSliderEnabled
    {
      get
      {
        return this.mFPSSliderEnabled;
      }
      set
      {
        this.mFPSSliderEnabled = value;
        this.RaisePropertyChanged(nameof (FPSSliderEnabled));
      }
    }

    public string StartInterval
    {
      get
      {
        return this.mStartInterval;
      }
      set
      {
        this.mStartInterval = value;
        this.RaisePropertyChanged(nameof (StartInterval));
      }
    }

    public bool IsDisableAudioCheckBoxSelected
    {
      get
      {
        return this.mIsDisableAudioCheckBoxSelected;
      }
      set
      {
        this.mIsDisableAudioCheckBoxSelected = value;
        this.RaisePropertyChanged(nameof (IsDisableAudioCheckBoxSelected));
      }
    }

    public bool IsDisableGameControlCheckBoxSelected
    {
      get
      {
        return this.mIsDisableGameControlCheckBoxSelected;
      }
      set
      {
        this.mIsDisableGameControlCheckBoxSelected = value;
        this.RaisePropertyChanged(nameof (IsDisableGameControlCheckBoxSelected));
      }
    }

    public string InstancesPerRowCount
    {
      get
      {
        return this.mInstancesPerRowCount;
      }
      set
      {
        this.mInstancesPerRowCount = value;
        this.RaisePropertyChanged(nameof (InstancesPerRowCount));
      }
    }

    public int MinimumFPSValue
    {
      get
      {
        return this.minimumFPSValue;
      }
      set
      {
        this.minimumFPSValue = value;
        this.RaisePropertyChanged(nameof (MinimumFPSValue));
      }
    }

    public int MaximumFPSValue
    {
      get
      {
        return this.maximumFPSValue;
      }
      set
      {
        this.maximumFPSValue = value;
        this.RaisePropertyChanged(nameof (MaximumFPSValue));
      }
    }

    public bool IsRememberWindowPositionCheckboxSelected
    {
      get
      {
        return this.mIsRememberWindowPositionCheckboxSelected;
      }
      set
      {
        this.mIsRememberWindowPositionCheckboxSelected = value;
        this.RaisePropertyChanged(nameof (IsRememberWindowPositionCheckboxSelected));
      }
    }

    public AlignmentType Alignment
    {
      get
      {
        return this.mAlignment;
      }
      set
      {
        this.mAlignment = value;
        this.RaisePropertyChanged(nameof (Alignment));
      }
    }

    private void OnGetWindow(Window window)
    {
      if (window == null)
        return;
      this._currentWindow = window;
    }

    private void CloseWindow()
    {
      if (this._currentWindow != null)
        this._currentWindow.Close();
      Messenger.Default.Send<ControlVisibilityMessage>(new ControlVisibilityMessage()
      {
        IsVisible = false
      });
    }

    private void SaveChanges()
    {
      List<string> coexistingOemList = InstalledOem.InstalledCoexistingOemList;
      if (!string.IsNullOrEmpty(this.StartInterval))
      {
        foreach (string index in coexistingOemList)
          RegistryManager.RegistryManagers[index].BatchInstanceStartInterval = int.Parse(this.StartInterval, (IFormatProvider) CultureInfo.InvariantCulture);
      }
      if (this.FPSSliderEnabled)
      {
        foreach (string index in coexistingOemList)
          RegistryManager.RegistryManagers[index].CommonFPS = this.FpsValue;
        foreach (string index in coexistingOemList)
        {
          foreach (string key in RegistryManager.RegistryManagers[index].Guest.Keys)
            RegistryManager.RegistryManagers[index].Guest[key].FPS = this.FpsValue;
        }
        Messenger.Default.Send<int>(this.FpsValue);
      }
      if (RegistryManager.Instance.AreAllInstancesMuted != this.IsDisableAudioCheckBoxSelected)
      {
        foreach (string index in coexistingOemList)
          RegistryManager.RegistryManagers[index].AreAllInstancesMuted = this.IsDisableAudioCheckBoxSelected;
        Messenger.Default.Send<bool>(this.IsDisableAudioCheckBoxSelected);
      }
      foreach (string index in coexistingOemList)
        RegistryManager.RegistryManagers[index].IsAutoShowGuidance = !this.IsDisableGameControlCheckBoxSelected;
      foreach (string index in coexistingOemList)
        RegistryManager.RegistryManagers[index].IsRememberWindowPositionEnabled = this.IsRememberWindowPositionCheckboxSelected;
      foreach (string index in coexistingOemList)
        RegistryManager.RegistryManagers[index].ArrangeWindowMode = this.Alignment == AlignmentType.Spread ? 0 : 1;
      if (!string.IsNullOrEmpty(this.InstancesPerRowCount))
      {
        foreach (string index in coexistingOemList)
          RegistryManager.RegistryManagers[index].TileWindowColumnCount = long.Parse(this.InstancesPerRowCount, (IFormatProvider) CultureInfo.InvariantCulture);
      }
      this.CloseWindow();
    }
  }
}
