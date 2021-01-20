// Decompiled with JetBrains decompiler
// Type: KeyInput
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using System;

[Serializable]
public class KeyInput : IMAction
{
  private string mKeyIn;
  private string mKeyOut;

  public string KeyIn
  {
    get
    {
      return this.mKeyIn;
    }
    set
    {
      this.mKeyIn = value;
    }
  }

  public string KeyOut
  {
    get
    {
      return this.mKeyOut;
    }
    set
    {
      this.mKeyOut = value;
    }
  }
}
