// Decompiled with JetBrains decompiler
// Type: Swipe
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System;
using System.ComponentModel;

[Description("Dependent")]
[Serializable]
public class Swipe : IMAction
{
  private double mX1 = -1.0;
  private double mY1 = -1.0;
  private double mRadius = 10.0;
  private double mSpeed = 200.0;
  private double mX2;
  private double mY2;
  private bool mHold;
  private string mKey;
  private string mKey_1;

  [Description("IMAP_CanvasElementYIMAP_PopupUIElement")]
  [Category("Fields")]
  public double X1
  {
    get
    {
      return this.mX1;
    }
    set
    {
      this.mX1 = value;
      if (this.Direction == Direction.Up || this.Direction == Direction.Down)
        this.mX2 = this.X1;
      else if (this.Direction == Direction.Left)
      {
        this.mX2 = Math.Round(this.X1 - this.mRadius, 2);
      }
      else
      {
        if (this.Direction != Direction.Right)
          return;
        this.mX2 = Math.Round(this.X1 + this.mRadius, 2);
      }
    }
  }

  [Description("IMAP_CanvasElementXIMAP_PopupUIElement")]
  [Category("Fields")]
  public double Y1
  {
    get
    {
      return this.mY1;
    }
    set
    {
      this.mY1 = value;
      if (this.Direction == Direction.Left || this.Direction == Direction.Right)
        this.mY2 = this.Y1;
      else if (this.Direction == Direction.Up)
      {
        this.mY2 = Math.Round(this.Y1 - this.mRadius, 2);
      }
      else
      {
        if (this.Direction != Direction.Down)
          return;
        this.mY2 = Math.Round(this.Y1 + this.mRadius, 2);
      }
    }
  }

  public double X2
  {
    get
    {
      return this.mX2;
    }
    set
    {
      this.mX2 = value;
      this.CheckDirection();
    }
  }

  public double Y2
  {
    get
    {
      return this.mY2;
    }
    set
    {
      this.mY2 = value;
      this.CheckDirection();
    }
  }

  [Description("IMAP_CanvasElementRadiusIMAP_PopupUIElement")]
  [Category("Fields")]
  internal double Radius
  {
    get
    {
      return this.mRadius;
    }
    set
    {
      this.mRadius = value;
      if (this.Direction == Direction.Left)
      {
        this.Y2 = this.Y1;
        this.X2 = Math.Round(this.X1 - value, 2);
        Logger.Debug("SWIPE_L: X2: " + this.X2.ToString() + "...............Y2: " + this.Y2.ToString());
      }
      else if (this.Direction == Direction.Right)
      {
        this.Y2 = this.Y1;
        this.X2 = Math.Round(this.X1 + value, 2);
        Logger.Debug("SWIPE_R: X2: " + this.X2.ToString() + "...............Y2: " + this.Y2.ToString());
      }
      else if (this.Direction == Direction.Up)
      {
        this.X2 = this.X1;
        this.Y2 = Math.Round(this.Y1 - value, 2);
        Logger.Debug("SWIPE_U: X2: " + this.X2.ToString() + "...............Y2: " + this.Y2.ToString());
      }
      else
      {
        if (this.Direction != Direction.Down)
          return;
        this.X2 = this.X1;
        this.Y2 = Math.Round(this.Y1 + value, 2);
        Logger.Debug("SWIPE_D: X2: " + this.X2.ToString() + "...............Y2: " + this.Y2.ToString());
      }
    }
  }

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public double Speed
  {
    get
    {
      return this.mSpeed;
    }
    set
    {
      this.mSpeed = value;
    }
  }

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public bool Hold
  {
    get
    {
      return this.mHold;
    }
    set
    {
      this.mHold = value;
    }
  }

  [Description("IMAP_PopupUIElementNotCommon")]
  [Category("Fields")]
  public string Key
  {
    get
    {
      return this.mKey;
    }
    set
    {
      this.mKey = value;
    }
  }

  [Description("IMAP_PopupUIElementNotCommon")]
  [Category("Fields")]
  public string Key_alt1
  {
    get
    {
      return this.mKey_1;
    }
    set
    {
      this.mKey_1 = value;
    }
  }

  private void CheckDirection()
  {
    if (this.X1 == this.X2)
    {
      if (this.Y1 > this.Y2)
      {
        this.Direction = Direction.Up;
        this.mRadius = Math.Round(this.Y1 - this.Y2, 2);
      }
      else
      {
        this.Direction = Direction.Down;
        this.mRadius = Math.Round(this.Y2 - this.Y1, 2);
      }
    }
    else
    {
      if (this.Y1 != this.Y2)
        return;
      if (this.X1 > this.X2)
      {
        this.Direction = Direction.Left;
        this.mRadius = Math.Round(this.X1 - this.X2, 2);
      }
      else
      {
        this.Direction = Direction.Right;
        this.mRadius = Math.Round(this.X2 - this.X1, 2);
      }
    }
  }
}
