// Decompiled with JetBrains decompiler
// Type: BlueStacks.Core.TextBoxEnterKeyUpdateBehavior
// Assembly: BlueStacks.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C36AABCB-E7F4-4E86-A72F-981C56431F94
// Assembly location: C:\Program Files\BlueStacks\BlueStacks.Core.dll

using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace BlueStacks.Core
{
  public class TextBoxEnterKeyUpdateBehavior : Behavior<TextBox>
  {
    protected override void OnAttached()
    {
      if (this.AssociatedObject == null)
        return;
      base.OnAttached();
      this.AssociatedObject.KeyDown += new KeyEventHandler(this.AssociatedObject_KeyDown);
    }

    protected override void OnDetaching()
    {
      if (this.AssociatedObject == null)
        return;
      this.AssociatedObject.KeyDown -= new KeyEventHandler(this.AssociatedObject_KeyDown);
      base.OnDetaching();
    }

    private void AssociatedObject_KeyDown(object sender, KeyEventArgs e)
    {
      if (!(sender is TextBox textBox) || e.Key != Key.Return && e.Key != Key.Return)
        return;
      textBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
    }
  }
}
