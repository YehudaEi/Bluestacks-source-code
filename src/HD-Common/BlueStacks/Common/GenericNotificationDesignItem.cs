// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.GenericNotificationDesignItem
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System.Collections.Generic;

namespace BlueStacks.Common
{
  public class GenericNotificationDesignItem
  {
    public double AutoHideTime { get; set; } = 3500.0;

    public string LeftGifName { get; set; }

    public string LeftGifUrl { get; set; }

    public string TitleForeGroundColor { get; set; }

    public string MessageForeGroundColor { get; set; }

    public string BorderColor { get; set; }

    public string Ribboncolor { get; set; }

    public string HoverBorderColor { get; set; }

    public string HoverRibboncolor { get; set; }

    public List<SerializableKeyValuePair<string, double>> BackgroundGradient { get; } = new List<SerializableKeyValuePair<string, double>>();

    public List<SerializableKeyValuePair<string, double>> HoverBackGroundGradient { get; } = new List<SerializableKeyValuePair<string, double>>();
  }
}
