// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.WidgetIcon
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
using System.Windows.Markup;
using System.Windows.Media.Animation;

namespace BlueStacks.BlueStacksUI
{
  public class WidgetIcon : Button, IComponentConnector, IStyleConnector
  {
    public static readonly DependencyProperty MyFooterTextProperty = DependencyProperty.Register(nameof (FooterText), typeof (string), typeof (WidgetIcon));
    private string mBusyImageNamePostFix = "_busy";
    private CustomPictureBox mImage;
    private CustomPictureBox mBusyImage;
    private Storyboard mBusyIconStoryBoard;
    private string mImageName;
    internal WidgetIcon mWidgetIcon;
    private bool _contentLoaded;

    public string ImageName
    {
      get
      {
        return this.mImageName;
      }
      set
      {
        this.mImageName = value;
        if (this.mImage != null)
          this.mImage.ImageName = this.mImageName;
        if (this.mBusyImage == null)
          return;
        this.mBusyImage.ImageName = this.mImageName + this.mBusyImageNamePostFix;
      }
    }

    public string FooterText
    {
      get
      {
        return this.GetValue(WidgetIcon.MyFooterTextProperty) as string;
      }
      set
      {
        this.SetValue(WidgetIcon.MyFooterTextProperty, (object) value);
      }
    }

    public WidgetIcon()
    {
      this.InitializeComponent();
    }

    internal void ShowBusyIcon(Visibility visibility)
    {
      this.mBusyImage.Visibility = visibility;
    }

    private void Image_Initialized(object sender, EventArgs e)
    {
      if (this.mImage == null)
        this.mImage = sender as CustomPictureBox;
      if (string.IsNullOrEmpty(this.mImageName))
        return;
      this.mImage.ImageName = this.mImageName;
    }

    private void BusyImage_Initialized(object sender, EventArgs e)
    {
      if (this.mBusyImage == null)
        this.mBusyImage = sender as CustomPictureBox;
      if (string.IsNullOrEmpty(this.mImageName))
        return;
      this.mBusyImage.ImageName = this.mImageName + this.mBusyImageNamePostFix;
    }

    private void CustomPictureBox_IsVisibleChanged(object _1, DependencyPropertyChangedEventArgs _2)
    {
      if (this.mBusyImage.IsVisible)
      {
        if (this.mBusyIconStoryBoard == null)
        {
          this.mBusyIconStoryBoard = new Storyboard();
          DoubleAnimation doubleAnimation1 = new DoubleAnimation();
          doubleAnimation1.From = new double?(0.0);
          doubleAnimation1.To = new double?(360.0);
          doubleAnimation1.RepeatBehavior = RepeatBehavior.Forever;
          doubleAnimation1.Duration = new Duration(new TimeSpan(0, 0, 1));
          DoubleAnimation doubleAnimation2 = doubleAnimation1;
          Storyboard.SetTarget((DependencyObject) doubleAnimation2, (DependencyObject) this.mBusyImage);
          Storyboard.SetTargetProperty((DependencyObject) doubleAnimation2, new PropertyPath("(UIElement.RenderTransform).(RotateTransform.Angle)", new object[0]));
          this.mBusyIconStoryBoard.Children.Add((Timeline) doubleAnimation2);
        }
        this.mBusyIconStoryBoard.Begin();
      }
      else
        this.mBusyIconStoryBoard.Pause();
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/controls/widgeticon.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      if (connectionId == 1)
        this.mWidgetIcon = (WidgetIcon) target;
      else
        this._contentLoaded = true;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IStyleConnector.Connect(int connectionId, object target)
    {
      if (connectionId != 2)
      {
        if (connectionId != 3)
          return;
        ((FrameworkElement) target).Initialized += new EventHandler(this.BusyImage_Initialized);
        ((UIElement) target).IsVisibleChanged += new DependencyPropertyChangedEventHandler(this.CustomPictureBox_IsVisibleChanged);
      }
      else
        ((FrameworkElement) target).Initialized += new EventHandler(this.Image_Initialized);
    }
  }
}
