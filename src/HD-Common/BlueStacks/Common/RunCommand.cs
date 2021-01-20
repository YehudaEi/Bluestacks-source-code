// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.RunCommand
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using System.Diagnostics;

namespace BlueStacks.Common
{
  public static class RunCommand
  {
    public static CmdRes RunCmd(
      string cmd,
      string args,
      bool addOutputToLogs = false,
      bool addCommandToLogs = true,
      bool requireAdministrator = false,
      int timeout = 0)
    {
      if (addCommandToLogs)
      {
        Logger.Info("RunCmd: starting cmd: " + cmd);
        Logger.Info("  args: " + args);
      }
      CmdRes res = new CmdRes();
      using (Process proc = new Process())
      {
        proc.StartInfo.FileName = cmd;
        proc.StartInfo.Arguments = args;
        proc.StartInfo.UseShellExecute = false;
        proc.StartInfo.CreateNoWindow = true;
        if (requireAdministrator)
          proc.StartInfo.Verb = "runas";
        proc.StartInfo.RedirectStandardInput = true;
        proc.StartInfo.RedirectStandardOutput = true;
        proc.StartInfo.RedirectStandardError = true;
        proc.OutputDataReceived += (DataReceivedEventHandler) ((sender, line) =>
        {
          string str = line.Data?.Trim();
          if (string.IsNullOrEmpty(str))
            return;
          if (addOutputToLogs)
            Logger.Info(proc.Id.ToString() + " OUT: " + str);
          CmdRes cmdRes = res;
          cmdRes.StdOut = cmdRes.StdOut + str + "\n";
        });
        proc.ErrorDataReceived += (DataReceivedEventHandler) ((sender, line) =>
        {
          if (addOutputToLogs)
            Logger.Info(proc.Id.ToString() + " ERR: " + line.Data);
          CmdRes cmdRes = res;
          cmdRes.StdErr = cmdRes.StdErr + line.Data + "\n";
        });
        proc.Start();
        proc.BeginOutputReadLine();
        proc.BeginErrorReadLine();
        bool flag = true;
        if (timeout == 0)
          proc.WaitForExit();
        else
          flag = proc.WaitForExit(timeout);
        proc.CancelOutputRead();
        proc.CancelErrorRead();
        if (addOutputToLogs & flag)
        {
          Logger.Info("RunCmd for proc.ID = " + proc.Id.ToString() + " exited with exitCode: " + proc.ExitCode.ToString());
          res.ExitCode = proc.ExitCode;
        }
        if (!flag)
        {
          Logger.Fatal("RunCmd for proc.ID = {0} terminated after timeout of {1}", (object) proc.Id, (object) timeout);
          res.ExitCode = -1;
        }
      }
      return res;
    }
  }
}
