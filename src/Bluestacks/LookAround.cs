// Decompiled with JetBrains decompiler
// Type: LookAround
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using System;
using System.ComponentModel;

[Description("SubElement")]
[Serializable]
internal class LookAround : IMAction
{
  private Pan mPan;

  public string Key
  {
    get
    {
      return this.mPan.KeyLookAround;
    }
    set
    {
      this.mPan.KeyLookAround = value;
    }
  }

  [Description("IMAP_CanvasElementY")]
  public double X
  {
    get
    {
      return this.mPan.LookAroundX;
    }
    set
    {
      this.mPan.LookAroundX = value;
    }
  }

  [Description("IMAP_CanvasElementX")]
  public double Y
  {
    get
    {
      return this.mPan.LookAroundY;
    }
    set
    {
      this.mPan.LookAroundY = value;
    }
  }

  public string LookAroundXExpr
  {
    get
    {
      return this.mPan.LookAroundXExpr;
    }
    set
    {
      this.mPan.LookAroundXExpr = value;
    }
  }

  public string LookAroundYExpr
  {
    get
    {
      return this.mPan.LookAroundYExpr;
    }
    set
    {
      this.mPan.LookAroundYExpr = value;
    }
  }

  public string LookAroundXOverlayOffset
  {
    get
    {
      return this.mPan.LookAroundXOverlayOffset;
    }
    set
    {
      this.mPan.LookAroundXOverlayOffset = value;
    }
  }

  public string LookAroundYOverlayOffset
  {
    get
    {
      return this.mPan.LookAroundYOverlayOffset;
    }
    set
    {
      this.mPan.LookAroundYOverlayOffset = value;
    }
  }

  public string LookAroundShowOnOverlayExpr
  {
    get
    {
      return this.mPan.LookAroundShowOnOverlayExpr;
    }
    set
    {
      this.mPan.LookAroundShowOnOverlayExpr = value;
    }
  }

  internal LookAround(Pan action)
  {
    this.IsChildAction = true;
    this.Type = KeyActionType.LookAround;
    this.mPan = action;
    this.ParentAction = (IMAction) action;
  }
}
