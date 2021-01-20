// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.HomeAppTabButton
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace BlueStacks.BlueStacksUI
{
  public class HomeAppTabButton : Button, IComponentConnector
  {
    private static bool mIsSwipeDirectonLeft = true;
    private int animationTime = 150;
    private TranslateTransform mTranslateTransform = new TranslateTransform();
    private MainWindow mMainWindow;
    private BrowserControl mAssociatedUserControl;
    private bool mIsSelected;
    internal Grid tabGrid;
    internal CustomPictureBox ImageBox;
    internal TextBlock mTabHeader;
    internal Border mAppTabNotificationCountBorder;
    internal TextBlock mAppTabNotificationCount;
    internal Grid mBottomGrid;
    internal Grid mGridHighlighterBox;
    private bool _contentLoaded;

    public string Key { get; set; } = string.Empty;

    public MainWindow ParentWindow
    {
      get
      {
        if (this.mMainWindow == null)
          this.mMainWindow = Window.GetWindow((DependencyObject) this) as MainWindow;
        return this.mMainWindow;
      }
    }

    public string Text
    {
      get
      {
        return this.mTabHeader.Text;
      }
      set
      {
        this.Key = value + "-Normal";
        this.ImageBox.ImageName = this.Key;
        BlueStacksUIBinding.Bind(this.mTabHeader, value, "");
      }
    }

    public int Column { get; internal set; }

    public BrowserControl AssociatedUserControl
    {
      get
      {
        return this.mAssociatedUserControl;
      }
      set
      {
        this.mAssociatedUserControl = value;
        if (this.mAssociatedUserControl == null)
          return;
        this.mAssociatedUserControl.RenderTransform = (Transform) this.mTranslateTransform;
      }
    }

    private void AssociatedGrid_LayoutUpdated(object sender, EventArgs e)
    {
      try
      {
        if (this.IsSelected || Math.Abs(Math.Abs(this.mTranslateTransform.X) - this.mAssociatedUserControl.ActualWidth) > 10.0)
          return;
        this.mAssociatedUserControl.Visibility = Visibility.Hidden;
        this.mAssociatedUserControl.LayoutUpdated -= new EventHandler(this.AssociatedGrid_LayoutUpdated);
      }
      catch (Exception ex)
      {
        Logger.Info("Error updating " + ex.ToString());
      }
    }

    public bool IsSelected
    {
      get
      {
        return this.mIsSelected;
      }
      set
      {
        if (this.ParentWindow.StaticComponents.mSelectedHomeAppTabButton == this && value)
          return;
        this.mIsSelected = value;
        if (this.ParentWindow.StaticComponents.mSelectedHomeAppTabButton != null)
        {
          HomeAppTabButton homeAppTabButton = this.ParentWindow.StaticComponents.mSelectedHomeAppTabButton;
          HomeAppTabButton.mIsSwipeDirectonLeft = this.Column > homeAppTabButton.Column;
          this.ParentWindow.StaticComponents.mSelectedHomeAppTabButton = (HomeAppTabButton) null;
          if (this.mAssociatedUserControl == homeAppTabButton.mAssociatedUserControl)
          {
            homeAppTabButton.IsAnimationIgnored = true;
            this.IsAnimationIgnored = true;
          }
          homeAppTabButton.IsSelected = false;
        }
        if (this.mIsSelected)
        {
          BlueStacksUIBinding.BindColor((DependencyObject) this.mTabHeader, TextBlock.ForegroundProperty, "SelectedHomeAppTabForegroundColor");
          BlueStacksUIBinding.BindColor((DependencyObject) this.mBottomGrid, Panel.BackgroundProperty, "HomeAppTabBackgroundColor");
          this.ParentWindow.StaticComponents.mSelectedHomeAppTabButton = this;
          this.ParentWindow.Utils.ResetPendingUIOperations();
          this.mGridHighlighterBox.Visibility = Visibility.Visible;
          this.AnimateAssociatedGrid(true);
          this.ImageBox.ImageName = this.Key.Replace("Normal", "Active");
          if (this.mAssociatedUserControl.CefBrowser == null)
            return;
          this.ParentWindow.mWelcomeTab.mHomeAppManager.SetSearchTextBoxFocus(this.animationTime + 100);
          MiscUtils.SetFocusAsync((UIElement) this.mAssociatedUserControl.CefBrowser, 0);
        }
        else
        {
          BlueStacksUIBinding.BindColor((DependencyObject) this.mTabHeader, TextBlock.ForegroundProperty, "HomeAppTabForegroundColor");
          this.mGridHighlighterBox.Visibility = Visibility.Hidden;
          this.AnimateAssociatedGrid(false);
          this.ImageBox.ImageName = this.Key.Replace("Active", "Normal");
        }
      }
    }

    public bool IsAnimationIgnored { get; set; }

    private void AnimateAssociatedGrid(bool show)
    {
      if (this.IsAnimationIgnored)
      {
        this.IsAnimationIgnored = false;
      }
      else
      {
        this.mAssociatedUserControl.LayoutUpdated += new EventHandler(this.AssociatedGrid_LayoutUpdated);
        DoubleAnimation doubleAnimation;
        if (show)
        {
          this.mAssociatedUserControl.Visibility = Visibility.Visible;
          doubleAnimation = !HomeAppTabButton.mIsSwipeDirectonLeft ? new DoubleAnimation(-1.0 * this.mAssociatedUserControl.ActualWidth, 0.0, (Duration) TimeSpan.FromMilliseconds((double) this.animationTime)) : new DoubleAnimation(this.mAssociatedUserControl.ActualWidth, 0.0, (Duration) TimeSpan.FromMilliseconds((double) this.animationTime));
        }
        else
          doubleAnimation = !HomeAppTabButton.mIsSwipeDirectonLeft ? new DoubleAnimation(0.0, this.mAssociatedUserControl.ActualWidth, (Duration) TimeSpan.FromMilliseconds((double) this.animationTime)) : new DoubleAnimation(0.0, -1.0 * this.mAssociatedUserControl.ActualWidth, (Duration) TimeSpan.FromMilliseconds((double) this.animationTime));
        this.mTranslateTransform.BeginAnimation(TranslateTransform.XProperty, (AnimationTimeline) doubleAnimation);
      }
    }

    public HomeAppTabButton()
    {
      this.InitializeComponent();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      this.IsSelected = true;
    }

    private void Button_MouseEnter(object sender, MouseEventArgs e)
    {
      if (this.IsSelected)
        return;
      BlueStacksUIBinding.BindColor((DependencyObject) this.mBottomGrid, Panel.BackgroundProperty, "HomeAppTabBackgroundHoverColor");
      BlueStacksUIBinding.BindColor((DependencyObject) this.mTabHeader, TextBlock.ForegroundProperty, "HomeAppTabForegroundHoverColor");
    }

    private void Button_MouseLeave(object sender, MouseEventArgs e)
    {
      if (this.IsSelected)
        return;
      BlueStacksUIBinding.BindColor((DependencyObject) this.mBottomGrid, Panel.BackgroundProperty, "HomeAppTabBackgroundColor");
      BlueStacksUIBinding.BindColor((DependencyObject) this.mTabHeader, TextBlock.ForegroundProperty, "HomeAppTabForegroundColor");
    }

    private void Button_Loaded(object sender, RoutedEventArgs e)
    {
      this.SetMaxWidth(0.0);
    }

    internal void SetMaxWidth(double extraWidth = 0.0)
    {
      double num = 0.0 + this.mAppTabNotificationCountBorder.ActualWidth + new FormattedText(this.mTabHeader.Text, Thread.CurrentThread.CurrentCulture, this.mTabHeader.FlowDirection, new Typeface(this.mTabHeader.FontFamily, this.mTabHeader.FontStyle, this.mTabHeader.FontWeight, this.mTabHeader.FontStretch), this.mTabHeader.FontSize, this.mTabHeader.Foreground).WidthIncludingTrailingWhitespace + this.tabGrid.ActualHeight + 30.0;
      GridLength width;
      if (extraWidth == double.MaxValue)
      {
        width = this.tabGrid.ColumnDefinitions[0].Width;
        if (width.IsStar)
          num += 50.0;
      }
      else
        num += extraWidth;
      int column = Grid.GetColumn((UIElement) this.mTabHeader);
      if (extraWidth <= 0.0 || extraWidth >= double.MaxValue)
      {
        width = this.tabGrid.ColumnDefinitions[0].Width;
        if (!width.IsStar || extraWidth != double.MaxValue)
        {
          this.tabGrid.ColumnDefinitions[column].Width = new GridLength(1.0, GridUnitType.Star);
          this.tabGrid.ColumnDefinitions[0].Width = new GridLength(0.0, GridUnitType.Pixel);
          this.tabGrid.ColumnDefinitions[5].Width = new GridLength(0.0, GridUnitType.Pixel);
          goto label_8;
        }
      }
      this.tabGrid.ColumnDefinitions[column].Width = new GridLength(1.0, GridUnitType.Auto);
      this.tabGrid.ColumnDefinitions[0].Width = new GridLength(1.0, GridUnitType.Star);
      this.tabGrid.ColumnDefinitions[5].Width = new GridLength(1.0, GridUnitType.Star);
label_8:
      ((Grid) this.Parent).ColumnDefinitions[Grid.GetColumn((UIElement) this)].MaxWidth = num;
    }

    private void mAppTabNotificationCountBorder_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      this.SetMaxWidth(0.0);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Bluestacks;component/homeapptabbutton.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          ((ButtonBase) target).Click += new RoutedEventHandler(this.Button_Click);
          ((UIElement) target).MouseEnter += new MouseEventHandler(this.Button_MouseEnter);
          ((UIElement) target).MouseLeave += new MouseEventHandler(this.Button_MouseLeave);
          ((FrameworkElement) target).Loaded += new RoutedEventHandler(this.Button_Loaded);
          break;
        case 2:
          this.tabGrid = (Grid) target;
          break;
        case 3:
          this.ImageBox = (CustomPictureBox) target;
          break;
        case 4:
          this.mTabHeader = (TextBlock) target;
          break;
        case 5:
          this.mAppTabNotificationCountBorder = (Border) target;
          this.mAppTabNotificationCountBorder.SizeChanged += new SizeChangedEventHandler(this.mAppTabNotificationCountBorder_SizeChanged);
          break;
        case 6:
          this.mAppTabNotificationCount = (TextBlock) target;
          break;
        case 7:
          this.mBottomGrid = (Grid) target;
          break;
        case 8:
          this.mGridHighlighterBox = (Grid) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
