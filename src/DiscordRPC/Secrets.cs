// Decompiled with JetBrains decompiler
// Type: DiscordRPC.Secrets
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
  public class Secrets
  {
    private string _matchSecret;
    private string _joinSecret;
    private string _spectateSecret;

    [Obsolete("This feature has been deprecated my Mason in issue #152 on the offical library. Was originally used as a Notify Me feature, it has been replaced with Join / Spectate.")]
    [JsonProperty("match", NullValueHandling = NullValueHandling.Ignore)]
    public string MatchSecret
    {
      get
      {
        return this._matchSecret;
      }
      set
      {
        if (!RichPresence.ValidateString(value, out this._matchSecret, 128, Encoding.UTF8))
          throw new StringOutOfRangeException(nameof (MatchSecret), 128);
      }
    }

    [JsonProperty("join", NullValueHandling = NullValueHandling.Ignore)]
    public string JoinSecret
    {
      get
      {
        return this._joinSecret;
      }
      set
      {
        if (!RichPresence.ValidateString(value, out this._joinSecret, 128, Encoding.UTF8))
          throw new StringOutOfRangeException(nameof (JoinSecret), 128);
      }
    }

    [JsonProperty("spectate", NullValueHandling = NullValueHandling.Ignore)]
    public string SpectateSecret
    {
      get
      {
        return this._spectateSecret;
      }
      set
      {
        if (!RichPresence.ValidateString(value, out this._spectateSecret, 128, Encoding.UTF8))
          throw new StringOutOfRangeException(nameof (SpectateSecret), 128);
      }
    }

    public static Encoding Encoding
    {
      get
      {
        return Encoding.UTF8;
      }
    }

    public static int SecretLength
    {
      get
      {
        return 128;
      }
    }

    public static string CreateSecret(Random random)
    {
      byte[] numArray = new byte[Secrets.SecretLength];
      random.NextBytes(numArray);
      return Secrets.Encoding.GetString(numArray);
    }

    public static string CreateFriendlySecret(Random random)
    {
      string str1 = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
      string str2 = "";
      for (int index = 0; index < Secrets.SecretLength; ++index)
        str2 += str1[random.Next(str1.Length)].ToString();
      return str2;
    }
  }
}
