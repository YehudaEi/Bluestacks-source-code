// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.CustomVolumeEventArgs
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.Collections.Generic;

namespace BlueStacks.Common
{
  public class CustomVolumeEventArgs : EventArgs
  {
    public int Volume { get; set; }

    public Dictionary<string, string> dictData { get; set; }

    public string mSelected { get; set; }

    public CustomVolumeEventArgs(int volume)
    {
      this.Volume = volume;
    }

    public CustomVolumeEventArgs(Dictionary<string, string> dict, string selected)
    {
      this.dictData = dict;
      this.mSelected = selected;
    }
  }
}
