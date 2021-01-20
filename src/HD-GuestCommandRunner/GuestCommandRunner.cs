// Decompiled with JetBrains decompiler
// Type: BlueStacks.GuestCommandRunner.GuestCommandRunner
// Assembly: HD-GuestCommandRunner, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 899CB498-70B0-44E2-A8EB-5E2DBF0FFF50
// Assembly location: C:\Program Files\BlueStacks\HD-GuestCommandRunner.exe

using BlueStacks.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace BlueStacks.GuestCommandRunner
{
  internal class GuestCommandRunner
  {
    private static string s_RunAppFileName = "HD-RunApp.exe";

    private static void Main(string[] args)
    {
      Mutex lck = (Mutex) null;
      bool flag = false;
      Strings.CurrentDefaultVmName = "Android";
      int num1 = ProcessUtils.CheckAlreadyRunningAndTakeLock(Strings.GetPlayerLockName(Strings.CurrentDefaultVmName, "bgp"), out lck) ? 1 : 0;
      lck?.Close();
      if (num1 == 0 && BlueStacks.GuestCommandRunner.GuestCommandRunner.LaunchBlueStacks())
      {
        flag = true;
        Console.WriteLine("Successfully Launched AppPlayer");
      }
      lck?.Close();
      if (!BlueStacks.GuestCommandRunner.GuestCommandRunner.CheckIfGuestStarted())
        Environment.Exit(-1);
      AdbCommandRunner adbCommandRunner = new AdbCommandRunner(Strings.CurrentDefaultVmName);
      for (int index = 0; index < VMCommand.COMMAND.Length; ++index)
      {
        if (!string.IsNullOrEmpty(VMCommand.COMMAND[index]) && string.Compare(VmCmdHandler.RunCommand(VMCommand.COMMAND[index], Strings.CurrentDefaultVmName, "bgp"), "ok", true) != 0)
        {
          if (Oem.Instance.IsHideMessageBoxIconInTaskBar)
          {
            int num2 = (int) MessageBox.Show((IWin32Window) new Form(), "Error, Something went wrong!");
          }
          else
          {
            int num3 = (int) MessageBox.Show("Error, Something went wrong!");
          }
          adbCommandRunner.Dispose();
          Environment.Exit(-1);
        }
      }
      adbCommandRunner.Dispose();
      Thread.Sleep(2000);
      BlueStacks.GuestCommandRunner.GuestCommandRunner.StopZygote();
      BlueStacks.GuestCommandRunner.GuestCommandRunner.StartZygote();
      if (!flag)
        return;
      BlueStacks.GuestCommandRunner.GuestCommandRunner.ExitBlueStacks();
    }

    private static void ExitBlueStacks()
    {
      try
      {
        Utils.StopFrontend(Strings.CurrentDefaultVmName, true);
      }
      catch (Exception ex)
      {
        Console.WriteLine("Exception while exiting Bluestacks AppPlayer");
        Console.WriteLine(ex.ToString());
      }
    }

    private static void StopZygote()
    {
      BstHttpClient.Post(string.Format("http://127.0.0.1:{0}/stopzygote", (object) RegistryManager.Instance.DefaultGuest.FrontendServerPort), new Dictionary<string, string>()
      {
        {
          "vmName",
          Strings.CurrentDefaultVmName
        }
      }, (Dictionary<string, string>) null, false, Strings.CurrentDefaultVmName, 0, 1, 0, false, "bgp");
      Thread.Sleep(100);
    }

    private static void StartZygote()
    {
      BstHttpClient.Post(string.Format("http://127.0.0.1:{0}/startzygote", (object) RegistryManager.Instance.DefaultGuest.FrontendServerPort), new Dictionary<string, string>()
      {
        {
          "vmName",
          Strings.CurrentDefaultVmName
        }
      }, (Dictionary<string, string>) null, false, Strings.CurrentDefaultVmName, 0, 1, 0, false, "bgp");
    }

    private static bool CheckIfGuestStarted()
    {
      try
      {
        if (VmCmdHandler.RunCommand("ping", Strings.CurrentDefaultVmName, "bgp") == null)
          return false;
      }
      catch (Exception ex)
      {
        Console.WriteLine(string.Format("Error Occured, Err: {0}", (object) ex.ToString()));
        return false;
      }
      return true;
    }

    private static bool LaunchBlueStacks()
    {
      try
      {
        string str = Path.Combine(RegistryStrings.InstallDir, BlueStacks.GuestCommandRunner.GuestCommandRunner.s_RunAppFileName);
        Console.WriteLine(string.Format("path to start launcher = {0}", (object) str));
        Process process = new Process();
        process.StartInfo.FileName = str;
        process.StartInfo.Arguments = "-h";
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;
        process.Start();
        process.WaitForExit();
        return true;
      }
      catch (Exception ex)
      {
        Console.WriteLine(string.Format("Error Occured, Err: {0}", (object) ex.ToString()));
        return false;
      }
    }
  }
}
