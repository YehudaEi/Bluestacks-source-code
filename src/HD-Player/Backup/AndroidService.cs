// Decompiled with JetBrains decompiler
// Type: BlueStacks.Player.AndroidService
// Assembly: HD-Player, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7E2636C4-7B08-4EB4-9C45-ADCCA898CA6B
// Assembly location: C:\Program Files\BlueStacks\HD-Player.exe

using BlueStacks.Common;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;

namespace BlueStacks.Player
{
  public class AndroidService
  {
    private static AndroidService mAndroidService;
    internal static Thread androidThread;
    internal static EventWaitHandle HDQuitEvent;
    private string mVmName;
    private bool mConsoleMode;
    private StateMachine mStateMachine;
    private const int BstAndroidPort = 9999;
    private const int BstPort5555 = 5555;
    private const int BstPort6666 = 6666;
    private const int BstPort7777 = 7777;

    internal static AndroidService Instance
    {
      get
      {
        return AndroidService.mAndroidService;
      }
    }

    public static void StartAsync()
    {
      AndroidService.androidThread = new Thread((ThreadStart) (() =>
      {
        if (!Utils.CheckIfAndroidBstkExistAndValid(Strings.CurrentDefaultVmName))
          Utils.CreateBstkFileFromPrev(MultiInstanceStrings.VmName);
        ServiceManager.StartService(Strings.BlueStacksDriverName, true);
        ServiceHelper.FindAndSyncConfig();
        MonitorLocator.Publish(Strings.CurrentDefaultVmName, (uint) Process.GetCurrentProcess().Id);
        AndroidService.mAndroidService = new AndroidService(true, Strings.CurrentDefaultVmName);
        ThreadPool.QueueUserWorkItem((WaitCallback) (stateInfo =>
        {
          AndroidService.HDQuitEvent = new EventWaitHandle(false, EventResetMode.AutoReset, string.Format("HD-Plus-{0}", (object) Process.GetCurrentProcess().Id));
          AndroidService.HDQuitEvent.WaitOne();
          AndroidService.mAndroidService.OnStop();
        }));
        AndroidService.mAndroidService.RunDebug();
      }))
      {
        IsBackground = true
      };
      AndroidService.androidThread.Start();
    }

    internal void CloseService()
    {
      if (!AndroidBootUp.isAndroidBooted)
        return;
      AndroidService.HDQuitEvent.Set();
      this.mStateMachine.WaitForTermination();
      Logger.Info("Finally vbox service stopped");
    }

    public static AndroidService.CmdRes RunCmd(
      string cmd,
      string args,
      bool throwOnNonZero)
    {
      Logger.Info("Command Runner");
      Logger.Info("  cmd:  " + cmd);
      Logger.Info("  args: " + args);
      AndroidService.CmdRes res = new AndroidService.CmdRes();
      using (Process proc = new Process())
      {
        proc.StartInfo.FileName = cmd;
        proc.StartInfo.Arguments = args;
        proc.StartInfo.UseShellExecute = false;
        proc.StartInfo.CreateNoWindow = true;
        proc.StartInfo.RedirectStandardInput = true;
        proc.StartInfo.RedirectStandardOutput = true;
        proc.StartInfo.RedirectStandardError = true;
        proc.OutputDataReceived += (DataReceivedEventHandler) ((sender, line) =>
        {
          string data = line.Data;
          string str;
          if (data == null || !((str = data.Trim()) != string.Empty))
            return;
          Logger.Info(proc.Id.ToString() + " OUT: " + str);
          AndroidService.CmdRes cmdRes = res;
          cmdRes.StdOut = cmdRes.StdOut + str + "\n";
        });
        proc.ErrorDataReceived += (DataReceivedEventHandler) ((sender, line) =>
        {
          Logger.Info(proc.Id.ToString() + " ERR: " + line.Data);
          AndroidService.CmdRes cmdRes = res;
          cmdRes.StdErr = cmdRes.StdErr + line.Data + "\n";
        });
        proc.Start();
        proc.BeginOutputReadLine();
        proc.BeginErrorReadLine();
        proc.WaitForExit();
        int num = proc.Id;
        string str1 = num.ToString();
        num = proc.ExitCode;
        string str2 = num.ToString();
        Logger.Info(str1 + " EXIT: " + str2);
        res.ExitCode = proc.ExitCode;
        if (throwOnNonZero)
        {
          if (proc.ExitCode != 0)
          {
            if ((int) Registry.LocalMachine.OpenSubKey("Software").GetValue("IgnoreError", (object) 0) == 0)
            {
              num = proc.ExitCode;
              throw new ApplicationException("Command  returned exit code " + num.ToString());
            }
          }
        }
      }
      return res;
    }

    private AndroidService(bool consoleMode, string vmName)
    {
      this.mConsoleMode = consoleMode;
      this.mStateMachine = new StateMachine(vmName);
      this.mVmName = vmName;
    }

    private void RunDebug()
    {
      ConsoleControl.SetHandler((ConsoleControl.Handler) (ctrl =>
      {
        if (ctrl != CtrlType.CTRL_C_EVENT)
          return false;
        this.mStateMachine.RequestTermination();
        return true;
      }));
      this.OnStart();
      Logger.Info("Waiting for signal from user");
      this.mStateMachine.WaitForTermination();
      this.OnStop();
    }

    protected void OnStart()
    {
      try
      {
        Logger.Info("Starting {0} VM", (object) this.mVmName);
        Utils.SetCurrentEngineStateAndGlTransportValue(EngineState.plus, MultiInstanceStrings.VmName);
        this.OnStartInternal();
      }
      catch (Exception ex)
      {
        Logger.Info("Exception in OnStart()");
        Logger.Info(ex.ToString());
        throw;
      }
    }

    private void OnStartInternal()
    {
      EventWaitHandle waitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
      bool success = true;
      this.mStateMachine.RunningCallback = (StateMachine.VoidCallback) (() => this.ApplyNetworkRedirects());
      this.mStateMachine.Start((StateMachine.BooleanCallback) (callbackSuccess =>
      {
        Logger.Info("State machine has started -> {0}", (object) callbackSuccess);
        success = callbackSuccess;
        waitHandle.Set();
      }));
      waitHandle.WaitOne();
      if (!success)
        throw new ApplicationException("Cannot start state machine");
    }

    protected void OnStop()
    {
      try
      {
        Logger.Info("Stopping {0} VM", (object) this.mVmName);
        this.OnStopInternal();
        Logger.Info("{0} VM is stopped", (object) this.mVmName);
      }
      catch (Exception ex)
      {
        Logger.Error("Exception in OnStop()");
        Logger.Error(ex.ToString());
        throw;
      }
    }

    private void OnStopInternal()
    {
      this.mStateMachine.RequestTermination();
      this.mStateMachine.WaitForTermination();
    }

    private bool ApplyNetworkRedirects()
    {
      string[] networkInboundRules = RegistryManager.Instance.DefaultGuest.NetworkInboundRules;
      if (networkInboundRules == null)
      {
        Logger.Info("Cannot get network inbound rules");
        return false;
      }
      Logger.Info("Applying network rules:");
      foreach (string str in networkInboundRules)
      {
        string[] strArray = str.Split(':');
        if (strArray.Length != 3)
        {
          Logger.Info("Cannot parse rule: " + str);
        }
        else
        {
          int guestPort = int.Parse(strArray[1]);
          int num1 = int.Parse(strArray[2]);
          int num2 = num1 + 10;
          while (num1 < num2 && this.IsPortInUse(num1))
            ++num1;
          Logger.Info("the port number tried is " + num1.ToString());
          VBoxBridgeService.Instance.AddNetworkRedirect(strArray[0] == "tcp", guestPort, num1);
          this.UpdateNetworkChangePortRedirectInfo(guestPort, num1);
        }
      }
      return true;
    }

    private void UpdateNetworkChangePortRedirectInfo(int guestPort, int hostPort)
    {
      switch (guestPort)
      {
        case 5555:
          Logger.Info("Bst 5555 Port Updated to {0}", (object) hostPort);
          RegistryManager.Instance.DefaultGuest.BstAdbPort = hostPort;
          RegistryManager.Instance.DefaultGuest.NetworkRedirectTcp5555 = hostPort;
          break;
        case 6666:
          Logger.Info("Bst 6666 Port Updated to {0}", (object) hostPort);
          RegistryManager.Instance.DefaultGuest.NetworkRedirectTcp6666 = hostPort;
          break;
        case 7777:
          Logger.Info("Bst 7777 Port Updated to {0}", (object) hostPort);
          RegistryManager.Instance.DefaultGuest.NetworkRedirectTcp7777 = hostPort;
          break;
        case 9999:
          Logger.Info("Bst Android Port Updated to {0}", (object) hostPort);
          RegistryManager.Instance.DefaultGuest.BstAndroidPort = hostPort;
          RegistryManager.Instance.DefaultGuest.NetworkRedirectTcp9999 = hostPort;
          break;
      }
    }

    private bool IsPortInUse(int port)
    {
      bool flag = false;
      foreach (IPEndPoint activeTcpListener in IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners())
      {
        if (activeTcpListener.Port == port)
        {
          Logger.Info("port {0} is in use", (object) port);
          flag = true;
          break;
        }
      }
      return flag;
    }

    public class CmdRes
    {
      public string StdOut = "";
      public string StdErr = "";
      public int ExitCode;
    }
  }
}
