// Decompiled with JetBrains decompiler
// Type: BlueStacks.Core.LayoutItemBase
// Assembly: BlueStacks.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C36AABCB-E7F4-4E86-A72F-981C56431F94
// Assembly location: C:\Program Files\BlueStacks\BlueStacks.Core.dll

namespace BlueStacks.Core
{
  public abstract class LayoutItemBase : UiViewModelBase
  {
    private static int sOrder = 1;
    private int mOrder;

    public int Order
    {
      get
      {
        return this.mOrder;
      }
      set
      {
        this.mOrder = value;
        this.RaisePropertyChanged(nameof (Order));
      }
    }

    public LayoutItemBase()
    {
      this.Order = LayoutItemBase.sOrder++;
    }

    public LayoutItemBase(int order)
    {
      this.Order = order;
    }
  }
}
