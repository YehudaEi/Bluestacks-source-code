// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.Mouse
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using System.Windows.Forms;

namespace BlueStacks.Player
{
  public class Mouse
  {
    private static Mouse mInstance;
    private uint x;
    private uint y;
    private bool b0;
    private bool b1;
    private bool b2;

    public static Mouse Instance
    {
      get
      {
        if (Mouse.mInstance == null)
          Mouse.mInstance = new Mouse();
        return Mouse.mInstance;
      }
    }

    public Mouse()
    {
      this.x = 0U;
      this.y = 0U;
      this.b0 = false;
      this.b1 = false;
      this.b2 = false;
    }

    public void UpdateCursor(uint x, uint y)
    {
      this.x = x;
      this.y = y;
    }

    public void UpdateButton(uint x, uint y, MouseButtons button, bool pressed)
    {
      this.x = x;
      this.y = y;
      switch (button)
      {
        case MouseButtons.Left:
          this.b0 = pressed;
          break;
        case MouseButtons.Right:
          this.b1 = pressed;
          break;
        case MouseButtons.Middle:
          this.b2 = pressed;
          break;
      }
    }

    public uint X
    {
      get
      {
        return this.x;
      }
    }

    public uint Y
    {
      get
      {
        return this.y;
      }
    }

    public uint Mask
    {
      get
      {
        uint num = 0;
        if (this.b0)
          num |= 1U;
        if (this.b1)
          num |= 2U;
        if (this.b2)
          num |= 4U;
        return num;
      }
    }
  }
}
