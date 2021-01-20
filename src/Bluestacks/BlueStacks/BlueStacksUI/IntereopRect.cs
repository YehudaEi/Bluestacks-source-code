// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.IntereopRect
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using System;
using System.Drawing;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace BlueStacks.BlueStacksUI
{
  public struct IntereopRect : IEquatable<IntereopRect>
  {
    public int Left { [IsReadOnly] get; set; }

    public int Top { [IsReadOnly] get; set; }

    public int Right { [IsReadOnly] get; set; }

    public int Bottom { [IsReadOnly] get; set; }

    public IntereopRect(int left, int top, int right, int bottom)
    {
      this.Left = left;
      this.Top = top;
      this.Right = right;
      this.Bottom = bottom;
    }

    public IntereopRect(Rectangle r)
      : this(r.Left, r.Top, r.Right, r.Bottom)
    {
    }

    public int X
    {
      get
      {
        return this.Left;
      }
      set
      {
        this.Right -= this.Left - value;
        this.Left = value;
      }
    }

    public int Y
    {
      get
      {
        return this.Top;
      }
      set
      {
        this.Bottom -= this.Top - value;
        this.Top = value;
      }
    }

    public int Height
    {
      get
      {
        return this.Bottom - this.Top;
      }
      set
      {
        this.Bottom = value + this.Top;
      }
    }

    public int Width
    {
      get
      {
        return this.Right - this.Left;
      }
      set
      {
        this.Right = value + this.Left;
      }
    }

    public Point Location
    {
      get
      {
        return new Point(this.Left, this.Top);
      }
      set
      {
        this.X = value.X;
        this.Y = value.Y;
      }
    }

    public Size Size
    {
      get
      {
        return new Size(this.Width, this.Height);
      }
      set
      {
        this.Width = value.Width;
        this.Height = value.Height;
      }
    }

    public static implicit operator Rectangle(IntereopRect r)
    {
      return new Rectangle(r.Left, r.Top, r.Width, r.Height);
    }

    public static implicit operator IntereopRect(Rectangle r)
    {
      return new IntereopRect(r);
    }

    public static bool operator ==(IntereopRect r1, IntereopRect r2)
    {
      return r1.Equals(new IntereopRect()) ? r2.Equals(new IntereopRect()) : r1.Equals(r2);
    }

    public static bool operator !=(IntereopRect r1, IntereopRect r2)
    {
      return !(r1 == r2);
    }

    public bool Equals(IntereopRect r)
    {
      return r.Left == this.Left && r.Top == this.Top && r.Right == this.Right && r.Bottom == this.Bottom;
    }

    public override bool Equals(object obj)
    {
      switch (obj)
      {
        case IntereopRect r:
          return this.Equals(r);
        case Rectangle r:
          return this.Equals(new IntereopRect(r));
        default:
          return false;
      }
    }

    public override int GetHashCode()
    {
      return ((Rectangle) this).GetHashCode();
    }

    public override string ToString()
    {
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{{Left={0},Top={1},Right={2},Bottom={3}}}", (object) this.Left, (object) this.Top, (object) this.Right, (object) this.Bottom);
    }

    public Rectangle ToRectangle()
    {
      return new Rectangle(this.Left, this.Top, this.Width, this.Height);
    }

    public IntereopRect ToIntereopRect()
    {
      return new IntereopRect((Rectangle) this);
    }
  }
}
