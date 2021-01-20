// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.NumericBehavior
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BlueStacks.Common
{
  public static class NumericBehavior
  {
    public static readonly DependencyProperty IsNumericOnlyProperty = DependencyProperty.RegisterAttached(nameof (IsNumericOnlyProperty), typeof (bool), typeof (NumericBehavior), new PropertyMetadata((object) false, new PropertyChangedCallback(NumericBehavior.OnIsNumericOnlyChanged)));

    public static bool GetIsNumericOnly(DependencyObject obj)
    {
      return (bool) obj?.GetValue(NumericBehavior.IsNumericOnlyProperty);
    }

    public static void SetIsNumericOnly(DependencyObject obj, bool value)
    {
      obj?.SetValue(NumericBehavior.IsNumericOnlyProperty, (object) value);
    }

    private static void OnIsNumericOnlyChanged(
      DependencyObject sender,
      DependencyPropertyChangedEventArgs args)
    {
      TextBox textBox = (TextBox) sender;
      if ((bool) args.NewValue)
      {
        textBox.PreviewTextInput += new TextCompositionEventHandler(NumericBehavior.TextBox_PreviewTextInput);
        textBox.PreviewKeyDown += new KeyEventHandler(NumericBehavior.TextBox_PreviewKeyDown);
        DataObject.AddPastingHandler((DependencyObject) textBox, new DataObjectPastingEventHandler(NumericBehavior.OnPaste));
      }
      else
      {
        textBox.PreviewTextInput -= new TextCompositionEventHandler(NumericBehavior.TextBox_PreviewTextInput);
        textBox.PreviewKeyDown -= new KeyEventHandler(NumericBehavior.TextBox_PreviewKeyDown);
      }
    }

    private static void OnPaste(object sender, DataObjectPastingEventArgs e)
    {
      if (e.DataObject.GetDataPresent(typeof (string)))
      {
        if (NumericBehavior.IsTextAllowed((string) e.DataObject.GetData(typeof (string))))
          return;
        e.CancelCommand();
      }
      else
        e.CancelCommand();
    }

    private static void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key != Key.Space)
        return;
      e.Handled = true;
    }

    private static void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
      e.Handled = !NumericBehavior.IsTextAllowed(e.Text);
    }

    private static bool IsTextAllowed(string text)
    {
      return new Regex("^[0-9]+$").IsMatch(text) && text.IndexOf(' ') == -1;
    }
  }
}
