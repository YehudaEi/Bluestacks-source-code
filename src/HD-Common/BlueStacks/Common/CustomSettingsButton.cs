// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.CustomSettingsButton
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;

namespace BlueStacks.Common
{
  public class CustomSettingsButton : Button, IComponentConnector
  {
    private static Dictionary<string, CustomSettingsButton> dictSelecetedButtons = new Dictionary<string, CustomSettingsButton>();
    private bool isSelected;
    private bool showButtonNotification;
    private bool _contentLoaded;

    public CustomSettingsButton()
    {
      this.InitializeComponent();
      this.SetBackground();
      this.Loaded += new RoutedEventHandler(this.CustomSettingsButton_Loaded);
      BlueStacksUIBinding.Instance.PropertyChanged += new PropertyChangedEventHandler(this.BlueStacksUIBinding_PropertyChanged);
    }

    private void CustomSettingsButton_Loaded(object sender, RoutedEventArgs e)
    {
      this.SetNotification();
      this.SetSelectedLine();
    }

    private void BlueStacksUIBinding_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!(e.PropertyName == "LocaleModel"))
        return;
      this.SetSelectedLine();
    }

    public string Group { get; set; } = string.Empty;

    public string ImageName { get; set; } = string.Empty;

    public bool IsSelected
    {
      get
      {
        return this.isSelected;
      }
      set
      {
        this.isSelected = value;
        this.SetBackground();
        this.SetForeGround();
        this.SetSelectedLine();
        if (!this.IsSelected || string.IsNullOrEmpty(this.Group))
          return;
        if (CustomSettingsButton.dictSelecetedButtons.ContainsKey(this.Group))
          CustomSettingsButton.dictSelecetedButtons[this.Group].IsSelected = false;
        CustomSettingsButton.dictSelecetedButtons[this.Group] = this;
      }
    }

    public bool ShowButtonNotification
    {
      get
      {
        return this.showButtonNotification;
      }
      set
      {
        this.showButtonNotification = value;
        this.SetNotification();
      }
    }

    private void SetNotification()
    {
      Ellipse name = (Ellipse) this.Template.FindName("mBtnNotification", (FrameworkElement) this);
      if (name == null)
        return;
      if (this.showButtonNotification)
        name.Visibility = Visibility.Visible;
      else
        name.Visibility = Visibility.Hidden;
    }

    private void SetForeGround()
    {
      if (this.isSelected)
        BlueStacksUIBinding.BindColor((DependencyObject) this, Control.ForegroundProperty, "SettingsWindowTabMenuItemSelectedForeground");
      else
        BlueStacksUIBinding.BindColor((DependencyObject) this, Control.ForegroundProperty, "SettingsWindowTabMenuItemForeground");
    }

    private void SetBackground()
    {
      if (this.IsSelected)
        BlueStacksUIBinding.BindColor((DependencyObject) this, Control.BackgroundProperty, "SettingsWindowTabMenuItemSelectedBackground");
      else
        this.Button_MouseEvent((object) null, (MouseEventArgs) null);
    }

    private void SetSelectedLine()
    {
      try
      {
        Line name1 = (Line) this.Template.FindName("mSelectedLine", (FrameworkElement) this);
        ContentPresenter name2 = (ContentPresenter) this.Template.FindName("contentPresenter", (FrameworkElement) this);
        if (name1 == null)
          return;
        if (this.isSelected)
        {
          name1.Visibility = Visibility.Visible;
          TextBlock content = (TextBlock) name2.Content;
          Typeface typeface = new Typeface(content.FontFamily, content.FontStyle, content.FontWeight, content.FontStretch);
          double trailingWhitespace = new FormattedText(content.Text, Thread.CurrentThread.CurrentCulture, content.FlowDirection, typeface, content.FontSize, content.Foreground).WidthIncludingTrailingWhitespace;
          name1.X2 = trailingWhitespace;
        }
        else
          name1.Visibility = Visibility.Collapsed;
      }
      catch (Exception ex)
      {
      }
    }

    private void Button_MouseEvent(object sender, MouseEventArgs e)
    {
      if (this.IsSelected)
        return;
      if (this.IsMouseOver)
        BlueStacksUIBinding.BindColor((DependencyObject) this, Control.BackgroundProperty, "SettingsWindowTabMenuItemHoverBackground");
      else
        BlueStacksUIBinding.BindColor((DependencyObject) this, Control.BackgroundProperty, "SettingsWindowTabMenuItemBackground");
    }

    private void Button_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
      if (!string.IsNullOrEmpty(this.Group))
        return;
      this.IsSelected = true;
    }

    private void Button_PreviewMouseUp(object sender, MouseButtonEventArgs e)
    {
      if (!string.IsNullOrEmpty(this.Group))
        return;
      this.IsSelected = false;
      this.Button_MouseEvent((object) null, (MouseEventArgs) null);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/HD-Common;component/uielements/customsettingsbutton.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      if (connectionId == 1)
      {
        ((UIElement) target).MouseEnter += new MouseEventHandler(this.Button_MouseEvent);
        ((UIElement) target).MouseLeave += new MouseEventHandler(this.Button_MouseEvent);
        ((UIElement) target).PreviewMouseDown += new MouseButtonEventHandler(this.Button_PreviewMouseDown);
        ((UIElement) target).PreviewMouseUp += new MouseButtonEventHandler(this.Button_PreviewMouseUp);
      }
      else
        this._contentLoaded = true;
    }
  }
}
