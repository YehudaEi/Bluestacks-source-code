// Decompiled with JetBrains decompiler
// Type: BlueStacks.Common.AdbCommandRunner
// Assembly: HD-Common, Version=4.250.0.1070, Culture=neutral, PublicKeyToken=null
// MVID: 7033AB66-5028-4A08-B35C-D9B2B424A68A
// Assembly location: C:\Program Files\BlueStacks\HD-Common.dll

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading;

namespace BlueStacks.Common
{
  public class AdbCommandRunner : IDisposable
  {
    private static string GUEST_URL = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "http://127.0.0.1:{0}", (object) RegistryManager.Instance.Guest["Android"].BstAndroidPort);
    private string mVmName = "Android";
    private const string SU_PATH = "/system/xbin/bstk/su";
    private bool disposedValue;

    public int Port { get; set; }

    public string Path { get; set; }

    public bool IsHostConnected { get; set; }

    public AdbCommandRunner(string vmName = "Android")
    {
      if (!string.IsNullOrEmpty(vmName))
      {
        this.mVmName = vmName;
        AdbCommandRunner.GUEST_URL = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "http://127.0.0.1:{0}", (object) RegistryManager.Instance.Guest[this.mVmName].BstAndroidPort);
      }
      this.IsHostConnected = this.EnableADB();
    }

    private static bool CheckIfGuestCommandSuccess(string res)
    {
      string a = JObject.Parse(res)["result"].ToString().Trim();
      if (string.Equals(a, "ok", StringComparison.InvariantCultureIgnoreCase))
        return true;
      Logger.Error("result: {0}", (object) a);
      return false;
    }

    private bool EnableADB()
    {
      return this.HitGuestAPI("connectHost");
    }

    private bool DisableADB()
    {
      return this.HitGuestAPI("disconnectHost");
    }

    private bool HitGuestAPI(string api)
    {
      try
      {
        return AdbCommandRunner.CheckIfGuestCommandSuccess(HTTPUtils.SendRequestToGuest(api, (Dictionary<string, string>) null, this.mVmName, 0, (Dictionary<string, string>) null, false, 1, 0, "bgp"));
      }
      catch (Exception ex)
      {
        Logger.Error("Error in Sending request {0} to guest {1}", (object) api, (object) ex.ToString());
      }
      return false;
    }

    public AdbCommandRunner.OutputLineHandlerDelegate OutputLineHandler { get; set; }

    public bool Connect(string vmName)
    {
      this.Port = RegistryManager.Instance.Guest[vmName].BstAdbPort;
      this.Path = System.IO.Path.Combine(RegistryStrings.InstallDir, "HD-Adb.exe");
      if (!this.RunInternal(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "connect 127.0.0.1:{0}", (object) this.Port), true))
        return false;
      this.RunInternal("devices", true);
      return true;
    }

    private bool RunInternal(string cmd, bool retry = true)
    {
      Logger.Info("ADB CMD: " + cmd);
      using (Process process = new Process())
      {
        process.StartInfo.FileName = this.Path;
        process.StartInfo.Arguments = cmd;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.CreateNoWindow = true;
        process.OutputDataReceived += (DataReceivedEventHandler) ((sender, evt) => Logger.Info("ADB OUT: " + evt.Data));
        process.ErrorDataReceived += (DataReceivedEventHandler) ((sender, evt) =>
        {
          if (string.IsNullOrEmpty(evt.Data))
            return;
          Logger.Info("ERR: " + evt.Data);
        });
        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        process.WaitForExit();
        int num = process.ExitCode;
        Logger.Info("ADB EXIT: " + num.ToString());
        if (num != 0 && retry)
        {
          Logger.Info("ADB retring");
          Thread.Sleep(4000);
          num = this.RunInternal(cmd, false) ? 0 : 1;
        }
        return num == 0;
      }
    }

    public bool Push(string localPath, string remotePath)
    {
      Logger.Info("Pushing {0} to {1}", (object) localPath, (object) remotePath);
      return this.RunInternal(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "-s 127.0.0.1:{0} push \"{1}\" \"{2}\"", (object) this.Port, (object) localPath, (object) remotePath), true);
    }

    public bool Pull(string filePath, string destPath)
    {
      Logger.Info("Pull file {0} in {1}", (object) filePath, (object) destPath);
      return this.RunInternal(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "-s 127.0.0.1:{0} pull \"{1}\" \"{2}\"", (object) this.Port, (object) filePath, (object) destPath), true);
    }

    public bool RunShell(string fmt, params object[] args)
    {
      return this.RunShell(string.Format((IFormatProvider) CultureInfo.InvariantCulture, fmt, args));
    }

    public bool RunShell(string cmd)
    {
      Logger.Info("RunShell: " + cmd);
      return this.RunInternal(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "-s 127.0.0.1:{0} shell {1}", (object) this.Port, (object) cmd), true);
    }

    public bool RunShellScript(string[] cmdList)
    {
      if (cmdList != null)
      {
        foreach (string cmd in cmdList)
        {
          if (!this.RunShell(cmd))
            return false;
        }
      }
      return true;
    }

    public bool RunShellPrivileged(string fmt, params object[] cmd)
    {
      return this.RunShellPrivileged(string.Format((IFormatProvider) CultureInfo.InvariantCulture, fmt, cmd));
    }

    public bool RunShellPrivileged(string cmd)
    {
      Logger.Info("RunShellPrivileged: " + cmd);
      return this.RunInternal(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "-s 127.0.0.1:{0} shell {1} -c {2}", (object) this.Port, (object) "/system/xbin/bstk/su", (object) cmd), true);
    }

    public bool RunShellScriptPrivileged(string[] cmdList)
    {
      if (cmdList != null)
      {
        foreach (string cmd in cmdList)
        {
          if (!this.RunShellPrivileged(cmd))
            return false;
        }
      }
      return true;
    }

    protected virtual void Dispose(bool disposing)
    {
      if (this.disposedValue)
        return;
      int num = disposing ? 1 : 0;
      this.DisableADB();
      this.disposedValue = true;
    }

    ~AdbCommandRunner()
    {
      this.Dispose(false);
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    public delegate void OutputLineHandlerDelegate(string line);
  }
}
