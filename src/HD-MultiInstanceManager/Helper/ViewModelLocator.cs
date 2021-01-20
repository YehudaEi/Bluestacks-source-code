// Decompiled with JetBrains decompiler
// Type: MultiInstanceManagerMVVM.Helper.ViewModelLocator
// Assembly: HD-MultiInstanceManager, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 0C4C54E8-53C7-4A46-AE96-CC8E89A6B884
// Assembly location: C:\Program Files\BlueStacks\HD-MultiInstanceManager.exe

using BlueStacks.Common;
using GalaSoft.MvvmLight.Ioc;
using MultiInstanceManagerMVVM.ViewModel.Classes;
using MultiInstanceManagerMVVM.ViewModel.Classes.CreateInstance;
using MultiInstanceManagerMVVM.ViewModel.Classes.Settings;
using System;

namespace MultiInstanceManagerMVVM.Helper
{
  public class ViewModelLocator
  {
    static ViewModelLocator()
    {
      SimpleIoc.Default.Register<MultiInstanceViewModel>();
      SimpleIoc.Default.Register<NewInstanceSelectorViewModel>();
      SimpleIoc.Default.Register<OptimizationSettingsViewModel>();
      SimpleIoc.Default.Register<NewEngineSelectorViewModel>();
      SimpleIoc.Default.Register<UpdateViewModel>();
      SimpleIoc.Default.Register<CreateNewInstanceViewModel>((Func<CreateNewInstanceViewModel>) (() => new CreateNewInstanceViewModel(NewInstanceType.Fresh, (AppPlayerModel) null, (string) null, "", (string) null, true)), NewInstanceType.Fresh.ToString());
      SimpleIoc.Default.Register<CreateNewInstanceViewModel>((Func<CreateNewInstanceViewModel>) (() => new CreateNewInstanceViewModel(NewInstanceType.Clone, (AppPlayerModel) null, (string) null, "", (string) null, true)), NewInstanceType.Clone.ToString());
    }

    public MultiInstanceViewModel MultiInstanceViewModel
    {
      get
      {
        return SimpleIoc.Default.GetInstance<MultiInstanceViewModel>();
      }
    }

    public InstanceViewModel InstanceViewModel
    {
      get
      {
        return SimpleIoc.Default.GetInstance<InstanceViewModel>();
      }
    }

    public OptimizationSettingsViewModel OptimizationSettingsViewModel
    {
      get
      {
        return SimpleIoc.Default.GetInstance<OptimizationSettingsViewModel>();
      }
    }

    public NewInstanceSelectorViewModel NewInstanceSelectorViewModel
    {
      get
      {
        return SimpleIoc.Default.GetInstance<NewInstanceSelectorViewModel>();
      }
    }

    public NewEngineSelectorViewModel NewEngineOemSelectorViewModel
    {
      get
      {
        return SimpleIoc.Default.GetInstance<NewEngineSelectorViewModel>();
      }
    }

    public UpdateViewModel UpdateOemViewModel
    {
      get
      {
        return SimpleIoc.Default.GetInstance<UpdateViewModel>();
      }
    }
  }
}
