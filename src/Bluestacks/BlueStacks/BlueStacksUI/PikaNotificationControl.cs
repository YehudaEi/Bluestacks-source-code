// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.PikaNotificationControl
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using Newtonsoft.Json;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BlueStacks.BlueStacksUI
{
  public class PikaNotificationControl : UserControl, IComponentConnector
  {
    internal MainWindow ParentWindow;
    private GenericNotificationItem mNotificationItem;
    internal Grid mNotificationGrid;
    internal System.Windows.Shapes.Path ribbonBack;
    internal System.Windows.Shapes.Path ribbonStroke;
    internal StackPanel backgroundPanel;
    internal CustomPictureBox pikaGif;
    internal TextBlock titleText;
    internal TextBlock messageText;
    internal Border notificationBorder;
    internal CustomPictureBox mCloseBtn;
    private bool _contentLoaded;

    public PikaNotificationControl()
    {
      this.InitializeComponent();
    }

    private void pikanotificationcontrol_MouseUp(object sender, MouseButtonEventArgs e)
    {
      if (this.ParentWindow == null || !this.ParentWindow.mGuestBootCompleted)
        return;
      this.ParentWindow.Utils.HandleGenericActionFromDictionary((Dictionary<string, string>) this.mNotificationItem.ExtraPayload, "notification_ribbon", this.mNotificationItem.NotificationMenuImageName);
      ClientStats.SendMiscellaneousStatsAsync("RibbonClicked", RegistryManager.Instance.UserGuid, RegistryManager.Instance.ClientVersion, this.mNotificationItem.Id, this.mNotificationItem.Title, JsonConvert.SerializeObject((object) this.mNotificationItem.ExtraPayload), (string) null, (string) null, (string) null, "Android");
      GenericNotificationManager.MarkNotification((IEnumerable<string>) new List<string>()
      {
        this.mNotificationItem.Id
      }, (System.Action<GenericNotificationItem>) (x => x.IsRead = true));
      IEnumerable<NotificationDrawerItem> source = (this.ParentWindow.mTopBar.mNotificationDrawerControl.mNotificationScroll.Content as StackPanel).Children.OfType<NotificationDrawerItem>().Where<NotificationDrawerItem>((Func<NotificationDrawerItem, bool>) (_ => _.Id == this.mNotificationItem.Id));
      if (source.Any<NotificationDrawerItem>())
        source.First<NotificationDrawerItem>().ChangeToReadBackground();
      this.ParentWindow.mTopBar.RefreshNotificationCentreButton();
      this.CloseClicked(sender, (EventArgs) e);
    }

    private void ApplyHoverColors(bool hover)
    {
      if (hover)
      {
        if (string.IsNullOrEmpty(this.mNotificationItem.NotificationDesignItem.HoverBorderColor))
          this.mNotificationItem.NotificationDesignItem.HoverBorderColor = this.mNotificationItem.NotificationDesignItem.BorderColor;
        this.notificationBorder.BorderBrush = (Brush) new SolidColorBrush((Color) ColorConverter.ConvertFromString(this.mNotificationItem.NotificationDesignItem.HoverBorderColor));
        if (string.IsNullOrEmpty(this.mNotificationItem.NotificationDesignItem.HoverRibboncolor))
          this.mNotificationItem.NotificationDesignItem.HoverRibboncolor = this.mNotificationItem.NotificationDesignItem.Ribboncolor;
        this.ribbonStroke.Stroke = (Brush) new SolidColorBrush((Color) ColorConverter.ConvertFromString(this.mNotificationItem.NotificationDesignItem.HoverBorderColor));
        this.ribbonBack.Fill = (Brush) new SolidColorBrush((Color) ColorConverter.ConvertFromString(this.mNotificationItem.NotificationDesignItem.HoverRibboncolor));
        if (this.mNotificationItem.NotificationDesignItem.HoverBackGroundGradient.Count == 0)
          this.mNotificationItem.NotificationDesignItem.HoverBackGroundGradient.ClearAddRange<SerializableKeyValuePair<string, double>>(this.mNotificationItem.NotificationDesignItem.BackgroundGradient);
        this.backgroundPanel.Background = (Brush) new LinearGradientBrush(new GradientStopCollection(this.mNotificationItem.NotificationDesignItem.HoverBackGroundGradient.Select<SerializableKeyValuePair<string, double>, GradientStop>((Func<SerializableKeyValuePair<string, double>, GradientStop>) (_ => new GradientStop((Color) ColorConverter.ConvertFromString(_.Key), _.Value)))));
      }
      else
      {
        this.notificationBorder.BorderBrush = (Brush) new SolidColorBrush((Color) ColorConverter.ConvertFromString(this.mNotificationItem.NotificationDesignItem.BorderColor));
        this.ribbonStroke.Stroke = (Brush) new SolidColorBrush((Color) ColorConverter.ConvertFromString(this.mNotificationItem.NotificationDesignItem.BorderColor));
        this.ribbonBack.Fill = (Brush) new SolidColorBrush((Color) ColorConverter.ConvertFromString(this.mNotificationItem.NotificationDesignItem.Ribboncolor));
        this.backgroundPanel.Background = (Brush) new LinearGradientBrush(new GradientStopCollection(this.mNotificationItem.NotificationDesignItem.BackgroundGradient.Select<SerializableKeyValuePair<string, double>, GradientStop>((Func<SerializableKeyValuePair<string, double>, GradientStop>) (_ => new GradientStop((Color) ColorConverter.ConvertFromString(_.Key), _.Value)))));
      }
    }

    internal void Init(GenericNotificationItem notifItem)
    {
      this.mNotificationItem = notifItem;
      this.titleText.Text = notifItem.Title;
      this.titleText.Foreground = (Brush) new SolidColorBrush((Color) ColorConverter.ConvertFromString(notifItem.NotificationDesignItem.TitleForeGroundColor));
      this.messageText.Text = notifItem.Message;
      this.messageText.Foreground = (Brush) new SolidColorBrush((Color) ColorConverter.ConvertFromString(notifItem.NotificationDesignItem.MessageForeGroundColor));
      this.notificationBorder.BorderBrush = (Brush) new SolidColorBrush((Color) ColorConverter.ConvertFromString(notifItem.NotificationDesignItem.BorderColor));
      this.ribbonStroke.Stroke = (Brush) new SolidColorBrush((Color) ColorConverter.ConvertFromString(notifItem.NotificationDesignItem.BorderColor));
      if (notifItem.NotificationDesignItem.BackgroundGradient.Count == 0)
      {
        notifItem.NotificationDesignItem.BackgroundGradient.Add(new SerializableKeyValuePair<string, double>("#FFF350", 0.0));
        notifItem.NotificationDesignItem.BackgroundGradient.Add(new SerializableKeyValuePair<string, double>("#FFF8AF", 0.3));
        notifItem.NotificationDesignItem.BackgroundGradient.Add(new SerializableKeyValuePair<string, double>("#FFE940", 0.6));
        notifItem.NotificationDesignItem.BackgroundGradient.Add(new SerializableKeyValuePair<string, double>("#FCE74E", 0.8));
        notifItem.NotificationDesignItem.BackgroundGradient.Add(new SerializableKeyValuePair<string, double>("#FDF09C", 0.9));
        notifItem.NotificationDesignItem.BackgroundGradient.Add(new SerializableKeyValuePair<string, double>("#FFE227", 1.0));
      }
      this.backgroundPanel.Background = (Brush) new LinearGradientBrush(new GradientStopCollection(notifItem.NotificationDesignItem.BackgroundGradient.Select<SerializableKeyValuePair<string, double>, GradientStop>((Func<SerializableKeyValuePair<string, double>, GradientStop>) (_ => new GradientStop((Color) ColorConverter.ConvertFromString(_.Key), _.Value)))));
      if (string.IsNullOrEmpty(notifItem.NotificationDesignItem.Ribboncolor))
        this.ribbonBack.Fill = (Brush) new SolidColorBrush((Color) ColorConverter.ConvertFromString("#FFF350"));
      else
        this.ribbonBack.Fill = (Brush) new SolidColorBrush((Color) ColorConverter.ConvertFromString(notifItem.NotificationDesignItem.Ribboncolor));
      if (string.IsNullOrEmpty(notifItem.NotificationDesignItem.LeftGifName))
      {
        this.pikaGif.Visibility = Visibility.Collapsed;
      }
      else
      {
        this.pikaGif.Visibility = Visibility.Visible;
        this.pikaGif.ImageName = notifItem.NotificationDesignItem.LeftGifName;
      }
      Canvas.SetLeft((UIElement) this, 0.0);
    }

    private void PikaNotificationControl_MouseLeave(object sender, MouseEventArgs e)
    {
      this.mCloseBtn.Visibility = Visibility.Hidden;
      this.ApplyHoverColors(false);
    }

    private void PikaNotificationControl_MouseEnter(object sender, MouseEventArgs e)
    {
      this.mCloseBtn.Visibility = Visibility.Visible;
      this.ApplyHoverColors(true);
    }

    public event EventHandler CloseClicked;

    private void CloseBtn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      Logger.Info("Pika notification close button clicked");
      this.CloseClicked(sender, (EventArgs) e);
      e.Handled = true;
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
      try
      {
        if (File.Exists(System.IO.Path.Combine(RegistryStrings.PromotionDirectory, this.pikaGif.ImageName)))
        {
          ImageBehavior.SetAnimatedSource((Image) this.pikaGif, (ImageSource) new BitmapImage(new Uri(System.IO.Path.Combine(RegistryStrings.PromotionDirectory, this.pikaGif.ImageName))));
        }
        else
        {
          if (!File.Exists(System.IO.Path.Combine(CustomPictureBox.AssetsDir, this.pikaGif.ImageName)))
            return;
          ImageBehavior.SetAnimatedSource((Image) this.pikaGif, (ImageSource) new BitmapImage(new Uri(System.IO.Path.Combine(CustomPictureBox.AssetsDir, this.pikaGif.ImageName))));
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Exception while loading pika notification. " + ex.ToString());
      }
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/pikanotificationcontrol.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          ((UIElement) target).AddHandler(Mouse.MouseUpEvent, (Delegate) new MouseButtonEventHandler(this.pikanotificationcontrol_MouseUp));
          ((UIElement) target).MouseEnter += new MouseEventHandler(this.PikaNotificationControl_MouseEnter);
          ((UIElement) target).MouseLeave += new MouseEventHandler(this.PikaNotificationControl_MouseLeave);
          ((FrameworkElement) target).Loaded += new RoutedEventHandler(this.UserControl_Loaded);
          break;
        case 2:
          this.mNotificationGrid = (Grid) target;
          break;
        case 3:
          this.ribbonBack = (System.Windows.Shapes.Path) target;
          break;
        case 4:
          this.ribbonStroke = (System.Windows.Shapes.Path) target;
          break;
        case 5:
          this.backgroundPanel = (StackPanel) target;
          break;
        case 6:
          this.pikaGif = (CustomPictureBox) target;
          break;
        case 7:
          this.titleText = (TextBlock) target;
          break;
        case 8:
          this.messageText = (TextBlock) target;
          break;
        case 9:
          this.notificationBorder = (Border) target;
          break;
        case 10:
          this.mCloseBtn = (CustomPictureBox) target;
          this.mCloseBtn.MouseLeftButtonUp += new MouseButtonEventHandler(this.CloseBtn_MouseLeftButtonUp);
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
