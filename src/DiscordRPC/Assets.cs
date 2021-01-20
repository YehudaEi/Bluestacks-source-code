// Decompiled with JetBrains decompiler
// Type: DiscordRPC.Assets
// Assembly: DiscordRPC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B95F11B0-E129-4510-93B3-855D1EDEA68A
// Assembly location: C:\Program Files\BlueStacks\DiscordRPC.dll

using DiscordRPC.Exceptions;
using Newtonsoft.Json;
using System;
using System.Text;

namespace DiscordRPC
{
  [Serializable]
  public class Assets
  {
    private string _largeimagekey;
    private string _largeimagetext;
    private string _smallimagekey;
    private string _smallimagetext;
    private ulong? _largeimageID;
    private ulong? _smallimageID;

    [JsonProperty("large_image", NullValueHandling = NullValueHandling.Ignore)]
    public string LargeImageKey
    {
      get
      {
        return this._largeimagekey;
      }
      set
      {
        if (!RichPresence.ValidateString(value, out this._largeimagekey, 32, Encoding.UTF8))
          throw new StringOutOfRangeException(nameof (LargeImageKey), 32);
        this._largeimageID = new ulong?();
      }
    }

    [JsonProperty("large_text", NullValueHandling = NullValueHandling.Ignore)]
    public string LargeImageText
    {
      get
      {
        return this._largeimagetext;
      }
      set
      {
        if (!RichPresence.ValidateString(value, out this._largeimagetext, 128, Encoding.UTF8))
          throw new StringOutOfRangeException(nameof (LargeImageText), 128);
      }
    }

    [JsonProperty("small_image", NullValueHandling = NullValueHandling.Ignore)]
    public string SmallImageKey
    {
      get
      {
        return this._smallimagekey;
      }
      set
      {
        if (!RichPresence.ValidateString(value, out this._smallimagekey, 32, Encoding.UTF8))
          throw new StringOutOfRangeException(nameof (SmallImageKey), 32);
        this._smallimageID = new ulong?();
      }
    }

    [JsonProperty("small_text", NullValueHandling = NullValueHandling.Ignore)]
    public string SmallImageText
    {
      get
      {
        return this._smallimagetext;
      }
      set
      {
        if (!RichPresence.ValidateString(value, out this._smallimagetext, 128, Encoding.UTF8))
          throw new StringOutOfRangeException(nameof (SmallImageText), 128);
      }
    }

    [JsonIgnore]
    public ulong? LargeImageID
    {
      get
      {
        return this._largeimageID;
      }
    }

    [JsonIgnore]
    public ulong? SmallImageID
    {
      get
      {
        return this._smallimageID;
      }
    }

    internal void Merge(Assets other)
    {
      this._smallimagetext = other._smallimagetext;
      this._largeimagetext = other._largeimagetext;
      ulong result1;
      if (ulong.TryParse(other._largeimagekey, out result1))
      {
        this._largeimageID = new ulong?(result1);
      }
      else
      {
        this._largeimagekey = other._largeimagekey;
        this._largeimageID = new ulong?();
      }
      ulong result2;
      if (ulong.TryParse(other._smallimagekey, out result2))
      {
        this._smallimageID = new ulong?(result2);
      }
      else
      {
        this._smallimagekey = other._smallimagekey;
        this._smallimageID = new ulong?();
      }
    }
  }
}
