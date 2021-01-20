// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.MouseHWheel
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using BlueStacks.Common;
using System;

namespace BlueStacks.Player
{
  public class MouseHWheel
  {
    private static MouseHWheel.MouseHWheelCallback s_MouseHWheelCallback;
    private int keyEnableSynaptic;

    public MouseHWheel()
    {
    }

    public MouseHWheel(MouseHWheel.MouseHWheelCallback cb)
    {
      this.setMousehWheelCallback(cb);
    }

    public bool setMousehWheelCallback(MouseHWheel.MouseHWheelCallback cb)
    {
      if (cb == null)
        return false;
      this.keyEnableSynaptic = RegistryManager.Instance.DefaultGuest.HScroll;
      if (this.keyEnableSynaptic != 1)
      {
        Logger.Info("Horizontal Mouse Wheel support is Disabled");
        return false;
      }
      MouseHWheel.s_MouseHWheelCallback = new MouseHWheel.MouseHWheelCallback(cb.Invoke);
      try
      {
        if (!HDPlusModule.SetMouseHWheelCallback(MouseHWheel.s_MouseHWheelCallback))
          Logger.Info("Horizontal scrolling disabled, no synaptic device found");
        return true;
      }
      catch (Exception ex)
      {
        Logger.Error("Continue with MouseHWheel error:");
        Logger.Error(ex.ToString());
      }
      return false;
    }

    public delegate void MouseHWheelCallback(int x, int y, int keyState, int delta);
  }
}
