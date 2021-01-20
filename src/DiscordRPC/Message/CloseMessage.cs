// Decompiled with JetBrains decompiler
// Type: DiscordRPC.Message.CloseMessage
// Assembly: DiscordRPC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B95F11B0-E129-4510-93B3-855D1EDEA68A
// Assembly location: C:\Program Files\BlueStacks\DiscordRPC.dll

namespace DiscordRPC.Message
{
  public class CloseMessage : IMessage
  {
    public override MessageType Type
    {
      get
      {
        return MessageType.Close;
      }
    }

    public string Reason { get; internal set; }

    public int Code { get; internal set; }

    internal CloseMessage()
    {
    }

    internal CloseMessage(string reason)
    {
      this.Reason = reason;
    }
  }
}
