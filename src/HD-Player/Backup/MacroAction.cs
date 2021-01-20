// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.MacroAction
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using BlueStacks.Common;
using System;
using System.Windows.Forms;

namespace BlueStacks.Player
{
  [Serializable]
  public class MacroAction
  {
    private MouseButtons mMouseButton = MouseButtons.Left;
    private double mDelayFromLastAction;
    private ActionType mActionType;
    private double mActionPointX;
    private double mActionPointY;
    private Keys mActionKey;

    public double DelayFromLastAction
    {
      get
      {
        return this.mDelayFromLastAction;
      }
      set
      {
        this.mDelayFromLastAction = value;
      }
    }

    public ActionType ActionType
    {
      get
      {
        return this.mActionType;
      }
      set
      {
        this.mActionType = value;
      }
    }

    public double ActionPointX
    {
      get
      {
        return this.mActionPointX;
      }
      set
      {
        this.mActionPointX = value;
      }
    }

    public double ActionPointY
    {
      get
      {
        return this.mActionPointY;
      }
      set
      {
        this.mActionPointY = value;
      }
    }

    public MouseButtons MouseButton
    {
      get
      {
        return this.mMouseButton;
      }
      set
      {
        this.mMouseButton = value;
      }
    }

    public Keys ActionKey
    {
      get
      {
        return this.mActionKey;
      }
      set
      {
        this.mActionKey = value;
      }
    }
  }
}
