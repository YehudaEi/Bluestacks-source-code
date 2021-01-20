// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.GameSettingViewModel
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace BlueStacks.BlueStacksUI
{
  public class GameSettingViewModel : ViewModelBase
  {
    private static readonly string sKnowMoreBaseUrl = WebHelper.GetUrlWithParams(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) WebHelper.GetServerHost(), (object) "help_articles"), (string) null, (string) null, (string) null) + "&article=";
    private Visibility mLearnMoreVisibility = Visibility.Collapsed;
    private readonly MainWindow mParentWindow;
    private CursorMode mPreviousCursorMode;
    private CursorMode mCursorMode;
    private string mImageName;
    private string mAppName;
    private string mPackageName;
    private CurrentGame mCurrentGame;
    private Uri mLearnMoreUri;
    private OtherAppGameSetting mOtherAppGameSetting;
    private FreeFireGameSettingViewModel mFreeFireGameSettingViewModel;
    private PubgGameSettingViewModel mPubgGameSettingViewModel;
    private CallOfDutyGameSettingViewModel mCallOfDutyGameSettingViewModel;
    private Visibility mShowGuideVisibility;
    private string mCustomCursorImageName;
    private CustomToastPopupControl mToastPopup;

    public GameSettingView View { get; set; }

    public CursorMode CursorMode
    {
      get
      {
        return this.mCursorMode;
      }
      set
      {
        this.SetProperty<CursorMode>(ref this.mCursorMode, value, (string) null);
      }
    }

    public string ImageName
    {
      get
      {
        return this.mImageName;
      }
      set
      {
        this.SetProperty<string>(ref this.mImageName, value, (string) null);
      }
    }

    public string AppName
    {
      get
      {
        return this.mAppName;
      }
      set
      {
        this.SetProperty<string>(ref this.mAppName, value, (string) null);
      }
    }

    public string PackageName
    {
      get
      {
        return this.mPackageName;
      }
      set
      {
        this.SetProperty<string>(ref this.mPackageName, value, (string) null);
      }
    }

    public CurrentGame CurrentGame
    {
      get
      {
        return this.mCurrentGame;
      }
      set
      {
        this.SetProperty<CurrentGame>(ref this.mCurrentGame, value, (string) null);
      }
    }

    public Uri LearnMoreUri
    {
      get
      {
        return this.mLearnMoreUri;
      }
      set
      {
        this.SetProperty<Uri>(ref this.mLearnMoreUri, value, (string) null);
      }
    }

    public Visibility LearnMoreVisibility
    {
      get
      {
        return this.mLearnMoreVisibility;
      }
      set
      {
        this.SetProperty<Visibility>(ref this.mLearnMoreVisibility, value, (string) null);
      }
    }

    public OtherAppGameSetting OtherAppGameSetting
    {
      get
      {
        return this.mOtherAppGameSetting;
      }
      set
      {
        this.SetProperty<OtherAppGameSetting>(ref this.mOtherAppGameSetting, value, (string) null);
      }
    }

    public FreeFireGameSettingViewModel FreeFireGameSettingViewModel
    {
      get
      {
        return this.mFreeFireGameSettingViewModel;
      }
      set
      {
        this.SetProperty<FreeFireGameSettingViewModel>(ref this.mFreeFireGameSettingViewModel, value, (string) null);
      }
    }

    public PubgGameSettingViewModel PubgGameSettingViewModel
    {
      get
      {
        return this.mPubgGameSettingViewModel;
      }
      set
      {
        this.SetProperty<PubgGameSettingViewModel>(ref this.mPubgGameSettingViewModel, value, (string) null);
      }
    }

    public CallOfDutyGameSettingViewModel CallOfDutyGameSettingViewModel
    {
      get
      {
        return this.mCallOfDutyGameSettingViewModel;
      }
      set
      {
        this.SetProperty<CallOfDutyGameSettingViewModel>(ref this.mCallOfDutyGameSettingViewModel, value, (string) null);
      }
    }

    public Visibility ShowGuideVisibility
    {
      get
      {
        return this.mShowGuideVisibility;
      }
      set
      {
        this.SetProperty<Visibility>(ref this.mShowGuideVisibility, value, (string) null);
      }
    }

    public string CustomCursorImageName
    {
      get
      {
        return this.mCustomCursorImageName;
      }
      set
      {
        this.SetProperty<string>(ref this.mCustomCursorImageName, value, (string) null);
      }
    }

    public ICommand SaveCommand { get; set; }

    public ICommand OpenGameGuideCommand { get; set; }

    public GameSettingViewModel(MainWindow owner)
    {
      this.mParentWindow = owner;
      if (owner?.StaticComponents.mSelectedTabButton != null)
      {
        this.ImageName = owner?.StaticComponents.mSelectedTabButton.mAppTabIcon.ImageName;
        this.AppName = owner?.StaticComponents.mSelectedTabButton.AppLabel;
        this.PackageName = owner?.StaticComponents.mSelectedTabButton.PackageName;
        this.OpenGameGuideCommand = (ICommand) new RelayCommand2(new System.Action<object>(this.OpenGameGuide));
        this.CustomCursorImageName = !string.Equals(this.PackageName, "com.supercell.brawlstars", StringComparison.InvariantCultureIgnoreCase) ? "yellow_cursor" : "yellow_cursor_brawl";
        this.CurrentGame = owner == null || owner.StaticComponents.mSelectedTabButton.mTabType != TabType.AppTab ? CurrentGame.None : (!"com.dts.freefireth".Contains(this.PackageName) ? (!PackageActivityNames.ThirdParty.AllPUBGPackageNames.Contains(this.PackageName) ? (!PackageActivityNames.ThirdParty.AllCallOfDutyPackageNames.Contains(this.PackageName) ? CurrentGame.OtherApp : CurrentGame.CallOfDuty) : CurrentGame.PubG) : CurrentGame.FreeFire);
        this.OtherAppGameSetting = new OtherAppGameSetting(this.mParentWindow, this.AppName, this.PackageName);
        this.FreeFireGameSettingViewModel = new FreeFireGameSettingViewModel(this.mParentWindow, this.AppName, this.PackageName);
        this.PubgGameSettingViewModel = new PubgGameSettingViewModel(this.mParentWindow, this.AppName, this.PackageName);
        this.CallOfDutyGameSettingViewModel = new CallOfDutyGameSettingViewModel(this.mParentWindow, this.AppName, this.PackageName);
      }
      this.SaveCommand = (ICommand) new RelayCommand2(new Func<object, bool>(this.CanSave), new System.Action<object>(this.SaveGameSettings));
      this.Init();
    }

    public void Init()
    {
      this.CursorMode = RegistryManager.Instance.CustomCursorEnabled ? CursorMode.Custom : CursorMode.Normal;
      this.mPreviousCursorMode = this.CursorMode;
      if (this.mParentWindow.StaticComponents.mSelectedTabButton == null)
        return;
      this.ShowGuideVisibility = this.mParentWindow.SelectedConfig.SelectedControlScheme == null || !this.mParentWindow.SelectedConfig.SelectedControlScheme.GameControls.Any<IMAction>() || !this.mParentWindow.SelectedConfig.SelectedControlScheme.GameControls.SelectMany<IMAction, KeyValuePair<string, string>>((Func<IMAction, IEnumerable<KeyValuePair<string, string>>>) (action => (IEnumerable<KeyValuePair<string, string>>) action.Guidance)).Any<KeyValuePair<string, string>>() ? Visibility.Collapsed : Visibility.Visible;
      switch (this.CurrentGame)
      {
        case CurrentGame.PubG:
          this.LearnMoreVisibility = Visibility.Visible;
          this.LearnMoreUri = new Uri(GameSettingViewModel.sKnowMoreBaseUrl + "game_settings_know_more_pubg");
          this.PubgGameSettingViewModel.Init();
          this.PubgGameSettingViewModel.LockOriginal();
          this.OtherAppGameSetting = (OtherAppGameSetting) this.PubgGameSettingViewModel;
          break;
        case CurrentGame.CallOfDuty:
          this.LearnMoreVisibility = Visibility.Visible;
          this.LearnMoreUri = new Uri(GameSettingViewModel.sKnowMoreBaseUrl + "game_settings_know_more_callofduty");
          this.CallOfDutyGameSettingViewModel.Init();
          this.CallOfDutyGameSettingViewModel.LockOriginal();
          this.OtherAppGameSetting = (OtherAppGameSetting) this.CallOfDutyGameSettingViewModel;
          break;
        case CurrentGame.FreeFire:
          this.LearnMoreVisibility = Visibility.Visible;
          this.LearnMoreUri = new Uri(GameSettingViewModel.sKnowMoreBaseUrl + "game_settings_know_more_freefire");
          this.FreeFireGameSettingViewModel.Init();
          this.FreeFireGameSettingViewModel.LockOriginal();
          this.OtherAppGameSetting = (OtherAppGameSetting) this.FreeFireGameSettingViewModel;
          break;
        case CurrentGame.OtherApp:
          this.OtherAppGameSetting.Init();
          this.OtherAppGameSetting.LockOriginal();
          if (this.OtherAppGameSetting.PlayInLandscapeModeVisibility == Visibility.Visible || this.OtherAppGameSetting.PlayInPortraitModeVisibility == Visibility.Visible)
          {
            this.LearnMoreVisibility = Visibility.Visible;
            this.LearnMoreUri = new Uri(GameSettingViewModel.sKnowMoreBaseUrl + "game_settings_know_more_sevendeadly");
            break;
          }
          if (this.ShowGuideVisibility != Visibility.Collapsed)
            break;
          this.CurrentGame = CurrentGame.None;
          this.LearnMoreVisibility = Visibility.Collapsed;
          break;
        default:
          this.LearnMoreVisibility = Visibility.Collapsed;
          break;
      }
    }

    private void OpenGameGuide(object obj)
    {
      if (KMManager.sGuidanceWindow != null && KMManager.sGuidanceWindow.Visibility == Visibility.Visible)
        KMManager.sGuidanceWindow.Highlight();
      else if (this.IsDirty())
      {
        CustomMessageWindow customMessageWindow = new CustomMessageWindow();
        customMessageWindow.Owner = (Window) this.mParentWindow;
        customMessageWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        BlueStacksUIBinding.Bind(customMessageWindow.TitleTextBlock, "STRING_DISCARD_CHANGES", "");
        BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlock, "STRING_SETTING_CLOSE_MESSAGE", "");
        customMessageWindow.AddButton(ButtonColors.Blue, "STRING_NO", (EventHandler) ((o, e) => {}), (string) null, false, (object) null, true);
        customMessageWindow.AddButton(ButtonColors.White, "STRING_DISCARD_CHANGES", (EventHandler) ((o, e) => this.OpenGameGuide()), (string) null, false, (object) null, true);
        customMessageWindow.ShowDialog();
      }
      else
        this.OpenGameGuide();
    }

    private void OpenGameGuide()
    {
      BlueStacksUIUtils.CloseContainerWindow((FrameworkElement) MainWindow.SettingsWindow);
      if (!this.mParentWindow.mCommonHandler.ToggleGamepadAndKeyboardGuidance("gamepad", false))
        KMManager.HandleInputMapperWindow(this.mParentWindow, "gamepad");
      ClientStats.SendMiscellaneousStatsAsync("game_setting", RegistryManager.Instance.UserGuid, "gameGuide", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, this.PackageName, (string) null, "Android");
    }

    private bool CanSave(object obj)
    {
      return this.IsDirty();
    }

    public bool IsDirty()
    {
      bool flag = false;
      switch (this.CurrentGame)
      {
        case CurrentGame.PubG:
          flag = this.PubgGameSettingViewModel.HasChanged();
          break;
        case CurrentGame.CallOfDuty:
          flag = this.CallOfDutyGameSettingViewModel.HasChanged();
          break;
        case CurrentGame.FreeFire:
          flag = this.FreeFireGameSettingViewModel.HasChanged();
          break;
        case CurrentGame.OtherApp:
          flag = this.OtherAppGameSetting.HasChanged();
          break;
      }
      return flag || this.mPreviousCursorMode != this.CursorMode;
    }

    private void SaveGameSettings(object obj)
    {
      bool restartReq = false;
      bool flag = false;
      switch (this.CurrentGame)
      {
        case CurrentGame.PubG:
          restartReq = this.PubgGameSettingViewModel.Save(restartReq);
          this.PubgGameSettingViewModel.LockOriginal();
          flag = true;
          break;
        case CurrentGame.CallOfDuty:
          restartReq = this.CallOfDutyGameSettingViewModel.Save(restartReq);
          this.CallOfDutyGameSettingViewModel.LockOriginal();
          flag = true;
          break;
        case CurrentGame.FreeFire:
          restartReq = this.FreeFireGameSettingViewModel.Save(restartReq);
          this.FreeFireGameSettingViewModel.LockOriginal();
          flag = true;
          break;
        case CurrentGame.OtherApp:
          restartReq = this.OtherAppGameSetting.Save(restartReq);
          this.OtherAppGameSetting.LockOriginal();
          flag = true;
          break;
      }
      if (flag)
        ClientStats.SendMiscellaneousStatsAsync("game_setting", RegistryManager.Instance.UserGuid, this.AppName, "Save", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
      if (this.mPreviousCursorMode != this.CursorMode)
      {
        RegistryManager.Instance.CustomCursorEnabled = this.CursorMode == CursorMode.Custom;
        foreach (MainWindow mainWindow in BlueStacksUIUtils.DictWindows.Values)
          mainWindow.mCommonHandler.SetCustomCursorForApp(this.PackageName);
        this.mPreviousCursorMode = this.CursorMode;
        ClientStats.SendMiscellaneousStatsAsync("game_setting", RegistryManager.Instance.UserGuid, "Is Custom Cursor", (this.CursorMode == CursorMode.Custom).ToString((IFormatProvider) CultureInfo.InvariantCulture), RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
      }
      this.AddToastPopup(LocaleStrings.GetLocalizedString("STRING_CHANGES_SAVED", ""));
      if (!restartReq)
        return;
      GameSettingViewModel.RestartApp(this.mParentWindow, this.AppName);
    }

    public static void SendGameSettingsEnabledToGuest(MainWindow parentWindow, bool enabled)
    {
      VmCmdHandler.RunCommandAsync(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} {1}", (object) "gameSettingsEnabled", (object) ("{" + string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\"package_name\":\"{0}\",", (object) "com.dts.freefireth") + string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\"game_settings_enabled\":\"{0}\"", (object) enabled.ToString((IFormatProvider) CultureInfo.InvariantCulture)) + "}")), (UIHelper.Action) null, (System.Windows.Forms.Control) null, parentWindow?.mVmName);
    }

    public static void SendGameSettingsStat(string statsTag)
    {
      ClientStats.SendMiscellaneousStatsAsync("game_setting", RegistryManager.Instance.UserGuid, statsTag, string.Empty, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
    }

    protected void AddToastPopup(string message)
    {
      try
      {
        if (this.mToastPopup == null)
          this.mToastPopup = new CustomToastPopupControl((System.Windows.Controls.UserControl) this.View);
        this.mToastPopup.Init((System.Windows.Controls.UserControl) this.View, message, (Brush) null, (Brush) null, System.Windows.HorizontalAlignment.Center, VerticalAlignment.Bottom, new Thickness?(), 12, new Thickness?(), (Brush) null);
        this.mToastPopup.ShowPopup(1.3);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in showing toast popup: " + ex.ToString());
      }
    }

    public static void RestartApp(MainWindow parentWindow, string appName)
    {
      CustomMessageWindow customMessageWindow1 = new CustomMessageWindow();
      customMessageWindow1.Owner = (Window) parentWindow;
      CustomMessageWindow customMessageWindow2 = customMessageWindow1;
      string path1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_RESTART", ""), (object) appName);
      BlueStacksUIBinding.Bind(customMessageWindow2.TitleTextBlock, path1, "");
      string path2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_SETTING_CHANGED_RESTART_APP_MESSAGE", ""), (object) appName);
      BlueStacksUIBinding.Bind(customMessageWindow2.BodyTextBlock, path2, "");
      customMessageWindow2.AddButton(ButtonColors.Blue, "STRING_RESTART_NOW", (EventHandler) ((o, e) =>
      {
        if (MainWindow.SettingsWindow.ParentWindow == parentWindow)
          BlueStacksUIUtils.CloseContainerWindow((FrameworkElement) MainWindow.SettingsWindow);
        Thread thread = new Thread((ThreadStart) (() => parentWindow.mTopBar.mAppTabButtons.RestartTab(parentWindow.StaticComponents.mSelectedTabButton.PackageName)));
        thread.IsBackground = true;
        Logger.Info("Restarting Game Tab.");
        thread.Start();
      }), (string) null, false, (object) null, true);
      customMessageWindow2.AddButton(ButtonColors.White, "STRING_CANCEL", (EventHandler) null, (string) null, false, (object) null, true);
      customMessageWindow2.ShowDialog();
    }

    public void Reset()
    {
      if (this.OtherAppGameSetting.ShowSensitivity != Visibility.Visible)
        return;
      this.OtherAppGameSetting.Reset();
    }
  }
}
