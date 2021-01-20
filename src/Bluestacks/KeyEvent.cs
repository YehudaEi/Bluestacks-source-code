// Decompiled with JetBrains decompiler
// Type: KeyEvent
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using System;

[Serializable]
public class KeyEvent : IMAction
{
  private string mKey;
  private int mHoldTime;
  private object mKeyDownEvents;
  private object mKeyUpEvents;

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

  public int HoldTime
  {
    get
    {
      return this.mHoldTime;
    }
    set
    {
      this.mHoldTime = value;
    }
  }

  public object KeyDownEvents
  {
    get
    {
      return this.mKeyDownEvents;
    }
    set
    {
      this.mKeyDownEvents = value;
    }
  }

  public object KeyUpEvents
  {
    get
    {
      return this.mKeyUpEvents;
    }
    set
    {
      this.mKeyUpEvents = value;
    }
  }
}
