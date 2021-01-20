// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.SerialWorkQueue
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Timers;

namespace BlueStacks.Common
{
  public class SerialWorkQueue
  {
    private static int sAutoId;
    private Thread mThread;
    private Queue<SerialWorkQueue.Work> mQueue;
    private object mLock;
    private SerialWorkQueue.ExceptionHandlerCallback mExceptionHandler;

    public SerialWorkQueue()
    {
      this.Initialize(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "SerialWorkQueue.{0}", (object) Interlocked.Increment(ref SerialWorkQueue.sAutoId)));
    }

    public SerialWorkQueue(string name)
    {
      this.Initialize(name);
    }

    private void Initialize(string name)
    {
      this.mQueue = new Queue<SerialWorkQueue.Work>();
      this.mLock = new object();
      this.mThread = new Thread(new ThreadStart(this.Run))
      {
        Name = name,
        IsBackground = true
      };
    }

    public SerialWorkQueue.ExceptionHandlerCallback ExceptionHandler
    {
      set
      {
        this.mExceptionHandler = value;
      }
    }

    public void Start()
    {
      if (this.mThread.IsAlive)
        return;
      this.mThread.Start();
    }

    public void Join()
    {
      this.mThread.Join();
    }

    public void Stop()
    {
      this.Enqueue((SerialWorkQueue.Work) null);
    }

    public void Enqueue(SerialWorkQueue.Work work)
    {
      lock (this.mLock)
      {
        this.mQueue.Enqueue(work);
        Monitor.PulseAll(this.mLock);
      }
    }

    public void DispatchAsync(SerialWorkQueue.Work work)
    {
      this.Enqueue(work);
    }

    public void DispatchAfter(double delay, SerialWorkQueue.Work work)
    {
      System.Timers.Timer timer = new System.Timers.Timer();
      timer.Elapsed += (ElapsedEventHandler) ((source, evt) =>
      {
        this.DispatchSync(work);
        timer.Close();
      });
      timer.Interval = delay;
      timer.Enabled = true;
    }

    public void DispatchSync(SerialWorkQueue.Work work)
    {
      EventWaitHandle waitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
      this.Enqueue((SerialWorkQueue.Work) (() =>
      {
        work();
        waitHandle.Set();
      }));
      waitHandle.WaitOne();
      waitHandle.Close();
    }

    public bool IsCurrentWorkQueue()
    {
      return Thread.CurrentThread == this.mThread;
    }

    private void Run()
    {
      while (true)
      {
        SerialWorkQueue.Work work;
        lock (this.mLock)
        {
          while (this.mQueue.Count == 0)
            Monitor.Wait(this.mLock);
          work = this.mQueue.Dequeue();
        }
        if (work != null)
        {
          try
          {
            work();
          }
          catch (Exception ex)
          {
            if (this.mExceptionHandler != null)
              this.mExceptionHandler(ex);
            else
              throw;
          }
        }
        else
          break;
      }
    }

    public delegate void Work();

    public delegate void ExceptionHandlerCallback(Exception exc);
  }
}
