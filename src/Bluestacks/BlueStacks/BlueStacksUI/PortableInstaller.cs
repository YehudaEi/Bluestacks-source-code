// Decompiled with JetBrains decompiler
// Type: BlueStacks.BlueStacksUI.PortableInstaller
// Assembly: Bluestacks, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 99F027F6-79F1-4BCA-896C-81F7B404BE8F
// Assembly location: C:\Program Files\BlueStacks\Bluestacks.exe

using BlueStacks.Common;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace BlueStacks.BlueStacksUI
{
  internal class PortableInstaller
  {
    internal static void CheckAndRunPortableInstaller()
    {
      try
      {
        if (!Oem.Instance.IsPortableInstaller)
          return;
        string registryValue = (string) RegistryUtils.GetRegistryValue(Strings.RegistryBaseKeyPath, "Version", (object) "", RegistryKeyKind.HKEY_LOCAL_MACHINE);
        string fullName = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory.Trim('\\')).FullName;
        Logger.InitLogAtPath(Path.Combine(fullName, "Logs\\PortableInstaller.log"), nameof (PortableInstaller), true);
        if (!string.IsNullOrEmpty(registryValue) && !((string) RegistryUtils.GetRegistryValue(Strings.RegistryBaseKeyPath, "InstallDir", (object) "", RegistryKeyKind.HKEY_LOCAL_MACHINE) != Path.Combine(fullName, "BlueStacksPF")) && !Opt.Instance.isForceInstall)
          return;
        PortableInstaller.InstallPortableBlueStacks(AppDomain.CurrentDomain.BaseDirectory);
      }
      catch (Exception ex)
      {
        Logger.Info("Error in CheckAndRunPortableInstaller" + ex?.ToString());
      }
    }

    private static void InstallPortableBlueStacks(string cwd)
    {
      try
      {
        string fullName = Directory.GetParent(cwd.Trim('\\')).FullName;
        string str = Path.Combine(fullName, "Engine");
        string installDir = Path.Combine(fullName, "BlueStacksPF");
        string path1 = Path.Combine(Path.Combine(str, "Android"), "Android.bstk");
        string path2 = Path.Combine(Path.Combine(str, "Manager"), "BstkGlobal.xml");
        if (File.Exists(path1))
          File.Delete(path1);
        if (File.Exists(path2))
          File.Delete(path2);
        CommonInstallUtils.ModifyDirectoryPermissionsForEveryone(fullName);
        if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "install.bat")))
        {
          if (PortableInstaller.RunInstallBat(installDir, str) != 0)
            return;
          PortableInstaller.FixRegistries(fullName, installDir);
          PortableInstaller.DoComRegistration(installDir);
          CommonInstallUtils.InstallVirtualBoxConfig(str, false);
          CommonInstallUtils.InstallVmConfig(installDir, str);
        }
        else
          Logger.Error("Install.bat file missing");
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in InstallPortableBlueStacks " + ex?.ToString());
      }
    }

    private static void FixRegistries(string userDefinedDir, string installDir)
    {
      string path1 = Path.Combine(userDefinedDir, "Engine");
      string index = "Android";
      int CSIDL1 = 46;
      int CSIDL2 = 54;
      RegistryManager.Instance.SetAccessPermissions();
      RegistryManager.Instance.UserDefinedDir = userDefinedDir.Trim('\\');
      RegistryManager.Instance.DataDir = path1.Trim('\\') + "\\";
      RegistryManager.Instance.LogDir = Path.Combine(userDefinedDir, "Logs").Trim('\\') + "\\";
      RegistryManager.Instance.InstallDir = installDir.Trim('\\') + "\\";
      RegistryManager.Instance.EngineDataDir = Path.Combine(userDefinedDir, "Engine");
      RegistryManager.Instance.ClientInstallDir = Path.Combine(userDefinedDir, "Client");
      RegistryManager.Instance.CefDataPath = Path.Combine(userDefinedDir, "CefData");
      RegistryManager.Instance.SetupFolder = Path.Combine(Directory.GetParent(userDefinedDir).ToString(), "BlueStacksSetup");
      RegistryManager.Instance.PartnerExePath = Path.Combine(RegistryManager.Instance.InstallDir, "BlueStacks.exe");
      RegistryManager.Instance.UserGuid = Guid.NewGuid().ToString();
      Utils.UpdateValueInBootParams("GUID", RegistryManager.Instance.UserGuid, index, true, "bgp");
      string str1 = Path.Combine(path1, index) + "\\";
      RegistryManager.Instance.Guest[index].BlockDevice0Name = "sda1";
      RegistryManager.Instance.Guest[index].BlockDevice0Path = str1 + "Root.vdi";
      RegistryManager.Instance.Guest[index].BlockDevice1Name = "sdb1";
      RegistryManager.Instance.Guest[index].BlockDevice1Path = str1 + "Data.vdi";
      RegistryManager.Instance.Guest[index].BlockDevice2Name = "sdc1";
      RegistryManager.Instance.Guest[index].BlockDevice2Path = str1 + "SDCard.vdi";
      string str2 = Path.Combine(path1, "UserData\\SharedFolder\\");
      RegistryManager.Instance.Guest[index].SharedFolder0Name = "BstSharedFolder";
      RegistryManager.Instance.Guest[index].SharedFolder0Path = str2;
      RegistryManager.Instance.Guest[index].SharedFolder0Writable = 1;
      string folderPath1 = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
      RegistryManager.Instance.Guest[index].SharedFolder1Name = "Pictures";
      RegistryManager.Instance.Guest[index].SharedFolder1Path = folderPath1;
      RegistryManager.Instance.Guest[index].SharedFolder1Writable = 1;
      string folderPath2 = CommonInstallUtils.GetFolderPath(CSIDL2);
      RegistryManager.Instance.Guest[index].SharedFolder2Name = "PublicPictures";
      RegistryManager.Instance.Guest[index].SharedFolder2Path = folderPath2;
      RegistryManager.Instance.Guest[index].SharedFolder2Writable = 1;
      string folderPath3 = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
      RegistryManager.Instance.Guest[index].SharedFolder3Name = "Documents";
      RegistryManager.Instance.Guest[index].SharedFolder3Path = folderPath3;
      RegistryManager.Instance.Guest[index].SharedFolder3Writable = 1;
      string folderPath4 = CommonInstallUtils.GetFolderPath(CSIDL1);
      RegistryManager.Instance.Guest[index].SharedFolder4Name = "PublicDocuments";
      RegistryManager.Instance.Guest[index].SharedFolder4Path = folderPath4;
      RegistryManager.Instance.Guest[index].SharedFolder4Writable = 1;
      string str3 = Path.Combine(path1, "UserData\\InputMapper");
      RegistryManager.Instance.Guest[index].SharedFolder5Name = "InputMapper";
      RegistryManager.Instance.Guest[index].SharedFolder5Path = str3;
      RegistryManager.Instance.Guest[index].SharedFolder5Writable = 1;
    }

    private static int RunInstallBat(string installDir, string dataDir)
    {
      Process process = new Process();
      process.StartInfo.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
      process.StartInfo.FileName = "install.bat";
      process.StartInfo.Arguments = "\"" + installDir + "\" \"" + dataDir + "\"";
      process.StartInfo.UseShellExecute = false;
      process.StartInfo.CreateNoWindow = true;
      process.StartInfo.RedirectStandardOutput = true;
      process.StartInfo.RedirectStandardError = true;
      Countdown countDown = new Countdown(2);
      StringBuilder sb = new StringBuilder();
      process.OutputDataReceived += (DataReceivedEventHandler) ((sender, outLine) =>
      {
        if (outLine.Data != null)
        {
          try
          {
            string data = outLine.Data;
            sb.AppendLine(data);
            Logger.Info(data);
          }
          catch (Exception ex)
          {
            Console.WriteLine("Exception in RunInstallBat");
            Console.WriteLine(ex.ToString());
          }
        }
        else
          countDown.Signal();
      });
      process.ErrorDataReceived += (DataReceivedEventHandler) ((sender, outLine) =>
      {
        if (outLine.Data != null)
        {
          try
          {
            string data = outLine.Data;
            sb.AppendLine(data);
            Logger.Info(data);
          }
          catch (Exception ex)
          {
            Console.WriteLine("A crash occured in RunInstallBat");
            Console.WriteLine(ex.ToString());
          }
        }
        else
          countDown.Signal();
      });
      process.Start();
      process.BeginOutputReadLine();
      process.BeginErrorReadLine();
      int milliseconds = 200000;
      process.WaitForExit(milliseconds);
      Logger.Info("Exit Code for InstallBat " + process.ExitCode.ToString());
      countDown.Wait();
      return process.ExitCode;
    }

    private static void DoComRegistration(string installDir)
    {
      string path2 = "HD-ComRegistrar.exe";
      try
      {
        Logger.Info("Starting registration of COM process with: {0}", (object) path2);
        Process process = new Process();
        process.StartInfo.FileName = Path.Combine(installDir, path2);
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;
        process.Start();
        process.WaitForExit();
        Logger.Info("ExitCode: {0}", (object) process.ExitCode);
      }
      catch (Exception ex)
      {
        Logger.Warning("Failed to execute process {0}. Err: {1}", (object) path2, (object) ex.ToString());
      }
    }
  }
}
