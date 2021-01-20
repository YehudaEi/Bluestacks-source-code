// Decompiled with JetBrains decompiler
// Type: BlueStacks.Core.RelayKeyBinding
// Assembly: BlueStacks.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C36AABCB-E7F4-4E86-A72F-981C56431F94
// Assembly location: C:\Program Files\BlueStacks\BlueStacks.Core.dll

using System.Windows;
using System.Windows.Input;

namespace BlueStacks.Core
{
  public class RelayKeyBinding : KeyBinding
  {
    public static readonly DependencyProperty CommandBindingProperty = DependencyProperty.Register(nameof (CommandBinding), typeof (ICommand), typeof (RelayKeyBinding), (PropertyMetadata) new FrameworkPropertyMetadata(new PropertyChangedCallback(RelayKeyBinding.OnCommandBindingChanged)));

    public ICommand CommandBinding
    {
      get
      {
        return (ICommand) this.GetValue(RelayKeyBinding.CommandBindingProperty);
      }
      set
      {
        this.SetValue(RelayKeyBinding.CommandBindingProperty, (object) value);
      }
    }

    private static void OnCommandBindingChanged(
      DependencyObject d,
      DependencyPropertyChangedEventArgs e)
    {
      ((InputBinding) d).Command = (ICommand) e.NewValue;
    }
  }
}
