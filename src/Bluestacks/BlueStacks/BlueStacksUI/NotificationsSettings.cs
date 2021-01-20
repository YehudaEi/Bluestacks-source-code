// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.NotificationsSettings
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace BlueStacks.BlueStacksUI
{
  public class NotificationsSettings : UserControl, IComponentConnector
  {
    private string mVmName = "Android";
    private MainWindow ParentWindow;
    internal ScrollViewer mScroll;
    internal Grid mNotificationModeSettingsSection;
    internal CustomCheckbox mMinimzeOnCloseCheckBox;
    internal Label mReadMoreSection;
    internal System.Windows.Shapes.Path mCollapsedArrow;
    internal System.Windows.Shapes.Path mExpandededArrow;
    internal CustomToggleButtonWithState mNotificationModeToggleButton;
    internal Border mNotifModeInfoGrid;
    internal CustomToggleButtonWithState mNotificationSoundToggleButton;
    internal CustomPictureBox mRibbonHelp;
    internal CustomToggleButtonWithState mRibbonNotificationsToggleButton;
    internal CustomPopUp mRibbonPopup;
    internal CustomPictureBox mToastHelp;
    internal CustomToggleButtonWithState mToastNotificationsToggleButton;
    internal CustomPopUp mToastPopup;
    internal CustomPictureBox mGamepadNotificationHelp;
    internal CustomToggleButtonWithState mGamepadDesktopNotificationToggle;
    internal CustomPopUp mGamepadNotifPopup;
    internal CustomToggleButtonWithState mAppSpecificNotificationsToggleButton;
    internal Grid mHeaders;
    internal CustomPictureBox mBellHelp;
    internal CustomPopUp mBellPopup;
    internal CustomPictureBox mDesktopHelp;
    internal CustomPopUp mDesktopPopup;
    internal StackPanel mStackPanel;
    internal CustomPictureBox mInfoIcon;
    private bool _contentLoaded;

    public static NotificationsSettings Instance { get; set; }

    public NotificationsSettings(MainWindow window)
    {
      NotificationsSettings.Instance = this;
      this.InitializeComponent();
      this.ParentWindow = window;
      this.Visibility = Visibility.Hidden;
      this.mVmName = window?.mVmName;
      this.mNotificationModeToggleButton.BoolValue = RegistryManager.Instance.IsNotificationModeAlwaysOn;
      this.mNotificationSoundToggleButton.BoolValue = RegistryManager.Instance.IsNotificationSoundsActive;
      this.mRibbonNotificationsToggleButton.BoolValue = RegistryManager.Instance.IsShowRibbonNotification;
      this.mToastNotificationsToggleButton.BoolValue = RegistryManager.Instance.IsShowToastNotification;
      this.mGamepadDesktopNotificationToggle.BoolValue = RegistryManager.Instance.IsShowGamepadDesktopNotification;
      if (!string.Equals(this.mVmName, "Android", StringComparison.InvariantCultureIgnoreCase))
      {
        this.mNotificationModeSettingsSection.Visibility = Visibility.Collapsed;
        this.mMinimzeOnCloseCheckBox.Visibility = Visibility.Collapsed;
      }
      else
      {
        this.mExpandededArrow.Visibility = Visibility.Collapsed;
        this.mNotifModeInfoGrid.Visibility = Visibility.Collapsed;
        this.mCollapsedArrow.Visibility = Visibility.Visible;
      }
      this.mScroll.ScrollChanged += new ScrollChangedEventHandler(BluestacksUIColor.ScrollBarScrollChanged);
      this.mMinimzeOnCloseCheckBox.IsChecked = new bool?(!this.ParentWindow.EngineInstanceRegistry.IsShowMinimizeBlueStacksPopupOnClose);
    }

    private void NotificationSettings_Loaded(object sender, RoutedEventArgs e)
    {
      NotificationManager.Instance.ReloadNotificationDetails();
      List<AppInfo> list = ((IEnumerable<AppInfo>) new JsonParser(this.mVmName).GetAppList()).ToList<AppInfo>();
      bool flag1 = true;
      foreach (AppInfo appInfo in list)
      {
        bool flag2 = !this.AddNotificationToggleButton(appInfo.Name, appInfo.Img, appInfo.Package);
        flag1 &= flag2;
      }
      this.mAppSpecificNotificationsToggleButton.BoolValue = list.Count <= 0 || !flag1;
      if (this.mAppSpecificNotificationsToggleButton.BoolValue)
      {
        this.mStackPanel.Visibility = Visibility.Visible;
        this.mHeaders.Visibility = Visibility.Visible;
      }
      else
      {
        this.mStackPanel.Visibility = Visibility.Collapsed;
        this.mHeaders.Visibility = Visibility.Collapsed;
      }
    }

    private bool AddNotificationToggleButton(string name, string imageName, string package)
    {
      string imagePath = System.IO.Path.Combine(RegistryStrings.GadgetDir, imageName);
      AppNotificationsToggleButton notificationsToggleButton1 = new AppNotificationsToggleButton(this.ParentWindow, name, imagePath, package);
      notificationsToggleButton1.Margin = new Thickness(0.0, 0.0, 0.0, 12.0);
      AppNotificationsToggleButton notificationsToggleButton2 = notificationsToggleButton1;
      this.mStackPanel.Children.Add((UIElement) notificationsToggleButton2);
      return notificationsToggleButton2.mBellNotificationStatus.BoolValue || notificationsToggleButton2.mDesktopNotificationStatus.BoolValue;
    }

    private void CheckBox_Click(object sender, RoutedEventArgs e)
    {
      InstanceRegistry instanceRegistry = this.ParentWindow.EngineInstanceRegistry;
      bool? isChecked = this.mMinimzeOnCloseCheckBox.IsChecked;
      bool flag = true;
      int num = !(isChecked.GetValueOrDefault() == flag & isChecked.HasValue) ? 1 : 0;
      instanceRegistry.IsShowMinimizeBlueStacksPopupOnClose = num != 0;
      BlueStacks.Common.Stats.SendCommonClientStatsAsync("notification_mode", "donotshow_checkbox", this.ParentWindow.mVmName, string.Empty, (!this.ParentWindow.EngineInstanceRegistry.IsShowMinimizeBlueStacksPopupOnClose).ToString((IFormatProvider) CultureInfo.InvariantCulture), "", "");
    }

    private void ReadMoreLinkMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      BlueStacks.Common.Stats.SendCommonClientStatsAsync("notification_mode", "readarticle", this.ParentWindow.mVmName, KMManager.sPackageName, "", "", "");
      Utils.OpenUrl(WebHelper.GetUrlWithParams(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) WebHelper.GetServerHost(), (object) "help_articles"), (string) null, (string) null, (string) null) + "&article=notification_mode_help");
      e.Handled = true;
    }

    private void mReadMoreSection_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      if (this.mCollapsedArrow.Visibility == Visibility.Visible)
      {
        this.mCollapsedArrow.Visibility = Visibility.Collapsed;
        this.mExpandededArrow.Visibility = Visibility.Visible;
        this.mNotifModeInfoGrid.Visibility = Visibility.Visible;
      }
      else
      {
        this.mExpandededArrow.Visibility = Visibility.Collapsed;
        this.mNotifModeInfoGrid.Visibility = Visibility.Collapsed;
        this.mCollapsedArrow.Visibility = Visibility.Visible;
      }
    }

    private void mNotificationModeToggleButton_PreviewMouseLeftButtonUp(
      object sender,
      MouseButtonEventArgs e)
    {
      RegistryManager.Instance.IsNotificationModeAlwaysOn = !this.mNotificationModeToggleButton.BoolValue;
      BlueStacks.Common.Stats.SendCommonClientStatsAsync("notification_mode", RegistryManager.Instance.IsNotificationModeAlwaysOn ? "toggle_on" : "toggle_off", this.ParentWindow.mVmName, "", "", "", "");
    }

    private void mNotificationSoundToggleButton_PreviewMouseLeftButtonUp(
      object sender,
      MouseButtonEventArgs e)
    {
      RegistryManager.Instance.IsNotificationSoundsActive = !this.mNotificationSoundToggleButton.BoolValue;
      BlueStacks.Common.Stats.SendCommonClientStatsAsync("notification_mode", "notification_sound_toggle", this.ParentWindow.mVmName, string.Empty, RegistryManager.Instance.IsNotificationSoundsActive.ToString((IFormatProvider) CultureInfo.InvariantCulture), "", "");
    }

    private void mToastNotificationsToggleButton_PreviewMouseLeftButtonUp(
      object sender,
      MouseButtonEventArgs e)
    {
      RegistryManager.Instance.IsShowToastNotification = !this.mToastNotificationsToggleButton.BoolValue;
      BlueStacks.Common.Stats.SendCommonClientStatsAsync("notification_mode", "toast_notification_toggle", this.ParentWindow.mVmName, string.Empty, RegistryManager.Instance.IsShowToastNotification.ToString((IFormatProvider) CultureInfo.InvariantCulture), "", "");
    }

    private void mRibbonNotificationsToggleButton_PreviewMouseLeftButtonUp(
      object sender,
      MouseButtonEventArgs e)
    {
      RegistryManager.Instance.IsShowRibbonNotification = !this.mRibbonNotificationsToggleButton.BoolValue;
      BlueStacks.Common.Stats.SendCommonClientStatsAsync("notification_mode", "ribbon_notification_toggle", this.ParentWindow.mVmName, string.Empty, RegistryManager.Instance.IsShowRibbonNotification.ToString((IFormatProvider) CultureInfo.InvariantCulture), "", "");
    }

    private void mAppSpecificNotificationsToggleButton_PreviewMouseLeftButtonUp(
      object sender,
      MouseButtonEventArgs e)
    {
      BlueStacks.Common.Stats.SendCommonClientStatsAsync("notification_mode", "all_apps_notifications_muted_toggle", this.ParentWindow.mVmName, string.Empty, (!this.mAppSpecificNotificationsToggleButton.BoolValue).ToString((IFormatProvider) CultureInfo.InvariantCulture), "", "");
      if (!this.mAppSpecificNotificationsToggleButton.BoolValue)
      {
        NotificationManager.Instance.UpdateMuteState(MuteState.NotMuted, NotificationManager.Instance.ShowNotificationText, this.ParentWindow.mVmName);
        this.mStackPanel.Visibility = Visibility.Visible;
        this.mHeaders.Visibility = Visibility.Visible;
      }
      else
      {
        NotificationManager.Instance.UpdateMuteState(MuteState.MutedForever, NotificationManager.Instance.ShowNotificationText, this.ParentWindow.mVmName);
        this.mStackPanel.Visibility = Visibility.Collapsed;
        this.mHeaders.Visibility = Visibility.Collapsed;
      }
      foreach (AppNotificationsToggleButton child in this.mStackPanel.Children)
      {
        child.mBellNotificationStatus.BoolValue = !this.mAppSpecificNotificationsToggleButton.BoolValue;
        if (child.mBellNotificationStatus.BoolValue)
        {
          NotificationManager.Instance.UpdateMuteState(MuteState.NotMuted, child.mAppTitle.Text, this.ParentWindow.mVmName);
          bool flag = false;
          if (PostBootCloudInfoManager.Instance.mPostBootCloudInfo.DesktopNotificationsChatPackages.ChatApplicationPackages != null)
            flag = PostBootCloudInfoManager.Instance.mPostBootCloudInfo.DesktopNotificationsChatPackages.ChatApplicationPackages.IsPackageAvailable(child.PackageName);
          if (flag)
            NotificationManager.Instance.DictNotificationItems[child.mAppTitle.Text].ShowDesktopNotifications = true;
          else
            NotificationManager.Instance.DictNotificationItems[child.mAppTitle.Text].ShowDesktopNotifications = false;
        }
        else
        {
          child.mBellNotificationStatus.BoolValue = !this.mAppSpecificNotificationsToggleButton.BoolValue;
          NotificationManager.Instance.UpdateMuteState(MuteState.MutedForever, child.mAppTitle.Text, this.ParentWindow.mVmName);
          NotificationManager.Instance.DictNotificationItems[child.mAppTitle.Text].ShowDesktopNotifications = false;
        }
        child.mDesktopNotificationStatus.BoolValue = NotificationManager.Instance.DictNotificationItems[child.mAppTitle.Text].ShowDesktopNotifications;
      }
      NotificationManager.Instance.UpdateNotificationsSettings();
    }

    private void mRibbonHelp_MouseEnter(object sender, MouseEventArgs e)
    {
      this.mRibbonPopup.IsOpen = true;
    }

    private void mRibbonHelp_MouseLeave(object sender, MouseEventArgs e)
    {
      this.mRibbonPopup.IsOpen = false;
    }

    private void mToastHelp_MouseEnter(object sender, MouseEventArgs e)
    {
      this.mToastPopup.IsOpen = true;
    }

    private void mToastHelp_MouseLeave(object sender, MouseEventArgs e)
    {
      this.mToastPopup.IsOpen = false;
    }

    private void mGamepadDesktopNotificationToggleButton_PreviewMouseLeftButtonUp(
      object sender,
      MouseButtonEventArgs e)
    {
      RegistryManager.Instance.IsShowGamepadDesktopNotification = !this.mGamepadDesktopNotificationToggle.BoolValue;
      BlueStacks.Common.Stats.SendCommonClientStatsAsync("notification_mode", "gamepad_desktop_notification_toggle", this.ParentWindow.mVmName, string.Empty, RegistryManager.Instance.IsShowGamepadDesktopNotification.ToString((IFormatProvider) CultureInfo.InvariantCulture), "", "");
    }

    private void mGamepadNotificationHelp_MouseEnter(object sender, MouseEventArgs e)
    {
      this.mGamepadNotifPopup.IsOpen = true;
    }

    private void mGamepadNotificationHelp_MouseLeave(object sender, MouseEventArgs e)
    {
      this.mGamepadNotifPopup.IsOpen = false;
    }

    private void mBellHelp_MouseEnter(object sender, MouseEventArgs e)
    {
      this.mBellPopup.IsOpen = true;
    }

    private void mBellHelp_MouseLeave(object sender, MouseEventArgs e)
    {
      this.mBellPopup.IsOpen = false;
    }

    private void mDesktopHelp_MouseEnter(object sender, MouseEventArgs e)
    {
      this.mDesktopPopup.IsOpen = true;
    }

    private void mDesktopHelp_MouseLeave(object sender, MouseEventArgs e)
    {
      this.mDesktopPopup.IsOpen = false;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/settingswindows/notificationssettings.xaml", UriKind.Relative));
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
          ((FrameworkElement) target).Loaded += new RoutedEventHandler(this.NotificationSettings_Loaded);
          break;
        case 2:
          this.mScroll = (ScrollViewer) target;
          break;
        case 3:
          this.mNotificationModeSettingsSection = (Grid) target;
          break;
        case 4:
          this.mMinimzeOnCloseCheckBox = (CustomCheckbox) target;
          this.mMinimzeOnCloseCheckBox.Click += new RoutedEventHandler(this.CheckBox_Click);
          break;
        case 5:
          this.mReadMoreSection = (Label) target;
          this.mReadMoreSection.MouseLeftButtonUp += new MouseButtonEventHandler(this.mReadMoreSection_MouseLeftButtonUp);
          break;
        case 6:
          this.mCollapsedArrow = (System.Windows.Shapes.Path) target;
          break;
        case 7:
          this.mExpandededArrow = (System.Windows.Shapes.Path) target;
          break;
        case 8:
          this.mNotificationModeToggleButton = (CustomToggleButtonWithState) target;
          this.mNotificationModeToggleButton.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.mNotificationModeToggleButton_PreviewMouseLeftButtonUp);
          break;
        case 9:
          this.mNotifModeInfoGrid = (Border) target;
          break;
        case 10:
          ((UIElement) target).MouseLeftButtonDown += new MouseButtonEventHandler(this.ReadMoreLinkMouseLeftButtonUp);
          break;
        case 11:
          this.mNotificationSoundToggleButton = (CustomToggleButtonWithState) target;
          this.mNotificationSoundToggleButton.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.mNotificationSoundToggleButton_PreviewMouseLeftButtonUp);
          break;
        case 12:
          this.mRibbonHelp = (CustomPictureBox) target;
          this.mRibbonHelp.MouseEnter += new MouseEventHandler(this.mRibbonHelp_MouseEnter);
          this.mRibbonHelp.MouseLeave += new MouseEventHandler(this.mRibbonHelp_MouseLeave);
          break;
        case 13:
          this.mRibbonNotificationsToggleButton = (CustomToggleButtonWithState) target;
          this.mRibbonNotificationsToggleButton.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.mRibbonNotificationsToggleButton_PreviewMouseLeftButtonUp);
          break;
        case 14:
          this.mRibbonPopup = (CustomPopUp) target;
          break;
        case 15:
          this.mToastHelp = (CustomPictureBox) target;
          this.mToastHelp.MouseEnter += new MouseEventHandler(this.mToastHelp_MouseEnter);
          this.mToastHelp.MouseLeave += new MouseEventHandler(this.mToastHelp_MouseLeave);
          break;
        case 16:
          this.mToastNotificationsToggleButton = (CustomToggleButtonWithState) target;
          this.mToastNotificationsToggleButton.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.mToastNotificationsToggleButton_PreviewMouseLeftButtonUp);
          break;
        case 17:
          this.mToastPopup = (CustomPopUp) target;
          break;
        case 18:
          this.mGamepadNotificationHelp = (CustomPictureBox) target;
          this.mGamepadNotificationHelp.MouseEnter += new MouseEventHandler(this.mGamepadNotificationHelp_MouseEnter);
          this.mGamepadNotificationHelp.MouseLeave += new MouseEventHandler(this.mGamepadNotificationHelp_MouseLeave);
          break;
        case 19:
          this.mGamepadDesktopNotificationToggle = (CustomToggleButtonWithState) target;
          this.mGamepadDesktopNotificationToggle.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.mGamepadDesktopNotificationToggleButton_PreviewMouseLeftButtonUp);
          break;
        case 20:
          this.mGamepadNotifPopup = (CustomPopUp) target;
          break;
        case 21:
          this.mAppSpecificNotificationsToggleButton = (CustomToggleButtonWithState) target;
          this.mAppSpecificNotificationsToggleButton.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.mAppSpecificNotificationsToggleButton_PreviewMouseLeftButtonUp);
          break;
        case 22:
          this.mHeaders = (Grid) target;
          break;
        case 23:
          this.mBellHelp = (CustomPictureBox) target;
          this.mBellHelp.MouseEnter += new MouseEventHandler(this.mBellHelp_MouseEnter);
          this.mBellHelp.MouseLeave += new MouseEventHandler(this.mBellHelp_MouseLeave);
          break;
        case 24:
          this.mBellPopup = (CustomPopUp) target;
          break;
        case 25:
          this.mDesktopHelp = (CustomPictureBox) target;
          this.mDesktopHelp.MouseEnter += new MouseEventHandler(this.mDesktopHelp_MouseEnter);
          this.mDesktopHelp.MouseLeave += new MouseEventHandler(this.mDesktopHelp_MouseLeave);
          break;
        case 26:
          this.mDesktopPopup = (CustomPopUp) target;
          break;
        case 27:
          this.mStackPanel = (StackPanel) target;
          break;
        case 28:
          this.mInfoIcon = (CustomPictureBox) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
