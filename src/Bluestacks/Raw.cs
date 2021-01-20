// Decompiled with JetBrains decompiler
// Type: Raw
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using System;

[Serializable]
public class Raw : IMAction
{
  private string mKey;
  private object mSequence;

  public string Key
  {
    get
    {
      return this.mKey;
    }
    set
    {
      this.mKey = value;
    }
  }

  public object Sequence
  {
    get
    {
      return this.mSequence;
    }
    set
    {
      this.mSequence = value;
    }
  }
}
