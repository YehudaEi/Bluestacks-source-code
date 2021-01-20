// Decompiled with JetBrains decompiler
// Type: MOBASkillCancel
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using System;
using System.ComponentModel;

[Description("SubElementDependent")]
[Serializable]
internal class MOBASkillCancel : IMAction
{
  internal MOBASkill mMOBASkill;

  public string Key
  {
    get
    {
      return this.mMOBASkill.KeyCancel;
    }
    set
    {
      this.mMOBASkill.KeyCancel = value;
    }
  }

  public string Key_alt1
  {
    get
    {
      return this.mMOBASkill.KeyCancel_alt1;
    }
    set
    {
      this.mMOBASkill.KeyCancel_alt1 = value;
    }
  }

  [Description("IMAP_CanvasElementY")]
  public double X
  {
    get
    {
      return this.mMOBASkill.CancelX;
    }
    set
    {
      this.mMOBASkill.CancelX = value;
    }
  }

  [Description("IMAP_CanvasElementX")]
  public double Y
  {
    get
    {
      return this.mMOBASkill.CancelY;
    }
    set
    {
      this.mMOBASkill.CancelY = value;
    }
  }

  public string MOBASkillCancelXExpr
  {
    get
    {
      return this.mMOBASkill.CancelXExpr;
    }
    set
    {
      this.mMOBASkill.CancelXExpr = value;
    }
  }

  public string MOBASkillCancelYExpr
  {
    get
    {
      return this.mMOBASkill.CancelYExpr;
    }
    set
    {
      this.mMOBASkill.CancelYExpr = value;
    }
  }

  public string MOBASkillCancelOffsetX
  {
    get
    {
      return this.mMOBASkill.CancelXOverlayOffset;
    }
    set
    {
      this.mMOBASkill.CancelXOverlayOffset = value;
    }
  }

  public string MOBASkillCancelOffsetY
  {
    get
    {
      return this.mMOBASkill.CancelYOverlayOffset;
    }
    set
    {
      this.mMOBASkill.CancelYOverlayOffset = value;
    }
  }

  public string MOBASkillShowOnOverlayExpr
  {
    get
    {
      return this.mMOBASkill.CancelShowOnOverlayExpr;
    }
    set
    {
      this.mMOBASkill.CancelShowOnOverlayExpr = value;
    }
  }

  internal MOBASkillCancel(MOBASkill action)
  {
    this.IsChildAction = true;
    this.Type = KeyActionType.MOBASkillCancel;
    this.mMOBASkill = action;
    this.ParentAction = (IMAction) action;
  }
}
