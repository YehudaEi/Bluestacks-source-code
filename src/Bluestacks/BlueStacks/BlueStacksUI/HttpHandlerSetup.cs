// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.HttpHandlerSetup
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System.Collections.Generic;

namespace BlueStacks.BlueStacksUI
{
  public static class HttpHandlerSetup
  {
    public static HTTPServer Server { get; set; }

    public static void InitHTTPServer(
      Dictionary<string, HTTPServer.RequestHandler> routes)
    {
      int startingPort = 2871;
      HttpHandlerSetup.Server = HTTPUtils.SetupServer(startingPort, startingPort + 10, routes, string.Empty);
      RegistryManager.Instance.PartnerServerPort = HttpHandlerSetup.Server.Port;
      HttpHandlerSetup.Server.Run();
    }
  }
}
