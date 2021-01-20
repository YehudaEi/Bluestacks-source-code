// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.UpdatePrompt
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Navigation;

namespace BlueStacks.BlueStacksUI
{
  public class UpdatePrompt : UserControl, IComponentConnector
  {
    private BlueStacksUpdateData mBstUpdateData;
    internal Label titleLabel;
    internal CustomPictureBox mCloseBtn;
    internal Label bodyLabel;
    internal Label mLabelVersion;
    internal Hyperlink mDetailedChangeLogs;
    internal CustomButton mDownloadNewButton;
    private bool _contentLoaded;

    internal UpdatePrompt(BlueStacksUpdateData bstUpdateData)
    {
      this.InitializeComponent();
      this.mBstUpdateData = bstUpdateData;
      if (string.IsNullOrEmpty(bstUpdateData.EngineVersion))
      {
        this.mLabelVersion.Content = (object) "";
        this.mLabelVersion.Visibility = Visibility.Collapsed;
      }
      else
        this.mLabelVersion.Content = (object) ("v" + bstUpdateData.EngineVersion);
      BlueStacksUIBinding.Bind(this.titleLabel, "STRING_BLUESTACKS_UPDATE_AVAILABLE");
      BlueStacksUIBinding.Bind(this.bodyLabel, "STRING_UPDATE_AVAILABLE");
      BlueStacksUIBinding.Bind((Button) this.mDownloadNewButton, "STRING_DOWNLOAD_UPDATE");
      this.mCloseBtn.Visibility = Visibility.Visible;
      this.mDetailedChangeLogs.NavigateUri = new Uri(bstUpdateData.DetailedChangeLogsUrl);
      this.mDetailedChangeLogs.Inlines.Clear();
      this.mDetailedChangeLogs.Inlines.Add(LocaleStrings.GetLocalizedString("STRING_LEARN_WHATS_NEW", "Learn What's New"));
    }

    private void CloseBtn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      Logger.Info("Clicked UpdateNow Menu Close button");
      RegistryManager.Instance.LastUpdateSkippedVersion = this.mBstUpdateData.EngineVersion;
      ClientStats.SendBluestacksUpdaterUIStatsAsync(ClientStatsEvent.UpgradePopupCross, "");
      BlueStacksUIUtils.CloseContainerWindow((FrameworkElement) this);
    }

    private void DownloadNowButton_Click(object sender, RoutedEventArgs e)
    {
      Logger.Info("Clicked Download_Now button");
      ClientStats.SendBluestacksUpdaterUIStatsAsync(ClientStatsEvent.UpgradePopupDwnld, "");
      BlueStacksUpdater.DownloadNow(this.mBstUpdateData, false);
      BlueStacksUIUtils.CloseContainerWindow((FrameworkElement) this);
    }

    private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
      BlueStacksUIUtils.OpenUrl(e.Uri.OriginalString);
      e.Handled = true;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/updateprompt.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.titleLabel = (Label) target;
          break;
        case 2:
          this.mCloseBtn = (CustomPictureBox) target;
          this.mCloseBtn.MouseLeftButtonUp += new MouseButtonEventHandler(this.CloseBtn_MouseLeftButtonUp);
          break;
        case 3:
          this.bodyLabel = (Label) target;
          break;
        case 4:
          this.mLabelVersion = (Label) target;
          break;
        case 5:
          this.mDetailedChangeLogs = (Hyperlink) target;
          this.mDetailedChangeLogs.RequestNavigate += new RequestNavigateEventHandler(this.Hyperlink_RequestNavigate);
          break;
        case 6:
          this.mDownloadNewButton = (CustomButton) target;
          this.mDownloadNewButton.Click += new RoutedEventHandler(this.DownloadNowButton_Click);
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
