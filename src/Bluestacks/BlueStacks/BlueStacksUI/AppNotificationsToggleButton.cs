// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.AppNotificationsToggleButton
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace BlueStacks.BlueStacksUI
{
  public class AppNotificationsToggleButton : UserControl, IComponentConnector
  {
    internal CustomPictureBox mAppIcon;
    internal TextBlock mAppTitle;
    internal CustomToggleButtonWithState mBellNotificationStatus;
    internal CustomToggleButtonWithState mDesktopNotificationStatus;
    private bool _contentLoaded;

    public AppNotificationsToggleButton(
      MainWindow window,
      string name,
      string imagePath,
      string packageName)
    {
      this.InitializeComponent();
      this.ParentWindow = window;
      this.PackageName = packageName;
      this.mAppTitle.Text = name;
      this.mAppIcon.Source = (ImageSource) CustomPictureBox.GetBitmapImage(imagePath, "", true);
      if (NotificationManager.Instance.DictNotificationItems.Keys.Contains<string>(name))
      {
        this.mBellNotificationStatus.BoolValue = NotificationManager.Instance.DictNotificationItems[name].MuteState != MuteState.MutedForever;
        if (NotificationManager.Instance.DictNotificationItems[name].ShowDesktopNotifications)
          this.mDesktopNotificationStatus.BoolValue = true;
        else
          this.mDesktopNotificationStatus.BoolValue = false;
      }
      else
      {
        MuteState defaultState = NotificationManager.Instance.GetDefaultState(this.ParentWindow?.mVmName);
        bool isShowDesktopNotifications = false;
        if (PostBootCloudInfoManager.Instance.mPostBootCloudInfo?.DesktopNotificationsChatPackages?.ChatApplicationPackages != null)
          isShowDesktopNotifications = PostBootCloudInfoManager.Instance.mPostBootCloudInfo.DesktopNotificationsChatPackages.ChatApplicationPackages.IsPackageAvailable(this.PackageName);
        NotificationManager.Instance.DictNotificationItems.Add(name, new NotificationItem(name, defaultState, DateTime.Now, isShowDesktopNotifications));
        NotificationManager.Instance.UpdateNotificationsSettings();
        this.mBellNotificationStatus.BoolValue = defaultState != MuteState.MutedForever;
        if (NotificationManager.Instance.DictNotificationItems[name].ShowDesktopNotifications)
          this.mDesktopNotificationStatus.BoolValue = true;
        else
          this.mDesktopNotificationStatus.BoolValue = false;
      }
    }

    public MainWindow ParentWindow { get; private set; }

    public string PackageName { get; private set; }

    private void mBellNotificationStatus_PreviewMouseLeftButtonUp(
      object sender,
      MouseButtonEventArgs e)
    {
      if (this.mBellNotificationStatus.BoolValue)
      {
        NotificationManager.Instance.UpdateMuteState(MuteState.MutedForever, this.mAppTitle.Text, this.ParentWindow.mVmName);
        BlueStacks.Common.Stats.SendCommonClientStatsAsync("notification_mode", "app_notifications_preferences", this.ParentWindow.mVmName, this.PackageName, "Mute", "", "");
      }
      else
      {
        NotificationManager.Instance.UpdateMuteState(MuteState.NotMuted, this.mAppTitle.Text, this.ParentWindow.mVmName);
        BlueStacks.Common.Stats.SendCommonClientStatsAsync("notification_mode", "app_notifications_preferences", this.ParentWindow.mVmName, this.PackageName, "UnMute", "", "");
      }
    }

    private void mDesktopNotificationStatus_PreviewMouseLeftButtonUp(
      object sender,
      MouseButtonEventArgs e)
    {
      if (this.mDesktopNotificationStatus.BoolValue)
      {
        NotificationManager.Instance.DictNotificationItems[this.mAppTitle.Text].ShowDesktopNotifications = false;
        BlueStacks.Common.Stats.SendCommonClientStatsAsync("notification_mode", "show_desktop_notification", this.ParentWindow.mVmName, this.PackageName, "Mute", "", "");
      }
      else
      {
        NotificationManager.Instance.DictNotificationItems[this.mAppTitle.Text].ShowDesktopNotifications = true;
        BlueStacks.Common.Stats.SendCommonClientStatsAsync("notification_mode", "show_desktop_notification", this.ParentWindow.mVmName, this.PackageName, "UnMute", "", "");
      }
      NotificationManager.Instance.UpdateNotificationsSettings();
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/settingswindows/appnotificationstogglebutton.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.mAppIcon = (CustomPictureBox) target;
          break;
        case 2:
          this.mAppTitle = (TextBlock) target;
          break;
        case 3:
          this.mBellNotificationStatus = (CustomToggleButtonWithState) target;
          this.mBellNotificationStatus.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.mBellNotificationStatus_PreviewMouseLeftButtonUp);
          break;
        case 4:
          this.mDesktopNotificationStatus = (CustomToggleButtonWithState) target;
          this.mDesktopNotificationStatus.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.mDesktopNotificationStatus_PreviewMouseLeftButtonUp);
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
