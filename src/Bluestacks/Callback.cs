// Decompiled with JetBrains decompiler
// Type: Callback
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using System;
using System.ComponentModel;

[Description("Independent")]
[Serializable]
public class Callback : IMAction
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
  public string Id { get; set; } = "";

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public string Action { get; set; } = "";

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
