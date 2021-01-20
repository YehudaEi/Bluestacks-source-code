// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.Keyboard
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace BlueStacks.Player
{
  public class Keyboard
  {
    private static Keyboard mInstance;
    private Dictionary<Keys, bool> escapeSet;

    public static Keyboard Instance
    {
      get
      {
        if (Keyboard.mInstance == null)
          Keyboard.mInstance = new Keyboard();
        return Keyboard.mInstance;
      }
    }

    [DllImport("user32.dll")]
    private static extern uint MapVirtualKey(uint code, uint mapType);

    public Keyboard()
    {
      this.escapeSet = new Dictionary<Keys, bool>()
      {
        {
          Keys.LWin,
          true
        },
        {
          Keys.RWin,
          true
        },
        {
          Keys.Apps,
          true
        },
        {
          Keys.Home,
          true
        },
        {
          Keys.End,
          true
        },
        {
          Keys.Prior,
          true
        },
        {
          Keys.Next,
          true
        },
        {
          Keys.Left,
          true
        },
        {
          Keys.Right,
          true
        },
        {
          Keys.Up,
          true
        },
        {
          Keys.Down,
          true
        }
      };
    }

    public uint NativeToScanCodes(Keys key)
    {
      uint num = Keyboard.MapVirtualKey((uint) (key & Keys.KeyCode), 0U);
      return !this.NeedEscape(key) ? num : 57344U | num;
    }

    private bool NeedEscape(Keys key)
    {
      return this.escapeSet.ContainsKey(key);
    }

    public bool IsAltDepressed()
    {
      return (Control.ModifierKeys & Keys.Alt) == Keys.Alt;
    }
  }
}
