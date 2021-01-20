// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.Subscription`1
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;

namespace BlueStacks.Common
{
  public class Subscription<T> : IDisposable
  {
    private bool disposedValue;

    public System.Action<T> Action { get; private set; }

    public Subscription(System.Action<T> action)
    {
      this.Action = action;
    }

    protected virtual void Dispose(bool disposing)
    {
      if (this.disposedValue)
        return;
      int num = disposing ? 1 : 0;
      this.disposedValue = true;
    }

    ~Subscription()
    {
      this.Dispose(false);
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }
  }
}
