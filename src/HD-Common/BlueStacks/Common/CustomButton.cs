// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.CustomButton
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace BlueStacks.Common
{
  public class CustomButton : Button, IComponentConnector, IStyleConnector
  {
    public static readonly DependencyProperty ButtonColorProperty = DependencyProperty.Register(nameof (ButtonColor), typeof (ButtonColors), typeof (CustomButton), new PropertyMetadata((object) ButtonColors.Blue));
    public static readonly DependencyProperty IsMouseDownProperty = DependencyProperty.Register(nameof (IsMouseDown), typeof (bool), typeof (CustomButton), new PropertyMetadata((object) false));
    public static readonly DependencyProperty ImageOrderProperty = DependencyProperty.Register(nameof (ImageOrder), typeof (ButtonImageOrder), typeof (CustomButton), new PropertyMetadata((object) ButtonImageOrder.BeforeText));
    public static readonly DependencyProperty ImageNameProperty = DependencyProperty.Register(nameof (ImageName), typeof (string), typeof (CustomButton), new PropertyMetadata((object) ""));
    public static readonly DependencyProperty ImageMarginProperty = DependencyProperty.Register(nameof (ImageMargin), typeof (Thickness), typeof (CustomButton), new PropertyMetadata((object) new Thickness(0.0, 0.0, 5.0, 0.0)));
    public static readonly DependencyProperty IsForceTooltipRequiredProperty = DependencyProperty.Register(nameof (IsForceTooltipRequired), typeof (bool), typeof (CustomButton), new PropertyMetadata((object) false));
    internal CustomButton mButton;
    private bool _contentLoaded;

    public CustomButton()
    {
      this.InitializeComponent();
    }

    public CustomButton(ButtonColors color)
      : this()
    {
      this.ButtonColor = color;
    }

    public ButtonColors ButtonColor
    {
      get
      {
        return (ButtonColors) this.GetValue(CustomButton.ButtonColorProperty);
      }
      set
      {
        this.SetValue(CustomButton.ButtonColorProperty, (object) value);
      }
    }

    public bool IsMouseDown
    {
      get
      {
        return (bool) this.GetValue(CustomButton.IsMouseDownProperty);
      }
      set
      {
        this.SetValue(CustomButton.IsMouseDownProperty, (object) value);
      }
    }

    private void Button_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
      this.IsMouseDown = true;
    }

    private void Button_PreviewMouseUp(object sender, MouseButtonEventArgs e)
    {
      this.IsMouseDown = false;
    }

    public ButtonImageOrder ImageOrder
    {
      get
      {
        return (ButtonImageOrder) this.GetValue(CustomButton.ImageOrderProperty);
      }
      set
      {
        this.SetValue(CustomButton.ImageOrderProperty, (object) value);
      }
    }

    public string ImageName
    {
      get
      {
        return (string) this.GetValue(CustomButton.ImageNameProperty);
      }
      set
      {
        this.SetValue(CustomButton.ImageNameProperty, (object) value);
      }
    }

    public Thickness ImageMargin
    {
      get
      {
        return (Thickness) this.GetValue(CustomButton.ImageMarginProperty);
      }
      set
      {
        this.SetValue(CustomButton.ImageMarginProperty, (object) value);
      }
    }

    public bool IsForceTooltipRequired
    {
      get
      {
        return (bool) this.GetValue(CustomButton.IsForceTooltipRequiredProperty);
      }
      set
      {
        this.SetValue(CustomButton.IsForceTooltipRequiredProperty, (object) value);
      }
    }

    private void ButtonTextBlock_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      this.ToolTipIfTextTrimmed();
    }

    private void ButtonTextBlock_Loaded(object sender, RoutedEventArgs e)
    {
      this.ToolTipIfTextTrimmed();
    }

    private void ToolTipIfTextTrimmed()
    {
      if (this.IsForceTooltipRequired)
        return;
      ContentPresenter visualChild = WpfUtils.FindVisualChild<ContentPresenter>((DependencyObject) this);
      if (visualChild == null)
        return;
      if (visualChild.ContentTemplate.FindName("buttonTextBlock", (FrameworkElement) visualChild) is TextBlock name && name.IsTextTrimmed())
        ToolTipService.SetIsEnabled((DependencyObject) this, true);
      else
        ToolTipService.SetIsEnabled((DependencyObject) this, false);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/HD-Common;component/uielements/custombutton.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      if (connectionId == 1)
      {
        this.mButton = (CustomButton) target;
        this.mButton.PreviewMouseDown += new MouseButtonEventHandler(this.Button_PreviewMouseDown);
        this.mButton.PreviewMouseUp += new MouseButtonEventHandler(this.Button_PreviewMouseUp);
      }
      else
        this._contentLoaded = true;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IStyleConnector.Connect(int connectionId, object target)
    {
      if (connectionId != 2)
        return;
      ((FrameworkElement) target).Loaded += new RoutedEventHandler(this.ButtonTextBlock_Loaded);
      ((FrameworkElement) target).SizeChanged += new SizeChangedEventHandler(this.ButtonTextBlock_SizeChanged);
    }
  }
}
