// Decompiled with JetBrains decompiler
// Type: Scroll
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using System;
using System.ComponentModel;

[Description("Independent")]
[Serializable]
public class Scroll : IMAction
{
  private double mX = -1.0;
  private double mY = -1.0;
  private double mSpeed = 100.0;
  private double mAmplitude = 20.0;
  private bool mOverride = true;

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
  public double Amplitude
  {
    get
    {
      return this.mAmplitude;
    }
    set
    {
      this.mAmplitude = value;
    }
  }

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public bool Override
  {
    get
    {
      return this.mOverride;
    }
    set
    {
      this.mOverride = value;
    }
  }
}
