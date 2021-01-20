// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.AboutSettingsControl
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Navigation;

namespace BlueStacks.BlueStacksUI
{
  public class AboutSettingsControl : UserControl, IComponentConnector
  {
    private MainWindow ParentWindow;
    internal Grid mPoweredByBSGrid;
    internal Grid mBSIconAndNameGrid;
    internal Grid mProductTextGrid;
    internal Label mVersionLabel;
    internal Grid mUpdateInfoGrid;
    internal Label bodyLabel;
    internal Label mLabelVersion;
    internal Hyperlink mDetailedChangeLogs;
    internal CustomButton mCheckUpdateBtn;
    internal TextBlock mStatusLabel;
    internal Grid mCheckingGrid;
    internal Grid ContactInfoGrid;
    internal Label mWebsiteLabel;
    internal Label mSupportLabel;
    internal Label mSupportMailLabel;
    internal Hyperlink mSupportEMailHyperlink;
    internal TextBlock mTermsOfUse;
    internal Hyperlink mTermsOfUseLink;
    private bool _contentLoaded;

    public AboutSettingsControl(MainWindow window, SettingsWindow _)
    {
      this.ParentWindow = window;
      this.InitializeComponent();
      this.Visibility = Visibility.Hidden;
      this.Loaded += new RoutedEventHandler(this.AboutSettingsControl_Loaded);
      AppPlayerModel appPlayerModel = InstalledOem.GetAppPlayerModel("bgp", Utils.GetValueInBootParams("abivalue", "Android", string.Empty, "bgp"));
      this.mVersionLabel.Content = (object) ("v" + RegistryManager.Instance.Version);
      BlueStacksUIBinding.Bind(this.mSupportLabel, "STRING_SUPPORT");
      this.mSupportLabel.ContentStringFormat = "{0} - ";
      BlueStacksUIBinding.Bind(this.mWebsiteLabel, "STRING_WEBSITE");
      this.mWebsiteLabel.ContentStringFormat = "{0} - ";
      BlueStacksUIBinding.Bind(this.mSupportMailLabel, "STRING_SUPPORT_EMAIL");
      this.mSupportMailLabel.ContentStringFormat = "{0} - ";
      if (Oem.Instance.IsProductBeta)
      {
        Label mVersionLabel = this.mVersionLabel;
        mVersionLabel.Content = (object) (mVersionLabel.Content?.ToString() + LocaleStrings.GetLocalizedString("STRING_BETA", ""));
      }
      if (appPlayerModel != null && appPlayerModel.AppPlayerOsArch != null)
      {
        Label mVersionLabel = this.mVersionLabel;
        mVersionLabel.Content = (object) (mVersionLabel.Content?.ToString() + ", " + appPlayerModel.AppPlayerOsArch);
      }
      else if (Oem.Instance.IsAndroid64Bit)
      {
        Label mVersionLabel = this.mVersionLabel;
        mVersionLabel.Content = (object) (mVersionLabel.Content?.ToString() + ", " + LocaleStrings.GetLocalizedString("STRING_64BIT_ANDROID", ""));
      }
      else
      {
        Label mVersionLabel = this.mVersionLabel;
        mVersionLabel.Content = (object) (mVersionLabel.Content?.ToString() + ", " + LocaleStrings.GetLocalizedString("STRING_32BIT_ANDROID", ""));
      }
      this.mTermsOfUseLink.NavigateUri = new Uri(WebHelper.GetUrlWithParams(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) WebHelper.GetServerHost(), (object) "help_articles"), (string) null, (string) null, (string) null) + "&article=" + "terms_of_use");
      this.mDetailedChangeLogs.Inlines.Clear();
      this.mDetailedChangeLogs.Inlines.Add(LocaleStrings.GetLocalizedString("STRING_LEARN_WHATS_NEW", "Learn What's New"));
    }

    private void AboutSettingsControl_Loaded(object sender, RoutedEventArgs e)
    {
      if (FeatureManager.Instance.IsCustomUIForDMMSandbox)
        this.ContactInfoGrid.Visibility = Visibility.Hidden;
      if (RegistryManager.Instance.InstallationType == InstallationTypes.GamingEdition)
      {
        this.mPoweredByBSGrid.Visibility = Visibility.Visible;
        this.mBSIconAndNameGrid.Visibility = Visibility.Hidden;
      }
      if (Oem.Instance.IsProductBeta)
      {
        string text = "beta-support@bluestacks.com";
        this.mSupportEMailHyperlink.Inlines.Clear();
        this.mSupportEMailHyperlink.Inlines.Add(text);
        this.mSupportEMailHyperlink.NavigateUri = new Uri("mailto:" + text);
      }
      this.HandleUpdateStateGridVisibility(BlueStacksUpdater.SUpdateState);
      BlueStacksUpdater.StateChanged -= new System.Action(this.BlueStacksUpdater_StateChanged);
      BlueStacksUpdater.StateChanged += new System.Action(this.BlueStacksUpdater_StateChanged);
    }

    private void BlueStacksUpdater_StateChanged()
    {
      this.HandleUpdateStateGridVisibility(BlueStacksUpdater.SUpdateState);
    }

    private void HandleUpdateStateGridVisibility(BlueStacksUpdater.UpdateState state)
    {
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        switch (state)
        {
          case BlueStacksUpdater.UpdateState.NO_UPDATE:
            this.mUpdateInfoGrid.Visibility = Visibility.Collapsed;
            this.mCheckUpdateBtn.HorizontalAlignment = HorizontalAlignment.Left;
            this.mCheckUpdateBtn.Visibility = Visibility.Visible;
            BlueStacksUIBinding.Bind((Button) this.mCheckUpdateBtn, "STRING_CHECK_UPDATES");
            this.mStatusLabel.Visibility = Visibility.Collapsed;
            this.mCheckingGrid.Visibility = Visibility.Collapsed;
            break;
          case BlueStacksUpdater.UpdateState.UPDATE_AVAILABLE:
            this.mUpdateInfoGrid.Visibility = Visibility.Visible;
            BlueStacksUIBinding.Bind(this.bodyLabel, "STRING_UPDATE_AVAILABLE");
            this.mDetailedChangeLogs.NavigateUri = new Uri(BlueStacksUpdater.sBstUpdateData.DetailedChangeLogsUrl);
            this.mLabelVersion.Content = (object) ("v" + BlueStacksUpdater.sBstUpdateData.EngineVersion);
            this.mCheckUpdateBtn.HorizontalAlignment = HorizontalAlignment.Right;
            this.mCheckUpdateBtn.Visibility = Visibility.Visible;
            BlueStacksUIBinding.Bind((Button) this.mCheckUpdateBtn, "STRING_DOWNLOAD_UPDATE");
            this.mStatusLabel.Visibility = Visibility.Collapsed;
            this.mCheckingGrid.Visibility = Visibility.Collapsed;
            break;
          case BlueStacksUpdater.UpdateState.DOWNLOADING:
            this.mUpdateInfoGrid.Visibility = Visibility.Visible;
            BlueStacksUIBinding.Bind(this.bodyLabel, "STRING_DOWNLOADING_UPDATE");
            this.mDetailedChangeLogs.NavigateUri = new Uri(BlueStacksUpdater.sBstUpdateData.DetailedChangeLogsUrl);
            this.mLabelVersion.Content = (object) ("v" + BlueStacksUpdater.sBstUpdateData.EngineVersion);
            this.mCheckUpdateBtn.Visibility = Visibility.Collapsed;
            this.mStatusLabel.Visibility = Visibility.Collapsed;
            this.mCheckingGrid.Visibility = Visibility.Collapsed;
            break;
          case BlueStacksUpdater.UpdateState.DOWNLOADED:
            this.mUpdateInfoGrid.Visibility = Visibility.Visible;
            BlueStacksUIBinding.Bind(this.bodyLabel, "STRING_UPDATES_READY_TO_INSTALL");
            this.mDetailedChangeLogs.NavigateUri = new Uri(BlueStacksUpdater.sBstUpdateData.DetailedChangeLogsUrl);
            this.mLabelVersion.Content = (object) ("v" + BlueStacksUpdater.sBstUpdateData.EngineVersion);
            this.mCheckUpdateBtn.HorizontalAlignment = HorizontalAlignment.Right;
            this.mCheckUpdateBtn.Visibility = Visibility.Visible;
            BlueStacksUIBinding.Bind((Button) this.mCheckUpdateBtn, "STRING_INSTALL_UPDATE");
            this.mStatusLabel.Visibility = Visibility.Collapsed;
            this.mCheckingGrid.Visibility = Visibility.Collapsed;
            break;
        }
      }));
    }

    private void ShowCheckingForUpdateGrid()
    {
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        this.mUpdateInfoGrid.Visibility = Visibility.Collapsed;
        this.mCheckUpdateBtn.Visibility = Visibility.Collapsed;
        this.mStatusLabel.Visibility = Visibility.Collapsed;
        this.mCheckingGrid.Visibility = Visibility.Visible;
      }));
    }

    private void ShowLatestVersionGrid()
    {
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        this.mUpdateInfoGrid.Visibility = Visibility.Collapsed;
        this.mCheckUpdateBtn.Visibility = Visibility.Collapsed;
        this.mStatusLabel.Visibility = Visibility.Visible;
        BlueStacksUIBinding.Bind(this.mStatusLabel, "STRING_LATEST_VERSION", "");
        this.mCheckingGrid.Visibility = Visibility.Collapsed;
      }));
    }

    private void ShowInternetConnectionErrorGrid()
    {
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        this.mUpdateInfoGrid.Visibility = Visibility.Collapsed;
        this.mCheckUpdateBtn.HorizontalAlignment = HorizontalAlignment.Right;
        this.mCheckUpdateBtn.Visibility = Visibility.Visible;
        BlueStacksUIBinding.Bind((Button) this.mCheckUpdateBtn, "STRING_RETRY_CONNECTION_ISSUE_TEXT1");
        this.mStatusLabel.Visibility = Visibility.Visible;
        BlueStacksUIBinding.Bind(this.mStatusLabel, "STRING_POST_OTS_FAILED_WARNING_MESSAGE", "");
        this.mCheckingGrid.Visibility = Visibility.Collapsed;
      }));
    }

    private void mCheckUpdateBtn_Click(object sender, RoutedEventArgs e)
    {
      if (string.Equals(this.mCheckUpdateBtn.Content.ToString(), LocaleStrings.GetLocalizedString("STRING_DOWNLOAD_UPDATE", ""), StringComparison.InvariantCulture))
      {
        BlueStacksUpdater.DownloadNow(BlueStacksUpdater.sBstUpdateData, false);
        ClientStats.SendBluestacksUpdaterUIStatsAsync(ClientStatsEvent.SettingDownloadUpdate, "");
      }
      else if (string.Equals(this.mCheckUpdateBtn.Content.ToString(), LocaleStrings.GetLocalizedString("STRING_INSTALL_UPDATE", ""), StringComparison.InvariantCulture))
      {
        this.ParentWindow.ShowInstallPopup();
        ClientStats.SendBluestacksUpdaterUIStatsAsync(ClientStatsEvent.SettingInstallUpdate, "");
      }
      else
      {
        if (!string.Equals(this.mCheckUpdateBtn.Content.ToString(), LocaleStrings.GetLocalizedString("STRING_CHECK_UPDATES", ""), StringComparison.InvariantCulture) && !string.Equals(this.mCheckUpdateBtn.Content.ToString(), LocaleStrings.GetLocalizedString("STRING_RETRY_CONNECTION_ISSUE_TEXT1", ""), StringComparison.InvariantCulture))
          return;
        this.ShowCheckingForUpdateGrid();
        BlueStacksUpdater.sCheckUpdateBackgroundWorker.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(this.HandleCheckForUpdateResult);
        BlueStacksUpdater.sCheckUpdateBackgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.HandleCheckForUpdateResult);
        BlueStacksUpdater.SetupBlueStacksUpdater(this.ParentWindow, false);
        ClientStats.SendBluestacksUpdaterUIStatsAsync(ClientStatsEvent.SettingCheckUpdate, "");
      }
    }

    private void HandleCheckForUpdateResult(object sender, RunWorkerCompletedEventArgs e)
    {
      if (BlueStacksUpdater.sBstUpdateData.IsUpdateAvailble)
      {
        this.HandleUpdateStateGridVisibility(BlueStacksUpdater.SUpdateState);
        ClientStats.SendBluestacksUpdaterUIStatsAsync(ClientStatsEvent.SettingUpdateAvailable, "");
      }
      else if (BlueStacksUpdater.sBstUpdateData.IsTryAgain)
      {
        this.ShowInternetConnectionErrorGrid();
      }
      else
      {
        this.ShowLatestVersionGrid();
        ClientStats.SendBluestacksUpdaterUIStatsAsync(ClientStatsEvent.SettingUpdateNotAvailable, "");
      }
    }

    private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
      BlueStacksUIUtils.OpenUrl(e.Uri.OriginalString);
      e.Handled = true;
    }

    private void mTermsOfUseLink_Loaded(object sender, RoutedEventArgs e)
    {
      this.mTermsOfUseLink.Inlines.Clear();
      this.mTermsOfUseLink.Inlines.Add(LocaleStrings.GetLocalizedString("STRING_TERMS_OF_USE_LINK", ""));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/settingswindows/aboutsettingscontrol.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.mPoweredByBSGrid = (Grid) target;
          break;
        case 2:
          this.mBSIconAndNameGrid = (Grid) target;
          break;
        case 3:
          this.mProductTextGrid = (Grid) target;
          break;
        case 4:
          this.mVersionLabel = (Label) target;
          break;
        case 5:
          this.mUpdateInfoGrid = (Grid) target;
          break;
        case 6:
          this.bodyLabel = (Label) target;
          break;
        case 7:
          this.mLabelVersion = (Label) target;
          break;
        case 8:
          this.mDetailedChangeLogs = (Hyperlink) target;
          this.mDetailedChangeLogs.RequestNavigate += new RequestNavigateEventHandler(this.Hyperlink_RequestNavigate);
          break;
        case 9:
          this.mCheckUpdateBtn = (CustomButton) target;
          this.mCheckUpdateBtn.Click += new RoutedEventHandler(this.mCheckUpdateBtn_Click);
          break;
        case 10:
          this.mStatusLabel = (TextBlock) target;
          break;
        case 11:
          this.mCheckingGrid = (Grid) target;
          break;
        case 12:
          this.ContactInfoGrid = (Grid) target;
          break;
        case 13:
          this.mWebsiteLabel = (Label) target;
          break;
        case 14:
          ((Hyperlink) target).RequestNavigate += new RequestNavigateEventHandler(this.Hyperlink_RequestNavigate);
          break;
        case 15:
          this.mSupportLabel = (Label) target;
          break;
        case 16:
          ((Hyperlink) target).RequestNavigate += new RequestNavigateEventHandler(this.Hyperlink_RequestNavigate);
          break;
        case 17:
          this.mSupportMailLabel = (Label) target;
          break;
        case 18:
          this.mSupportEMailHyperlink = (Hyperlink) target;
          this.mSupportEMailHyperlink.RequestNavigate += new RequestNavigateEventHandler(this.Hyperlink_RequestNavigate);
          break;
        case 19:
          this.mTermsOfUse = (TextBlock) target;
          break;
        case 20:
          this.mTermsOfUseLink = (Hyperlink) target;
          this.mTermsOfUseLink.RequestNavigate += new RequestNavigateEventHandler(this.Hyperlink_RequestNavigate);
          this.mTermsOfUseLink.Loaded += new RoutedEventHandler(this.mTermsOfUseLink_Loaded);
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
