// Decompiled with JetBrains decompiler
// Type: BlueStacks.ConfigHttpProxy.ConfigHttpProxy
// Assembly: HD-ConfigHttpProxy, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 12C81935-F2EE-4FEC-B10B-2FD5D0D8A1FF
// Assembly location: C:\Program Files\BlueStacks\HD-ConfigHttpProxy.exe

using BlueStacks.Common;
using System;
using System.Diagnostics;
using System.IO;

namespace BlueStacks.ConfigHttpProxy
{
  internal class ConfigHttpProxy
  {
    internal static string FILE_NAME = "settings_global.xml";
    internal static string ORIG_FILE_PATH = "/data/system/users/0/" + BlueStacks.ConfigHttpProxy.ConfigHttpProxy.FILE_NAME;
    internal static string SDCARD_FILE_PATH = "/sdcard/" + BlueStacks.ConfigHttpProxy.ConfigHttpProxy.FILE_NAME;
    internal static string TEMP_XML_FILE_PATH = Path.Combine(Path.GetTempPath(), BlueStacks.ConfigHttpProxy.ConfigHttpProxy.FILE_NAME);
    internal static string sVMName = "Android";

    public static void Main(string[] args)
    {
      Logger.InitUserLog();
      if (args.Length < 1)
        BlueStacks.ConfigHttpProxy.ConfigHttpProxy.Usage();
      switch (args[0])
      {
        case "set":
          if (args.Length == 3)
          {
            BlueStacks.ConfigHttpProxy.ConfigHttpProxy.SetProxy(args[1], args[2]);
            return;
          }
          break;
        case "reset":
          if (args.Length == 1)
          {
            BlueStacks.ConfigHttpProxy.ConfigHttpProxy.ResetProxy();
            return;
          }
          break;
      }
      BlueStacks.ConfigHttpProxy.ConfigHttpProxy.Usage();
    }

    private static void Usage()
    {
      string moduleName = Process.GetCurrentProcess().MainModule.ModuleName;
      Console.Error.WriteLine("Usage: {0} set <host> <port>", (object) moduleName);
      Console.Error.WriteLine("       {0} reset", (object) moduleName);
      Environment.Exit(1);
    }

    private static void SetProxy(string host, string port)
    {
      Console.WriteLine("Setting proxy configuration: host = {0}, port = {1}", (object) host, (object) port);
      BlueStacks.ConfigHttpProxy.ConfigHttpProxy.CheckIfGuestReady();
      AdbCommandRunner runner = new AdbCommandRunner("Android");
      BlueStacks.ConfigHttpProxy.ConfigHttpProxy.ConnectToAdb(runner);
      BlueStacks.ConfigHttpProxy.ConfigHttpProxy.CopyXMLToSDCard(runner);
      runner.Pull(BlueStacks.ConfigHttpProxy.ConfigHttpProxy.SDCARD_FILE_PATH, BlueStacks.ConfigHttpProxy.ConfigHttpProxy.TEMP_XML_FILE_PATH);
      XMLManipulation.AppendToXMLFile("global_http_proxy_host", "91", host);
      XMLManipulation.AppendToXMLFile("global_http_proxy_port", "92", port);
      XMLManipulation.AppendToXMLFile("global_http_proxy_exclusion_list", "93", "127.0.0.1,10.0.2.2,10.0.2.15");
      runner.Push(BlueStacks.ConfigHttpProxy.ConfigHttpProxy.TEMP_XML_FILE_PATH, BlueStacks.ConfigHttpProxy.ConfigHttpProxy.SDCARD_FILE_PATH);
      BlueStacks.ConfigHttpProxy.ConfigHttpProxy.MoveXMLToOrigLocationAndFixPermissions(runner);
      Console.WriteLine("Process completed, please wait while BlueStacks restarts.");
      runner.Dispose();
      BlueStacks.ConfigHttpProxy.ConfigHttpProxy.RestartBlueStacks();
      Console.WriteLine("New HTTP proxy server was successfully configured.");
      Environment.Exit(0);
    }

    private static void ResetProxy()
    {
      Console.WriteLine("Resetting proxy configuration.");
      BlueStacks.ConfigHttpProxy.ConfigHttpProxy.CheckIfGuestReady();
      AdbCommandRunner runner = new AdbCommandRunner("Android");
      BlueStacks.ConfigHttpProxy.ConfigHttpProxy.ConnectToAdb(runner);
      BlueStacks.ConfigHttpProxy.ConfigHttpProxy.CopyXMLToSDCard(runner);
      runner.Pull(BlueStacks.ConfigHttpProxy.ConfigHttpProxy.SDCARD_FILE_PATH, BlueStacks.ConfigHttpProxy.ConfigHttpProxy.TEMP_XML_FILE_PATH);
      XMLManipulation.DeleteFromXMLFile("global_http_proxy_host");
      XMLManipulation.DeleteFromXMLFile("global_http_proxy_port");
      XMLManipulation.DeleteFromXMLFile("global_http_proxy_exclusion_list");
      runner.Push(BlueStacks.ConfigHttpProxy.ConfigHttpProxy.TEMP_XML_FILE_PATH, BlueStacks.ConfigHttpProxy.ConfigHttpProxy.SDCARD_FILE_PATH);
      BlueStacks.ConfigHttpProxy.ConfigHttpProxy.MoveXMLToOrigLocationAndFixPermissions(runner);
      Console.WriteLine("Please wait while BlueStacks restarts.");
      runner.Dispose();
      BlueStacks.ConfigHttpProxy.ConfigHttpProxy.RestartBlueStacks();
      Console.WriteLine("HTTP proxy server was successfully removed.");
      Environment.Exit(0);
    }

    private static void RestartBlueStacks()
    {
      if (Utils.RestartBlueStacks())
        return;
      Console.Error.WriteLine("Could not restart BlueStacks. Could you please manually restart BlueStacks and see if the changes have been configured");
      Environment.Exit(1);
    }

    private static void MoveXMLToOrigLocationAndFixPermissions(AdbCommandRunner runner)
    {
      if (runner.RunShellScriptPrivileged(new string[3]
      {
        string.Format("mv {0} {1}", (object) BlueStacks.ConfigHttpProxy.ConfigHttpProxy.SDCARD_FILE_PATH, (object) BlueStacks.ConfigHttpProxy.ConfigHttpProxy.ORIG_FILE_PATH),
        string.Format("chown system:system {0}", (object) BlueStacks.ConfigHttpProxy.ConfigHttpProxy.ORIG_FILE_PATH),
        string.Format("chmod 600 {0}", (object) BlueStacks.ConfigHttpProxy.ConfigHttpProxy.ORIG_FILE_PATH)
      }))
        return;
      Console.Error.WriteLine("Cannot configure proxy parameters.");
      Environment.Exit(1);
    }

    private static void CopyXMLToSDCard(AdbCommandRunner runner)
    {
      runner.RunShellScriptPrivileged(new string[1]
      {
        string.Format("cp {0} {1}", (object) BlueStacks.ConfigHttpProxy.ConfigHttpProxy.ORIG_FILE_PATH, (object) BlueStacks.ConfigHttpProxy.ConfigHttpProxy.SDCARD_FILE_PATH)
      });
    }

    private static void ConnectToAdb(AdbCommandRunner runner)
    {
      if (runner.Connect(BlueStacks.ConfigHttpProxy.ConfigHttpProxy.sVMName))
        return;
      Console.Error.WriteLine("Cannot connect to guest.  Please make sure BlueStacks is running.");
      Environment.Exit(1);
    }

    private static void CheckIfGuestReady()
    {
      if (Utils.CheckIfGuestReady(BlueStacks.ConfigHttpProxy.ConfigHttpProxy.sVMName, 3))
        return;
      Console.Error.WriteLine("Cannot connect to guest.  Please make sure BlueStacks is running.");
      Environment.Exit(1);
    }
  }
}
