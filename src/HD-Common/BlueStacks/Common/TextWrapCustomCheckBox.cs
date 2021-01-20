// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.TextWrapCustomCheckBox
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
using System.Windows.Media;

namespace BlueStacks.Common
{
  public class TextWrapCustomCheckBox : UserControl, IComponentConnector
  {
    public static readonly DependencyProperty TextBlockForegroundProperty = DependencyProperty.RegisterAttached(nameof (CheckBoxTextBlockForeground), typeof (SolidColorBrush), typeof (TextWrapCustomCheckBox), new PropertyMetadata((object) Brushes.White, new PropertyChangedCallback(TextWrapCustomCheckBox.OnForegroundPropertyChanged)));
    public static readonly DependencyProperty TextBlockTextProperty = DependencyProperty.Register(nameof (CheckBoxTextBlockText), typeof (string), typeof (TextWrapCustomCheckBox), new PropertyMetadata((object) "Agree", new PropertyChangedCallback(TextWrapCustomCheckBox.OnTextPropertyChanged)));
    private bool mIsChecked;
    private CheckBoxType mCheckBoxType;
    internal CustomPictureBox mCheckBoxImage;
    internal TextBlock mCheckBoxContent;
    private bool _contentLoaded;

    public SolidColorBrush CheckBoxTextBlockForeground
    {
      get
      {
        return (SolidColorBrush) this.GetValue(TextWrapCustomCheckBox.TextBlockForegroundProperty);
      }
      set
      {
        this.SetValue(TextWrapCustomCheckBox.TextBlockForegroundProperty, (object) value);
      }
    }

    private static void OnForegroundPropertyChanged(
      DependencyObject d,
      DependencyPropertyChangedEventArgs e)
    {
      if (!(d is TextWrapCustomCheckBox wrapCustomCheckBox))
        Logger.Debug("custom check box is null");
      else
        wrapCustomCheckBox.mCheckBoxContent.Foreground = (Brush) (e.NewValue as SolidColorBrush);
    }

    public string CheckBoxTextBlockText
    {
      get
      {
        return (string) this.GetValue(TextWrapCustomCheckBox.TextBlockTextProperty);
      }
      set
      {
        this.SetValue(TextWrapCustomCheckBox.TextBlockTextProperty, (object) value);
      }
    }

    private static void OnTextPropertyChanged(
      DependencyObject d,
      DependencyPropertyChangedEventArgs e)
    {
      if (!(d is TextWrapCustomCheckBox wrapCustomCheckBox))
        Logger.Debug("custom check box is null");
      else
        wrapCustomCheckBox.mCheckBoxContent.Text = e.NewValue as string;
    }

    public TextWrapCustomCheckBox()
    {
      this.InitializeComponent();
    }

    public bool IsChecked
    {
      get
      {
        return this.mIsChecked;
      }
      set
      {
        this.mIsChecked = value;
        this.UpdateImage();
      }
    }

    internal CheckBoxType CheckBoxType
    {
      get
      {
        return this.mCheckBoxType;
      }
      set
      {
        this.mCheckBoxType = value;
        this.UpdateImage();
      }
    }

    private void UpdateImage()
    {
      this.mCheckBoxImage.ImageName = this.CheckBoxType != CheckBoxType.White ? (!this.IsChecked ? "unchecked_gray" : "checked_gray") : (!this.IsChecked ? "unchecked_white" : "checked_white");
    }

    private void CustomCheckBox_MouseDown(object sender, MouseButtonEventArgs e)
    {
      if (this.IsChecked)
        this.IsChecked = false;
      else
        this.IsChecked = true;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/HD-Common;component/uielements/textwrapcustomcheckbox.xaml", UriKind.Relative));
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
          ((UIElement) target).MouseDown += new MouseButtonEventHandler(this.CustomCheckBox_MouseDown);
          break;
        case 2:
          this.mCheckBoxImage = (CustomPictureBox) target;
          break;
        case 3:
          this.mCheckBoxContent = (TextBlock) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
