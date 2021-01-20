// Decompiled with JetBrains decompiler
// Type: BlueStacks.LogCollector.Opt
// Assembly: HD-LogCollector, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: FD44426D-0F78-46B3-8118-1E64B979C51B
// Assembly location: C:\Program Files\BlueStacks\HD-LogCollector.exe

using BlueStacks.Common;

namespace BlueStacks.LogCollector
{
  public class Opt : GetOpt
  {
    public string Vmname { get; set; } = "Android";

    public bool Boot { get; set; }

    public bool Thin { get; set; }

    public bool Apk { get; set; }

    public bool D { get; set; }

    public bool Silent { get; set; }

    public bool Extra { get; set; }

    public bool Hidden { get; set; }

    public bool LogAllOems { get; set; }

    public bool StartAllOems { get; set; }

    public string Source { get; set; } = string.Empty;

    public bool QuickLogs { get; set; }
  }
}
