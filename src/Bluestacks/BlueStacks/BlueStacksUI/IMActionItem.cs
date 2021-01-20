// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.IMActionItem
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System;

namespace BlueStacks.BlueStacksUI
{
  [Serializable]
  public class IMActionItem : ViewModelBase
  {
    private string mActionItem;
    private IMAction mIMAction;

    public string ActionItem
    {
      get
      {
        return this.mActionItem;
      }
      set
      {
        this.SetProperty<string>(ref this.mActionItem, value, (string) null);
      }
    }

    public IMAction IMAction
    {
      get
      {
        return this.mIMAction;
      }
      set
      {
        this.SetProperty<IMAction>(ref this.mIMAction, value, (string) null);
      }
    }
  }
}
