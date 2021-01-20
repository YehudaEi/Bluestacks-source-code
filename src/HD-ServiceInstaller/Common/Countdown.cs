// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.Countdown
// Assembly: HD-ServiceInstaller, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 15F93427-26B3-4C7E-BAB1-0A00688BC4D4
// Assembly location: C:\Program Files\BlueStacks\HD-ServiceInstaller.exe

using System.Threading;

namespace BlueStacks.Common
{
  public class Countdown
  {
    private object _locker = new object();
    private int _value;

    public Countdown()
    {
    }

    public Countdown(int initialCount)
    {
      this._value = initialCount;
    }

    public void Signal()
    {
      this.AddCount(-1);
    }

    public void AddCount(int amount)
    {
      lock (this._locker)
      {
        this._value += amount;
        if (this._value > 0)
          return;
        Monitor.PulseAll(this._locker);
      }
    }

    public void Wait()
    {
      lock (this._locker)
      {
        while (this._value > 0)
          Monitor.Wait(this._locker);
      }
    }
  }
}
