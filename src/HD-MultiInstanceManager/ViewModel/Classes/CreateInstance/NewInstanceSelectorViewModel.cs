// Decompiled with JetBrains decompiler
// Type: MultiInstanceManagerMVVM.ViewModel.Classes.CreateInstance.NewInstanceSelectorViewModel
// Assembly: HD-MultiInstanceManager, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 0C4C54E8-53C7-4A46-AE96-CC8E89A6B884
// Assembly location: C:\Program Files\BlueStacks\HD-MultiInstanceManager.exe

using BlueStacks.Common;
using BlueStacks.Core;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using MultiInstanceManagerMVVM.MessageModel.Classes;
using MultiInstanceManagerMVVM.View.Classes.CreateInstance;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace MultiInstanceManagerMVVM.ViewModel.Classes.CreateInstance
{
  public class NewInstanceSelectorViewModel : UiViewModelBase
  {
    public NewInstanceSelectorViewModel()
    {
      this.CloseWindowCommand = (ICommand) new RelayCommand(new System.Action(this.CloseCurrentWindow), false);
      this.CreateInstanceCommand = (ICommand) new RelayCommand<NewInstanceType>(new System.Action<NewInstanceType>(this.OnCreateNewInstance), false);
    }

    public ICommand CloseWindowCommand { get; set; }

    public ICommand CreateInstanceCommand { get; set; }

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

    private void CloseCurrentWindow()
    {
      if (this.View is Window view)
        view.Close();
      Messenger.Default.Send<ControlVisibilityMessage>(new ControlVisibilityMessage()
      {
        IsVisible = false
      });
    }

    private void OnCreateNewInstance(NewInstanceType instanceType)
    {
      Window window = (Window) null;
      if (this.View is Window view)
        window = view.Owner;
      this.CloseCurrentWindow();
      string str = "bgp".Equals("bgp", StringComparison.InvariantCultureIgnoreCase) ? "" : "_bgp";
      if (instanceType == NewInstanceType.Clone)
      {
        AppPlayerModel appPlayerModel = InstalledOem.GetAppPlayerModel("bgp", Utils.GetValueInBootParams("abivalue", "Android", string.Empty, "bgp"));
        Stats.SendMultiInstanceStatsAsync("clone_instance_clicked", "", "", "", 0, "", 0, "bgp", "", "", "", "", RegistryManager.Instance.CampaignMD5, true, "");
        Messenger.Default.Send<InstanceCreationMessage>(new InstanceCreationMessage()
        {
          AppPlayerModel = appPlayerModel,
          InstanceType = instanceType,
          CloneFromVm = instanceType == NewInstanceType.Clone ? "Android" + str : (string) null
        });
      }
      else
      {
        Stats.SendMultiInstanceStatsAsync("fresh_instance_clicked", "", "", "", 0, "", 0, "bgp", "", "", "", "", RegistryManager.Instance.CampaignMD5, true, "");
        Messenger.Default.Send<ControlVisibilityMessage>(new ControlVisibilityMessage()
        {
          IsVisible = true
        });
        SimpleIoc.Default.Unregister<NewEngineSelectorViewModel>();
        SimpleIoc.Default.Register<NewEngineSelectorViewModel>();
        NewEngineSelectorView engineSelectorView1 = new NewEngineSelectorView();
        engineSelectorView1.Owner = window;
        engineSelectorView1.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        NewEngineSelectorView engineSelectorView2 = engineSelectorView1;
        SimpleIoc.Default.GetInstance<NewEngineSelectorViewModel>().View = (IView) engineSelectorView2;
        SimpleIoc.Default.GetInstance<NewEngineSelectorViewModel>().SetOemData(InstalledOem.GetAppPlayerModel("bgp", Utils.GetValueInBootParams("abivalue", "Android", string.Empty, "bgp")));
        if (InstalledOem.CoexistingOemList.Count < 2)
          SimpleIoc.Default.GetInstance<NewEngineSelectorViewModel>().OemInstalledCheckABI();
        else
          engineSelectorView2.ShowDialog();
      }
    }
  }
}
