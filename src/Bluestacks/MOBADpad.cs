// Decompiled with JetBrains decompiler
// Type: MOBADpad
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using System;
using System.Collections.Generic;
using System.ComponentModel;

[Description("Independent")]
[Serializable]
public class MOBADpad : IMAction
{
  internal static List<MOBADpad> sListMOBADpad = new List<MOBADpad>();
  internal bool mShowOnOverlay = true;
  internal MOBAHeroDummy mMOBAHeroDummy;

  public MOBADpad()
  {
    if (this.OriginX != -1.0 && this.OriginY != -1.0)
      this.mMOBAHeroDummy = new MOBAHeroDummy(this);
    this.Type = KeyActionType.MOBADpad;
    MOBADpad.sListMOBADpad.Add(this);
  }

  [Description("IMAP_CanvasElementYIMAP_PopupUIElement")]
  [Category("Fields")]
  public double X { get; set; } = -1.0;

  [Description("IMAP_CanvasElementXIMAP_PopupUIElement")]
  [Category("Fields")]
  public double Y { get; set; } = -1.0;

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  internal string KeyMove { get; set; } = "MouseRButton";

  [Description("IMAP_CanvasElementRadiusIMAP_PopupUIElement")]
  [Category("Fields")]
  public double XRadius { get; set; } = 6.0;

  public double Speed { get; set; } = 200.0;

  [Description("IMAP_PopupUIElement")]
  [Category("")]
  public double OriginX { get; set; } = -1.0;

  [Description("IMAP_PopupUIElement")]
  [Category("")]
  public double OriginY { get; set; }

  [Description("IMAP_PopupUIElement")]
  [Category("")]
  public double CharSpeed { get; set; } = 10.0;

  public string OriginXExpr { get; set; } = "";

  public string OriginYExpr { get; set; } = "";

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
