// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.CustomMessageWindow
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BlueStacks.Common
{
  public class CustomMessageWindow : CustomWindow, IComponentConnector
  {
    private Dictionary<object, Tuple<ButtonColors, EventHandler, object>> mDictActions = new Dictionary<object, Tuple<ButtonColors, EventHandler, object>>();
    private Predicate<object> mCloseButtonEventHandler;
    private CustomButton mButton1;
    private CustomButton mButton2;
    private object mCloseButtonEventData;
    private double mContentMaxWidth;
    internal Border mMaskBorder;
    internal Grid mParentGrid;
    internal Grid mTitleGrid;
    internal CustomPictureBox mTitleIcon;
    internal TextBlock mTitleText;
    internal CustomPictureBox mCustomMessageBoxMinimizeButton;
    internal CustomPictureBox mCustomMessageBoxCloseButton;
    internal CustomPictureBox mMessageIcon;
    internal Grid mTextBlockGrid;
    internal StackPanel mBodyTextStackPanel;
    internal TextBlock mBodyTextBlockTitle;
    internal TextBlock mAboveBodyWarningTextBlock;
    internal TextBlock mBodyTextBlock;
    internal TextBlock mBodyWarningTextBlock;
    internal TextBlock mUrlTextBlock;
    internal Hyperlink mUrlLink;
    internal CustomCheckbox mCheckBox;
    internal Grid mProgressGrid;
    internal BlueProgressBar mProgressbar;
    internal Grid mProgressUpdatesGrid;
    internal TextBlock mProgressStatus;
    internal Label mProgressPercentage;
    internal StackPanel mStackPanel;
    private bool _contentLoaded;

    public EventHandler MinimizeEventHandler { get; set; }

    public ButtonColors ClickedButton { get; set; } = ButtonColors.Background;

    public double ContentMaxWidth
    {
      get
      {
        return this.mContentMaxWidth;
      }
      set
      {
        this.mContentMaxWidth = value;
        this.mTitleGrid.MaxWidth = value;
        this.mBodyTextStackPanel.MaxWidth = value;
        this.mProgressGrid.MaxWidth = value;
      }
    }

    public bool ProgressBarEnabled
    {
      set
      {
        this.mProgressGrid.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
      }
    }

    public bool ProgressStatusEnabled
    {
      set
      {
        this.mProgressUpdatesGrid.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
      }
    }

    public bool IsWindowMinizable
    {
      set
      {
        if (value)
          this.mCustomMessageBoxMinimizeButton.Visibility = Visibility.Visible;
        else
          this.mCustomMessageBoxMinimizeButton.Visibility = Visibility.Hidden;
      }
    }

    public bool IsWindowClosable
    {
      set
      {
        if (value)
          this.mCustomMessageBoxCloseButton.Visibility = Visibility.Visible;
        else
          this.mCustomMessageBoxCloseButton.Visibility = Visibility.Hidden;
      }
    }

    public bool IsWindowCloseButtonDisabled
    {
      set
      {
        if (!value)
          return;
        this.mCustomMessageBoxCloseButton.ToolTip = (object) null;
        this.mCustomMessageBoxCloseButton.IsDisabled = true;
        this.mCustomMessageBoxCloseButton.PreviewMouseLeftButtonUp -= new MouseButtonEventHandler(this.Close_PreviewMouseLeftButtonUp);
      }
    }

    public string ImageName
    {
      set
      {
        this.mTitleIcon.ImageName = value;
        if (string.IsNullOrEmpty(value))
          return;
        this.mTitleIcon.Visibility = Visibility.Visible;
      }
    }

    public bool IsWithoutButtons
    {
      set
      {
        if (value)
          this.mStackPanel.Visibility = Visibility.Collapsed;
        else
          this.mStackPanel.Visibility = Visibility.Visible;
      }
    }

    public TextBlock TitleTextBlock
    {
      get
      {
        return this.mTitleText;
      }
    }

    public CustomPictureBox MessageIcon
    {
      get
      {
        return this.mMessageIcon;
      }
    }

    public TextBlock BodyTextBlockTitle
    {
      get
      {
        return this.mBodyTextBlockTitle;
      }
    }

    public TextBlock BodyTextBlock
    {
      get
      {
        return this.mBodyTextBlock;
      }
    }

    public TextBlock BodyWarningTextBlock
    {
      get
      {
        return this.mBodyWarningTextBlock;
      }
    }

    public TextBlock AboveBodyWarningTextBlock
    {
      get
      {
        return this.mAboveBodyWarningTextBlock;
      }
    }

    public CustomPictureBox CloseButton
    {
      get
      {
        return this.mCustomMessageBoxCloseButton;
      }
    }

    public TextBlock UrlTextBlock
    {
      get
      {
        return this.mUrlTextBlock;
      }
    }

    public Hyperlink UrlLink
    {
      get
      {
        return this.mUrlLink;
      }
    }

    public CustomCheckbox CheckBox
    {
      get
      {
        return this.mCheckBox;
      }
    }

    public BlueProgressBar CustomProgressBar
    {
      get
      {
        return this.mProgressbar;
      }
    }

    public TextBlock ProgressStatusTextBlock
    {
      get
      {
        return this.mProgressStatus;
      }
    }

    public Label ProgressPercentageTextBlock
    {
      get
      {
        return this.mProgressPercentage;
      }
    }

    public bool IsDraggable { get; set; }

    protected override void OnClosed(EventArgs e)
    {
      base.OnClosed(e);
    }

    public CustomMessageWindow()
    {
      this.InitializeComponent();
      this.Loaded += new RoutedEventHandler(this.CustomMessageWindow_Loaded);
      this.SizeChanged += new SizeChangedEventHandler(this.CustomMessageWindow_SizeChanged);
      this.mStackPanel.Children.Clear();
      this.ContentMaxWidth = 340.0;
    }

    private void CustomMessageWindow_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      if (this.mStackPanel.ActualWidth <= this.ContentMaxWidth)
        return;
      if (this.mButton2 != null)
      {
        this.mStackPanel.Orientation = Orientation.Vertical;
        this.mStackPanel.Height = 90.0;
        this.mButton1.Width = this.ContentMaxWidth;
        this.mButton1.Height = 35.0;
        this.mButton2.Width = this.ContentMaxWidth;
        this.mButton2.Height = 35.0;
        this.mButton2.Margin = new Thickness(0.0, 15.0, 0.0, 0.0);
      }
      else
        this.mButton1.MaxWidth = this.ContentMaxWidth;
    }

    public void CustomMessageWindow_Loaded(object sender, RoutedEventArgs e)
    {
      if (this.Owner == null || InteropWindow.FindMainWindowState((Window) this) == WindowState.Minimized)
        this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
      if (this.mButton2 == null)
        return;
      this.UpdateLayout();
    }

    public void CloseButtonHandle(Predicate<object> handle, object data = null)
    {
      this.mCloseButtonEventHandler = handle;
      this.mCloseButtonEventData = data;
    }

    public void CloseButtonHandle(EventHandler handle, object data = null)
    {
      this.mCloseButtonEventHandler = (Predicate<object>) (o =>
      {
        if (handle != null)
          handle(o, new EventArgs());
        return false;
      });
      this.mCloseButtonEventData = data;
    }

    private void HandleMouseDrag(object sender, MouseButtonEventArgs e)
    {
      if (!this.IsDraggable)
        return;
      if (e.OriginalSource.GetType() == typeof (CustomPictureBox))
        return;
      try
      {
        this.DragMove();
      }
      catch
      {
      }
    }

    public void AddWarning(string title, string imageName = "")
    {
      this.mBodyWarningTextBlock.Text = title;
      if (string.IsNullOrEmpty(imageName))
        return;
      this.mMessageIcon.Visibility = Visibility.Visible;
      this.mMessageIcon.ImageName = imageName;
    }

    public void AddAboveBodyWarning(string title)
    {
      this.mAboveBodyWarningTextBlock.Text = title;
    }

    public void AddButton(
      ButtonColors color,
      string text,
      EventHandler handle,
      string image = null,
      bool ChangeImageAlignment = false,
      object data = null,
      bool isEnabled = true)
    {
      this.AddButtonInUI(new CustomButton(color), color, text, handle, image, ChangeImageAlignment, data, isEnabled);
    }

    public void AddButtonInUI(
      CustomButton button,
      ButtonColors color,
      string text,
      EventHandler handle,
      string image,
      bool ChangeImageAlignment,
      object data,
      bool isEnabled)
    {
      if (button != null)
      {
        if (this.mButton1 == null)
        {
          this.mButton1 = button;
        }
        else
        {
          this.mButton2 = button;
          button.Margin = new Thickness(15.0, 0.0, 0.0, 0.0);
        }
        button.IsEnabled = isEnabled;
        button.Click += new RoutedEventHandler(this.Button_Click);
        button.MinWidth = 100.0;
        button.Visibility = Visibility.Visible;
        BlueStacksUIBinding.Bind((Button) button, text);
        if (image != null)
        {
          button.ImageName = image;
          button.ImageMargin = new Thickness(0.0, 6.0, 5.0, 6.0);
          if (ChangeImageAlignment)
          {
            button.ImageOrder = ButtonImageOrder.AfterText;
            button.ImageMargin = new Thickness(5.0, 6.0, 0.0, 6.0);
          }
        }
      }
      this.mStackPanel.Children.Add((UIElement) button);
      this.mDictActions.Add((object) button, new Tuple<ButtonColors, EventHandler, object>(color, handle, data));
    }

    public void AddHyperLinkInUI(string text, Uri navigateUri, RequestNavigateEventHandler handle)
    {
      Hyperlink hyperlink = new Hyperlink((Inline) new Run(text))
      {
        NavigateUri = navigateUri
      };
      hyperlink.RequestNavigate += new RequestNavigateEventHandler(handle.Invoke);
      hyperlink.Foreground = (Brush) new SolidColorBrush((Color) ColorConverter.ConvertFromString("#047CD2"));
      this.mUrlTextBlock.Inlines.Clear();
      this.mUrlTextBlock.Inlines.Add((Inline) hyperlink);
      this.mUrlTextBlock.Visibility = Visibility.Visible;
    }

    public void Button_Click(object sender, RoutedEventArgs e)
    {
      this.ClickedButton = this.mDictActions[sender].Item1;
      if (this.mDictActions[sender].Item2 != null)
        this.mDictActions[sender].Item2(this.mDictActions[sender].Item3, new EventArgs());
      this.CloseWindow();
    }

    private void Close_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      if (this.mCloseButtonEventHandler != null && this.mCloseButtonEventHandler(this.mCloseButtonEventData))
        return;
      this.CloseWindow();
    }

    public void CloseWindow()
    {
      this.Close();
    }

    private void Minimize_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      EventHandler minimizeEventHandler = this.MinimizeEventHandler;
      if (minimizeEventHandler != null)
        minimizeEventHandler((object) this, (EventArgs) null);
      this.WindowState = WindowState.Minimized;
    }

    public void AddBulletInBody(string text)
    {
      Ellipse ellipse1 = new Ellipse();
      ellipse1.Width = 9.0;
      ellipse1.Height = 9.0;
      ellipse1.VerticalAlignment = VerticalAlignment.Center;
      Ellipse ellipse2 = ellipse1;
      BlueStacksUIBinding.BindColor((DependencyObject) ellipse2, Shape.FillProperty, "ContextMenuItemForegroundDimColor");
      TextBlock textBlock1 = new TextBlock();
      textBlock1.FontSize = 18.0;
      textBlock1.MaxWidth = 300.0;
      textBlock1.FontWeight = FontWeights.Regular;
      TextBlock textBlock2 = textBlock1;
      BlueStacksUIBinding.BindColor((DependencyObject) textBlock2, Control.ForegroundProperty, "ContextMenuItemForegroundDimColor");
      textBlock2.TextWrapping = TextWrapping.Wrap;
      textBlock2.Text = text;
      textBlock2.HorizontalAlignment = HorizontalAlignment.Left;
      textBlock2.VerticalAlignment = VerticalAlignment.Center;
      textBlock2.Margin = new Thickness(0.0, 0.0, 0.0, 10.0);
      BulletDecorator bulletDecorator = new BulletDecorator();
      bulletDecorator.Bullet = (UIElement) ellipse2;
      bulletDecorator.Child = (UIElement) textBlock2;
      this.mBodyTextStackPanel.Children.Add((UIElement) bulletDecorator);
    }

    private void mMessageIcon_IsVisibleChanged(object _1, DependencyPropertyChangedEventArgs _2)
    {
      if (this.mMessageIcon.Visibility == Visibility.Visible)
        this.mTitleGrid.MaxWidth = this.ContentMaxWidth + 85.0;
      else
        this.mTitleGrid.MaxWidth = this.ContentMaxWidth;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/HD-Common;component/uielements/custommessagewindow.xaml", UriKind.Relative));
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
          this.mMaskBorder = (Border) target;
          break;
        case 2:
          this.mParentGrid = (Grid) target;
          this.mParentGrid.MouseDown += new MouseButtonEventHandler(this.HandleMouseDrag);
          break;
        case 3:
          this.mTitleGrid = (Grid) target;
          break;
        case 4:
          this.mTitleIcon = (CustomPictureBox) target;
          break;
        case 5:
          this.mTitleText = (TextBlock) target;
          break;
        case 6:
          this.mCustomMessageBoxMinimizeButton = (CustomPictureBox) target;
          break;
        case 7:
          this.mCustomMessageBoxCloseButton = (CustomPictureBox) target;
          break;
        case 8:
          this.mMessageIcon = (CustomPictureBox) target;
          break;
        case 9:
          this.mTextBlockGrid = (Grid) target;
          break;
        case 10:
          this.mBodyTextStackPanel = (StackPanel) target;
          break;
        case 11:
          this.mBodyTextBlockTitle = (TextBlock) target;
          break;
        case 12:
          this.mAboveBodyWarningTextBlock = (TextBlock) target;
          break;
        case 13:
          this.mBodyTextBlock = (TextBlock) target;
          break;
        case 14:
          this.mBodyWarningTextBlock = (TextBlock) target;
          break;
        case 15:
          this.mUrlTextBlock = (TextBlock) target;
          break;
        case 16:
          this.mUrlLink = (Hyperlink) target;
          break;
        case 17:
          this.mCheckBox = (CustomCheckbox) target;
          break;
        case 18:
          this.mProgressGrid = (Grid) target;
          break;
        case 19:
          this.mProgressbar = (BlueProgressBar) target;
          break;
        case 20:
          this.mProgressUpdatesGrid = (Grid) target;
          break;
        case 21:
          this.mProgressStatus = (TextBlock) target;
          break;
        case 22:
          this.mProgressPercentage = (Label) target;
          break;
        case 23:
          this.mStackPanel = (StackPanel) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
