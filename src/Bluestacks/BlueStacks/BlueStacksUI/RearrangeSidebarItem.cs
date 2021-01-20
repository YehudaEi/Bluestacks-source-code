// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.RearrangeSidebarItem
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Core;

namespace BlueStacks.BlueStacksUI
{
  internal class RearrangeSidebarItem : LayoutItemBase
  {
    private string mImageName;

    public string ImageName
    {
      get
      {
        return this.mImageName;
      }
      set
      {
        this.mImageName = value;
        this.RaisePropertyChanged(nameof (ImageName));
      }
    }
  }
}
