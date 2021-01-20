// Decompiled with JetBrains decompiler
// Type: DiscordRPC.Message.PresenceMessage
// Assembly: DiscordRPC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B95F11B0-E129-4510-93B3-855D1EDEA68A
// Assembly location: C:\Program Files\BlueStacks\DiscordRPC.dll

namespace DiscordRPC.Message
{
  public class PresenceMessage : IMessage
  {
    public override MessageType Type
    {
      get
      {
        return MessageType.PresenceUpdate;
      }
    }

    internal PresenceMessage()
      : this((RichPresenceResponse) null)
    {
    }

    internal PresenceMessage(RichPresenceResponse rpr)
    {
      if (rpr == null)
      {
        this.Presence = (RichPresence) null;
        this.Name = "No Rich Presence";
        this.ApplicationID = "";
      }
      else
      {
        this.Presence = (RichPresence) rpr;
        this.Name = rpr.Name;
        this.ApplicationID = rpr.ClientID;
      }
    }

    public RichPresence Presence { get; internal set; }

    public string Name { get; internal set; }

    public string ApplicationID { get; internal set; }
  }
}
