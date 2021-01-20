// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.ForceDedicatedGPU
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System;
using System.IO;

namespace BlueStacks.Common
{
  public static class ForceDedicatedGPU
  {
    private const string ENABLE_ARG = "1";
    private const string DISABLE_ARG = "0";

    public static bool ToggleDedicatedGPU(bool enable, string binPath = null)
    {
      try
      {
        if (binPath == null)
          binPath = Path.Combine(RegistryStrings.InstallDir, "HD-ForceGPU.exe");
        string args = enable ? "1" : "0";
        return RunCommand.RunCmd(binPath, args, true, true, false, 0).ExitCode == 0;
      }
      catch (Exception ex)
      {
        Logger.Error("An error occured while running {0}, Ex: {1}", (object) binPath, (object) ex);
      }
      return false;
    }
  }
}
