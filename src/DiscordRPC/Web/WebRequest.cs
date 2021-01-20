// Decompiled with JetBrains decompiler
// Type: DiscordRPC.Web.WebRequest
// Assembly: DiscordRPC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B95F11B0-E129-4510-93B3-855D1EDEA68A
// Assembly location: C:\Program Files\BlueStacks\DiscordRPC.dll

using System;
using System.Collections.Generic;

namespace DiscordRPC.Web
{
  [Obsolete("Web Requests is no longer supported as Discord removed HTTP Rich Presence support. See offical Rich Presence github for more information.")]
  public struct WebRequest
  {
    private string _url;
    private string _json;
    private Dictionary<string, string> _headers;

    public string URL
    {
      get
      {
        return this._url;
      }
    }

    public string Data
    {
      get
      {
        return this._json;
      }
    }

    public Dictionary<string, string> Headers
    {
      get
      {
        return this._headers;
      }
    }

    internal WebRequest(string url, string json)
    {
      this._url = url;
      this._json = json;
      this._headers = new Dictionary<string, string>();
      this._headers.Add("content-type", "application/json");
    }
  }
}
