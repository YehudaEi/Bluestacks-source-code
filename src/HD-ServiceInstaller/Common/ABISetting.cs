// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.ABISetting
// Assembly: HD-ServiceInstaller, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 15F93427-26B3-4C7E-BAB1-0A00688BC4D4
// Assembly location: C:\Program Files\BlueStacks\HD-ServiceInstaller.exe

using System.ComponentModel;

namespace BlueStacks.Common
{
  public enum ABISetting
  {
    [Description("4")] ARM,
    [Description("15")] Auto,
    [Description("15")] ARM64,
    [Description("7")] Auto64,
    [Description("-1")] Custom,
  }
}
