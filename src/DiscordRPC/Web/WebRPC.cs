// Decompiled with JetBrains decompiler
// Type: DiscordRPC.Web.WebRPC
// Assembly: DiscordRPC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B95F11B0-E129-4510-93B3-855D1EDEA68A
// Assembly location: C:\Program Files\BlueStacks\DiscordRPC.dll

using DiscordRPC.Exceptions;
using DiscordRPC.RPC;
using DiscordRPC.RPC.Commands;
using DiscordRPC.RPC.Payload;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net;

namespace DiscordRPC.Web
{
  [Obsolete("Rich Presence over HTTP is no longer supported by Discord. See offical Rich Presence github for more information.")]
  public static class WebRPC
  {
    [Obsolete("Setting Rich Presence over HTTP is no longer supported by Discord. See offical Rich Presence github for more information.")]
    public static RichPresence SetRichPresence(
      RichPresence presence,
      string applicationID,
      int port = 6463)
    {
      try
      {
        RichPresence response;
        return WebRPC.TrySetRichPresence(presence, out response, applicationID, port) ? response : (RichPresence) null;
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    [Obsolete("Setting Rich Presence over HTTP is no longer supported by Discord. See offical Rich Presence github for more information.")]
    public static bool TrySetRichPresence(
      RichPresence presence,
      out RichPresence response,
      string applicationID,
      int port = 6463)
    {
      if (presence != null)
      {
        if (presence.HasSecrets())
          throw new BadPresenceException("Cannot send a presence with secrets as HTTP endpoint does not suppport events.");
        if (presence.HasParty() && presence.Party.Max < presence.Party.Size)
          throw new BadPresenceException("Presence maximum party size cannot be smaller than the current size.");
      }
      for (int port1 = port; port1 < 6472; ++port1)
      {
        using (WebClient webClient = new WebClient())
        {
          try
          {
            WebRequest webRequest = WebRPC.PrepareRequest(presence, applicationID, port1);
            webClient.Headers.Add("content-type", "application/json");
            if (WebRPC.TryParseResponse(webClient.UploadString(webRequest.URL, webRequest.Data), out response))
              return true;
          }
          catch (Exception ex)
          {
          }
        }
      }
      response = (RichPresence) null;
      return false;
    }

    public static bool TryParseResponse(string json, out RichPresence response)
    {
      try
      {
        EventPayload eventPayload = JsonConvert.DeserializeObject<EventPayload>(json);
        if (eventPayload != null)
        {
          response = (RichPresence) eventPayload.GetObject<RichPresenceResponse>();
          return true;
        }
      }
      catch (Exception ex)
      {
      }
      response = (RichPresence) null;
      return false;
    }

    [Obsolete("WebRequests are no longer supported because of the removed HTTP functionality by Discord. See offical Rich Presence github for more information.")]
    public static WebRequest PrepareRequest(
      RichPresence presence,
      string applicationID,
      int port = 6463)
    {
      if (presence != null)
      {
        if (presence.HasSecrets())
          throw new BadPresenceException("Cannot send a presence with secrets as HTTP endpoint does not suppport events.");
        if (presence.HasParty() && presence.Party.Max < presence.Party.Size)
          throw new BadPresenceException("Presence maximum party size cannot be smaller than the current size.");
      }
      int id = Process.GetCurrentProcess().Id;
      string json = JsonConvert.SerializeObject((object) new PresenceCommand()
      {
        PID = id,
        Presence = presence
      }.PreparePayload(DateTime.UtcNow.ToFileTime()));
      return new WebRequest("http://127.0.0.1:" + (object) port + "/rpc?v=" + (object) RpcConnection.VERSION + "&client_id=" + applicationID, json);
    }
  }
}
