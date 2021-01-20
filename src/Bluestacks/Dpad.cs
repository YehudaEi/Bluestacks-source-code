// Decompiled with JetBrains decompiler
// Type: Dpad
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.BlueStacksUI;
using BlueStacks.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Input;

[Description("Independent")]
[Serializable]
public class Dpad : IMAction
{
  internal static List<Dpad> sListDpad = new List<Dpad>();
  private double mX = -1.0;
  private double mY = -1.0;
  internal bool mShowOnOverlay = true;

  public Dpad()
  {
    this.Type = KeyActionType.Dpad;
    Dpad.sListDpad.Add(this);
  }

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

  [Description("IMAP_CanvasElementRadiusIMAP_PopupUIElement")]
  [Category("Fields")]
  public double XRadius { get; set; } = 6.0;

  [JsonIgnore]
  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public string DpadTitle
  {
    get
    {
      return (LocaleStrings.GetLocalizedString(KMManager.GetStringsToShowInUI(this.KeyUp.ToString((IFormatProvider) CultureInfo.InvariantCulture)), "") + " " + LocaleStrings.GetLocalizedString(KMManager.GetStringsToShowInUI(this.KeyLeft.ToString((IFormatProvider) CultureInfo.InvariantCulture)), "") + " " + LocaleStrings.GetLocalizedString(KMManager.GetStringsToShowInUI(this.KeyDown.ToString((IFormatProvider) CultureInfo.InvariantCulture)), "") + " " + LocaleStrings.GetLocalizedString(KMManager.GetStringsToShowInUI(this.KeyRight.ToString((IFormatProvider) CultureInfo.InvariantCulture)), "")).Trim();
    }
  }

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public string KeyUp { get; set; } = IMAPKeys.GetStringForFile(Key.W);

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public string KeyUp_alt1 { get; set; } = string.Empty;

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public string KeyLeft { get; set; } = IMAPKeys.GetStringForFile(Key.A);

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public string KeyLeft_alt1 { get; set; } = string.Empty;

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public string KeyDown { get; set; } = IMAPKeys.GetStringForFile(Key.S);

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public string KeyDown_alt1 { get; set; } = string.Empty;

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public string KeyRight { get; set; } = IMAPKeys.GetStringForFile(Key.D);

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public string KeyRight_alt1 { get; set; } = string.Empty;

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public string GamepadStick { get; set; } = "";

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public string KeySpeedModifier1 { get; set; }

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public string KeySpeedModifier1_alt1 { get; set; }

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public double XRadius1 { get; set; }

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public string KeySpeedModifier2 { get; set; }

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public string KeySpeedModifier2_alt1 { get; set; }

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public double XRadius2 { get; set; }

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public double Speed { get; set; } = 200.0;

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public int ActivationTime { get; set; }

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public double ActivationSpeed { get; set; }

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public double DeadzoneRadius { get; set; }

  [Description("IMAP_DeveloperModeUIElemnt")]
  [Category("Fields")]
  public int Tweaks { get; set; }

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
