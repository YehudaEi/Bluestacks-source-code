// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.NotificationPopup
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

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
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace BlueStacks.Common
{
  public class NotificationPopup : System.Windows.Controls.UserControl, IDisposable, IComponentConnector
  {
    private Timer mTimer = new Timer();
    private string mTitle = string.Empty;
    private int mDuration = int.MinValue;
    private MouseButtonEventHandler mClickHandler;
    private static EventHandler mSettingsImageClickedHandler;
    private static object mSettingsImageClickedEventData;
    private bool disposedValue;
    internal NotificationPopup mPopupConrol;
    internal Popup mPopup;
    internal Border mImageOuterBorder;
    internal Border RestoreBtnInnerBorder;
    internal StackPanel mRestoreBtnGrid;
    internal Image mImage;
    internal TextBlock mLblHeader;
    internal TextBlock mLblContent;
    internal CustomButton mButton;
    internal CustomPictureBox mImgMute;
    internal Grid PopUpGrid;
    internal CustomPopUp mMutePopup;
    internal Grid mOuterGridPopUp;
    internal Border mMaskBorder2;
    internal TextBlock mLbl1Hour;
    internal TextBlock mLbl1Day;
    internal TextBlock mLbl1Week;
    internal TextBlock mLblForever;
    internal CustomPictureBox mImgSettings;
    internal CustomPictureBox mImgDismiss;
    private bool _contentLoaded;

    public string Title
    {
      get
      {
        return this.mTitle;
      }
      set
      {
        this.mTitle = value;
        this.mLblHeader.Text = this.mTitle;
      }
    }

    public bool AutoClose
    {
      get
      {
        return this.mTimer.Enabled;
      }
      set
      {
        this.mTimer.Enabled = value;
      }
    }

    public int Duration
    {
      get
      {
        return this.mDuration;
      }
      set
      {
        this.mDuration = value;
        this.mTimer.Interval = this.mDuration;
      }
    }

    private string AppName { get; set; } = string.Empty;

    public string VmName { get; set; } = string.Empty;

    public string PackageName { get; set; } = string.Empty;

    public string AndroidNotificationId { get; private set; }

    private void MyTimer_Tick(object sender, EventArgs e)
    {
      this.mTimer.Enabled = false;
      if (this.mMutePopup.IsOpen)
        this.mMutePopup.Closed += new EventHandler(this.MutePopup_Closed);
      else if (this.mPopup.IsMouseOver)
        this.mPopup.MouseLeave += new System.Windows.Input.MouseEventHandler(this.PopupConrol_MouseLeave);
      else
        this.Close();
    }

    private void SetProperties()
    {
      this.mImgSettings.ToolTip = (object) LocaleStrings.GetLocalizedString("STRING_MANAGE_NOTIFICATION", "");
      this.mImgMute.ToolTip = (object) LocaleStrings.GetLocalizedString("STRING_MUTE_NOTIFICATION_TOOLTIP", "");
      this.mImgDismiss.ToolTip = (object) LocaleStrings.GetLocalizedString("STRING_DISMISS_TOOLTIP", "");
      this.mLbl1Hour.Text = LocaleStrings.GetLocalizedString("STRING_HOUR", "");
      this.mLbl1Day.Text = LocaleStrings.GetLocalizedString("STRING_DAY", "");
      this.mLbl1Week.Text = LocaleStrings.GetLocalizedString("STRING_WEEK", "");
      this.mLblForever.Text = LocaleStrings.GetLocalizedString("STRING_FOREVER", "");
    }

    private NotificationPopup(
      string imagePath,
      string title,
      string displayMsg,
      bool autoClose,
      int duration,
      MouseButtonEventHandler clickHandler,
      bool hideMute,
      string vmName,
      MouseButtonEventHandler buttonClickHandler = null,
      MouseButtonEventHandler closeButtonHandler = null,
      MouseButtonEventHandler muteButtonHandler = null,
      bool showOnlyMute = false,
      string buttonText = "",
      string id = "0",
      bool showOnlySettings = false,
      string package = "")
    {
      this.InitializeComponent();
      this.SetProperties();
      this.Width = NotificationWindow.Instance.ActualWidth;
      this.mTimer.Tick += new EventHandler(this.MyTimer_Tick);
      this.mTimer.Interval = duration;
      this.Title = title;
      this.mLblContent.Text = displayMsg;
      this.VmName = vmName;
      this.PackageName = package;
      this.AndroidNotificationId = id;
      if (clickHandler != null)
      {
        this.mPopup.MouseUp += clickHandler;
        this.mClickHandler = clickHandler;
      }
      if (buttonClickHandler != null)
      {
        this.mButton.PreviewMouseLeftButtonUp += buttonClickHandler;
        this.mButton.Visibility = Visibility.Visible;
        if (!string.IsNullOrEmpty(buttonText))
          this.mButton.Content = (object) buttonText;
      }
      if (hideMute)
      {
        this.mImgMute.Visibility = Visibility.Hidden;
        this.mImgSettings.Visibility = Visibility.Hidden;
      }
      if (showOnlyMute)
      {
        this.mImgMute.Visibility = Visibility.Visible;
        this.mImgSettings.Visibility = Visibility.Collapsed;
      }
      if (showOnlySettings)
      {
        this.mImgMute.Visibility = Visibility.Collapsed;
        this.mImgSettings.Visibility = Visibility.Visible;
      }
      if (closeButtonHandler != null)
        this.mImgDismiss.MouseLeftButtonUp += closeButtonHandler;
      if (muteButtonHandler != null)
        this.mOuterGridPopUp.PreviewMouseLeftButtonUp += muteButtonHandler;
      this.AutoClose = autoClose;
      if (!NotificationWindow.Instance.AppNotificationCountDictForEachVM.ContainsKey(this.VmName))
        NotificationWindow.Instance.AppNotificationCountDictForEachVM[this.VmName] = new Dictionary<string, int>();
      if (!NotificationWindow.Instance.AppNotificationCountDictForEachVM[this.VmName].ContainsKey(this.Title))
        NotificationWindow.Instance.AppNotificationCountDictForEachVM[this.VmName].Add(this.Title, 0);
      NotificationWindow.Instance.AppNotificationCountDictForEachVM[this.VmName][this.Title]++;
      JsonParser jsonParser = new JsonParser(vmName);
      this.AppName = string.IsNullOrEmpty(jsonParser.GetAppNameFromPackage(this.PackageName)) ? title : jsonParser.GetAppNameFromPackage(this.PackageName);
      CustomPictureBox.SetBitmapImage(this.mImage, "bluestackslogo", false);
      if (!string.IsNullOrEmpty(imagePath))
      {
        CustomPictureBox.SetBitmapImage(this.mImage, imagePath, true);
      }
      else
      {
        try
        {
          if (string.IsNullOrEmpty(this.AppName))
            return;
          AppInfo infoFromPackageName = jsonParser.GetAppInfoFromPackageName(this.PackageName);
          Logger.Info("For notification {0}: AppName-{1} Package-{2}", (object) id, (object) this.AppName, (object) this.PackageName);
          if (infoFromPackageName == null)
            return;
          Logger.Info("For notification {0}: ImageName-{1}", (object) id, (object) infoFromPackageName.Img);
          if (!File.Exists(Path.Combine(RegistryStrings.GadgetDir, infoFromPackageName.Img)))
            return;
          CustomPictureBox.SetBitmapImage(this.mImage, Path.Combine(RegistryStrings.GadgetDir, infoFromPackageName.Img), true);
        }
        catch
        {
          Logger.Error("Error loading app icon file");
        }
      }
    }

    public static void SettingsImageClickedHandle(EventHandler handle, object data = null)
    {
      NotificationPopup.mSettingsImageClickedHandler = handle;
      NotificationPopup.mSettingsImageClickedEventData = data;
    }

    internal static NotificationPopup InitPopup(
      string imagePath,
      string title,
      string displayMsg,
      bool autoClose,
      int duration,
      MouseButtonEventHandler clickHandler,
      bool hideMute,
      string vmName,
      MouseButtonEventHandler buttonClickHandler = null,
      MouseButtonEventHandler closeButtonHandler = null,
      MouseButtonEventHandler muteButtonHandler = null,
      bool showOnlyMute = false,
      string buttonText = "",
      string id = "0",
      bool showOnlySettings = false,
      string package = "")
    {
      return new NotificationPopup(imagePath, title, displayMsg, autoClose, duration, clickHandler, hideMute, vmName, buttonClickHandler, closeButtonHandler, muteButtonHandler, showOnlyMute, buttonText, id, showOnlySettings, package);
    }

    internal void UpdatePopup(
      string displayMsg,
      bool autoClose,
      int duration,
      MouseButtonEventHandler clickHandler)
    {
      if (autoClose)
        this.Duration = duration;
      this.mLblContent.Text = displayMsg;
      if (this.mClickHandler != null)
        this.mPopup.MouseUp -= this.mClickHandler;
      if (clickHandler == null)
        return;
      this.mPopup.MouseUp += clickHandler;
      this.mClickHandler = clickHandler;
    }

    private void mPopupConrol_LayoutUpdated(object sender, EventArgs e)
    {
      ++this.mPopup.VerticalOffset;
      --this.mPopup.VerticalOffset;
      ++this.mMutePopup.VerticalOffset;
      --this.mMutePopup.VerticalOffset;
    }

    private void ImgMute_MouseUp(object sender, MouseButtonEventArgs e)
    {
      this.mPopup.MouseLeave -= new System.Windows.Input.MouseEventHandler(this.PopupConrol_MouseLeave);
      this.mMutePopup.IsOpen = !this.mMutePopup.IsOpen;
      e.Handled = true;
    }

    private void ImgDismiss_MouseUp(object sender, MouseButtonEventArgs e)
    {
      this.Close();
      e.Handled = true;
    }

    private void Close()
    {
      this.mMutePopup.IsOpen = false;
      this.mTimer.Enabled = false;
      NotificationWindow.Instance.RemovePopup(this);
    }

    public void StopTimer()
    {
      this.mTimer.Enabled = false;
    }

    private void ImgSetting_MouseUp(object sender, MouseButtonEventArgs e)
    {
      this.Close();
      if (NotificationPopup.mSettingsImageClickedHandler != null)
        NotificationPopup.mSettingsImageClickedHandler(NotificationPopup.mSettingsImageClickedEventData, new EventArgs());
      e.Handled = true;
    }

    private void mPopupConrol_MouseUp(object sender, MouseButtonEventArgs e)
    {
      if (this.mMutePopup.IsOpen)
      {
        this.mMutePopup.IsOpen = false;
      }
      else
      {
        if (this.mClickHandler == null)
        {
          try
          {
            HTTPUtils.SendRequestToClient("markNotificationInDrawer", new Dictionary<string, string>()
            {
              {
                "vmname",
                this.VmName
              },
              {
                "id",
                this.AndroidNotificationId
              }
            }, MultiInstanceStrings.VmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp");
            if (string.Compare(this.AppName, "Successfully copied files:", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(this.AppName, "Cannot copy files:", StringComparison.OrdinalIgnoreCase) == 0)
            {
              NotificationPopup.LaunchExplorer(this.mLblContent.Text);
              return;
            }
            Logger.Info("launching " + this.AppName);
            string packageName = "com.bluestacks.appmart";
            string activityName = "com.bluestacks.appmart.StartTopAppsActivity";
            string fileName = RegistryStrings.InstallDir + "\\HD-RunApp.exe";
            if (!new JsonParser(this.VmName).GetAppInfoFromAppName(this.AppName, out packageName, out string _, out activityName))
            {
              Logger.Error("Failed to launch app: {0}. No info found in json. Starting home app", (object) this.AppName);
              if (!string.IsNullOrEmpty(this.PackageName))
                Process.Start(fileName, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "-p {0} -a {1} -vmname:{2}", (object) this.PackageName, (object) activityName, (object) this.VmName));
            }
            else
            {
              string str = "-json \"" + new JObject()
              {
                {
                  "app_icon_url",
                  (JToken) ""
                },
                {
                  "app_name",
                  (JToken) this.AppName
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
              Process.Start(fileName, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} -vmname {1}", (object) str, (object) this.VmName));
            }
          }
          catch (Exception ex)
          {
            Logger.Error(ex.ToString());
          }
        }
        this.Close();
        e.Handled = true;
      }
    }

    public static void LaunchExplorer(string message)
    {
      try
      {
        string[] strArray1;
        if (message == null)
          strArray1 = (string[]) null;
        else
          strArray1 = message.Split('\n');
        string[] strArray2 = strArray1;
        string fullName = Directory.GetParent(strArray2[0]).FullName;
        string fileName = "explorer.exe";
        string arguments;
        if (strArray2.Length == 1)
          arguments = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/Select, {0}", (object) strArray2[0]);
        else
          arguments = fullName;
        Process.Start(fileName, arguments);
      }
      catch (Exception ex)
      {
        Logger.Error(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Error Occured, Err : {0}", (object) ex.ToString()));
      }
    }

    private void Lbl1Hour_MouseUp(object sender, MouseButtonEventArgs e)
    {
      NotificationManager.Instance.UpdateMuteState(MuteState.MutedFor1Hour, this.mTitle, this.VmName);
      Stats.SendCommonClientStatsAsync("notification_mode", "app_notifications_snoozed", this.VmName, this.PackageName, "Muted_" + (sender as TextBlock).Text, "desktop_notification", "");
      this.Close();
      e.Handled = true;
    }

    private void Lbl1Day_MouseUp(object sender, MouseButtonEventArgs e)
    {
      NotificationManager.Instance.UpdateMuteState(MuteState.MutedFor1Day, this.mTitle, this.VmName);
      Stats.SendCommonClientStatsAsync("notification_mode", "app_notifications_snoozed", this.VmName, this.PackageName, "Muted_" + (sender as TextBlock).Text, "desktop_notification", "");
      this.Close();
      e.Handled = true;
    }

    private void Lbl1Week_MouseUp(object sender, MouseButtonEventArgs e)
    {
      NotificationManager.Instance.UpdateMuteState(MuteState.MutedFor1Week, this.mTitle, this.VmName);
      Stats.SendCommonClientStatsAsync("notification_mode", "app_notifications_snoozed", this.VmName, this.PackageName, "Muted_" + (sender as TextBlock).Text, "desktop_notification", "");
      this.Close();
      e.Handled = true;
    }

    private void LblForever_MouseUp(object sender, MouseButtonEventArgs e)
    {
      NotificationManager.Instance.UpdateMuteState(MuteState.MutedForever, this.mTitle, this.VmName);
      if (NotificationManager.Instance.DictNotificationItems.ContainsKey(this.mTitle))
        NotificationManager.Instance.DictNotificationItems[this.mTitle].ShowDesktopNotifications = false;
      NotificationManager.Instance.UpdateNotificationsSettings();
      Stats.SendCommonClientStatsAsync("notification_mode", "app_notifications_snoozed", this.VmName, this.PackageName, "Muted_" + (sender as TextBlock).Text, "desktop_notification", "");
      this.Close();
      e.Handled = true;
    }

    private void Grid_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
    {
      ((System.Windows.Controls.Panel) sender).Background = (Brush) new SolidColorBrush((Color) ColorConverter.ConvertFromString("#262c4b"));
    }

    private void Grid_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
    {
      ((System.Windows.Controls.Panel) sender).Background = (Brush) new SolidColorBrush((Color) ColorConverter.ConvertFromString("#34375C"));
    }

    private void mButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
    }

    ~NotificationPopup()
    {
      this.Dispose(false);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (this.disposedValue)
        return;
      int num = disposing ? 1 : 0;
      this.mTimer?.Dispose();
      this.disposedValue = true;
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    private void mPopup_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      this.Height = (sender as Grid).ActualHeight;
      this.mPopup.Height = (sender as Grid).ActualHeight;
    }

    private void MutePopup_Closed(object sender, EventArgs e)
    {
      this.mPopup.MouseLeave += new System.Windows.Input.MouseEventHandler(this.PopupConrol_MouseLeave);
    }

    private void PopupConrol_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
    {
      this.Close();
      this.mPopup.MouseLeave -= new System.Windows.Input.MouseEventHandler(this.PopupConrol_MouseLeave);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      System.Windows.Application.LoadComponent((object) this, new Uri("/HD-Common;component/notificationpopup.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    internal Delegate _CreateDelegate(System.Type delegateType, string handler)
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
          this.mPopupConrol = (NotificationPopup) target;
          this.mPopupConrol.LayoutUpdated += new EventHandler(this.mPopupConrol_LayoutUpdated);
          this.mPopupConrol.MouseUp += new MouseButtonEventHandler(this.mPopupConrol_MouseUp);
          break;
        case 2:
          this.mPopup = (Popup) target;
          break;
        case 3:
          ((FrameworkElement) target).SizeChanged += new SizeChangedEventHandler(this.mPopup_SizeChanged);
          break;
        case 4:
          this.mImageOuterBorder = (Border) target;
          break;
        case 5:
          this.RestoreBtnInnerBorder = (Border) target;
          break;
        case 6:
          this.mRestoreBtnGrid = (StackPanel) target;
          break;
        case 7:
          this.mImage = (Image) target;
          break;
        case 8:
          this.mLblHeader = (TextBlock) target;
          break;
        case 9:
          this.mLblContent = (TextBlock) target;
          break;
        case 10:
          this.mButton = (CustomButton) target;
          break;
        case 11:
          this.mImgMute = (CustomPictureBox) target;
          break;
        case 12:
          this.PopUpGrid = (Grid) target;
          break;
        case 13:
          this.mMutePopup = (CustomPopUp) target;
          break;
        case 14:
          this.mOuterGridPopUp = (Grid) target;
          break;
        case 15:
          this.mMaskBorder2 = (Border) target;
          break;
        case 16:
          ((UIElement) target).MouseEnter += new System.Windows.Input.MouseEventHandler(this.Grid_MouseEnter);
          ((UIElement) target).MouseLeave += new System.Windows.Input.MouseEventHandler(this.Grid_MouseLeave);
          break;
        case 17:
          this.mLbl1Hour = (TextBlock) target;
          this.mLbl1Hour.MouseUp += new MouseButtonEventHandler(this.Lbl1Hour_MouseUp);
          break;
        case 18:
          ((UIElement) target).MouseEnter += new System.Windows.Input.MouseEventHandler(this.Grid_MouseEnter);
          ((UIElement) target).MouseLeave += new System.Windows.Input.MouseEventHandler(this.Grid_MouseLeave);
          break;
        case 19:
          this.mLbl1Day = (TextBlock) target;
          this.mLbl1Day.MouseUp += new MouseButtonEventHandler(this.Lbl1Day_MouseUp);
          break;
        case 20:
          ((UIElement) target).MouseEnter += new System.Windows.Input.MouseEventHandler(this.Grid_MouseEnter);
          ((UIElement) target).MouseLeave += new System.Windows.Input.MouseEventHandler(this.Grid_MouseLeave);
          break;
        case 21:
          this.mLbl1Week = (TextBlock) target;
          this.mLbl1Week.MouseUp += new MouseButtonEventHandler(this.Lbl1Week_MouseUp);
          break;
        case 22:
          ((UIElement) target).MouseEnter += new System.Windows.Input.MouseEventHandler(this.Grid_MouseEnter);
          ((UIElement) target).MouseLeave += new System.Windows.Input.MouseEventHandler(this.Grid_MouseLeave);
          break;
        case 23:
          this.mLblForever = (TextBlock) target;
          this.mLblForever.MouseUp += new MouseButtonEventHandler(this.LblForever_MouseUp);
          break;
        case 24:
          this.mImgSettings = (CustomPictureBox) target;
          break;
        case 25:
          this.mImgDismiss = (CustomPictureBox) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
