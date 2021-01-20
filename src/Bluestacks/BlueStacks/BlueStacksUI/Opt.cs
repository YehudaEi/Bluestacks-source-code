// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.Opt
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System;

namespace BlueStacks.BlueStacksUI
{
  public sealed class Opt : GetOpt
  {
    private static object syncRoot = new object();
    private string json = "";
    private static volatile Opt instance;

    public string vmname { get; set; } = "Android";

    public bool h { get; set; }

    public bool mergeCfg { get; set; }

    public bool isForceInstall { get; set; }

    public string newPDPath { get; set; } = string.Empty;

    public bool isUpgradeFromImap13 { get; set; }

    public bool force { get; set; }

    public bool launchedFromSysTray { get; set; }

    public string Json
    {
      get
      {
        return this.json;
      }
      set
      {
        this.json = Uri.UnescapeDataString(value);
      }
    }

    public bool hiddenBootMode { get; set; }

    private Opt()
    {
    }

    public static Opt Instance
    {
      get
      {
        if (Opt.instance == null)
        {
          lock (Opt.syncRoot)
          {
            if (Opt.instance == null)
              Opt.instance = new Opt();
          }
        }
        return Opt.instance;
      }
    }
  }
}
