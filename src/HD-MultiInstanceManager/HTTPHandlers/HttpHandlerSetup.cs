// Decompiled with JetBrains decompiler
// Type: MultiInstanceManagerMVVM.HTTPHandlers.HttpHandlerSetup
// Assembly: HD-MultiInstanceManager, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 0C4C54E8-53C7-4A46-AE96-CC8E89A6B884
// Assembly location: C:\Program Files\BlueStacks\HD-MultiInstanceManager.exe

using BlueStacks.Common;
using System;
using System.Collections.Generic;

namespace MultiInstanceManagerMVVM.HTTPHandlers
{
  public static class HttpHandlerSetup
  {
    public static HTTPServer Server { get; set; }

    public static void InitHTTPServer(
      Dictionary<string, HTTPServer.RequestHandler> routes)
    {
      try
      {
        int startingPort = 2961;
        int maxPort = startingPort + 10;
        HttpHandlerSetup.Server = HTTPUtils.SetupServer(startingPort, maxPort, routes, string.Empty);
        RegistryManager.Instance.MultiInstanceServerPort = HttpHandlerSetup.Server.Port;
        HttpHandlerSetup.Server.Run();
      }
      catch (Exception ex)
      {
        Logger.Error("Error in setting up HTTPServer" + ex?.ToString());
      }
    }
  }
}
