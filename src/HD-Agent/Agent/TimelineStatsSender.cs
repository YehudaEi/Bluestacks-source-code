// Decompiled with JetBrains decompiler
// Type: BlueStacks.Agent.TimelineStatsSender
// Assembly: HD-Agent, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 06DAED18-1D79-40C2-83F8-3A28B5222574
// Assembly location: C:\Program Files\BlueStacks\HD-Agent.exe

using BlueStacks.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace BlueStacks.Agent
{
  public class TimelineStatsSender
  {
    private static long sSequenceNumber = 1000000L * TimelineStatsSender.UtcToUnixTimestampSecs(DateTime.UtcNow);
    private static Queue<TimelineStatsSender.TimelineEvent> sEventQueue = new Queue<TimelineStatsSender.TimelineEvent>();
    private static Mutex sEventQueueMutex = new Mutex();

    private static long UtcToUnixTimestampSecs(DateTime value)
    {
      return (long) (value - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds;
    }

    [DllImport("kernel32.dll")]
    private static extern long GetTickCount64();

    private static long TicksInSeconds()
    {
      return TimelineStatsSender.GetTickCount64() / 1000L;
    }

    public static void Init(string vmName)
    {
      if (SystemUtils.IsOSWinXP())
      {
        Logger.Warning("TimelineStats: Not supported for WindowsXP");
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

    public static void HandleTopActivityInfo(RequestData data)
    {
      if (SystemUtils.IsOSWinXP() || !RegistryManager.Instance.IsTimelineStats4Enabled)
        return;
      TimelineStatsSender.TimelineEvent timelineEvent = new TimelineStatsSender.TimelineEvent("app-activity", data.Data["packageName"], data.Data["activityName"], "");
      TimelineStatsSender.sEventQueueMutex.WaitOne();
      TimelineStatsSender.sEventQueue.Enqueue(timelineEvent);
      TimelineStatsSender.sEventQueueMutex.ReleaseMutex();
    }

    public static void HandleGuestStatusUpdate(RequestData data)
    {
      if (SystemUtils.IsOSWinXP() || !RegistryManager.Instance.IsTimelineStats4Enabled)
        return;
      Logger.Info("TimelineStats: HandleGuestBootStatusUpdate: {0}", (object) data.Data["event"]);
      TimelineStatsSender.TimelineEvent timelineEvent = new TimelineStatsSender.TimelineEvent("guest-status", data.Data["S1"] == null ? string.Empty : data.Data["S1"], data.Data["S2"] == null ? string.Empty : data.Data["S2"], data.Data["S3"] == null ? string.Empty : data.Data["S3"]);
      Logger.Info("TimelineStats: HandleGuestBootStatusUpdate: S1={0} S2={1} S3={2}", (object) data.Data["S1"], (object) data.Data["S2"], (object) data.Data["S3"]);
      TimelineStatsSender.sEventQueueMutex.WaitOne();
      TimelineStatsSender.sEventQueue.Enqueue(timelineEvent);
      TimelineStatsSender.sEventQueueMutex.ReleaseMutex();
    }

    public static void HandleEngineBootEvent(string eventName)
    {
      Logger.Info("TimelineStats: UpdateEngineBootSate: " + eventName);
      TimelineStatsSender.TimelineEvent timelineEvent = new TimelineStatsSender.TimelineEvent("engine-boot", eventName, string.Empty, string.Empty);
      TimelineStatsSender.sEventQueueMutex.WaitOne();
      TimelineStatsSender.sEventQueue.Enqueue(timelineEvent);
      TimelineStatsSender.sEventQueueMutex.ReleaseMutex();
    }

    public static void HandleFrontendStatusUpdate(RequestData data)
    {
      if (SystemUtils.IsOSWinXP() || !RegistryManager.Instance.IsTimelineStats4Enabled)
        return;
      Logger.Info("TimelineStats: HandleFrontendStatusUpdate: {0}", (object) data.Data["event"]);
      TimelineStatsSender.TimelineEvent timelineEvent = new TimelineStatsSender.TimelineEvent(data.Data["event"], "", "", "");
      TimelineStatsSender.sEventQueueMutex.WaitOne();
      TimelineStatsSender.sEventQueue.Enqueue(timelineEvent);
      TimelineStatsSender.sEventQueueMutex.ReleaseMutex();
    }

    public static void HandleS2PEvents(RequestData data)
    {
      if (SystemUtils.IsOSWinXP() || !RegistryManager.Instance.IsTimelineStats4Enabled)
        return;
      TimelineStatsSender.TimelineEvent timelineEvent = new TimelineStatsSender.TimelineEvent(data.Data["event"], "", "", "");
      TimelineStatsSender.sEventQueueMutex.WaitOne();
      TimelineStatsSender.sEventQueue.Enqueue(timelineEvent);
      TimelineStatsSender.sEventQueueMutex.ReleaseMutex();
    }

    public static void HandleAppInstallEvents(JObject json)
    {
      if (SystemUtils.IsOSWinXP() || !RegistryManager.Instance.IsTimelineStats4Enabled)
        return;
      TimelineStatsSender.TimelineEvent timelineEvent = new TimelineStatsSender.TimelineEvent(string.Compare(json["update"].ToString().Trim(), "true", StringComparison.InvariantCultureIgnoreCase) == 0 ? "app-updated" : "app-installed", json["package"].ToString().Trim(), json["name"].ToString().Trim(), json["source"].ToString().Trim());
      TimelineStatsSender.sEventQueueMutex.WaitOne();
      TimelineStatsSender.sEventQueue.Enqueue(timelineEvent);
      TimelineStatsSender.sEventQueueMutex.ReleaseMutex();
    }

    public static void HandleAppUninstallEvents(JObject json)
    {
      if (SystemUtils.IsOSWinXP() || !RegistryManager.Instance.IsTimelineStats4Enabled)
        return;
      TimelineStatsSender.TimelineEvent timelineEvent = new TimelineStatsSender.TimelineEvent("app-uninstalled", json["package"].ToString().Trim(), "", json["source"].ToString().Trim());
      TimelineStatsSender.sEventQueueMutex.WaitOne();
      TimelineStatsSender.sEventQueue.Enqueue(timelineEvent);
      TimelineStatsSender.sEventQueueMutex.ReleaseMutex();
    }

    public static void HandleNotificationUpdates(
      string evt,
      string package,
      string activity,
      string id)
    {
      if (SystemUtils.IsOSWinXP() || !RegistryManager.Instance.IsTimelineStats4Enabled)
        return;
      TimelineStatsSender.TimelineEvent timelineEvent = new TimelineStatsSender.TimelineEvent(evt, package, activity, id);
      TimelineStatsSender.sEventQueueMutex.WaitOne();
      TimelineStatsSender.sEventQueue.Enqueue(timelineEvent);
      TimelineStatsSender.sEventQueueMutex.ReleaseMutex();
    }

    public static void HandleAdEvents(RequestData data)
    {
      if (SystemUtils.IsOSWinXP() || !RegistryManager.Instance.IsTimelineStats4Enabled)
        return;
      TimelineStatsSender.TimelineEvent timelineEvent = new TimelineStatsSender.TimelineEvent(data.Data["event"], data.Data["status"], data.Data["ecode"], data.Data["estring"]);
      TimelineStatsSender.sEventQueueMutex.WaitOne();
      TimelineStatsSender.sEventQueue.Enqueue(timelineEvent);
      TimelineStatsSender.sEventQueueMutex.ReleaseMutex();
    }

    public static void SendTimelineStats(
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
      if (!RegistryManager.Instance.IsTimelineStats4Enabled)
        return;
      string timezone = TimeZone.CurrentTimeZone.DaylightName;
      string locale = Thread.CurrentThread.CurrentCulture.Name;
      new Thread((ThreadStart) (() => Stats.SendTimelineStats(TimelineStatsSender.UtcToUnixTimestampSecs(timestamp), TimelineStatsSender.sSequenceNumber++, evt, duration, s1, s2, s3, s4, s5, s6, s7, s8, timezone, locale, TimelineStatsSender.UtcToUnixTimestampSecs(fromTimestamp), TimelineStatsSender.UtcToUnixTimestampSecs(toTimestamp), fromTicks, toTicks, vmName)))
      {
        IsBackground = true
      }.Start();
    }

    private static void StatsSenderThread(string vmName)
    {
      DateTime fromTimestamp1 = DateTime.UtcNow;
      long fromTicks1 = TimelineStatsSender.TicksInSeconds();
      bool flag1 = false;
      bool flag2 = false;
      DateTime dateTime = DateTime.UtcNow;
      long toTicks = TimelineStatsSender.TicksInSeconds();
      string str1 = "";
      string str2 = "";
      DateTime fromTimestamp2 = DateTime.UtcNow;
      long fromTicks2 = TimelineStatsSender.TicksInSeconds();
      DateTime utcNow1 = DateTime.UtcNow;
      TimelineStatsSender.TicksInSeconds();
      DateTime utcNow2 = DateTime.UtcNow;
      TimelineStatsSender.TicksInSeconds();
      DateTime utcNow3 = DateTime.UtcNow;
      TimelineStatsSender.TicksInSeconds();
      while (true)
      {
        try
        {
          TimelineStatsSender.sEventQueueMutex.WaitOne();
          if (TimelineStatsSender.sEventQueue.Count <= 0)
          {
            TimelineStatsSender.sEventQueueMutex.ReleaseMutex();
            long num1 = TimelineStatsSender.TicksInSeconds();
            long num2 = toTicks + 600L;
            Logger.Debug("IsUserActive: {0} TimeInSec {1} LastUserActivity {2}", (object) flag2, (object) num1, (object) num2);
            if (flag2 && num1 > num2)
            {
              if (toTicks < fromTicks2)
                Logger.Error("TimelineStats: user-idle: lastUserActivityTicks {0} < currentActivityStartTicks {1}", (object) toTicks, (object) fromTicks2);
              else if (string.Compare(str1, "", true) != 0 && RegistryManager.Instance.IsTimelineStats4Enabled)
              {
                Logger.Info("TimelineStats: Sending stat for user-idle");
                string s1;
                string s2;
                string s3;
                string s4;
                string s5;
                TimelineStatsSender.GetInteractionStatsFromEngine(out s1, out s2, out s3, out s4, out s5, vmName);
                TimelineStatsSender.SendTimelineStats(dateTime, "app-usage", toTicks - fromTicks2, str1, str2, "user-idle", s1, s2, s3, s4, s5, fromTimestamp2, dateTime, fromTicks2, toTicks, vmName);
              }
              flag2 = false;
            }
            Thread.Sleep(1000);
          }
          else
          {
            TimelineStatsSender.TimelineEvent timelineEvent = TimelineStatsSender.sEventQueue.Dequeue();
            TimelineStatsSender.sEventQueueMutex.ReleaseMutex();
            if (string.Compare(timelineEvent.Event, "frontend-launched", true) == 0)
            {
              flag1 = true;
              fromTimestamp1 = timelineEvent.Time;
              fromTicks1 = timelineEvent.Ticks;
              TimelineStatsSender.SendTimelineStats(timelineEvent.Time, timelineEvent.Event, 0L, "none", "none", "none", "none", "none", "none", "none", "none", timelineEvent.Time, timelineEvent.Time, 0L, 0L, vmName);
            }
            else if (string.Compare(timelineEvent.Event, "frontend-ready", true) == 0)
              TimelineStatsSender.SendTimelineStats(timelineEvent.Time, timelineEvent.Event, timelineEvent.Ticks - fromTicks1, "none", "none", "none", "none", "none", "none", "none", "none", fromTimestamp1, timelineEvent.Time, fromTicks1, timelineEvent.Ticks, vmName);
            else if (string.Compare(timelineEvent.Event, "frontend-closed", true) == 0)
            {
              if (flag2 & flag1)
              {
                flag2 = false;
                flag1 = false;
                if (timelineEvent.Ticks < fromTicks2)
                  Logger.Error("TimelineStats: frontend-closed: e.Ticks {0} < currentActivityStartTicks {1}", (object) timelineEvent.Ticks, (object) fromTicks2);
                else if (string.Compare(str1, "", true) != 0 && RegistryManager.Instance.IsTimelineStats4Enabled)
                {
                  string s1;
                  string s2;
                  string s3;
                  string s4;
                  string s5;
                  TimelineStatsSender.GetInteractionStatsFromEngine(out s1, out s2, out s3, out s4, out s5, vmName);
                  TimelineStatsSender.SendTimelineStats(timelineEvent.Time, "app-usage", timelineEvent.Ticks - fromTicks2, str1, str2, "frontend-closed", s1, s2, s3, s4, s5, fromTimestamp2, timelineEvent.Time, fromTicks2, timelineEvent.Ticks, vmName);
                }
              }
              TimelineStatsSender.SendTimelineStats(timelineEvent.Time, timelineEvent.Event, timelineEvent.Ticks - fromTicks1, "none", "none", "none", "none", "none", "none", "none", "none", fromTimestamp1, timelineEvent.Time, fromTicks1, timelineEvent.Ticks, vmName);
              flag1 = false;
              flag2 = false;
              str1 = "";
              str2 = "";
            }
            else if (string.Compare(timelineEvent.Event, "frontend-activated", true) == 0)
            {
              flag1 = true;
              flag2 = true;
              dateTime = timelineEvent.Time;
              toTicks = timelineEvent.Ticks;
              fromTimestamp2 = timelineEvent.Time;
              fromTicks2 = timelineEvent.Ticks;
            }
            else if (string.Compare(timelineEvent.Event, "frontend-deactivated", true) == 0)
            {
              if (flag2)
              {
                if (timelineEvent.Ticks < fromTicks2)
                  Logger.Error("TimelineStats: frontend-deactivated: e.Ticks {0} < currentActivityStartTicks {1}", (object) timelineEvent.Ticks, (object) fromTicks2);
                else if (string.Compare(str1, "", true) != 0 && RegistryManager.Instance.IsTimelineStats4Enabled)
                {
                  string s1;
                  string s2;
                  string s3;
                  string s4;
                  string s5;
                  TimelineStatsSender.GetInteractionStatsFromEngine(out s1, out s2, out s3, out s4, out s5, vmName);
                  TimelineStatsSender.SendTimelineStats(timelineEvent.Time, "app-usage", timelineEvent.Ticks - fromTicks2, str1, str2, "frontend-deactivated", s1, s2, s3, s4, s5, fromTimestamp2, timelineEvent.Time, fromTicks2, timelineEvent.Ticks, vmName);
                }
              }
              flag1 = false;
              flag2 = false;
            }
            else if (string.Compare(timelineEvent.Event, "user-active", true) == 0)
            {
              if (!flag2)
              {
                fromTimestamp2 = timelineEvent.Time;
                fromTicks2 = timelineEvent.Ticks;
              }
              flag1 = true;
              flag2 = true;
              dateTime = timelineEvent.Time;
              toTicks = timelineEvent.Ticks;
            }
            else if (string.Compare(timelineEvent.Event, "app-activity", true) == 0)
            {
              if (string.Compare(timelineEvent.S1, str1, true) == 0)
              {
                if (string.Compare(timelineEvent.S2, str2, true) == 0)
                  continue;
              }
              if (flag2 & flag1 && string.Compare(str1, "", true) != 0)
              {
                if (timelineEvent.Ticks < fromTicks2)
                  Logger.Error("TimelineStats: app-activity: e.Ticks {0} < currentActivityStartTicks {1}", (object) timelineEvent.Ticks, (object) fromTicks2);
                else if (RegistryManager.Instance.IsTimelineStats4Enabled)
                {
                  string s1;
                  string s2;
                  string s3;
                  string s4;
                  string s5;
                  TimelineStatsSender.GetInteractionStatsFromEngine(out s1, out s2, out s3, out s4, out s5, vmName);
                  TimelineStatsSender.SendTimelineStats(timelineEvent.Time, "app-usage", timelineEvent.Ticks - fromTicks2, str1, str2, "new-app-activity", s1, s2, s3, s4, s5, fromTimestamp2, timelineEvent.Time, fromTicks2, timelineEvent.Ticks, vmName);
                }
              }
              str1 = timelineEvent.S1;
              str2 = timelineEvent.S2;
              fromTimestamp2 = timelineEvent.Time;
              fromTicks2 = timelineEvent.Ticks;
            }
            else if (string.Compare(timelineEvent.Event, "guest-status", true) == 0)
            {
              if (RegistryManager.Instance.IsTimelineStats4Enabled)
                TimelineStatsSender.SendTimelineStats(timelineEvent.Time, timelineEvent.S1, timelineEvent.Ticks - fromTicks2, timelineEvent.S2, timelineEvent.S3, "", "", "", "", "", "", fromTimestamp2, timelineEvent.Time, fromTicks2, timelineEvent.Ticks, vmName);
            }
            else if (string.Equals(timelineEvent.Event, "engine-boot", StringComparison.OrdinalIgnoreCase))
              TimelineStatsSender.SendTimelineStats(timelineEvent.Time, timelineEvent.S1, timelineEvent.Ticks - fromTicks2, timelineEvent.S2, timelineEvent.S3, "", "", "", "", "", "", fromTimestamp2, timelineEvent.Time, fromTicks2, timelineEvent.Ticks, vmName);
            else
              Logger.Error("Unknown event {0}", (object) timelineEvent.Event);
          }
        }
        catch (Exception ex)
        {
          Logger.Warning("Exception in sending timeline stats: " + ex.ToString());
          Thread.Sleep(1000);
        }
      }
    }

    private static void GetInteractionStatsFromEngine(
      out string s1,
      out string s2,
      out string s3,
      out string s4,
      out string s5,
      string vmName)
    {
      string engine = HTTPUtils.SendRequestToEngine("getInteractionStats", (Dictionary<string, string>) null, vmName, 0, (Dictionary<string, string>) null, false, 1, 0, "", "bgp");
      Logger.Debug("NATIVE: response received: " + engine);
      JObject jobject = JObject.Parse(engine);
      int num1 = jobject[nameof (s1)].ToObject<int>();
      int num2 = jobject[nameof (s2)].ToObject<int>();
      int num3 = jobject[nameof (s3)].ToObject<int>();
      string str = jobject[nameof (s4)].ToString();
      int num4 = jobject[nameof (s5)].ToObject<int>();
      Logger.Debug("NATIVE: IsNativeGamepadUsed:" + num4.ToString());
      s1 = num1.ToString();
      s2 = num2.ToString();
      s3 = num3.ToString();
      s4 = str;
      s5 = num4.ToString();
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
