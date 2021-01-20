// Decompiled with JetBrains decompiler
// Type: Zoom
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using System;
using System.ComponentModel;
using System.Windows.Input;

[Description("Independent")]
[Serializable]
public class Zoom : IMAction
{
  private double mX = -1.0;
  private double mY = -1.0;
  private double mX1 = -1.0;
  private double mY1 = -1.0;
  private double mX2 = -1.0;
  private double mY2 = -1.0;
  private double mRadius = 20.0;
  private string mKeyIn = IMAPKeys.GetStringForFile(Key.OemPlus);
  private string mKeyIn_1 = string.Empty;
  private string mKeyOut = IMAPKeys.GetStringForFile(Key.OemMinus);
  private string mKeyOut_1 = string.Empty;
  private string mKeyWheel = IMAPKeys.GetStringForFile(Key.LeftCtrl) + " + MouseWheel";
  private double mSpeed = 1.0;
  private double mAmplitude = 20.0;
  private double mAcceleration = 1.0;
  private bool mOverride = true;
  private string mKeyModifier;
  private string mKeyModifier_1;
  private int mMode;

  [Description("IMAP_CanvasElementYIMAP_PopupUIElement")]
  [Category("Fields")]
  internal double X
  {
    get
    {
      this.mX = this.mX1 != -1.0 || this.mX2 != -1.0 ? (this.Direction == Direction.Left || this.Direction == Direction.Right ? this.mX1 + this.mRadius : this.mX1) : -1.0;
      return this.mX;
    }
    set
    {
      this.mX = value;
      if (this.Direction == Direction.Right)
      {
        this.mX2 = Math.Round(this.mX + this.mRadius, 2);
        this.mX1 = Math.Round(this.mX - this.mRadius, 2);
      }
      else
      {
        if (this.Direction != Direction.Up)
          return;
        this.mX1 = Math.Round(this.mX, 2);
        this.mX2 = this.X1;
      }
    }
  }

  [Description("IMAP_CanvasElementXIMAP_PopupUIElement")]
  [Category("Fields")]
  internal double Y
  {
    get
    {
      this.mY = this.mY1 != -1.0 || this.mY2 != -1.0 ? (this.Direction == Direction.Up || this.Direction == Direction.Down ? this.mY1 + this.mRadius : this.mY1) : -1.0;
      return this.mY;
    }
    set
    {
      this.mY = value;
      if (this.Direction == Direction.Right)
      {
        this.mY1 = Math.Round(this.mY, 2);
        this.mY2 = this.Y1;
      }
      else
      {
        if (this.Direction != Direction.Up)
          return;
        this.mY2 = Math.Round(this.mY + this.mRadius, 2);
        this.mY1 = Math.Round(this.mY - this.mRadius, 2);
      }
    }
  }

  public double X1
  {
    get
    {
      return this.mX1;
    }
    set
    {
      this.mX1 = value;
      this.CheckDirection();
      if (this.Direction != Direction.Up && this.Direction != Direction.Down)
        return;
      this.mX2 = this.X1;
    }
  }

  public double Y1
  {
    get
    {
      return this.mY1;
    }
    set
    {
      this.mY1 = value;
      this.CheckDirection();
      if (this.Direction != Direction.Left && this.Direction != Direction.Right)
        return;
      this.mY2 = this.Y1;
    }
  }

  [Category("Fields")]
  internal double Size
  {
    get
    {
      return this.Radius * 2.0;
    }
    set
    {
      this.Radius = value / 2.0;
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

  [Description("IMAP_CanvasElementRadius")]
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
      if (this.Direction == Direction.Right)
      {
        this.mX2 = Math.Round(this.mX + this.mRadius, 2);
        this.mX1 = Math.Round(this.mX - this.mRadius, 2);
        this.mY1 = Math.Round(this.mY, 2);
        this.mY2 = this.Y1;
      }
      else
      {
        if (this.Direction != Direction.Up)
          return;
        this.mY2 = Math.Round(this.mY + this.mRadius, 2);
        this.mY1 = Math.Round(this.mY - this.mRadius, 2);
        this.mX1 = Math.Round(this.mX, 2);
        this.mX2 = this.X1;
      }
    }
  }

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public string KeyIn
  {
    get
    {
      return this.mKeyIn;
    }
    set
    {
      this.mKeyIn = value;
    }
  }

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public string KeyIn_alt1
  {
    get
    {
      return this.mKeyIn_1;
    }
    set
    {
      this.mKeyIn_1 = value;
    }
  }

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public string KeyOut
  {
    get
    {
      return this.mKeyOut;
    }
    set
    {
      this.mKeyOut = value;
    }
  }

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public string KeyOut_alt1
  {
    get
    {
      return this.mKeyOut_1;
    }
    set
    {
      this.mKeyOut_1 = value;
    }
  }

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public string KeyWheel
  {
    get
    {
      return this.mKeyWheel;
    }
    set
    {
      this.mKeyWheel = value;
    }
  }

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public string KeyModifier
  {
    get
    {
      return this.mKeyModifier;
    }
    set
    {
      this.mKeyModifier = value;
    }
  }

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public string KeyModifier_alt1
  {
    get
    {
      return this.mKeyModifier_1;
    }
    set
    {
      this.mKeyModifier_1 = value;
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
  public double Amplitude
  {
    get
    {
      return this.mAmplitude;
    }
    set
    {
      this.mAmplitude = value;
    }
  }

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public double Acceleration
  {
    get
    {
      return this.mAcceleration;
    }
    set
    {
      this.mAcceleration = value;
    }
  }

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public int Mode
  {
    get
    {
      return this.mMode;
    }
    set
    {
      this.mMode = value;
    }
  }

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public bool Override
  {
    get
    {
      return this.mOverride;
    }
    set
    {
      this.mOverride = value;
    }
  }

  private void CheckDirection()
  {
    if (this.X1 == this.X2)
    {
      this.Direction = Direction.Up;
      this.mRadius = Math.Round(Math.Abs(this.Y2 - this.Y1) / 2.0, 2);
    }
    else
    {
      if (this.Y1 != this.Y2)
        return;
      this.Direction = Direction.Right;
      this.mRadius = Math.Round(Math.Abs(this.X2 - this.X1) / 2.0, 2);
    }
  }
}
