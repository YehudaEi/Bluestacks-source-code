// Decompiled with JetBrains decompiler
// Type: DiscordRPC.Timestamps
// Assembly: DiscordRPC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B95F11B0-E129-4510-93B3-855D1EDEA68A
// Assembly location: C:\Program Files\BlueStacks\DiscordRPC.dll

using Newtonsoft.Json;
using System;

namespace DiscordRPC
{
  [Serializable]
  public class Timestamps
  {
    public static Timestamps Now
    {
      get
      {
        return new Timestamps(DateTime.UtcNow, new DateTime?());
      }
    }

    public static Timestamps FromTimeSpan(double seconds)
    {
      return Timestamps.FromTimeSpan(TimeSpan.FromSeconds(seconds));
    }

    public static Timestamps FromTimeSpan(TimeSpan timespan)
    {
      return new Timestamps()
      {
        Start = new DateTime?(DateTime.UtcNow),
        End = new DateTime?(DateTime.UtcNow + timespan)
      };
    }

    [JsonIgnore]
    public DateTime? Start { get; set; }

    [JsonIgnore]
    public DateTime? End { get; set; }

    public Timestamps()
    {
      this.Start = new DateTime?();
      this.End = new DateTime?();
    }

    public Timestamps(DateTime start, DateTime? end = null)
    {
      this.Start = new DateTime?(start);
      this.End = end;
    }

    [JsonProperty("start", NullValueHandling = NullValueHandling.Ignore)]
    public ulong? StartUnixMilliseconds
    {
      get
      {
        return !this.Start.HasValue ? new ulong?() : new ulong?(Timestamps.ToUnixMilliseconds(this.Start.Value));
      }
      set
      {
        this.Start = value.HasValue ? new DateTime?(Timestamps.FromUnixMilliseconds(value.Value)) : new DateTime?();
      }
    }

    [JsonProperty("end", NullValueHandling = NullValueHandling.Ignore)]
    public ulong? EndUnixMilliseconds
    {
      get
      {
        return !this.End.HasValue ? new ulong?() : new ulong?(Timestamps.ToUnixMilliseconds(this.End.Value));
      }
      set
      {
        this.End = value.HasValue ? new DateTime?(Timestamps.FromUnixMilliseconds(value.Value)) : new DateTime?();
      }
    }

    public static DateTime FromUnixMilliseconds(ulong unixTime)
    {
      return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(Convert.ToDouble(unixTime));
    }

    public static ulong ToUnixMilliseconds(DateTime date)
    {
      DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
      return Convert.ToUInt64((date - dateTime).TotalMilliseconds);
    }
  }
}
