// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.PreferenceDropDownControl
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.BlueStacksUI.Controls;
using BlueStacks.Common;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;

namespace BlueStacks.BlueStacksUI
{
  public class PreferenceDropDownControl : UserControl, IComponentConnector, IStyleConnector
  {
    internal Grid EngineSettingGrid;
    internal CustomPictureBox mEngineSettingsButtonImage;
    internal Ellipse mSettingsBtnNotification;
    internal Grid mPinToTopGrid;
    internal CustomPictureBox mPinToTopImage;
    internal CustomPictureBox mPinToTopToggleButton;
    internal Grid mStreamingMode;
    internal CustomPictureBox mStreamingModeImage;
    internal CustomPictureBox mStreaminModeToggleButton;
    internal Grid mMultiInstanceSectionTag;
    internal Separator mMultiInstanceSectionBorderLine;
    internal Grid mMultiInstanceSection;
    internal Grid mSyncGrid;
    internal CustomPictureBox mSyncOperationsImage;
    internal Grid mAutoAlignGrid;
    internal CustomPictureBox mAutoAlignImage;
    internal Grid mUpgradeBluestacksStatus;
    internal CustomPictureBox mUpdateImage;
    internal TextBlock mUpgradeBluestacksStatusTextBlock;
    internal Label mUpdateDownloadProgressPercentage;
    internal Grid mUpgradeToFullBlueStacks;
    internal TextBlock mUpgradeToFullTextBlock;
    internal Grid mLogoutButtonGrid;
    internal Grid mCustomiseSectionTag;
    internal Separator mCustomiseSectionBorderLine;
    internal Grid mCustomiseSection;
    internal Grid mChangeSkinGrid;
    internal CustomPictureBox mChangeSkinImage;
    internal Grid mChangeWallpaperGrid;
    internal CustomPictureBox mChangeWallpaperImage;
    internal Grid mHelpandsupportSectionTag;
    internal Separator mHelpAndSupportSectionBorderLine;
    internal Grid mHelpandsupportSection;
    internal Grid ReportProblemGrid;
    internal Grid mHelpCenterGrid;
    internal CustomPictureBox mHelpCenterImage;
    internal Grid mSpeedUpBstGrid;
    internal CustomPictureBox mSpeedUpBstImage;
    internal CustomPopUp mWallpaperPopup;
    internal Grid mWallpaperPopupGrid;
    internal Grid dummyGridForSize;
    internal Border mWallpaperPopupBorder;
    internal Border mMaskBorder;
    internal TextBlock mTitleText;
    internal TextBlock mBodyText;
    internal System.Windows.Shapes.Path RightArrow;
    internal CustomPopUp mChooseWallpaperPopup;
    internal Grid mChooseWallpaperPopupGrid;
    internal Grid dummyGridForSize2;
    internal Border mPopupGridBorder;
    internal Border mMaskBorder2;
    internal Grid mChooseNewGrid;
    internal Grid mSetDefaultGrid;
    internal TextBlock mRestoreDefaultText;
    internal System.Windows.Shapes.Path mRightArrow;
    private bool _contentLoaded;

    public MainWindow ParentWindow { get; set; }

    private event EventHandler LogoutConfirmationResetAccountAcceptedHandler;

    private event EventHandler RestoreDefaultConfirmationClicked;

    public PreferenceDropDownControl()
    {
      this.InitializeComponent();
      this.LogoutConfirmationResetAccountAcceptedHandler += new EventHandler(this.PreferenceDropDownControl_CloseWindowConfirmationResetAccountAcceptedHandler);
      this.RestoreDefaultConfirmationClicked += new EventHandler(this.PreferenceDropDownControl_RestoreDefaultConfirmationClicked);
      if (RegistryManager.Instance.InstallationType == InstallationTypes.GamingEdition)
      {
        this.mSpeedUpBstGrid.Visibility = Visibility.Collapsed;
        this.mUpgradeToFullBlueStacks.Visibility = Visibility.Visible;
      }
      if (!FeatureManager.Instance.IsShowSpeedUpTips)
        this.mSpeedUpBstGrid.Visibility = Visibility.Collapsed;
      if (FeatureManager.Instance.IsShowHelpCenter)
        return;
      this.mHelpCenterGrid.Visibility = Visibility.Collapsed;
    }

    private void PreferenceDropDownControl_RestoreDefaultConfirmationClicked(
      object sender,
      EventArgs e)
    {
      ClientStats.SendMiscellaneousStatsAsync("hamburgerMenu", RegistryManager.Instance.UserGuid, "RestoreDefaultWallpaper", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, "Premium", (string) null, "Android");
      this.mChooseWallpaperPopup.IsOpen = false;
      this.ParentWindow.Utils.RestoreWallpaperImageForAllVms();
    }

    internal void Init(MainWindow parentWindow)
    {
      this.ParentWindow = parentWindow;
      if (Oem.Instance.IsRemoveAccountOnExit)
        this.mLogoutButtonGrid.Visibility = Visibility.Visible;
      if (RegistryManager.Instance.InstallationType != InstallationTypes.GamingEdition)
        return;
      this.mUpgradeToFullTextBlock.Text = LocaleStrings.GetLocalizedString("STRING_UPGRADE_TO_STANDARD_BST", "").Replace(GameConfig.Instance.AppName, "BlueStacks");
    }

    internal void LateInit()
    {
      if (FeatureManager.Instance.ShowClientOnTopPreference)
        this.mPinToTopToggleButton.ImageName = !this.ParentWindow.EngineInstanceRegistry.IsClientOnTop ? this.mPinToTopToggleButton.ImageName.Replace("_on", "_off") : this.mPinToTopToggleButton.ImageName.Replace("_off", "_on");
      else
        this.mPinToTopGrid.Visibility = Visibility.Collapsed;
      if (FeatureManager.Instance.IsThemeEnabled && RegistryManager.Instance.InstallationType != InstallationTypes.GamingEdition)
        this.mChangeSkinGrid.Visibility = Visibility.Visible;
      if (this.ParentWindow != null && this.ParentWindow.EngineInstanceRegistry.IsGoogleSigninDone && (!FeatureManager.Instance.IsWallpaperChangeDisabled && RegistryManager.Instance.InstallationType != InstallationTypes.GamingEdition) && !FeatureManager.Instance.IsHtmlHome)
        this.mChangeWallpaperGrid.Visibility = Visibility.Visible;
      this.mAutoAlignGrid.MouseLeftButtonUp += new MouseButtonEventHandler(this.AutoAlign_MouseLeftButtonUp);
      this.mAutoAlignGrid.Opacity = 1.0;
      if (!FeatureManager.Instance.IsOperationsSyncEnabled)
        this.mSyncGrid.Visibility = Visibility.Collapsed;
      else if (BlueStacksUIUtils.sSyncInvolvedInstances.Contains(this.ParentWindow.mVmName) && !this.ParentWindow.mIsSyncMaster)
      {
        this.mSyncGrid.PreviewMouseLeftButtonUp -= new MouseButtonEventHandler(this.SyncGrid_MouseLeftButtonUp);
        this.mSyncGrid.MouseEnter -= new MouseEventHandler(this.Grid_MouseEnter);
        this.mSyncGrid.Opacity = 0.5;
      }
      else
      {
        this.mSyncGrid.PreviewMouseLeftButtonUp -= new MouseButtonEventHandler(this.SyncGrid_MouseLeftButtonUp);
        this.mSyncGrid.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.SyncGrid_MouseLeftButtonUp);
        this.mSyncGrid.MouseEnter -= new MouseEventHandler(this.Grid_MouseEnter);
        this.mSyncGrid.MouseEnter += new MouseEventHandler(this.Grid_MouseEnter);
        this.mSyncGrid.Opacity = 1.0;
      }
      this.SectionsTagVisibilityToggling();
    }

    internal void SectionsTagVisibilityToggling()
    {
      this.mCustomiseSectionTag.Visibility = PreferenceDropDownControl.CheckSectionTagVisibility(this.mCustomiseSection) ? Visibility.Visible : Visibility.Collapsed;
      this.mHelpandsupportSectionTag.Visibility = PreferenceDropDownControl.CheckSectionTagVisibility(this.mHelpandsupportSection) ? Visibility.Visible : Visibility.Collapsed;
    }

    private static bool CheckSectionTagVisibility(Grid sectionGrid)
    {
      foreach (UIElement child in sectionGrid.Children)
      {
        if ((child as Grid).Visibility == Visibility.Visible)
          return true;
      }
      return false;
    }

    private void Grid_MouseEnter(object sender, MouseEventArgs e)
    {
      BlueStacksUIBinding.BindColor((DependencyObject) (sender as Grid), Panel.BackgroundProperty, "ContextMenuItemBackgroundHoverColor");
    }

    private void Grid_MouseLeave(object sender, MouseEventArgs e)
    {
      (sender as Grid).Background = (Brush) Brushes.Transparent;
    }

    private void EngineSettingGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      Logger.Info("Clicked settings button");
      this.ParentWindow.mTopBar.mSettingsMenuPopup.IsOpen = false;
      string tabName = string.Empty;
      if (this.ParentWindow.StaticComponents.mSelectedTabButton.mTabType == TabType.AppTab && !PackageActivityNames.SystemApps.Contains(this.ParentWindow.StaticComponents.mSelectedTabButton.PackageName))
        tabName = "STRING_GAME_SETTINGS";
      ClientStats.SendMiscellaneousStatsAsync("hamburgerMenu", RegistryManager.Instance.UserGuid, "Settings", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
      this.ParentWindow.mCommonHandler.LaunchSettingsWindow(tabName);
    }

    private void ReportProblemGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      Logger.Info("Clicked report problem button");
      using (Process process = new Process())
      {
        process.StartInfo.Arguments = "-vmname:" + this.ParentWindow.mVmName;
        process.StartInfo.FileName = System.IO.Path.Combine(RegistryStrings.InstallDir, "HD-LogCollector.exe");
        process.Start();
      }
      ClientStats.SendMiscellaneousStatsAsync("hamburgerMenu", RegistryManager.Instance.UserGuid, "ReportProblem", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
    }

    private void LogoutButtonGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      Logger.Info("Clicked logout button");
      if (!this.ParentWindow.mGuestBootCompleted)
        return;
      CustomMessageWindow customMessageWindow = new CustomMessageWindow();
      BlueStacksUIBinding.Bind(customMessageWindow.TitleTextBlock, "STRING_LOGOUT_BLUESTACKS3", "");
      BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlock, "STRING_REMOVE_GOOGLE_ACCOUNT", "");
      customMessageWindow.AddButton(ButtonColors.Red, "STRING_LOGOUT_BUTTON", this.LogoutConfirmationResetAccountAcceptedHandler, (string) null, false, (object) null, true);
      customMessageWindow.AddButton(ButtonColors.White, "STRING_CANCEL", (EventHandler) null, (string) null, false, (object) null, true);
      this.ParentWindow.ShowDimOverlay((IDimOverlayControl) null);
      customMessageWindow.Owner = (Window) this.ParentWindow.mDimOverlay;
      customMessageWindow.ShowDialog();
      this.ParentWindow.HideDimOverlay();
      ClientStats.SendMiscellaneousStatsAsync("hamburgerMenu", RegistryManager.Instance.UserGuid, "Logout", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
    }

    private void PreferenceDropDownControl_CloseWindowConfirmationResetAccountAcceptedHandler(
      object sender,
      EventArgs e)
    {
      this.ParentWindow.mAppHandler.SendRequestToRemoveAccountAndCloseWindowASync(false);
    }

    private void SpeedUpBstGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      Logger.Info("Clicked SpeedUp BlueStacks button");
      SpeedUpBlueStacks speedUpBlueStacks = new SpeedUpBlueStacks();
      if (this.ParentWindow.mTopBar.mSnailMode == PerformanceState.VtxDisabled)
        speedUpBlueStacks.mEnableVt.Visibility = Visibility.Visible;
      speedUpBlueStacks.mUpgradeComputer.Visibility = Visibility.Visible;
      speedUpBlueStacks.mPowerPlan.Visibility = Visibility.Visible;
      speedUpBlueStacks.mConfigureAntivirus.Visibility = Visibility.Visible;
      ContainerWindow containerWindow = new ContainerWindow(this.ParentWindow, (UserControl) speedUpBlueStacks, 640.0, 440.0, false, true, false, -1.0, (Brush) null);
      ClientStats.SendMiscellaneousStatsAsync("hamburgerMenu", RegistryManager.Instance.UserGuid, "SpeedUpBlueStacks", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
    }

    private void mHelpCenterGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      string helpCenterUrl = BlueStacksUIUtils.GetHelpCenterUrl();
      this.ParentWindow.mTopBar.mSettingsMenuPopup.IsOpen = false;
      ClientStats.SendMiscellaneousStatsAsync("hamburgerMenu", RegistryManager.Instance.UserGuid, "HelpCentre", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
      if (RegistryManager.Instance.InstallationType == InstallationTypes.GamingEdition)
        BlueStacksUIUtils.OpenUrl(helpCenterUrl);
      else
        this.ParentWindow.mTopBar.mAppTabButtons.AddWebTab(helpCenterUrl, "STRING_FEEDBACK", "help_center", true, "FEEDBACK_TEXT", false);
    }

    private void mChangeSkinGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      ChangeThemeWindow changeThemeWindow = new ChangeThemeWindow(this.ParentWindow);
      int num1 = 504;
      int num2 = 652;
      ContainerWindow containerWindow = new ContainerWindow(this.ParentWindow, (UserControl) changeThemeWindow, (double) num2, (double) num1, false, true, false, -1.0, (Brush) null);
      ClientStats.SendMiscellaneousStatsAsync("hamburgerMenu", RegistryManager.Instance.UserGuid, "ChangeSkin", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
    }

    private void NotificationPopup_Opened(object sender, EventArgs e)
    {
      this.dummyGridForSize2.Visibility = Visibility.Visible;
    }

    private void NotificationPopup_Closed(object sender, EventArgs e)
    {
      this.mChangeWallpaperGrid.Background = (Brush) Brushes.Transparent;
    }

    private void ChooseNewGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.ParentWindow.mTopBar.mSettingsMenuPopup.IsOpen = false;
      if (RegistryManager.Instance.IsPremium)
      {
        ClientStats.SendMiscellaneousStatsAsync("hamburgerMenu", RegistryManager.Instance.UserGuid, "ChangeWallPaperButton", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, "Premium", (string) null, "Android");
        this.ParentWindow.Utils.ChooseWallpaper();
      }
      else
      {
        ClientStats.SendMiscellaneousStatsAsync("hamburgerMenu", RegistryManager.Instance.UserGuid, "ChangeWallPaperButton", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, "NonPremium", (string) null, "Android");
        this.ParentWindow.mTopBar.mAppTabButtons.AddWebTab(WebHelper.GetUrlWithParams(WebHelper.GetServerHost() + "/bluestacks_account?extra=section:plans", (string) null, (string) null, (string) null) + "&email=" + RegistryManager.Instance.RegisteredEmail + "&token=" + RegistryManager.Instance.Token, "STRING_ACCOUNT", "account_tab", true, "account_tab", false);
      }
    }

    private void SetDefaultGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      if (!File.Exists(HomeAppManager.BackgroundImagePath))
        return;
      CustomMessageWindow customMessageWindow = new CustomMessageWindow();
      BlueStacksUIBinding.Bind(customMessageWindow.TitleTextBlock, "STRING_LBL_RESTORE_DEFAULT", "");
      BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlock, "STRING_RESTORE_DEFAULT_WALLPAPER", "");
      customMessageWindow.AddButton(ButtonColors.Red, "STRING_RESTORE_BUTTON", this.RestoreDefaultConfirmationClicked, (string) null, false, (object) null, true);
      customMessageWindow.AddButton(ButtonColors.White, "STRING_CANCEL", (EventHandler) null, (string) null, false, (object) null, true);
      this.ParentWindow.ShowDimOverlay((IDimOverlayControl) null);
      customMessageWindow.Owner = (Window) this.ParentWindow.mDimOverlay;
      customMessageWindow.ShowDialog();
      this.ParentWindow.HideDimOverlay();
    }

    private void mChangeWallpaperGrid_MouseEnter(object sender, MouseEventArgs e)
    {
      if (File.Exists(HomeAppManager.BackgroundImagePath))
      {
        this.mChangeWallpaperGrid.MouseLeftButtonUp -= new MouseButtonEventHandler(this.ChooseNewGrid_MouseLeftButtonUp);
        this.mWallpaperPopup.PlacementTarget = (UIElement) this.mChooseNewGrid;
        this.mChooseWallpaperPopup.IsOpen = false;
        this.mChooseWallpaperPopup.IsOpen = true;
      }
      else
      {
        if (!RegistryManager.Instance.IsPremium)
        {
          this.mWallpaperPopup.PlacementTarget = (UIElement) this.mChangeWallpaperGrid;
          this.mWallpaperPopup.IsOpen = false;
          this.mWallpaperPopup.IsOpen = true;
        }
        this.mChangeWallpaperGrid.MouseLeftButtonUp -= new MouseButtonEventHandler(this.ChooseNewGrid_MouseLeftButtonUp);
        this.mChangeWallpaperGrid.MouseLeftButtonUp += new MouseButtonEventHandler(this.ChooseNewGrid_MouseLeftButtonUp);
      }
      BlueStacksUIBinding.BindColor((DependencyObject) (sender as Grid), Panel.BackgroundProperty, "ContextMenuItemBackgroundHoverColor");
    }

    private void mChangeWallpaperGrid_MouseLeave(object sender, MouseEventArgs e)
    {
      (sender as Grid).Background = (Brush) Brushes.Transparent;
      if (this.mChangeWallpaperGrid.IsMouseOver || this.mChooseWallpaperPopupGrid.IsMouseOver || this.mWallpaperPopupGrid.IsMouseOver)
        return;
      this.mChooseWallpaperPopup.IsOpen = false;
      this.mWallpaperPopup.IsOpen = false;
    }

    private void ChooseNewGrid_MouseEnter(object sender, MouseEventArgs e)
    {
      if (!RegistryManager.Instance.IsPremium)
        this.mWallpaperPopup.IsOpen = true;
      BlueStacksUIBinding.BindColor((DependencyObject) (sender as Grid), Panel.BackgroundProperty, "ContextMenuItemBackgroundHoverColor");
    }

    private void ChooseNewGrid_MouseLeave(object sender, MouseEventArgs e)
    {
      if (!this.mChooseNewGrid.IsMouseOver && !this.mWallpaperPopupGrid.IsMouseOver)
        this.mWallpaperPopup.IsOpen = false;
      (sender as Grid).Background = (Brush) Brushes.Transparent;
    }

    private void SetDefaultGrid_MouseLeave(object sender, MouseEventArgs e)
    {
      if (!File.Exists(HomeAppManager.BackgroundImagePath))
        return;
      (sender as Grid).Background = (Brush) Brushes.Transparent;
    }

    private void SetDefaultGrid_MouseEnter(object sender, MouseEventArgs e)
    {
      if (!File.Exists(HomeAppManager.BackgroundImagePath))
        return;
      BlueStacksUIBinding.BindColor((DependencyObject) (sender as Grid), Panel.BackgroundProperty, "ContextMenuItemBackgroundHoverColor");
    }

    private void mWallpaperPopup_MouseLeave(object sender, MouseEventArgs e)
    {
      if (this.mChooseNewGrid.IsMouseOver)
        return;
      this.mWallpaperPopup.IsOpen = false;
    }

    private void mChooseWallpaperPopup_MouseLeave(object sender, MouseEventArgs e)
    {
      if (this.mChangeWallpaperGrid.IsMouseOver || this.mChooseWallpaperPopupGrid.IsMouseOver || this.mWallpaperPopupGrid.IsMouseOver)
        return;
      this.mChooseWallpaperPopup.IsOpen = false;
      this.mWallpaperPopup.IsOpen = false;
    }

    private void mUpgradeToFullBlueStacks_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      string localizedString = LocaleStrings.GetLocalizedString("STRING_UPGRADE_TO_STANDARD_BST", "");
      string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} {1}", (object) LocaleStrings.GetLocalizedString("STRING_CONTINUING_WILL_UPGRADE_TO_STD_BST", ""), (object) LocaleStrings.GetLocalizedString("STRING_LAUNCH_BLUESTACKS_FROM_DESK_SHORTCUT", ""));
      string path1 = localizedString.Replace(GameConfig.Instance.AppName, "BlueStacks");
      string path2 = str.Replace(GameConfig.Instance.AppName, "BlueStacks");
      CustomMessageWindow customMessageWindow = new CustomMessageWindow();
      customMessageWindow.AddButton(ButtonColors.Blue, "STRING_YES", new EventHandler(this.UpgradeToFullBstHandler), (string) null, false, (object) null, true);
      customMessageWindow.AddButton(ButtonColors.White, "STRING_NO", (EventHandler) null, (string) null, false, (object) null, true);
      BlueStacksUIBinding.Bind(customMessageWindow.TitleTextBlock, path1, "");
      BlueStacksUIBinding.Bind(customMessageWindow.BodyTextBlock, path2, "");
      this.ParentWindow.ShowDimOverlay((IDimOverlayControl) null);
      customMessageWindow.Owner = (Window) this.ParentWindow.mDimOverlay;
      customMessageWindow.ShowDialog();
      this.ParentWindow.HideDimOverlay();
      ClientStats.SendMiscellaneousStatsAsync("hamburgerMenu", RegistryManager.Instance.UserGuid, "UpgradeBlueStacks", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
    }

    private void UpgradeToFullBstHandler(object sender, EventArgs e)
    {
      this.ParentWindow.mWelcomeTab.mBackground.Visibility = Visibility.Visible;
      this.ParentWindow.ShowDimOverlayForUpgrade();
      using (BackgroundWorker backgroundWorker = new BackgroundWorker())
      {
        backgroundWorker.DoWork += new DoWorkEventHandler(this.MBWUpdateToFullVersion_DoWork);
        backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.MBWUpdateToFullVersion_RunWorkerCompleted);
        backgroundWorker.RunWorkerAsync();
      }
    }

    private void MBWUpdateToFullVersion_RunWorkerCompleted(
      object sender,
      RunWorkerCompletedEventArgs e)
    {
      this.ParentWindow.MainWindow_CloseWindowConfirmationAcceptedHandler((object) null, (EventArgs) null);
    }

    private void MBWUpdateToFullVersion_DoWork(object sender, DoWorkEventArgs e)
    {
      Utils.UpgradeToFullVersionAndCreateBstShortcut(true);
    }

    private void SyncGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.ParentWindow.mTopBar.mSettingsMenuPopup.IsOpen = false;
      this.ParentWindow.ShowSynchronizerWindow();
      ClientStats.SendMiscellaneousStatsAsync("hamburgerMenu", RegistryManager.Instance.UserGuid, "OperationSync", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
    }

    private void mUpgradeBluestacksStatus_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      if (this.mUpgradeBluestacksStatusTextBlock.Text.ToString((IFormatProvider) CultureInfo.InvariantCulture).Equals(LocaleStrings.GetLocalizedString("STRING_DOWNLOAD_UPDATE", ""), StringComparison.OrdinalIgnoreCase))
      {
        ClientStats.SendBluestacksUpdaterUIStatsAsync(ClientStatsEvent.SettingsGearDwnld, "");
        UpdatePrompt updatePrompt1 = new UpdatePrompt(BlueStacksUpdater.sBstUpdateData);
        updatePrompt1.Height = 215.0;
        updatePrompt1.Width = 400.0;
        UpdatePrompt updatePrompt2 = updatePrompt1;
        ContainerWindow containerWindow = new ContainerWindow(this.ParentWindow, (UserControl) updatePrompt2, (double) (int) updatePrompt2.Width, (double) (int) updatePrompt2.Height, false, true, false, -1.0, (Brush) null);
      }
      else if (this.mUpgradeBluestacksStatusTextBlock.Text.ToString((IFormatProvider) CultureInfo.InvariantCulture).Equals(LocaleStrings.GetLocalizedString("STRING_DOWNLOADING_UPDATE", ""), StringComparison.OrdinalIgnoreCase))
      {
        this.ParentWindow.mTopBar.mSettingsMenuPopup.IsOpen = false;
        BlueStacksUpdater.ShowDownloadProgress();
      }
      else
      {
        if (!this.mUpgradeBluestacksStatusTextBlock.Text.ToString((IFormatProvider) CultureInfo.InvariantCulture).Equals(LocaleStrings.GetLocalizedString("STRING_INSTALL_UPDATE", ""), StringComparison.OrdinalIgnoreCase))
          return;
        this.ParentWindow.ShowInstallPopup();
      }
    }

    internal void ToggleStreamingMode(bool enable)
    {
      if (enable)
        this.mStreaminModeToggleButton.ImageName = this.mStreaminModeToggleButton.ImageName.Replace("_off", "_on");
      else
        this.mStreaminModeToggleButton.ImageName = this.mStreaminModeToggleButton.ImageName.Replace("_on", "_off");
    }

    private void AutoAlign_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.ParentWindow.mTopBar.mSettingsMenuPopup.IsOpen = false;
      CommonHandlers.ArrangeWindow();
      ClientStats.SendMiscellaneousStatsAsync("hamburgerMenu", RegistryManager.Instance.UserGuid, "AutoAlign", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
    }

    private void PinToTop_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      CustomPictureBox customPictureBox = sender as CustomPictureBox;
      if (customPictureBox.ImageName.Contains("_off"))
      {
        customPictureBox.ImageName = "toggle_on";
        this.ParentWindow.EngineInstanceRegistry.IsClientOnTop = true;
        this.ParentWindow.Topmost = true;
        ClientStats.SendMiscellaneousStatsAsync("hamburgerMenu", RegistryManager.Instance.UserGuid, "PinToTopOn", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
      }
      else
      {
        customPictureBox.ImageName = "toggle_off";
        this.ParentWindow.EngineInstanceRegistry.IsClientOnTop = false;
        this.ParentWindow.Topmost = false;
        ClientStats.SendMiscellaneousStatsAsync("hamburgerMenu", RegistryManager.Instance.UserGuid, "PinToTopOff", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
      }
    }

    private void Streaming_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      CustomPictureBox customPictureBox = sender as CustomPictureBox;
      if (customPictureBox.ImageName.Contains("_off"))
      {
        customPictureBox.ImageName = "toggle_on";
        this.ParentWindow.mFrontendHandler.ToggleStreamingMode(true);
        ClientStats.SendMiscellaneousStatsAsync("hamburgerMenu", RegistryManager.Instance.UserGuid, "StreamingModeStart", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
      }
      else
      {
        customPictureBox.ImageName = "toggle_off";
        this.ParentWindow.mFrontendHandler.ToggleStreamingMode(false);
        ClientStats.SendMiscellaneousStatsAsync("hamburgerMenu", RegistryManager.Instance.UserGuid, "StreamingModeStop", "MouseClick", RegistryManager.Instance.ClientVersion, RegistryManager.Instance.Version, RegistryManager.Instance.Oem, (string) null, (string) null, "Android");
      }
      this.ParentWindow.mTopBar.mSettingsMenuPopup.IsOpen = false;
    }

    private void TextBlock_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      if (sender == null)
        return;
      (sender as TextBlock).SetTextblockTooltip();
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/preferencedropdowncontrol.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    internal Delegate _CreateDelegate(Type delegateType, string handler)
    {
      return Delegate.CreateDelegate(delegateType, (object) this, handler);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 2:
          this.EngineSettingGrid = (Grid) target;
          this.EngineSettingGrid.MouseEnter += new MouseEventHandler(this.Grid_MouseEnter);
          this.EngineSettingGrid.MouseLeave += new MouseEventHandler(this.Grid_MouseLeave);
          this.EngineSettingGrid.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.EngineSettingGrid_MouseLeftButtonUp);
          break;
        case 3:
          this.mEngineSettingsButtonImage = (CustomPictureBox) target;
          break;
        case 4:
          this.mSettingsBtnNotification = (Ellipse) target;
          break;
        case 5:
          this.mPinToTopGrid = (Grid) target;
          this.mPinToTopGrid.MouseEnter += new MouseEventHandler(this.Grid_MouseEnter);
          this.mPinToTopGrid.MouseLeave += new MouseEventHandler(this.Grid_MouseLeave);
          break;
        case 6:
          this.mPinToTopImage = (CustomPictureBox) target;
          break;
        case 7:
          this.mPinToTopToggleButton = (CustomPictureBox) target;
          this.mPinToTopToggleButton.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.PinToTop_MouseLeftButtonUp);
          break;
        case 8:
          this.mStreamingMode = (Grid) target;
          this.mStreamingMode.MouseEnter += new MouseEventHandler(this.Grid_MouseEnter);
          this.mStreamingMode.MouseLeave += new MouseEventHandler(this.Grid_MouseLeave);
          break;
        case 9:
          this.mStreamingModeImage = (CustomPictureBox) target;
          break;
        case 10:
          this.mStreaminModeToggleButton = (CustomPictureBox) target;
          this.mStreaminModeToggleButton.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.Streaming_MouseLeftButtonUp);
          break;
        case 11:
          this.mMultiInstanceSectionTag = (Grid) target;
          break;
        case 12:
          this.mMultiInstanceSectionBorderLine = (Separator) target;
          break;
        case 13:
          this.mMultiInstanceSection = (Grid) target;
          break;
        case 14:
          this.mSyncGrid = (Grid) target;
          this.mSyncGrid.MouseEnter += new MouseEventHandler(this.Grid_MouseEnter);
          this.mSyncGrid.MouseLeave += new MouseEventHandler(this.Grid_MouseLeave);
          this.mSyncGrid.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.SyncGrid_MouseLeftButtonUp);
          break;
        case 15:
          this.mSyncOperationsImage = (CustomPictureBox) target;
          break;
        case 16:
          this.mAutoAlignGrid = (Grid) target;
          this.mAutoAlignGrid.MouseEnter += new MouseEventHandler(this.Grid_MouseEnter);
          this.mAutoAlignGrid.MouseLeave += new MouseEventHandler(this.Grid_MouseLeave);
          this.mAutoAlignGrid.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.AutoAlign_MouseLeftButtonUp);
          break;
        case 17:
          this.mAutoAlignImage = (CustomPictureBox) target;
          break;
        case 18:
          this.mUpgradeBluestacksStatus = (Grid) target;
          this.mUpgradeBluestacksStatus.MouseEnter += new MouseEventHandler(this.Grid_MouseEnter);
          this.mUpgradeBluestacksStatus.MouseLeave += new MouseEventHandler(this.Grid_MouseLeave);
          this.mUpgradeBluestacksStatus.MouseLeftButtonUp += new MouseButtonEventHandler(this.mUpgradeBluestacksStatus_MouseLeftButtonUp);
          break;
        case 19:
          this.mUpdateImage = (CustomPictureBox) target;
          break;
        case 20:
          this.mUpgradeBluestacksStatusTextBlock = (TextBlock) target;
          break;
        case 21:
          this.mUpdateDownloadProgressPercentage = (Label) target;
          break;
        case 22:
          this.mUpgradeToFullBlueStacks = (Grid) target;
          this.mUpgradeToFullBlueStacks.MouseEnter += new MouseEventHandler(this.Grid_MouseEnter);
          this.mUpgradeToFullBlueStacks.MouseLeave += new MouseEventHandler(this.Grid_MouseLeave);
          this.mUpgradeToFullBlueStacks.MouseLeftButtonUp += new MouseButtonEventHandler(this.mUpgradeToFullBlueStacks_MouseLeftButtonUp);
          break;
        case 23:
          this.mUpgradeToFullTextBlock = (TextBlock) target;
          break;
        case 24:
          this.mLogoutButtonGrid = (Grid) target;
          this.mLogoutButtonGrid.MouseEnter += new MouseEventHandler(this.Grid_MouseEnter);
          this.mLogoutButtonGrid.MouseLeave += new MouseEventHandler(this.Grid_MouseLeave);
          this.mLogoutButtonGrid.MouseLeftButtonUp += new MouseButtonEventHandler(this.LogoutButtonGrid_MouseLeftButtonUp);
          break;
        case 25:
          this.mCustomiseSectionTag = (Grid) target;
          break;
        case 26:
          this.mCustomiseSectionBorderLine = (Separator) target;
          break;
        case 27:
          this.mCustomiseSection = (Grid) target;
          break;
        case 28:
          this.mChangeSkinGrid = (Grid) target;
          this.mChangeSkinGrid.MouseEnter += new MouseEventHandler(this.Grid_MouseEnter);
          this.mChangeSkinGrid.MouseLeave += new MouseEventHandler(this.Grid_MouseLeave);
          this.mChangeSkinGrid.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.mChangeSkinGrid_MouseLeftButtonUp);
          break;
        case 29:
          this.mChangeSkinImage = (CustomPictureBox) target;
          break;
        case 30:
          this.mChangeWallpaperGrid = (Grid) target;
          this.mChangeWallpaperGrid.MouseEnter += new MouseEventHandler(this.mChangeWallpaperGrid_MouseEnter);
          this.mChangeWallpaperGrid.MouseLeave += new MouseEventHandler(this.mChangeWallpaperGrid_MouseLeave);
          break;
        case 31:
          this.mChangeWallpaperImage = (CustomPictureBox) target;
          break;
        case 32:
          this.mHelpandsupportSectionTag = (Grid) target;
          break;
        case 33:
          this.mHelpAndSupportSectionBorderLine = (Separator) target;
          break;
        case 34:
          this.mHelpandsupportSection = (Grid) target;
          break;
        case 35:
          this.ReportProblemGrid = (Grid) target;
          this.ReportProblemGrid.MouseEnter += new MouseEventHandler(this.Grid_MouseEnter);
          this.ReportProblemGrid.MouseLeave += new MouseEventHandler(this.Grid_MouseLeave);
          this.ReportProblemGrid.MouseLeftButtonUp += new MouseButtonEventHandler(this.ReportProblemGrid_MouseLeftButtonUp);
          break;
        case 36:
          this.mHelpCenterGrid = (Grid) target;
          this.mHelpCenterGrid.MouseEnter += new MouseEventHandler(this.Grid_MouseEnter);
          this.mHelpCenterGrid.MouseLeave += new MouseEventHandler(this.Grid_MouseLeave);
          this.mHelpCenterGrid.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.mHelpCenterGrid_MouseLeftButtonUp);
          break;
        case 37:
          this.mHelpCenterImage = (CustomPictureBox) target;
          break;
        case 38:
          this.mSpeedUpBstGrid = (Grid) target;
          this.mSpeedUpBstGrid.MouseEnter += new MouseEventHandler(this.Grid_MouseEnter);
          this.mSpeedUpBstGrid.MouseLeave += new MouseEventHandler(this.Grid_MouseLeave);
          this.mSpeedUpBstGrid.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.SpeedUpBstGrid_MouseLeftButtonUp);
          break;
        case 39:
          this.mSpeedUpBstImage = (CustomPictureBox) target;
          break;
        case 40:
          this.mWallpaperPopup = (CustomPopUp) target;
          break;
        case 41:
          this.mWallpaperPopupGrid = (Grid) target;
          break;
        case 42:
          this.dummyGridForSize = (Grid) target;
          break;
        case 43:
          this.mWallpaperPopupBorder = (Border) target;
          break;
        case 44:
          this.mMaskBorder = (Border) target;
          break;
        case 45:
          this.mTitleText = (TextBlock) target;
          break;
        case 46:
          this.mBodyText = (TextBlock) target;
          break;
        case 47:
          this.RightArrow = (System.Windows.Shapes.Path) target;
          break;
        case 48:
          this.mChooseWallpaperPopup = (CustomPopUp) target;
          break;
        case 49:
          this.mChooseWallpaperPopupGrid = (Grid) target;
          break;
        case 50:
          this.dummyGridForSize2 = (Grid) target;
          break;
        case 51:
          this.mPopupGridBorder = (Border) target;
          break;
        case 52:
          this.mMaskBorder2 = (Border) target;
          break;
        case 53:
          this.mChooseNewGrid = (Grid) target;
          this.mChooseNewGrid.MouseEnter += new MouseEventHandler(this.ChooseNewGrid_MouseEnter);
          this.mChooseNewGrid.MouseLeave += new MouseEventHandler(this.ChooseNewGrid_MouseLeave);
          this.mChooseNewGrid.MouseLeftButtonUp += new MouseButtonEventHandler(this.ChooseNewGrid_MouseLeftButtonUp);
          break;
        case 54:
          this.mSetDefaultGrid = (Grid) target;
          this.mSetDefaultGrid.MouseEnter += new MouseEventHandler(this.SetDefaultGrid_MouseEnter);
          this.mSetDefaultGrid.MouseLeave += new MouseEventHandler(this.SetDefaultGrid_MouseLeave);
          this.mSetDefaultGrid.MouseLeftButtonUp += new MouseButtonEventHandler(this.SetDefaultGrid_MouseLeftButtonUp);
          break;
        case 55:
          this.mRestoreDefaultText = (TextBlock) target;
          break;
        case 56:
          this.mRightArrow = (System.Windows.Shapes.Path) target;
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
      if (connectionId != 1)
        return;
      ((Style) target).Setters.Add((SetterBase) new EventSetter()
      {
        Event = FrameworkElement.SizeChangedEvent,
        Handler = (Delegate) new SizeChangedEventHandler(this.TextBlock_SizeChanged)
      });
    }
  }
}
