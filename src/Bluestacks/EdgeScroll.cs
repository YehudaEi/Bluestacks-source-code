// Decompiled with JetBrains decompiler
// Type: EdgeScroll
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using System;
using System.ComponentModel;

[Description("Independent")]
[Serializable]
public class EdgeScroll : IMAction
{
  internal bool mShowOnOverlay;

  [Description("IMAP_CanvasElementYIMAP_PopupUIElement")]
  [Category("Fields")]
  public double X { get; set; } = -1.0;

  [Description("IMAP_CanvasElementXIMAP_PopupUIElement")]
  [Category("Fields")]
  public double Y { get; set; } = -1.0;

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public double XVelocity { get; set; } = 100.0;

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public double YVelocity { get; set; } = 100.0;

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public double XActiveMargin { get; set; } = 3.0;

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public double YActiveMargin { get; set; } = 3.0;

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public int ResetDelay { get; set; } = 150;

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public double SpeedUpFactor { get; set; } = 2.0;

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public int SpeedUpWaitTime { get; set; } = 200;

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public bool EdgeScrollEnabled { get; set; } = true;

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
}
