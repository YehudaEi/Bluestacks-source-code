// Decompiled with JetBrains decompiler
// Type: Tilt
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using System;
using System.ComponentModel;
using System.Windows.Input;

[Description("Independent")]
[Serializable]
public class Tilt : IMAction
{
  private double mX = -1.0;
  private double mY = -1.0;
  private double mRadius = 10.0;
  private string mKeyUp = IMAPKeys.GetStringForFile(Key.Up);
  private string mKeyUp_1 = string.Empty;
  private string mKeyDown = IMAPKeys.GetStringForFile(Key.Down);
  private string mKeyDown_1 = string.Empty;
  private string mKeyLeft = IMAPKeys.GetStringForFile(Key.Left);
  private string mKeyLeft_1 = string.Empty;
  private string mKeyRight = IMAPKeys.GetStringForFile(Key.Right);
  private string mKeyRight_1 = string.Empty;
  private double mMaxAngle = 20.0;
  private double mSpeed = 90.0;
  private bool mAutoReset = true;

  [Description("IMAP_CanvasElementYIMAP_PopupUIElement")]
  [Category("Fields")]
  public double X
  {
    get
    {
      return this.mX;
    }
    set
    {
      this.mX = value;
    }
  }

  [Description("IMAP_CanvasElementXIMAP_PopupUIElement")]
  [Category("Fields")]
  public double Y
  {
    get
    {
      return this.mY;
    }
    set
    {
      this.mY = value;
    }
  }

  [Description("IMAP_CanvasElementRadiusIMAP_PopupUIElement")]
  [Category("Fields")]
  public double Radius
  {
    get
    {
      return this.mRadius;
    }
    set
    {
      this.mRadius = value;
    }
  }

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public string KeyUp
  {
    get
    {
      return this.mKeyUp;
    }
    set
    {
      this.mKeyUp = value;
    }
  }

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public string KeyUp_alt1
  {
    get
    {
      return this.mKeyUp_1;
    }
    set
    {
      this.mKeyUp_1 = value;
    }
  }

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public string KeyDown
  {
    get
    {
      return this.mKeyDown;
    }
    set
    {
      this.mKeyDown = value;
    }
  }

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public string KeyDown_alt1
  {
    get
    {
      return this.mKeyDown_1;
    }
    set
    {
      this.mKeyDown_1 = value;
    }
  }

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public string KeyLeft
  {
    get
    {
      return this.mKeyLeft;
    }
    set
    {
      this.mKeyLeft = value;
    }
  }

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public string KeyLeft_alt1
  {
    get
    {
      return this.mKeyLeft_1;
    }
    set
    {
      this.mKeyLeft_1 = value;
    }
  }

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public string KeyRight
  {
    get
    {
      return this.mKeyRight;
    }
    set
    {
      this.mKeyRight = value;
    }
  }

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public string KeyRight_alt1
  {
    get
    {
      return this.mKeyRight_1;
    }
    set
    {
      this.mKeyRight_1 = value;
    }
  }

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public double MaxAngle
  {
    get
    {
      return this.mMaxAngle;
    }
    set
    {
      this.mMaxAngle = value;
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
  public bool AutoReset
  {
    get
    {
      return this.mAutoReset;
    }
    set
    {
      this.mAutoReset = value;
    }
  }
}
