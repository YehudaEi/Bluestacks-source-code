// Decompiled with JetBrains decompiler
// Type: MOBAHeroDummy
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using System;
using System.ComponentModel;

[Description("SubElementDependent")]
[Serializable]
internal class MOBAHeroDummy : IMAction
{
  internal MOBADpad mMOBADpad;

  [Description("IMAP_CanvasElementYIMAP_PopupUIElement")]
  [Category("Fields")]
  public double X
  {
    get
    {
      return this.mMOBADpad.OriginX;
    }
    set
    {
      this.mMOBADpad.OriginX = value;
    }
  }

  [Description("IMAP_CanvasElementXIMAP_PopupUIElement")]
  [Category("Fields")]
  public double Y
  {
    get
    {
      return this.mMOBADpad.OriginY;
    }
    set
    {
      this.mMOBADpad.OriginY = value;
    }
  }

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public double CharSpeed { get; set; } = 10.0;

  internal MOBAHeroDummy(MOBADpad action)
  {
    this.IsChildAction = true;
    this.Type = KeyActionType.MOBAHeroDummy;
    this.mMOBADpad = action;
    this.ParentAction = (IMAction) action;
  }
}
