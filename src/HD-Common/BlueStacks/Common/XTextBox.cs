// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.XTextBox
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace BlueStacks.Common
{
  public class XTextBox : TextBox
  {
    public static readonly DependencyProperty InputTextValidityProperty = DependencyProperty.Register(nameof (InputTextValidity), typeof (TextValidityOptions), typeof (XTextBox), new PropertyMetadata((object) TextValidityOptions.Success));
    public static readonly DependencyProperty WatermarkTextProperty = DependencyProperty.Register(nameof (WatermarkText), typeof (string), typeof (XTextBox), new PropertyMetadata((object) "", new PropertyChangedCallback(XTextBox.OnWatermarkTextChangedCallback)));
    public static readonly DependencyProperty SelectAllOnStartProperty = DependencyProperty.Register(nameof (SelectAllOnStart), typeof (bool), typeof (XTextBox), new PropertyMetadata((object) true));
    public static readonly DependencyProperty ErrorIfNullOrEmptyProperty = DependencyProperty.Register(nameof (ErrorIfNullOrEmpty), typeof (bool), typeof (XTextBox), new PropertyMetadata((object) false));
    private TextBlock mTextBlock;

    static XTextBox()
    {
      FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (XTextBox), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (XTextBox)));
    }

    public TextValidityOptions InputTextValidity
    {
      get
      {
        return (TextValidityOptions) this.GetValue(XTextBox.InputTextValidityProperty);
      }
      set
      {
        this.SetValue(XTextBox.InputTextValidityProperty, (object) value);
      }
    }

    public string WatermarkText
    {
      get
      {
        return (string) this.GetValue(XTextBox.WatermarkTextProperty);
      }
      set
      {
        this.SetValue(XTextBox.WatermarkTextProperty, (object) value);
      }
    }

    private static void OnWatermarkTextChangedCallback(
      DependencyObject sender,
      DependencyPropertyChangedEventArgs args)
    {
      if (!(sender is XTextBox xtextBox))
        return;
      xtextBox.Text = args.NewValue.ToString();
    }

    public bool SelectAllOnStart
    {
      get
      {
        return (bool) this.GetValue(XTextBox.SelectAllOnStartProperty);
      }
      set
      {
        this.SetValue(XTextBox.SelectAllOnStartProperty, (object) value);
      }
    }

    public bool ErrorIfNullOrEmpty
    {
      get
      {
        return (bool) this.GetValue(XTextBox.ErrorIfNullOrEmptyProperty);
      }
      set
      {
        this.SetValue(XTextBox.ErrorIfNullOrEmptyProperty, (object) value);
      }
    }

    protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
    {
      base.OnGotKeyboardFocus(e);
      if (string.Equals(this.Text, this.WatermarkText, StringComparison.InvariantCulture))
      {
        this.Clear();
      }
      else
      {
        if (!this.SelectAllOnStart)
          return;
        this.Dispatcher.BeginInvoke((Delegate) (() => this.SelectAll()), DispatcherPriority.ApplicationIdle);
      }
    }

    protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
    {
      base.OnLostKeyboardFocus(e);
      if (!string.IsNullOrEmpty(this.Text))
        return;
      this.Text = this.WatermarkText;
    }

    public TextBlock TextBlock
    {
      get
      {
        if (this.mTextBlock == null)
          this.mTextBlock = (TextBlock) this.Template.FindName("mTextBlock", (FrameworkElement) this);
        return this.mTextBlock;
      }
    }
  }
}
