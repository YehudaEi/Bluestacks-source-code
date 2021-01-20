// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.TimelineStatsSender
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using BlueStacks.Common;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace BlueStacks.Player
{
  internal class TimelineStatsSender
  {
    private static Queue<TimelineStatsSender.TimelineEvent> sEventQueue = new Queue<TimelineStatsSender.TimelineEvent>();
    private static Mutex sEventQueueMutex = new Mutex();
    private static long sSequenceNumber = 1000000L * TimelineStatsSender.UtcToUnixTimestampSecs(DateTime.UtcNow);

    [DllImport("kernel32.dll")]
    private static extern long GetTickCount64();

    private static long UtcToUnixTimestampSecs(DateTime value)
    {
      return (long) (value - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds;
    }

    private static long TicksInSeconds()
    {
      return TimelineStatsSender.GetTickCount64() / 1000L;
    }

    internal static void Init(string vmName)
    {
      if (SystemUtils.IsOSWinXP())
        Logger.Warning("TimelineStats: Not supported for WindowsXP");
      else if (!RegistryManager.Instance.IsTimelineStats4Enabled)
      {
        Logger.Warning("TimelineStats are disabled.");
      }
      else
      {
        Logger.Info("TimelineStats: Initalizing: Staring thread.");
        new Thread((ThreadStart) (() => TimelineStatsSender.StatsSenderThread(vmName)))
        {
          IsBackground = true
        }.Start();
      }
    }

    public static void HandleEngineBootEvent(string eventName)
    {
      if (SystemUtils.IsOSWinXP() || !RegistryManager.Instance.IsTimelineStats4Enabled)
        return;
      Logger.Info("TimelineStats: UpdateEngineBootState: " + eventName);
      TimelineStatsSender.TimelineEvent timelineEvent = new TimelineStatsSender.TimelineEvent("engine-boot", eventName, string.Empty, string.Empty);
      TimelineStatsSender.sEventQueue.Enqueue(timelineEvent);
    }

    internal static void SendTimelineStats(
      DateTime timestamp,
      string evt,
      long duration,
      string s1,
      string s2,
      string s3,
      string s4,
      string s5,
      string s6,
      string s7,
      string s8,
      DateTime fromTimestamp,
      DateTime toTimestamp,
      long fromTicks,
      long toTicks,
      string vmName)
    {
      string timezone = TimeZone.CurrentTimeZone.DaylightName;
      string locale = Thread.CurrentThread.CurrentCulture.Name;
      new Thread((ThreadStart) (() =>
      {
        TimelineStatsSender.sEventQueueMutex.WaitOne();
        Stats.SendTimelineStats(TimelineStatsSender.UtcToUnixTimestampSecs(timestamp), TimelineStatsSender.sSequenceNumber++, evt, duration, s1, s2, s3, s4, s5, s6, s7, s8, timezone, locale, TimelineStatsSender.UtcToUnixTimestampSecs(fromTimestamp), TimelineStatsSender.UtcToUnixTimestampSecs(toTimestamp), fromTicks, toTicks, vmName);
        TimelineStatsSender.sEventQueueMutex.ReleaseMutex();
      }))
      {
        IsBackground = true
      }.Start();
    }

    private static void StatsSenderThread(string vmName)
    {
      DateTime utcNow = DateTime.UtcNow;
      long fromTicks = TimelineStatsSender.TicksInSeconds();
      while (true)
      {
        try
        {
          TimelineStatsSender.sEventQueueMutex.WaitOne();
          if (TimelineStatsSender.sEventQueue.Count <= 0)
          {
            TimelineStatsSender.sEventQueueMutex.ReleaseMutex();
            Thread.Sleep(1000);
          }
          else
          {
            TimelineStatsSender.TimelineEvent timelineEvent = TimelineStatsSender.sEventQueue.Dequeue();
            TimelineStatsSender.sEventQueueMutex.ReleaseMutex();
            TimelineStatsSender.SendTimelineStats(timelineEvent.Time, timelineEvent.S1, timelineEvent.Ticks - fromTicks, timelineEvent.S2, timelineEvent.S3, "", "", "", "", "", "", utcNow, timelineEvent.Time, fromTicks, timelineEvent.Ticks, vmName);
          }
        }
        catch (Exception ex)
        {
          Logger.Warning("Exception in sending timeline stats: " + ex.ToString());
          Thread.Sleep(1000);
        }
      }
    }

    private class TimelineEvent
    {
      public DateTime Time;
      public long Ticks;
      public string Event;
      public string S1;
      public string S2;
      public string S3;

      public TimelineEvent(string evt, string s1, string s2, string s3)
      {
        this.Time = DateTime.UtcNow;
        this.Ticks = TimelineStatsSender.TicksInSeconds();
        this.Event = evt;
        this.S1 = s1;
        this.S2 = s2;
        this.S3 = s3;
      }
    }
  }
}
