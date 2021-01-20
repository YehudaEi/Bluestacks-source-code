// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.CallOfDutyGameSettingViewModel
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System;

namespace BlueStacks.BlueStacksUI
{
  public class CallOfDutyGameSettingViewModel : OtherAppGameSetting
  {
    private InGameResolution mOldInGameResolution;
    private GraphicsQuality mOldGraphicsQuality;
    private InGameResolution mInGameResolution;
    private GraphicsQuality mGraphicsQuality;

    public InGameResolution InGameResolution
    {
      get
      {
        return this.mInGameResolution;
      }
      set
      {
        this.SetProperty<InGameResolution>(ref this.mInGameResolution, value, (string) null);
      }
    }

    public GraphicsQuality GraphicsQuality
    {
      get
      {
        return this.mGraphicsQuality;
      }
      set
      {
        this.SetProperty<GraphicsQuality>(ref this.mGraphicsQuality, value, (string) null);
      }
    }

    public CallOfDutyGameSettingViewModel(
      MainWindow parentWindow,
      string appName,
      string packageName)
      : base(parentWindow, appName, packageName)
    {
    }

    public override void Init()
    {
      base.Init();
      if (string.IsNullOrEmpty(RegistryManager.Instance.Guest[this.ParentWindow.mVmName].GamingResolutionCOD) || string.Equals(RegistryManager.Instance.Guest[this.ParentWindow.mVmName].GamingResolutionCOD, "720", StringComparison.InvariantCultureIgnoreCase))
        this.InGameResolution = InGameResolution.HD_720p;
      else if (string.Equals(RegistryManager.Instance.Guest[this.ParentWindow.mVmName].GamingResolutionCOD, "1080", StringComparison.InvariantCultureIgnoreCase))
        this.InGameResolution = InGameResolution.FHD_1080p;
      else if (string.Equals(RegistryManager.Instance.Guest[this.ParentWindow.mVmName].GamingResolutionCOD, "1440", StringComparison.InvariantCultureIgnoreCase))
        this.InGameResolution = InGameResolution.QHD_1440p;
      else if (string.Equals(RegistryManager.Instance.Guest[this.ParentWindow.mVmName].GamingResolutionCOD, "2160", StringComparison.InvariantCultureIgnoreCase))
        this.InGameResolution = InGameResolution.UHD_2160p;
      if (string.IsNullOrEmpty(RegistryManager.Instance.Guest[this.ParentWindow.mVmName].DisplayQualityCOD) || string.Equals(RegistryManager.Instance.Guest[this.ParentWindow.mVmName].DisplayQualityCOD, "-1", StringComparison.InvariantCultureIgnoreCase))
        this.GraphicsQuality = GraphicsQuality.Auto;
      else if (string.Equals(RegistryManager.Instance.Guest[this.ParentWindow.mVmName].DisplayQualityCOD, "0", StringComparison.InvariantCultureIgnoreCase))
        this.GraphicsQuality = GraphicsQuality.Smooth;
      else if (string.Equals(RegistryManager.Instance.Guest[this.ParentWindow.mVmName].DisplayQualityCOD, "1", StringComparison.InvariantCultureIgnoreCase))
      {
        this.GraphicsQuality = GraphicsQuality.Balanced;
      }
      else
      {
        if (!string.Equals(RegistryManager.Instance.Guest[this.ParentWindow.mVmName].DisplayQualityCOD, "2", StringComparison.InvariantCultureIgnoreCase))
          return;
        this.GraphicsQuality = GraphicsQuality.HD;
      }
    }

    public override void LockOriginal()
    {
      base.LockOriginal();
      this.mOldInGameResolution = this.InGameResolution;
      this.mOldGraphicsQuality = this.GraphicsQuality;
    }

    public override bool HasChanged()
    {
      return base.HasChanged() || this.InGameResolution != this.mOldInGameResolution || this.GraphicsQuality != this.mOldGraphicsQuality;
    }

    public override bool Save(bool restartReq)
    {
      restartReq = base.Save(restartReq);
      if (this.InGameResolution != this.mOldInGameResolution)
      {
        restartReq = true;
        switch (this.InGameResolution)
        {
          case InGameResolution.HD_720p:
            RegistryManager.Instance.Guest[this.ParentWindow.mVmName].GamingResolutionCOD = "720";
            GameSettingViewModel.SendGameSettingsStat("cod_res_720");
            break;
          case InGameResolution.FHD_1080p:
            RegistryManager.Instance.Guest[this.ParentWindow.mVmName].GamingResolutionCOD = "1080";
            GameSettingViewModel.SendGameSettingsStat("cod_res_1080");
            break;
          case InGameResolution.QHD_1440p:
            RegistryManager.Instance.Guest[this.ParentWindow.mVmName].GamingResolutionCOD = "1440";
            GameSettingViewModel.SendGameSettingsStat("cod_res_1440");
            break;
          case InGameResolution.UHD_2160p:
            RegistryManager.Instance.Guest[this.ParentWindow.mVmName].GamingResolutionCOD = "2160";
            GameSettingViewModel.SendGameSettingsStat("cod_res_2160");
            break;
        }
      }
      if (this.GraphicsQuality != this.mOldGraphicsQuality)
      {
        restartReq = true;
        switch (this.GraphicsQuality)
        {
          case GraphicsQuality.Auto:
            RegistryManager.Instance.Guest[this.ParentWindow.mVmName].DisplayQualityCOD = "-1";
            GameSettingViewModel.SendGameSettingsStat("pubg_gfx_auto");
            break;
          case GraphicsQuality.Smooth:
            RegistryManager.Instance.Guest[this.ParentWindow.mVmName].DisplayQualityCOD = "0";
            GameSettingViewModel.SendGameSettingsStat("pubg_gfx_smooth");
            break;
          case GraphicsQuality.Balanced:
            RegistryManager.Instance.Guest[this.ParentWindow.mVmName].DisplayQualityCOD = "1";
            GameSettingViewModel.SendGameSettingsStat("pubg_gfx_balanced");
            break;
          case GraphicsQuality.HD:
            RegistryManager.Instance.Guest[this.ParentWindow.mVmName].DisplayQualityCOD = "2";
            GameSettingViewModel.SendGameSettingsStat("pubg_gfx_hd");
            break;
        }
      }
      return restartReq;
    }
  }
}
