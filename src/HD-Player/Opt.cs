// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.Opt
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using BlueStacks.Common;

namespace BlueStacks.Player
{
  public sealed class Opt : GetOpt
  {
    private static object syncRoot = new object();
    public string vmname = "Android";
    public bool h;
    public bool help;
    public bool w;
    public bool sysPrep;
    private static volatile Opt instance;

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
