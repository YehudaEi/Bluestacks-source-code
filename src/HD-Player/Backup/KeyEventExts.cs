// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.KeyEventExts
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using System.Windows.Forms;
using System.Windows.Input;

namespace BlueStacks.Player
{
  public static class KeyEventExts
  {
    public static System.Windows.Forms.KeyEventArgs ToWinforms(
      this System.Windows.Input.KeyEventArgs keyEventArgs)
    {
      return new System.Windows.Forms.KeyEventArgs((Keys) KeyInterop.VirtualKeyFromKey(keyEventArgs.Key == Key.System ? keyEventArgs.SystemKey : keyEventArgs.Key) | keyEventArgs.KeyboardDevice.Modifiers.ToWinforms());
    }

    private static Keys ToWinforms(this ModifierKeys modifier)
    {
      Keys keys = Keys.None;
      if (modifier == ModifierKeys.Alt)
        keys |= Keys.Alt;
      if (modifier == ModifierKeys.Control)
        keys |= Keys.Control;
      if (modifier == ModifierKeys.Shift)
        keys |= Keys.Shift;
      return keys;
    }
  }
}
