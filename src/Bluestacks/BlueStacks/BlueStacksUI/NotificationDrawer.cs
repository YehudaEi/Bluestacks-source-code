// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.NotificationDrawer
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
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Shapes;

namespace BlueStacks.BlueStacksUI
{
  public class NotificationDrawer : UserControl, IComponentConnector
  {
    private static Timer snoozeInfoGridTimer = new Timer(2000.0);
    private bool noNotification;
    private MainWindow mMainWindow;
    internal NotificationDrawer mNotificationDrawer;
    internal Grid grdImportantUpdates;
    internal ScrollViewer mImportantNotificationScroll;
    internal Grid grdNormalUpdates;
    internal TextBlock mNotificationText;
    internal CustomPictureBox mSettingsbtn;
    internal Grid mSnoozeInfoGrid;
    internal TextBlock mSnoozeInfoBlock;
    internal ScrollViewer mNotificationScroll;
    internal Grid noNotifControl;
    internal Rectangle mAnimationRect;
    private bool _contentLoaded;

    public NotificationDrawer()
    {
      this.InitializeComponent();
    }

    public MainWindow ParentWindow
    {
      get
      {
        if (this.mMainWindow == null)
          this.mMainWindow = Window.GetWindow((DependencyObject) this) as MainWindow;
        return this.mMainWindow;
      }
    }

    public static Timer SnoozeInfoGridTimer
    {
      get
      {
        return NotificationDrawer.snoozeInfoGridTimer;
      }
      set
      {
        NotificationDrawer.snoozeInfoGridTimer = value;
      }
    }

    public static Timer DrawerAnimationTimer { get; set; } = new Timer(2000.0);

    internal void Populate(
      SerializableDictionary<string, GenericNotificationItem> items)
    {
      List<NotificationDrawerItem> notificationDrawerItemList1 = new List<NotificationDrawerItem>();
      List<NotificationDrawerItem> notificationDrawerItemList2 = new List<NotificationDrawerItem>();
      List<string> stringList1 = new List<string>();
      List<string> stringList2 = new List<string>();
      StackPanel content1 = this.mNotificationScroll.Content as StackPanel;
      StackPanel content2 = this.mImportantNotificationScroll.Content as StackPanel;
      content1.Children.Clear();
      content2.Children.Clear();
      this.mSnoozeInfoGrid.Visibility = Visibility.Collapsed;
      NotificationDrawer.SnoozeInfoGridTimer.Elapsed -= new ElapsedEventHandler(this.mSnoozeInfoGridTimer_Elapsed);
      NotificationDrawer.SnoozeInfoGridTimer.Elapsed += new ElapsedEventHandler(this.mSnoozeInfoGridTimer_Elapsed);
      NotificationDrawer.SnoozeInfoGridTimer.AutoReset = false;
      NotificationDrawer.DrawerAnimationTimer.Elapsed -= new ElapsedEventHandler(this.DrawerAnimationTimer_Elapsed);
      NotificationDrawer.DrawerAnimationTimer.Elapsed += new ElapsedEventHandler(this.DrawerAnimationTimer_Elapsed);
      NotificationDrawer.DrawerAnimationTimer.AutoReset = false;
      foreach (KeyValuePair<string, GenericNotificationItem> keyValuePair in items.Where<KeyValuePair<string, GenericNotificationItem>>((Func<KeyValuePair<string, GenericNotificationItem>, bool>) (_ => !_.Value.IsDeleted)))
        this.AddNotificationItem(keyValuePair.Value);
      this.HideUnhideNoNotification();
      this.UpdateNotificationCount();
    }

    private void DrawerAnimationTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
      this.Dispatcher.Invoke((Delegate) (() =>
      {
        BlueStacksUIBinding.BindColor((DependencyObject) this.ParentWindow.mTopBar.mNotificationCaret, Shape.FillProperty, "ContextMenuItemBackgroundColor");
        BlueStacksUIBinding.BindColor((DependencyObject) this.ParentWindow.mTopBar.mNotificationCaret, Shape.StrokeProperty, "ContextMenuItemBackgroundColor");
        BlueStacksUIBinding.BindColor((DependencyObject) this.ParentWindow.mTopBar.mNotificationCentreDropDownBorder, Control.BorderBrushProperty, "PopupBorderBrush");
        this.ParentWindow.mTopBar.mNotificationDrawerControl.mAnimationRect.Visibility = Visibility.Collapsed;
        this.ParentWindow.mTopBar.mNotificationCentreButton.ImageName = "notification";
        this.ParentWindow.mTopBar.mNotificationCountBadge.Visibility = Visibility.Collapsed;
        if (!this.ParentWindow.IsActive)
          return;
        this.ParentWindow.mTopBar.mNotificationCentrePopup.IsOpen = true;
      }));
    }

    private void mSnoozeInfoGridTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
      this.Dispatcher.Invoke((Delegate) (() => this.mSnoozeInfoGrid.Visibility = Visibility.Collapsed));
    }

    private void HideUnhideNoNotification()
    {
      StackPanel content1 = this.mNotificationScroll.Content as StackPanel;
      StackPanel content2 = this.mImportantNotificationScroll.Content as StackPanel;
      if (!content2.Children.OfType<NotificationDrawerItem>().Any<NotificationDrawerItem>() && !content1.Children.OfType<NotificationDrawerItem>().Any<NotificationDrawerItem>())
      {
        this.grdImportantUpdates.Visibility = Visibility.Collapsed;
        this.grdNormalUpdates.Visibility = Visibility.Visible;
        this.noNotifControl.Visibility = Visibility.Visible;
        this.mNotificationScroll.Visibility = Visibility.Collapsed;
        this.ParentWindow.mTopBar.mNotificationCentreDropDownBorder_LayoutUpdated((object) null, (EventArgs) null);
        this.noNotification = true;
      }
      else
      {
        if (!content2.Children.OfType<NotificationDrawerItem>().Any<NotificationDrawerItem>())
          this.grdImportantUpdates.Visibility = Visibility.Collapsed;
        if (!content1.Children.OfType<NotificationDrawerItem>().Any<NotificationDrawerItem>())
          this.grdNormalUpdates.Visibility = Visibility.Collapsed;
        if (!this.noNotification)
          return;
        this.noNotifControl.Visibility = Visibility.Collapsed;
        this.mNotificationScroll.Visibility = Visibility.Visible;
      }
    }

    private void AddNotificationItem(GenericNotificationItem notifItem)
    {
      try
      {
        NotificationDrawerItem notificationDrawerItem = new NotificationDrawerItem();
        notificationDrawerItem.InitFromGenricNotificationItem(notifItem, this.ParentWindow);
        if (notifItem.Priority == NotificationPriority.Important)
        {
          StackPanel content = this.mImportantNotificationScroll.Content as StackPanel;
          Separator separator = new Separator();
          if (this.FindResource((object) ToolBar.SeparatorStyleKey) is Style resource)
            separator.Style = resource;
          BlueStacksUIBinding.BindColor((DependencyObject) separator, Panel.BackgroundProperty, "HorizontalSeparator");
          separator.Margin = new Thickness(0.0);
          if (content.Children.OfType<NotificationDrawerItem>().Any<NotificationDrawerItem>())
            content.Children.Insert(0, (UIElement) separator);
          content.Children.Insert(0, (UIElement) notificationDrawerItem);
          this.grdImportantUpdates.Visibility = Visibility.Visible;
        }
        else
        {
          StackPanel content = this.mNotificationScroll.Content as StackPanel;
          Separator separator = new Separator();
          if (this.FindResource((object) ToolBar.SeparatorStyleKey) is Style resource)
            separator.Style = resource;
          BlueStacksUIBinding.BindColor((DependencyObject) separator, Panel.BackgroundProperty, "HorizontalSeparator");
          separator.Margin = new Thickness(0.0);
          if (content.Children.OfType<NotificationDrawerItem>().Any<NotificationDrawerItem>())
            content.Children.Insert(0, (UIElement) separator);
          content.Children.Insert(0, (UIElement) notificationDrawerItem);
          this.grdNormalUpdates.Visibility = Visibility.Visible;
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Could not add notificationdraweritem. Id " + notifItem.Id + "Error:" + ex.ToString());
      }
    }

    private void ClearButton_Click(object sender, RoutedEventArgs e)
    {
      this.RemoveAllNotificationItems();
      e.Handled = true;
    }

    public void RemoveAllNotificationItems()
    {
      StackPanel content = this.mNotificationScroll.Content as StackPanel;
      GenericNotificationManager.MarkNotification(content.Children.OfType<NotificationDrawerItem>().Select<NotificationDrawerItem, string>((Func<NotificationDrawerItem, string>) (_ => _.Id)), (System.Action<GenericNotificationItem>) (_ => _.IsDeleted = true));
      content.Children.Clear();
      this.noNotifControl.Visibility = Visibility.Visible;
      this.mNotificationScroll.Visibility = Visibility.Collapsed;
      this.noNotification = true;
    }

    public void RemoveNotificationItem(string id)
    {
      StackPanel content = this.mNotificationScroll.Content as StackPanel;
      foreach (NotificationDrawerItem notificationDrawerItem in content.Children.OfType<NotificationDrawerItem>())
      {
        if (string.Equals(notificationDrawerItem.Id, id, StringComparison.InvariantCultureIgnoreCase))
        {
          GenericNotificationManager.MarkNotification((IEnumerable<string>) new List<string>()
          {
            id
          }, (System.Action<GenericNotificationItem>) (x => x.IsDeleted = true));
          int index = content.Children.IndexOf((UIElement) notificationDrawerItem);
          content.Children.Remove((UIElement) notificationDrawerItem);
          if (content.Children.Count > index)
          {
            content.Children.RemoveAt(index);
            break;
          }
          break;
        }
      }
      if (content.Children.Count != 0)
        return;
      this.noNotifControl.Visibility = Visibility.Visible;
      this.mNotificationScroll.Visibility = Visibility.Collapsed;
      this.noNotification = true;
    }

    public void UpdateNotificationCount()
    {
      SerializableDictionary<string, GenericNotificationItem> notificationItems = GenericNotificationManager.GetNotificationItems((Predicate<GenericNotificationItem>) (x => !x.IsDeleted && !x.IsRead && string.Equals(x.VmName, this.ParentWindow.mVmName, StringComparison.InvariantCulture)));
      if (notificationItems.Count > 0 && !this.ParentWindow.mTopBar.mNotificationCentrePopup.IsOpen)
      {
        Border border1 = new Border();
        border1.VerticalAlignment = VerticalAlignment.Center;
        border1.Height = 14.0;
        border1.MaxWidth = 24.0;
        Border border2 = border1;
        TextBlock textBlock1 = new TextBlock();
        textBlock1.Text = notificationItems.Count.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        textBlock1.FontSize = 10.0;
        textBlock1.MaxWidth = 24.0;
        textBlock1.FontWeight = FontWeights.Bold;
        textBlock1.VerticalAlignment = VerticalAlignment.Center;
        textBlock1.HorizontalAlignment = HorizontalAlignment.Center;
        textBlock1.Padding = new Thickness(3.0, 0.0, 3.0, 1.0);
        TextBlock textBlock2 = textBlock1;
        if (notificationItems.Count > 99)
          textBlock2.Text = "99+";
        BlueStacksUIBinding.BindColor((DependencyObject) textBlock2, Control.ForegroundProperty, "SettingsWindowTitleBarForeGround");
        BlueStacksUIBinding.BindColor((DependencyObject) border2, Control.BackgroundProperty, "XPackPopupColor");
        border2.CornerRadius = new CornerRadius(7.0);
        border2.Child = (UIElement) textBlock2;
        Canvas.SetLeft((UIElement) border2, 20.0);
        Canvas.SetTop((UIElement) border2, 9.0);
        if (this.ParentWindow.mTopBar.mNotificationCountBadge == null)
          return;
        if (GenericNotificationManager.GetNotificationItems((Predicate<GenericNotificationItem>) (x => !x.IsRead && !x.IsDeleted && x.Priority == NotificationPriority.Important)).Count > 0)
          this.ParentWindow.mTopBar.mNotificationCountBadge.Visibility = Visibility.Collapsed;
        else
          this.ParentWindow.mTopBar.mNotificationCountBadge.Visibility = Visibility.Visible;
        this.ParentWindow.mTopBar.mNotificationCountBadge.Children.Clear();
        this.ParentWindow.mTopBar.mNotificationCountBadge.Children.Add((UIElement) border2);
      }
      else
        this.ParentWindow.mTopBar.mNotificationCountBadge.Visibility = Visibility.Collapsed;
    }

    private void mSettingsbtn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      try
      {
        BlueStacks.Common.Stats.SendCommonClientStatsAsync("notification_mode", "bell_settings_clicked", this.ParentWindow.mVmName, "", "", "", "");
        this.ParentWindow.Dispatcher.Invoke((Delegate) (() => MainWindow.OpenSettingsWindow(this.ParentWindow, "STRING_NOTIFICATION")));
      }
      catch (Exception ex)
      {
        Logger.Info("Error in opening settings window" + ex?.ToString());
      }
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/genericnotification/notificationdrawer.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.mNotificationDrawer = (NotificationDrawer) target;
          break;
        case 2:
          this.grdImportantUpdates = (Grid) target;
          break;
        case 3:
          this.mImportantNotificationScroll = (ScrollViewer) target;
          break;
        case 4:
          this.grdNormalUpdates = (Grid) target;
          break;
        case 5:
          this.mNotificationText = (TextBlock) target;
          break;
        case 6:
          ((ButtonBase) target).Click += new RoutedEventHandler(this.ClearButton_Click);
          break;
        case 7:
          this.mSettingsbtn = (CustomPictureBox) target;
          this.mSettingsbtn.MouseLeftButtonUp += new MouseButtonEventHandler(this.mSettingsbtn_MouseLeftButtonUp);
          break;
        case 8:
          this.mSnoozeInfoGrid = (Grid) target;
          break;
        case 9:
          this.mSnoozeInfoBlock = (TextBlock) target;
          break;
        case 10:
          this.mNotificationScroll = (ScrollViewer) target;
          break;
        case 11:
          this.noNotifControl = (Grid) target;
          break;
        case 12:
          this.mAnimationRect = (Rectangle) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
