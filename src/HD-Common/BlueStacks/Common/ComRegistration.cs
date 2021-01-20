// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.ComRegistration
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System.Diagnostics;
using System.IO;

namespace BlueStacks.Common
{
  public static class ComRegistration
  {
    private static string BIN_PATH = Path.Combine(RegistryStrings.InstallDir, "HD-ComRegistrar.exe");

    public static int Register()
    {
      Logger.Info("Registering COM components");
      return ComRegistration.RunBinary("-reg");
    }

    public static int Unregister()
    {
      Logger.Info("Unregistering COM components");
      return ComRegistration.RunBinary("-unreg");
    }

    private static int RunBinary(string args)
    {
      Process process = new Process();
      process.StartInfo.FileName = ComRegistration.BIN_PATH;
      process.StartInfo.Arguments = args;
      process.StartInfo.UseShellExecute = true;
      process.StartInfo.CreateNoWindow = true;
      process.StartInfo.Verb = "runas";
      process.Start();
      process.WaitForExit();
      return process.ExitCode;
    }
  }
}
