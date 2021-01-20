// Decompiled with JetBrains decompiler
// Type: DiscordRPC.Helper.BackoffDelay
// Assembly: DiscordRPC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B95F11B0-E129-4510-93B3-855D1EDEA68A
// Assembly location: C:\Program Files\BlueStacks\DiscordRPC.dll

using System;

namespace DiscordRPC.Helper
{
  internal class BackoffDelay
  {
    private int _current;
    private int _fails;

    public int Maximum { get; private set; }

    public int Minimum { get; private set; }

    public int Current
    {
      get
      {
        return this._current;
      }
    }

    public int Fails
    {
      get
      {
        return this._fails;
      }
    }

    public Random Random { get; set; }

    private BackoffDelay()
    {
    }

    public BackoffDelay(int min, int max)
      : this(min, max, new Random())
    {
    }

    public BackoffDelay(int min, int max, Random random)
    {
      this.Minimum = min;
      this.Maximum = max;
      this._current = min;
      this._fails = 0;
      this.Random = random;
    }

    public void Reset()
    {
      this._fails = 0;
      this._current = this.Minimum;
    }

    public int NextDelay()
    {
      ++this._fails;
      this._current = (int) Math.Floor((double) (this.Maximum - this.Minimum) / 100.0 * (double) this._fails) + this.Minimum;
      return Math.Min(Math.Max(this._current, this.Minimum), this.Maximum);
    }
  }
}
