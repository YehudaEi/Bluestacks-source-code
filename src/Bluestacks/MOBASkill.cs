// Decompiled with JetBrains decompiler
// Type: MOBASkill
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;

[Description("ParentElement")]
[Serializable]
public class MOBASkill : IMAction
{
  private double mCancelX = -1.0;
  private double mCancelY = -1.0;
  internal bool mShowOnOverlay = true;
  internal MOBASkillCancel mMOBASkillCancel;

  internal static MOBADpad MOBADpad
  {
    get
    {
      foreach (MOBADpad mobaDpad in MOBADpad.sListMOBADpad)
      {
        if (mobaDpad.OriginX != -1.0 && mobaDpad.OriginY != -1.0)
          return mobaDpad;
      }
      return (MOBADpad) null;
    }
  }

  [Description("IMAP_CanvasElementYIMAP_PopupUIElement")]
  [Category("Fields")]
  public double X { get; set; } = -1.0;

  [Description("IMAP_CanvasElementXIMAP_PopupUIElement")]
  [Category("Fields")]
  public double Y { get; set; } = -1.0;

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public string GamepadStick { get; set; } = "";

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public string KeyActivate { get; set; }

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public string KeyActivate_alt1 { get; set; }

  [Description("IMAP_DeveloperModeUIElemnt")]
  [Category("Fields")]
  public string KeyAutocastToggle { get; set; }

  [Description("IMAP_DeveloperModeUIElemnt")]
  [Category("Fields")]
  public string KeyAutocastToggle_alt1 { get; set; }

  [Description("IMAP_DeveloperModeUIElemnt")]
  [Category("Fields")]
  public double YAxisRatio { get; set; }

  public string KeyCancel { get; set; } = IMAPKeys.GetStringForFile(Key.Space);

  public string KeyCancel_alt1 { get; set; } = string.Empty;

  [Description("IMAP_DeveloperModeUIElemnt")]
  [Category("Fields")]
  public int Tweaks { get; set; } = 1;

  public double CancelX
  {
    get
    {
      return this.mCancelX;
    }
    set
    {
      this.mCancelX = value;
      this.CheckSkillCancel();
    }
  }

  public double CancelY
  {
    get
    {
      return this.mCancelY;
    }
    set
    {
      this.mCancelY = value;
      this.CheckSkillCancel();
    }
  }

  [Description("IMAP_CanvasElementRadiusIMAP_PopupUIElement")]
  [Category("Fields")]
  public double XRadius { get; set; } = 5.0;

  [Description("IMAP_DeveloperModeUIElemnt")]
  [Category("Fields")]
  public double DeadZoneRadius { get; set; }

  [Description("IMAP_DeveloperModeUIElemnt")]
  [Category("Fields")]
  public double CancelSpeed { get; set; } = 500.0;

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  internal bool IsCancelSkillEnabled
  {
    get
    {
      return this.mMOBASkillCancel != null;
    }
  }

  private void CheckSkillCancel()
  {
    if (this.mCancelX == -1.0 && this.mCancelY == -1.0)
    {
      this.mMOBASkillCancel = (MOBASkillCancel) null;
    }
    else
    {
      if (this.mMOBASkillCancel != null)
        return;
      this.mMOBASkillCancel = new MOBASkillCancel(this);
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

  [Description("IMAP_DeveloperModeUIElemnt")]
  [Category("Fields")]
  public int NoCancelOnSwitch { get; set; }

  [Description("IMAP_DeveloperModeUIElemnt")]
  [Category("Fields")]
  public int NoCancelTime { get; set; }

  [Description("IMAP_DeveloperModeUIElemnt")]
  [Category("Fields")]
  public bool AutoAttack { get; set; }

  [Description("IMAP_PopupUIElement")]
  [Category("Fields")]
  public bool StopMOBADpad { get; set; }

  [Description("IMAP_DeveloperModeUIElemnt")]
  [Category("Fields")]
  public bool AdvancedMode { get; set; } = true;

  [Description("IMAP_DeveloperModeUIElemnt")]
  [Category("Fields")]
  public bool AutocastEnabled { get; set; } = true;

  [Description("IMAP_DeveloperModeUIElemnt")]
  [Category("Fields")]
  public int MinSkillTime { get; set; }

  [Description("IMAP_DeveloperModeUIElemnt")]
  [Category("Fields")]
  public double MinSwipeRadius { get; set; }

  [Description("IMAP_DeveloperModeUIElemnt")]
  [Category("Fields")]
  public int MinSkillHoldTime { get; set; }

  [Description("IMAP_DeveloperModeUIElemnt")]
  [Category("Fields")]
  public double Speed { get; set; } = 200.0;

  [Description("IMAP_DeveloperModeUIElemnt")]
  [Category("Fields")]
  public double OriginX { get; set; } = 50.0;

  [Description("IMAP_DeveloperModeUIElemnt")]
  [Category("Fields")]
  public double OriginY { get; set; } = 50.0;

  [Description("IMAP_DeveloperModeUIElemnt")]
  [Category("Fields")]
  public List<double> ExtraData { get; private set; } = new List<double>();

  [Description("IMAP_DeveloperModeUIElemnt")]
  [Category("Fields")]
  public string OriginXExpr { get; set; } = "";

  [Description("IMAP_DeveloperModeUIElemnt")]
  [Category("Fields")]
  public string OriginYExpr { get; set; } = "";

  public string CancelXExpr { get; set; } = "";

  public string CancelYExpr { get; set; } = "";

  public string CancelXOverlayOffset { get; set; } = "";

  public string CancelYOverlayOffset { get; set; } = "";

  public string CancelShowOnOverlayExpr { get; set; }
}
