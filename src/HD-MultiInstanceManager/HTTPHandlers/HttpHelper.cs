// Decompiled with JetBrains decompiler
// Type: MultiInstanceManagerMVVM.HTTPHandlers.HttpHelper
// Assembly: HD-MultiInstanceManager, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 0C4C54E8-53C7-4A46-AE96-CC8E89A6B884
// Assembly location: C:\Program Files\BlueStacks\HD-MultiInstanceManager.exe

using BlueStacks.Common;
using System;
using System.Collections.Generic;
using System.Threading;

namespace MultiInstanceManagerMVVM.HTTPHandlers
{
  public static class HttpHelper
  {
    internal static void InitHttpServerAsync()
    {
      new Thread(new ThreadStart(HttpHelper.SetupHTTPServer))
      {
        IsBackground = true
      }.Start();
    }

    internal static void SetupHTTPServer()
    {
      try
      {
        HttpHandlerSetup.InitHTTPServer(new Dictionary<string, HTTPServer.RequestHandler>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
        {
          {
            "/ping",
            new HTTPServer.RequestHandler(HttpHandler.PingHandler)
          },
          {
            "/toggleMIMFarmMode",
            new HTTPServer.RequestHandler(HttpHandler.ToggleMIMFarmMode)
          }
        });
      }
      catch (Exception ex)
      {
        Logger.Error("Error in setting up HTTPServer" + ex?.ToString());
      }
    }
  }
}
