// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.HttpHandlerSetup
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using BlueStacks.Common;
using System.Collections.Generic;

namespace BlueStacks.Player
{
  public class HttpHandlerSetup
  {
    public static HTTPServer Server;

    public static void InitHTTPServer(
      Dictionary<string, HTTPServer.RequestHandler> routes,
      string vmName)
    {
      int frontendServerPort = RegistryManager.Instance.Guest[vmName].FrontendServerPort;
      HttpHandlerSetup.Server = HTTPUtils.SetupServer(frontendServerPort, frontendServerPort + 40, routes, string.Empty);
      HttpHandlerSetup.SetFrontendPortInBootParams(HttpHandlerSetup.Server.Port, vmName);
      RegistryManager.Instance.Guest[vmName].FrontendServerPort = HttpHandlerSetup.Server.Port;
      HttpHandlerSetup.Server.Run();
    }

    private static void SetFrontendPortInBootParams(int frontendPort, string vmName)
    {
      string bootParameters = RegistryManager.Instance.Guest[vmName].BootParameters;
      string[] strArray = bootParameters.Split(' ');
      string str1 = "";
      string str2 = string.Format("10.0.2.2:{0}", (object) frontendPort);
      if (bootParameters.IndexOf("WINDOWSFRONTEND") == -1)
      {
        str1 = bootParameters + " WINDOWSFRONTEND=" + str2;
      }
      else
      {
        foreach (string str3 in strArray)
        {
          if (str3.IndexOf("WINDOWSFRONTEND") != -1)
          {
            if (!string.IsNullOrEmpty(str1))
              str1 += " ";
            str1 = str1 + "WINDOWSFRONTEND=" + str2;
          }
          else
          {
            if (!string.IsNullOrEmpty(str1))
              str1 += " ";
            str1 += str3;
          }
        }
      }
      RegistryManager.Instance.Guest[vmName].BootParameters = str1;
    }
  }
}
