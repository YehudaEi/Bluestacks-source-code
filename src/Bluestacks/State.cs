// Decompiled with JetBrains decompiler
// Type: State
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using System;
using System.ComponentModel;

[Description("Independent")]
[Serializable]
public class State : IMAction
{
  private double mX = -1.0;
  private double mY = -1.0;
  private string mName = string.Empty;
  private string mModel = string.Empty;
  private string mKey;
  private string mKey_alt1;
  private int mDelay;

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
  public string Name
  {
    get
    {
      return this.mName;
    }
    set
    {
      this.mName = value;
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
  public string Model
  {
    get
    {
      return this.mModel;
    }
    set
    {
      this.mModel = value;
    }
  }

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public int Delay
  {
    get
    {
      return this.mDelay;
    }
    set
    {
      this.mDelay = value;
    }
  }
}
