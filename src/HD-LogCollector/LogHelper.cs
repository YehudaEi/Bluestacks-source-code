// Decompiled with JetBrains decompiler
// Type: BlueStacks.LogCollector.LogHelper
// Assembly: HD-LogCollector, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: FD44426D-0F78-46B3-8118-1E64B979C51B
// Assembly location: C:\Program Files\BlueStacks\HD-LogCollector.exe

using BlueStacks.Common;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Management;
using System.Security.AccessControl;
using System.Threading;

namespace BlueStacks.LogCollector
{
  internal static class LogHelper
  {
    internal static void RunCmdAsync(string prog, string args)
    {
      try
      {
        LogHelper.RunCmdAsyncInternal(prog, args);
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
      }
    }

    internal static void RunCmdAsyncInternal(string prog, string args)
    {
      Console.WriteLine("Running Command Async  :::::::  {0}", (object) DateTime.Now);
      Console.WriteLine("    prog: " + prog);
      Console.WriteLine("    args: " + args);
      using (Process process = new Process())
      {
        process.StartInfo.FileName = prog;
        process.StartInfo.Arguments = args;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;
        process.Start();
      }
    }

    internal static void RunCmd(string prog, string args, string outPath)
    {
      try
      {
        LogHelper.RunCmdInternal(prog, args, outPath);
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
      }
    }

    internal static void RunCmdInternal(string prog, string args, string outPath)
    {
      Console.WriteLine("Running Command  :::::::  {0}", (object) DateTime.Now);
      Console.WriteLine("    prog: " + prog);
      Console.WriteLine("    args: " + args);
      Console.WriteLine("    out:  " + outPath);
      using (Process process = new Process())
      {
        process.StartInfo.FileName = prog;
        process.StartInfo.Arguments = args;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;
        TextWriter writer = (TextWriter) null;
        StreamWriter streamWriter = (StreamWriter) null;
        if (!string.IsNullOrEmpty(outPath))
        {
          streamWriter = new StreamWriter(outPath);
          writer = TextWriter.Synchronized((TextWriter) streamWriter);
          Countdown countDown = new Countdown(2);
          process.StartInfo.RedirectStandardInput = true;
          process.StartInfo.RedirectStandardOutput = true;
          process.StartInfo.RedirectStandardError = true;
          process.OutputDataReceived += (DataReceivedEventHandler) ((obj, line) =>
          {
            if (line.Data != null)
              writer.WriteLine(line.Data);
            else
              countDown.Signal();
          });
          process.ErrorDataReceived += (DataReceivedEventHandler) ((obj, line) =>
          {
            if (line.Data != null)
              writer.WriteLine(line.Data);
            else
              countDown.Signal();
          });
        }
        process.Start();
        if (outPath != null)
        {
          process.BeginOutputReadLine();
          process.BeginErrorReadLine();
        }
        process.WaitForExit();
        if (outPath != null)
        {
          streamWriter.Close();
          writer.Close();
        }
        Console.WriteLine("command run successfully");
      }
    }

    internal static void RunCmdWithList(string prog, string[] argList, string outPath)
    {
      try
      {
        List<string> stringList = new List<string>();
        foreach (string str in argList)
          stringList.Add(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "\"{0}\"", (object) str));
        LogHelper.RunCmd(prog, string.Join(" ", stringList.ToArray()), outPath);
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
      }
    }

    internal static void RunCmdWithList(string prog, string[] argList)
    {
      LogHelper.RunCmdWithList(prog, argList, (string) null);
    }

    internal static void DumpDirectoryListing(string dir, string outPath, string outputDirectory)
    {
      if (string.IsNullOrEmpty(dir))
        return;
      LogHelper.RunCmd("cmd", string.Format((IFormatProvider) CultureInfo.CurrentCulture, "/c dir \"{0}\" /s", (object) dir), Path.Combine(outputDirectory, outPath));
    }

    internal static void DumpInstalledPrograms(string outPath)
    {
      string regKeyPath1 = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall";
      LogHelper.WriteInstalledPrograms(outPath, regKeyPath1);
      if (!SystemUtils.IsOs64Bit())
        return;
      string regKeyPath2 = "SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall";
      LogHelper.WriteInstalledPrograms(outPath, regKeyPath2);
    }

    private static void WriteInstalledPrograms(string outPath, string regKeyPath)
    {
      RegistryKey registryKey1 = Registry.LocalMachine.OpenSubKey(regKeyPath);
      string[] subKeyNames = registryKey1.GetSubKeyNames();
      StreamWriter streamWriter = new StreamWriter(outPath, true);
      foreach (string name in subKeyNames)
      {
        RegistryKey registryKey2 = registryKey1.OpenSubKey(name);
        streamWriter.WriteLine("Key: " + name);
        object obj1 = registryKey2.GetValue("DisplayName");
        object obj2 = registryKey2.GetValue("DisplayVersion");
        if (obj1 != null)
        {
          streamWriter.WriteLine("Application Name: " + (string) obj1);
          streamWriter.WriteLine("Application Version: " + (string) obj2);
        }
        streamWriter.WriteLine("");
      }
      streamWriter.Close();
    }

    internal static void DumpEventLogs(string name, string outPath)
    {
      Console.WriteLine("In DumpEventsLogs name = " + name);
      StreamWriter streamWriter = new StreamWriter(outPath);
      using (EventLog eventLog = new EventLog(name))
      {
        int count = eventLog.Entries.Count;
        int num = 0;
        if (count > 2000)
          num = count - 2000;
        for (int index = num; index < count; ++index)
        {
          streamWriter.WriteLine("Event[{0}]:", (object) index);
          streamWriter.WriteLine(" Log Name: {0}", (object) name);
          streamWriter.WriteLine(" Source: {0}", (object) eventLog.Entries[index].Source);
          streamWriter.WriteLine(" Date: {0}", (object) eventLog.Entries[index].TimeGenerated);
          streamWriter.WriteLine(" Event ID: {0}", (object) eventLog.Entries[index].InstanceId);
          streamWriter.WriteLine(" User: {0}", (object) eventLog.Entries[index].UserName);
          streamWriter.WriteLine(" Description:");
          streamWriter.WriteLine("{0}", (object) eventLog.Entries[index].Message);
          streamWriter.WriteLine("");
        }
        streamWriter.Close();
      }
    }

    internal static void DumpHyperVLogs(string outPath, string tempPath)
    {
      Console.WriteLine("In DumpHyperVLogs");
      StreamWriter streamWriter = new StreamWriter(outPath);
      string prog = "wevtutil.exe";
      string args1 = "qe \"Microsoft-Windows-Hyper-V-Compute-Admin\" /f:text";
      LogHelper.RunCmd(prog, args1, tempPath);
      streamWriter.WriteLine("Dumping Microsoft-Windows-Hyper-V-Compute-Admin logs\n");
      streamWriter.Write(File.ReadAllText(tempPath));
      string args2 = "qe \"Microsoft-Windows-Hyper-V-Worker-Admin\" /f:text";
      LogHelper.RunCmd(prog, args2, tempPath);
      streamWriter.WriteLine("Dumping Microsoft-Windows-Hyper-V-Worker-Admin logs \n");
      streamWriter.Write(File.ReadAllText(tempPath));
      string args3 = "qe \"Microsoft-Windows-Host-Network-Service-Admin\" /f:text";
      LogHelper.RunCmd(prog, args3, tempPath);
      streamWriter.WriteLine("Dumping Microsoft-Windows-Host-Network-Service-Admin \n");
      streamWriter.Write(File.ReadAllText(tempPath));
      streamWriter.Close();
      File.Delete(tempPath);
    }

    internal static void DumpHotfixInfo(string outPath)
    {
      StreamWriter writer = new StreamWriter(outPath);
      using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_QuickFixEngineering"))
      {
        ManagementObjectCollection objectCollection = managementObjectSearcher.Get();
        writer.WriteLine("{0} Hotfixes Applied", (object) objectCollection.Count);
        foreach (ManagementObject managementObject in objectCollection)
          LogHelper.DumpHotfixDetails(managementObject, writer);
        writer.Close();
      }
    }

    private static void DumpHotfixDetails(ManagementObject obj, StreamWriter writer)
    {
      writer.WriteLine("{0} - {1}", (object) LogHelper.GetP(obj, "HotFixID"), (object) LogHelper.GetP(obj, "Description"));
    }

    internal static void DumpProcessList(string outPath)
    {
      Console.WriteLine("In DumpProcessList");
      StreamWriter streamWriter1 = new StreamWriter(outPath);
      using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_Process"))
      {
        ManagementObjectCollection objectCollection = managementObjectSearcher.Get();
        try
        {
          Dictionary<Process, PerformanceCounter> dictionary = new Dictionary<Process, PerformanceCounter>();
          using (PerformanceCounter performanceCounter1 = new PerformanceCounter("Processor", "% Processor Time", "_Total"))
          {
            double num1 = (double) performanceCounter1.NextValue();
            foreach (Process process in Process.GetProcesses())
            {
              try
              {
                using (PerformanceCounter performanceCounter2 = new PerformanceCounter("Process", "% Processor Time", process.ProcessName))
                {
                  double num2 = (double) performanceCounter2.NextValue();
                  dictionary.Add(process, performanceCounter2);
                }
              }
              catch (Exception ex)
              {
                Logger.Error("Error dumping process info" + process.ProcessName);
              }
            }
            Thread.Sleep(1000);
            StreamWriter streamWriter2 = streamWriter1;
            float num3 = performanceCounter1.NextValue();
            string str1 = "Total CPU Usage: " + num3.ToString() + "%";
            streamWriter2.WriteLine(str1);
            streamWriter1.WriteLine("");
            foreach (KeyValuePair<Process, PerformanceCounter> keyValuePair in dictionary)
            {
              streamWriter1.WriteLine("Process:  " + keyValuePair.Key.ProcessName);
              StreamWriter streamWriter3 = streamWriter1;
              num3 = keyValuePair.Value.NextValue();
              string str2 = "CPU Usage:    " + num3.ToString() + "%";
              streamWriter3.WriteLine(str2);
              streamWriter1.WriteLine("");
            }
          }
        }
        catch (Exception ex)
        {
        }
        foreach (ManagementObject o in objectCollection)
        {
          streamWriter1.WriteLine("");
          StreamWriter w = streamWriter1;
          LogHelper.DumpProcess(o, w);
        }
        streamWriter1.Close();
      }
    }

    internal static void DumpProcess(ManagementObject o, StreamWriter w)
    {
      w.WriteLine("Name:        " + LogHelper.GetP(o, "Name"));
      w.WriteLine("Path:        " + LogHelper.GetP(o, "ExecutablePath"));
      w.WriteLine("Command:     " + LogHelper.GetP(o, "CommandLine"));
      w.WriteLine("PID:         " + LogHelper.GetP(o, "ProcessId"));
      w.WriteLine("User:        " + LogHelper.GetProcessOwner(o));
      w.WriteLine("Session:     " + LogHelper.GetP(o, "SessionId"));
      w.WriteLine("Threads:     " + LogHelper.GetP(o, "ThreadCount"));
      w.WriteLine("Handles:     " + LogHelper.GetP(o, "HandleCount"));
      w.WriteLine("Memory (KB): " + (long.Parse(LogHelper.GetP(o, "WorkingSetSize"), (IFormatProvider) CultureInfo.InvariantCulture) / 1024L).ToString());
      w.WriteLine("Peak (KB):   " + LogHelper.GetP(o, "PeakWorkingSetSize"));
      w.WriteLine("User (ms):   " + (long.Parse(LogHelper.GetP(o, "UserModeTime"), (IFormatProvider) CultureInfo.InvariantCulture) / 10000L).ToString());
      w.WriteLine("Kernel (ms): " + (long.Parse(LogHelper.GetP(o, "KernelModeTime"), (IFormatProvider) CultureInfo.InvariantCulture) / 10000L).ToString());
    }

    internal static void DumpStartupPrograms(string outPath)
    {
      using (ManagementClass managementClass = new ManagementClass("Win32_StartupCommand"))
      {
        ManagementObjectCollection instances = managementClass.GetInstances();
        StreamWriter streamWriter = new StreamWriter(outPath);
        foreach (ManagementObject managementObject in instances)
        {
          streamWriter.WriteLine("Application Name: " + managementObject["Name"].ToString());
          streamWriter.WriteLine("Application Location: " + managementObject["Location"].ToString());
          streamWriter.WriteLine("Application Command: " + managementObject["Command"].ToString());
          streamWriter.WriteLine("User: " + managementObject["User"].ToString());
          streamWriter.WriteLine("");
        }
        streamWriter.Close();
      }
    }

    private static string GetProcessOwner(ManagementObject obj)
    {
      try
      {
        string[] strArray = new string[2]{ "", "" };
        if (Convert.ToInt32(obj.InvokeMethod("GetOwner", (object[]) strArray), (IFormatProvider) CultureInfo.InvariantCulture) == 0)
          return strArray[1] + "\\" + strArray[0];
      }
      catch (Exception ex)
      {
        Logger.Error(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Error Occured, Err: {0}", (object) ex.ToString()));
      }
      return "";
    }

    internal static string GetP(ManagementObject obj, string name)
    {
      object propertyValue = obj.GetPropertyValue(name);
      return propertyValue == null ? "" : propertyValue.ToString();
    }

    internal static void CopyRecursive(string srcPath, string dstPath)
    {
      if (!Directory.Exists(dstPath))
        Directory.CreateDirectory(dstPath);
      DirectoryInfo directoryInfo = new DirectoryInfo(srcPath);
      foreach (DirectoryInfo directory in directoryInfo.GetDirectories())
      {
        Console.WriteLine(directory.FullName + " {0}", (object) DateTime.Now);
        LogHelper.CopyRecursive(Path.Combine(srcPath, directory.Name), Path.Combine(dstPath, directory.Name));
      }
      foreach (FileInfo file in directoryInfo.GetFiles())
      {
        Console.WriteLine("file name is =" + file.Name);
        Console.WriteLine(file.FullName + " {0}", (object) DateTime.Now);
        file.CopyTo(Path.Combine(dstPath, file.Name), true);
      }
    }

    internal static void CreateZipFile(
      DirectoryInfo dir,
      string destinationFolder,
      string installDir)
    {
      Console.WriteLine("Creating zip file  :::::::  {0}", (object) DateTime.Now);
      string currentDirectory = Environment.CurrentDirectory;
      Environment.CurrentDirectory = dir.FullName;
      string str = Path.Combine(destinationFolder, "BlueStacks-Support.7z");
      try
      {
        LogHelper.RunCmd(Path.Combine(installDir, "7zr.exe"), "a archive.zip -m0=LZMA:a=2 *", (string) null);
        if (File.Exists(str))
          File.Delete(str);
        File.Move("archive.zip", str);
        Console.WriteLine("Zip at {0}", (object) str);
      }
      finally
      {
        Environment.CurrentDirectory = currentDirectory;
        try
        {
          if (dir.Exists)
            Directory.Delete(dir.FullName, true);
        }
        catch (Exception ex)
        {
          Logger.Warning("Exception while deleting dir in CreateZipFile " + ex.ToString());
        }
      }
    }

    internal static void CopyAllAppsJsonFiles(DirectoryInfo tmpDir, string oem)
    {
      foreach (string vm in RegistryManager.RegistryManagers[oem].VmList)
      {
        string path2 = "apps_" + vm + ".json";
        string str = Path.Combine(Path.Combine(Path.Combine(RegistryManager.RegistryManagers[oem].DataDir, "UserData"), "Gadget"), path2);
        string destFileName = Path.Combine(tmpDir.FullName, path2);
        try
        {
          if (File.Exists(str))
            File.Copy(str, destFileName);
        }
        catch (Exception ex)
        {
          Console.WriteLine("Failed to copy apps json file : " + path2);
          Console.WriteLine(ex.ToString());
        }
      }
    }

    internal static void DumpServiceInfo()
    {
      try
      {
        using (EventWaitHandle eventWaitHandle = EventWaitHandle.OpenExisting("Global\\BlueStacks_Core_Service_Info_Event", EventWaitHandleRights.Modify))
        {
          eventWaitHandle.Set();
          Thread.Sleep(2000);
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine("Cannot dump service info: :::::::  {0}", (object) DateTime.Now);
      }
    }

    internal static void CollectVmLogs(DirectoryInfo tmpDir, string oem)
    {
      try
      {
        foreach (string vm in RegistryManager.RegistryManagers[oem].VmList)
        {
          if (Directory.Exists(Path.Combine(RegistryManager.RegistryManagers[oem].DataDir, vm + "\\Logs")))
            LogHelper.CopyRecursive(Path.Combine(RegistryManager.RegistryManagers[oem].DataDir, vm + "\\Logs"), Path.Combine(tmpDir.FullName, vm + "\\Logs"));
        }
        if (Directory.Exists(Path.Combine(RegistryManager.RegistryManagers[oem].DataDir, "Manager")))
          LogHelper.CopyRecursive(Path.Combine(RegistryManager.RegistryManagers[oem].DataDir, "Manager"), Path.Combine(tmpDir.FullName, "Manager"));
      }
      catch (Exception ex)
      {
        Logger.Error(ex.ToString());
      }
      try
      {
        foreach (string vm in RegistryManager.RegistryManagers[oem].VmList)
        {
          string str1 = Path.Combine(RegistryManager.RegistryManagers[oem].DataDir, vm + "\\" + vm + ".bstk");
          string str2 = Path.Combine(RegistryManager.RegistryManagers[oem].DataDir, vm + "\\" + vm + ".bstk-prev");
          string str3 = Path.Combine(RegistryManager.RegistryManagers[oem].DataDir, vm + "\\Android.json");
          string str4 = Path.Combine(RegistryManager.RegistryManagers[oem].DataDir, vm + "\\Android.Endpoint.json");
          string str5 = Path.Combine(RegistryManager.RegistryManagers[oem].DataDir, vm + "\\Android.Network.json");
          if (!Directory.Exists(Path.Combine(tmpDir.FullName, vm)))
            Directory.CreateDirectory(Path.Combine(tmpDir.FullName, vm));
          if (File.Exists(str1))
            File.Copy(str1, Path.Combine(tmpDir.FullName, vm + "\\" + vm + ".bstk"));
          if (File.Exists(str2))
            File.Copy(str2, Path.Combine(tmpDir.FullName, vm + "\\" + vm + ".bstk-prev"));
          if (File.Exists(str3))
            File.Copy(str3, Path.Combine(tmpDir.FullName, vm + "\\Android.json"));
          if (File.Exists(str4))
            File.Copy(str4, Path.Combine(tmpDir.FullName, vm + "\\Android.Endpoint.json"));
          if (File.Exists(str5))
            File.Copy(str5, Path.Combine(tmpDir.FullName, vm + "\\Android.Network.json"));
        }
      }
      catch (Exception ex)
      {
        Logger.Error(ex.ToString());
      }
      Console.WriteLine("CollectVmLogs end");
    }

    internal static void CollectBlueStacksGPArtifactsIfExists(
      DirectoryInfo tempDir,
      string oem,
      ref string mSource)
    {
      try
      {
        string clientInstallDir = RegistryManager.RegistryManagers[oem].ClientInstallDir;
        if (!string.IsNullOrEmpty(clientInstallDir))
        {
          string str1 = Path.Combine(clientInstallDir, "bst_config");
          if (File.Exists(str1))
            File.Copy(str1, Path.Combine(tempDir.FullName, "bst_config"), true);
          string str2 = Path.Combine(clientInstallDir, "logCollectorSourceData.txt");
          if (!File.Exists(str2))
            return;
          try
          {
            string str3 = File.ReadAllText(str2);
            if (string.IsNullOrEmpty(str3))
              return;
            mSource = str3.Substring(0, str3.Length >= 500 ? 500 : str3.Length);
            File.Move(str2, Path.Combine(tempDir.FullName, "logCollectorSourceData.txt"));
          }
          catch (Exception ex)
          {
            Logger.Error("Error in handling logCollectorSourceData ex:" + ex?.ToString());
          }
        }
        else
          Logger.Info("Bgp client does not exist.");
      }
      catch (Exception ex)
      {
        Console.WriteLine("got exception in CollectBlueStacksGpArtifacts {0}", (object) ex.ToString());
      }
    }

    internal static void CollectUserCfgFilesIfPresent(DirectoryInfo tmpDir, string oem)
    {
      try
      {
        string path = Path.Combine(Path.Combine(Path.Combine(RegistryManager.RegistryManagers[oem].DataDir, "UserData"), "InputMapper"), "UserFiles");
        if (Directory.GetFiles(path).Length != 0)
        {
          Console.WriteLine("Copying user cfg files :::::::  {0}", (object) DateTime.Now);
          string str = Path.Combine(tmpDir.FullName, "UserCfg");
          if (!Directory.Exists(str))
            Directory.CreateDirectory(str);
          foreach (string file in Directory.GetFiles(path))
          {
            FileInfo fileInfo = new FileInfo(file);
            File.Copy(fileInfo.FullName, Path.Combine(str, fileInfo.Name), true);
          }
        }
        else
          Console.WriteLine("No userfiles cfg is present");
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
      }
    }
  }
}
