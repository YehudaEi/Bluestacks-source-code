// Decompiled with JetBrains decompiler
// Type: DiscordRPC.RichPresence
// Assembly: DiscordRPC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B95F11B0-E129-4510-93B3-855D1EDEA68A
// Assembly location: C:\Program Files\BlueStacks\DiscordRPC.dll

using DiscordRPC.Exceptions;
using DiscordRPC.Helper;
using Newtonsoft.Json;
using System;
using System.Text;

namespace DiscordRPC
{
  [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
  [Serializable]
  public class RichPresence
  {
    private string _state;
    private string _details;

    [JsonProperty("state", NullValueHandling = NullValueHandling.Ignore)]
    public string State
    {
      get
      {
        return this._state;
      }
      set
      {
        if (!RichPresence.ValidateString(value, out this._state, 128, Encoding.UTF8))
          throw new StringOutOfRangeException(nameof (State), 128);
      }
    }

    [JsonProperty("details", NullValueHandling = NullValueHandling.Ignore)]
    public string Details
    {
      get
      {
        return this._details;
      }
      set
      {
        if (!RichPresence.ValidateString(value, out this._details, 128, Encoding.UTF8))
          throw new StringOutOfRangeException(nameof (Details), 128);
      }
    }

    [JsonProperty("timestamps", NullValueHandling = NullValueHandling.Ignore)]
    public Timestamps Timestamps { get; set; }

    [JsonProperty("assets", NullValueHandling = NullValueHandling.Ignore)]
    public Assets Assets { get; set; }

    [JsonProperty("party", NullValueHandling = NullValueHandling.Ignore)]
    public Party Party { get; set; }

    [JsonProperty("secrets", NullValueHandling = NullValueHandling.Ignore)]
    public Secrets Secrets { get; set; }

    [JsonProperty("instance", NullValueHandling = NullValueHandling.Ignore)]
    [Obsolete("This was going to be used, but was replaced by JoinSecret instead")]
    private bool Instance { get; set; }

    public RichPresence Clone()
    {
      RichPresence richPresence = new RichPresence();
      richPresence.State = this._state != null ? this._state.Clone() as string : (string) null;
      richPresence.Details = this._details != null ? this._details.Clone() as string : (string) null;
      Secrets secrets;
      if (this.HasSecrets())
        secrets = new Secrets()
        {
          JoinSecret = this.Secrets.JoinSecret != null ? this.Secrets.JoinSecret.Clone() as string : (string) null,
          SpectateSecret = this.Secrets.SpectateSecret != null ? this.Secrets.SpectateSecret.Clone() as string : (string) null
        };
      else
        secrets = (Secrets) null;
      richPresence.Secrets = secrets;
      Timestamps timestamps;
      if (this.HasTimestamps())
        timestamps = new Timestamps()
        {
          Start = this.Timestamps.Start,
          End = this.Timestamps.End
        };
      else
        timestamps = (Timestamps) null;
      richPresence.Timestamps = timestamps;
      Assets assets;
      if (this.HasAssets())
        assets = new Assets()
        {
          LargeImageKey = this.Assets.LargeImageKey != null ? this.Assets.LargeImageKey.Clone() as string : (string) null,
          LargeImageText = this.Assets.LargeImageText != null ? this.Assets.LargeImageText.Clone() as string : (string) null,
          SmallImageKey = this.Assets.SmallImageKey != null ? this.Assets.SmallImageKey.Clone() as string : (string) null,
          SmallImageText = this.Assets.SmallImageText != null ? this.Assets.SmallImageText.Clone() as string : (string) null
        };
      else
        assets = (Assets) null;
      richPresence.Assets = assets;
      Party party;
      if (this.HasParty())
        party = new Party()
        {
          ID = this.Party.ID,
          Size = this.Party.Size,
          Max = this.Party.Max
        };
      else
        party = (Party) null;
      richPresence.Party = party;
      return richPresence;
    }

    internal void Merge(RichPresence presence)
    {
      this._state = presence._state;
      this._details = presence._details;
      this.Party = presence.Party;
      this.Timestamps = presence.Timestamps;
      this.Secrets = presence.Secrets;
      if (presence.HasAssets())
      {
        if (!this.HasAssets())
          this.Assets = presence.Assets;
        else
          this.Assets.Merge(presence.Assets);
      }
      else
        this.Assets = (Assets) null;
    }

    internal void Update(RichPresence presence)
    {
      if (presence == null)
        return;
      this._state = presence._state ?? this._state;
      this._details = presence._details ?? this._details;
      if (presence.Party == null)
        return;
      if (this.Party != null)
      {
        this.Party.ID = presence.Party.ID ?? this.Party.ID;
        this.Party.Size = presence.Party.Size;
        this.Party.Max = presence.Party.Max;
      }
      else
        this.Party = presence.Party;
    }

    public bool HasTimestamps()
    {
      if (this.Timestamps == null)
        return false;
      return this.Timestamps.Start.HasValue || this.Timestamps.End.HasValue;
    }

    public bool HasAssets()
    {
      return this.Assets != null;
    }

    public bool HasParty()
    {
      return this.Party != null && this.Party.ID != null;
    }

    public bool HasSecrets()
    {
      if (this.Secrets == null)
        return false;
      return this.Secrets.JoinSecret != null || this.Secrets.SpectateSecret != null;
    }

    internal static bool ValidateString(
      string str,
      out string result,
      int bytes,
      Encoding encoding)
    {
      result = str;
      if (str == null)
        return true;
      string str1 = str.Trim();
      if (!str1.WithinLength(bytes, encoding))
        return false;
      result = str1.NullEmpty();
      return true;
    }

    public static implicit operator bool(RichPresence presesnce)
    {
      return presesnce != null;
    }
  }
}
