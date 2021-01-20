// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.CustomCheckbox
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
using System.Windows.Input;
using System.Windows.Markup;

namespace BlueStacks.Common
{
  public class CustomCheckbox : CheckBox, IComponentConnector, IStyleConnector
  {
    public static readonly Dictionary<string, Tuple<CustomCheckbox, List<CustomCheckbox>, List<CustomCheckbox>>> dictInterminentTags = new Dictionary<string, Tuple<CustomCheckbox, List<CustomCheckbox>, List<CustomCheckbox>>>();
    public static readonly DependencyProperty ImageMarginProperty = DependencyProperty.Register(nameof (ImageMargin), typeof (Thickness), typeof (CustomCheckbox), new PropertyMetadata((object) new Thickness(0.0)));
    public static readonly DependencyProperty TextWrappingProperty = DependencyProperty.Register(nameof (TextWrapping), typeof (TextWrapping), typeof (CustomCheckbox), new PropertyMetadata((object) TextWrapping.NoWrap));
    public static readonly DependencyProperty TextTrimmingProperty = DependencyProperty.Register(nameof (TextTrimming), typeof (TextTrimming), typeof (CustomCheckbox), new PropertyMetadata((object) TextTrimming.CharacterEllipsis));
    public static readonly DependencyProperty TextFontSizeProperty = DependencyProperty.Register(nameof (TextFontSize), typeof (double), typeof (CustomCheckbox), new PropertyMetadata((object) 16.0));
    private string mGroup = string.Empty;
    private bool _mSetInterminate;
    private Image mImage;
    private Orientation mOrientaion;
    private Visibility mLabelVisibility;
    private bool _contentLoaded;

    public string Group
    {
      get
      {
        return this.mGroup;
      }
      set
      {
        this.mGroup = value;
        if (this.IsThreeState)
          CustomCheckbox.dictInterminentTags[this.mGroup] = new Tuple<CustomCheckbox, List<CustomCheckbox>, List<CustomCheckbox>>(this, new List<CustomCheckbox>(), new List<CustomCheckbox>());
        else
          CustomCheckbox.dictInterminentTags[this.Group].Item3.Add(this);
      }
    }

    public void SetInterminate()
    {
      if (!this.IsChecked.HasValue)
        return;
      this._mSetInterminate = true;
      this.IsChecked = new bool?();
    }

    public Image Image
    {
      get
      {
        if (this.mImage == null)
          this.mImage = (Image) this.Template.FindName("mImage", (FrameworkElement) this);
        return this.mImage;
      }
    }

    public ColumnDefinition colDefMargin
    {
      get
      {
        return (ColumnDefinition) this.Template.FindName(nameof (colDefMargin), (FrameworkElement) this);
      }
    }

    public ColumnDefinition colDefHorizontalLabel
    {
      get
      {
        return (ColumnDefinition) this.Template.FindName(nameof (colDefHorizontalLabel), (FrameworkElement) this);
      }
    }

    public TextBlock BottomLabel
    {
      get
      {
        return (TextBlock) this.Template.FindName("VerticalTextBlock", (FrameworkElement) this);
      }
    }

    public TextBlock CheckboxText
    {
      get
      {
        return (TextBlock) this.Template.FindName("HorizontalTextBlock", (FrameworkElement) this);
      }
    }

    public Orientation Orientation
    {
      set
      {
        this.mOrientaion = value;
      }
    }

    public Visibility LabelVisibility
    {
      set
      {
        this.mLabelVisibility = value;
      }
    }

    public CustomCheckbox()
    {
      this.InitializeComponent();
    }

    private void CheckBox_MouseEnter(object sender, MouseEventArgs e)
    {
      if (!this.IsChecked.HasValue || this.IsChecked.Value)
        return;
      CustomPictureBox.SetBitmapImage(this.Image, "check_box_hover", false);
    }

    private void CheckBox_MouseLeave(object sender, MouseEventArgs e)
    {
      if (!this.IsChecked.HasValue)
        CustomPictureBox.SetBitmapImage(this.Image, "check_box_Indeterminate", false);
      else if (this.IsChecked.Value)
        CustomPictureBox.SetBitmapImage(this.Image, "check_box_checked", false);
      else
        CustomPictureBox.SetBitmapImage(this.Image, "check_box", false);
    }

    private void CheckBox_Checked(object sender, RoutedEventArgs e)
    {
      if (!string.IsNullOrEmpty(this.mGroup))
      {
        if (this.IsThreeState)
        {
          if (CustomCheckbox.dictInterminentTags[this.mGroup].Item3.Count > 0)
          {
            foreach (ToggleButton toggleButton in CustomCheckbox.dictInterminentTags[this.mGroup].Item3.ToArray())
              toggleButton.IsChecked = new bool?(true);
          }
        }
        else
        {
          CustomCheckbox.dictInterminentTags[this.mGroup].Item2.Add(this);
          CustomCheckbox.dictInterminentTags[this.mGroup].Item3.Remove(this);
          if (CustomCheckbox.dictInterminentTags[this.mGroup].Item3.Count == 0)
            CustomCheckbox.dictInterminentTags[this.mGroup].Item1.IsChecked = new bool?(true);
          else
            CustomCheckbox.dictInterminentTags[this.mGroup].Item1.SetInterminate();
        }
      }
      if (this.Image == null)
        return;
      CustomPictureBox.SetBitmapImage(this.Image, "check_box_checked", false);
    }

    private void CheckBox_Indeterminate(object sender, RoutedEventArgs e)
    {
      if (!this._mSetInterminate)
      {
        this.IsChecked = new bool?(false);
      }
      else
      {
        this._mSetInterminate = false;
        if (this.Image == null)
          return;
        CustomPictureBox.SetBitmapImage(this.Image, "check_box_Indeterminate", false);
      }
    }

    private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
      if (!string.IsNullOrEmpty(this.mGroup))
      {
        if (this.IsThreeState)
        {
          if (CustomCheckbox.dictInterminentTags[this.mGroup].Item2.Count > 0)
          {
            foreach (ToggleButton toggleButton in CustomCheckbox.dictInterminentTags[this.mGroup].Item2.ToArray())
              toggleButton.IsChecked = new bool?(false);
          }
        }
        else
        {
          CustomCheckbox.dictInterminentTags[this.mGroup].Item2.Remove(this);
          CustomCheckbox.dictInterminentTags[this.mGroup].Item3.Add(this);
          if (CustomCheckbox.dictInterminentTags[this.mGroup].Item2.Count == 0)
            CustomCheckbox.dictInterminentTags[this.mGroup].Item1.IsChecked = new bool?(false);
          else
            CustomCheckbox.dictInterminentTags[this.mGroup].Item1.SetInterminate();
        }
      }
      if (this.IsMouseOver)
      {
        if (this.Image == null)
          return;
        CustomPictureBox.SetBitmapImage(this.Image, "check_box_hover", false);
      }
      else
      {
        if (this.Image == null)
          return;
        CustomPictureBox.SetBitmapImage(this.Image, "check_box", false);
      }
    }

    private void CheckBox_Loaded(object sender, RoutedEventArgs e)
    {
      if (DesignerProperties.GetIsInDesignMode((DependencyObject) this))
        return;
      if (this.mOrientaion == Orientation.Vertical)
      {
        Grid.SetRowSpan((UIElement) this.Image, 1);
        this.colDefHorizontalLabel.Width = new GridLength(0.0);
        this.BottomLabel.Visibility = Visibility.Visible;
      }
      if (this.mLabelVisibility == Visibility.Hidden)
      {
        this.CheckboxText.Visibility = Visibility.Hidden;
        this.BottomLabel.Visibility = Visibility.Hidden;
      }
      if (this.Image == null)
        return;
      bool? isChecked = this.IsChecked;
      if (!isChecked.HasValue)
      {
        CustomPictureBox.SetBitmapImage(this.Image, "check_box__Indeterminate", false);
      }
      else
      {
        isChecked = this.IsChecked;
        if (isChecked.HasValue)
        {
          isChecked = this.IsChecked;
          if (isChecked.Value)
          {
            CustomPictureBox.SetBitmapImage(this.Image, "check_box_checked", false);
            return;
          }
        }
        CustomPictureBox.SetBitmapImage(this.Image, "check_box", false);
      }
    }

    public Thickness ImageMargin
    {
      get
      {
        return (Thickness) this.GetValue(CustomCheckbox.ImageMarginProperty);
      }
      set
      {
        this.SetValue(CustomCheckbox.ImageMarginProperty, (object) value);
      }
    }

    public TextWrapping TextWrapping
    {
      get
      {
        return (TextWrapping) this.GetValue(CustomCheckbox.TextWrappingProperty);
      }
      set
      {
        this.SetValue(CustomCheckbox.TextWrappingProperty, (object) value);
      }
    }

    public TextTrimming TextTrimming
    {
      get
      {
        return (TextTrimming) this.GetValue(CustomCheckbox.TextTrimmingProperty);
      }
      set
      {
        this.SetValue(CustomCheckbox.TextTrimmingProperty, (object) value);
      }
    }

    public double TextFontSize
    {
      get
      {
        return (double) this.GetValue(CustomCheckbox.TextFontSizeProperty);
      }
      set
      {
        this.SetValue(CustomCheckbox.TextFontSizeProperty, (object) value);
      }
    }

    private void CheckBoxText_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      ContentPresenter visualChild = WpfUtils.FindVisualChild<ContentPresenter>((DependencyObject) this);
      if (visualChild == null)
        return;
      if (visualChild.ContentTemplate.FindName("HorizontalTextBlock", (FrameworkElement) visualChild) is TextBlock name && name.IsTextTrimmed())
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
      Application.LoadComponent((object) this, new Uri("/HD-Common;component/uielements/customcheckbox.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      if (connectionId == 1)
      {
        ((UIElement) target).MouseEnter += new MouseEventHandler(this.CheckBox_MouseEnter);
        ((UIElement) target).MouseLeave += new MouseEventHandler(this.CheckBox_MouseLeave);
        ((ToggleButton) target).Checked += new RoutedEventHandler(this.CheckBox_Checked);
        ((ToggleButton) target).Unchecked += new RoutedEventHandler(this.CheckBox_Unchecked);
        ((ToggleButton) target).Indeterminate += new RoutedEventHandler(this.CheckBox_Indeterminate);
        ((FrameworkElement) target).Loaded += new RoutedEventHandler(this.CheckBox_Loaded);
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
      ((FrameworkElement) target).SizeChanged += new SizeChangedEventHandler(this.CheckBoxText_SizeChanged);
    }
  }
}
