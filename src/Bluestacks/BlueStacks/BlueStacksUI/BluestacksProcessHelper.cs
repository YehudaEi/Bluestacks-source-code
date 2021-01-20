// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.BluestacksProcessHelper
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;

namespace BlueStacks.BlueStacksUI
{
  internal class BluestacksProcessHelper
  {
    internal static int StartFrontend(string vmName)
    {
      try
      {
        string installDir = RegistryStrings.InstallDir;
        string str1 = Path.Combine(installDir, "HD-Player.exe");
        string str2 = FeatureManager.Instance.IsUseWpfTextbox ? " -w" : "";
        string str3 = " -h";
        if (RegistryManager.Instance.DevEnv == 1)
          str3 = "";
        Process process = new Process();
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.FileName = str1;
        process.StartInfo.Arguments = vmName + str3 + str2;
        process.StartInfo.WorkingDirectory = installDir;
        Logger.Info("Starting Frontend for vm: {0} with args: {1}", (object) vmName, (object) process.StartInfo.Arguments);
        process.Start();
        process.WaitForExit();
        return process.ExitCode;
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in starting frontend. Err : " + ex.ToString());
      }
      return 0;
    }

    public static void RunUpdateInstaller(string filePath, string arg, bool isAdmin = false)
    {
      Logger.Info("RunUpdateInstaller start");
      try
      {
        using (Process process = new Process())
        {
          process.StartInfo.FileName = filePath;
          process.StartInfo.Arguments = arg;
          if (isAdmin)
            process.StartInfo.Verb = "runas";
          process.Start();
        }
      }
      catch (Exception ex)
      {
        Logger.Warning("Exception in running update installer " + ex.ToString());
      }
    }

    public static Process StartBluestacks(string vmName)
    {
      string str = Path.Combine(RegistryStrings.InstallDir, "HD-RunApp.exe");
      Process process = new Process();
      process.StartInfo.UseShellExecute = false;
      process.StartInfo.CreateNoWindow = true;
      process.StartInfo.FileName = str;
      process.StartInfo.Arguments = "-vmname:" + vmName + " -h";
      Logger.Info("Sending RunApp for vm calling {0}", (object) vmName);
      Logger.Info("Utils: Starting hidden Frontend");
      process.Start();
      return process;
    }

    public static int RunApkInstaller(string apkPath, bool isSilentInstall, string vmName)
    {
      Logger.Info("Installing apk :{0} vmname: {1} ", (object) apkPath, (object) vmName);
      if (vmName == null)
        vmName = "Android";
      int num = -1;
      try
      {
        string installDir = RegistryStrings.InstallDir;
        ProcessStartInfo startInfo = new ProcessStartInfo()
        {
          WorkingDirectory = installDir
        };
        if (string.Equals(Path.GetExtension(apkPath), ".xapk", StringComparison.InvariantCultureIgnoreCase))
        {
          startInfo.FileName = Path.Combine(installDir, "HD-XapkHandler.exe");
          if (isSilentInstall)
            startInfo.Arguments = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "-xapk \"{0}\" -s -vmname {1}", (object) apkPath, (object) vmName);
          else
            startInfo.Arguments = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "-xapk \"{0}\" -vmname {1}", (object) apkPath, (object) vmName);
        }
        else
        {
          startInfo.FileName = Path.Combine(installDir, "HD-ApkHandler.exe");
          if (isSilentInstall)
            startInfo.Arguments = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "-apk \"{0}\" -s -vmname {1}", (object) apkPath, (object) vmName);
          else
            startInfo.Arguments = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "-apk \"{0}\" -vmname {1}", (object) apkPath, (object) vmName);
        }
        startInfo.UseShellExecute = false;
        startInfo.CreateNoWindow = true;
        Logger.Info("Console: installer path {0}", (object) startInfo.FileName);
        Process process = Process.Start(startInfo);
        process.WaitForExit();
        num = process.ExitCode;
        Logger.Info("Console: apk installer exit code: {0}", (object) process.ExitCode);
      }
      catch (Exception ex)
      {
        Logger.Info("Error Installing Apk : " + ex.ToString());
      }
      return num;
    }

    internal static bool TakeLock(string lockBane)
    {
      return ProcessUtils.CheckAlreadyRunningAndTakeLock(lockBane, out Mutex _);
    }
  }
}
