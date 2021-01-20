// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.NotificationDrawerItem
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace BlueStacks.BlueStacksUI
{
  public class NotificationDrawerItem : UserControl, IComponentConnector
  {
    internal string Id;
    private MainWindow ParentWindow;
    internal CustomPictureBox icon;
    internal TextBlock titleText;
    internal TextBlock dateText;
    internal Grid mNotificationActions;
    internal CustomPictureBox mSnoozeBtn;
    internal CustomPopUp mMutePopup;
    internal TextBlock mLbl1Hour;
    internal TextBlock mLbl1Day;
    internal TextBlock mLbl1Week;
    internal TextBlock mLblForever;
    internal CustomPictureBox mCloseBtn;
    internal TextBlock messageText;
    private bool _contentLoaded;

    public NotificationDrawerItem()
    {
      this.InitializeComponent();
    }

    public string PackageName { get; set; }

    internal void InitFromGenricNotificationItem(GenericNotificationItem item, MainWindow parentWin)
    {
      this.ParentWindow = parentWin;
      this.Id = item.Id;
      this.PackageName = item.Package;
      this.titleText.Text = item.Title;
      this.messageText.Text = item.Message;
      if (!item.IsRead)
        this.ChangeToUnreadBackground();
      else
        this.ChangeToReadBackground();
      if (string.Equals(item.Title, Strings.ProductDisplayName, StringComparison.InvariantCultureIgnoreCase))
      {
        this.mSnoozeBtn.IsEnabled = false;
        this.mSnoozeBtn.Opacity = 0.5;
      }
      if (!string.IsNullOrEmpty(item.NotificationMenuImageName) && !string.IsNullOrEmpty(item.NotificationMenuImageUrl) && !File.Exists(Path.Combine(RegistryStrings.PromotionDirectory, item.NotificationMenuImageName)))
        item.NotificationMenuImageName = Utils.TinyDownloader(item.NotificationMenuImageUrl, item.NotificationMenuImageName, RegistryStrings.PromotionDirectory, false);
      this.icon.ImageName = item.NotificationMenuImageName;
      this.dateText.Text = DateTimeHelper.GetReadableDateTimeString(item.CreationTime);
    }

    private void UserControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      string fileName = RegistryStrings.InstallDir + "\\HD-RunApp.exe";
      GenericNotificationItem notificationItem = GenericNotificationManager.Instance.GetNotificationItem(this.Id);
      JsonParser jsonParser = new JsonParser(this.ParentWindow.mVmName);
      if (this.ParentWindow != null && this.ParentWindow.mGuestBootCompleted)
      {
        if (notificationItem == null)
          return;
        ClientStats.SendMiscellaneousStatsAsync("NotificationDrawerItemClicked", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, notificationItem.Id, notificationItem.Title, JsonConvert.SerializeObject((object) notificationItem.ExtraPayload), notificationItem.ExtraPayload.ContainsKey("campaign_id") ? notificationItem.ExtraPayload["campaign_id"] : "", (string) null, (string) null, "Android");
        GenericNotificationManager.MarkNotification((IEnumerable<string>) new List<string>()
        {
          notificationItem.Id
        }, (System.Action<GenericNotificationItem>) (x => x.IsRead = true));
        this.ChangeToReadBackground();
        this.ParentWindow.mTopBar.RefreshNotificationCentreButton();
        if (notificationItem.ExtraPayload.Keys.Count > 0)
        {
          this.ParentWindow.Utils.HandleGenericActionFromDictionary((Dictionary<string, string>) notificationItem.ExtraPayload, "notification_drawer", notificationItem.NotificationMenuImageName);
        }
        else
        {
          try
          {
            if (string.Compare(notificationItem.Title, "Successfully copied files:", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(notificationItem.Title, "Cannot copy files:", StringComparison.OrdinalIgnoreCase) == 0)
            {
              NotificationPopup.LaunchExplorer(notificationItem.Message);
            }
            else
            {
              Logger.Info("launching " + notificationItem.Title);
              AppInfo infoFromPackageName = jsonParser.GetAppInfoFromPackageName(this.PackageName);
              if (infoFromPackageName != null)
              {
                string str = "-json \"" + new JObject()
                {
                  {
                    "app_icon_url",
                    (JToken) ""
                  },
                  {
                    "app_name",
                    (JToken) infoFromPackageName.Name
                  },
                  {
                    "app_url",
                    (JToken) ""
                  },
                  {
                    "app_pkg",
                    (JToken) this.PackageName
                  }
                }.ToString(Formatting.None).Replace("\"", "\\\"") + "\"";
                Process.Start(fileName, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} -vmname {1}", (object) str, (object) this.ParentWindow.mVmName));
              }
              else
                this.ParentWindow.Utils.HandleGenericActionFromDictionary(new Dictionary<string, string>()
                {
                  {
                    "click_generic_action",
                    GenericAction.InstallPlay.ToString()
                  },
                  {
                    "click_action_packagename",
                    notificationItem.Package
                  }
                }, "notification_drawer", "");
            }
          }
          catch (Exception ex)
          {
            Logger.Error(ex.ToString());
          }
          finally
          {
            this.ParentWindow.mTopBar.mNotificationCentrePopup.IsOpen = false;
          }
        }
      }
      else
      {
        if (notificationItem == null)
          return;
        this.ParentWindow.mPostBootNotificationAction = this.PackageName;
        this.ParentWindow.mTopBar.mNotificationCentrePopup.IsOpen = false;
      }
    }

    internal void ChangeToUnreadBackground()
    {
      this.Background = (Brush) Brushes.Transparent;
    }

    internal void ChangeToReadBackground()
    {
      this.Opacity = 0.5;
      this.Background = (Brush) Brushes.Transparent;
    }

    private void UserControl_MouseEnter(object sender, MouseEventArgs e)
    {
      this.mNotificationActions.Visibility = Visibility.Visible;
      BlueStacksUIBinding.BindColor((DependencyObject) this, Control.BackgroundProperty, "ContextMenuItemBackgroundHoverColor");
    }

    private void UserControl_MouseLeave(object sender, MouseEventArgs e)
    {
      if (this.mMutePopup.IsOpen)
        return;
      this.mNotificationActions.Visibility = Visibility.Collapsed;
      if (GenericNotificationManager.Instance.GetNotificationItem(this.Id) == null)
        return;
      if (!GenericNotificationManager.Instance.GetNotificationItem(this.Id).IsRead)
        this.ChangeToUnreadBackground();
      else
        this.ChangeToReadBackground();
    }

    private void mCloseBtn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.ParentWindow.mTopBar.mNotificationDrawerControl.RemoveNotificationItem(this.Id);
      e.Handled = true;
    }

    private void Grid_MouseLeave(object sender, MouseEventArgs e)
    {
      BlueStacksUIBinding.BindColor((DependencyObject) (sender as Grid), Panel.BackgroundProperty, "ContextMenuItemBackgroundColor");
    }

    private void Grid_MouseEnter(object sender, MouseEventArgs e)
    {
      BlueStacksUIBinding.BindColor((DependencyObject) (sender as Grid), Panel.BackgroundProperty, "ContextMenuItemBackgroundHoverColor");
    }

    private void Lbl1Hour_MouseUp(object sender, MouseButtonEventArgs e)
    {
      NotificationManager.Instance.UpdateMuteState(MuteState.MutedFor1Hour, this.titleText.Text, this.ParentWindow.mVmName);
      string packageName;
      new JsonParser(this.ParentWindow.mVmName).GetAppInfoFromAppName(this.titleText.Text, out packageName, out string _, out string _);
      BlueStacks.Common.Stats.SendCommonClientStatsAsync("notification_mode", "app_notifications_snoozed", this.ParentWindow.mVmName, packageName, "Muted_" + (sender as TextBlock).Text, "bell_notification", "");
      this.mMutePopup.IsOpen = false;
      this.ParentWindow.mTopBar.mNotificationDrawerControl.RemoveNotificationItem(this.Id);
      this.ParentWindow.mTopBar.mNotificationDrawerControl.mSnoozeInfoGrid.Visibility = Visibility.Visible;
      this.ParentWindow.mTopBar.mNotificationDrawerControl.mSnoozeInfoBlock.Text = string.Format((IFormatProvider) CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_PACKAGE_SNOOZED", ""), (object) this.titleText.Text, (object) (sender as TextBlock).Text);
      NotificationDrawer.SnoozeInfoGridTimer.Start();
      e.Handled = true;
    }

    private void Lbl1Day_MouseUp(object sender, MouseButtonEventArgs e)
    {
      NotificationManager.Instance.UpdateMuteState(MuteState.MutedFor1Day, this.titleText.Text, this.ParentWindow.mVmName);
      string packageName;
      new JsonParser(this.ParentWindow.mVmName).GetAppInfoFromAppName(this.titleText.Text, out packageName, out string _, out string _);
      BlueStacks.Common.Stats.SendCommonClientStatsAsync("notification_mode", "app_notifications_snoozed", this.ParentWindow.mVmName, packageName, "Muted_" + (sender as TextBlock).Text, "bell_notification", "");
      this.mMutePopup.IsOpen = false;
      this.ParentWindow.mTopBar.mNotificationDrawerControl.RemoveNotificationItem(this.Id);
      this.ParentWindow.mTopBar.mNotificationDrawerControl.mSnoozeInfoGrid.Visibility = Visibility.Visible;
      this.ParentWindow.mTopBar.mNotificationDrawerControl.mSnoozeInfoBlock.Text = string.Format((IFormatProvider) CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_PACKAGE_SNOOZED", ""), (object) this.titleText.Text, (object) (sender as TextBlock).Text);
      NotificationDrawer.SnoozeInfoGridTimer.Start();
      e.Handled = true;
    }

    private void Lbl1Week_MouseUp(object sender, MouseButtonEventArgs e)
    {
      NotificationManager.Instance.UpdateMuteState(MuteState.MutedFor1Week, this.titleText.Text, this.ParentWindow.mVmName);
      string packageName;
      new JsonParser(this.ParentWindow.mVmName).GetAppInfoFromAppName(this.titleText.Text, out packageName, out string _, out string _);
      BlueStacks.Common.Stats.SendCommonClientStatsAsync("notification_mode", "app_notifications_snoozed", this.ParentWindow.mVmName, packageName, "Muted_" + (sender as TextBlock).Text, "bell_notification", "");
      this.mMutePopup.IsOpen = false;
      this.ParentWindow.mTopBar.mNotificationDrawerControl.RemoveNotificationItem(this.Id);
      this.ParentWindow.mTopBar.mNotificationDrawerControl.mSnoozeInfoGrid.Visibility = Visibility.Visible;
      this.ParentWindow.mTopBar.mNotificationDrawerControl.mSnoozeInfoBlock.Text = string.Format((IFormatProvider) CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_PACKAGE_SNOOZED", ""), (object) this.titleText.Text, (object) (sender as TextBlock).Text);
      NotificationDrawer.SnoozeInfoGridTimer.Start();
      e.Handled = true;
    }

    private void LblForever_MouseUp(object sender, MouseButtonEventArgs e)
    {
      NotificationManager.Instance.UpdateMuteState(MuteState.MutedForever, this.titleText.Text, this.ParentWindow.mVmName);
      if (NotificationManager.Instance.DictNotificationItems.ContainsKey(this.titleText.Text))
        NotificationManager.Instance.DictNotificationItems[this.titleText.Text].ShowDesktopNotifications = false;
      NotificationManager.Instance.UpdateNotificationsSettings();
      string packageName;
      new JsonParser(this.ParentWindow.mVmName).GetAppInfoFromAppName(this.titleText.Text, out packageName, out string _, out string _);
      BlueStacks.Common.Stats.SendCommonClientStatsAsync("notification_mode", "app_notifications_snoozed", this.ParentWindow.mVmName, packageName, "Muted_" + (sender as TextBlock).Text, "bell_notification", "");
      this.mMutePopup.IsOpen = false;
      this.ParentWindow.mTopBar.mNotificationDrawerControl.RemoveNotificationItem(this.Id);
      this.ParentWindow.mTopBar.mNotificationDrawerControl.mSnoozeInfoGrid.Visibility = Visibility.Visible;
      this.ParentWindow.mTopBar.mNotificationDrawerControl.mSnoozeInfoBlock.Text = string.Format((IFormatProvider) CultureInfo.InvariantCulture, LocaleStrings.GetLocalizedString("STRING_PACKAGE_SNOOZED", ""), (object) this.titleText.Text, (object) (sender as TextBlock).Text);
      NotificationDrawer.SnoozeInfoGridTimer.Start();
      e.Handled = true;
    }

    private void mSnoozeBtn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.mMutePopup.IsOpen = !this.mMutePopup.IsOpen;
      e.Handled = true;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/genericnotification/notificationdraweritem.xaml", UriKind.Relative));
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
        case 1:
          ((UIElement) target).MouseLeftButtonUp += new MouseButtonEventHandler(this.UserControl_MouseLeftButtonUp);
          ((UIElement) target).MouseEnter += new MouseEventHandler(this.UserControl_MouseEnter);
          ((UIElement) target).MouseLeave += new MouseEventHandler(this.UserControl_MouseLeave);
          break;
        case 2:
          this.icon = (CustomPictureBox) target;
          break;
        case 3:
          this.titleText = (TextBlock) target;
          break;
        case 4:
          this.dateText = (TextBlock) target;
          break;
        case 5:
          this.mNotificationActions = (Grid) target;
          break;
        case 6:
          this.mSnoozeBtn = (CustomPictureBox) target;
          this.mSnoozeBtn.MouseLeftButtonUp += new MouseButtonEventHandler(this.mSnoozeBtn_MouseLeftButtonUp);
          break;
        case 7:
          this.mMutePopup = (CustomPopUp) target;
          break;
        case 8:
          ((UIElement) target).MouseEnter += new MouseEventHandler(this.Grid_MouseEnter);
          ((UIElement) target).MouseLeave += new MouseEventHandler(this.Grid_MouseLeave);
          break;
        case 9:
          this.mLbl1Hour = (TextBlock) target;
          this.mLbl1Hour.MouseUp += new MouseButtonEventHandler(this.Lbl1Hour_MouseUp);
          break;
        case 10:
          ((UIElement) target).MouseEnter += new MouseEventHandler(this.Grid_MouseEnter);
          ((UIElement) target).MouseLeave += new MouseEventHandler(this.Grid_MouseLeave);
          break;
        case 11:
          this.mLbl1Day = (TextBlock) target;
          this.mLbl1Day.MouseUp += new MouseButtonEventHandler(this.Lbl1Day_MouseUp);
          break;
        case 12:
          ((UIElement) target).MouseEnter += new MouseEventHandler(this.Grid_MouseEnter);
          ((UIElement) target).MouseLeave += new MouseEventHandler(this.Grid_MouseLeave);
          break;
        case 13:
          this.mLbl1Week = (TextBlock) target;
          this.mLbl1Week.MouseUp += new MouseButtonEventHandler(this.Lbl1Week_MouseUp);
          break;
        case 14:
          ((UIElement) target).MouseEnter += new MouseEventHandler(this.Grid_MouseEnter);
          ((UIElement) target).MouseLeave += new MouseEventHandler(this.Grid_MouseLeave);
          break;
        case 15:
          this.mLblForever = (TextBlock) target;
          this.mLblForever.MouseUp += new MouseButtonEventHandler(this.LblForever_MouseUp);
          break;
        case 16:
          this.mCloseBtn = (CustomPictureBox) target;
          this.mCloseBtn.MouseLeftButtonUp += new MouseButtonEventHandler(this.mCloseBtn_MouseLeftButtonUp);
          break;
        case 17:
          this.messageText = (TextBlock) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
