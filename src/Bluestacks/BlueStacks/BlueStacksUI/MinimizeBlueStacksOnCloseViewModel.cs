// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.MinimizeBlueStacksOnCloseViewModel
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace BlueStacks.BlueStacksUI
{
  public class MinimizeBlueStacksOnCloseViewModel : INotifyPropertyChanged
  {
    private bool mIsMinimizeBlueStacksRadioBtnChecked = true;
    private bool mIsDoNotShowAgainChkBoxChecked;
    private bool mIsQuitBluestacksChecked;

    public event PropertyChangedEventHandler PropertyChanged;

    public MainWindow ParentWindow { get; set; }

    public bool IsDoNotShowAgainChkBoxChecked
    {
      get
      {
        return this.mIsDoNotShowAgainChkBoxChecked;
      }
      set
      {
        this.mIsDoNotShowAgainChkBoxChecked = value;
        this.NotifyPropertyChanged(nameof (IsDoNotShowAgainChkBoxChecked));
      }
    }

    public bool IsQuitBluestacksChecked
    {
      get
      {
        return this.mIsQuitBluestacksChecked;
      }
      set
      {
        this.mIsQuitBluestacksChecked = value;
        this.NotifyPropertyChanged(nameof (IsQuitBluestacksChecked));
      }
    }

    public bool IsMinimizeBlueStacksRadioBtnChecked
    {
      get
      {
        return this.mIsMinimizeBlueStacksRadioBtnChecked;
      }
      set
      {
        this.mIsMinimizeBlueStacksRadioBtnChecked = value;
        this.NotifyPropertyChanged(nameof (IsMinimizeBlueStacksRadioBtnChecked));
      }
    }

    public static Dictionary<string, string> LocaleModel
    {
      get
      {
        return BlueStacksUIBinding.Instance.LocaleModel;
      }
    }

    public static Dictionary<string, Brush> ColorModel
    {
      get
      {
        return BlueStacksUIBinding.Instance.ColorModel;
      }
    }

    public ICommand CloseControlCommand { get; set; }

    public ICommand MinimizeCommand { get; set; }

    public ICommand QuitCommand { get; set; }

    private void NotifyPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }

    public MinimizeBlueStacksOnCloseViewModel(MainWindow window)
    {
      this.ParentWindow = window;
      this.Init();
    }

    private void Init()
    {
      this.CloseControlCommand = (ICommand) new RelayCommand<UserControl>(new System.Action<UserControl>(this.Close), false);
      this.MinimizeCommand = (ICommand) new RelayCommand<UserControl>(new System.Action<UserControl>(this.MinimizeBluestacksHandler), false);
      this.QuitCommand = (ICommand) new RelayCommand(new System.Action(this.QuitBlueStacks), false);
      if (this.ParentWindow.EngineInstanceRegistry.IsMinimizeSelectedOnReceiveGameNotificationPopup)
        this.IsMinimizeBlueStacksRadioBtnChecked = true;
      else
        this.IsQuitBluestacksChecked = true;
    }

    private void DoNotShowAgainPromptHandler()
    {
      if (!this.IsDoNotShowAgainChkBoxChecked)
        return;
      this.ParentWindow.EngineInstanceRegistry.IsShowMinimizeBlueStacksPopupOnClose = false;
    }

    private void QuitBlueStacks()
    {
      BlueStacks.Common.Stats.SendCommonClientStatsAsync("minimize_bluestacks_notification", "BlueStacks_exit_popup", this.ParentWindow.mVmName, "", "", "", "");
      this.ParentWindow.EngineInstanceRegistry.IsMinimizeSelectedOnReceiveGameNotificationPopup = false;
      this.DoNotShowAgainPromptHandler();
      this.ParentWindow.CloseWindowHandler(false);
    }

    private void MinimizeBluestacksHandler(UserControl control)
    {
      BlueStacks.Common.Stats.SendCommonClientStatsAsync("minimize_bluestacks_notification", "BlueStacks_minimize_popup", this.ParentWindow.mVmName, "", "", "", "");
      this.ParentWindow.EngineInstanceRegistry.IsMinimizeSelectedOnReceiveGameNotificationPopup = true;
      this.DoNotShowAgainPromptHandler();
      this.Close(control);
      this.ParentWindow.MinimizeWindowHandler();
    }

    private void Close(UserControl control)
    {
      try
      {
        BlueStacksUIUtils.CloseContainerWindow((FrameworkElement) control);
        this.ParentWindow.HideDimOverlay();
        control.Visibility = Visibility.Hidden;
      }
      catch (Exception ex)
      {
        Logger.Error("Exception while trying to close CloseBluestacksControl from dimoverlay " + ex.ToString());
      }
    }
  }
}
