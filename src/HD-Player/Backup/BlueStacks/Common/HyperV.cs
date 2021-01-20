// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.HyperV
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using System.Runtime.InteropServices;

namespace BlueStacks.Common
{
  public class HyperV
  {
    private static object syncRoot = new object();
    private static HyperV sInstance;
    public HyperV.ReturnCodes HyperVStatus;

    [DllImport("HD-Common-Native.dll", SetLastError = true)]
    private static extern int IsHyperVEnabled();

    public static HyperV Instance
    {
      get
      {
        if (HyperV.sInstance == null)
        {
          lock (HyperV.syncRoot)
          {
            if (HyperV.sInstance == null)
            {
              HyperV hyperV = new HyperV();
              hyperV.SetValues();
              HyperV.sInstance = hyperV;
            }
          }
        }
        return HyperV.sInstance;
      }
    }

    private void SetValues()
    {
      this.HyperVStatus = HyperV.GetHyperVStatus();
    }

    private static HyperV.ReturnCodes GetHyperVStatus()
    {
      Logger.Info("Checking Hyper-V in system");
      int num = HyperV.IsHyperVEnabled();
      Logger.Info("IsHyperVEnabled: {0}", (object) num);
      return (HyperV.ReturnCodes) num;
    }

    public bool IsAnyHyperVPresent
    {
      get
      {
        return (uint) this.HyperVStatus > 0U;
      }
    }

    public bool IsMicrosoftHyperVPresent
    {
      get
      {
        return this.HyperVStatus == HyperV.ReturnCodes.MicrosoftHyperV;
      }
    }

    public enum ReturnCodes
    {
      None,
      MicrosoftHyperV,
      OtherHyperV,
    }
  }
}
