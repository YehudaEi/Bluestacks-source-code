﻿// Decompiled with JetBrains decompiler
// Type: SendOriginalKeys
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using System;

[Serializable]
public class SendOriginalKeys : IMAction
{
  private string mComments;

  public string Comments
  {
    get
    {
      return this.mComments;
    }
    set
    {
      this.mComments = value;
    }
  }
}
