// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.Stats
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using System;
using System.Globalization;

namespace BlueStacks.BlueStacksUI
{
  internal static class Stats
  {
    private static string sSessionId;

    private static string SessionId
    {
      get
      {
        if (Stats.sSessionId == null)
          Stats.ResetSessionId();
        return Stats.sSessionId;
      }
      set
      {
        Stats.sSessionId = value;
      }
    }

    public static string GetSessionId()
    {
      return Stats.SessionId;
    }

    public static string ResetSessionId()
    {
      Stats.SessionId = Stats.Timestamp;
      return Stats.SessionId;
    }

    private static string Timestamp
    {
      get
      {
        DateTime now = DateTime.Now;
        long ticks1 = now.Ticks;
        now = DateTime.Parse("01/01/1970 00:00:00", (IFormatProvider) CultureInfo.InvariantCulture);
        long ticks2 = now.Ticks;
        return ((ticks1 - ticks2) / 10000000L).ToString((IFormatProvider) CultureInfo.InvariantCulture);
      }
    }
  }
}
