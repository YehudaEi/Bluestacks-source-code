// Decompiled with JetBrains decompiler
// Type: FreeLook
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using System;
using System.ComponentModel;

[Description("Independent")]
[Serializable]
public class FreeLook : IMAction
{
  internal bool mShowOnOverlay = true;

  [Description("IMAP_CanvasElementYIMAP_PopupUIElement")]
  [Category("Fields")]
  public double X { get; set; } = -1.0;

  [Description("IMAP_CanvasElementXIMAP_PopupUIElement")]
  [Category("Fields")]
  public double Y { get; set; } = -1.0;

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public string Key { get; set; } = IMAPKeys.GetStringForFile(System.Windows.Input.Key.V);

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public string Key_alt1 { get; set; } = string.Empty;

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public string KeyLeft { get; set; } = IMAPKeys.GetStringForFile(System.Windows.Input.Key.Left);

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public string KeyLeft_alt1 { get; set; } = string.Empty;

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public string KeyRight { get; set; } = IMAPKeys.GetStringForFile(System.Windows.Input.Key.Right);

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public string KeyRight_alt1 { get; set; } = string.Empty;

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public string KeyUp { get; set; } = IMAPKeys.GetStringForFile(System.Windows.Input.Key.Up);

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public string KeyUp_alt1 { get; set; } = string.Empty;

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public string KeyDown { get; set; } = IMAPKeys.GetStringForFile(System.Windows.Input.Key.Down);

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public string KeyDown_alt1 { get; set; } = string.Empty;

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public int DeviceType { get; set; }

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
  public double Sensitivity { get; set; } = 1.0;

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public double Speed { get; set; } = 20.0;

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public bool MouseAcceleration { get; set; }

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public int Delay { get; set; } = 50;
}
