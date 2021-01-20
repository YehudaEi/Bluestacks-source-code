// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.ServiceInstaller
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System.Diagnostics;
using System.IO;

namespace BlueStacks.Common
{
  public static class ServiceInstaller
  {
    private static string BinaryName = Path.Combine(RegistryStrings.InstallDir, "HD-ServiceInstaller.exe");

    public static int ReinstallService()
    {
      string args = "-reinstall -oem bgp";
      Process process = ProcessUtils.StartExe(ServiceInstaller.BinaryName, args, true);
      process.WaitForExit();
      return process.ExitCode;
    }
  }
}
