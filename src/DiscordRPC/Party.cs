// Decompiled with JetBrains decompiler
// Type: DiscordRPC.Party
// Assembly: DiscordRPC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B95F11B0-E129-4510-93B3-855D1EDEA68A
// Assembly location: C:\Program Files\BlueStacks\DiscordRPC.dll

using DiscordRPC.Helper;
using Newtonsoft.Json;
using System;

namespace DiscordRPC
{
  [Serializable]
  public class Party
  {
    private string _partyid;

    [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
    public string ID
    {
      get
      {
        return this._partyid;
      }
      set
      {
        this._partyid = value.NullEmpty();
      }
    }

    [JsonIgnore]
    public int Size { get; set; }

    [JsonIgnore]
    public int Max { get; set; }

    [JsonProperty("size", NullValueHandling = NullValueHandling.Ignore)]
    private int[] _size
    {
      get
      {
        int val1 = Math.Max(1, this.Size);
        return new int[2]{ val1, Math.Max(val1, this.Max) };
      }
      set
      {
        if (value.Length != 2)
        {
          this.Size = 0;
          this.Max = 0;
        }
        else
        {
          this.Size = value[0];
          this.Max = value[1];
        }
      }
    }
  }
}
