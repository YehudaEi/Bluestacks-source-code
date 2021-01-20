// Decompiled with JetBrains decompiler
// Type: TapRepeat
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using System;
using System.ComponentModel;

[Description("Independent")]
[Serializable]
public class TapRepeat : IMAction
{
  private double mX = -1.0;
  private double mY = -1.0;
  private int mCount = 5;
  private int mDelay = 100;
  internal bool mShowOnOverlay = true;
  private bool mRepeatUntilKeyUp = true;
  private string mKey;
  private string mKey_alt1;

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

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public string Key_alt1
  {
    get
    {
      return this.mKey_alt1;
    }
    set
    {
      this.mKey_alt1 = value;
    }
  }

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public int Count
  {
    get
    {
      return this.mCount;
    }
    set
    {
      this.mCount = value;
    }
  }

  public int Delay
  {
    get
    {
      return this.mDelay;
    }
    set
    {
      this.mDelay = 1000 / (2 * this.Count);
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
  public bool RepeatUntilKeyUp
  {
    get
    {
      return this.mRepeatUntilKeyUp;
    }
    set
    {
      this.mRepeatUntilKeyUp = value;
    }
  }
}
