// Decompiled with JetBrains decompiler
// Type: DiscordRPC.User
// Assembly: DiscordRPC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B95F11B0-E129-4510-93B3-855D1EDEA68A
// Assembly location: C:\Program Files\BlueStacks\DiscordRPC.dll

using Newtonsoft.Json;
using System;

namespace DiscordRPC
{
  public class User
  {
    private string _cdn = "cdn.discordapp.com";

    [JsonProperty("id")]
    public ulong ID { get; set; }

    [JsonProperty("username")]
    public string Username { get; set; }

    [JsonProperty("discriminator")]
    public int Discriminator { get; set; }

    [JsonProperty("avatar")]
    public string Avatar { get; set; }

    public string CdnEndpoint
    {
      get
      {
        return this._cdn;
      }
      private set
      {
        this._cdn = value;
      }
    }

    internal void SetConfiguration(Configuration configuration)
    {
      this._cdn = configuration.CdnHost;
    }

    public string GetAvatarURL(User.AvatarFormat format, User.AvatarSize size = User.AvatarSize.x128)
    {
      string str = "/avatars/" + (object) this.ID + "/" + this.Avatar;
      if (string.IsNullOrEmpty(this.Avatar))
      {
        if (format != User.AvatarFormat.PNG)
          throw new BadImageFormatException("The user has no avatar and the requested format " + format.ToString() + " is not supported. (Only supports PNG).");
        str = "/embed/avatars/" + (object) (this.Discriminator % 5);
      }
      return string.Format("https://{0}{1}{2}?size={3}", (object) this.CdnEndpoint, (object) str, (object) this.GetAvatarExtension(format), (object) (int) size);
    }

    public string GetAvatarExtension(User.AvatarFormat format)
    {
      return "." + format.ToString().ToLowerInvariant();
    }

    public override string ToString()
    {
      return this.Username + "#" + (object) this.Discriminator;
    }

    public enum AvatarFormat
    {
      PNG,
      JPEG,
      WebP,
      GIF,
    }

    public enum AvatarSize
    {
      x16 = 16, // 0x00000010
      x32 = 32, // 0x00000020
      x64 = 64, // 0x00000040
      x128 = 128, // 0x00000080
      x256 = 256, // 0x00000100
      x512 = 512, // 0x00000200
      x1024 = 1024, // 0x00000400
      x2048 = 2048, // 0x00000800
    }
  }
}
