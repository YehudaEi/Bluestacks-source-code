// Decompiled with JetBrains decompiler
// Type: MultiInstanceManagerMVVM.View.Classes.MultiInstance.Settings.SettingsWindow
// Assembly: HD-MultiInstanceManager, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 0C4C54E8-53C7-4A46-AE96-CC8E89A6B884
// Assembly location: C:\Program Files\BlueStacks\HD-MultiInstanceManager.exe

using BlueStacks.Common;
using BlueStacks.ViewModel.Classes.Settings;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MultiInstanceManagerMVVM.View.Classes.MultiInstance.Settings
{
  public class SettingsWindow : SettingsWindowBase
  {
    private bool mIsEcoModeEnabled = false;
    private string vmNameWithSuffix;
    private Window ParentWindow;

    public string OEM { get; private set; }

    public string VmName { get; private set; }

    public SettingsWindow(Window window, string vmName, string startUpTab, bool isEcoModeEnabled)
    {
      this.mIsEcoModeEnabled = isEcoModeEnabled;
      this.vmNameWithSuffix = vmName;
      this.VmName = this.vmNameWithSuffix;
      this.ParentWindow = window;
      this.SettingsControlNameList.Add("STRING_ENGINE_SETTING");
      this.SettingsControlNameList.Add("STRING_DISPLAY_SETTINGS");
      this.OEM = InstalledOem.GetOemFromVmnameWithSuffix(this.vmNameWithSuffix);
      string oldValue = this.OEM.Equals("bgp", StringComparison.InvariantCultureIgnoreCase) ? "" : "_" + this.OEM;
      if (!string.IsNullOrEmpty(oldValue))
        this.VmName = this.vmNameWithSuffix?.Replace(oldValue, "");
      this.Loaded += (RoutedEventHandler) ((sender, e) => this.SettingsWindow_Loaded(sender, e));
      if (!string.IsNullOrEmpty(startUpTab))
        this.StartUpTab = startUpTab;
      this.CreateAllButtons(this.StartUpTab);
      UserControl control = this.GetUserControl(this.StartUpTab) ?? this.GetUserControl("STRING_ENGINE_SETTING");
      this.AddControlInGridAndDict(this.StartUpTab, control);
      this.BringToFront(control);
    }

    private void SettingsWindow_Loaded(object sender, RoutedEventArgs e)
    {
      new Thread((ThreadStart) (() =>
      {
        Thread.Sleep(500);
        foreach (string settingsControlName in this.SettingsControlNameList)
        {
          string settingName = settingsControlName;
          if (!settingName.Equals(this.StartUpTab, StringComparison.InvariantCulture))
            this.Dispatcher.Invoke((Delegate) (() =>
            {
              UserControl userControl = this.GetUserControl(settingName);
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

    private UserControl GetUserControl(string controlName)
    {
      switch (controlName)
      {
        case "STRING_DISPLAY_SETTINGS":
          return (UserControl) new InstanceDisplaySettings(this.ParentWindow, this.VmName, this.OEM);
        case "STRING_ENGINE_SETTING":
          EngineSettingBase engineSettingBase1 = new EngineSettingBase();
          engineSettingBase1.Visibility = Visibility.Collapsed;
          EngineSettingBase engineSettingBase2 = engineSettingBase1;
          EngineSettingViewModel settingViewModel = new EngineSettingViewModel(this.ParentWindow, this.VmName, engineSettingBase2, this.OEM, this.mIsEcoModeEnabled);
          engineSettingBase2.DataContext = (object) settingViewModel;
          return (UserControl) engineSettingBase2;
        default:
          return (UserControl) null;
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
        TextBlock tb = new TextBlock()
        {
          FontSize = 16.0,
          TextWrapping = TextWrapping.Wrap
        };
        BlueStacksUIBinding.Bind(tb, settingsControlName, "");
        customSettingsButton2.Content = (object) tb;
        customSettingsButton2.MinHeight = 40.0;
        customSettingsButton2.FontWeight = FontWeights.SemiBold;
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
      if (this.SettingsWindowControlsDict[(sender as CustomSettingsButton).Name].GetType() == this.visibleControl.GetType())
        return;
      EngineSettingBaseViewModel dataContext;
      int num;
      if (this.visibleControl is EngineSettingBase visibleControl)
      {
        dataContext = visibleControl.DataContext as EngineSettingBaseViewModel;
        num = dataContext != null ? 1 : 0;
      }
      else
        num = 0;
      if (num != 0)
      {
        if (dataContext.Status == Status.Progress)
          Logger.Info("Compatibility check is running");
        else if (dataContext.IsDirty())
        {
          CustomMessageWindow customMessageWindow1 = new CustomMessageWindow();
          customMessageWindow1.Owner = dataContext.Owner;
          customMessageWindow1.WindowStartupLocation = WindowStartupLocation.CenterOwner;
          CustomMessageWindow customMessageWindow2 = customMessageWindow1;
          BlueStacksUIBinding.Bind(customMessageWindow2.TitleTextBlock, "STRING_DISCARD_CHANGES", "");
          BlueStacksUIBinding.Bind(customMessageWindow2.BodyTextBlock, "STRING_SETTING_TAB_CHANGE_MESSAGE", "");
          customMessageWindow2.AddButton(ButtonColors.Blue, "STRING_NO", (EventHandler) ((o, e) => args.Handled = true), (string) null, false, (object) null, true);
          customMessageWindow2.AddButton(ButtonColors.White, "STRING_DISCARD_CHANGES", (EventHandler) ((o, e) => this.SettingsBtn_Click(sender, (RoutedEventArgs) null)), (string) null, false, (object) null, true);
          customMessageWindow2.ShowDialog();
        }
        else
          this.SettingsBtn_Click(sender, (RoutedEventArgs) null);
      }
      else
      {
        InstanceDisplaySettings displaySetting = this.visibleControl as InstanceDisplaySettings;
        if (displaySetting != null && displaySetting.IsDirty())
        {
          CustomMessageWindow customMessageWindow1 = new CustomMessageWindow();
          customMessageWindow1.Owner = displaySetting.Window;
          customMessageWindow1.WindowStartupLocation = WindowStartupLocation.CenterOwner;
          CustomMessageWindow customMessageWindow2 = customMessageWindow1;
          BlueStacksUIBinding.Bind(customMessageWindow2.TitleTextBlock, "STRING_DISCARD_CHANGES", "");
          BlueStacksUIBinding.Bind(customMessageWindow2.BodyTextBlock, "STRING_SETTING_TAB_CHANGE_MESSAGE", "");
          customMessageWindow2.AddButton(ButtonColors.Blue, "STRING_NO", (EventHandler) ((o, e) => args.Handled = true), (string) null, false, (object) null, true);
          customMessageWindow2.AddButton(ButtonColors.White, "STRING_DISCARD_CHANGES", (EventHandler) ((o, e) =>
          {
            displaySetting.DiscardCurrentChangingModel();
            this.SettingsBtn_Click(sender, (RoutedEventArgs) null);
          }), (string) null, false, (object) null, true);
          customMessageWindow2.ShowDialog();
        }
        else
          this.SettingsBtn_Click(sender, (RoutedEventArgs) null);
      }
    }

    public override void CloseButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      Logger.Info("Clicked settings menu close button");
      bool flag = false;
      UserControl visibleControl = this.visibleControl;
      if (!(visibleControl is EngineSettingBase engineSettingBase))
      {
        if (visibleControl is InstanceDisplaySettings instanceDisplaySettings)
          flag = instanceDisplaySettings.IsDirty();
      }
      else
      {
        EngineSettingViewModel dataContext = engineSettingBase.DataContext as EngineSettingViewModel;
        if (dataContext.Status == Status.Progress)
        {
          Logger.Info("Compatibility check is running");
          return;
        }
        flag = dataContext.IsDirty();
      }
      if (flag)
      {
        CustomMessageWindow customMessageWindow = new CustomMessageWindow();
        BlueStacksUIBinding.Bind(customMessageWindow.TitleTextBlock, "STRING_DISCARD_CHANGES", "");
        BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlock, LocaleStrings.GetLocalizedString("STRING_SETTING_CLOSE_MESSAGE", ""), "");
        customMessageWindow.AddButton(ButtonColors.Blue, "STRING_NO", (EventHandler) ((o, evt) => {}), (string) null, false, (object) null, true);
        customMessageWindow.AddButton(ButtonColors.White, "STRING_DISCARD_CHANGES", (EventHandler) ((o, evt) =>
        {
          if (this.ParentWindow == null)
            return;
          this.ParentWindow.Close();
        }), (string) null, false, (object) null, true);
        customMessageWindow.Owner = this.ParentWindow;
        customMessageWindow.ShowDialog();
      }
      else
      {
        if (this.ParentWindow == null)
          return;
        this.ParentWindow.Close();
      }
    }
  }
}
