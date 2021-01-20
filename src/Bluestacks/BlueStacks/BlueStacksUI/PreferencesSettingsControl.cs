// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.PreferencesSettingsControl
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using BlueStacks.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace BlueStacks.BlueStacksUI
{
  public class PreferencesSettingsControl : UserControl, IComponentConnector, IStyleConnector
  {
    private Dictionary<string, ComboBoxItem> dictComboBoxItems = new Dictionary<string, ComboBoxItem>();
    private NativeGamepadState currentNativeGamePadState = NativeGamepadState.Auto;
    private ObservableCollection<RearrangeSidebarItem> mRearrangeSidebarItems = new ObservableCollection<RearrangeSidebarItem>();
    private List<SidebarElement> mCurrentVisibleSidebarElements = new List<SidebarElement>();
    private CustomToastPopupControl mToastPopup;
    private MainWindow ParentWindow;
    private bool mAdbEnabled;
    private bool mTouchPointEnabled;
    private bool mTouchPointCoordinateEnabled;
    internal ScrollViewer mScrollBar;
    internal Grid mMainGrid;
    internal Grid mLanguageSettingsGrid;
    internal CustomComboBox mLanguageCombobox;
    internal Grid mLanguagePreferencePaddingGrid;
    internal Grid mPerformancePreference;
    internal Label mPerformanceSettingsLabel;
    internal CustomCheckbox mSwitchToHome;
    internal CustomCheckbox mSwitchKillWebTab;
    internal CustomCheckbox mEnableMemoryTrim;
    internal TextBlock mEnableMemoryTrimWarning;
    internal Grid mGameControlPreferencePaddingGrid;
    internal Grid mGameControlsSettingsGrid;
    internal Label mGameControlSettingsLabel;
    internal StackPanel mGameControlsStackPanel;
    internal CustomCheckbox mEnableGamePadCheckbox;
    internal CustomPictureBox mHelpIcon;
    internal Grid mEnableNativeGamepad;
    internal CustomRadioButton mForcedOnMode;
    internal CustomRadioButton mForcedOffMode;
    internal CustomRadioButton mAutoMode;
    internal CustomCheckbox mShowSchemeDeleteWarning;
    internal Grid mPerformancePreferencePaddingGrid;
    internal StackPanel mPlatformStackPanel;
    internal CustomCheckbox mAddDesktopShortcuts;
    internal CustomCheckbox mShowGamingSummary;
    internal CustomCheckbox mEnableTouchSound;
    internal CustomPictureBox mTouchSoundHelpIcon;
    internal CustomCheckbox mShowBlueHighlighter;
    internal CustomCheckbox mShowMacroDeleteWarning;
    internal CustomCheckbox mDiscordCheckBox;
    internal CustomCheckbox mEnableAdbCheckBox;
    internal TextBlock mEnableAdbWarning;
    internal Grid mInputGrid;
    internal CustomCheckbox mEnableTouchPointsCheckBox;
    internal CustomCheckbox mEnableTouchCoordinatesCheckbox;
    internal TextBlock mEnableDevSettingsWarning;
    internal Grid mQuitOptionsGrid;
    internal CustomComboBox mQuitOptionsComboBox;
    internal CustomCheckbox mShowOnExitCheckbox;
    internal StackPanel mScreenshotGrid;
    internal TextBlock mScreenShotPathLable;
    internal CustomButton mChangePathBtn;
    internal CustomCheckbox mAccessWidowsFolderCheckbox;
    internal ListBox mRearrangeListBox;
    internal CustomButton mResetOrder;
    internal CustomButton nSaveBtn;
    private bool _contentLoaded;

    public PreferencesSettingsControl(MainWindow window)
    {
      this.InitializeComponent();
      this.ParentWindow = window;
      this.mCurrentVisibleSidebarElements = this.ParentWindow.mSidebar.mListSidebarElements.Where<SidebarElement>((Func<SidebarElement, bool>) (ele => ele.Visibility == Visibility.Visible)).ToList<SidebarElement>();
      this.Visibility = Visibility.Hidden;
      this.AddLanguages();
      this.Reset();
      this.mScrollBar.ScrollChanged += new ScrollChangedEventHandler(BluestacksUIColor.ScrollBarScrollChanged);
    }

    private void Reset()
    {
      this.InitSettings();
      this.SelectDefaultLanguage();
      this.AddQuitOptions();
      if (!FeatureManager.Instance.IsShowLanguagePreference)
      {
        this.mLanguageSettingsGrid.Visibility = Visibility.Collapsed;
        this.mLanguagePreferencePaddingGrid.Visibility = Visibility.Collapsed;
      }
      if (!FeatureManager.Instance.IsShowDesktopShortcutPreference)
        this.mAddDesktopShortcuts.Visibility = Visibility.Collapsed;
      if (!FeatureManager.Instance.IsShowGamingSummaryPreference)
        this.mShowGamingSummary.Visibility = Visibility.Collapsed;
      if (!FeatureManager.Instance.IsShowPerformancePreference)
      {
        this.mPerformancePreference.Visibility = Visibility.Collapsed;
        this.mPerformancePreferencePaddingGrid.Visibility = Visibility.Collapsed;
      }
      if (!FeatureManager.Instance.IsShowDiscordPreference)
        this.mDiscordCheckBox.Visibility = Visibility.Collapsed;
      if (!FeatureManager.Instance.IsCustomUIForNCSoft)
        this.mQuitOptionsGrid.Visibility = Visibility.Collapsed;
      this.PopulateSidebarIcons(RegistryManager.Instance.UserDefinedSidebarElements.Length == 0 ? RegistryManager.Instance.DefaultSidebarElements : RegistryManager.Instance.UserDefinedSidebarElements);
      this.mRearrangeListBox.ItemsSource = (IEnumerable) this.mRearrangeSidebarItems;
      this.mResetOrder.IsEnabled = this.IsResetOrderButtonEnabled();
      this.nSaveBtn.IsEnabled = this.IsDirty();
    }

    private void AddQuitOptions()
    {
      this.mQuitOptionsComboBox.Items?.Clear();
      ComboBoxItem comboBoxItem1 = (ComboBoxItem) null;
      foreach (string exitOption in LocaleStringsConstants.ExitOptions)
      {
        ComboBoxItem comboBoxItem2 = new ComboBoxItem();
        comboBoxItem2.Content = (object) LocaleStrings.GetLocalizedString(exitOption, "");
        comboBoxItem2.Tag = (object) exitOption;
        ComboBoxItem comboBoxItem3 = comboBoxItem2;
        this.mQuitOptionsComboBox.Items.Add((object) comboBoxItem3);
        if (exitOption == RegistryManager.Instance.QuitDefaultOption)
          comboBoxItem1 = comboBoxItem3;
      }
      foreach (string restartOption in LocaleStringsConstants.RestartOptions)
      {
        ComboBoxItem comboBoxItem2 = new ComboBoxItem();
        comboBoxItem2.Content = (object) LocaleStrings.GetLocalizedString(restartOption, "");
        ComboBoxItem comboBoxItem3 = comboBoxItem2;
        this.mQuitOptionsComboBox.Items.Add((object) comboBoxItem3);
        comboBoxItem3.Tag = (object) restartOption;
        if (restartOption == RegistryManager.Instance.QuitDefaultOption)
          comboBoxItem1 = comboBoxItem3;
      }
      if (comboBoxItem1 == null)
        this.mQuitOptionsComboBox.SelectedIndex = 0;
      else
        this.mQuitOptionsComboBox.SelectedItem = (object) comboBoxItem1;
    }

    private void AddLanguages()
    {
      foreach (string key in BlueStacks.Common.Globalization.sSupportedLocales.Keys)
      {
        ComboBoxItem comboBoxItem1 = new ComboBoxItem();
        comboBoxItem1.Content = (object) BlueStacks.Common.Globalization.sSupportedLocales[key].ToString((IFormatProvider) CultureInfo.InvariantCulture);
        ComboBoxItem comboBoxItem2 = comboBoxItem1;
        this.dictComboBoxItems.Add(comboBoxItem2.Content.ToString(), comboBoxItem2);
        this.mLanguageCombobox.Items.Add((object) comboBoxItem2);
      }
    }

    private void SelectDefaultLanguage()
    {
      this.mLanguageCombobox.SelectionChanged -= new SelectionChangedEventHandler(this.mLanguageCombobox_SelectionChanged);
      string key = RegistryManager.Instance.UserSelectedLocale;
      if (string.IsNullOrEmpty(key))
      {
        key = LocaleStrings.GetLocaleName("Android", false);
        RegistryManager.Instance.UserSelectedLocale = key;
      }
      else if (!BlueStacks.Common.Globalization.sSupportedLocales.ContainsKey(key))
      {
        string locale = key;
        key = "en-US";
        string str = BlueStacks.Common.Globalization.sSupportedLocales.Keys.FirstOrDefault<string>((Func<string, bool>) (x => x.StartsWith(locale.Substring(0, 2), StringComparison.InvariantCulture)));
        if (!string.IsNullOrEmpty(str))
          key = str;
      }
      this.mLanguageCombobox.SelectedItem = (object) this.dictComboBoxItems[BlueStacks.Common.Globalization.sSupportedLocales[key].ToString((IFormatProvider) CultureInfo.InvariantCulture)];
      this.mLanguageCombobox.SelectionChanged += new SelectionChangedEventHandler(this.mLanguageCombobox_SelectionChanged);
    }

    private void InitSettings()
    {
      if (!this.ParentWindow.IsDefaultVM)
        this.mAddDesktopShortcuts.Visibility = Visibility.Collapsed;
      this.mAddDesktopShortcuts.IsChecked = new bool?(RegistryManager.Instance.AddDesktopShortcuts);
      this.mSwitchToHome.IsChecked = new bool?(RegistryManager.Instance.SwitchToAndroidHome);
      this.mSwitchKillWebTab.IsChecked = new bool?(RegistryManager.Instance.SwitchKillWebTab);
      this.mEnableMemoryTrim.IsChecked = new bool?(RegistryManager.Instance.EnableMemoryTrim);
      this.mShowGamingSummary.IsChecked = new bool?(RegistryManager.Instance.ShowGamingSummary);
      this.mShowMacroDeleteWarning.IsChecked = new bool?(this.ParentWindow.EngineInstanceRegistry.ShowMacroDeletePopup);
      this.mShowSchemeDeleteWarning.IsChecked = new bool?(this.ParentWindow.EngineInstanceRegistry.ShowSchemeDeletePopup);
      this.mDiscordCheckBox.IsChecked = new bool?(RegistryManager.Instance.DiscordEnabled);
      this.mEnableGamePadCheckbox.IsChecked = new bool?(RegistryManager.Instance.GamepadDetectionEnabled);
      this.mShowBlueHighlighter.IsChecked = new bool?(this.ParentWindow.EngineInstanceRegistry.ShowBlueHighlighter);
      this.mEnableTouchSound.IsChecked = new bool?(this.ParentWindow.EngineInstanceRegistry.TouchSoundEnabled);
      if (FeatureManager.Instance.IsShowTouchSoundSetting && this.ParentWindow.mGuestBootCompleted)
        this.mEnableTouchSound.Visibility = Visibility.Visible;
      else
        this.mEnableTouchSound.Visibility = Visibility.Collapsed;
      if (!FeatureManager.Instance.IsMacroRecorderEnabled)
        this.mShowMacroDeleteWarning.Visibility = Visibility.Collapsed;
      if (PromotionObject.Instance == null || string.IsNullOrEmpty(PromotionObject.Instance.DiscordClientID) || !this.ParentWindow.IsDefaultVM)
        this.mDiscordCheckBox.Visibility = Visibility.Collapsed;
      this.InitNativeGamepadSettings();
      this.CheckIfAdbIsEnabled();
      this.CheckIfAndroidTouchPointsEnabled();
      Utils.ValidateScreenShotFolder();
      this.mScreenShotPathLable.Text = RegistryManager.Instance.ScreenShotsPath;
      this.mAccessWidowsFolderCheckbox.IsChecked = new bool?(RegistryManager.Instance.Guest[this.ParentWindow.mVmName].CanAccessWindowsFolder);
      if (!RegistryManager.Instance.Guest[this.ParentWindow.mVmName].IsGoogleSigninDone)
      {
        this.mLanguageSettingsGrid.Visibility = Visibility.Collapsed;
        this.mLanguagePreferencePaddingGrid.Visibility = Visibility.Collapsed;
      }
      this.mShowOnExitCheckbox.IsChecked = new bool?(!RegistryManager.Instance.IsQuitOptionSaved);
    }

    private void InitNativeGamepadSettings()
    {
      this.currentNativeGamePadState = this.ParentWindow.EngineInstanceRegistry.NativeGamepadState;
      switch (this.currentNativeGamePadState)
      {
        case NativeGamepadState.ForceOn:
          this.mForcedOnMode.IsChecked = new bool?(true);
          break;
        case NativeGamepadState.ForceOff:
          this.mForcedOffMode.IsChecked = new bool?(true);
          break;
        case NativeGamepadState.Auto:
          this.mAutoMode.IsChecked = new bool?(true);
          break;
      }
    }

    private void CheckIfAdbIsEnabled()
    {
      new Thread((ThreadStart) (() =>
      {
        try
        {
          bool result = string.Equals("ok", (JsonConvert.DeserializeObject(HTTPUtils.SendRequestToGuest("checkADBStatus", (Dictionary<string, string>) null, this.ParentWindow.mVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp"), Utils.GetSerializerSettings()) as JObject)["result"].Value<string>(), StringComparison.OrdinalIgnoreCase);
          this.mAdbEnabled = result;
          this.Dispatcher.Invoke((Delegate) (() =>
          {
            this.mEnableAdbWarning.Text = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} {1}", (object) LocaleStrings.GetLocalizedString("STRING_ENABLE_ADB_WARNING", ""), (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_ADB_CONNECTED_PORT_0", ""), (object) this.ParentWindow.EngineInstanceRegistry.BstAdbPort));
            this.mEnableAdbCheckBox.IsChecked = new bool?(result);
            if (this.ParentWindow.mGuestBootCompleted && FeatureManager.Instance.AllowADBSettingToggle)
            {
              this.mEnableAdbCheckBox.Visibility = Visibility.Visible;
              this.mEnableAdbWarning.Visibility = Visibility.Visible;
            }
            else
            {
              this.mEnableAdbCheckBox.Visibility = Visibility.Collapsed;
              this.mEnableAdbWarning.Visibility = Visibility.Collapsed;
            }
            this.nSaveBtn.IsEnabled = this.IsDirty();
          }));
        }
        catch (Exception ex)
        {
          Logger.Error("Exception when initialising adb checkbox " + ex.ToString());
        }
      })).Start();
    }

    private void CheckIfAndroidTouchPointsEnabled()
    {
      new Thread((ThreadStart) (() =>
      {
        try
        {
          JObject jobject = JObject.Parse(HTTPUtils.SendRequestToGuest("checkTouchPointState", (Dictionary<string, string>) null, this.ParentWindow.mVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp"));
          bool touchPointSetting = string.Equals("enabled", jobject["touchPoint"].ToString().Trim(), StringComparison.InvariantCultureIgnoreCase);
          bool pointerLocationSetting = string.Equals("enabled", jobject["pointerLocation"].ToString().Trim(), StringComparison.InvariantCultureIgnoreCase);
          this.mTouchPointEnabled = touchPointSetting;
          this.mTouchPointCoordinateEnabled = pointerLocationSetting;
          this.Dispatcher.Invoke((Delegate) (() =>
          {
            this.mEnableTouchPointsCheckBox.IsChecked = new bool?(touchPointSetting);
            this.mEnableTouchCoordinatesCheckbox.IsChecked = new bool?(pointerLocationSetting);
            this.mInputGrid.Visibility = !FeatureManager.Instance.IsShowAndroidInputDebugSetting || !this.ParentWindow.mGuestBootCompleted ? Visibility.Collapsed : Visibility.Visible;
            this.nSaveBtn.IsEnabled = this.IsDirty();
          }));
        }
        catch (Exception ex)
        {
          Logger.Error("Exception when initialising android input debugging checkbox " + ex.ToString());
        }
      })).Start();
    }

    private void CheckBox_Click(object sender, RoutedEventArgs e)
    {
      this.nSaveBtn.IsEnabled = this.IsDirty();
    }

    private void mLanguageCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      this.nSaveBtn.IsEnabled = this.IsDirty();
    }

    private void mSwitchToHome_Click(object sender, RoutedEventArgs e)
    {
      this.nSaveBtn.IsEnabled = this.IsDirty();
    }

    private void SwitchKillWebTab_Click(object sender, RoutedEventArgs e)
    {
      this.nSaveBtn.IsEnabled = this.IsDirty();
    }

    private void mChangePathBtn_Click(object sender, RoutedEventArgs e)
    {
      this.ParentWindow.mCommonHandler.ShowFolderBrowserDialog(this.mScreenShotPathLable.Text);
      this.nSaveBtn.IsEnabled = this.IsDirty();
      this.mScreenShotPathLable.Text = RegistryManager.Instance.ScreenShotsPath;
    }

    private void MQuitOptionsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      this.nSaveBtn.IsEnabled = this.IsDirty();
    }

    private void MShowOnExitCheckbox_Click(object sender, RoutedEventArgs e)
    {
      this.nSaveBtn.IsEnabled = this.IsDirty();
    }

    private void HelpIconPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      Utils.OpenUrl(WebHelper.GetUrlWithParams(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) WebHelper.GetServerHost(), (object) "help_articles"), (string) null, (string) null, (string) null) + "&article=native_gamepad_help");
    }

    private void NativeGamepadMode_Click(object sender, RoutedEventArgs e)
    {
      switch ((sender as CustomRadioButton).Name)
      {
        case "mForcedOnMode":
          this.currentNativeGamePadState = NativeGamepadState.ForceOn;
          break;
        case "mForcedOffMode":
          this.currentNativeGamePadState = NativeGamepadState.ForceOff;
          break;
        case "mAutoMode":
          this.currentNativeGamePadState = NativeGamepadState.Auto;
          break;
      }
      this.nSaveBtn.IsEnabled = this.IsDirty();
    }

    private void EnableMemoryTrim_Click(object sender, RoutedEventArgs e)
    {
      this.nSaveBtn.IsEnabled = this.IsDirty();
    }

    public bool IsDirty()
    {
      bool flag = false;
      try
      {
        if (this.mLanguageCombobox.SelectedItem is ComboBoxItem selectedItem)
        {
          string selectedLocale = selectedItem.Content as string;
          if (selectedLocale != null)
          {
            if (!string.Equals(RegistryManager.Instance.UserSelectedLocale, BlueStacks.Common.Globalization.sSupportedLocales.FirstOrDefault<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (x => x.Value == selectedLocale)).Key, StringComparison.InvariantCultureIgnoreCase))
              flag = true;
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in set locale" + ex.ToString());
      }
      try
      {
        if (this.mQuitOptionsComboBox.SelectedItem is ComboBoxItem selectedItem)
        {
          if (selectedItem.Tag is string tag)
          {
            if (!string.Equals(RegistryManager.Instance.QuitDefaultOption, tag, StringComparison.InvariantCultureIgnoreCase))
              flag = true;
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in set locale" + ex.ToString());
      }
      if (!flag)
      {
        int num1 = RegistryManager.Instance.SwitchToAndroidHome ? 1 : 0;
        bool? nullable = this.mSwitchToHome?.IsChecked.GetValueOrDefault();
        int num2 = nullable.GetValueOrDefault() ? 1 : 0;
        if (num1 == num2 & nullable.HasValue)
        {
          int num3 = RegistryManager.Instance.SwitchKillWebTab ? 1 : 0;
          nullable = this.mSwitchKillWebTab?.IsChecked.GetValueOrDefault();
          int num4 = nullable.GetValueOrDefault() ? 1 : 0;
          if (num3 == num4 & nullable.HasValue)
          {
            int num5 = RegistryManager.Instance.EnableMemoryTrim ? 1 : 0;
            nullable = this.mEnableMemoryTrim?.IsChecked.GetValueOrDefault();
            int num6 = nullable.GetValueOrDefault() ? 1 : 0;
            if (num5 == num6 & nullable.HasValue)
            {
              int num7 = RegistryManager.Instance.GamepadDetectionEnabled ? 1 : 0;
              nullable = this.mEnableGamePadCheckbox?.IsChecked.GetValueOrDefault();
              int num8 = nullable.GetValueOrDefault() ? 1 : 0;
              if (num7 == num8 & nullable.HasValue && RegistryManager.Instance.Guest[this.ParentWindow.mVmName].NativeGamepadState == this.currentNativeGamePadState)
              {
                int num9 = RegistryManager.Instance.Guest[this.ParentWindow.mVmName].ShowSchemeDeletePopup ? 1 : 0;
                nullable = this.mShowSchemeDeleteWarning?.IsChecked.GetValueOrDefault();
                int num10 = nullable.GetValueOrDefault() ? 1 : 0;
                if (num9 == num10 & nullable.HasValue)
                {
                  int num11 = RegistryManager.Instance.AddDesktopShortcuts ? 1 : 0;
                  nullable = this.mAddDesktopShortcuts?.IsChecked.GetValueOrDefault();
                  int num12 = nullable.GetValueOrDefault() ? 1 : 0;
                  if (num11 == num12 & nullable.HasValue)
                  {
                    int num13 = RegistryManager.Instance.ShowGamingSummary ? 1 : 0;
                    nullable = this.mShowGamingSummary?.IsChecked.GetValueOrDefault();
                    int num14 = nullable.GetValueOrDefault() ? 1 : 0;
                    if (num13 == num14 & nullable.HasValue)
                    {
                      int num15 = RegistryManager.Instance.Guest[this.ParentWindow.mVmName].ShowBlueHighlighter ? 1 : 0;
                      nullable = this.mShowBlueHighlighter.IsChecked;
                      int num16 = nullable.GetValueOrDefault() ? 1 : 0;
                      if (num15 == num16)
                      {
                        int num17 = RegistryManager.Instance.Guest[this.ParentWindow.mVmName].ShowMacroDeletePopup ? 1 : 0;
                        nullable = this.mShowMacroDeleteWarning?.IsChecked.GetValueOrDefault();
                        int num18 = nullable.GetValueOrDefault() ? 1 : 0;
                        if (num17 == num18 & nullable.HasValue)
                        {
                          int num19 = RegistryManager.Instance.DiscordEnabled ? 1 : 0;
                          nullable = this.mDiscordCheckBox?.IsChecked.GetValueOrDefault();
                          int num20 = nullable.GetValueOrDefault() ? 1 : 0;
                          if (num19 == num20 & nullable.HasValue)
                          {
                            int num21 = this.mAdbEnabled ? 1 : 0;
                            nullable = this.mEnableAdbCheckBox?.IsChecked.GetValueOrDefault();
                            int num22 = nullable.GetValueOrDefault() ? 1 : 0;
                            if (num21 == num22 & nullable.HasValue)
                            {
                              int num23 = this.mTouchPointEnabled ? 1 : 0;
                              nullable = this.mEnableTouchPointsCheckBox?.IsChecked.GetValueOrDefault();
                              int num24 = nullable.GetValueOrDefault() ? 1 : 0;
                              if (num23 == num24 & nullable.HasValue)
                              {
                                int num25 = this.mTouchPointCoordinateEnabled ? 1 : 0;
                                nullable = this.mEnableTouchCoordinatesCheckbox?.IsChecked.GetValueOrDefault();
                                int num26 = nullable.GetValueOrDefault() ? 1 : 0;
                                if (num25 == num26 & nullable.HasValue && !(RegistryManager.Instance.ScreenShotsPath != this.mScreenShotPathLable?.Text))
                                {
                                  int num27 = RegistryManager.Instance.Guest[this.ParentWindow.mVmName].CanAccessWindowsFolder ? 1 : 0;
                                  nullable = this.mAccessWidowsFolderCheckbox?.IsChecked.GetValueOrDefault();
                                  int num28 = nullable.GetValueOrDefault() ? 1 : 0;
                                  if (num27 == num28 & nullable.HasValue && !(RegistryManager.Instance.QuitDefaultOption != (this.mQuitOptionsComboBox.SelectedItem is ComboBoxItem selectedItem ? selectedItem.Tag.ToString() : (string) null)))
                                  {
                                    int num29 = RegistryManager.Instance.IsQuitOptionSaved ? 1 : 0;
                                    nullable = this.mShowOnExitCheckbox?.IsChecked.GetValueOrDefault();
                                    int num30 = nullable.GetValueOrDefault() ? 1 : 0;
                                    if (!(num29 == num30 & nullable.HasValue))
                                    {
                                      int num31 = RegistryManager.Instance.Guest[this.ParentWindow.mVmName].TouchSoundEnabled ? 1 : 0;
                                      nullable = this.mEnableTouchSound.IsChecked;
                                      int num32 = nullable.GetValueOrDefault() ? 1 : 0;
                                      if (num31 == num32)
                                        return this.HasSidebarOrderChanged();
                                    }
                                  }
                                }
                              }
                            }
                          }
                        }
                      }
                    }
                  }
                }
              }
            }
          }
        }
      }
      return true;
    }

    private bool HasSidebarOrderChanged()
    {
      string[] strArray = RegistryManager.Instance.UserDefinedSidebarElements.Length == 0 ? RegistryManager.Instance.DefaultSidebarElements : RegistryManager.Instance.UserDefinedSidebarElements;
      List<string> stringList = new List<string>();
      foreach (string str in strArray)
      {
        string sidebarItem = str;
        SidebarElement sidebarElement = this.mCurrentVisibleSidebarElements.Where<SidebarElement>((Func<SidebarElement, bool>) (ele => string.Equals(ele.Tag.ToString(), sidebarItem, StringComparison.InvariantCultureIgnoreCase))).FirstOrDefault<SidebarElement>();
        if (sidebarElement != null && sidebarElement.Visibility == Visibility.Visible)
          stringList.Add(sidebarItem);
      }
      string[] array = this.mRearrangeSidebarItems.OrderBy<RearrangeSidebarItem, int>((Func<RearrangeSidebarItem, int>) (ele => ele.Order)).Select<RearrangeSidebarItem, string>((Func<RearrangeSidebarItem, string>) (ele => ele.ImageName)).ToArray<string>();
      return PreferencesSettingsControl.IsElementOrderDifferent(stringList.ToArray(), array);
    }

    private static bool IsElementOrderDifferent(string[] currentSidebarOrder, string[] newOrder)
    {
      if (currentSidebarOrder.Length != newOrder.Length)
        return true;
      for (int index = 0; index < newOrder.Length; ++index)
      {
        if (!string.Equals(currentSidebarOrder[index], newOrder[index], StringComparison.InvariantCultureIgnoreCase))
          return true;
      }
      return false;
    }

    private bool IsResetOrderButtonEnabled()
    {
      List<string> stringList = new List<string>();
      foreach (string defaultSidebarElement in RegistryManager.Instance.DefaultSidebarElements)
      {
        string sidebarItem = defaultSidebarElement;
        SidebarElement sidebarElement = this.mCurrentVisibleSidebarElements.Where<SidebarElement>((Func<SidebarElement, bool>) (ele => string.Equals(ele.Tag.ToString(), sidebarItem, StringComparison.InvariantCultureIgnoreCase))).FirstOrDefault<SidebarElement>();
        if (sidebarElement != null && sidebarElement.Visibility == Visibility.Visible)
          stringList.Add(sidebarItem);
      }
      string[] array = this.mRearrangeSidebarItems.OrderBy<RearrangeSidebarItem, int>((Func<RearrangeSidebarItem, int>) (ele => ele.Order)).Select<RearrangeSidebarItem, string>((Func<RearrangeSidebarItem, string>) (ele => ele.ImageName)).ToArray<string>();
      return PreferencesSettingsControl.IsElementOrderDifferent(stringList.ToArray(), array);
    }

    private void AccessWidowsFolderCheckbox_Changed(object sender, RoutedEventArgs e)
    {
      this.nSaveBtn.IsEnabled = this.IsDirty();
    }

    public void SaveChanges(object sender, EventArgs e)
    {
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        Logger.Info("PreferencesSettingsControl -> SaveChanges");
        this.SavePreferencesData();
        this.ParentWindow.mFrontendHandler.mEventOnFrontendClosed -= new EventHandler(this.SaveChanges);
      }));
    }

    public void DiscardChanges()
    {
      this.Reset();
    }

    private void SaveBtnClick(object sender, RoutedEventArgs e1)
    {
      if (Oem.IsOEMDmm)
      {
        RegistryManager.Instance.Guest[this.ParentWindow.mVmName].CanAccessWindowsFolder = this.mAccessWidowsFolderCheckbox.IsChecked.GetValueOrDefault();
        BlueStacksUIUtils.CloseContainerWindow((FrameworkElement) this);
        this.ParentWindow.Close();
      }
      else
      {
        int num1 = RegistryManager.Instance.Guest[this.ParentWindow.mVmName].CanAccessWindowsFolder ? 1 : 0;
        bool? isChecked = this.mAccessWidowsFolderCheckbox.IsChecked;
        int num2 = isChecked.GetValueOrDefault() ? 1 : 0;
        if (!(num1 == num2 & isChecked.HasValue))
        {
          CustomMessageWindow customMessageWindow = new CustomMessageWindow();
          customMessageWindow.Owner = (Window) this.ParentWindow;
          customMessageWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
          customMessageWindow.TitleTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_RESTART_BLUESTACKS", "");
          customMessageWindow.BodyTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_RESTART_BLUESTACKS_MESSAGE", "");
          customMessageWindow.AddButton(ButtonColors.Blue, "STRING_RESTART_NOW", (EventHandler) ((o, e2) =>
          {
            if (BlueStacksUIUtils.DictWindows.Count == 1)
            {
              App.defaultResolution = new Fraction((long) RegistryManager.Instance.Guest[BlueStacks.Common.Strings.CurrentDefaultVmName].GuestWidth, (long) RegistryManager.Instance.Guest[BlueStacks.Common.Strings.CurrentDefaultVmName].GuestHeight);
              PromotionManager.ReloadPromotionsAsync();
            }
            BlueStacksUIUtils.CloseContainerWindow((FrameworkElement) this);
            this.ParentWindow.mFrontendHandler.mEventOnFrontendClosed += new EventHandler(this.SaveChanges);
            BlueStacksUIUtils.RestartInstance(this.ParentWindow.mVmName, true);
          }), (string) null, false, (object) null, true);
          customMessageWindow.AddButton(ButtonColors.White, "STRING_DISCARD_CHANGES", (EventHandler) ((o, e2) => this.DiscardChanges()), (string) null, false, (object) null, true);
          customMessageWindow.ShowDialog();
        }
        else
          this.SaveChanges((object) null, (EventArgs) null);
      }
    }

    private void SavePreferencesData()
    {
      try
      {
        if (this.mLanguageCombobox.SelectedItem is ComboBoxItem selectedItem)
        {
          string selectedLocale = selectedItem.Content as string;
          if (selectedLocale != null)
          {
            string key = BlueStacks.Common.Globalization.sSupportedLocales.FirstOrDefault<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (x => x.Value == selectedLocale)).Key;
            if (key != null)
            {
              if (!string.Equals(RegistryManager.Instance.UserSelectedLocale, key, StringComparison.InvariantCultureIgnoreCase))
              {
                RegistryManager.Instance.UserSelectedLocale = key;
                BlueStacksUIUtils.UpdateLocale(key, "");
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in set locale" + ex.ToString());
      }
      int num1 = RegistryManager.Instance.SwitchToAndroidHome ? 1 : 0;
      bool? isChecked = this.mSwitchToHome.IsChecked;
      int num2 = isChecked.GetValueOrDefault() ? 1 : 0;
      if (num1 != num2)
      {
        RegistryManager instance = RegistryManager.Instance;
        isChecked = this.mSwitchToHome.IsChecked;
        int num3 = isChecked.GetValueOrDefault() ? 1 : 0;
        instance.SwitchToAndroidHome = num3 != 0;
      }
      int num4 = RegistryManager.Instance.SwitchKillWebTab ? 1 : 0;
      isChecked = this.mSwitchKillWebTab.IsChecked;
      int num5 = isChecked.GetValueOrDefault() ? 1 : 0;
      if (num4 != num5)
      {
        RegistryManager instance = RegistryManager.Instance;
        isChecked = this.mSwitchKillWebTab.IsChecked;
        int num3 = isChecked.GetValueOrDefault() ? 1 : 0;
        instance.SwitchKillWebTab = num3 != 0;
      }
      int num6 = RegistryManager.Instance.EnableMemoryTrim ? 1 : 0;
      isChecked = this.mEnableMemoryTrim.IsChecked;
      int num7 = isChecked.GetValueOrDefault() ? 1 : 0;
      if (num6 != num7)
      {
        RegistryManager instance = RegistryManager.Instance;
        isChecked = this.mEnableMemoryTrim.IsChecked;
        int num3 = isChecked.GetValueOrDefault() ? 1 : 0;
        instance.EnableMemoryTrim = num3 != 0;
        isChecked = this.mEnableMemoryTrim.IsChecked;
        bool flag = true;
        if (isChecked.GetValueOrDefault() == flag & isChecked.HasValue)
        {
          foreach (string vmName in BlueStacksUIUtils.DictWindows.Keys.ToList<string>())
            HTTPUtils.SendRequestToEngineAsync("enableMemoryTrim", (Dictionary<string, string>) null, vmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
        }
      }
      int num8 = RegistryManager.Instance.GamepadDetectionEnabled ? 1 : 0;
      isChecked = this.mEnableGamePadCheckbox.IsChecked;
      int num9 = isChecked.GetValueOrDefault() ? 1 : 0;
      bool flag1;
      if (num8 != num9)
      {
        isChecked = this.mEnableGamePadCheckbox.IsChecked;
        bool flag2 = true;
        if (isChecked.GetValueOrDefault() == flag2 & isChecked.HasValue)
        {
          RegistryManager.Instance.GamepadDetectionEnabled = true;
          string str = "false";
          switch (this.currentNativeGamePadState)
          {
            case NativeGamepadState.ForceOn:
              str = "true";
              break;
            case NativeGamepadState.ForceOff:
              str = "false";
              break;
            case NativeGamepadState.Auto:
              if (this.ParentWindow.mGuestBootCompleted)
              {
                flag1 = this.ParentWindow.mCommonHandler.CheckNativeGamepadState(this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName);
                str = flag1.ToString((IFormatProvider) CultureInfo.InvariantCulture);
                break;
              }
              break;
          }
          this.ParentWindow.mFrontendHandler.SendFrontendRequestAsync("enableNativeGamepad", new Dictionary<string, string>()
          {
            {
              "isEnabled",
              str
            }
          });
        }
        else
        {
          RegistryManager.Instance.GamepadDetectionEnabled = false;
          this.ParentWindow.mFrontendHandler.SendFrontendRequestAsync("enableNativeGamepad", new Dictionary<string, string>()
          {
            {
              "isEnabled",
              RegistryManager.Instance.GamepadDetectionEnabled.ToString((IFormatProvider) CultureInfo.InvariantCulture)
            }
          });
        }
        Dictionary<string, string> data = new Dictionary<string, string>();
        flag1 = RegistryManager.Instance.GamepadDetectionEnabled;
        data.Add("enable", flag1.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        this.ParentWindow.mFrontendHandler.SendFrontendRequestAsync("enableGamepad", data);
      }
      if (RegistryManager.Instance.Guest[this.ParentWindow.mVmName].NativeGamepadState != this.currentNativeGamePadState)
      {
        RegistryManager.Instance.Guest[this.ParentWindow.mVmName].NativeGamepadState = this.currentNativeGamePadState;
        string str = string.Empty;
        switch (this.currentNativeGamePadState)
        {
          case NativeGamepadState.ForceOn:
            flag1 = RegistryManager.Instance.GamepadDetectionEnabled;
            str = flag1.ToString((IFormatProvider) CultureInfo.InvariantCulture);
            break;
          case NativeGamepadState.ForceOff:
            str = "false";
            break;
          case NativeGamepadState.Auto:
            if (this.ParentWindow.mGuestBootCompleted)
            {
              flag1 = this.ParentWindow.mCommonHandler.CheckNativeGamepadState(this.ParentWindow.mTopBar.mAppTabButtons.SelectedTab.PackageName);
              str = flag1.ToString((IFormatProvider) CultureInfo.InvariantCulture);
              break;
            }
            break;
        }
        ClientStats.SendMiscellaneousStatsAsync("GamepadModeChanged", RegistryManager.Instance.UserGuid, this.currentNativeGamePadState.ToString(), "SettingsWindow", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
        this.ParentWindow.mFrontendHandler.SendFrontendRequestAsync("enableNativeGamepad", new Dictionary<string, string>()
        {
          {
            "isEnabled",
            str
          }
        });
      }
      int num10 = RegistryManager.Instance.Guest[this.ParentWindow.mVmName].ShowSchemeDeletePopup ? 1 : 0;
      isChecked = this.mShowSchemeDeleteWarning.IsChecked;
      int num11 = isChecked.GetValueOrDefault() ? 1 : 0;
      if (num10 != num11)
      {
        InstanceRegistry instanceRegistry = RegistryManager.Instance.Guest[this.ParentWindow.mVmName];
        isChecked = this.mShowSchemeDeleteWarning.IsChecked;
        int num3 = isChecked.GetValueOrDefault() ? 1 : 0;
        instanceRegistry.ShowSchemeDeletePopup = num3 != 0;
      }
      int num12 = RegistryManager.Instance.AddDesktopShortcuts ? 1 : 0;
      isChecked = this.mAddDesktopShortcuts.IsChecked;
      int num13 = isChecked.GetValueOrDefault() ? 1 : 0;
      if (num12 != num13)
      {
        RegistryManager instance = RegistryManager.Instance;
        isChecked = this.mAddDesktopShortcuts.IsChecked;
        int num3 = isChecked.GetValueOrDefault() ? 1 : 0;
        instance.AddDesktopShortcuts = num3 != 0;
      }
      int num14 = RegistryManager.Instance.ShowGamingSummary ? 1 : 0;
      isChecked = this.mShowGamingSummary.IsChecked;
      int num15 = isChecked.GetValueOrDefault() ? 1 : 0;
      if (num14 != num15)
      {
        RegistryManager instance = RegistryManager.Instance;
        isChecked = this.mShowGamingSummary.IsChecked;
        int num3 = isChecked.GetValueOrDefault() ? 1 : 0;
        instance.ShowGamingSummary = num3 != 0;
        string userGuid = RegistryManager.Instance.UserGuid;
        string clientVersion = RegistryManager.Instance.ClientVersion;
        isChecked = this.mShowGamingSummary.IsChecked;
        string str = "checked" + isChecked.ToString();
        ClientStats.SendMiscellaneousStatsAsync("gamingSummaryCheckboxClicked", userGuid, clientVersion, str, (string) null, (string) null, (string) null, (string) null, (string) null, "Android");
      }
      int num16 = RegistryManager.Instance.Guest[this.ParentWindow.mVmName].ShowBlueHighlighter ? 1 : 0;
      isChecked = this.mShowBlueHighlighter.IsChecked;
      int num17 = isChecked.GetValueOrDefault() ? 1 : 0;
      if (num16 != num17)
      {
        InstanceRegistry instanceRegistry = RegistryManager.Instance.Guest[this.ParentWindow.mVmName];
        isChecked = this.mShowBlueHighlighter.IsChecked;
        int num3 = isChecked.GetValueOrDefault() ? 1 : 0;
        instanceRegistry.ShowBlueHighlighter = num3 != 0;
        string userGuid = RegistryManager.Instance.UserGuid;
        string clientVersion = RegistryManager.Instance.ClientVersion;
        isChecked = this.mShowBlueHighlighter.IsChecked;
        string str = "checked" + isChecked.ToString();
        ClientStats.SendMiscellaneousStatsAsync("bluehighlightercheckboxclicked", userGuid, clientVersion, str, (string) null, (string) null, (string) null, (string) null, (string) null, "Android");
      }
      int num18 = RegistryManager.Instance.Guest[this.ParentWindow.mVmName].TouchSoundEnabled ? 1 : 0;
      isChecked = this.mEnableTouchSound.IsChecked;
      int num19 = isChecked.GetValueOrDefault() ? 1 : 0;
      if (num18 != num19)
      {
        InstanceRegistry instanceRegistry = RegistryManager.Instance.Guest[this.ParentWindow.mVmName];
        isChecked = this.mEnableTouchSound.IsChecked;
        int num3 = isChecked.GetValueOrDefault() ? 1 : 0;
        instanceRegistry.TouchSoundEnabled = num3 != 0;
        this.ParentWindow.mCommonHandler.SetTouchSoundSettings();
      }
      int num20 = RegistryManager.Instance.Guest[this.ParentWindow.mVmName].ShowMacroDeletePopup ? 1 : 0;
      isChecked = this.mShowMacroDeleteWarning.IsChecked;
      int num21 = isChecked.GetValueOrDefault() ? 1 : 0;
      if (num20 != num21)
      {
        InstanceRegistry instanceRegistry = RegistryManager.Instance.Guest[this.ParentWindow.mVmName];
        isChecked = this.mShowMacroDeleteWarning.IsChecked;
        int num3 = isChecked.GetValueOrDefault() ? 1 : 0;
        instanceRegistry.ShowMacroDeletePopup = num3 != 0;
      }
      int num22 = RegistryManager.Instance.DiscordEnabled ? 1 : 0;
      isChecked = this.mDiscordCheckBox.IsChecked;
      int num23 = isChecked.GetValueOrDefault() ? 1 : 0;
      if (num22 != num23)
      {
        isChecked = this.mDiscordCheckBox.IsChecked;
        flag1 = true;
        if (isChecked.GetValueOrDefault() == flag1 & isChecked.HasValue)
        {
          RegistryManager.Instance.DiscordEnabled = true;
          if (this.ParentWindow.mAppHandler.IsOneTimeSetupCompleted && this.ParentWindow.mGuestBootCompleted)
          {
            if (this.ParentWindow.mDiscordhandler == null)
              this.ParentWindow.InitDiscord();
            else
              this.ParentWindow.mDiscordhandler.ToggleDiscordState(true);
          }
        }
        else
        {
          RegistryManager.Instance.DiscordEnabled = false;
          if (this.ParentWindow.mDiscordhandler != null)
            this.ParentWindow.mDiscordhandler.ToggleDiscordState(false);
          this.ParentWindow.mDiscordhandler = (Discord) null;
        }
      }
      int num24 = this.mAdbEnabled ? 1 : 0;
      isChecked = this.mEnableAdbCheckBox.IsChecked;
      int num25 = isChecked.GetValueOrDefault() ? 1 : 0;
      if (num24 != num25)
      {
        isChecked = this.mEnableAdbCheckBox.IsChecked;
        flag1 = true;
        HTTPUtils.SendRequestToGuestAsync(isChecked.GetValueOrDefault() == flag1 & isChecked.HasValue ? "connectHost?d=permanent" : "disconnectHost?d=permanent", (Dictionary<string, string>) null, this.ParentWindow.mVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
        isChecked = this.mEnableAdbCheckBox.IsChecked;
        flag1 = true;
        if (isChecked.GetValueOrDefault() == flag1 & isChecked.HasValue && SecurityMetrics.SecurityMetricsInstanceList.ContainsKey(this.ParentWindow.mVmName))
          SecurityMetrics.SecurityMetricsInstanceList[this.ParentWindow.mVmName].AddSecurityBreach(SecurityBreach.DEVICE_PROBED, "");
        isChecked = this.mEnableAdbCheckBox.IsChecked;
        this.mAdbEnabled = isChecked.GetValueOrDefault();
      }
      int num26 = this.mTouchPointEnabled ? 1 : 0;
      isChecked = this.mEnableTouchPointsCheckBox.IsChecked;
      int num27 = isChecked.GetValueOrDefault() ? 1 : 0;
      bool flag3 = num26 != num27;
      int num28 = this.mTouchPointCoordinateEnabled ? 1 : 0;
      isChecked = this.mEnableTouchCoordinatesCheckbox.IsChecked;
      int num29 = isChecked.GetValueOrDefault() ? 1 : 0;
      bool flag4 = num28 != num29;
      if (flag3 | flag4)
      {
        isChecked = this.mEnableTouchPointsCheckBox.IsChecked;
        flag1 = true;
        string str1 = isChecked.GetValueOrDefault() == flag1 & isChecked.HasValue ? "enable" : "disable";
        isChecked = this.mEnableTouchCoordinatesCheckbox.IsChecked;
        flag1 = true;
        string str2 = isChecked.GetValueOrDefault() == flag1 & isChecked.HasValue ? "enable" : "disable";
        HTTPUtils.SendRequestToGuestAsync("showTouchPoints", new Dictionary<string, string>()
        {
          {
            "data",
            "{" + string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\"touchPoint\":\"{0}\",", (object) str1) + string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\"pointerLocation\":\"{0}\"", (object) str2) + "}"
          }
        }, this.ParentWindow.mVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
        isChecked = this.mEnableTouchPointsCheckBox.IsChecked;
        this.mTouchPointEnabled = isChecked.GetValueOrDefault();
        isChecked = this.mEnableTouchCoordinatesCheckbox.IsChecked;
        this.mTouchPointCoordinateEnabled = isChecked.GetValueOrDefault();
        if (flag3)
          this.SendAndroidInputDebuggingStats(this.mTouchPointEnabled ? "input_debugging_show-visual-feedback-for-taps_checked" : "input_debugging_show-visual-feedback-for-taps_unchecked");
        if (flag4)
          this.SendAndroidInputDebuggingStats(this.mTouchPointCoordinateEnabled ? "input_debugging_show-pointer-location_checked" : "input_debugging_show-pointer-location_unchecked");
      }
      if (RegistryManager.Instance.ScreenShotsPath != this.mScreenShotPathLable.Text)
      {
        ClientStats.SendMiscellaneousStatsAsync("MediaFilesPathSet", RegistryManager.Instance.UserGuid, "PathChangeFromPreferences", RegistryManager.Instance.ScreenShotsPath, this.mScreenShotPathLable.Text, RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, "Android");
        RegistryManager.Instance.ScreenShotsPath = this.mScreenShotPathLable.Text;
      }
      int num30 = RegistryManager.Instance.Guest[this.ParentWindow.mVmName].CanAccessWindowsFolder ? 1 : 0;
      isChecked = this.mAccessWidowsFolderCheckbox.IsChecked;
      int num31 = isChecked.GetValueOrDefault() ? 1 : 0;
      if (!(num30 == num31 & isChecked.HasValue))
      {
        InstanceRegistry instanceRegistry = RegistryManager.Instance.Guest[this.ParentWindow.mVmName];
        isChecked = this.mAccessWidowsFolderCheckbox.IsChecked;
        int num3 = isChecked.GetValueOrDefault() ? 1 : 0;
        instanceRegistry.CanAccessWindowsFolder = num3 != 0;
        Utils.UpdateValueInBootParams("SF", RegistryManager.Instance.Guest[this.ParentWindow.mVmName].CanAccessWindowsFolder ? "Documents,Pictures,InputMapper,BstSharedFolder" : "InputMapper,BstSharedFolder", this.ParentWindow.mVmName, true, "bgp");
        HTTPUtils.SendRequestToAgent("resetSharedFolders", new Dictionary<string, string>()
        {
          ["vmname"] = this.ParentWindow.mVmName
        }, this.ParentWindow.mVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp", true);
        string userGuid = RegistryManager.Instance.UserGuid;
        string clientVersion = RegistryManager.Instance.ClientVersion;
        flag1 = RegistryManager.Instance.Guest[this.ParentWindow.mVmName].CanAccessWindowsFolder;
        string str = "checked " + flag1.ToString();
        ClientStats.SendMiscellaneousStatsAsync("enableAcccessToMediaFilesOnWindowsClicked", userGuid, clientVersion, str, (string) null, (string) null, (string) null, (string) null, (string) null, "Android");
      }
      if (RegistryManager.Instance.QuitDefaultOption != (this.mQuitOptionsComboBox.SelectedItem as ComboBoxItem).Tag.ToString())
        RegistryManager.Instance.QuitDefaultOption = (this.mQuitOptionsComboBox.SelectedItem as ComboBoxItem).Tag.ToString();
      int num32 = RegistryManager.Instance.IsQuitOptionSaved ? 1 : 0;
      isChecked = this.mShowOnExitCheckbox.IsChecked;
      int num33 = isChecked.GetValueOrDefault() ? 1 : 0;
      if (num32 == num33 & isChecked.HasValue)
      {
        RegistryManager instance = RegistryManager.Instance;
        isChecked = this.mShowOnExitCheckbox.IsChecked;
        int num3 = !isChecked.GetValueOrDefault() ? 1 : 0;
        instance.IsQuitOptionSaved = num3 != 0;
      }
      if (this.HasSidebarOrderChanged())
      {
        if (this.mResetOrder.IsEnabled)
        {
          if (this.mRearrangeSidebarItems.Count == RegistryManager.Instance.DefaultSidebarElements.Length)
          {
            RegistryManager.Instance.UserDefinedSidebarElements = this.mRearrangeSidebarItems.OrderBy<RearrangeSidebarItem, int>((Func<RearrangeSidebarItem, int>) (ele => ele.Order)).Select<RearrangeSidebarItem, string>((Func<RearrangeSidebarItem, string>) (ele => ele.ImageName)).ToArray<string>();
          }
          else
          {
            List<string> defaultSidebarElements = ((IEnumerable<string>) (RegistryManager.Instance.UserDefinedSidebarElements.Length != 0 ? RegistryManager.Instance.UserDefinedSidebarElements : RegistryManager.Instance.DefaultSidebarElements)).ToList<string>();
            List<string> userdefinedSidebarElements = this.mRearrangeSidebarItems.OrderBy<RearrangeSidebarItem, int>((Func<RearrangeSidebarItem, int>) (ele => ele.Order)).Select<RearrangeSidebarItem, string>((Func<RearrangeSidebarItem, string>) (ele => ele.ImageName)).ToList<string>();
            defaultSidebarElements.ForEach((System.Action<string>) (element =>
            {
              if (userdefinedSidebarElements.Contains(element))
                return;
              userdefinedSidebarElements.Insert(defaultSidebarElements.IndexOf(element), element);
            }));
            RegistryManager.Instance.UserDefinedSidebarElements = userdefinedSidebarElements.ToArray();
          }
          PreferencesSettingsControl.SideToolBarOrderChangeStats(RegistryManager.Instance.UserDefinedSidebarElements);
        }
        else
        {
          RegistryManager.Instance.UserDefinedSidebarElements = new string[0];
          PreferencesSettingsControl.SideToolBarOrderChangeStats(RegistryManager.Instance.DefaultSidebarElements);
        }
        foreach (MainWindow mainWindow in BlueStacksUIUtils.DictWindows.Values)
          mainWindow.mSidebar.RearrangeSidebarIcons();
      }
      this.AddToastPopup(LocaleStrings.GetLocalizedString("STRING_CHANGES_SAVED", ""));
      ClientStats.SendMiscellaneousStatsAsync("Setting-save", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "Preferences-Settings", "", (string) null, this.ParentWindow.mVmName, (string) null, (string) null, "Android");
      this.nSaveBtn.IsEnabled = this.IsDirty();
    }

    private static void SideToolBarOrderChangeStats(string[] elements)
    {
      List<string> stringList = new List<string>();
      int num = 1;
      foreach (string element in elements)
      {
        stringList.Add(string.Format("{0}:{1}", (object) num, (object) element));
        ++num;
      }
      ClientStats.SendMiscellaneousStatsAsync("sideToolbar_Order_Changed", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "Preferences-Settings", string.Join(",", stringList.ToArray()), (string) null, (string) null, (string) null, (string) null, "Android");
    }

    private void SendAndroidInputDebuggingStats(string settingChanged)
    {
      ClientStats.SendMiscellaneousStatsAsync("Setting_change", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "Preferences-Settings", settingChanged, (string) null, this.ParentWindow.mVmName, (string) null, (string) null, "Android");
    }

    private void AddToastPopup(string message)
    {
      try
      {
        if (this.mToastPopup == null)
          this.mToastPopup = new CustomToastPopupControl((UserControl) this);
        this.mToastPopup.Init((Window) this.ParentWindow, message, (Brush) null, (Brush) null, HorizontalAlignment.Center, VerticalAlignment.Bottom, new Thickness?(), 12, new Thickness?(), (Brush) null, false, false);
        this.mToastPopup.ShowPopup(1.3);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in showing toast popup: " + ex.ToString());
      }
    }

    private void ArrangePanel_ReOrderedEvent()
    {
      this.nSaveBtn.IsEnabled = this.IsDirty();
      this.mResetOrder.IsEnabled = this.IsResetOrderButtonEnabled();
    }

    private void ResetOrder_Click(object sender, RoutedEventArgs e)
    {
      this.PopulateSidebarIcons(RegistryManager.Instance.DefaultSidebarElements);
      this.mResetOrder.IsEnabled = this.IsResetOrderButtonEnabled();
      this.nSaveBtn.IsEnabled = this.IsDirty();
      ClientStats.SendMiscellaneousStatsAsync("reorder_reset", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, "Preferences-Settings", (string) null, (string) null, (string) null, (string) null, (string) null, "Android");
    }

    private void PopulateSidebarIcons(string[] sidebarItems)
    {
      this.mRearrangeSidebarItems.Clear();
      foreach (string sidebarItem1 in sidebarItems)
      {
        string sidebarItem = sidebarItem1;
        SidebarElement sidebarElement = this.mCurrentVisibleSidebarElements.Where<SidebarElement>((Func<SidebarElement, bool>) (ele => string.Equals(ele.Tag.ToString(), sidebarItem, StringComparison.InvariantCultureIgnoreCase))).FirstOrDefault<SidebarElement>();
        if (sidebarElement != null && sidebarElement.Visibility == Visibility.Visible)
          this.mRearrangeSidebarItems.Add(new RearrangeSidebarItem()
          {
            ImageName = sidebarItem
          });
      }
    }

    private void TouchSoundHelpIconHelpIconPreviewMouseLeftButtonUp(
      object sender,
      MouseButtonEventArgs e)
    {
      Utils.OpenUrl(WebHelper.GetUrlWithParams(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) WebHelper.GetServerHost(), (object) "help_articles"), (string) null, (string) null, (string) null) + "&article=touch_sound_help");
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/settingswindows/preferencessettingscontrol.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.mScrollBar = (ScrollViewer) target;
          break;
        case 2:
          this.mMainGrid = (Grid) target;
          break;
        case 3:
          this.mLanguageSettingsGrid = (Grid) target;
          break;
        case 4:
          this.mLanguageCombobox = (CustomComboBox) target;
          this.mLanguageCombobox.SelectionChanged += new SelectionChangedEventHandler(this.mLanguageCombobox_SelectionChanged);
          break;
        case 5:
          this.mLanguagePreferencePaddingGrid = (Grid) target;
          break;
        case 6:
          this.mPerformancePreference = (Grid) target;
          break;
        case 7:
          this.mPerformanceSettingsLabel = (Label) target;
          break;
        case 8:
          this.mSwitchToHome = (CustomCheckbox) target;
          this.mSwitchToHome.Click += new RoutedEventHandler(this.mSwitchToHome_Click);
          break;
        case 9:
          this.mSwitchKillWebTab = (CustomCheckbox) target;
          this.mSwitchKillWebTab.Click += new RoutedEventHandler(this.SwitchKillWebTab_Click);
          break;
        case 10:
          this.mEnableMemoryTrim = (CustomCheckbox) target;
          this.mEnableMemoryTrim.Click += new RoutedEventHandler(this.EnableMemoryTrim_Click);
          break;
        case 11:
          this.mEnableMemoryTrimWarning = (TextBlock) target;
          break;
        case 12:
          this.mGameControlPreferencePaddingGrid = (Grid) target;
          break;
        case 13:
          this.mGameControlsSettingsGrid = (Grid) target;
          break;
        case 14:
          this.mGameControlSettingsLabel = (Label) target;
          break;
        case 15:
          this.mGameControlsStackPanel = (StackPanel) target;
          break;
        case 16:
          this.mEnableGamePadCheckbox = (CustomCheckbox) target;
          this.mEnableGamePadCheckbox.Click += new RoutedEventHandler(this.CheckBox_Click);
          break;
        case 17:
          this.mHelpIcon = (CustomPictureBox) target;
          this.mHelpIcon.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.HelpIconPreviewMouseLeftButtonUp);
          break;
        case 18:
          this.mEnableNativeGamepad = (Grid) target;
          break;
        case 19:
          this.mForcedOnMode = (CustomRadioButton) target;
          this.mForcedOnMode.Click += new RoutedEventHandler(this.NativeGamepadMode_Click);
          break;
        case 20:
          this.mForcedOffMode = (CustomRadioButton) target;
          this.mForcedOffMode.Click += new RoutedEventHandler(this.NativeGamepadMode_Click);
          break;
        case 21:
          this.mAutoMode = (CustomRadioButton) target;
          this.mAutoMode.Click += new RoutedEventHandler(this.NativeGamepadMode_Click);
          break;
        case 22:
          this.mShowSchemeDeleteWarning = (CustomCheckbox) target;
          this.mShowSchemeDeleteWarning.Click += new RoutedEventHandler(this.CheckBox_Click);
          break;
        case 23:
          this.mPerformancePreferencePaddingGrid = (Grid) target;
          break;
        case 24:
          this.mPlatformStackPanel = (StackPanel) target;
          break;
        case 25:
          this.mAddDesktopShortcuts = (CustomCheckbox) target;
          this.mAddDesktopShortcuts.Click += new RoutedEventHandler(this.CheckBox_Click);
          break;
        case 26:
          this.mShowGamingSummary = (CustomCheckbox) target;
          this.mShowGamingSummary.Click += new RoutedEventHandler(this.CheckBox_Click);
          break;
        case 27:
          this.mEnableTouchSound = (CustomCheckbox) target;
          this.mEnableTouchSound.Click += new RoutedEventHandler(this.CheckBox_Click);
          break;
        case 28:
          this.mTouchSoundHelpIcon = (CustomPictureBox) target;
          this.mTouchSoundHelpIcon.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.TouchSoundHelpIconHelpIconPreviewMouseLeftButtonUp);
          break;
        case 29:
          this.mShowBlueHighlighter = (CustomCheckbox) target;
          this.mShowBlueHighlighter.Click += new RoutedEventHandler(this.CheckBox_Click);
          break;
        case 30:
          this.mShowMacroDeleteWarning = (CustomCheckbox) target;
          this.mShowMacroDeleteWarning.Click += new RoutedEventHandler(this.CheckBox_Click);
          break;
        case 31:
          this.mDiscordCheckBox = (CustomCheckbox) target;
          this.mDiscordCheckBox.Click += new RoutedEventHandler(this.CheckBox_Click);
          break;
        case 32:
          this.mEnableAdbCheckBox = (CustomCheckbox) target;
          this.mEnableAdbCheckBox.Click += new RoutedEventHandler(this.CheckBox_Click);
          break;
        case 33:
          this.mEnableAdbWarning = (TextBlock) target;
          break;
        case 34:
          this.mInputGrid = (Grid) target;
          break;
        case 35:
          this.mEnableTouchPointsCheckBox = (CustomCheckbox) target;
          this.mEnableTouchPointsCheckBox.Click += new RoutedEventHandler(this.CheckBox_Click);
          break;
        case 36:
          this.mEnableTouchCoordinatesCheckbox = (CustomCheckbox) target;
          this.mEnableTouchCoordinatesCheckbox.Click += new RoutedEventHandler(this.CheckBox_Click);
          break;
        case 37:
          this.mEnableDevSettingsWarning = (TextBlock) target;
          break;
        case 38:
          this.mQuitOptionsGrid = (Grid) target;
          break;
        case 39:
          this.mQuitOptionsComboBox = (CustomComboBox) target;
          this.mQuitOptionsComboBox.SelectionChanged += new SelectionChangedEventHandler(this.MQuitOptionsComboBox_SelectionChanged);
          break;
        case 40:
          this.mShowOnExitCheckbox = (CustomCheckbox) target;
          this.mShowOnExitCheckbox.Click += new RoutedEventHandler(this.MShowOnExitCheckbox_Click);
          break;
        case 41:
          this.mScreenshotGrid = (StackPanel) target;
          break;
        case 42:
          this.mScreenShotPathLable = (TextBlock) target;
          break;
        case 43:
          this.mChangePathBtn = (CustomButton) target;
          this.mChangePathBtn.Click += new RoutedEventHandler(this.mChangePathBtn_Click);
          break;
        case 44:
          this.mAccessWidowsFolderCheckbox = (CustomCheckbox) target;
          this.mAccessWidowsFolderCheckbox.Checked += new RoutedEventHandler(this.AccessWidowsFolderCheckbox_Changed);
          this.mAccessWidowsFolderCheckbox.Unchecked += new RoutedEventHandler(this.AccessWidowsFolderCheckbox_Changed);
          break;
        case 45:
          this.mRearrangeListBox = (ListBox) target;
          break;
        case 47:
          this.mResetOrder = (CustomButton) target;
          this.mResetOrder.Click += new RoutedEventHandler(this.ResetOrder_Click);
          break;
        case 48:
          this.nSaveBtn = (CustomButton) target;
          this.nSaveBtn.Click += new RoutedEventHandler(this.SaveBtnClick);
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IStyleConnector.Connect(int connectionId, object target)
    {
      if (connectionId != 46)
        return;
      ((ArrangePanel) target).ReOrderedEvent += new System.Action(this.ArrangePanel_ReOrderedEvent);
    }
  }
}
