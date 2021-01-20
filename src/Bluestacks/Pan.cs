// Decompiled with JetBrains decompiler
// Type: Pan
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using System;
using System.ComponentModel;
using System.Windows.Input;

[Description("ParentElement")]
[Serializable]
public class Pan : IMAction
{
  private double mX = -1.0;
  private double mY = -1.0;
  private string mKeyStartStop = IMAPKeys.GetStringForFile(Key.F1);
  private string mKeyStartStop_1 = string.Empty;
  private string mKeySuspend = IMAPKeys.GetStringForFile(Key.LeftAlt);
  private string mKeySuspend_1 = string.Empty;
  private double mLookAroundX = -1.0;
  private double mLookAroundY = -1.0;
  private string mKeyLookAround = IMAPKeys.GetStringForFile(Key.V);
  private double mLButtonX = -1.0;
  private double mLButtonY = -1.0;
  private string mKeyAction = "MouseLButton";
  private double mSensitivity = 1.0;
  private double mSensitivityRatioY = 1.0;
  internal bool mShowOnOverlay = true;
  private string mGamepadStick = "";
  private double mGamepadSensitivity = 1.0;
  internal LookAround mLookAround;
  internal PanShoot mPanShoot;
  private int mTweaks;
  private bool mMouseAcceleration;

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

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public string KeyStartStop
  {
    get
    {
      return this.mKeyStartStop;
    }
    set
    {
      this.mKeyStartStop = value;
    }
  }

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public string KeyStartStop_alt1
  {
    get
    {
      return this.mKeyStartStop_1;
    }
    set
    {
      this.mKeyStartStop_1 = value;
    }
  }

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public string KeySuspend
  {
    get
    {
      return this.mKeySuspend;
    }
    set
    {
      this.mKeySuspend = value;
    }
  }

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public string KeySuspend_alt1
  {
    get
    {
      return this.mKeySuspend_1;
    }
    set
    {
      this.mKeySuspend_1 = value;
    }
  }

  public double LookAroundX
  {
    get
    {
      return this.mLookAroundX;
    }
    set
    {
      this.mLookAroundX = value;
      this.CheckLookAround();
    }
  }

  public double LookAroundY
  {
    get
    {
      return this.mLookAroundY;
    }
    set
    {
      this.mLookAroundY = value;
      this.CheckLookAround();
    }
  }

  public string KeyLookAround
  {
    get
    {
      return this.mKeyLookAround;
    }
    set
    {
      this.mKeyLookAround = value;
    }
  }

  public double LButtonX
  {
    get
    {
      return this.mLButtonX;
    }
    set
    {
      this.mLButtonX = value;
      this.CheckShootOnClick();
    }
  }

  public double LButtonY
  {
    get
    {
      return this.mLButtonY;
    }
    set
    {
      this.mLButtonY = value;
      this.CheckShootOnClick();
    }
  }

  internal string KeyAction
  {
    get
    {
      return this.mKeyAction;
    }
  }

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public double Sensitivity
  {
    get
    {
      return this.mSensitivity;
    }
    set
    {
      this.mSensitivity = value;
    }
  }

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public int Tweaks
  {
    get
    {
      return this.mTweaks;
    }
    set
    {
      this.mTweaks = value;
    }
  }

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public double SensitivityRatioY
  {
    get
    {
      return this.mSensitivityRatioY;
    }
    set
    {
      this.mSensitivityRatioY = value;
    }
  }

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  internal bool IsLookAroundEnabled
  {
    get
    {
      return this.mLookAround != null;
    }
  }

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  internal bool IsShootOnClickEnabled
  {
    get
    {
      return this.mPanShoot != null;
    }
  }

  private void CheckLookAround()
  {
    if (this.mLookAroundX == -1.0 && this.mLookAroundY == -1.0)
    {
      this.mLookAround = (LookAround) null;
    }
    else
    {
      if (this.mLookAround != null)
        return;
      this.mLookAround = new LookAround(this);
    }
  }

  private void CheckShootOnClick()
  {
    if (this.mLButtonX == -1.0 && this.mLButtonY == -1.0)
    {
      this.mPanShoot = (PanShoot) null;
    }
    else
    {
      if (this.mPanShoot != null)
        return;
      this.mPanShoot = new PanShoot(this);
    }
  }

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public bool ShowOnOverlay
  {
    get
    {
      return this.mShowOnOverlay;
    }
    set
    {
      this.mShowOnOverlay = value;
    }
  }

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public bool MouseAcceleration
  {
    get
    {
      return this.mMouseAcceleration;
    }
    set
    {
      this.mMouseAcceleration = value;
    }
  }

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public string GamepadStick
  {
    get
    {
      return this.mGamepadStick;
    }
    set
    {
      this.mGamepadStick = value;
    }
  }

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public double GamepadSensitivity
  {
    get
    {
      return this.mGamepadSensitivity;
    }
    set
    {
      this.mGamepadSensitivity = value;
    }
  }

  public string LButtonXExpr { get; set; }

  public string LButtonYExpr { get; set; }

  public string LButtonXOverlayOffset { get; set; }

  public string LButtonYOverlayOffset { get; set; }

  public string LookAroundXExpr { get; set; }

  public string LookAroundYExpr { get; set; }

  public string LookAroundXOverlayOffset { get; set; }

  public string LookAroundYOverlayOffset { get; set; }

  public string LButtonShowOnOverlayExpr { get; set; }

  public string LookAroundShowOnOverlayExpr { get; set; }
}
