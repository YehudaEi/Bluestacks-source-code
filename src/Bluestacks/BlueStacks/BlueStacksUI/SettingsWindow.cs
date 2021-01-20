// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.SettingsWindow
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace BlueStacks.BlueStacksUI
{
  public class SettingsWindow : SettingsWindowBase
  {
    internal List<string> mDuplicateShortcutsList = new List<string>();
    internal Dictionary<string, CustomSettingsButton> mSettingsButtons = new Dictionary<string, CustomSettingsButton>();
    internal CustomSettingsButton updateButton;
    internal CustomSettingsButton gameSettingsButton;
    internal bool mIsShortcutEdited;
    internal bool mIsShortcutSaveBtnEnabled;

    public MainWindow ParentWindow { get; }

    public SettingsWindow(MainWindow window, string startUpTab)
    {
      SettingsWindow settingsWindow = this;
      this.ParentWindow = window;
      this.SettingsControlNameList.Add("STRING_ENGINE_SETTING");
      this.SettingsControlNameList.Add("STRING_DISPLAY_SETTINGS");
      this.SettingsControlNameList.Add("STRING_PREFERENCES");
      if (window.mGuestBootCompleted)
        this.SettingsControlNameList.Add("STRING_DEVICE_SETTING");
      this.SettingsControlNameList.Add("STRING_NOTIFICATION");
      this.SettingsControlNameList.Add("STRING_GAME_SETTINGS");
      if (window.mCommonHandler.mShortcutsConfigInstance != null)
        this.SettingsControlNameList.Add("STRING_SHORTCUT_KEY_SETTINGS");
      else
        Logger.Warning("Not showing shortcuts settings as the config instance is null");
      this.SettingsControlNameList.Add("STRING_USER_DATA_SETTINGS");
      this.SettingsControlNameList.Add("STRING_ABOUT_SETTING");
      this.UpdateSettingsListAndStartTabForCustomOEMs();
      this.Loaded += (RoutedEventHandler) ((sender, e) => settingsWindow.SettingsWindow_Loaded(window));
      if (!string.IsNullOrEmpty(startUpTab))
        this.StartUpTab = startUpTab;
      this.CreateAllButtons(this.StartUpTab);
      this.ChangeSettingsTab(window, this.StartUpTab);
    }

    public void ChangeSettingsTab(MainWindow window, string tab)
    {
      UserControl control = this.GetUserControl(tab, window) ?? this.GetUserControl("STRING_ENGINE_SETTING", window);
      this.AddControlInGridAndDict(tab, control);
      this.BringToFront(control);
      if (this.mSettingsButtons[tab].IsSelected)
        return;
      this.mSettingsButtons[tab].IsSelected = true;
      this.mSettingsButtons[tab].IsEnabled = true;
    }

    public void UpdateSettingsListAndStartTabForCustomOEMs()
    {
      if (Oem.IsOEMDmm)
        this.SettingsControlNameList = new List<string>()
        {
          "STRING_DISPLAY_SETTINGS",
          "STRING_ENGINE_SETTING",
          "STRING_SCREENSHOT"
        };
      else if (FeatureManager.Instance.IsCustomUIForDMMSandbox)
      {
        this.SettingsControlNameList = new List<string>()
        {
          "STRING_ABOUT_SETTING"
        };
        this.StartUpTab = "STRING_ABOUT_SETTING";
      }
      else if (string.Equals(Oem.Instance.OEM, "yoozoo", StringComparison.InvariantCulture))
        this.SettingsControlNameList = new List<string>()
        {
          "STRING_DISPLAY_SETTINGS",
          "STRING_ENGINE_SETTING",
          "STRING_PREFERENCES"
        };
      else if (RegistryManager.Instance.InstallationType == InstallationTypes.GamingEdition)
      {
        this.SettingsControlNameList = new List<string>()
        {
          "STRING_DISPLAY_SETTINGS",
          "STRING_ENGINE_SETTING",
          "STRING_ABOUT_SETTING"
        };
      }
      else
      {
        if (!FeatureManager.Instance.IsCustomUIForNCSoft)
          return;
        this.SettingsControlNameList = new List<string>()
        {
          "STRING_DISPLAY_SETTINGS",
          "STRING_ENGINE_SETTING",
          "STRING_PREFERENCES",
          "STRING_SHORTCUT_KEY_SETTINGS",
          "STRING_USER_DATA_SETTINGS"
        };
      }
    }

    private UserControl GetUserControl(string controlName, MainWindow window)
    {
      UserControl userControl;
      switch (controlName)
      {
        case "STRING_ABOUT_SETTING":
          userControl = (UserControl) new AboutSettingsControl(window, this);
          break;
        case "STRING_DEVICE_SETTING":
          userControl = (UserControl) new DeviceProfileControl(window);
          break;
        case "STRING_DISPLAY_SETTINGS":
          userControl = (UserControl) new DisplaySettingsControl(window);
          break;
        case "STRING_ENGINE_SETTING":
          userControl = (UserControl) SettingsWindow.GetEngineView(window);
          break;
        case "STRING_GAME_SETTINGS":
          userControl = (UserControl) SettingsWindow.GetGameSettingView(window);
          break;
        case "STRING_NOTIFICATION":
          userControl = (UserControl) new NotificationsSettings(window);
          break;
        case "STRING_PREFERENCES":
          userControl = (UserControl) new PreferencesSettingsControl(window);
          break;
        case "STRING_SCREENSHOT":
          userControl = (UserControl) new DMMScreenshotSettingControl(window);
          break;
        case "STRING_SHORTCUT_KEY_SETTINGS":
          userControl = (UserControl) new ShortcutKeysControl(window, this);
          break;
        case "STRING_USER_DATA_SETTINGS":
          userControl = (UserControl) new BackupRestoreSettingsControl(window);
          break;
        default:
          userControl = (UserControl) null;
          break;
      }
      return userControl;
    }

    private static GameSettingView GetGameSettingView(MainWindow window)
    {
      GameSettingViewModel settingViewModel = new GameSettingViewModel(window);
      GameSettingView gameSettingView1 = new GameSettingView();
      gameSettingView1.Visibility = Visibility.Collapsed;
      gameSettingView1.DataContext = (object) settingViewModel;
      GameSettingView gameSettingView2 = gameSettingView1;
      settingViewModel.View = gameSettingView2;
      return gameSettingView2;
    }

    private static EngineSettingBase GetEngineView(MainWindow window)
    {
      EngineSettingBase engineSettingBase1 = new EngineSettingBase();
      engineSettingBase1.Visibility = Visibility.Collapsed;
      EngineSettingBase engineSettingBase2 = engineSettingBase1;
      EngineSettingViewModel settingViewModel = new EngineSettingViewModel(window, window.mVmName, engineSettingBase2, window.mSidebar.mIsEcoModeEnabled);
      engineSettingBase2.DataContext = (object) settingViewModel;
      return engineSettingBase2;
    }

    private void SettingsWindow_Loaded(MainWindow window)
    {
      Window.GetWindow((DependencyObject) this).Closing += new CancelEventHandler(this.SettingWindow_Closing);
      new Thread((ThreadStart) (() =>
      {
        Thread.Sleep(500);
        foreach (string settingsControlName in this.SettingsControlNameList)
        {
          string settingName = settingsControlName;
          if (!string.Equals(settingName, this.StartUpTab, StringComparison.InvariantCulture))
            this.Dispatcher.Invoke((Delegate) (() =>
            {
              UserControl userControl = this.GetUserControl(settingName, window);
              if (userControl == null)
                return;
              this.AddControlInGridAndDict(settingName, userControl);
              foreach (CustomSettingsButton child in this.SettingsWindowStackPanel.Children)
              {
                if (child.Name == settingName)
                  child.IsEnabled = true;
              }
            }));
        }
      }))
      {
        IsBackground = true
      }.Start();
    }

    private void SettingWindow_Closing(object sender, CancelEventArgs e)
    {
      try
      {
        MainWindow.CloseSettingsWindow((SettingsWindow) null);
        if (!this.mIsShortcutEdited || !this.mIsShortcutSaveBtnEnabled)
          return;
        CommonHandlers.ReloadShortcutsForAllInstances();
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in SettingsWindowClosing. Exception: " + ex?.ToString());
      }
    }

    private void CreateAllButtons(string mstartUpTab)
    {
      foreach (string settingsControlName in this.SettingsControlNameList)
      {
        CustomSettingsButton customSettingsButton1 = new CustomSettingsButton();
        customSettingsButton1.Name = settingsControlName;
        customSettingsButton1.Group = "Settings";
        CustomSettingsButton customSettingsButton2 = customSettingsButton1;
        this.mSettingsButtons.Add(settingsControlName, customSettingsButton2);
        TextBlock tb = new TextBlock()
        {
          FontSize = 15.0,
          TextWrapping = TextWrapping.Wrap
        };
        BlueStacksUIBinding.Bind(tb, settingsControlName, "");
        customSettingsButton2.Content = (object) tb;
        customSettingsButton2.MinHeight = 40.0;
        customSettingsButton2.FontWeight = FontWeights.Normal;
        customSettingsButton2.IsTabStop = false;
        customSettingsButton2.FocusVisualStyle = (Style) null;
        customSettingsButton2.IsEnabled = false;
        customSettingsButton2.PreviewMouseDown += new MouseButtonEventHandler(this.ValidateAndSwitchTab);
        this.SettingsWindowStackPanel.Children.Add((UIElement) customSettingsButton2);
        if (mstartUpTab == settingsControlName)
        {
          customSettingsButton2.IsEnabled = true;
          customSettingsButton2.IsSelected = true;
        }
      }
    }

    private void ValidateAndSwitchTab(object sender, MouseButtonEventArgs args)
    {
      if ((object) this.SettingsWindowControlsDict[(sender as CustomSettingsButton).Name].GetType() == (object) this.visibleControl.GetType())
        return;
      if (this.visibleControl is EngineSettingBase visibleControl && visibleControl.DataContext is EngineSettingBaseViewModel dataContext)
      {
        if (dataContext.Status == Status.Progress)
          Logger.Info("Compatibility check is running");
        else if (dataContext.IsDirty())
        {
          CustomMessageWindow customMessageWindow = new CustomMessageWindow();
          customMessageWindow.Owner = dataContext.Owner;
          customMessageWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
          BlueStacksUIBinding.Bind(customMessageWindow.TitleTextBlock, "STRING_DISCARD_CHANGES", "");
          BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlock, "STRING_SETTING_TAB_CHANGE_MESSAGE", "");
          customMessageWindow.AddButton(ButtonColors.Blue, "STRING_NO", (EventHandler) ((o, e) => args.Handled = true), (string) null, false, (object) null, true);
          customMessageWindow.AddButton(ButtonColors.White, "STRING_DISCARD_CHANGES", (EventHandler) ((o, e) => this.SettingsBtn_Click(sender, (RoutedEventArgs) null)), (string) null, false, (object) null, true);
          customMessageWindow.ShowDialog();
        }
        else
          this.SettingsBtn_Click(sender, (RoutedEventArgs) null);
      }
      else
      {
        DisplaySettingsControl displaySetting = this.visibleControl as DisplaySettingsControl;
        if (displaySetting != null && displaySetting.IsDirty())
        {
          CustomMessageWindow customMessageWindow = new CustomMessageWindow();
          customMessageWindow.Owner = (Window) displaySetting.ParentWindow;
          customMessageWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
          BlueStacksUIBinding.Bind(customMessageWindow.TitleTextBlock, "STRING_DISCARD_CHANGES", "");
          BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlock, "STRING_SETTING_TAB_CHANGE_MESSAGE", "");
          customMessageWindow.AddButton(ButtonColors.Blue, "STRING_NO", (EventHandler) ((o, e) => args.Handled = true), (string) null, false, (object) null, true);
          customMessageWindow.AddButton(ButtonColors.White, "STRING_DISCARD_CHANGES", (EventHandler) ((o, e) =>
          {
            displaySetting.DiscardCurrentChangingModel();
            this.SettingsBtn_Click(sender, (RoutedEventArgs) null);
          }), (string) null, false, (object) null, true);
          customMessageWindow.ShowDialog();
        }
        else
        {
          if (this.visibleControl is GameSettingView visibleControl)
          {
            GameSettingViewModel gameSettingViewModel = visibleControl.DataContext as GameSettingViewModel;
            if (gameSettingViewModel != null && gameSettingViewModel.IsDirty())
            {
              CustomMessageWindow customMessageWindow = new CustomMessageWindow();
              customMessageWindow.Owner = (Window) this.ParentWindow;
              customMessageWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
              BlueStacksUIBinding.Bind(customMessageWindow.TitleTextBlock, "STRING_DISCARD_CHANGES", "");
              BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlock, "STRING_SETTING_TAB_CHANGE_MESSAGE", "");
              customMessageWindow.AddButton(ButtonColors.Blue, "STRING_NO", (EventHandler) ((o, e) => args.Handled = true), (string) null, false, (object) null, true);
              customMessageWindow.AddButton(ButtonColors.White, "STRING_DISCARD_CHANGES", (EventHandler) ((o, e) =>
              {
                gameSettingViewModel.Reset();
                gameSettingViewModel.Init();
                this.SettingsBtn_Click(sender, (RoutedEventArgs) null);
              }), (string) null, false, (object) null, true);
              customMessageWindow.ShowDialog();
              return;
            }
          }
          DeviceProfileControl deviceSetting = this.visibleControl as DeviceProfileControl;
          if (deviceSetting != null && deviceSetting.IsDirty())
          {
            CustomMessageWindow customMessageWindow = new CustomMessageWindow();
            customMessageWindow.Owner = (Window) this.ParentWindow;
            customMessageWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            BlueStacksUIBinding.Bind(customMessageWindow.TitleTextBlock, "STRING_DISCARD_CHANGES", "");
            BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlock, "STRING_SETTING_TAB_CHANGE_MESSAGE", "");
            customMessageWindow.AddButton(ButtonColors.Blue, "STRING_NO", (EventHandler) ((o, e) => args.Handled = true), (string) null, false, (object) null, true);
            customMessageWindow.AddButton(ButtonColors.White, "STRING_DISCARD_CHANGES", (EventHandler) ((o, e) =>
            {
              deviceSetting.Init();
              this.SettingsBtn_Click(sender, (RoutedEventArgs) null);
            }), (string) null, false, (object) null, true);
            customMessageWindow.ShowDialog();
          }
          else
          {
            PreferencesSettingsControl preferencesSetting = this.visibleControl as PreferencesSettingsControl;
            if (preferencesSetting != null && preferencesSetting.IsDirty())
            {
              CustomMessageWindow customMessageWindow = new CustomMessageWindow();
              customMessageWindow.Owner = (Window) this.ParentWindow;
              customMessageWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
              BlueStacksUIBinding.Bind(customMessageWindow.TitleTextBlock, "STRING_DISCARD_CHANGES", "");
              BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlock, "STRING_SETTING_TAB_CHANGE_MESSAGE", "");
              customMessageWindow.AddButton(ButtonColors.Blue, "STRING_NO", (EventHandler) ((o, e) => args.Handled = true), (string) null, false, (object) null, true);
              customMessageWindow.AddButton(ButtonColors.White, "STRING_DISCARD_CHANGES", (EventHandler) ((o, e) =>
              {
                preferencesSetting.DiscardChanges();
                this.SettingsBtn_Click(sender, (RoutedEventArgs) null);
              }), (string) null, false, (object) null, true);
              customMessageWindow.ShowDialog();
            }
            else
              this.SettingsBtn_Click(sender, (RoutedEventArgs) null);
          }
        }
      }
    }

    protected override void SetPopupOffset()
    {
      new Thread((ThreadStart) (() => this.Dispatcher.Invoke((Delegate) (() =>
      {
        if (this.ParentWindow.mTopBar.mSnailMode != PerformanceState.VtxDisabled || this.IsVtxLearned || !this.CheckWidth())
          return;
        this.EnableVTPopup.HorizontalOffset = this.SettingsWindowStackPanel.ActualWidth;
        this.EnableVTPopup.Width = this.SettingsWindowGrid.ActualWidth;
        this.EnableVTPopup.IsOpen = true;
        this.EnableVTPopup.StaysOpen = true;
      }), DispatcherPriority.Render)))
      {
        IsBackground = true
      }.Start();
    }

    public override void CloseButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      Logger.Info("Clicked settings menu close button");
      bool staysOpen = false;
      bool hasChanges = false;
      switch (this.visibleControl)
      {
        case EngineSettingBase engineSettingBase:
          EngineSettingViewModel dataContext1 = engineSettingBase.DataContext as EngineSettingViewModel;
          if (dataContext1.Status == Status.Progress)
          {
            Logger.Info("Compatibility check is running");
            return;
          }
          hasChanges = dataContext1.IsDirty();
          break;
        case DisplaySettingsControl displaySettingsControl:
          hasChanges = displaySettingsControl.IsDirty();
          break;
        case GameSettingView gameSettingView:
          hasChanges = (gameSettingView.DataContext as GameSettingViewModel).IsDirty();
          break;
        case DeviceProfileControl deviceProfileControl:
          hasChanges = deviceProfileControl.IsDirty();
          break;
        case PreferencesSettingsControl preferencesSettingsControl:
          hasChanges = preferencesSettingsControl.IsDirty();
          break;
      }
      if (Oem.IsOEMDmm)
        hasChanges = false;
      if (hasChanges)
      {
        CustomMessageWindow customMessageWindow = new CustomMessageWindow();
        BlueStacksUIBinding.Bind(customMessageWindow.TitleTextBlock, "STRING_DISCARD_CHANGES", "");
        BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlock, LocaleStrings.GetLocalizedString("STRING_SETTING_CLOSE_MESSAGE", ""), "");
        customMessageWindow.AddButton(ButtonColors.Blue, "STRING_NO", (EventHandler) ((o, evt) => {}), (string) null, false, (object) null, true);
        customMessageWindow.AddButton(ButtonColors.White, "STRING_DISCARD_CHANGES", (EventHandler) ((o, evt) =>
        {
          if (this.visibleControl.DataContext is GameSettingViewModel dataContext2)
            dataContext2.Reset();
          hasChanges = false;
        }), (string) null, false, (object) null, true);
        customMessageWindow.Owner = (Window) this.ParentWindow;
        customMessageWindow.ShowDialog();
      }
      if (hasChanges)
        return;
      GrmHandler.RequirementConfigUpdated(this.ParentWindow.mVmName);
      if (this.mIsShortcutEdited && this.mIsShortcutSaveBtnEnabled)
      {
        CustomMessageWindow customMessageWindow = new CustomMessageWindow();
        BlueStacksUIBinding.Bind(customMessageWindow.TitleTextBlock, "STRING_SAVE_CHANGES_QUESTION", "");
        BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlock, "STRING_UNSAVED_CHANGES", "");
        customMessageWindow.AddButton(ButtonColors.Blue, "STRING_SAVE_CHANGES", (EventHandler) ((o, evt) =>
        {
          this.ParentWindow.mCommonHandler.SaveAndReloadShortcuts();
          this.mIsShortcutEdited = false;
        }), (string) null, false, (object) null, true);
        customMessageWindow.AddButton(ButtonColors.White, "STRING_DISCARD", (EventHandler) ((o, evt) => CommonHandlers.ReloadShortcutsForAllInstances()), (string) null, false, (object) null, true);
        customMessageWindow.Owner = (Window) this.ParentWindow;
        customMessageWindow.CloseButton.PreviewMouseLeftButtonUp += (MouseButtonEventHandler) ((o, evt) => staysOpen = true);
        customMessageWindow.ShowDialog();
      }
      else if (this.mDuplicateShortcutsList.Count > 0)
        CommonHandlers.ReloadShortcutsForAllInstances();
      if (staysOpen)
        return;
      BlueStacksUIUtils.CloseContainerWindow((FrameworkElement) this);
    }
  }
}
