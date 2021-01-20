// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.AppIconPromotionObject
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;

namespace BlueStacks.BlueStacksUI
{
  public class AppIconPromotionObject
  {
    public string AppPromotionID { get; set; } = string.Empty;

    public GenericAction AppPromotionAction { get; set; } = GenericAction.InstallPlay;

    public string AppPromotionPackage { get; set; } = string.Empty;

    public string AppPromotionName { get; set; } = string.Empty;

    public string AppPromotionActionParam { get; set; } = string.Empty;

    public string AppPromotionImagePath { get; set; } = string.Empty;
  }
}
