// Decompiled with JetBrains decompiler
// Type: BlueStacks.Core.NumericBehavior
// Assembly: BlueStacks.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C36AABCB-E7F4-4E86-A72F-981C56431F94
// Assembly location: C:\Program Files\BlueStacks\BlueStacks.Core.dll

using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace BlueStacks.Core
{
  public class NumericBehavior : Behavior<TextBox>
  {
    protected override void OnAttached()
    {
      this.AssociatedObject.PreviewTextInput += new TextCompositionEventHandler(this.AssociatedObject_PreviewTextInput);
      this.AssociatedObject.PreviewKeyDown += new KeyEventHandler(this.AssociatedObject_PreviewKeyDown);
      DataObject.AddPastingHandler((DependencyObject) this.AssociatedObject, new DataObjectPastingEventHandler(this.OnPaste));
    }

    protected override void OnDetaching()
    {
      this.AssociatedObject.PreviewTextInput -= new TextCompositionEventHandler(this.AssociatedObject_PreviewTextInput);
      this.AssociatedObject.PreviewKeyDown -= new KeyEventHandler(this.AssociatedObject_PreviewKeyDown);
    }

    private void OnPaste(object sender, DataObjectPastingEventArgs e)
    {
      if (e.DataObject.GetDataPresent(typeof (string)))
      {
        if (this.IsTextAllowed((string) e.DataObject.GetData(typeof (string))))
          return;
        e.CancelCommand();
      }
      else
        e.CancelCommand();
    }

    private void AssociatedObject_PreviewKeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key != Key.Space)
        return;
      e.Handled = true;
    }

    private void AssociatedObject_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
      e.Handled = !this.IsTextAllowed(e.Text);
    }

    private bool IsTextAllowed(string text)
    {
      return new Regex("^[0-9]+$").IsMatch(text) && text.IndexOf(' ') == -1;
    }
  }
}
