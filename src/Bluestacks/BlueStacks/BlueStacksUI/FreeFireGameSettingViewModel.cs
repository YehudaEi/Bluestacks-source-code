// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.FreeFireGameSettingViewModel
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

namespace BlueStacks.BlueStacksUI
{
  public class FreeFireGameSettingViewModel : OtherAppGameSetting
  {
    private readonly MainWindow mParentWindow;
    private bool mOldOptimizeInGameSetting;
    private bool mOptimizeInGameSetting;

    public bool OptimizeInGameSetting
    {
      get
      {
        return this.mOptimizeInGameSetting;
      }
      set
      {
        this.SetProperty<bool>(ref this.mOptimizeInGameSetting, value, (string) null);
      }
    }

    public FreeFireGameSettingViewModel(
      MainWindow parentWindow,
      string appName,
      string packageName)
      : base(parentWindow, appName, packageName)
    {
      this.mParentWindow = parentWindow;
    }

    public override void Init()
    {
      base.Init();
      this.OptimizeInGameSetting = this.mParentWindow.EngineInstanceRegistry.IsFreeFireInGameSettingsCustomized;
    }

    public override void LockOriginal()
    {
      base.LockOriginal();
      this.mOldOptimizeInGameSetting = this.OptimizeInGameSetting;
    }

    public override bool HasChanged()
    {
      return base.HasChanged() || this.OptimizeInGameSetting != this.mOldOptimizeInGameSetting;
    }

    public override bool Save(bool restartReq)
    {
      restartReq = base.Save(restartReq);
      if (this.OptimizeInGameSetting != this.mOldOptimizeInGameSetting)
      {
        this.mParentWindow.EngineInstanceRegistry.IsFreeFireInGameSettingsCustomized = this.OptimizeInGameSetting;
        GameSettingViewModel.SendGameSettingsEnabledToGuest(this.mParentWindow, this.OptimizeInGameSetting);
        GameSettingViewModel.SendGameSettingsStat(this.OptimizeInGameSetting ? "freefire_optimizegame_enabled" : "freefire_optimizegame_disabled");
      }
      return restartReq;
    }
  }
}
