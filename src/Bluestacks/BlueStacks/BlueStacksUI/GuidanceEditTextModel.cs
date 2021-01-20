﻿// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.GuidanceEditTextModel
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System;

namespace BlueStacks.BlueStacksUI
{
  [Serializable]
  public class GuidanceEditTextModel : GuidanceEditModel
  {
    private TextValidityOptions mTextValidityOption = TextValidityOptions.Success;

    public TextValidityOptions TextValidityOption
    {
      get
      {
        return this.mTextValidityOption;
      }
      set
      {
        this.SetProperty<TextValidityOptions>(ref this.mTextValidityOption, value, (string) null);
      }
    }
  }
}
