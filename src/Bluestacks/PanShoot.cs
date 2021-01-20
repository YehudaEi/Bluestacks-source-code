// Decompiled with JetBrains decompiler
// Type: PanShoot
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using System;
using System.ComponentModel;

[Description("SubElement")]
[Serializable]
internal class PanShoot : IMAction
{
  private Pan mPan;

  public string Key
  {
    get
    {
      return this.mPan.KeyAction;
    }
  }

  [Description("IMAP_CanvasElementY")]
  public double X
  {
    get
    {
      return this.mPan.LButtonX;
    }
    set
    {
      this.mPan.LButtonX = value;
    }
  }

  [Description("IMAP_CanvasElementX")]
  public double Y
  {
    get
    {
      return this.mPan.LButtonY;
    }
    set
    {
      this.mPan.LButtonY = value;
    }
  }

  public string LButtonXExpr
  {
    get
    {
      return this.mPan.LButtonXExpr;
    }
    set
    {
      this.mPan.LButtonXExpr = value;
    }
  }

  public string LButtonYExpr
  {
    get
    {
      return this.mPan.LButtonYExpr;
    }
    set
    {
      this.mPan.LButtonYExpr = value;
    }
  }

  public string LButtonXOverlayOffset
  {
    get
    {
      return this.mPan.LButtonXOverlayOffset;
    }
    set
    {
      this.mPan.LButtonXOverlayOffset = value;
    }
  }

  public string LButtonYOverlayOffset
  {
    get
    {
      return this.mPan.LButtonYOverlayOffset;
    }
    set
    {
      this.mPan.LButtonYOverlayOffset = value;
    }
  }

  public string LButtonShowOnOverlayExpr
  {
    get
    {
      return this.mPan.LButtonShowOnOverlayExpr;
    }
    set
    {
      this.mPan.LButtonShowOnOverlayExpr = value;
    }
  }

  internal PanShoot(Pan action)
  {
    this.IsChildAction = true;
    this.Type = KeyActionType.PanShoot;
    this.mPan = action;
    this.ParentAction = (IMAction) action;
  }
}
