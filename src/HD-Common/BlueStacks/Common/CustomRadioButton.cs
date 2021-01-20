// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.CustomRadioButton
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace BlueStacks.Common
{
  public class CustomRadioButton : RadioButton, IComponentConnector, IStyleConnector
  {
    public static readonly DependencyProperty ImageNameProperty = DependencyProperty.Register(nameof (ImageName), typeof (string), typeof (CustomRadioButton), new PropertyMetadata((object) ""));
    public static readonly DependencyProperty TextMarginProperty = DependencyProperty.Register(nameof (TextMargin), typeof (Thickness), typeof (CustomRadioButton), new PropertyMetadata((object) new Thickness(10.0, 0.0, 0.0, 0.0)));
    private bool _contentLoaded;

    public CustomRadioButton()
    {
      this.InitializeComponent();
    }

    public Thickness TextMargin
    {
      get
      {
        return (Thickness) this.GetValue(CustomRadioButton.TextMarginProperty);
      }
      set
      {
        this.SetValue(CustomRadioButton.TextMarginProperty, (object) value);
      }
    }

    public string ImageName
    {
      get
      {
        return (string) this.GetValue(CustomRadioButton.ImageNameProperty);
      }
      set
      {
        this.SetValue(CustomRadioButton.ImageNameProperty, (object) value);
      }
    }

    public CustomPictureBox RadioBtnImage
    {
      get
      {
        return (CustomPictureBox) this.Template.FindName("mRadioBtnImage", (FrameworkElement) this);
      }
    }

    private void ContentPresenter_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      try
      {
        if (sender == null || !(VisualTreeHelper.GetChild((DependencyObject) (sender as ContentPresenter), 0) is TextBlock child))
          return;
        if (child.IsTextTrimmed())
          ToolTipService.SetIsEnabled((DependencyObject) this, true);
        else
          ToolTipService.SetIsEnabled((DependencyObject) this, false);
      }
      catch (Exception ex)
      {
      }
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/HD-Common;component/uielements/customradiobutton.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      this._contentLoaded = true;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IStyleConnector.Connect(int connectionId, object target)
    {
      if (connectionId != 1)
        return;
      ((FrameworkElement) target).SizeChanged += new SizeChangedEventHandler(this.ContentPresenter_SizeChanged);
    }
  }
}
