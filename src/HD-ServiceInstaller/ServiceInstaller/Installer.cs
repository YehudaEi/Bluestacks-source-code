// Decompiled with JetBrains decompiler
// Type: BlueStacks.ServiceInstaller.Installer
// Assembly: HD-ServiceInstaller, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 15F93427-26B3-4C7E-BAB1-0A00688BC4D4
// Assembly location: C:\Program Files\BlueStacks\HD-ServiceInstaller.exe

using BlueStacks.Common;
using System;

namespace BlueStacks.ServiceInstaller
{
  public class Installer
  {
    internal static ServiceOpt sOpt = new ServiceOpt();

    public static void Main(string[] args)
    {
      try
      {
        Installer.sOpt.Parse(args);
        if (!Installer.sOpt.install && !Installer.sOpt.uninstall && !Installer.sOpt.reinstall)
          Installer.ShowHelpAndExit();
        Logger.InitUserLog();
        string installDirOrExit = Installer.GetInstallDirOrExit();
        string driverName = Installer.GetDriverName();
        if (Installer.sOpt.install)
          Installer.InstallDriver(driverName, installDirOrExit);
        else if (Installer.sOpt.uninstall)
        {
          Installer.UninstallDriver(driverName);
        }
        else
        {
          if (!Installer.sOpt.reinstall)
            return;
          Installer.ReinstallDriver(driverName, installDirOrExit);
        }
      }
      catch (Exception ex)
      {
        string msg = "An error occured while doing the operation\n" + ex.ToString();
        Logger.Error(msg);
        Console.Error.WriteLine(msg);
      }
    }

    private static bool InstallDriver(string driverName, string installDir)
    {
      if (ServiceManager.InstallBstkDrv(installDir, driverName) == 0)
      {
        Console.WriteLine("The driver was installed successfully!");
        return true;
      }
      Console.Error.WriteLine("An error occured while installing the BlueStacks driver");
      return false;
    }

    private static bool UninstallDriver(string driverName)
    {
      int num = ServiceManager.UninstallService(driverName, true) ? 1 : 0;
      if (num != 0)
      {
        Console.WriteLine("The driver was uninstalled successfully!");
        return num != 0;
      }
      Console.Error.WriteLine("An error occured while uninstalling the BlueStacks driver");
      return num != 0;
    }

    private static void ReinstallDriver(string driverName, string installDir)
    {
      if (!Installer.UninstallDriver(driverName))
        return;
      Installer.InstallDriver(driverName, installDir);
    }

    private static void ShowHelpAndExit()
    {
      Console.WriteLine("Usage:");
      Console.WriteLine("\t " + AppDomain.CurrentDomain.FriendlyName + " <function> [OEM]");
      Console.WriteLine("\t -install\t Installs the BlueStacks driver");
      Console.WriteLine("\t -uninstall\t Uninstalls the BlueStacks driver");
      Console.WriteLine("\t -reinstall\t Attempts to reinstall the BlueStacks driver");
      Console.WriteLine("\t -oem\t\t The OEM to install the service (default: {0})", (object) "bgp");
      Environment.Exit(-1);
    }

    private static string GetInstallDirOrExit()
    {
      string registryPath = "Software\\BlueStacks";
      if (!string.Equals(Installer.sOpt.oem, "bgp", StringComparison.InvariantCultureIgnoreCase))
        registryPath = registryPath + "_" + Installer.sOpt.oem;
      string registryValue = (string) RegistryUtils.GetRegistryValue(registryPath, "InstallDir", (object) "", RegistryKeyKind.HKEY_LOCAL_MACHINE);
      if (!string.IsNullOrEmpty(registryValue))
        return registryValue;
      Console.Error.WriteLine("Error! Couldn't find BlueStacks installation path");
      Environment.Exit(-1);
      return registryValue;
    }

    private static string GetDriverName()
    {
      string str = "BlueStacksDrv";
      if (!string.Equals(Installer.sOpt.oem, "bgp", StringComparison.InvariantCultureIgnoreCase))
        str = str + "_" + Installer.sOpt.oem;
      return str;
    }
  }
}
