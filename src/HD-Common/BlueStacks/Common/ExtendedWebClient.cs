// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.ExtendedWebClient
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.Net;

namespace BlueStacks.Common
{
  public class ExtendedWebClient : WebClient
  {
    private int mTimeout;

    public ExtendedWebClient(int timeout)
    {
      this.mTimeout = timeout;
    }

    protected override WebRequest GetWebRequest(Uri address)
    {
      WebRequest webRequest = base.GetWebRequest(address);
      webRequest.Timeout = this.mTimeout;
      return webRequest;
    }
  }
}
