// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.NotificationWindow
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using Microsoft.Win32;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace BlueStacks.Common
{
  public class NotificationWindow : Window, IComponentConnector
  {
    private Dictionary<string, NotificationPopup> mDictPopups = new Dictionary<string, NotificationPopup>();
    private bool mIsPopupsEnabled = true;
    private const int MAX_ALLOWED_NOTIFICATION = 3;
    private static NotificationWindow mInstance;
    internal StackPanel mStackPanel;
    private bool _contentLoaded;

    [DllImport("user32.dll")]
    public static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);

    public static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
    {
      IntPtr num1 = IntPtr.Zero;
      NotificationWindow.SetLastError(0);
      int lastWin32Error;
      if (IntPtr.Size == 4)
      {
        int num2 = NotificationWindow.IntSetWindowLong(hWnd, nIndex, NotificationWindow.IntPtrToInt32(dwNewLong));
        lastWin32Error = Marshal.GetLastWin32Error();
        num1 = new IntPtr(num2);
      }
      else
      {
        num1 = NotificationWindow.IntSetWindowLongPtr(hWnd, nIndex, dwNewLong);
        lastWin32Error = Marshal.GetLastWin32Error();
      }
      if (num1 == IntPtr.Zero && lastWin32Error != 0)
        throw new Win32Exception(lastWin32Error);
      return num1;
    }

    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", SetLastError = true)]
    private static extern IntPtr IntSetWindowLongPtr(
      IntPtr hWnd,
      int nIndex,
      IntPtr dwNewLong);

    [DllImport("user32.dll", EntryPoint = "SetWindowLong", SetLastError = true)]
    private static extern int IntSetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    private static int IntPtrToInt32(IntPtr intPtr)
    {
      return (int) intPtr.ToInt64();
    }

    [DllImport("kernel32.dll")]
    public static extern void SetLastError(int dwErrorCode);

    private void NotificationWindow_Loaded(object sender, RoutedEventArgs e)
    {
    }

    public Dictionary<string, bool> IsOverrideDesktopNotificationSettingsDict { get; set; } = new Dictionary<string, bool>();

    public Dictionary<string, Dictionary<string, int>> AppNotificationCountDictForEachVM { get; set; } = new Dictionary<string, Dictionary<string, int>>();

    public static NotificationWindow Instance
    {
      get
      {
        if (NotificationWindow.mInstance == null)
          NotificationWindow.Init();
        return NotificationWindow.mInstance;
      }
    }

    public static void Init()
    {
      NotificationWindow.mInstance = new NotificationWindow();
      SystemEvents.DisplaySettingsChanged -= new EventHandler(NotificationWindow.mInstance.HandleDisplaySettingsChanged);
      SystemEvents.DisplaySettingsChanged += new EventHandler(NotificationWindow.mInstance.HandleDisplaySettingsChanged);
    }

    private NotificationWindow()
    {
      this.InitializeComponent();
      this.SetWindowPosition();
    }

    public void HandleDisplaySettingsChanged(object _1, EventArgs _2)
    {
      this.SetWindowPosition();
    }

    public void AddAlert(
      string imagePath,
      string title,
      string displayMsg,
      bool autoClose,
      int duration,
      MouseButtonEventHandler clickHandler,
      bool hideMute,
      string vmName,
      bool isCloudNotification,
      string package = "",
      bool isForceNotification = false,
      string id = "0",
      bool showOnlySettings = false)
    {
      if (!this.mIsPopupsEnabled)
        return;
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        MuteState muteState = NotificationManager.Instance.IsShowNotificationForKey(title, vmName);
        if (((muteState == MuteState.NotMuted ? 1 : (muteState == MuteState.AutoHide ? 1 : 0)) | (isForceNotification ? 1 : 0)) == 0 && !NotificationManager.Instance.IsDesktopNotificationToBeShown(title))
          return;
        if (this.Visibility == Visibility.Collapsed || this.Visibility == Visibility.Hidden)
          this.Show();
        string upper = (string.IsNullOrEmpty(package) ? title : package).ToUpper(CultureInfo.InvariantCulture);
        if (this.mDictPopups.ContainsKey(upper))
          this.RemovePopup(this.mDictPopups[upper]);
        if (this.mDictPopups.Count >= 3)
          this.RemovePopup((NotificationPopup) this.mStackPanel.Children[2]);
        if (!isCloudNotification & isForceNotification)
        {
          autoClose = true;
          duration = 5000;
        }
        NotificationPopup notificationPopup = NotificationPopup.InitPopup(imagePath, title, displayMsg, autoClose, duration, clickHandler, hideMute, vmName, (MouseButtonEventHandler) null, (MouseButtonEventHandler) null, (MouseButtonEventHandler) null, false, string.Empty, id, showOnlySettings, package);
        this.mStackPanel.Children.Insert(0, (UIElement) notificationPopup);
        this.mDictPopups.Add(upper, notificationPopup);
      }));
    }

    public void ForceShowAlert(
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
      bool showOnlySettings = false)
    {
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        NotificationPopup notificationPopup = NotificationPopup.InitPopup(imagePath, title, displayMsg, autoClose, duration, clickHandler, hideMute, vmName, buttonClickHandler, closeButtonHandler, muteButtonHandler, showOnlyMute, buttonText, id, showOnlySettings, "");
        this.mStackPanel.Children.Insert(0, (UIElement) notificationPopup);
        if (!this.mDictPopups.ContainsKey(title.ToUpper(CultureInfo.InvariantCulture)))
          this.mDictPopups.Add(title.ToUpper(CultureInfo.InvariantCulture), notificationPopup);
        this.Topmost = false;
      }));
    }

    internal void RemovePopup(NotificationPopup popup)
    {
      this.mDictPopups.Remove((string.IsNullOrEmpty(popup.PackageName) ? popup.Title : popup.PackageName).ToUpper(CultureInfo.InvariantCulture));
      popup.mPopup.IsOpen = false;
      if (this.mStackPanel.Children.Contains((UIElement) popup))
      {
        popup.StopTimer();
        this.mStackPanel.Children.Remove((UIElement) popup);
      }
      if (this.mStackPanel.Children.Count >= 3)
      {
        (this.mStackPanel.Children[2] as NotificationPopup).StopTimer();
        this.mStackPanel.Children.RemoveAt(2);
      }
      foreach (NotificationPopup child in this.mStackPanel.Children)
      {
        if (string.Equals(child.Title, popup.Title, StringComparison.InvariantCultureIgnoreCase))
        {
          child.StopTimer();
          this.mStackPanel.Children.Remove((UIElement) child);
          break;
        }
      }
      if (this.mDictPopups.Count != 0)
        return;
      this.Hide();
    }

    public void EnablePopups(bool visible)
    {
      if (visible)
      {
        this.mIsPopupsEnabled = true;
      }
      else
      {
        this.mIsPopupsEnabled = false;
        foreach (NotificationPopup popup in this.mDictPopups.Values.ToArray<NotificationPopup>())
          this.RemovePopup(popup);
      }
    }

    private static void GamepadNotificationButtonClick(object sender, RoutedEventArgs e)
    {
      string fileName = WebHelper.GetUrlWithParams(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) WebHelper.GetServerHost(), (object) "help_articles"), (string) null, (string) null, (string) null) + "&article=gamepad_connected_notif_help";
      Logger.Info("Launching browser with URL: {0}", (object) fileName);
      Process.Start(fileName);
      Stats.SendCommonClientStatsAsync("notification_gamepad", "notification_gamepad_click", "Android", "", "", "", "");
    }

    public void ShowSimplePopupForClient(string title, string description)
    {
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        this.ShowInTaskbar = true;
        this.Visibility = Visibility.Visible;
        this.ForceShowAlert(Path.Combine(RegistryManager.Instance.InstallDir, "default_icon.png"), title, description, true, 15000, new MouseButtonEventHandler(NotificationWindow.GamepadNotificationButtonClick), true, "Android", (MouseButtonEventHandler) null, (MouseButtonEventHandler) null, (MouseButtonEventHandler) null, false, "", "0", false);
        Stats.SendCommonClientStatsAsync("notification_gamepad", "notification_gamepad_impression", "Android", "", "", "", "");
      }));
    }

    private void Window_IsVisibleChanged(object _1, DependencyPropertyChangedEventArgs _2)
    {
      this.SetWindowPosition();
    }

    private void SetWindowPosition()
    {
      try
      {
        this.Height = SystemParameters.WorkArea.Height;
        this.Width = SystemParameters.FullPrimaryScreenWidth * 0.2;
        this.Left = SystemParameters.FullPrimaryScreenWidth - this.Width;
        this.Top = 0.0;
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in HandleDisplaySettingsChanged. Exception: " + ex.ToString());
      }
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/HD-Common;component/notificationwindow.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      if (connectionId != 1)
      {
        if (connectionId == 2)
          this.mStackPanel = (StackPanel) target;
        else
          this._contentLoaded = true;
      }
      else
      {
        ((FrameworkElement) target).Loaded += new RoutedEventHandler(this.NotificationWindow_Loaded);
        ((UIElement) target).IsVisibleChanged += new DependencyPropertyChangedEventHandler(this.Window_IsVisibleChanged);
      }
    }

    [System.Flags]
    public enum ExtendedWindowStyles
    {
      WS_EX_TOOLWINDOW = 128, // 0x00000080
    }

    public enum GetWindowLongFields
    {
      GWL_EXSTYLE = -20, // 0xFFFFFFEC
    }
  }
}
